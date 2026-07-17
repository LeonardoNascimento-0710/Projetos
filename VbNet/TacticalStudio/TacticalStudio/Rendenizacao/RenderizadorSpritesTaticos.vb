Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports TacticalStudio.Core
Imports TacticalStudio.Core.Classes
Imports TacticalStudio.Core.Enums

Public NotInheritable Class RenderizadorSpritesTaticos

    Private Sub New()
    End Sub

#Region "Escala e tamanho"

    Public Shared Function NormalizarEscala(
        escala As Single) As Single

        If Single.IsNaN(escala) OrElse
           Single.IsInfinity(escala) Then

            Return 1.0F

        End If

        Return Math.Max(
            0.5F,
            Math.Min(
                2.5F,
                escala))

    End Function

    Public Shared Function ObterMetadeTamanhoJogador(
        jogador As Jogador) As SizeF

        If jogador Is Nothing Then

            Return New SizeF(
                21.0F,
                29.0F)

        End If

        Dim escala As Single =
            NormalizarEscala(
                jogador.EscalaVisual)

        Return New SizeF(
            21.0F * escala,
            29.0F * escala)

    End Function

    Public Shared Function ObterRaioSelecaoJogador(
        jogador As Jogador) As Single

        If jogador Is Nothing Then
            Return 30.0F
        End If

        Return 30.0F *
            NormalizarEscala(
                jogador.EscalaVisual)

    End Function

#End Region

#Region "Jogador"

    Public Shared Sub DesenharJogador(
        g As Graphics,
        jogador As Jogador,
        centro As PointF)

        If g Is Nothing OrElse
           jogador Is Nothing Then

            Exit Sub

        End If

        Dim escala As Single =
            NormalizarEscala(
                jogador.EscalaVisual)

        DesenharSombraJogador(
            g,
            centro,
            escala)

        Dim estadoGrafico As GraphicsState =
            g.Save()

        Try

            g.TranslateTransform(
                centro.X,
                centro.Y)

            g.RotateTransform(
                ObterAnguloDirecao(
                    jogador.Direcao))

            g.ScaleTransform(
                escala,
                escala)

            DesenharCorpoJogador(
    g,
    jogador)

        Finally

            g.Restore(
                estadoGrafico)

        End Try

        DesenharNumeroJogador(
            g,
            jogador,
            centro,
            escala)

    End Sub

    Private Shared Sub DesenharSombraJogador(
        g As Graphics,
        centro As PointF,
        escala As Single)

        Using sombra As New SolidBrush(
            Color.FromArgb(
                70,
                0,
                0,
                0))

            g.FillEllipse(
                sombra,
                centro.X -
                14.0F * escala,
                centro.Y +
                15.0F * escala,
                28.0F * escala,
                10.0F * escala)

        End Using

    End Sub

    Private Shared Sub DesenharCorpoJogador(
        g As Graphics,
        jogador As Jogador)

        Dim pose As PoseJogador =
            jogador.Pose

        Dim maoEsquerda As New PointF(
            -13.0F,
            4.0F)

        Dim maoDireita As New PointF(
            13.0F,
            4.0F)

        Dim peEsquerdo As New PointF(
            -7.0F,
            23.0F)

        Dim peDireito As New PointF(
            7.0F,
            23.0F)

        Dim cabecaY As Single =
            -17.0F

        ConfigurarPose(
            pose,
            maoEsquerda,
            maoDireita,
            peEsquerdo,
            peDireito,
            cabecaY)

        Dim corCamisa As Color =
    ObterCorCamisaJogador(
        jogador)

        Dim corUniformeClara As Color =
    AjustarLuminosidadeCor(
        corCamisa,
        1.24F)

        Dim corUniformeEscura As Color =
    AjustarLuminosidadeCor(
        corCamisa,
        0.55F)

        Dim corShort As Color =
            Color.FromArgb(
                38,
                40,
                47)

        Dim corPele As Color =
            Color.FromArgb(
                225,
                171,
                123)

        Dim corCalcado As Color =
            Color.FromArgb(
                20,
                20,
                24)

        'Pernas ficam atrás do corpo.
        DesenharPerna(
            g,
            New PointF(
                -4.0F,
                8.0F),
            peEsquerdo,
            corShort,
            corCalcado)

        DesenharPerna(
            g,
            New PointF(
                4.0F,
                8.0F),
            peDireito,
            corShort,
            corCalcado)

        'Braços.
        DesenharBraco(
            g,
            New PointF(
                -7.0F,
                -5.0F),
            maoEsquerda,
            corUniformeEscura,
            corPele)

        DesenharBraco(
            g,
            New PointF(
                7.0F,
                -5.0F),
            maoDireita,
            corUniformeEscura,
            corPele)

        'Short.
        Dim pontosShort() As PointF = {
            New PointF(-7.0F, 6.0F),
            New PointF(7.0F, 6.0F),
            New PointF(5.5F, 13.0F),
            New PointF(0.0F, 10.0F),
            New PointF(-5.5F, 13.0F)
        }

        Using pincelShort As New LinearGradientBrush(
            New RectangleF(
                -7.0F,
                6.0F,
                14.0F,
                8.0F),
            Color.FromArgb(
                78,
                80,
                88),
            corShort,
            90.0F)

            g.FillPolygon(
                pincelShort,
                pontosShort)

        End Using

        'Torso.
        Dim pontosTorso() As PointF = {
            New PointF(-8.5F, -7.0F),
            New PointF(8.5F, -7.0F),
            New PointF(7.0F, 6.0F),
            New PointF(4.5F, 10.0F),
            New PointF(-4.5F, 10.0F),
            New PointF(-7.0F, 6.0F)
        }

        Using caminhoTorso As New GraphicsPath()

            caminhoTorso.AddPolygon(
                pontosTorso)

            Using uniforme As New LinearGradientBrush(
                New RectangleF(
                    -9.0F,
                    -8.0F,
                    18.0F,
                    19.0F),
                corUniformeClara,
                corUniformeEscura,
                90.0F)

                g.FillPath(
                    uniforme,
                    caminhoTorso)

            End Using

            Using contorno As New Pen(
                Color.FromArgb(
                    230,
                    255,
                    255,
                    255),
                1.3F)

                contorno.LineJoin =
                    LineJoin.Round

                g.DrawPath(
                    contorno,
                    caminhoTorso)

            End Using

        End Using

        'Brilho do uniforme.
        Using brilho As New SolidBrush(
            Color.FromArgb(
                50,
                255,
                255,
                255))

            g.FillEllipse(
                brilho,
                -5.5F,
                -5.0F,
                7.5F,
                5.0F)

        End Using

        'Cabeça posicionada na frente do jogador.
        Dim retanguloCabeca As New RectangleF(
            -6.0F,
            cabecaY - 6.0F,
            12.0F,
            12.0F)

        Using pele As New LinearGradientBrush(
            retanguloCabeca,
            Color.FromArgb(
                247,
                201,
                154),
            Color.FromArgb(
                151,
                93,
                60),
            90.0F)

            g.FillEllipse(
                pele,
                retanguloCabeca)

        End Using

        Using contornoCabeca As New Pen(
            Color.FromArgb(
                110,
                65,
                38),
            1.1F)

            g.DrawEllipse(
                contornoCabeca,
                retanguloCabeca)

        End Using

        Using cabelo As New SolidBrush(
            Color.FromArgb(
                43,
                29,
                23))

            g.FillPie(
                cabelo,
                retanguloCabeca,
                180.0F,
                180.0F)

        End Using

        If pose = PoseJogador.ComBola Then

            DesenharBolaPequena(
                g,
                New PointF(
                    11.0F,
                    21.0F))

        End If

    End Sub

    Private Shared Function ObterCorCamisaJogador(
    jogador As Jogador
) As Color

        If jogador Is Nothing Then

            Return Color.FromArgb(
            185,
            35,
            35)

        End If

        Dim cor As Color =
        Color.FromArgb(
            jogador.CorCamisaArgb)

        If cor.A = 0 Then

            Return Color.FromArgb(
            185,
            35,
            35)

        End If

        Return Color.FromArgb(
        255,
        cor.R,
        cor.G,
        cor.B)

    End Function

    Private Shared Function AjustarLuminosidadeCor(
    cor As Color,
    fator As Single
) As Color

        Dim vermelho As Integer =
        Math.Max(
            0,
            Math.Min(
                255,
                CInt(
                    cor.R *
                    fator)))

        Dim verde As Integer =
        Math.Max(
            0,
            Math.Min(
                255,
                CInt(
                    cor.G *
                    fator)))

        Dim azul As Integer =
        Math.Max(
            0,
            Math.Min(
                255,
                CInt(
                    cor.B *
                    fator)))

        Return Color.FromArgb(
        255,
        vermelho,
        verde,
        azul)

    End Function
    Private Shared Sub ConfigurarPose(
        pose As PoseJogador,
        ByRef maoEsquerda As PointF,
        ByRef maoDireita As PointF,
        ByRef peEsquerdo As PointF,
        ByRef peDireito As PointF,
        ByRef cabecaY As Single)

        Select Case pose

            Case PoseJogador.Correndo

                maoEsquerda =
                    New PointF(
                        -11.0F,
                        -10.0F)

                maoDireita =
                    New PointF(
                        13.0F,
                        11.0F)

                peEsquerdo =
                    New PointF(
                        -10.0F,
                        24.0F)

                peDireito =
                    New PointF(
                        9.0F,
                        17.0F)

            Case PoseJogador.ComBola

                maoEsquerda =
                    New PointF(
                        -13.0F,
                        2.0F)

                maoDireita =
                    New PointF(
                        11.0F,
                        -3.0F)

                peDireito =
                    New PointF(
                        10.0F,
                        18.0F)

            Case PoseJogador.Passe

                maoEsquerda =
                    New PointF(
                        -16.0F,
                        1.0F)

                maoDireita =
                    New PointF(
                        16.0F,
                        1.0F)

                peDireito =
                    New PointF(
                        14.0F,
                        16.0F)

            Case PoseJogador.Chute

                maoEsquerda =
                    New PointF(
                        -16.0F,
                        0.0F)

                maoDireita =
                    New PointF(
                        15.0F,
                        3.0F)

                peDireito =
                    New PointF(
                        17.0F,
                        9.0F)

            Case PoseJogador.Cabeceio

                maoEsquerda =
                    New PointF(
                        -18.0F,
                        -5.0F)

                maoDireita =
                    New PointF(
                        18.0F,
                        -5.0F)

                cabecaY =
                    -20.0F

            Case PoseJogador.Marcacao

                maoEsquerda =
                    New PointF(
                        -18.0F,
                        2.0F)

                maoDireita =
                    New PointF(
                        18.0F,
                        2.0F)

                peEsquerdo =
                    New PointF(
                        -12.0F,
                        21.0F)

                peDireito =
                    New PointF(
                        12.0F,
                        21.0F)

            Case PoseJogador.Goleiro

                maoEsquerda =
                    New PointF(
                        -23.0F,
                        -6.0F)

                maoDireita =
                    New PointF(
                        23.0F,
                        -6.0F)

                peEsquerdo =
                    New PointF(
                        -13.0F,
                        22.0F)

                peDireito =
                    New PointF(
                        13.0F,
                        22.0F)

        End Select

    End Sub

    Private Shared Sub DesenharBraco(
        g As Graphics,
        ombro As PointF,
        mao As PointF,
        corManga As Color,
        corPele As Color)

        Dim cotovelo As New PointF(
            ombro.X +
            (mao.X - ombro.X) *
            0.52F,
            ombro.Y +
            (mao.Y - ombro.Y) *
            0.52F)

        Using manga As New Pen(
            corManga,
            7.0F)

            manga.StartCap =
                LineCap.Round

            manga.EndCap =
                LineCap.Round

            g.DrawLine(
                manga,
                ombro,
                cotovelo)

        End Using

        Using braco As New Pen(
            corPele,
            4.0F)

            braco.StartCap =
                LineCap.Round

            braco.EndCap =
                LineCap.Round

            g.DrawLine(
                braco,
                cotovelo,
                mao)

        End Using

        Using pincelMao As New SolidBrush(
            corPele)

            g.FillEllipse(
                pincelMao,
                mao.X - 2.2F,
                mao.Y - 2.2F,
                4.4F,
                4.4F)

        End Using

    End Sub

    Private Shared Sub DesenharPerna(
        g As Graphics,
        quadril As PointF,
        pe As PointF,
        corPerna As Color,
        corCalcado As Color)

        Using perna As New Pen(
            corPerna,
            5.5F)

            perna.StartCap =
                LineCap.Round

            perna.EndCap =
                LineCap.Round

            g.DrawLine(
                perna,
                quadril,
                pe)

        End Using

        Using calcado As New SolidBrush(
            corCalcado)

            g.FillEllipse(
                calcado,
                pe.X - 3.0F,
                pe.Y - 2.2F,
                6.0F,
                4.4F)

        End Using

    End Sub

    Private Shared Sub DesenharBolaPequena(
        g As Graphics,
        centro As PointF)

        Using pincel As New SolidBrush(
            Color.WhiteSmoke)

            g.FillEllipse(
                pincel,
                centro.X - 3.8F,
                centro.Y - 3.8F,
                7.6F,
                7.6F)

        End Using

        Using borda As New Pen(
            Color.FromArgb(
                35,
                35,
                38),
            1.0F)

            g.DrawEllipse(
                borda,
                centro.X - 3.8F,
                centro.Y - 3.8F,
                7.6F,
                7.6F)

        End Using

    End Sub

    Private Shared Sub DesenharNumeroJogador(g As Graphics, jogador As Jogador, centro As PointF, escala As Single)

        If g Is Nothing OrElse
       jogador Is Nothing Then

            Exit Sub

        End If

        Dim textoNumero As String =
        jogador.Numero.ToString()

        Dim quantidadeDigitos As Integer =
        textoNumero.Length

        Dim altura As Single =
        16.0F *
        escala

        Dim largura As Single

        Select Case quantidadeDigitos

            Case 1

                largura =
                16.0F *
                escala

            Case 2

                largura =
                22.0F *
                escala

            Case Else

                largura =
                28.0F *
                escala

        End Select

        Dim retanguloNumero As New RectangleF(
        centro.X -
        largura / 2.0F,
        centro.Y -
        altura / 2.0F,
        largura,
        altura)

        '==================================================
        ' SOMBRA
        '==================================================

        Using sombra As New SolidBrush(
        Color.FromArgb(
            75,
            0,
            0,
            0))

            g.FillEllipse(
            sombra,
            retanguloNumero.X +
            1.5F *
            escala,
            retanguloNumero.Y +
            2.0F *
            escala,
            retanguloNumero.Width,
            retanguloNumero.Height)

        End Using

        '==================================================
        ' FUNDO DO NÚMERO
        '==================================================

        Using fundo As New LinearGradientBrush(
        retanguloNumero,
        Color.FromArgb(
            225,
            170,
            30,
            35),
        Color.FromArgb(
            225,
            92,
            8,
            14),
        90.0F)

            g.FillEllipse(
            fundo,
            retanguloNumero)

        End Using

        '==================================================
        ' BORDA
        '==================================================

        Using borda As New Pen(
        Color.White,
        Math.Max(
            1.0F,
            1.2F *
            escala))

            g.DrawEllipse(
            borda,
            retanguloNumero)

        End Using

        '==================================================
        ' TAMANHO DA FONTE
        '==================================================

        Dim tamanhoFonte As Single

        Select Case quantidadeDigitos

            Case 1

                tamanhoFonte =
                7.3F *
                escala

            Case 2

                tamanhoFonte =
                6.8F *
                escala

            Case Else

                tamanhoFonte =
                5.9F *
                escala

        End Select

        tamanhoFonte =
        Math.Max(
            5.5F,
            tamanhoFonte)

        Using fonte As New Font(
        "Segoe UI",
        tamanhoFonte,
        FontStyle.Bold,
        GraphicsUnit.Point)

            Using pincelTexto As New SolidBrush(
            Color.White)

                Using formato As New StringFormat()

                    formato.Alignment =
                    StringAlignment.Center

                    formato.LineAlignment =
                    StringAlignment.Center

                    formato.Trimming =
                    StringTrimming.None

                    formato.FormatFlags =
                    StringFormatFlags.NoWrap Or
                    StringFormatFlags.NoClip

                    g.DrawString(
                    textoNumero,
                    fonte,
                    pincelTexto,
                    retanguloNumero,
                    formato)

                End Using

            End Using

        End Using

    End Sub

    Private Shared Function ObterAnguloDirecao(
        direcao As DirecaoJogador) As Single

        Select Case direcao

            Case DirecaoJogador.Cima

                Return 0.0F

            Case DirecaoJogador.CimaDireita

                Return 45.0F

            Case DirecaoJogador.Direita

                Return 90.0F

            Case DirecaoJogador.BaixoDireita

                Return 135.0F

            Case DirecaoJogador.Baixo

                Return 180.0F

            Case DirecaoJogador.BaixoEsquerda

                Return 225.0F

            Case DirecaoJogador.Esquerda

                Return 270.0F

            Case DirecaoJogador.CimaEsquerda

                Return 315.0F

            Case Else

                Return 0.0F

        End Select

    End Function

#End Region

End Class