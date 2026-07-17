Imports System.Drawing
Imports System.IO
Imports TacticalStudio.Core
Imports TacticalStudio.Core.Classes

Public NotInheritable Class GerenciadorSprites

    Private Shared ReadOnly Imagens As New Dictionary(Of String, Image)(
        StringComparer.OrdinalIgnoreCase)

    Private Shared ReadOnly Sincronizacao As New Object()

    Private Shared _carregado As Boolean

    Private Sub New()
    End Sub

    Public Shared Sub Carregar()

        SyncLock Sincronizacao

            LiberarInterno()

            Dim pastaRaiz As String =
                Path.Combine(
                    AppContext.BaseDirectory,
                    "Assets",
                    "Jogadores")

            If Not Directory.Exists(pastaRaiz) Then

                _carregado = True

                Exit Sub

            End If

            For Each orientacao As OrientacaoVisualJogador In
                [Enum].GetValues(Of OrientacaoVisualJogador)()

                For Each pose As PoseJogador In
                    [Enum].GetValues(Of PoseJogador)()

                    CarregarImagemJogador(
                        pastaRaiz,
                        orientacao,
                        pose,
                        "Base")

                    CarregarImagemJogador(
                        pastaRaiz,
                        orientacao,
                        pose,
                        "Camisa")

                Next

            Next

            _carregado = True

        End SyncLock

    End Sub

    Public Shared Function TentarObterJogador(
        orientacao As OrientacaoVisualJogador,
        pose As PoseJogador,
        ByRef imagemBase As Image,
        ByRef imagemCamisa As Image) As Boolean

        GarantirCarregado()

        Dim chaveBase As String =
            CriarChaveJogador(
                orientacao,
                pose,
                "Base")

        Dim chaveCamisa As String =
            CriarChaveJogador(
                orientacao,
                pose,
                "Camisa")

        SyncLock Sincronizacao

            imagemBase = Nothing
            imagemCamisa = Nothing

            If Not Imagens.TryGetValue(
                chaveBase,
                imagemBase) Then

                Return False

            End If

            If Not Imagens.TryGetValue(
                chaveCamisa,
                imagemCamisa) Then

                imagemBase = Nothing

                Return False

            End If

            Return True

        End SyncLock

    End Function

    Public Shared Function Obter(nome As String) As Image

        If String.IsNullOrWhiteSpace(nome) Then
            Return Nothing
        End If

        GarantirCarregado()

        SyncLock Sincronizacao

            Dim imagem As Image = Nothing

            If Imagens.TryGetValue(
                nome,
                imagem) Then

                Return imagem

            End If

        End SyncLock

        Return Nothing

    End Function

    Public Shared Sub Liberar()

        SyncLock Sincronizacao

            LiberarInterno()

            _carregado = False

        End SyncLock

    End Sub

    Private Shared Sub GarantirCarregado()

        If _carregado Then
            Exit Sub
        End If

        Carregar()

    End Sub

    Private Shared Sub CarregarImagemJogador(
        pastaRaiz As String,
        orientacao As OrientacaoVisualJogador,
        pose As PoseJogador,
        camada As String)

        Dim caminho As String =
            Path.Combine(
                pastaRaiz,
                orientacao.ToString(),
                $"{pose}_{camada}.png")

        If Not File.Exists(caminho) Then
            Exit Sub
        End If

        Dim imagem As Image =
            CarregarImagemSemBloquearArquivo(
                caminho)

        If imagem Is Nothing Then
            Exit Sub
        End If

        Dim chave As String =
            CriarChaveJogador(
                orientacao,
                pose,
                camada)

        Imagens(chave) =
            imagem

    End Sub

    Private Shared Function CarregarImagemSemBloquearArquivo(
        caminho As String) As Image

        Try

            Using fluxo As New FileStream(
                caminho,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite)

                Using imagemOriginal As Image =
                    Image.FromStream(
                        fluxo,
                        True,
                        True)

                    Return New Bitmap(
                        imagemOriginal)

                End Using

            End Using

        Catch

            Return Nothing

        End Try

    End Function

    Private Shared Function CriarChaveJogador(
        orientacao As OrientacaoVisualJogador,
        pose As PoseJogador,
        camada As String) As String

        Return $"Jogador|{orientacao}|{pose}|{camada}"

    End Function

    Private Shared Sub LiberarInterno()

        For Each imagem As Image In
            Imagens.Values

            imagem.Dispose()

        Next

        Imagens.Clear()

    End Sub

End Class
