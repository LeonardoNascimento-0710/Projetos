Imports System.Collections.Generic

Namespace Classes

    Public Class EstadoCampo

        Public Property Objetos As List(Of EstadoObjetoCampo)

        Public Sub New()

            Objetos =
                New List(Of EstadoObjetoCampo)()

        End Sub

    End Class

End Namespace