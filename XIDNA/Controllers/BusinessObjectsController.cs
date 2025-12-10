using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XIDNA.Models;
using XIDNA.ViewModels;
using System.Data;
using System.Data.SqlClient;
using XIDNA.Repository;
using XIDNA.Common;
using System.Security.Claims;
using System.Threading;
using XICore;
using XISystem;
using System.Net;
using System.Globalization;
using System.Timers;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using System.Web.UI.WebControls;
using System.Runtime.Remoting.Contexts;

namespace XIDNA.Controllers
{
    [Authorize]
    //[SessionTimeout]
    public class BusinessObjectsController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IBusinessObjectsRepository BusinessObjectsRepository;

        public BusinessObjectsController() : this(new BusinessObjectsRepository()) { }

        public BusinessObjectsController(IBusinessObjectsRepository BusinessObjectsRepository)
        {
            this.BusinessObjectsRepository = BusinessObjectsRepository;
        }
        //    public BusinessObjectsController()
        //    {

        //}
        XIInfraUsers oUser = new XIInfraUsers();
        XIInfraCache oCache = new XIInfraCache();
        XIConfigs oConfig = new XIConfigs();
        CommonRepository Common = new CommonRepository();
        //
        // GET: /BusinessObjects/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BusinessObjectsList()
        {
            return PartialView("BusinessObjectsList");
        }
        public ActionResult GetBusinessObjects(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = BusinessObjectsRepository.GetBusinessObjects(param, iUserID, sOrgName, SessionManager.CoreDatabase);
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

        public ActionResult AddEditBO(int ID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                BOs Bo = new BOs();
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                var fkiApplicationID = oUser.FKiApplicationID;
                if (ID == 0)
                {
                    Bo.StatusTypes = Common.GetStatusTypeDDL(sDatabase);
                    Bo.HelpTypes = BusinessObjectsRepository.GetHelpItems(sDatabase);
                    Bo.DataSources = BusinessObjectsRepository.GetDataSources(iUserID, sOrgName, sDatabase);
                    Bo.sVersion = "1";
                    Bo.sUpdateVersion = "1";
                    Bo.bUID = true;
                    if (fkiApplicationID == 0)
                    {
                        Bo.ddlApplications = Common.GetApplicationsDDL();
                    }
                    Bo.FKiApplicationID = fkiApplicationID;
                    return PartialView("_AddBOForm", Bo);
                }
                else
                {
                    //Bo = BusinessObjectsRepository.GetBOByID(ID);
                    Bo = BusinessObjectsRepository.CopyBOByID(ID, iUserID, sOrgName, sDatabase);
                    Bo.StatusTypes = Common.GetStatusTypeDDL(sDatabase);
                    Bo.HelpTypes = BusinessObjectsRepository.GetHelpItems(sDatabase);
                    Bo.DataSources = BusinessObjectsRepository.GetDataSources(iUserID, sOrgName, sDatabase);
                    if (fkiApplicationID == 0)
                    {
                        Bo.ddlApplications = Common.GetApplicationsDDL();
                    }
                    Bo.ddlBOFieldAttributes = Common.GetBOFieldAttributesDDL(ID);
                    Bo.FKiApplicationID = fkiApplicationID;
                    return PartialView("_AddBOForm", Bo);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult AddEditExtratBO(int ID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                BOs Bo = new BOs();
                int iUserID = SessionManager.UserID;
                string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                var fkiApplicationID = oUser.FKiApplicationID;
                if (ID == 0)
                {
                    Bo.StatusTypes = Common.GetStatusTypeDDL(sDatabase);
                    if (fkiApplicationID == 0)
                    {
                        Bo.ddlApplications = Common.GetApplicationsDDL();
                    }
                    Bo.DataSources = BusinessObjectsRepository.GetDataSources(iUserID, sOrgName, sDatabase);
                    Bo.FKiApplicationID = fkiApplicationID;
                    return PartialView("_ExtractBOFromTable", Bo);
                }
                else
                {
                    Bo = BusinessObjectsRepository.GetBOByID(ID, sDatabase);
                    Bo.StatusTypes = Common.GetStatusTypeDDL(sDatabase);
                    if (fkiApplicationID == 0)
                    {
                        Bo.ddlApplications = Common.GetApplicationsDDL();
                    }
                    Bo.DataSources = BusinessObjectsRepository.GetDataSources(iUserID, sOrgName, sDatabase);
                    Bo.FKiApplicationID = fkiApplicationID;
                    return PartialView("_ExtractBOFromTable", Bo);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //Poovanna 23/01/2018
        public ActionResult DeleteBO(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iStatus = BusinessObjectsRepository.DeleteBO(ID, sDatabase);
                return Json(iStatus, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveBO(BOs model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                model.CreatedBy = iUserID;
                var Res = BusinessObjectsRepository.SaveBO(model, iUserID, sOrgName, sDatabase);
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveExtractedBO(BOs model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                model.CreatedBy = iUserID;
                var Res = BusinessObjectsRepository.SaveExtractedBO(model, iUserID, sOrgName, sDatabase);
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddAttributes(int BOID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
            oUser.UserID = iUserID;
            oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
            int OrgID = oUser.FKiOrganisationID;
            try
            {
                List<BOFields> model = BusinessObjectsRepository.GetBOFields(BOID, OrgID, iUserID, sOrgName, sDatabase);
                model.FirstOrDefault().BOID = BOID;
                return PartialView("_BOAttributesForm", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult AddAttributesFromTab(string BOName, int BOID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                List<BOs> AllBOs = BusinessObjectsRepository.GetAllBos(iUserID, sOrgName, sDatabase);
                return PartialView("_AddAttributesFromTab", AllBOs);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult AssignBOAttributes(int BOID, string BOName)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                List<BOs> AllBOs = BusinessObjectsRepository.GetAllBos(iUserID, sOrgName, sDatabase);
                ViewBag.BOID = BOID;
                ViewBag.BOName = BOName;
                //AllBOs.FirstOrDefault().BOID = BOID;
                return PartialView("AssignBOAttributes", AllBOs);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }
        public ActionResult AssignBOAttributesFromGrid(int BOID, string BOName)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                List<BOs> AllBOs = BusinessObjectsRepository.GetAllBos(iUserID, sOrgName, sDatabase);
                BOName = AllBOs.Where(m => m.BOID == BOID).Select(m => m.Name).FirstOrDefault();
                return RedirectToAction("AssignBOAttributes", new { BOID = BOID, BOName = BOName });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }

        public ActionResult AddAttributesFromGrid(int BOID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                List<BOFields> model = BusinessObjectsRepository.GetBOFields(BOID, OrgID, iUserID, sOrgName, sDatabase);
                return PartialView("_BOAttributesFormFromGrid", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult CreateAttributeForField(string Labels, string FieldName, string Checkboxes)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                List<string> Label = Labels.Split(',').ToList();
                List<string> Checkbox = Checkboxes.Split(',').ToList();
                int id = BusinessObjectsRepository.CreateAttributeForField(Label, Checkbox, FieldName, sDatabase);
                return Json(id);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult SaveBOAttributes(List<BOFields> model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                model.FirstOrDefault().CreatedByName = User.Identity.Name;
                int Res = BusinessObjectsRepository.AddBOAttributes(model, sDatabase, iUserID);
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult IDESaveBOAttributes(XIDAttribute model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                XIConfigs oConfig = new XIConfigs();
                if (model != null)
                {
                    model.iUserID = SessionManager.UserID;
                    model.CreatedByName = User.Identity.Name;
                    var Result = oConfig.Save_BOAttr(model);
                }
                var Res = Common.ResponseMessage();
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult BOAttributeGrouping()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                List<BOs> AllBOs = BusinessObjectsRepository.GetAllBos(iUserID, sOrgName, sDatabase);
                return PartialView("_BOAttributeGrouping", AllBOs);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult BOAttributeGroupingFromTab(int BOID, string BOName)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                ViewBag.BOID = BOID;
                ViewBag.BOName = BOName;
                List<BOs> AllBOs = BusinessObjectsRepository.GetAllBos(iUserID, sOrgName, sDatabase);
                return PartialView("_BOAttributeGroupingFromTab", AllBOs);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult AddBOAttributeGroup(int BOID, string Name)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                BOAttributeVIewModel model = BusinessObjectsRepository.GetBOFieldsByID(BOID, sDatabase);
                model.BOName = Name;
                model.IsMultiColumnGroup = true;
                return PartialView("_AttributeGroupingForm", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpPost]
        public ActionResult GetAvailableFields(int BOID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                BOAttributeVIewModel model = BusinessObjectsRepository.GetBOFieldsByID(BOID, sDatabase);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpPost]
        public ActionResult AddBOAttributesGroup(BOGroupFields model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                //XIInfraCache oCache = new XIInfraCache();
                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "", model.BOIDXIGUID.ToString());
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                model.CreatedByName = User.Identity.Name;
                XIDGroup oGroup = new XIDGroup();
                List<string> IDsNames = new List<string>();
                IDsNames = model.BOFieldNames.Split(',').ToList();
                string fieldids = "", fieldsqlnames = "", fieldnames = "";
                if (model.IsMultiColumnGroup == false)
                {
                    foreach (var items in IDsNames)
                    {
                        List<string> IDs = items.Split(':').ToList();
                        fieldids = fieldids + IDs[0] + ", ";
                        fieldsqlnames = fieldsqlnames + "Convert(varchar(256), " + "[" + oBOD.TableName + "]." + IDs[1] + ")" + "+' '+";
                        fieldnames = fieldnames + IDs[1] + ", ";
                    }
                    string GroupName = model.GroupName;
                    //if (GroupName.Contains(' '))
                    //{
                    //    GroupName = GroupName.Replace(' ', '_');
                    //}
                    fieldsqlnames = fieldsqlnames.Substring(0, fieldsqlnames.Length - 5);
                    fieldnames = fieldnames.Substring(0, fieldnames.Length - 2);
                }
                else
                {
                    foreach (var items in IDsNames)
                    {
                        List<string> IDs = items.Split(':').ToList();
                        fieldids = fieldids + IDs[0] + ", ";
                        fieldnames = fieldnames + IDs[1] + ", ";
                        fieldsqlnames = fieldsqlnames + IDs[1] + ", ";
                    }
                    fieldnames = fieldnames.Substring(0, fieldnames.Length - 2);
                    fieldsqlnames = fieldsqlnames.Substring(0, fieldsqlnames.Length - 2);
                }
                fieldids = fieldids.Substring(0, fieldids.Length - 2);
                BOGroupFields bogmodel = new BOGroupFields();
                oGroup.BOID = model.BOID;
                oGroup.BOIDXIGUID = model.BOIDXIGUID;
                oGroup.XIGUID = model.XIGUID;
                oGroup.BOFieldIDs = fieldids;
                oGroup.BOFieldIDXIGUIDs = fieldids;
                oGroup.BOSqlFieldNames = fieldsqlnames;
                oGroup.BOFieldNames = fieldnames;
                oGroup.GroupName = model.GroupName;
                oGroup.Description = model.GroupName + " Description";
                oGroup.TypeID = 0;
                oGroup.IsMultiColumnGroup = model.IsMultiColumnGroup;
                oGroup.ID = model.ID;
                oConfig.Save_BOGroup(oGroup);
                var Response = Common.ResponseMessage();
                //int group = BusinessObjectsRepository.AddAttributeGroup(model, sDatabase, iUserID, sOrgName);
                return Json(Response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GroupingGrid(int BOID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                BOFields model = new BOFields();
                return PartialView("_GroupingGrid", BOID);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetAttributeGroups(jQueryDataTableParamModel param, int BOID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = BusinessObjectsRepository.GetAttributeGroups(param, BOID, sDatabase);
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

        public ActionResult EditBOAttributeGroup(int GroupID, int BOID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                if (GroupID != 0)
                {
                    BOAttributeVIewModel model = BusinessObjectsRepository.EditBOAttributeGroup(GroupID, BOID, sDatabase);
                    return PartialView("_EditBOAttributeGrouping", model);
                }
                else
                {
                    BOAttributeVIewModel model = BusinessObjectsRepository.GetBOFieldsByID(BOID, sDatabase);
                    return PartialView("_EditBOAttributeGrouping", model);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }

        public int RemoveGroup(int GroupID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int id = BusinessObjectsRepository.RemoveGroup(GroupID, sDatabase);
                return id;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return 0;
            }
        }

        [HttpPost]
        public ActionResult IsExistsBOName(string Name, int BOID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return BusinessObjectsRepository.IsExistsBOName(Name, BOID, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult IsExistsGroup(string GroupName, int GroupID, int BOID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return BusinessObjectsRepository.IsExistsGroupName(GroupName, GroupID, BOID, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult WherePopUP(int FieldID, string FieldDataType, string Type)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var result = BusinessObjectsRepository.GetPopUpDataByID(FieldID, sDatabase);
                VMWherePopUP model = new VMWherePopUP();
                model.FieldName = result.Name;
                model.FieldID = FieldID;
                model.FieldDataType = result.DataType;
                model.BOName = result.BOName;
                model.IsDate = result.IsDate;
                model.IsDBValue = result.IsDBValue;
                model.DBQuery = result.DBQuery;
                model.IsWhereExpression = result.IsWhereExpression;
                model.WhereExpression = result.WhereExpression;
                model.WhereExpressionValue = result.WhereExpreValue;
                model.IsExpression = result.IsExpression;
                model.ExpressionText = result.ExpressionText;
                model.ExpressionValue = result.ExpressionValue;
                model.IsRuntimeValue = result.IsRunTime;
                model.Type = Type;
                return PartialView("_WherePopUP", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpPost]
        public ActionResult SavePopUpItems(VMWherePopUP model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int id = BusinessObjectsRepository.CreatePopUpItems(model, sDatabase);
                return Json(id, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetScriptWindow(int ID, int Type)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var Data = BusinessObjectsRepository.GetPopUpDataByID(ID, sDatabase);
                VMWherePopUP model = new VMWherePopUP();
                model.FieldID = Data.ID;
                model.Script = Data.Script;
                model.ScriptExecutionType = Data.ScriiptExecutionType;
                return PartialView("_ScriptWindow", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult SaveScript(int ID, string Script, string ExecuteType)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var Res = BusinessObjectsRepository.SaveScript(ID, Script, ExecuteType, sDatabase);
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ValidateScript(string Script)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var Res = BusinessObjectsRepository.ValidateScript(Script);
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        //Create a form to view the attributes details
        public ActionResult GetBOAtrributesForm(string FieldName, int BOID, bool IsLayout = true)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                var ddlOneClicks = BusinessObjectsRepository.GetOneClicks(sDatabase);
                BOFields model = BusinessObjectsRepository.GetBOAtrributesForm(FieldName, BOID, iUserID, sOrgName, sDatabase);
                //Check if the model value is null
                model.BOID = BOID;
                model.ddlOneClicks = ddlOneClicks;
                ViewBag.IsLayout = IsLayout;
                return PartialView("BOFormWithAttributes", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        //option list
        public ActionResult SaveBoOptionList(string Values, int BOID, int iID, string AtrName)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int sResults = BusinessObjectsRepository.SaveBoOptionList(Values, BOID, iID, AtrName, sDatabase);
                return Json(sResults, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        //Edit option list
        public ActionResult EditBoOptionList(int BOID, string AtrName)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                List<List<string>> sResults = BusinessObjectsRepository.EditBoOptionList(BOID, AtrName, sDatabase);
                return Json(sResults, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        //check option list
        public ActionResult CheckBoOptionList(int BOID, string AtrName)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                bool sResults = BusinessObjectsRepository.CheckBoOptionList(BOID, AtrName, sDatabase);
                return Json(sResults, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        //Delete option list
        public ActionResult DeleteBoOptionList(int BOID, int iID, string AtrName)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iResult = BusinessObjectsRepository.DeleteBoOptionList(BOID, iID, AtrName, sDatabase);
                return Json(iResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        //Save attribute form details
        public ActionResult SaveFormBOAttributes(BOFields model)
        //public ActionResult SaveFormBOAttributes(List<FormData> FormValues)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                var Res = BusinessObjectsRepository.SaveFormBOAttributes(model, iUserID, sOrgName, sDatabase);
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        //Delete attributes from the BO attributes
        public ActionResult DeleteAttribute(int BOID, string AtrName)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                int iDelete = BusinessObjectsRepository.DeleteAttribute(BOID, AtrName, iUserID, sOrgName, sDatabase);
                return Json(iDelete, JsonRequestBehavior.AllowGet);
                //Refresh "_BOAttributeForm" but the tabs like "Bo attributes etc are in "AssignBOAttributes".
                //return RedirectToAction("AddAttributes", "BusinessObjects", new { @BOID = BOID });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        //copy BO
        public ActionResult CopyBO(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                BOs Bo = new BOs();
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                Bo = BusinessObjectsRepository.CopyBOByID(ID, iUserID, sOrgName, sDatabase);
                Bo.StatusTypes = Common.GetStatusTypeDDL(sDatabase);
                return PartialView("_CopyBOForm", Bo);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        //Create Bo with table
        public ActionResult CreateTableFromBO(BOs model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
            oUser.UserID = iUserID;
            oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
            int OrgID = oUser.FKiOrganisationID;
            // to create BO
            int CreatedByID = SessionManager.UserID;
            string CreatedByName = User.Identity.Name;
            try
            {
                //var sCreateTablBO = BusinessObjectsRepository.CreateTableFromBO(model, database, CreatedByID, CreatedByName);
                var sCreateTablBO = BusinessObjectsRepository.CreateTableFromBO(model, OrgID, CreatedByID, CreatedByName, iUserID, sOrgName, sDatabase);
                //Create DashBoards for BO Charts
                if (model.sType == "MasterEntity" || model.sType == "Transaction")
                {
                    var DashBoardType = model.sDashBoardType;
                    if (DashBoardType == "" || DashBoardType == null)
                    {
                        DashBoardType = "AM-Charts";
                    }
                    model.sColumns = model.ColName;
                    int BOID = Convert.ToInt32(sCreateTablBO.ID);
                    SignalR oSignalRs = new SignalR();
                    XIConfigs oConfigs = new XIConfigs();
                    oConfigs.iBODID = BOID;
                    oConfigs.sBOName = model.Name;
                    oConfigs.sConfigDatabase = SessionManager.ConfigDatabase;
                    oConfigs.iAppID = SessionManager.ApplicationID;
                    oConfigs.iUserID = iUserID;
                    oConfig.sSessionID = HttpContext.Session.SessionID;
                    oConfigs.iOrgID = OrgID;
                    List<CNV> oiParams = new List<CNV>();
                    oiParams.Add(new CNV { sName = XIConstant.Param_BO, sValue = model.Name });
                    oiParams.Add(new CNV { sName = XIConstant.Param_InstanceID, sValue = BOID.ToString() });
                    //Build_BO method Creating for bo Popup, Structure 
                    var oCR1 = oConfigs.Build_BO(oiParams);
                    List<CNV> oParams = new List<CNV>();
                    oParams.Add(new CNV { sName = "LayoutName", sValue = model.Name + " Default BO Data Layout" });
                    oParams.Add(new CNV { sName = "DialogName", sValue = model.Name + " Default BO Data" });
                    oParams.Add(new CNV { sName = "XilinkName", sValue = model.Name + " Default BO Data Xilink" });
                    oParams.Add(new CNV { sName = "sParentID", sValue = "1346" }); //2705
                    oParams.Add(new CNV { sName = "sApplicationName", sValue = oUser.sDatabaseName });
                    oParams.Add(new CNV { sName = "irowxilinkid", sValue = oCR1.sCode.ToString() }); //Based on Structure BO's RowXilink ID
                    oParams.Add(new CNV { sName = XIConstant.Param_ApplicationID, sValue = oUser.FKiApplicationID.ToString() });
                    oConfigs.iBODID = BOID;
                    var oCR = oConfigs.Save_BODashBoards(oParams, DashBoardType);
                }
                else
                {

                }
                return Json(sCreateTablBO, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse
                {
                    Status = false,
                    ResponseMessage = ServiceConstants.ErrorMessage
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult IDECreateTableFromBO(XIDBO model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
            oUser.UserID = iUserID;
            oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
            int OrgID = oUser.FKiOrganisationID;
            int FKiAppID = oUser.FKiApplicationID;
            // to create BO
            int CreatedByID = SessionManager.UserID;
            string CreatedByName = User.Identity.Name;
            try
            {
                var iApplicationID = SessionManager.ApplicationID;
                SignalR oSignalR = new SignalR();
                XIConfigs oConfig = new XIConfigs(oSignalR);
                oConfig.sConfigDatabase = SessionManager.ConfigDatabase;
                oConfig.iAppID = iApplicationID;
                oConfig.iUserID = iUserID;
                oConfig.sSessionID = HttpContext.Session.SessionID;
                oConfig.iOrgID = OrgID;
                var oBODef = oConfig.Save_BO(model);
                var Response = Common.ResponseMessage();
                return Json(Response, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse
                {
                    Status = false,
                    ResponseMessage = ServiceConstants.ErrorMessage
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public string GetVersionForBO(string sUpdatedVersion)
        {
            float rUpdatedVersion = float.Parse(sUpdatedVersion, CultureInfo.InvariantCulture.NumberFormat);
            rUpdatedVersion += 0.1F;
            return rUpdatedVersion.ToString();
        }
        #region DynamicForm

        public ActionResult GetDefaultValues(string sAttrNames, string BOName)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                List<string> lAddDefaults = BusinessObjectsRepository.GetDefaultValues(sAttrNames, BOName, sDatabase);
                return Json(lAddDefaults, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        //public ActionResult CreateNewForm(FormDetails model)
        //{
        //    string sDatabase = SessionManager.CoreDatabase;
        //    VMCreateForm sGetBOAttr = ClientRepository.CreateNewForm(model, sDatabase);
        //    return PartialView("_GenerateForm", sGetBOAttr);
        //}

        public ActionResult SaveFormData(List<FormData> FormValues, string sTableName)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                int iStatus = BusinessObjectsRepository.SaveFormData(FormValues, sTableName, iUserID, sOrgName, sDatabase);
                return Json(iStatus, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        //XILink Dynamic form
        public ActionResult CreateDynamicForm(int XiLinkID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                // int XiLinkID = 8;
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                VMCreateForm sGetBOAttr = BusinessObjectsRepository.CreateDynamicForm(XiLinkID, iUserID, sOrgName, sDatabase);
                return PartialView("_CreateDynamicForm", sGetBOAttr);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion DynamicForm

        #region BOScripts

        public ActionResult BOScripts(int BOID = 0, bool isFromBO = false)
        {
            ViewBag.isFromBO = isFromBO;
            return PartialView("_GridBOScripts", BOID);
        }

        public ActionResult GetBOScriptsList(jQueryDataTableParamModel param, int BOID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = BusinessObjectsRepository.GetBOScriptsList(param, BOID, iUserID, sOrgName, sDatabase);
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

        public ActionResult AddEditBOScript(int FKiBOID = 0, int ID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                BOScripts oBOScript = new BOScripts();
                if (ID > 0)
                {
                    oBOScript = BusinessObjectsRepository.GetScriptByID(ID, sDatabase);
                }
                oBOScript.FKiBOID = FKiBOID;
                oBOScript.ddlLanguages = Common.GetScriptLanguageDDL(sDatabase);
                oBOScript.ddlScriptTypes = Common.GetScriptTypeDDL(sDatabase);
                oBOScript.ddlStatusTypes = Common.GetStatusTypeDDL(sDatabase);
                oBOScript.ScriptResults = new List<BOScriptResults>();
                return PartialView("_AddEditBOScriptForm", oBOScript);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        [ValidateInput(true)]
        public ActionResult SaveBOScript(BOScripts model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                model.CreatedBy = model.UpdatedBy = iUserID;
                var Response = BusinessObjectsRepository.SaveBOScript(model, iUserID, sOrgName, sDatabase);
                return Json(Response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        //IDE SaveBOScript
        [HttpPost]
        [ValidateInput(true)]
        public ActionResult IDESaveBOScript(XIDScript oScript)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oScript.CreatedBy = oScript.UpdatedBy = iUserID;
                XIConfigs oConfig = new XIConfigs();
                var Result = oConfig.Save_BOScript(oScript);
                var Response = Common.ResponseMessage();
                return Json(Response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult CopyScript(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                return Json(BusinessObjectsRepository.CopyScript(ID, iUserID, sOrgName, sDatabase), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet); ;
            }
        }
        #endregion BOScripts

        #region BOClassAttributes

        public ActionResult ClassAttributes()
        {
            string sDatabase = SessionManager.CoreDatabase;
            var BOs = Common.GetBOsDDL(sDatabase);
            return View("GridClassAttribute", BOs);
        }
        public ActionResult GetClassAttributesGrid(jQueryDataTableParamModel param, int BOID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = BusinessObjectsRepository.GetClassAttributesGrid(param, BOID, sDatabase);
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

        public ActionResult AddEditClassAttribute(int BOID, int ID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                BOClassAttributes oBOClassAttr = new BOClassAttributes();
                if (ID > 0)
                {
                    //oBOScript = BusinessObjectsRepository.GetScriptByID(ID);
                }
                oBOClassAttr.ddlBOs = Common.GetBOsDDL(sDatabase);
                oBOClassAttr.BOID = BOID;
                return PartialView("_AddEditClassAttribute", oBOClassAttr);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult EditClassAttribute(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var result = BusinessObjectsRepository.EditClassAttribute(ID, sDatabase);
                result.ddlBOs = Common.GetBOsDDL(sDatabase);
                return PartialView("_AddEditClassAttribute", result);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult IsExistsClassName(string Class, int BOID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return BusinessObjectsRepository.IsExistsClassName(Class, BOID, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveBOClassAttibute(BOClassAttributes model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var Response = BusinessObjectsRepository.SaveBOClassAttibute(model, sDatabase);
                return Json(Response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        #endregion BOClassAttributes

        #region Import BO
        //create a button in bo grid and call this method
        public ActionResult ImportBODetails(int ID = 0)
        {
            return View();
        }

        //public ActionResult UploadXMLBO(int ID,List<HttpPostedFileBase> UploadXML)
        public ActionResult UploadXMLBO(string sFilePath)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID;
                string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                string sCreatedByName = User.Identity.Name;
                string sStatus = BusinessObjectsRepository.ImportBOInXML(sFilePath, iUserID, sOrgName, sDatabase);
                return Json(sStatus, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        #endregion Import BO
        #region Data Source
        public ActionResult DataSourceGrid()
        {
            return View();
        }

        public ActionResult GetDataSource(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = BusinessObjectsRepository.GetDataSource(param, iUserID, sOrgName, SessionManager.CoreDatabase);
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

        public ActionResult AddEditXIDataSource(int ID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                XIDataSources oDataSource = new XIDataSources();
                int iUserID = SessionManager.UserID;
                string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                var fkiApplicationID = oUser.FKiApplicationID;
                if (ID == 0)
                {
                    if (fkiApplicationID == 0)
                    {
                        oDataSource.ddlApplications = Common.GetApplicationsDDL();
                    }
                    oDataSource.ddlOrgs = new List<VMDropDown>();
                    oDataSource.FKiApplicationID = fkiApplicationID;
                    return PartialView("_AddXIDataSource", oDataSource);
                }
                else
                {
                    oDataSource = BusinessObjectsRepository.GetDataSourceDetails(ID, iUserID, sOrgName, sDatabase);
                    if (fkiApplicationID == 0)
                    {
                        oDataSource.ddlApplications = Common.GetApplicationsDDL();
                    }
                    if (oDataSource.FKiApplicationID > 0)
                    {
                        oDataSource.ddlOrgs = new List<VMDropDown>();// BusinessObjectsRepository.GetAppOrganisations(oDataSource.FKiApplicationID);
                    }
                    else
                    {
                        oDataSource.ddlOrgs = new List<VMDropDown>();
                    }
                    oDataSource.FKiApplicationID = fkiApplicationID;
                    return PartialView("_AddXIDataSource", oDataSource);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult CreateDataSource(XIDataSources model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID;
                string sOrgName = SessionManager.OrganisationName;
                //model.CreatedByID = iUserID;
                //model.CreatedByName = User.Identity.Name;
                var Res = BusinessObjectsRepository.CreateDataSource(model, iUserID, sOrgName, sDatabase);
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        //Save_DataSource
        [HttpPost]
        public ActionResult Save_DataSource(XIDataSources model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                int iOrgID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiOrgID;
                int FKiAppID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiApplicationID;
                ModelDbContext dbContext = new ModelDbContext();
                XIIBO oBOI = new XIIBO();
                XIDBO oBOD = new XIDBO();
                XIEncryption oXIAPI = new XIEncryption();
                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XI DataSources");
                oBOI.BOD = oBOD;
                //oBOI.SetAttribute("ID", model.ID.ToString());
                oBOI.SetAttribute("XIGUID", model.XIGUID.ToString());
                oBOI.SetAttribute("FKiApplicationIDXIGUID", model.FKiApplicationIDXIGUID.ToString());
                oBOI.SetAttribute("sName", model.sName);
                oBOI.SetAttribute("sType", model.sType);
                oBOI.SetAttribute("sDescription", model.sDescription);
                oBOI.SetAttribute("OrganisationID", iOrgID.ToString());
                //oBOI.SetAttribute("FKiApplicationID", model.FKiApplicationID.ToString());
                oBOI.SetAttribute("sQueryType", "MSSQL");
                oBOI.SetAttribute("sDataSourceType", "Database");
                oBOI.SetAttribute("StatusTypeID", model.StatusTypeID.ToString());
                if (model.ID == 0)
                {
                    oBOI.SetAttribute("CreatedBy", iUserID.ToString());
                    oBOI.SetAttribute("CreatedTime", DateTime.Now.ToString());
                    oBOI.SetAttribute("CreatedBySYSID", Dns.GetHostAddresses(Dns.GetHostName())[1].ToString());
                }
                oBOI.SetAttribute("UpdatedBy", iUserID.ToString());
                oBOI.SetAttribute("UpdatedTime", DateTime.Now.ToString());
                oBOI.SetAttribute("UpdatedBySYSID", Dns.GetHostAddresses(Dns.GetHostName())[1].ToString());
                var oXIDS = oBOI.Save(oBOI);
                var oXIDSID = "";
                if (oXIDS.bOK && oXIDS.oResult != null)
                {
                    var oXL = (XIIBO)oXIDS.oResult;
                    oXIDSID = oXL.Attributes.Values.Where(m => m.sName.ToLower() == "xiguid").Select(m => m.sValue).FirstOrDefault();
                }
                XIIXI oXI = new XIIXI();
                var oDSI = oXI.BOI("XI DataSources", oXIDSID);
                var oDSID = oDSI.AttributeI("id").sValue;
                var sEnrypted = oXIAPI.EncryptData(model.sConnectionString, true, oDSID.ToString());
                oBOI.Attributes["sConnectionString".ToLower()] = new XIIAttribute { sName = "sConnectionString", sValue = sEnrypted, bDirty = true };
                oXIDS = oBOI.Save(oBOI);
                var Res = Common.ResponseMessage();
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult CheckConnectionString(string sConnectionString)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID;
                string sOrgName = SessionManager.OrganisationName;
                var sStatus = BusinessObjectsRepository.CheckConnectionString(sConnectionString);
                return Json(sStatus, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetAppOrganisations(int iAppID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var Data = BusinessObjectsRepository.GetAppOrganisations(iAppID);
                return Json(Data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new List<VMDropDown>(), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion Data Source

        #region XIStructure

        public ActionResult GetBOStructure(int iBOID)
        {
            return PartialView("_GridBOStructures", iBOID);
        }

        public ActionResult GetBOStructuresList(jQueryDataTableParamModel param, int BOID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = BusinessObjectsRepository.GetBOStructuresList(param, BOID, sDatabase);
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

        public ActionResult BOXIStructure(int BOID, int iStructureID = 0, string sSavingType = "")
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                ModelDbContext dbContext = new ModelDbContext();
                int iUserID = SessionManager.UserID;
                string sOrgName = SessionManager.OrganisationName;
                if (iStructureID == 0)
                {
                    List<cXIStructure> Tree = BusinessObjectsRepository.GetBOStructureTree(BOID, sDatabase);
                    if (Tree.Count() == 0)
                    {
                        Tree.Add(new cXIStructure() { sSavingType = sSavingType });
                    }
                    else
                    {
                        Tree.FirstOrDefault().sSavingType = sSavingType;
                    }
                    Dictionary<string, string> QSSteps = new Dictionary<string, string>();
                    Tree.FirstOrDefault().BOList = Common.GetBOsDDL(sDatabase);
                    var oQSStpesList = dbContext.QSStepDefiniton.ToList();
                    foreach (var item in oQSStpesList)
                    {
                        QSSteps[item.ID.ToString()] = item.sName;
                    }
                    Tree.FirstOrDefault().AllQSSteps = QSSteps;
                    return PartialView("_BOXIStructureTree", Tree);
                }
                else
                {
                    List<cXIStructure> Tree = BusinessObjectsRepository.GetXIStructureTreeDetails(BOID, iStructureID.ToString(), sDatabase);
                    if (Tree.Count() == 0)
                    {
                        Tree.Add(new cXIStructure() { sSavingType = sSavingType });
                    }
                    else
                    {
                        Tree.FirstOrDefault().sSavingType = sSavingType;
                    }
                    Dictionary<string, string> QSSteps = new Dictionary<string, string>();
                    Tree.FirstOrDefault().BOList = Common.GetBOsDDL(sDatabase);
                    var oQSStpesList = dbContext.QSStepDefiniton.ToList();
                    foreach (var item in oQSStpesList)
                    {
                        QSSteps[item.ID.ToString()] = item.sName;
                    }
                    Tree.FirstOrDefault().AllQSSteps = QSSteps;
                    return PartialView("_BOXIStructureTree", Tree);
                }

                //if (Tree.Count() > 0)
                //{
                //    Tree.FirstOrDefault().bIsExists = true;
                //    return PartialView("_BOXIStructureTree", Tree);
                //}
                //else
                //{
                //    cXIStructure oStru = new cXIStructure();
                //    oStru.sBO = BOName;
                //    oStru.BOID = BOID;
                //    oStru.bIsExists = false;
                //    Tree = new List<cXIStructure>();
                //    Tree.Add(oStru);
                //    return PartialView("_BOXIStructureTree", Tree);
                //}

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetXIStructureTreeDetails(int BOID = 0, string iStuctureID = "")
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID;
                string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                List<cXIStructure> Model = BusinessObjectsRepository.GetXIStructureTreeDetails(BOID, iStuctureID, sDatabase);
                return Json(Model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult SaveBODetailsToXIStructure(int BOID, string BOName)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                //Save the details to XIStructure
                int iUserID = SessionManager.UserID;
                string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                List<cXIStructure> Tree = BusinessObjectsRepository.SaveBODetailsToXIStructure(BOID, BOName, iUserID, sOrgName, sDatabase);
                return Json(Tree, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveBOStructure(List<cXIStructure> model, int iStructureID = 0, string sSavingType = "")
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID;
                var Result = BusinessObjectsRepository.SaveBOStructure(model, iStructureID, sSavingType, iUserID, sDatabase);
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse() { ID = 0, Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Get_BOStructureDetails(int iBOID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                List<XIDStructure> oStrList = new List<XIDStructure>();
                XIDXI oXID = new XIDXI();
                var oCR = oXID.Get_XIBOStructureDefinition(iBOID.ToString(), "", "Create");
                if (oCR.bOK && oCR.oResult != null)
                {
                    oStrList = (List<XIDStructure>)oCR.oResult;
                }
                return Json(oStrList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpPost]
        public ActionResult Save_BOStructure(List<XIDStructure> oStructure, string sStructureName = "", string sCode = "")
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID;
                SignalR oSignalR = new SignalR();
                XIConfigs oConfig = new XIConfigs(oSignalR);
                oConfig.iUserID = iUserID;
                oConfig.sSessionID = HttpContext.Session.SessionID;
                oStructure.ForEach(m => m.sStructureName = sStructureName); oStructure.ForEach(m => m.sCode = sCode);
                var oCR = oConfig.Save_BOStructure(oStructure);
                if (oCR.bOK && oCR.oResult != null)
                {

                }
                return Json(oCR.bOK, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse() { ID = 0, Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CreateAndSaveTreeNode(string ParentNode, string NodeID, string NodeTitle)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                long DBstatus = BusinessObjectsRepository.CreateAndSaveTreeNode(ParentNode, NodeID, NodeTitle, iUserID, OrgID, sDatabase);
                return Json(DBstatus, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DeleteNodeDetails(string ParentNode, string NodeID, string ChildrnIDs, string Type)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                List<cXIStructure> lResult = BusinessObjectsRepository.DeleteNodeDetails(ParentNode, NodeID, ChildrnIDs, Type, iUserID, OrgID, sDatabase);
                return Json(lResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult RenameTreeNode(string ParentNode, string NodeID, string NodeTitle)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                int DBstatus = BusinessObjectsRepository.RenameTreeNode(ParentNode, NodeID, NodeTitle, iUserID, OrgID, sDatabase);
                return Json(DBstatus, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AddDetailsForStructure(string ParentNode, string NodeID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                //ViewBag.DetID = DetailsID;
                ViewBag.DetID = NodeID;
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                cXIStructure model = BusinessObjectsRepository.AddDetailsForStructure(ParentNode, NodeID, OrgID, iUserID, sDatabase);
                return PartialView("_AddDetailsForXIStructure", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult SaveAddedDetails(cXIStructure model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID;
                string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                var Status = BusinessObjectsRepository.SaveAddedDetails(iUserID, model, sDatabase);
                //var result= HomeRepository.SaveAddedDetails(iUserID, model, sDatabase);
                //return null;
                return Json(Status, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                //return null;
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetBOUIDetails(int ID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var Result = BusinessObjectsRepository.GetBOUIDetails(ID, sDatabase);
                Result.FKiStructureID = ID;
                return PartialView("_BOUIDetails", Result);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveBOUIDetails(cBOUIDetails model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID;
                string sOrgName = SessionManager.OrganisationName;
                var Response = BusinessObjectsRepository.SaveBOUI(model, iUserID, sOrgName, sDatabase);
                return Json(Response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        //IDE XIBOUI Generate and Save
        [HttpPost]
        public ActionResult IDEBOUIDetails(XIDBOUI oBOUI)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID;
                string sOrgName = SessionManager.OrganisationName;

                SignalR oSignalR = new SignalR();
                XIConfigs oConfig = new XIConfigs(oSignalR);
                oConfig.iUserID = iUserID;
                oConfig.sSessionID = HttpContext.Session.SessionID;
                var oStrDetails = oConfig.Get_BOUIDetails(oBOUI);
                if (oStrDetails.bOK)
                {
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                return Json("Failure", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetBOUI(int iBOID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var Result = Common.GetBOStructuresDDL(iBOID, sDatabase);
                return PartialView("_BOUITab", Result);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetDefaultUI(int iBOID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID;
                string sOrgName = SessionManager.OrganisationName;
                var Result = BusinessObjectsRepository.GetBODefaults(iBOID, iUserID, sOrgName, sDatabase);
                return PartialView("_DefaultUI", Result);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveBODefaults(cBOUIDefaults model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                model.CreatedBy = model.UpdatedBy = SessionManager.UserID;
                var Result = BusinessObjectsRepository.SaveBODefaults(model, sDatabase);
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DragAndDropNodes(string NodeID, string OldParentID, int Oldposition, int Newposition)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;

                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                var Tab = BusinessObjectsRepository.DragAndDropNodes(NodeID, OldParentID, iUserID, sDatabase, Oldposition, Newposition);
                return Json(Tab, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult Refresh_BOStructure(string iBODID, string ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                XIInfraXIBOStructureComponent BOStruct = new XIInfraXIBOStructureComponent();
                List<CNV> oParams = new List<CNV>();
                oParams.Add(new CNV { sName = XIConstant.Param_BODID, sValue = iBODID });
                oParams.Add(new CNV { sName = "ID", sValue = ID });
                oParams.Add(new CNV { sName = "sType", sValue = "Refresh" });
                var oCR = BOStruct.XILoad(oParams);
                if (oCR.bOK && oCR.oResult != null)
                {
                    var Nodes = (List<XIDStructure>)oCR.oResult;
                    return Json(Nodes, JsonRequestBehavior.AllowGet);
                }
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        #endregion

        #region GUID

        public ActionResult GenerateBOGUID()
        {
            XIDXI oDXI = new XIDXI();
            var oBODDL = oDXI.Get_BOsDDL();
            List<XIDropDown> BOsDDL = new List<XIDropDown>();
            if (oBODDL.bOK && oBODDL.oResult != null)
            {
                BOsDDL = (List<XIDropDown>)oBODDL.oResult;
            }
            XIDBOGUID model = new XIDBOGUID();
            model.ddlBOs = BOsDDL;
            return View(model);
        }

        [HttpPost]
        public ActionResult RunBOGUID(XIDBOGUID model)
        {
            XIDXI oDXI = new XIDXI();
            oDXI.RunBOGUID(model);
            return Json(0, JsonRequestBehavior.AllowGet);
        }

        #endregion GUID

        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetAutoCompleteList(string sType, string iBODID = "", string sBOName = "", string sGUID = "")
        {
            string sDatabase = SessionManager.CoreDatabase;
            var UserID = SessionManager.UserID;
            var iRoleID = SessionManager.iRoleID;
            var iOrgID = SessionManager.OrganizationID;
            var iAppID = SessionManager.ApplicationID;
            var iUserLevel = SessionManager.iUserLevel;
            string sSessionID = HttpContext.Session.SessionID;
            try
            {
                var BODID = 0;
                Guid BOXIGUID = Guid.Empty;
                int.TryParse(iBODID, out BODID);
                Guid.TryParse(iBODID, out BOXIGUID);
                XIDBO oBOD = new XIDBO();
                XIIBO oBOI = new XIIBO();
                bool bAllowed = true;
                object AutoCompleteList = 0;
                var sReference = SessionManager.sReference;
                if (sType == "bo")
                {
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName, iBODID.ToString());
                }
                else
                {
                    var o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, iBODID.ToString());
                    if (o1ClickD.BOIDXIGUID != null && o1ClickD.BOIDXIGUID != Guid.Empty)
                    {
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickD.BOIDXIGUID.ToString());
                    }
                    else if (o1ClickD.BOID > 0)
                    {
                        oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickD.BOID.ToString());
                    }
                }
                var WhiteListCheck = System.Configuration.ConfigurationManager.AppSettings["WhitelistCheck"];
                if (WhiteListCheck == "yes")
                {
                    var oCR = oBOI.Check_Whitelist(oBOD.BOID, iRoleID, iOrgID, iAppID, "query", oBOD.iLevel, iUserLevel);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        var bUNAuth = (bool)oCR.oResult;
                        if (bUNAuth)
                        {
                            return Json(AutoCompleteList, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                if (BODID > 0 || (BOXIGUID != null && BOXIGUID != Guid.Empty) || !string.IsNullOrEmpty(sBOName))
                {
                    XIDXI oXID = new XIDXI();
                    XIInfraCache oCache = new XIInfraCache();
                    if (sType == "bo")
                    {
                        if (oBOD.sType == xiEnumSystem.xiBOTypes.CacheReference.ToString())
                        {
                            AutoCompleteList = oCache.GetObjectFromCache(XIConstant.CacheRefData, sType + "_" + sBOName, sType + "_" + iBODID);
                        }
                        else
                        {
                            var oResult = oXID.Get_AutoCompleteList(sType + "_" + iBODID.ToString(), sType + "_" + sBOName, null, 0, sReference);
                            if (oResult.bOK && oResult.oResult != null)
                            {
                                AutoCompleteList = oResult.oResult;

                            }
                        }
                    }
                    else
                    {
                        if (oBOD.sType == xiEnumSystem.xiBOTypes.CacheReference.ToString())
                        {
                            AutoCompleteList = oCache.GetObjectFromCache(XIConstant.CacheRefData, sType + "_" + sBOName, sType + "_" + iBODID);
                        }
                        else
                        {
                            if (BODID != 0 || (BOXIGUID != null && BOXIGUID != Guid.Empty) && !string.IsNullOrEmpty(sBOName)) //if we get iBODID & sBOName, sBOName dominates the iBODID. so sBOName = "" on 13-08-2019
                            {
                                sBOName = "";
                            }
                            XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                            List<CNV> nParms = new List<CNV>();
                            nParms = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                            var AppParam = nParms.Where(m => m.sName == "{XIP|iAppID}").FirstOrDefault();
                            if (AppParam == null)
                            {
                                nParms.Add(new CNV { sName = "{XIP|iAppID}", sValue = iAppID.ToString() });
                            }
                            else
                            {
                                nParms.Where(m => m.sName == "{XIP|iAppID}").FirstOrDefault().sValue = iAppID.ToString();
                            }
                            var OrgParam = nParms.Where(m => m.sName == "{XIP|iOrgID}").FirstOrDefault();
                            if (OrgParam == null)
                            {
                                nParms.Add(new CNV { sName = "{XIP|iOrgID}", sValue = iOrgID.ToString() });
                            }
                            else
                            {
                                nParms.Where(m => m.sName == "{XIP|iOrgID}").FirstOrDefault().sValue = iOrgID.ToString();
                            }
                            var oResult = oXID.Get_AutoCompleteList(sType + "_" + iBODID.ToString(), sType + "_" + sBOName, nParms, 0, sReference);
                            if (oResult.bOK && oResult.oResult != null)
                            {
                                AutoCompleteList = oResult.oResult;
                            }
                        }
                    }
                }
                return Json(AutoCompleteList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }


        #region NewFieldCreation
        public ActionResult CreateFieldsForm(int BOID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                BOFields model = new BOFields();
                model.BOID = BOID;
                model.CreatedByID = iUserID;
                model.CreatedByName = User.Identity.Name;
                return PartialView("_CreateFieldsForm", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion NewFieldCreation

        #region Audit
        public ActionResult CreateAuditTable(int BOID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int iOrgID = oUser.FKiOrganisationID;
                string CreatedByName = User.Identity.Name;
                var result = BusinessObjectsRepository.CreateAuditTable(BOID, iOrgID, iUserID, sOrgName, sDatabase, CreatedByName);
                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion Audit

        #region ParentDDL

        [HttpPost]
        public ActionResult GetParentDDL(string sBO)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                XIConfigs oConfig = new XIConfigs();
                List<CNV> oParams = new List<CNV>();
                oParams.Add(new CNV { sName = XIConstant.Param_BO, sValue = sBO });
                var oCR = oConfig.Get_ParentDDL(oParams);
                if (oCR.bOK && oCR.oResult != null)
                {
                    List<XIDOptionList> oDDL = (List<XIDOptionList>)oCR.oResult;
                    return Json(oDDL, JsonRequestBehavior.AllowGet);
                }
                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        #endregion ParentDDL

        #region BOAction

        [HttpPost]
        public ActionResult TriggerBOAction(int iActionID, string sBOAction, string sID, int iBOIID, string sBODID, string sGUID)
        {
            var sSessionID = HttpContext.Session.SessionID;
            var iOrgID = SessionManager.OrganizationID;
            var iAppID = SessionManager.ApplicationID;
            var iUserLevel = SessionManager.iUserLevel;
            List<CNV> oParams = new List<CNV>();
            oParams.Add(new CNV { sName = XIConstant.Param_UserID, sValue = SessionManager.UserID.ToString() });
            var BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, sBODID);
            var WhiteListCheck = System.Configuration.ConfigurationManager.AppSettings["WhitelistCheck"];
            if (WhiteListCheck == "yes")
            {
                XIIBO oBOI = new XIIBO();
                var oCR = oBOI.Check_Whitelist(BOD.BOID, SessionManager.iRoleID, iOrgID, iAppID, "action", BOD.iLevel, iUserLevel);
                if (oCR.bOK && oCR.oResult != null)
                {
                    var bUNAuth = (bool)oCR.oResult;
                    if (bUNAuth)
                    {
                        return null;
                    }
                }
            }
            if (!string.IsNullOrEmpty(sBOAction) && iActionID > 0)
            {
                if (sBOAction.ToLower() == XIConstant.Action_StructureCopy)
                {
                    XIMatrix oXIMatrix = new XIMatrix();
                    oXIMatrix.MatrixAction("ObjectAction", xiEnumSystem.EnumMatrixAction.ObjectAction, BOD.Name, iBOIID, iBOIID.ToString(), BOD.Name, oParams);
                    XIIBO oBOI = new XIIBO();
                    oBOI.sBOName = "xiboactioninstance";
                    oBOI.SetAttribute("fkiboactionid", iActionID.ToString());
                    oBOI.SetAttribute("fkiboiid", iBOIID.ToString());
                    oBOI.SetAttribute("fkibodid", BOD.BOID.ToString());
                    oBOI.Save(oBOI);
                    List<CNV> Parms = new List<CNV>();
                    XIDStructure oStructure = new XIDStructure();
                    var oCR = oStructure.StructureCopy(BOD.Name, iBOIID.ToString(), "Supplier", Parms, true);
                    if (oCR.bOK)
                    {
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                }
                else if (sBOAction.ToLower() == XIConstant.Action_XIDelete)
                {

                }
                else if (sBOAction.ToLower() == XIConstant.Action_XIAlgorithm)
                {
                    var sNewGUID = Guid.NewGuid().ToString();
                    List<CNV> oNVsList = new List<CNV>();
                    oNVsList.Add(new CNV { sName = "-iBOIID", sValue = iBOIID.ToString() });
                    oNVsList.Add(new CNV { sName = "{XIP|iInstanceID}", sValue = iBOIID.ToString() });
                    oCache.SetXIParams(oNVsList, sNewGUID, sSessionID);
                    XIDAlgorithm oAlogD = new XIDAlgorithm();
                    oAlogD = (XIDAlgorithm)oCache.GetObjectFromCache(XIConstant.CacheXIAlgorithm, null, sID);
                    var oCR = oAlogD.Execute_XIAlgorithm(sSessionID, sNewGUID);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        var Response = (CResult)oCR.oResult;
                        var sMessage = Response.sMessage;
                        if (!string.IsNullOrEmpty(sMessage) && sMessage == "ReturnYes" && !string.IsNullOrEmpty(oAlogD.sName) && oAlogD.sName.ToLower() == "Renewal Policy Button".ToLower())
                        {
                            var ResponseData = new { sType = "Dialog", ButtonXIlink = "085db811-c396-4e7e-8d15-152a398ea355", InputParams = oNVsList, sDialogText = "This renewal is not due for renewal yet, are you sure you want to begin the renewal process?" };
                            return Json(ResponseData, JsonRequestBehavior.AllowGet);
                        }
                        else if (!string.IsNullOrEmpty(sMessage) && sMessage == "ReturnNoLeadYes" && !string.IsNullOrEmpty(oAlogD.sName) && oAlogD.sName.ToLower() == "Renewal Policy Button".ToLower())
                        {
                            var ResponseData = new { sType = "Dialog", ButtonXIlink = "085db811-c396-4e7e-8d15-152a398ea355", InputParams = oNVsList, sDialogText = "This renewal is not initiated yet, are you sure you want to begin the renewal process?" };
                            return Json(ResponseData, JsonRequestBehavior.AllowGet);
                        }
                        else if (!string.IsNullOrEmpty(sMessage) && sMessage == "ReturnNo" && !string.IsNullOrEmpty(oAlogD.sName) && oAlogD.sName.ToLower() == "Renewal Policy Button".ToLower())
                        {
                            var ResponseData = new { sType = "Dialog", ButtonXIlink = "49A26748-0500-4CBD-AC73-66378CE6FEAA", InputParams = oNVsList, sYesBtnText = "Renewal Workflow", sNoBtnText = "Close", sDialogText = "Renewal has already been started for this renewal" };
                            return Json(ResponseData, JsonRequestBehavior.AllowGet);
                        }
                        else if (!string.IsNullOrEmpty(sMessage) && sMessage == "ReturnYes" && !string.IsNullOrEmpty(oAlogD.sName) && oAlogD.sName.ToLower() == "Policy MTA Button".ToLower())
                        {
                            var ResponseData = new { sType = "Dialog", ButtonXIlink = "71C38766-D87E-40F1-B169-5821192848F7", InputParams = oNVsList, sDialogText = "This MTA is not due for renewal yet, are you sure you want to begin the MTA process?" };
                            return Json(ResponseData, JsonRequestBehavior.AllowGet);
                        }
                        else if (!string.IsNullOrEmpty(sMessage) && sMessage == "ReturnNoLeadYes" && !string.IsNullOrEmpty(oAlogD.sName) && oAlogD.sName.ToLower() == "Policy MTA Button".ToLower())
                        {
                            var ResponseData = new { sType = "Dialog", ButtonXIlink = "71C38766-D87E-40F1-B169-5821192848F7", InputParams = oNVsList, sDialogText = "This MTA is not initiated yet, are you sure you want to begin the MTA process?" };
                            return Json(ResponseData, JsonRequestBehavior.AllowGet);
                        }
                        else if (!string.IsNullOrEmpty(sMessage) && sMessage == "ReturnNo" && !string.IsNullOrEmpty(oAlogD.sName) && oAlogD.sName.ToLower() == "Policy MTA Button".ToLower())
                        {
                            var ResponseData = new { sType = "Dialog", ButtonXIlink = "d7863758-e37a-4c1e-a73a-df47438b9ecc", InputParams = oNVsList, sYesBtnText = "MTA Workflow", sNoBtnText = "Close", sDialogText = "MTA has already been started for this MTA", NewButtonXIlink = "71C38766-D87E-40F1-B169-5821192848F7", sNewYesBtnText = "Yes" };
                            return Json(ResponseData, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }

            return null;
        }

        #endregion BOAction

        public void Build_BO()
        {
            SignalR oSignalR = new SignalR();
            XIConfigs oConfig = new XIConfigs(oSignalR);
            List<CNV> oParams = new List<CNV>();
            oParams.Add(new CNV { sName = XIConstant.Param_InstanceID.ToLower(), sValue = "1232" });
            oConfig.Build_BO(oParams);
        }

        [HttpPost]
        public ActionResult GetBOInstanceID(string sBOName, string sAttr, string sValue)
        {
            List<CNV> oWhrParams = new List<CNV>();
            oWhrParams.Add(new CNV { sName = sAttr, sValue = sValue });
            XIIXI oXI = new XIIXI();
            var BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName);
            var oCR = oXI.BOI(sBOName, null, null, oWhrParams);
            if (oCR != null && oCR.Attributes.Count() > 0)
            {
                var ID = oCR.AttributeI(BOD.sPrimaryKey).sValue;
                return Json(ID, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetTree(string iBODID, string sCode, string sMode)
        {
            XIInfraCache oCache = new XIInfraCache();
            XIDXI oXID = new XIDXI();
            var iAppID = SessionManager.ApplicationID;
            var role = SessionManager.sRoleName;
            var OrgID = SessionManager.OrganizationID;
            XIDOrganisation oOrg = new XIDOrganisation();
            if (sMode.ToLower() == "ide")
            {
                if (role.ToLower() == "orgide")
                {
                    sCode = "orgide";
                    iBODID = "934";
                }
                if (OrgID > 0)
                {
                    XIDXI oXI = new XIDXI();
                    oXI.sOrgDatabase = SessionManager.CoreDatabase; //System.Configuration.ConfigurationManager.AppSettings["CoreDataBase"];
                    oOrg = (XIDOrganisation)(oXI.Get_OrgDefinition(null, OrgID.ToString()).oResult);
                }
            }
            List<XIDStructure> oStruct = new List<XIDStructure>();
            var oStructD = (List<XIDStructure>)oCache.GetObjectFromCache(XIConstant.CacheStructure, sCode, iBODID.ToString());
            oStruct = (List<XIDStructure>)oXID.Clone(oStructD);
            var AppName = SessionManager.AppName;
            if (role.ToLower() == EnumRoles.OrgIDE.ToString().ToLower())
            {
                oStruct.FirstOrDefault().sParentFKColumn = "id";
                oStruct.FirstOrDefault().sInsID = OrgID.ToString();
                oStruct.FirstOrDefault().sName = oStruct.FirstOrDefault().sName + " (" + oOrg.Name + ")";
            }
            else if (role.ToLower() == EnumRoles.OrgAdmin.ToString().ToLower())
            {
                oStruct.FirstOrDefault().sParentFKColumn = "id";
                oStruct.FirstOrDefault().sInsID = OrgID.ToString();
                oStruct.FirstOrDefault().sName = oStruct.FirstOrDefault().sName + " (" + oOrg.Name + ")";
            }
            else if (sMode.ToLower() == "ide" && (SessionManager.ApplicationID > 0 || (SessionManager.ApplicationIDXIGUID != null && SessionManager.ApplicationIDXIGUID != Guid.Empty)))
            {
                if (SessionManager.sRoleName.ToLower() == EnumRoles.XISuperAdmin.ToString().ToLower() || SessionManager.sRoleName.ToLower() == EnumRoles.AppAdmin.ToString().ToLower())
                {
                    oStruct.FirstOrDefault().sParentFKColumn = "XIGUID";
                    oStruct.FirstOrDefault().sInsID = SessionManager.ApplicationIDXIGUID.ToString();
                    oStruct.FirstOrDefault().sName = oStruct.FirstOrDefault().sName + " (" + AppName + ")";
                }
            }
            //for(int i = 0; i < 1000; i++)
            //{
            //    oStruct.Add(new XIDStructure { ID = i, sName = "Node"+i.ToString(), FKiParentID = "1515" });
            //}
            return Json(oStruct, JsonRequestBehavior.AllowGet);
        }

        /*::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        :::::::::::::::::::::::::::::::::::SearchType=100 & 80 bar cahrt 10 piechart::::::::::::::::::::::
        :::::::::::::::::::::::::::::::::::Adding Layout :::::::::::::::::::::::::::::::::::::::::::::: */
        [HttpPost]
        public void DefaultboDashboards(int BOID, string SearchType, string sType, string sSystemType)
        {
            try
            {
                //CResult oCResult = new CResult();
                //CResult oCR = new CResult();
                XIConfigs oXIConfig = new XIConfigs();
                oXIConfig.iBODID = BOID;
                XICore.XIBoDefaultDashboard ListCount = new XICore.XIBoDefaultDashboard();
                int i1ClickID = 0;
                var sRowXilinkID = string.Empty;
                if (sType == "True" && sSystemType != "0")
                {
                    /* get DashBoard count in XIBODashBoardCharts_T table */
                    var oCResult = oXIConfig.DashboardCount(BOID, SearchType);
                    ListCount = (XICore.XIBoDefaultDashboard)oCResult.oResult;
                    if (ListCount != null)
                    {
                        string DBName = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinDatabase"];
                        oCResult = oXIConfig.DefaultBoChartsXilink(sSystemType, SearchType, ListCount.iRowXilinkID);
                        if (oCResult.oResult != null)
                        {
                            sRowXilinkID = oCResult.oResult.ToString();
                        }
                        oCResult = oXIConfig.CreateXISystemDefaultBODashboards1Click(BOID, SearchType, sSystemType, sRowXilinkID, DBName);
                        if (oCResult.oResult != null)
                        {
                            i1ClickID = Convert.ToInt32(oCResult.oResult);
                        }
                        if (ListCount.sChartType == "AM-Charts")
                        {
                            if (sSystemType == "100" || sSystemType == "80")
                            {
                                ListCount.FKiComponentTypeID = XIConstant.DefaultAM4BarChartComponentID;
                            }
                            else if (sSystemType == "10")
                            {

                                ListCount.FKiComponentTypeID = XIConstant.DefaultAM4PieChartComponentID;
                            }
                        }
                        else
                        {
                            if (sSystemType == "100" || sSystemType == "80")
                            {
                                ListCount.FKiComponentTypeID = XIConstant.DefaultBarChartComponentID;
                            }
                            else if (sSystemType == "10")
                            {

                                ListCount.FKiComponentTypeID = XIConstant.DefaultPieChartComponentID;
                            }
                        }
                    }
                    ListCount.FKiOneClickID = i1ClickID;
                    ListCount.iRowXilinkID = Convert.ToInt32(sRowXilinkID);
                }
                oXIConfig.AddingChart2Layout(ListCount);

            }
            catch (Exception ex)
            {
                string sDatabase = SessionManager.CoreDatabase;
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
            }
        }

        [HttpPost]
        public ActionResult GetBOAttributeValue(string iBODID, string iBOIID)
        {
            XIIBO oBOI = new XIIBO();
            var oCR = oBOI.Get_BODialogLabel(iBODID.ToString(), iBOIID.ToString());
            //var BOD = (XIDBO)(oCache.GetObjectFromCache(XIConstant.CacheBO, null, iBODID.ToString()));
            var Res = (string)oCR.oResult;
            return Json(Res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AttributeChange(int BOID, string AttributeID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, null, BOID.ToString());
                if (AttributeID != "")
                {
                    var AttrF = oBOD.Attributes.Where(s => s.Value.ID == Convert.ToInt32(AttributeID)).Select(s => s.Value).FirstOrDefault();
                    return Json(AttrF, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetBOI(string iBOIID, string sBO)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                XIIXI oXI = new XIIXI();
                XIInfraCache oCache = new XIInfraCache();
                var BOI = oXI.BOI(sBO, iBOIID.ToString());
                if (BOI != null && BOI.Attributes.Count() > 0)
                {
                    var iBODID = BOI.AttributeI("FKiBODID").sValue;
                    var BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, iBODID);
                    return Json(BOD.Name, JsonRequestBehavior.AllowGet);
                }
                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetBOName(string iBODID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, iBODID);
                return Json(BOD.Name, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetDefaultPopup_Chat(string iBODID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                XIDBODefault oDefault = new XIDBODefault();
                oDefault = (XIDBODefault)oCache.GetObjectFromCache(XIConstant.CacheBODefault, null, iBODID.ToString());
                if (oDefault.iNannoXIGUID != null && oDefault.iNannoXIGUID != Guid.Empty)
                {
                    return Json(oDefault.iNannoXIGUID, JsonRequestBehavior.AllowGet);
                }
                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpPost]
        public ActionResult GetDefaultPopup(string iBODID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                XIDBODefault oDefault = new XIDBODefault();
                oDefault = (XIDBODefault)oCache.GetObjectFromCache(XIConstant.CacheBODefault, null, iBODID.ToString());
                if (oDefault.iPopupIDXIGUID != null && oDefault.iPopupIDXIGUID != Guid.Empty)
                {
                    return Json(oDefault.iPopupIDXIGUID, JsonRequestBehavior.AllowGet);
                }
                else if (oDefault.iPopupID > 0)
                {
                    return Json(oDefault.iPopupID, JsonRequestBehavior.AllowGet);
                }
                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult Get_DynamicDefaultPopup(string iBODID, string iBOIID)
        {

            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                XIIXI oXI = new XIIXI();
                var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, iBODID);
                var oBOI = oXI.BOI(oBOD.Name, iBOIID);
                if (oBOI != null && oBOI.Attributes.Count() > 0)
                {
                    var FKiBODID = oBOI.AttributeI("FKiBODID").sValue;
                    var FKiBOIID = oBOI.AttributeI("FKiBOIID").sValue;
                    var objectBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, FKiBODID);
                    var sBO = objectBOD.Name;
                    return Json(FKiBODID + ":" + FKiBOIID + ":" + sBO, JsonRequestBehavior.AllowGet);
                }
                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult Extract_Object()
        {
            XIConfigs oConf = new XIConfigs();
            var oCR = oConf.Extract_Object();
            return null;
        }

        public ActionResult Build_Object()
        {
            XIConfigs oConf = new XIConfigs();
            List<CNV> oParams = new List<CNV>();
            oParams.Add(new CNV { sName = XIConstant.Param_BO.ToLower(), sValue = "refComCategory" });
            oParams.Add(new CNV { sName = XIConstant.Param_InstanceID.ToLower(), sValue = "04C8C428-0B7A-48E9-A203-354ABA35D859" });
            var oCR = oConf.Build_BO(oParams);
            return null;
        }

        public ActionResult Import_File()
        {
            XIInfraImportComponent oImport = new XIInfraImportComponent();
            List<CNV> oParams = new List<CNV>();
            oParams.Add(new CNV { sName = XIConstant.Param_InstanceID.ToLower(), sValue = "4" });
            oImport.Import_File(oParams);
            return null;
        }

        public ActionResult Resolve_Notation()
        {
            XIIXI oIXI = new XIIXI();

            XIInfraCache oCache = new XIInfraCache();
            var TempD = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, "SubQS Notation", "0");
            var oQSSI = oIXI.BOI("QS Instance", "1366").Structure("subQSLoad").XILoad();
            var sVl = oQSSI.oSubStructureI("QS Instance").Item(0).ChildI("subQSDriver", "2").ChildI("subQSClaim", "0").XIIValue("sClaimCost").sDerivedValue;
            var sValue = oQSSI.oSubStructureI("QS Instance").Item(0).ChildI("subQSDriver", "1").ChildI("subQSConviction", "0").XIIValue("dWhendidtheconvictionhappenMSM").sDerivedValue;
            XIContentEditors oTemp = new XIContentEditors();
            var oCR = oTemp.MergeTemplateContent(TempD.FirstOrDefault(), oQSSI);
            return null;
        }

        public ActionResult Copy_SubQS()
        {
            var CopyParams = new List<CNV>();
            CopyParams.Add(new CNV { sType = "Recurrsive", sName = "Copy", sValue = "yes" });
            CopyParams.Add(new CNV { sType = "Recurrsive", sName = "BO", sValue = "QS Instance" });
            CopyParams.Add(new CNV { sType = "Recurrsive", sName = "Attribute", sValue = "iParentQSIID" });
            XIDStructure oStructure = new XIDStructure();
            var oCR = oStructure.StructureCopy("QS Instance", "92360", "subqscopy", CopyParams, true);
            return null;
        }

        public ActionResult Execute_Algorithm(string PCGUID, string BO, string BOIID, string CommID, string sGUID)
        {
            var sSessionID = HttpContext.Session.SessionID;
            //var iBODID = "4546";
            //var iBOIID = "430";
            //var ScriptID = "f8b8c9b7-32f0-46fa-8b79-6acd19a413e2";
            //var sGUID = Guid.NewGuid().ToString();
            XIDAlgorithm oAlogD = new XIDAlgorithm();
            XIInfraUsers oUsers = new XIInfraUsers();
            oAlogD = (XIDAlgorithm)oCache.GetObjectFromCache(XIConstant.CacheXIAlgorithm, null, PCGUID); //3033,2032
            List<CNV> oNVsList = new List<CNV>();
            oNVsList.Add(new CNV { sName = "-sBO", sValue = BO });
            oNVsList.Add(new CNV { sName = "-iBOIID", sValue = BOIID });
            oNVsList.Add(new CNV { sName = "-iCommID", sValue = CommID });
            oCache.SetXIParams(oNVsList, sGUID, sSessionID);
            var oCR = oAlogD.Execute_XIAlgorithm(sSessionID, sGUID);
            if (oCR.bOK && oCR.oResult != null)
            {
                var Messages = new Dictionary<string, string>();
                oCR = oUsers.Get_UserMessage(sGUID);
                if (oCR.bOK && oCR.oResult != null)
                {
                    Messages = (Dictionary<string, string>)oCR.oResult;
                }
                return Json(Messages, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("falied", JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult GetDataTreeData(string sInsID, string sCode, int iLevel, string ParentInsID, string sMode, string XIGUID, string BOIDXIGUID)
        {
           // var oStructD = (List<XIDStructure>)oCache.GetObjectFromCache(XIConstant.CacheStructure, sCode, "CDA88F22-6824-4B68-AC30-B1DF8EAD7B96");
            var oStructD = (List<XIDStructure>)oCache.GetObjectFromCache(XIConstant.CacheStructure, sCode, BOIDXIGUID);
			int iUserID = SessionManager.UserID;
            if (oStructD != null && oStructD.Count() > 0)
            {
                if (iLevel < oStructD.Count())
                {
                    if (!string.IsNullOrEmpty(sMode) && sMode.ToLower() == "node")
                    {
                        var GUID = Guid.Empty;
                        Guid.TryParse(XIGUID, out GUID);
                        var Childs = oStructD.Where(m => m.FKiParentIDXIGUID == GUID).ToList();
                        Childs.ForEach(m => m.sInsID = m.XIGUID.ToString());
                        return Json(Childs, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var NewNodeGUID = Guid.Empty;
                        Guid.TryParse(sInsID, out NewNodeGUID);
                        XIDStructure NextNode = new XIDStructure();
                        if (NewNodeGUID != null && NewNodeGUID != Guid.Empty && (NewNodeGUID.ToString().ToLower() == "538fc7cc-3ceb-4c35-80a7-0264aa96cda8" || sMode == "Multiple"))
                        {
                            NextNode = oStructD.Where(m => m.XIGUID == NewNodeGUID).FirstOrDefault();
                        }
                        else
                        {
                            NextNode = oStructD[iLevel];
                        }
                        var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BOIDXIGUID);
                        string sNameAttribute = oBOD.sNameAttribute;
                        if (!string.IsNullOrEmpty(sNameAttribute))
                        {
                            sNameAttribute = "sName";
                        }
                        var o1ClickXIGUID = NextNode.FKi1ClickIDXIGUID;
                        if (o1ClickXIGUID != null && o1ClickXIGUID != Guid.Empty)
                        {
                            XID1Click o1Click = new XID1Click();
                            XID1Click o1ClickC = new XID1Click();
                            o1Click = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, o1ClickXIGUID.ToString());
                            o1ClickC = (XID1Click)o1Click.Clone(o1Click);
                            string sParentWhere = NextNode.sParentFKColumn + "=" + ParentInsID;
                            o1ClickC.sParentWhere = sParentWhere;
                            Dictionary<string, XIIBO> oResult = new Dictionary<string, XIIBO>();
                            if (o1ClickC.IsStoredProcedure)
                            {
                                o1ClickC.NVs = o1ClickC.NVs ?? new List<CNV>();
                                o1ClickC.NVs.Add(new CNV { sName = "iUserID", sValue = Convert.ToString(iUserID) });
                                o1ClickC.NVs.Add(new CNV { sName = "iClientID", sValue = Convert.ToString(ParentInsID) });
                                o1ClickC.Query = o1ClickC.Query;
                                oResult = o1ClickC.OneClick_Run();
                            }
                            else
                            {
                                oResult = o1ClickC.OneClick_Run();
                            }
                            if (oResult != null && oResult.Count() > 0)
                            {
                                List<XIDStructure> oNVs = new List<XIDStructure>();
                                foreach (var boi in oResult.Values.ToList())
                                {
                                    var ID = boi.AttributeI("id").sValue;
									var sName = boi.AttributeI(sNameAttribute).sValue;
                                    //var sName = boi.AttributeI("sName").sValue;
                                    Guid GUID = Guid.Empty;
                                    var sGUID = boi.AttributeI("xiguid").sValue;
                                    Guid.TryParse(sGUID, out GUID);
                                    if (GUID == Guid.Empty)
                                    {
                                        GUID = Guid.NewGuid();
                                    }
                                    oNVs.Add(new XIDStructure { sName = sName, ID = Convert.ToInt32(ID), XIGUID = NextNode.XIGUID, sBO = NextNode.sBO, FKiStepDefinitionIDXIGUID = NextNode.FKiStepDefinitionIDXIGUID, sOutputArea = NextNode.sOutputArea, sMode = NextNode.sMode, sInsID = GUID.ToString(), sCSS = NextNode.sCSS, FKi1ClickIDXIGUID=NextNode.FKi1ClickIDXIGUID });
                                }
                                return Json(oNVs, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}