Imports TacticalStudio.Core

Public Class FrmFormacoes

    Inherits Form

#Region "Variáveis"

    Private ReadOnly _formacoes As New List(Of ModeloFormacao)()

    Private ReadOnly CmbFormacao As New ComboBox()

    Private ReadOnly LblDescricao As New Label()

    Private ReadOnly LvPosicoes As New ListView()

    Private ReadOnly BtnAplicar As New Button()

    Private ReadOnly BtnCancelar As New Button()

    Private ReadOnly _campo As CampoTatico

    Private ReadOnly BtnSalvarAtual As New Button()

    Private ReadOnly BtnExcluirPersonalizada As New Button()

#End Region

#Region "Propriedades"

    <System.ComponentModel.Browsable(False)>
    <System.ComponentModel.DesignerSerializationVisibility(
    System.ComponentModel.DesignerSerializationVisibility.Hidden)>
    Public Property FormacaoSelecionada As ModeloFormacao

#End Region

#Region "Inicialização"

    Public Sub New()

        Me.New(
        Nothing)

    End Sub

    Public Sub New(
    campo As CampoTatico)

        _campo =
        campo

        Text =
        "Formações táticas"

        StartPosition =
        FormStartPosition.CenterParent

        FormBorderStyle =
        FormBorderStyle.FixedDialog

        MaximizeBox =
        False

        MinimizeBox =
        False

        ShowInTaskbar =
        False

        ClientSize =
        New Size(
            760,
            520)

        MinimumSize =
        New Size(
            776,
            559)

        CriarInterface()

        AplicarTemaAtual()

        CarregarFormacoes()

        AcceptButton =
        BtnAplicar

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
            .Margin = New Padding(0)
        }

        painelPrincipal.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                32.0F))

        painelPrincipal.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                42.0F))

        painelPrincipal.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                72.0F))

        painelPrincipal.RowStyles.Add(
            New RowStyle(
                SizeType.Percent,
                100.0F))

        painelPrincipal.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                52.0F))

        Controls.Add(
            painelPrincipal)

        Dim labelTitulo As New Label With {
            .Text = "Escolha uma formação:",
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .Font = New Font(
                Font,
                FontStyle.Bold),
            .TextAlign = ContentAlignment.MiddleLeft
        }

        painelPrincipal.Controls.Add(
            labelTitulo,
            0,
            0)

        CmbFormacao.Dock =
            DockStyle.Fill

        CmbFormacao.Margin =
            New Padding(
                0,
                4,
                0,
                6)

        CmbFormacao.DropDownStyle =
            ComboBoxStyle.DropDownList

        CmbFormacao.FlatStyle =
            FlatStyle.Flat

        AddHandler CmbFormacao.SelectedIndexChanged,
            AddressOf CmbFormacao_SelectedIndexChanged

        painelPrincipal.Controls.Add(
            CmbFormacao,
            0,
            1)

        LblDescricao.Dock =
            DockStyle.Fill

        LblDescricao.Margin =
            New Padding(
                0,
                4,
                0,
                8)

        LblDescricao.Padding =
            New Padding(10)

        LblDescricao.BorderStyle =
            BorderStyle.FixedSingle

        LblDescricao.TextAlign =
            ContentAlignment.MiddleLeft

        painelPrincipal.Controls.Add(
            LblDescricao,
            0,
            2)

        LvPosicoes.Dock =
            DockStyle.Fill

        LvPosicoes.Margin =
            New Padding(0)

        LvPosicoes.View =
            View.Details

        LvPosicoes.FullRowSelect =
            True

        LvPosicoes.MultiSelect =
            False

        LvPosicoes.HideSelection =
            False

        LvPosicoes.GridLines =
            True

        LvPosicoes.HeaderStyle =
            ColumnHeaderStyle.Nonclickable

        LvPosicoes.Columns.Add(
            "Número",
            80)

        LvPosicoes.Columns.Add(
            "Posição",
            260)

        LvPosicoes.Columns.Add(
            "Horizontal",
            120)

        LvPosicoes.Columns.Add(
            "Vertical",
            120)

        painelPrincipal.Controls.Add(
            LvPosicoes,
            0,
            3)

        Dim painelBotoes As New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.RightToLeft,
            .WrapContents = False,
            .Padding = New Padding(
                0,
                9,
                0,
                0),
            .Margin = New Padding(0)
        }

        ConfigurarBotao(
            BtnAplicar,
            "Aplicar formação",
            145)

        AddHandler BtnAplicar.Click,
            AddressOf BtnAplicar_Click

        painelBotoes.Controls.Add(
            BtnAplicar)

        ConfigurarBotao(
            BtnCancelar,
            "Cancelar",
            110)

        BtnCancelar.DialogResult =
            DialogResult.Cancel

        AddHandler BtnCancelar.Click,
            AddressOf BtnCancelar_Click

        painelBotoes.Controls.Add(
            BtnCancelar)

        ConfigurarBotao(
    BtnExcluirPersonalizada,
    "Excluir personalizada",
    165)

        AddHandler BtnExcluirPersonalizada.Click,
    AddressOf BtnExcluirPersonalizada_Click

        painelBotoes.Controls.Add(
    BtnExcluirPersonalizada)

        ConfigurarBotao(
    BtnSalvarAtual,
    "Salvar formação atual",
    175)

        AddHandler BtnSalvarAtual.Click,
    AddressOf BtnSalvarAtual_Click

        painelBotoes.Controls.Add(
    BtnSalvarAtual)

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

#Region "Formações"

    Private Sub CarregarFormacoes(
    Optional idSelecionar As String = Nothing)

        _formacoes.Clear()

        CmbFormacao.Items.Clear()

        Dim formacoesPadrao As List(Of ModeloFormacao) =
        Formacoes.ObterTodas()

        If formacoesPadrao IsNot Nothing Then

            For Each formacao As ModeloFormacao
            In formacoesPadrao

                AdicionarFormacaoNaLista(
                formacao)

            Next

        End If

        Dim formacoesPersonalizadas As List(Of ModeloFormacao) =
        RepositorioFormacoesPersonalizadas.CarregarTodas()

        If formacoesPersonalizadas IsNot Nothing Then

            For Each formacao As ModeloFormacao
            In formacoesPersonalizadas

                AdicionarFormacaoNaLista(
                formacao)

            Next

        End If

        If CmbFormacao.Items.Count = 0 Then

            FormacaoSelecionada =
            Nothing

            LblDescricao.Text =
            "Nenhuma formação disponível."

            LvPosicoes.Items.Clear()

            BtnAplicar.Enabled =
            False

            BtnExcluirPersonalizada.Enabled =
            False

            Exit Sub

        End If

        Dim indiceSelecionar As Integer =
        0

        If Not String.IsNullOrWhiteSpace(
        idSelecionar) Then

            For indice As Integer =
            0 To _formacoes.Count - 1

                If String.Equals(
                _formacoes(indice).Id,
                idSelecionar,
                StringComparison.OrdinalIgnoreCase) Then

                    indiceSelecionar =
                    indice

                    Exit For

                End If

            Next

        End If

        CmbFormacao.SelectedIndex =
        indiceSelecionar

    End Sub

    Private Sub AdicionarFormacaoNaLista(
    formacao As ModeloFormacao)

        If formacao Is Nothing Then
            Exit Sub
        End If

        _formacoes.Add(
        formacao)

        Dim nomeExibicao As String =
        formacao.Nome

        If FormacaoEhPersonalizada(
        formacao) Then

            nomeExibicao &=
            " — Personalizada"

        End If

        CmbFormacao.Items.Add(
        nomeExibicao)

    End Sub

    Private Sub AtualizarFormacaoSelecionada()

        Dim indice As Integer =
            CmbFormacao.SelectedIndex

        If indice < 0 OrElse indice >= _formacoes.Count Then

            FormacaoSelecionada = Nothing

            LblDescricao.Text = String.Empty

            LvPosicoes.Items.Clear()

            BtnAplicar.Enabled = False

            BtnExcluirPersonalizada.Enabled = False

            Exit Sub

        End If

        FormacaoSelecionada = _formacoes(indice)

        BtnAplicar.Enabled = FormacaoSelecionada IsNot Nothing

        BtnExcluirPersonalizada.Enabled = FormacaoEhPersonalizada(FormacaoSelecionada)

        If FormacaoSelecionada Is Nothing Then
            Exit Sub
        End If

        LblDescricao.Text =
            FormacaoSelecionada.Descricao

        AtualizarListaPosicoes(
            FormacaoSelecionada)

    End Sub

    Private Sub AtualizarListaPosicoes(
        formacao As ModeloFormacao)

        LvPosicoes.BeginUpdate()

        Try

            LvPosicoes.Items.Clear()

            If formacao Is Nothing OrElse
               formacao.Posicoes Is Nothing Then

                Exit Sub

            End If

            For Each posicao As PosicaoFormacao
                In formacao.Posicoes

                If posicao Is Nothing Then
                    Continue For
                End If

                Dim item As New ListViewItem(
                    posicao.Numero.ToString())

                item.SubItems.Add(
                    posicao.Nome)

                item.SubItems.Add(
                    posicao.XPercentual.ToString(
                        "0.##") &
                    "%")

                item.SubItems.Add(
                    posicao.YPercentual.ToString(
                        "0.##") &
                    "%")

                item.Tag =
                    posicao

                LvPosicoes.Items.Add(
                    item)

            Next

        Finally

            LvPosicoes.EndUpdate()

        End Try

    End Sub

    Private Function FormacaoEhPersonalizada(
    formacao As ModeloFormacao) As Boolean

        If formacao Is Nothing Then
            Return False
        End If

        Return Not String.IsNullOrWhiteSpace(
               formacao.Id) AndAlso
           formacao.Id.StartsWith(
               "personalizada-",
               StringComparison.OrdinalIgnoreCase)

    End Function

    Private Function SolicitarDadosFormacao(
    ByRef nome As String,
    ByRef descricao As String,
    ByRef usarSomenteSelecionados As Boolean
) As Boolean

        Using formulario As New Form()

            formulario.Text =
            "Salvar formação atual"

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
                500,
                285)

            formulario.BackColor =
            Tema.Fundo

            formulario.ForeColor =
            Tema.Texto

            Dim painel As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 1,
            .RowCount = 6,
            .Padding = New Padding(14)
        }

            painel.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                28.0F))

            painel.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                38.0F))

            painel.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                28.0F))

            painel.RowStyles.Add(
            New RowStyle(
                SizeType.Percent,
                100.0F))

            painel.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                38.0F))

            painel.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                48.0F))

            formulario.Controls.Add(
            painel)

            Dim labelNome As New Label With {
            .Text = "Nome da formação:",
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleLeft
        }

            painel.Controls.Add(
            labelNome,
            0,
            0)

            Dim txtNome As New TextBox With {
            .Dock = DockStyle.Fill,
            .MaxLength = 80,
            .Text = "Minha formação",
            .BackColor = Tema.CampoEntrada,
            .ForeColor = Tema.TextoCampo
        }

            painel.Controls.Add(
            txtNome,
            0,
            1)

            Dim labelDescricao As New Label With {
            .Text = "Descrição:",
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleLeft
        }

            painel.Controls.Add(
            labelDescricao,
            0,
            2)

            Dim txtDescricao As New TextBox With {
            .Dock = DockStyle.Fill,
            .Multiline = True,
            .ScrollBars = ScrollBars.Vertical,
            .MaxLength = 250,
            .BackColor = Tema.CampoEntrada,
            .ForeColor = Tema.TextoCampo
        }

            painel.Controls.Add(
            txtDescricao,
            0,
            3)

            Dim chkSelecionados As New CheckBox With {
            .Text = "Salvar somente os jogadores selecionados",
            .Dock = DockStyle.Fill,
            .Checked = False,
            .ForeColor = Tema.Texto
        }

            painel.Controls.Add(
            chkSelecionados,
            0,
            4)

            Dim painelBotoes As New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.RightToLeft,
            .WrapContents = False,
            .Padding = New Padding(0, 8, 0, 0)
        }

            Dim btnConfirmar As New Button With {
            .Text = "Salvar",
            .Width = 110,
            .Height = 32,
            .DialogResult = DialogResult.OK,
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Tema.Painel,
            .ForeColor = Tema.Texto
        }

            btnConfirmar.FlatAppearance.BorderColor =
            Tema.Borda

            Dim btnCancelarDialogo As New Button With {
            .Text = "Cancelar",
            .Width = 110,
            .Height = 32,
            .DialogResult = DialogResult.Cancel,
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Tema.Painel,
            .ForeColor = Tema.Texto
        }

            btnCancelarDialogo.FlatAppearance.BorderColor =
            Tema.Borda

            painelBotoes.Controls.Add(
            btnConfirmar)

            painelBotoes.Controls.Add(
            btnCancelarDialogo)

            painel.Controls.Add(
            painelBotoes,
            0,
            5)

            formulario.AcceptButton =
            btnConfirmar

            formulario.CancelButton =
            btnCancelarDialogo

            txtNome.SelectAll()

            txtNome.Focus()

            If formulario.ShowDialog(Me) <>
           DialogResult.OK Then

                Return False

            End If

            nome =
            txtNome.Text.Trim()

            descricao =
            txtDescricao.Text.Trim()

            usarSomenteSelecionados =
            chkSelecionados.Checked

            Return True

        End Using

    End Function

    Private Sub SalvarFormacaoAtual()

        If _campo Is Nothing Then

            MessageBox.Show(
            "O campo tático não está disponível.",
            "Salvar formação",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning)

            Exit Sub

        End If

        Dim nome As String =
        String.Empty

        Dim descricao As String =
        String.Empty

        Dim usarSomenteSelecionados As Boolean =
        False

        If Not SolicitarDadosFormacao(
        nome,
        descricao,
        usarSomenteSelecionados) Then

            Exit Sub

        End If

        Dim modelo As ModeloFormacao =
        _campo.CriarModeloFormacaoAtual(
            nome,
            descricao,
            usarSomenteSelecionados)

        If modelo Is Nothing Then

            Dim mensagem As String =
            "Nenhum jogador foi encontrado no campo."

            If usarSomenteSelecionados Then

                mensagem =
                "Nenhum jogador selecionado foi encontrado."

            End If

            MessageBox.Show(
            mensagem,
            "Salvar formação",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information)

            Exit Sub

        End If

        If Not RepositorioFormacoesPersonalizadas.Adicionar(
        modelo) Then

            MessageBox.Show(
            "Não foi possível salvar a formação personalizada.",
            "Salvar formação",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error)

            Exit Sub

        End If

        CarregarFormacoes(
        modelo.Id)

        MessageBox.Show(
        "A formação foi salva com sucesso.",
        "Salvar formação",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information)

    End Sub
    Private Sub ExcluirFormacaoPersonalizada()

        Dim formacao As ModeloFormacao =
        FormacaoSelecionada

        If Not FormacaoEhPersonalizada(
        formacao) Then

            MessageBox.Show(
            "Somente formações personalizadas podem ser excluídas.",
            "Excluir formação",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information)

            Exit Sub

        End If

        Dim resposta As DialogResult =
        MessageBox.Show(
            "Deseja excluir a formação personalizada """ &
            formacao.Nome &
            """?",
            "Excluir formação",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2)

        If resposta <> DialogResult.Yes Then
            Exit Sub
        End If

        If Not RepositorioFormacoesPersonalizadas.Remover(
        formacao.Id) Then

            MessageBox.Show(
            "Não foi possível excluir a formação.",
            "Excluir formação",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error)

            Exit Sub

        End If

        CarregarFormacoes()

    End Sub


#End Region

#Region "Eventos"

    Private Sub CmbFormacao_SelectedIndexChanged(
        sender As Object,
        e As EventArgs)

        AtualizarFormacaoSelecionada()

    End Sub

    Private Sub BtnAplicar_Click(
        sender As Object,
        e As EventArgs)

        If FormacaoSelecionada Is Nothing Then

            MessageBox.Show(
                "Selecione uma formação.",
                "Formações táticas",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information)

            Exit Sub

        End If

        DialogResult =
            DialogResult.OK

        Close()

    End Sub

    Private Sub BtnCancelar_Click(
        sender As Object,
        e As EventArgs)

        DialogResult =
            DialogResult.Cancel

        Close()

    End Sub

    Private Sub BtnSalvarAtual_Click(
    sender As Object,
    e As EventArgs)

        SalvarFormacaoAtual()

    End Sub

    Private Sub BtnExcluirPersonalizada_Click(
    sender As Object,
    e As EventArgs)

        ExcluirFormacaoPersonalizada()

    End Sub

#End Region

#Region "Tema"

    Public Sub AplicarTemaAtual()

        BackColor =
            Tema.Fundo

        ForeColor =
            Tema.Texto

        CmbFormacao.BackColor =
            Tema.CampoEntrada

        CmbFormacao.ForeColor =
            Tema.TextoCampo

        LblDescricao.BackColor =
            Tema.Painel

        LblDescricao.ForeColor =
            Tema.Texto

        LvPosicoes.BackColor =
            Tema.CampoEntrada

        LvPosicoes.ForeColor =
            Tema.TextoCampo

        BtnAplicar.BackColor =
            Tema.Painel

        BtnAplicar.ForeColor =
            Tema.Texto

        BtnAplicar.FlatAppearance.BorderColor =
            Tema.Borda

        BtnAplicar.FlatAppearance.MouseOverBackColor =
            Tema.PainelHover

        BtnCancelar.BackColor =
            Tema.Painel

        BtnCancelar.ForeColor =
            Tema.Texto

        BtnCancelar.FlatAppearance.BorderColor =
            Tema.Borda

        BtnCancelar.FlatAppearance.MouseOverBackColor =
            Tema.PainelHover

        BtnSalvarAtual.BackColor =
    Tema.Painel

        BtnSalvarAtual.ForeColor =
            Tema.Texto

        BtnSalvarAtual.FlatAppearance.BorderColor =
            Tema.Borda

        BtnSalvarAtual.FlatAppearance.MouseOverBackColor =
            Tema.PainelHover

        BtnExcluirPersonalizada.BackColor =
            Tema.Painel

        BtnExcluirPersonalizada.ForeColor =
            Tema.Texto

        BtnExcluirPersonalizada.FlatAppearance.BorderColor =
            Tema.Borda

        BtnExcluirPersonalizada.FlatAppearance.MouseOverBackColor =
            Tema.PainelHover

    End Sub

#End Region

End Class