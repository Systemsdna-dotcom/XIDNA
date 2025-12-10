using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using XICore;
using xiEnumSystem;
using XISystem;
using System.Data.SqlClient;
using static XIDatabase.XIDBAPI;

namespace ZeeInsurance
{
    public class JournalTransactions
    {
        public JournalTransactions() // Parameterless Constructor Default
        {
            TransactionInitiation oTXInitial = new TransactionInitiation();
            //this.TXSqlConn = oTXInitial.TXSqlconn;
            //this.TXSqlTrans = oTXInitial.TXSqltrans;
            TXInitiation = oTXInitial;
        }
        public JournalTransactions(TransactionInitiation OTrans) // With Parameter Constructor
        {
            //this.TXSqlConn = OTrans.TXSqlconn;
            //this.TXSqlTrans = OTrans.TXSqltrans;
            this.TXInitiation = OTrans;
        }
        public TransactionInitiation TXInitiation;
        CUserInfo oInfo = new CUserInfo();
        public int FKiProductID { get; set; }
        private int _ipostedfrom = 20;
        public int iPostedFrom
        {
            get { return _ipostedfrom; }
            set { _ipostedfrom = value; }
        }
        int iVersionNo = 0;
        int sProductOverrideID = 0;
        string sEDIProcessingClass = "";
        string sNameACTT = "";
        int iEDIMode = 0;
        int FKiEDITypeID = 0;
        int FKiEDIID = 0;
        int EDIID = 0;
        int iActionCode = 0;
        string sRebroke = "";
        string sTransCode = "";
        string sPolTransCode = "";
        int FKiEDITransTypeID = 0;
        int EDITransactionTypeID = 0;
        int iFrequency = 0;
        string sTransactionCode = "";
        decimal rPremium = 0;             // hard coded
        public decimal rAdmin { get; set; }
        public decimal zDefaultAdmin { get; set; }
        decimal rInsurerCharge = 10;      // hard coded
        int iDays = 365;                  // hard coded
        int iPaymentType = 0;             // hard coded
        string sCommissionOverride = "";  // hard coded
        int FKiAddOnID = 0;
        string sOpposingCode = "";
        string sNotes = "";
        int FKiBrokerID = 0;
        //int ACCTransTypeID = 0;
        int iType = 0;
        string sAddOnID = "";
        public decimal zCommission { get; set; }
        int iCommissionType = 0;
        int iAdminType = 0;
        int iDepositType = 0;
        int iTaxType = 0;
        int refTypeID = 0;
        int iTransTypeCode = 0;
        string sSeqFunction = string.Empty;
        string sSupIDConcat = string.Empty;
        int iNewPolStatus = 0;
        int FKiCPolicyCalcID = 0;
        int iStatus = 0;
        string sCode = "";
        public int FKiACPolicyID { get; set; }// = 0;
        public int FKiPFSchemeID { get; set; }
        int FKiACTransTypeID = 0;
        string refAnalysis1 = "";
        int FKiProductAddOnID = 0;
        int FKiPurchaseID = 0;
        string sChequeNo = "";
        string sReference = "";
        string sName = "";
        bool bIPT = false;
        decimal rTax = 0;
        DateTime dInception = DateTime.Now;
        DateTime dCurrentEffFrom = DateTime.Now;
        decimal zReVal = 0;
        XIIXI oPolicy = new XIIXI();
        int FKiQuoteID = 0;
        int FKiSupplierID = 0;
        int FKiCustomerID = 0;
        int FKiEnterpriseID = 2;  //Hard coded
        int refAccountCategory = 3; //Hard coded
        decimal zBaseValue = 0;
        decimal zDefaultDeposit = 0;
        decimal rDeposit = 0;
        decimal zCurrentVal = 0;
        decimal rComOverride = 0;
        decimal rCommission = 0;
        bool bReverse = false;
        decimal rNetToSupplier = 0;
        int iSystemType = 0;
        int XIDeleted = 0;
        decimal rCommAdj = 0;
        int AdminTransTypeID = 0;
        int CRECTransTypeID = 0;
        StringBuilder sAnalysis = new StringBuilder();
        decimal rJEAmount = 0;
        decimal rDRAmount = 0;
        decimal rCRAmount = 0;
        decimal rBalance = 0;
        string DRAccountID;
        string CRAccountID;
        string dWhen = "";
        int iTransType = 0;
        bool bCommission = false;
        string tCoverStart = "";
        string dCoverStart = "";
        public int iProcessStatus = 0;
        CResult oACCTransTypeClick = new CResult();
        XIInfraCache oCache = new XIInfraCache();
        XIBOInstance oData = new XIBOInstance();
        string FKsUserID = null;
        string FKiUserID = null;
        string FKsPolicyNo = null;
        string FKsWhoID = null;
        int iActivePlicyVersion = 0;
        #region TRANSACTION_METHODS
        public CResult JournalEntriesProcessingForNonPolicy(Dictionary<string, string> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult(); // always
            XIInstanceBase oIB = new XIInstanceBase();
            try
            {
                long iTraceLevel = 10;
                oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
                oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
                if (iTraceLevel > 0)
                {
                    oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
                }
                if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
                {
                    oCResult.xiStatus = oCR.xiStatus;
                }
                var oBrokerClick = new CResult();
                var oACCAccount = new XIIBO();
                rJEAmount = 0;
                rDRAmount = 0;
                rCRAmount = 0;
                int iCoreTransType = 0;
                int FKiAccountNoID = 0;
                int iOverrideType = 0;
                int FKiACAccountID = 0;
                int oACC = 0;
                bool bFlipTrans = false;
                int iJEType = 0;
                int TransTypeiStatus = 0;
                int refControlSectionID = 0;
                int FKiTransactionID = 0;
                int iRecType = 0;
                int iAmount = 0;
                int FKiAccountID = 0;
                int OverRideTypeID = 10;
                int ACCAccountID = 0;
                bool bAllocatedTo = false;
                int FKiACPolicyAllocID = 0;
                zCurrentVal = Convert.ToDecimal(oParams["zBaseValue"]);
                if (bAllocatedTo == true)
                {
                    FKiACPolicyAllocID = Convert.ToInt32(oParams["FKiACPolicyID"]);
                }
                if (sName == "")
                {
                    sName = oParams["sTransCode"];
                }
                if (zBaseValue < 0)
                {
                    bFlipTrans = true;
                    sAnalysis.Append("WARNING! Transaction Amount is Negative so the DR/CR has been FLIPPED ----" + "<br>");
                }
                XIIXI oIXI = new XIIXI();
                var JournalTemplateList = oData.oSubStructureI("Journal Template").oBOIList;
                //oJournalTemplateList = GetTempleteDetails(Convert.ToInt32(oParams["FKiACTransTypeID"]));
                //var JournalTemplateList = (Dictionary<string, XIIBO>)oJournalTemplateList.oResult;
                if (JournalTemplateList != null)
                {
                    int LoopCount = 0;
                    foreach (var oJournalTemplate in JournalTemplateList)
                    {
                        LoopCount++;
                        sAnalysis.Append("Journal Trans " + LoopCount + "--------------" + "<br>");
                        sAnalysis.Append("Journal template " + oJournalTemplate.AttributeI("sName").sValue + " - "
                                    + oJournalTemplate.AttributeI("id").iValue + "<br>");
                        iRecType = oJournalTemplate.AttributeI("iReconcilliationType").iValue;
                        iJEType = oJournalTemplate.AttributeI("iType").iValue;
                        iAmount = oJournalTemplate.AttributeI("iAmount").iValue;
                        FKiAccountID = oJournalTemplate.AttributeI("FKiAccountID").iValue;

                        switch (iAmount)
                        {
                            case 0:
                                //insurer prem
                                rJEAmount = rNetToSupplier;
                                sAnalysis.Append("Insurer Premium" + rJEAmount + "<br>");
                                break;

                            case 10:
                                //comm
                                rJEAmount = rCommission;
                                sAnalysis.Append("Commission" + rJEAmount + "<br>");
                                break;

                            case 20:
                                //admin
                                rJEAmount = Convert.ToDecimal(oParams["zBaseValue"]);    //rAdmin
                                sAnalysis.Append("Administration = " + rJEAmount + "<br>");
                                //iRecType = 40
                                break;
                            case 100:
                                //nominal
                                rJEAmount = Convert.ToDecimal(oParams["zBaseValue"]);    //be careful, maybe not for all postings - check
                                sAnalysis.Append("Nominal = " + rJEAmount + "<br>");

                                break;
                        }

                        if (bFlipTrans)
                        {
                            if (iJEType == 10)
                            {
                                iJEType = 20;
                            }
                            else if (iJEType == 20)
                            {
                                iJEType = 10;
                            }
                        }
                        if (iJEType == 10)
                        {
                            rDRAmount = rDRAmount + rJEAmount;
                            sAnalysis.Append("-- DEBIT journal<br>");
                        }
                        else if (iJEType == 20)
                        {
                            rCRAmount = rCRAmount + rJEAmount;
                            sAnalysis.Append("-- CREDIT journal<br>");
                        }
                        else
                        {
                            oCResult.sMessage = "ERROR:  CRITICAL ALERT Account Type DR / CR not set" + "\r\n PolicyID:" + oParams["FKiACPolicyID"] + ", sAnalysis: " + sAnalysis;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.LogToFile();
                            oCResult.sMessage = "ERROR:  CRITICAL ALERT Account Type DR / CR not set" + "\r\n PolicyID:" + oParams["FKiACPolicyID"] + ", sAnalysis: " + sAnalysis;
                            oIB.SaveErrortoDB(oCResult);
                        }
                        //nParams.Add(new CNV { sName = "{XIP|ACrefAccountNo_T.FKiACrefBaseAccountID}", sValue = FKiAccountID.ToString() });
                        //nParams.Add(new CNV { sName = "{XIP|ACrefAccountNo_T.FKiEnterpriseID}", sValue = FKiEnterpriseID.ToString() });
                        //XIIBO oACCNo = oIXI.BOI("ACrefAccountNo_T", "", "Matrix", nParams);  //Loading Reference Account Details
                        var oACCNo = oJournalTemplate.SubChildI.Values.FirstOrDefault();//.GetChildI("ACrefAccountNo_T").FirstOrDefault();
                        var Account = oACCNo.FirstOrDefault().SubChildI.Values.FirstOrDefault();

                        if (Account.FirstOrDefault() != null)
                        {
                            XIIAttribute FKiAccountNoIDv = new XIIAttribute();
                            if (Account.FirstOrDefault().Attributes.TryGetValue("FKiAccountNoID", out FKiAccountNoIDv))
                            {
                                FKiAccountNoID = Account.FirstOrDefault().AttributeI("FKiAccountNoID").iValue;
                            }
                        }

                        //iCoreTransType

                        XIIBO oACCAccountC = new XIIBO();
                        List<CNV> nParams = new List<CNV>();
                        nParams = new List<CNV>();
                        if (FKiAccountNoID != 0)
                        {
                            oACCAccount = oIXI.BOI("ACAccount_T", FKiAccountNoID.ToString());
                            if (oACCAccount != null)
                            {
                                ACCAccountID = oACCAccount.AttributeI("id").iValue;
                                iOverrideType = oACCAccount.AttributeI("iOverrideType").iValue;

                                if (iOverrideType != 0)
                                {
                                    OverRideTypeID = iOverrideType;
                                    switch (OverRideTypeID)
                                    {
                                        case 10:   // Client Account

                                            if (FKiCustomerID != 0)
                                            {
                                                nParams.Add(new CNV { sName = "FKiClientID", sValue = FKiCustomerID.ToString() });
                                                nParams.Add(new CNV { sName = "iType", sValue = 0.ToString() });
                                                //nParams.AddRange(nParams.Where(m => m.sType == "Structure").ToList());
                                                oACCAccountC = oIXI.BOI("ACAccount_T", "", "Main", nParams);  //Loading Account Details
                                                if (oACCAccountC != null)
                                                {
                                                    FKiAccountNoID = oACCAccountC.AttributeI("id").iValue;
                                                }
                                                else
                                                {

                                                    sAnalysis.Append("--  Client account not created  from Policy " + FKiAccountNoID);
                                                    //XIIBO oboiACC = new XIIBO();
                                                    //XIDXI oxidACC = new XIDXI();
                                                    //XIDBO obodACC = (XIDBO)oxidACC.Get_BODefinition("ACAccount_T").oResult;
                                                    //oboiACC.BOD = obodACC;
                                                    //oboiACC.SetAttribute("FKiClientID", FKiCustomerID.ToString());
                                                    //var CustomerDetails = oIXI.BOI("Customer_T", FKiCustomerID.ToString());
                                                    //oboiACC.SetAttribute("sName", CustomerDetails.AttributeI("sName").sValue);
                                                    //oboiACC.SetAttribute("iStatus", Convert.ToInt32(EnumTransactions.Live).ToString());
                                                    //oboiACC.SetAttribute("iType", 0.ToString());
                                                    //oboiACC.SetAttribute("iOverrideType", 10.ToString());
                                                    //var AccountCreate = oboiACC.Save(oboiACC);
                                                    //var IBOAccountCreate = (XIIBO)AccountCreate.oResult;
                                                    //if (!AccountCreate.bOK && AccountCreate.oResult == null)
                                                    //{
                                                    //    oCResult.sMessage = "ERROR: Transaction Insert Failed \r\n PolicyID:" + oParams["FKiACPolicyID"] + ", sAnalysis: " + sAnalysis;
                                                    //    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                    //    oCResult.LogToFile();
                                                    //    oCResult.sMessage = "ERROR: Transaction Insert Failed \r\n PolicyID:" + oParams["FKiACPolicyID"] + ", sAnalysis: " + sAnalysis;
                                                    //    oIB.SaveErrortoDB(oCResult);
                                                    //}
                                                    //else
                                                    //{
                                                    //    FKiAccountNoID = IBOAccountCreate.AttributeI("id").iValue;
                                                    //    CustomerDetails.SetAttribute("FKiACAccountID", FKiAccountNoID.ToString());
                                                    //    var UpdateAccountID = CustomerDetails.Save(CustomerDetails);
                                                    //}
                                                }
                                                sAnalysis.Append("-- derive Client account from Policy " + FKiAccountNoID);
                                                //+ (oAcc.iBO_getAttr("id").formattedValue + "<br>")));
                                            }

                                            break;

                                        case 20:   //'supplier account

                                            //'get the supplier from the policy
                                            if (FKiSupplierID != 0)
                                            {
                                                nParams.Add(new CNV { sName = "id", sValue = FKiSupplierID.ToString() });
                                                nParams.Add(new CNV { sName = "iType", sValue = 0.ToString() });
                                                oACCAccountC = oIXI.BOI("Supplier_T", "", "FKiACAccountID", nParams);  //Loading Account Details
                                                if (oACCAccountC != null)
                                                {
                                                    FKiAccountNoID = oACCAccountC.AttributeI("FKiACAccountID").iValue;
                                                    sAnalysis.Append("-- derive Insurer account from Context (Policy or Supplier): " + FKiAccountNoID);
                                                    //+ oAcc.iBO_getAttr("id").formattedValue + "<br>";
                                                }
                                            }

                                            break;
                                        case 30: // PF account
                                            if (FKiPFSchemeID != 0)
                                            {
                                                // ok the journal is referencing the PF scheme, but this has not been assigned to the trans so try and load it
                                                nParams.Add(new CNV { sName = "id", sValue = FKiPFSchemeID.ToString() });
                                                nParams.Add(new CNV { sName = "iType", sValue = 0.ToString() });
                                                oACCAccountC = oIXI.BOI("PFScheme_T", "", "FKiACAccountID", nParams);
                                                if (oACCAccountC != null)
                                                {
                                                    FKiAccountNoID = oACCAccountC.AttributeI("FKiACAccountID").iValue;
                                                    sAnalysis.Append("-- derive Premium Finance account from Scheme: " + FKiAccountNoID + "<br>");
                                                }
                                            }
                                            break;
                                            //case 35:     //Agent/Broker Account
                                            //    if (oBrokerClick.bOK && oBrokerClick.oResult != null)
                                            //    {
                                            //        var oACCAccountC = new CResult();
                                            //        // Code to implement if Broker Exists 
                                            //        oACCAccountC = Get_AccountDetails(FKiSupplierID, 10);
                                            //        var ACCAccountC = ((Dictionary<string, XIIBO>)oACCAccountC.oResult).Values.FirstOrDefault();
                                            //        if (ACCAccountC != null)
                                            //        {
                                            //            FKiAccountNoID = ACCAccountC.AttributeI("id").iValue;
                                            //            sAnalysis = sAnalysis + "-- derive Broker account from Broker ";
                                            //            //+ oBrokerClick.GetAttr("sName").strValue + " (\'"
                                            //            //+ oBroker.GetAttr("id").strValue + "\') - Account =\'"
                                            //            //+ oAcc.iBO_getAttr("id").formattedValue + "\'<br>";
                                            //        }
                                            //        else
                                            //        {

                                            //        }
                                            //    }
                                            //    else
                                            //    {
                                            //        FKiAccountNoID = 0;
                                            //    }

                                            //    break;
                                    }
                                    //if (oACC == 0)
                                    //{
                                    //    if (iCoreTransType >= 2000)
                                    //    {
                                    //        if ((iJEType == 10 && bFlipTrans == false) || (iJEType == 20 && bFlipTrans == true))
                                    //        {
                                    //            oACC = oACCAccount.AttributeI("FKiDRAccountID").iValue;
                                    //            sAnalysis = sAnalysis + "-- derive DR Account from Transaction: <br>";
                                    //            //+ oACC.("id").formattedValue + "<br>";
                                    //        }
                                    //        else if ((iJEType == 10 && bFlipTrans == true) || (iJEType == 20 && bFlipTrans == false))
                                    //        {
                                    //            oACC = oACCAccount.AttributeI("FKiCRAccountID").iValue;
                                    //            sAnalysis = sAnalysis + "-- derive CR Account from Transaction: <br>";
                                    //            //+ oACC.iBO_getAttr("id").formattedValue + "<br>";
                                    //        }
                                    //    }
                                    //}
                                    sAnalysis.Append("Account No. <br>" + FKiAccountNoID + " [id:" + FKiAccountNoID + "]<br>");
                                    //if (bFlipTrans)
                                    //{
                                    //    if (rJEAmount < 0)
                                    //        rJEAmount = rJEAmount * -1;
                                    //}
                                }
                            }
                        }
                        //code to insert bo
                        if (bFlipTrans)
                        {
                            if (rJEAmount < 0)
                                rJEAmount = rJEAmount * -1;
                        }
                        XIIBO oboi = new XIIBO();
                        XIDXI oxid = new XIDXI();
                        XIDBO obod = (XIDBO)oxid.Get_BODefinition("ACJournalEntry_T").oResult;
                        oboi.BOD = obod;
                        oboi.SetAttribute("FKiTransactionID", oParams["FKiTransactionID"]);
                        oboi.SetAttribute("FKiACPolicyID", oParams["FKiACPolicyID"]);
                        //iTransType= otramstype.getattribite("iType")
                        oboi.SetAttribute("iTransType", iTransType.ToString());
                        oboi.SetAttribute("FKiNominalAccID", FKiAccountID.ToString());

                        if (iJEType == 10)
                        {
                            oboi.SetAttribute("rDR", rJEAmount.ToString());
                            oboi.SetAttribute("rSignAmount", (rJEAmount * -1).ToString());
                        }
                        else if (iJEType == 20)
                        {
                            oboi.SetAttribute("rCR", rJEAmount.ToString());
                            oboi.SetAttribute("rSignAmount", rJEAmount.ToString());
                        }
                        oboi.SetAttribute("dtWhen", dWhen + " " + DateTime.Now.ToString(XIConstant.Time_Format));
                        oboi.SetAttribute("dDate", DateTime.Now.ToString(XIConstant.Date_Format) + " " + DateTime.Now.ToString(XIConstant.Time_Format));
                        oboi.SetAttribute("rAmount", rJEAmount.ToString());
                        oboi.SetAttribute("FKiTransTypeID", oParams["FKiACTransTypeID"]);
                        oboi.SetAttribute("iType", iJEType.ToString());
                        oboi.SetAttribute("iStatus", iStatus.ToString());
                        oboi.SetAttribute("iReconcilliationType", iRecType.ToString());
                        oboi.SetAttribute("FKiAccountID", FKiAccountNoID.ToString());
                        oboi.SetAttribute("FKiJETemplateID", oJournalTemplate.AttributeI("id").sValue);
                        oboi.SetAttribute("FKiEnterpriseID", FKiEnterpriseID.ToString());
                        oboi.SetAttribute("refAccountCategory", refAccountCategory.ToString());
                        oboi.SetAttribute("refControlSectionID", refControlSectionID.ToString());
                        oboi.SetAttribute("iReconciled", 10.ToString());
                        oboi.SetAttribute("FKsUserID", FKiUserID);
                        oboi.SetAttribute("sWho", FKsUserID);
                        oboi.SetAttribute("sNotes", sNotes);
                        oboi.iUserID = Convert.ToInt32(FKiUserID);
                        oCResult = oboi.Save(oboi);
                        if (!oCResult.bOK && oCResult.oResult == null)
                        {
                            oCResult.sMessage = "ERROR: " + LoopCount + " Journal Entry has not Performed \r\n PolicyID:" + oParams["FKiACPolicyID"] + ", sAnalysis: " + sAnalysis;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.LogToFile();
                            oCResult.sMessage = "ERROR: " + LoopCount + " Journal Entry has not Performed \r\n PolicyID:" + oParams["FKiACPolicyID"] + ", sAnalysis: " + sAnalysis;
                            oIB.SaveErrortoDB(oCResult);
                        }
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        Update_rBalance(FKiAccountNoID);
                    } //end for Transation tmplate items
                    //XIIBO oboiP = new XIIBO();
                    //XIDXI oxidP = new XIDXI();
                    //XIDBO obodP = (XIDBO)oxidP.Get_BODefinition("ACPolicy_T").oResult;
                    //oboiP.BOD = obodP;
                    //oboiP.SetAttribute("id", oParams["FKiACPolicyID"]);
                    //oboiP.SetAttribute("iStatus", 10.ToString());
                    //sAnalysis.Append("-- Policy Status updated to NEW<br>");
                    //var Policyresponse = oboiP.Save(oboiP);
                    //if (!Policyresponse.oCResult.bOK && Policyresponse.oCResult.oResult == null)
                    //{
                    //    oCResult.sMessage = "ERROR: " + LoopCount + " Policy Not Updated" + "\r\n";
                    //    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    //    oCResult.LogToFile();
                    //    oIB.SaveErrortoDB(oCResult);
                    //}
                }
                //}
                if (rDRAmount != rCRAmount)
                {
                    // BIZARRE! sometimes claims they are different as numbers even if not!
                    //          If Abs(rDRAmount - rCRAmount) = Abs(rTax) Then
                    //             'dont do anything, this is ok as the difference is the tax
                    //          Else
                    oCResult.sMessage = "ERROR: [CRITICAL ALERT Credit and Debit Amounts do not match (DR="
                                + rDRAmount + ", CR="
                                + rCRAmount + ") for Transaction Type: " + sNameACTT + "] \r\n PolicyID:" + oParams["FKiACPolicyID"] + ", sAnalysis: " + sAnalysis;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oCResult.sMessage = "ERROR: [CRITICAL ALERT Credit and Debit Amounts do not match (DR="
                               + rDRAmount + ", CR="
                               + rCRAmount + ") for Transaction Type: " + sNameACTT + "] \r\n PolicyID:" + oParams["FKiACPolicyID"] + ", sAnalysis: " + sAnalysis;
                    oIB.SaveErrortoDB(oCResult);
                    TXInitiation.TXCommitRollback(oCResult);
                    return oCResult;
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n PolicyID:" + oParams["FKiACPolicyID"] + ", sAnalysis: " + sAnalysis;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n PolicyID:" + oParams["FKiACPolicyID"] + ", sAnalysis: " + sAnalysis;
                oIB.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        public CResult JournalEntriesProcessing(Dictionary<string, string> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult(); // always
            XIInstanceBase oIB = new XIInstanceBase();
            try
            {
                long iTraceLevel = 10;
                oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
                oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
                if (iTraceLevel > 0)
                {
                    oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
                }
                if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
                {
                    oCResult.xiStatus = oCR.xiStatus;
                }
                var oBrokerClick = new CResult();
                var oACCAccount = new XIIBO();
                rJEAmount = 0;
                rDRAmount = 0;
                rCRAmount = 0;
                int iCoreTransType = 0;
                int FKiAccountNoID = 0;
                int iOverrideType = 0;
                int FKiACAccountID = 0;
                int oACC = 0;
                bool bFlipTrans = false;
                int iJEType = 0;
                int TransTypeiStatus = 0;
                int refControlSectionID = 0;
                int FKiTransactionID = 0;
                int iRecType = 0;
                int iAmount = 0;
                int FKiAccountID = 0;
                int OverRideTypeID = 10;
                int ACCAccountID = 0;
                bool bAllocatedTo = false;
                int FKiACPolicyAllocID = 0;
                zCurrentVal = Convert.ToDecimal(oParams["zBaseValue"]);
                if (bAllocatedTo == true)
                {
                    FKiACPolicyAllocID = Convert.ToInt32(oParams["FKiACPolicyID"]);
                }
                if (sName == "")
                {
                    sName = oParams["sTransCode"];
                }
                if (sCode == "ADDP")
                {
                    iTransType = 20;
                }
                else if (sCode == "RETP")
                {
                    iTransType = 60;
                }
                if (zBaseValue < 0)
                {
                    bFlipTrans = true;
                    sAnalysis.Append("WARNING! Transaction Amount is Negative so the DR/CR has been FLIPPED ----" + "<br>");
                }
                XIIXI oIXI = new XIIXI();
                //var oJournalTemplateList = new CResult();
                //oJournalTemplateList = GetTempleteDetails(Convert.ToInt32(oParams["FKiACTransTypeID"]));
                var JournalTemplateList = oData.oSubStructureI("Journal Template").oBOIList;
                if (JournalTemplateList != null)
                {
                    int LoopCount = 0;
                    foreach (var oJournalTemplate in JournalTemplateList)
                    {
                        LoopCount++;
                        sAnalysis.Append("Journal Trans " + LoopCount + "--------------" + "<br>");
                        sAnalysis.Append("Journal template " + oJournalTemplate.AttributeI("sName").sValue + " - "
                                    + oJournalTemplate.AttributeI("id").iValue + "<br>");
                        iRecType = oJournalTemplate.AttributeI("iReconcilliationType").iValue;
                        iJEType = oJournalTemplate.AttributeI("iType").iValue;
                        iAmount = oJournalTemplate.AttributeI("iAmount").iValue;
                        FKiAccountID = oJournalTemplate.AttributeI("FKiAccountID").iValue;

                        switch (iAmount)
                        {
                            case 0:
                                //insurer prem
                                rJEAmount = rNetToSupplier;
                                sAnalysis.Append("Insurer Premium" + rJEAmount + "<br>");
                                break;

                            case 10:
                                //comm
                                rJEAmount = rCommission;
                                sAnalysis.Append("Commission" + rJEAmount + "<br>");
                                break;

                            case 20:
                                //admin
                                rJEAmount = Convert.ToDecimal(oParams["zBaseValue"]);    //rAdmin
                                sAnalysis.Append("Administration = " + rJEAmount + "<br>");
                                //iRecType = 40
                                break;
                            case 100:
                                //nominal
                                rJEAmount = Convert.ToDecimal(oParams["zBaseValue"]);    //be careful, maybe not for all postings - check
                                sAnalysis.Append("Nominal = " + rJEAmount + "<br>");

                                break;
                        }

                        if (bFlipTrans)
                        {
                            if (iJEType == 10)
                            {
                                iJEType = 20;
                            }
                            else if (iJEType == 20)
                            {
                                iJEType = 10;
                            }
                        }
                        if (iJEType == 10)
                        {
                            rDRAmount = rDRAmount + rJEAmount;
                            sAnalysis.Append("-- DEBIT journal<br>");
                        }
                        else if (iJEType == 20)
                        {
                            rCRAmount = rCRAmount + rJEAmount;
                            sAnalysis.Append("-- CREDIT journal<br>");
                        }
                        else
                        {
                            oCResult.sMessage = "ERROR:  CRITICAL ALERT Account Type DR / CR not set \r\n PolicyID:" + oParams["FKiACPolicyID"] + ", sAnalysis: " + sAnalysis;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.LogToFile();
                            oCResult.sMessage = "ERROR:  CRITICAL ALERT Account Type DR / CR not set \r\n PolicyID:" + oParams["FKiACPolicyID"] + ", sAnalysis: " + sAnalysis;
                            oIB.SaveErrortoDB(oCResult);
                        }
                        //CResult ACCNo = GetRefAccountNo(FKiAccountID, FKiEnterpriseID);
                        //XIIBO oACCNo = (XIIBO)ACCNo.oResult;
                        //if (oACCNo != null)
                        //{
                        //    XIIAttribute FKiAccountNoIDv = new XIIAttribute();
                        //    if (oACCNo.Attributes.TryGetValue("FKiAccountNoID", out FKiAccountNoIDv))
                        //    {
                        //        FKiAccountNoID = oACCNo.AttributeI("FKiAccountNoID").iValue;
                        //    }
                        //}
                        var oACCNo = oJournalTemplate.SubChildI.Values.FirstOrDefault();//.GetChildI("ACrefAccountNo_T").FirstOrDefault();
                        var Account = oACCNo.FirstOrDefault().SubChildI.Values.FirstOrDefault();

                        if (Account.FirstOrDefault() != null)
                        {
                            XIIAttribute FKiAccountNoIDv = new XIIAttribute();
                            if (Account.FirstOrDefault().Attributes.TryGetValue("FKiAccountNoID", out FKiAccountNoIDv))
                            {
                                FKiAccountNoID = Account.FirstOrDefault().AttributeI("FKiAccountNoID").iValue;
                            }
                        }
                        //iCoreTransType
                        XIIBO oACCAccountC = new XIIBO();
                        List<CNV> nParams = new List<CNV>();
                        if (FKiAccountNoID != 0)
                        {
                            oACCAccount = oIXI.BOI("ACAccount_T", FKiAccountNoID.ToString());
                            if (oACCAccount != null)
                            {
                                ACCAccountID = oACCAccount.AttributeI("id").iValue;
                                iOverrideType = oACCAccount.AttributeI("iOverrideType").iValue;

                                if (iOverrideType != 0)
                                {
                                    OverRideTypeID = iOverrideType;
                                    switch (OverRideTypeID)
                                    {
                                        case 10:   // Client Account

                                            if (FKiCustomerID != 0)
                                            {
                                                nParams.Add(new CNV { sName = "FKiClientID", sValue = FKiCustomerID.ToString() });
                                                nParams.Add(new CNV { sName = "iType", sValue = 0.ToString() });
                                                //nParams.AddRange(nParams.Where(m => m.sType == "Structure").ToList());
                                                oACCAccountC = oIXI.BOI("ACAccount_T", "", "Main", nParams);  //Loading Account Details
                                                if (oACCAccountC != null)
                                                {
                                                    FKiAccountNoID = oACCAccountC.AttributeI("id").iValue;
                                                }
                                                else
                                                {
                                                    sAnalysis.Append("--  Client account not created  from Policy " + FKiAccountNoID);
                                                    //XIIBO oboiACC = new XIIBO();
                                                    //XIDXI oxidACC = new XIDXI();
                                                    //XIDBO obodACC = (XIDBO)oxidACC.Get_BODefinition("ACAccount_T").oResult;
                                                    //oboiACC.BOD = obodACC;
                                                    //oboiACC.SetAttribute("FKiClientID", FKiCustomerID.ToString());
                                                    //var CustomerDetails = oIXI.BOI("Customer_T", FKiCustomerID.ToString());
                                                    //oboiACC.SetAttribute("sName", CustomerDetails.AttributeI("sName").sValue);
                                                    //oboiACC.SetAttribute("iStatus", Convert.ToInt32(EnumTransactions.Live).ToString());
                                                    //oboiACC.SetAttribute("iType", 0.ToString());
                                                    //oboiACC.SetAttribute("iOverrideType", 10.ToString());
                                                    //var AccountCreate = oboiACC.Save(oboiACC);
                                                    //var IBOAccountCreate = (XIIBO)AccountCreate.oResult;
                                                    //if (!AccountCreate.bOK && AccountCreate.oResult == null)
                                                    //{
                                                    //    oCResult.sMessage = "ERROR: Transaction Insert Failed \r\n PolicyID:" + oParams["FKiACPolicyID"] + ", sAnalysis: " + sAnalysis;
                                                    //    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                                    //    oCResult.LogToFile();
                                                    //    oCResult.sMessage = "ERROR: Transaction Insert Failed \r\n PolicyID:" + oParams["FKiACPolicyID"] + ", sAnalysis: " + sAnalysis;
                                                    //    oIB.SaveErrortoDB(oCResult);
                                                    //}
                                                    //else
                                                    //{
                                                    //    FKiAccountNoID = IBOAccountCreate.AttributeI("id").iValue;
                                                    //    CustomerDetails.SetAttribute("FKiACAccountID", FKiAccountNoID.ToString());
                                                    //    var UpdateAccountID = CustomerDetails.Save(CustomerDetails);
                                                    //}
                                                }
                                                sAnalysis.Append("-- derive Client account from Policy " + FKiAccountNoID);
                                                //+ (oAcc.iBO_getAttr("id").formattedValue + "<br>")));
                                            }
                                            break;

                                        case 20:   //'supplier account

                                            //'get the supplier from the policy
                                            if (FKiSupplierID != 0)
                                            {
                                                nParams.Add(new CNV { sName = "id", sValue = FKiSupplierID.ToString() });
                                                nParams.Add(new CNV { sName = "iType", sValue = 0.ToString() });
                                                oACCAccountC = oIXI.BOI("Supplier_T", "", "FKiACAccountID", nParams);  //Loading Account Details
                                                if (oACCAccountC != null)
                                                {
                                                    FKiAccountNoID = oACCAccountC.AttributeI("FKiACAccountID").iValue;
                                                    sAnalysis.Append("-- derive Insurer account from Context (Policy or Supplier): " + FKiAccountNoID);
                                                    //+ oAcc.iBO_getAttr("id").formattedValue + "<br>";
                                                }
                                            }
                                            break;

                                        case 30: // PF account
                                            if (FKiPFSchemeID != 0)
                                            {
                                                // ok the journal is referencing the PF scheme, but this has not been assigned to the trans so try and load it
                                                nParams.Add(new CNV { sName = "id", sValue = FKiPFSchemeID.ToString() });
                                                nParams.Add(new CNV { sName = "iType", sValue = 0.ToString() });
                                                oACCAccountC = oIXI.BOI("PFScheme_T", "", "FKiACAccountID", nParams);
                                                if (oACCAccountC != null)
                                                {
                                                    FKiAccountNoID = oACCAccountC.AttributeI("FKiACAccountID").iValue;
                                                    sAnalysis.Append("-- derive Premium Finance account from Scheme: " + FKiAccountNoID + "<br>");
                                                }
                                            }
                                            break;
                                            //case 35:     //Agent/Broker Account
                                            //    if (oBrokerClick.bOK && oBrokerClick.oResult != null)
                                            //    {
                                            //        var oACCAccountC = new CResult();
                                            //        // Code to implement if Broker Exists 
                                            //        oACCAccountC = Get_AccountDetails(FKiSupplierID, 10);
                                            //        var ACCAccountC = ((Dictionary<string, XIIBO>)oACCAccountC.oResult).Values.FirstOrDefault();
                                            //        if (ACCAccountC != null)
                                            //        {
                                            //            FKiAccountNoID = ACCAccountC.AttributeI("id").iValue;
                                            //            sAnalysis = sAnalysis + "-- derive Broker account from Broker ";
                                            //            //+ oBrokerClick.GetAttr("sName").strValue + " (\'"
                                            //            //+ oBroker.GetAttr("id").strValue + "\') - Account =\'"
                                            //            //+ oAcc.iBO_getAttr("id").formattedValue + "\'<br>";
                                            //        }
                                            //        else
                                            //        {

                                            //        }
                                            //    }
                                            //    else
                                            //    {
                                            //        FKiAccountNoID = 0;
                                            //    }

                                            //    break;
                                    }
                                    //if (oACC == 0)
                                    //{
                                    //    if (iCoreTransType >= 2000)
                                    //    {
                                    //        if ((iJEType == 10 && bFlipTrans == false) || (iJEType == 20 && bFlipTrans == true))
                                    //        {
                                    //            oACC = oACCAccount.AttributeI("FKiDRAccountID").iValue;
                                    //            sAnalysis = sAnalysis + "-- derive DR Account from Transaction: <br>";
                                    //            //+ oACC.("id").formattedValue + "<br>";
                                    //        }
                                    //        else if ((iJEType == 10 && bFlipTrans == true) || (iJEType == 20 && bF lipTrans == false))
                                    //        {
                                    //            oACC = oACCAccount.AttributeI("FKiCRAccountID").iValue;
                                    //            sAnalysis = sAnalysis + "-- derive CR Account from Transaction: <br>";
                                    //            //+ oACC.iBO_getAttr("id").formattedValue + "<br>";
                                    //        }
                                    //    }
                                    //}
                                    sAnalysis.Append("Account No. <br>" + FKiAccountNoID + " [id:" + FKiAccountNoID + "]<br>");
                                    //if (bFlipTrans)
                                    //{
                                    //    if (rJEAmount < 0)
                                    //        rJEAmount = rJEAmount * -1;
                                    //}
                                }
                            }
                        }
                        //code to insert bo
                        if (bFlipTrans)
                        {
                            if (rJEAmount < 0)
                                rJEAmount = rJEAmount * -1;
                        }
                        XIIBO oboi = new XIIBO();
                        XIDXI oxid = new XIDXI();
                        XIDBO obod = (XIDBO)oxid.Get_BODefinition("ACJournalEntry_T").oResult;
                        oboi.BOD = obod;
                        oboi.SetAttribute("FKiTransactionID", oParams["FKiTransactionID"]);
                        oboi.SetAttribute("FKiACPolicyID", oParams["FKiACPolicyID"]);
                        oboi.SetAttribute("iTransType", iTransType.ToString());
                        oboi.SetAttribute("FKiNominalAccID", FKiAccountID.ToString());

                        if (iJEType == 10)
                        {
                            oboi.SetAttribute("rDR", rJEAmount.ToString());
                            oboi.SetAttribute("rSignAmount", (rJEAmount * -1).ToString());
                        }
                        else if (iJEType == 20)
                        {
                            oboi.SetAttribute("rCR", rJEAmount.ToString());
                            oboi.SetAttribute("rSignAmount", rJEAmount.ToString());
                        }
                        oboi.SetAttribute("dtWhen", dWhen + " " + DateTime.Now.ToString(XIConstant.Time_Format));
                        oboi.SetAttribute("dDate", DateTime.Now.ToString(XIConstant.Date_Format) + " " + DateTime.Now.ToString(XIConstant.Time_Format));
                        oboi.SetAttribute("rAmount", rJEAmount.ToString());
                        oboi.SetAttribute("FKiTransTypeID", oParams["FKiACTransTypeID"]);
                        oboi.SetAttribute("iType", iJEType.ToString());
                        oboi.SetAttribute("iStatus", iStatus.ToString());
                        oboi.SetAttribute("iReconcilliationType", iRecType.ToString());
                        oboi.SetAttribute("FKiAccountID", FKiAccountNoID.ToString());
                        oboi.SetAttribute("FKiJETemplateID", oJournalTemplate.AttributeI("id").sValue);
                        oboi.SetAttribute("FKiEnterpriseID", FKiEnterpriseID.ToString());
                        oboi.SetAttribute("refAccountCategory", refAccountCategory.ToString());
                        oboi.SetAttribute("refControlSectionID", refControlSectionID.ToString());
                        oboi.SetAttribute("iReconciled", 10.ToString());
                        oboi.SetAttribute("FKsUserID", FKiUserID);
                        oboi.SetAttribute("sWho", FKsUserID);
                        oboi.SetAttribute("sNotes", sNotes);
                        oboi.iUserID = Convert.ToInt32(FKiUserID);
                        oCResult = oboi.Save(oboi);

                        if (!oCResult.bOK && oCResult.oResult == null)
                        {
                            oCResult.sMessage = "ERROR: " + LoopCount + " Journal Entry has not Performed \r\n PolicyID:" + oParams["FKiACPolicyID"] + ", sAnalysis: " + sAnalysis;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.LogToFile();
                            oCResult.sMessage = "ERROR: " + LoopCount + " Journal Entry has not Performed \r\n PolicyID:" + oParams["FKiACPolicyID"] + ", sAnalysis: " + sAnalysis;
                            oIB.SaveErrortoDB(oCResult);
                        }
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        Update_rBalance(FKiAccountNoID);
                    } //end for Transation tmplate items
                    //XIIBO oboiP = new XIIBO();
                    //XIDXI oxidP = new XIDXI();
                    //XIDBO obodP = (XIDBO)oxidP.Get_BODefinition("ACPolicy_T").oResult;
                    //oboiP.BOD = obodP;
                    //oboiP.SetAttribute("id", oParams["FKiACPolicyID"]);
                    //oboiP.SetAttribute("iStatus", 10.ToString());
                    //sAnalysis.Append("-- Policy Status updated to NEW<br>");
                    //var Policyresponse = oboiP.Save(oboiP);
                    //if (!Policyresponse.oCResult.bOK && Policyresponse.oCResult.oResult == null)
                    //{
                    //    oCResult.sMessage = "ERROR: " + LoopCount + " Policy Not Updated" + "\r\n";
                    //    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    //    oCResult.LogToFile();
                    //    oIB.SaveErrortoDB(oCResult);
                    //}
                }
                //}
                if (rDRAmount != rCRAmount)
                {
                    // BIZARRE! sometimes claims they are different as numbers even if not!
                    //          If Abs(rDRAmount - rCRAmount) = Abs(rTax) Then
                    //             'dont do anything, this is ok as the difference is the tax
                    //          Else
                    oCResult.sMessage = "ERROR: [CRITICAL ALERT Credit and Debit Amounts do not match (DR=" + rDRAmount + ", CR="
                                + rCRAmount + ") for Transaction Type: " + sNameACTT + "] \r\n PolicyID:" + oParams["FKiACPolicyID"] + ", sAnalysis: " + sAnalysis;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oCResult.sMessage = "ERROR: [CRITICAL ALERT Credit and Debit Amounts do not match (DR="
                               + rDRAmount + ", CR="
                               + rCRAmount + ") for Transaction Type: " + sNameACTT + "] \r\n PolicyID:" + oParams["FKiACPolicyID"] + ", sAnalysis: " + sAnalysis;
                    oIB.SaveErrortoDB(oCResult);
                    TXInitiation.TXCommitRollback(oCResult);
                    return oCResult;
                }
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n PolicyID:" + oParams["FKiACPolicyID"] + ", sAnalysis:" + sAnalysis;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n PolicyID:" + oParams["FKiACPolicyID"] + ", sAnalysis:" + sAnalysis;
                oIB.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
        public CResult GetTempleteDetails(int FKiTransTypeID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            XIInstanceBase oIB = new XIInstanceBase();
            try
            {
                oCResult.sClassName = oCResult.Get_Class();
                oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
                XIInfraScript oScript = new XIInfraScript();
                XID1Click JT1Click1 = new XID1Click();
                XID1Click JT1Click2 = new XID1Click();
                //Get BO Definition
                XIDBO oBOD = new XIDBO();
                XIInfraCache oCache = new XIInfraCache();
                //Get 1-Click Defintion             
                JT1Click1 = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "Journal Template", null);
                JT1Click2 = (XID1Click)JT1Click1.Clone(JT1Click1);
                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, JT1Click1.BOID.ToString());
                XIDXI oXID = new XIDXI();
                var DataSource = oXID.GetBODataSource(oBOD.iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                List<CNV> nParms = new List<CNV>();
                CNV oNVParams = new CNV();
                oNVParams.sName = "{XIP|" + oBOD.Name + ".FKiTransTypeID}";
                oNVParams.sValue = FKiTransTypeID.ToString();
                nParms.Add(oNVParams);
                var oJournalTemplateClick = new Dictionary<string, XIIBO>();
                JT1Click2.ReplaceFKExpressions(nParms);
                JT1Click2.sConnectionString = DataSource;
                oJournalTemplateClick = JT1Click2.OneClick_Execute();
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oJournalTemplateClick;
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

        public CResult GetRefAccountNo(int FKiAccountID, int FKiEnterpriseID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            XIInstanceBase oIB = new XIInstanceBase();
            try
            {
                oCResult.sClassName = oCResult.Get_Class();
                oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
                XIIXI oIXI = new XIIXI();
                List<CNV> nParams = new List<CNV>();
                nParams.Add(new CNV { sName = "FKiACrefBaseAccountID", sValue = FKiAccountID.ToString() });
                nParams.Add(new CNV { sName = "FKiEnterpriseID", sValue = FKiEnterpriseID.ToString() });
                XIIBO oACCNo = oIXI.BOI("ACrefAccountNo_T", "", "Matrix", nParams); //Loading Reference Account Details
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = (object)oACCNo;
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
        public Decimal Get_TaxRateIPT()
        {
            Decimal Tax = 11;  // Tax hard coded 
            return Tax;
        }
        public decimal RealRound(decimal dValue)
        {
            string sAmount;
            decimal rRounded;
            bool bRoundUp = false;
            // TODO: On Error GoTo Warning!!!: The statement is not translatable 
            // always round up to nearest penny
            dValue = Math.Round(dValue, 3);
            sAmount = dValue.ToString();
            if (sAmount.Contains(".") && double.Parse(sAmount.Substring(sAmount.Length - 1)) > 4)
            {
                bRoundUp = true;
                sAmount = sAmount.Substring(0, (sAmount.Length - 1));
            }
            rRounded = decimal.Parse(sAmount);
            if (bRoundUp)
            {
                rRounded = (rRounded + Convert.ToDecimal(0.01));
            }

            return rRounded;
        }
        public void Calculation(string sTransCode)
        {
            decimal rPremiumT = zCurrentVal = rPremium = zBaseValue;
            decimal zTaxPC = 0;
            if (bIPT)
            {
                string sFunction = "xi.s|{xi.a|'XIConfig_T','Tax','sValue','','sName'}";
                CResult oCR = new CResult();
                XIDScript oXIScript = new XIDScript();
                oXIScript.sScript = sFunction.ToString();
                oCR = oXIScript.Execute_Script("", "");
                if (oCR.bOK && oCR.oResult != null)
                {
                    //XIInstanceBase oIB = new XIInstanceBase();
                    //CResult oCResult = new CResult();
                    //oCResult.sMessage = "TaxRate:" + ConfigurationManager.AppSettings["TaxRate"];
                    //oIB.SaveErrortoDB(oCResult);
                    //zTaxPC = Convert.ToDecimal(oCR.oResult);
                    zTaxPC = Convert.ToDecimal(ConfigurationManager.AppSettings["TaxRate"]); //Get_TaxRateIPT();  // hard coded value
                }

                if (iTaxType == 10)
                {
                    rTax = ((zCurrentVal / 100) * zTaxPC); //zTaxPC hard coded
                    if (rTax != 0)
                    {
                        rTax = RealRound(rTax);
                    }
                    //rPremiumT = (zCurrentVal - rTax);
                    sAnalysis.Append("Tax calculated at \u00A3" + rTax + "<br>");
                }
                else
                {
                    zReVal = zCurrentVal * 100 / (100 + zTaxPC);
                    if (zReVal != 0)
                    {
                        zReVal = RealRound(zReVal);
                    }
                    rTax = zCurrentVal - zReVal;
                    sAnalysis.Append("Tax calculated at \u00A3" + rTax + "<br>");
                    sAnalysis.Append("Premium (due to Tax) revaluation from \u00A3" + zCurrentVal + " to \u00A3" + zReVal + "<br>");
                    zCurrentVal = zReVal;
                    //rPremiumT = zCurrentVal;
                }
            }
            if (bCommission)
            {
                if (rComOverride != 0)
                {
                    rCommission = rComOverride;
                    if (iStatus == Convert.ToInt32(EnumTransactions.CorrectionTransaction) && bReverse == true && rCommission < 0)
                    {
                        rCommission = rCommission * -1; //we are reversing
                    }
                    else if (iStatus == Convert.ToInt32(EnumTransactions.CorrectionTransaction) && bReverse == true)
                    {
                        rCommission = rCommission * -1; //we are reversing
                    }
                }
                else
                {
                    if (iCommissionType == 0)
                    {
                        if (Math.Abs(zCommission) < 1)
                        {
                            zCommission = zCommission * 100;
                        }
                        rCommission = zCurrentVal / 100 * zCommission;
                    }
                    else
                    {
                        rCommission = zCommission;
                    }
                }
                if (rCommission != 0)
                {
                    rCommission = RealRound(rCommission);
                }
            }
            else
            {
                if (sTransCode == "DNEWP" || sTransCode == "DADDP" || sTransCode == "DRNWP" || sTransCode == "DRETP" || sTransCode == "DRBRP")
                {
                    rCommission = rPremiumT;
                }
            }
            if (iStatus == Convert.ToInt32(EnumTransactions.CorrectionTransaction) && bReverse == true)
            {
                if (zCurrentVal > rNetToSupplier && rCommission < 0)
                {
                    rNetToSupplier = zCurrentVal + rCommission;
                }
                else
                {
                    rNetToSupplier = zCurrentVal - rCommission;
                }
            }
            else
            {
                rNetToSupplier = zCurrentVal - rCommission;
            }
            if (bIPT)
            {
                rNetToSupplier = rNetToSupplier + rTax;
            }
            if ((iSystemType == 100 || iSystemType == 110) && bReverse == true)
            {
                rTax = rTax * -1;
                rCommission = rCommission * -1;
                rNetToSupplier = rNetToSupplier * -1;
            }
            else if (iSystemType == 200 || iSystemType == 210)
            {
                rAdmin = zCurrentVal;
            }
            //else if (sTransCode == "ADMIN" || sTransCode == "PFCH" || sTransCode == "FFCH")
            //{
            //    rAdmin = zCurrentVal;
            //}
            else if (sTransCode == "CADJ")
            {
                iPostedFrom = 0;
                rCommAdj = zBaseValue;
            }
            iPostedFrom = sTransCode == "PAYI" ? 0 : iPostedFrom;
            //  if (zDefaultDeposit == 0) {
            //      rDeposit = 0;
            //  }   
            //else
            //  {
            //      rDeposit = (rClientQuote * zDefaultDeposit) / 100;
            //  }
        }
        public CResult AddTransaction(int iNewPolStatus, int iTransTypeCode, string sTransactionCode, int iRefTypeID, string sSessionID, string sCurrentGUID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            XIInstanceBase oIB = new XIInstanceBase();
            XIInfraCache oCache = new XIInfraCache();
            try
            {
                oCResult.sClassName = oCResult.Get_Class();
                oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
                Calculation(sTransCode);
                sAnalysis.Append("Transaction type " + sTransCode + "<br>");
                var sTransTypeWhere = "";
                if (sTransactionCode == "")
                {
                    iType = iTransTypeCode;
                }
                else
                {
                    if (iTransTypeCode == 10 && rPremium < 0)
                    {
                        sTransactionCode = "RETP";
                        rPremium = rPremium * -1;
                    }
                    sCode = sTransactionCode;
                    sTransTypeWhere = "sCode= '" + sCode + "'";
                }
                //var oACTransTypeOneClick = ((Dictionary<string, XIIBO>)oACCTransTypeClick.oResult).Values.FirstOrDefault();
                //if (oACTransTypeOneClick != null)
                //{
                //    //trans type loaded
                //    XIIAttribute FKiACTransTypeIDv = new XIIAttribute();
                //    if (oACTransTypeOneClick.Attributes.TryGetValue("id", out FKiACTransTypeIDv))
                //    {
                //        FKiACTransTypeIDv.sValue = oACTransTypeOneClick.Attributes["id"].sValue;
                //        if (!string.IsNullOrEmpty(FKiACTransTypeIDv.sValue))
                //            if (int.TryParse(FKiACTransTypeIDv.sValue, out FKiACTransTypeID)) { }
                //    }
                //Transaction Insert Code
                XIIBO oboiACT = new XIIBO();
                XIDXI oxidACT = new XIDXI();
                XIDBO obodACT = (XIDBO)oxidACT.Get_BODefinition("ACTransaction_T").oResult;
                oboiACT.BOD = obodACT;
                oboiACT.SetAttribute("FKiACPolicyID", FKiACPolicyID.ToString()); //= new XIIAttribute { sName = "FKiACPolicyID", sValue = FKiACPolicyID.ToString(), bDirty = true };
                oboiACT.SetAttribute("FKiEDITransTypeID", FKiEDITransTypeID.ToString());
                oboiACT.SetAttribute("iAllocPriority", "10");
                oboiACT.SetAttribute("FKiBrokerID", FKiBrokerID.ToString());
                oboiACT.SetAttribute("sNotes", sNotes);
                oboiACT.SetAttribute("zCommission", zCommission.ToString());
                oboiACT.SetAttribute("iCommissionType", iCommissionType.ToString());
                oboiACT.SetAttribute("iTaxType", iTaxType.ToString());
                oboiACT.SetAttribute("zBaseValue", zBaseValue.ToString());
                oboiACT.SetAttribute("iDays", iDays.ToString());
                oboiACT.SetAttribute("sTransCode", sTransactionCode.ToString());
                oboiACT.SetAttribute("iSystemType", iSystemType.ToString());
                oboiACT.SetAttribute("refTypeID", refTypeID.ToString());
                oboiACT.SetAttribute("refAnalysis1", refAnalysis1);
                oboiACT.SetAttribute("sWho", FKsUserID);
                oboiACT.SetAttribute("FKsWhoID", FKsWhoID);
                oboiACT.SetAttribute("rTax", rTax.ToString());
                oboiACT.SetAttribute("sName", sNameACTT);
                oboiACT.SetAttribute("iStatus", iStatus.ToString());
                oboiACT.SetAttribute(XIConstant.Key_XIDeleted, XIDeleted.ToString());
                oboiACT.SetAttribute("FKiACTransTypeID", FKiACTransTypeID.ToString());
                oboiACT.SetAttribute("FKiQuoteID", FKiQuoteID.ToString());
                oboiACT.SetAttribute("FKiClientID", FKiCustomerID.ToString());
                oboiACT.SetAttribute("FKiSupplierID", FKiSupplierID.ToString());
                oboiACT.SetAttribute("FKiProductID", FKiProductID.ToString());
                oboiACT.SetAttribute("FKiBrokerID", FKiBrokerID.ToString());
                oboiACT.SetAttribute("FKiEnterpriseID", FKiEnterpriseID.ToString());
                oboiACT.SetAttribute("iAdminType", iAdminType.ToString());
                oboiACT.SetAttribute("sPolTransCode", sTransactionCode.ToString());
                oboiACT.SetAttribute("refAccountCategory", refAccountCategory.ToString());
                oboiACT.SetAttribute("FKiACPolicyAllocID", 0.ToString());
                oboiACT.SetAttribute("iType", iType.ToString());
                oboiACT.SetAttribute("FKiPFSchemeID", FKiPFSchemeID.ToString());
                //oboiACT.SetAttribute("XICreatedWhen", DateTime.Now.ToString());
                //oboiACT.SetAttribute("XIUpdatedWhen", DateTime.Now.ToString());
                oboiACT.SetAttribute("dWhen", dWhen + " " + DateTime.Now.ToString(XIConstant.Time_Format));
                oboiACT.SetAttribute("tCoverStart", tCoverStart);
                oboiACT.SetAttribute("dCoverStart", dCoverStart);
                oboiACT.SetAttribute("rPremium", rPremium.ToString());
                oboiACT.SetAttribute("rCommission", rCommission.ToString());
                oboiACT.SetAttribute("rNetToSupplier", rNetToSupplier.ToString());
                oboiACT.SetAttribute("rBalance", rBalance.ToString());
                oboiACT.SetAttribute("FKiAddOnID", FKiAddOnID.ToString());
                oboiACT.SetAttribute("FKiPurchaseID", FKiPurchaseID.ToString());
                oboiACT.SetAttribute("FKiProductAddOnID", FKiProductAddOnID.ToString());
                oboiACT.SetAttribute("sAnalysis", sAnalysis.ToString());
                oboiACT.SetAttribute("FKiCRAccountID", CRAccountID);
                oboiACT.SetAttribute("FKiDRAccountID", DRAccountID);
                oboiACT.SetAttribute("iProcessStatus", iProcessStatus.ToString());
                oboiACT.SetAttribute("zDefaultDeposit", zDefaultDeposit.ToString());
                oboiACT.SetAttribute("iDepositType", iDepositType.ToString());
                oboiACT.SetAttribute("iPostedFrom", iPostedFrom.ToString());
                oboiACT.SetAttribute("rCommAdj", rCommAdj.ToString());
                oboiACT.SetAttribute("FKsPolicyNo", FKsPolicyNo.ToString());
                oboiACT.SetAttribute("iPaymentType", iPaymentType.ToString());
                oboiACT.SetAttribute("FKiPolicyVersionID", iActivePlicyVersion.ToString());
                //post the transactions for a new policy
                oboiACT.iUserID = Convert.ToInt32(FKiUserID);
                var BOITransactionInsert = new XIIBO();
                if (rPremium != 0)
                {
                    oCResult = oboiACT.Save(oboiACT);
                    BOITransactionInsert = (XIIBO)oCResult.oResult;
                    if (!oCResult.bOK && oCResult.oResult == null)
                    {
                        oCResult.sMessage = "ERROR: Transaction Insert Failed \r\n PolicyID:" + FKiACPolicyID + ", sAnalysis: " + sAnalysis;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oCResult.sMessage = "ERROR: Transaction Insert Failed \r\n PolicyID:" + FKiACPolicyID + ", sAnalysis: " + sAnalysis;
                        oIB.SaveErrortoDB(oCResult);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(sCurrentGUID))
                        {
                            XIInfraCache oICache = new XIInfraCache();
                            oICache.Set_ParamVal(sSessionID, sCurrentGUID, null, "{XIP|ACTransaction_T.id}", BOITransactionInsert.AttributeI("id").sValue, null, null);
                        }
                        if (sTransactionCode == "NEWP" || sTransactionCode == "CHRG" || sTransactionCode == "ADDP" || sTransactionCode == "RETP" || sTransactionCode.ToUpper() == "RNWP" || sTransactionCode.ToUpper() == "RBRP")
                        {
                            oboiACT = new XIIBO();
                            oxidACT = new XIDXI();
                            obodACT = (XIDBO)oxidACT.Get_BODefinition("ACPolicy_T").oResult;
                            oboiACT.BOD = obodACT;
                            oboiACT.SetAttribute("id", FKiACPolicyID.ToString());
                            if (sTransactionCode == "NEWP" || sTransactionCode == "CHRG" || sTransactionCode.ToUpper() == "RNWP"
                                || sTransactionCode.ToUpper() == "RBRP" || sTransactionCode.ToUpper() == "RETP" || sTransactionCode.ToUpper() == "ADDP")
                            {
                                oboiACT.SetAttribute("rCommission", rCommission.ToString());
                                oboiACT.SetAttribute("rNetToSupplier", rNetToSupplier.ToString());
                                oboiACT.SetAttribute("rTax", rTax.ToString());
                                oboiACT.SetAttribute("FKiEnterpriseID", FKiEnterpriseID.ToString());
                            }
                            else
                            {
                                oboiACT.SetAttribute("rMTACommission", rCommission.ToString());
                                oboiACT.SetAttribute("rMTANetToSupplier", rNetToSupplier.ToString());
                                oboiACT.SetAttribute("rMTATax", rTax.ToString());

                                //Update Policy Version
                                XIIBO oBOIPolicyVersion = new XIIBO();
                                XIDXI oXIDPolicyVersion = new XIDXI();
                                XIDBO oBODPolicyVersion = (XIDBO)oXIDPolicyVersion.Get_BODefinition("ACPolicyVersion_T").oResult;
                                oBOIPolicyVersion.BOD = oBODPolicyVersion;
                                oBOIPolicyVersion.SetAttribute("rMTACommission", rCommission.ToString());
                                oBOIPolicyVersion.SetAttribute("rMTANetToSupplier", rNetToSupplier.ToString());
                                oBOIPolicyVersion.SetAttribute("rMTATax", rTax.ToString());
                                oBOIPolicyVersion.SetAttribute("id", iActivePlicyVersion.ToString());
                                oBOIPolicyVersion.iUserID = Convert.ToInt32(FKiUserID);
                                var UpdatePolicyVersion = oBOIPolicyVersion.Save(oBOIPolicyVersion);
                                if (!UpdatePolicyVersion.bOK && UpdatePolicyVersion.oResult == null)
                                {
                                    oCResult.sMessage = "ERROR: Policy Version update Failed \r\n PolicyID:" + FKiACPolicyID + ", Policy VersionID :" + iActivePlicyVersion + ", sAnalysis: " + sAnalysis;
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.LogToFile();
                                    oCResult.sMessage = "ERROR: Policy Version update Failed \r\n PolicyID:" + FKiACPolicyID + ", Policy VersionID :" + iActivePlicyVersion + ", sAnalysis: " + sAnalysis;
                                    oIB.SaveErrortoDB(oCResult);
                                }
                            }
                            oboiACT.iUserID = Convert.ToInt32(FKiUserID);
                            var UpdatePolicy = oboiACT.Save(oboiACT);
                            if (!UpdatePolicy.bOK && UpdatePolicy.oResult == null)
                            {
                                oCResult.sMessage = "ERROR: Policy update Failed \r\n PolicyID:" + FKiACPolicyID + ", sAnalysis: " + sAnalysis;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oCResult.LogToFile();
                                oCResult.sMessage = "ERROR: Policy update Failed \r\n PolicyID:" + FKiACPolicyID + ", sAnalysis: " + sAnalysis;
                                oIB.SaveErrortoDB(oCResult);
                            }
                        }
                        Dictionary<string, string> dictnparams = new Dictionary<string, string>();
                        dictnparams.Add("FKiACTransTypeID", BOITransactionInsert.AttributeI("FKiACTransTypeID").sValue);
                        dictnparams.Add("zBaseValue", BOITransactionInsert.AttributeI("zBaseValue").sValue);
                        dictnparams.Add("rComOverride", rComOverride.ToString());
                        dictnparams.Add("zCommission", BOITransactionInsert.AttributeI("zCommission").sValue);
                        dictnparams.Add("sTransCode", BOITransactionInsert.AttributeI("sTransCode").sValue);
                        dictnparams.Add("FKiACPolicyID", BOITransactionInsert.AttributeI("FKiACPolicyID").sValue);
                        dictnparams.Add("rPremium", BOITransactionInsert.AttributeI("rPremium").sValue);
                        dictnparams.Add("FKiTransactionID", BOITransactionInsert.AttributeI("id").sValue);
                        CResult JournalEntries = new CResult();
                        if (BOITransactionInsert.AttributeI("FKiACPolicyID").iValue > 0)
                        {
                            JournalEntries = JournalEntriesProcessing(dictnparams);
                        }
                        else
                        {
                            JournalEntries = JournalEntriesProcessingForNonPolicy(dictnparams);
                        }
                        if (!JournalEntries.bOK && JournalEntries.oResult == null)
                        {
                            oCResult.sMessage = "ERROR: Something Went Wrong in Inserting Journal Entries \r\n PolicyID:" + FKiACPolicyID + ", sAnalysis: " + sAnalysis;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.LogToFile();
                            oCResult.sMessage = "ERROR: Something Went Wrong in Inserting Journal Entries \r\n PolicyID:" + FKiACPolicyID + ", sAnalysis: " + sAnalysis;
                            oIB.SaveErrortoDB(oCResult);
                        }
                    }
                }
                else
                {
                    oCResult.sMessage = "ERROR: rPremium should not be 0 \r\n PolicyID:" + FKiACPolicyID + ", sAnalysis: " + sAnalysis;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oCResult.sMessage = "ERROR: rPremium should not be 0 \r\n PolicyID:" + FKiACPolicyID + ", sAnalysis: " + sAnalysis;
                    oIB.SaveErrortoDB(oCResult);
                }
                //XIIBO oboiPol = new XIIBO();
                //XIDXI oxidPol = new XIDXI();
                //XIDBO obodPol = (XIDBO)oxidPol.Get_BODefinition("ACPolicy_T").oResult;
                //oboiPol.BOD = obodPol;
                //oboiPol.SetAttribute("id", FKiACPolicyID.ToString());
                //oboiPol.SetAttribute("rIPT", TransactionInsert.AttributeI("rTax").sValue);
                //var PolicyUpdate = oboiPol.Save(oboiPol);
                //if (!PolicyUpdate.oCResult.bOK && PolicyUpdate.oCResult.oResult == null)
                //{
                //    oCResult.sMessage = "ERROR: Policy Update Failed \r\n";
                //    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                //    oCResult.LogToFile();
                //    oIB.SaveErrortoDB(oCResult);
                //}

                //Admin Transaction Insert Code
                //if (rAdmin != 0 && sTransCode.ToLower() != "ADMIN".ToLower())
                //{
                //    XIIBO oboiAACT = new XIIBO(TXInitiation);
                //    XIDXI oxidAACT = new XIDXI();
                //    XIDBO obodAACT = (XIDBO)oxidAACT.Get_BODefinition("ACTransaction_T").oResult;
                //    oboiAACT.BOD = obodAACT;

                //    oboiAACT.SetAttribute("FKiACPolicyID", FKiACPolicyID.ToString());
                //    oboiAACT.SetAttribute("FKiEDITransTypeID", FKiEDITransTypeID.ToString());
                //    oboiAACT.SetAttribute("iAllocPriority", FKiEDITransTypeID.ToString());
                //    oboiAACT.SetAttribute("FKiBrokerID", FKiBrokerID.ToString());
                //    oboiAACT.SetAttribute("sNotes", sNotes);
                //    oboiAACT.SetAttribute("zBaseValue", rAdmin.ToString());
                //    oboiAACT.SetAttribute("iDays", iDays.ToString());
                //    oboiAACT.SetAttribute("sPolTransCode", sTransactionCode.ToString());
                //    oboiAACT.SetAttribute("iSystemType", iSystemType.ToString());
                //    oboiAACT.SetAttribute("FKsWhoID", FKsUserID);
                //    oboiAACT.SetAttribute("refTypeID", refTypeID.ToString());
                //    oboiAACT.SetAttribute("refAnalysis1", refAnalysis1);
                //    oboiAACT.SetAttribute("sName", "Admin Charge");
                //    oboiAACT.SetAttribute("sTransCode", "ADMIN");
                //    oboiAACT.SetAttribute("iStatus", Convert.ToInt32(EnumTransactions.Live).ToString());
                //    oboiAACT.SetAttribute(XIConstant.Key_XIDeleted, "0");

                //    List<CNV> oWhere = new List<CNV>();
                //    XIIXI oIXI = new XIIXI();
                //    oWhere.Add(new CNV { sName = "sCode", sValue = "ADMIN" });
                //    oData = oIXI.BOI("ACTransType_T", null, null, oWhere).Structure("TRANTYPE").XILoad();
                //    var TransTypeID = oData.oSubStructureI("ACTransType_T").oBOIList.FirstOrDefault();
                //    AdminTransTypeID = TransTypeID.AttributeI("ID").iValue;//Convert.ToInt32(TransTypeID.Attributes["ID"].sValue);
                //    iTransType = TransTypeID.AttributeI("iType").iValue;
                //    oboiAACT.SetAttribute("FKiACTransTypeID", AdminTransTypeID.ToString());
                //    //oboiAACT.SetAttribute("FKiQuoteID", FKiQuoteID.ToString());
                //    oboiAACT.SetAttribute("FKiClientID", FKiCustomerID.ToString());
                //    oboiAACT.SetAttribute("FKiSupplierID", FKiSupplierID.ToString());
                //    oboiAACT.SetAttribute("FKiProductID", FKiProductID.ToString());

                //    oboiAACT.SetAttribute("FKiBrokerID", FKiBrokerID.ToString());
                //    oboiAACT.SetAttribute("FKiEnterpriseID", FKiEnterpriseID.ToString());
                //    oboiAACT.SetAttribute("iAdminType", iAdminType.ToString());
                //    oboiAACT.SetAttribute("iDepositType", iDepositType.ToString());
                //    oboiAACT.SetAttribute("sPolTransCode", sTransactionCode.ToString());

                //    oboiAACT.SetAttribute("refAccountCategory", refAccountCategory.ToString());
                //    oboiAACT.SetAttribute("FKiACPolicyAllocID", 0.ToString());
                //    //oboiAACT.SetAttribute("XICreatedWhen", DateTime.Now.ToString());
                //    //oboiAACT.SetAttribute("XIUpdatedWhen", DateTime.Now.ToString());
                //    oboiAACT.SetAttribute("dWhen", dWhen + " " + DateTime.Now.ToString("HH:mm:ss"));
                //    oboiAACT.SetAttribute("tCoverStart", tCoverStart);
                //    oboiAACT.SetAttribute("dCoverStart", dCoverStart);
                //    oboiAACT.SetAttribute("FKiAddOnID", FKiAddOnID.ToString());
                //    oboiAACT.SetAttribute("FKiPurchaseID", FKiPurchaseID.ToString());
                //    oboiAACT.SetAttribute("FKiProductAddOnID", FKiProductAddOnID.ToString());
                //    oboiAACT.SetAttribute("sAnalysis", sAnalysis.ToString());
                //    oboiAACT.SetAttribute("iPostedFrom", iPostedFrom.ToString());
                //    oboiAACT.SetAttribute("FKsPolicyNo", FKsPolicyNo.ToString());
                //    oboiAACT.SetAttribute("iPaymentType", iPaymentType.ToString());
                //    oCResult = oboiAACT.Save(oboiAACT);
                //    var BOIAdminTransactionInsert = (XIIBO)oCResult.oResult;
                //    if (!oCResult.bOK && oCResult.oResult == null)
                //    {
                //        oCResult.sMessage = "ERROR: Transaction Insert Failed \r\n PolicyID:" + FKiACPolicyID + ", sAnalysis: " + sAnalysis;
                //        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                //        oCResult.LogToFile();
                //        oCResult.sMessage = "ERROR: Transaction Insert Failed \r\n PolicyID:" + FKiACPolicyID + ", sAnalysis: " + sAnalysis;
                //        oIB.SaveErrortoDB(oCResult);
                //    }
                //    else
                //    {
                //        Dictionary<string, string> dictparams = new Dictionary<string, string>();
                //        dictparams.Add("FKiACTransTypeID", BOIAdminTransactionInsert.AttributeI("FKiACTransTypeID").sValue);
                //        dictparams.Add("zBaseValue", BOIAdminTransactionInsert.AttributeI("zBaseValue").sValue);
                //        dictparams.Add("rComOverride", rComOverride.ToString());
                //        dictparams.Add("zCommission", 0.ToString());
                //        dictparams.Add("sTransCode", BOIAdminTransactionInsert.AttributeI("sTransCode").sValue);
                //        dictparams.Add("FKiACPolicyID", BOIAdminTransactionInsert.AttributeI("FKiACPolicyID").sValue);
                //        dictparams.Add("rPremium", BOIAdminTransactionInsert.AttributeI("zBaseValue").sValue);
                //        dictparams.Add("FKiTransactionID", BOIAdminTransactionInsert.AttributeI("id").sValue);
                //        CResult JournalEntries = new CResult();
                //        if (BOIAdminTransactionInsert.AttributeI("FKiACPolicyID").iValue > 0)
                //        {
                //            JournalEntries = JournalEntriesProcessing(dictparams);
                //        }
                //        else
                //        {
                //            JournalEntries = JournalEntriesProcessingForNonPolicy(dictparams);
                //        }
                //        if (!JournalEntries.bOK && JournalEntries.oResult == null)
                //        {
                //            oCResult.sMessage = "ERROR: Somthing Went Wrong in Inserting Journal Entries \r\n PolicyID:" + FKiACPolicyID + ", sAnalysis: " + sAnalysis;
                //            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                //            oCResult.LogToFile();
                //            oCResult.sMessage = "ERROR: Somthing Went Wrong in Inserting Journal Entries \r\n PolicyID:" + FKiACPolicyID + ", sAnalysis: " + sAnalysis;
                //            oIB.SaveErrortoDB(oCResult);
                //        }
                //    }
                //    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                //}
                //}
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n PolicyID:" + FKiACPolicyID + ", sAnalysis:" + sAnalysis;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n PolicyID:" + FKiACPolicyID + ", sAnalysis:" + sAnalysis;
                oIB.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        public CResult GetACCTransType(string sTransCode, int XIDeleted, string sOpposingCode)
        {
            CResult oCResult = new CResult();
            XIInstanceBase oIB = new XIInstanceBase();
            try
            {
                oCResult.sClassName = oCResult.Get_Class();
                oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
                XIIXI oIXI = new XIIXI();
                List<CNV> nParams = new List<CNV>();
                nParams.Add(new CNV { sName = "sCode", sValue = sTransCode == "" ? sOpposingCode.ToString() : sTransCode.ToString() });
                nParams.Add(new CNV { sName = XIConstant.Key_XIDeleted, sValue = XIDeleted.ToString() });
                XIIBO OTransType = oIXI.BOI("ACTransType_T", "", "Main", nParams);
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = (object)OTransType;
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

        public CResult Get_ProductVersion(DateTime dEffectiveFrom)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            XIInstanceBase oIB = new XIInstanceBase();
            try
            {
                oCResult.sClassName = oCResult.Get_Class();
                oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
                XIInfraCache oCache = new XIInfraCache();
                var oOneClick = (XID1Click)oCache.GetObjectFromCache("oneclick", "Product Details", ""); // hard coded 1click ID  //1116.ToString()
                XIDBO oBOD = new XIDBO();
                oBOD = (XIDBO)oCache.GetObjectFromCache("bo", null, oOneClick.BOID.ToString());
                oOneClick.BOD = oBOD;
                //get DataSource
                XIDXI oDXI = new XIDXI();
                List<XIIBO> oBOInstanceLIst = new List<XIIBO>();
                XIDStructure oStructure = new XIDStructure();
                List<CNV> nParams = new List<CNV>();
                nParams.Add(new CNV { sName = "{XIP|" + dEffectiveFrom + "}", sValue = dEffectiveFrom.ToString() });
                nParams.AddRange(nParams.Where(m => m.sType == "Structure").ToList());
                var sDataSource = oDXI.GetBODataSource(oBOD.iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                oOneClick.sConnectionString = sDataSource;
                oOneClick.BOID = oOneClick.BOID;
                oOneClick.Query = oStructure.ReplaceExpressionWithCacheValue(oOneClick.Query, nParams);
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oOneClick.OneClick_Execute();
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

        public CResult Get_AccountDetails(int FKiClientID, int iType)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            XIInstanceBase oIB = new XIInstanceBase();
            try
            {
                oCResult.sClassName = oCResult.Get_Class();
                oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
                XIInfraCache oCache = new XIInfraCache();
                XID1Click Account1Click1 = new XID1Click();
                XID1Click Account1Click2 = new XID1Click();
                Account1Click1 = (XID1Click)oCache.GetObjectFromCache("oneclick", "Account Details", ""); // hard coded 1click ID  //1116.ToString()
                Account1Click2 = (XID1Click)Account1Click1.Clone(Account1Click1);
                XIDBO oBOD = new XIDBO();
                oBOD = (XIDBO)oCache.GetObjectFromCache("bo", null, Account1Click1.BOID.ToString());
                Account1Click2.BOD = oBOD;
                //get DataSource
                XIDXI oDXI = new XIDXI();
                List<XIIBO> oBOInstanceLIst = new List<XIIBO>();
                XIDStructure oStructure = new XIDStructure();
                List<CNV> nParams = new List<CNV>();
                nParams.Add(new CNV { sName = "{XIP|FKiClientID}", sValue = FKiClientID.ToString() });
                nParams.Add(new CNV { sName = "{XIP|iType}", sValue = iType.ToString() });
                nParams.AddRange(nParams.Where(m => m.sType == "Structure").ToList());
                var sDataSource = oDXI.GetBODataSource(oBOD.iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                Account1Click2.sConnectionString = sDataSource;
                Account1Click2.BOID = Account1Click2.BOID;
                Account1Click2.Query = oStructure.ReplaceExpressionWithCacheValue(Account1Click2.Query, nParams);
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = Account1Click2.OneClick_Execute();
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

        public CResult GetTransTypeID(string sCode)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            XIInstanceBase oIB = new XIInstanceBase();
            try
            {
                oCResult.sClassName = oCResult.Get_Class();
                oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
                XIIXI oIXI = new XIIXI();
                List<CNV> nParams = new List<CNV>();
                nParams.Add(new CNV { sName = "sCode", sValue = sCode });
                XIIBO OTransType = oIXI.BOI("ACTransType_T", "", "QS", nParams);
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = (object)OTransType;
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

        #endregion


        #region Transaction_Latest

        private void WriteError(CResult currentobj, string message)
        {
            XIInstanceBase oIB = new XIInstanceBase();
            currentobj.sMessage = message;
            currentobj.xiStatus = xiEnumSystem.xiFuncResult.xiError;
            currentobj.LogToFile();
            oIB.SaveErrortoDB(currentobj);
        }
        public CResult PostTransaction(XIIBO oTransaction, string sDataBase, string sSessionID = "", string sCurrentGUID = "", bool bLocalnoEDI = false, string sUserID = "")
        {
            CResult oCResult = new CResult();
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                if (oTransaction.AttributeI("zBaseValue").doValue != 0)
                {
                    oCResult.sClassName = oCResult.Get_Class();
                    oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
                    bool bPolicy = true;
                    XIInstanceBase oIB = new XIInstanceBase();
                    XIIXI oIXI = new XIIXI();
                    XIIBO oPolicy = null;
                    XIIBO oProduct = null;
                    DateTime dEffectiveFrom = new DateTime();
                    //DRAccountID = oTransaction.AttributeI("FromAccount").sValue;
                    //CRAccountID = oTransaction.AttributeI("ToAccount").sValue;
                    rCommission = 0; FKiProductAddOnID = 0; iTransTypeCode = 0;
                    rComOverride = Convert.ToDecimal(oTransaction.AttributeI("rComOverride").doValue);
                    zCommission = Convert.ToDecimal(oTransaction.AttributeI("zCommission").doValue);
                    iCommissionType = Convert.ToInt32(oTransaction.AttributeI("iCommissionType").doValue);
                    rNetToSupplier = Convert.ToDecimal(oTransaction.AttributeI("rNetToSupplier").doValue);
                    rTax = Convert.ToDecimal(oTransaction.AttributeI("rTax").doValue);
                    zBaseValue = Convert.ToDecimal(oTransaction.AttributeI("zBaseValue").doValue);
                    rBalance = Convert.ToDecimal(oTransaction.AttributeI("rBalance").doValue);
                    iProcessStatus = oTransaction.AttributeI("iProcessStatus").iValue;
                    FKiSupplierID = oTransaction.AttributeI("FKiSupplierID").iValue;
                    sTransCode = oTransaction.AttributeI("sTransCode").sValue;
                    iStatus = oTransaction.AttributeI("iStatus").iValue;
                    FKiPFSchemeID = oTransaction.AttributeI("FKiPFSchemeID").iValue;
                    List<CNV> oParams = new List<CNV>();
                    oParams.Add(new CNV { sName = "iUserID", sValue = oTransaction.AttributeI("FKiUserID").sValue });
                    oParams.Add(new CNV { sName = "sDataBase", sValue = sDataBase });
                    FKsUserID = GetUser(oParams);
                    FKiUserID = oTransaction.AttributeI("FKiUserID").sValue;
                    FKsWhoID = oTransaction.AttributeI("FKsWhoID").sValue;
                    iPostedFrom = oTransaction.AttributeI("iPostedFrom").iValue == 0 ? iPostedFrom : oTransaction.AttributeI("iPostedFrom").iValue;
                    //FKsUserID = oTransaction.AttributeI("FKsUserID").sValue == null ? FKsUserID : oTransaction.AttributeI("FKsUserID").sValue;
                    tCoverStart = oTransaction.AttributeI("tCoverStart").sValue == null ? DateTime.Now.ToString(XIConstant.Time_Format) : oTransaction.AttributeI("tCoverStart").sValue;
                    dCoverStart = oTransaction.AttributeI("dCoverStart").sValue == null ? DateTime.Now.ToString(XIConstant.Date_Format) + " " + DateTime.Now.ToString(XIConstant.Time_Format) : oTransaction.AttributeI("dCoverStart").sValue;
                    iPaymentType = oTransaction.AttributeI("iPaymentType").sValue == null ? iPaymentType : Convert.ToInt32(oTransaction.AttributeI("iPaymentType").sValue);
                    iSystemType = oTransaction.AttributeI("iSystemType").sValue == null ? 100 : Convert.ToInt32(oTransaction.AttributeI("iSystemType").sValue);
                    XIDeleted = oTransaction.AttributeI(XIConstant.Key_XIDeleted).sValue == null ? 0 : Convert.ToInt32(oTransaction.AttributeI(XIConstant.Key_XIDeleted).sValue);
                    //get transation type info here
                    List<CNV> oWhere = new List<CNV>();
                    oWhere.Add(new CNV { sName = "sCode", sValue = oTransaction.AttributeI("sTransCode").sValue });
                    oData = oIXI.BOI("ACTransType_T", null, null, oWhere).Structure("TRANTYPE", FKiUserID).XILoad("", false, FKiUserID);

                    //CResult ACCTransType = GetACCTransType(oTransaction.AttributeI("sTransCode").sValue, 0, "");
                    //var OTransType = (XIIBO)ACCTransType.oResult;
                    var OTransType = oData.oSubStructureI("ACTransType_T").oBOIList.FirstOrDefault();
                    if (OTransType == null)
                    {
                        WriteError(oCResult, "ERROR: Cannot load Transaction Type for Policy Calculation: " + oTransaction.AttributeI("FKiACPolicyID").iValue + " and Product :" + FKiProductID + "  \r\n");
                    }
                    else
                    {
                        bPolicy = OTransType.AttributeI("bPolicy").bValue;
                        sNameACTT = string.IsNullOrEmpty(oTransaction.AttributeI("sName").sValue) ? OTransType.AttributeI("sName").sValue : oTransaction.AttributeI("sName").sValue;
                        bIPT = OTransType.AttributeI("bIPT").bValue;
                        FKiACTransTypeID = OTransType.AttributeI("id").iValue;
                        iTransType = OTransType.AttributeI("iType").iValue;
                        bCommission = OTransType.AttributeI("bCommission").bValue;
                    }
                    dWhen = oTransaction.AttributeI("dWhen").sValue == null ? DateTime.Now.ToString(XIConstant.Date_Format) : Convert.ToDateTime(oTransaction.AttributeI("dWhen").sValue).ToString(XIConstant.Date_Format);
                    refAccountCategory = oTransaction.AttributeI("refAccountCategory").sValue == "" ? 0 : Convert.ToInt32(oTransaction.AttributeI("refAccountCategory").sValue);
                    sNotes = oTransaction.AttributeI("sNotes").sValue;
                    if (oTransaction.AttributeI("FKiACPolicyID").iValue > 0) //if policy available get policy details ( && bPolicy)
                    {
                        oPolicy = oIXI.BOI("ACPolicy_T", oTransaction.AttributeI("FKiACPolicyID").iValue.ToString(), "Policy Details for Transaction", null);
                        if (oPolicy != null) //Assign policy Details to varialbles
                        {
                            FKiACPolicyID = oTransaction.AttributeI("FKiACPolicyID").iValue;
                            FKiBrokerID = oPolicy.AttributeI("FKiBrokerID").iValue;
                            refAccountCategory = refAccountCategory == 0 ? oPolicy.AttributeI("refAccountCategory").iValue : refAccountCategory;
                            FKiCustomerID = oPolicy.AttributeI("FKiCustomerID").iValue;
                            FKiQuoteID = oPolicy.AttributeI("FKiQuoteID").iValue;
                            FKiSupplierID = FKiSupplierID == 0 ? oPolicy.AttributeI("FKiSupplierID").iValue : FKiSupplierID;
                            FKiCPolicyCalcID = oPolicy.AttributeI("FKiCPolicyCalcID").iValue;
                            refTypeID = oPolicy.AttributeI("refTypeID").iValue;
                            rPremium = Convert.ToDecimal(oTransaction.AttributeI("rPremium").doValue);
                            FKiProductID = oPolicy.AttributeI("FKiProductID").iValue;
                            iVersionNo = oPolicy.AttributeI("iVersionNo").iValue;
                            FKiEDIID = oPolicy.AttributeI("FKiEDIID").iValue;
                            sName = oPolicy.AttributeI("sName").sValue;
                            dEffectiveFrom = oPolicy.AttributeI("dEffectiveFrom").dValue;
                            dCurrentEffFrom = oPolicy.AttributeI("dCurrentEffFrom").dValue;
                            FKsPolicyNo = oPolicy.AttributeI("sPolicyNo").sValue;
                        }
                        oWhere = new List<CNV>();
                        oWhere.Add(new CNV { sName = "FKiACPolicyID", sValue = oTransaction.AttributeI("FKiACPolicyID").sValue });
                        oWhere.Add(new CNV { sName = "iStatus", sValue = "10" }); // Active Policy Version Status
                        var oPolicyVersion = oIXI.BOI("ACPolicyVersion_T", null, "id", oWhere);
                        iActivePlicyVersion = oPolicyVersion.AttributeI("id").iValue;
                        oIXI = new XIIXI();
                        oProduct = oIXI.BOI("Product", FKiProductID.ToString()); // Get Product Details 
                        if (oProduct != null)
                        {
                            //iSeqMethod = oProduct.AttributeI("iSeqMethod").iValue;
                            iType = oProduct.AttributeI("iType").iValue;
                            sNotes = sNotes == "" ? oProduct.AttributeI("sNotes").sValue : sNotes;
                            if (FKiProductID != 0)
                            {
                                sProductOverrideID = FKiProductID;
                            }
                            FKiEDITransTypeID = oProduct.AttributeI("FKiEDITypeID").iValue;
                            //sEDIProcessingClass = oProduct.AttributeI("sEDIProcessingClass").sValue;
                            //iEDIMode = oProduct.AttributeI("iEDIMode").iValue;
                            //FKiEDITypeID = oProduct.AttributeI("FKiEDITypeID").iValue;
                            refAnalysis1 = oProduct.AttributeI("refAnalysis1").sValue;
                            zCommission = (bCommission == true && !string.IsNullOrEmpty(sNotes)) ? oTransaction.AttributeI("zCommission").rValue : zCommission == 0 ? oProduct.AttributeI("zCommission").rValue : oTransaction.AttributeI("zCommission").rValue;
                            iCommissionType = oProduct.AttributeI("iCommissionType").iValue;
                            //iAdminType = oProduct.AttributeI("iAdminType").iValue;
                            //zDefaultAdmin = oTransaction.AttributeI("zDefaultAdmin").doValue == 0 ? Convert.ToDecimal(oProduct.AttributeI("zDefaultAdmin").doValue) : Convert.ToDecimal(oTransaction.AttributeI("zDefaultAdmin").doValue);
                            iDepositType = oProduct.AttributeI("iDepositType").iValue;
                            zDefaultDeposit = oProduct.AttributeI("zDefaultDeposit").iValue;
                            iTaxType = oProduct.AttributeI("iTaxType").iValue;
                            if (oTransaction.AttributeI("sTransCode").sValue == "CHRG")
                            {
                                rAdmin = oProduct.AttributeI("rCancellationFee").iValue;
                            }
                            else if (oTransaction.AttributeI("sTransCode").sValue == "NEWP" || oTransaction.AttributeI("sTransCode").sValue == "CREF" || oTransaction.AttributeI("sTransCode").sValue == "CREF2")
                            {
                                rAdmin = iAdminType == 0 ? (rPremium * (zDefaultAdmin / 100)) : zDefaultAdmin;
                            }
                            else
                            {
                                rAdmin = 0; zDefaultAdmin = 0;
                            }
                            //sIPTSettings = oProduct.AttributeI("sIPTSettings").sValue;
                            //iIPTDate = oProduct.AttributeI("iIPTDate").iValue;
                            //iIPTChange = oProduct.AttributeI("iIPTChange").iValue;
                            //FKiEnterpriseID = oProduct.AttributeI("FKiEnterpriseID").iValue;
                        }
                    }  // PolicyID > 0 && bPolicy
                    else
                    {
                        FKiSupplierID = oTransaction.AttributeI("FKiSupplierID").iValue;
                        rPremium = Convert.ToDecimal(oTransaction.AttributeI("zBaseValue").doValue);
                        FKiACPolicyID = oTransaction.AttributeI("FKiACPolicyID").iValue;
                        FKiPFSchemeID = oTransaction.AttributeI("FKiPFSchemeID").iValue;
                        iType = oTransaction.AttributeI("iType").iValue;
                        FKiCustomerID = oTransaction.AttributeI("FKiClientID").iValue;
                        FKiProductID = oTransaction.AttributeI("FKiProductID").iValue;
                        FKsPolicyNo = "0";
                    }
                    if (bLocalnoEDI == false)
                    {
                        if (!oACCTransTypeClick.bOK && oACCTransTypeClick.oResult == null)
                        {
                            WriteError(oCResult, "ERROR: Cannot load Transaction Type for Policy Calculation: " + oTransaction.AttributeI("FKiACPolicyID").iValue + " and Product :" + FKiProductID + "  \r\n");
                        }
                        else
                        {
                            if (rPremium < 0)
                            {
                                sOpposingCode = OTransType.AttributeI("sOpposingTransCode").sValue;
                                if (!string.IsNullOrEmpty(sOpposingCode))
                                {
                                    var oACCTransTypeOpClick = new CResult();
                                    oACCTransTypeOpClick = GetACCTransType("", 0, sOpposingCode);
                                    var ACCTransTypeOp = (XIIBO)oACCTransTypeOpClick.oResult;
                                    if (!oACCTransTypeOpClick.bOK && oACCTransTypeOpClick.oResult == null)
                                    {
                                        WriteError(oCResult, "ERROR: Cannot load Transaction Type for Policy Calculation: " + oTransaction.AttributeI("FKiACPolicyID").iValue + " and Product :" + FKiProductID + "  \r\n");
                                    }
                                    else
                                    {
                                        //trans type loaded
                                        oACCTransTypeClick = oACCTransTypeOpClick;
                                        var ACCTransTypeFinal = (XIIBO)oACCTransTypeClick.oResult;
                                        if (ACCTransTypeFinal != null)
                                        {
                                            FKiACTransTypeID = ACCTransTypeFinal.AttributeI("id").iValue;
                                            iType = ACCTransTypeFinal.AttributeI("iType").iValue;
                                            sNameACTT = ACCTransTypeFinal.AttributeI("sName").sValue;
                                        }
                                        rPremium = rPremium * -1;
                                        rAdmin = rAdmin * -1;
                                        rInsurerCharge = rInsurerCharge * -1;
                                        sTransCode = sOpposingCode;
                                        oIXI = new XIIXI();
                                        var oProductAddon = oIXI.BOI("ProductAddon_T", FKiProductID.ToString());
                                        if (oProductAddon != null)
                                        {
                                            FKiAddOnID = oProductAddon.AttributeI("refAddon").iValue;
                                        }
                                        var oCUseProduct = oProduct;
                                        if (iType == 10 || iType == 40 || !string.IsNullOrEmpty(sAddOnID))
                                        {
                                            if (sCommissionOverride == "" && oProductAddon == null)
                                            {
                                                if (oProductAddon == null)
                                                {
                                                    var oCUseProductoverride = Get_ProductVersion(dEffectiveFrom);
                                                    if (oCUseProductoverride.bOK && oCUseProductoverride.oResult != null)
                                                    {
                                                        var oCUseProductoresult = (Dictionary<string, XIIBO>)oCUseProductoverride.oResult;
                                                        oCUseProduct = oCUseProductoresult.Values.FirstOrDefault();
                                                    }
                                                }
                                            }
                                        }
                                        else if (oProductAddon == null)
                                        {
                                            var oCUseProductoverride = Get_ProductVersion(dEffectiveFrom);
                                            if (oCUseProductoverride.bOK && oCUseProductoverride.oResult != null)
                                            {
                                                var oCUseProductoresult = (Dictionary<string, XIIBO>)oCUseProductoverride.oResult;
                                                oCUseProduct = oCUseProductoresult.Values.FirstOrDefault();
                                            }
                                        }
                                    }
                                }
                            } //end for rPremium < 0
                        }
                        sRebroke = "N";
                        // sTransCode = sTransCode;
                        //sPolTransCode = ""; //////////Not Used
                        //////////////////////////
                        //sTransCode = sTransCode;
                        if (sRebroke == "Y" && sTransCode == "RNWP")
                        {
                            sTransCode = "RBRP";
                        }
                        XIIBO oboiT = new XIIBO();
                        XIDXI oxidT = new XIDXI();
                        switch (sTransCode)
                        {
                            case "NEWP":
                            case "RBRP":
                                if (sTransCode == "NEWP")
                                {
                                    sTransactionCode = "NEWP";
                                }
                                else if (sTransCode == "RBRP")
                                {
                                    sTransactionCode = "RBRP";
                                }
                                iActionCode = 10;     // new
                                iTransTypeCode = 0;
                                iVersionNo = iVersionNo + 1;
                                iNewPolStatus = 10;  //live
                                break;
                            case "ADDP":  //' 15     'pending MTA, so next stage has to be MTA
                                iActionCode = 20;     //mta
                                iTransTypeCode = 10;
                                sTransactionCode = "ADDP";     // "MTA"
                                iVersionNo = iVersionNo + 1;
                                break;

                            case "RNWP":        //25     'pending renewal
                                iActionCode = 40;  //renew
                                sTransactionCode = "RNWP";     // "RENEW"
                                iTransTypeCode = 30;
                                iVersionNo = iVersionNo + 1;
                                iNewPolStatus = 20;  //renewed
                                break;
                            case "RETP":        //190    'pending cancellation
                                iActionCode = 30;     //cancel
                                iTransTypeCode = 40;
                                sTransactionCode = "RETP";  // "RETURN"
                                iVersionNo = iVersionNo + 1;
                                iNewPolStatus = 200; //cancelled
                                break;
                            default:
                                sTransactionCode = sTransCode;
                                iActionCode = -1;
                                iTransTypeCode = -10;
                                break;
                        }
                        oIXI = new XIIXI();
                        var oCalc = new XIIBO();
                        if (sTransactionCode == "ADDON")
                        {
                            //Dictionary<string, XIIBO> oXIBOI = new Dictionary<string, XIIBO>();
                            //QueryEngine oQE = new QueryEngine();
                            //string sWhereCondition = "FKiACPolicyID =" + oTransaction.AttributeI("FKiACPolicyID").iValue.ToString() + ", iStatus = 10";
                            //var oQResult = oQE.Execute_QueryEngine("ACPurchase_T", "*", sWhereCondition);
                            //if (oQResult.bOK && oQResult.oResult != null)
                            //{
                            //    oXIBOI = (Dictionary<string, XIIBO>)oQResult.oResult;
                            //}

                            decimal rBreakdownCover = 0, rXSProtection = 0;
                            int iQBusType = 0;
                            iSystemType = 110;
                            //foreach (var item in oXIBOI)
                            //{
                            rPremium = 0; zCommission = 0; zDefaultAdmin = 0; zDefaultDeposit = 0; rAdmin = 0;
                            if (oTransaction.AttributeI("FKiProductAddOnID").iValue != 0)
                            {
                                FKiProductAddOnID = oTransaction.AttributeI("FKiProductAddonID").iValue;
                                List<CNV> oParam = new List<CNV>();
                                oParam.Add(new CNV { sName = "id", sValue = FKiProductAddOnID.ToString() });
                                var oProductAddOn = oIXI.BOI("ProductAddon_T", null, null, oParam);
                                if (oProductAddOn != null)
                                {
                                    //FKiAddOnID = oProductAddon.AttributeI("refAddon").iValue;
                                    zBaseValue = oTransaction.AttributeI("zBaseValue").rValue;
                                    iCommissionType = oProductAddOn.AttributeI("iCommissionType").iValue;
                                    zCommission = oProductAddOn.AttributeI("zCommission").rValue;
                                    //iAdminType = oProductAddOn.AttributeI("iAdminType").iValue;
                                    //zDefaultAdmin = oProductAddOn.AttributeI("zDefaultAdmin").rValue;
                                    iDepositType = oProductAddOn.AttributeI("iDepositType").iValue;
                                    zDefaultDeposit = oProductAddOn.AttributeI("zDefaultDeposit").rValue;
                                    iTaxType = oProductAddOn.AttributeI("iTaxType").iValue;
                                    rPremium = oTransaction.AttributeI("zBaseValue").rValue;
                                    iPostedFrom = 20;
                                    FKiAddOnID = oProductAddOn.AttributeI("refAddon").iValue;
                                    //rAdmin = zDefaultAdmin;// iAdminType == 0 ? (rPremium * (zDefaultAdmin / 100)) : zDefaultAdmin;
                                    var oBaseAddOn = oIXI.BOI("refAddon_T", oProductAddOn.AttributeI("refAddon").iValue.ToString());
                                    if (oBaseAddOn != null)
                                    {
                                        if (oBaseAddOn.AttributeI("iBusinessType").iValue == 10)
                                        {
                                            rBreakdownCover = rBreakdownCover + oCalc.AttributeI("rCost").rValue;
                                        }
                                        else if (oBaseAddOn.AttributeI("iBusinessType").iValue == 30)
                                        {
                                            rXSProtection = rXSProtection + oCalc.AttributeI("rCost").rValue;
                                        }
                                        iQBusType = oBaseAddOn.AttributeI("iBusinessType").iValue;
                                    }
                                }
                                oCResult = AddTransaction(iNewPolStatus, iTransTypeCode, sTransactionCode, refTypeID, sSessionID, sCurrentGUID);
                                if (!oCResult.bOK && oCResult.oResult == null)
                                {
                                    WriteError(oCResult, "ERROR: Something went wrong in GoLive \r\n");
                                }
                            }
                            //    else if (oTransaction.AttributeI("FKiProductAddOnID").iValue == 0)
                            //    {
                            //        FKiProductAddOnID = item.Value.AttributeI("FKiProductAddonID").iValue;
                            //        List<CNV> oParam = new List<CNV>();
                            //        oParam.Add(new CNV { sName = "id", sValue = FKiProductAddOnID.ToString() });
                            //        var oProductAddOn = oIXI.BOI("ProductAddon_T", null, null, oParam);
                            //        if (oProductAddOn != null)
                            //        {
                            //            //FKiAddOnID = oProductAddon.AttributeI("refAddon").iValue;
                            //            zBaseValue = item.Value.AttributeI("rPremiumOverride").rValue;
                            //            iCommissionType = oProductAddOn.AttributeI("iCommissionType").iValue;
                            //            zCommission = oProductAddOn.AttributeI("zCommission").rValue;
                            //            iAdminType = oProductAddOn.AttributeI("iAdminType").iValue;
                            //            zDefaultAdmin = oProductAddOn.AttributeI("zDefaultAdmin").rValue;
                            //            iDepositType = oProductAddOn.AttributeI("iDepositType").iValue;
                            //            zDefaultDeposit = oProductAddOn.AttributeI("zDefaultDeposit").rValue;
                            //            iTaxType = oProductAddOn.AttributeI("iTaxType").iValue;
                            //            rPremium = item.Value.AttributeI("rPremiumOverride").rValue;
                            //            iPostedFrom = 20;
                            //            rAdmin = zDefaultAdmin;// iAdminType == 0 ? (rPremium * (zDefaultAdmin / 100)) : zDefaultAdmin;
                            //            var oBaseAddOn = oIXI.BOI("refAddon_T", oProductAddOn.AttributeI("refAddon").iValue.ToString());
                            //            if (oBaseAddOn != null)
                            //            {
                            //                if (oBaseAddOn.AttributeI("iBusinessType").iValue == 10)
                            //                {
                            //                    rBreakdownCover = rBreakdownCover + oCalc.AttributeI("rCost").rValue;
                            //                }
                            //                else if (oBaseAddOn.AttributeI("iBusinessType").iValue == 30)
                            //                {
                            //                    rXSProtection = rXSProtection + oCalc.AttributeI("rCost").rValue;
                            //                }
                            //                iQBusType = oBaseAddOn.AttributeI("iBusinessType").iValue;
                            //            }
                            //        }
                            //        oCResult = AddTransaction(iNewPolStatus, iTransTypeCode, sTransactionCode, refTypeID, sSessionID, sCurrentGUID);
                            //        if (!oCResult.bOK && oCResult.oResult == null)
                            //        {
                            //            WriteError(oCResult, "ERROR: Something went wrong in GoLive \r\n");
                            //        }
                            //    }
                            //}
                            //oCalc = oIXI.BOI("PolicyCalculation_T", FKiCPolicyCalcID.ToString());
                        }
                        else
                        {
                            //oCalc = oIXI.BOI("PolicyCalculation_T", FKiCPolicyCalcID.ToString());
                        }
                        if (oCalc != null && oCalc.Attributes.Count() > 0)
                        {
                            oCalc.SetAttribute("FKiClientID", oPolicy.AttributeI("FKiCustomerID").iValue.ToString());
                            iStatus = oCalc.AttributeI("iStatus").iValue;
                            //int.TryParse(GetAttributeDetails(oCalc.Attributes, "iStatus"), out iStatus);
                            if (iStatus == 0)
                            {
                                WriteError(oCResult, "ERROR: Policy Calculation is not Active \r\n");
                            }
                            else if (sTransactionCode != "ADDON")
                            {
                                oCResult = AddTransaction(iNewPolStatus, iTransTypeCode, sTransactionCode, refTypeID, sSessionID, sCurrentGUID);
                                if (!oCResult.bOK && oCResult.oResult == null)
                                {
                                    WriteError(oCResult, "ERROR: Something went wrong in GoLive \r\n");
                                }
                            }
                        }
                        else if (sTransactionCode == "ACSI")
                        {
                            oCResult = AddTransaction(iNewPolStatus, iTransTypeCode, sTransactionCode, refTypeID, sSessionID, sCurrentGUID);
                            if (!oCResult.bOK && oCResult.oResult == null)
                            {
                                WriteError(oCResult, "ERROR: Something went wrong in GoLive \r\n");
                            }
                        }
                        else if (sTransactionCode != "ADDON")
                        {
                            oCResult = AddTransaction(iNewPolStatus, iTransTypeCode, sTransactionCode, refTypeID, sSessionID, sCurrentGUID);
                            if (!oCResult.bOK && oCResult.oResult == null)
                            {
                                WriteError(oCResult, "ERROR: Something went wrong in GoLive \r\n");
                            }
                        }
                    } // end for bLocalNoEDI

                    else
                    {
                        //oACCTransTypeClick = new CResult();
                        //oACCTransTypeClick = GetACCTransType(sTransCode, 0, "");
                        if (OTransType != null)
                        {
                            WriteError(oCResult, "ERROR: Cannot load Transaction Type for Policy Calculation: " + oTransaction.AttributeI("FKiACPolicyID").iValue + " and Product :" + FKiProductID + "  \r\n");
                        }
                        else
                        {
                            //trans type loaded
                            if (OTransType != null && OTransType.Attributes.Count() > 0)
                            {
                                sNameACTT = OTransType.AttributeI("sName").sValue;// GetAttributeDetails(ACCTransType.Attributes, "sName");
                            }
                        }
                        if (rPremium < 0)
                        {
                            if (OTransType != null && OTransType.Attributes.Count() > 0)
                            {
                                sOpposingCode = OTransType.AttributeI("sOpposingTransCode").sValue;//GetAttributeDetails(ACCTransType.Attributes, "sOpposingTransCode");
                                if (!string.IsNullOrEmpty(sOpposingCode))
                                {
                                    var oACCTransTypeOpClick = new CResult();
                                    oACCTransTypeOpClick = GetACCTransType("", 0, sOpposingCode);
                                    var ACCTransTypeOp = (Dictionary<string, XIIBO>)oACCTransTypeOpClick.oResult;
                                    if (!oACCTransTypeOpClick.bOK && oACCTransTypeOpClick.oResult == null)
                                    {
                                        WriteError(oCResult, "ERROR: Cannot load Transaction Type for Policy Calculation: " + oTransaction.AttributeI("FKiACPolicyID").iValue + " and Product :" + FKiProductID + "  \r\n");
                                    }
                                    else
                                    {
                                        //trans type loaded
                                        oACCTransTypeClick = oACCTransTypeOpClick;
                                        var ACCTransTypeFinal = ((Dictionary<string, XIIBO>)oACCTransTypeClick.oResult).Values.FirstOrDefault();
                                        if (ACCTransTypeFinal != null && ACCTransTypeFinal.Attributes.Count() > 0)
                                        {
                                            FKiACTransTypeID = ACCTransTypeFinal.AttributeI("id").iValue;
                                            iTransType = ACCTransTypeFinal.AttributeI("iType").iValue;
                                            rPremium = rPremium * -1;
                                            rAdmin = rAdmin * -1;
                                            rInsurerCharge = rInsurerCharge * -1;
                                            sTransCode = sOpposingCode;
                                        }
                                    }
                                }
                            }
                        }
                        XIIBO oboiAACT = new XIIBO();
                        XIDXI oxidAACT = new XIDXI();
                        XIDBO obodAACT = (XIDBO)oxidAACT.Get_BODefinition("ACTransaction_T").oResult;
                        oboiAACT.BOD = obodAACT;
                        oboiAACT.SetAttribute("sChequeNo", sChequeNo.ToString());// ChequeNo required, Didnt find the source
                        oboiAACT.SetAttribute("sReference", sReference.ToString());//sReference didnt find the source
                        //oboiAACT.SetAttribute("FKsWhoID", userinfo.sUserName);//UserID from Session or from quickContext
                        oboiAACT.SetAttribute("sNotes", sNotes);
                        oboiAACT.SetAttribute("FKiPurchaseID", FKiPurchaseID.ToString()); //FKiPurchaseID, didnt find the source
                        oboiAACT.SetAttribute("FKiAddOnID", FKiAddOnID.ToString());
                        oboiAACT.SetAttribute("FKiACPolicyID", FKiACPolicyID.ToString());
                        oboiAACT.SetAttribute("FKiBrokerID", FKiBrokerID.ToString());
                        oboiAACT.SetAttribute("FKiProductID", FKiProductID.ToString());
                        var oTransTypeID = GetTransTypeID(sTransactionCode);
                        var TransTypeID = (XIIBO)oTransTypeID.oResult;
                        CRECTransTypeID = TransTypeID.AttributeI("ID").iValue;
                        oboiAACT.SetAttribute("FKiACTransTypeID", CRECTransTypeID.ToString());
                        oboiAACT.SetAttribute("iPostedFrom", iPostedFrom.ToString());
                        oboiAACT.SetAttribute("FKiProductAddOnID", FKiProductAddOnID.ToString());//FKiProductAddOnID, didnt find the source
                        oboiAACT.SetAttribute("sName", sNameACTT);
                        //oboiAACT.SetAttribute("XICreatedWhen", DateTime.Now.ToString());  // Assumed to be current date
                        //oboiAACT.SetAttribute("XIUpdatedWhen", DateTime.Now.ToString()); // Assumed to be current date
                        oboiAACT.SetAttribute("dWhen", dWhen + " " + DateTime.Now.ToString(XIConstant.Time_Format));   // Assumed to be current date 
                        oboiAACT.SetAttribute("tCoverStart", tCoverStart);
                        oboiAACT.SetAttribute("dCoverStart", dCoverStart);
                        //oboiAACT.SetAttribute("XIUpdatedBy", userinfo.sUserName);
                        oboiAACT.SetAttribute("sTransCode", sTransCode);
                        oboiAACT.SetAttribute("FKiClientID", FKiCustomerID.ToString());
                        oboiAACT.SetAttribute("FKiQuoteID", FKiQuoteID.ToString());
                        oboiAACT.SetAttribute("FKiSupplierID", FKiSupplierID.ToString());
                        //oboiAACT.SetAttribute("zCommission",zCommission.ToString());
                        oboiAACT.SetAttribute("iCommissionType", iCommissionType.ToString());
                        oboiAACT.SetAttribute("FKiEnterpriseID", FKiEnterpriseID.ToString());
                        oboiAACT.SetAttribute("iAllocPriority", 10.ToString());
                        oboiAACT.SetAttribute("refAccountCategory", refAccountCategory.ToString());
                        oboiAACT.SetAttribute("iStatus", iStatus.ToString());
                        oboiAACT.SetAttribute(XIConstant.Key_XIDeleted, "0");
                        oboiAACT.SetAttribute("rBalance", oTransaction.AttributeI("zBaseValue").doValue.ToString());
                        oboiAACT.SetAttribute("iProcessStatus", "0");
                        oboiAACT.SetAttribute("zBaseValue", oTransaction.AttributeI("zBaseValue").doValue.ToString());
                        oboiAACT.SetAttribute("rAllocatedAgainst", "0");  // Set Default value, need extra info
                        oboiAACT.SetAttribute("rAllocatedToByOthers", "0");// Set Default value, need extra info
                        oboiAACT.SetAttribute("iStatutory", "0");        // Set Default value, need extra info
                        oboiAACT.SetAttribute("iRiskTrnasfer", "0");     // Set Default value, need extra info
                        oboiAACT.SetAttribute("iEDIStatus", "0");        // Set Default value, need extra info
                        oboiAACT.SetAttribute("iPaymentType", iPaymentType.ToString());      // Set Default value, need extra info
                        oboiAACT.SetAttribute("bBusinessWritten", "0");  // Set Default value, need extra info
                        oboiAACT.SetAttribute("iSystemType", "0");       // Set Default value, need extra info
                        oboiAACT.SetAttribute("iCodeType", "0");         // Set Default value, need extra info
                        oboiAACT.SetAttribute("iSystem", "0");           // Set Default value, need extra info
                        oboiAACT.SetAttribute("FKsPolicyNo", FKsPolicyNo);
                        var oProductAddon = oIXI.BOI("ProductAddon_T", FKiProductID.ToString());
                        if (oProductAddon != null && oProductAddon.Attributes.Count() > 0)
                        {
                            sAddOnID = oProductAddon.AttributeI("refAddon").sValue;//GetAttributeDetails(oProductAddon.Attributes, "refAddon");
                        }
                        var oCUseProduct = oProduct;
                        if (iType == 10 || iType == 40 || !string.IsNullOrEmpty(sAddOnID))
                        {
                            if (!string.IsNullOrEmpty(sCommissionOverride))
                            {
                                oboiAACT.SetAttribute("zCommission", sCommissionOverride);
                            }
                            else if (oProductAddon == null)
                            {
                                var oCUseProductoverride = Get_ProductVersion(dEffectiveFrom);
                                if (oCUseProductoverride.bOK && oCUseProductoverride.oResult != null)
                                {
                                    var oCUseProductoresult = (Dictionary<string, XIIBO>)oCUseProductoverride.oResult;
                                    oCUseProduct = oCUseProductoresult.Values.FirstOrDefault();
                                }
                                if (oCUseProduct != null)
                                {
                                    oboiAACT.SetAttribute("zCommission", oCUseProduct.Attributes["zCommission"].sValue);
                                    oboiAACT.SetAttribute("iCommissionType", oCUseProduct.Attributes["iCommissionType"].sValue);
                                }
                                else
                                {
                                    oboiAACT.SetAttribute("zCommission", oProductAddon.Attributes["zCommission"].sValue);
                                    oboiAACT.SetAttribute("iCommissionType", oProductAddon.Attributes["iCommissionType"].sValue);
                                }
                                oboiAACT.SetAttribute("iSystemType", "100");

                            }
                        }
                        else
                        {
                            oboiAACT.SetAttribute("zCommission", "0");
                            if (oProductAddon == null)
                            {
                                var oCUseProductoverride = Get_ProductVersion(dEffectiveFrom);
                                if (oCUseProductoverride.bOK && oCUseProductoverride.oResult != null)
                                {
                                    var oCUseProductoresult = (Dictionary<string, XIIBO>)oCUseProductoverride.oResult;
                                    oCUseProduct = oCUseProductoresult.Values.FirstOrDefault();
                                    oboiAACT.SetAttribute("iCommissionType", oCUseProduct.Attributes["iCommissionType"].sValue);
                                }
                                else
                                {
                                    oboiAACT.SetAttribute("iCommissionType", oProductAddon.Attributes["iCommissionType"].sValue);
                                }
                                oboiAACT.SetAttribute("iSystemType", 10.ToString());
                            }
                        }
                        oboiAACT.SetAttribute("zDefaultAdmin", "0");
                        oboiAACT.SetAttribute("zDefaultDeposit", "0");
                        if (oProductAddon != null)
                        {
                            oboiAACT.SetAttribute("iAdminType", oCUseProduct.Attributes["iAdminType"].sValue);
                            oboiAACT.SetAttribute("iDepositType", oCUseProduct.Attributes["iDepositType"].sValue);
                            oboiAACT.SetAttribute("iTaxType", oCUseProduct.Attributes["iTaxType"].sValue);
                        }
                        else
                        {
                            oboiAACT.SetAttribute("iAdminType", oProductAddon.Attributes["iAdminType"].sValue);
                            oboiAACT.SetAttribute("iDepositType", oProductAddon.Attributes["iDepositType"].sValue);
                            oboiAACT.SetAttribute("iTaxType", oProductAddon.Attributes["iTaxType"].sValue);
                        }
                        oboiAACT.SetAttribute("zBaseValue", rPremium.ToString());// zBase Value is assigned as rPremium not sure
                        oboiAACT.SetAttribute("rAdmin", rAdmin.ToString());
                        oboiAACT.SetAttribute("iDays", iDays.ToString());
                        oboiAACT.SetAttribute("refTypeID", refTypeID.ToString());
                        oboiAACT.SetAttribute("rTax", "0");//Not implemented in refered code by as it is updated in policy, to avoid exception initialized to 0 
                        if (iPaymentType != 0)
                        {
                            oboiAACT.SetAttribute("iPaymentType", iPaymentType.ToString());
                        }
                        oboiAACT.iUserID = Convert.ToInt32(FKiUserID);
                        var TransactionInsert = oboiAACT.Save(oboiAACT);//SaveBO
                        var BOITransactionInsert = (XIIBO)TransactionInsert.oResult;
                        if (!TransactionInsert.bOK && TransactionInsert.oResult == null)
                        {
                            WriteError(oCResult, "ERROR: Transaction Insert Failed \r\n");
                        }
                        else
                        {
                            oCache.Set_ParamVal(sSessionID, sCurrentGUID, null, "{XIP|ACTransaction_T.id}", BOITransactionInsert.AttributeI("id").sValue, null, null);
                            Dictionary<string, string> dictparamas = new Dictionary<string, string>();
                            dictparamas.Add("FKiACTransTypeID", BOITransactionInsert.AttributeI("FKiACTransTypeID").sValue);
                            dictparamas.Add("zBaseValue", BOITransactionInsert.AttributeI("zBaseValue").sValue);
                            dictparamas.Add("rComOverride", rComOverride.ToString());
                            dictparamas.Add("zCommission", BOITransactionInsert.AttributeI("zCommission").sValue);
                            dictparamas.Add("sTransCode", BOITransactionInsert.AttributeI("sTransCode").sValue);
                            dictparamas.Add("rPremium", BOITransactionInsert.AttributeI("zBaseValue").sValue);
                            dictparamas.Add("FKiEnterpriseID", BOITransactionInsert.AttributeI("id").sValue);
                            dictparamas.Add("FKiTransactionID", BOITransactionInsert.AttributeI("id").sValue);

                            CResult JournalEntries = new CResult();
                            if (BOITransactionInsert.AttributeI("FKiACPolicyID").iValue > 0)
                            {
                                JournalEntries = JournalEntriesProcessing(dictparamas);
                            }
                            else
                            {
                                JournalEntries = JournalEntriesProcessingForNonPolicy(dictparamas);
                            }
                            if (!JournalEntries.bOK && JournalEntries.oResult == null)
                            {
                                WriteError(oCResult, "ERROR: Somthing Went Wrong in Inserting Journal Entries \r\n");
                            }
                            //Admin Transaction
                            oboiAACT = new XIIBO();
                            oxidAACT = new XIDXI();
                            //obodAACT = (XIDBO)oxidAACT.Get_BODefinition("ACTransaction_T").oResult;
                            oboiAACT.BOD = obodAACT;
                            oboiAACT.SetAttribute("FKiACPolicyID", FKiACPolicyID.ToString());
                            oboiAACT.SetAttribute("FKiEDITransTypeID", FKiEDITransTypeID.ToString());
                            oboiAACT.SetAttribute("iAllocPriority", 10.ToString());
                            oboiAACT.SetAttribute("FKiBrokerID", FKiBrokerID.ToString());
                            oboiAACT.SetAttribute("sNotes", "");//oNewTransaction.iBO_SetAttr "sNotes", oMyPolicy.GetAttr("sChangeSummary")
                            oboiAACT.SetAttribute("zBaseValue", rAdmin.ToString());
                            oboiAACT.SetAttribute("iDays", iDays.ToString());
                            oboiAACT.SetAttribute("sPolTransCode", sTransactionCode.ToString());
                            oboiAACT.SetAttribute("iSystemType", 200.ToString());
                            oboiAACT.SetAttribute("refTypeID", refTypeID.ToString());
                            oboiAACT.SetAttribute("refAnalysis1", refAnalysis1);

                            oboiAACT.SetAttribute("sName", "Admin Charge");   //hard coded
                            oboiAACT.SetAttribute("sTransCode", "ADMIN");
                            oboiAACT.SetAttribute("iStatus", Convert.ToInt32(EnumTransactions.Live).ToString());
                            oboiAACT.SetAttribute(XIConstant.Key_XIDeleted, "0");

                            oWhere.Add(new CNV { sName = "sCode", sValue = "ADMIN" });
                            oData = oIXI.BOI("ACTransType_T", null, null, oWhere).Structure("TRANTYPE").XILoad();
                            var TransTypesID = oData.oSubStructureI("ACTransType_T").oBOIList.FirstOrDefault();
                            AdminTransTypeID = TransTypesID.AttributeI("ID").iValue;
                            iTransType = TransTypesID.AttributeI("iType").iValue;
                            oboiAACT.SetAttribute("FKiACTransTypeID", AdminTransTypeID.ToString());
                            oboiAACT.SetAttribute("FKiQuoteID", FKiQuoteID.ToString());
                            oboiAACT.SetAttribute("FKiClientID", FKiCustomerID.ToString());
                            oboiAACT.SetAttribute("FKiSupplierID", FKiSupplierID.ToString());
                            oboiAACT.SetAttribute("FKiProductID", FKiProductID.ToString());
                            oboiAACT.SetAttribute("FKiBrokerID", FKiBrokerID.ToString());
                            oboiAACT.SetAttribute("FKiEnterpriseID", FKiEnterpriseID.ToString());
                            oboiAACT.SetAttribute("iAdminType", iAdminType.ToString());
                            oboiAACT.SetAttribute("iDepositType", iDepositType.ToString());
                            oboiAACT.SetAttribute("sPolTransCode", sTransactionCode.ToString());
                            oboiAACT.SetAttribute("refAccountCategory", refAccountCategory.ToString());
                            oboiAACT.SetAttribute("FKiACPolicyAllocID", FKiACPolicyID.ToString());
                            //oboiAACT.SetAttribute("XICreatedWhen", DateTime.Now.ToString());   // Assumed to be current date
                            //oboiAACT.SetAttribute("XIUpdatedWhen", DateTime.Now.ToString());  // Assumed to be current date
                            oboiAACT.SetAttribute("dWhen", dWhen + " " + DateTime.Now.ToString(XIConstant.Time_Format));   // Assumed to be current date 
                            oboiAACT.SetAttribute("tCoverStart", tCoverStart);   // Assumed to be current time 
                            oboiAACT.SetAttribute("dCoverStart", dCoverStart);
                            oboiAACT.SetAttribute("FKsPolicyNo", FKsPolicyNo);
                            oboiAACT.iUserID = Convert.ToInt32(FKiUserID);
                            var AdminTransactionInsert = oboiAACT.Save(oboiAACT);
                            var BOIAdminTransactionInsert = (XIIBO)AdminTransactionInsert.oResult;
                            if (!AdminTransactionInsert.bOK && AdminTransactionInsert.oResult == null)
                            {
                                WriteError(oCResult, "ERROR: Transaction Insert Failed \r\n");
                            }
                            else
                            {
                                dictparamas = new Dictionary<string, string>();
                                dictparamas.Add("FKiACTransTypeID", BOIAdminTransactionInsert.AttributeI("FKiACTransTypeID").sValue);
                                dictparamas.Add("zBaseValue", BOIAdminTransactionInsert.AttributeI("zBaseValue").sValue);
                                dictparamas.Add("rComOverride", rComOverride.ToString());
                                dictparamas.Add("zCommission", 0.ToString());
                                dictparamas.Add("sTransCode", BOIAdminTransactionInsert.AttributeI("sTransCode").sValue);
                                dictparamas.Add("FKiACPolicyID", BOIAdminTransactionInsert.AttributeI("FKiACPolicyID").sValue);
                                dictparamas.Add("rPremium", BOIAdminTransactionInsert.AttributeI("zBaseValue").sValue);
                                dictparamas.Add("FKiEnterpriseID", BOIAdminTransactionInsert.AttributeI("id").sValue);
                                dictparamas.Add("FKiTransactionID", BOIAdminTransactionInsert.AttributeI("id").sValue);

                                JournalEntries = new CResult();
                                if (BOITransactionInsert.AttributeI("FKiACPolicyID").iValue > 0)
                                {
                                    JournalEntries = JournalEntriesProcessing(dictparamas);
                                }
                                else
                                {
                                    JournalEntries = JournalEntriesProcessingForNonPolicy(dictparamas);
                                }


                                if (!JournalEntries.bOK && JournalEntries.oResult == null)
                                {
                                    WriteError(oCResult, "ERROR: Something Went Wrong in Inserting Journal Entries \r\n");
                                }
                            }
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            oCResult.oResult = AdminTransactionInsert;

                            //Update Policy
                            //XIIBO oboiPol = new XIIBO();
                            //XIDXI oxidPol = new XIDXI();
                            //XIDBO obodPol = (XIDBO)oxidPol.Get_BODefinition("ACPolicy_T").oResult;
                            //oboiPol.BOD = obodPol;
                            //oboiPol.SetAttribute("id", FKiACPolicyID.ToString());
                            //oboiPol.SetAttribute("rIPT", TransactionInsert.Attributes["rTax"].sValue);
                            //var PolicyUpdate = oboiPol.Save(oboiPol);
                            //if (!PolicyUpdate.oCResult.bOK && PolicyUpdate.oCResult.oResult == null)
                            //{
                            //    WriteError(oCResult, "ERROR: Policy Insert Failed \r\n");
                            //}
                        }
                    } //end for else bLocalNoEDI
                    if (!string.IsNullOrEmpty(FKiACPolicyID.ToString()) && FKiACPolicyID != 0)
                    {
                        Update_PolicyBalance(FKiACPolicyID, 10);
                        Update_PolicyBalance(FKiACPolicyID, 10, 20);
                    }
                }
                else
                {
                    WriteError(oCResult, "ERROR: If you pay 0/below your transaction is Invalid. \r\n");
                }
            }
            catch (Exception ex)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                oCResult.sMessage = "ERROR: " + ex.Message.ToString() + "\r\n PolicyID:" + oTransaction.AttributeI("FKiACPolicyID").iValue + ": sAnalysis :" + sAnalysis;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oCResult.sMessage = "ERROR: " + ex.Message.ToString() + "\r\n PolicyID:" + oTransaction.AttributeI("FKiACPolicyID").iValue + ": sAnalysis :" + sAnalysis;
                oIB.SaveErrortoDB(oCResult);
            }
            if (string.IsNullOrEmpty(TXInitiation.sTXBeginAt))
                TXInitiation.TXCommitRollback(oCResult);
            return oCResult;
        }
        #endregion
        #region EDI
        public CResult GetEDIType(int FKiEDITypeID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            XIInstanceBase oIB = new XIInstanceBase();
            try
            {
                XIIXI oIXI = new XIIXI();
                Dictionary<string, object> Params = new Dictionary<string, object>();
                Params["Name"] = "EDITransactionType_T";
                XIInfraScript oScript = new XIInfraScript();
                //Getting Product table database
                XIDXI oXID = new XIDXI();

                var oEDITransactionTypeClick = new Dictionary<string, XIIBO>();
                var oBOD = ((XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "EDITransactionType_T", null));
                var DataSource = oXID.GetBODataSource(oBOD.iDataSourceXIGUID.ToString(), oBOD.FKiApplicationID);
                XID1Click EDITransactionType1Click = new XID1Click();
                var EDITransactionTypeQuery = "select * from EDITransactionType_T where id = " + FKiEDITypeID;
                EDITransactionType1Click.sConnectionString = DataSource;
                EDITransactionType1Click.Query = EDITransactionTypeQuery;
                EDITransactionType1Click.Name = "EDITransactionType_T";
                oEDITransactionTypeClick = EDITransactionType1Click.OneClick_Execute();
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oEDITransactionTypeClick;
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
        #endregion
        #region Post Transactions_PolicyTX
        public CResult TransactionPopup(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            try
            {
                oCResult.sClassName = oCResult.Get_Class();
                oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
                XIIXI oXI = new XIIXI();
                XIInfraCache oCache = new XIInfraCache();
                string sSessionID = oParams.Where(t => t.sName == "sSessionID").Select(f => f.sValue).FirstOrDefault();
                string sCurrentGUID = oParams.Where(t => t.sName == "sGUID").Select(f => f.sValue).FirstOrDefault();
                XICacheInstance sCurParams = oCache.GetAllParamsUnderGUID(sSessionID, sCurrentGUID, null);
                var iQSIID = oParams.Where(t => t.sName == "iQSIID").Select(f => f.sValue).FirstOrDefault();
                int PolicyID = oParams.Where(t => t.sName == "PolicyID").Select(f => Convert.ToInt32(f.sValue)).FirstOrDefault();
                //FKsUserID = GetUser(oParams);
                // For Non-Policy
                //string FromAccount = ""; string FromOverrideType = ""; string ToAccount = ""; string ToOverrideType = "";
                //if (PolicyID == 0)
                //{
                //    FromAccount = sCurParams.NInstance("FromAccount").nSubParams.Where(x => x.sName == "iInstanceID").Select(x => x.sValue).FirstOrDefault();
                //    FromOverrideType = sCurParams.NInstance("FromAccount").nSubParams.Where(x => x.sName == "iOverrideType").Select(x => x.sValue).FirstOrDefault();
                //    ToAccount = sCurParams.NInstance("ToAccount").nSubParams.Where(x => x.sName == "iInstanceID").Select(x => x.sValue).FirstOrDefault();
                //    ToOverrideType = sCurParams.NInstance("ToAccount").nSubParams.Where(x => x.sName == "iOverrideType").Select(x => x.sValue).FirstOrDefault();
                //}
                //var oQSInstance = oXI.GetQuestionSetInstanceByID(0, iQSIID, null, 0, 0, "");
                var oQSInstance = oCache.Get_QuestionSetCache("QuestionSetCache", sCurrentGUID, iQSIID.ToString());

                // Perform Trasactions here 
                XIIBO oTransaction = new XIIBO();
                XIIXI oIXI = new XIIXI();
                //if (FromOverrideType.ToLower() == "Client Control Account".ToLower())
                //{
                //    oParams = new List<CNV>();
                //    //oParams.Add(new CNV { sName = "FKiACAccountID", sValue = FromAccount.ToString() });
                //    var CustomerAccount = oIXI.BOI("Customer_T", null, null, oParams);
                //    oTransaction.SetAttribute("FKiClientID", CustomerAccount == null ? 0.ToString() : CustomerAccount.AttributeI("id").sValue);
                //}
                //else if (FromOverrideType.ToLower() == "Insurer Control Account".ToLower() || FromOverrideType.ToLower() == "Premium Finance Control".ToLower())
                //{
                //    oParams = new List<CNV>();
                //    //oParams.Add(new CNV { sName = "FKiACAccountID", sValue = FromAccount.ToString() });
                //    var SupplierAccount = oIXI.BOI("Supplier_T", null, null, oParams);
                //    oTransaction.SetAttribute("FKiSupplierID", SupplierAccount == null ? 0.ToString() : SupplierAccount.AttributeI("id").sValue);
                //}
                oTransaction.SetAttribute("FKiACPolicyID", PolicyID.ToString());
                if (oQSInstance.Steps.Last().Value.Sections.FirstOrDefault().Value.FormValues.Count() > 0)
                {
                    foreach (var item in oQSInstance.Steps.Last().Value.Sections.FirstOrDefault().Value.FormValues)
                    {
                        oTransaction.SetAttribute(item.sName, item.sValue);
                    }
                }
                //oTransaction.SetAttribute("zBaseValue", oQSInstance.GetXIIValue("rAmountTX").sValue);
                //oTransaction.SetAttribute("dWhen", oQSInstance.GetXIIValue("dWhen").sValue);
                //oTransaction.SetAttribute("sTransCode", oQSInstance.GetXIIValue("sTransactionTypeTX").sResolvedValue);
                //oTransaction.SetAttribute("sNotes", oQSInstance.GetXIIValue("sNotes").sValue);
                //oTransaction.SetAttribute("XICreatedBy", oQSInstance.GetXIIValue("Who").sValue);
                //oTransaction.SetAttribute("refAccountCategory", oQSInstance.GetXIIValue("AccountCategory").sValue);
                //oTransaction.SetAttribute("FromAccount", FromAccount);
                //oTransaction.SetAttribute("ToAccount", ToAccount);
                oCResult = PostTransaction(oTransaction, oParams.Where(t => t.sName == "sDataBase").Select(f => f.sValue).FirstOrDefault(), sSessionID, sCurrentGUID, false);
                Update_PolicyBalance(PolicyID, 10, 20);
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                oCResult.sMessage = "ERROR: " + ex.Message.ToString();
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oIB.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        // For Non-Policy
        public CResult NonPolicyTransactions(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            try
            {
                oCResult.sClassName = oCResult.Get_Class();
                oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
                XIIXI oXI = new XIIXI();
                XIInfraCache oCache = new XIInfraCache();
                string sSessionID = oParams.Where(t => t.sName == "sSessionID").Select(f => f.sValue).FirstOrDefault();
                string sCurrentGUID = oParams.Where(t => t.sName == "sGUID").Select(f => f.sValue).FirstOrDefault();
                XICacheInstance sCurParams = oCache.GetAllParamsUnderGUID(sSessionID, sCurrentGUID, null);
                var iQSIID = oParams.Where(t => t.sName == "iQSIID").Select(f => f.sValue).FirstOrDefault();

                var oQSInstance = oXI.GetQuestionSetInstanceByID("0", iQSIID.ToString(), null, 0, 0, "");
                //FKsUserID = GetUser(oParams);
                // Perform Trasactions here 
                XIIBO oTransaction = new XIIBO();
                XIIXI oIXI = new XIIXI();
                oTransaction.SetAttribute("zBaseValue", oQSInstance.GetXIIValue("rAmountTX").sValue);
                oTransaction.SetAttribute("sTransCode", oQSInstance.GetXIIValue("sTransactionTypeTX").sResolvedValue);
                oTransaction.SetAttribute("dWhen", oQSInstance.GetXIIValue("dWhen").sValue);
                oTransaction.SetAttribute("sNotes", oQSInstance.GetXIIValue("sNotes").sValue);
                oTransaction.SetAttribute("FKsWhoID", oQSInstance.GetXIIValue("Who").sValue);
                oTransaction.SetAttribute("refAccountCategory", oQSInstance.GetXIIValue("AccountCategory").sValue);
                // TRANSACTION SCOPE HERE
                oCResult = PostTransaction(oTransaction, oParams.Where(t => t.sName == "sDataBase").Select(f => f.sValue).FirstOrDefault(), sSessionID, sCurrentGUID, false);
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                oCResult.sMessage = "ERROR: " + ex.Message.ToString();
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oIB.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
        public static string GetUser(List<CNV> oParams)
        {
            XIInfraUsers oUser = new XIInfraUsers();
            string User = null;
            oUser.UserID = Convert.ToInt32(oParams.Where(t => t.sName == "iUserID").Select(f => f.sValue).FirstOrDefault());
            oUser = (XIInfraUsers)oUser.Get_UserDetails(oParams.Where(t => t.sName == "sDataBase").Select(f => f.sValue).FirstOrDefault()).oResult;
            if (oUser != null)
            {
                if (oUser.Role != null)
                {
                    User = oUser.sFirstName + " " + oUser.sLastName;
                }
            }
            return User;
        }
        public CResult ReverseTransaction(List<CNV> oParams)//int iReversalType, string TransType = "REVB", int PolicyID = 100)
        {
            TXInitiation.sTXBeginAt = "Reverse";
            CResult oCResult = new CResult();
            oCResult.sClassName = oCResult.Get_Class();
            oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            const string ROUTINE_NAME = "[Reverse]";
            string rNewAmount;
            CResult oCR = new CResult();
            XIInstanceBase oIB = new XIInstanceBase();
            XIIXI oXI = new XIIXI();
            XIInfraCache oCache = new XIInfraCache();
            string sSessionID = oParams.Where(t => t.sName == "sSessionID").Select(f => f.sValue).FirstOrDefault();
            string sCurrentGUID = oParams.Where(t => t.sName == "sGUID").Select(f => f.sValue).FirstOrDefault();
            XICacheInstance sCurParams = oCache.GetAllParamsUnderGUID(sSessionID, sCurrentGUID, null);
            var iQSIID = oParams.Where(t => t.sName == "iQSIID").Select(f => f.sValue).FirstOrDefault();
            int iReversalType = oParams.Where(t => t.sName == "{XIP|iReversalType}").Select(f => Convert.ToInt32(f.sValue)).FirstOrDefault();
            //FKsUserID = GetUser(oParams);
            var oQSInstance = oXI.GetQSXIValuesByQSIID(iQSIID.ToString());
            try
            {
                XIIBO BOI = oXI.BOI("ACTransaction_T", oParams.Where(t => t.sName == "{XIP|ACTransaction_T.id}").Select(f => f.sValue).FirstOrDefault().ToString());
                XIIBO oboi = new XIIBO();
                XIDXI oxid = new XIDXI();
                CResult oCresult = new CResult();
                oboi = BOI.GetCopy();
                oboi.Exclude("id,dWhen,rBalance,rAllocatedAgainst,rAllocatedToByOthers,XIGUID");
                oboi.Attributes.Values.ToList().ForEach(x => x.bDirty = true);
                oboi.SetAttribute("rComOverride", (BOI.AttributeI("rCommission").doValue * -1).ToString());
                oboi.SetAttribute("zDefaultAdmin", (BOI.AttributeI("zDefaultAdmin").doValue * -1).ToString());
                oboi.SetAttribute("zDefaultDeposit", (BOI.AttributeI("zDefaultDeposit").doValue * -1).ToString());
                oboi.SetAttribute("rNetToSupplier", (BOI.AttributeI("rNetToSupplier").doValue * -1).ToString());
                oboi.SetAttribute("rAdmin", (BOI.AttributeI("rAdmin").doValue * -1).ToString());
                oboi.SetAttribute("rDeposit", (BOI.AttributeI("rDeposit").doValue * -1).ToString());
                oboi.SetAttribute("rTax", (BOI.AttributeI("rTax").doValue * -1).ToString());
                oboi.SetAttribute("zBaseValue", (BOI.AttributeI("zBaseValue").doValue * -1).ToString());
                oboi.SetAttribute("rBalance", (BOI.AttributeI("rBalance").doValue * -1).ToString());
                oboi.SetAttribute("iProcessStatus", 100.ToString());
                oboi.SetAttribute("iStatus", Convert.ToInt32(EnumTransactions.CorrectionTransaction).ToString());
                oboi.SetAttribute("sNotes", oQSInstance.GetXIIValue("sNotes").sValue);
                oboi.SetAttribute(XIConstant.Key_XICrtdBy, oQSInstance.GetXIIValue("Who").sDerivedValue);
                oboi.SetAttribute(XIConstant.Key_XIUpdtdBy, oQSInstance.GetXIIValue("Who").sDerivedValue);
                oboi.SetAttribute("dWhen", DateTime.Now.ToString(XIConstant.Date_Format) + " " + DateTime.Now.ToString(XIConstant.Time_Format));
                //oCResult = PostTransaction(oboi, oParams.Where(t => t.sName == "sDataBase").Select(f => f.sValue).FirstOrDefault(), sSessionID, sCurrentGUID, false);
                oboi.iUserID = Convert.ToInt32(FKiUserID);
                var CorrectionTransaction = oboi.Save(oboi);
                var result = (XIIBO)CorrectionTransaction.oResult;
                if (!CorrectionTransaction.bOK && CorrectionTransaction.oResult == null)
                {
                    oCResult.sMessage = "INSERT of Correcting Transaction FAILED - id=:" + result.AttributeI("id").sValue;//Transaction Id
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oCResult.sMessage = ROUTINE_NAME + ", INSERT of Correcting Transaction FAILED - id=:" + result.AttributeI("id").sValue;//Transaction Id
                    oIB.SaveErrortoDB(oCResult);
                }

                // NO: PRE-PERSIST SHOULD TAKE CARE OF ALL THIS:
                // oNewTransaction.iBO_SetAttr "sAnalysis", i.zX.strValue("REVERSAL OF TRANSACTION ID= " & i.GetAttr("id").strValue & vbCrLf & "DETAILS:" & vbCrLf & oNewTransaction.iBO_getAttr("sAnalysis").strValue)
                // For Each oJE In oColJEs
                //    'WARNING - a lot goes on in trans pre-persist - i think you should create a new one and post it, not copy each line
                // Next oJE
                BOI.SetAttribute("iStatus", Convert.ToInt32(EnumTransactions.Corrected).ToString());
                //BOI.TransactionIntiation = TXInitiation;
                oCResult = BOI.Save(BOI);

                if (!oCResult.bOK && oCResult.oResult == null)
                {
                    oCResult.sMessage = "Update of iStatus (corrected) for transaction failed Transaction id=:" + BOI.AttributeI("id");//Transaction Id
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oCResult.sMessage = ROUTINE_NAME + ", Update of iStatus (corrected) for transaction failed Transaction id=:" + BOI.AttributeI("id");//Transaction Id
                    oIB.SaveErrortoDB(oCResult);
                }
                List<XIIBO> oJEBOI = new List<XIIBO>();
                XIDXI oJEXI = new XIDXI();
                List<XIWhereParams> oWParams = new List<XIWhereParams>();
                QueryEngine oQE = new QueryEngine();
                oWParams.Add(new XIWhereParams { sField = "FKiTransactionID", sOperator = "=", sValue = oParams.Where(t => t.sName == "{XIP|ACTransaction_T.id}").Select(f => f.sValue).FirstOrDefault().ToString() });
                oQE.AddBO("ACJournalEntry_T", null, oWParams);
                oCresult = oQE.BuildQuery();
                if (oCresult.bOK && oCresult.oResult != null)
                {
                    var sSql = (string)oCresult.oResult;
                    ExecutionEngine oEE = new ExecutionEngine();
                    oEE.XIDataSource = oQE.XIDataSource;
                    oEE.sSQL = sSql;
                    var oQResult = oEE.Execute(TXInitiation);
                    if (oQResult.bOK && oQResult.oResult != null)
                    {
                        oJEBOI = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.ToList();
                    }
                }
                if (oJEBOI == null)
                {
                    oCResult.sMessage = ROUTINE_NAME + ", Unable to DB2Collection Transaction JEs ";//i.zX.trace.formatStack
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                }
                var BOD = (XIDBO)oxid.Get_BODefinition("ACJournalEntry_T").oResult;
                foreach (var item in oJEBOI)
                {
                    var cr = item.AttributeI("rCR").sValue;
                    var dr = item.AttributeI("rDR").sValue;
                    var iType = item.AttributeI("iType").sValue;
                    item.BOD = BOD;
                    item.SetAttribute("iStatus", Convert.ToInt32(EnumTransactions.Corrected).ToString());
                    item.SetAttribute(XIConstant.Key_XICrtdBy, oQSInstance.GetXIIValue("Who").sDerivedValue);
                    oboi.SetAttribute(XIConstant.Key_XIUpdtdBy, oQSInstance.GetXIIValue("Who").sDerivedValue);
                    item.TransactionIntiation = TXInitiation;
                    var res = item.Save(item);
                    if (!res.bOK && res.oResult == null)
                    {
                        oCResult.sMessage = ROUTINE_NAME + ", Update of iStatus (corrected) for transaction Journal Entry failed JE id=: " + item.AttributeI("id").ToString();
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oCResult.sMessage = ROUTINE_NAME + ", Update of iStatus (corrected) for transaction Journal Entry failed JE id=: " + item.AttributeI("id").ToString();
                        oIB.SaveErrortoDB(oCResult);
                    }
                    item.Exclude("id,XIGUID");
                    item.SetAttribute("rCR", dr);
                    item.SetAttribute("rDR", cr);
                    item.SetAttribute("iType", iType == "10" ? "20" : "10");
                    item.Attributes.Values.ToList().ForEach(x => x.bDirty = true);
                    item.SetAttribute("iStatus", Convert.ToInt32(EnumTransactions.CorrectionTransaction).ToString());
                    item.SetAttribute("FKiTransactionID", result.AttributeI("id").sValue);
                    item.iUserID = Convert.ToInt32(FKiUserID);
                    res = item.Save(item);
                    if (!res.bOK && res.oResult == null)
                    {
                        oCResult.sMessage = ROUTINE_NAME + ", Failed to insert New Journal Entry for New Transaction";
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oCResult.sMessage = ROUTINE_NAME + ",Failed to insert New Journal Entry for New Transaction";
                        oIB.SaveErrortoDB(oCResult);
                    }
                }
                // DS TO DO - cancel off any allocations in here

                if (iReversalType == 20)
                {
                    // we really want to keep this transaction in place and just change the amounts. Take a copy if it and post it as the 'corrected' transaction
                    //    it may already have
                    //   DS: Still to do - re-point any allocations to the current one. Not sure exactly how critical this is
                    // so now post the correcting transaction
                    rNewAmount = oQSInstance.GetXIIValue("rAmountTX").sValue;
                    oboi.SetAttribute("zBaseValue", rNewAmount.ToString());
                    oboi.SetAttribute("iStatus", Convert.ToInt32(EnumTransactions.ReplacementTransaction).ToString());
                    oboi.SetAttribute(XIConstant.Key_XICrtdBy, oQSInstance.GetXIIValue("Who").sDerivedValue);
                    oboi.SetAttribute(XIConstant.Key_XIUpdtdBy, oQSInstance.GetXIIValue("Who").sDerivedValue);
                    oboi.SetAttribute("rComOverride", "0");
                    oboi.SetAttribute("rNetToSupplier", "0");
                    oboi.SetAttribute("rPremium", "0");
                    oboi.SetAttribute("rBalance", "0");
                    oboi.SetAttribute("iProcessStatus", "0");

                    oCResult = PostTransaction(oboi, oParams.Where(t => t.sName == "sDataBase").Select(f => f.sValue).FirstOrDefault(), sSessionID, sCurrentGUID, false);

                    if (!oCResult.bOK && oCResult.oResult == null)
                    {
                        oCResult.sMessage = "INSERT of Correcting Transaction FAILED - id=:" + BOI.AttributeI("id"); //Transaction ID
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oCResult.sMessage = "INSERT of Correcting Transaction FAILED - id=:" + BOI.AttributeI("id"); //Transaction ID
                        oIB.SaveErrortoDB(oCResult);
                    }
                }
                if (TXInitiation.sTXBeginAt == "Reverse")
                    TXInitiation.TXCommitRollback(oCResult);
                if (BOI.AttributeI("FKiACPolicyID").sValue != null && BOI.AttributeI("FKiACPolicyID").sValue != "0")
                {
                    Update_PolicyBalance(BOI.AttributeI("FKiACPolicyID").iValue, 10);
                    Update_PolicyBalance(BOI.AttributeI("FKiACPolicyID").iValue, 10, 20);
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

        public CResult Update_PolicyBalance(int PolicyID, int iUpdateMe, int iType = 0)
        {
            CResult oCResult = new CResult();
            oCResult.sClassName = oCResult.Get_Class();
            oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            const string ROUTINE_NAME = "[Update_PolicyBalance]";
            // Warning!!! Optional parameters not supported
            XIInstanceBase oIB = new XIInstanceBase();
            int AccountNo = 0; int iCRDR = 0; int j = 0;
            double dCR = 0; double dDR = 0; double dTotal = 0;
            StringBuilder sWhereClause = new StringBuilder();

            // TODO: On Error GoTo Warning!!!: The statement is not translatable 
            // Total up all transactions and mark up the pol balance
            // DS: there will be an issue as to a few pence here and there - what to do?
            XIIXI oXII = new XIIXI();
            XIIBO oBOIPolicy = oXII.BOI("ACPolicy_T", PolicyID.ToString());
            XIIBO oBOIClient = oXII.BOI("Customer_T", oBOIPolicy.AttributeI("FKiCustomerID").sValue, "id,FKiACAccountID");
            if (oBOIClient != null)
            {
                AccountNo = oBOIClient.AttributeI("FKiACAccountID").iValue;
            }
            else
            {
                oCResult.sMessage = ROUTINE_NAME + ", Cannot derive Client from Policy for policy id = ";//Policy ID
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oCResult.sMessage = ROUTINE_NAME + ", Cannot derive Client from Policy for policy id = ";//Policy ID
                oIB.SaveErrortoDB(oCResult);
            }
            // DS: This is a total of all journals (on the basis of transactions) against this policy which are totalled here rather than chained up in order to try and minimise errors
            for (j = 1; j <= 2; j++)
            {
                // Add up the credits and then the debits
                if (j == 1)
                {
                    iCRDR = 10;
                }
                else if (j == 2)
                {
                    iCRDR = 20;
                }
                sWhereClause = new StringBuilder();
                string Journal = "[ACJournalEntry_T]"; string Transaction = "[ACTransaction_T]";
                // All this is doing is checking if the user has restricted the item that shows in results to one
                // instance
                sWhereClause.Append(" AND " + Journal + ".[FKiAccountID] =" + AccountNo.ToString());
                sWhereClause.Append(" AND " + Journal + ".[iType] =" + iCRDR.ToString());
                sWhereClause.Append(" AND " + Transaction + ".[FKiACPolicyID] =" + PolicyID.ToString());
                if (iType == 10)
                {
                    // premium finance transactions. Logic is to get all of the prem fin transactions THAT HAVE NOT BEEN RECONCILED and add them up
                    sWhereClause.Append(" AND " + Journal + ".[iTransType] = 200");
                    sWhereClause.Append(" AND " + Transaction + ".[iProcessStatus] < 20");
                    sWhereClause.Append(" AND (" + Transaction + ".[iStatus] = 0 OR " + Transaction + ".[iStatus] = 30)");
                }
                XID1Click UpdateBalance1Click = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "Update PolicyBalance", null);
                XID1Click UpdateBalance = (XID1Click)UpdateBalance1Click.Clone(UpdateBalance1Click);
                UpdateBalance.Query += sWhereClause;
                // eg: SELECT SUM (rAmount)  FROM [ACJournalEntry_T] LEFT JOIN [ACTransaction_T] ON 
                //[ACJournalEntry_T].[FKiTransactionID] = [ACTransaction_T].[id] WHERE (1 = 1)  AND ([ACJournalEntry_T].[FKiAccountID] = 528) 
                //AND ( ([ACTransaction_T].[FKiACPolicyID] = 3217) AND ([ACJournalEntry_T].[iTransType] = 200) 
                //AND [ACTransaction_T].[iProcessStatus] < 20   '[ACJournalEntry_T].[iType] = 10) AND

                // multi db switch
                // Set oRS = i.zX.Db.rs(sQry)
                var oXIBODTrans = new Dictionary<string, XIIBO>();
                oXIBODTrans = UpdateBalance.OneClick_Execute();
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = oXIBODTrans;

                if (oXIBODTrans == null)
                {
                    oCResult.sMessage = ROUTINE_NAME + "Unable to generate Policy Balance query and Unable to generate Policy Balance query for policy id = " + PolicyID;//Policy ID
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oCResult.sMessage = ROUTINE_NAME + "Unable to generate Policy Balance query and Unable to generate Policy Balance query for policy id = " + PolicyID;//Policy ID
                    oIB.SaveErrortoDB(oCResult);
                }
                dTotal = 0;
                while (oXIBODTrans.Count() > 0)
                {
                    //             oRS.rs2obj oCBO, "rAmount"
                    //             dTotal = oCBO.GetAttr("rAmount").dblValue
                    if (oXIBODTrans.Values.Select(v => v.Attributes).Count() == 0)
                    {
                        //          iResultType = 20     'DS: allow null to mean zero as this is the result of the additions  ' 10
                        dTotal = 0;
                    }
                    else
                    {
                        dTotal = oXIBODTrans.Values.Select(v => v.AttributeI("Amount").doValue).FirstOrDefault();
                        //          iResultType = 20
                    }
                    break; //Warning!!! Review that break works as 'Exit Do' as it could be in a nested instruction like switch
                           // should only be one result
                }
                if (j == 1)
                {
                    dDR = dTotal;
                }
                else if (j == 2)
                {
                    dCR = dTotal;
                }
            }
            if (iType == 10)
            {
                // premium finance transactions
                oBOIPolicy.SetAttribute("rFinanceBalance", Math.Round(dCR - dDR, 3).ToString());
                // relative to them (policy holder) - a negative amount means they still have to pay us
                if (Math.Abs(dCR - dDR) < 1)
                {
                    oBOIPolicy.SetAttribute("dAFZerodAFZero", (DateTime.Now).ToString());
                }
                if (iUpdateMe == 10)
                {
                    oBOIPolicy.iUserID = Convert.ToInt32(FKiUserID);
                    var Result = oBOIPolicy.Save(oBOIPolicy);
                    if (!Result.bOK && Result.oResult != null)
                    {
                        oCResult.sMessage = ROUTINE_NAME + " Unable to UPDATE Policy rFinanceBalance for policy id = " + PolicyID;//Policy ID
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oCResult.sMessage = ROUTINE_NAME + " Unable to UPDATE Policy rFinanceBalance for policy id =" + PolicyID;//Policy ID
                        oIB.SaveErrortoDB(oCResult);
                    }
                }
            }
            else
            {
                oBOIPolicy.SetAttribute("rPolicyBalance", Math.Round(dCR - dDR, 3).ToString());
                oBOIPolicy.SetAttribute("rBalance", Math.Round(dCR - dDR, 3).ToString());
                // relative to them (policy holder) - a negative amount means they still have to pay us
                if (Math.Abs(dCR - dDR) < 1)
                {
                    //Zero
                    oBOIPolicy.SetAttribute("iBalanceStatus", "0");
                    oBOIPolicy.SetAttribute("dADZero", (DateTime.Now).ToString());
                }
                else
                {
                    // non-zero
                    oBOIPolicy.SetAttribute("iBalanceStatus", "10");
                }
                if (iUpdateMe == 10)
                {
                    oBOIPolicy.iUserID = Convert.ToInt32(FKiUserID);
                    var Result = oBOIPolicy.Save(oBOIPolicy);
                    if (!Result.bOK && Result.oResult != null)
                    {
                        oCResult.sMessage = ROUTINE_NAME + " Unable to UPDATE Policy rPolicyBalance for policy id = " + PolicyID;//Policy ID
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oCResult.sMessage = ROUTINE_NAME + " Unable to UPDATE Policy rPolicyBalance for policy id = " + PolicyID;//Policy ID
                        oIB.SaveErrortoDB(oCResult);
                    }
                }
            }
            return oCResult;
        }
        #endregion

        #region Copy Transaction 
        public CResult CopyTransaction(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            try
            {
                oCResult.sClassName = oCResult.Get_Class();
                oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
                XIIXI oXI = new XIIXI();
                XIInstanceBase oIB = new XIInstanceBase();
                XIIBO BOI = oXI.BOI("ACTransaction_T", oParams.Where(t => t.sName == "TransactionID").Select(f => f.sValue).FirstOrDefault(), "*");
                XIIBO oboi = new XIIBO();
                oboi = BOI.GetCopy();
                oboi.Exclude("id,iStatus,XIGUID");
                oboi.SetAttribute("iStatus", "999");
                oboi.SetAttribute(XIConstant.Key_XICrtdWhn, DateTime.Now.ToString(XIConstant.Date_Format) + " " + DateTime.Now.ToString(XIConstant.Time_Format));
                oboi.SetAttribute(XIConstant.Key_XIUpdtdWhn, DateTime.Now.ToString(XIConstant.Date_Format) + " " + DateTime.Now.ToString(XIConstant.Time_Format));
                oboi.SetAttribute("dCoverStart", DateTime.Now.ToString(XIConstant.Date_Format) + " " + DateTime.Now.ToString(XIConstant.Time_Format));
                oboi.Attributes.ToList().ForEach(m => m.Value.bDirty = true);
                oboi.iUserID = Convert.ToInt32(FKiUserID);
                oCResult = oboi.Save(oboi);
                if (!oCResult.bOK && oCResult.oResult == null)
                {
                    oCResult.sMessage = "ERROR: Faild to Copy the Transaction";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oCResult.sMessage = "ERROR: Faild to Copy the Transaction";
                    oIB.SaveErrortoDB(oCResult);
                }
                else
                {
                    List<XIIBO> oBOIJournalsList = new List<XIIBO>();
                    QueryEngine oQE = new QueryEngine();
                    string sWhereCondition = "FKiTransactionID=" + oParams.Where(t => t.sName == "TransactionID").Select(f => f.sValue).FirstOrDefault() + "," + XIConstant.Key_XIDeleted + "=0";
                    var oQResult = oQE.Execute_QueryEngine("ACJournalEntry_T", "*", sWhereCondition);
                    if (oQResult.bOK && oQResult.oResult != null)
                    {
                        oBOIJournalsList = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.ToList();
                    }
                    if (oBOIJournalsList.Count() > 0)
                    {
                        var sNewTransactionID = ((XIIBO)oCResult.oResult).AttributeI("id").sValue;
                        foreach (var item in oBOIJournalsList)
                        {
                            item.Exclude("id,iStatus,FKiTransactionID,XIGUID");
                            item.SetAttribute("iStatus", "999");
                            item.SetAttribute(XIConstant.Key_XICrtdWhn, DateTime.Now.ToString(XIConstant.Date_Format) + " " + DateTime.Now.ToString(XIConstant.Time_Format));
                            item.SetAttribute(XIConstant.Key_XIUpdtdWhn, DateTime.Now.ToString(XIConstant.Date_Format) + " " + DateTime.Now.ToString(XIConstant.Time_Format));
                            item.SetAttribute("dtWhen", DateTime.Now.ToString(XIConstant.Date_Format) + " " + DateTime.Now.ToString(XIConstant.Time_Format));
                            item.SetAttribute("FKiTransactionID", sNewTransactionID);
                            item.iUserID = Convert.ToInt32(FKiUserID);
                            oCResult = item.Save(item);
                            if (!oCResult.bOK && oCResult.oResult == null)
                            {
                                oCResult.sMessage = "ERROR: Faild to Copy the Journal entry";
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oCResult.LogToFile();
                                oCResult.sMessage = "ERROR: Faild to Copy the Journal entry";
                                oIB.SaveErrortoDB(oCResult);
                            }
                        }
                    }
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                XIInstanceBase oIB = new XIInstanceBase();
                oCResult.sMessage = "ERROR: " + ex.Message.ToString();
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oIB.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
        #endregion
        #region Update_rBalance
        public Dictionary<string, double> Update_rBalance(int iAccountID)//, string pstrAttr = null)
        {
            string sADWhere = null;
            XIIBO oJE = new XIIBO();
            double rBalanceCR = 0;
            double rBalanceDR = 0;
            double rBalance = 0;
            double iPKEnt = 0;
            CResult oCResult = new CResult();
            //switch (pstrAttr)
            //{
            //    case "_rbalance":


            //      '2012.12.27 - I added this in as lynn needs unreconciled balance. not sure if this is used elsewhere. if so just create a new _attr and put this code and uncomment the above

            //      '2012.12.27 - ALERT!!! THIS WHERE CLAUSE NEEDS TO BE SPECIFIC TO WHAT TYPE OF REC THIS SHOWS FOR. THIS MAY NOT BE EASY. CODE FOR THIS IS IN COMMatrixAccounts.asp -> asp.GetRestriction

            //      '   you need the case statement. But somehow we would need to either interrogate the acc type, or else pass in the type as a param (_attr) into this BO and pick it up here

            //      'SELECT SUM([ACJournalEntry_T].[rAmount]) FROM [ACJournalEntry_T] WHERE (1 = 1) AND ([ACJournalEntry_T].[FKiAccountID] = 118 AND ([ACJournalEntry_T].[iStatus] = 0 OR [ACJournalEntry_T].[iStatus] = 30) AND [ACJournalEntry_T].[iReconcilliationType] = 10 AND [ACJournalEntry_T].[iReconciled] < 30 AND [ACJournalEntry_T].[FKiEnterpriseID] = 0 AND [ACJournalEntry_T].[iType] = 20)

            //iPKEnt = Val(i.zX.quickContext.getEntry("-pk.Ent"));

            //'2017.03.21 - removed the enterprise restriction for now as i don't think it was working
            string Select = "select sum(rAmount) from ACJournalEntry_T with(nolock) where ";
            sADWhere = "FKiAccountID=" + iAccountID + " and (iStatus=0 or iStatus=30) and iReconcilliationType=10 and iReconciled<30"; //'&FKiEnterpriseID=" & iPKEnt

            XID1Click oXI1Click = new XID1Click();
            oXI1Click.Query = Select + sADWhere + " and iType=10";
            oXI1Click.Name = "ACJournalEntry_T";
            var tRtn = oXI1Click.Execute_Query();
            if (tRtn.Rows.Count > 0)
            {
                rBalanceCR = Convert.ToDouble(tRtn.Rows[0].ItemArray[0] == DBNull.Value ? 0 : tRtn.Rows[0].ItemArray[0]);
            }
            oXI1Click = new XID1Click();
            oXI1Click.Query = Select + sADWhere + " and iType=20";
            oXI1Click.Name = "ACJournalEntry_T";
            tRtn = oXI1Click.Execute_Query();


            //'tRtn = i.zX.BOS.selectSum(oJE, "rAmount", ":dDate>=#" & Format(dDateTo, "dd mmm yyyy") & "#&dDate<=#" & Format(dDateFrom, "dd mmm yyyy") & "#" & "&FKiAccountID=" & oClient.GetAttr("FKiACAccountID").strValue & "&FKiACPolicyID=" & i.GetAttr("id").strValue & "&iType=20&iStatus=0")

            if (tRtn.Rows.Count > 0)
            {
                rBalanceDR = Convert.ToDouble(tRtn.Rows[0].ItemArray[0] == DBNull.Value ? 0 : tRtn.Rows[0].ItemArray[0]);
            }

            rBalance = rBalanceDR - rBalanceCR;

            //    case "_rbalreverse":

            //        rBalance = i.GetAttr("rBalance").dblValue * -1;

            //i.zX.BOS.SetAttr Me, pstrAttr, i.zX.dblValue(rBalance);

            //}

            XIIXI oIXI = new XIIXI();
            var oACCAccount = oIXI.BOI("ACAccount_T", iAccountID.ToString());
            oACCAccount.SetAttribute("rBalance", rBalance.ToString());
            oCResult = oACCAccount.Save(oACCAccount);
            Dictionary<string, double> oDictionary = new Dictionary<string, double>();
            oDictionary["Balance"] = Math.Round(rBalance, 2);
            return oDictionary;
        }
        public CResult Calculate_rBalance(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            try
            {
                int iAccountID = 0;
                var sAccountID = oParams.Where(m => m.sName.ToLower() == "fkiaccountid").FirstOrDefault();
                if (sAccountID != null)
                {
                    int.TryParse(sAccountID.sValue, out iAccountID);
                }
                Dictionary<string, XIIValue> XIIValues = new Dictionary<string, XIIValue>();
                var Result = Update_rBalance(iAccountID);
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = Result;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing MTAAddon" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                XIDefinitionBase oXID = new XIDefinitionBase();
                oXID.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
        #endregion
    }
}
