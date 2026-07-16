Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports TacticalStudio.Core.Classes
Imports TacticalStudio.Core.Enums
Imports System.Text.Json

Public Class CampoTatico
    Inherits UserControl

#Region "Variáveis"

    Private ReadOnly _objetos As New List(Of ObjetoCampo)

    Private _objetoSelecionado As ObjetoCampo

    Private _arrastando As Boolean

    Private _offsetMouse As PointF

    Private ReadOnly _historico As New List(Of String)()

    Private _indiceHistorico As Integer = -1

    Private _restaurandoHistorico As Boolean

    Private Const LimiteHistorico As Integer = 100

    Private _modoManipulacao As ModoManipulacaoCampo = ModoManipulacaoCampo.Nenhum

    Private _mouseInicialLinha As PointF

    Private _inicioOriginalLinha As PointF

    Private _fimOriginalLinha As PointF

    Private _estadoAntesManipulacao As String = String.Empty

    Private _houveAlteracaoManipulacao As Boolean

    Private Const RaioManipulador As Single = 8.0F

    Private Const TamanhoMinimoArea As Single = 20.0F

    Private Const ComprimentoMinimoLinha As Single = 15.0F

    Private _ferramentaAtual As FerramentaCampo = FerramentaCampo.Selecionar

    Private _criacaoEmAndamento As Boolean

    Private _pontoInicialCriacao As Posicao

    Private _pontoPreviewTela As PointF

    Private _zoomVisual As Single = 1.0F

    Private _deslocamentoVisual As PointF = PointF.Empty

    Private _panEmAndamento As Boolean

    Private _pontoInicialPan As Point

    Private _deslocamentoInicialPan As PointF

    Private _espacoPressionado As Boolean

    Private Const ZoomMinimo As Single = 0.5F

    Private Const ZoomMaximo As Single = 3.0F

    Private Const PassoZoom As Single = 0.1F

    Private Const MargemCampo As Single = 35.0F

    Private Const RaioJogador As Single = 18.0F

    Private Const RaioBola As Single = 10.0F

    Private Const RaioCone As Single = 14.0F

    Private Const LarguraBocaGol As Single = 44.0F

    Private Const ProfundidadeGol As Single = 22.0F

    Private Const LarguraManequim As Single = 26.0F

    Private Const AlturaManequim As Single = 50.0F

    Private _gradeVisivel As Boolean = False

    Private _encaixeGradeAtivo As Boolean = False

    Private _espacamentoGradePercentual As Integer = 5


    Private ReadOnly _objetosSelecionados As New List(Of ObjetoCampo)()

    Private ReadOnly _estadosMovimentoGrupo As New Dictionary(Of ObjetoCampo, EstadoMovimentoGrupo)()

    Private _pontoInicialMovimentoGrupo As PointF

    Private _movendoGrupo As Boolean

#End Region

    Private Enum ModoManipulacaoCampo

        Nenhum
        MoverObjeto
        LinhaInicio
        LinhaFim
        AreaSuperiorEsquerda
        AreaSuperiorDireita
        AreaInferiorEsquerda
        AreaInferiorDireita

    End Enum

    Private Class EstadoMovimentoGrupo

        Public Property X As Double
        Public Property Y As Double

        Public Property XFinal As Double
        Public Property YFinal As Double

        Public Property PossuiPontoFinal As Boolean

    End Class


    Public Sub New()

        DoubleBuffered = True
        ResizeRedraw = True
        TabStop = True

        Dock = DockStyle.Fill
        BackColor = Tema.Fundo

        SetStyle(
        ControlStyles.AllPaintingInWmPaint Or
        ControlStyles.UserPaint Or
        ControlStyles.OptimizedDoubleBuffer Or
        ControlStyles.ResizeRedraw,
        True)

        UpdateStyles()

        RegistrarEstadoHistorico()

    End Sub

#Region "Eventos"

    Public Event FerramentaAtualAlterada(ferramenta As FerramentaCampo)

    Public Event ObjetoCriado(objeto As ObjetoCampo)

    Public Event ObjetoSelecionadoAlterado(objeto As ObjetoCampo)

    Public Event HistoricoAlterado(podeDesfazer As Boolean, podeRefazer As Boolean)

    Public Event VisualizacaoAlterada(zoomPercentual As Integer)

#End Region

#Region "Propriedades"

    <Browsable(False)>
    <DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)>
    Public ReadOnly Property PodeDesfazer As Boolean

        Get
            Return _indiceHistorico > 0
        End Get

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)>
    Public ReadOnly Property PodeRefazer As Boolean

        Get
            Return _indiceHistorico >= 0 AndAlso
               _indiceHistorico <
               _historico.Count - 1
        End Get

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)>
    Public ReadOnly Property ObjetoSelecionadoAtual As ObjetoCampo

        Get
            Return _objetoSelecionado
        End Get

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)>
    Public ReadOnly Property QuantidadeObjetosSelecionados As Integer

        Get
            Return _objetosSelecionados.Count
        End Get

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)>
    Public Property FerramentaAtual As FerramentaCampo

        Get
            Return _ferramentaAtual
        End Get

        Set(value As FerramentaCampo)

            If _ferramentaAtual = value Then
                Exit Property
            End If

            _ferramentaAtual = value

            CancelarCriacao()

            If value = FerramentaCampo.Selecionar Then
                Cursor = Cursors.Default
            Else
                Cursor = Cursors.Cross
            End If

            RaiseEvent FerramentaAtualAlterada(value)

            Invalidate()

        End Set

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)>
    Public ReadOnly Property ZoomPercentual As Integer

        Get

            Return CInt(
            Math.Round(
                _zoomVisual * 100.0F))

        End Get

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)>
    Public Property GradeVisivel As Boolean

        Get
            Return _gradeVisivel
        End Get

        Set(value As Boolean)

            If _gradeVisivel = value Then
                Exit Property
            End If

            _gradeVisivel =
            value

            Invalidate()

        End Set

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)>
    Public Property EncaixeGradeAtivo As Boolean

        Get
            Return _encaixeGradeAtivo
        End Get

        Set(value As Boolean)

            If _encaixeGradeAtivo = value Then
                Exit Property
            End If

            _encaixeGradeAtivo =
            value

            Invalidate()

        End Set

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)>
    Public Property EspacamentoGradePercentual As Integer

        Get
            Return _espacamentoGradePercentual
        End Get

        Set(value As Integer)

            Dim valorSeguro As Integer =
            5

            If value = 2 OrElse
           value = 5 OrElse
           value = 10 Then

                valorSeguro =
                value

            End If

            If _espacamentoGradePercentual =
           valorSeguro Then

                Exit Property

            End If

            _espacamentoGradePercentual =
            valorSeguro

            Invalidate()

        End Set

    End Property

#End Region

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

    Public Function AdicionarLinha(
    tipo As TipoLinhaTatica,
    cor As CorLinhaTatica,
    xInicial As Double,
    yInicial As Double,
    xFinal As Double,
    yFinal As Double,
    Optional espessura As Single = 3.0F) As LinhaTatica

        Dim linha As New LinhaTatica With {
        .Tipo = tipo,
        .Cor = cor
    }

        If espessura < 1.0F Then
            espessura = 1.0F
        End If

        linha.Espessura = espessura

        linha.Posicao.X =
        LimitarPercentual(xInicial)

        linha.Posicao.Y =
        LimitarPercentual(yInicial)

        linha.PosicaoFinal.X =
        LimitarPercentual(xFinal)

        linha.PosicaoFinal.Y =
        LimitarPercentual(yFinal)

        _objetos.Add(linha)

        Invalidate()

        Return linha

    End Function

    Public Function AdicionarAreaTatica(
    cor As CorAreaTatica,
    xInicial As Double,
    yInicial As Double,
    xFinal As Double,
    yFinal As Double,
    Optional tracejada As Boolean = True,
    Optional opacidade As Integer = 45,
    Optional espessura As Single = 2.5F) As AreaTatica

        If opacidade < 0 Then
            opacidade = 0
        End If

        If opacidade > 255 Then
            opacidade = 255
        End If

        If espessura < 1.0F Then
            espessura = 1.0F
        End If

        Dim area As New AreaTatica With {
        .Cor = cor,
        .Tracejada = tracejada,
        .Opacidade = opacidade,
        .Espessura = espessura
    }

        area.Posicao.X =
        LimitarPercentual(xInicial)

        area.Posicao.Y =
        LimitarPercentual(yInicial)

        area.PosicaoFinal.X =
        LimitarPercentual(xFinal)

        area.PosicaoFinal.Y =
        LimitarPercentual(yFinal)

        _objetos.Add(area)

        Invalidate()

        Return area

    End Function

    Public Function AdicionarMarcador(
    texto As String,
    cor As CorMarcadorTatico,
    xPercentual As Double,
    yPercentual As Double,
    Optional diametro As Single = 34.0F) As MarcadorTatico

        If String.IsNullOrWhiteSpace(texto) Then
            texto = "1"
        End If

        If diametro < 22.0F Then
            diametro = 22.0F
        End If

        If diametro > 80.0F Then
            diametro = 80.0F
        End If

        Dim marcador As New MarcadorTatico With {
        .Texto = texto.Trim(),
        .Cor = cor,
        .Diametro = diametro
    }

        marcador.Posicao.X =
        LimitarPercentual(xPercentual)

        marcador.Posicao.Y =
        LimitarPercentual(yPercentual)

        _objetos.Add(marcador)

        Invalidate()

        Return marcador

    End Function

    Public Function AdicionarTextoTatico(
    texto As String,
    cor As CorTextoTatico,
    xPercentual As Double,
    yPercentual As Double,
    Optional tamanhoFonte As Single = 18.0F,
    Optional negrito As Boolean = True,
    Optional fundoVisivel As Boolean = True) As TextoTatico

        If String.IsNullOrWhiteSpace(texto) Then
            texto = "Instrução"
        End If

        If tamanhoFonte < 8.0F Then
            tamanhoFonte = 8.0F
        End If

        If tamanhoFonte > 48.0F Then
            tamanhoFonte = 48.0F
        End If

        Dim objetoTexto As New TextoTatico With {
        .Texto = texto.Trim(),
        .Cor = cor,
        .TamanhoFonte = tamanhoFonte,
        .Negrito = negrito,
        .FundoVisivel = fundoVisivel
    }

        objetoTexto.Posicao.X =
        LimitarPercentual(xPercentual)

        objetoTexto.Posicao.Y =
        LimitarPercentual(yPercentual)

        _objetos.Add(objetoTexto)

        Invalidate()

        Return objetoTexto

    End Function

    Private Sub ExecutarFerramentaAtual(localMouse As Point, campo As RectangleF)

        Dim percentual As Posicao = ConverterTelaParaPercentual(New PointF(localMouse.X, localMouse.Y), campo)

        percentual = AjustarPosicaoNaGrade(percentual)

        Dim objetoCriado As ObjetoCampo = Nothing

        Select Case FerramentaAtual

            Case FerramentaCampo.Jogador

                objetoCriado =
                AdicionarJogador(
                    1,
                    "Jogador",
                    percentual.X,
                    percentual.Y)

            Case FerramentaCampo.Bola

                objetoCriado =
                AdicionarBola(
                    percentual.X,
                    percentual.Y)

            Case FerramentaCampo.Cone

                objetoCriado =
                AdicionarCone(
                    CorCone.Laranja,
                    percentual.X,
                    percentual.Y)

            Case FerramentaCampo.Gol

                objetoCriado =
                AdicionarGol(
                    OrientacaoGol.Direita,
                    percentual.X,
                    percentual.Y)

            Case FerramentaCampo.Manequim

                objetoCriado =
                AdicionarManequim(
                    CorManequim.Amarelo,
                    percentual.X,
                    percentual.Y)

            Case FerramentaCampo.Marcador

                objetoCriado =
                AdicionarMarcador(
                    "1",
                    CorMarcadorTatico.Branco,
                    percentual.X,
                    percentual.Y,
                    36.0F)

            Case FerramentaCampo.Texto

                objetoCriado =
                AdicionarTextoTatico(
                    "Nova instrução",
                    CorTextoTatico.Branco,
                    percentual.X,
                    percentual.Y,
                    18.0F,
                    True,
                    True)

            Case FerramentaCampo.LinhaContinua,
             FerramentaCampo.LinhaTracejada,
             FerramentaCampo.Seta,
             FerramentaCampo.Area

                ExecutarFerramentaDoisPontos(
                percentual,
                localMouse)

                Exit Sub

        End Select

        If objetoCriado IsNot Nothing Then

            SelecionarObjetoCriado(
            objetoCriado)

        End If

    End Sub

    Private Sub ExecutarFerramentaDoisPontos(
    percentual As Posicao,
    localMouse As Point)

        If Not _criacaoEmAndamento Then

            _pontoInicialCriacao =
            New Posicao(
                percentual.X,
                percentual.Y)

            _pontoPreviewTela =
            New PointF(
                localMouse.X,
                localMouse.Y)

            _criacaoEmAndamento = True

            Invalidate()

            Exit Sub

        End If

        Dim objetoCriado As ObjetoCampo = Nothing

        Select Case FerramentaAtual

            Case FerramentaCampo.LinhaContinua

                objetoCriado =
                AdicionarLinha(
                    TipoLinhaTatica.Continua,
                    CorLinhaTatica.Branca,
                    _pontoInicialCriacao.X,
                    _pontoInicialCriacao.Y,
                    percentual.X,
                    percentual.Y,
                    3.0F)

            Case FerramentaCampo.LinhaTracejada

                objetoCriado =
                AdicionarLinha(
                    TipoLinhaTatica.Tracejada,
                    CorLinhaTatica.Amarela,
                    _pontoInicialCriacao.X,
                    _pontoInicialCriacao.Y,
                    percentual.X,
                    percentual.Y,
                    3.0F)

            Case FerramentaCampo.Seta

                objetoCriado =
                AdicionarLinha(
                    TipoLinhaTatica.Seta,
                    CorLinhaTatica.Vermelha,
                    _pontoInicialCriacao.X,
                    _pontoInicialCriacao.Y,
                    percentual.X,
                    percentual.Y,
                    4.0F)

            Case FerramentaCampo.Area

                objetoCriado =
                AdicionarAreaTatica(
                    CorAreaTatica.Amarela,
                    _pontoInicialCriacao.X,
                    _pontoInicialCriacao.Y,
                    percentual.X,
                    percentual.Y,
                    True,
                    40,
                    2.5F)

        End Select

        CancelarCriacao()

        If objetoCriado IsNot Nothing Then

            SelecionarObjetoCriado(
            objetoCriado)

        End If

    End Sub

    Private Sub SelecionarObjetoCriado(
        objeto As ObjetoCampo)

        SelecionarSomente(objeto)

        RaiseEvent ObjetoCriado(objeto)

        NotificarSelecaoAlterada()

        RegistrarEstadoHistorico()

        Invalidate()

    End Sub

    Private Sub CancelarCriacao()

        _criacaoEmAndamento = False
        _pontoInicialCriacao = Nothing

        Invalidate()

    End Sub

    Public Sub LimparObjetos()

        _objetos.Clear()
        _objetosSelecionados.Clear()
        _estadosMovimentoGrupo.Clear()

        _objetoSelecionado = Nothing
        _arrastando = False
        _movendoGrupo = False

        Capture = False

        Invalidate()

    End Sub

#End Region

#Region "Pintura"

    Public Function GerarImagemCampo(Optional larguraImagem As Integer = 2560) As Bitmap

        Return GerarImagemCampo(
        larguraImagem,
        False,
        String.Empty,
        String.Empty,
        0,
        String.Empty,
        String.Empty)

    End Function

    Public Function GerarImagemCampo(
    larguraImagem As Integer,
    incluirCabecalho As Boolean,
    nomeExercicio As String,
    categoria As String,
    duracaoMinutos As Integer,
    descricao As String,
    observacoes As String) As Bitmap

        If larguraImagem < 800 Then
            larguraImagem = 800
        End If

        If larguraImagem > 7680 Then
            larguraImagem = 7680
        End If

        Dim campoAtual As RectangleF =
        ObterRetanguloCampo()

        If campoAtual.Width <= 1.0F OrElse
       campoAtual.Height <= 1.0F Then

            Throw New InvalidOperationException(
            "O campo não possui tamanho válido para exportação.")

        End If

        Dim margemImagem As Integer =
        CInt(
            Math.Max(
                24.0,
                larguraImagem * 0.0125))

        Dim larguraUtil As Integer =
        larguraImagem -
        margemImagem * 2

        Dim escalaCampo As Single =
        CSng(
            larguraUtil /
            campoAtual.Width)

        Dim alturaCampo As Integer =
        CInt(
            Math.Ceiling(
                campoAtual.Height *
                escalaCampo))

        Dim alturaCabecalho As Integer = 0

        If incluirCabecalho Then

            alturaCabecalho =
            CalcularAlturaCabecalho(
                larguraImagem,
                nomeExercicio,
                categoria,
                duracaoMinutos,
                descricao,
                observacoes)

        End If

        Dim alturaImagem As Integer =
        alturaCabecalho +
        alturaCampo +
        margemImagem * 2

        Dim imagem As New Bitmap(
        larguraImagem,
        alturaImagem,
        System.Drawing.Imaging.PixelFormat.Format32bppArgb)

        Dim objetoSelecionadoAnterior As ObjetoCampo =
        _objetoSelecionado

        Try

            _objetoSelecionado =
            Nothing

            Using g As Graphics =
            Graphics.FromImage(imagem)

                g.Clear(
                Color.FromArgb(
                    28,
                    28,
                    28))

                g.SmoothingMode =
                SmoothingMode.AntiAlias

                g.PixelOffsetMode =
                PixelOffsetMode.HighQuality

                g.CompositingQuality =
                CompositingQuality.HighQuality

                g.InterpolationMode =
                InterpolationMode.HighQualityBicubic

                g.TextRenderingHint =
                System.Drawing.Text.TextRenderingHint.AntiAliasGridFit

                If incluirCabecalho Then

                    DesenharCabecalhoExportacao(
                    g,
                    larguraImagem,
                    alturaCabecalho,
                    nomeExercicio,
                    categoria,
                    duracaoMinutos,
                    descricao,
                    observacoes)

                End If

                Dim deslocamentoX As Single =
                margemImagem -
                campoAtual.Left *
                escalaCampo

                Dim deslocamentoY As Single =
                alturaCabecalho +
                margemImagem -
                campoAtual.Top *
                escalaCampo

                Using matriz As New Matrix(
                escalaCampo,
                0.0F,
                0.0F,
                escalaCampo,
                deslocamentoX,
                deslocamentoY)

                    g.Transform =
                    matriz

                    DesenharGramado(
                    g,
                    campoAtual)

                    DesenharMarcacoes(
                    g,
                    campoAtual)

                    DesenharObjetos(
                    g,
                    campoAtual)

                End Using

            End Using

        Catch

            imagem.Dispose()

            Throw

        Finally

            _objetoSelecionado =
            objetoSelecionadoAnterior

        End Try

        Return imagem

    End Function

    Private Function CalcularAlturaCabecalho(
    larguraImagem As Integer,
    nomeExercicio As String,
    categoria As String,
    duracaoMinutos As Integer,
    descricao As String,
    observacoes As String) As Integer

        Dim fatorEscala As Single =
        CSng(
            larguraImagem /
            2560.0)

        Dim margem As Single =
        Math.Max(
            20.0F,
            48.0F * fatorEscala)

        Dim espacamento As Single =
        Math.Max(
            8.0F,
            18.0F * fatorEscala)

        Dim larguraTexto As Single =
        larguraImagem -
        margem * 2.0F

        Dim tamanhoTitulo As Single =
        Math.Max(
            18.0F,
            46.0F * fatorEscala)

        Dim tamanhoInformacao As Single =
        Math.Max(
            11.0F,
            27.0F * fatorEscala)

        Dim tamanhoCorpo As Single =
        Math.Max(
            10.0F,
            23.0F * fatorEscala)

        Dim nomeSeguro As String =
        If(
            String.IsNullOrWhiteSpace(
                nomeExercicio),
            "Novo exercício",
            nomeExercicio.Trim())

        Dim linhaInformacoes As String =
        MontarLinhaInformacoesExportacao(
            categoria,
            duracaoMinutos)

        Dim textoDescricao As String =
        String.Empty

        If Not String.IsNullOrWhiteSpace(
        descricao) Then

            textoDescricao =
            "Descrição: " &
            descricao.Trim()

        End If

        Dim textoObservacoes As String =
        String.Empty

        If Not String.IsNullOrWhiteSpace(
        observacoes) Then

            textoObservacoes =
            "Observações: " &
            observacoes.Trim()

        End If

        Dim alturaTotal As Single =
        margem

        Using imagemMedicao As New Bitmap(
        1,
        1)

            Using g As Graphics =
            Graphics.FromImage(
                imagemMedicao)

                Using fonteTitulo As New Font(
                "Segoe UI",
                tamanhoTitulo,
                FontStyle.Bold,
                GraphicsUnit.Pixel)

                    alturaTotal +=
                    g.MeasureString(
                        nomeSeguro,
                        fonteTitulo,
                        CInt(larguraTexto)).Height

                End Using

                If Not String.IsNullOrWhiteSpace(
                linhaInformacoes) Then

                    alturaTotal +=
                    espacamento

                    Using fonteInformacao As New Font(
                    "Segoe UI",
                    tamanhoInformacao,
                    FontStyle.Bold,
                    GraphicsUnit.Pixel)

                        alturaTotal +=
                        g.MeasureString(
                            linhaInformacoes,
                            fonteInformacao,
                            CInt(larguraTexto)).Height

                    End Using

                End If

                Using fonteCorpo As New Font(
                "Segoe UI",
                tamanhoCorpo,
                FontStyle.Regular,
                GraphicsUnit.Pixel)

                    If Not String.IsNullOrWhiteSpace(
                    textoDescricao) Then

                        alturaTotal +=
                        espacamento

                        alturaTotal +=
                        g.MeasureString(
                            textoDescricao,
                            fonteCorpo,
                            CInt(larguraTexto)).Height

                    End If

                    If Not String.IsNullOrWhiteSpace(
                    textoObservacoes) Then

                        alturaTotal +=
                        espacamento

                        alturaTotal +=
                        g.MeasureString(
                            textoObservacoes,
                            fonteCorpo,
                            CInt(larguraTexto)).Height

                    End If

                End Using

            End Using

        End Using

        alturaTotal +=
        margem

        Return CInt(
        Math.Ceiling(
            alturaTotal))

    End Function

    Private Function MontarLinhaInformacoesExportacao(
    categoria As String,
    duracaoMinutos As Integer) As String

        Dim categoriaSegura As String =
        If(
            String.IsNullOrWhiteSpace(
                categoria),
            "Sem categoria",
            categoria.Trim())

        If duracaoMinutos > 0 Then

            Return categoriaSegura &
            "  •  " &
            duracaoMinutos.ToString() &
            " minutos"

        End If

        Return categoriaSegura

    End Function

    Private Sub DesenharCabecalhoExportacao(
    g As Graphics,
    larguraImagem As Integer,
    alturaCabecalho As Integer,
    nomeExercicio As String,
    categoria As String,
    duracaoMinutos As Integer,
    descricao As String,
    observacoes As String)

        Dim fatorEscala As Single =
        CSng(
            larguraImagem /
            2560.0)

        Dim margem As Single =
        Math.Max(
            20.0F,
            48.0F * fatorEscala)

        Dim espacamento As Single =
        Math.Max(
            8.0F,
            18.0F * fatorEscala)

        Dim larguraTexto As Single =
        larguraImagem -
        margem * 2.0F

        Dim tamanhoTitulo As Single =
        Math.Max(
            18.0F,
            46.0F * fatorEscala)

        Dim tamanhoInformacao As Single =
        Math.Max(
            11.0F,
            27.0F * fatorEscala)

        Dim tamanhoCorpo As Single =
        Math.Max(
            10.0F,
            23.0F * fatorEscala)

        Using fundo As New SolidBrush(
        Color.FromArgb(
            28,
            28,
            28))

            g.FillRectangle(
            fundo,
            0,
            0,
            larguraImagem,
            alturaCabecalho)

        End Using

        Dim nomeSeguro As String =
        If(
            String.IsNullOrWhiteSpace(
                nomeExercicio),
            "Novo exercício",
            nomeExercicio.Trim())

        Dim linhaInformacoes As String =
        MontarLinhaInformacoesExportacao(
            categoria,
            duracaoMinutos)

        Dim posicaoY As Single =
        margem

        Using pincelTitulo As New SolidBrush(
        Color.White)

            Using fonteTitulo As New Font(
            "Segoe UI",
            tamanhoTitulo,
            FontStyle.Bold,
            GraphicsUnit.Pixel)

                Dim alturaTitulo As Single =
                g.MeasureString(
                    nomeSeguro,
                    fonteTitulo,
                    CInt(larguraTexto)).Height

                g.DrawString(
                nomeSeguro,
                fonteTitulo,
                pincelTitulo,
                New RectangleF(
                    margem,
                    posicaoY,
                    larguraTexto,
                    alturaTitulo + 5.0F))

                posicaoY +=
                alturaTitulo

            End Using

        End Using

        If Not String.IsNullOrWhiteSpace(
        linhaInformacoes) Then

            posicaoY +=
            espacamento

            Using pincelInformacoes As New SolidBrush(
            Color.FromArgb(
                220,
                220,
                220))

                Using fonteInformacoes As New Font(
                "Segoe UI",
                tamanhoInformacao,
                FontStyle.Bold,
                GraphicsUnit.Pixel)

                    Dim alturaInformacoes As Single =
                    g.MeasureString(
                        linhaInformacoes,
                        fonteInformacoes,
                        CInt(larguraTexto)).Height

                    g.DrawString(
                    linhaInformacoes,
                    fonteInformacoes,
                    pincelInformacoes,
                    New RectangleF(
                        margem,
                        posicaoY,
                        larguraTexto,
                        alturaInformacoes + 5.0F))

                    posicaoY +=
                    alturaInformacoes

                End Using

            End Using

        End If

        Using pincelCorpo As New SolidBrush(
        Color.FromArgb(
            205,
            205,
            205))

            Using fonteCorpo As New Font(
            "Segoe UI",
            tamanhoCorpo,
            FontStyle.Regular,
            GraphicsUnit.Pixel)

                If Not String.IsNullOrWhiteSpace(
                descricao) Then

                    posicaoY +=
                    espacamento

                    Dim textoDescricao As String =
                    "Descrição: " &
                    descricao.Trim()

                    Dim alturaDescricao As Single =
                    g.MeasureString(
                        textoDescricao,
                        fonteCorpo,
                        CInt(larguraTexto)).Height

                    g.DrawString(
                    textoDescricao,
                    fonteCorpo,
                    pincelCorpo,
                    New RectangleF(
                        margem,
                        posicaoY,
                        larguraTexto,
                        alturaDescricao + 5.0F))

                    posicaoY +=
                    alturaDescricao

                End If

                If Not String.IsNullOrWhiteSpace(
                observacoes) Then

                    posicaoY +=
                    espacamento

                    Dim textoObservacoes As String =
                    "Observações: " &
                    observacoes.Trim()

                    Dim alturaObservacoes As Single =
                    g.MeasureString(
                        textoObservacoes,
                        fonteCorpo,
                        CInt(larguraTexto)).Height

                    g.DrawString(
                    textoObservacoes,
                    fonteCorpo,
                    pincelCorpo,
                    New RectangleF(
                        margem,
                        posicaoY,
                        larguraTexto,
                        alturaObservacoes + 5.0F))

                End If

            End Using

        End Using

        Dim alturaFaixa As Single =
        Math.Max(
            5.0F,
            9.0F * fatorEscala)

        Using pincelFaixa As New SolidBrush(
        Tema.CorPrimaria)

            g.FillRectangle(
            pincelFaixa,
            0.0F,
            alturaCabecalho -
            alturaFaixa,
            larguraImagem,
            alturaFaixa)

        End Using

    End Sub

    Protected Overrides Sub OnPaint(
    e As PaintEventArgs)

        MyBase.OnPaint(e)

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias

        e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality

        Dim estadoGrafico As GraphicsState = e.Graphics.Save()

        Try

            Using matriz As Matrix = ObterMatrizVisualizacao()

                e.Graphics.Transform = matriz

                DesenharCampo(e.Graphics)

            End Using

        Finally

            e.Graphics.Restore(estadoGrafico)

        End Try

    End Sub

    Private Sub DesenharCampo(g As Graphics)

        g.Clear(BackColor)

        Dim campo As RectangleF = ObterRetanguloCampo()

        If campo.Width <= 0 OrElse campo.Height <= 0 Then
            Exit Sub
        End If

        DesenharGramado(g, campo)

        DesenharGrade(g, campo)

        DesenharMarcacoes(g, campo)

        DesenharObjetos(g, campo)

        DesenharPreVisualizacao(g, campo)

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

    Private Sub DesenharPreVisualizacao(
    g As Graphics,
    campo As RectangleF)

        If Not _criacaoEmAndamento Then
            Exit Sub
        End If

        If _pontoInicialCriacao Is Nothing Then
            Exit Sub
        End If

        Dim inicio As PointF =
        ConverterPercentualParaTela(
            _pontoInicialCriacao,
            campo)

        Dim fim As New PointF(
        LimitarSingle(
            _pontoPreviewTela.X,
            campo.Left,
            campo.Right),
        LimitarSingle(
            _pontoPreviewTela.Y,
            campo.Top,
            campo.Bottom))

        If _encaixeGradeAtivo Then

            fim =
        AjustarPontoTelaNaGrade(
            fim,
            campo)

        End If

        Using caneta As New Pen(
        Color.Gold,
        2.0F)

            caneta.DashStyle =
            DashStyle.Dash

            caneta.StartCap =
            LineCap.Round

            caneta.EndCap =
            LineCap.Round

            Select Case FerramentaAtual

                Case FerramentaCampo.LinhaContinua,
                 FerramentaCampo.LinhaTracejada

                    g.DrawLine(
                    caneta,
                    inicio,
                    fim)

                Case FerramentaCampo.Seta

                    Using ponta As New AdjustableArrowCap(
                    5.0F,
                    7.0F,
                    True)

                        caneta.CustomEndCap = ponta

                        g.DrawLine(
                        caneta,
                        inicio,
                        fim)

                    End Using

                Case FerramentaCampo.Area

                    Dim esquerda As Single =
                    Math.Min(
                        inicio.X,
                        fim.X)

                    Dim topo As Single =
                    Math.Min(
                        inicio.Y,
                        fim.Y)

                    Dim largura As Single =
                    Math.Abs(
                        fim.X -
                        inicio.X)

                    Dim altura As Single =
                    Math.Abs(
                        fim.Y -
                        inicio.Y)

                    Dim retangulo As New RectangleF(
                    esquerda,
                    topo,
                    largura,
                    altura)

                    Using preenchimento As New SolidBrush(
                    Color.FromArgb(
                        35,
                        Color.Gold))

                        g.FillRectangle(
                        preenchimento,
                        retangulo)

                    End Using

                    g.DrawRectangle(
                    caneta,
                    retangulo.X,
                    retangulo.Y,
                    retangulo.Width,
                    retangulo.Height)

            End Select

        End Using

        Using pincel As New SolidBrush(
        Color.Gold)

            g.FillEllipse(
            pincel,
            inicio.X - 5.0F,
            inicio.Y - 5.0F,
            10.0F,
            10.0F)

        End Using

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

            If TypeOf objeto Is AreaTatica Then

                DesenharAreaTatica(
                g,
                DirectCast(objeto, AreaTatica),
                campo)

            End If

        Next

        For Each objeto As ObjetoCampo In _objetos

            If Not objeto.Visivel Then
                Continue For
            End If

            If TypeOf objeto Is AreaTatica OrElse
           TypeOf objeto Is MarcadorTatico OrElse
           TypeOf objeto Is TextoTatico Then

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

                DesenharGol(
                g,
                DirectCast(objeto, Gol),
                campo)

            ElseIf TypeOf objeto Is Manequim Then

                DesenharManequim(
                g,
                DirectCast(objeto, Manequim),
                campo)

            ElseIf TypeOf objeto Is LinhaTatica Then

                DesenharLinhaTatica(
                g,
                DirectCast(objeto, LinhaTatica),
                campo)

            End If

        Next

        For Each objeto As ObjetoCampo In _objetos

            If Not objeto.Visivel Then
                Continue For
            End If

            If TypeOf objeto Is TextoTatico Then

                DesenharTextoTatico(
                g,
                DirectCast(objeto, TextoTatico),
                campo)

            End If

        Next

        For Each objeto As ObjetoCampo In _objetos

            If Not objeto.Visivel Then
                Continue For
            End If

            If TypeOf objeto Is MarcadorTatico Then

                DesenharMarcadorTatico(
                g,
                DirectCast(objeto, MarcadorTatico),
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

        If jogador.Selecionado Then

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

        If bola.Selecionado Then

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

        If cone.Selecionado Then

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

        If gol.Selecionado Then

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

        If manequim.Selecionado Then

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

    Private Sub DesenharLinhaTatica(
    g As Graphics,
    linha As LinhaTatica,
    campo As RectangleF)

        Dim inicio As PointF =
        ConverterPercentualParaTela(
            linha.Posicao,
            campo)

        Dim fim As PointF =
        ConverterPercentualParaTela(
            linha.PosicaoFinal,
            campo)

        Dim corLinha As Color =
        ObterCorLinhaTatica(linha.Cor)

        Using caneta As New Pen(
        corLinha,
        linha.Espessura)

            caneta.StartCap =
            LineCap.Round

            caneta.EndCap =
            LineCap.Round

            If linha.Tipo =
           TipoLinhaTatica.Tracejada Then

                caneta.DashStyle =
                DashStyle.Dash

            End If

            If linha.Tipo =
           TipoLinhaTatica.Seta Then

                Using pontaSeta As New AdjustableArrowCap(
                5.0F,
                7.0F,
                True)

                    caneta.CustomEndCap =
                    pontaSeta

                    g.DrawLine(
                    caneta,
                    inicio,
                    fim)

                End Using

            Else

                g.DrawLine(
                caneta,
                inicio,
                fim)

            End If

        End Using

        If linha.Selecionado Then

            If _objetosSelecionados.Count = 1 Then

                DesenharSelecaoLinha(
                    g,
                    inicio,
                    fim)

            Else

                DesenharSelecaoLinhaMultipla(
                    g,
                    inicio,
                    fim)

            End If

        End If

    End Sub

    Private Sub DesenharSelecaoLinha(
    g As Graphics,
    inicio As PointF,
    fim As PointF)

        Using canetaSelecao As New Pen(
        Color.Gold,
        2.0F)

            canetaSelecao.DashStyle =
            DashStyle.Dot

            g.DrawLine(
            canetaSelecao,
            inicio,
            fim)

        End Using

        Using pincelPonto As New SolidBrush(
        Color.Gold)

            g.FillEllipse(
            pincelPonto,
            inicio.X - 5.0F,
            inicio.Y - 5.0F,
            10.0F,
            10.0F)

            g.FillEllipse(
            pincelPonto,
            fim.X - 5.0F,
            fim.Y - 5.0F,
            10.0F,
            10.0F)

        End Using

        Using bordaPonto As New Pen(
        Color.FromArgb(80, 60, 10),
        1.5F)

            g.DrawEllipse(
            bordaPonto,
            inicio.X - 5.0F,
            inicio.Y - 5.0F,
            10.0F,
            10.0F)

            g.DrawEllipse(
            bordaPonto,
            fim.X - 5.0F,
            fim.Y - 5.0F,
            10.0F,
            10.0F)

        End Using

    End Sub

    Private Sub DesenharSelecaoLinhaMultipla(
        g As Graphics,
        inicio As PointF,
        fim As PointF)

        Using caneta As New Pen(Color.Gold, 2.0F)

            caneta.DashStyle = DashStyle.Dot
            caneta.StartCap = LineCap.Round
            caneta.EndCap = LineCap.Round

            g.DrawLine(caneta, inicio, fim)

        End Using

    End Sub

    Private Function ObterCorLinhaTatica(
    cor As CorLinhaTatica) As Color

        Select Case cor

            Case CorLinhaTatica.Branca
                Return Color.White

            Case CorLinhaTatica.Amarela
                Return Color.FromArgb(245, 210, 35)

            Case CorLinhaTatica.Vermelha
                Return Color.FromArgb(220, 45, 45)

            Case CorLinhaTatica.Azul
                Return Color.FromArgb(45, 125, 230)

            Case Else
                Return Color.White

        End Select

    End Function

    Private Sub DesenharAreaTatica(
    g As Graphics,
    area As AreaTatica,
    campo As RectangleF)

        Dim retangulo As RectangleF =
        ObterRetanguloArea(
            area,
            campo)

        If retangulo.Width < 1.0F OrElse
       retangulo.Height < 1.0F Then

            Exit Sub

        End If

        Dim corArea As Color =
        ObterCorAreaTatica(
            area.Cor)

        Using pincelPreenchimento As New SolidBrush(
        Color.FromArgb(
            area.Opacidade,
            corArea))

            g.FillRectangle(
            pincelPreenchimento,
            retangulo)

        End Using

        Using canetaBorda As New Pen(
        corArea,
        area.Espessura)

            canetaBorda.LineJoin =
            LineJoin.Round

            If area.Tracejada Then

                canetaBorda.DashStyle =
                DashStyle.Dash

            End If

            g.DrawRectangle(
            canetaBorda,
            retangulo.X,
            retangulo.Y,
            retangulo.Width,
            retangulo.Height)

        End Using

        If area.Selecionado Then

            If _objetosSelecionados.Count = 1 Then

                DesenharSelecaoArea(
                    g,
                    retangulo)

            Else

                DesenharSelecaoRetangular(
                    g,
                    retangulo)

            End If

        End If

    End Sub

    Private Sub DesenharSelecaoArea(
    g As Graphics,
    retangulo As RectangleF)

        DesenharSelecaoRetangular(
        g,
        retangulo)

        Dim pontos() As PointF = {
        New PointF(
            retangulo.Left,
            retangulo.Top),
        New PointF(
            retangulo.Right,
            retangulo.Top),
        New PointF(
            retangulo.Left,
            retangulo.Bottom),
        New PointF(
            retangulo.Right,
            retangulo.Bottom)
    }

        Using pincel As New SolidBrush(
        Color.Gold)

            Using borda As New Pen(
            Color.FromArgb(80, 60, 10),
            1.2F)

                For Each ponto As PointF In pontos

                    Dim marcador As New RectangleF(
                    ponto.X - 4.0F,
                    ponto.Y - 4.0F,
                    8.0F,
                    8.0F)

                    g.FillRectangle(
                    pincel,
                    marcador)

                    g.DrawRectangle(
                    borda,
                    marcador.X,
                    marcador.Y,
                    marcador.Width,
                    marcador.Height)

                Next

            End Using

        End Using

    End Sub

    Private Function ObterCorAreaTatica(
    cor As CorAreaTatica) As Color

        Select Case cor

            Case CorAreaTatica.Branca
                Return Color.White

            Case CorAreaTatica.Amarela
                Return Color.FromArgb(245, 210, 35)

            Case CorAreaTatica.Vermelha
                Return Color.FromArgb(220, 45, 45)

            Case CorAreaTatica.Azul
                Return Color.FromArgb(45, 125, 230)

            Case CorAreaTatica.Verde
                Return Color.FromArgb(45, 185, 95)

            Case Else
                Return Color.White

        End Select

    End Function

    Private Sub DesenharMarcadorTatico(
    g As Graphics,
    marcador As MarcadorTatico,
    campo As RectangleF)

        Dim centro As PointF =
        ConverterPercentualParaTela(
            marcador.Posicao,
            campo)

        Dim raio As Single =
        marcador.Diametro / 2.0F

        Dim corFundo As Color =
        ObterCorMarcador(
            marcador.Cor)

        Dim corTexto As Color =
        ObterCorTextoMarcador(
            marcador.Cor)

        Using pincelSombra As New SolidBrush(
        Color.FromArgb(85, 0, 0, 0))

            g.FillEllipse(
            pincelSombra,
            centro.X - raio + 3.0F,
            centro.Y - raio + 4.0F,
            marcador.Diametro,
            marcador.Diametro)

        End Using

        Using pincelFundo As New SolidBrush(
        corFundo)

            g.FillEllipse(
            pincelFundo,
            centro.X - raio,
            centro.Y - raio,
            marcador.Diametro,
            marcador.Diametro)

        End Using

        Using canetaBorda As New Pen(
        Color.FromArgb(210, 30, 30, 30),
        2.0F)

            g.DrawEllipse(
            canetaBorda,
            centro.X - raio,
            centro.Y - raio,
            marcador.Diametro,
            marcador.Diametro)

        End Using

        Dim tamanhoFonte As Single =
        Math.Max(
            9.0F,
            marcador.Diametro * 0.38F)

        Using fonte As New Font(
        "Segoe UI",
        tamanhoFonte,
        FontStyle.Bold,
        GraphicsUnit.Pixel)

            Using pincelTexto As New SolidBrush(
            corTexto)

                Using formato As New StringFormat()

                    formato.Alignment =
                    StringAlignment.Center

                    formato.LineAlignment =
                    StringAlignment.Center

                    Dim areaTexto As New RectangleF(
                    centro.X - raio,
                    centro.Y - raio,
                    marcador.Diametro,
                    marcador.Diametro)

                    g.DrawString(
                    marcador.Texto,
                    fonte,
                    pincelTexto,
                    areaTexto,
                    formato)

                End Using

            End Using

        End Using

        If marcador.Selecionado Then

            DesenharSelecao(
            g,
            centro,
            raio)

        End If

    End Sub

    Private Function ObterCorMarcador(
    cor As CorMarcadorTatico) As Color

        Select Case cor

            Case CorMarcadorTatico.Branco
                Return Color.FromArgb(245, 245, 245)

            Case CorMarcadorTatico.Preto
                Return Color.FromArgb(35, 35, 35)

            Case CorMarcadorTatico.Amarelo
                Return Color.FromArgb(245, 205, 35)

            Case CorMarcadorTatico.Vermelho
                Return Color.FromArgb(205, 45, 45)

            Case CorMarcadorTatico.Azul
                Return Color.FromArgb(45, 115, 210)

            Case CorMarcadorTatico.Verde
                Return Color.FromArgb(45, 165, 85)

            Case Else
                Return Color.White

        End Select

    End Function

    Private Function ObterCorTextoMarcador(
    cor As CorMarcadorTatico) As Color

        Select Case cor

            Case CorMarcadorTatico.Branco,
             CorMarcadorTatico.Amarelo

                Return Color.FromArgb(
                30,
                30,
                30)

            Case Else

                Return Color.White

        End Select

    End Function

    Private Sub DesenharGrade(
    g As Graphics,
    campo As RectangleF)

        If Not _gradeVisivel Then
            Exit Sub
        End If

        Dim passo As Integer =
        _espacamentoGradePercentual

        If passo <= 0 Then
            Exit Sub
        End If

        Dim espessuraSecundaria As Single =
        0.8F /
        Math.Max(
            _zoomVisual,
            0.5F)

        Dim espessuraPrincipal As Single =
        1.3F /
        Math.Max(
            _zoomVisual,
            0.5F)

        Using canetaSecundaria As New Pen(
        Color.FromArgb(
            65,
            255,
            255,
            255),
        espessuraSecundaria)

            canetaSecundaria.DashStyle =
            DashStyle.Dot

            Using canetaPrincipal As New Pen(
            Color.FromArgb(
                115,
                255,
                255,
                255),
            espessuraPrincipal)

                canetaPrincipal.DashStyle =
                DashStyle.Dash

                For valor As Integer =
                passo To 99 Step passo

                    Dim percentual As Single =
                        valor / 100.0F

                    Dim x As Single =
                    campo.Left +
                    campo.Width *
                    percentual

                    Dim y As Single =
                    campo.Top +
                    campo.Height *
                    percentual

                    Dim linhaPrincipal As Boolean =
                    valor Mod (
                        passo * 5) = 0

                    Dim canetaAtual As Pen

                    If linhaPrincipal Then

                        canetaAtual =
                        canetaPrincipal

                    Else

                        canetaAtual =
                        canetaSecundaria

                    End If

                    g.DrawLine(
                    canetaAtual,
                    x,
                    campo.Top,
                    x,
                    campo.Bottom)

                    g.DrawLine(
                    canetaAtual,
                    campo.Left,
                    y,
                    campo.Right,
                    y)

                Next

            End Using

        End Using

    End Sub

    Private Sub DesenharTextoTatico(
    g As Graphics,
    texto As TextoTatico,
    campo As RectangleF)

        Dim centro As PointF =
        ConverterPercentualParaTela(
            texto.Posicao,
            campo)

        Dim retangulo As RectangleF =
        ObterRetanguloTextoTatico(
            centro,
            texto)

        Dim corTexto As Color =
        ObterCorTextoTatico(
            texto.Cor)

        Using fonte As Font =
        CriarFonteTextoTatico(texto)

            If texto.FundoVisivel Then

                Using pincelSombra As New SolidBrush(
                Color.FromArgb(75, 0, 0, 0))

                    g.FillRectangle(
                    pincelSombra,
                    retangulo.X + 3.0F,
                    retangulo.Y + 3.0F,
                    retangulo.Width,
                    retangulo.Height)

                End Using

                Using pincelFundo As New SolidBrush(
                Color.FromArgb(175, 25, 25, 25))

                    g.FillRectangle(
                    pincelFundo,
                    retangulo)

                End Using

                Using bordaFundo As New Pen(
                Color.FromArgb(150, 255, 255, 255),
                1.0F)

                    g.DrawRectangle(
                    bordaFundo,
                    retangulo.X,
                    retangulo.Y,
                    retangulo.Width,
                    retangulo.Height)

                End Using

            End If

            Dim areaTexto As Rectangle =
            Rectangle.Round(retangulo)

            Dim formato As TextFormatFlags =
            TextFormatFlags.HorizontalCenter Or
            TextFormatFlags.VerticalCenter Or
            TextFormatFlags.SingleLine Or
            TextFormatFlags.NoPadding

            Dim areaSombra As New Rectangle(
            areaTexto.X + 1,
            areaTexto.Y + 1,
            areaTexto.Width,
            areaTexto.Height)

            TextRenderer.DrawText(
            g,
            texto.Texto,
            fonte,
            areaSombra,
            Color.FromArgb(185, 0, 0, 0),
            formato)

            TextRenderer.DrawText(
            g,
            texto.Texto,
            fonte,
            areaTexto,
            corTexto,
            formato)

        End Using

        If texto.Selecionado Then

            DesenharSelecaoRetangular(
            g,
            retangulo)

        End If

    End Sub

    Private Function CriarFonteTextoTatico(
    texto As TextoTatico) As Font

        Dim estilo As FontStyle =
        FontStyle.Regular

        If texto.Negrito Then
            estilo = FontStyle.Bold
        End If

        Return New Font(
        "Segoe UI",
        texto.TamanhoFonte,
        estilo,
        GraphicsUnit.Pixel)

    End Function

    Private Function ObterTamanhoTextoTatico(
    texto As TextoTatico) As SizeF

        Using fonte As Font =
        CriarFonteTextoTatico(texto)

            Dim tamanho As Size =
            TextRenderer.MeasureText(
                texto.Texto,
                fonte,
                New Size(
                    Integer.MaxValue,
                    Integer.MaxValue),
                TextFormatFlags.SingleLine Or
                TextFormatFlags.NoPadding)

            Return New SizeF(
            tamanho.Width + 16.0F,
            tamanho.Height + 10.0F)

        End Using

    End Function

    Private Function ObterRetanguloTextoTatico(
    centro As PointF,
    texto As TextoTatico) As RectangleF

        Dim tamanho As SizeF =
        ObterTamanhoTextoTatico(texto)

        Return New RectangleF(
        centro.X - tamanho.Width / 2.0F,
        centro.Y - tamanho.Height / 2.0F,
        tamanho.Width,
        tamanho.Height)

    End Function

    Private Function ObterCorTextoTatico(
    cor As CorTextoTatico) As Color

        Select Case cor

            Case CorTextoTatico.Branco
                Return Color.White

            Case CorTextoTatico.Preto
                Return Color.FromArgb(25, 25, 25)

            Case CorTextoTatico.Amarelo
                Return Color.FromArgb(250, 215, 35)

            Case CorTextoTatico.Vermelho
                Return Color.FromArgb(235, 55, 55)

            Case CorTextoTatico.Azul
                Return Color.FromArgb(70, 145, 245)

            Case CorTextoTatico.Verde
                Return Color.FromArgb(65, 205, 110)

            Case Else
                Return Color.White

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

#Region "Duplicação e camadas"

    Public Sub DuplicarSelecionado()

        If _objetosSelecionados.Count = 0 Then
            Exit Sub
        End If

        Dim objetosOriginais As New List(Of ObjetoCampo)(_objetosSelecionados)
        Dim objetosDuplicados As New List(Of ObjetoCampo)()

        For Each objetoOriginal As ObjetoCampo In objetosOriginais

            Dim estadoObjeto As EstadoObjetoCampo =
                CapturarEstadoObjeto(objetoOriginal)

            Dim objetoDuplicado As ObjetoCampo =
                CriarObjetoDoEstado(estadoObjeto)

            If objetoDuplicado Is Nothing Then
                Continue For
            End If

            DeslocarObjetoDuplicado(
                objetoDuplicado,
                4.0,
                4.0)

            _objetos.Add(objetoDuplicado)
            objetosDuplicados.Add(objetoDuplicado)

            RaiseEvent ObjetoCriado(objetoDuplicado)

        Next

        If objetosDuplicados.Count = 0 Then
            Exit Sub
        End If

        DeselecionarTodos()

        For Each objeto As ObjetoCampo In objetosDuplicados

            objeto.Selecionado = True
            _objetosSelecionados.Add(objeto)

        Next

        _objetoSelecionado =
            objetosDuplicados(objetosDuplicados.Count - 1)

        NotificarSelecaoAlterada()

        RegistrarEstadoHistorico()

        Invalidate()

    End Sub

    Private Sub DeslocarObjetoDuplicado(
    objeto As ObjetoCampo,
    deslocamentoX As Double,
    deslocamentoY As Double)

        If TypeOf objeto Is LinhaTatica Then

            Dim linha As LinhaTatica =
            DirectCast(
                objeto,
                LinhaTatica)

            DeslocarObjetoDoisPontos(
            linha.Posicao,
            linha.PosicaoFinal,
            deslocamentoX,
            deslocamentoY)

            Exit Sub

        End If

        If TypeOf objeto Is AreaTatica Then

            Dim area As AreaTatica =
            DirectCast(
                objeto,
                AreaTatica)

            DeslocarObjetoDoisPontos(
            area.Posicao,
            area.PosicaoFinal,
            deslocamentoX,
            deslocamentoY)

            Exit Sub

        End If

        Dim novoX As Double =
        objeto.Posicao.X +
        deslocamentoX

        Dim novoY As Double =
        objeto.Posicao.Y +
        deslocamentoY

        If novoX > 100.0 Then

            novoX =
            objeto.Posicao.X -
            deslocamentoX

        End If

        If novoY > 100.0 Then

            novoY =
            objeto.Posicao.Y -
            deslocamentoY

        End If

        objeto.Posicao.X =
        LimitarPercentual(
            novoX)

        objeto.Posicao.Y =
        LimitarPercentual(
            novoY)

    End Sub

    Private Sub DeslocarObjetoDoisPontos(
    pontoInicial As Posicao,
    pontoFinal As Posicao,
    deslocamentoX As Double,
    deslocamentoY As Double)

        Dim menorX As Double =
        Math.Min(
            pontoInicial.X,
            pontoFinal.X)

        Dim maiorX As Double =
        Math.Max(
            pontoInicial.X,
            pontoFinal.X)

        Dim menorY As Double =
        Math.Min(
            pontoInicial.Y,
            pontoFinal.Y)

        Dim maiorY As Double =
        Math.Max(
            pontoInicial.Y,
            pontoFinal.Y)

        Dim deltaX As Double =
        CalcularDeslocamentoPermitido(
            menorX,
            maiorX,
            deslocamentoX)

        Dim deltaY As Double =
        CalcularDeslocamentoPermitido(
            menorY,
            maiorY,
            deslocamentoY)

        pontoInicial.X += deltaX
        pontoInicial.Y += deltaY

        pontoFinal.X += deltaX
        pontoFinal.Y += deltaY

    End Sub

    Private Function CalcularDeslocamentoPermitido(
    menorValor As Double,
    maiorValor As Double,
    deslocamentoDesejado As Double) As Double

        If menorValor + deslocamentoDesejado >= 0.0 AndAlso
       maiorValor + deslocamentoDesejado <= 100.0 Then

            Return deslocamentoDesejado

        End If

        Dim deslocamentoInverso As Double =
        -deslocamentoDesejado

        If menorValor + deslocamentoInverso >= 0.0 AndAlso
       maiorValor + deslocamentoInverso <= 100.0 Then

            Return deslocamentoInverso

        End If

        Return Math.Max(
        -menorValor,
        Math.Min(
            100.0 - maiorValor,
            deslocamentoDesejado))

    End Function

    Private Function ObterCamadaVisual(
    objeto As ObjetoCampo) As Integer

        If TypeOf objeto Is AreaTatica Then
            Return 0
        End If

        If TypeOf objeto Is TextoTatico Then
            Return 2
        End If

        If TypeOf objeto Is MarcadorTatico Then
            Return 3
        End If

        Return 1

    End Function

    Public Sub TrazerParaFrente()

        If _objetoSelecionado Is Nothing Then
            Exit Sub
        End If

        Dim indiceAtual As Integer =
        _objetos.IndexOf(
            _objetoSelecionado)

        If indiceAtual < 0 Then
            Exit Sub
        End If

        Dim camadaAtual As Integer =
        ObterCamadaVisual(
            _objetoSelecionado)

        Dim ultimoIndiceCamada As Integer =
        indiceAtual

        For indice As Integer =
        indiceAtual + 1 To _objetos.Count - 1

            If ObterCamadaVisual(
                _objetos(indice)) =
               camadaAtual Then

                ultimoIndiceCamada =
                indice

            End If

        Next

        If ultimoIndiceCamada =
       indiceAtual Then

            Exit Sub

        End If

        Dim objeto As ObjetoCampo =
        _objetoSelecionado

        _objetos.RemoveAt(
        indiceAtual)

        _objetos.Add(
        objeto)

        RegistrarEstadoHistorico()

        Invalidate()

    End Sub

    Public Sub EnviarParaTras()

        If _objetoSelecionado Is Nothing Then
            Exit Sub
        End If

        Dim indiceAtual As Integer =
        _objetos.IndexOf(
            _objetoSelecionado)

        If indiceAtual < 0 Then
            Exit Sub
        End If

        Dim camadaAtual As Integer =
        ObterCamadaVisual(
            _objetoSelecionado)

        Dim primeiroIndiceCamada As Integer =
        indiceAtual

        For indice As Integer =
        indiceAtual - 1 To 0 Step -1

            If ObterCamadaVisual(
            _objetos(indice)) =
           camadaAtual Then

                primeiroIndiceCamada =
                indice

            End If

        Next

        If primeiroIndiceCamada =
       indiceAtual Then

            Exit Sub

        End If

        Dim objeto As ObjetoCampo =
        _objetoSelecionado

        _objetos.RemoveAt(
        indiceAtual)

        _objetos.Insert(
        0,
        objeto)

        RegistrarEstadoHistorico()

        Invalidate()

    End Sub

#End Region

#Region "Histórico"

    Public Sub RegistrarAlteracaoExterna()

        Invalidate()

        RegistrarEstadoHistorico()

    End Sub

    Public Sub Desfazer()

        If Not PodeDesfazer Then
            Exit Sub
        End If

        _indiceHistorico -= 1

        RestaurarEstadoHistorico(_historico(_indiceHistorico))

        AtualizarDisponibilidadeHistorico()

    End Sub

    Public Sub Refazer()

        If Not PodeRefazer Then
            Exit Sub
        End If

        _indiceHistorico += 1

        RestaurarEstadoHistorico(
        _historico(_indiceHistorico))

        AtualizarDisponibilidadeHistorico()

    End Sub

    Private Sub RegistrarEstadoHistorico()

        If _restaurandoHistorico Then
            Exit Sub
        End If

        AdicionarEstadoAoHistorico(
        CapturarEstadoAtualJson())

    End Sub

    Private Sub AdicionarEstadoAoHistorico(
    estadoJson As String)

        If _restaurandoHistorico Then
            Exit Sub
        End If

        If String.IsNullOrWhiteSpace(
        estadoJson) Then

            Exit Sub

        End If

        If _indiceHistorico >= 0 AndAlso
       _indiceHistorico < _historico.Count Then

            If _historico(_indiceHistorico) =
           estadoJson Then

                Exit Sub

            End If

        End If

        'Uma nova alteração depois de desfazer
        'elimina os estados disponíveis para refazer.
        If _indiceHistorico <
       _historico.Count - 1 Then

            Dim indiceInicial As Integer =
            _indiceHistorico + 1

            Dim quantidade As Integer =
            _historico.Count -
            indiceInicial

            If quantidade > 0 Then

                _historico.RemoveRange(
                indiceInicial,
                quantidade)

            End If

        End If

        _historico.Add(estadoJson)

        If _historico.Count >
       LimiteHistorico Then

            _historico.RemoveAt(0)

        End If

        _indiceHistorico =
        _historico.Count - 1

        AtualizarDisponibilidadeHistorico()

    End Sub

    Private Sub AtualizarDisponibilidadeHistorico()

        RaiseEvent HistoricoAlterado(
        PodeDesfazer,
        PodeRefazer)

    End Sub

    Private Function CapturarEstadoAtual() As EstadoCampo

        Dim estadoCampo As New EstadoCampo()

        For Each objeto As ObjetoCampo In _objetos

            estadoCampo.Objetos.Add(
            CapturarEstadoObjeto(objeto))

        Next

        Return estadoCampo

    End Function

    Private Function CapturarEstadoAtualJson() As String

        Return JsonSerializer.Serialize(
        CapturarEstadoAtual())

    End Function

    Private Function CapturarEstadoObjeto(
    objeto As ObjetoCampo) As EstadoObjetoCampo

        If objeto Is Nothing Then

            Throw New ArgumentNullException(
            NameOf(objeto))

        End If

        Dim estado As New EstadoObjetoCampo With {
        .TipoObjeto = objeto.GetType().Name,
        .X = objeto.Posicao.X,
        .Y = objeto.Posicao.Y,
        .Visivel = objeto.Visivel
    }

        Select Case estado.TipoObjeto

            Case "Jogador"

                Dim objetoJogador As Jogador =
                DirectCast(
                    objeto,
                    Jogador)

                estado.Numero =
                objetoJogador.Numero

                estado.Nome =
                objetoJogador.Nome

            Case "Bola"

            'A bola não possui propriedades adicionais.
            'Posição e visibilidade já foram registradas acima.

            Case "Cone"

                Dim objetoCone As Cone =
                DirectCast(
                    objeto,
                    Cone)

                estado.CorConeValor =
                CInt(objetoCone.Cor)

            Case "Gol"

                Dim objetoGol As Gol =
                DirectCast(
                    objeto,
                    Gol)

                estado.OrientacaoGolValor =
                CInt(objetoGol.Orientacao)

            Case "Manequim"

                Dim objetoManequim As Manequim =
                DirectCast(
                    objeto,
                    Manequim)

                estado.CorManequimValor =
                CInt(objetoManequim.Cor)

            Case "LinhaTatica"

                Dim objetoLinha As LinhaTatica =
                DirectCast(
                    objeto,
                    LinhaTatica)

                estado.XFinal =
                objetoLinha.PosicaoFinal.X

                estado.YFinal =
                objetoLinha.PosicaoFinal.Y

                estado.TipoLinhaValor =
                CInt(objetoLinha.Tipo)

                estado.CorLinhaValor =
                CInt(objetoLinha.Cor)

                estado.Espessura =
                objetoLinha.Espessura

            Case "AreaTatica"

                Dim objetoArea As AreaTatica =
                DirectCast(
                    objeto,
                    AreaTatica)

                estado.XFinal =
                objetoArea.PosicaoFinal.X

                estado.YFinal =
                objetoArea.PosicaoFinal.Y

                estado.CorAreaValor =
                CInt(objetoArea.Cor)

                estado.Espessura =
                objetoArea.Espessura

                estado.Opacidade =
                objetoArea.Opacidade

                estado.Tracejada =
                objetoArea.Tracejada

            Case "MarcadorTatico"

                Dim objetoMarcador As MarcadorTatico =
                DirectCast(
                    objeto,
                    MarcadorTatico)

                estado.Texto =
                objetoMarcador.Texto

                estado.CorMarcadorValor =
                CInt(objetoMarcador.Cor)

                estado.Diametro =
                objetoMarcador.Diametro

            Case "TextoTatico"

                Dim objetoTexto As TextoTatico =
                DirectCast(
                    objeto,
                    TextoTatico)

                estado.Texto =
                objetoTexto.Texto

                estado.CorTextoValor =
                CInt(objetoTexto.Cor)

                estado.TamanhoFonte =
                objetoTexto.TamanhoFonte

                estado.Negrito =
                objetoTexto.Negrito

                estado.FundoVisivel =
                objetoTexto.FundoVisivel

            Case Else

                Throw New NotSupportedException(
                "Objeto não suportado no histórico: " &
                estado.TipoObjeto)

        End Select

        Return estado

    End Function

    Private Sub RestaurarEstadoHistorico(
    estadoJson As String)

        Dim estadoCampo As EstadoCampo =
        JsonSerializer.Deserialize(
            Of EstadoCampo)(
            estadoJson)

        If estadoCampo Is Nothing Then
            Exit Sub
        End If

        AplicarEstadoCampo(
        estadoCampo)

    End Sub

    Private Function CriarObjetoDoEstado(
    estado As EstadoObjetoCampo) As ObjetoCampo

        Dim objeto As ObjetoCampo = Nothing

        Select Case estado.TipoObjeto

            Case "Jogador"

                objeto = New Jogador With {
            .Numero = estado.Numero,
            .Nome = estado.Nome
        }

            Case "Bola"

                objeto = New Bola()

            Case "Cone"

                objeto = New Cone With {
            .Cor = CType(
                estado.CorConeValor,
                CorCone)
        }

            Case "Gol"

                objeto = New Gol With {
            .Orientacao = CType(
                estado.OrientacaoGolValor,
                OrientacaoGol)
        }

            Case "Manequim"

                objeto = New Manequim With {
            .Cor = CType(
                estado.CorManequimValor,
                CorManequim)
        }

            Case "LinhaTatica"

                Dim linha As New LinhaTatica With {
            .Tipo = CType(
                estado.TipoLinhaValor,
                TipoLinhaTatica),
            .Cor = CType(
                estado.CorLinhaValor,
                CorLinhaTatica),
            .Espessura = estado.Espessura
        }

                linha.PosicaoFinal.X =
            estado.XFinal

                linha.PosicaoFinal.Y =
            estado.YFinal

                objeto = linha

            Case "AreaTatica"

                Dim area As New AreaTatica With {
            .Cor = CType(
                estado.CorAreaValor,
                CorAreaTatica),
            .Espessura = estado.Espessura,
            .Opacidade = estado.Opacidade,
            .Tracejada = estado.Tracejada
        }

                area.PosicaoFinal.X =
            estado.XFinal

                area.PosicaoFinal.Y =
            estado.YFinal

                objeto = area

            Case "MarcadorTatico"

                objeto = New MarcadorTatico With {
            .Texto = estado.Texto,
            .Cor = CType(
                estado.CorMarcadorValor,
                CorMarcadorTatico),
            .Diametro = estado.Diametro
        }

            Case "TextoTatico"

                objeto = New TextoTatico With {
            .Texto = estado.Texto,
            .Cor = CType(
                estado.CorTextoValor,
                CorTextoTatico),
            .TamanhoFonte = estado.TamanhoFonte,
            .Negrito = estado.Negrito,
            .FundoVisivel = estado.FundoVisivel
        }

        End Select

        If objeto Is Nothing Then
            Return Nothing
        End If

        objeto.Posicao.X =
        estado.X

        objeto.Posicao.Y =
        estado.Y

        objeto.Visivel =
        estado.Visivel

        objeto.Selecionado =
        False

        Return objeto

    End Function

    Private Sub IniciarTransacaoManipulacao()

        _estadoAntesManipulacao =
        CapturarEstadoAtualJson()

        _houveAlteracaoManipulacao =
        False

    End Sub

    Private Sub FinalizarTransacaoManipulacao()

        If Not _houveAlteracaoManipulacao Then

            _estadoAntesManipulacao =
            String.Empty

            Exit Sub

        End If

        Dim estadoDepois As String =
        CapturarEstadoAtualJson()

        'Garante que o estado anterior à movimentação
        'esteja imediatamente antes do novo estado.
        AdicionarEstadoAoHistorico(
        _estadoAntesManipulacao)

        AdicionarEstadoAoHistorico(
        estadoDepois)

        _estadoAntesManipulacao =
        String.Empty

        _houveAlteracaoManipulacao =
        False

    End Sub

    Public Function ExportarExercicioJson(
    nomeExercicio As String,
    categoria As String,
    duracaoMinutos As Integer,
    descricao As String,
    observacoes As String) As String

        If String.IsNullOrWhiteSpace(
        nomeExercicio) Then

            nomeExercicio =
            "Novo exercício"

        End If

        If String.IsNullOrWhiteSpace(
        categoria) Then

            categoria =
            "Tático"

        End If

        If duracaoMinutos < 1 Then

            duracaoMinutos =
            1

        End If

        Dim arquivo As New ArquivoExercicio With {
        .VersaoFormato = 2,
        .Nome = nomeExercicio.Trim(),
        .Categoria = categoria.Trim(),
        .DuracaoMinutos = duracaoMinutos,
        .Descricao = If(
            descricao,
            String.Empty),
        .Observacoes = If(
            observacoes,
            String.Empty),
        .Campo = CapturarEstadoAtual()
    }

        Dim opcoes As New JsonSerializerOptions With {
        .WriteIndented = True
    }

        Return JsonSerializer.Serialize(
        arquivo,
        opcoes)

    End Function

    Public Function ImportarExercicioJson(
    conteudoJson As String) As ArquivoExercicio

        If String.IsNullOrWhiteSpace(
        conteudoJson) Then

            Throw New ArgumentException(
            "O arquivo está vazio.")

        End If

        Dim arquivo As ArquivoExercicio =
        JsonSerializer.Deserialize(
            Of ArquivoExercicio)(
            conteudoJson)

        If arquivo Is Nothing OrElse
       arquivo.Campo Is Nothing Then

            Throw New System.IO.InvalidDataException(
            "O arquivo não contém um exercício válido.")

        End If

        If arquivo.VersaoFormato > 2 Then

            Throw New System.IO.InvalidDataException(
            "Este exercício foi criado em uma versão mais recente do TacticalStudio.")

        End If

        AplicarEstadoCampo(
        arquivo.Campo)

        ReiniciarHistorico()

        Return arquivo

    End Function

    Public Sub NovoExercicio()

        AplicarEstadoCampo(
        New EstadoCampo())

        ReiniciarHistorico()

    End Sub

    Public Function ObterAssinaturaEstado() As String

        Return CapturarEstadoAtualJson()

    End Function

    Private Sub AplicarEstadoCampo(
    estadoCampo As EstadoCampo)

        _restaurandoHistorico = True

        Try

            _objetos.Clear()

            _objetosSelecionados.Clear()
            _estadosMovimentoGrupo.Clear()
            _movendoGrupo = False

            If estadoCampo IsNot Nothing AndAlso
           estadoCampo.Objetos IsNot Nothing Then

                For Each estadoObjeto As EstadoObjetoCampo
                In estadoCampo.Objetos

                    Dim objeto As ObjetoCampo =
                    CriarObjetoDoEstado(
                        estadoObjeto)

                    If objeto IsNot Nothing Then

                        _objetos.Add(
                        objeto)

                    End If

                Next

            End If

            _objetoSelecionado = Nothing
            _arrastando = False

            _modoManipulacao =
            ModoManipulacaoCampo.Nenhum

            _houveAlteracaoManipulacao =
            False

            _estadoAntesManipulacao =
            String.Empty

            Capture = False

            CancelarCriacao()

        Finally

            _restaurandoHistorico = False

        End Try

        RaiseEvent ObjetoSelecionadoAlterado(
        Nothing)

        Invalidate()

    End Sub

    Private Sub ReiniciarHistorico()

        _historico.Clear()

        _indiceHistorico = -1

        RegistrarEstadoHistorico()

        AtualizarDisponibilidadeHistorico()

    End Sub

#End Region

#Region "Mouse"

    Protected Overrides Sub OnMouseDown(
        e As MouseEventArgs)

        MyBase.OnMouseDown(e)

        Focus()

        If e.Button = MouseButtons.Middle OrElse
           (_espacoPressionado AndAlso e.Button = MouseButtons.Left) Then

            _panEmAndamento = True
            _pontoInicialPan = e.Location
            _deslocamentoInicialPan = _deslocamentoVisual

            Capture = True
            Cursor = Cursors.SizeAll

            Exit Sub

        End If

        e = CriarEventoMouseMundo(e)

        Dim campo As RectangleF = ObterRetanguloCampo()

        If e.Button = MouseButtons.Right Then

            CancelarCriacao()
            Exit Sub

        End If

        If e.Button <> MouseButtons.Left Then
            Exit Sub
        End If

        If Not campo.Contains(CSng(e.X), CSng(e.Y)) Then
            Exit Sub
        End If

        If FerramentaAtual <> FerramentaCampo.Selecionar Then

            ExecutarFerramentaAtual(e.Location, campo)
            Exit Sub

        End If

        Dim selecaoAdicional As Boolean =
            (ModifierKeys And Keys.Control) = Keys.Control OrElse
            (ModifierKeys And Keys.Shift) = Keys.Shift

        If Not selecaoAdicional AndAlso
           _objetosSelecionados.Count = 1 Then

            Dim manipuladorAtual As ModoManipulacaoCampo =
                LocalizarManipuladorSelecionado(e.Location, campo)

            If manipuladorAtual <> ModoManipulacaoCampo.Nenhum Then

                IniciarManipulacaoPorAlca(
                    manipuladorAtual,
                    e.Location,
                    campo)

                Exit Sub

            End If

        End If

        Dim objetoClicado As ObjetoCampo =
            LocalizarObjetoNaPosicao(e.Location, campo)

        If selecaoAdicional Then

            If objetoClicado IsNot Nothing Then

                AlternarSelecaoObjeto(objetoClicado)
                NotificarSelecaoAlterada()

            End If

            Exit Sub

        End If

        If objetoClicado Is Nothing Then

            DeselecionarTodos()

            _arrastando = False
            _modoManipulacao = ModoManipulacaoCampo.Nenhum

            NotificarSelecaoAlterada()

            Cursor = Cursors.Default

            Exit Sub

        End If

        If Not ObjetoEstaSelecionado(objetoClicado) Then

            SelecionarSomente(objetoClicado)
            NotificarSelecaoAlterada()

        Else

            _objetoSelecionado = objetoClicado

        End If

        If _objetosSelecionados.Count = 1 Then

            Dim manipuladorNovo As ModoManipulacaoCampo =
                LocalizarManipuladorSelecionado(e.Location, campo)

            If manipuladorNovo <> ModoManipulacaoCampo.Nenhum Then

                IniciarManipulacaoPorAlca(
                    manipuladorNovo,
                    e.Location,
                    campo)

                Exit Sub

            End If

        End If

        If _objetosSelecionados.Count > 1 Then

            PrepararMovimentoGrupo(e.Location)

        Else

            _movendoGrupo = False

            If TypeOf _objetoSelecionado Is LinhaTatica Then

                PrepararManipulacaoLinha(
                    DirectCast(_objetoSelecionado, LinhaTatica),
                    e.Location,
                    campo)

            Else

                Dim centro As PointF =
                    ObterCentroObjetoTela(_objetoSelecionado, campo)

                _offsetMouse = New PointF(
                    e.X - centro.X,
                    e.Y - centro.Y)

            End If

        End If

        _houveAlteracaoManipulacao = False
        _modoManipulacao = ModoManipulacaoCampo.MoverObjeto
        _arrastando = True

        Capture = True
        Cursor = Cursors.SizeAll

        Invalidate()

    End Sub

    Protected Overrides Sub OnMouseMove(
        e As MouseEventArgs)

        MyBase.OnMouseMove(e)

        If _panEmAndamento Then

            Dim deltaX As Single = e.X - _pontoInicialPan.X
            Dim deltaY As Single = e.Y - _pontoInicialPan.Y

            _deslocamentoVisual =
                New PointF(
                    _deslocamentoInicialPan.X + deltaX,
                    _deslocamentoInicialPan.Y + deltaY)

            LimitarDeslocamentoVisual()

            Cursor = Cursors.SizeAll

            Invalidate()
            Exit Sub

        End If

        e = CriarEventoMouseMundo(e)

        Dim campo As RectangleF = ObterRetanguloCampo()

        If FerramentaAtual <> FerramentaCampo.Selecionar Then

            Cursor = Cursors.Cross

            If _criacaoEmAndamento Then

                _pontoPreviewTela =
                    New PointF(
                        LimitarSingle(
                            e.X,
                            campo.Left,
                            campo.Right),
                        LimitarSingle(
                            e.Y,
                            campo.Top,
                            campo.Bottom))

                Invalidate()

            End If

            Exit Sub

        End If

        If _arrastando AndAlso
           _objetoSelecionado IsNot Nothing Then

            If _movendoGrupo Then

                MoverGrupoSelecionado(e.Location, campo)

            ElseIf _modoManipulacao =
                   ModoManipulacaoCampo.MoverObjeto Then

                MoverObjetoSelecionado(e.Location, campo)

            Else

                RedimensionarObjetoSelecionado(e.Location, campo)

            End If

            _houveAlteracaoManipulacao = True

            Invalidate()
            Exit Sub

        End If

        Dim manipulador As ModoManipulacaoCampo =
            LocalizarManipuladorSelecionado(e.Location, campo)

        If manipulador <> ModoManipulacaoCampo.Nenhum Then

            Cursor = ObterCursorManipulador(manipulador)
            Exit Sub

        End If

        Dim objetoSobMouse As ObjetoCampo =
            LocalizarObjetoNaPosicao(e.Location, campo)

        If objetoSobMouse IsNot Nothing Then
            Cursor = Cursors.Hand
        Else
            Cursor = Cursors.Default
        End If

    End Sub

    Protected Overrides Sub OnMouseUp(
        e As MouseEventArgs)

        MyBase.OnMouseUp(e)

        If _panEmAndamento Then

            If e.Button = MouseButtons.Middle OrElse
               e.Button = MouseButtons.Left Then

                _panEmAndamento = False
                Capture = False

                If _espacoPressionado Then
                    Cursor = Cursors.Hand
                ElseIf FerramentaAtual = FerramentaCampo.Selecionar Then
                    Cursor = Cursors.Default
                Else
                    Cursor = Cursors.Cross
                End If

                Exit Sub

            End If

        End If

        If e.Button <> MouseButtons.Left Then
            Exit Sub
        End If

        Dim deveRegistrar As Boolean =
            _arrastando AndAlso _houveAlteracaoManipulacao

        _arrastando = False
        _modoManipulacao = ModoManipulacaoCampo.Nenhum

        Capture = False

        If FerramentaAtual = FerramentaCampo.Selecionar Then
            Cursor = Cursors.Default
        Else
            Cursor = Cursors.Cross
        End If

        If deveRegistrar Then
            RegistrarEstadoHistorico()
        End If

        _movendoGrupo = False
        _estadosMovimentoGrupo.Clear()

        _estadoAntesManipulacao = String.Empty
        _houveAlteracaoManipulacao = False

    End Sub

    Protected Overrides Sub OnMouseLeave(
        e As EventArgs)

        MyBase.OnMouseLeave(e)

        If Not _arrastando Then
            Cursor = Cursors.Default
        End If

    End Sub

    Private Sub MoverObjetoSelecionado(localMouse As Point, campo As RectangleF)

        If _objetoSelecionado Is Nothing Then
            Exit Sub
        End If

        If campo.Width <= 0 OrElse
       campo.Height <= 0 Then

            Exit Sub

        End If

        If TypeOf _objetoSelecionado Is LinhaTatica Then

            MoverLinhaSelecionada(DirectCast(_objetoSelecionado, LinhaTatica), localMouse, campo)

            Exit Sub

        End If

        If TypeOf _objetoSelecionado Is AreaTatica Then

            MoverAreaSelecionada(DirectCast(_objetoSelecionado, AreaTatica), localMouse, campo)

            Exit Sub

        End If

        Dim metadeTamanho As SizeF = ObterMetadeTamanhoVisual(_objetoSelecionado)

        Dim xTela As Single = localMouse.X - _offsetMouse.X

        Dim yTela As Single = localMouse.Y - _offsetMouse.Y

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

        Dim novaPosicao As New Posicao(
            xPercentual,
            yPercentual)

        novaPosicao =
            AjustarPosicaoNaGrade(
                novaPosicao)

        Dim pontoAjustado As PointF =
            ConverterPercentualParaTela(
                novaPosicao,
                campo)

        pontoAjustado.X =
            LimitarSingle(
                pontoAjustado.X,
                campo.Left +
                metadeTamanho.Width,
                campo.Right -
                metadeTamanho.Width)

        pontoAjustado.Y =
            LimitarSingle(
                pontoAjustado.Y,
                campo.Top +
                metadeTamanho.Height,
                campo.Bottom -
                metadeTamanho.Height)

        novaPosicao =
            ConverterTelaParaPercentual(
                pontoAjustado,
                campo)

        _objetoSelecionado.Posicao.X =
            novaPosicao.X

        _objetoSelecionado.Posicao.Y =
            novaPosicao.Y
    End Sub

    Private Sub MoverLinhaSelecionada(linha As LinhaTatica, localMouse As Point, campo As RectangleF)

        Dim deltaX As Single =
        localMouse.X -
        _mouseInicialLinha.X

        Dim deltaY As Single =
        localMouse.Y -
        _mouseInicialLinha.Y

        Dim menorXOriginal As Single =
        Math.Min(
            _inicioOriginalLinha.X,
            _fimOriginalLinha.X)

        Dim maiorXOriginal As Single =
        Math.Max(
            _inicioOriginalLinha.X,
            _fimOriginalLinha.X)

        Dim menorYOriginal As Single =
        Math.Min(
            _inicioOriginalLinha.Y,
            _fimOriginalLinha.Y)

        Dim maiorYOriginal As Single =
        Math.Max(
            _inicioOriginalLinha.Y,
            _fimOriginalLinha.Y)

        deltaX =
        LimitarSingle(
            deltaX,
            campo.Left - menorXOriginal,
            campo.Right - maiorXOriginal)

        deltaY =
        LimitarSingle(
            deltaY,
            campo.Top - menorYOriginal,
            campo.Bottom - maiorYOriginal)

        Dim novoInicio As New PointF(
        _inicioOriginalLinha.X + deltaX,
        _inicioOriginalLinha.Y + deltaY)

        Dim novoFim As New PointF(
        _fimOriginalLinha.X + deltaX,
        _fimOriginalLinha.Y + deltaY)

        Dim percentualInicio As Posicao =
        ConverterTelaParaPercentual(
            novoInicio,
            campo)

        Dim percentualFim As Posicao =
        ConverterTelaParaPercentual(
            novoFim,
            campo)

        AplicarEncaixeMovimentoDoisPontos(percentualInicio, percentualFim)

        linha.Posicao.X =
        percentualInicio.X

        linha.Posicao.Y =
        percentualInicio.Y

        linha.PosicaoFinal.X =
        percentualFim.X

        linha.PosicaoFinal.Y =
        percentualFim.Y

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

            If Not TypeOf objeto Is MarcadorTatico Then
                Continue For
            End If

            If EstaSobreObjeto(
            objeto,
            localMouse,
            campo) Then

                Return objeto

            End If

        Next

        For indice As Integer =
        _objetos.Count - 1 To 0 Step -1

            Dim objeto As ObjetoCampo =
            _objetos(indice)

            If Not objeto.Visivel Then
                Continue For
            End If

            If Not TypeOf objeto Is TextoTatico Then
                Continue For
            End If

            If EstaSobreObjeto(
            objeto,
            localMouse,
            campo) Then

                Return objeto

            End If

        Next

        For indice As Integer =
        _objetos.Count - 1 To 0 Step -1

            Dim objeto As ObjetoCampo =
            _objetos(indice)

            If Not objeto.Visivel Then
                Continue For
            End If

            If TypeOf objeto Is AreaTatica OrElse
           TypeOf objeto Is MarcadorTatico OrElse
           TypeOf objeto Is TextoTatico Then

                Continue For

            End If

            If EstaSobreObjeto(
            objeto,
            localMouse,
            campo) Then

                Return objeto

            End If

        Next

        For indice As Integer =
        _objetos.Count - 1 To 0 Step -1

            Dim objeto As ObjetoCampo =
            _objetos(indice)

            If Not objeto.Visivel Then
                Continue For
            End If

            If Not TypeOf objeto Is AreaTatica Then
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

        If TypeOf objeto Is TextoTatico Then

            Dim texto As TextoTatico =
        DirectCast(
            objeto,
            TextoTatico)

            Dim area As RectangleF =
        ObterRetanguloTextoTatico(
            centro,
            texto)

            area.Inflate(
        4.0F,
        4.0F)

            Return area.Contains(
        localMouse.X,
        localMouse.Y)

        End If

        If TypeOf objeto Is LinhaTatica Then

            Dim linha As LinhaTatica =
        DirectCast(objeto, LinhaTatica)

            Dim inicio As PointF =
        ConverterPercentualParaTela(
            linha.Posicao,
            campo)

            Dim fim As PointF =
        ConverterPercentualParaTela(
            linha.PosicaoFinal,
            campo)

            Dim pontoMouse As New PointF(
        localMouse.X,
        localMouse.Y)

            Dim tolerancia As Single =
        linha.Espessura + 7.0F

            Return DistanciaPontoSegmento(
        pontoMouse,
        inicio,
        fim) <= tolerancia

        End If

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

        If TypeOf objeto Is AreaTatica Then

            Dim area As AreaTatica =
        DirectCast(
            objeto,
            AreaTatica)

            Dim retangulo As RectangleF =
        ObterRetanguloArea(
            area,
            campo)

            Return retangulo.Contains(
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

    Private Sub MoverAreaSelecionada(area As AreaTatica, localMouse As Point, campo As RectangleF)

        Dim pontoInicial As PointF =
        ConverterPercentualParaTela(
            area.Posicao,
            campo)

        Dim pontoFinal As PointF =
        ConverterPercentualParaTela(
            area.PosicaoFinal,
            campo)

        Dim centroAtual As New PointF(
        (pontoInicial.X + pontoFinal.X) / 2.0F,
        (pontoInicial.Y + pontoFinal.Y) / 2.0F)

        Dim centroDesejado As New PointF(
        localMouse.X - _offsetMouse.X,
        localMouse.Y - _offsetMouse.Y)

        Dim deltaX As Single =
        centroDesejado.X -
        centroAtual.X

        Dim deltaY As Single =
        centroDesejado.Y -
        centroAtual.Y

        Dim menorX As Single =
        Math.Min(
            pontoInicial.X,
            pontoFinal.X)

        Dim maiorX As Single =
        Math.Max(
            pontoInicial.X,
            pontoFinal.X)

        Dim menorY As Single =
        Math.Min(
            pontoInicial.Y,
            pontoFinal.Y)

        Dim maiorY As Single =
        Math.Max(
            pontoInicial.Y,
            pontoFinal.Y)

        deltaX = LimitarSingle(
        deltaX,
        campo.Left - menorX,
        campo.Right - maiorX)

        deltaY = LimitarSingle(
        deltaY,
        campo.Top - menorY,
        campo.Bottom - maiorY)

        Dim novoInicial As New PointF(
        pontoInicial.X + deltaX,
        pontoInicial.Y + deltaY)

        Dim novoFinal As New PointF(
        pontoFinal.X + deltaX,
        pontoFinal.Y + deltaY)

        Dim percentualInicial As Posicao = ConverterTelaParaPercentual(novoInicial, campo)

        Dim percentualFinal As Posicao = ConverterTelaParaPercentual(novoFinal, campo)

        AplicarEncaixeMovimentoDoisPontos(percentualInicial, percentualFinal)

        area.Posicao.X =
        percentualInicial.X

        area.Posicao.Y =
        percentualInicial.Y

        area.PosicaoFinal.X =
        percentualFinal.X

        area.PosicaoFinal.Y =
        percentualFinal.Y

    End Sub

    Public Sub ExcluirSelecionado()

        If _objetosSelecionados.Count = 0 Then
            Exit Sub
        End If

        Dim objetosRemover As New List(Of ObjetoCampo)(_objetosSelecionados)

        For Each objeto As ObjetoCampo In objetosRemover
            _objetos.Remove(objeto)
        Next

        DeselecionarTodos()

        _arrastando = False
        _modoManipulacao = ModoManipulacaoCampo.Nenhum
        _movendoGrupo = False

        Capture = False

        NotificarSelecaoAlterada()

        RegistrarEstadoHistorico()

        Invalidate()

    End Sub

    Protected Overrides Sub OnKeyDown(
    e As KeyEventArgs)

        MyBase.OnKeyDown(e)

        If e.KeyCode = Keys.Delete Then

            ExcluirSelecionado()

            e.Handled = True
            e.SuppressKeyPress = True

            Exit Sub

        End If

        If e.KeyCode = Keys.Space Then

            _espacoPressionado =
            True

            If Not _panEmAndamento Then

                Cursor =
                Cursors.Hand

            End If

            e.Handled =
            True

        End If

        If e.KeyCode = Keys.Escape Then

            CancelarCriacao()

            FerramentaAtual =
            FerramentaCampo.Selecionar

            e.Handled = True
            e.SuppressKeyPress = True

        End If

    End Sub

    Protected Overrides Sub OnKeyUp(
    e As KeyEventArgs)

        MyBase.OnKeyUp(e)

        If e.KeyCode = Keys.Space Then

            _espacoPressionado =
            False

            If Not _panEmAndamento Then

                If FerramentaAtual =
               FerramentaCampo.Selecionar Then

                    Cursor =
                    Cursors.Default

                Else

                    Cursor =
                    Cursors.Cross

                End If

            End If

            e.Handled =
            True

        End If

    End Sub

    Private Function ObterCursorManipulador(
    modo As ModoManipulacaoCampo) As Cursor

        Select Case modo

            Case ModoManipulacaoCampo.LinhaInicio,
             ModoManipulacaoCampo.LinhaFim

                Return Cursors.Cross

            Case ModoManipulacaoCampo.AreaSuperiorEsquerda,
             ModoManipulacaoCampo.AreaInferiorDireita

                Return Cursors.SizeNWSE

            Case ModoManipulacaoCampo.AreaSuperiorDireita,
             ModoManipulacaoCampo.AreaInferiorEsquerda

                Return Cursors.SizeNESW

            Case Else

                Return Cursors.Default

        End Select

    End Function

    Private Function PontoDentroDoRaio(
    localMouse As Point,
    ponto As PointF,
    raio As Single) As Boolean

        Dim diferencaX As Single =
        localMouse.X - ponto.X

        Dim diferencaY As Single =
        localMouse.Y - ponto.Y

        Dim distanciaQuadrada As Single =
        diferencaX * diferencaX +
        diferencaY * diferencaY

        Return distanciaQuadrada <=
        raio * raio

    End Function

    Private Sub NormalizarAreaSelecionada(
    area As AreaTatica,
    campo As RectangleF)

        Dim retangulo As RectangleF =
        ObterRetanguloArea(
            area,
            campo)

        Dim superiorEsquerda As Posicao =
        ConverterTelaParaPercentual(
            New PointF(
                retangulo.Left,
                retangulo.Top),
            campo)

        Dim inferiorDireita As Posicao =
        ConverterTelaParaPercentual(
            New PointF(
                retangulo.Right,
                retangulo.Bottom),
            campo)

        area.Posicao.X =
        superiorEsquerda.X

        area.Posicao.Y =
        superiorEsquerda.Y

        area.PosicaoFinal.X =
        inferiorDireita.X

        area.PosicaoFinal.Y =
        inferiorDireita.Y

    End Sub

    Private Sub RedimensionarObjetoSelecionado(
    localMouse As Point,
    campo As RectangleF)

        If _objetoSelecionado Is Nothing Then
            Exit Sub
        End If

        Dim pontoTela As New PointF(
        LimitarSingle(
            localMouse.X,
            campo.Left,
            campo.Right),
        LimitarSingle(
            localMouse.Y,
            campo.Top,
            campo.Bottom))

        pontoTela =
    AjustarPontoTelaNaGrade(
        pontoTela,
        campo)

        If TypeOf _objetoSelecionado Is LinhaTatica Then

            RedimensionarLinhaSelecionada(
            DirectCast(
                _objetoSelecionado,
                LinhaTatica),
            pontoTela,
            campo)

            Exit Sub

        End If

        If TypeOf _objetoSelecionado Is AreaTatica Then

            RedimensionarAreaSelecionada(
            DirectCast(
                _objetoSelecionado,
                AreaTatica),
            pontoTela,
            campo)

        End If

    End Sub

    Private Sub RedimensionarLinhaSelecionada(
    linha As LinhaTatica,
    pontoTela As PointF,
    campo As RectangleF)

        Dim pontoOposto As PointF

        Select Case _modoManipulacao

            Case ModoManipulacaoCampo.LinhaInicio

                pontoOposto =
                _fimOriginalLinha

            Case ModoManipulacaoCampo.LinhaFim

                pontoOposto =
                _inicioOriginalLinha

            Case Else

                Exit Sub

        End Select

        Dim diferencaX As Single =
        pontoTela.X -
        pontoOposto.X

        Dim diferencaY As Single =
        pontoTela.Y -
        pontoOposto.Y

        Dim distanciaQuadrada As Single =
        diferencaX * diferencaX +
        diferencaY * diferencaY

        If distanciaQuadrada <
       ComprimentoMinimoLinha *
       ComprimentoMinimoLinha Then

            Exit Sub

        End If

        Dim percentual As Posicao =
        ConverterTelaParaPercentual(
            pontoTela,
            campo)

        Select Case _modoManipulacao

            Case ModoManipulacaoCampo.LinhaInicio

                linha.Posicao.X =
                percentual.X

                linha.Posicao.Y =
                percentual.Y

            Case ModoManipulacaoCampo.LinhaFim

                linha.PosicaoFinal.X =
                percentual.X

                linha.PosicaoFinal.Y =
                percentual.Y

        End Select

    End Sub

    Private Sub RedimensionarAreaSelecionada(
    area As AreaTatica,
    pontoTela As PointF,
    campo As RectangleF)

        Dim superiorEsquerda As PointF =
        ConverterPercentualParaTela(
            area.Posicao,
            campo)

        Dim inferiorDireita As PointF =
        ConverterPercentualParaTela(
            area.PosicaoFinal,
            campo)

        Dim esquerda As Single =
        superiorEsquerda.X

        Dim topo As Single =
        superiorEsquerda.Y

        Dim direita As Single =
        inferiorDireita.X

        Dim inferior As Single =
        inferiorDireita.Y

        Select Case _modoManipulacao

            Case ModoManipulacaoCampo.AreaSuperiorEsquerda

                esquerda =
                Math.Min(
                    pontoTela.X,
                    direita - TamanhoMinimoArea)

                topo =
                Math.Min(
                    pontoTela.Y,
                    inferior - TamanhoMinimoArea)

            Case ModoManipulacaoCampo.AreaSuperiorDireita

                direita =
                Math.Max(
                    pontoTela.X,
                    esquerda + TamanhoMinimoArea)

                topo =
                Math.Min(
                    pontoTela.Y,
                    inferior - TamanhoMinimoArea)

            Case ModoManipulacaoCampo.AreaInferiorEsquerda

                esquerda =
                Math.Min(
                    pontoTela.X,
                    direita - TamanhoMinimoArea)

                inferior =
                Math.Max(
                    pontoTela.Y,
                    topo + TamanhoMinimoArea)

            Case ModoManipulacaoCampo.AreaInferiorDireita

                direita =
                Math.Max(
                    pontoTela.X,
                    esquerda + TamanhoMinimoArea)

                inferior =
                Math.Max(
                    pontoTela.Y,
                    topo + TamanhoMinimoArea)

        End Select

        esquerda =
        LimitarSingle(
            esquerda,
            campo.Left,
            campo.Right)

        direita =
        LimitarSingle(
            direita,
            campo.Left,
            campo.Right)

        topo =
        LimitarSingle(
            topo,
            campo.Top,
            campo.Bottom)

        inferior =
        LimitarSingle(
            inferior,
            campo.Top,
            campo.Bottom)

        Dim percentualInicial As Posicao =
        ConverterTelaParaPercentual(
            New PointF(
                esquerda,
                topo),
            campo)

        Dim percentualFinal As Posicao =
        ConverterTelaParaPercentual(
            New PointF(
                direita,
                inferior),
            campo)

        area.Posicao.X =
        percentualInicial.X

        area.Posicao.Y =
        percentualInicial.Y

        area.PosicaoFinal.X =
        percentualFinal.X

        area.PosicaoFinal.Y =
        percentualFinal.Y

    End Sub

    Private Function LocalizarManipuladorSelecionado(
    localMouse As Point,
    campo As RectangleF) As ModoManipulacaoCampo

        If _objetosSelecionados.Count <> 1 Then
            Return ModoManipulacaoCampo.Nenhum
        End If

        If _objetoSelecionado Is Nothing Then
            Return ModoManipulacaoCampo.Nenhum
        End If

        If TypeOf _objetoSelecionado Is LinhaTatica Then

            Dim linha As LinhaTatica =
            DirectCast(
                _objetoSelecionado,
                LinhaTatica)

            Dim inicio As PointF =
            ConverterPercentualParaTela(
                linha.Posicao,
                campo)

            Dim fim As PointF =
            ConverterPercentualParaTela(
                linha.PosicaoFinal,
                campo)

            If PontoDentroDoRaio(
            localMouse,
            inicio,
            RaioManipulador) Then

                Return ModoManipulacaoCampo.LinhaInicio

            End If

            If PontoDentroDoRaio(
            localMouse,
            fim,
            RaioManipulador) Then

                Return ModoManipulacaoCampo.LinhaFim

            End If

        End If

        If TypeOf _objetoSelecionado Is AreaTatica Then

            Dim area As AreaTatica =
            DirectCast(
                _objetoSelecionado,
                AreaTatica)

            Dim retangulo As RectangleF =
            ObterRetanguloArea(
                area,
                campo)

            Dim superiorEsquerda As New PointF(
            retangulo.Left,
            retangulo.Top)

            Dim superiorDireita As New PointF(
            retangulo.Right,
            retangulo.Top)

            Dim inferiorEsquerda As New PointF(
            retangulo.Left,
            retangulo.Bottom)

            Dim inferiorDireita As New PointF(
            retangulo.Right,
            retangulo.Bottom)

            If PontoDentroDoRaio(
            localMouse,
            superiorEsquerda,
            RaioManipulador) Then

                Return ModoManipulacaoCampo.AreaSuperiorEsquerda

            End If

            If PontoDentroDoRaio(
            localMouse,
            superiorDireita,
            RaioManipulador) Then

                Return ModoManipulacaoCampo.AreaSuperiorDireita

            End If

            If PontoDentroDoRaio(
            localMouse,
            inferiorEsquerda,
            RaioManipulador) Then

                Return ModoManipulacaoCampo.AreaInferiorEsquerda

            End If

            If PontoDentroDoRaio(
            localMouse,
            inferiorDireita,
            RaioManipulador) Then

                Return ModoManipulacaoCampo.AreaInferiorDireita

            End If

        End If

        Return ModoManipulacaoCampo.Nenhum

    End Function

    Private Sub IniciarManipulacaoPorAlca(
    modo As ModoManipulacaoCampo,
    localMouse As Point,
    campo As RectangleF)

        If _objetoSelecionado Is Nothing Then
            Exit Sub
        End If

        If TypeOf _objetoSelecionado Is AreaTatica Then

            NormalizarAreaSelecionada(
            DirectCast(
                _objetoSelecionado,
                AreaTatica),
            campo)

        End If

        If TypeOf _objetoSelecionado Is LinhaTatica Then

            PrepararManipulacaoLinha(
            DirectCast(
                _objetoSelecionado,
                LinhaTatica),
            localMouse,
            campo)

        End If

        _houveAlteracaoManipulacao = False

        _modoManipulacao = modo
        _arrastando = True

        Capture = True

        Cursor =
    ObterCursorManipulador(modo)

    End Sub

    Private Sub PrepararManipulacaoLinha(
    linha As LinhaTatica,
    localMouse As Point,
    campo As RectangleF)

        _mouseInicialLinha =
        New PointF(
            localMouse.X,
            localMouse.Y)

        _inicioOriginalLinha =
        ConverterPercentualParaTela(
            linha.Posicao,
            campo)

        _fimOriginalLinha =
        ConverterPercentualParaTela(
            linha.PosicaoFinal,
            campo)

    End Sub

    Protected Overrides Sub OnMouseWheel(e As MouseEventArgs)

        MyBase.OnMouseWheel(e)

        Focus()

        Dim quantidadePassos As Single =
        e.Delta /
        120.0F

        Dim novoZoom As Single =
        _zoomVisual +
        PassoZoom *
        quantidadePassos

        DefinirZoom(
        novoZoom,
        e.Location)

    End Sub

    Protected Overrides Function IsInputKey(keyData As Keys) As Boolean

        Dim tecla As Keys =
        keyData And Keys.KeyCode

        If tecla = Keys.Space Then
            Return True
        End If

        Return MyBase.IsInputKey(
        keyData)

    End Function

    Protected Overrides Sub OnLostFocus(e As EventArgs)

        MyBase.OnLostFocus(e)

        _espacoPressionado =
        False

        If Not _panEmAndamento Then

            If FerramentaAtual =
           FerramentaCampo.Selecionar Then

                Cursor =
                Cursors.Default

            Else

                Cursor =
                Cursors.Cross

            End If

        End If

    End Sub

    Private Sub DeselecionarTodos()

        For Each objeto As ObjetoCampo In _objetos
            objeto.Selecionado = False
        Next

        _objetosSelecionados.Clear()
        _objetoSelecionado = Nothing

        _movendoGrupo = False
        _estadosMovimentoGrupo.Clear()

    End Sub

#End Region


#Region "Seleção múltipla"

    Private Function ObjetoEstaSelecionado(
        objeto As ObjetoCampo) As Boolean

        If objeto Is Nothing Then
            Return False
        End If

        Return _objetosSelecionados.Contains(objeto)

    End Function

    Private Sub SelecionarSomente(
        objeto As ObjetoCampo)

        DeselecionarTodos()

        If objeto Is Nothing Then
            Exit Sub
        End If

        objeto.Selecionado = True
        _objetosSelecionados.Add(objeto)
        _objetoSelecionado = objeto

    End Sub

    Private Sub AlternarSelecaoObjeto(
        objeto As ObjetoCampo)

        If objeto Is Nothing Then
            Exit Sub
        End If

        If _objetosSelecionados.Contains(objeto) Then

            objeto.Selecionado = False
            _objetosSelecionados.Remove(objeto)

            If _objetoSelecionado Is objeto Then

                If _objetosSelecionados.Count > 0 Then
                    _objetoSelecionado =
                        _objetosSelecionados(_objetosSelecionados.Count - 1)
                Else
                    _objetoSelecionado = Nothing
                End If

            End If

        Else

            objeto.Selecionado = True
            _objetosSelecionados.Add(objeto)
            _objetoSelecionado = objeto

        End If

    End Sub

    Private Sub NotificarSelecaoAlterada()

        RaiseEvent ObjetoSelecionadoAlterado(_objetoSelecionado)

        Invalidate()

    End Sub

    Public Sub SelecionarTodos()

        DeselecionarTodos()

        For Each objeto As ObjetoCampo In _objetos

            If Not objeto.Visivel Then
                Continue For
            End If

            objeto.Selecionado = True
            _objetosSelecionados.Add(objeto)

        Next

        If _objetosSelecionados.Count > 0 Then
            _objetoSelecionado =
                _objetosSelecionados(_objetosSelecionados.Count - 1)
        End If

        NotificarSelecaoAlterada()

    End Sub

    Private Sub PrepararMovimentoGrupo(
        localMouse As Point)

        _estadosMovimentoGrupo.Clear()

        _pontoInicialMovimentoGrupo =
            New PointF(localMouse.X, localMouse.Y)

        For Each objeto As ObjetoCampo In _objetosSelecionados

            Dim estado As New EstadoMovimentoGrupo With {
                .X = objeto.Posicao.X,
                .Y = objeto.Posicao.Y
            }

            If TypeOf objeto Is LinhaTatica Then

                Dim linha As LinhaTatica =
                    DirectCast(objeto, LinhaTatica)

                estado.XFinal = linha.PosicaoFinal.X
                estado.YFinal = linha.PosicaoFinal.Y
                estado.PossuiPontoFinal = True

            ElseIf TypeOf objeto Is AreaTatica Then

                Dim area As AreaTatica =
                    DirectCast(objeto, AreaTatica)

                estado.XFinal = area.PosicaoFinal.X
                estado.YFinal = area.PosicaoFinal.Y
                estado.PossuiPontoFinal = True

            End If

            _estadosMovimentoGrupo.Add(objeto, estado)

        Next

        _movendoGrupo = _estadosMovimentoGrupo.Count > 1

    End Sub

    Private Sub MoverGrupoSelecionado(
        localMouse As Point,
        campo As RectangleF)

        If _estadosMovimentoGrupo.Count = 0 Then
            Exit Sub
        End If

        If campo.Width <= 0 OrElse campo.Height <= 0 Then
            Exit Sub
        End If

        Dim deltaX As Double =
            (localMouse.X - _pontoInicialMovimentoGrupo.X) /
            campo.Width * 100.0

        Dim deltaY As Double =
            (localMouse.Y - _pontoInicialMovimentoGrupo.Y) /
            campo.Height * 100.0

        Dim menorX As Double = Double.MaxValue
        Dim maiorX As Double = Double.MinValue
        Dim menorY As Double = Double.MaxValue
        Dim maiorY As Double = Double.MinValue

        For Each item As KeyValuePair(Of ObjetoCampo, EstadoMovimentoGrupo) In _estadosMovimentoGrupo

            Dim estado As EstadoMovimentoGrupo = item.Value

            menorX = Math.Min(menorX, estado.X)
            maiorX = Math.Max(maiorX, estado.X)
            menorY = Math.Min(menorY, estado.Y)
            maiorY = Math.Max(maiorY, estado.Y)

            If estado.PossuiPontoFinal Then

                menorX = Math.Min(menorX, estado.XFinal)
                maiorX = Math.Max(maiorX, estado.XFinal)
                menorY = Math.Min(menorY, estado.YFinal)
                maiorY = Math.Max(maiorY, estado.YFinal)

            End If

        Next

        If _encaixeGradeAtivo AndAlso
           _objetoSelecionado IsNot Nothing Then

            Dim estadoPrincipal As EstadoMovimentoGrupo = Nothing

            If _estadosMovimentoGrupo.TryGetValue(
                _objetoSelecionado,
                estadoPrincipal) Then

                Dim destinoX As Double =
                    AjustarValorNaGrade(estadoPrincipal.X + deltaX)

                Dim destinoY As Double =
                    AjustarValorNaGrade(estadoPrincipal.Y + deltaY)

                deltaX = destinoX - estadoPrincipal.X
                deltaY = destinoY - estadoPrincipal.Y

            End If

        End If

        deltaX =
            Math.Max(
                -menorX,
                Math.Min(100.0 - maiorX, deltaX))

        deltaY =
            Math.Max(
                -menorY,
                Math.Min(100.0 - maiorY, deltaY))

        For Each item As KeyValuePair(Of ObjetoCampo, EstadoMovimentoGrupo) In _estadosMovimentoGrupo

            Dim objeto As ObjetoCampo = item.Key
            Dim estado As EstadoMovimentoGrupo = item.Value

            objeto.Posicao.X = estado.X + deltaX
            objeto.Posicao.Y = estado.Y + deltaY

            If TypeOf objeto Is LinhaTatica Then

                Dim linha As LinhaTatica =
                    DirectCast(objeto, LinhaTatica)

                linha.PosicaoFinal.X = estado.XFinal + deltaX
                linha.PosicaoFinal.Y = estado.YFinal + deltaY

            ElseIf TypeOf objeto Is AreaTatica Then

                Dim area As AreaTatica =
                    DirectCast(objeto, AreaTatica)

                area.PosicaoFinal.X = estado.XFinal + deltaX
                area.PosicaoFinal.Y = estado.YFinal + deltaY

            End If

        Next

    End Sub

#End Region


#Region "Alinhamento e distribuição"

    Private Function ObterCentroPercentualObjeto(
        objeto As ObjetoCampo) As PointF

        If objeto Is Nothing Then
            Return PointF.Empty
        End If

        If TypeOf objeto Is LinhaTatica Then

            Dim linha As LinhaTatica =
                DirectCast(objeto, LinhaTatica)

            Return New PointF(
                CSng((linha.Posicao.X + linha.PosicaoFinal.X) / 2.0),
                CSng((linha.Posicao.Y + linha.PosicaoFinal.Y) / 2.0))

        End If

        If TypeOf objeto Is AreaTatica Then

            Dim area As AreaTatica =
                DirectCast(objeto, AreaTatica)

            Return New PointF(
                CSng((area.Posicao.X + area.PosicaoFinal.X) / 2.0),
                CSng((area.Posicao.Y + area.PosicaoFinal.Y) / 2.0))

        End If

        Return New PointF(
            CSng(objeto.Posicao.X),
            CSng(objeto.Posicao.Y))

    End Function

    Private Function ObterLimitesPercentuaisObjeto(
        objeto As ObjetoCampo) As RectangleF

        Dim menorX As Double = objeto.Posicao.X
        Dim maiorX As Double = objeto.Posicao.X
        Dim menorY As Double = objeto.Posicao.Y
        Dim maiorY As Double = objeto.Posicao.Y

        If TypeOf objeto Is LinhaTatica Then

            Dim linha As LinhaTatica =
                DirectCast(objeto, LinhaTatica)

            menorX = Math.Min(menorX, linha.PosicaoFinal.X)
            maiorX = Math.Max(maiorX, linha.PosicaoFinal.X)
            menorY = Math.Min(menorY, linha.PosicaoFinal.Y)
            maiorY = Math.Max(maiorY, linha.PosicaoFinal.Y)

        ElseIf TypeOf objeto Is AreaTatica Then

            Dim area As AreaTatica =
                DirectCast(objeto, AreaTatica)

            menorX = Math.Min(menorX, area.PosicaoFinal.X)
            maiorX = Math.Max(maiorX, area.PosicaoFinal.X)
            menorY = Math.Min(menorY, area.PosicaoFinal.Y)
            maiorY = Math.Max(maiorY, area.PosicaoFinal.Y)

        End If

        Return RectangleF.FromLTRB(
            CSng(menorX),
            CSng(menorY),
            CSng(maiorX),
            CSng(maiorY))

    End Function

    Private Sub AplicarDeltaPercentualObjeto(
        objeto As ObjetoCampo,
        deltaX As Double,
        deltaY As Double)

        objeto.Posicao.X += deltaX
        objeto.Posicao.Y += deltaY

        If TypeOf objeto Is LinhaTatica Then

            Dim linha As LinhaTatica =
                DirectCast(objeto, LinhaTatica)

            linha.PosicaoFinal.X += deltaX
            linha.PosicaoFinal.Y += deltaY

        ElseIf TypeOf objeto Is AreaTatica Then

            Dim area As AreaTatica =
                DirectCast(objeto, AreaTatica)

            area.PosicaoFinal.X += deltaX
            area.PosicaoFinal.Y += deltaY

        End If

    End Sub

    Private Function LimitarDeltaPercentualObjeto(
        objeto As ObjetoCampo,
        deltaX As Double,
        deltaY As Double) As PointF

        Dim limites As RectangleF =
            ObterLimitesPercentuaisObjeto(objeto)

        deltaX =
            Math.Max(
                -limites.Left,
                Math.Min(100.0 - limites.Right, deltaX))

        deltaY =
            Math.Max(
                -limites.Top,
                Math.Min(100.0 - limites.Bottom, deltaY))

        Return New PointF(CSng(deltaX), CSng(deltaY))

    End Function

    Private Function MoverObjetoPercentualmente(
        objeto As ObjetoCampo,
        deltaX As Double,
        deltaY As Double) As Boolean

        If objeto Is Nothing Then
            Return False
        End If

        Dim deltaLimitado As PointF =
            LimitarDeltaPercentualObjeto(objeto, deltaX, deltaY)

        If Math.Abs(deltaLimitado.X) < 0.0001F AndAlso
           Math.Abs(deltaLimitado.Y) < 0.0001F Then

            Return False

        End If

        AplicarDeltaPercentualObjeto(
            objeto,
            deltaLimitado.X,
            deltaLimitado.Y)

        Return True

    End Function

    Private Function ObterLimitesPercentuaisSelecao() As RectangleF

        If _objetosSelecionados.Count = 0 Then
            Return RectangleF.Empty
        End If

        Dim menorX As Double = Double.MaxValue
        Dim maiorX As Double = Double.MinValue
        Dim menorY As Double = Double.MaxValue
        Dim maiorY As Double = Double.MinValue

        For Each objeto As ObjetoCampo In _objetosSelecionados

            Dim limites As RectangleF =
                ObterLimitesPercentuaisObjeto(objeto)

            menorX = Math.Min(menorX, limites.Left)
            maiorX = Math.Max(maiorX, limites.Right)
            menorY = Math.Min(menorY, limites.Top)
            maiorY = Math.Max(maiorY, limites.Bottom)

        Next

        Return RectangleF.FromLTRB(
            CSng(menorX),
            CSng(menorY),
            CSng(maiorX),
            CSng(maiorY))

    End Function

    Private Function MoverSelecaoPercentualmente(
        deltaX As Double,
        deltaY As Double) As Boolean

        If _objetosSelecionados.Count = 0 Then
            Return False
        End If

        Dim limites As RectangleF =
            ObterLimitesPercentuaisSelecao()

        deltaX =
            Math.Max(
                -limites.Left,
                Math.Min(100.0 - limites.Right, deltaX))

        deltaY =
            Math.Max(
                -limites.Top,
                Math.Min(100.0 - limites.Bottom, deltaY))

        If Math.Abs(deltaX) < 0.0001 AndAlso
           Math.Abs(deltaY) < 0.0001 Then

            Return False

        End If

        For Each objeto As ObjetoCampo In _objetosSelecionados
            AplicarDeltaPercentualObjeto(objeto, deltaX, deltaY)
        Next

        Return True

    End Function

    Private Sub FinalizarAlinhamentoSelecao(
        houveAlteracao As Boolean)

        If Not houveAlteracao Then
            Exit Sub
        End If

        RegistrarEstadoHistorico()
        NotificarSelecaoAlterada()
        Invalidate()

    End Sub

    Public Sub AlinharSelecaoNaMesmaLinha()

        If _objetosSelecionados.Count < 2 OrElse
           _objetoSelecionado Is Nothing Then

            Exit Sub

        End If

        Dim centroReferencia As PointF =
            ObterCentroPercentualObjeto(_objetoSelecionado)

        Dim houveAlteracao As Boolean = False

        For Each objeto As ObjetoCampo In _objetosSelecionados

            If objeto Is _objetoSelecionado Then
                Continue For
            End If

            Dim centroObjeto As PointF =
                ObterCentroPercentualObjeto(objeto)

            Dim deltaY As Double =
                centroReferencia.Y - centroObjeto.Y

            If MoverObjetoPercentualmente(objeto, 0.0, deltaY) Then
                houveAlteracao = True
            End If

        Next

        FinalizarAlinhamentoSelecao(houveAlteracao)

    End Sub

    Public Sub AlinharSelecaoNaMesmaColuna()

        If _objetosSelecionados.Count < 2 OrElse
           _objetoSelecionado Is Nothing Then

            Exit Sub

        End If

        Dim centroReferencia As PointF =
            ObterCentroPercentualObjeto(_objetoSelecionado)

        Dim houveAlteracao As Boolean = False

        For Each objeto As ObjetoCampo In _objetosSelecionados

            If objeto Is _objetoSelecionado Then
                Continue For
            End If

            Dim centroObjeto As PointF =
                ObterCentroPercentualObjeto(objeto)

            Dim deltaX As Double =
                centroReferencia.X - centroObjeto.X

            If MoverObjetoPercentualmente(objeto, deltaX, 0.0) Then
                houveAlteracao = True
            End If

        Next

        FinalizarAlinhamentoSelecao(houveAlteracao)

    End Sub

    Public Sub CentralizarSelecaoNoCampo()

        If _objetosSelecionados.Count = 0 Then
            Exit Sub
        End If

        Dim limites As RectangleF =
            ObterLimitesPercentuaisSelecao()

        Dim centroX As Double =
            limites.Left + limites.Width / 2.0

        Dim centroY As Double =
            limites.Top + limites.Height / 2.0

        Dim houveAlteracao As Boolean =
            MoverSelecaoPercentualmente(
                50.0 - centroX,
                50.0 - centroY)

        FinalizarAlinhamentoSelecao(houveAlteracao)

    End Sub

    Public Sub DistribuirSelecaoHorizontalmente()

        If _objetosSelecionados.Count < 3 Then
            Exit Sub
        End If

        Dim objetosOrdenados As New List(Of ObjetoCampo)(_objetosSelecionados)

        objetosOrdenados.Sort(
            Function(objetoA, objetoB)

                Dim centroA As PointF =
                    ObterCentroPercentualObjeto(objetoA)

                Dim centroB As PointF =
                    ObterCentroPercentualObjeto(objetoB)

                Return centroA.X.CompareTo(centroB.X)

            End Function)

        Dim primeiroCentro As PointF =
            ObterCentroPercentualObjeto(objetosOrdenados(0))

        Dim ultimoCentro As PointF =
            ObterCentroPercentualObjeto(
                objetosOrdenados(objetosOrdenados.Count - 1))

        Dim distanciaTotal As Double =
            ultimoCentro.X - primeiroCentro.X

        If Math.Abs(distanciaTotal) < 0.0001 Then
            Exit Sub
        End If

        Dim intervalo As Double =
            distanciaTotal / (objetosOrdenados.Count - 1)

        Dim houveAlteracao As Boolean = False

        For indice As Integer = 1 To objetosOrdenados.Count - 2

            Dim objeto As ObjetoCampo = objetosOrdenados(indice)

            Dim centroAtual As PointF =
                ObterCentroPercentualObjeto(objeto)

            Dim destinoX As Double =
                primeiroCentro.X + intervalo * indice

            If MoverObjetoPercentualmente(
                objeto,
                destinoX - centroAtual.X,
                0.0) Then

                houveAlteracao = True

            End If

        Next

        FinalizarAlinhamentoSelecao(houveAlteracao)

    End Sub

    Public Sub DistribuirSelecaoVerticalmente()

        If _objetosSelecionados.Count < 3 Then
            Exit Sub
        End If

        Dim objetosOrdenados As New List(Of ObjetoCampo)(_objetosSelecionados)

        objetosOrdenados.Sort(
            Function(objetoA, objetoB)

                Dim centroA As PointF =
                    ObterCentroPercentualObjeto(objetoA)

                Dim centroB As PointF =
                    ObterCentroPercentualObjeto(objetoB)

                Return centroA.Y.CompareTo(centroB.Y)

            End Function)

        Dim primeiroCentro As PointF =
            ObterCentroPercentualObjeto(objetosOrdenados(0))

        Dim ultimoCentro As PointF =
            ObterCentroPercentualObjeto(
                objetosOrdenados(objetosOrdenados.Count - 1))

        Dim distanciaTotal As Double =
            ultimoCentro.Y - primeiroCentro.Y

        If Math.Abs(distanciaTotal) < 0.0001 Then
            Exit Sub
        End If

        Dim intervalo As Double =
            distanciaTotal / (objetosOrdenados.Count - 1)

        Dim houveAlteracao As Boolean = False

        For indice As Integer = 1 To objetosOrdenados.Count - 2

            Dim objeto As ObjetoCampo = objetosOrdenados(indice)

            Dim centroAtual As PointF =
                ObterCentroPercentualObjeto(objeto)

            Dim destinoY As Double =
                primeiroCentro.Y + intervalo * indice

            If MoverObjetoPercentualmente(
                objeto,
                0.0,
                destinoY - centroAtual.Y) Then

                houveAlteracao = True

            End If

        Next

        FinalizarAlinhamentoSelecao(houveAlteracao)

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

        If TypeOf objeto Is MarcadorTatico Then

            Dim marcador As MarcadorTatico =
        DirectCast(
            objeto,
            MarcadorTatico)

            Dim raio As Single =
        marcador.Diametro / 2.0F

            Return New SizeF(
        raio,
        raio)

        End If

        If TypeOf objeto Is TextoTatico Then

            Dim texto As TextoTatico =
        DirectCast(
            objeto,
            TextoTatico)

            Dim tamanho As SizeF =
        ObterTamanhoTextoTatico(texto)

            Return New SizeF(
        tamanho.Width / 2.0F,
        tamanho.Height / 2.0F)

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

        If TypeOf objeto Is MarcadorTatico Then

            Dim marcador As MarcadorTatico =
        DirectCast(
            objeto,
            MarcadorTatico)

            Return marcador.Diametro / 2.0F + 5.0F

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

    Private Function ObterRetanguloArea(
    area As AreaTatica,
    campo As RectangleF) As RectangleF

        Dim pontoInicial As PointF =
        ConverterPercentualParaTela(
            area.Posicao,
            campo)

        Dim pontoFinal As PointF =
        ConverterPercentualParaTela(
            area.PosicaoFinal,
            campo)

        Dim esquerda As Single =
        Math.Min(
            pontoInicial.X,
            pontoFinal.X)

        Dim topo As Single =
        Math.Min(
            pontoInicial.Y,
            pontoFinal.Y)

        Dim largura As Single =
        Math.Abs(
            pontoFinal.X -
            pontoInicial.X)

        Dim altura As Single =
        Math.Abs(
            pontoFinal.Y -
            pontoInicial.Y)

        Return New RectangleF(
        esquerda,
        topo,
        largura,
        altura)

    End Function

    Protected Overrides Sub OnResize(e As EventArgs)

        MyBase.OnResize(e)

        LimitarDeslocamentoVisual()

        Invalidate()

    End Sub

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

    Private Function DistanciaPontoSegmento(
    ponto As PointF,
    inicio As PointF,
    fim As PointF) As Single

        Dim deltaX As Single =
        fim.X - inicio.X

        Dim deltaY As Single =
        fim.Y - inicio.Y

        Dim comprimentoQuadrado As Single =
        deltaX * deltaX +
        deltaY * deltaY

        If comprimentoQuadrado <= 0.0001F Then

            Dim distanciaX As Single =
            ponto.X - inicio.X

            Dim distanciaY As Single =
            ponto.Y - inicio.Y

            Return CSng(
            Math.Sqrt(
                distanciaX * distanciaX +
                distanciaY * distanciaY))

        End If

        Dim percentual As Single =
        ((ponto.X - inicio.X) * deltaX +
        (ponto.Y - inicio.Y) * deltaY) /
        comprimentoQuadrado

        percentual = LimitarSingle(
        percentual,
        0.0F,
        1.0F)

        Dim projecaoX As Single =
        inicio.X +
        percentual * deltaX

        Dim projecaoY As Single =
        inicio.Y +
        percentual * deltaY

        Dim diferencaX As Single =
        ponto.X - projecaoX

        Dim diferencaY As Single =
        ponto.Y - projecaoY

        Return CSng(
        Math.Sqrt(
            diferencaX * diferencaX +
            diferencaY * diferencaY))

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

    Private Function ObterCentroObjetoTela(
    objeto As ObjetoCampo,
    campo As RectangleF) As PointF

        If TypeOf objeto Is AreaTatica Then

            Dim area As AreaTatica =
        DirectCast(
            objeto,
            AreaTatica)

            Dim pontoInicial As PointF =
        ConverterPercentualParaTela(
            area.Posicao,
            campo)

            Dim pontoFinal As PointF =
        ConverterPercentualParaTela(
            area.PosicaoFinal,
            campo)

            Return New PointF(
        (pontoInicial.X + pontoFinal.X) / 2.0F,
        (pontoInicial.Y + pontoFinal.Y) / 2.0F)

        End If

        Return ConverterPercentualParaTela(
            objeto.Posicao,
            campo)

    End Function

    Private Function ConverterTelaParaPercentual(
    ponto As PointF,
    campo As RectangleF) As Posicao

        Dim xPercentual As Double =
            ((ponto.X - campo.Left) /
            campo.Width) * 100.0

        Dim yPercentual As Double =
            ((ponto.Y - campo.Top) /
            campo.Height) * 100.0

        Return New Posicao(
            LimitarPercentual(xPercentual),
            LimitarPercentual(yPercentual))

    End Function

#End Region

#Region "Zoom e navegação"

    Public Sub AumentarZoom()

        Dim centro As New Point(
            ClientSize.Width \ 2,
            ClientSize.Height \ 2)

        DefinirZoom(
            _zoomVisual + PassoZoom,
            centro)

    End Sub

    Public Sub DiminuirZoom()

        Dim centro As New Point(
            ClientSize.Width \ 2,
            ClientSize.Height \ 2)

        DefinirZoom(
            _zoomVisual - PassoZoom,
            centro)

    End Sub

    Public Sub RestaurarVisualizacao()

        _zoomVisual =
            1.0F

        _deslocamentoVisual =
            PointF.Empty

        RaiseEvent VisualizacaoAlterada(
            ZoomPercentual)

        Invalidate()

    End Sub

    Private Sub DefinirZoom(
    novoZoom As Single,
    pontoFocoTela As Point)

        novoZoom =
            LimitarSingle(
                novoZoom,
                ZoomMinimo,
                ZoomMaximo)

        If Math.Abs(
            novoZoom -
            _zoomVisual) < 0.001F Then

            Exit Sub

        End If

        Dim pontoMundo As PointF =
            ConverterTelaParaMundo(
                pontoFocoTela)

        _zoomVisual =
            novoZoom

        Dim campoBase As RectangleF =
            ObterRetanguloCampo()

        Dim centroX As Single =
            campoBase.Left +
            campoBase.Width / 2.0F

        Dim centroY As Single =
            campoBase.Top +
            campoBase.Height / 2.0F

        _deslocamentoVisual.X =
            pontoFocoTela.X -
            centroX -
            _zoomVisual *
            (pontoMundo.X - centroX)

        _deslocamentoVisual.Y =
            pontoFocoTela.Y -
            centroY -
            _zoomVisual *
            (pontoMundo.Y - centroY)

        LimitarDeslocamentoVisual()

        RaiseEvent VisualizacaoAlterada(
            ZoomPercentual)

        Invalidate()

    End Sub

    Private Function ObterMatrizVisualizacao() As Matrix

        Dim campoBase As RectangleF =
            ObterRetanguloCampo()

        Dim centroX As Single =
            campoBase.Left +
            campoBase.Width / 2.0F

        Dim centroY As Single =
            campoBase.Top +
            campoBase.Height / 2.0F

        Dim deslocamentoX As Single =
            centroX +
            _deslocamentoVisual.X -
            _zoomVisual * centroX

        Dim deslocamentoY As Single =
            centroY +
            _deslocamentoVisual.Y -
            _zoomVisual * centroY

        Return New Matrix(
            _zoomVisual,
            0.0F,
            0.0F,
            _zoomVisual,
            deslocamentoX,
            deslocamentoY)

    End Function

    Private Function ConverterTelaParaMundo(
    pontoTela As Point) As PointF

        Dim pontos() As PointF = {
            New PointF(
                pontoTela.X,
                pontoTela.Y)
        }

        Using matriz As Matrix =
            ObterMatrizVisualizacao()

            matriz.Invert()

            matriz.TransformPoints(
                pontos)

        End Using

        Return pontos(0)

    End Function

    Private Function CriarEventoMouseMundo(
    evento As MouseEventArgs) As MouseEventArgs

        Dim pontoMundo As PointF =
            ConverterTelaParaMundo(
                evento.Location)

        Return New MouseEventArgs(
            evento.Button,
            evento.Clicks,
            CInt(
                Math.Round(
                    pontoMundo.X)),
            CInt(
                Math.Round(
                    pontoMundo.Y)),
            evento.Delta)

    End Function

    Private Sub LimitarDeslocamentoVisual()

        If _zoomVisual <= 1.0F Then

            _deslocamentoVisual =
                PointF.Empty

            Exit Sub

        End If

        Dim campoBase As RectangleF =
            ObterRetanguloCampo()

        Dim centroX As Single =
            campoBase.Left +
            campoBase.Width / 2.0F

        Dim centroY As Single =
            campoBase.Top +
            campoBase.Height / 2.0F

        Dim limitePositivoX As Single =
            (_zoomVisual - 1.0F) *
            (centroX - campoBase.Left)

        Dim limiteNegativoX As Single =
            (_zoomVisual - 1.0F) *
            (campoBase.Right - centroX)

        Dim limitePositivoY As Single =
            (_zoomVisual - 1.0F) *
            (centroY - campoBase.Top)

        Dim limiteNegativoY As Single =
            (_zoomVisual - 1.0F) *
            (campoBase.Bottom - centroY)

        _deslocamentoVisual.X =
            LimitarSingle(
                _deslocamentoVisual.X,
                -limiteNegativoX,
                limitePositivoX)

        _deslocamentoVisual.Y =
            LimitarSingle(
                _deslocamentoVisual.Y,
                -limiteNegativoY,
                limitePositivoY)

    End Sub

#End Region

#Region "Grade e encaixe"

    Private Function AjustarValorNaGrade(
    valor As Double) As Double

        valor =
            LimitarPercentual(
                valor)

        If Not _encaixeGradeAtivo Then
            Return valor
        End If

        Dim passo As Double =
            _espacamentoGradePercentual

        Dim valorAjustado As Double =
            Math.Round(
                valor / passo,
                MidpointRounding.AwayFromZero) *
            passo

        Return LimitarPercentual(
            valorAjustado)

    End Function

    Private Function AjustarPosicaoNaGrade(
        posicao As Posicao) As Posicao

        Return New Posicao(
            AjustarValorNaGrade(
                posicao.X),
            AjustarValorNaGrade(
                posicao.Y))

    End Function

    Private Function AjustarPontoTelaNaGrade(
        pontoTela As PointF,
        campo As RectangleF) As PointF

        If Not _encaixeGradeAtivo Then

            Return pontoTela

        End If

        Dim percentual As Posicao =
            ConverterTelaParaPercentual(
                pontoTela,
                campo)

        percentual =
            AjustarPosicaoNaGrade(
                percentual)

        Return ConverterPercentualParaTela(
            percentual,
            campo)

    End Function

    Private Sub AplicarEncaixeMovimentoDoisPontos(
    pontoInicial As Posicao,
    pontoFinal As Posicao)

        If Not _encaixeGradeAtivo Then
            Exit Sub
        End If

        Dim destinoX As Double =
            AjustarValorNaGrade(
                pontoInicial.X)

        Dim destinoY As Double =
            AjustarValorNaGrade(
                pontoInicial.Y)

        Dim deltaX As Double =
            destinoX -
            pontoInicial.X

        Dim deltaY As Double =
            destinoY -
            pontoInicial.Y

        Dim menorX As Double =
            Math.Min(
                pontoInicial.X,
                pontoFinal.X)

        Dim maiorX As Double =
            Math.Max(
                pontoInicial.X,
                pontoFinal.X)

        Dim menorY As Double =
            Math.Min(
                pontoInicial.Y,
                pontoFinal.Y)

        Dim maiorY As Double =
            Math.Max(
                pontoInicial.Y,
                pontoFinal.Y)

        deltaX =
            Math.Max(
                -menorX,
                Math.Min(
                    100.0 - maiorX,
                    deltaX))

        deltaY =
            Math.Max(
                -menorY,
                Math.Min(
                    100.0 - maiorY,
                    deltaY))

        pontoInicial.X +=
            deltaX

        pontoInicial.Y +=
            deltaY

        pontoFinal.X +=
            deltaX

        pontoFinal.Y +=
            deltaY

    End Sub

#End Region

End Class
