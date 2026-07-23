Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Reflection
Imports System.Windows.Forms

Public Class FrmSplash
    Inherits Form

    Private WithEvents TmrAbrir As New Timer()

    Private _barraFundo As Panel
    Private _barraProgresso As Panel
    Private _lblCarregando As Label
    Private _picSplash As PictureBox
    Private _picLogoMini As PictureBox
    Private _lblTitulo As Label
    Private _lblSubtitulo As Label
    Private _lblAutor As Label
    Private fluxoIniciado As Boolean = False
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
            1000,
            625)

        Me.BackColor =
        Color.FromArgb(
            12,
            12,
            12)

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

            Dim painelFundo As New Panel With {
            .Dock = DockStyle.Fill,
            .BackColor = Color.FromArgb(
                12,
                12,
                12)
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
            .Height = 100,
            .BackColor = Color.FromArgb(
                18,
                18,
                18)
        }

            painelFundo.Controls.Add(
            faixaInferior)

            _barraFundo =
            New Panel With {
                .Dock = DockStyle.Bottom,
                .Height = 8,
                .BackColor = Color.FromArgb(
                    55,
                    55,
                    55)
            }

            faixaInferior.Controls.Add(
            _barraFundo)

            _barraProgresso =
            New Panel With {
                .Dock = DockStyle.Left,
                .Width = 0,
                .BackColor = Color.FromArgb(
                    178,
                    35,
                    46)
            }

            _barraFundo.Controls.Add(
            _barraProgresso)

            _lblCarregando =
            New Label With {
                .Text =
                    "Inicializando Player Promocional...",
                .ForeColor =
                    Color.White,
                .BackColor =
                    Color.Transparent,
                .Font =
                    New Font(
                        "Segoe UI",
                        10.0F,
                        FontStyle.Regular),
                .Location =
                    New Point(
                        20,
                        62),
                .Size =
                    New Size(
                        420,
                        24),
                .TextAlign =
                    ContentAlignment.MiddleLeft
            }

            faixaInferior.Controls.Add(
            _lblCarregando)

            Dim lblVersao As New Label With {
            .Text =
                "Versão " & ObterVersaoLimpa(),
            .ForeColor =
                Color.Gainsboro,
            .BackColor =
                Color.Transparent,
            .Font =
                New Font(
                    "Segoe UI",
                    9.5F,
                    FontStyle.Regular),
            .Location =
                New Point(
                    780,
                    62),
            .Size =
                New Size(
                    200,
                    24),
            .TextAlign =
                ContentAlignment.MiddleRight
        }

            faixaInferior.Controls.Add(
            lblVersao)

            _lblTitulo =
            New Label With {
                .Text =
                    "PLAYER PROMOCIONAL",
                .ForeColor =
                    Color.White,
                .BackColor =
                    Color.Transparent,
                .Font =
                    New Font(
                        "Segoe UI",
                        20.0F,
                        FontStyle.Bold),
                .Location =
                    New Point(
                        20,
                        8),
                .Size =
                    New Size(
                        520,
                        34),
                .TextAlign =
                    ContentAlignment.MiddleLeft
            }

            faixaInferior.Controls.Add(
            _lblTitulo)

            _lblSubtitulo =
            New Label With {
                .Text =
                    "Áudio inteligente para promoções",
                .ForeColor =
                    Color.FromArgb(
                        220,
                        220,
                        220),
                .BackColor =
                    Color.Transparent,
                .Font =
                    New Font(
                        "Segoe UI",
                        10.5F,
                        FontStyle.Regular),
                .Location =
                    New Point(
                        22,
                        36),
                .Size =
                    New Size(
                        360,
                        22),
                .TextAlign =
                    ContentAlignment.MiddleLeft
            }

            faixaInferior.Controls.Add(
            _lblSubtitulo)

            _lblAutor =
            New Label With {
                .Text =
                    "Desenvolvido por OrbitVerso",
                .ForeColor =
                    Color.FromArgb(
                        190,
                        190,
                        190),
                .BackColor =
                    Color.Transparent,
                .Font =
                    New Font(
                        "Segoe UI",
                        9.5F,
                        FontStyle.Regular),
                .Location =
                    New Point(
                        600,
                        18),
                .Size =
                    New Size(
                        380,
                        22),
                .TextAlign =
                    ContentAlignment.MiddleRight
            }

            faixaInferior.Controls.Add(
            _lblAutor)

            Dim lblEmpresa As New Label With {
            .Text =
                "Orbit / Player Promocional",
            .ForeColor =
                Color.FromArgb(
                    150,
                    150,
                    150),
            .BackColor =
                Color.Transparent,
            .Font =
                New Font(
                    "Segoe UI",
                    9.0F,
                    FontStyle.Regular),
            .Location =
                New Point(
                    600,
                    40),
            .Size =
                New Size(
                    380,
                    18),
            .TextAlign =
                ContentAlignment.MiddleRight
        }

            faixaInferior.Controls.Add(
            lblEmpresa)

            '==================================================
            ' IMAGEM DO SPLASH
            '==================================================

            Dim painelImagem As New Panel With {
                .Dock = DockStyle.Fill,
                .Padding = New Padding(
                    20,
                    20,
                    20,
                    10),
                .BackColor = Color.FromArgb(
                    12,
                    12,
                    12)
            }

            painelFundo.Controls.Add(
                painelImagem)

            '==================================================
            ' IMAGEM PRINCIPAL
            '==================================================

            _picSplash =
                New PictureBox With {
                    .Dock = DockStyle.Fill,
                    .Image = CarregarImagemSplash(),
                    .SizeMode = PictureBoxSizeMode.Zoom,
                    .BackColor = Color.FromArgb(
                        12,
                        12,
                        12)
                }

            painelImagem.Controls.Add(
                _picSplash)

            '==================================================
            ' LOGO ORBIT PEQUENA
            '==================================================

            Dim painelLogoMarca As New Panel With {
                .Size = New Size(
                    180,
                    68),
                .Location = New Point(
                    790,
                    18),
                .BackColor = Color.White,
                .Anchor =
                    AnchorStyles.Top Or
                    AnchorStyles.Right
            }

            painelImagem.Controls.Add(
                painelLogoMarca)

            _picLogoMini =
                New PictureBox With {
                    .Image = CarregarLogoOrbitSplash(),
                    .SizeMode = PictureBoxSizeMode.Zoom,
                    .BackColor = Color.White,
                    .Dock = DockStyle.Fill,
                    .Padding = New Padding(
                        8,
                        6,
                        8,
                        6)
                }

            painelLogoMarca.Controls.Add(
                _picLogoMini)

            painelLogoMarca.BringToFront()

        Finally

            Me.ResumeLayout(
            True)

        End Try

    End Sub

    Private Function CarregarImagemSplash() As Image

        Dim caminhoSplash As String =
        Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Assets",
            "Splash",
            "SplashPlayerPromocional.png")

        If Not File.Exists(caminhoSplash) Then
            Return CriarImagemSplashAusente()
        End If

        Try
            Using imagemOriginal As Image =
            Image.FromFile(caminhoSplash)

                Return New Bitmap(imagemOriginal)
            End Using
        Catch
            Return CriarImagemSplashAusente()
        End Try

    End Function

    Private Function CarregarLogoOrbitSplash() As Image

        Dim caminhoLogo As String =
        Path.Combine(
            AppContext.BaseDirectory,
            "Assets",
            "Icones",
            "LogoOrbitSplash.png")

        If Not File.Exists(caminhoLogo) Then
            Return Nothing
        End If

        Try

            Using imagemOriginal As Image =
            Image.FromFile(caminhoLogo)

                Return New Bitmap(
                imagemOriginal)

            End Using

        Catch

            Return Nothing

        End Try

    End Function

    Private Function CriarImagemSplashAusente() As Image

        Dim imagem As New Bitmap(1000, 525)

        Using g As Graphics =
        Graphics.FromImage(imagem)

            g.Clear(
            Color.FromArgb(
                15,
                15,
                15))

            Using fonteTitulo As New Font(
            "Segoe UI",
            28.0F,
            FontStyle.Bold)

                Using fonteSub As New Font(
                "Segoe UI",
                14.0F,
                FontStyle.Regular)

                    Using pincelVermelho As New SolidBrush(
                    Color.FromArgb(
                        178,
                        35,
                        46))

                        Using pincelBranco As New SolidBrush(
                        Color.White)

                            g.DrawString(
                            "PLAYER PROMOCIONAL",
                            fonteTitulo,
                            pincelBranco,
                            50,
                            180)

                            g.DrawString(
                            "Áudio inteligente para promoções",
                            fonteSub,
                            pincelVermelho,
                            54,
                            235)

                        End Using

                    End Using

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
                Return "1.5.0"
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

                Return "1.5.0"

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
                    "Inicializando Player Promocional..."

            Case 20 To 39

                _lblCarregando.Text =
                    "Carregando configurações..."

            Case 40 To 59

                _lblCarregando.Text =
                    "Preparando playlists e promoções..."

            Case 60 To 79

                _lblCarregando.Text =
                    "Carregando recursos de áudio..."

            Case 80 To 99

                _lblCarregando.Text =
                    "Finalizando..."

            Case Else

                _lblCarregando.Text =
                    "Player Promocional pronto."

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

            AbrirLoginEPrincipal()

        Catch ex As Exception

            _formPrincipalAberto =
                False

            Me.Show()

            MessageBox.Show(
                "Não foi possível iniciar o Player Promocional." &
                Environment.NewLine &
                Environment.NewLine &
                ex.Message,
                "Player Promocional",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error)

            Me.Close()

        End Try

    End Sub

    Private Sub AbrirLoginEPrincipal()

        If fluxoIniciado Then
            Exit Sub
        End If

        fluxoIniciado = True

        Me.Hide()

        Using frmLogin As New FrmLogin()

            If frmLogin.ShowDialog() <>
               DialogResult.OK Then

                Me.Close()
                Exit Sub

            End If

        End Using

        Using frmPrincipal As New FrmPrincipal()

            frmPrincipal.ShowDialog()

        End Using

        Me.Close()

    End Sub

    Private Sub FrmSplash_FormClosed(
        sender As Object,
        e As FormClosedEventArgs) Handles Me.FormClosed

        TmrAbrir.Stop()

        If _picSplash IsNot Nothing AndAlso
   _picSplash.Image IsNot Nothing Then

            _picSplash.Image.Dispose()

            _picSplash.Image =
        Nothing

        End If

        If _picLogoMini IsNot Nothing AndAlso
   _picLogoMini.Image IsNot Nothing Then

            _picLogoMini.Image.Dispose()

            _picLogoMini.Image =
        Nothing

        End If

        TmrAbrir.Dispose()

    End Sub

End Class
