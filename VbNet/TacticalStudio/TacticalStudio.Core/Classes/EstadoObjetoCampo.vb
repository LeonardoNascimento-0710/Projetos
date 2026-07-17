Namespace Classes

    Public Class EstadoObjetoCampo

        Public Property TipoObjeto As String = ""
        Public Property EscalaVisual As Single = 1.0F
        Public Property X As Double
        Public Property Y As Double
        Public Property XFinal As Double
        Public Property YFinal As Double
        Public Property Visivel As Boolean = True
        Public Property NomePersonalizado As String = String.Empty
        Public Property Numero As Integer
        Public Property Nome As String = ""
        Public Property Texto As String = ""
        Public Property CorConeValor As Integer
        Public Property OrientacaoGolValor As Integer
        Public Property CorManequimValor As Integer
        Public Property DirecaoDoJogador As DirecaoJogador = DirecaoJogador.Cima
        Public Property PoseDoJogador As PoseJogador = PoseJogador.Parado
        Public Property TipoLinhaValor As Integer
        Public Property CorLinhaValor As Integer
        Public Property CorCamisaJogadorArgb As Integer = System.Drawing.Color.FromArgb(185, 35, 35).ToArgb()
        Public Property CorAreaValor As Integer
        Public Property CorMarcadorValor As Integer
        Public Property CorTextoValor As Integer
        Public Property Espessura As Single
        Public Property Diametro As Single
        Public Property TamanhoFonte As Single
        Public Property Opacidade As Integer
        Public Property Tracejada As Boolean
        Public Property Negrito As Boolean
        Public Property FundoVisivel As Boolean
        Public Property GrupoId As String = String.Empty
        Public Property Bloqueado As Boolean = False

    End Class

End Namespace