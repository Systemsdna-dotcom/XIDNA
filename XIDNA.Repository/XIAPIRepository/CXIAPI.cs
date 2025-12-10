using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using XIDNA.Models;
using XIDNA.ViewModels;

namespace XIDNA.Repository
{
    public class CXiAPI
    {
        public string DevDB { get; set; }
        public string SharedDB { get; set; }
        //readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        CommonRepository Common = new CommonRepository();
        cXICache oCache = new cXICache();
        #region BOMethods
        //Common functions

        public BOs Get_BODefinition(string sBO, int iUserID, string sOrgName, string sDatabase)
        {
            var CacheStatus = ConfigurationManager.AppSettings["Cache"];
            object oBO = new object();
            if (CacheStatus != "OFF")
            {
                if (HttpRuntime.Cache[sBO] == null)
                {
                    var Obj = AddBOToCache(sBO, iUserID, sOrgName, sDatabase);
                    if (Obj != null)
                    {
                        HttpRuntime.Cache.Add(sBO, Obj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                        oBO = HttpRuntime.Cache[sBO];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    oBO = HttpRuntime.Cache[sBO];
                }
                //return ((IXiLinkRepository)oXiLink).DeepClone();
            }
            else
            {
                var Obj = AddBOToCache(sBO, iUserID, sOrgName, sDatabase);
                oBO = Obj;
            }
            return (BOs)oBO;
        }

        private object AddBOToCache(string sBO, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            ModelDbContext dbContext = new ModelDbContext();
            var AllBOs = dbContext.BOs.ToList();
            BOs oBO = new BOs();
            oBO = AllBOs.Where(m => m.Name.ToLower() == sBO.ToLower()).FirstOrDefault();
            var MasterDataFields = oBO.BOFields.Where(m => m.iMasterDataID > 0).ToList();
            foreach (var items in MasterDataFields)
            {
                List<VMDropDown> FieldDDL = new List<VMDropDown>();
                FieldDDL = dbContext.Types.Where(m => m.Code == items.iMasterDataID).ToList().Select(m => new VMDropDown { ID = m.ID, Expression = m.Expression }).ToList();
                oBO.BOFields.Where(m => m.ID == items.ID).Select(m => { m.FieldDDL = FieldDDL; return m; }).ToList();
            }

            var FkList = oBO.BOFields.Where(m => m.FKiType > 0 && (!string.IsNullOrEmpty(m.FKTableName))).ToList();
            string sBODataSource = string.Empty;
            foreach (var item in FkList)
            {
                var sTableName = item.FKTableName;
                var BO = AllBOs.Where(m => m.TableName == sTableName).FirstOrDefault();
                sBODataSource = GetBODataSource(BO.iDataSource, UserDetails.FKiOrgID, sDatabase, UserDetails.sUserDatabase);
                List<VMDropDown> FKDDL = new List<VMDropDown>();
                using (SqlConnection Con = new SqlConnection(sBODataSource))
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    if (BO != null)
                    {
                        var LabelGroup = BO.BOGroups.Where(m => m.BOID == BO.BOID && m.GroupName.ToString() == ServiceConstants.LabelGroup.ToString()).FirstOrDefault().BOFieldNames;
                        if (!string.IsNullOrEmpty(LabelGroup))
                        {
                            cmd.CommandText = "Select " + LabelGroup + " from " + sTableName;
                        }
                    }
                    // cmd.CommandText = "select id,LocationName from "+sTableName+"";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader.FieldCount > 1)
                        {
                            FKDDL.Add(new VMDropDown
                            {
                                text = reader.IsDBNull(0) ? null : reader.GetValue(0).ToString(),
                                Expression = reader.IsDBNull(1) ? null : reader.GetValue(1).ToString()
                            });
                        }
                        else if (reader.FieldCount > 0)
                        {
                            FKDDL.Add(new VMDropDown
                            {
                                text = reader.IsDBNull(0) ? null : reader.GetValue(0).ToString(),
                            });
                        }

                    }
                    Con.Close();
                }
                oBO.BOFields.Where(m => m.ID == item.ID).FirstOrDefault().sFKBOSize = BO.sSize;
                oBO.BOFields.Where(m => m.ID == item.ID).Select(m => { m.FieldDDL = FKDDL; return m; }).ToList();
            }
            //var iOneClickID = oBO.BOFields.Where(m => m.iOneClickID > 0).ToList();
            //foreach (var ClickID in iOneClickID)
            //{
            //    var FieldDDL = new List<VMDropDown>();
            //    var iClickID = ClickID.iOneClickID;
            //    if (iClickID > 0)
            //    {
            //        dbContext = new ModelDbContext();
            //        var o1Click = dbContext.Reports.Find(iClickID);
            //        int iBOID = o1Click.BOID;
            //        var BODef = dbContext.BOs.Find(iBOID);
            //        sBODataSource = GetBODataSource(BODef.iDataSource, UserDetails.FKiOrgID, sDatabase, UserDetails.sUserDatabase);
            //        SqlConnection Con = new SqlConnection(sBODataSource);
            //        Con.Open();
            //        SqlCommand cmd = new SqlCommand();
            //        cmd.Connection = Con;
            //        cmd.CommandText = o1Click.Query;
            //        SqlDataReader reader = cmd.ExecuteReader();
            //        while (reader.Read())
            //        {
            //            FieldDDL.Add(new VMDropDown
            //            {
            //                ID = reader.GetInt32(0),
            //                Expression = reader.GetString(1)
            //            });
            //        }
            //        Con.Close();
            //        oBO.BOFields.Where(m => m.iOneClickID == iClickID).Select(m => { m.FieldDDL = FieldDDL; return m; }).ToList();
            //    }
            //}
            return oBO;
            //}
            //catch (Exception ex)
            //{
            //    logger.Error(ex);
            //    Common.SaveErrorLog(ex.ToString(), sDatabase);
            //    Common.SaveErrorLog(ex.StackTrace,sDatabase);
            //    return null;
            //}
        }

        public cBOInstance Get_BOInstance(string sBOName, string GroupName, int iUserID, string sOrgName, string sDatabase)
        {
            //try
            //    {
            cBOInstance oBOInstance = new cBOInstance();
            if (!string.IsNullOrEmpty(sBOName))
            {
                List<cNameValuePairs> lSaveFields = new List<cNameValuePairs>();
                var BODefinition = Get_BODefinition(sBOName, iUserID, sOrgName, sDatabase);
                if (!string.IsNullOrEmpty(GroupName))
                {
                    var sFields = BODefinition.BOGroups.Where(m => m.GroupName.ToLower() == GroupName.ToLower()).Select(m => m.BOFieldNames).FirstOrDefault();
                    lSaveFields = sFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList().Select(m => new cNameValuePairs { sName = m }).ToList();
                }
                if (BODefinition != null)
                {
                    oBOInstance.Definition = BODefinition;
                    if (!string.IsNullOrEmpty(GroupName))
                    {
                        lSaveFields.ToList().ForEach(m => m.bDirty = true);
                        oBOInstance.NVPairs = lSaveFields;
                    }
                    else
                    {
                        List<cNameValuePairs> Fields = BODefinition.BOFields.Select(m => new cNameValuePairs { sName = m.Name }).ToList();
                        Fields.Where(m => lSaveFields.Any(n => n.sName == m.sName)).ToList().ForEach(c => c.bDirty = true);
                        oBOInstance.NVPairs = Fields;
                    }
                }
            }
            return oBOInstance;
            //}
            //catch (Exception ex)
            //{
            //    logger.Error(ex);
            //    Common.SaveErrorLog(ex.ToString(), sDatabase);
            //    Common.SaveErrorLog(ex.StackTrace, sDatabase);
            //    return null;
            //}

        }

        public cBODisplay GetFormData(string BOName, string Group, int iInstanceID, string sVisualisation, int iUserID, string sOrgName, string sDatabase, List<cNameValuePairs> nWCParams)
        {
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetails.FKiOrgID;
            var sOrgDB = UserDetails.sUserDatabase;
            ModelDbContext dbContext = new ModelDbContext();
            DataContext Spdb = new DataContext(sOrgDB);
            cBODisplay oBODisplay = new cBODisplay();
            var sBODataSource = string.Empty;
            var oBOInstance = Get_BOInstance(BOName, Group, iUserID, sOrgName, sDatabase);//GroupName
            var GroupFields = oBOInstance.Definition.BOGroups.Where(m => m.GroupName.ToLower() == Group.ToLower()).Select(m => m.BOFieldNames).FirstOrDefault();
            string TableName = string.Empty;
            if (!string.IsNullOrEmpty(oBOInstance.Definition.TableName))
            {
                TableName = oBOInstance.Definition.TableName;
            }
            else
            {
                TableName = oBOInstance.Definition.Name;
            }
            List<string[]> Rows = new List<string[]>();
            string sWhereClause = string.Empty;
            var oBO = oBOInstance.Definition;
            int FKiStepInstanceID = 0;
            int FKiSectionInstanceID = 0;
            if (nWCParams != null && nWCParams.Count() > 0)
            {
                string sContext = nWCParams.Where(m => m.sName.ToLower() == "context".ToLower()).Select(m => m.sValue).FirstOrDefault();
                if (sContext.ToLower() == "QSStep".ToLower())
                {
                    FKiStepInstanceID = Convert.ToInt32(nWCParams.Where(m => m.sName.ToLower() == "FKiStepInstanceID".ToLower()).Select(m => m.sValue).FirstOrDefault());
                    if (FKiStepInstanceID > 0)
                    {
                        sWhereClause = sWhereClause + "FKiStepInstanceID = " + FKiStepInstanceID;
                    }

                }
                else if (sContext.ToLower() == "QSStepSection".ToLower())
                {
                    FKiSectionInstanceID = Convert.ToInt32(nWCParams.Where(m => m.sName.ToLower() == "FKiSectionInstanceID".ToLower()).Select(m => m.sValue).FirstOrDefault());
                    if (FKiSectionInstanceID > 0)
                    {
                        sWhereClause = sWhereClause + "FKiSectionInstanceID = " + FKiSectionInstanceID;
                    }
                }
            }
            if (sWhereClause.Length > 0)
            {
                sWhereClause = " Where " + sWhereClause;
            }
            //var Columns = new List<string>();
            if (iInstanceID > 0 || sWhereClause.Length > 0)
            {
                string sPrimaryKey = string.Empty;
                if (!string.IsNullOrEmpty(oBO.sPrimaryKey))
                {
                    sPrimaryKey = oBO.sPrimaryKey;
                }
                else
                {
                    sPrimaryKey = "id";
                }
                sBODataSource = GetBODataSource(oBO.iDataSource, UserDetails.FKiOrgID, sDatabase, sOrgDB);
                using (SqlConnection Con = new SqlConnection(sBODataSource))
                {
                    SqlCommand cmd = new SqlCommand("", Con);
                    var SelectQuery = "Select " + GroupFields;
                    var SelectPart = ServiceUtil.GetFKLabelGroup(oBO, null, SelectQuery, iUserID, sOrgName, sDatabase);
                    if (!string.IsNullOrEmpty(sWhereClause))
                    {
                        cmd.CommandText = SelectPart + " from " + TableName + sWhereClause;
                    }
                    else
                    {
                        cmd.CommandText = SelectPart + " from " + TableName + " Where " + sPrimaryKey + " = " + iInstanceID;
                    }
                    Con.Open();
                    //Con.ChangeDatabase(sOrgDB);
                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable data = new DataTable();
                    data.Load(reader);
                    Rows = data.Rows.Cast<DataRow>()
      .Select(row => data.Columns.Cast<DataColumn>()
         .Select(col => Convert.ToString(row[col]))
      .ToArray())
   .ToList();
                    Con.Dispose();
                }
                if (Rows.Count() > 0)
                {
                    var RowData = Rows[0];
                    var Fields = GroupFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    for (int i = 0; i < Fields.Count(); i++)
                    {
                        var Format = oBOInstance.Definition.BOFields.Where(m => m.Name.ToLower() == Fields[i].ToLower()).Select(m => m.Format).FirstOrDefault();
                        if (Format != null)
                        {
                            string sFormattedValue = string.Empty;
                            if (Group != "Save Group")
                            {
                                sFormattedValue = FormatValue(Fields[i], Rows[0][i], Format);
                            }
                            if (!string.IsNullOrEmpty(sFormattedValue))
                            {
                                oBOInstance.NVPairs.Where(m => m.sName.ToLower() == Fields[i].ToLower()).FirstOrDefault().sValue = sFormattedValue;
                            }
                            else
                            {
                                oBOInstance.NVPairs.Where(m => m.sName.ToLower() == Fields[i].ToLower()).FirstOrDefault().sValue = RowData[i];
                            }
                        }
                        else
                        {
                            oBOInstance.NVPairs.Where(m => m.sName.ToLower() == Fields[i].ToLower()).FirstOrDefault().sValue = RowData[i];
                        }

                        oBOInstance.NVPairs.Where(m => m.sName.ToLower() == Fields[i].ToLower()).FirstOrDefault().bDirty = true;
                    }
                }
            }
            //oBOInstance = RunGroupFieldsQuery(oBOInstance, iUserID, sOrgName, sDatabase);
            oBODisplay.BOInstance = oBOInstance;

            List<VMDropDown> ImagePathDetails = new List<VMDropDown>();
            var ImageData = oBO.BOFields.Where(m => m.FKiFileTypeID > 0).ToList();
            foreach (var img in ImageData)
            {
                foreach (var listdata in ImageData)
                {
                    var sName = listdata.Name;
                    var sNewValue = oBOInstance.NVPairs.Where(m => m.sName == sName).Select(m => m.sValue).FirstOrDefault();
                    if (!string.IsNullOrEmpty(sNewValue))
                    {
                        var sPairValues = sNewValue.Split(',').Select(Int32.Parse).ToList();
                        if (iOrgID > 0)
                        {
                            //var sDatabases = sOrgDB;
                            dbContext = new ModelDbContext(sDatabase);
                        }
                        ImagePathDetails = dbContext.XIDocs.Where(m => m.FKiDocType == img.FKiFileTypeID && sPairValues.Contains(m.ID)).ToList().Select(m => new VMDropDown { ID = m.ID, text = m.FileName, Type = m.SubDirectoryPath }).ToList();
                        foreach (var imges in ImagePathDetails)
                        {
                            var DocID = Convert.ToInt32(imges.ID);
                            var XIDoc = new XIDocs();
                            if (iOrgID > 0)
                            {
                                //var sDatabases = sOrgDB;
                                dbContext = new ModelDbContext(sDatabase);
                                XIDoc = dbContext.XIDocs.Find(DocID);
                            }
                            else
                            {
                                XIDoc = dbContext.XIDocs.Find(DocID);
                            }
                            dbContext = new ModelDbContext(sDatabase);
                            var FileFolder = dbContext.XIDocTypes.Where(m => m.ID == XIDoc.FKiDocType).Select(m => m.Type).FirstOrDefault();
                            var Path = ConfigurationManager.AppSettings["XIDocsPath"] + "//" + FileFolder + "//" + XIDoc.SubDirectoryPath + "//" + XIDoc.FileName;
                            imges.Expression = Path;
                        }
                    }
                }
                oBOInstance.Definition.BOFields.Where(m => m.ID == img.ID).Select(m => { m.ImagePathDetails = ImagePathDetails; return m; }).ToList();
            }
            return oBODisplay;
        }

        public string FormatValue(string sField, string sValue, string Format)
        {
            string sFormattedValue = string.Empty;
            var Char = sField.Select(c => char.IsUpper(c)).ToList();
            var Position = Char.IndexOf(true);
            if (Position == 1)
            {
                char FirstLetter = sField[0];
                if (FirstLetter == 'r')
                {
                    CultureInfo rgi = new CultureInfo(Format);
                    sFormattedValue = string.Format(rgi, "{0:c}", Convert.ToDecimal(sValue)).ToString();
                }
            }
            else if (Position == 2)
            {
                var Prefix = sField.Substring(0, 2);
                if (Prefix == "dt")
                {
                    sFormattedValue = String.Format("{0:" + Format + "}", Convert.ToDateTime(sValue));
                }
            }
            return sFormattedValue;
        }

        public cBOInstance RunGroupFieldsQuery(cBOInstance oBOInstance, int iUserID, string sOrgName, string sDatabase)
        {
            int iInstanceID = 0;
            string sTableName = string.Empty;
            var FKFields = oBOInstance.Definition.BOFields.Where(m => m.FKTableName != null && !string.IsNullOrEmpty(m.FKTableName)).ToList();
            foreach (var items in FKFields)
            {
                List<string[]> Rows = new List<string[]>();
                //var FKoBOInstance = Get_BOInstance(items.FKTableName, null, iUserID, sOrgName, sDatabase);
                if (!string.IsNullOrEmpty(items.FKTableName))
                {
                    var FKinstanceID = oBOInstance.NVPairs.Where(m => m.sName == items.Name).Select(m => m.sValue).FirstOrDefault();
                    if (!string.IsNullOrEmpty(FKinstanceID))
                    {
                        iInstanceID = Convert.ToInt32(FKinstanceID);
                    }
                    else
                    {
                        iInstanceID = 0;
                    }
                    var FKTablename = items.FKTableName;
                    if (iInstanceID > 0)
                    {
                        var sGroupValues = ResolveGroupFieldsWithValues("Label", iInstanceID, FKTablename, iUserID, sOrgName, sDatabase);
                        oBOInstance.NVPairs.Where(m => m.sName == items.Name).FirstOrDefault().sGroupValue = sGroupValues;
                    }
                }
            }
            return oBOInstance;
        }

        public string ResolveGroupFieldsWithValues(string sGroup, int iInstanceID, string FKTableName, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sBOName = dbContext.BOs.Where(m => m.TableName.ToLower() == FKTableName.ToLower()).Select(m => m.Name).FirstOrDefault();
            if (!string.IsNullOrEmpty(sBOName))
            {
                var oBODef = Get_BODefinition(sBOName, iUserID, sOrgName, sDatabase);
                var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
                var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
                var Group = oBODef.BOGroups.Where(m => m.GroupName.ToLower() == sGroup.ToLower()).FirstOrDefault();
                string sTableName = string.Empty;
                List<string[]> Rows = new List<string[]>();
                if (string.IsNullOrEmpty(oBODef.TableName))
                {
                    sTableName = oBODef.Name;
                }
                else
                {
                    sTableName = oBODef.TableName;
                }
                var sBODataSource = string.Empty;
                var oBO = oBODef;
                string sGroupValues = string.Empty;
                if (Group != null)
                {
                    sBODataSource = GetBODataSource(oBO.iDataSource, UserDetails.FKiOrgID, sDatabase, sOrgDB);
                    using (SqlConnection Con = new SqlConnection(sBODataSource))
                    {
                        SqlCommand cmd = new SqlCommand("", Con);
                        cmd.CommandText = "Select " + Group.BOFieldNames + " from " + sTableName + " Where ID=" + iInstanceID;
                        Con.Open();
                        //Con.ChangeDatabase(sDatabase);
                        SqlDataReader reader = cmd.ExecuteReader();
                        DataTable data = new DataTable();
                        data.Load(reader);
                        Rows = data.Rows.Cast<DataRow>()
          .Select(row => data.Columns.Cast<DataColumn>()
             .Select(col => Convert.ToString(row[col]))
          .ToArray())
        .ToList();
                        Con.Dispose();
                    }
                    var GroupFields = Group.BOFieldNames.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (Rows != null && Rows.Count() > 0)
                    {
                        for (int i = 0; i < GroupFields.Count(); i++)
                        {
                            var Def = oBODef.BOFields.Where(m => m.Name.ToLower() == GroupFields[i].ToLower()).FirstOrDefault();
                            if (Def.Format != null)
                            {
                                var sFormattedValue = FormatValue(GroupFields[i], Rows[0][i], Def.Format);
                                if (!string.IsNullOrEmpty(sFormattedValue))
                                {
                                    sGroupValues = sGroupValues + sFormattedValue + ", ";
                                }
                                else
                                {
                                    sGroupValues = sGroupValues + Rows[0][i] + ", ";
                                }
                            }
                            else
                            {
                                sGroupValues = sGroupValues + Rows[0][i] + ", ";
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(sGroupValues))
                {
                    sGroupValues = sGroupValues.Substring(0, sGroupValues.Length - 2);
                }
                return sGroupValues;
            }
            return null;
        }

        public cBOInstance SaveFormData(cBOInstance oBOInstance, string sGUID, string sContext, string sDatabase, int iUserID, string sOrgName)
        {
            string sActiveBO = string.Empty;
            string sActiveFK = string.Empty;
            string sSessionID = HttpContext.Current.Session.SessionID;
            var ISS = oCache.Get_ParamVal(sSessionID, sGUID, sContext, "|XIParent");
            CInstance parentparams = new CInstance();
            if (!string.IsNullOrEmpty(ISS))
            {
                parentparams = oCache.GetAllParamsUnderGUID(sSessionID, ISS, sContext);
            }
            else
            {
                parentparams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, sContext);
            }
            sActiveBO = parentparams.NMyInstance.Where(m => m.Key == "{XIP|ActiveBO}").Select(m => m.Value.sValue).FirstOrDefault();
            if (!string.IsNullOrEmpty(sActiveBO))
            {
                sActiveFK = oBOInstance.Definition.BOFields.Where(m => m.FKTableName == sActiveBO).Select(m => m.Name).FirstOrDefault();
            }
            if (!string.IsNullOrEmpty(sActiveFK))
            {
                var FKValue = parentparams.NMyInstance.Where(m => m.Key == "{XIP|" + sActiveBO + ".id}").Select(m => m.Value.sValue).FirstOrDefault();
                var ColExists = oBOInstance.NVPairs.Where(m => m.sName.ToLower() == sActiveFK.ToLower()).FirstOrDefault();
                if (ColExists == null)
                {
                    cNameValuePairs Pair = new cNameValuePairs();
                    Pair.sName = sActiveFK;
                    Pair.sValue = FKValue;
                    Pair.bDirty = true;
                    oBOInstance.NVPairs.Add(Pair);
                }
                else
                {
                    oBOInstance.NVPairs.Where(m => m.sName.ToLower() == sActiveFK.ToLower()).FirstOrDefault().sValue = FKValue;
                    oBOInstance.NVPairs.Where(m => m.sName.ToLower() == sActiveFK.ToLower()).FirstOrDefault().bDirty = true;
                }
            }
            foreach (var itesm in parentparams.NMyInstance)
            {
                if (itesm.Value.sType == "autoset")
                {
                    var ColExists = oBOInstance.NVPairs.Where(m => m.sName.ToLower() == itesm.Key.ToLower()).FirstOrDefault();
                    if (ColExists == null)
                    {
                        cNameValuePairs Pair = new cNameValuePairs();
                        Pair.sName = itesm.Key;
                        Pair.sValue = itesm.Value.sValue;
                        Pair.bDirty = true;
                        oBOInstance.NVPairs.Add(Pair);
                    }
                    else
                    {
                        oBOInstance.NVPairs.Where(m => m.sName.ToLower() == itesm.Key.ToLower()).FirstOrDefault().sValue = itesm.Value.sValue;
                        oBOInstance.NVPairs.Where(m => m.sName.ToLower() == itesm.Key.ToLower()).FirstOrDefault().bDirty = true;
                    }
                }
            }
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            //var oBOInstance = Get_BOInstance(sBOName, ServiceConstants.SaveGroup, sDatabase);//GroupName
            //var sResult = MapFormToInstance(Savedata, oBOInstance);
            var Response = Update_Data(oBOInstance, sDatabase, iUserID, sOrgName);
            return Response;
        }

        public cBOInstance Update_Data(cBOInstance oBOInstance, string sDatabase, int iUserID, string sOrgName)
        {
            VMCustomResponse oResponse = new VMCustomResponse();
            List<BOScripts> Scrpts = new List<BOScripts>();
            if (oBOInstance != null && oBOInstance.Definition.BOScripts.Where(m => m.sType.ToLower() == "prepersist" && m.StatusTypeID == 10).Count() > 0)
            {
                Scrpts = RunScript(oBOInstance.Definition.BOScripts.Where(m => m.sType.ToLower() == "prepersist" && m.StatusTypeID == 10).ToList(), oBOInstance);
            }
            //Add new record to respective table  
            //dbSet.Add(oBOInstance);

            if (Scrpts.Where(m => m.IsSuccess == false).Count() == 0)
            {
                oBOInstance = Save(oBOInstance, sDatabase, iUserID, sOrgName);
            }
            else
            {
                oBOInstance.Definition.BOScripts = Scrpts;
                return oBOInstance;
            }

            if (oBOInstance != null && oBOInstance.Definition.BOScripts.Where(m => m.sType.ToLower() == "postpersist" && m.StatusTypeID == 10).Count() > 0)
            {
                Scrpts = RunScript(oBOInstance.Definition.BOScripts.Where(m => m.sType.ToLower() == "postpersist" && m.StatusTypeID == 10).ToList(), oBOInstance);
            }
            return oBOInstance;
        }

        public List<BOScripts> RunScript(List<BOScripts> lScripts, cBOInstance oBOInstance)
        {
            List<XiParameters> oParams = new List<XiParameters>();
            foreach (var script in lScripts)
            {
                if (script.sLanguage.ToLower() == "regular expressions")
                {
                    string sRuntimescript = script.sScript;
                    var srtScritMerged = MergeScript(sRuntimescript, oParams, oBOInstance);
                    var sMergedScript = srtScritMerged[1];
                    //Example: [a-zA-Z0-9]{BO.sMob}
                    string sRegex = "";
                    var pattern = @"\<(.*?)\>";
                    var matches = Regex.Matches(sRuntimescript, pattern);
                    foreach (Match m in matches)
                    {
                        sRegex = m.ToString().Replace("<", "").Replace(">", "");
                    }
                    var sScrptRegex = new Regex(sRegex);
                    var sValue = sMergedScript.Split('>')[1];
                    if (sScrptRegex.IsMatch(sValue.ToString()))
                    {
                        //Valid
                        script.IsSuccess = true;
                    }
                    else
                    {
                        //Not Valid
                        script.IsSuccess = false;
                    }
                    script.sFieldName = srtScritMerged[0];
                }
            }
            return lScripts;
        }

        public List<string> MergeScript(string sRuntimescript, List<XiParameters> oParams, cBOInstance oBOInstance)
        {
            List<string> Field = new List<string>();
            Regex regex = new Regex(@"(?<=\{)[^}]*(?=\})", RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(sRuntimescript);
            if (matches.Count > 0)
            {
                foreach (var match in matches)
                {
                    if (match.ToString().IndexOf('.') > 0)
                    {
                        var sProperty = match.ToString();
                        var sName = match.ToString().Split('.')[1];
                        Field.Add(sName);
                        var sValue = oBOInstance.NVPairs.Where(m => m.sName.ToLower() == sName.ToLower()).Select(m => m.sValue).FirstOrDefault();
                        sRuntimescript = sRuntimescript.ToString().Replace("{" + sProperty + "}", sValue);
                        Field.Add(sRuntimescript);
                    }
                }
            }
            return Field;
        }

        public cBOInstance Save(cBOInstance oBOInstance, string sDatabase, int iUserID, string sOrgName)
        {
            //try
            //{
            string sOrgDB = string.Empty;
            ModelDbContext dbContext = new ModelDbContext();
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            sOrgDB = UserDetails.sUserDatabase;
            Common.SaveErrorLog("sOrgDB" + sOrgDB, "XIDNA");
            var Columns = oBOInstance.NVPairs.Where(m => m.bDirty == true).Select(m => m.sName).ToList();
            var Data = oBOInstance.NVPairs.Where(m => m.bDirty == true).Select(m => m.sValue).ToList();
            string sQuery = "";
            string sUpdateQuery = "";
            string sValues = string.Empty;
            string sLabelValue = "";
            using (IEnumerator<string>
    enumerator1 = Columns.GetEnumerator(),
    enumerator2 = Data.GetEnumerator())
            {
                while (enumerator1.MoveNext() && enumerator2.MoveNext())
                {
                    if (enumerator1.Current != "ID")
                    {
                        sLabelValue = enumerator1.Current;
                        var oBOField = oBOInstance.Definition.BOFields.Where(m => m.Name == sLabelValue).FirstOrDefault();
                        //sQuery = sQuery + enumerator1.Current + "='" + enumerator2.Current + "', ";
                        if (oBOField != null)
                        {
                            sQuery = sQuery + enumerator1.Current + ',';
                            if (oBOField.TypeID == 150 && !string.IsNullOrEmpty(enumerator2.Current))
                            {
                                sValues = sValues + "'" + enumerator2.Current + "'" + ',';
                                sUpdateQuery = sUpdateQuery + enumerator1.Current + "='" + enumerator2.Current + "'" + ',';
                            }
                            else
                            {
                                sValues = sValues + "'" + enumerator2.Current + "'" + ',';
                                sUpdateQuery = sUpdateQuery + enumerator1.Current + "='" + enumerator2.Current + "'" + ',';
                            }
                        }
                    }
                }
            }
            sQuery = sQuery.Substring(0, sQuery.Length - 1);
            sValues = sValues.Substring(0, sValues.Length - 1);
            sUpdateQuery = sUpdateQuery.Substring(0, sUpdateQuery.Length - 1);
            var sBODataSource = string.Empty;
            sBODataSource = GetBODataSource(oBOInstance.Definition.iDataSource, UserDetails.FKiOrgID, sDatabase, sOrgDB);
            Common.SaveErrorLog(Convert.ToString(oBOInstance.Definition.iDataSource) + Convert.ToString(UserDetails.FKiOrgID) + sDatabase + sOrgDB, "XIDNA");
            Common.SaveErrorLog(sBODataSource, "XIDNA");
            Common.SaveErrorLog("Query build successfully", "XIDNA");
            //Data = "'" + Data + "'";
            //SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNAClientDbContext"].ConnectionString);
            using (SqlConnection Conn = new SqlConnection(sBODataSource))
            {
                Conn.Open();
                SqlCommand SqlCmd = new SqlCommand();
                SqlCmd.Connection = Conn;
                var TableName = string.Empty;
                if (oBOInstance.Definition.TableName != null)
                {
                    TableName = oBOInstance.Definition.TableName;
                }
                else
                {
                    TableName = oBOInstance.Definition.Name;
                }
                if (oBOInstance.NVPairs.Where(m => m.sName.ToLower() == "ID".ToLower()).Select(m => m.sValue).FirstOrDefault() == null)
                {

                    //Conn.ChangeDatabase(sOrgDB);
                    //should be used for 192 Server
                    string cmdText = "INSERT INTO " + TableName + "(" + sQuery + ") output INSERTED.ID VALUES(" + sValues + ")";
                    Common.SaveErrorLog("cmdText:" + cmdText, "XIDNA");
                    //should be used for 88 Server
                    //sQuery = "id," + sQuery;
                    //sValues = "@max+1," + sValues;
                    //string cmdText = "declare @max int; set @max = (SELECT MAX(id) FROM " + TableName + "); INSERT INTO " + TableName + "(" + sQuery + ") output @max+1 VALUES(" + sValues + ")";
                    SqlCommand cmd = new SqlCommand(cmdText, Conn);
                    int iInstanceID = Convert.ToInt32(cmd.ExecuteScalar());
                    var IDPair = oBOInstance.NVPairs.Where(m => m.sName.ToLower() == "id").FirstOrDefault();
                    if (IDPair != null)
                    {
                        oBOInstance.NVPairs.Where(m => m.sName.ToLower() == "id").FirstOrDefault().sValue = iInstanceID.ToString();
                    }
                }
                else
                {
                    string cmdText = "UPDATE " + TableName + " SET" + " " + sUpdateQuery + " WHERE" + " ID=" + oBOInstance.NVPairs.Where(m => m.sName.ToLower() == "ID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                    SqlCommand cmd = new SqlCommand(cmdText, Conn);
                    cmd.ExecuteNonQuery();
                    //Spdb.Database.ExecuteSqlCommand("UPDATE " + TableName + " SET" + " " + sUpdateQuery + " WHERE" + " ID=" + oBOInstance.NVPairs.Where(m => m.sName == "ID").Select(m => m.sValue).FirstOrDefault());
                }
                Conn.Close();
            }
            //}
            //catch (Exception ex)
            //{
            //    logger.Error(ex);
            //    Common.SaveErrorLog(ex.ToString(), sDatabase);
            //    return null;
            //}
            return oBOInstance;
        }

        public List<cVisualisations> GetVisualistions(string sBOName, string sVisualisation, int iInstanceID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var sOrgDB = UserDetails.sUserDatabase;
            //if (UserDetails.FKiOrgID > 0)
            //{
            //    sDatabase = sOrgDB;
            //}
            ModelDbContext dbContext = new ModelDbContext();
            var sBODataSource = string.Empty;
            var oBOInstance = Get_BOInstance(sBOName, "", iUserID, sOrgName, sDatabase);
            var oBO = oBOInstance.Definition;
            string sTableName = string.Empty;
            List<cVisualisations> lVisualisations = new List<cVisualisations>();
            var oVisualisations = dbContext.XiVisualisations.Where(m => m.Name.Equals(sVisualisation, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            foreach (var items in oVisualisations.XiVisualisationLists)
            {
                cVisualisations oVisual = new cVisualisations();
                oVisual.sAttribute = items.XiVisualisationListNVs.Where(m => m.sName.ToLower() == "Attributes".ToLower()).Select(m => m.sValue).FirstOrDefault();
                oVisual.sHelpText = items.XiVisualisationListNVs.Where(m => m.sName.ToLower() == "Help Text".ToLower()).Select(m => m.sValue).FirstOrDefault();
                oVisual.iDrillDownXiLinkID = Convert.ToInt32(items.XiVisualisationListNVs.Where(m => m.sName.ToLower() == "Drilldown XiLink".ToLower()).Select(m => m.sValue).FirstOrDefault());
                oVisual.sPreviewGroup = items.XiVisualisationListNVs.Where(m => m.sName.ToLower() == "Resolve Group".ToLower()).Select(m => m.sValue).FirstOrDefault();
                List<string[]> Rows = new List<string[]>();
                if (string.IsNullOrEmpty(oBOInstance.Definition.TableName))
                {
                    sTableName = oBOInstance.Definition.Name;
                }
                else
                {
                    sTableName = oBOInstance.Definition.TableName;
                }
                sBODataSource = GetBODataSource(oBO.iDataSource, UserDetails.FKiOrgID, sDatabase, sOrgDB);
                using (SqlConnection Con = new SqlConnection(sBODataSource))
                {
                    SqlCommand cmd = new SqlCommand("", Con);
                    cmd.CommandText = "Select " + oVisual.sAttribute + " from " + sTableName + " Where ID=" + iInstanceID;
                    Con.Open();
                    //Con.ChangeDatabase(sDatabase);
                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable data = new DataTable();
                    data.Load(reader);
                    Rows = data.Rows.Cast<DataRow>()
      .Select(row => data.Columns.Cast<DataColumn>()
         .Select(col => Convert.ToString(row[col]))
      .ToArray())
    .ToList();
                    Con.Dispose();
                }

                int FKiInstanceID = 0;
                if (Rows[0].Count() > 0)
                {
                    FKiInstanceID = Convert.ToInt32(Rows[0][0]);
                }

                if (!string.IsNullOrEmpty(oVisual.sPreviewGroup))
                {
                    if (oVisual.sAttribute.Substring(0, 2).ToLower() == "fk")
                    {
                        var FKField = oBOInstance.Definition.BOFields.Where(m => m.Name == oVisual.sAttribute).Select(m => m.FKTableName).FirstOrDefault();
                        var FKoBOInstance = Get_BOInstance(FKField, null, iUserID, sOrgName, sDatabase);
                        oVisual.sPreviewData = ResolveGroupFieldsWithValues(oVisual.sPreviewGroup, FKiInstanceID, FKField, iUserID, sOrgName, sDatabase);
                        if (!string.IsNullOrEmpty(oVisual.sPreviewData))
                        {
                            oVisual.sPreviewData = "<table><tr><td>" + oVisual.sPreviewData.Replace(",", "</td></tr><tr><td>") + "</td></tr></table>";
                        }
                    }
                    else
                    {
                        oVisual.sPreviewData = ResolveGroupFieldsWithValues(oVisual.sPreviewGroup, FKiInstanceID, oBOInstance.Definition.TableName, iUserID, sOrgName, sDatabase);
                    }
                }
                oVisual.sResolveGroup = items.XiVisualisationListNVs.Where(m => m.sName.ToLower() == "Preview Group".ToLower()).Select(m => m.sValue).FirstOrDefault();
                oVisual.sNarrowBar = items.XiVisualisationListNVs.Where(m => m.sName.ToLower() == "Narrow Bar".ToLower()).Select(m => m.sValue).FirstOrDefault();
                lVisualisations.Add(oVisual);
            }
            return lVisualisations;
        }

        public cBOInstance GetListHover(int iInstanceID, int BOID, string BOName, string ColumnName, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            var oBO = dbContext.BOs.Find(BOID);
            var sBOName = string.Empty;
            sBOName = oBO.Name;
            cBOInstance oBOInstance = Get_BOInstance(sBOName, null, iUserID, sOrgName, sDatabase);
            var sGroupResult = new List<cNameValuePairs>();
            if (ColumnName.ToLower() == "id")
            {
                sGroupResult = ResolveGroupFieldsWithNameValuePairs(ServiceConstants.SummaryGroup.ToLower(), iInstanceID, oBOInstance, iUserID, sOrgName, sDatabase);
            }
            else
            {
                var FKBO = oBOInstance.Definition.BOFields.Where(m => m.LabelName == ColumnName).Select(m => m.FKTableName).FirstOrDefault();
                if (!string.IsNullOrEmpty(FKBO))
                {
                    oBOInstance = Get_BOInstance(FKBO, null, iUserID, sOrgName, sDatabase);
                }
                sGroupResult = ResolveGroupFieldsWithNameValuePairs(ServiceConstants.LabelGroup.ToLower(), iInstanceID, oBOInstance, iUserID, sOrgName, sDatabase);
            }

            foreach (var items in sGroupResult)
            {
                var Def = oBOInstance.Definition.BOFields.Where(m => m.Name == items.sName).FirstOrDefault();
                if (Def.Format != null)
                {
                    var sFormattedValue = FormatValue(items.sName, items.sValue, Def.Format);
                    if (!string.IsNullOrEmpty(sFormattedValue))
                    {
                        items.sValue = sFormattedValue;
                    }
                }
                else if (Def.FKiFileTypeID > 0)
                {
                    var ItemFields = items.sValue.Split(',').ToList();
                    var FieldValue = "";
                    for (int i = 0; i < ItemFields.Count(); i++)
                    {
                        FieldValue = ItemFields[i];
                        if (!string.IsNullOrEmpty(FieldValue) && FieldValue.Length > 0 && Convert.ToInt32(FieldValue) > 0)
                        {
                            var DocID = Convert.ToInt32(FieldValue);
                            if (DocID > 0)
                            {
                                if (iOrgID > 0)
                                {
                                    var sNewDatabase = sOrgDB;
                                    dbContext = new ModelDbContext(sNewDatabase);
                                    var XIDoc = dbContext.XIDocs.Find(DocID);
                                    if (XIDoc != null)
                                    {
                                        var FileFolder = dbContext.XIDocTypes.Where(m => m.ID == XIDoc.FKiDocType).Select(m => m.Type).FirstOrDefault();
                                        var Path = ConfigurationManager.AppSettings["XIDocsPath"] + "//" + FileFolder + "//" + XIDoc.SubDirectoryPath + "//" + XIDoc.FileName;
                                        items.sValue = Path;
                                    }
                                    else
                                    {
                                        items.sValue = ConfigurationManager.AppSettings["NoImagePath"];
                                    }
                                }
                                else
                                {
                                    var XIDoc = dbContext.XIDocs.Find(DocID);
                                    if (XIDoc != null)
                                    {
                                        var FileFolder = dbContext.XIDocTypes.Where(m => m.ID == XIDoc.FKiDocType).Select(m => m.Type).FirstOrDefault();
                                        var Path = ConfigurationManager.AppSettings["XIDocsPath"] + "//" + FileFolder + "//" + XIDoc.SubDirectoryPath + "//" + XIDoc.FileName;
                                        items.sValue = Path;
                                    }
                                    else
                                    {
                                        items.sValue = ConfigurationManager.AppSettings["NoImagePath"];
                                    }
                                }
                            }
                        }
                    }
                }
            }
            oBOInstance.NVPairs = sGroupResult;
            oBOInstance.NVPairs.ToList().ForEach(m => m.sName = oBOInstance.Definition.BOFields.Where(n => n.Name == m.sName).FirstOrDefault().LabelName);
            return oBOInstance;
        }

        private List<cNameValuePairs> ResolveGroupFieldsWithNameValuePairs(string sGroup, int iInstanceID, cBOInstance oBOInstance, int iUserID, string sOrgName, string sDatabase)
        {
            var sOrgDb = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            List<cNameValuePairs> NVPairs = new List<cNameValuePairs>();
            var Group = oBOInstance.Definition.BOGroups.Where(m => m.GroupName.ToLower() == sGroup).FirstOrDefault();
            if (Group != null)
            {
                string sTableName = string.Empty;
                List<string[]> Rows = new List<string[]>();
                if (string.IsNullOrEmpty(oBOInstance.Definition.TableName))
                {
                    sTableName = oBOInstance.Definition.Name;
                }
                else
                {
                    sTableName = oBOInstance.Definition.TableName;
                }
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("", Con);
                    cmd.CommandText = "Select " + Group.BOFieldNames + " from " + sTableName + " Where ID=" + iInstanceID;
                    Con.Open();
                    Con.ChangeDatabase(sOrgDb);
                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable data = new DataTable();
                    data.Load(reader);
                    Rows = data.Rows.Cast<DataRow>()
      .Select(row => data.Columns.Cast<DataColumn>()
         .Select(col => Convert.ToString(row[col]))
      .ToArray())
    .ToList();
                    Con.Dispose();
                    var GroupFields = Group.BOFieldNames.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (Rows.Count() > 0)
                    {
                        for (int i = 0; i < GroupFields.Count(); i++)
                        {
                            cNameValuePairs oNV = new cNameValuePairs();
                            oNV.sName = GroupFields[i];
                            oNV.sValue = Rows[0][i];
                            NVPairs.Add(oNV);
                        }
                    }
                }
            }
            return NVPairs;
        }

        public string GetFKTableName(string sFKey, string sDatabase)
        {
            if (!string.IsNullOrEmpty(sFKey))
            {
                sFKey = sFKey.ToLower().Replace("fk", "");
                string sName = sFKey;
                var Char = sName.Select(c => char.IsUpper(c)).ToList();
                char FirstLetter = sName[0];
                if (FirstLetter == 'i')
                {
                    sFKey = sFKey.Substring(1, sFKey.Length - 3);
                }
                var BO = Get_BODefinition(sFKey, 0, null, sDatabase);
                if (BO != null)
                {
                    return BO.Name;
                }
            }
            return sFKey;
        }

        public string Get_ParamVal(string sSessionID, string sUID, string sContext, string sParamName)
        {
            //CInstance oCache = Get_XICache();
            var sParamValue = oCache.Get_ParamVal(sSessionID, sUID, sContext, sParamName);
            return sParamValue;
        }

        public CInstance Get_Paramobject(string sSessionID, string sUID, string sContext, string sParamName)
        {
            //CInstance oCache = Get_XICache();
            var sParamValue = oCache.Get_Paramobject(sSessionID, sUID, sContext, sParamName);
            return sParamValue;
        }

        public string Set_ParamVal(string sSessionID, string sUID, string sParamName, string sParamValue)
        {
            CInstance oCache = Get_XICache();
            if (sUID != null && sUID.Length > 0)
            {
                oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).sValue = sParamValue;
            }
            return "TRUE";
        }

        //Demo to check whether script can be added.
        public string Set_ScriptVal(string sSessionID, string sUID, string sParamName, MethodInfo sParamValue)
        {
            CInstance oCache = Get_XICache();
            if (sUID != null && sUID.Length > 0)
            {
                oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).Script = sParamValue;
            }
            return "TRUE";
        }

        //Demo Get the script added
        public MethodInfo Get_ScriptVal(string sSessionID, string sUID, string sParamName)
        {
            CInstance oCache = Get_XICache();
            MethodInfo oMethodInfo = null;
            if (sUID != null)
            {
                oMethodInfo = oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance(sParamName).Script;
            }
            //if (sParentUID != "") {oCache.NInstance("XISession").NInstance("SS_" + sSessionID).NInstance("UID_" + sUID).NInstance("|XIParent").sValue=sParentUID }
            return oMethodInfo;
        }

        private CInstance Get_XICache()
        {
            object obj;
            CInstance oCacheobj = new CInstance();
            if (HttpRuntime.Cache["XICache"] == null)
            {
                CInstance oCache = new CInstance();
                HttpRuntime.Cache.Add("XICache", oCache, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                //oCache = HttpRuntime.Cache["XICache"];
            }
            else
            {
                obj = HttpRuntime.Cache["XICache"];
                oCacheobj = (CInstance)obj;
                return oCacheobj;
            }
            Get_XICache();
            return oCacheobj;
        }

        public cBOInstance Load_BOInstance(int iBOID, int iInstanceID, string sBOGroup, string sActionName, int iUserID, string sOrgName)
        {
            List<string[]> Rows = new List<string[]>();
            CommonRepository Common = new CommonRepository();
            //ModelDbContext dbContext = new ModelDbContext(DevDB);
            ModelDbContext dbContext = new ModelDbContext();
            var BO = dbContext.BOs.Where(m => m.BOID == iBOID).FirstOrDefault();
            var BoFields = BO.BOFields;
            var oBOInstance = Get_BOInstance(BO.Name, sBOGroup, iUserID, sOrgName, DevDB);//GroupName 
            var Group = oBOInstance.Definition.BOGroups.Where(m => m.GroupName == sBOGroup).FirstOrDefault();
            using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("", Con);
                cmd.CommandText = "Select " + Group.BOFieldNames + " from " + BO.TableName + " Where ID=" + iInstanceID;
                Con.Open();
                Con.ChangeDatabase(SharedDB);
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable data = new DataTable();
                data.Load(reader);
                Rows = data.Rows.Cast<DataRow>()
  .Select(row => data.Columns.Cast<DataColumn>()
     .Select(col => Convert.ToString(row[col]))
  .ToArray())
.ToList();
                Con.Dispose();
            }
            var lFields = new List<string>();
            lFields = Group.BOFieldNames.Split(',').ToList();
            Dictionary<string, string> dMyFields = new Dictionary<string, string>();
            for (var i = 0; i < lFields.Count(); i++)
            {
                dMyFields.Add(lFields[i].TrimStart().TrimEnd(), Rows[0][i]);
            }
            for (int i = 0; i < lFields.Count(); i++)
            {
                var sLabel = lFields[i].TrimStart().TrimEnd();
                var sValues = dMyFields[sLabel].ToString();
                oBOInstance.NVPairs.Where(m => m.sName == sLabel).Where(m => m.bDirty == true).FirstOrDefault().sValue = sValues;
                //set bDirty=false as its to be set only
                oBOInstance.NVPairs.Where(m => m.sName == sLabel).FirstOrDefault().bDirty = false;
            }
            return oBOInstance;
        }

        //Save to DB
        public string UpdateBO(cBOInstance oBOInstance)
        {
            ModelDbContext dbContext = new ModelDbContext(DevDB);
            DataContext Spdb = new DataContext(SharedDB);
            var Columns = oBOInstance.NVPairs.Where(m => m.bDirty == true).Select(m => m.sName).ToList();
            var Data = oBOInstance.NVPairs.Where(m => m.bDirty == true).Select(m => m.sValue).ToList();
            string sQuery = "";
            using (IEnumerator<string>
    enumerator1 = Columns.GetEnumerator(),
    enumerator2 = Data.GetEnumerator())
            {
                while (enumerator1.MoveNext() && enumerator2.MoveNext())
                {
                    if (enumerator1.Current != "ID")
                    {
                        sQuery = sQuery + enumerator1.Current + "='" + enumerator2.Current + "', ";
                    }
                }
            }
            sQuery = sQuery.Substring(0, sQuery.Length - 2);
            var TableName = string.Empty;
            if (oBOInstance.Definition.TableName != null)
            {
                TableName = oBOInstance.Definition.TableName;
            }
            else
            {
                TableName = oBOInstance.Definition.Name;
            }
            Spdb.Database.ExecuteSqlCommand("UPDATE " + TableName + " SET" + " " + sQuery + " WHERE" + " ID=" + oBOInstance.NVPairs.Where(m => m.sName == "ID").Select(m => m.sValue).FirstOrDefault());
            return null;
        }

        public string GetBODataSource(int iDataSourceID, int iOrgID, string sDatabase, string sOrgDB)
        {
            ModelDbContext XIContext = new ModelDbContext();
            var sBODataSource = string.Empty;
            if (iDataSourceID != 0)
            {
                if (iDataSourceID == -1 || iDataSourceID == 4083)
                {
                    sBODataSource = ModelDbContext.ConnectionString(sDatabase);
                }
                else if (iDataSourceID == -2 || iDataSourceID == 4082)
                {
                    sBODataSource = ModelDbContext.ConnectionString(sOrgDB);
                }
                else if (iDataSourceID == 4081)
                {
                    sBODataSource = ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString;// ModelDbContext.ConnectionString(sOrgDB);
                }
                else
                {
                    var SrcID = iDataSourceID;
                    var DataSource = XIContext.XIDataSources.Find(SrcID);
                    if (DataSource != null)
                    {
                        sBODataSource = DecryptData(DataSource.sConnectionString, true, DataSource.ID.ToString());
                    }

                }
            }
            else
            {
                if (iOrgID > 0)
                {
                    sBODataSource = ModelDbContext.ConnectionString(sOrgDB);
                }
                else
                {
                    sBODataSource = ModelDbContext.ConnectionString(sDatabase);
                }
            }
            return sBODataSource;
        }
        public string GetXIGUIDBODataSource(string iDataSourceID, int iOrgID, string sDatabase, string sOrgDB, int sApplicationID)
        {
            ModelDbContext XIContext = new ModelDbContext();
            var sBODataSource = string.Empty;
            Guid DataSourceGuid = new Guid();
            if (!string.IsNullOrEmpty(iDataSourceID))
            {
                if (iDataSourceID == "-1")
                {
                    sBODataSource = ModelDbContext.ConnectionString(sDatabase);
                }
                else if (iDataSourceID == "-2")
                {
                    sBODataSource = ModelDbContext.ConnectionString(sOrgDB);
                }
                else
                {
                    var SrcID = iDataSourceID;
                    if (Guid.TryParse(iDataSourceID, out DataSourceGuid))
                    {
                        sBODataSource = GetBODataSource(DataSourceGuid.ToString(), sApplicationID);
                        //sBODataSource = GetBODataSource(DataSourceGuid.ToString(), sApplicationID);
                        //var DataSource = XIContext.XIDataSources.Where(k => k.XIGUID == DataSourceGuid).FirstOrDefault();
                        //if (DataSource != null)
                        //{
                        //    sBODataSource = DecryptData(DataSource.sConnectionString, true, DataSourceGuid.ToString());
                        //}
                    }
                    else
                    {
                        var DataSource = XIContext.XIDataSources.Find(iDataSourceID);
                        if (DataSource != null)
                        {
                            sBODataSource = DecryptData(DataSource.sConnectionString, true, iDataSourceID);
                        }
                    }

                }
            }
            else
            {
                if (iOrgID > 0)
                {
                    sBODataSource = ModelDbContext.ConnectionString(sOrgDB);
                }
                else
                {
                    sBODataSource = ModelDbContext.ConnectionString(sDatabase);
                }
            }
            return sBODataSource;
        }
        public string GetBODataSource(string iDataSourceID, int iApplicationID, string sType = "")
        {
            int DataSourceID = 0;
            Guid DataSourceXIGUID = Guid.Empty;
            int.TryParse(iDataSourceID, out DataSourceID);
            Guid.TryParse(iDataSourceID, out DataSourceXIGUID);
            var sBODataSource = string.Empty;
            string[] encstring;
            var SelectQuery = "";
            if (DataSourceXIGUID != null && DataSourceXIGUID != Guid.Empty)
            {
                SelectQuery = "Select sConnectionString from XIDataSource_XID_T where XIGUID='" + DataSourceXIGUID.ToString() + "'";
            }
            else
            {
                SelectQuery = "Select sConnectionString from XIDataSource_XID_T where ID='" + DataSourceID.ToString() + "'";
            }
            sBODataSource = ConfigurationManager.ConnectionStrings["XIEnvironmentDbContext"].ConnectionString;
            using (SqlConnection Con = new SqlConnection(sBODataSource))
            {
                SqlCommand cmd = new SqlCommand("", Con);
                cmd.CommandText = SelectQuery;
                Con.Open();
                //Con.ChangeDatabase(sOrgDB);
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable data = new DataTable();
                data.Load(reader);
                encstring = data.Rows.Cast<DataRow>()
  .Select(row => data.Columns.Cast<DataColumn>()
     .Select(col => Convert.ToString(row[col]))
  .ToArray()).FirstOrDefault();
                Con.Dispose();
            }
            //XIInfraCache oCache = new XIInfraCache();
            //var SrcID = iDataSourceID.ToString();
            //XIInfraEncryption oEncrypt = new XIInfraEncryption();

            //var DataSource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, null, SrcID.ToString(), null, null, iApplicationID); //Connection.Select<XIDataSource>("XIDataSource_XID_T", Params).FirstOrDefault();// XIContext.XIDataSources.Find(SrcID);
            //sBODataSource = DecryptData(DataSource.sConnectionString, true, DataSource.XIGUID.ToString());
            sBODataSource = DecryptData(encstring[0].ToString(), true, iDataSourceID);
        
            return sBODataSource;
        }

    public string GetBONameAttributeValue(string sBOName, int iBOIID, int iUserID, string sDatabase)
    {
        var UserDetails = Common.GetUserDetails(iUserID, null, sDatabase);
        var oBODef = Get_BODefinition(sBOName, iUserID, null, sDatabase);
        var sNameAttribute = oBODef.sNameAttribute;
        List<string[]> Rows = new List<string[]>();
        if (!string.IsNullOrEmpty(sNameAttribute))
        {
            string sBODataSource = GetBODataSource(oBODef.iDataSource, UserDetails.FKiOrgID, sDatabase, UserDetails.sUserDatabase);
            using (SqlConnection Con = new SqlConnection(sBODataSource))
            {
                SqlCommand cmd = new SqlCommand("", Con);
                var SelectQuery = "Select " + sNameAttribute + " from " + oBODef.TableName + " where " + oBODef.sPrimaryKey + " = " + iBOIID;
                cmd.CommandText = SelectQuery;
                Con.Open();
                //Con.ChangeDatabase(sOrgDB);
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable data = new DataTable();
                data.Load(reader);
                Rows = data.Rows.Cast<DataRow>()
  .Select(row => data.Columns.Cast<DataColumn>()
     .Select(col => Convert.ToString(row[col]))
  .ToArray())
.ToList();
                Con.Dispose();
            }
        }
        if (Rows.Count() > 0)
        {
            return Rows[0][0];
        }
        return null;
    }

    public string GetBOGroupFields(string sBOName, string sGroupName, int iUserID, string sDatabase)
    {
        var GroupFields = string.Empty;
        var UserDetails = Common.GetUserDetails(iUserID, null, sDatabase);
        var oBODef = Get_BODefinition(sBOName, iUserID, null, sDatabase);
        var Group = oBODef.BOGroups.Where(m => m.GroupName.ToLower() == sGroupName.ToLower()).FirstOrDefault();
        if (Group != null)
        {
            GroupFields = Group.BOFieldNames;
            return GroupFields;
        }
        return GroupFields;
    }

    #endregion BOMethods

    #region QuestionSetMethods
    public cQSInstance Get_QuestionSetCache(string ObjName, string sGUID, int ID)
    {
        object obj;
        cQSInstance oCacheobj = new cQSInstance();
        if (HttpRuntime.Cache[ObjName + "_" + sGUID + "_" + ID] == null)
        {
            HttpRuntime.Cache.Add(ObjName + "_" + sGUID + "_" + ID, oCacheobj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
        }
        else
        {
            obj = HttpRuntime.Cache[ObjName + "_" + sGUID + "_" + ID];
            oCacheobj = (cQSInstance)obj;
            return oCacheobj;
        }
        Get_QuestionSetCache(ObjName, sGUID, ID);
        return oCacheobj;
    }
    public cQSInstance Set_QuestionSetCache(string ObjName, string sGUID, int ID, cQSInstance oCacheobj)
    {
        object obj;
        //if (HttpRuntime.Cache[ObjName + "_" + sGUID + "_" + ID] == null)
        //{
        HttpRuntime.Cache.Remove(ObjName + "_" + sGUID + "_" + ID);
        obj = HttpRuntime.Cache[ObjName + "_" + sGUID + "_" + ID];
        HttpRuntime.Cache.Add(ObjName + "_" + sGUID + "_" + ID, oCacheobj, null, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
        //}
        //else
        //{
        //    obj = HttpRuntime.Cache[ObjName + "_" + sGUID + "_" + ID];
        //    oCacheobj = (cQSInstance)obj;
        //    return oCacheobj;
        //}
        Get_QuestionSetCache(ObjName, sGUID, ID);
        return oCacheobj;
    }

    public string GetArea(string PostCode)
    {
        DevDB = ConfigurationManager.AppSettings["CoreDataBase"];
        Common.SaveErrorLog("Devdb:" + DevDB, "XIDNA");
        var postcode = PostCode.Substring(0, 5);
        ModelDbContext dbContext = new ModelDbContext(DevDB);
        string Group = dbContext.PostCodeLookUp.Where(x => x.sMPostCode.ToLower() == postcode.ToLower()).Select(s => s.Group).FirstOrDefault();
        return Group.Trim();
    }
    public static string GetQuestionSetKeyValue(cQSInstance QSInstance, string value)
    {
        string[] Agelist = value.Split('.');
        string result = "";
        var SectionInstances = QSInstance.nStepInstances.Where(x => x.ID == Convert.ToInt32(Agelist[0])).Select(x => x.nSectionInstances);
        var Fieldinstances = new List<cFieldInstance>();
        foreach (var instance in SectionInstances)
        {
            var list = instance.Where(x => x.ID == Convert.ToInt32(Agelist[1])).Select(x => x.nFieldInstances).ToList();
            if (list.Count != 0)
            {
                foreach (var instances in list)
                {
                    result = instances.Where(x => x.FKiFieldDefinitionID == Convert.ToInt32(Agelist[2])).Select(x => x.sValue).FirstOrDefault();
                }
            }
            else
            {
                var FieldInstance = QSInstance.nStepInstances.Where(x => x.ID == Convert.ToInt32(Agelist[0])).Select(x => x.nFieldInstances).ToList();
                foreach (var Finstance in FieldInstance)
                {
                    result = Finstance.Where(x => x.FKiFieldDefinitionID == Convert.ToInt32(Agelist[2])).Select(x => x.sValue).FirstOrDefault();
                }
            }
        }
        return result;
    }
    #endregion QuestionSetMethods

    #region Encrypt/Decrypt

    public string EncryptData(string toEncrypt, bool useHashing, string Key)
    {
        byte[] keyArray;
        byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

        System.Configuration.AppSettingsReader settingsReader =
                                            new AppSettingsReader();
        // Get the key from config file

        string key = Key;
        //System.Windows.Forms.MessageBox.Show(key);
        //If hashing use get hashcode regards to your key
        if (useHashing)
        {
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            //Always release the resources and flush data
            // of the Cryptographic service provide. Best Practice

            hashmd5.Clear();
        }
        else
            keyArray = UTF8Encoding.UTF8.GetBytes(key);

        TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
        //set the secret key for the tripleDES algorithm
        tdes.Key = keyArray;
        //mode of operation. there are other 4 modes.
        //We choose ECB(Electronic code Book)
        tdes.Mode = CipherMode.ECB;
        //padding mode(if any extra byte added)

        tdes.Padding = PaddingMode.PKCS7;

        ICryptoTransform cTransform = tdes.CreateEncryptor();
        //transform the specified region of bytes array to resultArray
        byte[] resultArray =
          cTransform.TransformFinalBlock(toEncryptArray, 0,
          toEncryptArray.Length);
        //Release resources held by TripleDes Encryptor
        tdes.Clear();
        //Return the encrypted data into unreadable string format
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }

    public string DecryptData(string cipherString, bool useHashing, string Key)
    {
        byte[] keyArray;
        //get the byte code of the string

        byte[] toEncryptArray = Convert.FromBase64String(cipherString);

        System.Configuration.AppSettingsReader settingsReader =
                                            new AppSettingsReader();
        //Get your key from config file to open the lock!
        string key = Key;

        if (useHashing)
        {
            //if hashing was used get the hash code with regards to your key
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            //release any resource held by the MD5CryptoServiceProvider

            hashmd5.Clear();
        }
        else
        {
            //if hashing was not implemented get the byte code of the key
            keyArray = UTF8Encoding.UTF8.GetBytes(key);
        }

        TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
        //set the secret key for the tripleDES algorithm
        tdes.Key = keyArray;
        //mode of operation. there are other 4 modes. 
        //We choose ECB(Electronic code Book)

        tdes.Mode = CipherMode.ECB;
        //padding mode(if any extra byte added)
        tdes.Padding = PaddingMode.PKCS7;

        ICryptoTransform cTransform = tdes.CreateDecryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(
                             toEncryptArray, 0, toEncryptArray.Length);
        //Release resources held by TripleDes Encryptor                
        tdes.Clear();
        //return the Clear decrypted TEXT
        return UTF8Encoding.UTF8.GetString(resultArray);
    }

    #endregion Encrypt/Decrypt
    #region UserCreation
    private static Random random = new Random();
    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    public cXIAppUsers CreateUserAccount(cQSInstance oQSInstance, string sDataBase)
    {
        Common.SaveErrorLog(sDataBase, "XIDNA");
        ModelDbContext dbContext = new ModelDbContext(sDataBase);

        string sFirstNameQSNotations = "QS(iMyQSID).StepName('Your Details').XIField('sForeName').sValue";
        string FirstName = GetQuestionSetParamValue(oQSInstance, sFirstNameQSNotations);
        string sLastNameQSNotations = "QS(iMyQSID).StepName('Your Details').XIField('sLastName').sValue";
        string LastName = GetQuestionSetParamValue(oQSInstance, sLastNameQSNotations);
        string sEmailQSNotations = "QS(iMyQSID).StepName('Your Details').XIField('sEmail').sValue";
        string sEmail = GetQuestionSetParamValue(oQSInstance, sEmailQSNotations);
        string sMobQSNotations = "QS(iMyQSID).StepName('Your Details').XIField('sMob').sValue";
        string sMob = GetQuestionSetParamValue(oQSInstance, sMobQSNotations);
        Common.SaveErrorLog("query executed successfully", "XIDNA");
        //Create Login for newly created Application
        cXIAppUsers oUser = new cXIAppUsers();
        oUser = dbContext.XIAppUsers.Where(s => s.sEmail == sEmail).FirstOrDefault();
        Common.SaveErrorLog("oUser:" + oUser, "XIDNA");
        //Create Login for newly created Application
        if (oUser == null)
        {
            oUser = new cXIAppUsers();
            oUser.IsUserExist = false;
            oUser.sUserName = sEmail;
            oUser.sEmail = sEmail;
            oUser.sFirstName = FirstName;
            oUser.sLastName = LastName;
            oUser.FKiOrganisationID = oQSInstance.QSDefinition.FKiOrganisationID;
            oUser.FKiApplicationID = oQSInstance.QSDefinition.FKiApplicationID;
            //oUser.sDatabaseName = sDatabase;
            oUser.sDatabaseName = dbContext.Organization.Find(oQSInstance.QSDefinition.FKiOrganisationID).DatabaseName;
            oUser.sCoreDatabaseName = sDataBase;
            oUser.sPhoneNumber = sMob;
            oUser.iReportTo = 0;
            oUser.LockoutEndDateUtc = DateTime.Now;
            oUser.sLocation = "Test";
            oUser.iPaginationCount = 10;
            oUser.sMenu = "Open,Open";
            oUser.iInboxRefreshTime = 0;
            dbContext.XIAppUsers.Add(oUser);
            dbContext.SaveChanges();
            string sTemporaryPWD = RandomString(8);
            //Users = dbcontext.ClientUsers.Find(Users.UserID);
            var EncryptedPwd = EncryptData(sTemporaryPWD, true, oUser.UserID.ToString());
            oUser = dbContext.XIAppUsers.Find(oUser.UserID);
            oUser.sPasswordHash = EncryptedPwd;
            dbContext.SaveChanges();

            if (oUser.UserID > 0)
            {
                cXIAppUserRoles oURole = new cXIAppUserRoles();
                oURole.UserID = oUser.UserID;
                int RoleID = dbContext.XIAppRoles.Where(m => m.sRoleName.ToLower() == "WebUsers".ToLower()).Select(m => m.RoleID).FirstOrDefault();
                oURole.RoleID = RoleID;
                dbContext.XIAppUserRoles.Add(oURole);
                dbContext.SaveChanges();
            }
        }
        else
        {
            oUser.IsUserExist = true;
        }
        //}
        //catch (Exception ex)
        //{
        //    logger.Error(ex);
        //    Common.SaveErrorLog(ex.ToString(), sDatabase);
        //    return Json(new VMCustomResponse() { ID = 0, Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
        //}
        return oUser;
    }
    #endregion

    #region Payment
    public string Payment(string sDatabase, int OrganizationID, string ID, decimal Amount)
    {
        ModelDbContext dbcontext = new ModelDbContext(sDatabase);
        var oPaymentGateWay = dbcontext.PaymentGateWay.Where(x => x.OrganizationID == OrganizationID && x.StatusTypeID == 10).FirstOrDefault();
        if (oPaymentGateWay != null)
        {
            if (oPaymentGateWay.sName.ToLower() == "GlobalPayment".ToLower())
            {
                PayWithGlobalPayments(oPaymentGateWay, ID, Amount);
            }
        }
        return null;
    }
    public void PayWithGlobalPayments(PaymentGateWay oPaymentGateWay, string ID, decimal Amount)
    {
        var timestamp = DateTime.Now.AddHours(1).ToString("yyyyMMddHHmmss");
        var currency = "GBP";
        var orderid = ID;
        var amount = Convert.ToInt32(Amount * 100);
        var sha = SHA1HashStringForUTF8String(timestamp + "." + oPaymentGateWay.sMerchantID + "." + orderid + "." + amount + "." + currency);
        var sha1 = SHA1HashStringForUTF8String(sha + "." + oPaymentGateWay.sSecret);
        RemotePost myremotepost = new RemotePost();
        myremotepost.Url = oPaymentGateWay.ReturnUrl;
        myremotepost.Add("MERCHANT_ID", oPaymentGateWay.sMerchantID);
        myremotepost.Add("SECRET_ID", oPaymentGateWay.sSecret);
        myremotepost.Add("TIMESTAMP", timestamp);
        myremotepost.Add("ACCOUNT", "internet");
        myremotepost.Add("ORDER_ID", orderid.ToString());
        myremotepost.Add("AMOUNT", amount.ToString());
        myremotepost.Add("CURRENCY", currency);
        myremotepost.Add("SHA1HASH", sha1);
        myremotepost.Add("AUTO_SETTLE_FLAG", "1");
        myremotepost.Add("MERCHANT_RESPONSE_URL", oPaymentGateWay.ResponseUrl);
        myremotepost.Post();
    }
    public string SHA1HashStringForUTF8String(string input)
    {
        using (SHA1Managed sha1 = new SHA1Managed())
        {
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder(hash.Length * 2);

            foreach (byte b in hash)
            {
                // can be "x2" if you want lowercase
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
    public class RemotePost
    {
        private System.Collections.Specialized.NameValueCollection Inputs = new System.Collections.Specialized.NameValueCollection();
        public string Url = "";
        public string Method = "post";
        public string FormName = "form1";
        public void Add(string name, string value)
        {
            Inputs.Add(name, value);
        }
        public void Post()
        {
            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.Write("<html><head>");
            System.Web.HttpContext.Current.Response.Write(string.Format("</head><body onload=\"document.{0}.submit()\">", FormName));
            System.Web.HttpContext.Current.Response.Write(string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" >", FormName, Method, Url));
            for (int i = 0; i < Inputs.Keys.Count; i++)
            {
                System.Web.HttpContext.Current.Response.Write(string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", Inputs.Keys[i], Inputs[Inputs.Keys[i]]));
            }
            System.Web.HttpContext.Current.Response.Write("</form>");
            System.Web.HttpContext.Current.Response.Write("</body></html>");
            System.Web.HttpContext.Current.Response.End();
        }
    }
    #endregion

    public string GetQuestionSetParamValue(cQSInstance cQSInstance, string sQSNotation)
    {
        ModelDbContext dbContext = new ModelDbContext(); string CommText = ""; string sReturnValue = "";
        List<string[]> Rows = new List<string[]>();
        List<cFieldInstance> oFieldInstance = new List<cFieldInstance>();
        string sValue = "";
        string sSelectType = ""; int iStepID = 0; int iFieldDefID = 0;
        IDictionary<string, string> DictionaryList = new Dictionary<string, string>();
        if (!string.IsNullOrEmpty(sQSNotation))
        {
            string[] sQSNotations = sQSNotation.Split('.');
            if (sQSNotations != null)
            {
                //var sBODataSource = string.Empty;
                //sBODataSource = GetBODataSource(cQSInstance.FKiBODID, 0, "XIDNA", null);
                //SqlConnection Con = new SqlConnection(sBODataSource);
                //using (SqlCommand cmd = new SqlCommand("", Con))
                //{
                //    cmd.CommandText = "";
                //    cmd.Connection = Con;
                //    Con.Open();

                foreach (var str in sQSNotations)
                {
                    string[] sType = str.Split('(');
                    if (sType.Count() == 2)
                    {
                        if (sType[0] == "QS")
                        {
                            sType[1] = sType[1].TrimEnd(')');
                            if (sType[1] == "iMyQSID")
                            {
                                //sSelectType = "FKiQSInstanceID";
                                sSelectType = Convert.ToString(cQSInstance.ID);
                                DictionaryList.Add("FKiQSInstanceID", sSelectType);
                                //if (oFieldInstance != null)
                                //{
                                //    oFieldInstance = dbContext.XIFieldInstance.Where(X => X.FKiQSInstanceID == cQSInstance.ID).ToList();
                                //}
                                //cmd.CommandText += " and FKiQSInstanceID="+ cQSInstance.ID + "";
                            }
                        }
                        if (sType[0] == "StepName")
                        {
                            sType[1] = sType[1].TrimEnd(')');
                            sType[1] = sType[1].Trim('\'');
                            iStepID = cQSInstance.QSDefinition.QSSteps.Where(x => x.sName == sType[1]).Select(x => x.ID).FirstOrDefault();
                            sSelectType = Convert.ToString(iStepID);
                            DictionaryList.Add("FKiQSStepDefinitionID", sSelectType);
                            //if(oFieldInstance!=null)
                            //{
                            //    oFieldInstance = oFieldInstance.Where(x => x.FKiQSStepDefinitionID == iStepID).ToList();
                            //}
                            //cmd.CommandText += " and FKiQSStepDefinitionID=" + iStepID + "";
                        }
                        if (sType[0] == "XIField")
                        {
                            sType[1] = sType[1].TrimEnd(')');
                            sType[1] = sType[1].Trim('\'');
                            foreach (var sec in cQSInstance.QSDefinition.QSSteps.Where(x => x.ID == iStepID))
                            {
                                foreach (var fielddef in sec.FieldDefinitions)
                                {
                                    if (fielddef.FieldOrigin.sName == sType[1])
                                    {
                                        iFieldDefID = fielddef.ID;
                                        sSelectType = Convert.ToString(iFieldDefID);
                                        DictionaryList.Add("FKiFieldDefinitionID", sSelectType);
                                        //if (oFieldInstance != null)
                                        //{
                                        //    oFieldInstance = oFieldInstance.Where(x => x.FKiFieldDefinitionID == iFieldDefID).ToList();

                                        //}
                                        //cmd.CommandText += " and FKiFieldDefinitionID=" + iFieldDefID + "";
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        sSelectType = sType[0];
                        DictionaryList.Add("sSelectValue", sSelectType);
                        CommText = "select " + sType[0] + " from XIFieldInstance_T where 1=1";
                    }

                }

                //    cmd.CommandText = cmd.CommandText.Insert(0, CommText);
                //    SqlDataReader reader = cmd.ExecuteReader();
                //    if (reader.Read())
                //    {
                //        sReturnValue=reader.GetString(0);
                //    }
                //    Con.Dispose();
                //}
                string s = sSelectType;
            }
            // return sReturnValue;
        }
        sValue = GetQuestionSetParamWithValues(cQSInstance, DictionaryList);
        return sValue;
    }
    public string GetQuestionSetParamWithValues(cQSInstance cQSInstance, IDictionary<string, string> DictionaryList)
    {
        string str = ""; string sSelectString = ""; string sReturnValue = ""; string sReturnType = "";
        var sBODataSource = string.Empty;
        string sDatabase = ConfigurationManager.AppSettings["CoreApplicatoinDatabase"];
        sBODataSource = GetBODataSource(-1, 0, sDatabase, null);
        foreach (var item in DictionaryList)
        {
            if (item.Key != "sSelectValue")
            {
                str += " and " + item.Key + "=" + item.Value + "";
            }
            else
            {
                sSelectString = "select CONVERT(varchar," + item.Value + "),SQL_VARIANT_PROPERTY(" + item.Value + ",'BaseType') AS 'Base Type' from XIFieldInstance_T where 1=1";
            }

        }
        Common.SaveErrorLog(sBODataSource, "XIDNA");
        using (SqlConnection Con = new SqlConnection(sBODataSource))
        {
            SqlCommand cmd = new SqlCommand("", Con);
            cmd.CommandText = "";
            cmd.Connection = Con;
            Con.Open();
            cmd.CommandText = str;
            cmd.CommandText = cmd.CommandText.Insert(0, sSelectString);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                sReturnValue = reader.GetString(0);
                sReturnType = reader.GetString(1);
            }
            Con.Dispose();
            if (sReturnType == "datetime")
            {
                DateTime dtvalue = Convert.ToDateTime(sReturnValue);
                sReturnValue = dtvalue.ToString("MM/dd/yyyy");
            }
            return sReturnValue;
        }
    }
    public int SaveQuotesList(cAggregations oQuotes, int iQSInstanceID)
    {
        //try
        //{
        if (oQuotes != null)
        {

            using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
            {
                Common.SaveErrorLog(Con.ToString(), "XIDNA");
                SqlCommand cmd = new SqlCommand("", Con);
                cmd.CommandText = "INSERT INTO Aggregations_T (sInsurer,rPrice,bLiablityCover,bLiabilityLimit,bLossOfMeteredWater,bLegelExpensesCover,FKiQSInstanceID,FKiCustomerID,FKiUserID ) " +
                   " VALUES ('" + oQuotes.sInsurer + "'," + oQuotes.rPrice + "," + Convert.ToByte(oQuotes.bLiablityCover) + "," + Convert.ToByte(oQuotes.bLiabilityLimit) + "," + Convert.ToByte(oQuotes.bLossOfMeteredWater) + "," + Convert.ToByte(oQuotes.bLegelExpensesCover) + "," + iQSInstanceID + "," + oQuotes.FKiCustomerID + "," + oQuotes.FKiUserID + ")";

                Con.Open();
                //Con.ChangeDatabase(sOrgDB);
                cmd.ExecuteNonQuery();
                Con.Dispose();

            }
            XiLinkRepository XiLinkRepository = new XiLinkRepository();
            XiLinkRepository.InsertIntoAggregations(iQSInstanceID, ConfigurationManager.AppSettings["SharedDataBase"], oQuotes.FKiUserID, oQuotes.FKiCustomerID);
            Common.SaveErrorLog("Quotes created successfully", "XIDNA");
        }
        return 1;
        //}
        //catch (Exception ex)
        //{
        //    logger.Error(ex);
        //    Common.SaveErrorLog(ex.ToString(), "XIDNA");
        //    return 0;
        //}
    }

    public string TestMethod(string Message)
    {
        return null;
    }


    #region 

    public int BuildPolicyObject(cQSInstance oQSInstance, int iUserID, string sOrgName, int iCustomerID)
    {
        var oBOInsatnce = Get_BOInstance("ACPolicy_T", "Create", iUserID, sOrgName, DevDB);
        Common.SaveErrorLog("ACPolicy_T boinstance created successfully", "XIDNA");
        string sFirstNameQSNotations = "QS(iMyQSID).StepName('Your Details').XIField('sForeName').sValue";
        string FirstName = GetQuestionSetParamValue(oQSInstance, sFirstNameQSNotations);
        string sLastNameQSNotations = "QS(iMyQSID).StepName('Your Details').XIField('sLastName').sValue";
        string LastName = GetQuestionSetParamValue(oQSInstance, sLastNameQSNotations);
        var sName = oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "sName".ToLower()).FirstOrDefault();
        var irefID = oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "refTypeID".ToLower()).FirstOrDefault();
        var Customerid = oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "FKiCustomerID".ToLower()).FirstOrDefault();
        var userid = oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "FKiUserID".ToLower()).FirstOrDefault();
        var notes = oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "sNotes".ToLower()).FirstOrDefault();
        if (sName != null)
        {
            oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "sName".ToLower()).FirstOrDefault().sValue = FirstName + " " + LastName;
        }
        if (irefID != null)
        {
            oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "refTypeID".ToLower()).FirstOrDefault().sValue = null;
        }
        if (Customerid != null)
        {
            oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "FKiCustomerID".ToLower()).FirstOrDefault().sValue = iCustomerID.ToString();
        }
        if (userid != null)
        {
            oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "FKiUserID".ToLower()).FirstOrDefault().sValue = iUserID.ToString();
        }
        if (notes != null)
        {
            oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "sNotes".ToLower()).FirstOrDefault().sValue = null;
        }
        Common.SaveErrorLog("FKiUserID at PolicyObject " + iUserID, "XIDNA");
        Random random = new Random();
        int iPolicyNo = random.Next(10000, 20000);
        oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "iPolicyNo".ToLower()).FirstOrDefault().sValue = iPolicyNo.ToString();
        oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "dCoverStart".ToLower()).FirstOrDefault().sValue = DateTime.Now.ToString();
        oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "dCoverEnd".ToLower()).FirstOrDefault().sValue = DateTime.Now.AddYears(1).ToString();
        oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "bPaymentCollected".ToLower()).FirstOrDefault().sValue = "1";
        var obj = Save(oBOInsatnce, DevDB, iUserID, sOrgName);
        var ACPolicyID = obj.NVPairs.Where(m => m.sName.ToLower() == "ID".ToLower()).Select(m => m.sValue).FirstOrDefault();
        Common.SaveErrorLog("Policy Created Successfully", "XIDNA");
        Common.SaveErrorLog(ACPolicyID, "XIDNA");
        return Convert.ToInt32(ACPolicyID);

    }
    public void BuildTransactionObject(cQSInstance oQSInstance, int iUserID, string sOrgName, int ACPolicyID)
    {
        var oBOInsatnce = Get_BOInstance("ACTransaction_T", "Create", iUserID, sOrgName, DevDB);
        Common.SaveErrorLog("ACTransaction_T boinstance created successfully", "XIDNA");
        string sFirstNameQSNotations = "QS(iMyQSID).StepName('Your Details').XIField('sForeName').sValue";
        string FirstName = GetQuestionSetParamValue(oQSInstance, sFirstNameQSNotations);
        string sLastNameQSNotations = "QS(iMyQSID).StepName('Your Details').XIField('sLastName').sValue";
        string LastName = GetQuestionSetParamValue(oQSInstance, sLastNameQSNotations);
        var sName = oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "sName".ToLower()).FirstOrDefault();
        var clientid = oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "FKiClientID".ToLower()).FirstOrDefault();
        var policyid = oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "FKiACPolicyID".ToLower()).FirstOrDefault();
        var analysis = oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "sAnalysis".ToLower()).FirstOrDefault();
        var transactionid = oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "FKiACTransTypeID".ToLower()).FirstOrDefault();
        if (sName != null)
        {
            oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "sName".ToLower()).FirstOrDefault().sValue = FirstName + " " + LastName;
        }
        if (clientid != null)
        {
            oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "FKiClientID".ToLower()).FirstOrDefault().sValue = null;
        }
        if (policyid != null)
        {
            oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "FKiACPolicyID".ToLower()).FirstOrDefault().sValue = ACPolicyID.ToString();
        }
        if (analysis != null)
        {
            oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "sAnalysis".ToLower()).FirstOrDefault().sValue = null;
        }
        if (transactionid != null)
        {
            oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "FKiACTransTypeID".ToLower()).FirstOrDefault().sValue = null;
        }
        Save(oBOInsatnce, DevDB, iUserID, sOrgName);
        Common.SaveErrorLog("Transaction Created Successfully", "XIDNA");
    }
    public void BuildJournalEntryObject(cQSInstance oQSInstance, int iUserID, string sOrgName)
    {
        var oBOInsatnce = Get_BOInstance("ACJournalEntry_T", "Create", iUserID, sOrgName, DevDB);
        oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "FKiAccountID".ToLower()).FirstOrDefault().sValue = null;
        oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "FKiTwinJEID".ToLower()).FirstOrDefault().sValue = null;
        oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "FKiJETemplateID".ToLower()).FirstOrDefault().sValue = null;
        oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "rAmount".ToLower()).FirstOrDefault().sValue = null;
        Save(oBOInsatnce, DevDB, iUserID, sOrgName);
    }
    public void BuildRequirementObject(cQSInstance oQSInstance, int iUserID, string sOrgName)
    {
        var oBOInsatnce = Get_BOInstance("Requirement_T", "Create", iUserID, sOrgName, DevDB);
        oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "sName".ToLower()).FirstOrDefault().sValue = null;
        oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "FKiClientID".ToLower()).FirstOrDefault().sValue = null;
        Save(oBOInsatnce, DevDB, iUserID, sOrgName);
    }
    public void BuildCallObject(cQSInstance oQSInstance, int iUserID, string sOrgName)
    {
        var oBOInsatnce = Get_BOInstance("Call_T", "Create", iUserID, sOrgName, DevDB);
        oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "sName".ToLower()).FirstOrDefault().sValue = null;
        oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "sCallDesc".ToLower()).FirstOrDefault().sValue = null;
        oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "FKsRecordedID".ToLower()).FirstOrDefault().sValue = null;
        oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "FKiQuoteID".ToLower()).FirstOrDefault().sValue = null;
        Save(oBOInsatnce, DevDB, iUserID, sOrgName);
    }
    public cBOInstance BuildCustomerObject(cQSInstance oQSInstance, int iUserID, string sOrgName)
    {
        //var DevDB = "xidna";
        Common.SaveErrorLog(DevDB, "XIDNA");
        var oBOInsatnce = Get_BOInstance("Customer_t", "Save Group", iUserID, sOrgName, DevDB);
        Common.SaveErrorLog("customer boinstance created successfully", "XIDNA");
        string sFirstNameQSNotations = "QS(iMyQSID).StepName('Your Details').XIField('sForeName').sValue";
        string FirstName = GetQuestionSetParamValue(oQSInstance, sFirstNameQSNotations);
        Common.SaveErrorLog(FirstName, "XIDNA");
        string sLastNameQSNotations = "QS(iMyQSID).StepName('Your Details').XIField('sLastName').sValue";
        string LastName = GetQuestionSetParamValue(oQSInstance, sLastNameQSNotations);
        string sEmailQSNotations = "QS(iMyQSID).StepName('Your Details').XIField('sEmail').sValue";
        string sEmail = GetQuestionSetParamValue(oQSInstance, sEmailQSNotations);
        string sMobQSNotations = "QS(iMyQSID).StepName('Your Details').XIField('sMob').sValue";
        Common.SaveErrorLog(sMobQSNotations, "XIDNA");
        string sMob = GetQuestionSetParamValue(oQSInstance, sMobQSNotations);
        string sQSNotations = "QS(iMyQSID).StepName('About You').XIField('dtDateofbirth').dValue";
        string dDOB = GetQuestionSetParamValue(oQSInstance, sQSNotations);
        DateTime now = DateTime.Today;
        Common.SaveErrorLog(dDOB, "XIDNA");
        DateTime dDateOfBirth = Convert.ToDateTime(dDOB);
        int Age = now.Year - dDateOfBirth.Year;
        if (now < dDateOfBirth.AddYears(Age)) Age--;

        oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "sName".ToLower()).FirstOrDefault().sValue = FirstName + " " + LastName;
        oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "sEmail".ToLower()).FirstOrDefault().sValue = sEmail;
        oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "sMobileNo".ToLower()).FirstOrDefault().sValue = sMob;
        oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "sCity".ToLower()).FirstOrDefault().sValue = null;
        oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "sAge".ToLower()).FirstOrDefault().sValue = Age.ToString();
        oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "FKiLocationID".ToLower()).FirstOrDefault().sValue = null;
        var obj = Save(oBOInsatnce, DevDB, iUserID, sOrgName);
        Common.SaveErrorLog("customer created successfully", "XIDNA");
        return obj;
    }
    public void BuildERPTaskObject(cQSInstance oQSInstance, int iUserID, string sOrgName)
    {
        var oBOInsatnce = Get_BOInstance("ERP_Task", "Create", iUserID, sOrgName, DevDB);
        oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "sName".ToLower()).FirstOrDefault().sValue = null;
        oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "FKiTaskTypeID".ToLower()).FirstOrDefault().sValue = null;
        oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "sTaskDesc".ToLower()).FirstOrDefault().sValue = null;
        oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "FKsUserID".ToLower()).FirstOrDefault().sValue = null;
        oBOInsatnce.NVPairs.Where(m => m.sName.ToLower() == "FKiOrderID".ToLower()).FirstOrDefault().sValue = null;
        Save(oBOInsatnce, "xicore", iUserID, sOrgName);
    }

    public cAggregations GetInstanceID(int QuoteID, string sSharedDB)
    {
        ModelDbContext dbContext = new ModelDbContext(sSharedDB);
        cAggregations oAggregtionsList = dbContext.Aggregations.Where(m => m.ID == QuoteID).FirstOrDefault();
        return oAggregtionsList;
        //return dbContext.Aggregations.Where(m => m.ID == QuoteID).Select(mbox => mbox.FKiQSInstanceID).FirstOrDefault();
    }

    public void UpdateQSInstanceStatus(int iQsInstanceID)
    {
        ModelDbContext dbContext = new ModelDbContext();
        var oQSINS = dbContext.QSInstance.Find(iQsInstanceID);
        if (oQSINS != null)
        {
            oQSINS.iCurrentStepID = -1;
            dbContext.SaveChanges();
        }
    }

    public void UpdateDriversToPolicy(int iQSID, int iPolicyID, int iUserID, string sOrgDB)
    {
        var sBODataSource = string.Empty;
        sBODataSource = GetBODataSource(-2, 0, null, sOrgDB);
        using (SqlConnection Conn = new SqlConnection(sBODataSource))
        {
            Conn.Open();
            SqlCommand SqlCmd = new SqlCommand();
            SqlCmd.Connection = Conn;
            string cmdText = "UPDATE Driver_T SET FKiPolicyID =" + iPolicyID + " WHERE" + " FKiQSInstanceID=" + iQSID;
            SqlCommand cmd = new SqlCommand(cmdText, Conn);
            cmd.ExecuteNonQuery();
            Conn.Close();
        }

    }
    #endregion

}
}
