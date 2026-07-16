Imports System.IO
Imports System.Text.Json
Imports TacticalStudio.Core

Public NotInheritable Class RepositorioBibliotecaExercicios

#Region "Constantes"

    Private Const NomePastaAplicacao As String =
        "TacticalStudio"

    Private Const NomePastaBiblioteca As String =
        "Biblioteca"

    Private Const NomePastaExercicios As String =
        "Exercicios"

    Private Const NomePastaMiniaturas As String =
        "Miniaturas"

    Private Const NomeArquivoIndice As String =
        "biblioteca.json"

#End Region

#Region "Inicialização"

    Private Sub New()
    End Sub

#End Region

#Region "Estrutura de pastas"

    Public Shared Function GarantirEstrutura() As Boolean

        Try

            Directory.CreateDirectory(
                ObterPastaBiblioteca())

            Directory.CreateDirectory(
                ObterPastaExercicios())

            Directory.CreateDirectory(
                ObterPastaMiniaturas())

            Return True

        Catch

            Return False

        End Try

    End Function

#End Region

#Region "Leitura"

    Public Shared Function CarregarIndice() As IndiceBibliotecaExercicios

        Dim indiceVazio As New IndiceBibliotecaExercicios()

        Try

            If Not GarantirEstrutura() Then
                Return indiceVazio
            End If

            Dim caminhoIndice As String =
                ObterCaminhoIndice()

            If Not File.Exists(
                caminhoIndice) Then

                SalvarIndice(
                    indiceVazio)

                Return indiceVazio

            End If

            Dim conteudo As String =
                File.ReadAllText(
                    caminhoIndice)

            If String.IsNullOrWhiteSpace(
                conteudo) Then

                Return indiceVazio

            End If

            Dim opcoes As New JsonSerializerOptions With {
                .PropertyNameCaseInsensitive = True
            }

            Dim indice As IndiceBibliotecaExercicios =
                JsonSerializer.Deserialize(
                    Of IndiceBibliotecaExercicios)(
                        conteudo,
                        opcoes)

            If indice Is Nothing Then
                Return indiceVazio
            End If

            If indice.Itens Is Nothing Then

                indice.Itens =
                    New List(Of ItemBibliotecaExercicio)()

            End If

            RemoverItensInvalidos(
                indice)

            Return indice

        Catch

            Return indiceVazio

        End Try

    End Function

    Public Shared Function ObterItens() As List(Of ItemBibliotecaExercicio)

        Dim indice As IndiceBibliotecaExercicios =
            CarregarIndice()

        Return indice.Itens

    End Function

    Public Shared Function ObterPorId(
        idItem As String
    ) As ItemBibliotecaExercicio

        If String.IsNullOrWhiteSpace(
            idItem) Then

            Return Nothing

        End If

        Dim indice As IndiceBibliotecaExercicios =
            CarregarIndice()

        For Each item As ItemBibliotecaExercicio
            In indice.Itens

            If item Is Nothing Then
                Continue For
            End If

            If String.Equals(
                item.Id,
                idItem,
                StringComparison.OrdinalIgnoreCase) Then

                Return item

            End If

        Next

        Return Nothing

    End Function

#End Region

#Region "Gravação do índice"

    Public Shared Function SalvarIndice(
        indice As IndiceBibliotecaExercicios
    ) As Boolean

        If indice Is Nothing Then
            Return False
        End If

        Try

            If Not GarantirEstrutura() Then
                Return False
            End If

            If indice.Itens Is Nothing Then

                indice.Itens =
                    New List(Of ItemBibliotecaExercicio)()

            End If

            indice.VersaoFormato =
                1

            Dim opcoes As New JsonSerializerOptions With {
                .WriteIndented = True
            }

            Dim conteudo As String =
                JsonSerializer.Serialize(
                    indice,
                    opcoes)

            Dim caminhoIndice As String =
                ObterCaminhoIndice()

            Dim caminhoTemporario As String =
                caminhoIndice & ".tmp"

            File.WriteAllText(
                caminhoTemporario,
                conteudo)

            File.Move(
                caminhoTemporario,
                caminhoIndice,
                True)

            Return True

        Catch

            Return False

        End Try

    End Function

#End Region

#Region "Adicionar exercício"

    Public Shared Function AdicionarArquivo(
        caminhoArquivoOrigem As String,
        nome As String,
        descricao As String,
        categoria As String,
        Optional favorito As Boolean = False
    ) As ItemBibliotecaExercicio

        If String.IsNullOrWhiteSpace(
            caminhoArquivoOrigem) OrElse
           Not File.Exists(
               caminhoArquivoOrigem) Then

            Return Nothing

        End If

        Try

            If Not GarantirEstrutura() Then
                Return Nothing
            End If

            Dim idItem As String =
                CriarIdItem()

            Dim nomeArquivoDestino As String =
                idItem & ".tactical"

            Dim caminhoArquivoDestino As String =
                Path.Combine(
                    ObterPastaExercicios(),
                    nomeArquivoDestino)

            File.Copy(
                caminhoArquivoOrigem,
                caminhoArquivoDestino,
                True)

            Dim agoraUtc As DateTime =
                DateTime.UtcNow

            Dim item As New ItemBibliotecaExercicio With {
                .Id = idItem,
                .nome = NormalizarNome(nome),
                .descricao = NormalizarDescricao(descricao),
                .categoria = NormalizarCategoria(categoria),
                .favorito = favorito,
                .DataCriacaoUtc = agoraUtc,
                .DataAtualizacaoUtc = agoraUtc,
                .NomeArquivoExercicio = nomeArquivoDestino,
                .NomeArquivoMiniatura = String.Empty
            }

            Dim indice As IndiceBibliotecaExercicios =
                CarregarIndice()

            indice.Itens.Add(
                item)

            If Not SalvarIndice(
                indice) Then

                TentarExcluirArquivo(
                    caminhoArquivoDestino)

                Return Nothing

            End If

            Return item

        Catch

            Return Nothing

        End Try

    End Function

#End Region

#Region "Atualização"

    Public Shared Function AtualizarMetadados(
        idItem As String,
        nome As String,
        descricao As String,
        categoria As String,
        favorito As Boolean
    ) As Boolean

        If String.IsNullOrWhiteSpace(
            idItem) Then

            Return False

        End If

        Dim indice As IndiceBibliotecaExercicios =
            CarregarIndice()

        For Each item As ItemBibliotecaExercicio
            In indice.Itens

            If item Is Nothing Then
                Continue For
            End If

            If Not String.Equals(
                item.Id,
                idItem,
                StringComparison.OrdinalIgnoreCase) Then

                Continue For

            End If

            item.Nome =
                NormalizarNome(
                    nome)

            item.Descricao =
                NormalizarDescricao(
                    descricao)

            item.Categoria =
                NormalizarCategoria(
                    categoria)

            item.Favorito =
                favorito

            item.DataAtualizacaoUtc =
                DateTime.UtcNow

            Return SalvarIndice(
                indice)

        Next

        Return False

    End Function

    Public Shared Function DefinirFavorito(
        idItem As String,
        favorito As Boolean
    ) As Boolean

        Dim item As ItemBibliotecaExercicio =
            ObterPorId(
                idItem)

        If item Is Nothing Then
            Return False
        End If

        Return AtualizarMetadados(
            item.Id,
            item.Nome,
            item.Descricao,
            item.Categoria,
            favorito)

    End Function

    Public Shared Function AtualizarArquivoExercicio(
        idItem As String,
        caminhoArquivoOrigem As String
    ) As Boolean

        If String.IsNullOrWhiteSpace(
            idItem) OrElse
           String.IsNullOrWhiteSpace(
               caminhoArquivoOrigem) OrElse
           Not File.Exists(
               caminhoArquivoOrigem) Then

            Return False

        End If

        Dim item As ItemBibliotecaExercicio =
            ObterPorId(
                idItem)

        If item Is Nothing Then
            Return False
        End If

        Try

            Dim caminhoDestino As String =
                ObterCaminhoExercicio(
                    item)

            If String.IsNullOrWhiteSpace(
                caminhoDestino) Then

                Return False

            End If

            Dim caminhoTemporario As String =
                caminhoDestino & ".tmp"

            File.Copy(
                caminhoArquivoOrigem,
                caminhoTemporario,
                True)

            File.Move(
                caminhoTemporario,
                caminhoDestino,
                True)

            Return AtualizarMetadados(
                item.Id,
                item.Nome,
                item.Descricao,
                item.Categoria,
                item.Favorito)

        Catch

            Return False

        End Try

    End Function

#End Region

#Region "Remoção"

    Public Shared Function Remover(
        idItem As String
    ) As Boolean

        If String.IsNullOrWhiteSpace(
            idItem) Then

            Return False

        End If

        Dim indice As IndiceBibliotecaExercicios =
            CarregarIndice()

        Dim itemRemover As ItemBibliotecaExercicio =
            Nothing

        For Each item As ItemBibliotecaExercicio
            In indice.Itens

            If item Is Nothing Then
                Continue For
            End If

            If String.Equals(
                item.Id,
                idItem,
                StringComparison.OrdinalIgnoreCase) Then

                itemRemover =
                    item

                Exit For

            End If

        Next

        If itemRemover Is Nothing Then
            Return False
        End If

        indice.Itens.Remove(
            itemRemover)

        If Not SalvarIndice(
            indice) Then

            Return False

        End If

        TentarExcluirArquivo(
            ObterCaminhoExercicio(
                itemRemover))

        TentarExcluirArquivo(
            ObterCaminhoMiniatura(
                itemRemover))

        Return True

    End Function

#End Region

#Region "Caminhos públicos"

    Public Shared Function ObterCaminhoExercicio(
        item As ItemBibliotecaExercicio
    ) As String

        If item Is Nothing OrElse
           String.IsNullOrWhiteSpace(
               item.NomeArquivoExercicio) Then

            Return String.Empty

        End If

        Return Path.Combine(
            ObterPastaExercicios(),
            item.NomeArquivoExercicio)

    End Function

    Public Shared Function ObterCaminhoMiniatura(
        item As ItemBibliotecaExercicio
    ) As String

        If item Is Nothing OrElse
           String.IsNullOrWhiteSpace(
               item.NomeArquivoMiniatura) Then

            Return String.Empty

        End If

        Return Path.Combine(
            ObterPastaMiniaturas(),
            item.NomeArquivoMiniatura)

    End Function

    Public Shared Function ObterPastaBiblioteca() As String

        Dim pastaLocal As String =
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData)

        Return Path.Combine(
            pastaLocal,
            NomePastaAplicacao,
            NomePastaBiblioteca)

    End Function

    Public Shared Function ObterPastaExercicios() As String

        Return Path.Combine(
            ObterPastaBiblioteca(),
            NomePastaExercicios)

    End Function

    Public Shared Function ObterPastaMiniaturas() As String

        Return Path.Combine(
            ObterPastaBiblioteca(),
            NomePastaMiniaturas)

    End Function

    Public Shared Function ObterCaminhoIndice() As String

        Return Path.Combine(
            ObterPastaBiblioteca(),
            NomeArquivoIndice)

    End Function

#End Region

#Region "Validação"

    Private Shared Sub RemoverItensInvalidos(
        indice As IndiceBibliotecaExercicios)

        If indice Is Nothing OrElse
           indice.Itens Is Nothing Then

            Exit Sub

        End If

        For indiceItem As Integer =
            indice.Itens.Count - 1 To 0 Step -1

            Dim item As ItemBibliotecaExercicio =
                indice.Itens(indiceItem)

            If item Is Nothing OrElse
               String.IsNullOrWhiteSpace(
                   item.Id) OrElse
               String.IsNullOrWhiteSpace(
                   item.NomeArquivoExercicio) Then

                indice.Itens.RemoveAt(
                    indiceItem)

                Continue For

            End If

            item.Nome =
                NormalizarNome(
                    item.Nome)

            item.Descricao =
                NormalizarDescricao(
                    item.Descricao)

            item.Categoria =
                NormalizarCategoria(
                    item.Categoria)

        Next

    End Sub

    Private Shared Function NormalizarNome(
        valor As String
    ) As String

        Dim resultado As String =
            If(
                valor,
                String.Empty).
            Trim()

        If String.IsNullOrWhiteSpace(
            resultado) Then

            resultado =
                "Exercício sem nome"

        End If

        If resultado.Length > 100 Then

            resultado =
                resultado.Substring(
                    0,
                    100)

        End If

        Return resultado

    End Function

    Private Shared Function NormalizarDescricao(
        valor As String
    ) As String

        Dim resultado As String =
            If(
                valor,
                String.Empty).
            Trim()

        If resultado.Length > 500 Then

            resultado =
                resultado.Substring(
                    0,
                    500)

        End If

        Return resultado

    End Function

    Private Shared Function NormalizarCategoria(
        valor As String
    ) As String

        Dim resultado As String =
            If(
                valor,
                String.Empty).
            Trim()

        If String.IsNullOrWhiteSpace(
            resultado) Then

            resultado =
                "Sem categoria"

        End If

        If resultado.Length > 80 Then

            resultado =
                resultado.Substring(
                    0,
                    80)

        End If

        Return resultado

    End Function

#End Region

#Region "Auxiliares"

    Private Shared Function CriarIdItem() As String

        Return "exercicio-" &
            Guid.NewGuid().
                ToString("N")

    End Function

    Private Shared Sub TentarExcluirArquivo(
        caminho As String)

        If String.IsNullOrWhiteSpace(
            caminho) Then

            Exit Sub

        End If

        Try

            If File.Exists(
                caminho) Then

                File.Delete(
                    caminho)

            End If

        Catch

        End Try

    End Sub

#End Region

End Class