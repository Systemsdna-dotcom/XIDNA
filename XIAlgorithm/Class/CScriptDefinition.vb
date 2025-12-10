Imports XISystem
Imports XISystem2

Public Class CScriptDefinition


    Dim oNCodeLines As New Dictionary(Of String, CCodeLine)
    Dim oTopNode As New CCodeLine


    'Public Property Name() As String
    '    Get
    '        Return sName
    '    End Get
    '    Set(ByVal value As String)
    '        sName = value
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

    Public Function XILoad_Script(ByVal sXMLFile As String) As CResult

        Dim oMyResult As New CResult
        ' Dim oCodeLine As New CCodeLine

        Try


            Dim oXMLBase As System.Xml.XmlDocument
            Dim sProblem As String

            If My.Computer.FileSystem.FileExists(sXMLFile) Then
                'My.Computer.FileSystem.ReadAllText(sConfigXML)
                Try
                    oXMLBase = New System.Xml.XmlDocument
                    oXMLBase.Load(sXMLFile)

                    For Each oXMLNode As System.Xml.XmlNode In oXMLBase.ChildNodes
                        oTopNode.SerialiseNodeFromXML(oXMLNode, oTopNode)      ' oMyXI)
                    Next oXMLNode
                    'xiXML2OM(oXMLBase, oMyXI)


                Catch ex As Exception
                    sProblem = "The XML file could not be loaded due to " & _
                       "the following error:" & vbCrLf & vbCrLf & _
                       ex.Message
                    oXMLBase = Nothing

                End Try

            Else
                sProblem = "File " & sXMLFile & " does not exist"
            End If

        Catch ex As Exception

        End Try

        Return oMyResult

    End Function

    Public Function Get_Lines_Recurse(ByVal qParent As List(Of XElement), ByVal oParentLine As CCodeLine) As CResult     'System.Xml.Linq.XElement

        Dim oMyResult As New CResult
        Dim oCodeLine As CCodeLine
        Dim oCR As CResult

        Try
            'TO DO - THIS IS RECURSIVE - USE XIGENERICS METHOD
            Dim qLines = From Nodes In qParent.Descendants("line") Select Nodes

            For Each oResult In qLines
                oCodeLine = New CCodeLine
                oCodeLine.sOperator = oResult.Elements("Operator").Value
                oCodeLine.sResolvedValue = oResult.Elements("Value").Value
                oCodeLine.sResult = oResult.Elements("Value").Value

                oCR = oParentLine.AddCodeLine(oCodeLine)
                If oCR.xiStatus = xiEnum.xiFuncResult.xiSuccess Then
                    'oCR = Get_Lines_Recurse(oResult.tolist, oCodeLine)
                    'qParent = From nodes In oResult.Descendants("xiScript") Select nodes
                    'oCR = Get_Lines_Recurse(oResult.tolist, oCodeLine)
                End If

                'oCodeLine.Name = oResult.Elements("name").Value
                'sParamName = oCodeLine.Name
                'oCodeLine.Type = oResult.Elements("type").Value
                'oCodeLine.Value = oResult.Elements("value").Value
                'oCodeLine.ParamOpt = oResult.Elements("optional").Value
                'oNParameters.Add(sParamName, oCodeLine)
            Next
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

    Public Function TemplateFunction() As CResult

        Dim oMyResult As New CResult

        Try

        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = "ERROR: [" & Get_Class() & "." & System.Reflection.MethodBase.GetCurrentMethod.Name & "] - " & ex.Message & " - Trace: " & ex.StackTrace & vbCrLf
        End Try

        Return oMyResult

    End Function

    Public Function Serialise_FromOM() As String

        Dim sResult As String = String.Empty
        'Dim sConcat As String = ""
        'sConcat = sName & vbCrLf
        'For Each oParam In NParameter
        '    sConcat = sConcat & "Param  :  " & oParam.Value.Name & vbCrLf & vbTab & oParam.Value.Type & vbCrLf & vbTab & oParam.Value.Value & vbCrLf & vbTab & oParam.Value.ParamOpt & vbCrLf
        'Next
        'sConcat = sConcat & vbCrLf

        'For Each oSteps In NStep
        '    sConcat = sConcat & "Step  : " & oSteps.Value.Name & vbCrLf & vbTab & oSteps.Value.Type & vbCrLf & vbTab & oSteps.Value.QueryName & vbCrLf & vbTab & oSteps.Value.Resolve & vbCrLf

        '    For Each oStepNod In oSteps.Value.NStepNode
        '        sConcat = sConcat & vbTab & "StepNode : " & oStepNod.Value.Name & vbCrLf & vbTab & vbTab & oStepNod.Value.NodeName & vbCrLf & vbTab & vbTab & oStepNod.Value.UIDParam & vbCrLf

        '        For Each oStepGrp In oStepNod.Value.NStepGroup
        '            sConcat = sConcat & vbTab & vbTab & "StepGroup : " & oStepGrp.Value.Load & vbCrLf & vbTab & vbTab & vbTab & oStepGrp.Value.List & vbCrLf & vbTab & vbTab & vbTab & oStepGrp.Value.Edit & vbCrLf & vbTab & vbTab & vbTab & oStepGrp.Value.Lock & vbCrLf & vbTab & vbTab & vbTab & oStepGrp.Value.OrderBy & vbCrLf
        '        Next
        '    Next

        '    For Each oStepNRef In oSteps.Value.NStepNodeRef
        '        sConcat = sConcat & vbTab & "StepNodeRef : " & oStepNRef.Value.StepName & vbCrLf & vbTab & vbTab & vbTab & oStepNRef.Value.StepNode & vbCrLf
        '    Next
        '    For Each oStepBtn In oSteps.Value.NButton
        '        sConcat = sConcat & vbTab & "Buttons:" & oStepBtn.Value.Link & vbCrLf & vbTab & vbTab & vbTab & oStepBtn.Value.Text & vbCrLf
        '    Next
        '    sConcat = sConcat & vbTab & oSteps.Value.NextStep & vbCrLf
        '    sConcat = sConcat & vbTab & oSteps.Value.ListStep & vbCrLf
        '    sConcat = sConcat & vbCrLf
        'Next

        'AppFlowFormat = sConcat
        Return sResult
    End Function


End Class
