Public Class PromocaoTexto

    Public Const VozMasculina As String = "Masculina"
    Public Const VozFeminina As String = "Feminina"

    Public Property Texto As String = ""

    Public Property Voz As String = VozMasculina

    Public Function Copiar() As PromocaoTexto

        Return New PromocaoTexto With {
            .Texto = Texto,
            .Voz = Voz
        }

    End Function

End Class
