Public Class CBOPlaceholder


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
