Imports TacticalStudio.Core.Enums

Namespace Classes

    Public Class TextoTatico
        Inherits ObjetoCampo

        Public Property Texto As String

        Public Property Cor As CorTextoTatico

        Public Property TamanhoFonte As Single

        Public Property Negrito As Boolean

        Public Property FundoVisivel As Boolean

        Public Sub New()

            MyBase.New()

            Texto = "Instrução"
            Cor = CorTextoTatico.Branco
            TamanhoFonte = 18.0F
            Negrito = True
            FundoVisivel = True

        End Sub

    End Class

End Namespace