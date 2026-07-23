Imports System.Security.Cryptography
Imports System.Text

Public Class FrmAcessoAdmin

#Region "Constantes de acesso"

    Private Const USUARIO_ADMINISTRADOR As String =
        "admin"

    ' Senha inicial: Admin@2026
    Private Const HASH_SENHA_ADMINISTRADOR As String =
        "A36AEF5A11C4073FBE60314FC9DF530A9D5F986533594D1F5190742FF9E0E408"

    Private Const MAXIMO_TENTATIVAS As Integer =
        5

#End Region

#Region "Campos"

    Private tentativasRealizadas As Integer = 0

    Private PnlTopo As Panel
    Private PnlConteudo As Panel

    Private LblTitulo As Label
    Private LblSubtitulo As Label
    Private LblUsuario As Label
    Private LblSenha As Label
    Private LblMensagem As Label
    Private LblTentativas As Label

    Private TxtUsuario As TextBox
    Private TxtSenha As TextBox

    Private WithEvents ChkMostrarSenha As CheckBox
    Private WithEvents BtnEntrar As Button
    Private WithEvents BtnCancelar As Button

#End Region

#Region "Construtor"

    Public Sub New()

        InitializeComponent()

        ConfigurarFormulario()
        CriarInterface()

    End Sub

#End Region

#Region "Configuração do formulário"

    Private Sub ConfigurarFormulario()

        Me.SuspendLayout()
        Me.Controls.Clear()

        Me.Name =
            "FrmAcessoAdmin"

        Me.Text =
            "Acesso administrativo"

        Me.StartPosition =
            FormStartPosition.CenterParent

        Me.ClientSize =
            New Size(440, 420)

        Me.FormBorderStyle =
            FormBorderStyle.FixedSingle

        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.ShowIcon = False

        Me.BackColor =
            Color.FromArgb(28, 28, 28)

        Me.Font =
            New Font(
                "Segoe UI",
                10.0F,
                FontStyle.Regular)

    End Sub

#End Region

#Region "Criação da interface"

    Private Sub CriarInterface()

        CriarCabecalho()
        CriarPainelConteudo()
        CriarCampos()
        CriarBotoes()

        Me.AcceptButton =
            BtnEntrar

        Me.CancelButton =
            BtnCancelar

        Me.ResumeLayout(False)

        TxtUsuario.Focus()

    End Sub

    Private Sub CriarCabecalho()

        PnlTopo =
            New Panel With {
                .Name = "PnlTopo",
                .Dock = DockStyle.Top,
                .Height = 85,
                .BackColor = Color.FromArgb(134, 29, 29)
            }

        LblTitulo =
            New Label With {
                .Name = "LblTitulo",
                .Text = "ÁREA ADMINISTRATIVA",
                .AutoSize = True,
                .Location = New Point(25, 16),
                .ForeColor = Color.White,
                .BackColor = Color.Transparent,
                .Font = New Font(
                    "Segoe UI",
                    18.0F,
                    FontStyle.Bold)
            }

        LblSubtitulo =
            New Label With {
                .Name = "LblSubtitulo",
                .Text = "Acesso ao gerador de licenças",
                .AutoSize = True,
                .Location = New Point(28, 52),
                .ForeColor = Color.FromArgb(235, 235, 235),
                .BackColor = Color.Transparent,
                .Font = New Font(
                    "Segoe UI",
                    10.0F,
                    FontStyle.Regular)
            }

        PnlTopo.Controls.Add(
            LblTitulo)

        PnlTopo.Controls.Add(
            LblSubtitulo)

        Me.Controls.Add(
            PnlTopo)

    End Sub

    Private Sub CriarPainelConteudo()

        PnlConteudo =
            New Panel With {
                .Name = "PnlConteudo",
                .Location = New Point(25, 105),
                .Size = New Size(390, 285),
                .BackColor = Color.FromArgb(38, 38, 38),
                .BorderStyle = BorderStyle.FixedSingle
            }

        Me.Controls.Add(
            PnlConteudo)

    End Sub

    Private Sub CriarCampos()

        LblUsuario =
            CriarLabel(
                "LblUsuario",
                "Usuário administrativo",
                New Point(25, 22))

        TxtUsuario =
            CriarTextBox(
                "TxtUsuario",
                New Point(25, 50))

        TxtUsuario.CharacterCasing =
            CharacterCasing.Lower

        LblSenha =
            CriarLabel(
                "LblSenha",
                "Senha geral",
                New Point(25, 98))

        TxtSenha =
            CriarTextBox(
                "TxtSenha",
                New Point(25, 126))

        TxtSenha.UseSystemPasswordChar =
            True

        TxtSenha.MaxLength =
            100

        ChkMostrarSenha =
            New CheckBox With {
                .Name = "ChkMostrarSenha",
                .Text = "Mostrar senha",
                .Location = New Point(25, 163),
                .AutoSize = True,
                .ForeColor = Color.FromArgb(210, 210, 210),
                .BackColor = Color.Transparent,
                .Cursor = Cursors.Hand,
                .Font = New Font(
                    "Segoe UI",
                    9.0F,
                    FontStyle.Regular)
            }

        LblMensagem =
            New Label With {
                .Name = "LblMensagem",
                .Text = "",
                .Location = New Point(25, 190),
                .Size = New Size(338, 24),
                .ForeColor = Color.FromArgb(230, 90, 90),
                .BackColor = Color.Transparent,
                .Font = New Font(
                    "Segoe UI",
                    9.0F,
                    FontStyle.Bold)
            }

        LblTentativas =
            New Label With {
                .Name = "LblTentativas",
                .Text = "",
                .Location = New Point(25, 214),
                .Size = New Size(338, 20),
                .ForeColor = Color.FromArgb(170, 170, 170),
                .BackColor = Color.Transparent,
                .Font = New Font(
                    "Segoe UI",
                    8.5F,
                    FontStyle.Regular)
            }

        PnlConteudo.Controls.AddRange({
            LblUsuario,
            TxtUsuario,
            LblSenha,
            TxtSenha,
            ChkMostrarSenha,
            LblMensagem,
            LblTentativas
        })

    End Sub

    Private Sub CriarBotoes()

        BtnEntrar =
            New Button With {
                .Name = "BtnEntrar",
                .Text = "Acessar",
                .Location = New Point(25, 236),
                .Size = New Size(160, 36)
            }

        BtnCancelar =
            New Button With {
                .Name = "BtnCancelar",
                .Text = "Cancelar",
                .Location = New Point(203, 236),
                .Size = New Size(160, 36),
                .DialogResult = DialogResult.Cancel
            }

        AplicarEstiloBotao(
            BtnEntrar,
            True)

        AplicarEstiloBotao(
            BtnCancelar,
            False)

        PnlConteudo.Controls.Add(
            BtnEntrar)

        PnlConteudo.Controls.Add(
            BtnCancelar)

    End Sub

#End Region

#Region "Fábrica de controles"

    Private Function CriarLabel(
        nome As String,
        texto As String,
        localizacao As Point
    ) As Label

        Return New Label With {
            .Name = nome,
            .Text = texto,
            .Location = localizacao,
            .AutoSize = True,
            .ForeColor = Color.White,
            .BackColor = Color.Transparent,
            .Font = New Font(
                "Segoe UI",
                10.0F,
                FontStyle.Bold)
        }

    End Function

    Private Function CriarTextBox(
        nome As String,
        localizacao As Point
    ) As TextBox

        Return New TextBox With {
            .Name = nome,
            .Location = localizacao,
            .Size = New Size(338, 30),
            .BackColor = Color.FromArgb(48, 48, 48),
            .ForeColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle,
            .Font = New Font(
                "Segoe UI",
                11.0F,
                FontStyle.Regular)
        }

    End Function

    Private Sub AplicarEstiloBotao(
        botao As Button,
        destaque As Boolean)

        botao.FlatStyle =
            FlatStyle.Flat

        botao.FlatAppearance.BorderSize =
            0

        botao.BackColor =
            If(
                destaque,
                Color.FromArgb(134, 29, 29),
                Color.FromArgb(55, 55, 55))

        botao.FlatAppearance.MouseOverBackColor =
            If(
                destaque,
                Color.FromArgb(155, 35, 35),
                Color.FromArgb(75, 75, 75))

        botao.FlatAppearance.MouseDownBackColor =
            If(
                destaque,
                Color.FromArgb(105, 22, 22),
                Color.FromArgb(35, 35, 35))

        botao.ForeColor =
            Color.White

        botao.Font =
            New Font(
                "Segoe UI",
                9.5F,
                FontStyle.Bold)

        botao.Cursor =
            Cursors.Hand

        botao.UseVisualStyleBackColor =
            False

    End Sub

#End Region

#Region "Validação administrativa"

    Private Sub BtnEntrar_Click(
        sender As Object,
        e As EventArgs
    ) Handles BtnEntrar.Click

        LblMensagem.Text =
            ""

        Dim usuarioInformado As String =
            TxtUsuario.Text.Trim()

        Dim senhaInformada As String =
            TxtSenha.Text

        If usuarioInformado = "" Then

            ExibirErro(
                "Informe o usuário administrativo.",
                TxtUsuario)

            Exit Sub

        End If

        If senhaInformada = "" Then

            ExibirErro(
                "Informe a senha geral.",
                TxtSenha)

            Exit Sub

        End If

        Dim usuarioCorreto As Boolean =
            String.Equals(
                usuarioInformado,
                USUARIO_ADMINISTRADOR,
                StringComparison.OrdinalIgnoreCase)

        Dim hashInformado As String =
            GerarHashSHA256(
                senhaInformada)

        Dim senhaCorreta As Boolean =
            CompararHashes(
                hashInformado,
                HASH_SENHA_ADMINISTRADOR)

        If usuarioCorreto AndAlso
           senhaCorreta Then

            Me.DialogResult =
                DialogResult.OK

            Me.Close()
            Exit Sub

        End If

        RegistrarTentativaInvalida()

    End Sub

    Private Sub RegistrarTentativaInvalida()

        tentativasRealizadas += 1

        Dim restantes As Integer =
            MAXIMO_TENTATIVAS -
            tentativasRealizadas

        LblMensagem.ForeColor =
            Color.FromArgb(230, 90, 90)

        LblMensagem.Text =
            "Usuário ou senha administrativa incorretos."

        TxtSenha.Clear()
        TxtSenha.Focus()

        If restantes > 0 Then

            LblTentativas.Text =
                $"Tentativas restantes: {restantes}"

            Exit Sub

        End If

        BtnEntrar.Enabled = False
        TxtUsuario.Enabled = False
        TxtSenha.Enabled = False
        ChkMostrarSenha.Enabled = False

        LblTentativas.Text =
            "Limite de tentativas atingido."

    End Sub

    Private Sub ExibirErro(
        mensagem As String,
        controle As Control)

        LblMensagem.ForeColor =
            Color.FromArgb(230, 90, 90)

        LblMensagem.Text =
            mensagem

        controle.Focus()

    End Sub

#End Region

#Region "Segurança da senha"

    Private Function GerarHashSHA256(
        valor As String
    ) As String

        Dim bytesEntrada As Byte() =
            Encoding.UTF8.GetBytes(
                valor)

        Dim bytesHash As Byte() =
            SHA256.HashData(
                bytesEntrada)

        Return Convert.ToHexString(
            bytesHash)

    End Function

    Private Function CompararHashes(
        hashInformado As String,
        hashEsperado As String
    ) As Boolean

        Dim bytesInformado As Byte() =
            Encoding.UTF8.GetBytes(
                hashInformado)

        Dim bytesEsperado As Byte() =
            Encoding.UTF8.GetBytes(
                hashEsperado)

        If bytesInformado.Length <>
           bytesEsperado.Length Then

            Return False

        End If

        Return CryptographicOperations.FixedTimeEquals(
            bytesInformado,
            bytesEsperado)

    End Function

#End Region

#Region "Eventos"

    Private Sub ChkMostrarSenha_CheckedChanged(
        sender As Object,
        e As EventArgs
    ) Handles ChkMostrarSenha.CheckedChanged

        TxtSenha.UseSystemPasswordChar =
            Not ChkMostrarSenha.Checked

        TxtSenha.Focus()

        TxtSenha.SelectionStart =
            TxtSenha.TextLength

    End Sub

    Private Sub BtnCancelar_Click(
        sender As Object,
        e As EventArgs
    ) Handles BtnCancelar.Click

        Me.DialogResult =
            DialogResult.Cancel

        Me.Close()

    End Sub

#End Region

End Class