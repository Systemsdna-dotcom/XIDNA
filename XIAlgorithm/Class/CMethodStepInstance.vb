Imports XISystem

Public Class CMethodStepInstance

    Private oMyResult As Object

    Private sMyKey As String
    Public Property sKey() As String
        Get
            Return sMyKey
        End Get
        Set(ByVal value As String)
            sMyKey = value
        End Set
    End Property

    Private sMyMethodTrace As String
    Public Property sMethodTrace() As String
        Get
            Return sMyMethodTrace
        End Get
        Set(ByVal value As String)
            sMyMethodTrace = value
        End Set
    End Property

    Private oMyDefinition As CMethodStepDefinition
    Public Property oDefinition() As CMethodStepDefinition
        Get
            Return oMyDefinition
        End Get
        Set(ByVal value As CMethodStepDefinition)
            oMyDefinition = value
            iExecuteStatus = xiAlgoExecuteStatus.xiWaiting
        End Set
    End Property

    Private oMyAlgoI As CAlgorithmInstance
    Public Property oAlgoI() As CAlgorithmInstance
        Get
            Return oMyAlgoI
        End Get
        Set(ByVal value As CAlgorithmInstance)
            oMyAlgoI = value
        End Set
    End Property

    Public Property oMethodResult() As Object
        Get
            Return oMyResult
        End Get
        Set(ByVal value As Object)
            oMyResult = value
        End Set
    End Property

    Private iMyExecuteStatus As xiAlgoExecuteStatus
    Public Property iExecuteStatus() As xiAlgoExecuteStatus
        Get
            Return iMyExecuteStatus
        End Get
        Set(ByVal value As xiAlgoExecuteStatus)
            iMyExecuteStatus = value
        End Set
    End Property

    Public Enum xiAlgoExecuteStatus
        xiWaiting = 10
        xiProcessing = 20
        xiComplete = 30
        xiError = 40

    End Enum

    Public Function ConcatDebug(ByRef sDebug As String) As CResult
        Dim oCResult As CResult = New CResult
        Dim oSubMethod As CMethodStepDefinition = Nothing
        Dim oSubMethodI As CMethodStepInstance = Nothing

        sDebug = sDebug & sMyMethodTrace
        For Each oSubMethod In oDefinition.NMethodSteps.Values
            If oAlgoI.oSteps.ContainsKey(oSubMethod.sKey) Then
                oSubMethodI = oAlgoI.oSteps(oSubMethod.sKey)
                oSubMethodI.ConcatDebug(sDebug)

            End If

        Next

        Return oCResult
    End Function

    Public Function Execute() As CResult
        Dim oCResult As CResult = New CResult
        Dim oCR As CResult = Nothing
        Dim iAction As CMethodStepDefinition.xiAlgoActionType = CMethodStepDefinition.xiAlgoActionType.xiNothing
        Dim sExecute As String = ""
        Dim oPlaceholder As New CXICorePlaceholder
        Dim oBOI1 As CBOPlaceholder = Nothing
        Dim oBOI2 As CBOPlaceholder = Nothing
        Dim oAlgoD As CAlgorithmDefinition = Nothing
        Dim oSubMethod As CMethodStepDefinition = Nothing
        Dim oDictionary As Object = Nothing   'dictionary - but can be of anything (should we use collection or something??)
        Dim oSubMethodI As CMethodStepInstance = Nothing
        Dim oSValue1 As String = ""
        Dim oSValue2 As String = ""
        Dim bYes As Boolean

        'so this class knows its parent definition
        '  and it can execute child lines first. The only reason for having child lines is if this is an iterative method (??)
        '  then, depending on its type it can do certain things

        'ALERT TO DO - the compiler should check that the return of a method is compatible with the action
        'ALERT TO DO - in the compiler, are there parameters matching the expected input params for the action
        'ALERT TO DO - if you reference algo inbound params, are they defined in the algo param set?
        'ALERT TO DO - the compiler has to check that an iterate method uses an input that can be iterated (such as a BOInstanceGrid). This might be relatively wide ranging and could also be numbers - eg 1 to 5 or something
        'ALERT TO DO - compiler to make sure each line is named uniquely. Or it can have no name and use a default
        'TO DO - replace CXI with simple NV with the NNodes method - use NMethodDefinitionas example template

        'need to get the parameters
        'oAlgoI.
        'xi.ag|DiaryTemplate|p.PolicyFK,p.dGoLive,p.TemplateID

        oAlgoD = oAlgoI.Definition

        Debug.WriteLine(oDefinition.sStepName)
        sMyMethodTrace = sMyMethodTrace & oDefinition.sStepName & " [" & oDefinition.iActionType.ToString & "] " & ": "

        iAction = oDefinition.iActionType

        If oDefinition.iMethodType = CMethodStepDefinition.xiMethodType.xiMethod Then
            Select Case iAction
                Case CMethodStepDefinition.xiAlgoActionType.xiBOLoad
                    'xi.m|s:-|i:PolicyI|r:BOInstance|e:DBLog,xiFail|a:m.BOLoad|x:Policy_T,PolicyFK
                    'expecting 2 params. These have to be here, the compiler is checking, we can assume
                    'ALERT TO DO - Interpret these inputs - so they might be parameters or some other dynamic value
                    oCR = Get_ParameterObject(oDefinition.oParamSet.Get_NodeByNumber(1).sValue)
                    oSValue1 = oCR.oResult
                    oCR = Get_ParameterObject(oDefinition.oParamSet.Get_NodeByNumber(2).sValue)
                    oSValue2 = oCR.oResult

                    oCR = oPlaceholder.BOLoad(oSValue1, oSValue2) 'oAlgoD.oXIParameters.Get_NodeByNumber(1).sValue, oAlgoD.oMethodOM.oParamSet.Get_NodeByNumber(2).sValue)
                    If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then
                        'assign result to the instance in the OM
                        sMyMethodTrace = sMyMethodTrace & "With '" & oSValue1 & "' and '" & oSValue2 & "' Loaded BO Successfully" & vbCrLf
                        oMethodResult = oCR.oResult
                    End If
                Case CMethodStepDefinition.xiAlgoActionType.xiBOCopy
                    'xi.m|s:--|i:CopyDiary|r:xiR|e:DBLog, xiFail|a: m.BOCopy|x:EachDiaryT, xim.CreateDiaryInstance, xig.CopyLive
                    'Need to get hold of 2 BOs. in the above example, one is from the iteration and one is from a method which created
                    oCR = Get_ParameterObject(oDefinition.oParamSet.Get_NodeByNumber(1).sValue)
                    oBOI1 = oCR.oResult
                    oCR = Get_ParameterObject(oDefinition.oParamSet.Get_NodeByNumber(2).sValue)
                    oBOI2 = oCR.oResult

                    oCR = oPlaceholder.BOCopy(oBOI1, oBOI2)
                    If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then
                        sMyMethodTrace = sMyMethodTrace & "With '" & oDefinition.oParamSet.Get_NodeByNumber(1).sValue & "' and '" & oDefinition.oParamSet.Get_NodeByNumber(2).sValue & "' Copied Successfully" & vbCrLf
                    Else
                        sMyMethodTrace = sMyMethodTrace & "With '" & oDefinition.oParamSet.Get_NodeByNumber(1).sValue & "' and '" & oDefinition.oParamSet.Get_NodeByNumber(2).sValue & "' FAILED" & vbCrLf
                    End If
                    oMethodResult = oCR.xiStatus
                Case CMethodStepDefinition.xiAlgoActionType.xiBOSave
                    'xi.m|s:--|i:InsertDiary|r:xiR|e:DBLog, xiFail|a: m.BOSave|x:xim.CreateDiaryInstance
                    oCR = Get_ParameterObject(oDefinition.oParamSet.Get_NodeByNumber(1).sValue)
                    oBOI1 = oCR.oResult

                    oCR = oPlaceholder.BOSave(oBOI1)
                    If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then
                        sMyMethodTrace = sMyMethodTrace & "'" & oDefinition.oParamSet.Get_NodeByNumber(1).sValue & "' Saved Successfully" & vbCrLf
                    Else
                        sMyMethodTrace = sMyMethodTrace & "'" & oDefinition.oParamSet.Get_NodeByNumber(1).sValue & "' Save FAILED" & vbCrLf
                    End If
                    oMethodResult = oCR.xiStatus

                Case CMethodStepDefinition.xiAlgoActionType.xiCreateBO
                    'xi.m|s:--|i:CreateDiaryInstance|r:BOInstance|e:DBLog, xiFail|a: m.CreateBO|x:bod.Diary_T
                    oCR = Get_ParameterObject(oDefinition.oParamSet.Get_NodeByNumber(1).sValue)
                    oSValue1 = oCR.oResult
                    oCR = oPlaceholder.BOCreate(oSValue1)
                    If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then
                        'assign result to the instance in the OM
                        oMethodResult = oCR.oResult
                        sMyMethodTrace = sMyMethodTrace & "'" & oDefinition.oParamSet.Get_NodeByNumber(1).sValue & "' Created Successfully" & vbCrLf
                    Else
                        sMyMethodTrace = sMyMethodTrace & "'" & oDefinition.oParamSet.Get_NodeByNumber(1).sValue & "' Create FAILED" & vbCrLf
                    End If

                Case CMethodStepDefinition.xiAlgoActionType.xiCondition, CMethodStepDefinition.xiAlgoActionType.xiConditionNot
                    'xi.m|s:--|i:IfClient7|a:m.Condition|sc:{if|{eq|{xi.p|'-clientid'},'7'},'y','n'}


                    oCR = Get_ParameterObject(oDefinition.oParamSet.Get_NodeByNumber(1).sValue)
                    If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then
                        'TO DO - Execute the condition (or get it??)

                        If iAction = CMethodStepDefinition.xiAlgoActionType.xiConditionNot Then bYes = Not bYes
                        If bYes Then
                            sMyMethodTrace = sMyMethodTrace & "'" & oDefinition.oParamSet.Get_NodeByNumber(1).sValue & "' Executing" & vbCrLf
                            For Each oSubMethod In oDefinition.NMethodSteps.Values
                                If oAlgoI.oSteps.ContainsKey(oSubMethod.sKey) Then
                                    sMyMethodTrace = sMyMethodTrace & "'" & oSubMethod.sKey & "'" & vbCrLf
                                    oSubMethodI = oAlgoI.oSteps(oSubMethod.sKey)
                                    oCR = oSubMethodI.Execute
                                    If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then
                                        'do anything?
                                    Else
                                        oCResult.xiStatus = oCR.xiStatus  'keep going?
                                    End If
                                End If

                            Next
                        Else
                            sMyMethodTrace = sMyMethodTrace & "'" & oDefinition.oParamSet.Get_NodeByNumber(1).sValue & "' Not Executed" & vbCrLf
                        End If 'bYes

                    End If
                    'oMethodResult = oCR.xiStatus

                Case CMethodStepDefinition.xiAlgoActionType.xiIterate
                    'xi.m|s:-|i:EachDiaryT|r:xiR|e:DBLog, xiFail|a: m.Iterate|x:xim.DiaryTemplate

                    'has to be a dictionary?
                    '
                    oCR = Get_ParameterObject(oDefinition.oParamSet.Get_NodeByNumber(1).sValue)
                    If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then
                        oDictionary = oCR.oResult
                        For Each oIterate In oDictionary.oBOInstances.values
                            oMethodResult = oIterate 'this cycle, changes each iteration

                            'FOR EACH [GET SUB ENTITY FROM COLLECTION] 'Set the current instance 
                            ' THEN WITHIN EACH ITEM DO THESE STEPS:
                            For Each oSubMethod In oDefinition.NMethodSteps.Values
                                If oAlgoI.oSteps.ContainsKey(oSubMethod.sKey) Then
                                    sMyMethodTrace = sMyMethodTrace & "'" & oSubMethod.sKey & "'" & vbCrLf
                                    oSubMethodI = oAlgoI.oSteps(oSubMethod.sKey)
                                    oCR = oSubMethodI.Execute
                                    If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then
                                        'do anything?
                                    Else
                                        oCResult.xiStatus = oCR.xiStatus  'keep going?
                                    End If
                                End If

                            Next
                        Next


                    End If
                    'oMethodResult = oCR.xiStatus

                Case CMethodStepDefinition.xiAlgoActionType.xiSetValue
                    'xi.m|s:--|i:SetValue1|r:xiR|e:DBLog, xiFail|a: m.SetValue|x:xim.CreateDiaryInstance,[dDue=xim.EachDiary.iDaysFromZero+p.dGoLive]
                    oCR = Get_ParameterObject(oDefinition.oParamSet.Get_NodeByNumber(1).sValue)
                    oBOI1 = oCR.oResult
                    oCR = Get_ParameterObject(oDefinition.oParamSet.Get_NodeByNumber(2).sValue)
                    oSValue1 = oCR.oResult

                    oCR = oPlaceholder.BOSetValue(oBOI1, oSValue1)
                    If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then
                        sMyMethodTrace = sMyMethodTrace & "With '" & oDefinition.oParamSet.Get_NodeByNumber(1).sValue & "' and '" & oDefinition.oParamSet.Get_NodeByNumber(2).sValue & "' Value Set" & vbCrLf
                    Else
                        sMyMethodTrace = sMyMethodTrace & "With '" & oDefinition.oParamSet.Get_NodeByNumber(1).sValue & "' and '" & oDefinition.oParamSet.Get_NodeByNumber(2).sValue & "' Set FAILED" & vbCrLf
                    End If
                    oMethodResult = oCR.xiStatus

                Case CMethodStepDefinition.xiAlgoActionType.xi1Click
                    'xi.m|s:-|i:TemplateDiaries|r:BOInstanceGrid|e:DBLog,xiFail|a:m.1Click|x:templatediaries
                    oCR = Get_ParameterObject(oDefinition.oParamSet.Get_NodeByNumber(1).sValue)
                    oSValue1 = oCR.oResult

                    oCR = oPlaceholder.BO1Click(oSValue1)
                    If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then
                        'assign result to the instance in the OM
                        oMethodResult = oCR.oResult
                    End If
                    If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then
                        sMyMethodTrace = sMyMethodTrace & "'" & oDefinition.oParamSet.Get_NodeByNumber(1).sValue & "' 1 Click Run Successfully" & vbCrLf
                    Else
                        sMyMethodTrace = sMyMethodTrace & "'" & oDefinition.oParamSet.Get_NodeByNumber(1).sValue & "' 1 Click FAILED" & vbCrLf
                    End If


                Case CMethodStepDefinition.xiAlgoActionType.xiBOToXIValues
                    'oMethodResult = oCR.xiStatus
                Case CMethodStepDefinition.xiAlgoActionType.xixiValuesToBO
                    'oMethodResult = oCR.xiStatus


            End Select
        ElseIf oDefinition.iMethodType = CMethodStepDefinition.xiMethodType.xiScript Then

            'TO DO - REPLACE THIS WITH A MORE GLOBAL API AND ALSO THE PLACEHOLDER - MERGE INTO THIS API
            Dim oCAPI As CXIAPI = New CXIAPI


            oCR = Script_Execute(sKey, oCAPI, oAlgoI)
            If oCR.xiStatus <> xiEnum.xiFuncResult.xiSuccess Then
                'TO DO - handle the error - to log? Check error method and do what it says (abandon or whatever)
                oCResult.xiStatus = xiEnum.xiFuncResult.xiError
                oCResult.sMessage = oCR.sMessage
            End If
            If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then
                sMyMethodTrace = sMyMethodTrace & " Script returned '" & oCR.oResult & "'" & vbCrLf
            Else
                sMyMethodTrace = sMyMethodTrace & "Script FAILED: '" & oCR.sMessage & "'" & vbCrLf
            End If
        End If


        Return oCResult

    End Function

    Public Function Get_ParameterObject(sParameterValue As String) As CResult
        Dim oCResult As CResult = New CResult
        Dim oCR As CResult = Nothing
        Dim AsParam() As String
        Dim sValue As String = ""
        Dim oValue As Object = Nothing
        Dim sPrefix As String = ""
        Dim sParamIdentifier As String = ""
        Dim oStepResult As CMethodStepInstance = Nothing
        Dim oAlgoD As CAlgorithmDefinition = Nothing

        'eg.: x:xic.EachDiaryT, xim.CreateDiaryInstance, xif.CopyLive

        'xic = xicurrent value
        'xim = xi method value
        'xif = xi fixed - ie just whatever value you write, as though it were a string
        'nothing - this is assumed to be just the value - same as xif

        'ALERT - are you sure? Sometimes there might be strings?
        sParameterValue = sParameterValue.ToLower
        AsParam = Split(sParameterValue, ".")
        If UBound(AsParam) = 0 Then  'just the value
            sValue = sParameterValue
            oCResult.oResult = sValue
        ElseIf UBound(AsParam) > 0 Then
            sPrefix = AsParam(0)
            sParamIdentifier = sParameterValue.Substring(Len(sPrefix) + 1, Len(sParameterValue) - Len(sPrefix) - 1)
            Select Case sPrefix.ToLower
                Case "xif"
                    sValue = sParamIdentifier
                    oCResult.oResult = sValue
                Case "bod"
                    'FOR NOW - just return the value (a string)
                    sValue = sParamIdentifier
                    oCResult.oResult = sValue
                Case "p"
                    'Get from the params passed into this algo
                    'oAlgoD = oAlgoI.Definition
                    'oXIParameters.NNodeItem(sParameterValue).sValue
                    'ALERT TO DO - shall we rename the parameters coming in? ie without the 'p.' prefix? or strip them out when we put in the collection?
                    oCResult.oResult = oAlgoI.oXIParameters.NNode(sParameterValue).sValue
                Case "xim", "xis"
                    'get the value from the OM as an object

                    'so now we have the key to the item in the instances OM
                    '  sParamIdentifier
                    If oAlgoI.oSteps.ContainsKey(sParamIdentifier) Then
                        oStepResult = oAlgoI.oSteps(sParamIdentifier)
                        If oStepResult.oMethodResult Is Nothing Then
                            oCResult.xiStatus = xiEnum.xiFuncResult.xiLogicalError
                            'EXIT the whole thing? Maybe there are circumstances when we should not
                            'NO! in my case i am looking up the value in an iteration of the current line, which to start with is null, but i want to count that as zero
                        Else
                            oCResult.oResult = oStepResult.oMethodResult
                            oCResult.xiStatus = xiEnum.xiFuncResult.xiSuccess
                        End If
                    End If

                Case "xic"
                    'get the value from the OM as an object, but it will change over time with each iteration


                    If oAlgoI.oSteps.ContainsKey(sParamIdentifier) Then
                        oStepResult = oAlgoI.oSteps(sParamIdentifier)
                        If oStepResult.oMethodResult Is Nothing Then
                            oCResult.xiStatus = xiEnum.xiFuncResult.xiLogicalError
                            'EXIT the whole thing? Maybe there are circumstances when we should not
                        Else
                            oCResult.oResult = oStepResult.oMethodResult
                            oCResult.xiStatus = xiEnum.xiFuncResult.xiSuccess
                        End If
                    End If
                Case Else
                    If sParameterValue.Length > 0 Then
                        If Left(sParameterValue, 1) = "[" Then
                            'this is some kind of method/script etc, so just return it
                            sValue = sParameterValue
                            oCResult.oResult = sValue
                        End If
                    End If
            End Select
        End If


        Return oCResult

    End Function

    Public Function Script_Execute(sKeyEx As String, oCXIAPI As CXIAPI, oAlgoI As CAlgorithmInstance) As CResult

        Dim oCResult As CResult = New CResult
        Dim oCR As CResult = Nothing
        Dim sScript As String = ""
        Dim oXIScript As CScriptController = Nothing

        'HERE WE ARE BOTH COMPILING AND EXECUTING
        'TO DO - move the compilation of the script to the compiler and execute here at run time

        If oDefinition.iMethodType = CMethodStepDefinition.xiMethodType.xiScript Then
            oXIScript = oAlgoI.oScriptController

            sScript = oDefinition.sScript

            oCR = oXIScript.API2_Serialise_From_String(sScript)
            oCR = oXIScript.API2_ExecuteMyOM(sKeyEx, oCXIAPI, oAlgoI)

            If oCR.xiStatus = xiEnum.xiFuncResult.xiError Then
                oCResult.xiStatus = oCR.xiStatus
            Else
                oCResult.xiStatus = oCR.xiStatus
                oCResult.oResult = oCR.oResult
                oMethodResult = oCR.oResult
            End If

        Else
            oCResult.xiStatus = xiEnum.xiFuncResult.xiLogicalError
            oCResult.sMessage = "'" & oDefinition.sStepName & "' is not a script"
        End If


        'oCR = oXIScript.API2_Serialise_From_String(txtScript.Text)
        'txtCodeFormatted.Text = oXIScript.API_FormattedFunction.oResult
        'txtParsedFunction.Text = oXIScript.API_ParsedFunction.oResult
        'oCR = oXIScript.API2_ExecuteMyOM
        'If oCR.xiStatus = xiEnum.xiFuncResult.xiError Then
        '    txtOutput.Text = oCR.sMessage
        '    txtScript.BackColor = Color.Red
        'Else
        '    txtOutput.Text = oCR.oResult
        '    txtScript.BackColor = Color.Green
        'End If

        Return oCResult

    End Function

End Class
