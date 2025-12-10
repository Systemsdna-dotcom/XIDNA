using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XICore;
using XIDatabase;
using XIDNA.Models;
using XIDNA.Repository;
using XISystem;

namespace XIDNA.Controllers
{
    public class XIInterpreterController : Controller
    {
        // GET: XIInterpreter
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult XIInterpreter(string XICode)
        {
            string XITask = string.Empty;
            SignalR oSignalR = new SignalR();
            XIConfigs oConfig = new XIConfigs(oSignalR);
            var ApplicationID = SessionManager.ApplicationID;
            oConfig.iAppID = ApplicationID;
            if (!string.IsNullOrEmpty(XICode))
            {
                var CodeLines = XICode.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (CodeLines != null && CodeLines.Count() > 0)
                {
                    var XIKeyWord = CodeLines.FirstOrDefault();
                    if (XIKeyWord.StartsWith("XI."))
                    {
                        var sKey = XIKeyWord.Split('.')[1];
                        XITask = sKey.Split('-')[0];
                    }
                    switch (XITask)
                    {
                        case "QuestionSet":
                            oConfig.Interpret_QuestionSet(XICode);
                            break;
                        case "BO":
                            oConfig.Interpret_BO(XICode);
                            break;
                        case "XILink":
                            oConfig.Interpret_XILink(XICode);
                            break;
                        case "Menu":
                            oConfig.Interpret_Menu(XICode);
                            break;
                        default:
                            break;
                    }
                }
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult XIScriptEditor(string sValue, int FKiBOID, int FKiBOAttributeID, string sBOName, string sAttrName)
        {
            int iScriptID = 0;
            XIDScript oScriptD = new XIDScript();
            XIInfraCache oCache = new XIInfraCache();
            if (!string.IsNullOrEmpty(sValue))
            {
                if (sValue.StartsWith("xi.sr|"))
                {
                    var sScriptID = string.Empty;
                    var values = sValue.Split(new string[] { "xi.sr|" }, StringSplitOptions.None).ToList();
                    if (values.Count() > 0)
                    {
                        sScriptID = values[1];
                    }
                    int.TryParse(sScriptID, out iScriptID);
                    if (iScriptID > 0)
                    {
                        oScriptD = (XIDScript)oCache.GetObjectFromCache(XIConstant.CacheScript, null, iScriptID.ToString());
                    }
                    else if (!string.IsNullOrEmpty(sScriptID))
                    {
                        oScriptD = (XIDScript)oCache.GetObjectFromCache(XIConstant.CacheScript, sScriptID);
                    }
                }
            }
            CommonRepository Common = new CommonRepository();
            oScriptD.ddlLanguages = Common.GetScriptLanguageDDL("").Select(m => new XIDropDown { Value = m.Value, text = m.text }).ToList();
            oScriptD.ddlScriptTypes = Common.GetScriptTypeDDL("").Select(m => new XIDropDown { Value = m.Value, text = m.text }).ToList();
            oScriptD.ddlStatusTypes = Common.GetStatusTypeDDL("").Select(m => new XIDropDown { Value = m.Value, text = m.text }).ToList();
            oScriptD.ddlScriptLevels = Common.GetScriptLevelDDL("").Select(m => new XIDropDown { Value = m.Value, text = m.text }).ToList();
            oScriptD.ddlScriptCategory = Common.GetScriptCategoryDDL("").Select(m => new XIDropDown { Value = m.Value, text = m.text }).ToList();
            oScriptD.ddlScriptClassification = Common.GetScriptClassificationDDL("").Select(m => new XIDropDown { Value = m.Value, text = m.text }).ToList();
            oScriptD.ScriptResults = new List<XIDScriptResult>();
            XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
            List<XIDScript> oScript = null;
            oScript = Connection.Select<XIDScript>("XIBOScript_T", new Dictionary<string, object>()).ToList();
            ViewBag.Scripts = oScript;
            oScriptD.FKiBOID = FKiBOID;
            oScriptD.FKiBOAttributeID = FKiBOAttributeID;
            oScriptD.sBOName = sBOName;
            oScriptD.sAttrName = sAttrName;
            return PartialView("_XIScriptEditor", oScriptD);
        }

        [HttpPost]
        public ActionResult GetXIScriptValidator(string XIScript, string sScriptType)
        {
            XIInfraScript oScript = new XIInfraScript();
            var oCR = oScript.ValidateXIScript(XIScript, sScriptType);
            if (oCR.bOK && oCR.oResult != null)
            {
                var Result = (string)oCR.oResult;
                if (Result == "XIScriptSuccess")
                {
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Failure", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("Failure", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[ValidateInput(true)]       
        public ActionResult SaveBOScript(XIDScript model, string submit)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                if (!string.IsNullOrEmpty(submit) && submit.ToLower() == "validate")
                {
                    XIInfraScript oScript = new XIInfraScript();
                    var oCR = oScript.ValidateXIScript(model.sScript, model.sLanguage);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        var Result = (string)oCR.oResult;
                        if (Result == "XIScriptSuccess")
                        {
                            return Json("Success", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json("Failure", JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else if (submit.ToLower() == "save")
                {
                    SignalR oSignalR = new SignalR();
                    XIConfigs oConfig = new XIConfigs(oSignalR);
                    oConfig.Save_BOScript(model);
                }
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                model.CreatedBy = model.UpdatedBy = iUserID;
                //var Response = BusinessObjectsRepository.SaveBOScript(model, iUserID, sOrgName, sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        [HttpPost]
        public ActionResult LoadScriptToEditor(int iScriptID = 0)
        {
            var oScript = new XIDScript();
            if (iScriptID > 0)
            {
                XIInfraCache oCache = new XIInfraCache();
                oScript = (XIDScript)oCache.GetObjectFromCache(XIConstant.CacheScript, null, iScriptID.ToString());
            }
            return Json(oScript.sScript, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddNewScript(string sScrptId)
        {
            var oScriptD = new XIDScript();
            CommonRepository Common = new CommonRepository();
            oScriptD.ddlLanguages = Common.GetScriptLanguageDDL("").Select(m => new XIDropDown { Value = m.Value, text = m.text }).ToList();
            oScriptD.ddlScriptTypes = Common.GetScriptTypeDDL("").Select(m => new XIDropDown { Value = m.Value, text = m.text }).ToList();
            oScriptD.ddlStatusTypes = Common.GetStatusTypeDDL("").Select(m => new XIDropDown { Value = m.Value, text = m.text }).ToList();
            oScriptD.ddlScriptLevels = Common.GetScriptLevelDDL("").Select(m => new XIDropDown { Value = m.Value, text = m.text }).ToList();
            oScriptD.ddlScriptCategory = Common.GetScriptCategoryDDL("").Select(m => new XIDropDown { Value = m.Value, text = m.text }).ToList();
            oScriptD.ddlScriptClassification = Common.GetScriptClassificationDDL("").Select(m => new XIDropDown { Value = m.Value, text = m.text }).ToList();
            ViewBag.sScrptId = sScrptId;
            return PartialView("_XIEditor", oScriptD);
        }
    }
}