Imports TacticalStudio.Core.Enums
Public Class FrmPrincipal

    Private CampoCanvas As CampoTatico

    Private Sub FrmPrincipal_Load(
        sender As Object,
        e As EventArgs) Handles MyBase.Load

        AplicarTema()

        CriarCampoTatico()

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

        PnlCentral.Controls.Clear()
        PnlCentral.Controls.Add(CampoCanvas)

        CampoCanvas.AdicionarJogador(
        10,
        "Centroavante",
        42,
        50)

        CampoCanvas.AdicionarJogador(
        9,
        "Atacante",
        62,
        35)

        CampoCanvas.AdicionarBola(
        52,
        50)

        CampoCanvas.AdicionarCone(
    CorCone.Laranja,
    30,
    30)

        CampoCanvas.AdicionarCone(
            CorCone.Amarelo,
            30,
            45)

        CampoCanvas.AdicionarCone(
            CorCone.Azul,
            30,
            60)
        CampoCanvas.AdicionarGol(
    OrientacaoGol.Direita,
    10,
    50)

        CampoCanvas.AdicionarGol(
            OrientacaoGol.Esquerda,
            90,
            50)

        CampoCanvas.AdicionarGol(
            OrientacaoGol.Baixo,
            50,
            18)
        CampoCanvas.AdicionarManequim(
    CorManequim.Amarelo,
    70,
    30)

        CampoCanvas.AdicionarManequim(
            CorManequim.Vermelho,
            70,
            50)

        CampoCanvas.AdicionarManequim(
            CorManequim.Azul,
            70,
            70)
    End Sub

End Class