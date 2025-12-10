

Imports XISystem

Public Class CScriptController
    Private sMyGUID As String
    Private sMySessionID As String
    'eg:
    '    func.{div|{add|{-|9,2},{getattr|'BO','attr1'}}},{-|7,3}}


    'func.{
    '   div|
    '       {add|{-|9,2},
    '           {getattr|'BO','attr1'}
    '       },
    '       {-|7,3}
    '}


    Private oTopLine As New CCodeLine

    Public Function API_ExecuteScript(ByVal sScript As String) As CResult

        Dim oMyResult As New CResult
        Dim oCR As CResult

        Try
            oCR = API2_Serialise_From_String(sScript)
            'so OM should now be built
            If oCR.xiStatus = xiEnum.xiFuncResult.xiError Then
                Return oCR
                Exit Function
            Else
                oCR = API2_ExecuteMyOM()
                Return oCR
                Exit Function
            End If
        Catch ex As Exception

        End Try

        Return oMyResult

    End Function

    Public Function API2_Serialise_From_String(ByVal sSerialFunction As String) As CResult

        Dim oMyResult As New CResult
        Dim sOriginal As String

        Try
            'ok, so the logic is to work inwards and match up brackets from either end
            '  every time we locate a bracket we will get the end one and then that substring is the function code
            '  and then that line de-codes itself

            sOriginal = sSerialFunction  'sOriginal is used for extracting the original strings between quotes


            sSerialFunction = sSerialFunction
            If Left(sSerialFunction, 5) = "func." Then
                sSerialFunction = sSerialFunction.Substring(5, Len(sSerialFunction) - 5)
                sOriginal = sOriginal.Substring(5, Len(sOriginal) - 5)
            ElseIf Left(sSerialFunction, 4) = "xi.s" Then
                sSerialFunction = sSerialFunction.Substring(4, Len(sSerialFunction) - 4)
                sOriginal = sOriginal.Substring(4, Len(sOriginal) - 4)
            ElseIf Left(sSerialFunction, 3) = "xi." Then
                sSerialFunction = sSerialFunction.Substring(3, Len(sSerialFunction) - 3)
                sOriginal = sOriginal.Substring(3, Len(sOriginal) - 3)
            End If


            'use whatever brackets you like
            sSerialFunction = sSerialFunction.Replace("(", "{")
            sSerialFunction = sSerialFunction.Replace(")", "}")
            sSerialFunction = sSerialFunction.Replace("[", "{")
            sSerialFunction = sSerialFunction.Replace("]", "}")
            '2018.05.16
            oTopLine = New CCodeLine

            oTopLine.DeSerialiseMe(sSerialFunction, sOriginal)
        Catch ex As Exception

        End Try

        Return oMyResult

    End Function

    'Public Function API2_Serialise_To_String() As CResult

    '    Dim oMyResult As New CResult

    '    Try

    '    Catch ex As Exception

    '    End Try

    '    Return oMyResult

    'End Function

    Public Function API2_ExecuteMyOM(Optional sKeyEx As String = "", Optional oCXIAPIEx As CXIAPI = Nothing, Optional oXIAlgoIEx As CAlgorithmInstance = Nothing, Optional sGUID As String = "", Optional sSessionID As String = "") As CResult

        Dim oMyResult As New CResult
        Dim oCR As CResult

        Try
            sMyGUID = sGUID
            sMySessionID = sSessionID
            'so the OM is built. At each level there should be the function name (operator)
            '  and then the sub-objects as parameters. If they don't have an operator and they have no sub-params then
            '  that is an actual value which needs to be resolved
            'so this is recursive. execute each level, which checks sub-levels and executes them first

            'TO DO - THERE IS NO SINGLE POINT OF REFERENCE FOR THE LINES, EACH ONLY HAS WHAT YOU PASS DOWN
            oCR = oTopLine.ExecuteMe(sKeyEx, oCXIAPIEx, oXIAlgoIEx, sMyGUID, sSessionID)
            oMyResult = oCR
            'If oCR.xiStatus = xiEnum.xiFuncResult.xiError Then
            '    oMyResult = oCR
            'Else
            '    oMyResult.oResult=oCR.
            'End If

            'If oCR.xiStatus = xiEnum.xiFuncResult.xiError Then

            'Else

            'End If

        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = "ERROR: [" & Get_Class() & "." & System.Reflection.MethodBase.GetCurrentMethod.Name & "] - " & ex.Message & " - Trace: " & ex.StackTrace & vbCrLf
        End Try

        Return oMyResult

    End Function

    Public Function API_ParsedFunction() As CResult

        Dim oMyResult As New CResult

        Try
            oMyResult = oTopLine.ParsedLine(0)
        Catch ex As Exception

        End Try

        Return oMyResult

    End Function

    Public Function API_FormattedFunction() As CResult

        Dim oMyResult As New CResult

        Try
            oMyResult = oTopLine.FormattedLine(0)
        Catch ex As Exception

        End Try

        Return oMyResult

    End Function

    Public Function API_ResetResults() As CResult

        Dim oMyResult As New CResult

        Try
            'TO DO 
            oTopLine.ResetResults()
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

    Public Property Switch_sOpenBracket() As String
        Get
            Return sOB
        End Get
        Set(ByVal value As String)
            sOB = value
        End Set
    End Property

    Public Property Switch_sCloseBracket() As String
        Get
            Return sCB
        End Get
        Set(ByVal value As String)
            sCB = value
        End Set
    End Property

    Public Property Switch_sFunctionDelimiter() As String
        Get
            Return sFDelim
        End Get
        Set(ByVal value As String)
            sFDelim = value
        End Set
    End Property

    Public Property Switch_sParameterDelimiter() As String
        Get
            Return sParamDelim
        End Get
        Set(ByVal value As String)
            sParamDelim = value
        End Set
    End Property

    Public Function Get_Class() As String

        Dim mb As System.Reflection.MethodBase
        Dim Type As Type
        Dim sFullReference As String = ""

        mb = System.Reflection.MethodBase.GetCurrentMethod
        Type = mb.DeclaringType

        sFullReference = Type.FullName

        Return sFullReference

    End Function

End Class
