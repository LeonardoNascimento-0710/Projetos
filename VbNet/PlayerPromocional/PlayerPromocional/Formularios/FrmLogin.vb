Imports System.Globalization
Imports System.Security.Cryptography
Imports System.Text
Public Class FrmLogin

#Region "Constantes de acesso"

    Private Const USUARIO_AUTORIZADO As String =
        "admin"

    Private Const SEGREDO_LICENCA_BASE64 As String =
    "toKe01djhLfdCjF3i57oq3m7vV0SkDP3WBGMUADnPEU="

    Private Const MAXIMO_TENTATIVAS As Integer = 5

#End Region

#Region "Campos"

    Private ReadOnly caminhoArquivoLicenca As String =
    IO.Path.Combine(
        AppContext.BaseDirectory,
        "licenca.txt")

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
    Private WithEvents BtnSair As Button

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
            "FrmLogin"

        Me.Text =
            "Acesso ao sistema"

        Me.StartPosition =
            FormStartPosition.CenterScreen

        Me.ClientSize =
            New Size(440, 430)

        Me.FormBorderStyle =
            FormBorderStyle.FixedSingle

        Me.MaximizeBox =
            False

        Me.MinimizeBox =
            False

        Me.ShowIcon =
            False

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
        CriarCamposLogin()
        CriarBotoes()

        Me.AcceptButton =
            BtnEntrar

        Me.CancelButton =
            BtnSair

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
            .Text = "ATIVAÇÃO DO SISTEMA",
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
            .Text = "Informe os dados da licença",
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
                .Size = New Size(390, 295),
                .BackColor = Color.FromArgb(38, 38, 38),
                .BorderStyle = BorderStyle.FixedSingle
            }

        Me.Controls.Add(
            PnlConteudo)

    End Sub

    Private Sub CriarCamposLogin()

        LblUsuario =
    CriarLabel(
        "LblUsuario",
        "Nome do cliente",
        New Point(25, 25))

        TxtUsuario =
            CriarTextBox(
                "TxtUsuario",
                New Point(25, 52))

        TxtUsuario.CharacterCasing =
            CharacterCasing.Lower

        LblSenha =
    CriarLabel(
        "LblSenha",
        "Chave de ativação",
        New Point(25, 100))

        TxtSenha =
            CriarTextBox(
                "TxtSenha",
                New Point(25, 127))

        TxtSenha.UseSystemPasswordChar =
            True

        TxtSenha.MaxLength =
    500

        ChkMostrarSenha =
            New CheckBox With {
                .Name = "ChkMostrarChave",
                .Text = "Mostrar Chave",
                .Location = New Point(25, 165),
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
                .Location = New Point(25, 195),
                .Size = New Size(338, 25),
                .ForeColor = Color.FromArgb(230, 90, 90),
                .BackColor = Color.Transparent,
                .TextAlign = ContentAlignment.MiddleLeft,
                .Font = New Font(
                    "Segoe UI",
                    9.0F,
                    FontStyle.Bold)
            }

        LblTentativas =
            New Label With {
                .Name = "LblTentativas",
                .Text = "",
                .Location = New Point(25, 220),
                .Size = New Size(338, 22),
                .ForeColor = Color.FromArgb(170, 170, 170),
                .BackColor = Color.Transparent,
                .TextAlign = ContentAlignment.MiddleLeft,
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
                .Text = "Entrar",
                .Location = New Point(25, 245),
                .Size = New Size(160, 38)
            }

        BtnSair =
            New Button With {
                .Name = "BtnSair",
                .Text = "Sair",
                .Location = New Point(203, 245),
                .Size = New Size(160, 38),
                .DialogResult = DialogResult.Cancel
            }

        AplicarEstiloBotao(
            BtnEntrar,
            True)

        AplicarEstiloBotao(
            BtnSair,
            False)

        PnlConteudo.Controls.Add(
            BtnEntrar)

        PnlConteudo.Controls.Add(
            BtnSair)

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

#Region "Validação de acesso"

    Private Sub BtnEntrar_Click(
    sender As Object,
    e As EventArgs
) Handles BtnEntrar.Click

        LblMensagem.Text = ""

        Dim nomeCliente As String =
        TxtUsuario.Text.Trim()

        Dim chaveAtivacao As String =
        TxtSenha.Text.Trim()

        If nomeCliente = "" Then

            ExibirErro(
            "Informe o nome do cliente.",
            TxtUsuario)

            Exit Sub

        End If

        If chaveAtivacao = "" Then

            ExibirErro(
            "Informe a chave de ativação.",
            TxtSenha)

            Exit Sub

        End If

        Dim validade As DateTime
        Dim mensagemValidacao As String = ""

        Dim licencaValida As Boolean =
        ValidarLicenca(
            nomeCliente,
            chaveAtivacao,
            validade,
            mensagemValidacao)

        If licencaValida Then

            LblMensagem.ForeColor =
            Color.FromArgb(90, 200, 120)

            LblMensagem.Text =
            $"Licença válida até {validade:dd/MM/yyyy}."

            Try

                SalvarLicencaLocal(
        nomeCliente,
        chaveAtivacao)

            Catch ex As Exception

                MessageBox.Show(
        "A licença foi validada, mas não foi possível salvá-la." &
        Environment.NewLine &
        ex.Message,
        "Aviso",
        MessageBoxButtons.OK,
        MessageBoxIcon.Warning)

            End Try
            Me.DialogResult =
            DialogResult.OK

            Me.Close()

            Exit Sub

        End If

        RegistrarTentativaInvalida(
        mensagemValidacao)

    End Sub

    Private Sub RegistrarTentativaInvalida(
    mensagem As String)

        tentativasRealizadas += 1

        Dim tentativasRestantes As Integer =
        MAXIMO_TENTATIVAS -
        tentativasRealizadas

        LblMensagem.ForeColor =
        Color.FromArgb(230, 90, 90)

        LblMensagem.Text =
        mensagem

        TxtSenha.Clear()
        TxtSenha.Focus()

        If tentativasRestantes > 0 Then

            LblTentativas.Text =
            $"Tentativas restantes: {tentativasRestantes}"

            Exit Sub

        End If

        BtnEntrar.Enabled = False
        TxtUsuario.Enabled = False
        TxtSenha.Enabled = False
        ChkMostrarSenha.Enabled = False

        LblTentativas.Text =
        "Limite de tentativas atingido."

        MessageBox.Show(
        "O limite de tentativas de ativação foi atingido." &
        Environment.NewLine &
        "O sistema será encerrado.",
        "Ativação bloqueada",
        MessageBoxButtons.OK,
        MessageBoxIcon.Error)

        Me.DialogResult =
        DialogResult.Cancel

        Me.Close()

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

#Region "Hash da senha"

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
        hashAutorizado As String
    ) As Boolean

        Dim bytesInformado As Byte() =
            Encoding.UTF8.GetBytes(
                hashInformado)

        Dim bytesAutorizado As Byte() =
            Encoding.UTF8.GetBytes(
                hashAutorizado)

        If bytesInformado.Length <>
           bytesAutorizado.Length Then

            Return False

        End If

        Return CryptographicOperations.FixedTimeEquals(
            bytesInformado,
            bytesAutorizado)

    End Function

#End Region

#Region "Eventos da interface"

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

    Private Sub BtnSair_Click(
        sender As Object,
        e As EventArgs
    ) Handles BtnSair.Click

        Me.DialogResult =
            DialogResult.Cancel

        Me.Close()

    End Sub

#Region "Validação da licença"

    Private Function ValidarLicenca(
    nomeClienteInformado As String,
    chaveAtivacao As String,
    ByRef validade As DateTime,
    ByRef mensagem As String
) As Boolean

        validade = DateTime.MinValue
        mensagem = "Chave de ativação inválida."

        Try

            Dim partesChave As String() =
            chaveAtivacao.Trim().Split("."c)

            If partesChave.Length <> 2 Then
                Return False
            End If

            Dim dadosLicenca As Byte() =
            DecodificarBase64Url(
                partesChave(0))

            Dim assinaturaRecebida As Byte() =
            DecodificarBase64Url(
                partesChave(1))

            Dim segredo As Byte() =
            Convert.FromBase64String(
                SEGREDO_LICENCA_BASE64)

            Dim assinaturaEsperada As Byte()

            Using hmac As New HMACSHA256(segredo)

                assinaturaEsperada =
                hmac.ComputeHash(
                    dadosLicenca)

            End Using

            If assinaturaRecebida.Length <>
           assinaturaEsperada.Length Then

                Return False

            End If

            If Not CryptographicOperations.FixedTimeEquals(
            assinaturaRecebida,
            assinaturaEsperada) Then

                Return False

            End If

            Dim conteudoLicenca As String =
            Encoding.UTF8.GetString(
                dadosLicenca)

            Dim dados As String() =
            conteudoLicenca.Split("|"c)

            If dados.Length <> 2 Then
                Return False
            End If

            Dim clienteDaLicenca As String =
            dados(0)

            Dim clienteInformadoNormalizado As String =
            NormalizarNomeCliente(
                nomeClienteInformado)

            If Not String.Equals(
            clienteDaLicenca,
            clienteInformadoNormalizado,
            StringComparison.Ordinal) Then

                mensagem =
                "A chave não pertence ao cliente informado."

                Return False

            End If

            If Not DateTime.TryParseExact(
            dados(1),
            "yyyyMMdd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            validade) Then

                Return False

            End If

            If Date.Today > validade.Date Then

                mensagem =
                $"A licença venceu em {validade:dd/MM/yyyy}."

                Return False

            End If

            mensagem = ""
            Return True

        Catch

            mensagem =
            "Chave de ativação inválida."

            Return False

        End Try

    End Function

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

    Private Function DecodificarBase64Url(
    valor As String
) As Byte()

        Dim base64 As String =
        valor.Replace("-", "+").
        Replace("_", "/")

        Select Case base64.Length Mod 4

            Case 2
                base64 &= "=="

            Case 3
                base64 &= "="

        End Select

        Return Convert.FromBase64String(
        base64)

    End Function

#End Region

#End Region

#Region "Acesso administrativo"

    Protected Overrides Function ProcessCmdKey(
    ByRef msg As Message,
    keyData As Keys
) As Boolean

        If keyData =
       (Keys.Control Or
        Keys.Shift Or
        Keys.F12) Then

            AbrirAreaAdministrativa()

            Return True

        End If

        Return MyBase.ProcessCmdKey(
        msg,
        keyData)

    End Function

    Private Sub AbrirAreaAdministrativa()

        Using frmAcesso As New FrmAcessoAdmin()

            If frmAcesso.ShowDialog(Me) <>
           DialogResult.OK Then

                Exit Sub

            End If

        End Using

        Using frmGerador As New FrmGeradorLicencas()

            frmGerador.ShowDialog(Me)

        End Using

    End Sub

#End Region

#Region "Licença salva localmente"

    Private Sub SalvarLicencaLocal(
    nomeCliente As String,
    chaveAtivacao As String)

        Dim conteudo As String =
        nomeCliente.Trim() &
        Environment.NewLine &
        chaveAtivacao.Trim()

        IO.File.WriteAllText(
        caminhoArquivoLicenca,
        conteudo,
        Encoding.UTF8)

    End Sub

    Private Function TentarValidarLicencaSalva() As Boolean

        If Not IO.File.Exists(
        caminhoArquivoLicenca) Then

            Return False

        End If

        Try

            Dim linhas As String() =
            IO.File.ReadAllLines(
                caminhoArquivoLicenca,
                Encoding.UTF8)

            If linhas.Length < 2 Then

                LblMensagem.ForeColor =
                Color.FromArgb(230, 90, 90)

                LblMensagem.Text =
                "O arquivo de licença está incompleto."

                Return False

            End If

            Dim nomeCliente As String =
            linhas(0).Trim()

            Dim chaveAtivacao As String =
            linhas(1).Trim()

            If nomeCliente = "" OrElse
           chaveAtivacao = "" Then

                Return False

            End If

            Dim validade As DateTime
            Dim mensagemValidacao As String = ""

            Dim licencaValida As Boolean =
            ValidarLicenca(
                nomeCliente,
                chaveAtivacao,
                validade,
                mensagemValidacao)

            TxtUsuario.Text =
            nomeCliente

            TxtSenha.Text =
            chaveAtivacao

            If Not licencaValida Then

                LblMensagem.ForeColor =
                Color.FromArgb(230, 90, 90)

                LblMensagem.Text =
                mensagemValidacao

                Return False

            End If

            Return True

        Catch ex As Exception

            LblMensagem.ForeColor =
            Color.FromArgb(230, 90, 90)

            LblMensagem.Text =
            "Não foi possível ler a licença salva."

            Return False

        End Try

    End Function

#End Region

    Private Sub FrmLogin_Shown(
    sender As Object,
    e As EventArgs
) Handles MyBase.Shown

        If Not TentarValidarLicencaSalva() Then
            Exit Sub
        End If

        Me.DialogResult =
            DialogResult.OK

        Me.Close()

    End Sub

End Class