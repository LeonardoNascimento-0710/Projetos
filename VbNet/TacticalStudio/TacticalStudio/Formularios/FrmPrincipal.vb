Imports System.Collections.Generic
Imports System.Drawing.Imaging
Imports System.Drawing.Printing
Imports System.IO
Imports System.Text
Imports System.Text.Json
Imports PdfSharp
Imports PdfSharp.Drawing
Imports PdfSharp.Pdf
Imports TacticalStudio.Core
Imports TacticalStudio.Core.Classes
Imports TacticalStudio.Core.Enums

Public Class FrmPrincipal

#Region "Variáveis"

    Private CampoCanvas As CampoTatico

    Private _caminhoArquivoAtual As String = String.Empty

    Private _nomeExercicioAtual As String = "Novo exercício"

    Private _categoriaExercicioAtual As String = "Tático"

    Private _duracaoExercicioAtual As Integer = 30

    Private _descricaoExercicioAtual As String = String.Empty

    Private _observacoesExercicioAtual As String = String.Empty

    Private _assinaturaSalva As String = String.Empty

    Private _alteracoesNaoSalvas As Boolean

    Private ReadOnly _botoesFerramentas As New Dictionary(Of FerramentaCampo, Button)()

    Private ReadOnly _temporizadorAutosave As New System.Windows.Forms.Timer()

    Private _caminhoRecuperacao As String = String.Empty

    Private _ultimaAssinaturaAutosave As String = String.Empty

    Private _processandoRecuperacao As Boolean

    Private _preferencias As New PreferenciasAplicacao()

    Private _caminhoPreferencias As String = String.Empty

    Private LblZoom As Label

    Private _frmGerenciadorObjetos As FrmGerenciadorObjetos

    Private _botaoRecortarCampo As Button

    Private ReadOnly _botoesAbasRibbon As New Dictionary(Of String, Button)(StringComparer.OrdinalIgnoreCase)

    Private _painelConteudoRibbon As FlowLayoutPanel

    Private _abaRibbonAtual As String = "Arquivo"

    Private _containerPainelPropriedades As Panel

    Private _painelConteudoPropriedades As FlowLayoutPanel

    Private _ajustandoPainelPropriedades As Boolean

    Private _montandoPainelPropriedades As Boolean

    Private _versaoMontagemPropriedades As Integer

    Private _painelConfiguracoesCampoAberto As Boolean

#End Region

#Region "Inicialização do formulário"

    Private Sub FrmPrincipal_Load(
    sender As Object,
    e As EventArgs) Handles MyBase.Load

        KeyPreview = True

        CarregarPreferencias()

        NormalizarPreferencias()

        Tema.AplicarPreferencias(
        _preferencias)

        AplicarTema()

        CriarCampoTatico()

        CriarBarraFerramentas()

        CriarBarraArquivo()

        CriarBarraZoom()

        ConfigurarSalvamentoAutomatico()

        MarcarComoSalvo()

        BeginInvoke(
        New MethodInvoker(
            AddressOf VerificarRecuperacaoSessao))

    End Sub

#End Region

#Region "Preferências e tema"

    Private Sub CarregarPreferencias()

        Dim pastaAplicacao As String =
        Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "TacticalStudio")

        Directory.CreateDirectory(
        pastaAplicacao)

        _caminhoPreferencias =
        Path.Combine(
            pastaAplicacao,
            "preferencias.json")

        _preferencias =
        New PreferenciasAplicacao()

        If Not File.Exists(
        _caminhoPreferencias) Then

            NormalizarPreferencias()

            SalvarPreferencias()

            Exit Sub

        End If

        Try

            Dim conteudoJson As String =
            File.ReadAllText(
                _caminhoPreferencias,
                Encoding.UTF8)

            Dim opcoes As New JsonSerializerOptions With {
            .PropertyNameCaseInsensitive = True
        }

            Dim preferenciasCarregadas As PreferenciasAplicacao =
            JsonSerializer.Deserialize(
                Of PreferenciasAplicacao)(
                conteudoJson,
                opcoes)

            If preferenciasCarregadas IsNot Nothing Then

                _preferencias =
                preferenciasCarregadas

            End If

        Catch ex As Exception

            System.Diagnostics.Debug.WriteLine(
            "Não foi possível carregar as preferências: " &
            ex.Message)

            _preferencias =
            New PreferenciasAplicacao()

        End Try

        NormalizarPreferencias()

    End Sub

    Private Sub NormalizarPreferencias()

        _preferencias.IntervaloAutosaveSegundos =
        Math.Max(
            30,
            Math.Min(
                600,
                _preferencias.IntervaloAutosaveSegundos))

        Dim resolucoesPermitidas() As Integer = {
        1280,
        1920,
        2560,
        3840,
        5120
    }

        If Array.IndexOf(
        resolucoesPermitidas,
        _preferencias.ResolucaoExportacao) < 0 Then

            _preferencias.ResolucaoExportacao =
            2560

        End If

        If Not [Enum].IsDefined(
    GetType(ModoTemaAplicacao),
    _preferencias.ModoTema) Then

            _preferencias.ModoTema =
                ModoTemaAplicacao.Escuro

        End If

        Dim coresPermitidas() As Integer = {
            Color.FromArgb(134, 29, 29).ToArgb(),
            Color.FromArgb(35, 105, 190).ToArgb(),
            Color.FromArgb(35, 145, 75).ToArgb(),
            Color.FromArgb(110, 60, 170).ToArgb(),
            Color.FromArgb(220, 105, 25).ToArgb()
        }

        If Array.IndexOf(
            coresPermitidas,
            _preferencias.CorPrincipalArgb) < 0 Then

            _preferencias.CorPrincipalArgb =
                Color.FromArgb(
                    134,
                    29,
                    29).ToArgb()

        End If

        Dim espacamentosPermitidos() As Integer = {2, 5, 10}

        If Array.IndexOf(
            espacamentosPermitidos,
            _preferencias.EspacamentoGradePercentual) < 0 Then

            _preferencias.EspacamentoGradePercentual =
                5

        End If
    End Sub

    Private Sub SalvarPreferencias()

        Try

            If String.IsNullOrWhiteSpace(
            _caminhoPreferencias) Then

                Dim pastaAplicacao As String =
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData),
                    "TacticalStudio")

                Directory.CreateDirectory(
                pastaAplicacao)

                _caminhoPreferencias =
                Path.Combine(
                    pastaAplicacao,
                    "preferencias.json")

            End If

            NormalizarPreferencias()

            Dim opcoes As New JsonSerializerOptions With {
            .WriteIndented = True
        }

            Dim conteudoJson As String =
            JsonSerializer.Serialize(
                _preferencias,
                opcoes)

            Dim caminhoTemporario As String =
            _caminhoPreferencias &
            ".tmp"

            File.WriteAllText(
            caminhoTemporario,
            conteudoJson,
            New UTF8Encoding(False))

            File.Move(
            caminhoTemporario,
            _caminhoPreferencias,
            True)

        Catch ex As Exception

            MessageBox.Show(
            "Não foi possível salvar as preferências." &
            Environment.NewLine &
            Environment.NewLine &
            ex.Message,
            "Preferências",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error)

        End Try

    End Sub

    Private Sub AplicarTema()

        BackColor = Tema.Fundo

        ForeColor = Tema.Texto

        Font = Tema.FontePadrao

        PnlSuperior.BackColor = Tema.CorPrimaria

        PnlEsquerdo.BackColor = Tema.Painel

        PnlDireito.BackColor = Tema.Painel

        PnlInferior.BackColor = Tema.Painel

        PnlCentral.BackColor = Tema.Fundo

    End Sub

    Private Sub Preferencias_Click(
    sender As Object,
    e As EventArgs)

        AbrirPreferencias()

    End Sub

    Private Sub AbrirPreferencias()

        Using formulario As New FrmPreferencias()

            formulario.ModoTema = _preferencias.ModoTema

            formulario.CorPrincipalArgb = _preferencias.CorPrincipalArgb

            formulario.AutosaveAtivo = _preferencias.AutosaveAtivo

            formulario.IntervaloAutosaveSegundos = _preferencias.IntervaloAutosaveSegundos

            formulario.ResolucaoExportacao = _preferencias.ResolucaoExportacao

            formulario.GradeVisivel = _preferencias.GradeVisivel

            formulario.EncaixeGradeAtivo = _preferencias.EncaixeGradeAtivo

            formulario.EspacamentoGradePercentual = _preferencias.EspacamentoGradePercentual

            If formulario.ShowDialog(Me) <>
           DialogResult.OK Then

                CampoCanvas.Focus()

                Exit Sub

            End If

            _preferencias.ModoTema = formulario.ModoTema

            _preferencias.CorPrincipalArgb = formulario.CorPrincipalArgb

            _preferencias.AutosaveAtivo = formulario.AutosaveAtivo

            _preferencias.IntervaloAutosaveSegundos = formulario.IntervaloAutosaveSegundos

            _preferencias.ResolucaoExportacao = formulario.ResolucaoExportacao

            _preferencias.GradeVisivel =
                formulario.GradeVisivel

            _preferencias.EncaixeGradeAtivo =
                formulario.EncaixeGradeAtivo

            _preferencias.EspacamentoGradePercentual =
                formulario.EspacamentoGradePercentual

            NormalizarPreferencias()

            SalvarPreferencias()

            Tema.AplicarPreferencias(_preferencias)

            AplicarPreferenciasGrade()

            ReaplicarTemaAplicacao()

            ConfigurarSalvamentoAutomatico()

            If Not _preferencias.AutosaveAtivo Then

                ExcluirArquivoRecuperacao()

            End If

            CampoCanvas.Focus()

        End Using

    End Sub

    Private Sub ReaplicarTemaAplicacao()

        SuspendLayout()

        Try

            AplicarTema()

            If CampoCanvas IsNot Nothing Then

                CampoCanvas.BackColor =
                Tema.Fundo

                CampoCanvas.Invalidate()

            End If

            CriarBarraFerramentas()

            CriarBarraArquivo()

            CriarBarraZoom()

            If _frmGerenciadorObjetos IsNot Nothing AndAlso
               Not _frmGerenciadorObjetos.IsDisposed Then

                _frmGerenciadorObjetos.AplicarTemaAtual()
                _frmGerenciadorObjetos.AtualizarConteudo()

            End If

            If CampoCanvas IsNot Nothing Then

                If CampoCanvas.ModoSelecaoRecorteAtivo Then

                    MontarPainelRecorteCampo()

                Else

                    MontarPainelPropriedades(
                        CampoCanvas.ObjetoSelecionadoAtual)

                End If

            End If

        Finally

            ResumeLayout(True)

        End Try

        Invalidate(
        True)

        Refresh()

    End Sub

#End Region

#Region "Salvamento automático e recuperação"

    Private Sub ConfigurarSalvamentoAutomatico()

        Dim pastaAplicacao As String =
        Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "TacticalStudio",
            "Recuperacao")

        Directory.CreateDirectory(
        pastaAplicacao)

        _caminhoRecuperacao =
        Path.Combine(
            pastaAplicacao,
            "sessao_autosave.tactical")

        _temporizadorAutosave.Stop()

        RemoveHandler _temporizadorAutosave.Tick,
        AddressOf TemporizadorAutosave_Tick

        Dim intervaloSegundos As Integer =
        Math.Max(
            30,
            Math.Min(
                600,
                _preferencias.IntervaloAutosaveSegundos))

        _temporizadorAutosave.Interval =
        intervaloSegundos *
        1000

        AddHandler _temporizadorAutosave.Tick,
        AddressOf TemporizadorAutosave_Tick

        If _preferencias.AutosaveAtivo Then

            _temporizadorAutosave.Start()

        End If

    End Sub

    Private Sub TemporizadorAutosave_Tick(sender As Object, e As EventArgs)

        SalvarRecuperacaoAutomatica()

    End Sub

    Private Sub SalvarRecuperacaoAutomatica()

        If Not _preferencias.AutosaveAtivo Then
            Exit Sub
        End If

        If CampoCanvas Is Nothing Then
            Exit Sub
        End If

        If Not _alteracoesNaoSalvas Then
            Exit Sub
        End If

        If String.IsNullOrWhiteSpace(
        _caminhoRecuperacao) Then

            Exit Sub

        End If

        Try

            Dim assinaturaAtual As String =
            ObterAssinaturaCompleta()

            If File.Exists(
            _caminhoRecuperacao) AndAlso
           String.Equals(
               assinaturaAtual,
               _ultimaAssinaturaAutosave,
               StringComparison.Ordinal) Then

                Exit Sub

            End If

            Dim conteudoJson As String =
            CampoCanvas.ExportarExercicioJson(
                _nomeExercicioAtual,
                _categoriaExercicioAtual,
                _duracaoExercicioAtual,
                _descricaoExercicioAtual,
                _observacoesExercicioAtual)

            Dim caminhoTemporario As String =
            _caminhoRecuperacao &
            ".tmp"

            File.WriteAllText(
            caminhoTemporario,
            conteudoJson,
            New UTF8Encoding(False))

            File.Move(
            caminhoTemporario,
            _caminhoRecuperacao,
            True)

            _ultimaAssinaturaAutosave =
            assinaturaAtual

        Catch ex As Exception

            System.Diagnostics.Debug.WriteLine(
            "Falha no salvamento automático: " &
            ex.Message)

        End Try

    End Sub

    Private Sub ExcluirArquivoRecuperacao()

        If String.IsNullOrWhiteSpace(
        _caminhoRecuperacao) Then

            Exit Sub

        End If

        Try

            If File.Exists(
            _caminhoRecuperacao) Then

                File.Delete(
                _caminhoRecuperacao)

            End If

            Dim caminhoTemporario As String =
            _caminhoRecuperacao &
            ".tmp"

            If File.Exists(
            caminhoTemporario) Then

                File.Delete(
                caminhoTemporario)

            End If

            _ultimaAssinaturaAutosave =
            String.Empty

        Catch ex As Exception

            System.Diagnostics.Debug.WriteLine(
            "Não foi possível excluir a recuperação: " &
            ex.Message)

        End Try

    End Sub

    Private Sub VerificarRecuperacaoSessao()

        If CampoCanvas Is Nothing Then
            Exit Sub
        End If

        If String.IsNullOrWhiteSpace(
        _caminhoRecuperacao) Then

            Exit Sub

        End If

        If Not File.Exists(
        _caminhoRecuperacao) Then

            Exit Sub

        End If

        Dim dataRecuperacao As String =
        File.GetLastWriteTime(
            _caminhoRecuperacao).
            ToString(
                "dd/MM/yyyy 'às' HH:mm")

        Dim resposta As DialogResult =
        MessageBox.Show(
            "Foi encontrada uma sessão não finalizada do TacticalStudio." &
            Environment.NewLine &
            Environment.NewLine &
            "Última recuperação: " &
            dataRecuperacao &
            Environment.NewLine &
            Environment.NewLine &
            "Deseja recuperar essa sessão?",
            "Recuperação de sessão",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question)

        If resposta <> DialogResult.Yes Then

            ExcluirArquivoRecuperacao()

            Exit Sub

        End If

        _processandoRecuperacao =
        True

        Try

            Dim conteudoJson As String =
            File.ReadAllText(
                _caminhoRecuperacao,
                Encoding.UTF8)

            Dim arquivo As ArquivoExercicio =
            CampoCanvas.ImportarExercicioJson(
                conteudoJson)

            _caminhoArquivoAtual =
            String.Empty

            If String.IsNullOrWhiteSpace(
            arquivo.Nome) Then

                _nomeExercicioAtual =
                "Exercício recuperado"

            Else

                _nomeExercicioAtual =
                arquivo.Nome

            End If

            _categoriaExercicioAtual =
            If(
                String.IsNullOrWhiteSpace(
                    arquivo.Categoria),
                "Tático",
                arquivo.Categoria)

            _duracaoExercicioAtual =
            Math.Max(
                1,
                arquivo.DuracaoMinutos)

            _descricaoExercicioAtual =
            If(
                arquivo.Descricao,
                String.Empty)

            _observacoesExercicioAtual =
            If(
                arquivo.Observacoes,
                String.Empty)

            'A recuperação ainda não é considerada
            'um salvamento manual.
            _assinaturaSalva =
            String.Empty

            _alteracoesNaoSalvas =
            True

            _ultimaAssinaturaAutosave =
            ObterAssinaturaCompleta()

            AtualizarTituloJanela()

            MessageBox.Show(
            "A sessão foi recuperada com sucesso." &
            Environment.NewLine &
            Environment.NewLine &
            "Use Ctrl+S para salvar o exercício.",
            "Sessão recuperada",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information)

            CampoCanvas.Focus()

        Catch ex As Exception

            MessageBox.Show(
            "Não foi possível recuperar a sessão." &
            Environment.NewLine &
            Environment.NewLine &
            ex.Message,
            "Erro na recuperação",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error)

            ExcluirArquivoRecuperacao()

        Finally

            _processandoRecuperacao =
            False

        End Try

    End Sub

#End Region

#Region "Campo tático e eventos"

    Private Sub CriarCampoTatico()

        CampoCanvas = New CampoTatico With {
        .Dock = DockStyle.Fill
    }

        AddHandler CampoCanvas.FerramentaAtualAlterada,
        AddressOf CampoCanvas_FerramentaAtualAlterada

        AddHandler CampoCanvas.ObjetoSelecionadoAlterado,
    AddressOf CampoCanvas_ObjetoSelecionadoAlterado

        AddHandler CampoCanvas.HistoricoAlterado,
    AddressOf CampoCanvas_HistoricoAlterado

        AddHandler CampoCanvas.VisualizacaoAlterada,
    AddressOf CampoCanvas_VisualizacaoAlterada

        AddHandler CampoCanvas.RecorteCampoAlterado,
    AddressOf CampoCanvas_RecorteCampoAlterado

        PnlCentral.Controls.Clear()
        PnlCentral.Controls.Add(CampoCanvas)

        AplicarPreferenciasGrade()

    End Sub

    Private Sub CampoCanvas_VisualizacaoAlterada(
    zoomPercentual As Integer)

        AtualizarIndicadorZoom(
        zoomPercentual)

    End Sub

    Private Sub CampoCanvas_ObjetoSelecionadoAlterado(objeto As ObjetoCampo)

        If CampoCanvas IsNot Nothing AndAlso
           CampoCanvas.ModoSelecaoRecorteAtivo Then

            MontarPainelRecorteCampo()

            Exit Sub

        End If

        If CampoCanvas IsNot Nothing AndAlso
           CampoCanvas.QuantidadeObjetosSelecionados > 1 Then

            MontarPainelSelecaoMultipla(
                CampoCanvas.QuantidadeObjetosSelecionados)

            Exit Sub

        End If

        MontarPainelPropriedades(objeto)

    End Sub

    Private Sub CampoCanvas_RecorteCampoAlterado()

        AtualizarEstadoBotaoRecorte()

        If CampoCanvas Is Nothing Then
            Exit Sub
        End If

        If CampoCanvas.ModoSelecaoRecorteAtivo OrElse
           CampoCanvas.RecorteAtivo Then

            MontarPainelRecorteCampo()

        ElseIf CampoCanvas.QuantidadeObjetosSelecionados > 1 Then

            MontarPainelSelecaoMultipla(
                CampoCanvas.QuantidadeObjetosSelecionados)

        Else

            MontarPainelPropriedades(
                CampoCanvas.ObjetoSelecionadoAtual)

        End If

    End Sub

#End Region

#Region "Zoom, grade e encaixe"

    Private Sub CriarBarraZoom()

        PnlInferior.Controls.Clear()

        PnlInferior.Height = 50
        PnlInferior.Padding = New Padding(0)

        Dim painelGrade As New FlowLayoutPanel With {
    .Dock = DockStyle.Left,
    .Width = 390,
    .FlowDirection =
        FlowDirection.LeftToRight,
    .WrapContents = False,
    .AutoScroll = False,
    .Padding = New Padding(
        5,
        6,
        5,
        4),
    .Margin = New Padding(0),
    .BackColor = Tema.Painel
}

        Dim botaoGrade As Button =
    CriarBotaoZoom(
        "Grade",
        80)

        AtualizarBotaoAlternancia(
    botaoGrade,
    CampoCanvas.GradeVisivel)

        AddHandler botaoGrade.Click,
    Sub(sender, e)

        AlternarGrade()

        AtualizarBotaoAlternancia(
            botaoGrade,
            CampoCanvas.GradeVisivel)

    End Sub

        painelGrade.Controls.Add(
    botaoGrade)

        Dim botaoEncaixe As Button =
    CriarBotaoZoom(
        "Encaixe",
        90)

        AtualizarBotaoAlternancia(
    botaoEncaixe,
    CampoCanvas.EncaixeGradeAtivo)

        AddHandler botaoEncaixe.Click,
    Sub(sender, e)

        AlternarEncaixeGrade()

        AtualizarBotaoAlternancia(
            botaoEncaixe,
            CampoCanvas.EncaixeGradeAtivo)

    End Sub

        painelGrade.Controls.Add(
    botaoEncaixe)

        Dim labelPasso As New Label With {
    .Text = "Passo:",
    .Width = 50,
    .Height = 32,
    .Margin = New Padding(
        6,
        0,
        2,
        0),
    .ForeColor = Tema.Texto,
    .BackColor = Tema.Painel,
    .TextAlign =
        ContentAlignment.MiddleCenter
}

        painelGrade.Controls.Add(
    labelPasso)

        Dim comboPasso As New ComboBox With {
    .Width = 70,
    .Height = 32,
    .Margin = New Padding(
        2,
        1,
        2,
        0),
    .DropDownStyle =
        ComboBoxStyle.DropDownList,
    .BackColor = Tema.CampoEntrada,
    .ForeColor = Tema.TextoCampo,
    .FlatStyle = FlatStyle.Flat
}

        comboPasso.Items.AddRange(
    {
        2,
        5,
        10
    })

        comboPasso.SelectedItem =
    CampoCanvas.EspacamentoGradePercentual

        AddHandler comboPasso.SelectedIndexChanged,
    Sub(sender, e)

        If comboPasso.SelectedItem Is Nothing Then
            Exit Sub
        End If

        Dim passo As Integer =
            CInt(
                comboPasso.SelectedItem)

        CampoCanvas.EspacamentoGradePercentual =
            passo

        _preferencias.EspacamentoGradePercentual =
            passo

        SalvarPreferencias()

        CampoCanvas.Focus()

    End Sub

        painelGrade.Controls.Add(
    comboPasso)

        PnlInferior.Controls.Add(
    painelGrade)

        Dim painelZoom As New FlowLayoutPanel With {
    .Dock = DockStyle.Right,
    .Width = 330,
    .FlowDirection = FlowDirection.LeftToRight,
    .WrapContents = False,
    .AutoScroll = False,
    .Padding = New Padding(5, 6, 5, 4),
    .Margin = New Padding(0),
    .BackColor = Tema.Painel
}

        Dim botaoDiminuir As Button =
        CriarBotaoZoom(
            "−",
            42)

        AddHandler botaoDiminuir.Click,
        Sub(sender, e)

            CampoCanvas.DiminuirZoom()
            CampoCanvas.Focus()

        End Sub

        painelZoom.Controls.Add(
        botaoDiminuir)

        LblZoom =
        New Label With {
            .Text = "100%",
            .Width = 70,
            .Height = 32,
            .Margin = New Padding(4, 0, 4, 0),
            .ForeColor = Tema.Texto,
            .BackColor = Tema.Painel,
            .TextAlign =
                ContentAlignment.MiddleCenter
        }

        painelZoom.Controls.Add(
        LblZoom)

        Dim botaoAumentar As Button =
        CriarBotaoZoom(
            "+",
            42)

        AddHandler botaoAumentar.Click,
        Sub(sender, e)

            CampoCanvas.AumentarZoom()
            CampoCanvas.Focus()

        End Sub

        painelZoom.Controls.Add(
        botaoAumentar)

        Dim botaoAjustar As Button =
        CriarBotaoZoom(
            "Ajustar",
            100)

        AddHandler botaoAjustar.Click,
        Sub(sender, e)

            CampoCanvas.RestaurarVisualizacao()
            CampoCanvas.Focus()

        End Sub

        painelZoom.Controls.Add(
        botaoAjustar)

        PnlInferior.Controls.Add(
        painelZoom)

        AtualizarIndicadorZoom(
        CampoCanvas.ZoomPercentual)

    End Sub

    Private Function CriarBotaoZoom(texto As String, largura As Integer) As Button

        Dim botao As New Button With {
        .Text = texto,
        .Width = largura,
        .Height = 32,
        .Margin = New Padding(3, 0, 3, 0),
        .FlatStyle = FlatStyle.Flat,
        .BackColor = Tema.Painel,
        .ForeColor = Tema.Texto,
        .Cursor = Cursors.Hand,
        .UseVisualStyleBackColor = False
    }

        botao.FlatAppearance.BorderColor = Tema.Borda

        botao.FlatAppearance.MouseOverBackColor = Tema.PainelHover

        botao.FlatAppearance.MouseDownBackColor = Tema.PainelHover

        Return botao

    End Function

    Private Sub AtualizarIndicadorZoom(zoomPercentual As Integer)

        If LblZoom Is Nothing Then
            Exit Sub
        End If

        LblZoom.Text =
        zoomPercentual.ToString() &
        "%"

    End Sub

    Private Sub AtualizarBotaoAlternancia(botao As Button, ativo As Boolean)

        If ativo Then

            botao.BackColor = Tema.CorPrimaria

            botao.ForeColor = Tema.TextoSobreCorPrimaria

            botao.FlatAppearance.BorderColor = Tema.TextoSobreCorPrimaria

        Else

            botao.BackColor = Tema.Painel

            botao.ForeColor = Tema.Texto

            botao.FlatAppearance.BorderColor = Tema.Borda

        End If

    End Sub

    Private Sub AlternarGrade()

        CampoCanvas.GradeVisivel =
        Not CampoCanvas.GradeVisivel

        _preferencias.GradeVisivel =
        CampoCanvas.GradeVisivel

        SalvarPreferencias()

        CampoCanvas.Focus()

    End Sub

    Private Sub AlternarEncaixeGrade()

        CampoCanvas.EncaixeGradeAtivo =
        Not CampoCanvas.EncaixeGradeAtivo

        _preferencias.EncaixeGradeAtivo =
        CampoCanvas.EncaixeGradeAtivo

        SalvarPreferencias()

        CampoCanvas.Focus()

    End Sub

    Private Sub AplicarPreferenciasGrade()

        If CampoCanvas Is Nothing Then
            Exit Sub
        End If

        CampoCanvas.GradeVisivel =
        _preferencias.GradeVisivel

        CampoCanvas.EncaixeGradeAtivo =
        _preferencias.EncaixeGradeAtivo

        CampoCanvas.EspacamentoGradePercentual =
        _preferencias.EspacamentoGradePercentual

    End Sub

#End Region

#Region "Barra de ferramentas"

    Private Sub CriarBarraFerramentas()

        PnlEsquerdo.SuspendLayout()

        Try

            PnlEsquerdo.Controls.Clear()

            _botoesFerramentas.Clear()

            PnlEsquerdo.Padding =
            New Padding(0)

            PnlEsquerdo.BackColor =
            Tema.Fundo

            If PnlEsquerdo.Width < 280 Then

                PnlEsquerdo.Width =
                280

            End If

            Dim painelFerramentas As New FlowLayoutPanel With {
            .Name = "PnlFerramentasModernas",
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False,
            .AutoScroll = True,
            .Margin = New Padding(0),
            .Padding = New Padding(10),
            .BackColor = Tema.Fundo
        }

            Dim larguraConteudo As Integer =
            Math.Max(
                230,
                PnlEsquerdo.ClientSize.Width - 32)

            '==================================================
            ' CABEÇALHO
            '==================================================

            Dim titulo As New Label With {
            .Text = "FERRAMENTAS",
            .Width = larguraConteudo,
            .Height = 28,
            .Margin = New Padding(0),
            .ForeColor = Tema.Texto,
            .BackColor = Tema.Fundo,
            .Font = New Font(
                "Segoe UI",
                11.0F,
                FontStyle.Bold),
            .TextAlign = ContentAlignment.MiddleLeft
        }

            painelFerramentas.Controls.Add(
            titulo)

            Dim subtitulo As New Label With {
            .Text = "Adicione e organize objetos no campo",
            .Width = larguraConteudo,
            .Height = 26,
            .Margin = New Padding(
                0,
                0,
                0,
                8),
            .ForeColor = Tema.TextoSecundario,
            .BackColor = Tema.Fundo,
            .Font = New Font(
                "Segoe UI",
                8.0F,
                FontStyle.Regular),
            .TextAlign = ContentAlignment.MiddleLeft
        }

            painelFerramentas.Controls.Add(
            subtitulo)

            '==================================================
            ' SELEÇÃO
            '==================================================

            Dim botaoSelecionar As Button =
            CriarBotaoFerramenta(
                FerramentaCampo.Selecionar,
                "Selecionar")

            botaoSelecionar.Width =
            larguraConteudo

            botaoSelecionar.Height =
            42

            botaoSelecionar.Margin =
            New Padding(
                0,
                0,
                0,
                10)

            botaoSelecionar.TextAlign =
            ContentAlignment.MiddleCenter

            painelFerramentas.Controls.Add(
            botaoSelecionar)

            '==================================================
            ' OBJETOS
            '==================================================

            Dim grupoObjetos As TableLayoutPanel =
            CriarGrupoFerramentas(
                "OBJETOS",
                larguraConteudo,
                4)

            grupoObjetos.Controls.Add(
            CriarBotaoFerramenta(
                FerramentaCampo.Jogador,
                "Jogador"),
            0,
            1)

            grupoObjetos.Controls.Add(
            CriarBotaoFerramenta(
                FerramentaCampo.Bola,
                "Bola"),
            1,
            1)

            grupoObjetos.Controls.Add(
            CriarBotaoFerramenta(
                FerramentaCampo.Cone,
                "Cone"),
            0,
            2)

            grupoObjetos.Controls.Add(
            CriarBotaoFerramenta(
                FerramentaCampo.Gol,
                "Gol"),
            1,
            2)

            grupoObjetos.Controls.Add(
            CriarBotaoFerramenta(
                FerramentaCampo.Manequim,
                "Manequim"),
            0,
            3)

            Dim botaoLinhaObjetos As Button =
                CriarBotaoFerramenta(
                    FerramentaCampo.LinhaObjetos,
                    "Linha de objetos / barreira")

            grupoObjetos.Controls.Add(
                botaoLinhaObjetos,
                0,
                4)

            grupoObjetos.SetColumnSpan(
                botaoLinhaObjetos,
                2)

            painelFerramentas.Controls.Add(
            grupoObjetos)

            '==================================================
            ' DESENHO
            '==================================================

            Dim grupoDesenho As TableLayoutPanel =
            CriarGrupoFerramentas(
                "LINHAS E ÁREAS",
                larguraConteudo,
                2)

            grupoDesenho.Controls.Add(
            CriarBotaoFerramenta(
                FerramentaCampo.LinhaContinua,
                "Linha"),
            0,
            1)

            grupoDesenho.Controls.Add(
            CriarBotaoFerramenta(
                FerramentaCampo.LinhaTracejada,
                "Tracejada"),
            1,
            1)

            grupoDesenho.Controls.Add(
            CriarBotaoFerramenta(
                FerramentaCampo.Seta,
                "Seta"),
            0,
            2)

            grupoDesenho.Controls.Add(
            CriarBotaoFerramenta(
                FerramentaCampo.Area,
                "Área"),
            1,
            2)

            painelFerramentas.Controls.Add(
            grupoDesenho)

            '==================================================
            ' ANOTAÇÕES
            '==================================================

            Dim grupoAnotacoes As TableLayoutPanel =
            CriarGrupoFerramentas(
                "ANOTAÇÕES",
                larguraConteudo,
                1)

            grupoAnotacoes.Controls.Add(
            CriarBotaoFerramenta(
                FerramentaCampo.Marcador,
                "Marcador"),
            0,
            1)

            grupoAnotacoes.Controls.Add(
            CriarBotaoFerramenta(
                FerramentaCampo.Texto,
                "Texto"),
            1,
            1)

            painelFerramentas.Controls.Add(
            grupoAnotacoes)

            '==================================================
            ' CAMPO
            '==================================================

            Dim grupoCampo As TableLayoutPanel =
            CriarGrupoFerramentas(
                "CAMPO",
                larguraConteudo,
                1)

            _botaoRecortarCampo =
            New Button With {
                .Text = "Recortar campo",
                .Dock = DockStyle.Fill,
                .Margin = New Padding(4),
                .FlatStyle = FlatStyle.Flat,
                .TextAlign =
                    ContentAlignment.MiddleCenter,
                .Cursor = Cursors.Cross,
                .BackColor = Tema.Painel,
                .ForeColor = Tema.Texto,
                .Font = New Font(
                    "Segoe UI",
                    9.0F,
                    FontStyle.Regular),
                .UseVisualStyleBackColor = False
            }

            _botaoRecortarCampo.FlatAppearance.BorderSize =
            1

            _botaoRecortarCampo.FlatAppearance.BorderColor =
            Tema.CorPrimaria

            _botaoRecortarCampo.FlatAppearance.MouseOverBackColor =
            Tema.PainelHover

            _botaoRecortarCampo.FlatAppearance.MouseDownBackColor =
            Tema.CorPrimaria

            AddHandler _botaoRecortarCampo.Click,
            AddressOf RecortarCampo_Click

            grupoCampo.Controls.Add(
            _botaoRecortarCampo,
            0,
            1)

            grupoCampo.SetColumnSpan(
            _botaoRecortarCampo,
            2)

            painelFerramentas.Controls.Add(
            grupoCampo)

            PnlEsquerdo.Controls.Add(
            painelFerramentas)

            AtualizarBotoesFerramentas(
            FerramentaCampo.Selecionar)

            AtualizarEstadoBotaoRecorte()

        Finally

            PnlEsquerdo.ResumeLayout(
            True)

        End Try

    End Sub

    Private Function CriarGrupoFerramentas(titulo As String, largura As Integer, quantidadeLinhas As Integer) As TableLayoutPanel

        quantidadeLinhas =
        Math.Max(
            1,
            quantidadeLinhas)

        Dim alturaGrupo As Integer =
        44 +
        quantidadeLinhas *
        48

        Dim grupo As New TableLayoutPanel With {
        .Width = largura,
        .Height = alturaGrupo,
        .ColumnCount = 2,
        .RowCount = quantidadeLinhas + 1,
        .Margin = New Padding(
            0,
            0,
            0,
            10),
        .Padding = New Padding(
            7,
            6,
            7,
            7),
        .BackColor = Tema.Painel,
        .CellBorderStyle =
            TableLayoutPanelCellBorderStyle.None,
        .GrowStyle =
            TableLayoutPanelGrowStyle.FixedSize
    }

        grupo.ColumnStyles.Add(
        New ColumnStyle(
            SizeType.Percent,
            50.0F))

        grupo.ColumnStyles.Add(
        New ColumnStyle(
            SizeType.Percent,
            50.0F))

        grupo.RowStyles.Add(
        New RowStyle(
            SizeType.Absolute,
            28.0F))

        For indice As Integer =
        1 To quantidadeLinhas

            grupo.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                48.0F))

        Next

        Dim labelTitulo As New Label With {
        .Text = titulo,
        .Dock = DockStyle.Fill,
        .Margin = New Padding(
            4,
            0,
            4,
            2),
        .ForeColor = Tema.TextoSecundario,
        .BackColor = Tema.Painel,
        .Font = New Font(
            "Segoe UI",
            8.0F,
            FontStyle.Bold),
        .TextAlign =
            ContentAlignment.MiddleLeft
    }

        grupo.Controls.Add(
        labelTitulo,
        0,
        0)

        grupo.SetColumnSpan(
        labelTitulo,
        2)

        Return grupo

    End Function

    Private Function CriarBotaoFerramenta(
    ferramenta As FerramentaCampo,
    texto As String
) As Button

        Dim botao As New Button With {
        .Text = texto,
        .Tag = ferramenta,
        .Dock = DockStyle.Fill,
        .Margin = New Padding(4),
        .FlatStyle = FlatStyle.Flat,
        .TextAlign =
            ContentAlignment.MiddleCenter,
        .Cursor = Cursors.Hand,
        .BackColor = Tema.Fundo,
        .ForeColor = Tema.Texto,
        .Font = New Font(
            "Segoe UI",
            9.0F,
            FontStyle.Regular),
        .UseVisualStyleBackColor = False
    }

        botao.FlatAppearance.BorderSize =
        1

        botao.FlatAppearance.BorderColor =
        Tema.Borda

        botao.FlatAppearance.MouseOverBackColor =
        Tema.PainelHover

        botao.FlatAppearance.MouseDownBackColor =
        Tema.CorPrimaria

        AddHandler botao.Click,
        AddressOf BotaoFerramenta_Click

        _botoesFerramentas(
        ferramenta) =
        botao

        Return botao

    End Function

    Private Sub RecortarCampo_Click(
    sender As Object,
    e As EventArgs)

        If CampoCanvas Is Nothing Then
            Exit Sub
        End If

        If CampoCanvas.ModoSelecaoRecorteAtivo Then

            CampoCanvas.CancelarSelecaoRecorteCampo()

        Else

            CampoCanvas.IniciarSelecaoRecorteCampo()

        End If

        AtualizarEstadoBotaoRecorte()

        CampoCanvas.Focus()

    End Sub

    Private Sub BotaoFerramenta_Click(
    sender As Object,
    e As EventArgs)

        Dim botao As Button =
            TryCast(sender, Button)

        If botao Is Nothing Then
            Exit Sub
        End If

        If botao.Tag Is Nothing Then
            Exit Sub
        End If

        Dim ferramenta As FerramentaCampo =
    CType(botao.Tag, FerramentaCampo)

        If ferramenta =
           FerramentaCampo.LinhaObjetos Then

            Using configuracao As New FrmLinhaObjetos()

                If configuracao.ShowDialog(Me) <>
                   DialogResult.OK Then

                    Exit Sub

                End If

                CampoCanvas.ConfigurarLinhaObjetos(
                    configuracao.TipoSelecionado,
                    configuracao.QuantidadeSelecionada,
                    configuracao.OrientacaoSelecionada,
                    configuracao.EscalaSelecionada,
                    configuracao.AgruparAutomaticamente)

            End Using

        End If

        If CampoCanvas.ModoSelecaoRecorteAtivo Then

            CampoCanvas.CancelarSelecaoRecorteCampo()

        End If

        AtualizarEstadoBotaoRecorte()

        CampoCanvas.FerramentaAtual =
            ferramenta

        AtualizarBotoesFerramentas(
            ferramenta)

        CampoCanvas.Focus()

    End Sub

    Private Sub CampoCanvas_FerramentaAtualAlterada(
    ferramenta As FerramentaCampo)

        AtualizarBotoesFerramentas(
        ferramenta)

    End Sub

    Private Sub AtualizarBotoesFerramentas(
    ferramentaAtual As FerramentaCampo)

        For Each item As KeyValuePair(
            Of FerramentaCampo,
            Button) In _botoesFerramentas

            Dim botao As Button =
                item.Value

            If item.Key = ferramentaAtual Then

                botao.BackColor =
                    Tema.CorPrimaria

                botao.ForeColor =
                    Tema.TextoSobreCorPrimaria

                botao.FlatAppearance.BorderColor =
                    Tema.TextoSobreCorPrimaria

            Else

                botao.BackColor =
                    Tema.Painel

                botao.ForeColor =
                    Tema.Texto

                botao.FlatAppearance.BorderColor =
                    Tema.Borda

            End If

        Next

    End Sub

    Private Sub AtualizarEstadoBotaoRecorte()

        If _botaoRecortarCampo Is Nothing OrElse
           CampoCanvas Is Nothing Then

            Exit Sub

        End If

        Dim ativo As Boolean =
            CampoCanvas.ModoSelecaoRecorteAtivo

        If ativo Then

            _botaoRecortarCampo.Text =
                "Cancelar recorte"

            _botaoRecortarCampo.BackColor =
                Tema.CorPrimaria

            _botaoRecortarCampo.ForeColor =
                Tema.TextoSobreCorPrimaria

            _botaoRecortarCampo.FlatAppearance.BorderColor =
                Tema.TextoSobreCorPrimaria

        Else

            _botaoRecortarCampo.Text =
                "Recortar campo"

            _botaoRecortarCampo.BackColor =
                Tema.Painel

            _botaoRecortarCampo.ForeColor =
                Tema.Texto

            _botaoRecortarCampo.FlatAppearance.BorderColor =
                Tema.CorPrimaria

        End If

    End Sub

#End Region

#Region "Painel de propriedades"

    Private Sub LimparPainelDireito()

        _painelConfiguracoesCampoAberto = False

        PnlDireito.SuspendLayout()

        Try

            _containerPainelPropriedades = Nothing

            _painelConteudoPropriedades = Nothing

            While PnlDireito.Controls.Count > 0

                Dim controleAnterior As Control =
                    PnlDireito.Controls(0)

                PnlDireito.Controls.RemoveAt(
                    0)

                controleAnterior.Dispose()

            End While

        Finally

            PnlDireito.ResumeLayout(
                False)

        End Try

    End Sub

    Private Sub ContainerPainelPropriedades_Resize(
        sender As Object,
        e As EventArgs)

        If _montandoPainelPropriedades Then
            Exit Sub
        End If

        CentralizarPainelPropriedades()

    End Sub

    Private Sub MontarPainelRecorteCampo()

        If CampoCanvas Is Nothing Then
            Exit Sub
        End If

        LimparPainelDireito()

        _painelConfiguracoesCampoAberto = True

        Dim painel As New FlowLayoutPanel With {
        .Dock = DockStyle.Fill,
        .FlowDirection = FlowDirection.TopDown,
        .WrapContents = False,
        .AutoScroll = True,
        .BackColor = Tema.Painel,
        .Padding = New Padding(10)
    }

        PnlDireito.Controls.Add(
        painel)

        Dim largura As Integer =
        Math.Max(
            180,
            PnlDireito.ClientSize.Width - 32)

        Dim titulo As New Label With {
        .Text = "CONFIGURAÇÕES DO CAMPO",
        .ForeColor = Tema.CorPrimaria,
        .Font = New Font(
            "Segoe UI",
            11.0F,
            FontStyle.Bold),
        .Width = largura,
        .Height = 36,
        .TextAlign = ContentAlignment.MiddleLeft
    }

        painel.Controls.Add(
        titulo)

        '==================================================
        ' CARTÃO: ESTILO DO CAMPO
        '==================================================

        Dim cartaoEstiloCampo As FlowLayoutPanel =
        CriarCartaoPropriedades(
            "Estilo do campo",
            Color.FromArgb(
                75,
                36,
                36))

        Dim cmbEstiloCampo As New ComboBox With {
        .DropDownStyle =
            ComboBoxStyle.DropDownList,
        .FlatStyle =
            FlatStyle.Flat
    }

        cmbEstiloCampo.Items.AddRange(
        New Object() {
            "Clássico",
            "Estádio",
            "Treino",
            "Noturno"
        })

        cmbEstiloCampo.SelectedIndex =
        Math.Max(
            0,
            Math.Min(
                3,
                CInt(
                    CampoCanvas.EstiloCampoAtual)))

        AdicionarCampoCartao(
        cartaoEstiloCampo,
        "Visual",
        cmbEstiloCampo)

        Dim nudIntensidadeTextura As New NumericUpDown With {
        .Minimum = 0D,
        .Maximum = 100D,
        .Increment = 5D,
        .Value =
            Math.Max(
                0,
                Math.Min(
                    100,
                    CampoCanvas.IntensidadeTexturaCampo)),
        .TextAlign =
            HorizontalAlignment.Center
    }

        AdicionarCampoCartao(
        cartaoEstiloCampo,
        "Intensidade da textura (%)",
        nudIntensidadeTextura)

        Dim chkFaixasGrama As New CheckBox With {
        .Text = "Exibir faixas alternadas",
        .Checked =
            CampoCanvas.FaixasGramaAtivas,
        .AutoSize = False,
        .TextAlign =
            ContentAlignment.MiddleLeft,
        .BackColor =
            Tema.CampoEntrada,
        .ForeColor =
            Tema.TextoCampo
    }

        AdicionarCampoCartao(
        cartaoEstiloCampo,
        "Faixas da grama",
        chkFaixasGrama)

        Dim chkSombrasCampo As New CheckBox With {
        .Text = "Ativar iluminação e sombras",
        .Checked =
            CampoCanvas.SombrasCampoAtivas,
        .AutoSize = False,
        .TextAlign =
            ContentAlignment.MiddleLeft,
        .BackColor =
            Tema.CampoEntrada,
        .ForeColor =
            Tema.TextoCampo
    }

        AdicionarCampoCartao(
        cartaoEstiloCampo,
        "Sombras",
        chkSombrasCampo)

        Dim aplicandoVisualCampo As Boolean = False

        Dim aplicarVisualCampo As Action =
        Sub()

            If aplicandoVisualCampo Then
                Exit Sub
            End If

            aplicandoVisualCampo = True

            Try

                CampoCanvas.AplicarConfiguracaoVisualCampo(
                    CType(
                        cmbEstiloCampo.SelectedIndex,
                        EstiloVisualCampo),
                    CInt(
                        nudIntensidadeTextura.Value),
                    chkFaixasGrama.Checked,
                    chkSombrasCampo.Checked)

            Finally

                aplicandoVisualCampo = False

            End Try

        End Sub

        AddHandler cmbEstiloCampo.SelectedIndexChanged,
        Sub(sender, e)

            aplicarVisualCampo()

        End Sub

        AddHandler nudIntensidadeTextura.ValueChanged,
        Sub(sender, e)

            aplicarVisualCampo()

        End Sub

        AddHandler chkFaixasGrama.CheckedChanged,
        Sub(sender, e)

            aplicarVisualCampo()

        End Sub

        AddHandler chkSombrasCampo.CheckedChanged,
        Sub(sender, e)

            aplicarVisualCampo()

        End Sub

        Dim btnAplicarEstiloCampo As New Button With {
        .Text = "Aplicar visual do campo",
        .Width =
            Math.Max(
                240,
                cartaoEstiloCampo.Width - 20),
        .Height = 38,
        .Margin =
            New Padding(
                8,
                8,
                8,
                0),
        .FlatStyle =
            FlatStyle.Flat,
        .BackColor =
            Tema.CorPrimaria,
        .ForeColor =
            Tema.TextoSobreCorPrimaria,
        .Cursor =
            Cursors.Hand,
        .UseVisualStyleBackColor =
            False
    }

        btnAplicarEstiloCampo.FlatAppearance.BorderColor =
        Tema.Borda

        btnAplicarEstiloCampo.FlatAppearance.MouseOverBackColor =
        Tema.CorPrimariaHover

        AddHandler btnAplicarEstiloCampo.Click,
        Sub(sender, e)

            aplicarVisualCampo()

        End Sub

        cartaoEstiloCampo.Controls.Add(
        btnAplicarEstiloCampo)

        painel.Controls.Add(
        cartaoEstiloCampo)

        '==================================================
        ' CARTÃO: MÉTRICAS REAIS
        '==================================================

        Dim cartaoMetricasReais As FlowLayoutPanel =
        CriarCartaoPropriedades(
            "Métricas reais",
            Color.FromArgb(
                75,
                36,
                36))

        Dim chkUsarMetricasReais As New CheckBox With {
        .Text = "Utilizar medidas reais",
        .Checked =
            CampoCanvas.UsarMetricasReais,
        .AutoSize = False,
        .TextAlign =
            ContentAlignment.MiddleLeft,
        .BackColor =
            Tema.CampoEntrada,
        .ForeColor =
            Tema.TextoCampo
    }

        AdicionarCampoCartao(
        cartaoMetricasReais,
        "Medição do campo",
        chkUsarMetricasReais)

        Dim nudComprimentoCampo As New NumericUpDown With {
        .Minimum = 1D,
        .Maximum = 500D,
        .Increment = 0.5D,
        .DecimalPlaces = 2,
        .Value =
            CDec(
                Math.Max(
                    1.0R,
                    Math.Min(
                        500.0R,
                        CampoCanvas.ComprimentoCampoMetros))),
        .TextAlign =
            HorizontalAlignment.Center
    }

        AdicionarCampoCartao(
        cartaoMetricasReais,
        "Comprimento do campo (m)",
        nudComprimentoCampo)

        Dim nudLarguraCampo As New NumericUpDown With {
        .Minimum = 1D,
        .Maximum = 500D,
        .Increment = 0.5D,
        .DecimalPlaces = 2,
        .Value =
            CDec(
                Math.Max(
                    1.0R,
                    Math.Min(
                        500.0R,
                        CampoCanvas.LarguraCampoMetros))),
        .TextAlign =
            HorizontalAlignment.Center
    }

        AdicionarCampoCartao(
        cartaoMetricasReais,
        "Largura do campo (m)",
        nudLarguraCampo)

        Dim chkExibirMetricasCampo As New CheckBox With {
        .Text = "Exibir medidas nos objetos",
        .Checked =
            CampoCanvas.ExibirMetricasNoCampo,
        .AutoSize = False,
        .TextAlign =
            ContentAlignment.MiddleLeft,
        .BackColor =
            Tema.CampoEntrada,
        .ForeColor =
            Tema.TextoCampo
    }

        AdicionarCampoCartao(
        cartaoMetricasReais,
        "Exibição",
        chkExibirMetricasCampo)

        Dim atualizarHabilitacaoMetricas As Action =
        Sub()

            nudComprimentoCampo.Enabled =
                chkUsarMetricasReais.Checked

            nudLarguraCampo.Enabled =
                chkUsarMetricasReais.Checked

            chkExibirMetricasCampo.Enabled =
                chkUsarMetricasReais.Checked

        End Sub

        AddHandler chkUsarMetricasReais.CheckedChanged,
        Sub(sender, e)

            atualizarHabilitacaoMetricas()

        End Sub

        atualizarHabilitacaoMetricas()

        Dim btnAplicarMetricas As New Button With {
        .Text = "Aplicar métricas reais",
        .Width =
            Math.Max(
                240,
                cartaoMetricasReais.Width - 20),
        .Height = 38,
        .Margin =
            New Padding(
                8,
                8,
                8,
                0),
        .FlatStyle =
            FlatStyle.Flat,
        .BackColor =
            Tema.CorPrimaria,
        .ForeColor =
            Tema.TextoSobreCorPrimaria,
        .Cursor =
            Cursors.Hand,
        .UseVisualStyleBackColor =
            False
    }

        btnAplicarMetricas.FlatAppearance.BorderColor =
        Tema.Borda

        btnAplicarMetricas.FlatAppearance.MouseOverBackColor =
        Tema.CorPrimariaHover

        AddHandler btnAplicarMetricas.Click,
        Sub(sender, e)

            CampoCanvas.AplicarConfiguracaoMetricasReais(
                chkUsarMetricasReais.Checked,
                CDbl(
                    nudComprimentoCampo.Value),
                CDbl(
                    nudLarguraCampo.Value),
                chkExibirMetricasCampo.Checked)

        End Sub

        cartaoMetricasReais.Controls.Add(
        btnAplicarMetricas)

        painel.Controls.Add(
        cartaoMetricasReais)

        '==================================================
        ' CARTÃO: CORTES RÁPIDOS
        '==================================================

        Dim cartaoCortesRapidos As FlowLayoutPanel =
        CriarCartaoPropriedades(
            "Cortes rápidos",
            Color.FromArgb(
                75,
                36,
                36))

        Dim cmbCorteRapido As New ComboBox With {
        .DropDownStyle =
            ComboBoxStyle.DropDownList,
        .FlatStyle =
            FlatStyle.Flat
    }

        cmbCorteRapido.Items.AddRange(
        New Object() {
            "Campo inteiro",
            "Meio campo esquerdo",
            "Meio campo direito",
            "Metade superior",
            "Metade inferior",
            "Terço esquerdo",
            "Terço central",
            "Terço direito",
            "Área esquerda",
            "Área direita"
        })

        cmbCorteRapido.SelectedIndex = 0

        AdicionarCampoCartao(
        cartaoCortesRapidos,
        "Pré-definição",
        cmbCorteRapido)
        Dim botaoCampoInteiro As Button = Nothing

        Dim btnAplicarCorte As New Button With {
        .Text = "Aplicar corte",
        .Width =
            Math.Max(
                240,
                cartaoCortesRapidos.Width - 20),
        .Height = 38,
        .Margin =
            New Padding(
                8,
                8,
                8,
                0),
        .FlatStyle =
            FlatStyle.Flat,
        .BackColor =
            Tema.CorPrimaria,
        .ForeColor =
            Tema.TextoSobreCorPrimaria,
        .Cursor =
            Cursors.Hand,
        .UseVisualStyleBackColor =
            False
    }

        btnAplicarCorte.FlatAppearance.BorderColor =
        Tema.Borda

        btnAplicarCorte.FlatAppearance.MouseOverBackColor =
        Tema.CorPrimariaHover

        AddHandler btnAplicarCorte.Click,
        Sub(sender, e)

            Dim tipoRecorte As TipoRecorteCampo =
                TipoRecorteCampo.CampoInteiro

            Select Case cmbCorteRapido.SelectedIndex

                Case 0

                    tipoRecorte =
                        TipoRecorteCampo.CampoInteiro

                Case 1

                    tipoRecorte =
                        TipoRecorteCampo.MeioCampoEsquerdo

                Case 2

                    tipoRecorte =
                        TipoRecorteCampo.MeioCampoDireito

                Case 3

                    tipoRecorte =
                        TipoRecorteCampo.MetadeSuperior

                Case 4

                    tipoRecorte =
                        TipoRecorteCampo.MetadeInferior

                Case 5

                    tipoRecorte =
                        TipoRecorteCampo.TercoEsquerdo

                Case 6

                    tipoRecorte =
                        TipoRecorteCampo.TercoCentral

                Case 7

                    tipoRecorte =
                        TipoRecorteCampo.TercoDireito

                Case 8

                    tipoRecorte =
                        TipoRecorteCampo.AreaEsquerda

                Case 9

                    tipoRecorte =
                        TipoRecorteCampo.AreaDireita

                Case Else

                    Exit Sub

            End Select

            Dim aplicou As Boolean =
                CampoCanvas.AplicarRecortePredefinido(
                    tipoRecorte)

            If Not aplicou Then
                Exit Sub
            End If

            AtualizarEstadoBotaoRecorte()

            botaoCampoInteiro.Enabled = CampoCanvas.RecorteAtivo

        End Sub

        cartaoCortesRapidos.Controls.Add(
        btnAplicarCorte)

        painel.Controls.Add(
        cartaoCortesRapidos)

        '==================================================
        ' CARTÃO: ORIENTAÇÃO DOS JOGADORES
        '==================================================

        Dim cartaoOrientacaoTodos As FlowLayoutPanel =
        CriarCartaoPropriedades(
            "Orientação dos jogadores",
            Color.FromArgb(
                75,
                36,
                36))

        Dim cmbOrientacaoTodos As ComboBox =
        CriarComboOrientacaoVisual(
            OrientacaoVisualJogador.Costas)

        AdicionarCampoCartao(
        cartaoOrientacaoTodos,
        "Aplicar em todos os jogadores",
        cmbOrientacaoTodos)

        Dim btnAplicarOrientacaoTodos As New Button With {
        .Text = "Alterar todos os jogadores",
        .Width =
            Math.Max(
                240,
                cartaoOrientacaoTodos.Width - 20),
        .Height = 38,
        .Margin =
            New Padding(
                8,
                8,
                8,
                0),
        .FlatStyle =
            FlatStyle.Flat,
        .BackColor =
            Tema.Painel,
        .ForeColor =
            Tema.Texto,
        .Cursor =
            Cursors.Hand,
        .UseVisualStyleBackColor =
            False
    }

        btnAplicarOrientacaoTodos.FlatAppearance.BorderColor =
        Tema.Borda

        btnAplicarOrientacaoTodos.FlatAppearance.MouseOverBackColor =
        Tema.PainelHover

        AddHandler btnAplicarOrientacaoTodos.Click,
        Sub(sender, e)

            Dim possuiJogadores As Boolean = False

            For Each objeto As ObjetoCampo
                In CampoCanvas.ObjetosAtuais

                If TypeOf objeto Is Jogador Then

                    possuiJogadores = True

                    Exit For

                End If

            Next

            If Not possuiJogadores Then

                MessageBox.Show(
                    "O campo ainda não possui jogadores.",
                    "Orientação dos jogadores",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information)

                Exit Sub

            End If

            Dim alterou As Boolean =
                CampoCanvas.
                    AlterarOrientacaoVisualTodosJogadores(
                        ObterOrientacaoVisualSelecionada(
                            cmbOrientacaoTodos))

            If Not alterou Then

                MessageBox.Show(
                    "Todos os jogadores já estão nessa orientação ou estão bloqueados.",
                    "Orientação dos jogadores",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information)

            End If

        End Sub

        cartaoOrientacaoTodos.Controls.Add(
        btnAplicarOrientacaoTodos)

        painel.Controls.Add(
        cartaoOrientacaoTodos)

        Dim selecionando As Boolean =
        CampoCanvas.ModoSelecaoRecorteAtivo

        Dim statusTexto As String
        Dim orientacaoTexto As String

        If selecionando Then

            statusTexto =
            "Selecionando nova área..."

            orientacaoTexto =
            "Clique em um ponto do campo, mantenha o botão " &
            "esquerdo pressionado e arraste livremente até " &
            "formar a região desejada." &
            Environment.NewLine &
            Environment.NewLine &
            "A largura e a altura podem ser escolhidas de " &
            "forma independente, como na ferramenta de área." &
            Environment.NewLine &
            Environment.NewLine &
            "Use o botão direito ou a tecla Esc para cancelar."

        ElseIf CampoCanvas.RecorteAtivo Then

            statusTexto =
            "Recorte ativo"

            orientacaoTexto =
            "A região selecionada continuará ativa ao criar, " &
            "mover ou editar os elementos do exercício." &
            Environment.NewLine &
            Environment.NewLine &
            "PNG, PDF e impressão utilizarão somente essa área."

        Else

            statusTexto =
            "Campo inteiro"

            orientacaoTexto =
            "Nenhuma região está recortada." &
            Environment.NewLine &
            Environment.NewLine &
            "Clique em Selecionar nova área para criar um recorte."

        End If

        Dim status As New Label With {
        .Text = statusTexto,
        .ForeColor =
            If(
                selecionando,
                Color.Gold,
                Tema.Texto),
        .Font = New Font(
            "Segoe UI",
            9.5F,
            FontStyle.Bold),
        .Width = largura,
        .Height = 30,
        .TextAlign =
            ContentAlignment.MiddleLeft
    }

        painel.Controls.Add(
        status)

        Dim alturaOrientacao As Integer

        If selecionando Then

            alturaOrientacao = 150

        ElseIf CampoCanvas.RecorteAtivo Then

            alturaOrientacao = 100

        Else

            alturaOrientacao = 82

        End If

        Dim orientacao As New Label With {
        .Text = orientacaoTexto,
        .ForeColor = Tema.TextoSecundario,
        .Font = Tema.FontePadrao,
        .Width = largura,
        .Height = alturaOrientacao,
        .TextAlign =
            ContentAlignment.TopLeft
    }

        painel.Controls.Add(
        orientacao)

        If CampoCanvas.RecorteAtivo AndAlso
       Not selecionando Then

            Dim recorte As RectangleF =
            CampoCanvas.RetanguloRecortePercentual

            Dim detalhes As New Label With {
            .Text =
                "Início X: " &
                recorte.Left.ToString("0.0") &
                "%" &
                Environment.NewLine &
                "Início Y: " &
                recorte.Top.ToString("0.0") &
                "%" &
                Environment.NewLine &
                "Largura: " &
                recorte.Width.ToString("0.0") &
                "%" &
                Environment.NewLine &
                "Altura: " &
                recorte.Height.ToString("0.0") &
                "%",
            .ForeColor = Tema.Texto,
            .Font = Tema.FontePadrao,
            .Width = largura,
            .Height = 92,
            .Margin =
                New Padding(
                    0,
                    8,
                    0,
                    8),
            .TextAlign =
                ContentAlignment.MiddleLeft
        }

            painel.Controls.Add(
            detalhes)

        End If

        Dim botaoSelecionar As New Button With {
        .Text =
            If(
                selecionando,
                "Cancelar seleção do recorte",
                "Selecionar nova área"),
        .Width = largura,
        .Height = 38,
        .Margin =
            New Padding(
                0,
                8,
                0,
                4),
        .FlatStyle =
            FlatStyle.Flat,
        .BackColor =
            If(
                selecionando,
                Tema.CorPrimaria,
                Tema.Painel),
        .ForeColor =
            If(
                selecionando,
                Tema.TextoSobreCorPrimaria,
                Tema.Texto),
        .Cursor =
            Cursors.Hand,
        .UseVisualStyleBackColor =
            False
    }

        botaoSelecionar.FlatAppearance.BorderColor =
        If(
            selecionando,
            Tema.TextoSobreCorPrimaria,
            Tema.Borda)

        botaoSelecionar.FlatAppearance.MouseOverBackColor =
        Tema.PainelHover

        AddHandler botaoSelecionar.Click,
        Sub(sender, e)

            If CampoCanvas.ModoSelecaoRecorteAtivo Then

                CampoCanvas.CancelarSelecaoRecorteCampo()

            Else

                CampoCanvas.IniciarSelecaoRecorteCampo()

            End If

            CampoCanvas.Focus()

        End Sub

        painel.Controls.Add(
        botaoSelecionar)

        botaoCampoInteiro =
    New Button With {
        .Text = "Usar campo inteiro",
        .Width = largura,
        .Height = 38,
        .Margin =
            New Padding(
                0,
                4,
                0,
                4),
        .FlatStyle =
            FlatStyle.Flat,
        .BackColor =
            Tema.Painel,
        .ForeColor =
            Tema.Texto,
        .Cursor =
            Cursors.Hand,
        .UseVisualStyleBackColor =
            False,
        .Enabled =
            CampoCanvas.RecorteAtivo OrElse
            selecionando
    }

        botaoCampoInteiro.FlatAppearance.BorderColor =
        Tema.Borda

        botaoCampoInteiro.FlatAppearance.MouseOverBackColor =
        Tema.PainelHover

        AddHandler botaoCampoInteiro.Click,
        Sub(sender, e)

            CampoCanvas.LimparRecorteCampo()

            AtualizarEstadoBotaoRecorte()

            botaoCampoInteiro.Enabled =
    False

        End Sub

        painel.Controls.Add(
        botaoCampoInteiro)

        Dim avisoExportacao As New Label With {
        .Text =
            "PNG, PDF e impressão seguem automaticamente " &
            "o recorte ativo.",
        .ForeColor = Tema.TextoSecundario,
        .Font = New Font(
            "Segoe UI",
            8.5F,
            FontStyle.Italic),
        .Width = largura,
        .Height = 58,
        .Margin =
            New Padding(
                0,
                12,
                0,
                0),
        .TextAlign =
            ContentAlignment.MiddleLeft
    }

        painel.Controls.Add(
        avisoExportacao)

    End Sub

    Private Sub MontarPainelPropriedades(objeto As ObjetoCampo)

        If _painelConfiguracoesCampoAberto AndAlso
   objeto Is Nothing Then

            Exit Sub

        End If

        _painelConfiguracoesCampoAberto =
    False

        _versaoMontagemPropriedades += 1

        Dim versaoAtual As Integer =
        _versaoMontagemPropriedades

        _montandoPainelPropriedades = True

        LimparPainelDireito()

        PnlDireito.SuspendLayout()

        Dim montagemConcluida As Boolean = False

        Try

            _containerPainelPropriedades =
            New Panel With {
                .Dock = DockStyle.Fill,
                .Padding = New Padding(0),
                .Margin = New Padding(0),
                .BackColor = Tema.Fundo,
                .AutoScroll = True,
                .Visible = False
            }

            Dim larguraDisponivel As Integer =
            PnlDireito.ClientSize.Width -
            SystemInformation.VerticalScrollBarWidth -
            18

            Dim larguraPainel As Integer =
            Math.Max(
                240,
                Math.Min(
                    380,
                    larguraDisponivel))

            _painelConteudoPropriedades =
            New FlowLayoutPanel With {
                .AutoSize = False,
                .Width = larguraPainel,
                .Height = 1,
                .FlowDirection = FlowDirection.TopDown,
                .WrapContents = False,
                .Margin = New Padding(0),
                .Padding = New Padding(6),
                .BackColor = Tema.Fundo
            }

            AddHandler _containerPainelPropriedades.Resize,
            AddressOf ContainerPainelPropriedades_Resize

            _containerPainelPropriedades.Controls.Add(
            _painelConteudoPropriedades)

            PnlDireito.Controls.Add(
            _containerPainelPropriedades)

            Dim painel As FlowLayoutPanel =
            _painelConteudoPropriedades

            Dim largura As Integer =
            Math.Max(
                210,
                painel.Width -
                painel.Padding.Horizontal)

            Dim titulo As New Label With {
            .Text = "PROPRIEDADES",
            .ForeColor = Tema.Texto,
            .BackColor = Tema.Fundo,
            .Font = New Font(
                "Segoe UI",
                11.0F,
                FontStyle.Bold),
            .Width = largura,
            .Height = 32,
            .Margin = New Padding(0),
            .TextAlign = ContentAlignment.MiddleLeft
        }

            painel.Controls.Add(
            titulo)

            If objeto Is Nothing Then

                Dim mensagem As New Label With {
                .Text = "Nenhum objeto selecionado.",
                .ForeColor = Tema.TextoSecundario,
                .BackColor = Tema.Fundo,
                .Font = Tema.FontePadrao,
                .Width = largura,
                .Height = 50,
                .Margin = New Padding(0),
                .TextAlign = ContentAlignment.MiddleLeft
            }

                painel.Controls.Add(
                mensagem)

            Else

                Dim tipoObjeto As New Label With {
                .Text = ObterNomeObjeto(objeto),
                .ForeColor = Tema.CorPrimaria,
                .BackColor = Tema.Fundo,
                .Font = New Font(
                    "Segoe UI",
                    10.0F,
                    FontStyle.Bold),
                .Width = largura,
                .Height = 30,
                .Margin = New Padding(0),
                .TextAlign = ContentAlignment.MiddleLeft
            }

                painel.Controls.Add(
                tipoObjeto)

                If TypeOf objeto Is Jogador Then

                    MontarPropriedadesJogador(
                    painel,
                    DirectCast(
                        objeto,
                        Jogador))

                ElseIf TypeOf objeto Is Bola Then

                    AdicionarMensagemPainel(
                    painel,
                    "A bola não possui propriedades editáveis no momento.")

                ElseIf TypeOf objeto Is Cone Then

                    MontarPropriedadesCone(
                    painel,
                    DirectCast(
                        objeto,
                        Cone))

                ElseIf TypeOf objeto Is Gol Then

                    MontarPropriedadesGol(
                    painel,
                    DirectCast(
                        objeto,
                        Gol))

                ElseIf TypeOf objeto Is Manequim Then

                    MontarPropriedadesManequim(
                    painel,
                    DirectCast(
                        objeto,
                        Manequim))

                ElseIf TypeOf objeto Is LinhaTatica Then

                    MontarPropriedadesLinha(
                    painel,
                    DirectCast(
                        objeto,
                        LinhaTatica))

                ElseIf TypeOf objeto Is AreaTatica Then

                    MontarPropriedadesArea(
                    painel,
                    DirectCast(
                        objeto,
                        AreaTatica))

                ElseIf TypeOf objeto Is MarcadorTatico Then

                    MontarPropriedadesMarcador(
                    painel,
                    DirectCast(
                        objeto,
                        MarcadorTatico))

                ElseIf TypeOf objeto Is TextoTatico Then

                    MontarPropriedadesTexto(
                    painel,
                    DirectCast(
                        objeto,
                        TextoTatico))

                End If

                '==================================================
                ' CARTÃO: MÉTRICA DO OBJETO
                '==================================================

                If Not TypeOf objeto Is TextoTatico Then

                    Dim cartaoMetrica As FlowLayoutPanel =
        CriarCartaoPropriedades(
            "Métrica",
            Color.FromArgb(
                45,
                85,
                70))

                    Dim chkExibirMetrica As New CheckBox With {
        .Text = "Exibir métrica deste objeto",
        .Checked = objeto.ExibirMetrica,
        .AutoSize = False,
        .TextAlign =
            ContentAlignment.MiddleLeft,
        .BackColor =
            Tema.CampoEntrada,
        .ForeColor =
            Tema.TextoCampo
    }

                    AddHandler chkExibirMetrica.CheckedChanged,
        Sub(sender, e)

            If objeto.ExibirMetrica =
               chkExibirMetrica.Checked Then

                Exit Sub

            End If

            objeto.ExibirMetrica =
                chkExibirMetrica.Checked

            CampoCanvas.RegistrarAlteracaoExterna()

        End Sub

                    AdicionarCampoCartao(
        cartaoMetrica,
        "Exibição",
        chkExibirMetrica)

                    painel.Controls.Add(
        cartaoMetrica)

                End If

                If Not TypeOf objeto Is Jogador Then

                    MontarEscalaVisualObjeto(
                        painel,
                        objeto)

                End If

                '==================================================
                ' CARTÃO: AÇÕES
                '==================================================

                Dim cartaoAcoes As FlowLayoutPanel =
                CriarCartaoPropriedades(
                    "Ações",
                    Color.FromArgb(
                        70,
                        70,
                        76))

                AdicionarBotaoCartao(
                cartaoAcoes,
                CriarBotaoAcaoCartao(
                    "Duplicar objeto  (Ctrl+D)",
                    Sub()

                        CampoCanvas.DuplicarSelecionado()

                    End Sub))

                AdicionarLinhaDuplaCartao(
                cartaoAcoes,
                CriarBotaoAcaoCartao(
                    "Trazer para frente",
                    Sub()

                        CampoCanvas.TrazerParaFrente()

                    End Sub),
                CriarBotaoAcaoCartao(
                    "Enviar para trás",
                    Sub()

                        CampoCanvas.EnviarParaTras()

                    End Sub))

                painel.Controls.Add(
                cartaoAcoes)

                '==================================================
                ' CARTÃO: PROTEÇÃO
                '==================================================

                Dim textoBloqueio As String =
                If(
                    objeto.Bloqueado,
                    "Desbloquear objeto",
                    "Bloquear objeto")

                Dim cartaoProtecao As FlowLayoutPanel =
                CriarCartaoPropriedades(
                    "Proteção",
                    Color.FromArgb(
                        92,
                        65,
                        24))

                AdicionarBotaoCartao(
                cartaoProtecao,
                CriarBotaoAcaoCartao(
                    textoBloqueio,
                    Sub()

                        CampoCanvas.AlternarBloqueioSelecionados()

                    End Sub))

                painel.Controls.Add(
                cartaoProtecao)

                '==================================================
                ' CARTÃO: EXCLUSÃO
                '==================================================

                Dim cartaoExclusao As FlowLayoutPanel =
                CriarCartaoPropriedades(
                    "Excluir",
                    Tema.CorPrimaria)

                AdicionarBotaoCartao(
                cartaoExclusao,
                CriarBotaoAcaoCartao(
                    "Excluir objeto",
                    Sub()

                        CampoCanvas.ExcluirSelecionado()

                    End Sub,
                    True))

                painel.Controls.Add(
                cartaoExclusao)

            End If

            painel.PerformLayout()

            Dim tamanhoPreferido As Size =
            painel.GetPreferredSize(
                New Size(
                    painel.Width,
                    0))

            painel.Height =
            Math.Max(
                1,
                tamanhoPreferido.Height)

            _containerPainelPropriedades.AutoScrollPosition =
            Point.Empty

            montagemConcluida = True

        Finally

            PnlDireito.ResumeLayout(
            True)

            _montandoPainelPropriedades = False

        End Try

        If montagemConcluida AndAlso
       versaoAtual =
       _versaoMontagemPropriedades Then

            CentralizarPainelPropriedades()

            _containerPainelPropriedades.Visible = True

            _containerPainelPropriedades.Invalidate(
            True)

        End If

    End Sub

    Private Sub AdicionarCampoPainel(
    painel As FlowLayoutPanel,
    titulo As String,
    controle As Control)

        Dim largura As Integer =
            Math.Max(
                180,
                PnlDireito.ClientSize.Width - 32)

        Dim container As New Panel With {
            .Width = largura,
            .Height = 60,
            .Margin = New Padding(0, 4, 0, 4),
            .BackColor = Tema.Painel
        }

        Dim labelCampo As New Label With {
            .Text = titulo,
            .ForeColor = Tema.Texto,
            .Font = New Font(
                "Segoe UI",
                8.5F,
                FontStyle.Regular),
            .Left = 0,
            .Top = 0,
            .Width = largura,
            .Height = 22
        }

        controle.Left = 0
        controle.Top = 24
        controle.Width = largura
        controle.Height = 28
        controle.Anchor =
            AnchorStyles.Left Or
            AnchorStyles.Right Or
            AnchorStyles.Top

        container.Controls.Add(labelCampo)
        container.Controls.Add(controle)

        painel.Controls.Add(container)

    End Sub

    Private Sub AdicionarMensagemPainel(
        painel As FlowLayoutPanel,
        mensagem As String)

        Dim largura As Integer =
            Math.Max(
                180,
                PnlDireito.ClientSize.Width - 32)

        Dim labelMensagem As New Label With {
            .Text = mensagem,
            .ForeColor = Tema.TextoSecundario,
            .Font = Tema.FontePadrao,
            .Width = largura,
            .Height = 55,
            .TextAlign = ContentAlignment.MiddleLeft
        }

        painel.Controls.Add(labelMensagem)

    End Sub

    Private Function CriarComboEnum(
        tipoEnum As Type,
        valorAtual As Object) As ComboBox

        Dim combo As New ComboBox With {
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .BackColor = Tema.CampoEntrada,
            .ForeColor = Tema.TextoCampo,
            .FlatStyle = FlatStyle.Flat
        }

        combo.DataSource =
            [Enum].GetValues(tipoEnum)

        combo.SelectedItem =
            valorAtual

        Return combo

    End Function

    Private Function CriarComboOrientacaoVisual(
        valorAtual As OrientacaoVisualJogador
    ) As ComboBox

        Dim combo As New ComboBox With {
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .BackColor = Tema.CampoEntrada,
            .ForeColor = Tema.TextoCampo,
            .FlatStyle = FlatStyle.Flat
        }

        combo.Items.AddRange(
            New Object() {
                "Costas",
                "Frente",
                "Lado direito",
                "Lado esquerdo"
            })

        Select Case valorAtual

            Case OrientacaoVisualJogador.Frente

                combo.SelectedIndex =
                    1

            Case OrientacaoVisualJogador.Lado

                combo.SelectedIndex =
                    2

            Case OrientacaoVisualJogador.LadoEsquerdo

                combo.SelectedIndex =
                    3

            Case Else

                combo.SelectedIndex =
                    0

        End Select

        Return combo

    End Function

    Private Function ObterOrientacaoVisualSelecionada(
        combo As ComboBox
    ) As OrientacaoVisualJogador

        If combo Is Nothing Then

            Return OrientacaoVisualJogador.Costas

        End If

        Select Case combo.SelectedIndex

            Case 1

                Return OrientacaoVisualJogador.Frente

            Case 2

                Return OrientacaoVisualJogador.Lado

            Case 3

                Return OrientacaoVisualJogador.LadoEsquerdo

            Case Else

                Return OrientacaoVisualJogador.Costas

        End Select

    End Function

    Private Function CriarCampoNumerico(
        minimo As Decimal,
        maximo As Decimal,
        valor As Decimal,
        Optional casasDecimais As Integer = 0,
        Optional incremento As Decimal = 1D) As NumericUpDown

        Return New NumericUpDown With {
            .Minimum = minimo,
            .Maximum = maximo,
            .Value = Math.Max(
                minimo,
                Math.Min(maximo, valor)),
            .DecimalPlaces = casasDecimais,
            .Increment = incremento,
            .BackColor = Tema.CampoEntrada,
            .ForeColor = Tema.TextoCampo,
            .BorderStyle = BorderStyle.FixedSingle
        }

    End Function

    Private Function ObterNomeObjeto(objeto As ObjetoCampo) As String

        If objeto Is Nothing Then
            Return String.Empty
        End If

        Dim nomePersonalizado As String = If(objeto.NomePersonalizado, String.Empty).Trim()

        If Not String.IsNullOrWhiteSpace(nomePersonalizado) Then

            Return nomePersonalizado

        End If

        If TypeOf objeto Is Jogador Then
            Return "Jogador"
        End If

        If TypeOf objeto Is Bola Then
            Return "Bola"
        End If

        If TypeOf objeto Is Cone Then
            Return "Cone"
        End If

        If TypeOf objeto Is Gol Then
            Return "Gol"
        End If

        If TypeOf objeto Is Manequim Then
            Return "Manequim"
        End If

        If TypeOf objeto Is LinhaTatica Then
            Return "Linha tática"
        End If

        If TypeOf objeto Is AreaTatica Then
            Return "Área tática"
        End If

        If TypeOf objeto Is MarcadorTatico Then
            Return "Marcador"
        End If

        If TypeOf objeto Is TextoTatico Then
            Return "Texto tático"
        End If

        Return objeto.GetType().Name

    End Function

    Private Function CriarCartaoPropriedades(
        titulo As String,
        Optional corDestaque As Color = Nothing
    ) As FlowLayoutPanel

        If corDestaque = Color.Empty Then

            corDestaque =
                Tema.CorPrimaria

        End If

        Dim larguraBase As Integer =
            280

        If _painelConteudoPropriedades IsNot Nothing AndAlso
           Not _painelConteudoPropriedades.IsDisposed AndAlso
           _painelConteudoPropriedades.Width > 0 Then

            larguraBase =
                _painelConteudoPropriedades.Width -
                _painelConteudoPropriedades.Padding.Horizontal

        End If

        larguraBase =
            Math.Max(
                220,
                larguraBase)

        Dim cartao As New FlowLayoutPanel With {
            .Width = larguraBase,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False,
            .Margin = New Padding(
                0,
                0,
                0,
                8),
            .Padding = New Padding(
                0,
                0,
                0,
                8),
            .BackColor = Tema.Painel,
            .BorderStyle = BorderStyle.FixedSingle
        }

        Dim cabecalho As New Label With {
            .Text = titulo.ToUpperInvariant(),
            .Width = larguraBase - 2,
            .Height = 32,
            .Margin = New Padding(0),
            .Padding = New Padding(
                14,
                0,
                0,
                0),
            .BackColor = corDestaque,
            .ForeColor = Tema.ObterCorTextoContraste(corDestaque),
            .Font = New Font(
                "Segoe UI",
                8.8F,
                FontStyle.Bold),
            .TextAlign = ContentAlignment.MiddleLeft
        }

        cartao.Controls.Add(
            cabecalho)

        Return cartao

    End Function

    Private Sub AdicionarCampoCartao(
        cartao As FlowLayoutPanel,
        titulo As String,
        controle As Control)

        If cartao Is Nothing OrElse
           controle Is Nothing Then

            Exit Sub

        End If

        Dim larguraCampo As Integer =
            Math.Max(
                180,
                cartao.Width - 20)

        Dim container As New TableLayoutPanel With {
            .Width = larguraCampo,
            .Height = 54,
            .ColumnCount = 1,
            .RowCount = 2,
            .Margin = New Padding(
                8,
                6,
                8,
                0),
            .Padding = New Padding(0),
            .BackColor = Tema.Painel,
            .GrowStyle =
                TableLayoutPanelGrowStyle.FixedSize
        }

        container.ColumnStyles.Add(
            New ColumnStyle(
                SizeType.Percent,
                100.0F))

        container.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                22.0F))

        container.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                30.0F))

        Dim labelCampo As New Label With {
            .Text = titulo,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .ForeColor = Tema.TextoSecundario,
            .BackColor = Tema.Painel,
            .Font = New Font(
                "Segoe UI",
                8.5F,
                FontStyle.Regular),
            .TextAlign =
                ContentAlignment.MiddleLeft
        }

        container.Controls.Add(
            labelCampo,
            0,
            0)

        controle.Dock =
            DockStyle.Fill

        controle.Margin =
            New Padding(
                0,
                2,
                0,
                0)

        controle.BackColor =
            Tema.CampoEntrada

        controle.ForeColor =
            Tema.TextoCampo

        container.Controls.Add(
            controle,
            0,
            1)

        cartao.Controls.Add(
            container)

    End Sub

    Private Function CriarBotaoAcaoCartao(texto As String, acao As Action, Optional destrutivo As Boolean = False) As Button

        Dim botao As New Button With {
            .Text = texto,
            .Height = 36,
            .FlatStyle = FlatStyle.Flat,
            .Cursor = Cursors.Hand,
            .UseVisualStyleBackColor = False,
            .TextAlign = ContentAlignment.MiddleCenter
        }

        If destrutivo Then

            botao.BackColor =
                Tema.CorPrimaria

            botao.ForeColor =
                Color.White

            botao.FlatAppearance.BorderColor =
                Color.FromArgb(
                    235,
                    235,
                    235)

            botao.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(
                    165,
                    35,
                    35)

        Else

            botao.BackColor =
                Tema.Fundo

            botao.ForeColor =
                Tema.Texto

            botao.FlatAppearance.BorderColor =
                Tema.Borda

            botao.FlatAppearance.MouseOverBackColor =
                Tema.PainelHover

        End If

        botao.FlatAppearance.MouseDownBackColor =
            Tema.CorPrimariaPressionada

        AddHandler botao.Click,
            Sub(sender, e)

                If acao IsNot Nothing Then

                    acao.Invoke()

                End If

                CampoCanvas.Focus()

            End Sub

        Return botao

    End Function

    Private Sub AdicionarBotaoCartao(
        cartao As FlowLayoutPanel,
        botao As Button)

        If cartao Is Nothing OrElse
           botao Is Nothing Then

            Exit Sub

        End If

        botao.Width =
            Math.Max(
                180,
                cartao.Width - 20)

        botao.Margin =
            New Padding(
                8,
                7,
                8,
                0)

        cartao.Controls.Add(
            botao)

    End Sub

    Private Sub AdicionarLinhaDuplaCartao(
        cartao As FlowLayoutPanel,
        botaoEsquerdo As Button,
        botaoDireito As Button)

        If cartao Is Nothing OrElse
           botaoEsquerdo Is Nothing OrElse
           botaoDireito Is Nothing Then

            Exit Sub

        End If

        Dim larguraLinha As Integer =
            Math.Max(
                180,
                cartao.Width - 20)

        Dim linha As New TableLayoutPanel With {
            .Width = larguraLinha,
            .Height = 44,
            .ColumnCount = 2,
            .RowCount = 1,
            .Margin = New Padding(
                8,
                4,
                8,
                0),
            .Padding = New Padding(0),
            .BackColor = Tema.Painel,
            .GrowStyle =
                TableLayoutPanelGrowStyle.FixedSize
        }

        linha.ColumnStyles.Add(
            New ColumnStyle(
                SizeType.Percent,
                50.0F))

        linha.ColumnStyles.Add(
            New ColumnStyle(
                SizeType.Percent,
                50.0F))

        linha.RowStyles.Add(
            New RowStyle(
                SizeType.Percent,
                100.0F))

        botaoEsquerdo.Dock =
            DockStyle.Fill

        botaoEsquerdo.Margin =
            New Padding(
                0,
                3,
                3,
                3)

        botaoDireito.Dock =
            DockStyle.Fill

        botaoDireito.Margin =
            New Padding(
                3,
                3,
                0,
                3)

        linha.Controls.Add(
            botaoEsquerdo,
            0,
            0)

        linha.Controls.Add(
            botaoDireito,
            1,
            0)

        cartao.Controls.Add(
            linha)

    End Sub

    Private Sub CentralizarPainelPropriedades()

        If _ajustandoPainelPropriedades OrElse
           _montandoPainelPropriedades Then

            Exit Sub

        End If

        If _containerPainelPropriedades Is Nothing OrElse
           _painelConteudoPropriedades Is Nothing OrElse
           _containerPainelPropriedades.IsDisposed OrElse
           _painelConteudoPropriedades.IsDisposed Then

            Exit Sub

        End If

        If _containerPainelPropriedades.ClientSize.Width <= 0 OrElse
           _containerPainelPropriedades.ClientSize.Height <= 0 Then

            Exit Sub

        End If

        _ajustandoPainelPropriedades =
            True

        Try

            Dim larguraDisponivel As Integer =
                _containerPainelPropriedades.ClientSize.Width

            Dim alturaDisponivel As Integer =
                _containerPainelPropriedades.ClientSize.Height

            Dim larguraConteudo As Integer =
                _painelConteudoPropriedades.Width

            Dim alturaConteudo As Integer =
                _painelConteudoPropriedades.Height

            Dim esquerda As Integer =
                Math.Max(
                    6,
                    (larguraDisponivel -
                     larguraConteudo) \ 2)

            Dim topo As Integer

            If alturaConteudo <
               alturaDisponivel - 16 Then

                topo =
                    Math.Max(
                        8,
                        (alturaDisponivel -
                         alturaConteudo) \ 3)

            Else

                topo =
                    8

            End If

            _painelConteudoPropriedades.Location =
                New Point(
                    esquerda,
                    topo)

            _containerPainelPropriedades.AutoScrollMinSize =
                New Size(
                    0,
                    topo +
                    alturaConteudo +
                    12)

        Finally

            _ajustandoPainelPropriedades =
                False

        End Try

    End Sub

#End Region

#Region "Propriedades dos objetos"

    Private Sub MontarEscalaVisualObjeto(
        painel As FlowLayoutPanel,
        objeto As ObjetoCampo)

        If painel Is Nothing OrElse
           objeto Is Nothing Then

            Exit Sub

        End If

        Dim cartaoTamanho As FlowLayoutPanel =
            CriarCartaoPropriedades(
                "Tamanho visual",
                Color.FromArgb(
                    60,
                    70,
                    88))

        Dim escala As NumericUpDown =
            CriarCampoNumerico(
                0.5D,
                2.5D,
                CDec(
                    objeto.EscalaVisual),
                1,
                0.1D)

        escala.ThousandsSeparator =
            False

        AddHandler escala.ValueChanged,
            Sub(sender, e)

                CampoCanvas.
                    AlterarEscalaVisualSelecionados(
                        CSng(
                            escala.Value))

            End Sub

        AdicionarCampoCartao(
            cartaoTamanho,
            "Escala do objeto",
            escala)

        painel.Controls.Add(
            cartaoTamanho)

    End Sub

    Private Sub MontarPropriedadesJogador(painel As FlowLayoutPanel, jogador As Jogador)

        If painel Is Nothing OrElse
       jogador Is Nothing Then

            Exit Sub

        End If

        '==================================================
        ' CARTÃO: IDENTIFICAÇÃO
        '==================================================

        Dim cartaoIdentificacao As FlowLayoutPanel =
        CriarCartaoPropriedades(
            "Identificação")

        Dim numero As NumericUpDown =
        CriarCampoNumerico(
            1D,
            99D,
            jogador.Numero)

        AddHandler numero.ValueChanged,
        Sub(sender, e)

            jogador.Numero =
                CInt(
                    numero.Value)

            CampoCanvas.
                RegistrarAlteracaoExterna()

        End Sub

        AdicionarCampoCartao(
        cartaoIdentificacao,
        "Número",
        numero)

        Dim nome As New TextBox With {
        .Text = jogador.Nome,
        .BorderStyle =
            BorderStyle.FixedSingle
    }

        AddHandler nome.TextChanged,
        Sub(sender, e)

            jogador.Nome =
                nome.Text

            CampoCanvas.
                RegistrarAlteracaoExterna()

        End Sub

        AdicionarCampoCartao(
        cartaoIdentificacao,
        "Nome",
        nome)

        painel.Controls.Add(
        cartaoIdentificacao)

        '==================================================
        ' CARTÃO: APARÊNCIA
        '==================================================

        Dim cartaoAparencia As FlowLayoutPanel =
        CriarCartaoPropriedades(
            "Aparência",
            Color.FromArgb(
                105,
                30,
                30))

        Dim escala As NumericUpDown =
        CriarCampoNumerico(
            0.5D,
            2.5D,
            CDec(
                jogador.EscalaVisual),
            1,
            0.1D)

        escala.ThousandsSeparator =
        False

        AddHandler escala.ValueChanged,
        Sub(sender, e)

            CampoCanvas.
                AlterarEscalaVisualSelecionados(
                    CSng(
                        escala.Value))

        End Sub

        AdicionarCampoCartao(
        cartaoAparencia,
        "Tamanho visual",
        escala)

        Dim corAtual As Color =
    Color.FromArgb(
        jogador.CorCamisaArgb)

        If corAtual.A = 0 Then

            corAtual =
        Color.FromArgb(
            185,
            35,
            35)

        End If

        Dim painelCor As New TableLayoutPanel With {
    .ColumnCount = 2,
    .RowCount = 1,
    .Margin = New Padding(0),
    .Padding = New Padding(0),
    .BackColor = Tema.CampoEntrada
}

        painelCor.ColumnStyles.Add(
    New ColumnStyle(
        SizeType.Absolute,
        52.0F))

        painelCor.ColumnStyles.Add(
    New ColumnStyle(
        SizeType.Percent,
        100.0F))

        painelCor.RowStyles.Add(
    New RowStyle(
        SizeType.Percent,
        100.0F))

        Dim amostraCor As New Panel With {
    .Dock = DockStyle.Fill,
    .Margin = New Padding(
        0,
        2,
        8,
        2),
    .BackColor = corAtual,
    .BorderStyle = BorderStyle.FixedSingle
}

        painelCor.Controls.Add(
    amostraCor,
    0,
    0)

        Dim botaoEscolherCor As New Button With {
    .Text = "Escolher cor",
    .Dock = DockStyle.Fill,
    .Margin = New Padding(0),
    .FlatStyle = FlatStyle.Flat,
    .BackColor = Tema.Painel,
    .ForeColor = Tema.Texto,
    .Cursor = Cursors.Hand,
    .UseVisualStyleBackColor = False
}

        botaoEscolherCor.FlatAppearance.BorderColor =
    Tema.Borda

        botaoEscolherCor.FlatAppearance.MouseOverBackColor =
    Tema.PainelHover

        AddHandler botaoEscolherCor.Click,
    Sub(sender, e)

        Using seletorCor As New ColorDialog()

            seletorCor.Color =
                amostraCor.BackColor

            seletorCor.FullOpen =
                True

            seletorCor.AnyColor =
                True

            If seletorCor.ShowDialog(Me) <>
               DialogResult.OK Then

                Exit Sub

            End If

            Dim novaCor As Color =
                Color.FromArgb(
                    255,
                    seletorCor.Color.R,
                    seletorCor.Color.G,
                    seletorCor.Color.B)

            If CampoCanvas.
                AlterarCorCamisaJogadoresSelecionados(
                    novaCor) Then

                amostraCor.BackColor =
                    novaCor

            End If

            CampoCanvas.Focus()

        End Using

    End Sub

        painelCor.Controls.Add(
    botaoEscolherCor,
    1,
    0)

        AdicionarCampoCartao(
    cartaoAparencia,
    "Cor da camisa",
    painelCor)

        Dim orientacaoVisual As ComboBox =
            CriarComboOrientacaoVisual(
                jogador.OrientacaoVisual)

        AddHandler orientacaoVisual.SelectedIndexChanged,
            Sub(sender, e)

                CampoCanvas.
                    AlterarOrientacaoVisualJogadoresSelecionados(
                        ObterOrientacaoVisualSelecionada(
                            orientacaoVisual))

            End Sub

        AdicionarCampoCartao(
        cartaoAparencia,
        "Orientação visual",
        orientacaoVisual)

        Dim pose As ComboBox =
        CriarComboEnum(
            GetType(
                PoseJogador),
            jogador.Pose)

        AddHandler pose.SelectedIndexChanged,
        Sub(sender, e)

            If pose.SelectedItem Is Nothing Then
                Exit Sub
            End If

            CampoCanvas.
                AlterarPoseJogadoresSelecionados(
                    DirectCast(
                        pose.SelectedItem,
                        PoseJogador))

        End Sub

        AdicionarCampoCartao(
        cartaoAparencia,
        "Pose do jogador",
        pose)

        painel.Controls.Add(
        cartaoAparencia)

    End Sub

    Private Sub MontarPropriedadesCone(
    painel As FlowLayoutPanel,
    cone As Cone)

        Dim cor As ComboBox =
            CriarComboEnum(
                GetType(CorCone),
                cone.Cor)

        AddHandler cor.SelectedIndexChanged,
            Sub(sender, e)

                If cor.SelectedItem Is Nothing Then
                    Exit Sub
                End If

                cone.Cor =
                    CType(
                        cor.SelectedItem,
                        CorCone)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Cor",
            cor)

    End Sub

    Private Sub MontarPropriedadesGol(
    painel As FlowLayoutPanel,
    gol As Gol)

        Dim orientacao As ComboBox =
            CriarComboEnum(
                GetType(OrientacaoGol),
                gol.Orientacao)

        AddHandler orientacao.SelectedIndexChanged,
            Sub(sender, e)

                If orientacao.SelectedItem Is Nothing Then
                    Exit Sub
                End If

                gol.Orientacao =
                    CType(
                        orientacao.SelectedItem,
                        OrientacaoGol)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Orientação",
            orientacao)

    End Sub

    Private Sub MontarPropriedadesManequim(
    painel As FlowLayoutPanel,
    manequim As Manequim)

        Dim cor As ComboBox =
            CriarComboEnum(
                GetType(CorManequim),
                manequim.Cor)

        AddHandler cor.SelectedIndexChanged,
            Sub(sender, e)

                If cor.SelectedItem Is Nothing Then
                    Exit Sub
                End If

                manequim.Cor =
                    CType(
                        cor.SelectedItem,
                        CorManequim)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Cor",
            cor)

    End Sub

    Private Sub MontarPropriedadesLinha(
    painel As FlowLayoutPanel,
    linha As LinhaTatica)

        Dim tipo As ComboBox =
            CriarComboEnum(
                GetType(TipoLinhaTatica),
                linha.Tipo)

        AddHandler tipo.SelectedIndexChanged,
            Sub(sender, e)

                If tipo.SelectedItem Is Nothing Then
                    Exit Sub
                End If

                linha.Tipo =
                    CType(
                        tipo.SelectedItem,
                        TipoLinhaTatica)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Tipo",
            tipo)

        Dim cor As ComboBox =
            CriarComboEnum(
                GetType(CorLinhaTatica),
                linha.Cor)

        AddHandler cor.SelectedIndexChanged,
            Sub(sender, e)

                If cor.SelectedItem Is Nothing Then
                    Exit Sub
                End If

                linha.Cor =
                    CType(
                        cor.SelectedItem,
                        CorLinhaTatica)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Cor",
            cor)

        Dim espessura As NumericUpDown =
            CriarCampoNumerico(
                1D,
                12D,
                CDec(linha.Espessura),
                1,
                0.5D)

        AddHandler espessura.ValueChanged,
            Sub(sender, e)

                linha.Espessura =
                    CSng(espessura.Value)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Espessura",
            espessura)

    End Sub

    Private Sub MontarPropriedadesArea(
    painel As FlowLayoutPanel,
    area As AreaTatica)

        Dim cor As ComboBox =
            CriarComboEnum(
                GetType(CorAreaTatica),
                area.Cor)

        AddHandler cor.SelectedIndexChanged,
            Sub(sender, e)

                If cor.SelectedItem Is Nothing Then
                    Exit Sub
                End If

                area.Cor =
                    CType(
                        cor.SelectedItem,
                        CorAreaTatica)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Cor",
            cor)

        Dim tracejada As New CheckBox With {
            .Text = "Borda tracejada",
            .Checked = area.Tracejada,
            .ForeColor = Tema.Texto,
            .BackColor = Tema.Painel
        }

        AddHandler tracejada.CheckedChanged,
            Sub(sender, e)

                area.Tracejada =
                    tracejada.Checked

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Estilo da borda",
            tracejada)

        Dim opacidade As NumericUpDown =
            CriarCampoNumerico(
                0D,
                255D,
                area.Opacidade)

        AddHandler opacidade.ValueChanged,
            Sub(sender, e)

                area.Opacidade =
                    CInt(opacidade.Value)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Opacidade",
            opacidade)

        Dim espessura As NumericUpDown =
            CriarCampoNumerico(
                1D,
                12D,
                CDec(area.Espessura),
                1,
                0.5D)

        AddHandler espessura.ValueChanged,
            Sub(sender, e)

                area.Espessura =
                    CSng(espessura.Value)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Espessura",
            espessura)

    End Sub

    Private Sub MontarPropriedadesMarcador(
    painel As FlowLayoutPanel,
    marcador As MarcadorTatico)

        Dim texto As New TextBox With {
            .Text = marcador.Texto,
            .MaxLength = 5,
            .BackColor = Tema.CampoEntrada,
            .ForeColor = Tema.TextoCampo,
            .BorderStyle = BorderStyle.FixedSingle
        }

        AddHandler texto.TextChanged,
            Sub(sender, e)

                marcador.Texto =
                    texto.Text

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Texto",
            texto)

        Dim cor As ComboBox =
            CriarComboEnum(
                GetType(CorMarcadorTatico),
                marcador.Cor)

        AddHandler cor.SelectedIndexChanged,
            Sub(sender, e)

                If cor.SelectedItem Is Nothing Then
                    Exit Sub
                End If

                marcador.Cor =
                    CType(
                        cor.SelectedItem,
                        CorMarcadorTatico)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Cor",
            cor)

        Dim diametro As NumericUpDown =
            CriarCampoNumerico(
                22D,
                80D,
                CDec(marcador.Diametro),
                1,
                1D)

        AddHandler diametro.ValueChanged,
            Sub(sender, e)

                marcador.Diametro =
                    CSng(diametro.Value)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Diâmetro",
            diametro)

    End Sub

    Private Sub MontarPropriedadesTexto(
    painel As FlowLayoutPanel,
    textoTatico As TextoTatico)

        Dim texto As New TextBox With {
            .Text = textoTatico.Texto,
            .BackColor = Tema.CampoEntrada,
            .ForeColor = Tema.TextoCampo,
            .BorderStyle = BorderStyle.FixedSingle
        }

        AddHandler texto.TextChanged,
            Sub(sender, e)

                textoTatico.Texto =
                    texto.Text

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Texto",
            texto)

        Dim cor As ComboBox =
            CriarComboEnum(
                GetType(CorTextoTatico),
                textoTatico.Cor)

        AddHandler cor.SelectedIndexChanged,
            Sub(sender, e)

                If cor.SelectedItem Is Nothing Then
                    Exit Sub
                End If

                textoTatico.Cor =
                    CType(
                        cor.SelectedItem,
                        CorTextoTatico)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Cor",
            cor)

        Dim tamanho As NumericUpDown =
            CriarCampoNumerico(
                8D,
                48D,
                CDec(textoTatico.TamanhoFonte),
                1,
                1D)

        AddHandler tamanho.ValueChanged,
            Sub(sender, e)

                textoTatico.TamanhoFonte =
                    CSng(tamanho.Value)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Tamanho da fonte",
            tamanho)

        Dim negrito As New CheckBox With {
            .Text = "Negrito",
            .Checked = textoTatico.Negrito,
            .ForeColor = Tema.Texto,
            .BackColor = Tema.Painel
        }

        AddHandler negrito.CheckedChanged,
            Sub(sender, e)

                textoTatico.Negrito =
                    negrito.Checked

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Estilo",
            negrito)

        Dim fundo As New CheckBox With {
            .Text = "Exibir fundo",
            .Checked = textoTatico.FundoVisivel,
            .ForeColor = Tema.Texto,
            .BackColor = Tema.Painel
        }

        AddHandler fundo.CheckedChanged,
            Sub(sender, e)

                textoTatico.FundoVisivel =
                    fundo.Checked

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Fundo",
            fundo)

    End Sub

#End Region

#Region "Seleção múltipla e bloqueio"

    Private Function CriarBotaoSelecaoMultipla(texto As String, largura As Integer, acao As Action, Optional habilitado As Boolean = True, Optional destaque As Boolean = False) As Button

        Dim botao As New Button With {
            .Text = texto,
            .Width = largura,
            .Height = 38,
            .Margin = New Padding(3),
            .FlatStyle = FlatStyle.Flat,
            .Cursor = Cursors.Hand,
            .UseVisualStyleBackColor = False,
            .Enabled = habilitado
        }

        If destaque Then

            botao.BackColor =
                Tema.CorPrimaria

            botao.ForeColor =
                Tema.TextoSobreCorPrimaria

            botao.FlatAppearance.BorderColor =
                Tema.TextoSobreCorPrimaria

        Else

            botao.BackColor =
                Tema.Painel

            botao.ForeColor =
                Tema.Texto

            botao.FlatAppearance.BorderColor =
                Tema.Borda

        End If

        botao.FlatAppearance.MouseOverBackColor =
            Tema.PainelHover

        AddHandler botao.Click,
            Sub(sender, e)

                acao.Invoke()

                CampoCanvas.Focus()

            End Sub

        Return botao

    End Function

    Private Function CriarLinhaBotoesSelecao(largura As Integer) As FlowLayoutPanel

        Return New FlowLayoutPanel With {
            .Width = largura,
            .Height = 46,
            .FlowDirection =
                FlowDirection.LeftToRight,
            .WrapContents = False,
            .Margin = New Padding(0),
            .Padding = New Padding(0),
            .BackColor = Tema.Painel
        }

    End Function

    Private Sub MontarPainelSelecaoMultipla(
    quantidade As Integer)

        LimparPainelDireito()

        Dim painel As New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False,
            .AutoScroll = True,
            .BackColor = Tema.Painel,
            .Padding = New Padding(10)
        }

        PnlDireito.Controls.Add(
            painel)

        Dim largura As Integer =
            Math.Max(
                190,
                PnlDireito.ClientSize.Width - 32)

        Dim larguraBotaoDuplo As Integer =
            Math.Max(
                85,
                (largura - 12) \ 2)

        Dim titulo As New Label With {
            .Text = "SELEÇÃO MÚLTIPLA",
            .ForeColor = Tema.CorPrimaria,
            .Font = New Font(
                "Segoe UI",
                11.0F,
                FontStyle.Bold),
            .Width = largura,
            .Height = 36,
            .TextAlign =
                ContentAlignment.MiddleLeft
        }

        painel.Controls.Add(
            titulo)

        Dim mensagem As New Label With {
            .Text =
                quantidade.ToString() &
                " objetos selecionados." &
                Environment.NewLine &
                "O último objeto selecionado é usado " &
                "como referência para o alinhamento.",
            .ForeColor = Tema.TextoSecundario,
            .Width = largura,
            .Height = 68,
            .TextAlign =
                ContentAlignment.MiddleLeft
        }

        painel.Controls.Add(
            mensagem)

        Dim tituloTamanho As New Label With {
            .Text = "TAMANHO VISUAL",
            .ForeColor = Tema.Texto,
            .Font = New Font(
                "Segoe UI",
                9.0F,
                FontStyle.Bold),
            .Width = largura,
            .Height = 28,
            .Margin = New Padding(
                0,
                8,
                0,
                2),
            .TextAlign =
                ContentAlignment.MiddleLeft
        }

        painel.Controls.Add(
            tituloTamanho)

        Dim escalaSelecao As NumericUpDown =
            CriarCampoNumerico(
                0.5D,
                2.5D,
                CDec(
                    CampoCanvas.
                        EscalaVisualReferenciaSelecao),
                1,
                0.1D)

        escalaSelecao.ThousandsSeparator =
            False

        AddHandler escalaSelecao.ValueChanged,
            Sub(sender, e)

                CampoCanvas.
                    AlterarEscalaVisualSelecionados(
                        CSng(
                            escalaSelecao.Value))

            End Sub

        AdicionarCampoPainel(
            painel,
            "Aplicar o mesmo tamanho à seleção",
            escalaSelecao)

        Dim tituloAlinhamento As New Label With {
            .Text = "ALINHAMENTO",
            .ForeColor = Tema.Texto,
            .Font = New Font(
                "Segoe UI",
                9.0F,
                FontStyle.Bold),
            .Width = largura,
            .Height = 28,
            .Margin = New Padding(
                0,
                8,
                0,
                2),
            .TextAlign =
                ContentAlignment.MiddleLeft
        }

        painel.Controls.Add(
            tituloAlinhamento)

        Dim linhaAlinhamento As FlowLayoutPanel =
            CriarLinhaBotoesSelecao(
                largura)

        linhaAlinhamento.Controls.Add(
            CriarBotaoSelecaoMultipla(
                "Mesma linha",
                larguraBotaoDuplo,
                Sub()
                    CampoCanvas.AlinharSelecaoNaMesmaLinha()
                End Sub))

        linhaAlinhamento.Controls.Add(
            CriarBotaoSelecaoMultipla(
                "Mesma coluna",
                larguraBotaoDuplo,
                Sub()
                    CampoCanvas.AlinharSelecaoNaMesmaColuna()
                End Sub))

        painel.Controls.Add(
            linhaAlinhamento)

        Dim tituloDistribuicao As New Label With {
            .Text = "DISTRIBUIÇÃO",
            .ForeColor = Tema.Texto,
            .Font = New Font(
                "Segoe UI",
                9.0F,
                FontStyle.Bold),
            .Width = largura,
            .Height = 28,
            .Margin = New Padding(
                0,
                8,
                0,
                2),
            .TextAlign =
                ContentAlignment.MiddleLeft
        }

        painel.Controls.Add(
            tituloDistribuicao)

        Dim linhaDistribuicao As FlowLayoutPanel =
            CriarLinhaBotoesSelecao(
                largura)

        linhaDistribuicao.Controls.Add(
            CriarBotaoSelecaoMultipla(
                "Horizontal",
                larguraBotaoDuplo,
                Sub()
                    CampoCanvas.DistribuirSelecaoHorizontalmente()
                End Sub,
                quantidade >= 3))

        linhaDistribuicao.Controls.Add(
            CriarBotaoSelecaoMultipla(
                "Vertical",
                larguraBotaoDuplo,
                Sub()
                    CampoCanvas.DistribuirSelecaoVerticalmente()
                End Sub,
                quantidade >= 3))

        painel.Controls.Add(
            linhaDistribuicao)

        painel.Controls.Add(
            CriarBotaoSelecaoMultipla(
                "Centralizar seleção no campo",
                largura,
                Sub()
                    CampoCanvas.CentralizarSelecaoNoCampo()
                End Sub))

        Dim tituloOrganizacao As New Label With {
    .Text = "AGRUPAMENTO E PROTEÇÃO",
    .ForeColor = Tema.Texto,
    .Font = New Font(
        "Segoe UI",
        9.0F,
        FontStyle.Bold),
    .Width = largura,
    .Height = 28,
    .Margin = New Padding(
        0,
        12,
        0,
        2),
    .TextAlign =
        ContentAlignment.MiddleLeft
}

        painel.Controls.Add(
    tituloOrganizacao)

        Dim linhaAgrupamento As FlowLayoutPanel =
    CriarLinhaBotoesSelecao(
        largura)

        linhaAgrupamento.Controls.Add(
    CriarBotaoSelecaoMultipla(
        "Agrupar",
        larguraBotaoDuplo,
        Sub()
            CampoCanvas.AgruparSelecionados()
        End Sub,
        quantidade >= 2))

        linhaAgrupamento.Controls.Add(
    CriarBotaoSelecaoMultipla(
        "Desagrupar",
        larguraBotaoDuplo,
        Sub()
            CampoCanvas.DesagruparSelecionados()
        End Sub,
        CampoCanvas.SelecaoPossuiGrupo))

        painel.Controls.Add(
    linhaAgrupamento)

        Dim textoBloqueio As String

        If CampoCanvas.SelecaoPossuiObjetoBloqueado Then

            textoBloqueio =
        "Desbloquear seleção  (Ctrl+L)"

        Else

            textoBloqueio =
        "Bloquear seleção  (Ctrl+L)"

        End If

        painel.Controls.Add(
    CriarBotaoSelecaoMultipla(
        textoBloqueio,
        largura,
        Sub()
            CampoCanvas.AlternarBloqueioSelecionados()
        End Sub))

        Dim tituloAcoes As New Label With {
            .Text = "AÇÕES",
            .ForeColor = Tema.Texto,
            .Font = New Font(
                "Segoe UI",
                9.0F,
                FontStyle.Bold),
            .Width = largura,
            .Height = 28,
            .Margin = New Padding(
                0,
                12,
                0,
                2),
            .TextAlign =
                ContentAlignment.MiddleLeft
        }

        painel.Controls.Add(
            tituloAcoes)

        painel.Controls.Add(
            CriarBotaoSelecaoMultipla(
                "Duplicar seleção  (Ctrl+D)",
                largura,
                Sub()
                    CampoCanvas.DuplicarSelecionado()
                End Sub))

        painel.Controls.Add(
            CriarBotaoSelecaoMultipla(
                "Excluir seleção",
                largura,
                Sub()
                    CampoCanvas.ExcluirSelecionado()
                End Sub,
                True,
                True))

    End Sub

    Private Sub AdicionarAcoesBloqueioIndividual(painel As FlowLayoutPanel, largura As Integer)

        Dim titulo As New Label With {
            .Text = "PROTEÇÃO",
            .ForeColor = Tema.Texto,
            .Font = New Font(
                "Segoe UI",
                9.0F,
                FontStyle.Bold),
            .Width = largura,
            .Height = 28,
            .Margin = New Padding(
                0,
                12,
                0,
                2),
            .TextAlign =
                ContentAlignment.MiddleLeft
        }

        painel.Controls.Add(
            titulo)

        Dim textoBotao As String

        If CampoCanvas.SelecaoPossuiObjetoBloqueado Then

            textoBotao =
                "Desbloquear objeto  (Ctrl+L)"

        Else

            textoBotao =
                "Bloquear objeto  (Ctrl+L)"

        End If

        Dim botao As Button =
            CriarBotaoSelecaoMultipla(
                textoBotao,
                largura,
                Sub()
                    CampoCanvas.AlternarBloqueioSelecionados()
                End Sub)

        painel.Controls.Add(
            botao)

    End Sub

#End Region

#Region "Atalhos de teclado"

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean

        If CampoCanvas Is Nothing Then

            Return MyBase.ProcessCmdKey(
            msg,
            keyData)

        End If

        Dim tecla As Keys = keyData And Keys.KeyCode

        Dim modificadores As Keys = keyData And Keys.Modifiers

        If modificadores = (Keys.Control Or Keys.Shift) AndAlso tecla = Keys.F Then

            AbrirFormacoes()

            Return True

        End If

        If modificadores = Keys.Control AndAlso tecla = Keys.E Then

            ExportarImagemCampo()

            Return True

        End If

        If modificadores = (Keys.Control Or Keys.Shift) AndAlso tecla = Keys.S Then

            SalvarExercicio(True)

            Return True

        End If

        If modificadores = Keys.Control AndAlso tecla = Keys.J Then

            CampoCanvas.AgruparSelecionados()

            CampoCanvas.Focus()

            Return True

        End If

        If modificadores = (Keys.Control Or Keys.Shift) AndAlso tecla = Keys.J Then

            CampoCanvas.DesagruparSelecionados()

            CampoCanvas.Focus()

            Return True

        End If

        If modificadores = Keys.Control AndAlso tecla = Keys.L Then

            CampoCanvas.AlternarBloqueioSelecionados()

            CampoCanvas.Focus()

            Return True

        End If

        If modificadores = Keys.Control AndAlso tecla = Keys.S Then

            SalvarExercicio()

            Return True

        End If

        If modificadores = Keys.Control AndAlso tecla = Keys.O Then

            AbrirExercicio()

            Return True

        End If

        If modificadores = (Keys.Control Or Keys.Shift) AndAlso
           tecla = Keys.O Then

            AbrirGerenciadorObjetos()

            Return True

        End If

        If modificadores = (Keys.Control Or Keys.Alt) AndAlso
           tecla = Keys.Up Then

            CampoCanvas.SubirCamadaSelecionados()

            Return True

        End If

        If modificadores = (Keys.Control Or Keys.Alt) AndAlso
           tecla = Keys.Down Then

            CampoCanvas.DescerCamadaSelecionados()

            Return True

        End If

        If modificadores = Keys.Control AndAlso tecla = Keys.N Then

            NovoExercicio_Click(
        Me,
        EventArgs.Empty)

            Return True

        End If

        If modificadores = Keys.Control AndAlso tecla = Keys.D Then

            CampoCanvas.DuplicarSelecionado()

            Return True

        End If

        If modificadores = (Keys.Control Or Keys.Shift) AndAlso tecla = Keys.Up Then

            CampoCanvas.TrazerParaFrente()

            Return True

        End If

        If modificadores = (Keys.Control Or Keys.Shift) AndAlso tecla = Keys.Down Then

            CampoCanvas.EnviarParaTras()

            Return True

        End If

        If modificadores = Keys.Control AndAlso tecla = Keys.Z Then

            CampoCanvas.Desfazer()

            Return True

        End If

        If modificadores = Keys.Control AndAlso tecla = Keys.Y Then

            CampoCanvas.Refazer()

            Return True

        End If

        If modificadores = (Keys.Control Or Keys.Shift) AndAlso tecla = Keys.Z Then

            CampoCanvas.Refazer()

            Return True

        End If

        If modificadores = Keys.Control AndAlso tecla = Keys.I Then

            AbrirConfiguracoesExercicio()

            Return True

        End If

        If modificadores = (Keys.Control Or Keys.Shift) AndAlso tecla = Keys.E Then

            ExportarExercicioPdf()

            Return True

        End If

        If modificadores = Keys.Control AndAlso tecla = Keys.P Then

            VisualizarImpressaoExercicio()

            Return True

        End If

        If modificadores = Keys.Control AndAlso tecla = Keys.A Then

            CampoCanvas.SelecionarTodos()

            CampoCanvas.Focus()

            Return True

        End If

        If modificadores = (Keys.Control Or Keys.Alt) AndAlso tecla = Keys.P Then

            AbrirPreferencias()

            Return True

        End If

        If modificadores = Keys.Control AndAlso (tecla = Keys.Oemplus OrElse tecla = Keys.Add) Then

            CampoCanvas.AumentarZoom()

            Return True

        End If

        If modificadores = Keys.Control AndAlso (tecla = Keys.OemMinus OrElse tecla = Keys.Subtract) Then

            CampoCanvas.DiminuirZoom()

            Return True

        End If

        If modificadores = Keys.Control AndAlso (tecla = Keys.D0 OrElse tecla = Keys.NumPad0) Then

            CampoCanvas.RestaurarVisualizacao()

            Return True

        End If

        If modificadores = (Keys.Control Or Keys.Shift) AndAlso tecla = Keys.G Then

            AlternarEncaixeGrade()

            CriarBarraZoom()

            Return True

        End If

        If modificadores = Keys.Control AndAlso tecla = Keys.G Then

            AlternarGrade()

            CriarBarraZoom()

            Return True

        End If

        If modificadores = (Keys.Control Or Keys.Alt) AndAlso tecla = Keys.H Then

            CampoCanvas.AlinharSelecaoNaMesmaLinha()

            Return True

        End If

        If modificadores = (Keys.Control Or Keys.Alt) AndAlso tecla = Keys.V Then

            CampoCanvas.AlinharSelecaoNaMesmaColuna()

            Return True

        End If

        If modificadores = (Keys.Control Or Keys.Shift) AndAlso tecla = Keys.H Then

            CampoCanvas.DistribuirSelecaoHorizontalmente()

            Return True

        End If

        If modificadores = (Keys.Control Or Keys.Shift) AndAlso tecla = Keys.V Then

            CampoCanvas.DistribuirSelecaoVerticalmente()

            Return True

        End If

        If modificadores = (Keys.Control Or Keys.Alt) AndAlso tecla = Keys.C Then

            CampoCanvas.CentralizarSelecaoNoCampo()

            Return True

        End If

        If modificadores = (Keys.Control Or Keys.Shift) AndAlso tecla = Keys.B Then

            AbrirBibliotecaExercicios()

            Return True

        End If

        Return MyBase.ProcessCmdKey(msg, keyData)

    End Function

#End Region

#Region "Barra de arquivos"

    Private Sub CriarBarraArquivo()

        PnlSuperior.SuspendLayout()

        Try

            PnlSuperior.Controls.Clear()

            _botoesAbasRibbon.Clear()

            PnlSuperior.Height = 118

            PnlSuperior.Padding =
            New Padding(0)

            PnlSuperior.BackColor =
            Tema.CorPrimaria

            Dim estruturaRibbon As New TableLayoutPanel With {
            .Name = "PnlRibbonPrincipal",
            .Dock = DockStyle.Fill,
            .ColumnCount = 1,
            .RowCount = 2,
            .Margin = New Padding(0),
            .Padding = New Padding(0),
            .BackColor = Tema.CorPrimaria
        }

            estruturaRibbon.RowStyles.Add(New RowStyle(SizeType.Absolute, 34.0F))

            estruturaRibbon.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

            '==================================================
            ' ABAS
            '==================================================

            Dim painelAbas As New FlowLayoutPanel With {
            .Name = "PnlAbasRibbon",
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = False,
            .AutoScroll = False,
            .Margin = New Padding(0),
            .Padding = New Padding(10, 2, 10, 2),
            .BackColor = Tema.CorPrimaria
        }

            Dim marca As New Label With {
            .Text = "TACTICAL STUDIO",
            .Width = 190,
            .Height = 32,
            .Margin = New Padding(0, 0, 16, 0),
            .ForeColor = Tema.TextoSobreCorPrimaria,
            .BackColor = Tema.CorPrimaria,
            .Font = New Font(
                "Segoe UI",
                10.0F,
                FontStyle.Bold),
            .TextAlign = ContentAlignment.MiddleLeft
        }

            painelAbas.Controls.Add(
            marca)

            painelAbas.Controls.Add(
            CriarAbaRibbon(
                "Arquivo"))

            painelAbas.Controls.Add(
            CriarAbaRibbon(
                "Exercício"))

            painelAbas.Controls.Add(
            CriarAbaRibbon(
                "Visualização"))

            painelAbas.Controls.Add(
            CriarAbaRibbon(
                "Ajuda"))

            estruturaRibbon.Controls.Add(
            painelAbas,
            0,
            0)

            '==================================================
            ' CONTEÚDO DA ABA
            '==================================================

            _painelConteudoRibbon =
            New FlowLayoutPanel With {
                .Name = "PnlConteudoRibbon",
                .Dock = DockStyle.Fill,
                .FlowDirection = FlowDirection.LeftToRight,
                .WrapContents = False,
                .AutoScroll = True,
                .Margin = New Padding(0),
                .Padding = New Padding(8, 4, 8, 4),
                .BackColor = Tema.Fundo
            }

            estruturaRibbon.Controls.Add(
            _painelConteudoRibbon,
            0,
            1)

            PnlSuperior.Controls.Add(
            estruturaRibbon)

            ExibirAbaRibbon(
            _abaRibbonAtual)

        Finally

            PnlSuperior.ResumeLayout(
            True)

        End Try

    End Sub



    Private Sub CampoInteiro_Click(
        sender As Object,
        e As EventArgs)

        If CampoCanvas Is Nothing Then
            Exit Sub
        End If

        If CampoCanvas.ModoSelecaoRecorteAtivo Then

            CampoCanvas.CancelarSelecaoRecorteCampo()

        End If

        If CampoCanvas.RecorteAtivo Then

            CampoCanvas.LimparRecorteCampo()

        Else

            CampoCanvas.RestaurarVisualizacao()

        End If

        MontarPainelRecorteCampo()

        CampoCanvas.Focus()

    End Sub

    Private Function CriarAbaRibbon(
    nomeAba As String) As Button

        Dim largura As Integer

        Select Case nomeAba

            Case "Arquivo"

                largura =
                100

            Case "Exercício"

                largura =
                110

            Case "Visualização"

                largura =
                135

            Case "Ajuda"

                largura =
                90

            Case Else

                largura =
                105

        End Select

        Dim botao As New Button With {
        .Text = nomeAba,
        .Tag = nomeAba,
        .Width = largura,
        .Height = 32,
        .Margin = New Padding(0, 0, 4, 0),
        .FlatStyle = FlatStyle.Flat,
        .BackColor = Tema.CorPrimaria,
        .ForeColor = Tema.TextoSobreCorPrimaria,
        .Cursor = Cursors.Hand,
        .UseVisualStyleBackColor = False,
        .TextAlign = ContentAlignment.MiddleCenter
    }

        botao.FlatAppearance.BorderSize =
        1

        botao.FlatAppearance.BorderColor =
        Tema.CorPrimaria

        botao.FlatAppearance.MouseOverBackColor =
        Tema.PainelHover

        botao.FlatAppearance.MouseDownBackColor =
        Tema.Painel

        AddHandler botao.Click,
        AddressOf AbaRibbon_Click

        _botoesAbasRibbon(
        nomeAba) =
        botao

        Return botao

    End Function

    Private Sub AbaRibbon_Click(
    sender As Object,
    e As EventArgs)

        Dim botao As Button =
        TryCast(
            sender,
            Button)

        If botao Is Nothing OrElse
       botao.Tag Is Nothing Then

            Exit Sub

        End If

        Dim nomeAba As String =
        CStr(
            botao.Tag)

        ExibirAbaRibbon(
        nomeAba)

    End Sub

    Private Sub ExibirAbaRibbon(
    nomeAba As String)

        If _painelConteudoRibbon Is Nothing OrElse
       _painelConteudoRibbon.IsDisposed Then

            Exit Sub

        End If

        _abaRibbonAtual =
        nomeAba

        _painelConteudoRibbon.SuspendLayout()

        Try

            _painelConteudoRibbon.Controls.Clear()

            Select Case nomeAba

                Case "Arquivo"

                    _painelConteudoRibbon.Controls.Add(
                    CriarGrupoRibbon(
                        "ARQUIVO",
                        CriarBotaoRibbon(
                            "Novo",
                            AddressOf NovoExercicio_Click),
                        CriarBotaoRibbon(
                            "Abrir",
                            AddressOf AbrirExercicio_Click),
                        CriarBotaoRibbon(
                            "Salvar",
                            AddressOf SalvarExercicio_Click)))

                    _painelConteudoRibbon.Controls.Add(
                    CriarGrupoRibbon(
                        "EXPORTAÇÃO",
                        CriarBotaoRibbon(
                            "Exportar",
                            AddressOf ExportarImagem_Click,
                            96),
                        CriarBotaoRibbon(
                            "PDF",
                            AddressOf ExportarPdf_Click),
                        CriarBotaoRibbon(
                            "Imprimir",
                            AddressOf ImprimirExercicio_Click,
                            96)))

                Case "Exercício"

                    _painelConteudoRibbon.Controls.Add(
                    CriarGrupoRibbon(
                        "CONFIGURAÇÃO",
                        CriarBotaoRibbon(
                            "Dados",
                            AddressOf ConfiguracoesExercicio_Click),
                        CriarBotaoRibbon(
                            "Formações",
                            AddressOf Formacoes_Click,
                            105)))

                    _painelConteudoRibbon.Controls.Add(
                    CriarGrupoRibbon(
                        "ORGANIZAÇÃO",
                        CriarBotaoRibbon(
                            "Biblioteca",
                            AddressOf Biblioteca_Click,
                            105),
                        CriarBotaoRibbon(
                            "Objetos",
                            AddressOf Objetos_Click)))

                Case "Visualização"

                    _painelConteudoRibbon.Controls.Add(
                    CriarGrupoRibbon(
                        "CAMPO",
                        CriarBotaoRibbon(
                            "Ajustes Campo",
                            AddressOf CampoInteiro_Click,
                            120)))

                    _painelConteudoRibbon.Controls.Add(
                    CriarGrupoRibbon(
                        "APLICAÇÃO",
                        CriarBotaoRibbon(
                            "Opções",
                            AddressOf Preferencias_Click,
                            96)))

                Case "Ajuda"

                    _painelConteudoRibbon.Controls.Add(
                    CriarGrupoRibbon(
                        "INFORMAÇÕES",
                        CriarBotaoRibbon(
                            "Sobre",
                            AddressOf Sobre_Click,
                            96)))

                Case Else

                    _abaRibbonAtual =
                    "Arquivo"

                    ExibirAbaRibbon(
                    "Arquivo")

                    Exit Sub

            End Select

        Finally

            _painelConteudoRibbon.ResumeLayout(
            True)

        End Try

        AtualizarAbasRibbon()

    End Sub

    Private Function CriarGrupoRibbon(
    titulo As String,
    ParamArray botoes() As Button
) As Control

        Dim larguraConteudo As Integer =
        20

        For Each botao As Button
        In botoes

            If botao Is Nothing Then
                Continue For
            End If

            larguraConteudo +=
            botao.Width +
            botao.Margin.Horizontal

        Next

        Dim larguraGrupo As Integer =
        Math.Max(
            155,
            larguraConteudo)

        Dim grupo As New TableLayoutPanel With {
        .Width = larguraGrupo,
        .Height = 76,
        .ColumnCount = 1,
        .RowCount = 2,
        .Margin = New Padding(
            5,
            0,
            0,
            0),
        .Padding = New Padding(
            5,
            3,
            5,
            3),
        .BackColor = Tema.Painel,
        .BorderStyle = BorderStyle.FixedSingle,
        .GrowStyle = TableLayoutPanelGrowStyle.FixedSize
    }

        grupo.ColumnStyles.Add(
        New ColumnStyle(
            SizeType.Percent,
            100.0F))

        grupo.RowStyles.Add(
        New RowStyle(
            SizeType.Percent,
            100.0F))

        grupo.RowStyles.Add(
        New RowStyle(
            SizeType.Absolute,
            18.0F))

        '==================================================
        ' BOTÕES
        '==================================================

        Dim quantidadeBotoes As Integer =
        0

        For Each botao As Button
        In botoes

            If botao IsNot Nothing Then

                quantidadeBotoes +=
                1

            End If

        Next

        If quantidadeBotoes = 0 Then

            quantidadeBotoes =
            1

        End If

        Dim painelBotoes As New TableLayoutPanel With {
        .Dock = DockStyle.Fill,
        .ColumnCount = quantidadeBotoes,
        .RowCount = 1,
        .Margin = New Padding(0),
        .Padding = New Padding(4, 1, 4, 1),
        .BackColor = Tema.Painel,
        .GrowStyle = TableLayoutPanelGrowStyle.FixedSize
    }

        painelBotoes.RowStyles.Add(
        New RowStyle(
            SizeType.Percent,
            100.0F))

        For indice As Integer =
        0 To quantidadeBotoes - 1

            painelBotoes.ColumnStyles.Add(
            New ColumnStyle(
                SizeType.Percent,
                100.0F /
                quantidadeBotoes))

        Next

        Dim colunaAtual As Integer =
        0

        For Each botao As Button
        In botoes

            If botao Is Nothing Then
                Continue For
            End If

            botao.Anchor =
            AnchorStyles.None

            botao.Margin =
            New Padding(4)

            painelBotoes.Controls.Add(
            botao,
            colunaAtual,
            0)

            colunaAtual +=
            1

        Next

        grupo.Controls.Add(
        painelBotoes,
        0,
        0)

        '==================================================
        ' TÍTULO DO GRUPO
        '==================================================

        Dim labelGrupo As New Label With {
        .Text = titulo,
        .Dock = DockStyle.Fill,
        .AutoSize = False,
        .Margin = New Padding(0),
        .Padding = New Padding(0),
        .ForeColor = Tema.TextoSecundario,
        .BackColor = Tema.Painel,
        .Font = New Font(
            "Segoe UI",
            7.5F,
            FontStyle.Regular),
        .TextAlign = ContentAlignment.MiddleCenter
    }

        grupo.Controls.Add(
        labelGrupo,
        0,
        1)

        Return grupo

    End Function

    Private Function CriarBotaoRibbon(
    texto As String,
    acao As EventHandler,
    Optional largura As Integer = 86
) As Button

        Dim botao As New Button With {
        .Text = texto,
        .Width = largura,
        .Height = 43,
        .Margin = New Padding(3, 0, 3, 0),
        .FlatStyle = FlatStyle.Flat,
        .BackColor = Tema.Painel,
        .ForeColor = Tema.Texto,
        .Cursor = Cursors.Hand,
        .UseVisualStyleBackColor = False,
        .TextAlign = ContentAlignment.MiddleCenter
    }

        botao.FlatAppearance.BorderSize =
        1

        botao.FlatAppearance.BorderColor =
        Tema.Borda

        botao.FlatAppearance.MouseOverBackColor =
        Tema.PainelHover

        botao.FlatAppearance.MouseDownBackColor =
        Tema.CorPrimaria

        AddHandler botao.Click,
        acao

        Return botao

    End Function

    Private Sub AtualizarAbasRibbon()

        For Each item As KeyValuePair(Of String, Button)
        In _botoesAbasRibbon

            Dim botao As Button =
            item.Value

            Dim selecionada As Boolean =
            String.Equals(
                item.Key,
                _abaRibbonAtual,
                StringComparison.OrdinalIgnoreCase)

            If selecionada Then

                botao.BackColor =
                Tema.Fundo

                botao.ForeColor =
                Tema.Texto

                botao.FlatAppearance.BorderColor =
                Tema.CorPrimaria

            Else

                botao.BackColor =
                Tema.CorPrimaria

                botao.ForeColor =
                Tema.TextoSobreCorPrimaria

                botao.FlatAppearance.BorderColor =
                Tema.CorPrimaria

            End If

        Next

    End Sub

    Private Sub Sobre_Click(sender As Object, e As EventArgs)

        Using formulario As New FrmSobre()

            formulario.ShowDialog(Me)

        End Using

        CampoCanvas.Focus()

    End Sub

#End Region

#Region "Formações táticas"

    Private Sub Formacoes_Click(
    sender As Object,
    e As EventArgs)

        AbrirFormacoes()

    End Sub

    Private Sub AbrirFormacoes()

        If CampoCanvas Is Nothing Then
            Exit Sub
        End If

        Using formulario As New FrmFormacoes(
            CampoCanvas)

            Dim resultado As DialogResult =
                formulario.ShowDialog(
                    Me)

            If resultado <>
               DialogResult.OK Then

                CampoCanvas.Focus()

                Exit Sub

            End If

            Dim formacaoMeuTime As ModeloFormacao =
                Nothing

            Dim formacaoAdversario As ModeloFormacao =
                Nothing

            If formulario.AplicarMeuTime Then

                formacaoMeuTime =
                    formulario.FormacaoMeuTimeSelecionada

            End If

            If formulario.AplicarAdversario Then

                formacaoAdversario =
                    formulario.FormacaoAdversarioSelecionada

            End If

            If formacaoMeuTime Is Nothing AndAlso
               formacaoAdversario Is Nothing Then

                MessageBox.Show(
                    "Nenhuma formação foi selecionada.",
                    "Formações táticas",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information)

                CampoCanvas.Focus()

                Exit Sub

            End If

            Dim possuiJogadores As Boolean =
                CampoPossuiJogadores()

            Dim substituirJogadores As Boolean =
                True

            If possuiJogadores Then

                Dim resposta As DialogResult =
                    MessageBox.Show(
                        "O campo já possui jogadores." &
                        Environment.NewLine &
                        Environment.NewLine &
                        "Sim: substituir todos os jogadores existentes." &
                        Environment.NewLine &
                        "Não: manter os jogadores e adicionar as formações." &
                        Environment.NewLine &
                        "Cancelar: não aplicar as formações.",
                        "Aplicar formações defensivas",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question)

                If resposta =
                   DialogResult.Cancel Then

                    CampoCanvas.Focus()

                    Exit Sub

                End If

                substituirJogadores =
                    resposta = DialogResult.Yes

            End If

            Dim jogadoresCriados As IReadOnlyList(Of Jogador) =
                CampoCanvas.AplicarFormacoesDefensivas(
                    formacaoMeuTime,
                    formacaoAdversario,
                    substituirJogadores)

            If jogadoresCriados Is Nothing OrElse
               jogadoresCriados.Count = 0 Then

                MessageBox.Show(
                    "Não foi possível adicionar os jogadores das formações.",
                    "Formações táticas",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning)

                CampoCanvas.Focus()

                Exit Sub

            End If

            Dim equipesAplicadas As New List(Of String)()

            If formacaoMeuTime IsNot Nothing Then

                equipesAplicadas.Add(
                    "Meu time: " &
                    formacaoMeuTime.Nome)

            End If

            If formacaoAdversario IsNot Nothing Then

                equipesAplicadas.Add(
                    "Adversário: " &
                    formacaoAdversario.Nome)

            End If

            MessageBox.Show(
                String.Join(
                    Environment.NewLine,
                    equipesAplicadas) &
                Environment.NewLine &
                Environment.NewLine &
                jogadoresCriados.Count.ToString() &
                " jogadores foram posicionados.",
                "Formações táticas",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information)

            CampoCanvas.Focus()

        End Using

    End Sub

    Private Function CampoPossuiJogadores() As Boolean

        If CampoCanvas Is Nothing Then
            Return False
        End If

        For Each objeto As ObjetoCampo
        In CampoCanvas.ObjetosAtuais

            If TypeOf objeto Is Jogador Then
                Return True
            End If

        Next

        Return False

    End Function

#End Region

#Region "Gerenciador de objetos"

    Private Sub Objetos_Click(
    sender As Object,
    e As EventArgs)

        AbrirGerenciadorObjetos()

    End Sub

    Private Sub AbrirGerenciadorObjetos()

        If CampoCanvas Is Nothing Then
            Exit Sub
        End If

        If _frmGerenciadorObjetos Is Nothing OrElse
           _frmGerenciadorObjetos.IsDisposed Then

            _frmGerenciadorObjetos =
                New FrmGerenciadorObjetos(
                    CampoCanvas)

            AddHandler _frmGerenciadorObjetos.FormClosed,
                Sub(sender, e)
                    _frmGerenciadorObjetos = Nothing
                End Sub

            _frmGerenciadorObjetos.Show(
                Me)

        Else

            _frmGerenciadorObjetos.AplicarTemaAtual()
            _frmGerenciadorObjetos.AtualizarConteudo()
            _frmGerenciadorObjetos.BringToFront()
            _frmGerenciadorObjetos.Activate()

        End If

    End Sub

#End Region

#Region "Biblioteca de exercícios"

    Private Sub Biblioteca_Click(
    sender As Object,
    e As EventArgs)

        AbrirBibliotecaExercicios()

    End Sub

    Private Sub AbrirBibliotecaExercicios()

        If CampoCanvas Is Nothing Then
            Exit Sub
        End If

        Using formulario As New FrmBibliotecaExercicios()

            AddHandler formulario.AdicionarAtualSolicitado,
            Sub()

                Dim itemAdicionado As ItemBibliotecaExercicio =
                    AdicionarExercicioAtualBiblioteca(
                        formulario)

                If itemAdicionado IsNot Nothing Then

                    formulario.CarregarBiblioteca(
                        itemAdicionado.Id)

                End If

            End Sub

            Dim resultado As DialogResult =
            formulario.ShowDialog(
                Me)

            If resultado <> DialogResult.OK Then

                CampoCanvas.Focus()

                Exit Sub

            End If

            Dim itemSelecionado As ItemBibliotecaExercicio =
            formulario.ItemSelecionado

            If itemSelecionado Is Nothing Then

                CampoCanvas.Focus()

                Exit Sub

            End If

            Dim caminhoExercicio As String =
            RepositorioBibliotecaExercicios.
                ObterCaminhoExercicio(
                    itemSelecionado)

            If String.IsNullOrWhiteSpace(
            caminhoExercicio) OrElse
           Not File.Exists(
               caminhoExercicio) Then

                MessageBox.Show(
                "O arquivo do exercício selecionado não foi encontrado.",
                "Biblioteca de exercícios",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning)

                CampoCanvas.Focus()

                Exit Sub

            End If

            If Not ConfirmarAlteracoesNaoSalvas() Then

                CampoCanvas.Focus()

                Exit Sub

            End If

            CarregarExercicioDoCaminho(
            caminhoExercicio,
            False)

        End Using

    End Sub

    Private Function SolicitarDadosBiblioteca(
    ByRef nome As String,
    ByRef descricao As String,
    ByRef categoria As String,
    ByRef favorito As Boolean,
    Optional proprietario As IWin32Window = Nothing
) As Boolean

        Using formulario As New Form()

            formulario.Text =
            "Adicionar à biblioteca"

            formulario.StartPosition =
            FormStartPosition.CenterParent

            formulario.FormBorderStyle =
            FormBorderStyle.FixedDialog

            formulario.MaximizeBox =
            False

            formulario.MinimizeBox =
            False

            formulario.ShowInTaskbar =
            False

            formulario.ClientSize =
            New Size(
                520,
                350)

            formulario.BackColor =
            Tema.Fundo

            formulario.ForeColor =
            Tema.Texto

            Dim painel As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 1,
            .RowCount = 8,
            .Padding = New Padding(14),
            .Margin = New Padding(0)
        }

            painel.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                26.0F))

            painel.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                38.0F))

            painel.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                26.0F))

            painel.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                38.0F))

            painel.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                26.0F))

            painel.RowStyles.Add(
            New RowStyle(
                SizeType.Percent,
                100.0F))

            painel.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                38.0F))

            painel.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                48.0F))

            formulario.Controls.Add(
            painel)

            Dim labelNome As New Label With {
            .Text = "Nome do exercício:",
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleLeft
        }

            painel.Controls.Add(
            labelNome,
            0,
            0)

            Dim txtNome As New TextBox With {
            .Dock = DockStyle.Fill,
            .MaxLength = 100,
            .Text = nome,
            .BackColor = Tema.CampoEntrada,
            .ForeColor = Tema.TextoCampo
        }

            painel.Controls.Add(
            txtNome,
            0,
            1)

            Dim labelCategoria As New Label With {
            .Text = "Categoria:",
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleLeft
        }

            painel.Controls.Add(
            labelCategoria,
            0,
            2)

            Dim txtCategoria As New TextBox With {
            .Dock = DockStyle.Fill,
            .MaxLength = 80,
            .Text = categoria,
            .BackColor = Tema.CampoEntrada,
            .ForeColor = Tema.TextoCampo
        }

            painel.Controls.Add(
            txtCategoria,
            0,
            3)

            Dim labelDescricao As New Label With {
            .Text = "Descrição:",
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleLeft
        }

            painel.Controls.Add(
            labelDescricao,
            0,
            4)

            Dim txtDescricao As New TextBox With {
            .Dock = DockStyle.Fill,
            .Multiline = True,
            .ScrollBars = ScrollBars.Vertical,
            .MaxLength = 500,
            .Text = descricao,
            .BackColor = Tema.CampoEntrada,
            .ForeColor = Tema.TextoCampo
        }

            painel.Controls.Add(
            txtDescricao,
            0,
            5)

            Dim chkFavorito As New CheckBox With {
            .Text = "Adicionar como favorito",
            .Dock = DockStyle.Fill,
            .Checked = favorito,
            .ForeColor = Tema.Texto,
            .BackColor = Tema.Fundo
        }

            painel.Controls.Add(
            chkFavorito,
            0,
            6)

            Dim painelBotoes As New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.RightToLeft,
            .WrapContents = False,
            .Padding = New Padding(
                0,
                8,
                0,
                0),
            .Margin = New Padding(0)
        }

            Dim btnAdicionar As New Button With {
            .Text = "Adicionar",
            .Width = 115,
            .Height = 32,
            .DialogResult = DialogResult.OK,
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Tema.Painel,
            .ForeColor = Tema.Texto,
            .Cursor = Cursors.Hand
        }

            btnAdicionar.FlatAppearance.BorderColor =
            Tema.Borda

            btnAdicionar.FlatAppearance.MouseOverBackColor =
            Tema.PainelHover

            Dim btnCancelar As New Button With {
            .Text = "Cancelar",
            .Width = 110,
            .Height = 32,
            .DialogResult = DialogResult.Cancel,
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Tema.Painel,
            .ForeColor = Tema.Texto,
            .Cursor = Cursors.Hand
        }

            btnCancelar.FlatAppearance.BorderColor =
            Tema.Borda

            btnCancelar.FlatAppearance.MouseOverBackColor =
            Tema.PainelHover

            painelBotoes.Controls.Add(
            btnAdicionar)

            painelBotoes.Controls.Add(
            btnCancelar)

            painel.Controls.Add(
            painelBotoes,
            0,
            7)

            formulario.AcceptButton =
            btnAdicionar

            formulario.CancelButton =
            btnCancelar

            txtNome.SelectAll()

            Dim resultado As DialogResult

            If proprietario Is Nothing Then

                resultado =
                formulario.ShowDialog(
                    Me)

            Else

                resultado =
                formulario.ShowDialog(
                    proprietario)

            End If

            If resultado <> DialogResult.OK Then
                Return False
            End If

            nome =
            txtNome.Text.Trim()

            descricao =
            txtDescricao.Text.Trim()

            categoria =
            txtCategoria.Text.Trim()

            favorito =
            chkFavorito.Checked

            Return True

        End Using

    End Function

    Private Function AdicionarExercicioAtualBiblioteca(
    Optional proprietario As IWin32Window = Nothing
) As ItemBibliotecaExercicio

        If CampoCanvas Is Nothing Then
            Return Nothing
        End If

        If CampoCanvas.ObjetosAtuais.Count = 0 Then

            MessageBox.Show(
            proprietario,
            "O campo não possui objetos para adicionar à biblioteca.",
            "Biblioteca de exercícios",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information)

            Return Nothing

        End If

        Dim nome As String =
        _nomeExercicioAtual

        Dim descricao As String =
        _descricaoExercicioAtual

        Dim categoria As String =
        _categoriaExercicioAtual

        Dim favorito As Boolean =
        False

        If Not SolicitarDadosBiblioteca(
        nome,
        descricao,
        categoria,
        favorito,
        proprietario) Then

            Return Nothing

        End If

        Dim caminhoTemporario As String =
        Path.Combine(
            Path.GetTempPath(),
            "TacticalStudio-" &
            Guid.NewGuid().
                ToString("N") &
            ".tactical")

        Try

            Dim conteudoJson As String =
            CampoCanvas.ExportarExercicioJson(
                nome,
                categoria,
                _duracaoExercicioAtual,
                descricao,
                _observacoesExercicioAtual)

            File.WriteAllText(
            caminhoTemporario,
            conteudoJson,
            New UTF8Encoding(False))

            Dim itemAdicionado As ItemBibliotecaExercicio =
            RepositorioBibliotecaExercicios.AdicionarArquivo(
                caminhoTemporario,
                nome,
                descricao,
                categoria,
                favorito)

            If itemAdicionado Is Nothing Then

                MessageBox.Show(
        proprietario,
        "Não foi possível adicionar o exercício à biblioteca.",
        "Biblioteca de exercícios",
        MessageBoxButtons.OK,
        MessageBoxIcon.Error)

                Return Nothing

            End If

            Dim miniaturaSalva As Boolean =
    SalvarMiniaturaBiblioteca(
        itemAdicionado)

            If miniaturaSalva Then

                MessageBox.Show(
        proprietario,
        "O exercício foi adicionado à biblioteca com sucesso." &
        Environment.NewLine &
        Environment.NewLine &
        "A miniatura de pré-visualização também foi criada.",
        "Biblioteca de exercícios",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information)

            Else

                MessageBox.Show(
        proprietario,
        "O exercício foi adicionado à biblioteca com sucesso." &
        Environment.NewLine &
        Environment.NewLine &
        "Porém, não foi possível criar a miniatura de pré-visualização.",
        "Biblioteca de exercícios",
        MessageBoxButtons.OK,
        MessageBoxIcon.Warning)

            End If

            Return itemAdicionado

        Catch ex As Exception

            MessageBox.Show(
            proprietario,
            "Não foi possível adicionar o exercício à biblioteca." &
            Environment.NewLine &
            Environment.NewLine &
            ex.Message,
            "Biblioteca de exercícios",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error)

            Return Nothing

        Finally

            Try

                If File.Exists(
                caminhoTemporario) Then

                    File.Delete(
                    caminhoTemporario)

                End If

            Catch

            End Try

        End Try

    End Function

    Private Function SalvarMiniaturaBiblioteca(
    item As ItemBibliotecaExercicio
) As Boolean

        If CampoCanvas Is Nothing OrElse
       item Is Nothing OrElse
       String.IsNullOrWhiteSpace(
           item.Id) Then

            Return False

        End If

        Try

            Using miniatura As Bitmap =
            CampoCanvas.GerarMiniaturaCampo(
                640)

                If miniatura Is Nothing Then
                    Return False
                End If

                Return RepositorioBibliotecaExercicios.
                SalvarMiniatura(
                    item.Id,
                    miniatura)

            End Using

        Catch

            Return False

        End Try

    End Function

#End Region

#Region "Arquivos do exercício"

    Private Sub NovoExercicio_Click(
    sender As Object,
    e As EventArgs)

        If Not ConfirmarAlteracoesNaoSalvas() Then
            Exit Sub
        End If

        CampoCanvas.NovoExercicio()

        _caminhoArquivoAtual =
        String.Empty

        _nomeExercicioAtual = "Novo exercício"

        _categoriaExercicioAtual = "Tático"

        _duracaoExercicioAtual = 30

        _descricaoExercicioAtual = String.Empty

        _observacoesExercicioAtual = String.Empty

        MarcarComoSalvo()

        ExcluirArquivoRecuperacao()

        CampoCanvas.Focus()

    End Sub

    Private Sub AbrirExercicio_Click(
        sender As Object,
        e As EventArgs)

        AbrirExercicio()

    End Sub

    Private Sub SalvarExercicio_Click(
        sender As Object,
        e As EventArgs)

        SalvarExercicio()

    End Sub

    Private Sub AbrirExercicio()

        If Not ConfirmarAlteracoesNaoSalvas() Then
            Exit Sub
        End If

        Using dialogo As New OpenFileDialog()

            dialogo.Title =
            "Abrir exercício tático"

            dialogo.Filter =
            "Exercício TacticalStudio (*.tactical)|*.tactical|" &
            "Arquivo JSON (*.json)|*.json|" &
            "Todos os arquivos (*.*)|*.*"

            dialogo.Multiselect =
            False

            If dialogo.ShowDialog(Me) <>
           DialogResult.OK Then

                Exit Sub

            End If

            CarregarExercicioDoCaminho(
            dialogo.FileName,
            True)

        End Using

    End Sub

    Private Function SalvarExercicio(Optional salvarComo As Boolean = False) As Boolean

        Dim caminhoDestino As String =
        _caminhoArquivoAtual

        If salvarComo OrElse
       String.IsNullOrWhiteSpace(
           caminhoDestino) Then

            Using dialogo As New SaveFileDialog()

                dialogo.Title =
                "Salvar exercício tático"

                dialogo.Filter =
                "Exercício TacticalStudio (*.tactical)|*.tactical|" &
                "Arquivo JSON (*.json)|*.json"

                dialogo.DefaultExt =
                "tactical"

                dialogo.AddExtension =
                True

                dialogo.FileName =
                _nomeExercicioAtual &
                ".tactical"

                If dialogo.ShowDialog() <> DialogResult.OK Then

                    Return False

                End If

                caminhoDestino = dialogo.FileName

            End Using

        End If

        Try

            Dim conteudoJson As String = CampoCanvas.ExportarExercicioJson(_nomeExercicioAtual, _categoriaExercicioAtual, _duracaoExercicioAtual, _descricaoExercicioAtual, _observacoesExercicioAtual)

            File.WriteAllText(caminhoDestino, conteudoJson, New UTF8Encoding(False))

            _caminhoArquivoAtual = caminhoDestino

            MarcarComoSalvo()

            ExcluirArquivoRecuperacao()

            Return True

        Catch ex As Exception

            MessageBox.Show(
            "Não foi possível salvar o exercício." &
            Environment.NewLine &
            Environment.NewLine &
            ex.Message,
            "Erro ao salvar",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error)

            Return False

        End Try

    End Function

    Private Function CarregarExercicioDoCaminho(
    caminhoArquivo As String,
    Optional usarComoArquivoAtual As Boolean = True
) As Boolean

        If CampoCanvas Is Nothing OrElse
       String.IsNullOrWhiteSpace(
           caminhoArquivo) OrElse
       Not File.Exists(
           caminhoArquivo) Then

            Return False

        End If

        Try

            Dim conteudoJson As String =
            File.ReadAllText(
                caminhoArquivo,
                Encoding.UTF8)

            Dim arquivo As ArquivoExercicio =
            CampoCanvas.ImportarExercicioJson(
                conteudoJson)

            If usarComoArquivoAtual Then

                _caminhoArquivoAtual =
                caminhoArquivo

            Else

                _caminhoArquivoAtual =
                String.Empty

            End If

            If String.IsNullOrWhiteSpace(
            arquivo.Nome) Then

                _nomeExercicioAtual =
                Path.GetFileNameWithoutExtension(
                    caminhoArquivo)

            Else

                _nomeExercicioAtual =
                arquivo.Nome

            End If

            _categoriaExercicioAtual =
            If(
                String.IsNullOrWhiteSpace(
                    arquivo.Categoria),
                "Tático",
                arquivo.Categoria)

            _duracaoExercicioAtual =
            Math.Max(
                1,
                arquivo.DuracaoMinutos)

            _descricaoExercicioAtual =
            If(
                arquivo.Descricao,
                String.Empty)

            _observacoesExercicioAtual =
            If(
                arquivo.Observacoes,
                String.Empty)

            MarcarComoSalvo()

            ExcluirArquivoRecuperacao()

            CampoCanvas.Focus()

            Return True

        Catch ex As Exception

            MessageBox.Show(
            "Não foi possível abrir o exercício." &
            Environment.NewLine &
            Environment.NewLine &
            ex.Message,
            "Erro ao abrir",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error)

            Return False

        End Try

    End Function

#End Region

#Region "Exportação de imagem"

    Private Sub ExportarImagem_Click(
    sender As Object,
    e As EventArgs)

        ExportarImagemCampo()

    End Sub

    Private Sub ExportarImagemCampo()

        If CampoCanvas Is Nothing Then
            Exit Sub
        End If

        Dim respostaCabecalho As DialogResult =
        MessageBox.Show(
            "Deseja incluir os dados do exercício na imagem?" &
            Environment.NewLine &
            Environment.NewLine &
            "Sim: exporta com cabeçalho." &
            Environment.NewLine &
            "Não: exporta somente o campo.",
            "Exportar imagem",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Question)

        If respostaCabecalho =
       DialogResult.Cancel Then

            CampoCanvas.Focus()

            Exit Sub

        End If

        Dim incluirCabecalho As Boolean =
        respostaCabecalho =
        DialogResult.Yes

        Using dialogo As New SaveFileDialog()

            dialogo.Title =
            "Exportar campo como imagem"

            dialogo.Filter =
            "Imagem PNG (*.png)|*.png"

            dialogo.DefaultExt =
            "png"

            dialogo.AddExtension =
            True

            dialogo.OverwritePrompt =
            True

            dialogo.FileName =
            _nomeExercicioAtual &
            ".png"

            If dialogo.ShowDialog() <>
           DialogResult.OK Then

                CampoCanvas.Focus()

                Exit Sub

            End If

            Try

                Using imagem As Bitmap =
                CampoCanvas.GerarImagemCampo(
                    _preferencias.ResolucaoExportacao,
                    incluirCabecalho,
                    _nomeExercicioAtual,
                    _categoriaExercicioAtual,
                    _duracaoExercicioAtual,
                    _descricaoExercicioAtual,
                    _observacoesExercicioAtual)

                    imagem.Save(
                    dialogo.FileName,
                    ImageFormat.Png)

                End Using

                MessageBox.Show(
                "Imagem exportada com sucesso." &
                Environment.NewLine &
                Environment.NewLine &
                dialogo.FileName,
                "Exportação concluída",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information)

            Catch ex As Exception

                MessageBox.Show(
                "Não foi possível exportar a imagem." &
                Environment.NewLine &
                Environment.NewLine &
                ex.Message,
                "Erro na exportação",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error)

            Finally

                CampoCanvas.Focus()

            End Try

        End Using

    End Sub

#End Region

#Region "Controle de alterações"

    Private Sub AtualizarTituloJanela()

        Dim indicadorAlteracao As String =
        String.Empty

        If _alteracoesNaoSalvas Then

            indicadorAlteracao =
            " *"

        End If

        Text =
        "TacticalStudio - " &
        _nomeExercicioAtual &
        indicadorAlteracao

    End Sub

    Private Sub CampoCanvas_HistoricoAlterado(
    podeDesfazer As Boolean,
    podeRefazer As Boolean)

        AtualizarEstadoAlteracoes()

    End Sub

    Private Sub MarcarComoSalvo()

        If CampoCanvas Is Nothing Then
            Exit Sub
        End If

        _assinaturaSalva = ObterAssinaturaCompleta()

        _alteracoesNaoSalvas = False

        AtualizarTituloJanela()

    End Sub

    Private Sub AtualizarEstadoAlteracoes()

        If CampoCanvas Is Nothing Then
            Exit Sub
        End If

        Dim assinaturaAtual As String = ObterAssinaturaCompleta()

        _alteracoesNaoSalvas =
            Not String.Equals(
                assinaturaAtual,
                _assinaturaSalva,
                StringComparison.Ordinal)

        AtualizarTituloJanela()

        If Not _alteracoesNaoSalvas AndAlso Not _processandoRecuperacao Then

            ExcluirArquivoRecuperacao()

        End If

    End Sub

    Private Function ConfirmarAlteracoesNaoSalvas() As Boolean

        If Not _alteracoesNaoSalvas Then

            Return True

        End If

        Dim resposta As DialogResult =
            MessageBox.Show(
                "O exercício possui alterações não salvas." &
                Environment.NewLine &
                Environment.NewLine &
                "Deseja salvar as alterações?",
                "Alterações não salvas",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning)

        Select Case resposta

            Case DialogResult.Yes

                Return SalvarExercicio()

            Case DialogResult.No

                Return True

            Case Else

                Return False

        End Select

    End Function

    Protected Overrides Sub OnDeactivate(e As EventArgs)

        MyBase.OnDeactivate(e)

        SalvarRecuperacaoAutomatica()

    End Sub

    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)

        If Not ConfirmarAlteracoesNaoSalvas() Then

            e.Cancel =
            True

            Return

        End If

        _temporizadorAutosave.Stop()

        ExcluirArquivoRecuperacao()

        If _frmGerenciadorObjetos IsNot Nothing AndAlso
           Not _frmGerenciadorObjetos.IsDisposed Then

            _frmGerenciadorObjetos.Close()

        End If

        MyBase.OnFormClosing(e)

    End Sub

    Private Function ObterAssinaturaCompleta() As String

        If CampoCanvas Is Nothing Then
            Return String.Empty
        End If

        Return String.Join(
            ChrW(30),
            CampoCanvas.ObterAssinaturaEstado(),
            _nomeExercicioAtual,
            _categoriaExercicioAtual,
            _duracaoExercicioAtual.ToString(),
            _descricaoExercicioAtual,
            _observacoesExercicioAtual)

    End Function

#End Region

#Region "Configurações do exercício"

    Private Sub ConfiguracoesExercicio_Click(
    sender As Object,
    e As EventArgs)

        AbrirConfiguracoesExercicio()

    End Sub

    Private Sub AbrirConfiguracoesExercicio()

        Using formulario As New FrmConfiguracoesExercicio()

            formulario.NomeExercicio =
                _nomeExercicioAtual

            formulario.CategoriaExercicio =
                _categoriaExercicioAtual

            formulario.DuracaoMinutos =
                _duracaoExercicioAtual

            formulario.DescricaoExercicio =
                _descricaoExercicioAtual

            formulario.ObservacoesExercicio =
                _observacoesExercicioAtual

            If formulario.ShowDialog(Me) <>
               DialogResult.OK Then

                CampoCanvas.Focus()

                Exit Sub

            End If

            _nomeExercicioAtual =
                formulario.NomeExercicio

            _categoriaExercicioAtual =
                formulario.CategoriaExercicio

            _duracaoExercicioAtual =
                formulario.DuracaoMinutos

            _descricaoExercicioAtual =
                formulario.DescricaoExercicio

            _observacoesExercicioAtual =
                formulario.ObservacoesExercicio

            AtualizarEstadoAlteracoes()

            CampoCanvas.Focus()

        End Using

    End Sub

#End Region

#Region "PDF e impressão"

    Private Sub ExportarPdf_Click(sender As Object, e As EventArgs)

        ExportarExercicioPdf()

    End Sub

    Private Sub ImprimirExercicio_Click(sender As Object, e As EventArgs)

        VisualizarImpressaoExercicio()

    End Sub

    Private Function PerguntarInclusaoCabecalho(
    tituloJanela As String) As Boolean?

        Dim resposta As DialogResult =
            MessageBox.Show(
                "Deseja incluir os dados do exercício?" &
                Environment.NewLine &
                Environment.NewLine &
                "Sim: inclui nome, categoria, duração, " &
                "descrição e observações." &
                Environment.NewLine &
                "Não: utiliza somente o campo.",
                tituloJanela,
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question)

        Select Case resposta

            Case DialogResult.Yes

                Return True

            Case DialogResult.No

                Return False

            Case Else

                Return Nothing

        End Select

    End Function

    Private Sub ExportarExercicioPdf()

        If CampoCanvas Is Nothing Then
            Exit Sub
        End If

        Dim incluirCabecalho As Boolean? =
            PerguntarInclusaoCabecalho(
                "Exportar PDF")

        If Not incluirCabecalho.HasValue Then

            CampoCanvas.Focus()

            Exit Sub

        End If

        Using dialogo As New SaveFileDialog()

            dialogo.Title =
                "Exportar exercício como PDF"

            dialogo.Filter =
                "Documento PDF (*.pdf)|*.pdf"

            dialogo.DefaultExt =
                "pdf"

            dialogo.AddExtension =
                True

            dialogo.OverwritePrompt =
                True

            dialogo.FileName =
                _nomeExercicioAtual &
                ".pdf"

            If dialogo.ShowDialog() <>
               DialogResult.OK Then

                CampoCanvas.Focus()

                Exit Sub

            End If

            Try

                Using imagemCampo As Bitmap =
                    CampoCanvas.GerarImagemCampo(
                        _preferencias.ResolucaoExportacao,
                        incluirCabecalho.Value,
                        _nomeExercicioAtual,
                        _categoriaExercicioAtual,
                        _duracaoExercicioAtual,
                        _descricaoExercicioAtual,
                        _observacoesExercicioAtual)

                    CriarArquivoPdf(
                        imagemCampo,
                        dialogo.FileName)

                End Using

                MessageBox.Show(
                    "PDF exportado com sucesso." &
                    Environment.NewLine &
                    Environment.NewLine &
                    dialogo.FileName,
                    "Exportação concluída",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information)

            Catch ex As Exception

                MessageBox.Show(
                    "Não foi possível exportar o PDF." &
                    Environment.NewLine &
                    Environment.NewLine &
                    ex.Message,
                    "Erro na exportação",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error)

            Finally

                CampoCanvas.Focus()

            End Try

        End Using

    End Sub

    Private Sub CriarArquivoPdf(
    imagemCampo As Bitmap,
    caminhoDestino As String)

        If imagemCampo Is Nothing Then

            Throw New ArgumentNullException(
                NameOf(imagemCampo))

        End If

        If String.IsNullOrWhiteSpace(
            caminhoDestino) Then

            Throw New ArgumentException(
                "O caminho do PDF não foi informado.")

        End If

        Using documento As New PdfDocument()

            documento.Info.Title =
                _nomeExercicioAtual

            documento.Info.Author =
                "TacticalStudio"

            documento.Info.Subject =
                _categoriaExercicioAtual

            Dim pagina As PdfPage =
                documento.AddPage()

            pagina.Size =
                PageSize.A4

            If imagemCampo.Width >=
               imagemCampo.Height Then

                pagina.Orientation =
                    PageOrientation.Landscape

            Else

                pagina.Orientation =
                    PageOrientation.Portrait

            End If

            Using fluxoImagem As New MemoryStream()

                imagemCampo.Save(
                    fluxoImagem,
                    ImageFormat.Png)

                fluxoImagem.Position =
                    0

                Using imagemPdf As XImage =
                    XImage.FromStream(
                        fluxoImagem)

                    Using grafico As XGraphics =
                        XGraphics.FromPdfPage(
                            pagina)

                        Dim margem As Double =
                            24.0

                        Dim larguraPagina As Double =
                            pagina.Width.Point

                        Dim alturaPagina As Double =
                            pagina.Height.Point

                        Dim larguraDisponivel As Double =
                            larguraPagina -
                            margem * 2.0

                        Dim alturaDisponivel As Double =
                            alturaPagina -
                            margem * 2.0

                        Dim escalaX As Double =
                            larguraDisponivel /
                            imagemPdf.PixelWidth

                        Dim escalaY As Double =
                            alturaDisponivel /
                            imagemPdf.PixelHeight

                        Dim escalaFinal As Double =
                            Math.Min(
                                escalaX,
                                escalaY)

                        Dim larguraFinal As Double =
                            imagemPdf.PixelWidth *
                            escalaFinal

                        Dim alturaFinal As Double =
                            imagemPdf.PixelHeight *
                            escalaFinal

                        Dim posicaoX As Double =
                            (larguraPagina -
                             larguraFinal) / 2.0

                        Dim posicaoY As Double =
                            (alturaPagina -
                             alturaFinal) / 2.0

                        grafico.DrawImage(
                            imagemPdf,
                            posicaoX,
                            posicaoY,
                            larguraFinal,
                            alturaFinal)

                    End Using

                End Using

            End Using

            documento.Save(
                caminhoDestino)

        End Using

    End Sub

    Private Sub VisualizarImpressaoExercicio()

        If CampoCanvas Is Nothing Then
            Exit Sub
        End If

        Dim incluirCabecalho As Boolean? =
            PerguntarInclusaoCabecalho(
                "Imprimir exercício")

        If Not incluirCabecalho.HasValue Then

            CampoCanvas.Focus()

            Exit Sub

        End If

        Try

            Using imagemCampo As Bitmap =
                CampoCanvas.GerarImagemCampo(
                    _preferencias.ResolucaoExportacao,
                    incluirCabecalho.Value,
                    _nomeExercicioAtual,
                    _categoriaExercicioAtual,
                    _duracaoExercicioAtual,
                    _descricaoExercicioAtual,
                    _observacoesExercicioAtual)

                Using documento As New PrintDocument()

                    documento.DocumentName =
                        _nomeExercicioAtual

                    documento.DefaultPageSettings.Landscape =
                        imagemCampo.Width >=
                        imagemCampo.Height

                    documento.DefaultPageSettings.Margins =
                        New Margins(
                            30,
                            30,
                            30,
                            30)

                    AddHandler documento.PrintPage,
                        Sub(sender, evento)

                            DesenharPaginaImpressao(
                                evento,
                                imagemCampo)

                        End Sub

                    Using visualizacao As New PrintPreviewDialog With {
                        .Document = documento,
                        .Width = 1200,
                        .Height = 800,
                        .UseAntiAlias = True,
                        .StartPosition =
                            FormStartPosition.CenterParent
                    }

                        visualizacao.ShowDialog(Me)

                    End Using

                End Using

            End Using

        Catch ex As Exception

            MessageBox.Show(
                "Não foi possível preparar a impressão." &
                Environment.NewLine &
                Environment.NewLine &
                ex.Message,
                "Erro na impressão",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error)

        Finally

            CampoCanvas.Focus()

        End Try

    End Sub

    Private Sub DesenharPaginaImpressao(
    evento As PrintPageEventArgs,
    imagemCampo As Bitmap)

        Dim areaImpressao As Rectangle =
            evento.MarginBounds

        Dim escalaX As Double =
            areaImpressao.Width /
            CDbl(imagemCampo.Width)

        Dim escalaY As Double =
            areaImpressao.Height /
            CDbl(imagemCampo.Height)

        Dim escalaFinal As Double =
            Math.Min(
                escalaX,
                escalaY)

        Dim larguraFinal As Integer =
            CInt(
                imagemCampo.Width *
                escalaFinal)

        Dim alturaFinal As Integer =
            CInt(
                imagemCampo.Height *
                escalaFinal)

        Dim posicaoX As Integer =
            areaImpressao.Left +
            (areaImpressao.Width -
             larguraFinal) \ 2

        Dim posicaoY As Integer =
            areaImpressao.Top +
            (areaImpressao.Height -
             alturaFinal) \ 2

        evento.Graphics.InterpolationMode =
            Drawing2D.InterpolationMode.HighQualityBicubic

        evento.Graphics.SmoothingMode =
            Drawing2D.SmoothingMode.HighQuality

        evento.Graphics.PixelOffsetMode =
            Drawing2D.PixelOffsetMode.HighQuality

        evento.Graphics.DrawImage(
            imagemCampo,
            New Rectangle(
                posicaoX,
                posicaoY,
                larguraFinal,
                alturaFinal))

        evento.HasMorePages =
            False

    End Sub

#End Region

End Class
