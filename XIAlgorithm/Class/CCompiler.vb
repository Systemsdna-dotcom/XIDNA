Imports xiGenerics
Imports XISystem

Public Class CCompiler

    Public Function Compile_FromText(sSourceCode As String) As CResult

        Dim oMyResult As New CResult
        Dim oCR As CResult = Nothing
        Dim AsLines() As String
        Dim j As Long
        Dim oAlgo As New CAlgorithmDefinition
        Dim AsThisLine() As String
        Dim k As Long
        Dim sThisLine As String = ""
        Dim sIdentifier As String = ""
        Dim sMethodName As String = ""
        Dim sReturnD As String = ""
        Dim sErrorD As String = ""
        Dim sActionD As String = ""
        Dim sExecuteD As String = ""
        Dim sAlgoName As String = ""
        Dim oXIParam As CNodeItem = Nothing
        Dim AsParams() As String
        Dim sThisItem As String = ""
        Dim p As Long
        Dim sThisParam As String = ""
        Dim AsMethodItem() As String
        Dim sMethodSection As String = ""
        Dim sRemainingSection As String = ""
        Dim oMethodStepD As CMethodStepDefinition = Nothing
        Dim sIndent As String = ""
        Dim iIndent As Long
        Dim sScript As String = ""
        Dim iScriptStart As Long

        Try

            oMyResult.xiStatus = xiEnum.xiFuncResult.xiInProcess

            AsLines = Split(sSourceCode, vbCrLf)

            'build an OM of the sourcecode, don't try and interpret anything, but need to validate to ensure no invalids
            '  and that everything is on each line that should be
            For j = 0 To UBound(AsLines)
                sThisLine = AsLines(j)
                If sThisLine <> "" Then
                    AsThisLine = Split(sThisLine, "|")
                    If UBound(AsThisLine) > 0 Then
                        'first element always has to be identifier. If it is not, error the line
                        sIdentifier = AsThisLine(0).ToLower
                        If sIdentifier = "xi.ag" Then  'algorithm defintition
                            For k = 1 To UBound(AsThisLine)
                                sThisItem = AsThisLine(k)
                                If k = 1 Then
                                    sAlgoName = sThisItem
                                    oAlgo.sName = sAlgoName
                                Else
                                    'should be a param
                                    AsParams = Split(sThisItem, ",")
                                    For p = 0 To UBound(AsParams)
                                        sThisParam = AsParams(p)
                                        oXIParam = oAlgo.oXIParameters.NNode(sThisParam)
                                        'need to type it or anything??
                                    Next p
                                End If
                                'get the params

                            Next k
                        ElseIf sIdentifier = "xi.m" Then 'method
                            oMethodStepD = Nothing
                            sIndent = ""
                            iIndent = 0
                            For k = 1 To UBound(AsThisLine)
                                sThisItem = AsThisLine(k)

                                sMethodSection = ""
                                sRemainingSection = ""
                                AsMethodItem = Split(sThisItem, ":")
                                If UBound(AsMethodItem) > 0 Then
                                    For p = 0 To UBound(AsMethodItem)
                                        'what this element is, is defined by the first of these. So it might read 'r:xiR' which indicates the return. Although we could force the order, we do not, but that means you have to specify what this section is
                                        If p = 0 Then
                                            sMethodSection = AsMethodItem(p)
                                            sRemainingSection = sThisItem.Substring(Len(sMethodSection) + 1, Len(sThisItem) - Len(sMethodSection) - 1)
                                            Select Case Left(sMethodSection, 1)
                                                Case "s"
                                                    sIndent = sRemainingSection
                                                    iIndent = Len(sIndent)
                                                'Case "-" 'another way of 's' and i prefer it
                                                '    sIndent = sMethodSection
                                                '    iIndent = Len(sIndent)
                                                Case "i", "-" 'method identifier
                                                    'add a new method
                                                    If Left(sMethodSection, 1) = "-" Then
                                                        sIndent = sMethodSection
                                                        iIndent = Len(sIndent)
                                                    End If
                                                    sMethodName = sRemainingSection 'TO DO - validate

                                                    If oAlgo.oMethodDefinition.NMethodSteps.Contains(sMethodName.ToLower) Then
                                                        oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                                                        oMyResult.sAppend = "Warning: Step: '" & sMethodName & "' is not unique in this algorithm"
                                                    End If
                                                    oCR = oAlgo.oMethodDefinition.NMethodAdd(sMethodName)
                                                    If oCR.xiStatus <> xiEnum.xiFuncResult.xiSuccess Then
                                                        oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                                                        oMyResult.sAppend = "Error: Adding step: '" & sMethodName & "' :" & oCR.sMessage
                                                        Exit For
                                                    Else
                                                        oMethodStepD = oCR.oResult
                                                    End If
                                                    'oMethodStepD = oAlgo.oMethodDefinition.NMethod(sMethodName)
                                                    oMethodStepD.sStepName = sRemainingSection
                                                    oMethodStepD.iMethodType = CMethodStepDefinition.xiMethodType.xiMethod
                                                    oMethodStepD.sIndent = sIndent
                                                    oMethodStepD.iIndent = iIndent
                                                Case "r"
                                                    If Not oMethodStepD Is Nothing Then
                                                        sReturnD = sRemainingSection 'TO DO - validate
                                                        oMethodStepD.sReturnType = sReturnD
                                                    End If
                                                Case "e"
                                                    If Not oMethodStepD Is Nothing Then
                                                        sErrorD = sRemainingSection 'TO DO - validate
                                                        oMethodStepD.sErrorType = sErrorD
                                                    End If
                                                Case "a"
                                                    If Not oMethodStepD Is Nothing Then
                                                        sActionD = sRemainingSection 'TO DO - validate
                                                        oMethodStepD.sActionType = sActionD
                                                    End If
                                                Case "x"
                                                    If Not oMethodStepD Is Nothing Then
                                                        sExecuteD = sRemainingSection 'TO DO - validate
                                                        oMethodStepD.sExecute = sExecuteD
                                                    End If
                                            End Select


                                            Exit For
                                        End If
                                    Next p

                                    If oMethodStepD Is Nothing Then
                                        Exit For 'this line had an error, don't continue
                                    End If

                                Else
                                    If Left(sThisItem, 1) = "-" Then
                                        sIndent = sThisItem
                                        iIndent = Len(sIndent)
                                    End If

                                End If

                            Next k
                        ElseIf sIdentifier = "xi.s" Then 'method
                            oMethodStepD = Nothing
                            sIndent = ""
                            iIndent = 0
                            For k = 1 To UBound(AsThisLine)
                                sThisItem = AsThisLine(k)

                                sMethodSection = ""
                                sRemainingSection = ""
                                AsMethodItem = Split(sThisItem, ":")
                                If UBound(AsMethodItem) > 0 Then
                                    For p = 0 To UBound(AsMethodItem)
                                        'what this element is, is defined by the first of these. So it might read 'r:xiR' which indicates the return. Although we could force the order, we do not, but that means you have to specify what this section is
                                        If p = 0 Then
                                            sMethodSection = AsMethodItem(p)
                                            sRemainingSection = sThisItem.Substring(Len(sMethodSection) + 1, Len(sThisItem) - Len(sMethodSection) - 1)
                                            Select Case Left(sMethodSection, 1)
                                                Case "x"

                                                    'sScript = sRemainingSection
                                                    iScriptStart = InStr(sThisLine, "{") - 1
                                                    sScript = sThisLine.Substring(iScriptStart, Len(sThisLine) - iScriptStart)
                                                    oMethodStepD.sScript = sScript
                                                Case "s"
                                                    sIndent = sRemainingSection
                                                    iIndent = Len(sIndent)
                                                Case "i", "-" 'script identifier
                                                    'add a new method
                                                    If Left(sMethodSection, 1) = "-" Then
                                                        sIndent = sMethodSection
                                                        iIndent = Len(sIndent)
                                                    End If

                                                    sMethodName = sRemainingSection 'TO DO - validate

                                                    'oMethodStepD = oAlgo.oMethodDefinition.NMethod(sMethodName)

                                                    oCR = oAlgo.oMethodDefinition.NMethodAdd(sMethodName)
                                                    If oCR.xiStatus <> xiEnum.xiFuncResult.xiSuccess Then
                                                        oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                                                        oMyResult.sAppend = "Error adding step: '" & sMethodName & "' :" & oCR.sMessage
                                                        Exit For
                                                    Else
                                                        oMethodStepD = oCR.oResult
                                                    End If
                                                    oMethodStepD.sStepName = sRemainingSection
                                                    oMethodStepD.iMethodType = CMethodStepDefinition.xiMethodType.xiScript
                                                    oMethodStepD.sIndent = sIndent
                                                    oMethodStepD.iIndent = iIndent
                                                'Case "r"
                                                '    If Not oMethodStepD Is Nothing Then
                                                '        sReturnD = sRemainingSection 'TO DO - validate
                                                '        oMethodStepD.sReturnType = sReturnD
                                                '    End If
                                                Case "e"
                                                    If Not oMethodStepD Is Nothing Then
                                                        sErrorD = sRemainingSection 'TO DO - validate
                                                        oMethodStepD.sErrorType = sErrorD
                                                    End If
                                            End Select


                                            Exit For
                                        End If
                                    Next p

                                    If oMethodStepD Is Nothing Then
                                        Exit For 'this line had an error, don't continue
                                    End If
                                Else
                                    'Error - we didn't find an identifier for this section. is it a return, is it error or execute etc?
                                End If


                            Next k
                        Else
                            'TO DO - error 'unknown identifier: '
                        End If 'sIdentifier = "xi.ag"


                    Else
                        'error - not enough info on the line to do anything
                    End If  'UBound(AsThisLine) > 0 
                End If  'sThisLine <> ""
            Next j

            oCR = oAlgo.CompileOM()
            If oCR.xiStatus <> xiEnum.xiFuncResult.xiSuccess Then
                oMyResult.xiStatus = oCR.xiStatus
                oMyResult.sAppend = "Error in compilation Object Model: " & oCR.sMessage
                Return oMyResult
                Exit Function
            Else
                oCR = oAlgo.ValidateOM

            End If

            'now check to see if 

            oMyResult.oResult = oAlgo
            If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then
                oMyResult.xiStatus = xiEnum.xiFuncResult.xiSuccess
            Else
                oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
                oMyResult.sAppend = oCR.sMessage
            End If

        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
        End Try

        Return oMyResult

        '        xi.ag|DiaryTemplate|p.PolicyFK,p.dGoLive,p.TemplateID
        'xi.m|s:-|i:TemplateDiaries|r:BOInstanceGrid|e:DBLog, xiFail|a: m0.1Click|x: templatediaries
        '        xi.m|s:-|i:PolicyI|r:BOInstance|e:DBLog, xiFail|a: m.BOLoad|x:Policy_T, PolicyFK
        'xi.m|s:-|i:EachDiaryT|r:xiR|e:DBLog, xiFail|a: m.Iterate|x:xim.DiaryTemplate
        '        xi.m|s:--|i:CreateDiaryInstance|r:BOInstance|e:DBLog, xiFail|a: m.CreateBO|x:bod.Diary_T
        '        xi.m|s:--|i:CopyDiary|r:xiR|e:DBLog, xiFail|a: m.BOCopy|x:EachDiaryT, xim.CreateDiaryInstance, xig.CopyLive
        'xi.m|s:--|i:SetValue1|r:xiR|e:DBLog, xiFail|a: m.SetValue|x:xim.CreateDiaryInstance,[dDue=xim.EachDiary.iDaysFromZero+p.dGoLive]
        'xi.m|s:--|i:SetValue2|r:xiR|e:DBLog, xiFail|a: m.SetValue|x:xim.CreateDiaryInstance,[iType=0]
        'xi.m|s:--|i:SetValue3|r:xiR|e:DBLog, xiFail|a: m.SetValue|x:xim.CreateDiaryInstance,[FKiPolicyID=p.PolicyID]
        'xi.m|s:--|i:SetValue4|r:xiR|e:DBLog, xiFail|a: m.SetValue|x:xim.CreateDiaryInstance,[FKiClientID=xim.PolicyI.FKiClientID]
        'xi.m|s:--|i:InsertDiary|r:xiR|e:DBLog, xiFail|a: m.BOSave|x:xim.CreateDiaryInstance











    End Function

    Public Function CompileFromCXI(oCXI As CNodeItem) As CResult 'return a xiResult

        Dim oMyResult As New CResult
        Dim oCR As CResult = Nothing
        Dim oXIValue As CNodeItem

        Dim oAlgorithmDefinition As New CAlgorithmDefinition

        For Each oXIValue In oCXI.NElements.Values
            'param

            With oAlgorithmDefinition.oXIParameters.NNode(oXIValue.sValue)
                .ElementValue("name", oXIValue.sValue)
                .ElementValue("value", oXIValue.sName)
            End With
        Next

        CompileFromCXI_Recurse(oCXI, oAlgorithmDefinition.oMethodDefinition)

        Return oMyResult

    End Function

    Public Function CompileFromCXI_Recurse(oCXI As CNodeItem, oParentMethod As CMethodStepDefinition) As CResult

        Dim oMyResult As New CResult
        Dim oCR As CResult
        Dim sError As String = ""
        Dim sName As String = ""
        Dim sExecute As String = ""
        Dim sReturn As String = ""
        Dim oSubXI As CNodeItem
        Dim oChildMethod As CMethodStepDefinition = Nothing


        For Each oXIValue In oCXI.NNodeItems.Values
            'check validity of each including compiling of the xiscript in execute
            sName = oXIValue.NElements("name").sValue
            sReturn = oXIValue.NElements("result").sValue
            sError = oXIValue.NElements("error").sValue
            sExecute = oXIValue.NElements("execute").sValue

            'TO DO - check the validity of these aspects and if they are not valid return warnings and/or errors with this info


            oChildMethod = oParentMethod.NMethod(sName)
            With oChildMethod
                .sStepName = sName
                .sReturnType = sReturn
                .sErrorType = sError
                .sExecute = sExecute
            End With

            For Each oSubXI In oCXI.NNodeItems.Values
                oCR = CompileFromCXI_Recurse(oSubXI, oChildMethod)
            Next

        Next



        Return oMyResult

    End Function
End Class
