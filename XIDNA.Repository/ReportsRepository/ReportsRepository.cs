using XIDNA.Models;
using XIDNA.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO; // HSSFWorkbook, HSSFSheet

namespace XIDNA.Repository
{
    public class ReportsRepository : IReportsRepository
    {
        ModelDbContext dbContext = new ModelDbContext();
        public List<UserReports> GetAllDashboardReports(int UserID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == UserID).Select(m => m.RoleID).FirstOrDefault();
            int Type = (Int32)Enum.Parse(typeof(EnumLocations), EnumLocations.DashboardReports.ToString());
            var Reports = dbContext.UserReports.Where(m => m.RoleID == RoleID && m.Location == Type).ToList();
            return Reports;
        }

        public List<string> GetDialyIncome()
        {
            var lFinalList = new List<string>();
            List<int> lIncome = new List<int>();
            List<string> lStage = new List<string>();
            int iTotal = 0;
            try
            {
                //var a = new System.Data.DataTable();
                Dictionary<string, string> myDict = new Dictionary<string, string>();
                var lID = new List<string>();
                using (SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["TestEmail"].ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    Con.Open();
                    cmd.Connection = Con;
                    string sGetSourceID = "SELECT ID from Status";
                    cmd.CommandText = sGetSourceID;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        lID.Add(reader.GetValue(0).ToString());
                    }
                    reader.Close();
                    Con.Close();
                    foreach (var sID in lID)
                    {
                        Con.Open();
                        cmd.Connection = Con;
                        string sGetIncome = "SELECT SUM(income) FROM " + EnumLeadTables.Leads.ToString() + " WHERE status='" + sID + "'";
                        cmd.CommandText = sGetIncome;
                        SqlDataReader reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            int iIncome = Convert.ToInt32(reader1.GetValue(0));
                            lIncome.Add(iIncome);
                        }
                        reader1.Close();
                        Con.Close();
                        Con.Open();
                        cmd.Connection = Con;
                        string sGetData = "SELECT Area from Status WHERE ID='" + sID + "'";
                        cmd.CommandText = sGetData;
                        SqlDataReader reader2 = cmd.ExecuteReader();
                        int count = reader2.FieldCount;
                        while (reader2.Read())
                        {
                            lStage.Add(reader2.GetValue(0).ToString());
                        }
                        reader2.Close();
                        Con.Close();
                    }
                }
                //Get the sum of values, add total
                iTotal = lIncome.Sum(x => Convert.ToInt32(x));
                lIncome.Add(iTotal);
                lStage.Add("Total");
                int iCount = lStage.Count();
                for (int i = 0; i < iCount; i++)
                {
                    string ColName = lStage[i].ToString();
                    string COlValue = lIncome[i].ToString();
                    lFinalList.Add(ColName + ":" + COlValue);
                }
            }
            catch (Exception ex)
            {

            }
            return lFinalList;
        }

        public List<List<string>> GetTransLeadLife()
        {
            var iListValues = new List<List<string>>();
            try
            {
                var a = new System.Data.DataTable();
                var lStageID = new List<string>();
                List<string> lSources = new List<string>();
                lSources.Add("");
                List<string> lFive = new List<string>();
                List<string> lFifteen = new List<string>();
                List<string> lThirty = new List<string>();
                List<string> lSixty = new List<string>();
                List<string> lOneHr = new List<string>();
                List<string> lOneDay = new List<string>();
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand();
                    Con.Open();
                    cmd.Connection = Con;
                    string sGetSourceID = "SELECT ID,Name from Stages";
                    cmd.CommandText = sGetSourceID;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        lStageID.Add(reader.GetValue(0).ToString());
                        lSources.Add(reader.GetValue(1).ToString());
                    }
                    //reader.Close();
                    Con.Close();
                    lFive.Add("1-5 Mins");
                    lFifteen.Add("6-15 Mins");
                    lThirty.Add("16-30 Mins");
                    lSixty.Add("31-60 Mins");
                    lOneHr.Add("1 Hour Plus");
                    lOneDay.Add("1 Day plus");
                    foreach (var sSrc in lStageID)
                    {
                        Con.Open();
                        cmd.Connection = Con;
                        //string sGetFive = "SELECT COUNT(ID) FROM Leads WHERE dImportedOn < DATEADD(MINUTE,1,GETDATE()) AND dImportedOn > DATEADD(MINUTE,-5,GETDATE()) AND [iStatus]='" + sSrc + "'";
                        // string sGetFive = "SELECT COUNT(ID) FROM Leads WHERE DATEDIFF(MINUTE,dImportedOn,GETDATE()) >=1 AND DATEDIFF(MINUTE,dImportedOn,GETDATE()) <=5 AND [iStatus]='" + sSrc + "'";
                        string sGetFive = "SELECT COUNT(ID) FROM " + EnumLeadTables.Leads.ToString() + " WHERE ((DATEDIFF(minute,[dImportedOn],GETDATE()))between 0 and 5) AND [iStatus]='" + sSrc + "'";
                        cmd.CommandText = sGetFive;
                        SqlDataReader reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            lFive.Add(reader1.GetValue(0).ToString());
                        }
                        Con.Close();

                        Con.Open();
                        cmd.Connection = Con;
                        // string sGetFifteen = "SELECT COUNT(ID) FROM Leads WHERE dImportedOn <= DATEADD(MINUTE,6,GETDATE()) AND dImportedOn >= DATEADD(MINUTE,-15,GETDATE()) AND [iStatus]='" + sSrc + "'";
                        string sGetFifteen = "SELECT COUNT(ID) FROM " + EnumLeadTables.Leads.ToString() + " WHERE ((DATEDIFF(minute,[dImportedOn],GETDATE()))between 6 and 15) AND [iStatus]='" + sSrc + "'";
                        cmd.CommandText = sGetFifteen;
                        SqlDataReader reader2 = cmd.ExecuteReader();
                        while (reader2.Read())
                        {
                            lFifteen.Add(reader2.GetValue(0).ToString());
                        }
                        Con.Close();
                        Con.Open();
                        cmd.Connection = Con;
                        //string sGetThirty = "SELECT COUNT(ID) FROM Leads WHERE dImportedOn <= DATEADD(MINUTE,16,GETDATE()) AND dImportedOn >= DATEADD(MINUTE,-30,GETDATE()) AND [iStatus]='" + sSrc + "'";
                        string sGetThirty = "SELECT COUNT(ID) FROM " + EnumLeadTables.Leads.ToString() + " WHERE ((DATEDIFF(minute,[dImportedOn],GETDATE()))between 16 and 30) AND [iStatus]='" + sSrc + "'";
                        cmd.CommandText = sGetThirty;
                        SqlDataReader reader3 = cmd.ExecuteReader();
                        while (reader3.Read())
                        {
                            lThirty.Add(reader3.GetValue(0).ToString());
                        }
                        Con.Close();
                        Con.Open();
                        cmd.Connection = Con;
                        //string sGetSixty = "SELECT COUNT(ID) FROM Leads WHERE dImportedOn <= DATEADD(MINUTE,31,GETDATE()) AND dImportedOn >= DATEADD(MINUTE,-60,GETDATE()) AND [iStatus]='" + sSrc + "'";
                        string sGetSixty = "SELECT COUNT(ID) FROM " + EnumLeadTables.Leads.ToString() + " WHERE ((DATEDIFF(minute,[dImportedOn],GETDATE()))between 31 and 60) AND [iStatus]='" + sSrc + "'";
                        cmd.CommandText = sGetSixty;
                        SqlDataReader reader4 = cmd.ExecuteReader();
                        while (reader4.Read())
                        {
                            lSixty.Add(reader4.GetValue(0).ToString());
                        }
                        Con.Close();
                        Con.Open();
                        cmd.Connection = Con;
                        //string sGetHour = "SELECT COUNT(ID) FROM Leads WHERE dImportedOn <= DATEADD(HOUR,1,GETDATE()) AND [iStatus]='" + sSrc + "'";
                        string sGetHour = "SELECT COUNT(ID) FROM " + EnumLeadTables.Leads.ToString() + " WHERE DATEDIFF(HOUR,dImportedOn,GETDATE()) >=1 AND [iStatus]='" + sSrc + "'";
                        cmd.CommandText = sGetHour;
                        SqlDataReader reader5 = cmd.ExecuteReader();
                        while (reader5.Read())
                        {
                            lOneHr.Add(reader5.GetValue(0).ToString());
                        }
                        Con.Close();
                        Con.Open();
                        cmd.Connection = Con;
                        //string sGetDay = "SELECT COUNT(ID) FROM Leads WHERE dImportedOn <= DATEADD(DAY,1,GETDATE()) AND [iStatus]='" + sSrc + "'";
                        string sGetDay = "SELECT COUNT(ID) FROM " + EnumLeadTables.Leads.ToString() + " WHERE DATEDIFF(DAY,dImportedOn,GETDATE()) >=1 AND [iStatus]='" + sSrc + "'";
                        cmd.CommandText = sGetDay;
                        SqlDataReader reader6 = cmd.ExecuteReader();
                        while (reader6.Read())
                        {
                            lOneDay.Add(reader6.GetValue(0).ToString());
                        }
                        Con.Close();
                    }
                }
                iListValues.Add(lSources);
                iListValues.Add(lFive);
                iListValues.Add(lFifteen);
                iListValues.Add(lThirty);
                iListValues.Add(lSixty);
                iListValues.Add(lOneHr);
                iListValues.Add(lOneDay);
            }
            catch (Exception ex)
            {

            }
            return iListValues;
        }

        public List<List<string>> GetLeadLife()
        {
            var iListValues = new List<List<string>>();
            try
            {
                var a = new System.Data.DataTable();
                var lStageID = new List<string>();
                List<string> lActioned = new List<string>();
                List<string> lQuoted = new List<string>();
                List<string> lConverted = new List<string>();
                lActioned.Add("Actioned");
                lQuoted.Add("Quoted");
                lConverted.Add("Converted");
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand();
                    Con.Open();
                    cmd.Connection = Con;
                    string sGetSourceID = "SELECT [ID] FROM [dbo].[Stages] WHERE [Name]='Actioned' UNION SELECT [ID] FROM [dbo].[Stages] WHERE [Name]='Converted' UNION SELECT [ID] FROM [dbo].[Stages] WHERE [Name]='Quoted'";
                    cmd.CommandText = sGetSourceID;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        lStageID.Add(reader.GetValue(0).ToString());
                    }
                    //reader.Close();
                    Con.Close();
                    List<string> lTime = new List<string>();
                    lTime.Add("");
                    lTime.Add("1-5 Mins");
                    lTime.Add("6-15 Mins");
                    lTime.Add("16-30 Mins");
                    lTime.Add("31-60 Mins");
                    lTime.Add("1 Hour Plus");
                    lTime.Add("1 Day plus");
                    string sFive = "", sFifteen = "", sThirty = "", sSixty = "", sOnehr = "", sOneDay = "";
                    foreach (var sSrc in lStageID)
                    {
                        string sStage = "";
                        Con.Open();
                        cmd.Connection = Con;
                        string sGetStage = "SELECT [Name] FROM Stages WHERE [ID]='" + sSrc + "'";
                        cmd.CommandText = sGetStage;
                        SqlDataReader readern = cmd.ExecuteReader();
                        while (readern.Read())
                        {
                            sStage = readern.GetValue(0).ToString();
                        }
                        Con.Close();

                        Con.Open();
                        cmd.Connection = Con;
                        string sGetFive = "SELECT COUNT(ID) FROM " + EnumLeadTables.Leads.ToString() + " WHERE ((DATEDIFF(minute,[dImportedOn],GETDATE()))between 0 and 5) AND [iStatus]='" + sSrc + "'";
                        cmd.CommandText = sGetFive;
                        SqlDataReader reader1 = cmd.ExecuteReader();
                        while (reader1.Read())
                        {
                            sFive = reader1.GetValue(0).ToString();
                        }
                        Con.Close();

                        Con.Open();
                        cmd.Connection = Con;
                        string sGetFifteen = "SELECT COUNT(ID) FROM " + EnumLeadTables.Leads.ToString() + " WHERE((DATEDIFF(minute,[dImportedOn], GETDATE()))between 6 and 15) AND[iStatus] = '" + sSrc + "'";
                        cmd.CommandText = sGetFifteen;
                        SqlDataReader reader2 = cmd.ExecuteReader();
                        while (reader2.Read())
                        {
                            sFifteen = reader2.GetValue(0).ToString();
                        }
                        Con.Close();
                        Con.Open();
                        cmd.Connection = Con;
                        string sGetThirty = "SELECT COUNT(ID) FROM " + EnumLeadTables.Leads.ToString() + " WHERE ((DATEDIFF(minute,[dImportedOn],GETDATE()))between 16 and 30) AND [iStatus]='" + sSrc + "'";
                        cmd.CommandText = sGetThirty;
                        SqlDataReader reader3 = cmd.ExecuteReader();
                        while (reader3.Read())
                        {
                            sThirty = reader3.GetValue(0).ToString();
                        }
                        Con.Close();
                        Con.Open();
                        cmd.Connection = Con;
                        string sGetSixty = "SELECT COUNT(ID) FROM " + EnumLeadTables.Leads.ToString() + " WHERE ((DATEDIFF(minute,[dImportedOn],GETDATE()))between 31 and 60) AND [iStatus]='" + sSrc + "'";
                        cmd.CommandText = sGetSixty;
                        SqlDataReader reader4 = cmd.ExecuteReader();
                        while (reader4.Read())
                        {
                            sSixty = reader4.GetValue(0).ToString();
                        }
                        Con.Close();
                        Con.Open();
                        cmd.Connection = Con;
                        string sGetHour = "SELECT COUNT(ID) FROM " + EnumLeadTables.Leads.ToString() + " WHERE DATEDIFF(HOUR,dImportedOn,GETDATE()) >=1 AND [iStatus]='" + sSrc + "'";
                        cmd.CommandText = sGetHour;
                        SqlDataReader reader5 = cmd.ExecuteReader();
                        while (reader5.Read())
                        {
                            sOnehr = reader5.GetValue(0).ToString();
                        }
                        Con.Close();
                        Con.Open();
                        cmd.Connection = Con;
                        string sGetDay = "SELECT COUNT(ID) FROM " + EnumLeadTables.Leads.ToString() + " WHERE DATEDIFF(DAY,dImportedOn,GETDATE()) >=1 AND [iStatus]='" + sSrc + "'";
                        cmd.CommandText = sGetDay;
                        SqlDataReader reader6 = cmd.ExecuteReader();
                        while (reader6.Read())
                        {
                            sOneDay = reader6.GetValue(0).ToString();
                        }
                        Con.Close();
                        if (sStage == "Actioned")
                        {
                            lActioned.Add(sFive);
                            lActioned.Add(sFifteen);
                            lActioned.Add(sThirty);
                            lActioned.Add(sSixty);
                            lActioned.Add(sOnehr);
                            lActioned.Add(sOneDay);
                        }
                        else if (sStage == "Quoted")
                        {
                            lQuoted.Add(sFive);
                            lQuoted.Add(sFifteen);
                            lQuoted.Add(sThirty);
                            lQuoted.Add(sSixty);
                            lQuoted.Add(sOnehr);
                            lQuoted.Add(sOneDay);
                        }
                        else if (sStage == "Converted")
                        {
                            lConverted.Add(sFive);
                            lConverted.Add(sFifteen);
                            lConverted.Add(sThirty);
                            lConverted.Add(sSixty);
                            lConverted.Add(sOnehr);
                            lConverted.Add(sOneDay);
                        }
                    }
                    iListValues.Add(lTime);
                    iListValues.Add(lActioned);
                    iListValues.Add(lQuoted);
                    iListValues.Add(lConverted);
                }
            }
            catch (Exception ex)
            {

            }
            return iListValues;
        }

        public List<List<string>> GetCLassAndSource()
        {
            var iListValues = new List<List<string>>();
            try
            {
                var a = new System.Data.DataTable();
                var lSourceID = new List<string>();
                var lClassID = new List<string>();
                var lClassDetails = new List<string>();
                List<string> lSources = new List<string>();
                lSources.Add("");
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand();
                    Con.Open();
                    cmd.Connection = Con;
                    string sGetSourceID = "SELECT [ID],[Name] FROM [dbo].[OrganizationSources]";
                    cmd.CommandText = sGetSourceID;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        lSourceID.Add(reader.GetValue(0).ToString());
                        lSources.Add(reader.GetValue(1).ToString());
                    }
                    reader.Close();
                    Con.Close();

                    iListValues.Add(lSources);
                    Con.Open();
                    cmd.Connection = Con;
                    string sGetClassID = "SELECT [ClassID],[Class] FROM [dbo].[OrganizationClasses]";
                    cmd.CommandText = sGetClassID;
                    SqlDataReader reader1 = cmd.ExecuteReader();
                    while (reader1.Read())
                    {
                        lClassID.Add(reader1.GetValue(0).ToString());
                        lClassDetails.Add(reader1.GetValue(1).ToString());
                    }
                    reader1.Close();
                    Con.Close();

                    for (int i = 0; i < lClassID.Count; i++)
                    {

                        var lClassCount = new List<string>();
                        if (lClassCount.Count == 0)
                        {
                            lClassCount.Add(lClassDetails[i]);
                        }
                        for (int j = 0; j < lSourceID.Count; j++)
                        {
                            Con.Open();
                            cmd.Connection = Con;
                            string sGetIDCount = "SELECT Count(ID) FROM [dbo].[Leads] WHERE [FKiSourceID]='" + lSourceID[j] + "' AND FKiLeadClassID='" + lClassID[i] + "'";
                            cmd.CommandText = sGetIDCount;
                            SqlDataReader reader3 = cmd.ExecuteReader();
                            while (reader3.Read())
                            {
                                lClassCount.Add(reader3.GetValue(0).ToString());
                            }
                            reader3.Close();
                            Con.Close();

                        }
                        iListValues.Add(lClassCount);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return iListValues;
        }

        //public List<VMDashReports> GetDashboardReports(string database)
        //{
        //    Reports Report = dbcontext.Reports.Find(27);
        //    Common Com = new Common();
        //    var Keys = Com.GetForeginkeyValues(Report.Query);
        //    foreach (var items in Keys)
        //    {

        //    }
        //    List<string[]> results = new List<string[]>();
        //    SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString());
        //    Con.Open();
        //    Con.ChangeDatabase(database);
        //    SqlCommand cmd = new SqlCommand();
        //    cmd.Connection = Con;
        //    cmd.CommandText = Report.Query;
        //    //cmd.ExecuteNonQuery();
        //    SqlDataReader reader = cmd.ExecuteReader();
        //    int count = reader.FieldCount;
        //    string[] rows = new string[count];
        //    string[] value = null;
        //    while (reader.Read())
        //    {
        //        List<string> values = new List<string>();

        //        for (int i = 0; i < count; i++)
        //        {
        //            if (!reader.IsDBNull(i))
        //            {
        //                values.Add(reader.GetValue(i).ToString());
        //            }
        //            else
        //            {
        //                values.Add("0");
        //            }
        //        }
        //        string[] result = values.ToArray();
        //        results.Add(result);
        //        value = result;
        //    }
        //    DataContext Spdb = new DataContext(database);
        //    List<string> Headings = new List<string>();
        //    List<VMDashReports> DReports = new List<VMDashReports>();
        //    foreach (var items in results)
        //    {
        //        VMDashReports Rep = new VMDashReports();
        //        int SorID = Convert.ToInt32(items[0]);
        //        string SorceName = Spdb.OrganizationSources.Where(m => m.ID == SorID).Select(m => m.Name).FirstOrDefault();
        //        Rep.Heading = SorceName;
        //        Headings.Add(SorceName);
        //        int SgID = Convert.ToInt32(items[1]);
        //        string SgName = dbcontext.Stages.Where(m => m.ID == SgID).Select(m => m.Name).FirstOrDefault();
        //        Rep.Status = SgName;
        //        //Left.Add(SgName);
        //        Rep.TCount = Convert.ToInt32(items[2]);
        //        //Counts.Add(Convert.ToInt32(items[2]));
        //        DReports.Add(Rep);
        //    }
        //    List<VMDashReports> Final = new List<VMDashReports>();
        //    List<string> Stats = new List<string>();
        //    Headings = Headings.Distinct().ToList();
        //    foreach (var items in DReports)
        //    {

        //        if (Stats.Contains(items.Status))
        //        {

        //        }
        //        else
        //        {
        //            VMDashReports repp = new VMDashReports();
        //            List<string> Heads = new List<string>();
        //            List<int> Counts = new List<int>();
        //            var res = DReports.Where(m => m.Status == items.Status).ToList();
        //            int i = 0;
        //            foreach (var item in res)
        //            {
        //                if (i == 0)
        //                {
        //                    foreach (var head in Headings)
        //                    {
        //                        var xxss = res.Where(m => m.Heading == head).FirstOrDefault();
        //                        if (xxss != null)
        //                        {
        //                            Counts.Add(xxss.TCount);
        //                        }
        //                        else
        //                        {
        //                            Counts.Add(0);
        //                        }
        //                        i++;
        //                    }
        //                }
        //            }
        //            repp.Status = items.Status;
        //            repp.Heads = Heads;
        //            repp.Counts = Counts;
        //            Final.Add(repp);
        //        }
        //        Stats.Add(items.Status);
        //    }
        //    Final.FirstOrDefault().Headings = Headings.Distinct().ToList();
        //    Con.Close();
        //    return Final;
        //}

        //public List<VMDashReports> GetClassSource(string database)
        //{
        //    DataContext Spdb = new DataContext(database);
        //    //27-Stages vs Source, 133-Class vs Source, 134-Class vs Stages
        //    Reports Report = dbcontext.Reports.Find(172);
        //    Common Com = new Common();
        //    var Keys = Com.GetForeginkeyValues(Report.Query);
        //    foreach (var items in Keys)
        //    {

        //    }          
        //             List<string[]> results = new List<string[]>();
        //    SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString());
        //    Con.Open();
        //    Con.ChangeDatabase(database);
        //    SqlCommand cmd = new SqlCommand();
        //    cmd.Connection = Con;
        //    cmd.CommandText = Report.Query;
        //    //cmd.ExecuteNonQuery();
        //    SqlDataReader reader = cmd.ExecuteReader();
        //    int count = reader.FieldCount;
        //    string[] rows = new string[count];
        //    string[] value = null;
        //    while (reader.Read())
        //    {
        //        List<string> values = new List<string>();

        //        for (int i = 0; i < count; i++)
        //        {
        //            if (!reader.IsDBNull(i))
        //            {
        //                values.Add(reader.GetValue(i).ToString());
        //            }
        //            else
        //            {
        //                values.Add("0");
        //            }
        //        }
        //        string[] result = values.ToArray();
        //        results.Add(result);
        //        value = result;
        //    }

        //    List<string> Headings = new List<string>();
        //    List<VMDashReports> DReports = new List<VMDashReports>();

        //    foreach (var items in results)
        //    {
        //        VMDashReports Rep = new VMDashReports();
        //        int SorID = Convert.ToInt32(items[0]);
        //        string SorceName = "";
        //        if (Keys[0].text == "FKiLeadClassID")
        //        {
        //             SorceName = Spdb.OrganizationClasses.Where(m => m.ClassID == SorID).Select(m => m.Class).FirstOrDefault();
        //        }
        //        else if (Keys[0].text == "FKiSourceID")
        //        {
        //             SorceName = Spdb.OrganizationSources.Where(m => m.ID == SorID).Select(m => m.Name).FirstOrDefault();
        //        }
        //        else if (Keys[0].text == "iStatus")
        //        {
        //            SorceName = dbcontext.Stages.Where(m => m.ID == SorID).Select(m => m.Name).FirstOrDefault();
        //        }
        //        Rep.Heading = SorceName;
        //        Headings.Add(SorceName);

        //        string SgName = "";
        //        int SgID = Convert.ToInt32(items[1]);
        //        if (Keys[1].text == "FKiLeadClassID")
        //        {
        //            SgName = Spdb.OrganizationClasses.Where(m => m.ClassID == SgID).Select(m => m.Class).FirstOrDefault();
        //        }
        //        else if (Keys[1].text == "FKiSourceID")
        //        {
        //            SgName = Spdb.OrganizationSources.Where(m => m.ID == SgID).Select(m => m.Name).FirstOrDefault();
        //        }
        //        else if (Keys[1].text == "iStatus")
        //        {
        //            SgName = dbcontext.Stages.Where(m => m.ID == SgID).Select(m => m.Name).FirstOrDefault();
        //        }
        //        Rep.Status = SgName;
        //        //Left.Add(SgName);
        //        Rep.TCount = Convert.ToInt32(items[2]);
        //        //Counts.Add(Convert.ToInt32(items[2]));
        //        DReports.Add(Rep);
        //    }
        //    List<VMDashReports> Final = new List<VMDashReports>();
        //    List<string> Stats = new List<string>();
        //    Headings = Headings.Distinct().ToList();
        //    foreach (var items in DReports)
        //    {

        //        if (Stats.Contains(items.Status))
        //        {

        //        }
        //        else
        //        {
        //            VMDashReports repp = new VMDashReports();
        //            List<string> Heads = new List<string>();
        //            List<int> Counts = new List<int>();
        //            var res = DReports.Where(m => m.Status == items.Status).ToList();
        //            int i = 0;
        //            foreach (var item in res)
        //            {
        //                if (i == 0)
        //                {
        //                    foreach (var head in Headings)
        //                    {
        //                        var xxss = res.Where(m => m.Heading == head).FirstOrDefault();
        //                        if (xxss != null)
        //                        {
        //                            Counts.Add(xxss.TCount);
        //                        }
        //                        else
        //                        {
        //                            Counts.Add(0);
        //                        }
        //                        i++;
        //                    }
        //                }
        //            }
        //            repp.Status = items.Status;
        //            repp.Heads = Heads;
        //            repp.Counts = Counts;
        //            Final.Add(repp);
        //        }
        //        Stats.Add(items.Status);
        //    Final.FirstOrDefault().Headings = Headings.Distinct().ToList();
        //    }
        //    Con.Close();
        //    return Final;
        //}


        public List<List<VMDashReports>> GetOneClickResult(int ReportID, int OrgID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var Report = dbContext.Reports.Find(ReportID);
            var database = dbCore.Organization.Where(m => m.ID == OrgID).Select(m => m.DatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(database);
            var BOName = dbContext.BOs.Where(m => m.BOID == Report.BOID).Select(m => m.Name).FirstOrDefault();
            List<object[]> Res = new List<object[]>();
            SqlConnection Con = new SqlConnection();
            using (Con)
            {
                if (BOName == EnumLeadTables.Reports.ToString())
                {
                    Con = new SqlConnection(ServiceUtil.GetConnectionString());
                    Con.Open();
                }
                else
                {
                    Con = new SqlConnection(ServiceUtil.GetClientConnectionString());
                    Con.Open();
                    Con.ChangeDatabase(database);
                }
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                cmd.CommandText = Report.Query;
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable data = new DataTable();
                data.Load(reader);
                List<object[]> TotalResult = data.AsEnumerable().Select(m => m.ItemArray).ToList();
                Res = TotalResult.ToList();
                Con.Close();
            }
            List<List<VMDashReports>> AllReports = new List<List<VMDashReports>>();
            foreach (var id in Res)
            {
                List<VMDashReports> Final = new List<VMDashReports>();
                int RepID = Convert.ToInt32(id[0]);
                var Rep = dbContext.Reports.Find(RepID);
                DataTable datad = new DataTable();
                List<object[]> TotalRes = new List<object[]>();
                SqlConnection Conn = new SqlConnection();
                using (Conn)
                {
                    Conn = new SqlConnection(ServiceUtil.GetClientConnectionString());
                    Conn.Open();
                    Conn.ChangeDatabase(database);
                    SqlCommand cmmd = new SqlCommand();
                    cmmd.Connection = Conn;
                    cmmd.CommandText = Rep.Query;
                    SqlDataReader reader1 = cmmd.ExecuteReader();
                    datad.Load(reader1);
                    TotalRes = datad.AsEnumerable().Select(m => m.ItemArray).ToList();
                    Conn.Close();
                }
                var Headingss = datad.Columns.ToString();
                List<string> HeadNames = new List<string>();
                HeadNames = (from dc in datad.Columns.Cast<DataColumn>()
                             select dc.ColumnName).ToList();
                var ColName = HeadNames[2];
                List<Targets> Targets = new List<Targets>();
                if (ColName == "Finance")
                {
                    Targets = Spdb.Targets.Where(m => m.ReportID == ReportID).ToList();
                }
                List<string> Headings = new List<string>();
                List<VMDashReports> DReports = new List<VMDashReports>();
                DReports = (from c in TotalRes
                            select new VMDashReports { TCount = Convert.ToInt32(c[2]), Heading = c[0].ToString(), Status = c[1].ToString() }).ToList();
                Headings.AddRange(TotalRes.Select(m => m[0].ToString()));
                List<string> Stats = new List<string>();
                Headings = Headings.Distinct().ToList();
                if (DReports.Count() > 0)
                {
                    foreach (var items in DReports)
                    {
                        if (Stats.Contains(items.Status))
                        {
                        }
                        else
                        {
                            VMDashReports repp = new VMDashReports();
                            List<string> Heads = new List<string>();
                            List<string> Counts = new List<string>();
                            List<int> IntCounts = new List<int>();
                            var res = DReports.Where(m => m.Status == items.Status).ToList();
                            int i = 0;
                            foreach (var item in res)
                            {
                                if (i == 0)
                                {
                                    foreach (var head in Headings)
                                    {
                                        var UserID = dbCore.XIAppUsers.Where(m => m.sFirstName == items.Status).Select(m => m.UserID).FirstOrDefault();
                                        var target = Targets.Where(m => m.UserID == UserID && m.ReportID == ReportID).FirstOrDefault();
                                        var xxss = res.Where(m => m.Heading == head).FirstOrDefault();
                                        if (xxss != null)
                                        {
                                            var FormattedCount = GetFormattedText(ColName, xxss.TCount, target, head, OrgID, sDatabase);
                                            Counts.Add(FormattedCount);
                                            IntCounts.Add(xxss.TCount);
                                        }
                                        else
                                        {
                                            var FormattedCount = GetFormattedText(ColName, 0, target, head, OrgID, sDatabase);
                                            Counts.Add(FormattedCount);
                                            IntCounts.Add(0);
                                        }
                                        i++;
                                    }
                                }
                            }
                            repp.Status = items.Status;
                            repp.Heads = Heads;
                            if (Report.IsRowTotal)
                            {
                                var FormattedCount = GetFormattedText(ColName, IntCounts.Sum(x => Convert.ToInt32(x)), null, null, OrgID, sDatabase);
                                Counts.Add(FormattedCount);
                                IntCounts.Add(IntCounts.Sum(x => Convert.ToInt32(x)));
                            }
                            repp.Counts = Counts;
                            repp.IntCounts = IntCounts;
                            Final.Add(repp);
                        }
                        Stats.Add(items.Status);
                    }
                }
                else
                {
                    VMDashReports repp = new VMDashReports();
                    repp.Counts = new List<string>();
                    Final.Add(repp);
                }
                if (Report.IsRowTotal)
                {
                    Headings.Add("Total");
                }
                if (Headings.Count() > 0)
                {
                    Final.FirstOrDefault().Headings = Headings;
                }
                else
                {
                    Final.FirstOrDefault().Headings = Headings;
                }
                List<string> ColTotals = new List<string>();
                var rowvals = Final.Select(m => m.IntCounts).ToList();
                if (Report.IsColumnTotal)
                {
                    var count = rowvals[0].Count();
                    for (int i = 0; i < count; i++)
                    {
                        var ColTotal = 0;
                        for (int j = 0; j < rowvals.Count(); j++)
                        {
                            ColTotal = ColTotal + Convert.ToInt32(rowvals[j][i]);
                        }
                        var FormattedCount = GetFormattedText(ColName, ColTotal, null, null, OrgID, sDatabase);
                        ColTotals.Add(FormattedCount.ToString());
                    }
                }
                Final.FirstOrDefault().ReportName = Rep.Name;
                AllReports.Add(Final);
            }
            return AllReports;
        }


        public List<VMDashReports> GetOneClickSummary(int ReportID, string Query, int OrgID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var Report = dbContext.Reports.Find(ReportID);
            var database = dbCore.Organization.Where(m => m.ID == OrgID).Select(m => m.DatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(database);
            List<string[]> results = new List<string[]>();
            var BOName = dbContext.BOs.Where(m => m.BOID == Report.BOID).Select(m => m.Name).FirstOrDefault();
            List<object[]> TotalResult = new List<object[]>();
            List<string> HeadNames = new List<string>();
            List<Targets> Targets = new List<Targets>();
            string ColName = string.Empty;
            SqlConnection Con = new SqlConnection();
            using (Con)
            {
                if (BOName == EnumLeadTables.Reports.ToString())
                {
                    Con = new SqlConnection(ServiceUtil.GetConnectionString());
                    Con.Open();
                }
                else
                {
                    Con = new SqlConnection(ServiceUtil.GetClientConnectionString());
                    Con.Open();
                    Con.ChangeDatabase(database);
                }
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                if (Query != null && Query.Length > 0)
                {
                    cmd.CommandText = Query;
                }
                else
                {
                    cmd.CommandText = Report.Query;
                }
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable data = new DataTable();
                data.Load(reader);
                TotalResult = data.AsEnumerable().Select(m => m.ItemArray).ToList();
                var Lists = TotalResult.OfType<string>();
                var list = TotalResult.Select(m => m).ToList();
                var Headingss = data.Columns.ToString();
                HeadNames = (from dc in data.Columns.Cast<DataColumn>()
                             select dc.ColumnName).ToList();
                ColName = HeadNames[2];
                if (ColName == "Finance")
                {
                    Targets = Spdb.Targets.Where(m => m.ReportID == ReportID).ToList();
                }
                Con.Close();
            }
            List<string> Headings = new List<string>();
            List<VMDashReports> DReports = new List<VMDashReports>();
            DReports = (from c in TotalResult
                        select new VMDashReports { TCount = string.IsNullOrEmpty(c[2].ToString()) ? 0 : Convert.ToInt32(c[2]), Heading = string.IsNullOrEmpty(c[0].ToString()) ? " " : c[0].ToString(), Status = string.IsNullOrEmpty(c[1].ToString()) ? "0" : c[1].ToString() }).ToList();
            Headings.AddRange(TotalResult.Select(m => m[0].ToString()));
            List<VMDashReports> Final = new List<VMDashReports>();
            List<string> Stats = new List<string>();
            Headings = Headings.Distinct().ToList();
            if (DReports.Count() > 0)
            {
                foreach (var items in DReports)
                {
                    if (Stats.Contains(items.Status))
                    {
                    }
                    else
                    {
                        VMDashReports repp = new VMDashReports();
                        List<string> Heads = new List<string>();
                        List<string> Counts = new List<string>();
                        List<int> IntCounts = new List<int>();
                        var res = DReports.Where(m => m.Status == items.Status).ToList();
                        int i = 0;
                        foreach (var item in res)
                        {
                            if (i == 0)
                            {
                                foreach (var head in Headings)
                                {
                                    var UserID = dbCore.XIAppUsers.Where(m => m.sFirstName == items.Status).Select(m => m.UserID).FirstOrDefault();
                                    var target = Targets.Where(m => m.UserID == UserID && m.ReportID == ReportID).FirstOrDefault();
                                    var xxss = res.Where(m => m.Heading == head).FirstOrDefault();
                                    if (xxss != null)
                                    {
                                        var FormattedCount = GetFormattedText(ColName, xxss.TCount, target, head, OrgID, sDatabase);
                                        Counts.Add(FormattedCount);
                                        IntCounts.Add(xxss.TCount);
                                    }
                                    else
                                    {
                                        var FormattedCount = GetFormattedText(ColName, 0, target, head, OrgID, sDatabase);
                                        Counts.Add(FormattedCount);
                                        IntCounts.Add(0);
                                    }
                                    i++;
                                }
                            }
                        }
                        repp.Status = items.Status;
                        repp.Heads = Heads;
                        if (Report.IsRowTotal)
                        {
                            var FormattedCount = GetFormattedText(ColName, IntCounts.Sum(x => Convert.ToInt32(x)), null, null, OrgID, sDatabase);
                            Counts.Add(FormattedCount);
                            IntCounts.Add(IntCounts.Sum(x => Convert.ToInt32(x)));
                        }
                        repp.Counts = Counts;
                        repp.IntCounts = IntCounts;
                        Final.Add(repp);
                    }
                    Stats.Add(items.Status);
                }
            }
            else
            {
                VMDashReports repp = new VMDashReports();
                repp.Counts = new List<string>();
                Final.Add(repp);
            }
            if (Report.IsRowTotal)
            {
                Headings.Add("Total");
            }
            if (Headings.Count() > 0)
            {
                Final.FirstOrDefault().Headings = Headings;
            }
            else
            {
                Final.FirstOrDefault().Headings = Headings;
            }
            List<string> ColTotals = new List<string>();
            var rowvals = Final.Select(m => m.IntCounts).ToList();
            if (Report.IsColumnTotal)
            {
                var count = rowvals[0].Count();
                for (int i = 0; i < count; i++)
                {
                    var ColTotal = 0;
                    for (int j = 0; j < rowvals.Count(); j++)
                    {
                        ColTotal = ColTotal + Convert.ToInt32(rowvals[j][i]);
                    }
                    var FormattedCount = GetFormattedText(ColName, ColTotal, null, null, OrgID, sDatabase);
                    ColTotals.Add(FormattedCount.ToString());
                }
                VMDashReports Rep = new VMDashReports();
                Rep.Status = "Total";
                Rep.Counts = ColTotals;
                Final.Add(Rep);
            }
            Final.FirstOrDefault().HeadNames = HeadNames;
            Final.FirstOrDefault().ReportName = Report.Name;
            Final.FirstOrDefault().RowClickType = Report.OnRowClickType;
            Final.FirstOrDefault().RowClickValue = Report.OnRowClickValue;
            Final.FirstOrDefault().ColumnClickType = Report.OnColumnClickType;
            Final.FirstOrDefault().Column = Report.OnClickColumn;
            Final.FirstOrDefault().ColumnClickValue = Report.OnColumnClickValue;
            Final.FirstOrDefault().CellClickType = Report.OnCellClickType;
            Final.FirstOrDefault().CellClickValue = Report.OnCellClickValue;
            Final.FirstOrDefault().ColumnName = HeadNames[0];
            Final.FirstOrDefault().RowName = HeadNames[1];
            Final.FirstOrDefault().FinaceColumn = HeadNames[2];
            return Final;
        }

        private string GetFormattedText(string ColName, int TCount, Targets target, string User, int OrgID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            if (ColName == "Finance")
            {
                var Format = dbContext.BOFields.Where(m => m.Name == "iFinance").Select(m => m.Format).FirstOrDefault();
                CultureInfo rgi = new CultureInfo(Format);
                string totalValueCurrency = string.Format(rgi, "{0:c}", Convert.ToInt32(TCount));
                if (target != null)
                {
                    if (Convert.ToInt32(TCount) > target.Target)
                    {
                        return totalValueCurrency + "<span class='targetgreencolor'></span>";
                    }
                    else
                    {
                        var UserID = dbCore.XIAppUsers.Where(m => m.sFirstName == User).FirstOrDefault();
                        if (target.IsSMS)
                        {
                            //SendTargetAlert(Convert.ToInt32(TCount), target.Target, UserID, OrgID);
                        }
                        else if (target.IsEmail)
                        {
                            //SendTargetAlertEmail("", OrgID);
                        }
                        else if (target.IsNotification)
                        {
                            //SendTargetAlertNotification(UserID, OrgID);
                        }
                        return totalValueCurrency + "<span class='targetredcolor'></span>";
                    }
                }
                else
                {
                    return totalValueCurrency;
                }
            }
            else
            {
                return TCount.ToString();
            }
        }

        private void SendTargetAlertNotification(cXIAppUsers UserID, int OrgID)
        {
            var NotificationMsg = "Please check target";
            throw new NotImplementedException();
        }
        public string SendTargetAlert(int Actual, int Target, cXIAppUsers UserID, int OrgID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            try
            {
                var database = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
                var SMSDetails = dbContext.IOServerDetails.Where(m => m.OrganizationID == OrgID && m.Type == 2).FirstOrDefault();
                string PhNo = "9032781819";
                string Message = "Please check your target.";
                string BulkSMSPath = SMSDetails.SMSPath; //"http://login.bulksmsglobal.in/api/sendhttp.php?";
                string SMSAPIKey = SMSDetails.SMSAPIKey; //"4411Ac9v78SjO9b57adb887";
                string strResult = BulkSMSPath + "authkey=" + SMSAPIKey + "&mobiles=" + PhNo + "&message=" + Message + "&sender=TARGET" + "&route=6" + "&country=0";
                WebClient WebClient = new WebClient();
                System.IO.StreamReader reader = new System.IO.StreamReader(WebClient.OpenRead(strResult));
                dynamic ResultHTML = reader.ReadToEnd();
                return ResultHTML;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int SendTargetAlertEmail(string EmailID, int OrgID)
        {
            try
            {
                //Getting the Server Details from the Database                    
                var sDetails = dbContext.IOServerDetails.Where(m => m.OrganizationID == OrgID && m.Type == 1 && m.Category == 1).FirstOrDefault();
                string usern = "", pass = "", sender = "", security = "", serverName = "";
                int port = 0;
                usern = sDetails.UserName;
                pass = sDetails.Password;
                sender = sDetails.FromAddress;
                serverName = sDetails.ServerName;
                port = sDetails.Port;
                security = sDetails.Security;
                string username = HttpUtility.UrlEncode(usern);
                string password = HttpUtility.UrlEncode(pass);
                string messageBody = "Please Check Target";
                MailMessage msg = new MailMessage();
                msg.To.Add("raviteja.m@inativetech.com");
                msg.From = new MailAddress(sender);
                msg.Subject = "Welcome Mail";
                string html = @"<html><body>" + messageBody + "</body></html>";
                AlternateView altView = AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html);
                msg.AlternateViews.Add(altView);
                msg.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = serverName;
                smtp.Port = port;
                //for gmail
                smtp.EnableSsl = false;
                smtp.Credentials = new System.Net.NetworkCredential(usern, pass);
                smtp.Send(msg);
                //}
            }
            catch (Exception e)
            {

            }
            return 0;
        }


        //public List<VMDropDown> GetForeginkeyValues(string Query)
        //{
        //    List<VMDropDown> AllVlaues = new List<VMDropDown>();
        //    var IsClass = Query.IndexOf("FKiLeadClassID", StringComparison.OrdinalIgnoreCase);
        //    if (IsClass > 0)
        //    {
        //        AllVlaues.Add(new VMDropDown
        //        {
        //            text = "FKiLeadClassID",
        //            Value = IsClass
        //        });
        //    }
        //    var IsOrg = Query.IndexOf("FKiOrgID", StringComparison.OrdinalIgnoreCase);
        //    if (IsOrg > 0)
        //    {
        //        AllVlaues.Add(new VMDropDown
        //        {
        //            text = "FKiOrgID",
        //            Value = IsOrg
        //        });
        //    }
        //    var IsTeam = Query.IndexOf("iTeamID", StringComparison.OrdinalIgnoreCase);
        //    if (IsTeam > 0)
        //    {
        //        AllVlaues.Add(new VMDropDown
        //        {
        //            text = "iTeamID",
        //            Value = IsTeam
        //        });
        //    }
        //    var IsSource = Query.IndexOf("FKiSourceID", StringComparison.OrdinalIgnoreCase);
        //    if (IsSource > 0)
        //    {
        //        AllVlaues.Add(new VMDropDown
        //        {
        //            text = "FKiSourceID",
        //            Value = IsSource
        //        });
        //    }
        //    var IsStatus = Query.IndexOf("iStatus", StringComparison.OrdinalIgnoreCase);
        //    if (IsStatus > 0)
        //    {
        //        AllVlaues.Add(new VMDropDown
        //        {
        //            text = "iStatus",
        //            Value = IsStatus
        //        });
        //    }
        //    var IsUserID = Query.IndexOf("UserID", StringComparison.OrdinalIgnoreCase);
        //    if (IsUserID > 0)
        //    {
        //        AllVlaues.Add(new VMDropDown
        //        {
        //            text = "UserID",
        //            Value = IsUserID
        //        });
        //    }
        //    List<VMDropDown> SortedList = AllVlaues.OrderBy(o => o.Value).ToList();
        //    return SortedList;
        //}

        public string RunScheduler(string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            try
            {
                DateTime Today = DateTime.Now;
                var Hour = Today.ToString("HH:00");
                var Date = Convert.ToInt32(Today.ToString("dd"));
                var Week = Today.DayOfWeek.ToString();
                List<string> Dbs = new List<string>();
                var Orgs = dbCore.Organization.ToList();
                foreach (var org in Orgs)
                {
                    DataContext Spdb = new DataContext(org.DatabaseName);
                    //var Schedulers = (from c in Spdb.Schedulers.Where(m => m.OrganizationID == org.ID)
                    //                  //where c.Time == Hour
                    //                  join l in Spdb.SchedulersLogs on c.ID equals l.SchedulerID
                    //                  //where l.LastExecutedOn.ToString("HH::00") != Hour
                    //                  select c).ToList();
                    var Schedulers = Spdb.Schedulers.Where(m => m.OrganizationID == org.ID && m.Time == Hour).ToList();
                    Common Com = new Common();
                    foreach (var items in Schedulers)
                    {
                        try
                        {
                            var Log = Spdb.SchedulersLogs.Where(m => m.SchedulerID == items.ID).OrderByDescending(m => m.ID).Select(m => m.LastExecutedOn).FirstOrDefault();
                            var LastRunHour = Log.ToString("HH:00");
                            var LastRunDay = Log.DayOfWeek.ToString();
                            var LastRunDate = Convert.ToInt32(Log.ToString("dd"));
                            if (items.Period == "Daily" && items.Time == Hour)
                            {
                                if (LastRunHour != Hour)
                                {
                                    DoSchedulerTask(items, 0, org.ID, sDatabase);
                                    SaveSchedulerLog(items.ID, "Success", null, org.DatabaseName, items.OrganizationID);
                                }
                            }
                            else if (items.Period == "Weekly" && items.Day == Week && items.Time == Hour)
                            {
                                if (items.Day == LastRunDay && LastRunHour != Hour)
                                {
                                    DoSchedulerTask(items, 0, org.ID, sDatabase);
                                    SaveSchedulerLog(items.ID, "Success", null, org.DatabaseName, items.OrganizationID);
                                }
                            }
                            else if (items.Period == "Monthly" && items.Date == Date && items.Time == Hour)
                            {
                                if (LastRunHour != Hour && LastRunDate != Date)
                                {
                                    DoSchedulerTask(items, 0, org.ID, sDatabase);
                                    SaveSchedulerLog(items.ID, "Success", null, org.DatabaseName, items.OrganizationID);
                                }
                            }
                            //}
                        }
                        catch (Exception ex)
                        {
                            SaveSchedulerLog(items.ID, "Failure", ex.ToString(), org.DatabaseName, items.OrganizationID);
                        }
                    }
                }
                return "Ok";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private void SaveSchedulerLog(int SchID, string Status, string Error, string database, int OrgID)
        {
            DataContext Spdb = new DataContext(database);
            SchedulersLogs Log = new SchedulersLogs();
            Log.SchedulerID = SchID;
            Log.OrganizationID = OrgID;
            Log.LastExecutedOn = DateTime.Now;
            Log.Status = Status;
            Log.Error = Error;
            Spdb.SchedulersLogs.Add(Log);
            Spdb.SaveChanges();
        }

        private void DoSchedulerTask(Schedulers Task, int ReportID, int OrgID, string sDatabase)
        {
            Common Com = new Common();
            if (ReportID == 0)
            {
                ReportID = Task.ReportID;
            }
            if (Task.Type == "Email")
            {
                //string Table = "<!DOCTYPE html><html><head><meta charset='UTF-8'><style> .grafico { box-sizing: border-box; } .grafico { height: 200px; margin: 1rem auto; position: relative; width: 200px; } .recorte { border-radius: 50%; clip: rect(0px, 200px, 200px, 100px); height: 100%; position: absolute; width: 100%; } .quesito { border-radius: 50%; clip: rect(0px, 100px, 200px, 0px); height: 100%; position: absolute; width: 100%; font-family: monospace; font-size: 1.5rem; } .sombra { background-color: #fff; border-radius: 50%; box-shadow: 0 4px 7px rgba(0, 0, 0, 0.3); border: 5px solid #000; height: 100%; position: absolute; width: 100%; } #porcion1 { transform: rotate(0deg); } #porcion1 .quesito { background-color: rgba(0,0,255,.7); transform: rotate(70deg); } #porcion2 { transform: rotate(70deg); } #porcion2 .quesito { background-color: rgba(255,255,0,.7); transform: rotate(120deg); } #porcion3 { transform: rotate(-170deg); } #porcion3 .quesito { background-color: rgba(0,128,0,.7); transform: rotate(25deg); } #porcionFin { transform:rotate(-145deg); } #porcionFin .quesito { background-color: rgba(255,0,0,.7); transform: rotate(145deg); } #porcion1 .quesito:after { content: attr(data-rel); left: 25%; line-height: 5; position: absolute; top: 0; transform: rotate(-70deg); } #porcion2 .quesito:after { content: attr(data-rel); left: 15%; position: absolute; top: 30%; transform: rotate(-190deg); } #porcion3 .quesito:after { content: attr(data-rel); left: 35%; position: absolute; top: 4%; transform: rotate(70deg); } #porcionFin .quesito:after { content: attr(data-rel); left: 10%; position: absolute; top: 30%; } </style></head><body><div class='grafico'><div class='sombra'></div><div id='porcion1' class='recorte'><div class='quesito' data-rel='70'></div></div><div id='porcion2' class='recorte'><div class='quesito' data-rel='120'></div></div><div id='porcion3' class='recorte'><div class='quesito' data-rel='25'></div></div><div id='porcionFin' class='recorte'><div class='quesito' data-rel='145'></div></div></div></body></html>";
                //Table = Table + "<style> body, html { height: 100%; } body { display: flex; flex-direction: column; justify-content: center; align-items: center; font-family: 'fira-sans-2', Verdana, sans-serif; } #q-graph { display: block; /* fixes layout wonkiness in FF1.5 */ position: relative; width: 600px; height: 300px; margin: 1.1em 0 0; padding: 0; background: transparent; font-size: 11px; } #q-graph caption { caption-side: top; width: 600px; text-transform: uppercase; letter-spacing: .5px; top: -40px; position: relative; z-index: 10; font-weight: bold; } #q-graph tr, #q-graph th, #q-graph td { position: absolute; bottom: 0; width: 150px; z-index: 2; margin: 0; padding: 0; text-align: center; } #q-graph td { transition: all .3s ease; &:hover; { background-color: desaturate(#85144b, 100); opacity: .9; color: white; } } #q-graph thead tr { left: 100%; top: 50%; bottom: auto; margin: -2.5em 0 0 5em; } #q-graph thead th { width: 7.5em; height: auto; padding: 0.5em 1em; } #q-graph thead th.sent { top: 0; left: 0; line-height: 2; } #q-graph thead th.paid { top: 2.75em; line-height: 2; left: 0; } #q-graph tbody tr { height: 296px; padding-top: 2px; border-right: 1px dotted #C4C4C4; color: #AAA; } #q-graph #q1 { left: 0; } #q-graph #q2 { left: 150px; } #q-graph #q3 { left: 300px; } #q-graph #q4 { left: 450px; border-right: none; } #q-graph tbody th { bottom: -1.75em; vertical-align: top; font-weight: normal; color: #333; } #q-graph .bar { width: 60px; border: 1px solid; border-bottom: none; color: #000; } #q-graph .bar p { margin: 5px 0 0; padding: 0; opacity: .4; } #q-graph .sent { left: 13px; background-color: #39cccc; border-color: transparent; } #q-graph .paid { left: 77px; background-color: #7fdbff; border-color: transparent; } #ticks { position: relative; top: -300px; left: 2px; width: 596px; height: 300px; z-index: 1; margin-bottom: -300px; font-size: 10px; font-family: 'fira-sans-2', Verdana, sans-serif; } #ticks .tick { position: relative; border-bottom: 1px dotted #C4C4C4; width: 600px; } #ticks .tick p {  position: absolute;  left: -5em;  top: -0.8em;  margin: 0 0 0 0.5em; } </style>";
                //Table = Table + "<style> * { box-sizing: border-box; } .grafico { height: 200px; margin: 1rem auto; position: relative; width: 200px; } .recorte { border-radius: 50%; clip: rect(0px, 200px, 200px, 100px); height: 100%; position: absolute; width: 100%; } .quesito { border-radius: 50%; clip: rect(0px, 100px, 200px, 0px); height: 100%; position: absolute; width: 100%; font-family: monospace; font-size: 1.5rem; } .sombra { background-color: #fff; border-radius: 50%; box-shadow: 0 4px 7px rgba(0, 0, 0, 0.3); border: 5px solid #000; height: 100%; position: absolute; width: 100%; } #porcion1 { transform: rotate(0deg); } #porcion1 .quesito { background-color: rgba(162, 162, 228, 0.7); transform: rotate(70deg); } #porcion2 { transform: rotate(70deg); } #porcion2 .quesito { background-color: rgba(187, 187, 106, 0.7); transform: rotate(120deg); } #porcion3 { transform: rotate(-170deg); } #porcion3 .quesito { background-color: rgba(57, 154, 57, 0.7); transform: rotate(25deg); } #porcionFin { transform: rotate(-145deg); } #porcionFin .quesito { background-color: #337f92; transform: rotate(145deg); } #porcion1 .quesito:after { content: attr(data-rel); left: 25%; line-height: 5; position: absolute; top: 0; transform: rotate(-70deg); } #porcion2 .quesito:after { content: attr(data-rel); left: 15%; position: absolute; top: 30%; transform: rotate(-190deg); } #porcion3 .quesito:after { content: attr(data-rel); left: 35%; position: absolute; top: 4%; transform: rotate(70deg); } #porcionFin .quesito:after { content: attr(data-rel); left: 10%; position: absolute; top: 30%; } </style>";
                //Table = Table + "<style>body { background-color: #b2e9e4; } svg.graph { height: 500px; width: 800px; } svg.graph .grid { stroke: white; stroke-dasharray: 1 2; stroke-width: 1; } svg.graph .points { stroke: white; stroke-width: 3; } svg.graph .first_set { fill: #00554d; } svg.graph .surfaces { fill-opacity: 0.5; } svg.graph .grid.double { stroke-opacity: 0.4; } svg.graph .labels { font-family: Arial; font-size: 14px; kerning: 1; } svg.graph .labels.x-labels { text-anchor: middle; } svg.graph .labels.y-labels { text-anchor: end; } </style>";
                //Table = Table + "</head><body>";
                //Barchart
                //Table = "<div style='display: flex;flex-direction: column;justify-content: center;align-items: center;font-family: 'fira-sans-2', Verdana, sans-serif;' ><table id='q-graph' style='display: block;position: relative; width: 600px;height: 300px;margin: 1.1em 0 0;padding: 0;background: transparent;font-size: 11px;'><caption style='caption-side: top; width: 600px; text-transform: uppercase; letter-spacing: .5px; top: -40px; position: relative; z-index: 10; font-weight: bold;'>Quarterly Results</caption><thead><tr style='position: absolute; bottom: 0; width: 150px; z-index: 2; margin: 0; padding: 0; text-align: center; left: 100%; top: 50%; margin: -2.5em 0 0 5em; bottom:auto;'><th style='position: absolute; bottom: 0; width: 150px; z-index: 2; margin: 0; padding: 0; text-align: center; width: 7.5em; height: auto; padding: 0.5em 1em; '></th><th style='position: absolute; bottom: 0; width: 150px; z-index: 2; margin: 0; padding: 0; text-align: center; width: 7.5em; height: auto; padding: 0.5em 1em; top: 0; left: 0; line-height: 2; left: 13px; background-color: #39cccc; border-color: transparent; ' class='sent'>Invoiced</th><th style='position: absolute; bottom: 0; width: 150px; z-index: 2; margin: 0; padding: 0; text-align: center; width: 7.5em; height: auto; padding: 0.5em 1em; top: 2.75em; line-height: 2; left: 0; left: 77px; background-color: #7fdbff; border-color: transparent; ' class='paid'>Collected</th></tr></thead><tbody><tr style='position: absolute; bottom: 0; width: 150px; z-index: 2; margin: 0; padding: 0; text-align: center; height: 296px; padding-top: 2px; border-right: 1px dotted #C4C4C4; color: #AAA; left: 0; 'class='qtr' id='q1'><th style='position: absolute; bottom: 0; width: 150px; z-index: 2; margin: 0; padding: 0; text-align: center;'scope='row'>Q1</th><td style='position: absolute; bottom: 0; width: 150px; z-index: 2; margin: 0; padding: 0; text-align: center; height: 111px; width: 60px; border: 1px solid; border-bottom: none; color: #000;' class='sent bar'><p style='margin: 5px 0 0;padding: 0;opacity: .4;'>$18,450.00</p></td><td style='position: absolute; bottom: 0; width: 150px; z-index: 2; margin: 0; padding: 0; text-align: center; height: 99px; width: 60px; border: 1px solid; border-bottom: none; color: #000;' class='paid bar'><p style='margin: 5px 0 0;padding: 0;opacity: .4;'>$16,500.00</p></td></tr><tr style='position: absolute; bottom: 0; width: 150px; z-index: 2; margin: 0; padding: 0; text-align: center; height: 296px; padding-top: 2px; border-right: 1px dotted #C4C4C4; color: #AAA; left: 150px; 'class='qtr' id='q2'><th style='position: absolute; bottom: 0; width: 150px; z-index: 2; margin: 0; padding: 0; text-align: center;'scope='row'>Q2</th><td style='position: absolute; bottom: 0; width: 150px; z-index: 2; margin: 0; padding: 0; text-align: center; height: 206px; width: 60px; border: 1px solid; border-bottom: none; color: #000;' class='sent bar'><p style='margin: 5px 0 0;padding: 0;opacity: .4;'>$34,340.72</p></td><td style='position: absolute; bottom: 0; width: 150px; z-index: 2; margin: 0; padding: 0; text-align: center; height: 194px; width: 60px; border: 1px solid; border-bottom: none; color: #000;' class='paid bar'><p style='margin: 5px 0 0;padding: 0;opacity: .4;'>$32,340.72</p></td></tr><tr style='position: absolute; bottom: 0; width: 150px; z-index: 2; margin: 0; padding: 0; text-align: center; height: 296px; padding-top: 2px; border-right: 1px dotted #C4C4C4; color: #AAA; left: 300px; 'class='qtr' id='q3'><th style='position: absolute; bottom: 0; width: 150px; z-index: 2; margin: 0; padding: 0; text-align: center;'scope='row'>Q3</th><td style='position: absolute; bottom: 0; width: 150px; z-index: 2; margin: 0; padding: 0; text-align: center; height: 259px; width: 60px; border: 1px solid; border-bottom: none; color: #000;' class='sent bar'><p style='margin: 5px 0 0;padding: 0;opacity: .4;'>$43,145.52</p></td><td style='position: absolute; bottom: 0; width: 150px; z-index: 2; margin: 0; padding: 0; text-align: center; height: 193px; width: 60px; border: 1px solid; border-bottom: none; color: #000;' class='paid bar'><p style='margin: 5px 0 0;padding: 0;opacity: .4;'>$32,225.52</p></td></tr><tr style='position: absolute; bottom: 0; width: 150px; z-index: 2; margin: 0; padding: 0; text-align: center; height: 296px; padding-top: 2px; border-right: 1px dotted #C4C4C4; color: #AAA; left: 450px; border-right: none; 'class='qtr' id='q4'><th style='position: absolute; bottom: 0; width: 150px; z-index: 2; margin: 0; padding: 0; text-align: center;'scope='row'>Q4</th><td style='position: absolute; bottom: 0; width: 150px; z-index: 2; margin: 0; padding: 0; text-align: center; height: 110px; width: 60px; border: 1px solid; border-bottom: none; color: #000;' class='sent bar'><p style='margin: 5px 0 0;padding: 0;opacity: .4;'>$18,415.96</p></td><td style='position: absolute; bottom: 0; width: 150px; z-index: 2; margin: 0; padding: 0; text-align: center; height: 195px; width: 60px; border: 1px solid; border-bottom: none; color: #000;' class='paid bar'><p style='margin: 5px 0 0;padding: 0;opacity: .4;'>$32,425.00</p></td></tr></tbody></table><div id='ticks' style='position: relative; top: -300px; left: 2px; width: 596px; height: 300px; z-index: 1; margin-bottom: -300px; font-size: 10px; font-family:'fira-sans-2',Verdana,sans-serif;'><div class='tick' style='height: 59px; position: relative; border-bottom: 1px dotted #C4C4C4; width: 600px;'><p style='position:absolute;left: -5em;top: -0.8em; margin: 0 0 0 0.5em;'>$50,000</p></div><div class='tick' style='height: 59px; position: relative; border-bottom: 1px dotted #C4C4C4; width: 600px;'><p style='position:absolute;left: -5em;top: -0.8em; margin: 0 0 0 0.5em;'>$40,000</p></div><div class='tick' style='height: 59px; position: relative; border-bottom: 1px dotted #C4C4C4; width: 600px;'><p style='position:absolute;left: -5em;top: -0.8em; margin: 0 0 0 0.5em;'>$30,000</p></div><div class='tick' style='height: 59px; position: relative; border-bottom: 1px dotted #C4C4C4; width: 600px;'><p style='position:absolute;left: -5em;top: -0.8em; margin: 0 0 0 0.5em;'>$20,000</p></div><div class='tick' style='height: 59px; position: relative; border-bottom: 1px dotted #C4C4C4; width: 600px;'><p style='position:absolute;left: -5em;top: -0.8em; margin: 0 0 0 0.5em;'>$10,000</p></div></div>";

                //Table = Table + "<br/><br/><br/>";
                ////PieChart
                //Table = Table + "<h4>Leads count with status</h4><br/>";
                //Table = Table + "<div class='grafico' style='height: 200px; margin: 1rem auto; position: relative; width: 200px;'><div class='sombra' style='background-color: #fff; border-radius: 50%; box-shadow: 0 4px 7px rgba(0, 0, 0, 0.3); border: 5px solid #000; height: 100%; position: absolute; width: 100%;'></div><div id='porcion1' class='recorte' style='transform: rotate(0deg); border-radius: 50%; clip: rect(0px, 200px, 200px, 100px); height: 100%; position: absolute; width: 100%;'><div class='quesito' style='border-radius: 50%; clip: rect(0px, 100px, 200px, 0px); height: 100%; position: absolute; width: 100%; font-family: monospace; font-size: 1.5rem; background-color: rgba(0,0,255,.7); transform: rotate(70deg); ' data-rel='70'></div></div><div id='porcion2' class='recorte' style=' transform: rotate(70deg); border-radius: 50%; clip: rect(0px, 200px, 200px, 100px); height: 100%; position: absolute; width: 100%;'><div class='quesito' style='border-radius: 50%; clip: rect(0px, 100px, 200px, 0px); height: 100%; position: absolute; width: 100%; font-family: monospace; font-size: 1.5rem; background-color: rgba(255,255,0,.7); transform: rotate(120deg); ' data-rel='120'></div></div><div id='porcion3' class='recorte' style='transform: rotate(-170deg); border-radius: 50%; clip: rect(0px, 200px, 200px, 100px); height: 100%; position: absolute; width: 100%; '><div class='quesito' style='border-radius: 50%; clip: rect(0px, 100px, 200px, 0px); height: 100%; position: absolute; width: 100%; font-family: monospace; font-size: 1.5rem; background-color: rgba(0,128,0,.7); transform: rotate(25deg); ' data-rel='25'></div></div><div id='porcionFin' class='recorte' style='transform: rotate(-145deg); border-radius: 50%; clip: rect(0px, 200px, 200px, 100px); height: 100%; position: absolute; width: 100%;'><div class='quesito' style='border-radius: 50%; clip: rect(0px, 100px, 200px, 0px); height: 100%; position: absolute; width: 100%; font-family: monospace; font-size: 1.5rem; background-color: rgba(255,0,0,.7); transform: rotate(145deg); ' data-rel='145'></div></div></div>";

                //Table = Table + "<br/><br/><br/>";
                ////Line Graph
                //Table = Table + "<h4>Leads count with months</h4><br/>";
                //Table = Table + "<svg class='graph' version='1.1' xmlns:xlink='http://www.w3.org/1999/xlink' xmlns='http://www.w3.org/2000/svg' style='height:500px;width:800px;'><g class='grid x-grid' id='xGrid' style='stroke: white;stroke-dasharray: 1 2;stroke-width: 1;'><line x1='113' x2='113' y1='10' y2='380'></line><line x1='259' x2='259' y1='10' y2='380'></line><line x1='405' x2='405' y1='10' y2='380'></line><line x1='551' x2='551' y1='10' y2='380'></line><line x1='697' x2='697' y1='10' y2='380'></line></g><g class='grid y-grid' id='yGrid'><line x1='86' x2='697' y1='10' y2='10'></line><line x1='86' x2='697' y1='68' y2='68'></line><line x1='86' x2='697' y1='126' y2='126'></line><line x1='86' x2='697' y1='185' y2='185'></line><line x1='86' x2='697' y1='243' y2='243'></line><line x1='86' x2='697' y1='301' y2='301'></line><line x1='86' x2='697' y1='360' y2='360'></line></g><g class='surfaces' style='fill-opacity: 0.5;'><path class='first_set' d='M113,360 L113,192 L259,171 L405,179 L551,200 L697,204 L697,360 Z' style='fill: #00554d;'></path></g><use class='grid double' xlink:href='#xGrid' style='stroke-opacity: 0.4;'></use><use class='grid double' xlink:href='#yGrid' style='stroke-opacity: 0.4;'></use><g class='first_set points' data-setname='Our first data set' style='stroke: white; stroke-width: 3; fill: #00554d;'><circle cx='113' cy='192' data-value='7.2' r='5'></circle><circle cx='259' cy='171' data-value='8.1' r='5'></circle><circle cx='405' cy='179' data-value='7.7' r='5'></circle><circle cx='551' cy='200' data-value='6.8' r='5'></circle><circle cx='697' cy='204' data-value='6.7' r='5'></circle></g><g class='labels x-labels' style='font-family: Arial; font-size: 14px; kerning: 1; text-anchor: middle;'><text x='113' y='400'>2008</text><text x='259' y='400'>2009</text><text x='405' y='400'>2010</text><text x='551' y='400'>2011</text><text x='697' y='400'>2012</text></g><g class='labels y-labels' style='font-family: Arial; font-size: 14px; kerning: 1; text-anchor: end; '><text x='80' y='15'>15</text><text x='80' y='131'>10</text><text x='80' y='248'>5</text><text x='80' y='365'>0</text><text x='50' y='15'>Weeks</text></g></svg>";

                //Table = Table + "<br/><br/><br/>";
                //Grids
                string Table = "<!DOCTYPE html><html><head><meta charset='UTF-8'></head><body>";
                var BOID = dbContext.Reports.Find(Task.ReportID);
                var BoName = dbContext.BOs.Where(m => m.BOID == BOID.BOID).Select(m => m.Name).FirstOrDefault();
                string MsgBody = "";
                if (BoName == EnumLeadTables.Reports.ToString())
                {
                    var Data = GetOneClickResult(Task.ReportID, OrgID, sDatabase);
                    foreach (var Rep in Data)
                    {
                        Table = Table + "<h4>" + Rep.FirstOrDefault().ReportName + "</h4><br/>";
                        Table = Table + "<table align='center' border='1' cellpadding='0' cellspacing='0' style='width:650px'><thead><tr><th></th>";
                        foreach (var items in Rep.FirstOrDefault().Headings)
                        {
                            Table = Table + "<th>" + items + "</th>";
                        }
                        Table = Table + "</tr></thead><tbody>";
                        foreach (var items in Rep)
                        {
                            Table = Table + "<tr><td>" + items.Status + "</td>";
                            foreach (var val in items.Counts)
                            {
                                Table = Table + "<td>" + val + "</td>";
                            }
                            Table = Table + "</tr>";
                        }
                        Table = Table + "</tbody></table><br/><br/>";
                    }
                    Table = Table + "</body></html>";
                    MsgBody = Table;
                }
                Com.SendMail(Task.UserID, OrgID, null, Task.EmailTemplateID, MsgBody, sDatabase, "sOrgName");
                //if (Report.DisplayAs == "Email Report")
                //{
                //    string Name = Report.Name.Replace(" ", "_");
                //    DataTable dt = new DataTable();
                //    dt = ExportDatatableContent(ReportID, OrgID, Task.UserID);
                //    string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                //    string str = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\images\\";
                //    Name = Name + DateTime.Now.TimeOfDay.Hours.ToString() + DateTime.Now.TimeOfDay.Minutes.ToString() + DateTime.Now.TimeOfDay.Seconds.ToString();
                //    using (FileStream stream = new FileStream(str + Name + ".xlsx", FileMode.Create, FileAccess.Write))
                //    {
                //        IWorkbook wb = new XSSFWorkbook();
                //        ISheet sheet = wb.CreateSheet("Sheet1");
                //        ICreationHelper cH = wb.GetCreationHelper();
                //        IRow rows = sheet.CreateRow(0);
                //        for (int j = 0; j < dt.Columns.Count; j++)
                //        {
                //            ICell cell = rows.CreateCell(j);
                //            String columnName = dt.Columns[j].ToString();
                //            cell.SetCellValue(columnName);
                //        }
                //        for (int i = 0; i < dt.Rows.Count; i++)
                //        {
                //            IRow row = sheet.CreateRow(i + 1);
                //            for (int j = 0; j < dt.Columns.Count; j++)
                //            {
                //                ICell cell = row.CreateCell(j);
                //                cell.SetCellValue(cH.CreateRichTextString(dt.Rows[i].ItemArray[j].ToString()));
                //            }
                //        }
                //        wb.Write(stream);
                //    }
                //    var Attachment = str + Name + ".xlsx";
                //    Com.SendMail(Task.UserID, OrgID, Attachment, Task.EmailTemplateID);
                //}
                //else
                //{
                //    Com.SendMail(Task.UserID, OrgID, null, Task.EmailTemplateID);
                //}
            }
            else if (Task.Type == "SMS")
            {
                Com.SendSMS(Task.UserID, OrgID, Task.SMSTemplateID, sDatabase, "sOrgName");
            }
        }

        public DataTable ExportDatatableContent(Guid ReportID, int OrgID, int UserID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var database = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            VMResultList vmquery = new VMResultList();
            DataContext Spdb = new DataContext(database);
            Reports query = dbContext.Reports.Find(ReportID);
            Common Com = new Common();
            var Heads = Com.GetHeadings(ReportID, database, OrgID, UserID, "sOrgName");
            string UserIDs = Com.GetSubUsers(UserID, OrgID, "", "sOrgName");
            string Query = ServiceUtil.ReplaceQueryContent(query.Query, UserIDs, UserID, OrgID, 0, 0);
            var BoName = dbContext.BOs.Where(m => m.BOID == query.BOID).Select(m => m.Name).FirstOrDefault();
            var Location = dbCore.XIAppUsers.Find(UserID);
            string BOName = dbContext.BOs.Where(m => m.BOID == query.BOID).Select(m => m.Name).FirstOrDefault();
            int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == UserID).Select(m => m.RoleID).FirstOrDefault();
            var Role = dbCore.XIAppRoles.Find(RoleID);
            if (Role.sRoleName != EnumRoles.SuperAdmin.ToString() && Role.sRoleName != EnumRoles.Admin.ToString() && BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
            {
                Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + OrgID);
                var LocCondition = "";
                var Locs = Location.sLocation.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var items in Locs)
                {
                    LocCondition = LocCondition + "OrgHeirarchyID='ORG" + OrgID + "_" + items + "' or ";
                }
                LocCondition = LocCondition.Substring(0, LocCondition.Length - 4);
                LocCondition = "(" + LocCondition + ")";
                Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
            }
            else if (BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
            {
                Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + OrgID);
                var LocCondition = "OrgHeirarchyID Like 'ORG" + OrgID + "_%'";
                Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
            }
            DataTable data = new DataTable();
            SqlConnection Con = new SqlConnection();
            using (Con)
            {
                if (BoName == EnumLeadTables.Reports.ToString())
                {
                    Con = new SqlConnection(ServiceUtil.GetConnectionString());
                    Con.Open();
                }
                else
                {
                    Con = new SqlConnection(ServiceUtil.GetClientConnectionString());
                    Con.Open();
                    Con.ChangeDatabase(database);
                }
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                cmd.CommandText = Query;
                SqlDataReader reader = cmd.ExecuteReader();
                data.Load(reader);
                for (int i = 0; i < Heads.Headings.Count(); i++)
                {
                    if (data.Columns[i].ToString() != Heads.Headings[i])
                    {
                        data.Columns[i].ColumnName = Heads.Headings[i];
                    }
                }
                Con.Close();
            }
            return data;
        }

        public DTResponse GetSchedulersLogList(jQueryDataTableParamModel param, int OrgID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var database = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(database);
            IQueryable<SchedulersLogs> AllLogs;
            AllLogs = Spdb.SchedulersLogs.Where(m => m.OrganizationID == OrgID);
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                //AllLogs = AllLogs.Where(m => m.Name.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllLogs.Count();
            AllLogs = QuerableUtil.GetResultsForDataTables(AllLogs, "", sortExpression, param);
            var clients = AllLogs.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     join s in Spdb.Schedulers on c.SchedulerID equals s.ID
                     join r in dbContext.Reports on s.ReportID equals r.ID
                     select new[] {
                                (i++).ToString(),c.ID.ToString(), r.Name, GetUserName(s.UserID,sDatabase), s.Period, s.Time, s.Type, s.LastExecutedOn.ToString(), c.Status};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        private string GetUserName(int p, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var UserName = dbCore.XIAppUsers.Find(p);
            return UserName.sFirstName;
        }


        ////This method is called from strored procedure api call
        //public string RunScheduler()
        //{
        //    try
        //    {
        //        //Getting today date, day and time details
        //        DateTime Today = DateTime.Now;
        //        var Hour = Today.ToString("HH:00");
        //        var Date = Convert.ToInt32(Today.ToString("dd"));
        //        var Week = Today.DayOfWeek.ToString();
        //        DataContext dbContext = new DataContext();
        //        //Getting list of scheduled reports from table
        //        var Schedulers = dbContext.Schedulers.ToList();
        //        foreach (var items in Schedulers)
        //        {
        //            try
        //            {
        //                //Getting last run details of scheduled report
        //                var Log = dbContext.SchedulersLogs.Where(m => m.SchedulerID == items.ID).OrderByDescending(m => m.ID).Select(m => m.LastExecutedOn).FirstOrDefault();
        //                var LastRunHour = Log.ToString("HH:00");
        //                var LastRunDay = Log.DayOfWeek.ToString();
        //                var LastRunDate = Convert.ToInt32(Log.ToString("dd"));
        //                //Checking whether the report is of type daily or weekly or monthly and current date, day and time matches with report scheduled time
        //                if (items.Period == "Daily" && items.Time == Hour)
        //                {
        //                    //checking whether scheduled job completed or not
        //                    if (LastRunHour != Hour)
        //                    {
        //                        //This method executes the scheduled job
        //                        DoSchedulerTask(items);
        //                        //Saving the status of job whether success or failure
        //                        SaveSchedulerLog(items.ID, "Success", null, items.OrganizationID);
        //                    }
        //                }                        
        //                else if (items.Period == "Weekly" && items.Day == Week && items.Time == Hour)
        //                {
        //                    if (items.Day == LastRunDay && LastRunHour != Hour)
        //                    {
        //                        DoSchedulerTask(items);
        //                        SaveSchedulerLog(items.ID, "Success", null, items.OrganizationID);
        //                    }
        //                }                        
        //                else if (items.Period == "Monthly" && items.Date == Date && items.Time == Hour)
        //                {
        //                    if (LastRunHour != Hour && LastRunDate != Date)
        //                    {
        //                        DoSchedulerTask(items);
        //                        SaveSchedulerLog(items.ID, "Success", null, items.OrganizationID);
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                SaveSchedulerLog(items.ID, "Failure", ex.ToString(), items.OrganizationID);
        //            }
        //        }
        //        return "Ok";
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.ToString();
        //    }
        //}

        ////Method is called from RunScheduler() Method
        //private void DoSchedulerTask(Schedulers Task)
        //{
        //    Common Com = new Common();
        //    var User = dbcontext.AspNetUsers.Find(Task.UserID);
        //    var Report = dbcontext.Reports.Find(Task.ReportID);
        //    //Checks whether task is email
        //    if (Task.Type == "Email")
        //    {
        //        //checking for any attachemnt along with email
        //        if (Report.DisplayAs == "Email Report")
        //        {
        //            string Name = Report.Name.Replace(" ", "_");
        //            DataTable dt = new DataTable();
        //            //Getting 1-click summery result to add in attachment
        //            dt = ExportDatatableContent(Task.ReportID, Task.OrganizationID, Task.UserID);
        //            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
        //            string str = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\images\\";
        //            Name = Name + DateTime.Now.TimeOfDay.Hours.ToString() + DateTime.Now.TimeOfDay.Minutes.ToString() + DateTime.Now.TimeOfDay.Seconds.ToString();
        //            //Creating attachment file
        //            using (FileStream stream = new FileStream(str + Name + ".xlsx", FileMode.Create, FileAccess.Write))
        //            {
        //                IWorkbook wb = new XSSFWorkbook();
        //                ISheet sheet = wb.CreateSheet("Sheet1");
        //                ICreationHelper cH = wb.GetCreationHelper();
        //                IRow rows = sheet.CreateRow(0);
        //                for (int j = 0; j < dt.Columns.Count; j++)
        //                {
        //                    ICell cell = rows.CreateCell(j);
        //                    String columnName = dt.Columns[j].ToString();
        //                    cell.SetCellValue(columnName);
        //                }
        //                for (int i = 0; i < dt.Rows.Count; i++)
        //                {
        //                    IRow row = sheet.CreateRow(i + 1);
        //                    for (int j = 0; j < dt.Columns.Count; j++)
        //                    {
        //                        ICell cell = row.CreateCell(j);
        //                        cell.SetCellValue(cH.CreateRichTextString(dt.Rows[i].ItemArray[j].ToString()));
        //                    }
        //                }
        //                wb.Write(stream);
        //            }
        //            var Attachment = str + Name + ".xlsx";
        //            //Sending mail with attachment
        //            Com.SendMail(Task.UserID, Task.OrganizationID, Attachment, Task.EmailTemplateID);
        //        }
        //        else
        //        {
        //            //sending mail without attachment
        //            Com.SendMail(Task.UserID, Task.OrganizationID, null, Task.EmailTemplateID);
        //        }
        //    }
        //    //Checks whether task is SMS
        //    else if (Task.Type == "SMS")
        //    {
        //        //Sending SMS to user
        //        Com.SendSMS(Task.UserID, Task.OrganizationID, Task.SMSTemplateID);
        //    }
        //}
        ////Saving schedulers log details like last executed time and status
        //private void SaveSchedulerLog(int SchID, string Status, string Error, int OrgID)
        //{
        //    DataContext Spdb = new DataContext();
        //    SchedulersLogs Log = new SchedulersLogs();
        //    Log.SchedulerID = SchID;
        //    Log.OrganizationID = OrgID;
        //    Log.LastExecutedOn = DateTime.Now;
        //    Log.Status = Status;
        //    Log.Error = Error;
        //    Spdb.SchedulersLogs.Add(Log);
        //    Spdb.SaveChanges();
        //}
    }
}
