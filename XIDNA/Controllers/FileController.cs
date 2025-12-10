using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XIDNA.Models;
using System.Web.Mvc;
using XIDNA.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using XIDNA.ViewModels;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
//using XIDataBase.Hubs;
using XIDNA.Common;
using XICore;
using XISystem;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Web.Script.Serialization;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace XIDNA.Controllers
{
    // [Authorize]
    public class FileController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IFileRepository FileRepository;

        public FileController() : this(new FileRepository()) { }

        public FileController(IFileRepository FileRepository)
        {
            this.FileRepository = FileRepository;
        }
        CommonRepository Common = new CommonRepository();
        private readonly string xiconstant;

        //[HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetXIFileDetails(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = FileRepository.GetXIFileDetails(param, iUserID, sOrgName, sDatabase);
                return Json(new
                {
                    sEcho = result.sEcho,
                    iTotalRecords = result.iTotalRecords,
                    iTotalDisplayRecords = result.iTotalDisplayRecords,
                    aaData = result.aaData
                },
                JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult AddXIFileType(int ID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
            XIFileTypes FileDetails = FileRepository.AddXIFileType(ID, iUserID, sOrgName, sDatabase);
            if (FileDetails == null)
            {
                return null;
            }
            else
            {

                return PartialView("AddXIFileConfigSettings", FileDetails);
            }
        }
        public ActionResult CreateFileSettings(XIFileTypes model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                var Create = FileRepository.CreateFileSettings(model, iUserID, sOrgName, sDatabase);
                return Json(Create, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteFileDetails(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                int iStatus = FileRepository.DeleteFileDetails(ID, iUserID, sOrgName, sDatabase);
                return Json(iStatus, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult ViewXIDocDetails()
        {
            return View();
        }
        public ActionResult GetXIDocDetails(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = FileRepository.GetXIDocDetails(param, iUserID, sOrgName, sDatabase);
                return Json(new
                {
                    sEcho = result.sEcho,
                    iTotalRecords = result.iTotalRecords,
                    iTotalDisplayRecords = result.iTotalDisplayRecords,
                    aaData = result.aaData
                },
                JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult AddXIDocDetails(int ID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
            XIDocTypes DocSettings = FileRepository.AddXIDocDetails(ID, iUserID, sOrgName, sDatabase);
            return PartialView("AddXIDocDetails", DocSettings);
        }

        public ActionResult DeleteDocDetails(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                int iStatus = FileRepository.DeleteDocDetails(ID, iUserID, sOrgName, sDatabase);
                return Json(iStatus, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult CreateDocSettings(XIDocTypes model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                var Create = FileRepository.CreateDocSettings(model, iUserID, sOrgName, sDatabase);
                return Json(Create, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        #region FolerOperations

        [HttpPost]
        public ActionResult FolderOperations(string sParentID, string sFolderName, string sType, string sOldFolder, string ID, string sFileType, string iBuildingID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                sFolderName = sFolderName.Replace(@"//", @"\");
                if (!string.IsNullOrEmpty(sOldFolder))
                {
                    sOldFolder = sOldFolder.Replace(@"//", @"\");
                    sOldFolder = sOldFolder.Replace("&amp;", "&");
                }
                XIInfraCache oCache = new XIInfraCache();
                var BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "xidocumenttree");
                string sPath = string.Empty;
                var sVirtualDir = System.Configuration.ConfigurationManager.AppSettings["VirtualDirectoryPath"];
                var sVirtualPath = @"~\" + sVirtualDir + @"\Createif\PDF\Client1\Project1\";
                sFolderName = sFolderName.Replace("&amp;", "&");
                string sNewFolder = string.Empty;
                //Create new folder or Add level in a directory
                if (sType.ToLower() == "create" || sType.ToLower() == "add level")
                {
                    var sFoldName = string.Empty;
                    sFoldName = sFolderName;
                    if (sFolderName.Contains(@"\"))
                    {
                        var FName = sFolderName.LastIndexOf(@"\");
                        var FolName = sFolderName.Substring(FName, sFolderName.Length - FName).Replace(@"\", "");
                        sFoldName = FolName;
                        sNewFolder = FolName;
                    }

                    sPath = Server.MapPath(sVirtualPath) + sFolderName;
                    System.IO.Directory.CreateDirectory(sPath);
                    //Save folder to XIDocumentTree BO
                    XIIBO oBOI = new XIIBO();
                    oBOI.BOD = BOD;
                    oBOI.SetAttribute("sname", sFoldName);
                    oBOI.SetAttribute("sparentid", sParentID);
                    oBOI.SetAttribute("sType", "10");
                    oBOI.SetAttribute("sPageNo", "1");
                    oBOI.SetAttribute("iBuildingID", iBuildingID.ToString());
                    oBOI.SetAttribute("iApprovalStatus", "20");
                    var oCR = oBOI.Save(oBOI);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        var iID = oBOI.AttributeI(BOD.sPrimaryKey).sValue;
                        if (sType.ToLower() == "add level")
                        {
                            //Get all child nodes and change parentid
                            XID1Click o1Click = new XID1Click();
                            o1Click.BOID = BOD.BOID;
                            o1Click.Query = "select * from xidocumenttree_t where sparentid=" + sParentID;
                            var ChildNodes = o1Click.OneClick_Run();
                            if (ChildNodes != null && ChildNodes.Values.Count() > 0)
                            {
                                foreach (var Child in ChildNodes.Values)
                                {
                                    var PID = Child.AttributeI(BOD.sPrimaryKey).sValue;
                                    if (PID != iID)
                                    {
                                        Child.BOD = BOD;
                                        Child.SetAttribute("sparentid", iID);
                                        oCR = Child.Save(Child);
                                        if (oCR.bOK && oCR.oResult != null)
                                        {

                                        }
                                    }
                                }
                                //Copy the files and directories from old to new directory
                                //var sFoldName = string.Empty;
                                sFoldName = sFolderName;
                                if (sFolderName.Contains(@"\"))
                                {
                                    var FName = sFolderName.LastIndexOf(@"\");
                                    var FolName = sFolderName.Substring(0, FName);
                                    sFoldName = FolName;
                                }
                                var targetDirectory = Server.MapPath(sVirtualPath) + sFolderName;
                                string sourceDirectory = Server.MapPath(sVirtualPath) + sFoldName;
                                Copy(sourceDirectory, targetDirectory);
                                //System.IO.Directory.Move(sOldPath, sNewPath);
                            }
                            //Change the full path of files/docs in Documents_T table after adding the new level
                            if (sType.ToLower() == "add level")
                            {
                                List<CNV> oParms = new List<CNV>();
                                oParms.Add(new CNV { sName = "ID", sValue = sParentID });
                                XIIXI oXI = new XIIXI();
                                var BOI = oXI.BOI("xidocumenttree", null, null, oParms);
                                var sParentFolder = BOI.AttributeI("sname").sValue;
                                XIDStructure oStr = new XIDStructure();
                                var oResult = oStr.Get_SelfStructure("", sParentID, 1232, "", iBuildingID, "treeamend");
                                if (oResult != null && oResult.oResult != null)
                                {
                                    var Childs = (Dictionary<string, XIIBO>)oResult.oResult;
                                    if (Childs.Count() > 0)
                                    {
                                        GetFiles(Childs.Values.ToList(), sParentFolder, sParentFolder + @"\" + sNewFolder);
                                    }
                                }
                            }
                        }
                        return Json(iID, JsonRequestBehavior.AllowGet);
                    }
                }
                else if (sType.ToLower() == "rename")
                {
                    //Rename the directory or Doc
                    var sFoldName = string.Empty;
                    sFoldName = sFolderName;
                    if (sFolderName.Contains(@"\"))
                    {
                        var FName = sFolderName.LastIndexOf(@"\");
                        var FolName = sFolderName.Substring(FName, sFolderName.Length - FName).Replace(@"\", "");
                        sFoldName = FolName;
                    }

                    //if (oCR.bOK && oCR.oResult != null)
                    //{
                    //Renaming the directory with Move command
                    sPath = Server.MapPath(sVirtualPath) + sFolderName;
                    string sOldPath = Server.MapPath(sVirtualPath) + sOldFolder;
                    System.IO.Directory.Move(sOldPath, sPath);
                    XIIBO oBOI = new XIIBO();
                    oBOI.BOD = BOD;
                    oBOI.SetAttribute("id", ID);
                    oBOI.SetAttribute("sname", sFoldName);
                    var oCR = oBOI.Save(oBOI);
                    if (oCR.bOK && oCR.oResult != null)
                    {

                    }
                    else
                    {
                        return Json(0, JsonRequestBehavior.AllowGet);
                    }
                    var sOldFoldName = string.Empty;
                    if (sOldFolder.Contains(@"\"))
                    {
                        var FName = sOldFolder.LastIndexOf(@"\");
                        var FolName = sOldFolder.Substring(FName, sOldFolder.Length - FName).Replace(@"\", "");
                        sOldFoldName = FolName;
                    }
                    //Change the full path of files/docs in Documents_T table after adding the new level
                    List<CNV> oParms = new List<CNV>();
                    oParms.Add(new CNV { sName = "sname", sValue = sFoldName });
                    if (!string.IsNullOrEmpty(iBuildingID))
                    {
                        oParms.Add(new CNV { sName = "ibuildingid", sValue = iBuildingID.ToString() });
                    }
                    XIIXI oXI = new XIIXI();
                    var BOI = oXI.BOI("xidocumenttree", null, null, oParms);
                    var PID = BOI.AttributeI(BOD.sPrimaryKey).sValue;
                    XIDStructure oStr = new XIDStructure();
                    var oResult = oStr.Get_SelfStructure("", PID, 1232, "", iBuildingID, "treeamend");
                    if (oResult != null && oResult.oResult != null)
                    {
                        var Childs = (Dictionary<string, XIIBO>)oResult.oResult;
                        if (Childs.Count() > 0)
                        {
                            GetFiles(Childs.Values.ToList(), sOldFoldName, sFoldName);
                        }
                    }
                    var iID = oBOI.AttributeI(BOD.sPrimaryKey).sValue;
                    return Json(iID, JsonRequestBehavior.AllowGet);
                    //}
                    //return Json(0, JsonRequestBehavior.AllowGet);
                }
                else if (sType.ToLower() == "delete" || sType.ToLower() == "remove level")
                {
                    //Delete or remove level of directory
                    //Delete means not the physical delete, updating XIDeleted to 1
                    var oCR = new CResult();
                    if (sType.ToLower() == "remove level")
                    {
                        //Get the child dictories or docs and change the parentid
                        XID1Click o1Click = new XID1Click();
                        o1Click.BOID = BOD.BOID;
                        o1Click.Query = "select * from xidocumenttree_t where sparentid=" + ID + " and " + XIConstant.Key_XIDeleted + "=0";
                        var oRes = o1Click.OneClick_Run();
                        if (oRes != null && oRes.Values.Count() > 0)
                        {
                            foreach (var item in oRes.Values)
                            {
                                item.BOD = BOD;
                                item.SetAttribute("sparentid", sParentID);
                                oCR = item.Save(item);
                            }
                            //Copy the files from old to new directory
                            var sFoldNam = sFolderName;
                            if (sFolderName.Contains(@"\"))
                            {
                                var FName = sFolderName.LastIndexOf(@"\");
                                var FolName = sFolderName.Substring(0, FName);
                                sFoldNam = FolName;
                            }
                            var sourceDirectory = Server.MapPath(sVirtualPath) + sFolderName;
                            string targetDirectory = Server.MapPath(sVirtualPath) + sFoldNam;
                            Copy(sourceDirectory, targetDirectory);
                        }
                    }
                    //Change the directory or doc name by adding Delete_CurrentDateTime
                    var sFoldName = string.Empty;
                    sFoldName = sFolderName;
                    if (sFolderName.Contains(@"\"))
                    {
                        var FName = sFolderName.LastIndexOf(@"\");
                        var FolName = sFolderName.Substring(FName, sFolderName.Length - FName).Replace(@"\", "");
                        sFoldName = FolName;
                    }
                    var sDeleteFolderName = "_Delete_" + DateTime.Now.ToString("dd-MMM-yyyy HHmmss");
                    XIIBO oBOI = new XIIBO();
                    oBOI.BOD = BOD;
                    oBOI.SetAttribute("id", ID);
                    oBOI.SetAttribute("sname", sFoldName + sDeleteFolderName);
                    oBOI.SetAttribute(XIConstant.Key_XIDeleted, "1");
                    oCR = oBOI.Save(oBOI);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        //Renaming the directory
                        if (!string.IsNullOrEmpty(sFileType) && sFileType == "10")
                        {
                            sPath = Server.MapPath(sVirtualPath) + sFolderName;
                            string sOldPath = sPath + sDeleteFolderName;
                            System.IO.Directory.Move(sPath, sOldPath);
                        }
                        //Renaming the Doc
                        else if (!string.IsNullOrEmpty(sFileType) && sFileType == "20")
                        {
                            if (!string.IsNullOrEmpty(sFolderName))
                            {
                                var sOldPath = Server.MapPath(sVirtualPath) + sFolderName + ".pdf";
                                sPath = Server.MapPath(sVirtualPath) + sFolderName + sDeleteFolderName + ".pdf";
                                System.IO.File.Move(sOldPath, sPath);
                                //System.IO.File.Delete(sPath);
                                //System.IO.Directory.Delete(sPath);
                            }
                        }
                        if (sType.ToLower() == "remove level")
                        {
                            //Change the full path of files/docs in Documents_T table after removing the level
                            List<CNV> oParms = new List<CNV>();
                            oParms.Add(new CNV { sName = "ID", sValue = sParentID });
                            XIIXI oXI = new XIIXI();
                            var BOI = oXI.BOI("xidocumenttree", null, null, oParms);
                            var sParentFolder = BOI.AttributeI("sname").sValue;
                            XIDStructure oStr = new XIDStructure();
                            var oResult = oStr.Get_SelfStructure("", sParentID, 1232, "", iBuildingID, "treeamend");
                            if (oResult != null && oResult.oResult != null)
                            {
                                var Childs = (Dictionary<string, XIIBO>)oResult.oResult;
                                if (Childs.Count() > 0)
                                {
                                    GetFiles(Childs.Values.ToList(), sParentFolder + @"\" + sFoldName, sParentFolder);
                                }
                            }
                        }
                        return Json(ID, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        //Import File
        //string sFolderPath, string sCategory, string sDelim, string sXMappingDefinition, string sSpecificReference, string sParentFK, string sParentFKAttr
        public string ImportFiles(List<CNV> oParams)
        //public string ImportFiles()
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {
                string sFolderPath = oParams.Where(m => m.sName.ToLower() == "sFolderPath".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sCategory = oParams.Where(m => m.sName.ToLower() == "sCategory".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sDelim = oParams.Where(m => m.sName.ToLower() == "sDelim".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sXMappingDefinition = oParams.Where(m => m.sName.ToLower() == "sXMappingDefinition".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sSpecificReference = oParams.Where(m => m.sName.ToLower() == "sSpecificReference".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sParentFK = oParams.Where(m => m.sName.ToLower() == "sParentFK".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sParentFKAttr = oParams.Where(m => m.sName.ToLower() == "sParentFKAttr".ToLower()).Select(m => m.sValue).FirstOrDefault();
                //Attachment path is  not correct we should be using the document type path based on file type
                var sSharedPath = System.Configuration.ConfigurationManager.AppSettings["SharedPath"];
                var sPath = Server.MapPath(sSharedPath) + "\\Attachements";
                //Copy from XILink Controller and method name is CheckAndCreateDirectory 
                List<string> sSubDirList = new List<string>();
                sSubDirList.Add("year");
                sSubDirList.Add("month");
                sSubDirList.Add("day");
                foreach (var DirNames in sSubDirList)
                {
                    string sNewPath = string.Empty;

                    string sVal = "";
                    DateTime DateTme = DateTime.Now;
                    if (DirNames.ToLower() == "year")
                    {
                        sPath = sPath + "\\" + DateTime.Now.Year;
                    }
                    else if (DirNames.ToLower() == "month")
                    {
                        sPath = sPath + "\\" + DateTime.Now.Month;
                        sVal = DateTme.Month.ToString();
                    }
                    else if (DirNames.ToLower() == "day")
                    {
                        sPath = sPath + "\\" + DateTime.Now.Day;
                    }
                    if (Directory.Exists(sNewPath))
                    {

                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(sNewPath);
                    }
                }

                //sFolderPath = "C:\\Users\\ravit\\XIDNA\\ImportFiles\\Attachements";
                //sCategory = "Email";
                //sDelim = "_";
                string ErrorFolder = string.Empty;
                string sTargetFolder = sPath;
                DirectoryInfo diTarget = new DirectoryInfo(sTargetFolder);
                XIInfraCache oCache = new XIInfraCache();
                oTrace.oParams.Add(new CNV { sName = "sFolderPath", sValue = sFolderPath });
                XIIXI oXI = new XIIXI();
                XIDBO oXIDOCBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIDocumentTree_T");
                XIIBO oDocTreeI = new XIIBO();
                var sParentID = string.Empty;
                bool bImport = false;
                if (!string.IsNullOrEmpty(sFolderPath))
                {
                    DirectoryInfo diSource = new DirectoryInfo(sFolderPath);
                    foreach (FileInfo fi in diSource.GetFiles())
                    {
                        bImport = false;
                        if (!string.IsNullOrEmpty(sSpecificReference))
                        {
                            if (fi.Name.ToLower().StartsWith(sSpecificReference.ToLower()))
                            {
                                bImport = true;
                            }
                        }
                        else
                        {
                            bImport = true;
                        }
                        //Get document tree node name, sCategory and / loadAttr eg Email/alsde33s/myfile.png
                        //check if document tree node already exists or not, if exists get the id
                        if (bImport)
                        {
                            var Splits = fi.Name.Split(new string[] { sDelim }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            if (Splits.Count() > 0)
                            {
                                var sLoadAttr = Splits[0];
                                var sOriginalFileName = Splits[1];
                                oCR = Check_DocumentTree(sCategory, "");
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    string sParent = (string)oCR.oResult;
                                    oCR = Check_DocumentTree(sLoadAttr, sParent);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        sParent = (string)oCR.oResult;
                                        oCR = Check_DocumentTree(sOriginalFileName, sParent);
                                        if (oCR.bOK && oCR.oResult != null)
                                        {
                                            var iDocID = string.Empty;
                                            XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Documents_T");
                                            var oBOI = new XIIBO();
                                            oBOI.BOD = oBOD;
                                            oBOI.SetAttribute("iInstanceID", "");
                                            oBOI.SetAttribute("sFileName", fi.Name);
                                            oBOI.SetAttribute("sAliasName", fi.Name);
                                            oBOI.SetAttribute("FKiDocType", "");
                                            oCR = oBOI.Save(oBOI);
                                            if (oCR.bOK && oCR.oResult != null)
                                            {
                                                oBOI = (XIIBO)oCR.oResult;
                                                iDocID = oBOI.AttributeI("id").sValue;
                                                var oCopyResponse = fi.CopyTo(Path.Combine(diTarget.FullName, fi.Name), true);
                                                oBOI.SetAttribute("sfullPath", Path.Combine(diTarget.FullName, fi.Name));
                                                oCR = oBOI.Save(oBOI);
                                                if (oCR.bOK && oCR.oResult != null)
                                                {
                                                    //XIDBO oXIDOCBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIDocumentTree_T");
                                                    oDocTreeI = new XIIBO();
                                                    oDocTreeI.BOD = oXIDOCBOD;
                                                    oDocTreeI.SetAttribute("sName", fi.Name);
                                                    oDocTreeI.SetAttribute("sParentId", sParent);
                                                    oDocTreeI.SetAttribute("sTags", "");
                                                    oDocTreeI.SetAttribute("sPageNo", "");
                                                    oDocTreeI.SetAttribute("sFolderName", "");
                                                    oDocTreeI.SetAttribute("spath", iDocID);
                                                    oCR = oDocTreeI.Save(oDocTreeI);
                                                    if (oCR.bOK && oCR.oResult != null)
                                                    {
                                                        if (!string.IsNullOrEmpty(sXMappingDefinition))//Enhancement required to extract the sloadattr, we will do only when specific
                                                        {
                                                            XIDBO oMapBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sXMappingDefinition);
                                                            XIIBO oMapBOI = new XIIBO();
                                                            oMapBOI.BOD = oMapBOD;
                                                            oMapBOI.SetAttribute(sParentFKAttr, sParentFK);//FKiCommunicationID
                                                            oMapBOI.SetAttribute("FKiDocumentID", iDocID);//XMappingObject should have this Document FK
                                                            oCR = oMapBOI.Save(oMapBOI);
                                                            if (oCR.bOK && oCR.oResult != null)
                                                            {
                                                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }

                        //Create XIDocument
                        //Copy file on origin to XIDocument target path
                        //if error move file from source to error directory with year/month/date subdirectories
                        //if success move from source to completed directory with year/month/date subdirectories
                        //Split the file name by delimiter and take the first split item as the load attribute
                        //load attribute is passed in as a parameter as Example: sExtRef
                        //load FKDefinition instance by sLoadAttribute(for this we need sFKDefinition and sLoadAtrribute)
                        //use the document fkdefinition and fkinstance to attach to fkobject(XICommunication)

                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: sFolderPath is missing";
                }

            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                //SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            //return oCResult;
            return "Success";
        }

        public string Keywords(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {
                XIIXI oXI = new XIIXI();
                XIInfraCache oCache = new XIInfraCache();
                var CommIID = oParams.Where(m => m.sName.ToLower() == "commiid").Select(m => m.sValue).FirstOrDefault();
                var oCommI = oXI.BOI("XICommunicationI", CommIID);
                if (oCommI != null && oCommI.Attributes.Count() > 0)
                {
                    var sSubject = oCommI.AttributeI("sHeader").sValue;
                    var sBody = oCommI.AttributeI("sContent").sValue;
                    //conversation should have keywords not individual
                    //1click of keyword definitions getting from cache
                    List<XIIBO> KeywordDefs = new List<XIIBO>();
                    for (int i = 0; i < KeywordDefs.Count(); i++)
                    {
                        var bMatch = false;
                        var sKeyword = KeywordDefs[i].AttributeI("sname").sValue;
                        if (sSubject.Contains(sKeyword))
                        {
                            bMatch = true;
                        }
                        else if (sBody.Contains(sKeyword))
                        {
                            bMatch = true;
                        }
                        if (bMatch)
                        {
                            //create keyword instance
                            var oKeyBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "KeywordI");
                            XIIBO oKeyI = new XIIBO();
                            oKeyI.BOD = oKeyBOD;
                            oKeyI.SetAttribute("FKiKeywordDefID", KeywordDefs[i].AttributeI("id").sValue);
                            oKeyI.SetAttribute("iStatus", "0");
                            oKeyI.SetAttribute("FKiBODID", oCommI.iBODID.ToString());
                            oKeyI.SetAttribute("FKiBOIID", CommIID);
                            oKeyI.Save(oKeyI);
                            //now need to execute process controller
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                //SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            //return oCResult;
            return "Success";
        }

        public ActionResult ProcessCommunication()
        {
            List<CNV> oParams = new List<CNV>();
            oParams.Add(new CNV { sName = "commiid", sValue = "450" });
            CommunicationStreamInbound(oParams);
            return null;
        }
        public string CommunicationStreamInbound(List<CNV> oParams)//ConversationMatch
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            XIDefinitionBase oBase = new XIDefinitionBase();
            try
            {
                //TO DO: need a default stream
                //get all streams of type inbound
                //if the identifier is from address then check from and to 
                //if there is a match break the loop
                //if comm type on stream then get pre delim and post delim, check the subject first if it matches then assign the conversationid to this communcation
                //eg:subject='hello ##1234776##'
                //eg:contains'conversion:[dfe343sdfe]'
                //if no match then use predelim2 and postdelim2, check the body if it matches then assign the conversationid to this communcation

                XIInfraCache oCache = new XIInfraCache();
                XID1Click o1ClickD = new XID1Click();
                XIIXI oXI = new XIIXI();
                XIAPI oAPI = new XIAPI();
                o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "Inbound Stream Import");//this will get all the streams with iDirection=10(inbound)
                if (o1ClickD != null)
                {
                    bool bMatch = false;
                    var sSubject = string.Empty;
                    var sBody = string.Empty;
                    var sComTypeID = string.Empty;
                    var oCommI = new XIIBO();
                    var oCommTypeI = new XIIBO();
                    string PCGUID = string.Empty;
                    var CommIID = string.Empty;
                    var Response = o1ClickD.OneClick_Execute();
                    if (Response != null && Response.Count() > 0)
                    {
                        var sRegexCode = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, "InboundRegexOverride");
                        foreach (var stream in Response.Values.ToList())
                        {
                            PCGUID = stream.AttributeI("FKiProcessControllerIDXIGUID").sValue;
                            //Regex on from and to address and subject and body, we need to check all
                            //Examples: *@systemdna.com
                            //Example2: ravit@systemsdna.com,dan.s@systemsdna.com
                            CommIID = oParams.Where(m => m.sName.ToLower() == "commiid").Select(m => m.sValue).FirstOrDefault();
                            oCommI = oXI.BOI("XICommunicationI", CommIID);
                            if (oCommI != null && oCommI.Attributes.Count() > 0)
                            {
                                sSubject = oCommI.AttributeI("sHeader").sValue;
                                sBody = oCommI.AttributeI("sContent").sValue;
                                var sTo = oCommI.AttributeI("sTo").sValue;
                                var sFrom = oCommI.AttributeI("sFrom").sValue;
                                sComTypeID = oCommI.AttributeI("FkiComTypeID").sValue;
                                oCommTypeI = oXI.BOI("XICommunicationType", sComTypeID);
                                //if iIdentifier is not null and > 0 then call the match method with iIdentifier and sIdentifier, 
                                var iIdentifier = stream.AttributeI("iIdentifier").iValue;
                                var iIdentifier2 = stream.AttributeI("iIdentifier2").iValue;
                                var iIdentifier3 = stream.AttributeI("iIdentifier3").iValue;
                                var iIdentifier4 = stream.AttributeI("iIdentifier4").iValue;
                                var sIdentifierMatch = stream.AttributeI("sIdentifierMatch").sValue;
                                var sIdentifierMatch2 = stream.AttributeI("sIdentifierMatch2").sValue;
                                var sIdentifierMatch3 = stream.AttributeI("sIdentifierMatch3").sValue;
                                var sIdentifierMatch4 = stream.AttributeI("sIdentifierMatch4").sValue;
                                List<CNV> oSrchParams = new List<CNV>();
                                int iCurrentIdentifier = 0;
                                string sCurrentIdentifier = String.Empty;
                                string sCurrentTarget = String.Empty;
                                if (iIdentifier > 0)
                                {
                                    for (int j = 1; j < 5; j++)
                                    {
                                        if (j == 1)
                                        {
                                            iCurrentIdentifier = iIdentifier;
                                            sCurrentIdentifier = sIdentifierMatch;
                                        }
                                        else if (j == 2)
                                        {
                                            iCurrentIdentifier = iIdentifier2;
                                            sCurrentIdentifier = sIdentifierMatch2;
                                        }
                                        else if (j == 3)
                                        {
                                            iCurrentIdentifier = iIdentifier3;
                                            sCurrentIdentifier = sIdentifierMatch3;
                                        }
                                        else if (j == 4)
                                        {
                                            iCurrentIdentifier = iIdentifier4;
                                            sCurrentIdentifier = sIdentifierMatch4;
                                        }
                                        if (iCurrentIdentifier > 0)
                                        {
                                            if (iCurrentIdentifier == 10)
                                            {
                                                sCurrentTarget = sTo;
                                            }
                                            else if (iCurrentIdentifier == 20)
                                            {
                                                sCurrentTarget = sFrom;
                                            }
                                            else if (iCurrentIdentifier == 30)
                                            {
                                                sCurrentTarget = sSubject;
                                            }
                                            else if (iCurrentIdentifier == 40)
                                            {
                                                sCurrentTarget = sBody;
                                            }
                                            oSrchParams = new List<CNV>();
                                            oSrchParams.Add(new CNV { sName = "sStringToMatch", sValue = sCurrentTarget });
                                            oSrchParams.Add(new CNV { sName = "sMatchSearchFor", sValue = sCurrentIdentifier });
                                            //oSrchParams.Add(new CNV { sName = "sRegex", sValue = sRegexCode });
                                            oCR = MatchIdentifier(oSrchParams);
                                            if (oCR.bOK && oCR.oResult != null)
                                            {
                                                bMatch = (bool)oCR.oResult;
                                                if (bMatch)
                                                {
                                                    break;
                                                }
                                            }
                                            else
                                            {

                                            }
                                        }
                                    }
                                    if (bMatch)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        if (bMatch)
                        {
                            if (oCommTypeI != null && oCommTypeI.Attributes.Count() > 0)
                            {
                                var sPreDelim1 = oCommTypeI.AttributeI("sPreDelim1").sValue;
                                var sIdentifier1 = oCommTypeI.AttributeI("iIdentifier1").sValue;
                                var sPostDelim1 = oCommTypeI.AttributeI("sPostDelim1").sValue;
                                var sPreDelim2 = oCommTypeI.AttributeI("sPreDelim2").sValue;
                                var sIdentifier2 = oCommTypeI.AttributeI("iIdentifier2").sValue;
                                var sPostDelim2 = oCommTypeI.AttributeI("sPostDelim2").sValue;
                                bool bFound = false;
                                if (!string.IsNullOrEmpty(sSubject))//check subject
                                {
                                    int iStart = sSubject.IndexOf(sPreDelim1);
                                    if (iStart >= 0)
                                    {
                                        var sAfterStart = sSubject.Substring(iStart + 2, sSubject.Length - iStart - 2);
                                        int iEnd = sAfterStart.IndexOf(sPostDelim1);
                                        if (iEnd >= 0)
                                        {
                                            var sMatchCode = sAfterStart.Substring(0, iEnd);
                                            if (!string.IsNullOrEmpty(sMatchCode) && sMatchCode.Length <= 36)
                                            {
                                                bFound = true;
                                                bool bExist = false;
                                                //Check Conversation object for sMatchCode
                                                oCR = FindConversation(sMatchCode);
                                                if (oCR.bOK && oCR.oResult != null)
                                                {
                                                    bExist = (bool)oCR.oResult;
                                                }
                                                if (bExist)
                                                {
                                                    oCommI.SetAttribute("FKiConversationID", sMatchCode);
                                                    oCR = oCommI.Save(oCommI);
                                                }
                                                else
                                                {
                                                    oCR.sMessage = "Conversation not found for code:" + sMatchCode;
                                                    oBase.SaveErrortoDB(oCR);
                                                }
                                            }
                                            else if (!string.IsNullOrEmpty(sMatchCode))
                                            {
                                                oCR.sMessage = "Invalid conversation identifier found:" + sMatchCode;
                                                oBase.SaveErrortoDB(oCR);
                                            }
                                            else
                                            {
                                                //No conversation found
                                            }
                                        }
                                    }
                                }
                                if (!bFound)//check body
                                {
                                    if (!string.IsNullOrEmpty(sBody))
                                    {
                                        int iStart = sBody.IndexOf(sPreDelim2);
                                        if (iStart >= 0)
                                        {
                                            var sAfterStart = sBody.Substring(iStart, sBody.Length);
                                            int iEnd = sAfterStart.IndexOf(sPostDelim2);
                                            if (iEnd >= 0 && iEnd > iStart)
                                            {
                                                var sMatchCode = sBody.Substring(iStart, iEnd - iStart);
                                                if (!string.IsNullOrEmpty(sMatchCode) && sMatchCode.Length <= 36)
                                                {
                                                    bFound = true;
                                                    bool bExist = false;
                                                    //Check Conversation object for sMatchCode
                                                    oCR = FindConversation(sMatchCode);
                                                    if (oCR.bOK && oCR.oResult != null)
                                                    {
                                                        bExist = (bool)oCR.oResult;
                                                    }
                                                    if (bExist)
                                                    {
                                                        oCommI.SetAttribute("FKiConversationID", sMatchCode);
                                                        oCR = oCommI.Save(oCommI);
                                                    }
                                                    else
                                                    {
                                                        oCR.sMessage = "Conversation not found for code:" + sMatchCode;
                                                        oBase.SaveErrortoDB(oCR);
                                                    }
                                                }
                                                else if (!string.IsNullOrEmpty(sMatchCode))
                                                {
                                                    oCR.sMessage = "Invalid conversation identifier found:" + sMatchCode;
                                                    oBase.SaveErrortoDB(oCR);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            //
                            Guid PCXIGUID = Guid.Empty;
                            Guid.TryParse(PCGUID, out PCXIGUID);
                            if (PCXIGUID != null && PCXIGUID != Guid.Empty)
                            {
                                XIDAlgorithm oAlgoD = new XIDAlgorithm();
                                string sSessionID = Guid.NewGuid().ToString();
                                string sNewGUID = Guid.NewGuid().ToString();
                                List<CNV> oNVsList = new List<CNV>();
                                oNVsList.Add(new CNV { sName = "-iBODID", sValue = oCommI.iBODID.ToString() });
                                oNVsList.Add(new CNV { sName = "-iBOIID", sValue = CommIID });
                                oCache.SetXIParams(oNVsList, sNewGUID, sSessionID);
                                oAlgoD = (XIDAlgorithm)oCache.GetObjectFromCache(XIConstant.CacheXIAlgorithm, null, PCXIGUID.ToString());
                                oCR = oAlgoD.Execute_XIAlgorithm(sSessionID, sNewGUID);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    //if error update to the process error
                                    //if success update to process completed
                                    oCommI.SetAttribute("iProcessStatus", "30");
                                    oCR = oCommI.Save(oCommI);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {

                                    }
                                    else
                                    {
                                        oCR.sMessage = "Communication instance saving failed while setting iProcessStatus to 30";
                                        oBase.SaveErrortoDB(oCR);
                                    }
                                }
                                else
                                {
                                    oCommI.SetAttribute("iProcessStatus", "20");
                                    oCR = oCommI.Save(oCommI);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {

                                    }
                                    else
                                    {
                                        oCR.sMessage = "Communication instance saving failed while setting iProcessStatus to 20";
                                        oBase.SaveErrortoDB(oCR);
                                    }
                                }
                            }
                            else
                            {
                                //update status of comm instance to No Process required-10, Process Error-20, Process completed-30
                                oCommI.SetAttribute("iProcessStatus", "10");
                                oCR = oCommI.Save(oCommI);
                                if (oCR.bOK && oCR.oResult != null)
                                {

                                }
                                else
                                {
                                    oCR.sMessage = "Communication instance saving failed while setting iProcessStatus to 10";
                                    oBase.SaveErrortoDB(oCR);
                                }
                            }
                        }
                        else
                        {
                            oCommI.SetAttribute("iProcessStatus", "100");
                            oCR = oCommI.Save(oCommI);
                            if (oCR.bOK && oCR.oResult != null)
                            {

                            }
                            else
                            {
                                oCR.sMessage = "Communication instance saving failed while setting iProcessStatus to 100";
                                oBase.SaveErrortoDB(oCR);
                            }
                        }
                    }
                }



                //var CommIID = oParams.Where(m => m.sName.ToLower() == "commiid").Select(m => m.sValue).FirstOrDefault();
                //var oCommI = oXI.BOI("XICommunicationI", CommIID);
                //if (oCommI != null && oCommI.Attributes.Count() > 0)
                //{
                //    var sSubject = oCommI.AttributeI("sHeader").sValue;
                //    var sBody = oCommI.AttributeI("sContent").sValue;
                //    //conversation should have keywords not individual
                //    //1click of keyword definitions getting from cache
                //    List<XIIBO> KeywordDefs = new List<XIIBO>();
                //    for (int i = 0; i < KeywordDefs.Count(); i++)
                //    {
                //        var bMatch = false;
                //        var sKeyword = KeywordDefs[i].AttributeI("sname").sValue;
                //        if (sSubject.Contains(sKeyword))
                //        {
                //            bMatch = true;
                //        }
                //        else if (sBody.Contains(sKeyword))
                //        {
                //            bMatch = true;
                //        }
                //        if (bMatch)
                //        {
                //            //create keyword instance
                //            var oKeyBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "KeywordI");
                //            XIIBO oKeyI = new XIIBO();
                //            oKeyI.BOD = oKeyBOD;
                //            oKeyI.SetAttribute("FKiKeywordDefID", KeywordDefs[i].AttributeI("id").sValue);
                //            oKeyI.SetAttribute("iStatus", "0");
                //            oKeyI.SetAttribute("FKiBODID", oCommI.iBODID.ToString());
                //            oKeyI.SetAttribute("FKiBOIID", CommIID);
                //            oKeyI.Save(oKeyI);
                //            //now need to execute process controller
                //        }
                //    }
                //}

            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                //SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            //return oCResult;
            return "Success";
        }

        public CResult MatchIdentifier(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            XIDefinitionBase oBase = new XIDefinitionBase();
            try
            {
                XIAPI oAPI = new XIAPI();
                oCR = oAPI.MatchString(oParams);
                if (oCR.bOK && oCR.oResult != null)
                {
                    oCResult.oResult = (bool)oCR.oResult;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
                else
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                //SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        public CResult FindConversation(string sUID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            XIDefinitionBase oBase = new XIDefinitionBase();
            try
            {
                XIIXI oXI = new XIIXI();
                var ConversationI = oXI.BOI("ConversationI", sUID);
                if (ConversationI != null && ConversationI.Attributes.Count() > 0)
                {
                    oCResult.oResult = true;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
                else
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                //SaveErrortoDB(oCResult);
            }            
            return oCResult;
        }

        public string RunScript(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                var iBODID = oParams.Where(m => m.sName.ToLower() == "iBODID").Select(m => m.sValue).FirstOrDefault();
                var iBOIID = oParams.Where(m => m.sName.ToLower() == "iBOIID").Select(m => m.sValue).FirstOrDefault();
                var ScriptID = oParams.Where(m => m.sName.ToLower() == "scriptid").Select(m => m.sValue).FirstOrDefault();
                iBODID = "4546";
                iBOIID = "430";
                ScriptID = "f8b8c9b7-32f0-46fa-8b79-6acd19a413e2";
                XIDScript oScriptD = new XIDScript();
                oScriptD = (XIDScript)oCache.GetObjectFromCache(XIConstant.CacheScript, null, ScriptID);
                var sScript = oScriptD.sScript;
                var sMethodName = oScriptD.sMethodName;
                XIInfraScript oScript = new XIInfraScript();
                MethodInfo methodInfoFromCache = (MethodInfo)oCache.GetFromCache("XIScript_" + ScriptID);
                object oResult = null;
                List<CNV> lParam = new List<CNV>();
                lParam.Add(new CNV { sName = "iBODID", sValue = iBODID });
                lParam.Add(new CNV { sName = "iBOIID", sValue = iBOIID });
                if (methodInfoFromCache != null)
                {
                    oResult = methodInfoFromCache.Invoke(null, new object[] { lParam });
                }
                else
                {
                    var info = oScript.WriteXIMethod(sScript, sMethodName);
                    if (info.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess && info.oResult != null)
                    {
                        MethodInfo methodInfo = (MethodInfo)info.oResult;
                        methodInfoFromCache = methodInfo;
                        oResult = methodInfoFromCache.Invoke(null, new object[] { lParam });
                        oCache.InsertIntoCache(methodInfoFromCache, "XIScript_" + ScriptID);
                    }
                }

            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                //SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            //return oCResult;
            return "Success";
        }
        public CResult Check_DocumentTree(string sName, string sParent)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {
                XIIXI oXI = new XIIXI();
                XIInfraCache oCache = new XIInfraCache();
                string sParentID = String.Empty;
                var oWhrParams = new List<CNV>();
                var oNode = oXI.BOI("XIDocumentTree_T", null, null, oWhrParams);
                XIDBO oXIDOCBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIDocumentTree_T");
                if (oNode != null && oNode.Attributes.Values.Count() > 0)
                {
                    sParentID = oNode.AttributeI("id").sValue;
                }
                else
                {
                    XIIBO oDocTreeI = new XIIBO();
                    oDocTreeI.BOD = oXIDOCBOD;
                    oDocTreeI.SetAttribute("sName", sName);
                    oDocTreeI.SetAttribute("sParentId", sParent);
                    oDocTreeI.SetAttribute("sTags", "");
                    oDocTreeI.SetAttribute("sPageNo", "");
                    oDocTreeI.SetAttribute("sFolderName", "");
                    //oDocTreeI.SetAttribute("spath", iDocID);
                    oCR = oDocTreeI.Save(oDocTreeI);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        oDocTreeI = (XIIBO)oCR.oResult;
                        sParentID = oDocTreeI.AttributeI("id").sValue;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    }
                    else
                    {
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    }
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                //SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }


        //Copy fiels source to target directory
        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        //Copy fiels source to target directory recursively
        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                string sOldPath = fi.FullName;
                string sNewPath = Path.Combine(target.FullName, fi.Name);
                //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                //Delete(Path.Combine(source.FullName, fi.Name));
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                if (diSourceSubDir.Name != "New folder")
                {
                    DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                    CopyAll(diSourceSubDir, nextTargetSubDir);
                    //Delete(diSourceSubDir.FullName);
                }
            }

        }

        //Delete Directory or File physically
        public static void Delete(string diSourceSubDir)
        {
            var Path = diSourceSubDir;
            FileAttributes attr = System.IO.File.GetAttributes(Path);

            if (attr.HasFlag(FileAttributes.Directory))
            {
                System.IO.Directory.Delete(Path);
            }
            else
            {
                System.IO.File.Delete(Path);
            }

        }

        //Update the document full path in Documents_T Table
        public void GetFiles(List<XIIBO> Files, string sOldFolder, string sNewFolder)
        {
            foreach (var items in Files)
            {
                if (items.SubChildI != null && items.SubChildI.Values.Count() > 0)
                {
                    var SUbs = items.SubChildI.Values.ToList().FirstOrDefault();
                    if (SUbs.Count() > 0)
                    {
                        GetFiles(SUbs, sOldFolder, sNewFolder);

                    }
                    if (items.AttributeI("stype").sValue == "20")
                    {
                        var DocID = items.AttributeI("spath").sValue;
                        if (!string.IsNullOrEmpty(DocID))
                        {
                            XIIXI oXI = new XIIXI();
                            var oBOI = oXI.BOI("Documents_T", DocID);
                            var sfullPath = oBOI.AttributeI("sfullPath").sValue;
                            sfullPath = sfullPath.Replace(@"\" + sOldFolder + @"\", @"\" + sNewFolder + @"\");
                            oBOI.SetAttribute("sfullpath", sfullPath);
                            var oCR = oBOI.Save(oBOI);
                        }
                    }
                }
                else
                {

                }
            }
        }

        [HttpPost]
        public ActionResult GetChildData(string nodeID, string nodeName, string iBuildingID, string sLoadType, string sSearchText, string sFilterType)
        {
            try
            {
                nodeName = nodeName.Replace(@"//", @"\");
                nodeName = nodeName.Replace("&amp;", "&");
                XIDStructure oStr = new XIDStructure();
                var Data = oStr.Get_SelfStructure(nodeName, nodeID, 1232, sSearchText, iBuildingID, sLoadType, sFilterType);
                var Res = (Dictionary<string, XIIBO>)Data.oResult;
                var Childs = Res.Values.ToList();
                // create the first list by using a specific "template" type.;

                // start adding "actual" values.
                List<CCIFNode> Datas = new List<CCIFNode>();
                List<Dictionary<string, string>> Nodes = new List<Dictionary<string, string>>();
                foreach (var items in Childs)
                {
                    CCIFNode CNode = new CCIFNode();
                    CNode.bHasChilds = items.bHasChilds;
                    Dictionary<string, CNV> Node = new Dictionary<string, CNV>();
                    Node["id"] = new CNV { sName = "id", sValue = items.AttributeI("id").sValue };
                    Node["sname"] = new CNV { sName = "sname", sValue = items.AttributeI("sname").sValue };
                    Node["stype"] = new CNV { sName = "stype", sValue = items.AttributeI("stype").sValue };
                    Node["iversionbatchid"] = new CNV { sName = "iversionbatchid", sValue = items.AttributeI("iversionbatchid").sValue };
                    Node["sparentid"] = new CNV { sName = "sparentid", sValue = items.AttributeI("sparentid").sValue };
                    Node["brestrict"] = new CNV { sName = "brestrict", sValue = items.AttributeI("brestrict").sValue };
                    Node["bproject"] = new CNV { sName = "bproject", sValue = items.AttributeI("bproject").sValue };
                    Node["sfoldername"] = new CNV { sName = "sfoldername", sValue = items.AttributeI("sfoldername").sValue };
                    Node["iapprovalstatus"] = new CNV { sName = "iapprovalstatus", sValue = items.AttributeI("iapprovalstatus").sValue };
                    Dictionary<string, List<CCIFNode>> SubChildI = new Dictionary<string, List<CCIFNode>>();
                    if (items.SubChildI != null && items.SubChildI.Count() > 0)
                    {
                        List<CCIFNode> Subs = new List<CCIFNode>();
                        foreach (var sub in items.SubChildI)
                        {
                            var sParent = "";
                            foreach (var BOIs in sub.Value.ToList())
                            {
                                sParent = BOIs.AttributeI("sparentid").sValue;
                                CCIFNode CSubNode = new CCIFNode();
                                CSubNode.bHasChilds = BOIs.bHasChilds;
                                Dictionary<string, CNV> SubNode = new Dictionary<string, CNV>();
                                SubNode["id"] = new CNV { sName = "id", sValue = BOIs.AttributeI("id").sValue };
                                SubNode["sname"] = new CNV { sName = "sname", sValue = BOIs.AttributeI("sname").sValue };
                                SubNode["stype"] = new CNV { sName = "stype", sValue = BOIs.AttributeI("stype").sValue };
                                SubNode["iversionbatchid"] = new CNV { sName = "iversionbatchid", sValue = BOIs.AttributeI("iversionbatchid").sValue };
                                SubNode["sparentid"] = new CNV { sName = "sparentid", sValue = BOIs.AttributeI("sparentid").sValue };
                                SubNode["brestrict"] = new CNV { sName = "brestrict", sValue = BOIs.AttributeI("brestrict").sValue };
                                SubNode["bproject"] = new CNV { sName = "bproject", sValue = BOIs.AttributeI("bproject").sValue };
                                SubNode["sfoldername"] = new CNV { sName = "sfoldername", sValue = BOIs.AttributeI("sfoldername").sValue };
                                SubNode["iapprovalstatus"] = new CNV { sName = "iapprovalstatus", sValue = BOIs.AttributeI("iapprovalstatus").sValue };
                                CSubNode.Attributes = SubNode;
                                Subs.Add(CSubNode);
                            }
                            //for (int i = 0; i < 1000; i++)
                            //{
                            //    CCIFNode CtestNode = new CCIFNode();
                            //    CNode.bHasChilds = false;
                            //    Dictionary<string, CNV> test = new Dictionary<string, CNV>();
                            //    test["id"] = new CNV { sName = "id", sValue = i.ToString() };
                            //    test["sname"] = new CNV { sName = "sname", sValue = i.ToString() };
                            //    test["stype"] = new CNV { sName = "stype", sValue = "" };
                            //    test["iversionbatchid"] = new CNV { sName = "iversionbatchid", sValue = "" };
                            //    test["sparentid"] = new CNV { sName = "sparentid", sValue = sParent };
                            //    test["brestrict"] = new CNV { sName = "brestrict", sValue = "" };
                            //    test["bproject"] = new CNV { sName = "bproject", sValue ="" };
                            //    CtestNode.Attributes = test;
                            //    Subs.Add(CtestNode);
                            //}
                            SubChildI[sub.Key] = Subs;
                        }

                    }

                    CNode.Attributes = Node;
                    CNode.SubChildI = SubChildI;
                    Datas.Add(CNode);
                }

                return Json(Datas, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), "");
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetProjects()
        {
            try
            {
                int iUserID = SessionManager.UserID;

                List<CNV> Data = new List<CNV>();
                XID1Click o1Click = new XID1Click();
                o1Click.BOID = 1237;
                o1Click.Query = "Select * from CR_UserProject_T where fkiuserid=" + iUserID;
                var Res = o1Click.OneClick_Run();
                if (Res != null && Res.Values.Count() > 0)
                {
                    var Projects = Res.Values.ToList();
                    foreach (var Pro in Projects)
                    {
                        var Name = Pro.AttributeI("sproject").sValue;
                        var Projs = Name.Split(',').ToList();
                        foreach (var item in Projs)
                        {
                            XIIXI oXI = new XIIXI();
                            var Proji = oXI.BOI("project", item);
                            if (Proji != null && Proji.Attributes.Count() > 0)
                            {
                                var name = Proji.AttributeI("sname").sValue;
                                var ID1 = Proji.AttributeI("fkinodeid").sValue;
                                Data.Add(new CNV { sName = name, sValue = ID1 });
                            }
                        }
                    }
                }
                return Json(Data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), "");
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion FolderOperations

        #region WaterMark

        [HttpPost]
        public ActionResult PrintPDF(int iDocID)
        {
            try
            {
                XIIXI oXI = new XIIXI();
                var oBOI = oXI.BOI("xidocumenttree", iDocID.ToString());
                if (oBOI != null && oBOI.Attributes.Count() > 0)
                {
                    var sFolderName = oBOI.AttributeI("sFolderName").sValue;
                    var sDocName = oBOI.AttributeI("sName").sValue + ".pdf";
                    var sVirtualDir = System.Configuration.ConfigurationManager.AppSettings["VirtualDirectoryPath"];
                    var sVirtualPath = @"~\" + sVirtualDir + @"\Createif\PDF\Client1\Project1\";
                    var sDocPath = Server.MapPath(sVirtualPath) + sFolderName + "\\" + sDocName;
                    var sNewFile = WriteToPdf(sDocPath, "CreateIF-Space", sDocName);
                    string NewPath = System.Configuration.ConfigurationManager.AppSettings["SharedPath"] + @"\Createif\PDF\Client1\Project1\" + "\\" + sFolderName + "\\" + sNewFile;
                    return Json(NewPath, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), "");
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }

        public string WriteToPdf(string sFilePath, string sWaterMark, string sFileName)
        {
            sWaterMark = "This Document is an Uncontrolled Copy @ and may not be the Latest Revision.@ Please check the Document Portal @ using the QR code below";
            sWaterMark = sWaterMark.Replace("@", "@" + System.Environment.NewLine);
            string line1 = "This Document is an Uncontrolled Copy";
            string line2 = "and may not be the Latest Revision.";
            string line3 = "Please check the Document Portal";
            string line4 = "using the QR code below";
            var sourceFile = Adding1stPageEmpty(sFilePath);
            PdfReader reader = new PdfReader(sourceFile);
            //Rectangle cropbox = reader.GetCropBox(0);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                iTextSharp.text.pdf.BarcodeQRCode qrcode = new BarcodeQRCode("http://portal.create-if.space", 200, 200, null);
                iTextSharp.text.Image img1 = qrcode.GetImage();
                //
                // PDFStamper is the class we use from iTextSharp to alter an existing PDF.
                //
                PdfStamper pdfStamper = new PdfStamper(reader, memoryStream);

                for (int i = 1; i <= reader.NumberOfPages; i++) // Must start at 1 because 0 is not an actual page.
                {
                    //
                    // If you ask for the page size with the method getPageSize(), you always get a
                    // Rectangle object without rotation (rot. 0 degrees)—in other words, the paper size
                    // without orientation. That’s fine if that’s what you’re expecting; but if you reuse
                    // the page, you need to know its orientation. You can ask for it separately with
                    // getPageRotation(), or you can use getPageSizeWithRotation(). - (Manning Java iText Book)
                    //   
                    //
                    iTextSharp.text.Rectangle pageSize = reader.GetPageSizeWithRotation(i);

                    //
                    // Gets the content ABOVE the PDF, Another option is GetUnderContent(...)  
                    // which will place the text below the PDF content. 
                    //
                    PdfContentByte pdfPageContents = pdfStamper.GetUnderContent(i);
                    pdfPageContents.BeginText(); // Start working with text.

                    //
                    // Create a font to work with 
                    //
                    BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, Encoding.ASCII.EncodingName, false);
                    pdfPageContents.SetFontAndSize(baseFont, 20); // 40 point font
                    pdfPageContents.SetRGBColorFill(240, 240, 240); // Sets the color of the font, Light Gray in this instance


                    //
                    // Angle of the text. This will give us the angle so we can angle the text diagonally 
                    // from the bottom left corner to the top right corner through the use of simple trigonometry. 
                    //
                    float textAngle = (float)FooTheoryMath.GetHypotenuseAngleInDegreesFrom(pageSize.Height, pageSize.Width);

                    //
                    // Note: The x,y of the Pdf Matrix is from bottom left corner. 
                    // This command tells iTextSharp to write the text at a certain location with a certain angle.
                    // Again, this will angle the text from bottom left corner to top right corner and it will 
                    // place the text in the middle of the page. 
                    //
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, line1,
                                                    (pageSize.Width / 2) + 10,
                                                    (pageSize.Height / 2) + 10,
                                                    textAngle);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, line2,
                                                   (pageSize.Width / 2) + 30,
                                                    (pageSize.Height / 2) - 0,
                                                    textAngle);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, line3,
                                                    (pageSize.Width / 2) + 50,
                                                    (pageSize.Height / 2) - 10,
                                                    textAngle);
                    pdfPageContents.ShowTextAligned(PdfContentByte.ALIGN_CENTER, line4,
                                                    (pageSize.Width / 2) + 70,
                                                    (pageSize.Height / 2) - 20,
                                                    textAngle);
                    //img1.setAbsolutePosition(0f, 0f);
                    Rectangle currentPageRectangle = reader.GetPageSizeWithRotation(i);
                    if (currentPageRectangle.Width > currentPageRectangle.Height)
                    {
                        //page is landscape
                        img1.SetAbsolutePosition(1091, 10);
                    }
                    else
                    {
                        //page is portrait
                        img1.SetAbsolutePosition(500, 15);
                    }

                    img1.ScalePercent(20f);
                    img1.Alignment = Element.ALIGN_CENTER;
                    //img1.ScaleToFit(180f, 250f);
                    pdfPageContents.AddImage(img1);
                    pdfPageContents.EndText(); // Done working with text
                }
                pdfStamper.FormFlattening = true; // enable this if you want the PDF flattened. 
                pdfStamper.Close(); // Always close the stamper or you'll have a 0 byte stream. 
                                    //DOwnload PDF
                var sNewFilePath = DownloadPDF(memoryStream, sFileName, sFilePath);
                return sNewFilePath;
                //return Json(fileresult, JsonRequestBehavior.AllowGet);
            }
        }
        public static class FooTheoryMath
        {
            public static double GetHypotenuseAngleInDegreesFrom(double opposite, double adjacent)
            {
                //http://www.regentsprep.org/Regents/Math/rtritrig/LtrigA.htm
                // Tan <angle> = opposite/adjacent
                // Math.Atan2: http://msdn.microsoft.com/en-us/library/system.math.atan2(VS.80).aspx 

                double radians = Math.Atan2(opposite, adjacent); // Get Radians for Atan2
                double angle = radians * (180 / Math.PI); // Change back to degrees
                return angle;
            }
        }
        public string Adding1stPageEmpty(string src)
        {
            string outputFileName = Path.GetTempFileName();
            PdfReader reader = new PdfReader(src);
            PdfStamper stamper = new PdfStamper(reader, new FileStream(outputFileName, FileMode.Create));
            int total = reader.NumberOfPages + 1;
            for (int pageNumber = total; pageNumber > 0; pageNumber--)
            {
                if (pageNumber == 1)
                {
                    stamper.InsertPage(pageNumber, PageSize.A4);
                }
            }
            stamper.Close();
            reader.Close();
            return outputFileName;
        }

        public string DownloadPDF(MemoryStream memoryStream, string sFileName, string sFilePath)
        {
            var fileresult = new FileContentResult((byte[])(memoryStream.ToArray()), "application/pdf");
            var NewFile = "Print-" + sFileName;
            fileresult.FileDownloadName = "Print-" + sFileName;
            sFilePath = sFilePath.Replace(sFileName, "Print-" + sFileName);
            System.IO.File.WriteAllBytes(sFilePath, fileresult.FileContents);

            //var binary = fileresult.BuildPdf(ControllerContext);
            //System.IO.File.WriteAllBytes(@"c:\foobar.pdf", binary);
            return NewFile;
        }

        #endregion WaterMark
        public ActionResult GenerateQuotes()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadFile(int FKiOriginID, int FKiSourceID, int FKiClassID, HttpPostedFileBase file)
        {
            XIInfraCache oCache = new XIInfraCache();
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            List<string> Info = new List<string>();
            try
            {
                oTrace.oTrace.Add(oCR.oTrace);
                if (file.ContentLength > 0)
                {
                    List<string> csvData = new List<string>();
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(file.InputStream))
                    {
                        while (!reader.EndOfStream)
                        {
                            csvData.Add(reader.ReadLine());
                        }
                    }
                    XIIXI oXI = new XIIXI();
                    XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "xileadimport", null);
                    XIIBO oBOI = new XIIBO();
                    oBOI.BOD = oBOD;
                    oBOI.SetAttribute("sLeadData", string.Join("", csvData));
                    oBOI.SetAttribute("FKiClassID", FKiClassID.ToString());
                    oBOI.SetAttribute("FKiOriginID", FKiOriginID.ToString());
                    oBOI.SetAttribute("FKiSourceID", FKiSourceID.ToString());
                    var res = oBOI.Save(oBOI);
                    string sSessionID = HttpContext.Session.SessionID;
                    var resp = GetRecordByID<CResult>("api/XMLHandler/PostMHXML/" + ((XIIBO)res.oResult).AttributeI("id").iValue + "/'" + sSessionID + "'/0/null");
                    XIIBO oQuoteI = new XIIBO();
                    QueryEngine oQE = new QueryEngine();
                    string sWhereCondition = "FKiQSInstanceID=" + resp.oResult.ToString() + "," + XIConstant.Key_XIDeleted + "=0";
                    oCResult = oQE.Execute_QueryEngine("Aggregations", "sGUID,ID,sInsurer,iQuoteStatus,rCompulsoryExcess, rVoluntaryExcess, rTotalExcess, rMonthlyPrice, rMonthlyTotal, zDefaultDeposit, rFinalQuote, bIsFlood, bIsApplyFlood", sWhereCondition);
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return PartialView("GenerateQuotes", ((Dictionary<string, XIIBO>)oCResult.oResult).Select(x => x.Value).ToList());
        }
        public static T GetRecordByID<T>(string path)
        {
            string token = null;
            var client = new RestSharp.RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["APIBaseUrl"].ToString());
            var req = new RestSharp.RestRequest(path, RestSharp.Method.GET);
            req.AddHeader(System.Net.HttpRequestHeader.Authorization.ToString(), string.Format("Bearer {0}", token));
            req.Timeout = 300000;
            var res = client.Execute(req).Content;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            T collection = serializer.Deserialize<T>(res);
            return collection;
        }
    }
}