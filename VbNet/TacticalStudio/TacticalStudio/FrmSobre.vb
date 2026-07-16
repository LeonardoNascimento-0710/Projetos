Public Class FrmSobre
    Inherits Form

    Public Sub New()

        Text =
            "Sobre o TacticalStudio"

        StartPosition = FormStartPosition.CenterParent

        FormBorderStyle = FormBorderStyle.FixedDialog

        MaximizeBox = False
        MinimizeBox = False
        ShowInTaskbar = False

        ClientSize =
            New Size(
                460,
                330)

        BackColor = Tema.Fundo

        ForeColor = Tema.Texto

        Font = Tema.FontePadrao

        CriarInterface()

    End Sub

    Private Sub CriarInterface()

        Dim painel As New Panel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(24),
            .BackColor = Tema.Fundo
        }

        Controls.Add(painel)

        Dim marca As New Label With {
            .Text = "TACTICALSTUDIO",
            .Left = 24,
            .Top = 30,
            .Width = 412,
            .Height = 52,
            .Font = New Font(
                "Segoe UI",
                22.0F,
                FontStyle.Bold),
            .ForeColor = Tema.CorPrimaria,
            .TextAlign =
                ContentAlignment.MiddleCenter
        }

        painel.Controls.Add(marca)

        Dim versao As New Label With {
            .Text =
                "Versão " &
                InformacoesAplicacao.Versao,
            .Left = 24,
            .Top = 86,
            .Width = 412,
            .Height = 30,
            .ForeColor = Tema.Texto,
            .TextAlign =
                ContentAlignment.MiddleCenter
        }

        painel.Controls.Add(versao)

        Dim descricao As New Label With {
            .Text =
                "Editor de exercícios e estratégias de futebol." &
                Environment.NewLine &
                Environment.NewLine &
                "Crie campos táticos, organize jogadores, " &
                "adicione linhas, áreas, textos e exporte " &
                "seus exercícios em PNG e PDF.",
            .Left = 40,
            .Top = 132,
            .Width = 380,
            .Height = 105,
            .ForeColor = Tema.TextoSecundario,
            .TextAlign =
                ContentAlignment.MiddleCenter
        }

        painel.Controls.Add(descricao)

        Dim desenvolvimento As New Label With {
            .Text = "Desenvolvido por Leonardo Nascimento",
            .Left = 24,
            .Top = 242,
            .Width = 412,
            .Height = 24,
            .ForeColor = Tema.Texto,
            .TextAlign =
                ContentAlignment.MiddleCenter
        }

        painel.Controls.Add(
            desenvolvimento)

        Dim botaoFechar As New Button With {
            .Text = "Fechar",
            .Left = 170,
            .Top = 278,
            .Width = 120,
            .Height = 38,
            .DialogResult = DialogResult.OK,
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Tema.CorPrimaria,
            .ForeColor = Color.White
        }

        botaoFechar.FlatAppearance.BorderColor = Color.White

        painel.Controls.Add(botaoFechar)

        AcceptButton = botaoFechar

        CancelButton = botaoFechar

    End Sub

End Class