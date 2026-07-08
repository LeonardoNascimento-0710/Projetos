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
        BtnRetornar = New Button()
        BtnCancelar = New Button()
        BtnConversor = New Button()
        GrpDownload = New GroupBox()
        Panel1.SuspendLayout()
        GrpDownload.SuspendLayout()
        SuspendLayout()
        ' 
        ' PbDownload
        ' 
        PbDownload.Location = New Point(22, 62)
        PbDownload.Margin = New Padding(4, 3, 4, 3)
        PbDownload.Name = "PbDownload"
        PbDownload.Size = New Size(589, 24)
        PbDownload.TabIndex = 0
        ' 
        ' LblVideo
        ' 
        LblVideo.AutoSize = True
        LblVideo.ForeColor = Color.White
        LblVideo.Location = New Point(22, 17)
        LblVideo.Margin = New Padding(4, 0, 4, 0)
        LblVideo.Name = "LblVideo"
        LblVideo.Size = New Size(63, 16)
        LblVideo.TabIndex = 1
        LblVideo.Text = "VÍDEO: "
        ' 
        ' LblNome
        ' 
        LblNome.AutoSize = True
        LblNome.ForeColor = Color.White
        LblNome.Location = New Point(22, 38)
        LblNome.Margin = New Padding(4, 0, 4, 0)
        LblNome.Name = "LblNome"
        LblNome.Size = New Size(55, 16)
        LblNome.TabIndex = 2
        LblNome.Text = "NOME: "
        ' 
        ' LblVelocidade
        ' 
        LblVelocidade.AutoSize = True
        LblVelocidade.ForeColor = Color.White
        LblVelocidade.Location = New Point(22, 90)
        LblVelocidade.Margin = New Padding(4, 0, 4, 0)
        LblVelocidade.Name = "LblVelocidade"
        LblVelocidade.Size = New Size(95, 16)
        LblVelocidade.TabIndex = 3
        LblVelocidade.Text = "VELOCIDADE:"
        ' 
        ' LblETA
        ' 
        LblETA.AutoSize = True
        LblETA.ForeColor = Color.White
        LblETA.Location = New Point(22, 112)
        LblETA.Margin = New Padding(4, 0, 4, 0)
        LblETA.Name = "LblETA"
        LblETA.Size = New Size(39, 16)
        LblETA.TabIndex = 4
        LblETA.Text = "ETA:"
        ' 
        ' LblStatus
        ' 
        LblStatus.AutoSize = True
        LblStatus.ForeColor = Color.White
        LblStatus.Location = New Point(22, 133)
        LblStatus.Margin = New Padding(4, 0, 4, 0)
        LblStatus.Name = "LblStatus"
        LblStatus.Size = New Size(63, 16)
        LblStatus.TabIndex = 5
        LblStatus.Text = "STATUS:"
        ' 
        ' TxtLog
        ' 
        TxtLog.Enabled = False
        TxtLog.Font = New Font("Monocraft", 8.999999F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        TxtLog.Location = New Point(22, 154)
        TxtLog.Margin = New Padding(4, 3, 4, 3)
        TxtLog.Name = "TxtLog"
        TxtLog.Size = New Size(589, 71)
        TxtLog.TabIndex = 6
        TxtLog.Text = ""
        ' 
        ' BtnSelecionarPlaylist
        ' 
        BtnSelecionarPlaylist.Font = New Font("Monocraft", 8.999999F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        BtnSelecionarPlaylist.Location = New Point(22, 251)
        BtnSelecionarPlaylist.Margin = New Padding(4, 3, 4, 3)
        BtnSelecionarPlaylist.Name = "BtnSelecionarPlaylist"
        BtnSelecionarPlaylist.Size = New Size(124, 43)
        BtnSelecionarPlaylist.TabIndex = 7
        BtnSelecionarPlaylist.Text = "Selecionar Playlist"
        BtnSelecionarPlaylist.UseVisualStyleBackColor = True
        ' 
        ' Panel1
        ' 
        Panel1.BackColor = SystemColors.ControlDarkDark
        Panel1.Controls.Add(BtnRetornar)
        Panel1.Location = New Point(-6, 347)
        Panel1.Margin = New Padding(4, 3, 4, 3)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(662, 62)
        Panel1.TabIndex = 8
        ' 
        ' BtnRetornar
        ' 
        BtnRetornar.Font = New Font("Monocraft", 8.999999F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        BtnRetornar.Location = New Point(284, 15)
        BtnRetornar.Margin = New Padding(4, 3, 4, 3)
        BtnRetornar.Name = "BtnRetornar"
        BtnRetornar.Size = New Size(100, 34)
        BtnRetornar.TabIndex = 1
        BtnRetornar.Text = "Retornar"
        BtnRetornar.UseVisualStyleBackColor = True
        ' 
        ' BtnCancelar
        ' 
        BtnCancelar.Enabled = False
        BtnCancelar.Font = New Font("Monocraft", 8.999999F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        BtnCancelar.Location = New Point(327, 262)
        BtnCancelar.Margin = New Padding(4, 3, 4, 3)
        BtnCancelar.Name = "BtnCancelar"
        BtnCancelar.Size = New Size(91, 21)
        BtnCancelar.TabIndex = 9
        BtnCancelar.Text = "Cancelar"
        BtnCancelar.UseVisualStyleBackColor = True
        BtnCancelar.Visible = False
        ' 
        ' BtnConversor
        ' 
        BtnConversor.Font = New Font("Monocraft", 8.999999F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        BtnConversor.Location = New Point(173, 251)
        BtnConversor.Margin = New Padding(4, 3, 4, 3)
        BtnConversor.Name = "BtnConversor"
        BtnConversor.Size = New Size(124, 42)
        BtnConversor.TabIndex = 8
        BtnConversor.Text = "Converter para Mp3" & vbCrLf
        BtnConversor.UseVisualStyleBackColor = True
        ' 
        ' GrpDownload
        ' 
        GrpDownload.Controls.Add(LblVideo)
        GrpDownload.Controls.Add(BtnCancelar)
        GrpDownload.Controls.Add(BtnConversor)
        GrpDownload.Controls.Add(PbDownload)
        GrpDownload.Controls.Add(LblNome)
        GrpDownload.Controls.Add(LblVelocidade)
        GrpDownload.Controls.Add(TxtLog)
        GrpDownload.Controls.Add(LblETA)
        GrpDownload.Controls.Add(LblStatus)
        GrpDownload.Controls.Add(BtnSelecionarPlaylist)
        GrpDownload.Location = New Point(12, 12)
        GrpDownload.Name = "GrpDownload"
        GrpDownload.Size = New Size(632, 316)
        GrpDownload.TabIndex = 10
        GrpDownload.TabStop = False
        ' 
        ' FrmDownloadsConversor
        ' 
        AutoScaleDimensions = New SizeF(8F, 16F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(134), CByte(29), CByte(29))
        ClientSize = New Size(656, 409)
        Controls.Add(GrpDownload)
        Controls.Add(Panel1)
        Font = New Font("Monocraft", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        FormBorderStyle = FormBorderStyle.FixedSingle
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Margin = New Padding(4, 3, 4, 3)
        MaximizeBox = False
        MdiChildrenMinimizedAnchorBottom = False
        MinimizeBox = False
        Name = "FrmDownloadsConversor"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Downloads"
        Panel1.ResumeLayout(False)
        GrpDownload.ResumeLayout(False)
        GrpDownload.PerformLayout()
        ResumeLayout(False)
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
    Friend WithEvents GrpDownload As GroupBox
End Class
