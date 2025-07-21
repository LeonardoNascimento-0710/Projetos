Public Class FrmAgenda
    ' Listas para armazenar os dados de Nome e Horário
    Private nomes As List(Of String)
    Private horarios As List(Of DateTime)

    ' Evento de carregamento do formulário
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Inicializa as listas de dados (isso pode vir de um banco de dados ou outro lugar)
        nomes = New List(Of String) From {
            "João", "Maria", "Pedro", "Ana", "Carlos"
        }

        horarios = New List(Of DateTime) From {
            New DateTime(2025, 1, 19, 10, 30, 0),
            New DateTime(2025, 1, 20, 14, 0, 0),
            New DateTime(2025, 1, 20, 16, 45, 0),
            New DateTime(2025, 1, 21, 9, 0, 0),
            New DateTime(2025, 1, 20, 11, 15, 0)
        }

        ' Adiciona as colunas ao DataGridView
        DgDados.Columns.Add("Nome", "Nome")
        DgDados.Columns.Add("Horario", "Horário")

        ' Preenche o DataGridView com todos os dados ao carregar o formulário
        PreencherDataGridView()
    End Sub

    ' Função para preencher o DataGridView com base na data selecionada
    Private Sub PreencherDataGridView()
        ' Limpa o DataGridView antes de adicionar novos itens
        DgDados.Rows.Clear()

        ' Filtra os dados pela data selecionada no DateTimePicker (apenas data, sem considerar hora)
        Dim dataSelecionada As DateTime = DtData.Value.Date

        ' Percorre as listas de nomes e horários
        For i As Integer = 0 To nomes.Count - 1
            ' Verifica se a data do horário corresponde à data selecionada
            If horarios(i).Date = dataSelecionada Then
                ' Adiciona a linha no DataGridView com nome e horário
                DgDados.Rows.Add(nomes(i), horarios(i).ToString("HH:mm"))
            End If
        Next
    End Sub

    ' Evento do DateTimePicker para filtrar os dados ao mudar a data
    Private Sub dtdata_ValueChanged(sender As Object, e As EventArgs) Handles DtData.ValueChanged
        PreencherDataGridView()
    End Sub

    ' Evento para capturar quando o valor do horário for alterado na célula do DataGridView
    Private Sub DataGridView1_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles DgDados.CellValueChanged
        ' Verifica se a célula editada foi da coluna de horário
        If e.ColumnIndex = 1 Then ' Índice da coluna "Horário"
            ' Captura o novo valor da célula
            Dim novoHorario As String = DgDados.Rows(e.RowIndex).Cells(e.ColumnIndex).Value.ToString()

            ' Tenta converter o valor para DateTime (com formato HH:mm)
            Dim horarioConvertido As DateTime
            If DateTime.TryParseExact(novoHorario, "HH:mm", Nothing, Globalization.DateTimeStyles.None, horarioConvertido) Then
                ' Atualiza a lista de horários com a nova data/hora
                horarios(e.RowIndex) = horarioConvertido
            Else
                ' Se a conversão falhar, exibe uma mensagem de erro
                MessageBox.Show("Horário inválido. Use o formato HH:mm.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End If
    End Sub

    ' Evento para garantir que o valor seja alterado antes de salvar no DataGridView
    Private Sub DataGridView1_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DgDados.CellEndEdit
        ' Salva qualquer alteração quando a edição da célula terminar
        ' Isso também é onde você pode fazer o armazenamento ou gravação no banco de dados
    End Sub


End Class
