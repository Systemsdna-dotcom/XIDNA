using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using Mvc.Mailer;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Configuration;
using Microsoft.AspNet.Identity.EntityFramework;
using XIDNA.Models;
using XIDNA.Repository;
using XIDNA.ViewModels;
using XIDNA.Common;
using System.Net.Mail;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Text;
using XIDNA.Mailer;
using System.Data.SqlClient;
using System.Data;
using XICore;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text.html.simpleparser;
using System.Web.Services.Description;
using XISystem;
//using iTextSharp.text.pdf.events;
//using iTextSharp.tool.xml;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using System.Net;

namespace XIDNA.Controllers
{
    [Authorize]
    public class ContentController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IContentRepository ContentRepository;
        private readonly IXiLinkRepository XiLinkRepository;
        public ContentController()
            : this(new ContentRepository(), new XiLinkRepository())
        { }

        public ContentController(IContentRepository ContentRepository, IXiLinkRepository XiLinkRepository)
        {
            this.ContentRepository = ContentRepository;
            this.XiLinkRepository = XiLinkRepository;
        }
        XIInfraUsers oUser = new XIInfraUsers();
        CommonRepository Common = new CommonRepository();
        CXiAPI oXiAPI = new CXiAPI();
        XIInfraEmail oEmail = new XIInfraEmail();
        XIDStructure oXIDStructure = new XIDStructure();
        XIInfraScript oIScript = new XIInfraScript();
        XIIXI oIXI = new XIIXI();
        XIInfraCache oCache = new XIInfraCache();
        XIContentEditors oXIContent = new XIContentEditors();
        XIInfraDocs oXIDocs = new XIInfraDocs();
        //
        // GET: /Content/
        public ActionResult Index()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                //Get the public methods of model class
                var MethodsList = ShowMethods(typeof(Organizations));
                List<VMDropDown> ModelPropertiesList = new List<VMDropDown>();
                foreach (var method in MethodsList)
                {
                    VMDropDown ModelProperties = new VMDropDown();
                    if (method[1].Contains("_"))
                    {
                        var iPropertyType = method[0];
                        if (iPropertyType.ToLower() != "Void".ToLower())
                        {
                            var sPropertyName = method[1].Split('_')[1];
                            ModelProperties.text = sPropertyName;
                            ModelProperties.Type = method[3];
                            ModelPropertiesList.Add(ModelProperties);
                        }
                    }
                }

                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                ContentEditors model = new ContentEditors();
                ModelDbContext dbContext = new ModelDbContext();
                var editor = dbContext.ContentEditors.ToList();
                Dictionary<int, string> parentList = new Dictionary<int, string>();
                foreach (var item in editor)
                {
                    parentList.Add(item.ID, item.Name);
                }
                model.ddlParentList = parentList;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                var fkiApplicationID = oUser.FKiApplicationID;
                model.ContentList = ContentRepository.LeadFieldsList(0, sDatabase);
                model.TypeList = ContentRepository.TypesList(sDatabase);
                model.Images = ContentRepository.GetOrgImages(OrgID, iUserID, sOrgName, sDatabase);
                model.BOList = Common.GetBOsDDL(sDatabase);
                //model.BOList = ContentRepository.GetBOsList(sDatabase);
                ViewBag.ModelList = ModelPropertiesList;
                List<VMDropDown> ModelList = new List<VMDropDown>();
                VMDropDown ModelName = new VMDropDown();
                ModelName.text = "Organizations";
                ModelName.Type = "";
                ModelList.Add(ModelName);
                ViewBag.ModelName = ModelList;
                if (fkiApplicationID == 0)
                {
                    model.ddlApplications = Common.GetApplicationsDDL();
                }
                model.FKiApplicationID = fkiApplicationID;
                return View(model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }


        public ActionResult GetUsersList(int id)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                int Type = 0;
                var res = ContentRepository.GetUsersList(Type, sDatabase, OrgID, iUserID, sOrgName);
                ViewBag.ContentID = id;
                return View("UsersList", res);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public List<string[]> ShowMethods(Type type)
        {
            //Get the public methods of model class
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                List<string[]> strarr = new List<string[]>(); var ClassName = "";
                //foreach (MethodInfo method in typeof(ContentEditors).GetMethods())
                foreach (var method in type.GetMethods())
                {
                    var parameters = method.GetParameters();
                    var parameterDescriptions = string.Join
                        (", ", method.GetParameters()
                                     .Select(x => x.ParameterType + " " + x.Name)
                                     .ToArray());
                    var retuntype = method.ReturnType;
                    var methodname = method.Name;
                    var description = parameterDescriptions;
                    if (retuntype.Name.ToString() == "List`1")
                    {
                        ClassName = method.ReturnType.UnderlyingSystemType.GenericTypeArguments[0].Name;
                    }

                    string[] arr = new string[] { retuntype.Name, methodname, parameterDescriptions, ClassName };

                    strarr.Add(arr);
                }
                return strarr;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetContentList(string sType)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                ViewBag.sType = sType;
                return View("ContentList", OrgID);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult GetBoMailFields(int BOID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {

                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                var res = ContentRepository.LeadFieldsList(BOID, sDatabase);
                //Get the public methods of model class
                var MethodsList = ShowMethods(typeof(ContentEditors));
                List<string> PropertList = new List<string>();
                List<VMDropDown> ModelPropertiesList = new List<VMDropDown>();
                foreach (var method in MethodsList)
                {
                    VMDropDown ModelProperties = new VMDropDown();
                    if (method[1].Contains("_"))
                    {
                        var iPropertyType = method[0];
                        if (iPropertyType.ToLower() != "Void".ToLower())
                        {
                            var sPropertyName = method[1].Split('_')[1];
                            // ModelProperties.Type = method[0];
                            ModelProperties.text = sPropertyName;
                            ModelProperties.Type = method[3];
                            ModelPropertiesList.Add(ModelProperties);
                        }

                    }
                }
                return Json(new { result = res, ModelList = ModelPropertiesList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpPost]
        public ActionResult GetListOfTemplates(jQueryDataTableParamModel param, int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = ContentRepository.GetListOfTemplates(param, OrgID, iUserID, sOrgName, sDatabase);
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
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }
        public ActionResult GetContentByID(int ID, string str)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                ContentEditors model = ContentRepository.GetContent(ID, sDatabase, iUserID, sOrgName);
                if (model.Content != null)
                {
                    string Content = model.Content;
                    //Content = Content.Replace("<p>", "");
                    //Content = Content.Replace("</p>", "");
                    model.Content = Content;
                }
                model.str = str;
                model.TypeList = ContentRepository.TypesList(sDatabase);
                model.ContentList = ContentRepository.LeadFieldsList(model.BO, sDatabase);
                ViewBag.str = str;
                model.Images = ContentRepository.GetOrgImages(OrgID, iUserID, sOrgName, sDatabase);
                model.BOList = Common.GetBOsDDL(sDatabase);
                model.IOServerList = Common.GetIOServerDDL(sDatabase);
                //model.BOList = ContentRepository.GetBOsList(sDatabase);
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                var fkiApplicationID = oUser.FKiApplicationID;
                if (fkiApplicationID == 0)
                {
                    model.ddlApplications = Common.GetApplicationsDDL();
                }
                ModelDbContext dbContext = new ModelDbContext();
                var editor = dbContext.ContentEditors.ToList();
                Dictionary<int, string> parentList = new Dictionary<int, string>();
                foreach (var item in editor)
                {
                    parentList.Add(item.ID, item.Name);
                }
                model.ddlParentList = parentList;
                model.FKiApplicationID = fkiApplicationID;
                //Get the public methods of model class
                var MethodsList = ShowMethods(typeof(Organizations));
                List<string> PropertList = new List<string>();
                List<VMDropDown> ModelPropertiesList = new List<VMDropDown>();
                foreach (var method in MethodsList)
                {
                    VMDropDown ModelProperties = new VMDropDown();
                    if (method[1].Contains("_"))
                    {
                        var iPropertyType = method[0];
                        if (iPropertyType.ToLower() != "Void".ToLower())
                        {
                            var sPropertyName = method[1].Split('_')[1];
                            // ModelProperties.Type = method[0];
                            ModelProperties.text = sPropertyName;
                            ModelProperties.Type = method[3];
                            ModelPropertiesList.Add(ModelProperties);
                        }

                    }
                }
                ViewBag.ModelList = ModelPropertiesList;
                List<VMDropDown> ModelList = new List<VMDropDown>();
                VMDropDown ModelName = new VMDropDown();
                ModelName.text = "Organizations";
                ModelName.Type = "Organizations";
                ModelList.Add(ModelName);
                //ModelName = new VMDropDown();
                //ModelName.text = "ContentEditors";
                //ModelName.Type = "ContentEditors";
                //ModelList.Add(ModelName);
                ViewBag.ModelName = ModelList;
                return View("CreateTemplate", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpGet]
        public ActionResult GetPreviewContent(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                ContentEditors res = ContentRepository.GetContent(ID, sDatabase, iUserID, sOrgName);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }


        [HttpGet]
        public ActionResult GetContentPreview(int? LeadID, int ContentID, string Users)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                var res = ContentRepository.GetContent(ContentID, sDatabase, iUserID, sOrgName);
                string field = res.Content;
                var GetFieldsRgx = @"(?<=\{{)[^}]*(?=\}})";
                List<string> Columns = new List<string>();
                foreach (Match m in Regex.Matches(field, GetFieldsRgx))
                {
                    Columns.Add(m.Value);
                }
                var model = ContentRepository.GetLead(LeadID, Users, Columns, sDatabase, iUserID, sOrgName);
                model[0].Columns = Columns;
                model[0].Content = field;
                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult GenerateTemplateValues(int ID, int iTemplateID, string sBOName)
        {
            int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName; string sDatabase = SessionManager.CoreDatabase;
            oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int iOrgID = oUser.FKiOrganisationID;
            string sOrgDB = SessionManager.OrgDatabase; int iStartIndex = 0;
            XIIXI oIXI = new XIIXI();
            XIIBO oBII = new XIIBO();
            oXIContent.sOrgDatabase = sOrgDB;
            var boi = oIXI.BOI(sBOName, Convert.ToString(ID));
            var oContentDef = oCache.GetObjectFromCache(XIConstant.CacheTemplate, null, iTemplateID.ToString()); //oDXI.Get_ContentDefinition(iTemplateID);
            if (oContentDef != null)
            {
                XIContentEditors oDocumentContent = (XIContentEditors)oContentDef;
                string sFinalString = oXIContent.CheckStructureNotation(oDocumentContent.Content, iStartIndex);
                string sInstanceName = "";
                while (sFinalString.Length > 0)
                {
                    sInstanceName = oXIContent.CheckModelHtmlContentNoation(sFinalString);
                    while (sInstanceName.Length > 0)
                    {
                        string sName = sInstanceName.Replace("{{", "").Replace("}}", "");
                        // var sBODataSource = oXiAPI.GetBODataSource(12, oUser.FKiOrganisationID, sDatabase, sOrgDB);
                        string sID = Convert.ToString(iTemplateID);
                        // iDocumentID = Convert.ToInt32(sID);
                        XIIBO oBOI = new XIIBO();
                        XIDXI oXID = new XIDXI();
                        XIDBO oBOD = (XIDBO)oXID.Get_BODefinition("TemplateParameters_T").oResult;
                        oBOI.BOD = oBOD;
                        oBOI.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = null, bDirty = true };
                        oBOI.Attributes["FKiTemplateID"] = new XIIAttribute { sName = "FKiTemplateID", sValue = sID, bDirty = true };
                        oBOI.Attributes["TemplateParameterName"] = new XIIAttribute { sName = "TemplateParameterName", sValue = sName, bDirty = true };

                        var Response = oBOI.Save(oBOI);//to save Template related parameters


                        oBOI = new XIIBO();
                        oXID = new XIDXI();
                        oBOD = (XIDBO)oXID.Get_BODefinition("DocumentGeneration").oResult;
                        oBOI.BOD = oBOD;
                        oBOI.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = null, bDirty = true };
                        oBOI.Attributes["DocumentType"] = new XIIAttribute { sName = "DocumentType", sValue = oDocumentContent.Name, bDirty = true };
                        oBOI.Attributes["DocumentTemplateID"] = new XIIAttribute { sName = "DocumentTemplateID", sValue = Convert.ToString(oDocumentContent.ID), bDirty = true };
                        oBOI.Attributes["DocumentStatus"] = new XIIAttribute { sName = "DocumentStatus", sValue = "10", bDirty = true };

                        //XIIXI oXII = new XIIXI();
                        //oXII.sCoreDatabase = sDatabase;
                        //oXII.sOrgDatabase = sOrgDB;
                        Response = oBOI.Save(oBOI);//to save document with status
                        XIIBO oBOInstance = new XIIBO();
                        if (Response.bOK && Response.oResult != null)
                        {
                            oBOInstance = (XIIBO)Response.oResult;
                        }
                        sID = oBOInstance.Attributes.Where(x => x.Key.ToLower() == oBOI.BOD.sPrimaryKey.ToLower()).Select(x => x.Value.sValue).FirstOrDefault();
                        // iDocumentID = Convert.ToInt32(sID);
                        oBOI = new XIIBO();
                        oXID = new XIDXI();
                        oBOD = (XIDBO)oXID.Get_BODefinition("TemplateInstances").oResult;
                        oBOI.BOD = oBOD;
                        oBOI.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = null, bDirty = true };
                        oBOI.Attributes["FKiDocumentID"] = new XIIAttribute { sName = "FKiDocumentID", sValue = sID, bDirty = true };
                        oBOI.Attributes["TemplateParameterName"] = new XIIAttribute { sName = "TemplateParameterName", sValue = sName, bDirty = true };
                        oBOI.Attributes["TemplateParameterValue"] = new XIIAttribute { sName = "TemplateParameterValue", sValue = boi.Attributes.Where(x => x.Key.ToLower() == sName.ToLower()).Select(x => x.Value.sValue).FirstOrDefault(), bDirty = true };

                        //oXII = new XIIXI();
                        //oXII.sCoreDatabase = sDatabase;
                        //oXII.sOrgDatabase = sOrgDB;
                        Response = oBOI.Save(oBOI);//to save Template related parameters with instances

                        oDocumentContent.Content = oDocumentContent.Content.Replace(sInstanceName, sName);
                        sFinalString = sFinalString.Replace(sInstanceName, sName);
                        sInstanceName = oXIContent.CheckModelHtmlContentNoation(sFinalString);
                    }
                    iStartIndex = oDocumentContent.Content.IndexOf(sFinalString) + sFinalString.Length;
                    string sReplacedTableRow = sFinalString.Replace("[[", "").Replace("]]", "");
                    oDocumentContent.Content = oDocumentContent.Content.Replace(sFinalString, sReplacedTableRow);
                    sFinalString = oXIContent.CheckStructureNotation(oDocumentContent.Content, iStartIndex);
                }
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult PostContent(ContentEditors model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                if (model.Content != null || model.SMSContent != null)
                {
                    model.OrganizationID = OrgID;
                    string HtmlDecode = WebUtility.HtmlDecode(model.Content);
                    model.Content = HtmlDecode;
                    var res = ContentRepository.PostContent(model, iUserID, sOrgName, sDatabase);
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Please Enter Template Content Before Saving", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Editor(string res)
        {
            ViewBag.feilds = res;
            return View("Index");
        }
        public ActionResult GetdropdownforContent()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int Type = 0;
                VMContentEditors content = new VMContentEditors();
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                List<VMDropdowns> list = ContentRepository.ContentList(sDatabase, OrgID, iUserID, sOrgName);
                content.ContentList = list;
                var res = ContentRepository.GetUsersList(Type, sDatabase, OrgID, iUserID, sOrgName);
                content.Data = res;
                return View("UsersList", content);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }
        //Getting The Template Values

        public ActionResult Contentdropdown(int Category)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                List<VMDropDown> list = ContentRepository.Contentdropdown(Category, sDatabase, OrgID, iUserID, sOrgName);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Save(String Users, int Type, int ID, HttpPostedFileBase UploadFile, string Cc, Outbounds Content)
        {
            string sDatabase = SessionManager.CoreDatabase;
            int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
            oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
            var res = ContentRepository.GetContent(ID, sDatabase, iUserID, sOrgName);
            var GetFieldsRgx = @"(?<=\{{)[^}]*(?=\}})";
            //string field = res.Content;            
            //var today = System.DateTime.Now;
            //var tomorrow = System.DateTime.Now.AddDays(1);
            //var yesterday = System.DateTime.Now.AddDays(-1);           
            List<string> Columns = new List<string>();
            foreach (Match m in Regex.Matches(res.Content, GetFieldsRgx))
            {
                Columns.Add(m.Value);
            }
            var leadsdata = ContentRepository.GetLeadsData(Users, Columns, sDatabase, iUserID, sOrgName);
            string[] s = Users.Split(',');
            //path to save the attachment in the project folder
            string physicalPath = "", str = "", sFileName = "";
            if (UploadFile != null)
            {
                physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                str = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\Attachment";
                logger.Info(str);
                UploadFile.SaveAs(str + "\\" + "Attachment" + ".pdf");
                sFileName = "Attachment" + ".pdf";
                Content.Attachment = sFileName;
            }
            for (int i = 0; i < s.Count(); i++)
            {
                Content.Cc = Cc;
                Content.Users = Convert.ToInt32(s[i]);
                var outbounds = ContentRepository.SaveOutbounds(Content, sDatabase, OrgID, iUserID, sOrgName);
            }

            try
            {
                if (Type == 1)
                {
                    //Getting the Server Details from the Database
                    var sDetails = ContentRepository.ServerDetails(Type, OrgID, sDatabase);
                    string usern = "", pass = "", sender = "", security = "", serverName = "";
                    int port = 0;
                    foreach (var items in sDetails)
                    {
                        usern = items.UserName;
                        pass = items.Password;
                        sender = items.FromAddress;
                        serverName = items.ServerName;
                        port = items.Port;
                        security = items.Security;
                    }
                    string username = HttpUtility.UrlEncode(usern);
                    string password = HttpUtility.UrlEncode(pass);
                    string emailSubject = "Welcome Email";
                    string messageBody = "";
                    foreach (var user in leadsdata)
                    {
                        string field = res.Content;
                        int n = 0;
                        foreach (var items in user.Data)
                        {
                            string Con = "";
                            foreach (var item in items)
                            {
                                if (item != null)
                                {
                                    Con = Con + item + ",  ";
                                }
                            }
                            Con = Con.Substring(0, Con.Length - 2);
                            Con = Con.TrimEnd(',');
                            field = field.Replace("{{" + Columns[n] + "}}", Con);
                            n++;

                            //field = field.Replace("{{" + Columns[n] + "}}", items);
                            //n++;
                        }
                        //string DateFields = field.Replace("((Today))", today.ToString()).Replace("((Tomorrow))", tomorrow.ToString()).Replace("((Yesterday))", yesterday.ToString());
                        messageBody = field;
                        List<string> ContentIds = new List<string>();
                        List<string> Paths = new List<string>();
                        string pattern = "(?<=src=\")[^,]+?(?=\")";
                        string input = messageBody;
                        if (messageBody.IndexOf("src=") >= 0)
                        {
                            int i = 1;
                            foreach (Match m in Regex.Matches(input, pattern))
                            {
                                physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                                Paths.Add(physicalPath + m.Value);
                                ContentIds.Add("ContentID" + i);
                                messageBody = messageBody.Replace(m.Value, "cid:ContentID" + i);
                                i++;
                            }
                        }
                        MailMessage msg = new MailMessage();
                        //checking that if there exist a Cc                                                     
                        if (Cc != "" && Cc != String.Empty && Cc != null)
                        {
                            msg.CC.Add(Cc);
                        }
                        //checking that if there exists BCC
                        //if (BCC != "" && BCC != null)
                        //{
                        //    msg.Bcc.Add(BCC);
                        //}
                        msg.To.Add(user.Email);
                        msg.From = new MailAddress(sender);
                        msg.Subject = "Welcome Mail";
                        string html = @"<html><body>" + messageBody + "</body></html>";
                        AlternateView altView = AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html);
                        for (int j = 0; j < Paths.Count(); j++)
                        {
                            string Path = Paths[j];
                            string CntID = ContentIds[j];
                            logger.Info(Path);
                            LinkedResource yourPictureRes = new LinkedResource(Path, MediaTypeNames.Image.Jpeg);
                            yourPictureRes.ContentId = CntID;
                            altView.LinkedResources.Add(yourPictureRes);
                        }
                        msg.AlternateViews.Add(altView);
                        string Attach = "";
                        if (UploadFile != null)
                        {
                            //Getting Attachment file
                            var att = ContentRepository.GetAttachment(154, sDatabase, iUserID, sOrgName);
                            physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                            str = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\Attachment";
                            sFileName = (str + "\\" + "Attachment" + ".pdf");

                            Attach = sFileName;
                            msg.Attachments.Add(new Attachment(Attach));
                        }
                        msg.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = serverName;
                        smtp.Port = port;
                        //for gmail
                        smtp.EnableSsl = false;
                        smtp.Credentials = new System.Net.NetworkCredential(usern, pass);
                        smtp.Send(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                logger.Error(ex);
            }

            return null;
        }
        [HttpPost]
        public ActionResult IsExistsTitle(int Category, string Name, int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                if (Category > 0)
                {
                    int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                    oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                    return ContentRepository.IsExistsTitle(Name, ID, Category, OrgID, iUserID, sOrgName, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
                         : Json(false, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return PartialView();
            }
        }

        public ActionResult GetListOfLeads(int Type)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                var Leads = ContentRepository.GetUsersList(Type, sDatabase, OrgID, iUserID, sOrgName);
                return PartialView("_ListOfLeads", Leads);
            }
            catch (Exception ex)
            {
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return PartialView();
            }
        }

        public ActionResult GetModelListFields(string ModelPropertListName)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                //Get modelclass methods
                var MethodsList = ShowMethods(typeof(cAggregations));
                List<string> PropertList = new List<string>();
                List<VMDropDown> ModelPropertiesList = new List<VMDropDown>();
                foreach (var method in MethodsList)
                {
                    VMDropDown ModelProperties = new VMDropDown();
                    if (method[1].Contains("_"))
                    {
                        var iPropertyType = method[0];
                        if (iPropertyType.ToLower() != "Void".ToLower())
                        {
                            var sPropertyName = method[1].Split('_')[1];
                            // ModelProperties.Type = method[0];
                            ModelProperties.text = sPropertyName;
                            ModelProperties.Type = method[3];
                            ModelPropertiesList.Add(ModelProperties);
                        }

                    }
                }

                return Json(ModelPropertiesList, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }
        public ActionResult GetQuotesList(int ContentID)
        {
            //Get Template Result
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                ContentEditors model = ContentRepository.GetContent(ContentID, sDatabase, iUserID, sOrgName);//Template Model
                object Orgmodel = ContentRepository.GetOrganizations(ContentID, sDatabase, iUserID, sOrgName);//Organizations Model(sample)

                return GetDynamicTemplate(model, Orgmodel);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        private ActionResult GetDynamicTemplate(ContentEditors model, object Orgmodel)
        {
            int iStartIndex = 0;
            string sFinalTableString = "";
            HtmlContentReplace HtmlContentObj = new HtmlContentReplace();
            var sFinalString = HtmlContentObj.CheckHtmlContentTable(model.Content, iStartIndex);//Check if htmlcontent has List of class(table)

            while (sFinalString.Length > 0)
            {
                sFinalTableString = HtmlContentObj.ReplaceDynamicData(Orgmodel, sFinalString, model.Content);//if htmlcontent has collection object then the collection data will be append
                if (sFinalTableString != null)
                {
                    model.Content = sFinalTableString;
                }
                else
                {
                    iStartIndex = model.Content.IndexOf(sFinalString) + sFinalString.Length;
                    string sReplacedTableRow = sFinalString.Replace("{{", "").Replace("}}", "");
                    model.Content = model.Content.Replace(sFinalString, sReplacedTableRow);
                }
                sFinalString = HtmlContentObj.CheckHtmlContentTable(model.Content, iStartIndex);//Check if htmlcontent has another List of class(table)
            }
            var sModelContentString = HtmlContentObj.CheckModelContent(model.Content);//Check if htmlcontent has model properties
            while (sModelContentString.Length > 0)
            {
                model.Content = HtmlContentObj.ReplaceDynamicModelData(Orgmodel, sModelContentString, model.Content);//Replace model properties with model values
                sModelContentString = HtmlContentObj.CheckModelContent(model.Content);//Check if htmlcontent has another model properties
            }
            //ViewBag.ModelList = ModelPropertiesList;
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #region DocumentGenerationUsingModels
        public class MethodInfoList
        {
            public MethodInfo oMethoInfo { get; set; }
            public string sHtmlString { get; set; }
        }
        public class HtmlContentReplace
        {
            readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            CommonRepository Common = new CommonRepository();
            public string ReplaceDynamicData(object model, string sFinalString, string sTotalHtmlString)
            {
                try
                {
                    List<MethodInfoList> oMethodInfoObj = new List<MethodInfoList>();
                    string sRowString = sFinalString;
                    string str = "";
                    Regex regexb = new Regex("{{");
                    string comment = regexb.Replace(sFinalString, "");
                    regexb = new Regex("}}");
                    comment = regexb.Replace(comment, "");
                    sFinalString = comment;
                    String pattern = @"<td>";
                    string[] StrSplit = Regex.Split(sFinalString, pattern);
                    List<string> SplitStr = new List<string>(); int i = 0;
                    string sModelCollectionName = "";
                    foreach (var sStr in StrSplit)
                    {
                        if (i != 0)
                        {
                            if (sStr.Contains("</td>"))
                            {
                                String Splitpattern = @"</td>";
                                string[] sArray = Regex.Split(sStr, Splitpattern);
                                if (sArray[0].Contains("."))
                                {
                                    string[] ParamName = sArray[0].Split('.');
                                    if (ParamName.Count() > 1)
                                    {
                                        sModelCollectionName = ParamName[0];
                                    }
                                }
                            }
                        }
                        i++;
                    }
                    i = 0;
                    sModelCollectionName = "get_" + sModelCollectionName;
                    var test = ((object)model).GetType().GetMethods();
                    var method1 = ((object)model).GetType().GetMethod(sModelCollectionName);//get methods of model class
                    if (method1 != null)
                    {

                        var MethodResult = method1.Invoke(model, null);

                        foreach (var sStr in StrSplit)
                        {
                            int iLoopcount = 0;
                            foreach (var item in (dynamic)MethodResult)
                            {
                                MethodInfoList oMethodObj = new MethodInfoList();
                                if (i != 0)
                                {
                                    if (sStr.Contains("</td>"))
                                    {

                                        String Splitpattern = @"</td>";
                                        string[] sArray = Regex.Split(sStr, Splitpattern);
                                        SplitStr.Add(Regex.Split(sStr, Splitpattern)[0]);
                                        string sListParamName = Regex.Split(sStr, Splitpattern)[0];
                                        if (sListParamName.Contains("."))
                                        {
                                            string[] ParamName = sListParamName.Split('.');
                                            if (ParamName.Count() > 1)
                                            {
                                                var paramtype = ParamName[1];
                                                var GetMethodName = "get_" + paramtype;
                                                if (iLoopcount == 0)
                                                {
                                                    oMethodObj.sHtmlString = sListParamName;
                                                    var submethod = ((object)item).GetType().GetMethod(GetMethodName);//get methods of collection object of model class
                                                    oMethodObj.oMethoInfo = submethod;
                                                    oMethodInfoObj.Add(oMethodObj);
                                                }
                                            }
                                        }
                                    }
                                    iLoopcount++;
                                }

                            }
                            i++;
                        }

                        str = ReplaceDynamicHtmlContent(oMethodInfoObj, MethodResult, sFinalString);
                        sTotalHtmlString = sTotalHtmlString.Replace(sRowString, str);
                        return sTotalHtmlString;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    //   Common.SaveErrorLog(ex.ToString(), sDatabase);
                    return null;
                }
            }
            public string CheckHtmlContentTable(string HtmlContentTable, int iStartingIndex)
            {
                try
                {
                    //here we get table row
                    string sHtmlTableRow = "";
                    int iIndex = HtmlContentTable.IndexOf("}}.{{", iStartingIndex);
                    if (iIndex > 0)
                    {
                        int iStartPosi = HtmlContentTable.LastIndexOf("<tr>", iIndex);
                        int iEndPosi = HtmlContentTable.IndexOf("</tr>", iStartPosi);
                        int iLenght = "</tr>".Length;
                        sHtmlTableRow = HtmlContentTable.Substring(iStartPosi, (iEndPosi - iStartPosi) + iLenght);
                    }
                    return sHtmlTableRow;
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    //   Common.SaveErrorLog(ex.ToString(), sDatabase);
                    return null;
                }
            }
            public string CheckModelContent(string HtmlContentTable)
            {
                try
                {

                    //we get model properties
                    string sFinalString = "";
                    if (HtmlContentTable.Contains("}}"))
                    {
                        int iCollectionIndex = HtmlContentTable.IndexOf("}}");
                        int iStartPosi = HtmlContentTable.LastIndexOf("{{", iCollectionIndex);
                        int iStringLength = "}}".Length;
                        sFinalString = HtmlContentTable.Substring(iStartPosi, (iCollectionIndex - iStartPosi) + iStringLength);
                    }
                    return sFinalString;
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    //   Common.SaveErrorLog(ex.ToString(), sDatabase);
                    return null;
                }

            }
            public string ReplaceDynamicHtmlContent(List<MethodInfoList> oMethodInfoList, object oListInfo, string sHtmlString)
            {
                //string sDatabase = SessionManager.CoreDatabase;
                try
                {
                    StringBuilder sTableRowData = new StringBuilder();

                    foreach (var list in (dynamic)oListInfo)
                    {
                        StringBuilder sTableRow = new StringBuilder(sHtmlString);
                        foreach (var methodinfo in oMethodInfoList)
                        {
                            if (methodinfo.oMethoInfo != null)
                            {
                                var subMethodResult = methodinfo.oMethoInfo.Invoke(list, null);
                                sTableRow = sTableRow.Replace(methodinfo.sHtmlString, Convert.ToString(subMethodResult));
                            }

                        }
                        sTableRowData.Append(sTableRow.ToString());
                    }
                    return sTableRowData.ToString();
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    //   Common.SaveErrorLog(ex.ToString(), sDatabase);
                    return null;
                }
            }
            public string ReplaceDynamicModelData(object model, string sFinalString, string sTotalHtmlContent)
            {
                try
                {
                    var sModelParamName = sFinalString.Replace("{{", "").Replace("}}", "");
                    var sGetMethodName = "get_" + sModelParamName;
                    var oMethod = ((object)model).GetType().GetMethod(sGetMethodName);
                    if (oMethod != null)
                    {
                        var MethodResult = oMethod.Invoke(model, null);
                        sTotalHtmlContent = sTotalHtmlContent.Replace(sFinalString, Convert.ToString(MethodResult));
                    }
                    else
                    {
                        sTotalHtmlContent = sTotalHtmlContent.Replace(sFinalString, sModelParamName);
                    }
                    return sTotalHtmlContent;
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    //   Common.SaveErrorLog(ex.ToString(), sDatabase);
                    return null;
                }
            }
        }
        #endregion

        #region XIStructureDocumentGeneration
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult SendEmailTemplateMail(int iPolicyID, int iTemplateID, string sBOName, string sHtmlContent, int iQSInstanceID)
        {
            try
            {
                iPolicyID = 127;
                //iTemplateID = 46;
                iTemplateID = 58;
                XIIQS oXIIQS = new XIIQS();
                string sSessionID = HttpContext.Session.SessionID; string sContent = "";
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                string sDatabase = SessionManager.CoreDatabase; string sOrgdatabase = SessionManager.OrgDatabase;
                int iOrgID = SessionManager.OrganizationID;
                oXIContent.sOrgDatabase = sOrgdatabase;
                //oDXI.sOrgDatabase = sOrgdatabase;

                XIInfraCache oCache = new XIInfraCache();
                //Get Document Template htmlcontent
                //var oContentDef = oDXI.Get_ContentDefinition(iTemplateID);
                var oBODCache = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName);
                var oContentDef = oBODCache.Templates.Values.Where(m => m.ID == iTemplateID).FirstOrDefault();
                //var oInstanceobj = oIXI.BOI(sBOName, Convert.ToString(iPolicyID)).Structure("New Policy").XILoad();
                var oInstanceobj = oIXI.BOI(sBOName, Convert.ToString(iPolicyID)).Structure("New Policy").XILoad();
                //var oInstanceobj = oXIIQS.QSI(oIXI.BOI(sBOName, Convert.ToString(iPolicyID)).Structure("New Policy").oSubStruct("Policy Version").oSubStruct("QS Instance"));//for question set document
                //if (oContentDef.xiStatus == 0 && oContentDef.oResult != null)
                //{
                if (oContentDef != null)
                {
                    //XIContentEditors oDocumentContent = (XIContentEditors)oContentDef.oResult;
                    if (!string.IsNullOrEmpty(sHtmlContent))
                    {
                        oContentDef.Content = sHtmlContent;
                    }
                    //Get Document Template htmlcontent with dynamic data
                    CResult oResult = new CResult();
                    oXIContent.sSessionID = sSessionID;
                    oResult = oXIContent.MergeTemplateContent(oContentDef, oInstanceobj);
                    if (oResult.bOK && oResult.oResult != null)
                    {
                        sContent = (string)oResult.oResult;
                        var oLIst = oIXI.BOI(sBOName, iPolicyID.ToString());
                        string sEmail = oLIst.Attributes.Where(x => x.Key.ToLower() == "sEmail".ToLower()).Select(x => x.Value.sValue).FirstOrDefault();
                        oEmail.EmailID = sEmail;
                        //pdf generation
                        if (sContent.Contains("html"))
                        {
                            var oCResult = oEmail.PDFGenerate(sContent, false, null, null);
                            if (oCResult.bOK && oCResult.oResult != null)
                            {
                                var oAttachment = oEmail.GeneratePDFFile((MemoryStream)oCResult.oResult);
                                if (oAttachment.bOK && oAttachment.oResult != null)
                                {
                                    Attachment data = (Attachment)oCResult.oResult;
                                    List<Attachment> oAttachments = new List<Attachment>();
                                    MemoryStream file = (MemoryStream)oCResult.oResult;
                                    oEmail.Sendmail(iOrgID, sContent, oAttachments);//send mail with attachment
                                    oXIDocs.FKiUserID = iUserID;
                                    oXIDocs.sOrgName = sOrgName;
                                    oXIDocs.sCoreDatabase = sDatabase;
                                    oXIDocs.iOrgID = iOrgID;
                                    oXIDocs.sOrgDatabase = sOrgdatabase;
                                    oXIDocs.SaveDocuments(file, data.Name); ;//save documents to folder
                                }
                            }
                        }

                        XmlDocument writer = new XmlDocument();
                        XIBOInstance oStructureI = new XIBOInstance();
                        oStructureI.oStructureInstance = oInstanceobj.oStructureInstance;
                        var oPolicyInfo = oStructureI.oSubStructureI("QS Instance");

                        if (oPolicyInfo != null && oPolicyInfo.oBOIList != null)
                        {
                            var oPolicy = oPolicyInfo.oBOIList[0];
                            XIIBO oBOI = new XIIBO();
                            XIDXI oXID = new XIDXI();
                            XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "EDI_Transaction");
                            oBOI.BOD = oBOD;
                            oBOI.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = null, bDirty = true };
                            oBOI.Attributes["sMergedText"] = new XIIAttribute { sName = "sMergedText", sValue = sContent.Replace("'", "''"), bDirty = true };
                            oBOI.Attributes["FkiBOID"] = new XIIAttribute { sName = "FkiBOID", sValue = oInstanceobj.BOI.BOD.BOID.ToString(), bDirty = true };
                            oBOI.Attributes["FkiTemplateID"] = new XIIAttribute { sName = "FkiTemplateID", sValue = iTemplateID.ToString(), bDirty = true };
                            oBOI.Attributes["iStatus"] = new XIIAttribute { sName = "iStatus", sValue = "0", bDirty = true };
                            oBOI.Attributes["iInstanceID"] = new XIIAttribute { sName = "iInstanceID", sValue = oPolicy.Attributes["id"].sValue, bDirty = true };
                            var response = oBOI.Save(oBOI);
                        }

                        //XML Generation
                        //XmlDocument writer = new XmlDocument();
                        //writer.LoadXml(string.Format("<root>{0}</root>", @sContent));
                        //writer.LoadXml(sContent);
                        //writer.Save("D:/text.xml");

                    }
                }
                //SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString());
                //Common.SaveErrorLog(Con.ToString(), "XIDNA");
                //using (SqlCommand cmd = new SqlCommand("", Con))
                //{
                //    cmd.CommandText = "Update '"+sBOName+"' set sReferReason= '" + sAttachmentPath + "RunTime_Attachment.pdf" + "' where id="+ iPolicyID + "";

                //    Con.Open();
                //    //Con.ChangeDatabase(sOrgDB);
                //    cmd.ExecuteNonQuery();
                //    Con.Dispose();

                //}

                return Json(sContent, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), "");
                return null;
            }
        }
        public FileStreamResult GenerateXML(string sContent)
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;

            using (XmlWriter xw = XmlWriter.Create(ms, xws))
            {
                XDocument doc = new XDocument(sContent);
                doc.WriteTo(xw);
            }
            ms.Position = 0;
            return File(ms, "text/xml", "Sample.xml");
        }
        #endregion

        //sending an sms

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult SendSMS(String Users ,int ID,int Type)
        //{
        //    var res = ContentRepository.GetContent(ID, sDatabase);
        //    string body = "";
        //    body = res.Content;
        //    var leadsdata = ContentRepository.GetLeadsData(Users, sDatabase);
        //    var today = System.DateTime.Now;
        //    var tomorrow = System.DateTime.Now.AddDays(1);
        //    var yesterday = System.DateTime.Now.AddDays(-1);
        //    foreach (var user in leadsdata)
        //    {
        //        //string[] splitedValues = body.Split(new char[] { '{','{', '}','}' }, StringSplitOptions.RemoveEmptyEntries);
        //        //string feild = body.Replace('{' + splitedValues[1] + '}', user.FirstName).Replace('{' + splitedValues[3] + '}', user.PhoneNumber);
        //        string feild = body.Replace("{{FirstName}}", "{0}").Replace("{{LastName}}", "{1}").Replace("{{Class}}", "{2}").Replace("{{Mobile}}", "{3}").Replace("{{Today}}", "{4}").Replace("{{Tomorrow}}", "{5}").Replace("{{Yesterday}}", "{6}");
        //        try
        //        {
        //            string messageBody = String.Format(feild, user.FirstName, user.LastName, user.Class, user.Mobile, today, tomorrow, yesterday);
        //            Utility.SentSMSToUser("7396285878", "Welcome to INative tech solutions");
        //        }

        //        catch (Exception e)
        //        {
        //            Console.WriteLine("failed to send SMS with the following error:");
        //            Console.WriteLine(e.Message);
        //            //ModelState.AddModelError("", e.Message);
        //        }
        //    }
        //    return RedirectToAction("GetdropdownforContent");
        //}  

    }
}