Imports System.Globalization
Imports System.Security.Cryptography
Imports System.Text

Public Class FrmGeradorLicencas

#Region "Configuração da licença"

    ' Deve ser exatamente o mesmo valor utilizado no FrmLogin.
    Private Const SEGREDO_LICENCA_BASE64 As String =
        "toKe01djhLfdCjF3i57oq3m7vV0SkDP3WBGMUADnPEU="

#End Region

#Region "Controles"

    Private PnlTopo As Panel
    Private PnlConteudo As Panel

    Private LblTitulo As Label
    Private LblSubtitulo As Label

    Private LblCliente As Label
    Private LblValidade As Label
    Private LblChave As Label
    Private LblResumo As Label

    Private TxtCliente As TextBox
    Private DtpValidade As DateTimePicker
    Private TxtChave As TextBox

    Private WithEvents BtnGerar As Button
    Private WithEvents BtnCopiar As Button
    Private WithEvents BtnLimpar As Button
    Private WithEvents BtnFechar As Button

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
            "FrmGeradorLicencas"

        Me.Text =
            "Gerador de licenças"

        Me.StartPosition =
            FormStartPosition.CenterParent

        Me.ClientSize =
            New Size(720, 550)

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
            BtnGerar

        Me.CancelButton =
            BtnFechar

        Me.ResumeLayout(False)

        TxtCliente.Focus()

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
                .Text = "GERADOR DE LICENÇAS",
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
                .Text = "Crie uma licença vinculada ao cliente e à validade",
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
                .Size = New Size(670, 415),
                .BackColor = Color.FromArgb(38, 38, 38),
                .BorderStyle = BorderStyle.FixedSingle
            }

        Me.Controls.Add(
            PnlConteudo)

    End Sub

    Private Sub CriarCampos()

        LblCliente =
            CriarLabel(
                "LblCliente",
                "Nome do cliente",
                New Point(25, 22))

        TxtCliente =
            New TextBox With {
                .Name = "TxtCliente",
                .Location = New Point(25, 50),
                .Size = New Size(615, 31),
                .BackColor = Color.FromArgb(48, 48, 48),
                .ForeColor = Color.White,
                .BorderStyle = BorderStyle.FixedSingle,
                .Font = New Font(
                    "Segoe UI",
                    11.0F,
                    FontStyle.Regular),
                .MaxLength = 150
            }

        LblValidade =
            CriarLabel(
                "LblValidade",
                "Validade da licença",
                New Point(25, 100))

        DtpValidade =
            New DateTimePicker With {
                .Name = "DtpValidade",
                .Location = New Point(25, 128),
                .Size = New Size(220, 31),
                .Format = DateTimePickerFormat.Custom,
                .CustomFormat = "dd/MM/yyyy",
                .Value = DateTime.Today.AddYears(1),
                .MinDate = DateTime.Today,
                .Font = New Font(
                    "Segoe UI",
                    10.0F,
                    FontStyle.Regular)
            }

        LblChave =
            CriarLabel(
                "LblChave",
                "Chave de ativação",
                New Point(25, 180))

        TxtChave =
            New TextBox With {
                .Name = "TxtChave",
                .Location = New Point(25, 208),
                .Size = New Size(615, 100),
                .BackColor = Color.FromArgb(45, 45, 45),
                .ForeColor = Color.White,
                .BorderStyle = BorderStyle.FixedSingle,
                .Font = New Font(
                    "Consolas",
                    9.5F,
                    FontStyle.Regular),
                .Multiline = True,
                .ReadOnly = True,
                .WordWrap = True,
                .ScrollBars = ScrollBars.Vertical
            }

        LblResumo =
            New Label With {
                .Name = "LblResumo",
                .Text = "Preencha o cliente e a validade.",
                .Location = New Point(25, 318),
                .Size = New Size(615, 42),
                .ForeColor = Color.FromArgb(190, 190, 190),
                .BackColor = Color.Transparent,
                .Font = New Font(
                    "Segoe UI",
                    9.0F,
                    FontStyle.Regular)
            }

        PnlConteudo.Controls.AddRange({
            LblCliente,
            TxtCliente,
            LblValidade,
            DtpValidade,
            LblChave,
            TxtChave,
            LblResumo
        })

    End Sub

    Private Sub CriarBotoes()

        BtnGerar =
            New Button With {
                .Name = "BtnGerar",
                .Text = "Gerar licença",
                .Location = New Point(25, 365),
                .Size = New Size(140, 36)
            }

        BtnCopiar =
            New Button With {
                .Name = "BtnCopiar",
                .Text = "Copiar chave",
                .Location = New Point(175, 365),
                .Size = New Size(140, 36),
                .Enabled = False
            }

        BtnLimpar =
            New Button With {
                .Name = "BtnLimpar",
                .Text = "Limpar",
                .Location = New Point(325, 365),
                .Size = New Size(120, 36)
            }

        BtnFechar =
            New Button With {
                .Name = "BtnFechar",
                .Text = "Fechar",
                .Location = New Point(520, 365),
                .Size = New Size(120, 36),
                .DialogResult = DialogResult.Cancel
            }

        AplicarEstiloBotao(
            BtnGerar,
            True)

        AplicarEstiloBotao(
            BtnCopiar,
            False)

        AplicarEstiloBotao(
            BtnLimpar,
            False)

        AplicarEstiloBotao(
            BtnFechar,
            False)

        PnlConteudo.Controls.AddRange({
            BtnGerar,
            BtnCopiar,
            BtnLimpar,
            BtnFechar
        })

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

#Region "Geração da licença"

    Private Sub BtnGerar_Click(
        sender As Object,
        e As EventArgs
    ) Handles BtnGerar.Click

        Dim nomeCliente As String =
            TxtCliente.Text.Trim()

        If nomeCliente = "" Then

            MessageBox.Show(
                "Informe o nome do cliente.",
                "Gerador de licenças",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning)

            TxtCliente.Focus()
            Exit Sub

        End If

        Dim validade As DateTime =
            DtpValidade.Value.Date

        If validade < DateTime.Today Then

            MessageBox.Show(
                "A validade não pode ser anterior à data atual.",
                "Gerador de licenças",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning)

            DtpValidade.Focus()
            Exit Sub

        End If

        Try

            Dim clienteNormalizado As String =
                NormalizarNomeCliente(
                    nomeCliente)

            Dim dadosLicencaTexto As String =
                clienteNormalizado &
                "|" &
                validade.ToString(
                    "yyyyMMdd",
                    CultureInfo.InvariantCulture)

            Dim dadosLicenca As Byte() =
                Encoding.UTF8.GetBytes(
                    dadosLicencaTexto)

            Dim segredo As Byte() =
                Convert.FromBase64String(
                    SEGREDO_LICENCA_BASE64)

            Dim assinatura As Byte()

            Using hmac As New HMACSHA256(segredo)

                assinatura =
                    hmac.ComputeHash(
                        dadosLicenca)

            End Using

            Dim chaveAtivacao As String =
                CodificarBase64Url(dadosLicenca) &
                "." &
                CodificarBase64Url(assinatura)

            TxtChave.Text =
                chaveAtivacao

            BtnCopiar.Enabled =
                True

            LblResumo.ForeColor =
                Color.FromArgb(90, 200, 120)

            LblResumo.Text =
                $"Licença gerada para: {nomeCliente}" &
                Environment.NewLine &
                $"Validade: {validade:dd/MM/yyyy}"

        Catch ex As Exception

            BtnCopiar.Enabled =
                False

            LblResumo.ForeColor =
                Color.FromArgb(230, 90, 90)

            LblResumo.Text =
                "Não foi possível gerar a licença."

            MessageBox.Show(
                "Não foi possível gerar a licença." &
                Environment.NewLine &
                ex.Message,
                "Erro",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error)

        End Try

    End Sub

#End Region

#Region "Normalização do cliente"

    Private Function NormalizarNomeCliente(
        valor As String
    ) As String

        Dim textoDecomposto As String =
            valor.Trim().
            ToUpperInvariant().
            Normalize(
                NormalizationForm.FormD)

        Dim resultado As New StringBuilder()

        For Each caractere As Char In textoDecomposto

            Dim categoria As UnicodeCategory =
                CharUnicodeInfo.GetUnicodeCategory(
                    caractere)

            If categoria <>
               UnicodeCategory.NonSpacingMark Then

                resultado.Append(
                    caractere)

            End If

        Next

        Dim textoSemAcentos As String =
            resultado.ToString().
            Normalize(
                NormalizationForm.FormC)

        Dim separadores As Char() = {
            " "c,
            ChrW(9),
            ChrW(10),
            ChrW(13)
        }

        Dim palavras As String() =
            textoSemAcentos.Split(
                separadores,
                StringSplitOptions.RemoveEmptyEntries)

        Return String.Join(
            " ",
            palavras)

    End Function

#End Region

#Region "Codificação da chave"

    Private Function CodificarBase64Url(
        dados As Byte()
    ) As String

        Return Convert.ToBase64String(dados).
            TrimEnd("="c).
            Replace("+", "-").
            Replace("/", "_")

    End Function

#End Region

#Region "Eventos dos botões"

    Private Sub BtnCopiar_Click(
        sender As Object,
        e As EventArgs
    ) Handles BtnCopiar.Click

        If String.IsNullOrWhiteSpace(
            TxtChave.Text) Then

            Exit Sub

        End If

        Clipboard.SetText(
            TxtChave.Text)

        LblResumo.ForeColor =
            Color.FromArgb(90, 200, 120)

        LblResumo.Text =
            "Chave copiada para a área de transferência."

    End Sub

    Private Sub BtnLimpar_Click(
        sender As Object,
        e As EventArgs
    ) Handles BtnLimpar.Click

        TxtCliente.Clear()
        TxtChave.Clear()

        DtpValidade.Value =
            DateTime.Today.AddYears(1)

        BtnCopiar.Enabled =
            False

        LblResumo.ForeColor =
            Color.FromArgb(190, 190, 190)

        LblResumo.Text =
            "Preencha o cliente e a validade."

        TxtCliente.Focus()

    End Sub

    Private Sub BtnFechar_Click(
        sender As Object,
        e As EventArgs
    ) Handles BtnFechar.Click

        Me.Close()

    End Sub

#End Region

End Class