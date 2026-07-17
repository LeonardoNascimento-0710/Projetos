Imports System.IO
Imports System.Text.Json
Imports TacticalStudio.Core

Public NotInheritable Class RepositorioFormacoesPersonalizadas

#Region "Constantes e caminhos"

    Private Const NomePastaAplicacao As String =
        "TacticalStudio"

    Private Const NomeArquivo As String =
        "formacoes-personalizadas.json"

#End Region

#Region "Inicialização"

    Private Sub New()
    End Sub

#End Region

#Region "Leitura"

    Public Shared Function CarregarTodas() As List(Of ModeloFormacao)

        Dim formacoes As New List(Of ModeloFormacao)()

        Try

            Dim caminho As String =
                ObterCaminhoArquivo()

            If Not File.Exists(caminho) Then
                Return formacoes
            End If

            Dim conteudo As String =
                File.ReadAllText(caminho)

            If String.IsNullOrWhiteSpace(conteudo) Then
                Return formacoes
            End If

            Dim opcoes As New JsonSerializerOptions With {
                .PropertyNameCaseInsensitive = True
            }

            Dim formacoesSalvas As List(Of ModeloFormacao) =
                JsonSerializer.Deserialize(
                    Of List(Of ModeloFormacao))(
                        conteudo,
                        opcoes)

            If formacoesSalvas Is Nothing Then
                Return formacoes
            End If

            For Each formacao As ModeloFormacao
                In formacoesSalvas

                Dim formacaoValidada As ModeloFormacao =
                    ValidarEClonarFormacao(
                        formacao)

                If formacaoValidada Is Nothing Then
                    Continue For
                End If

                formacoes.Add(
                    formacaoValidada)

            Next

        Catch

            'Um arquivo inválido não deve impedir
            'a abertura do TacticalStudio.

        End Try

        Return formacoes

    End Function

#End Region

#Region "Gravação"

    Public Shared Function SalvarTodas(
        formacoes As IEnumerable(Of ModeloFormacao)
    ) As Boolean

        Try

            Dim formacoesSalvar As New List(Of ModeloFormacao)()

            If formacoes IsNot Nothing Then

                For Each formacao As ModeloFormacao
                    In formacoes

                    Dim formacaoValidada As ModeloFormacao =
                        ValidarEClonarFormacao(
                            formacao)

                    If formacaoValidada Is Nothing Then
                        Continue For
                    End If

                    formacoesSalvar.Add(
                        formacaoValidada)

                Next

            End If

            Dim caminho As String =
                ObterCaminhoArquivo()

            Dim pasta As String =
                Path.GetDirectoryName(
                    caminho)

            If Not Directory.Exists(pasta) Then

                Directory.CreateDirectory(
                    pasta)

            End If

            Dim opcoes As New JsonSerializerOptions With {
                .WriteIndented = True
            }

            Dim conteudo As String =
                JsonSerializer.Serialize(
                    formacoesSalvar,
                    opcoes)

            Dim caminhoTemporario As String =
                caminho & ".tmp"

            File.WriteAllText(
                caminhoTemporario,
                conteudo)

            File.Move(
                caminhoTemporario,
                caminho,
                True)

            Return True

        Catch

            Return False

        End Try

    End Function

    Public Shared Function Adicionar(
        formacao As ModeloFormacao
    ) As Boolean

        Dim formacaoValidada As ModeloFormacao =
            ValidarEClonarFormacao(
                formacao)

        If formacaoValidada Is Nothing Then
            Return False
        End If

        Dim formacoes As List(Of ModeloFormacao) =
            CarregarTodas()

        For indice As Integer =
            formacoes.Count - 1 To 0 Step -1

            If String.Equals(
                formacoes(indice).Id,
                formacaoValidada.Id,
                StringComparison.OrdinalIgnoreCase) Then

                formacoes.RemoveAt(
                    indice)

            End If

        Next

        formacoes.Add(
            formacaoValidada)

        Return SalvarTodas(
            formacoes)

    End Function

    Public Shared Function Remover(
        idFormacao As String
    ) As Boolean

        If String.IsNullOrWhiteSpace(
            idFormacao) Then

            Return False

        End If

        Dim formacoes As List(Of ModeloFormacao) =
            CarregarTodas()

        Dim removeu As Boolean =
            False

        For indice As Integer =
            formacoes.Count - 1 To 0 Step -1

            If String.Equals(
                formacoes(indice).Id,
                idFormacao,
                StringComparison.OrdinalIgnoreCase) Then

                formacoes.RemoveAt(
                    indice)

                removeu =
                    True

            End If

        Next

        If Not removeu Then
            Return False
        End If

        Return SalvarTodas(
            formacoes)

    End Function

#End Region

#Region "Criação de identificadores"

    Public Shared Function CriarIdPersonalizado() As String

        Return "personalizada-" &
            Guid.NewGuid().
                ToString("N")

    End Function

#End Region

#Region "Validação"

    Private Shared Function ValidarEClonarFormacao(
        formacao As ModeloFormacao
    ) As ModeloFormacao

        If formacao Is Nothing OrElse
           formacao.Posicoes Is Nothing OrElse
           formacao.Posicoes.Count = 0 Then

            Return Nothing

        End If

        Dim id As String =
            If(
                formacao.Id,
                String.Empty).
            Trim()

        If String.IsNullOrWhiteSpace(id) Then

            id =
                CriarIdPersonalizado()

        End If

        Dim nome As String =
            If(
                formacao.Nome,
                String.Empty).
            Trim()

        If String.IsNullOrWhiteSpace(nome) Then

            nome =
                "Formação personalizada"

        End If

        If nome.Length > 80 Then

            nome =
                nome.Substring(
                    0,
                    80)

        End If

        Dim descricao As String =
            If(
                formacao.Descricao,
                String.Empty).
            Trim()

        If descricao.Length > 250 Then

            descricao =
                descricao.Substring(
                    0,
                    250)

        End If

        Dim copia As New ModeloFormacao With {
            .id = id,
            .nome = nome,
            .descricao = descricao
        }

        For Each posicao As PosicaoFormacao
            In formacao.Posicoes

            If posicao Is Nothing Then
                Continue For
            End If

            Dim nomePosicao As String =
                If(
                    posicao.Nome,
                    String.Empty).
                Trim()

            If nomePosicao.Length > 80 Then

                nomePosicao =
                    nomePosicao.Substring(
                        0,
                        80)

            End If

            copia.Posicoes.Add(
                New PosicaoFormacao With {
                    .Numero = posicao.Numero,
                    .nome = nomePosicao,
                    .XPercentual =
                        LimitarPercentual(
                            posicao.XPercentual),
                    .YPercentual =
                        LimitarPercentual(
                            posicao.YPercentual)
                })

        Next

        If copia.Posicoes.Count = 0 Then
            Return Nothing
        End If

        Return copia

    End Function

    Private Shared Function LimitarPercentual(
        valor As Single
    ) As Single

        Return Math.Max(
            0.0F,
            Math.Min(
                100.0F,
                valor))

    End Function

#End Region

#Region "Caminhos"

    Private Shared Function ObterCaminhoArquivo() As String

        Dim pastaLocal As String =
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData)

        Return Path.Combine(
            pastaLocal,
            NomePastaAplicacao,
            NomeArquivo)

    End Function

#End Region

End Class