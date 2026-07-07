<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmDownloadsConversor
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmDownloadsConversor))
        PbDownload = New ProgressBar()
        LblVideo = New Label()
        LblNome = New Label()
        LblVelocidade = New Label()
        LblETA = New Label()
        LblStatus = New Label()
        TxtLog = New RichTextBox()
        BtnSelecionarPlaylist = New Button()
        Panel1 = New Panel()
        BtnCancelar = New Button()
        BtnConversor = New Button()
        BtnRetornar = New Button()
        Panel1.SuspendLayout()
        SuspendLayout()
        ' 
        ' PbDownload
        ' 
        PbDownload.Location = New Point(12, 61)
        PbDownload.Name = "PbDownload"
        PbDownload.Size = New Size(368, 23)
        PbDownload.Style = ProgressBarStyle.Continuous
        PbDownload.TabIndex = 0
        ' 
        ' LblVideo
        ' 
        LblVideo.AutoSize = True
        LblVideo.ForeColor = Color.White
        LblVideo.Location = New Point(12, 21)
        LblVideo.Name = "LblVideo"
        LblVideo.Size = New Size(46, 15)
        LblVideo.TabIndex = 1
        LblVideo.Text = "VÍDEO: "
        ' 
        ' LblNome
        ' 
        LblNome.AutoSize = True
        LblNome.ForeColor = Color.White
        LblNome.Location = New Point(12, 41)
        LblNome.Name = "LblNome"
        LblNome.Size = New Size(48, 15)
        LblNome.TabIndex = 2
        LblNome.Text = "NOME: "
        ' 
        ' LblVelocidade
        ' 
        LblVelocidade.AutoSize = True
        LblVelocidade.ForeColor = Color.White
        LblVelocidade.Location = New Point(12, 89)
        LblVelocidade.Name = "LblVelocidade"
        LblVelocidade.Size = New Size(79, 15)
        LblVelocidade.TabIndex = 3
        LblVelocidade.Text = "VELOCIDADE:"
        ' 
        ' LblETA
        ' 
        LblETA.AutoSize = True
        LblETA.ForeColor = Color.White
        LblETA.Location = New Point(12, 109)
        LblETA.Name = "LblETA"
        LblETA.Size = New Size(29, 15)
        LblETA.TabIndex = 4
        LblETA.Text = "ETA:"
        ' 
        ' LblStatus
        ' 
        LblStatus.AutoSize = True
        LblStatus.ForeColor = Color.White
        LblStatus.Location = New Point(12, 129)
        LblStatus.Name = "LblStatus"
        LblStatus.Size = New Size(48, 15)
        LblStatus.TabIndex = 5
        LblStatus.Text = "STATUS:"
        ' 
        ' TxtLog
        ' 
        TxtLog.Location = New Point(12, 149)
        TxtLog.Name = "TxtLog"
        TxtLog.Size = New Size(368, 69)
        TxtLog.TabIndex = 6
        TxtLog.Text = ""
        ' 
        ' BtnSelecionarPlaylist
        ' 
        BtnSelecionarPlaylist.Location = New Point(51, 14)
        BtnSelecionarPlaylist.Name = "BtnSelecionarPlaylist"
        BtnSelecionarPlaylist.Size = New Size(87, 49)
        BtnSelecionarPlaylist.TabIndex = 7
        BtnSelecionarPlaylist.Text = "Selecionar Playlist"
        BtnSelecionarPlaylist.UseVisualStyleBackColor = True
        ' 
        ' Panel1
        ' 
        Panel1.BackColor = SystemColors.ControlDarkDark
        Panel1.Controls.Add(BtnCancelar)
        Panel1.Controls.Add(BtnConversor)
        Panel1.Controls.Add(BtnRetornar)
        Panel1.Controls.Add(BtnSelecionarPlaylist)
        Panel1.Location = New Point(0, 236)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(411, 77)
        Panel1.TabIndex = 8
        ' 
        ' BtnCancelar
        ' 
        BtnCancelar.Enabled = False
        BtnCancelar.Location = New Point(162, 14)
        BtnCancelar.Name = "BtnCancelar"
        BtnCancelar.Size = New Size(87, 49)
        BtnCancelar.TabIndex = 9
        BtnCancelar.Text = "Cancelar"
        BtnCancelar.UseVisualStyleBackColor = True
        BtnCancelar.Visible = False
        ' 
        ' BtnConversor
        ' 
        BtnConversor.Location = New Point(162, 14)
        BtnConversor.Name = "BtnConversor"
        BtnConversor.Size = New Size(87, 49)
        BtnConversor.TabIndex = 8
        BtnConversor.Text = "Converter para Mp3" & vbCrLf
        BtnConversor.UseVisualStyleBackColor = True
        ' 
        ' BtnRetornar
        ' 
        BtnRetornar.Location = New Point(272, 15)
        BtnRetornar.Name = "BtnRetornar"
        BtnRetornar.Size = New Size(87, 49)
        BtnRetornar.TabIndex = 1
        BtnRetornar.Text = "Retornar"
        BtnRetornar.UseVisualStyleBackColor = True
        ' 
        ' FrmDownloadsConversor
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(134), CByte(29), CByte(29))
        ClientSize = New Size(401, 312)
        Controls.Add(Panel1)
        Controls.Add(TxtLog)
        Controls.Add(LblStatus)
        Controls.Add(LblETA)
        Controls.Add(LblVelocidade)
        Controls.Add(LblNome)
        Controls.Add(LblVideo)
        Controls.Add(PbDownload)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MdiChildrenMinimizedAnchorBottom = False
        MinimizeBox = False
        Name = "FrmDownloadsConversor"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Downloads"
        Panel1.ResumeLayout(False)
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents PbDownload As ProgressBar
    Friend WithEvents LblVideo As Label
    Friend WithEvents LblNome As Label
    Friend WithEvents LblVelocidade As Label
    Friend WithEvents LblETA As Label
    Friend WithEvents LblStatus As Label
    Friend WithEvents TxtLog As RichTextBox
    Friend WithEvents BtnSelecionarPlaylist As Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents BtnRetornar As Button
    Friend WithEvents BtnConversor As Button
    Friend WithEvents BtnCancelar As Button
End Class
