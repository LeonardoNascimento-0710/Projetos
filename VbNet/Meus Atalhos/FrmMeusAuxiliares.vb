Imports System.Diagnostics
Imports System.IO
Imports ClosedXML.Excel
Imports FirebirdSql.Data.FirebirdClient
Imports System.Data


Public Class FrmMeusAuxiliares

    Private Sub FrmMeusAuxiliares_Load(sender As Object, e As EventArgs) Handles Me.Load


        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.MaximizeBox = False

    End Sub

#Region "Downloads"
    Private Sub BtnBaixar_Click(sender As Object, e As EventArgs) Handles BtnDowloadVideos.Click

        Dim link = InputBox("Cole o link da playlist do YouTube:", "Baixar Playlist")

        If link.Trim = "" Then
            MessageBox.Show("Nenhum link informado!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim pastaDestino As String = ""

        Using fbd As New FolderBrowserDialog()
            fbd.Description = "Escolha a pasta onde os vídeos serão salvos"

            If fbd.ShowDialog() = DialogResult.OK Then
                pastaDestino = fbd.SelectedPath
            Else
                MessageBox.Show("Nenhuma pasta selecionada!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If
        End Using

        Dim ytDlpPath = "C:\Users\Leo_f\OneDrive\Área de Trabalho\GitHub\Projetos\Library\yt-dlp.exe"

        Dim argumentos =
        $"-f ""bestvideo+bestaudio/best"" --yes-playlist --ignore-errors -o ""{pastaDestino}\%(playlist_index)03d - %(title)s.%(ext)s"" ""{link}"""

        Dim proc As New Process()
        proc.StartInfo.FileName = ytDlpPath
        proc.StartInfo.Arguments = argumentos
        proc.StartInfo.UseShellExecute = True
        proc.Start()

        MessageBox.Show("Download iniciado! Os vídeos serão salvos em:" & vbCrLf & pastaDestino,
                "OK", MessageBoxButtons.OK, MessageBoxIcon.Information)

    End Sub

    Private Sub BtnConverter_Click(sender As Object, e As EventArgs) Handles BtnConversor.Click
        Dim pasta As String = ""

        Using fbd As New FolderBrowserDialog()
            fbd.Description = "Selecione a pasta que contém os arquivos MP4"

            If fbd.ShowDialog() = DialogResult.OK Then
                pasta = fbd.SelectedPath
            Else
                Exit Sub
            End If
        End Using

        Dim arquivos = Directory.GetFiles(pasta, "*.MP4")
        If arquivos.Length = 0 Then
            MessageBox.Show("Nenhum arquivo .webm encontrado na pasta!", "Aviso",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim sucesso As Integer = 0
        Dim falha As Integer = 0

        For Each caminhoVideo As String In arquivos
            If ConverterFfmpeg(caminhoVideo) Then
                sucesso += 1
            Else
                falha += 1
            End If
        Next

        MessageBox.Show($"Áudios extraídos: {sucesso}{Environment.NewLine}Falhas: {falha}",
                        "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information)

    End Sub

    Private Function ConverterFfmpeg(caminhoVideo As String) As Boolean
        Try
            Dim saidaMp3 As String = Path.ChangeExtension(caminhoVideo, ".mp3")

            Dim ffmpegPath As String = "C:\Users\Leo_f\OneDrive\Área de Trabalho\GitHub\Projetos\Library\ffmpeg.exe"

            Dim ffmpeg As New Process()
            ffmpeg.StartInfo.FileName = ffmpegPath
            ffmpeg.StartInfo.Arguments = $"-i ""{caminhoVideo}"" -vn -ab 192k -ar 44100 -y ""{saidaMp3}"""
            ffmpeg.StartInfo.CreateNoWindow = True
            ffmpeg.StartInfo.UseShellExecute = False
            ffmpeg.StartInfo.RedirectStandardError = True
            ffmpeg.StartInfo.RedirectStandardOutput = True

            ffmpeg.Start()
            ffmpeg.WaitForExit()

            Return File.Exists(saidaMp3)

        Catch ex As Exception
            Return False
        End Try
    End Function

#End Region

#Region "Atalhos"
    Private Sub BrnBancoHoras_Click(sender As Object, e As EventArgs) Handles BrnBancoHoras.Click

        Dim frm As New FrmBancoHoras
        frm.Show()

    End Sub

    Private Sub BtnConversorFilizola_Click(sender As Object, e As EventArgs) Handles BtnConversorFilizola.Click
        Dim frm As New FrmConversorBalancaFilizola
        frm.Show()
    End Sub
    Private Sub BtnFdbforXlsx_Click(sender As Object, e As EventArgs) Handles BtnFdbforXlsx.Click

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
