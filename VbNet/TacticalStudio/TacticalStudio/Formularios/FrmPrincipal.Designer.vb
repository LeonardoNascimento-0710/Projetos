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
        PnlSuperior.BackColor = Color.FromArgb(CByte(28), CByte(28), CByte(28))
        PnlSuperior.Dock = DockStyle.Top
        PnlSuperior.Location = New Point(0, 0)
        PnlSuperior.Margin = New Padding(3, 4, 3, 4)
        PnlSuperior.Name = "PnlSuperior"
        PnlSuperior.Size = New Size(987, 93)
        PnlSuperior.TabIndex = 0
        ' 
        ' PnlInferior
        ' 
        PnlInferior.Dock = DockStyle.Bottom
        PnlInferior.Location = New Point(0, 841)
        PnlInferior.Margin = New Padding(3, 4, 3, 4)
        PnlInferior.Name = "PnlInferior"
        PnlInferior.Size = New Size(987, 40)
        PnlInferior.TabIndex = 1
        ' 
        ' PnlEsquerdo
        ' 
        PnlEsquerdo.Dock = DockStyle.Left
        PnlEsquerdo.Location = New Point(0, 93)
        PnlEsquerdo.Margin = New Padding(3, 4, 3, 4)
        PnlEsquerdo.Name = "PnlEsquerdo"
        PnlEsquerdo.Size = New Size(297, 748)
        PnlEsquerdo.TabIndex = 2
        ' 
        ' PnlDireito
        ' 
        PnlDireito.Dock = DockStyle.Right
        PnlDireito.Location = New Point(987, 0)
        PnlDireito.Margin = New Padding(3, 4, 3, 4)
        PnlDireito.Name = "PnlDireito"
        PnlDireito.Size = New Size(366, 881)
        PnlDireito.TabIndex = 3
        ' 
        ' PnlCentral
        ' 
        PnlCentral.BackColor = Color.Gray
        PnlCentral.Dock = DockStyle.Fill
        PnlCentral.Location = New Point(297, 93)
        PnlCentral.Margin = New Padding(3, 4, 3, 4)
        PnlCentral.Name = "PnlCentral"
        PnlCentral.Size = New Size(690, 748)
        PnlCentral.TabIndex = 4
        ' 
        ' FrmPrincipal
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(28), CByte(28), CByte(28))
        ClientSize = New Size(1353, 881)
        Controls.Add(PnlCentral)
        Controls.Add(PnlEsquerdo)
        Controls.Add(PnlSuperior)
        Controls.Add(PnlInferior)
        Controls.Add(PnlDireito)
        Margin = New Padding(3, 4, 3, 4)
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
