using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using XICore;
using XISystem;

namespace XIInfrastructure
{
    public class XIBOBuilding
    {
        public string OrderID { get; set; }
        public int iPaymentID { get; set; }
        public decimal rGrossPremium { get; set; }
        public string PolicySequenceNumber { get; set; }
        XIDefinitionBase oXID = new XIDefinitionBase();
        public CResult BuildPolicyObject()
        {
            CResult oResult = new CResult();
            try
            {
                int iACPolicyID = 0; bool bIshaving = false; string FKiProductID = string.Empty; string sPolicyStartDate = string.Empty;
                oResult.oTraceStack.Add(new CNV { sName = "Thread started executing", sValue = "Thread started executing" });
                oResult.oTraceStack.Add(new CNV { sName = "OrderID", sValue = "OrderID :" + OrderID });
                XIIXI oIXI = new XIIXI();
                XIInfraCache oCache = new XIInfraCache();
                string[] ResList = OrderID.Split('_');
                oResult.oTraceStack.Add(new CNV { sName = "QuoteID", sValue = "QuoteID :" + ResList[0] });
                //oResult.oTraceStack.Add(new CNV { sName = "sGUID", sValue = "sGUID :" + ResList[1] });
                Dictionary<string, CNV> OQSD = new Dictionary<string, CNV>(StringComparer.CurrentCultureIgnoreCase);
                int iInsatnceID = 0;
                var oBOI = oIXI.BOI("Aggregations", ResList[0]);
                if (oBOI.Attributes.ContainsKey("iStatus"))
                {
                    oBOI.Attributes["iStatus"].sValue = "10";
                    oBOI.Save(oBOI);
                    var iID = oBOI.Attributes["FKiQSInstanceID"].sValue;
                    QueryEngine oQE = new QueryEngine();
                    List<XIWhereParams> oWParams = new List<XIWhereParams>();
                    XIIXI oXII = new XIIXI();
                    List<CNV> oWhrParams = new List<CNV>();
                    oWhrParams.Add(new CNV { sName = "FKiQSInstanceID", sValue = iID });
                    oQE.AddBO("Aggregations", "", oWParams);
                    CResult oCresult = oQE.BuildQuery();
                    if (oCresult.bOK && oCresult.oResult != null)
                    {
                        var sSql = (string)oCresult.oResult;
                        ExecutionEngine oEE = new ExecutionEngine();
                        oEE.XIDataSource = oQE.XIDataSource;
                        oEE.sSQL = sSql;
                        var oQResult = oEE.Execute();
                        if (oQResult.bOK && oQResult.oResult != null)
                        {
                            var oBOIList = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.Where(m => m.Attributes[m.BOD.sPrimaryKey].sValue != ResList[0]).ToList();
                            var oBOD = oQE.QParams.FirstOrDefault().BOD;
                            oBOIList.ForEach(x => x.BOD = oBOD);
                            foreach (var instance in oBOIList)
                            {
                                XIIBO oBO = new XIIBO();
                                if (instance.Attributes.ContainsKey("iStatus"))
                                {
                                    instance.Attributes["iStatus"].sValue = "20";
                                    oBO.Save(instance);
                                }
                            }
                        }
                    }
                }
                oResult.oTraceStack.Add(new CNV { sName = "Load Aggregations BO Instance", sValue = "Aggregations bo instance loaded for this id :" + ResList[0] });
                if (oBOI.Attributes != null)
                {
                    var InstanceID = oBOI.Attributes.Where(m => m.Key == "FKiQSInstanceID").Select(m => m.Value.sValue).FirstOrDefault();
                    oResult.oTraceStack.Add(new CNV { sName = "InstanceID", sValue = "InstanceID :" + InstanceID });
                    if (InstanceID != null)
                    {
                        iInsatnceID = Convert.ToInt32(InstanceID);
                    }
                    foreach (var fields in oBOI.Attributes.Where(m => m.Key.Contains("FK")).Select(m => m.Value).ToList())
                    {
                        CNV oNV1 = new CNV();
                        oNV1.sName = fields.sName;
                        oNV1.sValue = fields.sValue;
                        if (!OQSD.ContainsKey(oNV1.sName))
                        {
                            OQSD.Add(oNV1.sName, oNV1);
                        }
                    }
                    var sProcuctVersionID = oBOI.Attributes.Where(m => m.Key.Contains("FKiProductVersionID")).Select(m => m.Value.sValue).FirstOrDefault();
                    var oProductVersionI = oIXI.BOI("ProductVersion_T", sProcuctVersionID);
                    foreach (var fields in oProductVersionI.Attributes.Where(m => m.Key.Contains("FK")).Select(m => m.Value).ToList())
                    {
                        CNV oNV1 = new CNV();
                        oNV1.sName = fields.sName;
                        oNV1.sValue = fields.sValue;
                        if (!OQSD.ContainsKey(oNV1.sName))
                        {
                            OQSD.Add(oNV1.sName, oNV1);
                        }
                    }
                    if (!OQSD.ContainsKey("FKiQuoteID"))
                    {
                        CNV oNV = new CNV();
                        oNV.sName = "FKiQuoteID";
                        oNV.sValue = oBOI.Attributes[oBOI.BOD.sPrimaryKey].sValue;
                        OQSD.Add(oNV.sName, oNV);
                    }
                }
                //XIIXI oXII = new XIIXI();
                // XIIQS oQsInstance= oXII.GetQuestionSetInstanceByID(121, 2598, null, 0, 0, null);
                XIIQS oQsInstance = oCache.Get_QuestionSetCache("QuestionSetCache", ResList[1], iInsatnceID);
                oResult.oTraceStack.Add(new CNV { sName = "Load QuestionSet from cache", sValue = "Question set loaded Sucessfully" });
                oResult.oTraceStack.Add(new CNV { sName = "QsInstance from cache", sValue = "QsInstance ID from Cache :" + oQsInstance.ID });
                var oQSNameValuePair = GetQSNVPairs(oQsInstance);
                if (oQSNameValuePair.xiStatus == 0 && oQSNameValuePair.oResult != null)
                {
                    var oDQS = (Dictionary<string, CNV>)oQSNameValuePair.oResult;
                    foreach (var oQS in oDQS)
                    {
                        if (!OQSD.ContainsKey(oQS.Key))
                        {
                            OQSD.Add(oQS.Key, oQS.Value);
                        }
                    }
                }
                CNV oNV3 = new CNV();
                oNV3.sName = "iStatus";
                oNV3.sValue = "10";
                if (!OQSD.ContainsKey(oNV3.sName.ToString()))
                {
                    OQSD.Add(oNV3.sName.ToString(), oNV3);
                }
                CNV oNV4 = new CNV();
                oNV4.sName = "rGrossPremium";
                oNV4.sValue = rGrossPremium.ToString();
                if (!OQSD.ContainsKey(oNV4.sName.ToString()))
                {
                    OQSD.Add(oNV4.sName.ToString(), oNV4);
                }
                CNV oNV5 = new CNV();
                oNV5.sName = "iPolicyNo";
                oNV5.sValue = PolicySequenceNumber;
                if (!OQSD.ContainsKey(oNV5.sName.ToString()))
                {
                    OQSD.Add(oNV5.sName.ToString(), oNV5);
                }
                if (!OQSD.ContainsKey("tCoverStart"))
                {
                    OQSD["tCoverStart"] = new CNV { sName = "tCoverStart", sValue = "00:00:00" };
                }
                if (!OQSD.ContainsKey("dCoverEnd"))
                {
                    XIIBO oBOIns = new XIIBO();
                    oResult.sMessage = "dCoverStart before conversion" + OQSD["dCoverStart"].sValue;
                    oXID.SaveErrortoDB(oResult);
                    DateTime dt = oBOIns.ConvertToDtTime(OQSD["dCoverStart"].sValue);
                    oResult.sMessage = "dCoverStart after conversion" + dt;
                    oXID.SaveErrortoDB(oResult);
                    if (dt != DateTime.MinValue)
                    {
                        OQSD["dCoverEnd"] = new CNV { sName = "dCoverEnd", sValue = dt.AddYears(1).ToString() };
                    }
                }
                if (!OQSD.ContainsKey("tCoverEnd"))
                {
                    OQSD["tCoverEnd"] = new CNV { sName = "tCoverEnd", sValue = "00:00:00" };
                }
                var PolicyData = oBOI.BuildBoObject("ACPolicy_T", "Create", OQSD);
                if (PolicyData.xiStatus == 0 && PolicyData.oResult != null)
                {
                    oResult.oTraceStack.Add(new CNV { sName = "Create Policy", sValue = "Success: Policy Created Sucessfully" });
                    var oPolicy = (XIIBO)PolicyData.oResult;
                    CNV oNV1 = new CNV();
                    oNV1.sName = "FKi" + oPolicy.BOD.TableName.Split('_').FirstOrDefault() + oPolicy.BOD.sPrimaryKey;
                    oNV1.sValue = oPolicy.Attributes[oPolicy.BOD.sPrimaryKey].sValue;
                    iACPolicyID = Convert.ToInt32(oPolicy.Attributes[oPolicy.BOD.sPrimaryKey].sValue);
                    FKiProductID = oPolicy.Attributes["FKiProductID"].sValue;
                    if (oPolicy.Attributes.ContainsKey("dCoverStart"))
                    {
                        sPolicyStartDate = oPolicy.Attributes["dCoverStart"].sValue;
                    }

                    oResult.oTraceStack.Add(new CNV { sName = "Policy ID", sValue = "Policy ID:" + oNV1.sValue });
                    if (!OQSD.ContainsKey(oNV1.sName))
                    {
                        OQSD.Add(oNV1.sName, oNV1);
                    }
                    // oXiAPI.UpdateDriversToPolicy(oQsInstance.ID, Convert.ToInt32(oPolicy.Attributes.Values.Where(m => m.sName.ToLower() == oPolicy.BOD.sPrimaryKey.ToLower()).Select(m => m.sValue).FirstOrDefault()), 0, sSharedDB);
                }
                var PolicyVersionData = oBOI.BuildBoObject("Policy Version", "Create", OQSD);
                if (PolicyVersionData.xiStatus == 0 && PolicyVersionData.oResult != null)
                {
                    oResult.oTraceStack.Add(new CNV { sName = "Create Policy Version", sValue = "Success: Policy Version Created Sucessfully" });
                    var oPolicyVersion = (XIIBO)PolicyVersionData.oResult;
                    CNV oNV1 = new CNV();
                    oNV1.sName = "FKi" + oPolicyVersion.BOD.TableName.Split('_').FirstOrDefault() + oPolicyVersion.BOD.sPrimaryKey;
                    oNV1.sValue = oPolicyVersion.Attributes[oPolicyVersion.BOD.sPrimaryKey].sValue;
                    oResult.oTraceStack.Add(new CNV { sName = "Policy Version ID", sValue = "Policy Version ID:" + oNV1.sValue });
                    if (!OQSD.ContainsKey(oNV1.sName))
                    {
                        OQSD.Add(oNV1.sName, oNV1);
                    }
                }
                oResult.oTraceStack.Add(new CNV { sName = "QSI ID", sValue = "QS Instance ID:" + oQsInstance.ID });
                var QsInstanceData = oBOI.BuildBoObject("QS Instance", "Create", OQSD, oQsInstance.ID.ToString());
                if (QsInstanceData.xiStatus == 0 && QsInstanceData.oResult != null)
                {
                    oResult.oTraceStack.Add(new CNV { sName = "Update QuestionSet Instance to policy version", sValue = "Success: QuestionSet Instance is Updated with policy version" });
                }
                var TransactionData = oBOI.BuildBoObject("ACTransaction_T", "Create", OQSD);
                if (TransactionData.xiStatus == 0 && TransactionData.oResult != null)
                {
                    oResult.oTraceStack.Add(new CNV { sName = "Create Transaction", sValue = "Success: Transaction Created Sucessfully" });
                    var oTransaction = (XIIBO)TransactionData.oResult;
                    CNV oNV1 = new CNV();
                    oNV1.sName = "FKi" + oTransaction.BOD.TableName.Split('_').FirstOrDefault() + oTransaction.BOD.sPrimaryKey;
                    oNV1.sValue = oTransaction.Attributes[oTransaction.BOD.sPrimaryKey].sValue;
                    if (!OQSD.ContainsKey(oNV1.sName))
                    {
                        OQSD.Add(oNV1.sName, oNV1);
                    }
                }
                CNV oNV2 = new CNV();
                oNV2.sName = "bPolicyStatus";
                oNV2.sValue = "1";
                if (!OQSD.ContainsKey(oNV2.sName))
                {
                    OQSD.Add(oNV2.sName, oNV2);
                }
                var oPayment = oBOI.BuildBoObject("PaymentDetails_T", "Update", OQSD, iPaymentID.ToString());
                if (oPayment.xiStatus == 0 && oPayment.oResult != null)
                {
                    oResult.oTraceStack.Add(new CNV { sName = "Update Policy status in Payment Details", sValue = "Success: Policy status updated Sucessfully" });
                }
                var oResponse = BuildRequirementsBO(iACPolicyID, FKiProductID, sPolicyStartDate);
                if (!oResponse.bOK && oResponse.xiStatus == xiEnumSystem.xiFuncResult.xiError)
                {
                    oResult.oTraceStack.Add(new CNV { sName = "Create Requirements", sValue = "Error: While Creating Requirements" });
                }
                var oTermsResponse = BuildTermsBo(iACPolicyID, iInsatnceID);
                if (!oTermsResponse.bOK && oTermsResponse.xiStatus == xiEnumSystem.xiFuncResult.xiError)
                {
                    oResult.oTraceStack.Add(new CNV { sName = "Create Terms", sValue = "Error: While Creating Terms" });
                }
                oXID.SaveErrortoDB(oResult);
            }
            catch (Exception ex)
            {
                oResult.sMessage = "ERROR: [" + oResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oResult.LogToFile();
                oXID.SaveErrortoDB(oResult);
            }
            return oResult;
        }

        public CResult BuildRequirementsBO(int iACPolicyID, string FKiProductID, string sPolicyStartDate)
        {
            CResult oCResult = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = oCResult.Get_MethodName();

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            if (oCResult.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCResult.xiStatus;
            }
            //in the case of
            //xiEnumSystem.xiFuncResult.xiLogicalError
            oCResult.sMessage = "someone tried to do something they shouldnt";
            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                //XIInfraCache oCache = new XIInfraCache();
                QueryEngine oQE = new QueryEngine();
                List<XIWhereParams> oWParams = new List<XIWhereParams>();
                XIWhereParams oWP = new XIWhereParams();
                oWP.sField = "iType";
                oWP.sOperator = "=";
                oWP.sValue = "10";
                oWParams.Add(oWP);
                oWP = new XIWhereParams();
                oWP.sField = "FKiTransactionTypeID";
                oWP.sOperator = "=";
                oWP.sValue = "2";
                oWParams.Add(oWP);
                oWP = new XIWhereParams();
                oWP.sField = "FKiProductID";
                oWP.sOperator = "=";
                oWP.sValue = FKiProductID;
                oWParams.Add(oWP);
                //load requirement template definition of productid and FKiTransactionTypeID
                oQE.AddBO("Requirement_T", "Save Group", oWParams);
                //oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "BO definition added successfully to the QueryEngine" });
                CResult oCresult = oQE.BuildQuery();
                //oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "BO Instance query build successfully" });
                if (oCresult.bOK && oCresult.oResult != null)
                {
                    var sSql = (string)oCresult.oResult;
                    ExecutionEngine oEE = new ExecutionEngine();
                    oEE.XIDataSource = oQE.XIDataSource;
                    oEE.sSQL = sSql;
                    var oQResult = oEE.Execute();
                    if (oQResult.bOK && oQResult.oResult != null)
                    {
                        //oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "BO Instance query executed successfully" });
                        var oBOIList = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.ToList();
                        var oBOD = oQE.QParams.FirstOrDefault().BOD;
                        oBOIList.ForEach(x => x.BOD = oBOD);
                        foreach (var instance in oBOIList)
                        {
                            XIIBO oBO = new XIIBO(); string sDueInDays = string.Empty;
                            if (instance.Attributes.ContainsKey("FKiPolicyID"))
                            {
                                instance.Attributes["FKiPolicyID"].sValue = iACPolicyID.ToString();
                                if (instance.Attributes.ContainsKey("iType"))
                                {
                                    instance.Attributes["iType"].sValue = "0";
                                }
                                if (instance.Attributes.ContainsKey("iDueInDays"))
                                {
                                    sDueInDays = instance.Attributes["iDueInDays"].sValue;
                                }
                                if (instance.Attributes.ContainsKey("dDue"))
                                {
                                    if (!string.IsNullOrEmpty(sDueInDays))
                                    {
                                        int iDueInDays = Convert.ToInt32(sDueInDays);
                                        var sPolicyStartdate = oBO.ConvertToDateTime(sPolicyStartDate);
                                        var sDueDate = Convert.ToDateTime(sPolicyStartdate).AddDays(iDueInDays).ToString("yyyy-MM-dd");
                                        instance.Attributes["dDue"].sValue = sDueDate;
                                    }
                                }
                                int sPrimaryKeyValue = Convert.ToInt32(instance.Attributes.Where(n => n.Key.ToLower().Equals(instance.BOD.sPrimaryKey.ToLower())).Select(s => s.Value).FirstOrDefault().sValue);
                                if (sPrimaryKeyValue != 0)
                                {
                                    instance.Attributes.Where(n => n.Key.ToLower().Equals(instance.BOD.sPrimaryKey.ToLower())).Select(m => m.Value).ToList().ForEach(m => { m.sValue = null; });//set primary column to null
                                    instance.Attributes.ToList().ForEach(m => m.Value.bDirty = true);
                                }
                                if (instance.Attributes.ContainsKey("iStatus"))
                                {
                                    instance.Attributes["iStatus"].sValue = "50";
                                    //instance.Attributes.Where(n => n.Key.ToLower().Equals("iStatus".ToLower())).Select(m => m.Value).ToList().ForEach(m => { m.sValue = "0";});
                                }

                                if (instance.Attributes.ContainsKey("sFunction"))
                                {
                                    string sFunction = instance.Attributes["sFunction"].sValue;
                                    if (!string.IsNullOrEmpty(sFunction))
                                    {
                                        string sGUID = Guid.NewGuid().ToString();
                                        List<CNV> oNVList = new List<CNV>();
                                        CNV oParam = new CNV();
                                        oParam.sName = "sFunction";
                                        oParam.sValue = sFunction;
                                        oNVList.Add(oParam);
                                        oParam = new CNV();
                                        oParam.sName = "-policyid";
                                        oParam.sValue = iACPolicyID.ToString();
                                        oNVList.Add(oParam);
                                        oCache.SetXIParams(oNVList, sGUID,"");
                                        CResult oCR = new CResult();
                                        XIDScript oXIScript = new XIDScript();
                                        oXIScript.sScript = sFunction.ToString();
                                        oCR = oXIScript.Execute_Script(sGUID,"");
                                        if (oCR.bOK && oCR.oResult != null)
                                        {
                                            string sValue = (string)oCR.oResult;
                                            if (sValue == "true")
                                            {
                                                var oRequirement = oBO.Save(instance);
                                            }
                                            oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Requirements script executed successfully" });
                                        }
                                        // var oInstanceobj = oIXI.BOI("ACPolicy_T", Convert.ToString(iACPolicyID)).Structure("New Policy").XILoad();
                                    }
                                    else
                                    {
                                        var oRequirement = oBO.Save(instance);
                                    }

                                }
                                //set policyid and add related requirement templates
                            }
                        }
                    }
                }

                oCResult.oResult = null;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oXID.SaveErrortoDB(oCResult);
            }
            return oCResult; // always
        }
        public CResult BuildTermsBo(int iACPolicyID, int iInsatnceID)
        {
            CResult oCResult = new CResult();
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = oCResult.Get_MethodName();

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            if (oCResult.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCResult.xiStatus;
            }
            //in the case of
            //xiEnumSystem.xiFuncResult.xiLogicalError
            oCResult.sMessage = "someone tried to do something they shouldnt";
            //tracing
            // if tracing is on (which is a config switch for this user) then
            //   oCResult.Trace.Write
            //ALL OK?
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                QueryEngine oQE = new QueryEngine();
                List<XIWhereParams> oWParams = new List<XIWhereParams>();
                XIWhereParams oWP = new XIWhereParams();
                oWP.sField = "FKiQsInstanceID";
                oWP.sOperator = "=";
                oWP.sValue = iInsatnceID.ToString();
                oWParams.Add(oWP);
                //load requirement template definition of productid and FKiTransactionTypeID
                oQE.AddBO("Term_T", "", oWParams);
                CResult oCresult = oQE.BuildQuery();
                //oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "BO Instance query build successfully" });
                if (oCresult.bOK && oCresult.oResult != null)
                {
                    var sSql = (string)oCresult.oResult;
                    ExecutionEngine oEE = new ExecutionEngine();
                    oEE.XIDataSource = oQE.XIDataSource;
                    oEE.sSQL = sSql;
                    var oQResult = oEE.Execute();
                    if (oQResult.bOK && oQResult.oResult != null)
                    {
                        //oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "BO Instance query executed successfully" });
                        var oBOIList = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.ToList();
                        var oBOD = oQE.QParams.FirstOrDefault().BOD;
                        oBOIList.ForEach(x => x.BOD = oBOD);
                        foreach (var instance in oBOIList)
                        {
                            XIIBO oBO = new XIIBO();
                            if (instance.Attributes.ContainsKey("FKiACPolicyID"))
                            {
                                instance.Attributes["FKiACPolicyID"].sValue = iACPolicyID.ToString();
                            }
                            var oTerms = oBO.Save(instance);
                        }
                    }
                }
                oCResult.oResult = null;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oXID.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        public CResult GetQSNVPairs(XIIQS oQsInstance)
        {
            CResult oResult = new CResult();
            try
            {
                Dictionary<string, CNV> OQSD = new Dictionary<string, CNV>(StringComparer.CurrentCultureIgnoreCase);
                foreach (var Step in oQsInstance.Steps.Values)
                {
                    foreach (var Section in Step.Sections.Values.ToList())
                    {
                        foreach (var xival in Section.XIValues.Values.ToList())
                        {
                            var FieldName = oQsInstance.QSDefinition.Steps.Values.Where(m => m.ID == Step.FKiQSStepDefinitionID).Select(m => m.FieldDefs).FirstOrDefault();
                            if (FieldName != null)
                            {
                                var FName = FieldName.Values.Where(m => m.ID == xival.FKiFieldDefinitionID).Select(m => m.FieldOrigin).FirstOrDefault();
                                if (FName != null)
                                {
                                    CNV oNV = new CNV();
                                    oNV.sName = FName.sName.ToString();
                                    oNV.sValue = xival.sValue;
                                    if (!OQSD.ContainsKey(FName.sName.ToString()))
                                    {
                                        OQSD.Add(FName.sName.ToString(), oNV);
                                    }
                                }
                            }
                        }
                    }
                }
                oResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oResult.oResult = OQSD;
            }
            catch (Exception ex)
            {
                oResult.sMessage = "ERROR: [" + oResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oResult.LogToFile();
                oXID.SaveErrortoDB(oResult);
            }
            return oResult;
        }

    }
}