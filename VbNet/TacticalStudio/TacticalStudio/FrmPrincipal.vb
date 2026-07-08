Imports TacticalStudio.Core.Classes

Public Class FrmPrincipal

    Private ReadOnly Jogadores As New List(Of Jogador)
    Private JogadorSelecionado As Jogador = Nothing
    Private Arrastando As Boolean = False
    Private OffsetMouse As PointF

    Private Sub FrmPrincipal_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Me.BackColor = Tema.Fundo

        PnlSuperior.BackColor = Tema.CorPrimaria
        PnlEsquerdo.BackColor = Tema.Painel
        PnlDireito.BackColor = Tema.Painel
        PnlInferior.BackColor = Tema.Painel
        PnlCentral.BackColor = Tema.Fundo

        Me.ForeColor = Tema.Texto
        Me.Font = Tema.FontePadrao

        Dim jogador As New Jogador()

        jogador.Numero = 10
        jogador.Nome = "Centroavante"

        jogador.Posicao.X = 50
        jogador.Posicao.Y = 50

        Jogadores.Add(jogador)

        PnlCentral.Invalidate()

    End Sub

    Private Sub PnlCentral_MouseDown(sender As Object,
                                 e As MouseEventArgs) Handles PnlCentral.MouseDown

        Dim margem As Integer = 40

        Dim campo As New Rectangle(
        margem,
        margem,
        PnlCentral.Width - margem * 2,
        PnlCentral.Height - margem * 2)

        JogadorSelecionado = Nothing

        For Each jogador In Jogadores

            Dim x As Single = CSng(campo.Left + (jogador.Posicao.X / 100.0F) * campo.Width)
            Dim y As Single = CSng(campo.Top + (jogador.Posicao.Y / 100.0F) * campo.Height)

            Dim raio As Integer = 18

            Dim distancia As Double =
            Math.Sqrt((e.X - x) ^ 2 + (e.Y - y) ^ 2)

            If distancia <= raio Then

                JogadorSelecionado = jogador

                OffsetMouse = New PointF(
                e.X - x,
                e.Y - y)

                Arrastando = True

                Exit For

            End If

        Next

        PnlCentral.Invalidate()

    End Sub

    Private Sub PnlCentral_MouseMove(sender As Object,
                                 e As MouseEventArgs) Handles PnlCentral.MouseMove

        If Not Arrastando Then Exit Sub

        If JogadorSelecionado Is Nothing Then Exit Sub

        Dim margem As Integer = 40

        Dim campo As New Rectangle(
        margem,
        margem,
        PnlCentral.Width - margem * 2,
        PnlCentral.Height - margem * 2)

        Dim x As Single = e.X - OffsetMouse.X
        Dim y As Single = e.Y - OffsetMouse.Y

        JogadorSelecionado.Posicao.X =
        ((x - campo.Left) / campo.Width) * 100.0F

        JogadorSelecionado.Posicao.Y =
        ((y - campo.Top) / campo.Height) * 100.0F

        PnlCentral.Invalidate()

    End Sub

    Private Sub PnlCentral_MouseUp(sender As Object,
                               e As MouseEventArgs) Handles PnlCentral.MouseUp

        Arrastando = False

    End Sub

#Region "Campo"

    Private Sub PnlCentral_Paint(sender As Object, e As PaintEventArgs) Handles PnlCentral.Paint

        e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

        DesenharCampo(e.Graphics)

    End Sub

    Private Sub DesenharCampo(g As Graphics)

        g.Clear(PnlCentral.BackColor)

        Dim margem As Integer = 40

        Dim campo As New Rectangle(
        margem,
        margem,
        PnlCentral.Width - margem * 2,
        PnlCentral.Height - margem * 2)

        DesenharLinhas(g, campo)

        DesenharJogadores(g, campo)

    End Sub

    Private Sub DesenharLinhas(g As Graphics, campo As Rectangle)

        Using caneta As New Pen(Color.White, 3)

            g.DrawRectangle(caneta, campo)

            g.DrawLine(caneta,
                       campo.Left + campo.Width \ 2,
                       campo.Top,
                       campo.Left + campo.Width \ 2,
                       campo.Bottom)

            Dim raio As Integer = 70

            g.DrawEllipse(caneta,
                          campo.Left + campo.Width \ 2 - raio,
                          campo.Top + campo.Height \ 2 - raio,
                          raio * 2,
                          raio * 2)

        End Using

    End Sub

    Private Sub DesenharJogadores(g As Graphics, campo As Rectangle)

        For Each jogador In Jogadores

            Dim x As Single = CSng(campo.Left + (jogador.Posicao.X / 100.0) * campo.Width)
            Dim y As Single = CSng(campo.Top + (jogador.Posicao.Y / 100.0) * campo.Height)

            Dim raio As Integer = 18

            Using pincel As New SolidBrush(Color.Red)

                g.FillEllipse(
                    pincel,
                    x - raio,
                    y - raio,
                    raio * 2,
                    raio * 2)

                If jogador Is JogadorSelecionado Then

                    Using caneta As New Pen(Color.Gold, 3)

                        g.DrawEllipse(
                            caneta,
                            x - raio - 3,
                            y - raio - 3,
                            raio * 2 + 6,
                            raio * 2 + 6)

                    End Using

                End If

            End Using

            Using fonte As New Font("Segoe UI", 9, FontStyle.Bold),
                  brushTexto As New SolidBrush(Color.White)

                Dim texto = jogador.Numero.ToString()

                Dim tamanho = g.MeasureString(texto, fonte)

                g.DrawString(
                    texto,
                    fonte,
                    brushTexto,
                    x - tamanho.Width / 2,
                    y - tamanho.Height / 2)

            End Using

        Next

    End Sub

#End Region

End Class
