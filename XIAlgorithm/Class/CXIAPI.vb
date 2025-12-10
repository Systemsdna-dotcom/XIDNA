Imports XISystem
Imports XISystem2

Public Class CXIAPI

    Public Function LoadBO(sBODef As String, iInstID As String) As CResult

        Dim oMyResult As New CResult


        Try



            'oMyResult.oResult =
        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = oMyResult.sMessage & ex.Message & vbCrLf
        End Try

        Return oMyResult

    End Function

    Public Function Execute_1Click(i1ClickID As Long) As CResult

        Dim oMyResult As New CResult


        Try



            'oMyResult.oResult =
        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = oMyResult.sMessage & ex.Message & vbCrLf
        End Try

        Return oMyResult

    End Function

    Public Function LoadBOFK(sBODef As String, iInstID As String, sFKAttr As String) As CResult

        Dim oMyResult As New CResult


        Try



            'oMyResult.oResult =
        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = oMyResult.sMessage & ex.Message & vbCrLf
        End Try

        Return oMyResult

    End Function

    Public Function Attr(sBODef As String, iInstID As String, sAttrName As String, Optional sAttrValue As String = "", Optional sLoadByAttr As String = "", Optional IsAudit As String = "") As CResult

        Dim oMyResult As New CResult


        Try

            'get the attr value
            Dim oMyData As New CDataLoad
            Dim Params As List(Of CNV) = New List(Of CNV)()
            Params.Add(New CNV With {
        .sName = "sBOName",
        .sValue = sBODef
    })
            Params.Add(New CNV With {
        .sName = "sInstanceID",
        .sValue = iInstID
    })
            Params.Add(New CNV With {
        .sName = "sAttrName",
        .sValue = sAttrName
    })
            Params.Add(New CNV With {
        .sName = "sLoadByAttr",
        .sValue = sLoadByAttr
    })
            Params.Add(New CNV With {
        .sName = "IsAudit",
        .sValue = IsAudit
    })
            If String.IsNullOrEmpty(sAttrValue) Then
                oMyResult = CType(oMyData.API_Load("GetBOAttribute", Params), CResult)
            Else
                Params.Add(New CNV With {
        .sName = "sAttrValue",
        .sValue = sAttrValue
    })
                oMyResult = CType(oMyData.API_Load("SaveBOAttribute", Params), CResult)
            End If

            ' If sLoadByAttr="" then load by PK

            'TEMP
            'oMyResult.oResult = "1234"
        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = oMyResult.sMessage & ex.Message & vbCrLf
        End Try

        Return oMyResult

    End Function

    'API
    Public Function API(sConcatRequest As String) As CResult

        Dim oMyResult As New CResult


        Try
            'TO DO - split the concat request, it should be in square brackets
            'TEMP!!!!!!!!!!!!!
            If sConcatRequest = "logintype" Then
                oMyResult.oResult = "private"

            End If




            'oMyResult.oResult =
        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = oMyResult.sMessage & ex.Message & vbCrLf
        End Try

        Return oMyResult

    End Function

    'Reserved
    Public Function Reserved(sReservedWord As String) As CResult

        Dim oMyResult As New CResult


        Try
            Dim oMyData As New CDataLoad
            Select Case sReservedWord
                Case "currentuser"
                    'TEMP!!!!
                    oMyResult = CType(oMyData.API_Load("GetUserRole"), CResult)

                    If oMyResult.oResult = Nothing Then
                        'oMyResult.oResult = ""
                    End If

                    'oMyResult.oResult = "DAN"
                Case "currentuserid"
                    'TEMP!!!!
                    oMyResult = CType(oMyData.API_Load("GetUserID"), CResult)

                    If oMyResult.oResult = Nothing Then
                        'oMyResult.oResult = ""
                    End If

                    'oMyResult.oResult = "DAN"

            End Select

            'oMyResult.oResult =
        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = oMyResult.sMessage & ex.Message & vbCrLf
        End Try

        Return oMyResult

    End Function

    Public Function AttrFormatted(sBODef As String, iInstID As String, sAttrName As String) As CResult

        Dim oMyResult As New CResult


        Try



            'oMyResult.oResult =
        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = oMyResult.sMessage & ex.Message & vbCrLf
        End Try

        Return oMyResult

    End Function

    Public Function AttrFK(sBODef As String, iInstID As String, sFKAttrName As String, sAttrName As String) As CResult

        Dim oMyResult As New CResult


        Try



            'oMyResult.oResult =
        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = oMyResult.sMessage & ex.Message & vbCrLf
        End Try

        Return oMyResult

    End Function

    Public Function UserAttribute(sAttrName As String) As CResult

        Dim oMyResult As New CResult


        Try



            'oMyResult.oResult =
        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = oMyResult.sMessage & ex.Message & vbCrLf
        End Try

        Return oMyResult

    End Function

    Public Function UserRole(sRoleName As String) As CResult

        Dim oMyResult As New CResult


        Try
            'in this role or not?


            'oMyResult.oResult =
        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = oMyResult.sMessage & ex.Message & vbCrLf
        End Try

        Return oMyResult

    End Function

    '
    Public Function QuestionSetFieldValue(sStepID As String, sFieldID As String) As CResult

        Dim oMyResult As New CResult


        Try
            'ASSUMES CURRENT QS INSTANCE, but should we allow any QS?


            'oMyResult.oResult =
        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = oMyResult.sMessage & ex.Message & vbCrLf
        End Try

        Return oMyResult

    End Function

    Public Function Parameter(sParamName As String, sGUID As String, sSessionID As String) As CResult

        Dim oMyResult As New CResult


        Try
            Dim oMyData As New CDataLoad
            Dim Params As List(Of CNV) = New List(Of CNV)()
            Params.Add(New CNV With {
        .sName = "sGUID",
        .sValue = sGUID
    })
            Params.Add(New CNV With {
        .sName = "sParamName",
        .sValue = sParamName
    })
            Params.Add(New CNV With {
        .sName = "sSessionID",
        .sValue = sSessionID
    })
            oMyResult = CType(oMyData.API_Load("Parameter", Params), CResult)

            'oMyResult.oResult =
        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = oMyResult.sMessage & ex.Message & vbCrLf
        End Try

        Return oMyResult

    End Function

    'QSXIValue
    Public Function QSXIValue(sParamName As String, iInstID As String, sGUID As String, sSessionID As String) As CResult

        Dim oMyResult As New CResult


        Try
            Dim oMyData As New CDataLoad
            Dim Params As List(Of CNV) = New List(Of CNV)()
            Params.Add(New CNV With {
       .sName = "sGUID",
       .sValue = sGUID
   })
            Params.Add(New CNV With {
       .sName = "sAttrName",
       .sValue = sParamName
   })
            Params.Add(New CNV With {
       .sName = "sInstanceID",
       .sValue = iInstID
   })
            Params.Add(New CNV With {
       .sName = "sSessionID",
       .sValue = sSessionID
   })
            '        Params.Add(New CNV With {
            '    .sName = "sBOName",
            '    .sValue = sBODef
            '})
            oMyResult = CType(oMyData.API_Load("GetQSAttributeValue", Params), CResult)
            'GET FROM Questionset - this step
            'TEMP:
            'oMyResult.oResult = "1234"
        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = oMyResult.sMessage & ex.Message & vbCrLf
        End Try

        Return oMyResult

    End Function

    Public Function QSStepXIValue(sStepName As String, sParamName As String) As CResult

        Dim oMyResult As New CResult


        Try

            'GET FROM Questionset - named step
            'oMyResult.oResult =
        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = oMyResult.sMessage & ex.Message & vbCrLf
        End Try

        Return oMyResult

    End Function

    Public Function OM(Optional sMethod As String = "", Optional sUser As String = "", Optional sNotificationType As String = "", Optional sDoc As String = "") As CResult

        Dim oMyResult As New CResult


        Try

            'get the attr value
            Dim oMyData As New CDataLoad
            Dim Params As List(Of CNV) = New List(Of CNV)()
            Params.Add(New CNV With {
        .sName = "sMethod",
        .sValue = sMethod
    })
            Params.Add(New CNV With {
        .sName = "sUser",
        .sValue = sUser
    })
            Params.Add(New CNV With {
        .sName = "sNotificationType",
        .sValue = sNotificationType
    })
            Params.Add(New CNV With {
        .sName = "sDoc",
        .sValue = sDoc
    })
            oMyResult = CType(oMyData.API_Load("OM", Params), CResult)
        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            oMyResult.sMessage = oMyResult.sMessage & ex.Message & vbCrLf
        End Try

        Return oMyResult

    End Function

    'Public Function setattr(sName As String, sValue As String, sGUID As String) As CResult

    '    Dim oMyResult As New CResult


    '    Try

    '        'get the attr value
    '        Dim oMyData As New CDataLoad
    '        Dim Params As List(Of CNV) = New List(Of CNV)()
    '        Params.Add(New CNV With {
    '    .sName = "sName",
    '    .sValue = sName
    '})
    '        Params.Add(New CNV With {
    '    .sName = "sValue",
    '    .sValue = sValue
    '})
    '        Params.Add(New CNV With {
    '    .sName = "sGUID",
    '    .sValue = sGUID
    '})
    '        oMyResult = CType(oMyData.API_Load("SetBOAttribute", Params), CResult)

    '        ' If sLoadByAttr="" then load by PK

    '        'TEMP
    '        'oMyResult.oResult = "1234"
    '    Catch ex As Exception
    '        oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
    '        oMyResult.sMessage = oMyResult.sMessage & ex.Message & vbCrLf
    '    End Try

    '    Return oMyResult

    'End Function
End Class
