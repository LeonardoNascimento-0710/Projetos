Imports System.IO
Imports System.Windows.Forms
Imports Windows.Win32.UI
Imports System.Net
Imports Newtonsoft.Json
Imports System.Drawing.Printing
Imports Windows.Win32.UI.Input
Imports MySql.Data.MySqlClient
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.Drawing.Text
Imports iTextSharp.text
Imports iTextSharp.text.pdf

Public Class FrmOrdemDeServico
    Public Property TipoMaquina As String
    Dim conn As MySqlConnection
    Public Class Endereco
        Public Property cep As String
        Public Property logradouro As String
        Public Property complemento As String
        Public Property bairro As String
        Public Property localidade As String
        Public Property uf As String
        Public Property ibge As String
        Public Property gia As String
        Public Property ddd As String
        Public Property siafi As String

    End Class

#Region "Funções"
    Private Sub TxtCnpj_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtCnpj.KeyDown

        If e.KeyCode = Keys.Enter Then

            If TxtCnpj.TextLength = 14 Then

                If ValidarCNPJ(TxtCnpj.Text) = False Then
                    MsgBox("CNPJ inválido!", vbInformation, "Dados inválidos")
                End If

                Dim cnpj = TxtCnpj.Text.Replace(".", "").Replace("/", "").Replace("-", "").Replace(",", "")

                TxtCnpj.Text = cnpj.Insert(2, ".").Insert(6, ".").Insert(10, "/").Insert(15, "-")

                VerificaCpfCnpj()

            Else

                If ValidarCPF(TxtCnpj.Text) = False Then
                    MsgBox("CPF inválido!", vbInformation, "Dados inválidos")
                End If

                Dim cpf = TxtCnpj.Text.Replace(".", "").Replace("-", "").Replace(",", "")

                TxtCnpj.Text = cpf.Insert(3, ".").Insert(7, ".").Insert(11, "-")

                VerificaCpfCnpj()

            End If

        End If

    End Sub
    Private Sub BtnLimparDados_Click(sender As Object, e As EventArgs) Handles BtnLimparDados.Click
        LimpaDados()
    End Sub
    Private Sub TxtCep_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtCep.KeyDown

        If e.KeyCode = Keys.Enter Then

            FormatarCep()

            Dim cep = TxtCep.Text.Trim

            Try
                If TxtCep.TextLength <> 9 Then
                    MessageBox.Show("Informe um CEP válido!")
                    Return
                End If

                Dim url = $"https://viacep.com.br/ws/{cep}/json/"

                Dim requisicao = WebRequest.Create(url)
                requisicao.Method = "GET"

                Using resposta = requisicao.GetResponse
                    Using reader As New StreamReader(resposta.GetResponseStream)
                        Dim json = reader.ReadToEnd

                        Dim endereco = JsonConvert.DeserializeObject(Of Endereco)(json)

                        TxtLogradouro.Text = endereco.logradouro
                        TxtBairro.Text = endereco.bairro
                        TxtCidade.Text = endereco.localidade
                        TxtEstado.Text = endereco.uf

                    End Using
                End Using

            Catch ex As Exception
                MessageBox.Show("Erro ao buscar o CEP: " & ex.Message)
            End Try
        End If
    End Sub


    Private Sub FrmOrdemDeServico_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
        TxtDataEmissao.Text = DateTime.Now.ToString("dddd',' dd 'de' MMMM 'de' yyyy",
        New System.Globalization.CultureInfo("pt-BR"))

        If TipoMaquina = "SERVIDOR" Then
            conn = New MySqlConnection("server=localhost;user id=root;password=root;database=acsauxiliares")
        Else
            conn = New MySqlConnection("server=192.168.15.123;user id=root;password=root;database=acsauxiliares")
        End If

        CarregaClientes()
    End Sub
    Private Sub FrmOrdemDeServico_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If Not (TxtCep.Focused Or TxtCnpj.Focused) Then
            If e.KeyCode = Keys.Enter Then
                e.SuppressKeyPress = True
                Me.SelectNextControl(Me.ActiveControl, True, True, True, True)
            End If
        End If
    End Sub

    Private Sub PesquisaClientes()
        Try
            conn.Open()

            Dim cmd As New MySqlCommand("SELECT c.id_cliente, c.cpf_cnpj, c.nome, c.email, c.telefone, e.cep, e.bairro, e.cidade, e.estado, e.logradouro, e.numero, e.complemento 
                                     FROM clientes c
                                     LEFT JOIN endereco e ON c.id_cliente = e.id_cliente
                                     WHERE c.razao_social = @razao_social", conn)
            cmd.Parameters.AddWithValue("@razao_social", CmbRazaoSocial.Text)

            Dim reader As MySqlDataReader = cmd.ExecuteReader()

            If reader.Read() Then
                TxtIdCliente.Text = reader("id_cliente").ToString()
                TxtCliente.Text = reader("nome").ToString()
                TxtCnpj.Text = reader("cpf_cnpj").ToString()
                TxtCnpj.Enabled = False
                TxtEmail.Text = reader("email").ToString()
                TxtTellCell.Text = reader("telefone").ToString
                TxtCep.Text = reader("cep").ToString()
                TxtBairro.Text = reader("bairro").ToString()
                TxtCidade.Text = reader("cidade").ToString()
                TxtEstado.Text = reader("estado").ToString()
                TxtLogradouro.Text = reader("logradouro").ToString()
                TxtNumero.Text = reader("numero").ToString()
                TxtComplemento.Text = reader("complemento").ToString()
            Else
                reader.Close()

                Dim cmdUltimoId As New MySqlCommand("SELECT MAX(id_cliente) FROM clientes", conn)
                Dim ultimoId As Object = cmdUltimoId.ExecuteScalar()
                Dim novoId As Integer = If(ultimoId IsNot DBNull.Value AndAlso ultimoId IsNot Nothing, Convert.ToInt32(ultimoId) + 1, 1)

                TxtIdCliente.Text = novoId.ToString()
            End If

            reader.Close()

        Catch ex As Exception
            MessageBox.Show("Erro ao conectar ao banco de dados: " & ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub
    Private Sub VerificaCpfCnpj()
        Try
            conn.Open()

            Dim cmd As New MySqlCommand("SELECT c.id_cliente, c.nome, c.email, c.razao_social, c.cpf_cnpj, c.telefone, 
                                            e.cep, e.bairro, e.cidade, e.estado, e.logradouro, e.numero, e.complemento
                                     FROM clientes c
                                     LEFT JOIN endereco e ON c.id_cliente = e.id_cliente
                                     WHERE c.cpf_cnpj = @cpf_cnpj", conn)
            cmd.Parameters.AddWithValue("@cpf_cnpj", TxtCnpj.Text)

            Dim reader As MySqlDataReader = cmd.ExecuteReader()

            If reader.Read() Then
                Dim resposta As DialogResult = MessageBox.Show("Este CPF/CNPJ já está cadastrado. Deseja carregar os dados?",
                                                           "Registro Encontrado", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

                If resposta = DialogResult.Yes Then
                    RemoveHandler CmbRazaoSocial.SelectedIndexChanged, AddressOf CmbRazaoSocial_SelectedIndexChanged

                    TxtIdCliente.Text = reader("id_cliente").ToString()
                    TxtCliente.Text = reader("nome").ToString()
                    TxtCnpj.Text = reader("cpf_cnpj").ToString()
                    TxtCnpj.Enabled = False
                    TxtEmail.Text = reader("email").ToString()
                    TxtTellCell.Text = reader("telefone").ToString
                    TxtCep.Text = reader("cep").ToString()
                    TxtBairro.Text = reader("bairro").ToString()
                    TxtCidade.Text = reader("cidade").ToString()
                    TxtEstado.Text = reader("estado").ToString()
                    TxtLogradouro.Text = reader("logradouro").ToString()
                    TxtNumero.Text = reader("numero").ToString()
                    TxtComplemento.Text = reader("complemento").ToString()

                    CmbRazaoSocial.Text = reader("razao_social").ToString()

                    AddHandler CmbRazaoSocial.SelectedIndexChanged, AddressOf CmbRazaoSocial_SelectedIndexChanged

                Else
                    TxtCnpj.Text = ""
                End If
            End If

            reader.Close()
        Catch ex As Exception
            MessageBox.Show("Erro ao verificar CPF/CNPJ: " & ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub

    Private Sub CmbRazaoSocial_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbRazaoSocial.SelectedIndexChanged
        PesquisaClientes()
    End Sub
    Private Sub BtnGerarPdf_Click(sender As Object, e As EventArgs) Handles BtnGerarPdf.Click
        Gravardados()
    End Sub

    Private Sub Gravardados()
        If ValidaCampos() = False Then
            Exit Sub
        End If
        Try
            conn.Open()

            Dim cmdCNPJ As New MySqlCommand("SELECT id_cliente FROM clientes WHERE cpf_cnpj = @cpf_cnpj", conn)
            cmdCNPJ.Parameters.AddWithValue("@cpf_cnpj", TxtCnpj.Text)
            Dim idCliente As Object = cmdCNPJ.ExecuteScalar()

            If idCliente Is Nothing OrElse idCliente Is DBNull.Value Then

                Dim cmdUltimoID As New MySqlCommand("SELECT MAX(id_cliente) FROM clientes", conn)
                Dim ultimoID As Object = cmdUltimoID.ExecuteScalar()
                Dim novoId As Integer = If(ultimoID IsNot DBNull.Value AndAlso ultimoID IsNot Nothing, Convert.ToInt32(ultimoID) + 1, 1)

                idCliente = novoId

                Dim cmdCliente As New MySqlCommand("INSERT INTO clientes (id_cliente, nome, cpf_cnpj, email, razao_social, telefone) 
                                              VALUES (@id_cliente, @nome, @cpf_cnpj, @email, @razao_social, @telefone)", conn)
                cmdCliente.Parameters.AddWithValue("@id_cliente", novoId)
                cmdCliente.Parameters.AddWithValue("@nome", TxtCliente.Text)
                cmdCliente.Parameters.AddWithValue("@cpf_cnpj", TxtCnpj.Text)
                cmdCliente.Parameters.AddWithValue("@email", TxtEmail.Text)
                cmdCliente.Parameters.AddWithValue("@razao_social", CmbRazaoSocial.Text)
                cmdCliente.Parameters.AddWithValue("@telefone", TxtTellCell.Text)
                cmdCliente.ExecuteNonQuery()

                Dim cmdEndereco As New MySqlCommand("INSERT INTO endereco (id_cliente, cep, bairro, cidade, estado, logradouro, numero, complemento) 
                                              VALUES (@id_cliente, @cep, @bairro, @cidade, @estado, @logradouro, @numero, @complemento)", conn)
                cmdEndereco.Parameters.AddWithValue("@id_cliente", novoId)
                cmdEndereco.Parameters.AddWithValue("@cep", TxtCep.Text)
                cmdEndereco.Parameters.AddWithValue("@bairro", TxtBairro.Text)
                cmdEndereco.Parameters.AddWithValue("@cidade", TxtCidade.Text)
                cmdEndereco.Parameters.AddWithValue("@estado", TxtEstado.Text)
                cmdEndereco.Parameters.AddWithValue("@logradouro", TxtLogradouro.Text)
                cmdEndereco.Parameters.AddWithValue("@numero", TxtNumero.Text)
                cmdEndereco.Parameters.AddWithValue("@complemento", TxtComplemento.Text)
                cmdEndereco.ExecuteNonQuery()
            Else
                idCliente = Convert.ToInt32(idCliente)
            End If

            Dim cmdOrdemServico As New MySqlCommand("INSERT INTO ordem_servico (id_ordem, id_cliente, data, tecnico, chamado, observacoes) 
                                              VALUES (@id_ordem, @id_cliente, @data, @tecnico, @chamado, @observacoes)", conn)
            cmdOrdemServico.Parameters.AddWithValue("@id_ordem", Convert.ToInt32(TxtIdOrdem.Text))
            cmdOrdemServico.Parameters.AddWithValue("@id_cliente", idCliente)
            cmdOrdemServico.Parameters.AddWithValue("@data", DateTime.Now.ToString("yyyy-MM-dd"))
            cmdOrdemServico.Parameters.AddWithValue("@tecnico", TxtTecnico.Text)
            cmdOrdemServico.Parameters.AddWithValue("@chamado", Txtservicos.Text)
            cmdOrdemServico.Parameters.AddWithValue("@observacoes", Txtobservacoes.Text)
            cmdOrdemServico.ExecuteNonQuery()

        Catch ex As Exception
            MessageBox.Show("Erro ao processar a ordem de serviço: " & ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            conn.Close()
            GerarPdf()
        End Try

    End Sub
    Private Sub GerarPdf()
        If ValidaCampos() = False Then
            Exit Sub
        End If

        Try
            ' Definir a data e configurar o diálogo para salvar o PDF
            Dim data = DateTime.Now.ToString("ddMMyyyy_hhmm")
            Dim saveFileDialog As New SaveFileDialog()
            saveFileDialog.Filter = "Arquivos PDF (*.pdf)|*.pdf"
            saveFileDialog.Title = "Salvar Ordem de Serviço"
            saveFileDialog.FileName = "OS_" & TxtCliente.Text & "_" & data & ".pdf"

            If saveFileDialog.ShowDialog() = DialogResult.OK Then
                Dim outputPath As String = saveFileDialog.FileName
                ' Usando iTextSharp.text.Document
                Dim doc As New iTextSharp.text.Document(PageSize.A4, 40, 40, 100, 40)

                ' Criar o escritor de PDF
                Using writer As PdfWriter = PdfWriter.GetInstance(doc, New FileStream(outputPath, FileMode.Create))
                    doc.Open()

                    ' Definir fontes
                    Dim fonteTitulo As Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)
                    Dim fonteTexto As Font = FontFactory.GetFont(FontFactory.HELVETICA, 8)

                    ' Cabeçalho do documento
                    Dim tabelaCabecalho As New PdfPTable(3)
                    tabelaCabecalho.WidthPercentage = 100
                    tabelaCabecalho.SetWidths({20, 60, 20})

                    ' Logo (se disponível)
                    Dim logoPath As String = "C:\Caminho\Para\Logo.png"
                    If System.IO.File.Exists(logoPath) Then
                        Dim logo As Image = Image.GetInstance(logoPath)
                        logo.ScaleAbsolute(60, 60)
                        tabelaCabecalho.AddCell(New PdfPCell(logo) With {.Border = 0, .HorizontalAlignment = Element.ALIGN_LEFT})
                    Else
                        tabelaCabecalho.AddCell(New PdfPCell(New Phrase(" ", fonteTexto)) With {.Border = 0})
                    End If

                    ' Informações da empresa
                    Dim empresaInfo As String = "ACS AUTOMAÇÃO COMERCIAL E SERVIÇOS LTDA" & vbCrLf &
                                            "10.603.263/0001-49" & vbCrLf &
                                            "RUA SÃO JANUÁRIO Nº 93" & vbCrLf &
                                            "CEP: 02245-140 - PARADA INGLESA" & vbCrLf &
                                            "(11) 2976-3900" & vbCrLf &
                                            "www.acsautomacao.com.br" & vbCrLf &
                                            "contato@acsautomacao.com.br"
                    Dim cellEmpresa As New PdfPCell(New Phrase(empresaInfo, fonteTexto)) With {
                    .Border = 0,
                    .HorizontalAlignment = Element.ALIGN_LEFT
                }
                    cellEmpresa.Colspan = 2
                    tabelaCabecalho.AddCell(cellEmpresa)

                    ' Número da ordem de serviço
                    Dim osNumero As String = "Ordem de Serviço" & vbCrLf & "Nº " & TxtIdOrdem.Text
                    tabelaCabecalho.AddCell(New PdfPCell(New Phrase(osNumero, fonteTitulo)) With {.Border = 0, .HorizontalAlignment = Element.ALIGN_RIGHT})

                    ' Adicionar cabeçalho ao documento
                    doc.Add(tabelaCabecalho)
                    doc.Add(New Paragraph(" "))

                    ' Informações do cliente
                    Dim tabelaCliente As New PdfPTable(1)
                    tabelaCliente.WidthPercentage = 100
                    tabelaCliente.AddCell(New PdfPCell(New Phrase("Cliente: " & TxtCliente.Text, fonteTexto)) With {.Border = 1, .HorizontalAlignment = Element.ALIGN_LEFT})
                    tabelaCliente.AddCell(New PdfPCell(New Phrase("Razão Social: " & CmbRazaoSocial.Text, fonteTexto)) With {.Border = 1, .HorizontalAlignment = Element.ALIGN_LEFT})
                    tabelaCliente.AddCell(New PdfPCell(New Phrase("CPF/CNPJ: " & TxtCnpj.Text, fonteTexto)) With {.Border = 1, .HorizontalAlignment = Element.ALIGN_LEFT})
                    tabelaCliente.AddCell(New PdfPCell(New Phrase("Email: " & TxtEmail.Text, fonteTexto)) With {.Border = 1, .HorizontalAlignment = Element.ALIGN_LEFT})
                    tabelaCliente.AddCell(New PdfPCell(New Phrase("Telefone: " & TxtTellCell.Text, fonteTexto)) With {.Border = 1, .HorizontalAlignment = Element.ALIGN_LEFT})
                    doc.Add(tabelaCliente)

                    doc.Add(New Paragraph(" "))

                    ' Endereço
                    Dim tabelaEndereco As New PdfPTable(1)
                    tabelaEndereco.WidthPercentage = 100
                    tabelaEndereco.AddCell(New PdfPCell(New Phrase("Logradouro: " & TxtLogradouro.Text, fonteTexto)) With {.Border = 1, .HorizontalAlignment = Element.ALIGN_LEFT})
                    tabelaEndereco.AddCell(New PdfPCell(New Phrase("Número: " & TxtNumero.Text, fonteTexto)) With {.Border = 1, .HorizontalAlignment = Element.ALIGN_LEFT})
                    tabelaEndereco.AddCell(New PdfPCell(New Phrase("Bairro: " & TxtBairro.Text, fonteTexto)) With {.Border = 1, .HorizontalAlignment = Element.ALIGN_LEFT})
                    tabelaEndereco.AddCell(New PdfPCell(New Phrase("Cidade: " & TxtCidade.Text, fonteTexto)) With {.Border = 1, .HorizontalAlignment = Element.ALIGN_LEFT})
                    tabelaEndereco.AddCell(New PdfPCell(New Phrase("Estado: " & TxtEstado.Text, fonteTexto)) With {.Border = 1, .HorizontalAlignment = Element.ALIGN_LEFT})
                    tabelaEndereco.AddCell(New PdfPCell(New Phrase("CEP: " & TxtCep.Text, fonteTexto)) With {.Border = 1, .HorizontalAlignment = Element.ALIGN_LEFT})
                    tabelaEndereco.AddCell(New PdfPCell(New Phrase("Complemento: " & TxtComplemento.Text, fonteTexto)) With {.Border = 1, .HorizontalAlignment = Element.ALIGN_LEFT})
                    doc.Add(tabelaEndereco)

                    doc.Add(New Paragraph(" "))

                    ' Ordem de serviço
                    Dim tabelaOrdem As New PdfPTable(1)
                    tabelaOrdem.WidthPercentage = 100
                    tabelaOrdem.AddCell(New PdfPCell(New Phrase("Técnico: " & TxtTecnico.Text, fonteTexto)) With {.Border = 1, .HorizontalAlignment = Element.ALIGN_LEFT})
                    tabelaOrdem.AddCell(New PdfPCell(New Phrase("Serviço: " & Txtservicos.Text, fonteTexto)) With {.Border = 1, .HorizontalAlignment = Element.ALIGN_LEFT})
                    tabelaOrdem.AddCell(New PdfPCell(New Phrase("Observações: " & Txtobservacoes.Text, fonteTexto)) With {.Border = 1, .HorizontalAlignment = Element.ALIGN_LEFT})
                    doc.Add(tabelaOrdem)

                    ' Fechar o documento
                    doc.Close()

                    ' Mensagem de sucesso
                    MessageBox.Show("Ordem de Serviço gerada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Using
            End If

        Catch ex As Exception
            MessageBox.Show("Erro ao gerar a Ordem de Serviço: " & ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub CarregaClientes()
        Try
            conn.Open()

            Dim queryClientes As String = "SELECT razao_social FROM clientes"
            Dim cmdClientes As New MySqlCommand(queryClientes, conn)
            Dim reader As MySqlDataReader = cmdClientes.ExecuteReader()

            CmbRazaoSocial.Items.Clear()

            While reader.Read()
                CmbRazaoSocial.Items.Add(reader("razao_social").ToString())
            End While
            reader.Close()

            Dim queryOrdem As String = "SELECT COALESCE(MAX(id_ordem), 0) + 1 AS proximo_id FROM ordem_servico"
            Dim cmdOrdem As New MySqlCommand(queryOrdem, conn)
            Dim proximoId As Integer = Convert.ToInt32(cmdOrdem.ExecuteScalar())

            TxtIdOrdem.Text = proximoId.ToString()

            Dim cmdUltimoId As New MySqlCommand("SELECT MAX(id_cliente) FROM clientes", conn)
            Dim ultimoId As Object = cmdUltimoId.ExecuteScalar()

            Dim novoId As Integer = If(ultimoId IsNot DBNull.Value AndAlso ultimoId IsNot Nothing, Convert.ToInt32(ultimoId) + 1, 1)
            TxtIdCliente.Text = novoId.ToString()

        Catch ex As Exception
            MessageBox.Show("Erro: " & ex.Message)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub


#End Region

#Region "Dados e formatação"

    Private Sub LimpaDados()

        CmbRazaoSocial.SelectedIndex = -1
        TxtBairro.Text = ""
        TxtCep.Text = ""
        TxtCidade.Text = ""
        TxtCliente.Text = ""
        TxtCnpj.Text = ""
        TxtCnpj.Enabled = True
        TxtComplemento.Text = ""
        TxtEmail.Text = ""
        TxtEstado.Text = ""
        TxtLogradouro.Text = ""
        TxtNumero.Text = ""
        Txtobservacoes.Text = ""
        Txtservicos.Text = ""
        TxtTecnico.Text = ""
        TxtTellCell.Text = ""

        CarregaClientes()

    End Sub
    Private Sub FormatarCep()

        Dim cep As String = New String(TxtCep.Text.Where(AddressOf Char.IsDigit).ToArray())


        If cep.Length > 5 Then
            cep = cep.Insert(5, "-")
        End If

        TxtCep.Text = cep
        TxtCep.SelectionStart = cep.Length
    End Sub

    Private Function ValidaCampos()
        Dim missingField As String = ""

        If TxtCliente.Text = "" Then missingField = "Cliente"
        If TxtCnpj.Text = "" Then missingField = "CNPJ"
        If TxtTecnico.Text = "" Then missingField = "Tecnico"
        If Txtservicos.Text = "" Then missingField = "Servicos"
        If TxtCep.Text = "" Then missingField = "CEP"
        If TxtNumero.Text = "" Then missingField = "Nº"

        If missingField <> "" Then
            MsgBox("Preencha todos os campos!", vbInformation, "Informativo")
            Return False
            Exit Function
        End If

        Return True

    End Function

    Function ValidarCPF(cpf As String) As Boolean

        cpf = New String(cpf.Where(AddressOf Char.IsDigit).ToArray())

        If cpf.Length <> 11 OrElse Not cpf.All(AddressOf Char.IsDigit) Then
            Return False
        End If

        Dim soma As Integer = 0
        Dim resto As Integer
        For i As Integer = 0 To 8
            soma += Integer.Parse(cpf(i).ToString()) * (10 - i)
        Next

        resto = soma Mod 11
        If resto < 2 Then
            If Integer.Parse(cpf(9).ToString()) <> 0 Then
                Return False
            End If
        Else
            If Integer.Parse(cpf(9).ToString()) <> 11 - resto Then
                Return False
            End If
        End If

        soma = 0
        For i As Integer = 0 To 9
            soma += Integer.Parse(cpf(i).ToString()) * (11 - i)
        Next

        resto = soma Mod 11
        If resto < 2 Then
            If Integer.Parse(cpf(10).ToString()) <> 0 Then
                Return False
            End If
        Else
            If Integer.Parse(cpf(10).ToString()) <> 11 - resto Then
                Return False
            End If
        End If

        Return True
    End Function

    Function ValidarCNPJ(cnpj As String) As Boolean

        cnpj = New String(cnpj.Where(AddressOf Char.IsDigit).ToArray())

        If cnpj.Length <> 14 OrElse Not cnpj.All(AddressOf Char.IsDigit) Then
            Return False
        End If

        Dim soma As Integer = 0
        Dim resto As Integer
        Dim pesos1() As Integer = {5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2}
        For i As Integer = 0 To 11
            soma += Integer.Parse(cnpj(i).ToString()) * pesos1(i)
        Next
        resto = soma Mod 11
        If resto < 2 Then
            If Integer.Parse(cnpj(12).ToString()) <> 0 Then
                Return False
            End If
        Else
            If Integer.Parse(cnpj(12).ToString()) <> 11 - resto Then
                Return False
            End If
        End If

        soma = 0
        Dim pesos2() As Integer = {6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2}
        For i As Integer = 0 To 12
            soma += Integer.Parse(cnpj(i).ToString()) * pesos2(i)
        Next
        resto = soma Mod 11
        If resto < 2 Then
            If Integer.Parse(cnpj(13).ToString()) <> 0 Then
                Return False
            End If
        Else
            If Integer.Parse(cnpj(13).ToString()) <> 11 - resto Then
                Return False
            End If
        End If

        Return True
    End Function


#End Region

End Class
