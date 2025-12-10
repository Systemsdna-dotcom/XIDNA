using System.Globalization;
using XIDNA.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;
using System.Text.RegularExpressions;
using XIDNA.Common;
using XIDNA.Repository;
using XICore;
using System.Net.Mail;
using System.Net;
using XIDNA.Mailer;
using XISystem;
using XIDNA.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Web.Routing;
using Microsoft.Ajax.Utilities;
using ZeeInsurance.IDUServiceLive;
using XIDNA.Providers;
using Microsoft.SqlServer.Management.Smo;
using NPOI.SS.Formula.Functions;
using static NPOI.SS.Formula.PTG.ArrayPtg;

namespace XIDNA.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public string sOTP { get; set; }
        ModelDbContext dbContext = new ModelDbContext();
        CommonRepository Common = new CommonRepository();
        CXiAPI oXIAPI = new CXiAPI();
        XIInfraEmail oXIMail = new XIInfraEmail();
        IUserMailer mailer = new UserMailer();
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                var sServerKey = System.Configuration.ConfigurationManager.AppSettings["ServerKey"];
                if (string.IsNullOrEmpty(SessionManager.Theme))
                {
                    SessionManager.Theme = System.Configuration.ConfigurationManager.AppSettings["ThemeFile"];
                }
                if (string.IsNullOrEmpty(SessionManager.AppName) && (sServerKey.ToLower() == "zeeadmin" || sServerKey.ToLower() == "zeedev"))
                {
                    var ApplicationPath = System.Configuration.ConfigurationManager.AppSettings["ApplicationPath"];
                    var ApplicationName = System.Configuration.ConfigurationManager.AppSettings["ApplicationName"];
                    SessionManager.AppName = ApplicationName;
                    return Redirect(ApplicationPath + ApplicationName);
                }
                SessionManager.UserID = 0;
                ViewBag.ReturnUrl = returnUrl;
                //FormsAuthentication.SignOut();
                var AppPath = System.Configuration.ConfigurationManager.AppSettings["ApplicationPath"];
                var Url = Request.Url.AbsoluteUri;
                if (Url == AppPath)
                {
                    SessionManager.CoreDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinDatabase"];
                    ViewBag.Logo = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                    ViewBag.AppName = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinName"];
                    SessionManager.Logo = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                    SessionManager.AppName = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinName"];
                    SessionManager.bNannoApp = false;
                    SessionManager.OrganizationID = 0;
                }
                else if (!string.IsNullOrEmpty(SessionManager.AppName))
                {
                    //SessionManager.CoreDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinDatabase"];
                    if (!string.IsNullOrEmpty(SessionManager.Logo))
                    {
                        ViewBag.Logo = SessionManager.Logo;
                    }
                    else
                    {
                        ViewBag.Logo = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                    }
                    ViewBag.AppName = SessionManager.AppName;
                }
                else
                {
                    SessionManager.CoreDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinDatabase"];
                    ViewBag.Logo = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                    ViewBag.AppName = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinName"];
                }
                return View();
            }
            catch (Exception ex)
            {
                //logger.Error(ex);
                //Common.SaveErrorLog(ex.ToString());
                ModelState.AddModelError("", "Error Occured");
                return View();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                var Database = SessionManager.CoreDatabase;
                int iMainApplicationID = SessionManager.ApplicationID;
                XIInfraUsers oUser = new XIInfraUsers();
                XIInfraCache oCacheO = new XIInfraCache();
                XIInfraEncryption oEncrypt = new XIInfraEncryption();
                //var EncryptedPwd01 = oEncrypt.EncryptData("Data Source=LENOVO_SDNA\\SQLEXPRESS;initial catalog=CSP_XIEnvironment;User Id=XIDNAADMIN; Password=XIDNAADMIN; MultipleActiveResultSets=True", true, "6105A023-04DF-40A8-9669-69E75D4A6B50".ToLower());
                //var EncryptedPwd02 = oEncrypt.EncryptData("Data Source=LENOVO_SDNA\\SQLEXPRESS;initial catalog=CSP_Setting;User Id=XIDNAADMIN; Password=XIDNAADMIN; MultipleActiveResultSets=True", true, "D2EF9017-0D1C-44CA-A2C0-AD0527BAAC5B".ToLower());
                //var EncryptedPwd03 = oEncrypt.EncryptData("Data Source=LENOVO_SDNA\\SQLEXPRESS;initial catalog=CSP_OrgName;User Id=XIDNAADMIN; Password=XIDNAADMIN; MultipleActiveResultSets=True", true, "32702371-2853-4F84-99CE-0EE4726E39B8".ToLower());
                //var EncryptedPwd04 = oEncrypt.EncryptData("Data Source=LENOVO_SDNA\\SQLEXPRESS;initial catalog=CSP_Core;User Id=XIDNAADMIN; Password=XIDNAADMIN; MultipleActiveResultSets=True", true, "A428DFD2-FCAB-409E-802E-D6746A75CC60");


                var EncryptedPwd02 = oEncrypt.EncryptData("Data Source= Spot\\SQLEXPRESS;initial catalog=SDNA_XIEnvironment;User Id=SpotAdmin; Password=P@W$$W0rD; MultipleActiveResultSets=True", true, "6105A023-04DF-40A8-9669-69E75D4A6B50".ToLower());
                var EncryptedPwd03 = oEncrypt.EncryptData("Data Source= Spot\\SQLEXPRESS;initial catalog=SDNA_Setting;User Id=SpotAdmin; Password=P@W$$W0rD; MultipleActiveResultSets=True", true, "D2EF9017-0D1C-44CA-A2C0-AD0527BAAC5B".ToLower());
                var EncryptedPwd04 = oEncrypt.EncryptData("Data Source= Spot\\SQLEXPRESS;initial catalog=SDNA_Core;User Id=SpotAdmin; Password=P@W$$W0rD; MultipleActiveResultSets=True", true, "9cdb21ca-0dcb-42fd-8a68-d3aba2d7313e".ToLower());
                var EncryptedPwd05 = oEncrypt.EncryptData("Data Source= Spot\\SQLEXPRESS;initial catalog=SDNA_Shared;User Id=SpotAdmin; Password=P@W$$W0rD; MultipleActiveResultSets=True", true, "04d71f9c-5bd7-4c4a-a217-39624ebbfa09".ToLower());



                //var EncryptedPwd2 = oEncrypt.EncryptData("Data Source=WIN-IN1I7P2MED3;initial catalog=XIEnvironment_BChain;User Id=XIDNAADMIN; Password=XIDNAADMIN; MultipleActiveResultSets=True", true, "6105a023-04df-40a8-9669-69e75d4a6b50".ToLower());
                var EncryptedPwd2 = oEncrypt.EncryptData("Data Source= LENOVO_SDNA\\SQLEXPRESS;initial catalog=SDNA_XIEnvironment;User Id=XIDNAADMIN; Password=XIDNAADMIN; MultipleActiveResultSets=True", true, "6105a023-04df-40a8-9669-69e75d4a6b50".ToLower());
                var EncryptedPwd3 = oEncrypt.EncryptData("Data Source= LENOVO_SDNA\\SQLEXPRESS;initial catalog=SDNA_Setting;User Id=XIDNAADMIN; Password=XIDNAADMIN; MultipleActiveResultSets=True", true, "d2ef9017-0d1c-44ca-a2c0-ad0527baac5b".ToLower());
                var EncryptedPwd4 = oEncrypt.EncryptData("Data Source= LENOVO_SDNA\\SQLEXPRESS;initial catalog=SDNA_Core;User Id=XIDNAADMIN; Password=XIDNAADMIN; MultipleActiveResultSets=True", true, "9cdb21ca-0dcb-42fd-8a68-d3aba2d7313e".ToLower());
                var EncryptedPwd5 = oEncrypt.EncryptData("Data Source= LENOVO_SDNA\\SQLEXPRESS;initial catalog=SDNA_Shared;User Id=XIDNAADMIN; Password=XIDNAADMIN; MultipleActiveResultSets=True", true, "04d71f9c-5bd7-4c4a-a217-39624ebbfa09".ToLower());

                oUser.sUserName = model.UserName;   
                if (SessionManager.bNannoApp)
                {
                    Database = "Nanno_Core";
                    SessionManager.CoreDatabase = Database;
                }
                if (SessionManager.OrganizationID > 0)
                {
                    oUser.FKiOrganisationID = SessionManager.OrganizationID;
                }
                oUser.CheckAccessCode = System.Configuration.ConfigurationManager.AppSettings["CheckAccessCode"];
                oUser.sAccessCode = System.Configuration.ConfigurationManager.AppSettings["AccessCode"];
                oUser = (XIInfraUsers)oUser.Get_UserDetails(Database.ToString()).oResult;
                if (oUser != null && !oUser.bIsActive)
                {
                    ViewBag.Logo = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                    ViewBag.AppName = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinName"];
                    ModelState.AddModelError("", "Your account is deactivated or doesn't exist. Please contact our customer service team for assistance.");
                    return View(model);
                }
                if (!ModelState.IsValid)
                {
                    SessionManager.CoreDatabase = SessionManager.CoreDatabase; //null;
                    if (!string.IsNullOrEmpty(SessionManager.Logo))
                    {
                        ViewBag.Logo = SessionManager.Logo; //System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                    }
                    else
                    {
                        ViewBag.Logo = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                    }
                    if (!string.IsNullOrEmpty(SessionManager.Logo))
                    {
                        ViewBag.AppName = SessionManager.AppName; //System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinName"];
                    }
                    else
                    {
                        ViewBag.AppName = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinName"];
                    }
                    ModelState.AddModelError("", "Invalid Username or Password");
                    return View(model);
                }
                if (oUser != null && oUser.Role != null && oUser.Role.RoleID != 55)
                {
                    XIInfraCache oCache = new XIInfraCache();
                    var sReferenceID = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, oUser.FKiApplicationID + "_" + oUser.FKiOrganisationID + "_" + oUser.Role.RoleID + "_" + XIConstant.IDRef_internal);
                    SessionManager.sReference = sReferenceID;
                    bool bOTP = false;
                    XIDApplicationOTPSetting oOTPSetting = (XIDApplicationOTPSetting)oCacheO.GetObjectFromCache(XIConstant.CacheApplicationOTP, null, oUser.FKiApplicationIDXIGUID.ToString());
                    if (oOTPSetting.iStatus == 10)
                    {
                        bOTP = true;
                    }
                    var EncryptedPwd = oEncrypt.EncryptData(model.Password, true, oUser.UserID.ToString());
                    if (EncryptedPwd == oUser.sPasswordHash)
                    {
                        var authCookiee = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                        if (authCookiee != null)
                        {
                            FormsAuthenticationTicket authTickett = FormsAuthentication.Decrypt(authCookiee.Value);
                            if (authTickett != null && !authTickett.Expired)
                            {
                                var roles = authTickett.UserData.Split(',');
                                HttpContext.User = new System.Security.Principal.GenericPrincipal(new FormsIdentity(authTickett), roles);
                            }
                        }

                        if (authCookiee == null && bOTP)
                        {
                            GenerateOTPToUser(oUser, iMainApplicationID);
                            oUser.sOTP = sOTP;
                            oUser.sPassword = model.Password;
                            XIConfigs oConfig = new XIConfigs();
                            XIDBO oBoD = (XIDBO)oCacheO.GetObjectFromCache(XIConstant.CacheBO, "XIAPPUsers_AU_T", null);
                            //int iDataSourceID = oBoD.iDataSource;
                            Guid DataSourceXIGUID = oBoD.iDataSourceXIGUID;
                            oConfig.Save_User(oUser, DataSourceXIGUID.ToString());
                            LoginWithOtp sLoginOTP = new LoginWithOtp();
                            sLoginOTP.UserName = model.UserName;
                            sLoginOTP.Password = model.Password;
                            sLoginOTP.iUserID = oUser.UserID;
                            sLoginOTP.sLength = sOTP.Length;
                            ViewBag.Logo = SessionManager.Logo; //System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                            sLoginOTP.Identifier = !string.IsNullOrEmpty(sOTP) && sOTP.All(Char.IsDigit);
                            return View("LoginPageWithOTP", sLoginOTP);
                        }
                        //else
                        if (oUser.Role.iThemeID > 0 || (oUser.Role.iThemeIDXIGUID != null && oUser.Role.iThemeIDXIGUID != Guid.Empty))
                        {
                            if (oUser.Role.iThemeID > 0 || (oUser.Role.iThemeIDXIGUID != null && oUser.Role.iThemeIDXIGUID != Guid.Empty))
                            {
                                var sThemeName = oUser.Role.GetRoleTheme(System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinDatabase"]);
                                if (!string.IsNullOrEmpty(sThemeName))
                                {
                                    SessionManager.Theme = sThemeName;
                                }
                            }
                            //check signalr notification subscriptions
                            if (oUser.Role.bSignalR)
                            {
                                var Categories = new List<string>();
                                var sSigRCategory = string.Empty;
                                XID1Click oSub = (XID1Click)oCacheO.GetObjectFromCache(XIConstant.Cache1Click, "RoleNotificationSubscriptions", null);
                                XID1Click oSubC = (XID1Click)oSub.Clone(oSub);
                                List<CNV> nParams = new List<CNV>();
                                nParams.Add(new CNV() { sName = "{XIP|iAppID}", sValue = iMainApplicationID.ToString() });
                                nParams.Add(new CNV() { sName = "{XIP|iOrgID}", sValue = oUser.FKiOrganisationID.ToString() });
                                nParams.Add(new CNV() { sName = "{XIP|iRoleID}", sValue = oUser.Role.RoleID.ToString() });
                                oSubC.ReplaceFKExpressions(nParams);
                                oSubC.Query = oSubC.Query;
                                var Res = oSubC.OneClick_Run();
                                if (Res != null && Res.Values.Count() > 0)
                                {
                                    foreach (var item in Res.Values.ToList())
                                    {
                                        var CategoryID = item.AttributeI("FKiCategoryID").sValue;
                                        Categories.Add(CategoryID);
                                    }
                                    sSigRCategory = string.Join(",", Categories);
                                    SessionManager.sSigRCategory = sSigRCategory;
                                }
                            }
                            XIIXI oXI = new XIIXI();
                            oUser.dtLastLogin = DateTime.Now;
                            oUser.Update_User(oUser.sCoreDatabaseName);
                            if (iMainApplicationID == 37)
                            {
                                var oWhereParams = new List<CNV>();
                                oWhereParams.Add(new CNV { sName = "fkiuserid", sValue = oUser.UserID.ToString() });
                                var oSettingI = oXI.BOI("usersetting", null, null, oWhereParams);
                                if (oSettingI != null && oSettingI.Attributes.Count() > 0)
                                {
                                    if (oSettingI.Attributes.ContainsKey("imenu"))
                                    {
                                        var sMenu = oSettingI.AttributeI("imenu").sResolvedValue;
                                        if (!string.IsNullOrEmpty(sMenu))
                                        {
                                            SessionManager.MenuType = sMenu;
                                        }
                                    }
                                }
                            }
                            Singleton.Instance.sCoreDatabase = oUser.sCoreDatabaseName;
                            Singleton.Instance.sUserID = oUser.UserID.ToString();
                            Singleton.Instance.sOrgID = oUser.FKiOrganisationID.ToString();
                            Singleton.Instance.sAppName = oUser.sAppName;
                            SessionManager.sUserName = oUser.sUserName;
                            SessionManager.sEmail = oUser.sEmail;
                            SessionManager.sRoleName = oUser.Role.sRoleName;
                            SessionManager.sHierarchy = oUser.sUserHierarchy;
                            SessionManager.sUpdateHierarchy = oUser.sUserUpdateHierarchy;
                            SessionManager.sInsertDefaultCode = oUser.sInsertDefaultCode;
                            SessionManager.sViewHierarchy = oUser.sUserViewHierarchy;
                            SessionManager.sDeleteHierarchy = oUser.sUserDeleteHierarchy;
                            SessionManager.OrgDatabase = oUser.sDatabaseName;
                            SessionManager.UserID = oUser.UserID;
                            SessionManager.OrganisationName = "Org";
                            SessionManager.UserUniqueID = null;
                            SessionManager.OrganizationID = oUser.FKiOrganisationID;
                            SessionManager.iUserOrg = oUser.FKiOrganisationID;
                            SessionManager.sName = oUser.sFirstName + " " + oUser.sLastName;
                            SessionManager.iRoleID = oUser.RoleID.RoleID;
                            SessionManager.sCustomerRefNo = oUser.sCustomerRefNo;
                            SessionManager.XIGUID = Guid.NewGuid().ToString();
                            SessionManager.ApplicationID = oUser.FKiApplicationID;
                            SessionManager.ApplicationIDXIGUID = oUser.FKiApplicationIDXIGUID;
                            SessionManager.iUserLevel = oUser.iLevel;
                            SessionManager.sTeam = oUser.sTeamHierarchy;
                            SessionManager.ProjectID = oUser.FKiProjectID;
                            if (oUser.Settings != null)
                            {
                                SessionManager.bOrgSwitch = oUser.Settings.bOrgSwitch;
                                SessionManager.BColor = oUser.Settings.BColour;
                                SessionManager.FColor = oUser.Settings.FColour;
                                SessionManager.FSize = oUser.Settings.FSize;
                            }
                            FormsAuthentication.SetAuthCookie(model.UserName, false);
                            SessionManager.ConfigDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinDatabase"];
                            //var authTicket = new FormsAuthenticationTicket(1, model.UserName, DateTime.Now, DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes), false, "Admin", FormsAuthentication.FormsCookiePath);
                            //string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                            //var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
                            //{
                            //    HttpOnly = true,
                            //    Secure = FormsAuthentication.RequireSSL,
                            //    Path = FormsAuthentication.FormsCookiePath,
                            //    Domain = FormsAuthentication.CookieDomain,
                            //    Expires = DateTime.Now.AddDays(1)
                            //};
                            //HttpContext.Response.Cookies.Add(authCookie);

                            // HERE SETTING ROLENAME AND USERNAME INTO CACHE.
                            XIInfraCache oUserCache = new XIInfraCache();
                            string sSessionID = HttpContext.Session.SessionID;
                            var oCacheduser = new VM_UserLoginCache { sUserName = oUser.sUserName, sRole = oUser.Role.sRoleName };
                            oUserCache.InsertIntoCache(oCacheduser, "SSS_" + sSessionID);
                            //Session.Abandon();
                            //if (oUser.sTotpSecretKey != null)
                            if (oUser.TwoFactorEnabled)
                            {
                                EnableTOTPAuthenticatorViewModel obj = new EnableTOTPAuthenticatorViewModel();
                                obj.ManualEnterKey = oUser.sTotpSecretKey.ToString();
                                return View("LoginPageWith2FA", obj);
                            }
                            else
                            {
                                return RedirectToAction("LandingPages", "Home");
                            }
                        }
                        //if (oUser.sTotpSecretKey != null)
                        if (oUser.TwoFactorEnabled)
                        {
                            EnableTOTPAuthenticatorViewModel objs = new EnableTOTPAuthenticatorViewModel();
                            objs.ManualEnterKey = oUser.sTotpSecretKey.ToString();
                            return View("LoginPageWith2FA", objs);
                        }
                        else
                        {
                            return RedirectToAction("LandingPages", "Home");
                        }
                    }
                    else
                    {
                        XIDefinitionBase oXID = new XIDefinitionBase();
                        CResult oCResult = new CResult();
                        oCResult.sMessage = "There is a failed login attempt for username:" + oUser.sUserName;
                        oCResult.sCategory = "Failed login";
                        oXID.SaveErrortoDB(oCResult);
                        var XIAppInfo = GetApplicationInfo(Request.Url.AbsoluteUri);

                        if (XIAppInfo.ID > 0)
                        {
                            SessionManager.CoreDatabase = XIAppInfo.sDatabaseName;
                            ViewBag.Logo = XIAppInfo.sLogo;
                            ViewBag.AppName = XIAppInfo.sApplicationName;
                        }
                        else
                        {
                            SessionManager.CoreDatabase = SessionManager.CoreDatabase; //null;
                            ViewBag.Logo = SessionManager.Logo; //System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                            ViewBag.AppName = SessionManager.AppName; //System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinName"];
                        }
                        ModelState.AddModelError("", "Invalid Username or Password");
                        return View(model);
                    }
                }
                else
                {
                    XIDefinitionBase oXID = new XIDefinitionBase();
                    CResult oCResult = new CResult();
                    oCResult.sMessage = "There is a failed login attempt for username:" + oUser.sUserName;
                    oCResult.sCategory = "Failed login";
                    oXID.SaveErrortoDB(oCResult);
                    var XIAppInfo = GetApplicationInfo(Request.Url.AbsoluteUri);
                    if (XIAppInfo.ID > 0)
                    {
                        SessionManager.CoreDatabase = XIAppInfo.sDatabaseName;
                        ViewBag.Logo = XIAppInfo.sLogo;
                        ViewBag.AppName = XIAppInfo.sApplicationName;
                    }
                    else
                    {
                        SessionManager.CoreDatabase = SessionManager.CoreDatabase; //null;
                        ViewBag.Logo = SessionManager.Logo; //System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                        ViewBag.AppName = SessionManager.AppName; //System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinName"];
                    }
                    ModelState.AddModelError("", "Invalid Username or Password");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                Common.SaveErrorLog("ERROR: [Login] unable to login " + ex.ToString(), "");
                ModelState.AddModelError("", "Something went wrong!!");
                return View(model);
            }
        }
        /*===================================Start Login with OTP==================*/
        [HttpPost]
        [AllowAnonymous]
        public ActionResult GenerateOTPToUser(XIInfraUsers oUserDetails, int AppliationID, XIIBO SendingType = null, string sType = null, string sResendOTP = null)
        {
            var sOTPValue = string.Empty;
            int iOtpLength = 0;
            XIInfraCache oCache = new XIInfraCache();
            XIDApplicationOTPSetting oOTP = (XIDApplicationOTPSetting)oCache.GetObjectFromCache(XIConstant.CacheApplicationOTP, null, AppliationID.ToString());
            var sEmail = string.Empty;
            var sSMS = string.Empty;
            if (!string.IsNullOrEmpty(sType))
            {
                if (sType.ToLower() == "semail")
                {
                    sEmail = "true";
                }
                if (sType.ToLower() == "ssms")
                {
                    sSMS = "true";
                }
                if (sType == "both")
                {
                    sEmail = "true";
                    sSMS = "true";
                }

                if (!string.IsNullOrEmpty(sResendOTP))
                    sOTPValue = sResendOTP;
            }
            else
            {
                sEmail = oOTP.bMail.ToString(); //SendingType.Attributes.Where(u => u.Key.ToLower() == "bmail").Select(y => y.Value.sValue).FirstOrDefault();
                sSMS = oOTP.bSMS.ToString(); //SendingType.Attributes.Where(u => u.Key.ToLower() == "bsms").Select(y => y.Value.sValue).FirstOrDefault();
                var OTPType = oOTP.iOTPType; //SendingType.Attributes.Where(u => u.Key.ToLower() == "iotptype").Select(y => y.Value.sValue).FirstOrDefault();
                var OTPCase = oOTP.iOTPCase; //SendingType.Attributes.Where(u => u.Key.ToLower() == "iotpcase").Select(y => y.Value.sValue).FirstOrDefault();
                var iOTPLength = oOTP.iOTPLength; //SendingType.Attributes.Where(u => u.Key.ToLower() == "iotplength").Select(y => y.Value.sValue).FirstOrDefault();
                //int.TryParse(OTPLength, out iOtpLength);
                ViewBag.length = iOTPLength;
                XIConfigs oConfig = new XIConfigs();
                // XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "Generate OTP");
                var sAlphaNumerics = string.Empty;
                var LengthOTP = new char[iOTPLength];
                var sSelectRandom = new Random();
                if (OTPType == 10)
                {
                    sAlphaNumerics = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
                }
                else if (OTPType == 20)
                {
                    sAlphaNumerics = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                }
                else
                {
                    sAlphaNumerics = "1234567890";
                }
                for (int i = 0; i < LengthOTP.Length; i++)
                {
                    LengthOTP[i] = sAlphaNumerics[sSelectRandom.Next(sAlphaNumerics.Length)];
                }

                sOTPValue = new String(LengthOTP);
                if (OTPCase == 20)
                {
                    sOTPValue = sOTPValue.ToLower();
                }
                else if (OTPCase == 30)
                {
                    sOTPValue = sOTPValue.ToUpper();
                }
            }
            if (!string.IsNullOrEmpty(sEmail) && sEmail.ToLower() == "true")
            {
                ViewBag.Email = "email";
                string sTemplateID = "LoginOTP";
                XIContentEditors oContent = new XIContentEditors();
                List<XIContentEditors> oContentDef = new List<XIContentEditors>();
                if (!string.IsNullOrEmpty(sTemplateID))
                {
                    oContentDef = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, sTemplateID, null);
                    if (oContentDef != null && oContentDef.Count() > 0)
                    {
                        oContent = oContentDef.FirstOrDefault();
                    }
                }
                XIInfraEmail oEmail = new XIInfraEmail();
                oEmail.EmailID = oUserDetails.sEmail;
                string sContext = XIConstant.Lead_Transfer;
                string sNewGUID = Guid.NewGuid().ToString();
                XIIBO oBOI = new XIIBO();
                XIBOInstance oBOIInstance = new XIBOInstance();
                oBOIInstance.oStructureInstance = new Dictionary<string, List<XIIBO>>();
                //oBOI.SetAttribute("sUserName", FirstName + " " + LastName);
                //oBOI.SetAttribute("sEmail", sUserName);
                oBOI.SetAttribute("sOTP", sOTPValue);
                List<XIIBO> oBOIList = new List<XIIBO>();
                oBOI.XIIValues = oBOI.Attributes.ToDictionary(x => x.Value.sName.ToLower(), x => new XIIValue { sValue = x.Value.sValue });
                oBOIList.Add(oBOI);
                oBOIInstance.oStructureInstance["GenerateOTP"] = oBOIList;
                XIContentEditors oConent = new XIContentEditors();
                oEmail.sSubject = oContent.sSubject;
                var Result = new CResult();
                Result = oConent.MergeContentTemplate(oContent, oBOIInstance);
                var oMailResult = oEmail.Sendmail(oContent.OrganizationID, Result.oResult.ToString(), null, 0, sContext, 0, null, 0, oContent.bIsBCCOnly);//send mail with attachments
                if (oMailResult.bOK && oMailResult.oResult != null)
                {
                    //oCResult.oTraceStack.Add(new CNV { sName = "Mail send successfully", sValue = "Mail send successfully to email:" + oEmail.EmailID + "" });
                    // oXID.SaveErrortoDB(oCResult);
                }
            }
            if (sSMS.ToLower() == "true" && !string.IsNullOrEmpty(sSMS))
            {
                ViewBag.SMS = "SMS";
                string message = "Your OTP is " + sOTPValue + " ( Sent By : XI )";
                Util.SentOTPToUser(oUserDetails.sPhoneNumber, message);
            }
            Session["OTP"] = sOTPValue;
            oUserDetails.sOTP = sOTPValue;
            sOTP = sOTPValue;
            return null;
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult LoginPageWith2FA(EnableTOTPAuthenticatorViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var status = XIDNATOTPGenerator.VerifyOTP(model.ManualEnterKey, model.OTP);
                //if (status)
                //{
                    return RedirectToAction("LandingPages", "Home");
               // }
                ModelState.AddModelError("", "Invalid OTP! Please try again");
                return View(model);
            }
            ModelState.AddModelError("", "Input filed is required");
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult LoginPageWithOTP(LoginWithOtp model)
        {
            ViewBag.Logo = SessionManager.Logo;
            XIInfraUsers oUser = new XIInfraUsers();
            CResult Result = new CResult();
            XIInfraCache oCache = new XIInfraCache();
            XID1Click o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "Get OTP from User", null);
            XID1Click o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
            XIDStructure oXIDStructure = new XIDStructure();
            List<CNV> nParams = new List<CNV>();
            CNV nvpairs = new CNV();
            nvpairs.sName = "{XIP|UserID}";
            nvpairs.sValue = model.iUserID.ToString();
            nParams.Add(nvpairs);
            o1ClickC.Query = oXIDStructure.ReplaceExpressionWithCacheValue(o1ClickC.Query, nParams);
            Result = o1ClickC.GetList();
            var UserOTP = string.Empty;
            if (Result.bOK == true && Result.oResult != null)
            {
                var o1ClickResult = ((Dictionary<string, XIIBO>)Result.oResult).Values.FirstOrDefault();
                if (o1ClickResult != null)
                {
                    UserOTP = o1ClickResult.Attributes.Where(u => u.Key.ToLower() == "sotp").Select(y => y.Value.sValue).FirstOrDefault();
                }
            }
            var oMergeOTP = string.Empty;
            //if (Session["OTP"].ToString() == model.OTP)
            if (model.OTP != null)
            {
                foreach (var item in model.OTP)
                {
                    oMergeOTP += item;
                }
            }
            if (UserOTP == oMergeOTP/* || Session["OTP"].ToString() == oMergeOTP*/)
            {
                //Session["OTP"] = null;
                var Database = SessionManager.CoreDatabase;
                XIInfraEncryption oEncrypt = new XIInfraEncryption();
                //var EncryptedPwd = oEncrypt.EncryptData("Admin.123", true, 255.ToString());
                oUser.sUserName = model.UserName;
                oUser.CheckAccessCode = System.Configuration.ConfigurationManager.AppSettings["CheckAccessCode"];
                oUser.sAccessCode = System.Configuration.ConfigurationManager.AppSettings["AccessCode"];
                oUser = (XIInfraUsers)oUser.Get_UserDetails(Database.ToString()).oResult;

                if (oUser.Role.iThemeID > 0)
                {
                    var sThemeName = oUser.Role.GetRoleTheme(System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinDatabase"]);
                    if (!string.IsNullOrEmpty(sThemeName))
                    {
                        SessionManager.Theme = sThemeName;
                    }
                }
                XIIXI oXI = new XIIXI();
                oUser.dtLastLogin = DateTime.Now;
                oUser.Update_User(oUser.sCoreDatabaseName);
                if (oUser.FKiApplicationID == 37)
                {
                    var oWhereParams = new List<CNV>();
                    oWhereParams.Add(new CNV { sName = "fkiuserid", sValue = oUser.UserID.ToString() });
                    var oSettingI = oXI.BOI("usersetting", null, null, oWhereParams);
                    if (oSettingI != null && oSettingI.Attributes.Count() > 0)
                    {
                        if (oSettingI.Attributes.ContainsKey("imenu"))
                        {
                            var sMenu = oSettingI.AttributeI("imenu").sResolvedValue;
                            if (!string.IsNullOrEmpty(sMenu))
                            {
                                SessionManager.MenuType = sMenu;
                            }
                        }
                    }
                }
                Singleton.Instance.sCoreDatabase = oUser.sCoreDatabaseName;
                Singleton.Instance.sUserID = oUser.UserID.ToString();
                Singleton.Instance.sOrgID = oUser.FKiOrganisationID.ToString();
                Singleton.Instance.sAppName = oUser.sAppName;
                SessionManager.sUserName = oUser.sUserName;
                SessionManager.sEmail = oUser.sEmail;
                SessionManager.sRoleName = oUser.Role.sRoleName;
                SessionManager.sHierarchy = oUser.sHierarchy;
                SessionManager.sUpdateHierarchy = oUser.sUpdateHierarchy;
                SessionManager.sInsertDefaultCode = oUser.sInsertDefaultCode;
                SessionManager.sViewHierarchy = oUser.sViewHierarchy;
                SessionManager.sDeleteHierarchy = oUser.sDeleteHierarchy;
                SessionManager.OrgDatabase = oUser.sDatabaseName;
                SessionManager.UserID = oUser.UserID;
                SessionManager.OrganisationName = "Org";
                SessionManager.UserUniqueID = null;
                SessionManager.OrganizationID = oUser.FKiOrganisationID;
                SessionManager.sName = oUser.sFirstName + " " + oUser.sLastName;
                SessionManager.iRoleID = oUser.RoleID.RoleID;
                SessionManager.sCustomerRefNo = oUser.sCustomerRefNo;
                SessionManager.XIGUID = Guid.NewGuid().ToString();
                SessionManager.ApplicationID = oUser.FKiApplicationID;
                SessionManager.sTeam = oUser.sTeamHierarchy;
                if (oUser.Settings != null)
                {
                    SessionManager.bOrgSwitch = oUser.Settings.bOrgSwitch;
                    SessionManager.BColor = oUser.Settings.BColour;
                    SessionManager.FColor = oUser.Settings.FColour;
                    SessionManager.FSize = oUser.Settings.FSize;
                }
                FormsAuthentication.SetAuthCookie(model.UserName, false);
                SessionManager.ConfigDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinDatabase"];
                var authTicket = new FormsAuthenticationTicket(1, model.UserName, DateTime.Now, DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes), false, "Admin", FormsAuthentication.FormsCookiePath);
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
                {
                    HttpOnly = true,
                    Secure = FormsAuthentication.RequireSSL,
                    Path = FormsAuthentication.FormsCookiePath,
                    Domain = FormsAuthentication.CookieDomain,
                    Expires = DateTime.Now.AddDays(1)
                };
                HttpContext.Response.Cookies.Add(authCookie);

                // HERE SETTING ROLENAME AND USERNAME INTO CACHE.
                XIInfraCache oUserCache = new XIInfraCache();
                string sSessionID = HttpContext.Session.SessionID;
                var oCacheduser = new VM_UserLoginCache { sUserName = oUser.sUserName, sRole = oUser.Role.sRoleName };
                oUserCache.InsertIntoCache(oCacheduser, "SSS_" + sSessionID);

                return RedirectToAction("LandingPages", "Home");
            }
            else
            {
                model.Identifier = !string.IsNullOrEmpty(UserOTP) && UserOTP.All(Char.IsDigit);
                model.sLength = UserOTP.Length;
                ModelState.AddModelError("", "Invalid OTP");
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResendOTP(string UserID, string ApplicationID, string sSMS, string sEmail)
        {
            var sSendingType = string.Empty;
            if (!string.IsNullOrEmpty(sEmail) && sEmail.ToLower() == "email" && !string.IsNullOrEmpty(sSMS) && sSMS.ToLower() == "sms")
            {
                sSendingType = "both";
            }
            else if (!string.IsNullOrEmpty(sEmail) && sEmail.ToLower() == "email")
            {
                sSendingType = "sEmail";
            }
            else if (!string.IsNullOrEmpty(sSMS) && sSMS.ToLower() == "sms")
            {
                sSendingType = "sSMS";
            }
            var Database = SessionManager.CoreDatabase;
            XIInfraUsers oUser = new XIInfraUsers();
            oUser = (XIInfraUsers)oUser.Get_UserDetails(Database.ToString(), Convert.ToInt32(UserID)).oResult;
            var ResendOTP = oUser.sOTP;
            GenerateOTPToUser(oUser, Convert.ToInt32(ApplicationID), null, sSendingType, ResendOTP);
            return null;
        }
        /*===================================END Login with OTP==================*/
        //[HttpPost]
        [AllowAnonymous]
        public ActionResult RedirectToClientPage(string UserID = null, string QSInstanceID = null)
        {
            XIInfraEncryption oEncrypt = new XIInfraEncryption();
            var pwd = "";
            LoginViewModel model = new LoginViewModel();
            cXIAppUsers User = new cXIAppUsers();
            string sUserName = string.Empty;
            if (string.IsNullOrEmpty(UserID))
            {
                //UserID = SessionManager.UserID.ToString();
                sUserName = SessionManager.sEmail;
            }
            else
            {
                int user = Convert.ToInt32(UserID);
                User = dbContext.XIAppUsers.Find(user);
                model.UserName = User.sEmail;
                pwd = oEncrypt.DecryptData(User.sPasswordHash, true, UserID);
            }
            var Database = "xicoreqa_live";//SessionManager.CoreDatabase;
            List<CNV> oParams = new List<CNV>();
            //oParams.Add(new CNV { sName = "Description", sValue = "Tried to Login" });
            //oParams.Add(new CNV { sName = "Email", sValue = model.UserName });
            XIMatrix oMatrix = new XIMatrix();
            oMatrix.MatrixAction(xiEnumSystem.EnumMatrixAction.ClientLogin.ToString(), xiEnumSystem.EnumMatrixAction.ClientLogin, null, 0, "0", "Client Login", oParams);
            XIInfraUsers oUser = new XIInfraUsers();
            //var EncryptedPwd = oEncrypt.EncryptData("Admin.123", true, 255.ToString());
            oUser.sUserName = model.UserName == null ? sUserName : model.UserName;
            oUser.CheckAccessCode = System.Configuration.ConfigurationManager.AppSettings["CheckAccessCode"];
            oUser.sAccessCode = System.Configuration.ConfigurationManager.AppSettings["AccessCode"];
            oUser = (XIInfraUsers)oUser.Get_UserDetails(Database.ToString()).oResult;
            if (oUser != null && !oUser.bIsActive)
            {
                ViewBag.Logo = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                ViewBag.AppName = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinName"];
                ModelState.AddModelError("", "Your account is deactivated or doesn't exist. Please contact our customer service team for assistance.");
                return View(model);
            }
            if (!ModelState.IsValid)
            {
                SessionManager.CoreDatabase = SessionManager.CoreDatabase; //null;
                if (!string.IsNullOrEmpty(SessionManager.Logo))
                {
                    ViewBag.Logo = SessionManager.Logo; //System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                }
                else
                {
                    ViewBag.Logo = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                }
                if (!string.IsNullOrEmpty(SessionManager.Logo))
                {
                    ViewBag.AppName = SessionManager.AppName; //System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinName"];
                }
                else
                {
                    ViewBag.AppName = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinName"];
                }
                ModelState.AddModelError("", "Invalid Username or Password");
                return View(model);
            }
            if (oUser != null && oUser.Role != null && oUser.Role.RoleID == 55)
            {
                if (string.IsNullOrEmpty(model.UserName))
                    pwd = oEncrypt.DecryptData(oUser.sPasswordHash, true, oUser.UserID.ToString());
                var EncryptedPwd = oEncrypt.EncryptData(pwd, true, oUser.UserID.ToString());
                if (EncryptedPwd == oUser.sPasswordHash)
                {
                    if (oUser.Role.iThemeID > 0)
                    {
                        var sThemeName = oUser.Role.GetRoleTheme(System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinDatabase"]);
                        if (!string.IsNullOrEmpty(sThemeName))
                        {
                            SessionManager.Theme = sThemeName;
                        }
                    }
                    oUser.dtLastLogin = DateTime.Now;
                    oUser.Update_User(oUser.sCoreDatabaseName);
                    if (!string.IsNullOrEmpty(model.sTheme))
                    {
                        SessionManager.Theme = model.sTheme;
                    }
                    Singleton.Instance.sCoreDatabase = oUser.sCoreDatabaseName;
                    Singleton.Instance.sUserID = oUser.UserID.ToString();
                    Singleton.Instance.sOrgID = oUser.FKiOrganisationID.ToString();
                    Singleton.Instance.sAppName = oUser.sAppName;
                    //SessionManager.AppName = oUser.sAppName;
                    SessionManager.sUserName = oUser.sUserName;
                    SessionManager.sEmail = oUser.sEmail;
                    SessionManager.sRoleName = oUser.Role.sRoleName;
                    SessionManager.OrgDatabase = oUser.sDatabaseName;
                    SessionManager.UserID = oUser.UserID;
                    SessionManager.OrganisationName = "Org";
                    SessionManager.UserUniqueID = null;
                    SessionManager.OrganizationID = oUser.FKiOrganisationID;
                    SessionManager.sName = oUser.sFirstName + " " + oUser.sLastName;
                    SessionManager.iRoleID = oUser.RoleID.RoleID;
                    SessionManager.sCustomerRefNo = oUser.sCustomerRefNo;
                    SessionManager.XIGUID = Guid.NewGuid().ToString();
                    SessionManager.ConfigDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinDatabase"];
                    FormsAuthentication.SetAuthCookie(model.UserName == null ? oUser.sUserName : model.UserName, false);
                    var authTicket = new FormsAuthenticationTicket(1, model.UserName == null ? oUser.sUserName : model.UserName, DateTime.Now, DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes), false, "Admin", FormsAuthentication.FormsCookiePath);
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
                    {
                        HttpOnly = true,
                        Secure = FormsAuthentication.RequireSSL,
                        Path = FormsAuthentication.FormsCookiePath,
                        Domain = FormsAuthentication.CookieDomain,
                        Expires = DateTime.Now.AddDays(1)
                    };
                    HttpContext.Response.Cookies.Add(authCookie);

                    // HERE SETTING ROLENAME AND USERNAME INTO CACHE.
                    XIInfraCache oUserCache = new XIInfraCache();
                    string sSessionID = HttpContext.Session.SessionID;
                    var oCacheduser = new VM_UserLoginCache { sUserName = oUser.sUserName, sRole = oUser.Role.sRoleName };
                    oUserCache.InsertIntoCache(oCacheduser, "SSS_" + sSessionID);
                    oParams = new List<CNV>();
                    oParams.Add(new CNV { sName = "Description", sValue = "Client Login Successful" });
                    oParams.Add(new CNV { sName = "Email", sValue = model.UserName == null ? oUser.sUserName : model.UserName });
                    oMatrix = new XIMatrix();
                    oMatrix.MatrixAction(xiEnumSystem.EnumMatrixAction.ClientLogin.ToString(), xiEnumSystem.EnumMatrixAction.ClientLogin, null, 0, "0", "Client Login", oParams);
                    //Session.Abandon();
                    XIIXI oXI = new XIIXI();
                    List<CNV> oWhereParams = new List<CNV>();
                    oWhereParams.Add(new CNV { sName = "FKiQSInstanceID", sValue = QSInstanceID });
                    var oLeadI = oXI.BOI("Lead_T", null, null, oWhereParams);
                    return RedirectToAction("LandingPages", "Home", new { XilinkId = "9517", LeadID = oLeadI.AttributeI("id").sValue, Userid = UserID });
                }
                else
                {
                    var XIAppInfo = GetApplicationInfo(Request.Url.AbsoluteUri);

                    if (XIAppInfo.ID > 0)
                    {
                        SessionManager.CoreDatabase = XIAppInfo.sDatabaseName;
                        ViewBag.Logo = XIAppInfo.sLogo;
                        ViewBag.AppName = XIAppInfo.sApplicationName;
                    }
                    else
                    {
                        SessionManager.CoreDatabase = SessionManager.CoreDatabase; //null;
                        ViewBag.Logo = SessionManager.Logo; //System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                        ViewBag.AppName = SessionManager.AppName; //System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinName"];
                    }
                    oParams = new List<CNV>();
                    oParams.Add(new CNV { sName = "Description", sValue = "Incorrect password entered" });
                    oParams.Add(new CNV { sName = "Email", sValue = model.UserName });
                    oMatrix = new XIMatrix();
                    oMatrix.MatrixAction(xiEnumSystem.EnumMatrixAction.ClientLogin.ToString(), xiEnumSystem.EnumMatrixAction.ClientLogin, null, 0, "0", "Client Login", oParams);
                    ModelState.AddModelError("", "Invalid Username or Password");
                    return View(model);
                }
            }
            else
            {
                var XIAppInfo = GetApplicationInfo(Request.Url.AbsoluteUri);
                if (XIAppInfo.ID > 0)
                {
                    SessionManager.CoreDatabase = XIAppInfo.sDatabaseName;
                    ViewBag.Logo = XIAppInfo.sLogo;
                    ViewBag.AppName = XIAppInfo.sApplicationName;
                }
                else
                {
                    SessionManager.CoreDatabase = SessionManager.CoreDatabase; //null;
                    ViewBag.Logo = SessionManager.Logo; //System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                    ViewBag.AppName = SessionManager.AppName; //System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinName"];
                }
                oParams = new List<CNV>();
                oParams.Add(new CNV { sName = "Description", sValue = "Invalid username or password entered" });
                oParams.Add(new CNV { sName = "Email", sValue = model.UserName });
                oMatrix = new XIMatrix();
                oMatrix.MatrixAction(xiEnumSystem.EnumMatrixAction.ClientLogin.ToString(), xiEnumSystem.EnumMatrixAction.ClientLogin, null, 0, "0", "Client Login", oParams);
                ModelState.AddModelError("", "Invalid Username or Password");
                return View(model);
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ClientLogin(LoginViewModel model, string returnUrl)
        {
            if (Request.UrlReferrer.Query.Length > 0)
            {
                var val = Request.UrlReferrer.Query.Substring(1, Request.UrlReferrer.Query.Length - 1).Split('&');//.QueryString["UserID"];
                string UserID = val.Where(x => x.StartsWith("UserID")).Select(t => t.Split('=')[1]).FirstOrDefault();
                string QSInstanceID = val.Where(x => x.StartsWith("QSInstanceID")).Select(t => t.Split('=')[1]).FirstOrDefault();
                if (!string.IsNullOrEmpty(UserID) && !string.IsNullOrEmpty(QSInstanceID))
                    return RedirectToAction("RedirectToClientPage", new RouteValueDictionary(
        new { controller = "Account", action = "RedirectToClientPage", UserID = UserID, QSInstanceID = QSInstanceID }));
            }
            var Database = SessionManager.CoreDatabase;
            List<CNV> oParams = new List<CNV>();
            oParams.Add(new CNV { sName = "Description", sValue = "Tried to Login" });
            oParams.Add(new CNV { sName = "Email", sValue = model.UserName });
            XIMatrix oMatrix = new XIMatrix();
            oMatrix.MatrixAction(xiEnumSystem.EnumMatrixAction.ClientLogin.ToString(), xiEnumSystem.EnumMatrixAction.ClientLogin, null, 0, "0", "Client Login", oParams);
            XIInfraUsers oUser = new XIInfraUsers();
            XIInfraEncryption oEncrypt = new XIInfraEncryption();
            //var EncryptedPwd = oEncrypt.EncryptData("Admin.123", true, 255.ToString());
            oUser.sUserName = model.UserName;
            oUser.CheckAccessCode = System.Configuration.ConfigurationManager.AppSettings["CheckAccessCode"];
            oUser.sAccessCode = System.Configuration.ConfigurationManager.AppSettings["AccessCode"];
            oUser = (XIInfraUsers)oUser.Get_UserDetails(Database.ToString()).oResult;
            if (oUser != null && !oUser.bIsActive)
            {
                ViewBag.Logo = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                ViewBag.AppName = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinName"];
                ModelState.AddModelError("", "Your account is deactivated or doesn't exist. Please contact our customer service team for assistance.");
                return View(model);
            }
            if (!ModelState.IsValid)
            {
                SessionManager.CoreDatabase = SessionManager.CoreDatabase; //null;
                if (!string.IsNullOrEmpty(SessionManager.Logo))
                {
                    ViewBag.Logo = SessionManager.Logo; //System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                }
                else
                {
                    ViewBag.Logo = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                }
                if (!string.IsNullOrEmpty(SessionManager.Logo))
                {
                    ViewBag.AppName = SessionManager.AppName; //System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinName"];
                }
                else
                {
                    ViewBag.AppName = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinName"];
                }
                ModelState.AddModelError("", "Invalid Username or Password");
                return View(model);
            }
            if (oUser != null && oUser.Role != null && oUser.Role.RoleID == 55)
            {
                var EncryptedPwd = oEncrypt.EncryptData(model.Password, true, oUser.UserID.ToString());
                if (EncryptedPwd == oUser.sPasswordHash)
                {
                    if (oUser.Role.iThemeID > 0)
                    {
                        var sThemeName = oUser.Role.GetRoleTheme(System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinDatabase"]);
                        if (!string.IsNullOrEmpty(sThemeName))
                        {
                            SessionManager.Theme = sThemeName;
                        }
                    }
                    oUser.dtLastLogin = DateTime.Now;
                    oUser.Update_User(oUser.sCoreDatabaseName);
                    if (!string.IsNullOrEmpty(model.sTheme))
                    {
                        SessionManager.Theme = model.sTheme;
                    }
                    Singleton.Instance.sCoreDatabase = oUser.sCoreDatabaseName;
                    Singleton.Instance.sUserID = oUser.UserID.ToString();
                    Singleton.Instance.sOrgID = oUser.FKiOrganisationID.ToString();
                    Singleton.Instance.sAppName = oUser.sAppName;
                    //SessionManager.AppName = oUser.sAppName;
                    SessionManager.sUserName = oUser.sUserName;
                    SessionManager.sEmail = oUser.sEmail;
                    SessionManager.sRoleName = oUser.Role.sRoleName;
                    SessionManager.OrgDatabase = oUser.sDatabaseName;
                    SessionManager.UserID = oUser.UserID;
                    SessionManager.OrganisationName = "Org";
                    SessionManager.UserUniqueID = null;
                    SessionManager.OrganizationID = oUser.FKiOrganisationID;
                    SessionManager.sName = oUser.sFirstName + " " + oUser.sLastName;
                    SessionManager.iRoleID = oUser.RoleID.RoleID;
                    SessionManager.sCustomerRefNo = oUser.sCustomerRefNo;
                    SessionManager.ApplicationID = oUser.FKiApplicationID;
                    SessionManager.XIGUID = Guid.NewGuid().ToString();
                    SessionManager.ConfigDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinDatabase"];
                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                    var authTicket = new FormsAuthenticationTicket(1, model.UserName, DateTime.Now, DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes), false, "Admin", FormsAuthentication.FormsCookiePath);
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
                    {
                        HttpOnly = true,
                        Secure = FormsAuthentication.RequireSSL,
                        Path = FormsAuthentication.FormsCookiePath,
                        Domain = FormsAuthentication.CookieDomain,
                        Expires = DateTime.Now.AddDays(1)
                    };
                    HttpContext.Response.Cookies.Add(authCookie);

                    // HERE SETTING ROLENAME AND USERNAME INTO CACHE.
                    XIInfraCache oUserCache = new XIInfraCache();
                    string sSessionID = HttpContext.Session.SessionID;
                    var oCacheduser = new VM_UserLoginCache { sUserName = oUser.sUserName, sRole = oUser.Role.sRoleName };
                    oUserCache.InsertIntoCache(oCacheduser, "SSS_" + sSessionID);
                    oParams = new List<CNV>();
                    oParams.Add(new CNV { sName = "Description", sValue = "Client Login Successful" });
                    oParams.Add(new CNV { sName = "Email", sValue = model.UserName });
                    oMatrix = new XIMatrix();
                    oMatrix.MatrixAction(xiEnumSystem.EnumMatrixAction.ClientLogin.ToString(), xiEnumSystem.EnumMatrixAction.ClientLogin, null, 0, "0", "Client Login", oParams);
                    //Session.Abandon();
                    return RedirectToAction("LandingPages", "Home");
                }
                else
                {
                    var XIAppInfo = GetApplicationInfo(Request.Url.AbsoluteUri);

                    if (XIAppInfo.ID > 0)
                    {
                        SessionManager.CoreDatabase = XIAppInfo.sDatabaseName;
                        ViewBag.Logo = XIAppInfo.sLogo;
                        ViewBag.AppName = XIAppInfo.sApplicationName;
                    }
                    else
                    {
                        SessionManager.CoreDatabase = SessionManager.CoreDatabase; //null;
                        ViewBag.Logo = SessionManager.Logo; //System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                        ViewBag.AppName = SessionManager.AppName; //System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinName"];
                    }
                    oParams = new List<CNV>();
                    oParams.Add(new CNV { sName = "Description", sValue = "Incorrect password entered" });
                    oParams.Add(new CNV { sName = "Email", sValue = model.UserName });
                    oMatrix = new XIMatrix();
                    oMatrix.MatrixAction(xiEnumSystem.EnumMatrixAction.ClientLogin.ToString(), xiEnumSystem.EnumMatrixAction.ClientLogin, null, 0, "0", "Client Login", oParams);
                    ModelState.AddModelError("", "Invalid Username or Password");
                    return View(model);
                }
            }
            else
            {
                var XIAppInfo = GetApplicationInfo(Request.Url.AbsoluteUri);
                if (XIAppInfo.ID > 0)
                {
                    SessionManager.CoreDatabase = XIAppInfo.sDatabaseName;
                    ViewBag.Logo = XIAppInfo.sLogo;
                    ViewBag.AppName = XIAppInfo.sApplicationName;
                }
                else
                {
                    SessionManager.CoreDatabase = SessionManager.CoreDatabase; //null;
                    ViewBag.Logo = SessionManager.Logo; //System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                    ViewBag.AppName = SessionManager.AppName; //System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinName"];
                }
                oParams = new List<CNV>();
                oParams.Add(new CNV { sName = "Description", sValue = "Invalid username or password entered" });
                oParams.Add(new CNV { sName = "Email", sValue = model.UserName });
                oMatrix = new XIMatrix();
                oMatrix.MatrixAction(xiEnumSystem.EnumMatrixAction.ClientLogin.ToString(), xiEnumSystem.EnumMatrixAction.ClientLogin, null, 0, "0", "Client Login", oParams);
                ModelState.AddModelError("", "Invalid Username or Password");
                return View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult ClientLogin(string returnUrl, string SAPN)
        {
            try
            {
                if (!string.IsNullOrEmpty(SAPN))
                {
                    SessionManager.AppName = SAPN;
                }
                if (string.IsNullOrEmpty(SessionManager.Theme))
                {
                    SessionManager.Theme = System.Configuration.ConfigurationManager.AppSettings["ThemeFile"];
                }
                SessionManager.UserID = 0;
                ViewBag.ReturnUrl = returnUrl;
                //FormsAuthentication.SignOut();

                if (!string.IsNullOrEmpty(SessionManager.AppName))
                {
                    //SessionManager.CoreDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinDatabase"];
                    if (!string.IsNullOrEmpty(SessionManager.Logo))
                    {
                        ViewBag.Logo = SessionManager.Logo;
                    }
                    else
                    {
                        ViewBag.Logo = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                    }
                    ViewBag.AppName = SessionManager.AppName;
                }
                else
                {
                    SessionManager.CoreDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreDataBase"];
                    //SessionManager.CoreDatabase = SessionManager.CoreDatabase; //System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinDatabase"];
                    ViewBag.Logo = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                    ViewBag.AppName = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinName"];
                }
                SessionManager.CoreDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreDataBase"];
                //SessionManager.CoreDatabase = SessionManager.CoreDatabase; //System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinDatabase"];
                ViewBag.Logo = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                ViewBag.AppName = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinName"];
                return View();
            }
            catch (Exception ex)
            {
                //logger.Error(ex);
                //Common.SaveErrorLog(ex.ToString());
                ModelState.AddModelError("", "Error Occured");
                return View();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterXIUser(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (SessionManager.CoreDatabase != null)
                {
                    dbContext = new ModelDbContext(SessionManager.CoreDatabase);
                }
                cXIAppUsers User = new cXIAppUsers();
                User.sUserName = model.UserName;
                dbContext.XIAppUsers.Add(User);
                dbContext.SaveChanges();
                var EncryptedPwd = oXIAPI.EncryptData(model.Password, true, User.UserID.ToString());
                User = dbContext.XIAppUsers.Find(User.UserID);
                User.sPasswordHash = EncryptedPwd;
                dbContext.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("Register", model);
            }
        }


        //
        // GET: /Account/Login
        //[AllowAnonymous]
        //public ActionResult Login(string returnUrl)
        //{
        //    ViewBag.ReturnUrl = returnUrl;
        //    return View();
        //}

        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }

        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            var user = await UserManager.FindByIdAsync(await SignInManager.GetVerifiedUserIdAsync());
            if (user != null)
            {
                ViewBag.Status = "For DEMO purposes the current " + provider + " code is: " + await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl });
        }

        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: false, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.UserName };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                    ViewBag.Link = callbackUrl;
                    return View("DisplayEmail");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //Register Nanno User
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterNannoUser(RegisterViewModel model)
        {
            XIInfraUsers oUser = new XIInfraUsers();
            XIInfraEncryption xifEncrypt = new XIInfraEncryption();
            XIConfigs oConfig = new XIConfigs();
            var AppID = SessionManager.ApplicationID;
            var OrgID = SessionManager.OrganizationID;
            XIInfraCache oCache = new XIInfraCache();
            var oAppD = (XIDApplication)oCache.GetObjectFromCache(XIConstant.CacheApplication, null, AppID.ToString());
            if (oAppD.bNannoApp)
            {
                XIDXI oXID = new XIDXI();
                XIIXI oXI = new XIIXI();
                oXID.sOrgDatabase = "Nanno_Core";
                var oCR = oXID.Get_OrgDefinition(null, OrgID.ToString());
                if (oCR.bOK && oCR.oResult != null)
                {
                    XIDOrganisation oOrgD = (XIDOrganisation)oCR.oResult;
                    if (oOrgD.bNannoApp)
                    {
                        List<CNV> oPrams = new List<CNV>();
                        oPrams.Add(new CNV { sName = "FKiOrgID", sValue = OrgID.ToString() });
                        var NannoAppInst = oXI.BOI("Nanno App Instance", null, null, oPrams);
                        if (NannoAppInst != null && NannoAppInst.Attributes.Count() > 0)
                        {
                            int iNannoAppInstID = 0;
                            var NannoAppInstID = NannoAppInst.AttributeI("ID").sValue;
                            int.TryParse(NannoAppInstID, out iNannoAppInstID);
                            if (iNannoAppInstID > 0)
                            {
                                oUser.sUserName = model.Email;
                                oUser = (XIInfraUsers)oUser.Get_UserDetails("Nanno_Core").oResult;
                                if (oUser != null && oUser.UserID > 0)
                                {
                                    oPrams = new List<CNV>();
                                    oPrams.Add(new CNV { sName = "iUserID", sValue = oUser.UserID.ToString() });
                                    oPrams.Add(new CNV { sName = "iNannoAppInstID", sValue = iNannoAppInstID.ToString() });
                                    oCR = oConfig.Register_NannoUser(oPrams);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        var sRespMsg = (string)oCR.oResult;
                                        return RedirectToAction("Login", "Account");
                                    }
                                    else
                                    {
                                        return View("Register", model);
                                    }
                                }
                                else
                                {
                                    oUser = new XIInfraUsers();
                                    oUser.sUserName = model.Email;
                                    oUser.sEmail = model.Email;
                                    oUser.sFirstName = model.FirstName;
                                    oUser.sLastName = model.LastName;
                                    oUser.FKiOrganisationID = OrgID;
                                    oUser.FKiApplicationID = AppID;
                                    //oUser.sDatabaseName = sDatabase;
                                    oUser.sDatabaseName = oOrgD.DatabaseName;
                                    oUser.sCoreDatabaseName = "Nanno_Core";
                                    oUser.sPhoneNumber = model.PhoneNumber;
                                    oUser.iReportTo = model.ReportTo;
                                    oUser.LockoutEndDateUtc = DateTime.Now;
                                    oUser.sLocation = "";
                                    oUser.iPaginationCount = 10;
                                    oUser.sMenu = "Open,Open";
                                    oUser.iInboxRefreshTime = 0;
                                    oUser.CreatedBy = oUser.UpdatedBy = 0;
                                    oUser.CreatedTime = DateTime.Now;
                                    oUser.CreatedBySYSID = oUser.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                                    oUser.StatusTypeID = model.StatusTypeID;
                                    oUser.sHierarchy = model.sHierarchy;
                                    oUser.sInsertDefaultCode = model.sInsertDefaultCode;
                                    oUser.sUpdateHierarchy = model.sUpdateHierarchy;
                                    oUser.sViewHierarchy = model.sViewHierarchy;
                                    oUser.sDeleteHierarchy = model.sDeleteHierarchy;
                                    oUser.sTeamHierarchy = model.FKiTeamID.ToString();
                                    var oUserData = oUser.Save_User("Nanno_Core");
                                    if (oUserData.xiStatus == 0 && oUserData.oResult != null)
                                    {
                                        oUser = (XIInfraUsers)oUserData.oResult;
                                        var EncryptedPwd = xifEncrypt.EncryptData(model.Password, true, oUser.UserID.ToString());
                                        oUser.sPasswordHash = EncryptedPwd;
                                        oUserData = oUser.Update_User("Nanno_Core");
                                        if (oUserData.bOK && oUserData.oResult != null)
                                        {
                                            oUser = (XIInfraUsers)oUserData.oResult;
                                            if (oUser.UserID > 0)
                                            {
                                                XIInfraRoles xifRole = new XIInfraRoles();
                                                xifRole.sRoleName = "NannoUser";
                                                var RoleData = xifRole.Get_RoleDefinition("Nanno_Core");
                                                if (RoleData.bOK && RoleData.oResult != null)
                                                {
                                                    xifRole = (XIInfraRoles)RoleData.oResult;
                                                    XIInfraUserRoles xifURole = new XIInfraUserRoles();
                                                    xifURole.UserID = oUser.UserID;
                                                    xifURole.RoleID = xifRole.RoleID;
                                                    var URoleData = xifURole.Save_UserRole("Nanno_Core");
                                                    if (URoleData.xiStatus == 0 && URoleData.oResult != null)
                                                    {
                                                        oPrams = new List<CNV>();
                                                        oPrams.Add(new CNV { sName = "iUserID", sValue = oUser.UserID.ToString() });
                                                        oPrams.Add(new CNV { sName = "iNannoAppInstID", sValue = iNannoAppInstID.ToString() });
                                                        oCR = oConfig.Register_NannoUser(oPrams);
                                                        if (oCR.bOK && oCR.oResult != null)
                                                        {
                                                            return View("DisplayEmail");
                                                        }
                                                        else
                                                        {
                                                            return View("Register", model);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        return View("Register", model);
                                                    }
                                                }
                                                else
                                                {
                                                    return View("Register", model);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            return View("Register", model);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {

                }
            }
            // If we got this far, something failed, redisplay form
            return View("Register", model);
        }

        [AllowAnonymous]
        public ActionResult Registration(string TID, string returnUrl)
        {
            var sTID = HttpContext.Session["sCurrentTransactionID"];
            if (string.IsNullOrEmpty(TID) && sTID != null)
            {
                TID = sTID.ToString();
            }
            Session["sCurrentTransactionID"] = TID;
            XIIBO oBOI = new XIIBO();
            XIIXI oXI = new XIIXI();
            List<CNV> oWhrParams = new List<CNV>();
            oWhrParams.Add(new CNV() { sName = "xiguid", sValue = TID.ToString() });
            oBOI = oXI.BOI("wm transaction", null, null, oWhrParams);
            string sEmail = string.Empty;
            if (oBOI != null)
            {
                if (oBOI.Attributes.ContainsKey("sEmail"))
                {
                    sEmail = oBOI.Attributes["sEmail"].sValue;
                }
            }
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.sEmail = sEmail;
            return View("Register");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(RegisterWMUserModel model, string returnUrl)
        {
            try
            {
                var sTID = HttpContext.Session["sCurrentTransactionID"];
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Select(x => x.Value.Errors)
                          .Where(y => y.Count > 0)
                          .ToList();

                    foreach (var items in errors)
                    {
                        ModelState.AddModelError("", items[0].ErrorMessage);
                    }
                    ViewBag.ReturnUrl = returnUrl;
                    return View("Register", model);
                }
                var CoreDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreDataBase"];
                if (!string.IsNullOrEmpty(model.Email))
                {
                    XIInfraUsers oUser = new XIInfraUsers();
                    XIInfraCache oCache = new XIInfraCache();
                    XIInfraEncryption xifEncrypt = new XIInfraEncryption();
                    XIIXI oXI = new XIIXI();
                    oUser.sUserName = model.Email;
                    oUser = (XIInfraUsers)oUser.Get_UserDetails(CoreDatabase.ToString()).oResult;
                    if (oUser != null)
                    {
                        var EncryptedPwd = xifEncrypt.EncryptData(model.Password, true, oUser.UserID.ToString());
                        oUser.sPasswordHash = EncryptedPwd;
                        oUser.sPhoneNumber = model.MobileNumber;
                        var oUserData = oUser.Update_User(CoreDatabase);
                        if (oUserData.xiStatus == 0 && oUserData.oResult != null)
                        {
                            var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "wm user", null);
                            XIIBO oBOI = new XIIBO();
                            List<CNV> oWhrParams = new List<CNV>();
                            oWhrParams.Add(new CNV() { sName = "fkiuserid", sValue = oUser.UserID.ToString() });
                            oBOI = oXI.BOI("wm user", null, null, oWhrParams);
                            if (oBOI != null)
                            {
                                if (oBOI.Attributes.ContainsKey("bIsRegistered"))
                                {
                                    oBOI.Attributes["bIsRegistered"].sValue = "1";
                                    oBOI.Attributes["bIsRegistered"].bDirty = true;
                                }
                                else
                                {
                                    oBOI.Attributes.Add("bIsRegistered", new XIIAttribute() { sName = "bIsRegistered", sValue = "1", bDirty = true });
                                }
                                var oCR = oBOI.Save(oBOI);
                                if (oCR.bOK && oCR.oResult != null)
                                {

                                }
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid Email Id");
                        return View("Register", model);
                    }
                }
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                Common.SaveErrorLog("ERROR: [Login] unable to login " + ex.ToString(), "");
                ModelState.AddModelError("", "Something went wrong!!");
                return View(model);
            }
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult UserForgotPassword()
        {
            return View("ClientForgotPassword");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var Database = SessionManager.CoreDatabase;
            try
            {
                var ItemID = "";
                XIInfraUsers oUser = new XIInfraUsers();
                oUser.sUserName = model.Email;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(Database.ToString()).oResult;
                if (oUser == null)
                {
                    ItemID = "Invalid";
                    return Json(ItemID, JsonRequestBehavior.AllowGet);
                }
                var iOrgID = oUser.FKiOrganisationID;
                XIInfraCache oCache = new XIInfraCache();
                CResult oCResult = new CResult();
                var oContentDef = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, null, "38");
                if (oContentDef != null && oContentDef.Count() > 0)
                {
                    string sTemporaryPWD = string.Empty;
                    XIInfraEncryption xifEncrypt = new XIInfraEncryption();
                    if (string.IsNullOrEmpty(oUser.sTemporaryPasswordHash))
                    {
                        sTemporaryPWD = RandomNumber(8)/*random.Next(1, 100000000).ToString(new String('0', 7))*/;
                        var EncryptedPwd = xifEncrypt.EncryptData(sTemporaryPWD, true, oUser.UserID.ToString());
                        oUser.sTemporaryPasswordHash = EncryptedPwd;
                        oUser.UpdatedTime = DateTime.Now;
                        oUser.dtLastLogin = DateTime.Now;
                        var oUserData = oUser.Update_User(Database.ToString());
                    }
                    else
                    {
                        var DecryptedPwd = xifEncrypt.DecryptData(oUser.sTemporaryPasswordHash, true, oUser.UserID.ToString());
                        sTemporaryPWD = DecryptedPwd;
                    }
                    XIIBO oBOI = new XIIBO();
                    string sBOName = "XIAPPUsers";
                    if (sBOName != null)
                    {
                        oBOI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName, null);
                    }
                    XIBOInstance oBOIInstance = new XIBOInstance();
                    oBOIInstance.oStructureInstance = new Dictionary<string, List<XIIBO>>();
                    oBOI.SetAttribute("stemppassword", sTemporaryPWD);
                    oBOI.SetAttribute("sname", oUser.sFirstName + " " + oUser.sLastName);
                    List<XIIBO> oBOIList = new List<XIIBO>();
                    //oBOI.Attributes = oBOI.Attributes.ToDictionary(x => x.Value.sName.ToLower(), x => x.Value);
                    oBOI.XIIValues = oBOI.Attributes.ToDictionary(x => x.Value.sName.ToLower(), x => new XIIValue { sValue = x.Value.sValue });
                    oBOIList.Add(oBOI);
                    oBOIInstance.oStructureInstance["User"] = oBOIList;
                    XIContentEditors oDocumentContent = new XIContentEditors();
                    oDocumentContent = oContentDef.FirstOrDefault();
                    //Get Document Template htmlcontent with dynamic data
                    XIContentEditors oConent = new XIContentEditors();
                    oCResult.sMessage = "Template Merging Started";
                    XIDefinitionBase oXID = new XIDefinitionBase();
                    oXID.SaveErrortoDB(oCResult);
                    oConent.sSessionID = "";
                    //var oRes = oConent.MergeTemplateContent(oDocumentContent, oBOIInstance);
                    var oRes = oConent.MergeContentTemplate(oDocumentContent, oBOIInstance);
                    if (!oRes.bOK)
                    {
                        return null;
                    }
                    oCResult.sMessage = "Template Merged Sucessfully";
                    oXID.SaveErrortoDB(oCResult);
                    string sContent = (string)oRes.oResult;
                    XIInfraEmail oEmail = new XIInfraEmail();
                    oEmail.EmailID = oUser.sEmail;
                    oEmail.sSubject = oDocumentContent.Name;
                    oEmail.iServerID = oDocumentContent.FkiServerID;
                    oEmail.Sendmail(oUser.FKiOrganisationID, sContent, null, 0, XIConstant.Email_ForgotPassword, 0, null, 0, false, false, null, null, oEmail.iServerID);//send mail with attachment
                    oCResult.sMessage = "Mail send successfully";
                    oXID.SaveErrortoDB(oCResult);
                    oCResult.oTraceStack.Add(new CNV { sName = "Mail send successfully", sValue = "Mail send successfully" });
                }
                ItemID = "Valid";
                TempData["HoldMail"] = model.Email;
                return Json(new ForgotPasswordViewModel { Email = model.Email });
                //return Json(ItemID, JsonRequestBehavior.AllowGet);

                //return Json(new ResetPasswordViewModel {Email = model.Email});

                //ResetPasswordViewModel oReset = new ResetPasswordViewModel();
                //oReset.Email = oUser.sUserName;
                //return PartialView("ClientOTPVerify", oReset);
                //var result = xiinf.Sendmail(iOrgID, null, null).oResult;
                //mailer.SendPassword(model.Email, oUser.sFirstName).Send();
                //ItemID = "Valid";
                //return Json(ItemID, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return View(0);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ClientForgotPassword(ForgotPasswordViewModel model)
        {
            var Database = SessionManager.CoreDatabase;
            try
            {
                List<CNV> oParams = new List<CNV>();
                oParams.Add(new CNV { sName = "Description", sValue = "Forgot password requested" });
                oParams.Add(new CNV { sName = "Email", sValue = model.Email });
                XIMatrix oMatrix = new XIMatrix();
                oMatrix.MatrixAction(xiEnumSystem.EnumMatrixAction.ClientForgotPassword.ToString(), xiEnumSystem.EnumMatrixAction.ClientForgotPassword, null, 0, "0", "Forgot Password", oParams);
                if (string.IsNullOrEmpty(model.Email))
                {
                    ModelState.AddModelError("", "Please enter valid Email address");
                    return View(model);
                }
                XIInfraUsers oUser = new XIInfraUsers();
                oUser.sUserName = model.Email;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(Database.ToString()).oResult;
                if (oUser == null)
                {
                    ModelState.AddModelError("", "Your account is deactivated or doesn't exist. Please contact our customer service team for assistance.");
                    return View(model);
                }
                if (!oUser.bIsActive)
                {
                    ModelState.AddModelError("", "Your account is deactivated or doesn't exist. Please contact our customer service team for assistance.");
                    return View(model);
                }

                var iOrgID = oUser.FKiOrganisationID;
                XIInfraCache oCache = new XIInfraCache();
                CResult oCResult = new CResult();
                var oContentDef = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, null, "38");
                if (oContentDef != null && oContentDef.Count() > 0)
                {
                    string sTemporaryPWD = string.Empty;
                    XIInfraEncryption xifEncrypt = new XIInfraEncryption();
                    if (string.IsNullOrEmpty(oUser.sTemporaryPasswordHash))
                    {
                        sTemporaryPWD = RandomNumber(8)/*random.Next(1, 100000000).ToString(new String('0', 7))*/;
                        var EncryptedPwd = xifEncrypt.EncryptData(sTemporaryPWD, true, oUser.UserID.ToString());
                        oUser.sTemporaryPasswordHash = EncryptedPwd;
                        oUser.UpdatedTime = DateTime.Now;
                        oUser.dtLastLogin = DateTime.Now;
                        var oUserData = oUser.Update_User(Database.ToString());
                    }
                    else
                    {
                        var DecryptedPwd = xifEncrypt.DecryptData(oUser.sTemporaryPasswordHash, true, oUser.UserID.ToString());
                        sTemporaryPWD = DecryptedPwd;
                    }

                    XIIBO oBOI = new XIIBO();
                    string sBOName = "XIAPPUsers";
                    if (sBOName != null)
                    {
                        oBOI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName, null);
                    }
                    XIBOInstance oBOIInstance = new XIBOInstance();
                    oBOIInstance.oStructureInstance = new Dictionary<string, List<XIIBO>>();
                    oBOI.SetAttribute("stemppassword", sTemporaryPWD);
                    oBOI.SetAttribute("sname", oUser.sFirstName + " " + oUser.sLastName);
                    List<XIIBO> oBOIList = new List<XIIBO>();
                    //oBOI.Attributes = oBOI.Attributes.ToDictionary(x => x.Value.sName.ToLower(), x => x.Value);
                    oBOI.XIIValues = oBOI.Attributes.ToDictionary(x => x.Value.sName.ToLower(), x => new XIIValue { sValue = x.Value.sValue });
                    oBOIList.Add(oBOI);
                    oBOIInstance.oStructureInstance["User"] = oBOIList;
                    XIContentEditors oDocumentContent = new XIContentEditors();
                    oDocumentContent = oContentDef.FirstOrDefault();
                    //Get Document Template htmlcontent with dynamic data
                    XIContentEditors oConent = new XIContentEditors();
                    oCResult.sMessage = "Template Merging Started";
                    XIDefinitionBase oXID = new XIDefinitionBase();
                    oXID.SaveErrortoDB(oCResult);
                    oConent.sSessionID = "";
                    //var oRes = oConent.MergeTemplateContent(oDocumentContent, oBOIInstance);
                    var oRes = oConent.MergeContentTemplate(oDocumentContent, oBOIInstance);
                    if (!oRes.bOK)
                    {
                        return null;
                    }
                    oCResult.sMessage = "Template Merged Sucessfully";
                    oXID.SaveErrortoDB(oCResult);
                    string sContent = (string)oRes.oResult;
                    XIInfraEmail oEmail = new XIInfraEmail();
                    oEmail.EmailID = oUser.sEmail;
                    oEmail.sSubject = oDocumentContent.Name;
                    oEmail.Sendmail(oUser.FKiOrganisationID, sContent, null, 0, XIConstant.Email_ForgotPassword);//send mail with attachment
                    oCResult.sMessage = "Mail send successfully";
                    oParams = new List<CNV>();
                    oParams.Add(new CNV { sName = "Description", sValue = "Forgot password verification code sent" });
                    oParams.Add(new CNV { sName = "Email", sValue = model.Email });
                    oMatrix = new XIMatrix();
                    oMatrix.MatrixAction(xiEnumSystem.EnumMatrixAction.ClientForgotPassword.ToString(), xiEnumSystem.EnumMatrixAction.ClientForgotPassword, null, 0, "0", "Forgot Password", oParams);
                    oXID.SaveErrortoDB(oCResult);
                    oCResult.oTraceStack.Add(new CNV { sName = "Mail send successfully", sValue = "Mail send successfully" });
                }
                ResetPasswordViewModel oReset = new ResetPasswordViewModel();
                oReset.Email = oUser.sUserName;
                return PartialView("ClientOTPVerify", oReset);
                //var result = xiinf.Sendmail(iOrgID, null, null).oResult;
                //mailer.SendPassword(model.Email, oUser.sFirstName).Send();
                //ItemID = "Valid";
                //return Json(ItemID, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                XIMatrix oMatrix = new XIMatrix();
                List<CNV> oParams = new List<CNV>();
                oParams.Add(new CNV { sName = "Email", sValue = model.Email });
                oParams.Add(new CNV { sName = "Description", sValue = "Forgot password request failed" });
                oMatrix.MatrixAction(xiEnumSystem.EnumMatrixAction.ClientForgotPassword.ToString(), xiEnumSystem.EnumMatrixAction.ClientForgotPassword, null, 0, "0", "Forgot Password", oParams);
                Common.SaveErrorLog("ERROR: ClientForgotPassword " + ex.ToString(), "");
                return View(0);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }
        // GET: /Account/ResetPasswordLink
        [AllowAnonymous]
        public ActionResult ResetPasswordLink(string key)
        {
            try
            {
                XIInfraEncryption xiinfa = new XIInfraEncryption();
                //Decrypting the password
                string Decrypt = xiinfa.DecryptData(key.Replace(" ", "+"), true, "URP");
                var range = Decrypt.Split('_');
                string first = range[0];
                string second = range[1];
                //  string password = range[2];
                DateTime RequestedDate = Convert.ToDateTime(second);
                DateTime Currentdate = DateTime.Now;
                DateTime modifiedDatetime = RequestedDate.AddHours(24);
                ResetPasswordViewModel ItemsList = new ResetPasswordViewModel();
                ItemsList.Email = first;
                if (modifiedDatetime >= Currentdate)
                {
                    return key == null ? View("Error") : View(ItemsList);
                }
                else
                {
                    ItemsList.Error = "Your reset login password link was expired";
                    return View(ItemsList);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var Database = SessionManager.CoreDatabase;
            var ResetPass = string.Empty;
            if (!string.IsNullOrEmpty(SessionManager.AppName))
            {
                if (!string.IsNullOrEmpty(SessionManager.Logo))
                {
                    ViewBag.Logo = SessionManager.Logo;
                }
                else
                {
                    ViewBag.Logo = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                }
                ViewBag.AppName = SessionManager.AppName;
            }
            XIInfraUsers oUser = new XIInfraUsers();
            XIInfraEncryption oEncrypt = new XIInfraEncryption();
            oUser.sEmail = model.Email;
            oUser = (XIInfraUsers)oUser.Get_UserDetails(Database.ToString()).oResult;//Getting UserDetails based on sUserName
            oUser.UserID = oUser.UserID;
            var EncryptedPwd = oEncrypt.EncryptData(model.Password, true, oUser.UserID.ToString());//Encrypting the Password
            XIInfraUsers oUser1 = oUser;
            oUser1.UserID = oUser.UserID;
            oUser1.sPasswordHash = EncryptedPwd;
            oUser1.dtLastLogin = DateTime.Now;
            var result = oUser1.Update_User(Database).oResult;//Updating PasswordHash using UserID
            if (result != null)
            {
                ResetPass = "Valid";
                ViewBag.Email = model.Email;
                return Json(ResetPass, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ResetPass = "Invalid";
                return Json("ResetPasswordLink", ResetPass, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ClientResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("", "Please enter new password and confirm password correctly");
                    return View(model);
                }
                var Database = SessionManager.CoreDatabase;
                var ResetPass = string.Empty;
                if (!string.IsNullOrEmpty(SessionManager.AppName))
                {
                    if (!string.IsNullOrEmpty(SessionManager.Logo))
                    {
                        ViewBag.Logo = SessionManager.Logo;
                    }
                    else
                    {
                        ViewBag.Logo = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                    }
                    ViewBag.AppName = SessionManager.AppName;
                }
                XIInfraUsers oUser = new XIInfraUsers();
                XIInfraEncryption oEncrypt = new XIInfraEncryption();
                oUser.sUserName = model.Email;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(Database.ToString()).oResult;//Getting UserDetails based on sUserName
                oUser.UserID = oUser.UserID;
                var EncryptedPwd = oEncrypt.EncryptData(model.Password, true, oUser.UserID.ToString());//Encrypting the Password
                XIInfraUsers oUser1 = oUser;
                oUser1.UserID = oUser.UserID;
                oUser1.sPasswordHash = EncryptedPwd;
                oUser1.dtLastLogin = DateTime.Now;
                oUser1.sTemporaryPasswordHash = null;
                var result = oUser1.Update_User(Database).oResult;//Updating PasswordHash using UserID
                if (oUser.Role.iThemeID > 0)
                {
                    var sThemeName = oUser.Role.GetRoleTheme(System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinDatabase"]);
                    if (!string.IsNullOrEmpty(sThemeName))
                    {
                        SessionManager.Theme = sThemeName;
                    }
                }
                oUser.dtLastLogin = DateTime.Now;
                oUser.Update_User(oUser.sCoreDatabaseName);
                Singleton.Instance.sCoreDatabase = oUser.sCoreDatabaseName;
                Singleton.Instance.sUserID = oUser.UserID.ToString();
                Singleton.Instance.sOrgID = oUser.FKiOrganisationID.ToString();
                Singleton.Instance.sAppName = oUser.sAppName;
                SessionManager.sUserName = oUser.sUserName;
                SessionManager.sEmail = oUser.sEmail;
                SessionManager.sRoleName = oUser.Role.sRoleName;
                SessionManager.OrgDatabase = oUser.sDatabaseName;
                SessionManager.UserID = oUser.UserID;
                SessionManager.OrganisationName = "Org";
                SessionManager.UserUniqueID = null;
                SessionManager.OrganizationID = oUser.FKiOrganisationID;
                SessionManager.sName = oUser.sFirstName + " " + oUser.sLastName;
                SessionManager.iRoleID = oUser.RoleID.RoleID;
                SessionManager.sCustomerRefNo = oUser.sCustomerRefNo;
                SessionManager.ApplicationID = oUser.FKiApplicationID;

                FormsAuthentication.SetAuthCookie(oUser.sUserName, false);
                var authTicket = new FormsAuthenticationTicket(1, oUser.sUserName, DateTime.Now, DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes), false, "Admin", FormsAuthentication.FormsCookiePath);
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
                {
                    HttpOnly = true,
                    Secure = FormsAuthentication.RequireSSL,
                    Path = FormsAuthentication.FormsCookiePath,
                    Domain = FormsAuthentication.CookieDomain,
                    Expires = DateTime.Now.AddDays(1)
                };
                HttpContext.Response.Cookies.Add(authCookie);

                // HERE SETTING ROLENAME AND USERNAME INTO CACHE.
                XIInfraCache oUserCache = new XIInfraCache();
                string sSessionID = HttpContext.Session.SessionID;
                var oCacheduser = new VM_UserLoginCache { sUserName = oUser.sUserName, sRole = oUser.Role.sRoleName };
                oUserCache.InsertIntoCache(oCacheduser, "SSS_" + sSessionID);
                //Session.Abandon();
                XIMatrix oMatrix = new XIMatrix();
                List<CNV> oParams = new List<CNV>();
                oParams.Add(new CNV { sName = "Email", sValue = model.Email });
                oParams.Add(new CNV { sName = "Description", sValue = "Client Reset Password Successful" });
                oMatrix.MatrixAction(xiEnumSystem.EnumMatrixAction.ClientResetPassword.ToString(), xiEnumSystem.EnumMatrixAction.ClientResetPassword, null, 0, "0", "Client Reset Password", oParams);
                return RedirectToAction("LandingPages", "Home");
            }
            return View(model);
        }

        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            string sSessionID = HttpContext.Session.SessionID;
            string sAppName = string.Empty;
            if (SessionManager.AppName != null)
            {
                sAppName = SessionManager.AppName;
            }
            SessionManager.CoreDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinDatabase"];
            string HostingPath = System.Configuration.ConfigurationManager.AppSettings["HostingPath"];
            SessionManager.UserID = 0;
            Session.Clear();
            Session.Abandon();
            //FormsAuthentication.SignOut();
            /* REMOVE CACHE FOR LOGOUT SESSION */
            var oUserCache = HttpRuntime.Cache["XICache"];
            var oCacheobj = (XICacheInstance)oUserCache;
            if (oCacheobj != null)
                oCacheobj.RemoveUserCache(sSessionID);
            /*REMOVE USER CACHE USERNAME AND ROLENAME*/
            XIInfraCache.RemoveCacheWithKey("SSS_" + sSessionID);

            if (!string.IsNullOrEmpty(sAppName))
            {
                sAppName = sAppName.Replace(" ", "");
                if (!string.IsNullOrEmpty(HostingPath))
                {
                    return Redirect(HostingPath + "/" + sAppName);
                }
                else
                {
                    return Redirect("/" + sAppName);
                }

            }
            else
            {
                var ApplicationPath = System.Configuration.ConfigurationManager.AppSettings["ApplicationPath"];
                var ApplicationName = System.Configuration.ConfigurationManager.AppSettings["ApplicationName"];
                return Redirect(ApplicationPath + ApplicationName);
                //return RedirectToAction("Login", "Account");
            }
        }

        [AllowAnonymous]
        public ActionResult ClientLogOff()
        {
            string sSessionID = HttpContext.Session.SessionID;
            string sAppName = string.Empty;
            if (SessionManager.AppName != null)
            {
                sAppName = SessionManager.AppName;
            }
            string sCoreDatabase = SessionManager.CoreDatabase;
            //SessionManager.CoreDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinDatabase"];
            string HostingPath = System.Configuration.ConfigurationManager.AppSettings["HostingPath"];
            SessionManager.UserID = 0;
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            /* REMOVE CACHE FOR LOGOUT SESSION */
            var oUserCache = HttpRuntime.Cache["XICache"];
            var oCacheobj = (XICacheInstance)oUserCache;
            if (oCacheobj != null)
                oCacheobj.RemoveUserCache(sSessionID);

            /*REMOVE USER CACHE USERNAME AND ROLENAME*/
            XIInfraCache.RemoveCacheWithKey("SSS_" + sSessionID);

            SessionManager.CoreDatabase = sCoreDatabase;
            return RedirectToAction("ClientLogin", "Account", new { returnUrl = "", SAPN = sAppName });
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";
        private object WebSecurity;

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
        #region URL Based Login
        public XIDApplication GetApplicationInfo(string UrlName)
        {
            XIInfraCache oCache = new XIInfraCache();
            XIDApplication result = new XIDApplication();
            string StrUrlName = GetUrlName(UrlName);
            //if (StrUrlName.ToLower().Trim() == EnumDefaultUrls.CorporateUser.ToString().ToLower().Trim())
            //{
            //    result = new VMOrganizations { OrganizationID = -1 };
            //}
            //else
            //{
            var oAPPData = oCache.GetObjectFromCache(XIConstant.CacheApplication, StrUrlName, null);
            //var oAPPData = oDXI.Get_ApplicationDefinition(StrUrlName);
            if (oAPPData != null)
            {
                result = (XIDApplication)oAPPData;
            }
            //result = dbContext.XIApplications.Where(Q => Q.sApplicationName.ToLower().Trim() == StrUrlName.ToLower().Trim()).FirstOrDefault();
            //}
            result = result ?? new XIDApplication { ID = -1 };
            return result;
        }

        public string GetUrlName(string InputUrl)
        {
            try
            {
                if (!string.IsNullOrEmpty(InputUrl))
                {
                    //remove Last special characters
                    string result = string.Empty;
                    char[] chars = { '/', '\\', '@', ':', '*', '&' };
                    result = InputUrl.TrimEnd(chars);
                    int Index_ = result.LastIndexOf('/');
                    string strfinal = result.Substring(Index_, result.Length - Index_);
                    return RemoveSpecialCharacters(strfinal);
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9]+", "", RegexOptions.Compiled);
        }

        #endregion URL Based Login

        public ActionResult RefreshToken()
        {
            return PartialView("_AntiForgeryToken");
        }

        [AllowAnonymous]
        public ActionResult SessionHandler()
        {
            return PartialView("~/Views/Shared/_SessionExpire.cshtml");
        }

        private static Random random = new Random();
        public static string RandomNumber(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ClientOTPVerify(ResetPasswordViewModel model)
        {
            if (string.IsNullOrEmpty(model.Code))
            {
                ModelState.AddModelError("", "Please enter verification code");
                return View(model);
            }
            if (string.IsNullOrEmpty(SessionManager.CoreDatabase))
            {
                SessionManager.CoreDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreDataBase"];
            }
            var Database = SessionManager.CoreDatabase;
            XIInfraUsers oUser = new XIInfraUsers();
            XIInfraEncryption oEncrypt = new XIInfraEncryption();
            oUser.sUserName = model.Email;
            oUser = (XIInfraUsers)oUser.Get_UserDetails(Database.ToString()).oResult;//Getting UserDetails based on sUserName
            oUser.UserID = oUser.UserID;
            var EncryptedPwd = oEncrypt.EncryptData(model.Code, true, oUser.UserID.ToString());//Encrypting the Password
                                                                                               //XIInfraUsers oUser1 = oUser;
                                                                                               //oUser1.UserID = oUser.UserID;
                                                                                               //oUser1.sPasswordHash = EncryptedPwd;
            List<CNV> oParams = new List<CNV>();
            oParams.Add(new CNV { sName = "Email", sValue = model.Email });
            XIMatrix oMatrix = new XIMatrix();
            if (oUser.sTemporaryPasswordHash == EncryptedPwd)
            {
                oParams.Add(new CNV { sName = "Description", sValue = "Entered valid verification code" });
                oMatrix.MatrixAction(xiEnumSystem.EnumMatrixAction.ClientCodeVerify.ToString(), xiEnumSystem.EnumMatrixAction.ClientCodeVerify, null, 0, "0", "Client Code Verify", oParams);
                return PartialView("ClientResetPassword", model);
            }
            else
            {
                oParams.Add(new CNV { sName = "Description", sValue = "Entered invalid verification code: " + model.Code });
                oMatrix.MatrixAction(xiEnumSystem.EnumMatrixAction.ClientCodeVerify.ToString(), xiEnumSystem.EnumMatrixAction.ClientCodeVerify, null, 0, "0", "Client Code Verify", oParams);
                ModelState.AddModelError("", "Please enter correct verification code");
                return View(model);
            }
            return null;
        }

        [HttpPost]
        public ActionResult CheckSecureKey(string sSecureKey)
        {
            int iXILinkID = 0;
            Guid XILinkGUID = Guid.Empty;
            if (int.TryParse(sSecureKey, out iXILinkID))
            {
                return Json(iXILinkID, JsonRequestBehavior.AllowGet);
            }
            else if (Guid.TryParse(sSecureKey, out XILinkGUID))
            {
                return Json(XILinkGUID, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var XIGUID = SessionManager.XIGUID;
                cEncryption oEncrypt = new cEncryption();
                var DecryptKey = oEncrypt.DecryptData(sSecureKey, true, XIGUID);
                var keys = DecryptKey.Split('_').ToList();
                if (keys.Count() > 1 && keys[0] == XIGUID)
                {
                    return Json(keys[1], JsonRequestBehavior.AllowGet);
                }
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        #region AdminOTPandForgotPassword

        [AllowAnonymous]
        public ActionResult AdminOTPVerify()
        {
            ResetPasswordViewModel model = new ResetPasswordViewModel();
            model.Value = 1;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AdminOTPVerify(ResetPasswordViewModel model)
        {
            if (string.IsNullOrEmpty(model.Code))
            {
                ModelState.AddModelError("Code", "Please enter verification code");
                return View(model);
            }
            string ResetPass = string.Empty;
            if (string.IsNullOrEmpty(SessionManager.CoreDatabase))
            {
                SessionManager.CoreDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreDataBase"];
            }
            var Database = SessionManager.CoreDatabase;
            XIInfraUsers oUser = new XIInfraUsers();
            XIInfraEncryption oEncrypt = new XIInfraEncryption();
            oUser.sUserName = model.Email;
            oUser = (XIInfraUsers)oUser.Get_UserDetails(Database.ToString()).oResult;//Getting UserDetails based on sUserName
            oUser.UserID = oUser.UserID;
            var EncryptedPwd = oEncrypt.EncryptData(model.Code, true, oUser.UserID.ToString());
            if (oUser.sTemporaryPasswordHash == EncryptedPwd)
            {
                ResetPass = "Valid";
                return Json(new ResetPasswordViewModel { Email = model.Email });
                //return Json(ResetPass, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ResetPasswordViewModel { Value = 3, Error = "Please enter correct verification code" });
            }
        }

        [AllowAnonymous]
        public ActionResult AdminForgotPassword()
        {
            ResetPasswordViewModel Model = new ResetPasswordViewModel();
            Model.Email = TempData.Values.FirstOrDefault().ToString();
            return View(Model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AdminResetPassword(ResetPasswordViewModel model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Please enter new password and confirm password correctly");
                return View("AdminForgotPassword", model);
            }
            var Database = SessionManager.CoreDatabase;
            var ResetPass = string.Empty;
            if (!string.IsNullOrEmpty(SessionManager.AppName))
            {
                if (!string.IsNullOrEmpty(SessionManager.Logo))
                {
                    ViewBag.Logo = SessionManager.Logo;
                }
                else
                {
                    ViewBag.Logo = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinLogo"];
                }
                ViewBag.AppName = SessionManager.AppName;
            }
            XIInfraUsers oUser = new XIInfraUsers();
            XIInfraEncryption oEncrypt = new XIInfraEncryption();
            oUser.sUserName = model.Email;
            oUser = (XIInfraUsers)oUser.Get_UserDetails(Database.ToString()).oResult;//Getting UserDetails based on sUserName
            oUser.UserID = oUser.UserID;
            var EncryptedPwd = oEncrypt.EncryptData(model.Password, true, oUser.UserID.ToString());//Encrypting the Password
            XIInfraUsers oUser1 = oUser;
            oUser1.UserID = oUser.UserID;
            oUser1.sPasswordHash = EncryptedPwd;
            oUser1.dtLastLogin = DateTime.Now;
            oUser1.sTemporaryPasswordHash = null;
            var result = oUser1.Update_User(Database).oResult;//Updating PasswordHash using UserID
            if (oUser.Role.iThemeID > 0)
            {
                var sThemeName = oUser.Role.GetRoleTheme(System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinDatabase"]);
                if (!string.IsNullOrEmpty(sThemeName))
                {
                    SessionManager.Theme = sThemeName;
                }
            }
            oUser.dtLastLogin = DateTime.Now;
            oUser.Update_User(oUser.sCoreDatabaseName);
            Singleton.Instance.sCoreDatabase = oUser.sCoreDatabaseName;
            Singleton.Instance.sUserID = oUser.UserID.ToString();
            Singleton.Instance.sOrgID = oUser.FKiOrganisationID.ToString();
            Singleton.Instance.sAppName = oUser.sAppName;
            SessionManager.sUserName = oUser.sUserName;
            SessionManager.sEmail = oUser.sEmail;
            SessionManager.sRoleName = oUser.Role.sRoleName;
            SessionManager.OrgDatabase = oUser.sDatabaseName;
            SessionManager.UserID = oUser.UserID;
            SessionManager.OrganisationName = "Org";
            SessionManager.UserUniqueID = null;
            SessionManager.OrganizationID = oUser.FKiOrganisationID;
            SessionManager.sName = oUser.sFirstName + " " + oUser.sLastName;
            SessionManager.iRoleID = oUser.RoleID.RoleID;
            SessionManager.sCustomerRefNo = oUser.sCustomerRefNo;
            SessionManager.ApplicationID = oUser.FKiApplicationID;

            //FormsAuthentication.SetAuthCookie(oUser.sUserName, false);
            //var authTicket = new FormsAuthenticationTicket(1, oUser.sUserName, DateTime.Now, DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes), false, "Admin", FormsAuthentication.FormsCookiePath);
            //string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            //var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            //{
            //    HttpOnly = true,
            //    Secure = FormsAuthentication.RequireSSL,
            //    Path = FormsAuthentication.FormsCookiePath,
            //    Domain = FormsAuthentication.CookieDomain,
            //    Expires = DateTime.Now.AddDays(1)
            //};
            //HttpContext.Response.Cookies.Add(authCookie);

            // HERE SETTING ROLENAME AND USERNAME INTO CACHE.
            XIInfraCache oUserCache = new XIInfraCache();
            string sSessionID = HttpContext.Session.SessionID;
            var oCacheduser = new VM_UserLoginCache { sUserName = oUser.sUserName, sRole = oUser.Role.sRoleName };
            oUserCache.InsertIntoCache(oCacheduser, $"SSS_{sSessionID}");
            //Session.Abandon();
            return RedirectToAction("LandingPages", "Home");
        }

        #endregion AdminOTPandForgotPassword

        #region UserResetPassword

        [AllowAnonymous]
        [HttpPost]
        public ActionResult UserResetPassword(string sEmail = "")
        {
            var ResetLink = System.Configuration.ConfigurationManager.AppSettings["ApplicationPath"];
            var Database = SessionManager.CoreDatabase;
            try
            {
                var ItemID = "";
                XIInfraUsers oUser = new XIInfraUsers();
                oUser.sUserName = sEmail;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(Database.ToString()).oResult;
                if (oUser == null)
                {
                    ItemID = "Invalid";
                    return Json(ItemID, JsonRequestBehavior.AllowGet);
                }
                if (oUser.bIsDisableClient == "Deactive")
                {
                    var bIsDisable = "Deactive";
                    return Json(bIsDisable, JsonRequestBehavior.AllowGet);
                }
                var iOrgID = oUser.FKiOrganisationID;
                XIInfraCache oCache = new XIInfraCache();
                CResult oCResult = new CResult();
                var oContentDef = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, null, "287");
                if (oContentDef != null && oContentDef.Count() > 0)
                {
                    XIInfraEncryption xifEncrypt = new XIInfraEncryption();
                    string sTemporaryPWD = RandomNumber(8)/*random.Next(1, 100000000).ToString(new String('0', 7))*/;
                    var EncryptedPwd = xifEncrypt.EncryptData(sTemporaryPWD, true, oUser.UserID.ToString());
                    oUser.sTemporaryPasswordHash = EncryptedPwd;
                    oUser.UpdatedTime = DateTime.Now;
                    oUser.dtLastLogin = DateTime.Now;
                    var oUserData = oUser.Update_User(Database.ToString());
                    string EncryptedHash = xifEncrypt.EncryptData(sEmail + "_" + DateTime.Now.ToString("yyyy-MM-dd HH:mm tt"), true, "URP");
                    //string Messagebody = "http://localhost:53996/Account/UserResetPasswordLink?key=" + EncryptedHash;
                    string Messagebody = ResetLink + "Account/UserResetPasswordLink?key=" + EncryptedHash;
                    XIIBO oBOI = new XIIBO();
                    string sBOName = "XIAPPUsers";
                    if (sBOName != null)
                    {
                        oBOI.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName, null);
                    }
                    XIBOInstance oBOIInstance = new XIBOInstance();
                    oBOIInstance.oStructureInstance = new Dictionary<string, List<XIIBO>>();
                    oBOI.SetAttribute("stemppassword", sTemporaryPWD);
                    oBOI.SetAttribute("sname", oUser.sFirstName + " " + oUser.sLastName);
                    oBOI.SetAttribute("smessagebody", Messagebody);
                    List<XIIBO> oBOIList = new List<XIIBO>();
                    oBOI.XIIValues = oBOI.Attributes.ToDictionary(x => x.Value.sName.ToLower(), x => new XIIValue { sValue = x.Value.sValue });
                    oBOIList.Add(oBOI);
                    oBOIInstance.oStructureInstance["User"] = oBOIList;
                    XIContentEditors oDocumentContent = new XIContentEditors();
                    oDocumentContent = oContentDef.FirstOrDefault();
                    //Get Document Template htmlcontent with dynamic data
                    XIContentEditors oConent = new XIContentEditors();
                    oCResult.sMessage = "Template Merging Started";
                    XIDefinitionBase oXID = new XIDefinitionBase();
                    oXID.SaveErrortoDB(oCResult);
                    oConent.sSessionID = "";
                    var oRes = oConent.MergeContentTemplate(oDocumentContent, oBOIInstance);
                    if (!oRes.bOK)
                    {
                        return null;
                    }
                    oCResult.sMessage = "Template Merged Sucessfully";
                    oXID.SaveErrortoDB(oCResult);
                    string sContent = (string)oRes.oResult;
                    XIInfraEmail oEmail = new XIInfraEmail();
                    oEmail.EmailID = oUser.sEmail;
                    oEmail.sSubject = oDocumentContent.Name;
                    oEmail.Sendmail(oUser.FKiOrganisationID, sContent, null, 0, XIConstant.Email_ResetPassword);//send mail with attachment
                    oCResult.sMessage = "Mail send successfully";
                    oXID.SaveErrortoDB(oCResult);
                    oCResult.oTraceStack.Add(new CNV { sName = "Mail send successfully", sValue = "Mail send successfully" });
                }
                ItemID = "Valid";
                TempData["HoldMail"] = sEmail;
                return Json(new ForgotPasswordViewModel { Email = sEmail });
            }
            catch (Exception ex)
            {
                return View(0);
            }
        }

        [AllowAnonymous]
        public ActionResult UserResetPasswordLink(string key)
        {
            try
            {
                XIInfraEncryption xiinfa = new XIInfraEncryption();
                //Decrypting the password
                string Decrypt = xiinfa.DecryptData(key.Replace(" ", "+"), true, "URP");
                var range = Decrypt.Split('_');
                string first = range[0];
                string second = range[1];
                //  string password = range[2];
                DateTime RequestedDate = Convert.ToDateTime(second);
                DateTime Currentdate = DateTime.Now;
                DateTime modifiedDatetime = RequestedDate.AddHours(24);
                ResetPasswordViewModel model = new ResetPasswordViewModel();
                model.Email = first;
                TempData["HoldMail"] = first;
                if (modifiedDatetime >= Currentdate)
                {
                    return key == null ? View("Error") : View("ClientOTPVerify", model);
                }
                else
                {
                    model.Value = 2;
                    model.Error = "Your reset login password link was expired";
                    return key == null ? View("Error") : View("ClientOTPVerify", model);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion UserResetPassword


        public ActionResult ViewProfile()
        {
            IndexViewModel prof = new IndexViewModel();
            prof.HasPassword = true;
            prof.PhoneNumber = "9848022338";
            return PartialView("_ViewProfile", prof);
        }
       

    }
}