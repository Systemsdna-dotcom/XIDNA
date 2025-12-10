using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using XICore;
using XISystem;

namespace ZeeInsurance
{
    public class WalletFileHandler
    {
        CDataTable oDT = new CDataTable();
        bool bISFileColMismatch = false;
        bool bISTransactionColMismatch = false;
        bool bISFileDiscard = false;
        int iTransactionHeadingsCount = 0;
        int iTransactionRowsCount = 0;
        int iFileHeadingsCount = 0;
        int iFileRowsCount = 0;
        int iSuucessFulTransactions = 0;
        int iFileID = 0;
        int iOrgID = 0;
        string FKiUserID = string.Empty;
        Dictionary<string, string> FileDBMappings = new Dictionary<string, string>();
        Dictionary<string, string> TransactionDBMappings = new Dictionary<string, string>();
        XIInfraCache oCache = new XIInfraCache();
        XIDefinitionBase oXID = new XIDefinitionBase();
        public CResult ProcessFile(XIIBO oBOI)
        {
            CResult oCR = new CResult();
            try
            {
                //Save File Details
                var FileBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, oBOI.BOD.BOID.ToString());
                if (oBOI.Attributes.ContainsKey(FileBOD.sPrimaryKey.ToLower()))
                {
                    var sFileID = oBOI.Attributes[FileBOD.sPrimaryKey.ToLower()].sValue;
                    int.TryParse(sFileID, out iFileID);
                }
                if (iFileID > 0)
                {
                    //Rollback
                    //var oAttrD = new XIDAttribute();
                    var oAttrD = FileBOD.Attributes.Values.Where(m => m.bIsFileProcess == true).FirstOrDefault();
                    if (oAttrD.bIsFileProcess)
                    //if (true)
                    {
                        var sAttrName = oAttrD.Name;
                        var sAttrVal = oBOI.Attributes[sAttrName.ToLower()].sValue;
                        XIIXI oXII = new XIIXI();
                        XID1Click oD1Click = new XID1Click();
                        List<CNV> oDocWhrParams = new List<CNV>();
                        oDocWhrParams.Add(new CNV { sName = "ID", sValue = sAttrVal });
                        var oDOCI = oXII.BOI("Documents_T", null, null, oDocWhrParams);
                        string sFilePath = string.Empty;
                        if (oDOCI.Attributes.ContainsKey("sFullPath"))
                        {
                            sFilePath = oDOCI.Attributes["sFullPath"].sValue;
                        }
                        if (!string.IsNullOrEmpty(sFilePath))
                        {
                            //Rollback
                            if (oAttrD.iFileProcessType == 1)
                            //if (true)
                            {
                                var sVirtualDir = System.Web.Hosting.HostingEnvironment.MapPath("~") + "\\" + System.Configuration.ConfigurationManager.AppSettings["VirtualDirectoryPath"];
                                //var sVirtualPath = sVirtualDir + "\\";
                                sFilePath = sVirtualDir + "\\" + sFilePath;

                                //Read data in CSV File
                                ReadCSV(sFilePath);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oCR.sMessage = "ERROR: [" + oCR.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCR.sCode = "WalletManager";
                oCR.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCR.LogToFile();
                oXID.SaveErrortoDB(oCR);
            }
            return oCR;
        }

        public CResult ReadCSV(string sFilePath)
        {
            CResult oCR = new CResult();
            try
            {
                var sData = new List<string>();
                using (StreamReader sr = new StreamReader(sFilePath))
                {
                    string sCurrentLine;
                    // currentLine will be null when the StreamReader reaches the end of file
                    while ((sCurrentLine = sr.ReadLine()) != null)
                    {
                        string sLineData = sCurrentLine;
                        // Search, case insensitive, if the currentLine contains the searched keyword
                        //if (currentLine.IndexOf("CRLF", StringComparison.CurrentCultureIgnoreCase) >= 0)

                        //remove CRLF
                        Regex rgxNewLine = new Regex("CRLF");
                        sLineData = rgxNewLine.Replace(sLineData, "");

                        //replace double quotes
                        Regex rgxDouble = new Regex("\"\"");
                        sLineData = rgxDouble.Replace(sLineData, " ");

                        //replace space
                        Regex rgxSpace1 = new Regex("\" ");
                        sLineData = rgxSpace1.Replace(sLineData, "\"");

                        //replace space
                        Regex rgxSpace2 = new Regex(" \"");
                        sLineData = rgxSpace2.Replace(sLineData, "\"");

                        //replace single quotes
                        Regex rgxSingle = new Regex("\"");
                        sLineData = rgxSingle.Replace(sLineData, "");

                        Regex rgxSpace3 = new Regex(" ,");
                        sLineData = rgxSingle.Replace(sLineData, ",");

                        //add to the list.
                        sData.Add(sLineData);
                    }
                }

                //Read headings and rows from CSV Data
                BuildDataObjectFromCSV(sData);

                //Check BO mappings for CSV Format and verify data is savable or not
                GetCSVMappings();
                if (bISFileDiscard)
                {

                }
            }
            catch (Exception ex)
            {
                oCR.sMessage = "ERROR: [" + oCR.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCR.sCode = "WalletManager";
                oCR.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCR.LogToFile();
                oXID.SaveErrortoDB(oCR);
            }
            return oCR;
        }

        //Read CSV Row by Row
        private void BuildDataObjectFromCSV(List<string> CSVData)
        {
            CResult oCR = new CResult();
            try
            {
                int i = 0;
                List<string> FileHeadings = new List<string>();
                List<List<string>> FileRows = new List<List<string>>();
                List<string> TransactionHeadings = new List<string>();
                List<List<string>> TransactionRows = new List<List<string>>();
                foreach (var row in CSVData)
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        if (i == 0)
                        {
                            FileHeadings = row.Split(',').ToList();
                        }
                        else if (i == 1)
                        {
                            var CurrentRow = row.Split(',').ToList();
                            FileRows.Add(CurrentRow);
                        }
                        else if (i == 2)
                        {
                            TransactionHeadings = row.Split(',').ToList();
                        }
                        else if (i > 2)
                        {
                            var CurrentRow = row.Split(',').ToList();
                            TransactionRows.Add(CurrentRow);
                        }
                        i++;
                    }
                }
                iFileHeadingsCount = FileHeadings.Count();
                iFileRowsCount = FileRows.Count();
                iTransactionHeadingsCount = TransactionHeadings.Count();
                iTransactionRowsCount = TransactionRows.Count();
                oDT.FileHeadings = FileHeadings;
                oDT.FileRows = FileRows;
                oDT.TransactionHeadings = TransactionHeadings;
                oDT.TransactionRows = TransactionRows;
            }
            catch (Exception ex)
            {
                oCR.sMessage = "ERROR: [" + oCR.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCR.sCode = "WalletManager";
                oCR.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCR.LogToFile();
                oXID.SaveErrortoDB(oCR);
            }
        }

        private void GetCSVMappings()
        {
            CResult oCR = new CResult();
            try
            {
                //Get Mapped Fields for CSV Format
                XIIBO oFTBOI = new XIIBO();
                XIIXI oXII = new XIIXI();
                XID1Click oD1Click = new XID1Click();
                List<CNV> oFTWhrParams = new List<CNV>();
                oFTWhrParams.Add(new CNV { sName = "sCode", sValue = "csv" });
                oFTBOI = oXII.BOI("WM FileType", null, null, oFTWhrParams);
                int iFileTypeID = 0;
                int iBOID = 0;
                if (oFTBOI.Attributes.ContainsKey("ID"))
                {
                    int.TryParse(oFTBOI.Attributes["ID"].sValue, out iFileTypeID);
                    if (oFTBOI.Attributes.ContainsKey("FKiBOID"))
                    {
                        int.TryParse(oFTBOI.Attributes["FKiBOID"].sValue, out iBOID);
                    }
                    if (iFileTypeID > 0)
                    {
                        var FileMapping = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, 3571.ToString());
                        XIDStructure oStructure = new XIDStructure();
                        List<CNV> nParams = new List<CNV>();
                        CNV oNV = new CNV();
                        oNV.sName = "{XIP|FileTypeID}";
                        oNV.sValue = iFileTypeID.ToString();
                        nParams.Add(oNV);
                        var Query = oStructure.ReplaceExpressionWithCacheValue(FileMapping.Query, nParams);
                        oD1Click.Query = Query;
                        oD1Click.Name = "WM FileMapping";
                        var oMappings = oD1Click.OneClick_Run(false);
                        //if (oMappings.Count != iTransactionHeadingsCount)
                        //{
                        //    bISColMismatch = true;
                        //}
                        //if (bISColMismatch)
                        //{
                        //    bISFileDiscard = true;
                        //}
                        //else
                        //{
                        //Validate CSV Schema
                        ValidateCSVSchema(oMappings);
                        if (bISFileColMismatch || bISTransactionColMismatch)
                        {
                            bISFileDiscard = true;
                            UpdateFileStatus(40);
                        }
                        else
                        {
                            //Validate CSV Data
                            ValidateAndSaveCSVData();
                        }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                oCR.sMessage = "ERROR: [" + oCR.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCR.sCode = "WalletManager";
                oCR.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCR.LogToFile();
                oXID.SaveErrortoDB(oCR);
            }
        }

        //Check for all mapping fields has data in CSV file
        private void ValidateCSVSchema(Dictionary<string, XIIBO> oMappings)
        {
            CResult oCR = new CResult();
            try
            {
                oDT.FileDBColumns = new List<string>();
                oDT.TransactionDBColumns = new List<string>();
                var FileMaps = oMappings.Values.Where(m => m.Attributes["sCode"].sValue.ToLower() == "head1").ToList();
                for (int k = 0; k < FileMaps.Count(); k++)
                {
                    bool bIsColExists = false;
                    if (FileMaps[k].Attributes.ContainsKey("sOriginField"))
                    {
                        var FileField = FileMaps[k].Attributes["sOriginField"].sValue;
                        if (!string.IsNullOrEmpty(FileField))
                        {
                            //Check mapping filed exists in file or not
                            var DTCol = oDT.FileHeadings.Where(m => m == FileField).FirstOrDefault();
                            var DBCol = FileMaps[k].Attributes["smappedfield"].sValue;
                            oDT.FileDBColumns.Add(DBCol);
                            FileDBMappings.Add(FileField, DBCol);
                            if (DTCol != null)
                            {
                                bIsColExists = true;
                            }
                        }
                    }
                    if (!bIsColExists)
                    {
                        bISFileColMismatch = true;
                        break;
                    }
                }
                var TransactionMaps = oMappings.Values.Where(m => m.Attributes["sCode"].sValue.ToLower() == "head2").ToList();
                for (int k = 0; k < TransactionMaps.Count(); k++)
                {
                    bool bIsColExists = false;
                    if (TransactionMaps[k].Attributes.ContainsKey("sOriginField"))
                    {
                        var FileField = TransactionMaps[k].Attributes["sOriginField"].sValue;
                        if (!string.IsNullOrEmpty(FileField))
                        {
                            //Check mapping filed exists in file or not
                            var DBCol = TransactionMaps[k].Attributes["smappedfield"].sValue;
                            var DTCol = oDT.TransactionHeadings.Where(m => m == FileField).FirstOrDefault();
                            oDT.TransactionDBColumns.Add(DBCol);
                            TransactionDBMappings.Add(FileField, DBCol);
                            if (DTCol != null)
                            {
                                bIsColExists = true;
                            }
                        }
                    }
                    if (!bIsColExists)
                    {
                        bISTransactionColMismatch = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                oCR.sMessage = "ERROR: [" + oCR.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCR.sCode = "WalletManager";
                oCR.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCR.LogToFile();
                oXID.SaveErrortoDB(oCR);
            }
        }

        private void ValidateAndSaveCSVData()
        {
            CResult oCR = new CResult();
            try
            {
                var sUserName = HttpContext.Current.Session["sUserName"].ToString();
                int iRowCount = 0;
                int iSuccessCount = 0;
                if (iFileID > 0)
                {
                    var FileData = oDT.FileRows.FirstOrDefault();
                    XIIXI oXI = new XIIXI();
                    var FileBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "wm file", null);
                    var FileBOI = oXI.BOI("wm file", iFileID.ToString());
                    int k = 0;
                    foreach (var cell in FileData)
                    {
                        if (!string.IsNullOrEmpty(cell))
                        {
                            var Column = oDT.FileHeadings[k];
                            var DBCol = FileDBMappings[Column];
                            if (FileBOI.Attributes.ContainsKey(DBCol))
                            {
                                FileBOI.Attributes[DBCol].sValue = cell;
                                FileBOI.Attributes[DBCol].bDirty = true;
                            }
                            else
                            {
                                FileBOI.Attributes.Add(DBCol, new XIIAttribute() { sName = DBCol, sValue = cell, bDirty = true });
                            }
                            k++;
                        }
                    }

                    if (FileBOI.Attributes.ContainsKey("sFileUploadedBy"))
                    {
                        FileBOI.Attributes["sFileUploadedBy"].sValue = sUserName;
                        FileBOI.Attributes["sFileUploadedBy"].bDirty = true;
                    }
                    else
                    {
                        FileBOI.Attributes.Add("sFileUploadedBy", new XIIAttribute() { sName = "sFileUploadedBy", sValue = sUserName, bDirty = true });
                    }

                    FileBOI.BOD = FileBOD;
                    oCR = FileBOI.Save(FileBOI);
                    if (oCR.bOK && oCR.oResult != null)
                    {

                    }
                }

                foreach (var row in oDT.TransactionRows)
                {
                    var TransactionBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "wm transaction", null);
                    //Build BOI object for saving into Database
                    XIIBO oBOI = new XIIBO();
                    int i = 0;
                    foreach (var cell in row)
                    {
                        var Column = oDT.TransactionHeadings[i];
                        var DBCol = TransactionDBMappings[Column];
                        oBOI.Attributes.Add(DBCol, new XIIAttribute() { sName = DBCol, sValue = cell, bDirty = true });
                        i++;
                    }
                    oBOI.Attributes.Add("fkifileid", new XIIAttribute() { sName = "fkifileid", sValue = iFileID.ToString(), bDirty = true });
                    if (oBOI.Attributes.ContainsKey("iStatus"))
                    {
                        oBOI.Attributes["iStatus"].sValue = "10";
                        oBOI.Attributes["iStatus"].bDirty = true;
                    }
                    else
                    {
                        oBOI.Attributes.Add("iStatus", new XIIAttribute() { sName = "iStatus", sValue = "10", bDirty = true });
                    }
                    if (oBOI.Attributes.ContainsKey("dtTransaction"))
                    {
                        oBOI.Attributes["dtTransaction"].sValue = DateTime.Now.ToString();
                        oBOI.Attributes["dtTransaction"].bDirty = true;
                    }
                    else
                    {
                        oBOI.Attributes.Add("dtTransaction", new XIIAttribute() { sName = "dtTransaction", sValue = DateTime.Now.ToString(), bDirty = true });
                    }
                    if (oBOI.Attributes.ContainsKey("dtExpiry"))
                    {
                        oBOI.Attributes["dtExpiry"].sValue = DateTime.Now.ToString();
                        oBOI.Attributes["dtExpiry"].bDirty = true;
                    }
                    else
                    {
                        oBOI.Attributes.Add("dtExpiry", new XIIAttribute() { sName = "dtExpiry", sValue = DateTime.Now.ToString(), bDirty = true });
                    }
                    if (oBOI.Attributes.ContainsKey("XIGUID"))
                    {
                        oBOI.Attributes["XIGUID"].sValue = Guid.NewGuid().ToString();
                        oBOI.Attributes["XIGUID"].bDirty = true;
                    }
                    else
                    {
                        oBOI.Attributes.Add("XIGUID", new XIIAttribute() { sName = "XIGUID", sValue = Guid.NewGuid().ToString(), bDirty = true });
                    }
                    oBOI.BOD = TransactionBOD;
                    //Save will take care of validation failures
                    oCR = oBOI.Save(oBOI);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        iSuccessCount++;
                    }
                    iRowCount++;
                }
                iSuucessFulTransactions = iSuccessCount;
                if (iSuucessFulTransactions == iTransactionRowsCount)
                {
                    UpdateFileStatus(10, iSuucessFulTransactions);
                }
                else
                {
                    UpdateFileStatus(30);
                }
            }
            catch (Exception ex)
            {
                oCR.sMessage = "ERROR: [" + oCR.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCR.sCode = "WalletManager";
                oCR.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCR.LogToFile();
                oXID.SaveErrortoDB(oCR);
            }
        }

        private void UpdateFileStatus(int iStatus = 0, int iNoOfPayments = 0)
        {
            CResult oCR = new CResult();
            try
            {
                XIIXI oXI = new XIIXI();
                var FileBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "wm file", null);
                var FileBOI = oXI.BOI("wm file", iFileID.ToString());
                if (FileBOI.Attributes.ContainsKey("iStatus"))
                {
                    FileBOI.Attributes["iStatus"].sValue = iStatus.ToString();
                    FileBOI.Attributes["iStatus"].bDirty = true;
                }
                else
                {
                    FileBOI.Attributes.Add("iStatus", new XIIAttribute() { sName = "iStatus", sValue = iStatus.ToString(), bDirty = true });
                }
                if (FileBOI.Attributes.ContainsKey("iNoOfPayments"))
                {
                    FileBOI.Attributes["iNoOfPayments"].sValue = iNoOfPayments.ToString();
                    FileBOI.Attributes["iNoOfPayments"].bDirty = true;
                }
                else
                {
                    FileBOI.Attributes.Add("iNoOfPayments", new XIIAttribute() { sName = "iNoOfPayments", sValue = iNoOfPayments.ToString(), bDirty = true });
                }
                FileBOI.BOD = FileBOD;
                oCR = FileBOI.Save(FileBOI);
                if (oCR.bOK && oCR.oResult != null)
                {
                    List<CNV> oParams = new List<CNV>();
                    oParams.Add(new CNV() { sName = "iFileID", sValue = iFileID.ToString() });
                    Thread threadObj = new Thread(new ThreadStart(() => { ProcessTransactions(oParams); }));
                    threadObj.Start();
                }
            }
            catch (Exception ex)
            {
                oCR.sMessage = "ERROR: [" + oCR.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCR.sCode = "WalletManager";
                oCR.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCR.LogToFile();
                oXID.SaveErrortoDB(oCR);
            }
        }

        public void ProcessTransactions(List<CNV> oParams)
        {
            CResult oCR = new CResult();
            const string ROUTINE_NAME = "[Process Transactions]";
            try
            {
                long iFileID = 0;
                var sFileID = oParams.Where(m => m.sName.ToLower() == "ifileid").Select(m => m.sValue).FirstOrDefault();
                long.TryParse(sFileID, out iFileID);
                if (iFileID > 0)
                {
                    //TO DO: Load All Transactions
                    CResult oCResult = new CResult();
                    XID1Click oFiletransactions = new XID1Click();
                    oFiletransactions = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "File Transactions", null);
                    oFiletransactions = (XID1Click)oFiletransactions.Clone(oFiletransactions);
                    XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, oFiletransactions.BOID.ToString());
                    XIDXI oXID = new XIDXI();
                    var DataSource = oXID.GetBODataSource(oBOD.iDataSourceXIGUID.ToString(),oBOD.FKiApplicationID);
                    List<CNV> nParms = new List<CNV>();
                    CNV oNVParams = new CNV();
                    oNVParams.sName = "{XIP|ifileid}";
                    oNVParams.sValue = sFileID.ToString();
                    nParms.Add(oNVParams);
                    var oFileTransactionList = new Dictionary<string, XIIBO>();
                    oFiletransactions.ReplaceFKExpressions(nParms);
                    oFiletransactions.sConnectionString = DataSource;
                    oFileTransactionList = oFiletransactions.OneClick_Execute();
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = oFileTransactionList;

                    if (!oCResult.bOK && oCResult.oResult == null)
                    {
                        oCResult.sMessage = ROUTINE_NAME + "Unable to DB2Collection File Transactions";
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                    }
                    //TO DO: Check user exists or not based on EmailID
                    string sEmail = "";
                    if (oFileTransactionList.Count() > 0)
                    {
                        foreach (var item in oFileTransactionList)
                        {
                            if (string.IsNullOrEmpty(item.Value.AttributeI("FKiUserID").sValue))
                            {
                                //Create User
                                sEmail = item.Value.AttributeI("sEmail").sValue;
                                oCR = CreateUser(item.Value);
                                item.Value.SetAttribute("FKiUserID", FKiUserID);
                                item.Value.BOD = oBOD;
                                var res = item.Value.Save(item.Value);
                                //TO DO: Send Mail to user for registration 
                                SendMail(sEmail);
                            }
                        }
                    }

                    //var oDocContent = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, null, "395");
                    //string sContent = oDocContent.FirstOrDefault().Content;
                    //XIInfraEmail oEmail = new XIInfraEmail();
                    //oEmail.sSubject = "Mail From Wallet Manager";
                    //oEmail.EmailID = sEmail;
                    //var oMailResult = oEmail.Sendmail(iOrgID, sContent, null, 0, null, 0);//send mail with attachments
                    //if (oMailResult.bOK && oMailResult.oResult != null)
                    //{
                    //    oCResult.oTraceStack.Add(new CNV { sName = "Mail send successfully", sValue = "Mail send successfully to email:" + sEmail + "" });
                    //    oXID.SaveErrortoDB(oCR);
                    //}
                }
            }
            catch (Exception ex)
            {
                oCR.sMessage = "ERROR: [" + oCR.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCR.sCode = "WalletManager";
                oCR.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCR.LogToFile();
                oXID.SaveErrortoDB(oCR);
            }
        }
        public CResult CreateUser(XIIBO oTransactiondetails)
        {
            CResult oCR = new CResult();
            try
            {
                string ConfigDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinDatabase"];
                string CoreDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreDataBase"];
                string ShareDatabase = System.Configuration.ConfigurationManager.AppSettings["SharedDataBase"];
                XIInfraUsers oUser = new XIInfraUsers();
                XIInfraEncryption oEncrypt = new XIInfraEncryption();
                //var EncryptedPwd = oEncrypt.EncryptData("Admin.123", true, 255.ToString());
                oUser.sUserName = oTransactiondetails.AttributeI("sEmail").sValue;
                oUser.CheckAccessCode = System.Configuration.ConfigurationManager.AppSettings["CheckAccessCode"];
                oUser.sAccessCode = System.Configuration.ConfigurationManager.AppSettings["AccessCode"];

                XIInfraRoles xifRole = new XIInfraRoles();
                xifRole.sRoleName = xiEnumSystem.EnumRoles.WMBeneficier.ToString().ToLower();
                var oRoleData = xifRole.Get_RoleDefinition(CoreDatabase);
                if (oRoleData.bOK)
                {
                    xifRole = (XIInfraRoles)oRoleData.oResult;
                }

                List<XIIBO> oUsers = new List<XIIBO>();
                List<XIWhereParams> oWParams = new List<XIWhereParams>();
                QueryEngine oQE = new QueryEngine();
                oWParams.Add(new XIWhereParams { sField = "sUserName", sOperator = "=", sValue = oTransactiondetails.AttributeI("sEmail").sValue });
                oWParams.Add(new XIWhereParams { sField = "FKiOrganisationID", sOperator = "=", sValue = xifRole.FKiOrganizationID.ToString() });
                oQE.AddBO("XIAPPUsers", null, oWParams);
                oCR = oQE.BuildQuery();
                if (oCR.bOK && oCR.oResult != null)
                {
                    var sSql = (string)oCR.oResult;
                    ExecutionEngine oEE = new ExecutionEngine();
                    oEE.XIDataSource = oQE.XIDataSource;
                    oEE.sSQL = sSql;
                    var oQResult = oEE.Execute();
                    if (oQResult.bOK && oQResult.oResult != null)
                    {
                        oUsers = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.ToList();
                    }
                }
                XIIBO oBOI = new XIIBO();
                XIDBO oBOD = new XIDBO();
                CResult oCResult = new CResult();
                if (oUsers.Count() == 0)
                {
                    //User
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIAPPUsers", null); //"667"
                    oBOI.BOD = oBOD;
                    oBOI.SetAttribute("sUserName", oTransactiondetails.AttributeI("sEmail").sValue);
                    oBOI.SetAttribute("sDatabaseName", ShareDatabase);
                    oBOI.SetAttribute("sCoreDatabaseName", CoreDatabase);
                    oBOI.SetAttribute("sPhoneNumber", oTransactiondetails.AttributeI("sMobileNumber").sValue);
                    oBOI.SetAttribute("sEmail", oTransactiondetails.AttributeI("sEmail").sValue);
                    oBOI.SetAttribute("sFirstName", oTransactiondetails.AttributeI("sName").sValue);
                    oBOI.SetAttribute("FKiOrganisationID", xifRole.FKiOrganizationID.ToString());
                    oBOI.SetAttribute("FKiApplicationID", oQE.XIDataSource.FKiApplicationID.ToString());
                    iOrgID = xifRole.FKiOrganizationID;
                    oCResult = oBOI.Save(oBOI);
                    FKiUserID = ((XIIBO)oCResult.oResult).AttributeI("UserID").sValue;

                    //User Roles
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "User Roles", null);
                    oBOI.BOD = oBOD;
                    oBOI.SetAttribute("UserID", FKiUserID);
                    oBOI.SetAttribute("RoleID", xifRole.RoleID.ToString());
                    oCResult = oBOI.Save(oBOI);

                    //WM User
                    oBOI = new XIIBO();
                    oBOI.SetAttribute("sEmail", oTransactiondetails.AttributeI("sEmail").sValue);
                    oBOI.SetAttribute("sCountry", oTransactiondetails.AttributeI("sMobileNumberCountryCode").sValue);
                    oBOI.SetAttribute("sCurrency", oTransactiondetails.AttributeI("sTransactionCurrencyType").sValue);
                    oBOI.SetAttribute("sMobileNumber", oTransactiondetails.AttributeI("sMobileNumber").sValue);
                    oBOI.SetAttribute("sUniqueNumber", oTransactiondetails.AttributeI("sUniqueNumber").sValue);
                    oBOI.SetAttribute("FKiUserID", FKiUserID);
                    oBOI.SetAttribute("iStatus", "0");
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "WM User", null);
                    oBOI.BOD = oBOD;
                    oCResult = oBOI.Save(oBOI);
                    if (oCResult.bOK == true && oCResult.oResult != null)
                    {
                        oCR.oResult = oCR;
                    }
                }
                else
                {
                    List<XIIBO> oUserMappings = new List<XIIBO>();
                    oQE = new QueryEngine();
                    oWParams = new List<XIWhereParams>();
                    oWParams.Add(new XIWhereParams { sField = "sEmail", sOperator = "=", sValue = oTransactiondetails.AttributeI("sEmail").sValue });
                    oWParams.Add(new XIWhereParams { sField = "FKiUserID", sOperator = "=", sValue = oUsers.FirstOrDefault().AttributeI("UserID").sValue });
                    oQE.AddBO("WM User", null, oWParams);
                    oCR = oQE.BuildQuery();
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        var sSql = (string)oCR.oResult;
                        ExecutionEngine oEE = new ExecutionEngine();
                        oEE.XIDataSource = oQE.XIDataSource;
                        oEE.sSQL = sSql;
                        var oQResult = oEE.Execute();
                        if (oQResult.bOK && oQResult.oResult != null)
                        {
                            oUserMappings = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.ToList();
                        }
                    }
                    if (oUserMappings.Count() == 0)
                    {
                        oBOI = new XIIBO();
                        oBOI.SetAttribute("sEmail", oTransactiondetails.AttributeI("sEmail").sValue);
                        oBOI.SetAttribute("sCountry", oTransactiondetails.AttributeI("sMobileNumberCountryCode").sValue);
                        oBOI.SetAttribute("sCurrency", oTransactiondetails.AttributeI("sTransactionCurrencyType").sValue);
                        oBOI.SetAttribute("sMobileNumber", oTransactiondetails.AttributeI("sMobileNumber").sValue);
                        oBOI.SetAttribute("sUniqueNumber", oTransactiondetails.AttributeI("sUniqueNumber").sValue);
                        oBOI.SetAttribute("FKiUserID", oUsers.FirstOrDefault().AttributeI("UserID").sValue);
                        oBOI.SetAttribute("iStatus", "0");
                        FKiUserID = oUsers.FirstOrDefault().AttributeI("UserID").sValue;
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "WM User", null);
                        oBOI.BOD = oBOD;
                        oCResult = oBOI.Save(oBOI);
                        if (oCResult.bOK == true && oCResult.oResult != null)
                        {
                            oCR.oResult = oCR;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oCR.sMessage = "ERROR: [" + oCR.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCR.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCR.LogToFile();
                oXID.SaveErrortoDB(oCR);
            }
            return oCR;
        }

        public CResult SendMail(string sEmail)
        {
            CResult oCResult = new CResult();
            try
            {
                string sOneClickName = "Guest User";
                XID1Click o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, sOneClickName, null);
                XID1Click o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);

                oCResult.oTraceStack.Add(new CNV { sName = "Guest User Mail template", sValue = "Guest User Mail template Query:" + o1ClickC.Query });
                oXID.SaveErrortoDB(oCResult);
                Dictionary<string, XIIBO> oRes = o1ClickC.OneClick_Run(false);

                if (oRes != null && oRes.Count() > 0)
                {
                    XIIXI oIXI = new XIIXI();
                    List<CNV> oParams = new List<CNV>();
                    oParams.Add(new CNV { sName = "FKiUserID", sValue = FKiUserID });
                    var oLIst = oIXI.BOI("WM Transaction", null, null, oParams);
                    var oInstance = oLIst.Structure("User Transactions").XILoad();
                    var oBOIList = oRes.Values.ToList();

                    XIContentEditors oXIContent = new XIContentEditors();
                    string sContent = string.Empty;
                    var oXIIContent = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, null, "395");
                    var oXIContentC = oXIIContent.GetCopy();
                    var oXIContentCDef = oXIContentC.FirstOrDefault();
                    if (oXIContentCDef != null)
                    {
                        sContent = (string)oXIContent.MergeContentTemplate(oXIContentCDef, oInstance).oResult;
                    }
                    XIInfraEmail oEmail = new XIInfraEmail();
                    oEmail.sSubject = oRes.FirstOrDefault().Value.AttributeI("sDefaultSubject").sValue;
                    //sEmail = "mounika.menikonda@inativetech.com";
                    oEmail.EmailID = sEmail;
                    var oMailResult = oEmail.Sendmail(iOrgID, sContent, null, 0, null, 0);//send mail with attachments
                    if (oMailResult.bOK && oMailResult.oResult != null)
                    {
                        oCResult.oTraceStack.Add(new CNV { sName = "Mail send successfully", sValue = "Mail send successfully to email:" + sEmail + "" });
                        oXID.SaveErrortoDB(oCResult);
                    }
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oXID.SaveErrortoDB(oCResult);
            }
            oCResult.oTraceStack.Add(new CNV { sName = "Policy Documents", sValue = "Success : Mail sended successfully" });
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            return oCResult;
        }


        public CResult InsertAddress(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            try
            {
                int iQSInstanceID = 0;
                XIIXI oXII = new XIIXI();
                string iID = oParams.Where(m => m.sName.ToLower() == "iQSInstanceID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sUserID = oParams.Where(m => m.sName.ToLower() == "iUserID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                int.TryParse(iID, out iQSInstanceID);
                if (iQSInstanceID > 0)
                {
                    XIIQS oQSI = oXII.GetQSXIValuesByQSIID(iQSInstanceID.ToString());
                    XIIBO oBOI = new XIIBO();
                    XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "wm address", null);
                    oBOI.BOD = oBOD;
                    oBOI.LoadBOI("create");

                    if (oQSI.XIValues.ContainsKey("CountryofResidence") && oBOI.Attributes.ContainsKey("scountry"))
                    {
                        oBOI.SetAttribute("scountry", oQSI.XIValues["CountryofResidence"].sValue);
                    }
                    if (oQSI.XIValues.ContainsKey("BuildingNumber") && oBOI.Attributes.ContainsKey("sbuildingname"))
                    {
                        oBOI.SetAttribute("sbuildingname", oQSI.XIValues["BuildingNumber"].sValue);
                    }
                    if (oQSI.XIValues.ContainsKey("StreetAddress") && oBOI.Attributes.ContainsKey("sstreet"))
                    {
                        oBOI.SetAttribute("sstreet", oQSI.XIValues["StreetAddress"].sValue);
                    }
                    if (oQSI.XIValues.ContainsKey("City") && oBOI.Attributes.ContainsKey("scity"))
                    {
                        oBOI.SetAttribute("scity", oQSI.XIValues["City"].sValue);
                    }
                    if (oQSI.XIValues.ContainsKey("CountyState") && oBOI.Attributes.ContainsKey("sstate"))
                    {
                        oBOI.SetAttribute("sstate", oQSI.XIValues["CountyState"].sValue);
                    }
                    if (oQSI.XIValues.ContainsKey("PostalZipCode") && oBOI.Attributes.ContainsKey("spostcode"))
                    {
                        oBOI.SetAttribute("spostcode", oQSI.XIValues["PostalZipCode"].sValue);
                    }
                    if (oBOI.Attributes.ContainsKey("fkiuserid"))
                    {
                        oBOI.SetAttribute("fkiuserid", sUserID);
                    }
                    oCResult = oBOI.Save(oBOI);
                    if (oCResult.bOK && oCResult.oResult != null)
                    {

                    }
                    else
                    {
                        oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - Error while saving Address in WM_Address_T table ";
                    }
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oXID.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        public CResult InsertAccount(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            try
            {
                int iQSInstanceID = 0;
                XIIXI oXII = new XIIXI();
                string iID = oParams.Where(m => m.sName.ToLower() == "iQSInstanceID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sUserID = oParams.Where(m => m.sName.ToLower() == "iUserID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                int.TryParse(iID, out iQSInstanceID);
                if (iQSInstanceID > 0)
                {
                    XIIQS oQSI = oXII.GetQSXIValuesByQSIID(iQSInstanceID.ToString());
                    XIIBO oBOI = new XIIBO();
                    XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "wm account", null);
                    oBOI.BOD = oBOD;
                    oBOI.LoadBOI("create");

                    if (oQSI.XIValues.ContainsKey("BankAccountHeld") && oBOI.Attributes.ContainsKey("scountry"))
                    {
                        oBOI.SetAttribute("scountry", oQSI.XIValues["BankAccountHeld"].sValue);
                    }
                    if (oQSI.XIValues.ContainsKey("AccountType") && oBOI.Attributes.ContainsKey("iaccounttype"))
                    {
                        oBOI.SetAttribute("iaccounttype", oQSI.XIValues["AccountType"].sValue);
                    }
                    if (oQSI.XIValues.ContainsKey("BranchCode") && oBOI.Attributes.ContainsKey("sbranchcode"))
                    {
                        oBOI.SetAttribute("sbranchcode", oQSI.XIValues["BranchCode"].sValue);
                    }
                    if (oQSI.XIValues.ContainsKey("SWIFTBIC") && oBOI.Attributes.ContainsKey("sswiftbic"))
                    {
                        oBOI.SetAttribute("sswiftbic", oQSI.XIValues["SWIFTBIC"].sValue);
                    }
                    if (oQSI.XIValues.ContainsKey("BankAccountNumber") && oBOI.Attributes.ContainsKey("saccountnumber"))
                    {
                        oBOI.SetAttribute("saccountnumber", oQSI.XIValues["BankAccountNumber"].sValue);
                    }
                    if (oQSI.XIValues.ContainsKey("AccountCurrency") && oBOI.Attributes.ContainsKey("scurrency"))
                    {
                        oBOI.SetAttribute("scurrency", oQSI.XIValues["AccountCurrency"].sValue);
                    }
                    if (oBOI.Attributes.ContainsKey("fkiuserid"))
                    {
                        oBOI.SetAttribute("fkiuserid", sUserID);
                    }
                    oCResult = oBOI.Save(oBOI);
                    if (oCResult.bOK && oCResult.oResult != null)
                    {

                    }
                    else
                    {
                        oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - Error while saving Address in WM_Address_T table ";
                    }
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oXID.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        public CResult InsertPayPreferences(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            try
            {
                int iQSInstanceID = 0;
                XIIXI oXII = new XIIXI();
                string iID = oParams.Where(m => m.sName.ToLower() == "iQSInstanceID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sUserID = oParams.Where(m => m.sName.ToLower() == "iUserID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                int.TryParse(iID, out iQSInstanceID);
                if (iQSInstanceID > 0)
                {
                    XIIQS oQSI = oXII.GetQSXIValuesByQSIID(iQSInstanceID.ToString());
                    XIIBO oBOI = new XIIBO();
                    XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "wm paypreference", null);
                    oBOI.BOD = oBOD;
                    oBOI.LoadBOI("create");

                    if (oQSI.XIValues.ContainsKey("SelectAccountNumberId") && oBOI.Attributes.ContainsKey("fkiaccountid"))
                    {
                        oBOI.SetAttribute("fkiaccountid", oQSI.XIValues["SelectAccountNumberId"].sValue);
                    }
                    if (oQSI.XIValues.ContainsKey("PaytoAccountType") && oBOI.Attributes.ContainsKey("iaccounttype"))
                    {
                        oBOI.SetAttribute("iaccounttype", oQSI.XIValues["PaytoAccountType"].sValue);
                    }
                    if (oQSI.XIValues.ContainsKey("SelectPaymentCurrency") && oBOI.Attributes.ContainsKey("scurrency"))
                    {
                        oBOI.SetAttribute("scurrency", oQSI.XIValues["SelectPaymentCurrency"].sValue);
                    }
                    if (oQSI.XIValues.ContainsKey("AutoPayPreferenceType") && oBOI.Attributes.ContainsKey("sautopaypreferencetype"))
                    {
                        oBOI.SetAttribute("sautopaypreferencetype", oQSI.XIValues["AutoPayPreferenceType"].sValue);
                    }
                    if (oQSI.XIValues.ContainsKey("AutoPayPreferences") && oBOI.Attributes.ContainsKey("sautopayrefrence"))
                    {
                        oBOI.SetAttribute("sautopayrefrence", oQSI.XIValues["AutoPayPreferences"].sValue);
                    }
                    if (oBOI.Attributes.ContainsKey("fkiuserid"))
                    {
                        oBOI.SetAttribute("fkiuserid", sUserID);
                    }
                    oCResult = oBOI.Save(oBOI);
                    if (oCResult.bOK && oCResult.oResult != null)
                    {

                    }
                    else
                    {
                        oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - Error while saving Address in WM_Address_T table ";
                    }
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oXID.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        public CResult ChangeTransactionStatus(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            try
            {
                string sTID = oParams.Where(m => m.sName.ToLower() == "sTID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                if (!string.IsNullOrEmpty(sTID))
                {
                    XIIBO oBOI = new XIIBO();
                    XIIXI oXI = new XIIXI();
                    List<CNV> oWhrParams = new List<CNV>();
                    oWhrParams.Add(new CNV() { sName = "xiguid", sValue = sTID.ToString() });
                    oBOI = oXI.BOI("wm transaction", null, null, oWhrParams);
                    if (oBOI.Attributes.ContainsKey("istatus"))
                    {
                        oBOI.Attributes["istatus"].sValue = "20";
                        oBOI.Attributes["istatus"].bDirty = true;
                    }
                    oCResult = oBOI.Save(oBOI);
                    if(oCResult.bOK && oCResult.oResult != null)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oXID.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
    }
}