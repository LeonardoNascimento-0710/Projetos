<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmBancoDeConversao
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
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
        PnlBancoDeConversao = New Panel()
        DgBanco = New DataGridView()
        BtnFileSelect = New Button()
        PnlBancoDeConversao.SuspendLayout()
        CType(DgBanco, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' PnlBancoDeConversao
        ' 
        PnlBancoDeConversao.Controls.Add(DgBanco)
        PnlBancoDeConversao.Location = New Point(23, 12)
        PnlBancoDeConversao.Name = "PnlBancoDeConversao"
        PnlBancoDeConversao.Size = New Size(1038, 589)
        PnlBancoDeConversao.TabIndex = 0
        ' 
        ' DgBanco
        ' 
        DgBanco.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DgBanco.Location = New Point(27, 21)
        DgBanco.Name = "DgBanco"
        DgBanco.Size = New Size(987, 548)
        DgBanco.TabIndex = 0
        ' 
        ' BtnFileSelect
        ' 
        BtnFileSelect.Location = New Point(214, 619)
        BtnFileSelect.Name = "BtnFileSelect"
        BtnFileSelect.Size = New Size(257, 75)
        BtnFileSelect.TabIndex = 1
        BtnFileSelect.Text = "Selecione o banco"
        BtnFileSelect.UseVisualStyleBackColor = True
        ' 
        ' FrmBancoDeConversao
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1089, 706)
        Controls.Add(BtnFileSelect)
        Controls.Add(PnlBancoDeConversao)
        MaximizeBox = False
        MinimizeBox = False
        Name = "FrmBancoDeConversao"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Banco de conversão"
        PnlBancoDeConversao.ResumeLayout(False)
        CType(DgBanco, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents PnlBancoDeConversao As Panel
    Friend WithEvents DgBanco As DataGridView
    Friend WithEvents BtnFileSelect As Button

End Class
