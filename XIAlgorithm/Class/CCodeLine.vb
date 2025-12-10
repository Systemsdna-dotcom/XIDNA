Imports XISystem

Public Class CCodeLine

    Private oNCodeLines As New Dictionary(Of String, CCodeLine)       'New Hashtable  ' New Dictionary(Of String, CDataSource)
    'Private oNParameters As New Dictionary(Of String, CParameter)
    Private sMyLine As String = ""
    Private sMyLineOrig As String = ""
    Private iMyType As Long = 0
    Private sMyOperator As String = ""
    Private iMyParamCount As Long = 0
    Private sMyResolvedValue As String = ""
    Private sMyResult As String = ""
    Private sMyKey As String = ""
    Private sMyName As String = ""
    Private oMyXIAPI As CXIAPI = Nothing
    Private oMyAlgoI As CAlgorithmInstance = Nothing

    'Public Property NCodeLines() As Hashtable
    '    Get
    '        Return oNCodeLines
    '    End Get
    '    Set(ByVal value As Hashtable)
    '        oNCodeLines = value
    '    End Set
    'End Property

    Public Property NCodeLines() As Dictionary(Of String, CCodeLine)
        Get
            Return oNCodeLines
        End Get
        Set(ByVal value As Dictionary(Of String, CCodeLine))
            oNCodeLines = value
        End Set
    End Property

    'Public Property NParameters() As Dictionary(Of String, CParameter)
    '    Get
    '        Return oNParameters
    '    End Get
    '    Set(ByVal value As Dictionary(Of String, CParameter))
    '        oNParameters = value
    '    End Set
    'End Property



    Public Function DeSerialiseMe(ByVal sLine As String, sOriginalLine As String)

        Dim oMyResult As New CResult
        Dim iFirstBracket As Long
        Dim iLastBracket As Long
        Dim sFunction As String = ""
        Dim bSubFunction As Boolean
        Dim iOperator As String
        Dim sRemainingLine As String
        'Dim AsParameters() As String
        Dim j As Long
        Dim sParam As String = ""
        Dim iBracketLevel As Long = 0
        Dim sChar As String = ""
        Dim sCurrentParam As String = ""
        Dim sCurrentOrig As String = ""
        Dim sRemainingOrig As String = ""
        Dim sOrigChar As String = ""
        'Dim oSubCodeLine As CCodeLine

        Try
            sMyLine = sLine
            sMyLineOrig = sOriginalLine
            iFirstBracket = InStr(sMyLine, sOB)
            iLastBracket = InStrRev(sMyLine, sCB)

            If iFirstBracket <> 0 And iLastBracket <> 0 Then
                'so there is a function to resolve here
                bSubFunction = True
            ElseIf iFirstBracket <> 0 Or iLastBracket <> 0 Then
                'bracket mis-match, so error out
                oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                oMyResult.sMessage = oMyResult.sMessage & vbCrLf & "Bracket Mismatch: First Bracket position = " & iFirstBracket & ", Last Bracket position = " & iLastBracket & " [Line = '" & sMyLine & "']"
            Else
                'no more brackets
                bSubFunction = False
            End If

            If Left(sMyLine, 1) = sOB Then
                sMyLine = sMyLine.Substring(1, Len(sMyLine) - 2)
                sMyLineOrig = sMyLineOrig.Substring(1, Len(sMyLineOrig) - 2)
            End If
            iOperator = InStr(sMyLine, sFDelim)
            If iOperator = 0 Then
                'A base value, or in any case, not a function
                Debug.Print("XIScripting.CCodeLine.DeSerialiseMe." & sMyLine)
                'oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                'oMyResult.sMessage = oMyResult.sMessage & vbCrLf & "No operator - looking for delimiter '" & sFDelim & "' [Line = '" & sMyLine & "']"
            Else
                sOperator = Left(sMyLine, iOperator - 1)
                sRemainingLine = sMyLine.Substring(iOperator, Len(sMyLine) - iOperator)
                sRemainingOrig = sMyLineOrig.Substring(iOperator, Len(sMyLineOrig) - iOperator)
                'warning: do not try to strip out brackets here. for example: {add|{-|9,2},{*|2,3}},{-|7,5} , if you look carefully, this is not a bracket at start and bracket at end, but 2 params which happen to be functions
                If bSubFunction Then
                    'Solved? TO DO - a logic problem, the matching bracket for this function may not be the last bracket in the string, if there is
                    '   more than one sub-func on the same level then this will include all of the interior code!!!
                    'DS: solution to this is to have code here which gets next param, so it doesn't just do a 'split' based on commas or whatever
                    '  what it does is to go through the string and find any commas, or if it comes across an open bracket then it will count the open brackets from
                    '  then on and the close brackets and ONLY apply the commas which are at bracketlevel zero
                    For j = 0 To Len(sRemainingLine) - 1
                        sChar = sRemainingLine.Substring(j, 1)
                        sOrigChar = sRemainingOrig.Substring(j, 1)
                        sCurrentParam = sCurrentParam & sChar
                        sCurrentOrig = sCurrentOrig & sOrigChar
                        If sChar = sParamDelim Then
                            If iBracketLevel = 0 Then
                                sCurrentParam = sCurrentParam.Substring(0, Len(sCurrentParam) - 1)
                                sCurrentOrig = sCurrentOrig.Substring(0, Len(sCurrentOrig) - 1)
                                AddParam(sCurrentParam, sCurrentOrig)
                                sCurrentParam = ""
                                sCurrentOrig = ""
                            Else
                                'just a comma or something of a sub-func
                                'sCurrentParam = sCurrentParam & sChar
                            End If
                        ElseIf sChar = sOB Then
                            iBracketLevel = iBracketLevel + 1
                        ElseIf sChar = sCB Then
                            iBracketLevel = iBracketLevel - 1
                        End If

                        If j + 1 = Len(sRemainingLine) And sChar <> sParamDelim Then 'got to the end of a param. the char should NEVER be the paramdelim, but you never know
                            AddParam(sCurrentParam, sCurrentOrig)
                            sCurrentParam = ""
                            sCurrentOrig = ""
                        End If
                    Next j
                Else
                    'TO DO - NO - don't hold these even if it is '3' or 'hello'
                    'sResolvedValue = sRemainingLine
                    'sResult = sRemainingLine
                End If
                'don't resolve the parameters here, each code line should be resolved only once they have their assigned lines chained up




                'AsParameters = Split(sRemainingLine, sParamDelim)
                'For j = 0 To UBound(AsParameters)
                '    sParam = AsParameters(j)
                '    If InStr(sParam, sOB) = 0 Then 'this is not a 'sub-function'

                '    Else

                '    End If
                'Next j
            End If

        Catch ex As Exception
            Debug.Print("XIScripting.CCodeLine.DeSerialiseMe." & "Error: " & ex.Message)
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = oMyResult.sMessage & vbCrLf & "Bracket Mismatch: [Line = '" & sMyLine & "']"
        End Try

        Return oMyResult

    End Function


    Public Function AddParam(ByVal sParameter As String, sOriginalParam As String) As CResult

        Dim oMyResult As New CResult
        Dim oSubCodeLine As CCodeLine

        Try
            oSubCodeLine = New CCodeLine
            oSubCodeLine.DeSerialiseMe(sParameter, sOriginalParam)
            iMyParamCount = iMyParamCount + 1
            NCodeLines.Add("CL" & iParamCount, oSubCodeLine)
        Catch ex As Exception

        End Try

        Return oMyResult

    End Function

    Public Function ParsedLine(ByVal iLevel As Long) As CResult

        Dim oMyResult As New CResult
        Dim sConcat As String = ""
        Dim sTabs As String = ""

        Try

            For j = 1 To iLevel
                sTabs = sTabs & vbTab
            Next
            If iParamCount > 0 Then
                sConcat = sTabs & sOperator & " " & sOB & vbCrLf
                For Each oLine In NCodeLines.Values
                    sConcat = sConcat & oLine.ParsedLine(iLevel + 1).oResult & vbCrLf
                Next
                sConcat = sConcat & vbCrLf & sTabs & sCB
            Else
                sConcat = sTabs & sMyLine
            End If

            oMyResult.oResult = sConcat
        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = oMyResult.sMessage & ex.Message & vbCrLf
        End Try

        Return oMyResult

    End Function

    Public Function FormattedLine(ByVal iLevel As Long) As CResult

        Dim oMyResult As New CResult
        Dim sConcat As String = ""
        Dim sTabs As String = ""
        Dim k As Long

        Try

            For j = 1 To iLevel
                sTabs = sTabs & vbTab
            Next
            If iParamCount > 0 Then
                sConcat = sTabs & OperatorFormat(sOperator) & " " & vbCrLf
                For Each oLine In NCodeLines.Values
                    sConcat = sConcat & oLine.FormattedLine(iLevel + 1).oResult & vbCrLf
                    k = k + 1
                    If k < NCodeLines.Count Then
                        sConcat = sConcat & sTabs & OperatorGlyph(sOperator)
                    End If
                Next
                sConcat = sConcat & vbCrLf
            Else
                sConcat = sTabs & sMyLine
            End If

            oMyResult.oResult = sConcat
        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = oMyResult.sMessage & ex.Message & vbCrLf
        End Try

        Return oMyResult

    End Function

    Private Function OperatorFormat(ByVal sOperator As String) As String

        Try
            Select Case LCase(sOperator)
                Case "-", "minus"
                    Return "Minus"
                Case "+", "plus"
                    Return "Plus"
                Case "/", "divide"
                    Return "Divide"
                Case "*", "multiply"
                    Return "Multiply"
                Case Else
                    Return sOperator


            End Select


        Catch ex As Exception
            Return "Error in Operator Format"
        End Try

    End Function

    Private Function OperatorGlyph(ByVal sOperator As String) As String

        Try
            Select Case LCase(sOperator)
                Case "-", "minus"
                    Return "'-'"
                Case "+", "plus"
                    Return "'+'"
                Case "/", "divide"
                    Return "'/'"
                Case "*", "multiply"
                    Return "'*'"
                Case Else
                    Return sOperator


            End Select


        Catch ex As Exception
            Return "Error in Operator Glyph"
        End Try

    End Function

    'TO DO - we may need an OM as a template (ie don't keep resolving the same func all the time)
    '  so we need a way of keeping the unresolved values and then re-resolving everything for each func execute
    '  this means maintaining the unresolved values and om (which this does)
    Public Function ExecuteMe(Optional sKeyEx As String = "", Optional oCXIAPIEx As CXIAPI = Nothing, Optional oXIAlgoIEx As CAlgorithmInstance = Nothing, Optional sGUID As String = "", Optional sSessionID As String = "") As CResult

        Dim oMyResult As New CResult
        Dim oCParameter As CCodeLine
        Dim oCR As CResult
        Dim oLastChildResult As CResult = Nothing

        Try
            If sKeyEx <> "" Then sMyKey = sKeyEx
            If Not oCXIAPIEx Is Nothing Then oMyXIAPI = oCXIAPIEx
            If Not oXIAlgoIEx Is Nothing Then oMyAlgoI = oXIAlgoIEx

            If iParamCount > 0 Then
                For Each oCParameter In NCodeLines.Values
                    oCR = oCParameter.ExecuteMe(sKeyEx, oCXIAPIEx, oXIAlgoIEx, sGUID, sSessionID)
                    If oCR.xiStatus = xiEnum.xiFuncResult.xiError Then
                        oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                        oMyResult.sMessage = oMyResult.sMessage & oCR.sMessage & vbCrLf
                    Else
                        oLastChildResult = oCR
                    End If
                Next oCParameter
            End If

            If oMyResult.xiStatus <> xiEnum.xiFuncResult.xiError Then
                oCR = ResolveMe(sGUID, sSessionID)
                If oCR.xiStatus = xiEnum.xiFuncResult.xiError Then
                    oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                    oMyResult.sMessage = oMyResult.sMessage & oCR.sMessage & vbCrLf
                    Return oMyResult
                    Exit Function
                Else
                    If sOperator = "" And Not oLastChildResult Is Nothing Then 'empty bracket for no reason just contains subtext, not a result. So in this case pass up the last child result. 
                        'BE CAREFUL i don't know if negative consequence
                        oMyResult = oLastChildResult
                    Else
                        oMyResult = oCR
                    End If

                End If
            End If

        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = oMyResult.sMessage & ex.Message & vbCrLf
        End Try

        Return oMyResult

    End Function

    Public Function InitialiseMe(oXIAPI As CXIAPI, Optional oXIAlgoI As CAlgorithmInstance = Nothing) As CResult

        Dim oMyResult As New CResult
        'Dim oCParameter As CCodeLine
        'Dim oCR As CResult

        Try
            oMyXIAPI = oXIAPI
            oMyAlgoI = oXIAlgoI

        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = oMyResult.sMessage & ex.Message & vbCrLf
        End Try

        Return oMyResult

    End Function

    Public Function ResolveMe(Optional sGUID As String = "", Optional sSessionID As String = "") As CResult

        Dim oMyResult As New CResult
        Dim oSubResult As CCodeLine
        Dim j As Long
        Dim sCondition As String = ""
        Dim sIf1 As String = ""
        Dim sIf2 As String = ""
        Dim sV1 As String = ""
        Dim k As Long
        Dim sV2 As String = ""
        Dim sV3 As String = ""
        Dim sCleanedValue As String = ""
        Dim oCR As CResult

        Try
            'remember everything is already in lower case

            If sOperator <> "" Then
                sMyResolvedValue = sOB & sOperator & sFDelim
                For Each oSubResult In NCodeLines.Values
                    j = j + 1
                    'RT_Resolve(oSubResult.sResult)
                    oSubResult.RT_Resolve(oSubResult.sResult)  'very odd way of doing so be careful
                    sMyResolvedValue = sMyResolvedValue & oSubResult.sResult
                    If j < NCodeLines.Count Then
                        sMyResolvedValue = sMyResolvedValue & sParamDelim
                    End If
                Next oSubResult
                sMyResolvedValue = sMyResolvedValue & sCB

                Select Case sOperator

                    Case "add", "+", "sub", "-", "mul", "*", "div", "\", "increment", "decrement"


                        If iParamCount <> 2 Or NCodeLines.Count <> 2 Then
                            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            oMyResult.sMessage = "Param count for function '" & sOperator & "' is " & iParamCount & " when it should be 2 [Line='" & sMyLine & "']"
                        Else

                            Select Case sOperator
                                Case "add", "+", "increment"
                                    sMyResult = Val(NCodeLines("CL1").sResult) + Val(NCodeLines("CL2").sResult)
                                Case "sub", "-", "decrement"
                                    sMyResult = Val(NCodeLines("CL1").sResult) - Val(NCodeLines("CL2").sResult)
                                Case "mul", "*"
                                    sMyResult = Val(NCodeLines("CL1").sResult) * Val(NCodeLines("CL2").sResult)
                                Case "div", "\"
                                    Try
                                        sMyResult = Val(NCodeLines("CL1").sResult) / Val(NCodeLines("CL2").sResult)
                                    Catch exDiv As Exception
                                        oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                                        oMyResult.sMessage = "[Line='" & sMyLine & "'] - Error:" & exDiv.Message
                                    End Try
                                    'Case "concat", "c"
                                    '    sMyResult = NCodeLines("CL1").sResult & NCodeLines("CL2").sResult
                                    'Case "dec"
                                    '    Try
                                    '        sMyResult = Math.Round(CDbl(Val(NCodeLines("CL1").sResult)), CInt(Val(NCodeLines("CL2").sResult)))
                                    '    Catch exFunc As Exception
                                    '        oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                                    '    End Try

                            End Select


                        End If
                    'conditions
                    Case "eq", "=", "gt", ">", "lt", "<", "ne", "<>", "gteq", ">=", "lteq", "<=", "or", "||", "contains"


                        If (iParamCount <> 2 Or NCodeLines.Count <> 2) And (iParamCount <> 3 Or NCodeLines.Count <> 3) Then
                            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            oMyResult.sMessage = "Param count for condition '" & sOperator & "' is " & iParamCount & " when it should be 2 [Line='" & sMyLine & "']"
                        Else

                            Select Case sOperator
                                Case "eq", "="
                                    If IsNumeric(NCodeLines("CL1").sResult) And IsNumeric(NCodeLines("CL2").sResult) Then
                                        If Val(NCodeLines("CL1").sResult) = Val(NCodeLines("CL2").sResult) Then
                                            sMyResult = "true"
                                        Else
                                            sMyResult = "false"
                                        End If
                                    Else 'non-numeric
                                        'case sensitive for now
                                        If NCodeLines("CL1").sResult = NCodeLines("CL2").sResult Then
                                            sMyResult = "true"
                                        Else
                                            sMyResult = "false"
                                        End If
                                    End If

                                Case "gt", ">"
                                    If IsNumeric(NCodeLines("CL1").sResult) And IsNumeric(NCodeLines("CL2").sResult) Then
                                        If Val(NCodeLines("CL1").sResult) > Val(NCodeLines("CL2").sResult) Then
                                            sMyResult = "true"
                                        Else
                                            sMyResult = "false"
                                        End If
                                    Else 'non-numeric
                                        'don't do anything
                                    End If
                                Case "lt", "<"
                                    If IsNumeric(NCodeLines("CL1").sResult) And IsNumeric(NCodeLines("CL2").sResult) Then
                                        If Val(NCodeLines("CL1").sResult) < Val(NCodeLines("CL2").sResult) Then
                                            sMyResult = "true"
                                        Else
                                            sMyResult = "false"
                                        End If
                                    Else 'non-numeric

                                    End If
                                Case "ne", "<>"
                                    If IsNumeric(NCodeLines("CL1").sResult) And IsNumeric(NCodeLines("CL2").sResult) Then
                                        If Val(NCodeLines("CL1").sResult) <> Val(NCodeLines("CL2").sResult) Then
                                            sMyResult = "true"
                                        Else
                                            sMyResult = "false"
                                        End If
                                    Else 'non-numeric
                                        'case sensitive for now
                                        If NCodeLines("CL1").sResult <> NCodeLines("CL2").sResult Then
                                            sMyResult = "true"
                                        Else
                                            sMyResult = "false"
                                        End If
                                    End If
                                Case "gteq", ">="
                                    If IsNumeric(NCodeLines("CL1").sResult) And IsNumeric(NCodeLines("CL2").sResult) Then
                                        If Val(NCodeLines("CL1").sResult) >= Val(NCodeLines("CL2").sResult) Then
                                            sMyResult = "true"
                                        Else
                                            sMyResult = "false"
                                        End If
                                    Else 'non-numeric

                                    End If
                                Case "lteq", "<="
                                    If IsNumeric(NCodeLines("CL1").sResult) And IsNumeric(NCodeLines("CL2").sResult) Then
                                        If Val(NCodeLines("CL1").sResult) <= Val(NCodeLines("CL2").sResult) Then
                                            sMyResult = "true"
                                        Else
                                            sMyResult = "false"
                                        End If
                                    Else 'non-numeric
                                        'case sensitive for now

                                    End If
                                Case "or", "||"
                                    If IsNumeric(NCodeLines("CL1").sResult) And IsNumeric(NCodeLines("CL2").sResult) And IsNumeric(NCodeLines("CL3").sResult) Then
                                        If Val(NCodeLines("CL1").sResult) = Val(NCodeLines("CL2").sResult) Then
                                            sMyResult = "true"
                                        ElseIf Val(NCodeLines("CL1").sResult) = Val(NCodeLines("CL3").sResult) Then
                                            sMyResult = "true"
                                        Else
                                            sMyResult = "false"
                                        End If
                                    Else 'non-numeric
                                        'case sensitive for now
                                        If NCodeLines("CL1").sResult = NCodeLines("CL2").sResult Then
                                            sMyResult = "true"
                                        ElseIf NCodeLines("CL1").sResult = NCodeLines("CL3").sResult Then
                                            sMyResult = "true"
                                        Else
                                            sMyResult = "false"
                                        End If
                                    End If
                                Case "contains"
                                    If Not String.IsNullOrEmpty(NCodeLines("CL1").sResult) Then
                                        If NCodeLines("CL1").sResult.Contains("|||") Then
                                            Dim STRarray As String() = NCodeLines("CL1").sResult.Split("|||")
                                            If STRarray.Contains(NCodeLines("CL2").sResult) Then
                                                sMyResult = "true"
                                            Else
                                                sMyResult = "false"
                                            End If
                                        Else
                                            Dim STRarray As String() = NCodeLines("CL2").sResult.Split("|||")
                                            If STRarray.Contains(NCodeLines("CL1").sResult) Then
                                                sMyResult = "true"
                                            Else
                                                sMyResult = "false"
                                            End If
                                        End If
                                    Else
                                        sMyResult = "false"
                                    End If

                            End Select


                        End If

                    Case "if"
                        Dim bResult As Boolean
                        'Dim oCR As CResult
                        If NCodeLines.Count <> 3 Then
                            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            oMyResult.sMessage = "Condition count for if statement is " & NCodeLines.Count & " when it should be 3 - [condition][if][else] [Line='" & sMyLine & "']"
                        Else

                            sCondition = NCodeLines("CL1").sResult
                            sIf1 = NCodeLines("CL2").sResult
                            sIf2 = NCodeLines("CL3").sResult
                            oCR = Get_BooleanResult(sCondition)
                            If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then
                                bResult = oCR.oResult
                                If bResult = True Then
                                    sMyResult = sIf1
                                Else
                                    sMyResult = sIf2
                                End If
                            Else
                                'error to do
                            End If


                        End If

                    Case "concat", "c"
                        'multiple unknown no params
                        Select Case sOperator
                            Case "concat", "c"
                                For Each oParam In NCodeLines.Values
                                    sMyResult = sMyResult & oParam.sResult
                                Next

                        End Select
                    Case "dec"
                        If iParamCount <> 2 Or NCodeLines.Count <> 2 Then
                            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            oMyResult.sMessage = "Param count for function '" & sOperator & "' is " & iParamCount & " when it should be 2 [Line='" & sMyLine & "']"
                        Else
                            Try
                                sMyResult = Math.Round(CDbl(Val(NCodeLines("CL1").sResult)), CInt(Val(NCodeLines("CL2").sResult)))
                            Catch exFunc As Exception
                                oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            End Try
                        End If
                    Case "formatdate", "fd"
                        If iParamCount <> 2 Or NCodeLines.Count <> 2 Then
                            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            oMyResult.sMessage = "Param count for function '" & sOperator & "' is " & iParamCount & " when it should be 2 [Line='" & sMyLine & "']"
                        Else
                            Try
                                sMyResult = Format(CDate(NCodeLines("CL1").sResult), NCodeLines("CL2").sResult)
                            Catch exFunc As Exception
                                oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            End Try
                        End If
                    Case "select", "s", "translate", "match"
                        'select|dan,bob,3,john,4,dan,5
                        'so you specify the value, and when it works out which you want gives you the corresponding result

                        If iParamCount < 2 Or NCodeLines.Count < 2 Then
                            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            oMyResult.sMessage = "Param count for function '" & sOperator & "' is " & iParamCount & " when it should be greater than 1 [Line='" & sMyLine & "']"
                        Else
                            Try
                                sV1 = NCodeLines("CL1").sResult

                                Dim q As Long
                                For Each oParam In NCodeLines.Values
                                    'sMyResult = sMyResult & oParam.sResult
                                    k = k + 1
                                    If k > 1 Then
                                        q = q + 1
                                        If q = 1 Then
                                            sV2 = oParam.sResult
                                        End If
                                        If q = 2 Then 'second of a pair
                                            q = 0
                                            sV3 = oParam.sResult

                                            If sV2 = sV1 Then
                                                sMyResult = sV3
                                                Exit For
                                            End If

                                            sV2 = ""
                                            sV3 = ""
                                        End If

                                    End If
                                Next
                                If sMyResult = "" And sV3 = "" And sV2 <> "" Then 'so a default was left at the end (ie otherwise its this)
                                    sMyResult = sV2
                                End If
                            Catch exFunc As Exception
                                oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            End Try
                        End If
                    Case "isdate", "d"
                        If iParamCount <> 1 Or NCodeLines.Count <> 1 Then
                            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            oMyResult.sMessage = "Param count for function '" & sOperator & "' is " & iParamCount & " when it should be 1 [Line='" & sMyLine & "']"
                        Else
                            Try
                                If IsDate(NCodeLines("CL1").sResult) Then
                                    sMyResult = "true"
                                Else
                                    sMyResult = "false"
                                End If
                            Catch exFunc As Exception
                                oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            End Try
                        End If

                    Case "isnumeric", "n"
                        If iParamCount <> 1 Or NCodeLines.Count <> 1 Then
                            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            oMyResult.sMessage = "Param count for function '" & sOperator & "' is " & iParamCount & " when it should be 1 [Line='" & sMyLine & "']"
                        Else
                            Try
                                If IsNumeric(NCodeLines("CL1").sResult) Then
                                    sMyResult = "true"
                                Else
                                    sMyResult = "false"
                                End If
                            Catch exFunc As Exception
                                oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            End Try
                        End If
                    Case "sum"
                        For Each oParam In NCodeLines.Values
                            sMyResult = sMyResult + Val(oParam.sResult)
                        Next
                    Case "replace", "r"  'replace|lookfor,replacewith,inwhat
                        If iParamCount <> 3 Or NCodeLines.Count <> 2 Then
                            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            oMyResult.sMessage = "Param count for function '" & sOperator & "' is " & iParamCount & " when it should be 3 [Line='" & sMyLine & "']"
                        Else
                            Try
                                sMyResult = Replace(NCodeLines("CL3").sResult, NCodeLines("CL1").sResult, NCodeLines("CL2").sResult)
                            Catch exFunc As Exception
                                oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            End Try
                        End If
                    Case "left", "l"
                        If iParamCount <> 2 Or NCodeLines.Count <> 2 Then
                            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            oMyResult.sMessage = "Param count for function '" & sOperator & "' is " & iParamCount & " when it should be 2 [Line='" & sMyLine & "']"
                        Else
                            Try
                                sMyResult = Left(NCodeLines("CL1").sResult, NCodeLines("CL2").sResult)
                            Catch exFunc As Exception
                                oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            End Try
                        End If
                    Case "right", "r"
                        If iParamCount <> 2 Or NCodeLines.Count <> 2 Then
                            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            oMyResult.sMessage = "Param count for function '" & sOperator & "' is " & iParamCount & " when it should be 2 [Line='" & sMyLine & "']"
                        Else
                            Try
                                sMyResult = Right(NCodeLines("CL1").sResult, NCodeLines("CL2").sResult)
                            Catch exFunc As Exception
                                oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            End Try
                        End If
                    Case "age"
                        If iParamCount <> 2 Or NCodeLines.Count <> 2 Then
                            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            oMyResult.sMessage = "Param count for function '" & sOperator & "' is " & iParamCount & " when it should be 2 [Line='" & sMyLine & "']"
                        Else
                            'second param is what to calc age in - years, months, days
                            Try
                                'sMyResult = Right(NCodeLines("CL1").sResult, NCodeLines("CL2").sResult)
                            Catch exFunc As Exception
                                oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            End Try
                        End If

                        'choose, translate, select, in, match, regex, isdate, isnumeric, sum, average, biggest, smallest, earliest, latest
                        'Replace, makedate, between, Replace, Mid, Right, Left, InStr, round, And, age, Day, Month, Year, week, LCase, UCase, ascii, abs, Random
                        'Not, double, int, date, boolean, trim, ltrim, rtrim, yeardiff, mondiff, daydiff, between


                        'Case "round", "r"
                        '    Select Case sOperator
                        '        Case "round", "r"
                        '            'For Each oParam In NCodeLines.Values
                        '            '    sMyResult = sMyResult & oParam.sResult
                        '            'Next
                        '            sMyResult = round(NCodeLines("CL1").sResult)

                        '    End Select

                        'Case 

                    Case "xi.a", "xi.a.f", "xi.a.fk", "xi.a.fk.f", "xi.d", "xi.a.d", "xi.p", "xi.r", "xi.api", "xi.qsxivalue", "xi.qsstepxivalue"   'xi attribute, xi attribute formatted, xi attr fk, xi attr fk format, xi definition,  xi attr definition 
                        'BO: xibo|a|['client',100,'sName'] - a shortcut to getting a bo attribute
                        '                        xibo|aformat|['client',100,'rBudget'] - formatted attr value - '£32,000.00'
                        'xibo|fk['client',100,'FKiCompanyID','iYears'] - a shortcut to get an FK value
                        'xibo|d['client','Description'] - get any aspect for a bo definition
                        If oMyXIAPI Is Nothing Then oMyXIAPI = New CXIAPI
                        Select Case sOperator
                            Case "xi.p"  'parameter - what do we need to pass in? Just the name?
                                If iParamCount <> 1 Or NCodeLines.Count <> 1 Then
                                    oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                                    oMyResult.sMessage = "Param count for function '" & sOperator & "' is " & iParamCount & " when it should be 1 [Line='" & sMyLine & "']"
                                Else

                                    Try
                                        oCR = Nothing
                                        Select Case sOperator
                                            Case "xi.p"
                                                oCR = oMyXIAPI.Parameter(NCodeLines("CL1").sResult, sGUID, sSessionID)

                                        End Select
                                        If Not oCR Is Nothing Then
                                            If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then
                                                sMyResult = oCR.oResult
                                            Else

                                            End If
                                        End If

                                    Catch exFunc As Exception
                                        oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                                    End Try
                                End If
                                'xi.qsxivalue
                            Case "xi.qsxivalue"  'xivalue - reference by name, even if in a sub-section??
                                If iParamCount < 3 Or NCodeLines.Count < 3 Then
                                    oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                                    oMyResult.sMessage = "Param count for function '" & sOperator & "' is " & iParamCount & " when it should be 1 [Line='" & sMyLine & "']"
                                Else

                                    Try
                                        oCR = Nothing
                                        Select Case sOperator
                                            Case "xi.qsxivalue"
                                                oCR = oMyXIAPI.QSXIValue(NCodeLines("CL3").sResult, NCodeLines("CL2").sResult, sGUID, sSessionID)

                                        End Select
                                        If Not oCR Is Nothing Then
                                            If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then
                                                sMyResult = oCR.oResult
                                            Else

                                            End If
                                        End If

                                    Catch exFunc As Exception
                                        oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                                    End Try
                                End If
                            Case "xi.qsstepxivalue"  'xivalue - reference by name, even if in a sub-section??
                                If iParamCount <> 2 Or NCodeLines.Count <> 2 Then
                                    oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                                    oMyResult.sMessage = "Param count for function '" & sOperator & "' is " & iParamCount & " when it should be 2 [Line='" & sMyLine & "']"
                                Else

                                    Try
                                        oCR = Nothing
                                        Select Case sOperator
                                            Case "xi.qsstepxivalue"
                                                oCR = oMyXIAPI.QSStepXIValue(NCodeLines("CL1").sResult, NCodeLines("CL2").sResult)

                                        End Select
                                        If Not oCR Is Nothing Then
                                            If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then
                                                sMyResult = oCR.oResult
                                            Else

                                            End If
                                        End If

                                    Catch exFunc As Exception
                                        oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                                    End Try
                                End If
                            Case "xi.r"  'reserved word - something hard coded that the xi can do such as 'today' 
                                If iParamCount <> 1 Or NCodeLines.Count <> 1 Then
                                    oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                                    oMyResult.sMessage = "Param count for function '" & sOperator & "' is " & iParamCount & " when it should be 1 [Line='" & sMyLine & "']"
                                Else

                                    Try
                                        oCR = Nothing
                                        Select Case sOperator
                                            Case "xi.r"
                                                'Select Case NCodeLines("CL1").sResult
                                                '    Case "currentuser"
                                                '        'oCR = oMyXIAPI.Credentials.CurrentUser
                                                '        oCR = oMyXIAPI.Reserved()
                                                'End Select
                                                oCR = oMyXIAPI.Reserved(NCodeLines("CL1").sResult)

                                        End Select
                                        If Not oCR Is Nothing Then
                                            If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then
                                                sMyResult = oCR.oResult
                                            Else

                                            End If
                                        End If

                                    Catch exFunc As Exception
                                        oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                                    End Try
                                End If
                            Case "xi.api"
                                If iParamCount <> 1 Or NCodeLines.Count <> 1 Then
                                    oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                                    oMyResult.sMessage = "Param count for function '" & sOperator & "' is " & iParamCount & " when it should be 1 [Line='" & sMyLine & "']"
                                Else

                                    Try
                                        oCR = Nothing
                                        Select Case sOperator
                                            Case "xi.api"
                                                'Select Case NCodeLines("CL1").sResult
                                                '    Case "currentuser"
                                                '        'oCR = oMyXIAPI.Credentials.CurrentUser
                                                '        oCR = oMyXIAPI.Reserved()
                                                'End Select
                                                oCR = oMyXIAPI.API(NCodeLines("CL1").sResult)  'can be anything, just means the script can keep up with the OM

                                        End Select
                                        If Not oCR Is Nothing Then
                                            If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then
                                                sMyResult = oCR.oResult
                                            Else

                                            End If
                                        End If

                                    Catch exFunc As Exception
                                        oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                                    End Try
                                End If
                            Case "xi.a", "xi.f"
                                'attribute/attribute formatted
                                If iParamCount < 3 Or NCodeLines.Count < 3 Then
                                    oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                                    oMyResult.sMessage = "Param count for function '" & sOperator & "' is " & iParamCount & " when it should be 3 [Line='" & sMyLine & "']"
                                Else

                                    Try
                                        oCR = Nothing
                                        Select Case sOperator
                                            Case "xi.a"  'can be 3 or 4
                                                If iParamCount = 3 Then
                                                    oCR = oMyXIAPI.Attr(NCodeLines("CL1").sResult, NCodeLines("CL2").sResult, NCodeLines("CL3").sResult)
                                                ElseIf iParamCount = 4 Then
                                                    oCR = oMyXIAPI.Attr(NCodeLines("CL1").sResult, NCodeLines("CL2").sResult, NCodeLines("CL3").sResult, NCodeLines("CL4").sResult)
                                                ElseIf iParamCount = 5 Then
                                                    oCR = oMyXIAPI.Attr(NCodeLines("CL1").sResult, NCodeLines("CL2").sResult, NCodeLines("CL3").sResult, NCodeLines("CL4").sResult, NCodeLines("CL5").sResult)
                                                ElseIf iParamCount = 6 Then
                                                    oCR = oMyXIAPI.Attr(NCodeLines("CL1").sResult, NCodeLines("CL2").sResult, NCodeLines("CL3").sResult, NCodeLines("CL4").sResult, NCodeLines("CL5").sResult, NCodeLines("CL6").sResult)
                                                End If

                                            Case "xi.a.f"
                                                oCR = oMyXIAPI.AttrFormatted(NCodeLines("CL1").sResult, NCodeLines("CL2").sResult, NCodeLines("CL3").sResult)
                                        End Select
                                        If Not oCR Is Nothing Then
                                            If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then
                                                sMyResult = oCR.oResult
                                            Else

                                            End If
                                        End If

                                    Catch exFunc As Exception
                                        oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                                    End Try
                                End If

                            Case "xi.a.fk"
                                'attribute FK
                                If iParamCount <> 4 Or NCodeLines.Count <> 4 Then
                                    oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                                    oMyResult.sMessage = "Param count for function '" & sOperator & "' is " & iParamCount & " when it should be 4 [Line='" & sMyLine & "']"
                                Else

                                    Try
                                        oCR = Nothing
                                        Select Case sOperator
                                            'Case "xi.a"
                                            '    oCR = oMyXIAPI.Attr(NCodeLines("CL1").sResult, NCodeLines("CL2").sResult, NCodeLines("CL3").sResult)
                                            Case "xi.a.fk"  'bo, id, fk attr, attr you want
                                                oCR = oMyXIAPI.AttrFK(NCodeLines("CL1").sResult, NCodeLines("CL2").sResult, NCodeLines("CL3").sResult, NCodeLines("CL4").sResult)
                                        End Select
                                        If Not oCR Is Nothing Then
                                            If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then
                                                sMyResult = oCR.oResult
                                            Else

                                            End If
                                        End If
                                    Catch exFunc As Exception
                                        oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                                    End Try
                                End If

                        End Select
                    Case "xi.user"
                        'xi.user|name
                        If iParamCount <> 1 Or NCodeLines.Count <> 1 Then
                            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            oMyResult.sMessage = "Param count for function '" & sOperator & "' is " & iParamCount & " when it should be 3 [Line='" & sMyLine & "']"
                        Else

                            Try
                                oCR = oMyXIAPI.UserAttribute(NCodeLines("CL1").sResult)
                                If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then 'should return a boolean
                                    sMyResult = oCR.oResult
                                Else

                                End If

                            Catch exFunc As Exception
                                oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            End Try
                        End If

                    Case "xi.userrole"
                        If iParamCount <> 1 Or NCodeLines.Count <> 1 Then
                            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            oMyResult.sMessage = "Param count for function '" & sOperator & "' is " & iParamCount & " when it should be 1 [Line='" & sMyLine & "']"
                        Else

                            Try
                                oCR = oMyXIAPI.UserRole(NCodeLines("CL1").sResult)
                                If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then 'should return a boolean
                                    sMyResult = oCR.oResult
                                Else

                                End If

                            Catch exFunc As Exception
                                oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            End Try
                        End If

                    Case "xi.qs"
                        'questionset - get the step id and the field id (and maybe more complex for sections etc
                        If iParamCount <> 2 Or NCodeLines.Count <> 2 Then
                            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            oMyResult.sMessage = "Param count for function '" & sOperator & "' is " & iParamCount & " when it should be 2 [Line='" & sMyLine & "']"
                        Else

                            Try
                                oCR = oMyXIAPI.QuestionSetFieldValue(NCodeLines("CL1").sResult, NCodeLines("CL2").sResult)
                                If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then 'should return a boolean
                                    sMyResult = oCR.oResult
                                Else

                                End If

                            Catch exFunc As Exception
                                oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            End Try
                        End If
                    Case "xi.om"
                        If oMyXIAPI Is Nothing Then oMyXIAPI = New CXIAPI
                        If iParamCount <> 4 Or NCodeLines.Count <> 4 Then
                            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            oMyResult.sMessage = "Param count for function '" & sOperator & "' is " & iParamCount & " when it should be 1 [Line='" & sMyLine & "']"
                        Else

                            Try
                                Dim sparam1 As String
                                Dim sparam2 As String
                                Dim sparam3 As String
                                Dim sparam4 As String
                                sparam1 = NCodeLines("CL1").sResult 'coms.notification.create
                                sparam2 = NCodeLines("CL2").sResult
                                sparam3 = NCodeLines("CL3").sResult
                                sparam4 = NCodeLines("CL4").sResult

                                If sparam1 = "XIInfrastructure.XIInfraNotifications.Create" Then
                                    oCR = oMyXIAPI.OM(sparam1, sparam2, sparam3, sparam4)
                                    If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then 'should return a boolean
                                        sMyResult = oCR.oResult
                                    Else

                                    End If
                                    'Create notification using param1 and param2
                                End If

                            Catch exFunc As Exception
                                oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            End Try
                        End If
                    'Case "xi.setattr"
                    '    If oMyXIAPI Is Nothing Then oMyXIAPI = New CXIAPI
                    '    If iParamCount <> 2 Or NCodeLines.Count <> 2 Then
                    '        oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                    '        oMyResult.sMessage = "Param count for function '" & sOperator & "' is " & iParamCount & " when it should be 1 [Line='" & sMyLine & "']"
                    '    Else

                    '        Try
                    '            Dim sparam1 As String
                    '            Dim sparam2 As String
                    '            sparam1 = NCodeLines("CL1").sResult 'coms.notification.create
                    '            sparam2 = NCodeLines("CL2").sResult


                    '            ''If sparam1 = "XIInfrastructure.XIInfraNotifications.Create" Then
                    '            oCR = oMyXIAPI.setattr(sparam1, sparam2, sGUID)
                    '            If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then 'should return a boolean
                    '                    sMyResult = oCR.oResult
                    '                Else

                    '                End If
                    '            'Create notification using param1 and param2
                    '            '' End If

                    '        Catch exFunc As Exception
                    '            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                    '        End Try
                    '    End If
                    Case "xim", "xis"
                        If iParamCount <> 1 Or NCodeLines.Count <> 1 Then
                            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            oMyResult.sMessage = "Param count for function '" & sOperator & "' is " & iParamCount & " when it should be 1 [Line='" & sMyLine & "']"
                        Else

                            Try
                                If Not oMyAlgoI Is Nothing Then
                                    'TO DO - get access to the same lookup used by method line instances. Actually this must have a method line
                                    Dim oMethodStepInstance As CMethodStepInstance

                                    oMethodStepInstance = oMyAlgoI.oSteps(sMyKey)
                                    oCR = oMethodStepInstance.Get_ParameterObject(NCodeLines("CL2").sResult)
                                    If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then
                                        sMyResult = oCR.oResult
                                    Else
                                        'TO DO - FAIL
                                    End If

                                Else
                                    oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                                    oMyResult.sMessage = "Algorithm Instance not set on Code Line [Line='" & sMyLine & "']"
                                End If


                                'oCR = oMyXIAPI.QuestionSetFieldValue(NCodeLines("CL1").sResult, NCodeLines("CL2").sResult)
                                'If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then 'should return a boolean
                                '    sMyResult = oCR.oResult
                                'Else

                                'End If

                            Catch exFunc As Exception
                                oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                            End Try
                        End If
                    Case Else       'just hold the value, since we don't know what it is
                        sMyResult = sMyLine
                End Select
            Else
                'sMyResolvedValue = sMyLine
                'sMyResult = sMyLine
                sCleanedValue = sMyLineOrig
                If sCleanedValue.Length > 1 Then
                    If Left(sCleanedValue, 1) = "'" Or Left(sCleanedValue, 1) = """" Then

                        sCleanedValue = sCleanedValue.Substring(1, Len(sCleanedValue) - 2)
                    End If
                End If
                sMyResolvedValue = sCleanedValue
                sMyResult = sCleanedValue
            End If

            oMyResult.oResult = sMyResult     'OK, you should look at the sresult value, but this helps
        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = "ERROR: " & ex.Message  'TO DO - put in func names etc all over this DLL
        End Try

        Return oMyResult

    End Function

    Private Function RT_Resolve(sValue As String) As String
        Dim oMethodStepInstance As CMethodStepInstance
        Dim oCR As CResult
        Dim sResolved As String = ""

        Try
            sResolved = sValue
            If sValue.Contains("xim") Or sValue.Contains("xis") Then
                If Not oMyAlgoI Is Nothing Then
                    'TO DO - get access to the same lookup used by method line instances. Actually this must have a method line


                    oMethodStepInstance = oMyAlgoI.oSteps(sMyKey)
                    oCR = oMethodStepInstance.Get_ParameterObject(sValue)
                    If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then
                        sResolved = oCR.oResult
                        sResolvedValue = sResolved     'assumption that this is not used anywhere else as it needs to be reset each time
                    Else
                        'TO DO - FAIL
                        sResolved = ""
                        sResolvedValue = ""
                    End If

                Else
                    'oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                    'oMyResult.sMessage = "Algorithm Instance not set on Code Line [Line='" & sMyLine & "']"
                End If
            End If
        Catch ex As Exception

        End Try

        Return sResolved

    End Function

    Public Property sLine() As String
        Get
            Return sMyLine
        End Get
        Set(ByVal value As String)
            sMyLine = value
        End Set
    End Property

    Public Property iType() As Long
        Get
            Return iMyType
        End Get
        Set(ByVal value As Long)
            iMyType = value
        End Set
    End Property

    Public Property sName() As String
        Get
            Return sMyName
        End Get
        Set(ByVal value As String)
            sMyName = value
        End Set
    End Property

    Public Property sKey() As String
        Get
            Return sMyKey
        End Get
        Set(ByVal value As String)
            sMyKey = value
        End Set
    End Property

    Public Property sOperator() As String
        Get
            Return sMyOperator
        End Get
        Set(ByVal value As String)
            sMyOperator = value
        End Set
    End Property

    Private bMyResolved As Boolean
    Public Property bResolved() As Boolean
        Get
            Return bMyResolved
        End Get
        Set(ByVal value As Boolean)
            bMyResolved = value
        End Set
    End Property

    Public Property sResolvedValue() As String
        Get
            Return sMyResolvedValue
        End Get
        Set(ByVal value As String)
            sMyResolvedValue = value
            bResolved = True
        End Set
    End Property

    Public Property sResult() As String
        Get
            If bResolved Then
                Return sResolvedValue
            Else
                Return sMyResult
            End If

        End Get
        Set(ByVal value As String)
            sMyResult = value
            bResolved = False
        End Set
    End Property

    Private sMyRawValue As String
    Public ReadOnly Property sRawValue() As String
        Get
            Return sMyResult
        End Get
        'Set(ByVal value As String)
        '    sMyRawValue = value
        'End Set
    End Property

    Public ReadOnly Property iParamCount() As Long
        'Private Property iParamCount() As Long
        Get
            Return iMyParamCount
        End Get
        'Set(ByVal value As Long)
        '    iMyParamCount = value
        'End Set
    End Property

    Public Function Get_BooleanResult(sValue As String) As CResult

        Dim oMyResult As New CResult
        Dim sValLower As String

        Try
            sValLower = sValue.ToLower
            If sValLower = "y" Or sValLower = "true" Then
                oMyResult.xiStatus = xiEnum.xiFuncResult.xiSuccess
                oMyResult.oResult = True
            ElseIf sValLower = "n" Or sValLower = "false" Then
                oMyResult.xiStatus = xiEnum.xiFuncResult.xiSuccess
                oMyResult.oResult = False
            Else
                'evaluate the condition

            End If
        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = "ERROR: [" & Get_Class() & "." & System.Reflection.MethodBase.GetCurrentMethod.Name & "] - " & ex.Message & " - Trace: " & ex.StackTrace & vbCrLf
        End Try

        Return oMyResult

    End Function

    Public Function ResetResults() As CResult

        Dim oMyResult As New CResult
        Dim oLine As CCodeLine

        Try
            For Each oLine In NCodeLines.Values
                oLine.ResetResults()
            Next
            sResolvedValue = ""
            sResult = ""
        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = "ERROR: [" & Get_Class() & "." & System.Reflection.MethodBase.GetCurrentMethod.Name & "] - " & ex.Message & " - Trace: " & ex.StackTrace & vbCrLf
        End Try

        Return oMyResult

    End Function

    Public Function AddCodeLine(ByVal oNewCodeLine As CCodeLine) As CResult

        Dim oMyResult As New CResult

        Try
            iMyParamCount = iMyParamCount + 1
            NCodeLines.Add("CL" & iParamCount, oNewCodeLine)
        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = "ERROR: [" & Get_Class() & "." & System.Reflection.MethodBase.GetCurrentMethod.Name & "] - " & ex.Message & " - Trace: " & ex.StackTrace & vbCrLf
        End Try

        Return oMyResult

    End Function

    Public Function TemplateFunction() As CResult

        Dim oMyResult As New CResult

        Try

        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = "ERROR: [" & Get_Class() & "." & System.Reflection.MethodBase.GetCurrentMethod.Name & "] - " & ex.Message & " - Trace: " & ex.StackTrace & vbCrLf
        End Try

        Return oMyResult

    End Function

    Public Function Get_Class() As String

        Dim mb As System.Reflection.MethodBase
        Dim Type As Type
        Dim sFullReference As String = ""

        mb = System.Reflection.MethodBase.GetCurrentMethod
        Type = mb.DeclaringType

        sFullReference = Type.FullName

        Return sFullReference

    End Function

    Public Function SerialiseNodeFromXML(ByVal oXMLNode As System.Xml.XmlNode, ByVal oCLine As CCodeLine, Optional ByVal iUseKey As Long = 0) As String

        Dim oCNewNode As CCodeLine
        Dim sClass As String = ""
        ' Dim oXIParent As CCodeLine
        Dim sSubKey As String = ""
        Dim sNodeID As String = ""
        Dim sNodeName As String = ""
        Dim sNodeKey As String = ""
        Dim sNodeType As String = ""
        Dim sNameKey As String = ""
        Dim sResult As String = String.Empty

        ' ----- Ignore plain text nodes, as they are picked up
        '       by the inner-text code below.
        If (oXMLNode.NodeType = System.Xml.XmlNodeType.Text) Then Return ""

        ' ----- Treat the "<?xml..." node specially.
        If (oXMLNode.NodeType = System.Xml.XmlNodeType.XmlDeclaration) And (oCLine Is Nothing) Then

            Return ""
        End If

        ' ----- Add the node itself.
        If (oCLine Is Nothing) Then
            'baseNode = XMLTree.Nodes.Add(oXMLNode.Name)

        Else

            'If oCLine.sSubKey <> "" Then
            '    Try
            '        sNodeID = oXMLNode.Item(oCLine.sSubKey).InnerText
            '    Catch ex As Exception
            '        'tricky, if we are really in a collection then it will fail later as it tries to add nodes with the same key
            '        ' if we are not they it's ok to leave blank
            '    End Try

            'End If
            sNodeID = ""
            'Try
            '    sNodeID = oCLine.sName
            'Catch ex As Exception

            'End Try

            'oCNewNode = oCLine.AddNode(oXMLNode.Name, sNodeID, , oXMLNode)        'NNodes.Add(oXMLNode.Name)

            Try
                If oXMLNode.Attributes.Count > 0 Then
                    If Not oXMLNode.Attributes("name") Is Nothing Then
                        sNodeName = oXMLNode.Attributes("name").Value
                    Else
                        sNodeName = ""
                    End If
                    If Not oXMLNode.Attributes("key") Is Nothing Then
                        sNodeKey = oXMLNode.Attributes("key").Value
                    Else
                        sNodeKey = ""
                    End If
                    If Not oXMLNode.Attributes("type") Is Nothing Then
                        sNodeType = oXMLNode.Attributes("type").Value
                    Else
                        sNodeType = ""
                    End If
                End If
            Catch ex As Exception
                Debug.Print("XIScripting.CCodeLine.SerialiseNodeFromXML." & Err.Description)
            End Try

            If iUseKey = 10 Then      'use the key for the OM (should do really, but defaults to no)
                sNameKey = sNodeKey
            Else
                sNameKey = sNodeName
            End If

            'If oXMLNode.Name.ToLower = "value" Then
            '    oCNewNode = oCLine.AddElement(sNameKey, , , oXMLNode)
            '    oCNewNode.sValue = Replace(oXMLNode.InnerText, Chr(10), "")       'DS - not sure if this is some kind of bug or what, but keeps returning chr 10 (line feed i think). Me or xml, don't know
            '    oCNewNode.sType = sNodeType
            'ElseIf oXMLNode.Name.ToLower = "node" Then
            'oCNewNode = oCLine.AddNode(sNameKey, , , oXMLNode)
            oCNewNode = New CCodeLine
            'oCNewNode.sOperator
            oCLine.AddCodeLine(oCNewNode)
            oCNewNode.iType = Val(sNodeType)
            'End If
            ' ----- Add the child nodes.
            If (oXMLNode.ChildNodes IsNot Nothing) Then
                For Each subNode As System.Xml.XmlNode In oXMLNode.ChildNodes
                    Select Case subNode.NodeType
                        Case System.Xml.XmlNodeType.Attribute

                        Case System.Xml.XmlNodeType.CDATA

                        Case System.Xml.XmlNodeType.Comment


                        Case System.Xml.XmlNodeType.Element
                            If subNode.Name.ToLower = "Lines" Then  ' Or subNode.Name.ToLower = "values" Then   'so these are just collection markers - jump straight to the next level
                                SerialiseNodeFromXML(subNode, oCNewNode, iUseKey)        'oCLine)
                            ElseIf subNode.Name.ToLower = "Line" Then
                                If Not oCLine Is Nothing Then
                                    SerialiseNodeFromXML(subNode, oCLine, iUseKey)
                                End If
                            End If
                        Case Else
                    End Select

                Next subNode
            End If
        End If


        Return sResult


    End Function

    Public Function SerialiseNodeToXML() As String

        Dim oConcat As New System.Text.StringBuilder

        ''oConcat.Append("<XIDocument>")
        'oConcat.Append("<Values>")
        'For Each oChildXI In Me.NCodeLines.Values
        '    oConcat.Append("<Value").Append(" key='").Append(oChildXI.sKey).Append("' ").Append(" name='").Append(oChildXI.sName).Append("' ").Append(" type='").Append(oChildXI.iType).Append("'>").Append(vbCrLf).Append(oChildXI.sOperator).Append("</Value>").Append(vbCrLf)
        'Next
        'oConcat.Append("</Values>")

        oConcat.Append("<Lines>")
        For Each oChildXI In Me.NCodeLines.Values
            oConcat.Append("<Line").Append(" key='").Append(oChildXI.sKey).Append("' ").Append(" name='").Append(oChildXI.sName).Append("' ").Append(" type='").Append(oChildXI.iType).Append("'>").Append(vbCrLf).Append(oChildXI.SerialiseNodeToXML).Append("</Line>").Append(vbCrLf)
        Next
        oConcat.Append("</Lines>")
        'oConcat.Append("</XIDocument>")

        Return oConcat.ToString

    End Function
End Class
