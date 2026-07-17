Imports System.Collections.Generic

Namespace Classes

    Public Class EstadoCampo

        Public Property Objetos As List(Of EstadoObjetoCampo)

        Public Property EstiloVisualValor As Integer =
            CInt(
                EstiloVisualCampo.Estadio)

        Public Property IntensidadeTextura As Integer =
            35

        Public Property FaixasGramaVisiveis As Boolean =
            True

        Public Property SombrasCampoAtivas As Boolean =
            True

        Public Sub New()

            Objetos =
                New List(Of EstadoObjetoCampo)()

        End Sub

    End Class

End Namespace