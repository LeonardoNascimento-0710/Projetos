Namespace Classes

    Public Class Jogador
        Inherits ObjetoCampo

        Public Property Nome As String

        Public Property Numero As Integer

        'Mantida para compatibilidade com exercícios antigos.
        Public Property Direcao As DirecaoJogador = DirecaoJogador.Cima

        Public Property OrientacaoVisual As OrientacaoVisualJogador =
            OrientacaoVisualJogador.Costas

        Public Property Pose As PoseJogador = PoseJogador.Parado

        Public Property CorCamisaArgb As Integer = System.Drawing.Color.FromArgb(185, 35, 35).ToArgb()

        Public Sub New()

            MyBase.New()

            Nome = "Jogador"

            Numero = 0

        End Sub

    End Class

End Namespace