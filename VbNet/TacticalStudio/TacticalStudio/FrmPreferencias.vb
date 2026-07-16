Imports System.ComponentModel

Public Class FrmPreferencias
    Inherits Form

    Private ReadOnly ChkAutosave As New CheckBox()
    Private ReadOnly NudIntervalo As New NumericUpDown()
    Private ReadOnly CmbResolucao As New ComboBox()

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
            Return CInt(
                NudIntervalo.Value)
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

            If CmbResolucao.Items.Contains(
                value) Then

                CmbResolucao.SelectedItem =
                    value

            Else

                CmbResolucao.SelectedItem =
                    2560

            End If

        End Set

    End Property

    Public Sub New()

        Text =
            "Preferências do TacticalStudio"

        StartPosition =
            FormStartPosition.CenterParent

        FormBorderStyle =
            FormBorderStyle.FixedDialog

        MaximizeBox =
            False

        MinimizeBox =
            False

        ShowInTaskbar =
            False

        ClientSize =
            New Size(
                500,
                360)

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

        Controls.Add(
            painel)

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

        painel.Controls.Add(
            titulo)

        ChkAutosave.Text =
            "Ativar salvamento automático"

        ChkAutosave.Left =
            20

        ChkAutosave.Top =
            72

        ChkAutosave.Width =
            440

        ChkAutosave.Height =
            28

        ChkAutosave.Checked =
            True

        ChkAutosave.ForeColor =
            Tema.Texto

        ChkAutosave.BackColor =
            Tema.Fundo

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
            116)

        NudIntervalo.Left =
            20

        NudIntervalo.Top =
            140

        NudIntervalo.Width =
            440

        NudIntervalo.Height =
            30

        NudIntervalo.Minimum =
            30D

        NudIntervalo.Maximum =
            600D

        NudIntervalo.Increment =
            30D

        NudIntervalo.Value =
            60D

        NudIntervalo.BackColor =
            Color.FromArgb(
                50,
                50,
                50)

        NudIntervalo.ForeColor =
            Color.White

        NudIntervalo.BorderStyle =
            BorderStyle.FixedSingle

        painel.Controls.Add(
            NudIntervalo)

        AdicionarLabel(
            painel,
            "Resolução padrão das exportações",
            20,
            190)

        CmbResolucao.Left =
            20

        CmbResolucao.Top =
            214

        CmbResolucao.Width =
            440

        CmbResolucao.Height =
            30

        CmbResolucao.DropDownStyle =
            ComboBoxStyle.DropDownList

        CmbResolucao.BackColor =
            Color.FromArgb(
                50,
                50,
                50)

        CmbResolucao.ForeColor =
            Color.White

        CmbResolucao.FlatStyle =
            FlatStyle.Flat

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

        painel.Controls.Add(
            CmbResolucao)

        Dim explicacao As New Label With {
            .Text =
                "Resoluções maiores geram imagens e PDFs com " &
                "mais qualidade, mas também aumentam o tamanho do arquivo.",
            .Left = 20,
            .Top = 252,
            .Width = 440,
            .Height = 42,
            .ForeColor = Color.Silver
        }

        painel.Controls.Add(
            explicacao)

        Dim botaoCancelar As New Button With {
            .Text = "Cancelar",
            .Left = 250,
            .Top = 304,
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
            .Top = 304,
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