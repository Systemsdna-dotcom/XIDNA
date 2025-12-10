using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using XICore;
using XIDNA.Repository;
using XISystem;

namespace XIDNA.Controllers
{
    public class TempDBCreationController : Controller
    {
        XIDefinitionBase oXID = new XIDefinitionBase();
        // GET: TempDBCreation
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DBCreation(string ServerName, string DBName, string DBID, string UserName, string Password, string SchemaPath, string DataPath)
        {
            string Result = string.Empty;
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "DB Creating";//expalin about this method logic
            String str = "CREATE DATABASE " + DBName;
            if (!string.IsNullOrEmpty(ServerName) && !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(DBName))//check mandatory params are passed or not
            {
                var sConnectionString = "Data Source = " + ServerName + "; initial catalog = master; User Id = " + UserName + "; Password = " + Password + "; MultipleActiveResultSets = True";

                SqlConnection myConn = new SqlConnection(sConnectionString);
                SqlCommand myCommand = new SqlCommand(str, myConn);
                try
                {
                    myConn.Open();
                    var sqlCreateDBQuery = string.Format("SELECT database_id FROM sys.databases WHERE Name = '{0}'", DBName);
                    SqlCommand myCommand1 = new SqlCommand(sqlCreateDBQuery, myConn);
                    var resultObj = myCommand1.ExecuteScalar();
                    int databaseID = 0;

                    if (resultObj != null)
                    {
                        int.TryParse(resultObj.ToString(), out databaseID);
                    }
                    if (databaseID == 0)
                    {
                        myCommand.ExecuteNonQuery();
                        var oCResult1 = DBChecking(SchemaPath, DataPath, false, DBName, sConnectionString, DBID);
                        Result = DBName + " DB Created Sucessfully " + oCResult1.oResult;
                    }
                    else
                    {
                        var oCResult1 = DBChecking(SchemaPath, DataPath, false, DBName, sConnectionString, DBID);
                        if (!string.IsNullOrEmpty((string)oCResult1.oResult))
                            Result = DBName + " Existing DB But " + oCResult1.oResult;
                        else
                            Result = DBName + " Existing DB";
                    }
                }
                catch (Exception ex)
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                    int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                    oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                    oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                    oCResult.sCategory = ex.GetType().ToString();
                    oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                    oXID.SaveErrortoDB(oCResult);
                    Result = "Check Your Connections";
                }
                finally
                {
                    if (myConn.State == ConnectionState.Open)
                    {
                        myConn.Close();
                    }
                }
            }
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public CResult DBChecking(string SchemaPath, string DataPath, bool result, string NewDBName, string sConnectionString, string DBID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "DB Creating";//expalin about this method logic
            var fileName = string.Empty;
            try
            {
                var ConString = sConnectionString;
                sConnectionString = sConnectionString.Replace("master", NewDBName);
                string sSchemaPath = SchemaPath;//oNVParam.Where(x => x.sName.ToLower() == "sSchemaPath".ToLower()).Select(t => t.sValue).First();
                string sDataPath = DataPath;//oNVParam.Where(x => x.sName.ToLower() == "sDataPath".ToLower()).Select(t => t.sValue).First();
                if (result == false)
                {
                    if (!string.IsNullOrEmpty(sSchemaPath))
                    {
                        //string dir = Directory.GetDirectories(@sSchemaPath).FirstOrDefault();
                        string[] filePaths = Directory.GetFiles(sSchemaPath, "*.sql", SearchOption.AllDirectories);

                        if (filePaths.Count() > 0)
                        {
                            // Copy the files and overwrite destination files if they already exist.
                            foreach (string s in filePaths)
                            {
                                fileName = System.IO.Path.GetFileName(s);
                                string text = System.IO.File.ReadAllText(s);
                                //SqlConnection Connetionstring = new SqlConnection("Data Source=INATIVE-PC137\\MSSQLEXPRESS;initial catalog=" + NewDBName + ";User Id=XIDNADMIN; Password=XIDNADMIN;"); ///*INATIVE-PC137"+@"\MSSQLEXPRESS*/
                                SqlConnection Connetionstring = new SqlConnection(sConnectionString);
                                string createtableCommandString = text;
                                using (Connetionstring)
                                {
                                    SqlCommand command = new SqlCommand(createtableCommandString, Connetionstring);
                                    command.Connection.Open();
                                    command.ExecuteNonQuery();
                                    command.Connection.Close();
                                }
                            }
                            oCResult.oResult = " Schema Inserted";
                        }
                    }
                    if (!string.IsNullOrEmpty(sDataPath))
                    {
                        string[] subdirectoryEntries = Directory.GetDirectories(sDataPath);
                        foreach (var subdirectory in subdirectoryEntries)
                        {
                            var PathInfo = subdirectory.Split('\\').ToList();
                            var TableName = PathInfo.LastOrDefault();
                            string[] filePaths = Directory.GetFiles(subdirectory, "*.sql", SearchOption.AllDirectories);
                            if (filePaths.Count() > 0)
                            {
                                // Copy the files and overwrite destination files if they already exist.
                                string text = string.Empty;
                                //SqlConnection Connetionstring = new SqlConnection("Data Source=INATIVE-PC137\\MSSQLEXPRESS;initial catalog="+NewDBName+";User Id=XIDNADMIN; Password=XIDNADMIN;"); 
                                SqlConnection Connetionstring = new SqlConnection(sConnectionString);
                                SqlCommand command = new SqlCommand();
                                bool Identity = true;
                                using (Connetionstring)
                                {
                                    Connetionstring.Open();
                                    command = new SqlCommand("SET IDENTITY_INSERT[dbo].[" + TableName + "] ON ", Connetionstring); try   //To do
                                    {
                                        command.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        Identity = false;
                                    }
                                    int maxRows = 5000;
                                    int Count = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(filePaths.Count()) / maxRows));
                                    //var Count1 = Math.Round(Convert.ToDouble(filePaths.Count()) / maxRows);
                                    int startIndex = 0; int n = 0;
                                    for (startIndex = 1; startIndex <= Convert.ToInt32(Count); startIndex++)
                                    {
                                        text = string.Empty;
                                        var FilePathslist = filePaths.Skip(n).Take(maxRows).ToList();
                                        foreach (string s in FilePathslist)
                                        {
                                            fileName = System.IO.Path.GetFileName(s);
                                            text += System.IO.File.ReadAllText(s) + "\n";
                                        }
                                        string createtableCommandString = text;
                                        command = new SqlCommand(createtableCommandString, Connetionstring);

                                        command.ExecuteNonQuery();
                                        if (fileName.Contains("XIDataSource_XID_T"))
                                        {
                                            Dictionary<int, string> myDict = new Dictionary<int, string>();
                                            myDict.Add(50, "XIEnvironment");
                                            myDict.Add(4081, "IO_Setting");
                                            myDict.Add(4082, "IO_Data_OrgName");
                                            myDict.Add(4083, "IO_Core");
                                            CXiAPI oXIAPI = new CXiAPI();
                                            foreach (var item in myDict)
                                            {
                                                var ConString1 = ConString.Replace("master", item.Value);
                                                var sEnrypted = oXIAPI.EncryptData(ConString1, true, item.Key.ToString());
                                                command = new SqlCommand();
                                                command = new SqlCommand("UPDATE XIDataSource_XID_T SET sConnectionString ='" + sEnrypted + "' WHERE ID =" + item.Key, Connetionstring);
                                                command.ExecuteNonQuery();
                                            }
                                        }
                                        n = startIndex * maxRows;
                                    }
                                    if (Identity)
                                    {
                                        command = new SqlCommand("SET IDENTITY_INSERT[dbo].[" + TableName + "] OFF ", Connetionstring); try   //To do
                                        {
                                            command.ExecuteNonQuery();
                                        }
                                        catch (Exception ex)
                                        {
                                            Identity = false;
                                        }
                                    }
                                    Connetionstring.Close();
                                }
                                //}
                                oCResult.oResult += " Data Inserted ";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oCResult.oResult = "Alredy Existing " + fileName;
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXID.SaveErrortoDB(oCResult);
            }
            return oCResult;
        }

    }
}