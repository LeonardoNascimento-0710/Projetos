<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmPrincipal
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        PnlSuperior = New Panel()
        PnlInferior = New Panel()
        PnlEsquerdo = New Panel()
        PnlDireito = New Panel()
        PnlCentral = New Panel()
        SuspendLayout()
        ' 
        ' PnlSuperior
        ' 
        PnlSuperior.BackColor = Color.Red
        PnlSuperior.Dock = DockStyle.Top
        PnlSuperior.Location = New Point(0, 0)
        PnlSuperior.Name = "PnlSuperior"
        PnlSuperior.Size = New Size(1184, 70)
        PnlSuperior.TabIndex = 0
        ' 
        ' PnlInferior
        ' 
        PnlInferior.BackColor = Color.Yellow
        PnlInferior.Dock = DockStyle.Bottom
        PnlInferior.Location = New Point(0, 631)
        PnlInferior.Name = "PnlInferior"
        PnlInferior.Size = New Size(1184, 30)
        PnlInferior.TabIndex = 1
        ' 
        ' PnlEsquerdo
        ' 
        PnlEsquerdo.BackColor = Color.Blue
        PnlEsquerdo.Dock = DockStyle.Left
        PnlEsquerdo.Location = New Point(0, 70)
        PnlEsquerdo.Name = "PnlEsquerdo"
        PnlEsquerdo.Size = New Size(260, 561)
        PnlEsquerdo.TabIndex = 2
        ' 
        ' PnlDireito
        ' 
        PnlDireito.BackColor = Color.Green
        PnlDireito.Dock = DockStyle.Right
        PnlDireito.Location = New Point(864, 70)
        PnlDireito.Name = "PnlDireito"
        PnlDireito.Size = New Size(320, 561)
        PnlDireito.TabIndex = 3
        ' 
        ' PnlCentral
        ' 
        PnlCentral.BackColor = Color.Gray
        PnlCentral.Dock = DockStyle.Fill
        PnlCentral.Location = New Point(260, 70)
        PnlCentral.Name = "PnlCentral"
        PnlCentral.Size = New Size(604, 561)
        PnlCentral.TabIndex = 4
        ' 
        ' FrmPrincipal
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(28), CByte(28), CByte(28))
        ClientSize = New Size(1184, 661)
        Controls.Add(PnlCentral)
        Controls.Add(PnlDireito)
        Controls.Add(PnlEsquerdo)
        Controls.Add(PnlSuperior)
        Controls.Add(PnlInferior)
        Name = "FrmPrincipal"
        Text = "Tactical Studio"
        WindowState = FormWindowState.Maximized
        ResumeLayout(False)
    End Sub

    Friend WithEvents PnlSuperior As Panel
    Friend WithEvents PnlInferior As Panel
    Friend WithEvents PnlEsquerdo As Panel
    Friend WithEvents PnlDireito As Panel
    Friend WithEvents PnlCentral As Panel

End Class
