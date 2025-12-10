using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XIDNA.Repository;
using XIDNA.Models;
using System.Data;
using System.Reflection;
using System.IO;
using XIDNA.Common;
using XICore;

namespace XIDNA.Controllers
{
    [Authorize]
    [SessionTimeout]
    public class MasterController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IMasterRepository MasterRepository;

        public MasterController() : this(new MasterRepository()) { }

        public MasterController(IMasterRepository MasterRepository)
        {
            this.MasterRepository = MasterRepository;
        }
        XIInfraUsers oUser = new XIInfraUsers();
        CommonRepository Common = new CommonRepository();
        //
        // GET: /Master/
        public ActionResult Index()
        {
            //var MasterData = MasterRepository.GetMasterData(OrgID, Database, PageIndex);
            return View();
        }
        public ActionResult GetSelectedFields(jQueryDataTableParamModel param, int Type)
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
                var result = MasterRepository.GetMasterDataList(param, Type, iUserID, sOrgName, sDatabase);
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

        public ActionResult CreateMasterData()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                Types model = new Types();
                model.Names = MasterRepository.GetAllNames(iUserID, sOrgName, sDatabase);

                return PartialView("_MasterDataForm", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetAllNames()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                var Names = MasterRepository.GetAllNames(iUserID, sOrgName, sDatabase);
                return Json(Names, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveMasterData(Types model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                var result = MasterRepository.SaveMasterData(model, iUserID, sOrgName, sDatabase);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult UploadFiles(int id, HttpPostedFileBase UploadFile)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                if (UploadFile != null)
                {
                    string ext = Path.GetExtension(UploadFile.FileName);
                    string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                    string str = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\css\\skins";
                    logger.Info(str);
                    UploadFile.SaveAs(str + "\\" + UploadFile.FileName);
                    var Image = "ProductImg_" + id + ext;
                    var res = MasterRepository.SaveMasterDataFile(id, UploadFile.FileName, sDatabase);
                }
                return Content(id.ToString(), "text/plain");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Content("0", "text/plain");
            }
        }

        public ActionResult EditMasterData(int DataID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                var result = MasterRepository.EditMasterData(DataID, sDatabase);
                result.Names = MasterRepository.GetAllNames(iUserID, sOrgName, sDatabase);
                return PartialView("_MasterDataForm", result);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpPost]
        public ActionResult IsExistsDataName(string Expression, int ID, int Code)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return MasterRepository.IsExistsDataName(Expression, ID, Code, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetTypeExpressions(int TypeID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                //var result = MasterRepository.GetTypeExpressions(TypeID);
                return PartialView("_ExpressionsList", TypeID);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult ContentOnScroll(int PageIndex)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                var GetTableDetails = MasterRepository.GetMasterData(OrgID, sDatabase, PageIndex);
                return Json(GetTableDetails, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult ErrorLog()
        {
            return View("ErrorLogList");
        }

        public ActionResult GetErrorsList(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = Common.GetErrorsList(param, sDatabase);
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
        public ActionResult ViewErrorMessage(int ErrorID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var model = Common.GetErrorByID(ErrorID, sDatabase);
                return PartialView("_ViewError", model);
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