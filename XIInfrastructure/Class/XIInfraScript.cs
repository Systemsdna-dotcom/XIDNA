using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using XICore;
using XIDatabase;
using XISystem;

namespace XIInfrastructure
{
    public class XIInfraScript
    {
        XIDefinitionBase oXID = new XIDefinitionBase();
        public CResult XIScripting(int iScriptID, string sGUID, int iInstanceID,int iUserID, string sDatabase, int iCustomerID, string ProductName, string Version, string ProductCode, string dtOldCoverStart, string rOldGrossPremium,int QuoteID)
        {
            CResult oCResult = new CResult();
            try
            {
                var oResult = RunXIScript(iInstanceID, sGUID, iScriptID, sDatabase, iUserID, iCustomerID, ProductName, Version, ProductCode, dtOldCoverStart, rOldGrossPremium,QuoteID);
                if (oResult.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess && oResult.oResult != null)
                {
                    oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Script Executed successfully" });
                    CResult xiResults = (CResult)(oResult.oResult);
                    //get scriptresult details
                    XIDScriptResult sScriptResult = new XIDScriptResult();
                    sScriptResult.FKiScriptID = iScriptID;
                    sScriptResult.sResultcode = xiResults.oCollectionResult.Where(m => m.sName == "sCode").Select(m => m.sValue).FirstOrDefault();
                    oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Get Script result" });
                    var oBoScriptResult = sScriptResult.Get_BOScriptResult(sDatabase);
                    if (oBoScriptResult.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess && oBoScriptResult.oResult != null)
                    {
                        oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Script result loaded successfully" });
                        sScriptResult = (XIDScriptResult)oBoScriptResult.oResult;
                    }
                    string sUserError = sScriptResult.sUserError;
                    string sGetStringVal = "";
                    if (sUserError.Contains('{') && sUserError.Contains('}'))
                    {
                        Regex Regx = new Regex(@"{(.+?)}");
                        MatchCollection MatchCol = Regx.Matches(sUserError);
                        for (int i = 0; i < MatchCol.Count; i++)
                        {
                            //Split and get string
                            string[] sSplitUserError = MatchCol[i].Value.Split(new Char[] { '{', '}' });
                            sGetStringVal = sSplitUserError[1];
                            var sNVValue = xiResults.oTraceStack.Where(m => m.sName == sGetStringVal).Select(m => m.sValue).FirstOrDefault();
                            sUserError = sUserError.Replace("{" + sGetStringVal + "}", sNVValue);
                        }
                    }
                    else
                    {
                        //show the error to user
                        // sUserError = sUserError;
                    }
                    string sReturn = sScriptResult.iType + "_" + sUserError;
                    oCResult.oResult = sReturn;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oCollectionResult = xiResults.oCollectionResult;
                }
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

        private CResult RunXIScript(int ID, string sGUID, int iScriptID, string sDatabase, int iUserID, int iCustomerID, string ProductName, string Version, string ProductCode, string dtOldCoverStart, string rOldGrossPremium,int QuoteID)
        {
            CResult oCResult = new CResult();
            try
            {
                //Set parameters....
                var sSessionID = HttpContext.Current.Session.SessionID;
                //Create a paramlist
                List<CNV> lParam = new List<CNV>();
                CNV Param = new CNV();
                Param.sName = "sUID";
                Param.sValue = sGUID;
                lParam.Add(Param);
                CNV Param1 = new CNV();
                Param1.sName = "sDataBase";
                Param1.sValue = sDatabase;
                lParam.Add(Param1);
                CNV ParamSession = new CNV();
                ParamSession.sName = "sSessionID";
                ParamSession.sValue = sSessionID;
                lParam.Add(ParamSession);
                CNV param = new CNV();
                param.sName = "iInsatnceID";
                param.sValue = ID.ToString();
                lParam.Add(param);
                CNV param1 = new CNV();
                param1.sName = "iUserID";
                param1.sValue = iUserID.ToString();
                lParam.Add(param1);
                CNV param2 = new CNV();
                param2.sName = "iCustomerID";
                param2.sValue = iCustomerID.ToString();
                lParam.Add(param2);
                CNV ProductNameparam = new CNV();
                ProductNameparam.sName = "ProductName";
                ProductNameparam.sValue = ProductName;
                lParam.Add(ProductNameparam);
                CNV Versionparam = new CNV();
                Versionparam.sName = "Version";
                Versionparam.sValue = Version;
                lParam.Add(Versionparam);
                CNV ProductCodeparam = new CNV();
                ProductCodeparam.sName = "ProductCode";
                ProductCodeparam.sValue = ProductCode;
                lParam.Add(ProductCodeparam);
                CNV dCoverStart = new CNV();
                dCoverStart.sName = "dCoverStart";
                dCoverStart.sValue = dtOldCoverStart;
                lParam.Add(dCoverStart);
                CNV rGrossPremium = new CNV();
                rGrossPremium.sName = "rGrossPremium";
                rGrossPremium.sValue = rOldGrossPremium;
                lParam.Add(rGrossPremium);
                CNV iQuoteID = new CNV();
                iQuoteID.sName = "iQuoteID";
                iQuoteID.sValue = QuoteID.ToString();
                lParam.Add(iQuoteID);
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Name Value pairs added successfully" });
                XIIXI oXI = new XIIXI();
                var oBOI = oXI.BOI("XiScript_T", iScriptID.ToString());
                string sScript = oBOI.Attributes["sScript"].sValue;
                string sMethodName = oBOI.Attributes["sMethodName"].sValue;
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Script loaded successfully" });
                //Create a object to get the script values returned
                object oResult = null;
                XIInfraCache oCache = new XIInfraCache();
                //TestScript.PolicyMainCal(lParam);
                ////Test 1:         

                //Stopwatch stopWatch = new Stopwatch();
                //stopWatch.Start();
                //for (int i = 0; i <= 1000; i++)
                //{
                //    Call the method to compile the script with script as parameter   
                //    MethodInfo methodInfo = WriteXIMethod(sScript);

                //    if (methodInfo != null)
                //    {
                //        oResult = methodInfo.Invoke(null, new object[] { oXiAPI, lParam });
                //    }
                //}
                // Thread.Sleep(10000);
                //stopWatch.Stop();
                // Get the elapsed time as a TimeSpan value.
                //TimeSpan ts = stopWatch.Elapsed;
                // Format and display the TimeSpan value. 
                //string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                //    ts.Hours, ts.Minutes, ts.Seconds,
                //    ts.Milliseconds / 10);

                //// test 2.
                //Call the method to compile the script with script as parameter   
                MethodInfo methodInfoFromCache = oCache.Get_ScriptVal(sSessionID, "XIScript_" + oBOI.Attributes["ID"].sValue);
                if (methodInfoFromCache != null)
                {
                    oResult = methodInfoFromCache.Invoke(null, new object[] { lParam });
                }
                else
                {
                    var info = WriteXIMethod(sScript, sMethodName);
                    if(info.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess && info.oResult !=null)
                    {
                        MethodInfo methodInfo = (MethodInfo)info.oResult;
                        methodInfoFromCache = methodInfo;
                        oResult = methodInfoFromCache.Invoke(null, new object[] { lParam });
                       
                        oCache.Set_ScriptVal(sSessionID, "XIScript_"+ oBOI.Attributes["ID"].sValue, methodInfoFromCache);
                    }
                }
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                //for (int i = 0; i <= 1000; i++)
                //{
                //}
                // Thread.Sleep(10000);
                stopWatch.Stop();
                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;
                // Format and display the TimeSpan value. 
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                oCResult.oResult = oResult;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch(Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oXID.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

        public CResult WriteXIMethod(string sScript, string sMethodName)
        {
            CResult oCResult = new CResult();
            try
            {
                string sCode = @"  using System;
                     using System.Core;
                  using System.Text.RegularExpressions;
                  using System.Linq;
                  using System.Collections.Generic;
                  using System.Threading;
                  using System.Data;
                  using System.Data.SqlClient;
                  using System.Data.Entity; 
                  using XICore;
                  using XISystem;
                   namespace XIScripting
                   {                
                       public class cXIScripting
                       {                
                          " + sScript + @"
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
                loParameters.ReferencedAssemblies.Add(sdllPath + "\\XIInfrastructure.dll");
                loParameters.ReferencedAssemblies.Add(sdllPath + "\\XISystem.dll");
                loParameters.GenerateInMemory = false;
                ICodeCompiler provider = new CSharpCodeProvider().CreateCompiler();
                CompilerResults results = provider.CompileAssemblyFromSource(loParameters, sCode);
                if (results.Errors.HasErrors)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (CompilerError error in results.Errors)
                    {
                        oCResult.sMessage = error.ErrorText;
                        oXID.SaveErrortoDB(oCResult);
                        sb.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
                    }
                    throw new InvalidOperationException(sb.ToString());
                }
                Type binaryFunction = results.CompiledAssembly.GetType("XIScripting.cXIScripting");
                var result = binaryFunction.GetMethod(sMethodName);
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = result;
            }
            catch(Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                oXID.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }


        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
        public CResult GetMultipleScripts(string sSelect, string sTableName, Dictionary<string, object> Params)
        {
            CResult oCResult = new CResult();
            try
            {
                var result = Connection.SelectString(sSelect, sTableName, Params);
                oCResult.oResult = result;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult;
        }
        public object EvalueNotation(string sScript, List<CNV> nvPairsList)
        {
            //sScript = sScript.Replace("\"", "\"\"");
            string sCode = @"  using System;
                  using XIDNA.Models;
                  using XIDNA.ViewModels;
                  using System.Core;
                  using XIDNA.Repository;
                  using System.Linq;
                  using System.Collections.Generic;
                  using System.Threading;
                  using System.Data;
                  using System.Data.SqlClient;
                  using System.Data.Entity;
                  using XISystem;
                  using XICore;
                   namespace XIScriptingNotation
                   {                
                       public class cXIScriptingNotation
                       {                
                             public static CResult GetNotationResult(List<CNV> nvPairsList)
        {
              XIIBO oBII = new XIIBO(); XIIXI oIXI = new XIIXI();
            CResult CResult = new CResult();
           oBII.sCoreDataBase = nvPairsList.Where(m => m.sName == ""sCoreDatabaseName"").FirstOrDefault().sValue;
            oBII.sOrgDataBase = nvPairsList.Where(m => m.sName == ""sOrgDatabase"").FirstOrDefault().sValue;
            oBII.iOrgId = Convert.ToInt32(nvPairsList.Where(m => m.sName == ""iOrgID"").FirstOrDefault().sValue);
            oIXI.sCoreDatabase = oBII.sCoreDataBase;
            oIXI.sOrgDatabase = oBII.sOrgDataBase;
            oIXI.iOrgID = oBII.iOrgId;
            var oLIst = " + sScript + @";
            CResult.oResult = oLIst;
            return CResult;
        }
                       }
                   }
               ";
            //var oLIst = oIXI.BOI(""ACPolicy_T"", ""127"").Structure(""ClientPolicy"").XILoad().oSubStructureI(""Driver_T"");
            // var oLIst = " + sScript + @";
            CompilerParameters loParameters = new CompilerParameters();
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            string sPath = physicalPath.Substring(0, physicalPath.Length) + "\\DLL";
            //string sPath = System.Configuration.ConfigurationManager.AppSettings["DLLPath"];
            // *** Start by adding any referenced assemblies
            loParameters.ReferencedAssemblies.Add("System.dll");
            loParameters.ReferencedAssemblies.Add(sPath + "\\System.Core.dll");
            loParameters.ReferencedAssemblies.Add(sPath + "\\System.Linq.dll");
            //loParameters.ReferencedAssemblies.Add(sPath + "\\System.Data.Linq.dll");
            loParameters.ReferencedAssemblies.Add(sPath + "\\XIDNA.Models.dll");
            loParameters.ReferencedAssemblies.Add(sPath + "\\XIDNA.Repository.dll");
            loParameters.ReferencedAssemblies.Add(sPath + "\\System.Data.dll");
            loParameters.ReferencedAssemblies.Add(sPath + "\\EntityFramework.dll");
            loParameters.ReferencedAssemblies.Add(sPath + "\\EntityFramework.SqlServer.dll");
            loParameters.ReferencedAssemblies.Add(sPath + "\\XIDNA.ViewModels.dll");
            loParameters.ReferencedAssemblies.Add(sPath + "\\XICore.dll");
            loParameters.ReferencedAssemblies.Add(sPath + "\\XISystem.dll");
            // loParameters.ReferencedAssemblies.Add("D:\\TfsProject\\XIDNA\\XIDNA\\XIDNA.Repository\\bin\\Debug\\XIDNA.Repository.dll");
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
            //if (api == null)
            //{

            //}
            //if (nvPairsList = null) { }
            object oResult = methodInfo.Invoke(null, new object[] { nvPairsList });

            return oResult;

        }

    }
}