Imports System.Drawing

Public Enum ModoTemaAplicacao

    Escuro = 0
    Claro = 1

End Enum

Public Class PreferenciasAplicacao

    Public Property AutosaveAtivo As Boolean =
        True

    Public Property IntervaloAutosaveSegundos As Integer =
        60

    Public Property ResolucaoExportacao As Integer =
        2560

    Public Property ModoTema As ModoTemaAplicacao =
        ModoTemaAplicacao.Escuro

    Public Property CorPrincipalArgb As Integer =
        Color.FromArgb(
            134,
            29,
            29).ToArgb()

End Class