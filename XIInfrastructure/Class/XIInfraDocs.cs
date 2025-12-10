using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using XISystem;
using XIDatabase;
using System.Configuration;
using XICore;
using System.Net.Mail;
using System.IO;

namespace XIInfrastructure
{
    //[Table("XIDocs")]
    public class XIInfraDocs:XIInstanceBase
    {
        [Key]
        public int ID { get; set; }
        public string FileName { get; set; }
        public int FKiDocType { get; set; }
        public string SubDirectoryPath { get; set; }
        public int iSystemgeneratedorUploaded { get; set; }
        public string sFullPath { get; set; }
        public int iCreatedBy { get; set; }
        public DateTime dCreatedTime { get; set; }
        public string sCreatedBySYSID { get; set; }
        public int iUpdatedBy { get; set; }
        public DateTime dUpdatedTime { get; set; }
        public string sUpdatedBySYSID { get; set; }
        public int iInstanceID { get; set; }

        public int FKiUserID { get; set; }
        public string sOrgName { get; set; }
        public string sCoreDatabase { get; set; }
        public int iOrgID { get; set; }
        public string sOrgDatabase { get; set; }

        XIIXI oIXI = new XIIXI();
        XIInfraCache oCache = new XIInfraCache();
        public CResult Get_FilePathDetails()
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            //if iTraceLevel>0 then 
            //oCResult.oTraceStack.Trace("Stage", "Begin query build",milliseconds)
            //oCResult.oTraceStack.Trace("Stage",sError)
            //end if

            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            //in the case of
            //xiEnumSystem.xiFuncResult.xiLogicalError
            oCResult.sMessage = "someone tried to do something they shouldnt";

            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?
            
            try
            {
                List<XIInfraDocs> oXIDocs = null;
                XIIXI oXIIXI = new XIIXI();
                XIInfraDocTypes oXIDocTypes = new XIInfraDocTypes();
                cConnectionString oConString = new cConnectionString();
                //string sConString = oConString.ConnectionString(sCoreDataBase);
                //XIDBAPI Connection = new XIDBAPI(sConString);
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params["ID"] = ID;
                //oXIDocs = Connection.Select<XIInfraDocs>("Documents_T", Params).ToList();
                XIIBO oBOI = oXIIXI.BOI("Documents_T",ID.ToString());
                var id = oBOI.Attributes.Where(x => x.Key.ToLower() == "ID".ToLower()).Select(x => x.Value.sValue).FirstOrDefault();
                var FileName = oBOI.Attributes.Where(x => x.Key.ToLower() == "FileName".ToLower()).Select(x => x.Value.sValue).FirstOrDefault();
                var sFileAliasName = oBOI.Attributes.Where(x => x.Key.ToLower() == "sAliasName".ToLower()).Select(x => x.Value.sValue).FirstOrDefault();
                var SubDirectoryPath = oBOI.Attributes.Where(x => x.Key.ToLower() == "SubDirectoryPath".ToLower()).Select(x => x.Value.sValue).FirstOrDefault();
                List<XIDropDown> oXIDFileDDL = new List<XIDropDown>();
                XIDropDown oFilesDDl = new XIDropDown();
                oFilesDDl.ID = Convert.ToInt32(id);
                oFilesDDl.text = sFileAliasName;
                oFilesDDl.Type = SubDirectoryPath;
                //oFilesDDl.Value = FileName;
                oXIDFileDDL.Add(oFilesDDl);
                //var oXIDLayoutDDL = oXIDocs.Select(m => new XIDropDown { ID = Convert.ToInt32(m.ID), text = m.FileName, Type = m.SubDirectoryPath }).ToList();
                var sFileDocTypeID = Convert.ToInt32(oBOI.Attributes.Where(x => x.Key.ToLower() == "FKiDocType".ToLower()).Select(x => x.Value.sValue).FirstOrDefault());
                var sFileDocTypePath = oXIDFileDDL.Select(d => d.Type).FirstOrDefault();
                var sFileDocFileName = FileName;
                oXIDocTypes.ID = sFileDocTypeID;
                var oXIDocDetails = (XIInfraDocTypes)oXIDocTypes.Get_FileDocTypes().oResult;
                if (oXIDocDetails != null)
                {
                    var sType = oXIDocDetails.Type;
                    //var Path = ConfigurationManager.AppSettings["XIDocsPath"] + "//" + sType + "//" + sFileDocTypePath + "//" + sFileDocFileName;
                    var Path = oXIDocDetails.Path + "//" + sFileDocTypePath + "//" + sFileDocFileName;
                    Path = Path.Replace("~","");
                    //var Path = ConfigurationManager.AppSettings["XIDocsPath"] + "//" + sType + "//" + sFileDocTypePath + "//" + sFileDocFileName;
                    oXIDFileDDL.ForEach(m => m.Expression = Path);
                    oCResult.oResult = oXIDFileDDL;
                }
                else
                {
                    oCResult.sMessage = "File Not Found";
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult; // always
        }

        public CResult SaveDocumentsToFolder(MemoryStream File, string sOrgName, string sFileName,string sFileType,string sFolderPath)
        {
            CResult oCResult = new CResult();
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            try
            {
                MemoryStream file = new MemoryStream(File.ToArray());
                byte[] data1 = file.ToArray();
                HttpPostedFileBase objFile = (HttpPostedFileBase)new MemoryPostedFile(data1, sFileName, "application/"+ sFileType + "");
                string sNewGUID = Guid.NewGuid().ToString(); 
                string sAttachmentPath = sNewGUID + "\\" + sOrgName + "\\" + sFileType + "\\";
                string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                //string sPath = physicalPath.Substring(0, physicalPath.Length) + System.Configuration.ConfigurationManager.AppSettings["Attachment"] + "\\" + sNewGUID + "\\" + sOrgName + "\\" + "PDF" + "\\";
                string sPath = physicalPath.Substring(0, physicalPath.Length) + sFolderPath.Replace("~","") + "\\" + sNewGUID + "\\" + sOrgName + "\\" + "PDF" + "\\";
                oCResult.sMessage = sPath;
                SaveErrortoDB(oCResult);
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                if (!System.IO.Directory.Exists(sPath))
                {
                    try
                    {
                        Directory.CreateDirectory(sPath);
                    }
                    catch (Exception ex)
                    {
                        oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                    }
                }

                WriteFileFromStream(objFile.InputStream, sPath + sFileName);
                oCResult.oResult = sAttachmentPath;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
        public CResult SaveDocuments(MemoryStream File,string sFileName)
        {
            CResult oCResult = new CResult();
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            try
            {
                string sID = ""; int iDocTypeID = 0;
                XIIBO oBOI = new XIIBO(); XIDBO oBOD = new XIDBO();
                XIDXI oXID = new XIDXI();
                // string sFileType = "pdf";
                string[] sFileTypeArray = sFileName.Split('.');
                string sDFileName = "";
                string sFileType = sFileTypeArray[1];
                //XIInfraDocTypes oXIDocTypes = new XIInfraDocTypes();
                //oXIDocTypes.Type = sFileType;
                var oXIDocDetails = (XIInfraDocTypes)oCache.GetObjectFromCache(XIConstant.CacheDocType, sFileType);
                var sFolderPath = oXIDocDetails.Path;
                var oResult = SaveDocumentsToFolder(File, sOrgName, sFileName, sFileName.Split('.')[1], sFolderPath);
                if (oResult.bOK && oResult.oResult != null)
                {
                    oCResult.oTraceStack.Add(new CNV { sName = "Document saving", sValue = "" + sFileName + " Document saved to folder successfully" });
                    string sAttachmentPath = (string)oResult.oResult;
                    //var oXIDocDetails = (XIInfraDocTypes)oXIDocTypes.Get_FileDocTypes(sFileType).oResult;
                    string sDoctTypeID = Convert.ToString(oXIDocDetails.ID);
                    //XIIBO oBOIList = oIXI.BOI("XIDocTypes", Convert.ToString(2));
                    //string sDoctTypeID = oBOIList.Attributes.Where(x => x.Key.ToLower() == "id".ToLower()).Select(x => x.Value.sValue).FirstOrDefault();
                    iDocTypeID = Convert.ToInt32(sDoctTypeID);

                    oBOI = new XIIBO();
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Documents_T");
                    //oBOD = (XIDBO)oXID.Get_BODefinition("Documents_T").oResult;
                    oBOI.BOD = oBOD;
                    oBOI.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = null, bDirty = true };
                    oBOI.Attributes["FileName"] = new XIIAttribute { sName = "FileName", sValue = sDFileName, bDirty = true };
                    oBOI.Attributes["FKiDocType"] = new XIIAttribute { sName = "FKiDocType", sValue = "0", bDirty = true };
                    oBOI.Attributes["dCreatedTime"] = new XIIAttribute { sName = "dCreatedTime", sValue = DateTime.Now.ToString(), bDirty = true };
                    oBOI.Attributes["dUpdatedTime"] = new XIIAttribute { sName = "dUpdatedTime", sValue = DateTime.Now.ToString(), bDirty = true };
                    
                    var Response = oBOI.Save(oBOI);//to save XIDocs related parameters
                    XIIBO oBOInstance = new XIIBO();
                    if (Response.bOK && Response.oResult != null)
                    {
                        oBOInstance = (XIIBO)Response.oResult;
                    }
                    if (oBOInstance != null)
                    {
                        sID = oBOInstance.Attributes.Where(x => x.Key.ToLower() == oBOI.BOD.sPrimaryKey.ToLower()).Select(x => x.Value.sValue).FirstOrDefault();
                    }
                    string sNewImageName = "";
                    int iDocID = 0;
                    if(!string.IsNullOrEmpty(sID))
                    {
                        iDocID = Convert.ToInt32(sID);
                    }
                    if (iDocID > 0)
                    {
                        sNewImageName = "" + sFileTypeArray[0] + "_" + iOrgID + "_" + FKiUserID + "_" + iDocID + "." + sFileTypeArray[1] + "";
                    }
                    oBOI = new XIIBO();
                    //oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Documents_T");
                    oBOI.BOD = oBOD;
                    oBOI.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = sID, bDirty = true };
                    oBOI.Attributes["FileName"] = new XIIAttribute { sName = "FileName", sValue = sNewImageName, bDirty = true };
                    oBOI.Attributes["FKiDocType"] = new XIIAttribute { sName = "FKiDocType", sValue = iDocTypeID.ToString(), bDirty = true };
                    oBOI.Attributes["SubDirectoryPath"] = new XIIAttribute { sName = "SubDirectoryPath", sValue = null, bDirty = true };
                    oBOI.Attributes["sFullPath"] = new XIIAttribute { sName = "sFullPath", sValue = sAttachmentPath+sFileName, bDirty = true };
                    oBOI.Attributes["iSystemgeneratedorUploaded"] = new XIIAttribute { sName = "iSystemgeneratedorUploaded", sValue = "10", bDirty = true };
                    oBOI.Attributes["dCreatedTime"] = new XIIAttribute { sName = "dCreatedTime", sValue = DateTime.Now.ToString(), bDirty = true };
                    oBOI.Attributes["dUpdatedTime"] = new XIIAttribute { sName = "dUpdatedTime", sValue = DateTime.Now.ToString(), bDirty = true };
                    oBOI.Attributes["iInstanceID"] = new XIIAttribute { sName = "iInstanceID", sValue = iInstanceID.ToString(), bDirty = true };
                    oBOI.Attributes["FKiUserID"] = new XIIAttribute { sName = "FKiUserID", sValue = FKiUserID.ToString(), bDirty = true };
                    oBOI.Attributes["sAliasName"] = new XIIAttribute { sName = "sAliasName", sValue = sFileName, bDirty = true };

                    var Result = oBOI.Save(oBOI);//to save XIDocs related parameters
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = sID;
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                oCResult.LogToFile();
            }
            return oCResult;
        }
        public static void WriteFileFromStream(Stream stream, string toFile)
        {
            using (FileStream fileToSave = new FileStream(toFile, FileMode.Create))
            {
                stream.CopyTo(fileToSave);
            }
        }
        public class MemoryPostedFile : HttpPostedFileBase
        {
            private readonly byte[] fileBytes;

            public MemoryPostedFile(byte[] fileBytes, string fileName = null, string contenttype = null)
            {
                this.fileBytes = fileBytes;
                this.FileName = fileName;
                this.ContentType = contenttype;
                this.InputStream = new MemoryStream(fileBytes);
            }
            public override string ContentType { get; }

            public override int ContentLength => fileBytes.Length;

            public override string FileName { get; }

            public override Stream InputStream { get; }
        }
    }
}