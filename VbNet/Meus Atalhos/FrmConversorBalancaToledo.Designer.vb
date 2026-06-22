<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmConversorBalancaToledo
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmConversorBalancaToledo))
        GrpNutricional = New GroupBox()
        TabControl1 = New TabControl()
        TabDados = New TabPage()
        LblCodSD = New Label()
        LblCodSdText = New Label()
        CmbMedida = New ComboBox()
        LblMedida = New Label()
        TxtValidade = New TextBox()
        LblValidade = New Label()
        CmbCodItem = New ComboBox()
        LblCodigo = New Label()
        TxtDescricao = New TextBox()
        LblDescricao = New Label()
        TbNutricionais = New TabPage()
        LblCodNutri = New Label()
        LblCodnutridesc = New Label()
        Panel2 = New Panel()
        CmbPorcao = New ComboBox()
        Label1 = New Label()
        CmbQtdePorcao = New ComboBox()
        TxtPorcaointeira = New TextBox()
        LblCompl2 = New Label()
        LblCompl1 = New Label()
        CmbTipoPorcao = New ComboBox()
        Txtporcoes = New TextBox()
        LblPorcao = New Label()
        GroupBox1 = New GroupBox()
        ChkSodio = New CheckBox()
        ChkGordurasSaturadas = New CheckBox()
        ChkAcucarAdc = New CheckBox()
        TxtSodio = New TextBox()
        TxtFibraAlimentar = New TextBox()
        TxtGordurasTrans = New TextBox()
        TxtPoteina = New TextBox()
        TxtGalactose = New TextBox()
        TxtLactose = New TextBox()
        TxtAcucarAdc = New TextBox()
        TxtGordurasSaturadas = New TextBox()
        TxtGorduraTotais = New TextBox()
        TxtAcucarTotais = New TextBox()
        TxtCarb = New TextBox()
        TxtVlrEnergetico = New TextBox()
        LblProteina = New Label()
        LblGordurasTrans = New Label()
        LblFibraAlimentar = New Label()
        LblSodio = New Label()
        LblGordurasSaturadas = New Label()
        LblAcucarAdc = New Label()
        LblLactose = New Label()
        LblGalactose = New Label()
        LblGorduraTotais = New Label()
        LblAcucarTotal = New Label()
        LblCarb = New Label()
        LblVlrEnergetico = New Label()
        TbAdicionais = New TabPage()
        Panel1 = New Panel()
        BtnExportar = New Button()
        BtnImportarArquivos = New Button()
        BtnRetornar = New Button()
        GrpNutricional.SuspendLayout()
        TabControl1.SuspendLayout()
        TabDados.SuspendLayout()
        TbNutricionais.SuspendLayout()
        Panel2.SuspendLayout()
        GroupBox1.SuspendLayout()
        Panel1.SuspendLayout()
        SuspendLayout()
        ' 
        ' GrpNutricional
        ' 
        GrpNutricional.Controls.Add(TabControl1)
        GrpNutricional.Location = New Point(12, 12)
        GrpNutricional.Name = "GrpNutricional"
        GrpNutricional.Size = New Size(527, 460)
        GrpNutricional.TabIndex = 0
        GrpNutricional.TabStop = False
        ' 
        ' TabControl1
        ' 
        TabControl1.Controls.Add(TabDados)
        TabControl1.Controls.Add(TbNutricionais)
        TabControl1.Controls.Add(TbAdicionais)
        TabControl1.Location = New Point(15, 22)
        TabControl1.Name = "TabControl1"
        TabControl1.SelectedIndex = 0
        TabControl1.Size = New Size(484, 408)
        TabControl1.TabIndex = 2
        ' 
        ' TabDados
        ' 
        TabDados.Controls.Add(LblCodSD)
        TabDados.Controls.Add(LblCodSdText)
        TabDados.Controls.Add(CmbMedida)
        TabDados.Controls.Add(LblMedida)
        TabDados.Controls.Add(TxtValidade)
        TabDados.Controls.Add(LblValidade)
        TabDados.Controls.Add(CmbCodItem)
        TabDados.Controls.Add(LblCodigo)
        TabDados.Controls.Add(TxtDescricao)
        TabDados.Controls.Add(LblDescricao)
        TabDados.Location = New Point(4, 24)
        TabDados.Name = "TabDados"
        TabDados.Size = New Size(476, 380)
        TabDados.TabIndex = 2
        TabDados.Text = "Dados do produto"
        TabDados.UseVisualStyleBackColor = True
        ' 
        ' LblCodSD
        ' 
        LblCodSD.AutoSize = True
        LblCodSD.Location = New Point(106, 137)
        LblCodSD.Name = "LblCodSD"
        LblCodSD.Size = New Size(13, 15)
        LblCodSD.TabIndex = 34
        LblCodSD.Text = "0"
        ' 
        ' LblCodSdText
        ' 
        LblCodSdText.AutoSize = True
        LblCodSdText.Location = New Point(24, 137)
        LblCodSdText.Name = "LblCodSdText"
        LblCodSdText.Size = New Size(63, 15)
        LblCodSdText.TabIndex = 33
        LblCodSdText.Text = "Código SD"
        ' 
        ' CmbMedida
        ' 
        CmbMedida.DropDownStyle = ComboBoxStyle.DropDownList
        CmbMedida.FormattingEnabled = True
        CmbMedida.Items.AddRange(New Object() {"-", "UND", "KG"})
        CmbMedida.Location = New Point(106, 104)
        CmbMedida.Name = "CmbMedida"
        CmbMedida.Size = New Size(48, 23)
        CmbMedida.TabIndex = 32
        ' 
        ' LblMedida
        ' 
        LblMedida.AutoSize = True
        LblMedida.Location = New Point(13, 108)
        LblMedida.Name = "LblMedida"
        LblMedida.Size = New Size(81, 15)
        LblMedida.TabIndex = 31
        LblMedida.Text = "Tipo de Venda"
        ' 
        ' TxtValidade
        ' 
        TxtValidade.Location = New Point(106, 75)
        TxtValidade.Name = "TxtValidade"
        TxtValidade.Size = New Size(47, 23)
        TxtValidade.TabIndex = 30
        ' 
        ' LblValidade
        ' 
        LblValidade.AutoSize = True
        LblValidade.Location = New Point(11, 79)
        LblValidade.Name = "LblValidade"
        LblValidade.Size = New Size(84, 15)
        LblValidade.TabIndex = 29
        LblValidade.Text = "Validade (Dias)"
        ' 
        ' CmbCodItem
        ' 
        CmbCodItem.DropDownStyle = ComboBoxStyle.DropDownList
        CmbCodItem.FormattingEnabled = True
        CmbCodItem.Location = New Point(106, 17)
        CmbCodItem.Name = "CmbCodItem"
        CmbCodItem.Size = New Size(89, 23)
        CmbCodItem.TabIndex = 3
        ' 
        ' LblCodigo
        ' 
        LblCodigo.AutoSize = True
        LblCodigo.ForeColor = Color.Black
        LblCodigo.Location = New Point(30, 21)
        LblCodigo.Name = "LblCodigo"
        LblCodigo.Size = New Size(46, 15)
        LblCodigo.TabIndex = 0
        LblCodigo.Text = "Código"
        ' 
        ' TxtDescricao
        ' 
        TxtDescricao.Location = New Point(106, 46)
        TxtDescricao.Name = "TxtDescricao"
        TxtDescricao.Size = New Size(233, 23)
        TxtDescricao.TabIndex = 6
        ' 
        ' LblDescricao
        ' 
        LblDescricao.AutoSize = True
        LblDescricao.ForeColor = Color.Black
        LblDescricao.Location = New Point(24, 50)
        LblDescricao.Name = "LblDescricao"
        LblDescricao.Size = New Size(58, 15)
        LblDescricao.TabIndex = 5
        LblDescricao.Text = "Descrição"
        ' 
        ' TbNutricionais
        ' 
        TbNutricionais.Controls.Add(LblCodNutri)
        TbNutricionais.Controls.Add(LblCodnutridesc)
        TbNutricionais.Controls.Add(Panel2)
        TbNutricionais.Controls.Add(GroupBox1)
        TbNutricionais.Controls.Add(TxtSodio)
        TbNutricionais.Controls.Add(TxtFibraAlimentar)
        TbNutricionais.Controls.Add(TxtGordurasTrans)
        TbNutricionais.Controls.Add(TxtPoteina)
        TbNutricionais.Controls.Add(TxtGalactose)
        TbNutricionais.Controls.Add(TxtLactose)
        TbNutricionais.Controls.Add(TxtAcucarAdc)
        TbNutricionais.Controls.Add(TxtGordurasSaturadas)
        TbNutricionais.Controls.Add(TxtGorduraTotais)
        TbNutricionais.Controls.Add(TxtAcucarTotais)
        TbNutricionais.Controls.Add(TxtCarb)
        TbNutricionais.Controls.Add(TxtVlrEnergetico)
        TbNutricionais.Controls.Add(LblProteina)
        TbNutricionais.Controls.Add(LblGordurasTrans)
        TbNutricionais.Controls.Add(LblFibraAlimentar)
        TbNutricionais.Controls.Add(LblSodio)
        TbNutricionais.Controls.Add(LblGordurasSaturadas)
        TbNutricionais.Controls.Add(LblAcucarAdc)
        TbNutricionais.Controls.Add(LblLactose)
        TbNutricionais.Controls.Add(LblGalactose)
        TbNutricionais.Controls.Add(LblGorduraTotais)
        TbNutricionais.Controls.Add(LblAcucarTotal)
        TbNutricionais.Controls.Add(LblCarb)
        TbNutricionais.Controls.Add(LblVlrEnergetico)
        TbNutricionais.Location = New Point(4, 24)
        TbNutricionais.Name = "TbNutricionais"
        TbNutricionais.Padding = New Padding(3)
        TbNutricionais.Size = New Size(476, 380)
        TbNutricionais.TabIndex = 0
        TbNutricionais.Text = "Nutricionais"
        TbNutricionais.UseVisualStyleBackColor = True
        ' 
        ' LblCodNutri
        ' 
        LblCodNutri.AutoSize = True
        LblCodNutri.ForeColor = Color.Black
        LblCodNutri.Location = New Point(152, 13)
        LblCodNutri.Name = "LblCodNutri"
        LblCodNutri.Size = New Size(13, 15)
        LblCodNutri.TabIndex = 31
        LblCodNutri.Text = "0"
        ' 
        ' LblCodnutridesc
        ' 
        LblCodnutridesc.AutoSize = True
        LblCodnutridesc.ForeColor = Color.Black
        LblCodnutridesc.Location = New Point(12, 13)
        LblCodnutridesc.Name = "LblCodnutridesc"
        LblCodnutridesc.Size = New Size(134, 15)
        LblCodnutridesc.TabIndex = 30
        LblCodnutridesc.Text = "CÓDIGO NUTRICIONAL:" & vbCrLf
        ' 
        ' Panel2
        ' 
        Panel2.BackColor = Color.DarkGray
        Panel2.Controls.Add(CmbPorcao)
        Panel2.Controls.Add(Label1)
        Panel2.Controls.Add(CmbQtdePorcao)
        Panel2.Controls.Add(TxtPorcaointeira)
        Panel2.Controls.Add(LblCompl2)
        Panel2.Controls.Add(LblCompl1)
        Panel2.Controls.Add(CmbTipoPorcao)
        Panel2.Controls.Add(Txtporcoes)
        Panel2.Controls.Add(LblPorcao)
        Panel2.Location = New Point(8, 31)
        Panel2.Name = "Panel2"
        Panel2.Size = New Size(461, 78)
        Panel2.TabIndex = 29
        ' 
        ' CmbPorcao
        ' 
        CmbPorcao.DropDownStyle = ComboBoxStyle.DropDownList
        CmbPorcao.FormattingEnabled = True
        CmbPorcao.Items.AddRange(New Object() {"Colher(es) de Sopa", "Colher(es) de Café", "Colher(es) de Chá", "Xícara(s)", "De Xícara(s)", "Unidade(s)", "Pacote(s)", "Fatia(s)", "Fatia(s) Fina(s)", "Pedaço(s)", "Folha(s)", "Pão(es)", "Biscoito(s)", "Bisnaguinha(s)", "Disco(s)", "Copo(s)", "Porção(ões)", "Tablete(s)", "Sachê(s)", "Almôndega(s)", "Bife(s)", "Filé(s)", "Concha(s)", "Bala(s)", "Prato(s) Fundo(s)", "Pitada(s)", "Lata(s)", "Xícara(s) de Chá", "Prato(s) Raso"})
        CmbPorcao.Location = New Point(314, 42)
        CmbPorcao.Name = "CmbPorcao"
        CmbPorcao.Size = New Size(123, 23)
        CmbPorcao.TabIndex = 19
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(109, 9)
        Label1.Name = "Label1"
        Label1.Size = New Size(252, 15)
        Label1.TabIndex = 20
        Label1.Text = "QUANTIDADE DE PORÇÕES POR EMBALAGEM"
        ' 
        ' CmbQtdePorcao
        ' 
        CmbQtdePorcao.DropDownStyle = ComboBoxStyle.DropDownList
        CmbQtdePorcao.FormattingEnabled = True
        CmbQtdePorcao.Items.AddRange(New Object() {"-", "1/4", "1/3", "1/2", "2/3", "3/4"})
        CmbQtdePorcao.Location = New Point(260, 42)
        CmbQtdePorcao.Name = "CmbQtdePorcao"
        CmbQtdePorcao.Size = New Size(48, 23)
        CmbQtdePorcao.TabIndex = 18
        ' 
        ' TxtPorcaointeira
        ' 
        TxtPorcaointeira.Location = New Point(207, 42)
        TxtPorcaointeira.Name = "TxtPorcaointeira"
        TxtPorcaointeira.Size = New Size(47, 23)
        TxtPorcaointeira.TabIndex = 17
        ' 
        ' LblCompl2
        ' 
        LblCompl2.AutoSize = True
        LblCompl2.Location = New Point(443, 46)
        LblCompl2.Name = "LblCompl2"
        LblCompl2.Size = New Size(11, 15)
        LblCompl2.TabIndex = 16
        LblCompl2.Text = ")"
        ' 
        ' LblCompl1
        ' 
        LblCompl1.AutoSize = True
        LblCompl1.Location = New Point(190, 46)
        LblCompl1.Name = "LblCompl1"
        LblCompl1.Size = New Size(11, 15)
        LblCompl1.TabIndex = 15
        LblCompl1.Text = "("
        ' 
        ' CmbTipoPorcao
        ' 
        CmbTipoPorcao.DropDownStyle = ComboBoxStyle.DropDownList
        CmbTipoPorcao.FormattingEnabled = True
        CmbTipoPorcao.Items.AddRange(New Object() {"g", "ml"})
        CmbTipoPorcao.Location = New Point(109, 42)
        CmbTipoPorcao.Name = "CmbTipoPorcao"
        CmbTipoPorcao.Size = New Size(70, 23)
        CmbTipoPorcao.TabIndex = 14
        ' 
        ' Txtporcoes
        ' 
        Txtporcoes.Location = New Point(58, 42)
        Txtporcoes.Name = "Txtporcoes"
        Txtporcoes.Size = New Size(47, 23)
        Txtporcoes.TabIndex = 13
        ' 
        ' LblPorcao
        ' 
        LblPorcao.AutoSize = True
        LblPorcao.Location = New Point(10, 46)
        LblPorcao.Name = "LblPorcao"
        LblPorcao.Size = New Size(44, 15)
        LblPorcao.TabIndex = 1
        LblPorcao.Text = "Porção"
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(ChkSodio)
        GroupBox1.Controls.Add(ChkGordurasSaturadas)
        GroupBox1.Controls.Add(ChkAcucarAdc)
        GroupBox1.Location = New Point(300, 260)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Size = New Size(164, 99)
        GroupBox1.TabIndex = 24
        GroupBox1.TabStop = False
        GroupBox1.Text = "Alto em"
        ' 
        ' ChkSodio
        ' 
        ChkSodio.AutoSize = True
        ChkSodio.Location = New Point(6, 74)
        ChkSodio.Name = "ChkSodio"
        ChkSodio.Size = New Size(56, 19)
        ChkSodio.TabIndex = 2
        ChkSodio.Text = "Sódio"
        ChkSodio.UseVisualStyleBackColor = True
        ' 
        ' ChkGordurasSaturadas
        ' 
        ChkGordurasSaturadas.AutoSize = True
        ChkGordurasSaturadas.Location = New Point(6, 49)
        ChkGordurasSaturadas.Name = "ChkGordurasSaturadas"
        ChkGordurasSaturadas.Size = New Size(123, 19)
        ChkGordurasSaturadas.TabIndex = 1
        ChkGordurasSaturadas.Text = "Gordura Saturadas"
        ChkGordurasSaturadas.UseVisualStyleBackColor = True
        ' 
        ' ChkAcucarAdc
        ' 
        ChkAcucarAdc.AutoSize = True
        ChkAcucarAdc.Location = New Point(6, 24)
        ChkAcucarAdc.Name = "ChkAcucarAdc"
        ChkAcucarAdc.Size = New Size(125, 19)
        ChkAcucarAdc.TabIndex = 0
        ChkAcucarAdc.Text = "Açúcar adicionado"
        ChkAcucarAdc.UseVisualStyleBackColor = True
        ' 
        ' TxtSodio
        ' 
        TxtSodio.Location = New Point(418, 172)
        TxtSodio.Name = "TxtSodio"
        TxtSodio.Size = New Size(47, 23)
        TxtSodio.TabIndex = 23
        ' 
        ' TxtFibraAlimentar
        ' 
        TxtFibraAlimentar.Location = New Point(418, 144)
        TxtFibraAlimentar.Name = "TxtFibraAlimentar"
        TxtFibraAlimentar.Size = New Size(47, 23)
        TxtFibraAlimentar.TabIndex = 22
        ' 
        ' TxtGordurasTrans
        ' 
        TxtGordurasTrans.Location = New Point(418, 116)
        TxtGordurasTrans.Name = "TxtGordurasTrans"
        TxtGordurasTrans.Size = New Size(47, 23)
        TxtGordurasTrans.TabIndex = 21
        ' 
        ' TxtPoteina
        ' 
        TxtPoteina.Location = New Point(179, 284)
        TxtPoteina.Name = "TxtPoteina"
        TxtPoteina.Size = New Size(47, 23)
        TxtPoteina.TabIndex = 20
        ' 
        ' TxtGalactose
        ' 
        TxtGalactose.Location = New Point(178, 256)
        TxtGalactose.Name = "TxtGalactose"
        TxtGalactose.Size = New Size(48, 23)
        TxtGalactose.TabIndex = 19
        ' 
        ' TxtLactose
        ' 
        TxtLactose.Location = New Point(179, 228)
        TxtLactose.Name = "TxtLactose"
        TxtLactose.Size = New Size(47, 23)
        TxtLactose.TabIndex = 18
        ' 
        ' TxtAcucarAdc
        ' 
        TxtAcucarAdc.Location = New Point(179, 200)
        TxtAcucarAdc.Name = "TxtAcucarAdc"
        TxtAcucarAdc.Size = New Size(47, 23)
        TxtAcucarAdc.TabIndex = 17
        ' 
        ' TxtGordurasSaturadas
        ' 
        TxtGordurasSaturadas.Location = New Point(179, 340)
        TxtGordurasSaturadas.Name = "TxtGordurasSaturadas"
        TxtGordurasSaturadas.Size = New Size(47, 23)
        TxtGordurasSaturadas.TabIndex = 16
        ' 
        ' TxtGorduraTotais
        ' 
        TxtGorduraTotais.Location = New Point(179, 312)
        TxtGorduraTotais.Name = "TxtGorduraTotais"
        TxtGorduraTotais.Size = New Size(47, 23)
        TxtGorduraTotais.TabIndex = 15
        ' 
        ' TxtAcucarTotais
        ' 
        TxtAcucarTotais.Location = New Point(179, 172)
        TxtAcucarTotais.Name = "TxtAcucarTotais"
        TxtAcucarTotais.Size = New Size(47, 23)
        TxtAcucarTotais.TabIndex = 14
        ' 
        ' TxtCarb
        ' 
        TxtCarb.Location = New Point(179, 144)
        TxtCarb.Name = "TxtCarb"
        TxtCarb.Size = New Size(47, 23)
        TxtCarb.TabIndex = 13
        ' 
        ' TxtVlrEnergetico
        ' 
        TxtVlrEnergetico.Location = New Point(179, 116)
        TxtVlrEnergetico.Name = "TxtVlrEnergetico"
        TxtVlrEnergetico.Size = New Size(47, 23)
        TxtVlrEnergetico.TabIndex = 12
        ' 
        ' LblProteina
        ' 
        LblProteina.AutoSize = True
        LblProteina.Location = New Point(17, 288)
        LblProteina.Name = "LblProteina"
        LblProteina.Size = New Size(75, 15)
        LblProteina.TabIndex = 11
        LblProteina.Text = "Proteina ( g )"
        ' 
        ' LblGordurasTrans
        ' 
        LblGordurasTrans.AutoSize = True
        LblGordurasTrans.Location = New Point(300, 120)
        LblGordurasTrans.Name = "LblGordurasTrans"
        LblGordurasTrans.Size = New Size(109, 15)
        LblGordurasTrans.TabIndex = 10
        LblGordurasTrans.Text = "Gorduras Trans ( g )"
        ' 
        ' LblFibraAlimentar
        ' 
        LblFibraAlimentar.AutoSize = True
        LblFibraAlimentar.Location = New Point(300, 148)
        LblFibraAlimentar.Name = "LblFibraAlimentar"
        LblFibraAlimentar.Size = New Size(112, 15)
        LblFibraAlimentar.TabIndex = 9
        LblFibraAlimentar.Text = "Fibra Alimentar ( g )"
        ' 
        ' LblSodio
        ' 
        LblSodio.AutoSize = True
        LblSodio.Location = New Point(300, 176)
        LblSodio.Name = "LblSodio"
        LblSodio.Size = New Size(61, 15)
        LblSodio.TabIndex = 8
        LblSodio.Text = "Sódio ( g )"
        ' 
        ' LblGordurasSaturadas
        ' 
        LblGordurasSaturadas.AutoSize = True
        LblGordurasSaturadas.Location = New Point(17, 344)
        LblGordurasSaturadas.Name = "LblGordurasSaturadas"
        LblGordurasSaturadas.Size = New Size(136, 15)
        LblGordurasSaturadas.TabIndex = 7
        LblGordurasSaturadas.Text = "Gorduras Saturadas ( g ) "
        ' 
        ' LblAcucarAdc
        ' 
        LblAcucarAdc.AutoSize = True
        LblAcucarAdc.Location = New Point(17, 204)
        LblAcucarAdc.Name = "LblAcucarAdc"
        LblAcucarAdc.Size = New Size(146, 15)
        LblAcucarAdc.TabIndex = 6
        LblAcucarAdc.Text = "Açucares adicionados ( g )" & vbCrLf
        ' 
        ' LblLactose
        ' 
        LblLactose.AutoSize = True
        LblLactose.Location = New Point(17, 232)
        LblLactose.Name = "LblLactose"
        LblLactose.Size = New Size(74, 15)
        LblLactose.TabIndex = 5
        LblLactose.Text = "Lactose ( g ) "
        ' 
        ' LblGalactose
        ' 
        LblGalactose.AutoSize = True
        LblGalactose.Location = New Point(17, 260)
        LblGalactose.Name = "LblGalactose"
        LblGalactose.Size = New Size(82, 15)
        LblGalactose.TabIndex = 4
        LblGalactose.Text = "Galactose ( g )"
        ' 
        ' LblGorduraTotais
        ' 
        LblGorduraTotais.AutoSize = True
        LblGorduraTotais.Location = New Point(17, 316)
        LblGorduraTotais.Name = "LblGorduraTotais"
        LblGorduraTotais.Size = New Size(112, 15)
        LblGorduraTotais.TabIndex = 3
        LblGorduraTotais.Text = "Gorduras Totais ( g )"
        ' 
        ' LblAcucarTotal
        ' 
        LblAcucarTotal.AutoSize = True
        LblAcucarTotal.Location = New Point(17, 176)
        LblAcucarTotal.Name = "LblAcucarTotal"
        LblAcucarTotal.Size = New Size(111, 15)
        LblAcucarTotal.TabIndex = 2
        LblAcucarTotal.Text = "Açúcares totais ( g )"
        ' 
        ' LblCarb
        ' 
        LblCarb.AutoSize = True
        LblCarb.Location = New Point(17, 148)
        LblCarb.Name = "LblCarb"
        LblCarb.Size = New Size(102, 15)
        LblCarb.TabIndex = 1
        LblCarb.Text = "Carboidratos ( g ) "
        ' 
        ' LblVlrEnergetico
        ' 
        LblVlrEnergetico.AutoSize = True
        LblVlrEnergetico.Location = New Point(17, 120)
        LblVlrEnergetico.Name = "LblVlrEnergetico"
        LblVlrEnergetico.Size = New Size(157, 15)
        LblVlrEnergetico.TabIndex = 0
        LblVlrEnergetico.Text = "Valor Energetico ( Kcal = KJ) "
        ' 
        ' TbAdicionais
        ' 
        TbAdicionais.Location = New Point(4, 24)
        TbAdicionais.Name = "TbAdicionais"
        TbAdicionais.Padding = New Padding(3)
        TbAdicionais.Size = New Size(476, 380)
        TbAdicionais.TabIndex = 1
        TbAdicionais.Text = "Adicionais"
        TbAdicionais.UseVisualStyleBackColor = True
        ' 
        ' Panel1
        ' 
        Panel1.BackColor = SystemColors.ControlDarkDark
        Panel1.Controls.Add(BtnExportar)
        Panel1.Controls.Add(BtnImportarArquivos)
        Panel1.Controls.Add(BtnRetornar)
        Panel1.Location = New Point(-2, 478)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(568, 68)
        Panel1.TabIndex = 5
        ' 
        ' BtnExportar
        ' 
        BtnExportar.Location = New Point(241, 10)
        BtnExportar.Name = "BtnExportar"
        BtnExportar.Size = New Size(87, 49)
        BtnExportar.TabIndex = 3
        BtnExportar.Text = "Exportar SD"
        BtnExportar.UseVisualStyleBackColor = True
        ' 
        ' BtnImportarArquivos
        ' 
        BtnImportarArquivos.Location = New Point(148, 10)
        BtnImportarArquivos.Name = "BtnImportarArquivos"
        BtnImportarArquivos.Size = New Size(87, 49)
        BtnImportarArquivos.TabIndex = 0
        BtnImportarArquivos.Text = "Importar arquivos"
        BtnImportarArquivos.UseVisualStyleBackColor = True
        ' 
        ' BtnRetornar
        ' 
        BtnRetornar.Location = New Point(334, 10)
        BtnRetornar.Name = "BtnRetornar"
        BtnRetornar.Size = New Size(87, 49)
        BtnRetornar.TabIndex = 1
        BtnRetornar.Text = "Retornar"
        BtnRetornar.UseVisualStyleBackColor = True
        ' 
        ' FrmConversorBalancaToledo
        ' 
        AutoScaleMode = AutoScaleMode.None
        AutoSizeMode = AutoSizeMode.GrowAndShrink
        BackColor = Color.FromArgb(CByte(134), CByte(29), CByte(29))
        ClientSize = New Size(562, 544)
        Controls.Add(Panel1)
        Controls.Add(GrpNutricional)
        ForeColor = Color.Black
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MdiChildrenMinimizedAnchorBottom = False
        MinimizeBox = False
        Name = "FrmConversorBalancaToledo"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterScreen
        Text = "Conversor Toledo"
        GrpNutricional.ResumeLayout(False)
        TabControl1.ResumeLayout(False)
        TabDados.ResumeLayout(False)
        TabDados.PerformLayout()
        TbNutricionais.ResumeLayout(False)
        TbNutricionais.PerformLayout()
        Panel2.ResumeLayout(False)
        Panel2.PerformLayout()
        GroupBox1.ResumeLayout(False)
        GroupBox1.PerformLayout()
        Panel1.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents GrpNutricional As GroupBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents BtnExportar As Button
    Friend WithEvents BtnImportarArquivos As Button
    Friend WithEvents BtnRetornar As Button
    Friend WithEvents LblCodigo As Label
    Friend WithEvents CmbCodItem As ComboBox
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TbNutricionais As TabPage
    Friend WithEvents TbAdicionais As TabPage
    Friend WithEvents LblGorduraTotais As Label
    Friend WithEvents LblAcucarTotal As Label
    Friend WithEvents LblCarb As Label
    Friend WithEvents LblVlrEnergetico As Label
    Friend WithEvents LblProteina As Label
    Friend WithEvents LblGordurasTrans As Label
    Friend WithEvents LblFibraAlimentar As Label
    Friend WithEvents LblSodio As Label
    Friend WithEvents LblGordurasSaturadas As Label
    Friend WithEvents LblAcucarAdc As Label
    Friend WithEvents LblLactose As Label
    Friend WithEvents LblGalactose As Label
    Friend WithEvents TxtVlrEnergetico As TextBox
    Friend WithEvents TxtLactose As TextBox
    Friend WithEvents TxtAcucarAdc As TextBox
    Friend WithEvents TxtGordurasSaturadas As TextBox
    Friend WithEvents TxtGorduraTotais As TextBox
    Friend WithEvents TxtAcucarTotais As TextBox
    Friend WithEvents TxtCarb As TextBox
    Friend WithEvents TxtSodio As TextBox
    Friend WithEvents TxtFibraAlimentar As TextBox
    Friend WithEvents TxtGordurasTrans As TextBox
    Friend WithEvents TxtPoteina As TextBox
    Friend WithEvents TxtGalactose As TextBox
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents ChkSodio As CheckBox
    Friend WithEvents ChkGordurasSaturadas As CheckBox
    Friend WithEvents ChkAcucarAdc As CheckBox
    Friend WithEvents LblDescricao As Label
    Friend WithEvents TxtDescricao As TextBox
    Friend WithEvents TabDados As TabPage
    Friend WithEvents LblCodNutri As Label
    Friend WithEvents LblCodnutridesc As Label
    Friend WithEvents Panel2 As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents CmbPorcao As ComboBox
    Friend WithEvents CmbQtdePorcao As ComboBox
    Friend WithEvents TxtPorcaointeira As TextBox
    Friend WithEvents LblCompl2 As Label
    Friend WithEvents LblCompl1 As Label
    Friend WithEvents CmbTipoPorcao As ComboBox
    Friend WithEvents Txtporcoes As TextBox
    Friend WithEvents LblPorcao As Label
    Friend WithEvents CmbMedida As ComboBox
    Friend WithEvents LblMedida As Label
    Friend WithEvents TxtValidade As TextBox
    Friend WithEvents LblValidade As Label
    Friend WithEvents LblCodSD As Label
    Friend WithEvents LblCodSdText As Label
End Class
