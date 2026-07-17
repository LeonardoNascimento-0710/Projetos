Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Text.Json
Imports System.Windows.Forms
Imports TacticalStudio.Core
Imports TacticalStudio.Core.Classes
Imports TacticalStudio.Core.Enums

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

    Private _tipoObjetoLinha As TipoObjetoLinha =
        TipoObjetoLinha.Cone

    Private _quantidadeObjetosLinha As Integer = 7

    Private _orientacaoLinhaObjetos As OrientacaoLinhaObjetos =
        OrientacaoLinhaObjetos.Livre

    Private _escalaObjetosLinha As Single = 1.0F

    Private _agruparObjetosLinha As Boolean = True

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

    Private _estiloVisualCampo As EstiloVisualCampo =
        EstiloVisualCampo.Estadio

    Private _intensidadeTexturaCampo As Integer =
        35

    Private _faixasGramaVisiveis As Boolean =
        True

    Private _sombrasCampoAtivas As Boolean =
        True


    Private ReadOnly _objetosSelecionados As New List(Of ObjetoCampo)()

    Private ReadOnly _estadosMovimentoGrupo As New Dictionary(Of ObjetoCampo, EstadoMovimentoGrupo)()

    Private _pontoInicialMovimentoGrupo As PointF

    Private _movendoGrupo As Boolean

    Private _recorteAtivo As Boolean = False

    Private _retanguloRecortePercentual As RectangleF = RectangleF.Empty

    Private _modoSelecaoRecorte As Boolean = False

    Private _desenhandoRecorte As Boolean = False

    Private _pontoInicialRecorte As PointF

    Private _pontoAtualRecorte As PointF

    Private _recorteAnteriorAtivo As Boolean = False

    Private _retanguloRecorteAnterior As RectangleF = RectangleF.Empty

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

    Public Event RecorteCampoAlterado()

    Public Event ObjetosAlterados()

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
    Public ReadOnly Property EscalaVisualReferenciaSelecao As Single

        Get

            If _objetosSelecionados.Count = 0 Then
                Return 1.0F
            End If

            Dim objetoReferencia As ObjetoCampo =
                _objetosSelecionados(
                    _objetosSelecionados.Count - 1)

            Return NormalizarEscalaVisual(
                objetoReferencia.EscalaVisual)

        End Get

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)>
    Public ReadOnly Property SelecaoPossuiGrupo As Boolean

        Get

            For Each objeto As ObjetoCampo
            In _objetosSelecionados

                If Not String.IsNullOrWhiteSpace(
                objeto.GrupoId) Then

                    Return True

                End If

            Next

            Return False

        End Get

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)>
    Public ReadOnly Property SelecaoPossuiObjetoBloqueado As Boolean

        Get
            Return ExisteObjetoBloqueadoNaSelecao()
        End Get

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)>
    Public ReadOnly Property ObjetosAtuais As IReadOnlyList(Of ObjetoCampo)

        Get
            Return New List(Of ObjetoCampo)(_objetos).AsReadOnly()
        End Get

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)>
    Public ReadOnly Property ObjetosSelecionadosAtuais As IReadOnlyList(Of ObjetoCampo)

        Get
            Return New List(Of ObjetoCampo)(_objetosSelecionados).AsReadOnly()
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

            Dim manterRecorteAtivo As Boolean =
                _recorteAtivo

            Dim manterRetanguloRecorte As RectangleF =
                _retanguloRecortePercentual

            _ferramentaAtual = value

            CancelarCriacao()

            'Trocar de ferramenta nunca remove o recorte concluído.
            _recorteAtivo =
                manterRecorteAtivo

            _retanguloRecortePercentual =
                manterRetanguloRecorte

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

    <Browsable(False)>
    <DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)>
    Public ReadOnly Property EstiloCampoAtual As EstiloVisualCampo

        Get
            Return _estiloVisualCampo
        End Get

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)>
    Public ReadOnly Property IntensidadeTexturaCampo As Integer

        Get
            Return _intensidadeTexturaCampo
        End Get

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)>
    Public ReadOnly Property FaixasGramaAtivas As Boolean

        Get
            Return _faixasGramaVisiveis
        End Get

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)>
    Public ReadOnly Property SombrasCampoAtivas As Boolean

        Get
            Return _sombrasCampoAtivas
        End Get

    End Property

    Public Function AplicarConfiguracaoVisualCampo(
        estilo As EstiloVisualCampo,
        intensidadeTextura As Integer,
        exibirFaixas As Boolean,
        exibirSombras As Boolean
    ) As Boolean

        Dim estiloSeguro As EstiloVisualCampo =
            estilo

        If Not System.Enum.IsDefined(
            GetType(EstiloVisualCampo),
            estiloSeguro) Then

            estiloSeguro =
                EstiloVisualCampo.Estadio

        End If

        Dim intensidadeSegura As Integer =
            Math.Max(
                0,
                Math.Min(
                    100,
                    intensidadeTextura))

        If _estiloVisualCampo = estiloSeguro AndAlso
           _intensidadeTexturaCampo = intensidadeSegura AndAlso
           _faixasGramaVisiveis = exibirFaixas AndAlso
           _sombrasCampoAtivas = exibirSombras Then

            Return False

        End If

        _estiloVisualCampo =
            estiloSeguro

        _intensidadeTexturaCampo =
            intensidadeSegura

        _faixasGramaVisiveis =
            exibirFaixas

        _sombrasCampoAtivas =
            exibirSombras

        Invalidate()

        RegistrarEstadoHistorico()

        RaiseEvent ObjetosAlterados()

        Return True

    End Function

    <Browsable(False)>
    <DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)>
    Public ReadOnly Property RecorteAtivo As Boolean

        Get
            Return _recorteAtivo
        End Get

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)>
    Public ReadOnly Property RetanguloRecortePercentual As RectangleF

        Get
            Return _retanguloRecortePercentual
        End Get

    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)>
    Public ReadOnly Property ModoSelecaoRecorteAtivo As Boolean

        Get
            Return _modoSelecaoRecorte
        End Get

    End Property

#End Region

#Region "Adicionar objetos"

    Public Sub ConfigurarLinhaObjetos(
        tipo As TipoObjetoLinha,
        quantidade As Integer,
        orientacao As OrientacaoLinhaObjetos,
        escalaVisual As Single,
        agruparAutomaticamente As Boolean)

        _tipoObjetoLinha = tipo

        _quantidadeObjetosLinha =
            Math.Max(
                2,
                Math.Min(
                    30,
                    quantidade))

        _orientacaoLinhaObjetos = orientacao

        _escalaObjetosLinha =
            NormalizarEscalaVisual(
                escalaVisual)

        _agruparObjetosLinha =
            agruparAutomaticamente

        CancelarCriacao()

    End Sub

    Public Function AdicionarJogador(
        numero As Integer,
        nome As String,
        xPercentual As Double,
        yPercentual As Double) As Jogador

        Dim jogador As New Jogador With {
    .Numero = numero,
    .Nome = nome,
    .Direcao = DirecaoJogador.Cima,
    .OrientacaoVisual = OrientacaoVisualJogador.Costas,
    .Pose = PoseJogador.Parado,
    .EscalaVisual = 1.0F
}

        jogador.Posicao.X = LimitarPercentual(xPercentual)
        jogador.Posicao.Y = LimitarPercentual(yPercentual)

        _objetos.Add(jogador)

        Invalidate()

        Return jogador

    End Function

    Public Function AplicarFormacao(
        formacao As ModeloFormacao,
        Optional substituirJogadoresExistentes As Boolean = True
    ) As IReadOnlyList(Of Jogador)

        Return AplicarFormacoesDefensivas(
            formacao,
            Nothing,
            substituirJogadoresExistentes)

    End Function

    Public Function AplicarFormacoesDefensivas(
        formacaoMeuTime As ModeloFormacao,
        formacaoAdversario As ModeloFormacao,
        Optional substituirJogadoresExistentes As Boolean = True
    ) As IReadOnlyList(Of Jogador)

        Dim jogadoresCriados As New List(Of Jogador)()

        Dim possuiMeuTime As Boolean =
            FormacaoValida(
                formacaoMeuTime)

        Dim possuiAdversario As Boolean =
            FormacaoValida(
                formacaoAdversario)

        If Not possuiMeuTime AndAlso
           Not possuiAdversario Then

            Return jogadoresCriados.AsReadOnly()

        End If

        FerramentaAtual =
            FerramentaCampo.Selecionar

        DeselecionarTodos()

        If substituirJogadoresExistentes Then

            For indice As Integer =
                _objetos.Count - 1 To 0 Step -1

                If TypeOf _objetos(indice) Is Jogador Then

                    _objetos.RemoveAt(
                        indice)

                End If

            Next

        End If

        Dim objetosSelecionar As New List(Of ObjetoCampo)()

        If possuiMeuTime Then

            AdicionarJogadoresDaFormacao(
                formacaoMeuTime,
                LadoDefesaFormacao.Esquerda,
                jogadoresCriados,
                objetosSelecionar)

        End If

        If possuiAdversario Then

            AdicionarJogadoresDaFormacao(
                formacaoAdversario,
                LadoDefesaFormacao.Direita,
                jogadoresCriados,
                objetosSelecionar)

        End If

        If jogadoresCriados.Count = 0 Then

            Return jogadoresCriados.AsReadOnly()

        End If

        SelecionarObjetosPelaLista(
            objetosSelecionar)

        RegistrarEstadoHistorico()

        RaiseEvent ObjetosAlterados()

        Invalidate()

        Return jogadoresCriados.AsReadOnly()

    End Function

    Private Shared Function FormacaoValida(
        formacao As ModeloFormacao
    ) As Boolean

        Return formacao IsNot Nothing AndAlso
               formacao.Posicoes IsNot Nothing AndAlso
               formacao.Posicoes.Count > 0

    End Function

    Private Sub AdicionarJogadoresDaFormacao(
        formacao As ModeloFormacao,
        ladoDefesa As LadoDefesaFormacao,
        jogadoresCriados As List(Of Jogador),
        objetosSelecionar As List(Of ObjetoCampo))

        If Not FormacaoValida(
            formacao) Then

            Exit Sub

        End If

        For Each posicao As PosicaoFormacao
            In formacao.Posicoes

            If posicao Is Nothing Then
                Continue For
            End If

            Dim nomeJogador As String =
                If(
                    String.IsNullOrWhiteSpace(
                        posicao.Nome),
                    "Jogador",
                    posicao.Nome.Trim())

            Dim xCampo As Single
            Dim yCampo As Single
            Dim orientacaoVisual As OrientacaoVisualJogador

            If ladoDefesa =
               LadoDefesaFormacao.Direita Then

                'O adversário é rotacionado em 180 graus:
                'defende o gol direito e ataca para a esquerda.
                xCampo =
                    posicao.YPercentual

                yCampo =
                    100.0F -
                    posicao.XPercentual

                orientacaoVisual =
                    OrientacaoVisualJogador.LadoEsquerdo

            Else

                'Meu time defende o gol esquerdo
                'e ataca para a direita.
                xCampo =
                    100.0F -
                    posicao.YPercentual

                yCampo =
                    posicao.XPercentual

                orientacaoVisual =
                    OrientacaoVisualJogador.Lado

            End If

            Dim jogador As Jogador =
                AdicionarJogador(
                    posicao.Numero,
                    nomeJogador,
                    xCampo,
                    yCampo)

            jogador.CorCamisaArgb =
                ObterCorCamisaFormacao(
                    posicao.CorCamisaArgb,
                    ladoDefesa)

            jogador.OrientacaoVisual =
                orientacaoVisual

            jogadoresCriados.Add(
                jogador)

            objetosSelecionar.Add(
                jogador)

        Next

    End Sub

    Private Shared Function ObterCorCamisaFormacao(
        corOriginalArgb As Integer,
        ladoDefesa As LadoDefesaFormacao
    ) As Integer

        Dim corPadrao As Color =
            Color.FromArgb(
                185,
                35,
                35)

        Dim corOriginal As Color

        Try

            corOriginal =
                Color.FromArgb(
                    corOriginalArgb)

        Catch

            corOriginal =
                corPadrao

        End Try

        If corOriginal.A = 0 Then

            corOriginal =
                corPadrao

        End If

        If ladoDefesa =
           LadoDefesaFormacao.Direita AndAlso
           corOriginal.R = corPadrao.R AndAlso
           corOriginal.G = corPadrao.G AndAlso
           corOriginal.B = corPadrao.B Then

            Return Color.FromArgb(
                40,
                105,
                190).ToArgb()

        End If

        Return Color.FromArgb(
            255,
            corOriginal.R,
            corOriginal.G,
            corOriginal.B).ToArgb()

    End Function

    Public Function CriarModeloFormacaoAtual(nome As String, descricao As String, Optional usarSomenteSelecionados As Boolean = False) As ModeloFormacao

        Dim nomeNormalizado As String =
        If(
            nome,
            String.Empty).
        Trim()

        If String.IsNullOrWhiteSpace(
        nomeNormalizado) Then

            nomeNormalizado =
            "Formação personalizada"

        End If

        If nomeNormalizado.Length > 80 Then

            nomeNormalizado =
            nomeNormalizado.Substring(
                0,
                80)

        End If

        Dim descricaoNormalizada As String =
        If(
            descricao,
            String.Empty).
        Trim()

        If descricaoNormalizada.Length > 250 Then

            descricaoNormalizada =
            descricaoNormalizada.Substring(
                0,
                250)

        End If

        Dim modelo As New ModeloFormacao With {
        .Id =
            "personalizada-" &
            System.Guid.NewGuid().
                ToString("N"),
        .Nome =
            nomeNormalizado,
        .Descricao =
            descricaoNormalizada
    }

        Dim objetosFonte As IEnumerable(Of ObjetoCampo)

        If usarSomenteSelecionados Then

            objetosFonte =
            _objetosSelecionados

        Else

            objetosFonte =
            _objetos

        End If

        For Each objeto As ObjetoCampo
        In objetosFonte

            If Not TypeOf objeto Is Jogador Then
                Continue For
            End If

            Dim jogador As Jogador =
            DirectCast(
                objeto,
                Jogador)

            Dim nomePosicao As String =
            If(
                jogador.Nome,
                String.Empty).
            Trim()

            If String.IsNullOrWhiteSpace(
            nomePosicao) Then

                nomePosicao =
                "Jogador " &
                jogador.Numero.ToString()

            End If

            If nomePosicao.Length > 80 Then

                nomePosicao =
                nomePosicao.Substring(
                    0,
                    80)

            End If

            modelo.Posicoes.Add(
    New PosicaoFormacao With {
        .Numero =
            jogador.Numero,
        .Nome =
            nomePosicao,
        .XPercentual =
            CSng(
                jogador.Posicao.Y),
        .YPercentual =
            100.0F -
            CSng(jogador.Posicao.X),
        .OrientacaoVisual =
            jogador.OrientacaoVisual,
        .CorCamisaArgb =
            jogador.CorCamisaArgb
    })

        Next

        If modelo.Posicoes.Count = 0 Then
            Return Nothing
        End If

        Return modelo

    End Function
    Public Function AdicionarBola(xPercentual As Double, yPercentual As Double) As Bola

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
             FerramentaCampo.Area,
             FerramentaCampo.LinhaObjetos

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

        If FerramentaAtual =
           FerramentaCampo.LinhaObjetos Then

            Dim pontoFinal As Posicao =
                AjustarFimLinhaObjetos(
                    _pontoInicialCriacao,
                    percentual)

            Dim objetosCriados As List(Of ObjetoCampo) =
                AdicionarLinhaDeObjetos(
                    _tipoObjetoLinha,
                    _quantidadeObjetosLinha,
                    _pontoInicialCriacao,
                    pontoFinal,
                    _escalaObjetosLinha,
                    _agruparObjetosLinha)

            CancelarCriacao()

            If objetosCriados.Count > 0 Then

                SelecionarObjetosCriados(
                    objetosCriados)

            End If

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

    Private Function AjustarFimLinhaObjetos(
        inicio As Posicao,
        fim As Posicao) As Posicao

        If inicio Is Nothing OrElse
           fim Is Nothing Then

            Return New Posicao()

        End If

        Dim resultado As New Posicao(
            fim.X,
            fim.Y)

        Select Case _orientacaoLinhaObjetos

            Case OrientacaoLinhaObjetos.Horizontal

                resultado.Y =
                    inicio.Y

            Case OrientacaoLinhaObjetos.Vertical

                resultado.X =
                    inicio.X

        End Select

        resultado.X =
            LimitarPercentual(
                resultado.X)

        resultado.Y =
            LimitarPercentual(
                resultado.Y)

        Return resultado

    End Function

    Private Function AdicionarLinhaDeObjetos(
        tipo As TipoObjetoLinha,
        quantidade As Integer,
        inicio As Posicao,
        fim As Posicao,
        escalaVisual As Single,
        agruparAutomaticamente As Boolean
    ) As List(Of ObjetoCampo)

        Dim objetosCriados As New List(Of ObjetoCampo)()

        If inicio Is Nothing OrElse
           fim Is Nothing Then

            Return objetosCriados

        End If

        Dim distanciaX As Double =
            fim.X - inicio.X

        Dim distanciaY As Double =
            fim.Y - inicio.Y

        Dim comprimentoPercentual As Double =
            Math.Sqrt(
                distanciaX * distanciaX +
                distanciaY * distanciaY)

        If comprimentoPercentual < 0.25 Then
            Return objetosCriados
        End If

        quantidade =
            Math.Max(
                2,
                Math.Min(
                    30,
                    quantidade))

        Dim escala As Single =
            NormalizarEscalaVisual(
                escalaVisual)

        Dim grupoId As String =
            String.Empty

        If agruparAutomaticamente Then

            grupoId =
                Guid.NewGuid().
                    ToString("N")

        End If

        For indice As Integer = 0 To quantidade - 1

            Dim percentual As Double =
                If(
                    quantidade <= 1,
                    0.0,
                    CDbl(indice) /
                    CDbl(quantidade - 1))

            Dim x As Double =
                inicio.X +
                (fim.X - inicio.X) *
                percentual

            Dim y As Double =
                inicio.Y +
                (fim.Y - inicio.Y) *
                percentual

            Dim objeto As ObjetoCampo =
                CriarObjetoParaLinha(
                    tipo,
                    indice + 1)

            If objeto Is Nothing Then
                Continue For
            End If

            objeto.Posicao.X =
                LimitarPercentual(x)

            objeto.Posicao.Y =
                LimitarPercentual(y)

            objeto.EscalaVisual =
                escala

            objeto.GrupoId =
                grupoId

            _objetos.Add(
                objeto)

            objetosCriados.Add(
                objeto)

        Next

        Return objetosCriados

    End Function

    Private Function CriarObjetoParaLinha(
        tipo As TipoObjetoLinha,
        numero As Integer) As ObjetoCampo

        Select Case tipo

            Case TipoObjetoLinha.Cone

                Return New Cone With {
                    .Cor = CorCone.Laranja
                }

            Case TipoObjetoLinha.Manequim

                Return New Manequim With {
                    .Cor = CorManequim.Amarelo
                }

            Case TipoObjetoLinha.Bola

                Return New Bola()

            Case TipoObjetoLinha.Marcador

                Return New MarcadorTatico With {
                    .Texto = numero.ToString(),
                    .Cor = CorMarcadorTatico.Branco,
                    .Diametro = 36.0F
                }

            Case TipoObjetoLinha.Jogador

                Return New Jogador With {
                    .Numero =
                        Math.Max(
                            1,
                            Math.Min(
                                99,
                                numero)),
                    .Nome =
                        "Jogador " &
                        numero.ToString(),
                    .Direcao = DirecaoJogador.Cima,
                    .OrientacaoVisual =
                        OrientacaoVisualJogador.Costas,
                    .Pose = PoseJogador.Parado
                }

            Case Else

                Return Nothing

        End Select

    End Function

    Private Sub SelecionarObjetosCriados(
        objetosCriados As IList(Of ObjetoCampo))

        If objetosCriados Is Nothing OrElse
           objetosCriados.Count = 0 Then

            Exit Sub

        End If

        Dim manterRecorteAtivo As Boolean =
            _recorteAtivo

        Dim manterRetanguloRecorte As RectangleF =
            _retanguloRecortePercentual

        DeselecionarTodos()

        For Each objeto As ObjetoCampo
            In objetosCriados

            If objeto Is Nothing Then
                Continue For
            End If

            objeto.Selecionado =
                True

            _objetosSelecionados.Add(
                objeto)

            _objetoSelecionado =
                objeto

        Next

        RaiseEvent ObjetoCriado(
            _objetoSelecionado)

        NotificarSelecaoAlterada()

        RegistrarEstadoHistorico()

        _recorteAtivo =
            manterRecorteAtivo

        _retanguloRecortePercentual =
            manterRetanguloRecorte

        Invalidate()

    End Sub

    Private Sub SelecionarObjetoCriado(
        objeto As ObjetoCampo)

        Dim manterRecorteAtivo As Boolean =
            _recorteAtivo

        Dim manterRetanguloRecorte As RectangleF =
            _retanguloRecortePercentual

        SelecionarSomente(objeto)

        RaiseEvent ObjetoCriado(objeto)

        NotificarSelecaoAlterada()

        RegistrarEstadoHistorico()

        'Criar ou selecionar objetos não altera o recorte atual.
        _recorteAtivo =
            manterRecorteAtivo

        _retanguloRecortePercentual =
            manterRetanguloRecorte

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

        RaiseEvent ObjetoSelecionadoAlterado(Nothing)
        RaiseEvent ObjetosAlterados()

        Invalidate()

    End Sub

#End Region

#Region "Pintura"

    Public Function GerarMiniaturaCampo(
    Optional larguraMiniatura As Integer = 640
) As Bitmap

        If larguraMiniatura < 240 Then

            larguraMiniatura =
            240

        End If

        If larguraMiniatura > 1600 Then

            larguraMiniatura =
            1600

        End If

        'GerarImagemCampo exige no mínimo 800 pixels.
        'Depois reduzimos para o tamanho da miniatura.
        Using imagemOriginal As Bitmap =
        GerarImagemCampo(
            800)

            If imagemOriginal Is Nothing OrElse
           imagemOriginal.Width <= 0 OrElse
           imagemOriginal.Height <= 0 Then

                Throw New InvalidOperationException(
                "Não foi possível gerar a imagem do campo.")

            End If

            Dim proporcao As Double =
            imagemOriginal.Height /
            CDbl(
                imagemOriginal.Width)

            Dim alturaMiniatura As Integer =
            Math.Max(
                1,
                CInt(
                    Math.Round(
                        larguraMiniatura *
                        proporcao)))

            Dim miniatura As New Bitmap(
            larguraMiniatura,
            alturaMiniatura,
            System.Drawing.Imaging.PixelFormat.Format32bppArgb)

            Try

                Using grafico As Graphics =
                Graphics.FromImage(
                    miniatura)

                    grafico.Clear(
                    Color.FromArgb(
                        28,
                        28,
                        28))

                    grafico.SmoothingMode =
                    SmoothingMode.AntiAlias

                    grafico.PixelOffsetMode =
                    PixelOffsetMode.HighQuality

                    grafico.CompositingQuality =
                    CompositingQuality.HighQuality

                    grafico.InterpolationMode =
                    InterpolationMode.HighQualityBicubic

                    grafico.DrawImage(
                    imagemOriginal,
                    New Rectangle(
                        0,
                        0,
                        miniatura.Width,
                        miniatura.Height),
                    0,
                    0,
                    imagemOriginal.Width,
                    imagemOriginal.Height,
                    GraphicsUnit.Pixel)

                End Using

            Catch

                miniatura.Dispose()

                Throw

            End Try

            Return miniatura

        End Using

    End Function

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

        Dim origemExportacao As RectangleF =
            ObterRetanguloOrigemRecorte(
                campoAtual)

        If origemExportacao.Width <= 1.0F OrElse
           origemExportacao.Height <= 1.0F Then

            origemExportacao =
                campoAtual

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
                origemExportacao.Width)

        Dim alturaCampo As Integer =
            CInt(
                Math.Ceiling(
                    origemExportacao.Height *
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

        Dim estadosSelecao As New Dictionary(
            Of ObjetoCampo,
            Boolean)()

        For Each objeto As ObjetoCampo In _objetos

            estadosSelecao(objeto) =
                objeto.Selecionado

            objeto.Selecionado =
                False

        Next

        Try

            _objetoSelecionado =
                Nothing

            Using g As Graphics =
                Graphics.FromImage(
                    imagem)

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

                Dim destinoCampo As New RectangleF(
                    margemImagem,
                    alturaCabecalho +
                    margemImagem,
                    larguraUtil,
                    alturaCampo)

                Dim estadoGrafico As GraphicsState =
                    g.Save()

                Try

                    g.SetClip(
                        destinoCampo)

                    Using matriz As Matrix =
                        CriarMatrizMapeamento(
                            origemExportacao,
                            destinoCampo)

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

                Finally

                    g.Restore(
                        estadoGrafico)

                End Try

            End Using

        Catch

            imagem.Dispose()

            Throw

        Finally

            _objetoSelecionado =
                objetoSelecionadoAnterior

            For Each item As KeyValuePair(
                Of ObjetoCampo,
                Boolean) In estadosSelecao

                item.Key.Selecionado =
                    item.Value

            Next

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

    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        MyBase.OnPaint(e)

        e.Graphics.Clear(
            BackColor)

        e.Graphics.SmoothingMode =
            SmoothingMode.AntiAlias

        e.Graphics.PixelOffsetMode =
            PixelOffsetMode.HighQuality

        Dim campoTela As RectangleF =
            ObterRetanguloCampo()

        If campoTela.Width <= 0 OrElse
           campoTela.Height <= 0 Then

            Exit Sub

        End If

        Dim estadoGrafico As GraphicsState =
            e.Graphics.Save()

        Try

            e.Graphics.SetClip(
                campoTela)

            Using matriz As Matrix =
                ObterMatrizVisualizacao()

                e.Graphics.Transform =
                    matriz

                DesenharCampo(
                    e.Graphics)

                DesenharPreVisualizacaoRecorte(
                    e.Graphics,
                    campoTela)

            End Using

        Finally

            e.Graphics.Restore(
                estadoGrafico)

        End Try

    End Sub

    Private Sub DesenharCampo(g As Graphics)

        Dim campo As RectangleF = ObterRetanguloCampo()

        If campo.Width <= 0 OrElse campo.Height <= 0 Then
            Exit Sub
        End If

        DesenharGramado(g, campo)

        DesenharGrade(g, campo)

        DesenharMarcacoes(g, campo)

        DesenharObjetos(g, campo)

        DesenharIndicadoresBloqueio(g, campo)

        DesenharPreVisualizacao(g, campo)

    End Sub

    Private Sub DesenharGramado(
        g As Graphics,
        campo As RectangleF)

        Dim corBase As Color
        Dim corFaixa As Color
        Dim quantidadeFaixas As Integer
        Dim opacidadeFaixa As Integer
        Dim intensidadeLuz As Integer
        Dim intensidadeSombra As Integer

        Select Case _estiloVisualCampo

            Case EstiloVisualCampo.Classico

                corBase =
                    Color.FromArgb(
                        63,
                        130,
                        58)

                corFaixa =
                    Color.White

                quantidadeFaixas =
                    10

                opacidadeFaixa =
                    20

                intensidadeLuz =
                    0

                intensidadeSombra =
                    26

            Case EstiloVisualCampo.Treino

                corBase =
                    Color.FromArgb(
                        76,
                        146,
                        66)

                corFaixa =
                    Color.FromArgb(
                        214,
                        239,
                        205)

                quantidadeFaixas =
                    8

                opacidadeFaixa =
                    16

                intensidadeLuz =
                    12

                intensidadeSombra =
                    22

            Case EstiloVisualCampo.Noturno

                corBase =
                    Color.FromArgb(
                        31,
                        88,
                        48)

                corFaixa =
                    Color.FromArgb(
                        148,
                        205,
                        156)

                quantidadeFaixas =
                    12

                opacidadeFaixa =
                    13

                intensidadeLuz =
                    35

                intensidadeSombra =
                    68

            Case Else

                corBase =
                    Color.FromArgb(
                        48,
                        119,
                        52)

                corFaixa =
                    Color.FromArgb(
                        207,
                        236,
                        200)

                quantidadeFaixas =
                    12

                opacidadeFaixa =
                    23

                intensidadeLuz =
                    18

                intensidadeSombra =
                    42

        End Select

        Using fundoCampo As New SolidBrush(
            corBase)

            g.FillRectangle(
                fundoCampo,
                campo)

        End Using

        If _faixasGramaVisiveis AndAlso
           quantidadeFaixas > 0 Then

            Dim larguraFaixa As Single =
                campo.Width /
                quantidadeFaixas

            For indice As Integer =
                0 To quantidadeFaixas - 1

                If indice Mod 2 = 0 Then

                    Dim faixa As New RectangleF(
                        campo.Left +
                        indice *
                        larguraFaixa,
                        campo.Top,
                        larguraFaixa +
                        0.5F,
                        campo.Height)

                    Using pincelFaixa As New SolidBrush(
                        Color.FromArgb(
                            opacidadeFaixa,
                            corFaixa))

                        g.FillRectangle(
                            pincelFaixa,
                            faixa)

                    End Using

                End If

            Next

        End If

        If _sombrasCampoAtivas AndAlso
           intensidadeLuz > 0 Then

            DesenharIluminacaoGramado(
                g,
                campo,
                intensidadeLuz)

        End If

        If _estiloVisualCampo =
           EstiloVisualCampo.Estadio OrElse
           _estiloVisualCampo =
           EstiloVisualCampo.Treino Then

            DesenharDesgasteAreas(
                g,
                campo)

        End If

        If _sombrasCampoAtivas Then

            DesenharVinhetaGramado(
                g,
                campo,
                intensidadeSombra)

        End If

        'A textura fica por último para não ser apagada pela
        'iluminação, pelo desgaste ou pela vinheta.
        DesenharTexturaGramado(
            g,
            campo)

    End Sub

    Private Sub DesenharTexturaGramado(
        g As Graphics,
        campo As RectangleF)

        Dim intensidade As Integer =
            Math.Max(
                0,
                Math.Min(
                    100,
                    _intensidadeTexturaCampo))

        If intensidade <= 0 OrElse
           campo.Width <= 0.0F OrElse
           campo.Height <= 0.0F Then

            Exit Sub

        End If

        Dim fator As Single =
            intensidade /
            100.0F

        Dim menorDimensao As Single =
            Math.Min(
                campo.Width,
                campo.Height)

        'A porcentagem agora altera densidade, opacidade,
        'espessura e comprimento dos detalhes.
        Dim quantidadeFios As Integer =
            35 +
            CInt(
                Math.Round(
                    intensidade *
                    5.5F))

        Dim quantidadePontos As Integer =
            15 +
            CInt(
                Math.Round(
                    intensidade *
                    2.8F))

        Dim alphaClaro As Integer =
            Math.Max(
                3,
                Math.Min(
                    42,
                    3 +
                    CInt(
                        Math.Round(
                            intensidade *
                            0.39F))))

        Dim alphaEscuro As Integer =
            Math.Max(
                2,
                Math.Min(
                    34,
                    2 +
                    CInt(
                        Math.Round(
                            intensidade *
                            0.31F))))

        Dim alphaPonto As Integer =
            Math.Max(
                2,
                Math.Min(
                    24,
                    2 +
                    CInt(
                        Math.Round(
                            intensidade *
                            0.22F))))

        Dim comprimento As Single =
            Math.Max(
                1.0F,
                menorDimensao *
                (0.003F +
                 fator *
                 0.006F))

        Dim espessura As Single =
            0.55F +
            fator *
            1.05F

        Dim tamanhoPonto As Single =
            Math.Max(
                0.8F,
                menorDimensao *
                (0.0018F +
                 fator *
                 0.0024F))

        Dim estadoGrafico As GraphicsState =
            g.Save()

        Try

            'Mantém o recorte externo da visualização ao aplicar zoom.
            'Usar SetClip sem CombineMode substituía o recorte anterior
            'e permitia que a textura fosse desenhada fora do campo.
            g.SetClip(
                campo,
                CombineMode.Intersect)

            g.SmoothingMode =
                SmoothingMode.AntiAlias

            Using canetaClara As New Pen(
                Color.FromArgb(
                    alphaClaro,
                    232,
                    255,
                    225),
                espessura),
                  canetaEscura As New Pen(
                Color.FromArgb(
                    alphaEscuro,
                    8,
                    54,
                    18),
                espessura),
                  pincelPontoClaro As New SolidBrush(
                Color.FromArgb(
                    alphaPonto,
                    225,
                    250,
                    218)),
                  pincelPontoEscuro As New SolidBrush(
                Color.FromArgb(
                    Math.Max(
                        2,
                        alphaPonto - 3),
                    12,
                    67,
                    22))

                For indice As Integer =
                    0 To quantidadeFios - 1

                    Dim percentualX As Single =
                        CSng(
                            ((indice * 73 + 19) Mod 997) /
                            997.0)

                    Dim percentualY As Single =
                        CSng(
                            ((indice * 151 + 47) Mod 991) /
                            991.0)

                    Dim inicioX As Single =
                        campo.Left +
                        percentualX *
                        campo.Width

                    Dim inicioY As Single =
                        campo.Top +
                        percentualY *
                        campo.Height

                    Dim inclinacao As Single =
                        CSng(
                            (((indice * 29) Mod 11) - 5) *
                            0.075F)

                    Dim variacaoComprimento As Single =
                        0.7F +
                        CSng(
                            (indice * 37 Mod 31) /
                            50.0F)

                    Dim caneta As Pen =
                        If(
                            indice Mod 3 = 0,
                            canetaEscura,
                            canetaClara)

                    g.DrawLine(
                        caneta,
                        inicioX,
                        inicioY,
                        inicioX +
                        inclinacao *
                        comprimento,
                        inicioY -
                        comprimento *
                        variacaoComprimento)

                Next

                For indice As Integer =
                    0 To quantidadePontos - 1

                    Dim percentualX As Single =
                        CSng(
                            ((indice * 193 + 61) Mod 983) /
                            983.0)

                    Dim percentualY As Single =
                        CSng(
                            ((indice * 109 + 31) Mod 977) /
                            977.0)

                    Dim centroX As Single =
                        campo.Left +
                        percentualX *
                        campo.Width

                    Dim centroY As Single =
                        campo.Top +
                        percentualY *
                        campo.Height

                    Dim variacao As Single =
                        0.65F +
                        CSng(
                            (indice * 17 Mod 29) /
                            45.0F)

                    Dim diametro As Single =
                        tamanhoPonto *
                        variacao

                    Dim pincel As Brush =
                        If(
                            indice Mod 4 = 0,
                            pincelPontoEscuro,
                            pincelPontoClaro)

                    g.FillEllipse(
                        pincel,
                        centroX -
                        diametro /
                        2.0F,
                        centroY -
                        diametro /
                        2.0F,
                        diametro,
                        diametro)

                Next

            End Using

        Finally

            g.Restore(
                estadoGrafico)

        End Try

    End Sub

    Private Sub DesenharIluminacaoGramado(
        g As Graphics,
        campo As RectangleF,
        intensidade As Integer)

        Dim elipse As New RectangleF(
            campo.Left +
            campo.Width *
            0.08F,
            campo.Top +
            campo.Height *
            0.08F,
            campo.Width *
            0.84F,
            campo.Height *
            0.84F)

        Using caminho As New GraphicsPath()

            caminho.AddEllipse(
                elipse)

            Using pincel As New PathGradientBrush(
                caminho)

                pincel.CenterColor =
                    Color.FromArgb(
                        Math.Max(
                            0,
                            Math.Min(
                                80,
                                intensidade)),
                        Color.White)

                pincel.SurroundColors =
                    New Color() {
                        Color.FromArgb(
                            0,
                            Color.White)
                    }

                g.FillEllipse(
                    pincel,
                    elipse)

            End Using

        End Using

    End Sub

    Private Sub DesenharDesgasteAreas(
        g As Graphics,
        campo As RectangleF)

        If _intensidadeTexturaCampo < 15 Then
            Exit Sub
        End If

        Dim alpha As Integer =
            Math.Max(
                2,
                Math.Min(
                    14,
                    _intensidadeTexturaCampo \ 9))

        Dim largura As Single =
            campo.Width *
            0.095F

        Dim altura As Single =
            campo.Height *
            0.34F

        Dim topo As Single =
            campo.Top +
            (campo.Height -
             altura) /
            2.0F

        Using pincel As New SolidBrush(
            Color.FromArgb(
                alpha,
                214,
                196,
                133))

            g.FillEllipse(
                pincel,
                campo.Left +
                campo.Width *
                0.015F,
                topo,
                largura,
                altura)

            g.FillEllipse(
                pincel,
                campo.Right -
                largura -
                campo.Width *
                0.015F,
                topo,
                largura,
                altura)

        End Using

    End Sub

    Private Sub DesenharVinhetaGramado(
        g As Graphics,
        campo As RectangleF,
        intensidade As Integer)

        Dim alpha As Integer =
            Math.Max(
                0,
                Math.Min(
                    100,
                    intensidade))

        If alpha = 0 Then
            Exit Sub
        End If

        Dim larguraBorda As Single =
            Math.Max(
                8.0F,
                Math.Min(
                    campo.Width,
                    campo.Height) *
                0.12F)

        Dim areaEsquerda As New RectangleF(
            campo.Left,
            campo.Top,
            larguraBorda,
            campo.Height)

        Dim areaDireita As New RectangleF(
            campo.Right -
            larguraBorda,
            campo.Top,
            larguraBorda,
            campo.Height)

        Dim areaSuperior As New RectangleF(
            campo.Left,
            campo.Top,
            campo.Width,
            larguraBorda)

        Dim areaInferior As New RectangleF(
            campo.Left,
            campo.Bottom -
            larguraBorda,
            campo.Width,
            larguraBorda)

        Using pincelEsquerdo As New LinearGradientBrush(
            areaEsquerda,
            Color.FromArgb(
                alpha,
                Color.Black),
            Color.FromArgb(
                0,
                Color.Black),
            LinearGradientMode.Horizontal),
              pincelDireito As New LinearGradientBrush(
            areaDireita,
            Color.FromArgb(
                0,
                Color.Black),
            Color.FromArgb(
                alpha,
                Color.Black),
            LinearGradientMode.Horizontal),
              pincelSuperior As New LinearGradientBrush(
            areaSuperior,
            Color.FromArgb(
                alpha,
                Color.Black),
            Color.FromArgb(
                0,
                Color.Black),
            LinearGradientMode.Vertical),
              pincelInferior As New LinearGradientBrush(
            areaInferior,
            Color.FromArgb(
                0,
                Color.Black),
            Color.FromArgb(
                alpha,
                Color.Black),
            LinearGradientMode.Vertical)

            g.FillRectangle(
                pincelEsquerdo,
                areaEsquerda)

            g.FillRectangle(
                pincelDireito,
                areaDireita)

            g.FillRectangle(
                pincelSuperior,
                areaSuperior)

            g.FillRectangle(
                pincelInferior,
                areaInferior)

        End Using

    End Sub

    Private Function ObterCorMarcacoesCampo() As Color

        Select Case _estiloVisualCampo

            Case EstiloVisualCampo.Noturno

                Return Color.FromArgb(
                    245,
                    250,
                    247)

            Case EstiloVisualCampo.Treino

                Return Color.FromArgb(
                    242,
                    245,
                    236)

            Case Else

                Return Color.White

        End Select

    End Function

    Private Function ObterEspessuraMarcacoesCampo() As Single

        Select Case _estiloVisualCampo

            Case EstiloVisualCampo.Estadio,
                 EstiloVisualCampo.Noturno

                Return 2.8F

            Case Else

                Return 2.5F

        End Select

    End Function

    Private Sub DesenharMarcacoes(
        g As Graphics,
        campo As RectangleF)

        Using caneta As New Pen(
            ObterCorMarcacoesCampo(),
            ObterEspessuraMarcacoesCampo())

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

    Private Function AjustarFimPreviewLinhaObjetos(
        inicio As PointF,
        fim As PointF) As PointF

        Select Case _orientacaoLinhaObjetos

            Case OrientacaoLinhaObjetos.Horizontal

                Return New PointF(
                    fim.X,
                    inicio.Y)

            Case OrientacaoLinhaObjetos.Vertical

                Return New PointF(
                    inicio.X,
                    fim.Y)

            Case Else

                Return fim

        End Select

    End Function

    Private Sub DesenharPreVisualizacao(g As Graphics, campo As RectangleF)

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

                Case FerramentaCampo.LinhaObjetos

                    Dim fimAjustado As PointF =
                        AjustarFimPreviewLinhaObjetos(
                            inicio,
                            fim)

                    g.DrawLine(
                        caneta,
                        inicio,
                        fimAjustado)

                    Dim quantidadePreview As Integer =
                        Math.Max(
                            2,
                            _quantidadeObjetosLinha)

                    Using pincelObjeto As New SolidBrush(
                        Color.FromArgb(
                            170,
                            Color.Gold))

                        For indice As Integer = 0 To quantidadePreview - 1

                            Dim percentualObjeto As Single =
                                CSng(indice) /
                                CSng(quantidadePreview - 1)

                            Dim ponto As PointF =
                                InterpolarPonto(
                                    inicio,
                                    fimAjustado,
                                    percentualObjeto)

                            Dim raioPreview As Single =
                                5.0F *
                                NormalizarEscalaVisual(
                                    _escalaObjetosLinha)

                            g.FillEllipse(
                                pincelObjeto,
                                ponto.X - raioPreview,
                                ponto.Y - raioPreview,
                                raioPreview * 2.0F,
                                raioPreview * 2.0F)

                        Next

                    End Using

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

    Private Sub DesenharIndicadoresBloqueio(
    g As Graphics,
    campo As RectangleF)

        Using fonte As New Font(
        "Segoe UI",
        8.0F,
        FontStyle.Bold)

            For Each objeto As ObjetoCampo In _objetos

                If Not objeto.Visivel OrElse
               Not objeto.Bloqueado Then

                    Continue For

                End If

                Dim centroPercentual As PointF =
                ObterCentroPercentualObjeto(
                    objeto)

                Dim posicaoCentro As New Posicao(
                centroPercentual.X,
                centroPercentual.Y)

                Dim centroTela As PointF =
                ConverterPercentualParaTela(
                    posicaoCentro,
                    campo)

                Dim retangulo As New RectangleF(
                centroTela.X + 10.0F,
                centroTela.Y - 25.0F,
                18.0F,
                18.0F)

                Using pincel As New SolidBrush(
                Color.FromArgb(
                    235,
                    135,
                    35))

                    g.FillEllipse(
                    pincel,
                    retangulo)

                End Using

                Using borda As New Pen(
                Color.White,
                1.5F)

                    g.DrawEllipse(
                    borda,
                    retangulo)

                End Using

                TextRenderer.DrawText(
                g,
                "L",
                fonte,
                Rectangle.Round(
                    retangulo),
                Color.White,
                TextFormatFlags.HorizontalCenter Or
                TextFormatFlags.VerticalCenter Or
                TextFormatFlags.NoPadding)

            Next

        End Using

    End Sub

    Private Sub AvisarSelecaoBloqueada()

        MessageBox.Show(
        "A seleção contém um ou mais objetos bloqueados." &
        Environment.NewLine &
        Environment.NewLine &
        "Desbloqueie usando Ctrl+L antes de mover, excluir, " &
        "redimensionar, alinhar ou distribuir os objetos.",
        "Objetos bloqueados",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information)

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

        Dim escala As Single =
        NormalizarEscalaVisual(
            jogador.EscalaVisual)

        '==================================================
        ' SOMBRA NO GRAMADO
        '==================================================

        Using sombra As New SolidBrush(
        Color.FromArgb(
            75,
            0,
            0,
            0))

            g.FillEllipse(
            sombra,
            centro.X -
            13.0F *
            escala,
            centro.Y +
            12.0F *
            escala,
            26.0F *
            escala,
            10.0F *
            escala)

        End Using

        Dim estadoGrafico As GraphicsState =
        g.Save()

        Try

            g.TranslateTransform(
            centro.X,
            centro.Y)

            'A orientação visual do jogador não gira o ícone.
            'Costas, frente e lado são desenhados sempre em pé.
            g.ScaleTransform(
            escala,
            escala)

            If Not TentarDesenharSpritePngJogador(
                g,
                jogador) Then

                DesenharSpriteVetorialJogador(
                    g,
                    jogador)

            End If

        Finally

            g.Restore(
            estadoGrafico)

        End Try

        '==================================================
        ' SELEÇÃO
        '==================================================

        If jogador.Selecionado Then

            DesenharSelecao(
            g,
            centro,
            28.0F *
            escala)

        End If

    End Sub

    Private Function TentarDesenharSpritePngJogador(
        g As Graphics,
        jogador As Jogador) As Boolean

        If g Is Nothing OrElse
           jogador Is Nothing Then

            Return False

        End If

        Dim imagemBase As Image = Nothing
        Dim imagemCamisa As Image = Nothing

        If Not GerenciadorSprites.TentarObterJogador(
            jogador.OrientacaoVisual,
            jogador.Pose,
            imagemBase,
            imagemCamisa) Then

            Return False

        End If

        If imagemBase Is Nothing OrElse
           imagemCamisa Is Nothing Then

            Return False

        End If

        Dim retanguloDestino As New RectangleF(
            -32.0F,
            -32.0F,
            64.0F,
            64.0F)

        Dim modoInterpolacaoAnterior As InterpolationMode =
            g.InterpolationMode

        Dim modoComposicaoAnterior As CompositingQuality =
            g.CompositingQuality

        Dim deslocamentoAnterior As PixelOffsetMode =
            g.PixelOffsetMode

        Try

            g.InterpolationMode =
                InterpolationMode.HighQualityBicubic

            g.CompositingQuality =
                CompositingQuality.HighQuality

            g.PixelOffsetMode =
                PixelOffsetMode.HighQuality

            g.DrawImage(
                imagemBase,
                retanguloDestino)

            DesenharCamadaCamisaPng(
                g,
                imagemCamisa,
                retanguloDestino,
                ObterCorCamisaJogador(
                    jogador))

            DesenharNumeroNoSpritePng(
                g,
                jogador)

            Return True

        Catch

            Return False

        Finally

            g.InterpolationMode =
                modoInterpolacaoAnterior

            g.CompositingQuality =
                modoComposicaoAnterior

            g.PixelOffsetMode =
                deslocamentoAnterior

        End Try

    End Function

    Private Sub DesenharCamadaCamisaPng(
        g As Graphics,
        imagemCamisa As Image,
        retanguloDestino As RectangleF,
        corCamisa As Color)

        If g Is Nothing OrElse
           imagemCamisa Is Nothing Then

            Exit Sub

        End If

        Dim vermelho As Single =
            corCamisa.R / 255.0F

        Dim verde As Single =
            corCamisa.G / 255.0F

        Dim azul As Single =
            corCamisa.B / 255.0F

        Dim matrizCor As New ColorMatrix(
            New Single()() {
                New Single() {vermelho, 0.0F, 0.0F, 0.0F, 0.0F},
                New Single() {0.0F, verde, 0.0F, 0.0F, 0.0F},
                New Single() {0.0F, 0.0F, azul, 0.0F, 0.0F},
                New Single() {0.0F, 0.0F, 0.0F, 1.0F, 0.0F},
                New Single() {0.0F, 0.0F, 0.0F, 0.0F, 1.0F}
            })

        Using atributos As New ImageAttributes()

            atributos.SetColorMatrix(
                matrizCor,
                ColorMatrixFlag.Default,
                ColorAdjustType.Bitmap)

            g.DrawImage(
                imagemCamisa,
                Rectangle.Round(
                    retanguloDestino),
                0,
                0,
                imagemCamisa.Width,
                imagemCamisa.Height,
                GraphicsUnit.Pixel,
                atributos)

        End Using

    End Sub

    Private Sub DesenharNumeroNoSpritePng(
        g As Graphics,
        jogador As Jogador)

        Select Case jogador.OrientacaoVisual

            Case OrientacaoVisualJogador.Frente

                DesenharNumeroNaCamisaEmArea(
                    g,
                    jogador.Numero,
                    New RectangleF(
                        -5.5F,
                        -3.8F,
                        11.0F,
                        8.0F),
                    5.6F)

            Case OrientacaoVisualJogador.Lado,
                 OrientacaoVisualJogador.LadoEsquerdo

                DesenharNumeroNaCamisaEmArea(
                    g,
                    jogador.Numero,
                    New RectangleF(
                        -3.6F,
                        -3.2F,
                        7.2F,
                        7.5F),
                    4.5F)

            Case Else

                DesenharNumeroNaCamisa(
                    g,
                    jogador.Numero)

        End Select

    End Sub

    Private Function NormalizarEscalaVisual(
    escala As Single) As Single

        If Single.IsNaN(
        escala) OrElse
       Single.IsInfinity(
           escala) Then

            Return 1.0F

        End If

        Return Math.Max(
        0.5F,
        Math.Min(
            2.5F,
            escala))

    End Function

    Private Function ObterAnguloDirecaoJogador(
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

                Return 90.0F

        End Select

    End Function

    Private Function ObterVetorDirecaoJogador(
    direcao As DirecaoJogador) As PointF

        Const diagonal As Single =
        0.707106769F

        Select Case direcao

            Case DirecaoJogador.Direita

                Return New PointF(
                1.0F,
                0.0F)

            Case DirecaoJogador.BaixoDireita

                Return New PointF(
                diagonal,
                diagonal)

            Case DirecaoJogador.Baixo

                Return New PointF(
                0.0F,
                1.0F)

            Case DirecaoJogador.BaixoEsquerda

                Return New PointF(
                -diagonal,
                diagonal)

            Case DirecaoJogador.Esquerda

                Return New PointF(
                -1.0F,
                0.0F)

            Case DirecaoJogador.CimaEsquerda

                Return New PointF(
                -diagonal,
                -diagonal)

            Case DirecaoJogador.Cima

                Return New PointF(
                0.0F,
                -1.0F)

            Case DirecaoJogador.CimaDireita

                Return New PointF(
                diagonal,
                -diagonal)

            Case Else

                Return New PointF(
                1.0F,
                0.0F)

        End Select

    End Function

    Private Sub DesenharBracoJogador(
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

        Using canetaManga As New Pen(
        corManga,
        7.0F)

            canetaManga.StartCap =
            LineCap.Round

            canetaManga.EndCap =
            LineCap.Round

            g.DrawLine(
            canetaManga,
            ombro,
            cotovelo)

        End Using

        Using canetaPele As New Pen(
        corPele,
        4.0F)

            canetaPele.StartCap =
            LineCap.Round

            canetaPele.EndCap =
            LineCap.Round

            g.DrawLine(
            canetaPele,
            cotovelo,
            mao)

        End Using

        Using pincelMao As New SolidBrush(
        corPele)

            g.FillEllipse(
            pincelMao,
            mao.X - 2.3F,
            mao.Y - 2.3F,
            4.6F,
            4.6F)

        End Using

    End Sub

    Private Sub DesenharPernaJogador(
    g As Graphics,
    quadril As PointF,
    pe As PointF,
    corPerna As Color,
    corCalcado As Color)

        Using canetaPerna As New Pen(
        corPerna,
        5.5F)

            canetaPerna.StartCap =
            LineCap.Round

            canetaPerna.EndCap =
            LineCap.Round

            g.DrawLine(
            canetaPerna,
            quadril,
            pe)

        End Using

        Dim vetorX As Single =
        pe.X -
        quadril.X

        Dim vetorY As Single =
        pe.Y -
        quadril.Y

        Dim comprimento As Single =
        CSng(
            Math.Sqrt(
                vetorX *
                vetorX +
                vetorY *
                vetorY))

        If comprimento <= 0.001F Then
            Exit Sub
        End If

        vetorX /=
        comprimento

        vetorY /=
        comprimento

        Dim inicioCalcado As New PointF(
        pe.X -
        vetorX *
        4.5F,
        pe.Y -
        vetorY *
        4.5F)

        Using canetaCalcado As New Pen(
        corCalcado,
        3.5F)

            canetaCalcado.StartCap =
            LineCap.Round

            canetaCalcado.EndCap =
            LineCap.Round

            g.DrawLine(
            canetaCalcado,
            inicioCalcado,
            pe)

        End Using

    End Sub

    Private Sub DesenharSpriteVetorialJogador(
    g As Graphics,
    jogador As Jogador)

        If g Is Nothing OrElse
           jogador Is Nothing Then

            Exit Sub

        End If

        Dim corCamisa As Color =
            ObterCorCamisaJogador(
                jogador)

        Dim corUniformeClara As Color =
            AjustarLuminosidadeCor(
                corCamisa,
                1.25F)

        Dim corUniformeEscura As Color =
            AjustarLuminosidadeCor(
                corCamisa,
                0.52F)

        Dim corShort As Color =
            Color.FromArgb(
                45,
                45,
                50)

        Dim corPele As Color =
            Color.FromArgb(
                222,
                168,
                119)

        Dim corPeleEscura As Color =
            Color.FromArgb(
                155,
                99,
                64)

        Dim corCalcado As Color =
            Color.FromArgb(
                22,
                22,
                25)

        Dim maoEsquerda As New PointF(
            -13.0F,
            5.0F)

        Dim maoDireita As New PointF(
            13.0F,
            5.0F)

        Dim peEsquerdo As New PointF(
            -6.0F,
            22.0F)

        Dim peDireito As New PointF(
            6.0F,
            22.0F)

        Dim centroCabecaY As Single =
            -17.0F

        ConfigurarPoseVisualJogador(
            jogador.Pose,
            maoEsquerda,
            maoDireita,
            peEsquerdo,
            peDireito,
            centroCabecaY)

        Select Case jogador.OrientacaoVisual

            Case OrientacaoVisualJogador.Frente

                DesenharJogadorFrente(
                    g,
                    jogador,
                    maoEsquerda,
                    maoDireita,
                    peEsquerdo,
                    peDireito,
                    centroCabecaY,
                    corUniformeClara,
                    corUniformeEscura,
                    corShort,
                    corPele,
                    corPeleEscura,
                    corCalcado)

            Case OrientacaoVisualJogador.Lado

                DesenharJogadorLado(
                    g,
                    jogador,
                    maoEsquerda,
                    maoDireita,
                    peEsquerdo,
                    peDireito,
                    centroCabecaY,
                    corUniformeClara,
                    corUniformeEscura,
                    corShort,
                    corPele,
                    corPeleEscura,
                    corCalcado)

            Case OrientacaoVisualJogador.LadoEsquerdo

                Dim estadoLadoEsquerdo As GraphicsState =
                    g.Save()

                Try

                    g.ScaleTransform(
                        -1.0F,
                        1.0F)

                    DesenharJogadorLado(
                        g,
                        jogador,
                        maoEsquerda,
                        maoDireita,
                        peEsquerdo,
                        peDireito,
                        centroCabecaY,
                        corUniformeClara,
                        corUniformeEscura,
                        corShort,
                        corPele,
                        corPeleEscura,
                        corCalcado,
                        False)

                Finally

                    g.Restore(
                        estadoLadoEsquerdo)

                End Try

                DesenharNumeroNaCamisaEmArea(
                    g,
                    jogador.Numero,
                    New RectangleF(
                        -3.6F,
                        -3.2F,
                        7.2F,
                        7.5F),
                    4.5F)

            Case Else

                DesenharJogadorCostas(
                    g,
                    jogador,
                    maoEsquerda,
                    maoDireita,
                    peEsquerdo,
                    peDireito,
                    centroCabecaY,
                    corUniformeClara,
                    corUniformeEscura,
                    corShort,
                    corPele,
                    corPeleEscura,
                    corCalcado)

        End Select

        If jogador.Pose =
           PoseJogador.ComBola Then

            Using pincelBola As New SolidBrush(
                Color.WhiteSmoke)

                g.FillEllipse(
                    pincelBola,
                    7.5F,
                    18.0F,
                    7.0F,
                    7.0F)

            End Using

            Using bordaBola As New Pen(
                Color.FromArgb(
                    35,
                    35,
                    35),
                1.0F)

                g.DrawEllipse(
                    bordaBola,
                    7.5F,
                    18.0F,
                    7.0F,
                    7.0F)

            End Using

        End If

    End Sub

    Private Sub ConfigurarPoseVisualJogador(
    pose As PoseJogador,
    ByRef maoEsquerda As PointF,
    ByRef maoDireita As PointF,
    ByRef peEsquerdo As PointF,
    ByRef peDireito As PointF,
    ByRef centroCabecaY As Single)

        Select Case pose

            Case PoseJogador.Correndo

                maoEsquerda =
                    New PointF(
                        -11.0F,
                        -9.0F)

                maoDireita =
                    New PointF(
                        13.0F,
                        11.0F)

                peEsquerdo =
                    New PointF(
                        -9.0F,
                        24.0F)

                peDireito =
                    New PointF(
                        9.0F,
                        16.0F)

            Case PoseJogador.ComBola

                maoEsquerda =
                    New PointF(
                        -12.0F,
                        3.0F)

                maoDireita =
                    New PointF(
                        11.0F,
                        -2.0F)

                peEsquerdo =
                    New PointF(
                        -7.0F,
                        22.0F)

                peDireito =
                    New PointF(
                        10.0F,
                        18.0F)

            Case PoseJogador.Passe

                maoEsquerda =
                    New PointF(
                        -15.0F,
                        1.0F)

                maoDireita =
                    New PointF(
                        15.0F,
                        1.0F)

                peEsquerdo =
                    New PointF(
                        -5.0F,
                        22.0F)

                peDireito =
                    New PointF(
                        13.0F,
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

                peEsquerdo =
                    New PointF(
                        -7.0F,
                        23.0F)

                peDireito =
                    New PointF(
                        16.0F,
                        10.0F)

            Case PoseJogador.Cabeceio

                maoEsquerda =
                    New PointF(
                        -17.0F,
                        -4.0F)

                maoDireita =
                    New PointF(
                        17.0F,
                        -4.0F)

                peEsquerdo =
                    New PointF(
                        -7.0F,
                        21.0F)

                peDireito =
                    New PointF(
                        7.0F,
                        21.0F)

                centroCabecaY =
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
                        -22.0F,
                        -5.0F)

                maoDireita =
                    New PointF(
                        22.0F,
                        -5.0F)

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

    Private Sub DesenharJogadorCostas(
    g As Graphics,
    jogador As Jogador,
    maoEsquerda As PointF,
    maoDireita As PointF,
    peEsquerdo As PointF,
    peDireito As PointF,
    centroCabecaY As Single,
    corUniformeClara As Color,
    corUniformeEscura As Color,
    corShort As Color,
    corPele As Color,
    corPeleEscura As Color,
    corCalcado As Color)

        DesenharPernaJogador(
            g,
            New PointF(
                -4.0F,
                9.0F),
            peEsquerdo,
            corShort,
            corCalcado)

        DesenharPernaJogador(
            g,
            New PointF(
                4.0F,
                9.0F),
            peDireito,
            corShort,
            corCalcado)

        DesenharBracoJogador(
            g,
            New PointF(
                -7.0F,
                -5.0F),
            maoEsquerda,
            corUniformeEscura,
            corPele)

        DesenharBracoJogador(
            g,
            New PointF(
                7.0F,
                -5.0F),
            maoDireita,
            corUniformeEscura,
            corPele)

        DesenharShortJogador(
            g,
            corShort)

        DesenharTorsoJogador(
            g,
            corUniformeClara,
            corUniformeEscura,
            False)

        DesenharCabecaJogadorCostas(
            g,
            centroCabecaY,
            corPele,
            corPeleEscura)

        DesenharNumeroNaCamisa(
            g,
            jogador.Numero)

    End Sub

    Private Sub DesenharJogadorFrente(
    g As Graphics,
    jogador As Jogador,
    maoEsquerda As PointF,
    maoDireita As PointF,
    peEsquerdo As PointF,
    peDireito As PointF,
    centroCabecaY As Single,
    corUniformeClara As Color,
    corUniformeEscura As Color,
    corShort As Color,
    corPele As Color,
    corPeleEscura As Color,
    corCalcado As Color)

        DesenharPernaJogador(
            g,
            New PointF(
                -4.0F,
                9.0F),
            peEsquerdo,
            corShort,
            corCalcado)

        DesenharPernaJogador(
            g,
            New PointF(
                4.0F,
                9.0F),
            peDireito,
            corShort,
            corCalcado)

        DesenharBracoJogador(
            g,
            New PointF(
                -7.0F,
                -5.0F),
            maoEsquerda,
            corUniformeEscura,
            corPele)

        DesenharBracoJogador(
            g,
            New PointF(
                7.0F,
                -5.0F),
            maoDireita,
            corUniformeEscura,
            corPele)

        DesenharShortJogador(
            g,
            corShort)

        DesenharTorsoJogador(
            g,
            corUniformeClara,
            corUniformeEscura,
            True)

        DesenharCabecaJogadorFrente(
            g,
            centroCabecaY,
            corPele,
            corPeleEscura)

        DesenharNumeroNaCamisaEmArea(
            g,
            jogador.Numero,
            New RectangleF(
                -5.5F,
                -3.8F,
                11.0F,
                8.0F),
            5.6F)

    End Sub

    Private Sub DesenharJogadorLado(
    g As Graphics,
    jogador As Jogador,
    maoEsquerda As PointF,
    maoDireita As PointF,
    peEsquerdo As PointF,
    peDireito As PointF,
    centroCabecaY As Single,
    corUniformeClara As Color,
    corUniformeEscura As Color,
    corShort As Color,
    corPele As Color,
    corPeleEscura As Color,
    corCalcado As Color,
    Optional desenharNumero As Boolean = True)

        Dim maoTraseira As New PointF(
            maoEsquerda.X * 0.42F - 1.0F,
            maoEsquerda.Y)

        Dim maoFrontal As New PointF(
            maoDireita.X * 0.42F + 2.0F,
            maoDireita.Y)

        Dim peTraseiro As New PointF(
            peEsquerdo.X * 0.48F - 1.0F,
            peEsquerdo.Y)

        Dim peFrontal As New PointF(
            peDireito.X * 0.48F + 2.0F,
            peDireito.Y)

        DesenharPernaJogador(
            g,
            New PointF(
                -1.5F,
                9.0F),
            peTraseiro,
            AjustarLuminosidadeCor(
                corShort,
                0.72F),
            corCalcado)

        DesenharBracoJogador(
            g,
            New PointF(
                -2.5F,
                -4.5F),
            maoTraseira,
            AjustarLuminosidadeCor(
                corUniformeEscura,
                0.78F),
            AjustarLuminosidadeCor(
                corPele,
                0.88F))

        DesenharShortJogadorLado(
            g,
            corShort)

        DesenharTorsoJogadorLado(
            g,
            corUniformeClara,
            corUniformeEscura)

        DesenharPernaJogador(
            g,
            New PointF(
                2.0F,
                9.0F),
            peFrontal,
            corShort,
            corCalcado)

        DesenharBracoJogador(
            g,
            New PointF(
                2.8F,
                -4.5F),
            maoFrontal,
            corUniformeEscura,
            corPele)

        DesenharCabecaJogadorLado(
            g,
            centroCabecaY,
            corPele,
            corPeleEscura)

        If desenharNumero Then

            DesenharNumeroNaCamisaEmArea(
                g,
                jogador.Numero,
                New RectangleF(
                    -3.6F,
                    -3.2F,
                    7.2F,
                    7.5F),
                4.5F)

        End If

    End Sub

    Private Sub DesenharShortJogador(
    g As Graphics,
    corShort As Color)

        Dim pontosShort() As PointF = {
            New PointF(
                -6.5F,
                6.0F),
            New PointF(
                6.5F,
                6.0F),
            New PointF(
                5.0F,
                13.0F),
            New PointF(
                0.0F,
                10.0F),
            New PointF(
                -5.0F,
                13.0F)
        }

        Using pincelShort As New LinearGradientBrush(
            New RectangleF(
                -7.0F,
                6.0F,
                14.0F,
                8.0F),
            Color.FromArgb(
                75,
                75,
                82),
            corShort,
            90.0F)

            g.FillPolygon(
                pincelShort,
                pontosShort)

        End Using

    End Sub

    Private Sub DesenharShortJogadorLado(
    g As Graphics,
    corShort As Color)

        Dim pontosShort() As PointF = {
            New PointF(
                -3.8F,
                6.0F),
            New PointF(
                4.5F,
                6.0F),
            New PointF(
                3.8F,
                12.0F),
            New PointF(
                -2.8F,
                12.0F)
        }

        Using pincelShort As New LinearGradientBrush(
            New RectangleF(
                -4.0F,
                6.0F,
                8.5F,
                7.0F),
            Color.FromArgb(
                78,
                80,
                88),
            corShort,
            0.0F)

            g.FillPolygon(
                pincelShort,
                pontosShort)

        End Using

    End Sub

    Private Sub DesenharTorsoJogador(
    g As Graphics,
    corUniformeClara As Color,
    corUniformeEscura As Color,
    desenharGolaFrontal As Boolean)

        Dim pontosTorso() As PointF = {
            New PointF(
                -8.5F,
                -7.0F),
            New PointF(
                8.5F,
                -7.0F),
            New PointF(
                7.0F,
                6.0F),
            New PointF(
                4.5F,
                10.0F),
            New PointF(
                -4.5F,
                10.0F),
            New PointF(
                -7.0F,
                6.0F)
        }

        Using caminhoTorso As New GraphicsPath()

            caminhoTorso.AddPolygon(
                pontosTorso)

            Using pincelTorso As New LinearGradientBrush(
                New RectangleF(
                    -9.0F,
                    -8.0F,
                    18.0F,
                    19.0F),
                corUniformeClara,
                corUniformeEscura,
                90.0F)

                g.FillPath(
                    pincelTorso,
                    caminhoTorso)

            End Using

            Using bordaTorso As New Pen(
                Color.FromArgb(
                    230,
                    255,
                    255,
                    255),
                1.4F)

                bordaTorso.LineJoin =
                    LineJoin.Round

                g.DrawPath(
                    bordaTorso,
                    caminhoTorso)

            End Using

        End Using

        Using brilho As New SolidBrush(
            Color.FromArgb(
                55,
                255,
                255,
                255))

            g.FillEllipse(
                brilho,
                -5.5F,
                -5.0F,
                7.0F,
                5.0F)

        End Using

        If desenharGolaFrontal Then

            Using canetaGola As New Pen(
                Color.FromArgb(
                    210,
                    255,
                    255,
                    255),
                1.1F)

                canetaGola.LineJoin =
                    LineJoin.Round

                g.DrawLines(
                    canetaGola,
                    New PointF() {
                        New PointF(
                            -3.0F,
                            -6.2F),
                        New PointF(
                            0.0F,
                            -3.5F),
                        New PointF(
                            3.0F,
                            -6.2F)
                    })

            End Using

        End If

    End Sub

    Private Sub DesenharTorsoJogadorLado(
    g As Graphics,
    corUniformeClara As Color,
    corUniformeEscura As Color)

        Dim pontosTorso() As PointF = {
            New PointF(
                -4.2F,
                -7.0F),
            New PointF(
                5.2F,
                -6.0F),
            New PointF(
                4.2F,
                7.0F),
            New PointF(
                2.8F,
                10.0F),
            New PointF(
                -2.8F,
                10.0F),
            New PointF(
                -3.8F,
                6.0F)
        }

        Using caminhoTorso As New GraphicsPath()

            caminhoTorso.AddPolygon(
                pontosTorso)

            Using pincelTorso As New LinearGradientBrush(
                New RectangleF(
                    -4.5F,
                    -7.5F,
                    10.0F,
                    18.0F),
                corUniformeEscura,
                corUniformeClara,
                0.0F)

                g.FillPath(
                    pincelTorso,
                    caminhoTorso)

            End Using

            Using bordaTorso As New Pen(
                Color.FromArgb(
                    220,
                    255,
                    255,
                    255),
                1.2F)

                bordaTorso.LineJoin =
                    LineJoin.Round

                g.DrawPath(
                    bordaTorso,
                    caminhoTorso)

            End Using

        End Using

        Using brilho As New SolidBrush(
            Color.FromArgb(
                48,
                255,
                255,
                255))

            g.FillEllipse(
                brilho,
                0.0F,
                -4.5F,
                3.8F,
                5.0F)

        End Using

    End Sub

    Private Sub DesenharCabecaJogadorCostas(
    g As Graphics,
    centroCabecaY As Single,
    corPele As Color,
    corPeleEscura As Color)

        Dim retanguloCabeca As New RectangleF(
            -6.0F,
            centroCabecaY - 6.0F,
            12.0F,
            12.0F)

        DesenharBaseCabecaJogador(
            g,
            retanguloCabeca,
            corPele,
            corPeleEscura)

        Using pincelCabelo As New SolidBrush(
            Color.FromArgb(
                45,
                30,
                24))

            g.FillPie(
                pincelCabelo,
                retanguloCabeca,
                170.0F,
                200.0F)

            g.FillRectangle(
                pincelCabelo,
                retanguloCabeca.X + 1.2F,
                retanguloCabeca.Y + 4.2F,
                retanguloCabeca.Width - 2.4F,
                3.0F)

        End Using

    End Sub

    Private Sub DesenharCabecaJogadorFrente(
    g As Graphics,
    centroCabecaY As Single,
    corPele As Color,
    corPeleEscura As Color)

        Dim retanguloCabeca As New RectangleF(
            -6.0F,
            centroCabecaY - 6.0F,
            12.0F,
            12.0F)

        DesenharBaseCabecaJogador(
            g,
            retanguloCabeca,
            corPele,
            corPeleEscura)

        Using pincelCabelo As New SolidBrush(
            Color.FromArgb(
                45,
                30,
                24))

            g.FillPie(
                pincelCabelo,
                retanguloCabeca,
                180.0F,
                180.0F)

        End Using

        Using pincelOlhos As New SolidBrush(
            Color.FromArgb(
                55,
                38,
                30))

            g.FillEllipse(
                pincelOlhos,
                -3.2F,
                centroCabecaY - 0.8F,
                1.5F,
                1.5F)

            g.FillEllipse(
                pincelOlhos,
                1.7F,
                centroCabecaY - 0.8F,
                1.5F,
                1.5F)

        End Using

        Using canetaBoca As New Pen(
            Color.FromArgb(
                125,
                72,
                58),
            0.8F)

            g.DrawArc(
                canetaBoca,
                -2.0F,
                centroCabecaY + 1.0F,
                4.0F,
                2.5F,
                10.0F,
                160.0F)

        End Using

    End Sub

    Private Sub DesenharCabecaJogadorLado(
    g As Graphics,
    centroCabecaY As Single,
    corPele As Color,
    corPeleEscura As Color)

        Dim retanguloCabeca As New RectangleF(
            -4.8F,
            centroCabecaY - 6.0F,
            10.0F,
            12.0F)

        DesenharBaseCabecaJogador(
            g,
            retanguloCabeca,
            corPele,
            corPeleEscura)

        Using pincelCabelo As New SolidBrush(
            Color.FromArgb(
                45,
                30,
                24))

            g.FillPie(
                pincelCabelo,
                retanguloCabeca,
                115.0F,
                205.0F)

        End Using

        Using pincelNariz As New SolidBrush(
            corPele)

            g.FillPolygon(
                pincelNariz,
                New PointF() {
                    New PointF(
                        4.0F,
                        centroCabecaY - 0.8F),
                    New PointF(
                        7.0F,
                        centroCabecaY + 0.3F),
                    New PointF(
                        4.0F,
                        centroCabecaY + 1.3F)
                })

        End Using

        Using pincelOlho As New SolidBrush(
            Color.FromArgb(
                55,
                38,
                30))

            g.FillEllipse(
                pincelOlho,
                1.2F,
                centroCabecaY - 1.2F,
                1.5F,
                1.5F)

        End Using

        Using canetaBoca As New Pen(
            Color.FromArgb(
                125,
                72,
                58),
            0.8F)

            g.DrawLine(
                canetaBoca,
                2.0F,
                centroCabecaY + 2.6F,
                4.3F,
                centroCabecaY + 2.6F)

        End Using

    End Sub

    Private Sub DesenharBaseCabecaJogador(
    g As Graphics,
    retanguloCabeca As RectangleF,
    corPele As Color,
    corPeleEscura As Color)

        Using pincelCabeca As New LinearGradientBrush(
            retanguloCabeca,
            AjustarLuminosidadeCor(
                corPele,
                1.1F),
            corPeleEscura,
            90.0F)

            g.FillEllipse(
                pincelCabeca,
                retanguloCabeca)

        End Using

        Using bordaCabeca As New Pen(
            Color.FromArgb(
                120,
                65,
                35),
            1.2F)

            g.DrawEllipse(
                bordaCabeca,
                retanguloCabeca)

        End Using

    End Sub

    Private Function ObterCorCamisaJogador(
    jogador As Jogador) As Color

        Dim corPadrao As Color =
        Color.FromArgb(
            185,
            35,
            35)

        If jogador Is Nothing Then
            Return corPadrao
        End If

        Try

            Dim corSalva As Color =
            Color.FromArgb(
                jogador.CorCamisaArgb)

            If corSalva.A = 0 Then
                Return corPadrao
            End If

            Return Color.FromArgb(
            255,
            corSalva.R,
            corSalva.G,
            corSalva.B)

        Catch

            Return corPadrao

        End Try

    End Function

    Private Function AjustarLuminosidadeCor(
    cor As Color,
    fator As Single) As Color

        Dim vermelho As Integer =
        CInt(
            Math.Round(
                cor.R *
                fator))

        Dim verde As Integer =
        CInt(
            Math.Round(
                cor.G *
                fator))

        Dim azul As Integer =
        CInt(
            Math.Round(
                cor.B *
                fator))

        vermelho =
        Math.Max(
            0,
            Math.Min(
                255,
                vermelho))

        verde =
        Math.Max(
            0,
            Math.Min(
                255,
                verde))

        azul =
        Math.Max(
            0,
            Math.Min(
                255,
                azul))

        Return Color.FromArgb(
        255,
        vermelho,
        verde,
        azul)

    End Function

    Private Sub DesenharNumeroNaCamisa(
    g As Graphics,
    numero As Integer)

        If g Is Nothing Then
            Exit Sub
        End If

        Dim textoNumero As String =
        numero.ToString()

        Dim tamanhoFonte As Single

        Select Case textoNumero.Length

            Case 1

                tamanhoFonte =
                7.5F

            Case 2

                tamanhoFonte =
                5.8F

            Case Else

                tamanhoFonte =
                4.8F

        End Select

        Dim retanguloNumero As New RectangleF(
        -8.0F,
        -5.5F,
        16.0F,
        12.5F)

        Using fonteNumero As New Font(
        "Segoe UI",
        tamanhoFonte,
        FontStyle.Bold,
        GraphicsUnit.Point)

            Using pincelSombra As New SolidBrush(
            Color.FromArgb(
                120,
                0,
                0,
                0))

                Using formato As New StringFormat With {
                .Alignment =
                    StringAlignment.Center,
                .LineAlignment =
                    StringAlignment.Center,
                .Trimming =
                    StringTrimming.None,
                .FormatFlags =
                    StringFormatFlags.NoWrap Or
                    StringFormatFlags.NoClip
            }

                    Dim retanguloSombra As New RectangleF(
                    retanguloNumero.X +
                    0.5F,
                    retanguloNumero.Y +
                    0.7F,
                    retanguloNumero.Width,
                    retanguloNumero.Height)

                    g.DrawString(
                    textoNumero,
                    fonteNumero,
                    pincelSombra,
                    retanguloSombra,
                    formato)

                End Using

            End Using

            Using pincelNumero As New SolidBrush(
            Color.White)

                Using formato As New StringFormat With {
                .Alignment =
                    StringAlignment.Center,
                .LineAlignment =
                    StringAlignment.Center,
                .Trimming =
                    StringTrimming.None,
                .FormatFlags =
                    StringFormatFlags.NoWrap Or
                    StringFormatFlags.NoClip
            }

                    g.DrawString(
                    textoNumero,
                    fonteNumero,
                    pincelNumero,
                    retanguloNumero,
                    formato)

                End Using

            End Using

        End Using

    End Sub

    Private Sub DesenharNumeroNaCamisaEmArea(
    g As Graphics,
    numero As Integer,
    retanguloNumero As RectangleF,
    tamanhoFonteUmDigito As Single)

        If g Is Nothing Then
            Exit Sub
        End If

        Dim textoNumero As String =
            numero.ToString()

        Dim tamanhoFonte As Single =
            tamanhoFonteUmDigito

        Select Case textoNumero.Length

            Case 1

                'Mantém o tamanho original.

            Case 2

                tamanhoFonte *=
                    0.78F

            Case Else

                tamanhoFonte *=
                    0.62F

        End Select

        Using fonteNumero As New Font(
            "Segoe UI",
            tamanhoFonte,
            FontStyle.Bold,
            GraphicsUnit.Point)

            Using formato As New StringFormat With {
                .Alignment =
                    StringAlignment.Center,
                .LineAlignment =
                    StringAlignment.Center,
                .Trimming =
                    StringTrimming.None,
                .FormatFlags =
                    StringFormatFlags.NoWrap Or
                    StringFormatFlags.NoClip
            }

                Dim retanguloSombra As New RectangleF(
                    retanguloNumero.X +
                    0.4F,
                    retanguloNumero.Y +
                    0.6F,
                    retanguloNumero.Width,
                    retanguloNumero.Height)

                Using pincelSombra As New SolidBrush(
                    Color.FromArgb(
                        120,
                        0,
                        0,
                        0))

                    g.DrawString(
                        textoNumero,
                        fonteNumero,
                        pincelSombra,
                        retanguloSombra,
                        formato)

                End Using

                Using pincelNumero As New SolidBrush(
                    Color.White)

                    g.DrawString(
                        textoNumero,
                        fonteNumero,
                        pincelNumero,
                        retanguloNumero,
                        formato)

                End Using

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

        Dim escalaObjeto As Single =
            NormalizarEscalaVisual(
                bola.EscalaVisual)

        Dim estadoEscalaObjeto As GraphicsState =
            g.Save()

        Try

            g.TranslateTransform(
                centro.X,
                centro.Y)

            g.ScaleTransform(
                escalaObjeto,
                escalaObjeto)

            g.TranslateTransform(
                -centro.X,
                -centro.Y)

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


        Finally

            g.Restore(
                estadoEscalaObjeto)

        End Try

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

        Dim escalaObjeto As Single =
            NormalizarEscalaVisual(
                cone.EscalaVisual)

        Dim estadoEscalaObjeto As GraphicsState =
            g.Save()

        Try

            g.TranslateTransform(
                centro.X,
                centro.Y)

            g.ScaleTransform(
                escalaObjeto,
                escalaObjeto)

            g.TranslateTransform(
                -centro.X,
                -centro.Y)

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


        Finally

            g.Restore(
                estadoEscalaObjeto)

        End Try

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

        Dim escalaObjeto As Single =
            NormalizarEscalaVisual(
                gol.EscalaVisual)

        Dim estadoEscalaObjeto As GraphicsState =
            g.Save()

        Try

            g.TranslateTransform(
                centro.X,
                centro.Y)

            g.ScaleTransform(
                escalaObjeto,
                escalaObjeto)

            g.TranslateTransform(
                -centro.X,
                -centro.Y)

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


        Finally

            g.Restore(
                estadoEscalaObjeto)

        End Try

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

        Dim escalaObjeto As Single =
            NormalizarEscalaVisual(
                manequim.EscalaVisual)

        Dim estadoEscalaObjeto As GraphicsState =
            g.Save()

        Try

            g.TranslateTransform(
                centro.X,
                centro.Y)

            g.ScaleTransform(
                escalaObjeto,
                escalaObjeto)

            g.TranslateTransform(
                -centro.X,
                -centro.Y)

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


        Finally

            g.Restore(
                estadoEscalaObjeto)

        End Try

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

        Dim escalaVisual As Single =
            NormalizarEscalaVisual(
                linha.EscalaVisual)

        Using caneta As New Pen(
        corLinha,
        linha.Espessura * escalaVisual)

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
                5.0F * escalaVisual,
                7.0F * escalaVisual,
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

        Dim escalaVisual As Single =
            NormalizarEscalaVisual(
                area.EscalaVisual)

        Using canetaBorda As New Pen(
        corArea,
        area.Espessura * escalaVisual)

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

        Dim escalaObjeto As Single =
            NormalizarEscalaVisual(
                marcador.EscalaVisual)

        Dim estadoEscalaObjeto As GraphicsState =
            g.Save()

        Try

            g.TranslateTransform(
                centro.X,
                centro.Y)

            g.ScaleTransform(
                escalaObjeto,
                escalaObjeto)

            g.TranslateTransform(
                -centro.X,
                -centro.Y)

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


        Finally

            g.Restore(
                estadoEscalaObjeto)

        End Try

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

        Dim escalaVisual As Single =
            NormalizarEscalaVisual(
                texto.EscalaVisual)

        Return New Font(
        "Segoe UI",
        texto.TamanhoFonte * escalaVisual,
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
        Dim mapaGrupos As New Dictionary(Of String, String)(StringComparer.Ordinal)

        For Each objetoOriginal As ObjetoCampo In objetosOriginais

            Dim estadoObjeto As EstadoObjetoCampo =
                CapturarEstadoObjeto(objetoOriginal)

            Dim objetoDuplicado As ObjetoCampo =
                CriarObjetoDoEstado(estadoObjeto)

            Dim grupoOriginal As String =
    If(
        objetoOriginal.GrupoId,
        String.Empty)

            If Not String.IsNullOrWhiteSpace(
    grupoOriginal) Then

                Dim novoGrupo As String =
        String.Empty

                If Not mapaGrupos.TryGetValue(
        grupoOriginal,
        novoGrupo) Then

                    novoGrupo =
            Guid.NewGuid().
            ToString("N")

                    mapaGrupos.Add(
            grupoOriginal,
            novoGrupo)

                End If

                objetoDuplicado.GrupoId =
        novoGrupo

            Else

                objetoDuplicado.GrupoId =
        String.Empty

            End If

            objetoDuplicado.Bloqueado =
    False

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

        TrazerSelecionadosParaFrente()

    End Sub

    Public Sub EnviarParaTras()

        EnviarSelecionadosParaTras()

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

        RaiseEvent ObjetosAlterados()

    End Sub

    Private Sub AtualizarDisponibilidadeHistorico()

        RaiseEvent HistoricoAlterado(
        PodeDesfazer,
        PodeRefazer)

    End Sub

    Private Function CapturarEstadoAtual() As EstadoCampo

        Dim estadoCampo As New EstadoCampo With {
            .EstiloVisualValor =
                CInt(
                    _estiloVisualCampo),
            .IntensidadeTextura =
                _intensidadeTexturaCampo,
            .FaixasGramaVisiveis =
                _faixasGramaVisiveis,
            .SombrasCampoAtivas =
                _sombrasCampoAtivas
        }

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

    Private Function CapturarEstadoObjeto(objeto As ObjetoCampo) As EstadoObjetoCampo

        If objeto Is Nothing Then

            Throw New ArgumentNullException(
            NameOf(objeto))

        End If

        Dim tipoObjeto As String =
        objeto.GetType().Name

        Dim estado As New EstadoObjetoCampo With {
        .TipoObjeto = tipoObjeto,
        .X = objeto.Posicao.X,
        .Y = objeto.Posicao.Y,
        .Visivel = objeto.Visivel,
        .NomePersonalizado =
            If(
                objeto.NomePersonalizado,
                String.Empty),
        .GrupoId =
            If(
                objeto.GrupoId,
                String.Empty),
        .Bloqueado = objeto.Bloqueado,
        .EscalaVisual = objeto.EscalaVisual
    }

        Select Case tipoObjeto

            Case "Jogador"

                Dim objetoJogador As Jogador =
                DirectCast(
                    objeto,
                    Jogador)

                estado.Numero = objetoJogador.Numero

                estado.Nome = objetoJogador.Nome

                estado.DirecaoDoJogador = objetoJogador.Direcao

                estado.OrientacaoVisualDoJogador =
                    objetoJogador.OrientacaoVisual

                estado.PoseDoJogador = objetoJogador.Pose

                estado.CorCamisaJogadorArgb = objetoJogador.CorCamisaArgb

            Case "Bola"

            Case "Cone"

                Dim objetoCone As Cone =
                DirectCast(
                    objeto,
                    Cone)

                estado.CorConeValor =
                CInt(
                    objetoCone.Cor)

            Case "Gol"

                Dim objetoGol As Gol =
                DirectCast(
                    objeto,
                    Gol)

                estado.OrientacaoGolValor =
                CInt(
                    objetoGol.Orientacao)

            Case "Manequim"

                Dim objetoManequim As Manequim =
                DirectCast(
                    objeto,
                    Manequim)

                estado.CorManequimValor =
                CInt(
                    objetoManequim.Cor)

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
                CInt(
                    objetoLinha.Tipo)

                estado.CorLinhaValor =
                CInt(
                    objetoLinha.Cor)

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
                CInt(
                    objetoArea.Cor)

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
                CInt(
                    objetoMarcador.Cor)

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
                CInt(
                    objetoTexto.Cor)

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

    Private Function CriarObjetoDoEstado(estado As EstadoObjetoCampo) As ObjetoCampo

        Dim objeto As ObjetoCampo = Nothing

        Select Case estado.TipoObjeto

            Case "Jogador"

                objeto =
    New Jogador With {
        .Numero = estado.Numero,
        .Nome = estado.Nome,
        .Direcao = estado.DirecaoDoJogador,
        .OrientacaoVisual =
            estado.OrientacaoVisualDoJogador,
        .Pose = estado.PoseDoJogador,
        .CorCamisaArgb =
            estado.CorCamisaJogadorArgb
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

        objeto.NomePersonalizado =
    If(
        estado.NomePersonalizado,
        String.Empty)

        objeto.GrupoId =
    If(
        estado.GrupoId,
        String.Empty)

        objeto.Bloqueado =
    estado.Bloqueado

        objeto.EscalaVisual =
    estado.EscalaVisual

        If TypeOf objeto Is Jogador Then

            Dim jogador As Jogador =
        DirectCast(
            objeto,
            Jogador)

            jogador.Direcao =
        estado.DirecaoDoJogador

            jogador.OrientacaoVisual =
        estado.OrientacaoVisualDoJogador

            jogador.Pose =
        estado.PoseDoJogador

        End If

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
        .VersaoFormato = 4,
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

        If arquivo.VersaoFormato > 4 Then

            Throw New System.IO.InvalidDataException(
            "Este exercício foi criado em uma versão mais recente do TacticalStudio.")

        End If

        If arquivo.VersaoFormato <= 2 Then

            MigrarOrientacaoVisualLegada(
                arquivo.Campo)

        End If

        If arquivo.VersaoFormato <= 3 Then

            arquivo.VersaoFormato = 4

        End If

        LimparRecorteCampo()

        AplicarEstadoCampo(
        arquivo.Campo)

        ReiniciarHistorico()

        Return arquivo

    End Function

    Private Sub MigrarOrientacaoVisualLegada(
    estadoCampo As EstadoCampo)

        If estadoCampo Is Nothing OrElse
           estadoCampo.Objetos Is Nothing Then

            Exit Sub

        End If

        For Each estado As EstadoObjetoCampo
        In estadoCampo.Objetos

            If estado Is Nothing OrElse
               Not String.Equals(
                   estado.TipoObjeto,
                   "Jogador",
                   StringComparison.OrdinalIgnoreCase) Then

                Continue For

            End If

            Select Case estado.DirecaoDoJogador

                Case DirecaoJogador.Cima,
                     DirecaoJogador.CimaDireita,
                     DirecaoJogador.CimaEsquerda

                    estado.OrientacaoVisualDoJogador =
                        OrientacaoVisualJogador.Costas

                Case DirecaoJogador.Baixo,
                     DirecaoJogador.BaixoDireita,
                     DirecaoJogador.BaixoEsquerda

                    estado.OrientacaoVisualDoJogador =
                        OrientacaoVisualJogador.Frente

                Case DirecaoJogador.Direita

                    estado.OrientacaoVisualDoJogador =
                        OrientacaoVisualJogador.Lado

                Case DirecaoJogador.Esquerda

                    estado.OrientacaoVisualDoJogador =
                        OrientacaoVisualJogador.LadoEsquerdo

                Case Else

                    estado.OrientacaoVisualDoJogador =
                        OrientacaoVisualJogador.Costas

            End Select

        Next

    End Sub

    Public Sub NovoExercicio()

        LimparRecorteCampo()

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

            If estadoCampo Is Nothing Then

                estadoCampo =
                    New EstadoCampo()

            End If

            Select Case estadoCampo.EstiloVisualValor

                Case CInt(EstiloVisualCampo.Classico),
                     CInt(EstiloVisualCampo.Estadio),
                     CInt(EstiloVisualCampo.Treino),
                     CInt(EstiloVisualCampo.Noturno)

                    _estiloVisualCampo =
                        CType(
                            estadoCampo.EstiloVisualValor,
                            EstiloVisualCampo)

                Case Else

                    _estiloVisualCampo =
                        EstiloVisualCampo.Estadio

            End Select

            _intensidadeTexturaCampo =
                Math.Max(
                    0,
                    Math.Min(
                        100,
                        estadoCampo.IntensidadeTextura))

            _faixasGramaVisiveis =
                estadoCampo.FaixasGramaVisiveis

            _sombrasCampoAtivas =
                estadoCampo.SombrasCampoAtivas

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

        RaiseEvent ObjetosAlterados()

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

    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)

        MyBase.OnMouseDown(e)

        Focus()

        If _modoSelecaoRecorte Then

            If e.Button = MouseButtons.Right Then

                CancelarSelecaoRecorteCampo()

                Exit Sub

            End If

            If e.Button <> MouseButtons.Left Then
                Exit Sub
            End If

            Dim pontoMundo As PointF =
                ConverterTelaParaMundo(
                    e.Location)

            Dim campoRecorte As RectangleF =
                ObterRetanguloCampo()

            If Not campoRecorte.Contains(
                pontoMundo) Then

                Exit Sub

            End If

            _desenhandoRecorte =
                True

            _pontoInicialRecorte =
                pontoMundo

            _pontoAtualRecorte =
                pontoMundo

            Capture =
                True

            Cursor =
                Cursors.Cross

            Invalidate()

            Exit Sub

        End If

        If e.Button = MouseButtons.Middle OrElse
           (_espacoPressionado AndAlso
            e.Button = MouseButtons.Left) Then

            _panEmAndamento =
                True

            _pontoInicialPan =
                e.Location

            _deslocamentoInicialPan =
                _deslocamentoVisual

            Capture =
                True

            Cursor =
                Cursors.SizeAll

            Exit Sub

        End If

        e =
            CriarEventoMouseMundo(
                e)

        Dim campo As RectangleF =
            ObterRetanguloCampo()

        If e.Button = MouseButtons.Right Then

            CancelarCriacao()

            Exit Sub

        End If

        If e.Button <> MouseButtons.Left Then
            Exit Sub
        End If

        If Not campo.Contains(
            CSng(e.X),
            CSng(e.Y)) Then

            Exit Sub

        End If

        If FerramentaAtual <>
           FerramentaCampo.Selecionar Then

            ExecutarFerramentaAtual(
                e.Location,
                campo)

            Exit Sub

        End If

        Dim selecaoAdicional As Boolean =
            (ModifierKeys And Keys.Control) =
            Keys.Control OrElse
            (ModifierKeys And Keys.Shift) =
            Keys.Shift

        If Not selecaoAdicional AndAlso
           _objetosSelecionados.Count = 1 Then

            Dim manipuladorAtual As ModoManipulacaoCampo =
                LocalizarManipuladorSelecionado(
                    e.Location,
                    campo)

            If manipuladorAtual <>
               ModoManipulacaoCampo.Nenhum Then

                IniciarManipulacaoPorAlca(
                    manipuladorAtual,
                    e.Location,
                    campo)

                Exit Sub

            End If

        End If

        Dim objetoClicado As ObjetoCampo =
            LocalizarObjetoNaPosicao(
                e.Location,
                campo)

        If selecaoAdicional Then

            If objetoClicado IsNot Nothing Then

                AlternarSelecaoObjeto(
                    objetoClicado)

                NotificarSelecaoAlterada()

            End If

            Exit Sub

        End If

        If objetoClicado Is Nothing Then

            DeselecionarTodos()

            _arrastando =
                False

            _modoManipulacao =
                ModoManipulacaoCampo.Nenhum

            NotificarSelecaoAlterada()

            Cursor =
                Cursors.Default

            Exit Sub

        End If

        If Not ObjetoEstaSelecionado(
            objetoClicado) Then

            SelecionarSomente(
                objetoClicado)

            NotificarSelecaoAlterada()

        Else

            _objetoSelecionado =
                objetoClicado

        End If

        If _objetosSelecionados.Count = 1 Then

            Dim manipuladorNovo As ModoManipulacaoCampo =
                LocalizarManipuladorSelecionado(
                    e.Location,
                    campo)

            If manipuladorNovo <>
               ModoManipulacaoCampo.Nenhum Then

                IniciarManipulacaoPorAlca(
                    manipuladorNovo,
                    e.Location,
                    campo)

                Exit Sub

            End If

        End If

        If Not PodeManipularSelecao() Then

            _arrastando =
                False

            _movendoGrupo =
                False

            Capture =
                False

            Cursor =
                Cursors.No

            Invalidate()

            Exit Sub

        End If

        If _objetosSelecionados.Count > 1 Then

            PrepararMovimentoGrupo(
                e.Location)

        Else

            _movendoGrupo =
                False

            If TypeOf _objetoSelecionado Is LinhaTatica Then

                PrepararManipulacaoLinha(
                    DirectCast(
                        _objetoSelecionado,
                        LinhaTatica),
                    e.Location,
                    campo)

            Else

                Dim centro As PointF =
                    ObterCentroObjetoTela(
                        _objetoSelecionado,
                        campo)

                _offsetMouse =
                    New PointF(
                        e.X - centro.X,
                        e.Y - centro.Y)

            End If

        End If

        _houveAlteracaoManipulacao =
            False

        _modoManipulacao =
            ModoManipulacaoCampo.MoverObjeto

        _arrastando =
            True

        Capture =
            True

        Cursor =
            Cursors.SizeAll

        Invalidate()

    End Sub

    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)

        MyBase.OnMouseMove(e)

        If _modoSelecaoRecorte Then

            Cursor =
                Cursors.Cross

            If _desenhandoRecorte Then

                Dim pontoMundo As PointF =
                    ConverterTelaParaMundo(
                        e.Location)

                Dim campoRecorte As RectangleF =
                    ObterRetanguloCampo()

                _pontoAtualRecorte =
                    New PointF(
                        LimitarSingle(
                            pontoMundo.X,
                            campoRecorte.Left,
                            campoRecorte.Right),
                        LimitarSingle(
                            pontoMundo.Y,
                            campoRecorte.Top,
                            campoRecorte.Bottom))

                Invalidate()

            End If

            Exit Sub

        End If

        If _panEmAndamento Then

            Dim deltaX As Single =
                e.X -
                _pontoInicialPan.X

            Dim deltaY As Single =
                e.Y -
                _pontoInicialPan.Y

            _deslocamentoVisual =
                New PointF(
                    _deslocamentoInicialPan.X +
                    deltaX,
                    _deslocamentoInicialPan.Y +
                    deltaY)

            LimitarDeslocamentoVisual()

            Cursor =
                Cursors.SizeAll

            Invalidate()

            Exit Sub

        End If

        e =
            CriarEventoMouseMundo(
                e)

        Dim campo As RectangleF =
            ObterRetanguloCampo()

        If FerramentaAtual <>
           FerramentaCampo.Selecionar Then

            Cursor =
                Cursors.Cross

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

                MoverGrupoSelecionado(
                    e.Location,
                    campo)

            ElseIf _modoManipulacao =
                   ModoManipulacaoCampo.MoverObjeto Then

                MoverObjetoSelecionado(
                    e.Location,
                    campo)

            Else

                RedimensionarObjetoSelecionado(
                    e.Location,
                    campo)

            End If

            _houveAlteracaoManipulacao =
                True

            Invalidate()

            Exit Sub

        End If

        Dim manipulador As ModoManipulacaoCampo =
            LocalizarManipuladorSelecionado(
                e.Location,
                campo)

        If manipulador <>
           ModoManipulacaoCampo.Nenhum Then

            Cursor =
                ObterCursorManipulador(
                    manipulador)

            Exit Sub

        End If

        Dim objetoSobMouse As ObjetoCampo =
            LocalizarObjetoNaPosicao(
                e.Location,
                campo)

        If objetoSobMouse IsNot Nothing Then

            Cursor =
                Cursors.Hand

        Else

            Cursor =
                Cursors.Default

        End If

    End Sub

    Protected Overrides Sub OnMouseUp(e As MouseEventArgs)

        MyBase.OnMouseUp(e)

        If _modoSelecaoRecorte Then

            If _desenhandoRecorte AndAlso
               e.Button = MouseButtons.Left Then

                Dim pontoMundo As PointF =
                    ConverterTelaParaMundo(
                        e.Location)

                Dim campoRecorte As RectangleF =
                    ObterRetanguloCampo()

                _pontoAtualRecorte =
                    New PointF(
                        LimitarSingle(
                            pontoMundo.X,
                            campoRecorte.Left,
                            campoRecorte.Right),
                        LimitarSingle(
                            pontoMundo.Y,
                            campoRecorte.Top,
                            campoRecorte.Bottom))

                _desenhandoRecorte =
                    False

                Capture =
                    False

                Dim retanguloRecorte As RectangleF =
                    ObterRetanguloRecorteLivre(
                        _pontoInicialRecorte,
                        _pontoAtualRecorte,
                        campoRecorte)

                If retanguloRecorte.Width >=
                   campoRecorte.Width * 0.02F AndAlso
                   retanguloRecorte.Height >=
                   campoRecorte.Height * 0.02F Then

                    ConcluirSelecaoRecorteCampo(
                        retanguloRecorte,
                        campoRecorte)

                Else

                    Cursor =
                        Cursors.Cross

                    Invalidate()

                End If

            End If

            Exit Sub

        End If


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

        If _objetoSelecionado Is Nothing OrElse
   _objetoSelecionado.Bloqueado Then

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
                linha.Espessura *
                NormalizarEscalaVisual(
                    linha.EscalaVisual) +
                7.0F

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
                gol.Orientacao,
                NormalizarEscalaVisual(
                    gol.EscalaVisual))

            area.Inflate(
            7.0F,
            7.0F)

            Return area.Contains(
            localMouse.X,
            localMouse.Y)

        End If

        If TypeOf objeto Is Manequim Then

            Dim manequim As Manequim =
                DirectCast(
                    objeto,
                    Manequim)

            Dim area As RectangleF =
                ObterRetanguloManequim(
                    centro,
                    NormalizarEscalaVisual(
                        manequim.EscalaVisual))

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

        If Not PodeManipularSelecao() Then
            Exit Sub
        End If

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

        If e.KeyCode = Keys.Escape AndAlso
           _modoSelecaoRecorte Then

            CancelarSelecaoRecorteCampo()

            e.Handled =
                True

            e.SuppressKeyPress =
                True

            Exit Sub

        End If

        If e.KeyCode = Keys.Delete Then

            ExcluirSelecionado()

            e.Handled =
                True

            e.SuppressKeyPress =
                True

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

            e.Handled =
                True

            e.SuppressKeyPress =
                True

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

        If _objetoSelecionado Is Nothing OrElse _objetoSelecionado.Bloqueado Then

            Exit Sub

        End If

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

        If _objetoSelecionado IsNot Nothing AndAlso
   _objetoSelecionado.Bloqueado Then

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

        If _modoSelecaoRecorte Then
            Exit Sub
        End If

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

        Dim objetosRelacionados As List(Of ObjetoCampo) =
        ObterObjetosRelacionadosSelecao(
            objeto)

        For Each item As ObjetoCampo
        In objetosRelacionados

            item.Selecionado =
            True

            _objetosSelecionados.Add(
            item)

        Next

        _objetoSelecionado =
        objeto

    End Sub

    Private Sub AlternarSelecaoObjeto(
    objeto As ObjetoCampo)

        If objeto Is Nothing Then
            Exit Sub
        End If

        Dim objetosRelacionados As List(Of ObjetoCampo) =
        ObterObjetosRelacionadosSelecao(
            objeto)

        Dim todosSelecionados As Boolean =
        True

        For Each item As ObjetoCampo
        In objetosRelacionados

            If Not _objetosSelecionados.Contains(
            item) Then

                todosSelecionados =
                False

                Exit For

            End If

        Next

        If todosSelecionados Then

            For Each item As ObjetoCampo
            In objetosRelacionados

                item.Selecionado =
                False

                _objetosSelecionados.Remove(
                item)

            Next

            If _objetosSelecionados.Count > 0 Then

                _objetoSelecionado =
                _objetosSelecionados(
                    _objetosSelecionados.Count - 1)

            Else

                _objetoSelecionado =
                Nothing

            End If

        Else

            For Each item As ObjetoCampo
            In objetosRelacionados

                If Not _objetosSelecionados.Contains(
                item) Then

                    item.Selecionado =
                    True

                    _objetosSelecionados.Add(
                    item)

                End If

            Next

            _objetoSelecionado =
            objeto

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

        If ExisteObjetoBloqueadoNaSelecao() Then
            Exit Sub
        End If

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

        If Not PodeManipularSelecao() Then
            Exit Sub
        End If

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

        If Not PodeManipularSelecao() Then
            Exit Sub
        End If

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

        If Not PodeManipularSelecao() Then
            Exit Sub
        End If

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

        If Not PodeManipularSelecao() Then
            Exit Sub
        End If

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

        If Not PodeManipularSelecao() Then
            Exit Sub
        End If

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

        If objeto Is Nothing Then

            Return New SizeF(
                15.0F,
                15.0F)

        End If

        If TypeOf objeto Is Jogador Then

            Return RenderizadorSpritesTaticos.
                ObterMetadeTamanhoJogador(
                    DirectCast(
                        objeto,
                        Jogador))

        End If

        Dim escala As Single =
            NormalizarEscalaVisual(
                objeto.EscalaVisual)

        If TypeOf objeto Is Bola Then

            Return New SizeF(
                RaioBola * escala,
                RaioBola * escala)

        End If

        If TypeOf objeto Is Cone Then

            Return New SizeF(
                RaioCone * escala,
                RaioCone * escala)

        End If

        If TypeOf objeto Is Gol Then

            Dim gol As Gol =
                DirectCast(
                    objeto,
                    Gol)

            If gol.Orientacao =
               OrientacaoGol.Esquerda OrElse
               gol.Orientacao =
               OrientacaoGol.Direita Then

                Return New SizeF(
                    ProfundidadeGol / 2.0F * escala,
                    LarguraBocaGol / 2.0F * escala)

            End If

            Return New SizeF(
                LarguraBocaGol / 2.0F * escala,
                ProfundidadeGol / 2.0F * escala)

        End If

        If TypeOf objeto Is Manequim Then

            Return New SizeF(
                LarguraManequim / 2.0F * escala,
                AlturaManequim / 2.0F * escala)

        End If

        If TypeOf objeto Is MarcadorTatico Then

            Dim marcador As MarcadorTatico =
                DirectCast(
                    objeto,
                    MarcadorTatico)

            Dim raio As Single =
                marcador.Diametro / 2.0F * escala

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
                ObterTamanhoTextoTatico(
                    texto)

            Return New SizeF(
                tamanho.Width / 2.0F,
                tamanho.Height / 2.0F)

        End If

        Return New SizeF(
            15.0F * escala,
            15.0F * escala)

    End Function

    Private Function ObterRaioSelecao(
        objeto As ObjetoCampo) As Single

        If objeto Is Nothing Then
            Return 20.0F
        End If

        If TypeOf objeto Is Jogador Then

            Return RenderizadorSpritesTaticos.
                ObterRaioSelecaoJogador(
                    DirectCast(
                        objeto,
                        Jogador))

        End If

        Dim escala As Single =
            NormalizarEscalaVisual(
                objeto.EscalaVisual)

        If TypeOf objeto Is Bola Then
            Return RaioBola * escala + 6.0F
        End If

        If TypeOf objeto Is Cone Then
            Return RaioCone * escala + 5.0F
        End If

        If TypeOf objeto Is MarcadorTatico Then

            Dim marcador As MarcadorTatico =
                DirectCast(
                    objeto,
                    MarcadorTatico)

            Return marcador.Diametro / 2.0F * escala + 5.0F

        End If

        Return 20.0F * escala

    End Function

    Private Function ObterRetanguloGol(
        centro As PointF,
        orientacao As OrientacaoGol,
        Optional escala As Single = 1.0F
    ) As RectangleF

        escala =
            NormalizarEscalaVisual(
                escala)

        Dim larguraBoca As Single =
            LarguraBocaGol * escala

        Dim profundidade As Single =
            ProfundidadeGol * escala

        If orientacao =
           OrientacaoGol.Esquerda OrElse
           orientacao =
           OrientacaoGol.Direita Then

            Return New RectangleF(
                centro.X - profundidade / 2.0F,
                centro.Y - larguraBoca / 2.0F,
                profundidade,
                larguraBoca)

        End If

        Return New RectangleF(
            centro.X - larguraBoca / 2.0F,
            centro.Y - profundidade / 2.0F,
            larguraBoca,
            profundidade)

    End Function

    Private Function ObterRetanguloManequim(
        centro As PointF,
        Optional escala As Single = 1.0F
    ) As RectangleF

        escala =
            NormalizarEscalaVisual(
                escala)

        Dim largura As Single =
            LarguraManequim * escala

        Dim altura As Single =
            AlturaManequim * escala

        Return New RectangleF(
            centro.X - largura / 2.0F,
            centro.Y - altura / 2.0F,
            largura,
            altura)

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

        Dim origemVisual As RectangleF =
            ObterRetanguloOrigemRecorte(
                campoBase)

        If origemVisual.Width <= 0 OrElse
           origemVisual.Height <= 0 Then

            origemVisual =
                campoBase

        End If

        Dim escalaRecorteX As Single =
            campoBase.Width /
            origemVisual.Width

        Dim escalaRecorteY As Single =
            campoBase.Height /
            origemVisual.Height

        Dim deslocamentoRecorteX As Single =
            campoBase.Left -
            origemVisual.Left *
            escalaRecorteX

        Dim deslocamentoRecorteY As Single =
            campoBase.Top -
            origemVisual.Top *
            escalaRecorteY

        Dim centroX As Single =
            campoBase.Left +
            campoBase.Width / 2.0F

        Dim centroY As Single =
            campoBase.Top +
            campoBase.Height / 2.0F

        Dim deslocamentoZoomX As Single =
            centroX +
            _deslocamentoVisual.X -
            _zoomVisual *
            centroX

        Dim deslocamentoZoomY As Single =
            centroY +
            _deslocamentoVisual.Y -
            _zoomVisual *
            centroY

        Dim escalaFinalX As Single =
            escalaRecorteX *
            _zoomVisual

        Dim escalaFinalY As Single =
            escalaRecorteY *
            _zoomVisual

        Dim deslocamentoFinalX As Single =
            deslocamentoRecorteX *
            _zoomVisual +
            deslocamentoZoomX

        Dim deslocamentoFinalY As Single =
            deslocamentoRecorteY *
            _zoomVisual +
            deslocamentoZoomY

        Return New Matrix(
            escalaFinalX,
            0.0F,
            0.0F,
            escalaFinalY,
            deslocamentoFinalX,
            deslocamentoFinalY)

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

#Region "Agrupamento e bloqueio"

    Private Function ObterObjetosRelacionadosSelecao(
    objeto As ObjetoCampo) As List(Of ObjetoCampo)

        Dim resultado As New List(Of ObjetoCampo)()

        If objeto Is Nothing Then
            Return resultado
        End If

        If String.IsNullOrWhiteSpace(
            objeto.GrupoId) Then

            resultado.Add(
                objeto)

            Return resultado

        End If

        For Each item As ObjetoCampo In _objetos

            If String.Equals(
                item.GrupoId,
                objeto.GrupoId,
                StringComparison.Ordinal) Then

                resultado.Add(
                    item)

            End If

        Next

        Return resultado

    End Function

    Private Function PodeManipularSelecao() As Boolean

        If _objetosSelecionados.Count = 0 Then
            Return False
        End If

        If ExisteObjetoBloqueadoNaSelecao() Then

            AvisarSelecaoBloqueada()

            Return False

        End If

        Return True

    End Function

    Public Sub AgruparSelecionados()

        If _objetosSelecionados.Count < 2 Then
            Exit Sub
        End If

        Dim novoGrupoId As String =
            Guid.NewGuid().
            ToString("N")

        Dim houveAlteracao As Boolean =
            False

        For Each objeto As ObjetoCampo
            In _objetosSelecionados

            If objeto.GrupoId <> novoGrupoId Then

                objeto.GrupoId =
                    novoGrupoId

                houveAlteracao =
                    True

            End If

        Next

        If Not houveAlteracao Then
            Exit Sub
        End If

        RegistrarEstadoHistorico()

        NotificarSelecaoAlterada()

        Invalidate()

    End Sub

    Public Sub DesagruparSelecionados()

        If _objetosSelecionados.Count = 0 Then
            Exit Sub
        End If

        Dim houveAlteracao As Boolean =
            False

        For Each objeto As ObjetoCampo
            In _objetosSelecionados

            If Not String.IsNullOrWhiteSpace(
                objeto.GrupoId) Then

                objeto.GrupoId =
                    String.Empty

                houveAlteracao =
                    True

            End If

        Next

        If Not houveAlteracao Then
            Exit Sub
        End If

        RegistrarEstadoHistorico()

        NotificarSelecaoAlterada()

        Invalidate()

    End Sub

    Public Sub AlternarBloqueioSelecionados()

        If _objetosSelecionados.Count = 0 Then
            Exit Sub
        End If

        Dim todosBloqueados As Boolean =
            True

        For Each objeto As ObjetoCampo
            In _objetosSelecionados

            If Not objeto.Bloqueado Then

                todosBloqueados =
                    False

                Exit For

            End If

        Next

        Dim novoEstado As Boolean =
            Not todosBloqueados

        For Each objeto As ObjetoCampo
            In _objetosSelecionados

            objeto.Bloqueado =
                novoEstado

        Next

        RegistrarEstadoHistorico()

        NotificarSelecaoAlterada()

        Invalidate()

    End Sub

    Private Function ExisteObjetoBloqueadoNaSelecao() As Boolean

        For Each objeto As ObjetoCampo
            In _objetosSelecionados

            If objeto.Bloqueado Then
                Return True
            End If

        Next

        Return False

    End Function

#End Region

#Region "Lista de objetos e camadas"

    Public Sub SelecionarObjetosPelaLista(
    objetos As IEnumerable(Of ObjetoCampo))

        DeselecionarTodos()

        If objetos Is Nothing Then

            NotificarSelecaoAlterada()

            Exit Sub

        End If

        Dim objetosAdicionar As New List(Of ObjetoCampo)()

        For Each objeto As ObjetoCampo In objetos

            If objeto Is Nothing OrElse
               Not _objetos.Contains(objeto) Then

                Continue For

            End If

            Dim relacionados As List(Of ObjetoCampo) =
                ObterObjetosRelacionadosSelecao(objeto)

            For Each relacionado As ObjetoCampo In relacionados

                If Not objetosAdicionar.Contains(relacionado) Then

                    objetosAdicionar.Add(relacionado)

                End If

            Next

            _objetoSelecionado =
                objeto

        Next

        For Each objeto As ObjetoCampo In objetosAdicionar

            objeto.Selecionado =
                True

            _objetosSelecionados.Add(
                objeto)

        Next

        If _objetoSelecionado Is Nothing AndAlso
           _objetosSelecionados.Count > 0 Then

            _objetoSelecionado =
                _objetosSelecionados(
                    _objetosSelecionados.Count - 1)

        End If

        NotificarSelecaoAlterada()

    End Sub

    Public Sub DefinirVisibilidadeObjetos(
    objetos As IEnumerable(Of ObjetoCampo),
    visivel As Boolean)

        If objetos Is Nothing Then
            Exit Sub
        End If

        Dim houveAlteracao As Boolean =
            False

        Dim processados As New List(Of ObjetoCampo)()

        For Each objeto As ObjetoCampo In objetos

            If objeto Is Nothing OrElse
               Not _objetos.Contains(objeto) OrElse
               processados.Contains(objeto) Then

                Continue For

            End If

            processados.Add(
                objeto)

            If objeto.Visivel <> visivel Then

                objeto.Visivel =
                    visivel

                houveAlteracao =
                    True

            End If

        Next

        If Not houveAlteracao Then
            Exit Sub
        End If

        RegistrarEstadoHistorico()
        NotificarSelecaoAlterada()
        Invalidate()

    End Sub

    Public Sub DefinirBloqueioObjetos(
    objetos As IEnumerable(Of ObjetoCampo),
    bloqueado As Boolean)

        If objetos Is Nothing Then
            Exit Sub
        End If

        Dim houveAlteracao As Boolean =
            False

        Dim processados As New List(Of ObjetoCampo)()

        For Each objeto As ObjetoCampo In objetos

            If objeto Is Nothing OrElse
               Not _objetos.Contains(objeto) OrElse
               processados.Contains(objeto) Then

                Continue For

            End If

            processados.Add(
                objeto)

            If objeto.Bloqueado <> bloqueado Then

                objeto.Bloqueado =
                    bloqueado

                houveAlteracao =
                    True

            End If

        Next

        If Not houveAlteracao Then
            Exit Sub
        End If

        RegistrarEstadoHistorico()
        NotificarSelecaoAlterada()
        Invalidate()

    End Sub

    Private Function ObterIndicesCamadaVisual(
    camada As Integer) As List(Of Integer)

        Dim indices As New List(Of Integer)()

        For indice As Integer =
            0 To _objetos.Count - 1

            If ObterCamadaVisual(
                _objetos(indice)) = camada Then

                indices.Add(
                    indice)

            End If

        Next

        Return indices

    End Function

    Private Sub TrocarObjetosDePosicao(
    indiceA As Integer,
    indiceB As Integer)

        If indiceA = indiceB Then
            Exit Sub
        End If

        Dim temporario As ObjetoCampo =
            _objetos(indiceA)

        _objetos(indiceA) =
            _objetos(indiceB)

        _objetos(indiceB) =
            temporario

    End Sub

    Private Sub FinalizarAlteracaoCamada(
    houveAlteracao As Boolean)

        If Not houveAlteracao Then
            Exit Sub
        End If

        RegistrarEstadoHistorico()
        NotificarSelecaoAlterada()
        Invalidate()

    End Sub

    Public Sub SubirCamadaSelecionados()

        If Not PodeManipularSelecao() Then
            Exit Sub
        End If

        Dim houveAlteracao As Boolean =
            False

        For camada As Integer =
            0 To 3

            Dim indices As List(Of Integer) =
                ObterIndicesCamadaVisual(camada)

            For posicao As Integer =
                indices.Count - 2 To 0 Step -1

                Dim indiceAtual As Integer =
                    indices(posicao)

                Dim indiceFrente As Integer =
                    indices(posicao + 1)

                Dim objetoAtual As ObjetoCampo =
                    _objetos(indiceAtual)

                Dim objetoFrente As ObjetoCampo =
                    _objetos(indiceFrente)

                If _objetosSelecionados.Contains(objetoAtual) AndAlso
                   Not _objetosSelecionados.Contains(objetoFrente) Then

                    TrocarObjetosDePosicao(
                        indiceAtual,
                        indiceFrente)

                    houveAlteracao =
                        True

                End If

            Next

        Next

        FinalizarAlteracaoCamada(
            houveAlteracao)

    End Sub

    Public Sub DescerCamadaSelecionados()

        If Not PodeManipularSelecao() Then
            Exit Sub
        End If

        Dim houveAlteracao As Boolean =
            False

        For camada As Integer =
            0 To 3

            Dim indices As List(Of Integer) =
                ObterIndicesCamadaVisual(camada)

            For posicao As Integer =
                1 To indices.Count - 1

                Dim indiceAtual As Integer =
                    indices(posicao)

                Dim indiceTras As Integer =
                    indices(posicao - 1)

                Dim objetoAtual As ObjetoCampo =
                    _objetos(indiceAtual)

                Dim objetoTras As ObjetoCampo =
                    _objetos(indiceTras)

                If _objetosSelecionados.Contains(objetoAtual) AndAlso
                   Not _objetosSelecionados.Contains(objetoTras) Then

                    TrocarObjetosDePosicao(
                        indiceAtual,
                        indiceTras)

                    houveAlteracao =
                        True

                End If

            Next

        Next

        FinalizarAlteracaoCamada(
            houveAlteracao)

    End Sub

    Public Sub TrazerSelecionadosParaFrente()

        If Not PodeManipularSelecao() Then
            Exit Sub
        End If

        Dim houveAlteracao As Boolean =
            False

        For camada As Integer =
            0 To 3

            Dim indices As List(Of Integer) =
                ObterIndicesCamadaVisual(camada)

            If indices.Count < 2 Then
                Continue For
            End If

            Dim ordemAtual As New List(Of ObjetoCampo)()
            Dim novaOrdem As New List(Of ObjetoCampo)()

            For Each indice As Integer In indices
                ordemAtual.Add(_objetos(indice))
            Next

            For Each objeto As ObjetoCampo In ordemAtual

                If Not _objetosSelecionados.Contains(objeto) Then
                    novaOrdem.Add(objeto)
                End If

            Next

            For Each objeto As ObjetoCampo In ordemAtual

                If _objetosSelecionados.Contains(objeto) Then
                    novaOrdem.Add(objeto)
                End If

            Next

            For posicao As Integer =
                0 To indices.Count - 1

                If Not ReferenceEquals(
                    _objetos(indices(posicao)),
                    novaOrdem(posicao)) Then

                    houveAlteracao =
                        True

                End If

                _objetos(indices(posicao)) =
                    novaOrdem(posicao)

            Next

        Next

        FinalizarAlteracaoCamada(
            houveAlteracao)

    End Sub

    Public Sub EnviarSelecionadosParaTras()

        If Not PodeManipularSelecao() Then
            Exit Sub
        End If

        Dim houveAlteracao As Boolean =
            False

        For camada As Integer =
            0 To 3

            Dim indices As List(Of Integer) =
                ObterIndicesCamadaVisual(camada)

            If indices.Count < 2 Then
                Continue For
            End If

            Dim ordemAtual As New List(Of ObjetoCampo)()
            Dim novaOrdem As New List(Of ObjetoCampo)()

            For Each indice As Integer In indices
                ordemAtual.Add(_objetos(indice))
            Next

            For Each objeto As ObjetoCampo In ordemAtual

                If _objetosSelecionados.Contains(objeto) Then
                    novaOrdem.Add(objeto)
                End If

            Next

            For Each objeto As ObjetoCampo In ordemAtual

                If Not _objetosSelecionados.Contains(objeto) Then
                    novaOrdem.Add(objeto)
                End If

            Next

            For posicao As Integer =
                0 To indices.Count - 1

                If Not ReferenceEquals(
                    _objetos(indices(posicao)),
                    novaOrdem(posicao)) Then

                    houveAlteracao =
                        True

                End If

                _objetos(indices(posicao)) =
                    novaOrdem(posicao)

            Next

        Next

        FinalizarAlteracaoCamada(
            houveAlteracao)

    End Sub

    Public Function RenomearObjeto(objeto As ObjetoCampo, novoNome As String) As Boolean

        If objeto Is Nothing OrElse
       Not _objetos.Contains(
           objeto) Then

            Return False

        End If

        If objeto.Bloqueado Then

            MessageBox.Show(
            "O objeto está bloqueado." &
            Environment.NewLine &
            Environment.NewLine &
            "Desbloqueie o objeto antes de renomeá-lo.",
            "Objeto bloqueado",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information)

            Return False

        End If

        Dim nomeNormalizado As String =
        If(
            novoNome,
            String.Empty).
        Trim()

        If nomeNormalizado.Length > 80 Then

            nomeNormalizado =
            nomeNormalizado.Substring(
                0,
                80)

        End If

        Dim nomeAtual As String =
        If(
            objeto.NomePersonalizado,
            String.Empty)

        If String.Equals(
        nomeAtual,
        nomeNormalizado,
        StringComparison.CurrentCulture) Then

            Return False

        End If

        objeto.NomePersonalizado =
        nomeNormalizado

        RegistrarEstadoHistorico()

        NotificarSelecaoAlterada()

        Invalidate()

        Return True

    End Function

    Public Function AlterarEscalaVisualSelecionados(
    novaEscala As Single) As Boolean

        If _objetosSelecionados.Count = 0 Then
            Return False
        End If

        If Single.IsNaN(novaEscala) OrElse
       Single.IsInfinity(novaEscala) Then

            Return False

        End If

        Dim escalaNormalizada As Single =
        Math.Max(
            0.5F,
            Math.Min(
                2.5F,
                novaEscala))

        Dim houveAlteracao As Boolean =
        False

        For Each objeto As ObjetoCampo
        In _objetosSelecionados

            If objeto Is Nothing OrElse
           objeto.Bloqueado Then

                Continue For

            End If

            If Math.Abs(
            objeto.EscalaVisual -
            escalaNormalizada) < 0.001F Then

                Continue For

            End If

            objeto.EscalaVisual =
            escalaNormalizada

            houveAlteracao =
            True

        Next

        If Not houveAlteracao Then
            Return False
        End If

        RegistrarEstadoHistorico()

        RaiseEvent ObjetosAlterados()

        Invalidate()

        Return True

    End Function

    Public Function AlterarDirecaoJogadoresSelecionados(
    novaDirecao As DirecaoJogador) As Boolean

        If _objetosSelecionados.Count = 0 Then
            Return False
        End If

        Dim houveAlteracao As Boolean =
        False

        For Each objeto As ObjetoCampo
        In _objetosSelecionados

            If objeto Is Nothing OrElse
           objeto.Bloqueado OrElse
           Not TypeOf objeto Is Jogador Then

                Continue For

            End If

            Dim jogador As Jogador =
            DirectCast(
                objeto,
                Jogador)

            If jogador.Direcao =
           novaDirecao Then

                Continue For

            End If

            jogador.Direcao =
            novaDirecao

            houveAlteracao =
            True

        Next

        If Not houveAlteracao Then
            Return False
        End If

        RegistrarEstadoHistorico()

        RaiseEvent ObjetosAlterados()

        Invalidate()

        Return True

    End Function

    Public Function AlterarOrientacaoVisualJogadoresSelecionados(
    novaOrientacao As OrientacaoVisualJogador) As Boolean

        If _objetosSelecionados.Count = 0 Then
            Return False
        End If

        Dim houveAlteracao As Boolean =
            False

        For Each objeto As ObjetoCampo
        In _objetosSelecionados

            If objeto Is Nothing OrElse
               objeto.Bloqueado OrElse
               Not TypeOf objeto Is Jogador Then

                Continue For

            End If

            Dim jogador As Jogador =
                DirectCast(
                    objeto,
                    Jogador)

            If jogador.OrientacaoVisual =
               novaOrientacao Then

                Continue For

            End If

            jogador.OrientacaoVisual =
                novaOrientacao

            houveAlteracao =
                True

        Next

        If Not houveAlteracao Then
            Return False
        End If

        RegistrarEstadoHistorico()

        RaiseEvent ObjetosAlterados()

        Invalidate()

        Return True

    End Function

    Public Function AlterarOrientacaoVisualTodosJogadores(
        novaOrientacao As OrientacaoVisualJogador
    ) As Boolean

        Dim houveAlteracao As Boolean =
            False

        For Each objeto As ObjetoCampo
            In _objetos

            If objeto Is Nothing OrElse
               objeto.Bloqueado OrElse
               Not TypeOf objeto Is Jogador Then

                Continue For

            End If

            Dim jogador As Jogador =
                DirectCast(
                    objeto,
                    Jogador)

            If jogador.OrientacaoVisual =
               novaOrientacao Then

                Continue For

            End If

            jogador.OrientacaoVisual =
                novaOrientacao

            houveAlteracao =
                True

        Next

        If Not houveAlteracao Then
            Return False
        End If

        RegistrarEstadoHistorico()

        RaiseEvent ObjetosAlterados()

        Invalidate()

        Return True

    End Function

    Public Function AlterarCorCamisaJogadoresSelecionados(
    novaCor As Color
) As Boolean

        If _objetosSelecionados.Count = 0 Then
            Return False
        End If

        Dim corNormalizada As Color =
        Color.FromArgb(
            255,
            novaCor.R,
            novaCor.G,
            novaCor.B)

        Dim novoArgb As Integer =
        corNormalizada.ToArgb()

        Dim houveAlteracao As Boolean =
        False

        For Each objeto As ObjetoCampo
        In _objetosSelecionados

            If objeto Is Nothing OrElse
           objeto.Bloqueado OrElse
           Not TypeOf objeto Is Jogador Then

                Continue For

            End If

            Dim jogador As Jogador =
            DirectCast(
                objeto,
                Jogador)

            If jogador.CorCamisaArgb =
           novoArgb Then

                Continue For

            End If

            jogador.CorCamisaArgb =
            novoArgb

            houveAlteracao =
            True

        Next

        If Not houveAlteracao Then
            Return False
        End If

        RegistrarEstadoHistorico()

        RaiseEvent ObjetosAlterados()

        Invalidate()

        Return True

    End Function
    Public Function AlterarPoseJogadoresSelecionados(
    novaPose As PoseJogador) As Boolean

        If _objetosSelecionados.Count = 0 Then
            Return False
        End If

        Dim houveAlteracao As Boolean =
        False

        For Each objeto As ObjetoCampo
        In _objetosSelecionados

            If objeto Is Nothing OrElse
           objeto.Bloqueado OrElse
           Not TypeOf objeto Is Jogador Then

                Continue For

            End If

            Dim jogador As Jogador =
            DirectCast(
                objeto,
                Jogador)

            If jogador.Pose =
           novaPose Then

                Continue For

            End If

            jogador.Pose =
            novaPose

            houveAlteracao =
            True

        Next

        If Not houveAlteracao Then
            Return False
        End If

        RegistrarEstadoHistorico()

        RaiseEvent ObjetosAlterados()

        Invalidate()

        Return True

    End Function

#End Region

#Region "Recorte do campo"

    Public Sub IniciarSelecaoRecorteCampo()

        _recorteAnteriorAtivo =
            _recorteAtivo

        _retanguloRecorteAnterior =
            _retanguloRecortePercentual

        _modoSelecaoRecorte =
            True

        _desenhandoRecorte =
            False

        _recorteAtivo =
            False

        _retanguloRecortePercentual =
            RectangleF.Empty

        _zoomVisual =
            1.0F

        _deslocamentoVisual =
            PointF.Empty

        FerramentaAtual =
            FerramentaCampo.Selecionar

        DeselecionarTodos()

        NotificarSelecaoAlterada()

        Cursor =
            Cursors.Cross

        RaiseEvent VisualizacaoAlterada(
            ZoomPercentual)

        RaiseEvent RecorteCampoAlterado()

        Invalidate()

    End Sub

    Public Sub CancelarSelecaoRecorteCampo()

        If Not _modoSelecaoRecorte Then
            Exit Sub
        End If

        _modoSelecaoRecorte =
            False

        _desenhandoRecorte =
            False

        _recorteAtivo =
            _recorteAnteriorAtivo

        _retanguloRecortePercentual =
            _retanguloRecorteAnterior

        _recorteAnteriorAtivo =
            False

        _retanguloRecorteAnterior =
            RectangleF.Empty

        Capture =
            False

        Cursor =
            Cursors.Default

        RaiseEvent RecorteCampoAlterado()

        Invalidate()

    End Sub

    Public Sub LimparRecorteCampo()

        _modoSelecaoRecorte =
            False

        _desenhandoRecorte =
            False

        _recorteAtivo =
            False

        _retanguloRecortePercentual =
            RectangleF.Empty

        _recorteAnteriorAtivo =
            False

        _retanguloRecorteAnterior =
            RectangleF.Empty

        _zoomVisual =
            1.0F

        _deslocamentoVisual =
            PointF.Empty

        Capture =
            False

        Cursor =
            Cursors.Default

        RaiseEvent VisualizacaoAlterada(
            ZoomPercentual)

        RaiseEvent RecorteCampoAlterado()

        Invalidate()

    End Sub

    Public Function AplicarRecortePredefinido(
    tipoRecorte As TipoRecorteCampo) As Boolean

        If tipoRecorte =
       TipoRecorteCampo.CampoInteiro Then

            LimparRecorteCampo()

            Return True

        End If

        Dim novoRecorte As RectangleF

        Select Case tipoRecorte

            Case TipoRecorteCampo.MeioCampoEsquerdo

                novoRecorte =
                New RectangleF(
                    0.0F,
                    0.0F,
                    50.0F,
                    100.0F)

            Case TipoRecorteCampo.MeioCampoDireito

                novoRecorte =
                New RectangleF(
                    50.0F,
                    0.0F,
                    50.0F,
                    100.0F)

            Case TipoRecorteCampo.MetadeSuperior

                novoRecorte =
                New RectangleF(
                    0.0F,
                    0.0F,
                    100.0F,
                    50.0F)

            Case TipoRecorteCampo.MetadeInferior

                novoRecorte =
                New RectangleF(
                    0.0F,
                    50.0F,
                    100.0F,
                    50.0F)

            Case TipoRecorteCampo.TercoEsquerdo

                novoRecorte =
                New RectangleF(
                    0.0F,
                    0.0F,
                    33.333F,
                    100.0F)

            Case TipoRecorteCampo.TercoCentral

                novoRecorte =
                New RectangleF(
                    33.333F,
                    0.0F,
                    33.334F,
                    100.0F)

            Case TipoRecorteCampo.TercoDireito

                novoRecorte =
                New RectangleF(
                    66.667F,
                    0.0F,
                    33.333F,
                    100.0F)

            Case TipoRecorteCampo.AreaEsquerda

                novoRecorte =
                New RectangleF(
                    0.0F,
                    18.0F,
                    20.0F,
                    64.0F)

            Case TipoRecorteCampo.AreaDireita

                novoRecorte =
                New RectangleF(
                    80.0F,
                    18.0F,
                    20.0F,
                    64.0F)

            Case Else

                Return False

        End Select

        _modoSelecaoRecorte =
        False

        _desenhandoRecorte =
        False

        _recorteAtivo =
        True

        _retanguloRecortePercentual =
        novoRecorte

        Invalidate()

        Return True

    End Function

    Private Sub ConcluirSelecaoRecorteCampo(
    retanguloTela As RectangleF,
    campo As RectangleF)

        _retanguloRecortePercentual =
            ConverterRetanguloTelaParaPercentual(
                retanguloTela,
                campo)

        _recorteAtivo =
            Not _retanguloRecortePercentual.IsEmpty

        _modoSelecaoRecorte =
            False

        _desenhandoRecorte =
            False

        _recorteAnteriorAtivo =
            False

        _retanguloRecorteAnterior =
            RectangleF.Empty

        _zoomVisual =
            1.0F

        _deslocamentoVisual =
            PointF.Empty

        Capture =
            False

        Cursor =
            Cursors.Default

        RaiseEvent VisualizacaoAlterada(
            ZoomPercentual)

        RaiseEvent RecorteCampoAlterado()

        Invalidate()

    End Sub

    Private Function ObterRetanguloOrigemRecorte(
    campo As RectangleF) As RectangleF

        If Not _recorteAtivo OrElse
           _retanguloRecortePercentual.IsEmpty Then

            Return campo

        End If

        Dim esquerda As Single =
            campo.Left +
            campo.Width *
            (_retanguloRecortePercentual.Left / 100.0F)

        Dim topo As Single =
            campo.Top +
            campo.Height *
            (_retanguloRecortePercentual.Top / 100.0F)

        Dim direita As Single =
            campo.Left +
            campo.Width *
            (_retanguloRecortePercentual.Right / 100.0F)

        Dim inferior As Single =
            campo.Top +
            campo.Height *
            (_retanguloRecortePercentual.Bottom / 100.0F)

        Dim resultado As RectangleF =
            RectangleF.FromLTRB(
                esquerda,
                topo,
                direita,
                inferior)

        If resultado.Width <= 0 OrElse
           resultado.Height <= 0 Then

            Return campo

        End If

        Return resultado

    End Function

    Private Function CriarMatrizMapeamento(
    origem As RectangleF,
    destino As RectangleF) As Matrix

        If origem.Width <= 0 OrElse
           origem.Height <= 0 Then

            Return New Matrix()

        End If

        Dim escalaX As Single =
            destino.Width /
            origem.Width

        Dim escalaY As Single =
            destino.Height /
            origem.Height

        Dim deslocamentoX As Single =
            destino.Left -
            origem.Left *
            escalaX

        Dim deslocamentoY As Single =
            destino.Top -
            origem.Top *
            escalaY

        Return New Matrix(
            escalaX,
            0.0F,
            0.0F,
            escalaY,
            deslocamentoX,
            deslocamentoY)

    End Function

    Private Function ObterRetanguloRecorteLivre(
    pontoInicial As PointF,
    pontoAtual As PointF,
    campo As RectangleF) As RectangleF

        If campo.Width <= 0 OrElse
           campo.Height <= 0 Then

            Return RectangleF.Empty

        End If

        Dim xInicial As Single =
            LimitarSingle(
                pontoInicial.X,
                campo.Left,
                campo.Right)

        Dim yInicial As Single =
            LimitarSingle(
                pontoInicial.Y,
                campo.Top,
                campo.Bottom)

        Dim xFinal As Single =
            LimitarSingle(
                pontoAtual.X,
                campo.Left,
                campo.Right)

        Dim yFinal As Single =
            LimitarSingle(
                pontoAtual.Y,
                campo.Top,
                campo.Bottom)

        Dim esquerda As Single =
            Math.Min(
                xInicial,
                xFinal)

        Dim topo As Single =
            Math.Min(
                yInicial,
                yFinal)

        Dim direita As Single =
            Math.Max(
                xInicial,
                xFinal)

        Dim inferior As Single =
            Math.Max(
                yInicial,
                yFinal)

        Return RectangleF.FromLTRB(
            esquerda,
            topo,
            direita,
            inferior)

    End Function

    Private Function ConverterRetanguloTelaParaPercentual(
    retanguloTela As RectangleF,
    campo As RectangleF) As RectangleF

        Dim esquerda As Single =
            CSng(
                ((retanguloTela.Left -
                  campo.Left) /
                 campo.Width) *
                100.0F)

        Dim topo As Single =
            CSng(
                ((retanguloTela.Top -
                  campo.Top) /
                 campo.Height) *
                100.0F)

        Dim direita As Single =
            CSng(
                ((retanguloTela.Right -
                  campo.Left) /
                 campo.Width) *
                100.0F)

        Dim inferior As Single =
            CSng(
                ((retanguloTela.Bottom -
                  campo.Top) /
                 campo.Height) *
                100.0F)

        esquerda =
            Math.Max(
                0.0F,
                Math.Min(
                    100.0F,
                    esquerda))

        topo =
            Math.Max(
                0.0F,
                Math.Min(
                    100.0F,
                    topo))

        direita =
            Math.Max(
                0.0F,
                Math.Min(
                    100.0F,
                    direita))

        inferior =
            Math.Max(
                0.0F,
                Math.Min(
                    100.0F,
                    inferior))

        Return RectangleF.FromLTRB(
            esquerda,
            topo,
            direita,
            inferior)

    End Function

    Private Sub DesenharPreVisualizacaoRecorte(
    g As Graphics,
    campo As RectangleF)

        If Not _modoSelecaoRecorte OrElse
           Not _desenhandoRecorte Then

            Exit Sub

        End If

        Dim retangulo As RectangleF =
            ObterRetanguloRecorteLivre(
                _pontoInicialRecorte,
                _pontoAtualRecorte,
                campo)

        If retangulo.Width < 1.0F OrElse
           retangulo.Height < 1.0F Then

            Exit Sub

        End If

        Using regiaoExterna As New Region(campo)

            regiaoExterna.Exclude(
                retangulo)

            Using sombra As New SolidBrush(
                Color.FromArgb(
                    115,
                    0,
                    0,
                    0))

                g.FillRegion(
                    sombra,
                    regiaoExterna)

            End Using

        End Using

        Using preenchimento As New SolidBrush(
            Color.FromArgb(
                35,
                255,
                215,
                0))

            g.FillRectangle(
                preenchimento,
                retangulo)

        End Using

        Using caneta As New Pen(
            Color.Gold,
            2.0F /
            Math.Max(
                _zoomVisual,
                0.5F))

            caneta.DashStyle =
                DashStyle.Dash

            g.DrawRectangle(
                caneta,
                retangulo.X,
                retangulo.Y,
                retangulo.Width,
                retangulo.Height)

        End Using

    End Sub

#End Region

End Class
