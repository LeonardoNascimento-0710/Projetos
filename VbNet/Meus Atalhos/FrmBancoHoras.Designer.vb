<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmBancoHoras
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmBancoHoras))
        GrpBanco = New GroupBox()
        ChkUtilizado = New CheckBox()
        TxtOs = New TextBox()
        LblOs = New Label()
        TxtSemData = New TextBox()
        ChkSemData = New CheckBox()
        TxtMotivo = New TextBox()
        LblMotivo = New Label()
        CmbLoja = New ComboBox()
        TxtDiasFora = New TextBox()
        LblDiasFora = New Label()
        TxtExtra = New TextBox()
        LblExtra = New Label()
        LblFim = New Label()
        DtpFinal = New DateTimePicker()
        LblInicio = New Label()
        Button1 = New Button()
        DtpInicio = New DateTimePicker()
        LblLoja = New Label()
        BtnGravar = New Button()
        BtnRetornar = New Button()
        GrpHist = New GroupBox()
        DtHist = New DataGridView()
        BtnExportar = New Button()
        TxtQntdUlt = New TextBox()
        LblQtd = New Label()
        GrpBanco.SuspendLayout()
        GrpHist.SuspendLayout()
        CType(DtHist, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' GrpBanco
        ' 
        GrpBanco.Controls.Add(TxtQntdUlt)
        GrpBanco.Controls.Add(LblQtd)
        GrpBanco.Controls.Add(ChkUtilizado)
        GrpBanco.Controls.Add(TxtOs)
        GrpBanco.Controls.Add(LblOs)
        GrpBanco.Controls.Add(TxtSemData)
        GrpBanco.Controls.Add(ChkSemData)
        GrpBanco.Controls.Add(TxtMotivo)
        GrpBanco.Controls.Add(LblMotivo)
        GrpBanco.Controls.Add(CmbLoja)
        GrpBanco.Controls.Add(TxtDiasFora)
        GrpBanco.Controls.Add(LblDiasFora)
        GrpBanco.Controls.Add(TxtExtra)
        GrpBanco.Controls.Add(LblExtra)
        GrpBanco.Controls.Add(LblFim)
        GrpBanco.Controls.Add(DtpFinal)
        GrpBanco.Controls.Add(LblInicio)
        GrpBanco.Controls.Add(Button1)
        GrpBanco.Controls.Add(DtpInicio)
        GrpBanco.Controls.Add(LblLoja)
        GrpBanco.Location = New Point(12, 12)
        GrpBanco.Name = "GrpBanco"
        GrpBanco.Size = New Size(369, 290)
        GrpBanco.TabIndex = 0
        GrpBanco.TabStop = False
        ' 
        ' ChkUtilizado
        ' 
        ChkUtilizado.AutoSize = True
        ChkUtilizado.Location = New Point(183, 217)
        ChkUtilizado.Name = "ChkUtilizado"
        ChkUtilizado.Size = New Size(85, 19)
        ChkUtilizado.TabIndex = 19
        ChkUtilizado.Text = "UTILIZADO"
        ChkUtilizado.UseVisualStyleBackColor = True
        ' 
        ' TxtOs
        ' 
        TxtOs.Location = New Point(275, 166)
        TxtOs.Name = "TxtOs"
        TxtOs.Size = New Size(74, 23)
        TxtOs.TabIndex = 20
        ' 
        ' LblOs
        ' 
        LblOs.AutoSize = True
        LblOs.Location = New Point(275, 147)
        LblOs.Name = "LblOs"
        LblOs.Size = New Size(22, 15)
        LblOs.TabIndex = 19
        LblOs.Text = "OS"
        ' 
        ' TxtSemData
        ' 
        TxtSemData.Location = New Point(21, 99)
        TxtSemData.Name = "TxtSemData"
        TxtSemData.Size = New Size(231, 23)
        TxtSemData.TabIndex = 18
        TxtSemData.Visible = False
        ' 
        ' ChkSemData
        ' 
        ChkSemData.AutoSize = True
        ChkSemData.Location = New Point(69, 80)
        ChkSemData.Name = "ChkSemData"
        ChkSemData.Size = New Size(81, 19)
        ChkSemData.TabIndex = 18
        ChkSemData.Text = "SEM DATA"
        ChkSemData.UseVisualStyleBackColor = True
        ' 
        ' TxtMotivo
        ' 
        TxtMotivo.Location = New Point(125, 242)
        TxtMotivo.Name = "TxtMotivo"
        TxtMotivo.Size = New Size(224, 23)
        TxtMotivo.TabIndex = 17
        ' 
        ' LblMotivo
        ' 
        LblMotivo.AutoSize = True
        LblMotivo.Location = New Point(125, 221)
        LblMotivo.Name = "LblMotivo"
        LblMotivo.Size = New Size(52, 15)
        LblMotivo.TabIndex = 16
        LblMotivo.Text = "MOTIVO"
        ' 
        ' CmbLoja
        ' 
        CmbLoja.FormattingEnabled = True
        CmbLoja.Location = New Point(21, 44)
        CmbLoja.Name = "CmbLoja"
        CmbLoja.Size = New Size(328, 23)
        CmbLoja.TabIndex = 15
        ' 
        ' TxtDiasFora
        ' 
        TxtDiasFora.Location = New Point(275, 99)
        TxtDiasFora.Name = "TxtDiasFora"
        TxtDiasFora.Size = New Size(74, 23)
        TxtDiasFora.TabIndex = 14
        ' 
        ' LblDiasFora
        ' 
        LblDiasFora.AutoSize = True
        LblDiasFora.Location = New Point(275, 80)
        LblDiasFora.Name = "LblDiasFora"
        LblDiasFora.Size = New Size(65, 15)
        LblDiasFora.TabIndex = 13
        LblDiasFora.Text = "DIAS FORA"
        ' 
        ' TxtExtra
        ' 
        TxtExtra.Location = New Point(21, 242)
        TxtExtra.Name = "TxtExtra"
        TxtExtra.Size = New Size(84, 23)
        TxtExtra.TabIndex = 10
        ' 
        ' LblExtra
        ' 
        LblExtra.AutoSize = True
        LblExtra.Location = New Point(21, 221)
        LblExtra.Name = "LblExtra"
        LblExtra.Size = New Size(84, 15)
        LblExtra.TabIndex = 9
        LblExtra.Text = "HORAS EXTRA"
        ' 
        ' LblFim
        ' 
        LblFim.AutoSize = True
        LblFim.Location = New Point(21, 148)
        LblFim.Name = "LblFim"
        LblFim.Size = New Size(27, 15)
        LblFim.TabIndex = 8
        LblFim.Text = "FIM"
        ' 
        ' DtpFinal
        ' 
        DtpFinal.ImeMode = ImeMode.NoControl
        DtpFinal.Location = New Point(21, 166)
        DtpFinal.Name = "DtpFinal"
        DtpFinal.Size = New Size(231, 23)
        DtpFinal.TabIndex = 7
        DtpFinal.Value = New Date(2026, 1, 19, 11, 47, 59, 0)
        ' 
        ' LblInicio
        ' 
        LblInicio.AutoSize = True
        LblInicio.Location = New Point(21, 81)
        LblInicio.Name = "LblInicio"
        LblInicio.Size = New Size(42, 15)
        LblInicio.TabIndex = 6
        LblInicio.Text = "INICIO"
        ' 
        ' Button1
        ' 
        Button1.Location = New Point(386, 257)
        Button1.Name = "Button1"
        Button1.Size = New Size(15, 8)
        Button1.TabIndex = 5
        Button1.Text = "Button1"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' DtpInicio
        ' 
        DtpInicio.Location = New Point(21, 99)
        DtpInicio.Name = "DtpInicio"
        DtpInicio.Size = New Size(231, 23)
        DtpInicio.TabIndex = 4
        DtpInicio.Value = New Date(2026, 1, 19, 11, 47, 52, 0)
        ' 
        ' LblLoja
        ' 
        LblLoja.AutoSize = True
        LblLoja.Location = New Point(21, 26)
        LblLoja.Name = "LblLoja"
        LblLoja.Size = New Size(34, 15)
        LblLoja.TabIndex = 1
        LblLoja.Text = "LOJA"
        ' 
        ' BtnGravar
        ' 
        BtnGravar.Location = New Point(338, 308)
        BtnGravar.Name = "BtnGravar"
        BtnGravar.Size = New Size(87, 49)
        BtnGravar.TabIndex = 0
        BtnGravar.Text = "Gravar" + vbCr + " Registro"
        BtnGravar.UseVisualStyleBackColor = True
        ' 
        ' BtnRetornar
        ' 
        BtnRetornar.Location = New Point(556, 308)
        BtnRetornar.Name = "BtnRetornar"
        BtnRetornar.Size = New Size(87, 49)
        BtnRetornar.TabIndex = 1
        BtnRetornar.Text = "Retornar"
        BtnRetornar.UseVisualStyleBackColor = True
        ' 
        ' GrpHist
        ' 
        GrpHist.Controls.Add(DtHist)
        GrpHist.Location = New Point(398, 12)
        GrpHist.Name = "GrpHist"
        GrpHist.Size = New Size(605, 290)
        GrpHist.TabIndex = 2
        GrpHist.TabStop = False
        GrpHist.Text = "histórico"
        ' 
        ' DtHist
        ' 
        DtHist.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DtHist.Location = New Point(32, 40)
        DtHist.Name = "DtHist"
        DtHist.Size = New Size(542, 221)
        DtHist.TabIndex = 0
        ' 
        ' BtnExportar
        ' 
        BtnExportar.Location = New Point(447, 307)
        BtnExportar.Name = "BtnExportar"
        BtnExportar.Size = New Size(87, 49)
        BtnExportar.TabIndex = 3
        BtnExportar.Text = "Exportar Registros"
        BtnExportar.UseVisualStyleBackColor = True
        ' 
        ' TxtQntdUlt
        ' 
        TxtQntdUlt.Location = New Point(274, 213)
        TxtQntdUlt.Name = "TxtQntdUlt"
        TxtQntdUlt.Size = New Size(74, 23)
        TxtQntdUlt.TabIndex = 22
        ' 
        ' LblQtd
        ' 
        LblQtd.AutoSize = True
        LblQtd.Location = New Point(275, 195)
        LblQtd.Name = "LblQtd"
        LblQtd.Size = New Size(81, 15)
        LblQtd.TabIndex = 21
        LblQtd.Text = "QUANTIDADE"
        ' 
        ' FrmBancoHoras
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1024, 368)
        ControlBox = False
        Controls.Add(BtnExportar)
        Controls.Add(GrpHist)
        Controls.Add(BtnRetornar)
        Controls.Add(BtnGravar)
        Controls.Add(GrpBanco)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "FrmBancoHoras"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Banco de Horas"
        GrpBanco.ResumeLayout(False)
        GrpBanco.PerformLayout()
        GrpHist.ResumeLayout(False)
        CType(DtHist, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents GrpBanco As GroupBox
    Friend WithEvents BtnGravar As Button
    Friend WithEvents BtnRetornar As Button
    Friend WithEvents LblLoja As Label
    Friend WithEvents LblFim As Label
    Friend WithEvents DtpFinal As DateTimePicker
    Friend WithEvents LblInicio As Label
    Friend WithEvents Button1 As Button
    Friend WithEvents DtpInicio As DateTimePicker
    Friend WithEvents TxtExtra As TextBox
    Friend WithEvents LblExtra As Label
    Friend WithEvents TxtDiasFora As TextBox
    Friend WithEvents LblDiasFora As Label
    Friend WithEvents TxtMotivo As TextBox
    Friend WithEvents LblMotivo As Label
    Friend WithEvents CmbLoja As ComboBox
    Friend WithEvents TxtSemData As TextBox
    Friend WithEvents ChkSemData As CheckBox
    Friend WithEvents GrpHist As GroupBox
    Friend WithEvents DtHist As DataGridView
    Friend WithEvents TxtOs As TextBox
    Friend WithEvents LblOs As Label
    Friend WithEvents ChkUtilizado As CheckBox
    Friend WithEvents BtnExportar As Button
    Friend WithEvents TxtQntdUlt As TextBox
    Friend WithEvents LblQtd As Label
End Class
