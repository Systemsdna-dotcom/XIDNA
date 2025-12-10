using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XIDNA.Models;
using XIDNA.ViewModels;
using XIDNA.Repository;
using System.IO;
using XIDNA.Common;
using XICore;

namespace XIDNA.Controllers
{
    public class WalletController : Controller
    {
        XIInfraUsers oUser = new XIInfraUsers();
        CommonRepository Common = new CommonRepository();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IWalletRepository WalletRepository;

        public WalletController() : this(new WalletRepository()) { }

        public WalletController(IWalletRepository WalletRepository)
        {
            this.WalletRepository = WalletRepository;
        }

        public ActionResult SendClientRequest(int LeadID, string Email, int OrgID, string OrgName, int ClassID)
        {
            try
            {
                //LeadRepository.SendRegisterMail(Email, "Register", OrgID, SessionManager.CoreDatabase, ClassID);
                WalletRequests Request = new WalletRequests();
                Request.LeadID = LeadID;
                Request.EmailID = Email;
                Request.OrganizationID = OrgID;
                Request.IsActivated = false;
                Request.FKiLeadClassID = 0;
                string database = SessionManager.CoreDatabase;
                var Res = WalletRepository.SaveWalletRequest(Request, database);
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        #region Products
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetWalletProductsList(jQueryDataTableParamModel param)
        {
            try
            {
                int OrgID = 0;
                string database = SessionManager.CoreDatabase;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = WalletRepository.GetWalletProductsList(param, OrgID, database);
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
                return null;
            }

        }

        public ActionResult AddWalletProduct()
        {
            try
            {
                WalletProducts Product = new WalletProducts();
                int OrgID = 0;
                Product.Classes = WalletRepository.GetOrgClasses(OrgID);
                Product.Templates = WalletRepository.TemplatesList(OrgID);
                return PartialView("_AddProductForm", Product);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AddWalletProdut(WalletProducts Product)
        {
            try
            {
                int OrgID = 0;
                string database = SessionManager.CoreDatabase;
                var Result = WalletRepository.AddWalletProdut(Product, OrgID, database);
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult UploadFiles(int id, HttpPostedFileBase UploadImage, HttpPostedFileBase UploadDoc)
        {
            try
            {
                int OrgID = 0;
                string database = SessionManager.CoreDatabase;
                if (UploadImage != null)
                {
                    string ext = Path.GetExtension(UploadImage.FileName);
                    string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                    string str = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\images";
                    logger.Info(str);
                    UploadImage.SaveAs(str + "\\ProductImg_" + id + ext);
                    var Image = "ProductImg_" + id + ext;
                    var res = WalletRepository.SaveImageOrDoc(id, Image, "Image", OrgID, database);
                }
                if (UploadDoc != null)
                {
                    string ext = Path.GetExtension(UploadDoc.FileName);
                    string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                    string str = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\images";
                    logger.Info(str);
                    UploadDoc.SaveAs(str + "\\ProductDoc_" + id + ext);
                    var Doc = "ProductDoc_" + id + ext;
                    var res = WalletRepository.SaveImageOrDoc(id, Doc, "Doc", OrgID, database);
                }
                return Json(1, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {

                logger.Error(ex);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditWalletProduct(int ID)
        {
            try
            {
                int OrgID = 0;
                string database = SessionManager.CoreDatabase;
                var Product = WalletRepository.GetWalletProductByID(ID, OrgID, database);
                Product.Classes = WalletRepository.GetOrgClasses(OrgID);
                Product.Templates = WalletRepository.TemplatesList(OrgID);
                return PartialView("_AddProductForm", Product);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        [HttpPost]
        public ActionResult IsExistsWalletProductName(string Name, int ID)
        {
            try
            {
                int OrgID = 0;
                string database = SessionManager.CoreDatabase;
                return WalletRepository.IsExistsWalletProductName(Name, ID, OrgID, database) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        #endregion Products

        #region Policies

        public ActionResult Policies()
        {
            return View();
        }
        public ActionResult GetWalletPoliciesList(jQueryDataTableParamModel param)
        {
            try
            {
                int OrgID = 0;
                string database = SessionManager.CoreDatabase;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = WalletRepository.GetWalletPoliciesList(param, OrgID, database);
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
                return null;
            }

        }
        //public ActionResult AddWalletPolicies(int LeadID)
        //{
        //    try
        //    {
        //        WalletPolicies Policy = new WalletPolicies();
        //        int OrgID = 0;
        //        string database = SessionManager.CoreDatabase;
        //        Policy.ProductList = WalletRepository.ProductList(OrgID, 0);
        //        Policy.Templates = WalletRepository.TemplatesList(OrgID);
        //        Policy.LeadID = LeadID;
        //        return PartialView("_AddPolicyForm", Policy);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex);
        //        return null;
        //    }
        //}
        public ActionResult AddWalletPolicies()
        {
            try
            {
                WalletPolicies Policy = new WalletPolicies();
                string sDatabase = SessionManager.CoreDatabase;
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                Policy.ProductList = WalletRepository.ProductList(iUserID, OrgID, 0, sDatabase, sOrgName);
                Policy.Templates = WalletRepository.TemplatesList(OrgID);
                return PartialView("_AddPolicyForm", Policy);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }
        [HttpPost]
        public ActionResult SaveWalletPolicy(WalletPolicies Policy)
        {
            try
            {
                int OrgID = 0;
                string database = SessionManager.CoreDatabase;
                Policy.OrganizationID = OrgID;
                var Result = WalletRepository.SaveWalletPolicy(Policy, OrgID, database);
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public ActionResult UploadPolicyFiles(int id, HttpPostedFileBase UploadImage, HttpPostedFileBase UploadDoc)
        {
            try
            {
                int OrgID = 0;
                string database = SessionManager.CoreDatabase;
                if (UploadImage != null)
                {
                    string ext = Path.GetExtension(UploadImage.FileName);
                    string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                    string str = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\images";
                    logger.Info(str);
                    UploadImage.SaveAs(str + "\\PolicyImg_" + id + ext);
                    var Image = "PolicyImg_" + id + ext;
                    var res = WalletRepository.SavePolicyImageOrDoc(id, Image, "Image", OrgID, database);
                }
                if (UploadDoc != null)
                {
                    string ext = Path.GetExtension(UploadDoc.FileName);
                    string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                    string str = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\images";
                    logger.Info(str);
                    UploadDoc.SaveAs(str + "\\PolicyDoc_" + id + ext);
                    var Doc = "PolicyDoc_" + id + ext;
                    var res = WalletRepository.SavePolicyImageOrDoc(id, Doc, "Doc", OrgID, database);
                }
                return Json(1, "text/plain");
            }

            catch (Exception ex)
            {

                logger.Error(ex);
                return Json(0, "text/plain");
            }
        }
        public ActionResult EditWalletPolicy(int ID)
        {
            try
            {
                int OrgID = 0;
                string database = SessionManager.CoreDatabase;
                var Policy = WalletRepository.EditWalletPolicy(ID, OrgID, database);
                Policy.ProductList = WalletRepository.GetProdList(OrgID);
                Policy.Templates = WalletRepository.TemplatesList(OrgID);
                return PartialView("_AddPolicyForm", Policy);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }
        [HttpGet]
        public ActionResult GetProductList(int ddlVal)
        {
            try
            {
                string sDatabase = SessionManager.CoreDatabase;
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                var sProductList = WalletRepository.ProductList(iUserID, OrgID, ddlVal, sDatabase, sOrgName);
                return Json(sProductList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }
        [HttpPost]
        public ActionResult IsExistsWalletPolicyName(string PolicyName, int ID)
        {
            try
            {
                int OrgID = 0;
                string database = SessionManager.CoreDatabase;
                return WalletRepository.IsExistsWalletPolicyName(PolicyName, ID, OrgID, database) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }
        #endregion Policies

        #region Quotes

        public ActionResult Quotes()
        {
            return View();
        }

        public ActionResult GetWalletQuotesList(jQueryDataTableParamModel param)
        {
            try
            {
                int OrgID = 0;
                string database = SessionManager.CoreDatabase;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = WalletRepository.GetWalletQuotesList(param, OrgID, database);
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
                return null;
            }

        }

        public ActionResult AddWalletQuote()
        {
            try
            {
                WalletQuotes Quote = new WalletQuotes();
                int OrgID = 0;
                Quote.ProductList = new List<VMDropDown>();
                Quote.Classes = WalletRepository.GetOrgClasses(OrgID);
                return PartialView("_AddQuoteForm", Quote);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AddWalletQuote(WalletQuotes model)
        {
            try
            {
                int OrgID = 0;
                string database = SessionManager.CoreDatabase;
                model.OrganizationID = OrgID;
                var Result = WalletRepository.AddWalletQuote(model, OrgID, database);
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UploadQuoteFiles(int id, HttpPostedFileBase UploadImage, HttpPostedFileBase UploadDoc)
        {
            try
            {
                int OrgID = 0;
                string database = SessionManager.CoreDatabase;
                if (UploadImage != null)
                {
                    string ext = Path.GetExtension(UploadImage.FileName);
                    string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                    string str = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\images";
                    logger.Info(str);
                    UploadImage.SaveAs(str + "\\QuoteImg_" + id + ext);
                    var Image = "QuoteImg_" + id + ext;
                    var res = WalletRepository.SaveQuoteImageOrDoc(id, Image, "Image", OrgID, database);
                }
                if (UploadDoc != null)
                {
                    string ext = Path.GetExtension(UploadDoc.FileName);
                    string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                    string str = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\images";
                    logger.Info(str);
                    UploadDoc.SaveAs(str + "\\QuoteDoc_" + id + ext);
                    var Doc = "QuoteDoc_" + id + ext;
                    var res = WalletRepository.SaveQuoteImageOrDoc(id, Doc, "Doc", OrgID, database);
                }
                return Json(1, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {

                logger.Error(ex);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditWalletQuote(int ID)
        {
            try
            {
                int OrgID = 0;
                string database = SessionManager.CoreDatabase;
                var Quote = WalletRepository.GetWalletQuoteByID(ID, OrgID, database);
                Quote.ProductList = new List<VMDropDown>();
                Quote.Classes = WalletRepository.GetOrgClasses(OrgID);
                return PartialView("_AddQuoteForm", Quote);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        [HttpPost]
        public ActionResult IsExistsWalletQuoteName(string Name, int ID)
        {
            try
            {
                int OrgID = 0;
                string database = SessionManager.CoreDatabase;
                return WalletRepository.IsExistsWalletProductName(Name, ID, OrgID, database) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        #endregion Quotes

        #region Orders

        public ActionResult Orders()
        {
            return View("WalletOrders");
        }

        public ActionResult GetOrdersList(jQueryDataTableParamModel param)
        {
            try
            {
                int OrgID = 0;
                string database = SessionManager.CoreDatabase;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = WalletRepository.GetOrdersList(param, OrgID, database);
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
                return null;
            }

        }

        #endregion Orders
    }
}