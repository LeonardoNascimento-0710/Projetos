Imports TacticalStudio.Core.Enums

Namespace Classes

    Public Class LinhaTatica
        Inherits ObjetoCampo

        Public Property PosicaoFinal As Posicao

        Public Property Tipo As TipoLinhaTatica

        Public Property Cor As CorLinhaTatica

        Public Property Espessura As Single

        Public Sub New()

            MyBase.New()

            PosicaoFinal = New Posicao()

            Tipo = TipoLinhaTatica.Continua

            Cor = CorLinhaTatica.Branca

            Espessura = 3.0F

        End Sub

    End Class

End Namespace