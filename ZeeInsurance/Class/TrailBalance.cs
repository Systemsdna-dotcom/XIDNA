using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XICore;
using XISystem;
using System.Data.SqlClient;
using System.Text;
using static XIDatabase.XIDBAPI;
using System.Data;

namespace ZeeInsurance
{
    public class TrailBalance
    {
        public SqlConnection TXSqlconn = null;
        public SqlTransaction TXSqlTrans = null;
        private TransactionInitiation TXinitial;
        public TrailBalance()
        {
            TXinitial = new TransactionInitiation();
            TXinitial.sTXBeginAt = "TRAILBALANCE";
        }
        string FKiAccountNoID = "";
        string FKiEnterpriseID = "";
        string _refAccountCat = "";
        private int ResultType = 0;
        public int iResultType
        {
            get { return ResultType; }
            set { ResultType = value; }
        }
        private double DR = 0;
        public double fDR
        {
            get { return DR; }
            set { DR = value; }
        }
        private double CR = 0;
        public double fCR
        {
            get { return CR; }
            set { CR = value; }
        }
        public CResult MonthEnd(List<CNV> oParams)
        {
            TXinitial.sTXBeginAt = "TRAILBALANCE";
            CResult oCResult = new CResult();
            XIInfraCache oCache = new XIInfraCache();
            XIInstanceBase oIB = new XIInstanceBase();
            try
            {
                DateTime dMonthToClose = DateTime.Now.AddMonths(-1);
                // previous month
                int iYear = dMonthToClose.Year;
                int iMonth = dMonthToClose.Month;
                XIIBO oBOI = new XIIBO();// (TXinitial);
                XIDXI oXID = new XIDXI();
                XIIXI oIXI = new XIIXI();
                List<XIIBO> oBOInstanceLIst = new List<XIIBO>();
                List<CNV> nParams = new List<CNV>();
                nParams.Add(new CNV { sName = "iYear", sValue = iYear.ToString() });
                nParams.Add(new CNV { sName = "iMonth", sValue = iMonth.ToString() });
                var oCMonthEnd = oIXI.BOI("ACMonthEnd_T", null, "Label", nParams);

                if (oCMonthEnd == null)
                {
                    XIDBO objNew = (XIDBO)oXID.Get_BODefinition("ACMonthEnd_T").oResult;
                    oBOI.BOD = objNew;
                    oBOI.SetAttribute("iYear", iYear.ToString());
                    oBOI.SetAttribute("iMonth", iMonth.ToString());
                    var tRC = oBOI.Save(oBOI);

                    if (tRC.oResult == null && !tRC.bOK)
                    {
                        oCResult.sMessage = "Error on INSERT of a Month End for Year:" + iYear + " and Month: " + iMonth + ". Contact your system administrator <BR>";
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oCResult.sMessage = "Error on INSERT of a Month End for Year:" + iYear + " and Month: " + iMonth + ". Contact your system administrator <BR>";
                        oIB.SaveErrortoDB(oCResult);
                    }
                    else
                    {
                        oCMonthEnd = (XIIBO)tRC.oResult;
                        oCResult.sMessage = "Month End successfully recorded for Year:" + iYear + " and Month: " + iMonth;
                    }
                }
                else
                {
                    oCResult.sMessage = "There is already a Month End for Year:" + iYear + " and Month: " + iMonth + ". Contact your system administrator";
                }

                //string strAction = "lf.Accounts";
                //int penmPersistAction = 1;
                //int paInsert = 1;
                //if (penmPersistAction == paInsert)
                //{
                // Now assign ALL transactions AND Journals to this month end id
                //  at the moment, this will be ALL transactions with a date
                DateTime dMonthEnd = Convert.ToDateTime(iYear.ToString() + "-" + iMonth.ToString() + "-1");
                DateTime dDateUpTo = dMonthEnd.AddMonths(1); // should anyway be the 1st day of the current month
                DateTime dPreviousME = dMonthEnd.AddMonths(-1);
                nParams = new List<CNV>();
                nParams.Add(new CNV { sName = "iYear", sValue = dPreviousME.Year.ToString() });
                nParams.Add(new CNV { sName = "iMonth", sValue = dPreviousME.Month.ToString() });
                var oPrevME = oIXI.BOI("ACMonthEnd_T", null, "Label", nParams);
                //var tRtn = oPrevME.oCResult;

                if (oPrevME != null)
                {
                    // there isn't one (hopefully!)
                }

                // WARNING - these collections may be very large
                StringBuilder sWhereClause = new StringBuilder();
                sWhereClause.Append(" WHERE dWhen < '" + dDateUpTo.ToString() + "' AND FKiMonthEndID IS NULL");
                XID1Click oMETransactions = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "Month End Transactions", null);
                XID1Click oMETransactionsCopy = (XID1Click)oMETransactions.Clone(oMETransactions);
                oMETransactionsCopy.Query += sWhereClause;
                // eg: SELECT SUM (rAmount)  FROM [ACJournalEntry_T] LEFT JOIN [ACTransaction_T] ON 
                //[ACJournalEntry_T].[FKiTransactionID] = [ACTransaction_T].[id] WHERE (1 = 1)  AND ([ACJournalEntry_T].[FKiAccountID] = 528) 
                //AND ( ([ACTransaction_T].[FKiACPolicyID] = 3217) AND ([ACJournalEntry_T].[iTransType] = 200) 
                //AND [ACTransaction_T].[iProcessStatus] < 20   '[ACJournalEntry_T].[iType] = 10) AND

                // multi db switch
                // Set oRS = i.zX.Db.rs(sQry)
                var oColTransactions = new Dictionary<string, XIIBO>();
                oColTransactions = oMETransactionsCopy.OneClick_Execute();
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oColTransactions;

                if (oColTransactions == null)
                {
                    oCResult.sMessage = "Unable to DB2Collection oColTransactions \r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oCResult.sMessage = "Unable to DB2Collection oColTransactions \r\n";
                    oIB.SaveErrortoDB(oCResult);
                    return oCResult;
                }

                XID1Click oMEJournals = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "Month End Journal Entries", null);
                XID1Click oMEJournalsCopy = (XID1Click)oMEJournals.Clone(oMEJournals);
                sWhereClause = new StringBuilder();
                sWhereClause.Append(" WHERE dDate < '" + dDateUpTo.ToString() + "' AND FKiMonthEndID IS NULL");
                oMEJournalsCopy.Query += sWhereClause;
                var oColJournals = new Dictionary<string, XIIBO>();
                oColJournals = oMEJournalsCopy.OneClick_Execute();
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oColJournals;
                if (oColJournals == null)
                {
                    oCResult.sMessage = "Unable to DB2Collection oColJournals \r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oCResult.sMessage = "Unable to DB2Collection oColJournals \r\n";
                    oIB.SaveErrortoDB(oCResult);
                    return oCResult;
                }

                var TransactionBOD = (XIDBO)oXID.Get_BODefinition("ACTransaction_T").oResult;
                foreach (var oTransaction in oColTransactions)
                {
                    oTransaction.Value.SetAttribute("FKiMonthEndID", oCMonthEnd.AttributeI("id").sValue);
                    oTransaction.Value.BOD = TransactionBOD;
                    var res = oTransaction.Value.Save(oTransaction.Value);
                    if (!res.bOK && res.oResult == null)
                    {
                        oCResult.sMessage = "Error UPDATING TRANSACTION id=" + oTransaction.Value.AttributeI("id").sValue;
                        return oCResult;
                    }
                }
                var JournalsBOD = (XIDBO)oXID.Get_BODefinition("ACJournalEntry_T").oResult;
                foreach (var oJE in oColJournals)
                {
                    _refAccountCat = oJE.Value.AttributeI("refAccountCategory").sValue;
                    oJE.Value.SetAttribute("FKiMonthEndID", oCMonthEnd.AttributeI("id").sValue);
                    oJE.Value.BOD = JournalsBOD;
                    var res = oJE.Value.Save(oJE.Value);
                    if (!res.bOK && res.oResult == null)
                    {
                        oCResult.sMessage = "Error UPDATING JOURNAL ENTRY id=" + oJE.Value.AttributeI("id").sValue;
                        return oCResult;
                    }
                }

                // OK so far, now we have to create a snapshot of the totals WHEN THIS IS INSERTED which may include transactions outside of the
                //   range of the dates above. This is an issue to discuss with the accountants. If the button is pressed just after end of month
                //   before anyone does anything else then fine, otherwise what?
                // So, get all of the refAccounts (which will have one for each Nominal Account PER Enterprise)
                //   and for each create a snapsot AC/ACMonthEndNominal assign the enterprise to our snapshot, and the
                //   nominal account FK, and the cr and dr totals

                Dictionary<string, XIIBO> oColNominals = new Dictionary<string, XIIBO>();
                QueryEngine oQE = new QueryEngine();
                if (oCResult.bOK && oCResult.oResult != null)
                {
                    var oQResult = oQE.Execute_QueryEngine("ACrefAccountNo_T", "*", null);
                    if (oQResult.bOK && oQResult.oResult != null)
                    {
                        oColNominals = (Dictionary<string, XIIBO>)oQResult.oResult;
                    }
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oColNominals;
                if (oColNominals == null)
                {
                    oCResult.sMessage = "Unable to DB2Collection oColNominals ";
                    return oCResult;
                }
                foreach (var oNominal in oColNominals)
                {
                    XIIBO oMonthEndNominal = new XIIBO();// (TXinitial);
                    XIDBO oXIBOD = (XIDBO)oXID.Get_BODefinition("ACMonthEndNominal_T").oResult;
                    oMonthEndNominal.BOD = oXIBOD;
                    FKiAccountNoID = oNominal.Value.AttributeI("FKiAccountNoID").sValue;
                    FKiEnterpriseID = oNominal.Value.AttributeI("FKiEnterpriseID").sValue;
                    oMonthEndNominal.SetAttribute("FKiMonthEndID", oCMonthEnd.AttributeI("id").sValue);
                    oMonthEndNominal.SetAttribute("FKiEnterpriseID", oNominal.Value.AttributeI("FKiEnterpriseID").sValue);
                    oMonthEndNominal.SetAttribute("FKiAccountNoID", oNominal.Value.AttributeI("FKiAccountNoID").sValue);
                    oMonthEndNominal.SetAttribute("FKirefConfigurationID", oNominal.Value.AttributeI("FKirefConfigurationID").sValue);
                    oMonthEndNominal.SetAttribute("FKiACrefBaseAccountID", oNominal.Value.AttributeI("FKiACrefBaseAccountID").sValue);
                    // now get the current totals and apply to this
                    oMonthEndNominal.SetAttribute("rCRCurrent", iBO_getAttr("_rCRTB"));//oNominal.Value.AttributeI("_rCRTB").sValue);
                    oMonthEndNominal.SetAttribute("rDRCurrent", iBO_getAttr("_rDRTB"));// oNominal.Value.AttributeI("_rDRTB").sValue);
                    // now load the previous month's if there is one, and copy it's CR and DR to this one
                    if (oPrevME.AttributeI("id").iValue > 0)
                    {
                        oParams = new List<CNV>();
                        oParams.Add(new CNV { sName = "FKiMonthEndID", sValue = oPrevME.AttributeI("id").sValue });
                        var oPrevMENominal = oIXI.BOI("ACMonthEndNominal_T", null, "MonthEnd", oParams);
                        if (oPrevMENominal != null)
                        {
                            oMonthEndNominal.SetAttribute("rCRPrevious", oPrevME.AttributeI("rCRCurrent").sValue);
                            oMonthEndNominal.SetAttribute("rDRPrevious", oPrevME.AttributeI("rDRCurrent").sValue);
                        }
                    }
                    var response = oMonthEndNominal.Save(oMonthEndNominal);
                    if (!response.bOK && response.oResult != null)
                    {
                        oCResult.sMessage = "Error INSERTING oMonthEndNominal for Nominal id=" + oNominal.Value.AttributeI("id").sValue;
                    }
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oIB.SaveErrortoDB(oCResult);
            }
            if (TXinitial.sTXBeginAt == "TRAILBALANCE")
                TXinitial.TXCommitRollback(oCResult);
            return oCResult;
        }
        public string iBO_getAttr(string pstrAttr)
        {
            double dAmount;
            iResultType = 0;
            string sFont;
            double fLocalAmountDR = 0; double fLocalAmountCR = 0;
            switch (pstrAttr.ToLower())
            {
                case "_cr":
                    dAmount = Get_Sum(20);
                    if (iResultType == 20)
                    {
                        if (dAmount < 0)
                        {
                            sFont = "#FF0000";
                        }
                        else
                        {
                            sFont = "#000000";
                        }

                        pstrAttr = "<p align=\'right\'><font color=\'" + sFont + "\'>" + dAmount + "</font>"; //currency format
                    }

                    break;
                case "_dr":
                    dAmount = Get_Sum(10);
                    if (iResultType == 20)
                    {
                        if (dAmount < 0)
                        {
                            sFont = "#FF0000";
                        }
                        else
                        {
                            sFont = "#000000";
                        }
                        pstrAttr = "<p align=\'right\'><font color=\'" + sFont + "\'>" + dAmount + "</font>"; //Currency Fromat
                    }

                    break;
                case "_rcr":
                    dAmount = Get_Sum(20);
                    if (iResultType == 20)
                    {
                        pstrAttr = dAmount.ToString();
                    }

                    break;
                case "_rdr":
                    dAmount = Get_Sum(10);
                    if (iResultType == 20)
                    {
                        pstrAttr = dAmount.ToString();
                    }

                    break;
                case "_rnet":
                    Get_LocalTotals();
                    if (iResultType == 20)
                    {
                        pstrAttr = (fLocalAmountDR - fLocalAmountCR).ToString();
                    }

                    break;
                case "_rcrtb":
                    Get_LocalTotals();
                    if (iResultType == 20)
                    {
                        pstrAttr = fCR.ToString();
                    }

                    break;
                case "_rdrtb":
                    Get_LocalTotals();
                    if (iResultType == 20)
                    {
                        pstrAttr = fDR.ToString();
                    }

                    break;
            }
            return pstrAttr;
        }
        public double Get_Sum(int iCRDR)
        {
            const string ROUTINE_NAME = "[Get_Sum]";
            double dTotal = 0;
            XIIBO oAccount = new XIIBO();
            string sAccountCode;

            //            TODO: On Error GoTo Warning!!!: The statement is not translatable
            //             this ENTITY is only every created from the account type matrix.it references an account and it is this account that you have to see what transactions there
            //             are for. So for example bank account, so add up all the DRs for that account and all the CRs(all from the base journal entries i think)
            //              - however for some such as { Client}
            //            it is a bit more difficult and these must be added up for all accounts of type client
            XIDXI oXID = new XIDXI();
            XIIXI oXII = new XIIXI();
            CResult oCResult = new CResult();
            XIInstanceBase oIB = new XIInstanceBase();
            //List<CNV> oParams = new List<CNV>();
            //oParams.Add(new CNV { sName = "FKiAccountNoID", sValue = FKiAccountNoID });
            oAccount = oXII.BOI("ACAccount_T", FKiAccountNoID, "*", null);  //Load Account Details
            if (oAccount != null)
            {
                sAccountCode = oAccount.AttributeI("sAccountCode").sValue;
                string JEBO = "ACJournalEntry_T";
                string ACBO = "ACAccount_T";
                string TranBO = "ACTransaction_T";
                //All this is doing is checking if the user has restricted the item that shows in results to one
                //instance
                StringBuilder sWhereClause = new StringBuilder();
                sWhereClause.Append(" AND " + JEBO + ".iType=" + iCRDR.ToString());
                switch (sAccountCode.ToLower())
                {
                    case "{client}":
                        sWhereClause.Append(" AND " + ACBO + ".iType = 0");
                        break;
                    case "{insurer}":
                        sWhereClause.Append(" AND " + ACBO + ".iType = 10");
                        break;
                    case "{premium finance}":
                        sWhereClause.Append(" AND " + ACBO + ".iType = 40");
                        break;
                    default:
                        sWhereClause.Append(" AND " + ACBO + ".sAccountCode='" + sAccountCode + "'");
                        break;
                }
                sWhereClause.Append(" AND " + TranBO + ".FKiEnterpriseID=" + FKiEnterpriseID);
                if (!string.IsNullOrEmpty(_refAccountCat))
                {
                    sWhereClause.Append(" AND " + TranBO + ".refAccountCategory=" + _refAccountCat);
                }

                //i.GetAttr("_refAccountCat")
                // 2011.02.01 - exclude corrected, temp etc. Only allow live and replacement
                sWhereClause.Append(" AND (" + TranBO + ".iStatus = 0 OR " + TranBO + ".iStatus = 30 )");
                XIInfraCache oCache = new XIInfraCache();
                XID1Click oJournalsSum = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "Trail Balance Sum", null);
                XID1Click oJournalsSumCopy = (XID1Click)oJournalsSum.Clone(oJournalsSum);
                oJournalsSumCopy.Query += sWhereClause;
                var oSum = new Dictionary<string, XIIBO>();
                oSum = oJournalsSumCopy.OneClick_Execute();
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oSum;
                if (oCResult.oResult == null)
                {
                    oCResult.sMessage = "Unable to generate Report query \r\n";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oCResult.sMessage = "Unable to generate Report query \r\n";
                    oIB.SaveErrortoDB(oCResult);
                    return 0;
                }
                if (oCResult.oResult != null)
                {
                    foreach (var item in oSum)
                    {
                        //dTotal = item.Value.AttributeI("rAmount").doValue;
                        if (string.IsNullOrEmpty(item.Value.AttributeI("rAmount").sValue))
                        {
                            iResultType = 20;
                            // DS: allow null to mean zero as this is the result of the additions  ' 10
                            dTotal = 0;
                        }
                        else
                        {
                            dTotal = item.Value.AttributeI("rAmount").doValue;
                            iResultType = 20;
                        }
                    }
                }
            }
            return dTotal;
        }
        public void Get_LocalTotals()
        {
            const string ROUTINE_NAME = "[Get_LocalTotals]";
            //TODO: On Error GoTo Warning!!!: The statement is not translatable
            //If bLocalTotals = False Then
            double fLocalAmountDR = Get_Sum(10);
            double fLocalAmountCR = Get_Sum(20);

            if (fLocalAmountDR > fLocalAmountCR)
            {
                fDR = fLocalAmountDR - fLocalAmountCR;
                fCR = 0;
            }
            else
            {
                fCR = Convert.ToDouble(fLocalAmountCR - fLocalAmountDR);
                fDR = 0;
            }
            //bool bLocalTotals = true;
            // Get_LocalTotals(iResultType);
            return;
        }
        public CResult AGEDebt(Dictionary<string, XIIValue> XIValues)
        {
            CResult oCResult = new CResult();
            XIInstanceBase oIB = new XIInstanceBase();
            try
            {
                string ROUTINE_NAME = "[AGEDebt]";
                //XIIXI oIXI = new XIIXI();
                //int iQSIID = oParams.Where(t => t.sName == "iQSIID").Select(f => Convert.ToInt32(f.sValue)).FirstOrDefault();
                //var oQSInstance = oIXI.GetQuestionSetInstanceByID(0, iQSIID, null, 0, 0, "");
                List<XIIBO> oBOIPolicies = new List<XIIBO>();
                string sWhereCondition = " and iStatus!=5 and rPolicyBalance!=0 and dCreated<'" + Convert.ToDateTime(XIValues["dAsOfDate"].sValue).AddDays(1).ToString("dd-MMM-yyyy") + "'";
                XID1Click oXI1Click = new XID1Click();
                oXI1Click.Query = XIConstant.AgeDebtQuery + sWhereCondition + " order by id desc";
                oXI1Click.Name = "ACPolicy_T";
                var oQResult = oXI1Click.GetList();

                //QueryEngine oQE = new QueryEngine();
                // oQResult = oQE.Execute_QueryEngine("ACPolicy_T", "*", sWhereCondition);
                if (oQResult.bOK || oQResult.oResult != null)
                {
                    oBOIPolicies = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.ToList();
                }
                if (oBOIPolicies == null && oBOIPolicies.Count() < 1)
                {
                    oCResult.sMessage = ROUTINE_NAME + ", Unable to get the Policies Collection ";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oCResult.sMessage = ROUTINE_NAME + ", Unable to get the Policies Collection ";
                    oIB.SaveErrortoDB(oCResult);
                }
                List<XIIBO> oAgedDebts = new List<XIIBO>();
                XIDXI oXID = new XIDXI();
                XIDBO oBOD = (XIDBO)oXID.Get_BODefinition("ACAgedDebtByDay_T").oResult;
                XIIBO oBOI = new XIIBO();
                foreach (var oPolicy in oBOIPolicies)
                {
                    var res = iBO_getAttrAGEDebtACT(oPolicy.AttributeI("id").iValue, oPolicy.AttributeI("FKiCustomerID").iValue, "_rshort", XIValues);
                    oBOI = new XIIBO();
                    oBOI.BOD = oBOD;
                    oBOI.Attributes["Client"] = new XIIAttribute { sName = "FKiCustomerID", sValue = oPolicy.AttributeI("Client").sValue, bDirty = true };
                    oBOI.Attributes["Supplier"] = new XIIAttribute { sName = "FKiSupplierID", sValue = oPolicy.AttributeI("Supplier").sValue, bDirty = true };
                    oBOI.Attributes["Status"] = new XIIAttribute { sName = "iStatus", sValue = oPolicy.AttributeI("Status").sValue, bDirty = true };
                    oBOI.Attributes["Type"] = new XIIAttribute { sName = "iType", sValue = "", bDirty = true };
                    //oBOI.Attributes["FKiOrgID"] = new XIIAttribute { sName = "FKiOrgID", sValue = oPolicy.AttributeI("FKiOrgID").sValue, bDirty = true };
                    oBOI.Attributes["Date"] = new XIIAttribute { sName = "dDate", sValue = oPolicy.AttributeI("dCoverStart").sValue == "" ? "" : Convert.ToDateTime(oPolicy.AttributeI("dCoverStart").sValue).ToString("dd-MM-yyyy"), bDirty = true };
                    oBOI.Attributes["Short"] = new XIIAttribute { sName = "rShort", sValue = res["short"].ToString(), bDirty = true };
                    oBOI.Attributes["Medium"] = new XIIAttribute { sName = "rMedium", sValue = res["medium"].ToString(), bDirty = true };
                    oBOI.Attributes["Long"] = new XIIAttribute { sName = "rLong", sValue = res["long"].ToString(), bDirty = true };
                    oBOI.Attributes["Long+"] = new XIIAttribute { sName = "rLongPlus", sValue = res["longplus"].ToString(), bDirty = true };
                    oBOI.Attributes["Balance"] = new XIIAttribute { sName = "rTotal", sValue = oPolicy.AttributeI("rPolicyBalance").sValue, bDirty = true };
                    oBOI.Attributes["Policy"] = new XIIAttribute { sName = "FKiACPolicyID", sValue = oPolicy.AttributeI("Policy").sValue, bDirty = true };
                    oBOI.Attributes["Enterprise"] = new XIIAttribute { sName = "FKiEnterpriseID", sValue = oPolicy.AttributeI("Enterprise").sValue, bDirty = true };
                    oBOI.Attributes["Account Category"] = new XIIAttribute { sName = "refAccountCategory", sValue = oPolicy.AttributeI("AccountCategory").sValue, bDirty = true };
                    oBOI.Attributes["Name"] = new XIIAttribute { sName = "sName", sValue = oPolicy.AttributeI("sName").sValue, bDirty = true };
                    oAgedDebts.Add(oBOI);
                }
                if (oBOIPolicies.Count() > 0)
                {
                    var Short = oAgedDebts.Select(x => x.AttributeI("Short").doValue).Sum();
                    var Medium = oAgedDebts.Select(x => x.AttributeI("Medium").doValue).Sum();
                    var Long = oAgedDebts.Select(x => x.AttributeI("Long").doValue).Sum();
                    var LongPlus = oAgedDebts.Select(x => x.AttributeI("Long+").doValue).Sum();
                    var Balance = oAgedDebts.Select(x => x.AttributeI("Balance").doValue).Sum();
                    oBOI = new XIIBO();
                    oBOI.BOD = oBOD;
                    oBOI.Attributes["Client"] = new XIIAttribute { sName = "FKiCustomerID", sValue = "", bDirty = true };
                    oBOI.Attributes["Supplier"] = new XIIAttribute { sName = "FKiSupplierID", sValue = "", bDirty = true };
                    oBOI.Attributes["Status"] = new XIIAttribute { sName = "iStatus", sValue = "", bDirty = true };
                    oBOI.Attributes["Type"] = new XIIAttribute { sName = "iType", sValue = "Aged Debt", bDirty = true };
                    oBOI.Attributes["Date"] = new XIIAttribute { sName = "dDate", sValue = "", bDirty = true };
                    oBOI.Attributes["Short"] = new XIIAttribute { sName = "rShort", sValue = Short.ToString(), bDirty = true };
                    oBOI.Attributes["Medium"] = new XIIAttribute { sName = "rMedium", sValue = Medium.ToString(), bDirty = true };
                    oBOI.Attributes["Long"] = new XIIAttribute { sName = "rLong", sValue = Long.ToString(), bDirty = true };
                    oBOI.Attributes["Long+"] = new XIIAttribute { sName = "rLongPlus", sValue = LongPlus.ToString(), bDirty = true };
                    oBOI.Attributes["Balance"] = new XIIAttribute { sName = "rTotal", sValue = Math.Round(Balance, 2).ToString(), bDirty = true };
                    oBOI.Attributes["Policy"] = new XIIAttribute { sName = "FKiACPolicyID", sValue = "", bDirty = true };
                    oBOI.Attributes["Enterprise"] = new XIIAttribute { sName = "FKiEnterpriseID", sValue = "", bDirty = true };
                    oBOI.Attributes["Account Category"] = new XIIAttribute { sName = "refAccountCategory", sValue = "", bDirty = true };
                    oBOI.Attributes["Name"] = new XIIAttribute { sName = "sName", sValue = "", bDirty = true };
                    oAgedDebts.Add(oBOI);
                }
                oCResult.oResult = oAgedDebts;
                //XIIBO xibulk = new XIIBO();
                //DataTable dtbulk = xibulk.MakeBulkSqlTable(oAgedDebts);
                //var resp = xibulk.SaveBulk(dtbulk, oAgedDebts[0].BOD.iDataSource, oAgedDebts[0].BOD.TableName);
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message;
                oIB.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
        public Dictionary<string, double> iBO_getAttrAGEDebtACT(int PolicyID, int CustomerID, string pstrAttr, Dictionary<string, XIIValue> XIIValues)
        {
            CResult oCResult = new CResult();
            double rAgedDebt;
            int iDayRangeFrom = 0;
            int iDayRangeTo = 0;
            DateTime dDateFrom = DateTime.Now;
            DateTime dDateTo = DateTime.Now;
            XIIBO oClaim = new XIIBO();
            CResult tRtn = new CResult();
            int iSelect = 0;
            int iShort = 0;
            int iMedium = 0;
            int iLong = 0;
            bool bOverride = false;
            Dictionary<string, XIIBO> oColClaim = new Dictionary<string, XIIBO>();
            XIIBO oDriver = new XIIBO();
            XIIBO oE = new XIIBO();
            Dictionary<string, XIIBO> oColE = new Dictionary<string, XIIBO>();
            double rAgedDebtCR = 0;
            double rAgedDebtDR = 0;
            XIIBO oClient = new XIIBO();
            int iADType = 0;
            XIIBO oJE = new XIIBO();
            long iAgedDebtType;
            XIIBO oRelationship = new XIIBO();
            DateTime dDateAsOf = DateTime.Now;
            string sDateAsOf;
            double rShort = 0;
            double rMedium = 0;
            double rLong = 0;
            double rLongPlus = 0;
            long j;
            string ROUTINE_NAME = "iBO_getAttr";
            XIDXI oXID = new XIDXI();
            XIDBO oBOD = new XIDBO();
            XIIXI oIXI = new XIIXI();
            XIInstanceBase oIB = new XIInstanceBase();
            QueryEngine oQE = new QueryEngine();
            CResult oQResult = new CResult();
            XIInfraCache oCache = new XIInfraCache();
            Dictionary<string, double> oDictionary = new Dictionary<string, double>();
            oDictionary.Add("short", 0);
            oDictionary.Add("medium", 0);
            oDictionary.Add("long", 0);
            oDictionary.Add("longplus", 0);
            var JournalEntryClick = new Dictionary<string, XIIBO>();
            string FinalQuery = "select * from (";
            try
            {
                switch (pstrAttr.ToLower())
                {
                    //case "_sendorsements":
                    //    sWhere = XIConstant.Key_XIDeleted+"!=40,bApply=1,FKiACPolicyID=" + oPolicy.AttributeI("id").sValue;
                    //    if (oCResult.bOK && oCResult.oResult != null)
                    //    {
                    //        oQResult = oQE.Execute_QueryEngine("Term_T", "id,sName", sWhere);
                    //        if (oQResult.bOK && oQResult.oResult != null)
                    //        {
                    //            oColE = (Dictionary<string, XIIBO>)oQResult.oResult;
                    //        }
                    //    }
                    //    if (oColE == null)
                    //    {
                    //        oCResult.sMessage = oCResult.sClassName + ", " + ROUTINE_NAME + ", Unable to DB2Collection oColE";
                    //        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    //        oCResult.LogToFile();
                    //        oCResult.sMessage = oCResult.sClassName + ", " + ROUTINE_NAME + ", Unable to DB2Collection oColE";
                    //        oIB.SaveErrortoDB(oCResult);
                    //        return oCResult;
                    //    }
                    //    sConcat = "";
                    //    foreach (var item in oColE)
                    //    {
                    //        if (sConcat != "")
                    //        {
                    //            sConcat = sConcat + ", ";
                    //        }
                    //        sConcat = sConcat + item.Value.AttributeI("sName").sValue;
                    //    }
                    //    pstrAttr = sConcat;
                    //    break;
                    //case "_sriskoccupation":
                    //    sWhere = "sValue=P";
                    //    oQResult = oQE.Execute_QueryEngine("enumRelationship_T", "*", sWhere);
                    //    if (oQResult.bOK && oQResult.oResult != null)
                    //    {
                    //        var result = (Dictionary<string, XIIBO>)oQResult.oResult;
                    //        oRelationship = result.FirstOrDefault().Value;
                    //    }
                    //    if (oRelationship != null)
                    //    {
                    //        sWhere = XIConstant.Key_XIDeleted+"!=40,FKiPolicyID=" + oPolicy.AttributeI("id").sValue + ",enumRelationship=" + oRelationship.AttributeI("id").sValue;
                    //        oQResult = oQE.Execute_QueryEngine("Driver_T", "*", sWhere);
                    //        if (oQResult.bOK && oQResult.oResult != null)
                    //        {
                    //            tRtn = oQResult;
                    //        }
                    //        if (tRtn.bOK == true)
                    //        {
                    //            sDriverConcat = oDriver.AttributeI("enumProfessionMainID").sValue;
                    //            pstrAttr = sDriverConcat;
                    //        }
                    //    }
                    //    break;
                    //case "sdelegatedplain":
                    //    pstrAttr = oPolicy.AttributeI("sDelegatedCode").sValue;
                    //    break;
                    //case "_claimtotalcost":
                    //    sWhere = XIConstant.Key_XIDeleted+"<40,FKiACPolicyID" + oPolicy.AttributeI("id").sValue;
                    //    oQResult = oQE.Execute_QueryEngine("Claim_T", "id,rTotalClaimCost", sWhere);
                    //    if (oQResult.bOK && oQResult.oResult != null)
                    //    {
                    //        oColClaim = (Dictionary<string, XIIBO>)oQResult.oResult;
                    //    }
                    //    if (oColClaim == null)
                    //    {
                    //        oCResult.sMessage = oCResult.sClassName + ", " + ROUTINE_NAME + ", Unable to DB2Collection ";
                    //        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    //        oCResult.LogToFile();
                    //        oCResult.sMessage = oCResult.sClassName + ", " + ROUTINE_NAME + ", Unable to DB2Collection ";
                    //        oIB.SaveErrortoDB(oCResult);
                    //    }

                    //    foreach (var item in oColClaim)
                    //    {
                    //        rTotalCost = rTotalCost + item.Value.AttributeI("rTotalClaimCost").doValue;
                    //    }
                    //    pstrAttr = rTotalCost.ToString();
                    //    break;
                    //case "_claimcountfault":
                    //    sWhere = XIConstant.Key_XIDeleted+"!=40,WhoseFault>9,FKiACPolicyID" + oPolicy.AttributeI("id").sValue;
                    //    oQResult = oQE.Execute_QueryEngine("Claim_T", "*", sWhere);
                    //    if (oQResult.bOK && oQResult.oResult != null)
                    //    {
                    //        var result = (Dictionary<string, XIIBO>)oQResult.oResult;
                    //        iClaimCount = result.Count;
                    //    }
                    //    pstrAttr = iClaimCount.ToString();
                    //    break;
                    //case "_vehiclecount":
                    //    sWhere = XIConstant.Key_XIDeleted+"!=40,FKiACPolicyID" + oPolicy.AttributeI("id").sValue;
                    //    oQResult = oQE.Execute_QueryEngine("Vehicle_T", "*", sWhere);
                    //    if (oQResult.bOK && oQResult.oResult != null)
                    //    {
                    //        var result = (Dictionary<string, XIIBO>)oQResult.oResult;
                    //        iCount = result.Count;
                    //    }
                    //    pstrAttr = iCount.ToString();
                    //    break;
                    //case "_ilockstatus":
                    //    if (oPolicy.AttributeI("id").iValue > 0)
                    //    {
                    //        iLockStatus = oPolicy.AttributeI(XIConstant.Key_XIDeleted).iValue;
                    //        //          Set oPolDB = i.zX.BOS.quickLoad("AC/ACPolicy", i.GetAttr("id"), , XIConstant.Key_XIDeleted)
                    //        //          If Not oPolDB Is Nothing Then
                    //        //             iLockStatus = oPolDB.GetAttr(XIConstant.Key_XIDeleted).lngValue
                    //        // 
                    //        if (iLockStatus == 0)
                    //        {
                    //            if (oPolicy.AttributeI("iStatus").iValue == 200)
                    //            {
                    //                iLockStatus = 10;
                    //            }
                    //            else if (oPolicy.AttributeI("iStatus").iValue == 80 || oPolicy.AttributeI("iStatus").iValue == 40)
                    //            {
                    //                // allow up to end of pol
                    //                iLockStatus = 10;
                    //                // default
                    //                sCoverEnd = oPolicy.AttributeI("dCoverEnd").sValue + " " + oPolicy.AttributeI("tCoverStart").sValue;
                    //                if (sCoverEnd != null)//check IsDate or not
                    //                {
                    //                    dtCoverEnd = DateTime.Parse(sCoverEnd);
                    //                    if (dtCoverEnd > DateTime.Now)
                    //                    {
                    //                        iLockStatus = 5;
                    //                    }
                    //                }
                    //            }
                    //            else if (i.zX.userProfile.userInGroup("ADMINEDIT") == false)
                    //            {
                    //                iLockStatus = 5;
                    //            }
                    //        }
                    //        //          End If   'if not oPolDB is nothing then
                    //    }
                    //    pstrAttr = iLockStatus.ToString();
                    //    // 2012.04.16 Case "_rshort", "_rmedium", "_rlong", "_rlongplus", "_radbalance"
                    //    break;
                    case "_rshort":
                        iSelect = Convert.ToInt32(XIIValues["oSelect"].sValue == "" ? "0" : XIIValues["oSelect"].sValue);
                        //iSelect = double.Parse(i.zX.quickContext.getEntry("-ad.Select"));
                        if (iSelect > 0)
                        {
                            switch (iSelect)
                            {
                                case 10:
                                    // 15, 30, 45
                                    iShort = 15;
                                    iMedium = 30;
                                    iLong = 45;
                                    bOverride = true;
                                    break;
                                case 20:
                                    // 30, 60, 90
                                    iShort = 30;
                                    iMedium = 60;
                                    iLong = 90;
                                    bOverride = true;
                                    break;
                                case 30:
                                    // 45, 90, 135
                                    iShort = 45;
                                    iMedium = 90;
                                    iLong = 135;
                                    bOverride = true;
                                    break;
                                case 40:
                                    // 60, 120, 180
                                    iShort = 60;
                                    iMedium = 120;
                                    iLong = 180;
                                    bOverride = true;
                                    break;
                            }
                        }
                        else
                        {
                            // DS: We have a logic problem here - on the search you want to specify the amounts, whereas on the popup you want to override, so on popup make sure you specify the std option "-ad.Select"=10
                            if (iSelect == 0)
                            //if (double.Parse(i.zX.quickContext.getEntry("-ad.Short")) == 0)
                            {
                                iShort = 15;
                                iMedium = 30;
                                iLong = 45;
                                bOverride = true;
                            }
                            else
                            {
                                bOverride = false;
                            }
                        }
                        sDateAsOf = XIIValues["dAsOfDate"].sValue;
                        //sDateAsOf = i.zX.quickContext.getEntry("-date.AsOf");
                        if (sDateAsOf != "")
                        {
                            //if (IsDate(sDateAsOf))
                            //{
                            dDateAsOf = DateTime.Parse(sDateAsOf);
                            //}
                        }
                        for (j = 1; j <= 4; j++)
                        {
                            switch (j)
                            {
                                case 4:
                                    // "_rshort"
                                    iDayRangeFrom = 0;
                                    if (bOverride)
                                    {
                                        iDayRangeTo = iShort;
                                    }
                                    else
                                    {
                                        iDayRangeTo = Convert.ToInt32(XIIValues["iShort"].sValue);
                                    }

                                    if (iDayRangeTo == 0)
                                    {
                                        iDayRangeTo = 29;
                                    }

                                    iDayRangeTo = iDayRangeTo * -1;
                                    dDateFrom = dDateAsOf.AddDays(iDayRangeFrom);
                                    dDateTo = dDateAsOf.AddDays(iDayRangeTo);
                                    break;
                                case 3:
                                    // "_rmedium"
                                    if (bOverride)
                                    {
                                        iDayRangeFrom = iShort;
                                    }
                                    else
                                    {
                                        iDayRangeFrom = Convert.ToInt32(XIIValues["iShort"].sValue);
                                    }

                                    if (iDayRangeFrom == 0)
                                    {
                                        iDayRangeFrom = 30;
                                    }

                                    iDayRangeFrom = (iDayRangeFrom * -1);
                                    if (bOverride)
                                    {
                                        iDayRangeTo = iMedium;
                                    }
                                    else
                                    {
                                        iDayRangeTo = Convert.ToInt32(XIIValues["iMedium"].sValue);
                                    }

                                    if (iDayRangeTo == 0)
                                    {
                                        iDayRangeTo = 59;
                                    }

                                    iDayRangeTo = iDayRangeTo * -1;
                                    dDateFrom = dDateAsOf.AddDays(iDayRangeFrom);
                                    dDateTo = dDateAsOf.AddDays(iDayRangeTo);
                                    break;
                                case 2:
                                    // "_rlong"
                                    if (bOverride)
                                    {
                                        iDayRangeFrom = iMedium;
                                    }
                                    else
                                    {
                                        iDayRangeFrom = Convert.ToInt32(XIIValues["iMedium"].sValue);
                                    }
                                    if (iDayRangeFrom == 0)
                                    {
                                        iDayRangeFrom = 60;
                                    }
                                    iDayRangeFrom = iDayRangeFrom * -1;
                                    if (bOverride)
                                    {
                                        iDayRangeTo = iLong;
                                    }
                                    else
                                    {
                                        iDayRangeTo = Convert.ToInt32(XIIValues["iLong"].sValue);
                                    }

                                    if (iDayRangeTo == 0)
                                    {
                                        iDayRangeTo = 89;
                                    }
                                    iDayRangeTo = iDayRangeTo * -1;
                                    dDateFrom = dDateAsOf.AddDays(iDayRangeFrom);
                                    dDateTo = dDateAsOf.AddDays(iDayRangeTo);
                                    break;
                                case 1:
                                    // "_rlongplus"
                                    // get the days from the querystring
                                    if (bOverride)
                                    {
                                        iDayRangeFrom = iLong;
                                    }
                                    else
                                    {
                                        iDayRangeFrom = Convert.ToInt32(XIIValues["iLong"].sValue);
                                    }
                                    if (iDayRangeFrom == 0)
                                    {
                                        iDayRangeFrom = 90;
                                    }
                                    iDayRangeFrom = iDayRangeFrom * -1;
                                    iDayRangeTo = -40000;
                                    //  i think 1899 is earliest year
                                    dDateFrom = dDateAsOf.AddDays(iDayRangeFrom);
                                    dDateTo = dDateAsOf.AddDays(iDayRangeTo);
                                    break;
                                    //case "_radbalance":
                                    //    iDayRangeFrom = 0;
                                    //    iDayRangeTo = -40000;
                                    //    //  i think 1899 is earliest year
                                    //    dDateFrom = dDateAsOf.AddDays(iDayRangeFrom);
                                    //    dDateTo = dDateAsOf.AddDays(iDayRangeTo);
                                    //    break;
                            }
                            // DS 06.06.09 - i think this has to be changed to 2 transactions where the sum is done twice - once for CR and once for DR
                            // DS 06.06.09 - as of now not from DB - why? Well maybe due to not having fixed short /med/long day ranges
                            // On Error Resume Next
                            iADType = 0;
                            //iADType = double.Parse(i.zX.quickContext.getEntry("-ADType"));
                            if (iADType == 0)
                            {
                                oClient = oIXI.BOI("Customer_T", CustomerID.ToString(), "id,FKiACAccountID");
                                if (oClient == null)
                                {
                                    oCResult.sMessage = oCResult.sClassName + " - " + ROUTINE_NAME + " - CANNOT LOAD CLIENT";
                                }
                            }
                            else if (iADType == 10)
                            {
                                int FKiSupplierID = 0;
                                // even though should be oSupplier, keep oClient so code below is same for both
                                oClient = oIXI.BOI("Supplier_T", FKiSupplierID.ToString(), "id,FKiACAccountID");
                                if (oClient == null)
                                {
                                    oCResult.sMessage = oCResult.sClassName + " - " + ROUTINE_NAME + " - CANNOT LOAD SUPPLIER";
                                }
                            }

                            // DS 12.06.09 this doesn't work as the DR and CR accounts are not set on the transaction (it doesn't know them as it is the journals that hold this info)
                            // '      Set oTransaction = i.zX.createBO("AC/ACTransaction")
                            // '      'tRtn = i.zX.BOS.selectSum(oTransaction, "zBaseValue", ":dWhen>=#" & Format(dDateTo, "dd mmm yyyy") & "#&dWhen<#" & Format(dDateFrom, "dd mmm yyyy") & "#" & "&FKiACPolicyID=" & i.GetAttr("id").strValue & "iStatus=0")  'CHANGE TO BALANCE
                            // '      tRtn = i.zX.BOS.selectSum(oTransaction, "zBaseValue", ":dWhen>=#" & Format(dDateTo, "dd mmm yyyy") & "#&dWhen<#" & Format(dDateFrom, "dd mmm yyyy") & "#" & "&FKiCRAccountID=" & oClient.GetAttr("FKiACAccountID").strValue & "&iStatus=0")
                            // '
                            // '      If tRtn = rcOk Then
                            // '         rAgedDebtCR = oTransaction.GetAttr("zBaseValue").dblValue
                            // '      End If
                            // '      Set oTransaction = i.zX.createBO("AC/ACTransaction")
                            // '
                            // '      tRtn = i.zX.BOS.selectSum(oTransaction, "zBaseValue", ":dWhen>=#" & Format(dDateTo, "dd mmm yyyy") & "#&dWhen<#" & Format(dDateFrom, "dd mmm yyyy") & "#" & "&FKiDRAccountID=" & oClient.GetAttr("FKiACAccountID").strValue & "&iStatus=0")
                            // '
                            // '      If tRtn = rcOk Then
                            // '         rAgedDebtDR = oTransaction.GetAttr("zBaseValue").dblValue
                            // '      End If
                            // '
                            // '
                            // '      If tRtn = rcOk Then
                            // '         rAgedDebt = rAgedDebtCR - rAgedDebtDR 'oTransaction.GetAttr("zBaseValue").dblValue
                            // '         i.zX.BOS.SetAttr Me, pstrAttr, i.zX.dblValue(rAgedDebt)
                            // '      End If
                            // DS 12.06.09 - change to journal to hold policy allows exact journal tracking
                            // -ADRptType - if this is 0 then normal aged debt. If 10 then premium finance aged debt
                            iAgedDebtType = 0;
                            //iAgedDebtType = i.GetAttr("_ADType").lngValue;
                            //if (i.zX.quickContext.getEntry("-ADRptType") != "")
                            //{
                            //    iAgedDebtType = double.Parse(i.zX.quickContext.getEntry("-ADRptType"));
                            //}

                            //oJE = i.zX.createBO("AC/ACJournalEntry");
                            // - date is in the appropriate range (eg. short day range)
                            // - the account no is the client (or supplier) account
                            // - journals are for this policy
                            // - journal status is live (ie this is not a corrected JE)
                            // 2011.02.01 Changed because the from and to dates can overlap
                            // 2010.04.29 - remove status restriction as corrected should zeroise and replacement should be counted & "&iStatus=0"
                            // Status of journal not working properly so we have to get the transaction

                            string Query = "";
                            Query = " SELECT " + j + " as 'Debt',[ACJournalEntry_T].[iType],SUM([ACJournalEntry_T].[rAmount]) as rAmount FROM [ACJournalEntry_T] inner join [ACTransaction_T] on [ACJournalEntry_T].FKiTransactionID=[ACTransaction_T].id";
                            Query += " WHERE(1 = 1) AND cast([ACJournalEntry_T].[dDate] as date) > '" + dDateTo.ToString("yyyy-MM-dd") + "' AND cast([ACJournalEntry_T].[dDate] as date) <= '" + dDateFrom.ToString("yyyy-MM-dd") + "'";
                            Query += " AND [ACJournalEntry_T].[FKiAccountID] = " + oClient.AttributeI("FKiACAccountID").sValue;
                            Query += " AND ([ACJournalEntry_T].[iStatus] = 0 OR [ACJournalEntry_T].[iStatus] = 30)";
                            Query += " AND [ACJournalEntry_T].[FKiACPolicyID] = " + PolicyID + " AND ([ACTransaction_T].[id] = [ACJournalEntry_T].[FKiTransactionID])";
                            Query += " AND ([ACTransaction_T].[iStatus] = 0 OR [ACTransaction_T].[iStatus] = 30) AND [ACTransaction_T].["+ XIConstant.Key_XIDeleted + "] = 0";
                            if (iAgedDebtType == 10)
                            {
                                // PF only
                                Query += " AND [ACJournalEntry_T].[iTransType] = 200";
                            }
                            Query += " Group by [ACJournalEntry_T].[iType]";

                            //JournalEntryClick = new Dictionary<string, XIIBO>();
                            //var iDataSources = ((XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "ACJournalEntry_T", null)).iDataSource;
                            //var DataSources = oXID.GetBODataSource(iDataSources);
                            //XID1Click JournalEntry1Clicks = new XID1Click();
                            //var JournalEntryQuerys = Query + sADWhere;
                            //JournalEntry1Clicks.sConnectionString = DataSources;
                            //JournalEntry1Clicks.Query = JournalEntryQuerys.ToString();
                            //JournalEntry1Clicks.Name = "ACJournalEntry_T";
                            //JournalEntryClick = JournalEntry1Clicks.OneClick_Execute();
                            //oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            //oCResult.oResult = JournalEntryClick.Values;
                            //tRtn = oCResult;
                            //if (tRtn.bOK == true)
                            //{
                            //    rAgedDebtCR = JournalEntryClick.Values.FirstOrDefault().AttributeI("rAmount").doValue;
                            //}
                            //sADWhere = " AND [ACJournalEntry_T].[iType] = 20";
                            //JournalEntry1Clicks = new XID1Click();
                            //JournalEntryQuerys = Query + sADWhere;
                            //JournalEntry1Clicks.sConnectionString = DataSources;
                            //JournalEntry1Clicks.Query = JournalEntryQuerys.ToString();
                            //JournalEntry1Clicks.Name = "ACJournalEntry_T";
                            //JournalEntryClick = JournalEntry1Clicks.OneClick_Execute();
                            //oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            //oCResult.oResult = JournalEntryClick.Values;
                            //tRtn = oCResult;
                            //if (tRtn.bOK == true)
                            //{
                            //    rAgedDebtDR = JournalEntryClick.Values.FirstOrDefault().AttributeI("rAmount").doValue;
                            //}
                            FinalQuery += Query + " Union";
                        }
                        FinalQuery = FinalQuery.Substring(0, FinalQuery.Length - 5);
                        FinalQuery = FinalQuery + ")AS SourceTable PIVOT  (sum(ramount) FOR iType IN ([10],[20])) AS PivotTable";
                        XID1Click oXI1Click = new XID1Click();
                        oXI1Click.Query = FinalQuery;
                        oXI1Click.Name = "ACJournalEntry_T";
                        var Result = oXI1Click.Execute_Query();
                        if (Result.Rows.Count > 0)
                        {
                            for (j = 1; j <= 4; j++) //10 Dr & 20 Cr
                            {
                                var res = Result.Select("Debt=" + j).FirstOrDefault();
                                if (res == null)
                                {
                                    rAgedDebtCR = 0;
                                    rAgedDebtDR = 0;
                                }
                                else
                                {
                                    rAgedDebtCR = Convert.ToDouble(res.ItemArray[1] == DBNull.Value ? 0 : res.ItemArray[1]);
                                    rAgedDebtDR = Convert.ToDouble(res.ItemArray[2] == DBNull.Value ? 0 : res.ItemArray[2]);
                                }
                                rAgedDebt = rAgedDebtCR - rAgedDebtDR;
                                // 2012.04.16 - ok PROBLEM is we do not know if short is executed before medium etc
                                //    possible solution to that is to change the system so that all 3 are calculated at the same time and then assigned. so this getattr would only trigger on say _short, the others would be automatically calculated
                                switch (j)
                                {
                                    case 4:
                                        rShort = rAgedDebt;
                                        break;
                                    case 3:
                                        rMedium = rAgedDebt;
                                        break;
                                    case 2:
                                        rLong = rAgedDebt;
                                        break;
                                    case 1:
                                        rLongPlus = rAgedDebt;
                                        break;
                                }
                                //             i.zX.BOS.SetAttr Me, pstrAttr, i.zX.dblValue(rAgedDebt)
                            }
                        }

                        // OK, now compare long plus all the way back to short and stack up in reverse in each case. Not just one step at a time, but re-check the whole sequence (for example long may be �90, medium zero and short �30, so that would mean that
                        //   long is �60 and medium and short zero when done
                        for (j = 2; j <= 4; j++)
                        {
                            // not 1 to 4 as for long plus there is no figure to adjust
                            switch (j)
                            {
                                case 2:
                                    // long
                                    if ((rLong > 0 && rLongPlus < 0) || (rLong < 0 && rLongPlus > 0))
                                    {
                                        if (Math.Abs(rLongPlus) > Math.Abs(rLong))
                                        {
                                            rLongPlus = rLongPlus + rLong;
                                            rLong = 0;
                                        }
                                        else
                                        {
                                            rLong = rLong + rLongPlus;
                                            rLongPlus = 0;
                                        }
                                    }
                                    break;
                                case 3:
                                    // medium
                                    if ((rMedium > 0 && rLongPlus < 0) || (rMedium < 0 && rLongPlus > 0))
                                    {
                                        if (Math.Abs(rLongPlus) > Math.Abs(rMedium))
                                        {
                                            rLongPlus = rLongPlus + rMedium;
                                            rMedium = 0;
                                        }
                                        else
                                        {
                                            rMedium = rMedium + rLongPlus;
                                            rLongPlus = 0;
                                        }

                                    }

                                    if ((rMedium > 0 && rLong < 0) || (rMedium < 0 && rLong > 0))
                                    {
                                        if (Math.Abs(rLong) > Math.Abs(rMedium))
                                        {
                                            rLong = rLong + rMedium;
                                            rMedium = 0;
                                        }
                                        else
                                        {
                                            rMedium = rMedium + rLong;
                                            rLong = 0;
                                        }
                                    }
                                    break;
                                case 4:
                                    // short
                                    if ((rShort > 0 && rLongPlus < 0) || (rShort < 0 && rLongPlus > 0))
                                    {
                                        if (Math.Abs(rLongPlus) > Math.Abs(rShort))
                                        {
                                            rLongPlus = rLongPlus + rShort;
                                            rShort = 0;
                                        }
                                        else
                                        {
                                            rShort = rShort + rLongPlus;
                                            rLongPlus = 0;
                                        }
                                    }

                                    if ((rShort > 0 && rLong < 0) || (rShort < 0 && rLong > 0))
                                    {
                                        if (Math.Abs(rLong) > Math.Abs(rShort))
                                        {
                                            rLong = rLong + rShort;
                                            rShort = 0;
                                        }
                                        else
                                        {
                                            rShort = rShort + rLong;
                                            rLong = 0;
                                        }
                                    }

                                    if ((rShort > 0 && rMedium < 0) || (rShort < 0 && rMedium > 0))
                                    {
                                        if (Math.Abs(rMedium) > Math.Abs(rShort))
                                        {
                                            rMedium = rMedium + rShort;
                                            rShort = 0;
                                        }
                                        else
                                        {
                                            rShort = rShort + rMedium;
                                            rMedium = 0;
                                        }
                                    }

                                    break;
                            }
                        }
                        //var _rShort = rShort.ToString();
                        //var _rMedium = rMedium.ToString();
                        //var _rLong = rLong.ToString();
                        //var _rLongPlus = rLongPlus.ToString();

                        oDictionary["short"] = Math.Round(rShort, 2);
                        oDictionary["medium"] = Math.Round(rMedium, 2);
                        oDictionary["long"] = Math.Round(rLong, 2);
                        oDictionary["longplus"] = Math.Round(rLongPlus, 2);
                        //i.SetAttr("_rShort", rShort.ToString());
                        //i.SetAttr("_rMedium", rMedium);
                        //i.SetAttr("_rLong", rLong);
                        //i.SetAttr("_rLongPlus", i.zX.dblValue(rLongPlus);
                        //2012.04.16 - everthing after this line was added back in from the original code JUST to handle _radbalance. BUT it is the original code, so easy to put back
                        break;
                        //case "_radbalance":
                        //    iSelect = double.Parse(i.zX.quickContext.getEntry("-ad.Select"));
                        //    if ((iSelect > 0))
                        //    {
                        //        switch (iSelect)
                        //        {
                        //            case 10:
                        //                // 15, 30, 45
                        //                iShort = 15;
                        //                iMedium = 30;
                        //                iLong = 45;
                        //                bOverride = true;
                        //                break;
                        //            case 20:
                        //                // 30, 60, 90
                        //                iShort = 30;
                        //                iMedium = 60;
                        //                iLong = 90;
                        //                bOverride = true;
                        //                break;
                        //            case 30:
                        //                // 45, 90, 135
                        //                iShort = 45;
                        //                iMedium = 90;
                        //                iLong = 135;
                        //                bOverride = true;
                        //                break;
                        //            case 40:
                        //                // 60, 120, 180
                        //                iShort = 60;
                        //                iMedium = 120;
                        //                iLong = 180;
                        //                bOverride = true;
                        //                break;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        // DS: We have a logic problem here - on the search you want to specify the amounts, whereas on the popup you want to override, so on popup make sure you specify the std option "-ad.Select"=10
                        //        if (double.Parse(i.zX.quickContext.getEntry("-ad.Short") == 0))
                        //        {
                        //            iShort = 15;
                        //            iMedium = 30;
                        //            iLong = 45;
                        //            bOverride = true;
                        //        }
                        //        else
                        //        {
                        //            bOverride = false;
                        //        }

                        //    }

                        //    sDateAsOf = i.zX.quickContext.getEntry("-date.AsOf");
                        //    if (sDateAsOf != "")
                        //    {
                        //        if (sDateAsOf === null)
                        //        {
                        //            dDateAsOf = DateTime.Parse(sDateAsOf);
                        //        }

                        //    }

                        //    switch (pstrAttr.ToLower())
                        //    {
                        //        case "_rshort":
                        //            iDayRangeFrom = 0;
                        //            if (bOverride)
                        //            {
                        //                iDayRangeTo = iShort;
                        //            }
                        //            else
                        //            {
                        //                iDayRangeTo = double.Parse(i.zX.quickContext.getEntry("-ad.Short"));
                        //            }

                        //            if (iDayRangeTo == 0)
                        //            {
                        //                iDayRangeTo = 29;
                        //            }

                        //            iDayRangeTo = (iDayRangeTo * -1);
                        //            dDateFrom = dDateAsOf.AddDays(iDayRangeFrom);
                        //            dDateTo = dDateAsOf.AddDays(iDayRangeTo);
                        //            break;
                        //        case "_rmedium":
                        //            if (bOverride)
                        //            {
                        //                iDayRangeFrom = iShort;
                        //            }
                        //            else
                        //            {
                        //                iDayRangeFrom = double.Parse(i.zX.quickContext.getEntry("-ad.Short"));
                        //            }

                        //            if ((iDayRangeFrom == 0))
                        //            {
                        //                iDayRangeFrom = 30;
                        //            }

                        //            iDayRangeFrom = (iDayRangeFrom * -1);
                        //            if (bOverride)
                        //            {
                        //                iDayRangeTo = iMedium;
                        //            }
                        //            else
                        //            {
                        //                iDayRangeTo = double.Parse(i.zX.quickContext.getEntry("-ad.Medium"));
                        //            }

                        //            if (iDayRangeTo == 0)
                        //            {
                        //                iDayRangeTo = 59;
                        //            }

                        //            iDayRangeTo = (iDayRangeTo * -1);
                        //            dDateFrom = dDateAsOf.AddDays(iDayRangeFrom);
                        //            dDateTo = dDateAsOf.AddDays(iDayRangeTo);
                        //            break;
                        //        case "_rlong":
                        //            if (bOverride)
                        //            {
                        //                iDayRangeFrom = iMedium;
                        //            }
                        //            else
                        //            {
                        //                iDayRangeFrom = double.Parse(i.zX.quickContext.getEntry("-ad.Medium"));
                        //            }

                        //            if (iDayRangeFrom == 0)
                        //            {
                        //                iDayRangeFrom = 60;
                        //            }

                        //            iDayRangeFrom = (iDayRangeFrom * -1);
                        //            if (bOverride)
                        //            {
                        //                iDayRangeTo = iLong;
                        //            }
                        //            else
                        //            {
                        //                iDayRangeTo = double.Parse(i.zX.quickContext.getEntry("-ad.Long"));
                        //            }

                        //            if (iDayRangeTo == 0)
                        //            {
                        //                iDayRangeTo = 89;
                        //            }

                        //            iDayRangeTo = (iDayRangeTo * -1);
                        //            dDateFrom = dDateAsOf.AddDays(iDayRangeFrom);
                        //            dDateTo = dDateAsOf.AddDays(iDayRangeTo);
                        //            break;
                        //        case "_rlongplus":
                        //            if (bOverride)
                        //            {
                        //                iDayRangeFrom = iLong;
                        //            }
                        //            else
                        //            {
                        //                iDayRangeFrom = double.Parse(i.zX.quickContext.getEntry("-ad.Long"));
                        //            }

                        //            if ((iDayRangeFrom == 0))
                        //            {
                        //                iDayRangeFrom = 90;
                        //            }

                        //            iDayRangeFrom = (iDayRangeFrom * -1);
                        //            iDayRangeTo = -40000;
                        //            //  i think 1899 is earliest year
                        //            dDateFrom = dDateAsOf.AddDays(iDayRangeFrom);
                        //            dDateTo = dDateAsOf.AddDays(iDayRangeTo);
                        //            break;
                        //        case "_radbalance":
                        //            iDayRangeFrom = 0;
                        //            iDayRangeTo = -40000;
                        //            //  i think 1899 is earliest year
                        //            dDateFrom = dDateAsOf.AddDays(iDayRangeFrom);
                        //            dDateTo = dDateAsOf.AddDays(iDayRangeTo);
                        //            break;
                        //    }
                        //    // DS 06.06.09 - i think this has to be changed to 2 transactions where the sum is done twice - once for CR and once for DR
                        //    // DS 06.06.09 - as of now not from DB - why? Well maybe due to not having fixed short /med/long day ranges
                        //    // On Error Resume Next
                        //    iADType = double.Parse(i.zX.quickContext.getEntry("-ADType"));
                        //    if (iADType == 0)
                        //    {
                        //        oClient = i.zX.BOS.quickFKLoad(this, "FKiCustomerID", "id,FKiACAccountID");
                        //        if (oClient == null)
                        //        {
                        //            oCResult.sMessage = oCResult.sClassName + " - " + ROUTINE_NAME + " - CANNOT LOAD CLIENT";
                        //        }

                        //    }
                        //    else if (iADType == 10)
                        //    {
                        //        // even though should be oSupplier, keep oClient so code below is same for both
                        //        oClient = i.zX.BOS.quickFKLoad(this, "FKiSupplierID", "id,FKiACAccountID");
                        //        if ((oClient == null))
                        //        {
                        //            oCResult.sMessage = oCResult.sClassName + " - " + ROUTINE_NAME + " - CANNOT LOAD SUPPLIER";
                        //        }

                        //    }

                        //    // DS 12.06.09 this doesn't work as the DR and CR accounts are not set on the transaction (it doesn't know them as it is the journals that hold this info)
                        //    // '      Set oTransaction = i.zX.createBO("AC/ACTransaction")
                        //    // '      'tRtn = i.zX.BOS.selectSum(oTransaction, "zBaseValue", ":dWhen>=#" & Format(dDateTo, "dd mmm yyyy") & "#&dWhen<#" & Format(dDateFrom, "dd mmm yyyy") & "#" & "&FKiACPolicyID=" & i.GetAttr("id").strValue & "iStatus=0")  'CHANGE TO BALANCE
                        //    // '      tRtn = i.zX.BOS.selectSum(oTransaction, "zBaseValue", ":dWhen>=#" & Format(dDateTo, "dd mmm yyyy") & "#&dWhen<#" & Format(dDateFrom, "dd mmm yyyy") & "#" & "&FKiCRAccountID=" & oClient.GetAttr("FKiACAccountID").strValue & "&iStatus=0")
                        //    // '
                        //    // '      If tRtn = rcOk Then
                        //    // '         rAgedDebtCR = oTransaction.GetAttr("zBaseValue").dblValue
                        //    // '      End If
                        //    // '      Set oTransaction = i.zX.createBO("AC/ACTransaction")
                        //    // '
                        //    // '      tRtn = i.zX.BOS.selectSum(oTransaction, "zBaseValue", ":dWhen>=#" & Format(dDateTo, "dd mmm yyyy") & "#&dWhen<#" & Format(dDateFrom, "dd mmm yyyy") & "#" & "&FKiDRAccountID=" & oClient.GetAttr("FKiACAccountID").strValue & "&iStatus=0")
                        //    // '
                        //    // '      If tRtn = rcOk Then
                        //    // '         rAgedDebtDR = oTransaction.GetAttr("zBaseValue").dblValue
                        //    // '      End If
                        //    // '
                        //    // '
                        //    // '      If tRtn = rcOk Then
                        //    // '         rAgedDebt = rAgedDebtCR - rAgedDebtDR 'oTransaction.GetAttr("zBaseValue").dblValue
                        //    // '         i.zX.BOS.SetAttr Me, pstrAttr, i.zX.dblValue(rAgedDebt)
                        //    // '      End If
                        //    // DS 12.06.09 - change to journal to hold policy allows exact journal tracking
                        //    // -ADRptType - if this is 0 then normal aged debt. If 10 then premium finance aged debt
                        //    iAgedDebtType = i.GetAttr("_ADType").lngValue;
                        //    if (i.zX.quickContext.getEntry("-ADRptType") != "")
                        //    {
                        //        iAgedDebtType = double.Parse(i.zX.quickContext.getEntry("-ADRptType"));
                        //    }

                        //    //oJE = i.zX.createBO("AC/ACJournalEntry");
                        //    // - date is in the appropriate range (eg. short day range)
                        //    // - the account no is the client (or supplier) account
                        //    // - journals are for this policy
                        //    // - journal status is live (ie this is not a corrected JE)
                        //    // 2011.02.01 Changed because the from and to dates can overlap
                        //    sADWhere = new StringBuilder();
                        //    sADWhere.Append(" SELECT SUM([ACJournalEntry_T].[rAmount]) FROM [ACJournalEntry_T] inner join [ACTransaction_T] on [ACJournalEntry_T].FKiTransactionID=[ACTransaction_T].id ");
                        //    sADWhere.Append(" WHERE(1 = 1) AND [ACJournalEntry_T].dDate>" + dDateTo.ToString("yyyy-mm-dd"));
                        //    sADWhere.Append(" AND [ACJournalEntry_T].dDate<=" + dDateFrom.ToString("yyyy-mm-dd") + " AND [ACJournalEntry_T].FKiAccountID=");
                        //    sADWhere.Append(oClient.AttributeI("FKiACAccountID").sValue + " AND ([ACJournalEntry_T].iStatus=0 OR [ACJournalEntry_T].iStatus=30)");
                        //    sADWhere.Append(" AND [ACJournalEntry_T].FKiACPolicyID=" + oPolicy.AttributeI("id").sValue);
                        //    // 2010.04.29 - remove status restriction as corrected should zeroise and replacement should be counted & "&iStatus=0"
                        //    // Status of journal not working properly so we have to get the transaction
                        //    // 24.03.2017 - change made for aged finance
                        //    //       sADWhere = sADWhere & "&#exists[AC/ACTransaction, iStatus=0|istatus=30]"
                        //    // sADWhere = ":dDate>=#" & Format(dDateTo, "dd mmm yyyy") & "#&dDate<=#" & Format(dDateFrom, "dd mmm yyyy") & "#" & "&FKiAccountID=" & oClient.GetAttr("FKiACAccountID").strValue & "&(iStatus=0|iStatus=30)&FKiACPolicyID=" & i.GetAttr("id").strValue  '2010.04.29 - remove status restriction as corrected should zeroise and replacement should be counted & "&iStatus=0"
                        //    if (iAgedDebtType == 10)
                        //    {
                        //        // PF only
                        //        sADWhere.Append(" AND [ACJournalEntry_T].iTransType=200");
                        //        sADWhere.Append(" AND ([ACTransaction_T].iStatus=0 OR [ACTransaction_T].istatus=30) AND ([ACTransaction_T].iPFRec=0 OR [ACTransaction_T].iPFRec=Null)");
                        //    }
                        //    else
                        //    {
                        //        sADWhere.Append(" AND [ACTransaction_T].iStatus=0 OR [ACTransaction_T].istatus=30");
                        //    }
                        //    oCache = new XIInfraCache();
                        //    JournalEntryClick = new Dictionary<string, XIIBO>();
                        //    var iDataSource = ((XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "ACJournalEntry_T", null)).iDataSource;
                        //    var DataSource = oXID.GetBODataSource(iDataSource);
                        //    XID1Click JournalEntry1Click = new XID1Click();
                        //    var JournalEntryQuery = sADWhere + " AND [ACJournalEntry_T].iType=10";
                        //    JournalEntry1Click.sConnectionString = DataSource;
                        //    JournalEntry1Click.Query = JournalEntryQuery.ToString();
                        //    JournalEntry1Click.Name = "ACJournalEntry_T";
                        //    JournalEntryClick = JournalEntry1Click.OneClick_Execute();
                        //    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        //    oCResult.oResult = JournalEntryClick;
                        //    tRtn = oCResult;
                        //    if (tRtn.bOK == true)
                        //    {
                        //        rAgedDebtCR = oJE.AttributeI("rAmount").doValue;
                        //    }
                        //    JournalEntryQuery = sADWhere + " AND [ACJournalEntry_T].iType=20";
                        //    JournalEntryClick = JournalEntry1Click.OneClick_Execute();
                        //    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        //    oCResult.oResult = JournalEntryClick;
                        //    tRtn = oCResult;
                        //    // tRtn = i.zX.BOS.selectSum(oJE, "rAmount", ":dDate>=#" & Format(dDateTo, "dd mmm yyyy") & "#&dDate<=#" & Format(dDateFrom, "dd mmm yyyy") & "#" & "&FKiAccountID=" & oClient.GetAttr("FKiACAccountID").strValue & "&FKiACPolicyID=" & i.GetAttr("id").strValue & "&iType=20&iStatus=0")
                        //    if (tRtn.bOK == true)
                        //    {
                        //        rAgedDebtDR = oJE.AttributeI("rAmount").doValue;
                        //    }

                        //    if (tRtn.bOK == true)
                        //    {
                        //        rAgedDebt = rAgedDebtCR - rAgedDebtDR;
                        //        // 2012.04.16 - ok PROBLEM is we do not know if short is executed before medium etc
                        //        //    possible solution to that is to change the system so that all 3 are calculated at the same time and then assigned. so this getattr would only trigger on say _short, the others would be automatically calculated
                        //        pstrAttr = rAgedDebt.ToString();
                        //    }

                        //    // >>> 2012.04.16
                        //    // DS 12.06.09 - back to old method of relying on the positives and negatives to cancel each other out. May not work, but if not why not?
                        //    //       Set oTransaction = i.zX.createBO("AC/ACTransaction")
                        //    //       tRtn = i.zX.BOS.selectSum(oTransaction, "zBaseValue", ":dWhen>=#" & Format(dDateTo, "dd mmm yyyy") & "#&dWhen<#" & Format(dDateFrom, "dd mmm yyyy") & "#" & "&FKiACPolicyID=" & i.GetAttr("id").strValue & "&iStatus=0")  'CHANGE TO BALANCE
                        //    // 
                        //    //       If tRtn = rcOk Then
                        //    //          rAgedDebt = oTransaction.GetAttr("zBaseValue").dblValue
                        //    //          i.zX.BOS.SetAttr Me, pstrAttr, i.zX.dblValue(rAgedDebt)
                        //    //       End If
                        //    break;
                        //case "_age":
                        //    if (string.IsNullOrEmpty(oPolicy.AttributeI("dDOB").sValue))
                        //    {
                        //        DateTime zeroTime = new DateTime(1, 1, 1);
                        //        dDOB = oPolicy.AttributeI("dDOB").dValue;
                        //        TimeSpan span = DateTime.Now - dDOB;
                        //        iAge = (zeroTime + span).Year - 1;
                        //        dThisYearBD = DateSerial(DateTime.Now.Year, dDOB.Month, dDOB.Day);
                        //        //iAge = iAge - 1;
                        //        pstrAttr = iAge.ToString();
                        //    }

                        //    break;
                        //case "_ageinception":
                        //    if (string.IsNullOrEmpty(oPolicy.AttributeI("dDOB").sValue))
                        //    {
                        //        DateTime zeroTime = new DateTime(1, 1, 1);
                        //        dCompare = oPolicy.AttributeI("dEffectiveFrom").dValue;
                        //        dDOB = oPolicy.AttributeI("dDOB").dValue;
                        //        TimeSpan span = dCompare - dDOB;
                        //        iAge = (zeroTime + span).Year - 1;
                        //        dThisYearBD = DateSerial(dCompare.Year, dDOB.Month, dDOB.Day);
                        //        if (dThisYearBD > dCompare)
                        //        {
                        //            iAge = iAge - 1;
                        //        }
                        //        pstrAttr = iAge.ToString();
                        //    }

                        //    // XS - just get this from compulsary always
                        //    break;
                        //case "rXSAccidental"://"rXSAccidental".ToLower()
                        //    pstrAttr = oPolicy.AttributeI("rXSCompulsary").sValue;
                        //    break;
                        //case "rXSMalicious": //"rXSMalicious".ToLower()
                        //    pstrAttr = oPolicy.AttributeI("rXSCompulsary").sValue;
                        //    break;
                        //case "rXSWindscreen"://"rXSWindscreen".ToLower()
                        //                     //          i.zX.BOS.SetAttr Me, pstrAttr, i.GetAttr("rXSCompulsary")
                        //    break;
                }
            }
            catch (Exception e)
            {
                oCResult.sMessage = oCResult.sClassName + ", " + ROUTINE_NAME + ", Error for attr: \'" + pstrAttr + "\'\r\n";
                //return i.zX.BOS.GetAttr(this, pstrAttr);
                //return oCResult;
            }
            return oDictionary;// i.zX.BOS.GetAttr(this, pstrAttr);
        }

        public CResult iBO_getAttrAGEDebt(XIIBO oPolicy, string pstrAttr, int adSelect = 0)
        {
            CResult oCResult = new CResult();
            double rAgedDebt;
            int iDayRangeFrom = 0;
            int iDayRangeTo = 0;
            DateTime dDateFrom = DateTime.Now;
            DateTime dDateTo = DateTime.Now;
            //iBO oTransaction;
            XIIBO oClaim = new XIIBO();
            CResult tRtn = new CResult();
            int iSelect = 0;
            int iShort = 0;
            int iMedium = 0;
            int iLong = 0;
            bool bOverride = false;
            int iAge = 0;
            DateTime dDOB;
            DateTime dThisYearBD;
            int iLockStatus = 0;
            long iDBLockStatus;
            //iBO oPolDB;
            double rTotalCost = 0;
            Dictionary<string, XIIBO> oColClaim = new Dictionary<string, XIIBO>();
            int iClaimCount = 0;
            XIIBO oDriver = new XIIBO();
            string sDriverConcat;
            //Collection oColVehicles;
            string sVehicleConcat;
            //iBO oVehicle;
            XIIBO oE = new XIIBO();
            Dictionary<string, XIIBO> oColE = new Dictionary<string, XIIBO>();
            //Collection oColDrivers;
            double rAgedDebtCR = 0;
            double rAgedDebtDR = 0;
            XIIBO oClient = new XIIBO();
            //iBO oSupplier;
            int iADType = 0;
            XIIBO oJE = new XIIBO();
            string sADWhere = "";
            long iAgedDebtType;
            XIIBO oRelationship = new XIIBO();
            string sCoverEnd;
            DateTime dtCoverEnd;
            DateTime dDateAsOf = DateTime.Now;
            string sDateAsOf;
            int iCount = 0;
            string sConcat;
            DateTime dCompare;
            double rShort = 0;
            double rMedium = 0;
            double rLong = 0;
            double rLongPlus = 0;
            long j;
            string ROUTINE_NAME = "iBO_getAttr";
            XIDXI oXID = new XIDXI();
            XIDBO oBOD = new XIDBO();
            XIIXI oIXI = new XIIXI();
            XIInstanceBase oIB = new XIInstanceBase();
            string sWhere = null;
            QueryEngine oQE = new QueryEngine();
            CResult oQResult = new CResult();
            XIInfraCache oCache = new XIInfraCache();
            var JournalEntryClick = new Dictionary<string, XIIBO>();
            try
            {
                switch (pstrAttr.ToLower())
                {
                    case "_sendorsements":
                        sWhere = XIConstant.Key_XIDeleted + "!=40,bApply=1,FKiACPolicyID=" + oPolicy.AttributeI("id").sValue;
                        if (oCResult.bOK && oCResult.oResult != null)
                        {
                            oQResult = oQE.Execute_QueryEngine("Term_T", "id,sName", sWhere);
                            if (oQResult.bOK && oQResult.oResult != null)
                            {
                                oColE = (Dictionary<string, XIIBO>)oQResult.oResult;
                            }
                        }
                        if (oColE == null)
                        {
                            oCResult.sMessage = oCResult.sClassName + ", " + ROUTINE_NAME + ", Unable to DB2Collection oColE";
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.LogToFile();
                            oCResult.sMessage = oCResult.sClassName + ", " + ROUTINE_NAME + ", Unable to DB2Collection oColE";
                            oIB.SaveErrortoDB(oCResult);
                            return oCResult;
                        }
                        sConcat = "";
                        foreach (var item in oColE)
                        {
                            if (sConcat != "")
                            {
                                sConcat = sConcat + ", ";
                            }
                            sConcat = sConcat + item.Value.AttributeI("sName").sValue;
                        }
                        pstrAttr = sConcat;
                        break;
                    case "_sriskoccupation":
                        sWhere = "sValue=P";
                        oQResult = oQE.Execute_QueryEngine("enumRelationship_T", "*", sWhere);
                        if (oQResult.bOK && oQResult.oResult != null)
                        {
                            var result = (Dictionary<string, XIIBO>)oQResult.oResult;
                            oRelationship = result.FirstOrDefault().Value;
                        }
                        if (oRelationship != null)
                        {
                            sWhere = XIConstant.Key_XIDeleted + "!=40,FKiPolicyID=" + oPolicy.AttributeI("id").sValue + ",enumRelationship=" + oRelationship.AttributeI("id").sValue;
                            oQResult = oQE.Execute_QueryEngine("Driver_T", "*", sWhere);
                            if (oQResult.bOK && oQResult.oResult != null)
                            {
                                tRtn = oQResult;
                            }
                            if (tRtn.bOK == true)
                            {
                                sDriverConcat = oDriver.AttributeI("enumProfessionMainID").sValue;
                                pstrAttr = sDriverConcat;
                            }
                        }
                        break;
                    case "sdelegatedplain":
                        pstrAttr = oPolicy.AttributeI("sDelegatedCode").sValue;
                        break;
                    case "_claimtotalcost":
                        sWhere = XIConstant.Key_XIDeleted + "<40,FKiACPolicyID" + oPolicy.AttributeI("id").sValue;
                        oQResult = oQE.Execute_QueryEngine("Claim_T", "id,rTotalClaimCost", sWhere);
                        if (oQResult.bOK && oQResult.oResult != null)
                        {
                            oColClaim = (Dictionary<string, XIIBO>)oQResult.oResult;
                        }
                        if (oColClaim == null)
                        {
                            oCResult.sMessage = oCResult.sClassName + ", " + ROUTINE_NAME + ", Unable to DB2Collection ";
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.LogToFile();
                            oCResult.sMessage = oCResult.sClassName + ", " + ROUTINE_NAME + ", Unable to DB2Collection ";
                            oIB.SaveErrortoDB(oCResult);
                        }

                        foreach (var item in oColClaim)
                        {
                            rTotalCost = rTotalCost + item.Value.AttributeI("rTotalClaimCost").doValue;
                        }
                        pstrAttr = rTotalCost.ToString();
                        break;
                    case "_claimcountfault":
                        sWhere = XIConstant.Key_XIDeleted + "!=40,WhoseFault>9,FKiACPolicyID" + oPolicy.AttributeI("id").sValue;
                        oQResult = oQE.Execute_QueryEngine("Claim_T", "*", sWhere);
                        if (oQResult.bOK && oQResult.oResult != null)
                        {
                            var result = (Dictionary<string, XIIBO>)oQResult.oResult;
                            iClaimCount = result.Count;
                        }
                        pstrAttr = iClaimCount.ToString();
                        break;
                    case "_vehiclecount":
                        sWhere = XIConstant.Key_XIDeleted + "!=40,FKiACPolicyID" + oPolicy.AttributeI("id").sValue;
                        oQResult = oQE.Execute_QueryEngine("Vehicle_T", "*", sWhere);
                        if (oQResult.bOK && oQResult.oResult != null)
                        {
                            var result = (Dictionary<string, XIIBO>)oQResult.oResult;
                            iCount = result.Count;
                        }
                        pstrAttr = iCount.ToString();
                        break;
                    //case "_ilockstatus":
                    //    if (oPolicy.AttributeI("id").iValue > 0)
                    //    {
                    //        iLockStatus = oPolicy.AttributeI(XIConstant.Key_XIDeleted).iValue;
                    //        //          Set oPolDB = i.zX.BOS.quickLoad("AC/ACPolicy", i.GetAttr("id"), , XIConstant.Key_XIDeleted)
                    //        //          If Not oPolDB Is Nothing Then
                    //        //             iLockStatus = oPolDB.GetAttr(XIConstant.Key_XIDeleted).lngValue
                    //        // 
                    //        if (iLockStatus == 0)
                    //        {
                    //            if (oPolicy.AttributeI("iStatus").iValue == 200)
                    //            {
                    //                iLockStatus = 10;
                    //            }
                    //            else if (oPolicy.AttributeI("iStatus").iValue == 80 || oPolicy.AttributeI("iStatus").iValue == 40)
                    //            {
                    //                // allow up to end of pol
                    //                iLockStatus = 10;
                    //                // default
                    //                sCoverEnd = oPolicy.AttributeI("dCoverEnd").sValue + " " + oPolicy.AttributeI("tCoverStart").sValue;
                    //                if (sCoverEnd != null)//check IsDate or not
                    //                {
                    //                    dtCoverEnd = DateTime.Parse(sCoverEnd);
                    //                    if (dtCoverEnd > DateTime.Now)
                    //                    {
                    //                        iLockStatus = 5;
                    //                    }
                    //                }
                    //            }
                    //            else if (i.zX.userProfile.userInGroup("ADMINEDIT") == false)
                    //            {
                    //                iLockStatus = 5;
                    //            }
                    //        }
                    //        //          End If   'if not oPolDB is nothing then
                    //    }
                    //    pstrAttr = iLockStatus.ToString();
                    //    // 2012.04.16 Case "_rshort", "_rmedium", "_rlong", "_rlongplus", "_radbalance"
                    //    break;
                    case "_rshort":
                        iSelect = adSelect;
                        //iSelect = double.Parse(i.zX.quickContext.getEntry("-ad.Select"));
                        if (iSelect > 0)
                        {
                            switch (iSelect)
                            {
                                case 10:
                                    // 15, 30, 45
                                    iShort = 15;
                                    iMedium = 30;
                                    iLong = 45;
                                    bOverride = true;
                                    break;
                                case 20:
                                    // 30, 60, 90
                                    iShort = 30;
                                    iMedium = 60;
                                    iLong = 90;
                                    bOverride = true;
                                    break;
                                case 30:
                                    // 45, 90, 135
                                    iShort = 45;
                                    iMedium = 90;
                                    iLong = 135;
                                    bOverride = true;
                                    break;
                                case 40:
                                    // 60, 120, 180
                                    iShort = 60;
                                    iMedium = 120;
                                    iLong = 180;
                                    bOverride = true;
                                    break;
                            }
                        }
                        else
                        {
                            // DS: We have a logic problem here - on the search you want to specify the amounts, whereas on the popup you want to override, so on popup make sure you specify the std option "-ad.Select"=10
                            if (0 == 0)
                            //if (double.Parse(i.zX.quickContext.getEntry("-ad.Short")) == 0)
                            {
                                iShort = 15;
                                iMedium = 30;
                                iLong = 45;
                                bOverride = true;
                            }
                            else
                            {
                                bOverride = false;
                            }
                        }
                        sDateAsOf = "";
                        //sDateAsOf = i.zX.quickContext.getEntry("-date.AsOf");
                        if (sDateAsOf != "")
                        {
                            //if (IsDate(sDateAsOf))
                            //{
                            dDateAsOf = DateTime.Parse(sDateAsOf);
                            //}
                        }
                        for (j = 1; j <= 4; j++)
                        {
                            switch (j)
                            {
                                case 4:
                                    // "_rshort"
                                    iDayRangeFrom = 0;
                                    if (bOverride)
                                    {
                                        iDayRangeTo = iShort;
                                    }
                                    else
                                    {
                                        // iDayRangeTo = double.Parse(i.zX.quickContext.getEntry("-ad.Short"));
                                    }

                                    if (iDayRangeTo == 0)
                                    {
                                        iDayRangeTo = 29;
                                    }

                                    iDayRangeTo = iDayRangeTo * -1;
                                    dDateFrom = dDateAsOf.AddDays(iDayRangeFrom);
                                    dDateTo = dDateAsOf.AddDays(iDayRangeTo);
                                    break;
                                case 3:
                                    // "_rmedium"
                                    if (bOverride)
                                    {
                                        iDayRangeFrom = iShort;
                                    }
                                    else
                                    {
                                        // iDayRangeFrom = double.Parse(i.zX.quickContext.getEntry("-ad.Short"));
                                    }

                                    if (iDayRangeFrom == 0)
                                    {
                                        iDayRangeFrom = 30;
                                    }

                                    iDayRangeFrom = (iDayRangeFrom * -1);
                                    if (bOverride)
                                    {
                                        iDayRangeTo = iMedium;
                                    }
                                    else
                                    {
                                        // iDayRangeTo = double.Parse(i.zX.quickContext.getEntry("-ad.Medium"));
                                    }

                                    if (iDayRangeTo == 0)
                                    {
                                        iDayRangeTo = 59;
                                    }

                                    iDayRangeTo = iDayRangeTo * -1;
                                    dDateFrom = dDateAsOf.AddDays(iDayRangeFrom);
                                    dDateTo = dDateAsOf.AddDays(iDayRangeTo);
                                    break;
                                case 2:
                                    // "_rlong"
                                    if (bOverride)
                                    {
                                        iDayRangeFrom = iMedium;
                                    }
                                    else
                                    {
                                        //iDayRangeFrom = double.Parse(i.zX.quickContext.getEntry("-ad.Medium"));
                                    }
                                    if (iDayRangeFrom == 0)
                                    {
                                        iDayRangeFrom = 60;
                                    }
                                    iDayRangeFrom = iDayRangeFrom * -1;
                                    if (bOverride)
                                    {
                                        iDayRangeTo = iLong;
                                    }
                                    else
                                    {
                                        // iDayRangeTo = double.Parse(i.zX.quickContext.getEntry("-ad.Long"));
                                    }

                                    if (iDayRangeTo == 0)
                                    {
                                        iDayRangeTo = 89;
                                    }
                                    iDayRangeTo = iDayRangeTo * -1;
                                    dDateFrom = dDateAsOf.AddDays(iDayRangeFrom);
                                    dDateTo = dDateAsOf.AddDays(iDayRangeTo);
                                    break;
                                case 1:
                                    // "_rlongplus"
                                    // get the days from the querystring
                                    if (bOverride)
                                    {
                                        iDayRangeFrom = iLong;
                                    }
                                    else
                                    {
                                        //iDayRangeFrom = double.Parse(i.zX.quickContext.getEntry("-ad.Long"));
                                    }
                                    if (iDayRangeFrom == 0)
                                    {
                                        iDayRangeFrom = 90;
                                    }
                                    iDayRangeFrom = iDayRangeFrom * -1;
                                    iDayRangeTo = -40000;
                                    //  i think 1899 is earliest year
                                    dDateFrom = dDateAsOf.AddDays(iDayRangeFrom);
                                    dDateTo = dDateAsOf.AddDays(iDayRangeTo);
                                    break;
                                    //case "_radbalance":
                                    //    iDayRangeFrom = 0;
                                    //    iDayRangeTo = -40000;
                                    //    //  i think 1899 is earliest year
                                    //    dDateFrom = dDateAsOf.AddDays(iDayRangeFrom);
                                    //    dDateTo = dDateAsOf.AddDays(iDayRangeTo);
                                    //    break;
                            }
                            // DS 06.06.09 - i think this has to be changed to 2 transactions where the sum is done twice - once for CR and once for DR
                            // DS 06.06.09 - as of now not from DB - why? Well maybe due to not having fixed short /med/long day ranges
                            // On Error Resume Next
                            iADType = 0;
                            //iADType = double.Parse(i.zX.quickContext.getEntry("-ADType"));
                            if (iADType == 0)
                            {
                                oClient = oIXI.BOI("Customer_T", oPolicy.AttributeI("FKiCustomerID").sValue, "id,FKiACAccountID");
                                if (oClient == null)
                                {
                                    oCResult.sMessage = oCResult.sClassName + " - " + ROUTINE_NAME + " - CANNOT LOAD CLIENT";
                                }
                            }
                            else if (iADType == 10)
                            {
                                // even though should be oSupplier, keep oClient so code below is same for both
                                oClient = oIXI.BOI("Supplier_T", oPolicy.AttributeI("FKiSupplierID").sValue, "id,FKiACAccountID");
                                if (oClient == null)
                                {
                                    oCResult.sMessage = oCResult.sClassName + " - " + ROUTINE_NAME + " - CANNOT LOAD SUPPLIER";
                                }
                            }

                            // DS 12.06.09 this doesn't work as the DR and CR accounts are not set on the transaction (it doesn't know them as it is the journals that hold this info)
                            // '      Set oTransaction = i.zX.createBO("AC/ACTransaction")
                            // '      'tRtn = i.zX.BOS.selectSum(oTransaction, "zBaseValue", ":dWhen>=#" & Format(dDateTo, "dd mmm yyyy") & "#&dWhen<#" & Format(dDateFrom, "dd mmm yyyy") & "#" & "&FKiACPolicyID=" & i.GetAttr("id").strValue & "iStatus=0")  'CHANGE TO BALANCE
                            // '      tRtn = i.zX.BOS.selectSum(oTransaction, "zBaseValue", ":dWhen>=#" & Format(dDateTo, "dd mmm yyyy") & "#&dWhen<#" & Format(dDateFrom, "dd mmm yyyy") & "#" & "&FKiCRAccountID=" & oClient.GetAttr("FKiACAccountID").strValue & "&iStatus=0")
                            // '
                            // '      If tRtn = rcOk Then
                            // '         rAgedDebtCR = oTransaction.GetAttr("zBaseValue").dblValue
                            // '      End If
                            // '      Set oTransaction = i.zX.createBO("AC/ACTransaction")
                            // '
                            // '      tRtn = i.zX.BOS.selectSum(oTransaction, "zBaseValue", ":dWhen>=#" & Format(dDateTo, "dd mmm yyyy") & "#&dWhen<#" & Format(dDateFrom, "dd mmm yyyy") & "#" & "&FKiDRAccountID=" & oClient.GetAttr("FKiACAccountID").strValue & "&iStatus=0")
                            // '
                            // '      If tRtn = rcOk Then
                            // '         rAgedDebtDR = oTransaction.GetAttr("zBaseValue").dblValue
                            // '      End If
                            // '
                            // '
                            // '      If tRtn = rcOk Then
                            // '         rAgedDebt = rAgedDebtCR - rAgedDebtDR 'oTransaction.GetAttr("zBaseValue").dblValue
                            // '         i.zX.BOS.SetAttr Me, pstrAttr, i.zX.dblValue(rAgedDebt)
                            // '      End If
                            // DS 12.06.09 - change to journal to hold policy allows exact journal tracking
                            // -ADRptType - if this is 0 then normal aged debt. If 10 then premium finance aged debt
                            iAgedDebtType = 0;
                            //iAgedDebtType = i.GetAttr("_ADType").lngValue;
                            //if (i.zX.quickContext.getEntry("-ADRptType") != "")
                            //{
                            //    iAgedDebtType = double.Parse(i.zX.quickContext.getEntry("-ADRptType"));
                            //}

                            //oJE = i.zX.createBO("AC/ACJournalEntry");
                            // - date is in the appropriate range (eg. short day range)
                            // - the account no is the client (or supplier) account
                            // - journals are for this policy
                            // - journal status is live (ie this is not a corrected JE)
                            // 2011.02.01 Changed because the from and to dates can overlap
                            // 2010.04.29 - remove status restriction as corrected should zeroise and replacement should be counted & "&iStatus=0"
                            // Status of journal not working properly so we have to get the transaction
                            string Query = "";
                            Query = "SELECT " + j + " as 'Debt',SUM([ACJournalEntry_T].[rAmount]) as rAmount FROM [ACJournalEntry_T] inner join [ACTransaction_T] on [ACJournalEntry_T].FKiTransactionID=[ACTransaction_T].id";
                            Query += " WHERE(1 = 1) AND cast([ACJournalEntry_T].[dDate] as date) > '" + dDateTo.ToString("yyyy-MM-dd") + "' AND cast([ACJournalEntry_T].[dDate] as date) <= '" + dDateFrom.ToString("yyyy-MM-dd") + "'";
                            Query += " AND [ACJournalEntry_T].[FKiAccountID] = " + oClient.AttributeI("FKiACAccountID").sValue;
                            Query += " AND ([ACJournalEntry_T].[iStatus] = 0 OR [ACJournalEntry_T].[iStatus] = 30)";
                            Query += " AND [ACJournalEntry_T].[FKiACPolicyID] = " + oPolicy.AttributeI("id").sValue + " AND ([ACTransaction_T].[id] = [ACJournalEntry_T].[FKiTransactionID])";
                            Query += " AND ([ACTransaction_T].[iStatus] = 0 OR [ACTransaction_T].[iStatus] = 30)";
                            if (iAgedDebtType == 10)
                            {
                                // PF only
                                Query += " AND [ACJournalEntry_T].[iTransType] = 200";
                            }
                            sADWhere = " AND [ACJournalEntry_T].[iType] = 10";

                            // SELECT SUM([ACJournalEntry_T].[rAmount]) FROM [ACJournalEntry_T] WHERE (1 = 1)
                            // AND ([ACJournalEntry_T].[dDate] > '1907-09-15' AND [ACJournalEntry_T].[dDate] <= '2017-02-05'
                            // AND [ACJournalEntry_T].[FKiAccountID] = 516
                            // AND ([ACJournalEntry_T].[iStatus] = 0 OR [ACJournalEntry_T].[iStatus] = 30)
                            // AND [ACJournalEntry_T].[FKiACPolicyID] = 2747
                            // AND [ACJournalEntry_T].[FKiTransactionID] IN (SELECT  TOP 1 [ACTransaction_T].[id] AS [ACTransaction_T_id]
                            //           FROM [ACTransaction_T] WHERE (1 = 1)  AND ([ACTransaction_T].[id] = [ACJournalEntry_T].[FKiTransactionID])
                            //           AND ([ACTransaction_T].[iStatus] = 0 OR [ACTransaction_T].[iStatus] = 30))
                            //           AND [ACJournalEntry_T].[iTransType] = 200
                            //           AND [ACJournalEntry_T].[iType] = 10)


                            JournalEntryClick = new Dictionary<string, XIIBO>();
                            var oJournalBOD = ((XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "ACJournalEntry_T", null));
                            var DataSources = oXID.GetBODataSource(oJournalBOD.iDataSourceXIGUID.ToString(), oJournalBOD.FKiApplicationID);
                            XID1Click JournalEntry1Clicks = new XID1Click();
                            var JournalEntryQuerys = Query + sADWhere;
                            JournalEntry1Clicks.sConnectionString = DataSources;
                            JournalEntry1Clicks.Query = JournalEntryQuerys.ToString();
                            JournalEntry1Clicks.Name = "ACJournalEntry_T";
                            JournalEntryClick = JournalEntry1Clicks.OneClick_Execute();
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            oCResult.oResult = JournalEntryClick.Values;
                            tRtn = oCResult;
                            if (tRtn.bOK == true)
                            {
                                rAgedDebtCR = JournalEntryClick.Values.FirstOrDefault().AttributeI("rAmount").doValue;
                            }
                            sADWhere = " AND [ACJournalEntry_T].[iType] = 20";
                            JournalEntry1Clicks = new XID1Click();
                            JournalEntryQuerys = Query + sADWhere;
                            JournalEntry1Clicks.sConnectionString = DataSources;
                            JournalEntry1Clicks.Query = JournalEntryQuerys.ToString();
                            JournalEntry1Clicks.Name = "ACJournalEntry_T";
                            JournalEntryClick = JournalEntry1Clicks.OneClick_Execute();
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            oCResult.oResult = JournalEntryClick.Values;
                            tRtn = oCResult;
                            if (tRtn.bOK == true)
                            {
                                rAgedDebtDR = JournalEntryClick.Values.FirstOrDefault().AttributeI("rAmount").doValue;
                            }
                            if (tRtn.bOK == true)
                            {
                                rAgedDebt = rAgedDebtCR - rAgedDebtDR;
                                // 2012.04.16 - ok PROBLEM is we do not know if short is executed before medium etc
                                //    possible solution to that is to change the system so that all 3 are calculated at the same time and then assigned. so this getattr would only trigger on say _short, the others would be automatically calculated
                                switch (j)
                                {
                                    case 4:
                                        rShort = rAgedDebt;
                                        break;
                                    case 3:
                                        rMedium = rAgedDebt;
                                        break;
                                    case 2:
                                        rLong = rAgedDebt;
                                        break;
                                    case 1:
                                        rLongPlus = rAgedDebt;
                                        break;
                                }
                                //             i.zX.BOS.SetAttr Me, pstrAttr, i.zX.dblValue(rAgedDebt)
                            }

                        }

                        // OK, now compare long plus all the way back to short and stack up in reverse in each case. Not just one step at a time, but re-check the whole sequence (for example long may be �90, medium zero and short �30, so that would mean that
                        //   long is �60 and medium and short zero when done
                        for (j = 2; j <= 4; j++)
                        {
                            // not 1 to 4 as for long plus there is no figure to adjust
                            switch (j)
                            {
                                case 2:
                                    // long
                                    if ((rLong > 0 && rLongPlus < 0) || (rLong < 0 && rLongPlus > 0))
                                    {
                                        if (Math.Abs(rLongPlus) > Math.Abs(rLong))
                                        {
                                            rLongPlus = rLongPlus + rLong;
                                            rLong = 0;
                                        }
                                        else
                                        {
                                            rLong = rLong + rLongPlus;
                                            rLongPlus = 0;
                                        }
                                    }
                                    break;
                                case 3:
                                    // medium
                                    if ((rMedium > 0 && rLongPlus < 0) || (rMedium < 0 && rLongPlus > 0))
                                    {
                                        if (Math.Abs(rLongPlus) > Math.Abs(rMedium))
                                        {
                                            rLongPlus = rLongPlus + rMedium;
                                            rMedium = 0;
                                        }
                                        else
                                        {
                                            rMedium = rMedium + rLongPlus;
                                            rLongPlus = 0;
                                        }

                                    }

                                    if ((rMedium > 0 && rLong < 0) || (rMedium < 0 && rLong > 0))
                                    {
                                        if (Math.Abs(rLong) > Math.Abs(rMedium))
                                        {
                                            rLong = rLong + rMedium;
                                            rMedium = 0;
                                        }
                                        else
                                        {
                                            rMedium = rMedium + rLong;
                                            rLong = 0;
                                        }
                                    }
                                    break;
                                case 4:
                                    // short
                                    if ((rShort > 0 && rLongPlus < 0) || (rShort < 0 && rLongPlus > 0))
                                    {
                                        if (Math.Abs(rLongPlus) > Math.Abs(rShort))
                                        {
                                            rLongPlus = rLongPlus + rShort;
                                            rShort = 0;
                                        }
                                        else
                                        {
                                            rShort = rShort + rLongPlus;
                                            rLongPlus = 0;
                                        }
                                    }

                                    if ((rShort > 0 && rLong < 0) || (rShort < 0 && rLong > 0))
                                    {
                                        if (Math.Abs(rLong) > Math.Abs(rShort))
                                        {
                                            rLong = rLong + rShort;
                                            rShort = 0;
                                        }
                                        else
                                        {
                                            rShort = rShort + rLong;
                                            rLong = 0;
                                        }
                                    }

                                    if ((rShort > 0 && rMedium < 0) || (rShort < 0 && rMedium > 0))
                                    {
                                        if (Math.Abs(rMedium) > Math.Abs(rShort))
                                        {
                                            rMedium = rMedium + rShort;
                                            rShort = 0;
                                        }
                                        else
                                        {
                                            rShort = rShort + rMedium;
                                            rMedium = 0;
                                        }
                                    }

                                    break;
                            }
                        }
                        var _rShort = rShort.ToString();
                        var _rMedium = rMedium.ToString();
                        var _rLong = rLong.ToString();
                        var _rLongPlus = rLongPlus.ToString();

                        //i.SetAttr("_rShort", rShort.ToString());
                        //i.SetAttr("_rMedium", rMedium);
                        //i.SetAttr("_rLong", rLong);
                        //i.SetAttr("_rLongPlus", i.zX.dblValue(rLongPlus);
                        //2012.04.16 - everthing after this line was added back in from the original code JUST to handle _radbalance. BUT it is the original code, so easy to put back
                        break;
                        //case "_radbalance":
                        //    iSelect = double.Parse(i.zX.quickContext.getEntry("-ad.Select"));
                        //    if ((iSelect > 0))
                        //    {
                        //        switch (iSelect)
                        //        {
                        //            case 10:
                        //                // 15, 30, 45
                        //                iShort = 15;
                        //                iMedium = 30;
                        //                iLong = 45;
                        //                bOverride = true;
                        //                break;
                        //            case 20:
                        //                // 30, 60, 90
                        //                iShort = 30;
                        //                iMedium = 60;
                        //                iLong = 90;
                        //                bOverride = true;
                        //                break;
                        //            case 30:
                        //                // 45, 90, 135
                        //                iShort = 45;
                        //                iMedium = 90;
                        //                iLong = 135;
                        //                bOverride = true;
                        //                break;
                        //            case 40:
                        //                // 60, 120, 180
                        //                iShort = 60;
                        //                iMedium = 120;
                        //                iLong = 180;
                        //                bOverride = true;
                        //                break;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        // DS: We have a logic problem here - on the search you want to specify the amounts, whereas on the popup you want to override, so on popup make sure you specify the std option "-ad.Select"=10
                        //        if (double.Parse(i.zX.quickContext.getEntry("-ad.Short") == 0))
                        //        {
                        //            iShort = 15;
                        //            iMedium = 30;
                        //            iLong = 45;
                        //            bOverride = true;
                        //        }
                        //        else
                        //        {
                        //            bOverride = false;
                        //        }

                        //    }

                        //    sDateAsOf = i.zX.quickContext.getEntry("-date.AsOf");
                        //    if (sDateAsOf != "")
                        //    {
                        //        if (sDateAsOf === null)
                        //        {
                        //            dDateAsOf = DateTime.Parse(sDateAsOf);
                        //        }

                        //    }

                        //    switch (pstrAttr.ToLower())
                        //    {
                        //        case "_rshort":
                        //            iDayRangeFrom = 0;
                        //            if (bOverride)
                        //            {
                        //                iDayRangeTo = iShort;
                        //            }
                        //            else
                        //            {
                        //                iDayRangeTo = double.Parse(i.zX.quickContext.getEntry("-ad.Short"));
                        //            }

                        //            if (iDayRangeTo == 0)
                        //            {
                        //                iDayRangeTo = 29;
                        //            }

                        //            iDayRangeTo = (iDayRangeTo * -1);
                        //            dDateFrom = dDateAsOf.AddDays(iDayRangeFrom);
                        //            dDateTo = dDateAsOf.AddDays(iDayRangeTo);
                        //            break;
                        //        case "_rmedium":
                        //            if (bOverride)
                        //            {
                        //                iDayRangeFrom = iShort;
                        //            }
                        //            else
                        //            {
                        //                iDayRangeFrom = double.Parse(i.zX.quickContext.getEntry("-ad.Short"));
                        //            }

                        //            if ((iDayRangeFrom == 0))
                        //            {
                        //                iDayRangeFrom = 30;
                        //            }

                        //            iDayRangeFrom = (iDayRangeFrom * -1);
                        //            if (bOverride)
                        //            {
                        //                iDayRangeTo = iMedium;
                        //            }
                        //            else
                        //            {
                        //                iDayRangeTo = double.Parse(i.zX.quickContext.getEntry("-ad.Medium"));
                        //            }

                        //            if (iDayRangeTo == 0)
                        //            {
                        //                iDayRangeTo = 59;
                        //            }

                        //            iDayRangeTo = (iDayRangeTo * -1);
                        //            dDateFrom = dDateAsOf.AddDays(iDayRangeFrom);
                        //            dDateTo = dDateAsOf.AddDays(iDayRangeTo);
                        //            break;
                        //        case "_rlong":
                        //            if (bOverride)
                        //            {
                        //                iDayRangeFrom = iMedium;
                        //            }
                        //            else
                        //            {
                        //                iDayRangeFrom = double.Parse(i.zX.quickContext.getEntry("-ad.Medium"));
                        //            }

                        //            if (iDayRangeFrom == 0)
                        //            {
                        //                iDayRangeFrom = 60;
                        //            }

                        //            iDayRangeFrom = (iDayRangeFrom * -1);
                        //            if (bOverride)
                        //            {
                        //                iDayRangeTo = iLong;
                        //            }
                        //            else
                        //            {
                        //                iDayRangeTo = double.Parse(i.zX.quickContext.getEntry("-ad.Long"));
                        //            }

                        //            if (iDayRangeTo == 0)
                        //            {
                        //                iDayRangeTo = 89;
                        //            }

                        //            iDayRangeTo = (iDayRangeTo * -1);
                        //            dDateFrom = dDateAsOf.AddDays(iDayRangeFrom);
                        //            dDateTo = dDateAsOf.AddDays(iDayRangeTo);
                        //            break;
                        //        case "_rlongplus":
                        //            if (bOverride)
                        //            {
                        //                iDayRangeFrom = iLong;
                        //            }
                        //            else
                        //            {
                        //                iDayRangeFrom = double.Parse(i.zX.quickContext.getEntry("-ad.Long"));
                        //            }

                        //            if ((iDayRangeFrom == 0))
                        //            {
                        //                iDayRangeFrom = 90;
                        //            }

                        //            iDayRangeFrom = (iDayRangeFrom * -1);
                        //            iDayRangeTo = -40000;
                        //            //  i think 1899 is earliest year
                        //            dDateFrom = dDateAsOf.AddDays(iDayRangeFrom);
                        //            dDateTo = dDateAsOf.AddDays(iDayRangeTo);
                        //            break;
                        //        case "_radbalance":
                        //            iDayRangeFrom = 0;
                        //            iDayRangeTo = -40000;
                        //            //  i think 1899 is earliest year
                        //            dDateFrom = dDateAsOf.AddDays(iDayRangeFrom);
                        //            dDateTo = dDateAsOf.AddDays(iDayRangeTo);
                        //            break;
                        //    }
                        //    // DS 06.06.09 - i think this has to be changed to 2 transactions where the sum is done twice - once for CR and once for DR
                        //    // DS 06.06.09 - as of now not from DB - why? Well maybe due to not having fixed short /med/long day ranges
                        //    // On Error Resume Next
                        //    iADType = double.Parse(i.zX.quickContext.getEntry("-ADType"));
                        //    if (iADType == 0)
                        //    {
                        //        oClient = i.zX.BOS.quickFKLoad(this, "FKiCustomerID", "id,FKiACAccountID");
                        //        if (oClient == null)
                        //        {
                        //            oCResult.sMessage = oCResult.sClassName + " - " + ROUTINE_NAME + " - CANNOT LOAD CLIENT";
                        //        }

                        //    }
                        //    else if (iADType == 10)
                        //    {
                        //        // even though should be oSupplier, keep oClient so code below is same for both
                        //        oClient = i.zX.BOS.quickFKLoad(this, "FKiSupplierID", "id,FKiACAccountID");
                        //        if ((oClient == null))
                        //        {
                        //            oCResult.sMessage = oCResult.sClassName + " - " + ROUTINE_NAME + " - CANNOT LOAD SUPPLIER";
                        //        }

                        //    }

                        //    // DS 12.06.09 this doesn't work as the DR and CR accounts are not set on the transaction (it doesn't know them as it is the journals that hold this info)
                        //    // '      Set oTransaction = i.zX.createBO("AC/ACTransaction")
                        //    // '      'tRtn = i.zX.BOS.selectSum(oTransaction, "zBaseValue", ":dWhen>=#" & Format(dDateTo, "dd mmm yyyy") & "#&dWhen<#" & Format(dDateFrom, "dd mmm yyyy") & "#" & "&FKiACPolicyID=" & i.GetAttr("id").strValue & "iStatus=0")  'CHANGE TO BALANCE
                        //    // '      tRtn = i.zX.BOS.selectSum(oTransaction, "zBaseValue", ":dWhen>=#" & Format(dDateTo, "dd mmm yyyy") & "#&dWhen<#" & Format(dDateFrom, "dd mmm yyyy") & "#" & "&FKiCRAccountID=" & oClient.GetAttr("FKiACAccountID").strValue & "&iStatus=0")
                        //    // '
                        //    // '      If tRtn = rcOk Then
                        //    // '         rAgedDebtCR = oTransaction.GetAttr("zBaseValue").dblValue
                        //    // '      End If
                        //    // '      Set oTransaction = i.zX.createBO("AC/ACTransaction")
                        //    // '
                        //    // '      tRtn = i.zX.BOS.selectSum(oTransaction, "zBaseValue", ":dWhen>=#" & Format(dDateTo, "dd mmm yyyy") & "#&dWhen<#" & Format(dDateFrom, "dd mmm yyyy") & "#" & "&FKiDRAccountID=" & oClient.GetAttr("FKiACAccountID").strValue & "&iStatus=0")
                        //    // '
                        //    // '      If tRtn = rcOk Then
                        //    // '         rAgedDebtDR = oTransaction.GetAttr("zBaseValue").dblValue
                        //    // '      End If
                        //    // '
                        //    // '
                        //    // '      If tRtn = rcOk Then
                        //    // '         rAgedDebt = rAgedDebtCR - rAgedDebtDR 'oTransaction.GetAttr("zBaseValue").dblValue
                        //    // '         i.zX.BOS.SetAttr Me, pstrAttr, i.zX.dblValue(rAgedDebt)
                        //    // '      End If
                        //    // DS 12.06.09 - change to journal to hold policy allows exact journal tracking
                        //    // -ADRptType - if this is 0 then normal aged debt. If 10 then premium finance aged debt
                        //    iAgedDebtType = i.GetAttr("_ADType").lngValue;
                        //    if (i.zX.quickContext.getEntry("-ADRptType") != "")
                        //    {
                        //        iAgedDebtType = double.Parse(i.zX.quickContext.getEntry("-ADRptType"));
                        //    }

                        //    //oJE = i.zX.createBO("AC/ACJournalEntry");
                        //    // - date is in the appropriate range (eg. short day range)
                        //    // - the account no is the client (or supplier) account
                        //    // - journals are for this policy
                        //    // - journal status is live (ie this is not a corrected JE)
                        //    // 2011.02.01 Changed because the from and to dates can overlap
                        //    sADWhere = new StringBuilder();
                        //    sADWhere.Append(" SELECT SUM([ACJournalEntry_T].[rAmount]) FROM [ACJournalEntry_T] inner join [ACTransaction_T] on [ACJournalEntry_T].FKiTransactionID=[ACTransaction_T].id ");
                        //    sADWhere.Append(" WHERE(1 = 1) AND [ACJournalEntry_T].dDate>" + dDateTo.ToString("yyyy-mm-dd"));
                        //    sADWhere.Append(" AND [ACJournalEntry_T].dDate<=" + dDateFrom.ToString("yyyy-mm-dd") + " AND [ACJournalEntry_T].FKiAccountID=");
                        //    sADWhere.Append(oClient.AttributeI("FKiACAccountID").sValue + " AND ([ACJournalEntry_T].iStatus=0 OR [ACJournalEntry_T].iStatus=30)");
                        //    sADWhere.Append(" AND [ACJournalEntry_T].FKiACPolicyID=" + oPolicy.AttributeI("id").sValue);
                        //    // 2010.04.29 - remove status restriction as corrected should zeroise and replacement should be counted & "&iStatus=0"
                        //    // Status of journal not working properly so we have to get the transaction
                        //    // 24.03.2017 - change made for aged finance
                        //    //       sADWhere = sADWhere & "&#exists[AC/ACTransaction, iStatus=0|istatus=30]"
                        //    // sADWhere = ":dDate>=#" & Format(dDateTo, "dd mmm yyyy") & "#&dDate<=#" & Format(dDateFrom, "dd mmm yyyy") & "#" & "&FKiAccountID=" & oClient.GetAttr("FKiACAccountID").strValue & "&(iStatus=0|iStatus=30)&FKiACPolicyID=" & i.GetAttr("id").strValue  '2010.04.29 - remove status restriction as corrected should zeroise and replacement should be counted & "&iStatus=0"
                        //    if (iAgedDebtType == 10)
                        //    {
                        //        // PF only
                        //        sADWhere.Append(" AND [ACJournalEntry_T].iTransType=200");
                        //        sADWhere.Append(" AND ([ACTransaction_T].iStatus=0 OR [ACTransaction_T].istatus=30) AND ([ACTransaction_T].iPFRec=0 OR [ACTransaction_T].iPFRec=Null)");
                        //    }
                        //    else
                        //    {
                        //        sADWhere.Append(" AND [ACTransaction_T].iStatus=0 OR [ACTransaction_T].istatus=30");
                        //    }
                        //    oCache = new XIInfraCache();
                        //    JournalEntryClick = new Dictionary<string, XIIBO>();
                        //    var iDataSource = ((XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "ACJournalEntry_T", null)).iDataSource;
                        //    var DataSource = oXID.GetBODataSource(iDataSource);
                        //    XID1Click JournalEntry1Click = new XID1Click();
                        //    var JournalEntryQuery = sADWhere + " AND [ACJournalEntry_T].iType=10";
                        //    JournalEntry1Click.sConnectionString = DataSource;
                        //    JournalEntry1Click.Query = JournalEntryQuery.ToString();
                        //    JournalEntry1Click.Name = "ACJournalEntry_T";
                        //    JournalEntryClick = JournalEntry1Click.OneClick_Execute();
                        //    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        //    oCResult.oResult = JournalEntryClick;
                        //    tRtn = oCResult;
                        //    if (tRtn.bOK == true)
                        //    {
                        //        rAgedDebtCR = oJE.AttributeI("rAmount").doValue;
                        //    }
                        //    JournalEntryQuery = sADWhere + " AND [ACJournalEntry_T].iType=20";
                        //    JournalEntryClick = JournalEntry1Click.OneClick_Execute();
                        //    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        //    oCResult.oResult = JournalEntryClick;
                        //    tRtn = oCResult;
                        //    // tRtn = i.zX.BOS.selectSum(oJE, "rAmount", ":dDate>=#" & Format(dDateTo, "dd mmm yyyy") & "#&dDate<=#" & Format(dDateFrom, "dd mmm yyyy") & "#" & "&FKiAccountID=" & oClient.GetAttr("FKiACAccountID").strValue & "&FKiACPolicyID=" & i.GetAttr("id").strValue & "&iType=20&iStatus=0")
                        //    if (tRtn.bOK == true)
                        //    {
                        //        rAgedDebtDR = oJE.AttributeI("rAmount").doValue;
                        //    }

                        //    if (tRtn.bOK == true)
                        //    {
                        //        rAgedDebt = rAgedDebtCR - rAgedDebtDR;
                        //        // 2012.04.16 - ok PROBLEM is we do not know if short is executed before medium etc
                        //        //    possible solution to that is to change the system so that all 3 are calculated at the same time and then assigned. so this getattr would only trigger on say _short, the others would be automatically calculated
                        //        pstrAttr = rAgedDebt.ToString();
                        //    }

                        //    // >>> 2012.04.16
                        //    // DS 12.06.09 - back to old method of relying on the positives and negatives to cancel each other out. May not work, but if not why not?
                        //    //       Set oTransaction = i.zX.createBO("AC/ACTransaction")
                        //    //       tRtn = i.zX.BOS.selectSum(oTransaction, "zBaseValue", ":dWhen>=#" & Format(dDateTo, "dd mmm yyyy") & "#&dWhen<#" & Format(dDateFrom, "dd mmm yyyy") & "#" & "&FKiACPolicyID=" & i.GetAttr("id").strValue & "&iStatus=0")  'CHANGE TO BALANCE
                        //    // 
                        //    //       If tRtn = rcOk Then
                        //    //          rAgedDebt = oTransaction.GetAttr("zBaseValue").dblValue
                        //    //          i.zX.BOS.SetAttr Me, pstrAttr, i.zX.dblValue(rAgedDebt)
                        //    //       End If
                        //    break;
                        //case "_age":
                        //    if (string.IsNullOrEmpty(oPolicy.AttributeI("dDOB").sValue))
                        //    {
                        //        DateTime zeroTime = new DateTime(1, 1, 1);
                        //        dDOB = oPolicy.AttributeI("dDOB").dValue;
                        //        TimeSpan span = DateTime.Now - dDOB;
                        //        iAge = (zeroTime + span).Year - 1;
                        //        dThisYearBD = DateSerial(DateTime.Now.Year, dDOB.Month, dDOB.Day);
                        //        //iAge = iAge - 1;
                        //        pstrAttr = iAge.ToString();
                        //    }

                        //    break;
                        //case "_ageinception":
                        //    if (string.IsNullOrEmpty(oPolicy.AttributeI("dDOB").sValue))
                        //    {
                        //        DateTime zeroTime = new DateTime(1, 1, 1);
                        //        dCompare = oPolicy.AttributeI("dEffectiveFrom").dValue;
                        //        dDOB = oPolicy.AttributeI("dDOB").dValue;
                        //        TimeSpan span = dCompare - dDOB;
                        //        iAge = (zeroTime + span).Year - 1;
                        //        dThisYearBD = DateSerial(dCompare.Year, dDOB.Month, dDOB.Day);
                        //        if (dThisYearBD > dCompare)
                        //        {
                        //            iAge = iAge - 1;
                        //        }
                        //        pstrAttr = iAge.ToString();
                        //    }

                        //    // XS - just get this from compulsary always
                        //    break;
                        //case "rXSAccidental"://"rXSAccidental".ToLower()
                        //    pstrAttr = oPolicy.AttributeI("rXSCompulsary").sValue;
                        //    break;
                        //case "rXSMalicious": //"rXSMalicious".ToLower()
                        //    pstrAttr = oPolicy.AttributeI("rXSCompulsary").sValue;
                        //    break;
                        //case "rXSWindscreen"://"rXSWindscreen".ToLower()
                        //                     //          i.zX.BOS.SetAttr Me, pstrAttr, i.GetAttr("rXSCompulsary")
                        //    break;
                }
            }
            catch (Exception e)
            {
                oCResult.sMessage = oCResult.sClassName + ", " + ROUTINE_NAME + ", Error for attr: \'" + pstrAttr + "\'\r\n";
                //return i.zX.BOS.GetAttr(this, pstrAttr);
                return oCResult;
            }
            return null;// i.zX.BOS.GetAttr(this, pstrAttr);
        }
        public CResult AGECreditors(Dictionary<string, XIIValue> XIValues)
        {
            CResult oCResult = new CResult();
            XIInstanceBase oIB = new XIInstanceBase();
            string ROUTINE_NAME = "[AGECredit]";
            //XIIXI oIXI = new XIIXI();
            //int iQSIID = oParams.Where(t => t.sName == "iQSIID").Select(f => Convert.ToInt32(f.sValue)).FirstOrDefault();
            //var oQSInstance = oIXI.GetQuestionSetInstanceByID(0, iQSIID, null, 0, 0, "");

            List<XIIBO> oBOIPolicies = new List<XIIBO>();
            QueryEngine oQE = new QueryEngine();
            string sWhereCondition = " and [ACPolicy_T].iStatus!=5 and dCreated<'" + Convert.ToDateTime(XIValues["dAsOfDate"].sValue).AddDays(1).ToString("dd-MMM-yyyy") + "'";
            XID1Click oXI1Click = new XID1Click();
            oXI1Click.Query = XIConstant.AgeCreditQuery + sWhereCondition + " order by id desc";
            oXI1Click.Name = "ACPolicy_T";
            var oQResult = oXI1Click.GetList();
            if (oQResult.bOK && oQResult.oResult != null)
            {
                oBOIPolicies = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.ToList();
            }
            //if (oBOIPolicies == null || oBOIPolicies.Count() < 1)
            //{
            //    oCResult.sMessage = ROUTINE_NAME + ", Unable to get the Policies Collection ";
            //    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
            //    oCResult.LogToFile();
            //    oCResult.sMessage = ROUTINE_NAME + ", Unable to get the Policies Collection ";
            //    oIB.SaveErrortoDB(oCResult);
            //}
            List<XIIBO> oAgedCredits = new List<XIIBO>();
            XIDXI oXID = new XIDXI();
            XIDBO oBOD = (XIDBO)oXID.Get_BODefinition("ACAgedDebtByDay_T").oResult;
            XIIBO oBOI = new XIIBO();
            foreach (var oPolicy in oBOIPolicies)
            {
                var res = iBO_getAttrAGECreditors(oPolicy, "_rshort", XIValues);
                oBOI = new XIIBO();
                oBOI.BOD = oBOD;
                oBOI.Attributes["Client"] = new XIIAttribute { sName = "FKiCustomerID", sValue = oPolicy.AttributeI("Client").sValue, bDirty = true };
                oBOI.Attributes["Supplier"] = new XIIAttribute { sName = "FKiSupplierID", sValue = oPolicy.AttributeI("Supplier").sValue, bDirty = true };
                oBOI.Attributes["Status"] = new XIIAttribute { sName = "iStatus", sValue = oPolicy.AttributeI("Status").sValue, bDirty = true };
                oBOI.Attributes["Type"] = new XIIAttribute { sName = "iType", sValue = "Aged Credit", bDirty = true };
                //oBOI.Attributes["FKiOrgID"] = new XIIAttribute { sName = "FKiOrgID", sValue = oPolicy.AttributeI("FKiOrgID").sValue, bDirty = true };
                oBOI.Attributes["Date"] = new XIIAttribute { sName = "dDate", sValue = oPolicy.AttributeI("dCoverStart").sValue == "" ? "" : Convert.ToDateTime(oPolicy.AttributeI("dCoverStart").sValue).ToString("dd-MM-yyyy"), bDirty = true };
                oBOI.Attributes["Short"] = new XIIAttribute { sName = "rShort", sValue = res["short"].ToString(), bDirty = true };
                oBOI.Attributes["Medium"] = new XIIAttribute { sName = "rMedium", sValue = res["medium"].ToString(), bDirty = true };
                oBOI.Attributes["Long"] = new XIIAttribute { sName = "rLong", sValue = res["long"].ToString(), bDirty = true };
                oBOI.Attributes["Long+"] = new XIIAttribute { sName = "rLongPlus", sValue = res["longplus"].ToString(), bDirty = true };
                oBOI.Attributes["Balance"] = new XIIAttribute { sName = "rTotal", sValue = oPolicy.AttributeI("rPolicyBalance").sValue, bDirty = true };
                oBOI.Attributes["Policy"] = new XIIAttribute { sName = "FKiACPolicyID", sValue = oPolicy.AttributeI("Policy").sValue, bDirty = true };
                oBOI.Attributes["Enterprise"] = new XIIAttribute { sName = "FKiEnterpriseID", sValue = oPolicy.AttributeI("Enterprise").sValue, bDirty = true };
                oBOI.Attributes["Account Category"] = new XIIAttribute { sName = "refAccountCategory", sValue = oPolicy.AttributeI("AccountCategory").sValue, bDirty = true };
                oBOI.Attributes["Name"] = new XIIAttribute { sName = "sName", sValue = oPolicy.AttributeI("sName").sValue, bDirty = true };
                oAgedCredits.Add(oBOI);
            }
            if (oBOIPolicies.Count > 0)
            {
                var Short = oAgedCredits.Select(x => x.AttributeI("Short").doValue).Sum();
                var Medium = oAgedCredits.Select(x => x.AttributeI("Medium").doValue).Sum();
                var Long = oAgedCredits.Select(x => x.AttributeI("Long").doValue).Sum();
                var LongPlus = oAgedCredits.Select(x => x.AttributeI("Long+").doValue).Sum();
                var Balance = oAgedCredits.Select(x => x.AttributeI("Balance").doValue).Sum();
                oBOI = new XIIBO();
                oBOI.BOD = oBOD;
                oBOI.Attributes["Client"] = new XIIAttribute { sName = "FKiCustomerID", sValue = "", bDirty = true };
                oBOI.Attributes["Supplier"] = new XIIAttribute { sName = "FKiSupplierID", sValue = "", bDirty = true };
                oBOI.Attributes["Status"] = new XIIAttribute { sName = "iStatus", sValue = "", bDirty = true };
                oBOI.Attributes["Type"] = new XIIAttribute { sName = "iType", sValue = "", bDirty = true };
                oBOI.Attributes["Date"] = new XIIAttribute { sName = "dDate", sValue = "", bDirty = true };
                oBOI.Attributes["Short"] = new XIIAttribute { sName = "rShort", sValue = Short.ToString(), bDirty = true };
                oBOI.Attributes["Medium"] = new XIIAttribute { sName = "rMedium", sValue = Medium.ToString(), bDirty = true };
                oBOI.Attributes["Long"] = new XIIAttribute { sName = "rLong", sValue = Long.ToString(), bDirty = true };
                oBOI.Attributes["Long+"] = new XIIAttribute { sName = "rLongPlus", sValue = LongPlus.ToString(), bDirty = true };
                oBOI.Attributes["Balance"] = new XIIAttribute { sName = "rTotal", sValue = Math.Round(Balance, 2).ToString(), bDirty = true };
                oBOI.Attributes["Policy"] = new XIIAttribute { sName = "FKiACPolicyID", sValue = "", bDirty = true };
                oBOI.Attributes["Enterprise"] = new XIIAttribute { sName = "FKiEnterpriseID", sValue = "", bDirty = true };
                oBOI.Attributes["Account Category"] = new XIIAttribute { sName = "refAccountCategory", sValue = "", bDirty = true };
                oBOI.Attributes["Name"] = new XIIAttribute { sName = "sName", sValue = "", bDirty = true };
                oAgedCredits.Add(oBOI);
            }
            oCResult.oResult = oAgedCredits;
            //XIIBO xibulk = new XIIBO();
            //DataTable dtbulk = xibulk.MakeBulkSqlTable(oAgedCredits);
            //var resp = xibulk.SaveBulk(dtbulk, oAgedCredits[0].BOD.iDataSource, oAgedCredits[0].BOD.TableName);
            return oCResult;
        }
        public Dictionary<string, double> iBO_getAttrAGECreditors(XIIBO oPolicy, string pstrAttr, Dictionary<string, XIIValue> XIIValues)
        {
            CResult oCResult = new CResult();
            double rAgedDebt;
            int iDayRangeFrom = 0;
            int iDayRangeTo = 0;
            DateTime dDateFrom = DateTime.Now;
            DateTime dDateTo = DateTime.Now;
            XIIBO oClaim = new XIIBO();
            CResult tRtn = new CResult();
            int iSelect = 0;
            int iShort = 0;
            int iMedium = 0;
            int iLong = 0;
            bool bOverride = false;
            Dictionary<string, XIIBO> oColClaim = new Dictionary<string, XIIBO>();
            XIIBO oDriver = new XIIBO();
            XIIBO oE = new XIIBO();
            Dictionary<string, XIIBO> oColE = new Dictionary<string, XIIBO>();
            double rAgedDebtCR = 0;
            double rAgedDebtDR = 0;
            XIIBO oClient = new XIIBO();
            int iADType = 0;
            XIIBO oJE = new XIIBO();
            long iAgedDebtType;
            XIIBO oRelationship = new XIIBO();
            DateTime dDateAsOf = DateTime.Now;
            string sDateAsOf;
            double rShort = 0;
            double rMedium = 0;
            double rLong = 0;
            double rLongPlus = 0;
            long j;
            string ROUTINE_NAME = "iBO_getAttr";
            XIDXI oXID = new XIDXI();
            XIDBO oBOD = new XIDBO();
            XIIXI oIXI = new XIIXI();
            XIInstanceBase oIB = new XIInstanceBase();
            QueryEngine oQE = new QueryEngine();
            CResult oQResult = new CResult();
            XIInfraCache oCache = new XIInfraCache();
            Dictionary<string, double> oDictionary = new Dictionary<string, double>();
            oDictionary.Add("short", 0);
            oDictionary.Add("medium", 0);
            oDictionary.Add("long", 0);
            oDictionary.Add("longplus", 0);
            var JournalEntryClick = new Dictionary<string, XIIBO>();
            string FinalQuery = "select * from (";
            try
            {
                switch (pstrAttr.ToLower())
                {
                    case "_rshort":
                        iSelect = Convert.ToInt32(XIIValues["oSelect"].sValue == "" ? "0" : XIIValues["oSelect"].sValue);
                        //iSelect = double.Parse(i.zX.quickContext.getEntry("-ad.Select"));
                        if (iSelect > 0)
                        {
                            switch (iSelect)
                            {
                                case 10:
                                    // 15, 30, 45
                                    iShort = 15;
                                    iMedium = 30;
                                    iLong = 45;
                                    bOverride = true;
                                    break;
                                case 20:
                                    // 30, 60, 90
                                    iShort = 30;
                                    iMedium = 60;
                                    iLong = 90;
                                    bOverride = true;
                                    break;
                                case 30:
                                    // 45, 90, 135
                                    iShort = 45;
                                    iMedium = 90;
                                    iLong = 135;
                                    bOverride = true;
                                    break;
                                case 40:
                                    // 60, 120, 180
                                    iShort = 60;
                                    iMedium = 120;
                                    iLong = 180;
                                    bOverride = true;
                                    break;
                            }
                        }
                        else
                        {
                            // DS: We have a logic problem here - on the search you want to specify the amounts, whereas on the popup you want to override, so on popup make sure you specify the std option "-ad.Select"=10
                            if (iSelect == 0)
                            //if (double.Parse(i.zX.quickContext.getEntry("-ad.Short")) == 0)
                            {
                                iShort = 15;
                                iMedium = 30;
                                iLong = 45;
                                bOverride = true;
                            }
                            else
                            {
                                bOverride = false;
                            }
                        }
                        sDateAsOf = XIIValues["dAsOfDate"].sValue;
                        //sDateAsOf = i.zX.quickContext.getEntry("-date.AsOf");
                        if (sDateAsOf != "")
                        {
                            //if (IsDate(sDateAsOf))
                            //{
                            dDateAsOf = DateTime.Parse(sDateAsOf);
                            //}
                        }
                        for (j = 1; j <= 4; j++)
                        {
                            switch (j)
                            {
                                case 4:
                                    // "_rshort"
                                    iDayRangeFrom = 0;
                                    if (bOverride)
                                    {
                                        iDayRangeTo = iShort;
                                    }
                                    else
                                    {
                                        iDayRangeTo = Convert.ToInt32(XIIValues["iShort"].sValue);
                                    }

                                    if (iDayRangeTo == 0)
                                    {
                                        iDayRangeTo = 29;
                                    }

                                    iDayRangeTo = iDayRangeTo * -1;
                                    dDateFrom = dDateAsOf.AddDays(iDayRangeFrom);
                                    dDateTo = dDateAsOf.AddDays(iDayRangeTo);
                                    break;
                                case 3:
                                    // "_rmedium"
                                    if (bOverride)
                                    {
                                        iDayRangeFrom = iShort;
                                    }
                                    else
                                    {
                                        iDayRangeFrom = Convert.ToInt32(XIIValues["iShort"].sValue);
                                    }

                                    if (iDayRangeFrom == 0)
                                    {
                                        iDayRangeFrom = 30;
                                    }

                                    iDayRangeFrom = (iDayRangeFrom * -1);
                                    if (bOverride)
                                    {
                                        iDayRangeTo = iMedium;
                                    }
                                    else
                                    {
                                        iDayRangeTo = Convert.ToInt32(XIIValues["iMedium"].sValue);
                                    }

                                    if (iDayRangeTo == 0)
                                    {
                                        iDayRangeTo = 59;
                                    }

                                    iDayRangeTo = iDayRangeTo * -1;
                                    dDateFrom = dDateAsOf.AddDays(iDayRangeFrom);
                                    dDateTo = dDateAsOf.AddDays(iDayRangeTo);
                                    break;
                                case 2:
                                    // "_rlong"
                                    if (bOverride)
                                    {
                                        iDayRangeFrom = iMedium;
                                    }
                                    else
                                    {
                                        iDayRangeFrom = Convert.ToInt32(XIIValues["iMedium"].sValue);
                                    }
                                    if (iDayRangeFrom == 0)
                                    {
                                        iDayRangeFrom = 60;
                                    }
                                    iDayRangeFrom = iDayRangeFrom * -1;
                                    if (bOverride)
                                    {
                                        iDayRangeTo = iLong;
                                    }
                                    else
                                    {
                                        iDayRangeTo = Convert.ToInt32(XIIValues["iLong"].sValue);
                                    }

                                    if (iDayRangeTo == 0)
                                    {
                                        iDayRangeTo = 89;
                                    }
                                    iDayRangeTo = iDayRangeTo * -1;
                                    dDateFrom = dDateAsOf.AddDays(iDayRangeFrom);
                                    dDateTo = dDateAsOf.AddDays(iDayRangeTo);
                                    break;
                                case 1:
                                    // "_rlongplus"
                                    // get the days from the querystring
                                    if (bOverride)
                                    {
                                        iDayRangeFrom = iLong;
                                    }
                                    else
                                    {
                                        iDayRangeFrom = Convert.ToInt32(XIIValues["iLong"].sValue);
                                    }
                                    if (iDayRangeFrom == 0)
                                    {
                                        iDayRangeFrom = 90;
                                    }
                                    iDayRangeFrom = iDayRangeFrom * -1;
                                    iDayRangeTo = -40000;
                                    //  i think 1899 is earliest year
                                    dDateFrom = dDateAsOf.AddDays(iDayRangeFrom);
                                    dDateTo = dDateAsOf.AddDays(iDayRangeTo);
                                    break;
                                    //case "_radbalance":
                                    //    iDayRangeFrom = 0;
                                    //    iDayRangeTo = -40000;
                                    //    //  i think 1899 is earliest year
                                    //    dDateFrom = dDateAsOf.AddDays(iDayRangeFrom);
                                    //    dDateTo = dDateAsOf.AddDays(iDayRangeTo);
                                    //    break;
                            }
                            // DS 06.06.09 - i think this has to be changed to 2 transactions where the sum is done twice - once for CR and once for DR
                            // DS 06.06.09 - as of now not from DB - why? Well maybe due to not having fixed short /med/long day ranges
                            // On Error Resume Next
                            iADType = 10;
                            //iADType = double.Parse(i.zX.quickContext.getEntry("-ADType"));
                            if (iADType == 0)
                            {
                                oClient = oIXI.BOI("Customer_T", oPolicy.AttributeI("FKiCustomerID").sValue, "id,FKiACAccountID");
                                if (oClient == null)
                                {
                                    oCResult.sMessage = oCResult.sClassName + " - " + ROUTINE_NAME + " - CANNOT LOAD CLIENT";
                                }
                            }
                            else if (iADType == 10)
                            {
                                // even though should be oSupplier, keep oClient so code below is same for both
                                var oProduct = oIXI.BOI("Product", oPolicy.AttributeI("FKiProductID").sValue, "id,FKiSupplierID");
                                if (oProduct == null)
                                {
                                    oCResult.sMessage = oCResult.sClassName + " - " + ROUTINE_NAME + " - CANNOT LOAD Product";
                                }
                                else
                                {
                                    oClient = oIXI.BOI("Supplier_T", oProduct.AttributeI("FKiSupplierID").sValue, "id,FKiACAccountID");
                                    if (oClient == null)
                                    {
                                        oCResult.sMessage = oCResult.sClassName + " - " + ROUTINE_NAME + " - CANNOT LOAD SUPPLIER";
                                    }
                                }
                            }
                            iAgedDebtType = 0;

                            string Query = "";
                            Query = " SELECT " + j + " as 'Credit',[ACJournalEntry_T].[iType],SUM([ACJournalEntry_T].[rAmount]) as rAmount FROM [ACJournalEntry_T] inner join [ACTransaction_T] on [ACJournalEntry_T].FKiTransactionID=[ACTransaction_T].id";
                            Query += " WHERE(1 = 1) AND cast([ACJournalEntry_T].[dDate] as date) > '" + dDateTo.ToString("yyyy-MM-dd") + "' AND cast([ACJournalEntry_T].[dDate] as date) <= '" + dDateFrom.ToString("yyyy-MM-dd") + "'";
                            Query += " AND [ACJournalEntry_T].[FKiAccountID] = " + oClient.AttributeI("FKiACAccountID").sValue;
                            Query += " AND ([ACJournalEntry_T].[iStatus] = 0 OR [ACJournalEntry_T].[iStatus] = 30) AND [ACJournalEntry_T].[iReconciled] = 10";
                            Query += " AND [ACJournalEntry_T].[FKiACPolicyID] = " + oPolicy.AttributeI("id").sValue + " AND ([ACTransaction_T].[id] = [ACJournalEntry_T].[FKiTransactionID])";
                            Query += " AND ([ACTransaction_T].[iStatus] = 0 OR [ACTransaction_T].[iStatus] = 30)";
                            if (iAgedDebtType == 10)
                            {
                                // PF only
                                Query += " AND [ACJournalEntry_T].[iTransType] = 200";
                            }
                            Query += " Group by [ACJournalEntry_T].[iType]";

                            FinalQuery += Query + " Union";
                        }
                        FinalQuery = FinalQuery.Substring(0, FinalQuery.Length - 5);
                        FinalQuery = FinalQuery + ")AS SourceTable PIVOT  (sum(ramount) FOR iType IN ([10],[20])) AS PivotTable";
                        XID1Click oXI1Click = new XID1Click();
                        oXI1Click.Query = FinalQuery;
                        oXI1Click.Name = "ACJournalEntry_T";
                        var Result = oXI1Click.Execute_Query();
                        if (Result.Rows.Count > 0)
                        {
                            for (j = 1; j <= 4; j++) //10 Dr & 20 Cr
                            {
                                var res = Result.Select("Credit=" + j).FirstOrDefault();
                                if (res == null)
                                {
                                    rAgedDebtCR = 0;
                                    rAgedDebtDR = 0;
                                }
                                else
                                {
                                    rAgedDebtCR = Convert.ToDouble(res.ItemArray[1] == DBNull.Value ? 0 : res.ItemArray[1]);
                                    rAgedDebtDR = Convert.ToDouble(res.ItemArray[2] == DBNull.Value ? 0 : res.ItemArray[2]);
                                }
                                rAgedDebt = rAgedDebtCR - rAgedDebtDR;
                                // 2012.04.16 - ok PROBLEM is we do not know if short is executed before medium etc
                                //    possible solution to that is to change the system so that all 3 are calculated at the same time and then assigned. so this getattr would only trigger on say _short, the others would be automatically calculated
                                switch (j)
                                {
                                    case 4:
                                        rShort = rAgedDebt;
                                        break;
                                    case 3:
                                        rMedium = rAgedDebt;
                                        break;
                                    case 2:
                                        rLong = rAgedDebt;
                                        break;
                                    case 1:
                                        rLongPlus = rAgedDebt;
                                        break;
                                }
                                //             i.zX.BOS.SetAttr Me, pstrAttr, i.zX.dblValue(rAgedDebt)
                            }
                        }

                        // OK, now compare long plus all the way back to short and stack up in reverse in each case. Not just one step at a time, but re-check the whole sequence (for example long may be �90, medium zero and short �30, so that would mean that
                        //   long is �60 and medium and short zero when done
                        for (j = 2; j <= 4; j++)
                        {
                            // not 1 to 4 as for long plus there is no figure to adjust
                            switch (j)
                            {
                                case 2:
                                    // long
                                    if ((rLong > 0 && rLongPlus < 0) || (rLong < 0 && rLongPlus > 0))
                                    {
                                        if (Math.Abs(rLongPlus) > Math.Abs(rLong))
                                        {
                                            rLongPlus = rLongPlus + rLong;
                                            rLong = 0;
                                        }
                                        else
                                        {
                                            rLong = rLong + rLongPlus;
                                            rLongPlus = 0;
                                        }
                                    }
                                    break;
                                case 3:
                                    // medium
                                    if ((rMedium > 0 && rLongPlus < 0) || (rMedium < 0 && rLongPlus > 0))
                                    {
                                        if (Math.Abs(rLongPlus) > Math.Abs(rMedium))
                                        {
                                            rLongPlus = rLongPlus + rMedium;
                                            rMedium = 0;
                                        }
                                        else
                                        {
                                            rMedium = rMedium + rLongPlus;
                                            rLongPlus = 0;
                                        }

                                    }

                                    if ((rMedium > 0 && rLong < 0) || (rMedium < 0 && rLong > 0))
                                    {
                                        if (Math.Abs(rLong) > Math.Abs(rMedium))
                                        {
                                            rLong = rLong + rMedium;
                                            rMedium = 0;
                                        }
                                        else
                                        {
                                            rMedium = rMedium + rLong;
                                            rLong = 0;
                                        }
                                    }
                                    break;
                                case 4:
                                    // short
                                    if ((rShort > 0 && rLongPlus < 0) || (rShort < 0 && rLongPlus > 0))
                                    {
                                        if (Math.Abs(rLongPlus) > Math.Abs(rShort))
                                        {
                                            rLongPlus = rLongPlus + rShort;
                                            rShort = 0;
                                        }
                                        else
                                        {
                                            rShort = rShort + rLongPlus;
                                            rLongPlus = 0;
                                        }
                                    }

                                    if ((rShort > 0 && rLong < 0) || (rShort < 0 && rLong > 0))
                                    {
                                        if (Math.Abs(rLong) > Math.Abs(rShort))
                                        {
                                            rLong = rLong + rShort;
                                            rShort = 0;
                                        }
                                        else
                                        {
                                            rShort = rShort + rLong;
                                            rLong = 0;
                                        }
                                    }

                                    if ((rShort > 0 && rMedium < 0) || (rShort < 0 && rMedium > 0))
                                    {
                                        if (Math.Abs(rMedium) > Math.Abs(rShort))
                                        {
                                            rMedium = rMedium + rShort;
                                            rShort = 0;
                                        }
                                        else
                                        {
                                            rShort = rShort + rMedium;
                                            rMedium = 0;
                                        }
                                    }

                                    break;
                            }
                        }
                        //var _rShort = rShort.ToString();
                        //var _rMedium = rMedium.ToString();
                        //var _rLong = rLong.ToString();
                        //var _rLongPlus = rLongPlus.ToString();
                        oDictionary["short"] = Math.Round(rShort, 2);
                        oDictionary["medium"] = Math.Round(rMedium, 2);
                        oDictionary["long"] = Math.Round(rLong, 2);
                        oDictionary["longplus"] = Math.Round(rLongPlus, 2);
                        //i.SetAttr("_rShort", rShort.ToString());
                        //i.SetAttr("_rMedium", rMedium);
                        //i.SetAttr("_rLong", rLong);
                        //i.SetAttr("_rLongPlus", i.zX.dblValue(rLongPlus);
                        //2012.04.16 - everthing after this line was added back in from the original code JUST to handle _radbalance. BUT it is the original code, so easy to put back
                        break;
                }
            }
            catch (Exception e)
            {
                oCResult.sMessage = oCResult.sClassName + ", " + ROUTINE_NAME + ", Error for attr: \'" + pstrAttr + "\'\r\n";
                //return i.zX.BOS.GetAttr(this, pstrAttr);
                //return oCResult;
            }
            return oDictionary;// i.zX.BOS.GetAttr(this, pstrAttr);
        }
    }
}