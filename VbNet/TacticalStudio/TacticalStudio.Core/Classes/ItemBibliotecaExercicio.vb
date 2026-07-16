Public Class ItemBibliotecaExercicio

    Public Property Id As String = String.Empty

    Public Property Nome As String = String.Empty

    Public Property Descricao As String = String.Empty

    Public Property Categoria As String = "Sem categoria"

    Public Property Favorito As Boolean

    Public Property DataCriacaoUtc As DateTime = DateTime.UtcNow

    Public Property DataAtualizacaoUtc As DateTime = DateTime.UtcNow

    Public Property NomeArquivoExercicio As String = String.Empty

    Public Property NomeArquivoMiniatura As String = String.Empty

End Class