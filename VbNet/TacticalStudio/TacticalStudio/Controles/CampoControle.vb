Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Public Class CampoControle
    Inherits UserControl

    Public Sub New()

        Me.DoubleBuffered = True
        Me.ResizeRedraw = True

        Me.BackColor = Tema.Fundo
        Me.Dock = DockStyle.Fill

    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        MyBase.OnPaint(e)

        DesenharCampo(e.Graphics)

    End Sub

    Private Sub DesenharCampo(g As Graphics)

        g.Clear(Color.ForestGreen)

    End Sub

End Class