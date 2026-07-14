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
        CampoCanvas.AdicionarLinha(
    TipoLinhaTatica.Continua,
    CorLinhaTatica.Branca,
    15,
    15,
    35,
    25,
    3.0F)

        CampoCanvas.AdicionarLinha(
            TipoLinhaTatica.Tracejada,
            CorLinhaTatica.Amarela,
            40,
            75,
            65,
            65,
            3.0F)

        CampoCanvas.AdicionarLinha(
            TipoLinhaTatica.Seta,
            CorLinhaTatica.Vermelha,
            55,
            35,
            78,
            42,
            4.0F)
        CampoCanvas.AdicionarAreaTatica(
    CorAreaTatica.Amarela,
    18,
    18,
    38,
    42,
    True,
    40,
    2.5F)

        CampoCanvas.AdicionarAreaTatica(
            CorAreaTatica.Azul,
            60,
            55,
            83,
            80,
            False,
            35,
            3.0F)

        CampoCanvas.AdicionarAreaTatica(
            CorAreaTatica.Vermelha,
            42,
            20,
            56,
            38,
            True,
            30,
            2.0F)

        CampoCanvas.AdicionarMarcador(
    "1",
    CorMarcadorTatico.Branco,
    15,
    18,
    36.0F)

        CampoCanvas.AdicionarMarcador(
            "2",
            CorMarcadorTatico.Amarelo,
            48,
            48,
            36.0F)

        CampoCanvas.AdicionarMarcador(
            "3",
            CorMarcadorTatico.Vermelho,
            82,
            75,
            36.0F)

        CampoCanvas.AdicionarMarcador(
            "A",
            CorMarcadorTatico.Azul,
            75,
            20,
            36.0F)

    End Sub

End Class