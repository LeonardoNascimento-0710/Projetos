Imports ClosedXML.Excel
Imports MySql.Data.MySqlClient
Imports System.Data
Imports System.IO

Public Class FrmConversorBalancaToledo

    Private conn As New MySqlConnection(
        "server=localhost;port=3307;database=projects;user id=root;password=root;")

#Region "Containers e Botões"

    Private Sub BtnImportarArquivos_Click(sender As Object, e As EventArgs) Handles BtnImportarArquivos.Click

        Try

            Using ofd As New OpenFileDialog

                ofd.Filter = "Arquivos TXT (*.txt)|*.txt"

                ofd.Title = "Selecione o arquivo ITENSMGV.TXT"

                If ofd.ShowDialog <> DialogResult.OK Then Exit Sub

                ImportarItens(ofd.FileName)

                ofd.Title = "Selecione o arquivo INFNUTRI.TXT"

                If ofd.ShowDialog <> DialogResult.OK Then Exit Sub

                ImportarNutri(ofd.FileName)

            End Using

            CarregarCodigos()

            MessageBox.Show("Importação concluída com sucesso!",
                        "Conversor Toledo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information)

        Catch ex As Exception

            MessageBox.Show(ex.Message,
                        "Erro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error)

        End Try

    End Sub

    Private Sub BtnExportar_Click(sender As Object, e As EventArgs) Handles BtnExportar.Click

        ExportarParaSd()

        Using fbd As New FolderBrowserDialog

            fbd.Description = "Selecione a pasta onde serão gerados os arquivos"

            If fbd.ShowDialog() = DialogResult.OK Then

                GerarArquivoNutricionais(fbd.SelectedPath)

                GerarArquivoUpdateNutricionais(fbd.SelectedPath)

                MessageBox.Show("Arquivos gerados com sucesso!")

            End If

        End Using

    End Sub

    Private Sub BtnRetornar_Click(sender As Object, e As EventArgs) Handles BtnRetornar.Click
        Me.Dispose()
    End Sub

#End Region


#Region "Funções"

    Private Sub ImportarItens(caminho As String)

        conn.Open()

        Dim cmd As New MySqlCommand("TRUNCATE TABLE balanca_toledo_itens", conn)
        cmd.ExecuteNonQuery()

        cmd.CommandText =
    "INSERT INTO balanca_toledo_itens
    (tipo,codigo,validade,descricao,cod_nutri)
    VALUES
    (@tipo,@codigo,@validade,@descricao,@codnutri)"

        cmd.Parameters.Clear()

        cmd.Parameters.Add("@tipo", MySqlDbType.VarChar)
        cmd.Parameters.Add("@codigo", MySqlDbType.Int32)
        cmd.Parameters.Add("@validade", MySqlDbType.Int32)
        cmd.Parameters.Add("@descricao", MySqlDbType.VarChar)
        cmd.Parameters.Add("@codnutri", MySqlDbType.Int32)

        For Each linha As String In IO.File.ReadLines(caminho)

            Try

                If linha.Trim = "" Then Continue For

                cmd.Parameters("@tipo").Value = linha.Substring(2, 1).Trim()

                cmd.Parameters("@codigo").Value =
                Integer.Parse(linha.Substring(3, 6))

                cmd.Parameters("@validade").Value =
                Integer.Parse(linha.Substring(15, 3))

                cmd.Parameters("@descricao").Value =
                linha.Substring(18, 25).Trim()

                cmd.Parameters("@codnutri").Value =
                Integer.Parse(linha.Substring(78, 6))

                cmd.ExecuteNonQuery()

            Catch
                Continue For
            End Try

        Next

        conn.Close()

    End Sub

    Private Sub CmbCodItem_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbCodItem.SelectedIndexChanged

        If CmbCodItem.Text <> "" Then

            BuscarProduto()

        End If

    End Sub

    Private Sub ImportarNutri(caminho As String)

        conn.Open()

        Dim cmd As New MySqlCommand("TRUNCATE TABLE balanca_toledo_nutri", conn)
        cmd.ExecuteNonQuery()

        cmd.CommandText =
"INSERT INTO balanca_toledo_nutri
(
    codigo,
    codigo_sd,
    porcoes,
    qtd_porcao,
    unidade,
    medida_inteira,
    medida_decimal,
    medida_caseira,
    valor_energetico,
    carboidratos,
    acucares_totais,
    acucares_adicionados,
    proteinas,
    gorduras_totais,
    gorduras_saturadas,
    gorduras_trans,
    fibras,
    sodio,
    alto_acucar,
    alto_gordura,
    alto_sodio,
    lactose,
    galactose,
    acucares_adicionados_ext,
    acucares_totais_ext,
    gorduras_totais_ext,
    proteinas_ext
)
VALUES
(
    @codigo,
    @codigo_sd,
    @porcoes,
    @qtd_porcao,
    @unidade,
    @medida_inteira,
    @medida_decimal,
    @medida_caseira,
    @valor_energetico,
    @carboidratos,
    @acucares_totais,
    @acucares_adicionados,
    @proteinas,
    @gorduras_totais,
    @gorduras_saturadas,
    @gorduras_trans,
    @fibras,
    @sodio,
    @alto_acucar,
    @alto_gordura,
    @alto_sodio,
    @lactose,
    @galactose,
    @acucares_adicionados_ext,
    @acucares_totais_ext,
    @gorduras_totais_ext,
    @proteinas_ext
)"

        Dim codigoSistema As Integer = 11

        For Each linha As String In IO.File.ReadLines(caminho)

            Dim dados As String = ""
            Dim pos As Integer = 0

            Try

                If linha.Trim = "" Then Continue For

                Dim partes() As String = linha.Split("|"c)

                If partes.Length < 2 Then Continue For

                Dim codigo As Integer = Integer.Parse(partes(0).Substring(1, 6))

                Dim cmdVerifica As New MySqlCommand(
                 "SELECT descricao FROM balanca_toledo_itens WHERE cod_nutri = @codigo",
                    conn)

                cmdVerifica.Parameters.AddWithValue("@codigo", codigo)

                Dim descricao As Object = cmdVerifica.ExecuteScalar()

                If descricao Is Nothing OrElse IsDBNull(descricao) OrElse descricao.ToString.Trim = "" Then
                    Continue For
                End If

                dados = partes(1)

                pos = 0

                pos += 1

                Dim porcoes As Integer = Integer.Parse(dados.Substring(pos, 3)) : pos += 3
                Dim qtdPorcao As Integer = Integer.Parse(dados.Substring(pos, 3)) : pos += 3
                Dim unidade As String = dados.Substring(pos, 1) : pos += 1
                Dim medidaInteira As Integer = Integer.Parse(dados.Substring(pos, 2)) : pos += 2
                Dim medidaDecimal As Integer = Integer.Parse(dados.Substring(pos, 1)) : pos += 1
                Dim medidaCaseira As Integer = Integer.Parse(dados.Substring(pos, 2)) : pos += 2
                Dim valorEnergetico As Integer = Integer.Parse(dados.Substring(pos, 4)) : pos += 4

                Dim carboidratos As Decimal = ConverteDecimal(dados.Substring(pos, 4)) : pos += 4
                Dim acucarTotal As Decimal = ConverteDecimal(dados.Substring(pos, 3)) : pos += 3
                Dim acucarAdicionado As Decimal = ConverteDecimal(dados.Substring(pos, 3)) : pos += 3
                Dim proteinas As Decimal = ConverteDecimal(dados.Substring(pos, 3)) : pos += 3
                Dim gordurasTotais As Decimal = ConverteDecimal(dados.Substring(pos, 3)) : pos += 3
                Dim gordurasSaturadas As Decimal = ConverteDecimal(dados.Substring(pos, 3)) : pos += 3
                Dim gordurasTrans As Decimal = ConverteDecimal(dados.Substring(pos, 3)) : pos += 3
                Dim fibras As Decimal = ConverteDecimal(dados.Substring(pos, 3)) : pos += 3
                Dim sodio As Decimal = ConverteDecimal(dados.Substring(pos, 5)) : pos += 5

                Dim altoAcucar As String = dados.Substring(pos, 1) : pos += 1
                Dim altoGordura As String = dados.Substring(pos, 1) : pos += 1
                Dim altoSodio As String = dados.Substring(pos, 1) : pos += 1

                Dim lactose As Decimal = ConverteDecimal(dados.Substring(pos, 5)) : pos += 5
                Dim galactose As Decimal = ConverteDecimal(dados.Substring(pos, 5)) : pos += 5

                pos += 1

                Dim acucarAddExt As Decimal = 0
                Dim acucarTotalExt As Decimal = 0
                Dim gorduraTotalExt As Decimal = 0
                Dim proteinaExt As Decimal = 0

                cmd.Parameters.Clear()

                cmd.Parameters.AddWithValue("@codigo", codigo)
                cmd.Parameters.AddWithValue("@codigo_sd", codigoSistema)
                cmd.Parameters.AddWithValue("@porcoes", porcoes)
                cmd.Parameters.AddWithValue("@qtd_porcao", qtdPorcao)
                cmd.Parameters.AddWithValue("@unidade", unidade)
                cmd.Parameters.AddWithValue("@medida_inteira", medidaInteira)
                cmd.Parameters.AddWithValue("@medida_decimal", medidaDecimal)
                cmd.Parameters.AddWithValue("@medida_caseira", medidaCaseira)
                cmd.Parameters.AddWithValue("@valor_energetico", valorEnergetico)
                cmd.Parameters.AddWithValue("@carboidratos", carboidratos)
                cmd.Parameters.AddWithValue("@acucares_totais", acucarTotal)
                cmd.Parameters.AddWithValue("@acucares_adicionados", acucarAdicionado)
                cmd.Parameters.AddWithValue("@proteinas", proteinas)
                cmd.Parameters.AddWithValue("@gorduras_totais", gordurasTotais)
                cmd.Parameters.AddWithValue("@gorduras_saturadas", gordurasSaturadas)
                cmd.Parameters.AddWithValue("@gorduras_trans", gordurasTrans)
                cmd.Parameters.AddWithValue("@fibras", fibras)
                cmd.Parameters.AddWithValue("@sodio", sodio)
                cmd.Parameters.AddWithValue("@alto_acucar", altoAcucar)
                cmd.Parameters.AddWithValue("@alto_gordura", altoGordura)
                cmd.Parameters.AddWithValue("@alto_sodio", altoSodio)
                cmd.Parameters.AddWithValue("@lactose", lactose)
                cmd.Parameters.AddWithValue("@galactose", galactose)
                cmd.Parameters.AddWithValue("@acucares_adicionados_ext", acucarAddExt)
                cmd.Parameters.AddWithValue("@acucares_totais_ext", acucarTotalExt)
                cmd.Parameters.AddWithValue("@gorduras_totais_ext", gorduraTotalExt)
                cmd.Parameters.AddWithValue("@proteinas_ext", proteinaExt)

                cmd.ExecuteNonQuery()

                codigoSistema += 10

            Catch ex As Exception

                MessageBox.Show(
                "Erro: " & ex.Message & vbCrLf &
                "Posição: " & pos & vbCrLf &
                "Tamanho da string: " & dados.Length & vbCrLf &
                "Dados: " & dados)

                Exit Sub

            End Try

        Next

        conn.Close()

        AtualizarDescricaoNutri()

    End Sub

    Private Sub AtualizarDescricaoNutri()

        conn.Open()

        Dim sql As String =
    "UPDATE balanca_toledo_nutri n
     INNER JOIN balanca_toledo_itens i
         ON i.cod_nutri = n.codigo
     SET n.descricao = i.descricao;"

        Dim cmd As New MySqlCommand(sql, conn)
        cmd.ExecuteNonQuery()

        conn.Close()

    End Sub

    Private Sub CarregarCodigos()

        Try

            CmbCodItem.Items.Clear()

            conn.Open()

            Dim cmd As New MySqlCommand("SELECT codigo FROM balanca_toledo_itens ORDER BY codigo", conn)

            Dim dr As MySqlDataReader = cmd.ExecuteReader()

            While dr.Read()

                CmbCodItem.Items.Add(dr("codigo").ToString())

            End While

            dr.Close()

            conn.Close()

        Catch ex As Exception

            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If

            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Function ConverteDecimal(valor As String) As Decimal

        valor = valor.Trim()

        If valor = "" Then Return 0

        If valor.Length = 1 Then
            Return Decimal.Parse("0," & valor)
        End If

        Dim inteiro As String = valor.Substring(0, valor.Length - 1)
        Dim decimalParte As String = valor.Substring(valor.Length - 1, 1)

        Return Decimal.Parse(inteiro & "," & decimalParte)

    End Function

    Private Sub BuscarProduto()

        Try

            conn.Open()

            Dim sql As String =
            "SELECT *
        FROM balanca_toledo_itens i
        LEFT JOIN balanca_toledo_nutri n
        ON i.cod_nutri = n.codigo
        WHERE i.codigo = @codigo"

            Dim cmd As New MySqlCommand(sql, conn)

            cmd.Parameters.AddWithValue("@codigo", CmbCodItem.Text)

            Dim dr As MySqlDataReader = cmd.ExecuteReader()

            If dr.Read() Then


                Select Case dr("tipo").ToString()
                    Case "0"
                        CmbMedida.SelectedItem = "KG"

                    Case "1"
                        CmbMedida.SelectedItem = "UND"

                    Case Else
                        CmbMedida.SelectedIndex = 0
                End Select

                TxtDescricao.Text = dr("descricao").ToString()
                TxtValidade.Text = dr("validade").ToString()
                LblCodNutri.Text = dr("cod_nutri").ToString()

                TxtQtdeTotal.Text = dr("qtd_porcao").ToString()

                TxtPorcao.Text = dr("medida_inteira").ToString()

                If dr("unidade").ToString() = "0" Then
                    CmbTipoPorcao.SelectedIndex = 0
                Else
                    CmbTipoPorcao.SelectedIndex = 1
                End If

                Select Case dr("medida_decimal").ToString()

                    Case "0"
                        CmbQtdePorcao.SelectedIndex = 0

                    Case "1"
                        CmbQtdePorcao.SelectedIndex = 1

                    Case "2"
                        CmbQtdePorcao.SelectedIndex = 2

                    Case "3"
                        CmbQtdePorcao.SelectedIndex = 3

                    Case "4"
                        CmbQtdePorcao.SelectedIndex = 4

                    Case "5"
                        CmbQtdePorcao.SelectedIndex = 5

                    Case Else
                        CmbQtdePorcao.SelectedIndex = 0

                End Select

                If Not IsDBNull(dr("medida_caseira")) Then

                    Dim indice As Integer = CInt(dr("medida_caseira"))

                    If indice >= 0 And indice < CmbPorcao.Items.Count Then
                        CmbPorcao.SelectedIndex = indice
                    Else
                        CmbPorcao.SelectedIndex = -1
                    End If

                Else

                    CmbPorcao.SelectedIndex = -1

                End If

                TxtVlrEnergetico.Text = dr("valor_energetico").ToString()
                TxtCarb.Text = dr("carboidratos").ToString()
                TxtAcucarTotais.Text = dr("acucares_totais").ToString()
                TxtAcucarAdc.Text = dr("acucares_adicionados").ToString()
                TxtPoteina.Text = dr("proteinas").ToString()
                TxtGorduraTotais.Text = dr("gorduras_totais").ToString()
                TxtGordurasSaturadas.Text = dr("gorduras_saturadas").ToString()
                TxtGordurasTrans.Text = dr("gorduras_trans").ToString()
                TxtFibraAlimentar.Text = dr("fibras").ToString()
                TxtSodio.Text = dr("sodio").ToString()
                TxtLactose.Text = dr("lactose").ToString()
                TxtGalactose.Text = dr("galactose").ToString()
                LblCodSD.Text = dr("codigo_sd").ToString()

                ChkAcucarAdc.Checked = (dr("alto_acucar").ToString() = "1")
                ChkGordurasSaturadas.Checked = (dr("alto_gordura").ToString() = "1")
                ChkSodio.Checked = (dr("alto_sodio").ToString() = "1")

            End If

            dr.Close()

            conn.Close()

        Catch ex As Exception

            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If

            MessageBox.Show(ex.Message)

        End Try

    End Sub

    Private Sub ExportarParaSd()

        Dim dt As New DataTable()

        Using conn As New MySqlConnection("server=127.0.0.1;port=3307;database=projects;uid=root;pwd=root;")

            conn.Open()

            Dim sql As String =
    "SELECT

i.codigo AS 'Código',
i.descricao AS 'Descrição',
CASE
    WHEN i.tipo='0' THEN 'KG'
    ELSE 'UND'
END AS 'Venda',
i.validade AS 'Validade',
i.cod_nutri AS 'Cod. Nutri',

n.qtd_porcao AS 'Qtd. Porção',
n.unidade AS 'Unidade',
n.medida_inteira AS 'Medida',
n.medida_decimal AS 'Fração',
n.medida_caseira AS 'Tipo Medida',

n.valor_energetico AS 'Energia',
n.carboidratos AS 'Carboidratos',
n.acucares_totais AS 'Açúcares Totais',
n.acucares_adicionados AS 'Açúcares Adicionados',
n.proteinas AS 'Proteínas',
n.gorduras_totais AS 'Gorduras Totais',
n.gorduras_saturadas AS 'Gorduras Saturadas',
n.gorduras_trans AS 'Gorduras Trans',
n.fibras AS 'Fibras',
n.sodio AS 'Sódio',
n.alto_acucar AS 'Alto Açúcar',
n.alto_gordura AS 'Alta Gordura',
n.alto_sodio AS 'Alto Sódio',
n.lactose AS 'Lactose',
n.galactose AS 'Galactose'

FROM balanca_toledo_itens i

LEFT JOIN balanca_toledo_nutri n
ON i.cod_nutri = n.codigo

ORDER BY i.codigo"

            Dim da As New MySqlDataAdapter(sql, conn)

            da.Fill(dt)

        End Using

        Using sfd As New SaveFileDialog

            sfd.Filter = "Excel (*.xlsx)|*.xlsx"
            sfd.FileName = "Produtos Toledo.xlsx"

            If sfd.ShowDialog = DialogResult.OK Then

                Using wb As New XLWorkbook()

                    Dim ws = wb.Worksheets.Add(dt, "Produtos")

                    ws.Columns().AdjustToContents()
                    ws.SheetView.FreezeRows(1)

                    wb.SaveAs(sfd.FileName)

                End Using

                MessageBox.Show("Excel exportado com sucesso!")

            End If

        End Using

    End Sub

    Private Function GerarInsertNutricional(dr As MySqlDataReader) As String

        Dim descricao As String = dr("descricao").ToString().Replace("'", "''")

        Return String.Format(
        "INSERT INTO NUTRICIONAIS (WIDNUTRICIONAL,WNOMENUTRICIONAL,WSTATUS,WPORCAO,WPORCAOUNIDADE,WQTDINTEIRA,WQTDFRACIONADA,WQTDUNIDADE,WVALORENERGETICO,WVALORCARBOIDRATOS,WVALORPROTEINAS,WVALORGORDURASTOTAIS,WVALORGORDURASSATURADAS,WVALORGORDURASTRANS,WVALORFIBRAALIMENTAR,WVALORSODIO,WIDANTIGO,WPORCAO100G,WPORCAOUNIDADE100G,WQTDINTEIRA100G,WQTDFRACIONADA100G,WQTDUNIDADE100G,WVALORENERGETICO100G,WVALORCARBOIDRATOS100G,WVALORPROTEINAS100G,WVALORGORDURASTOTAIS100G,WVALORGORDURASSATURADAS100G,WVALORGORDURASTRANS100G,WVALORFIBRAALIMENTAR100G,WVALORSODIO100G,WVALORLACTOSE100G,WVALORGALACTOSE100G,WPORCOESEMBALAGEM100G,WVALORACUCARESTOTAIS100G,WVALORACUCARESADD100G,WALTOEMACUCARESADD100G,WALTOEMGORDURASSATURADAS100G,WALTOEMSODIO100G,WHABILITARCAMPOSRDC429) VALUES ({0},'{1}','0',0,0,0,0,0,0,0,0,0,0,0,0,0,0,{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},1,{17},{18},'{19}','{20}','{21}','S');",
        dr("codigo_sd"),
        descricao,
        dr("porcoes"),
        dr("unidade"),
        dr("medida_inteira"),
        dr("medida_decimal"),
        dr("medida_caseira"),
        dr("valor_energetico"),
        FBDecimal(dr("carboidratos")),
        FBDecimal(dr("proteinas")),
        FBDecimal(dr("gorduras_totais")),
        FBDecimal(dr("gorduras_saturadas")),
        FBDecimal(dr("gorduras_trans")),
        FBDecimal(dr("fibras")),
        FBDecimal(dr("sodio")),
        FBDecimal(dr("lactose")),
        FBDecimal(dr("galactose")),
        FBDecimal(dr("acucares_totais")),
        FBDecimal(dr("acucares_adicionados")),
        dr("alto_acucar"),
        dr("alto_gordura"),
        dr("alto_sodio"))

    End Function

    Private Sub GerarArquivoNutricionais(pastaDestino As String)

        conn.Open()

        Dim cmd As New MySqlCommand(
        "SELECT * FROM balanca_toledo_nutri ORDER BY codigo_sd",
        conn)

        Dim dr As MySqlDataReader = cmd.ExecuteReader()

        Dim numeroArquivo As Integer = 1
        Dim contador As Integer = 0

        Dim caminhoArquivo As String =
        IO.Path.Combine(
            pastaDestino,
            $"NUTRICIONAIS_{numeroArquivo:000}.sql")

        Dim sw As New IO.StreamWriter(
        caminhoArquivo,
        False,
        System.Text.Encoding.UTF8)

        While dr.Read()

            sw.Write(GerarInsertNutricional(dr) & Environment.NewLine)

            contador += 1

            If contador = 50 Then

                sw.Close()

                numeroArquivo += 1
                contador = 0

                caminhoArquivo =
                IO.Path.Combine(
                    pastaDestino,
                    $"NUTRICIONAIS_{numeroArquivo:000}.sql")

                sw = New IO.StreamWriter(
                caminhoArquivo,
                False,
                System.Text.Encoding.UTF8)

            End If

        End While

        sw.Close()

        dr.Close()
        conn.Close()

    End Sub

    Private Function FBDecimal(valor As Object) As String

        If IsDBNull(valor) OrElse valor Is Nothing Then
            Return "0"
        End If

        Dim numero As Double = CDbl(valor)

        Return numero.ToString("0.####", System.Globalization.CultureInfo.InvariantCulture)

    End Function

    Private Function FBTexto(valor As Object) As String

        If IsDBNull(valor) OrElse valor Is Nothing Then
            Return "''"
        End If

        Return "'" & valor.ToString().Replace("'", "''") & "'"

    End Function

    Private Sub GerarArquivoUpdateNutricionais(pastaDestino As String)

        conn.Open()

        Dim sql As String =
    "SELECT
        i.codigo,
        i.validade,
        n.codigo_sd
     FROM balanca_toledo_itens i
     INNER JOIN balanca_toledo_nutri n
        ON i.cod_nutri = n.codigo
     ORDER BY i.codigo"

        Dim cmd As New MySqlCommand(sql, conn)

        Dim dr As MySqlDataReader = cmd.ExecuteReader()

        Dim numeroArquivo As Integer = 1
        Dim contador As Integer = 0

        Dim caminhoArquivo As String =
        IO.Path.Combine(
            pastaDestino,
            $"ATUALIZA_PRODUTOS_{numeroArquivo:000}.sql")

        Dim sw As New IO.StreamWriter(
        caminhoArquivo,
        False,
        System.Text.Encoding.UTF8)

        While dr.Read()

            sw.WriteLine(
            GerarUpdateProduto(
                CInt(dr("codigo")),
                CInt(dr("codigo_sd")),
                CInt(dr("validade"))
            )
        )

            contador += 1

            If contador = 100 Then

                sw.WriteLine("COMMIT;")
                sw.Close()

                numeroArquivo += 1
                contador = 0

                caminhoArquivo =
                IO.Path.Combine(
                    pastaDestino,
                    $"ATUALIZA_PRODUTOS_{numeroArquivo:000}.sql")

                sw = New IO.StreamWriter(
                caminhoArquivo,
                False,
                System.Text.Encoding.UTF8)

            End If

        End While

        sw.WriteLine("COMMIT;")

        sw.Close()

        dr.Close()
        conn.Close()

    End Sub

    Private Function GerarUpdateProduto(codigoProduto As Integer,
                                    codigoNutricional As Integer,
                                    validade As Integer) As String

        Return String.Format(
        "UPDATE PRODUTOS " &
        "SET WIDNUTRICIONAL = {0}, " &
        "WVALIDADE = {1}, " &
        "WFRACIONADO = 'S' " &
        "WHERE WIDPRODUTO = {2};",
        codigoNutricional,
        validade,
        codigoProduto)

    End Function

#End Region

End Class