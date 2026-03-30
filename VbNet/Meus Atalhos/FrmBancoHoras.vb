Imports ClosedXML.Excel
Imports MySql.Data.MySqlClient



Public Class FrmBancoHoras
    Private Sub FrmBancoHoras_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CarregarLojas()
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.MaximizeBox = False

    End Sub

#Region "Painel e Visual"

    Private Sub BtnRetornar_Click(sender As Object, e As EventArgs) Handles BtnRetornar.Click
        Dispose
    End Sub

    Private Sub BtnGravar_Click(sender As Object, e As EventArgs) Handles BtnGravar.Click
        SalvarDados()
        CarregarPorLoja()
    End Sub


    Private Sub cmbLoja_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbLoja.SelectedIndexChanged
        CarregarPorLoja()
    End Sub

    Private Sub ChkSemData_CheckedChanged(sender As Object, e As EventArgs) Handles ChkSemData.CheckedChanged
        If ChkSemData.Checked Then

            DtpInicio.Visible = False
            DtpFinal.Visible = False

            TxtSemData.Visible = True
            TxtSemData.Focus()
        Else

            DtpInicio.Visible = True
            DtpFinal.Visible = True

            TxtSemData.Visible = False
            TxtSemData.Clear()
        End If
    End Sub

    Private Sub TxtOs_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TxtOs.KeyPress

        If Not Char.IsDigit(e.KeyChar) AndAlso e.KeyChar <> ChrW(Keys.Back) Then
            e.Handled = True
        End If

    End Sub


#End Region


#Region "Funções"

    Private Function TestarConexao() As Boolean
        Try
            Using conn As New MySqlConnection(
            "server=localhost;database=projects;user id=root;password=root;"
        )
                conn.Open()
                Return True
            End Using
        Catch ex As Exception
            MessageBox.Show("Falha na conexão com o banco:" & vbCrLf & ex.Message)
            Return False
        End Try
    End Function

    Private Sub ObterInicioFim(ByRef inicioValor As String, ByRef fimValor As String)
        If ChkSemData.Checked Then

            inicioValor = TxtSemData.Text
            fimValor = TxtSemData.Text
        Else
            ' Datas
            inicioValor = DtpInicio.Value.ToString("dd/MM/yyyy")
            fimValor = DtpFinal.Value.ToString("dd/MM/yyyy")
        End If
    End Sub
    Private Sub CarregarLojas()
        Try
            Using conn As New MySqlConnection(
            "server=localhost;database=projects;user id=root;password=root;"
        )
                conn.Open()

                Dim sql As String = "SELECT DISTINCT loja FROM banco_horas ORDER BY loja"

                Using cmd As New MySqlCommand(sql, conn)
                    Using dr As MySqlDataReader = cmd.ExecuteReader()

                        CmbLoja.Items.Clear()

                        While dr.Read()
                            CmbLoja.Items.Add(dr("loja").ToString())
                        End While

                    End Using
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Erro ao carregar lojas:" & vbCrLf & ex.Message)
        End Try
    End Sub

    Private Sub SalvarDados()

        If Not TestarConexao() Then Exit Sub

        Dim inicioValor As String = ""
        Dim fimValor As String = ""

        ObterInicioFim(inicioValor, fimValor)

        Try
            Using conn As New MySqlConnection(
            "server=localhost;database=projects;user id=root;password=root;"
        )
                conn.Open()

                Dim sql As String =
            "INSERT INTO banco_horas
            (loja, inicio, fim, ordem_servico, dias_fora, horas_extra, motivo, dia_utilizado, utilizados)
            VALUES
            (@loja, @inicio, @fim, @ordem_servico, @dias_fora, @horas_extra, @motivo, @dia_utilizado, @utilizados)"

                Using cmd As New MySqlCommand(sql, conn)

                    cmd.Parameters.AddWithValue("@loja", CmbLoja.Text)
                    cmd.Parameters.AddWithValue("@inicio", inicioValor)
                    cmd.Parameters.AddWithValue("@fim", fimValor)
                    cmd.Parameters.AddWithValue("@ordem_servico",
                    If(String.IsNullOrWhiteSpace(TxtOs.Text), 0, Convert.ToInt32(TxtOs.Text)))
                    cmd.Parameters.AddWithValue("@dias_fora",
                    If(String.IsNullOrWhiteSpace(TxtDiasFora.Text), 0, Convert.ToInt32(TxtDiasFora.Text)))
                    cmd.Parameters.AddWithValue("@horas_extra", TxtExtra.Text)
                    cmd.Parameters.AddWithValue("@motivo", TxtMotivo.Text)
                    cmd.Parameters.AddWithValue("@dia_utilizado", ChkUtilizado.Checked)
                    cmd.Parameters.AddWithValue("@utilizados", TxtQntdUlt.Text)

                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("Registro salvo com sucesso!")

            Limpa()
            CarregarLojas()

        Catch ex As Exception
            MessageBox.Show("Erro ao salvar:" & vbCrLf & ex.Message)
        End Try

    End Sub


    Private Sub dgvDados_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) _
    Handles DtHist.CellEndEdit

        AtualizarRegistro(e.RowIndex)

        MessageBox.Show("Registro atualizado com sucesso!")

    End Sub
    Private Sub AtualizarRegistro(rowIndex As Integer)

        If rowIndex < 0 Then Exit Sub
        If DtHist.Rows(rowIndex).IsNewRow Then Exit Sub
        If Not TestarConexao() Then Exit Sub

        Try
            Dim row = DtHist.Rows(rowIndex)

            Dim id As Integer = Convert.ToInt32(row.Cells("id").Value)

            Dim inicio As String =
            If(row.Cells("inicio").Value Is Nothing OrElse IsDBNull(row.Cells("inicio").Value),
               "",
               row.Cells("inicio").Value.ToString())

            Dim fim As String =
            If(row.Cells("fim").Value Is Nothing OrElse IsDBNull(row.Cells("fim").Value),
               "",
               row.Cells("fim").Value.ToString())

            Dim ordem_servico As Integer =
            If(row.Cells("ordem_servico").Value Is Nothing OrElse IsDBNull(row.Cells("ordem_servico").Value),
               0,
               Convert.ToInt32(row.Cells("ordem_servico").Value))

            Dim diasFora As Integer =
            If(row.Cells("dias_fora").Value Is Nothing OrElse IsDBNull(row.Cells("dias_fora").Value),
               0,
               Convert.ToInt32(row.Cells("dias_fora").Value))

            Dim horasExtra As String =
            If(row.Cells("horas_extra").Value Is Nothing OrElse IsDBNull(row.Cells("horas_extra").Value),
               "",
               row.Cells("horas_extra").Value.ToString())

            Dim motivo As String =
            If(row.Cells("motivo").Value Is Nothing OrElse IsDBNull(row.Cells("motivo").Value),
               "",
               row.Cells("motivo").Value.ToString())

            Dim diaUtilizado As Boolean =
            If(row.Cells("dia_utilizado").Value Is Nothing OrElse IsDBNull(row.Cells("dia_utilizado").Value),
               False,
               Convert.ToBoolean(row.Cells("dia_utilizado").Value))

            Dim utilizados As Integer =
            If(row.Cells("utilizados").Value Is Nothing OrElse IsDBNull(row.Cells("utilizados").Value),
               0,
               Convert.ToInt32(row.Cells("utilizados").Value))

            Using conn As New MySqlConnection(
            "server=localhost;database=projects;user id=root;password=root;"
        )
                conn.Open()

                Dim sql As String =
                "UPDATE banco_horas SET
                    inicio = @inicio,
                    fim = @fim,
                    ordem_servico = @ordem_servico,
                    dias_fora = @dias_fora,
                    horas_extra = @horas_extra,
                    motivo = @motivo,
                    dia_utilizado = @dia_utilizado,
                    utilizados = @utilizados
                 WHERE id = @id"

                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@inicio", inicio)
                    cmd.Parameters.AddWithValue("@fim", fim)
                    cmd.Parameters.AddWithValue("@ordem_servico", ordem_servico)
                    cmd.Parameters.AddWithValue("@dias_fora", diasFora)
                    cmd.Parameters.AddWithValue("@horas_extra", horasExtra)
                    cmd.Parameters.AddWithValue("@motivo", motivo)
                    cmd.Parameters.AddWithValue("@dia_utilizado", diaUtilizado)
                    cmd.Parameters.AddWithValue("@utilizados", utilizados)
                    cmd.Parameters.AddWithValue("@id", id)

                    cmd.ExecuteNonQuery()
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show("Erro ao salvar:" & vbCrLf & ex.Message)
        End Try

    End Sub



    Private Sub CarregarPorLoja()
        Try
            Using conn As New MySqlConnection(
            "server=localhost;database=projects;user id=root;password=root;"
        )
                conn.Open()

                Dim sql As String =
            "SELECT id, inicio, fim, ordem_servico, dias_fora, horas_extra, motivo, dia_utilizado, utilizados
             FROM banco_horas
             WHERE loja = @loja"

                Dim da As New MySqlDataAdapter(sql, conn)
                da.SelectCommand.Parameters.AddWithValue("@loja", CmbLoja.Text)

                Dim dt As New DataTable
                da.Fill(dt)

                DtHist.AutoGenerateColumns = False
                DtHist.Columns.Clear()

                Dim colId As New DataGridViewTextBoxColumn()
                colId.Name = "id"
                colId.DataPropertyName = "id"
                colId.Visible = False
                DtHist.Columns.Add(colId)

                DtHist.Columns.Add(New DataGridViewTextBoxColumn() With {
                .Name = "inicio",
                .HeaderText = "Início",
                .DataPropertyName = "inicio"
            })

                DtHist.Columns.Add(New DataGridViewTextBoxColumn() With {
                .Name = "fim",
                .HeaderText = "Fim",
                .DataPropertyName = "fim"
            })

                DtHist.Columns.Add(New DataGridViewTextBoxColumn() With {
                .Name = "ordem_servico",
                .HeaderText = "OS",
                .DataPropertyName = "ordem_servico"
            })

                DtHist.Columns.Add(New DataGridViewTextBoxColumn() With {
                .Name = "dias_fora",
                .HeaderText = "Dias Fora",
                .DataPropertyName = "dias_fora"
            })


                DtHist.Columns.Add(New DataGridViewTextBoxColumn() With {
                .Name = "horas_extra",
                .HeaderText = "Horas Extra",
                .DataPropertyName = "horas_extra"
            })

                DtHist.Columns.Add(New DataGridViewTextBoxColumn() With {
                .Name = "motivo",
                .HeaderText = "Motivo",
                .DataPropertyName = "motivo"
            })
                DtHist.Columns.Add(New DataGridViewTextBoxColumn() With {
                .Name = "utilizados",
                .HeaderText = "utilizados",
                .DataPropertyName = "utilizados"
            })


                Dim chkCol As New DataGridViewCheckBoxColumn()
                chkCol.Name = "dia_utilizado"
                chkCol.HeaderText = "Dia Utilizado"
                chkCol.DataPropertyName = "dia_utilizado"
                chkCol.TrueValue = True
                chkCol.FalseValue = False
                chkCol.ThreeState = False

                DtHist.Columns.Add(chkCol)
                DtHist.DataSource = dt
                DtHist.Columns("id").Visible = False

            End Using

        Catch ex As Exception
            MessageBox.Show("Erro ao carregar dados: " & ex.Message)
        End Try
    End Sub


    Private Sub Limpa()

        CmbLoja.SelectedItem = -1
        TxtDiasFora.Text = ""
        TxtExtra.Text = ""
        TxtMotivo.Text = ""
        TxtSemData.Text = ""
        TxtOs.Text = ""
        TxtQntdUlt.Text = ""

    End Sub
    Private Sub BtnExportar_Click(sender As Object, e As EventArgs) Handles BtnExportar.Click

        Try
            Dim sfd As New SaveFileDialog With {
                .Filter = "Arquivo Excel (*.xlsx)|*.xlsx",
                .FileName = "banco_horas_" & Date.Now.ToString("yyyyMMdd_HHmmss")
            }

            If sfd.ShowDialog() <> DialogResult.OK Then Exit Sub

            Dim dt As New DataTable

            Using conn As New MySqlConnection(
                "server=localhost;database=projects;user id=root;password=root;"
            )
                conn.Open()

                Dim sql As String = "SELECT * FROM banco_horas"
                Dim da As New MySqlDataAdapter(sql, conn)
                da.Fill(dt)
            End Using

            Using wb As New XLWorkbook()
                Dim ws = wb.Worksheets.Add("BancoHoras")

                ' Cabeçalhos em negrito
                For col As Integer = 0 To dt.Columns.Count - 1
                    ws.Cell(1, col + 1).Value = dt.Columns(col).ColumnName
                    ws.Cell(1, col + 1).Style.Font.Bold = True
                Next

                ' Dados (TRUE/FALSE → SIM/NÃO apenas no Excel)
                For row As Integer = 0 To dt.Rows.Count - 1
                    For col As Integer = 0 To dt.Columns.Count - 1

                        Dim valor = dt.Rows(row)(col)

                        If dt.Columns(col).ColumnName = "dia_utilizado" Then
                            If IsDBNull(valor) OrElse Not Convert.ToBoolean(valor) Then
                                ws.Cell(row + 2, col + 1).Value = "Não"
                            Else
                                ws.Cell(row + 2, col + 1).Value = "Sim"
                            End If
                        Else
                            ws.Cell(row + 2, col + 1).Value =
                                If(IsDBNull(valor), "", valor.ToString())
                        End If

                    Next
                Next

                ws.Columns().AdjustToContents()
                wb.SaveAs(sfd.FileName)
            End Using

            MessageBox.Show("Exportação concluída com sucesso!",
                            "Excel",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show("Erro ao exportar:" & vbCrLf & ex.Message)
        End Try

    End Sub


#End Region

End Class