Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.Text.RegularExpressions
Imports Org.BouncyCastle.Asn1.Cmp
Public Class FrmDownloadsConversor

    Private WithEvents processo As Process
    Private CancelarDownload As Boolean = False

#Region "Botões e containers"

    Private Sub BtnSelecionarPlaylist_Click(sender As Object, e As EventArgs) Handles BtnSelecionarPlaylist.Click
        BaixarVideos()
    End Sub

    Private Sub BtnRetornar_Click(sender As Object, e As EventArgs) Handles BtnRetornar.Click
        Me.Hide()
    End Sub

    Private Sub BtnConversor_Click(sender As Object, e As EventArgs) Handles BtnConversor.Click
        ConverterVideos()
    End Sub

    Private Sub BtnCancelar_Click(sender As Object, e As EventArgs) Handles BtnCancelar.Click

        CancelarDownload = True

        BtnCancelar.Enabled = False
        LblStatus.Text = "Cancelando download..."

        Try

            If processo IsNot Nothing AndAlso Not processo.HasExited Then

                processo.Kill(True)

            End If

        Catch ex As Exception

            Debug.WriteLine(ex.Message)

        End Try

    End Sub

#End Region

#Region "Funções e procedimentos"

    Private Sub BaixarVideos()

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

        Dim ytDlpPath As String = "C:\Users\Administrador\OneDrive\Desktop\Projetos\Projetos\Library\yt-dlp.exe"

        Dim argumentos As String =
        $"-f ""bestvideo+bestaudio/best"" " &
        $"--yes-playlist " &
        $"--ignore-errors " &
        $"--newline " &
        $"-o ""{pastaDestino}\%(playlist_index)03d - %(title)s.%(ext)s"" " &
        $"""{link}"""

        CancelarDownload = False

        BtnCancelar.Visible = True
        BtnCancelar.Enabled = True

        LblStatus.Text = "Baixando... (Clique em Cancelar para abortar)"

        processo = New Process()

        With processo.StartInfo

            .FileName = ytDlpPath
            .Arguments = argumentos
            .UseShellExecute = False
            .RedirectStandardOutput = True
            .RedirectStandardError = True
            .CreateNoWindow = True
            .StandardOutputEncoding = System.Text.Encoding.UTF8
            .StandardErrorEncoding = System.Text.Encoding.UTF8

        End With

        AddHandler processo.OutputDataReceived, AddressOf LerSaida
        AddHandler processo.ErrorDataReceived, AddressOf LerSaida

        processo.Start()

        processo.BeginOutputReadLine()
        processo.BeginErrorReadLine()

        Task.Run(Sub()

                     If CancelarDownload Then Return

                     processo.WaitForExit()

                     Me.BeginInvoke(Sub()

                                        BtnCancelar.Visible = False
                                        BtnCancelar.Enabled = False

                                        If CancelarDownload Then

                                            LblStatus.Text = "Download cancelado."

                                        Else

                                            PbDownload.Value = 100
                                            LblStatus.Text = "Concluído"

                                        End If

                                    End Sub)

                 End Sub)

        MessageBox.Show(
        "Download iniciado!" &
        Environment.NewLine &
        Environment.NewLine &
        "Caso queira interromper o download, clique no botão CANCELAR.",
        "Download",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information)

    End Sub

    Private Sub LerSaida(sender As Object, e As DataReceivedEventArgs)

        If CancelarDownload Then Exit Sub

        If String.IsNullOrWhiteSpace(e.Data) Then Exit Sub

        Debug.WriteLine(e.Data)

        Me.BeginInvoke(Sub()
                           TxtLog.AppendText(e.Data & Environment.NewLine)
                           TxtLog.ScrollToCaret()
                       End Sub)

        AtualizarProgresso(e.Data)

    End Sub

    Private Sub AtualizarProgresso(linha As String)

        If linha.Contains("Downloading item") Then

            Dim m = System.Text.RegularExpressions.Regex.Match(
            linha,
            "Downloading item (\d+) of (\d+)")

            If m.Success Then

                Me.BeginInvoke(Sub()

                                   LblVideo.Text =
                                   $"Vídeo {m.Groups(1).Value} de {m.Groups(2).Value}"

                               End Sub)

            End If

            Exit Sub

        End If

        If linha.Contains("Destination:") Then

            Dim nome = linha.Substring(linha.IndexOf("Destination:") + 12).Trim()

            Me.BeginInvoke(Sub()

                               Dim nomeArquivo As String = IO.Path.GetFileNameWithoutExtension(nome)

                               If nomeArquivo.Length > 70 Then
                                   nomeArquivo = nomeArquivo.Substring(0, 70) & "..."
                               End If

                               LblNome.Text = nomeArquivo

                           End Sub)

            Exit Sub

        End If

        If linha.Contains("has already been downloaded") Then

            Dim nome = linha.Replace("[download]", "").
                        Replace("has already been downloaded", "").
                        Trim()

            Me.BeginInvoke(Sub()

                               Dim nomeArquivo As String = IO.Path.GetFileNameWithoutExtension(nome)

                               If nomeArquivo.Length > 70 Then
                                   nomeArquivo = nomeArquivo.Substring(0, 70) & "..."
                               End If

                               LblNome.Text = nomeArquivo

                               PbDownload.Value = 100

                               LblStatus.Text = "Concluído"

                           End Sub)

            Exit Sub

        End If

        Dim r = System.Text.RegularExpressions.Regex.Match(
        linha,
        "(\d+(?:\.\d+)?)%\s+of\s+(.+?)\s+at\s+(.+?)\s+ETA\s+([0-9:]+)")

        If r.Success Then

            Dim pct As Integer =
            CInt(Double.Parse(
                r.Groups(1).Value,
                Globalization.CultureInfo.InvariantCulture))

            Me.BeginInvoke(Sub()

                               PbDownload.Value = Math.Min(100, pct)

                               LblStatus.Text =
                               r.Groups(1).Value & "%"

                               LblVelocidade.Text =
                               "Velocidade: " & r.Groups(3).Value

                               LblETA.Text =
                               "ETA: " & r.Groups(4).Value

                           End Sub)

        End If

    End Sub

    Private Function FormatarVelocidade(valor As String) As String

        Dim bytes As Double

        If Not Double.TryParse(valor,
                               NumberStyles.Any,
                               CultureInfo.InvariantCulture,
                               bytes) Then

            Return "-"

        End If

        Return (bytes / 1024 / 1024).ToString("0.00") & " MB/s"

    End Function

    Private Sub ConverterVideos()

        Dim pasta As String = ""

        Using fbd As New FolderBrowserDialog()

            fbd.Description = "Selecione a pasta que contém os vídeos"

            If fbd.ShowDialog() <> DialogResult.OK Then Exit Sub

            pasta = fbd.SelectedPath

        End Using

        Dim extensoes() As String = {
        "*.mp4",
        "*.mkv",
        "*.avi",
        "*.mov",
        "*.webm",
        "*.flv"
    }

        Dim arquivos As New List(Of String)

        For Each ext As String In extensoes
            arquivos.AddRange(Directory.GetFiles(pasta, ext, SearchOption.TopDirectoryOnly))
        Next

        arquivos = arquivos.OrderBy(Function(x) x).ToList()

        If arquivos.Count = 0 Then

            MessageBox.Show("Nenhum vídeo encontrado na pasta!",
                        "Aviso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning)
            Exit Sub

        End If

        PbDownload.Minimum = 0
        PbDownload.Maximum = arquivos.Count
        PbDownload.Value = 0

        LblStatus.Text = "Preparando..."
        LblVelocidade.Text = ""
        LblETA.Text = ""
        LblVideo.Text = "0 / " & arquivos.Count
        LblNome.Text = ""

        TxtLog.Clear()

        Dim sucesso As Integer = 0
        Dim falha As Integer = 0
        Dim contador As Integer = 0

        For Each caminhoVideo As String In arquivos

            contador += 1

            LblVideo.Text = contador & " / " & arquivos.Count
            LblNome.Text = Path.GetFileName(caminhoVideo)
            LblStatus.Text = "Convertendo..."

            TxtLog.AppendText("Convertendo: " &
                          Path.GetFileName(caminhoVideo) &
                          Environment.NewLine)

            Application.DoEvents()

            If ConverterFfmpeg(caminhoVideo) Then

                sucesso += 1

                TxtLog.AppendText("✓ Concluído" &
                              Environment.NewLine &
                              Environment.NewLine)

            Else

                falha += 1

                TxtLog.AppendText("✗ Erro" &
                              Environment.NewLine &
                              Environment.NewLine)

            End If

            PbDownload.Value = contador

            Application.DoEvents()

        Next

        LblStatus.Text = "Finalizado"
        LblVideo.Text = arquivos.Count & " / " & arquivos.Count
        LblNome.Text = ""

        MessageBox.Show(
        "Conversão concluída!" &
        Environment.NewLine &
        Environment.NewLine &
        "Sucesso: " & sucesso &
        Environment.NewLine &
        "Falhas: " & falha,
        "Resultado",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information)

    End Sub

    Private Function ConverterFfmpeg(caminhoVideo As String) As Boolean

        Try

            Dim ffmpeg As String = "C:\Users\Administrador\OneDrive\Desktop\Projetos\Projetos\Library\ffmpeg.exe"

            If Not File.Exists(ffmpeg) Then

                MessageBox.Show("ffmpeg.exe não encontrado!")

                Return False

            End If

            Dim pastaDestino As String =
            Path.Combine(Path.GetDirectoryName(caminhoVideo), "MP3")

            If Not Directory.Exists(pastaDestino) Then
                Directory.CreateDirectory(pastaDestino)
            End If

            Dim arquivoSaida As String =
            Path.Combine(pastaDestino,
            Path.GetFileNameWithoutExtension(caminhoVideo) & ".mp3")

            Dim psi As New ProcessStartInfo

            psi.FileName = ffmpeg
            psi.Arguments = "-y -i """ &
                        caminhoVideo &
                        """ -vn -ar 44100 -ac 2 -b:a 192k """ &
                        arquivoSaida &
                        """"

            psi.CreateNoWindow = True
            psi.UseShellExecute = False
            psi.RedirectStandardError = True
            psi.RedirectStandardOutput = True

            Using p As Process = Process.Start(psi)

                While Not p.StandardError.EndOfStream

                    Dim linha = p.StandardError.ReadLine()

                    If linha.Contains("time=") Then

                        Application.DoEvents()

                    End If

                End While

                p.WaitForExit()

                Return p.ExitCode = 0

            End Using

        Catch ex As Exception

            TxtLog.AppendText("Erro: " & ex.Message & Environment.NewLine)

            Return False

        End Try

    End Function

#End Region

End Class