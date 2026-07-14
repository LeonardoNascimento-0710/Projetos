Imports TacticalStudio.Core.Enums

Namespace Classes

    Public Class AreaTatica
        Inherits ObjetoCampo

        Public Property PosicaoFinal As Posicao

        Public Property Cor As CorAreaTatica

        Public Property Espessura As Single

        Public Property Opacidade As Integer

        Public Property Tracejada As Boolean

        Public Sub New()

            MyBase.New()

            PosicaoFinal = New Posicao()

            Cor = CorAreaTatica.Amarela

            Espessura = 2.5F

            Opacidade = 45

            Tracejada = True

        End Sub

    End Class

End Namespace