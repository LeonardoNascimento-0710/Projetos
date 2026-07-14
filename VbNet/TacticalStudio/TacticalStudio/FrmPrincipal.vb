Imports System.Collections.Generic
Imports TacticalStudio.Core.Enums
Public Class FrmPrincipal

    Private CampoCanvas As CampoTatico

    Private ReadOnly _botoesFerramentas As New Dictionary(Of FerramentaCampo, Button)()

    Private Sub FrmPrincipal_Load(
    sender As Object,
    e As EventArgs) Handles MyBase.Load

        AplicarTema()

        CriarCampoTatico()

        CriarBarraFerramentas()

    End Sub

    Private Sub AplicarTema()

        BackColor = Tema.Fundo
        ForeColor = Tema.Texto
        Font = Tema.FontePadrao

        PnlSuperior.BackColor = Tema.CorPrimaria
        PnlEsquerdo.BackColor = Tema.Painel
        PnlDireito.BackColor = Tema.Painel
        PnlInferior.BackColor = Tema.Painel
        PnlCentral.BackColor = Tema.Fundo

    End Sub

    Private Sub CriarCampoTatico()

        CampoCanvas = New CampoTatico With {
        .Dock = DockStyle.Fill
    }

        AddHandler CampoCanvas.FerramentaAtualAlterada,
        AddressOf CampoCanvas_FerramentaAtualAlterada

        PnlCentral.Controls.Clear()
        PnlCentral.Controls.Add(CampoCanvas)

    End Sub

    Private Sub CriarBarraFerramentas()

        PnlEsquerdo.Controls.Clear()
        _botoesFerramentas.Clear()

        Dim painelFerramentas As New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False,
            .AutoScroll = True,
            .BackColor = Tema.Painel,
            .Padding = New Padding(6)
        }

        Dim larguraBotao As Integer =
            Math.Max(
                120,
                PnlEsquerdo.ClientSize.Width - 24)

        Dim titulo As New Label With {
            .Text = "FERRAMENTAS",
            .ForeColor = Tema.Texto,
            .Font = New Font(
                "Segoe UI",
                10.0F,
                FontStyle.Bold),
            .Width = larguraBotao,
            .Height = 34,
            .TextAlign = ContentAlignment.MiddleLeft
        }

        painelFerramentas.Controls.Add(titulo)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Selecionar,
            "Selecionar",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Jogador,
            "Jogador",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Bola,
            "Bola",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Cone,
            "Cone",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Gol,
            "Gol",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Manequim,
            "Manequim",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.LinhaContinua,
            "Linha",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.LinhaTracejada,
            "Linha tracejada",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Seta,
            "Seta",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Area,
            "Área",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Marcador,
            "Marcador",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Texto,
            "Texto",
            larguraBotao)

        PnlEsquerdo.Controls.Add(painelFerramentas)

        AtualizarBotoesFerramentas(
            FerramentaCampo.Selecionar)

    End Sub

    Private Sub AdicionarBotaoFerramenta(
    painel As FlowLayoutPanel,
    ferramenta As FerramentaCampo,
    texto As String,
    largura As Integer)

        Dim botao As New Button With {
            .Text = texto,
            .Tag = ferramenta,
            .Width = largura,
            .Height = 36,
            .Margin = New Padding(0, 3, 0, 3),
            .FlatStyle = FlatStyle.Flat,
            .TextAlign = ContentAlignment.MiddleLeft,
            .Cursor = Cursors.Hand,
            .BackColor = Tema.Painel,
            .ForeColor = Tema.Texto
        }

        botao.FlatAppearance.BorderColor =
            Tema.Borda

        botao.FlatAppearance.MouseOverBackColor =
            Color.FromArgb(55, 55, 55)

        botao.FlatAppearance.MouseDownBackColor =
            Tema.CorPrimaria

        AddHandler botao.Click,
            AddressOf BotaoFerramenta_Click

        _botoesFerramentas.Add(
            ferramenta,
            botao)

        painel.Controls.Add(botao)

    End Sub

    Private Sub BotaoFerramenta_Click(
    sender As Object,
    e As EventArgs)

        Dim botao As Button =
            TryCast(sender, Button)

        If botao Is Nothing Then
            Exit Sub
        End If

        If botao.Tag Is Nothing Then
            Exit Sub
        End If

        Dim ferramenta As FerramentaCampo =
    CType(botao.Tag, FerramentaCampo)

        CampoCanvas.FerramentaAtual =
            ferramenta

        AtualizarBotoesFerramentas(
            ferramenta)

        CampoCanvas.Focus()

    End Sub

    Private Sub CampoCanvas_FerramentaAtualAlterada(
    ferramenta As FerramentaCampo)

        AtualizarBotoesFerramentas(
        ferramenta)

    End Sub

    Private Sub AtualizarBotoesFerramentas(
    ferramentaAtual As FerramentaCampo)

        For Each item As KeyValuePair(
            Of FerramentaCampo,
            Button) In _botoesFerramentas

            Dim botao As Button =
                item.Value

            If item.Key = ferramentaAtual Then

                botao.BackColor =
                    Tema.CorPrimaria

                botao.ForeColor =
                    Color.White

                botao.FlatAppearance.BorderColor =
                    Color.White

            Else

                botao.BackColor =
                    Tema.Painel

                botao.ForeColor =
                    Tema.Texto

                botao.FlatAppearance.BorderColor =
                    Tema.Borda

            End If

        Next

    End Sub


End Class