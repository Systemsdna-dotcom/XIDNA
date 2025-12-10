Public Class BOInstanceGridPlaceholder

    Private oMyBOInstances As New Dictionary(Of String, CBOPlaceholder)
    Public Property oBOInstances() As Dictionary(Of String, CBOPlaceholder)
        Get
            Return oMyBOInstances
        End Get
        Set(ByVal value As Dictionary(Of String, CBOPlaceholder))
            oMyBOInstances = value
        End Set
    End Property

    Private sMyName As String
    Public Property sName() As String
        Get
            Return sMyName
        End Get
        Set(ByVal value As String)
            sMyName = value
        End Set
    End Property
End Class
