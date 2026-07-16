Imports System.ComponentModel
Imports Windows.Win32.UI

Public Class FrmConfiguracoesExercicio
    Inherits Form

    Private ReadOnly TxtNome As New TextBox()
    Private ReadOnly CmbCategoria As New ComboBox()
    Private ReadOnly NudDuracao As New NumericUpDown()
    Private ReadOnly TxtDescricao As New TextBox()
    Private ReadOnly TxtObservacoes As New TextBox()

    <Browsable(False)>
    <DesignerSerializationVisibility(
        DesignerSerializationVisibility.Hidden)>
    Public Property NomeExercicio As String

        Get
            Return TxtNome.Text.Trim()
        End Get

        Set(value As String)

            TxtNome.Text =
                If(value, String.Empty)

        End Set

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
        DesignerSerializationVisibility.Hidden)>
    Public Property CategoriaExercicio As String

        Get
            Return CmbCategoria.Text.Trim()
        End Get

        Set(value As String)

            CmbCategoria.Text = If(value, String.Empty)

        End Set

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
        DesignerSerializationVisibility.Hidden)>
    Public Property DuracaoMinutos As Integer

        Get
            Return CInt(NudDuracao.Value)
        End Get

        Set(value As Integer)

            Dim valorSeguro As Integer = Math.Max(CInt(NudDuracao.Minimum), Math.Min(CInt(NudDuracao.Maximum), value))

            NudDuracao.Value = valorSeguro

        End Set

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
        DesignerSerializationVisibility.Hidden)>
    Public Property DescricaoExercicio As String

        Get
            Return TxtDescricao.Text.Trim()
        End Get

        Set(value As String)

            TxtDescricao.Text =
                If(value, String.Empty)

        End Set

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
        DesignerSerializationVisibility.Hidden)>
    Public Property ObservacoesExercicio As String

        Get
            Return TxtObservacoes.Text.Trim()
        End Get

        Set(value As String)

            TxtObservacoes.Text = If(value, String.Empty)

        End Set

    End Property

    Public Sub New()

        Text =
            "Configurações do exercício"

        StartPosition =
            FormStartPosition.CenterParent

        FormBorderStyle =
            FormBorderStyle.FixedDialog

        MaximizeBox = False
        MinimizeBox = False
        ShowInTaskbar = False

        ClientSize =
            New Size(
                520,
                590)

        BackColor =
            Tema.Fundo

        ForeColor =
            Tema.Texto

        Font =
            Tema.FontePadrao

        CriarInterface()

    End Sub

    Private Sub CriarInterface()

        Dim painelPrincipal As New Panel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(20),
            .BackColor = Tema.Fundo
        }

        Controls.Add(
            painelPrincipal)

        Dim titulo As New Label With {
            .Text = "DADOS DO EXERCÍCIO",
            .Left = 20,
            .Top = 18,
            .Width = 460,
            .Height = 32,
            .Font = New Font(
                "Segoe UI",
                13.0F,
                FontStyle.Bold),
            .ForeColor = Tema.Texto
        }

        painelPrincipal.Controls.Add(
            titulo)

        ConfigurarTextBox(
            TxtNome,
            20,
            82,
            460,
            30)

        AdicionarLabel(
            painelPrincipal,
            "Nome",
            20,
            58)

        painelPrincipal.Controls.Add(
            TxtNome)

        ConfigurarComboCategoria()

        AdicionarLabel(
            painelPrincipal,
            "Categoria",
            20,
            126)

        painelPrincipal.Controls.Add(
            CmbCategoria)

        ConfigurarDuracao()

        AdicionarLabel(
            painelPrincipal,
            "Duração em minutos",
            270,
            126)

        painelPrincipal.Controls.Add(
            NudDuracao)

        ConfigurarTextBoxMultilinha(
            TxtDescricao,
            20,
            198,
            460,
            100)

        AdicionarLabel(
            painelPrincipal,
            "Descrição",
            20,
            174)

        painelPrincipal.Controls.Add(
            TxtDescricao)

        ConfigurarTextBoxMultilinha(
            TxtObservacoes,
            20,
            334,
            460,
            150)

        AdicionarLabel(
            painelPrincipal,
            "Observações",
            20,
            310)

        painelPrincipal.Controls.Add(
            TxtObservacoes)

        Dim botaoCancelar As New Button With {
            .Text = "Cancelar",
            .Left = 270,
            .Top = 520,
            .Width = 100,
            .Height = 38,
            .DialogResult = DialogResult.Cancel,
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Tema.Painel,
            .ForeColor = Tema.Texto
        }

        botaoCancelar.FlatAppearance.BorderColor =
            Tema.Borda

        painelPrincipal.Controls.Add(
            botaoCancelar)

        Dim botaoSalvar As New Button With {
            .Text = "Confirmar",
            .Left = 380,
            .Top = 520,
            .Width = 100,
            .Height = 38,
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Tema.CorPrimaria,
            .ForeColor = Color.White
        }

        botaoSalvar.FlatAppearance.BorderColor =
            Color.White

        AddHandler botaoSalvar.Click,
            AddressOf Confirmar_Click

        painelPrincipal.Controls.Add(
            botaoSalvar)

        AcceptButton =
            botaoSalvar

        CancelButton =
            botaoCancelar

    End Sub

    Private Sub ConfigurarTextBox(controle As TextBox, esquerda As Integer, topo As Integer, largura As Integer, altura As Integer)

        controle.Left = esquerda

        controle.Top = topo

        controle.Width = largura

        controle.Height = altura

        controle.BackColor = Tema.CampoEntrada

        controle.ForeColor = Tema.TextoCampo

        controle.BorderStyle =
            BorderStyle.FixedSingle

    End Sub

    Private Sub ConfigurarTextBoxMultilinha(
        controle As TextBox,
        esquerda As Integer,
        topo As Integer,
        largura As Integer,
        altura As Integer)

        ConfigurarTextBox(
            controle,
            esquerda,
            topo,
            largura,
            altura)

        controle.Multiline =
            True

        controle.ScrollBars =
            ScrollBars.Vertical

        controle.AcceptsReturn =
            True

    End Sub

    Private Sub ConfigurarComboCategoria()

        CmbCategoria.Left = 20

        CmbCategoria.Top = 150

        CmbCategoria.Width = 230

        CmbCategoria.Height = 30

        CmbCategoria.DropDownStyle = ComboBoxStyle.DropDown

        CmbCategoria.BackColor = Tema.CampoEntrada

        CmbCategoria.ForeColor = Tema.TextoCampo

        CmbCategoria.FlatStyle =
            FlatStyle.Flat

        CmbCategoria.Items.AddRange(
            {
                "Aquecimento",
                "Técnico",
                "Tático",
                "Físico",
                "Ataque",
                "Defesa",
                "Transição",
                "Finalização",
                "Bola parada",
                "Coletivo",
                "Outro"
            })

    End Sub

    Private Sub ConfigurarDuracao()

        NudDuracao.Left = 270

        NudDuracao.Top = 150

        NudDuracao.Width = 210

        NudDuracao.Height = 30

        NudDuracao.Minimum = 1D

        NudDuracao.Maximum = 600D

        NudDuracao.Value = 30D

        NudDuracao.BackColor = Tema.CampoEntrada

        NudDuracao.ForeColor = Tema.TextoCampo

        NudDuracao.BorderStyle =
            BorderStyle.FixedSingle

    End Sub

    Private Sub AdicionarLabel(
        painel As Control,
        texto As String,
        esquerda As Integer,
        topo As Integer)

        Dim labelCampo As New Label With {
            .Text = texto,
            .Left = esquerda,
            .Top = topo,
            .Width = 230,
            .Height = 22,
            .ForeColor = Tema.Texto
        }

        painel.Controls.Add(
            labelCampo)

    End Sub

    Private Sub Confirmar_Click(
        sender As Object,
        e As EventArgs)

        If String.IsNullOrWhiteSpace(
            TxtNome.Text) Then

            MessageBox.Show(
                "Informe o nome do exercício.",
                "Dados do exercício",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning)

            TxtNome.Focus()

            Exit Sub

        End If

        If String.IsNullOrWhiteSpace(
            CmbCategoria.Text) Then

            MessageBox.Show(
                "Informe a categoria do exercício.",
                "Dados do exercício",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning)

            CmbCategoria.Focus()

            Exit Sub

        End If

        DialogResult =
            DialogResult.OK

        Close()

    End Sub

End Class