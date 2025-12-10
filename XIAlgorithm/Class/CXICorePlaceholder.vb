Imports XISystem

Public Class CXICorePlaceholder

    Public Function BOCreate(sBOName As String) As CResult

        Dim oCResult As CResult = New CResult

        Dim oNewBOPlaceholder As New CBOPlaceholder
        oNewBOPlaceholder.sName = "BO CREATE"
        oCResult.oResult = oNewBOPlaceholder

        Return oCResult


    End Function

    Public Function BOLoad(sBOName As String, sUID As String) As CResult

        Dim oCResult As CResult = New CResult

        Dim oNewBOPlaceholder As New CBOPlaceholder
        oNewBOPlaceholder.sName = "BO LOAD"
        oCResult.oResult = oNewBOPlaceholder

        Return oCResult


    End Function

    Public Function BOSave(oBO As CBOPlaceholder, Optional sGroup As String = "") As CResult

        Dim oCResult As CResult = New CResult

        oBO.sName = "SAVE: " & oBO.sName
        oCResult.oResult = "xiSuccess"

        Return oCResult


    End Function

    'BO1Click
    Public Function BO1Click(s1ClickName As String) As CResult

        Dim oCResult As CResult = New CResult
        Dim j As Long
        Dim oBOI As CBOPlaceholder = Nothing

        'check for UID also?

        Dim oNewBOInstPlaceholder As New BOInstanceGridPlaceholder
        oNewBOInstPlaceholder.sName = "1 Click"

        For j = 1 To 2
            oBOI = New CBOPlaceholder
            oBOI.sName = "Sub Item " & j
            oNewBOInstPlaceholder.oBOInstances.Add("Item" & j, oBOI)
        Next j

        oCResult.oResult = oNewBOInstPlaceholder

        Return oCResult


    End Function

    Public Function BOCopy(oBOFrom As CBOPlaceholder, oBOTo As CBOPlaceholder, Optional sCopyGroup As String = "") As CResult


        Dim oCResult As CResult = New CResult

        'Dim oNewBOPlaceholder As New CBOPlaceholder

        'oNewBOPlaceholder.sName = "BO COPY"
        'oCResult.oResult = oNewBOPlaceholder

        oBOTo.sName = oBOTo.sName & ": COPY FROM: " & oBOFrom.sName

        oCResult.xiStatus = xiEnum.xiFuncResult.xiSuccess
        oCResult.oResult = "xiSuccess"
        Return oCResult


    End Function

    Public Function BOSetValue(oBO As CBOPlaceholder, sValueMethod As String) As CResult


        Dim oCResult As CResult = New CResult


        oBO.sName = oBO.sName & ": S:" & sValueMethod

        oCResult.xiStatus = xiEnum.xiFuncResult.xiSuccess
        oCResult.oResult = "xiSuccess"
        Return oCResult


    End Function

End Class
