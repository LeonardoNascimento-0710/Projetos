<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmMonitorAcs
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmMonitorAcs))
        DvVencimentos = New DataGridView()
        LblCertificadosNfce = New Label()
        CType(DvVencimentos, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' DvVencimentos
        ' 
        DvVencimentos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DvVencimentos.Location = New Point(13, 27)
        DvVencimentos.Name = "DvVencimentos"
        DvVencimentos.Size = New Size(535, 175)
        DvVencimentos.TabIndex = 0
        ' 
        ' LblCertificadosNfce
        ' 
        LblCertificadosNfce.AutoSize = True
        LblCertificadosNfce.Location = New Point(13, 9)
        LblCertificadosNfce.Name = "LblCertificadosNfce"
        LblCertificadosNfce.Size = New Size(105, 15)
        LblCertificadosNfce.TabIndex = 1
        LblCertificadosNfce.Text = "Certificados NFc-e"
        ' 
        ' FrmMonitorAcs
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(560, 340)
        Controls.Add(LblCertificadosNfce)
        Controls.Add(DvVencimentos)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "FrmMonitorAcs"
        Text = "Monitor ACS"
        CType(DvVencimentos, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents DvVencimentos As DataGridView
    Friend WithEvents BtnAtualizar As Button
    Friend WithEvents LblCertificadosNfce As Label

End Class
