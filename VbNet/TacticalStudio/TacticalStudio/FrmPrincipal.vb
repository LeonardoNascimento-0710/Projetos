Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports TacticalStudio.Core.Enums
Imports TacticalStudio.Core.Classes

Public Class FrmPrincipal

    Private CampoCanvas As CampoTatico

    Private _caminhoArquivoAtual As String =
    String.Empty

    Private _nomeExercicioAtual As String =
    "Novo exercício"

    Private ReadOnly _botoesFerramentas As New Dictionary(Of FerramentaCampo, Button)()

    Private Sub FrmPrincipal_Load(
    sender As Object,
    e As EventArgs) Handles MyBase.Load

        KeyPreview = True

        AplicarTema()

        CriarCampoTatico()

        CriarBarraFerramentas()

        CriarBarraArquivo()

        AtualizarTituloJanela()

    End Sub

    Private Sub AplicarTema()

        BackColor = Tema.Fundo
        ForeColor = Tema.Texto
        Font = Tema.FontePadrao

        PnlSuperior.BackColor = Tema.CorPrimaria
        PnlEsquerdo.BackColor = Tema.Painel
        PnlDireito.BackColor = Tema.Painel
        PnlInferior.BackColor = Tema.Painel
        PnlCentral.BackColor = Tema.Fundo

    End Sub

    Private Sub CriarCampoTatico()

        CampoCanvas = New CampoTatico With {
        .Dock = DockStyle.Fill
    }

        AddHandler CampoCanvas.FerramentaAtualAlterada,
        AddressOf CampoCanvas_FerramentaAtualAlterada

        AddHandler CampoCanvas.ObjetoSelecionadoAlterado,
    AddressOf CampoCanvas_ObjetoSelecionadoAlterado

        PnlCentral.Controls.Clear()
        PnlCentral.Controls.Add(CampoCanvas)

    End Sub

    Private Sub CampoCanvas_ObjetoSelecionadoAlterado(
    objeto As ObjetoCampo)

        MontarPainelPropriedades(objeto)

    End Sub

    Private Sub CriarBarraFerramentas()

        PnlEsquerdo.Controls.Clear()
        _botoesFerramentas.Clear()

        Dim painelFerramentas As New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False,
            .AutoScroll = True,
            .BackColor = Tema.Painel,
            .Padding = New Padding(6)
        }

        Dim larguraBotao As Integer =
            Math.Max(
                120,
                PnlEsquerdo.ClientSize.Width - 24)

        Dim titulo As New Label With {
            .Text = "FERRAMENTAS",
            .ForeColor = Tema.Texto,
            .Font = New Font(
                "Segoe UI",
                10.0F,
                FontStyle.Bold),
            .Width = larguraBotao,
            .Height = 34,
            .TextAlign = ContentAlignment.MiddleLeft
        }

        painelFerramentas.Controls.Add(titulo)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Selecionar,
            "Selecionar",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Jogador,
            "Jogador",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Bola,
            "Bola",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Cone,
            "Cone",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Gol,
            "Gol",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Manequim,
            "Manequim",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.LinhaContinua,
            "Linha",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.LinhaTracejada,
            "Linha tracejada",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Seta,
            "Seta",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Area,
            "Área",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Marcador,
            "Marcador",
            larguraBotao)

        AdicionarBotaoFerramenta(
            painelFerramentas,
            FerramentaCampo.Texto,
            "Texto",
            larguraBotao)

        PnlEsquerdo.Controls.Add(painelFerramentas)

        AtualizarBotoesFerramentas(
            FerramentaCampo.Selecionar)

    End Sub

    Private Sub AdicionarBotaoFerramenta(
    painel As FlowLayoutPanel,
    ferramenta As FerramentaCampo,
    texto As String,
    largura As Integer)

        Dim botao As New Button With {
            .Text = texto,
            .Tag = ferramenta,
            .Width = largura,
            .Height = 36,
            .Margin = New Padding(0, 3, 0, 3),
            .FlatStyle = FlatStyle.Flat,
            .TextAlign = ContentAlignment.MiddleLeft,
            .Cursor = Cursors.Hand,
            .BackColor = Tema.Painel,
            .ForeColor = Tema.Texto
        }

        botao.FlatAppearance.BorderColor =
            Tema.Borda

        botao.FlatAppearance.MouseOverBackColor =
            Color.FromArgb(55, 55, 55)

        botao.FlatAppearance.MouseDownBackColor =
            Tema.CorPrimaria

        AddHandler botao.Click,
            AddressOf BotaoFerramenta_Click

        _botoesFerramentas.Add(
            ferramenta,
            botao)

        painel.Controls.Add(botao)

    End Sub

    Private Sub BotaoFerramenta_Click(
    sender As Object,
    e As EventArgs)

        Dim botao As Button =
            TryCast(sender, Button)

        If botao Is Nothing Then
            Exit Sub
        End If

        If botao.Tag Is Nothing Then
            Exit Sub
        End If

        Dim ferramenta As FerramentaCampo =
    CType(botao.Tag, FerramentaCampo)

        CampoCanvas.FerramentaAtual =
            ferramenta

        AtualizarBotoesFerramentas(
            ferramenta)

        CampoCanvas.Focus()

    End Sub

    Private Sub CampoCanvas_FerramentaAtualAlterada(
    ferramenta As FerramentaCampo)

        AtualizarBotoesFerramentas(
        ferramenta)

    End Sub

    Private Sub AtualizarBotoesFerramentas(
    ferramentaAtual As FerramentaCampo)

        For Each item As KeyValuePair(
            Of FerramentaCampo,
            Button) In _botoesFerramentas

            Dim botao As Button =
                item.Value

            If item.Key = ferramentaAtual Then

                botao.BackColor =
                    Tema.CorPrimaria

                botao.ForeColor =
                    Color.White

                botao.FlatAppearance.BorderColor =
                    Color.White

            Else

                botao.BackColor =
                    Tema.Painel

                botao.ForeColor =
                    Tema.Texto

                botao.FlatAppearance.BorderColor =
                    Tema.Borda

            End If

        Next

    End Sub

    Private Sub MontarPainelPropriedades(
    objeto As ObjetoCampo)

        PnlDireito.Controls.Clear()

        Dim painel As New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False,
            .AutoScroll = True,
            .BackColor = Tema.Painel,
            .Padding = New Padding(10)
        }

        PnlDireito.Controls.Add(painel)

        Dim largura As Integer =
            Math.Max(
                180,
                PnlDireito.ClientSize.Width - 32)

        Dim titulo As New Label With {
            .Text = "PROPRIEDADES",
            .ForeColor = Tema.Texto,
            .Font = New Font(
                "Segoe UI",
                11.0F,
                FontStyle.Bold),
            .Width = largura,
            .Height = 32,
            .TextAlign = ContentAlignment.MiddleLeft
        }

        painel.Controls.Add(titulo)

        If objeto Is Nothing Then

            Dim mensagem As New Label With {
                .Text = "Nenhum objeto selecionado.",
                .ForeColor = Color.Silver,
                .Font = Tema.FontePadrao,
                .Width = largura,
                .Height = 50,
                .TextAlign = ContentAlignment.MiddleLeft
            }

            painel.Controls.Add(mensagem)

            Exit Sub

        End If

        Dim tipoObjeto As New Label With {
            .Text = ObterNomeObjeto(objeto),
            .ForeColor = Tema.CorPrimaria,
            .Font = New Font(
                "Segoe UI",
                10.0F,
                FontStyle.Bold),
            .Width = largura,
            .Height = 30,
            .TextAlign = ContentAlignment.MiddleLeft
        }

        painel.Controls.Add(tipoObjeto)

        If TypeOf objeto Is Jogador Then

            MontarPropriedadesJogador(
                painel,
                DirectCast(objeto, Jogador))

        ElseIf TypeOf objeto Is Bola Then

            AdicionarMensagemPainel(
                painel,
                "A bola não possui propriedades editáveis no momento.")

        ElseIf TypeOf objeto Is Cone Then

            MontarPropriedadesCone(
                painel,
                DirectCast(objeto, Cone))

        ElseIf TypeOf objeto Is Gol Then

            MontarPropriedadesGol(
                painel,
                DirectCast(objeto, Gol))

        ElseIf TypeOf objeto Is Manequim Then

            MontarPropriedadesManequim(
                painel,
                DirectCast(objeto, Manequim))

        ElseIf TypeOf objeto Is LinhaTatica Then

            MontarPropriedadesLinha(
                painel,
                DirectCast(objeto, LinhaTatica))

        ElseIf TypeOf objeto Is AreaTatica Then

            MontarPropriedadesArea(
                painel,
                DirectCast(objeto, AreaTatica))

        ElseIf TypeOf objeto Is MarcadorTatico Then

            MontarPropriedadesMarcador(
                painel,
                DirectCast(objeto, MarcadorTatico))

        ElseIf TypeOf objeto Is TextoTatico Then

            MontarPropriedadesTexto(
                painel,
                DirectCast(objeto, TextoTatico))

        End If

        Dim tituloAcoes As New Label With {
    .Text = "AÇÕES",
    .ForeColor = Tema.Texto,
    .Font = New Font(
        "Segoe UI",
        9.0F,
        FontStyle.Bold),
    .Width = largura,
    .Height = 28,
    .Margin = New Padding(
        0,
        12,
        0,
        2),
    .TextAlign =
        ContentAlignment.MiddleLeft
}

        painel.Controls.Add(
    tituloAcoes)

        Dim botaoDuplicar As New Button With {
    .Text = "Duplicar objeto  (Ctrl+D)",
    .Width = largura,
    .Height = 36,
    .Margin = New Padding(
        0,
        3,
        0,
        3),
    .FlatStyle = FlatStyle.Flat,
    .BackColor = Tema.Painel,
    .ForeColor = Tema.Texto,
    .Cursor = Cursors.Hand
}

        botaoDuplicar.FlatAppearance.BorderColor =
    Tema.Borda

        AddHandler botaoDuplicar.Click,
    Sub(sender, e)

        CampoCanvas.DuplicarSelecionado()
        CampoCanvas.Focus()

    End Sub

        painel.Controls.Add(
    botaoDuplicar)

        Dim botaoFrente As New Button With {
    .Text = "Trazer para frente",
    .Width = largura,
    .Height = 36,
    .Margin = New Padding(
        0,
        3,
        0,
        3),
    .FlatStyle = FlatStyle.Flat,
    .BackColor = Tema.Painel,
    .ForeColor = Tema.Texto,
    .Cursor = Cursors.Hand
}

        botaoFrente.FlatAppearance.BorderColor =
    Tema.Borda

        AddHandler botaoFrente.Click,
    Sub(sender, e)

        CampoCanvas.TrazerParaFrente()
        CampoCanvas.Focus()

    End Sub

        painel.Controls.Add(
    botaoFrente)

        Dim botaoTras As New Button With {
    .Text = "Enviar para trás",
    .Width = largura,
    .Height = 36,
    .Margin = New Padding(
        0,
        3,
        0,
        3),
    .FlatStyle = FlatStyle.Flat,
    .BackColor = Tema.Painel,
    .ForeColor = Tema.Texto,
    .Cursor = Cursors.Hand
}

        botaoTras.FlatAppearance.BorderColor =
    Tema.Borda

        AddHandler botaoTras.Click,
    Sub(sender, e)

        CampoCanvas.EnviarParaTras()
        CampoCanvas.Focus()

    End Sub

        painel.Controls.Add(
    botaoTras)

        Dim botaoExcluir As New Button With {
            .Text = "Excluir objeto",
            .Width = largura,
            .Height = 38,
            .Margin = New Padding(0, 12, 0, 4),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Tema.CorPrimaria,
            .ForeColor = Color.White,
            .Cursor = Cursors.Hand
        }

        botaoExcluir.FlatAppearance.BorderColor =
            Color.White

        AddHandler botaoExcluir.Click,
            Sub(sender, e)

                CampoCanvas.ExcluirSelecionado()
                CampoCanvas.Focus()

            End Sub

        painel.Controls.Add(botaoExcluir)

    End Sub

    Private Sub AdicionarCampoPainel(
    painel As FlowLayoutPanel,
    titulo As String,
    controle As Control)

        Dim largura As Integer =
            Math.Max(
                180,
                PnlDireito.ClientSize.Width - 32)

        Dim container As New Panel With {
            .Width = largura,
            .Height = 60,
            .Margin = New Padding(0, 4, 0, 4),
            .BackColor = Tema.Painel
        }

        Dim labelCampo As New Label With {
            .Text = titulo,
            .ForeColor = Tema.Texto,
            .Font = New Font(
                "Segoe UI",
                8.5F,
                FontStyle.Regular),
            .Left = 0,
            .Top = 0,
            .Width = largura,
            .Height = 22
        }

        controle.Left = 0
        controle.Top = 24
        controle.Width = largura
        controle.Height = 28
        controle.Anchor =
            AnchorStyles.Left Or
            AnchorStyles.Right Or
            AnchorStyles.Top

        container.Controls.Add(labelCampo)
        container.Controls.Add(controle)

        painel.Controls.Add(container)

    End Sub

    Private Sub AdicionarMensagemPainel(
        painel As FlowLayoutPanel,
        mensagem As String)

        Dim largura As Integer =
            Math.Max(
                180,
                PnlDireito.ClientSize.Width - 32)

        Dim labelMensagem As New Label With {
            .Text = mensagem,
            .ForeColor = Color.Silver,
            .Font = Tema.FontePadrao,
            .Width = largura,
            .Height = 55,
            .TextAlign = ContentAlignment.MiddleLeft
        }

        painel.Controls.Add(labelMensagem)

    End Sub

    Private Function CriarComboEnum(
        tipoEnum As Type,
        valorAtual As Object) As ComboBox

        Dim combo As New ComboBox With {
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .BackColor = Color.FromArgb(50, 50, 50),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }

        combo.DataSource =
            [Enum].GetValues(tipoEnum)

        combo.SelectedItem =
            valorAtual

        Return combo

    End Function

    Private Function CriarCampoNumerico(
        minimo As Decimal,
        maximo As Decimal,
        valor As Decimal,
        Optional casasDecimais As Integer = 0,
        Optional incremento As Decimal = 1D) As NumericUpDown

        Return New NumericUpDown With {
            .Minimum = minimo,
            .Maximum = maximo,
            .Value = Math.Max(
                minimo,
                Math.Min(maximo, valor)),
            .DecimalPlaces = casasDecimais,
            .Increment = incremento,
            .BackColor = Color.FromArgb(50, 50, 50),
            .ForeColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle
        }

    End Function

    Private Function ObterNomeObjeto(
    objeto As ObjetoCampo) As String

        If TypeOf objeto Is Jogador Then
            Return "Jogador"
        End If

        If TypeOf objeto Is Bola Then
            Return "Bola"
        End If

        If TypeOf objeto Is Cone Then
            Return "Cone"
        End If

        If TypeOf objeto Is Gol Then
            Return "Gol"
        End If

        If TypeOf objeto Is Manequim Then
            Return "Manequim"
        End If

        If TypeOf objeto Is LinhaTatica Then
            Return "Linha tática"
        End If

        If TypeOf objeto Is AreaTatica Then
            Return "Área tática"
        End If

        If TypeOf objeto Is MarcadorTatico Then
            Return "Marcador"
        End If

        If TypeOf objeto Is TextoTatico Then
            Return "Texto tático"
        End If

        Return objeto.GetType().Name

    End Function

    Private Sub MontarPropriedadesJogador(
    painel As FlowLayoutPanel,
    jogador As Jogador)

        Dim numero As NumericUpDown =
            CriarCampoNumerico(
                1D,
                99D,
                jogador.Numero)

        AddHandler numero.ValueChanged,
            Sub(sender, e)

                jogador.Numero =
                    CInt(numero.Value)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Número",
            numero)

        Dim nome As New TextBox With {
            .Text = jogador.Nome,
            .BackColor = Color.FromArgb(50, 50, 50),
            .ForeColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle
        }

        AddHandler nome.TextChanged,
            Sub(sender, e)

                jogador.Nome =
                    nome.Text

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Nome",
            nome)

    End Sub

    Private Sub MontarPropriedadesCone(
    painel As FlowLayoutPanel,
    cone As Cone)

        Dim cor As ComboBox =
            CriarComboEnum(
                GetType(CorCone),
                cone.Cor)

        AddHandler cor.SelectedIndexChanged,
            Sub(sender, e)

                If cor.SelectedItem Is Nothing Then
                    Exit Sub
                End If

                cone.Cor =
                    CType(
                        cor.SelectedItem,
                        CorCone)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Cor",
            cor)

    End Sub

    Private Sub MontarPropriedadesGol(
    painel As FlowLayoutPanel,
    gol As Gol)

        Dim orientacao As ComboBox =
            CriarComboEnum(
                GetType(OrientacaoGol),
                gol.Orientacao)

        AddHandler orientacao.SelectedIndexChanged,
            Sub(sender, e)

                If orientacao.SelectedItem Is Nothing Then
                    Exit Sub
                End If

                gol.Orientacao =
                    CType(
                        orientacao.SelectedItem,
                        OrientacaoGol)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Orientação",
            orientacao)

    End Sub

    Private Sub MontarPropriedadesManequim(
    painel As FlowLayoutPanel,
    manequim As Manequim)

        Dim cor As ComboBox =
            CriarComboEnum(
                GetType(CorManequim),
                manequim.Cor)

        AddHandler cor.SelectedIndexChanged,
            Sub(sender, e)

                If cor.SelectedItem Is Nothing Then
                    Exit Sub
                End If

                manequim.Cor =
                    CType(
                        cor.SelectedItem,
                        CorManequim)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Cor",
            cor)

    End Sub

    Private Sub MontarPropriedadesLinha(
    painel As FlowLayoutPanel,
    linha As LinhaTatica)

        Dim tipo As ComboBox =
            CriarComboEnum(
                GetType(TipoLinhaTatica),
                linha.Tipo)

        AddHandler tipo.SelectedIndexChanged,
            Sub(sender, e)

                If tipo.SelectedItem Is Nothing Then
                    Exit Sub
                End If

                linha.Tipo =
                    CType(
                        tipo.SelectedItem,
                        TipoLinhaTatica)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Tipo",
            tipo)

        Dim cor As ComboBox =
            CriarComboEnum(
                GetType(CorLinhaTatica),
                linha.Cor)

        AddHandler cor.SelectedIndexChanged,
            Sub(sender, e)

                If cor.SelectedItem Is Nothing Then
                    Exit Sub
                End If

                linha.Cor =
                    CType(
                        cor.SelectedItem,
                        CorLinhaTatica)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Cor",
            cor)

        Dim espessura As NumericUpDown =
            CriarCampoNumerico(
                1D,
                12D,
                CDec(linha.Espessura),
                1,
                0.5D)

        AddHandler espessura.ValueChanged,
            Sub(sender, e)

                linha.Espessura =
                    CSng(espessura.Value)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Espessura",
            espessura)

    End Sub

    Private Sub MontarPropriedadesArea(
    painel As FlowLayoutPanel,
    area As AreaTatica)

        Dim cor As ComboBox =
            CriarComboEnum(
                GetType(CorAreaTatica),
                area.Cor)

        AddHandler cor.SelectedIndexChanged,
            Sub(sender, e)

                If cor.SelectedItem Is Nothing Then
                    Exit Sub
                End If

                area.Cor =
                    CType(
                        cor.SelectedItem,
                        CorAreaTatica)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Cor",
            cor)

        Dim tracejada As New CheckBox With {
            .Text = "Borda tracejada",
            .Checked = area.Tracejada,
            .ForeColor = Tema.Texto,
            .BackColor = Tema.Painel
        }

        AddHandler tracejada.CheckedChanged,
            Sub(sender, e)

                area.Tracejada =
                    tracejada.Checked

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Estilo da borda",
            tracejada)

        Dim opacidade As NumericUpDown =
            CriarCampoNumerico(
                0D,
                255D,
                area.Opacidade)

        AddHandler opacidade.ValueChanged,
            Sub(sender, e)

                area.Opacidade =
                    CInt(opacidade.Value)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Opacidade",
            opacidade)

        Dim espessura As NumericUpDown =
            CriarCampoNumerico(
                1D,
                12D,
                CDec(area.Espessura),
                1,
                0.5D)

        AddHandler espessura.ValueChanged,
            Sub(sender, e)

                area.Espessura =
                    CSng(espessura.Value)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Espessura",
            espessura)

    End Sub

    Private Sub MontarPropriedadesMarcador(
    painel As FlowLayoutPanel,
    marcador As MarcadorTatico)

        Dim texto As New TextBox With {
            .Text = marcador.Texto,
            .MaxLength = 5,
            .BackColor = Color.FromArgb(50, 50, 50),
            .ForeColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle
        }

        AddHandler texto.TextChanged,
            Sub(sender, e)

                marcador.Texto =
                    texto.Text

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Texto",
            texto)

        Dim cor As ComboBox =
            CriarComboEnum(
                GetType(CorMarcadorTatico),
                marcador.Cor)

        AddHandler cor.SelectedIndexChanged,
            Sub(sender, e)

                If cor.SelectedItem Is Nothing Then
                    Exit Sub
                End If

                marcador.Cor =
                    CType(
                        cor.SelectedItem,
                        CorMarcadorTatico)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Cor",
            cor)

        Dim diametro As NumericUpDown =
            CriarCampoNumerico(
                22D,
                80D,
                CDec(marcador.Diametro),
                1,
                1D)

        AddHandler diametro.ValueChanged,
            Sub(sender, e)

                marcador.Diametro =
                    CSng(diametro.Value)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Diâmetro",
            diametro)

    End Sub

    Private Sub MontarPropriedadesTexto(
    painel As FlowLayoutPanel,
    textoTatico As TextoTatico)

        Dim texto As New TextBox With {
            .Text = textoTatico.Texto,
            .BackColor = Color.FromArgb(50, 50, 50),
            .ForeColor = Color.White,
            .BorderStyle = BorderStyle.FixedSingle
        }

        AddHandler texto.TextChanged,
            Sub(sender, e)

                textoTatico.Texto =
                    texto.Text

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Texto",
            texto)

        Dim cor As ComboBox =
            CriarComboEnum(
                GetType(CorTextoTatico),
                textoTatico.Cor)

        AddHandler cor.SelectedIndexChanged,
            Sub(sender, e)

                If cor.SelectedItem Is Nothing Then
                    Exit Sub
                End If

                textoTatico.Cor =
                    CType(
                        cor.SelectedItem,
                        CorTextoTatico)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Cor",
            cor)

        Dim tamanho As NumericUpDown =
            CriarCampoNumerico(
                8D,
                48D,
                CDec(textoTatico.TamanhoFonte),
                1,
                1D)

        AddHandler tamanho.ValueChanged,
            Sub(sender, e)

                textoTatico.TamanhoFonte =
                    CSng(tamanho.Value)

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Tamanho da fonte",
            tamanho)

        Dim negrito As New CheckBox With {
            .Text = "Negrito",
            .Checked = textoTatico.Negrito,
            .ForeColor = Tema.Texto,
            .BackColor = Tema.Painel
        }

        AddHandler negrito.CheckedChanged,
            Sub(sender, e)

                textoTatico.Negrito =
                    negrito.Checked

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Estilo",
            negrito)

        Dim fundo As New CheckBox With {
            .Text = "Exibir fundo",
            .Checked = textoTatico.FundoVisivel,
            .ForeColor = Tema.Texto,
            .BackColor = Tema.Painel
        }

        AddHandler fundo.CheckedChanged,
            Sub(sender, e)

                textoTatico.FundoVisivel =
                    fundo.Checked

                CampoCanvas.RegistrarAlteracaoExterna()

            End Sub

        AdicionarCampoPainel(
            painel,
            "Fundo",
            fundo)

    End Sub

    Protected Overrides Function ProcessCmdKey(
    ByRef msg As Message,
    keyData As Keys) As Boolean

        If CampoCanvas Is Nothing Then

            Return MyBase.ProcessCmdKey(
            msg,
            keyData)

        End If

        Dim tecla As Keys = keyData And Keys.KeyCode

        Dim modificadores As Keys = keyData And Keys.Modifiers

        If modificadores = (Keys.Control Or Keys.Shift) AndAlso tecla = Keys.S Then

            SalvarExercicio(
        True)

            Return True

        End If

        If modificadores = Keys.Control AndAlso
   tecla = Keys.S Then

            SalvarExercicio()

            Return True

        End If

        If modificadores = Keys.Control AndAlso
   tecla = Keys.O Then

            AbrirExercicio()

            Return True

        End If

        If modificadores = Keys.Control AndAlso
   tecla = Keys.N Then

            NovoExercicio_Click(
        Me,
        EventArgs.Empty)

            Return True

        End If

        If modificadores = Keys.Control AndAlso
   tecla = Keys.D Then

            CampoCanvas.DuplicarSelecionado()

            Return True

        End If

        If modificadores =
   (Keys.Control Or Keys.Shift) AndAlso
   tecla = Keys.Up Then

            CampoCanvas.TrazerParaFrente()

            Return True

        End If

        If modificadores =
   (Keys.Control Or Keys.Shift) AndAlso
   tecla = Keys.Down Then

            CampoCanvas.EnviarParaTras()

            Return True

        End If

        If modificadores = Keys.Control AndAlso
       tecla = Keys.Z Then

            CampoCanvas.Desfazer()

            Return True

        End If

        If modificadores = Keys.Control AndAlso
       tecla = Keys.Y Then

            CampoCanvas.Refazer()

            Return True

        End If

        If modificadores =
       (Keys.Control Or Keys.Shift) AndAlso
       tecla = Keys.Z Then

            CampoCanvas.Refazer()

            Return True

        End If

        Return MyBase.ProcessCmdKey(
        msg,
        keyData)

    End Function

    Private Sub CriarBarraArquivo()

        If PnlSuperior.Controls.ContainsKey(
            "PnlArquivoDinamico") Then

            PnlSuperior.Controls.RemoveByKey(
                "PnlArquivoDinamico")

        End If

        Dim painelArquivo As New FlowLayoutPanel With {
            .Name = "PnlArquivoDinamico",
            .Dock = DockStyle.Right,
            .Width = 300,
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = False,
            .Padding = New Padding(5),
            .BackColor = Tema.CorPrimaria
        }

        painelArquivo.Controls.Add(
            CriarBotaoArquivo(
                "Novo",
                AddressOf NovoExercicio_Click))

        painelArquivo.Controls.Add(
            CriarBotaoArquivo(
                "Abrir",
                AddressOf AbrirExercicio_Click))

        painelArquivo.Controls.Add(
            CriarBotaoArquivo(
                "Salvar",
                AddressOf SalvarExercicio_Click))

        PnlSuperior.Controls.Add(
            painelArquivo)

        painelArquivo.BringToFront()

    End Sub

    Private Function CriarBotaoArquivo(
        texto As String,
        acao As EventHandler) As Button

        Dim botao As New Button With {
            .Text = texto,
            .Width = 88,
            .Height = 34,
            .Margin = New Padding(3),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Tema.Painel,
            .ForeColor = Color.White,
            .Cursor = Cursors.Hand
        }

        botao.FlatAppearance.BorderColor =
            Color.White

        AddHandler botao.Click,
            acao

        Return botao

    End Function

    Private Sub NovoExercicio_Click(
    sender As Object,
    e As EventArgs)

        Dim resposta As DialogResult =
            MessageBox.Show(
                "Deseja iniciar um novo exercício?" &
                Environment.NewLine &
                "As alterações não salvas serão perdidas.",
                "Novo exercício",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question)

        If resposta <> DialogResult.Yes Then
            Exit Sub
        End If

        CampoCanvas.NovoExercicio()

        _caminhoArquivoAtual =
            String.Empty

        _nomeExercicioAtual =
            "Novo exercício"

        AtualizarTituloJanela()

        CampoCanvas.Focus()

    End Sub

    Private Sub AbrirExercicio_Click(
        sender As Object,
        e As EventArgs)

        AbrirExercicio()

    End Sub

    Private Sub SalvarExercicio_Click(
        sender As Object,
        e As EventArgs)

        SalvarExercicio()

    End Sub

    Private Sub AbrirExercicio()

        Using dialogo As New OpenFileDialog()

            dialogo.Title =
                "Abrir exercício tático"

            dialogo.Filter =
                "Exercício TacticalStudio (*.tactical)|*.tactical|" &
                "Arquivo JSON (*.json)|*.json|" &
                "Todos os arquivos (*.*)|*.*"

            dialogo.Multiselect =
                False

            If dialogo.ShowDialog() <>
               DialogResult.OK Then

                Exit Sub

            End If

            Try

                Dim conteudoJson As String =
                    File.ReadAllText(
                        dialogo.FileName,
                        Encoding.UTF8)

                CampoCanvas.ImportarExercicioJson(
                    conteudoJson)

                _caminhoArquivoAtual =
                    dialogo.FileName

                _nomeExercicioAtual =
                    Path.GetFileNameWithoutExtension(
                        dialogo.FileName)

                AtualizarTituloJanela()

                CampoCanvas.Focus()

            Catch ex As Exception

                MessageBox.Show(
                    "Não foi possível abrir o exercício." &
                    Environment.NewLine &
                    Environment.NewLine &
                    ex.Message,
                    "Erro ao abrir",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error)

            End Try

        End Using

    End Sub

    Private Sub SalvarExercicio(
    Optional salvarComo As Boolean = False)

        Dim caminhoDestino As String =
            _caminhoArquivoAtual

        If salvarComo OrElse
           String.IsNullOrWhiteSpace(
               caminhoDestino) Then

            Using dialogo As New SaveFileDialog()

                dialogo.Title =
                    "Salvar exercício tático"

                dialogo.Filter =
                    "Exercício TacticalStudio (*.tactical)|*.tactical|" &
                    "Arquivo JSON (*.json)|*.json"

                dialogo.DefaultExt =
                    "tactical"

                dialogo.AddExtension =
                    True

                dialogo.FileName =
                    _nomeExercicioAtual &
                    ".tactical"

                If dialogo.ShowDialog() <>
                   DialogResult.OK Then

                    Exit Sub

                End If

                caminhoDestino =
                    dialogo.FileName

            End Using

        End If

        Try

            Dim nomeExercicio As String =
                Path.GetFileNameWithoutExtension(
                    caminhoDestino)

            Dim conteudoJson As String =
                CampoCanvas.ExportarExercicioJson(
                    nomeExercicio)

            File.WriteAllText(
                caminhoDestino,
                conteudoJson,
                New UTF8Encoding(False))

            _caminhoArquivoAtual =
                caminhoDestino

            _nomeExercicioAtual =
                nomeExercicio

            AtualizarTituloJanela()

        Catch ex As Exception

            MessageBox.Show(
                "Não foi possível salvar o exercício." &
                Environment.NewLine &
                Environment.NewLine &
                ex.Message,
                "Erro ao salvar",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error)

        End Try

    End Sub

    Private Sub AtualizarTituloJanela()

        Text =
            "TacticalStudio - " &
            _nomeExercicioAtual

    End Sub

End Class