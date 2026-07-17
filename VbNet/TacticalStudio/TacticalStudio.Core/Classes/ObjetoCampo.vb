Namespace Classes

    Public MustInherit Class ObjetoCampo

        Public Property Posicao As Posicao

        Public Property Rotacao As Double

        Public Property Visivel As Boolean

        Public Property Selecionado As Boolean

        Public Property NomePersonalizado As String = String.Empty

        Public Property GrupoId As String = String.Empty

        Public Property Bloqueado As Boolean = False

        Public Property EscalaVisual As Single

            Get

                Return _escalaVisual

            End Get

            Set(value As Single)

                If Single.IsNaN(value) OrElse
           Single.IsInfinity(value) Then

                    _escalaVisual =
                1.0F

                    Exit Property

                End If

                _escalaVisual =
            Math.Max(
                0.5F,
                Math.Min(
                    2.5F,
                    value))

            End Set

        End Property

        Private _escalaVisual As Single = 1.0F

        Public Sub New()

            Posicao = New Posicao()

            Rotacao = 0

            Visivel = True

            Selecionado = False

        End Sub

    End Class

End Namespace