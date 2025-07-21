<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmOrdemDeServico
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmOrdemDeServico))
        GrpOs = New GroupBox()
        TbOrdemServico = New TabControl()
        TabPage1 = New TabPage()
        LblOrdem = New Label()
        TxtIdOrdem = New TextBox()
        PictureBox1 = New PictureBox()
        TxtDataEmissao = New TextBox()
        Txtobservacoes = New TextBox()
        LblObservacoes = New Label()
        Txtservicos = New TextBox()
        LblServicos = New Label()
        TxtTecnico = New TextBox()
        Label3 = New Label()
        Label2 = New Label()
        TbCadastro = New TabPage()
        LblCpfCnpj = New Label()
        TxtTellCell = New TextBox()
        Label4 = New Label()
        TxtComplemento = New TextBox()
        Label1 = New Label()
        CmbRazaoSocial = New ComboBox()
        TxtIdCliente = New TextBox()
        LblId = New Label()
        LblEmail = New Label()
        TxtEmail = New TextBox()
        LblComplemento = New Label()
        LblNumero = New Label()
        TxtNumero = New TextBox()
        LblLogradouro = New Label()
        TxtLogradouro = New TextBox()
        TxtCidade = New TextBox()
        LblCidade = New Label()
        TxtEstado = New TextBox()
        LblEstado = New Label()
        TxtBairro = New TextBox()
        LblBairro = New Label()
        TxtCnpj = New TextBox()
        TxtCep = New TextBox()
        LblCep = New Label()
        TxtCliente = New TextBox()
        LblNome = New Label()
        BtnGerarPdf = New Button()
        BtnLimparDados = New Button()
        GrpOs.SuspendLayout()
        TbOrdemServico.SuspendLayout()
        TabPage1.SuspendLayout()
        CType(PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        TbCadastro.SuspendLayout()
        SuspendLayout()
        ' 
        ' GrpOs
        ' 
        GrpOs.Controls.Add(TbOrdemServico)
        GrpOs.Location = New Point(26, 25)
        GrpOs.Name = "GrpOs"
        GrpOs.Size = New Size(745, 353)
        GrpOs.TabIndex = 1
        GrpOs.TabStop = False
        ' 
        ' TbOrdemServico
        ' 
        TbOrdemServico.Controls.Add(TabPage1)
        TbOrdemServico.Controls.Add(TbCadastro)
        TbOrdemServico.Location = New Point(6, 22)
        TbOrdemServico.Name = "TbOrdemServico"
        TbOrdemServico.SelectedIndex = 0
        TbOrdemServico.Size = New Size(733, 325)
        TbOrdemServico.TabIndex = 0
        ' 
        ' TabPage1
        ' 
        TabPage1.Controls.Add(LblOrdem)
        TabPage1.Controls.Add(TxtIdOrdem)
        TabPage1.Controls.Add(PictureBox1)
        TabPage1.Controls.Add(TxtDataEmissao)
        TabPage1.Controls.Add(Txtobservacoes)
        TabPage1.Controls.Add(LblObservacoes)
        TabPage1.Controls.Add(Txtservicos)
        TabPage1.Controls.Add(LblServicos)
        TabPage1.Controls.Add(TxtTecnico)
        TabPage1.Controls.Add(Label3)
        TabPage1.Controls.Add(Label2)
        TabPage1.Location = New Point(4, 24)
        TabPage1.Name = "TabPage1"
        TabPage1.Padding = New Padding(3)
        TabPage1.Size = New Size(725, 297)
        TabPage1.TabIndex = 0
        TabPage1.Text = "Ordem de serviço"
        TabPage1.UseVisualStyleBackColor = True
        ' 
        ' LblOrdem
        ' 
        LblOrdem.AutoSize = True
        LblOrdem.Location = New Point(6, 6)
        LblOrdem.Name = "LblOrdem"
        LblOrdem.Size = New Size(44, 15)
        LblOrdem.TabIndex = 102
        LblOrdem.Text = "Ordem"
        ' 
        ' TxtIdOrdem
        ' 
        TxtIdOrdem.Enabled = False
        TxtIdOrdem.Location = New Point(6, 24)
        TxtIdOrdem.Name = "TxtIdOrdem"
        TxtIdOrdem.Size = New Size(80, 23)
        TxtIdOrdem.TabIndex = 101
        ' 
        ' PictureBox1
        ' 
        PictureBox1.BackgroundImage = CType(resources.GetObject("PictureBox1.BackgroundImage"), Image)
        PictureBox1.BackgroundImageLayout = ImageLayout.Zoom
        PictureBox1.Location = New Point(599, 157)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(120, 134)
        PictureBox1.TabIndex = 57
        PictureBox1.TabStop = False
        ' 
        ' TxtDataEmissao
        ' 
        TxtDataEmissao.Enabled = False
        TxtDataEmissao.Location = New Point(92, 24)
        TxtDataEmissao.Name = "TxtDataEmissao"
        TxtDataEmissao.Size = New Size(279, 23)
        TxtDataEmissao.TabIndex = 51
        ' 
        ' Txtobservacoes
        ' 
        Txtobservacoes.Location = New Point(6, 312)
        Txtobservacoes.Name = "Txtobservacoes"
        Txtobservacoes.Size = New Size(279, 23)
        Txtobservacoes.TabIndex = 55
        ' 
        ' LblObservacoes
        ' 
        LblObservacoes.AutoSize = True
        LblObservacoes.Location = New Point(6, 294)
        LblObservacoes.Name = "LblObservacoes"
        LblObservacoes.Size = New Size(74, 15)
        LblObservacoes.TabIndex = 56
        LblObservacoes.Text = "Observações"
        ' 
        ' Txtservicos
        ' 
        Txtservicos.Location = New Point(6, 68)
        Txtservicos.Name = "Txtservicos"
        Txtservicos.Size = New Size(713, 23)
        Txtservicos.TabIndex = 54
        ' 
        ' LblServicos
        ' 
        LblServicos.AutoSize = True
        LblServicos.Location = New Point(6, 50)
        LblServicos.Name = "LblServicos"
        LblServicos.Size = New Size(113, 15)
        LblServicos.TabIndex = 53
        LblServicos.Text = "Serviços executados"
        ' 
        ' TxtTecnico
        ' 
        TxtTecnico.Location = New Point(377, 24)
        TxtTecnico.Name = "TxtTecnico"
        TxtTecnico.Size = New Size(342, 23)
        TxtTecnico.TabIndex = 52
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(377, 6)
        Label3.Name = "Label3"
        Label3.Size = New Size(47, 15)
        Label3.TabIndex = 50
        Label3.Text = "Técnico"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(92, 6)
        Label2.Name = "Label2"
        Label2.Size = New Size(93, 15)
        Label2.TabIndex = 46
        Label2.Text = "Data de emissão"
        ' 
        ' TbCadastro
        ' 
        TbCadastro.Controls.Add(LblCpfCnpj)
        TbCadastro.Controls.Add(TxtTellCell)
        TbCadastro.Controls.Add(Label4)
        TbCadastro.Controls.Add(TxtComplemento)
        TbCadastro.Controls.Add(Label1)
        TbCadastro.Controls.Add(CmbRazaoSocial)
        TbCadastro.Controls.Add(TxtIdCliente)
        TbCadastro.Controls.Add(LblId)
        TbCadastro.Controls.Add(LblEmail)
        TbCadastro.Controls.Add(TxtEmail)
        TbCadastro.Controls.Add(LblComplemento)
        TbCadastro.Controls.Add(LblNumero)
        TbCadastro.Controls.Add(TxtNumero)
        TbCadastro.Controls.Add(LblLogradouro)
        TbCadastro.Controls.Add(TxtLogradouro)
        TbCadastro.Controls.Add(TxtCidade)
        TbCadastro.Controls.Add(LblCidade)
        TbCadastro.Controls.Add(TxtEstado)
        TbCadastro.Controls.Add(LblEstado)
        TbCadastro.Controls.Add(TxtBairro)
        TbCadastro.Controls.Add(LblBairro)
        TbCadastro.Controls.Add(TxtCnpj)
        TbCadastro.Controls.Add(TxtCep)
        TbCadastro.Controls.Add(LblCep)
        TbCadastro.Controls.Add(TxtCliente)
        TbCadastro.Controls.Add(LblNome)
        TbCadastro.Location = New Point(4, 24)
        TbCadastro.Name = "TbCadastro"
        TbCadastro.Padding = New Padding(3)
        TbCadastro.Size = New Size(725, 297)
        TbCadastro.TabIndex = 1
        TbCadastro.Text = "Cadastro"
        TbCadastro.UseVisualStyleBackColor = True
        ' 
        ' LblCpfCnpj
        ' 
        LblCpfCnpj.AutoSize = True
        LblCpfCnpj.Location = New Point(6, 53)
        LblCpfCnpj.Name = "LblCpfCnpj"
        LblCpfCnpj.Size = New Size(60, 15)
        LblCpfCnpj.TabIndex = 101
        LblCpfCnpj.Text = "CPF/CNPJ"
        ' 
        ' TxtTellCell
        ' 
        TxtTellCell.Location = New Point(528, 121)
        TxtTellCell.Name = "TxtTellCell"
        TxtTellCell.Size = New Size(188, 23)
        TxtTellCell.TabIndex = 100
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Location = New Point(528, 103)
        Label4.Name = "Label4"
        Label4.Size = New Size(93, 15)
        Label4.TabIndex = 99
        Label4.Text = "Telefone/Celular"
        ' 
        ' TxtComplemento
        ' 
        TxtComplemento.Location = New Point(574, 71)
        TxtComplemento.Name = "TxtComplemento"
        TxtComplemento.Size = New Size(142, 23)
        TxtComplemento.TabIndex = 83
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(6, 3)
        Label1.Name = "Label1"
        Label1.Size = New Size(71, 15)
        Label1.TabIndex = 98
        Label1.Text = "Razão social"
        ' 
        ' CmbRazaoSocial
        ' 
        CmbRazaoSocial.FormattingEnabled = True
        CmbRazaoSocial.Location = New Point(6, 21)
        CmbRazaoSocial.Name = "CmbRazaoSocial"
        CmbRazaoSocial.Size = New Size(279, 23)
        CmbRazaoSocial.TabIndex = 97
        ' 
        ' TxtIdCliente
        ' 
        TxtIdCliente.Enabled = False
        TxtIdCliente.Location = New Point(233, 71)
        TxtIdCliente.Name = "TxtIdCliente"
        TxtIdCliente.Size = New Size(52, 23)
        TxtIdCliente.TabIndex = 96
        ' 
        ' LblId
        ' 
        LblId.AutoSize = True
        LblId.Location = New Point(233, 53)
        LblId.Name = "LblId"
        LblId.RightToLeft = RightToLeft.No
        LblId.Size = New Size(18, 15)
        LblId.TabIndex = 95
        LblId.Text = "ID"
        ' 
        ' LblEmail
        ' 
        LblEmail.AutoSize = True
        LblEmail.Location = New Point(6, 103)
        LblEmail.Name = "LblEmail"
        LblEmail.Size = New Size(41, 15)
        LblEmail.TabIndex = 94
        LblEmail.Text = "E-mail"
        ' 
        ' TxtEmail
        ' 
        TxtEmail.Location = New Point(6, 121)
        TxtEmail.Name = "TxtEmail"
        TxtEmail.Size = New Size(279, 23)
        TxtEmail.TabIndex = 93
        ' 
        ' LblComplemento
        ' 
        LblComplemento.AutoSize = True
        LblComplemento.Location = New Point(574, 53)
        LblComplemento.Name = "LblComplemento"
        LblComplemento.Size = New Size(84, 15)
        LblComplemento.TabIndex = 90
        LblComplemento.Text = "Complemento"
        ' 
        ' LblNumero
        ' 
        LblNumero.AutoSize = True
        LblNumero.Location = New Point(528, 53)
        LblNumero.Name = "LblNumero"
        LblNumero.Size = New Size(21, 15)
        LblNumero.TabIndex = 89
        LblNumero.Text = "Nº"
        ' 
        ' TxtNumero
        ' 
        TxtNumero.Location = New Point(528, 71)
        TxtNumero.Name = "TxtNumero"
        TxtNumero.Size = New Size(40, 23)
        TxtNumero.TabIndex = 82
        ' 
        ' LblLogradouro
        ' 
        LblLogradouro.AutoSize = True
        LblLogradouro.Location = New Point(291, 53)
        LblLogradouro.Name = "LblLogradouro"
        LblLogradouro.Size = New Size(69, 15)
        LblLogradouro.TabIndex = 88
        LblLogradouro.Text = "Logradouro"
        ' 
        ' TxtLogradouro
        ' 
        TxtLogradouro.Location = New Point(291, 71)
        TxtLogradouro.Name = "TxtLogradouro"
        TxtLogradouro.Size = New Size(231, 23)
        TxtLogradouro.TabIndex = 81
        ' 
        ' TxtCidade
        ' 
        TxtCidade.Location = New Point(503, 21)
        TxtCidade.Name = "TxtCidade"
        TxtCidade.Size = New Size(126, 23)
        TxtCidade.TabIndex = 79
        ' 
        ' LblCidade
        ' 
        LblCidade.AutoSize = True
        LblCidade.Location = New Point(503, 3)
        LblCidade.Name = "LblCidade"
        LblCidade.Size = New Size(44, 15)
        LblCidade.TabIndex = 87
        LblCidade.Text = "Cidade"
        ' 
        ' TxtEstado
        ' 
        TxtEstado.Location = New Point(635, 21)
        TxtEstado.Name = "TxtEstado"
        TxtEstado.Size = New Size(81, 23)
        TxtEstado.TabIndex = 80
        ' 
        ' LblEstado
        ' 
        LblEstado.AutoSize = True
        LblEstado.Location = New Point(635, 3)
        LblEstado.Name = "LblEstado"
        LblEstado.Size = New Size(42, 15)
        LblEstado.TabIndex = 86
        LblEstado.Text = "Estado"
        ' 
        ' TxtBairro
        ' 
        TxtBairro.Location = New Point(388, 21)
        TxtBairro.Name = "TxtBairro"
        TxtBairro.Size = New Size(108, 23)
        TxtBairro.TabIndex = 78
        ' 
        ' LblBairro
        ' 
        LblBairro.AutoSize = True
        LblBairro.Location = New Point(388, 3)
        LblBairro.Name = "LblBairro"
        LblBairro.Size = New Size(38, 15)
        LblBairro.TabIndex = 85
        LblBairro.Text = "Bairro"
        ' 
        ' TxtCnpj
        ' 
        TxtCnpj.Location = New Point(6, 71)
        TxtCnpj.MaxLength = 14
        TxtCnpj.Name = "TxtCnpj"
        TxtCnpj.Size = New Size(221, 23)
        TxtCnpj.TabIndex = 84
        ' 
        ' TxtCep
        ' 
        TxtCep.Location = New Point(291, 21)
        TxtCep.Name = "TxtCep"
        TxtCep.Size = New Size(90, 23)
        TxtCep.TabIndex = 76
        ' 
        ' LblCep
        ' 
        LblCep.AutoSize = True
        LblCep.Location = New Point(291, 3)
        LblCep.Name = "LblCep"
        LblCep.Size = New Size(28, 15)
        LblCep.TabIndex = 77
        LblCep.Text = "Cep"
        ' 
        ' TxtCliente
        ' 
        TxtCliente.Location = New Point(291, 121)
        TxtCliente.Name = "TxtCliente"
        TxtCliente.Size = New Size(231, 23)
        TxtCliente.TabIndex = 75
        ' 
        ' LblNome
        ' 
        LblNome.AutoSize = True
        LblNome.Location = New Point(291, 103)
        LblNome.Name = "LblNome"
        LblNome.Size = New Size(90, 15)
        LblNome.TabIndex = 74
        LblNome.Text = "Contato Cliente"
        ' 
        ' BtnGerarPdf
        ' 
        BtnGerarPdf.Location = New Point(396, 400)
        BtnGerarPdf.Name = "BtnGerarPdf"
        BtnGerarPdf.Size = New Size(93, 38)
        BtnGerarPdf.TabIndex = 13
        BtnGerarPdf.Text = "Gerar OS"
        BtnGerarPdf.UseVisualStyleBackColor = True
        ' 
        ' BtnLimparDados
        ' 
        BtnLimparDados.Location = New Point(297, 400)
        BtnLimparDados.Name = "BtnLimparDados"
        BtnLimparDados.Size = New Size(93, 38)
        BtnLimparDados.TabIndex = 14
        BtnLimparDados.Text = "Limpar dados"
        BtnLimparDados.UseVisualStyleBackColor = True
        ' 
        ' FrmOrdemDeServico
        ' 
        AutoScaleDimensions = New SizeF(7.0F, 15.0F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        Controls.Add(BtnLimparDados)
        Controls.Add(BtnGerarPdf)
        Controls.Add(GrpOs)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "FrmOrdemDeServico"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Abertura de Ordem de Serviço"
        GrpOs.ResumeLayout(False)
        TbOrdemServico.ResumeLayout(False)
        TabPage1.ResumeLayout(False)
        TabPage1.PerformLayout()
        CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        TbCadastro.ResumeLayout(False)
        TbCadastro.PerformLayout()
        ResumeLayout(False)
    End Sub
    Friend WithEvents GrpOs As GroupBox
    Friend WithEvents BtnGerarPdf As Button
    Friend WithEvents BtnLimparDados As Button
    Friend WithEvents TbOrdemServico As TabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents TbCadastro As TabPage
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents TxtDataEmissao As TextBox
    Friend WithEvents Txtobservacoes As TextBox
    Friend WithEvents LblObservacoes As Label
    Friend WithEvents Txtservicos As TextBox
    Friend WithEvents LblServicos As Label
    Friend WithEvents TxtTecnico As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents LblOrdem As Label
    Friend WithEvents TxtIdOrdem As TextBox
    Friend WithEvents TxtComplemento As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents CmbRazaoSocial As ComboBox
    Friend WithEvents TxtIdCliente As TextBox
    Friend WithEvents LblId As Label
    Friend WithEvents LblEmail As Label
    Friend WithEvents TxtEmail As TextBox
    Friend WithEvents LblComplemento As Label
    Friend WithEvents LblNumero As Label
    Friend WithEvents TxtNumero As TextBox
    Friend WithEvents LblLogradouro As Label
    Friend WithEvents TxtLogradouro As TextBox
    Friend WithEvents TxtCidade As TextBox
    Friend WithEvents LblCidade As Label
    Friend WithEvents TxtEstado As TextBox
    Friend WithEvents LblEstado As Label
    Friend WithEvents TxtBairro As TextBox
    Friend WithEvents LblBairro As Label
    Friend WithEvents TxtCnpj As TextBox
    Friend WithEvents TxtCep As TextBox
    Friend WithEvents LblCep As Label
    Friend WithEvents TxtCliente As TextBox
    Friend WithEvents LblNome As Label
    Friend WithEvents TxtTellCell As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents LblCpfCnpj As Label
End Class
