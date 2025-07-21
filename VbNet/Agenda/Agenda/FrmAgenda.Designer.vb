<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmAgenda
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
        DtData = New DateTimePicker()
        Panel1 = New Panel()
        DgDados = New DataGridView()
        Panel1.SuspendLayout()
        CType(DgDados, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' DtData
        ' 
        DtData.Format = DateTimePickerFormat.Short
        DtData.Location = New Point(89, 17)
        DtData.Name = "DtData"
        DtData.Size = New Size(78, 23)
        DtData.TabIndex = 0
        ' 
        ' Panel1
        ' 
        Panel1.Controls.Add(DgDados)
        Panel1.Controls.Add(DtData)
        Panel1.Location = New Point(12, 21)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(264, 340)
        Panel1.TabIndex = 1
        ' 
        ' DgDados
        ' 
        DgDados.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        DgDados.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DgDados.EnableHeadersVisualStyles = False
        DgDados.Location = New Point(31, 60)
        DgDados.Name = "DgDados"
        DgDados.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        DgDados.RowHeadersVisible = False
        DgDados.Size = New Size(201, 247)
        DgDados.TabIndex = 1
        ' 
        ' FrmAgenda
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(284, 382)
        Controls.Add(Panel1)
        Name = "FrmAgenda"
        Text = "Agenda"
        Panel1.ResumeLayout(False)
        CType(DgDados, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents DtData As DateTimePicker
    Friend WithEvents Panel1 As Panel
    Friend WithEvents DgDados As DataGridView

End Class
