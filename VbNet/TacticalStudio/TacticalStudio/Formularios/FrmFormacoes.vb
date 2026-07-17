Imports TacticalStudio.Core

Public Class FrmFormacoes

    Inherits Form

#Region "Variáveis"

    Private ReadOnly _formacoes As New List(Of ModeloFormacao)()

    Private ReadOnly CmbFormacaoMeuTime As New ComboBox()

    Private ReadOnly CmbFormacaoAdversario As New ComboBox()

    Private ReadOnly ChkAplicarMeuTime As New CheckBox()

    Private ReadOnly ChkAplicarAdversario As New CheckBox()

    Private ReadOnly LblDescricao As New Label()

    Private ReadOnly LvPosicoes As New ListView()

    Private ReadOnly BtnAplicar As New Button()

    Private ReadOnly BtnCancelar As New Button()

    Private ReadOnly BtnSalvarAtual As New Button()

    Private ReadOnly BtnExcluirPersonalizada As New Button()

    Private ReadOnly _campo As CampoTatico

    Private _comboAtivo As ComboBox

#End Region

#Region "Propriedades"

    <System.ComponentModel.Browsable(False)>
    <System.ComponentModel.DesignerSerializationVisibility(
        System.ComponentModel.DesignerSerializationVisibility.Hidden)>
    Public Property FormacaoSelecionada As ModeloFormacao

    <System.ComponentModel.Browsable(False)>
    <System.ComponentModel.DesignerSerializationVisibility(
        System.ComponentModel.DesignerSerializationVisibility.Hidden)>
    Public Property FormacaoMeuTimeSelecionada As ModeloFormacao

    <System.ComponentModel.Browsable(False)>
    <System.ComponentModel.DesignerSerializationVisibility(
        System.ComponentModel.DesignerSerializationVisibility.Hidden)>
    Public Property FormacaoAdversarioSelecionada As ModeloFormacao

    Public ReadOnly Property AplicarMeuTime As Boolean
        Get
            Return ChkAplicarMeuTime.Checked
        End Get
    End Property

    Public ReadOnly Property AplicarAdversario As Boolean
        Get
            Return ChkAplicarAdversario.Checked
        End Get
    End Property

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
                900,
                590)

        MinimumSize =
            New Size(
                916,
                629)

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
            .RowCount = 4,
            .Padding = New Padding(14),
            .Margin = New Padding(0)
        }

        painelPrincipal.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                155.0F))

        painelPrincipal.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                78.0F))

        painelPrincipal.RowStyles.Add(
            New RowStyle(
                SizeType.Percent,
                100.0F))

        painelPrincipal.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                54.0F))

        Controls.Add(
            painelPrincipal)

        Dim painelEquipes As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 2,
            .RowCount = 1,
            .Margin = New Padding(0, 0, 0, 10),
            .Padding = New Padding(0)
        }

        painelEquipes.ColumnStyles.Add(
            New ColumnStyle(
                SizeType.Percent,
                50.0F))

        painelEquipes.ColumnStyles.Add(
            New ColumnStyle(
                SizeType.Percent,
                50.0F))

        painelEquipes.Controls.Add(
            CriarCartaoEquipe(
                "MEU TIME",
                "Defende o gol esquerdo e ataca para a direita.",
                ChkAplicarMeuTime,
                CmbFormacaoMeuTime,
                New Padding(0, 0, 7, 0)),
            0,
            0)

        painelEquipes.Controls.Add(
            CriarCartaoEquipe(
                "ADVERSÁRIO",
                "Defende o gol direito e ataca para a esquerda.",
                ChkAplicarAdversario,
                CmbFormacaoAdversario,
                New Padding(7, 0, 0, 0)),
            1,
            0)

        painelPrincipal.Controls.Add(
            painelEquipes,
            0,
            0)

        LblDescricao.Dock =
            DockStyle.Fill

        LblDescricao.Margin =
            New Padding(
                0,
                0,
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
            1)

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
            320)

        LvPosicoes.Columns.Add(
            "Horizontal",
            130)

        LvPosicoes.Columns.Add(
            "Vertical",
            130)

        painelPrincipal.Controls.Add(
            LvPosicoes,
            0,
            2)

        Dim painelBotoes As New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.RightToLeft,
            .WrapContents = False,
            .Padding = New Padding(0, 10, 0, 0),
            .Margin = New Padding(0)
        }

        ConfigurarBotao(
            BtnAplicar,
            "Aplicar formações",
            150)

        AddHandler BtnAplicar.Click,
            AddressOf BtnAplicar_Click

        painelBotoes.Controls.Add(
            BtnAplicar)

        ConfigurarBotao(
            BtnCancelar,
            "Cancelar",
            105)

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
            3)

        AddHandler CmbFormacaoMeuTime.SelectedIndexChanged,
            AddressOf CmbFormacaoMeuTime_SelectedIndexChanged

        AddHandler CmbFormacaoAdversario.SelectedIndexChanged,
            AddressOf CmbFormacaoAdversario_SelectedIndexChanged

        AddHandler CmbFormacaoMeuTime.Enter,
            Sub(sender, e)

                _comboAtivo =
                    CmbFormacaoMeuTime

                AtualizarSelecoesEPreview(
                    CmbFormacaoMeuTime)

            End Sub

        AddHandler CmbFormacaoAdversario.Enter,
            Sub(sender, e)

                _comboAtivo =
                    CmbFormacaoAdversario

                AtualizarSelecoesEPreview(
                    CmbFormacaoAdversario)

            End Sub

        AddHandler ChkAplicarMeuTime.CheckedChanged,
            Sub(sender, e)

                AtualizarEstadoAplicacao()

            End Sub

        AddHandler ChkAplicarAdversario.CheckedChanged,
            Sub(sender, e)

                AtualizarEstadoAplicacao()

            End Sub

    End Sub

    Private Function CriarCartaoEquipe(
        titulo As String,
        descricao As String,
        checkBoxAplicar As CheckBox,
        comboFormacao As ComboBox,
        margem As Padding
    ) As Panel

        Dim cartao As New Panel With {
            .Dock = DockStyle.Fill,
            .Margin = margem,
            .Padding = New Padding(12),
            .BorderStyle = BorderStyle.FixedSingle
        }

        Dim layout As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 1,
            .RowCount = 4,
            .Margin = New Padding(0),
            .Padding = New Padding(0)
        }

        layout.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                28.0F))

        layout.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                30.0F))

        layout.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                40.0F))

        layout.RowStyles.Add(
            New RowStyle(
                SizeType.Percent,
                100.0F))

        cartao.Controls.Add(
            layout)

        Dim labelTitulo As New Label With {
            .Text = titulo,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .Font = New Font(
                "Segoe UI",
                10.0F,
                FontStyle.Bold),
            .TextAlign = ContentAlignment.MiddleLeft
        }

        layout.Controls.Add(
            labelTitulo,
            0,
            0)

        checkBoxAplicar.Text =
            "Aplicar esta formação"

        checkBoxAplicar.Dock =
            DockStyle.Fill

        checkBoxAplicar.Margin =
            New Padding(0)

        checkBoxAplicar.Checked =
            True

        layout.Controls.Add(
            checkBoxAplicar,
            0,
            1)

        comboFormacao.Dock =
            DockStyle.Fill

        comboFormacao.Margin =
            New Padding(0, 4, 0, 4)

        comboFormacao.DropDownStyle =
            ComboBoxStyle.DropDownList

        comboFormacao.FlatStyle =
            FlatStyle.Flat

        layout.Controls.Add(
            comboFormacao,
            0,
            2)

        Dim labelDescricao As New Label With {
            .Text = descricao,
            .Dock = DockStyle.Fill,
            .Margin = New Padding(0),
            .TextAlign = ContentAlignment.MiddleLeft
        }

        layout.Controls.Add(
            labelDescricao,
            0,
            3)

        Return cartao

    End Function

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
            New Padding(6, 0, 0, 0)

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

        CmbFormacaoMeuTime.Items.Clear()

        CmbFormacaoAdversario.Items.Clear()

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

        If _formacoes.Count = 0 Then

            FormacaoSelecionada =
                Nothing

            FormacaoMeuTimeSelecionada =
                Nothing

            FormacaoAdversarioSelecionada =
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
            ObterIndiceFormacao(
                idSelecionar)

        If indiceSelecionar < 0 Then

            indiceSelecionar =
                ObterIndiceFormacao(
                    "4-3-3")

        End If

        If indiceSelecionar < 0 Then
            indiceSelecionar = 0
        End If

        CmbFormacaoMeuTime.SelectedIndex =
            indiceSelecionar

        CmbFormacaoAdversario.SelectedIndex =
            indiceSelecionar

        _comboAtivo =
            CmbFormacaoMeuTime

        AtualizarSelecoesEPreview(
            CmbFormacaoMeuTime)

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

        CmbFormacaoMeuTime.Items.Add(
            nomeExibicao)

        CmbFormacaoAdversario.Items.Add(
            nomeExibicao)

    End Sub

    Private Function ObterIndiceFormacao(
        idFormacao As String
    ) As Integer

        If String.IsNullOrWhiteSpace(
            idFormacao) Then

            Return -1

        End If

        For indice As Integer =
            0 To _formacoes.Count - 1

            If String.Equals(
                _formacoes(indice).Id,
                idFormacao,
                StringComparison.OrdinalIgnoreCase) Then

                Return indice

            End If

        Next

        Return -1

    End Function

    Private Function ObterFormacaoCombo(
        combo As ComboBox
    ) As ModeloFormacao

        If combo Is Nothing Then
            Return Nothing
        End If

        Dim indice As Integer =
            combo.SelectedIndex

        If indice < 0 OrElse
           indice >= _formacoes.Count Then

            Return Nothing

        End If

        Return _formacoes(indice)

    End Function

    Private Sub AtualizarSelecoesEPreview(
        comboOrigem As ComboBox)

        FormacaoMeuTimeSelecionada =
            ObterFormacaoCombo(
                CmbFormacaoMeuTime)

        FormacaoAdversarioSelecionada =
            ObterFormacaoCombo(
                CmbFormacaoAdversario)

        If comboOrigem Is Nothing Then

            comboOrigem =
                _comboAtivo

        End If

        If comboOrigem Is Nothing Then

            comboOrigem =
                CmbFormacaoMeuTime

        End If

        _comboAtivo =
            comboOrigem

        FormacaoSelecionada =
            ObterFormacaoCombo(
                comboOrigem)

        If FormacaoSelecionada Is Nothing Then

            LblDescricao.Text =
                String.Empty

            LvPosicoes.Items.Clear()

        Else

            Dim ladoTexto As String

            If comboOrigem Is
               CmbFormacaoAdversario Then

                ladoTexto =
                    "Prévia do adversário — defesa no lado direito."

            Else

                ladoTexto =
                    "Prévia do meu time — defesa no lado esquerdo."

            End If

            LblDescricao.Text =
                ladoTexto &
                Environment.NewLine &
                FormacaoSelecionada.Descricao

            AtualizarListaPosicoes(
                FormacaoSelecionada)

        End If

        BtnExcluirPersonalizada.Enabled =
            FormacaoEhPersonalizada(
                FormacaoSelecionada)

        AtualizarEstadoAplicacao()

    End Sub

    Private Sub AtualizarEstadoAplicacao()

        Dim meuTimeValido As Boolean =
            ChkAplicarMeuTime.Checked AndAlso
            FormacaoMeuTimeSelecionada IsNot Nothing

        Dim adversarioValido As Boolean =
            ChkAplicarAdversario.Checked AndAlso
            FormacaoAdversarioSelecionada IsNot Nothing

        BtnAplicar.Enabled =
            meuTimeValido OrElse
            adversarioValido

        CmbFormacaoMeuTime.Enabled =
            ChkAplicarMeuTime.Checked

        CmbFormacaoAdversario.Enabled =
            ChkAplicarAdversario.Checked

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
        formacao As ModeloFormacao
    ) As Boolean

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

        If resposta <>
           DialogResult.Yes Then

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

    Private Sub CmbFormacaoMeuTime_SelectedIndexChanged(
        sender As Object,
        e As EventArgs)

        AtualizarSelecoesEPreview(
            CmbFormacaoMeuTime)

    End Sub

    Private Sub CmbFormacaoAdversario_SelectedIndexChanged(
        sender As Object,
        e As EventArgs)

        AtualizarSelecoesEPreview(
            CmbFormacaoAdversario)

    End Sub

    Private Sub BtnAplicar_Click(
        sender As Object,
        e As EventArgs)

        If Not AplicarMeuTime AndAlso
           Not AplicarAdversario Then

            MessageBox.Show(
                "Marque pelo menos uma equipe para aplicar a formação.",
                "Formações táticas",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information)

            Exit Sub

        End If

        If AplicarMeuTime AndAlso
           FormacaoMeuTimeSelecionada Is Nothing Then

            MessageBox.Show(
                "Selecione a formação do seu time.",
                "Formações táticas",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information)

            Exit Sub

        End If

        If AplicarAdversario AndAlso
           FormacaoAdversarioSelecionada Is Nothing Then

            MessageBox.Show(
                "Selecione a formação do adversário.",
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

        AplicarTemaRecursivo(
            Me)

        CmbFormacaoMeuTime.BackColor =
            Tema.CampoEntrada

        CmbFormacaoMeuTime.ForeColor =
            Tema.TextoCampo

        CmbFormacaoAdversario.BackColor =
            Tema.CampoEntrada

        CmbFormacaoAdversario.ForeColor =
            Tema.TextoCampo

        LblDescricao.BackColor =
            Tema.Painel

        LblDescricao.ForeColor =
            Tema.Texto

        LvPosicoes.BackColor =
            Tema.CampoEntrada

        LvPosicoes.ForeColor =
            Tema.TextoCampo

        ConfigurarTemaBotao(
            BtnAplicar)

        ConfigurarTemaBotao(
            BtnCancelar)

        ConfigurarTemaBotao(
            BtnSalvarAtual)

        ConfigurarTemaBotao(
            BtnExcluirPersonalizada)

    End Sub

    Private Sub AplicarTemaRecursivo(
        controlePai As Control)

        If controlePai Is Nothing Then
            Exit Sub
        End If

        For Each controle As Control
            In controlePai.Controls

            If TypeOf controle Is Panel OrElse
               TypeOf controle Is TableLayoutPanel OrElse
               TypeOf controle Is FlowLayoutPanel Then

                controle.BackColor =
                    Tema.Painel

            End If

            If TypeOf controle Is Label OrElse
               TypeOf controle Is CheckBox Then

                controle.ForeColor =
                    Tema.Texto

            End If

            If controle.HasChildren Then

                AplicarTemaRecursivo(
                    controle)

            End If

        Next

    End Sub

    Private Sub ConfigurarTemaBotao(
        botao As Button)

        botao.BackColor =
            Tema.Painel

        botao.ForeColor =
            Tema.Texto

        botao.FlatAppearance.BorderColor =
            Tema.Borda

        botao.FlatAppearance.MouseOverBackColor =
            Tema.PainelHover

    End Sub

#End Region

End Class
