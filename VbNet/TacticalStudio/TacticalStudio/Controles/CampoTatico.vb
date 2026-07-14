Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports TacticalStudio.Core.Classes
Imports TacticalStudio.Core.Enums

Public Class CampoTatico
    Inherits UserControl

    Private ReadOnly _objetos As New List(Of ObjetoCampo)

    Private _objetoSelecionado As ObjetoCampo
    Private _arrastando As Boolean
    Private _offsetMouse As PointF

    Private Const MargemCampo As Single = 35.0F
    Private Const RaioJogador As Single = 18.0F
    Private Const RaioBola As Single = 10.0F
    Private Const RaioCone As Single = 14.0F
    Private Const LarguraBocaGol As Single = 44.0F
    Private Const ProfundidadeGol As Single = 22.0F
    Private Const LarguraManequim As Single = 26.0F
    Private Const AlturaManequim As Single = 50.0F

    Public Sub New()

        DoubleBuffered = True
        ResizeRedraw = True

        Dock = DockStyle.Fill
        BackColor = Tema.Fundo

        SetStyle(
            ControlStyles.AllPaintingInWmPaint Or
            ControlStyles.UserPaint Or
            ControlStyles.OptimizedDoubleBuffer Or
            ControlStyles.ResizeRedraw,
            True)

        UpdateStyles()

    End Sub

#Region "Adicionar objetos"

    Public Function AdicionarJogador(
        numero As Integer,
        nome As String,
        xPercentual As Double,
        yPercentual As Double) As Jogador

        Dim jogador As New Jogador With {
            .Numero = numero,
            .Nome = nome
        }

        jogador.Posicao.X = LimitarPercentual(xPercentual)
        jogador.Posicao.Y = LimitarPercentual(yPercentual)

        _objetos.Add(jogador)

        Invalidate()

        Return jogador

    End Function

    Public Function AdicionarBola(
        xPercentual As Double,
        yPercentual As Double) As Bola

        Dim bola As New Bola()

        bola.Posicao.X = LimitarPercentual(xPercentual)
        bola.Posicao.Y = LimitarPercentual(yPercentual)

        _objetos.Add(bola)

        Invalidate()

        Return bola

    End Function

    Public Function AdicionarCone(
    cor As CorCone,
    xPercentual As Double,
    yPercentual As Double) As Cone

        Dim cone As New Cone With {
        .Cor = cor
    }

        cone.Posicao.X =
        LimitarPercentual(xPercentual)

        cone.Posicao.Y =
        LimitarPercentual(yPercentual)

        _objetos.Add(cone)

        Invalidate()

        Return cone

    End Function

    Public Function AdicionarGol(
    orientacao As OrientacaoGol,
    xPercentual As Double,
    yPercentual As Double) As Gol

        Dim gol As New Gol With {
        .Orientacao = orientacao
    }

        gol.Posicao.X =
        LimitarPercentual(xPercentual)

        gol.Posicao.Y =
        LimitarPercentual(yPercentual)

        _objetos.Add(gol)

        Invalidate()

        Return gol

    End Function

    Public Function AdicionarManequim(
    cor As CorManequim,
    xPercentual As Double,
    yPercentual As Double) As Manequim

        Dim manequim As New Manequim With {
        .Cor = cor
    }

        manequim.Posicao.X =
        LimitarPercentual(xPercentual)

        manequim.Posicao.Y =
        LimitarPercentual(yPercentual)

        _objetos.Add(manequim)

        Invalidate()

        Return manequim

    End Function

    Public Sub LimparObjetos()

        _objetos.Clear()

        _objetoSelecionado = Nothing
        _arrastando = False

        Capture = False

        Invalidate()

    End Sub

#End Region

#Region "Pintura"

    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        MyBase.OnPaint(e)

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality

        DesenharCampo(e.Graphics)

    End Sub

    Private Sub DesenharCampo(g As Graphics)

        g.Clear(BackColor)

        Dim campo As RectangleF = ObterRetanguloCampo()

        If campo.Width <= 0 OrElse campo.Height <= 0 Then
            Exit Sub
        End If

        DesenharGramado(g, campo)
        DesenharMarcacoes(g, campo)
        DesenharObjetos(g, campo)

    End Sub

    Private Sub DesenharGramado(
        g As Graphics,
        campo As RectangleF)

        Using fundoCampo As New SolidBrush(
            Color.FromArgb(63, 130, 58))

            g.FillRectangle(fundoCampo, campo)

        End Using

        Dim quantidadeFaixas As Integer = 10

        Dim larguraFaixa As Single =
            campo.Width / quantidadeFaixas

        For indice As Integer = 0 To quantidadeFaixas - 1

            If indice Mod 2 = 0 Then

                Dim faixa As New RectangleF(
                    campo.Left + indice * larguraFaixa,
                    campo.Top,
                    larguraFaixa,
                    campo.Height)

                Using pincelFaixa As New SolidBrush(
                    Color.FromArgb(20, 255, 255, 255))

                    g.FillRectangle(pincelFaixa, faixa)

                End Using

            End If

        Next

    End Sub

    Private Sub DesenharMarcacoes(
        g As Graphics,
        campo As RectangleF)

        Using caneta As New Pen(Color.White, 2.5F)

            caneta.LineJoin = LineJoin.Round

            g.DrawRectangle(
                caneta,
                campo.Left,
                campo.Top,
                campo.Width,
                campo.Height)

            Dim centroX As Single =
                campo.Left + campo.Width / 2.0F

            Dim centroY As Single =
                campo.Top + campo.Height / 2.0F

            g.DrawLine(
                caneta,
                centroX,
                campo.Top,
                centroX,
                campo.Bottom)

            Dim raioCirculo As Single =
                Math.Min(campo.Width, campo.Height) * 0.12F

            g.DrawEllipse(
                caneta,
                centroX - raioCirculo,
                centroY - raioCirculo,
                raioCirculo * 2.0F,
                raioCirculo * 2.0F)

            Using pincelCentro As New SolidBrush(Color.White)

                g.FillEllipse(
                    pincelCentro,
                    centroX - 3.0F,
                    centroY - 3.0F,
                    6.0F,
                    6.0F)

            End Using

            DesenharGrandeArea(
                g,
                caneta,
                campo,
                True)

            DesenharGrandeArea(
                g,
                caneta,
                campo,
                False)

        End Using

    End Sub

    Private Sub DesenharGrandeArea(
        g As Graphics,
        caneta As Pen,
        campo As RectangleF,
        ladoEsquerdo As Boolean)

        Dim larguraArea As Single =
            campo.Width * 0.16F

        Dim alturaArea As Single =
            campo.Height * 0.52F

        Dim y As Single =
            campo.Top +
            (campo.Height - alturaArea) / 2.0F

        Dim x As Single

        If ladoEsquerdo Then
            x = campo.Left
        Else
            x = campo.Right - larguraArea
        End If

        g.DrawRectangle(
            caneta,
            x,
            y,
            larguraArea,
            alturaArea)

        Dim larguraPequenaArea As Single =
            campo.Width * 0.06F

        Dim alturaPequenaArea As Single =
            campo.Height * 0.25F

        Dim yPequena As Single =
            campo.Top +
            (campo.Height - alturaPequenaArea) / 2.0F

        If ladoEsquerdo Then
            x = campo.Left
        Else
            x = campo.Right - larguraPequenaArea
        End If

        g.DrawRectangle(
            caneta,
            x,
            yPequena,
            larguraPequenaArea,
            alturaPequenaArea)

    End Sub

#End Region

#Region "Desenho dos objetos"

    Private Sub DesenharObjetos(
        g As Graphics,
        campo As RectangleF)

        For Each objeto As ObjetoCampo In _objetos

            If Not objeto.Visivel Then
                Continue For
            End If

            If TypeOf objeto Is Jogador Then

                DesenharJogador(
                    g,
                    DirectCast(objeto, Jogador),
                    campo)

            ElseIf TypeOf objeto Is Bola Then

                DesenharBola(
        g,
        DirectCast(objeto, Bola),
        campo)

            ElseIf TypeOf objeto Is Cone Then

                DesenharCone(
        g,
        DirectCast(objeto, Cone),
        campo)

            ElseIf TypeOf objeto Is Gol Then

            ElseIf TypeOf objeto Is Gol Then

                DesenharGol(
        g,
        DirectCast(objeto, Gol),
        campo)

            ElseIf TypeOf objeto Is Manequim Then

                DesenharManequim(
        g,
        DirectCast(objeto, Manequim),
        campo)

            End If


        Next

    End Sub

    Private Sub DesenharJogador(
        g As Graphics,
        jogador As Jogador,
        campo As RectangleF)

        Dim centro As PointF =
            ConverterPercentualParaTela(
                jogador.Posicao,
                campo)

        Using sombra As New SolidBrush(
            Color.FromArgb(80, 0, 0, 0))

            g.FillEllipse(
                sombra,
                centro.X - RaioJogador + 3.0F,
                centro.Y - RaioJogador + 5.0F,
                RaioJogador * 2.0F,
                RaioJogador * 2.0F)

        End Using

        Using pincelJogador As New SolidBrush(
            Color.FromArgb(185, 35, 35))

            g.FillEllipse(
                pincelJogador,
                centro.X - RaioJogador,
                centro.Y - RaioJogador,
                RaioJogador * 2.0F,
                RaioJogador * 2.0F)

        End Using

        Using bordaJogador As New Pen(
            Color.White,
            2.0F)

            g.DrawEllipse(
                bordaJogador,
                centro.X - RaioJogador,
                centro.Y - RaioJogador,
                RaioJogador * 2.0F,
                RaioJogador * 2.0F)

        End Using

        If jogador Is _objetoSelecionado Then

            DesenharSelecao(
                g,
                centro,
                RaioJogador)

        End If

        DesenharNumeroJogador(
            g,
            jogador,
            centro)

    End Sub

    Private Sub DesenharNumeroJogador(
        g As Graphics,
        jogador As Jogador,
        centro As PointF)

        Using fonte As New Font(
            "Segoe UI",
            9.0F,
            FontStyle.Bold,
            GraphicsUnit.Point)

            Using pincelTexto As New SolidBrush(
                Color.White)

                Dim texto As String =
                    jogador.Numero.ToString()

                Dim tamanho As SizeF =
                    g.MeasureString(texto, fonte)

                g.DrawString(
                    texto,
                    fonte,
                    pincelTexto,
                    centro.X - tamanho.Width / 2.0F,
                    centro.Y - tamanho.Height / 2.0F)

            End Using

        End Using

    End Sub

    Private Sub DesenharBola(
        g As Graphics,
        bola As Bola,
        campo As RectangleF)

        Dim centro As PointF =
            ConverterPercentualParaTela(
                bola.Posicao,
                campo)

        Using sombra As New SolidBrush(
            Color.FromArgb(90, 0, 0, 0))

            g.FillEllipse(
                sombra,
                centro.X - RaioBola + 2.0F,
                centro.Y - RaioBola + 3.0F,
                RaioBola * 2.0F,
                RaioBola * 2.0F)

        End Using

        Using pincelBola As New SolidBrush(
            Color.WhiteSmoke)

            g.FillEllipse(
                pincelBola,
                centro.X - RaioBola,
                centro.Y - RaioBola,
                RaioBola * 2.0F,
                RaioBola * 2.0F)

        End Using

        Using bordaBola As New Pen(
            Color.FromArgb(35, 35, 35),
            1.5F)

            g.DrawEllipse(
                bordaBola,
                centro.X - RaioBola,
                centro.Y - RaioBola,
                RaioBola * 2.0F,
                RaioBola * 2.0F)

        End Using

        DesenharDetalhesBola(
            g,
            centro)

        If bola Is _objetoSelecionado Then

            DesenharSelecao(
                g,
                centro,
                RaioBola)

        End If

    End Sub

    Private Sub DesenharDetalhesBola(
        g As Graphics,
        centro As PointF)

        Using pincelDetalhe As New SolidBrush(
            Color.FromArgb(40, 40, 40))

            g.FillEllipse(
                pincelDetalhe,
                centro.X - 3.5F,
                centro.Y - 3.5F,
                7.0F,
                7.0F)

            For indice As Integer = 0 To 4

                Dim angulo As Double =
                    indice * (Math.PI * 2.0 / 5.0)

                Dim distancia As Single = 6.3F

                Dim x As Single =
                    centro.X +
                    CSng(Math.Cos(angulo)) * distancia

                Dim y As Single =
                    centro.Y +
                    CSng(Math.Sin(angulo)) * distancia

                g.FillEllipse(
                    pincelDetalhe,
                    x - 1.6F,
                    y - 1.6F,
                    3.2F,
                    3.2F)

            Next

        End Using

    End Sub

    Private Sub DesenharCone(
    g As Graphics,
    cone As Cone,
    campo As RectangleF)

        Dim centro As PointF =
        ConverterPercentualParaTela(
            cone.Posicao,
            campo)

        Dim corCone As Color =
        ObterCorCone(cone.Cor)

        Dim largura As Single = 18.0F
        Dim altura As Single = 24.0F

        Dim topo As New PointF(
        centro.X,
        centro.Y - altura / 2.0F)

        Dim inferiorEsquerdo As New PointF(
        centro.X - largura / 2.0F,
        centro.Y + altura / 2.0F - 4.0F)

        Dim inferiorDireito As New PointF(
        centro.X + largura / 2.0F,
        centro.Y + altura / 2.0F - 4.0F)

        Dim pontosCone() As PointF = {
        topo,
        inferiorDireito,
        inferiorEsquerdo
    }

        Using sombra As New SolidBrush(
        Color.FromArgb(80, 0, 0, 0))

            g.FillEllipse(
            sombra,
            centro.X - 12.0F,
            centro.Y + 7.0F,
            24.0F,
            7.0F)

        End Using

        Using pincelCone As New SolidBrush(corCone)

            g.FillPolygon(
            pincelCone,
            pontosCone)

        End Using

        Using borda As New Pen(
        Color.FromArgb(90, 60, 30),
        1.5F)

            g.DrawPolygon(
            borda,
            pontosCone)

        End Using

        Using pincelFaixa As New SolidBrush(
        Color.FromArgb(220, 255, 255, 255))

            Dim faixa As New RectangleF(
            centro.X - 6.0F,
            centro.Y,
            12.0F,
            4.0F)

            g.FillRectangle(
            pincelFaixa,
            faixa)

        End Using

        Using pincelBase As New SolidBrush(
        Color.FromArgb(
            corCone.R,
            corCone.G,
            corCone.B))

            g.FillRectangle(
            pincelBase,
            centro.X - 12.0F,
            centro.Y + 8.0F,
            24.0F,
            5.0F)

        End Using

        Using bordaBase As New Pen(
        Color.FromArgb(90, 60, 30),
        1.2F)

            g.DrawRectangle(
            bordaBase,
            centro.X - 12.0F,
            centro.Y + 8.0F,
            24.0F,
            5.0F)

        End Using

        If cone Is _objetoSelecionado Then

            DesenharSelecao(
            g,
            centro,
            RaioCone)

        End If

    End Sub

    Private Function ObterCorCone(
    cor As CorCone) As Color

        Select Case cor

            Case CorCone.Amarelo
                Return Color.FromArgb(245, 205, 35)

            Case CorCone.Laranja
                Return Color.FromArgb(242, 120, 25)

            Case CorCone.Vermelho
                Return Color.FromArgb(205, 45, 45)

            Case CorCone.Azul
                Return Color.FromArgb(45, 115, 210)

            Case CorCone.Verde
                Return Color.FromArgb(45, 170, 85)

            Case Else
                Return Color.Orange

        End Select

    End Function

    Private Sub DesenharGol(
    g As Graphics,
    gol As Gol,
    campo As RectangleF)

        Dim centro As PointF =
        ConverterPercentualParaTela(
            gol.Posicao,
            campo)

        Dim pontos() As PointF =
        ObterPontosGol(
            centro,
            gol.Orientacao)

        Dim frente1 As PointF = pontos(0)
        Dim frente2 As PointF = pontos(1)
        Dim fundo1 As PointF = pontos(2)
        Dim fundo2 As PointF = pontos(3)

        Dim poligono() As PointF = {
        frente1,
        frente2,
        fundo2,
        fundo1
    }

        Dim sombra() As PointF = {
        New PointF(frente1.X + 3.0F, frente1.Y + 4.0F),
        New PointF(frente2.X + 3.0F, frente2.Y + 4.0F),
        New PointF(fundo2.X + 3.0F, fundo2.Y + 4.0F),
        New PointF(fundo1.X + 3.0F, fundo1.Y + 4.0F)
    }

        Using pincelSombra As New SolidBrush(
        Color.FromArgb(65, 0, 0, 0))

            g.FillPolygon(
            pincelSombra,
            sombra)

        End Using

        Using pincelRede As New SolidBrush(
        Color.FromArgb(45, 245, 245, 245))

            g.FillPolygon(
            pincelRede,
            poligono)

        End Using

        DesenharRedeGol(
        g,
        frente1,
        frente2,
        fundo1,
        fundo2)

        Using canetaEstrutura As New Pen(
        Color.WhiteSmoke,
        3.0F)

            canetaEstrutura.LineJoin =
            LineJoin.Round

            g.DrawLine(
            canetaEstrutura,
            frente1,
            frente2)

            g.DrawLine(
            canetaEstrutura,
            frente1,
            fundo1)

            g.DrawLine(
            canetaEstrutura,
            frente2,
            fundo2)

            g.DrawLine(
            canetaEstrutura,
            fundo1,
            fundo2)

        End Using

        Using pincelTrave As New SolidBrush(
        Color.White)

            g.FillEllipse(
            pincelTrave,
            frente1.X - 3.0F,
            frente1.Y - 3.0F,
            6.0F,
            6.0F)

            g.FillEllipse(
            pincelTrave,
            frente2.X - 3.0F,
            frente2.Y - 3.0F,
            6.0F,
            6.0F)

        End Using

        If gol Is _objetoSelecionado Then

            Dim areaGol As RectangleF =
            ObterRetanguloGol(
                centro,
                gol.Orientacao)

            DesenharSelecaoRetangular(
            g,
            areaGol)

        End If

    End Sub

    Private Sub DesenharRedeGol(
    g As Graphics,
    frente1 As PointF,
    frente2 As PointF,
    fundo1 As PointF,
    fundo2 As PointF)

        Using canetaRede As New Pen(
        Color.FromArgb(155, 235, 235, 235),
        1.0F)

            For indice As Integer = 1 To 4

                Dim percentual As Single =
                CSng(indice) / 5.0F

                Dim ponto1 As PointF =
                InterpolarPonto(
                    frente1,
                    fundo1,
                    percentual)

                Dim ponto2 As PointF =
                InterpolarPonto(
                    frente2,
                    fundo2,
                    percentual)

                g.DrawLine(
                canetaRede,
                ponto1,
                ponto2)

            Next

            For indice As Integer = 1 To 3

                Dim percentual As Single =
                CSng(indice) / 4.0F

                Dim pontoFrente As PointF =
                InterpolarPonto(
                    frente1,
                    frente2,
                    percentual)

                Dim pontoFundo As PointF =
                InterpolarPonto(
                    fundo1,
                    fundo2,
                    percentual)

                g.DrawLine(
                canetaRede,
                pontoFrente,
                pontoFundo)

            Next

        End Using

    End Sub

    Private Function ObterPontosGol(
    centro As PointF,
    orientacao As OrientacaoGol) As PointF()

        Dim meiaBoca As Single =
        LarguraBocaGol / 2.0F

        Dim meiaProfundidade As Single =
        ProfundidadeGol / 2.0F

        Dim meiaParteFundo As Single =
        LarguraBocaGol * 0.38F

        Select Case orientacao

            Case OrientacaoGol.Direita

                Return {
                New PointF(
                    centro.X + meiaProfundidade,
                    centro.Y - meiaBoca),
                New PointF(
                    centro.X + meiaProfundidade,
                    centro.Y + meiaBoca),
                New PointF(
                    centro.X - meiaProfundidade,
                    centro.Y - meiaParteFundo),
                New PointF(
                    centro.X - meiaProfundidade,
                    centro.Y + meiaParteFundo)
            }

            Case OrientacaoGol.Esquerda

                Return {
                New PointF(
                    centro.X - meiaProfundidade,
                    centro.Y - meiaBoca),
                New PointF(
                    centro.X - meiaProfundidade,
                    centro.Y + meiaBoca),
                New PointF(
                    centro.X + meiaProfundidade,
                    centro.Y - meiaParteFundo),
                New PointF(
                    centro.X + meiaProfundidade,
                    centro.Y + meiaParteFundo)
            }

            Case OrientacaoGol.Cima

                Return {
                New PointF(
                    centro.X - meiaBoca,
                    centro.Y - meiaProfundidade),
                New PointF(
                    centro.X + meiaBoca,
                    centro.Y - meiaProfundidade),
                New PointF(
                    centro.X - meiaParteFundo,
                    centro.Y + meiaProfundidade),
                New PointF(
                    centro.X + meiaParteFundo,
                    centro.Y + meiaProfundidade)
            }

            Case OrientacaoGol.Baixo

                Return {
                New PointF(
                    centro.X - meiaBoca,
                    centro.Y + meiaProfundidade),
                New PointF(
                    centro.X + meiaBoca,
                    centro.Y + meiaProfundidade),
                New PointF(
                    centro.X - meiaParteFundo,
                    centro.Y - meiaProfundidade),
                New PointF(
                    centro.X + meiaParteFundo,
                    centro.Y - meiaProfundidade)
            }

            Case Else

                Return Array.Empty(Of PointF)()

        End Select

    End Function

    Private Function InterpolarPonto(
    pontoInicial As PointF,
    pontoFinal As PointF,
    percentual As Single) As PointF

        Return New PointF(
        pontoInicial.X +
        (pontoFinal.X - pontoInicial.X) * percentual,
        pontoInicial.Y +
        (pontoFinal.Y - pontoInicial.Y) * percentual)

    End Function

    Private Sub DesenharSelecaoRetangular(
    g As Graphics,
    area As RectangleF)

        area.Inflate(
        6.0F,
        6.0F)

        Using canetaSelecao As New Pen(
        Color.Gold,
        2.5F)

            canetaSelecao.DashStyle =
            DashStyle.Dash

            g.DrawRectangle(
            canetaSelecao,
            area.X,
            area.Y,
            area.Width,
            area.Height)

        End Using

    End Sub

    Private Sub DesenharManequim(
    g As Graphics,
    manequim As Manequim,
    campo As RectangleF)

        Dim centro As PointF =
        ConverterPercentualParaTela(
            manequim.Posicao,
            campo)

        Dim corPrincipal As Color =
        ObterCorManequim(manequim.Cor)

        Dim topo As Single =
        centro.Y - AlturaManequim / 2.0F

        Dim baseY As Single =
        centro.Y + AlturaManequim / 2.0F

        Dim centroCabeca As New PointF(
        centro.X,
        topo + 7.0F)

        Dim ombroY As Single =
        topo + 15.0F

        Dim cinturaY As Single =
        centro.Y + 5.0F

        Dim inicioPernasY As Single =
        cinturaY + 2.0F

        Dim finalPernasY As Single =
        baseY - 7.0F

        Using sombra As New SolidBrush(
        Color.FromArgb(70, 0, 0, 0))

            g.FillEllipse(
            sombra,
            centro.X - 16.0F,
            baseY - 3.0F,
            32.0F,
            9.0F)

        End Using

        Using pincelBase As New SolidBrush(
        Color.FromArgb(55, 55, 55))

            g.FillEllipse(
            pincelBase,
            centro.X - 15.0F,
            baseY - 6.0F,
            30.0F,
            9.0F)

        End Using

        Using canetaBase As New Pen(
        Color.FromArgb(25, 25, 25),
        1.5F)

            g.DrawEllipse(
            canetaBase,
            centro.X - 15.0F,
            baseY - 6.0F,
            30.0F,
            9.0F)

        End Using

        Using canetaPernas As New Pen(
        corPrincipal,
        5.0F)

            canetaPernas.StartCap =
            LineCap.Round

            canetaPernas.EndCap =
            LineCap.Round

            g.DrawLine(
            canetaPernas,
            centro.X - 3.0F,
            inicioPernasY,
            centro.X - 7.0F,
            finalPernasY)

            g.DrawLine(
            canetaPernas,
            centro.X + 3.0F,
            inicioPernasY,
            centro.X + 7.0F,
            finalPernasY)

        End Using

        Dim pontosTorso() As PointF = {
        New PointF(
            centro.X - LarguraManequim / 2.0F,
            ombroY),
        New PointF(
            centro.X + LarguraManequim / 2.0F,
            ombroY),
        New PointF(
            centro.X + 7.0F,
            cinturaY),
        New PointF(
            centro.X + 4.0F,
            inicioPernasY),
        New PointF(
            centro.X - 4.0F,
            inicioPernasY),
        New PointF(
            centro.X - 7.0F,
            cinturaY)
    }

        Using pincelTorso As New SolidBrush(
        corPrincipal)

            g.FillPolygon(
            pincelTorso,
            pontosTorso)

        End Using

        Using canetaContorno As New Pen(
        Color.FromArgb(100, 75, 35),
        1.5F)

            canetaContorno.LineJoin =
            LineJoin.Round

            g.DrawPolygon(
            canetaContorno,
            pontosTorso)

        End Using

        Using pincelCabeca As New SolidBrush(
        corPrincipal)

            g.FillEllipse(
            pincelCabeca,
            centroCabeca.X - 6.5F,
            centroCabeca.Y - 6.5F,
            13.0F,
            13.0F)

        End Using

        Using canetaCabeca As New Pen(
        Color.FromArgb(100, 75, 35),
        1.5F)

            g.DrawEllipse(
            canetaCabeca,
            centroCabeca.X - 6.5F,
            centroCabeca.Y - 6.5F,
            13.0F,
            13.0F)

        End Using

        Using canetaDetalhes As New Pen(
        Color.FromArgb(115, 90, 45),
        1.2F)

            g.DrawLine(
            canetaDetalhes,
            centro.X - 8.0F,
            ombroY + 7.0F,
            centro.X + 8.0F,
            ombroY + 7.0F)

            g.DrawLine(
            canetaDetalhes,
            centro.X - 6.0F,
            cinturaY - 4.0F,
            centro.X + 6.0F,
            cinturaY - 4.0F)

        End Using

        If manequim Is _objetoSelecionado Then

            Dim area As RectangleF =
            ObterRetanguloManequim(centro)

            DesenharSelecaoRetangular(
            g,
            area)

        End If

    End Sub

    Private Function ObterCorManequim(
    cor As CorManequim) As Color

        Select Case cor

            Case CorManequim.Amarelo
                Return Color.FromArgb(242, 202, 32)

            Case CorManequim.Laranja
                Return Color.FromArgb(235, 116, 25)

            Case CorManequim.Vermelho
                Return Color.FromArgb(200, 45, 45)

            Case CorManequim.Azul
                Return Color.FromArgb(45, 105, 205)

            Case Else
                Return Color.Gold

        End Select

    End Function

    Private Sub DesenharSelecao(
        g As Graphics,
        centro As PointF,
        raio As Single)

        Using bordaSelecao As New Pen(
            Color.Gold,
            3.0F)

            g.DrawEllipse(
                bordaSelecao,
                centro.X - raio - 4.0F,
                centro.Y - raio - 4.0F,
                raio * 2.0F + 8.0F,
                raio * 2.0F + 8.0F)

        End Using

    End Sub

#End Region

#Region "Mouse"

    Protected Overrides Sub OnMouseDown(
        e As MouseEventArgs)

        MyBase.OnMouseDown(e)

        If e.Button <> MouseButtons.Left Then
            Exit Sub
        End If

        Dim campo As RectangleF =
            ObterRetanguloCampo()

        DeselecionarTodos()

        _objetoSelecionado =
            LocalizarObjetoNaPosicao(
                e.Location,
                campo)

        If _objetoSelecionado IsNot Nothing Then

            _objetoSelecionado.Selecionado = True

            Dim centro As PointF =
                ConverterPercentualParaTela(
                    _objetoSelecionado.Posicao,
                    campo)

            _offsetMouse = New PointF(
                e.X - centro.X,
                e.Y - centro.Y)

            _arrastando = True

            Capture = True
            Cursor = Cursors.SizeAll

        Else

            _arrastando = False

        End If

        Invalidate()

    End Sub

    Protected Overrides Sub OnMouseMove(
        e As MouseEventArgs)

        MyBase.OnMouseMove(e)

        Dim campo As RectangleF =
            ObterRetanguloCampo()

        If _arrastando AndAlso
           _objetoSelecionado IsNot Nothing Then

            MoverObjetoSelecionado(
                e.Location,
                campo)

            Invalidate()

            Exit Sub

        End If

        Dim objetoSobMouse As ObjetoCampo =
            LocalizarObjetoNaPosicao(
                e.Location,
                campo)

        If objetoSobMouse IsNot Nothing Then
            Cursor = Cursors.Hand
        Else
            Cursor = Cursors.Default
        End If

    End Sub

    Protected Overrides Sub OnMouseUp(
        e As MouseEventArgs)

        MyBase.OnMouseUp(e)

        If e.Button <> MouseButtons.Left Then
            Exit Sub
        End If

        _arrastando = False

        Capture = False
        Cursor = Cursors.Default

    End Sub

    Protected Overrides Sub OnMouseLeave(
        e As EventArgs)

        MyBase.OnMouseLeave(e)

        If Not _arrastando Then
            Cursor = Cursors.Default
        End If

    End Sub

    Private Sub MoverObjetoSelecionado(
    localMouse As Point,
    campo As RectangleF)

        If _objetoSelecionado Is Nothing Then
            Exit Sub
        End If

        If campo.Width <= 0 OrElse
       campo.Height <= 0 Then

            Exit Sub

        End If

        Dim metadeTamanho As SizeF =
        ObterMetadeTamanhoVisual(
            _objetoSelecionado)

        Dim xTela As Single =
        localMouse.X - _offsetMouse.X

        Dim yTela As Single =
        localMouse.Y - _offsetMouse.Y

        xTela = LimitarSingle(
        xTela,
        campo.Left + metadeTamanho.Width,
        campo.Right - metadeTamanho.Width)

        yTela = LimitarSingle(
        yTela,
        campo.Top + metadeTamanho.Height,
        campo.Bottom - metadeTamanho.Height)

        Dim xPercentual As Double =
        ((xTela - campo.Left) /
        campo.Width) * 100.0

        Dim yPercentual As Double =
        ((yTela - campo.Top) /
        campo.Height) * 100.0

        _objetoSelecionado.Posicao.X =
        LimitarPercentual(xPercentual)

        _objetoSelecionado.Posicao.Y =
        LimitarPercentual(yPercentual)

    End Sub

    Private Function LocalizarObjetoNaPosicao(
    localMouse As Point,
    campo As RectangleF) As ObjetoCampo

        For indice As Integer =
        _objetos.Count - 1 To 0 Step -1

            Dim objeto As ObjetoCampo =
            _objetos(indice)

            If Not objeto.Visivel Then
                Continue For
            End If

            If EstaSobreObjeto(
            objeto,
            localMouse,
            campo) Then

                Return objeto

            End If

        Next

        Return Nothing

    End Function

    Private Function EstaSobreObjeto(
    objeto As ObjetoCampo,
    localMouse As Point,
    campo As RectangleF) As Boolean

        Dim centro As PointF =
        ConverterPercentualParaTela(
            objeto.Posicao,
            campo)

        If TypeOf objeto Is Gol Then

            Dim gol As Gol =
            DirectCast(objeto, Gol)

            Dim area As RectangleF =
            ObterRetanguloGol(
                centro,
                gol.Orientacao)

            area.Inflate(
            7.0F,
            7.0F)

            Return area.Contains(
            localMouse.X,
            localMouse.Y)

        End If

        If TypeOf objeto Is Manequim Then

            Dim area As RectangleF =
        ObterRetanguloManequim(centro)

            area.Inflate(
        6.0F,
        6.0F)

            Return area.Contains(
        localMouse.X,
        localMouse.Y)

        End If

        Dim raioSelecao As Single =
        ObterRaioSelecao(objeto)

        Dim distanciaX As Single =
        localMouse.X - centro.X

        Dim distanciaY As Single =
        localMouse.Y - centro.Y

        Dim distanciaQuadrada As Single =
        distanciaX * distanciaX +
        distanciaY * distanciaY

        Return distanciaQuadrada <=
        raioSelecao * raioSelecao

    End Function

    Private Sub DeselecionarTodos()

        For Each objeto As ObjetoCampo In _objetos
            objeto.Selecionado = False
        Next

        _objetoSelecionado = Nothing

    End Sub

#End Region

#Region "Tamanhos"

    Private Function ObterMetadeTamanhoVisual(
        objeto As ObjetoCampo) As SizeF

        If TypeOf objeto Is Jogador Then

            Return New SizeF(
                RaioJogador,
                RaioJogador)

        End If

        If TypeOf objeto Is Bola Then

            Return New SizeF(
                RaioBola,
                RaioBola)

        End If

        If TypeOf objeto Is Cone Then

            Return New SizeF(
                RaioCone,
                RaioCone)

        End If

        If TypeOf objeto Is Gol Then

            Dim gol As Gol =
                DirectCast(objeto, Gol)

            If gol.Orientacao =
               OrientacaoGol.Esquerda OrElse
               gol.Orientacao =
               OrientacaoGol.Direita Then

                Return New SizeF(
                    ProfundidadeGol / 2.0F,
                    LarguraBocaGol / 2.0F)

            End If

            Return New SizeF(
                LarguraBocaGol / 2.0F,
                ProfundidadeGol / 2.0F)

        End If

        If TypeOf objeto Is Manequim Then

            Return New SizeF(
        LarguraManequim / 2.0F,
        AlturaManequim / 2.0F)

        End If

        Return New SizeF(
            15.0F,
            15.0F)

    End Function

    Private Function ObterRaioSelecao(
        objeto As ObjetoCampo) As Single

        If TypeOf objeto Is Jogador Then
            Return RaioJogador + 5.0F
        End If

        If TypeOf objeto Is Bola Then
            Return RaioBola + 6.0F
        End If

        If TypeOf objeto Is Cone Then
            Return RaioCone + 5.0F
        End If

        Return 20.0F

    End Function

    Private Function ObterRetanguloGol(
        centro As PointF,
        orientacao As OrientacaoGol) As RectangleF

        If orientacao =
           OrientacaoGol.Esquerda OrElse
           orientacao =
           OrientacaoGol.Direita Then

            Return New RectangleF(
                centro.X - ProfundidadeGol / 2.0F,
                centro.Y - LarguraBocaGol / 2.0F,
                ProfundidadeGol,
                LarguraBocaGol)

        End If

        Return New RectangleF(
            centro.X - LarguraBocaGol / 2.0F,
            centro.Y - ProfundidadeGol / 2.0F,
            LarguraBocaGol,
            ProfundidadeGol)

    End Function

    Private Function ObterRetanguloManequim(
    centro As PointF) As RectangleF

        Return New RectangleF(
        centro.X - LarguraManequim / 2.0F,
        centro.Y - AlturaManequim / 2.0F,
        LarguraManequim,
        AlturaManequim)

    End Function

#End Region

#Region "Conversões"

    Private Function ObterRetanguloCampo() As RectangleF

        Dim largura As Single =
            ClientSize.Width -
            MargemCampo * 2.0F

        Dim altura As Single =
            ClientSize.Height -
            MargemCampo * 2.0F

        If largura < 1.0F Then
            largura = 1.0F
        End If

        If altura < 1.0F Then
            altura = 1.0F
        End If

        Return New RectangleF(
            MargemCampo,
            MargemCampo,
            largura,
            altura)

    End Function

    Private Function ConverterPercentualParaTela(
        posicao As Posicao,
        campo As RectangleF) As PointF

        Dim x As Single =
            campo.Left +
            CSng(posicao.X / 100.0) *
            campo.Width

        Dim y As Single =
            campo.Top +
            CSng(posicao.Y / 100.0) *
            campo.Height

        Return New PointF(x, y)

    End Function

    Private Function LimitarPercentual(
        valor As Double) As Double

        If valor < 0.0 Then
            Return 0.0
        End If

        If valor > 100.0 Then
            Return 100.0
        End If

        Return valor

    End Function

    Private Function LimitarSingle(
        valor As Single,
        minimo As Single,
        maximo As Single) As Single

        If valor < minimo Then
            Return minimo
        End If

        If valor > maximo Then
            Return maximo
        End If

        Return valor

    End Function

#End Region

End Class