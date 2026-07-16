Public Class ModeloFormacao

    Public Property Id As String = String.Empty

    Public Property Nome As String = String.Empty

    Public Property Descricao As String = String.Empty

    Public Property Posicoes As New List(Of PosicaoFormacao)()

End Class