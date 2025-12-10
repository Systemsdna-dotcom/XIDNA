using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VM_cXIAppUserDetails
    {
        public int UserID { get; set; }
        public string sCoreDatabase { get; set; }
        public string sUserDatabase { get; set; }
        public int FKiOrgID { get; set; }
        public string sRoleName { get; set; }
        public int iRoleID { get; set; }
        public int iLayoutID { get; set; }
        public int iThemeID { get; set; }
        public string sUserName { get; set; }
        public string sAppName { get; set; }
        public string sThemeName { get; set; }
        public int FKiApplicationID { get; set; }
    }
    public class VM_CacheConfig
    {
        public string sUserName { get; set; }
        public string sRole { get; set; }
        public string sKey { get; set; }
        public EnumCacheTypes CacheType { get; set; }
        public long Size { get; set; }

    }
    public class VM_CacheData
    {
        public List<VM_CacheConfig> CacheList { get; set; }
        public EnumCacheTypes Cachefilteredtype { get; set; }
    }
    public class VM_UserLoginCache
    {
        public string sUserName { get; set; }
        public string sRole { get; set; }
    }
    public class VM_UserCacheKeyValue
    {
        public string sKey { get; set; }
        public string sValue { get; set; }
        public string sGUID { get; set; }
    }
}
