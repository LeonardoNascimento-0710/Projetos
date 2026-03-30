Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions


Public Class FrmConversorBalancaFilizola
    Private Sub FrmConversorBalancaFilizola_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

#Region "Botões"
    Private Sub BtnLimparArquivo_Click(sender As Object, e As EventArgs) Handles BtnLimparArquivo.Click

        Dim openFile As New OpenFileDialog()
        openFile.Filter = "Arquivos de texto (*.txt)|*.txt|Todos (*.*)|*.*"

        If openFile.ShowDialog() <> DialogResult.OK Then Exit Sub

        Dim saveFile As New SaveFileDialog()
        saveFile.Filter = "Arquivos de texto (*.txt)|*.txt"
        saveFile.FileName = "arquivo_limpo.txt"

        If saveFile.ShowDialog() <> DialogResult.OK Then Exit Sub

        Try
            Dim content As String = File.ReadAllText(openFile.FileName, Encoding.Default)

            Dim blocks = content.Split("@"c)
            Dim cleanBlocks As New List(Of String)

            For Each block In blocks
                Dim bloco = block.Trim()

                If String.IsNullOrWhiteSpace(bloco) Then Continue For

                Dim linhas = Regex.Split(bloco, "\r\n|\r|\n") _
                                  .Select(Function(l) l.Trim()) _
                                  .Where(Function(l) Not String.IsNullOrWhiteSpace(l)) _
                                  .ToList()

                If linhas.Count <= 1 Then Continue For

                Dim primeiraLinha = linhas(0)

                If primeiraLinha.Contains("BALANCA") Then
                    Try
                        Dim inicio = primeiraLinha.IndexOf("BALANCA") + "BALANCA".Length
                        Dim resto = primeiraLinha.Substring(inicio).Trim()

                        If resto.Length >= 6 Then
                            Dim codigo = resto.Substring(0, 6)

                            If codigo.All(AddressOf Char.IsDigit) Then
                                cleanBlocks.Add(bloco & vbCrLf & "@")
                            End If
                        End If
                    Catch
                    End Try
                End If
            Next

            File.WriteAllText(saveFile.FileName, String.Join(vbCrLf, cleanBlocks), Encoding.UTF8)

            MessageBox.Show($"Sucesso! {cleanBlocks.Count} blocos encontrados.")

        Catch ex As Exception
            MessageBox.Show("Erro: " & ex.Message)
        End Try

    End Sub

    Private Sub BtnGerarUpdate_Click(sender As Object, e As EventArgs) Handles BtnGerarUpdate.Click

        Dim openFile As New OpenFileDialog()
        openFile.Filter = "Arquivos de texto (*.txt)|*.txt|Todos (*.*)|*.*"

        If openFile.ShowDialog() <> DialogResult.OK Then Exit Sub

        Dim folderDialog As New FolderBrowserDialog()

        If folderDialog.ShowDialog() <> DialogResult.OK Then Exit Sub

        Dim limitePorArquivo As Integer = 200
        Dim updates As New List(Of String)

        Try
            Dim conteudo As String = File.ReadAllText(openFile.FileName, Encoding.Default)

            Dim blocos = conteudo.Split("@"c)

            For Each bloco In blocos

                If Not bloco.Contains("BALANCA") Then Continue For

                Dim match = Regex.Match(bloco, "BALANCA\s+(\d{12})")
                If Not match.Success Then Continue For

                Dim codigo As String = CInt(match.Groups(1).Value.Substring(0, 6)).ToString()

                Dim texto As String = bloco.Substring(match.Index + match.Length).Trim()

                Dim matchTexto = Regex.Match(texto, "'(.*?)'", RegexOptions.Singleline)
                If matchTexto.Success Then
                    texto = matchTexto.Groups(1).Value
                End If

                texto = texto.Trim()
                texto = texto.Replace("'", "''")
                texto = Regex.Replace(texto, "\r\n|\r|\n", " ")

                If String.IsNullOrWhiteSpace(texto) Then Continue For

                updates.Add($"UPDATE PRODUTOS SET WBALRECEITA = '{texto}' WHERE WCODIGOPRINCIPAL = '{codigo}';")

            Next

            Dim totalArquivos As Integer = Math.Ceiling(updates.Count / limitePorArquivo)

            For i As Integer = 0 To totalArquivos - 1
                Dim inicio = i * limitePorArquivo
                Dim fim = Math.Min(inicio + limitePorArquivo, updates.Count)

                Dim nomeArquivo = Path.Combine(folderDialog.SelectedPath, $"updates_{i + 1}.txt")

                Using writer As New StreamWriter(nomeArquivo, False, Encoding.Default)
                    For j As Integer = inicio To fim - 1
                        writer.WriteLine(updates(j))
                    Next

                    writer.WriteLine()
                    writer.WriteLine("COMMIT;")
                End Using
            Next

            MessageBox.Show($"{updates.Count} updates divididos em {totalArquivos} arquivos!")

        Catch ex As Exception
            MessageBox.Show("Erro: " & ex.Message)
        End Try

    End Sub

    Private Sub BtnRetornar_Click(sender As Object, e As EventArgs) Handles BtnRetornar.Click
        Me.Dispose()
    End Sub

    Private Sub Panel2_Paint(sender As Object, e As PaintEventArgs) Handles Panel2.Paint

    End Sub

#End Region

End Class