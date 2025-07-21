Imports ClosedXML.Excel
Imports System.IO
Imports System.Windows.Forms

Public Class FrmMonitorAcs

    Private WithEvents notify As New NotifyIcon()
    Private Async Sub FrmMonitorAcs_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.ShowInTaskbar = False
        Me.WindowState = FormWindowState.Minimized
        Me.Visible = False
        Me.Hide()

        notify.Icon = New Icon("C:\Monitor\Icone.ico")
        notify.Visible = True
        notify.Text = "Monitor ACS"
        notify.ContextMenuStrip = CriarMenu()

        With DvVencimentos
            .Columns.Clear()
            .Columns.Add("Loja", "Loja")
            .Columns.Add("CNPJ", "CNPJ")
            .Columns.Add("DataInstalacao", "Data de Instalação")
            .Columns.Add("DataVencimento", "Data de Vencimento")
            .Columns.Add("Senha", "Senha")
        End With

        CarregarDadosNaTabela()

        VerificarVencimentos()

        Await Task.Delay(15000)
    End Sub

    Private Sub CarregarDadosNaTabela()
        Dim caminhoPlanilha As String = "C:\Monitor\Vencimento.xlsx"

        If Not File.Exists(caminhoPlanilha) Then
            MostrarNotificacao("Erro", "Arquivo vencimentos.xlsx não encontrado.")
            Return
        End If

        Try
            Using wb = New XLWorkbook(caminhoPlanilha)
                Dim ws = wb.Worksheet(1)

                DvVencimentos.Rows.Clear()

                For Each linha In ws.RangeUsed().RowsUsed().Skip(1)
                    Dim loja = linha.Cell(1).GetValue(Of String)()
                    Dim cnpj = linha.Cell(2).GetValue(Of String)()
                    Dim dataInstalacao = linha.Cell(3).GetFormattedString()
                    Dim dataVencimento = linha.Cell(4).GetFormattedString()
                    Dim senha = linha.Cell(5).GetFormattedString()

                    DvVencimentos.Rows.Add(loja, cnpj, dataInstalacao, dataVencimento, senha)
                Next
            End Using
        Catch ex As Exception
            MostrarNotificacao("Erro", "Falha ao carregar dados: " & ex.Message)
        End Try
    End Sub

    Private Sub VerificarVencimentos()
        Dim caminhoPlanilha As String = "C:\Monitor\Vencimento.xlsx"

        If Not File.Exists(caminhoPlanilha) Then
            MostrarNotificacao("Erro", "Arquivo vencimentos.xlsx não encontrado.")
            Return
        End If

        Dim hoje As Date = Date.Today
        Dim lojasAviso As New List(Of String)

        Using wb = New XLWorkbook(caminhoPlanilha)
            Dim ws = wb.Worksheet(1)

            For Each linha In ws.RangeUsed().RowsUsed().Skip(1)
                Dim loja = linha.Cell(1).GetValue(Of String)()
                Dim dataVenc As Date

                If Date.TryParse(linha.Cell(4).GetValue(Of String)(), dataVenc) Then
                    Dim dias = (dataVenc - hoje).Days
                    If dias <= 10 AndAlso dias >= 0 Then
                        lojasAviso.Add($"{loja} vence em {dias} dias ({dataVenc:dd/MM/yyyy})")
                    End If
                End If
            Next
        End Using

        If lojasAviso.Any() Then
            Dim msg = "Lojas com vencimento próximo:" & vbCrLf & String.Join(vbCrLf, lojasAviso)
            MostrarNotificacao("Atenção", msg)
        End If
    End Sub


    Private Sub MostrarNotificacao(titulo As String, mensagem As String)
        notify.BalloonTipTitle = titulo
        notify.BalloonTipText = mensagem
        notify.BalloonTipIcon = ToolTipIcon.Warning
        notify.ShowBalloonTip(10000)
    End Sub

    Private Sub notify_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles notify.MouseDoubleClick
        Me.Show()
        Me.WindowState = FormWindowState.Normal
        Me.ShowInTaskbar = True
    End Sub

    Private Function CriarMenu() As ContextMenuStrip
        Dim menu As New ContextMenuStrip()
        menu.Items.Add("Abrir", Nothing, AddressOf AbrirJanela)
        menu.Items.Add("Sair", Nothing, AddressOf FecharPrograma)
        Return menu
    End Function

    Private Sub AbrirJanela(sender As Object, e As EventArgs)
        Me.Show()
        Me.WindowState = FormWindowState.Normal
        Me.ShowInTaskbar = True
    End Sub

    Private Sub FecharPrograma(sender As Object, e As EventArgs)
        notify.Visible = False
        Application.Exit()
    End Sub

    Private Sub SalvarDadosNaPlanilha()
        Dim caminhoPlanilha As String = "C:\Monitor\Vencimento.xlsx"

        Try
            Dim wb = New XLWorkbook()
            Dim ws = wb.Worksheets.Add("Plan1")

            ws.Cell(1, 1).Value = "Loja"
            ws.Cell(1, 2).Value = "CNPJ"
            ws.Cell(1, 3).Value = "DataInstalacao"
            ws.Cell(1, 4).Value = "DataVencimento"
            ws.Cell(1, 5).Value = "Senha"

            Dim linhaExcel As Integer = 2

            For Each row As DataGridViewRow In DvVencimentos.Rows
                If row.IsNewRow Then Continue For

                ws.Cell(linhaExcel, 1).Value = row.Cells("Loja").Value?.ToString()
                ws.Cell(linhaExcel, 2).Value = row.Cells("CNPJ").Value?.ToString()
                ws.Cell(linhaExcel, 3).Value = row.Cells("DataInstalacao").Value?.ToString()
                ws.Cell(linhaExcel, 4).Value = row.Cells("DataVencimento").Value?.ToString()
                ws.Cell(linhaExcel, 5).Value = row.Cells("Senha").Value?.ToString()
                linhaExcel += 1
            Next

            wb.SaveAs(caminhoPlanilha)
            MostrarNotificacao("Sucesso", "Planilha atualizada com sucesso.")
        Catch ex As Exception
            MostrarNotificacao("Erro", "Falha ao salvar a planilha: " & ex.Message)
        End Try
    End Sub

    Private Sub FrmMonitorAcs_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        SalvarDadosNaPlanilha()
    End Sub


End Class
