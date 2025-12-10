using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XIDNA.Repository;
using XIDNA.Models;
using XIDNA.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using XIDNA.Common;
using XICore;

namespace XIDNA.Controllers
{
    [Authorize]
    [SessionTimeout]
    public class FieldsController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IFieldsRepository FieldsRepository;

        public FieldsController() : this(new FieldsRepository()) { }

        public FieldsController(IFieldsRepository FieldsRepository)
        {
            this.FieldsRepository = FieldsRepository;
        }
        XIInfraUsers oUser = new XIInfraUsers();
        CommonRepository Common = new CommonRepository();
        //
        // GET: /Fields/
        public ActionResult AddClassFields(int? BID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                AddFields model = new AddFields();
                //model.Classes = Classes;
                model.OrgName = User.Identity.GetUserName().ToString();
                model.BID = BID;
                return View(model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult AddNonClassFields()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                AddFields model = new AddFields();
                model.OrgName = User.Identity.GetUserName().ToString();
                return View(model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult OrgAddNonClassFields()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                AddFields model = new AddFields();
                model.OrgName = User.Identity.GetUserName().ToString();
                string Type = "NonClassSpecific";
                return View();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult AddClassSpecificField()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                List<Classes> Classes = FieldsRepository.GetClasses(OrgID, sDatabase, iUserID, sOrgName);
                AddFields model = new AddFields();
                model.Classes = Classes;
                string OrgName = oUser.Role.sRoleName;
                model.OrgName = OrgName;
                if (OrgID != 0)
                {
                    string Name = OrgName.Substring(0, 3);
                    model.TableName = EnumLeadTables.Leads.ToString() + Name + OrgID;
                }
                else
                {
                    model.TableName = EnumLeadTables.Leads.ToString();
                }
                model.OrganizationID = OrgID;

                return PartialView("_AddClassFieldForm", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult AddOrgClassSpecificField()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                MappedFields model = new MappedFields();
                model.Classes = new List<Classes>();
                model.OrganizationID = OrgID;
                model.ddlBOs = Common.GetBOsDDL(sDatabase);
                return PartialView("_AddOrgClassFieldForm", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetBOClassesDDL(int BOID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var BOClasses = Common.GetBOClassesDDL(BOID, sDatabase);
                return Json(BOClasses, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpGet]
        public ActionResult SelectedFields(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
            oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
            if (OrgID == 0)
            {
                return PartialView("_SelectedClassFieldGrid", ID);
            }
            else
            {
                return PartialView("_SelectedOrgClassFieldsGrid", ID);
            }

        }
        [HttpGet]
        public ActionResult GetSelectedFields(jQueryDataTableParamModel param, int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = FieldsRepository.GetSelectedFields(param, ID, sDatabase, OrgID);
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

        public ActionResult GetSelectedOrgFields(jQueryDataTableParamModel param, int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = FieldsRepository.GetSelectedOrgFields(param, ID, sDatabase, OrgID, iUserID, sOrgName);
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
        public ActionResult AddNonClassSpecificField()
        {
            AddFields model = new AddFields();
            string sDatabase = SessionManager.CoreDatabase;
            int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
            oUser.UserID = iUserID;
            oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
            model.OrganizationID = oUser.FKiOrganisationID;
            return PartialView("_AddNonClassFieldForm", model);
        }
        public ActionResult AddOrgNonClassSpecificField()
        {
            MappedFields model = new MappedFields();
            string sDatabase = SessionManager.CoreDatabase;
            int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
            oUser.UserID = iUserID;
            oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
            model.OrganizationID = oUser.FKiOrganisationID;
            return PartialView("_AddOrgNonClassFieldForm", model);
        }
        [HttpGet]
        public ActionResult SelectedNonFields()
        {
            return PartialView("_SelectedNonClassFieldGrid");
        }
        public ActionResult SelectedOrgNonFields()
        {
            return PartialView("_SelectedOrgNonClassFieldGrid");
        }
        [HttpGet]
        public ActionResult SelectedNonClass(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = FieldsRepository.GetNonSelectedField(param, sDatabase, OrgID);
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

        public ActionResult SelectedOrgNonClass(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = FieldsRepository.GetOrgNonSelectedField(param, sDatabase, OrgID, iUserID, sOrgName);
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

        [HttpPost]
        public ActionResult SaveField(AddFields model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                model.OrganizationID = oUser.FKiOrganisationID;
                int fields = FieldsRepository.SaveField(model, sDatabase, iUserID, sOrgName);
                return Json(fields, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveOrgField(MappedFields model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                model.OrganizationID = oUser.FKiOrganisationID;
                var fields = FieldsRepository.SaveOrgField(model, sDatabase, iUserID, sOrgName);
                return Json(fields, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public int SaveEditedField(AddFields model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                model.OrganizationID = oUser.FKiOrganisationID;
                int fields = FieldsRepository.SaveField(model, sDatabase, iUserID, sOrgName);
                return fields;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return 0;
            }
        }

        [HttpPost]
        public ActionResult SaveOrgEditedField(MappedFields model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                model.OrganizationID = oUser.FKiOrganisationID;
                var fields = FieldsRepository.SaveOrgEditedField(model, sDatabase, iUserID, sOrgName);
                return Json(fields, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetOrgFields(string Type)
        {
            return PartialView("_FieldsList", Type);
        }
        public ActionResult GetClassOrgFields(string Type)
        {
            AddFields model = new AddFields();
            model.Type = Type;
            return PartialView("_ClassFieldsList", model);
        }

        public ActionResult GetFieldsList(jQueryDataTableParamModel param, string Type)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = FieldsRepository.GetFieldsList(param, Type, sDatabase, OrgID, iUserID, sOrgName);
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

        public ActionResult GetOrgNonClassFieldsList(jQueryDataTableParamModel param, string Type)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = FieldsRepository.GetOrgNonClassFieldsList(param, Type, sDatabase, OrgID, iUserID, sOrgName);
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

        public ActionResult EditField(int ColumnID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                AddFields model = FieldsRepository.EditField(ColumnID, OrgID, sDatabase, iUserID, sOrgName);
                model.OrgName = oUser.sUserName;
                return PartialView("_EditField", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult EditOrgField(int ColumnID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                MappedFields model = FieldsRepository.EditOrgField(ColumnID, OrgID, sDatabase, iUserID, sOrgName);
                List<Classes> Classes = FieldsRepository.GetClasses(OrgID, sDatabase, iUserID, sOrgName);
                //MappedFields model = new MappedFields();
                model.Classes = Classes;
                //model.OrgName = User.Identity.GetFirstName();
                return PartialView("_EditOrgField", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult IsExistsFieldName(string FieldName, int ID, int ClassID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                return FieldsRepository.IsExistsFieldName(FieldName, sDatabase, ID, ClassID, OrgID, iUserID, sOrgName) ? Json(true, JsonRequestBehavior.AllowGet)
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
        public ActionResult IsLengthChangable(string Length, int ID, string CreationType)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                return FieldsRepository.IsLengthChangable(Length, ID, sDatabase, OrgID, CreationType, iUserID, sOrgName) ? Json(true, JsonRequestBehavior.AllowGet)
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
        public ActionResult IsTypeChangable(string FieldType, int ID, string CreationType)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                return FieldsRepository.IsTypeChangable(FieldType, ID, sDatabase, OrgID, CreationType, iUserID, sOrgName) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult OrgnizationClassFields()
        {
            return View();
        }

        public ActionResult GetOrgClassFields(jQueryDataTableParamModel param, string Type, int ClassID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = FieldsRepository.GetOrgClassFields(param, Type, ClassID, sDatabase, OrgID, iUserID, sOrgName);
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

        //Fields Mapping
        public ActionResult OrganizationMapping()
        {
            return View("OrgMappedFields");
        }

        public ActionResult DisplayOrgMappedFields(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = FieldsRepository.DisplayOrgMappedFields(param, sDatabase, OrgID, iUserID, sOrgName);
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

        public ActionResult OrgMappedFieldsList(int ClassID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                var Model = FieldsRepository.ViewOrgMappedFields(ClassID, sDatabase, OrgID, iUserID, sOrgName);
                return PartialView("_OrgLeadFieldsGrid", Model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpPost]
        public ActionResult GetOrgMappedFields(int ClassID, string Type)
        {
            VMDropDown model = new VMDropDown();
            model.Value = ClassID;
            model.text = Type;
            return PartialView("_OrgMappedFieldsGrid", model);
        }

        [HttpPost]
        public ActionResult GetOrgLeadFields(int ClassID, string Category, string Type)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                var model = FieldsRepository.GetOrgLeadFields(ClassID, Type, Category, OrgID, sDatabase, iUserID, sOrgName);
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
        public ActionResult SaveOrgMappedLeadFields(int ClassID, string LeadField, string OrgField, int MasterID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                var res = FieldsRepository.SaveOrgMappedLeadFields(ClassID, LeadField, OrgField, MasterID, OrgID, sDatabase, iUserID, sOrgName);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult OrgMappedFieldsGrid(jQueryDataTableParamModel param, int ClassID, string Type)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = FieldsRepository.OrgMappedFieldsGrid(param, ClassID, Type, sDatabase, OrgID, iUserID, sOrgName);
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

        [HttpPost]
        public ActionResult DeleteMappedField(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                var result = FieldsRepository.DeleteMappedField(ID, sDatabase, iUserID, sOrgName);
                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult FieldsMapping()
        {
            string sDatabase = SessionManager.CoreDatabase;
            int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
            oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
            return View(OrgID);
        }

        public ActionResult DisplayDetails(jQueryDataTableParamModel param, string database)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = FieldsRepository.DisplayFields(param, sDatabase, OrgID, iUserID, sOrgName);
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
        public ActionResult EditMappedFields(int ClassID, string Type)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                var model = FieldsRepository.GetLeadFields(ClassID, Type, OrgID, sDatabase);
                model.OrganizationID = OrgID;
                model.ClassID = ClassID;
                model.Classes = FieldsRepository.GetAllOrgClasses(OrgID, sDatabase, iUserID, sOrgName);
                return PartialView("_EditMappedFields", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult MappedFieldsGrid(LeadMappings model, int ClsID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                var Model = FieldsRepository.MappedFieldsGrid(model, ClsID, sDatabase, OrgID);
                return PartialView("LeadFieldsGrid", Model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult IsExistsTemplateTitle(string Type, string Name, int ClassID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return FieldsRepository.IsExistsTemplateTitle(Type, Name, ClassID, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
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
        public ActionResult SaveMappedLeadFields(string Name, int ClsID, string DataField, string ColumnField)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                FieldsRepository.SaveMappedLeadFields(Name, ClsID, DataField, ColumnField, OrgID, sDatabase);
                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }


        public ActionResult GetMappedMasterFields(int ClassID)
        {
            return PartialView("_MappedMasterFieldsGrid", ClassID);
        }

        public ActionResult GetMappedMasterFieldsList(jQueryDataTableParamModel param, int ClassID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = FieldsRepository.GetMappedMasterFieldsList(param, ClassID, sDatabase);
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
        [HttpPost]
        public ActionResult DeleteMasterMappedField(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            var Result = FieldsRepository.DeleteMasterMappedField(ID, sDatabase);
            return null;
        }
        public ActionResult MapOrgFields()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                VMLeadMappings Mapping = new VMLeadMappings();
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                Mapping.ddlBOs = Common.GetBOsDDL(sDatabase);
                //Mapping.Classes = FieldsRepository.GetAllOrgClasses(OrgID, sDatabase);
                Mapping.Classes = new List<VMDropDown>();
                Mapping.MasterTypes = FieldsRepository.GetAllMasterTypes(sDatabase);
                return PartialView("_MapOrgFieldsForm", Mapping);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult EditMapOrgFields(int ClassID, int BOID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                var result = new VMLeadMappings();
                result.ExistingFields = new List<string>();
                result.NonClassFields = new List<string>();
                //result.Classes = FieldsRepository.GetAllOrgClasses(OrgID, sDatabase);
                result.Classes = Common.GetBOClassesDDL(BOID, sDatabase);
                result.ddlBOs = Common.GetBOsDDL(sDatabase);
                result.MasterTypes = FieldsRepository.GetAllMasterTypes(sDatabase);
                result.ClassID = ClassID;
                result.BOID = BOID;
                result.MappingType = "";
                return PartialView("_MapOrgFieldsForm", result);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetLeadFieldsForMapping()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                var Classes = FieldsRepository.GetAllOrgClasses(OrgID, sDatabase, iUserID, sOrgName);
                return PartialView("MapFieldsForm", Classes);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetLeadFields(int ClsID, string Type)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                var model = FieldsRepository.GetLeadFields(ClsID, Type, OrgID, sDatabase);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetClassTypes()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                var Model = FieldsRepository.GetClassTypes(OrgID, sDatabase);
                return Json(Model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
    }
}