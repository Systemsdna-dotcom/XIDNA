using System.Security.Claims;
using System.Threading;
using System.Web.Mvc;
using System.Linq;
using System;
using XIDNA.ViewModels;
using XIDNA.Repository;
using XIDNA.Models;
using XIDNA.Common;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;
using XICore;
using XISystem;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text;
using NPOI.SS.Formula.Eval;
using static iTextSharp.text.pdf.AcroFields;
using NPOI.SS.Formula.Functions;
using System.Data.Entity.Infrastructure;

namespace XIDNA.Controllers
{
    [Authorize]
    //[SessionTimeout]
    public class HomeController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IHomeRepository HomeRepository;

        public HomeController() : this(new HomeRepository())
        {

        }

        public HomeController(IHomeRepository HomeRepository)
        {
            this.HomeRepository = HomeRepository;
        }
        XIInfraUsers oUser = new XIInfraUsers();
        CommonRepository Common = new CommonRepository();
        XIInfraCache oCache = new XIInfraCache();

        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [AllowAnonymous]
        public ActionResult LandingPages(string XilinkId = null, string LeadID = null, string Userid = null)
        {
            string sDatabase = SessionManager.CoreDatabase;
            var sSessionID = HttpContext.Session.SessionID;
            try
            {
                var AppName = SessionManager.AppName;
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                if (oUser != null && oUser.UserID > 0)
                {

                }
                else
                {
                    //return RedirectToAction("Login", "Account");
                }
                
                int OrgID = oUser.FKiOrganisationID;
                XIDLayout oLayout = new XIDLayout();
                if (AppName.ToLower() == "zeeinsurance")
                {
                    //LoadEarly();
                }
                if (oUser.Role.sRoleName.ToLower() == EnumRoles.WebUsers.ToString().ToLower())
                {
                    XIInfraCache oCache = new XIInfraCache();
                    string sFunction = "xi.s|{if|{gt|{xi.a|'ACPolicy_T',{xi.p|-iUserID},'ID','','FKiUserID'},'0'},'true','false'}";
                    List<CNV> oNVList = new List<CNV>();
                    string sGUID = Guid.NewGuid().ToString();
                    CNV oParam = new CNV();
                    oParam.sName = "sFunction";
                    oParam.sValue = sFunction;
                    oNVList.Add(oParam);
                    oParam = new CNV();
                    oParam.sName = "-iUserID";
                    oParam.sValue = iUserID.ToString();
                    oNVList.Add(oParam);
                    oCache.SetXIParams(oNVList, sGUID, sSessionID);
                    CResult oCRes = new CResult();
                    XIDScript oXIScript = new XIDScript();
                    oXIScript.sScript = sFunction.ToString();
                    oCRes = oXIScript.Execute_Script(sGUID, sSessionID);
                    string sValue = string.Empty;
                    if (oCRes.bOK && oCRes.oResult != null)
                    {
                        sValue = (string)oCRes.oResult;
                        if (sValue == "false")
                        {
                            oUser.Role.iLayoutID = 2253;
                            Singleton.Instance.ActiveMenu = "Quotes";
                        }
                        else
                        {
                            Singleton.Instance.ActiveMenu = "Policies";
                        }
                    }

                    if (oUser.Role.iLayoutID > 0)
                    {
                        //var oLayDef = oXID.Get_LayoutDefinition(null, iLayoutID.ToString());
                        oLayout.ID = oUser.Role.iLayoutID;
                        //var oLayDef = oLayout.Load();
                        //if (oLayDef.bOK && oLayDef.oResult != null)
                        //{
                        //    oLayout = (XIDLayout)oLayDef.oResult;
                        //}
                        //var oLayDef = oDXI.Get_LayoutDefinition(null, oUser.Role.iLayoutID.ToString());
                        //var Layout = Common.GetLayoutDetails(oUser.Role.iLayoutID, 0, 0, 0, null, iUserID, sOrgName, sDatabase);
                        //SessionManager.sGUID = oLayDef.sGUID;
                        //oLayout = (XIDLayout)oLayDef.oResult;
                        oCache.Set_ParamVal(sSessionID, oLayout.sGUID, null, "{XIP|sUserName}", oUser.sFirstName, null, null);
                        oCache.Set_ParamVal(sSessionID, oLayout.sGUID, null, "{XIP|iUserID}", iUserID.ToString(), null, null);

                    }
                    //XIDStructure oXIDStructure = new XIDStructure();
                    //string sOneClickName = "Client Policy List for DropDown";
                    //XID1Click o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, sOneClickName, null);
                    ////o1ClickD.ReplaceFKExpressions(nParms);
                    ////o1ClickD.Query = oXIDStructure.ReplaceExpressionWithCacheValue(o1ClickD.Query, nParms);
                    //Dictionary<string, XIIBO> oRes = o1ClickD.OneClick_Execute();
                    //if (oRes != null && oRes.Count() > 0)
                    //{
                    //    var oBOIList = oRes.Values.ToList();
                    //    Dictionary<int, string> nPolicyDetails = new Dictionary<int, string>();
                    //    Session["PolicyCount"] = oBOIList.Count();
                    //    if (oBOIList != null && oBOIList.Count() > 0)
                    //    {
                    //        foreach (var oBOI in oBOIList)
                    //        {
                    //            int iInstanceID = 0; string sPolicyNo = string.Empty; string sRegistrationNo = string.Empty;
                    //            if (oBOI.Attributes.ContainsKey("id"))
                    //            {
                    //                iInstanceID = Convert.ToInt32(oBOI.AttributeI("id").sValue);
                    //            }
                    //            if (oBOI.Attributes.ContainsKey("sPolicyNo"))
                    //            {
                    //                sPolicyNo = oBOI.AttributeI("sPolicyNo").sValue;
                    //            }
                    //            if (oBOI.Attributes.ContainsKey("sRegNo"))
                    //            {
                    //                sRegistrationNo = oBOI.AttributeI("sRegNo").sValue;
                    //            }
                    //            nPolicyDetails[iInstanceID] = sPolicyNo + "_" + sRegistrationNo;
                    //        }
                    //        Session["PolicyCollection"] = nPolicyDetails;
                    //        if(nPolicyDetails!=null && nPolicyDetails.Count()>0)
                    //        {
                    //            Session["sRegistrationNo"] = nPolicyDetails.FirstOrDefault().Value.Split('_')[1];
                    //        }
                    //    }
                    //    else
                    //    {
                    //        //foreach (var oBOI in oBOIList)
                    //        //{
                    //        //    Session["iPolicyNo"] = Convert.ToInt32(oBOI.AttributeI("id").sValue);
                    //        //}
                    //    }
                    //}
                    if (string.IsNullOrEmpty(Userid))
                    {
                        ViewBag.XilinkId = XilinkId;
                        ViewBag.LeadID = LeadID;
                        ViewBag.sGUID = sGUID;
                    }
                    else
                    {
                        ViewBag.Userid = Userid;
                        ViewBag.XilinkId = XilinkId;
                        ViewBag.LeadID = LeadID;
                        ViewBag.sGUID = sGUID;
                    }
                    oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|sBOName}", "Lead_T", null, null);
                    oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|iInstanceID}", LeadID, null, null);

                    return View("UserPage", oLayout);
                }
                else
                {
                    if (oUser.Role.iLayoutID > 0 || (oUser.Role.iLayoutIDXIGUID != null && oUser.Role.iLayoutIDXIGUID != Guid.Empty))
                    {

                        var oLayDef = oCache.GetObjectFromCache(XIConstant.CacheLayout, null, oUser.Role.iLayoutIDXIGUID.ToString()); //oDXI.Get_LayoutDefinition(null, oUser.Role.iLayoutID.ToString());
                        //var Layout = Common.GetLayoutDetails(oUser.Role.iLayoutID, 0, 0, 0, null, iUserID, sOrgName, sDatabase);
                        //SessionManager.sGUID = oLayDef.sGUID;
                        if (oLayDef != null)
                        {
                            oLayout = (XIDLayout)oLayDef;
                        }
                        oCache.Set_ParamVal(sSessionID, oLayout.sGUID, null, "{XIP|sUserName}", oUser.sFirstName, "Default", null);
                        oCache.Set_ParamVal(sSessionID, oLayout.sGUID, null, "{XIP|iUserID}", iUserID.ToString(), "Default", null);
                    }
                    if (oUser.Role.sRoleName.ToLower() == EnumRoles.XISuperAdmin.ToString().ToLower() || oUser.Role.sRoleName.ToLower() == EnumRoles.OrgIDE.ToString().ToLower() || oUser.Role.sRoleName.ToLower() == EnumRoles.DeveloperStudio.ToString().ToLower() || oUser.Role.sRoleName.ToLower() == EnumRoles.AppAdmin.ToString().ToLower() || oUser.Role.sRoleName.ToLower() == EnumRoles.OrgAdmin.ToString().ToLower())
                    {
                        if ((oUser.Role.iLayoutID > 0 || (oUser.Role.iLayoutIDXIGUID != null && oUser.Role.iLayoutIDXIGUID != Guid.Empty)) && oLayout != null)
                        {
                            return View("LandingPage", oLayout);
                        }
                        else
                        {
                            return RedirectToAction("Index", "XIApplications");
                        }
                    }
                    else if (oUser.Role.sRoleName.ToLower() == EnumRoles.SuperAdmin.ToString().ToLower())
                    {
                        return RedirectToAction("Index", "QueryGeneration");
                    }
                    else if (oUser.Role.sRoleName.ToLower() == EnumRoles.Admin.ToString().ToLower())
                    {
                        return RedirectToAction("Index", "QueryGeneration");
                    }
                    else
                    {
                        //var oLayDef = oLayout.Load();
                        //if (oLayDef.bOK && oLayDef.oResult != null)
                        //{
                        //    oLayout = (XIDLayout)oLayDef.oResult;
                        //}
                        return View("InternalUsers", oLayout);
                    }
                }

            }
            catch (Exception ex)
            {
                //logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //[HttpPost]
        [AllowAnonymous]
        public ActionResult Page()
        {
            var sHomePage = SessionManager.sHomePage;
            return View(sHomePage);
        }

        private void LoadEarly()
        {
            XIInfraCache oCache = new XIInfraCache();
            Thread threadObj = new Thread(new ThreadStart(() => { oCache.GetObjectFromCache(XIConstant.CacheBO, "ACPolicy_T"); }));
            threadObj.SetApartmentState(ApartmentState.MTA);
            threadObj.IsBackground = true;
            threadObj.Start();
            Thread threadObj1 = new Thread(new ThreadStart(() => { oCache.GetObjectFromCache(XIConstant.CacheBO, "Policy Version"); }));
            threadObj1.SetApartmentState(ApartmentState.MTA);
            threadObj1.IsBackground = true;
            threadObj1.Start();
            Thread threadObj2 = new Thread(new ThreadStart(() => { oCache.GetObjectFromCache(XIConstant.CacheBO, "QS Instance"); }));
            threadObj2.SetApartmentState(ApartmentState.MTA);
            threadObj2.IsBackground = true;
            threadObj2.Start();
            Thread threadObj3 = new Thread(new ThreadStart(() => { oCache.GetObjectFromCache(XIConstant.CacheBO, "Driver_T"); }));
            threadObj3.SetApartmentState(ApartmentState.MTA);
            threadObj3.IsBackground = true;
            threadObj3.Start();
            Thread threadObj4 = new Thread(new ThreadStart(() => { oCache.GetObjectFromCache(XIConstant.CacheBO, "Claim_T"); }));
            threadObj4.SetApartmentState(ApartmentState.MTA);
            threadObj4.IsBackground = true;
            threadObj4.Start();
            Thread threadObj5 = new Thread(new ThreadStart(() => { oCache.GetObjectFromCache(XIConstant.CacheBO, "Conviction_T"); }));
            threadObj5.SetApartmentState(ApartmentState.MTA);
            threadObj5.IsBackground = true;
            threadObj5.Start();
            Thread threadObj6 = new Thread(new ThreadStart(() => { oCache.GetObjectFromCache(XIConstant.CacheBO_All, "ACPolicy_T"); }));
            threadObj6.SetApartmentState(ApartmentState.MTA);
            threadObj6.IsBackground = true;
            threadObj6.Start();
        }

        #region Menu Management

        public ActionResult EditRootMenu(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                ViewBag.DetailsID = ID;
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                RightMenuTrees model = HomeRepository.EditRootMenu(ID, OrgID, iUserID, sDatabase);
                if (OrgID == 0)
                {
                    model.Organisations.Insert(0, new VMDropDown { text = "Super Admin", Value = 0 });
                }
                else
                {
                    model.Organisations = model.Organisations.Where(m => m.Value == OrgID).ToList();
                }
                return PartialView("_EditMenuForm", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult DeleteRootMenu(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                int iStatus = HomeRepository.DeleteRootMenu(ID, OrgID, iUserID, sDatabase);
                return Json(iStatus, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //25/11/2017
        public ActionResult MenuWithTree()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                RightMenuTrees Model = HomeRepository.GetOrganisation(OrgID, iUserID, sOrgName, sDatabase);
                if (OrgID == 0)
                {
                    Model.Organisations.Insert(0, new VMDropDown { text = "Super Admin", Value = 0 });
                }
                else
                {
                    Model.Organisations = Model.Organisations.Where(m => m.Value == OrgID).ToList();
                }
                return PartialView("_MenuWithTree", Model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult SaveMenuTreeDetails(string RootNode, string ParentNode, string NodeID, string NodeTitle, string Type, int ID = 0, int iRoleID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                int DBstatus = HomeRepository.SaveMenuTreeDetails(ID, RootNode, ParentNode, NodeID, NodeTitle, Type, iRoleID, iUserID, OrgID, sDatabase, sOrgName);
                return Json(DBstatus, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        //public ActionResult GetMenuTreeDetails()
        //{
        //    int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
        //    oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
        //    List<RightMenuTrees> lResult = HomeRepository.GetMenuTreeDetails(iUserID, OrgID, sDatabase);
        //    return Json(lResult, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult DeleteNodeDetails(string ParentNode, string NodeID, string ChildrnIDs, string Type, int iRoleID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                List<RightMenuTrees> lResult = HomeRepository.DeleteNodeDetails(ParentNode, NodeID, ChildrnIDs, Type, iRoleID, iUserID, OrgID, sDatabase);
                return Json(lResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        //26/10/2017
        public ActionResult AddDetailsForMenu(string ParentNode, string NodeID, int iRoleID, string DetailsID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                //ViewBag.DetID = DetailsID;
                ViewBag.DetID = NodeID;
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                RightMenuTrees model = HomeRepository.AddDetailsForMenu(ParentNode, NodeID, iRoleID, OrgID, iUserID, sDatabase);
                //var AllXiLinkLists = (XILink)oDXI.Get_XILinkDefinition(0).oResult;
                //model.VMXILink = AllXiLinkLists.XiLinkDDLs.Select(m => new VMDropDown { text = m.Expression, Value = m.ID }).ToList();
                model.VMXILink = Common.GetXiLinksDDL(sDatabase);
                //ModelDbContext dbContext = new ModelDbContext();
                Dictionary<string, string> XiLinks = new Dictionary<string, string>();
                //var lXiLinks = dbContext.XiLinks.Where(m => m.FKiApplicationID == UserDetais.FKiApplicationID).ToList();
                foreach (var items in model.VMXILink)
                {
                    XiLinks[items.Value.ToString()] = items.text;
                }
                model.XILinks = XiLinks;
                return PartialView("_AddDetailsForMenu", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult SaveAddedDetails(RightMenuTrees model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                //if(model.XiLinkID==0)
                //{
                //    model.ActionType = 10;
                //}
                //else
                //{
                //    model.ActionType = 20;
                //}
                var Status = HomeRepository.SaveAddedDetails(iUserID, model, sDatabase);
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
        public ActionResult SaveEditedMenuDetails(int RoleID, int OrgID, string NewRootName, string OldRootName)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iStatus = HomeRepository.SaveEditedMenuDetails(RoleID, OrgID, NewRootName, OldRootName, sDatabase);
                return Json(iStatus, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult ShowMenuTreeDetails()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                List<RightMenuTrees> Models = HomeRepository.ShowMenuTreeDetails(iUserID, OrgID, sOrgName, sDatabase);
                return Json(Models, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        //public ActionResult GetChildForMenu(int ID)
        //{
        //    try
        //    {
        //        oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
        //        List<RightMenuTrees> Models = HomeRepository.GetChildForMenu(ID, OrgID);
        //        return Json(Models, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex);
        //        Common.SaveErrorLog(ex.ToString(), sDatabase);
        //        return null;
        //    }
        //}

        //public ActionResult GetXILinkDetails(int XilinkID)
        //{
        //    oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
        //    string sDatabase = SessionManager.CoreDatabase;
        //    int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
        //    VMXiLinks Details = HomeRepository.GetXILinkDetails(XilinkID, iUserID, OrgID, sDatabase);
        //    return Json(Details, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult ShowRightMenu()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                var model = HomeRepository.ShowMenuTreeDetails(iUserID, OrgID, sOrgName, sDatabase);
                return PartialView("_HomeRightMenu", model);
            }
            catch (Exception ex)
            {
                if (SessionManager.CoreDatabase == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }

        //public ActionResult DragAndDropNodes(string NodeID, string OldParentID, string NewParentID)
        //{
        //    oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
        //    string sDatabase = SessionManager.CoreDatabase;
        //    int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
        //    int Status = HomeRepository.DragAndDropNodes(NodeID, OldParentID, NewParentID, iUserID, OrgID, sDatabase);
        //    return Json(Status, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult DragAndDropNodes(string NodeID, string OldParentID, string NewParentID, int Oldposition, int Newposition)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                var Tab = HomeRepository.DragAndDropNodes(NodeID, OldParentID, NewParentID, iUserID, OrgID, sDatabase, Oldposition, Newposition);
                return Json(Tab, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult IDEGetRolesForMenu(string OrgName, int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                XIMenu oXIM = new XIMenu();
                oXIM.sCoreDatabase = SessionManager.CoreDatabase;
                oXIM.OrgID = OrgID;
                var List = oXIM.Get_XIRolesDDL();
                List<XIDropDown> oRoleName = new List<XIDropDown>();
                if (List.bOK && List.oResult != null)
                {
                    oRoleName = (List<XIDropDown>)List.oResult;
                }
                return Json(List, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //25/11/2017
        public ActionResult GetRolesForMenu(string OrgName, int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                List<VMDropDown> sRoleName = HomeRepository.GetRolesForMenu(OrgName, OrgID, sDatabase);
                return Json(sRoleName, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpPost]
        public ActionResult IsExistsRoot(int ID, string RootName, int OrgID, int RoleID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return HomeRepository.IsExistsRootName(ID, RootName, OrgID, RoleID, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
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
        public ActionResult SaveMenuDetails(int OrgID, string RootName, int RoleID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID;
                var Result = HomeRepository.SaveMenuDetails(iUserID, RoleID, OrgID, RootName, sDatabase);
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult GetMenuTreeDetails(string RootName, int OrgID, int RoleID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                List<RightMenuTrees> lResult = HomeRepository.GetMenuTreeDetails(RootName, OrgID, RoleID, sDatabase);
                return Json(lResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult GetChildForRootMenu(string NodeID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                List<RightMenuTrees> Model = HomeRepository.GetChildForRootMenu(NodeID, iUserID, OrgID, sDatabase);
                return Json(Model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult DisplayMenuDetails(string sType)
        {
            //var sDatabase = sDatabase;
            ViewBag.sType = sType;
            return View();
        }

        public ActionResult GetMenuDetails(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = HomeRepository.GetMenuDetails(param, iUserID, sOrgName, OrgID, sDatabase);
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

        #endregion Menu Management

        #region CacheDetails

        public ActionResult GetCachedDetails()
        {
            List<XIInfraCache> oCacheList = new List<XIInfraCache>();
            try
            {
                var oCurrentCache = System.Web.HttpContext.Current.Cache;
                if (oCurrentCache != null)
                {
                    var oCachedEnumr = oCurrentCache.GetEnumerator();
                    while (oCachedEnumr.MoveNext())
                    {
                        if (oCachedEnumr.Key.ToString().Contains("bo"))
                        {
                            XIInfraCache oCache = new XIInfraCache();
                            oCache.sKey = oCachedEnumr.Key.ToString();
                            oCache.oCachedObject = oCurrentCache[oCachedEnumr.Key.ToString()] == null ? "" : oCurrentCache[oCachedEnumr.Key.ToString()].ToString();
                            //oCache.sSize = "0";
                            var sSingleKey = oCachedEnumr;
                            string json = JsonConvert.SerializeObject(sSingleKey, Newtonsoft.Json.Formatting.Indented);
                            var bf = new BinaryFormatter();
                            var ms = new MemoryStream();
                            bf.Serialize(ms, json);
                            oCache.sSize = ms.Length.ToString() + " bytes";
                            oCacheList.Add(oCache);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView("_CacheInformation", oCacheList);
        }
        public string CacheClear(string sKey)
        {
            var sAppName = SessionManager.AppName;
            //if (!string.IsNullOrEmpty(sAppName))
            //{
            //    sKey = sAppName + "_" + sKey + "_";
            //}
            sKey = sKey + "_";
            var oCurrentCache = System.Web.HttpContext.Current.Cache;
            if (oCurrentCache != null)
            {
                //List<string> CacheRemove = new List<string>();
                var oCachedEnumr = oCurrentCache.GetEnumerator();
                while (oCachedEnumr.MoveNext())
                {
                    if (!string.IsNullOrEmpty(sKey))
                    {
                        if (oCachedEnumr.Key.ToString().StartsWith(sKey))
                        {
                            //CacheRemove.Add(oCachedEnumr.Key.ToString());
                            oCurrentCache.Remove(oCachedEnumr.Key.ToString());
                        }
                    }
                }
                //foreach (string CRemove in CacheRemove)
                //{
                //    oCurrentCache.Remove(CRemove);
                //}
            }
            return null;
        }

        public ActionResult ClearCache()
        {
            return PartialView("_CacheInformation");
        }

        #endregion CacheDetails


        #region AddRolesToMenus

        //Getting All the Menus
        public ActionResult RoleMenusTree()
        {
            string sDatabase = SessionManager.CoreDatabase;
            ModelDbContext dbContext = new ModelDbContext();
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                List<XIRoleMenu> Menus = new List<XIRoleMenu>();
                XIRoleMenu oMenu = new XIRoleMenu();
                XIMenu lMenu = new XIMenu();
                lMenu.sCoreDatabase = sDatabase;
                Menus = oMenu.Get_AllRoleMenuTreeDetails(); //For Role Based All Menus
                var oRolesList = (List<XIDropDown>)lMenu.Get_XIRolesDDL().oResult;
                oMenu.Roles = oRolesList.ToList().Select(m => new XIDropDown { text = m.text, Value = m.Value }).ToList();
                ViewBag.Menus = Menus;
                var GroupMenus = dbContext.XIMenuMappings.Where(m => m.ParentID != "#").ToList();
                //if (GroupMenus.Count() > 0)
                //{
                //    oMenu.GroupIDs = GroupMenus.Select(m => Convert.ToInt32(m.FKiMenuID)).Distinct().ToList();
                //}
                return View(oMenu);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //Saving Checked Menus into Database
        [HttpPost]
        public XIMenuMappings oXIMenusParams(List<XIMenuMappings> oMenuParams, string RootName, int iRoleID, string RoleName)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int iOrgID = oUser.FKiOrganisationID;
                var oMenu = HomeRepository.SaveRoleMappings(oMenuParams, RootName, iRoleID, RoleName, iOrgID, iUserID, sDatabase);
                return oMenu;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //For Getting Mapped MenuID's From Database
        [HttpPost]
        public ActionResult GetRoleMenusTree(string RootName, int iRoleID)
        {
            ModelDbContext dbContext = new ModelDbContext();
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int iOrgID = oUser.FKiOrganisationID;
                List<int> GroupIDs = new List<int>();
                var GroupMenus = dbContext.XIMenuMappings.Where(m => m.ParentID != "#").ToList();
                //if (GroupMenus.Count() > 0)
                //{
                //    GroupIDs = GroupMenus.Select(m => Convert.ToInt32(m.FKiMenuID)).Distinct().ToList();
                //}
                return Json(GroupIDs, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //Adding New Node
        public ActionResult AddTreeNode(XIRoleMenus node)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase, iUserID).oResult; int iOrgID = oUser.FKiOrganisationID; int fkiApplicationID = oUser.FKiApplicationID;
                node.FKiApplicationID = oUser.FKiApplicationID;
                var role = HomeRepository.AddTreeNode(node, sOrgName, iOrgID, iUserID, sDatabase);
                return Json(role, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //Creation and Rename of Menu
        public ActionResult CreateandRenameMenu(XIRoleMenus RootNode)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int iOrgID = oUser.FKiOrganisationID;
                RootNode.FKiApplicationID = oUser.FKiApplicationID;
                int dbStatus = HomeRepository.CreateandRenameMenu(RootNode, iOrgID, iUserID, sDatabase);
                return Json(dbStatus, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        //Deleting the Menu by ID
        public ActionResult DeleteTreeMenu(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int iOrgID = oUser.FKiOrganisationID;
                int dbStatus = HomeRepository.DeleteTreeMenu(ID);
                return Json(dbStatus, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        #endregion AddRolesToMenus

        public ActionResult UserLevelCache(string sKey)
        {
            CResult oCR = new CResult();
            XIDefinitionBase oDef = new XIDefinitionBase();
            List<VM_UserCacheKeyValue> oCacheKeyValue = new List<VM_UserCacheKeyValue>();
            try
            {
                var oUserCache = HttpRuntime.Cache["XICache"];
                var oCacheobj = (XICacheInstance)oUserCache;
                if (oCacheobj != null)
                {
                    var sSessionDetails = oCacheobj.NMyInstance;
                    var sUDetails = sSessionDetails.FirstOrDefault().Value.NMyInstance[sKey].NMyInstance;
                    //var sUDetails = sSessionDetails.FirstOrDefault().Value.NMyInstance.FirstOrDefault().Value.NMyInstance;
                    foreach (var items in sUDetails)
                    {
                        var sGUID = string.Empty;
                        if (items.Key.StartsWith("UID_"))
                        {
                            sGUID = items.Key.Replace("UID_", "");
                        }
                        foreach (var item in items.Value.NMyInstance)
                        {
                            var lVals = new VM_UserCacheKeyValue();
                            if (item.Key != "sSessionID" && item.Key != "sGUID")
                            {
                                lVals.sKey = item.Key;
                                lVals.sValue = item.Value.sValue;
                                lVals.sGUID = sGUID;
                                oCacheKeyValue.Add(lVals);
                            }
                        }
                    }
                }
                return View("_HtmlCacheList", oCacheKeyValue);
            }
            catch (Exception ex)
            {
                oCR.sMessage = "Home Controller:Search_Cache Method-" + ex.ToString();
                oCR.sCategory = "Manual DB query operation";
                oDef.SaveErrortoDB(oCR);
                return View("_HtmlCacheList", oCacheKeyValue);
            }
        }
            [HttpPost]
            public ActionResult Search_Cache(string sKey, string sGUID, string sParam)
            {
                CResult oCR = new CResult();
                XIDefinitionBase oDef = new XIDefinitionBase();
                List<VM_UserCacheKeyValue> oCacheKeyValue = new List<VM_UserCacheKeyValue>();
                try
                {
                    var oUserCache = HttpRuntime.Cache["XICache"];
                    var oCacheobj = (XICacheInstance)oUserCache;
                    if (oCacheobj != null)
                    {
                        var sSessionDetails = oCacheobj.NMyInstance;
                        if (!string.IsNullOrEmpty(sGUID) && !string.IsNullOrEmpty(sParam))
                        {
                            var sParamVal = sSessionDetails.FirstOrDefault().Value.NMyInstance[sKey].NMyInstance["UID_" + sGUID].NMyInstance[sParam].sValue;
                            var lVals = new VM_UserCacheKeyValue();
                            lVals.sKey = sParam;
                            lVals.sValue = sParamVal;
                            lVals.sGUID = sGUID;
                            oCacheKeyValue.Add(lVals);
                        }
                        else
                        {
                            var sUDetails = sSessionDetails.FirstOrDefault().Value.NMyInstance[sKey].NMyInstance["UID_" + sGUID].NMyInstance;
                            //var sUDetails = sSessionDetails.FirstOrDefault().Value.NMyInstance.FirstOrDefault().Value.NMyInstance;
                            foreach (var items in sUDetails)
                            {
                                if (items.Key != "sSessionID" && items.Key != "sGUID")
                                {
                                    var lVals = new VM_UserCacheKeyValue();
                                    lVals.sKey = items.Key;
                                    lVals.sValue = items.Value.sValue;
                                    lVals.sGUID = sGUID;
                                    oCacheKeyValue.Add(lVals);
                                }
                            }
                        }

                    }
                    return View("_HtmlCacheList", oCacheKeyValue);
                }
                catch (Exception ex)
                {
                    oCR.sMessage = "Home Controller:Search_Cache Method-" + ex.ToString();
                    oCR.sCategory = "Manual DB query operation";
                    oDef.SaveErrortoDB(oCR);
                    return View("_HtmlCacheList", oCacheKeyValue);
                }
            }

            [AllowAnonymous]
        public ActionResult RetrieveDetails()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var sSessionID = HttpContext.Session.SessionID;
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                if (iUserID > 0)
                {
                    oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                    XIDLayout oLayout = new XIDLayout();
                    LoadEarly();
                    if (oUser.Role.sRoleName.ToLower() == EnumRoles.WebUsers.ToString().ToLower())
                    {
                        oUser.Role.iLayoutID = 2253;
                        Singleton.Instance.ActiveMenu = "Quotes";
                        if (oUser.Role.iLayoutID > 0)
                        {
                            oLayout.ID = oUser.Role.iLayoutID;
                            oCache.Set_ParamVal(sSessionID, oLayout.sGUID, null, "{XIP|sUserName}", oUser.sFirstName, null, null);
                            oCache.Set_ParamVal(sSessionID, oLayout.sGUID, null, "{XIP|iUserID}", iUserID.ToString(), null, null);
                        }
                        return View("UserPage", oLayout);
                    }
                }
                else
                {
                    return RedirectToAction("ClientLogin", "Account");
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

        [AllowAnonymous]
        public ActionResult RequestHandler()
        {
            var sServerKey = System.Configuration.ConfigurationManager.AppSettings["ServerKey"];
            //Common.SaveErrorLog("Request forwarded to " + sServerKey, "");
            return Json(sServerKey, JsonRequestBehavior.AllowGet);
        }

        #region AssignMenu

        public ActionResult AssignMenu(string RootNode, string ParentNode, string NodeID, string NodeTitle, string Type, int ID = 0, int iRoleID = 0, int iOrgID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID;
                XIAssignMenu oAs = new XIAssignMenu();
                SignalR oSignalR = new SignalR();
                XIConfigs oConfig = new XIConfigs(oSignalR);
                oConfig.iUserID = iUserID;
                oConfig.sSessionID = HttpContext.Session.SessionID;
                if (ID == 0)
                {
                    oAs.ID = ID;
                    oAs.Name = NodeTitle;
                    oAs.RootName = RootNode;
                    oAs.RoleID = iRoleID;
                    oAs.sType = "AssignMenu";
                    oAs.OrgID = iOrgID;
                    if (ParentNode == "#")
                    {
                        oAs.ParentID = "#";
                        oAs.Name = RootNode;
                    }
                }
                else if (ID > 0)
                {
                    oAs = oAs.Get_AssignedTreeDetails(ID);
                    oAs.sType = "AssignMenu";
                    oAs.ParentID = ParentNode;
                    if (Type == "rename")
                    {
                        oAs.Name = NodeTitle;
                    }
                    else if (Type == "Assign")
                    {
                        //Passing FKiMenuID Value through NodeID
                        oAs.FKiMenuID = Convert.ToInt32(NodeID);
                    }
                }
                var oMenuDef = oConfig.Save_AssignMenu(oAs);
                if (oMenuDef.bOK && oMenuDef.oResult != null)
                {
                    int iID = 0;
                    var oMDef = (XIIBO)oMenuDef.oResult;
                    var sMenuID = oMDef.Attributes.Where(m => m.Key.ToLower() == "id").Select(m => m.Value).Select(m => m.sValue).FirstOrDefault();
                    int.TryParse(sMenuID, out iID);
                    return Json(iID);
                }
                return Json(0);
            }
            catch (Exception ex)
            {
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult GetAssignedMenu(int ID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            XIAssignMenu oAs = new XIAssignMenu();
            try
            {
                int iUserID = SessionManager.UserID;
                SignalR oSignalR = new SignalR();
                XIConfigs oConfig = new XIConfigs(oSignalR);
                oConfig.iUserID = iUserID;
                oConfig.sSessionID = HttpContext.Session.SessionID;
                if (ID > 0)
                {
                    var oAsDef = oAs.Get_AssingedMenuDetails(ID);
                    if (oAsDef.bOK && oAsDef.oResult != null)
                    {
                        oAs = (XIAssignMenu)oAsDef.oResult;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(oAs, JsonRequestBehavior.AllowGet);
            }
            return Json(oAs, JsonRequestBehavior.AllowGet);
        }

        #endregion AssignMenu

        #region MultiOrg

        public ActionResult Get_Organisations()
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            List<CNV> nOrg = new List<CNV>();
            try
            {
                var iUserID = SessionManager.UserID;
                oTrace.oParams.Add(new CNV { sName = "iUserID", sValue = iUserID.ToString() });
                if (iUserID > 0)//check mandatory params are passed or not
                {
                    XIIXI oXI = new XIIXI();
                    XID1Click o1Click = new XID1Click();
                    o1Click.BOID = 1302;
                    o1Click.Query = "Select * from XIUserOrgMapping_T where FKiUserID=" + iUserID;
                    var Data = o1Click.OneClick_Run();
                    if (Data != null && Data.Count() > 0)
                    {
                        foreach (var BOI in Data.Values)
                        {
                            var OrgID = BOI.AttributeI("FKiOrgID").sValue;
                            var Org = oXI.BOI("Organisations", OrgID);
                            if (Org != null && Org.Attributes.Count() > 0)
                            {
                                var OrgName = Org.AttributeI("name").sValue;
                                nOrg.Add(new CNV { sName = OrgName, sValue = OrgID });
                            }
                        }
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: iUserID is missing";
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
            return Json(nOrg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Landing(int ID)
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
                oTrace.oParams.Add(new CNV { sName = "ID", sValue = ID.ToString() });
                var sDatabase = SessionManager.CoreDatabase;
                var iUserID = SessionManager.UserID;
                List<CNV> WhrPrms = new List<CNV>();
                WhrPrms.Add(new CNV { sName = "FKiUserID", sValue = iUserID.ToString() });
                WhrPrms.Add(new CNV { sName = "FKiOrgID", sValue = ID.ToString() });
                SessionManager.iUserOrg = ID;
                var RoleID = string.Empty;
                XIIXI oXI = new XIIXI();
                var oBOI = oXI.BOI("XIUserOrgMapping", null, null, WhrPrms);
                if (oBOI != null && oBOI.Attributes.Count() > 0)
                {
                    RoleID = oBOI.AttributeI("FKiRoleID").sValue;
                    var OrgID = oBOI.AttributeI("FKiOrgID").sValue;
                }
                XIInfraRoles oRoleD = new XIInfraRoles();
                oRoleD.RoleID = Convert.ToInt32(RoleID);
                oCR = oRoleD.Get_RoleDefinition(sDatabase);
                oRoleD = (XIInfraRoles)oCR.oResult;
                var oLayDef = oCache.GetObjectFromCache(XIConstant.CacheLayout, null, oRoleD.iLayoutID.ToString()); //oDXI.Get_LayoutDefinition(null, oUser.Role.iLayoutID.ToString());
                XIDLayout oLayout = new XIDLayout();
                if (oLayDef != null)
                {
                    oLayout = (XIDLayout)oLayDef;
                    if (oLayout != null && oLayout.ID > 0)
                    {
                        return View("LandingPage", oLayout);
                    }
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
            return null;
        }

        #endregion MultiOrg


        #region MultiApp

        public ActionResult Get_Applications()
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            List<CNV> nOrg = new List<CNV>();
            try
            {
                var iUserID = SessionManager.UserID;
                oTrace.oParams.Add(new CNV { sName = "iUserID", sValue = iUserID.ToString() });
                if (iUserID > 0)//check mandatory params are passed or not
                {
                    XIIXI oXI = new XIIXI();
                    XID1Click o1Click = new XID1Click();
                    o1Click.BOID = 681;
                    o1Click.BOIDXIGUID = new Guid("D87C2402-8FC6-474B-B76E-AA7051B8FA17");
                    o1Click.Query = "Select * from XIApplication_T where XIDeleted=0 and StatusTypeID=10";
                    var Data = o1Click.OneClick_Run();
                    if (Data != null && Data.Count() > 0)
                    {
                        foreach (var BOI in Data.Values)
                        {
                            var ID = BOI.AttributeI("ID").sValue;
                            var Name = BOI.AttributeI("sApplicationName").sValue;
                            nOrg.Add(new CNV { sName = Name, sValue = ID });
                        }
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: iUserID is missing";
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
            return Json(nOrg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AppLanding(int ID)
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
                SessionManager.ApplicationID = ID;
                var AppD = (XIDApplication)oCache.GetObjectFromCache(XIConstant.CacheApplication, null, ID.ToString());
                SessionManager.AppName = AppD.sApplicationName;
                var sDatabase = SessionManager.CoreDatabase;
                var RoleID = SessionManager.iRoleID;
                XIInfraRoles oRoleD = new XIInfraRoles();
                oRoleD.RoleID = Convert.ToInt32(RoleID);
                oCR = oRoleD.Get_RoleDefinition(sDatabase);
                oRoleD = (XIInfraRoles)oCR.oResult;
                var oLayDef = oCache.GetObjectFromCache(XIConstant.CacheLayout, null, oRoleD.iLayoutID.ToString()); //oDXI.Get_LayoutDefinition(null, oUser.Role.iLayoutID.ToString());
                XIDLayout oLayout = new XIDLayout();
                if (oLayDef != null)
                {
                    oLayout = (XIDLayout)oLayDef;
                    if (oLayout != null && oLayout.ID > 0)
                    {
                        return View("LandingPage", oLayout);
                    }
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
            return null;
        }

        #endregion MultiApp

        #region Campaign

        public ActionResult Get_Campaigns()
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            List<CNV> nOrg = new List<CNV>();
            try
            {
                var iUserID = SessionManager.UserID;
                oTrace.oParams.Add(new CNV { sName = "iUserID", sValue = iUserID.ToString() });
                if (iUserID > 0)//check mandatory params are passed or not
                {
                    XIInfraCache oCache = new XIInfraCache();
                    var o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "All Campaign");
                    var Data = o1ClickD.OneClick_Run();
                    if (Data != null && Data.Count() > 0)
                    {
                        int i = 0;
                        foreach (var BOI in Data.Values)
                        {
                            var ID = BOI.AttributeI("ID").sValue;
                            var Name = BOI.AttributeI("sName").sValue;
                            nOrg.Add(new CNV { sName = Name, sValue = ID });
                            if (i == 0)
                            {
                                SessionManager.iCampaignID = Convert.ToInt32(ID);
                            }
                            i++;
                        }
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: iUserID is missing";
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
            return Json(nOrg, JsonRequestBehavior.AllowGet);
        }

        #endregion Campaign
        #region Dynami Css

        public ContentResult GetTheme()
        {
            var builder = new StringBuilder();
            try
            {
                IDictionary<string, IDictionary<string, string>> css = new Dictionary<string, IDictionary<string, string>>();
                var Prop = new Dictionary<string, string>();
                Prop.Add("Color", "Red");
                //css.Add("h3", Prop);
                /* Populate css object from the database */

                foreach (var selector in css)
                {
                    builder.Append(selector.Key);
                    builder.Append(" { ");
                    foreach (var entry in selector.Value)
                    {
                        builder.Append(string.Format("{0}: {1}; ", entry.Key, entry.Value));
                    }
                    builder.AppendLine("}");
                }
                var CSS = @" html,body,.ui-widget{font-family: 'Trebuchet MS',Arial, sans-serif;color: #1f1f1f;background-color: #eaebeb;} h1, h2, h3, h4, h5, h6, .h1, .h2, .h3, .h4, .h5, .h6{font-family: 'Trebuchet MS',Arial, sans-serif;color: #1f1f1f;} .loader, .loader-center{     border-top-color: #f28d48 !important; } /* Works on Firefox */ * {     scrollbar-width: thin;     scrollbar-color: #ddd #ffffff;   }      /* Works on Chrome, Edge, and Safari */   *::-webkit-scrollbar {     width: 12px;   }      *::-webkit-scrollbar-track {     background: #ffffff;   }      *::-webkit-scrollbar-thumb {     background-color: #ddd;     border-radius: 20px;     border: 3px solid #ffffff;   } /********************************/ .sidebar-collapse .content-wrapper, .sidebar-collapse .right-side, .sidebar-collapse .main-footer{     /* margin-right: 190px; */ } div#ZeeadminMenu{     position: absolute;     display: flex;     margin-left: auto;     flex: 1;     right: -230px;     -moz-transition: all 0.2s;     -webkit-transition: all 0.2s;     transition: all 0.2s; } div#ZeeadminMenu.openZeeadminMenu{     right: 0px;     -moz-transition: all 0.2s;     -webkit-transition: all 0.2s;     transition: all 0.2s; } div#ZeeadminMenu .menulist>li>a{     padding: 6px 8px;     font-size: 14px; } .inline-layout div#ZeeadminMenu  .control-sidebar{     height: calc(100vh - 70px) !important; } div#ZeeadminMenu #MenuContent{     min-width: 220px; } /********************************/ .content-wrapper, .right-side{background-color: #eaebeb;} .main-header li.user-header{background-color: #ddd;} .dropdown-menu{border-color: #eaebeb;} .navbar-nav > .user-menu > .dropdown-menu > li.user-header{background-color: #464f54;} .navbar-nav > .user-menu > .dropdown-menu > li.user-header > p{color: #fff;} .navbar-nav > .user-menu > .dropdown-menu > .user-footer{background-color: #323c41;} .navbar-custom-menu>.nav>li>a>i{font-size: 20px;} .btn-theme{background-color: #ff6600 !important;color: #fff !important;}  .ui-dialog .body-container .wrap-width .HelpIcon{top: 0px;} .body-container .helptext--desktop{background-color: #ffffff;color: #000;padding: 8px;} .body-container .wrap-width .HelpIcon{font-size: 14px;left: 0px;} .body-container .wrap-width .HelpIcon:before{color: #5488bd;} .form-group.foot-btns{     text-align: right; } .body-container .helptext--desktop{     border: 1px solid #e4e7eb;     box-shadow: 0 0 10px rgb(0 0 0 / 20%); } .body-container .form-btn.move-right{     display: block;     margin: 20px 15px;     float: none;     text-align: right; } .body-container .form-btn.move-right>.btn{     font-size: 16px;     padding: 15px 25px; } .body-container .form-btn.move-right>.btn.btn-success:after {     position: relative;     font-family: 'Font Awesome 5 Free';     content: ""\f30b"";     margin: 0 0 0 6px; } .wrap-12.wrap-fullwidth .wrap-width{width:100%;} .QSStepForm .form-group{margin-bottom: 10px !important;}  /********************************/ .main-header{     display: flex;     align-items: center;     height: 70px;     border-bottom: none;     background: #323c41;  } .main-header > .navbar{     min-height: 70px;     width: 100%;     margin: 0;     display: flex;     justify-content: space-between;     align-items: center; } .main-header .logo {     align-items: center;     font-size: 24px;     display: flex;     background-color: #323c41;     color: #ff6600;     border-bottom: 0 solid transparent;     width: auto; } .main-header > .navbar .header-search{     position: relative;     display: flex;     width: 60%;     top: 0 !important; } .main-header > .navbar .header-search>div{     flex: 2;     margin-right: 15px; } .main-header > .navbar .header-search>div+div{     flex: 1; } .header-search input.form-control, .header-search select.form-control{     border-color: #999999;     height: 44px;     -webkit-box-sizing: border-box;     box-sizing: border-box;     border: 1px solid transparent;     padding: 10px 40px;     line-height: normal;     font-size: 16px;     width: 100%;     /* -webkit-appearance: none; */     -webkit-transition: padding-right 0.177s ease-out;     transition: padding-right 0.177s ease-out;     padding-top: 12px;     padding-bottom: 12.5px;     padding-left: 16px; } .header-search .input-icon.right>.form-control{padding-right: 46px;min-width: 300px;} .header-search .input-icon.right > i{     right: 15px;     font-size: 20px;     margin-top: 13px; } .main-header .navbar .nav > li > a{color: #ffffff;} .nav > li > a:hover, .nav > li > a:active, .nav > li > a:focus{     background: #323c41; } .nav .open>a, .nav .open>a:focus, .nav .open>a:hover{background-color: #464f54;} .main-header .navbar-custom-menu, .main-header .navbar-right{     margin-left: auto; } /****************************************/ .control-sidebar-dark, .control-sidebar-dark + .control-sidebar-bg{background: #ffffff;} .sidebar-menu li > a{color: #1f1f1f;} /***************************************/ div#Zeeinbox{     width: 100%;     min-width: 100%;     background: #464f54; } .inline-layout #Zeeinbox .control-sidebar{height: auto !important;border: none;padding-bottom: 1px;} .inline-layout #Zeeinbox .control-sidebar .menulist{     display: flex;     flex-wrap: nowrap;     justify-content: space-between;     box-shadow: 0px 1px 0px rgba(0, 0, 0, 0.15); } .inline-layout #Zeeinbox .control-sidebar .menulist > li{     display: flex;     align-content: center;     justify-content: center;     flex: 1; } .inline-layout #Zeeinbox .control-sidebar .menulist > li > a > span.ProBar{     display: table;     float: none;     margin: 0 auto;     margin-bottom: 10px;     text-align: center;     color: #ffffff;     background: #323c41;     font-size: 14px;     border: 2px solid #999;     padding: 1px;     min-width: 44px; } .inline-layout #Zeeinbox .sidebar-menu > li > a, .inline-layout #Zeeinbox .sidebar-menu .treeview-menu > li > a{     font-size: 13px;     white-space: normal;     text-align: center;     padding: 5px;     cursor: pointer;     font-weight: 600;     color: #fff;      display: -webkit-box;     -webkit-line-clamp: 2;     -webkit-box-orient: vertical;     overflow: hidden;     text-overflow: ellipsis;     max-height: 75px; } .inline-layout #Zeeinbox .sidebar-menu li.active > .treeview-menu{background: #ddd;} div#Zeeinbox+div.col-md-8{     display: flex;     min-width: calc(100% - 225px);     flex-wrap: wrap; } /*********************************************/ .ui-dialog{padding-left: 40px;} .ui-jqdialog, .ui-dialog{background-color: #ffffff;font-size: 14px;box-shadow:none;} .ui-jqdialog .ui-jqdialog-titlebar, .ui-dialog .ui-dialog-titlebar{background: #eaebeb;border-right: none;color: #1f1f1f;width: 40px;} .dialogIcons{margin: 0 6px;} .ui-widget.ui-widget-content{border: 1px solid #ccc;} .ui-widget-content{border-color: #ff6600!important;} .ui-dialog .ui-dialog-title{-webkit-transform-origin: 7px 2px;} .dialogIcons > i{color: #999;} .ui-dialog .maintitle{     color: #ff6600;     font-weight: 600; } .ui-dialog .sidebar-menu > li > a, .ui-dialog .sidebar-menu .treeview-menu > li > a{font-size: 14px;} div.dataTables_processing, .ui-widget input, .ui-widget select, .ui-widget textarea{     background: #ebebeb;     color: #1f1f1f;     border-color: #ebebeb; } .ui-dialog .left-box{     width: 100%;     display: flex;     overflow: hidden;     overflow-x: auto;     white-space: nowrap; } .ui-dialog .left-nav-btns{     display: flex;     justify-content: center; } .ui-dialog .left-nav-btns::before,.ui-dialog .left-nav-btns::after{     position: absolute;     display: block;     content: "";     right: -7px;     top: 0;     border-top: 25px solid transparent;     border-bottom: 25px solid transparent;     border-left: 16px solid #e4e7eb;     z-index: 1; } .ui-dialog .left-nav-btns::before {     border-left-color: #f2f4f7;     z-index: 2;     right: -4px; } .ui-dialog .left-nav-btns:last-child:before,.ui-dialog .left-nav-btns:last-child:after{display: none;} .ui-dialog .left-nav-btns>li{     display: flex; } .ui-dialog .left-nav-btns > li > a,.ui-dialog .left-nav-btns > li.active > a{     position: relative;     /* display: flex; */     display: inline-block;     padding: 15px 20px;     align-items: center;     border: none;     font-size: 14px;     background: #f2f4f7;     font-weight: 600; } .ui-dialog .left-nav-btns > li.active > a,.ui-dialog .left-nav-btns > li:hover > a, .ui-dialog .left-nav-btns > li:active > a, .ui-dialog .left-nav-btns > li:focus > a{     color: #ff6600;/* font-size: 0px;padding: 14px; */ } .ui-dialog .left-nav-btns > li:hover > a:before,.ui-dialog .left-nav-btns > li.active > a:before{     position: absolute;     display: block;     content: attr(title);     /*height: 0;     overflow: hidden;     visibility: hidden;*/     font-size: 15px;     background: #f2f4f7;     top: 50%;     left: 50%;     -moz-transform: translate(-50%, -50%);     -webkit-transform: translate(-50%, -50%);     transform: translate(-50%, -50%); } .ui-dialog .right-box{width: 100%;} .body-container .questionset-section .form-btn .btn-success, .body-container .questionset-section .form-btn .btn-back, .body-container .questionset-section .form-btn .btn-success:hover, .body-container .questionset-section .form-btn .btn-back:hover{     background: #ff6600 !important;     color: #fff !important;     text-shadow: none !important;     border-color: #ff6600 !important;     background-image:none !important;     text-decoration: none; } .body-container .questionset-section .form-h .form-label{     padding-left: 15px; } .custom-table.table>thead>tr>th{     font-weight: 600;     color: #2b3e56; } .ui-dialog #DynamicForm{margin-top:0;} /*********************************************/ .color_Name {     color: #0090b8;     text-decoration: underline;     font-weight: 600 !important; }    /*********************************************/ .pagination>li>a, .pagination>li>span{color:#ff6600 ;} .pagination>.active>a, .pagination>.active>a:focus, .pagination>.active>a:hover, .pagination>.active>span, .pagination>.active>span:focus, .pagination>.active>span:hover{     background-color: #ff6600;     border-color: #ff6600; } /*********************************************/ table.dataTable thead{background: #eee;} .table-condensed>tbody>tr>td, .table-condensed>tbody>tr>th, .table-condensed>tfoot>tr>td, .table-condensed>tfoot>tr>th, .table-condensed>thead>tr>td, .table-condensed>thead>tr>th{     padding: 2px 5px;     white-space: nowrap; } .table-striped>tbody>tr:nth-of-type(odd){background-color: #f9f9f9;} .table-striped>tbody>tr:nth-of-type(even){background-color: #f2f2f2;} /*********************************************/ .form-control{border-radius: 3px !important;border-color: #999;} .form-label{font-size: 14px;} .form-group.highlight--help:hover{background-color: transparent;box-shadow: none;} .select-wrapper .form-control{background: #ebebeb;border-color: #ebebeb;} .select_wrapper_caret{border-left: none;} /*********************************************/ .LeadContent .btn-theme {     color: #ff6600;     background-color: rgb(144 200 68 / 20%);     border-color: #ff6600;     box-shadow: none !important; } .LeadContent .btn-theme:hover, .btn-theme.active:hover{     background-color: #ff6600;     border-color: #ff6600; } /*********************************************/ .NavbarWrapper.left.showLeftBtn, .NavbarWrapper.Left.showLeftBtn{     height: calc(100vh - 70px); } .NavbarWrapper.left #NavigationBar, .NavbarWrapper.Left #NavigationBar{     height: 100%;     background-color: #dddfdf;     top: 0; } .NavbarWrapper #NavigationBar .btnTabs{border: none;} .NavbarWrapper #NavigationBar .dialogNavBtn a.nav-dlg-btn {     display: inline-block;     min-width: 24px;     margin: 0 1px;     text-align: center; } .btnTabs .dialogNavBtn span.hoverText .closeNavBtn:hover {     color: #000; } .btnTabs .dialogNavBtn span.hoverText{     background-color: #ffffff;     background-image:none;     border: 1px solid #e4e7eb; } .btnTabs .dialogNavBtn:hover span.hoverText{     border-left: none;     box-shadow: none;     color: #000;     text-shadow: none; } .NavbarWrapper #NavigationBar .dialogNavBtn a.nav-dlg-btn{     color: #ff6600;     text-shadow: none; } /*********************************************/ .scroll_tabs_container div.scroll_tab_inner li a{     background: #ffffff !important;     color: #1f1f1f !important;     padding: 4px 10px !important;     font-size: 14px;     border-radius: 3px 3px 3px 3px; } .scroll_tabs_container div.scroll_tab_inner li.tab_selected a, .scroll_tabs_container div.scroll_tab_inner > li > a:hover{     background: #ff6600 !important;     border: 2px solid #999 !important; } .scroll_tabs_container, .scroll_tabs_container div.scroll_tab_inner, .scroll_tabs_container .scroll_tab_left_button, .scroll_tabs_container .scroll_tab_right_button{     border-bottom: none;     height: 40px !important; } .scroll_tabs_container div.scroll_tab_inner span, .scroll_tabs_container div.scroll_tab_inner li{     background-color: #ffffff !important;     padding-left: 4px !important;     padding-right: 4px !important;     line-height: 40px !important; } .scroll_tabs_container .scroll_tab_right_button,.scroll_tabs_container .scroll_tab_left_button, .scroll_tabs_container .scroll_tab_left_button_disabled{background-color: #ffffff !important;} .scroll_tabs_container .scroll_tab_left_button_over{background-color: #f2f4f7 !important;}  .scroll_tabs_container .scroll_tab_left_button::before{     font-family: ""Font Awesome 5 Free"";     content: ""\f053"" !important;     font-weight: 900; } .scroll_tabs_container .scroll_tab_right_button::before{     font-family: ""Font Awesome 5 Free"";     content: ""\f054"" !important;     font-weight: 900; } .nav-tabs-custom .tab-line li a{border-bottom:none !important;border: 2px solid #999 !important;}  /*********************************************/ .ui-datepicker{box-shadow: 0 0 15px rgba(19, 19, 19, 0.20);} .ui-datepicker .ui-datepicker-header{background: #ff6600;} .ui-state-highlight, .ui-widget-content .ui-state-highlight, .ui-widget-header .ui-state-highlight{     border: 1px solid #ff6600;     background: #ff6600; } .ui-datepicker-year,.ui-datepicker-month{color: #1f1f1f;} div.dataTables_processing, .ui-widget input, .ui-widget select, .ui-widget textarea{color: #1f1f1f;} /***********************************************/ .chosen-container{width: 100% !important;} .chosen-container-single .chosen-single{     background: #ebebeb !important;     border-color: #ebebeb !important;     z-index: 1;     width: 100%;     min-height: 28px;     position: relative !important;     padding: 3px 36px 5px 6px !important;     border: none !important;     outline: 0;     background: #fff;     border-radius: 2px !important;     -moz-appearance: none;     -webkit-appearance: none;     cursor: pointer;     box-shadow: none !important; } .chosen-container-single .chosen-single span{margin-right: 0 !important;} .chosen-container-single .chosen-single div{     display: none !important; } /*********************************/ .mainDate{width: auto !important;background: #ebebeb !important;border: none !important;} .myField{width: 62.5px !important;} /**********************************/ .cheapquote{     background-color: #f0c2c2;     color: #717171;     font-size: 16px;     font-weight: 600;     border: none;     box-shadow: none;     display: table;     min-height: 40px; } .cheaptext{     line-height: 1;     display: table-cell;     text-transform: uppercase;     vertical-align: middle; } .Quoteheaderul{     color: #717171;     background-color: #ebebeb; } .Addbtn, .Moredetailsbtn,.Addbtn:hover, .Addbtn:focus, .Moredetailsbtn:hover, .Moredetailsbtn:focus,.Buybtn,.Buybtn:hover, .Buybtn:focus,.Editbtnn,.Editbtnn:hover,.AMprice{     background-color: #ff6600;     background-image: none;     border-color: #ff6600;     color: #fff;     text-shadow: none; } .AMprice{margin-bottom: 5px;} .AMprice:hover, .AMprice.active{     background-color: #fff;     color: #ff6600;     border-color:#ff6600; } .Quotesummary{     background-color: #ebebeb;     border-color: #d9d9d9;     box-shadow:none;     padding: 0px; } .Quotesummary .table tr td{padding: 6px 6px 6px;} .Quotesummary h4, .Quotesummary .table tr td label{     font-size: 15px;     color: #717171;     font-weight: 600 !important; } .Quotesummary .table tr td p{     font-size: 15px;     margin: 0; } .Quotesummary h4{     color: #717171;     padding: 0 6px;     font-weight: 600; } .Quotesummary .panel-default,.panel-default{     border-color: #d9d9d9; } .Quotesummary .panel-default>.panel-heading,.panel-default>.panel-heading{     background-color: #dedede; } .Quotetable [type=""radio""]:checked + label, .Quotetable [type=""radio""]:not(:checked) + label, .pricebtned [type=""radio""]:checked + label, .pricebtned [type=""radio""]:not(:checked) + label{     border-color: #ff6600; } .Quotetable [type=""radio""]:checked + label, .pricebtned [type=""radio""]:checked + label{     background-color: #ff6600 !important; } .Quotetable [type=""radio""]:checked + label:before, .Quotetable [type=""radio""]:not(:checked) + label:before, .pricebtned [type=""radio""]:checked + label:before, .pricebtned [type=""radio""]:not(:checked) + label:before{     border-color: #ff6600; } .Quotetable [type=""radio""]:checked + label:after, .Quotetable [type=""radio""]:not(:checked) + label:after, .pricebtned [type=""radio""]:checked + label:after, .pricebtned [type=""radio""]:not(:checked) + label:after{     background: #ff6600; } .Quotebodyul .downstrip h4,.downstrip h4{     color: #66a7cc;     font-size: 14px;     line-height: 1.4; } .Quotebodyli .AnnualPrice-1 h3{color: #1f1f1f;} .policysummary .panel-body{background-color: #fff;} .ui-dialog .policybox table th, .ui-dialog .policybox table td{color: #717171;} /********************************************/ .addongrid{} .addonli{     color: #1f1f1f;     border: none;     background: #fff;     box-shadow: none;     padding: 0; } .addongrid .panel-title{     color: #717171;     line-height: 1.7;     font-size: 18px;     padding: 0;     margin: 0; } .addongrid .panel-heading{background-color: #fff !important;padding: 0;} .addongrid .firstul{} .addongrid .firstli{     color: #717171;     font-weight: 400;     line-height: 1.4;     margin-bottom: 10px;     font-size: 15px; } .addongrid .panel-body{padding: 0px;} .addonli .panel-body>h4,.contentaddon1{min-height: auto;}  .addon-price,.addon-price span{color: #717171;} .selectswitch{} .selectswitch .switch-label{background: #ff6600;} .selectswitch .switch{width: 90px;} .pdfsection a{color: #717171;} /***********************************************/ svg text{fill: #717171;} /***********************************************/ .bg-red, .callout.callout-danger, .alert-danger, .alert-error, .label-danger, .modal-danger .modal-body{     background-color: #f4c4be !important;     color: #717171;    } .alert-danger{      border: none;     font-size: 16px; } .cell-color{     color:#ff6600; } /*/////////////////////////////*/ .asidetoglbtn > a,.messages-menu > a,.user-menu > a{text-align: center;} .asidetoglbtn > a:after,.messages-menu > a:after,.user-menu > a:after{     position: relative;     display: block;     font-size: 14px; } .asidetoglbtn > a:after{content: 'Menu';} .messages-menu > a:after{content: 'Message';} .user-menu > a:after{content: 'User';} .user-menu > a > .fas.fa-chevron-down{display: none;}";
                builder.AppendLine(CSS);
                return Content(builder.ToString(), "text/css");
            }
            catch (Exception ex)
            {
                return Content(builder.ToString(), "text/css");
            }

        }

        #endregion Dynami Css

    }
}

