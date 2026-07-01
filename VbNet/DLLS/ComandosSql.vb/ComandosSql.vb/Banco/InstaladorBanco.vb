Imports System.IO
Imports MySqlConnector
Imports ComandosSQL.Models

Namespace Banco

    Public Class InstaladorBanco
        Public Shared Property UltimoErro As String = ""

        Public Shared Function MySQLDisponivel(
    cfg As ConfiguracaoBanco) As Boolean

            Try

                Using conn As New MySqlConnection(
            ConexaoMySql.StringConexao(cfg))

                    conn.Open()

                    Return True

                End Using

            Catch ex As Exception

                Throw

                Return False

            End Try

        End Function

        Public Shared Function BancoExiste(
            cfg As ConfiguracaoBanco) As Boolean

            Using conn As New MySqlConnection(
                ConexaoMySql.StringConexao(cfg))

                conn.Open()

                Dim sql As String =
                    "SELECT COUNT(*)
                     FROM INFORMATION_SCHEMA.SCHEMATA
                     WHERE SCHEMA_NAME = @banco"

                Using cmd As New MySqlCommand(sql, conn)

                    cmd.Parameters.AddWithValue(
                        "@banco",
                        cfg.Banco)

                    Return CInt(cmd.ExecuteScalar()) > 0

                End Using

            End Using

        End Function

        Private Shared Sub ExecutarScript(
            cfg As ConfiguracaoBanco,
            caminhoSql As String)

            Dim script As String =
                File.ReadAllText(caminhoSql)

            Dim comandos() As String =
                script.Split(";"c)

            Using conn As New MySqlConnection(
                ConexaoMySql.StringConexao(cfg))

                conn.Open()

                For Each comando As String In comandos

                    If comando.Trim <> "" Then

                        Using cmd As New MySqlCommand(
                            comando,
                            conn)

                            cmd.ExecuteNonQuery()

                        End Using

                    End If

                Next

            End Using

        End Sub

        Public Shared Function ValidarInstalacao(
            cfg As ConfiguracaoBanco,
            caminhoSql As String) As ResultadoValidacao

            Try

                If Not MySQLDisponivel(cfg) Then
                    Return ResultadoValidacao.MySQLNaoEncontrado
                End If

                If BancoExiste(cfg) Then
                    Return ResultadoValidacao.Ok
                End If

                If Not File.Exists(caminhoSql) Then
                    Return ResultadoValidacao.ScriptNaoEncontrado
                End If

                ExecutarScript(cfg, caminhoSql)

                Return ResultadoValidacao.Ok

            Catch ex As Exception

                UltimoErro = ex.Message

                Return ResultadoValidacao.ErroInstalacao

            End Try

        End Function

    End Class

End Namespace