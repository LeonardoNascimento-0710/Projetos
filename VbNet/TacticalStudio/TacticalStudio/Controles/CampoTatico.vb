Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Public Class CampoTatico

    Inherits UserControl

    Public Sub New()

        DoubleBuffered = True
        ResizeRedraw = True

        BackColor = Color.FromArgb(34, 139, 34)

    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        MyBase.OnPaint(e)

        DesenharCampo(e.Graphics)

    End Sub

    Private Sub DesenharCampo(g As Graphics)

        g.Clear(BackColor)

        Using caneta As New Pen(Color.White, 2)

            Dim margem As Integer = 40

            Dim campo As New Rectangle(
                margem,
                margem,
                Width - margem * 2,
                Height - margem * 2)

            g.DrawRectangle(caneta, campo)

            Dim centroX As Integer = campo.Left + (campo.Width \ 2)

            g.DrawLine(caneta, centroX, campo.Top, centroX, campo.Bottom)
            Dim raio As Integer = 70

            centroX = campo.Left + (campo.Width \ 2)
            Dim centroY As Integer = campo.Top + (campo.Height \ 2)

            g.DrawEllipse(caneta, centroX - raio, centroY - raio, raio * 2, raio * 2)

        End Using

    End Sub

End Class