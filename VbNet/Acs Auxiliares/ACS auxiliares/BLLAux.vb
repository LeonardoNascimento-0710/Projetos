Imports System.Drawing.Printing
Imports System.IO
Imports System.Text.RegularExpressions

Public Class BllAux

#Region "funções"

    Sub ExecutarComandoCMD(comando As String)
        Try

            Dim processo As New Process()


            processo.StartInfo.FileName = "cmd.exe"
            processo.StartInfo.Arguments = "/c " & comando
            processo.StartInfo.RedirectStandardOutput = True
            processo.StartInfo.UseShellExecute = False
            processo.StartInfo.CreateNoWindow = True

            processo.Start()

            Dim output As String = processo.StandardOutput.ReadToEnd()
            processo.WaitForExit()

            Console.WriteLine(output)
            MessageBox.Show(output)

        Catch ex As Exception

            MessageBox.Show("Erro ao executar o comando: " & ex.Message)
        End Try
    End Sub

    Function VerificaImpressora(impressora As String)

        Dim printers As PrinterSettings.StringCollection = PrinterSettings.InstalledPrinters

        Dim printerFound As Boolean = False
        For Each printer As String In printers
            If printer.Contains(impressora) Then
                printerFound = True
                Exit For
            End If
        Next

        If printerFound Then
            MessageBox.Show("Impressora 'Argox' encontrada.")
            Return True
        Else
            MessageBox.Show("Impressora 'Argox' não encontrada.")
            Return False
        End If

    End Function

    Sub BaixarArquivoFTP(arquivo As String, ftp As String)
        Dim folderDialog As New FolderBrowserDialog()
        folderDialog.Description = "Selecione a pasta onde deseja salvar o arquivo:"
        folderDialog.ShowNewFolderButton = True

        If ftp = "" Then
            MessageBox.Show("Preencha o Servidor FTP antes de continuar!")
            Exit Sub
        End If

        If folderDialog.ShowDialog() = DialogResult.OK Then

            Dim pastaDestino As String = folderDialog.SelectedPath
            Dim ftpScriptPath As String = Path.Combine(pastaDestino, "ftp_script.txt")

            Dim script As String = "open " & ftp & vbCrLf &
                                       "administrador" & vbCrLf &
                                       "acs10603" & vbCrLf &
                                       "lcd " & pastaDestino & vbCrLf &
                                       "get " & arquivo & vbCrLf &
                                       "bye"

            File.WriteAllText(ftpScriptPath, script)

            Dim processo As New Process()
            processo.StartInfo.FileName = "cmd.exe"
            processo.StartInfo.Arguments = "/C ftp -s:" & ftpScriptPath
            processo.StartInfo.UseShellExecute = False
            processo.StartInfo.RedirectStandardOutput = True
            processo.StartInfo.RedirectStandardError = True
            processo.StartInfo.CreateNoWindow = True
            Try

                processo.Start()

                Dim output As String = processo.StandardOutput.ReadToEnd()
                Dim errorOutput As String = processo.StandardError.ReadToEnd()

                processo.WaitForExit()

                MessageBox.Show("Saída do FTP: " & vbCrLf & output)
                If errorOutput <> "" Then MessageBox.Show("Erro: " & errorOutput)

                Dim caminhoArquivo As String = Path.Combine(pastaDestino, arquivo)
                If File.Exists(caminhoArquivo) Then
                    MessageBox.Show("Download concluído! Arquivo salvo em: " & caminhoArquivo)
                Else
                    MessageBox.Show("Erro: O arquivo não foi encontrado na pasta destino.")
                End If

            Catch ex As Exception
                MessageBox.Show("Erro ao executar o comando: " & ex.Message)
            End Try

            Try
                If File.Exists(ftpScriptPath) Then File.Delete(ftpScriptPath)
            Catch ex As Exception
                MessageBox.Show("Erro ao excluir o arquivo de script: " & ex.Message)
            End Try
        Else
            MessageBox.Show("Seleção de pasta cancelada pelo usuário.")
        End If

    End Sub

    Sub GerarPlanilhas()

        Dim openFileDialog As New OpenFileDialog()
        openFileDialog.Multiselect = True
        openFileDialog.Filter = "Text Files (*.txt)|*.txt"

        If openFileDialog.ShowDialog() <> DialogResult.OK Then
            MessageBox.Show("Nenhum arquivo selecionado.")
            Exit Sub
        End If

        Dim keywords As String() = {"cnpj", "Raz.Soc", "PDV ID", "Ass.SH", "Senha PDV", "Produto", "pedido"}
        Dim dataList As New List(Of Dictionary(Of String, String))()

        For Each filePath In openFileDialog.FileNames
            Try
                Dim text As String = File.ReadAllText(filePath)
                Dim extractedData As New Dictionary(Of String, String)()
                For Each key In keywords
                    Dim match As Match

                    If key = "Senha PDV" Or key = "Senha" Or key = "Senha TEF" Then
                        match = Regex.Match(text, "(?i)(Senha\s*[:=\s]*\s*(?:PDV\s*=\s*|TEF\s*=\s*)?(.+))", RegexOptions.IgnoreCase)
                        If match.Success Then
                            extractedData(key) = match.Groups(2).Value.Trim()
                        Else
                            extractedData(key) = ""
                        End If
                    Else
                        match = Regex.Match(text, key & "\s*[:\-]?\s*(.+)", RegexOptions.IgnoreCase)
                        If match.Success Then
                            extractedData(key) = match.Groups(1).Value.Trim()
                        Else
                            extractedData(key) = ""
                        End If
                    End If
                Next

                dataList.Add(extractedData)
            Catch ex As Exception
                MessageBox.Show("Erro ao processar o arquivo: " & filePath & vbCrLf & ex.Message)
            End Try
        Next

        dataList = dataList.OrderBy(Function(d) d("pedido")).ToList()

        Dim saveFileDialog As New SaveFileDialog()
        saveFileDialog.Filter = "CSV files (*.csv)|*.csv"
        saveFileDialog.Title = "Salvar CSV"

        If saveFileDialog.ShowDialog() = DialogResult.OK Then
            Try
                Dim csvPath As String = saveFileDialog.FileName

                Using writer As New StreamWriter(csvPath)
                    writer.WriteLine(String.Join(";", keywords))

                    For Each extractedData In dataList
                        writer.WriteLine(String.Join(";", keywords.Select(Function(k) If(extractedData.ContainsKey(k), extractedData(k), ""))))
                    Next
                End Using
                MessageBox.Show("Arquivo salvo com sucesso em: " & csvPath)
            Catch ex As Exception
                MessageBox.Show("Erro ao salvar o arquivo CSV: " & ex.Message)
            End Try
        Else
            MessageBox.Show("Operação cancelada.")
        End If

    End Sub

#End Region

End Class