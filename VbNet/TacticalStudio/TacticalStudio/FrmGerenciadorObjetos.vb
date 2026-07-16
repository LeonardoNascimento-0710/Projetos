Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms
Imports TacticalStudio.Core.Classes

Public Class FrmGerenciadorObjetos
    Inherits Form

    Private ReadOnly _campo As CampoTatico

    Private ReadOnly TxtFiltro As New TextBox()
    Private ReadOnly CmbTipo As New ComboBox()
    Private ReadOnly LvObjetos As New ListView()
    Private ReadOnly LblResumo As New Label()
    Private _selecionandoPelaLista As Boolean
    Private _atualizandoLista As Boolean
    Private _sincronizacaoPendente As Boolean

    Public Sub New(campo As CampoTatico)

        If campo Is Nothing Then

            Throw New ArgumentNullException(
                NameOf(campo))

        End If

        _campo =
            campo

        Text =
            "Objetos e camadas"

        StartPosition =
            FormStartPosition.CenterParent

        FormBorderStyle =
            FormBorderStyle.SizableToolWindow

        MinimumSize =
            New Size(
                720,
                500)

        ClientSize =
            New Size(
                860,
                590)

        ShowInTaskbar =
            False

        CriarInterface()

        AddHandler _campo.ObjetosAlterados,
            AddressOf Campo_ObjetosAlterados

        AddHandler _campo.ObjetoSelecionadoAlterado,
            AddressOf Campo_ObjetoSelecionadoAlterado

        AplicarTemaAtual()
        AtualizarConteudo()

    End Sub

    Private Sub CriarInterface()

        Dim painelPrincipal As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 1,
            .RowCount = 4,
            .Padding = New Padding(10)
        }

        painelPrincipal.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                42.0F))

        painelPrincipal.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                32.0F))

        painelPrincipal.RowStyles.Add(
            New RowStyle(
                SizeType.Percent,
                100.0F))

        painelPrincipal.RowStyles.Add(
            New RowStyle(
                SizeType.Absolute,
                104.0F))

        Controls.Add(
            painelPrincipal)

        Dim painelFiltro As New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = False,
            .Padding = New Padding(0, 4, 0, 0)
        }

        Dim labelFiltro As New Label With {
            .Text = "Localizar:",
            .Width = 70,
            .Height = 28,
            .TextAlign = ContentAlignment.MiddleLeft
        }

        painelFiltro.Controls.Add(
            labelFiltro)

        TxtFiltro.Width =
            270

        TxtFiltro.Height =
            28

        TxtFiltro.Margin =
            New Padding(
                0,
                0,
                14,
                0)

        AddHandler TxtFiltro.TextChanged,
            Sub(sender, e)
                AtualizarConteudo()
            End Sub

        painelFiltro.Controls.Add(
            TxtFiltro)

        Dim labelTipo As New Label With {
            .Text = "Tipo:",
            .Width = 44,
            .Height = 28,
            .TextAlign = ContentAlignment.MiddleLeft
        }

        painelFiltro.Controls.Add(
            labelTipo)

        CmbTipo.Width =
            160

        CmbTipo.Height =
            28

        CmbTipo.DropDownStyle =
            ComboBoxStyle.DropDownList

        CmbTipo.Items.AddRange(
            New Object() {
                "Todos",
                "Jogador",
                "Bola",
                "Cone",
                "Gol",
                "Manequim",
                "Linha tática",
                "Área tática",
                "Marcador",
                "Texto tático"
            })

        CmbTipo.SelectedIndex =
            0

        AddHandler CmbTipo.SelectedIndexChanged,
            Sub(sender, e)
                AtualizarConteudo()
            End Sub

        painelFiltro.Controls.Add(
            CmbTipo)

        painelPrincipal.Controls.Add(
            painelFiltro,
            0,
            0)

        LblResumo.Dock =
            DockStyle.Fill

        LblResumo.TextAlign =
            ContentAlignment.MiddleLeft

        painelPrincipal.Controls.Add(
            LblResumo,
            0,
            1)

        LvObjetos.Dock =
            DockStyle.Fill

        LvObjetos.View =
            View.Details

        LvObjetos.FullRowSelect =
            True

        LvObjetos.MultiSelect =
            True

        LvObjetos.HideSelection =
            False

        LvObjetos.GridLines =
            True

        LvObjetos.Columns.Add(
            "Objeto",
            260)

        LvObjetos.Columns.Add(
            "Tipo",
            110)

        LvObjetos.Columns.Add(
            "Grupo",
            90)

        LvObjetos.Columns.Add(
            "Visível",
            70)

        LvObjetos.Columns.Add(
            "Bloqueado",
            85)

        LvObjetos.Columns.Add(
            "Ordem",
            60)

        AddHandler LvObjetos.SelectedIndexChanged,
            AddressOf LvObjetos_SelectedIndexChanged

        painelPrincipal.Controls.Add(
            LvObjetos,
            0,
            2)

        Dim painelAcoes As New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = True,
            .Padding = New Padding(0, 8, 0, 0)
        }

        painelAcoes.Controls.Add(
            CriarBotaoAcao(
                "Mostrar",
                82,
                AddressOf MostrarSelecionados))

        painelAcoes.Controls.Add(
            CriarBotaoAcao(
                "Ocultar",
                82,
                AddressOf OcultarSelecionados))

        painelAcoes.Controls.Add(
            CriarBotaoAcao(
                "Bloquear",
                88,
                AddressOf BloquearSelecionados))

        painelAcoes.Controls.Add(
            CriarBotaoAcao(
                "Desbloquear",
                96,
                AddressOf DesbloquearSelecionados))

        painelAcoes.Controls.Add(
            CriarBotaoAcao(
                "Subir",
                76,
                AddressOf SubirCamada))

        painelAcoes.Controls.Add(
            CriarBotaoAcao(
                "Descer",
                76,
                AddressOf DescerCamada))

        painelAcoes.Controls.Add(
            CriarBotaoAcao(
                "Frente",
                78,
                AddressOf TrazerParaFrente))

        painelAcoes.Controls.Add(
            CriarBotaoAcao(
                "Trás",
                72,
                AddressOf EnviarParaTras))

        painelAcoes.Controls.Add(
            CriarBotaoAcao(
                "Excluir",
                82,
                AddressOf ExcluirSelecionados,
                True))

        painelPrincipal.Controls.Add(
            painelAcoes,
            0,
            3)

    End Sub

    Private Function CriarBotaoAcao(
    texto As String,
    largura As Integer,
    acao As Action,
    Optional destaque As Boolean = False) As Button

        Dim botao As New Button With {
            .Text = texto,
            .Width = largura,
            .Height = 36,
            .Margin = New Padding(3),
            .FlatStyle = FlatStyle.Flat,
            .Cursor = Cursors.Hand,
            .UseVisualStyleBackColor = False
        }

        If destaque Then

            botao.Tag =
                "Destaque"

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

        botao.FlatAppearance.MouseOverBackColor =
            Tema.PainelHover

        AddHandler botao.Click,
            Sub(sender, e)
                acao.Invoke()
            End Sub

        Return botao

    End Function

    Public Sub AplicarTemaAtual()

        BackColor =
            Tema.Fundo

        ForeColor =
            Tema.Texto

        Font =
            Tema.FontePadrao

        AplicarTemaRecursivo(
            Controls)

        LvObjetos.BackColor =
            Tema.CampoEntrada

        LvObjetos.ForeColor =
            Tema.TextoCampo

        TxtFiltro.BackColor =
            Tema.CampoEntrada

        TxtFiltro.ForeColor =
            Tema.TextoCampo

        CmbTipo.BackColor =
            Tema.CampoEntrada

        CmbTipo.ForeColor =
            Tema.TextoCampo

        Invalidate(
            True)

        Refresh()

    End Sub

    Private Sub AplicarTemaRecursivo(
    controles As Control.ControlCollection)

        For Each controle As Control In controles

            If TypeOf controle Is FlowLayoutPanel OrElse
               TypeOf controle Is TableLayoutPanel OrElse
               TypeOf controle Is Panel Then

                controle.BackColor =
                    Tema.Fundo

                controle.ForeColor =
                    Tema.Texto

            ElseIf TypeOf controle Is Label Then

                controle.BackColor =
                    Color.Transparent

                controle.ForeColor =
                    Tema.Texto

            ElseIf TypeOf controle Is Button Then

                Dim botao As Button =
                    DirectCast(
                        controle,
                        Button)

                botao.UseVisualStyleBackColor =
                    False

                If String.Equals(
                    TryCast(
                        botao.Tag,
                        String),
                    "Destaque",
                    StringComparison.Ordinal) Then

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

                botao.FlatAppearance.MouseOverBackColor =
                    Tema.PainelHover

            End If

            If controle.HasChildren Then

                AplicarTemaRecursivo(
                    controle.Controls)

            End If

        Next

    End Sub

    Public Sub AtualizarConteudo()

        If IsDisposed Then
            Exit Sub
        End If

        _atualizandoLista =
            True

        Try

            Dim selecionados As New HashSet(Of ObjetoCampo)(
                _campo.ObjetosSelecionadosAtuais)

            Dim filtro As String =
                TxtFiltro.Text.Trim()

            Dim tipoSelecionado As String =
                "Todos"

            If CmbTipo.SelectedItem IsNot Nothing Then

                tipoSelecionado =
                    CStr(
                        CmbTipo.SelectedItem)

            End If

            Dim objetos As IReadOnlyList(Of ObjetoCampo) =
                _campo.ObjetosAtuais

            LvObjetos.BeginUpdate()
            LvObjetos.Items.Clear()

            Dim quantidadeExibida As Integer =
                0

            Dim ordemFrente As Integer =
                1

            For indice As Integer =
                objetos.Count - 1 To 0 Step -1

                Dim objeto As ObjetoCampo =
                    objetos(indice)

                Dim tipoObjeto As String =
                    ObterTipoObjeto(
                        objeto)

                Dim descricao As String =
                    ObterDescricaoObjeto(
                        objeto)

                If tipoSelecionado <> "Todos" AndAlso
                   tipoSelecionado <> tipoObjeto Then

                    Continue For

                End If

                If filtro.Length > 0 AndAlso
                   descricao.IndexOf(
                       filtro,
                       StringComparison.CurrentCultureIgnoreCase) < 0 AndAlso
                   tipoObjeto.IndexOf(
                       filtro,
                       StringComparison.CurrentCultureIgnoreCase) < 0 Then

                    Continue For

                End If

                Dim grupo As String =
                    "-"

                If Not String.IsNullOrWhiteSpace(
                    objeto.GrupoId) Then

                    grupo =
                        "#" &
                        objeto.GrupoId.Substring(
                            0,
                            Math.Min(
                                6,
                                objeto.GrupoId.Length)).
                        ToUpperInvariant()

                End If

                Dim item As New ListViewItem(
                    descricao)

                item.Tag =
                    objeto

                item.SubItems.Add(
                    tipoObjeto)

                item.SubItems.Add(
                    grupo)

                item.SubItems.Add(
                    If(
                        objeto.Visivel,
                        "Sim",
                        "Não"))

                item.SubItems.Add(
                    If(
                        objeto.Bloqueado,
                        "Sim",
                        "Não"))

                item.SubItems.Add(
                    ordemFrente.ToString())

                item.Selected =
                    selecionados.Contains(
                        objeto)

                LvObjetos.Items.Add(
                    item)

                quantidadeExibida +=
                    1

                ordemFrente +=
                    1

            Next

            LvObjetos.EndUpdate()

            LblResumo.Text =
                quantidadeExibida.ToString() &
                " de " &
                objetos.Count.ToString() &
                " objetos exibidos — ordem 1 é a frente."

        Finally

            _atualizandoLista =
                False

        End Try

    End Sub

    Private Function ObterTipoObjeto(
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

    Private Function ObterDescricaoObjeto(
    objeto As ObjetoCampo) As String

        If TypeOf objeto Is Jogador Then

            Dim jogador As Jogador =
                DirectCast(
                    objeto,
                    Jogador)

            Dim descricao As String =
                "Jogador " &
                jogador.Numero.ToString()

            If Not String.IsNullOrWhiteSpace(
                jogador.Nome) Then

                descricao &=
                    " - " &
                    jogador.Nome.Trim()

            End If

            Return descricao

        End If

        If TypeOf objeto Is MarcadorTatico Then

            Dim marcador As MarcadorTatico =
                DirectCast(
                    objeto,
                    MarcadorTatico)

            Return MontarDescricaoComTexto(
                "Marcador",
                marcador.Texto)

        End If

        If TypeOf objeto Is TextoTatico Then

            Dim texto As TextoTatico =
                DirectCast(
                    objeto,
                    TextoTatico)

            Return MontarDescricaoComTexto(
                "Texto",
                texto.Texto)

        End If

        Return ObterTipoObjeto(
            objeto)

    End Function

    Private Function MontarDescricaoComTexto(
    prefixo As String,
    texto As String) As String

        If String.IsNullOrWhiteSpace(
            texto) Then

            Return prefixo

        End If

        Dim textoSeguro As String =
            texto.Trim().
            Replace(
                Environment.NewLine,
                " ").
            Replace(
                vbLf,
                " ").
            Replace(
                vbCr,
                " ")

        If textoSeguro.Length > 34 Then

            textoSeguro =
                textoSeguro.Substring(
                    0,
                    34) &
                "…"

        End If

        Return prefixo &
            " - " &
            textoSeguro

    End Function

    Private Sub LvObjetos_SelectedIndexChanged(
    sender As Object,
    e As EventArgs)

        If _atualizandoLista OrElse
           _sincronizacaoPendente Then

            Exit Sub

        End If

        _sincronizacaoPendente =
            True

        BeginInvoke(
            New MethodInvoker(
                AddressOf SincronizarSelecaoComCampo))

    End Sub

    Private Sub SincronizarSelecaoComCampo()

        _sincronizacaoPendente =
        False

        If IsDisposed OrElse
       _atualizandoLista Then

            Exit Sub

        End If

        _selecionandoPelaLista =
        True

        Try

            _campo.SelecionarObjetosPelaLista(
            ObterObjetosSelecionadosLista())

            AtualizarSelecaoListaSemRecriar()

        Finally

            _selecionandoPelaLista =
            False

        End Try

    End Sub

    Private Sub AtualizarSelecaoListaSemRecriar()

        If IsDisposed Then
            Exit Sub
        End If

        Dim objetosSelecionados As New HashSet(
        Of ObjetoCampo)(
        _campo.ObjetosSelecionadosAtuais)

        _atualizandoLista =
        True

        Try

            For Each item As ListViewItem
            In LvObjetos.Items

                Dim objeto As ObjetoCampo =
                TryCast(
                    item.Tag,
                    ObjetoCampo)

                item.Selected =
                objeto IsNot Nothing AndAlso
                objetosSelecionados.Contains(
                    objeto)

            Next

        Finally

            _atualizandoLista =
            False

        End Try

    End Sub

    Private Function ObterObjetosSelecionadosLista() As List(Of ObjetoCampo)

        Dim objetos As New List(Of ObjetoCampo)()

        For Each item As ListViewItem In LvObjetos.SelectedItems

            Dim objeto As ObjetoCampo =
                TryCast(
                    item.Tag,
                    ObjetoCampo)

            If objeto IsNot Nothing AndAlso
               Not objetos.Contains(objeto) Then

                objetos.Add(
                    objeto)

            End If

        Next

        Return objetos

    End Function

    Private Function PrepararObjetosSelecionados() As List(Of ObjetoCampo)

        Dim objetos As List(Of ObjetoCampo) =
            ObterObjetosSelecionadosLista()

        If objetos.Count = 0 Then

            MessageBox.Show(
                Me,
                "Selecione pelo menos um objeto na lista.",
                "Objetos e camadas",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information)

            Return objetos

        End If

        _campo.SelecionarObjetosPelaLista(
            objetos)

        Return New List(Of ObjetoCampo)(
            _campo.ObjetosSelecionadosAtuais)

    End Function

    Private Sub MostrarSelecionados()

        Dim objetos As List(Of ObjetoCampo) =
            PrepararObjetosSelecionados()

        If objetos.Count = 0 Then
            Exit Sub
        End If

        _campo.DefinirVisibilidadeObjetos(
            objetos,
            True)

    End Sub

    Private Sub OcultarSelecionados()

        Dim objetos As List(Of ObjetoCampo) =
            PrepararObjetosSelecionados()

        If objetos.Count = 0 Then
            Exit Sub
        End If

        _campo.DefinirVisibilidadeObjetos(
            objetos,
            False)

    End Sub

    Private Sub BloquearSelecionados()

        Dim objetos As List(Of ObjetoCampo) =
            PrepararObjetosSelecionados()

        If objetos.Count = 0 Then
            Exit Sub
        End If

        _campo.DefinirBloqueioObjetos(
            objetos,
            True)

    End Sub

    Private Sub DesbloquearSelecionados()

        Dim objetos As List(Of ObjetoCampo) =
            PrepararObjetosSelecionados()

        If objetos.Count = 0 Then
            Exit Sub
        End If

        _campo.DefinirBloqueioObjetos(
            objetos,
            False)

    End Sub

    Private Sub SubirCamada()

        If PrepararObjetosSelecionados().Count = 0 Then
            Exit Sub
        End If

        _campo.SubirCamadaSelecionados()

    End Sub

    Private Sub DescerCamada()

        If PrepararObjetosSelecionados().Count = 0 Then
            Exit Sub
        End If

        _campo.DescerCamadaSelecionados()

    End Sub

    Private Sub TrazerParaFrente()

        If PrepararObjetosSelecionados().Count = 0 Then
            Exit Sub
        End If

        _campo.TrazerSelecionadosParaFrente()

    End Sub

    Private Sub EnviarParaTras()

        If PrepararObjetosSelecionados().Count = 0 Then
            Exit Sub
        End If

        _campo.EnviarSelecionadosParaTras()

    End Sub

    Private Sub ExcluirSelecionados()

        If PrepararObjetosSelecionados().Count = 0 Then
            Exit Sub
        End If

        _campo.ExcluirSelecionado()

    End Sub

    Private Sub Campo_ObjetosAlterados()

        If IsDisposed OrElse
           Not IsHandleCreated Then

            Exit Sub

        End If

        BeginInvoke(
            New MethodInvoker(
                AddressOf AtualizarConteudo))

    End Sub

    Private Sub Campo_ObjetoSelecionadoAlterado(objeto As ObjetoCampo)

        If IsDisposed OrElse
       Not IsHandleCreated Then

            Exit Sub

        End If

        If _selecionandoPelaLista Then
            Exit Sub
        End If

        BeginInvoke(
        New MethodInvoker(
            AddressOf AtualizarSelecaoListaSemRecriar))

    End Sub

    Protected Overrides Sub OnFormClosed(
    e As FormClosedEventArgs)

        RemoveHandler _campo.ObjetosAlterados,
            AddressOf Campo_ObjetosAlterados

        RemoveHandler _campo.ObjetoSelecionadoAlterado,
            AddressOf Campo_ObjetoSelecionadoAlterado

        MyBase.OnFormClosed(
            e)

    End Sub

End Class
