Namespace Classes

    Public MustInherit Class ObjetoCampo

        Public Property Posicao As Posicao

        Public Property Rotacao As Double

        Public Property Visivel As Boolean

        Public Property Selecionado As Boolean

        Public Property NomePersonalizado As String = String.Empty

        Public Property GrupoId As String = String.Empty

        Public Property Bloqueado As Boolean = False

        Public Sub New()

            Posicao = New Posicao()

            Rotacao = 0

            Visivel = True

            Selecionado = False

        End Sub

    End Class

End Namespace