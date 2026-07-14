Namespace Classes

    Public Class Jogador
        Inherits ObjetoCampo

        Public Property Nome As String

        Public Property Numero As Integer

        Public Sub New()

            MyBase.New()

            Nome = "Jogador"

            Numero = 0

        End Sub

    End Class

End Namespace