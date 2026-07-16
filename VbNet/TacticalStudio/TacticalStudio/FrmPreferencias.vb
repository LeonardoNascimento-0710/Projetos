Imports System.ComponentModel
Imports System.Drawing

Public Class FrmPreferencias
    Inherits Form

#Region "Variaveis"

    Private ReadOnly CmbTema As New ComboBox()

    Private ReadOnly CmbCorPrincipal As New ComboBox()

    Private ReadOnly ChkAutosave As New CheckBox()

    Private ReadOnly NudIntervalo As New NumericUpDown()

    Private ReadOnly CmbResolucao As New ComboBox()

    Private ReadOnly ChkGradeVisivel As New CheckBox()

    Private ReadOnly ChkEncaixeGrade As New CheckBox()

    Private ReadOnly CmbEspacamentoGrade As New ComboBox()

#End Region

#Region "Propriedades"

    <Browsable(False)>
    <DesignerSerializationVisibility(
        DesignerSerializationVisibility.Hidden)>
    Public Property ModoTema As ModoTemaAplicacao

        Get

            If CmbTema.SelectedIndex = 1 Then
                Return ModoTemaAplicacao.Claro
            End If

            Return ModoTemaAplicacao.Escuro

        End Get

        Set(value As ModoTemaAplicacao)

            If value =
               ModoTemaAplicacao.Claro Then

                CmbTema.SelectedIndex = 1

            Else

                CmbTema.SelectedIndex = 0

            End If

        End Set

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
        DesignerSerializationVisibility.Hidden)>
    Public Property CorPrincipalArgb As Integer

        Get

            Return ObterCorSelecionada().
                ToArgb()

        End Get

        Set(value As Integer)

            Dim cor As Color =
                Color.FromArgb(value)

            If cor.R = 35 AndAlso
               cor.G = 105 AndAlso
               cor.B = 190 Then

                CmbCorPrincipal.SelectedItem =
                    "Azul"

            ElseIf cor.R = 35 AndAlso
                   cor.G = 145 AndAlso
                   cor.B = 75 Then

                CmbCorPrincipal.SelectedItem =
                    "Verde"

            ElseIf cor.R = 110 AndAlso
                   cor.G = 60 AndAlso
                   cor.B = 170 Then

                CmbCorPrincipal.SelectedItem =
                    "Roxo"

            ElseIf cor.R = 220 AndAlso
                   cor.G = 105 AndAlso
                   cor.B = 25 Then

                CmbCorPrincipal.SelectedItem =
                    "Laranja"

            Else

                CmbCorPrincipal.SelectedItem =
                    "Vermelho TacticalStudio"

            End If

        End Set

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
        DesignerSerializationVisibility.Hidden)>
    Public Property AutosaveAtivo As Boolean

        Get
            Return ChkAutosave.Checked
        End Get

        Set(value As Boolean)

            ChkAutosave.Checked =
                value

            AtualizarEstadoAutosave()

        End Set

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
        DesignerSerializationVisibility.Hidden)>
    Public Property IntervaloAutosaveSegundos As Integer

        Get
            Return CInt(NudIntervalo.Value)
        End Get

        Set(value As Integer)

            Dim valorSeguro As Integer =
                Math.Max(
                    CInt(NudIntervalo.Minimum),
                    Math.Min(
                        CInt(NudIntervalo.Maximum),
                        value))

            NudIntervalo.Value =
                valorSeguro

        End Set

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
        DesignerSerializationVisibility.Hidden)>
    Public Property ResolucaoExportacao As Integer

        Get

            If CmbResolucao.SelectedItem Is Nothing Then
                Return 2560
            End If

            Return CInt(
                CmbResolucao.SelectedItem)

        End Get

        Set(value As Integer)

            If CmbResolucao.Items.Contains(value) Then

                CmbResolucao.SelectedItem =
                    value

            Else

                CmbResolucao.SelectedItem =
                    2560

            End If

        End Set

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)>
    Public Property GradeVisivel As Boolean

        Get
            Return ChkGradeVisivel.Checked
        End Get

        Set(value As Boolean)

            ChkGradeVisivel.Checked =
            value

        End Set

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)>
    Public Property EncaixeGradeAtivo As Boolean

        Get
            Return ChkEncaixeGrade.Checked
        End Get

        Set(value As Boolean)

            ChkEncaixeGrade.Checked =
            value

        End Set

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)>
    Public Property EspacamentoGradePercentual As Integer

        Get

            If CmbEspacamentoGrade.SelectedItem Is Nothing Then
                Return 5
            End If

            Return CInt(
            CmbEspacamentoGrade.SelectedItem)

        End Get

        Set(value As Integer)

            If CmbEspacamentoGrade.Items.Contains(
            value) Then

                CmbEspacamentoGrade.SelectedItem =
                value

            Else

                CmbEspacamentoGrade.SelectedItem =
                5

            End If

        End Set

    End Property

#End Region
    Public Sub New()

        Text =
            "Preferências do TacticalStudio"

        StartPosition =
            FormStartPosition.CenterParent

        FormBorderStyle =
            FormBorderStyle.FixedDialog

        MaximizeBox = False
        MinimizeBox = False
        ShowInTaskbar = False

        ClientSize =
    New Size(
        500,
        690)

        BackColor =
            Tema.Fundo

        ForeColor =
            Tema.Texto

        Font =
            Tema.FontePadrao

        CriarInterface()

    End Sub

    Private Sub CriarInterface()

        Dim painel As New Panel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(20),
            .BackColor = Tema.Fundo
        }

        Controls.Add(painel)

        Dim titulo As New Label With {
            .Text = "PREFERÊNCIAS",
            .Left = 20,
            .Top = 18,
            .Width = 440,
            .Height = 34,
            .Font = New Font(
                "Segoe UI",
                13.0F,
                FontStyle.Bold),
            .ForeColor = Tema.Texto
        }

        painel.Controls.Add(titulo)

        AdicionarLabel(
            painel,
            "Tema da interface",
            20,
            68)

        ConfigurarCombo(
            CmbTema,
            20,
            92)

        CmbTema.Items.AddRange(
            {
                "Escuro",
                "Claro"
            })

        CmbTema.SelectedIndex = 0

        painel.Controls.Add(CmbTema)

        AdicionarLabel(
            painel,
            "Cor principal",
            20,
            136)

        ConfigurarCombo(
            CmbCorPrincipal,
            20,
            160)

        CmbCorPrincipal.Items.AddRange(
            {
                "Vermelho TacticalStudio",
                "Azul",
                "Verde",
                "Roxo",
                "Laranja"
            })

        CmbCorPrincipal.SelectedIndex = 0

        painel.Controls.Add(
            CmbCorPrincipal)

        ChkAutosave.Text =
            "Ativar salvamento automático"

        ChkAutosave.Left = 20
        ChkAutosave.Top = 212
        ChkAutosave.Width = 440
        ChkAutosave.Height = 28
        ChkAutosave.Checked = True
        ChkAutosave.ForeColor = Tema.Texto
        ChkAutosave.BackColor = Tema.Fundo

        AddHandler ChkAutosave.CheckedChanged,
            Sub(sender, e)

                AtualizarEstadoAutosave()

            End Sub

        painel.Controls.Add(
            ChkAutosave)

        AdicionarLabel(
            painel,
            "Intervalo do autosave em segundos",
            20,
            252)

        NudIntervalo.Left = 20
        NudIntervalo.Top = 276
        NudIntervalo.Width = 440
        NudIntervalo.Height = 30
        NudIntervalo.Minimum = 30D
        NudIntervalo.Maximum = 600D
        NudIntervalo.Increment = 30D
        NudIntervalo.Value = 60D
        NudIntervalo.BackColor = Tema.CampoEntrada
        NudIntervalo.ForeColor = Tema.TextoCampo
        NudIntervalo.BorderStyle = BorderStyle.FixedSingle

        painel.Controls.Add(
            NudIntervalo)

        AdicionarLabel(
            painel,
            "Resolução padrão das exportações",
            20,
            322)

        ConfigurarCombo(
            CmbResolucao,
            20,
            346)

        CmbResolucao.DropDownStyle =
            ComboBoxStyle.DropDownList

        CmbResolucao.Items.AddRange(
            {
                1280,
                1920,
                2560,
                3840,
                5120
            })

        CmbResolucao.SelectedItem =
            2560

        Dim tituloGrade As New Label With {
    .Text = "GRADE E ALINHAMENTO",
    .Left = 20,
    .Top = 398,
    .Width = 440,
    .Height = 26,
    .Font = New Font(
        "Segoe UI",
        9.5F,
        FontStyle.Bold),
    .ForeColor = Tema.Texto
}

        painel.Controls.Add(
    tituloGrade)

        ChkGradeVisivel.Text =
    "Exibir grade no campo"

        ChkGradeVisivel.Left =
    20

        ChkGradeVisivel.Top =
    430

        ChkGradeVisivel.Width =
    440

        ChkGradeVisivel.Height =
    28

        ChkGradeVisivel.ForeColor =
    Tema.Texto

        ChkGradeVisivel.BackColor =
    Tema.Fundo

        painel.Controls.Add(
    ChkGradeVisivel)

        ChkEncaixeGrade.Text =
    "Ativar encaixe automático na grade"

        ChkEncaixeGrade.Left =
    20

        ChkEncaixeGrade.Top =
    466

        ChkEncaixeGrade.Width =
    440

        ChkEncaixeGrade.Height =
    28

        ChkEncaixeGrade.ForeColor =
    Tema.Texto

        ChkEncaixeGrade.BackColor =
    Tema.Fundo

        painel.Controls.Add(
    ChkEncaixeGrade)

        AdicionarLabel(
    painel,
    "Espaçamento da grade",
    20,
    504)

        ConfigurarCombo(
    CmbEspacamentoGrade,
    20,
    528)

        CmbEspacamentoGrade.Items.AddRange(
    {
        2,
        5,
        10
    })

        CmbEspacamentoGrade.SelectedItem =
    5

        painel.Controls.Add(
    CmbEspacamentoGrade)

        painel.Controls.Add(
            CmbResolucao)

        Dim explicacao As New Label With {
.Text =
    "O encaixe mantém jogadores, linhas e áreas " &
    "alinhados aos pontos da grade.",
            .Left = 20,
            .Top = 574,
            .Width = 440,
            .Height = 40,
            .ForeColor = Tema.TextoSecundario
        }

        painel.Controls.Add(
            explicacao)

        Dim botaoCancelar As New Button With {
            .Text = "Cancelar",
            .Left = 250,
            .Top = 628,
            .Width = 100,
            .Height = 38,
            .DialogResult = DialogResult.Cancel,
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Tema.Painel,
            .ForeColor = Tema.Texto
        }

        botaoCancelar.FlatAppearance.BorderColor =
            Tema.Borda

        painel.Controls.Add(
            botaoCancelar)

        Dim botaoConfirmar As New Button With {
            .Text = "Confirmar",
            .Left = 360,
            .Top = 628,
            .Width = 100,
            .Height = 38,
            .DialogResult = DialogResult.OK,
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Tema.CorPrimaria,
            .ForeColor = Color.White
        }

        botaoConfirmar.FlatAppearance.BorderColor =
            Color.White

        painel.Controls.Add(
            botaoConfirmar)

        AcceptButton =
            botaoConfirmar

        CancelButton =
            botaoCancelar

        AtualizarEstadoAutosave()

    End Sub

    Private Sub ConfigurarCombo(
        combo As ComboBox,
        esquerda As Integer,
        topo As Integer)

        combo.Left = esquerda
        combo.Top = topo
        combo.Width = 440
        combo.Height = 30
        combo.DropDownStyle =
            ComboBoxStyle.DropDownList
        combo.BackColor =
            Tema.CampoEntrada
        combo.ForeColor =
            Tema.TextoCampo
        combo.FlatStyle =
            FlatStyle.Flat

    End Sub

    Private Function ObterCorSelecionada() As Color

        Select Case CStr(
            CmbCorPrincipal.SelectedItem)

            Case "Azul"

                Return Color.FromArgb(
                    35,
                    105,
                    190)

            Case "Verde"

                Return Color.FromArgb(
                    35,
                    145,
                    75)

            Case "Roxo"

                Return Color.FromArgb(
                    110,
                    60,
                    170)

            Case "Laranja"

                Return Color.FromArgb(
                    220,
                    105,
                    25)

            Case Else

                Return Color.FromArgb(
                    134,
                    29,
                    29)

        End Select

    End Function

    Private Sub AtualizarEstadoAutosave()

        NudIntervalo.Enabled =
            ChkAutosave.Checked

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
            .Width = 440,
            .Height = 22,
            .ForeColor = Tema.Texto
        }

        painel.Controls.Add(
            labelCampo)

    End Sub

End Class