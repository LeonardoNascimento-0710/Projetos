Imports System.Collections.Generic
Imports System.IO
Imports System.Text.Json
Imports System.Text
Imports System.Drawing.Imaging
Imports System.Drawing.Printing
Imports PdfSharp
Imports PdfSharp.Drawing
Imports PdfSharp.Pdf
Imports TacticalStudio.Core.Enums
Imports TacticalStudio.Core.Classes

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

            botao.ForeColor = Color.White

            botao.FlatAppearance.BorderColor = Color.White

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

        PnlEsquerdo.Controls.Clear()
        _botoesFerramentas.Clear()

        Dim painelFerramentas As New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False,
            .AutoScroll = True,
            .BackColor = Tema.Painel,
            .Padding = New Padding(6)
        }

        Dim larguraBotao As Integer =
            Math.Max(
                120,
                PnlEsquerdo.ClientSize.Width - 24)

        Dim titulo As New Label With {
            .Text = "FERRAMENTAS",
            .ForeColor = Tema.Texto,
            .Font = New Font(
                "Segoe UI",
                10.0F,
                FontStyle.Bold),
            .Width = larguraBotao,
            .Height = 34,
            .TextAlign = ContentAlignment.MiddleLeft
        }

        painelFerramentas.Controls.Add(titulo)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Selecionar,
            "Selecionar",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Jogador,
            "Jogador",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Bola,
            "Bola",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Cone,
            "Cone",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Gol,
            "Gol",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Manequim,
            "Manequim",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.LinhaContinua,
            "Linha",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.LinhaTracejada,
            "Linha tracejada",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Seta,
            "Seta",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Area,
            "Área",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Marcador,
            "Marcador",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Texto,
            "Texto",
            larguraBotao)

        _botaoRecortarCampo = New Button With {
            .Text = "Recortar campo",
            .Width = larguraBotao,
            .Height = 36,
            .Margin = New Padding(0, 12, 0, 3),
            .FlatStyle = FlatStyle.Flat,
            .TextAlign = ContentAlignment.MiddleLeft,
            .Cursor = Cursors.Cross,
            .BackColor = Tema.Painel,
            .ForeColor = Tema.Texto,
            .UseVisualStyleBackColor = False
        }

        _botaoRecortarCampo.FlatAppearance.BorderColor =
            Tema.CorPrimaria

        _botaoRecortarCampo.FlatAppearance.MouseOverBackColor =
            Tema.PainelHover

        _botaoRecortarCampo.FlatAppearance.MouseDownBackColor =
            Tema.CorPrimaria

        AddHandler _botaoRecortarCampo.Click,
            AddressOf RecortarCampo_Click

        painelFerramentas.Controls.Add(
            _botaoRecortarCampo)

        PnlEsquerdo.Controls.Add(painelFerramentas)

        AtualizarBotoesFerramentas(
            FerramentaCampo.Selecionar)

        AtualizarEstadoBotaoRecorte()

    End Sub

    Private Sub AdicionarBotaoFerramenta(
    painel As FlowLayoutPanel,
    ferramenta As FerramentaCampo,
    texto As String,
    largura As Integer)

        Dim botao As New Button With {
            .Text = texto,
            .Tag = ferramenta,
            .Width = largura,
            .Height = 36,
            .Margin = New Padding(0, 3, 0, 3),
            .FlatStyle = FlatStyle.Flat,
            .TextAlign = ContentAlignment.MiddleLeft,
            .Cursor = Cursors.Hand,
            .BackColor = Tema.Painel,
            .ForeColor = Tema.Texto
        }

        botao.FlatAppearance.BorderColor =
            Tema.Borda

        botao.FlatAppearance.MouseOverBackColor =
            Tema.PainelHover

        botao.FlatAppearance.MouseDownBackColor =
            Tema.CorPrimaria

        AddHandler botao.Click,
            AddressOf BotaoFerramenta_Click

        _botoesFerramentas.Add(
            ferramenta,
            botao)

        painel.Controls.Add(botao)

    End Sub

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
                    Color.White

                botao.FlatAppearance.BorderColor =
                    Color.White

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
                Color.White

            _botaoRecortarCampo.FlatAppearance.BorderColor =
                Color.White

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

    Private Sub MontarPainelRecorteCampo()

        If CampoCanvas Is Nothing Then
            Exit Sub
        End If

        PnlDireito.Controls.Clear()

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
            .Text = "RECORTE DO CAMPO",
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
            .ForeColor = If(
                selecionando,
                Color.Gold,
                Tema.Texto),
            .Font = New Font(
                "Segoe UI",
                9.5F,
                FontStyle.Bold),
            .Width = largura,
            .Height = 30,
            .TextAlign = ContentAlignment.MiddleLeft
        }

        painel.Controls.Add(
            status)

        Dim alturaOrientacao As Integer

        If selecionando Then

            alturaOrientacao =
                150

        ElseIf CampoCanvas.RecorteAtivo Then

            alturaOrientacao =
                100

        Else

            alturaOrientacao =
                82

        End If

        Dim orientacao As New Label With {
            .Text = orientacaoTexto,
            .ForeColor = Tema.TextoSecundario,
            .Font = Tema.FontePadrao,
            .Width = largura,
            .Height = alturaOrientacao,
            .TextAlign = ContentAlignment.TopLeft
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
                .Margin = New Padding(
                    0,
                    8,
                    0,
                    8),
                .TextAlign = ContentAlignment.MiddleLeft
            }

            painel.Controls.Add(
                detalhes)

        End If

        Dim botaoSelecionar As New Button With {
            .Text = If(
                selecionando,
                "Cancelar seleção do recorte",
                "Selecionar nova área"),
            .Width = largura,
            .Height = 38,
            .Margin = New Padding(
                0,
                8,
                0,
                4),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = If(
                selecionando,
                Tema.CorPrimaria,
                Tema.Painel),
            .ForeColor = If(
                selecionando,
                Color.White,
                Tema.Texto),
            .Cursor = Cursors.Hand,
            .UseVisualStyleBackColor = False
        }

        botaoSelecionar.FlatAppearance.BorderColor =
            If(
                selecionando,
                Color.White,
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

        Dim botaoCampoInteiro As New Button With {
            .Text = "Usar campo inteiro",
            .Width = largura,
            .Height = 38,
            .Margin = New Padding(
                0,
                4,
                0,
                4),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Tema.Painel,
            .ForeColor = Tema.Texto,
            .Cursor = Cursors.Hand,
            .UseVisualStyleBackColor = False,
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

                CampoCanvas.Focus()

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
            .Margin = New Padding(
                0,
                12,
                0,
                0),
            .TextAlign = ContentAlignment.MiddleLeft
        }

        painel.Controls.Add(
            avisoExportacao)

    End Sub

    Private Sub MontarPainelPropriedades(
    objeto As ObjetoCampo)

        PnlDireito.Controls.Clear()

        Dim painel As New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False,
            .AutoScroll = True,
            .BackColor = Tema.Painel,
            .Padding = New Padding(10)
        }

        PnlDireito.Controls.Add(painel)

        Dim largura As Integer =
            Math.Max(
                180,
                PnlDireito.ClientSize.Width - 32)

        Dim titulo As New Label With {
            .Text = "PROPRIEDADES",
            .ForeColor = Tema.Texto,
            .Font = New Font(
                "Segoe UI",
                11.0F,
                FontStyle.Bold),
            .Width = largura,
            .Height = 32,
            .TextAlign = ContentAlignment.MiddleLeft
        }

        painel.Controls.Add(titulo)

        If objeto Is Nothing Then

            Dim mensagem As New Label With {
                .Text = "Nenhum objeto selecionado.",
                .ForeColor = Color.Silver,
                .Font = Tema.FontePadrao,
                .Width = largura,
                .Height = 50,
                .TextAlign = ContentAlignment.MiddleLeft
            }

            painel.Controls.Add(mensagem)

            Exit Sub

        End If

        Dim tipoObjeto As New Label With {
            .Text = ObterNomeObjeto(objeto),
            .ForeColor = Tema.CorPrimaria,
            .Font = New Font(
                "Segoe UI",
                10.0F,
                FontStyle.Bold),
            .Width = largura,
            .Height = 30,
            .TextAlign = ContentAlignment.MiddleLeft
        }

        painel.Controls.Add(tipoObjeto)

        If TypeOf objeto Is Jogador Then

            MontarPropriedadesJogador(
                painel,
                DirectCast(objeto, Jogador))

        ElseIf TypeOf objeto Is Bola Then

            AdicionarMensagemPainel(
                painel,
                "A bola não possui propriedades editáveis no momento.")

        ElseIf TypeOf objeto Is Cone Then

            MontarPropriedadesCone(
                painel,
                DirectCast(objeto, Cone))

        ElseIf TypeOf objeto Is Gol Then

            MontarPropriedadesGol(
                painel,
                DirectCast(objeto, Gol))

        ElseIf TypeOf objeto Is Manequim Then

            MontarPropriedadesManequim(
                painel,
                DirectCast(objeto, Manequim))

        ElseIf TypeOf objeto Is LinhaTatica Then

            MontarPropriedadesLinha(
                painel,
                DirectCast(objeto, LinhaTatica))

        ElseIf TypeOf objeto Is AreaTatica Then

            MontarPropriedadesArea(
                painel,
                DirectCast(objeto, AreaTatica))

        ElseIf TypeOf objeto Is MarcadorTatico Then

            MontarPropriedadesMarcador(
                painel,
                DirectCast(objeto, MarcadorTatico))

        ElseIf TypeOf objeto Is TextoTatico Then

            MontarPropriedadesTexto(
                painel,
                DirectCast(objeto, TextoTatico))

        End If

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

        Dim botaoDuplicar As New Button With {
    .Text = "Duplicar objeto  (Ctrl+D)",
    .Width = largura,
    .Height = 36,
    .Margin = New Padding(
        0,
        3,
        0,
        3),
    .FlatStyle = FlatStyle.Flat,
    .BackColor = Tema.Painel,
    .ForeColor = Tema.Texto,
    .Cursor = Cursors.Hand
}

        botaoDuplicar.FlatAppearance.BorderColor =
    Tema.Borda

        AddHandler botaoDuplicar.Click,
    Sub(sender, e)

        CampoCanvas.DuplicarSelecionado()
        CampoCanvas.Focus()

    End Sub

        painel.Controls.Add(
    botaoDuplicar)

        Dim botaoFrente As New Button With {
    .Text = "Trazer para frente",
    .Width = largura,
    .Height = 36,
    .Margin = New Padding(
        0,
        3,
        0,
        3),
    .FlatStyle = FlatStyle.Flat,
    .BackColor = Tema.Painel,
    .ForeColor = Tema.Texto,
    .Cursor = Cursors.Hand
}

        botaoFrente.FlatAppearance.BorderColor =
    Tema.Borda

        AddHandler botaoFrente.Click,
    Sub(sender, e)

        CampoCanvas.TrazerParaFrente()
        CampoCanvas.Focus()

    End Sub

        painel.Controls.Add(
    botaoFrente)

        Dim botaoTras As New Button With {
    .Text = "Enviar para trás",
    .Width = largura,
    .Height = 36,
    .Margin = New Padding(
        0,
        3,
        0,
        3),
    .FlatStyle = FlatStyle.Flat,
    .BackColor = Tema.Painel,
    .ForeColor = Tema.Texto,
    .Cursor = Cursors.Hand
}

        botaoTras.FlatAppearance.BorderColor =
    Tema.Borda

        AddHandler botaoTras.Click,
    Sub(sender, e)

        CampoCanvas.EnviarParaTras()
        CampoCanvas.Focus()

    End Sub

        painel.Controls.Add(
    botaoTras)

        Dim botaoExcluir As New Button With {
            .Text = "Excluir objeto",
            .Width = largura,
            .Height = 38,
            .Margin = New Padding(0, 12, 0, 4),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Tema.CorPrimaria,
            .ForeColor = Color.White,
            .Cursor = Cursors.Hand
        }

        botaoExcluir.FlatAppearance.BorderColor =
            Color.White

        AddHandler botaoExcluir.Click,
            Sub(sender, e)

                CampoCanvas.ExcluirSelecionado()
                CampoCanvas.Focus()

            End Sub

        AdicionarAcoesBloqueioIndividual(painel, largura)

        painel.Controls.Add(botaoExcluir)

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

    Private Function ObterNomeObjeto(
    objeto As ObjetoCampo) As String

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

#End Region

#Region "Propriedades dos objetos"

    Private Sub MontarPropriedadesJogador(
    painel As FlowLayoutPanel,
    jogador As Jogador)

        Dim numero As NumericUpDown =
            CriarCampoNumerico(
                1D,
                99D,
                jogador.Numero)

        AddHandler numero.ValueChanged,
            Sub(sender, e)

                jogador.Numero =
                    CInt(numero.Value)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Número",
            numero)

        Dim nome As New TextBox With {
            .Text = jogador.Nome,
            .BackColor = Tema.CampoEntrada,
            .ForeColor = Tema.TextoCampo,
            .BorderStyle = BorderStyle.FixedSingle
        }

        AddHandler nome.TextChanged,
            Sub(sender, e)

                jogador.Nome =
                    nome.Text

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Nome",
            nome)

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
                Color.White

            botao.FlatAppearance.BorderColor =
                Color.White

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

        PnlDireito.Controls.Clear()

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

        Return MyBase.ProcessCmdKey(msg, keyData)

    End Function

#End Region

#Region "Barra de arquivos"

    Private Sub CriarBarraArquivo()

        If PnlSuperior.Controls.ContainsKey("PnlArquivoDinamico") Then

            PnlSuperior.Controls.RemoveByKey("PnlArquivoDinamico")

        End If

        Dim painelArquivo As New FlowLayoutPanel With {
            .Name = "PnlArquivoDinamico",
            .Dock = DockStyle.Right,
            .Width = 1080,
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = False,
            .Padding = New Padding(5),
            .BackColor = Tema.CorPrimaria
        }

        painelArquivo.Controls.Add(CriarBotaoArquivo("Novo", AddressOf NovoExercicio_Click))

        painelArquivo.Controls.Add(CriarBotaoArquivo("Abrir", AddressOf AbrirExercicio_Click))

        painelArquivo.Controls.Add(CriarBotaoArquivo("Salvar", AddressOf SalvarExercicio_Click))

        painelArquivo.Controls.Add(CriarBotaoArquivo("Exportar", AddressOf ExportarImagem_Click))

        painelArquivo.Controls.Add(CriarBotaoArquivo("Dados", AddressOf ConfiguracoesExercicio_Click))

        painelArquivo.Controls.Add(CriarBotaoArquivo("PDF", AddressOf ExportarPdf_Click))

        painelArquivo.Controls.Add(CriarBotaoArquivo("Imprimir", AddressOf ImprimirExercicio_Click))

        painelArquivo.Controls.Add(CriarBotaoArquivo("Opções", AddressOf Preferencias_Click))

        painelArquivo.Controls.Add(CriarBotaoArquivo("Objetos", AddressOf Objetos_Click))

        Dim botaoCampoInteiro As Button =
            CriarBotaoArquivo(
                "Campo inteiro",
                AddressOf CampoInteiro_Click)

        botaoCampoInteiro.Width =
            108

        painelArquivo.Controls.Add(
            botaoCampoInteiro)

        painelArquivo.Controls.Add(CriarBotaoArquivo("Sobre", AddressOf Sobre_Click))

        PnlSuperior.Controls.Add(painelArquivo)

        painelArquivo.BringToFront()

    End Sub

    Private Sub CampoInteiro_Click(
    sender As Object,
    e As EventArgs)

        If CampoCanvas Is Nothing Then
            Exit Sub
        End If

        CampoCanvas.LimparRecorteCampo()

        CampoCanvas.Focus()

    End Sub

    Private Function CriarBotaoArquivo(texto As String, acao As EventHandler) As Button

        Dim botao As New Button With {
        .Text = texto,
        .Width = 88,
        .Height = 34,
        .Margin = New Padding(3),
        .FlatStyle = FlatStyle.Flat,
        .BackColor = Tema.Painel,
        .ForeColor = Tema.Texto,
        .Cursor = Cursors.Hand,
        .UseVisualStyleBackColor = False
    }

        botao.FlatAppearance.BorderColor =
        Tema.Borda

        botao.FlatAppearance.MouseOverBackColor =
        Tema.PainelHover

        botao.FlatAppearance.MouseDownBackColor =
        Tema.PainelHover

        AddHandler botao.Click,
        acao

        Return botao

    End Function

    Private Sub Sobre_Click(sender As Object, e As EventArgs)

        Using formulario As New FrmSobre()

            formulario.ShowDialog(Me)

        End Using

        CampoCanvas.Focus()

    End Sub

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

            dialogo.Multiselect = False

            If dialogo.ShowDialog() <> DialogResult.OK Then

                Exit Sub

            End If

            Try

                Dim conteudoJson As String = File.ReadAllText(dialogo.FileName, Encoding.UTF8)

                Dim arquivo As ArquivoExercicio = CampoCanvas.ImportarExercicioJson(conteudoJson)

                _caminhoArquivoAtual = dialogo.FileName

                If String.IsNullOrWhiteSpace(arquivo.Nome) Then

                    _nomeExercicioAtual = Path.GetFileNameWithoutExtension(dialogo.FileName)

                Else

                    _nomeExercicioAtual = arquivo.Nome

                End If

                _categoriaExercicioAtual = If(String.IsNullOrWhiteSpace(arquivo.Categoria), "Tático", arquivo.Categoria)

                _duracaoExercicioAtual = Math.Max(1, arquivo.DuracaoMinutos)

                _descricaoExercicioAtual = If(arquivo.Descricao, String.Empty)

                _observacoesExercicioAtual = If(arquivo.Observacoes, String.Empty)

                MarcarComoSalvo()

                ExcluirArquivoRecuperacao()

                CampoCanvas.Focus()

            Catch ex As Exception

                MessageBox.Show(
                    "Não foi possível abrir o exercício." &
                    Environment.NewLine &
                    Environment.NewLine &
                    ex.Message,
                    "Erro ao abrir",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error)

            End Try

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
