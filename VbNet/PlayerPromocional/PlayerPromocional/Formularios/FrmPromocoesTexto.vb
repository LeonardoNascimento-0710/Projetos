Imports System.Collections.Generic
Imports System.Linq

Public Class FrmPromocoesTexto

#Region "Campos e controles"

    Private ReadOnly promocoes As New List(Of PromocaoTexto)

    Private PnlTopo As Panel
    Private LblTitulo As Label
    Private DgvPromocoes As DataGridView
    Private LblQuantidade As Label

    Private WithEvents BtnAdicionar As Button
    Private WithEvents BtnRemover As Button
    Private WithEvents BtnSubir As Button
    Private WithEvents BtnDescer As Button
    Private WithEvents BtnConcluir As Button
    Private WithEvents BtnCancelar As Button

    Private ChkOrdemAleatoria As CheckBox

#End Region

#Region "Propriedades públicas"

    Public ReadOnly Property PromocoesSelecionadas As List(Of PromocaoTexto)
        Get

            Dim resultado As New List(Of PromocaoTexto)

            For Each promocao As PromocaoTexto In promocoes
                resultado.Add(promocao.Copiar())
            Next

            Return resultado

        End Get
    End Property

    Public ReadOnly Property OrdemAleatoriaSelecionada As Boolean
        Get
            Return ChkOrdemAleatoria.Checked
        End Get
    End Property

#End Region

#Region "Construtores"

    Public Sub New()

        InitializeComponent()

        ConfigurarFormulario()
        CriarControles()

    End Sub

    Public Sub New(
        promocoesExistentes As IEnumerable(Of PromocaoTexto),
        ordemAleatoria As Boolean)

        Me.New()

        ChkOrdemAleatoria.Checked =
        ordemAleatoria

        If promocoesExistentes Is Nothing Then
            Exit Sub
        End If

        For Each promocao As PromocaoTexto In promocoesExistentes

            If promocao Is Nothing OrElse
               String.IsNullOrWhiteSpace(promocao.Texto) Then

                Continue For

            End If

            DgvPromocoes.Rows.Add(
                promocao.Texto,
                NormalizarVoz(promocao.Voz))

        Next

        AtualizarQuantidade()

    End Sub

#End Region

#Region "Montagem da interface"

    Private Sub ConfigurarFormulario()

        Me.SuspendLayout()

        Me.Controls.Clear()

        Me.Name =
        "FrmPromocoesTexto"

        Me.Text =
        "Promoções por texto"

        Me.StartPosition =
        FormStartPosition.CenterParent

        Me.ClientSize =
        New Size(900, 560)

        Me.MinimumSize =
        New Size(820, 500)

        Me.BackColor =
        Color.FromArgb(28, 28, 28)

        Me.Font =
        New Font(
            "Segoe UI",
            10.0F,
            FontStyle.Regular)

        Me.ShowIcon = False

    End Sub

    Private Sub CriarControles()

        CriarCabecalho()
        CriarGrade()
        CriarOpcoes()
        CriarBotoes()

        Me.AcceptButton =
        BtnConcluir

        Me.CancelButton =
        BtnCancelar

        Me.ResumeLayout(False)
        Me.PerformLayout()

        AtualizarQuantidade()

    End Sub

    Private Sub CriarCabecalho()

        PnlTopo =
        New Panel With {
            .Name = "PnlTopo",
            .Dock = DockStyle.Top,
            .Height = 60,
            .BackColor = Color.FromArgb(134, 29, 29)
        }

        LblTitulo =
        New Label With {
            .Name = "LblTitulo",
            .Text = "PROMOÇÕES POR TEXTO",
            .AutoSize = True,
            .Location = New Point(20, 16),
            .ForeColor = Color.White,
            .BackColor = Color.Transparent,
            .Font = New Font(
                "Segoe UI",
                17.0F,
                FontStyle.Bold)
        }

        PnlTopo.Controls.Add(
        LblTitulo)

        Me.Controls.Add(
        PnlTopo)

    End Sub

    Private Sub CriarGrade()

        DgvPromocoes =
        New DataGridView With {
            .Name = "DgvPromocoes",
            .Location = New Point(20, 80),
            .Size = New Size(860, 350),
            .Anchor = AnchorStyles.Top Or
                      AnchorStyles.Bottom Or
                      AnchorStyles.Left Or
                      AnchorStyles.Right,
            .BackgroundColor = Color.FromArgb(38, 38, 38),
            .BorderStyle = BorderStyle.FixedSingle,
            .AllowUserToAddRows = True,
            .AllowUserToDeleteRows = True,
            .AllowUserToResizeRows = False,
            .AutoGenerateColumns = False,
            .RowHeadersVisible = False,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .MultiSelect = True,
            .EditMode = DataGridViewEditMode.EditOnEnter
        }

        DgvPromocoes.DefaultCellStyle.BackColor =
        Color.FromArgb(48, 48, 48)

        DgvPromocoes.DefaultCellStyle.ForeColor =
        Color.White

        DgvPromocoes.DefaultCellStyle.SelectionBackColor =
        Color.FromArgb(134, 29, 29)

        DgvPromocoes.DefaultCellStyle.SelectionForeColor =
        Color.White

        DgvPromocoes.ColumnHeadersDefaultCellStyle.BackColor =
        Color.FromArgb(55, 55, 55)

        DgvPromocoes.ColumnHeadersDefaultCellStyle.ForeColor =
        Color.White

        DgvPromocoes.EnableHeadersVisualStyles = False

        Dim colunaTexto As New DataGridViewTextBoxColumn With {
            .Name = "ColTexto",
            .HeaderText = "Texto da promoção",
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            .MinimumWidth = 450,
            .MaxInputLength = 500
        }

        Dim colunaVoz As New DataGridViewComboBoxColumn With {
            .Name = "ColVoz",
            .HeaderText = "Voz",
            .Width = 150,
            .FlatStyle = FlatStyle.Flat,
            .DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
        }

        colunaVoz.Items.Add(
        PromocaoTexto.VozMasculina)

        colunaVoz.Items.Add(
        PromocaoTexto.VozFeminina)

        DgvPromocoes.Columns.Add(
        colunaTexto)

        DgvPromocoes.Columns.Add(
        colunaVoz)

        AddHandler DgvPromocoes.RowsAdded,
        AddressOf DgvPromocoes_RowsAdded

        AddHandler DgvPromocoes.RowsRemoved,
        AddressOf DgvPromocoes_RowsRemoved

        Me.Controls.Add(
        DgvPromocoes)

    End Sub

    Private Sub CriarOpcoes()

        ChkOrdemAleatoria =
        New CheckBox With {
            .Name = "ChkOrdemAleatoria",
            .Text = "Passar pelas promoções em ordem aleatória, sem repetir até completar a lista",
            .AutoSize = True,
            .Location = New Point(20, 445),
            .Anchor = AnchorStyles.Bottom Or AnchorStyles.Left,
            .ForeColor = Color.White,
            .BackColor = Color.Transparent
        }

        LblQuantidade =
        New Label With {
            .Name = "LblQuantidade",
            .Text = "Nenhuma promoção cadastrada",
            .Location = New Point(20, 475),
            .Size = New Size(420, 24),
            .Anchor = AnchorStyles.Bottom Or AnchorStyles.Left,
            .ForeColor = Color.FromArgb(190, 190, 190),
            .BackColor = Color.Transparent
        }

        Me.Controls.Add(
        ChkOrdemAleatoria)

        Me.Controls.Add(
        LblQuantidade)

    End Sub

    Private Sub CriarBotoes()

        BtnAdicionar =
        CriarBotao(
            "BtnAdicionar",
            "Adicionar promoção",
            New Point(20, 510),
            New Size(170, 36))

        BtnRemover =
        CriarBotao(
            "BtnRemover",
            "Remover selecionadas",
            New Point(200, 510),
            New Size(185, 36))

        BtnSubir =
        CriarBotao(
            "BtnSubir",
            "Subir",
            New Point(395, 510),
            New Size(90, 36))

        BtnDescer =
        CriarBotao(
            "BtnDescer",
            "Descer",
            New Point(495, 510),
            New Size(90, 36))

        BtnConcluir =
        CriarBotaoDestaque(
            "BtnConcluir",
            "Concluir",
            New Point(680, 510),
            New Size(95, 36))

        BtnCancelar =
        CriarBotao(
            "BtnCancelar",
            "Cancelar",
            New Point(785, 510),
            New Size(95, 36))

        BtnAdicionar.Anchor =
        AnchorStyles.Bottom Or AnchorStyles.Left

        BtnRemover.Anchor =
        AnchorStyles.Bottom Or AnchorStyles.Left

        BtnSubir.Anchor =
        AnchorStyles.Bottom Or AnchorStyles.Left

        BtnDescer.Anchor =
        AnchorStyles.Bottom Or AnchorStyles.Left

        BtnConcluir.Anchor =
        AnchorStyles.Bottom Or AnchorStyles.Right

        BtnCancelar.Anchor =
        AnchorStyles.Bottom Or AnchorStyles.Right

        Me.Controls.AddRange(
        New Control() {
            BtnAdicionar,
            BtnRemover,
            BtnSubir,
            BtnDescer,
            BtnConcluir,
            BtnCancelar
        })

    End Sub

    Private Function CriarBotao(
        nome As String,
        texto As String,
        localizacao As Point,
        tamanho As Size) As Button

        Return New Button With {
            .Name = nome,
            .Text = texto,
            .Location = localizacao,
            .Size = tamanho,
            .BackColor = Color.FromArgb(50, 50, 50),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Cursor = Cursors.Hand
        }

    End Function

    Private Function CriarBotaoDestaque(
        nome As String,
        texto As String,
        localizacao As Point,
        tamanho As Size) As Button

        Dim botao As Button =
        CriarBotao(
            nome,
            texto,
            localizacao,
            tamanho)

        botao.BackColor =
        Color.FromArgb(134, 29, 29)

        Return botao

    End Function

#End Region

#Region "Eventos"

    Private Sub BtnAdicionar_Click(
        sender As Object,
        e As EventArgs
    ) Handles BtnAdicionar.Click

        Dim indice As Integer =
        DgvPromocoes.Rows.Add(
            "",
            PromocaoTexto.VozMasculina)

        DgvPromocoes.CurrentCell =
        DgvPromocoes.Rows(indice).Cells("ColTexto")

        DgvPromocoes.BeginEdit(True)

    End Sub

    Private Sub BtnRemover_Click(
        sender As Object,
        e As EventArgs
    ) Handles BtnRemover.Click

        Dim indices As New List(Of Integer)

        For Each linha As DataGridViewRow In DgvPromocoes.SelectedRows

            If Not linha.IsNewRow Then
                indices.Add(linha.Index)
            End If

        Next

        indices.Sort()
        indices.Reverse()

        For Each indice As Integer In indices
            DgvPromocoes.Rows.RemoveAt(indice)
        Next

        AtualizarQuantidade()

    End Sub

    Private Sub BtnSubir_Click(
        sender As Object,
        e As EventArgs
    ) Handles BtnSubir.Click

        MoverLinha(-1)

    End Sub

    Private Sub BtnDescer_Click(
        sender As Object,
        e As EventArgs
    ) Handles BtnDescer.Click

        MoverLinha(1)

    End Sub

    Private Sub BtnConcluir_Click(
        sender As Object,
        e As EventArgs
    ) Handles BtnConcluir.Click

        DgvPromocoes.EndEdit()

        promocoes.Clear()

        For Each linha As DataGridViewRow In DgvPromocoes.Rows

            If linha.IsNewRow Then
                Continue For
            End If

            Dim texto As String =
            Convert.ToString(
                linha.Cells("ColTexto").Value).Trim()

            If texto = "" Then
                Continue For
            End If

            Dim voz As String =
            NormalizarVoz(
                Convert.ToString(
                    linha.Cells("ColVoz").Value))

            promocoes.Add(
            New PromocaoTexto With {
                .Texto = texto,
                .Voz = voz
            })

        Next

        If promocoes.Count = 0 Then

            MessageBox.Show(
                "Adicione pelo menos uma promoção com texto.",
                "Promoções",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning)

            Exit Sub

        End If

        Me.DialogResult =
        DialogResult.OK

        Me.Close()

    End Sub

    Private Sub BtnCancelar_Click(
        sender As Object,
        e As EventArgs
    ) Handles BtnCancelar.Click

        Me.DialogResult =
        DialogResult.Cancel

        Me.Close()

    End Sub

    Private Sub DgvPromocoes_RowsAdded(
        sender As Object,
        e As DataGridViewRowsAddedEventArgs)

        AtualizarQuantidade()

    End Sub

    Private Sub DgvPromocoes_RowsRemoved(
        sender As Object,
        e As DataGridViewRowsRemovedEventArgs)

        AtualizarQuantidade()

    End Sub

#End Region

#Region "Auxiliares"

    Private Sub MoverLinha(
        direcao As Integer)

        If DgvPromocoes.CurrentRow Is Nothing OrElse
           DgvPromocoes.CurrentRow.IsNewRow Then

            Exit Sub

        End If

        Dim indiceAtual As Integer =
        DgvPromocoes.CurrentRow.Index

        Dim novoIndice As Integer =
        indiceAtual + direcao

        Dim quantidadeLinhasValidas As Integer =
        DgvPromocoes.Rows.Count - 1

        If novoIndice < 0 OrElse
           novoIndice >= quantidadeLinhasValidas Then

            Exit Sub

        End If

        Dim texto As Object =
        DgvPromocoes.Rows(indiceAtual).Cells("ColTexto").Value

        Dim voz As Object =
        DgvPromocoes.Rows(indiceAtual).Cells("ColVoz").Value

        DgvPromocoes.Rows.RemoveAt(
        indiceAtual)

        DgvPromocoes.Rows.Insert(
            novoIndice,
            texto,
            NormalizarVoz(Convert.ToString(voz)))

        DgvPromocoes.CurrentCell =
        DgvPromocoes.Rows(novoIndice).Cells("ColTexto")

        DgvPromocoes.Rows(novoIndice).Selected =
        True

    End Sub

    Private Function NormalizarVoz(
        voz As String) As String

        If String.Equals(
            voz,
            PromocaoTexto.VozFeminina,
            StringComparison.OrdinalIgnoreCase) Then

            Return PromocaoTexto.VozFeminina

        End If

        Return PromocaoTexto.VozMasculina

    End Function

    Private Sub AtualizarQuantidade()

        If DgvPromocoes Is Nothing OrElse
           LblQuantidade Is Nothing Then

            Exit Sub

        End If

        Dim quantidade As Integer =
        DgvPromocoes.Rows.Cast(Of DataGridViewRow)().Count(
            Function(linha)
                Return Not linha.IsNewRow AndAlso
                       Not String.IsNullOrWhiteSpace(
                           Convert.ToString(
                               linha.Cells("ColTexto").Value))
            End Function)

        If quantidade = 0 Then

            LblQuantidade.Text =
            "Nenhuma promoção cadastrada"

        ElseIf quantidade = 1 Then

            LblQuantidade.Text =
            "1 promoção cadastrada"

        Else

            LblQuantidade.Text =
            quantidade.ToString() &
            " promoções cadastradas"

        End If

    End Sub

#End Region

End Class
