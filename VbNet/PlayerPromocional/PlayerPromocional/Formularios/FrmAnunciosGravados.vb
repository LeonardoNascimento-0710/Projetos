Imports System.IO
Imports System.Linq
Imports System.Collections.Generic

Public Class FrmAnunciosGravados

#Region "Campos e controles"

    Private ReadOnly caminhosAnuncios As New List(Of String)

    Private Shared ReadOnly extensoesPermitidas As New HashSet(Of String)(
        StringComparer.OrdinalIgnoreCase) From {
            ".mp3",
            ".wav",
            ".wma",
            ".aac",
            ".m4a"
        }

    Private PnlTopo As Panel
    Private LblTitulo As Label

    Private LstAnuncios As ListBox
    Private LblQuantidade As Label

    Private WithEvents BtnAdicionar As Button
    Private WithEvents BtnRemover As Button
    Private WithEvents BtnLimpar As Button
    Private WithEvents BtnConcluir As Button
    Private WithEvents BtnCancelar As Button

    Private ChkOrdemAleatoria As CheckBox

#End Region

#Region "Propriedades públicas"

    Public ReadOnly Property AnunciosSelecionados As List(Of String)
        Get
            Return New List(Of String)(
                caminhosAnuncios)
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
        anunciosExistentes As IEnumerable(Of String),
        ordemAleatoria As Boolean)

        Me.New()

        ChkOrdemAleatoria.Checked =
            ordemAleatoria

        If anunciosExistentes Is Nothing Then
            Exit Sub
        End If

        For Each caminho As String In anunciosExistentes

            AdicionarCaminho(
                caminho)

        Next

        AtualizarQuantidade()

    End Sub

#End Region

#Region "Montagem da interface"

    Private Sub ConfigurarFormulario()

        Me.SuspendLayout()

        Me.Controls.Clear()

        Me.Name =
            "FrmAnunciosGravados"

        Me.Text =
            "Anúncios gravados"

        Me.StartPosition =
            FormStartPosition.CenterParent

        Me.ClientSize =
            New Size(700, 500)

        Me.FormBorderStyle =
            FormBorderStyle.FixedSingle

        Me.MaximizeBox = False
        Me.MinimizeBox = False

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
        CriarLista()
        CriarBotoes()
        CriarOpcoes()

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
                .Text = "ANÚNCIOS GRAVADOS",
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

    Private Sub CriarLista()

        LstAnuncios =
            New ListBox With {
                .Name = "LstAnuncios",
                .Location = New Point(20, 80),
                .Size = New Size(660, 280),
                .BackColor = Color.FromArgb(45, 45, 45),
                .ForeColor = Color.White,
                .BorderStyle = BorderStyle.FixedSingle,
                .Font = New Font(
                    "Segoe UI",
                    10.0F,
                    FontStyle.Regular),
                .SelectionMode = SelectionMode.MultiExtended,
                .HorizontalScrollbar = True,
                .IntegralHeight = False
            }

        LblQuantidade =
            New Label With {
                .Name = "LblQuantidade",
                .Text = "Nenhum anúncio adicionado",
                .Location = New Point(20, 370),
                .Size = New Size(660, 22),
                .ForeColor = Color.FromArgb(190, 190, 190),
                .BackColor = Color.Transparent,
                .Font = New Font(
                    "Segoe UI",
                    9.0F,
                    FontStyle.Regular)
            }

        Me.Controls.Add(
            LstAnuncios)

        Me.Controls.Add(
            LblQuantidade)

    End Sub

    Private Sub CriarBotoes()

        BtnAdicionar =
            New Button With {
                .Name = "BtnAdicionar",
                .Text = "Adicionar áudios",
                .Location = New Point(20, 400),
                .Size = New Size(150, 38)
            }

        BtnRemover =
            New Button With {
                .Name = "BtnRemover",
                .Text = "Remover selecionados",
                .Location = New Point(180, 400),
                .Size = New Size(180, 38)
            }

        BtnLimpar =
            New Button With {
                .Name = "BtnLimpar",
                .Text = "Limpar lista",
                .Location = New Point(370, 400),
                .Size = New Size(130, 38)
            }

        BtnConcluir =
            New Button With {
                .Name = "BtnConcluir",
                .Text = "Concluir",
                .Location = New Point(480, 452),
                .Size = New Size(95, 34)
            }

        BtnCancelar =
            New Button With {
                .Name = "BtnCancelar",
                .Text = "Cancelar",
                .Location = New Point(585, 452),
                .Size = New Size(95, 34),
                .DialogResult = DialogResult.Cancel
            }

        AplicarEstiloBotao(
            BtnAdicionar,
            True)

        AplicarEstiloBotao(
            BtnRemover,
            False)

        AplicarEstiloBotao(
            BtnLimpar,
            False)

        AplicarEstiloBotao(
            BtnConcluir,
            True)

        AplicarEstiloBotao(
            BtnCancelar,
            False)

        Me.Controls.AddRange({
            BtnAdicionar,
            BtnRemover,
            BtnLimpar,
            BtnConcluir,
            BtnCancelar
        })

    End Sub

    Private Sub CriarOpcoes()

        ChkOrdemAleatoria =
            New CheckBox With {
                .Name = "ChkOrdemAleatoria",
                .Text = "Reproduzir anúncios em ordem aleatória",
                .Location = New Point(20, 458),
                .AutoSize = True,
                .Checked = False,
                .ForeColor = Color.White,
                .BackColor = Color.Transparent,
                .Font = New Font(
                    "Segoe UI",
                    9.0F,
                    FontStyle.Bold)
            }

        Me.Controls.Add(
            ChkOrdemAleatoria)

    End Sub

    Private Sub AplicarEstiloBotao(
        botao As Button,
        destaque As Boolean)

        botao.FlatStyle =
            FlatStyle.Flat

        botao.FlatAppearance.BorderSize =
            0

        botao.FlatAppearance.MouseOverBackColor =
            If(
                destaque,
                Color.FromArgb(155, 35, 35),
                Color.FromArgb(70, 70, 70))

        botao.FlatAppearance.MouseDownBackColor =
            If(
                destaque,
                Color.FromArgb(105, 22, 22),
                Color.FromArgb(35, 35, 35))

        botao.BackColor =
            If(
                destaque,
                Color.FromArgb(134, 29, 29),
                Color.FromArgb(50, 50, 50))

        botao.ForeColor =
            Color.White

        botao.Font =
            New Font(
                "Segoe UI",
                9.0F,
                FontStyle.Bold)

        botao.Cursor =
            Cursors.Hand

        botao.UseVisualStyleBackColor =
            False

    End Sub

#End Region

#Region "Adicionar anúncios"

    Private Sub BtnAdicionar_Click(
        sender As Object,
        e As EventArgs
    ) Handles BtnAdicionar.Click

        Using seletor As New OpenFileDialog()

            seletor.Title =
                "Selecione os anúncios gravados"

            seletor.Filter =
                "Arquivos de áudio|" &
                "*.mp3;*.wav;*.wma;*.aac;*.m4a|" &
                "Todos os arquivos|*.*"

            seletor.Multiselect =
                True

            If seletor.ShowDialog(Me) <>
               DialogResult.OK Then

                Exit Sub

            End If

            Dim quantidadeAnterior As Integer =
                caminhosAnuncios.Count

            For Each caminho As String In seletor.FileNames

                AdicionarCaminho(
                    caminho)

            Next

            AtualizarQuantidade()

            If caminhosAnuncios.Count =
               quantidadeAnterior Then

                MessageBox.Show(
                    "Nenhum anúncio novo foi adicionado.",
                    "Anúncios gravados",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information)

            End If

        End Using

    End Sub

    Private Sub AdicionarCaminho(
        caminho As String)

        If String.IsNullOrWhiteSpace(
            caminho) Then

            Exit Sub

        End If

        If Not File.Exists(caminho) Then
            Exit Sub
        End If

        Dim extensao As String =
            Path.GetExtension(caminho)

        If Not extensoesPermitidas.Contains(
            extensao) Then

            Exit Sub

        End If

        Dim jaAdicionado As Boolean =
            caminhosAnuncios.Any(
                Function(item)
                    Return String.Equals(
                        item,
                        caminho,
                        StringComparison.OrdinalIgnoreCase)
                End Function)

        If jaAdicionado Then
            Exit Sub
        End If

        caminhosAnuncios.Add(
            caminho)

        LstAnuncios.Items.Add(
            Path.GetFileName(caminho))

    End Sub

#End Region

#Region "Remover anúncios"

    Private Sub BtnRemover_Click(
        sender As Object,
        e As EventArgs
    ) Handles BtnRemover.Click

        If LstAnuncios.SelectedIndices.Count = 0 Then

            MessageBox.Show(
                "Selecione pelo menos um anúncio para remover.",
                "Aviso",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning)

            Exit Sub

        End If

        Dim indicesSelecionados As List(Of Integer) =
            LstAnuncios.SelectedIndices.
            Cast(Of Integer)().
            OrderByDescending(
                Function(indice)
                    Return indice
                End Function).
            ToList()

        Dim menorIndice As Integer =
            indicesSelecionados.Min()

        For Each indice As Integer In indicesSelecionados

            caminhosAnuncios.RemoveAt(
                indice)

            LstAnuncios.Items.RemoveAt(
                indice)

        Next

        If LstAnuncios.Items.Count > 0 Then

            menorIndice =
                Math.Min(
                    menorIndice,
                    LstAnuncios.Items.Count - 1)

            LstAnuncios.SelectedIndex =
                menorIndice

        End If

        AtualizarQuantidade()

    End Sub

    Private Sub BtnLimpar_Click(
        sender As Object,
        e As EventArgs
    ) Handles BtnLimpar.Click

        If caminhosAnuncios.Count = 0 Then
            Exit Sub
        End If

        Dim resposta As DialogResult =
            MessageBox.Show(
                "Deseja remover todos os anúncios gravados?",
                "Limpar anúncios",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question)

        If resposta <> DialogResult.Yes Then
            Exit Sub
        End If

        caminhosAnuncios.Clear()
        LstAnuncios.Items.Clear()

        AtualizarQuantidade()

    End Sub

#End Region

#Region "Concluir e cancelar"

    Private Sub BtnConcluir_Click(
        sender As Object,
        e As EventArgs
    ) Handles BtnConcluir.Click

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

#End Region

#Region "Atualização visual"

    Private Sub AtualizarQuantidade()

        Dim quantidade As Integer =
            caminhosAnuncios.Count

        If quantidade = 0 Then

            LblQuantidade.Text =
                "Nenhum anúncio adicionado"

        ElseIf quantidade = 1 Then

            LblQuantidade.Text =
                "1 anúncio adicionado"

        Else

            LblQuantidade.Text =
                $"{quantidade} anúncios adicionados"

        End If

        BtnRemover.Enabled =
            quantidade > 0

        BtnLimpar.Enabled =
            quantidade > 0

    End Sub

#End Region

End Class