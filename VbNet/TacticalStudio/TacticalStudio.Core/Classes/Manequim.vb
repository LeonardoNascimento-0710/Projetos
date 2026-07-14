Imports TacticalStudio.Core.Enums

Namespace Classes

    Public Class Manequim
        Inherits ObjetoCampo

        Public Property Cor As CorManequim

        Public Sub New()

            MyBase.New()

            Cor = CorManequim.Amarelo

        End Sub

    End Class

End Namespace