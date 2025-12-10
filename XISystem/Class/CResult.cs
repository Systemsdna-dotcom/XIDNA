using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using xiEnumSystem;
using System.ComponentModel;

namespace XISystem
{
    public class CResult
    {
        private string sMyMessage = string.Empty;
        private string sMyQuery = string.Empty;
        private string sMyClassName = string.Empty;
        private string sMyFunctionName = string.Empty;
        private List<CNV> oColNV = new List<CNV>();
        private xiFuncResult xiMyStatus;   // Long
        private List<CNV> oColResult = new List<CNV>();
        private object oMyResult;
        private cMyEnvironment oMyEnvironment;
        private StringBuilder oSB = new StringBuilder();
        public long iTraceLevel = 0;
        public string sCode { get; set; }
        public int iRowXilinkID { get; set; }
        public CTraceStack oTrace = new CTraceStack();
        public string sScript = string.Empty;
        public string sCategory { get; set; }
        public int FKiApplicationID { get; set; }
        public int iType { get; set; }
        public int iCriticality { get; set; }
        public string AddMessage(string sMessage)
        {
            string sResult = string.Empty;

            try
            {
                oSB.AppendLine(sMessage);
            }
            // sMyMessage = sMyMessage & sMessage & vbCrLf

            catch (Exception ex)
            {
            }

            return sResult;
        }

        public string sAppend
        {
            get
            {
                return sMyMessage;
            }

            set
            {
                sMessage = sMessage + value + "\r\n";
            }
        }

        public string LogToFile()
        {
            string sResult = "";

            // OK, this is the simplest logging method, which is used to write errors down to a file, without knowing whether any other infrastructure is loaded
            // so a simplle last ditch flat file system

            try
            {
                sMyMessage = oSB.ToString() + sMyMessage;
                if (sMyMessage == "")
                    return null;
                WriteLog(sMyMessage);
                oSB.Clear();   // this is so a concatenated message does not get written multiple times
                //sMyMessage = "";
                sResult = "";
            }
            catch (Exception ex)
            {
                //oMyResult.xiStatus = xiFuncResult.xiError;
                sResult = "ERROR: [" + Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
            }

            return sResult;
        }

        public string LogToFile_Query()
        {
            string sResult = "";

            //// OK, this is the simplest logging method, which is used to write errors down to a file, without knowing whether any other infrastructure is loaded
            //// so a simplle last ditch flat file system

            //try
            //{
            //    WriteLog(sMyQuery, sSubLog: "QUERY");
            //    sResult = "";
            //}
            //catch (Exception ex)
            //{
            //    oMyResult.xiStatus = xiFuncResult.xiError;
            //    sResult = "ERROR: [" + Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + Constants.vbCrLf;
            //}


            return sResult;
        }

        private void WriteLog(string sWrite, long iType = 0, string sSubLog = "")
        {
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            string sPath = physicalPath.Substring(0, physicalPath.Length) + "\\XIDNAErrorLog";
            // Dim oLogger As System.IO.StreamWriter

            string sMyLogFile = sPath + "\\XI.txt";
            System.IO.FileStream oFile;
            string sSubLogDir = "";

            try
            {

                // sMyLogFile = "C:\XI.txt"      ' My.Settings.LogFile

                if (oMyEnvironment != null)
                {
                    if (sSubLog != "")
                    {
                        sSubLogDir = sSubLog + @"\";
                        sSubLog = "." + sSubLog;
                    }

                    sMyLogFile = oMyEnvironment.sLogPath + sSubLogDir + oMyEnvironment.sAppName + sSubLog + "." + DateTime.Now.ToString("yyyy.MM.dd") + ".txt";

                    if (System.IO.Directory.Exists(oMyEnvironment.sLogPath + sSubLogDir) == false)
                    {
                        try
                        {
                            System.IO.Directory.CreateDirectory(oMyEnvironment.sLogPath + sSubLogDir);
                        }
                        catch (Exception exDir)
                        {
                            sMyLogFile = sPath + "\\XI_no_Dir.txt";
                        }
                    }

                    if (System.IO.File.Exists(sMyLogFile) == false)
                    {
                        try
                        {
                            oFile = System.IO.File.Create(sMyLogFile);
                            oFile.Close();
                        }
                        catch (Exception exCreate)
                        {
                            // default to something
                            sMyLogFile = sPath + "\\XI.txt";
                        }
                    }
                }

                // If My.Settings.LogLevel <= iType Then    'so if loglevel is 5 and the type is 1 (not very important) it wont get written.

                // if loglevel is 0 then EVERYTHING is written

                using (System.IO.StreamWriter w = System.IO.File.AppendText(sMyLogFile))
                {
                    Log(sMessage, w);

                    // Close the writer and underlying file.

                    w.Close();
                }
            }

            // End If



            catch (Exception ex)
            {
            }
        }



        public void Log(string logMessage, System.IO.TextWriter w)
        {
            w.Write("\r\n" + "Log Entry : ");

            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());

            w.WriteLine("  :");

            w.WriteLine("  :{0}", "C: " + sMyClassName);
            w.WriteLine("  :{0}", "F: " + sMyFunctionName);
            w.WriteLine("  :{0}", logMessage);

            if (oTraceStack != null && oTraceStack.Count() > 0)
            {
                var oTraceMessages = oTraceStack.Select(m => m.sValue).ToArray();
                var TraceMsg = string.Join("->", oTraceMessages);
                w.WriteLine("  :{0}", TraceMsg);
            }

            w.WriteLine("-------------------------------");

            // Update the underlying file.

            w.Flush();
        }

        public string sMessage
        {
            get
            {
                return sMyMessage;
            }
            set
            {
                sMyMessage = value;
            }
        }

        public string sQuery
        {
            get
            {
                return sMyQuery;
            }
            set
            {
                sMyQuery = value;
            }
        }

        public string sClassName
        {
            get
            {
                return sMyClassName;
            }
            set
            {
                sMyClassName = value + " - ";
            }
        }

        public string sFunctionName
        {
            get
            {
                return sMyFunctionName;
            }
            set
            {
                sMyFunctionName = value + " - ";
            }
        }


        public xiFuncResult xiStatus
        {
            get
            {
                return xiMyStatus;
            }
            set
            {
                if (xiMyStatus != xiFuncResult.xiError)
                    // DO NOT ALLOW setting if already an error. Which could cause probs if re-using, but prevents overwrite
                    xiMyStatus = value;
            }
        }// Long

        public bool bOK
        {
            get
            {
                if (xiMyStatus == xiFuncResult.xiSuccess)
                    return true;
                else
                    return false;
            }
        }

        public BindingList<CNV> oStack;

        public List<CNV> oTraceStack
        {
            get
            {
                return oColNV;
            }
            set
            {
                oColNV = value;
            }
        }

        public object oResult
        {
            get
            {
                return oMyResult;
            }
            set
            {
                oMyResult = value;
            }
        }

        public List<CNV> oCollectionResult
        {
            get
            {
                return oColResult;
            }
            set
            {
                oColResult = value;
            }
        }

        public cMyEnvironment oEnvironment
        {
            get
            {
                return oMyEnvironment;
            }
            set
            {
                oMyEnvironment = value;
            }
        }

        public string Get_Class()
        {
            System.Reflection.MethodBase mb;
            Type Type;
            string sFullReference = "";

            mb = System.Reflection.MethodBase.GetCurrentMethod();
            Type = mb.DeclaringType;

            sFullReference = Type.FullName;

            return sFullReference;
        }

        public string Get_MethodName()
        {
            var Name = System.Reflection.MethodBase.GetCurrentMethod().Name;
            return System.Reflection.MethodBase.GetCurrentMethod().Name;
        }

        public CResult Test_Method()
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {
                oTrace.oParams.Add(new CNV { sName = "", sValue = "" });
                if (true)//check mandatory params are passed or not
                {
                    //oCR = SubMethod();
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        oTrace.oTrace.Add(oCR.oTrace);
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            oCResult.oResult = "Success";
                        }
                        else
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        }
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param:  is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                //SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
    }
}