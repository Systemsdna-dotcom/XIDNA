Imports System.Collections.Specialized
Imports XISystem

Public Class CAlgorithmInstance

    'Private oMySteps As New OrderedDictionary
    'Public Property oSteps() As OrderedDictionary
    '    Get
    '        Return oMySteps
    '    End Get
    '    Set(ByVal value As OrderedDictionary)
    '        oMySteps = value
    '    End Set
    'End Property
    Private oMySteps As New Dictionary(Of String, CMethodStepInstance)
    Public Property oSteps() As Dictionary(Of String, CMethodStepInstance)
        Get
            Return oMySteps
        End Get
        Set(ByVal value As Dictionary(Of String, CMethodStepInstance))
            oMySteps = value
        End Set
    End Property

    Private oMyXIParams As New CNodeItem
    Public Property oXIParameters() As CNodeItem
        Get
            Return oMyXIParams
        End Get
        Set(ByVal value As CNodeItem)
            oMyXIParams = value
        End Set
    End Property

    Private oMyScriptController As CScriptController = New CScriptController
    Public Property oScriptController() As CScriptController
        Get
            Return oMyScriptController
        End Get
        Set(ByVal value As CScriptController)
            oMyScriptController = value
        End Set
    End Property

    Public Function Execute_OM(sRunTimeParams As String) As CResult
        Dim oCResult As CResult = New CResult
        Dim oCR As CResult = Nothing
        Dim oMethodStepD As CMethodStepDefinition = Nothing
        Dim oMethodStepI As CMethodStepInstance = Nothing
        Dim sFirstStepKey As String = ""
        Dim oFirstStep As CMethodStepDefinition = Nothing
        Dim oThisStep As CMethodStepDefinition = Nothing
        Dim sStepKey As String = ""
        Dim AsRTParams() As String
        Dim oXIParam As CNodeItem = Nothing
        Dim j As Long
        Dim bAtLeastOneIssue As Boolean
        Dim sDebug As String = ""

        'recursively run through the code and assign a value to each method instance
        '  the definition holds the structure, the methodinstance just holds an object which is a result of the
        '  execution of that step. Any step can refer to any other step (but if it hasn't executed yet then it wont return anything)
        AsRTParams = Split(sRunTimeParams, ",")

        For Each oXIParam In Definition.oXIParameters.NNodeItems.Values
            If UBound(AsRTParams) >= j Then
                oXIParameters.NNode(oXIParam.sKey).sValue = AsRTParams(j)
            End If
            j = j + 1
        Next

        'first build a flat collection of method instances so at least each step is represented
        oSteps.Clear() 'if running through again
        For Each oMethodStepD In Definition.oMethodDefinition.NMethodSteps.Values
            oMethodStepI = New CMethodStepInstance
            oMethodStepI.sKey = oMethodStepD.sKey
            oMethodStepI.oAlgoI = Me
            oMethodStepI.oDefinition = oMethodStepD

            oSteps.Add(oMethodStepD.sKey, oMethodStepI)
        Next

        'now execute the OM from the definition and assign the result to this method instance
        ''sFirstStepKey = Definition.oMethodOM.NMethodSteps.Keys(0)
        ''oFirstStep = Definition.oMethodOM '.NMethodSteps(sFirstStepKey)
        ''oSteps(sFirstStepKey).Execute()

        For Each oThisStep In Definition.oMethodOM.NMethodSteps.Values  'oSteps.Values
            sStepKey = oThisStep.sKey
            oCR = oSteps(sStepKey).Execute()
            If oCR.xiStatus <> xiEnum.xiFuncResult.xiSuccess Then
                bAtLeastOneIssue = True
            End If
        Next

        If bAtLeastOneIssue Then
            oCResult.xiStatus = xiEnum.xiFuncResult.xiError
            'SET THE MESSAGE
        Else
            oCResult.xiStatus = xiEnum.xiFuncResult.xiSuccess
        End If

        'if debug:

        For Each oThisStep In Definition.oMethodOM.NMethodSteps.Values  'oSteps.Values
            sStepKey = oThisStep.sKey
            oCR = oSteps(sStepKey).ConcatDebug(sDebug)
        Next
        oCResult.oResult = sDebug

        Return oCResult
    End Function

    Public Function NextStep() As CResult

        Dim oCResult As CResult = New CResult
        Dim oCStep As CMethodStepDefinition = Nothing
        Dim oNextStep As CMethodStepDefinition = Nothing
        Dim sKey As String = ""

        'this is a bit of a weird way to do this. really we should run the methods recursively

        If sCurrentStepKey = "" Then 'first step
            sCurrentStepKey = Definition.oMethodOM.NMethodSteps.Keys(0)
            'sCurrentStepKey = oCStep.sKey
            oNextStep = Definition.oMethodDefinition.NMethodSteps(sCurrentStepKey)
        Else   'get the next step
            oCStep = Definition.oMethodDefinition.NMethodSteps(sCurrentStepKey)
            If oCStep.NMethodSteps.Count > 0 Then
                oNextStep = oCStep.NMethodSteps(oCStep.NMethodSteps.Keys(0))
            Else
                'next sibling
                If Not oCStep.oParentMethod Is Nothing Then
                    If oCStep.oParentMethod.NMethodSteps.Count > (oCStep.iChildIndex + 1) Then
                        oNextStep = oCStep.oParentMethod.NMethodSteps(oCStep.oParentMethod.NMethodSteps.Keys(oCStep.iChildIndex + 1))
                    End If
                End If
            End If
        End If

        If Not oNextStep Is Nothing Then
            sCurrentStepKey = oNextStep.sKey
            oCResult.xiStatus = xiEnum.xiFuncResult.xiSuccess
        Else
            sCurrentStepKey = "END"
            oCResult.xiStatus = xiEnum.xiFuncResult.xiWarning
        End If

        Return oCResult

    End Function

    Private oMyAlgoDef As CAlgorithmDefinition
    Public Property Definition() As CAlgorithmDefinition
        Get
            Return oMyAlgoDef
        End Get
        Set(ByVal value As CAlgorithmDefinition)
            oMyAlgoDef = value
        End Set
    End Property

    Private sMyCurrentStepKey As String
    Public Property sCurrentStepKey() As String
        Get
            Return sMyCurrentStepKey
        End Get
        Set(ByVal value As String)
            sMyCurrentStepKey = value
        End Set
    End Property
End Class
