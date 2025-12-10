Imports XISystem

Public Class CTypeConvertor

    Public Function EnumStringToOp(ByVal sOperator As String) As CResult

        Dim oMyResult As New CResult
        'Dim oCR As CResult
        Dim xiOperator As xiEnum.xiQryOperator

        Try

            xiOperator = DirectCast([Enum].Parse(GetType(xiEnum.xiQryOperator), sOperator), xiEnum.xiQryOperator)

            oMyResult.oResult = xiOperator

        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = oMyResult.sMessage & "ERROR: [" & Get_Class() & "." & System.Reflection.MethodBase.GetCurrentMethod.Name & "] - " & ex.Message & " - Trace: " & ex.StackTrace & vbCrLf
        End Try


        Return oMyResult

    End Function

    Public Function EnumOpToString(ByVal xiOperator As xiEnum.xiQryOperator) As CResult

        Dim oFuncResult As New CResult
        Dim sOperator As String = ""
        Dim sOutput As String = String.Empty

        Try


            sOutput = xiOperator.ToString()
            'sOutput = CInt(xiOperator).ToString()

            'Select Case xiOperator
            '    Case xiEnum.xiQryOperator.xiEQ
            '        sOperator = "="
            '    Case xiEnum.xiQryOperator.xiGT
            '        sOperator = ">"
            '    Case xiEnum.xiQryOperator.xiGTEQ
            '        sOperator = ">="
            '    Case xiEnum.xiQryOperator.xiLT
            '        sOperator = "<"
            '    Case xiEnum.xiQryOperator.xiLTEQ
            '        sOperator = "<="

            '        'TO DO - HOW TO HANDLE??????
            '    Case xiEnum.xiQryOperator.xiLike
            '        sOperator = ""
            '    Case xiEnum.xiQryOperator.xiStarts
            '        sOperator = ""
            'End Select

        Catch ex As Exception

        End Try

        oFuncResult.oResult = sOperator

        Return oFuncResult

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

End Class
