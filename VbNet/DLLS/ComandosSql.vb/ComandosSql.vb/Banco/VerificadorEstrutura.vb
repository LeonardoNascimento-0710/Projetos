Imports MySqlConnector
Imports ComandosSQL.Models

Namespace Banco

    Public Class VerificadorEstrutura

        Public Shared Function TabelaExiste(cfg As ConfiguracaoBanco, nomeTabela As String) As Boolean

            Dim sql As String =
                "SELECT COUNT(*)
                 FROM information_schema.tables
                 WHERE table_schema = @banco
                 AND table_name = @tabela"

            Using conn As New MySqlConnection(
                ConexaoMySql.StringConexao(cfg))

                conn.Open()

                Using cmd As New MySqlCommand(sql, conn)

                    cmd.Parameters.AddWithValue("@banco", cfg.Banco)
                    cmd.Parameters.AddWithValue("@tabela", nomeTabela)

                    Return CInt(cmd.ExecuteScalar()) > 0

                End Using

            End Using

        End Function

        Public Shared Function ValidarTabelas(cfg As ConfiguracaoBanco) As Boolean

            If Not TabelaExiste(cfg, "balanca_toledo_itens") Then
                CriarTabelaItens(cfg)
            End If

            If Not TabelaExiste(cfg, "balanca_toledo_nutri") Then
                CriarTabelaNutri(cfg)
            End If

            Return True

        End Function

        Private Shared Sub ExecutarSQL(cfg As ConfiguracaoBanco, sql As String)

            Using conn As New MySqlConnection(
                ConexaoMySql.StringConexao(cfg))

                conn.Open()

                Using cmd As New MySqlCommand(sql, conn)

                    cmd.ExecuteNonQuery()

                End Using

            End Using

        End Sub

        Private Shared Sub CriarTabelaItens(cfg As ConfiguracaoBanco)

            Dim sql As String =
            "CREATE TABLE IF NOT EXISTS balanca_toledo_itens (
        tipo CHAR(1),
        codigo INT,
        validade INT,
        descricao VARCHAR(25),
        cod_nutri INT
    )"

            ExecutarSQL(cfg, sql)

        End Sub

        Private Shared Sub CriarTabelaNutri(cfg As ConfiguracaoBanco)

            Dim sql As String =
            "CREATE TABLE IF NOT EXISTS balanca_toledo_nutri (
        codigo INT,
        porcoes_embalagens INT,
        porcoes INT,
        unidade CHAR(1),
        medida_inteira INT,
        medida_decimal INT,
        medida_caseira INT,
        valor_energetico INT,
        carboidratos DECIMAL(5,1),
        acucares_totais DECIMAL(5,1),
        acucares_adicionados DECIMAL(5,1),
        proteinas DECIMAL(5,1),
        gorduras_totais DECIMAL(5,1),
        gorduras_saturadas DECIMAL(5,1),
        gorduras_trans DECIMAL(5,1),
        fibras DECIMAL(5,1),
        sodio DECIMAL(6,1),
        alto_acucar CHAR(1),
        alto_gordura CHAR(1),
        alto_sodio CHAR(1),
        lactose DECIMAL(6,1),
        galactose DECIMAL(6,1),
        acucares_adicionados_ext DECIMAL(6,1),
        acucares_totais_ext DECIMAL(6,1),
        gorduras_totais_ext DECIMAL(6,1),
        proteinas_ext DECIMAL(6,1),
        codigo_sd INT,
        descricao VARCHAR(50)
    )"

            ExecutarSQL(cfg, sql)

        End Sub

    End Class

End Namespace