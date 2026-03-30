<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmConversorBalancaFilizola
    Inherits System.Windows.Forms.Form

    'Descartar substituições de formulário para limpar a lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Exigido pelo Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'OBSERVAÇÃO: o procedimento a seguir é exigido pelo Windows Form Designer
    'Pode ser modificado usando o Windows Form Designer.  
    'Não o modifique usando o editor de códigos.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmConversorBalancaFilizola))
        BtnLimparArquivo = New Button()
        BtnGerarUpdate = New Button()
        BtnRetornar = New Button()
        Panel1 = New Panel()
        Panel2 = New Panel()
        Panel1.SuspendLayout()
        Panel2.SuspendLayout()
        SuspendLayout()
        ' 
        ' BtnLimparArquivo
        ' 
        BtnLimparArquivo.Location = New Point(12, 26)
        BtnLimparArquivo.Name = "BtnLimparArquivo"
        BtnLimparArquivo.Size = New Size(100, 44)
        BtnLimparArquivo.TabIndex = 0
        BtnLimparArquivo.Text = "Limpar Arquivo"
        BtnLimparArquivo.UseVisualStyleBackColor = True
        ' 
        ' BtnGerarUpdate
        ' 
        BtnGerarUpdate.Location = New Point(133, 26)
        BtnGerarUpdate.Name = "BtnGerarUpdate"
        BtnGerarUpdate.Size = New Size(100, 44)
        BtnGerarUpdate.TabIndex = 1
        BtnGerarUpdate.Text = "Gerar Update"
        BtnGerarUpdate.UseVisualStyleBackColor = True
        ' 
        ' BtnRetornar
        ' 
        BtnRetornar.Location = New Point(130, 6)
        BtnRetornar.Name = "BtnRetornar"
        BtnRetornar.Size = New Size(63, 43)
        BtnRetornar.TabIndex = 2
        BtnRetornar.Text = "Voltar"
        BtnRetornar.UseVisualStyleBackColor = True
        ' 
        ' Panel1
        ' 
        Panel1.BackColor = SystemColors.ControlDarkDark
        Panel1.Controls.Add(BtnRetornar)
        Panel1.Location = New Point(-5, 160)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(619, 107)
        Panel1.TabIndex = 3
        ' 
        ' Panel2
        ' 
        Panel2.BackColor = SystemColors.ControlLightLight
        Panel2.Controls.Add(BtnGerarUpdate)
        Panel2.Controls.Add(BtnLimparArquivo)
        Panel2.Location = New Point(31, 32)
        Panel2.Name = "Panel2"
        Panel2.Size = New Size(250, 96)
        Panel2.TabIndex = 4
        ' 
        ' FrmConversorBalancaFilizola
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = SystemColors.ActiveCaption
        ClientSize = New Size(317, 216)
        Controls.Add(Panel2)
        Controls.Add(Panel1)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MdiChildrenMinimizedAnchorBottom = False
        MinimizeBox = False
        Name = "FrmConversorBalancaFilizola"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Conversor De arquivos Balança Filizola"
        Panel1.ResumeLayout(False)
        Panel2.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents BtnLimparArquivo As Button
    Friend WithEvents BtnGerarUpdate As Button
    Friend WithEvents BtnRetornar As Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Panel2 As Panel
End Class
