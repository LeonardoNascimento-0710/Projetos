Imports ComandosSql.Models

Namespace Banco

    Public Class ConexaoMySql

        Public Shared Function StringConexao(cfg As ConfiguracaoBanco) As String

            Return "Server=" & cfg.Servidor &
           ";Port=" & cfg.Porta &
           ";Database=" & cfg.Banco &
           ";User ID=" & cfg.Usuario &
           ";Password=" & cfg.Senha & ";"

        End Function

    End Class

End Namespace