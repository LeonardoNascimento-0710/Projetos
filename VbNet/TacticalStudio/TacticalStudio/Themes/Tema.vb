Imports System.Drawing

Public NotInheritable Class Tema

    Private Shared _modoTema As ModoTemaAplicacao =
        ModoTemaAplicacao.Escuro

    Private Shared _corPrimaria As Color =
        Color.FromArgb(
            134,
            29,
            29)

    Private Shared ReadOnly _fontePadrao As New Font(
        "Segoe UI",
        9.0F,
        FontStyle.Regular)

    Private Sub New()
    End Sub

    Public Shared Sub AplicarPreferencias(
        preferencias As PreferenciasAplicacao)

        If preferencias Is Nothing Then
            Exit Sub
        End If

        _modoTema =
            preferencias.ModoTema

        Dim novaCor As Color =
            Color.FromArgb(
                preferencias.CorPrincipalArgb)

        If novaCor.A = 0 Then

            novaCor =
                Color.FromArgb(
                    134,
                    29,
                    29)

        End If

        _corPrimaria =
            Color.FromArgb(
                255,
                novaCor.R,
                novaCor.G,
                novaCor.B)

    End Sub

    Public Shared ReadOnly Property ModoAtual As ModoTemaAplicacao

        Get
            Return _modoTema
        End Get

    End Property

    Public Shared ReadOnly Property CorPrimaria As Color

        Get
            Return _corPrimaria
        End Get

    End Property

    Public Shared ReadOnly Property Fundo As Color

        Get

            If _modoTema =
               ModoTemaAplicacao.Claro Then

                Return Color.FromArgb(
                    235,
                    235,
                    235)

            End If

            Return Color.FromArgb(
                28,
                28,
                28)

        End Get

    End Property

    Public Shared ReadOnly Property Painel As Color

        Get

            If _modoTema =
               ModoTemaAplicacao.Claro Then

                Return Color.FromArgb(
                    250,
                    250,
                    250)

            End If

            Return Color.FromArgb(
                38,
                38,
                38)

        End Get

    End Property

    Public Shared ReadOnly Property PainelHover As Color

        Get

            If _modoTema =
               ModoTemaAplicacao.Claro Then

                Return Color.FromArgb(
                    220,
                    220,
                    220)

            End If

            Return Color.FromArgb(
                55,
                55,
                55)

        End Get

    End Property

    Public Shared ReadOnly Property Texto As Color

        Get

            If _modoTema =
               ModoTemaAplicacao.Claro Then

                Return Color.FromArgb(
                    35,
                    35,
                    35)

            End If

            Return Color.WhiteSmoke

        End Get

    End Property

    Public Shared ReadOnly Property TextoSecundario As Color

        Get

            If _modoTema =
               ModoTemaAplicacao.Claro Then

                Return Color.FromArgb(
                    90,
                    90,
                    90)

            End If

            Return Color.Silver

        End Get

    End Property

    Public Shared ReadOnly Property Borda As Color

        Get

            If _modoTema =
               ModoTemaAplicacao.Claro Then

                Return Color.FromArgb(
                    185,
                    185,
                    185)

            End If

            Return Color.FromArgb(
                70,
                70,
                70)

        End Get

    End Property

    Public Shared ReadOnly Property CampoEntrada As Color

        Get

            If _modoTema =
               ModoTemaAplicacao.Claro Then

                Return Color.White

            End If

            Return Color.FromArgb(
                50,
                50,
                50)

        End Get

    End Property

    Public Shared ReadOnly Property TextoCampo As Color

        Get

            If _modoTema =
               ModoTemaAplicacao.Claro Then

                Return Color.FromArgb(
                    25,
                    25,
                    25)

            End If

            Return Color.White

        End Get

    End Property

    Public Shared ReadOnly Property FontePadrao As Font

        Get
            Return _fontePadrao
        End Get

    End Property

End Class