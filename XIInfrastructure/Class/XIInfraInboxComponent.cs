using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XISystem;
using XICore;
using XIDatabase;
using System.Data;

namespace XIInfrastructure
{
    public class XIInfraInboxComponent
    {
        public int iRoleID;

        XIInfraCache oCache = new XIInfraCache();
        public CResult XILoad(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = oCResult.Get_MethodName();

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            //if iTraceLevel>0 then 
            //oCResult.oTraceStack.Trace("Stage", "Begin query build",milliseconds)
            //oCResult.oTraceStack.Trace("Stage",sError)
            //end if

            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            try
            {
                var sRoleID = oParams.Where(m => m.sName.ToLower() == "iRoleID".ToLower()).Select(m => m.sValue).FirstOrDefault();
                int.TryParse(sRoleID, out iRoleID);
                if (iRoleID > 0)
                {
                    var InboxD = (List<XIDInbox>)oCache.GetObjectFromCache(XIConstant.CacheInbox, null, iRoleID.ToString());
                    XIDefinitionBase oDef = new XIDefinitionBase();
                    var InboxC = (List<XIDInbox>)oDef.Clone(InboxD);
                    if (InboxC != null && InboxC.Count() > 0)
                    {
                        for (int k = 0; k < InboxC.Count(); k++)
                        {
                            if (InboxC[k].FKi1ClickID > 0)
                            {
                                var o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, InboxC[k].FKi1ClickID.ToString());
                                o1ClickD.Targets = InboxC[k].Target;
                                o1ClickD.Type = InboxC[k].TargetResultType;
                                if (InboxC[k].TargetResultType == "HTML")
                                {
                                    //Rep.HTMLCode = Spdb.ContentEditors.Where(m => m.ID == items.TargetTemplateID).Select(m => m.Content).FirstOrDefault();
                                }
                                o1ClickD = GetInboxCounts(o1ClickD);                                
                                InboxC[k].o1ClickD = o1ClickD;
                                InboxC[k].InboxCount = o1ClickD.InboxCount;
                                InboxC[k].Percentage = o1ClickD.Percentage;
                            }
                        }
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        oCResult.oResult = InboxC;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return oCResult;
        }
        private XID1Click GetInboxCounts(XID1Click o1ClickD)
        {
            int Count = GetInboxTargetResult(o1ClickD);
            XID1Click InboxCount = GetInboxTargetCount(o1ClickD, Count);
            o1ClickD.InboxCount = InboxCount.InboxCount;
            o1ClickD.Percentage = InboxCount.Percentage;
            if (o1ClickD.Sub1Clicks.Count() > 0)
            {
                o1ClickD.Sub1Clicks.ToList().ForEach(m => m.Type = o1ClickD.Type);
                foreach (var item in o1ClickD.Sub1Clicks)
                {
                    GetInboxCounts(item);
                }
            }
            return o1ClickD;
        }

        private XID1Click GetInboxTargetCount(XID1Click items, int Count)
        {
            string InboxCount = "";
            string Percentage = "";
            if (items.Type == "Progress Bar")
            {
                if (Count > 0 && items.Targets > 0)
                {
                    double percentage = (double)Count / items.Targets;
                    InboxCount = Math.Round(percentage * 100, 2).ToString();
                    if (Convert.ToDecimal(InboxCount) > 100)
                    {
                        InboxCount = 100.ToString();
                    }
                }
                else
                {
                    InboxCount = "0";
                }
            }
            else if (items.Type == "Number")
            {
                if (items.Targets > 0)
                {
                    InboxCount = Count + "/" + items.Targets;
                }
                else
                {
                    InboxCount = Count + "/0";
                }
            }
            else if (items.Type == "Progress Bar & Number")
            {
                if (items.Targets > 0)
                {
                    InboxCount = Count + "/" + items.Targets;
                    double percentage = (double)Count / items.Targets;
                    Percentage = Math.Round(percentage * 100, 2).ToString();
                }
                else
                {
                    InboxCount = Count + "/0";
                    Percentage = "0";
                }
            }
            else if (items.Type == "HTML")
            {
                if (items.Targets > 0)
                {
                    InboxCount = Count + "/" + items.Targets;
                    double percentage = (double)Count / items.Targets;
                    Percentage = Math.Round(percentage * 100, 2).ToString();
                }
                else
                {
                    InboxCount = Count + "/0";
                    Percentage = "0";
                }
                if (items.HTMLCode != null)
                {
                    items.HTMLCode = items.HTMLCode.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "'").Replace("{{Target}}", Percentage);
                }
            }
            items.InboxCount = InboxCount;
            items.Percentage = Percentage;
            return items;
        }

        private int GetInboxTargetResult(XID1Click o1ClickD)
        {
            int iCount = 0;
            XID1Click o1ClickC = new XID1Click();
            o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
            XIDXI oXID = new XIDXI();
            XIDBAPI Connection = new XIDBAPI();
            int iDataSource = 0;
            XIDBO oBOD = new XIDBO();
            if (o1ClickC.BOID > 0)
            {
                if (o1ClickC.BOIDXIGUID != null && o1ClickC.BOIDXIGUID != Guid.Empty)
                {
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickC.BOIDXIGUID.ToString());
                }
                else if (o1ClickC.BOID > 0)
                {
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickC.BOID.ToString());
                }
                iDataSource = oBOD.iDataSource;
            }
            string sConntection = oXID.GetBODataSource(iDataSource.ToString(),oBOD.FKiApplicationID);
            Connection = new XIDBAPI(sConntection);
            var Query = o1ClickC.Query;
            List<CNV> nParams = new List<CNV>();
            o1ClickC.ReplaceFKExpressions(nParams);
            string StarQuery = o1ClickC.ReplaceQueryStringWithStar(Query);
            string Count = Connection.GetTotalCount(CommandType.Text, StarQuery, null);
            int.TryParse(Count, out iCount);
            //var oDictionaryBOI = o1ClickC.OneClick_Run(false);
            return iCount;
        }
    }
}