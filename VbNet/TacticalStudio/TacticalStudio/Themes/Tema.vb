Imports System.Drawing

Public NotInheritable Class Tema

    Private Sub New()
    End Sub

#Region "Cores"


    Public Shared ReadOnly CorPrimaria As Color = Color.FromArgb(134, 29, 29)


    Public Shared ReadOnly Fundo As Color = Color.FromArgb(28, 28, 28)


    Public Shared ReadOnly Painel As Color = Color.FromArgb(40, 40, 40)


    Public Shared ReadOnly Barra As Color = Color.Black


    Public Shared ReadOnly Texto As Color = Color.White


    Public Shared ReadOnly Borda As Color = Color.FromArgb(60, 60, 60)

#End Region

#Region "Fontes"

    Public Shared ReadOnly FontePadrao As New Font("Segoe UI", 9.0!, FontStyle.Regular)

    Public Shared ReadOnly FonteTitulo As New Font("Segoe UI", 10.0!, FontStyle.Bold)

#End Region

End Class