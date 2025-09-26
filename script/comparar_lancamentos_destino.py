import pandas as pd
import numpy as np
import re
import logging
from pathlib import Path
from datetime import datetime, timedelta
from unidecode import unidecode

# Tente usar fuzzy matching apenas se disponível (para casos sem cadastro)
try:
    from rapidfuzz import process, fuzz
    HAS_RAPIDFUZZ = True
except Exception:
    HAS_RAPIDFUZZ = False

# ------------- CONFIGURACAO BÁSICA -------------
ARQ_ORIGEM = "copia de lancamentos.xlsx"
ARQ_DESTINO = "contas a pagar250925.xlsx"
ARQ_FORNEC = "fornecedor.xlsx"
SAIDA_XLSX = "comparacao_resultado.xlsx"
LOG_TXT = "divergencias_log.txt"

# ------------- LOGGING -------------
logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(message)s",
    handlers=[logging.FileHandler(LOG_TXT, mode="w", encoding="utf-8"), logging.StreamHandler()]
)

# ------------- AJUDANTES -------------
def norm_str(x: str) -> str:
    if pd.isna(x):
        return ""
    s = str(x).strip()
    s = unidecode(s)  # remove acentos
    s = s.lower()
    # remove pontuação e caracteres que atrapalham a comparação de nomes/strings livres
    s = re.sub(r"[^a-z0-9\s]", " ", s)
    s = re.sub(r"\s+", " ", s).strip()
    return s

def norm_hist(x: str) -> str:
    # histórico padronizado; mantém dígitos/letras, remove excesso
    if pd.isna(x) or str(x).strip() == "":
        return ""
    s = unidecode(str(x)).lower().strip()
    s = re.sub(r"[^a-z0-9\s\-_/\.]", " ", s)
    s = re.sub(r"\s+", " ", s).strip()
    return s

def parse_excel_date(v):
    # trata datas como:
    # - pd.Timestamp / datetime
    # - string (dd/mm/yyyy, yyyy-mm-dd, etc.)
    # - serial do Excel (número)
    if pd.isna(v):
        return pd.NaT
    # já é datetime?
    if isinstance(v, (pd.Timestamp, datetime)):
        return pd.to_datetime(v).normalize()
    # numérico: possivelmente serial do Excel
    if isinstance(v, (int, float)) and not isinstance(v, bool):
        # Excel usa base 1899-12-30
        try:
            base = datetime(1899, 12, 30)
            return (base + timedelta(days=float(v))).date()
        except Exception:
            pass
    # string
    s = str(v).strip()
    if not s:
        return pd.NaT
    # tenta vários formatos
    try:
        # dayfirst para datas brasileiras
        dt = pd.to_datetime(s, dayfirst=True, errors="coerce")
        if pd.isna(dt):
            return pd.NaT
        return dt.normalize()
    except Exception:
        return pd.NaT

def fmt_date(d):
    if pd.isna(d):
        return ""
    try:
        d = pd.to_datetime(d)
        return d.strftime("%d/%m/%Y")
    except Exception:
        return ""

def parse_valor_br(v):
    # aceita float, int, string com vírgula decimal, parênteses para negativo, etc.
    if pd.isna(v):
        return np.nan
    if isinstance(v, (int, float)) and not isinstance(v, bool):
        return round(float(v), 2)
    s = str(v).strip()
    if not s:
        return np.nan
    # negativo entre parênteses
    neg = False
    if s.startswith("(") and s.endswith(")"):
        neg = True
        s = s[1:-1]
    # remove currency e espaços
    s = re.sub(r"[^\d,.-]", "", s)
    # se tiver mais de um separador, normaliza: pontos como milhar e vírgula como decimal
    if s.count(",") == 1 and s.count(".") >= 1:
        s = s.replace(".", "")
    # troca vírgula por ponto
    s = s.replace(",", ".")
    try:
        val = float(s)
        if neg:
            val = -val
        return round(val, 2)
    except Exception:
        return np.nan

def build_parc_from_parts(parcela, total):
    try:
        p = int(float(parcela)) if not pd.isna(parcela) else None
    except Exception:
        p = None
    try:
        t = int(float(total)) if not pd.isna(total) else None
    except Exception:
        t = None
    if p and t:
        return f"{p}/{t}"
    return ""

def parse_parc_str(s):
    if pd.isna(s):
        return ""
    s = str(s).strip()
    if not s:
        return ""
    # pega padrão X/Y
    m = re.search(r"(\d+)\s*/\s*(\d+)", s)
    if m:
        x, y = m.group(1), m.group(2)
        return f"{int(x)}/{int(y)}"
    # se tiver só um número, usa como parcela e ignora total
    m2 = re.search(r"(\d+)", s)
    if m2:
        return f"{int(m2.group(1))}/"
    return ""

def find_col(df: pd.DataFrame, candidates):
    cols = list(df.columns)
    norm_map = {c: norm_str(c) for c in cols}
    cand_norm = [norm_str(c) for c in candidates]
    # 1) match exato normalizado
    for c, n in norm_map.items():
        if n in cand_norm:
            return c
    # 2) match por inclusão
    for c, n in norm_map.items():
        for cn in cand_norm:
            if cn and cn in n:
                return c
    return None

def detect_columns_origem(df: pd.DataFrame):
    col_credor = find_col(df, ["credor", "fornecedor", "razao social", "nome", "razao_social"])
    col_venc = find_col(df, ["vencimento", "venc", "vcto", "vencto", "data vencimento", "dt venc"])
    col_valor = find_col(df, ["valor", "vlr", "valor titulo", "valor documento"])
    col_parcela = find_col(df, ["parcela", "n parcela", "nr parcela"])
    col_total_parcelas = find_col(df, ["totalparcelas", "total parcelas", "qtd parcelas", "qtde parcelas"])
    col_hist = find_col(df, ["historico", "histórico", "hist", "descricao", "descrição"])
    # cnpj pode ou não existir na origem
    col_cnpj = find_col(df, ["cnpj", "cpf/cnpj", "cpf cnpj", "doc", "documento"])
    return dict(credor=col_credor, venc=col_venc, valor=col_valor,
                parcela=col_parcela, totalparcelas=col_total_parcelas,
                historico=col_hist, cnpj=col_cnpj)

def detect_columns_destino(df: pd.DataFrame):
    col_credor = find_col(df, ["credor", "fornecedor", "razao social", "nome", "razao_social"])
    col_venc = find_col(df, ["vencimento", "venc", "vcto", "vencto", "data vencimento", "dt venc"])
    col_valor = find_col(df, ["valor", "vlr", "valor titulo", "valor documento"])
    col_parc = find_col(df, ["parc", "parcela", "parc.", "x/y", "parcelas"])
    col_hist = find_col(df, ["historico", "histórico", "hist"])
    col_cnpj = find_col(df, ["cnpj", "cpf/cnpj", "cpf cnpj"])
    return dict(credor=col_credor, venc=col_venc, valor=col_valor,
                parc=col_parc, historico=col_hist, cnpj=col_cnpj)

def detect_columns_fornecedor(df: pd.DataFrame):
    col_nome = find_col(df, ["razao social", "fornecedor", "nome", "razao_social", "razao", "empresa"])
    col_cnpj = find_col(df, ["cnpj", "cpf/cnpj", "cpf cnpj"])
    col_alias = find_col(df, ["aliases", "apelidos", "sinonimos", "sinônimos", "aka", "variacoes", "variacoes de nome"])
    return dict(nome=col_nome, cnpj=col_cnpj, alias=col_alias)

def build_fornecedor_map(df_f: pd.DataFrame):
    cols = detect_columns_fornecedor(df_f)
    logging.info(f"[FORNECEDOR] Colunas detectadas: {cols}")
    nome_col = cols["nome"]
    cnpj_col = cols["cnpj"]
    alias_col = cols["alias"]

    if not nome_col or not cnpj_col:
        logging.warning("[FORNECEDOR] Não foi possível detectar colunas obrigatórias de Nome e CNPJ. Mapeamento limitado.")
        return {}

    mapa = {}
    for _, row in df_f.iterrows():
        nome_bruto = row.get(nome_col, "")
        cnpj = str(row.get(cnpj_col, "")).strip()
        nome_canon = str(nome_bruto).strip()
        if not nome_canon:
            continue
        norm_nome = norm_str(nome_canon)
        if norm_nome:
            mapa[norm_nome] = (nome_canon, cnpj)
        # aliases separados por ;
        if alias_col:
            aliases_val = row.get(alias_col, "")
            if pd.notna(aliases_val) and str(aliases_val).strip():
                for al in str(aliases_val).split(";"):
                    al = al.strip()
                    if not al:
                        continue
                    norm_al = norm_str(al)
                    if norm_al and norm_al not in mapa:
                        mapa[norm_al] = (nome_canon, cnpj)
    logging.info(f"[FORNECEDOR] Entradas no mapa: {len(mapa)}")
    return mapa

def canoniza_credor(nome_raw: str, mapa_fornec: dict):
    norm = norm_str(nome_raw)
    if not norm:
        return ("", "")
    if norm in mapa_fornec:
        return mapa_fornec[norm]
    # fallback fuzzy se não tiver no mapa
    if HAS_RAPIDFUZZ and mapa_fornec:
        choices = list(mapa_fornec.keys())
        match, score, _ = process.extractOne(norm, choices, scorer=fuzz.token_sort_ratio)
        if score >= 92: # Threshold para canonização, pode ser diferente do match fuzzy principal
            return mapa_fornec[match]
    # sem correspondência confiável
    return ("", "")

def build_keys(df: pd.DataFrame, date_col: str, val_col: str, parc_col: str, hist_col: str, prefix=""):
    # cria colunas padronizadas
    df[f"{prefix}std_venc"] = df[date_col].apply(parse_excel_date) if date_col else pd.NaT
    df[f"{prefix}std_valor"] = df[val_col].apply(parse_valor_br) if val_col else np.nan
    if parc_col:
        df[f"{prefix}std_parc"] = df[parc_col].apply(parse_parc_str)
    else:
        df[f"{prefix}std_parc"] = ""
    if hist_col:
        df[f"{prefix}std_hist"] = df[hist_col].apply(norm_hist)
    else:
        df[f"{prefix}std_hist"] = ""

    # chaves
    df[f"{prefix}key1"] = list(zip(df[f"{prefix}std_venc"], df[f"{prefix}std_valor"], df[f"{prefix}std_parc"], df[f"{prefix}std_hist"]))
    df[f"{prefix}key2"] = list(zip(df[f"{prefix}std_venc"], df[f"{prefix}std_valor"], df[f"{prefix}std_parc"]))
    df[f"{prefix}key3"] = list(zip(df[f"{prefix}std_venc"], df[f"{prefix}std_valor"]))
    return df

def main():
    logging.info("Início do processamento")
    logging.info(f"Origem (cópia): {ARQ_ORIGEM}")
    logging.info(f"Destino (contas a pagar): {ARQ_DESTINO}")
    logging.info(f"Fornecedores: {ARQ_FORNEC}")
    logging.info(f"Saída: {SAIDA_XLSX}")
    logging.info(f"Log: {LOG_TXT}")

    # Carregar arquivos
    df_o = pd.read_excel(ARQ_ORIGEM)
    df_d = pd.read_excel(ARQ_DESTINO)
    df_f = pd.read_excel(ARQ_FORNEC)

    logging.info(f"[ORIGEM] Linhas: {len(df_o)}, Colunas: {len(df_o.columns)}")
    logging.info(f"[DESTINO] Linhas: {len(df_d)}, Colunas: {len(df_d.columns)}")
    logging.info(f"[FORNECEDOR] Linhas: {len(df_f)}, Colunas: {len(df_f.columns)}")

    # Detectar colunas
    cols_o = detect_columns_origem(df_o)
    cols_d = detect_columns_destino(df_d)
    logging.info(f"[ORIGEM] Colunas detectadas: {cols_o}")
    logging.info(f"[DESTINO] Colunas detectadas: {cols_d}")

    # Checagem mínima
    min_o = all([cols_o["venc"], cols_o["valor"], cols_o["parcela"], cols_o["totalparcelas"]])
    min_d = all([cols_d["venc"], cols_d["valor"], cols_d["parc"]])
    if not min_o:
        raise RuntimeError(f"[ORIGEM] Colunas mínimas ausentes. Necessário: vencimento, valor, parcela, totalparcelas. Detectado: {cols_o}")
    if not min_d:
        raise RuntimeError(f"[DESTINO] Colunas mínimas ausentes. Necessário: vencimento, valor, parc. Detectado: {cols_d}")

    # Mapa de fornecedores (nome canônico + CNPJ)
    mapa_fornec = build_fornecedor_map(df_f)

    # Preparar origem: monta 'Parc' = parcela/totalparcelas
    df_o = df_o.copy()
    df_o["__parc_montado__"] = df_o.apply(lambda r: build_parc_from_parts(r.get(cols_o["parcela"]), r.get(cols_o["totalparcelas"])), axis=1)

    # Canonizar credor origem
    col_o_credor = cols_o["credor"]
    df_o["__credor_raw__"] = df_o[col_o_credor] if col_o_credor else ""
    df_o["__credor_norm__"] = df_o["__credor_raw__"].apply(norm_str)
    df_o["Credor_Canonico"], df_o["CNPJ_Fornec"] = zip(*df_o["__credor_raw__"].apply(lambda x: canoniza_credor(x, mapa_fornec)))

    # Preparar destino
    col_d_credor = cols_d["credor"]
    df_d["__credor_raw__"] = df_d[col_d_credor] if col_d_credor else ""
    df_d["__credor_norm__"] = df_d["__credor_raw__"].apply(norm_str)
    # Canonizar credor destino
    df_d["Credor_Canonico"], df_d["CNPJ_Fornec"] = zip(*df_d["__credor_raw__"].apply(lambda x: canoniza_credor(x, mapa_fornec)))

    # Construir chaves
    df_o = build_keys(df_o, cols_o["venc"], cols_o["valor"], "__parc_montado__", cols_o["historico"], prefix="o_")
    df_d = build_keys(df_d, cols_d["venc"], cols_d["valor"], cols_d["parc"], cols_d["historico"], prefix="d_")

    # Contagens de valores faltantes
    logging.info(f"[ORIGEM] Datas nulas: {df_o['o_std_venc'].isna().sum()} | Valores nulos: {df_o['o_std_valor'].isna().sum()}")
    logging.info(f"[DESTINO] Datas nulas: {df_d['d_std_venc'].isna().sum()} | Valores nulos: {df_d['d_std_valor'].isna().sum()}")

    # Preparar subconjuntos para merge
    cols_keep_d = [
        "d_std_venc", "d_std_valor", "d_std_parc", "d_std_hist",
        "Credor_Canonico", "CNPJ_Fornec", "__credor_raw__", "__credor_norm__"
    ]
    # também manter as colunas originais úteis do destino
    poss_d_cols = [cols_d["venc"], cols_d["valor"], cols_d["parc"], cols_d["historico"], cols_d["credor"], cols_d["cnpj"]]
    for c in poss_d_cols:
        if c and c not in cols_keep_d:
            cols_keep_d.append(c) # Usar append para lista
    cols_keep_d_filtered = [c for c in cols_keep_d if c in df_d.columns or c in ["d_std_venc", "d_std_valor", "d_std_parc", "d_std_hist", "Credor_Canonico", "CNPJ_Fornec", "__credor_raw__", "__credor_norm__"]]


    # Merge progressivo
    o_base = df_o.copy()
    o_base["match_level"] = ""
    o_base["dest_row_id"] = pd.NA # Usar pd.NA para melhor tratamento de nulos em colunas Int64

    # Para guardar o índice do destino de forma consistente
    df_d_with_id = df_d.reset_index().rename(columns={"index": "dest_row_id"})

    # --- 1) key1 ---
    logging.info("Realizando merge com key1 (venc-valor-parc-hist)")
    # Garantir que o df do lado direito tenha chaves únicas para não duplicar linhas do lado esquerdo
    d1_unique = df_d_with_id.drop_duplicates(subset=["d_key1"])
    # O merge de o_base com d1_unique mantém o index original de o_base no resultado
    m1 = o_base.merge(d1_unique[["dest_row_id", "d_key1"]], left_on="o_key1", right_on="d_key1", how="left", suffixes=("", "_d1"))
    
    # Identificar os índices originais de o_base que encontraram um match
    matched_indices_k1 = m1[m1["dest_row_id"].notna()].index
    
    o_base.loc[matched_indices_k1, "match_level"] = "key1(venc-valor-parc-hist)"
    o_base.loc[matched_indices_k1, "dest_row_id"] = m1.loc[matched_indices_k1, "dest_row_id"].astype("Int64")


    # --- 2) key2 para quem não bateu ---
    logging.info("Realizando merge com key2 (venc-valor-parc) para registros restantes")
    faltam_mask_k2 = o_base["match_level"] == ""
    # Criar um subconjunto EXPLICITAMENTE para evitar problemas de SettingWithCopyWarning
    o_base_subset_k2 = o_base[faltam_mask_k2].copy()

    d2_unique = df_d_with_id.drop_duplicates(subset=["d_key2"])
    # Merge com o subconjunto, o resultado m2 terá os índices originais de o_base_subset_k2
    m2 = o_base_subset_k2.merge(d2_unique[["dest_row_id", "d_key2"]], left_on="o_key2", right_on="d_key2", how="left", suffixes=("", "_d2"))
    
    # Identificar os índices do subconjunto que encontraram um match
    matched_indices_in_m2 = m2[m2["dest_row_id"].notna()].index
    
    # Usar esses índices (que são os índices originais do o_base) para atualizar o o_base
    o_base.loc[matched_indices_in_m2, "match_level"] = "key2(venc-valor-parc)"
    o_base.loc[matched_indices_in_m2, "dest_row_id"] = m2.loc[matched_indices_in_m2, "dest_row_id"].astype("Int64")


    # --- 3) key3 para o restante ---
    logging.info("Realizando merge com key3 (venc-valor) para registros restantes")
    faltam_mask_k3 = o_base["match_level"] == ""
    o_base_subset_k3 = o_base[faltam_mask_k3].copy()

    d3_unique = df_d_with_id.drop_duplicates(subset=["d_key3"])
    m3 = o_base_subset_k3.merge(d3_unique[["dest_row_id", "d_key3"]], left_on="o_key3", right_on="d_key3", how="left", suffixes=("", "_d3"))

    matched_indices_in_m3 = m3[m3["dest_row_id"].notna()].index
    
    o_base.loc[matched_indices_in_m3, "match_level"] = "key3(venc-valor)"
    o_base.loc[matched_indices_in_m3, "dest_row_id"] = m3.loc[matched_indices_in_m3, "dest_row_id"].astype("Int64")


    # --- NOVO PASSO: 4) Fuzzy match Credor para registros restantes (com outras chaves exatas) ---
    faltam_fuzzy_mask = o_base["match_level"] == ""

    # Somente processa se houver registros restantes e rapidfuzz estiver disponível
    if faltam_fuzzy_mask.any() and HAS_RAPIDFUZZ:
        logging.info("[FUZZY_CREDOR] Iniciando busca por credores similares (venc, valor, parc, hist + credor fuzzy) para registros não encontrados.")

        # Criar uma cópia dos registros de origem não encontrados, mantendo o índice original
        df_o_unmatched = o_base[faltam_fuzzy_mask].copy()
        
        # Iterar sobre cada registro de origem não encontrado
        # Usamos itertuples() que é mais rápido que iterrows() e mantém o índice original
        for row_tuple in df_o_unmatched.itertuples(index=True):
            o_idx = row_tuple.Index # Índice original do DataFrame o_base
            o_venc = getattr(row_tuple, "o_std_venc", pd.NaT)
            o_valor = getattr(row_tuple, "o_std_valor", np.nan)
            o_parc = getattr(row_tuple, "o_std_parc", "")
            o_hist = getattr(row_tuple, "o_std_hist", "")
            o_credor_norm = getattr(row_tuple, "__credor_norm__", "")

            # Filtrar registros de destino pelas chaves exatas (venc, valor, parc, hist)
            # Isto reduz drasticamente o número de comparações fuzzy
            potential_matches_d = df_d_with_id[
                (df_d_with_id["d_std_venc"] == o_venc) &
                (df_d_with_id["d_std_valor"] == o_valor) &
                (df_d_with_id["d_std_parc"] == o_parc) &
                (df_d_with_id["d_std_hist"] == o_hist)
            ].copy()

            if not potential_matches_d.empty and o_credor_norm:
                # Obter os nomes de credor normalizados dos possíveis matches no destino
                # E garantir que cada nome seja mapeado para um único dest_row_id, para evitar ambiguidade.
                # Se houver múltiplas entradas com o mesmo __credor_norm__, pegamos a primeira ocorrência.
                choices_d_norm_cred_unique = potential_matches_d.drop_duplicates(subset=["__credor_norm__"])
                choices = choices_d_norm_cred_unique["__credor_norm__"].tolist()
                
                # Mapear o nome normalizado de volta para o 'dest_row_id' do df_d_with_id
                d_norm_cred_to_id = {norm_cred: dest_id for norm_cred, dest_id in zip(choices_d_norm_cred_unique["__credor_norm__"], choices_d_norm_cred_unique["dest_row_id"])}
                
                # Realizar o fuzzy match
                FUZZY_THRESHOLD = 88 # Limiar sugerido para "pouca diferença" (0-100)

                best_match_result = process.extractOne(
                    o_credor_norm,
                    choices, # Usar a lista de choices únicas
                    scorer=fuzz.token_sort_ratio
                )

                if best_match_result and best_match_result[1] >= FUZZY_THRESHOLD:
                    best_match_norm_cred, score, _ = best_match_result
                    
                    # Um match fuzzy foi encontrado. Atualizar o_base.
                    # Recuperar o dest_row_id usando o nome do credor normalizado encontrado
                    dest_match_id = d_norm_cred_to_id.get(best_match_norm_cred)

                    if pd.notna(dest_match_id):
                        o_base.loc[o_idx, "match_level"] = f"fuzzy_credor(score={int(score)})"
                        o_base.loc[o_idx, "dest_row_id"] = int(dest_match_id) # Garante que seja int ou pd.NA

        logging.info("[FUZZY_CREDOR] Busca por credores similares concluída.")

    # Enriquecer com dados do destino definitivo
    o_base["dest_row_id"] = o_base["dest_row_id"].astype("Int64") # Garante o tipo de dado para a coluna
    # o_base.merge com df_d_with_id para puxar todas as colunas relevantes do destino
    df_join = o_base.merge(df_d_with_id[cols_keep_d_filtered + ["dest_row_id"]],
                           on="dest_row_id", how="left", suffixes=("", "_dest"))

    # Formatar campos de saída da origem
    out_cols_comuns = {
        "Vencimento_Origem": df_join["o_std_venc"].apply(fmt_date),
        "Valor_Origem": df_join["o_std_valor"],
        "Parc_Origem": df_join["o_std_parc"],
        "Historico_Origem": df_join["o_std_hist"],
        "Credor_Origem": df_join["__credor_raw__"]
    }
    # Formatar destino
    out_cols_dest = {
        "Vencimento_Dest": df_join["d_std_venc"].apply(fmt_date),
        "Valor_Dest": df_join["d_std_valor"],
        "Parc_Dest": df_join["d_std_parc"],
        "Historico_Dest": df_join["d_std_hist"],
        # Ajuste para garantir que as colunas existam antes de acessá-las
        "Credor_Dest_Bruto": df_join.get("__credor_raw___dest", df_join.get("__credor_raw__dest", df_join["__credor_raw__"] if "__credor_raw__" in df_join.columns else "")),
        "Credor_Dest_Canonico": df_join.get("Credor_Canonico_dest", df_join["Credor_Canonico"] if "Credor_Canonico" in df_join.columns else ""),
        "CNPJ_Dest": df_join.get("CNPJ_Fornec_dest", df_join["CNPJ_Fornec"] if "CNPJ_Fornec" in df_join.columns else ""),
        "Credor_Dest_Norm": df_join.get("__credor_norm___dest", df_join["__credor_norm__"] if "__credor_norm__" in df_join.columns else "")
    }

    saida = pd.DataFrame(out_cols_comuns)
    for k, v in out_cols_dest.items():
        saida[k] = v
    saida["Credor_Origem_Canonico"] = df_join["Credor_Canonico"]
    saida["CNPJ_Origem"] = df_join["CNPJ_Fornec"]
    saida["Credor_Origem_Norm"] = df_join["__credor_norm__"]
    saida["match_level"] = df_join["match_level"]

    # Determinar status de match
    ja_no_destino = saida[saida["match_level"] != ""].copy()
    nao_encontrados = saida[saida["match_level"] == ""].copy()

    # Divergências de Credor (quando bateu por chaves mas nomes canônicos divergem)
    def credor_diverge(row):
        co = str(row.get("Credor_Origem_Canonico") or "").strip()
        cd = str(row.get("Credor_Dest_Canonico") or "").strip()
        # se nenhum canônico está preenchido, usa bruto (menos confiável)
        if not co and not cd:
            cbo = norm_str(row.get("Credor_Origem") or "")
            cbd = norm_str(row.get("Credor_Dest_Bruto") or "")
            return cbo != "" and cbd != "" and cbo != cbd
        # Se um match fuzzy foi encontrado, o match_level terá o score.
        # A divergência só ocorre se os nomes canônicos forem realmente diferentes.
        return co != "" and cd != "" and co != cd

    divergencias = ja_no_destino[ja_no_destino.apply(credor_diverge, axis=1).astype(bool)].copy()

    # Para inserir (já formatado e com CNPJ do cadastro)
    para_inserir = nao_encontrados.copy()
    # escolher credor e cnpj: preferir canônico se houver
    pi_credor = []
    pi_cnpj = []
    for _, r in para_inserir.iterrows():
        cred_can = r.get("Credor_Origem_Canonico", "")
        cnpj_can = r.get("CNPJ_Origem", "")
        if cred_can and cnpj_can:
            pi_credor.append(cred_can)
            pi_cnpj.append(cnpj_can)
        else:
            # tenta canonizar de novo a partir do bruto
            can_name, can_cnpj = canoniza_credor(r.get("Credor_Origem", ""), mapa_fornec)
            pi_credor.append(can_name if can_name else r.get("Credor_Origem", ""))
            pi_cnpj.append(can_cnpj)
    para_inserir["Credor_Para_Inserir"] = pi_credor
    para_inserir["CNPJ_Para_Inserir"] = pi_cnpj
    para_inserir["Vencimento_Para_Inserir"] = para_inserir["Vencimento_Origem"]
    para_inserir["Valor_Para_Inserir"] = para_inserir["Valor_Origem"]
    # Parc já vem da origem como X/Y
    para_inserir["Parc_Para_Inserir"] = para_inserir["Parc_Origem"]
    para_inserir["Historico_Para_Inserir"] = para_inserir["Historico_Origem"]

    # Regras de dúvidas: sem data, sem valor, sem parc (quando esperado), sem CNPJ
    duvidas_mask = (
        (para_inserir["Vencimento_Para_Inserir"] == "") |
        (para_inserir["Valor_Para_Inserir"].isna()) |
        ((para_inserir["Parc_Para_Inserir"] == "") & (para_inserir["Parc_Origem"] == "")) |
        (para_inserir["CNPJ_Para_Inserir"].astype(str).str.strip() == "")
    )
    duvidas = para_inserir[duvidas_mask].copy()

    # Exportar
    with pd.ExcelWriter(SAIDA_XLSX, engine="openpyxl") as writer:
        ja_no_destino.to_excel(writer, index=False, sheet_name="Ja_no_destino")
        para_inserir.to_excel(writer, index=False, sheet_name="Para_inserir")
        divergencias.to_excel(writer, index=False, sheet_name="Divergencias")
        duvidas.to_excel(writer, index=False, sheet_name="Duvidas")

    logging.info(f"Concluído. Ja_no_destino: {len(ja_no_destino)}, Para_inserir: {len(para_inserir)}, Divergencias: {len(divergencias)}, Duvidas: {len(duvidas)}")
    input("Processo finalizado. Pressione Enter para sair...")

if __name__ == "__main__":
    try:
        main()
    except Exception as e:
        logging.exception("Erro inesperado durante o processamento")
        input("Ocorreu um erro. Verifique o arquivo de log. Pressione Enter para sair...")