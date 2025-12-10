using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using XICore;
using XISystem;
using ZeeInsurance.IDUServiceLive;

namespace ZeeInsurance
{
    public class IDUServicesLive
    {
        #region LexisNexisScore
        public CResult LoadLoginDetails(string sApplicationName, string sApplicationID, string sOrgID)
        {
            CResult oCResult = new CResult(); XIInstanceBase oXIIB = new XIInstanceBase();
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                XIIBO oBOI = new XIIBO(); XIIXI oXI = new XIIXI();
                XIIAttribute oAttr = new XIIAttribute();
                List<CNV> oNVParams = new List<CNV>();
                oNVParams.Add(new CNV { sName = "-iApplicationID", sValue = sApplicationID });
                oNVParams.Add(new CNV { sName = "-iOrgID", sValue = sOrgID });
                //oXIScript.sScript = "xi.s|{xi.secset|'LexNexUN'}";
                var sLNPUN = oXI.XIGetENVSetting("xi.secset|'LexNexUN'", oNVParams);
                var sLNPWD = oXI.XIGetENVSetting("xi.secset|'LexNexPW'", oNVParams);
                oBOI.Attributes = oBOI.SetAttribute("sUserName", sLNPUN);
                oBOI.Attributes = oBOI.SetAttribute("sPassword", sLNPWD);
                oCResult.oResult = oBOI;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = ex.ToString();
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.oResult = "Error";
                oXIIB.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
        public CResult BuildRequestObject(XIBOInstance oStructureInstance)
        {
            CResult oCResult = new CResult(); XIInstanceBase oXIIB = new XIInstanceBase();
            try
            {
                XIIBO oBOI = new XIIBO(); string sGender = string.Empty;
                // Create subject details
                //sGender = oStructureInstance.XIIValue("iGender").sResolvedValue;
                sGender = oStructureInstance.oSubStructureI("Driver_t").Item(0).AttributeI("isex").sResolvedValue;
                if (!string.IsNullOrEmpty(sGender))
                {
                    sGender = sGender[0].ToString();
                }
                string sDOB = oStructureInstance.oSubStructureI("Driver_t").Item(0).AttributeI("dDOB").sValue;
                if (!string.IsNullOrEmpty(sDOB))
                {
                    var dtDate = oBOI.ConvertToDtTime(sDOB);
                    sDOB = dtDate.ToString("yyyy-MM-dd");
                }
                oBOI.SetAttribute("forename", oStructureInstance.oSubStructureI("QS Instance").Item(0).XIIValue("sFirstName").sValue);
                oBOI.SetAttribute("middle", "");
                oBOI.SetAttribute("surname", oStructureInstance.oSubStructureI("QS Instance").Item(0).XIIValue("sLastName").sValue);
                oBOI.SetAttribute("gender", sGender);
                oBOI.SetAttribute("ddob", sDOB);
                oBOI.SetAttribute("address1", oStructureInstance.oSubStructureI("Driver_t").Item(0).AttributeI("sAddressLine1").sValue == ""? oStructureInstance.oSubStructureI("QS Instance").Item(0).AttributeI("sAddressLine1").sValue: oStructureInstance.oSubStructureI("Driver_t").Item(0).AttributeI("sAddressLine1").sValue);
                oBOI.SetAttribute("address2", oStructureInstance.oSubStructureI("Driver_t").Item(0).AttributeI("sAddressLine2").sValue == "" ? oStructureInstance.oSubStructureI("QS Instance").Item(0).AttributeI("sAddressLine2").sValue : oStructureInstance.oSubStructureI("Driver_t").Item(0).AttributeI("sAddressLine2").sValue);
                oBOI.SetAttribute("address3", oStructureInstance.oSubStructureI("Driver_t").Item(0).AttributeI("sAddressLine3").sValue == "" ? oStructureInstance.oSubStructureI("QS Instance").Item(0).AttributeI("sAddressLine3").sValue : oStructureInstance.oSubStructureI("Driver_t").Item(0).AttributeI("sAddressLine3").sValue);
                oBOI.SetAttribute("address4", "");
                oBOI.SetAttribute("address5", "");
                oBOI.SetAttribute("address6", "");
                oBOI.SetAttribute("postcode", oStructureInstance.oSubStructureI("Driver_t").Item(0).AttributeI("sPostcode").sValue == "" ? oStructureInstance.oSubStructureI("QS Instance").Item(0).AttributeI("sPostcode").sValue : oStructureInstance.oSubStructureI("Driver_t").Item(0).AttributeI("sPostcode").sValue);
                // Passport details
                oBOI.SetAttribute("passport1", "");
                oBOI.SetAttribute("passport2", "");
                oBOI.SetAttribute("passport3", "");
                oBOI.SetAttribute("passport4", "");
                oBOI.SetAttribute("passport5", "");
                oBOI.SetAttribute("passport6", "");
                oBOI.SetAttribute("passport7", "");
                oBOI.SetAttribute("passport8", "");
                // Travel Visa details
                oBOI.SetAttribute("travelvisa1", "");
                oBOI.SetAttribute("travelvisa2", "");
                oBOI.SetAttribute("travelvisa3", "");
                oBOI.SetAttribute("travelvisa4", "");
                oBOI.SetAttribute("travelvisa5", "");
                oBOI.SetAttribute("travelvisa6", "");
                oBOI.SetAttribute("travelvisa7", "");
                oBOI.SetAttribute("travelvisa8", "");
                oBOI.SetAttribute("travelvisa9", "");
                // ID Card details
                oBOI.SetAttribute("idcard1", "");
                oBOI.SetAttribute("idcard2", "");
                oBOI.SetAttribute("idcard3", "");
                oBOI.SetAttribute("idcard4", "");
                oBOI.SetAttribute("idcard5", "");
                oBOI.SetAttribute("idcard6", "");
                oBOI.SetAttribute("idcard7", "");
                oBOI.SetAttribute("idcard8", "");
                oBOI.SetAttribute("idcard9", "");
                oBOI.SetAttribute("idcard10", "");
                // Driving Licence details
                oBOI.SetAttribute("drivinglicence1", "");
                oBOI.SetAttribute("drivinglicence2", "");
                oBOI.SetAttribute("drivinglicence3", "");

                oBOI.SetAttribute("cardnumber", "");
                oBOI.SetAttribute("cardtype", "");
                oBOI.SetAttribute("ni", "");
                oBOI.SetAttribute("nhs", "");

                oBOI.SetAttribute("bforename", "");
                oBOI.SetAttribute("bmiddle", "");
                oBOI.SetAttribute("bsurname", "");
                oBOI.SetAttribute("maiden", "");
                oBOI.SetAttribute("bdistrict", "");
                oBOI.SetAttribute("bcertificate", "");

                oBOI.SetAttribute("mpannumber1", "");
                oBOI.SetAttribute("mpannumber2", "");
                oBOI.SetAttribute("mpannumber3", "");
                oBOI.SetAttribute("mpannumber4", "");
                oBOI.SetAttribute("sortcode", "");
                oBOI.SetAttribute("accountnumber", "");

                oBOI.SetAttribute("msubjectforename", "");
                oBOI.SetAttribute("msubjectsurname", "");
                oBOI.SetAttribute("mpartnerforename", "");
                oBOI.SetAttribute("mpartnersurname", "");
                oBOI.SetAttribute("mdate", "");
                oBOI.SetAttribute("mdistrict", "");

                oBOI.SetAttribute("mcertificate", "");
                oBOI.SetAttribute("pollnumber", "");
                oBOI.SetAttribute("email", "");
                oBOI.SetAttribute("email2", "");
                oBOI.SetAttribute("docfront", null);
                oBOI.SetAttribute("docback", null);
                oBOI.SetAttribute("docsize", "");

                oBOI.SetAttribute("address", "True");
                oBOI.SetAttribute("deathscreen", "True");
                oBOI.SetAttribute("dob", "True");
                oBOI.SetAttribute("sanction", "True");
                oBOI.SetAttribute("insolvency", "True");
                oBOI.SetAttribute("ccj", "True");

                oBOI.SetAttribute("docsize", "");
                oBOI.SetAttribute("passport", "False");
                oBOI.SetAttribute("driving", "False");
                oBOI.SetAttribute("birth", "False");
                oBOI.SetAttribute("smartlink", "False");
                oBOI.SetAttribute("ni", "False");
                oBOI.SetAttribute("nhs", "False");
                oBOI.SetAttribute("cardnumber", "False");
                oBOI.SetAttribute("mpan", "False");
                oBOI.SetAttribute("creditactive", "False");
                oBOI.SetAttribute("travelvisa", "False");
                oBOI.SetAttribute("idcard", "False");
                oBOI.SetAttribute("companydirector", "False");
                oBOI.SetAttribute("searchactivity", "False");
                oBOI.SetAttribute("prs", "False");
                oBOI.SetAttribute("marriage", "False");
                oBOI.SetAttribute("pollnumber", "False");
                oBOI.SetAttribute("onlineprofile", "False");
                oBOI.SetAttribute("age", "False");
                oBOI.SetAttribute("docauth", "False");
                oBOI.SetAttribute("onetimepassword", "False");

                oBOI.SetAttribute("ID", "");
                oBOI.SetAttribute("IKey", "");
                oBOI.SetAttribute("Scorecard", "IDU Default");
                oBOI.SetAttribute("equifaxUsername", "");
                oBOI.SetAttribute("Reference", "your-reference");
                oCResult.oResult = oBOI;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = ex.ToString();
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.oResult = "Error";
                oXIIB.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
        public CResult GetLexisNexisScoreResult(List<CNV> oParams)
        {
            CResult oResult = new CResult();
            XIInstanceBase oXIIB = new XIInstanceBase();
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                var sSessionID = HttpContext.Current.Session.SessionID;
                string sGUID = oParams.Where(m => m.sName.ToLower() == "sGUID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sQSInstanceID = oParams.Where(m => m.sName.ToLower() == "iQSInstanceID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sAdminSuccessStep = oParams.Where(m => m.sName.ToLower() == "AdminSuccessStep".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sAdminFailureStep = oParams.Where(m => m.sName.ToLower() == "FailureStep".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sUserSuccessStep = oParams.Where(m => m.sName.ToLower() == "UserSuccessStep".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sUserFailureStep = oParams.Where(m => m.sName.ToLower() == "UserFailureStep".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string srolename = oParams.Where(m => m.sName.ToLower() == "srolename".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sOrgID = oParams.Where(m => m.sName.ToLower() == "iOrganizationID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sApplicationID = oParams.Where(m => m.sName.ToLower() == "iApplicationID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                XIIXI oIXI = new XIIXI(); XIIBO oBo = new XIIBO(); XIIBO oBOI = new XIIBO();
                string sResponse = string.Empty;
                //string sApplicationName = SessionManager.AppName;
                string sApplicationName = "QMT";
                List<CNV> oWhrParams = new List<CNV>();
                CNV oCNV = new CNV();
                oCNV.sName = "sApplicationName";
                oCNV.sValue = sApplicationName;
                oWhrParams.Add(oCNV);
                //oBOI = oIXI.BOI("LexisNexisDetails_T", "", "", oWhrParams);
                XIDBO oBOD= (XIDBO)oCache.GetObjectFromCache(XIConstant.Param_BO, "LexisNexisDetails_T");
                var oBOIDetails = LoadLoginDetails(sApplicationName,sApplicationID,sOrgID);
                if (oBOIDetails.bOK && oBOIDetails.oResult != null)
                {
                    oBOI = (XIIBO)oBOIDetails.oResult;
                }
                var oQSI = oIXI.BOI("QS Instance", sQSInstanceID).Structure("NotationStructure").XILoad();
                var ostructureInstance = oQSI.oStructureInstance.FirstOrDefault().Value.FirstOrDefault();
                var oBOInstance = BuildRequestObject(oQSI);
                if (oBOInstance.bOK && oBOInstance.oResult != null)
                {
                    oBo = (XIIBO)oBOInstance.oResult;
                    if (oBOI.Attributes.ContainsKey("sUserName"))
                    {
                        oBo.Attributes["sUserName"] = new XIIAttribute { sName = "sUserName", sValue = oBOI.Attributes["sUserName"].sValue, bDirty = true };
                    }
                    if (oBOI.Attributes.ContainsKey("sPassword"))
                    {
                        oBo.Attributes["sPassword"] = new XIIAttribute { sName = "sPassword", sValue = oBOI.Attributes["sPassword"].sValue, bDirty = true };
                    }
                    oBo.Attributes["iQSInstanceID"] = new XIIAttribute { sName = "iQSInstanceID", sValue = sQSInstanceID, bDirty = true };
                }
                oResult.sMessage = oResult.sMessage + " Datasource: " + oBOD.iDataSource + " UserName : " + oBo.Attributes["sUserName"].sValue + " Pwd : " + oBo.Attributes["sPassword"].sValue;
                oXIIB.SaveErrortoDB(oResult);
                var oResponse = LoadLexisNexisScoreResult(oBo.Attributes);
                if (oResponse.bOK && oResponse.oResult != null)
                {
                    Result result = (Result)oResponse.oResult;
                    var oLexisNexisResult = SaveLexisNexisResponse(result, oBo.Attributes);
                    sResponse = result.Summary.ResultText;
                    //sResponse = (string)oResponse.oResult;
                    if (!string.IsNullOrEmpty(sResponse))
                    {
                        if (sResponse.ToLower() == "pass" && (srolename.ToLower() != xiEnumSystem.EnumRoles.WebUsers.ToString().ToLower()))
                        {
                            oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sAdminSuccessStep, null, null);
                        }
                        else if ((sResponse.ToLower() == "fail" || sResponse.ToLower() == "refer") && (srolename.ToLower() != xiEnumSystem.EnumRoles.WebUsers.ToString().ToLower()))
                        {
                            oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sAdminFailureStep, null, null);
                        }
                        //else if ((sResponse.ToLower() == "fail" || sResponse.ToLower() == "refer") && (srolename.ToLower() != xiEnumSystem.EnumRoles.WebUsers.ToString().ToLower() || !string.IsNullOrEmpty(srolename)))
                        //{
                        //    oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sAdminFailureStep, null, null);
                        //}
                        else if ((sResponse.ToLower() == "fail" || sResponse.ToLower() == "refer") && (srolename.ToLower() == xiEnumSystem.EnumRoles.WebUsers.ToString().ToLower()))
                        {
                            oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sUserFailureStep, null, null);
                        }
                        else if (sResponse.ToLower() == "pass" && (srolename.ToLower() == xiEnumSystem.EnumRoles.WebUsers.ToString().ToLower()))
                        {
                            oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", sUserSuccessStep, null, null);
                        }
                    }
                }
                oResult.oResult = sResponse;
            }
            catch (Exception ex)
            {
                oResult.sMessage = ex.ToString();
                oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oResult.oResult = "Error";
                oXIIB.SaveErrortoDB(oResult);
            }
            return oResult;
        }
        public CResult LoadLexisNexisScoreResult(Dictionary<string, XIIAttribute> nAttributes)
        {
            CResult oResult = new CResult();
            XIInstanceBase oXIIB = new XIInstanceBase();
            try
            {
                var jsonSerialiser = new JavaScriptSerializer();
                XIInfraCache oCache = new XIInfraCache();
                LoginDetails login = new LoginDetails();
                PersonDetails personDetails = new PersonDetails();
                ServiceDetails serviceDetails = new ServiceDetails();
                // string sDOB = "1962-01-01";
                // Create login details
                //login.username = "20015477";
                //login.password = "522TwX6Jehde";
                if (nAttributes.ContainsKey("sUserName"))
                {
                    login.username = nAttributes["sUserName"].sValue;
                }
                if (nAttributes.ContainsKey("sPassword"))
                {
                    login.password = nAttributes["sPassword"].sValue;
                }
                // Create subject details
                personDetails.forename = nAttributes["forename"].sValue;
                personDetails.middle = nAttributes["middle"].sValue;
                personDetails.surname = nAttributes["surname"].sValue;
                personDetails.gender = nAttributes["gender"].sValue;
                //if (!string.IsNullOrEmpty(nAttributes["ddob"].sValue))
                //{
                //   sDOB= nAttributes["ddob"].sValue;
                //}
                personDetails.dob = nAttributes["ddob"].sValue;
                personDetails.address1 = nAttributes["address1"].sValue;
                personDetails.address2 = nAttributes["address2"].sValue;
                personDetails.address3 = nAttributes["address3"].sValue;
                personDetails.address4 = nAttributes["address4"].sValue;
                personDetails.address5 = nAttributes["address5"].sValue;
                personDetails.address6 = nAttributes["address6"].sValue;
                personDetails.postcode = nAttributes["postcode"].sValue;

                // Passport details
                personDetails.passport1 = nAttributes["passport1"].sValue;
                personDetails.passport2 = nAttributes["passport2"].sValue;
                personDetails.passport3 = nAttributes["passport3"].sValue;
                personDetails.passport4 = nAttributes["passport4"].sValue;
                personDetails.passport5 = nAttributes["passport5"].sValue;
                personDetails.passport6 = nAttributes["passport6"].sValue;
                personDetails.passport7 = nAttributes["passport7"].sValue;
                personDetails.passport8 = nAttributes["passport8"].sValue;

                // Travel Visa details
                personDetails.travelvisa1 = nAttributes["travelvisa1"].sValue;
                personDetails.travelvisa2 = nAttributes["travelvisa2"].sValue;
                personDetails.travelvisa3 = nAttributes["travelvisa3"].sValue;
                personDetails.travelvisa4 = nAttributes["travelvisa4"].sValue;
                personDetails.travelvisa5 = nAttributes["travelvisa5"].sValue;
                personDetails.travelvisa6 = nAttributes["travelvisa6"].sValue;
                personDetails.travelvisa7 = nAttributes["travelvisa7"].sValue;
                personDetails.travelvisa8 = nAttributes["travelvisa8"].sValue;
                personDetails.travelvisa9 = nAttributes["travelvisa9"].sValue;

                // ID Card details
                personDetails.idcard1 = nAttributes["idcard1"].sValue;
                personDetails.idcard2 = nAttributes["idcard2"].sValue;
                personDetails.idcard3 = nAttributes["idcard3"].sValue;
                personDetails.idcard4 = nAttributes["idcard4"].sValue;
                personDetails.idcard5 = nAttributes["idcard5"].sValue;
                personDetails.idcard6 = nAttributes["idcard6"].sValue;
                personDetails.idcard7 = nAttributes["idcard7"].sValue;
                personDetails.idcard8 = nAttributes["idcard8"].sValue;
                personDetails.idcard9 = nAttributes["idcard9"].sValue;
                personDetails.idcard10 = nAttributes["idcard10"].sValue;

                // Driving Licence details
                personDetails.drivinglicence1 = nAttributes["drivinglicence1"].sValue;
                personDetails.drivinglicence2 = nAttributes["drivinglicence2"].sValue;
                personDetails.drivinglicence3 = nAttributes["drivinglicence3"].sValue;

                // Card Number
                personDetails.cardnumber = nAttributes["drivinglicence1"].sValue;
                personDetails.cardtype = nAttributes["drivinglicence1"].sValue;

                // NI
                personDetails.ni = nAttributes["drivinglicence1"].sValue;

                // NHS
                personDetails.nhs = nAttributes["drivinglicence1"].sValue;

                // Birth Details
                personDetails.bforename = nAttributes["bforename"].sValue;
                personDetails.bmiddle = nAttributes["bmiddle"].sValue;
                personDetails.bsurname = nAttributes["bsurname"].sValue;
                personDetails.maiden = nAttributes["maiden"].sValue;
                personDetails.bdistrict = nAttributes["bdistrict"].sValue;
                personDetails.bcertificate = nAttributes["bcertificate"].sValue;

                // Electricity Bill
                personDetails.mpannumber1 = nAttributes["mpannumber1"].sValue;
                personDetails.mpannumber2 = nAttributes["mpannumber2"].sValue;
                personDetails.mpannumber3 = nAttributes["mpannumber3"].sValue;
                personDetails.mpannumber4 = nAttributes["mpannumber4"].sValue;

                // Bank Account
                personDetails.sortcode = nAttributes["sortcode"].sValue;
                personDetails.accountnumber = nAttributes["accountnumber"].sValue;

                // Marriage Details
                personDetails.msubjectforename = nAttributes["msubjectforename"].sValue;
                personDetails.msubjectsurname = nAttributes["msubjectsurname"].sValue;
                personDetails.mpartnerforename = nAttributes["mpartnerforename"].sValue;
                personDetails.mpartnersurname = nAttributes["mpartnersurname"].sValue;
                personDetails.mdate = nAttributes["mdate"].sValue;
                personDetails.mdistrict = nAttributes["mdistrict"].sValue;
                personDetails.mcertificate = nAttributes["mcertificate"].sValue;

                // Poll Number Details
                personDetails.pollnumber = nAttributes["pollnumber"].sValue;

                // Email Details
                personDetails.email = nAttributes["email"].sValue;
                personDetails.email2 = nAttributes["email2"].sValue;

                // Document Authentication Details
                personDetails.docfront = nAttributes["docfront"].sValue != null ? Encoding.ASCII.GetBytes(nAttributes["docfront"].sValue) : null;
                personDetails.docback = nAttributes["docback"].sValue != null ? Encoding.ASCII.GetBytes(nAttributes["docback"].sValue) : null;
                personDetails.docsize = nAttributes["docsize"].sValue;

                // One Time Password Details
                //personDetails->otp[0]->input = "";
                //personDetails->otp[0]->otp = "";

                // Enable minimum services for the IDU configuration
                //Boolean.TryParse(nAttributes["mcertificate"].bv,out serviceDetails.address);
                serviceDetails.address = nAttributes["address"].bValue;
                serviceDetails.deathscreen = nAttributes["deathscreen"].bValue;
                serviceDetails.dob = nAttributes["dob"].bValue;
                serviceDetails.sanction = nAttributes["sanction"].bValue;
                serviceDetails.insolvency = nAttributes["insolvency"].bValue;
                serviceDetails.ccj = nAttributes["ccj"].bValue;

                // If your organisation has access to Crediva then this
                // should be added to the code:
                //serviceDetails.crediva = true;

                // Explicitly disable non-required services
                serviceDetails.passport = nAttributes["passport"].bValue;
                serviceDetails.driving = nAttributes["driving"].bValue;
                serviceDetails.birth = nAttributes["birth"].bValue;
                serviceDetails.smartlink = nAttributes["smartlink"].bValue;
                serviceDetails.ni = nAttributes["ni"].bValue;
                serviceDetails.nhs = nAttributes["nhs"].bValue;
                serviceDetails.cardnumber = nAttributes["cardnumber"].bValue;
                serviceDetails.mpan = nAttributes["mpan"].bValue;
                //serviceDetails.bankaccountvalidation = false;
                serviceDetails.creditactive = nAttributes["creditactive"].bValue;
                serviceDetails.travelvisa = nAttributes["travelvisa"].bValue;
                serviceDetails.idcard = nAttributes["idcard"].bValue;
                //serviceDetails.bankaccountverification = false;
                serviceDetails.companydirector = nAttributes["companydirector"].bValue;
                serviceDetails.searchactivity = nAttributes["searchactivity"].bValue;
                serviceDetails.prs = nAttributes["prs"].bValue;
                serviceDetails.marriage = nAttributes["marriage"].bValue;
                serviceDetails.pollnumber = nAttributes["pollnumber"].bValue;
                serviceDetails.onlineprofile = nAttributes["onlineprofile"].bValue;
                serviceDetails.age = nAttributes["age"].bValue;
                serviceDetails.docauth = nAttributes["docauth"].bValue;
                serviceDetails.onetimepassword = nAttributes["onetimepassword"].bValue;
                //serviceDetails.emailaddresses = false;
                //serviceDetails.phonenumbers = false;

                // Optional, to aid request/response tracking
                IDUDetails iduDetails = new IDUDetails();
                iduDetails.ID = nAttributes["ID"].sValue;
                iduDetails.IKey = nAttributes["IKey"].sValue;
                iduDetails.Scorecard = nAttributes["Scorecard"].sValue;
                iduDetails.equifaxUsername = nAttributes["equifaxUsername"].sValue;

                //Reference is optional/mandatory based on user settings
                iduDetails.Reference = nAttributes["Reference"].sValue;

                // Create Request
                Request request = new Request();
                request.Login = login;
                request.Person = personDetails;
                request.Services = serviceDetails;
                request.IDU = iduDetails;

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                // Make synchronous call to IduService
                iduService service = new iduService();
                service.Timeout = 20000;
                Result result = new Result();
                result = service.IDUProcess(request);
                //load xml file result
                //result = (Result)XMLToObject("", result);
                //create xml for response

                oResult.oResult = result;
            }
            catch (Exception ex)
            {
                oResult.sMessage = ex.ToString();
                oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oResult.oResult = "Error";
                oXIIB.SaveErrortoDB(oResult);
            }

            return oResult;
        }
        public CResult SaveLexisNexisResponse(Result result, Dictionary<string, XIIAttribute> nAttributes)
        {
            CResult oCResult = new CResult();
            XIInstanceBase oXIIB = new XIInstanceBase();
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                //result.Summary.ResultText = "PASS";
                string strResView = CreateXML(result);
                //save request and response objects
                XIIBO oBOI = new XIIBO();
                XIDBO oBOD = new XIDBO();
                oBOD = (XIDBO)oCache.GetObjectFromCache("bo", "LexisNexisResponse_T");
                Dictionary<string, XIIAttribute> oAttrs = new Dictionary<string, XIIAttribute>();
                oBOI.BOD = oBOD;
                oCResult.sMessage = "SaveLexisNexisResponse, UserName : " + nAttributes.Where(x => x.Key == "sUserName").Select(t => t.Value).ToString() + ", Pwd : " + nAttributes.Where(x => x.Key == "sPassword").Select(t => t.Value).ToString();
                oXIIB.SaveErrortoDB(oCResult);
                //create xml for request object
                var xEle = new XElement("Attributes",
                    from Attr in nAttributes
                    select new XElement(Attr.Key,
                                 new XAttribute("sName", Attr.Value.sName),
                                   new XElement("sValue", Attr.Value.sValue),
                                   new XElement("dValue", Attr.Value.dValue),
                                   new XElement("iValue", Attr.Value.iValue),
                                   new XElement("bValue", Attr.Value.bValue),
                                     //new XElement("ImagePathDetails", Attr.Value.ImagePathDetails),
                                     new XElement("bDirty", Attr.Value.bDirty),
                                     new XElement("bLock", Attr.Value.bLock)
                               //new XElement("oParent", Attr.Value.oParent),
                               //new XElement("oDefintion", Attr.Value.oDefintion),
                               //new XElement("BOD", Attr.Value.BOD),
                               //new XElement("BOI", Attr.Value.BOI)
                               ));
                //xEle.Save("E:/LexisNexisRequest.xml");
                //XmlDocument doc = new XmlDocument();
                //doc.Load("E:/LexisNexisRequest.xml");
                //xml request object
                //string sRequestXML = doc.InnerXml;

                //Req XML
                string sRequestXML = xEle.ToString();
                XmlDocument xmltest = new XmlDocument();
                xmltest.LoadXml(sRequestXML);
                sRequestXML = xmltest.InnerXml;
                //Req Json
                //var jsonSerialiser = new JavaScriptSerializer();
                //var sRequestObject = jsonSerialiser.Serialize(nAttributes);
                List<CNV> oWParams = new List<CNV>();
                CNV oWP = new CNV();
                oWP.sName = "FKiQsInstanceID";
                oWP.sValue = nAttributes["iQSInstanceID"].sValue;
                oWParams.Add(oWP);
                XIIXI oXIIXI = new XIIXI();
                oBOI = oXIIXI.BOI("LexisNexisResponse_T", "", null, oWParams);

                if (oBOI != null && oBOI.Attributes != null)
                {
                    if (oBOI.Attributes.ContainsKey("sRequest"))
                    {
                        oBOI.Attributes["sRequest"].sValue = sRequestXML;
                    }
                    if (oBOI.Attributes.ContainsKey("sResponse"))
                    {
                        oBOI.Attributes["sResponse"].sValue = strResView;
                    }
                    if (oBOI.Attributes.ContainsKey("sResultText"))
                    {
                        oBOI.Attributes["sResultText"].sValue = result.Summary.ResultText;
                    }
                    if (oBOI.Attributes.ContainsKey("iSmartscore"))
                    {
                        oBOI.Attributes["iSmartscore"].sValue = result.Summary.Smartscore.ToString();
                    }
                    if (oBOI.Attributes.ContainsKey("sProfileUrl"))
                    {
                        oBOI.Attributes["sProfileUrl"].sValue = result.Summary.ProfileURL;
                    }
                    oBOI.Attributes.ToList().ForEach(m => m.Value.bDirty = true);
                    var Response = oBOI.Save(oBOI);
                    if (Response.bOK && Response.oResult != null)
                    {
                        oBOI = (XIIBO)Response.oResult;
                    }
                }
                else
                {
                    oBOI = new XIIBO();
                    oBOI.BOD = oBOD;
                    oBOI.SetAttribute("sRequest", sRequestXML);
                    oBOI.SetAttribute("sResponse", strResView);
                    oBOI.SetAttribute("sResultText", result.Summary.ResultText);
                    oBOI.SetAttribute("iSmartscore", result.Summary.Smartscore.ToString());
                    oBOI.SetAttribute("FKiQSInstanceID", nAttributes["iQSInstanceID"].sValue);
                    oBOI.SetAttribute("sProfileUrl", result.Summary.ProfileURL);
                    var Response = oBOI.Save(oBOI);
                    if (Response.bOK && Response.oResult != null)
                    {
                        oBOI = (XIIBO)Response.oResult;
                    }
                }
                //string strReqView = CreateXML(nAttributes);
                //json request object
                //var oReqjson = jsonSerialiser.Serialize(nAttributes);
                //oCResult.oResult = result.Summary.ResultText;
                oCResult.oResult = oBOI.Attributes.Values.Where(m => m.sName.ToLower() == oBOI.BOD.sPrimaryKey.ToLower()).FirstOrDefault().sValue;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = ex.ToString();
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.oResult = "Error";
                oXIIB.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        #region MocLexisNexis
        public CResult GetMocLexisNexisScoreResult(List<CNV> oParams)
        {
            CResult oResult = new CResult();
            XIInstanceBase oXIIB = new XIInstanceBase();
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                var sSessionID = HttpContext.Current.Session.SessionID;
                string sGUID = oParams.Where(m => m.sName.ToLower() == "sGUID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                string sQSInstanceID = oParams.Where(m => m.sName.ToLower() == "iQSInstanceID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                XIIXI oIXI = new XIIXI(); XIIBO oBo = new XIIBO(); XIIBO oBOI = new XIIBO();
                string sResponse = string.Empty;
                //string sApplicationName = SessionManager.AppName;
                string sApplicationName = "QMT";
                List<CNV> oWhrParams = new List<CNV>();
                CNV oCNV = new CNV();
                oCNV.sName = "sApplicationName";
                oCNV.sValue = sApplicationName;
                oWhrParams.Add(oCNV);
                oBOI = oIXI.BOI("LexisNexisDetails_T", "", "", oWhrParams);
                //var oBOIDetails = LoadLoginDetails(sApplicationName);
                //if (oBOIDetails.bOK && oBOIDetails.oResult != null)
                //{
                //    oBOI = (XIIBO)oBOIDetails.oResult;
                //}
                var oQSI = oIXI.BOI("QS Instance", sQSInstanceID).Structure("NotationStructure").XILoad();
                var ostructureInstance = oQSI.oStructureInstance.FirstOrDefault().Value.FirstOrDefault();
                var oBOInstance = BuildRequestObject(oQSI);
                if (oBOInstance.bOK && oBOInstance.oResult != null)
                {
                    oBo = (XIIBO)oBOInstance.oResult;
                    if (oBOI.Attributes.ContainsKey("sUserName"))
                    {
                        oBo.Attributes["sUserName"] = new XIIAttribute { sName = "sUserName", sValue = oBOI.Attributes["sUserName"].sValue, bDirty = true };
                    }
                    if (oBOI.Attributes.ContainsKey("sPassword"))
                    {
                        oBo.Attributes["sPassword"] = new XIIAttribute { sName = "sPassword", sValue = oBOI.Attributes["sPassword"].sValue, bDirty = true };
                    }
                    oBo.Attributes["iQSInstanceID"] = new XIIAttribute { sName = "iQSInstanceID", sValue = sQSInstanceID, bDirty = true };
                }
                var oResponse = LoadLexisNexisScoreResult(oBo.Attributes);

                if (oResponse.bOK && oResponse.oResult != null)
                {
                    Result result = (Result)oResponse.oResult;
                    result.Summary.ResultText = "PASS";
                    sResponse = result.Summary.ResultText;
                    var oLexisNexisResult = SaveLexisNexisResponse(result, oBo.Attributes);
                    //sResponse = (string)oResponse.oResult;
                    if (!string.IsNullOrEmpty(result.Summary.ResultText))
                    {
                        if (result.Summary.ResultText.ToLower() == "pass")
                        {
                            oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", "Your Quotes", null, null);
                        }
                        else if (result.Summary.ResultText.ToLower() == "fail" || result.Summary.ResultText.ToLower() == "refer")
                        {
                            oCache.Set_ParamVal(sSessionID, sGUID, null, "NextStep", "Decline", null, null);
                        }
                    }
                }
                oResult.oResult = sResponse;
            }
            catch (Exception ex)
            {
                oResult.sMessage = ex.ToString();
                oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oResult.oResult = "Error";
                oXIIB.SaveErrortoDB(oResult);
            }

            return oResult;
        }
        #endregion

        public string CreateXML(object oResponse)
        {
            XmlDocument xmlDoc = new XmlDocument();   //Represents an XML document, 
                                                      // Initializes a new instance of the XmlDocument class.          
            XmlSerializer xmlSerializer = new XmlSerializer(oResponse.GetType());
            // Creates a stream whose backing store is memory. 
            using (MemoryStream xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, oResponse);
                xmlStream.Position = 0;
                //Loads the XML document from the specified string.
                xmlDoc.Load(xmlStream);
                //xmlDoc.Save("E:/text4.xml");
                return xmlDoc.InnerXml;
            }
        }

        public Object XMLToObject(string XMLString, Object oObject)
        {
            //HttpContext.Current.ApplicationInstance.Server.MapPath("~/App_Data/") + "cars.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load("E:/text.xml");
            string xmlcontents = doc.InnerXml;
            //XMLString = System.Web.Hosting.HostingEnvironment.MapPath("~")+ "text.xml";
            XmlSerializer oXmlSerializer = new XmlSerializer(oObject.GetType());
            oObject = oXmlSerializer.Deserialize(new StringReader(xmlcontents));
            return oObject;
        }
        #endregion
    }
}