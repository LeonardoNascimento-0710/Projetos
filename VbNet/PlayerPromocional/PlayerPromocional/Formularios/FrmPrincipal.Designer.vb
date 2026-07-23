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
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmPrincipal))
        PnlTopo = New Panel()
        LblTitulo = New Label()
        PnlPlaylist = New Panel()
        ChkOrdemAleatoria = New CheckBox()
        BtnAdicionarPasta = New Button()
        BtnLimparPlaylist = New Button()
        BtnRemoverMusica = New Button()
        BtnAdicionarMusicas = New Button()
        LstMusicas = New ListBox()
        LblPlaylist = New Label()
        PnlPromocao = New Panel()
        BtnGerenciarAnuncios = New Button()
        RdbAnuncioGravado = New RadioButton()
        RdbAnuncioTexto = New RadioButton()
        ChkAtivarPromocao = New CheckBox()
        TxtPromocao = New TextBox()
        BtnTestarVoz = New Button()
        LblMusicas = New Label()
        NudIntervalo = New NumericUpDown()
        LblIntervalo = New Label()
        LblPromocao = New Label()
        BtnAnterior = New Button()
        BtnPlay = New Button()
        BtnPausar = New Button()
        BtnProxima = New Button()
        BtnParar = New Button()
        LblMusicaAtual = New Label()
        OfdMusicas = New OpenFileDialog()
        FbdPlaylist = New FolderBrowserDialog()
        TrkProgresso = New TrackBar()
        LblTempoAtual = New Label()
        LblTempoTotal = New Label()
        LblVolume = New Label()
        TrkVolume = New TrackBar()
        TmrPlayer = New Timer(components)
        OfdAnuncioGravado = New OpenFileDialog()
        PnlTopo.SuspendLayout()
        PnlPlaylist.SuspendLayout()
        PnlPromocao.SuspendLayout()
        CType(NudIntervalo, ComponentModel.ISupportInitialize).BeginInit()
        CType(TrkProgresso, ComponentModel.ISupportInitialize).BeginInit()
        CType(TrkVolume, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' PnlTopo
        ' 
        PnlTopo.BackColor = Color.FromArgb(CByte(134), CByte(29), CByte(29))
        PnlTopo.Controls.Add(LblTitulo)
        PnlTopo.Dock = DockStyle.Top
        PnlTopo.Location = New Point(0, 0)
        PnlTopo.Name = "PnlTopo"
        PnlTopo.Size = New Size(1084, 65)
        PnlTopo.TabIndex = 0
        ' 
        ' LblTitulo
        ' 
        LblTitulo.AutoSize = True
        LblTitulo.BackColor = Color.Transparent
        LblTitulo.Font = New Font("Segoe UI Semibold", 18F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        LblTitulo.ForeColor = Color.White
        LblTitulo.Location = New Point(25, 17)
        LblTitulo.Name = "LblTitulo"
        LblTitulo.Size = New Size(276, 32)
        LblTitulo.TabIndex = 0
        LblTitulo.Text = "PLAYER PROMOCIONAL"
        ' 
        ' PnlPlaylist
        ' 
        PnlPlaylist.BackColor = Color.FromArgb(CByte(38), CByte(38), CByte(38))
        PnlPlaylist.Controls.Add(ChkOrdemAleatoria)
        PnlPlaylist.Controls.Add(BtnAdicionarPasta)
        PnlPlaylist.Controls.Add(BtnLimparPlaylist)
        PnlPlaylist.Controls.Add(BtnRemoverMusica)
        PnlPlaylist.Controls.Add(BtnAdicionarMusicas)
        PnlPlaylist.Controls.Add(LstMusicas)
        PnlPlaylist.Controls.Add(LblPlaylist)
        PnlPlaylist.Location = New Point(20, 85)
        PnlPlaylist.Name = "PnlPlaylist"
        PnlPlaylist.Size = New Size(500, 475)
        PnlPlaylist.TabIndex = 1
        ' 
        ' ChkOrdemAleatoria
        ' 
        ChkOrdemAleatoria.AutoSize = True
        ChkOrdemAleatoria.BackColor = Color.Transparent
        ChkOrdemAleatoria.ForeColor = Color.White
        ChkOrdemAleatoria.Location = New Point(358, 17)
        ChkOrdemAleatoria.Name = "ChkOrdemAleatoria"
        ChkOrdemAleatoria.Size = New Size(127, 21)
        ChkOrdemAleatoria.TabIndex = 7
        ChkOrdemAleatoria.Text = "Ordem aleatória"
        ChkOrdemAleatoria.UseVisualStyleBackColor = False
        ' 
        ' BtnAdicionarPasta
        ' 
        BtnAdicionarPasta.BackColor = Color.FromArgb(CByte(50), CByte(50), CByte(50))
        BtnAdicionarPasta.FlatAppearance.MouseDownBackColor = Color.FromArgb(CByte(70), CByte(70), CByte(70))
        BtnAdicionarPasta.FlatAppearance.MouseOverBackColor = Color.FromArgb(CByte(35), CByte(35), CByte(35))
        BtnAdicionarPasta.FlatStyle = FlatStyle.Flat
        BtnAdicionarPasta.ForeColor = Color.White
        BtnAdicionarPasta.Location = New Point(371, 410)
        BtnAdicionarPasta.Name = "BtnAdicionarPasta"
        BtnAdicionarPasta.Size = New Size(114, 40)
        BtnAdicionarPasta.TabIndex = 6
        BtnAdicionarPasta.Text = "Adicionar pasta"
        BtnAdicionarPasta.UseVisualStyleBackColor = False
        ' 
        ' BtnLimparPlaylist
        ' 
        BtnLimparPlaylist.BackColor = Color.FromArgb(CByte(60), CByte(60), CByte(60))
        BtnLimparPlaylist.Cursor = Cursors.Hand
        BtnLimparPlaylist.FlatAppearance.BorderSize = 0
        BtnLimparPlaylist.FlatAppearance.MouseDownBackColor = Color.FromArgb(CByte(70), CByte(70), CByte(70))
        BtnLimparPlaylist.FlatAppearance.MouseOverBackColor = Color.FromArgb(CByte(35), CByte(35), CByte(35))
        BtnLimparPlaylist.FlatStyle = FlatStyle.Flat
        BtnLimparPlaylist.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        BtnLimparPlaylist.ForeColor = Color.White
        BtnLimparPlaylist.Location = New Point(261, 410)
        BtnLimparPlaylist.Name = "BtnLimparPlaylist"
        BtnLimparPlaylist.Size = New Size(104, 40)
        BtnLimparPlaylist.TabIndex = 5
        BtnLimparPlaylist.Text = "Limpar"
        BtnLimparPlaylist.UseVisualStyleBackColor = False
        ' 
        ' BtnRemoverMusica
        ' 
        BtnRemoverMusica.BackColor = Color.FromArgb(CByte(60), CByte(60), CByte(60))
        BtnRemoverMusica.Cursor = Cursors.Hand
        BtnRemoverMusica.FlatAppearance.BorderSize = 0
        BtnRemoverMusica.FlatAppearance.MouseDownBackColor = Color.FromArgb(CByte(70), CByte(70), CByte(70))
        BtnRemoverMusica.FlatAppearance.MouseOverBackColor = Color.FromArgb(CByte(35), CByte(35), CByte(35))
        BtnRemoverMusica.FlatStyle = FlatStyle.Flat
        BtnRemoverMusica.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        BtnRemoverMusica.ForeColor = Color.White
        BtnRemoverMusica.Location = New Point(148, 410)
        BtnRemoverMusica.Name = "BtnRemoverMusica"
        BtnRemoverMusica.Size = New Size(104, 40)
        BtnRemoverMusica.TabIndex = 4
        BtnRemoverMusica.Text = "Remover"
        BtnRemoverMusica.UseVisualStyleBackColor = False
        ' 
        ' BtnAdicionarMusicas
        ' 
        BtnAdicionarMusicas.BackColor = Color.FromArgb(CByte(134), CByte(29), CByte(29))
        BtnAdicionarMusicas.Cursor = Cursors.Hand
        BtnAdicionarMusicas.FlatAppearance.BorderSize = 0
        BtnAdicionarMusicas.FlatAppearance.MouseDownBackColor = Color.FromArgb(CByte(70), CByte(70), CByte(70))
        BtnAdicionarMusicas.FlatAppearance.MouseOverBackColor = Color.FromArgb(CByte(35), CByte(35), CByte(35))
        BtnAdicionarMusicas.FlatStyle = FlatStyle.Flat
        BtnAdicionarMusicas.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        BtnAdicionarMusicas.ForeColor = Color.White
        BtnAdicionarMusicas.Location = New Point(15, 410)
        BtnAdicionarMusicas.Name = "BtnAdicionarMusicas"
        BtnAdicionarMusicas.Size = New Size(127, 40)
        BtnAdicionarMusicas.TabIndex = 3
        BtnAdicionarMusicas.Text = "Adicionar músicas"
        BtnAdicionarMusicas.UseVisualStyleBackColor = False
        ' 
        ' LstMusicas
        ' 
        LstMusicas.AllowDrop = True
        LstMusicas.BackColor = Color.FromArgb(CByte(48), CByte(48), CByte(48))
        LstMusicas.BorderStyle = BorderStyle.FixedSingle
        LstMusicas.ForeColor = Color.White
        LstMusicas.FormattingEnabled = True
        LstMusicas.HorizontalScrollbar = True
        LstMusicas.Location = New Point(15, 50)
        LstMusicas.Name = "LstMusicas"
        LstMusicas.Size = New Size(470, 342)
        LstMusicas.TabIndex = 2
        ' 
        ' LblPlaylist
        ' 
        LblPlaylist.AutoSize = True
        LblPlaylist.BackColor = Color.Transparent
        LblPlaylist.Font = New Font("Segoe UI Semibold", 12F, FontStyle.Bold)
        LblPlaylist.ForeColor = Color.White
        LblPlaylist.Location = New Point(15, 15)
        LblPlaylist.Name = "LblPlaylist"
        LblPlaylist.Size = New Size(174, 21)
        LblPlaylist.TabIndex = 1
        LblPlaylist.Text = "PLAYLIST DE MÚSICAS"
        ' 
        ' PnlPromocao
        ' 
        PnlPromocao.BackColor = Color.FromArgb(CByte(38), CByte(38), CByte(38))
        PnlPromocao.Controls.Add(BtnGerenciarAnuncios)
        PnlPromocao.Controls.Add(RdbAnuncioGravado)
        PnlPromocao.Controls.Add(RdbAnuncioTexto)
        PnlPromocao.Controls.Add(ChkAtivarPromocao)
        PnlPromocao.Controls.Add(TxtPromocao)
        PnlPromocao.Controls.Add(BtnTestarVoz)
        PnlPromocao.Controls.Add(LblMusicas)
        PnlPromocao.Controls.Add(NudIntervalo)
        PnlPromocao.Controls.Add(LblIntervalo)
        PnlPromocao.Controls.Add(LblPromocao)
        PnlPromocao.Location = New Point(540, 85)
        PnlPromocao.Name = "PnlPromocao"
        PnlPromocao.Size = New Size(525, 475)
        PnlPromocao.TabIndex = 2
        ' 
        ' BtnGerenciarAnuncios
        ' 
        BtnGerenciarAnuncios.BackColor = Color.FromArgb(CByte(50), CByte(50), CByte(50))
        BtnGerenciarAnuncios.Cursor = Cursors.Hand
        BtnGerenciarAnuncios.FlatAppearance.BorderSize = 0
        BtnGerenciarAnuncios.FlatStyle = FlatStyle.Flat
        BtnGerenciarAnuncios.ForeColor = Color.White
        BtnGerenciarAnuncios.Location = New Point(17, 385)
        BtnGerenciarAnuncios.Name = "BtnGerenciarAnuncios"
        BtnGerenciarAnuncios.Size = New Size(160, 38)
        BtnGerenciarAnuncios.TabIndex = 11
        BtnGerenciarAnuncios.Text = "Selecionar anúncios"
        BtnGerenciarAnuncios.UseVisualStyleBackColor = False
        ' 
        ' RdbAnuncioGravado
        ' 
        RdbAnuncioGravado.AutoSize = True
        RdbAnuncioGravado.ForeColor = Color.White
        RdbAnuncioGravado.Location = New Point(17, 348)
        RdbAnuncioGravado.Name = "RdbAnuncioGravado"
        RdbAnuncioGravado.Size = New Size(160, 21)
        RdbAnuncioGravado.TabIndex = 10
        RdbAnuncioGravado.Text = "Usar anúncio gravado"
        RdbAnuncioGravado.UseVisualStyleBackColor = True
        ' 
        ' RdbAnuncioTexto
        ' 
        RdbAnuncioTexto.AutoSize = True
        RdbAnuncioTexto.Checked = True
        RdbAnuncioTexto.ForeColor = Color.White
        RdbAnuncioTexto.Location = New Point(15, 312)
        RdbAnuncioTexto.Name = "RdbAnuncioTexto"
        RdbAnuncioTexto.Size = New Size(179, 21)
        RdbAnuncioTexto.TabIndex = 9
        RdbAnuncioTexto.TabStop = True
        RdbAnuncioTexto.Text = "Gerar anúncio pelo texto"
        RdbAnuncioTexto.UseVisualStyleBackColor = True
        ' 
        ' ChkAtivarPromocao
        ' 
        ChkAtivarPromocao.AutoSize = True
        ChkAtivarPromocao.BackColor = Color.Transparent
        ChkAtivarPromocao.Checked = True
        ChkAtivarPromocao.CheckState = CheckState.Checked
        ChkAtivarPromocao.ForeColor = Color.White
        ChkAtivarPromocao.Location = New Point(316, 17)
        ChkAtivarPromocao.Name = "ChkAtivarPromocao"
        ChkAtivarPromocao.Size = New Size(194, 21)
        ChkAtivarPromocao.TabIndex = 8
        ChkAtivarPromocao.Text = "Ativar narração automática"
        ChkAtivarPromocao.UseVisualStyleBackColor = False
        ' 
        ' TxtPromocao
        ' 
        TxtPromocao.AcceptsReturn = True
        TxtPromocao.BackColor = Color.FromArgb(CByte(48), CByte(48), CByte(48))
        TxtPromocao.BorderStyle = BorderStyle.FixedSingle
        TxtPromocao.ForeColor = Color.White
        TxtPromocao.Location = New Point(15, 102)
        TxtPromocao.MaxLength = 500
        TxtPromocao.Multiline = True
        TxtPromocao.Name = "TxtPromocao"
        TxtPromocao.ScrollBars = ScrollBars.Vertical
        TxtPromocao.Size = New Size(495, 150)
        TxtPromocao.TabIndex = 5
        ' 
        ' BtnTestarVoz
        ' 
        BtnTestarVoz.BackColor = Color.FromArgb(CByte(134), CByte(29), CByte(29))
        BtnTestarVoz.Cursor = Cursors.Hand
        BtnTestarVoz.FlatAppearance.BorderSize = 0
        BtnTestarVoz.FlatAppearance.MouseDownBackColor = Color.FromArgb(CByte(155), CByte(35), CByte(35))
        BtnTestarVoz.FlatAppearance.MouseOverBackColor = Color.FromArgb(CByte(105), CByte(22), CByte(22))
        BtnTestarVoz.FlatStyle = FlatStyle.Flat
        BtnTestarVoz.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        BtnTestarVoz.ForeColor = Color.White
        BtnTestarVoz.Location = New Point(350, 272)
        BtnTestarVoz.Name = "BtnTestarVoz"
        BtnTestarVoz.Size = New Size(160, 40)
        BtnTestarVoz.TabIndex = 4
        BtnTestarVoz.Text = "Testar Promoção"
        BtnTestarVoz.UseVisualStyleBackColor = False
        ' 
        ' LblMusicas
        ' 
        LblMusicas.AutoSize = True
        LblMusicas.BackColor = Color.Transparent
        LblMusicas.Font = New Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        LblMusicas.ForeColor = Color.FromArgb(CByte(210), CByte(210), CByte(210))
        LblMusicas.Location = New Point(247, 283)
        LblMusicas.Name = "LblMusicas"
        LblMusicas.Size = New Size(67, 17)
        LblMusicas.TabIndex = 3
        LblMusicas.Text = "Música(s)"
        ' 
        ' NudIntervalo
        ' 
        NudIntervalo.BorderStyle = BorderStyle.FixedSingle
        NudIntervalo.Location = New Point(181, 281)
        NudIntervalo.Maximum = New Decimal(New Integer() {20, 0, 0, 0})
        NudIntervalo.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        NudIntervalo.Name = "NudIntervalo"
        NudIntervalo.Size = New Size(60, 25)
        NudIntervalo.TabIndex = 2
        NudIntervalo.TextAlign = HorizontalAlignment.Center
        NudIntervalo.Value = New Decimal(New Integer() {1, 0, 0, 0})
        ' 
        ' LblIntervalo
        ' 
        LblIntervalo.AutoSize = True
        LblIntervalo.BackColor = Color.Transparent
        LblIntervalo.Font = New Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        LblIntervalo.ForeColor = Color.FromArgb(CByte(210), CByte(210), CByte(210))
        LblIntervalo.Location = New Point(15, 283)
        LblIntervalo.Name = "LblIntervalo"
        LblIntervalo.Size = New Size(160, 17)
        LblIntervalo.TabIndex = 1
        LblIntervalo.Text = "Narrar promoção a cada:"
        ' 
        ' LblPromocao
        ' 
        LblPromocao.AutoSize = True
        LblPromocao.BackColor = Color.Transparent
        LblPromocao.Font = New Font("Segoe UI Semibold", 12F, FontStyle.Bold)
        LblPromocao.ForeColor = Color.White
        LblPromocao.Location = New Point(15, 15)
        LblPromocao.Name = "LblPromocao"
        LblPromocao.Size = New Size(203, 21)
        LblPromocao.TabIndex = 0
        LblPromocao.Text = "ANUNCIO PROMOCIONAL"
        ' 
        ' BtnAnterior
        ' 
        BtnAnterior.BackColor = Color.FromArgb(CByte(50), CByte(50), CByte(50))
        BtnAnterior.FlatStyle = FlatStyle.Flat
        BtnAnterior.ForeColor = Color.White
        BtnAnterior.Location = New Point(247, 656)
        BtnAnterior.Name = "BtnAnterior"
        BtnAnterior.Size = New Size(110, 40)
        BtnAnterior.TabIndex = 3
        BtnAnterior.Text = "Anterior"
        BtnAnterior.UseVisualStyleBackColor = False
        ' 
        ' BtnPlay
        ' 
        BtnPlay.BackColor = Color.FromArgb(CByte(134), CByte(29), CByte(29))
        BtnPlay.FlatStyle = FlatStyle.Flat
        BtnPlay.ForeColor = Color.White
        BtnPlay.Location = New Point(367, 656)
        BtnPlay.Name = "BtnPlay"
        BtnPlay.Size = New Size(110, 40)
        BtnPlay.TabIndex = 4
        BtnPlay.Text = "Reproduzir"
        BtnPlay.UseVisualStyleBackColor = False
        ' 
        ' BtnPausar
        ' 
        BtnPausar.BackColor = Color.FromArgb(CByte(50), CByte(50), CByte(50))
        BtnPausar.FlatStyle = FlatStyle.Flat
        BtnPausar.ForeColor = Color.White
        BtnPausar.Location = New Point(487, 656)
        BtnPausar.Name = "BtnPausar"
        BtnPausar.Size = New Size(110, 40)
        BtnPausar.TabIndex = 5
        BtnPausar.Text = "Pausar"
        BtnPausar.UseVisualStyleBackColor = False
        ' 
        ' BtnProxima
        ' 
        BtnProxima.BackColor = Color.FromArgb(CByte(50), CByte(50), CByte(50))
        BtnProxima.FlatStyle = FlatStyle.Flat
        BtnProxima.ForeColor = Color.White
        BtnProxima.Location = New Point(607, 656)
        BtnProxima.Name = "BtnProxima"
        BtnProxima.Size = New Size(110, 40)
        BtnProxima.TabIndex = 6
        BtnProxima.Text = "Próxima"
        BtnProxima.UseVisualStyleBackColor = False
        ' 
        ' BtnParar
        ' 
        BtnParar.BackColor = Color.FromArgb(CByte(50), CByte(50), CByte(50))
        BtnParar.FlatStyle = FlatStyle.Flat
        BtnParar.ForeColor = Color.White
        BtnParar.Location = New Point(727, 656)
        BtnParar.Name = "BtnParar"
        BtnParar.Size = New Size(110, 40)
        BtnParar.TabIndex = 7
        BtnParar.Text = "Parar"
        BtnParar.UseVisualStyleBackColor = False
        ' 
        ' LblMusicaAtual
        ' 
        LblMusicaAtual.BackColor = Color.Transparent
        LblMusicaAtual.ForeColor = Color.FromArgb(CByte(190), CByte(190), CByte(190))
        LblMusicaAtual.Location = New Point(20, 618)
        LblMusicaAtual.Name = "LblMusicaAtual"
        LblMusicaAtual.Size = New Size(1045, 35)
        LblMusicaAtual.TabIndex = 8
        LblMusicaAtual.Text = "Nenhuma música sendo reproduzida"
        LblMusicaAtual.TextAlign = ContentAlignment.MiddleCenter
        ' 
        ' OfdMusicas
        ' 
        OfdMusicas.FileName = "OfdMusicas"
        OfdMusicas.Filter = "Arquivos de áudio|*.mp3;*.wav;*.wma;*.aac;*.m4a|Todos os arquivos|*.*"
        OfdMusicas.Multiselect = True
        OfdMusicas.Title = "Selecione as músicas"
        ' 
        ' FbdPlaylist
        ' 
        FbdPlaylist.Description = "Selecione a pasta que contém as músicas"
        FbdPlaylist.ShowNewFolderButton = False
        ' 
        ' TrkProgresso
        ' 
        TrkProgresso.Cursor = Cursors.Hand
        TrkProgresso.Location = New Point(90, 570)
        TrkProgresso.Maximum = 100
        TrkProgresso.Name = "TrkProgresso"
        TrkProgresso.Size = New Size(920, 45)
        TrkProgresso.TabIndex = 9
        TrkProgresso.TickStyle = TickStyle.None
        ' 
        ' LblTempoAtual
        ' 
        LblTempoAtual.AutoSize = True
        LblTempoAtual.BackColor = Color.Transparent
        LblTempoAtual.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        LblTempoAtual.ForeColor = Color.FromArgb(CByte(190), CByte(190), CByte(190))
        LblTempoAtual.Location = New Point(35, 575)
        LblTempoAtual.Name = "LblTempoAtual"
        LblTempoAtual.Size = New Size(38, 15)
        LblTempoAtual.TabIndex = 10
        LblTempoAtual.Text = "00:00"
        ' 
        ' LblTempoTotal
        ' 
        LblTempoTotal.AutoSize = True
        LblTempoTotal.BackColor = Color.Transparent
        LblTempoTotal.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        LblTempoTotal.ForeColor = Color.FromArgb(CByte(190), CByte(190), CByte(190))
        LblTempoTotal.Location = New Point(1020, 575)
        LblTempoTotal.Name = "LblTempoTotal"
        LblTempoTotal.Size = New Size(38, 15)
        LblTempoTotal.TabIndex = 11
        LblTempoTotal.Text = "00:00"
        ' 
        ' LblVolume
        ' 
        LblVolume.AutoSize = True
        LblVolume.ForeColor = Color.FromArgb(CByte(190), CByte(190), CByte(190))
        LblVolume.Location = New Point(849, 656)
        LblVolume.Name = "LblVolume"
        LblVolume.Size = New Size(59, 17)
        LblVolume.TabIndex = 12
        LblVolume.Text = "Volume:"
        ' 
        ' TrkVolume
        ' 
        TrkVolume.Cursor = Cursors.Hand
        TrkVolume.Location = New Point(910, 656)
        TrkVolume.Maximum = 100
        TrkVolume.Name = "TrkVolume"
        TrkVolume.Size = New Size(140, 45)
        TrkVolume.TabIndex = 13
        TrkVolume.TickStyle = TickStyle.None
        TrkVolume.Value = 70
        ' 
        ' TmrPlayer
        ' 
        TmrPlayer.Interval = 500
        ' 
        ' OfdAnuncioGravado
        ' 
        OfdAnuncioGravado.FileName = "OfdAnuncioGravado"
        OfdAnuncioGravado.Filter = "Arquivos de áudio|*.mp3;*.wav;*.wma;*.aac;*.m4a|Todos os arquivos|*.*"
        OfdAnuncioGravado.Multiselect = True
        OfdAnuncioGravado.Title = "Selecione o anúncio gravado" & vbLf
        ' 
        ' FrmPrincipal
        ' 
        AutoScaleDimensions = New SizeF(8F, 17F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(28), CByte(28), CByte(28))
        ClientSize = New Size(1084, 710)
        Controls.Add(TrkVolume)
        Controls.Add(LblVolume)
        Controls.Add(LblTempoTotal)
        Controls.Add(LblTempoAtual)
        Controls.Add(TrkProgresso)
        Controls.Add(LblMusicaAtual)
        Controls.Add(BtnParar)
        Controls.Add(BtnProxima)
        Controls.Add(BtnPausar)
        Controls.Add(BtnPlay)
        Controls.Add(BtnAnterior)
        Controls.Add(PnlPromocao)
        Controls.Add(PnlPlaylist)
        Controls.Add(PnlTopo)
        Font = New Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        FormBorderStyle = FormBorderStyle.FixedSingle
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "FrmPrincipal"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Player Promocional"
        PnlTopo.ResumeLayout(False)
        PnlTopo.PerformLayout()
        PnlPlaylist.ResumeLayout(False)
        PnlPlaylist.PerformLayout()
        PnlPromocao.ResumeLayout(False)
        PnlPromocao.PerformLayout()
        CType(NudIntervalo, ComponentModel.ISupportInitialize).EndInit()
        CType(TrkProgresso, ComponentModel.ISupportInitialize).EndInit()
        CType(TrkVolume, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents PnlTopo As Panel
    Friend WithEvents LblTitulo As Label
    Friend WithEvents PnlPlaylist As Panel
    Friend WithEvents LstMusicas As ListBox
    Friend WithEvents LblPlaylist As Label
    Friend WithEvents BtnAdicionarMusicas As Button
    Friend WithEvents PnlPromocao As Panel
    Friend WithEvents LblIntervalo As Label
    Friend WithEvents LblPromocao As Label
    Friend WithEvents LblMusicas As Label
    Friend WithEvents NudIntervalo As NumericUpDown
    Friend WithEvents BtnTestarVoz As Button
    Friend WithEvents BtnAnterior As Button
    Friend WithEvents BtnPlay As Button
    Friend WithEvents BtnPausar As Button
    Friend WithEvents BtnProxima As Button
    Friend WithEvents BtnParar As Button
    Friend WithEvents LblMusicaAtual As Label
    Friend WithEvents BtnLimparPlaylist As Button
    Friend WithEvents BtnRemoverMusica As Button
    Friend WithEvents TxtPromocao As TextBox
    Friend WithEvents OfdMusicas As OpenFileDialog
    Friend WithEvents FbdPlaylist As FolderBrowserDialog
    Friend WithEvents BtnAdicionarPasta As Button
    Friend WithEvents TrkProgresso As TrackBar
    Friend WithEvents LblTempoAtual As Label
    Friend WithEvents LblTempoTotal As Label
    Friend WithEvents LblVolume As Label
    Friend WithEvents TrkVolume As TrackBar
    Friend WithEvents TmrPlayer As Timer
    Friend WithEvents ChkAtivarPromocao As CheckBox
    Friend WithEvents ChkOrdemAleatoria As CheckBox
    Friend WithEvents RdbAnuncioGravado As RadioButton
    Friend WithEvents RdbAnuncioTexto As RadioButton
    Friend WithEvents OfdAnuncioGravado As OpenFileDialog
    Friend WithEvents BtnGerenciarAnuncios As Button

End Class
