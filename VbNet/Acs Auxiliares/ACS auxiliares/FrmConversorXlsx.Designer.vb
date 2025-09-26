<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmConversorXlsx
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmConversorXlsx))
        BtnCarregarExcel = New Button()
        DgvProdutos = New DataGridView()
        BtnGerarSQL = New Button()
        BtnScriptColeta = New Button()
        CType(DgvProdutos, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' BtnCarregarExcel
        ' 
        BtnCarregarExcel.Location = New Point(271, 45)
        BtnCarregarExcel.Name = "BtnCarregarExcel"
        BtnCarregarExcel.Size = New Size(89, 36)
        BtnCarregarExcel.TabIndex = 0
        BtnCarregarExcel.Text = "Importar Xlsx"
        BtnCarregarExcel.UseVisualStyleBackColor = True
        ' 
        ' DgvProdutos
        ' 
        DgvProdutos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DgvProdutos.Location = New Point(25, 87)
        DgvProdutos.Name = "DgvProdutos"
        DgvProdutos.Size = New Size(754, 319)
        DgvProdutos.TabIndex = 1
        ' 
        ' BtnGerarSQL
        ' 
        BtnGerarSQL.Location = New Point(386, 45)
        BtnGerarSQL.Name = "BtnGerarSQL"
        BtnGerarSQL.Size = New Size(89, 36)
        BtnGerarSQL.TabIndex = 2
        BtnGerarSQL.Text = "Gerar Script"
        BtnGerarSQL.UseVisualStyleBackColor = True
        ' 
        ' BtnScriptColeta
        ' 
        BtnScriptColeta.Location = New Point(699, 12)
        BtnScriptColeta.Name = "BtnScriptColeta"
        BtnScriptColeta.Size = New Size(64, 21)
        BtnScriptColeta.TabIndex = 3
        BtnScriptColeta.Text = "Script"
        BtnScriptColeta.UseVisualStyleBackColor = True
        ' 
        ' FrmConversorXlsx
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        Controls.Add(BtnScriptColeta)
        Controls.Add(BtnGerarSQL)
        Controls.Add(DgvProdutos)
        Controls.Add(BtnCarregarExcel)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "FrmConversorXlsx"
        Text = "Conversor de Tabelas"
        CType(DgvProdutos, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents BtnCarregarExcel As Button
    Friend WithEvents DgvProdutos As DataGridView
    Friend WithEvents BtnGerarSQL As Button
    Friend WithEvents BtnScriptColeta As Button
End Class
