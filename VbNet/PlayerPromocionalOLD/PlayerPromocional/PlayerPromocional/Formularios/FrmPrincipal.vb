Imports NAudio.Wave
Imports System.Text.Json
Imports System.Diagnostics
Imports System.Threading.Tasks
Imports System.Security.Cryptography
Imports System.Text

Public Class FrmPrincipal


#Region "Variáveis"


    Private ReadOnly caminhosMusicas As New List(Of String)
    Private dispositivoSaida As WaveOutEvent
    Private leitorAudio As AudioFileReader
    Private indiceMusicaAtual As Integer = -1
    Private musicasDesdeUltimaPromocao As Integer = 0
    Private indiceAposNarracao As Integer = -1
    Private narracaoAutomaticaEmAndamento As Boolean = False
    Private narracaoTesteEmAndamento As Boolean = False
    Private retomarMusicaAposTeste As Boolean = False
    Private ReadOnly geradorAleatorio As New Random()
    Private ReadOnly historicoAleatorio As New Stack(Of String)
    Private voltandoPeloHistorico As Boolean = False
    Private dispositivoPromocaoPiper As WaveOutEvent
    Private leitorPromocaoPiper As AudioFileReader
    Private ReadOnly caminhosAnunciosGravados As New List(Of String)
    Private anunciosGravadosAleatorios As Boolean = False
    Private indiceUltimoAnuncioGravado As Integer = -1
    Private ReadOnly geradorAnunciosGravados As New Random()

    Private ReadOnly promocoesTexto As New List(Of PromocaoTexto)
    Private promocoesTextoAleatorias As Boolean = False
    Private indiceUltimaPromocaoTexto As Integer = -1
    Private ReadOnly filaPromocoesTextoAleatorias As New Queue(Of Integer)

    Private WithEvents BtnGerenciarPromocoesTexto As Button
    Private LstPromocoesTexto As ListBox
    Private LblQuantidadePromocoesTexto As Label

#End Region

#Region "Configuração"

    Private ReadOnly pastaPiper As String =
    IO.Path.Combine(
        Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData),
        "PlayerPromocional",
        "Piper")

    Private ReadOnly pastaVozesPiper As String =
    IO.Path.Combine(
        pastaPiper,
        "vozes")

    Private ReadOnly pastaCachePromocoesPiper As String =
    IO.Path.Combine(
        pastaPiper,
        "cache_promocoes")

    Private Const modeloVozMasculinaPiper As String =
    "pt_BR-cadu-medium"

    Private ReadOnly caminhoModeloVozFemininaPiper As String =
    IO.Path.Combine(
        pastaVozesPiper,
        "dii_pt-BR.onnx")

    Private ReadOnly caminhoConfiguracaoVozFemininaPiper As String =
    caminhoModeloVozFemininaPiper & ".json"

    Private ReadOnly caminhoConfiguracao As String =
    IO.Path.Combine(
        Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData),
        "PlayerPromocional",
        "configuracao.json")

#End Region



    Private Class ConfiguracaoPlayer

        Public Property Musicas As New List(Of String)

        Public Property TextoPromocao As String = ""

        Public Property PromocoesTexto As New List(Of PromocaoTexto)

        Public Property PromocoesTextoAleatorias As Boolean = False

        Public Property IntervaloPromocao As Integer = 1

        Public Property Volume As Integer = 70

        Public Property PromocaoAtivada As Boolean = True

        Public Property OrdemAleatoria As Boolean = False

        Public Property UsarAnuncioGravado As Boolean = False

        Public Property AnunciosGravados As New List(Of String)

        Public Property AnunciosGravadosAleatorios As Boolean = False

    End Class

    Private Sub FrmPrincipal_Load(
    sender As Object,
    e As EventArgs
) Handles MyBase.Load

        CriarControlesPromocoesTexto()

        CarregarConfiguracao()

        AtualizarListaPromocoesTexto()
        AtualizarModoAnuncio()
        AtualizarBotaoAnunciosGravados()

    End Sub

#Region "botões"

    Private Sub BtnGerenciarAnuncios_Click(sender As Object, e As EventArgs) Handles BtnGerenciarAnuncios.Click

        Using frm As New FrmAnunciosGravados(
        caminhosAnunciosGravados,
        anunciosGravadosAleatorios)

            If frm.ShowDialog(Me) <>
           DialogResult.OK Then

                Exit Sub

            End If

            caminhosAnunciosGravados.Clear()

            caminhosAnunciosGravados.AddRange(
            frm.AnunciosSelecionados)

            anunciosGravadosAleatorios =
            frm.OrdemAleatoriaSelecionada

            indiceUltimoAnuncioGravado = -1

        End Using

        AtualizarBotaoAnunciosGravados()

    End Sub

    Private Sub CriarControlesPromocoesTexto()

        If BtnGerenciarPromocoesTexto IsNot Nothing Then
            Exit Sub
        End If

        TxtPromocao.Visible = False

        BtnGerenciarPromocoesTexto =
        New Button With {
            .Name = "BtnGerenciarPromocoesTexto",
            .Text = "Gerenciar promoções",
            .Location = New Point(15, 55),
            .Size = New Size(185, 36),
            .BackColor = Color.FromArgb(50, 50, 50),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Cursor = Cursors.Hand
        }

        LblQuantidadePromocoesTexto =
        New Label With {
            .Name = "LblQuantidadePromocoesTexto",
            .Text = "Nenhuma promoção",
            .Location = New Point(215, 64),
            .Size = New Size(295, 24),
            .ForeColor = Color.FromArgb(190, 190, 190),
            .BackColor = Color.Transparent,
            .TextAlign = ContentAlignment.MiddleRight
        }

        LstPromocoesTexto =
        New ListBox With {
            .Name = "LstPromocoesTexto",
            .Location = TxtPromocao.Location,
            .Size = TxtPromocao.Size,
            .BackColor = Color.FromArgb(48, 48, 48),
            .ForeColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle,
            .HorizontalScrollbar = True,
            .IntegralHeight = False
        }

        PnlPromocao.Controls.Add(
        BtnGerenciarPromocoesTexto)

        PnlPromocao.Controls.Add(
        LblQuantidadePromocoesTexto)

        PnlPromocao.Controls.Add(
        LstPromocoesTexto)

        LstPromocoesTexto.BringToFront()

    End Sub

    Private Sub BtnGerenciarPromocoesTexto_Click(
        sender As Object,
        e As EventArgs
    ) Handles BtnGerenciarPromocoesTexto.Click

        Using frm As New FrmPromocoesTexto(
            promocoesTexto,
            promocoesTextoAleatorias)

            If frm.ShowDialog(Me) <>
               DialogResult.OK Then

                Exit Sub

            End If

            promocoesTexto.Clear()

            For Each promocao As PromocaoTexto In
                frm.PromocoesSelecionadas

                promocoesTexto.Add(
                promocao.Copiar())

            Next

            promocoesTextoAleatorias =
            frm.OrdemAleatoriaSelecionada

            ReiniciarOrdemPromocoesTexto()
            AtualizarListaPromocoesTexto()

        End Using

    End Sub

    Private Async Sub BtnTestarVoz_Click(
    sender As Object,
    e As EventArgs
) Handles BtnTestarVoz.Click

        Dim musicaEstavaTocando As Boolean =
        dispositivoSaida IsNot Nothing AndAlso
        dispositivoSaida.PlaybackState =
        PlaybackState.Playing

        CancelarNarracaoAtual()

        If musicaEstavaTocando Then

            dispositivoSaida.Pause()
            TmrPlayer.Stop()

        End If

        Try

            '========================================
            ' ANÚNCIO GRAVADO
            '========================================

            If RdbAnuncioGravado.Checked Then

                Dim caminhoAnuncio As String =
                ObterProximoAnuncioGravado()

                If String.IsNullOrWhiteSpace(
                caminhoAnuncio) Then

                    MessageBox.Show(
                    "Nenhum anúncio gravado foi adicionado.",
                    "Anúncios gravados",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning)

                    If musicaEstavaTocando AndAlso
                   dispositivoSaida IsNot Nothing Then

                        dispositivoSaida.Play()
                        TmrPlayer.Start()

                    End If

                    Exit Sub

                End If

                ReproduzirAudioPromocao(
                caminhoAnuncio,
                False,
                musicaEstavaTocando)

                Exit Sub

            End If

            '========================================
            ' ANÚNCIO GERADO PELO TEXTO
            '========================================

            Dim promocao As PromocaoTexto =
            ObterPromocaoSelecionadaParaTeste()

            If promocao Is Nothing Then

                MessageBox.Show(
                "Cadastre pelo menos uma promoção por texto.",
                "Promoções",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning)

                If BtnGerenciarPromocoesTexto IsNot Nothing Then
                    BtnGerenciarPromocoesTexto.Focus()
                End If

                If musicaEstavaTocando AndAlso
               dispositivoSaida IsNot Nothing Then

                    dispositivoSaida.Play()
                    TmrPlayer.Start()

                End If

                Exit Sub

            End If

            BtnTestarVoz.Enabled = False
            BtnTestarVoz.Text = "Gerando..."

            LblMusicaAtual.Text =
            "Gerando promoção..."

            Dim caminhoAudioGerado As String =
            Await GerarAudioPromocaoPiperAsync(
                promocao.Texto,
                promocao.Voz)

            If String.IsNullOrWhiteSpace(
                caminhoAudioGerado) Then

                BtnTestarVoz.Enabled = True

                RestaurarTextoBotaoTeste()
                RestaurarNomeMusicaAtual()

                If musicaEstavaTocando AndAlso
               dispositivoSaida IsNot Nothing AndAlso
               dispositivoSaida.PlaybackState =
               PlaybackState.Paused Then

                    dispositivoSaida.Play()
                    TmrPlayer.Start()

                End If

                Exit Sub

            End If

            ReproduzirAudioPromocao(
            caminhoAudioGerado,
            False,
            musicaEstavaTocando)

        Catch ex As Exception

            BtnTestarVoz.Enabled = True

            RestaurarTextoBotaoTeste()

            LiberarAudioPromocaoPiper()
            RestaurarNomeMusicaAtual()

            If musicaEstavaTocando AndAlso
           dispositivoSaida IsNot Nothing AndAlso
           dispositivoSaida.PlaybackState =
           PlaybackState.Paused Then

                dispositivoSaida.Play()
                TmrPlayer.Start()

            End If

            MessageBox.Show(
            "Não foi possível reproduzir o anúncio." &
            Environment.NewLine &
            ex.Message,
            "Erro",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error)

        End Try

    End Sub

    Private Sub RdbModoAnuncio_CheckedChanged(
    sender As Object,
    e As EventArgs
) Handles RdbAnuncioTexto.CheckedChanged,
          RdbAnuncioGravado.CheckedChanged

        AtualizarModoAnuncio()

    End Sub

    Private Sub BtnAdicionarMusicas_Click(
    sender As Object,
    e As EventArgs
) Handles BtnAdicionarMusicas.Click

        If OfdMusicas.ShowDialog() <> DialogResult.OK Then
            Exit Sub
        End If

        For Each caminho As String In OfdMusicas.FileNames

            Dim jaAdicionada As Boolean =
            caminhosMusicas.Exists(
                Function(item)
                    Return String.Equals(
                        item,
                        caminho,
                        StringComparison.OrdinalIgnoreCase)
                End Function)

            If jaAdicionada Then
                Continue For
            End If

            caminhosMusicas.Add(caminho)

            LstMusicas.Items.Add(
            IO.Path.GetFileNameWithoutExtension(caminho))

        Next

        If LstMusicas.SelectedIndex = -1 AndAlso
       LstMusicas.Items.Count > 0 Then

            LstMusicas.SelectedIndex = 0

        End If

    End Sub

    Private Sub BtnAdicionarPasta_Click(
    sender As Object,
    e As EventArgs
) Handles BtnAdicionarPasta.Click

        If FbdPlaylist.ShowDialog() <> DialogResult.OK Then
            Exit Sub
        End If

        Dim extensoesPermitidas As String() = {
            ".mp3",
            ".wav",
            ".wma",
            ".aac",
            ".m4a"
        }

        Dim arquivos As String() =
            IO.Directory.GetFiles(
                FbdPlaylist.SelectedPath,
                "*.*",
                IO.SearchOption.TopDirectoryOnly)

        For Each caminho As String In arquivos

            Dim extensao As String =
                IO.Path.GetExtension(caminho).ToLowerInvariant()

            If Not extensoesPermitidas.Contains(extensao) Then
                Continue For
            End If

            Dim jaAdicionada As Boolean =
                caminhosMusicas.Exists(
                    Function(item)
                        Return String.Equals(
                            item,
                            caminho,
                            StringComparison.OrdinalIgnoreCase)
                    End Function)

            If jaAdicionada Then
                Continue For
            End If

            caminhosMusicas.Add(caminho)

            LstMusicas.Items.Add(
                IO.Path.GetFileNameWithoutExtension(caminho))

        Next

        If LstMusicas.SelectedIndex = -1 AndAlso
           LstMusicas.Items.Count > 0 Then

            LstMusicas.SelectedIndex = 0

        End If

    End Sub

    Private Sub BtnRemoverMusica_Click(
    sender As Object,
    e As EventArgs
) Handles BtnRemoverMusica.Click

        Dim indiceSelecionado As Integer =
        LstMusicas.SelectedIndex

        If indiceSelecionado < 0 Then

            MessageBox.Show(
            "Selecione uma música para remover.",
            "Aviso",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning)

            Exit Sub

        End If

        Dim removendoMusicaAtual As Boolean =
        indiceSelecionado = indiceMusicaAtual

        If removendoMusicaAtual Then

            LiberarAudioAtual()

            indiceMusicaAtual = -1

            TrkProgresso.Value = 0
            LblTempoAtual.Text = "00:00"
            LblTempoTotal.Text = "00:00"

            LblMusicaAtual.Text =
            "Nenhuma música sendo reproduzida"

        ElseIf indiceSelecionado < indiceMusicaAtual Then

            indiceMusicaAtual -= 1

        End If

        caminhosMusicas.RemoveAt(
        indiceSelecionado)

        LstMusicas.Items.RemoveAt(
        indiceSelecionado)

        If LstMusicas.Items.Count = 0 Then
            Exit Sub
        End If

        If indiceSelecionado >=
       LstMusicas.Items.Count Then

            indiceSelecionado =
            LstMusicas.Items.Count - 1

        End If

        LstMusicas.SelectedIndex =
        indiceSelecionado

    End Sub

    Private Sub BtnLimparPlaylist_Click(
    sender As Object,
    e As EventArgs
) Handles BtnLimparPlaylist.Click

        If LstMusicas.Items.Count = 0 Then
            Exit Sub
        End If

        Dim resposta As DialogResult =
        MessageBox.Show(
            "Deseja limpar toda a playlist?",
            "Limpar playlist",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question)

        If resposta <> DialogResult.Yes Then
            Exit Sub
        End If

        TmrPlayer.Stop()

        narracaoAutomaticaEmAndamento = False
        indiceAposNarracao = -1
        musicasDesdeUltimaPromocao = 0


        LiberarAudioAtual()

        indiceMusicaAtual = -1

        historicoAleatorio.Clear()
        voltandoPeloHistorico = False

        caminhosMusicas.Clear()
        LstMusicas.Items.Clear()

        TrkProgresso.Value = 0

        LblTempoAtual.Text = "00:00"
        LblTempoTotal.Text = "00:00"

        LblMusicaAtual.Text =
        "Nenhuma música sendo reproduzida"

    End Sub

    Private Sub BtnPausar_Click(
    sender As Object,
    e As EventArgs
) Handles BtnPausar.Click

        If dispositivoSaida Is Nothing Then
            Exit Sub
        End If

        If dispositivoSaida.PlaybackState =
       PlaybackState.Playing Then

            dispositivoSaida.Pause()
            TmrPlayer.Stop()

        End If

    End Sub

    Private Sub BtnParar_Click(
    sender As Object,
    e As EventArgs
) Handles BtnParar.Click

        LiberarAudioAtual()

        indiceMusicaAtual = -1

        TrkProgresso.Value = 0

        LblTempoAtual.Text = "00:00"
        LblTempoTotal.Text = "00:00"

        LblMusicaAtual.Text =
        "Nenhuma música sendo reproduzida"

        CancelarNarracaoAtual()

    End Sub

    Private Sub BtnProxima_Click(
    sender As Object,
    e As EventArgs
) Handles BtnProxima.Click

        If caminhosMusicas.Count = 0 Then
            Exit Sub
        End If

        CancelarNarracaoAtual()

        Dim proximoIndice As Integer =
    ObterProximoIndice()

        If proximoIndice < 0 Then
            Exit Sub
        End If

        ReproduzirMusica(proximoIndice)

    End Sub

    Private Sub BtnPlay_Click(sender As Object, e As EventArgs) Handles BtnPlay.Click


        If caminhosMusicas.Count = 0 Then

            MessageBox.Show(
            "Adicione pelo menos uma música à playlist.",
            "Playlist vazia",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning)

            Exit Sub

        End If

        If dispositivoSaida IsNot Nothing AndAlso
       dispositivoSaida.PlaybackState =
       PlaybackState.Paused Then

            dispositivoSaida.Play()
            TmrPlayer.Start()

            Exit Sub

        End If

        Dim estavaNarrando As Boolean =
        narracaoAutomaticaEmAndamento

        Dim indicePendente As Integer =
        indiceAposNarracao

        CancelarNarracaoAtual()

        Dim indice As Integer

        If estavaNarrando AndAlso
       indicePendente >= 0 AndAlso
       indicePendente < caminhosMusicas.Count Then

            indice = indicePendente

        Else

            indice = LstMusicas.SelectedIndex

            If indice < 0 Then
                indice = 0
            End If

        End If

        ReproduzirMusica(indice)

    End Sub

    Private Sub BtnAnterior_Click(
    sender As Object,
    e As EventArgs
) Handles BtnAnterior.Click

        If caminhosMusicas.Count = 0 Then
            Exit Sub
        End If

        CancelarNarracaoAtual()

        If ChkOrdemAleatoria.Checked Then

            While historicoAleatorio.Count > 0

                Dim caminhoAnterior As String =
            historicoAleatorio.Pop()

                Dim indiceAnteriorAleatorio As Integer =
            caminhosMusicas.FindIndex(
                Function(caminho)
                    Return String.Equals(
                        caminho,
                        caminhoAnterior,
                        StringComparison.OrdinalIgnoreCase)
                End Function)

                If indiceAnteriorAleatorio >= 0 Then

                    voltandoPeloHistorico = True

                    ReproduzirMusica(
                indiceAnteriorAleatorio)

                    Exit Sub

                End If

            End While

        End If


        Dim indiceAnterior As Integer

        If indiceMusicaAtual < 0 Then

            indiceAnterior =
            If(LstMusicas.SelectedIndex >= 0,
               LstMusicas.SelectedIndex,
               0)

        Else

            indiceAnterior =
            indiceMusicaAtual - 1

            If indiceAnterior < 0 Then
                indiceAnterior =
                caminhosMusicas.Count - 1
            End If

        End If

        ReproduzirMusica(indiceAnterior)

    End Sub

    Private Sub ChkAtivarPromocao_CheckedChanged(sender As Object, e As EventArgs) Handles ChkAtivarPromocao.CheckedChanged

        Dim ativada As Boolean =
        ChkAtivarPromocao.Checked

        NudIntervalo.Enabled = ativada
        LblIntervalo.Enabled = ativada
        LblMusicas.Enabled = ativada

        If ativada OrElse
       Not narracaoAutomaticaEmAndamento Then

            Exit Sub

        End If

        Dim proximoIndice As Integer =
        indiceAposNarracao

        narracaoAutomaticaEmAndamento = False
        indiceAposNarracao = -1
        musicasDesdeUltimaPromocao = 0

        LiberarAudioPromocaoPiper()

        If proximoIndice >= 0 AndAlso
       proximoIndice < caminhosMusicas.Count Then

            ReproduzirMusica(proximoIndice)

        End If

    End Sub

    Private Sub ChkOrdemAleatoria_CheckedChanged(
    sender As Object,
    e As EventArgs
) Handles ChkOrdemAleatoria.CheckedChanged

        historicoAleatorio.Clear()
        voltandoPeloHistorico = False

    End Sub

#End Region

#Region "Funções"

    Private Sub TrkProgresso_Scroll(sender As Object, e As EventArgs) Handles TrkProgresso.Scroll

        If leitorAudio Is Nothing Then
            Exit Sub
        End If

        leitorAudio.CurrentTime =
        TimeSpan.FromSeconds(
            TrkProgresso.Value)

        LblTempoAtual.Text =
        FormatarTempo(
            leitorAudio.CurrentTime)

    End Sub

    Private Sub TrkVolume_Scroll(
    sender As Object,
    e As EventArgs
) Handles TrkVolume.Scroll

        If leitorAudio Is Nothing Then
            Exit Sub
        End If

        leitorAudio.Volume =
        CSng(TrkVolume.Value / 100.0)

    End Sub

    Private Sub TmrPlayer_Tick(
    sender As Object,
    e As EventArgs
) Handles TmrPlayer.Tick

        If leitorAudio Is Nothing OrElse
       dispositivoSaida Is Nothing Then

            Exit Sub

        End If

        Dim segundosAtuais As Integer =
        CInt(Math.Floor(
            leitorAudio.CurrentTime.TotalSeconds))

        segundosAtuais =
        Math.Max(
            TrkProgresso.Minimum,
            Math.Min(
                segundosAtuais,
                TrkProgresso.Maximum))

        TrkProgresso.Value =
        segundosAtuais

        LblTempoAtual.Text =
        FormatarTempo(
            leitorAudio.CurrentTime)

        Dim musicaTerminou As Boolean =
        dispositivoSaida.PlaybackState =
        PlaybackState.Stopped AndAlso
        leitorAudio.CurrentTime >=
        leitorAudio.TotalTime.Subtract(
            TimeSpan.FromMilliseconds(700))

        If Not musicaTerminou Then
            Exit Sub
        End If

        TmrPlayer.Stop()

        Dim proximoIndice As Integer =
    ObterProximoIndice()

        If proximoIndice < 0 Then
            Exit Sub
        End If

        If ChkAtivarPromocao.Checked Then

            musicasDesdeUltimaPromocao += 1

        Else

            musicasDesdeUltimaPromocao = 0

        End If

        Dim intervalo As Integer =
    CInt(NudIntervalo.Value)

        Dim possuiPromocao As Boolean

        If RdbAnuncioGravado.Checked Then

            possuiPromocao =
        caminhosAnunciosGravados.Any(
            Function(caminho)
                Return IO.File.Exists(caminho)
            End Function)

        Else

            possuiPromocao =
            promocoesTexto.Any(
                Function(promocao)
                    Return promocao IsNot Nothing AndAlso
                           Not String.IsNullOrWhiteSpace(
                               promocao.Texto)
                End Function)

        End If

        If ChkAtivarPromocao.Checked AndAlso possuiPromocao AndAlso musicasDesdeUltimaPromocao >= intervalo Then

            musicasDesdeUltimaPromocao = 0

            ExecutarAnuncioAutomatico(
    proximoIndice)

        Else

            ReproduzirMusica(
                proximoIndice)

        End If

    End Sub

    Private Async Sub NarrarPromocaoAutomatica(
        proximoIndice As Integer,
        promocao As PromocaoTexto)

        If promocao Is Nothing OrElse
           String.IsNullOrWhiteSpace(
               promocao.Texto) Then

            ReproduzirMusica(
            proximoIndice)

            Exit Sub

        End If

        Try

            LiberarAudioAtual()

            narracaoAutomaticaEmAndamento = True
            indiceAposNarracao = proximoIndice

            LblMusicaAtual.Text =
            "Gerando promoção..."

            TrkProgresso.Value = 0
            LblTempoAtual.Text = "00:00"
            LblTempoTotal.Text = "00:00"

            Dim caminhoAudioGerado As String =
            Await GerarAudioPromocaoPiperAsync(
                promocao.Texto,
                promocao.Voz)

            If String.IsNullOrWhiteSpace(
                caminhoAudioGerado) Then

                narracaoAutomaticaEmAndamento = False
                indiceAposNarracao = -1

                ReproduzirMusica(
                proximoIndice)

                Exit Sub

            End If

            If Not narracaoAutomaticaEmAndamento Then
                Exit Sub
            End If

            ReproduzirAudioPromocao(
                caminhoAudioGerado,
                True,
                False,
                proximoIndice)

        Catch ex As Exception

            narracaoAutomaticaEmAndamento = False
            indiceAposNarracao = -1

            LiberarAudioPromocaoPiper()

            ReproduzirMusica(
            proximoIndice)

            MessageBox.Show(
            "Não foi possível narrar a promoção." &
            Environment.NewLine &
            ex.Message,
            "Erro",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error)

        End Try

    End Sub
    Private Sub AtualizarModoAnuncio()

        Dim usarTexto As Boolean =
        RdbAnuncioTexto.Checked

        TxtPromocao.Enabled = False

        If LstPromocoesTexto IsNot Nothing Then
            LstPromocoesTexto.Enabled = usarTexto
        End If

        If BtnGerenciarPromocoesTexto IsNot Nothing Then
            BtnGerenciarPromocoesTexto.Enabled = usarTexto
        End If

        BtnGerenciarAnuncios.Enabled =
        Not usarTexto

        RestaurarTextoBotaoTeste()

    End Sub

    Private Sub ReproduzirMusica(indice As Integer)

        If indice < 0 OrElse
       indice >= caminhosMusicas.Count Then

            Exit Sub

        End If

        LiberarAudioAtual()

        Try

            If ChkOrdemAleatoria.Checked AndAlso
   Not voltandoPeloHistorico AndAlso
   indiceMusicaAtual >= 0 AndAlso
   indiceMusicaAtual < caminhosMusicas.Count AndAlso
   indiceMusicaAtual <> indice Then

                historicoAleatorio.Push(
        caminhosMusicas(indiceMusicaAtual))

            End If

            voltandoPeloHistorico = False

            indiceMusicaAtual = indice

            leitorAudio =
            New AudioFileReader(
                caminhosMusicas(indice))

            leitorAudio.Volume =
            CSng(TrkVolume.Value / 100.0)

            dispositivoSaida =
            New WaveOutEvent()

            dispositivoSaida.Init(leitorAudio)

            LstMusicas.SelectedIndex = indice

            LblMusicaAtual.Text =
            IO.Path.GetFileNameWithoutExtension(
                caminhosMusicas(indice))

            TrkProgresso.Minimum = 0

            TrkProgresso.Maximum =
            Math.Max(
                1,
                CInt(leitorAudio.TotalTime.TotalSeconds))

            TrkProgresso.Value = 0

            LblTempoAtual.Text = "00:00"

            LblTempoTotal.Text =
            FormatarTempo(
                leitorAudio.TotalTime)

            dispositivoSaida.Play()
            TmrPlayer.Start()

        Catch ex As Exception

            LiberarAudioAtual()

            MessageBox.Show(
            "Não foi possível reproduzir a música." &
            Environment.NewLine &
            ex.Message,
            "Erro",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error)

        End Try

    End Sub

    Private Sub LiberarAudioAtual()

        TmrPlayer.Stop()

        If dispositivoSaida IsNot Nothing Then

            dispositivoSaida.Stop()
            dispositivoSaida.Dispose()
            dispositivoSaida = Nothing

        End If

        If leitorAudio IsNot Nothing Then

            leitorAudio.Dispose()
            leitorAudio = Nothing

        End If

    End Sub

    Private Function FormatarTempo(
    tempo As TimeSpan
) As String

        Dim minutos As Integer =
            CInt(Math.Floor(
                tempo.TotalMinutes))

        Return $"{minutos:00}:{tempo.Seconds:00}"

    End Function

    Private Sub LstMusicas_DoubleClick(
    sender As Object,
    e As EventArgs
) Handles LstMusicas.DoubleClick

        Dim indiceSelecionado As Integer =
        LstMusicas.SelectedIndex

        If indiceSelecionado < 0 Then
            Exit Sub
        End If

        narracaoAutomaticaEmAndamento = False
        indiceAposNarracao = -1

        narracaoTesteEmAndamento = False
        retomarMusicaAposTeste = False

        ReproduzirMusica(
        indiceSelecionado)

    End Sub

    Private Sub CancelarNarracaoAtual()

        narracaoAutomaticaEmAndamento = False
        indiceAposNarracao = -1

        narracaoTesteEmAndamento = False
        retomarMusicaAposTeste = False

        BtnTestarVoz.Enabled = True

        RestaurarTextoBotaoTeste()

        LiberarAudioPromocaoPiper()

    End Sub

    Private Sub SalvarConfiguracao()

        Try

            Dim pastaConfiguracao As String =
            IO.Path.GetDirectoryName(
                caminhoConfiguracao)

            IO.Directory.CreateDirectory(
            pastaConfiguracao)

            Dim configuracao As New ConfiguracaoPlayer With {
                .UsarAnuncioGravado = RdbAnuncioGravado.Checked,
                .AnunciosGravados =
                    New List(Of String)(caminhosAnunciosGravados),
                .AnunciosGravadosAleatorios =
                    anunciosGravadosAleatorios,
                .OrdemAleatoria = ChkOrdemAleatoria.Checked,
                .Musicas = New List(Of String)(caminhosMusicas),
                .PromocaoAtivada = ChkAtivarPromocao.Checked,
                .TextoPromocao =
                    If(promocoesTexto.Count > 0,
                       promocoesTexto(0).Texto,
                       ""),
                .PromocoesTexto = CopiarPromocoesTexto(),
                .PromocoesTextoAleatorias = promocoesTextoAleatorias,
                .IntervaloPromocao = CInt(NudIntervalo.Value),
                .Volume = TrkVolume.Value
            }

            Dim opcoes As New JsonSerializerOptions With {
            .WriteIndented = True
        }

            Dim conteudoJson As String =
            JsonSerializer.Serialize(
                configuracao,
                opcoes)

            IO.File.WriteAllText(
            caminhoConfiguracao,
            conteudoJson)

        Catch ex As Exception

            MessageBox.Show(
            "Não foi possível salvar as configurações." &
            Environment.NewLine &
            ex.Message,
            "Aviso",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning)

        End Try

    End Sub

    Private Sub CarregarConfiguracao()

        If Not IO.File.Exists(caminhoConfiguracao) Then
            Exit Sub
        End If

        Try

            Dim conteudoJson As String =
            IO.File.ReadAllText(
                caminhoConfiguracao)

            Dim configuracao As ConfiguracaoPlayer =
            JsonSerializer.Deserialize(
                Of ConfiguracaoPlayer)(
                conteudoJson)

            If configuracao Is Nothing Then
                Exit Sub
            End If

            promocoesTexto.Clear()

            If configuracao.PromocoesTexto IsNot Nothing Then

                For Each promocao As PromocaoTexto In
                    configuracao.PromocoesTexto

                    If promocao Is Nothing OrElse
                       String.IsNullOrWhiteSpace(
                           promocao.Texto) Then

                        Continue For

                    End If

                    promocoesTexto.Add(
                    New PromocaoTexto With {
                        .Texto = promocao.Texto.Trim(),
                        .Voz = NormalizarVozPromocao(
                            promocao.Voz)
                    })

                Next

            End If

            If promocoesTexto.Count = 0 AndAlso
               Not String.IsNullOrWhiteSpace(
                   configuracao.TextoPromocao) Then

                promocoesTexto.Add(
                New PromocaoTexto With {
                    .Texto = configuracao.TextoPromocao.Trim(),
                    .Voz = PromocaoTexto.VozMasculina
                })

            End If

            promocoesTextoAleatorias =
            configuracao.PromocoesTextoAleatorias

            ReiniciarOrdemPromocoesTexto()
            AtualizarListaPromocoesTexto()

            NudIntervalo.Value =
            Math.Max(
                NudIntervalo.Minimum,
                Math.Min(
                    configuracao.IntervaloPromocao,
                    NudIntervalo.Maximum))

            ChkAtivarPromocao.Checked = configuracao.PromocaoAtivada

            ChkOrdemAleatoria.Checked = configuracao.OrdemAleatoria

            RdbAnuncioGravado.Checked =
    configuracao.UsarAnuncioGravado

            RdbAnuncioTexto.Checked =
    Not configuracao.UsarAnuncioGravado

            caminhosAnunciosGravados.Clear()

            If configuracao.AnunciosGravados IsNot Nothing Then

                For Each caminho As String In
        configuracao.AnunciosGravados

                    If Not IO.File.Exists(caminho) Then
                        Continue For
                    End If

                    Dim jaAdicionado As Boolean =
            caminhosAnunciosGravados.Exists(
                Function(item)
                    Return String.Equals(
                        item,
                        caminho,
                        StringComparison.OrdinalIgnoreCase)
                End Function)

                    If Not jaAdicionado Then

                        caminhosAnunciosGravados.Add(
                caminho)

                    End If

                Next

            End If

            anunciosGravadosAleatorios =
    configuracao.AnunciosGravadosAleatorios

            indiceUltimoAnuncioGravado = -1

            AtualizarBotaoAnunciosGravados()
            AtualizarModoAnuncio()

            TrkVolume.Value =
            Math.Max(
                TrkVolume.Minimum,
                Math.Min(
                    configuracao.Volume,
                    TrkVolume.Maximum))

            caminhosMusicas.Clear()
            LstMusicas.Items.Clear()

            If configuracao.Musicas IsNot Nothing Then

                For Each caminho As String In configuracao.Musicas

                    If Not IO.File.Exists(caminho) Then
                        Continue For
                    End If

                    If caminhosMusicas.Contains(caminho) Then
                        Continue For
                    End If

                    caminhosMusicas.Add(caminho)

                    LstMusicas.Items.Add(
                    IO.Path.GetFileNameWithoutExtension(
                        caminho))

                Next

            End If

            If LstMusicas.Items.Count > 0 Then
                LstMusicas.SelectedIndex = 0
            End If


        Catch ex As Exception

            MessageBox.Show(
            "Não foi possível carregar as configurações salvas." &
            Environment.NewLine &
            ex.Message,
            "Aviso",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning)

        End Try

    End Sub

    Private Sub AtualizarBotaoAnunciosGravados()

        Dim quantidade As Integer =
        caminhosAnunciosGravados.Count

        If quantidade = 0 Then

            BtnGerenciarAnuncios.Text =
            "Gerenciar anúncios"

        ElseIf quantidade = 1 Then

            BtnGerenciarAnuncios.Text =
            "Gerenciar anúncios (1)"

        Else

            BtnGerenciarAnuncios.Text =
            $"Gerenciar anúncios ({quantidade})"

        End If

    End Sub

    Private Sub ReproduzirAudioPromocao(
    caminhoAudio As String,
    narracaoAutomatica As Boolean,
    deveRetomarMusica As Boolean,
    Optional proximoIndice As Integer = -1)

        If String.IsNullOrWhiteSpace(caminhoAudio) OrElse
       Not IO.File.Exists(caminhoAudio) Then

            Exit Sub

        End If

        LiberarAudioPromocaoPiper()

        leitorPromocaoPiper =
        New AudioFileReader(caminhoAudio)

        leitorPromocaoPiper.Volume = 1.0F

        dispositivoPromocaoPiper =
        New WaveOutEvent()

        AddHandler dispositivoPromocaoPiper.PlaybackStopped,
        AddressOf DispositivoPromocaoPiper_PlaybackStopped

        dispositivoPromocaoPiper.Init(
        leitorPromocaoPiper)

        narracaoAutomaticaEmAndamento =
        narracaoAutomatica

        narracaoTesteEmAndamento =
        Not narracaoAutomatica

        retomarMusicaAposTeste =
        deveRetomarMusica

        indiceAposNarracao =
        proximoIndice

        If Not narracaoAutomatica Then

            BtnTestarVoz.Enabled = False
            BtnTestarVoz.Text = "Narrando..."

        End If

        LblMusicaAtual.Text =
        "Reproduzindo anúncio..."

        dispositivoPromocaoPiper.Play()

    End Sub

    Private Sub LiberarAudioPromocaoPiper()

        If dispositivoPromocaoPiper IsNot Nothing Then

            RemoveHandler dispositivoPromocaoPiper.PlaybackStopped,
            AddressOf DispositivoPromocaoPiper_PlaybackStopped

            dispositivoPromocaoPiper.Stop()
            dispositivoPromocaoPiper.Dispose()

            dispositivoPromocaoPiper = Nothing

        End If

        If leitorPromocaoPiper IsNot Nothing Then

            leitorPromocaoPiper.Dispose()
            leitorPromocaoPiper = Nothing

        End If

    End Sub

    Private Sub RestaurarNomeMusicaAtual()

        If indiceMusicaAtual >= 0 AndAlso
       indiceMusicaAtual < caminhosMusicas.Count Then

            LblMusicaAtual.Text =
            IO.Path.GetFileNameWithoutExtension(
                caminhosMusicas(indiceMusicaAtual))

        Else

            LblMusicaAtual.Text =
            "Nenhuma música sendo reproduzida"

        End If

    End Sub

    Private Sub DispositivoPromocaoPiper_PlaybackStopped(
    sender As Object,
    e As StoppedEventArgs)

        If Me.InvokeRequired Then

            Me.BeginInvoke(
            Sub()
                DispositivoPromocaoPiper_PlaybackStopped(
                    sender,
                    e)
            End Sub)

            Exit Sub

        End If

        Dim eraNarracaoAutomatica As Boolean =
        narracaoAutomaticaEmAndamento

        Dim eraNarracaoTeste As Boolean =
        narracaoTesteEmAndamento

        Dim deveRetomarMusica As Boolean =
        retomarMusicaAposTeste

        Dim proximoIndice As Integer =
        indiceAposNarracao

        Dim erroReproducao As Exception =
        e.Exception

        narracaoAutomaticaEmAndamento = False
        narracaoTesteEmAndamento = False
        retomarMusicaAposTeste = False
        indiceAposNarracao = -1

        BtnTestarVoz.Enabled = True
        RestaurarTextoBotaoTeste()

        LiberarAudioPromocaoPiper()

        If erroReproducao IsNot Nothing Then

            MessageBox.Show(
            "Ocorreu um erro ao reproduzir a promoção." &
            Environment.NewLine &
            erroReproducao.Message,
            "Erro",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error)

        End If

        If eraNarracaoAutomatica Then

            If proximoIndice >= 0 AndAlso
           proximoIndice < caminhosMusicas.Count Then

                ReproduzirMusica(
                proximoIndice)

            End If

            Exit Sub

        End If

        If eraNarracaoTeste AndAlso
       deveRetomarMusica AndAlso
       dispositivoSaida IsNot Nothing AndAlso
       dispositivoSaida.PlaybackState =
       PlaybackState.Paused Then

            dispositivoSaida.Play()
            TmrPlayer.Start()

        Else

            RestaurarNomeMusicaAtual()

        End If

    End Sub

    Private Sub AdicionarArquivoNaPlaylist(
    caminho As String)

        Dim extensoesPermitidas As String() = {
        ".mp3",
        ".wav",
        ".wma",
        ".aac",
        ".m4a"
    }

        If Not IO.File.Exists(caminho) Then
            Exit Sub
        End If

        Dim extensao As String =
        IO.Path.GetExtension(caminho).
        ToLowerInvariant()

        If Not extensoesPermitidas.Contains(extensao) Then
            Exit Sub
        End If

        Dim jaAdicionada As Boolean =
        caminhosMusicas.Exists(
            Function(item)
                Return String.Equals(
                    item,
                    caminho,
                    StringComparison.OrdinalIgnoreCase)
            End Function)

        If jaAdicionada Then
            Exit Sub
        End If

        caminhosMusicas.Add(caminho)

        LstMusicas.Items.Add(
        IO.Path.GetFileNameWithoutExtension(caminho))

    End Sub

    Private Sub LstMusicas_DragEnter(
    sender As Object,
    e As DragEventArgs
) Handles LstMusicas.DragEnter

        If e.Data.GetDataPresent(
        DataFormats.FileDrop) Then

            e.Effect =
            DragDropEffects.Copy

        Else

            e.Effect =
            DragDropEffects.None

        End If

    End Sub

    Private Sub LstMusicas_DragDrop(
    sender As Object,
    e As DragEventArgs
) Handles LstMusicas.DragDrop

        Dim itensArrastados As String() =
        DirectCast(
            e.Data.GetData(DataFormats.FileDrop),
            String())

        For Each caminho As String In itensArrastados

            If IO.Directory.Exists(caminho) Then

                Try

                    For Each arquivo As String In
                    IO.Directory.GetFiles(
                        caminho,
                        "*.*",
                        IO.SearchOption.TopDirectoryOnly)

                        AdicionarArquivoNaPlaylist(
                        arquivo)

                    Next

                Catch ex As Exception

                    MessageBox.Show(
                    "Não foi possível abrir a pasta." &
                    Environment.NewLine &
                    ex.Message,
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning)

                End Try

            Else

                AdicionarArquivoNaPlaylist(
                caminho)

            End If

        Next

        If LstMusicas.SelectedIndex = -1 AndAlso
       LstMusicas.Items.Count > 0 Then

            LstMusicas.SelectedIndex = 0

        End If

    End Sub

    Private Function ObterProximoIndice() As Integer

        If caminhosMusicas.Count = 0 Then
            Return -1
        End If

        If ChkOrdemAleatoria.Checked AndAlso
       caminhosMusicas.Count > 1 Then

            Dim novoIndice As Integer

            Do

                novoIndice =
                geradorAleatorio.Next(
                    caminhosMusicas.Count)

            Loop While novoIndice = indiceMusicaAtual

            Return novoIndice

        End If

        Dim proximoIndice As Integer =
        indiceMusicaAtual + 1

        If proximoIndice < 0 OrElse
       proximoIndice >= caminhosMusicas.Count Then

            proximoIndice = 0

        End If

        Return proximoIndice

    End Function

    Private Sub AtualizarListaPromocoesTexto()

        If LstPromocoesTexto Is Nothing OrElse
           LblQuantidadePromocoesTexto Is Nothing Then

            Exit Sub

        End If

        Dim indiceSelecionado As Integer =
        LstPromocoesTexto.SelectedIndex

        LstPromocoesTexto.Items.Clear()

        For Each promocao As PromocaoTexto In promocoesTexto

            Dim textoResumido As String =
            promocao.Texto.Replace(
                Environment.NewLine,
                " ").Trim()

            If textoResumido.Length > 80 Then

                textoResumido =
                textoResumido.Substring(0, 77) & "..."

            End If

            LstPromocoesTexto.Items.Add(
                "[" &
                NormalizarVozPromocao(promocao.Voz) &
                "] " &
                textoResumido)

        Next

        If LstPromocoesTexto.Items.Count > 0 Then

            If indiceSelecionado < 0 OrElse
               indiceSelecionado >=
               LstPromocoesTexto.Items.Count Then

                indiceSelecionado = 0

            End If

            LstPromocoesTexto.SelectedIndex =
            indiceSelecionado

        End If

        If promocoesTexto.Count = 0 Then

            LblQuantidadePromocoesTexto.Text =
            "Nenhuma promoção"

        ElseIf promocoesTexto.Count = 1 Then

            LblQuantidadePromocoesTexto.Text =
            "1 promoção cadastrada"

        Else

            LblQuantidadePromocoesTexto.Text =
            promocoesTexto.Count.ToString() &
            " promoções cadastradas"

        End If

    End Sub

    Private Function ObterPromocaoSelecionadaParaTeste() As PromocaoTexto

        If promocoesTexto.Count = 0 Then
            Return Nothing
        End If

        Dim indice As Integer = 0

        If LstPromocoesTexto IsNot Nothing AndAlso
           LstPromocoesTexto.SelectedIndex >= 0 AndAlso
           LstPromocoesTexto.SelectedIndex <
           promocoesTexto.Count Then

            indice =
            LstPromocoesTexto.SelectedIndex

        End If

        Return promocoesTexto(indice)

    End Function

    Private Function ObterProximaPromocaoTexto() As PromocaoTexto

        If promocoesTexto.Count = 0 Then
            Return Nothing
        End If

        Dim indice As Integer

        If promocoesTextoAleatorias AndAlso
           promocoesTexto.Count > 1 Then

            If filaPromocoesTextoAleatorias.Count = 0 Then
                PreencherFilaPromocoesTextoAleatorias()
            End If

            indice =
            filaPromocoesTextoAleatorias.Dequeue()

        Else

            indice =
            indiceUltimaPromocaoTexto + 1

            If indice < 0 OrElse
               indice >= promocoesTexto.Count Then

                indice = 0

            End If

        End If

        indiceUltimaPromocaoTexto =
        indice

        If LstPromocoesTexto IsNot Nothing AndAlso
           indice >= 0 AndAlso
           indice < LstPromocoesTexto.Items.Count Then

            LstPromocoesTexto.SelectedIndex =
            indice

        End If

        Return promocoesTexto(indice)

    End Function

    Private Sub PreencherFilaPromocoesTextoAleatorias()

        filaPromocoesTextoAleatorias.Clear()

        Dim indices As New List(Of Integer)

        For indice As Integer = 0 To promocoesTexto.Count - 1

            indices.Add(indice)

        Next

        For indice As Integer =
            indices.Count - 1 To 1 Step -1

            Dim indiceTroca As Integer =
            geradorAleatorio.Next(indice + 1)

            Dim temporario As Integer =
            indices(indice)

            indices(indice) =
            indices(indiceTroca)

            indices(indiceTroca) =
            temporario

        Next

        If indices.Count > 1 AndAlso
           indices(0) = indiceUltimaPromocaoTexto Then

            Dim temporario As Integer =
            indices(0)

            indices(0) =
            indices(1)

            indices(1) =
            temporario

        End If

        For Each indice As Integer In indices
            filaPromocoesTextoAleatorias.Enqueue(indice)
        Next

    End Sub

    Private Sub ReiniciarOrdemPromocoesTexto()

        indiceUltimaPromocaoTexto = -1
        filaPromocoesTextoAleatorias.Clear()

    End Sub

    Private Function NormalizarVozPromocao(
        voz As String) As String

        If String.Equals(
            voz,
            PromocaoTexto.VozFeminina,
            StringComparison.OrdinalIgnoreCase) Then

            Return PromocaoTexto.VozFeminina

        End If

        Return PromocaoTexto.VozMasculina

    End Function

    Private Function CopiarPromocoesTexto() As List(Of PromocaoTexto)

        Dim resultado As New List(Of PromocaoTexto)

        For Each promocao As PromocaoTexto In promocoesTexto
            resultado.Add(promocao.Copiar())
        Next

        Return resultado

    End Function

    Private Function ObterModeloVozPiper(
        voz As String) As String

        If Not String.Equals(
            NormalizarVozPromocao(voz),
            PromocaoTexto.VozFeminina,
            StringComparison.OrdinalIgnoreCase) Then

            Return modeloVozMasculinaPiper

        End If

        If Not IO.File.Exists(
            caminhoModeloVozFemininaPiper) OrElse
           Not IO.File.Exists(
            caminhoConfiguracaoVozFemininaPiper) Then

            Throw New IO.FileNotFoundException(
                "A voz feminina ainda não foi instalada." &
                Environment.NewLine &
                "Coloque os arquivos dii_pt-BR.onnx e " &
                "dii_pt-BR.onnx.json na pasta:" &
                Environment.NewLine &
                pastaVozesPiper)

        End If

        Return caminhoModeloVozFemininaPiper

    End Function

    Private Function CriarHashAudioPromocao(
        texto As String,
        modelo As String) As String

        Dim conteudo As String =
        modelo &
        Environment.NewLine &
        texto

        Dim bytes As Byte() =
        Encoding.UTF8.GetBytes(conteudo)

        Using algoritmo As SHA256 =
            SHA256.Create()

            Return Convert.ToHexString(
                algoritmo.ComputeHash(bytes)).ToLowerInvariant()

        End Using

    End Function

    Private Async Function GerarAudioPromocaoPiperAsync(
        texto As String,
        voz As String
    ) As Task(Of String)

        texto = texto.Trim()

        If texto = "" Then
            Return ""
        End If

        Dim caminhoAudioGerado As String = ""

        Try

            IO.Directory.CreateDirectory(
            pastaPiper)

            IO.Directory.CreateDirectory(
            pastaVozesPiper)

            IO.Directory.CreateDirectory(
            pastaCachePromocoesPiper)

            Dim modelo As String =
            ObterModeloVozPiper(voz)

            Dim nomeArquivo As String =
            CriarHashAudioPromocao(
                texto,
                modelo) &
            ".wav"

            caminhoAudioGerado =
            IO.Path.Combine(
                pastaCachePromocoesPiper,
                nomeArquivo)

            If IO.File.Exists(caminhoAudioGerado) AndAlso
               New IO.FileInfo(caminhoAudioGerado).Length > 44 Then

                Return caminhoAudioGerado

            End If

            If IO.File.Exists(caminhoAudioGerado) Then

                IO.File.Delete(
                caminhoAudioGerado)

            End If

            Dim informacoes As New ProcessStartInfo With {
                .FileName = "py",
                .UseShellExecute = False,
                .CreateNoWindow = True,
                .RedirectStandardOutput = True,
                .RedirectStandardError = True
            }

            informacoes.ArgumentList.Add("-m")
            informacoes.ArgumentList.Add("piper")

            informacoes.ArgumentList.Add("--data-dir")
            informacoes.ArgumentList.Add(pastaVozesPiper)

            informacoes.ArgumentList.Add("-m")
            informacoes.ArgumentList.Add(modelo)

            informacoes.ArgumentList.Add("-f")
            informacoes.ArgumentList.Add(caminhoAudioGerado)

            informacoes.ArgumentList.Add("--")
            informacoes.ArgumentList.Add(texto)

            Using processo As New Process()

                processo.StartInfo =
                informacoes

                processo.Start()

                Dim tarefaSaida =
                processo.StandardOutput.ReadToEndAsync()

                Dim tarefaErro =
                processo.StandardError.ReadToEndAsync()

                Await processo.WaitForExitAsync()

                Await tarefaSaida

                Dim mensagemErro As String =
                Await tarefaErro

                If processo.ExitCode <> 0 OrElse
                   Not IO.File.Exists(caminhoAudioGerado) Then

                    If String.IsNullOrWhiteSpace(
                        mensagemErro) Then

                        mensagemErro =
                        "O Piper não gerou o arquivo de áudio."

                    End If

                    Throw New Exception(
                    mensagemErro.Trim())

                End If

            End Using

            Return caminhoAudioGerado

        Catch ex As Exception

            If caminhoAudioGerado <> "" AndAlso
               IO.File.Exists(caminhoAudioGerado) Then

                Try
                    IO.File.Delete(caminhoAudioGerado)
                Catch
                End Try

            End If

            MessageBox.Show(
            "Não foi possível gerar a promoção com o Piper." &
            Environment.NewLine &
            ex.Message,
            "Erro na voz neural",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error)

            Return ""

        End Try

    End Function

    Private Function ObterProximoAnuncioGravado() As String

        caminhosAnunciosGravados.RemoveAll(
        Function(caminho)
            Return Not IO.File.Exists(caminho)
        End Function)

        AtualizarBotaoAnunciosGravados()

        If caminhosAnunciosGravados.Count = 0 Then
            Return ""
        End If

        Dim indiceAnuncio As Integer

        If anunciosGravadosAleatorios AndAlso
       caminhosAnunciosGravados.Count > 1 Then

            Do

                indiceAnuncio =
                geradorAnunciosGravados.Next(
                    caminhosAnunciosGravados.Count)

            Loop While indiceAnuncio =
                   indiceUltimoAnuncioGravado

        Else

            indiceAnuncio =
            indiceUltimoAnuncioGravado + 1

            If indiceAnuncio >=
           caminhosAnunciosGravados.Count Then

                indiceAnuncio = 0

            End If

        End If

        indiceUltimoAnuncioGravado =
        indiceAnuncio

        Return caminhosAnunciosGravados(
        indiceAnuncio)

    End Function

    Private Sub ExecutarAnuncioAutomatico(
    proximoIndice As Integer)

        If RdbAnuncioGravado.Checked Then

            Dim caminhoAnuncio As String =
            ObterProximoAnuncioGravado()

            If String.IsNullOrWhiteSpace(caminhoAnuncio) Then

                ReproduzirMusica(
                proximoIndice)

                Exit Sub

            End If

            LiberarAudioAtual()

            TrkProgresso.Value = 0
            LblTempoAtual.Text = "00:00"
            LblTempoTotal.Text = "00:00"

            ReproduzirAudioPromocao(
            caminhoAnuncio,
            True,
            False,
            proximoIndice)

            Exit Sub

        End If

        Dim promocao As PromocaoTexto =
        ObterProximaPromocaoTexto()

        If promocao Is Nothing Then

            ReproduzirMusica(
            proximoIndice)

            Exit Sub

        End If

        NarrarPromocaoAutomatica(
            proximoIndice,
            promocao)

    End Sub

    Private Sub RestaurarTextoBotaoTeste()

        If RdbAnuncioGravado.Checked Then

            BtnTestarVoz.Text =
            "Testar anúncio gravado"

        Else

            BtnTestarVoz.Text =
            "Testar promoção selecionada"

        End If

    End Sub

#End Region

    Private Sub FrmPrincipal_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        TmrPlayer.Stop()

        LiberarAudioPromocaoPiper()

        LiberarAudioAtual()

        SalvarConfiguracao()

    End Sub

End Class
