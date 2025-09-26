Imports ClosedXML.Excel
Imports System.Text
Imports System.IO

Public Class FrmConversorXlsx

    Private Sub btnCarregarExcel_Click(sender As Object, e As EventArgs) Handles BtnCarregarExcel.Click
        Dim ofd As New OpenFileDialog()
        ofd.Filter = "Excel Files|*.xlsx"
        If ofd.ShowDialog() = DialogResult.OK Then
            Dim dt As New DataTable()
            Using workbook = New XLWorkbook(ofd.FileName)
                Dim ws = workbook.Worksheet(1)
                Dim firstRow = True

                For Each row In ws.RowsUsed()
                    If firstRow Then

                        For Each cell In row.Cells()
                            dt.Columns.Add(cell.Value.ToString())
                        Next
                        firstRow = False
                    Else
                        Dim newRow = dt.NewRow()
                        For i = 0 To row.CellsUsed().Count() - 1
                            newRow(i) = row.Cell(i + 1).Value.ToString()
                        Next
                        dt.Rows.Add(newRow)
                    End If
                Next
            End Using

            DgvProdutos.DataSource = dt
        End If
    End Sub


    Private Sub BtnGerarSQL_Click(sender As Object, e As EventArgs) Handles BtnGerarSQL.Click
        If DgvProdutos.Rows.Count = 0 Then
            MessageBox.Show("Não há dados carregados.")
            Return
        End If

        Dim sb As New StringBuilder()

        For Each row As DataGridViewRow In DgvProdutos.Rows
            If Not row.IsNewRow Then
                Dim wid As String = row.Cells("WIDPRODUTO").Value.ToString().Replace("'", "''")
                Dim widNutricional As String = row.Cells("WIDNUTRICIONAL").Value.ToString().Replace("'", "''")

                ' Limpa quebras de linha e aspas simples dentro do texto
                Dim fornecedorHex As String = row.Cells("WBALFORNECEDOR_HEX").Value.ToString()
                fornecedorHex = fornecedorHex.Replace("'", "''").Replace(vbCrLf, " ").Replace(vbLf, " ")

                Dim receitaHex As String = row.Cells("WBALRECEITA_HEX").Value.ToString()
                receitaHex = receitaHex.Replace("'", "''").Replace(vbCrLf, " ").Replace(vbLf, " ")

                ' Gera UPDATE em uma linha só, incluindo WIDNUTRICIONAL
                sb.AppendLine($"UPDATE PRODUTOS SET WIDNUTRICIONAL = '{widNutricional}', WBALFORNECEDOR = CAST('{fornecedorHex}' AS BLOB SUB_TYPE 0), WBALRECEITA = CAST('{receitaHex}' AS BLOB SUB_TYPE 0) WHERE WIDPRODUTO = {wid};")
            End If
        Next

        Dim sfd As New SaveFileDialog()
        sfd.Filter = "Text Files|*.txt"
        sfd.FileName = "update_produtos.sql"
        If sfd.ShowDialog() = DialogResult.OK Then
            File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8)
            MessageBox.Show("Arquivo SQL gerado com sucesso! Lembre-se de rodar no IBExpert usando F9 (Execute Script).")
        End If
    End Sub



    Private Sub BtnScriptColeta_Click(sender As Object, e As EventArgs) Handles BtnScriptColeta.Click

        Dim sb As New StringBuilder()
        sb.AppendLine("SELECT")
        sb.AppendLine("    WIDPRODUTO,")
        sb.AppendLine("    WNOMEPRODUTO,")
        sb.AppendLine("    WIDNUTRICIONAL,")
        sb.AppendLine("    CAST(WBALFORNECEDOR AS VARCHAR(32000)) AS WBALFORNECEDOR_HEX,")
        sb.AppendLine("    CAST(WBALRECEITA AS VARCHAR(32000)) AS WBALRECEITA_HEX")
        sb.AppendLine("FROM PRODUTOS")
        sb.AppendLine("WHERE WBALRECEITA IS NOT NULL OR WBALFORNECEDOR IS NOT NULL;")

        Dim sfd As New SaveFileDialog()
        sfd.Filter = "Text Files|*.txt"
        sfd.FileName = "select_produtos.sql"
        If sfd.ShowDialog() = DialogResult.OK Then
            File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8)
            MessageBox.Show("Arquivo SQL (SELECT) gerado com sucesso!")
        End If

    End Sub

End Class
