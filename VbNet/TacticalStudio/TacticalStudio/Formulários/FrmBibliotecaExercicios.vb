Imports System.IO
Imports TacticalStudio.Core

Public Class FrmBibliotecaExercicios

    Inherits Form

#Region "Variáveis"

    Private ReadOnly _itens As New List(Of ItemBibliotecaExercicio)()

    Private _carregandoFiltros As Boolean

    Private ReadOnly TxtPesquisa As New TextBox()

    Private ReadOnly CmbCategoria As New ComboBox()

    Private ReadOnly ChkSomenteFavoritos As New CheckBox()

    Private ReadOnly BtnAtualizar As New Button()

    Private ReadOnly LvExercicios As New ListView()

    Private ReadOnly LblResumo As New Label()

    Private ReadOnly LblDescricao As New Label()

    Private ReadOnly BtnAbrir As New Button()

    Private ReadOnly BtnCancelar As New Button()

    Private ReadOnly BtnAdicionarAtual As New Button()

    Private ReadOnly BtnEditar As New Button()

    Private ReadOnly BtnFavorito As New Button()

    Private ReadOnly BtnExcluir As New Button()

#End Region

#Region "Eventos públicos"

    Public Event AdicionarAtualSolicitado()

#End Region

#Region "Propriedades"

    <System.ComponentModel.Browsable(False)>
    <System.ComponentModel.DesignerSerializationVisibility(
        System.ComponentModel.DesignerSerializationVisibility.Hidden)>
    Public Property ItemSelecionado As ItemBibliotecaExercicio

#End Region

#Region "Inicialização"

    Public Sub New()

        Text =
            "Biblioteca de exercícios"

        StartPosition =
            FormStartPosition.CenterParent

        FormBorderStyle =
            FormBorderStyle.Sizable

        MinimizeBox =
            False

        ShowInTaskbar =
            False

        ClientSize =
            New Size(
                980,
                650)

        MinimumSize =
            New Size(
                820,
                560)

        CriarInterface()

        AplicarTemaAtual()

        CarregarBiblioteca()

        AcceptButton =
            BtnAbrir

        CancelButton =
            BtnCancelar

    End Sub

#End Region

#Region "Interface"

    Private Sub CriarInterface()

        Dim painelPrincipal As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 1,
            .RowCount = 5,
            .Padding = New Padding(14),
            .Margin = New Padding(0),
            .BackColor = Tema.Fundo
        }

        painelPrincipal.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                52.0F))

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
                92.0F))

        painelPrincipal.RowStyles.Add(
    New RowStyle(
        SizeType.Absolute,
        94.0F))

        Controls.Add(
            painelPrincipal)

        CriarPainelFiltros(
            painelPrincipal)

        CriarResumo(
            painelPrincipal)

        CriarLista(
            painelPrincipal)

        CriarDescricao(
            painelPrincipal)

        CriarPainelBotoes(
            painelPrincipal)

    End Sub

    Private Sub CriarPainelFiltros(
        painelPrincipal As TableLayoutPanel)

        Dim painelFiltros As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 6,
            .RowCount = 1,
            .Padding = New Padding(
                0,
                7,
                0,
                7),
            .Margin = New Padding(0),
            .BackColor = Tema.Painel,
            .GrowStyle =
                TableLayoutPanelGrowStyle.FixedSize
        }

        painelFiltros.ColumnStyles.Add(
            New ColumnStyle(
                SizeType.Absolute,
                76.0F))

        painelFiltros.ColumnStyles.Add(
            New ColumnStyle(
                SizeType.Percent,
                100.0F))

        painelFiltros.ColumnStyles.Add(
            New ColumnStyle(
                SizeType.Absolute,
                82.0F))

        painelFiltros.ColumnStyles.Add(
            New ColumnStyle(
                SizeType.Absolute,
                190.0F))

        painelFiltros.ColumnStyles.Add(
            New ColumnStyle(
                SizeType.Absolute,
                155.0F))

        painelFiltros.ColumnStyles.Add(
            New ColumnStyle(
                SizeType.Absolute,
                105.0F))

        painelFiltros.RowStyles.Add(
            New RowStyle(
                SizeType.Percent,
                100.0F))

        Dim labelPesquisa As New Label With {
            .Text = "Localizar:",
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .ForeColor = Tema.Texto,
            .BackColor = Tema.Painel,
            .TextAlign =
                ContentAlignment.MiddleLeft
        }

        painelFiltros.Controls.Add(
            labelPesquisa,
            0,
            0)

        TxtPesquisa.Dock =
            DockStyle.Fill

        TxtPesquisa.Margin =
            New Padding(
                4,
                3,
                14,
                3)

        TxtPesquisa.PlaceholderText =
            "Nome, descrição ou categoria"

        TxtPesquisa.BorderStyle =
            BorderStyle.FixedSingle

        AddHandler TxtPesquisa.TextChanged,
            AddressOf FiltrosAlterados

        painelFiltros.Controls.Add(
            TxtPesquisa,
            1,
            0)

        Dim labelCategoria As New Label With {
            .Text = "Categoria:",
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .ForeColor = Tema.Texto,
            .BackColor = Tema.Painel,
            .TextAlign =
                ContentAlignment.MiddleLeft
        }

        painelFiltros.Controls.Add(
            labelCategoria,
            2,
            0)

        CmbCategoria.Dock =
            DockStyle.Fill

        CmbCategoria.Margin =
            New Padding(
                4,
                3,
                12,
                3)

        CmbCategoria.DropDownStyle =
            ComboBoxStyle.DropDownList

        CmbCategoria.FlatStyle =
            FlatStyle.Flat

        AddHandler CmbCategoria.SelectedIndexChanged,
            AddressOf FiltrosAlterados

        painelFiltros.Controls.Add(
            CmbCategoria,
            3,
            0)

        ChkSomenteFavoritos.Text =
            "Somente favoritos"

        ChkSomenteFavoritos.Dock =
            DockStyle.Fill

        ChkSomenteFavoritos.Margin =
            New Padding(
                6,
                0,
                6,
                0)

        ChkSomenteFavoritos.TextAlign =
            ContentAlignment.MiddleLeft

        ChkSomenteFavoritos.ForeColor =
            Tema.Texto

        ChkSomenteFavoritos.BackColor =
            Tema.Painel

        AddHandler ChkSomenteFavoritos.CheckedChanged,
            AddressOf FiltrosAlterados

        painelFiltros.Controls.Add(
            ChkSomenteFavoritos,
            4,
            0)

        ConfigurarBotao(
            BtnAtualizar,
            "Atualizar",
            98)

        BtnAtualizar.Dock =
            DockStyle.Fill

        BtnAtualizar.Margin =
            New Padding(
                4,
                2,
                0,
                2)

        AddHandler BtnAtualizar.Click,
            AddressOf BtnAtualizar_Click

        painelFiltros.Controls.Add(
            BtnAtualizar,
            5,
            0)

        painelPrincipal.Controls.Add(
            painelFiltros,
            0,
            0)

    End Sub

    Private Sub CriarResumo(
        painelPrincipal As TableLayoutPanel)

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

        painelPrincipal.Controls.Add(
            LblResumo,
            0,
            1)

    End Sub

    Private Sub CriarLista(
        painelPrincipal As TableLayoutPanel)

        LvExercicios.Dock =
            DockStyle.Fill

        LvExercicios.Margin =
            New Padding(0)

        LvExercicios.View =
            View.Details

        LvExercicios.FullRowSelect =
            True

        LvExercicios.MultiSelect =
            False

        LvExercicios.HideSelection =
            False

        LvExercicios.GridLines =
            True

        LvExercicios.HeaderStyle =
            ColumnHeaderStyle.Nonclickable

        LvExercicios.Columns.Add(
            "Exercício",
            350)

        LvExercicios.Columns.Add(
            "Categoria",
            190)

        LvExercicios.Columns.Add(
            "Favorito",
            90)

        LvExercicios.Columns.Add(
            "Atualizado",
            160)

        AddHandler LvExercicios.SelectedIndexChanged,
            AddressOf LvExercicios_SelectedIndexChanged

        AddHandler LvExercicios.ItemActivate,
            AddressOf LvExercicios_ItemActivate

        painelPrincipal.Controls.Add(
            LvExercicios,
            0,
            2)

    End Sub

    Private Sub CriarDescricao(
        painelPrincipal As TableLayoutPanel)

        LblDescricao.Dock =
            DockStyle.Fill

        LblDescricao.Margin =
            New Padding(
                0,
                8,
                0,
                4)

        LblDescricao.Padding =
            New Padding(10)

        LblDescricao.BorderStyle =
            BorderStyle.FixedSingle

        LblDescricao.TextAlign =
            ContentAlignment.MiddleLeft

        LblDescricao.AutoEllipsis =
            True

        LblDescricao.Text =
            "Selecione um exercício para visualizar sua descrição."

        painelPrincipal.Controls.Add(
            LblDescricao,
            0,
            3)

    End Sub

    Private Sub CriarPainelBotoes(
        painelPrincipal As TableLayoutPanel)

        Dim painelBotoes As New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .FlowDirection =
                FlowDirection.RightToLeft,
            .WrapContents = True,
            .Padding = New Padding(
                0,
                9,
                0,
                0),
            .Margin = New Padding(0),
            .BackColor = Tema.Fundo
        }

        ConfigurarBotao(
    BtnEditar,
    "Editar dados",
    125)

        BtnEditar.Enabled =
    False

        AddHandler BtnEditar.Click,
    AddressOf BtnEditar_Click

        painelBotoes.Controls.Add(
    BtnEditar)

        ConfigurarBotao(
    BtnFavorito,
    "Adicionar favorito",
    145)

        BtnFavorito.Enabled =
    False

        AddHandler BtnFavorito.Click,
    AddressOf BtnFavorito_Click

        painelBotoes.Controls.Add(
    BtnFavorito)

        ConfigurarBotao(
    BtnExcluir,
    "Excluir",
    105)

        BtnExcluir.Enabled =
    False

        AddHandler BtnExcluir.Click,
    AddressOf BtnExcluir_Click

        painelBotoes.Controls.Add(
    BtnExcluir)

        ConfigurarBotao(
            BtnAbrir,
            "Abrir exercício",
            145)

        BtnAbrir.Enabled =
            False

        AddHandler BtnAbrir.Click,
            AddressOf BtnAbrir_Click

        painelBotoes.Controls.Add(
            BtnAbrir)

        ConfigurarBotao(
            BtnCancelar,
            "Fechar",
            110)



        BtnCancelar.DialogResult =
            DialogResult.Cancel

        AddHandler BtnCancelar.Click,
            AddressOf BtnCancelar_Click

        painelBotoes.Controls.Add(
            BtnCancelar)

        ConfigurarBotao(
    BtnAdicionarAtual,
    "Adicionar exercício atual",
    185)

        AddHandler BtnAdicionarAtual.Click,
    AddressOf BtnAdicionarAtual_Click

        painelBotoes.Controls.Add(
    BtnAdicionarAtual)

        painelPrincipal.Controls.Add(
            painelBotoes,
            0,
            4)

    End Sub

    Private Sub ConfigurarBotao(
        botao As Button,
        texto As String,
        largura As Integer)

        botao.Text =
            texto

        botao.Width =
            largura

        botao.Height =
            34

        botao.Margin =
            New Padding(
                6,
                0,
                0,
                0)

        botao.FlatStyle =
            FlatStyle.Flat

        botao.Cursor =
            Cursors.Hand

        botao.UseVisualStyleBackColor =
            False

    End Sub

#End Region

#Region "Carregamento"

    Public Sub CarregarBiblioteca(
        Optional idSelecionar As String = Nothing)

        Dim categoriaAnterior As String =
            "Todas"

        If CmbCategoria.SelectedItem IsNot Nothing Then

            categoriaAnterior =
                CStr(
                    CmbCategoria.SelectedItem)

        End If

        _itens.Clear()

        Dim itensRepositorio As List(Of ItemBibliotecaExercicio) =
            RepositorioBibliotecaExercicios.ObterItens()

        If itensRepositorio IsNot Nothing Then

            For Each item As ItemBibliotecaExercicio
                In itensRepositorio

                If item Is Nothing Then
                    Continue For
                End If

                _itens.Add(
                    item)

            Next

        End If

        OrdenarItens()

        CarregarCategorias(
            categoriaAnterior)

        AtualizarLista(
            idSelecionar)

    End Sub

    Private Sub OrdenarItens()

        _itens.Sort(
            New Comparison(Of ItemBibliotecaExercicio)(
                Function(itemA, itemB) As Integer

                    If itemA.Favorito <>
                       itemB.Favorito Then

                        If itemA.Favorito Then
                            Return -1
                        End If

                        Return 1

                    End If

                    Dim comparacaoData As Integer =
                        DateTime.Compare(
                            itemB.DataAtualizacaoUtc,
                            itemA.DataAtualizacaoUtc)

                    If comparacaoData <> 0 Then
                        Return comparacaoData
                    End If

                    Return StringComparer.
                        CurrentCultureIgnoreCase.
                        Compare(
                            itemA.Nome,
                            itemB.Nome)

                End Function))

    End Sub

    Private Sub CarregarCategorias(
        categoriaSelecionar As String)

        _carregandoFiltros =
            True

        Try

            CmbCategoria.Items.Clear()

            CmbCategoria.Items.Add(
                "Todas")

            Dim categorias As New List(Of String)()

            Dim categoriasEncontradas As New HashSet(Of String)(
                StringComparer.CurrentCultureIgnoreCase)

            For Each item As ItemBibliotecaExercicio
                In _itens

                Dim categoria As String =
                    If(
                        item.Categoria,
                        String.Empty).
                    Trim()

                If String.IsNullOrWhiteSpace(
                    categoria) Then

                    categoria =
                        "Sem categoria"

                End If

                If categoriasEncontradas.Add(
                    categoria) Then

                    categorias.Add(
                        categoria)

                End If

            Next

            categorias.Sort(
                StringComparer.CurrentCultureIgnoreCase)

            For Each categoria As String
                In categorias

                CmbCategoria.Items.Add(
                    categoria)

            Next

            Dim indiceSelecionar As Integer =
                0

            If Not String.IsNullOrWhiteSpace(
                categoriaSelecionar) Then

                For indice As Integer =
                    0 To CmbCategoria.Items.Count - 1

                    If String.Equals(
                        CStr(
                            CmbCategoria.Items(indice)),
                        categoriaSelecionar,
                        StringComparison.CurrentCultureIgnoreCase) Then

                        indiceSelecionar =
                            indice

                        Exit For

                    End If

                Next

            End If

            CmbCategoria.SelectedIndex =
                indiceSelecionar

        Finally

            _carregandoFiltros =
                False

        End Try

    End Sub

#End Region

#Region "Filtros e lista"

    Private Sub AtualizarLista(
        Optional idSelecionar As String = Nothing)

        Dim pesquisa As String =
            TxtPesquisa.Text.Trim()

        Dim categoriaSelecionada As String =
            "Todas"

        If CmbCategoria.SelectedItem IsNot Nothing Then

            categoriaSelecionada =
                CStr(
                    CmbCategoria.SelectedItem)

        End If

        LvExercicios.BeginUpdate()

        Try

            LvExercicios.Items.Clear()

            Dim quantidadeExibida As Integer =
                0

            For Each itemBiblioteca As ItemBibliotecaExercicio
                In _itens

                If Not ItemCorrespondeAosFiltros(
                    itemBiblioteca,
                    pesquisa,
                    categoriaSelecionada,
                    ChkSomenteFavoritos.Checked) Then

                    Continue For

                End If

                Dim itemLista As New ListViewItem(
                    itemBiblioteca.Nome)

                itemLista.Tag =
                    itemBiblioteca

                itemLista.SubItems.Add(
                    itemBiblioteca.Categoria)

                itemLista.SubItems.Add(
                    If(
                        itemBiblioteca.Favorito,
                        "Sim",
                        "Não"))

                itemLista.SubItems.Add(
                    FormatarData(
                        itemBiblioteca.DataAtualizacaoUtc))

                LvExercicios.Items.Add(
                    itemLista)

                quantidadeExibida +=
                    1

                If Not String.IsNullOrWhiteSpace(
                    idSelecionar) AndAlso
                   String.Equals(
                       itemBiblioteca.Id,
                       idSelecionar,
                       StringComparison.OrdinalIgnoreCase) Then

                    itemLista.Selected =
                        True

                    itemLista.Focused =
                        True

                    itemLista.EnsureVisible()

                End If

            Next

            LblResumo.Text =
                quantidadeExibida.ToString() &
                " de " &
                _itens.Count.ToString() &
                " exercícios exibidos."

        Finally

            LvExercicios.EndUpdate()

        End Try

        AtualizarItemSelecionado()

    End Sub

    Private Function ItemCorrespondeAosFiltros(
        item As ItemBibliotecaExercicio,
        pesquisa As String,
        categoriaSelecionada As String,
        somenteFavoritos As Boolean
    ) As Boolean

        If item Is Nothing Then
            Return False
        End If

        If somenteFavoritos AndAlso
           Not item.Favorito Then

            Return False

        End If

        If categoriaSelecionada <> "Todas" AndAlso
           Not String.Equals(
               item.Categoria,
               categoriaSelecionada,
               StringComparison.CurrentCultureIgnoreCase) Then

            Return False

        End If

        If pesquisa.Length = 0 Then
            Return True
        End If

        If TextoContem(
            item.Nome,
            pesquisa) Then

            Return True

        End If

        If TextoContem(
            item.Descricao,
            pesquisa) Then

            Return True

        End If

        If TextoContem(
            item.Categoria,
            pesquisa) Then

            Return True

        End If

        Return False

    End Function

    Private Function TextoContem(
        texto As String,
        pesquisa As String
    ) As Boolean

        Return If(
            texto,
            String.Empty).
            IndexOf(
                pesquisa,
                StringComparison.CurrentCultureIgnoreCase) >= 0

    End Function

    Private Function FormatarData(
        data As DateTime
    ) As String

        If data = DateTime.MinValue Then
            Return "-"
        End If

        Dim dataExibicao As DateTime =
            data

        If data.Kind = DateTimeKind.Utc Then

            dataExibicao =
                data.ToLocalTime()

        End If

        Return dataExibicao.ToString(
            "dd/MM/yyyy HH:mm")

    End Function

#End Region

#Region "Seleção"
    Private Function ObterItemSelecionadoLista() As ItemBibliotecaExercicio

        If LvExercicios.SelectedItems.Count <> 1 Then
            Return Nothing
        End If

        Return TryCast(
            LvExercicios.SelectedItems(0).Tag,
            ItemBibliotecaExercicio)

    End Function

    Private Sub AtualizarItemSelecionado()

        ItemSelecionado =
            ObterItemSelecionadoLista()

        BtnAbrir.Enabled = ItemSelecionado IsNot Nothing

        BtnEditar.Enabled =
    ItemSelecionado IsNot Nothing

        BtnFavorito.Enabled =
    ItemSelecionado IsNot Nothing

        BtnExcluir.Enabled =
    ItemSelecionado IsNot Nothing

        If ItemSelecionado Is Nothing Then

            BtnFavorito.Text =
        "Adicionar favorito"

        ElseIf ItemSelecionado.Favorito Then

            BtnFavorito.Text =
        "Remover favorito"

        Else

            BtnFavorito.Text =
        "Adicionar favorito"

        End If

        If ItemSelecionado Is Nothing Then

            LblDescricao.Text =
                "Selecione um exercício para visualizar sua descrição."

            Exit Sub

        End If

        Dim descricao As String =
            If(
                ItemSelecionado.Descricao,
                String.Empty).
            Trim()

        If String.IsNullOrWhiteSpace(
            descricao) Then

            descricao =
                "Este exercício não possui descrição."

        End If

        LblDescricao.Text =
            ItemSelecionado.Nome &
            Environment.NewLine &
            Environment.NewLine &
            descricao

    End Sub

    Private Sub AbrirItemSelecionado()

        Dim item As ItemBibliotecaExercicio =
            ObterItemSelecionadoLista()

        If item Is Nothing Then

            MessageBox.Show(
                "Selecione um exercício da biblioteca.",
                "Biblioteca de exercícios",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information)

            Exit Sub

        End If

        Dim caminhoExercicio As String =
            RepositorioBibliotecaExercicios.
                ObterCaminhoExercicio(
                    item)

        If String.IsNullOrWhiteSpace(
            caminhoExercicio) OrElse
           Not File.Exists(
               caminhoExercicio) Then

            MessageBox.Show(
                "O arquivo deste exercício não foi encontrado.",
                "Biblioteca de exercícios",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning)

            Exit Sub

        End If

        ItemSelecionado =
            item

        DialogResult =
            DialogResult.OK

        Close()

    End Sub

    Private Function SolicitarEdicaoItem(
    item As ItemBibliotecaExercicio,
    ByRef nome As String,
    ByRef descricao As String,
    ByRef categoria As String
) As Boolean

        If item Is Nothing Then
            Return False
        End If

        Using formulario As New Form()

            formulario.Text =
            "Editar exercício da biblioteca"

            formulario.StartPosition =
            FormStartPosition.CenterParent

            formulario.FormBorderStyle =
            FormBorderStyle.FixedDialog

            formulario.MaximizeBox =
            False

            formulario.MinimizeBox =
            False

            formulario.ShowInTaskbar =
            False

            formulario.ClientSize =
            New Size(
                520,
                320)

            formulario.BackColor =
            Tema.Fundo

            formulario.ForeColor =
            Tema.Texto

            Dim painel As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 1,
            .RowCount = 7,
            .Padding = New Padding(14),
            .Margin = New Padding(0)
        }

            painel.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                26.0F))

            painel.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                38.0F))

            painel.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                26.0F))

            painel.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                38.0F))

            painel.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                26.0F))

            painel.RowStyles.Add(
            New RowStyle(
                SizeType.Percent,
                100.0F))

            painel.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                48.0F))

            formulario.Controls.Add(
            painel)

            Dim labelNome As New Label With {
            .Text = "Nome do exercício:",
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleLeft
        }

            painel.Controls.Add(
            labelNome,
            0,
            0)

            Dim txtNome As New TextBox With {
            .Dock = DockStyle.Fill,
            .MaxLength = 100,
            .Text = item.Nome,
            .BackColor = Tema.CampoEntrada,
            .ForeColor = Tema.TextoCampo
        }

            painel.Controls.Add(
            txtNome,
            0,
            1)

            Dim labelCategoria As New Label With {
            .Text = "Categoria:",
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleLeft
        }

            painel.Controls.Add(
            labelCategoria,
            0,
            2)

            Dim txtCategoria As New TextBox With {
            .Dock = DockStyle.Fill,
            .MaxLength = 80,
            .Text = item.Categoria,
            .BackColor = Tema.CampoEntrada,
            .ForeColor = Tema.TextoCampo
        }

            painel.Controls.Add(
            txtCategoria,
            0,
            3)

            Dim labelDescricao As New Label With {
            .Text = "Descrição:",
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleLeft
        }

            painel.Controls.Add(
            labelDescricao,
            0,
            4)

            Dim txtDescricao As New TextBox With {
            .Dock = DockStyle.Fill,
            .Multiline = True,
            .ScrollBars = ScrollBars.Vertical,
            .MaxLength = 500,
            .Text = item.Descricao,
            .BackColor = Tema.CampoEntrada,
            .ForeColor = Tema.TextoCampo
        }

            painel.Controls.Add(
            txtDescricao,
            0,
            5)

            Dim painelBotoes As New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.RightToLeft,
            .WrapContents = False,
            .Padding = New Padding(
                0,
                8,
                0,
                0),
            .Margin = New Padding(0)
        }

            Dim btnSalvar As New Button With {
            .Text = "Salvar",
            .Width = 110,
            .Height = 32,
            .DialogResult = DialogResult.OK,
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Tema.Painel,
            .ForeColor = Tema.Texto,
            .Cursor = Cursors.Hand
        }

            btnSalvar.FlatAppearance.BorderColor =
            Tema.Borda

            btnSalvar.FlatAppearance.MouseOverBackColor =
            Tema.PainelHover

            Dim btnCancelarEdicao As New Button With {
            .Text = "Cancelar",
            .Width = 110,
            .Height = 32,
            .DialogResult = DialogResult.Cancel,
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Tema.Painel,
            .ForeColor = Tema.Texto,
            .Cursor = Cursors.Hand
        }

            btnCancelarEdicao.FlatAppearance.BorderColor =
            Tema.Borda

            btnCancelarEdicao.FlatAppearance.MouseOverBackColor =
            Tema.PainelHover

            painelBotoes.Controls.Add(
            btnSalvar)

            painelBotoes.Controls.Add(
            btnCancelarEdicao)

            painel.Controls.Add(
            painelBotoes,
            0,
            6)

            formulario.AcceptButton =
            btnSalvar

            formulario.CancelButton =
            btnCancelarEdicao

            txtNome.SelectAll()

            If formulario.ShowDialog(Me) <>
           DialogResult.OK Then

                Return False

            End If

            nome =
            txtNome.Text.Trim()

            descricao =
            txtDescricao.Text.Trim()

            categoria =
            txtCategoria.Text.Trim()

            Return True

        End Using

    End Function
    Private Sub EditarItemSelecionado()

        Dim item As ItemBibliotecaExercicio =
        ObterItemSelecionadoLista()

        If item Is Nothing Then
            Exit Sub
        End If

        Dim nome As String =
        item.Nome

        Dim descricao As String =
        item.Descricao

        Dim categoria As String =
        item.Categoria

        If Not SolicitarEdicaoItem(
        item,
        nome,
        descricao,
        categoria) Then

            Exit Sub

        End If

        If Not RepositorioBibliotecaExercicios.
        AtualizarMetadados(
            item.Id,
            nome,
            descricao,
            categoria,
            item.Favorito) Then

            MessageBox.Show(
            "Não foi possível atualizar o exercício.",
            "Biblioteca de exercícios",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error)

            Exit Sub

        End If

        CarregarBiblioteca(
        item.Id)

    End Sub

    Private Sub AlternarFavoritoItemSelecionado()

        Dim item As ItemBibliotecaExercicio =
        ObterItemSelecionadoLista()

        If item Is Nothing Then
            Exit Sub
        End If

        Dim novoEstado As Boolean =
        Not item.Favorito

        If Not RepositorioBibliotecaExercicios.
        DefinirFavorito(
            item.Id,
            novoEstado) Then

            MessageBox.Show(
            "Não foi possível alterar o favorito.",
            "Biblioteca de exercícios",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error)

            Exit Sub

        End If

        CarregarBiblioteca(
        item.Id)

    End Sub
    Private Sub ExcluirItemSelecionado()

        Dim item As ItemBibliotecaExercicio =
        ObterItemSelecionadoLista()

        If item Is Nothing Then
            Exit Sub
        End If

        Dim resposta As DialogResult =
        MessageBox.Show(
            "Deseja excluir o exercício """ &
            item.Nome &
            """ da biblioteca?" &
            Environment.NewLine &
            Environment.NewLine &
            "O arquivo armazenado na biblioteca também será excluído.",
            "Excluir exercício",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2)

        If resposta <> DialogResult.Yes Then
            Exit Sub
        End If

        If Not RepositorioBibliotecaExercicios.
        Remover(
            item.Id) Then

            MessageBox.Show(
            "Não foi possível excluir o exercício.",
            "Biblioteca de exercícios",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error)

            Exit Sub

        End If

        CarregarBiblioteca()

    End Sub


#End Region

#Region "Eventos"

    Private Sub FiltrosAlterados(
        sender As Object,
        e As EventArgs)

        If _carregandoFiltros Then
            Exit Sub
        End If

        AtualizarLista()

    End Sub

    Private Sub BtnAtualizar_Click(
        sender As Object,
        e As EventArgs)

        CarregarBiblioteca()

    End Sub

    Private Sub LvExercicios_SelectedIndexChanged(
        sender As Object,
        e As EventArgs)

        AtualizarItemSelecionado()

    End Sub

    Private Sub LvExercicios_ItemActivate(
        sender As Object,
        e As EventArgs)

        AbrirItemSelecionado()

    End Sub

    Private Sub BtnAbrir_Click(
        sender As Object,
        e As EventArgs)

        AbrirItemSelecionado()

    End Sub

    Private Sub BtnCancelar_Click(
        sender As Object,
        e As EventArgs)

        DialogResult =
            DialogResult.Cancel

        Close()

    End Sub
    Private Sub BtnAdicionarAtual_Click(
    sender As Object,
    e As EventArgs)

        RaiseEvent AdicionarAtualSolicitado()

    End Sub
    Private Sub BtnEditar_Click(
    sender As Object,
    e As EventArgs)

        EditarItemSelecionado()

    End Sub

    Private Sub BtnFavorito_Click(
    sender As Object,
    e As EventArgs)

        AlternarFavoritoItemSelecionado()

    End Sub

    Private Sub BtnExcluir_Click(
    sender As Object,
    e As EventArgs)

        ExcluirItemSelecionado()

    End Sub


#End Region

#Region "Atalhos"

    Protected Overrides Function ProcessCmdKey(
        ByRef msg As Message,
        keyData As Keys
    ) As Boolean

        Dim tecla As Keys =
            keyData And Keys.KeyCode

        Dim modificadores As Keys =
            keyData And Keys.Modifiers

        If tecla = Keys.F5 AndAlso
           modificadores = Keys.None Then

            CarregarBiblioteca()

            Return True

        End If

        Return MyBase.ProcessCmdKey(
            msg,
            keyData)

    End Function

#End Region

#Region "Tema"

    Public Sub AplicarTemaAtual()

        BackColor = Tema.Fundo

        ForeColor = Tema.Texto

        TxtPesquisa.BackColor = Tema.CampoEntrada

        TxtPesquisa.ForeColor = Tema.TextoCampo

        CmbCategoria.BackColor = Tema.CampoEntrada

        CmbCategoria.ForeColor = Tema.TextoCampo

        LvExercicios.BackColor = Tema.CampoEntrada

        LvExercicios.ForeColor = Tema.TextoCampo

        LblDescricao.BackColor = Tema.Painel

        LblDescricao.ForeColor = Tema.Texto

        BtnAtualizar.BackColor = Tema.Painel

        BtnAtualizar.ForeColor = Tema.Texto

        BtnAtualizar.FlatAppearance.BorderColor = Tema.Borda

        BtnAtualizar.FlatAppearance.MouseOverBackColor = Tema.PainelHover

        BtnAbrir.BackColor = Tema.Painel

        BtnAbrir.ForeColor = Tema.Texto

        BtnAbrir.FlatAppearance.BorderColor = Tema.Borda

        BtnAbrir.FlatAppearance.MouseOverBackColor = Tema.PainelHover

        BtnCancelar.BackColor = Tema.Painel

        BtnCancelar.ForeColor = Tema.Texto

        BtnCancelar.FlatAppearance.BorderColor = Tema.Borda

        BtnCancelar.FlatAppearance.MouseOverBackColor = Tema.PainelHover

        BtnAdicionarAtual.BackColor = Tema.Painel

        BtnAdicionarAtual.ForeColor = Tema.Texto

        BtnAdicionarAtual.FlatAppearance.BorderColor = Tema.Borda

        BtnAdicionarAtual.FlatAppearance.MouseOverBackColor = Tema.PainelHover

        BtnEditar.BackColor = Tema.Painel

        BtnEditar.ForeColor = Tema.Texto

        BtnEditar.FlatAppearance.BorderColor = Tema.Borda

        BtnEditar.FlatAppearance.MouseOverBackColor = Tema.PainelHover

        BtnFavorito.BackColor = Tema.Painel

        BtnFavorito.ForeColor = Tema.Texto

        BtnFavorito.FlatAppearance.BorderColor = Tema.Borda

        BtnFavorito.FlatAppearance.MouseOverBackColor = Tema.PainelHover

        BtnExcluir.BackColor = Tema.Painel

        BtnExcluir.ForeColor = Tema.Texto

        BtnExcluir.FlatAppearance.BorderColor = Tema.Borda

        BtnExcluir.FlatAppearance.MouseOverBackColor = Tema.PainelHover

    End Sub

#End Region

End Class