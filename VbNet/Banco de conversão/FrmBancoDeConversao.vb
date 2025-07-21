Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Windows.Forms


Public Class FrmBancoDeConversao
    Private Sub btnSelectFile_Click(sender As Object, e As EventArgs) Handles BtnFileSelect.Click
        ' Abrir o explorador de arquivos para selecionar um CSV
        Using openFileDialog As New OpenFileDialog()
            openFileDialog.Filter = "Arquivos CSV|*.csv|Arquivos Excel|*.xlsx;*.xls"
            openFileDialog.Title = "Selecione um arquivo CSV ou Excel"

            If openFileDialog.ShowDialog() = DialogResult.OK Then
                Dim filePath As String = openFileDialog.FileName
                Dim fileExtension As String = Path.GetExtension(filePath).ToLower()

                If fileExtension = ".csv" Then
                    LoadCsvToDataGridView(filePath)
                ElseIf fileExtension = ".xlsx" OrElse fileExtension = ".xls" Then
                    LoadExcelToDataGridView(filePath)
                Else
                    MessageBox.Show("Formato de arquivo não suportado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End If
        End Using
    End Sub

    Private Sub LoadCsvToDataGridView(filePath As String)
        Dim dt As New DataTable()

        ' Ler o arquivo CSV
        Using reader As New StreamReader(filePath)
            Dim headers As String() = reader.ReadLine().Split(","c)
            For Each header As String In headers
                dt.Columns.Add(header.Trim())
            Next

            While Not reader.EndOfStream
                Dim values As String() = reader.ReadLine().Split(","c)
                dt.Rows.Add(values)
            End While
        End Using

        ' Exibir no DataGridView
        DgBanco.DataSource = dt
    End Sub

    Private Sub LoadExcelToDataGridView(filePath As String)
        Dim dt As New DataTable()
        Dim excelConnectionString As String = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties='Excel 12.0 Xml;HDR=YES;'"

        ' Conectar ao Excel
        Using excelConnection As New OleDb.OleDbConnection(excelConnectionString)
            excelConnection.Open()

            ' Pegar o nome da primeira planilha
            Dim sheetName As String = excelConnection.GetSchema("Tables").Rows(0)("TABLE_NAME").ToString()

            ' Carregar os dados da planilha
            Dim query As String = $"SELECT * FROM [{sheetName}]"
            Dim adapter As New OleDb.OleDbDataAdapter(query, excelConnection)
            adapter.Fill(dt)
        End Using

        ' Exibir no DataGridView
        DgBanco.DataSource = dt
    End Sub
End Class

