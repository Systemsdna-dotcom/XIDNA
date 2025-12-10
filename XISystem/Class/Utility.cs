using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using xiEnumSystem;

namespace XISystem
{
    public static class Utility
    {
        public static List<CNV> SessionItems()
        {
            List<CNV> nCNV = new List<CNV>();
            if (HttpContext.Current.Session != null)
            {
                nCNV.Add(new CNV { sName = "sSessionID", sValue = HttpContext.Current.Session.SessionID.ToString() });
            }
            if (HttpContext.Current.Session != null && HttpContext.Current.Session["CoreDatabase"] != null)
            {
                nCNV.Add(new CNV { sName = "sDatabase", sValue = HttpContext.Current.Session["CoreDatabase"].ToString() });
            }
            if (HttpContext.Current.Session != null && HttpContext.Current.Session["OrgDatabase"] != null)
            {
                nCNV.Add(new CNV { sName = "sOrgDatabase", sValue = HttpContext.Current.Session["OrgDatabase"].ToString() });
            }
            if (HttpContext.Current.Session != null && HttpContext.Current.Session["UserID"] != null)
            {
                nCNV.Add(new CNV { sName = "iUserID", sValue = HttpContext.Current.Session["UserID"].ToString() });
            }
            if (HttpContext.Current.Session != null && HttpContext.Current.Session["OrganisationName"] != null)
            {
                nCNV.Add(new CNV { sName = "sOrgName", sValue = HttpContext.Current.Session["OrganisationName"].ToString() });
            }
            if (HttpContext.Current.Session != null && HttpContext.Current.Session["sUserName"] != null)
            {
                nCNV.Add(new CNV { sName = "sUserName", sValue = HttpContext.Current.Session["sUserName"].ToString() });
            }
            if (HttpContext.Current.Session != null && HttpContext.Current.Session["sRoleName"] != null)
            {
                nCNV.Add(new CNV { sName = "sRoleName", sValue = HttpContext.Current.Session["sRoleName"].ToString() });
            }
            if (HttpContext.Current.Session != null && HttpContext.Current.Session["sEmail"] != null)
            {
                nCNV.Add(new CNV { sName = "sEmail", sValue = HttpContext.Current.Session["sEmail"].ToString() });
            }
            if (HttpContext.Current.Session != null && HttpContext.Current.Session["UserUniqueID"] != null)
            {
                nCNV.Add(new CNV { sName = "sCurrentUserGUID", sValue = HttpContext.Current.Session["UserUniqueID"].ToString() });
            }
            if (HttpContext.Current.Session != null && HttpContext.Current.Session["OrganizationID"] != null)
            {
                nCNV.Add(new CNV { sName = "iOrganizationID", sValue = HttpContext.Current.Session["OrganizationID"].ToString() });
            }
            if (HttpContext.Current.Session != null && HttpContext.Current.Session["Name"] != null)
            {
                nCNV.Add(new CNV { sName = "sName", sValue = HttpContext.Current.Session["Name"].ToString() });
            }
            if (HttpContext.Current.Session != null && HttpContext.Current.Session["iRoleID"] != null)
            {
                nCNV.Add(new CNV { sName = "iRoleID", sValue = HttpContext.Current.Session["iRoleID"].ToString() });
            }
            return nCNV;
        }
        public static bool IsNumeric(this String input)
        {
            double temp;
            return double.TryParse(input, out temp);
        }
        public static bool IsDateTime(this String input)
        {
            bool result;
            var Date = ConvertToDate(input);
            if (Date != DateTime.MinValue)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
        //public static T DeepCopy<T>(this T objectToCopy)
        //{
        //    try
        //    {
        //        MemoryStream memoryStream = new MemoryStream();
        //        NetDataContractSerializer netFormatter = new NetDataContractSerializer();
        //        netFormatter.Serialize(memoryStream, objectToCopy);
        //        memoryStream.Position = 0;
        //        T returnValue = (T)netFormatter.Deserialize(memoryStream);
        //        memoryStream.Close();
        //        memoryStream.Dispose();
        //        return returnValue;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public static bool IsMatchedwithAny<T>(List<T> obj1, List<T> obj2)
        {
            int matchingcount = obj1.Intersect(obj2).Count();
            if (matchingcount <= 0)
            {
                return false;
            }
            return true;
        }
        public static T GetCopy<T>(this T objectToCopy)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                NetDataContractSerializer netFormatter = new NetDataContractSerializer();
                netFormatter.Serialize(memoryStream, objectToCopy);
                memoryStream.Position = 0;
                T returnValue = (T)netFormatter.Deserialize(memoryStream);
                memoryStream.Close();
                memoryStream.Dispose();
                return returnValue;
                //T newObj = Activator.CreateInstance<T>();
                //Type type = newObj.GetType();
                //foreach (PropertyInfo i in newObj.GetType().GetProperties())
                //{
                //    //"EntitySet" is specific to link and this conditional logic is optional/can be ignored
                //    //if (i.CanWrite && i.PropertyType.Name.Contains("EntitySet") == false)
                //    //{
                //    object value = S.GetType().GetProperty(i.Name).GetValue(S, null);
                //    i.SetValue(newObj, value, null);
                //    //}
                //}
                //return newObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string ToTimeStamp(this DateTime dValue, string Style = "")
        {
            if (string.IsNullOrEmpty(Style))
                Style = "H:mm:s";
            return dValue.ToString(Style);
        }
        //public static void ExcludeItems<T>(this T Source, string sItems)  where T : Dictionary<string, string>
        //{
        //    if (!string.IsNullOrEmpty(sItems))
        //    {
        //        // REMOVE ITEMS FROM DICTIONARY
        //        var lstitems = sItems.Split(',').Where(y => !string.IsNullOrEmpty(y)).Select(f => f.Trim()).ToList();
        //        lstitems = lstitems ?? new List<string>();
        //        foreach (var item in lstitems)
        //        {
        //            if (Source.ContainsKey(item))
        //            {
        //                Source.Remove(item);
        //            }
        //        }
        //    }
        //}
        public static string ConvertBase64(string strText)
        {
            try
            {
                if (!string.IsNullOrEmpty(strText))
                {
                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(strText);
                    string sbase64 = System.Convert.ToBase64String(plainTextBytes);
                    return sbase64;
                }
                return strText;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string FromBase64String(string strbase64)
        {
            try
            {
                if (!string.IsNullOrEmpty(strbase64))
                {
                    var base64EncodedBytes = System.Convert.FromBase64String(strbase64);
                    string soriginal = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                    return soriginal;
                }
                return strbase64;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string GetValueWithDelimiters(string sSource, Regex regexObj)
        {
            string dictresult = "";
            if (!string.IsNullOrEmpty(sSource))
            {
                dictresult = regexObj.Matches(sSource).Cast<Match>().Select(s => s.Groups[1].Value).Distinct().FirstOrDefault();
            }
            return dictresult;
        }
        public static string RemoveCharacters(string strInput, string Regpattern = "", string replacewith = "", string sremovechars = "")
        {
            try
            {
                string sresult = strInput;
                if (!string.IsNullOrEmpty(Regpattern))
                {
                    sresult = Regex.Replace(strInput, Regpattern, replacewith, RegexOptions.Compiled);
                }
                else if (!string.IsNullOrEmpty(sremovechars))
                {
                    sresult = strInput.Replace(sremovechars, replacewith);
                }
                return sresult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string SaveFile(string filepath, string filename, string extenstion, string Base64String, byte[] arraybytes = null)
        {
            try
            {
                if (arraybytes == null)
                {
                    if (!string.IsNullOrEmpty(Base64String))
                    {
                        arraybytes = Convert.FromBase64String(Base64String);
                    }
                }
                string sresult = "";
                if (!string.IsNullOrEmpty(filepath) && !string.IsNullOrEmpty(filename) && !string.IsNullOrEmpty(extenstion) && arraybytes != null)
                {
                    var lstextesions = Enum.GetNames(typeof(EnumFileExtensions)).ToList();
                    if (lstextesions.Contains(extenstion.Trim().ToLower()))
                    {
                        DirectoryInfo target = new DirectoryInfo(filepath);
                        if (!Directory.Exists(target.FullName))
                        {
                            Directory.CreateDirectory(target.FullName);
                        }
                        string fname = (RemoveCharacters(filename, @"(\s+|@|&|'|\(|\)|<|>|#)", "")) + "." + extenstion;
                        string finalpath = filepath + "/" + fname;
                        //Save into physical path
                        File.WriteAllBytes(finalpath, arraybytes);
                        sresult = fname;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return "Fail: " + ex.Message.ToString();
            }
        }

        public static DateTime ConvertToDate(string InputString)
        {
            // * NOTE: below codeto convert string to Datetime with current culture info
            var currentculture = CultureInfo.CurrentCulture;
            string getcurrent = currentculture.ToString();
            CultureInfo us = new CultureInfo(getcurrent);
            string[] formats = us.DateTimeFormat.GetAllDateTimePatterns();
            List<string> lst = formats.OfType<string>().ToList();
            lst.Add("dd-MMM-yyyy HH:mm:ss");
            lst.Add("dd-MMM-yyyy");
            lst.Add("dd-MM-yyyy");
            lst.Add("MM/dd/yyyy HH:mm:ss");
            lst.Add("MM/dd/yyyy");
            formats = lst.ToArray();
            //CultureInfo provider = CultureInfo.InvariantCulture;
            //string InputString = "31.12.18";
            //DateTime dateValue;
            //bool IsValidDate = DateTime.TryParseExact(InputString, formats, provider, DateTimeStyles.None, out dateValue);

            CultureInfo provider = CultureInfo.InvariantCulture;
            //string[] formats = {
            //"yyyy-MM-dd", "yyyy-MMM-dd", "yyyy.MM.dd","yyyy/MM/dd","yyyy/MMM/dd","yyyy.MMM.dd",
            //"dd-MM-yyyy","dd.MM.yyyy", "dd/MM/yyyy", "dd-MMM-yyyy", "dd.MMM.yyyy",
            //"MMM-dd-yyyy","MM-dd-yyyy", "MM.dd.yyyy", "MMM.dd.yyyy", "MM/dd/yyyy"
            //};
            DateTime dateValue;
            bool IsValidDate = DateTime.TryParseExact(InputString, formats, provider, DateTimeStyles.None, out dateValue);
            if (!IsValidDate)
            {
                dateValue = DateTime.MinValue;
            }
            return dateValue;
        }
        public static dynamic ConvertToExpected(string needconvert, int iTypeID, string sAttrName = null, BODatatypes enumtype = BODatatypes.NONE)
        {
            try
            {
                dynamic result = needconvert;
                if (iTypeID > 0) // then convert to Enum
                {
                    if (Enum.IsDefined(typeof(BODatatypes), iTypeID))
                    {
                        enumtype = (BODatatypes)iTypeID;
                    }
                }
                if (enumtype == BODatatypes.NONE && !string.IsNullOrEmpty(sAttrName))
                {
                    if (sAttrName.ToLower() == XIConstant.Key_XICrtdBy.ToLower() || sAttrName.ToLower() == XIConstant.Key_XICrtdWhn.ToLower() || sAttrName.ToLower() == XIConstant.Key_XIUpdtdBy.ToLower() || sAttrName.ToLower() == XIConstant.Key_XIUpdtdWhn.ToLower())
                    {
                        if (sAttrName.ToLower() == XIConstant.Key_XICrtdWhn.ToLower() || sAttrName.ToLower() == XIConstant.Key_XIUpdtdWhn.ToLower())
                        {
                            string sCurrentValue = DateTime.Now.ToString();
                            var dtDate = ConvertToDate(sCurrentValue);
                            result = dtDate.ToString(XIConstant.SqlDateFormat);
                        }
                        else
                        {
                            result = "0";
                        }
                    }
                }
                //Convert to enum 

                //if (!string.IsNullOrEmpty(needconvert))
                // {
                switch (enumtype)
                {
                    case BODatatypes.INT:
                        {
                            int val;
                            Int32.TryParse(needconvert, out val);
                            result = val;
                            break;
                        }
                    case BODatatypes.FLOAT:
                        {
                            float val;
                            float.TryParse(needconvert, out val);
                            result = val;
                            break;
                        }
                    case BODatatypes.BIGINT:
                        {
                            long val;
                            Int64.TryParse(needconvert, out val);
                            result = val;
                            break;
                        }
                    case BODatatypes.DECIMAL:
                        {
                            decimal val;
                            Decimal.TryParse(needconvert, out val);
                            result = val;
                            break;
                        }
                    case BODatatypes.DATETIME:
                        {
                            result = ConvertToDate(needconvert);
                            break;
                        }
                    case BODatatypes.DATE:
                        {
                            result = ConvertToDate(needconvert);
                            break;
                        }
                    case BODatatypes.BIT:
                        {
                            bool val;
                            Boolean.TryParse(needconvert, out val);
                            result = val;
                            break;
                        }
                    case BODatatypes.UniqueIdentifier:
                        {
                            Guid XIGUID;
                            Guid.TryParse(needconvert, out XIGUID);
                            result = XIGUID;
                            break;
                        }
                }
                //}
                return result;
            }
            catch (Exception ex)
            {
                return needconvert;
            }
        }
        public static Type ConverttoType(int iTypeID, BODatatypes enumtype = BODatatypes.NONE)
        {
            try
            {
                if (iTypeID > 0) // then convert to Enum
                {
                    if (Enum.IsDefined(typeof(BODatatypes), iTypeID))
                    {
                        enumtype = (BODatatypes)iTypeID;
                    }
                }
                //Convert to enum 
                Type type = typeof(string);
                switch (enumtype)
                {
                    case BODatatypes.INT:
                        {
                            type = typeof(int);
                            break;
                        }
                    case BODatatypes.FLOAT:
                        {
                            type = typeof(float);
                            break;
                        }
                    case BODatatypes.BIGINT:
                        {
                            type = typeof(long);
                            break;
                        }
                    case BODatatypes.DECIMAL:
                        {
                            type = typeof(decimal);
                            break;
                        }
                    case BODatatypes.DATETIME:
                        {
                            type = typeof(DateTime);
                            break;
                        }
                    case BODatatypes.DATE:
                        {
                            type = typeof(DateTime);
                            break;
                        }
                    case BODatatypes.UniqueIdentifier:
                        {
                            type = typeof(Guid);
                            break;
                        }
                }
                return type;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetDateResolvedValue(string sDateRange, string sFormat)
        {
            try
            {
                if (string.IsNullOrEmpty(sFormat))
                {
                    sFormat = "yyyy-MM-dd";
                }
                var TimeZone = System.Configuration.ConfigurationManager.AppSettings["TimeZone"];
                DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(TimeZone);
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);

                if (!string.IsNullOrEmpty(sDateRange))
                {
                    if (sDateRange.ToLower() == "T".ToLower() || sDateRange.ToUpper().StartsWith("T+") || sDateRange.ToUpper().StartsWith("T-"))
                    {
                        if (sDateRange.Contains("+"))
                        {
                            var oMinDate = sDateRange.Split('+');
                            int iAddDays = Convert.ToInt32(oMinDate[1]);
                            sDateRange = localTime.AddDays(iAddDays).Date.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else if (sDateRange.Contains("-"))
                        {
                            var oMinDate = sDateRange.Split('-');
                            int iAddDays = Convert.ToInt32(oMinDate[1]);
                            sDateRange = localTime.AddDays(-iAddDays).Date.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            //sDateRange = DateTime.Now.Date.ToString(sFormat);
                            sDateRange = localTime.ToString(sFormat);// DateTime.Now.ToString(sFormat);
                        }
                    }
                    else if (sDateRange.ToLower() == "M".ToLower() || sDateRange.ToUpper().StartsWith("M+") || sDateRange.ToUpper().StartsWith("M-"))
                    {
                        if (sDateRange.Contains("+"))
                        {
                            var oMinDate = sDateRange.Split('+');
                            var startDate = new DateTime(localTime.Year, localTime.Month, 1);
                            int iAddMonths = Convert.ToInt32(oMinDate[1]);
                            //var endDate = startDate.AddMonths(iAddMonths);
                            sDateRange = startDate.AddMonths(iAddMonths).Date.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else if (sDateRange.Contains("-"))
                        {
                            var oMinDate = sDateRange.Split('-');
                            var startDate = new DateTime(localTime.Year, localTime.Month, 1);
                            int iAddMonths = Convert.ToInt32(oMinDate[1]);
                            //var endDate = startDate.AddMonths(-iAddMonths);
                            sDateRange = startDate.AddMonths(-iAddMonths).Date.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            var startDate = new DateTime(localTime.Year, localTime.Month, 1);
                            sDateRange = startDate.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                    }
                    else if (sDateRange.ToLower() == "TM".ToLower() || sDateRange.ToUpper().StartsWith("TM+") || sDateRange.ToUpper().StartsWith("TM-"))
                    {
                        if (sDateRange.Contains("+"))
                        {
                            var oMinDate = sDateRange.Split('+');
                            var startDate = localTime;
                            int iAddMonths = Convert.ToInt32(oMinDate[1]);
                            //var endDate = startDate.AddMonths(iAddMonths);
                            sDateRange = startDate.AddMonths(iAddMonths).Date.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else if (sDateRange.Contains("-"))
                        {
                            var oMinDate = sDateRange.Split('-');
                            var startDate = localTime;
                            int iAddMonths = Convert.ToInt32(oMinDate[1]);
                            //var endDate = startDate.AddMonths(-iAddMonths);
                            sDateRange = startDate.AddMonths(-iAddMonths).Date.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            var startDate = localTime;
                            sDateRange = startDate.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                    }
                    else if (sDateRange.ToLower() == "Y".ToLower() || sDateRange.ToUpper().StartsWith("Y+") || sDateRange.ToUpper().StartsWith("Y-"))
                    {
                        if (sDateRange.Contains("+"))
                        {
                            DateTime firstDay = new DateTime(localTime.Year, 1, 1);
                            var oMinDate = sDateRange.Split('+');
                            int iAddYears = Convert.ToInt32(oMinDate[1]);
                            sDateRange = firstDay.AddYears(iAddYears).ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else if (sDateRange.Contains("-"))
                        {
                            DateTime firstDay = new DateTime(localTime.Year, 1, 1);
                            var oMinDate = sDateRange.Split('-');
                            int iAddYears = Convert.ToInt32(oMinDate[1]);
                            sDateRange = firstDay.AddYears(-iAddYears).ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            DateTime firstDay = new DateTime(localTime.Year, 1, 1);
                            sDateRange = firstDay.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                    }
                    else if (sDateRange.ToLower() == "TY".ToLower() || sDateRange.ToUpper().StartsWith("TY+") || sDateRange.ToUpper().StartsWith("TY-"))
                    {
                        if (sDateRange.Contains("+"))
                        {
                            var oMinDate = sDateRange.Split('+');
                            int iAddYears = Convert.ToInt32(oMinDate[1]);
                            sDateRange = localTime.AddYears(iAddYears).ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else if (sDateRange.Contains("-"))
                        {
                            var oMinDate = sDateRange.Split('-');
                            int iAddYears = Convert.ToInt32(oMinDate[1]);
                            sDateRange = localTime.AddYears(-iAddYears).ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            sDateRange = localTime.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                    }
                    else if (sDateRange.ToLower() == "MS".ToLower() || sDateRange.ToUpper().StartsWith("MS+") || sDateRange.ToUpper().StartsWith("MS-"))
                    {
                        DateTime firstDay = new DateTime(localTime.Year, 1, 1);
                        if (sDateRange.Contains("+"))
                        {
                            var oMinDate = sDateRange.Split('+');
                            int iAddMonths = Convert.ToInt32(oMinDate[1]);
                            sDateRange = firstDay.AddMonths(iAddMonths).ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else if (sDateRange.Contains("-"))
                        {
                            var oMinDate = sDateRange.Split('-');
                            int iAddMonths = Convert.ToInt32(oMinDate[1]);
                            sDateRange = firstDay.AddMonths(-iAddMonths).ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            sDateRange = firstDay.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                    }
                    else if (sDateRange.ToLower() == "WS".ToLower() || sDateRange.ToUpper().StartsWith("WS+") || sDateRange.ToUpper().StartsWith("WS-"))
                    {
                        DateTime startOfWeek = DateTime.Today.AddDays(-1 * (int)(DayOfWeek.Monday));
                        if (sDateRange.Contains("+"))
                        {
                            var oMinDate = sDateRange.Split('+');
                            int iAddWeeks = Convert.ToInt32(oMinDate[1]);
                            sDateRange = startOfWeek.AddDays(iAddWeeks).ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else if (sDateRange.Contains("-"))
                        {
                            var oMinDate = sDateRange.Split('-');
                            int iAddWeeks = Convert.ToInt32(oMinDate[1]);
                            sDateRange = startOfWeek.AddDays(-iAddWeeks).ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            sDateRange = startOfWeek.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                    }
                    else if (sDateRange.ToLower() == "YS".ToLower() || sDateRange.ToUpper().StartsWith("YS+") || sDateRange.ToUpper().StartsWith("YS-"))
                    {
                        DateTime firstDay = new DateTime(localTime.Year, 1, 1);
                        if (sDateRange.Contains("+"))
                        {
                            var oMinDate = sDateRange.Split('+');
                            int iAddYears = Convert.ToInt32(oMinDate[1]);
                            sDateRange = firstDay.AddYears(iAddYears).ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else if (sDateRange.Contains("-"))
                        {
                            var oMinDate = sDateRange.Split('-');
                            int iAddYears = Convert.ToInt32(oMinDate[1]);
                            sDateRange = firstDay.AddYears(-iAddYears).ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            sDateRange = firstDay.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                    }
                    else if (sDateRange.Contains("xi.s"))
                    {

                    }
                    else if (!System.Text.RegularExpressions.Regex.IsMatch(sDateRange, "^[0-9]*$"))
                    {
                        if (sDateRange.Contains("+"))
                        {
                            int iAddDays = 0;
                            var oMinDate = sDateRange.Split('+');
                            DateTime oVisualDate = Convert.ToDateTime(oMinDate[0]);
                            if (oMinDate.Count() > 1)
                            {
                                iAddDays = Convert.ToInt32(oMinDate[1]);
                            }
                            sDateRange = oVisualDate.AddDays(iAddDays).Date.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else if (sDateRange.Contains("-"))
                        {
                            int iAddDays = 0;
                            var oMinDate = sDateRange.Split('+');
                            DateTime oVisualDate = Convert.ToDateTime(oMinDate[0]);
                            if (oMinDate.Count() > 1)
                            {
                                iAddDays = Convert.ToInt32(oMinDate[1]);
                            }
                            sDateRange = oVisualDate.AddDays(-iAddDays).Date.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            DateTime oVisualDate = ConvertToDate(sDateRange);
                            //DateTime oVisualDate = Convert.ToDateTime(sDateRange);
                            sDateRange = oVisualDate.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                    }
                }
                //else
                //{
                //    sDateRange = Convert.ToDateTime("1920-01-01").ToString("yyyy-MM-dd");
                //}
                return sDateRange;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetDefaultDateResolvedValue(string sDateRange, string sFormat)
        {
            try
            {
                if (string.IsNullOrEmpty(sFormat))
                {
                    sFormat = "yyyy-MM-dd";
                }
                var TimeZone = System.Configuration.ConfigurationManager.AppSettings["TimeZone"];
                DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(TimeZone);
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
                if (!string.IsNullOrEmpty(sDateRange))
                {
                    if (sDateRange.ToLower() == "T".ToLower() || sDateRange.StartsWith("T+") || sDateRange.StartsWith("T-"))
                    {
                        if (sDateRange.Contains("+"))
                        {
                            var oMinDate = sDateRange.Split('+');
                            int iAddDays = Convert.ToInt32(oMinDate[1]);
                            sDateRange = localTime.AddDays(iAddDays).Date.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else if (sDateRange.Contains("-"))
                        {
                            var oMinDate = sDateRange.Split('-');
                            int iAddDays = Convert.ToInt32(oMinDate[1]);
                            sDateRange = localTime.AddDays(-iAddDays).Date.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            sDateRange = localTime.Date.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                    }
                    else if (sDateRange.ToLower() == "M".ToLower() || sDateRange.StartsWith("M+") || sDateRange.StartsWith("M-"))
                    {
                        if (sDateRange.Contains("+"))
                        {
                            var oMinDate = sDateRange.Split('+');
                            var startDate = new DateTime(localTime.Year, localTime.Month, 1);
                            int iAddMonths = Convert.ToInt32(oMinDate[1]);
                            //var endDate = startDate.AddMonths(iAddMonths);
                            sDateRange = startDate.AddMonths(iAddMonths).Date.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else if (sDateRange.Contains("-"))
                        {
                            var oMinDate = sDateRange.Split('-');
                            var startDate = new DateTime(localTime.Year, localTime.Month, 1);
                            int iAddMonths = Convert.ToInt32(oMinDate[1]);
                            //var endDate = startDate.AddMonths(-iAddMonths);
                            sDateRange = startDate.AddMonths(-iAddMonths).Date.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            var startDate = new DateTime(localTime.Year, localTime.Month, 1);
                            sDateRange = startDate.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                    }
                    else if (sDateRange.ToLower() == "TM".ToLower() || sDateRange.StartsWith("TM+") || sDateRange.StartsWith("TM-"))
                    {
                        if (sDateRange.Contains("+"))
                        {
                            var oMinDate = sDateRange.Split('+');
                            var startDate = localTime;
                            int iAddMonths = Convert.ToInt32(oMinDate[1]);
                            //var endDate = startDate.AddMonths(iAddMonths);
                            sDateRange = startDate.AddMonths(iAddMonths).Date.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else if (sDateRange.Contains("-"))
                        {
                            var oMinDate = sDateRange.Split('-');
                            var startDate = localTime;
                            int iAddMonths = Convert.ToInt32(oMinDate[1]);
                            //var endDate = startDate.AddMonths(-iAddMonths);
                            sDateRange = startDate.AddMonths(-iAddMonths).Date.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            var startDate = localTime;
                            sDateRange = startDate.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                    }
                    else if (sDateRange.ToLower() == "Y".ToLower() || sDateRange.StartsWith("Y+") || sDateRange.StartsWith("Y-"))
                    {
                        if (sDateRange.Contains("+"))
                        {
                            DateTime firstDay = new DateTime(localTime.Year, 1, 1);
                            var oMinDate = sDateRange.Split('+');
                            int iAddYears = Convert.ToInt32(oMinDate[1]);
                            sDateRange = firstDay.AddYears(iAddYears).ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else if (sDateRange.Contains("-"))
                        {
                            DateTime firstDay = new DateTime(localTime.Year, 1, 1);
                            var oMinDate = sDateRange.Split('-');
                            int iAddYears = Convert.ToInt32(oMinDate[1]);
                            sDateRange = firstDay.AddYears(-iAddYears).ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            DateTime firstDay = new DateTime(localTime.Year, 1, 1);
                            sDateRange = firstDay.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                    }
                    else if (sDateRange.ToLower() == "TY".ToLower() || sDateRange.StartsWith("TY+") || sDateRange.StartsWith("TY-"))
                    {
                        if (sDateRange.Contains("+"))
                        {
                            var oMinDate = sDateRange.Split('+');
                            int iAddYears = Convert.ToInt32(oMinDate[1]);
                            sDateRange = localTime.AddYears(iAddYears).ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else if (sDateRange.Contains("-"))
                        {
                            var oMinDate = sDateRange.Split('-');
                            int iAddYears = Convert.ToInt32(oMinDate[1]);
                            sDateRange = localTime.AddYears(-iAddYears).ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            sDateRange = localTime.ToString(sFormat, CultureInfo.InvariantCulture);
                        }
                    }
                    else
                    {
                        var dtDate = ConvertToDate(sDateRange);
                        sDateRange = dtDate.ToString(sFormat, CultureInfo.InvariantCulture);
                    }
                }
                //else
                //{
                //    sDateRange = Convert.ToDateTime("1920-01-01").ToString("yyyy-MM-dd");
                //}
                return sDateRange;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool IsValidGUID(string svalue)
        {
            try
            {
                if (!string.IsNullOrEmpty(svalue))
                {
                    Guid Gresult;
                    var Isvalid = Guid.TryParse(svalue, out Gresult);
                    return Isvalid;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #region XIScript_Execution

        public static object EvaluateScript<T>(string sScript, T source)
        {
            CResult oCResult = new CResult();
            object oResult = "";
            try
            {
                string sCode = @"  using System;
                  using System.Core;
                  using System.Linq;
                  using System.Collections.Generic;
                  using System.Threading;
                  using System.Data;
                  using System.Data.SqlClient;
                  using System.Data.Entity;
                  using XISystem;
                   namespace XIScriptingNotation
                   {                
                       public class cXIScriptingNotation
                       {                
                             public static CResult GetNotationResult(DataTable source)
                  {
             CResult CResult = new CResult();
            var oResult = " + sScript + @";
             CResult.oResult = oResult;
            return CResult;
        }
                       }
                   }
";
                CompilerParameters loParameters = new CompilerParameters();
                string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                string sPath = physicalPath.Substring(0, physicalPath.Length) + "\\DLL";
                string sdllPath = physicalPath.Substring(0, physicalPath.Length) + "\\bin";
                //string sPath = System.Configuration.ConfigurationManager.AppSettings["DLLPath"];
                // *** Start by adding any referenced assemblies
                loParameters.ReferencedAssemblies.Add("System.dll");
                loParameters.ReferencedAssemblies.Add(sPath + "\\System.Core.dll");
                loParameters.ReferencedAssemblies.Add(sPath + "\\System.Linq.dll");
                loParameters.ReferencedAssemblies.Add(sPath + "\\System.Data.dll");
                loParameters.ReferencedAssemblies.Add(sPath + "\\EntityFramework.dll");
                loParameters.ReferencedAssemblies.Add(sPath + "\\EntityFramework.SqlServer.dll");
                loParameters.ReferencedAssemblies.Add(sdllPath + "\\XICore.dll");
                loParameters.ReferencedAssemblies.Add(sPath + "\\System.XML.dll");
                loParameters.ReferencedAssemblies.Add(sdllPath + "\\XIDataBase.dll");
                loParameters.ReferencedAssemblies.Add(sdllPath + "\\XISystem.dll");
                loParameters.GenerateInMemory = false;
                ICodeCompiler provider = new CSharpCodeProvider().CreateCompiler();
                CompilerResults results = provider.CompileAssemblyFromSource(loParameters, sCode);
                if (results.Errors.HasErrors)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (CompilerError error in results.Errors)
                    {
                        //Common.SaveErrorLog(error.ErrorText, "XIDNA");
                        sb.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
                    }
                    throw new InvalidOperationException(sb.ToString());
                }
                Type binaryFunction = results.CompiledAssembly.GetType("XIScriptingNotation.cXIScriptingNotation");
                MethodInfo methodInfo = binaryFunction.GetMethod("GetNotationResult");
                if (source == null)
                {
                    oResult = methodInfo.Invoke(null, new object[] { null });
                }
                else
                {
                    oResult = methodInfo.Invoke(null, new object[] { source });
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oResult;
        }
        #endregion


        #region EXTENSION_METHODS

        private static string ExpressionResolver(DataTable dtSource, string expression)
        {
            string sresult = "";
            if (!string.IsNullOrEmpty(expression) && dtSource.Columns.Count > 0)
            {
                var startindex = expression.IndexOf('[');
                var endindex = expression.IndexOf(']') - startindex;
                var res = expression.Substring(startindex, endindex + 1);
                dtSource.Columns.Add("AddColumn", typeof(Int32), "CONVERT(" + res.Substring(1, res.Length - 2) + ", 'System.Int32')");
                expression = expression.Replace(res, "AddColumn");
                var oResult = (CResult)EvaluateScript<DataTable>(expression, dtSource);
                //var amount=dtSource.AsEnumerable().Sum(x => Convert.ToInt32(x["PPrice"]));
                if (oResult != null && oResult.oResult != null)
                {
                    sresult = oResult.oResult.ToString();
                }
                dtSource.Columns.Remove("AddColumn");
            }
            return sresult;
        }
        public static DataTable BuildDataTableWithString(this string sData, EnumReportAppendTypes formatType, bool IsHeader = false, DataTable dtSource = null, bool IsRowTotal = false, bool IsCalculate = false, bool IsTotal = false, bool IsColour = false, bool ColumnValue = false)
        {
            DataTable dt = new DataTable();
            try
            {
                Regex regPattern = new Regex(Regex.Escape("{{") + "(.+?)" + Regex.Escape("}}"));
                char splitoperator = '$';
                if (!string.IsNullOrEmpty(sData))
                {
                    var splitData = sData.Split('|').Select(d => d.Trim()).ToList();
                    // Whether it is the rows or column
                    if (IsRowTotal == true)
                    {
                        if (splitData.Count() == dtSource.Columns.Count)
                        {
                            dtSource.Columns.Add(splitData[0], typeof(decimal));
                            foreach (DataRow row in dtSource.Rows)
                            {
                                decimal rowSum = 0;
                                foreach (DataColumn col in dtSource.Columns)
                                {
                                    if (!row.IsNull(col))
                                    {
                                        string stringValue = row[col].ToString();
                                        decimal d;
                                        if (decimal.TryParse(stringValue, out d))
                                            rowSum += d;
                                    }
                                }
                                row.SetField(splitData[0], rowSum);
                            }
                        }
                        else
                        {
                            dtSource.Columns.Add(splitData[0], typeof(decimal));
                            foreach (DataRow row in dtSource.Rows)
                            {
                                decimal rowSum = 0;
                                foreach (DataColumn col in dtSource.Columns)
                                {
                                    if (!row.IsNull(col) && col.Ordinal > 0)
                                    {
                                        string stringValue = row[col].ToString();
                                        decimal d;
                                        if (decimal.TryParse(stringValue, out d))
                                            rowSum += d;
                                    }
                                }
                                row.SetField(splitData[0], rowSum);
                            }
                        }
                        dt = dtSource;
                    }
                    else if (ColumnValue == true)
                    {
                        int Columnscount = splitData.Count();
                        dtSource.Columns.Add(splitData[0]);
                        int Numerator = 0;
                        int Denominator = 0;
                        int i = 0;
                        for (int clm = 0; clm < Columnscount; clm++)
                        {
                            if (splitData[clm].Contains("{{"))
                            {
                                splitData[clm] = splitData[clm].Substring(splitData[clm].IndexOf("{{") + 2, splitData[clm].IndexOf("}}") - (splitData[clm].IndexOf("{{") + 2));
                                foreach (DataRow row in dtSource.Rows)
                                {
                                    var Column = splitData[clm].Split('=')[0];
                                    var ListConditions = splitData[clm].Split(',');
                                    foreach (DataColumn col in dtSource.Columns)
                                    {
                                        if (ListConditions.Contains(col.Caption.Replace(" ", "")))
                                        {
                                            if (col.Caption.Replace(" ", "") == ListConditions[0] && row[col] != DBNull.Value)
                                            {

                                                Numerator = Convert.ToInt32(row[col]);
                                            }
                                            if (col.Caption.Replace(" ", "") == ListConditions[1] && row[col] != DBNull.Value)
                                            {
                                                Denominator = Convert.ToInt32(row[col]);
                                            }
                                        }
                                        if (col.Caption == splitData[0] && Denominator != 0)
                                        {
                                            if (splitData[1] == "/" && Denominator != 0)
                                            {
                                                var value = Math.Round(Convert.ToDecimal(Numerator / Denominator));
                                                dtSource.Rows[i][col.Caption] = value;

                                            }
                                            else if (splitData[1] == "%" && Denominator != 0)
                                            {
                                                string value = Convert.ToString(Math.Round((Convert.ToDecimal(Numerator) / Convert.ToDecimal(Denominator)) * 100, 2));
                                                dtSource.Rows[i][col.Caption] = value;
                                            }
                                            else
                                            {
                                                string value = "0";
                                                dtSource.Rows[i][col.Caption] = value;
                                            }

                                        }
                                    }
                                    i++;
                                }
                            }
                        }
                    }
                    else if (splitData.Count() > 0 && (formatType == EnumReportAppendTypes.Left || formatType == EnumReportAppendTypes.Right))
                    {
                        // Build data table As Columns
                        int Columnscount = splitData.Count();
                        if (IsHeader == true)
                        {
                            for (int i = 0; i < Columnscount; i++)
                                dt.Columns.Add(splitData[i], typeof(string));
                        }
                        else if (IsCalculate == true && IsTotal == true)
                        {
                            DataRow row1 = dtSource.NewRow();
                            //sParameterValue.Substring(0, 1) == "["

                            foreach (DataColumn col in dtSource.Columns)
                            {
                                decimal rowSum = 0;
                                var replaces = splitData[1].Split('=')[1];
                                string replace1 = replaces.Remove(replaces.Length - 2);
                                List<string> rep = new List<string>(replace1.Split(','));
                                foreach (DataRow row in dtSource.Rows)
                                {
                                    //list.Any(x => stringValue.Contains(x));
                                    foreach (var items in rep)
                                    {
                                        //if (items.ToString()== row.ItemArray[0].ToString())
                                        //{
                                        if (!row.IsNull(col) && col.Ordinal > 0 && items.ToString() == col.ColumnName) /*row.ItemArray[0].ToString()*/
                                        {
                                            string stringValue = row[col].ToString();
                                            decimal d;
                                            if (decimal.TryParse(stringValue, out d))
                                                rowSum += d;
                                        }
                                        //}
                                    }
                                }
                                row1[col.Ordinal] = rowSum;
                            }
                            row1[0] = splitData[0];
                            dt = dtSource.Clone();
                            dt.Rows.Add(row1.ItemArray);

                        }

                        //else if (IsColour == true)
                        //{
                        //    DataRow row2 = dtSource.NewRow();
                        //    foreach (DataRow row in dtSource.Rows)
                        //    {
                        //        row2 = row;
                        //    }
                        //   dt= dtSource.Clone();
                        //    dt.Rows.Add(row2.ItemArray);
                        //}
                        else
                        {
                            // Add rows here
                            int rowslength = splitData[0].Split(splitoperator).Count();
                            object[] values = new object[Columnscount];
                            if (IsCalculate == true && splitData.Count() != dtSource.Columns.Count)
                            {
                                for (int i = 1; i <= dtSource.Columns.Count; i++)
                                    dt.Columns.Add("Column_" + i, typeof(string));
                                values = new object[dtSource.Columns.Count];
                                for (int clm = 0; clm < Columnscount; clm++)
                                {
                                    if (splitData[clm].Contains("{{"))
                                    {

                                        splitData[clm] = splitData[clm].Substring(splitData[clm].IndexOf("{{") + 2, splitData[clm].IndexOf("}}") - (splitData[clm].IndexOf("{{") + 2));
                                        //var SplitList = splitData[1].Split(';');
                                        var Column = splitData[clm].Split('=')[0];
                                        var ListConditions = splitData[clm].Split('=')[clm].Split(',');

                                        foreach (DataColumn dc in dtSource.Columns)
                                        {
                                            if (dc.Ordinal > 0)
                                            {
                                                var Numerator = (from DataRow dr in dtSource.Rows
                                                                 where (string)dr[Column] == ListConditions[0]
                                                                 select dr[dc.Caption]).FirstOrDefault();
                                                var Denominator = (from DataRow dr in dtSource.Rows
                                                                   where (string)dr[Column] == ListConditions[1]
                                                                   select dr[dc.Caption]).FirstOrDefault();
                                                if (Denominator != null && Denominator.ToString() != "0")
                                                {
                                                    values[dc.Ordinal] = (Denominator == DBNull.Value || Denominator == null) ? " " : Math.Round((Convert.ToDecimal(Numerator == DBNull.Value ? 0 : Numerator) / Convert.ToDecimal(Denominator == DBNull.Value ? 0 : Denominator)) * 100, 0) + "%";
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        values[clm] = splitData[clm];
                                    }
                                }
                                dt.Rows.Add(values);
                            }
                            else
                            {
                                for (int i = 1; i <= Columnscount; i++)
                                    dt.Columns.Add("Column_" + i, typeof(string));
                                for (int rw = 0; rw < rowslength; rw++)
                                {
                                    for (int clm = 0; clm < Columnscount; clm++)
                                    {
                                        if (IsCalculate == true && splitData[clm].Split(splitoperator)[rw].Contains("{{"))
                                        {
                                            splitData[clm] = splitData[clm].Substring(splitData[clm].IndexOf("{{") + 2, splitData[clm].IndexOf("}}") - (splitData[clm].IndexOf("{{") + 2));
                                            var SplitList = splitData[clm].Split(';');
                                            var Column = SplitList[0].Split('=')[0];
                                            var ListConditions = SplitList[0].Split('=')[1].Split(',');

                                            var Numerator = (from DataRow dr in dtSource.Rows
                                                             where (string)dr[Column] == ListConditions[0]
                                                             select dr[SplitList[1]]).FirstOrDefault();
                                            var Denominator = (from DataRow dr in dtSource.Rows
                                                               where (string)dr[Column] == ListConditions[1]
                                                               select dr[SplitList[1]]).FirstOrDefault();
                                            values[clm] = Denominator == DBNull.Value ? " " : Math.Round((Convert.ToDecimal(Numerator == DBNull.Value ? 0 : Numerator) / Convert.ToDecimal(Denominator == DBNull.Value ? 0 : Denominator)) * 100, 0) + "%";
                                        }
                                        else if (IsCalculate == false && splitData[clm].Split(splitoperator)[rw].Contains("{{"))
                                        {
                                            values[clm] = ExpressionResolver(dtSource, GetValueWithDelimiters(splitData[clm].Split(splitoperator)[rw], regPattern));// Expression execution value
                                        }
                                        else
                                        {
                                            values[clm] = splitData[clm].Split(splitoperator)[rw];
                                        }
                                    }
                                    dt.Rows.Add(values);
                                }
                            }
                        }
                    }
                    else if (splitData.Count() > 0 && formatType == EnumReportAppendTypes.Bottom)
                    {
                        // FOR ROWS APPENDING TO
                        var rowscount = splitData.Count();
                        var columnscount = splitData[0].Split(splitoperator).Count();
                        for (int i = 1; i <= columnscount; i++)
                            dt.Columns.Add("Column_" + i, typeof(string));
                        foreach (var rws in splitData)
                        {
                            var splitlist = rws.Split(splitoperator).Select(d => d).ToList();
                            int ioccuranceCount = splitlist.Where(v => v.Contains("{{")).Count();
                            if (ioccuranceCount > 0)
                            {
                                List<string> lstresult = new List<string>();
                                foreach (var ro in splitlist)
                                {
                                    if (ro.Contains("{{"))
                                        lstresult.Add(ExpressionResolver(dtSource, ro));
                                    else
                                        lstresult.Add(ro);
                                }
                                object[] rwvalues = lstresult.Cast<object>().ToArray();
                                dt.Rows.Add(rwvalues);
                            }
                            else
                            {
                                object[] rwvalues = rws.Split(splitoperator).Cast<object>().ToArray();
                                dt.Rows.Add(rwvalues);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return dt;
        }

        public static DataTable SetHeadersToDatatable(this DataTable dtsource, string sHeaders)
        {
            if (!string.IsNullOrEmpty(sHeaders))
            {
                var columns = sHeaders.Split(',').ToList();
                if (columns.Count() == dtsource.Columns.Count) // Compare column count and headers count here
                {
                    for (int clm = 0; clm < dtsource.Columns.Count; clm++)
                    {
                        dtsource.Columns[clm].ColumnName = columns[clm];
                    }
                }
            }
            return dtsource;
        }

        public static DataTable LeftRightMerging(this DataTable left, DataTable right)
        {
            DataTable dtresult = left.Clone();
            /* MERGING TWO DATATABLES HERE YOU CAN SWAP THE LEFT,RIGHT OBJECTS TO GET LEFT AND RIGHT MERGING*/
            if (left == null || right == null || (left.Rows.Count != right.Rows.Count))
            {
                for (int clm = 0; clm < left.Rows.Count; clm++)
                {
                    dtresult.Columns[clm].DataType = typeof(string);
                    if (right.Rows[clm].ItemArray[0].ToString() != left.Rows[clm].ItemArray[0].ToString())//==dtresult.Rows[clm]) && clm > 0)
                    {
                        var dataRow = right.NewRow();
                        right.Rows.InsertAt(dataRow, clm);
                        right.Rows[clm].ItemArray[0] = "0";
                    }
                    // Log here
                }
                right.Columns.RemoveAt(0);
                //  return left;
                // throw new ArgumentNullException("left or right or rows count doesnot match", "Both tables must not be null");
            }
            if (right.Columns.Count > 1)
            {
                right.Columns.RemoveAt(0);
            }
            foreach (DataColumn col in right.Columns)
            {
                string newColumnName = col.ColumnName;
                int colNum = 1;
                while (dtresult.Columns.Contains(newColumnName))
                {
                    newColumnName = string.Format("{0}_{1}", col.ColumnName, ++colNum);
                }
                dtresult.Columns.Add(newColumnName, col.DataType);
            }
            var mergedRows = left.AsEnumerable().Zip(right.AsEnumerable(),
                (r1, r2) => r1.ItemArray.Concat(r2.ItemArray).ToArray());
            foreach (object[] rowFields in mergedRows)
                dtresult.Rows.Add(rowFields);
            return dtresult;
        }
        public static DataTable BottomMerging(this DataTable first, DataTable second, EnumPivot Pivot)
        {
            DataTable dtresult = new DataTable();
            try
            {
                DataTable dtsecond = new DataTable();
                /*BOTTON MERGING APPENDING HERE*/
                // Comparing two objects Column Count
                if (Pivot == EnumPivot.Yes)
                {
                    if (first.Columns.Count == second.Columns.Count)
                    {
                        dtresult = first.Clone();
                        for (int clm = 0; clm < dtresult.Columns.Count; clm++)
                        {
                            dtresult.Columns[clm].DataType = typeof(string);
                            second.Columns[clm].ColumnName = dtresult.Columns[clm].ColumnName;
                        }
                        // fill the data here
                        foreach (DataRow row in first.Rows)
                            dtresult.ImportRow(row);
                        foreach (DataRow row in second.Rows)
                            dtresult.ImportRow(row);
                    }
                    else
                    {
                        dtresult = first.Clone();
                        //List<CDropDown> data = second.AsEnumerable().Select(row => new CDropDown
                        //{
                        //    Type = row.Field<string>(0),
                        //    text = String.IsNullOrEmpty(row.Field<string>(1)) ? "null" : row.Field<string>(1),
                        //    Value= row.Field<int>(2)
                        //}).ToList();
                        for (int clm = 0; clm < dtresult.Columns.Count; clm++)
                        {
                            dtresult.Columns[clm].DataType = typeof(string);
                            if (!second.Columns.Contains(dtresult.Columns[clm].ColumnName) && clm > 0)
                            {
                                //dtresult.Columns[clm].DefaultValue = "0";
                                second.Columns.Add(dtresult.Columns[clm].ColumnName).SetOrdinal(clm);
                                second.Columns[clm].DefaultValue = "0";
                                if (second.Rows.Count != 0)
                                {
                                    second.Rows[0][dtresult.Columns[clm].ColumnName.ToString()] = 0;
                                }
                            }
                            else
                            {
                                second.Columns[clm].ColumnName = dtresult.Columns[clm].ColumnName;
                            }
                        }
                        // fill the data here
                        foreach (DataRow row in first.Rows)
                            dtresult.ImportRow(row);
                        foreach (DataRow row in second.Rows)
                            dtresult.ImportRow(row);
                    }
                }
                else
                {
                    if (first.Columns.Count == second.Columns.Count)
                    {
                        dtresult = first.Clone();
                        for (int clm = 0; clm < dtresult.Columns.Count; clm++)
                        {
                            dtresult.Columns[clm].DataType = typeof(string);
                            second.Columns[clm].ColumnName = dtresult.Columns[clm].ColumnName;
                        }
                        // fill the data here
                        foreach (DataRow row in first.Rows)
                            dtresult.ImportRow(row);
                        foreach (DataRow row in second.Rows)
                            dtresult.ImportRow(row);
                        ////second table datatype need to change.
                        //dtsecond = second.Clone();
                        //for (int clm = 0; clm < dtsecond.Columns.Count; clm++)
                        //    dtsecond.Columns[clm].DataType = typeof(string);

                        //foreach (DataRow row in second.Rows)
                        //    dtsecond.ImportRow(row);
                        //// Merge here
                        //dtresult.Merge(dtsecond);
                        //dtresult.AcceptChanges();
                    }
                    else
                    {
                        // Here Log error
                        return first;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return dtresult;
        }
        public static DataTable ToPivotTable<T, TColumn, TRow, TData>(this IEnumerable<T> source, Func<T, TColumn> columnSelector, Expression<Func<T, TRow>> rowSelector, Func<IEnumerable<T>, TData> dataSelector)
        {
            DataTable table = new DataTable();
            var rowName = ((MemberExpression)rowSelector.Body).Member.Name;
            table.Columns.Add(new DataColumn(rowName));
            var columns = source.Select(columnSelector).Distinct();

            foreach (var column in columns)
                table.Columns.Add(new DataColumn(column.ToString()));
            var rows = source.GroupBy(rowSelector.Compile())
                             .Select(rowGroup => new
                             {
                                 Key = rowGroup.Key,
                                 Values = columns.GroupJoin(
                                     rowGroup,
                                     c => c,
                                     r => columnSelector(r),
                                     (c, columnGroup) => dataSelector(columnGroup))
                             });
            foreach (var row in rows)
            {
                var dataRow = table.NewRow();
                var items = row.Values.Cast<object>().ToList();
                items.Insert(0, row.Key);
                dataRow.ItemArray = items.ToArray();
                table.Rows.Add(dataRow);
            }
            return table;
        }

        public static string XIGUID(this int ID)
        {
            cEncryption oEncrypt = new cEncryption();
            var XIGUID = HttpContext.Current.Session["XIGUID"].ToString();
            var sEncString = XIGUID + "_" + ID;
            var sSecureKey = oEncrypt.EncryptData(sEncString, true, XIGUID);
            return sSecureKey;
        }

        #endregion

        public static string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            if (context == null)
            {
                return string.Empty;
            }
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        public static string Get_XILinkUID(int XILinkID)
        {
            cEncryption oEncrypt = new cEncryption();
            var XIGUID = HttpContext.Current.Session["XIGUID"].ToString();
            var sEncString = XIGUID + "_" + XILinkID;
            var sSecureKey = oEncrypt.EncryptData(sEncString, true, XIGUID);
            //var DecryptKey = oEncrypt.DecryptData(sSecureKey, true, XIGUID);

            return sSecureKey;
        }
        public static string GetBOGroupFields(string oGroupFields, bool bIsCrtdBy, bool bIsCrtdWhn, bool bIsUpdtdBy, bool bIsUpdtdWhn)
        {
            if (bIsCrtdBy == true)
            {
                oGroupFields = oGroupFields + ", " + "XICreatedBy";
            }
            if (bIsCrtdWhn == true)
            {
                oGroupFields = oGroupFields + ", " + "XICreatedWhen";
            }
            if (bIsUpdtdBy == true)
            {
                oGroupFields = oGroupFields + ", " + "XIUpdatedBy";
            }
            if (bIsUpdtdWhn == true)
            {
                oGroupFields = oGroupFields + ", " + "XIUpdatedWhen";
            }
            return oGroupFields;
        }

        public static string GetFileSizeinMB(Int64 bytes)
        {
            string[] suffixes =
                    { "Bytes", "KB", "MB", "GB", "TB", "PB" };
            int counter = 0;
            decimal number = (decimal)bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return string.Format("{0:n1}{1}", number, suffixes[counter]);
        }
    }
}