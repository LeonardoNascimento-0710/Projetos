Imports System.Drawing
Imports System.IO
Imports System.Reflection
Imports System.Windows.Forms

Public Class FrmSplash
    Inherits Form

    Private WithEvents TmrAbrir As New Timer()

    Private _barraFundo As Panel
    Private _barraProgresso As Panel
    Private _lblCarregando As Label
    Private _picLogo As PictureBox

    Private _progressoAtual As Integer
    Private _formPrincipalAberto As Boolean

    Public Sub New()

        ConfigurarFormulario()
        MontarInterface()

        TmrAbrir.Interval = 30

    End Sub

    Private Sub ConfigurarFormulario()

        Me.SuspendLayout()

        Me.FormBorderStyle =
            FormBorderStyle.None

        Me.StartPosition =
            FormStartPosition.CenterScreen

        Me.ClientSize =
            New Size(
                800,
                500)

        Me.BackColor =
            Color.FromArgb(
                248,
                248,
                248)

        Me.ShowInTaskbar =
            False

        Me.TopMost =
            True

        Me.DoubleBuffered =
            True

        Me.ResumeLayout(
            False)

    End Sub

    Private Sub MontarInterface()

        Me.SuspendLayout()

        Try

            Me.Controls.Clear()

            '==================================================
            ' FUNDO PRINCIPAL
            '==================================================

            Dim painelFundo As New Panel With {
                .Dock = DockStyle.Fill,
                .BackColor = Color.FromArgb(
                    248,
                    248,
                    248)
            }

            Me.Controls.Add(
                painelFundo)

            '==================================================
            ' FAIXA SUPERIOR
            '==================================================

            Dim faixaSuperior As New Panel With {
                .Dock = DockStyle.Top,
                .Height = 5,
                .BackColor = Color.FromArgb(
                    145,
                    25,
                    25)
            }

            painelFundo.Controls.Add(
                faixaSuperior)

            '==================================================
            ' RODAPÉ
            '==================================================

            Dim faixaInferior As New Panel With {
                .Dock = DockStyle.Bottom,
                .Height = 78,
                .BackColor = Color.FromArgb(
                    24,
                    24,
                    24)
            }

            painelFundo.Controls.Add(
                faixaInferior)

            _barraFundo =
                New Panel With {
                    .Dock = DockStyle.Bottom,
                    .Height = 7,
                    .BackColor = Color.FromArgb(
                        68,
                        68,
                        68)
                }

            faixaInferior.Controls.Add(
                _barraFundo)

            _barraProgresso =
                New Panel With {
                    .Dock = DockStyle.Left,
                    .Width = 0,
                    .BackColor = Color.FromArgb(
                        145,
                        25,
                        25)
                }

            _barraFundo.Controls.Add(
                _barraProgresso)

            _lblCarregando =
                New Label With {
                    .Text =
                        "Inicializando TacticalStudio...",
                    .ForeColor =
                        Color.White,
                    .BackColor =
                        Color.Transparent,
                    .Font =
                        New Font(
                            "Segoe UI",
                            10.0F,
                            FontStyle.Regular),
                    .Dock =
                        DockStyle.Left,
                    .Width =
                        390,
                    .Padding =
                        New Padding(
                            18,
                            0,
                            0,
                            7),
                    .TextAlign =
                        ContentAlignment.MiddleLeft
                }

            faixaInferior.Controls.Add(
                _lblCarregando)

            Dim lblVersao As New Label With {
                .Text =
                    "Versão " &
                    ObterVersaoLimpa(),
                .ForeColor =
                    Color.Gainsboro,
                .BackColor =
                    Color.Transparent,
                .Font =
                    New Font(
                        "Segoe UI",
                        9.5F,
                        FontStyle.Regular),
                .Dock =
                    DockStyle.Right,
                .Width =
                    190,
                .Padding =
                    New Padding(
                        0,
                        0,
                        18,
                        7),
                .TextAlign =
                    ContentAlignment.MiddleRight
            }

            faixaInferior.Controls.Add(
                lblVersao)

            _barraFundo.BringToFront()

            '==================================================
            ' LOGO DA ORBIT
            '==================================================

            _picLogo =
    New PictureBox With {
        .Image =
            CarregarLogoOrbit(),
        .SizeMode =
            PictureBoxSizeMode.Zoom,
        .BackColor =
            Color.Transparent,
        .Location =
            New Point(
                0,
                38),
        .Size =
            New Size(
                190,
                230),
        .Anchor =
            AnchorStyles.Top Or
            AnchorStyles.Left Or
            AnchorStyles.Right
    }

            painelFundo.Controls.Add(
                _picLogo)

            '==================================================
            ' NOME DO SISTEMA
            '==================================================

            Dim lblSistema As New Label With {
                .Text =
                    "TacticalStudio",
                .ForeColor =
                    Color.FromArgb(
                        20,
                        20,
                        20),
                .BackColor =
                    Color.Transparent,
                .Font =
                    New Font(
                        "Segoe UI",
                        21.0F,
                        FontStyle.Bold),
                .Location =
                    New Point(
                        0,
                        292),
                .Size =
                    New Size(
                        200,
                        48),
                .Anchor =
                    AnchorStyles.Top Or
                    AnchorStyles.Left Or
                    AnchorStyles.Right,
                .TextAlign =
                    ContentAlignment.MiddleCenter
            }

            painelFundo.Controls.Add(
                lblSistema)

            Dim lblDescricao As New Label With {
                .Text =
                    "Editor profissional de táticas e exercícios de futebol",
                .ForeColor =
                    Color.FromArgb(
                        75,
                        75,
                        75),
                .BackColor =
                    Color.Transparent,
                .Font =
                    New Font(
                        "Segoe UI",
                        10.0F,
                        FontStyle.Regular),
                .Location =
                    New Point(
                        0,
                        339),
                .Size =
                    New Size(
                        200,
                        28),
                .Anchor =
                    AnchorStyles.Top Or
                    AnchorStyles.Left Or
                    AnchorStyles.Right,
                .TextAlign =
                    ContentAlignment.MiddleCenter
            }

            painelFundo.Controls.Add(
                lblDescricao)

            Dim lblEmpresa As New Label With {
                .Text =
                    "Desenvolvido por ORBIT  •  " &
                    "Tecnologia que gira em torno do futuro.",
                .ForeColor =
                    Color.FromArgb(
                        105,
                        105,
                        105),
                .BackColor =
                    Color.Transparent,
                .Font =
                    New Font(
                        "Segoe UI",
                        9.5F,
                        FontStyle.Regular),
                .Location =
                    New Point(
                        0,
                        380),
                .Size =
                    New Size(
                        200,
                        28),
                .Anchor =
                    AnchorStyles.Top Or
                    AnchorStyles.Left Or
                    AnchorStyles.Right,
                .TextAlign =
                    ContentAlignment.MiddleCenter
            }

            painelFundo.Controls.Add(
                lblEmpresa)

        Finally

            Me.ResumeLayout(
                True)

        End Try

    End Sub

    Private Function CarregarLogoOrbit() As Image

        Dim caminhoLogo As String =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Assets",
                "Icones",
                "IconeOrbit.jpg")

        If Not File.Exists(
            caminhoLogo) Then

            Return CriarImagemLogoAusente()

        End If

        Try

            Using imagemOriginal As Image =
                Image.FromFile(
                    caminhoLogo)

                Using bitmapOriginal As New Bitmap(
                    imagemOriginal)

                    Return RecortarMargensClaras(
                        bitmapOriginal)

                End Using

            End Using

        Catch

            Return CriarImagemLogoAusente()

        End Try

    End Function

    Private Function RecortarMargensClaras(
        imagem As Bitmap) As Bitmap

        If imagem Is Nothing Then
            Return Nothing
        End If

        Dim menorX As Integer =
            imagem.Width

        Dim menorY As Integer =
            imagem.Height

        Dim maiorX As Integer =
            -1

        Dim maiorY As Integer =
            -1

        For y As Integer =
            0 To imagem.Height - 1

            For x As Integer =
                0 To imagem.Width - 1

                Dim corPixel As Color =
                    imagem.GetPixel(
                        x,
                        y)

                Dim pixelClaro As Boolean =
                    corPixel.R >= 242 AndAlso
                    corPixel.G >= 242 AndAlso
                    corPixel.B >= 242

                If corPixel.A > 10 AndAlso
                   Not pixelClaro Then

                    menorX =
                        Math.Min(
                            menorX,
                            x)

                    menorY =
                        Math.Min(
                            menorY,
                            y)

                    maiorX =
                        Math.Max(
                            maiorX,
                            x)

                    maiorY =
                        Math.Max(
                            maiorY,
                            y)

                End If

            Next

        Next

        If maiorX < menorX OrElse
           maiorY < menorY Then

            Return New Bitmap(
                imagem)

        End If

        Dim margemHorizontal As Integer =
            25

        Dim margemVertical As Integer =
            20

        menorX =
            Math.Max(
                0,
                menorX -
                margemHorizontal)

        menorY =
            Math.Max(
                0,
                menorY -
                margemVertical)

        maiorX =
            Math.Min(
                imagem.Width - 1,
                maiorX +
                margemHorizontal)

        maiorY =
            Math.Min(
                imagem.Height - 1,
                maiorY +
                margemVertical)

        Dim areaRecorte As New Rectangle(
            menorX,
            menorY,
            maiorX - menorX + 1,
            maiorY - menorY + 1)

        Return imagem.Clone(
            areaRecorte,
            Imaging.PixelFormat.Format32bppArgb)

    End Function

    Private Function CriarImagemLogoAusente() As Image

        Dim imagem As New Bitmap(
            620,
            190)

        Using g As Graphics =
            Graphics.FromImage(
                imagem)

            g.Clear(
                Color.FromArgb(
                    248,
                    248,
                    248))

            Using fonte As New Font(
                "Segoe UI",
                28.0F,
                FontStyle.Bold)

                Using pincel As New SolidBrush(
                    Color.FromArgb(
                        145,
                        25,
                        25))

                    Dim texto As String =
                        "ORBIT"

                    Dim tamanhoTexto As SizeF =
                        g.MeasureString(
                            texto,
                            fonte)

                    g.DrawString(
                        texto,
                        fonte,
                        pincel,
                        (imagem.Width -
                         tamanhoTexto.Width) / 2.0F,
                        (imagem.Height -
                         tamanhoTexto.Height) / 2.0F)

                End Using

            End Using

        End Using

        Return imagem

    End Function

    Private Function ObterVersaoLimpa() As String

        Try

            Dim assemblyAtual As Assembly =
                Assembly.GetExecutingAssembly()

            Dim versao As Version =
                assemblyAtual.
                    GetName().
                    Version

            If versao Is Nothing Then
                Return "1.0.0"
            End If

            Dim build As Integer =
                Math.Max(
                    0,
                    versao.Build)

            Return String.Format(
                "{0}.{1}.{2}",
                versao.Major,
                versao.Minor,
                build)

        Catch

            Dim textoVersao As String =
                Application.ProductVersion

            If String.IsNullOrWhiteSpace(
                textoVersao) Then

                Return "1.0.0"

            End If

            Dim posicaoMetadados As Integer =
                textoVersao.IndexOf(
                    "+"c)

            If posicaoMetadados >= 0 Then

                textoVersao =
                    textoVersao.Substring(
                        0,
                        posicaoMetadados)

            End If

            Return textoVersao.Trim()

        End Try

    End Function

    Private Sub AtualizarMensagemCarregamento()

        If _lblCarregando Is Nothing Then
            Exit Sub
        End If

        Select Case _progressoAtual

            Case 0 To 19

                _lblCarregando.Text =
                    "Inicializando TacticalStudio..."

            Case 20 To 39

                _lblCarregando.Text =
                    "Carregando configurações..."

            Case 40 To 59

                _lblCarregando.Text =
                    "Preparando ferramentas táticas..."

            Case 60 To 79

                _lblCarregando.Text =
                    "Carregando recursos visuais..."

            Case 80 To 99

                _lblCarregando.Text =
                    "Finalizando..."

            Case Else

                _lblCarregando.Text =
                    "TacticalStudio pronto."

        End Select

    End Sub

    Private Sub FrmSplash_Shown(
        sender As Object,
        e As EventArgs) Handles Me.Shown

        _progressoAtual = 0
        _formPrincipalAberto = False

        If _barraProgresso IsNot Nothing Then

            _barraProgresso.Width =
                0

        End If

        AtualizarMensagemCarregamento()

        TmrAbrir.Start()

    End Sub

    Private Sub TmrAbrir_Tick(
        sender As Object,
        e As EventArgs) Handles TmrAbrir.Tick

        _progressoAtual =
            Math.Min(
                100,
                _progressoAtual + 2)

        AtualizarMensagemCarregamento()

        If _barraFundo IsNot Nothing AndAlso
           _barraProgresso IsNot Nothing Then

            Dim larguraMaxima As Integer =
                _barraFundo.ClientSize.Width

            _barraProgresso.Width =
                CInt(
                    larguraMaxima *
                    (_progressoAtual / 100.0R))

        End If

        If _progressoAtual < 100 Then
            Exit Sub
        End If

        TmrAbrir.Stop()

        AbrirFormularioPrincipal()

    End Sub

    Private Sub AbrirFormularioPrincipal()

        If _formPrincipalAberto Then
            Exit Sub
        End If

        _formPrincipalAberto = True

        Try

            Dim frmPrincipal As New FrmPrincipal()

            AddHandler frmPrincipal.FormClosed,
                Sub(sender, e)

                    If Not Me.IsDisposed Then

                        Me.Close()

                    End If

                End Sub

            Me.Hide()

            frmPrincipal.Show()

        Catch ex As Exception

            _formPrincipalAberto =
                False

            Me.Show()

            MessageBox.Show(
                "Não foi possível iniciar o TacticalStudio." &
                Environment.NewLine &
                Environment.NewLine &
                ex.Message,
                "TacticalStudio",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error)

            Me.Close()

        End Try

    End Sub

    Private Sub FrmSplash_FormClosed(
        sender As Object,
        e As FormClosedEventArgs) Handles Me.FormClosed

        TmrAbrir.Stop()

        If _picLogo IsNot Nothing AndAlso
           _picLogo.Image IsNot Nothing Then

            _picLogo.Image.Dispose()

            _picLogo.Image =
                Nothing

        End If

        TmrAbrir.Dispose()

    End Sub

End Class