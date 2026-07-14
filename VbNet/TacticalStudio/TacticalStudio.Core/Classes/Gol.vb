Imports TacticalStudio.Core.Enums

Namespace Classes

    Public Class Gol
        Inherits ObjetoCampo

        Public Property Orientacao As OrientacaoGol

        Public Sub New()

            MyBase.New()

            Orientacao = OrientacaoGol.Direita

        End Sub

    End Class

End Namespace