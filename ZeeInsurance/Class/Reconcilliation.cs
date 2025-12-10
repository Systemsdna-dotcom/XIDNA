using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XISystem;
using XICore;
using System.Data.SqlClient;
using System.Diagnostics;
using static XIDatabase.XIDBAPI;
using System.Reflection;

namespace ZeeInsurance
{
    public class Reconcilliation
    {
        public SqlConnection TXSqlconn = null;
        public SqlTransaction TXSqlTrans = null;
        private TransactionInitiation TXinitial;
        JournalTransactions oJTrans = new JournalTransactions();
        public Reconcilliation()
        {
            TXinitial = new TransactionInitiation();
            //this.TXSqlconn = TXinitial.TXSqlconn;
            //this.TXSqlTrans = TXinitial.TXSqltrans;
            TXinitial.sTXBeginAt = "RECONC";
        }
        XIIBO oBatchTransaction = new XIIBO();
        double rBatchTotal = 0;
        StringBuilder sError = new StringBuilder();
        XIInfraCache oCache = new XIInfraCache();
        private string _sSessionID = "";
        public string sSessionID
        {
            get { return _sSessionID; }
            set { _sSessionID = value; }
        }
        private string _sGUID = "";
        public string sGUID
        {
            get { return _sGUID; }
            set { _sGUID = value; }
        }
        public string _FKsUserID = "";
        public string FKsUserID
        {
            get { return _FKsUserID; }
            set { _FKsUserID = value; }
        }
        public string sDataBase = null;
        public CResult PostReconciliation(List<CNV> oParams)
        {
            //TXinitial.sTXBeginAt = "PROCESSRECONC";
            CResult oCResult = new CResult();
            XIInstanceBase oIB = new XIInstanceBase();
            try
            {
                _sSessionID = oParams.Where(t => t.sName == "sSessionID").Select(f => f.sValue).FirstOrDefault();
                _sGUID = oParams.Where(t => t.sName == "sGUID").Select(f => f.sValue).FirstOrDefault();
                string ReconciliationID = oParams.Where(t => t.sName == "{XIP|ACReconcilliation_T.id}").Select(f => f.sValue).FirstOrDefault();
                string ActRec = oParams.Where(t => t.sName == "{XIP|ActRec}").Select(f => f.sValue).FirstOrDefault();
                sDataBase = oParams.Where(t => t.sName == "sDataBase").Select(f => f.sValue).FirstOrDefault();
                FKsUserID = oParams.Where(t => t.sName == "iUserID").Select(f => f.sValue).FirstOrDefault();
                var Reconcilliation = PreProcess(Convert.ToInt32(ReconciliationID), ActRec);
                oCResult.oResult = Reconcilliation;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oIB.SaveErrortoDB(oCResult);
            }
            //if (TXinitial.sTXBeginAt == "PROCESSRECONC")
            //    TXinitial.TXCommitRollback(oCResult);
            return oCResult;
        }
        #region PreProcess
        public CResult PreProcess(int ReconciliationID, string actrec = "")
        {
            CResult oCResult = new CResult();
            XIInstanceBase oIB = new XIInstanceBase();
            try
            {
                decimal rAlreadyReconciled = 0;
                decimal rTotalToReconcile = 0;
                decimal rDifference = 0;
                decimal rPremissableDifference = 0;
                decimal rTransAmount = 0;
                int iReconcileType = 0;
                decimal rLeftToReconcile = 0;
                string sDiffInsParam = "";
                int iAccCat = 0;
                string sAccCatSeq = "";
                string ROUTINE_NAME = "[PreProcess]";

                CResult oCR = new CResult();
                // TODO: On Error GoTo Warning!!!: The statement is not translatable 
                XIIXI oIXI = new XIIXI();
                XIIBO obj = oIXI.BOI("ACReconcilliation_T", ReconciliationID.ToString(), "iStatus", null);
                string iStatus = obj.AttributeI("iStatus").iValue.ToString();
                if (Convert.ToInt32(iStatus) > 0)
                {
                    oCResult.sMessage = ROUTINE_NAME + ", Reconciliation already Posted";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oCResult.sMessage = ROUTINE_NAME + ", Reconciliation already Posted";
                    oIB.SaveErrortoDB(oCResult);
                    return oCResult;
                }

                List<XIIBO> oBOIXReconciliation = new List<XIIBO>();
                XIDXI oXIDXReconciliation = new XIDXI();
                QueryEngine oQE = new QueryEngine();
                string sWhereCondition = "FKiACReconcilliationID=" + ReconciliationID.ToString() + ","+ XIConstant.Key_XIDeleted + "=0";
                var oQResult = oQE.Execute_QueryEngine("ACXReconcilliation_T", "*", sWhereCondition);
                if (oQResult.bOK && oQResult.oResult != null)
                {
                    oBOIXReconciliation = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.ToList();
                }
                if (oBOIXReconciliation == null && oBOIXReconciliation.Count() < 1)
                {
                    oCResult.sMessage = ROUTINE_NAME + ", Unable to get Collection ";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oCResult.sMessage = ROUTINE_NAME + ", Unable to get Collection ";
                    oIB.SaveErrortoDB(oCResult);
                }
                if (string.IsNullOrEmpty(actrec))
                    actrec = "insurerpay";
                switch (actrec.ToLower())
                {
                    case "reconcilepf":
                        sDiffInsParam = "REC.PF.DIFF";
                        break;
                    case "commissiondrawdown":
                        sDiffInsParam = "REC.COMMISSION.DIFF";
                        break;
                    case "insurerpay":
                        sDiffInsParam = "REC.SUPPLIER.DIFF";
                        break;
                    case "chargesdrawdown":
                        sDiffInsParam = "REC.CHARGES.DIFF";
                        break;
                    case "prebankrec":
                        sDiffInsParam = "REC.PREBANK.DIFF";
                        break;
                    case "bankrec":
                        sDiffInsParam = "REC.BANK.DIFF";
                        break;
                    case "supplierpayupm":
                        sDiffInsParam = "REC.SUPPLIER.DIFF";
                        break;
                    case "reconcilebr":
                        sDiffInsParam = "REC.BR.DIFF";
                        break;
                    case "pfpayrec":
                        sDiffInsParam = "REC.PFPAY.DIFF";
                        break;
                    case "ffpayrec":
                        sDiffInsParam = "REC.PFPAY.DIFF";
                        break;
                }
                if (sDiffInsParam == "")
                {
                    rPremissableDifference = 10;
                }
                else
                {
                    rPremissableDifference = GetGlobalParam(sDiffInsParam);
                }
                sAccCatSeq = ",";
                XIIBO oJE = new XIIBO();
                foreach (var oBOIXReconciliations in oBOIXReconciliation)
                {
                    iReconcileType = oBOIXReconciliations.AttributeI("iType").iValue;
                    oJE = oIXI.BOI("ACJournalEntry_T", oBOIXReconciliations.AttributeI("FKiACJEID").sValue, "refAccountCategory, rReconciled, rAmount, id, iStatus, iReconciled", null);
                    if (oJE == null)
                    {
                        sError.Append("No matching Journal found for Reconcilliation id=" + oBOIXReconciliations.AttributeI("id").sValue + "<BR>");
                    }
                    else
                    {
                        iAccCat = oJE.AttributeI("refAccountCategory").iValue;
                        if (iAccCat > 0)
                        {
                            if (sAccCatSeq.IndexOf("," + iAccCat + ",", 0) + 1 == 0)
                            {
                                sAccCatSeq = sAccCatSeq + iAccCat + ",";
                            }
                        }
                        else
                        {
                            sError.Append("INVALID ACCOUNT CATEGORY, for Reconcilliation id=" + oBOIXReconciliations.AttributeI("id").sValue + ", Journal id=" + oJE.AttributeI("id").iValue + "<BR>");
                        }
                        // Begin Journal entries iStatus,iReconciled condition checking 
                        if (oJE.AttributeI("iReconciled").iValue >= 20)
                        {
                            sError.Append("The journal entry alreay has been reconciled Partially or Fully, for Reconcilliation id=" + oBOIXReconciliations.AttributeI("id").sValue + ", Journal id=" + oJE.AttributeI("id").iValue + "<BR>");
                        }
                        else if (oJE.AttributeI("iStatus").sValue == "10" || oJE.AttributeI("iStatus").sValue == "20")
                        {
                            sError.Append("The journal entry has been Cancelled. So that we can't reconcile the Journal, for Reconcilliation id=" + oBOIXReconciliations.AttributeI("id").sValue + ", Journal id=" + oJE.AttributeI("id").iValue + "<BR>");
                        }
                        // End Journal entries iStatus,iReconciled condition checking 
                        rAlreadyReconciled = oJE.AttributeI("rReconciled").rValue;
                        rTotalToReconcile = oJE.AttributeI("rAmount").rValue;
                        rLeftToReconcile = (rTotalToReconcile - rAlreadyReconciled);
                        rTransAmount = Math.Abs(oBOIXReconciliations.AttributeI("rAmountReconciled").rValue);
                        rDifference = (rTotalToReconcile - rTransAmount);
                        if (oBOIXReconciliations.AttributeI("iType").iValue == 10)
                        {
                            // partial match
                            // TO DO - this is a partial match, so it is ok to have a difference, but it is not ok for the total of all partials to be abs greater than the total for the rec
                        }
                        else if (((rDifference < (rPremissableDifference * -1)) && (rTotalToReconcile > 0)) || ((rDifference > rPremissableDifference) && (rTotalToReconcile < 0)))
                        {
                            sError.Append("Amount reconciled is greater than total amount for Reconcilliation id=" + oBOIXReconciliations.AttributeI("id").sValue + ", Journal id=" + oJE.AttributeI("id").iValue + "<BR>");
                        }
                        else if ((Math.Abs(rLeftToReconcile) + Math.Abs(rPremissableDifference)) < Math.Abs(rTransAmount))
                        {
                            // if for example close match is 680 and actual is 675, so we need to factor in the premiss diff
                            sError.Append("Amount remaining for reconciliation is less than the allocated amount, for Reconciliation id=" + oBOIXReconciliations.AttributeI("id").sValue + ", Journal id=" + oJE.AttributeI("id").iValue + "<BR>");
                        }
                        else if ((Math.Abs(rDifference) > Math.Abs(rPremissableDifference)) && (iReconcileType != 10))
                        {
                            sError.Append("Difference of \u00A3" + rDifference + " is greater than permissable difference of \u00A3" + rPremissableDifference + ", for Reconciliation id="
                                        + oBOIXReconciliations.AttributeI("id").sValue + ", Journal id=" + oJE.AttributeI("id").iValue + "<BR>");
                        }
                        else
                        {
                            // iReconcileType=10 - partial
                            // all checks already done i think
                        }
                    }
                    if (sAccCatSeq.Length < 2)
                    {
                        sAccCatSeq = null;
                    }
                }
                oCResult.oResult = sError;

                if (!string.IsNullOrEmpty(sError.ToString()))
                {
                    oCResult.sMessage = ROUTINE_NAME + "," + sError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oCResult.sMessage = ROUTINE_NAME + "," + sError;
                    oIB.SaveErrortoDB(oCResult);
                }
                else
                {
                    ProcessReconciallition(sDiffInsParam, Convert.ToInt32(iStatus), Convert.ToInt32(ReconciliationID), sAccCatSeq, DateTime.Now.ToString(), actrec);
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
            return oCResult;
        }
        public static decimal GetGlobalParam(string ComSettings)
        {
            // * TO DO Load from DB
            XIIXI oIXI = new XIIXI();
            decimal Difference = 0;
            List<CNV> Params = new List<CNV>();
            Params.Add(new CNV { sName = "sCode", sValue = ComSettings });
            XIIBO oBOI = oIXI.BOI("COMGlobalSettings_T", null, "Description", Params);
            return Difference = oBOI.AttributeI("sValue").rValue;
        }
        #endregion
        #region Reconciallation_Process
        public CResult GetXReconcilliationDetails(int ReconciliationID, string cat)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            XIInstanceBase oIB = new XIInstanceBase();
            try
            {
                Dictionary<string, XIIBO> oBOIXReconciliation = new Dictionary<string, XIIBO>();
                XIDXI oXIDXReconciliation = new XIDXI();
                QueryEngine oQE = new QueryEngine();
                string sWhereCondition = "FKiACReconcilliationID=" + ReconciliationID.ToString() + ","+ XIConstant.Key_XIDeleted + "=0,FKiEnterpriseID = 2,refAccountCategory=" + cat.ToString();
                var oQResult = oQE.Execute_QueryEngine("ACXReconcilliation_T", "*", sWhereCondition);
                if (oQResult.bOK && oQResult.oResult != null)
                {
                    oBOIXReconciliation = (Dictionary<string, XIIBO>)oQResult.oResult;
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oBOIXReconciliation;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oIB.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
        public void ProcessReconciallition(string sName, int iStatus, int ReconciliationID, string sAccCatSeq, string dPostingDate, string sMyAction)
        {
            // NOTE : PARTIALLY DONE. NEED TO WORK ON IT(FINAL TRANSACTION IS PENDING)
            oJTrans = new JournalTransactions();
            const string ROUTINE_NAME = "[Process]";
            bool bLocalTrans;
            double rReconciled;
            double rNewReconciled = 0;
            long iReconcileType;
            double rTransAmount;
            double rTotalToReconcile = 0;
            decimal rAdjustment;
            decimal rTotalAdjustment;
            int iEnterpriseID = 2;
            string[] AsEnterprises;
            long j;
            string sEnterprise;
            string sHTMLResult;
            decimal rTotalPremium = 0;
            decimal rTotalIPT = 0;
            decimal rTotalCommission = 0;
            decimal rTotalAdjustments = 0;
            decimal rTotalNet = 0;
            double rTotalPaid;
            string sManualError = "";
            int iJEType = 0;
            List<string> AsAccCats = new List<string>();
            string sThisAccCat;
            long iAccCatID;
            string sDateOverride;
            string sStage;
            bool bPostCheck = false;
            CResult oCResult = new CResult();
            XIDefinitionBase oXID = new XIDefinitionBase();
            XIIBO oFinalTrans = new XIIBO();
            XIIBO oboiPol = new XIIBO();
            CResult ReconcilliationUpdate = new CResult();
            XIDStructure oXIDStructure = new XIDStructure();
            CResult tRC = new CResult();
            try
            {
                if (bPostCheck == true)
                {
                    // Or iPostCount > 1 Then
                    // type 20 made up - means finance trans. criticality 50 means important but this does happen
                    oCResult.sMessage = ROUTINE_NAME + ",Reconciliation Double Post Jump out rec double post - id =" + ReconciliationID;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oCResult.sMessage = ROUTINE_NAME + ",Reconciliation Double Post Jump out rec double post - id =" + ReconciliationID;
                    oXID.SaveErrortoDB(oCResult);
                }
                if (iStatus > 0)
                {
                    oCResult.sMessage = ROUTINE_NAME + ",Reconciliation already Posted";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oCResult.sMessage = ROUTINE_NAME + ",Reconciliation already Posted";
                    oXID.SaveErrortoDB(oCResult);
                    return;
                }
                // /* UPDATE iStatus,sActRec in ACReconcilliation_T TABLE AS PROGRESS 

                XIDXI oxidPol = new XIDXI();
                XIDBO obodPol = (XIDBO)oxidPol.Get_BODefinition("ACReconcilliation_T").oResult;
                oboiPol.BOD = obodPol;
                oboiPol.SetAttribute("id", ReconciliationID.ToString());
                oboiPol.SetAttribute("sActRec", sMyAction);
                oboiPol.SetAttribute("iStatus", "100"); // 100 means in progress
                ReconcilliationUpdate = oboiPol.Save(oboiPol);
                if (!ReconcilliationUpdate.bOK && ReconcilliationUpdate.oResult == null)
                {
                    oCResult.sMessage = "Unable to update Reconciliation \'iStatus\' to 100, id=" + ReconciliationID;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oCResult.sMessage = "Unable to update Reconciliation \'iStatus\' to 100, id=" + ReconciliationID;
                    oXID.SaveErrortoDB(oCResult);
                }

                if (!string.IsNullOrEmpty(sAccCatSeq))
                {
                    AsAccCats = sAccCatSeq.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                if (dPostingDate != null)
                {
                    sDateOverride = dPostingDate;
                }
                else
                {
                    sDateOverride = "";
                }
                foreach (var cat in AsAccCats) //Category iteration
                {
                    if (!string.IsNullOrEmpty(cat) && Convert.ToInt32(cat) > 0)
                    {
                        rTotalAdjustment = 0;
                        // Get all XTransactions that reference this id and whose checked it true
                        //oProcessTransaction = i.zX.createBO("AC/ACTransaction");

                        // LOAD ACXReconcilliation List One click Execution Here /* TO Do
                        XIIXI oXiiRec = new XIIXI();
                        XIIBO oXiiBO = new XIIBO();
                        var oReconcilliation = new CResult();
                        oReconcilliation = GetXReconcilliationDetails(ReconciliationID, cat.ToString());
                        var Xreconcillation = (Dictionary<string, XIIBO>)oReconcilliation.oResult;

                        if (Xreconcillation == null)
                        {
                            oCResult.sMessage = "Unable to Load ACXReconcilliation_T";
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.LogToFile();
                        }
                        string TransactionID = "";
                        tRC = Process(ReconciliationID, sMyAction, 0, 0, null, null, null, null);
                        XIDXI uJExid = new XIDXI();
                        XIDBO uJEXidbo = (XIDBO)uJExid.Get_BODefinition("ACJournalEntry_T").oResult;
                        XIDXI oxidAACT = new XIDXI();
                        XIDBO obodAACT = (XIDBO)oxidAACT.Get_BODefinition("ACTransaction_T").oResult;
                        List<string> JEIDs = new List<string>();
                        XIIBO uJErec = new XIIBO();
                        XID1Click oJournal = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "Reconciled journal", null);
                        XID1Click oTransactionD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "Reconciled Transaction", null);
                        List<CNV> nParms = new List<CNV>();
                        Dictionary<string, XIIBO> oRes = new Dictionary<string, XIIBO>();
                        XIIBO oTransaction = new XIIBO();
                        XIIBO oJE = new XIIBO();
                        foreach (var recon in Xreconcillation)
                        {
                            oXiiBO = (XIIBO)recon.Value;
                            //Load Journal Entry HERE
                            int FKiACJEID = recon.Value.AttributeI("FKiACJEID").iValue;
                            nParms = new List<CNV>();
                            nParms.Add(new CNV { sName = "{XIP|FKiACJEID}", sValue = FKiACJEID.ToString() });
                            XID1Click oJournalC = (XID1Click)oJournal.Clone(oJournal);
                            oJournalC.ReplaceFKExpressions(nParms);
                            oJournalC.Query = oXIDStructure.ReplaceExpressionWithCacheValue(oJournalC.Query, nParms);
                            oRes = new Dictionary<string, XIIBO>();
                            oRes = oJournalC.OneClick_Run(false);
                            oJE = new XIIBO();
                            if (oRes != null && oRes.Count() > 0)
                            {
                                oJE = (XIIBO)oRes.Values.FirstOrDefault();
                            }
                            if (oJE != null)
                            {
                                iJEType = oJE.AttributeI("iType").iValue;
                                //LOAD TRANSACTIONS TABLE HERE
                                nParms = new List<CNV>();
                                nParms.Add(new CNV { sName = "{XIP|FKiTransactionID}", sValue = oJE.AttributeI("FKiTransactionID").sValue });
                                XID1Click oTransactionC = (XID1Click)oTransactionD.Clone(oTransactionD);
                                oTransactionC.ReplaceFKExpressions(nParms);
                                oTransactionC.Query = oXIDStructure.ReplaceExpressionWithCacheValue(oTransactionC.Query, nParms);
                                oRes = new Dictionary<string, XIIBO>();
                                oRes = oTransactionC.OneClick_Run(false);
                                oTransaction = new XIIBO();
                                if (oRes != null && oRes.Count() > 0)
                                {
                                    oTransaction = (XIIBO)oRes.Values.FirstOrDefault();
                                }
                                TransactionID = oTransaction.AttributeI("id").sValue;
                                XIDBO oTransactionBOD = new XIDBO();
                                oTransactionBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "", oTransactionC.BOIDXIGUID.ToString());
                                oTransaction.BOD = oTransactionBOD;

                                // CALLING PROCESS METHOD HERE
                                tRC = Process(ReconciliationID, sMyAction, 10, iJEType, oTransaction, sDateOverride, oboiPol, (XIIBO)recon.Value);
                                if (iJEType == 20)
                                {
                                    // subtract
                                    if (oTransaction.AttributeI("zBaseValue").doValue < 0)
                                    {
                                        rTotalPremium = (rTotalPremium + Convert.ToDecimal(oTransaction.AttributeI("zBaseValue").doValue));
                                    }
                                    else
                                    {
                                        rTotalPremium = (rTotalPremium + Convert.ToDecimal(oTransaction.AttributeI("zBaseValue").doValue * -1));
                                    }
                                }
                                else if (oTransaction.AttributeI("zBaseValue").doValue < 0)
                                {
                                    rTotalPremium = (rTotalPremium + Convert.ToDecimal(oTransaction.AttributeI("zBaseValue").doValue * -1));
                                }
                                else
                                {
                                    rTotalPremium = (rTotalPremium + Convert.ToDecimal(oTransaction.AttributeI("zBaseValue").doValue));
                                }
                                rTotalIPT = (rTotalIPT + Convert.ToDecimal(oTransaction.AttributeI("rTax").doValue));
                                rTotalCommission = (rTotalCommission + Convert.ToDecimal(oTransaction.AttributeI("rCommission").doValue));
                                rTotalNet = (rTotalNet + Convert.ToDecimal(oTransaction.AttributeI("rNetToSupplier").doValue));
                                iReconcileType = recon.Value.AttributeI("iType").iValue;
                                rReconciled = oJE.AttributeI("rReconciled").doValue;
                                rTransAmount = recon.Value.AttributeI("rAmountReconciled").doValue;
                                rTotalToReconcile = Math.Round(oJE.AttributeI("rAmount").doValue, 2);
                                rNewReconciled = rReconciled + (rTransAmount.ToString().StartsWith("-") ? rTransAmount * -1 : rTransAmount);  // / * TO DO UPDATE in JE

                                // UPDATE  in JE
                                uJErec.BOD = uJEXidbo;
                                //uJErec.SetAttribute("iType", iJEType.ToString());
                                uJErec.SetAttribute("rReconciled", rNewReconciled.ToString());
                                uJErec.SetAttribute("id", recon.Value.AttributeI("FKiACJEID").iValue.ToString());
                                if (!string.IsNullOrEmpty(oJE.AttributeI("sNotes").sValue))
                                {
                                    uJErec.SetAttribute("sNotes", sName);
                                }

                                if ((iReconcileType == 20) && (rNewReconciled != rTotalToReconcile))
                                {
                                    // this is a partial /* TO DO
                                    uJErec.SetAttribute("iReconciled", "20");
                                    // Update JE rReconciled,iReconciled,sNotes
                                    var updateJErec = uJErec.Save(uJErec);
                                    if (!updateJErec.bOK && updateJErec.oResult == null)
                                    {
                                        oCResult.sMessage = "Unable to update Journal id";
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        oCResult.LogToFile();
                                        oCResult.sMessage = "Unable to update Journal id";
                                        oXID.SaveErrortoDB(oCResult);
                                    }
                                }
                                else
                                {
                                    JEIDs.Add(recon.Value.AttributeI("FKiACJEID").iValue.ToString());
                                    uJErec.SetAttribute("iReconciled", "30");
                                    if (rNewReconciled != Math.Round(rTotalToReconcile, 2))
                                    {
                                        // post the difference as commission
                                        rAdjustment = Convert.ToDecimal(rTotalToReconcile - rNewReconciled);
                                        rTotalAdjustment = (rTotalAdjustment + rAdjustment);
                                        // oTransType = i.zX.createBO("AC/ACTransType");
                                        string _sCode = "";
                                        switch (sMyAction.ToLower())
                                        {
                                            case "reconcilepf":
                                                _sCode = "PFPY";
                                                // PFPAY
                                                break;
                                            case "commissiondrawdown":
                                                _sCode = "CMCL";
                                                // CLRCOMM
                                                break;
                                            case "insurerpay":
                                                _sCode = "CADJ";
                                                break;
                                            case "chargesdrawdown":
                                                _sCode = "CHCL";
                                                break;
                                            case "prebankrec":
                                                _sCode = "PREBREC";
                                                // DS WARNING - NOT IMPLEMENTED YET. WHEN IT IS COPY ONTO .22 test system
                                                break;
                                            case "bankrec":
                                                _sCode = "BANKREC";
                                                break;
                                            case "supplierpayupm":
                                                _sCode = "CADJ";
                                                break;
                                            case "reconcilebr":
                                                _sCode = "REAA";
                                                break;
                                            case "pfpayrec":
                                                _sCode = "PCCL";
                                                break;
                                            case "ffpayrec":
                                                _sCode = "PCCL";
                                                // PCCL  payment charge clear  '2015.04.14 - SHOULD THIS BE THE SAME AS PF?
                                                break;
                                        }

                                        var oTrantype = oJTrans.GetTransTypeID(_sCode);
                                        var Trantype = (XIIBO)oTrantype.oResult;
                                        if (!oTrantype.bOK)
                                        {
                                            oCResult.sMessage = "Cannot load Transaction Type for Policy Calculation:" + ReconciliationID;
                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                            oCResult.LogToFile();
                                            oCResult.sMessage = "Cannot load Transaction Type for Policy Calculation:" + ReconciliationID;
                                            oXID.SaveErrortoDB(oCResult);
                                        }
                                        else
                                        {
                                            // trans type loaded
                                        }
                                        XIIBO oboiAACT = new XIIBO();
                                        oboiAACT.BOD = obodAACT;
                                        if (oTrantype == null)
                                        {
                                            //          i.zX.Logger.logError CLASS_NAME & "," & ROUTINE_NAME & ", Cannot load Transaction Type for Premium Finance Reconcilliation: " & i.GetAttr("id").strValue & " and Product: " & i.GetAttr("FKiProductID").strValue
                                            //          i.zX.trace.addError CLASS_NAME, ROUTINE_NAME, "Cannot load Transaction Type for Premium Finance Reconcilliation: " & i.GetAttr("id").strValue & " and Product: " & i.GetAttr("FKiProductID").strValue
                                            //          GoTo errExit
                                        }
                                        else
                                        {
                                            // create a new transaction for reconcile and change the status (processed status is changed automatically from config)
                                            oboiAACT.SetAttribute("FKiACTransTypeID", Trantype.AttributeI("id").sValue);
                                            //  i.zX.lngValue(21)     'DANGER - '21' is a BO is for new trans - this must match the DB
                                        }
                                        oboiAACT.SetAttribute("FKiEnterpriseID", iEnterpriseID.ToString());
                                        oboiAACT.SetAttribute("sTransCode", _sCode);
                                        oboiAACT.SetAttribute("refAccountCategory", cat.ToString());
                                        //oboiAACT.SetAttribute("FKiACPolicyID", oTransaction.AttributeI("FKiACPolicyID").sValue);
                                        oboiAACT.SetAttribute("FKiSupplierID", oTransaction.AttributeI("FKiSupplierID").sValue);
                                        oboiAACT.SetAttribute("FKiProductID", oTransaction.AttributeI("FKiProductID").sValue);
                                        //oboiAACT.SetAttribute("FKiQuoteID", oTransaction.AttributeI("FKiQuoteID").sValue);
                                        oboiAACT.SetAttribute("FKiClientID", oTransaction.AttributeI("FKiClientID").sValue);
                                        oboiAACT.SetAttribute("zBaseValue", rAdjustment.ToString());
                                        oboiAACT.SetAttribute("rCommAdj", rAdjustment.ToString());
                                        oboiAACT.SetAttribute("FKsWhoID", FKsUserID);
                                        // post the transaction for the reconcilliation
                                        //var Inserttransation = oboiAACT.Save(oboiAACT);
                                        var Inserttransation = oJTrans.PostTransaction(oboiAACT, sDataBase, sSessionID, sGUID, false);
                                        if (!Inserttransation.bOK && Inserttransation.oResult == null)
                                        {
                                            oCResult.sMessage = " Unable to insert instance of AC/ACTransaction_T";
                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                            oCResult.LogToFile();
                                            oCResult.sMessage = " Unable to insert instance of AC/ACTransaction_T";
                                            oXID.SaveErrortoDB(oCResult);
                                        }

                                        // NOTE:  REMOVED ALLOCATION  CODE HERE


                                        rTotalAdjustments = (rTotalAdjustments + rAdjustment);

                                        // UPDATE rCommAdj in Transaction
                                        XIIBO uTransrcommAdj = new XIIBO();
                                        uTransrcommAdj.BOD = obodAACT;
                                        uTransrcommAdj.SetAttribute("id", recon.Value.AttributeI("FKiACTransactionID").sValue);
                                        uTransrcommAdj.SetAttribute("rCommAdj", rAdjustment.ToString());
                                        var updaterCommAdj = uTransrcommAdj.Save(uTransrcommAdj);

                                        if (!updaterCommAdj.bOK && updaterCommAdj.oResult == null)
                                        {
                                            oCResult.sMessage = "Unable to Transaction Comm Adj id=" + recon.Value.AttributeI("FKiACTransactionID").sValue;
                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                            oCResult.LogToFile();
                                            oCResult.sMessage = "Unable to Transaction Comm Adj id=" + recon.Value.AttributeI("FKiACTransactionID").sValue;
                                            oXID.SaveErrortoDB(oCResult);
                                        }
                                    }
                                    // rNewReconciled <> rTotalToReconcile
                                }
                            }
                            QueryEngine oQE = new QueryEngine();
                            string sWhereCondition = "FKiACReconcilliationID!=" + ReconciliationID.ToString() + ","+ XIConstant.Key_XIDeleted + "=0,FKiACJEID=" + FKiACJEID;
                            var oQResult = oQE.Execute_QueryEngine("ACXReconcilliation_T", "*", sWhereCondition);
                            Dictionary<string, XIIBO> oXIBOI = new Dictionary<string, XIIBO>();
                            if (oQResult.bOK && oQResult.oResult != null)
                            {
                                XIDXI oxid = new XIDXI();
                                XIDBO obod = (XIDBO)oxid.Get_BODefinition("ACXReconcilliation_T").oResult;
                                oXIBOI = (Dictionary<string, XIIBO>)oQResult.oResult;
                                foreach (var item in oXIBOI.Values)
                                {
                                    item.BOD = obod;
                                    item.SetAttribute(XIConstant.Key_XIDeleted, "1");
                                    var res = item.Save(item);
                                }
                            }
                        }//end for JE!=null

                        // Update JE rReconciled,iReconciled,sNotes
                        uJErec.sIDs = JEIDs;
                        uJErec.sFields = new List<string>();
                        uJErec.sFields.Add("rAmount");
                        uJErec.SetAttribute("rReconciled", rNewReconciled.ToString());
                        uJErec.SetAttribute("iReconciled", "30");
                        var updateJErecBulk = uJErec.Save(uJErec);
                        if (!updateJErecBulk.bOK && updateJErecBulk.oResult == null)
                        {
                            oCResult.sMessage = "Unable to update Journal id";
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.LogToFile();
                            oCResult.sMessage = "Unable to update Journal id";
                            oXID.SaveErrortoDB(oCResult);
                        }
                        oFinalTrans = oXiiRec.BOI("ACTransaction_T", TransactionID, "*", null);

                        //WARNING! This is a dummy transaction that gets replaced in the 'process' function with the real batch trans so we can get a handle to it

                        tRC = Process(ReconciliationID, sMyAction, 20, iJEType, oFinalTrans, sDateOverride, oboiPol, oXiiBO);  //tRC = Process(sMyAction, 20, oFinalTrans, , sDateOverride, Me)
                        if (oFinalTrans.AttributeI("FKiACPolicyID").iValue != 0)
                        {
                            oJTrans.Update_PolicyBalance(oFinalTrans.AttributeI("FKiACPolicyID").iValue, 10);
                        }
                        if (tRC.xiStatus == xiEnumSystem.xiFuncResult.xiError)
                        {
                            sManualError = "Error processing final Reconciliation Transaction - Check Transaction set-up";
                            oCResult.sMessage = ROUTINE_NAME + sManualError;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.LogToFile();
                        }
                        else
                        {
                            XIIBO res = new XIIBO();
                            res.SetAttribute("FKiACTransactionID", oFinalTrans.AttributeI("id").ToString());
                        }
                    }
                    // sThisAccCat<>""
                    XIIBO oBatchTransaction = (XIIBO)tRC.oResult;
                    oboiPol.SetAttribute("id", ReconciliationID.ToString());
                    oboiPol.SetAttribute("dtPosted", DateTime.Now.ToString());
                    oboiPol.SetAttribute("sActRec", sMyAction);
                    oboiPol.SetAttribute("rTotalPremium", rTotalPremium.ToString());
                    oboiPol.SetAttribute("rTotalIPT", rTotalIPT.ToString());
                    oboiPol.SetAttribute("rTotalCommission", rTotalCommission.ToString());
                    oboiPol.SetAttribute("rTotalAdjustments", rTotalAdjustments.ToString());
                    oboiPol.SetAttribute("rTotalNet", (rTotalNet - rTotalIPT).ToString());
                    oboiPol.SetAttribute("rTotalPaid", (rTotalNet - rTotalAdjustments).ToString());
                    oboiPol.SetAttribute("iStatus", 20.ToString());
                    sHTMLResult = "<p><font face='Verdana' size='2'>Reconciliation Successful</font></p>";
                    oboiPol.SetAttribute("sHTMLResult", sHTMLResult);
                    oboiPol.SetAttribute("iDirection", "0");
                    oboiPol.SetAttribute("FKiACTransactionID", oBatchTransaction.AttributeI("id").sValue);
                    oboiPol.SetAttribute("FKiSupplierID", oBatchTransaction.AttributeI("FKiSupplierID").sValue);

                    ReconcilliationUpdate = oboiPol.Save(oboiPol);
                    if (!ReconcilliationUpdate.bOK)
                    {
                        oCResult.sMessage = ROUTINE_NAME + ", Unable to update Reconciliation id=" + oboiPol.AttributeI("id").sValue;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                    }
                }
            }
            catch (Exception e)
            {
                sHTMLResult = "<p><font face='Verdana' size='4'>Error in Reconciliation: " + sManualError + " </font></p>"; //e.Description
                oboiPol.SetAttribute("sHTMLResult", sHTMLResult + e.Message);
                ReconcilliationUpdate = oboiPol.Save(oboiPol);
                if (!ReconcilliationUpdate.bOK)
                {
                    oCResult.sMessage = ROUTINE_NAME + ", Unable to update Reconciliation id=" + oboiPol.AttributeI("id").sValue; ;  //i.GetAttr("id").strValue & i.zX.trace.formatStack
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                }
            }
        }

        // Process' method on the TOTALING transaction which increments the totals and then posts the final amount. This is the method on the transaction:
        public CResult Process(int ReconciliationID, string sActionName, int iIncrementType, int iJEType, XIIBO oEntity, string sDateOverride, XIIBO oRec, XIIBO oXTransaction)
        {
            // iIncrementType = 0 for initialise , iIncrementType = 10 for increment , iIncrementType = 20 for finalise
            // Warning!!! Optional parameters not supported
            const string ROUTINE_NAME = "Process";
            double rTransactionValue = 0;
            string sFeedback = "";
            string sOpposingCode = "";
            string sAddUpdateGroup = "";
            string sTransUpdate = "";
            string sStage = "";
            double rAmountReconciled = 0;
            int iCEnterprise = 2;
            int iCSupplier = 0;
            int iCAccCat = 0;
            DateTime TempDate = new DateTime();
            XIIBO oTransTypeOp = new XIIBO();
            // TODO: On Error GoTo Warning!!!: The statement is not translatable 
            // so this will apply the specified action to this entity
            // what this function does then is for example:
            // Accounts user reviews a list of charges to draw down. She selects 10 and clicks 'drawdown' option
            //  then the system batches these up and calls this function on an EMPTY BO with parameters for this function to batch
            //  up the selected transactions to a single figure and post the appropriate journals

            XIIXI oXIIRec = new XIIXI();
            XIIBO oTransType = oXIIRec.BOI("ACTransType_T", null, null, null);
            JournalTransactions oJournalTransactions = new JournalTransactions();
            CResult oCResult = new CResult();
            XIIXI oIXI = new XIIXI();
            XIDefinitionBase oXID = new XIDefinitionBase();
            CResult oTransTypeDetails;
            if (iIncrementType == 0)
            {
                // initialise
                sStage = "Initialise";
                switch (sActionName)
                {
                    case "reconcilepf":
                        oTransType.SetAttribute("sCode", "PFPY"); // PFPAY
                                                                  //i.zX.strValue("PFPY");
                        break;
                    case "commissiondrawdown":
                        oTransType.SetAttribute("sCode", "CMCL"); // CLRCOMM
                        break;
                    case "insurerpay":
                        oTransType.SetAttribute("sCode", "PAYI"); // INSPAY
                        break;
                    case "chargesdrawdown":
                        oTransType.SetAttribute("sCode", "CHCL"); // CLRCHRG
                        break;
                    case "prebankrec":
                        oTransType.SetAttribute("sCode", "PREBREC");
                        break;
                    case "bankrec":
                        oTransType.SetAttribute("sCode", "BANKREC");
                        break;
                    case "supplierpayupm":
                        oTransType.SetAttribute("sCode", "PAYS"); // SUP PAY
                        break;
                    case "reconcilebr":
                        oTransType.SetAttribute("sCode", "REAA");
                        break;
                    case "pfpayrec":
                        oTransType.SetAttribute("sCode", "PCCL");
                        break;
                    case "ffpayrec":
                        oTransType.SetAttribute("sCode", "PCCL");
                        // 2015.04.14 - should this be same as PFPAYREC?
                        break;
                }
                oTransType.SetAttribute(XIConstant.Key_XIDeleted, 0.ToString());
                oTransTypeDetails = oJournalTransactions.GetACCTransType(oTransType.AttributeI("sCode").sValue, oTransType.AttributeI(XIConstant.Key_XIDeleted).iValue, "");
                if (!oTransTypeDetails.bOK && oTransTypeDetails.oResult == null)
                {
                    oCResult.sMessage = ROUTINE_NAME + ", Cannot load Transaction Type for code: \'" + sActionName + "\', Trans id: " + ReconciliationID;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oCResult.sMessage = ROUTINE_NAME + ", Cannot load Transaction Type for code: \'" + sActionName + "\', Trans id: " + ReconciliationID;
                    oXID.SaveErrortoDB(oCResult);
                }
                XIDXI oxidAACT = new XIDXI();
                XIDBO obodAACT = (XIDBO)oxidAACT.Get_BODefinition("ACTransaction_T").oResult;
                oBatchTransaction.BOD = obodAACT;
                if (oTransType == null)
                {
                    //          i.zX.Logger.logError CLASS_NAME & "," & ROUTINE_NAME & ", Cannot load Transaction Type for Premium Finance Reconcilliation: " & i.GetAttr("id").strValue & " and Product: " & i.GetAttr("FKiProductID").strValue
                    //          i.zX.trace.addError CLASS_NAME, ROUTINE_NAME, "Cannot load Transaction Type for Premium Finance Reconcilliation: " & i.GetAttr("id").strValue & " and Product: " & i.GetAttr("FKiProductID").strValue
                    //          GoTo errExit
                }
                else
                {
                    // create a new transaction for reconcile and change the status (processed status is changed automatically from config)
                    oBatchTransaction.SetAttribute("FKiACTransTypeID", oTransType.AttributeI("id").sValue);
                    oBatchTransaction.SetAttribute("sTransCode", oTransType.AttributeI("sCode").sValue);
                }
            }
            else if (iIncrementType == 10)
            {
                // increment
                sStage = "Increment";
                if (oBatchTransaction.AttributeI("FKiEnterpriseID").iValue == 0)
                {
                    //   .IsNull Then   -> DS: Transaction was given a default enterprise as it is now mandatory
                    oBatchTransaction.SetAttribute("FKiEnterpriseID", oEntity.AttributeI("FKiEnterpriseID").iValue.ToString());
                    // oEntity will be actransaction
                    iCEnterprise = oEntity.AttributeI("FKiEnterpriseID").iValue;
                }
                else if (iCEnterprise != oEntity.AttributeI("FKiEnterpriseID").iValue)
                {
                    oCResult.sMessage = ROUTINE_NAME + ", These Enterprises do not match Transaction id = " + ReconciliationID + " and Product:" + ReconciliationID;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oCResult.sMessage = ROUTINE_NAME + ", These Enterprises do not match Transaction id = " + ReconciliationID + " and Product:" + ReconciliationID;
                    oXID.SaveErrortoDB(oCResult);
                }

                if (string.IsNullOrEmpty(oBatchTransaction.AttributeI("refAccountCategory").sValue))
                {
                    oBatchTransaction.SetAttribute("refAccountCategory", oEntity.AttributeI("refAccountCategory").sValue);
                    // oEntity will be actransaction
                    iCAccCat = oEntity.AttributeI("refAccountCategory").iValue;
                }
                else if (iCAccCat != oEntity.AttributeI("refAccountCategory").iValue)
                {
                    oCResult.sMessage = ROUTINE_NAME + ", These refAccountCategory do not match Transaction id = " + ReconciliationID + " and Product:" + ReconciliationID;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oCResult.sMessage = ROUTINE_NAME + ", These refAccountCategory do not match Transaction id = " + ReconciliationID + " and Product:" + ReconciliationID;
                    oXID.SaveErrortoDB(oCResult);  //ProductID=i.GetAttr("FKiProductID").strValue;
                }

                // NOTE: This next section is required if the reconciliation has a section where you restrict by something (eg. supplier, PF Supplier etc)
                if (sActionName == "insurerpay")
                {
                    if (string.IsNullOrEmpty(oBatchTransaction.AttributeI("FKiSupplierID").sValue))
                    {
                        oBatchTransaction.SetAttribute("FKiSupplierID", oEntity.AttributeI("FKiSupplierID").sValue);
                        // oEntity will be actransaction
                        iCSupplier = oEntity.AttributeI("FKiSupplierID").iValue;
                    }
                    else if (iCSupplier != oEntity.AttributeI("FKiSupplierID").iValue)
                    {
                        oCResult.sMessage = ROUTINE_NAME + ", These SUPPLIERS do not match Transaction id = " + ReconciliationID;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oCResult.sMessage = ROUTINE_NAME + ", These SUPPLIERS do not match Transaction id = " + ReconciliationID;
                        oXID.SaveErrortoDB(oCResult);  //Transaction id=" + i.GetAttr("id").strValue;
                    }
                }
                else if (sActionName == "supplierpayupm")
                {
                    if (string.IsNullOrEmpty(oBatchTransaction.AttributeI("FKiSupplierID").sValue))
                    {
                        oBatchTransaction.SetAttribute("FKiSupplierID", oEntity.AttributeI("FKiSupplierID").sValue);
                        // oEntity will be actransaction
                        iCSupplier = oEntity.AttributeI("FKiSupplierID").iValue;
                    }
                    else if (iCSupplier != oEntity.AttributeI("FKiSupplierID").iValue)
                    {
                        oCResult.sMessage = ROUTINE_NAME + ", These SUPPLIERS do not match Transaction id = " + ReconciliationID;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oCResult.sMessage = ROUTINE_NAME + ", These SUPPLIERS do not match Transaction id = " + ReconciliationID;
                        oXID.SaveErrortoDB(oCResult);  //Transaction id=" + i.GetAttr("id").strValue;
                    }
                }
                else if (sActionName == "reconcilepf")
                {
                    if (string.IsNullOrEmpty(oBatchTransaction.AttributeI("FKiPFSchemeID").sValue))
                    {
                        oBatchTransaction.SetAttribute("FKiPFSchemeID", oEntity.AttributeI("FKiSupplierID").sValue);
                        // DS: UH OH!!! I DONT THINK IT SHOULD BE SUPPLIER BUT PFScheme  'oEntity will be actransaction
                        iCSupplier = oEntity.AttributeI("FKiSupplierID").iValue;
                    }
                    else if (iCSupplier != oEntity.AttributeI("FKiSupplierID").iValue)
                    {
                        oCResult.sMessage = ROUTINE_NAME + ", These PREMIUM FINANCE SUPPLIERS do not match Transaction id = " + ReconciliationID;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oCResult.sMessage = ROUTINE_NAME + ", These PREMIUM FINANCE SUPPLIERS do not match Transaction id = " + ReconciliationID;
                        oXID.SaveErrortoDB(oCResult);  //Transaction id=" + i.GetAttr("id").strValue;
                    }

                }
                else if (sActionName == "reconcilebr")
                {
                    if (string.IsNullOrEmpty(oBatchTransaction.AttributeI("FKiBrokerID").sValue))
                    {
                        oBatchTransaction.SetAttribute("FKiBrokerID", oEntity.AttributeI("FKiBrokerID").sValue);
                        // DS: UH OH!!! I DONT THINK IT SHOULD BE SUPPLIER BUT PFScheme  'oEntity will be actransaction
                        iCSupplier = oEntity.AttributeI("FKiBrokerID").iValue;
                    }
                    else if (iCSupplier != oEntity.AttributeI("FKiBrokerID").iValue)
                    {
                        oCResult.sMessage = ROUTINE_NAME + ", These BROKERS do not match Transaction id = " + ReconciliationID;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oCResult.sMessage = ROUTINE_NAME + ", These BROKERS do not match Transaction id = " + ReconciliationID;
                        oXID.SaveErrortoDB(oCResult);  //Transaction id=" + i.GetAttr("id").strValue;
                    }
                }
                switch (sActionName)
                {
                    case "reconcilepf":
                        rTransactionValue = oEntity.AttributeI("zBaseValue").doValue;
                        if (iJEType == 20)
                        {
                            if (rTransactionValue > 0)
                            {
                                rTransactionValue = (rTransactionValue * -1);
                            }
                        }
                        else if (iJEType == 10)
                        {
                            if (rTransactionValue < 0)
                            {
                                rTransactionValue = (rTransactionValue * -1);
                            }
                        }
                        break;
                    case "commissiondrawdown":
                        rTransactionValue = oEntity.AttributeI("rCommission").doValue;
                        if ((rTransactionValue == 0))
                        {
                            // might be a CADJ
                            rTransactionValue = oEntity.AttributeI("rCommAdj").doValue;
                        }
                        break;
                    case "insurerpay":
                        rTransactionValue = oEntity.AttributeI("rNetToSupplier").doValue;
                        if (!(oXTransaction == null))
                        {
                            rAmountReconciled = oXTransaction.AttributeI("rAmountReconciled").doValue;
                            if (rAmountReconciled != rTransactionValue)
                            {
                                rTransactionValue = rAmountReconciled;
                            }
                        }
                        if (iJEType == 10)
                        {
                            if (rTransactionValue > 0)
                            {
                                rTransactionValue = (rTransactionValue * -1);
                            }
                        }
                        else if (iJEType == 20)
                        {
                            if (rTransactionValue < 0)
                            {
                                rTransactionValue = (rTransactionValue * -1);
                            }
                        }
                        break;
                    case "supplierpayupm":
                        rTransactionValue = oEntity.AttributeI("zBaseValue").doValue;
                        if (iJEType == 10)
                        {
                            if (rTransactionValue > 0)
                            {
                                rTransactionValue = (rTransactionValue * -1);
                            }
                        }
                        else if (iJEType == 20)
                        {
                            if (rTransactionValue < 0)
                            {
                                rTransactionValue = (rTransactionValue * -1);
                            }
                        }
                        break;
                    case "chargesdrawdown":
                        rTransactionValue = oEntity.AttributeI("zBaseValue").doValue;
                        if (iJEType == 10)
                        {
                            if (rTransactionValue > 0)
                            {
                                rTransactionValue = (rTransactionValue * -1);
                            }
                        }
                        else if (iJEType == 20)
                        {
                            if (rTransactionValue < 0)
                            {
                                rTransactionValue = (rTransactionValue * -1);
                            }
                        }
                        break;
                    case "prebankrec":
                        rTransactionValue = oEntity.AttributeI("zBaseValue").doValue;
                        if (iJEType == 20)
                        {
                            if (rTransactionValue > 0)
                            {
                                rTransactionValue = (rTransactionValue * -1);
                            }
                        }
                        else if (iJEType == 10)
                        {
                            if (rTransactionValue < 0)
                            {
                                rTransactionValue = (rTransactionValue * -1);
                            }
                        }
                        break;
                    case "bankrec":
                        rTransactionValue = oEntity.AttributeI("zBaseValue").doValue;
                        break;
                    case "reconcilebr":
                        rTransactionValue = oEntity.AttributeI("zBaseValue").doValue;
                        if (iJEType == 20)
                        {
                            if (rTransactionValue > 0)
                            {
                                rTransactionValue = (rTransactionValue * -1);
                            }
                        }
                        else if (iJEType == 10)
                        {
                            if (rTransactionValue < 0)
                            {
                                rTransactionValue = (rTransactionValue * -1);
                            }
                        }
                        break;
                    case "pfpayrec":
                        rTransactionValue = oEntity.AttributeI("zBaseValue").doValue;
                        if (iJEType == 10)
                        {
                            if (rTransactionValue > 0)
                            {
                                rTransactionValue = (rTransactionValue * -1);
                            }
                        }
                        else if (iJEType == 20)
                        {
                            if (rTransactionValue < 0)
                            {
                                rTransactionValue = (rTransactionValue * -1);
                            }
                        }
                        break;
                    case "ffpayrec":
                        rTransactionValue = oEntity.AttributeI("zBaseValue").doValue;
                        if (iJEType == 10)
                        {
                            if (rTransactionValue > 0)
                            {
                                rTransactionValue = (rTransactionValue * -1);
                            }
                        }
                        else if (iJEType == 20)
                        {
                            if (rTransactionValue < 0)
                            {
                                rTransactionValue = (rTransactionValue * -1);
                            }
                        }
                        break;
                }
                rBatchTotal = (rBatchTotal + rTransactionValue);

                //XIIBO oAllocBOI = new XIIBO();
                //XIDXI oAllocXID = new XIDXI();
                //XIDBO oAllocBOD = (XIDBO)oAllocXID.Get_BODefinition("ACAllocation_T").oResult;
                //oAllocBOI.BOD = oAllocBOD;
                //oAllocBOI.SetAttribute("FKiACTransactionFromID", oEntity.AttributeI("id").sValue);
                //// DS TO DO - 08.05.09 - this casuses problems as on a post=persist of the allocation it tries to update the allocated amounts of the from/to transactions
                ////   however the batch (FKiACTransactionToID) transaction has not yet been inserted and cannot be, so cannot be allocated against. What I
                ////   want is for on a post insert of a transaction it checks to see if there is any from/to allocations to self and update self to reflect this
                //oAllocBOI.SetAttribute("FKiACTransactionToID", oBatchTransaction.AttributeI("id").sValue);
                //oAllocBOI.SetAttribute("rValue", rTransactionValue.ToString());
                //oAllocBOI.SetAttribute("FKiEnterpriseID", oEntity.AttributeI("FKiEnterpriseID").ToString());
                //oAllocBOI.SetAttribute("refAccountCategory", oEntity.AttributeI("refAccountCategory").ToString());
                //// post the transactions for a new policy
                //var Inserttransation = oAllocBOI.Save(oAllocBOI);

                //if (Inserttransation.xiStatus == xiEnumSystem.xiFuncResult.xiError)
                //{
                //    oCResult.sMessage = ROUTINE_NAME + " Unable to insert instance of AC/ACAllocation";
                //    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                //    oCResult.LogToFile();
                //    oCResult.sMessage = ROUTINE_NAME + " Unable to insert instance of AC/ACAllocation";
                //    oXID.SaveErrortoDB(oCResult);
                //}
                sAddUpdateGroup = "";
                switch (sActionName)
                {
                    case "reconcilepf":
                        sFeedback = "This has been paid to the Finance Company on " + DateTime.Now;
                        oEntity.SetAttribute("iPFRec", 10.ToString());
                        // used in commission drawdowns - once we have received the premium finance we can draw down the comm
                        sAddUpdateGroup = ",iPFRec";
                        break;
                    case "commissiondrawdown":
                        sFeedback = "This commission has been Drawn down on " + DateTime.Now;
                        break;
                    case "insurerpay":
                        sFeedback = "This has been paid to the Insurer on " + DateTime.Now + " - Amount: \u00A3" + rAmountReconciled;
                        // SPELLED WRONG - but can't change it back now
                        oEntity.SetAttribute("iRiskTrnasfer", 10.ToString());
                        // used in commission drawdowns - once an insurer has been paid their premium we can draw down the comm
                        sAddUpdateGroup = ",iRiskTrnasfer";
                        break;
                    case "chargesdrawdown":
                        sFeedback = "This charge has been Drawn down on " + DateTime.Now;
                        break;
                    case "prebankrec":
                        sFeedback = "This transaction was pre-banked on " + DateTime.Now;
                        break;
                    case "bankrec":
                        sFeedback = "This has been reconciled with the Bank on " + DateTime.Now;
                        break;
                    case "supplierpayupm":
                        sFeedback = "This has been paid to the Supplier (UPM) on " + DateTime.Now;
                        break;
                    case "reconcilebr":
                        sFeedback = "This has been paid by the Broker on " + DateTime.Now;
                        //          oEntity.SetAttr "iPFRec", i.zX.lngValue(10)
                        //          sAddUpdateGroup = ",iPFRec"
                        break;
                    case "pfpayrec":
                        sFeedback = "This PF Payment Charge has been Drawn down on " + DateTime.Now;
                        break;
                    case "ffpayrec":
                        sFeedback = "This 50/50 Payment Charge has been Drawn down on " + DateTime.Now;
                        break;
                }
                oEntity.SetAttribute("sFeedback", oEntity.AttributeI("sFeedback").sValue + "<b><font size=\'2\' color=\'green\'>" + sFeedback + "</font></b><br>");
                oEntity.SetAttribute("sReconciliationHistory", oEntity.AttributeI("sReconciliationHistory").sValue + sFeedback + "<br>");
                oEntity.SetAttribute("iProcessStatus", 20.ToString());
                if (sDateOverride == "")
                {
                    oEntity.SetAttribute("dProcessDate", DateTime.Now.ToString());
                }
                else if (DateTime.TryParse(sDateOverride, out TempDate) == true)
                {
                    oEntity.SetAttribute("dProcessDate", DateTime.Parse(sDateOverride).ToString());
                }
                //sTransUpdate = "iProcessStatus,sFeedback,dProcessDate,sReconciliationHistory" + sAddUpdateGroup;
                var UpdateTransation = oEntity.Save(oEntity);//i.zX.BOS.updateBO(oEntity, sTransUpdate);
                if (!UpdateTransation.bOK)
                {
                    oCResult.sMessage = ROUTINE_NAME + " Unable to update Transaction" + oEntity.AttributeI("id").iValue + " to Process Status 20";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oCResult.sMessage = ROUTINE_NAME + "- Unable to update Transaction " + oEntity.AttributeI("id").iValue + " to Process Status 20" + "\r\n";// + i.zX.trace.formatStack;
                    oXID.SaveErrortoDB(oCResult);
                }
            }
            else if (iIncrementType == 20)
            {
                // finalise
                sStage = "Finalise";
                if (rBatchTotal < 0)
                {
                    oTransType = oXIIRec.BOI("ACTransType_T", "FKiACTransTypeID");
                    if (!(oTransType == null))
                    {
                        sOpposingCode = oTransType.AttributeI("sOpposingTransCode").sValue;
                        if (sOpposingCode != "")
                        {
                            oTransTypeOp = oXIIRec.BOI("ACTransType_T", null, null, null);
                            oTransTypeOp.SetAttribute("sCode", sOpposingCode);
                            oTransTypeOp.SetAttribute(XIConstant.Key_XIDeleted, 0.ToString());
                            oTransTypeDetails = oJournalTransactions.GetACCTransType(oTransType.AttributeI("sCode").sValue, oTransType.AttributeI(XIConstant.Key_XIDeleted).iValue, "");
                            if (!oTransTypeDetails.bOK && oTransTypeDetails.oResult == null)
                            {
                                oCResult.sMessage = ROUTINE_NAME + ", Cannot load OPPOSING Transaction Type for Policy Calculation: " + oEntity.AttributeI("id").iValue + " and Product: ";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oCResult.LogToFile();
                                oCResult.sMessage = ROUTINE_NAME + ", Cannot load OPPOSING Transaction Type for Policy Calculation: " + oEntity.AttributeI("id").iValue + " and Product: ";
                                oXID.SaveErrortoDB(oCResult); //Policy Calculation: "+i.GetAttr("id").strValue + " and Product: " + i.GetAttr("FKiProductID").strValue;
                            }
                            else
                            {
                                // trans type loaded
                                oBatchTransaction.SetAttribute("FKiACTransTypeID", oTransTypeOp.AttributeI("id").sValue);
                                rBatchTotal = (rBatchTotal * -1);
                            }
                        }
                        // sOpposingCode <> ""
                        oBatchTransaction.SetAttribute("iSystem", oTransType.AttributeI("iAccountsSet").sValue);
                    }
                    // oTransType is not nothing
                }
                switch (sActionName)
                {
                    case "supplierpayupm":
                        oBatchTransaction.SetAttribute("FKiCRAccountID", oRec.AttributeI("FKiAccountOverrideID").sValue);
                        break;
                }
                // 2017.07.05 request from ryan - the amount to insurer (and therefore the overall transaction amount and also the amount on the other side)
                //   should be decremented by rTotalAdjustments (see the calling function in CReconciliation for this function)
                oBatchTransaction.SetAttribute("zBaseValue", rBatchTotal.ToString());
                oBatchTransaction.SetAttribute("iTaxType", 10.ToString());
                // no tax implications
                if (sDateOverride != "")
                {
                    if (DateTime.TryParse(sDateOverride, out TempDate) == true)
                    {
                        oBatchTransaction.SetAttribute("dWhen", DateTime.Parse(sDateOverride).ToString());
                    }
                }
                // post the transaction for the reconcilliation
                oBatchTransaction.SetAttribute("FKiClientID", oEntity.AttributeI("FKiClientID").sValue);
                //oBatchTransaction.SetAttribute("FKiACPolicyID", oEntity.AttributeI("FKiACPolicyID").sValue);
                oBatchTransaction.SetAttribute("FKiProductID", oEntity.AttributeI("FKiProductID").sValue);
                oBatchTransaction.SetAttribute("refAccountCategory", oEntity.AttributeI("refAccountCategory").sValue);
                oBatchTransaction.SetAttribute("FKiQuoteID", oEntity.AttributeI("FKiQuoteID").sValue);
                oBatchTransaction.SetAttribute("rPremium", "0");
                oBatchTransaction.SetAttribute("FKsWhoID", FKsUserID);
                if (rBatchTotal != 0)
                {
                    // don't post zero bal transactions
                    //JournalTransactions oJournalTransaction = new JournalTransactions(TXinitial);
                    //var InsertBatchtransation = oBatchTransaction.Save(oBatchTransaction);
                    var InsertBatchtransation = oJTrans.PostTransaction(oBatchTransaction, sDataBase, sSessionID, sGUID, false);
                    if (InsertBatchtransation.xiStatus == xiEnumSystem.xiFuncResult.xiError)
                    {
                        oCResult.sMessage = ROUTINE_NAME + "Unable to insert instance of ACTransaction_T";
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oCResult.sMessage = ROUTINE_NAME + "- Unable to insert instance of ACTransaction_T";
                        oXID.SaveErrortoDB(oCResult);
                    }
                    else
                    {
                        oEntity.SetAttribute("id", oBatchTransaction.AttributeI("id").ToString());
                    }
                    oCResult.oResult = InsertBatchtransation.oResult;
                }
            }
            return oCResult;
        }
        #endregion

        public CResult SaveReconciliation(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            try
            {
                TXinitial.sTXBeginAt = "SAVERECONC";
                oCResult.sClassName = oCResult.Get_Class();
                oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
                XIIXI oXI = new XIIXI();
                string sSessionID = oParams.Where(t => t.sName == "sSessionID").Select(f => f.sValue).FirstOrDefault();
                string sCurrentGUID = oParams.Where(t => t.sName == "sGUID").Select(f => f.sValue).FirstOrDefault();
                //XICacheInstance sCurParams = oCache.GetAllParamsUnderGUID(sSessionID, sCurrentGUID, null);
                var QSInstance = oParams.Where(t => t.sName == "iQSIID").Select(f => f.sValue).FirstOrDefault();
                int iQSIID = 0;
                Guid QSGUID = Guid.Empty;
                int.TryParse(QSInstance, out iQSIID);
                Guid.TryParse(QSInstance, out QSGUID);
                XIIQS oQSInstance = new XIIQS();
                if(iQSIID>0)
                oQSInstance = (XIIQS)oCache.Get_QuestionSetCache("QuestionSetCache", sCurrentGUID, iQSIID.ToString());
                else
                    oQSInstance = (XIIQS)oCache.Get_QuestionSetCache("QuestionSetCache", sCurrentGUID, QSGUID.ToString());
                XIIBO oBOI = new XIIBO();
                XIDBO oBOD = new XIDBO();
                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "ACReconcilliation_T");
                oBOI.BOD = oBOD;
                oBOI.Attributes["sName".ToLower()] = new XIIAttribute { sName = "sName".ToLower(), sValue = oQSInstance.GetXIIValue("ReconciliationName").sValue, bDirty = true };
                oBOI.Attributes["FKiEnterpriseID".ToLower()] = new XIIAttribute { sName = "FKiEnterpriseID".ToLower(), sValue = oQSInstance.GetXIIValue("Enterprise").sValue, bDirty = true };
                oBOI.Attributes["refAccountCategory".ToLower()] = new XIIAttribute { sName = "refAccountCategory".ToLower(), sValue = oQSInstance.GetXIIValue("AccountCategory").sValue, bDirty = true };
                oBOI.Attributes["iType".ToLower()] = new XIIAttribute { sName = "iType".ToLower(), sValue = oCache.Get_ParamVal(sSessionID, sCurrentGUID, null, "{XIP|EnumReconType}"), bDirty = true };
                oBOI.Attributes["FKsUserID".ToLower()] = new XIIAttribute { sName = "FKsUserID".ToLower(), sValue = JournalTransactions.GetUser(oParams), bDirty = true };
                oBOI.Attributes["iStatus".ToLower()] = new XIIAttribute { sName = "iStatus".ToLower(), sValue = "0", bDirty = true };
                var oTX = oBOI.Save(oBOI);
                string iPKID = oBOI.Attributes.Values.Where(m => m.sName.ToLower() == oBOI.BOD.sPrimaryKey.ToLower()).Select(m => m.sValue).FirstOrDefault();
                oCache.Set_ParamVal(sSessionID, sCurrentGUID, null, "{XIP|ACReconcilliation_T.id}", iPKID, null, new List<CNV>());
                if (oQSInstance.GetXIIValue("AccountCategory").sValue == "1")
                {
                    QueryEngine oQE = new QueryEngine();
                    var oQResult = oQE.Execute_QueryEngine("refAccountCategory_T", "id", "");
                    if (oQResult.bOK && oQResult.oResult != null)
                    {
                        var AccountCategories = string.Join(",", ((Dictionary<string, XIIBO>)oQResult.oResult).Values.ToList().Skip(1).Select(x => x.AttributeI("id").sValue));
                        oCache.Set_ParamVal(sSessionID, sCurrentGUID, null, "{XIP|refAccountCategory}", AccountCategories, null, new List<CNV>());
                    }
                }
                else
                {
                    oCache.Set_ParamVal(sSessionID, sCurrentGUID, null, "{XIP|refAccountCategory}", oQSInstance.GetXIIValue("AccountCategory").sValue, null, new List<CNV>());
                }

                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oTX;
            }
            catch (Exception ex)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                oCResult.sMessage = "ERROR: " + ex.Message.ToString();
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oIB.SaveErrortoDB(oCResult);
            }
            if (TXinitial.sTXBeginAt == "SAVERECONC")
                TXinitial.TXCommitRollback(oCResult);
            return oCResult;
        }
        #region Reverse Reconcilliation 
        public CResult ReverseReconciliation(int ReconciliationID)
        {
            TXinitial.sTXBeginAt = "REVERSERECONC";
            const string ROUTINE_NAME = "[Reverse]";
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            XIInstanceBase oIB = new XIInstanceBase();
            XIIBO oACReconciliation = new XIIBO();
            string sHTMLResult = ""; string sManualError = "";
            try
            {
                XIIBO oBOI = new XIIBO(TXinitial);
                XIDXI oXID = new XIDXI();
                XIIXI oXII = new XIIXI();
                List<CNV> oParams = new List<CNV>();
                oParams.Add(new CNV { sName = "id", sValue = ReconciliationID.ToString() });
                oACReconciliation = oXII.BOI("ACReconcilliation_T", null, null, oParams);
                if (oACReconciliation.AttributeI("iStatus").iValue == 0)
                {
                    oCResult.iTraceLevel = 1;
                    return oCResult;
                }
                Dictionary<string, XIIBO> oXIBOI = new Dictionary<string, XIIBO>();
                QueryEngine oQE = new QueryEngine();
                string sWhereCondition = "FKiACReconcilliationID=" + ReconciliationID.ToString() + ","+ XIConstant.Key_XIDeleted + "=0,iType>=0";
                var oQResult = oQE.Execute_QueryEngine("ACXReconcilliation_T", "*", sWhereCondition);
                if (oQResult.bOK && oQResult.oResult != null)
                {
                    oXIBOI = (Dictionary<string, XIIBO>)oQResult.oResult;
                    oCResult.oResult = oQResult.oResult;
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                if (!oCResult.bOK && oCResult.oResult == null)
                {
                    // transaction loaded, which means this one right now is a duplicate
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + " _ " + ROUTINE_NAME + ", Unable to DB2Collection ]";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                };
                int iReconcileType = oACReconciliation.AttributeI("iType").iValue;
                bool bError = false;
                string sEnterpriseSeq = ","; string sDateOverride = "";
                string sAccCatSeq = ","; sError = new StringBuilder();
                string sEnterprise = "";
                foreach (var oXRecTransaction in oXIBOI)
                {
                    var oJE = oXII.BOI("ACJournalEntry_T", oXRecTransaction.Value.AttributeI("FKiACJEID").iValue.ToString());
                    if (oJE == null)
                    {
                        bError = true;
                        sError.Append("No matching Journal found for Reconcilliation id=" + oXRecTransaction.Value.AttributeI("id").sValue + "<BR>");
                    }
                    else
                    {
                        int iEnterprise = oXRecTransaction.Value.AttributeI("FKiEnterpriseID").iValue;
                        if (iEnterprise > 0)
                        {
                            if ((sEnterpriseSeq.IndexOf("," + (iEnterprise + ","), 0) + 1) == 0)
                            {
                                sEnterpriseSeq = sEnterpriseSeq + iEnterprise + ",";
                            }
                        }
                        else
                        {
                            bError = true;
                            sError.Append("INVALID ENTERPRISE, for Reconcilliation id=" + oXRecTransaction.Value.AttributeI("id").sValue + ", Journal id=" + oJE.AttributeI("id").sValue + "<BR>");
                        }
                        int iAccCat = oJE.AttributeI("refAccountCategory").iValue;

                        if (iAccCat > 0)
                        {
                            if ((sAccCatSeq.IndexOf(("," + iAccCat + ","), 0) + 1) == 0)
                            {
                                sAccCatSeq = sAccCatSeq + iAccCat + ",";
                            }
                        }
                        else
                        {
                            bError = true;
                            sError.Append("INVALID ACCOUNT CATEGORY, for Reconcilliation id=" + oXRecTransaction.Value.AttributeI("id").sValue + ", Journal id=" + oJE.AttributeI("id").sValue + "<BR>");
                        }
                    }
                }
                var AsEnterprises = sEnterpriseSeq.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var AsAccCats = sAccCatSeq.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                sDateOverride = oACReconciliation.AttributeI("dPostingDate").sValue == null ? "" : oACReconciliation.AttributeI("dPostingDate").sValue;
                for (int j = 0; j < AsEnterprises.Count(); j++)
                {
                    sEnterprise = AsEnterprises[j];
                    if (sEnterprise != "")
                    {
                        int iEnterpriseID = Convert.ToInt32(sEnterprise);
                        for (int k = 0; k < AsAccCats.Count(); k++)
                        {
                            string sThisAccCat = AsAccCats[k];
                            if (sThisAccCat != "")
                            {
                                int iAccCatID = Convert.ToInt32(sThisAccCat);
                                // so what this means is if we have 2 enterprises and 3 acc cats then a total of 6
                                //  cycles. However not each of these cells may need a transaction
                                double rTotalAdjustment = 0;
                                // Get all XTransactions that reference this id and whose checked it true
                                oQE = new QueryEngine();
                                sWhereCondition = "FKiACReconcilliationID=" + ReconciliationID.ToString() + ","+ XIConstant.Key_XIDeleted + "=0,iType>=0,FKiEnterpriseID=" + iEnterpriseID.ToString() + ",refAccountCategory=" + iAccCatID.ToString();
                                //sWhereCondition = $"FKiACReconcilliationID={ReconciliationID.ToString() },iType>0,FKiEnterpriseID={iEnterpriseID.ToString()},refAccountCategory={iAccCatID.ToString()}";
                                oQResult = oQE.Execute_QueryEngine("ACXReconcilliation_T", "*", sWhereCondition);
                                if (oQResult.bOK && oQResult.oResult != null)
                                {
                                    oXIBOI = (Dictionary<string, XIIBO>)oQResult.oResult;
                                    oCResult.oResult = oQResult.oResult;
                                }
                                if (oXIBOI == null)
                                {
                                    oCResult.sMessage = ROUTINE_NAME + ", Unable to DB2Collection";
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.LogToFile();
                                }
                                foreach (var oXRecTransaction in oXIBOI)
                                {
                                    // for each item user has checked
                                    var oJE = oXII.BOI("ACJournalEntry_T", oXRecTransaction.Value.AttributeI("FKiACJEID").iValue.ToString());
                                    // this is the original JE you are reconciling
                                    if (oJE != null)
                                    {
                                        // set it's rReconciled
                                        oJE.SetAttribute("iReconciled", 10.ToString());
                                        // 2011.10.19 PARTIAL TO DO - ALERT - WILL SET IT BACK TO NOT REC, BUT SHOULD REALLY CHECK IF IT SHOULD BE PARTIAL
                                        oJE.SetAttribute("sNotes", null);
                                        int rNewReconciled = 0;
                                        // ALERT - NO - should be a reversal of the reconciled amount
                                        oJE.SetAttribute("rReconciled", rNewReconciled.ToString());
                                        // ALERT - REVERSE ANY ADJUSTMENT WITH AN OPPOSITE ADJUSTMENT
                                        var response = oJE.Save(oJE);
                                        if (!response.bOK && response.oResult == null)
                                        {
                                            oCResult.sMessage = ROUTINE_NAME + ", Unable to update Journal id=" + oJE.AttributeI("id").sValue;
                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                            oCResult.LogToFile();
                                        }
                                    }
                                    // If not oJE Is Nothing
                                    var oTrans = oXII.BOI("ACTransaction_T", oJE.AttributeI("FKiTransactionID").sValue, "id,iProcessStatus,sFeedback");
                                    // 2014.05.29 - put iProcessStatus = 0 - ont he TRANSACTION - WARNING if the processed status was not zero before the rec then we have a problem
                                    if (oTrans != null)
                                    {
                                        oTrans.SetAttribute("iProcessStatus", "0");
                                        // can be dangerous if this is a reversal transaction. or does that matter?
                                        oTrans.SetAttribute("sFeedback", "");
                                        var response = oTrans.Save(oTrans);
                                        if (!response.bOK && response.oResult == null)
                                        {
                                            oCResult.sMessage = ROUTINE_NAME + ", Unable to update Transaction id=" + oTrans.AttributeI("id").sValue;
                                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                            oCResult.LogToFile();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                oACReconciliation.SetAttribute("dtPosted", DateTime.Now.ToString());
                oACReconciliation.SetAttribute("sActRec", oACReconciliation.AttributeI("sActRec").sValue);
                oACReconciliation.SetAttribute("rTotalPremium", "0");
                oACReconciliation.SetAttribute("rTotalIPT", "0");
                oACReconciliation.SetAttribute("rTotalCommission", "0");
                oACReconciliation.SetAttribute("rTotalAdjustments", "0");
                oACReconciliation.SetAttribute("rTotalNet", "0");    //exclude IPT so as to show what the supplier gets for themselves
                oACReconciliation.SetAttribute("rTotalPaid", "0"); //'usually includes IPT 
                oACReconciliation.SetAttribute("iStatus", "0");
                sHTMLResult = "<p><font face='Verdana' size='2'>Reconciliation Reversed</font></p>";
                oACReconciliation.SetAttribute("sHTMLResult", sHTMLResult);
                var Response = oACReconciliation.Save(oACReconciliation);
                if (!Response.bOK && Response.oResult == null)
                {
                    oCResult.sMessage = ROUTINE_NAME + ",  Unable to update Reconciliation id=" + oACReconciliation.AttributeI("id").sValue;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                }
            }
            catch (Exception ex)
            {
                sHTMLResult = "<p><font face='Verdana' size='4'>Error in Reconciliation: " + sManualError + " " + ex.Message + "</font></p>";
                oACReconciliation.SetAttribute("sHTMLResult", sHTMLResult);
                var Response = oACReconciliation.Save(oACReconciliation);
                if (!Response.bOK)
                {
                    oCResult.sMessage = ROUTINE_NAME + ",  Unable to update Reconciliation id=" + oACReconciliation.AttributeI("id").sValue;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oCResult.sMessage = ROUTINE_NAME + ",  Unable to update Reconciliation id=" + oACReconciliation.AttributeI("id").sValue;
                    oIB.SaveErrortoDB(oCResult);
                }
            }
            if (TXinitial.sTXBeginAt == "REVERSERECONC")
                TXinitial.TXCommitRollback(oCResult);
            return oCResult;
        }
        #endregion
        public CResult UpdateSupplier(List<CNV> oParams)
        {
            XIInfraCache oCache = new XIInfraCache();
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            try
            {
                int ReconciliationID = oParams.Where(t => t.sName == "ReconID").Select(f => Convert.ToInt32(f.sValue)).FirstOrDefault();
                string sSessionID = oParams.Where(t => t.sName == "sSessionID").Select(f => f.sValue).FirstOrDefault();
                string sGUID = oParams.Where(t => t.sName == "sGUID").Select(f => f.sValue).FirstOrDefault();
                string SupplierID = oCache.Get_ParamVal(sSessionID, sGUID, null, "iBOIID");
                XIIXI oIXI = new XIIXI();
                XIDXI oXID = new XIDXI();
                XIDBO oBOD = (XIDBO)oXID.Get_BODefinition("ACReconcilliation_T").oResult;
                XIIBO oBOI = oIXI.BOI("ACReconcilliation_T", ReconciliationID.ToString(), "*", null);
                oBOI.BOD = oBOD;
                oBOI.SetAttribute("FKiSupplierID", SupplierID);
                oCResult = oBOI.Save(oBOI);
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
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
            return oCResult;
        }
    }
}