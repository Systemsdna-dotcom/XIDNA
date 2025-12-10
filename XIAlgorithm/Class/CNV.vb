Public Class CNV

    Private sName As String = ""
    Private sValue As String = ""
    Private sType As String = ""

    Private oNNVs As New Dictionary(Of String, CNV)

    Public Property NNVs() As Dictionary(Of String, CNV)
        Get
            Return oNNVs
        End Get
        Set(ByVal value As Dictionary(Of String, CNV))
            oNNVs = value
        End Set
    End Property

    Public Property Name() As String
        Get
            Return sName
        End Get
        Set(ByVal value As String)
            sName = value
        End Set
    End Property

    Public Property Value() As String
        Get
            Return sValue
        End Get
        Set(ByVal value As String)
            sValue = value
        End Set
    End Property

    Public Property Type() As String
        Get
            Return sType
        End Get
        Set(ByVal value As String)
            sType = value
        End Set
    End Property

End Class
