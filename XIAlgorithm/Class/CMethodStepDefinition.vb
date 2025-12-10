Imports XISystem
Imports System.Collections.Specialized

Public Class CMethodStepDefinition

    Public Enum xiAlgoReturnType
        xiNothing = 0
        xiResult = 10
        xiBOInstanceGrid = 20
        xiBOInstance = 30


    End Enum

    Public Enum xiMethodType
        xiMethod = 0
        xiScript = 10

    End Enum

    Public Enum xiAlgoErrorType
        xiLogFail = 10

    End Enum

    Public Enum xiAlgoActionType
        xiSetValue = 10
        xiBOCopy = 20
        xiBOSave = 30
        xiBOLoad = 40
        xiCreateBO = 50
        xi1Click = 100
        xiBOToXIValues = 200
        xixiValuesToBO = 210
        xiIterate = 1000
        xiCondition = 1100
        xiConditionNot = 1110
        xiNothing = 0
    End Enum

    Private iMyActionType As xiAlgoActionType
    Public Property iActionType() As xiAlgoActionType
        Get
            Return iMyActionType
        End Get
        Set(ByVal value As xiAlgoActionType)
            iMyActionType = value
        End Set
    End Property

    Private iMyReturnType As xiAlgoReturnType
    Public Property iReturnType() As xiAlgoReturnType
        Get
            Return iMyReturnType
        End Get
        Set(ByVal value As xiAlgoReturnType)
            iMyReturnType = value
        End Set
    End Property

    Private iMyErrorType As xiAlgoErrorType
    Public Property iErrorType() As xiAlgoErrorType
        Get
            Return iMyErrorType
        End Get
        Set(ByVal value As xiAlgoErrorType)
            iMyErrorType = value
        End Set
    End Property

    Private iMyMethodType As xiMethodType
    Public Property iMethodType() As xiMethodType
        Get
            Return iMyMethodType
        End Get
        Set(ByVal value As xiMethodType)
            iMyMethodType = value
        End Set
    End Property

    'Each method has to have various properties

    Private oMethodSteps As New OrderedDictionary  '( (Of String, CMethodStepDefinition)

    Public Property NMethodSteps() As OrderedDictionary '(Of String, CMethodStepDefinition)
        Get
            Return oMethodSteps
        End Get
        Set(ByVal value As OrderedDictionary)
            '(Of String, CMethodStepDefinition))
            oMethodSteps = value
        End Set
    End Property

    Private oMyParentMethod As CMethodStepDefinition
    Public Property oParentMethod() As CMethodStepDefinition
        Get
            Return oMyParentMethod
        End Get
        Set(ByVal value As CMethodStepDefinition)
            oMyParentMethod = value
        End Set
    End Property

    Private iMyChildIndex As Long
    Public Property iChildIndex() As Long
        Get
            Return iMyChildIndex
        End Get
        Set(ByVal value As Long)
            iMyChildIndex = value
        End Set
    End Property

    Public Function AddMethodObject(oMethodToAdd As CMethodStepDefinition) As CResult
        Dim oResult As CResult = New CResult

        Try
            oMethodToAdd.iChildIndex = NMethodSteps.Count
            oMethodSteps.Add(oMethodToAdd.sKey, oMethodToAdd)
            oMethodToAdd.oParentMethod = Me
            oMethodToAdd.oAlgoD = oAlgoD
            oResult.xiStatus = xiEnum.xiFuncResult.xiSuccess
        Catch ex As Exception
            oResult.xiStatus = xiEnum.xiFuncResult.xiError
            oResult.sMessage = ex.Message & " - Stack: " & ex.StackTrace
        End Try


        Return oResult

    End Function

    Public Function NMethodAdd(ByVal sNodeName As String, Optional ByVal sKey As String = "") As CResult
        'essentially we build the key up in here to make it easier
        Dim sConcatKey As String = ""
        Dim oNewM As CMethodStepDefinition = Nothing
        Dim oResult As CResult = New CResult

        Try
            oResult.xiStatus = xiEnum.xiFuncResult.xiInProcess

            If sKey = "" Then
                sConcatKey = sNodeName
            Else
                sConcatKey = sNodeName
            End If

            If oMethodSteps.Contains(sConcatKey.ToLower) Then
                oResult.xiStatus = xiEnum.xiFuncResult.xiError
                oResult.sMessage = "Key: '" & sConcatKey & "' already exists in dictionary"
            Else
                oNewM = NMethod(sNodeName, sKey)
                oResult.oResult = oNewM
                oResult.xiStatus = xiEnum.xiFuncResult.xiSuccess
            End If

        Catch ex As Exception
            oResult.xiStatus = xiEnum.xiFuncResult.xiError
        End Try

        Return oResult
    End Function

    Public Function NMethod(ByVal sNodeName As String, Optional ByVal sKey As String = "") As CMethodStepDefinition
        'essentially we build the key up in here to make it easier
        Dim sConcatKey As String = ""
        Dim sNewKey As String = ""
        Dim oNewM As CMethodStepDefinition = Nothing
        'Dim oCxi As Cxi

        Try


            If sKey = "" Then
                sConcatKey = sNodeName
            Else
                sConcatKey = sNodeName
            End If


            If sConcatKey <> "" Then
                Try
                    If oMethodSteps.Contains(sConcatKey.ToLower) Then
                        NMethod = oMethodSteps(sConcatKey.ToLower)
                    Else
                        oNewM = AddMethod(sConcatKey.ToLower)
                        sNewKey = oNewM.sKey
                        'sNewKey = AddMethod(sConcatKey.ToLower).sKey
                        Try

                            NMethod = oMethodSteps(sNewKey)
                        Catch ex2 As Exception
                            NMethod = Nothing  'serious problem
                        End Try
                    End If
                Catch ex As Exception
                    Debug.Print("XIGenerics.CXI.NNodeMeta.Error")
                    NMethod = Nothing
                End Try
            Else
                NMethod = Nothing  'serious problem
            End If

        Catch ex As Exception
            NMethod = Nothing  'serious problem
        End Try
    End Function



    Function AddMethod(ByVal sMethodName As String) As CMethodStepDefinition

        Dim oNew As New CMethodStepDefinition
        Dim sKey As String
        Dim sFirstChar As String = ""
        Dim sName As String
        Dim bDataTypeAssigned As Boolean = False
        Dim sGlobalKey As String = ""

        Try

            sName = sMethodName
            sKey = sMethodName.ToLower
            If oMethodSteps.Contains(sKey) Or sKey = "" Then
                'TO DO - error or can we use a generated key??
                'sKey = Get_UID()
            End If

            oNew = New CMethodStepDefinition
            'Try
            '    sFirstChar = sName.Substring(0, 1)

            'Catch exName As Exception

            'End Try
            'If bHungarian Then
            '    Select Case sFirstChar
            '        Case "s"
            '            oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiString
            '            bDataTypeAssigned = True
            '        Case "i"
            '            oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiLong
            '        Case "f"
            '            oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiFloat
            '        Case "o"
            '            oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiObject
            '        Case "d"
            '            oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiDate
            '        Case "r"
            '            oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiCurrency
            '    End Select
            'End If

            'If oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiString And bDataTypeAssigned = False Then  'if this is dan notation then chop of first char of name
            'Else
            sName = sName.Substring(1, sName.Length - 1)
            'End If

            '    oNew.oParent = Me
            'oNew.oBaseClass = Me.oBaseClass
            'oNew.sClass = sMethodName
            'oNew.iLevel = Me.iLevel + 1
            'oNew.sUID = Get_UID()
            'oNew.sName = sName
            'If sSpecifiedGivenKey = "" Then
            '    oNew.sGivenKey = NextKey()
            'Else
            '    oNew.sGivenKey = sSpecifiedGivenKey
            'End If
            ''oNew.sGivenKey = NextKey()
            'oNew.xiBaseDataType = tBaseType.xiMeta

            'iMyMetaCount = iMyMetaCount + 1
            'If oCopyFrom Is Nothing Then
            '    'sKey = oNew.oBaseClass.Get_Key(sMethodName, Me, oXMLNode)       ', sNodeId
            '    sGlobalKey = "M." & oNew.sUID
            'Else
            '    'be careful then - why keep keys the same??
            '    sGlobalKey = oCopyFrom.sKey
            'End If

            'oNew.sUID = sGlobalKey        'do this in the get_uid itself
            oNew.sKey = sKey
            oMethodSteps.Add(sKey, oNew)        'keep to the original - in the local collection this is the reference
            'Try
            '    oNew.oBaseClass.oRootDictionary.Add(sGlobalKey, oNew)
            'Catch exKey As Exception
            '    Debug.Print("XIGenerics.CXI.AddMeta." & "Duplicate Key for Meta: " & sName)
            'End Try

            'Try
            '    oNew.oBaseClass.oRootKeyDictionary.Add(oNew.sGivenKey, oNew)
            'Catch exKey2 As Exception
            '    Debug.Print("XIGenerics.CXI.AddMeta." & "Duplicate Given Key for Meta: " & sName)
            'End Try

            'oNew.sKey = sKey

            'If oCopyFrom Is Nothing Then

            'Else
            '    'Debug.Print("Copying")
            'End If

            Return oNew
        Catch ex As Exception
            'Debug.Print("XIGenerics.CXI.AddMeta." & "ERROR IN ADDMETA: " & Err.Description)
            Return oNew  'really needs to be xiResult
        End Try
    End Function

    Public Function NMethodIndex(iIndex As Long) As CResult

        Dim oCResult As CResult = New CResult

        'If 

        Return oCResult

    End Function

    Private sMyKey As String
    Public Property sKey() As String
        Get
            Return sMyKey
        End Get
        Set(ByVal value As String)
            sMyKey = value
        End Set
    End Property

    Private sMyStepName As String
    Public Property sStepName() As String
        Get
            Return sMyStepName
        End Get
        Set(ByVal value As String)
            sMyStepName = value
        End Set
    End Property

    Private sMyScript As String
    Public Property sScript() As String
        Get
            Return sMyScript
        End Get
        Set(ByVal value As String)
            sMyScript = value
        End Set
    End Property

    Private sMyIndent As String
    Public Property sIndent() As String
        Get
            Return sMyIndent
        End Get
        Set(ByVal value As String)
            sMyIndent = value
        End Set
    End Property

    Private iMyIndent As Long
    Public Property iIndent() As Long
        Get
            Return iMyIndent
        End Get
        Set(ByVal value As Long)
            iMyIndent = value
        End Set
    End Property

    Private sMyActionType As String
    Public Property sActionType() As String
        Get
            Return sMyActionType
        End Get
        Set(ByVal value As String)
            sMyActionType = value

            Select Case sMyActionType.ToLower
                Case "setvalue", "m.setvalue", "sv"
                    iActionType = xiAlgoActionType.xiSetValue
                Case "createbo", "m.createbo", "boc"
                    iActionType = xiAlgoActionType.xiCreateBO
                Case "1click", "m.1click", "1cs", "1c"
                    iActionType = xiAlgoActionType.xi1Click
                Case "iterate", "m.iterate", "it", "loop"
                    iActionType = xiAlgoActionType.xiIterate
                Case "boload", "m.boload", "bol"
                    iActionType = xiAlgoActionType.xiBOLoad
                Case "bosave", "m.bosave", "bos"
                    iActionType = xiAlgoActionType.xiBOSave
                Case "bocopy", "m.bocopy", "copy"
                    iActionType = xiAlgoActionType.xiBOCopy
                Case "botoxivalues", "m.botoxivalues", "2vals"
                    iActionType = xiAlgoActionType.xiBOToXIValues
                Case "xivaluestobo", "m.xivaluestobo", "2bo"
                    iActionType = xiAlgoActionType.xixiValuesToBO
                Case "condition", "m.condition", "if", "c"
                    iActionType = xiAlgoActionType.xiCondition
                Case "conditionnot", "m.conditionnot", "ifnot", "cn"
                    iActionType = xiAlgoActionType.xiConditionNot
                Case Else
                    iActionType = xiAlgoActionType.xiNothing
            End Select
        End Set
    End Property

    Private sMyReturnType As String
    Public Property sReturnType() As String
        Get
            Return sMyReturnType
        End Get
        Set(ByVal value As String)
            sMyReturnType = value
            Select Case value.ToLower
                Case "boinstance"
                    iReturnType = xiAlgoReturnType.xiBOInstance
                Case "boinstancegrid"
                    iReturnType = xiAlgoReturnType.xiBOInstanceGrid
                Case Else
                    iReturnType = xiAlgoReturnType.xiResult
            End Select
        End Set
    End Property

    Private sMyErrorType As String
    Public Property sErrorType() As String
        Get
            Return sMyErrorType
        End Get
        Set(ByVal value As String)
            sMyErrorType = value

            Select Case sMyErrorType.ToLower
                Case Else
                    iErrorType = xiAlgoErrorType.xiLogFail
            End Select
        End Set
    End Property

    Private sMyExecute As String
    Public Property sExecute() As String
        Get
            Return sMyExecute
        End Get
        Set(ByVal value As String)

            Dim AsParams() As String
            Dim j As Long
            Dim oCXI As CNodeItem = Nothing


            sMyExecute = value

            'set the params
            'a:m.BOLoad|x:fx.Policy_T,in.PolicyFK  
            '  so in the above, params are not the names but what they are, one is a reference to an inbound param and one is a specific value
            oMyParamSet = New CNodeItem
            AsParams = Split(sMyExecute, ",")
            For j = 0 To UBound(AsParams)
                oCXI = oMyParamSet.NNode("Param_" & j + 1)
                oCXI.sValue = AsParams(j)
                'get the type from the prefix

            Next j

        End Set
    End Property

    'Private oMyResult As Object
    'Public Property oResult() As Object
    '    Get
    '        Return oMyResult
    '    End Get
    '    Set(ByVal value As Object)
    '        oMyResult = value
    '    End Set
    'End Property

    Public Function Imprint() As String
        Dim sConcat As String = ""
        Dim oMethod As CMethodStepDefinition

        For Each oMethod In NMethodSteps.Values
            sConcat = sConcat & oMethod.ImprintSelf
            sConcat = sConcat & oMethod.Imprint()
        Next

        Return sConcat
    End Function

    Public Function ImprintSelf() As String

        Dim sImprint As String = ""
        Dim j As Long

        For j = 1 To iIndent
            sImprint = sImprint & vbTab
        Next
        'TO DO - change to return the interpreted values for error, action etc, not the strings
        sImprint = sImprint & sStepName & " - Return='" & sReturnType & "' - Error = '" & sErrorType & "' - Action='" & sActionType & "' - Execute='" & sExecute & "'" & vbCrLf

        Return sImprint

    End Function

    Private oMyParamSet As CNodeItem = New CNodeItem
    Public Property oParamSet() As CNodeItem
        Get
            Return oMyParamSet
        End Get
        Set(ByVal value As CNodeItem)
            oMyParamSet = value
        End Set
    End Property

    Private oMyAlgoDef As CAlgorithmDefinition
    Public Property oAlgoD() As CAlgorithmDefinition
        Get
            Return oMyAlgoDef
        End Get
        Set(ByVal value As CAlgorithmDefinition)
            oMyAlgoDef = value
        End Set
    End Property


    Public Function Validate() As CResult
        Dim oCResult As CResult = New CResult
        Dim oCMethod As CMethodStepDefinition
        Dim oCR As CResult = Nothing
        Dim sValidationError As String = ""
        Dim bType As Boolean
        Dim bParams As Boolean
        Dim iParamCount As Long = 0
        Dim sParamOrigin As String = ""

        Try
            'so check various things
            Select Case iActionType
                Case xiAlgoActionType.xi1Click
                    If iReturnType = xiAlgoReturnType.xiNothing Then iReturnType = xiAlgoReturnType.xiBOInstanceGrid 'set the default, which is usual
                    If iReturnType <> xiAlgoReturnType.xiBOInstanceGrid Then
                        bType = True
                    End If
                Case xiAlgoActionType.xiBOCopy
                    If iReturnType = xiAlgoReturnType.xiNothing Then iReturnType = xiAlgoReturnType.xiResult 'set the default, which is usual
                    If iReturnType <> xiAlgoReturnType.xiResult Then
                        bType = True
                    End If
                Case xiAlgoActionType.xiBOLoad
                    If iReturnType = xiAlgoReturnType.xiNothing Then iReturnType = xiAlgoReturnType.xiBOInstance 'set the default, which is usual
                    If iReturnType <> xiAlgoReturnType.xiBOInstance Then
                        bType = True
                    End If
                Case xiAlgoActionType.xiBOSave
                    If iReturnType = xiAlgoReturnType.xiNothing Then iReturnType = xiAlgoReturnType.xiResult 'set the default, which is usual
                    If iReturnType <> xiAlgoReturnType.xiResult Then
                        bType = True
                    End If
                Case xiAlgoActionType.xiBOToXIValues
                    If iReturnType = xiAlgoReturnType.xiNothing Then iReturnType = xiAlgoReturnType.xiResult 'set the default, which is usual
                    If iReturnType <> xiAlgoReturnType.xiResult Then
                        bType = True
                    End If
                Case xiAlgoActionType.xiCreateBO
                    If iReturnType = xiAlgoReturnType.xiNothing Then iReturnType = xiAlgoReturnType.xiBOInstance 'set the default, which is usual
                    If iReturnType <> xiAlgoReturnType.xiBOInstance Then
                        bType = True
                    End If
                Case xiAlgoActionType.xiIterate
                    If iReturnType = xiAlgoReturnType.xiNothing Then iReturnType = xiAlgoReturnType.xiResult 'set the default, which is usual
                    If iReturnType <> xiAlgoReturnType.xiResult Then
                        bType = True
                    End If
                Case xiAlgoActionType.xiNothing
                    If iReturnType = xiAlgoReturnType.xiNothing Then iReturnType = xiAlgoReturnType.xiResult 'set the default, which is usual
                    If iReturnType <> xiAlgoReturnType.xiResult Then
                        bType = True
                    End If
                Case xiAlgoActionType.xiSetValue
                    If iReturnType = xiAlgoReturnType.xiNothing Then iReturnType = xiAlgoReturnType.xiResult 'set the default, which is usual
                    If iReturnType <> xiAlgoReturnType.xiResult Then
                        bType = True
                    End If
                Case xiAlgoActionType.xixiValuesToBO
                    If iReturnType = xiAlgoReturnType.xiNothing Then iReturnType = xiAlgoReturnType.xiResult 'set the default, which is usual
                    If iReturnType <> xiAlgoReturnType.xiResult Then
                        bType = True
                    End If
                    'Case xiAlgoActionType.xiIterate
                    'If iReturnType <> xiAlgoReturnType.xiBOInstanceGrid Then
                    '    bType = True
                    'End If
                    'Case xiAlgoActionType.xiIterate
                    'If iReturnType <> xiAlgoReturnType.xiBOInstanceGrid Then
                    '    bType = True
                    'End If
            End Select
            If bType Then sValidationError = sValidationError & "Action Type is " & iActionType.ToString & " but the return type is " & iReturnType.ToString & vbCrLf

            'input params
            ' for now, we are just counting how many. But in fact we could potentially
            '  work out whether the input params are of the correct type also
            Select Case iActionType
                Case xiAlgoActionType.xi1Click
                    iParamCount = 1
                    If oParamSet.iChildCount <> iParamCount Then
                        bParams = True
                    End If
                Case xiAlgoActionType.xiBOCopy
                    iParamCount = 3
                    If oParamSet.iChildCount <> iParamCount Then
                        bParams = True
                    End If
                Case xiAlgoActionType.xiBOLoad
                    iParamCount = 2
                    If oParamSet.iChildCount <> iParamCount Then
                        bParams = True
                    End If
                Case xiAlgoActionType.xiBOSave
                    iParamCount = 1
                    If oParamSet.iChildCount <> iParamCount Then
                        bParams = True
                    End If
                Case xiAlgoActionType.xiBOToXIValues
                    iParamCount = 2
                    If oParamSet.iChildCount <> iParamCount Then
                        bParams = True
                    End If
                Case xiAlgoActionType.xiCreateBO
                    iParamCount = 1
                    If oParamSet.iChildCount <> iParamCount Then
                        bParams = True
                    End If
                Case xiAlgoActionType.xiIterate
                    iParamCount = 1
                    If oParamSet.iChildCount <> iParamCount Then
                        bParams = True
                    End If
                Case xiAlgoActionType.xiNothing
                    iParamCount = 0
                    If oParamSet.iChildCount <> iParamCount Then
                        bParams = True
                    End If
                Case xiAlgoActionType.xiSetValue
                    iParamCount = 2
                    If oParamSet.iChildCount <> iParamCount Then
                        bParams = True
                    End If
                Case xiAlgoActionType.xixiValuesToBO
                    iParamCount = 2
                    If oParamSet.iChildCount <> iParamCount Then
                        bParams = True
                    End If
            End Select
            If bParams Then sValidationError = sValidationError & "Action Type is " & iActionType.ToString & " but the parameter count is " & oParamSet.iChildCount & " and should be " & iParamCount & vbCrLf

            Dim bValidInParam As Boolean
            For Each oXIParam In oParamSet.NNodeItems.Values
                bValidInParam = False
                sParamOrigin = oXIParam.sValue.ToLower
                If sParamOrigin.Length > 1 Then
                    If Left(sParamOrigin, 2) = "p." Then
                        For Each oInParam In oAlgoD.oXIParameters.NNodeItems.Values
                            If oInParam.sname.ToLower = sParamOrigin Then
                                bValidInParam = True
                            End If
                        Next
                    Else
                        'for now, assume it is valid
                        bValidInParam = True
                    End If
                End If
                If bValidInParam = False Then
                    sValidationError = sValidationError & "Parameter " & oXIParam.sValue & " expected but does not exist in algorithm inbound parameters" & vbCrLf
                End If
            Next


            If sValidationError <> "" Then
                oCResult.xiStatus = xiEnum.xiFuncResult.xiError
                oCResult.sMessage = "Method: " & sStepName & " - Validation Errors: " & vbCrLf & sValidationError
            End If

            'children
            For Each oCMethod In oMethodSteps.Values
                oCR = oCMethod.Validate
                If oCR.xiStatus <> xiEnum.xiFuncResult.xiSuccess Then
                    oCResult.xiStatus = xiEnum.xiFuncResult.xiError

                    oCResult.sMessage = oCResult.sMessage & oCR.sMessage & vbCrLf
                End If
            Next
        Catch ex As Exception
            oCResult.xiStatus = xiEnum.xiFuncResult.xiError
            oCResult.sMessage = ex.Message & " - Stack: " & ex.StackTrace
        End Try


        Return oCResult

    End Function

End Class
