import pandas as pd
from tkinter import Tk
from tkinter.filedialog import askopenfilename

# Inicializa Tkinter
Tk().withdraw()

# Selecionar arquivos
print("Selecione a Tabela antiga:")
arquivo_antiga = askopenfilename(filetypes=[("Excel files", "*.xlsx *.xls")])
tabela_antiga = pd.read_excel(arquivo_antiga)

print("Selecione a Tabela atual:")
arquivo_atual = askopenfilename(filetypes=[("Excel files", "*.xlsx *.xls")])
tabela_atual = pd.read_excel(arquivo_atual)

# Normalizar nomes das colunas
tabela_antiga.columns = tabela_antiga.columns.str.strip().str.lower()
tabela_atual.columns = tabela_atual.columns.str.strip().str.lower()

# Função para padronizar tipos
def padronizar_tipos(df):
    for col in df.columns:
        # Tenta converter para datetime
        try:
            df[col] = pd.to_datetime(df[col], errors='ignore')
        except:
            pass
        # Tenta converter para número
        try:
            df[col] = pd.to_numeric(df[col], errors='ignore')
        except:
            pass
        # Converte texto para string
        df[col] = df[col].astype(str)
    return df

tabela_antiga = padronizar_tipos(tabela_antiga)
tabela_atual = padronizar_tipos(tabela_atual)

# Colunas para comparar (todas)
colunas_para_comparar = tabela_antiga.columns.tolist()

# Filtrar linhas que estão na antiga mas não na atual
faltantes = tabela_antiga.merge(
    tabela_atual,
    on=colunas_para_comparar,
    how='left',
    indicator=True
)
faltantes = faltantes[faltantes['_merge'] == 'left_only']
faltantes = faltantes.drop(columns=['_merge'])

# Salvar resultado
faltantes.to_excel("faltantes.xlsx", index=False)
print("Arquivo 'faltantes.xlsx' gerado com sucesso!")
