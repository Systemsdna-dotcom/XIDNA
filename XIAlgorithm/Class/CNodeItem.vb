Imports System.Collections.Specialized
Imports XIAlgorithm.xiEnum
Imports xiEnum
Imports XISystem

Public Class CNodeItem
    Private oNodeItems As New OrderedDictionary  '( (Of String, CNodeItem)

    Private iMyDirty As xiDirty
    Private sMyName As String = ""
    Public Property sName() As String
        Get
            Return sMyName
        End Get
        Set(ByVal value As String)
            sMyName = Trim(value)
        End Set
    End Property

    Private sMyValue As String = ""
    Public Property sValue() As String
        Get
            Return sMyValue
        End Get
        Set(ByVal value As String)
            If value <> sMyValue Then
                If iMyDirty = xiDirty.xiClean Then iMyDirty = xiDirty.xiDirty
                sMyValue = Trim(value)
            End If

        End Set
    End Property

    'oCStep.oParentMethod.NMethodSteps.Keys(oCStep.iChildIndex + 1)
    Public Function Get_NodeByNumber(iIndex As Long) As CNodeItem

        Dim oReturnItem As CNodeItem = Nothing
        Dim sKey As String = ""

        Try
            iIndex = iIndex - 1  '1 based, not zero based. If you think about it 'get my zero'th child' is a bit weird
            sKey = oNodeItems.Keys(iIndex)
            oReturnItem = oNodeItems(sKey)
        Catch ex As Exception

        End Try

        Return oReturnItem

    End Function

    Public Property NNodeItems() As OrderedDictionary '(Of String, CNodeItem)
        Get
            Return oNodeItems
        End Get
        Set(ByVal value As OrderedDictionary)
            '(Of String, CNodeItem))
            oNodeItems = value
        End Set
    End Property

    Private oMyParentNode As CNodeItem
    Public Property oParentNode() As CNodeItem
        Get
            Return oMyParentNode
        End Get
        Set(ByVal value As CNodeItem)
            oMyParentNode = value
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

    Private iMyChildCount As Long
    Public Property iChildCount() As Long
        Get
            Try
                iMyChildCount = oNodeItems.Count
            Catch ex As Exception

            End Try

            Return iMyChildCount
        End Get
        Set(ByVal value As Long)
            iMyChildCount = value
        End Set
    End Property

    Public Function AddNodeObject(oNodeToAdd As CNodeItem) As CResult
        Dim oResult As CResult = New CResult

        Try
            oNodeToAdd.iChildIndex = NNodeItems.Count
            oNodeItems.Add(oNodeToAdd.sKey, oNodeToAdd)
            oNodeToAdd.oParentNode = Me

            oResult.xiStatus = xiEnum.xiFuncResult.xiSuccess
        Catch ex As Exception
            oResult.xiStatus = xiEnum.xiFuncResult.xiError
            oResult.sMessage = ex.Message & " - Stack: " & ex.StackTrace
        End Try


        Return oResult

    End Function

    Public Function NNode(ByVal sNodeName As String, Optional ByVal sKey As String = "") As CNodeItem
        'essentially we build the key up in here to make it easier
        Dim sConcatKey As String = ""
        Dim sNewKey As String = ""
        Dim oNewM As CNodeItem = Nothing
        'Dim oCxi As Cxi

        Try


            If sKey = "" Then
                sConcatKey = sNodeName
            Else
                sConcatKey = sNodeName
            End If


            If sConcatKey <> "" Then
                Try
                    If oNodeItems.Contains(sConcatKey.ToLower) Then
                        NNode = oNodeItems(sConcatKey.ToLower)
                    Else
                        oNewM = AddNode(sConcatKey.ToLower)
                        sNewKey = oNewM.sKey
                        'sNewKey = AddNode(sConcatKey.ToLower).sKey
                        Try

                            NNode = oNodeItems(sNewKey)
                            NNode.sName = sNodeName
                        Catch ex2 As Exception
                            NNode = Nothing  'serious problem
                        End Try
                    End If
                Catch ex As Exception
                    Debug.Print("XIGenerics.CXI.NNodeMeta.Error")
                    NNode = Nothing
                End Try
            Else
                NNode = Nothing  'serious problem
            End If

        Catch ex As Exception
            NNode = Nothing  'serious problem
        End Try
    End Function

    Function AddNode(ByVal sNodeName As String) As CNodeItem

        Dim oNew As New CNodeItem
        Dim sKey As String
        Dim sFirstChar As String = ""
        Dim sName As String
        Dim bDataTypeAssigned As Boolean = False
        Dim sGlobalKey As String = ""

        Try

            sName = sNodeName
            sKey = sNodeName.ToLower
            If oNodeItems.Contains(sKey) Or sKey = "" Then
                'TO DO - error or can we use a generated key??
                'sKey = Get_UID()
            End If

            oNew = New CNodeItem
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
            'oNew.sClass = sNodeName
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
            '    'sKey = oNew.oBaseClass.Get_Key(sNodeName, Me, oXMLNode)       ', sNodeId
            '    sGlobalKey = "M." & oNew.sUID
            'Else
            '    'be careful then - why keep keys the same??
            '    sGlobalKey = oCopyFrom.sKey
            'End If

            'oNew.sUID = sGlobalKey        'do this in the get_uid itself
            oNew.sKey = sKey
            oNodeItems.Add(sKey, oNew)        'keep to the original - in the local collection this is the reference
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



    Private sMyKey As String
    Public Property sKey() As String
        Get
            Return sMyKey
        End Get
        Set(ByVal value As String)
            sMyKey = value
        End Set
    End Property

    Private oMyNElements As New Dictionary(Of String, CNodeItem)

    Public Property NElements() As Dictionary(Of String, CNodeItem)
        Get
            Return oMyNElements
        End Get
        Set(ByVal value As Dictionary(Of String, CNodeItem))
            oMyNElements = value
        End Set
    End Property


    Function AddElement(ByVal sElementName As String, Optional ByVal sNodeId As String = "", Optional ByVal oCopyFrom As CNodeItem = Nothing, Optional ByVal oXMLNode As System.Xml.XmlNode = Nothing, Optional ByVal bHungarian As Boolean = False, Optional ByVal sSpecifiedGivenKey As String = "") As CNodeItem

        Dim oNew As New CNodeItem   'Object  'New Cxi
        Dim sKey As String
        Dim sFirstChar As String = ""
        Dim sName As String
        Dim bDataTypeAssigned As Boolean = False
        Dim sGlobalKey As String = ""

        Try

            sName = sElementName
            sKey = LCase(sElementName)
            If oMyNElements.ContainsKey(sKey) Or sKey = "" Then
                'sKey = Get_UID()
            End If

            oNew = New CNodeItem
            Try
                sFirstChar = sName.Substring(0, 1)

            Catch exName As Exception

            End Try
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
            '    sName = sName.Substring(1, sName.Length - 1)
            'End If

            'oNew.oParent = Me
            'oNew.oBaseClass = Me.oBaseClass
            'oNew.sClass = sElementName
            'oNew.iLevel = Me.iLevel + 1
            'oNew.sUID = Get_UID()
            'oNew.sName = sName
            'If sSpecifiedGivenKey = "" Then
            '    oNew.sGivenKey = NextKey()
            'Else
            '    oNew.sGivenKey = sSpecifiedGivenKey
            'End If
            'oNew.sGivenKey = NextKey()
            'oNew.xiBaseType = tBaseType.xiElement

            'iMyElementCount = iMyElementCount + 1
            'If oCopyFrom Is Nothing Then
            '    'sKey = oNew.oBaseClass.Get_Key(sElementName, Me, oXMLNode)       ', sNodeId
            '    sGlobalKey = "V." & oNew.sUID
            'Else
            '    'be careful then - why keep keys the same??
            '    sGlobalKey = oCopyFrom.sKey
            'End If

            'oNew.sUID = sGlobalKey        'do this in the get_uid itself

            oMyNElements.Add(sKey, oNew)        'keep to the original - in the local collection this is the reference
            'oNew.oBaseClass.oRootDictionary.Add(sGlobalKey, oNew)
            'oNew.oBaseClass.oRootKeyDictionary.Add(oNew.sGivenKey, oNew)
            'Try
            '    'this is a little bit weird. If the root dictionary is self then it cannot add into itself (i think). look at the code for orootdictionary
            '    '  so we check first but i don't know if this will add a heavy burden when the dictionary is big
            '    If oNew.oBaseClass.oRootDictionary.ContainsKey(sGlobalKey) = False Then
            '        oNew.oBaseClass.oRootDictionary.Add(sGlobalKey, oNew)
            '    End If

            'Catch exKey As Exception
            '    Debug.Print("XIGenerics.CXI.AddElement." & "Duplicate Key for Element: " & sName)
            'End Try

            'Try
            '    If oNew.oBaseClass.oRootKeyDictionary.ContainsKey(oNew.sGivenKey) = False Then
            '        oNew.oBaseClass.oRootKeyDictionary.Add(oNew.sGivenKey, oNew)
            '    End If

            'Catch exKey2 As Exception
            '    Debug.Print("XIGenerics.CXI.AddElement." & "Duplicate Given Key for Element: " & sName)
            'End Try

            oNew.sKey = sKey

            If oCopyFrom Is Nothing Then

            Else
                'Debug.Print("Copying")
            End If

            Return oNew
        Catch ex As Exception
            Debug.Print("XIGenerics.CXI.AddElement." & "ERROR IN ADDElement: " & Err.Description)
        End Try
    End Function


    Public Function NNodeElement(ByVal sNodeName As String, Optional ByVal sKey As String = "") As CNodeItem
        'essentially we build the key up in here to make it easier
        Dim sConcatKey As String = ""
        Dim sNewKey As String = ""
        'Dim oCxi As Cxi

        Try


            If sKey = "" Then
                sConcatKey = sNodeName
            ElseIf sKey.ToLower = sNodeName.ToLower Then   'same as name - usually you just want to call it this, as opposed to for example a GUID where you are specifically giving it a key
                sConcatKey = sNodeName
            Else        '2012.07.23 - this is a bit strange but is historical, meant to ensure that there are unique keys and this function doesnt fail. However now not really used, but may break old systems
                sConcatKey = sNodeName & "." & sKey
            End If

            'WHY NOT?? BECAUSE WHEN WE ADD THE NODE WE WANT THE NAME TO BE CASED, WHEREAS THE KEY IS ALWAYS LOWER. DO NOT CHANGE THIS AS IT WILL MESS UP! (WARNING YOU!)
            'sConcatKey = sConcatKey.ToLower

            If sConcatKey <> "" Then
                Try
                    If oMyNElements.ContainsKey(sConcatKey.ToLower) Then
                        NNodeElement = oMyNElements(sConcatKey.ToLower)
                    Else
                        sNewKey = AddElement(sConcatKey).sKey
                        Try
                            'DS: I know in some app this will cause problems, but i need to reference the object by given key, not some random one. The sKey (sUID) is the 
                            '  one to use with the base collection (ie it is totally unique in there)
                            NNodeElement = oMyNElements(sNewKey)     'sNewKey)
                        Catch ex2 As Exception
                            NNodeElement = Nothing  'serious problem
                        End Try
                    End If
                Catch ex As Exception
                    Debug.Print("XIGenerics.CXI.NNodeElement." & "Error")
                    NNodeElement = Nothing
                End Try
            Else
                NNodeElement = Nothing  'serious problem
            End If

        Catch ex As Exception
            NNodeElement = Nothing  'serious problem
        End Try
    End Function



    Public Function ElementValue(ByVal sKey As String, Optional ByVal sDefaultValue As String = "") As CNodeItem

        Dim sFR As String = ""
        Dim oXIResult As New CNodeItem

        Try
            'NOTE - This function does not add this element if it doesn't exist, unlike NNodeElement
            If oMyNElements.ContainsKey(sKey.ToLower) Then
                oXIResult = oMyNElements(sKey.ToLower)
            Else
                oXIResult.sValue = sDefaultValue
            End If

            'oXIResult.NNodeElement("xiResult").sValue = 0
        Catch ex As Exception
            sFR = "Error: " & ex.Message & " - Stack: " & ex.StackTrace
            oXIResult.NNodeElement("xiResult").sValue = 30
            oXIResult.NNodeElement("sResult").sValue = sFR
        End Try


        Return oXIResult

    End Function

    'Public Function Get_Element(ByVal sKey As String) As String

    '    Try
    '        Return NElements(sKey.ToLower).sValue     'needs to be lower case as key
    '    Catch ex As Exception
    '        Return ""
    '    End Try

    'End Function
End Class
