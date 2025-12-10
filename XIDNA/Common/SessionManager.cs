using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XISystem;

namespace XIDNA
{
    /// <summary>
    /// TODO: Contains Session information
    /// </summary>
    public class SessionManager
    {
        /// <summary>
        /// Gets or sets Current user ID stored in session
        /// </summary>
        /// 
        public static string sUserName
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.sUserName] != null)
                {
                    return Convert.ToString(HttpContext.Current.Session[SessionManagerConstants.sUserName]);
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.sUserName] = value;
                }
            }
        }

        public static string sRoleName
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.sRoleName] != null)
                {
                    return Convert.ToString(HttpContext.Current.Session[SessionManagerConstants.sRoleName]);
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.sRoleName] = value;
                }
            }
        }

        public static string sEmail
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.sEmail] != null)
                {
                    return Convert.ToString(HttpContext.Current.Session[SessionManagerConstants.sEmail]);
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.sEmail] = value;
                }
            }
        }

        public static int OrganizationID
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.OrganizationID] != null)
                {
                    return Convert.ToInt32(HttpContext.Current.Session[SessionManagerConstants.OrganizationID]);
                }
                else
                {
                    return 0;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.OrganizationID] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets Current user First Name stored in session
        /// </summary>
        public static int UserID
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.UserID] != null)
                {
                    return Convert.ToInt32(HttpContext.Current.Session[SessionManagerConstants.UserID]);
                }
                else
                {
                    return 0;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.UserID] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets Current user First Name stored in session
        /// </summary>
        public static string Theme
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.Theme] != null)
                {
                    return Convert.ToString(HttpContext.Current.Session[SessionManagerConstants.Theme]);
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.Theme] = value;
                }
            }
        }



        /// <summary>
        /// Gets or sets Current user First Name stored in session
        /// </summary>
        public static string AppName
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.AppName] != null)
                {
                    return Convert.ToString(HttpContext.Current.Session[SessionManagerConstants.AppName]);
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.AppName] = value;
                }
            }
        }

        public static int ApplicationID
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.ApplicationID] != null)
                {
                    return Convert.ToInt32(HttpContext.Current.Session[SessionManagerConstants.ApplicationID]);
                }
                else
                {
                    return 0;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.ApplicationID] = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets Current user Last Name stored in session
        /// </summary>
        public static string Logo
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.Logo] != null)
                {
                    return Convert.ToString(HttpContext.Current.Session[SessionManagerConstants.Logo]);
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.Logo] = value;
                }
            }
        }


        /// <summary>
        /// Gets or sets Current User Type stored in session
        /// </summary>
        public static string CoreDatabase
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.CoreDatabase] != null)
                {
                    return HttpContext.Current.Session[SessionManagerConstants.CoreDatabase].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.CoreDatabase] = value;
                }
            }
        }


        /// <summary>
        /// Gets or sets Current User Type stored in session
        /// </summary>
        public static string OrgDatabase
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.OrgDatabase] != null)
                {
                    return HttpContext.Current.Session[SessionManagerConstants.OrgDatabase].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.OrgDatabase] = value;
                }
            }
        }


        /// <summary>
        /// Gets or sets Current User Type stored in Token
        /// </summary>
        public static string OrganisationName
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.OrganisationName] != null)
                {
                    return HttpContext.Current.Session[SessionManagerConstants.OrganisationName].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.OrganisationName] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets Current SubScriptionID stored in session
        /// </summary>
        public static string UserUniqueID
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.UserUniqueID] != null)
                {
                    return Convert.ToString(HttpContext.Current.Session[SessionManagerConstants.UserUniqueID]);
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.UserUniqueID] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets Current SubScriptionID stored in session
        /// </summary>
        public static string sGUID
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.sGUID] != null)
                {
                    return Convert.ToString(HttpContext.Current.Session[SessionManagerConstants.sGUID]);
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.sGUID] = value;
                }
            }
        }


        /// <summary>
        /// Gets or sets Current SubScriptionID stored in session
        /// </summary>
        public static string LayoutName
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.LayoutName] != null)
                {
                    return Convert.ToString(HttpContext.Current.Session[SessionManagerConstants.LayoutName]);
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.LayoutName] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets Current user ID stored in session
        /// </summary>
        public static int CustomerID
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.CustomerID] != null)
                {
                    return Convert.ToInt32(HttpContext.Current.Session[SessionManagerConstants.CustomerID]);
                }
                else
                {
                    return 0;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.CustomerID] = value;
                }
            }
        }
        public static string sName
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.Name] != null)
                {
                    return Convert.ToString(HttpContext.Current.Session[SessionManagerConstants.Name]);
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.Name] = value;
                }
            }
        }

        public static int iRoleID
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.iRoleID] != null)
                {
                    return Convert.ToInt32((HttpContext.Current.Session[SessionManagerConstants.iRoleID]));
                }
                else
                {
                    return 0;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.iRoleID] = value;
                }
            }
        }
        public static string sCustomerRefNo
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.sCustomerRefNo] != null)
                {
                    return Convert.ToString(HttpContext.Current.Session[SessionManagerConstants.sCustomerRefNo]);
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.sCustomerRefNo] = value;
                }
            }
        }

        public static string BColor
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.BColor] != null)
                {
                    return Convert.ToString(HttpContext.Current.Session[SessionManagerConstants.BColor]);
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.BColor] = value;
                }
            }
        }
        public static string FColor
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.FColor] != null)
                {
                    return Convert.ToString(HttpContext.Current.Session[SessionManagerConstants.FColor]);
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.FColor] = value;
                }
            }
        }
        public static string FSize
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.FSize] != null)
                {
                    return Convert.ToString(HttpContext.Current.Session[SessionManagerConstants.FSize]);
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.FSize] = value;
                }
            }
        }
        public static List<CNV> SessionItems()
        {
            List<CNV> nCNV = new List<CNV>();
            if (HttpContext.Current.Session != null)
            {
                nCNV.Add(new CNV { sName = "sSessionID", sValue = HttpContext.Current.Session.SessionID.ToString() });
            }
            if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.CoreDatabase] != null)
            {
                nCNV.Add(new CNV { sName = "sDatabase", sValue = HttpContext.Current.Session[SessionManagerConstants.CoreDatabase].ToString() });
            }
            if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.OrgDatabase] != null)
            {
                nCNV.Add(new CNV { sName = "sOrgDatabase", sValue = HttpContext.Current.Session[SessionManagerConstants.OrgDatabase].ToString() });
            }
            if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.UserID] != null)
            {
                nCNV.Add(new CNV { sName = "iUserID", sValue = HttpContext.Current.Session[SessionManagerConstants.UserID].ToString() });
            }
            if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.OrganisationName] != null)
            {
                nCNV.Add(new CNV { sName = "sOrgName", sValue = HttpContext.Current.Session[SessionManagerConstants.OrganisationName].ToString() });
            }
            if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.sUserName] != null)
            {
                nCNV.Add(new CNV { sName = "sUserName", sValue = HttpContext.Current.Session[SessionManagerConstants.sUserName].ToString() });
            }
            if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.sRoleName] != null)
            {
                nCNV.Add(new CNV { sName = "sRoleName", sValue = HttpContext.Current.Session[SessionManagerConstants.sRoleName].ToString() });
            }
            if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.sEmail] != null)
            {
                nCNV.Add(new CNV { sName = "sEmail", sValue = HttpContext.Current.Session[SessionManagerConstants.sEmail].ToString() });
            }
            if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.UserUniqueID] != null)
            {
                nCNV.Add(new CNV { sName = "sCurrentUserGUID", sValue = HttpContext.Current.Session[SessionManagerConstants.UserUniqueID].ToString() });
            }
            if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.OrganizationID] != null)
            {
                nCNV.Add(new CNV { sName = "iOrganizationID", sValue = HttpContext.Current.Session[SessionManagerConstants.OrganizationID].ToString() });
            }
            if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.Name] != null)
            {
                nCNV.Add(new CNV { sName = "sName", sValue = HttpContext.Current.Session[SessionManagerConstants.Name].ToString() });
            }
            if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.iRoleID] != null)
            {
                nCNV.Add(new CNV { sName = "iRoleID", sValue = HttpContext.Current.Session[SessionManagerConstants.iRoleID].ToString() });
            }
            if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.sReference] != null)
            {
                nCNV.Add(new CNV { sName = "sReference", sValue = HttpContext.Current.Session[SessionManagerConstants.sReference].ToString() });
            }
            return nCNV;
        }

        /// <summary>
        /// Gets or sets Current User Type stored in session
        /// </summary>
        public static string QSSourceID
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.QSSourceID] != null)
                {
                    return HttpContext.Current.Session[SessionManagerConstants.QSSourceID].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.QSSourceID] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets Current User Type stored in session
        /// </summary>
        public static string QSName
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.QSName] != null)
                {
                    return HttpContext.Current.Session[SessionManagerConstants.QSName].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.QSName] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets Current User Type stored in session
        /// </summary>
        public static string sExternalRefID
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.sExternalRefID] != null)
                {
                    return HttpContext.Current.Session[SessionManagerConstants.sExternalRefID].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.sExternalRefID] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets Current User Type stored in session
        /// </summary>
        public static string XIGUID
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.XIGUID] != null)
                {
                    return HttpContext.Current.Session[SessionManagerConstants.XIGUID].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.XIGUID] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets Current User Type stored in session
        /// </summary>
        public static string UserCookie
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.UserCookie] != null)
                {
                    return HttpContext.Current.Session[SessionManagerConstants.UserCookie].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.UserCookie] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets Current User Type stored in session
        /// </summary>
        public static string ConfigDatabase
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.ConfigDatabase] != null)
                {
                    return HttpContext.Current.Session[SessionManagerConstants.ConfigDatabase].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.ConfigDatabase] = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets Current User Hierarchy in session
        /// </summary>
        public static string sHierarchy
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.Hierarchy] != null)
                {
                    return HttpContext.Current.Session[SessionManagerConstants.Hierarchy].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.Hierarchy] = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets Current User InsertDefault in session
        /// </summary>
        public static string sInsertDefaultCode
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.InsertHierachy] != null)
                {
                    return HttpContext.Current.Session[SessionManagerConstants.InsertHierachy].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.InsertHierachy] = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets Current User UpdateHierarchy in session
        /// </summary>
        public static string sUpdateHierarchy
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.UpdateHierarchy] != null)
                {
                    return HttpContext.Current.Session[SessionManagerConstants.UpdateHierarchy].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.UpdateHierarchy] = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets Current User ViewHierarchy in session
        /// </summary>
        public static string sViewHierarchy
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.ViewHierarchy] != null)
                {
                    return HttpContext.Current.Session[SessionManagerConstants.ViewHierarchy].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.ViewHierarchy] = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets Current User DeleteHierarchy in session
        /// </summary>
        public static string sDeleteHierarchy
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.DeleteHierarchy] != null)
                {
                    return HttpContext.Current.Session[SessionManagerConstants.DeleteHierarchy].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.DeleteHierarchy] = value;
                }
            }
        }

        public static string sSignalRCID
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.sSignalRCID] != null)
                {
                    return HttpContext.Current.Session[SessionManagerConstants.sSignalRCID].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.sSignalRCID] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets Current User Type stored in session
        /// </summary>
        public static string MenuType
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.MenuType] != null)
                {
                    return HttpContext.Current.Session[SessionManagerConstants.MenuType].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.MenuType] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets Current User Type stored in session
        /// </summary>
        public static int iUserOrg
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.iUserOrg] != null)
                {
                    return Convert.ToInt32(HttpContext.Current.Session[SessionManagerConstants.iUserOrg]);
                }
                else
                {
                    return 0;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.iUserOrg] = value;
                }
            }
        }

        public static bool bOrgSwitch
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.bOrgSwitch] != null)
                {
                    return Convert.ToBoolean(HttpContext.Current.Session[SessionManagerConstants.bOrgSwitch]);
                }
                else
                {
                    return false;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.bOrgSwitch] = value;
                }
            }
        }
        public static string sHomePage
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.sHomePage] != null)
                {
                    return HttpContext.Current.Session[SessionManagerConstants.sHomePage].ToString();
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.sHomePage] = value;
                }
            }
        }
        public static bool bNannoApp
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.bNannoApp] != null)
                {
                    return Convert.ToBoolean(HttpContext.Current.Session[SessionManagerConstants.bNannoApp]);
                }
                else
                {
                    return false;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.bNannoApp] = value;
                }
            }
        }
        public static int iUserLevel
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.iUserLevel] != null)
                {
                    return Convert.ToInt32(HttpContext.Current.Session[SessionManagerConstants.iUserLevel]);
                }
                else
                {
                    return 0;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.iUserLevel] = value;
                }
            }
        }

        public static string sReference
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.sReference] != null)
                {
                    return Convert.ToString(HttpContext.Current.Session[SessionManagerConstants.sReference]);
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.sReference] = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets Current User Team in session
        /// </summary>
        public static string sTeam
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.Team] != null)
                {
                    return HttpContext.Current.Session[SessionManagerConstants.Team].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.Team] = value;
                }
            }
        }

        public static string sSigRCategory
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.sSigRCategory] != null)
                {
                    return Convert.ToString(HttpContext.Current.Session[SessionManagerConstants.sSigRCategory]);
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.sSigRCategory] = value;
                }
            }
        }

        public static int iCampaignID
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.iCampaignID] != null)
                {
                    return Convert.ToInt32(HttpContext.Current.Session[SessionManagerConstants.iCampaignID]);
                }
                else
                {
                    return 0;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.iCampaignID] = value;
                }
            }
        }

        public static Guid ApplicationIDXIGUID
        {
            get
            {
                var APPIDGUID = Guid.Empty;
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.ApplicationIDXIGUID] != null)
                {
                    Guid.TryParse(HttpContext.Current.Session[SessionManagerConstants.ApplicationIDXIGUID].ToString(), out APPIDGUID);
                    return APPIDGUID;
                }
                else
                {
                    return Guid.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.ApplicationIDXIGUID] = value;
                }
            }
        }
        public static bool bDebug
        {
            get
            {
                bool Debug = false;
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.bDebug] != null)
                {
                    bool.TryParse(HttpContext.Current.Session[SessionManagerConstants.bDebug].ToString(), out Debug);
                    return Debug;
                }
                else
                {
                    return false;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.bDebug] = value;
                }
            }
        }
        //public static int ProjectID
        //{
        //    get
        //    {
        //        if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.ProjectID] != null)
        //        {
        //            //return Convert.ToString(HttpContext.Current.Session[SessionManagerConstants.ProjectID]);
        //            return Convert.ToInt32(HttpContext.Current.Session[SessionManagerConstants.ProjectID]);

        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }

        //    set
        //    {
        //        if (HttpContext.Current.Session != null)
        //        {
        //            HttpContext.Current.Session[SessionManagerConstants.ProjectID] = value;
        //        }
        //    }
        //}

        public static string ProjectID
        {
            get
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session[SessionManagerConstants.ProjectID] != null)
                {
                    //return Convert.ToString(HttpContext.Current.Session[SessionManagerConstants.ProjectID]);
                    return HttpContext.Current.Session[SessionManagerConstants.ProjectID].ToString();

                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SessionManagerConstants.ProjectID] = value;
                }
            }
        }
    }
}