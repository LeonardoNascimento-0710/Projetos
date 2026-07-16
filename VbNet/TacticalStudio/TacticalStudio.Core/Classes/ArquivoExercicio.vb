Namespace Classes

    Public Class ArquivoExercicio

        Public Property VersaoFormato As Integer = 2

        Public Property Nome As String =
            "Novo exercício"

        Public Property Categoria As String =
            "Tático"

        Public Property DuracaoMinutos As Integer =
            30

        Public Property Descricao As String =
            String.Empty

        Public Property Observacoes As String =
            String.Empty

        Public Property Campo As EstadoCampo

        Public Sub New()

            Campo =
                New EstadoCampo()

        End Sub

    End Class

End Namespace