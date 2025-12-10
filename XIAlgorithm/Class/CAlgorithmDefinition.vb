Imports xiGenerics
Imports XISystem

Public Class CAlgorithmDefinition

    'Private oParameters As New Dictionary(Of String, Cxi)

    'Public Property NParameters() As Dictionary(Of String, Cxi)
    '    Get
    '        Return oParameters
    '    End Get
    '    Set(ByVal value As Dictionary(Of String, Cxi))
    '        oParameters = value
    '    End Set
    'End Property

    Private oMyXIParams As New CNodeItem
    Public Property oXIParameters() As CNodeItem
        Get
            Return oMyXIParams
        End Get
        Set(ByVal value As CNodeItem)
            oMyXIParams = value
        End Set
    End Property

    Private oMyMethodDefinition As New CMethodStepDefinition
    Public Property oMethodDefinition() As CMethodStepDefinition
        Get
            Return oMyMethodDefinition
        End Get
        Set(ByVal value As CMethodStepDefinition)
            oMyMethodDefinition = value
        End Set
    End Property

    Private oMyMethodOM As New CMethodStepDefinition
    Public Property oMethodOM() As CMethodStepDefinition
        Get
            Return oMyMethodOM
        End Get
        Set(ByVal value As CMethodStepDefinition)
            oMyMethodOM = value
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

    Private bMyInitialised As Boolean
    Public Property bInitialised() As Boolean
        Get
            Return bMyInitialised
        End Get
        Set(ByVal value As Boolean)
            bMyInitialised = value
        End Set
    End Property

    Private bMyCompiled As Boolean
    Public Property bCompiled() As Boolean
        Get
            Return bMyCompiled
        End Get
        Set(ByVal value As Boolean)
            bMyCompiled = value
        End Set
    End Property

    Private bMyOM As Boolean
    Public Property bCOM() As Boolean
        Get
            Return bMyOM
        End Get
        Set(ByVal value As Boolean)
            bMyOM = value
        End Set
    End Property

    Public Function Imprint() As String
        Dim sConcat As String = ""

        sConcat = oMethodOM.Imprint

        Return sConcat
    End Function



    Public Function CompileOM() As CResult
        Dim oCResult As CResult = New CResult
        Dim oCMethod As CMethodStepDefinition
        Dim oParentMethod As CMethodStepDefinition
        Dim iIndentDiff As Long
        Dim j As Long

        Try
            'so the 2 base nodes are the same, now add on any sub nodes which are side by side in the methoddef
            oMethodOM.oAlgoD = Me
            oParentMethod = oMethodOM
            For Each oCMethod In oMethodDefinition.NMethodSteps.Values  'Make sure we are not associated with the base node oMethodDefinition, we are just iterating through all his methods
                iIndentDiff = oCMethod.iIndent - oParentMethod.iIndent
                If iIndentDiff = 1 Then  'child
                    oParentMethod.AddMethodObject(oCMethod)
                ElseIf iIndentDiff > 1 Then
                    oCResult.xiStatus = xiEnum.xiFuncResult.xiLogicalError
                    oCResult.sMessage = "Cannot add line: '" & oCMethod.sStepName & "' - Indent differential is: " & iIndentDiff
                    Return oCResult
                    Exit Function
                Else 'zero or less
                    'chain back up the tree
                    For j = 0 To (iIndentDiff * -1)
                        If Not oParentMethod Is Nothing Then
                            If Not oParentMethod.oParentMethod Is Nothing Then oParentMethod = oParentMethod.oParentMethod
                        End If
                    Next j
                    If Not oParentMethod Is Nothing Then
                        oParentMethod.AddMethodObject(oCMethod)
                    Else
                        'Assume a base method (but should we?
                        oParentMethod = oMethodOM
                        oParentMethod.AddMethodObject(oCMethod)
                        '''We got to a point where there is no parent
                        ''oCResult.xiStatus = xiEnum.xiFuncResult.xiLogicalError
                        ''oCResult.sMessage = "Cannot add line: '" & oCMethod.sStepName & "' - Indent differential is: " & iIndentDiff & " - Cannot get to a parent for indent '" & oCMethod.iIndent & "' [" & oCMethod.sIndent & "]"
                        ''Exit Function
                    End If

                End If

                oParentMethod = oCMethod
            Next
        Catch ex As Exception
            oCResult.xiStatus = xiEnum.xiFuncResult.xiError
            oCResult.sMessage = ex.Message & " - Stack: " & ex.StackTrace
        End Try


        Return oCResult

    End Function

    Public Function ValidateOM() As CResult
        Dim oCResult As CResult = New CResult
        Dim oCMethod As CMethodStepDefinition
        Dim oCR As CResult = Nothing

        Try

            For Each oCMethod In oMethodDefinition.NMethodSteps.Values  'Make sure we are not associated with the base node oMethodDefinition, we are just iterating through all his methods
                oCR = oCMethod.Validate
                If oCR.xiStatus <> xiEnum.xiFuncResult.xiSuccess Then
                    oCResult.xiStatus = xiEnum.xiFuncResult.xiError

                    oCResult.sMessage = "Errors in Compilation: Validation" & vbCrLf & oCR.sMessage
                End If
            Next
        Catch ex As Exception
            oCResult.xiStatus = xiEnum.xiFuncResult.xiError
            oCResult.sMessage = ex.Message & " - Stack: " & ex.StackTrace
        End Try


        Return oCResult

    End Function
End Class
