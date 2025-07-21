Imports System.Reflection.Emit
Imports System.Diagnostics
Imports System.Drawing.Printing
Imports System.IO
Imports ClosedXML.Excel
Imports System.Text
Imports System.Net
Imports Excel = Microsoft.Office.Interop.Excel
Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Imports System.Data


Public Class FrmAux

    Dim obj As New BllAux
    Dim servidor As String = "SERVIDOR"
    Dim estacao As String = "ESTACAO"

#Region "Dados Iniciais"
    Private Sub FrmAux_Load(sender As Object, e As EventArgs) Handles Me.Load
        LblCabeçalho.Text = DateTime.Now.ToString("dddd',' dd 'de' MMMM 'de' yyyy",
                      New System.Globalization.CultureInfo("pt-BR"))
        SenhaPockets()
        Dim Servidor As String = "192.168.15.123"
        If VerificarIP(Servidor) Then
            RbServidor.Checked = True
            RbEstacao.Enabled = False
        Else
            RbServidor.Enabled = False
            RbEstacao.Checked = True
        End If

    End Sub

#End Region

#Region "Auxiliares"
    Private Sub BtnExcluirLpt1_Click(sender As Object, e As EventArgs) Handles BtnComandoExLpt1.Click

        InputBox("Copie o comando abaixo:", "Informação", "net use lpt1 /delete")

    End Sub

    Private Sub BtnAdcLpt1_Click(sender As Object, e As EventArgs) Handles BtnComandoAdcLpt1.Click

        InputBox("Copie o comando abaixo:", "Informação", "net use lpt1 \\localhost\argox /persistent:yes")

    End Sub

    Private Sub BtnCmdAdcLpt1_Click(sender As Object, e As EventArgs) Handles BtnCmdAdcLpt1.Click

        Dim impressora = obj.VerificaImpressora("Argox")
        If impressora = True Then
            obj.ExecutarComandoCMD("net use lpt1 \\localhost\argox /persistent:yes")
        Else
            MessageBox.Show("Comando não executado, impressora não localizada!")
        End If
    End Sub
    Private Sub BtnLanmamCmd_Click(sender As Object, e As EventArgs) Handles BtnLanmamCmd.Click
        MensagemCMD()
    End Sub
    Private Sub BtnLanmamComan_Click(sender As Object, e As EventArgs) Handles BtnLanmamComan.Click
        MessageBox.Show("Vá até Iniciar." & vbCrLf &
                      "Acesse a opção Pesquisar, digite a palavra gpedit.msc e pressione Enter." & vbCrLf &
                      "Acesse Configuração do Computador > Modelos Administrativos > Rede > Estação de trabalho do LANMAN." & vbCrLf &
                      "No painel direito, clique duas vezes em Habilitar logons de convidados não seguros." & vbCrLf &
                      "Clique em Habilitado e pressione Enter.",
                      "Instruções para Configuração", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub
    Private Sub BtnCmdExLpt1_Click(sender As Object, e As EventArgs) Handles BtnCmdExLpt1.Click
        obj.ExecutarComandoCMD("net use lpt1 /delete")
    End Sub

    Private Sub Button73_Click(sender As Object, e As EventArgs) Handles BtnCmdIpconfig.Click
        obj.ExecutarComandoCMD("Ipconfig")
    End Sub
    Private Sub BtnBalancaLink_Click(sender As Object, e As EventArgs) Handles BtnBalancaLink.Click
        Process.Start(New ProcessStartInfo With {
            .FileName = "https://drive.google.com/file/d/1qnd5UrAyyWuGfV_K60uIo5Ds5_OdgWTp/view?usp=drive_link",
            .UseShellExecute = True
        })
    End Sub
    Private Sub BtnDrivers_Click(sender As Object, e As EventArgs) Handles BtnDriversLinkAux.Click
        Process.Start(New ProcessStartInfo With {
            .FileName = "https://drive.google.com/file/d/1yQJgeGJMdH9bIOoUHSjNwWRNTl0-svDx/view?usp=drive_link",
                .UseShellExecute = True
        })
    End Sub
    Private Sub BtnDesaDefLink_Click(sender As Object, e As EventArgs) Handles BtnDesaDefLink.Click
        Process.Start(New ProcessStartInfo With {
            .FileName = "https://www.dropbox.com/scl/fi/mtglt3j931183a9hw26pe/DesativadorDefender.rar?rlkey=rn40y9egspxwqe2d19dqkixl4&st=hvf7yvof&dl=0",
            .UseShellExecute = True
               })
    End Sub
    Private Sub BtnComIpconfig_Click(sender As Object, e As EventArgs) Handles BtnComIpconfig.Click
        MensagemComando()
    End Sub
    Private Sub BtnOffice2016Ftp_Click(sender As Object, e As EventArgs) Handles BtnOffice2016Ftp.Click
        obj.BaixarArquivoFTP("Office2016.zip", TxtFtp.Text)
    End Sub
    Private Sub BtnOffice2019Ftp_Click(sender As Object, e As EventArgs) Handles BtnOffice2019Ftp.Click
        obj.BaixarArquivoFTP("Office2019.zip", TxtFtp.Text)
    End Sub
    Private Sub BtnBalancaFTP_Click(sender As Object, e As EventArgs) Handles BtnBalancaFTP.Click
        obj.BaixarArquivoFTP("TesteBalanca.zip", TxtFtp.Text)
    End Sub
    Private Sub BtnDriversFtpAux_Click(sender As Object, e As EventArgs) Handles BtnDriversFtpAux.Click
        obj.BaixarArquivoFTP("Drivers.zip", TxtFtp.Text)
    End Sub
    Private Sub BtnDesaDefFtp_Click(sender As Object, e As EventArgs) Handles BtnDesaDefFtp.Click
        obj.BaixarArquivoFTP("DesativadorDefender.rar", TxtFtp.Text)
    End Sub
    Private Sub BtnDriverKnupFtp_Click(sender As Object, e As EventArgs) Handles BtnDriverKnupFtp.Click
        obj.BaixarArquivoFTP("DriverSerialKNUP.zip", TxtFtp.Text)
    End Sub

    Private Sub BtnSiteOs_Click(sender As Object, e As EventArgs) Handles BtnSiteOs.Click
        Process.Start(New ProcessStartInfo With {
        .FileName = "https://acsautomacao.agoraos.com.br",
        .UseShellExecute = True
         })
    End Sub
    Private Sub BtnLocalOs_Click(sender As Object, e As EventArgs) Handles BtnLocalOs.Click

        If RbServidor.Checked = True Then

            Dim frm As New FrmOrdemDeServico
            frm.TipoMaquina = servidor
            frm.Show()
        Else

            Dim frm As New FrmOrdemDeServico
            frm.TipoMaquina = estacao
            frm.Show()
        End If

    End Sub

    Private Sub BtnAtuTxt_Click(sender As Object, e As EventArgs) Handles BtnAtuTxt.Click

        Dim dataAntiga As String = InputBox("Digite a data errada (ex: dd/mm/aa):", "Data Antiga")

        If String.IsNullOrEmpty(dataAntiga) Then
            MessageBox.Show("Data antiga não fornecida. A operação será cancelada.")
            Return
        End If

        Dim dataNova As String = InputBox("Digite a nova data (ex: dd/mm/aa):", "Nova Data")

        If String.IsNullOrEmpty(dataNova) Then
            MessageBox.Show("Data nova não fornecida. A operação será cancelada.")
            Return
        End If

        Dim openFileDialog As New OpenFileDialog()
        openFileDialog.Title = "Selecione os arquivos .txt"
        openFileDialog.Filter = "Arquivos de texto (*.txt)|*.txt"
        openFileDialog.Multiselect = True


        If openFileDialog.ShowDialog() = DialogResult.OK Then

            For Each arquivo As String In openFileDialog.FileNames

                Dim linhas As List(Of String) = File.ReadAllLines(arquivo).ToList()

                For i As Integer = 0 To linhas.Count - 1
                    If linhas(i).Contains(dataAntiga) Then
                        linhas(i) = linhas(i).Replace(dataAntiga, dataNova)
                    End If
                Next

                File.WriteAllLines(arquivo, linhas)

                Console.WriteLine("Data atualizada no arquivo: " & Path.GetFileName(arquivo))
            Next

            MessageBox.Show("Atualização concluída para todos os arquivos selecionados.")
            Console.WriteLine("Atualização concluída para todos os arquivos selecionados.")
        Else
            MessageBox.Show("Nenhum arquivo foi selecionado.")
            Console.WriteLine("Nenhum arquivo foi selecionado.")
        End If
    End Sub



#End Region

#Region "Mobility"
    Private Sub BtnManuaisMG_Click(sender As Object, e As EventArgs) Handles BtnManMobLink.Click
        Process.Start(New ProcessStartInfo With {
    .FileName = "https://www.dropbox.com/scl/fi/g59i6iaa1uup6to7mhmbw/Mobility.zip?rlkey=5l226kah3hafvmvp31fefue8p&st=rmr59l56&dl=0",
    .UseShellExecute = True
        })
    End Sub
    Private Sub BtnAtuMobility_Click(sender As Object, e As EventArgs) Handles BtnComandoAtuMob.Click

        InputBox("Copie o comando abaixo e cole no navegador:", "Informação", "54.207.107.184:1603")

    End Sub
    Private Sub BtnPlanLicMobLink_Click(sender As Object, e As EventArgs) Handles BtnPlanLicMobLink.Click
        Process.Start(New ProcessStartInfo With {
            .FileName = "https://docs.google.com/spreadsheets/d/11lZ5WYKRHrVAZvXx8R2P5KhfiiNUISd-EfUsmcmRVjg/edit?usp=sharing",
            .UseShellExecute = True
            })
    End Sub
    Private Sub BtnAtuCmdMob_Click(sender As Object, e As EventArgs) Handles BtnAtuCmdMob.Click
        MensagemCMD()
    End Sub
    Private Sub BtnManMobFtp_Click(sender As Object, e As EventArgs) Handles BtnManMobFtp.Click
        obj.BaixarArquivoFTP("ManuaisMobility.zip", TxtFtp.Text)
    End Sub
    Private Sub BtnPlanLicMobFtp_Click(sender As Object, e As EventArgs) Handles BtnPlanLicMobFtp.Click
        obj.BaixarArquivoFTP("LicençasMobility.zip", TxtFtp.Text)
    End Sub
    Private Sub BtnImportarTxtLicencas_Click(sender As Object, e As EventArgs) Handles BtnImportarTxtLicencas.Click
        obj.GerarPlanilhas()
    End Sub
#End Region

#Region "SDSuper"
    Private Sub BtnManuaisSD_Click(sender As Object, e As EventArgs) Handles BtnManSdLink.Click
        Process.Start(New ProcessStartInfo With {
    .FileName = "https://www.dropbox.com/scl/fi/ytmc85prpadjck1p6gcel/SD.zip?rlkey=ocdpfm5e1mr1g247itj3we0tt&st=j19trp9i&dl=0",
    .UseShellExecute = True
        })
    End Sub
    Private Sub BtnSdBancoTesteLink_Click(sender As Object, e As EventArgs) Handles BtnSdBancoTesteLink.Click
        Process.Start(New ProcessStartInfo With {
    .FileName = "https://www.dropbox.com/scl/fi/3qku6g3976gpdr7m960b1/SDSuperBancoTeste.zip?rlkey=dl3kzt5ep0tjkihh2z8zr7bbe&st=m756ltpm&dl=0",
    .UseShellExecute = True
        })
    End Sub
    Private Sub BtnManSdFtp_Click(sender As Object, e As EventArgs) Handles BtnManSdFtp.Click
        obj.BaixarArquivoFTP("ManuaisSD.zip", TxtFtp.Text)
    End Sub

    Private Sub BtnSdBancoTesteFtp_Click(sender As Object, e As EventArgs) Handles BtnSdBancoTesteFtp.Click
        obj.BaixarArquivoFTP("SDSuperBancoTeste.zip", TxtFtp.Text)
    End Sub


#End Region

#Region "Eclética"
    Private Sub BtnManuaisEclt_Click(sender As Object, e As EventArgs) Handles BtnManEcltLink.Click
        Process.Start(New ProcessStartInfo With {
        .FileName = "https://www.dropbox.com/scl/fi/r4s8hkbfi1kqqzbks0nu3/Ecletica.zip?rlkey=ozly5wyyichxmb4s94catrblg&st=thab677n&dl=0",
        .UseShellExecute = True
        })
    End Sub
    Private Sub BtnNfeEcltLink_Click(sender As Object, e As EventArgs) Handles BtnNfeEcltLink.Click
        Process.Start(New ProcessStartInfo With {
        .FileName = "https://www.dropbox.com/scl/fi/hyvfiwms9iq0fqe38sift/SetupNFe.zip?rlkey=2twryx7hg6ftkizoiyxyh6i3e&st=s2bggxx7&dl=0",
        .UseShellExecute = True
        })
    End Sub
    Private Sub BtnSetupEcletica_Click(sender As Object, e As EventArgs) Handles BtnSetupEcltFtp.Click
        obj.BaixarArquivoFTP("SetupEcleticaFood.zip", TxtFtp.Text)
    End Sub
    Private Sub BtnSetupNfceEclt_Click(sender As Object, e As EventArgs) Handles BtnSetupNfceEcltFtp.Click
        obj.BaixarArquivoFTP("SetupNFCe.zip", TxtFtp.Text)
    End Sub
    Private Sub BtnManEcltFtp_Click(sender As Object, e As EventArgs) Handles BtnManEcltFtp.Click
        obj.BaixarArquivoFTP("ManuaisEclt.zip", TxtFtp.Text)
    End Sub
    Private Sub BtnNfeEcltFtp_Click(sender As Object, e As EventArgs) Handles BtnNfeEcltFtp.Click
        obj.BaixarArquivoFTP("SetupNFe.zip", TxtFtp.Text)
    End Sub
    Private Sub BtnElginI8Aux_Click(sender As Object, e As EventArgs) Handles BtnElginI8Aux.Click
        obj.BaixarArquivoFTP("ElginI8", TxtFtp.Text)
    End Sub

    Private Sub SenhaPockets()

        Dim dataAtual As Date = Date.Today

        Dim novaData As Date = dataAtual.AddDays(10).AddMonths(5).AddYears(1)

        Dim dataFormatada As String = novaData.ToString("ddMMyy")

        LblSenhaPocket.Text = dataFormatada

    End Sub



#End Region

#Region "Nova era"

    Private Sub BtnManuaisNE_Click(sender As Object, e As EventArgs) Handles BtnManNELink.Click
        Process.Start(New ProcessStartInfo With {
        .FileName = "https://www.dropbox.com/scl/fi/l8xxrplncwl7nsqcfsjrt/Nova-Era.zip?rlkey=b1598vqta18td157p2ru9ubwl&st=dthtofjs&dl=0",
        .UseShellExecute = True
        })
    End Sub

#End Region

#Region "Funções e Subs"

    Function VerificarIP(ByVal ipEsperado As String) As Boolean
        Try
            Dim hostName As String = Dns.GetHostName()
            Dim enderecos As IPAddress() = Dns.GetHostAddresses(hostName)

            For Each ip In enderecos
                If ip.ToString() = ipEsperado Then
                    Return True
                End If
            Next
        Catch ex As Exception
            MessageBox.Show("Erro ao obter IP: " & ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Return False
    End Function
    Private Sub MensagemCMD()
        MessageBox.Show("Comando/função não pode ser feito automaticamente!")
    End Sub
    Private Sub MensagemComando()
        MessageBox.Show("Comando/função não possui script")
    End Sub
    Private Sub RbServidor_CheckedChanged(sender As Object, e As EventArgs) Handles RbServidor.CheckedChanged
        If RbServidor.Checked = True Then
            RbEstacao.Checked = False
        End If
    End Sub

    Private Sub RbEstacao_CheckedChanged(sender As Object, e As EventArgs) Handles RbEstacao.CheckedChanged
        If RbEstacao.Checked = True Then
            RbServidor.Checked = False
        End If
    End Sub

#End Region

End Class
