Imports TacticalStudio.Core.Enums

Namespace Classes

    Public Class MarcadorTatico
        Inherits ObjetoCampo

        Public Property Texto As String

        Public Property Cor As CorMarcadorTatico

        Public Property Diametro As Single

        Public Sub New()

            MyBase.New()

            Texto = "1"
            Cor = CorMarcadorTatico.Branco
            Diametro = 34.0F

        End Sub

    End Class

End Namespace