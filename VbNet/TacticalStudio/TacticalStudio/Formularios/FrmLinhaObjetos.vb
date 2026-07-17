Imports TacticalStudio.Core.Enums

Public Class FrmLinhaObjetos
    Inherits Form

    Private ReadOnly _cmbTipo As ComboBox
    Private ReadOnly _nudQuantidade As NumericUpDown
    Private ReadOnly _cmbOrientacao As ComboBox
    Private ReadOnly _nudEscala As NumericUpDown
    Private ReadOnly _chkAgrupar As CheckBox

    Public ReadOnly Property TipoSelecionado As TipoObjetoLinha
        Get
            Return CType(
                Math.Max(0, _cmbTipo.SelectedIndex),
                TipoObjetoLinha)
        End Get
    End Property

    Public ReadOnly Property QuantidadeSelecionada As Integer
        Get
            Return CInt(_nudQuantidade.Value)
        End Get
    End Property

    Public ReadOnly Property OrientacaoSelecionada As OrientacaoLinhaObjetos
        Get
            Return CType(
                Math.Max(0, _cmbOrientacao.SelectedIndex),
                OrientacaoLinhaObjetos)
        End Get
    End Property

    Public ReadOnly Property EscalaSelecionada As Single
        Get
            Return CSng(_nudEscala.Value)
        End Get
    End Property

    Public ReadOnly Property AgruparAutomaticamente As Boolean
        Get
            Return _chkAgrupar.Checked
        End Get
    End Property

    Public Sub New()

        Text = "Linha de objetos"
        StartPosition = FormStartPosition.CenterParent
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        ShowInTaskbar = False
        ClientSize = New Size(430, 500)
        BackColor = Tema.Fundo
        ForeColor = Tema.Texto
        Font = New Font("Segoe UI", 9.0F)

        Dim painelPrincipal As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 1,
            .RowCount = 8,
            .Padding = New Padding(18),
            .BackColor = Tema.Fundo
        }

        painelPrincipal.ColumnStyles.Add(
            New ColumnStyle(SizeType.Percent, 100.0F))

        painelPrincipal.RowStyles.Add(New RowStyle(SizeType.Absolute, 42.0F))
        painelPrincipal.RowStyles.Add(New RowStyle(SizeType.Absolute, 64.0F))
        painelPrincipal.RowStyles.Add(New RowStyle(SizeType.Absolute, 64.0F))
        painelPrincipal.RowStyles.Add(New RowStyle(SizeType.Absolute, 64.0F))
        painelPrincipal.RowStyles.Add(New RowStyle(SizeType.Absolute, 64.0F))
        painelPrincipal.RowStyles.Add(New RowStyle(SizeType.Absolute, 42.0F))
        painelPrincipal.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        painelPrincipal.RowStyles.Add(New RowStyle(SizeType.Absolute, 46.0F))

        Dim titulo As New Label With {
            .Text = "CRIAR LINHA DE OBJETOS",
            .Dock = DockStyle.Fill,
            .ForeColor = Tema.CorPrimaria,
            .Font = New Font("Segoe UI", 11.0F, FontStyle.Bold),
            .TextAlign = ContentAlignment.MiddleLeft
        }

        painelPrincipal.Controls.Add(titulo, 0, 0)

        _cmbTipo = New ComboBox With {
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Tema.CampoEntrada,
            .ForeColor = Tema.TextoCampo
        }

        _cmbTipo.Items.AddRange(
            New Object() {
                "Cones",
                "Manequins / barreira",
                "Bolas",
                "Marcadores",
                "Jogadores"
            })
        _cmbTipo.SelectedIndex = 0

        painelPrincipal.Controls.Add(
            CriarCampo("Tipo de objeto", _cmbTipo),
            0,
            1)

        _nudQuantidade = New NumericUpDown With {
            .Minimum = 2D,
            .Maximum = 30D,
            .Value = 7D,
            .TextAlign = HorizontalAlignment.Center,
            .BackColor = Tema.CampoEntrada,
            .ForeColor = Tema.TextoCampo
        }

        painelPrincipal.Controls.Add(
            CriarCampo("Quantidade", _nudQuantidade),
            0,
            2)

        _cmbOrientacao = New ComboBox With {
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Tema.CampoEntrada,
            .ForeColor = Tema.TextoCampo
        }

        _cmbOrientacao.Items.AddRange(
            New Object() {
                "Livre — horizontal, vertical ou diagonal",
                "Horizontal",
                "Vertical"
            })
        _cmbOrientacao.SelectedIndex = 0

        painelPrincipal.Controls.Add(
            CriarCampo("Orientação", _cmbOrientacao),
            0,
            3)

        _nudEscala = New NumericUpDown With {
            .Minimum = 0.5D,
            .Maximum = 2.5D,
            .Increment = 0.1D,
            .DecimalPlaces = 1,
            .Value = 1.0D,
            .TextAlign = HorizontalAlignment.Center,
            .BackColor = Tema.CampoEntrada,
            .ForeColor = Tema.TextoCampo
        }

        painelPrincipal.Controls.Add(
            CriarCampo("Tamanho visual", _nudEscala),
            0,
            4)

        _chkAgrupar = New CheckBox With {
            .Text = "Agrupar os objetos automaticamente",
            .Checked = True,
            .Dock = DockStyle.Fill,
            .ForeColor = Tema.Texto,
            .BackColor = Tema.Fundo,
            .TextAlign = ContentAlignment.MiddleLeft
        }

        painelPrincipal.Controls.Add(_chkAgrupar, 0, 5)

        Dim instrucao As New Label With {
            .Text =
                "Depois de confirmar, clique no ponto inicial da linha e " &
                "depois no ponto final. No modo Livre, o segundo ponto " &
                "define qualquer inclinação diagonal.",
            .Dock = DockStyle.Fill,
            .ForeColor = Tema.TextoSecundario,
            .Font = New Font("Segoe UI", 8.5F, FontStyle.Italic),
            .TextAlign = ContentAlignment.TopLeft
        }

        painelPrincipal.Controls.Add(instrucao, 0, 6)

        Dim botoes As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 2,
            .RowCount = 1,
            .Margin = New Padding(0),
            .BackColor = Tema.Fundo
        }

        botoes.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0F))
        botoes.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0F))

        Dim btnCancelar As Button =
            CriarBotao("Cancelar", False)

        btnCancelar.DialogResult = DialogResult.Cancel
        btnCancelar.Margin = New Padding(0, 4, 5, 0)

        Dim btnConfirmar As Button =
            CriarBotao("Criar linha", True)

        btnConfirmar.DialogResult = DialogResult.OK
        btnConfirmar.Margin = New Padding(5, 4, 0, 0)

        botoes.Controls.Add(btnCancelar, 0, 0)
        botoes.Controls.Add(btnConfirmar, 1, 0)
        painelPrincipal.Controls.Add(botoes, 0, 7)

        Controls.Add(painelPrincipal)

        AcceptButton = btnConfirmar
        CancelButton = btnCancelar

    End Sub

    Private Function CriarCampo(
        titulo As String,
        controle As Control) As Control

        Dim painel As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 1,
            .RowCount = 2,
            .Margin = New Padding(0, 3, 0, 3),
            .BackColor = Tema.Fundo
        }

        painel.RowStyles.Add(New RowStyle(SizeType.Absolute, 24.0F))
        painel.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        Dim label As New Label With {
            .Text = titulo,
            .Dock = DockStyle.Fill,
            .ForeColor = Tema.TextoSecundario,
            .TextAlign = ContentAlignment.MiddleLeft
        }

        controle.Dock = DockStyle.Fill
        controle.Margin = New Padding(0)

        painel.Controls.Add(label, 0, 0)
        painel.Controls.Add(controle, 0, 1)

        Return painel

    End Function

    Private Function CriarBotao(
        texto As String,
        destaque As Boolean) As Button

        Dim botao As New Button With {
            .Text = texto,
            .Dock = DockStyle.Fill,
            .FlatStyle = FlatStyle.Flat,
            .Cursor = Cursors.Hand,
            .UseVisualStyleBackColor = False,
            .BackColor = If(destaque, Tema.CorPrimaria, Tema.Painel),
            .ForeColor = If(destaque, Tema.TextoSobreCorPrimaria, Tema.Texto)
        }

        botao.FlatAppearance.BorderColor =
            If(
                destaque,
                Tema.TextoSobreCorPrimaria,
                Tema.Borda)

        botao.FlatAppearance.MouseOverBackColor =
            If(
                destaque,
                Tema.CorPrimariaHover,
                Tema.PainelHover)

        Return botao

    End Function

End Class
