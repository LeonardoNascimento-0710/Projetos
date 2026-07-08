Imports System.Drawing

Public Class GerenciadorSprites

    Private Shared ReadOnly Imagens As New Dictionary(Of String, Image)

    Public Shared Sub Carregar()

        Imagens.Clear()

    End Sub

    Public Shared Function Obter(nome As String) As Image

        If Imagens.ContainsKey(nome) Then
            Return Imagens(nome)
        End If

        Return Nothing

    End Function

End Class