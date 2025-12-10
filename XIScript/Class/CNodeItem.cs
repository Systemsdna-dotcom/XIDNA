using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using XISystem;
using System.Xml;
using System.Diagnostics;
using XIScriptEnum;

namespace XIScript
{
    public class CNodeItem
    {

        private OrderedDictionary oNodeItems = new OrderedDictionary();

        // ( (Of String, CNodeItem)
        private XIScriptEnum.xiDirty iMyDirty;

        private string sMyName = "";

        public string sName
        {
            get
            {
                return sMyName;
            }
            set
            {
                sMyName = value.Trim();
            }
        }

        private string sMyValue = "";

        public string sValue
        {
            get
            {
                return sMyValue;
            }
            set
            {
                if ((value != sMyValue))
                {
                    if ((iMyDirty == xiDirty.xiClean))
                    {
                        iMyDirty = xiDirty.xiDirty;
                    }

                    sMyValue = value.Trim();
                }

            }
        }

        public CNodeItem Get_NodeByNumber(long iIndex)
        {
            CNodeItem oReturnItem = null/* TODO Change to default(_) if this is not a reference type */;
            string sKey = "";

            try
            {
                iIndex = iIndex - 1;  // 1 based, not zero based. If you think about it 'get my zero'th child' is a bit weird
                sKey = oNodeItems[iIndex].ToString();
                oReturnItem = (CNodeItem)oNodeItems[sKey];
            }
            catch (Exception ex)
            {
            }

            return oReturnItem;
        }

        public OrderedDictionary NNodeItems
        {
            get
            {
                return oNodeItems;
            }
            set
            {
                // (Of String, CNodeItem))
                oNodeItems = value;
            }
        }

        private CNodeItem oMyParentNode;
        public CNodeItem oParentNode
        {
            get
            {
                return oMyParentNode;
            }
            set
            {
                oMyParentNode = value;
            }
        }

        private long iMyChildIndex;

        public long iChildIndex
        {
            get
            {
                return iMyChildIndex;
            }
            set
            {
                iMyChildIndex = value;
            }
        }

        private long iMyChildCount;

        public long iChildCount
        {
            get
            {
                try
                {
                    iMyChildCount = oNodeItems.Count;
                }
                catch (Exception ex)
                {
                }

                return iMyChildCount;
            }
            set
            {
                iMyChildCount = value;
            }
        }

        public CResult AddNodeObject(CNodeItem oNodeToAdd)
        {
            CResult oResult = new CResult();
            try
            {
                oNodeToAdd.iChildIndex = NNodeItems.Count;
                oNodeItems.Add(oNodeToAdd.sKey, oNodeToAdd);
                oNodeToAdd.oParentNode = this;
                oResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oResult.sMessage = (ex.Message + (" - Stack: " + ex.StackTrace));
            }

            return oResult;
        }

        public CNodeItem NNode(string sNodeName, string sKey = "")
        {
            // essentially we build the key up in here to make it easier
            // Warning!!! Optional parameters not supported
            string sConcatKey = "";
            string sNewKey = "";
            CNodeItem oNewM = null;
            try
            {
                if ((sKey == ""))
                {
                    sConcatKey = sNodeName;
                }
                else
                {
                    sConcatKey = sNodeName;
                }

                if ((sConcatKey != ""))
                {
                    try
                    {
                        if (oNodeItems.Contains(sConcatKey.ToLower()))
                        {
                            NNode(oNodeItems[sConcatKey.ToLower()].ToString());
                        }
                        else
                        {
                            oNewM = AddNode(sConcatKey.ToLower());
                            sNewKey = oNewM.sKey;
                            // sNewKey = AddNode(sConcatKey.ToLower()).sKey
                            try
                            {
                                NNode(oNodeItems[sNewKey].ToString());
                                NNode(sNodeName);
                            }
                            catch (Exception ex2)
                            {
                                //NNode = null;
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.Print("XIGenerics.CXI.NNodeMeta.Error");
                        //NNode = null;
                    }

                }
                else
                {
                    //NNode = null;
                }

            }
            catch (Exception ex)
            {
                //NNode = null;
            }
            return oNewM;
        }

        CNodeItem AddNode(string sNodeName)
        {
            CNodeItem oNew = new CNodeItem();
            string sKey;
            string sFirstChar = "";
            string sName;
            bool bDataTypeAssigned = false;
            string sGlobalKey = "";
            try
            {
                sName = sNodeName;
                sKey = sNodeName.ToLower();
                if ((oNodeItems.Contains(sKey)
                            || (sKey == "")))
                {
                    // TO DO - error or can we use a generated key??
                    // sKey = Get_UID()
                }

                oNew = new CNodeItem();
                // Try
                //     sFirstChar = sName.Substring(0, 1)
                // Catch exName As Exception
                // End Try
                // If bHungarian Then
                //     Select Case sFirstChar
                //         Case "s"
                //             oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiString
                //             bDataTypeAssigned = True
                //         Case "i"
                //             oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiLong
                //         Case "f"
                //             oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiFloat
                //         Case "o"
                //             oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiObject
                //         Case "d"
                //             oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiDate
                //         Case "r"
                //             oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiCurrency
                //     End Select
                // End If
                // If oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiString And bDataTypeAssigned = False Then  'if this is dan notation then chop of first char of name
                // Else
                sName = sName.Substring(1, (sName.Length - 1));
                // End If
                //     oNew.oParent = Me
                // oNew.oBaseClass = Me.oBaseClass
                // oNew.sClass = sNodeName
                // oNew.iLevel = Me.iLevel + 1
                // oNew.sUID = Get_UID()
                // oNew.sName = sName
                // If sSpecifiedGivenKey = "" Then
                //     oNew.sGivenKey = NextKey()
                // Else
                //     oNew.sGivenKey = sSpecifiedGivenKey
                // End If
                // 'oNew.sGivenKey = NextKey()
                // oNew.xiBaseDataType = tBaseType.xiMeta
                // iMyMetaCount = iMyMetaCount + 1
                // If oCopyFrom Is Nothing Then
                //     'sKey = oNew.oBaseClass.Get_Key(sNodeName, Me, oXMLNode)       ', sNodeId
                //     sGlobalKey = "M." & oNew.sUID
                // Else
                //     'be careful then - why keep keys the same??
                //     sGlobalKey = oCopyFrom.sKey
                // End If
                // oNew.sUID = sGlobalKey        'do this in the get_uid itself
                oNew.sKey = sKey;
                oNodeItems.Add(sKey, oNew);
                // keep to the original - in the local collection this is the reference
                // Try
                //     oNew.oBaseClass.oRootDictionary.Add(sGlobalKey, oNew)
                // Catch exKey As Exception
                //     Debug.Print("XIGenerics.CXI.AddMeta." & "Duplicate Key for Meta: " & sName)
                // End Try
                // Try
                //     oNew.oBaseClass.oRootKeyDictionary.Add(oNew.sGivenKey, oNew)
                // Catch exKey2 As Exception
                //     Debug.Print("XIGenerics.CXI.AddMeta." & "Duplicate Given Key for Meta: " & sName)
                // End Try
                // oNew.sKey = sKey
                // If oCopyFrom Is Nothing Then
                // Else
                //     'Debug.Print("Copying")
                // End If
                return oNew;
            }
            catch (Exception ex)
            {
                // Debug.Print("XIGenerics.CXI.AddMeta." & "ERROR IN ADDMETA: " & Err.Description)
                return oNew;
                // really needs to be xiResult
            }

        }

        private string sMyKey;

        public string sKey
        {
            get
            {
                return sMyKey;
            }
            set
            {
                sMyKey = value;
            }
        }

        private Dictionary<string, CNodeItem> oMyNElements = new Dictionary<string, CNodeItem>();

        public Dictionary<string, CNodeItem> NElements
        {
            get
            {
                return oMyNElements;
            }
            set
            {
                oMyNElements = value;
            }
        }

        CNodeItem AddElement(string sElementName, string sNodeId = "", CNodeItem oCopyFrom = null, System.Xml.XmlNode oXMLNode = null, bool bHungarian = false, string sSpecifiedGivenKey = "")
        {
            CNodeItem oNew = new CNodeItem();
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            // Object  'New Cxi
            string sKey;
            string sFirstChar = "";
            string sName;
            bool bDataTypeAssigned = false;
            string sGlobalKey = "";
            try
            {
                sName = sElementName;
                sKey = sElementName.ToLower();
                if ((oMyNElements.ContainsKey(sKey)
                            || (sKey == "")))
                {
                    // sKey = Get_UID()
                }

                oNew = new CNodeItem();
                try
                {
                    sFirstChar = sName.Substring(0, 1);
                }
                catch (Exception exName)
                {
                }

                // If bHungarian Then
                //     Select Case sFirstChar
                //         Case "s"
                //             oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiString
                //             bDataTypeAssigned = True
                //         Case "i"
                //             oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiLong
                //         Case "f"
                //             oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiFloat
                //         Case "o"
                //             oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiObject
                //         Case "d"
                //             oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiDate
                //         Case "r"
                //             oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiCurrency
                //     End Select
                // End If
                // If oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiString And bDataTypeAssigned = False Then  'if this is dan notation then chop of first char of name
                // Else
                //     sName = sName.Substring(1, sName.Length - 1)
                // End If
                // oNew.oParent = Me
                // oNew.oBaseClass = Me.oBaseClass
                // oNew.sClass = sElementName
                // oNew.iLevel = Me.iLevel + 1
                // oNew.sUID = Get_UID()
                // oNew.sName = sName
                // If sSpecifiedGivenKey = "" Then
                //     oNew.sGivenKey = NextKey()
                // Else
                //     oNew.sGivenKey = sSpecifiedGivenKey
                // End If
                // oNew.sGivenKey = NextKey()
                // oNew.xiBaseType = tBaseType.xiElement
                // iMyElementCount = iMyElementCount + 1
                // If oCopyFrom Is Nothing Then
                //     'sKey = oNew.oBaseClass.Get_Key(sElementName, Me, oXMLNode)       ', sNodeId
                //     sGlobalKey = "V." & oNew.sUID
                // Else
                //     'be careful then - why keep keys the same??
                //     sGlobalKey = oCopyFrom.sKey
                // End If
                // oNew.sUID = sGlobalKey        'do this in the get_uid itself
                oMyNElements.Add(sKey, oNew);
                // keep to the original - in the local collection this is the reference
                // oNew.oBaseClass.oRootDictionary.Add(sGlobalKey, oNew)
                // oNew.oBaseClass.oRootKeyDictionary.Add(oNew.sGivenKey, oNew)
                // Try
                //     'this is a little bit weird. If the root dictionary is self then it cannot add into itself (i think). look at the code for orootdictionary
                //     '  so we check first but i don't know if this will add a heavy burden when the dictionary is big
                //     If oNew.oBaseClass.oRootDictionary.ContainsKey(sGlobalKey) = False Then
                //         oNew.oBaseClass.oRootDictionary.Add(sGlobalKey, oNew)
                //     End If
                // Catch exKey As Exception
                //     Debug.Print("XIGenerics.CXI.AddElement." & "Duplicate Key for Element: " & sName)
                // End Try
                // Try
                //     If oNew.oBaseClass.oRootKeyDictionary.ContainsKey(oNew.sGivenKey) = False Then
                //         oNew.oBaseClass.oRootKeyDictionary.Add(oNew.sGivenKey, oNew)
                //     End If
                // Catch exKey2 As Exception
                //     Debug.Print("XIGenerics.CXI.AddElement." & "Duplicate Given Key for Element: " & sName)
                // End Try
                oNew.sKey = sKey;
                if ((oCopyFrom == null))
                {

                }
                else
                {
                    // Debug.Print("Copying")
                }

                
            }
            catch (Exception ex)
            {
                Debug.Print(("XIGenerics.CXI.AddElement." + ("ERROR IN ADDElement: " + ex.Message)));
            }
            return oNew;
        }

        public CNodeItem NNodeElement(string sNodeName, string sKey = "")
        {
            CNodeItem oNode = new CNodeItem();
            // essentially we build the key up in here to make it easier
            // Warning!!! Optional parameters not supported
            string sConcatKey = "";
            string sNewKey = "";
            try
            {
                if ((sKey == ""))
                {
                    sConcatKey = sNodeName;
                }
                else if ((sKey.ToLower() == sNodeName.ToLower()))
                {
                    // same as name - usually you just want to call it this, as opposed to for example a GUID where you are specifically giving it a key
                    sConcatKey = sNodeName;
                }
                else
                {
                    // 2012.07.23 - this is a bit strange but is historical, meant to ensure that there are unique keys and this function doesnt fail. However now not really used, but may break old systems
                    sConcatKey = (sNodeName + ("." + sKey));
                }

                // WHY NOT?? BECAUSE WHEN WE ADD THE NODE WE WANT THE NAME TO BE CASED, WHEREAS THE KEY IS ALWAYS LOWER. DO NOT CHANGE THIS AS IT WILL MESS UP! (WARNING YOU!)
                // sConcatKey = sConcatKey.ToLower
                if ((sConcatKey != ""))
                {
                    try
                    {
                        if (oMyNElements.ContainsKey(sConcatKey.ToLower()))
                        {
                            NNodeElement(oMyNElements[sConcatKey.ToLower()].ToString());
                        }
                        else
                        {
                            sNewKey = AddElement(sConcatKey).sKey;
                            try
                            {
                                // DS: I know in some app this will cause problems, but i need to reference the object by given key, not some random one. The sKey (sUID) is the 
                                //   one to use with the base collection (ie it is totally unique in there)
                                NNodeElement(oMyNElements[sNewKey].ToString());
                                // sNewKey)
                            }
                            catch (Exception ex2)
                            {
                                //NNodeElement = null;
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.Print(("XIGenerics.CXI.NNodeElement." + "Error"));
                        //NNodeElement = null;
                    }

                }
                else
                {
                    //NNodeElement = null;
                }

            }
            catch (Exception ex)
            {
                //NNodeElement = null;
            }
            return oNode;
        }

        public CNodeItem ElementValue(string sKey, string sDefaultValue = "")
        {
            string sFR = "";
            // Warning!!! Optional parameters not supported
            CNodeItem oXIResult = new CNodeItem();
            try
            {
                // NOTE - This function does not add this element if it doesn't exist, unlike NNodeElement
                if (oMyNElements.ContainsKey(sKey.ToLower()))
                {
                    oXIResult = oMyNElements[sKey.ToLower()];
                }
                else
                {
                    oXIResult.sValue = sDefaultValue;
                }

                // oXIResult.NNodeElement("xiResult").sValue = 0
            }
            catch (Exception ex)
            {
                sFR = ("Error: "
                            + (ex.Message + (" - Stack: " + ex.StackTrace)));
                oXIResult.NNodeElement("xiResult").sValue = "30";
                oXIResult.NNodeElement("sResult").sValue = sFR;
            }

            return oXIResult;
        }
    }
}