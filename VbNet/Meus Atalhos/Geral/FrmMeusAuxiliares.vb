Imports System.Data
Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports ClosedXML.Excel
Imports FirebirdSql.Data.FirebirdClient

Public Class FrmMeusAuxiliares

    Private Sub FrmMeusAuxiliares_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub

#Region "Botões e containers"

    Private Sub BrnBancoHoras_Click(sender As Object, e As EventArgs) Handles BrnBancoHoras.Click

        Me.Hide()

        Using frm As New FrmBancoHoras
            frm.ShowDialog()
        End Using

        Me.Show()

    End Sub

    Private Sub BtnConversorFilizola_Click(sender As Object, e As EventArgs) Handles BtnConversorFilizola.Click

        Me.Hide()

        Using frm As New FrmConversorBalancaFilizola
            frm.ShowDialog()
        End Using

        Me.Show()

    End Sub

    Private Sub BtnBaixar_Click(sender As Object, e As EventArgs) Handles BtnDowloadVideos.Click

        Me.Hide()

        Using frm As New FrmDownloadsConversor()
            frm.ShowDialog()
        End Using

        Me.Show()

    End Sub

    Private Sub BtnFdbforXlsx_Click(sender As Object, e As EventArgs) Handles BtnFdbforXlsx.Click
        FdbforXlsx()
    End Sub

    Private Sub BtnConversorToledo_Click(sender As Object, e As EventArgs) Handles BtnConversorToledo.Click

        Me.Hide()

        Using frm As New FrmConversorBalancaToledo
            frm.ShowDialog()
        End Using

        Me.Show()

    End Sub

#End Region

#Region "Downloads"

#End Region

#Region "Atalhos"

    Private Sub FdbforXlsx()

        Dim caminhoFdb As String = ""
        Dim caminhoXlsx As String = ""


        Using ofd As New OpenFileDialog()
            ofd.Filter = "Banco Firebird (*.fdb)|*.fdb"
            ofd.Title = "Selecione o banco Firebird"

            If ofd.ShowDialog() = DialogResult.OK Then
                caminhoFdb = ofd.FileName
            Else
                Exit Sub
            End If
        End Using

        Using sfd As New SaveFileDialog()
            sfd.Filter = "Arquivo Excel (*.xlsx)|*.xlsx"
            sfd.Title = "Salvar arquivo Excel"
            sfd.FileName = "exportado.xlsx"

            If sfd.ShowDialog() = DialogResult.OK Then
                caminhoXlsx = sfd.FileName
            Else
                Exit Sub
            End If
        End Using

        ExportarFdbCompleto(caminhoFdb, caminhoXlsx)

    End Sub

    Private Sub ExportarFdbCompleto(caminhoFdb As String, caminhoXlsx As String)

        Dim connectionString As String =
        "User=SYSDBA;" &
        "Password=masterkey;" &
        "Database=" & caminhoFdb & ";" &
        "DataSource=localhost;" &
        "Port=3050;" &
        "Dialect=3;"

        Try
            Cursor = Cursors.WaitCursor

            Using con As New FbConnection(connectionString)
                con.Open()

                Dim tabelas As New List(Of String)

                Dim queryTabelas As String =
                "SELECT RDB$RELATION_NAME " &
                "FROM RDB$RELATIONS " &
                "WHERE RDB$SYSTEM_FLAG = 0 AND RDB$VIEW_BLR IS NULL"

                Using cmd As New FbCommand(queryTabelas, con)
                    Using reader As FbDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            tabelas.Add(reader(0).ToString().Trim())
                        End While
                    End Using
                End Using

                Using wb As New XLWorkbook()

                    For Each nomeTabela In tabelas

                        Dim dt As New DataTable()

                        Dim queryDados As String = "SELECT * FROM " & nomeTabela

                        Using cmdDados As New FbCommand(queryDados, con)
                            Using da As New FbDataAdapter(cmdDados)
                                da.Fill(dt)
                            End Using
                        End Using

                        If dt.Rows.Count > 0 Then

                            Dim nomeAba As String = nomeTabela
                            If nomeAba.Length > 31 Then
                                nomeAba = nomeAba.Substring(0, 31)
                            End If

                            wb.Worksheets.Add(dt, nomeAba)

                        End If

                    Next

                    wb.SaveAs(caminhoXlsx)

                End Using

            End Using

            Cursor = Cursors.Default
            MessageBox.Show("Exportação concluída com sucesso!", "Sucesso")

        Catch ex As Exception
            Cursor = Cursors.Default
            MessageBox.Show("Erro: " & ex.Message, "Erro")
        End Try

    End Sub


#End Region

End Class
