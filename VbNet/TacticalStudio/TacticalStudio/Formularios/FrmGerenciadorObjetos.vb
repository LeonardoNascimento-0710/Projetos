Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms
Imports TacticalStudio.Core.Classes

Public Class FrmGerenciadorObjetos
    Inherits Form

    Private ReadOnly _campo As CampoTatico

    Private ReadOnly TxtFiltro As New TextBox()

    Private ReadOnly CmbTipo As New ComboBox()

    Private ReadOnly CmbEstado As New ComboBox()

    Private ReadOnly CmbOrdenacao As New ComboBox()

    Private ReadOnly LvObjetos As New ListView()

    Private ReadOnly LblResumo As New Label()

    Private ReadOnly BtnSelecionarTipo As New Button()

    Private ReadOnly BtnRenomear As New Button()

    Private ReadOnly BtnSelecionarExibidos As New Button()

    Private _selecionandoPelaLista As Boolean

    Private _atualizandoLista As Boolean

    Private _sincronizacaoPendente As Boolean

    Public Sub New(campo As CampoTatico)

        InitializeComponent()

        If campo Is Nothing Then

            Throw New ArgumentNullException(
                NameOf(campo))

        End If

        _campo =
            campo

        Text =
            "Objetos e camadas"

        StartPosition =
            FormStartPosition.CenterParent

        FormBorderStyle =
            FormBorderStyle.SizableToolWindow

        MinimumSize =
            New Size(
                720,
                500)

        ClientSize =
            New Size(
                860,
                590)

        ShowInTaskbar =
            False

        CriarInterface()

        AddHandler _campo.ObjetosAlterados,
            AddressOf Campo_ObjetosAlterados

        AddHandler _campo.ObjetoSelecionadoAlterado,
            AddressOf Campo_ObjetoSelecionadoAlterado

        AplicarTemaAtual()

        AtualizarConteudo()

    End Sub

    Protected Overrides Sub OnShown(
        e As EventArgs)

        MyBase.OnShown(
            e)

        CentralizarNoProprietario()

    End Sub

    Private Sub CentralizarNoProprietario()

        Dim formularioProprietario As Form =
            TryCast(
                Owner,
                Form)

        If formularioProprietario Is Nothing Then

            StartPosition =
                FormStartPosition.CenterScreen

            Return

        End If

        StartPosition =
            FormStartPosition.Manual

        Dim areaTrabalho As Rectangle =
            Screen.FromControl(
                formularioProprietario).
            WorkingArea

        Dim esquerda As Integer =
            formularioProprietario.Left +
            (formularioProprietario.Width -
             Width) \ 2

        Dim topo As Integer =
            formularioProprietario.Top +
            (formularioProprietario.Height -
             Height) \ 2

        esquerda =
            Math.Max(
                areaTrabalho.Left,
                Math.Min(
                    esquerda,
                    areaTrabalho.Right -
                    Width))

        topo =
            Math.Max(
                areaTrabalho.Top,
                Math.Min(
                    topo,
                    areaTrabalho.Bottom -
                    Height))

        Location =
            New Point(
                esquerda,
                topo)

    End Sub

    Private Sub CriarInterface()

        Dim painelPrincipal As New TableLayoutPanel With {
        .Dock = DockStyle.Fill,
        .ColumnCount = 1,
        .RowCount = 4,
        .Padding = New Padding(10),
        .BackColor = Tema.Fundo
    }

        painelPrincipal.RowStyles.Add(
        New RowStyle(
            SizeType.Absolute,
            54.0F))

        painelPrincipal.RowStyles.Add(
        New RowStyle(
            SizeType.Absolute,
            32.0F))

        painelPrincipal.RowStyles.Add(
        New RowStyle(
            SizeType.Percent,
            100.0F))

        painelPrincipal.RowStyles.Add(
        New RowStyle(
            SizeType.Absolute,
            116.0F))

        Controls.Add(
        painelPrincipal)

        Dim painelFiltro As New TableLayoutPanel With {
        .Dock = DockStyle.Fill,
        .ColumnCount = 6,
        .RowCount = 1,
        .Padding = New Padding(0, 7, 0, 7),
        .Margin = New Padding(0),
        .BackColor = Tema.Painel,
        .GrowStyle = TableLayoutPanelGrowStyle.FixedSize
    }

        painelFiltro.ColumnStyles.Add(
        New ColumnStyle(
            SizeType.Absolute,
            72.0F))

        painelFiltro.ColumnStyles.Add(
        New ColumnStyle(
            SizeType.Percent,
            40.0F))

        painelFiltro.ColumnStyles.Add(
        New ColumnStyle(
            SizeType.Absolute,
            48.0F))

        painelFiltro.ColumnStyles.Add(
        New ColumnStyle(
            SizeType.Percent,
            30.0F))

        painelFiltro.ColumnStyles.Add(
        New ColumnStyle(
            SizeType.Absolute,
            65.0F))

        painelFiltro.ColumnStyles.Add(
        New ColumnStyle(
            SizeType.Percent,
            30.0F))

        painelFiltro.RowStyles.Add(
        New RowStyle(
            SizeType.Percent,
            100.0F))

        Dim labelFiltro As New Label With {
        .Text = "Localizar:",
        .Dock = DockStyle.Fill,
        .Margin = New Padding(0),
        .ForeColor = Tema.Texto,
        .BackColor = Tema.Painel,
        .TextAlign = ContentAlignment.MiddleLeft
    }

        painelFiltro.Controls.Add(
        labelFiltro,
        0,
        0)

        TxtFiltro.Dock =
        DockStyle.Fill

        TxtFiltro.Margin =
        New Padding(
            4,
            3,
            14,
            3)

        TxtFiltro.BackColor =
        Tema.CampoEntrada

        TxtFiltro.ForeColor =
        Tema.TextoCampo

        TxtFiltro.BorderStyle =
        BorderStyle.FixedSingle

        AddHandler TxtFiltro.TextChanged,
        Sub(sender, e)

            AtualizarConteudo()

        End Sub

        painelFiltro.Controls.Add(
        TxtFiltro,
        1,
        0)

        Dim labelTipo As New Label With {
        .Text = "Tipo:",
        .Dock = DockStyle.Fill,
        .Margin = New Padding(0),
        .ForeColor = Tema.Texto,
        .BackColor = Tema.Painel,
        .TextAlign = ContentAlignment.MiddleLeft
    }

        painelFiltro.Controls.Add(
        labelTipo,
        2,
        0)

        CmbTipo.Dock =
        DockStyle.Fill

        CmbTipo.Margin =
        New Padding(
            4,
            3,
            14,
            3)

        CmbTipo.DropDownStyle =
        ComboBoxStyle.DropDownList

        CmbTipo.FlatStyle =
        FlatStyle.Flat

        CmbTipo.BackColor =
        Tema.CampoEntrada

        CmbTipo.ForeColor =
        Tema.TextoCampo

        CmbTipo.Items.Clear()

        CmbTipo.Items.AddRange(
        New Object() {
            "Todos",
            "Jogador",
            "Bola",
            "Cone",
            "Gol",
            "Manequim",
            "Linha tática",
            "Área tática",
            "Marcador",
            "Texto tático"
        })

        CmbTipo.SelectedIndex =
        0

        AddHandler CmbTipo.SelectedIndexChanged,
        Sub(sender, e)

            AtualizarConteudo()

        End Sub

        painelFiltro.Controls.Add(
        CmbTipo,
        3,
        0)

        Dim labelEstado As New Label With {
        .Text = "Estado:",
        .Dock = DockStyle.Fill,
        .Margin = New Padding(0),
        .ForeColor = Tema.Texto,
        .BackColor = Tema.Painel,
        .TextAlign = ContentAlignment.MiddleLeft
    }

        painelFiltro.Controls.Add(
        labelEstado,
        4,
        0)

        CmbEstado.Dock =
        DockStyle.Fill

        CmbEstado.Margin =
        New Padding(
            4,
            3,
            0,
            3)

        CmbEstado.DropDownStyle =
        ComboBoxStyle.DropDownList

        CmbEstado.FlatStyle =
        FlatStyle.Flat

        CmbEstado.BackColor =
        Tema.CampoEntrada

        CmbEstado.ForeColor =
        Tema.TextoCampo

        CmbEstado.Items.Clear()

        CmbEstado.Items.AddRange(
        New Object() {
            "Todos",
            "Visíveis",
            "Ocultos",
            "Bloqueados",
            "Desbloqueados",
            "Agrupados",
            "Sem grupo"
        })

        CmbEstado.SelectedIndex =
        0

        AddHandler CmbEstado.SelectedIndexChanged,
        Sub(sender, e)

            AtualizarConteudo()

        End Sub

        painelFiltro.Controls.Add(
        CmbEstado,
        5,
        0)

        painelPrincipal.Controls.Add(
        painelFiltro,
        0,
        0)

        Dim painelResumo As New TableLayoutPanel With {
    .Dock = DockStyle.Fill,
    .ColumnCount = 3,
    .RowCount = 1,
    .Margin = New Padding(0),
    .Padding = New Padding(0),
    .BackColor = Tema.Fundo
}

        painelResumo.ColumnStyles.Add(
    New ColumnStyle(
        SizeType.Percent,
        100.0F))

        painelResumo.ColumnStyles.Add(
    New ColumnStyle(
        SizeType.Absolute,
        72.0F))

        painelResumo.ColumnStyles.Add(
    New ColumnStyle(
        SizeType.Absolute,
        230.0F))

        painelResumo.RowStyles.Add(
    New RowStyle(
        SizeType.Percent,
        100.0F))

        LblResumo.Dock =
    DockStyle.Fill

        LblResumo.Margin =
    New Padding(0)

        LblResumo.ForeColor =
    Tema.TextoSecundario

        LblResumo.BackColor =
    Tema.Fundo

        LblResumo.TextAlign =
    ContentAlignment.MiddleLeft

        painelResumo.Controls.Add(
    LblResumo,
    0,
    0)

        Dim labelOrdenacao As New Label With {
    .Text = "Ordenar:",
    .Dock = DockStyle.Fill,
    .Margin = New Padding(0),
    .ForeColor = Tema.Texto,
    .BackColor = Tema.Fundo,
    .TextAlign = ContentAlignment.MiddleLeft
}

        painelResumo.Controls.Add(
    labelOrdenacao,
    1,
    0)

        CmbOrdenacao.Dock =
    DockStyle.Fill

        CmbOrdenacao.Margin =
    New Padding(
        4,
        2,
        0,
        2)

        CmbOrdenacao.DropDownStyle =
    ComboBoxStyle.DropDownList

        CmbOrdenacao.FlatStyle =
    FlatStyle.Flat

        CmbOrdenacao.BackColor =
    Tema.CampoEntrada

        CmbOrdenacao.ForeColor =
    Tema.TextoCampo

        CmbOrdenacao.Items.Clear()

        CmbOrdenacao.Items.AddRange(
    New Object() {
        "Camada: frente para trás",
        "Camada: trás para frente",
        "Nome: A até Z",
        "Nome: Z até A",
        "Tipo: A até Z"
    })

        CmbOrdenacao.SelectedIndex =
    0

        AddHandler CmbOrdenacao.SelectedIndexChanged,
    Sub(sender, e)

        AtualizarConteudo()

    End Sub

        painelResumo.Controls.Add(
    CmbOrdenacao,
    2,
    0)

        painelPrincipal.Controls.Add(
    painelResumo,
    0,
    1)

        LvObjetos.Dock =
        DockStyle.Fill

        LvObjetos.Margin =
        New Padding(0)

        LvObjetos.View =
        View.Details

        LvObjetos.FullRowSelect =
        True

        LvObjetos.MultiSelect =
        True

        LvObjetos.HideSelection =
        False

        LvObjetos.GridLines =
        True

        LvObjetos.BackColor =
        Tema.CampoEntrada

        LvObjetos.ForeColor =
        Tema.TextoCampo

        LvObjetos.Columns.Clear()

        LvObjetos.Columns.Add(
        "Objeto",
        260)

        LvObjetos.Columns.Add(
        "Tipo",
        110)

        LvObjetos.Columns.Add(
        "Grupo",
        90)

        LvObjetos.Columns.Add(
        "Visível",
        70)

        LvObjetos.Columns.Add(
        "Bloqueado",
        85)

        LvObjetos.Columns.Add(
        "Ordem",
        60)

        AddHandler LvObjetos.SelectedIndexChanged,
        AddressOf LvObjetos_SelectedIndexChanged

        AddHandler LvObjetos.DoubleClick,
        AddressOf LvObjetos_DoubleClick

        painelPrincipal.Controls.Add(
        LvObjetos,
        0,
        2)

        Dim painelAcoes As New FlowLayoutPanel With {
        .Dock = DockStyle.Fill,
        .FlowDirection = FlowDirection.LeftToRight,
        .WrapContents = True,
        .AutoScroll = True,
        .Padding = New Padding(0, 8, 0, 0),
        .Margin = New Padding(0),
        .BackColor = Tema.Painel
    }

        painelAcoes.Controls.Add(
        CriarBotaoAcao(
            "Mostrar",
            82,
            AddressOf MostrarSelecionados))

        painelAcoes.Controls.Add(
        CriarBotaoAcao(
            "Ocultar",
            82,
            AddressOf OcultarSelecionados))

        painelAcoes.Controls.Add(
        CriarBotaoAcao(
            "Bloquear",
            88,
            AddressOf BloquearSelecionados))

        painelAcoes.Controls.Add(
        CriarBotaoAcao(
            "Desbloquear",
            96,
            AddressOf DesbloquearSelecionados))

        painelAcoes.Controls.Add(
        CriarBotaoAcao(
            "Subir",
            76,
            AddressOf SubirCamada))

        painelAcoes.Controls.Add(
        CriarBotaoAcao(
            "Descer",
            76,
            AddressOf DescerCamada))

        painelAcoes.Controls.Add(
        CriarBotaoAcao(
            "Frente",
            78,
            AddressOf TrazerParaFrente))

        painelAcoes.Controls.Add(
        CriarBotaoAcao(
            "Trás",
            72,
            AddressOf EnviarParaTras))

        painelAcoes.Controls.Add(
        CriarBotaoAcao(
            "Excluir",
            82,
            AddressOf ExcluirSelecionados,
            True))

        BtnRenomear.Text =
        "Renomear"

        BtnRenomear.Width =
        110

        BtnRenomear.Height =
        34

        BtnRenomear.Margin =
        New Padding(3)

        BtnRenomear.FlatStyle =
        FlatStyle.Flat

        BtnRenomear.BackColor =
        Tema.Painel

        BtnRenomear.ForeColor =
        Tema.Texto

        BtnRenomear.Cursor =
        Cursors.Hand

        BtnRenomear.UseVisualStyleBackColor =
        False

        BtnRenomear.FlatAppearance.BorderColor =
        Tema.Borda

        BtnRenomear.FlatAppearance.MouseOverBackColor =
        Tema.PainelHover

        AddHandler BtnRenomear.Click,
        AddressOf BtnRenomear_Click

        painelAcoes.Controls.Add(
        BtnRenomear)

        BtnSelecionarTipo.Text =
        "Selecionar tipo"

        BtnSelecionarTipo.Width =
        125

        BtnSelecionarTipo.Height =
        34

        BtnSelecionarTipo.Margin =
        New Padding(3)

        BtnSelecionarTipo.FlatStyle =
        FlatStyle.Flat

        BtnSelecionarTipo.BackColor =
        Tema.Painel

        BtnSelecionarTipo.ForeColor =
        Tema.Texto

        BtnSelecionarTipo.Cursor =
        Cursors.Hand

        BtnSelecionarTipo.UseVisualStyleBackColor =
        False

        BtnSelecionarTipo.FlatAppearance.BorderColor =
        Tema.Borda

        BtnSelecionarTipo.FlatAppearance.MouseOverBackColor =
        Tema.PainelHover

        AddHandler BtnSelecionarTipo.Click,
        AddressOf BtnSelecionarTipo_Click

        painelAcoes.Controls.Add(
        BtnSelecionarTipo)

        BtnSelecionarExibidos.Text =
        "Selecionar exibidos"

        BtnSelecionarExibidos.Width =
        145

        BtnSelecionarExibidos.Height =
        34

        BtnSelecionarExibidos.Margin =
        New Padding(3)

        BtnSelecionarExibidos.FlatStyle =
        FlatStyle.Flat

        BtnSelecionarExibidos.BackColor =
        Tema.Painel

        BtnSelecionarExibidos.ForeColor =
        Tema.Texto

        BtnSelecionarExibidos.Cursor =
        Cursors.Hand

        BtnSelecionarExibidos.UseVisualStyleBackColor =
        False

        BtnSelecionarExibidos.FlatAppearance.BorderColor =
        Tema.Borda

        BtnSelecionarExibidos.FlatAppearance.MouseOverBackColor =
        Tema.PainelHover

        AddHandler BtnSelecionarExibidos.Click,
        AddressOf BtnSelecionarExibidos_Click

        painelAcoes.Controls.Add(
        BtnSelecionarExibidos)

        painelPrincipal.Controls.Add(
        painelAcoes,
        0,
        3)

    End Sub

    Private Sub BtnSelecionarExibidos_Click(
    sender As Object,
    e As EventArgs)

        SelecionarObjetosExibidos()

    End Sub

    Private Sub SelecionarObjetosExibidos()

        If LvObjetos.Items.Count = 0 Then

            MessageBox.Show(
            "Nenhum objeto está sendo exibido pelos filtros atuais.",
            "Selecionar objetos",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information)

            Exit Sub

        End If

        Dim objetosSelecionar As New List(Of ObjetoCampo)()

        For Each item As ListViewItem
        In LvObjetos.Items

            Dim objeto As ObjetoCampo =
            TryCast(
                item.Tag,
                ObjetoCampo)

            If objeto Is Nothing Then
                Continue For
            End If

            If Not objetosSelecionar.Contains(
            objeto) Then

                objetosSelecionar.Add(
                objeto)

            End If

        Next

        If objetosSelecionar.Count = 0 Then
            Exit Sub
        End If

        _campo.SelecionarObjetosPelaLista(
        objetosSelecionar)

        AtualizarSelecaoListaSemRecriar()

        LvObjetos.Focus()

    End Sub

    Private Sub BtnSelecionarTipo_Click(
    sender As Object,
    e As EventArgs)

        SelecionarObjetosDoMesmoTipo()

    End Sub

    Private Sub SelecionarObjetosDoMesmoTipo()

        Dim objetoReferencia As ObjetoCampo =
        ObterObjetoFocadoLista()

        If objetoReferencia Is Nothing Then

            MessageBox.Show(
            "Selecione um objeto na lista para usar como referência.",
            "Selecionar por tipo",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information)

            Exit Sub

        End If

        Dim tipoReferencia As Type =
        objetoReferencia.GetType()

        Dim objetosSelecionar As New List(Of ObjetoCampo)()

        For Each item As ListViewItem
        In LvObjetos.Items

            Dim objetoItem As ObjetoCampo =
            TryCast(
                item.Tag,
                ObjetoCampo)

            If objetoItem Is Nothing Then
                Continue For
            End If

            If objetoItem.GetType() =
           tipoReferencia Then

                objetosSelecionar.Add(
                objetoItem)

            End If

        Next

        If objetosSelecionar.Count = 0 Then
            Exit Sub
        End If

        _campo.SelecionarObjetosPelaLista(
        objetosSelecionar)

        AtualizarSelecaoListaSemRecriar()

        LvObjetos.Focus()

    End Sub

    Private Function ObterObjetoFocadoLista() As ObjetoCampo

        If LvObjetos.FocusedItem IsNot Nothing Then

            Dim objetoFocado As ObjetoCampo =
            TryCast(
                LvObjetos.FocusedItem.Tag,
                ObjetoCampo)

            If objetoFocado IsNot Nothing Then

                Return objetoFocado

            End If

        End If

        If LvObjetos.SelectedItems.Count > 0 Then

            Return TryCast(
            LvObjetos.SelectedItems(0).Tag,
            ObjetoCampo)

        End If

        Return Nothing

    End Function

    Private Sub LvObjetos_DoubleClick(sender As Object, e As EventArgs)

        If LvObjetos.SelectedItems.Count <> 1 Then
            Exit Sub
        End If

        RenomearObjetoFocado()

    End Sub

    Private Function CriarBotaoAcao(
    texto As String,
    largura As Integer,
    acao As Action,
    Optional destaque As Boolean = False) As Button

        Dim botao As New Button With {
            .Text = texto,
            .Width = largura,
            .Height = 36,
            .Margin = New Padding(3),
            .FlatStyle = FlatStyle.Flat,
            .Cursor = Cursors.Hand,
            .UseVisualStyleBackColor = False
        }

        If destaque Then

            botao.Tag =
                "Destaque"

            botao.BackColor =
                Tema.CorPrimaria

            botao.ForeColor =
                Tema.TextoSobreCorPrimaria

            botao.FlatAppearance.BorderColor =
                Tema.TextoSobreCorPrimaria

        Else

            botao.BackColor =
                Tema.Painel

            botao.ForeColor =
                Tema.Texto

            botao.FlatAppearance.BorderColor =
                Tema.Borda

        End If

        botao.FlatAppearance.MouseOverBackColor =
            Tema.PainelHover

        AddHandler botao.Click,
            Sub(sender, e)
                acao.Invoke()
            End Sub

        Return botao

    End Function

    Public Sub AplicarTemaAtual()

        BackColor = Tema.Fundo

        ForeColor = Tema.Texto

        Font = Tema.FontePadrao

        AplicarTemaRecursivo(Controls)

        LvObjetos.BackColor = Tema.CampoEntrada

        LvObjetos.ForeColor = Tema.TextoCampo

        TxtFiltro.BackColor = Tema.CampoEntrada

        TxtFiltro.ForeColor = Tema.TextoCampo

        CmbTipo.BackColor = Tema.CampoEntrada

        CmbTipo.ForeColor = Tema.TextoCampo

        CmbEstado.BackColor = Tema.CampoEntrada

        CmbEstado.ForeColor = Tema.TextoCampo

        CmbOrdenacao.BackColor = Tema.CampoEntrada

        CmbOrdenacao.ForeColor = Tema.TextoCampo

        Invalidate(True)

        Refresh()

    End Sub

    Private Function ObterOrdemFrenteObjeto(
    objeto As ObjetoCampo) As Integer

        If objeto Is Nothing Then
            Return 0
        End If

        Dim objetosAtuais As IReadOnlyList(Of ObjetoCampo) =
        _campo.ObjetosAtuais

        For indice As Integer =
        0 To objetosAtuais.Count - 1

            If objetosAtuais(indice) Is objeto Then

                Return objetosAtuais.Count - indice

            End If

        Next

        Return 0

    End Function
    Private Sub OrdenarObjetosParaExibicao(
    objetos As List(Of ObjetoCampo))

        If objetos Is Nothing OrElse
       objetos.Count <= 1 Then

            Exit Sub

        End If

        Dim ordenacaoSelecionada As String =
        "Camada: frente para trás"

        If CmbOrdenacao.SelectedItem IsNot Nothing Then

            ordenacaoSelecionada =
            CStr(
                CmbOrdenacao.SelectedItem)

        End If

        Select Case ordenacaoSelecionada

            Case "Camada: frente para trás"

                'O último objeto da coleção é desenhado na frente.
                objetos.Reverse()

            Case "Camada: trás para frente"

            'A ordem original já representa trás para frente.

            Case "Nome: A até Z"

                objetos.Sort(
                New Comparison(Of ObjetoCampo)(
                    Function(objetoA, objetoB) As Integer

                        Return StringComparer.
                            CurrentCultureIgnoreCase.
                            Compare(
                                ObterDescricaoObjeto(
                                    objetoA),
                                ObterDescricaoObjeto(
                                    objetoB))

                    End Function))

            Case "Nome: Z até A"

                objetos.Sort(
                New Comparison(Of ObjetoCampo)(
                    Function(objetoA, objetoB) As Integer

                        Return StringComparer.
                            CurrentCultureIgnoreCase.
                            Compare(
                                ObterDescricaoObjeto(
                                    objetoB),
                                ObterDescricaoObjeto(
                                    objetoA))

                    End Function))

            Case "Tipo: A até Z"

                objetos.Sort(
                New Comparison(Of ObjetoCampo)(
                    Function(objetoA, objetoB) As Integer

                        Dim tipoA As String =
                            ObterTipoObjeto(
                                objetoA)

                        Dim tipoB As String =
                            ObterTipoObjeto(
                                objetoB)

                        Dim comparacaoTipo As Integer =
                            StringComparer.
                                CurrentCultureIgnoreCase.
                                Compare(
                                    tipoA,
                                    tipoB)

                        If comparacaoTipo <> 0 Then

                            Return comparacaoTipo

                        End If

                        Return StringComparer.
                            CurrentCultureIgnoreCase.
                            Compare(
                                ObterDescricaoObjeto(
                                    objetoA),
                                ObterDescricaoObjeto(
                                    objetoB))

                    End Function))

        End Select

    End Sub

    Private Function ObterOrdemRealObjeto(
    objeto As ObjetoCampo) As Integer

        If objeto Is Nothing Then
            Return 0
        End If

        Dim objetosAtuais As IReadOnlyList(Of ObjetoCampo) =
        _campo.ObjetosAtuais

        For indice As Integer =
        0 To objetosAtuais.Count - 1

            If objetosAtuais(indice) Is objeto Then

                Return indice + 1

            End If

        Next

        Return 0

    End Function
    Private Sub AplicarTemaRecursivo(
    controles As Control.ControlCollection)

        For Each controle As Control In controles

            If TypeOf controle Is FlowLayoutPanel OrElse
               TypeOf controle Is TableLayoutPanel OrElse
               TypeOf controle Is Panel Then

                controle.BackColor =
                    Tema.Fundo

                controle.ForeColor =
                    Tema.Texto

            ElseIf TypeOf controle Is Label Then

                controle.BackColor =
                    Color.Transparent

                controle.ForeColor =
                    Tema.Texto

            ElseIf TypeOf controle Is Button Then

                Dim botao As Button =
                    DirectCast(
                        controle,
                        Button)

                botao.UseVisualStyleBackColor =
                    False

                If String.Equals(
                    TryCast(
                        botao.Tag,
                        String),
                    "Destaque",
                    StringComparison.Ordinal) Then

                    botao.BackColor =
                        Tema.CorPrimaria

                    botao.ForeColor =
                        Tema.TextoSobreCorPrimaria

                    botao.FlatAppearance.BorderColor =
                        Tema.TextoSobreCorPrimaria

                Else

                    botao.BackColor =
                        Tema.Painel

                    botao.ForeColor =
                        Tema.Texto

                    botao.FlatAppearance.BorderColor =
                        Tema.Borda

                End If

                botao.FlatAppearance.MouseOverBackColor =
                    Tema.PainelHover

            End If

            If controle.HasChildren Then

                AplicarTemaRecursivo(
                    controle.Controls)

            End If

        Next

    End Sub

    Public Sub AtualizarConteudo()

        If IsDisposed Then
            Exit Sub
        End If

        _atualizandoLista =
        True

        LvObjetos.BeginUpdate()

        Try

            Dim selecionados As New HashSet(Of ObjetoCampo)(
            _campo.ObjetosSelecionadosAtuais)

            Dim filtro As String =
            TxtFiltro.Text.Trim()

            Dim tipoSelecionado As String =
            "Todos"

            If CmbTipo.SelectedItem IsNot Nothing Then

                tipoSelecionado =
                CStr(
                    CmbTipo.SelectedItem)

            End If

            Dim estadoSelecionado As String =
            "Todos"

            If CmbEstado.SelectedItem IsNot Nothing Then

                estadoSelecionado =
                CStr(
                    CmbEstado.SelectedItem)

            End If

            Dim objetosAtuais As IReadOnlyList(Of ObjetoCampo) =
            _campo.ObjetosAtuais

            Dim objetosExibicao As New List(Of ObjetoCampo)()

            For Each objeto As ObjetoCampo
            In objetosAtuais

                objetosExibicao.Add(
                objeto)

            Next

            OrdenarObjetosParaExibicao(
            objetosExibicao)

            LvObjetos.Items.Clear()

            Dim quantidadeExibida As Integer =
            0

            For Each objeto As ObjetoCampo
            In objetosExibicao

                Dim tipoObjeto As String =
                ObterTipoObjeto(
                    objeto)

                Dim descricao As String =
                ObterDescricaoObjeto(
                    objeto)

                If tipoSelecionado <> "Todos" AndAlso
               tipoSelecionado <> tipoObjeto Then

                    Continue For

                End If

                If Not ObjetoCorrespondeAoEstado(
                objeto,
                estadoSelecionado) Then

                    Continue For

                End If

                If filtro.Length > 0 AndAlso
               descricao.IndexOf(
                   filtro,
                   StringComparison.CurrentCultureIgnoreCase) < 0 AndAlso
               tipoObjeto.IndexOf(
                   filtro,
                   StringComparison.CurrentCultureIgnoreCase) < 0 Then

                    Continue For

                End If

                Dim grupo As String =
                "-"

                If Not String.IsNullOrWhiteSpace(
                objeto.GrupoId) Then

                    grupo =
                    "#" &
                    objeto.GrupoId.Substring(
                        0,
                        Math.Min(
                            6,
                            objeto.GrupoId.Length)).
                    ToUpperInvariant()

                End If

                Dim item As New ListViewItem(
                descricao)

                item.Tag =
                objeto

                item.SubItems.Add(
                tipoObjeto)

                item.SubItems.Add(
                grupo)

                item.SubItems.Add(
                If(
                    objeto.Visivel,
                    "Sim",
                    "Não"))

                item.SubItems.Add(
                If(
                    objeto.Bloqueado,
                    "Sim",
                    "Não"))

                item.SubItems.Add(
                ObterOrdemFrenteObjeto(
                    objeto).
                ToString())

                item.Selected =
                selecionados.Contains(
                    objeto)

                LvObjetos.Items.Add(
                item)

                quantidadeExibida +=
                1

            Next

            LblResumo.Text =
            quantidadeExibida.ToString() &
            " de " &
            objetosAtuais.Count.ToString() &
            " objetos exibidos — ordem 1 é a frente."

        Finally

            LvObjetos.EndUpdate()

            _atualizandoLista =
            False

        End Try

    End Sub

    Private Function ObjetoCorrespondeAoEstado(
    objeto As ObjetoCampo,
    estadoSelecionado As String) As Boolean

        If objeto Is Nothing Then
            Return False
        End If

        Select Case estadoSelecionado

            Case "Visíveis"

                Return objeto.Visivel

            Case "Ocultos"

                Return Not objeto.Visivel

            Case "Bloqueados"

                Return objeto.Bloqueado

            Case "Desbloqueados"

                Return Not objeto.Bloqueado

            Case "Agrupados"

                Return Not String.IsNullOrWhiteSpace(
                objeto.GrupoId)

            Case "Sem grupo"

                Return String.IsNullOrWhiteSpace(
                objeto.GrupoId)

            Case Else

                Return True

        End Select

    End Function
    Private Function ObterTipoObjeto(
    objeto As ObjetoCampo) As String

        If TypeOf objeto Is Jogador Then
            Return "Jogador"
        End If

        If TypeOf objeto Is Bola Then
            Return "Bola"
        End If

        If TypeOf objeto Is Cone Then
            Return "Cone"
        End If

        If TypeOf objeto Is Gol Then
            Return "Gol"
        End If

        If TypeOf objeto Is Manequim Then
            Return "Manequim"
        End If

        If TypeOf objeto Is LinhaTatica Then
            Return "Linha tática"
        End If

        If TypeOf objeto Is AreaTatica Then
            Return "Área tática"
        End If

        If TypeOf objeto Is MarcadorTatico Then
            Return "Marcador"
        End If

        If TypeOf objeto Is TextoTatico Then
            Return "Texto tático"
        End If

        Return objeto.GetType().Name

    End Function

    Private Function ObterDescricaoObjeto(
    objeto As ObjetoCampo) As String

        If TypeOf objeto Is Jogador Then

            Dim jogador As Jogador =
                DirectCast(
                    objeto,
                    Jogador)

            Dim descricao As String =
                "Jogador " &
                jogador.Numero.ToString()

            If Not String.IsNullOrWhiteSpace(
                jogador.Nome) Then

                descricao &=
                    " - " &
                    jogador.Nome.Trim()

            End If

            Return descricao

        End If

        If TypeOf objeto Is MarcadorTatico Then

            Dim marcador As MarcadorTatico =
                DirectCast(
                    objeto,
                    MarcadorTatico)

            Return MontarDescricaoComTexto(
                "Marcador",
                marcador.Texto)

        End If

        If TypeOf objeto Is TextoTatico Then

            Dim texto As TextoTatico =
                DirectCast(
                    objeto,
                    TextoTatico)

            Return MontarDescricaoComTexto(
                "Texto",
                texto.Texto)

        End If

        Return ObterTipoObjeto(
            objeto)

    End Function

    Private Function MontarDescricaoComTexto(
    prefixo As String,
    texto As String) As String

        If String.IsNullOrWhiteSpace(
            texto) Then

            Return prefixo

        End If

        Dim textoSeguro As String =
            texto.Trim().
            Replace(
                Environment.NewLine,
                " ").
            Replace(
                vbLf,
                " ").
            Replace(
                vbCr,
                " ")

        If textoSeguro.Length > 34 Then

            textoSeguro =
                textoSeguro.Substring(
                    0,
                    34) &
                "…"

        End If

        Return prefixo &
            " - " &
            textoSeguro

    End Function

    Private Sub LvObjetos_SelectedIndexChanged(
    sender As Object,
    e As EventArgs)

        If _atualizandoLista OrElse
           _sincronizacaoPendente Then

            Exit Sub

        End If

        _sincronizacaoPendente =
            True

        BeginInvoke(
            New MethodInvoker(
                AddressOf SincronizarSelecaoComCampo))

    End Sub

    Private Sub SincronizarSelecaoComCampo()

        _sincronizacaoPendente =
        False

        If IsDisposed OrElse
       _atualizandoLista Then

            Exit Sub

        End If

        _selecionandoPelaLista =
        True

        Try

            _campo.SelecionarObjetosPelaLista(
            ObterObjetosSelecionadosLista())

            AtualizarSelecaoListaSemRecriar()

        Finally

            _selecionandoPelaLista =
            False

        End Try

    End Sub

    Private Sub AtualizarSelecaoListaSemRecriar()

        If IsDisposed Then
            Exit Sub
        End If

        Dim objetosSelecionados As New HashSet(
        Of ObjetoCampo)(
        _campo.ObjetosSelecionadosAtuais)

        _atualizandoLista =
        True

        Try

            For Each item As ListViewItem
            In LvObjetos.Items

                Dim objeto As ObjetoCampo =
                TryCast(
                    item.Tag,
                    ObjetoCampo)

                item.Selected =
                objeto IsNot Nothing AndAlso
                objetosSelecionados.Contains(
                    objeto)

            Next

        Finally

            _atualizandoLista =
            False

        End Try

    End Sub

    Private Function ObterObjetosSelecionadosLista() As List(Of ObjetoCampo)

        Dim objetos As New List(Of ObjetoCampo)()

        For Each item As ListViewItem In LvObjetos.SelectedItems

            Dim objeto As ObjetoCampo =
                TryCast(
                    item.Tag,
                    ObjetoCampo)

            If objeto IsNot Nothing AndAlso
               Not objetos.Contains(objeto) Then

                objetos.Add(
                    objeto)

            End If

        Next

        Return objetos

    End Function

    Private Function PrepararObjetosSelecionados() As List(Of ObjetoCampo)

        Dim objetos As List(Of ObjetoCampo) =
            ObterObjetosSelecionadosLista()

        If objetos.Count = 0 Then

            MessageBox.Show(
                Me,
                "Selecione pelo menos um objeto na lista.",
                "Objetos e camadas",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information)

            Return objetos

        End If

        _campo.SelecionarObjetosPelaLista(
            objetos)

        Return New List(Of ObjetoCampo)(
            _campo.ObjetosSelecionadosAtuais)

    End Function

    Private Sub MostrarSelecionados()

        Dim objetos As List(Of ObjetoCampo) =
            PrepararObjetosSelecionados()

        If objetos.Count = 0 Then
            Exit Sub
        End If

        _campo.DefinirVisibilidadeObjetos(
            objetos,
            True)

    End Sub

    Private Sub OcultarSelecionados()

        Dim objetos As List(Of ObjetoCampo) =
            PrepararObjetosSelecionados()

        If objetos.Count = 0 Then
            Exit Sub
        End If

        _campo.DefinirVisibilidadeObjetos(
            objetos,
            False)

    End Sub

    Private Sub BloquearSelecionados()

        Dim objetos As List(Of ObjetoCampo) =
            PrepararObjetosSelecionados()

        If objetos.Count = 0 Then
            Exit Sub
        End If

        _campo.DefinirBloqueioObjetos(
            objetos,
            True)

    End Sub

    Private Sub DesbloquearSelecionados()

        Dim objetos As List(Of ObjetoCampo) =
            PrepararObjetosSelecionados()

        If objetos.Count = 0 Then
            Exit Sub
        End If

        _campo.DefinirBloqueioObjetos(
            objetos,
            False)

    End Sub

    Private Sub SubirCamada()

        If PrepararObjetosSelecionados().Count = 0 Then
            Exit Sub
        End If

        _campo.SubirCamadaSelecionados()

    End Sub

    Private Sub DescerCamada()

        If PrepararObjetosSelecionados().Count = 0 Then
            Exit Sub
        End If

        _campo.DescerCamadaSelecionados()

    End Sub

    Private Sub TrazerParaFrente()

        If PrepararObjetosSelecionados().Count = 0 Then
            Exit Sub
        End If

        _campo.TrazerSelecionadosParaFrente()

    End Sub

    Private Sub EnviarParaTras()

        If PrepararObjetosSelecionados().Count = 0 Then
            Exit Sub
        End If

        _campo.EnviarSelecionadosParaTras()

    End Sub

    Private Sub ExcluirSelecionados()

        If PrepararObjetosSelecionados().Count = 0 Then
            Exit Sub
        End If

        _campo.ExcluirSelecionado()

    End Sub

    Private Sub Campo_ObjetosAlterados()

        If IsDisposed OrElse
           Not IsHandleCreated Then

            Exit Sub

        End If

        BeginInvoke(
            New MethodInvoker(
                AddressOf AtualizarConteudo))

    End Sub

    Private Sub Campo_ObjetoSelecionadoAlterado(objeto As ObjetoCampo)

        If IsDisposed OrElse
       Not IsHandleCreated Then

            Exit Sub

        End If

        If _selecionandoPelaLista Then
            Exit Sub
        End If

        BeginInvoke(
        New MethodInvoker(
            AddressOf AtualizarSelecaoListaSemRecriar))

    End Sub

    Protected Overrides Sub OnFormClosed(
    e As FormClosedEventArgs)

        RemoveHandler _campo.ObjetosAlterados,
            AddressOf Campo_ObjetosAlterados

        RemoveHandler _campo.ObjetoSelecionadoAlterado,
            AddressOf Campo_ObjetoSelecionadoAlterado

        MyBase.OnFormClosed(
            e)

    End Sub

    Private Sub BtnRenomear_Click(
    sender As Object,
    e As EventArgs)

        RenomearObjetoFocado()

    End Sub

    Private Function SolicitarNomeObjeto(
    nomeAtual As String,
    ByRef novoNome As String) As Boolean

        Using formulario As New Form()

            formulario.Text =
                "Renomear objeto"

            formulario.StartPosition =
                FormStartPosition.CenterParent

            formulario.FormBorderStyle =
                FormBorderStyle.FixedDialog

            formulario.ClientSize =
                New Size(
                    430,
                    165)

            formulario.MaximizeBox =
                False

            formulario.MinimizeBox =
                False

            formulario.ShowInTaskbar =
                False

            formulario.BackColor =
                Tema.Fundo

            formulario.ForeColor =
                Tema.Texto

            formulario.Font =
                Tema.FontePadrao

            Dim label As New Label With {
                .Text =
                    "Digite um nome para o objeto:" &
                    Environment.NewLine &
                    "Deixe vazio para restaurar o nome automático.",
                .Left = 16,
                .Top = 14,
                .Width = 395,
                .Height = 44,
                .ForeColor = Tema.Texto
            }

            formulario.Controls.Add(
                label)

            Dim campoNome As New TextBox With {
                .Left = 16,
                .Top = 62,
                .Width = 395,
                .Height = 30,
                .MaxLength = 80,
                .Text = If(
                    nomeAtual,
                    String.Empty),
                .BackColor = Tema.CampoEntrada,
                .ForeColor = Tema.TextoCampo,
                .BorderStyle = BorderStyle.FixedSingle
            }

            formulario.Controls.Add(
                campoNome)

            Dim botaoCancelar As New Button With {
                .Text = "Cancelar",
                .Left = 221,
                .Top = 112,
                .Width = 90,
                .Height = 34,
                .DialogResult = DialogResult.Cancel,
                .FlatStyle = FlatStyle.Flat,
                .BackColor = Tema.Painel,
                .ForeColor = Tema.Texto,
                .UseVisualStyleBackColor = False
            }

            botaoCancelar.FlatAppearance.BorderColor =
                Tema.Borda

            formulario.Controls.Add(
                botaoCancelar)

            Dim botaoConfirmar As New Button With {
                .Text = "Confirmar",
                .Left = 321,
                .Top = 112,
                .Width = 90,
                .Height = 34,
                .DialogResult = DialogResult.OK,
                .FlatStyle = FlatStyle.Flat,
                .BackColor = Tema.CorPrimaria,
                .ForeColor = Tema.TextoSobreCorPrimaria,
                .UseVisualStyleBackColor = False
            }

            botaoConfirmar.FlatAppearance.BorderColor =
                Tema.TextoSobreCorPrimaria

            formulario.Controls.Add(
                botaoConfirmar)

            formulario.AcceptButton =
                botaoConfirmar

            formulario.CancelButton =
                botaoCancelar

            AddHandler formulario.Shown,
                Sub(sender, e)

                    campoNome.Focus()

                    campoNome.SelectAll()

                End Sub

            If formulario.ShowDialog(Me) <>
               DialogResult.OK Then

                Return False

            End If

            novoNome =
                campoNome.Text.Trim()

            Return True

        End Using

    End Function

    Private Sub RenomearObjetoFocado()

        If LvObjetos.SelectedItems.Count = 0 Then

            MessageBox.Show(
                "Selecione um objeto na lista para renomeá-lo.",
                "Renomear objeto",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information)

            Exit Sub

        End If

        If LvObjetos.SelectedItems.Count > 1 Then

            MessageBox.Show(
                "Selecione somente um objeto para renomear.",
                "Renomear objeto",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information)

            Exit Sub

        End If

        Dim itemSelecionado As ListViewItem =
            LvObjetos.SelectedItems(0)

        Dim objeto As ObjetoCampo =
            TryCast(
                itemSelecionado.Tag,
                ObjetoCampo)

        If objeto Is Nothing Then
            Exit Sub
        End If

        Dim nomeAtual As String =
            If(
                objeto.NomePersonalizado,
                String.Empty)

        Dim novoNome As String =
            nomeAtual

        If Not SolicitarNomeObjeto(
            nomeAtual,
            novoNome) Then

            Exit Sub

        End If

        If Not _campo.RenomearObjeto(
            objeto,
            novoNome) Then

            Exit Sub

        End If

        AtualizarConteudo()

        SelecionarObjetoNaLista(
            objeto)

        LvObjetos.Focus()

    End Sub

    Private Sub SelecionarObjetoNaLista(
    objeto As ObjetoCampo)

        If objeto Is Nothing Then
            Exit Sub
        End If

        _atualizandoLista =
            True

        Try

            For Each item As ListViewItem
                In LvObjetos.Items

                Dim objetoItem As ObjetoCampo =
                    TryCast(
                        item.Tag,
                        ObjetoCampo)

                Dim selecionar As Boolean =
                    objetoItem Is objeto

                item.Selected =
                    selecionar

                If selecionar Then

                    item.Focused =
                        True

                    item.EnsureVisible()

                End If

            Next

        Finally

            _atualizandoLista =
                False

        End Try

    End Sub

    Protected Overrides Function ProcessCmdKey(
    ByRef msg As Message,
    keyData As Keys) As Boolean

        Dim tecla As Keys = keyData And Keys.KeyCode

        Dim modificadores As Keys = keyData And Keys.Modifiers

        If tecla = Keys.F2 AndAlso modificadores = Keys.None Then

            RenomearObjetoFocado()

            Return True

        End If

        If tecla = Keys.T AndAlso modificadores = Keys.Control Then

            SelecionarObjetosDoMesmoTipo()

            Return True

        End If

        If tecla = Keys.A AndAlso modificadores = Keys.Control Then

            SelecionarObjetosExibidos()

            Return True

        End If

        Return MyBase.ProcessCmdKey(
        msg,
        keyData)

    End Function

End Class
