Namespace Classes

    Public Class ArquivoExercicio

        Public Property VersaoFormato As Integer = 1

        Public Property Nome As String =
            "Novo exercício"

        Public Property Campo As EstadoCampo

        Public Sub New()

            Campo =
                New EstadoCampo()

        End Sub

    End Class

End Namespace