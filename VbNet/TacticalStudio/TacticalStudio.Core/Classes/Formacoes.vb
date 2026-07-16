Public NotInheritable Class Formacoes

    Private Sub New()
    End Sub

    Public Shared Function ObterTodas() As List(Of ModeloFormacao)

        Return New List(Of ModeloFormacao) From {
            CriarFormacao433(),
            CriarFormacao442(),
            CriarFormacao352(),
            CriarFormacao4231()
        }

    End Function

    Public Shared Function ObterPorId(
        idFormacao As String) As ModeloFormacao

        If String.IsNullOrWhiteSpace(
            idFormacao) Then

            Return Nothing

        End If

        For Each formacao As ModeloFormacao
            In ObterTodas()

            If String.Equals(
                formacao.Id,
                idFormacao,
                StringComparison.OrdinalIgnoreCase) Then

                Return formacao

            End If

        Next

        Return Nothing

    End Function

    Private Shared Function CriarFormacao433() As ModeloFormacao

        Dim formacao As New ModeloFormacao With {
            .Id = "4-3-3",
            .Nome = "4-3-3",
            .Descricao = "Quatro defensores, três meio-campistas e três atacantes."
        }

        AdicionarPosicao(
            formacao,
            1,
            "Goleiro",
            50.0F,
            92.0F)

        AdicionarPosicao(
            formacao,
            2,
            "Lateral direito",
            82.0F,
            75.0F)

        AdicionarPosicao(
            formacao,
            3,
            "Zagueiro direito",
            61.0F,
            79.0F)

        AdicionarPosicao(
            formacao,
            4,
            "Zagueiro esquerdo",
            39.0F,
            79.0F)

        AdicionarPosicao(
            formacao,
            6,
            "Lateral esquerdo",
            18.0F,
            75.0F)

        AdicionarPosicao(
            formacao,
            5,
            "Volante",
            50.0F,
            61.0F)

        AdicionarPosicao(
            formacao,
            8,
            "Meia direito",
            68.0F,
            51.0F)

        AdicionarPosicao(
            formacao,
            10,
            "Meia esquerdo",
            32.0F,
            51.0F)

        AdicionarPosicao(
            formacao,
            7,
            "Ponta direita",
            80.0F,
            27.0F)

        AdicionarPosicao(
            formacao,
            9,
            "Centroavante",
            50.0F,
            19.0F)

        AdicionarPosicao(
            formacao,
            11,
            "Ponta esquerda",
            20.0F,
            27.0F)

        Return formacao

    End Function

    Private Shared Function CriarFormacao442() As ModeloFormacao

        Dim formacao As New ModeloFormacao With {
            .Id = "4-4-2",
            .Nome = "4-4-2",
            .Descricao = "Quatro defensores, quatro meio-campistas e dois atacantes."
        }

        AdicionarPosicao(
            formacao,
            1,
            "Goleiro",
            50.0F,
            92.0F)

        AdicionarPosicao(
            formacao,
            2,
            "Lateral direito",
            82.0F,
            75.0F)

        AdicionarPosicao(
            formacao,
            3,
            "Zagueiro direito",
            61.0F,
            79.0F)

        AdicionarPosicao(
            formacao,
            4,
            "Zagueiro esquerdo",
            39.0F,
            79.0F)

        AdicionarPosicao(
            formacao,
            6,
            "Lateral esquerdo",
            18.0F,
            75.0F)

        AdicionarPosicao(
            formacao,
            7,
            "Meia direita",
            80.0F,
            51.0F)

        AdicionarPosicao(
            formacao,
            5,
            "Meio-campista direito",
            61.0F,
            55.0F)

        AdicionarPosicao(
            formacao,
            8,
            "Meio-campista esquerdo",
            39.0F,
            55.0F)

        AdicionarPosicao(
            formacao,
            11,
            "Meia esquerda",
            20.0F,
            51.0F)

        AdicionarPosicao(
            formacao,
            9,
            "Atacante direito",
            62.0F,
            23.0F)

        AdicionarPosicao(
            formacao,
            10,
            "Atacante esquerdo",
            38.0F,
            23.0F)

        Return formacao

    End Function

    Private Shared Function CriarFormacao352() As ModeloFormacao

        Dim formacao As New ModeloFormacao With {
            .Id = "3-5-2",
            .Nome = "3-5-2",
            .Descricao = "Três defensores, cinco meio-campistas e dois atacantes."
        }

        AdicionarPosicao(
            formacao,
            1,
            "Goleiro",
            50.0F,
            92.0F)

        AdicionarPosicao(
            formacao,
            3,
            "Zagueiro direito",
            68.0F,
            77.0F)

        AdicionarPosicao(
            formacao,
            4,
            "Zagueiro central",
            50.0F,
            81.0F)

        AdicionarPosicao(
            formacao,
            6,
            "Zagueiro esquerdo",
            32.0F,
            77.0F)

        AdicionarPosicao(
            formacao,
            2,
            "Ala direita",
            84.0F,
            51.0F)

        AdicionarPosicao(
            formacao,
            5,
            "Volante",
            50.0F,
            61.0F)

        AdicionarPosicao(
            formacao,
            8,
            "Meia direito",
            65.0F,
            47.0F)

        AdicionarPosicao(
            formacao,
            10,
            "Meia esquerdo",
            35.0F,
            47.0F)

        AdicionarPosicao(
            formacao,
            11,
            "Ala esquerda",
            16.0F,
            51.0F)

        AdicionarPosicao(
            formacao,
            9,
            "Atacante direito",
            62.0F,
            22.0F)

        AdicionarPosicao(
            formacao,
            7,
            "Atacante esquerdo",
            38.0F,
            22.0F)

        Return formacao

    End Function

    Private Shared Function CriarFormacao4231() As ModeloFormacao

        Dim formacao As New ModeloFormacao With {
            .Id = "4-2-3-1",
            .Nome = "4-2-3-1",
            .Descricao = "Quatro defensores, dois volantes, três meias e um atacante."
        }

        AdicionarPosicao(
            formacao,
            1,
            "Goleiro",
            50.0F,
            92.0F)

        AdicionarPosicao(
            formacao,
            2,
            "Lateral direito",
            82.0F,
            75.0F)

        AdicionarPosicao(
            formacao,
            3,
            "Zagueiro direito",
            61.0F,
            79.0F)

        AdicionarPosicao(
            formacao,
            4,
            "Zagueiro esquerdo",
            39.0F,
            79.0F)

        AdicionarPosicao(
            formacao,
            6,
            "Lateral esquerdo",
            18.0F,
            75.0F)

        AdicionarPosicao(
            formacao,
            5,
            "Volante direito",
            61.0F,
            61.0F)

        AdicionarPosicao(
            formacao,
            8,
            "Volante esquerdo",
            39.0F,
            61.0F)

        AdicionarPosicao(
            formacao,
            7,
            "Meia direita",
            78.0F,
            40.0F)

        AdicionarPosicao(
            formacao,
            10,
            "Meia central",
            50.0F,
            43.0F)

        AdicionarPosicao(
            formacao,
            11,
            "Meia esquerda",
            22.0F,
            40.0F)

        AdicionarPosicao(
            formacao,
            9,
            "Centroavante",
            50.0F,
            19.0F)

        Return formacao

    End Function

    Private Shared Sub AdicionarPosicao(
        formacao As ModeloFormacao,
        numero As Integer,
        nome As String,
        xPercentual As Single,
        yPercentual As Single)

        If formacao Is Nothing Then
            Exit Sub
        End If

        formacao.Posicoes.Add(
            New PosicaoFormacao With {
                .Numero = numero,
                .Nome = nome,
                .XPercentual = LimitarPercentual(
                    xPercentual),
                .YPercentual = LimitarPercentual(
                    yPercentual)
            })

    End Sub

    Private Shared Function LimitarPercentual(
        valor As Single) As Single

        Return Math.Max(
            0.0F,
            Math.Min(
                100.0F,
                valor))

    End Function

End Class