Public Class PosicaoFormacao

    Public Property Numero As Integer

    Public Property Nome As String = String.Empty

    Public Property XPercentual As Single

    Public Property YPercentual As Single

    Public Property OrientacaoVisual As OrientacaoVisualJogador =
        OrientacaoVisualJogador.Costas

    Public Property CorCamisaArgb As Integer = System.Drawing.Color.FromArgb(185, 35, 35).ToArgb()

End Class