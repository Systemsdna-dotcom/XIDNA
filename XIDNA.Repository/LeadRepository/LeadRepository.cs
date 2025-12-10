using System;
using System.Collections.Generic;
using System.Linq;
using XIDNA.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Globalization;
using XIDNA.ViewModels;
using System.Data.Entity;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections;
using System.Web;
using Excel;
using System.Data;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Reflection;

namespace XIDNA.Repository
{
    public class LeadRepository : ILeadRepository
    {
        CommonRepository Common = new CommonRepository();
        ModelDbContext dbcontext = new ModelDbContext();
        #region OldEmailReading
        public List<List<string>> ExtractEmailData(int SourceID, int OrgID, string database)
        {
            SourceID = 5557;
            DataContext dbcontext = new DataContext(database);
            ModelDbContext modeldb = new ModelDbContext();
            LeadInbounds inbound = new LeadInbounds();
            SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString());
            SqlCommand cmd = new SqlCommand();
            var lLeadID = new List<int>();
            var lSuccessDatas = new List<List<string>>();

            inbound = dbcontext.LeadInbounds.Where(m => m.SourceID == SourceID).FirstOrDefault();
            var sEmailContent = inbound.Content;
            int iInboundID = inbound.ID;
            //Replacing special characters to create the proper key value structure....
            //Add ":" to all the keys as many doesnt have...
            var ValuesRemoveNewLine = sEmailContent.Replace("\r\n\r\n", "\r\n");
            var ValuesRemoveNewLine1 = ValuesRemoveNewLine.Replace("\r\n  \r\n", "\r\n");
            var ValuesRemoveNewLine2 = ValuesRemoveNewLine1.Replace("\r\r\n", "\r\n");
            var ValuesRemoveNewLine3 = ValuesRemoveNewLine2.Replace(" \r\n", "\r\n");
            var ValuesRemoveNewLine4 = ValuesRemoveNewLine3.Replace("\r\n ", "\r\n");
            var ValuesRemoveDouble = ValuesRemoveNewLine4.Replace(",,", ",");
            //replace "'" single quotes with "''" to avoid database error..
            var ValuesReplaceQuote = ValuesRemoveDouble.Replace("'", "''");

            //Regex rgxSpace = new Regex("\\s\\s\\s+\r\n");
            Regex rgxSpace = new Regex(@"\s{2,}\r\n");
            string RemoveSpace = rgxSpace.Replace(ValuesReplaceQuote, "<>\r\n");

            Regex rgxSpace1 = new Regex("\t+\r\n");
            string RemoveSpace1 = rgxSpace1.Replace(RemoveSpace, "<>\r\n");

            Regex rgxn = new Regex(":\t+");
            string ValuesAddn = rgxn.Replace(RemoveSpace1, "\t");

            Regex rgx = new Regex("\\s\\s\\s+");
            string ValuesAdd = rgx.Replace(ValuesAddn, "::<>");

            var ValuesADD = ValuesAdd.Replace(":\r\n", "<>\r\n");

            Regex rgx1 = new Regex("::+");
            string ValuesAdd1 = rgx1.Replace(ValuesADD, "");

            Regex rgx2 = new Regex(":+\\s+");
            string ValuesAdd2 = rgx2.Replace(ValuesAdd1, "<>");
            //-------
            Regex rgx3 = new Regex(":>+? +");
            string ValuesAdd3 = rgx3.Replace(ValuesAdd2, "\t");

            var ValuesReplace1 = ValuesAdd3.Replace("\t", "<>");
            var ValuesReplace2 = ValuesReplace1.Replace(":+", ":");
            var ValuesReplace3 = ValuesReplace2.Replace(" -\r\n", "-");
            var ValuesReplace4 = ValuesReplace3.Replace(" - ", "<>");
            var ValuesReplace5 = ValuesReplace4.Replace(" :>", ":>");
            var ValuesReplace6 = ValuesReplace5.Replace(" :<>", "<>");
            //statically assigning "<>" for Quotezone
            var ReplaceFinalQuote = ValuesReplace6.Replace(":", "<>");
            var FinalReplace = ReplaceFinalQuote.Replace("<><>", "<>");
            var ValuesReplace7 = FinalReplace.Replace("DETAILS\r\n", "DETAILS<>\r\n");

            //BgInsurance convicted driver static splitting
            //var ValuesStatic = ValuesReplace7.Replace("Convicton Type", "\r\nConvicton Type");
            //var ValuesStatic1 = ValuesStatic.Replace("Own This Vehicle", "\r\nOwn This Vehicle");
            //var ValuesStatic2 = ValuesStatic1.Replace("Overnight\r\nStorage", "\r\nOvernight Storage");
            //var ValuesStatic3 = ValuesStatic2.Replace("No Claims Bonus", "\r\nNo Claims Bonus");
            //var ValuesStatic4 = ValuesStatic3.Replace("Year Annual Mileage", "\r\nYear Annual Mileage");
            //var ValuesStatic5 = ValuesStatic4.Replace("Own Another Car", "\r\nOwn Another Car");
            //var ValuesStatic6 = ValuesStatic5.Replace("Policy Start Date", "\r\nPolicy Start Date");
            //var ValuesStatic7 = ValuesStatic6.Replace("Created When", "\r\nCreated When");
            //remove "-----" in quotesearcher
            Regex rgx6 = new Regex("------+");
            string NewValues = rgx6.Replace(ValuesReplace7, "");

            //New instance Table
            int iResult = EmailExtractedIntoInstance(NewValues, iInboundID, SourceID, OrgID, database);
            var SleadDetails = EmailExtractedIntoLead(NewValues, iInboundID, SourceID, OrgID, database);
            //Insert to Lead Table


            Con.Close();
            return SleadDetails;
        }

        public int EmailExtractedIntoInstance(string NewValues, int iInboundID, int SourceID, int OrgID, string database)
        {
            DataContext dbcontext = new DataContext(database);
            ModelDbContext modeldb = new ModelDbContext();
            LeadInbounds inbound = new LeadInbounds();
            string sGetDataTypes = "";
            try
            {
                var keyValuePairs = NewValues.Trim().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                IList<string> KeyValuePairInstance = new List<string>();
                KeyValuePairInstance = NewValues.Trim().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                //check if the key values is in the next line with out ":".....
                for (int key = 0; key < keyValuePairs.Count; key++)
                {
                    if (keyValuePairs[key].Contains("<>"))
                    {
                        //nothing to be done...
                    }
                    else
                    {
                        //int j;
                        if (keyValuePairs[key].Contains("<>"))
                        {
                            //Do not Add...
                        }
                        else
                        {
                            //changes made : var ValuesAppend = keyValuePairs[key - 1] + " " + keyValuePairs[key] + "\r\n";/////removed ">" as it appends
                            var ValuesAppend = keyValuePairs[key - 1] + " " + keyValuePairs[key] + "\r\n";
                            string GetKey = keyValuePairs[key - 1];
                            var KeyIndex = KeyValuePairInstance.IndexOf(GetKey);
                            //replace old with new one
                            KeyValuePairInstance[KeyIndex] = ValuesAppend;
                            KeyValuePairInstance.RemoveAt(KeyIndex + 1);
                        }
                    }
                }
                KeyValuePairInstance.Add("FKiLeadClassID<>43");
                KeyValuePairInstance.Add("InBoundID<>" + iInboundID);
                KeyValuePairInstance.Add("dImportedOn<>" + DateTime.Now);
                KeyValuePairInstance.Add("FKiOrgID<>" + OrgID);
                KeyValuePairInstance.Add("FKiSourceID<>" + SourceID);
                string sFirstLeadTable = EnumLeadTables.LeadInstances.ToString();
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand();
                    Con.Open();
                    Con.ChangeDatabase(database);
                    cmd.Connection = Con;
                    sGetDataTypes = "INSERT INTO " + sFirstLeadTable + " (FKiSourceID) VALUES ('" + SourceID + "'); SELECT SCOPE_IDENTITY()";
                    cmd.CommandText = sGetDataTypes;
                    int iInstanceID = Convert.ToInt32(cmd.ExecuteScalar());
                    Con.Close();
                    foreach (var values in KeyValuePairInstance)
                    {
                        if (values.Contains("<>"))
                        {
                            var sKeyValues = Regex.Split(values, "<>");
                            var sKeyIn = sKeyValues[0];
                            var sValueIn = "";
                            if (sKeyValues[1].Contains(">"))
                            {
                                sValueIn = sKeyValues[1].Replace(">", " ");
                            }
                            else
                            {
                                sValueIn = sKeyValues[1];
                            }

                            if (sValueIn != "")
                            {
                                var sColumnName = "";
                                var sDataType = "";
                                MasterTemplates TemplateColumnData = modeldb.MasterTemplates.Where(m => m.ClassID == 43 && m.DataFieldName == sKeyIn).FirstOrDefault();
                                if (TemplateColumnData != null)
                                {
                                    sColumnName = TemplateColumnData.FieldName;
                                    sDataType = TemplateColumnData.FieldType;
                                }
                                else
                                {
                                    var MappedColumnData = dbcontext.MappedFields.Where(u => u.FieldName == sKeyIn).FirstOrDefault();
                                    if (MappedColumnData != null)
                                    {
                                        sColumnName = MappedColumnData.AddField;
                                        sDataType = MappedColumnData.FieldType;
                                    }
                                }
                                string sDataTypeValue = (sDataType).ToLower();
                                string sGetDate = "";
                                if (sColumnName != "" && sColumnName != null)
                                {
                                    //CHECKS for the DATETIME DATATYPE
                                    if (sDataTypeValue == "datetime")
                                    {
                                        if (sValueIn != "1/1/0001" && sValueIn != "01/01/0001")
                                        {
                                            if (sColumnName == "dImportedOn")
                                            {
                                                Con.Open();
                                                Con.ChangeDatabase(database);
                                                cmd.Connection = Con;
                                                sGetDataTypes = "Update [dbo].[" + sFirstLeadTable + "] SET [" + sColumnName + "] = '" + sValueIn + "' WHERE [FKiSourceID] ='" + SourceID + "' AND [ID]='" + iInstanceID + "'";
                                                cmd.CommandText = sGetDataTypes;
                                                cmd.ExecuteNonQuery();
                                                Con.Close();
                                            }
                                            else
                                            {
                                                DateTime datetime;
                                                string[] Pattern = { "d/M/yyyy hh:mm:ss", "d/M/yyyy", "M/d/yyyy hh:mm:ss tt", "M/d/yyyy" };
                                                foreach (var PatternType in Pattern)
                                                {
                                                    if (DateTime.TryParseExact(sValueIn, PatternType, null, DateTimeStyles.None, out datetime))
                                                    {
                                                        if (sColumnName == "dDOB")
                                                        {
                                                            sGetDate = datetime.ToString("MM/dd/yyyy 00:00:00.000");
                                                        }
                                                        else
                                                        {
                                                            sGetDate = datetime.ToString("MM/dd/yyyy hh:mm:ss tt");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //error message
                                                    }
                                                }
                                                Con.Open();
                                                Con.ChangeDatabase(database);
                                                cmd.Connection = Con;
                                                sGetDataTypes = "Update [dbo].[" + sFirstLeadTable + "] SET [" + sColumnName + "] = '" + sGetDate + "' WHERE [FKiSourceID] ='" + SourceID + "' AND [ID]='" + iInstanceID + "'";
                                                cmd.CommandText = sGetDataTypes;
                                                cmd.ExecuteNonQuery();
                                                Con.Close();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //Not datetime datatype

                                        Con.Open();
                                        Con.ChangeDatabase(database);
                                        cmd.Connection = Con;
                                        //TODO write insert Query..........
                                        sGetDataTypes = "Update [dbo].[" + sFirstLeadTable + "] SET [" + sColumnName + "] = '" + sValueIn + "' WHERE [FKiSourceID] ='" + SourceID + "' AND [ID]='" + iInstanceID + "'";
                                        cmd.CommandText = sGetDataTypes;
                                        cmd.ExecuteNonQuery();
                                        Con.Close();

                                    }
                                }
                            }
                            else
                            {
                                //Save null
                            }
                        }
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public List<List<string>> EmailExtractedIntoLead(string NewValues, int iInboundID, int SourceID, int OrgID, string database)
        {
            ModelDbContext modeldb = new ModelDbContext();
            DataContext dbcontext = new DataContext(database);
            SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString());
            SqlCommand cmd = new SqlCommand();
            var lLeadID = new List<long>();
            string ClientsTable = EnumLeadTables.LeadClients.ToString();
            var lSuccessDatas = new List<List<string>>();
            var lColValNull = new List<VMImproperData>();
            try
            {
                string GetDataType;
                var Key = "";
                var Value = "";
                string KeyValues;

                //Leads....
                var keyValuePairs = NewValues.Trim().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                List<string> NewKeyValuePair = new List<string>();
                NewKeyValuePair = NewValues.Trim().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                //check if the key values is in the next line with out ":".....
                for (int key = 0; key < keyValuePairs.Count; key++)
                {

                    if (keyValuePairs[key].Contains("<>"))
                    {
                        //nothing to be done...
                    }
                    else
                    {
                        //int j;
                        if (keyValuePairs[key].Contains("<>"))
                        {
                            //Do not Add...
                        }
                        else
                        {
                            var ValuesAppend = keyValuePairs[key - 1] + " " + keyValuePairs[key] + "\r\n";
                            string GetKey = keyValuePairs[key - 1];
                            var KeyIndex = NewKeyValuePair.IndexOf(GetKey);
                            //NewKeyValuePair.RemoveAt(key);
                            //replace old with new one
                            NewKeyValuePair[KeyIndex] = ValuesAppend;
                            NewKeyValuePair.RemoveAt(KeyIndex + 1);
                        }
                    }
                }

                //====Add SourceID and LeadClassID to the keyValuesPair
                LeadEngine Engine = new LeadEngine();
                NewKeyValuePair.Add("FKiSourceID<>" + SourceID);
                VMLeadEngine result = Engine.GetEmailLeadDetails(NewKeyValuePair, 42, database, 43);
                string sMainTable = EnumLeadTables.Leads.ToString();
                if (result.LeadID > 0)
                {
                    lLeadID.Add(result.LeadID);
                    //Con.Open();
                    //Con.ChangeDatabase(database);
                    //cmd.Connection = Con;
                    //GetDataType = "SELECT InBoundID FROM [dbo].[" + sMainTable + "] WHERE [FKiSourceID] = '" + SourceID + "'";
                    //cmd.CommandText = GetDataType;
                    //SqlDataReader sourcereader = cmd.ExecuteReader();
                    //while (sourcereader.Read())
                    //{
                    //    InboundID = sourcereader.GetInt32(0);
                    //}
                    //Con.Close();
                    NewKeyValuePair.Add("FKiLeadClassID<>43");
                    //=======

                    foreach (var items in NewKeyValuePair)
                    {
                        if (items.Contains("<>"))
                        {
                            var keyvalues = Regex.Split(items, "<>");
                            Key = keyvalues[0].TrimStart().TrimEnd();
                            Value = keyvalues[1].TrimStart().TrimEnd();
                        }
                        //
                        if (Value == "")
                        {
                            //---Gets the count in instance
                        }
                        else
                        {
                            var ColumnName = "";
                            var DataType = "";
                            MasterTemplates TemplateColumnData = modeldb.MasterTemplates.Where(m => m.ClassID == 43 && m.DataFieldName == Key).FirstOrDefault();
                            if (TemplateColumnData != null)
                            {
                                ColumnName = TemplateColumnData.FieldName;
                                DataType = TemplateColumnData.FieldType;
                            }
                            else
                            {
                                var MappedColumnData = dbcontext.MappedFields.Where(u => u.FieldName == Key).FirstOrDefault();
                                if (MappedColumnData != null)
                                {
                                    ColumnName = MappedColumnData.AddField;
                                    DataType = MappedColumnData.FieldType;
                                }
                            }

                            string DataTypeValue = (DataType).ToLower();
                            string GetDate = "";
                            if (ColumnName != "" && ColumnName != null)
                            {
                                //CHECKS for the DATETIME DATATYPE
                                if (DataTypeValue == "datetime")
                                {
                                    if (Value != "1/1/0001" && Value != "01/01/0001")
                                    {
                                        if (ColumnName == "dImportedOn")
                                        {
                                            Con.Open();
                                            Con.ChangeDatabase(database);
                                            cmd.Connection = Con;
                                            KeyValues = "Update [dbo].[" + sMainTable + "] SET [" + ColumnName + "] = '" + Value + "' WHERE [ID] ='" + result.LeadID + "'";
                                            cmd.CommandText = KeyValues;
                                            cmd.ExecuteNonQuery();
                                            Con.Close();
                                        }
                                        else
                                        {
                                            DateTime datetime;
                                            string[] Pattern = { "d/M/yyyy hh:mm:ss", "d/M/yyyy", "M/d/yyyy hh:mm:ss tt", "M/d/yyyy" };
                                            foreach (var PatternType in Pattern)
                                            {
                                                if (DateTime.TryParseExact(Value, PatternType, null, DateTimeStyles.None, out datetime))
                                                {
                                                    if (ColumnName == "dDOB")
                                                    {
                                                        GetDate = datetime.ToString("MM/dd/yyyy 00:00:00.000");
                                                    }
                                                    else
                                                    {
                                                        GetDate = datetime.ToString("MM/dd/yyyy hh:mm:ss tt");
                                                    }
                                                }
                                                else
                                                {
                                                    //error message
                                                }
                                            }
                                            Con.Open();
                                            cmd.Connection = Con;
                                            KeyValues = "Update [dbo].[" + sMainTable + "] SET [" + ColumnName + "] = '" + GetDate + "' WHERE [ID] ='" + result.LeadID + "'";
                                            cmd.CommandText = KeyValues;
                                            cmd.ExecuteNonQuery();
                                            Con.Close();
                                        }
                                    }
                                }
                                else
                                {
                                    //Not datetime datatype

                                    Con.Open();
                                    Con.ChangeDatabase(database);
                                    cmd.Connection = Con;
                                    KeyValues = "Update [dbo].[" + sMainTable + "] SET [" + ColumnName + "] = '" + Value + "' WHERE [ID] ='" + result.LeadID + "'";
                                    cmd.CommandText = KeyValues;
                                    cmd.ExecuteNonQuery();
                                    Con.Close();

                                }
                            }
                        }


                    }
                    if (result.bUpdateSource)
                    {
                        Con.Open();
                        Con.ChangeDatabase(database);
                        cmd.Connection = Con;
                        cmd.CommandText = "UPDATE " + sMainTable + " SET FKiSourceID =" + "'" + SourceID + "'" + " " + "WHERE" + " ID=" + result.LeadID + "";
                        cmd.ExecuteNonQuery();
                        Con.Close();
                    }
                    else
                    {
                        Con.Open();
                        Con.ChangeDatabase(database);
                        cmd.Connection = Con;
                        cmd.CommandText = "UPDATE " + sMainTable + " SET FKiSourceID =" + "'" + result.OldSourceID + "'" + " " + "WHERE" + " ID=" + result.LeadID + "";
                        cmd.ExecuteNonQuery();
                        Con.Close();
                    }
                }
                else
                {
                    NewKeyValuePair.Add("InBoundID<>" + iInboundID);
                    NewKeyValuePair.Add("dImportedOn<>" + DateTime.Now);
                    NewKeyValuePair.Add("FKiOrgID<>" + OrgID);
                    NewKeyValuePair.Add("FKiLeadClassID<>43");
                    int LeadID = 0;
                    string LeadsTable = EnumLeadTables.Leads.ToString();
                    Con.Open();
                    Con.ChangeDatabase(database);
                    cmd.Connection = Con;
                    GetDataType = "INSERT INTO " + LeadsTable + " (FKiSourceID) VALUES ('" + SourceID + "'); SELECT SCOPE_IDENTITY()";
                    cmd.CommandText = GetDataType;
                    LeadID = Convert.ToInt32(cmd.ExecuteScalar());
                    lLeadID.Add(LeadID);
                    Con.Close();
                    foreach (var items in NewKeyValuePair)
                    {
                        if (items.Contains("<>"))
                        {
                            var keyvalues = Regex.Split(items, "<>");
                            Key = keyvalues[0].TrimStart().TrimEnd();
                            Value = keyvalues[1].TrimStart().TrimEnd();
                        }

                        if (Value != "")
                        {
                            var ColumnName = "";
                            var DataType = "";
                            MasterTemplates TemplateColumnData = modeldb.MasterTemplates.Where(m => m.ClassID == 43 && m.DataFieldName == Key).FirstOrDefault();
                            if (TemplateColumnData != null)
                            {
                                ColumnName = TemplateColumnData.FieldName;
                                DataType = TemplateColumnData.FieldType;
                            }
                            else
                            {
                                var MappedColumnData = dbcontext.MappedFields.Where(u => u.FieldName == Key).FirstOrDefault();
                                if (MappedColumnData != null)
                                {
                                    ColumnName = MappedColumnData.AddField;
                                    DataType = MappedColumnData.FieldType;
                                }
                            }
                            string DataTypeValue = (DataType).ToLower();
                            string GetDate = "";
                            if (ColumnName != "" && ColumnName != null)
                            {
                                if (DataTypeValue == "datetime")
                                {
                                    if (Value != "1/1/0001" && Value != "01/01/0001")
                                    {
                                        if (ColumnName == "dImportedOn")
                                        {
                                            Con.Open();
                                            Con.ChangeDatabase(database);
                                            cmd.Connection = Con;
                                            KeyValues = "Update [dbo].[" + LeadsTable + "] SET [" + ColumnName + "] = '" + Value + "' WHERE [FKiSourceID] ='" + SourceID + "' AND [ID]='" + LeadID + "'";
                                            cmd.CommandText = KeyValues;
                                            cmd.ExecuteNonQuery();
                                            Con.Close();
                                        }
                                        else
                                        {
                                            DateTime datetime;
                                            string[] Pattern = { "d/M/yyyy hh:mm:ss", "d/M/yyyy", "M/d/yyyy hh:mm:ss", "M/d/yyyy" };
                                            foreach (var PatternType in Pattern)
                                            {
                                                if (DateTime.TryParseExact(Value, PatternType, null, DateTimeStyles.None, out datetime))
                                                {
                                                    if (ColumnName == "dDOB")
                                                    {
                                                        GetDate = datetime.ToString("MM/dd/yyyy 00:00:00.000");
                                                    }
                                                    else
                                                    {
                                                        GetDate = datetime.ToString("MM/dd/yyyy hh:mm:ss tt");
                                                    }
                                                }
                                                else
                                                {
                                                    //error message
                                                }
                                            }
                                            Con.Open();
                                            Con.ChangeDatabase(database);
                                            cmd.Connection = Con;
                                            KeyValues = "Update [dbo].[" + LeadsTable + "] SET [" + ColumnName + "] = '" + GetDate + "' WHERE [FKiSourceID] ='" + SourceID + "' AND [ID]='" + LeadID + "'";
                                            cmd.CommandText = KeyValues;
                                            cmd.ExecuteNonQuery();
                                            Con.Close();
                                        }
                                    }
                                }
                                else
                                {
                                    //Not datetime datatype
                                    Con.Open();
                                    Con.ChangeDatabase(database);
                                    cmd.Connection = Con;
                                    KeyValues = "Update [dbo].[" + LeadsTable + "] SET [" + ColumnName + "] = '" + Value + "' WHERE [FKiSourceID] ='" + SourceID + "' AND [ID]='" + LeadID + "'";
                                    cmd.CommandText = KeyValues;
                                    cmd.ExecuteNonQuery();
                                    Con.Close();

                                }
                            }
                        }
                        else
                        {
                            //Save Null value.
                        }
                    }
                    int iClientID = 0;
                    if (result.ClientID == 0)
                    {
                        string FirstName = "", LastName = "", Mobile = "", Email = "", ClassID = "";
                        Con.Open();
                        Con.ChangeDatabase(database);
                        cmd.Connection = Con;
                        GetDataType = "SELECT sForeName,sLastName,sMob,sEmail,FKiLeadClassID FROM [dbo].[" + LeadsTable + "] WHERE [FKiSourceID] = '" + SourceID + "'";
                        cmd.CommandText = GetDataType;
                        SqlDataReader clientreader = cmd.ExecuteReader();
                        while (clientreader.Read())
                        {
                            FirstName = clientreader.IsDBNull(0) ? null : clientreader.GetString(0);
                            LastName = clientreader.IsDBNull(1) ? null : clientreader.GetString(1);
                            Mobile = clientreader.IsDBNull(2) ? null : clientreader.GetString(2);
                            Email = clientreader.IsDBNull(3) ? null : clientreader.GetString(3);
                            ClassID = "43";
                        }
                        Con.Close();
                        Con.Open();
                        Con.ChangeDatabase(database);
                        cmd.Connection = Con;
                        KeyValues = "INSERT INTO " + ClientsTable + " (Name,Email,Mobile,ClassID,InBoundID) VALUES ('" + FirstName + " " + LastName + "','" + Email + "','" + Mobile + "','" + ClassID + "','" + iInboundID + "'); SELECT SCOPE_IDENTITY()";
                        cmd.CommandText = KeyValues;
                        iClientID = Convert.ToInt32(cmd.ExecuteScalar());
                        //cmd.ExecuteNonQuery();
                        Con.Close();

                    }
                    Con.Open();
                    Con.ChangeDatabase(database);
                    cmd.Connection = Con;
                    KeyValues = "UPDATE " + LeadsTable + " SET FKiClientID =" + "'" + iClientID + "'" + " " + "WHERE" + " ID=" + LeadID + "";
                    cmd.CommandText = KeyValues;
                    cmd.ExecuteNonQuery();
                    Con.Close();

                    //Priority
                    string sQueryPriority = modeldb.Reports.Where(m => m.Name == "Prority").Select(m => m.Query).FirstOrDefault();
                    if (sQueryPriority != null && sQueryPriority.Length > 0)
                    {
                        string sPriority = sQueryPriority.Replace("ID = 0", "ID = " + LeadID);
                        Con.Open();
                        Con.ChangeDatabase(database);
                        cmd.Connection = Con;
                        KeyValues = sPriority;
                        cmd.CommandText = KeyValues;
                        cmd.ExecuteNonQuery();
                        Con.Close();
                    }

                    //Finance
                    DataContext Spdb = new DataContext(database);
                    var SubScrpDetails = Spdb.OrganizationSubscriptions.Where(m => m.OrganizationID == OrgID && m.ClassID == 43).FirstOrDefault();
                    string sQueryFinance = "Update " + EnumLeadTables.Leads.ToString() + " Set iFinance =" + SubScrpDetails.LeadCost + ", OrgHeirarchyID ='ORG" + OrgID + "_" + SubScrpDetails.LocationCode.ToUpper() + "' Where ID =" + LeadID;
                    string sFinance = sQueryFinance;
                    Con.Open();
                    Con.ChangeDatabase(database);
                    cmd.Connection = Con;
                    KeyValues = sFinance;
                    cmd.CommandText = KeyValues;
                    cmd.ExecuteNonQuery();
                    Con.Close();

                    //Users
                    AssignUserID(LeadID, OrgID);
                    //string sQueryUsers = modeldb.Reports.Where(m => m.Name == "Assign Users").Select(m => m.Query).FirstOrDefault();
                    //string sAssignUsers = sQueryUsers.Replace("ID = 0", "ID = " + LeadID);
                    //Con.Open();
                    //Con.ChangeDatabase(database);
                    //cmd.Connection = Con;
                    //KeyValues = sAssignUsers;
                    //cmd.CommandText = KeyValues;
                    //cmd.ExecuteNonQuery();
                    //Con.Close();
                }
                Con.Open();
                Con.ChangeDatabase(database);
                cmd.Connection = Con;
                //change to LeadID
                KeyValues = "UPDATE LeadInbounds SET StatusTypeID ='10'" + " " + "WHERE" + " ID=" + iInboundID + "";
                cmd.CommandText = KeyValues;
                cmd.ExecuteNonQuery();
                Con.Close();
            }
            catch (Exception ex)
            {
                Con.Open();
                Con.ChangeDatabase(database);
                cmd.Connection = Con;
                //Change to LeadID
                string KeyValues = "UPDATE LeadInbounds SET StatusTypeID ='20'" + " " + "WHERE" + " ID=" + iInboundID + "";
                cmd.CommandText = KeyValues;
                cmd.ExecuteNonQuery();
                Con.Close();
            }

            Con.Open();
            Con.ChangeDatabase(database);
            cmd.Connection = Con;
            string sBuildLeadID = "";

            for (int i = 0; i < lLeadID.Count; i++)
            {
                sBuildLeadID = sBuildLeadID + " ID='" + lLeadID[i] + "' or ";

            }
            sBuildLeadID = sBuildLeadID.Substring(0, sBuildLeadID.Length - 4);
            string sGetData = "SELECT [sName],[sForeName],[sLastName],[sMob],[sEmail],[dDOB],[sPostCode],[UserID],[iPriority],[iFinance] FROM [dbo].[Leads] WHERE " + sBuildLeadID;
            cmd.CommandText = sGetData;
            SqlDataReader reader1 = cmd.ExecuteReader();
            int count = reader1.FieldCount;
            while (reader1.Read())
            {
                var lSuccessData = new List<string>();
                for (int i = 0; i < count; i++)
                {
                    lSuccessData.Add(reader1.IsDBNull(i) ? null : reader1.GetValue(i).ToString());
                }
                lSuccessDatas.Add(lSuccessData);
            }
            Con.Close();
            //Saving to Import Histories
            //DataContext db = new DataContext(database);
            //if (lLeadID.Count() > 0)
            //{
            //    var sStatus = db.ImportHistories.Find(FileID);
            //    sStatus.StatusTypeID = 1;
            //    db.SaveChanges();
            //}
            //else
            //{
            //    var sStatus = db.ImportHistories.Find(FileID);
            //    sStatus.StatusTypeID = 0;
            //    db.SaveChanges();
            //}

            return lSuccessDatas;
        }

        //public DTResponse DisplayLeadDetails(jQueryDataTableParamModel param,string database)
        //{
        //   DataContext db = new DataContext();
        //    IEnumerable<EmailDetails> AllDetails, FilteredDetails;
        //    int displyCount = 0;
        //    var sortDirection = param.sSortDir;
        //    var sortColumnIndex = param.iSortCol;
        //    int i = param.iDisplayStart + 1;
        //    if (!string.IsNullOrEmpty(param.sSearch))
        //    {
        //        FilteredDetails = db.EmailDetails.Where(m => m.FirstName.Contains(param.sSearch.ToUpper())).ToList();
        //        AllDetails = FilteredDetails.OrderBy(m => m.FirstName).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
        //        displyCount = FilteredDetails.Count();
        //    }
        //    else
        //    {
        //        displyCount = db.EmailDetails.OrderBy(m => m.FirstName).Count();
        //        AllDetails = db.EmailDetails.OrderBy(m => m.FirstName).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

        //    }
        //    var result = from c in AllDetails
        //                 select new[] {
        //                     (i++).ToString(),Convert.ToString(c.id), c.Title, c.FirstName,c.LastName,c.FullName,c.DOB.ToString(),c.HomeTelephoneNo,c.DaytimeTelephoneNo,c.ContactTelephoneNumber,c.TelephoneNumber,c.MobileNo,c.EmailAddress};
        //    return new DTResponse()
        //    {
        //        sEcho = param.sEcho,
        //        iTotalRecords = displyCount,
        //        iTotalDisplayRecords = displyCount,
        //        aaData = result
        //    };
        //}
        //Get Source provider
        #endregion OldEmailReading

        #region Providers
        public List<VMDropDown> GetSourceProvider(int OrgID, string DbName)
        {
            DataContext Spdb = new DataContext(DbName);
            List<VMDropDown> SourceDetails = new List<VMDropDown>();
            var lSourceName = new List<string>();
            SourceDetails = (from s in Spdb.OrganizationSubscriptions.Where(m => m.OrganizationID == OrgID).Where(m => m.StatusTypeID == 10)
                             join r in Spdb.OrganizationSources on s.SourceID equals r.ID
                             select new VMDropDown { text = r.Name + "-" + Spdb.OrganizationClasses.Where(m => m.ClassID == s.ClassID).Select(m => m.Class).FirstOrDefault() + "(" + s.SubscriptionID + ")", Value = r.ID, Expression = s.SubscriptionID }).ToList();
            return SourceDetails;
        }

        public int ReportSourceProvider(List<int> iID, string database, int OrgID, int FileID)
        {
            DataContext Spdb = new DataContext(database);
            int iStatus = 0;
            int Type = 1;
            if (FileID == 0)
            {
                int iIDs = 0;
                for (int p = 0; p < iID.Count; p++)
                {
                    iIDs = iID[0];
                }
                FileID = Spdb.LeadInbounds.Where(m => m.ID == iIDs).Select(m => m.FileID).FirstOrDefault();
                int iSourceID = Spdb.LeadInbounds.Where(m => m.ID == iIDs).Select(m => m.SourceID).FirstOrDefault();
                var sSendOrgDetails = Spdb.OrganizationSources.Where(h => h.OrganizationID == OrgID).Where(h => h.ID == iSourceID).ToList();
                SaveDtailsToOutbound(sSendOrgDetails, iSourceID, FileID, database);
                iStatus = ContactSRCProvider(iID, database, OrgID, FileID, Type);
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand();
                    Con.Open();
                    //Delete from Lead Inbounds
                    for (int q = 0; q < iID.Count; q++)
                    {
                        int iDelID = iID[q];
                        cmd.Connection = Con;
                        var sDeleteLeadInstance = "Delete from [dbo].[LeadInbounds] where ID=" + iDelID;
                        cmd.CommandText = sDeleteLeadInstance;
                        cmd.ExecuteNonQuery();

                    }
                    Con.Close();
                }
            }
            else
            {
                int iSourceID = Spdb.ImportingErrorDetails.Where(m => m.FileID == FileID).Select(m => m.SourceID).FirstOrDefault();
                var sSendOrgDetails = Spdb.OrganizationSources.Where(h => h.OrganizationID == OrgID).Where(h => h.ID == iSourceID).ToList();
                SaveDtailsToOutbound(sSendOrgDetails, iSourceID, FileID, database);
                iStatus = ContactSRCProvider(iID, database, OrgID, FileID, Type);
            }
            return iStatus;
        }

        public int ContactSRCProvider(List<int> iID, string database, int OrgID, int FileID, int Type)
        {
            var lErrorList = new List<string>();
            DataContext Spdb = new DataContext(database);
            string messageBody = "";
            try
            {
                if (Type == 1)
                {
                    //Getting the Server Details from the Database
                    var sDetails = ServerDetails(Type, OrgID);
                    string usern = "", pass = "", sender = "", security = "", serverName = "";
                    int port = 0;
                    foreach (var items in sDetails)
                    {
                        usern = items.UserName;
                        pass = items.Password;
                        sender = items.FromAddress;
                        serverName = items.ServerName;
                        port = items.Port;
                        security = items.Security;
                    }
                    string username = HttpUtility.UrlEncode(usern);
                    string password = HttpUtility.UrlEncode(pass);
                    string emailSubject = "Error occured in the uploaded file";
                    string Message1 = "Hi Sir/Madam, <br/> We are unable to process your uploaded file.<br/>";
                    if (iID[0] != 0)
                    {
                        for (int t = 0; t < iID.Count(); t++)
                        {
                            int iLeadID = iID[t];
                            var sErrors = Spdb.ImportingErrorDetails.Where(m => m.InboundID == iLeadID).Select(m => m.Message).ToList();
                            foreach (var items in sErrors)
                            {
                                lErrorList.Add(items);
                            }
                        }

                        string Message2 = "";
                        string Message3 = "";
                        foreach (var items in lErrorList)
                        {
                            Message2 = items + "<br/>";
                            Message3 = Message3 + Message2;
                        }
                        messageBody = Message1 + "<br/>" + Message3 + "<br/>Please check the file and try again.<br/> Regards,<br/> Team XIDNA. <br/>";
                    }
                    else
                    {
                        messageBody = Message1 + "<br/>Please check the file and try again.<br/> Regards,<br/> Team XIDNA. <br/>";
                    }
                    MailMessage msg = new MailMessage();
                    var FilePath = Spdb.ImportHistories.Where(d => d.ID == FileID).FirstOrDefault();
                    msg.To.Add("raviteja.m@inativetech.com");
                    //msg.To.Add("srihari@inativetech.com");
                    msg.From = new MailAddress(sender);
                    msg.Subject = emailSubject;
                    string html = @"<html><body>" + messageBody + "</body></html>";
                    string Attach = "";
                    msg.Body = html;
                    string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                    string str = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\images\\";
                    Attach = str + FilePath.FileName;
                    var attachement = new Attachment(Attach);
                    attachement.Name = FilePath.OriginalName;
                    msg.Attachments.Add(attachement);
                    msg.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = serverName;
                    smtp.Port = port;
                    //for gmail
                    smtp.EnableSsl = false;
                    smtp.Credentials = new System.Net.NetworkCredential(usern, pass);
                    smtp.Send(msg);
                }
            }
            catch (Exception e)
            {
                var Exp = e;
                return 0;
            }
            return 1;
        }
        public string SaveDtailsToOutbound(List<OrganizationSources> sSendOrgDetails, int SourceID, int FileID, string database)
        {
            DataContext Spdb = new DataContext(database);
            Outbounds Ob = new Outbounds();
            string sFileName = Spdb.ImportHistories.Where(m => m.ID == FileID).Select(m => m.FileName).FirstOrDefault();
            Ob.FileID = FileID;
            Ob.SourceID = SourceID;
            Ob.Attachment = sFileName;
            //Ob.Mobile = items.Mobile;
            foreach (var items in sSendOrgDetails)
            {
                Ob.OrganizationID = items.OrganizationID;
                Ob.Email = items.EmailID;
                Ob.Mobile = items.MobileNumber;
                Ob.Type = 1;
                Ob.TemplateID = 2;
                //Ob.Cc = Content.Cc;
                Spdb.Outbounds.Add(Ob);
                Spdb.SaveChanges();
            }
            return null;
        }
        #endregion Providers

        #region Importing
        public List<string> ImportExcelData(HttpPostedFileBase file, string fileName, int OrgID, int FileID, string SubID)
        {
            string sGetExtension = Path.GetExtension(file.FileName);
            string sExtension = sGetExtension.Replace(".", "").ToLower();
            string sDBName = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString());
            SqlCommand cmd = new SqlCommand();
            var lIDs = new List<string>();
            var lIDTemp = new List<string>();
            var lErrorCount = new List<int>();
            var lErrorListTemp = new List<string>();
            var lErrorList = new List<string>();
            Stream stream = file.InputStream;
            IExcelDataReader reader = null;
            if (file.FileName.EndsWith(".xls"))
            {
                reader = ExcelReaderFactory.CreateBinaryReader(stream);
            }
            else if (file.FileName.EndsWith(".xlsx"))
            {
                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }
            else
            {
                //Do nothing 
            }
            var sListOfRecords = new List<List<string>>();
            reader.IsFirstRowAsColumnNames = true;
            DataSet result = reader.AsDataSet();
            reader.Close();
            DataTable data = result.Tables[0];
            var excelColdata = new List<string>();

            foreach (DataColumn column in data.Columns)
            {
                excelColdata.Add(column.ColumnName.ToString());
            }
            DataContext Spdb = new DataContext(sDBName);
            var Today = DateTime.Today;
            var SubDetails = Spdb.OrganizationSubscriptions.Where(m => m.SubscriptionID == SubID).FirstOrDefault();
            var NewDate = Today.AddDays(SubDetails.RenewalDate).ToString("dd/MM/yyyy");
            var RDateCol = "";
            RDateCol = dbcontext.MasterTemplates.Where(m => m.ClassID == SubDetails.ClassID && m.FieldName == "dDateRenewal").Select(m => m.DataFieldName).FirstOrDefault();
            var Renewal = (from c in excelColdata.Where(m => m == RDateCol) select c).ToList();
            if (Renewal == null || Renewal.Count() == 0)
            {
                if (RDateCol != null)
                {
                    excelColdata.Add(RDateCol);
                }
            }
            else
            {
                RDateCol = Renewal[0];
            }
            foreach (DataRow row in data.Rows)
            {
                var excelRowdata = new List<string>();
                foreach (var item in row.ItemArray)
                {
                    excelRowdata.Add(item.ToString());
                }
                if (Renewal == null || Renewal.Count() == 0)
                {
                    excelRowdata.Add(null);
                }
                sListOfRecords.Add(excelRowdata);
            }
            var Index = excelColdata.IndexOf(RDateCol);
            sListOfRecords.Where(c => c[Index] == null || c[Index] == "").ToList().ForEach(cc => cc[Index] = NewDate);
            sListOfRecords.Insert(0, excelColdata);

            //listOfCompany.Where(c => c.id == 1).ToList().ForEach(cc => cc.Name = "Whatever Name");

            var lID = ValidateFields(sListOfRecords, OrgID, sExtension, 1, FileID, SubID);

            if (lID.Count() > 0)
            {
                for (int i = 0; i < lID[0].iIDs.Count; i++)
                {
                    lIDTemp.Add(lID[0].iIDs[i].ToString());
                }
                for (int i = 0; i < lID[0].sErMessages.Count; i++)
                {
                    lErrorListTemp.Add(lID[0].sErMessages[i]);
                }

                if (lID[0].sStatus == "Failure")
                {
                    lIDs.Add("Failure");
                    for (int i = 0; i < lErrorListTemp.Count; i++)
                    {
                        lErrorList.Add(lErrorListTemp[i]);
                    }
                    lIDs = lErrorList;
                    return lIDs;
                }
                else
                {
                    lIDs.Add("Success");
                    for (int i = 0; i < lIDTemp.Count; i++)
                    {
                        lIDs.Add(lIDTemp[i]);
                    }
                }
            }
            else
            {
                return null;
            }
            return lIDs;
        }

        public List<string> ImportJSONData(string fileName, int OrgID, int FileID, string SubID)
        {
            string sGetExtension = Path.GetExtension(fileName);
            string sExtension = sGetExtension.Replace(".", "").ToLower();
            DataContext Spdb = new DataContext();
            string sDBName = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            //Install JSON.NET Nuget Packages
            var json = System.IO.File.ReadAllText(fileName);

            //To deserialize JSON 
            //var objects = JsonConvert.DeserializeObject(json)

            //To deserialize object 
            //var objects = JObject.Parse(json); 

            //Deserialize into Dynamic object
            //dynamic d = JObject.Parse(json);

            var lIDTemp = new List<string>();
            var lErrorCountTemp = new List<int>();
            var lErrorListTemp = new List<string>();
            var lErrorList = new List<string>();
            var lIDs = new List<string>();
            var sErrorStatus = new List<int>();
            var lErrorColDetails = new List<VMLeadValidation>();
            var sErrorCountDetails = new List<string>();
            var lAddedColumn = new List<string>();
            int SourceID = 0;
            //Parse Json as ARRAY
            var objects = JArray.Parse(json); // parse as array 
            int iRecordNo = 0;
            foreach (JObject ChildrenTokens in objects)
            {
                var sListOfRecords = new List<List<string>>();
                var sNameList = new List<string>();
                var sValueList = new List<string>();
                //var lID = new List<string>();
                foreach (KeyValuePair<string, JToken> app in ChildrenTokens)
                {
                    var sName = app.Key;
                    var sValue = app.Value;
                    sNameList.Add(sName);
                    sValueList.Add(sValue.ToString());
                }
                sListOfRecords.Add(sNameList);
                sListOfRecords.Add(sValueList);
                iRecordNo++;
                var lID = ValidateFields(sListOfRecords, OrgID, sExtension, iRecordNo, FileID, SubID);
                if (lID.Count() > 0)
                {
                    for (int i = 0; i < lID[0].iIDs.Count; i++)
                    {
                        lIDTemp.Add(lID[0].iIDs[i].ToString());
                    }
                    for (int j = 0; j < lID[0].sErMessages.Count; j++)
                    {
                        lErrorListTemp.Add(lID[0].sErMessages[j]);
                    }
                    lErrorCountTemp.Add(lID[0].iErrorCount);
                    SourceID = lID[0].SourceID;
                    for (int k = 0; k < lID[0].lErrorDetails.Count; k++)
                    {
                        var sErrList = new List<string>();
                        var sFerror = new VMLeadValidation();
                        string sColName = lID[0].lErrorDetails[k].sColumnName;
                        string sError = lID[0].lErrorDetails[k].sError;
                        sFerror.sColumnName = sColName;
                        var sSpecificError = lID[0].lErrorDetails.Where(m => m.sColumnName == sColName).ToList();
                        foreach (var sItem in sSpecificError)
                        {
                            sErrList.Add(sItem.sError);
                        }
                        sFerror.lErrors = sErrList;
                        lErrorColDetails.Add(sFerror);
                    }
                }
            }
            var sFinalErrors = new List<VMLeadValidation>();
            foreach (var item in lErrorColDetails)
            {
                var sFErrors = new VMLeadValidation();
                var sErrList = new List<string>();
                string Column = item.sColumnName;

                var sCheckColumn = lAddedColumn.Where(x => x.Contains(Column)).FirstOrDefault();
                if (sCheckColumn == null)
                {
                    sFErrors.sColumnName = Column;
                    var sSpecificErrors = lErrorColDetails.Where(m => m.sColumnName == Column).ToList();
                    foreach (var sItem in sSpecificErrors)
                    {
                        sErrList.Add(sItem.sError);
                    }
                    sFErrors.lErrors = sErrList;
                    sFinalErrors.Add(sFErrors);
                    lAddedColumn.Add(item.sColumnName);
                }
                else
                {

                }
            }
            for (int c = 0; c < sFinalErrors.Count(); c++)
            {
                string sErrColumnName = sFinalErrors[c].sColumnName;
                int iErrCount = sFinalErrors[c].lErrors.Count();
                int iDBErrorCount = dbcontext.ImportRules.Where(s => s.RuleName == sErrColumnName).Select(s => s.Count).FirstOrDefault();
                if (iDBErrorCount > 0)
                {
                    if (iErrCount > iDBErrorCount)
                    {
                        sErrorStatus.Add(0);
                    }
                    else
                    {
                        sErrorStatus.Add(1);
                    }
                }
            }
            if (sErrorStatus.Where(item => item == 0).Count() > 0)
            {
                for (int i = 0; i < lIDTemp.Count; i++)
                {
                    using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                    {
                        SqlCommand cmd = new SqlCommand();
                        Con.Open();
                        cmd.Connection = Con;
                        cmd.CommandText = "Delete from LeadInbounds where ID='" + lIDTemp[i] + "'";
                        cmd.ExecuteNonQuery();
                        Con.Close();
                    }
                }
                lIDs.Add("Failure");
                for (int i = 0; i < lErrorListTemp.Count; i++)
                {
                    lErrorList.Add(lErrorListTemp[i]);
                }
                lIDs = lErrorList;
                //Save details to Outbound
                //var sSendOrgDetails = Spdb.OrganizationSources.Where(h => h.OrganizationID == OrgID).Where(h => h.ID == SourceID).ToList();
                //SaveDtailsToOutbound(sSendOrgDetails, SourceID, FileID, sDBName);
                return lIDs;
            }
            else
            {
                lIDs.Add("Success");
                for (int i = 0; i < lIDTemp.Count; i++)
                {
                    lIDs.Add(lIDTemp[i]);
                }
                return lIDs;
            }
        }
        public List<string> ImportXMLData(string fileName, int OrgID, int FileID, string SubID)
        {
            string sGetExtension = Path.GetExtension(fileName);
            string sExtension = sGetExtension.Replace(".", "").ToLower();
            DataContext Spdb = new DataContext();
            string sDBName = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();

            XDocument doc = XDocument.Load(fileName);
            var lIDs = new List<string>();
            var lErrorListTemp = new List<string>();
            var lErrorList = new List<string>();
            var lIDTemp = new List<string>();
            var lErrorCountTemp = new List<int>();
            var sErrorStatus = new List<int>();
            var lErrorColDetails = new List<VMLeadValidation>();
            var sErrorCountDetails = new List<string>();
            var lAddedColumn = new List<string>();
            int SourceID = 0;
            int iRecordNo = 0;
            foreach (XElement el in doc.Root.Elements())
            {
                var sListOfRecords = new List<List<string>>();
                if (el.HasAttributes == true)
                {
                    foreach (XAttribute attr in el.Attributes())
                    { }
                }
                else
                {
                    //No attributes
                }
                if (el.HasElements == true)
                {
                    var sNodeList = new List<string>();
                    var sValueList = new List<string>();
                    foreach (XElement element in el.Elements())
                    {
                        var sNode = element.Name.LocalName;
                        Regex rgxn = new Regex("-");
                        string sNewNode = rgxn.Replace(sNode, " ");
                        var sValue = element.Value;
                        //creating two different list of node and value..
                        sNodeList.Add(sNewNode);
                        sValueList.Add(sValue);
                        //var sNewValue= sNewNode + ":"+sValue;
                        //sXmlList.Add(sNewValue);
                    }
                    sListOfRecords.Add(sNodeList);
                    sListOfRecords.Add(sValueList);
                    iRecordNo++;
                    var lID = ValidateFields(sListOfRecords, OrgID, sExtension, iRecordNo, FileID, SubID);
                    if (lID.Count() > 0)
                    {
                        for (int i = 0; i < lID[0].iIDs.Count; i++)
                        {
                            lIDTemp.Add(lID[0].iIDs[i].ToString());
                        }
                        for (int j = 0; j < lID[0].sErMessages.Count; j++)
                        {
                            lErrorListTemp.Add(lID[0].sErMessages[j]);
                        }
                        lErrorCountTemp.Add(lID[0].iErrorCount);
                        for (int k = 0; k < lID[0].lErrorDetails.Count; k++)
                        {
                            var sErrList = new List<string>();
                            var sFerror = new VMLeadValidation();
                            string sColName = lID[0].lErrorDetails[k].sColumnName;
                            string sError = lID[0].lErrorDetails[k].sError;
                            sFerror.sColumnName = sColName;
                            SourceID = lID[0].SourceID;
                            var sSpecificError = lID[0].lErrorDetails.Where(m => m.sColumnName == sColName).ToList();
                            foreach (var sItem in sSpecificError)
                            {
                                sErrList.Add(sItem.sError);
                            }
                            sFerror.lErrors = sErrList;
                            lErrorColDetails.Add(sFerror);
                        }


                    }
                }
            }
            var sFinalErrors = new List<VMLeadValidation>();
            foreach (var item in lErrorColDetails)
            {
                var sFErrors = new VMLeadValidation();
                var sErrList = new List<string>();
                string Column = item.sColumnName;

                var sCheckColumn = lAddedColumn.Where(x => x.Contains(Column)).FirstOrDefault();
                if (sCheckColumn == null)
                {
                    sFErrors.sColumnName = Column;
                    var sSpecificErrors = lErrorColDetails.Where(m => m.sColumnName == Column).ToList();
                    foreach (var sItem in sSpecificErrors)
                    {
                        sErrList.Add(sItem.sError);
                    }
                    sFErrors.lErrors = sErrList;
                    sFinalErrors.Add(sFErrors);
                    lAddedColumn.Add(item.sColumnName);
                }
                else
                {

                }
            }
            for (int c = 0; c < sFinalErrors.Count(); c++)
            {
                string sErrColumnName = sFinalErrors[c].sColumnName;
                int iErrCount = sFinalErrors[c].lErrors.Count();
                int iDBErrorCount = dbcontext.ImportRules.Where(s => s.RuleName == sErrColumnName).Select(s => s.Count).FirstOrDefault();
                if (iDBErrorCount > 0)
                {
                    if (iErrCount > iDBErrorCount)
                    {
                        sErrorStatus.Add(0);
                    }
                    else
                    {
                        sErrorStatus.Add(1);
                    }
                }
            }
            if (sErrorStatus.Where(item => item == 0).Count() > 0)
            {
                for (int i = 0; i < lIDTemp.Count; i++)
                {
                    using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                    {
                        SqlCommand cmd = new SqlCommand();
                        Con.Open();
                        cmd.Connection = Con;
                        cmd.CommandText = "Delete from LeadInbounds where ID='" + lIDTemp[i] + "'";
                        cmd.ExecuteNonQuery();
                        Con.Close();
                    }
                }
                lIDs.Add("Failure");
                for (int i = 0; i < lErrorListTemp.Count; i++)
                {
                    lErrorList.Add(lErrorListTemp[i]);
                }
                lIDs = lErrorList;
                //Save details to Outbound
                //var sSendOrgDetails = Spdb.OrganizationSources.Where(h => h.OrganizationID == OrgID).Where(h => h.ID == SourceID).ToList();
                //SaveDtailsToOutbound(sSendOrgDetails, SourceID, FileID, sDBName);
                return lIDs;
            }
            else
            {
                lIDs.Add("Success");
                for (int i = 0; i < lIDTemp.Count; i++)
                {
                    lIDs.Add(lIDTemp[i]);
                }
                return lIDs;
            }
        }
        public List<string> ImportTabDelimitedData(string fileName, int OrgID, int FileID, string SubID)
        {
            string sGetExtension = Path.GetExtension(fileName);
            string sExtension = sGetExtension.Replace(".", "").ToLower();
            string sDBName = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString());
            SqlCommand cmd = new SqlCommand();
            string[] sRecords = File.ReadAllLines(fileName);
            var sListOfRecords = new List<List<string>>();
            var lIDs = new List<string>();
            var lIDTemp = new List<string>();
            var lErrorCount = new List<int>();
            var lErrorListTemp = new List<string>();
            var lErrorList = new List<string>();
            foreach (string record in sRecords)
            {
                string[] sField = record.Split('\t');
                List<string> sRecordList = new List<string>();
                for (int i = 0; i < sField.Length; i++)
                {
                    sRecordList.Add(sField[i]);
                }
                sListOfRecords.Add(sRecordList);
            }
            var lID = ValidateFields(sListOfRecords, OrgID, sExtension, 1, FileID, SubID);
            if (lID.Count() > 0)
            {
                for (int i = 0; i < lID[0].iIDs.Count; i++)
                {
                    lIDTemp.Add(lID[0].iIDs[i].ToString());
                }
                for (int i = 0; i < lID[0].sErMessages.Count; i++)
                {
                    lErrorListTemp.Add(lID[0].sErMessages[i]);
                }

                if (lID[0].sStatus == "Failure")
                {
                    lIDs.Add("Failure");
                    for (int i = 0; i < lErrorListTemp.Count; i++)
                    {
                        lErrorList.Add(lErrorListTemp[i]);
                    }
                    lIDs = lErrorList;
                    return lIDs;
                }
                else
                {
                    lIDs.Add("Success");
                    for (int i = 0; i < lIDTemp.Count; i++)
                    {
                        lIDs.Add(lIDTemp[i]);
                    }
                }
            }
            else
            {
                return null;
            }
            return lIDs;
        }
        public List<string> ImportCSVData(string fileName, int OrgID, int FileID, string SubID)
        {
            string sDBName = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            string sGetExtension = Path.GetExtension(fileName);
            string sExtension = sGetExtension.Replace(".", "").ToLower();
            var lIDs = new List<string>();
            var lIDTemp = new List<string>();
            var lErrorCount = new List<int>();
            var lErrorListTemp = new List<string>();
            var lErrorList = new List<string>();
            var sValues = new List<string>();
            using (StreamReader sr = new StreamReader(fileName))
            {
                string currentLine;
                string sRemoveNewLine = "";
                string sRemoveSpace1 = "";
                string sRemoveSpace2 = "";
                string sRemoveQuotes = "";
                string sRemoveDoubleQuotes = "";
                // currentLine will be null when the StreamReader reaches the end of file
                while ((currentLine = sr.ReadLine()) != null)
                {

                    // Search, case insensitive, if the currentLine contains the searched keyword
                    //if (currentLine.IndexOf("CRLF", StringComparison.CurrentCultureIgnoreCase) >= 0)

                    //remove CRLF
                    Regex rgxNewLine = new Regex("CRLF");
                    sRemoveNewLine = rgxNewLine.Replace(currentLine, "");

                    //replace double quotes
                    Regex rgxDouble = new Regex("\"\"");
                    sRemoveDoubleQuotes = rgxDouble.Replace(sRemoveNewLine, " ");

                    //replace space
                    Regex rgxSpace1 = new Regex("\" ");
                    sRemoveSpace1 = rgxSpace1.Replace(sRemoveDoubleQuotes, "\"");

                    //replace space
                    Regex rgxSpace2 = new Regex(" \"");
                    sRemoveSpace2 = rgxSpace2.Replace(sRemoveSpace1, "\"");

                    //replace single quotes
                    Regex rgxSingle = new Regex("\"");
                    sRemoveQuotes = rgxSingle.Replace(sRemoveSpace2, "");

                    Regex rgxSpace3 = new Regex(" ,");
                    string sRemoveSpace3 = rgxSingle.Replace(sRemoveQuotes, ",");

                    //add to the list.
                    sValues.Add(sRemoveSpace3);
                }
            }
            //Extracting
            var sListOfRecords = new List<List<string>>();
            foreach (string record in sValues)
            {
                string[] sField = record.Split(',');
                List<string> sRecordList = new List<string>();
                for (int i = 0; i < sField.Length; i++)
                {
                    sRecordList.Add(sField[i]);
                }
                sListOfRecords.Add(sRecordList);
            }

            var lID = ValidateFields(sListOfRecords, OrgID, sExtension, 1, FileID, SubID);
            if (lID.Count() > 0)
            {
                for (int i = 0; i < lID[0].iIDs.Count; i++)
                {
                    lIDTemp.Add(lID[0].iIDs[i].ToString());
                }
                for (int i = 0; i < lID[0].sErMessages.Count; i++)
                {
                    lErrorListTemp.Add(lID[0].sErMessages[i]);
                }

                if (lID[0].sStatus == "Failure")
                {
                    lIDs.Add("Failure");
                    for (int i = 0; i < lErrorListTemp.Count; i++)
                    {
                        lErrorList.Add(lErrorListTemp[i]);
                    }
                    lIDs = lErrorList;
                    return lIDs;
                }
                else
                {
                    lIDs.Add("Success");
                    for (int i = 0; i < lIDTemp.Count; i++)
                    {
                        lIDs.Add(lIDTemp[i]);
                    }

                }
            }
            else
            {
                return null;
            }
            return lIDs;
        }
        #endregion Importing

        #region SaveImported

        public List<sVMValidationDetails> ValidateFields(List<List<string>> sListOfRecords, int OrgID, string sExtensions, int iSheetNumber, int FileID, string sSubscriptionID)
        {
            string sDBName = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sDBName);
            var sValidatnResult = new List<sVMValidationDetails>();
            var sVMLeadValidation = new List<VMLeadValidation>();

            // int SourceID = DbContext.LeadInbounds.Max(m=>m.SourceID);
            int SourceID = Spdb.OrganizationSubscriptions.Where(m => m.SubscriptionID == sSubscriptionID).Select(m => m.SourceID).FirstOrDefault();
            string sExtensionType = "";
            var lBOColWithExprs = new List<string>();
            lBOColWithExprs = dbcontext.BOFields.Where(q => q.ExpressionValue != null).Select(q => q.Name).ToList();
            var lID = new List<string>();
            var lLeadID = new List<int>();
            var aValidationErr = new List<string>();
            var lTempValidationErr = new List<string>();
            var lDetails = new List<string>();
            var ErrColumn = new List<string>();
            var lErrorCount = new List<string>();

            string sColumn = "";
            var SubDetails = Spdb.OrganizationSubscriptions.Where(m => m.OrganizationID == OrgID).FirstOrDefault();
            var MasterFields = dbcontext.MasterTemplates.Where(m => m.ClassID == SubDetails.ClassID).ToList();
            var BoName = EnumLeadTables.Leads.ToString();
            var BOID = dbcontext.BOs.Where(m => m.Name == BoName).Select(m => m.BOID).FirstOrDefault();
            var BoFields = dbcontext.BOFields.Where(m => m.BOID == BOID).ToList();
            var ExcelCols = sListOfRecords[0];
            int iErrorCount = 0;
            switch (sExtensions)
            {
                case "xlsx":
                    sExtensionType = "Excel";
                    break;
                case "xls":
                    sExtensionType = "Excel";
                    break;
                case "json":
                    sExtensionType = "Json";
                    break;
                case "xml":
                    sExtensionType = "XML";
                    break;
                case "txt":
                    sExtensionType = "Tab Delimited Text";
                    break;
                case "csv":
                    sExtensionType = "CSV";
                    break;
            }
            using (SqlConnection con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
            {
                con.Open();
                SqlCommand command = con.CreateCommand();
                using (SqlConnection con1 = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    con1.Open();
                    con1.ChangeDatabase(sDBName);
                    SqlCommand CmdSaveErr = con1.CreateCommand();
                    SqlTransaction transaction;

                    // Start a local transaction.
                    transaction = con.BeginTransaction("SampleTransaction");

                    // Must assign both transaction object and connection
                    // to Command object for a pending local transaction
                    command.Connection = con;
                    CmdSaveErr.Connection = con1;
                    command.Transaction = transaction;
                    string sError = "";
                    int LeadID = 0;
                    string sRegxE = "";
                    try
                    {
                        string GetStringValue = "";
                        string sGetDate = "";
                        for (int n = 0; n < sListOfRecords.Count - 1; n++)
                        {
                            int HeadCount = sListOfRecords[0].Count();
                            var sStringList = new List<string>();
                            for (int m = 0; m < sListOfRecords[0].Count; m++)
                            {
                                int DataCount = sListOfRecords[n + 1].Count();
                                if (HeadCount == DataCount)
                                {
                                    sColumn = sListOfRecords[0][m].TrimStart().TrimEnd();
                                    string sColumnValue = sListOfRecords[n + 1][m];
                                    int sLineNo = n + 1;
                                    string sColumnName = MasterFields.Where(a => a.DataFieldName == sColumn).Select(a => a.FieldName).FirstOrDefault();
                                    if (sColumnName != "" && sColumnName != null)
                                    {
                                        if (sColumnValue != "" && sColumnValue != null)
                                        {

                                            sColumnValue = sColumnValue.TrimStart().TrimEnd();
                                            sRegxE = BoFields.Where(e => e.Name == sColumnName).Select(e => e.ExpressionValue).FirstOrDefault();
                                            if (sRegxE != null)
                                            {
                                                for (int f = 0; f < lBOColWithExprs.Count; f++)
                                                {
                                                    int iType = BoFields.Where(u => u.Name == sColumnName).Select(u => u.TypeID).FirstOrDefault();
                                                    //   List<BOFields> AssignedFields = new List<BOFields>();
                                                    int TypeID = iType;
                                                    string sDatatype = ((BODatatypes)TypeID).ToString().ToLower();
                                                    if (sDatatype == "datetime" && sColumnName == lBOColWithExprs[f])
                                                    {
                                                        string[] formats = new string[50];
                                                        var sSplitValues = sRegxE.Split(',').ToList();
                                                        for (int i = 0; i < sSplitValues.Count; i++)
                                                        {
                                                            string sRemoveQuote = sSplitValues[i].Replace("\"", "");
                                                            formats[i] = sRemoveQuote;
                                                        }
                                                        sColumnValue = sColumnValue.Split(' ')[0];
                                                        DateTime datetime;
                                                        if (DateTime.TryParseExact(sColumnValue.ToString(),
                                                                                    formats,
                                                                                    CultureInfo.InvariantCulture,
                                                                                    DateTimeStyles.AssumeLocal,
                                                                                    out datetime) && sColumnValue.ToString() != "01/01/0001" && sColumnValue.ToString() != "1/1/0001")
                                                        {

                                                            sGetDate = datetime.ToString("dd/MM/yyyy");
                                                            sStringList.Add(sColumn + ":" + sGetDate);
                                                        }
                                                        else
                                                        {
                                                            sStringList.Add(sColumn + ":");
                                                            //string sError = "";
                                                            switch (sExtensions)
                                                            {
                                                                case "xlsx":
                                                                    sError = "Sheet number - " + iSheetNumber + " Line no - " + sLineNo + ". DOB Field is Invalid. Received data is " + sColumnValue;
                                                                    break;
                                                                case "xls":
                                                                    sError = "Sheet number - " + iSheetNumber + " Line no - " + sLineNo + ". DOB Field is Invalid. Received data is " + sColumnValue;
                                                                    break;
                                                                case "json":
                                                                    sError = "Record number - " + iSheetNumber + ". DOB Field is Invalid. Received data is " + sColumnValue;
                                                                    break;
                                                                case "xml":
                                                                    sError = "Record number - " + iSheetNumber + ". DOB Field is Invalid. Received data is " + sColumnValue;
                                                                    break;
                                                                case "txt":
                                                                    sError = "Line number - " + sLineNo + ". DOB Field is Invalid. Received data is " + sColumnValue;
                                                                    break;
                                                                case "csv":
                                                                    sError = "Line number - " + sLineNo + ". DOB Field is Invalid. Received data is " + sColumnValue;
                                                                    break;
                                                            }
                                                            aValidationErr.Add(sError);
                                                            lTempValidationErr.Add(sError);
                                                            iErrorCount = iErrorCount + 1;
                                                            sVMLeadValidation.Add(new VMLeadValidation
                                                            {
                                                                sColumnName = sColumn,
                                                                sError = sError
                                                            });
                                                            ErrColumn.Add(sColumnName);
                                                        }
                                                    }
                                                    else if (sColumnName == lBOColWithExprs[f])
                                                    {
                                                        var regexItem = new Regex(sRegxE);
                                                        if (regexItem.IsMatch(sColumnValue.ToString()))
                                                        {
                                                            sStringList.Add(sColumn + ":" + sColumnValue);
                                                        }
                                                        else
                                                        {
                                                            if (sColumnName == "sForeName" || sColumnName == "sLastName")
                                                            {
                                                                var newValue = Regex.Replace(sColumnValue.ToString(), "[^a-zA-Z]+", "");
                                                                sStringList.Add(sColumn + ":" + newValue);
                                                                switch (sExtensions)
                                                                {
                                                                    case "xlsx":
                                                                        sError = "Warning: Sheet number - " + iSheetNumber + ": Line Number - " + sLineNo + ". Special characters are not allowed.";
                                                                        break;
                                                                    case "xls":
                                                                        sError = "Warning: Sheet number - " + iSheetNumber + ": Line Number - " + sLineNo + ". Special characters are not allowed.";
                                                                        break;
                                                                    case "json":
                                                                        sError = "Warning: Record Number - " + iSheetNumber + ". Special characters are not allowed.";
                                                                        break;
                                                                    case "xml":
                                                                        sError = "Warning: Record Number - " + iSheetNumber + ". Special characters are not allowed.";
                                                                        break;
                                                                    case "txt":
                                                                        sError = "Warning: Line Number - " + sLineNo + ". Special characters are not allowed.";
                                                                        break;
                                                                    case "csv":
                                                                        sError = "Warning: Line Number - " + sLineNo + ". Special characters are not allowed.";
                                                                        break;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                sStringList.Add(sColumn + ":");
                                                                switch (sExtensions)
                                                                {
                                                                    case "xlsx":
                                                                        sError = "Sheet number - " + iSheetNumber + " Line no - " + sLineNo + ". " + sColumn + " is Invalid. Received data is " + sColumnValue;
                                                                        break;
                                                                    case "xls":
                                                                        sError = "Sheet number - " + iSheetNumber + " Line no - " + sLineNo + ". " + sColumn + " is Invalid. Received data is " + sColumnValue;
                                                                        break;
                                                                    case "json":
                                                                        sError = "Record number - " + sLineNo + ". " + sColumn + " is Invalid. Received data is " + sColumnValue;
                                                                        break;
                                                                    case "xml":
                                                                        sError = "Record number - " + sLineNo + ". " + sColumn + " is Invalid. Received data is " + sColumnValue;
                                                                        break;
                                                                    case "txt":
                                                                        sError = "Line number - " + sLineNo + ". " + sColumn + " is Invalid. Received data is " + sColumnValue;
                                                                        break;
                                                                    case "csv":
                                                                        sError = "Line number - " + sLineNo + ". " + sColumn + " is Invalid. Received data is " + sColumnValue;
                                                                        break;
                                                                }
                                                            }
                                                            aValidationErr.Add(sError);
                                                            lTempValidationErr.Add(sError);
                                                            iErrorCount = iErrorCount + 1;
                                                            sVMLeadValidation.Add(new VMLeadValidation
                                                            {
                                                                sColumnName = sColumn,
                                                                sError = sError
                                                            });
                                                            ErrColumn.Add(sColumnName);
                                                        }
                                                    }
                                                }
                                            }//regex is null
                                            else
                                            {
                                                sStringList.Add(sColumn + ":" + sColumnValue);
                                            }
                                        }//---check null value
                                        else
                                        {
                                            sStringList.Add(sColumn + ":" + null);
                                            for (int d = 0; d < lBOColWithExprs.Count; d++)
                                            {
                                                if (sColumnName == lBOColWithExprs[d])
                                                {
                                                    switch (sExtensions)
                                                    {
                                                        case "xlsx":
                                                            sError = "Sheet number - " + iSheetNumber + " Line no - " + sLineNo + ". " + sColumn + " Not Supplied";
                                                            break;
                                                        case "xls":
                                                            sError = "Sheet number - " + iSheetNumber + " Line no - " + sLineNo + ". " + sColumn + " Not Supplied";
                                                            break;
                                                        case "json":
                                                            sError = "Record number - " + iSheetNumber + ". " + sColumn + " Not Supplied";
                                                            break;
                                                        case "xml":
                                                            sError = "Record number - " + iSheetNumber + ". " + sColumn + " Not Supplied";
                                                            break;
                                                        case "txt":
                                                            sError = "Line number - " + sLineNo + ". " + sColumn + " Not Supplied";
                                                            break;
                                                        case "csv":
                                                            sError = "Line number - " + sLineNo + ". " + sColumn + " Not Supplied";
                                                            break;
                                                    }
                                                    aValidationErr.Add(sError);
                                                    lTempValidationErr.Add(sError);
                                                    iErrorCount = iErrorCount + 1;
                                                    ErrColumn.Add(sColumnName);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {

                                        var sGetFieldsMapped = Spdb.MappedFields.Where(u => u.SubscriptionID == sSubscriptionID).Where(u => u.FieldName == sColumn).Select(u => u.FieldType).FirstOrDefault();

                                        if ((sGetFieldsMapped != "") && (sGetFieldsMapped != null))
                                        {
                                            sStringList.Add(sColumn + ":" + sColumnValue);
                                        }
                                        else
                                        {
                                            switch (sExtensions)
                                            {
                                                case "xlsx":
                                                    sError = "Sheet number - " + iSheetNumber + " Line no - " + sLineNo + ". The " + sColumn + " doesnt exist.  Please check!!";
                                                    break;
                                                case "xls":
                                                    sError = "Sheet number - " + iSheetNumber + " Line no - " + sLineNo + ". The " + sColumn + " doesnt exist.  Please check!!";
                                                    break;
                                                case "json":
                                                    sError = "Record number - " + iSheetNumber + ". The " + sColumn + " doesnt exist.  Please check!!";
                                                    break;
                                                case "xml":
                                                    sError = "Record number - " + iSheetNumber + ". The " + sColumn + " doesnt exist.  Please check!!";
                                                    break;
                                                case "txt":
                                                    sError = "Line number - " + sLineNo + ". The " + sColumn + " doesnt exist.  Please check!!";
                                                    break;
                                                case "csv":
                                                    sError = "Line number - " + sLineNo + ". The " + sColumn + " doesnt exist.  Please check!!";
                                                    break;
                                            }
                                            sVMLeadValidation.Add(new VMLeadValidation
                                            {
                                                sColumnName = sColumn,
                                                sError = sError
                                            });
                                            aValidationErr.Add(sError);
                                            lTempValidationErr.Add(sError);
                                            iErrorCount = iErrorCount + 1;
                                            ErrColumn.Add(sColumn);
                                        }
                                    }
                                }
                            }

                            GetStringValue = string.Join(",\r\n ", sStringList.ToArray());
                            //DbContext.Database.CommandTimeout = 0;
                            // SourceID = SourceID + 1;
                            con.ChangeDatabase(sDBName);
                            command.CommandText = "INSERT INTO [dbo].[LeadInbounds] ([SubscriptionID],[SourceID], [FileID], [ImportedOn], [ImportedBy], [Content], [StatusTypeID]) VALUES ('" + sSubscriptionID + "','" + SourceID + "','" + FileID + "','" + DateTime.Now + "','Sales1','" + GetStringValue.ToString() + "','10');SELECT SCOPE_IDENTITY()";
                            LeadID = Convert.ToInt32(command.ExecuteScalar());
                            lLeadID.Add(LeadID);

                            for (int l = 0; l < lTempValidationErr.Count; l++)
                            {
                                //for(int q=0;q<ErrColumn.Count;q++)
                                //{
                                CmdSaveErr.CommandText = "INSERT INTO [dbo].[ImportingErrorDetails] ([SourceID],[FileID],[InBoundID],[OrganizationID],[SubscriptionID],[TypeOfData],[LoggedOn],[FieldName],[Message]) VALUES ('" + SourceID + "','" + FileID + "','" + LeadID + "','" + OrgID + "','" + sSubscriptionID + "','" + sExtensionType + "','" + DateTime.Now + "','" + ErrColumn[l] + "','" + lTempValidationErr[l] + "')";
                                CmdSaveErr.ExecuteScalar();
                                //    break;
                                //}
                            }
                            lTempValidationErr.Clear();
                            ErrColumn.Clear();
                        }
                        con1.Close();
                        var lAddedColumn = new List<string>();
                        var sFinalErrors = new List<VMLeadValidation>();
                        var ExcelColums = sVMLeadValidation.Select(m => m.sColumnName).Distinct().ToList();
                        foreach (var item in ExcelColums)
                        {
                            var sFErrors = new VMLeadValidation();
                            var sErrList = new List<string>();
                            string Column = item;

                            var sCheckColumn = lAddedColumn.Where(x => x.Contains(Column)).FirstOrDefault();
                            if (sCheckColumn == null)
                            {
                                sFErrors.sColumnName = Column;
                                var sSpecificErrors = sVMLeadValidation.Where(m => m.sColumnName == Column).ToList();
                                sErrList.AddRange(sSpecificErrors.Select(m => m.sError));
                                //foreach (var sItem in sSpecificErrors)
                                //{
                                //    sErrList.Add(sItem.sError);
                                //}
                                sFErrors.lErrors = sErrList;
                                sFinalErrors.Add(sFErrors);
                                lAddedColumn.Add(item);
                            }
                            else
                            {
                                //Discard
                            }
                        }
                        if (sExtensions == "json" || sExtensions == "xml")
                        {
                            transaction.Commit();
                            sValidatnResult.Add(new sVMValidationDetails
                            {
                                sErMessages = aValidationErr,
                                iIDs = lLeadID,
                                iErrorCount = iErrorCount,
                                lErrorDetails = sVMLeadValidation,
                                FileID = FileID,
                                SourceID = SourceID
                            });
                        }
                        else
                        {
                            var sErrorStatus = new List<int>();
                            //rollback if the error count reaches level.
                            var ImportRules = dbcontext.ImportRules.ToList();
                            foreach (var items in ImportRules)
                            {
                                var VColExists = sFinalErrors.Where(m => m.sColumnName == items.RuleName).FirstOrDefault();
                                if (ExcelCols.Where(m => m == items.RuleName).Count() == 0)
                                {
                                    sErrorStatus.Add(0);
                                }
                                else if (VColExists != null)
                                {
                                    int iErrCount = sFinalErrors.Where(m => m.sColumnName == items.RuleName).Count();
                                    int iDBErrorCount = dbcontext.ImportRules.Where(s => s.RuleName == items.RuleName).Select(s => s.Count).FirstOrDefault();
                                    if (iDBErrorCount > 0)
                                    {
                                        if (iErrCount > iDBErrorCount)
                                        {
                                            sErrorStatus.Add(0);
                                        }
                                        else
                                        {
                                            sErrorStatus.Add(1);
                                        }
                                    }
                                }
                            }
                            if (sErrorStatus.Where(item => item == 0).Count() > 0)
                            {
                                transaction.Rollback();
                                sValidatnResult.Add(new sVMValidationDetails
                                {
                                    sErMessages = aValidationErr,
                                    iIDs = lLeadID,
                                    iErrorCount = iErrorCount,
                                    sErrorCounts = lErrorCount,
                                    sStatus = "Failure"
                                });
                                //Save details to Outbound
                                //var sSendOrgDetails = Spdb.OrganizationSources.Where(h => h.OrganizationID == OrgID).Where(h => h.ID == SourceID).ToList();
                                //SaveDtailsToOutbound(sSendOrgDetails, SourceID, FileID, sDBName);
                                return sValidatnResult;
                            }
                            else
                            {
                                // Attempt to commit the transaction.
                                transaction.Commit();
                                sValidatnResult.Add(new sVMValidationDetails
                                {
                                    sErMessages = aValidationErr,
                                    iIDs = lLeadID,
                                    iErrorCount = iErrorCount,
                                    sErrorCounts = lErrorCount,
                                    sStatus = "Success"
                                });
                                return sValidatnResult;
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception ex2)
                        {
                            var Exp = ex;
                            var Exp1 = ex2;
                        }
                        //sValidatnResult.Add(new sVMValidationDetails
                        //{
                        //    sErMessages = aValidationErr,
                        //    iIDs = lLeadID,
                        //    iErrorCount = iErrorCount,
                        //    sErrorCounts = lErrorCount,
                        //    sStatus = ex.Message
                        //});
                    }
                    con.Close();
                }
            }
            return sValidatnResult;
        }

        //Get Details of data valid or Invalid....
        public List<List<int>> GetValidANDInvalidDetails(List<int> ID, string database, int OrgID, int FileID)
        {
            DataContext Spdb = new DataContext(database);
            SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString());
            SqlCommand cmd = new SqlCommand();
            var lSuccessDatas = new List<List<string>>();
            var lColValNull = new List<VMImproperData>();
            try
            {
                for (int p = 0; p < ID.Count; p++)
                {
                    int iTempID = 0;
                    iTempID = ID[p];
                    var inbound = Spdb.LeadInbounds.Where(m => m.ID == iTempID).FirstOrDefault();
                    int SourceID = inbound.SourceID;
                    string sData = inbound.Content;
                    var RemoveNewLine = sData.Replace("\r\n", "");
                    var RemoveColon = RemoveNewLine.Replace(":", "<>");
                    var sKeyIn = "";
                    var sValueIn = "";

                    //New instance Table
                    List<string> KeyValuePairInstance = new List<string>();
                    KeyValuePairInstance = RemoveColon.Split(',').ToList();
                    foreach (var items in KeyValuePairInstance)
                    {
                        if (items.Contains("<>"))
                        {
                            var sKeyValues = Regex.Split(items, "<>");
                            sKeyIn = sKeyValues[0].TrimStart().TrimEnd();
                            sValueIn = sKeyValues[1].TrimStart().TrimEnd();
                        }

                        if (sValueIn != "")
                        {

                        }
                        else
                        {
                            //Get the Count.
                            string sRuleNames = dbcontext.ImportRules.Where(s => s.RuleName == sKeyIn).Select(s => s.RuleName).FirstOrDefault();
                            if (sRuleNames != null && sRuleNames.Length > 0)
                            {
                                lColValNull.Add(new VMImproperData
                                {
                                    iLeadID = iTempID,
                                    sColumn = sKeyIn
                                });
                            }
                            else
                            {
                                //Save null
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var Exp = ex;
                return null;
            }
            var sFinalCol = new List<VMImproperData>();
            var lAddedID = new List<string>();
            var ilAddedID = new List<int>();
            foreach (var item in lColValNull)
            {
                var sFCol = new VMImproperData();
                var sColList = new List<string>();
                string iLead = item.iLeadID.ToString();
                sFCol.iLeadID = item.iLeadID;
                var sCheckColumn = lAddedID.Where(x => x.Contains(iLead)).FirstOrDefault();
                if (sCheckColumn == null)
                {
                    var sSpecificCol = lColValNull.Where(m => m.iLeadID == item.iLeadID).Select(m => m.sColumn).ToList();
                    foreach (var items in sSpecificCol)
                    {
                        sColList.Add(items);
                    }

                    sFCol.lColumn = sColList;
                    sFinalCol.Add(sFCol);
                    lAddedID.Add(item.iLeadID.ToString());
                    ilAddedID.Add(item.iLeadID);
                }
                else
                {

                }
            }
            //check for ID with no invalid data.
            var lIDList = new List<int>();
            for (int t = 0; t < ID.Count; t++)
            {
                string iID = ID[t].ToString();
                string iSuccessID = lAddedID.Where(x => x.Contains(iID)).FirstOrDefault();
                if (iSuccessID == null)
                {
                    lIDList.Add(ID[t]);
                }
            }
            var sResultList = new List<List<int>>();
            var sResult = new List<int>();
            sResult.Add(ID.Count());
            sResult.Add(lIDList.Count());
            sResult.Add(sFinalCol.Count());
            sResultList.Add(sResult);
            sResultList.Add(lIDList);
            sResultList.Add(ilAddedID);
            Con.Open();
            Con.ChangeDatabase(database);
            return sResultList;
        }

        public List<List<string>> GetInValidData(List<int> ID, string database, int OrgID)
        {
            DataContext Spdb = new DataContext(database);
            using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
            {
                SqlCommand cmd = new SqlCommand();
                var lLeadID = new List<int>();
                var lSuccessDatas = new List<List<string>>();
                try
                {
                    for (int p = 0; p < ID.Count; p++)
                    {
                        int iTempID = ID[p];
                        var inbound = Spdb.LeadInbounds.Where(m => m.ID == iTempID).FirstOrDefault();
                        int SubClassID = Spdb.OrganizationSubscriptions.Where(m => m.SubscriptionID == inbound.SubscriptionID).Select(m => m.ClassID).FirstOrDefault();
                        int SourceID = inbound.SourceID;
                        string sData = inbound.Content;
                        var RemoveNewLine = sData.Replace("\r\n", "");
                        var RemoveColon = RemoveNewLine.Replace(":", "<>");
                        string sGetDataTypes;
                        var sKeyIn = "";
                        var sValueIn = "";

                        //New instance Table
                        List<string> KeyValuePairInstance = new List<string>();
                        KeyValuePairInstance = RemoveColon.Split(',').ToList();
                        KeyValuePairInstance.Add("FKiLeadClassID<>" + SubClassID);
                        KeyValuePairInstance.Add("InBoundID<>" + inbound.ID);
                        KeyValuePairInstance.Add("dImportedOn<>" + DateTime.Now);
                        KeyValuePairInstance.Add("FKiOrgID<>" + OrgID);
                        KeyValuePairInstance.Add("FKiSourceID<>" + SourceID);
                        string sFirstLeadTable = EnumLeadTables.LeadInstances.ToString();
                        string UpdateStatements = "";
                        long iInstanceID = Spdb.Database.SqlQuery<long>("INSERT INTO " + sFirstLeadTable + " (FKiSourceID) VALUES ('" + SourceID + "'); SELECT CAST( SCOPE_IDENTITY() as bigint )").FirstOrDefault();
                        foreach (var items in KeyValuePairInstance)
                        {
                            if (items.Contains("<>"))
                            {
                                var sKeyValues = Regex.Split(items, "<>");
                                sKeyIn = sKeyValues[0].TrimStart().TrimEnd();
                                sValueIn = sKeyValues[1].TrimStart().TrimEnd();
                            }

                            if (sValueIn != "")
                            {
                                var sColumnName = "";
                                var sDataType = "";
                                MasterTemplates TemplateColumnData = dbcontext.MasterTemplates.Where(m => m.ClassID == SubClassID && m.DataFieldName == sKeyIn).FirstOrDefault();
                                if (TemplateColumnData != null)
                                {
                                    sColumnName = TemplateColumnData.FieldName;
                                    sDataType = TemplateColumnData.FieldType;
                                }
                                else
                                {
                                    var MappedColumnData = Spdb.MappedFields.Where(u => u.FieldName == sKeyIn && u.ClassID == SubClassID).FirstOrDefault();
                                    if (MappedColumnData != null)
                                    {
                                        sColumnName = MappedColumnData.AddField;
                                        sDataType = MappedColumnData.FieldType;
                                    }
                                }
                                string sDataTypeValue = (sDataType).ToLower();
                                string sGetDate = "";
                                if (sColumnName != "" && sColumnName != null)
                                {
                                    //CHECKS for the DATETIME DATATYPE
                                    if (sDataTypeValue == "datetime")
                                    {
                                        if (sValueIn != "1/1/0001" && sValueIn != "01/01/0001")
                                        {
                                            if (sColumnName == "dImportedOn")
                                            {
                                                UpdateStatements = UpdateStatements + sColumnName + "='" + sValueIn + "', ";
                                            }
                                            else
                                            {
                                                DateTime datetime;
                                                string[] Pattern = { "d/M/yyyy hh:mm:ss", "d/M/yyyy" };
                                                //string[] Pattern = { "d/M/yyyy hh:mm:ss", "d/M/yyyy", "M/d/yyyy hh:mm:ss tt", "M/d/yyyy" };
                                                foreach (var PatternType in Pattern)
                                                {
                                                    if (DateTime.TryParseExact(sValueIn, PatternType, null, DateTimeStyles.None, out datetime))
                                                    {
                                                        if (sColumnName == "dDOB")
                                                        {
                                                            sGetDate = datetime.ToString("MM/dd/yyyy 00:00:00.000");
                                                        }
                                                        else
                                                        {
                                                            sGetDate = datetime.ToString("MM/dd/yyyy hh:mm:ss tt");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //error message
                                                    }
                                                }
                                                UpdateStatements = UpdateStatements + sColumnName + "='" + sGetDate + "', ";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //Not datetime datatype
                                        UpdateStatements = UpdateStatements + sColumnName + "='" + sValueIn + "', ";
                                    }
                                }
                            }
                            else
                            {
                                //Save null
                            }
                        }
                        UpdateStatements = UpdateStatements.Substring(0, UpdateStatements.Length - 2);
                        UpdateStatements = "Update [dbo].[" + sFirstLeadTable + "] set " + UpdateStatements + " WHERE [ID]='" + iInstanceID + "'";
                        Spdb.Database.ExecuteSqlCommand(UpdateStatements);
                    }

                }
                catch (Exception ex)
                {
                    var Exp = ex;
                    return null;
                }

                //Get Invalid data details
                Con.Open();
                Con.ChangeDatabase(database);
                cmd.Connection = Con;
                string sBuildLeadID = "";

                for (int i = 0; i < ID.Count; i++)
                {
                    sBuildLeadID = sBuildLeadID + " InBoundID='" + ID[i] + "' or ";

                }
                sBuildLeadID = sBuildLeadID.Substring(0, sBuildLeadID.Length - 4);
                string sGetData = "SELECT [InBoundID],[sName],[sForeName],[sLastName],[sMob],[sEmail],[dDOB],[sPostCode] FROM [dbo].[LeadInstances] WHERE " + sBuildLeadID;
                cmd.CommandText = sGetData;
                SqlDataReader reader1 = cmd.ExecuteReader();
                int count = reader1.FieldCount;
                while (reader1.Read())
                {
                    var lSuccessData = new List<string>();
                    for (int i = 0; i < count; i++)
                    {
                        lSuccessData.Add(reader1.IsDBNull(i) ? null : reader1.GetValue(i).ToString());
                    }
                    lSuccessDatas.Add(lSuccessData);
                }
                Con.Close();
                Con.Open();
                Con.ChangeDatabase(database);
                //Delete from Instance
                for (int q = 0; q < ID.Count; q++)
                {
                    int iDelID = ID[q];
                    cmd.Connection = Con;
                    var sDeleteLeadInstance = "Delete from [dbo].[LeadInstances] where InBoundID=" + iDelID;
                    cmd.CommandText = sDeleteLeadInstance;
                    cmd.ExecuteNonQuery();
                }

                Con.Close();
                return lSuccessDatas;
            }
        }

        public List<object[]> ExtractLeadData(List<int> ID, string database, int OrgID, int FileID)
        {
            DataContext Spdb = new DataContext(database);
            using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
            {
                SqlCommand cmd = new SqlCommand();
                string SubID = "";
                int SubClassID = 0;
                if (FileID > 0)
                {
                    SubID = Spdb.LeadInbounds.Where(m => m.FileID == FileID).Select(m => m.SubscriptionID).FirstOrDefault();
                    SubClassID = Spdb.OrganizationSubscriptions.Where(m => m.SubscriptionID == SubID).Select(m => m.ClassID).FirstOrDefault();
                }
                else
                {
                    int InBndID = ID[0];
                    SubID = Spdb.LeadInbounds.Where(m => m.ID == InBndID).Select(m => m.SubscriptionID).FirstOrDefault();
                    SubClassID = Spdb.OrganizationSubscriptions.Where(m => m.SubscriptionID == SubID).Select(m => m.ClassID).FirstOrDefault();
                }

                if (FileID == 0)
                {
                    int iIDs = 0;
                    for (int p = 0; p < ID.Count; p++)
                    {
                        iIDs = ID[0];
                    }
                    FileID = Spdb.LeadInbounds.Where(m => m.ID == iIDs).Select(m => m.FileID).FirstOrDefault();
                }

                ////Insert to instance....Based on the class setings we need to insert to instance, not all at once so create new method
                ////to insert into instance which takes single ID at a time.
                //InsertToInstance(ID, database, OrgID, FileID);

                //Insert to Leads and LeadClients
                // int InboundID = -1;
                var MasterFields = dbcontext.MasterTemplates.Where(m => m.ClassID == SubClassID).ToList();
                var MapFields = Spdb.MappedFields.Where(m => m.ClassID == SubClassID).ToList();
                var lLeadID = new List<long>();
                string ClientsTable = EnumLeadTables.LeadClients.ToString();
                var lSuccessDatas = new List<object[]>();
                var lColValNull = new List<VMImproperData>();
                try
                {
                    for (int p = 0; p < ID.Count; p++)
                    {
                        int iTempID = 0;
                        try
                        {
                            iTempID = ID[p];
                            var inbound = Spdb.LeadInbounds.Where(m => m.ID == iTempID).FirstOrDefault();
                            int SourceID = inbound.SourceID;
                            string sData = inbound.Content;
                            var RemoveNewLine = sData.Replace("\r\n", "");
                            var RemoveColon = RemoveNewLine.Replace(":", "<>");
                            var Key = "";
                            var Value = "";
                            string InboundsTable = EnumLeadTables.LeadInbounds.ToString();
                            string LeadsTable = EnumLeadTables.Leads.ToString();
                            //Leads....
                            List<string> KeyValuePair = new List<string>();
                            KeyValuePair = RemoveColon.Split(',').ToList();

                            //====Add SourceID and LeadClassID to the keyValuesPair
                            LeadEngine Engine = new LeadEngine();
                            KeyValuePair.Add("FKiSourceID<>" + SourceID);
                            VMLeadEngine result = Engine.GetEmailLeadDetails(KeyValuePair, OrgID, database, SubClassID);
                            //string sMainTable = "Leads";
                            string UpdateStatements = "";
                            if (result.LeadID > 0)
                            {
                                lLeadID.Add(result.LeadID);
                                if (result.InsertToInstance)
                                {
                                    //Insert to instance......................
                                    InsertOneInstance(iTempID, database, OrgID, FileID, result.LeadID);
                                }
                                //Poovanna---------------Code for always Insert 24/07/2017
                                if (result.InsertToLead)
                                {
                                    //Insert into Leads
                                    KeyValuePair.Add("InBoundID<>" + inbound.ID);
                                    KeyValuePair.Add("dImportedOn<>" + DateTime.Now);
                                    KeyValuePair.Add("FKiOrgID<>" + OrgID);
                                    KeyValuePair.Add("FKiLeadClassID<>" + SubClassID);
                                    KeyValuePair.Add("iStatus<>" + 0);
                                    long LeadID = 0;

                                    //Insert into leads table
                                    LeadID = Spdb.Database.SqlQuery<long>("INSERT INTO " + LeadsTable + " (FKiSourceID) VALUES ('" + SourceID + "'); SELECT CAST(SCOPE_IDENTITY() as bigint)").FirstOrDefault();
                                    foreach (var items in KeyValuePair)
                                    {
                                        if (items.Contains("<>"))
                                        {
                                            var keyvalues = Regex.Split(items, "<>");
                                            Key = keyvalues[0].TrimStart().TrimEnd();
                                            Value = keyvalues[1].TrimStart().TrimEnd();
                                        }
                                        if (Value != "")
                                        {
                                            var ColumnName = "";
                                            var DataType = "";
                                            MasterTemplates TemplateColumnData = MasterFields.Where(m => m.DataFieldName == Key).FirstOrDefault();
                                            if (TemplateColumnData != null)
                                            {
                                                ColumnName = TemplateColumnData.FieldName;
                                                DataType = TemplateColumnData.FieldType;
                                            }
                                            else
                                            {
                                                var MappedColumnData = MapFields.Where(u => u.FieldName == Key).FirstOrDefault();
                                                if (MappedColumnData != null)
                                                {
                                                    ColumnName = MappedColumnData.AddField;
                                                    DataType = MappedColumnData.FieldType;
                                                }
                                            }
                                            string DataTypeValue = (DataType).ToLower();
                                            string GetDate = "";
                                            if (ColumnName != "" && ColumnName != null)
                                            {
                                                if (DataTypeValue == "datetime")
                                                {
                                                    if (Value != "1/1/0001" && Value != "01/01/0001")
                                                    {
                                                        if (ColumnName == "dImportedOn")
                                                        {
                                                            UpdateStatements = UpdateStatements + ColumnName + "='" + Value + "', ";
                                                        }
                                                        else
                                                        {
                                                            DateTime datetime;
                                                            string[] Pattern = { "d/M/yyyy hh:mm:ss", "d/M/yyyy" };
                                                            //string[] Pattern = { "d/M/yyyy hh:mm:ss", "d/M/yyyy", "M/d/yyyy hh:mm:ss", "M/d/yyyy" };
                                                            foreach (var PatternType in Pattern)
                                                            {
                                                                if (DateTime.TryParseExact(Value, PatternType, null, DateTimeStyles.None, out datetime))
                                                                {
                                                                    if (ColumnName == "dDOB")
                                                                    {
                                                                        GetDate = datetime.ToString("yyyy-MM-dd 00:00:00.000");
                                                                    }
                                                                    else
                                                                    {
                                                                        GetDate = datetime.ToString("yyyy-MM-dd hh:mm:ss tt");
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    //error message
                                                                }
                                                            }
                                                            UpdateStatements = UpdateStatements + ColumnName + "='" + GetDate + "', ";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //Not datetime datatype
                                                    UpdateStatements = UpdateStatements + ColumnName + "='" + Value + "', ";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //Save Null value.
                                        }
                                    }
                                    UpdateStatements = UpdateStatements.Substring(0, UpdateStatements.Length - 2);
                                    UpdateStatements = "Update [dbo].[" + LeadsTable + "] set " + UpdateStatements + " WHERE [ID]='" + LeadID + "'";
                                    Spdb.Database.ExecuteSqlCommand(UpdateStatements);
                                    int iClientID = 0;
                                    if (result.ClientID == 0)
                                    {
                                        //Get Lead Data
                                        var leadData = Spdb.Database.SqlQuery<VMLeads>("SELECT sForeName,sLastName,sMob,sEmail,FKiLeadClassID FROM [dbo].[" + LeadsTable + "] WHERE [ID]='" + LeadID + "'").FirstOrDefault();
                                        leadData.ClassID = SubClassID;
                                        //Insert into Clients Table
                                        iClientID = Spdb.Database.SqlQuery<int>("INSERT INTO " + ClientsTable + " (Name,Email,Mobile,ClassID,InBoundID) VALUES ('" + leadData.sForeName + " " + leadData.sLastName + "','" + leadData.sEmail + "','" + leadData.sMob + "','" + leadData.ClassID + "','" + inbound.ID + "'); SELECT SCOPE_IDENTITY()").FirstOrDefault();
                                    }
                                    //Update ClientID in Leads Table
                                    Spdb.Database.ExecuteSqlCommand("UPDATE " + LeadsTable + " SET FKiClientID =" + "'" + iClientID + "'" + " " + "WHERE" + " ID=" + LeadID + "");

                                    //Priority
                                    string sQueryPriority = dbcontext.Reports.Where(m => m.Name == "Prority").Select(m => m.Query).FirstOrDefault();
                                    string sPriority = sQueryPriority.Replace("ID = 0", "ID = " + LeadID);
                                    Spdb.Database.ExecuteSqlCommand(sPriority);

                                    //Finance
                                    var SubDetails = Spdb.OrganizationSubscriptions.Where(m => m.SubscriptionID == SubID).FirstOrDefault();
                                    string sQueryFinance = "Update Leads Set iFinance =" + SubDetails.LeadCost + ", OrgHeirarchyID ='ORG" + OrgID + "_" + SubDetails.LocationCode.ToUpper() + "' Where ID =" + LeadID;
                                    Spdb.Database.ExecuteSqlCommand(sQueryFinance);

                                    //Users
                                    AssignUserID(LeadID, OrgID);

                                    //Update the Leadinbound status.
                                    Spdb.Database.ExecuteSqlCommand("UPDATE LeadInbounds SET StatusTypeID ='10'" + " " + "WHERE" + " ID=" + iTempID + "");
                                }
                            }
                            else
                            //----------------------------------new lead
                            {
                                //insert to lead
                                KeyValuePair.Add("InBoundID<>" + inbound.ID);
                                KeyValuePair.Add("dImportedOn<>" + DateTime.Now);
                                KeyValuePair.Add("FKiOrgID<>" + OrgID);
                                KeyValuePair.Add("FKiLeadClassID<>" + SubClassID);
                                KeyValuePair.Add("iStatus<>" + 0);
                                long LeadID = 0;
                                LeadID = Spdb.Database.SqlQuery<long>("INSERT INTO " + LeadsTable + " (FKiSourceID) VALUES ('" + SourceID + "'); SELECT CAST(SCOPE_IDENTITY() as bigint)").FirstOrDefault();
                                lLeadID.Add(LeadID);
                                foreach (var items in KeyValuePair)
                                {
                                    if (items.Contains("<>"))
                                    {
                                        var keyvalues = Regex.Split(items, "<>");
                                        Key = keyvalues[0].TrimStart().TrimEnd();
                                        Value = keyvalues[1].TrimStart().TrimEnd();
                                    }

                                    if (Value != "")
                                    {
                                        var ColumnName = "";
                                        var DataType = "";
                                        MasterTemplates TemplateColumnData = MasterFields.Where(m => m.DataFieldName == Key).FirstOrDefault();
                                        if (TemplateColumnData != null)
                                        {
                                            ColumnName = TemplateColumnData.FieldName;
                                            DataType = TemplateColumnData.FieldType;
                                        }
                                        else
                                        {
                                            var MappedColumnData = MapFields.Where(u => u.FieldName == Key).FirstOrDefault();
                                            if (MappedColumnData != null)
                                            {
                                                ColumnName = MappedColumnData.AddField;
                                                DataType = MappedColumnData.FieldType;
                                            }
                                        }
                                        string DataTypeValue = (DataType).ToLower();
                                        string GetDate = "";
                                        if (ColumnName != "" && ColumnName != null)
                                        {
                                            if (DataTypeValue == "datetime")
                                            {
                                                if (Value != "1/1/0001" && Value != "01/01/0001")
                                                {
                                                    if (ColumnName == "dImportedOn")
                                                    {
                                                        UpdateStatements = UpdateStatements + ColumnName + "='" + Value + "', ";
                                                    }
                                                    else
                                                    {
                                                        DateTime datetime;
                                                        //string[] Pattern = { "d/M/yyyy hh:mm:ss", "d/M/yyyy", "M/d/yyyy hh:mm:ss", "M/d/yyyy" };
                                                        string[] Pattern = { "d/M/yyyy hh:mm:ss", "d/M/yyyy" };
                                                        foreach (var PatternType in Pattern)
                                                        {
                                                            if (DateTime.TryParseExact(Value, PatternType, null, DateTimeStyles.None, out datetime))
                                                            {
                                                                if (ColumnName == "dDOB")
                                                                {
                                                                    GetDate = datetime.ToString("yyyy-MM-dd 00:00:00.000");
                                                                }
                                                                else
                                                                {
                                                                    GetDate = datetime.ToString("yyyy-MM-dd hh:mm:ss tt");
                                                                }
                                                            }
                                                            else
                                                            {
                                                                //error message
                                                            }
                                                        }
                                                        UpdateStatements = UpdateStatements + ColumnName + "='" + GetDate + "', ";
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //Not datetime datatype
                                                UpdateStatements = UpdateStatements + ColumnName + "='" + Value + "', ";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //Save Null value.
                                    }
                                }
                                UpdateStatements = UpdateStatements.Substring(0, UpdateStatements.Length - 2);
                                UpdateStatements = "Update [dbo].[" + LeadsTable + "] set " + UpdateStatements + " WHERE [ID]='" + LeadID + "'";
                                Spdb.Database.ExecuteSqlCommand(UpdateStatements);
                                //Insert to instance......................
                                InsertOneInstance(iTempID, database, OrgID, FileID, LeadID);
                                long iClientID = 0;
                                if (result.ClientID == 0)
                                {
                                    //Get Lead Data
                                    var leadData = Spdb.Database.SqlQuery<VMLeads>("SELECT sForeName,sLastName,sMob,sEmail,FKiLeadClassID FROM [dbo].[" + LeadsTable + "] WHERE [ID]='" + LeadID + "'").FirstOrDefault();
                                    leadData.ClassID = SubClassID;
                                    //Insert into Clients Table
                                    iClientID = Spdb.Database.SqlQuery<long>("INSERT INTO " + ClientsTable + " (Name,Email,Mobile,ClassID,InBoundID) VALUES ('" + leadData.sForeName + " " + leadData.sLastName + "','" + leadData.sEmail + "','" + leadData.sMob + "','" + leadData.ClassID + "','" + inbound.ID + "'); SELECT CAST(SCOPE_IDENTITY() as bigint)").FirstOrDefault();
                                }
                                //Update ClientID in Leads Table
                                Spdb.Database.ExecuteSqlCommand("UPDATE " + LeadsTable + " SET FKiClientID =" + "'" + iClientID + "'" + " " + "WHERE" + " ID=" + LeadID + "");

                                //Priority
                                string sQueryPriority = dbcontext.Reports.Where(m => m.Name == "Prority").Select(m => m.Query).FirstOrDefault();
                                string sPriority = sQueryPriority.Replace("ID = 0", "ID = " + LeadID);
                                Spdb.Database.ExecuteSqlCommand(sPriority);

                                //Finance
                                var SubDetails = Spdb.OrganizationSubscriptions.Where(m => m.SubscriptionID == SubID).FirstOrDefault();
                                string sQueryFinance = "Update " + LeadsTable + " Set iFinance =" + SubDetails.LeadCost + ", OrgHeirarchyID ='ORG" + OrgID + "_" + SubDetails.LocationCode.ToUpper() + "' Where ID =" + LeadID;
                                Spdb.Database.ExecuteSqlCommand(sQueryFinance);

                                //Users
                                AssignUserID(LeadID, OrgID);
                            }//--------end LeadID==0
                            Spdb.Database.ExecuteSqlCommand("UPDATE " + InboundsTable + " SET StatusTypeID ='10'" + " " + "WHERE" + " ID=" + iTempID + "");
                        }
                        catch (Exception ex)
                        {
                            var Exp = ex;
                            Spdb.Database.ExecuteSqlCommand("UPDATE " + EnumLeadTables.LeadInbounds.ToString() + " SET StatusTypeID ='20'" + " " + "WHERE" + " ID=" + iTempID + "");
                        }
                    }
                    string sBuildLeadID = "";
                    for (int i = 0; i < lLeadID.Count; i++)
                    {
                        sBuildLeadID = sBuildLeadID + " ID='" + lLeadID[i] + "' or ";
                    }
                    if (sBuildLeadID.Length > 0)
                    {
                        Con.Open();
                        Con.ChangeDatabase(database);
                        cmd.Connection = Con;
                        sBuildLeadID = sBuildLeadID.Substring(0, sBuildLeadID.Length - 4);
                        string sGetData = "SELECT [ID],[sName],[sForeName],[sLastName],[sMob],[sEmail], convert(varchar, dDOB, 105),[sPostCode],[iTeamID],[iPriority],[iFinance] FROM [dbo].[Leads] WHERE " + sBuildLeadID;
                        cmd.CommandText = sGetData;
                        SqlDataReader reader1 = cmd.ExecuteReader();
                        DataTable Data = new DataTable();
                        Data.Load(reader1);
                        lSuccessDatas = Data.AsEnumerable().Select(m => m.ItemArray).ToList();
                        Con.Close();
                    }
                    //DataContext Spdb = new DataContext(database);
                    if (lLeadID.Count() > 0)
                    {
                        var sStatus = Spdb.ImportHistories.Find(FileID);
                        sStatus.StatusTypeID = 1;
                        Spdb.SaveChanges();
                    }
                    else
                    {
                        var sStatus = Spdb.ImportHistories.Find(FileID);
                        sStatus.StatusTypeID = 0;
                        Spdb.SaveChanges();
                    }

                    return lSuccessDatas;
                }

                catch (Exception ex)
                {
                    var Exp = ex;
                    return null;
                }
            }
            //return lSuccessDatas;
        }

        private string InsertOneInstance(int ID, string database, int OrgID, int FileID, long LeadID)
        {
            DataContext Spdb = new DataContext(database);
            var lLeadID = new List<int>();
            var lSuccessDatas = new List<List<string>>();
            var lColValNull = new List<VMImproperData>();
            string SubID = Spdb.LeadInbounds.Where(m => m.FileID == FileID).Select(m => m.SubscriptionID).FirstOrDefault();
            int SubClassID = Spdb.OrganizationSubscriptions.Where(m => m.SubscriptionID == SubID).Select(m => m.ClassID).FirstOrDefault();
            try
            {
                int iTempID = ID;
                var inbound = Spdb.LeadInbounds.Where(m => m.ID == iTempID).FirstOrDefault();
                int SourceID = inbound.SourceID;
                string sData = inbound.Content;
                var RemoveNewLine = sData.Replace("\r\n", "");
                var RemoveColon = RemoveNewLine.Replace(":", "<>");
                var sKeyIn = "";
                var sValueIn = "";
                int BOID = dbcontext.BOs.Where(m => m.Name == EnumLeadTables.LeadInstances.ToString()).Select(m => m.BOID).FirstOrDefault();
                //New instance Table
                List<string> KeyValuePairInstance = new List<string>();
                KeyValuePairInstance = RemoveColon.Split(',').ToList();
                KeyValuePairInstance.Add("FKiLeadID<>" + LeadID);
                KeyValuePairInstance.Add("FKiLeadClassID<>" + SubClassID);
                KeyValuePairInstance.Add("InBoundID<>" + inbound.ID);
                KeyValuePairInstance.Add("dImportedOn<>" + DateTime.Now);
                KeyValuePairInstance.Add("FKiOrgID<>" + OrgID);
                KeyValuePairInstance.Add("FKiSourceID<>" + SourceID);
                string sFirstLeadTable = EnumLeadTables.LeadInstances.ToString();
                string UpdateStatements = "";
                long iInstanceID = Spdb.Database.SqlQuery<long>("INSERT INTO " + sFirstLeadTable + " (FKiSourceID) VALUES ('" + SourceID + "'); SELECT CAST(SCOPE_IDENTITY() as bigint)").FirstOrDefault();
                foreach (var items in KeyValuePairInstance)
                {
                    if (items.Contains("<>"))
                    {
                        var sKeyValues = Regex.Split(items, "<>");
                        sKeyIn = sKeyValues[0].TrimStart().TrimEnd();
                        sValueIn = sKeyValues[1].TrimStart().TrimEnd();
                    }

                    if (sValueIn != "")
                    {
                        var sColumnName = "";
                        var sDataType = "";
                        MasterTemplates TemplateColumnData = dbcontext.MasterTemplates.Where(m => m.ClassID == SubClassID && m.DataFieldName == sKeyIn).FirstOrDefault();
                        if (TemplateColumnData != null)
                        {
                            sColumnName = TemplateColumnData.FieldName;
                            sDataType = TemplateColumnData.FieldType;
                        }
                        else
                        {
                            var MappedColumnData = Spdb.MappedFields.Where(u => u.FieldName == sKeyIn && u.ClassID == SubClassID).FirstOrDefault();
                            if (MappedColumnData != null)
                            {
                                sColumnName = MappedColumnData.AddField;
                                sDataType = MappedColumnData.FieldType;
                            }
                        }
                        if (sColumnName == "")
                        {
                            sColumnName = sKeyIn;
                            var TypeID = dbcontext.BOFields.Where(m => m.BOID == BOID && m.Name == sKeyIn).Select(m => m.TypeID).FirstOrDefault();
                            sDataType = ((BODatatypes)TypeID).ToString().ToLower();
                        }
                        string sDataTypeValue = (sDataType).ToLower();
                        string sGetDate = "";
                        if (sColumnName != "" && sColumnName != null)
                        {
                            //CHECKS for the DATETIME DATATYPE
                            if (sDataTypeValue == "datetime")
                            {
                                if (sColumnName == "dImportedOn")
                                {
                                    UpdateStatements = UpdateStatements + sColumnName + "='" + sValueIn + "', ";
                                }
                                else
                                {
                                    DateTime datetime;
                                    string[] Pattern = { "d/M/yyyy hh:mm:ss", "d/M/yyyy" };
                                    //string[] Pattern = { "d/M/yyyy hh:mm:ss", "d/M/yyyy", "M/d/yyyy hh:mm:ss tt", "M/d/yyyy" };
                                    foreach (var PatternType in Pattern)
                                    {
                                        if (DateTime.TryParseExact(sValueIn, PatternType, null, DateTimeStyles.None, out datetime))
                                        {
                                            if (sColumnName == "dDOB")
                                            {
                                                sGetDate = datetime.ToString("MM/dd/yyyy 00:00:00.000");
                                            }
                                            else
                                            {
                                                sGetDate = datetime.ToString("MM/dd/yyyy hh:mm:ss tt");
                                            }
                                        }
                                        else
                                        {
                                            //error message
                                        }
                                    }
                                    UpdateStatements = UpdateStatements + sColumnName + "='" + sGetDate + "', ";
                                }
                            }
                            else
                            {
                                //Not datetime datatype
                                UpdateStatements = UpdateStatements + sColumnName + "='" + sValueIn + "', ";
                            }
                        }
                    }
                    else
                    {
                        //Save null
                    }
                }
                UpdateStatements = UpdateStatements.Substring(0, UpdateStatements.Length - 2);
                UpdateStatements = "Update [dbo].[" + sFirstLeadTable + "] set " + UpdateStatements + " WHERE [ID]='" + iInstanceID + "'";
                Spdb.Database.ExecuteSqlCommand(UpdateStatements);
            }
            catch (Exception ex)
            {
                var Exp = ex;
                return null;
            }
            return null;
        }

        private void AssignUserID(long LeadID, int OrgID)
        {
            try
            {
                int UserID = 0, NewUserID = 0, index = 0;
                var AllUsers = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).ToList();
                string database = AllUsers.FirstOrDefault().sDatabaseName;
                DataContext Spdb = new DataContext(database);
                var Users = AllUsers.Select(m => m.UserID).ToList();
                List<int> UserIDs = new List<int>();
                foreach (var items in Users)
                {
                    var RoleID = dbcontext.XIAppUserRoles.Where(m => m.UserID == items).Select(m => m.RoleID).FirstOrDefault();
                    var RoleName = dbcontext.XIAppRoles.Where(m => m.RoleID == RoleID).Select(m => m.sRoleName).FirstOrDefault();
                    if (RoleName != EnumRoles.Admin.ToString())
                    {
                        UserIDs.Add(items);
                    }
                }
                DateTime? LastAssignedTime = null;
                LastAssignedTime = Spdb.Database.SqlQuery<DateTime>("select max(assignedtime) from " + EnumLeadTables.Leads.ToString() + " where fkiorgid=" + OrgID).FirstOrDefault();
                if (LastAssignedTime != null)
                {
                    UserID = Spdb.Database.SqlQuery<int>("select userid from " + EnumLeadTables.Leads.ToString() + " where fkiorgid=" + OrgID + " and AssignedTime='" + LastAssignedTime + "'").FirstOrDefault();
                }

                if (UserIDs.Count() > 0)
                {
                    if (UserID > 0)
                    {
                        index = UserIDs.IndexOf(UserID);
                    }
                    else
                    {
                        index = 0;
                    }

                    if (index > 0)
                    {
                        if (index == UserIDs.Count() - 1)
                        {
                            NewUserID = UserIDs[0];
                        }
                        else
                        {
                            NewUserID = UserIDs[index + 1];
                        }
                        if (NewUserID > 0)
                        {
                            Spdb.Database.ExecuteSqlCommand("update " + EnumLeadTables.Leads.ToString() + " set userid=" + NewUserID + ", AssignedTime='" + DateTime.Now + "' where fkiorgid=" + OrgID + " and ID=" + LeadID + " and (userid is null or userid = 0)");
                        }
                    }
                    else
                    {
                        NewUserID = UserIDs[0];
                        Spdb.Database.ExecuteSqlCommand("update " + EnumLeadTables.Leads.ToString() + " set userid=" + NewUserID + ", AssignedTime='" + DateTime.Now + "' where fkiorgid=" + OrgID + " and ID=" + LeadID + " and (userid is null or userid = 0)");
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }
        #endregion SaveImported

        #region Popup

        public int CallAction(VMLeadActions model, int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext(sDatabase);
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            Common Com = new Common();
            var ActionMenu = Spdb.LeadActionMenus.Find(model.ID);
            var ActionTypes = Spdb.LeadActions.Find(ActionMenu.ActionType);
            if (ActionTypes.IsSMS)
            {
                Com.SendSMS(iUserID, OrgID, ActionTypes.SMSTemplateID, sDatabase, sOrgName);
                int val = SaveToOutbounds(model, OrgID);
            }
            if (ActionTypes.IsEmail)
            {
                Com.SendMail(iUserID, OrgID, "", ActionTypes.EmailTemplateID, "", sDatabase, sOrgName);
                int val = SaveToOutbounds(model, OrgID);
            }
            if (ActionTypes.IsOneClcik)
            {
                var Query = ActionTypes.Query;
                if (Query.IndexOf("{ID}", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Query = Regex.Replace(Query, "{ID}", model.LeadID.ToString(), RegexOptions.IgnoreCase);
                }
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand();
                    Con.Open();
                    Con.ChangeDatabase(sOrgDB);
                    cmd.Connection = Con;
                    cmd.CommandText = Query;
                    cmd.ExecuteNonQuery();
                    Con.Close();
                }
            }
            if (ActionTypes.IsStage)
            {

            }
            return 0;
        }

        public Popup GetPopupDetails(int PopupID)
        {
            var Popup = dbcontext.Popups.Find(PopupID);
            return Popup;
        }

        //Lead Popup -- Left Content
        public VMLeadPopupLeft GetLeadPopupLeftContent(VMViewPopup popup, string database, int OrgID)
        {
            DataContext Spdb = new DataContext(database);
            var tablename = EnumLeadTables.Leads.ToString();
            //var Leads = Spdb.Database.SqlQuery<VMLeads>("SELECT * FROM " + tablename + " Where ID = " + popup.LeadID).FirstOrDefault();
            var Leads = Spdb.Database.SqlQuery<VMLeads>("SELECT * FROM " + tablename).ToList();
            var BOFields = dbcontext.BOFields.Where(m => m.BOID == 1).ToList();
            //for (int j = 0; j < BOFields.OrderBy(m=>m.Name).Count(); j++)
            //{

            //}
            //Dictionary<BOFields, Leads> d = new Dictionary<BOFields, Leads>();
            //List<Leads> AllLeads = new List<Leads>();
            //for (int i = 0; i < Leads.Count(); i++)
            //{
            //    Leads Lead = new Leads();
            //    Lead.LeadID = Leads[i].ID;
            //    Lead.ID = BOFields.Where(m => m.Name == "ID").FirstOrDefault();
            //    Lead.ID.Value = Leads[i].ID.ToString();
            //    Lead.sForeName = BOFields.Where(m => m.Name == "sForeName").FirstOrDefault();
            //    Lead.sForeName.Value = string.IsNullOrEmpty(Leads[i].sForeName) ? null : Leads[i].sForeName.ToString();
            //    AllLeads.Add(Lead);
            //}





            VMLeadPopupLeft LeftContent = new VMLeadPopupLeft();
            var orgname = dbcontext.Organization.Find(OrgID);
            //VMLeads lead = new VMLeads();
            int? ClassID = 0;
            string MobileNo = "";

            var leadData = Spdb.Database.SqlQuery<VMLeads>("SELECT ID, sForeName, sLastName, FKiLeadClassID, sEmail, iCallCount, dtCallSchedule, iCallbackStatus, dImportedOn, sSystemAlert, sMob FROM " + tablename + " Where ID = " + popup.LeadID).FirstOrDefault();
            leadData.ConversionStatus = getconversionstatus(popup.LeadID, database);
            leadData.OrganizationName = orgname.Name;
            leadData.OrganizationID = OrgID;
            leadData.CallBackStatus = (leadData.iCallbackStatus > 0 ? (leadData.iCallbackStatus == 10 ? "Call back pending" : "") : null);
            MobileNo = leadData.sMob;
            ClassID = leadData.FKiLeadClassID;
            var ClassName = dbcontext.Types.Where(m => m.ID == ClassID).Select(m => m.Expression).FirstOrDefault();
            leadData.Class = ClassName;
            leadData.StatusTypeID = 10;
            var Request = dbcontext.WalletRequests.Where(m => m.EmailID == leadData.sEmail).FirstOrDefault();
            if (Request != null)
            {
                leadData.IsReqSent = true;
            }
            else
            {
                leadData.IsReqSent = false;
            }
            LeftContent.LeadInfo = leadData;
            //Stages
            List<VMStages> AllStages = new List<VMStages>();
            var History = Spdb.LeadTransitions.Where(m => m.LeadID == popup.LeadID).ToList();
            LeadTransitions LeadHistory = new LeadTransitions();
            if (History.Count() > 0)
            {
                LeadHistory = History.Last();
            }
            else
            {
                LeadHistory = null;
            }
            if (LeadHistory != null)
            {
                var NextStages = Spdb.StagesFlows.Where(m => m.OrganizationID == OrgID && m.StageID == LeadHistory.StageID && m.StatusTypeID == 10).Select(m => m.SubStages).FirstOrDefault();
                if (NextStages != null)
                {
                    var NStages = NextStages.Split(',').ToList();
                    foreach (var items in NStages)
                    {
                        int ID = Convert.ToInt32(items);
                        var Stage = dbcontext.Stages.Where(m => m.ID == ID).FirstOrDefault();
                        AllStages.Add(new VMStages
                        {
                            ID = Stage.ID,
                            Name = Stage.Name,
                            LeadID = popup.LeadID,
                            PopupID = Stage.PopupID
                        });
                    }
                }
            }
            else
            {
                StagesFlows SFlow = Spdb.StagesFlows.Where(m => m.OrganizationID == OrgID && m.StatusTypeID == 10).FirstOrDefault();
                if (SFlow != null)
                {
                    var Stage = dbcontext.Stages.Where(m => m.ID == SFlow.StageID).FirstOrDefault();
                    AllStages.Add(new VMStages
                    {
                        ID = Stage.ID,
                        Name = Stage.Name,
                        LeadID = popup.LeadID,
                        PopupID = Stage.PopupID
                    });
                }
            }
            LeftContent.LeadStages = AllStages;
            //Lead Actions
            var res = Spdb.LeadActionMenus.Where(m => m.OrganizationID == OrgID && m.StatusTypeID == 10).ToList();
            List<VMLeadActions> AllActions = new List<VMLeadActions>();
            foreach (var items in res)
            {
                int PopupID = 0;
                int StageID = 0;
                var ActionType = Spdb.LeadActions.Where(m => m.ID == items.ActionType).FirstOrDefault();
                if (ActionType.IsPopup)
                {
                    PopupID = ActionType.PopupID;
                }
                if (ActionType.IsStage)
                {
                    StageID = ActionType.StageID;
                }
                AllActions.Add(new VMLeadActions
                {
                    ID = items.ID,
                    Name = items.Name,
                    PopupID = PopupID,
                    LeadID = popup.LeadID,
                    StageID = StageID
                });
            }
            LeftContent.LeadActions = AllActions;
            //Lead Clients
            var ClientData = Spdb.Database.SqlQuery<VMLeads>("SELECT ID, Name, ClassID FROM LeadClients Where Name = '" + leadData.sForeName + " " + leadData.sLastName + "' AND Email='" + leadData.sEmail + "' AND Mobile='" + MobileNo + "'").ToList();
            foreach (var items in ClientData)
            {
                items.Class = getclass(items.ClassID, OrgID, database);
            }
            LeftContent.LeadClients = ClientData;
            return LeftContent;
        }
        public List<VMDropDown> GetAllTabs(int ReportID, int PopupID, string database)
        {
            int PopupType = 0;
            string PopupName = "";
            if (ReportID > 0 && PopupID == 0)
            {
                PopupType = dbcontext.Reports.Where(m => m.ID == ReportID).Select(m => m.OnRowClickValue).FirstOrDefault();
            }
            else
            {
                PopupType = dbcontext.Popups.Where(m => m.ID == PopupID).Select(m => m.ID).FirstOrDefault();
            }
            //PopupName = dbcontext.Popups.Where(m => m.ID == PopupType).Select(m => m.Name).FirstOrDefault();
            List<VMDropDown> AllTabs = new List<VMDropDown>();
            if (PopupType > 0)
            {
                AllTabs = (from c in dbcontext.Tabs.Where(m => m.PopupID == PopupType && m.StatusTypeID == 10).OrderBy(m => m.Rank).ToList()
                           select new VMDropDown { text = c.Name, Value = c.ID }).ToList();
                return AllTabs;
            }
            else
            {
                return AllTabs;
            }
        }
        public List<VMQueryPreview> GetTabContent(VMViewPopup Popup, int UserID, int OrgID, string database, string sOrgName)
        {
            int? TabID = Popup.TabID;
            int? LeadID = Popup.LeadID;
            DataContext Spdb = new DataContext(database);
            int ClassID = Spdb.Database.SqlQuery<int>("Select FKiLeadClassID From " + EnumLeadTables.Leads.ToString() + " Where ID = " + LeadID).FirstOrDefault();
            var Tabs = dbcontext.Tabs.ToList();
            var Tab1ClicksList = dbcontext.Tab1Clicks.Where(m => m.TabID == TabID).ToList();
            var tab1click = Tab1ClicksList.Where(m => m.ClassID == ClassID && m.StatusTypeID == 10).ToList();
            int Dtype = (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.Bespoke.ToString());
            var tab1clickbespoke = Tab1ClicksList.Where(m => m.DisplayAs == Dtype).ToList();
            var AllTab1Clicks = tab1click.Union(tab1clickbespoke).ToList();
            var AllSections = dbcontext.Sections.ToList();
            List<VMQueryPreview> TabsResult = new List<VMQueryPreview>();
            foreach (var items in AllTab1Clicks)
            {
                ViewRecord ViewRecord = new ViewRecord();
                ViewRecord.SectionIDs = items.SectionID;
                ViewRecord.ViewFields = items.ViewFields;
                ViewRecord.EditFields = items.EditFields;
                if (items.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.ViewRecord.ToString()))
                {
                    if (ViewRecord.ViewFields != null || ViewRecord.EditFields != null)
                    {
                        VMQueryPreview result = new VMQueryPreview();
                        if (Popup.PopType == "Inner")
                        {
                            result = QueryDynamicForm(items.ID, Popup.RowID, database, OrgID);
                        }
                        else
                        {
                            var reportid = Tab1ClicksList.Where(m => m.ID == items.ID).Select(m => m.ReportID).FirstOrDefault();
                            var boid = dbcontext.Reports.Where(m => m.ID == reportid).Select(m => m.BOID).FirstOrDefault();
                            var BOName = dbcontext.BOs.Where(m => m.BOID == boid).Select(m => m.Name).FirstOrDefault();
                            if (BOName == EnumLeadTables.LeadClients.ToString())
                            {
                                result = QueryDynamicForm(items.ID, Popup.ClientID, database, OrgID);
                            }
                            else
                            {
                                result = QueryDynamicForm(items.ID, LeadID, database, OrgID);
                            }

                        }
                        result.SectionsData.FirstOrDefault().TabID = TabID;
                        result.SectionsData.FirstOrDefault().ClassID = ClassID;
                        result.SectionsData.FirstOrDefault().TabName = Tabs.Where(m => m.ID == TabID).Select(m => m.Name).FirstOrDefault();
                        result.PreviewType = EnumDisplayTypes.ViewRecord.ToString();
                        if (!(items.SectionID.Contains(',')))
                        {
                            var secid = Convert.ToInt32(items.SectionID);
                            int rank = 0;
                            if (secid > 0)
                            {
                                rank = AllSections.Where(m => m.ID == secid).Select(m => m.Rank).FirstOrDefault();
                            }
                            result.Rank = rank;
                        }
                        result.ReportID = items.ReportID;
                        result.TabName = Tabs.Where(m => m.ID == items.TabID).Select(m => m.Name).FirstOrDefault();
                        result.TabID = items.TabID;
                        TabsResult.Add(result);
                    }
                    else
                    {
                        VMQueryPreview preview = new VMQueryPreview();
                        TabsResult.Add(preview);
                    }
                }
                else if (items.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.Bespoke.ToString()))
                {
                    VMQueryPreview preview = new VMQueryPreview();
                    SectionsData data = new SectionsData();
                    List<SectionsData> Data = new List<SectionsData>();
                    data.IsBespoke = items.IsBespoke;
                    data.URL = items.URL;
                    data.RefreshType = items.RefreshType;
                    var sectionid = Convert.ToInt32(items.SectionID);
                    data.SectionName = AllSections.Where(m => m.ID == sectionid).Select(m => m.Name).FirstOrDefault();
                    data.LeadID = Popup.LeadID;
                    data.TabID = Popup.TabID;
                    Data.Add(data);
                    preview.SectionsData = Data;
                    preview.Rank = AllSections.Where(m => m.ID == sectionid).Select(m => m.Rank).FirstOrDefault();
                    preview.PreviewType = EnumDisplayTypes.Bespoke.ToString();
                    preview.TabName = Tabs.Where(m => m.ID == items.TabID).Select(m => m.Name).FirstOrDefault();
                    TabsResult.Add(preview);
                }
                else if (items.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.ResultList.ToString()))
                {
                    Common Com = new Common();
                    var result = Com.GetHeadings(items.FKi1ClickIDXIGUID, database, OrgID, UserID, sOrgName);
                    result.ViewRecord = ViewRecord;
                    var sectionid = Convert.ToInt32(items.SectionID);
                    int rank = 0;
                    if (sectionid > 0)
                    {
                        result.SectionName = AllSections.Where(m => m.ID == sectionid).Select(m => m.Name).FirstOrDefault();
                        result.TabID = Popup.TabID;
                        rank = AllSections.Where(m => m.ID == sectionid).Select(m => m.Rank).FirstOrDefault();
                        result.Rank = rank;
                        result.SectionID = sectionid;
                    }
                    result.TabID = TabID;
                    result.ReportID = items.ReportID;
                    result.PreviewType = EnumDisplayTypes.ResultList.ToString();
                    result.Tab1ClickID = items.ID;
                    result.BOID = dbcontext.Reports.Where(m => m.ID == items.ReportID).Select(m => m.BOID).FirstOrDefault();
                    SectionsData data = new SectionsData();
                    List<SectionsData> Data = new List<SectionsData>();
                    data.TabID = TabID;
                    data.SectionID = sectionid;
                    Data.Add(data);
                    result.SectionsData = Data;
                    result.IsView = items.IsView;
                    result.IsEdit = items.IsEdit;
                    result.IsCreate = items.IsCreate;
                    result.TabName = Tabs.Where(m => m.ID == items.TabID).Select(m => m.Name).FirstOrDefault();
                    TabsResult.Add(result);
                }
                else if (items.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.KPICircle.ToString()))
                {
                    VMQueryPreview preview = new VMQueryPreview();
                    var result = KPICirlceForTab(items.ReportID, UserID, OrgID, sOrgName);
                    preview.KpiCircle = result;
                    preview.PreviewType = EnumDisplayTypes.KPICircle.ToString();
                    preview.ViewRecord = ViewRecord;
                    var sectionid = Convert.ToInt32(items.SectionID);
                    if (sectionid > 0)
                    {
                        result.FirstOrDefault().SectionName = AllSections.Where(m => m.ID == sectionid).Select(m => m.Name).FirstOrDefault();
                        result.FirstOrDefault().TabID = Popup.TabID;
                    }
                    int rank = 0;
                    if (sectionid > 0)
                    {
                        rank = AllSections.Where(m => m.ID == sectionid).Select(m => m.Rank).FirstOrDefault();
                    }
                    preview.Rank = rank;
                    preview.ReportID = items.ReportID;
                    SectionsData data = new SectionsData();
                    List<SectionsData> Data = new List<SectionsData>();
                    data.TabID = TabID;
                    data.SectionID = sectionid;
                    Data.Add(data);
                    preview.SectionsData = Data;
                    preview.TabName = Tabs.Where(m => m.ID == items.TabID).Select(m => m.Name).FirstOrDefault();
                    TabsResult.Add(preview);
                }
                else if (items.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.PieChart.ToString()))
                {
                    VMQueryPreview preview = new VMQueryPreview();
                    VMViewPopup popup = new VMViewPopup();
                    popup.TabID = TabID;
                    popup.ClassID = ClassID;
                    preview.PreviewType = EnumDisplayTypes.PieChart.ToString();
                    preview.popup = popup;
                    preview.ViewRecord = ViewRecord;
                    var sectionid = Convert.ToInt32(items.SectionID);
                    if (sectionid > 0)
                    {
                        preview.SectionName = AllSections.Where(m => m.ID == sectionid).Select(m => m.Name).FirstOrDefault();
                    }
                    int rank = 0;
                    if (sectionid > 0)
                    {
                        rank = AllSections.Where(m => m.ID == sectionid).Select(m => m.Rank).FirstOrDefault();
                    }
                    preview.Rank = rank;
                    preview.ReportID = items.ReportID;
                    SectionsData data = new SectionsData();
                    List<SectionsData> Data = new List<SectionsData>();
                    data.TabID = TabID;
                    data.SectionID = sectionid;
                    Data.Add(data);
                    preview.SectionsData = Data;
                    var result = GetPieChartForTab(UserID, database, OrgID, items.ReportID, sOrgName);
                    preview.PieData = result;
                    preview.QueryName = dbcontext.Reports.Where(m => m.ID == items.ReportID).Select(m => m.Name).FirstOrDefault();
                    preview.TabName = Tabs.Where(m => m.ID == items.TabID).Select(m => m.Name).FirstOrDefault();
                    TabsResult.Add(preview);
                }
                else if (items.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.BarChart.ToString()))
                {
                    VMQueryPreview preview = new VMQueryPreview();
                    VMViewPopup popup = new VMViewPopup();
                    popup.TabID = TabID;
                    popup.ClassID = ClassID;
                    preview.PreviewType = EnumDisplayTypes.BarChart.ToString();
                    preview.popup = popup;
                    preview.ViewRecord = ViewRecord;
                    var sectionid = Convert.ToInt32(items.SectionID);
                    if (sectionid > 0)
                    {
                        preview.SectionName = AllSections.Where(m => m.ID == sectionid).Select(m => m.Name).FirstOrDefault();
                    }
                    int rank = 0;
                    if (sectionid > 0)
                    {
                        rank = AllSections.Where(m => m.ID == sectionid).Select(m => m.Rank).FirstOrDefault();
                    }
                    preview.Rank = rank;
                    preview.ReportID = items.ReportID;
                    SectionsData data = new SectionsData();
                    List<SectionsData> Data = new List<SectionsData>();
                    data.TabID = TabID;
                    data.SectionID = sectionid;
                    Data.Add(data);
                    preview.SectionsData = Data;
                    var result = GetBarChartForTab(UserID, database, OrgID, items.ReportID, sOrgName);
                    preview.BarData = result;
                    preview.QueryName = dbcontext.Reports.Where(m => m.ID == items.ReportID).Select(m => m.Name).FirstOrDefault();
                    preview.TabName = Tabs.Where(m => m.ID == items.TabID).Select(m => m.Name).FirstOrDefault();
                    TabsResult.Add(preview);
                }
                else if (items.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.LineChart.ToString()))
                {
                    VMQueryPreview preview = new VMQueryPreview();
                    VMViewPopup popup = new VMViewPopup();
                    popup.TabID = TabID;
                    popup.ClassID = ClassID;
                    preview.PreviewType = EnumDisplayTypes.LineChart.ToString();
                    preview.popup = popup;
                    preview.ViewRecord = ViewRecord;
                    var sectionid = Convert.ToInt32(items.SectionID);
                    if (sectionid > 0)
                    {
                        preview.SectionName = AllSections.Where(m => m.ID == sectionid).Select(m => m.Name).FirstOrDefault();
                    }
                    int rank = 0;
                    if (sectionid > 0)
                    {
                        rank = AllSections.Where(m => m.ID == sectionid).Select(m => m.Rank).FirstOrDefault();
                    }
                    preview.Rank = rank;
                    preview.ReportID = items.ReportID;
                    SectionsData data = new SectionsData();
                    List<SectionsData> Data = new List<SectionsData>();
                    data.TabID = TabID;
                    data.SectionID = sectionid;
                    Data.Add(data);
                    preview.SectionsData = Data;
                    var result = GetBarChartForTab(UserID, database, OrgID, items.ReportID, sOrgName);
                    preview.LineGraph = result;
                    preview.QueryName = dbcontext.Reports.Where(m => m.ID == items.ReportID).Select(m => m.Name).FirstOrDefault();
                    preview.TabName = Tabs.Where(m => m.ID == items.TabID).Select(m => m.Name).FirstOrDefault();
                    TabsResult.Add(preview);
                }
                else
                {
                    return null;
                }
            }
            return TabsResult;
        }
        private int SaveToOutbounds(VMLeadActions model, int OrgID)
        {
            var database = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(database);
            var ActionMenu = Spdb.LeadActionMenus.Find(model.ID);
            var ActionTypes = Spdb.LeadActions.Find(ActionMenu.ActionType);
            Outbounds outbound = new Outbounds();
            VMLeads Lead = Spdb.Database.SqlQuery<VMLeads>("SELECT sMob, sEMail FROM " + EnumLeadTables.Leads.ToString() + " WHERE ID='" + model.LeadID + "'").FirstOrDefault();
            outbound.Email = Lead.sEmail;
            outbound.Mobile = Lead.sMob;
            outbound.OrganizationID = OrgID;
            outbound.LeadID = model.LeadID;
            if (ActionTypes.IsSMS)
            {
                outbound.Type = 2;
                outbound.TemplateID = ActionTypes.SMSTemplateID;
            }
            else if (ActionTypes.IsEmail)
            {
                outbound.Type = 1;
                outbound.TemplateID = ActionTypes.EmailTemplateID;
            }
            Spdb.Outbounds.Add(outbound);
            Spdb.SaveChanges();
            return 0;
        }

        //Getting the Template Name to the View Instead of ID
        private string GetTemplate(int p, int OrgID)
        {
            string database = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(database);
            var template = Spdb.ContentEditors.Where(m => m.ID == p).Select(m => m.Name).FirstOrDefault();
            return template;
        }

        public string SaveLeadTransaction(Stages model, int OrgID, string database, int UserID)
        {
            DataContext Spdb = new DataContext(database);
            var lead = Spdb.LeadTransitions.Where(m => m.LeadID == model.LeadID).ToList();
            var AllStages = dbcontext.Stages.Where(m => m.OrganizationID == OrgID || m.OrganizationID == 0).ToList();
            LeadTransitions Trans = new LeadTransitions();
            Trans.StageID = model.ID;
            Trans.LeadID = model.LeadID;
            Trans.OrganizationID = OrgID;
            if (lead.Count() == 0)
            {
                Trans.FromStatus = "Inbound";
            }
            else
            {
                Trans.FromStatus = lead.Last().ToStatus;
            }
            Trans.ToStatus = AllStages.Where(m => m.ID == model.ID).Select(m => m.Name).FirstOrDefault();
            Spdb.LeadTransitions.Add(Trans);
            Spdb.SaveChanges();
            Spdb.Database.ExecuteSqlCommand("update " + EnumLeadTables.Leads.ToString() + " set istatus='" + model.ID + "' where ID=" + model.LeadID);
            LeadStatus Sta = new LeadStatus();
            Sta.Status = AllStages.Where(m => m.ID == model.ID).Select(m => m.Name).FirstOrDefault();
            Sta.LeadID = model.LeadID;
            Sta.UserName = dbcontext.XIAppUsers.Where(m => m.UserID == UserID).Select(m => m.sFirstName).FirstOrDefault();
            Spdb.LeadStatus.Add(Sta);
            Spdb.SaveChanges();
            var LeadData = Spdb.Database.SqlQuery<VMLeads>("SELECT sMob, sEMail FROM " + EnumLeadTables.Leads.ToString() + " WHERE ID='" + model.LeadID + "'").FirstOrDefault();
            var Stage = dbcontext.Stages.Find(model.ID);
            if (Stage.IsEmail == true)
            {
                Outbounds outbound = new Outbounds();
                outbound.OrganizationID = OrgID;
                outbound.LeadID = model.LeadID;
                outbound.Mobile = LeadData.sMob;
                outbound.Email = LeadData.sEmail;
                outbound.Type = 1;
                outbound.TemplateID = 2;
                Spdb.Outbounds.Add(outbound);
                Spdb.SaveChanges();
            }
            if (Stage.IsSMS == true)
            {
                Outbounds outbound = new Outbounds();
                outbound.OrganizationID = OrgID;
                outbound.LeadID = model.LeadID;
                outbound.Mobile = LeadData.sMob;
                outbound.Email = LeadData.sEmail;
                outbound.Type = 2;
                outbound.TemplateID = 2;
                Spdb.Outbounds.Add(outbound);
                Spdb.SaveChanges();
            }
            var stage = getstagename(model.ID);
            return stage;
        }

        private string getstagename(int p)
        {
            var stage = dbcontext.Stages.Where(m => m.ID == p).Select(m => m.Name).FirstOrDefault();
            return stage;
        }
        public List<Stages> GetNextStages(int LeadID, int StageID, int OrgID, string database)
        {
            DataContext Spdb = new DataContext(database);
            List<Stages> AllStages = new List<Stages>();
            var NextStages = Spdb.StagesFlows.Where(m => m.OrganizationID == OrgID && m.StageID == StageID).Select(m => m.SubStages).FirstOrDefault();
            var NStages = NextStages.Split(',').ToList();
            foreach (var items in NStages)
            {
                int ID = Convert.ToInt32(items);
                var Stage = dbcontext.Stages.Where(m => m.ID == ID).FirstOrDefault();
                AllStages.Add(Stage);
            }
            return AllStages;
        }

        #endregion Popup

        #region Rules&History

        public int SaveImportHistories(ImportHistories model, string database)
        {
            DataContext Spdb = new DataContext(database);
            ImportHistories IH = new ImportHistories();
            if (model.ID == 0)
            {
                IH.OrganizationID = model.OrganizationID;
                IH.UserID = model.UserID;
                IH.FileType = model.FileType;
                IH.OriginalName = model.OriginalName;
                IH.FileName = model.FileName;
                IH.StatusTypeID = model.StatusTypeID;
                IH.ImportedOn = model.ImportedOn;
                Spdb.ImportHistories.Add(IH);
                Spdb.SaveChanges();
                return IH.ID;
            }
            else
            {
                IH = Spdb.ImportHistories.Find(model.ID);
                IH.OrganizationID = model.OrganizationID;
                IH.UserID = model.UserID;
                IH.FileType = model.FileType;
                IH.FileName = model.FileName;
                IH.StatusTypeID = model.StatusTypeID;
                IH.ImportedOn = model.ImportedOn;
                Spdb.SaveChanges();
                return IH.ID;
            }
        }
        //import rules
        public ImportRules EditImportRulesDetails(int ID)
        {
            ImportRules importrules = new ImportRules();
            importrules = dbcontext.ImportRules.Find(ID);
            return importrules;
        }
        //3
        public int SaveImportRulesDetails(ImportRules model)
        {
            ImportRules importrules = new ImportRules();
            if (model.ID == 0)
            {

                importrules.RuleName = model.RuleName;
                importrules.RuleValue = model.RuleValue;
                importrules.RuleType = model.RuleType;
                importrules.Count = model.Count;
                dbcontext.ImportRules.Add(importrules);
                dbcontext.SaveChanges();
                return importrules.ID;
            }
            else
            {
                importrules = dbcontext.ImportRules.Find(model.ID);
                importrules.RuleName = model.RuleName;
                importrules.RuleValue = model.RuleValue;
                importrules.RuleType = model.RuleType;
                importrules.Count = model.Count;
                dbcontext.SaveChanges();
                return 0;
            }
        }

        //R1
        public DTResponse ImportRulesDetailsList(jQueryDataTableParamModel param, int ID)
        {
            IQueryable<ImportRules> AllRules;
            AllRules = dbcontext.ImportRules;
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllRules = AllRules.Where(m => m.RuleName.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllRules.Count();
            AllRules = QuerableUtil.GetResultsForDataTables(AllRules, "", sortExpression, param);
            var clients = AllRules.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.RuleName,c.RuleValue,c.RuleType,c.Count.ToString(),"Edit" };
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        public DTResponse ErrorDetailsList(jQueryDataTableParamModel param, int OrgID, string database)
        {
            DataContext Spdb = new DataContext(database);
            IEnumerable<ImportHistories> AllDetails, FilteredDetails;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredDetails = Spdb.ImportHistories.Where(m => m.OrganizationID == OrgID).Where(m => m.FileName.Contains(param.sSearch.ToUpper())).ToList();
                AllDetails = FilteredDetails.OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = FilteredDetails.Count();
            }
            else
            {
                displyCount = Spdb.ImportHistories.Where(m => m.OrganizationID == OrgID).Count();
                AllDetails = Spdb.ImportHistories.Where(m => m.OrganizationID == OrgID).OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

            }
            var result = from c in AllDetails
                         select new[] {
                             (i++).ToString(),Convert.ToString(c.ID), c.OriginalName, c.FileType, c.ImportedOn.ToString(), c.StatusTypeID.ToString(), ""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        public List<ImportingErrorDetails> GetFileErrorDetails(int ID, int OrgID, string database)
        {
            DataContext Spdb = new DataContext(database);
            var Errors = Spdb.ImportingErrorDetails.Where(m => m.FileID == ID).ToList();
            return Errors;
        }
        #endregion Rules&History

        #region LeadConfigurations
        //Saving into Leads Configurations Table
        public int SaveLeadConfigurations(LeadConfigurations model)
        {
            int OrgID = model.OrganizationID;
            string database = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(database);
            LeadConfigurations LC = new LeadConfigurations();
            if (model.ID == 0)
            {
                LC.OrganizationID = model.OrganizationID;
                LC.Class = model.Class;
                LC.Settings = model.Settings;
                if (LC.Settings == "Insert if with x days")
                {
                    LC.Interval = model.Interval;
                }
                else
                {
                    LC.Interval = 0;
                }
                Spdb.LeadConfigurations.Add(LC);
                Spdb.SaveChanges();
                return LC.ID;
            }
            else
            {
                LC = Spdb.LeadConfigurations.Find(model.ID);
                //LC.OrganizationID = model.OrganizationID;
                //LC.Class = model.Class;
                LC.Settings = model.Settings;
                if (LC.Settings == "Insert if with x days")
                {
                    LC.Interval = model.Interval;
                }
                else
                {
                    LC.Interval = 0;
                }
                Spdb.SaveChanges();
                return LC.ID;
            }
        }
        //Getting the Editing Form
        public LeadConfigurations EditLeadConfigurations(int ID, int OrganizationID)
        {
            string database = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == OrganizationID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(database);
            LeadConfigurations LC = new LeadConfigurations();
            LC = Spdb.LeadConfigurations.Where(m => m.OrganizationID == OrganizationID && m.ID == ID).FirstOrDefault();
            return LC;
        }
        //Data Table
        public DTResponse LeadConfigurationsList(jQueryDataTableParamModel param, int OrgID, string database)
        {
            var dbs = dbcontext.XIAppUsers.Select(m => m.sDatabaseName).Distinct().ToList();
            IQueryable<LeadConfigurations> AllConfigs;
            List<LeadConfigurations> Configs = new List<LeadConfigurations>();
            if (OrgID == 0)
            {
                //AllPopups = db.LeadConfigurations.OrderBy(m => m.OrganizationID == OrgID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                foreach (var items in dbs)
                {
                    DataContext AllDb = new DataContext(items);
                    Configs.AddRange(AllDb.LeadConfigurations.OrderBy(m => m.OrganizationID == OrgID).ToList());
                }
                AllConfigs = Configs.AsQueryable();
            }
            else
            {
                database = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
                DataContext AllDb = new DataContext(database);
                AllConfigs = AllDb.LeadConfigurations.Where(m => m.OrganizationID == OrgID);
            }
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                //AllConfigs = AllConfigs.Where(m => m.Name.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllConfigs.Count();
            AllConfigs = QuerableUtil.GetResultsForDataTables(AllConfigs, "", sortExpression, param);
            var clients = AllConfigs.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID),c.OrganizationID.ToString(),GetOrgName(c.OrganizationID),GetClasses(c.Class),c.Settings, Convert.ToString(c.Interval),"Edit" };
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        #endregion LeadConfigurations

        #region Actions
        public int SaveAction(VMActionTypes model)
        {
            int OrgID = model.OrganizationID;
            string database = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext db = new DataContext(database);
            LeadActions action = new LeadActions();
            if (model.ID == 0)
            {
                action.OrganizationID = model.OrganizationID;
                action.Name = model.Name;
                action.Type = model.Type;
                action.IsStage = model.IsStage;
                if (model.IsStage)
                {
                    action.StageID = model.StageID;
                }
                action.IsSMS = model.IsSMS;
                if (model.IsSMS)
                {
                    action.SMSTemplateID = model.SMSTemplateID;
                }
                action.IsEmail = model.IsEmail;
                if (model.IsEmail)
                {
                    action.EmailTemplateID = model.EmailTemplateID;
                }
                action.IsPopup = model.IsPopup;
                if (model.IsPopup)
                {
                    action.PopupID = model.PopupID;
                }
                action.IsOneClcik = model.IsOneClick;
                if (model.IsOneClick)
                {
                    action.Query = model.Query;
                }
                action.StatusTypeID = model.StatusTypeID;
                db.LeadActions.Add(action);
                db.SaveChanges();
            }
            else
            {
                action = db.LeadActions.Find(model.ID);
                action.Name = model.Name;
                action.Type = model.Type;
                action.IsStage = model.IsStage;
                if (model.IsStage)
                {
                    action.StageID = model.StageID;
                }
                else
                {
                    action.StageID = 0;
                }
                action.IsSMS = model.IsSMS;
                if (model.IsSMS)
                {
                    action.SMSTemplateID = model.SMSTemplateID;
                }
                else
                {
                    action.SMSTemplateID = 0;
                }
                action.IsEmail = model.IsEmail;
                if (model.IsEmail)
                {
                    action.EmailTemplateID = model.EmailTemplateID;
                }
                else
                {
                    action.EmailTemplateID = 0;
                }
                action.IsPopup = model.IsPopup;
                if (model.IsPopup)
                {
                    action.PopupID = model.PopupID;
                }
                else
                {
                    action.PopupID = 0;
                }
                action.IsOneClcik = model.IsOneClick;
                if (model.IsOneClick)
                {
                    action.Query = model.Query;
                }
                else
                {
                    action.Query = null;
                }
                action.StatusTypeID = model.StatusTypeID;
                db.SaveChanges();
            }
            return action.ID;
        }

        public DTResponse GetActionsList(jQueryDataTableParamModel param, int OrgID, string database)
        {
            var dbs = dbcontext.XIAppUsers.Select(m => m.sDatabaseName).Distinct().ToList();
            IQueryable<LeadActions> AllActions;
            List<LeadActions> Actions = new List<LeadActions>();
            if (OrgID == 0)
            {
                //AllPopups = db.LeadConfigurations.OrderBy(m => m.OrganizationID == OrgID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                foreach (var items in dbs)
                {
                    DataContext AllDb = new DataContext(items);
                    Actions.AddRange(AllDb.LeadActions.ToList());
                }
                AllActions = Actions.AsQueryable();
            }
            else
            {
                DataContext AllDb = new DataContext(database);
                Actions.AddRange(AllDb.LeadActions.Where(m => m.OrganizationID == OrgID).ToList());
                AllActions = Actions.AsQueryable();
            }
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllActions = AllActions.Where(m => m.Name.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllActions.Count();
            AllActions = QuerableUtil.GetResultsForDataTables(AllActions, "", sortExpression, param);
            var clients = AllActions.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID),c.OrganizationID.ToString(),GetOrgName(c.OrganizationID), c.Name, c.StatusTypeID.ToString(),""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        //Checking Wheather Action Name Exists or not
        public bool IsExistsActionName(string Name, int ID, int OrganizationID)
        {
            string database = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == OrganizationID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext db = new DataContext(database);

            var actions = db.LeadActions.ToList();
            var action = actions.Where(m => m.OrganizationID == OrganizationID).Where(m => m.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (ID == 0)
            {
                if (action != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (action != null)
                {
                    if (ID == action.ID)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        //Getting the LeadAction Form To Edit
        public VMActionTypes EditAction(int ActionID, int OrganizationID)
        {
            string database = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == OrganizationID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(database);
            VMActionTypes action = new VMActionTypes();
            var Action = Spdb.LeadActions.Find(ActionID);
            action.EmailTemplateID = Action.EmailTemplateID;
            action.ID = Action.ID;
            action.Name = Action.Name;
            action.OrganizationID = Action.OrganizationID;
            action.StatusTypeID = Action.StatusTypeID;
            action.Type = Action.Type;
            action.EmailTemplateID = Action.EmailTemplateID;
            action.IsEmail = Action.IsEmail;
            action.IsOneClick = Action.IsOneClcik;
            action.IsPopup = Action.IsPopup;
            action.IsSMS = Action.IsSMS;
            action.IsStage = Action.IsStage;
            action.PopupID = Action.PopupID;
            action.Query = Action.Query;
            action.SMSTemplateID = Action.SMSTemplateID;
            action.StageID = Action.StageID;
            return action;
        }
        //Getting the Organizations in Dropdown
        public List<VMDropDown> GetOrganizations()
        {
            var orgs = dbcontext.Organization.ToList();
            List<VMDropDown> AllOrgs = new List<VMDropDown>();
            AllOrgs = (from c in orgs
                       select new VMDropDown { Value = c.ID, text = c.Name }).ToList();
            return AllOrgs;
        }

        public List<VMDropDown> GetActionTypes(int OrgID, string database)
        {
            DataContext Spdb = new DataContext(database);
            List<VMDropDown> AllActionTypes = new List<VMDropDown>();
            AllActionTypes = (from c in Spdb.LeadActions.Where(m => m.StatusTypeID == 10 && m.OrganizationID == OrgID).ToList()
                              select new VMDropDown { text = c.Name, Value = c.ID }).ToList();
            return AllActionTypes;
        }
        public VMCustomResponse SaveActionMenu(LeadActionMenus model, int OrgID, string database)
        {
            DataContext Spdb = new DataContext(database);
            LeadActionMenus Menu = new LeadActionMenus();
            if (model.ID == 0)
            {
                Menu.OrganizationID = model.OrganizationID;
                Menu.Name = model.Name;
                Menu.ActionType = model.ActionType;
                Menu.Type = model.Type;
                Menu.StatusTypeID = model.StatusTypeID;
                Spdb.LeadActionMenus.Add(Menu);
                Spdb.SaveChanges();
                //return Menu.ID;
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = Menu.ID, Status = true };
            }
            else
            {
                LeadActionMenus ActionMenu = Spdb.LeadActionMenus.Find(model.ID);
                ActionMenu.Name = model.Name;
                ActionMenu.ActionType = model.ActionType;
                ActionMenu.Type = model.Type;
                ActionMenu.StatusTypeID = model.StatusTypeID;
                Spdb.SaveChanges();
                //return ActionMenu.ID;
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = ActionMenu.ID, Status = true };
            }

        }

        public bool IsExistsActionMenuName(string Name, int ID, int OrganizationID)
        {
            string database = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == OrganizationID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext db = new DataContext(database);

            var actions = db.LeadActionMenus.ToList();
            var action = actions.Where(m => m.OrganizationID == OrganizationID).Where(m => m.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (ID == 0)
            {
                if (action != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (action != null)
                {
                    if (ID == action.ID)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        public DTResponse GetActionMenusList(jQueryDataTableParamModel param, int OrgID, string database)
        {
            IQueryable<LeadActionMenus> AllMenus;
            List<LeadActionMenus> Menus = new List<LeadActionMenus>();
            var dbs = dbcontext.XIAppUsers.Select(m => m.sDatabaseName).Distinct().ToList();
            if (OrgID == 0)
            {
                //AllPopups = db.LeadConfigurations.OrderBy(m => m.OrganizationID == OrgID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                foreach (var items in dbs)
                {
                    DataContext AllDb = new DataContext(items);
                    Menus.AddRange(AllDb.LeadActionMenus.ToList());
                }
                AllMenus = Menus.AsQueryable(); ;
            }
            else
            {
                DataContext AllDb = new DataContext(database);
                Menus.AddRange(AllDb.LeadActionMenus.Where(m => m.OrganizationID == OrgID).ToList());
                AllMenus = Menus.AsQueryable();
            }
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllMenus = AllMenus.Where(m => m.Name.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllMenus.Count();
            AllMenus = QuerableUtil.GetResultsForDataTables(AllMenus, "", sortExpression, param);
            var clients = AllMenus.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID),c.OrganizationID.ToString(),GetOrgName(c.OrganizationID), c.Name, c.StatusTypeID.ToString(),""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        public LeadActionMenus GetActionMenuByID(int ID, int OrgID, string database)
        {
            DataContext Spdb = new DataContext(database);
            LeadActionMenus Menu = Spdb.LeadActionMenus.Find(ID);
            return Menu;
        }

        #endregion Actions

        #region TabGraphs
        private List<VMKPIResult> KPICirlceForTab(int ReportID, int UserID, int OrgID, string sOrgName)
        {
            var database = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            string Query = "";
            int target = 0;
            int RoleID = dbcontext.XIAppUserRoles.Where(m => m.UserID == UserID).Select(m => m.RoleID).FirstOrDefault();
            List<Reports> Reports = new List<Reports>();
            List<Reports> AllReports = dbcontext.Reports.Where(m => m.ID == ReportID).ToList();
            foreach (var item in AllReports)
            {
                Reports.Add(item);
            }
            List<VMKPIResult> KPIs = new List<VMKPIResult>();
            KPICircleColors colors = new KPICircleColors();
            KPIIconColors iconscolor = new KPIIconColors();
            List<string> color = new List<string>();
            List<string> iconcolor = new List<string>();
            foreach (var items in colors)
            {
                string str = Convert.ToString(items.KPIColor);
                color.Add(str);
            }
            foreach (var items in iconscolor)
            {
                string str = Convert.ToString(items.KPIColor);
                iconcolor.Add(str);
            }
            int j = 0;
            foreach (var items in Reports)
            {
                Reports report = dbcontext.Reports.Find(items.ID);
                Query = report.Query;
                target = 10;
                Common Com = new Common();
                string UserIDs = Com.GetSubUsers(UserID, OrgID, database, sOrgName);
                if (Query != null && Query.Length > 0)
                {
                    Query = ServiceUtil.ReplaceQueryContent(Query, UserIDs, UserID, OrgID, 0, 0);
                    string[] value = null;
                    VMKPIResult kpi = new VMKPIResult();
                    using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                    {
                        Con.Open();
                        Con.ChangeDatabase(database);
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = Con;
                        cmd.CommandText = Query;
                        SqlDataReader reader = cmd.ExecuteReader();
                        List<string[]> results = new List<string[]>();
                        int count = reader.FieldCount;
                        string[] rows = new string[count];
                        while (reader.Read())
                        {
                            List<string> values = new List<string>();
                            for (int i = 0; i < count; i++)
                            {
                                values.Add(reader.GetValue(i).ToString());
                            }
                            string[] result = values.ToArray();
                            results.Add(result);
                            value = result;
                        }
                        Con.Close();
                    }
                    int com = Convert.ToInt32(value[0]);
                    double percentage = (double)com / target;
                    int completed = (int)Math.Round(percentage * 100, 0);
                    kpi.Name = report.Name;
                    kpi.KPIPercent = completed;
                    kpi.KPIValue = completed + "%";
                    kpi.KPICircleColor = color[j];
                    kpi.KPIIconColor = iconcolor[j];
                    kpi.KPIIcon = "fa fa-car";
                    KPIs.Add(kpi);
                    j++;
                }
            }
            return KPIs;
        }

        private List<DashBoardGraphs> GetPieChartForTab(int UserID, string database, int OrganizationID, int ReportID, string sOrgName)
        {
            //var roels = GetParentRoles(UserRoleID);
            //roels.Add(UserRoleID);
            string QueryName = "";
            Common Com = new Common();
            string UserIDs = Com.GetSubUsers(UserID, OrganizationID, database, sOrgName);
            List<Reports> ids = new List<Reports>();
            List<Reports> AllReports = dbcontext.Reports.Where(m => m.ID == ReportID).ToList();
            foreach (var reports in AllReports)
            {
                ids.Add(reports);
            }
            List<DashBoardGraphs> list = new List<DashBoardGraphs>();
            List<string[]> results = new List<string[]>();
            foreach (var items in ids)
            {
                Reports model = dbcontext.Reports.Find(items.ID);
                QueryName = model.Name;
                string Query = model.Query;
                Query = ServiceUtil.ReplaceQueryContent(Query, UserIDs, UserID, OrganizationID, 0, 0);
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    Con.Open();
                    Con.ChangeDatabase(database);
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    cmd.CommandText = Query;
                    SqlDataReader reader = cmd.ExecuteReader();
                    VMKPIResult kpi = new VMKPIResult();
                    int count = reader.FieldCount;
                    string[] rows = new string[count];
                    string[] value = null;
                    while (reader.Read())
                    {
                        List<string> values = new List<string>();
                        for (int i = 0; i < count; i++)
                        {
                            values.Add(reader.GetValue(i).ToString());
                        }
                        string[] result = values.ToArray();
                        results.Add(result);
                        value = result;
                    }
                    var Keys = ServiceUtil.GetForeginkeyValues(model.Query);
                    if (reader.HasRows == true)
                    {
                        foreach (var items1 in results)
                        {
                            for (int i = 0; i < items1.Count(); i++)
                            {
                                DashBoardGraphs model1 = new DashBoardGraphs();
                                //int ID = Convert.ToInt32(items1[0]);
                                var Name = ServiceUtil.ReplaceForeginKeyValues(Keys[0], items1[0], database);
                                if (Name != null)
                                {
                                    model1.label = Name;
                                    model1.value = Convert.ToInt32(items1[1]);
                                    list.Add(model1);
                                }
                            }
                        }
                    }
                    else
                    {
                        DashBoardGraphs model1 = new DashBoardGraphs();
                        list.Add(model1);
                    }
                    Con.Close();
                }
            }

            list.FirstOrDefault().QueryName = QueryName;
            return list;
        }

        private LineGraph GetBarChartForTab(int UserID, string database, int OrganizationID, int ReportID, string sOrgName)
        {
            string QueryName = "";
            Common Com = new Common();
            string UserIDs = Com.GetSubUsers(UserID, OrganizationID, database, sOrgName);
            LineGraph line = new LineGraph();
            //var roels = GetParentRoles(UserRoleID);
            //roels.Add(UserRoleID);
            List<Reports> ids = new List<Reports>();
            List<Reports> AllReports = dbcontext.Reports.Where(m => m.ID == ReportID).ToList();
            foreach (var reports in AllReports)
            {
                ids.Add(reports);
            }
            List<DashBoardGraphs> list = new List<DashBoardGraphs>();
            List<string[]> results = new List<string[]>();
            foreach (var items in ids)
            {
                Reports model = dbcontext.Reports.Find(items.ID);
                QueryName = model.Name;
                string Query = model.Query;
                Query = ServiceUtil.ReplaceQueryContent(Query, UserIDs, UserID, OrganizationID, 0, 0);
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    Con.Open();
                    Con.ChangeDatabase(database);
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    cmd.CommandText = Query;
                    SqlDataReader reader = cmd.ExecuteReader();

                    VMKPIResult kpi = new VMKPIResult();
                    int count = reader.FieldCount;
                    string[] rows = new string[count];
                    string[] value = null;
                    while (reader.Read())
                    {
                        List<string> values = new List<string>();
                        for (int i = 0; i < count; i++)
                        {
                            values.Add(reader.GetValue(i).ToString());
                        }
                        string[] result = values.ToArray();
                        results.Add(result);
                        value = result;
                    }
                    Con.Close();
                }
                List<List<string>> Chart = new List<List<string>>();
                List<string> XValues = new List<string>();
                List<string> xval = new List<string>();
                XValues.Add("x");
                var Keys = ServiceUtil.GetForeginkeyValues(model.Query);
                if (Keys.Count() > 1)
                {
                    string Name = "";
                    xval = results.Select(m => m[0]).Distinct().ToList();
                    foreach (var item in xval)
                    {
                        Name = ServiceUtil.ReplaceForeginKeyValues(Keys[0], item, database);
                        XValues.Add(Name);
                    }
                }
                else
                {
                    xval = results.Select(m => m[0]).Distinct().ToList();
                    XValues.AddRange(xval);
                }
                Chart.Add(XValues);
                var types = results.Select(m => m[1]).Distinct();
                foreach (var type in types)
                {
                    List<string> Y = new List<string>();
                    //var ID = Convert.ToInt32(type);
                    string Name = "";
                    if (Keys.Count() > 1)
                    {
                        Name = ServiceUtil.ReplaceForeginKeyValues(Keys[1], type, database);
                    }
                    else
                    {
                        Name = ServiceUtil.ReplaceForeginKeyValues(Keys[0], type, database);
                    }
                    if (Name != null)
                    {
                        Y = new List<string> { Name };
                        foreach (var xaxis in xval)
                        {
                            var yvalue = results.Where(m => m[1] == type && m[0] == xaxis).Select(m => m[2].ToString()).FirstOrDefault();
                            Y.Add(string.IsNullOrWhiteSpace(yvalue) ? "0" : yvalue);
                        }
                        Chart.Add(Y);
                    }
                }

                line.Data = Chart;
                line.QueryName = model.Name;
            }
            return line;
        }

        private LineGraph GetLineGraphForTab(int ReportID, int OrgID, int UserID, string sOrgName)
        {
            LineGraph line = new LineGraph();
            var database = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            string Query = "";
            int RoleID = dbcontext.XIAppUserRoles.Where(m => m.UserID == UserID).Select(m => m.RoleID).FirstOrDefault();
            List<Reports> Reports = new List<Reports>();
            List<Reports> AllReports = dbcontext.Reports.Where(m => m.ID == ReportID).ToList();
            foreach (var item in AllReports)
            {
                Reports.Add(item);
            }
            List<string[]> results = new List<string[]>();
            foreach (var items in Reports)
            {
                Reports report = dbcontext.Reports.Find(ReportID);
                Query = report.Query;
                Common Com = new Common();
                string UserIDs = Com.GetSubUsers(UserID, OrgID, database, sOrgName);
                Query = ServiceUtil.ReplaceQueryContent(Query, UserIDs, UserID, OrgID, 0, 0);
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    Con.Open();
                    Con.ChangeDatabase(database);
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    cmd.CommandText = Query;
                    SqlDataReader reader = cmd.ExecuteReader();
                    VMKPIResult kpi = new VMKPIResult();
                    int count = reader.FieldCount;
                    string[] rows = new string[count];
                    string[] value = null;
                    while (reader.Read())
                    {
                        List<string> values = new List<string>();
                        for (int i = 0; i < count; i++)
                        {
                            values.Add(reader.GetValue(i).ToString());
                        }
                        string[] result = values.ToArray();
                        results.Add(result);
                        value = result;
                    }
                    Con.Close();
                }
                List<List<string>> Chart = new List<List<string>>();
                List<string> XValues = new List<string>();
                List<string> xval = new List<string>();
                XValues.Add("x");
                var Keys = ServiceUtil.GetForeginkeyValues(report.Query);
                if (Keys.Count() > 1)
                {
                    string Name = "";
                    xval = results.Select(m => m[0]).Distinct().ToList();
                    foreach (var item in xval)
                    {
                        Name = ServiceUtil.ReplaceForeginKeyValues(Keys[0], item, database);
                        XValues.Add(Name);
                    }
                }
                else
                {
                    xval = results.Select(m => m[0]).Distinct().ToList();
                    XValues.AddRange(xval);
                }
                Chart.Add(XValues);
                var types = results.Select(m => m[1]).Distinct();
                foreach (var type in types)
                {
                    List<string> Y = new List<string>();
                    //var ID = Convert.ToInt32(type);
                    string Name = "";
                    if (Keys.Count() > 1)
                    {
                        Name = ServiceUtil.ReplaceForeginKeyValues(Keys[1], type, database);
                    }
                    else
                    {
                        Name = ServiceUtil.ReplaceForeginKeyValues(Keys[0], type, database);
                    }
                    if (Name != null)
                    {
                        Y = new List<string> { Name };
                        foreach (var xaxis in xval)
                        {
                            var yvalue = results.Where(m => m[1] == type && m[0] == xaxis).Select(m => m[2].ToString()).FirstOrDefault();
                            Y.Add(string.IsNullOrWhiteSpace(yvalue) ? "0" : yvalue);
                        }
                        Chart.Add(Y);
                    }
                }
                line.Data = Chart;
                line.QueryName = report.Name;
            }
            return line;
        }
        #endregion TabGraphs

        #region TabResultList
        public DTResponse GetReportResult(jQueryDataTableParamModel param, VMQuickSearch Search, string sDatabase, int iUserID, string sOrgName)
        {
            List<string> AllHeadings = new List<string>();
            try
            {
                ModelDbContext dbContext = new ModelDbContext(sDatabase);
                var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
                VMResultList vmquery = new VMResultList();
                DataContext Spdb = new DataContext(sOrgDB);
                Reports query = dbcontext.Reports.Find(Search.ReportID);
                int BOID = query.BOID;
                BOs Bo = dbcontext.BOs.Find(BOID);
                Common Com = new Common();
                var FromIndex = query.Query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
                var SelectQuery = query.Query.Substring(0, FromIndex);
                var Keys = ServiceUtil.GetForeginkeyValues(SelectQuery);
                List<string> Headings = new List<string>();
                List<string> Formatting = new List<string>();
                List<string> Scripts = new List<string>();
                var Heads = Com.GetHeadings(Search.FKi1ClickIDXIGUID, Search.database, Search.OrgID, Search.UserID, sOrgName);
                List<VMDropDown> KeyPositions = Heads.FKPositions;
                Headings = Heads.Headings;
                Formatting = Heads.Formats;
                Scripts = Heads.Scripts;
                string NewQuery = Heads.Query;
                string UserIDs = ""; //Com.GetSubUsers(Search.UserID, Search.OrgID);
                string Query = ServiceUtil.ReplaceQueryContent(NewQuery, UserIDs, Search.UserID, Search.OrgID, Search.LeadID, 0);
                //if (Bo.Name == "WalletQuotes" || Bo.Name == "WalletPolicies")
                //{
                //    Query = Com.AddSearchParameters(Query, "LeadID=" + Search.LeadID);
                //}
                var TabOptions = dbcontext.Tab1Clicks.Where(m => m.TabID == Search.TabID && m.ReportID == Search.ReportID).FirstOrDefault();
                List<object[]> TotalResult = new List<object[]>();
                List<string[]> results = new List<string[]>();
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    Con.Open();
                    Con.ChangeDatabase(Search.database);
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    cmd.CommandText = Query;
                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable data = new DataTable();
                    data.Load(reader);
                    TotalResult = data.AsEnumerable().Select(m => m.ItemArray).ToList();
                    List<object[]> Res = new List<object[]>();
                    if (query.ResultListDisplayType == 1)
                    {
                        Res = TotalResult.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                    }
                    for (int i = 0; i < Res.Count(); i++)
                    {
                        List<string> NewRes = new List<string>();
                        for (int j = 0; j < Res[i].Count(); j++)
                        {
                            var pos = KeyPositions.Where(m => m.Value == j).FirstOrDefault();
                            if (Scripts[j] != null && Scripts[j].Length > 0)
                            {
                                DataTable Cdata = new DataTable();
                                var scrs = new List<List<string>>();
                                List<List<string>> Cols = new List<List<string>>();
                                var Script = Scripts[j].Split(',').ToList();
                                string col = "";
                                foreach (var items in Script)
                                {
                                    var scr = items.Substring(1, items.Length - 2).Split('.').ToList();
                                    scrs.Add(scr);
                                    col = col + scr[1] + ", ";
                                    List<string> Val = new List<string>();
                                    Val.AddRange(scr);
                                    Cols.Add(Val);
                                }
                                col = col.Substring(0, col.Length - 2);
                                var FIndex = Query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
                                var CodingQuery = "Select " + col + " " + Query.Substring(FIndex, Query.Length - FIndex);
                                SqlCommand cmmd = new SqlCommand();
                                cmmd.Connection = Con;
                                cmmd.CommandText = CodingQuery;
                                reader = cmmd.ExecuteReader();
                                Cdata.Load(reader);
                                reader.Close();
                                List<object[]> CodeResult = Cdata.AsEnumerable().Select(m => m.ItemArray).ToList();
                                List<object[]> Codes = new List<object[]>();
                                Codes = CodeResult.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                                string Resutl = "";
                                for (int h = 0; h < 3; h++)
                                {
                                    if (scrs[h][2] == "formattedvalue")
                                    {
                                        var Value = ServiceUtil.ReplaceForeginKeyValues(new VMDropDown { text = scrs[h][1] }, Codes[i][h].ToString(), Search.database);
                                        if (Value != null && Value.Length > 0)
                                        {
                                            Resutl = Resutl + Value + ",";
                                        }
                                        else
                                        {
                                            Resutl = Resutl + Codes[i][h] + ",";
                                        }
                                    }
                                    else
                                    {
                                        Resutl = Resutl + Codes[i][h] + ",";
                                    }
                                }
                                Resutl = Resutl.Substring(0, Resutl.Length - 1);
                                NewRes.Add(Resutl);
                            }
                            else if (pos != null)
                            {
                                var DbValue = Res[i][j];
                                if (DbValue != null)
                                {
                                    var Value = ServiceUtil.ReplaceForeginKeyValues(pos, Res[i][j].ToString(), Search.database);
                                    NewRes.Add(Value);
                                }
                                else
                                {
                                    NewRes.Add(null);
                                }
                            }
                            else
                            {
                                if (Formatting[j] != null)
                                {
                                    if (Formatting[j] == "%")
                                    {
                                        NewRes.Add(string.Format("{0}%", Res[i][j]));
                                    }
                                    else if (Formatting[j] == "en-GB")
                                    {
                                        CultureInfo rgi = new CultureInfo(Formatting[j]);
                                        string totalValueCurrency = string.Format(rgi, "{0:c}", Res[i][j]);
                                        NewRes.Add(totalValueCurrency);
                                    }
                                    else
                                    {
                                        NewRes.Add(String.Format("{0:" + Formatting[j] + "}", Res[i][j]));
                                    }
                                }
                                else
                                {
                                    NewRes.Add(Res[i][j].ToString());
                                }
                            }
                        }
                        if (Bo.Name == EnumLeadTables.WalletQuotes.ToString() || Bo.Name == EnumLeadTables.WalletPolicies.ToString())
                        {
                            int QtID = Convert.ToInt32(NewRes[0]);

                            bool IsPosted = false;
                            if (Bo.Name == EnumLeadTables.WalletQuotes.ToString())
                            {
                                IsPosted = Spdb.WalletQuotes.Where(m => m.ID == QtID && m.OrganizationID == Search.OrgID && m.LeadID == Search.LeadID).Select(m => m.IsPosted).FirstOrDefault();
                            }
                            else
                            {
                                IsPosted = Spdb.WalletPolicies.Where(m => m.ID == QtID && m.OrganizationID == Search.OrgID && m.LeadID == Search.LeadID).Select(m => m.IsPosted).FirstOrDefault();
                            }

                            if (!IsPosted)
                            {
                                NewRes.Add("<button data-leadid='" + NewRes[0] + "' class='font-red bg-transparent no-border PostFromRow GridButtons'>Post</button>");
                                if (TabOptions.IsEdit)
                                {
                                    NewRes.Add("<button data-leadid='" + NewRes[0] + "' class='font-yellow-casablanca bg-transparent no-border EditRecordFromRow GridButtons'>Edit</button>");
                                }
                                else
                                {
                                    NewRes.Add("");
                                }
                            }
                            else
                            {
                                NewRes.Add("<button class='font-red bg-transparent no-border' disabled='disabled'>Posted</button>");
                                NewRes.Add("");
                            }
                        }
                        else
                        {
                            NewRes.Add("");
                            NewRes.Add("");
                        }
                        //NewRes.Add("<button data-leadid='" + NewRes[0] + "' class='font-red bg-transparent no-border DeleteRecordFromRow GridButtons'>Delete</button>");
                        results.Add(NewRes.ToArray());
                    }
                    Con.Close();
                }
                return new DTResponse()
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalResult.Count(),
                    iTotalDisplayRecords = TotalResult.Count(),
                    aaData = results,
                    Headings = vmquery.Headings
                };
            }
            catch (Exception ex)
            {
                return new DTResponse()
                {
                    sEcho = param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new List<string[]>(),
                    Headings = AllHeadings
                };
            }
        }
        public VMQueryPreview QueryDynamicForm(int Tab1clickID, int? leadid, string database, int orgid)
        {
            //string database = dbcontext.AspNetUsers.Where(m=>m.OrganizationID==orgid).Select(m=>m.DatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(database);
            VMQueryPreview vmquery = new VMQueryPreview();
            Tab1Clicks tab1click = dbcontext.Tab1Clicks.Find(Tab1clickID);
            Reports query = dbcontext.Reports.Find(tab1click.ReportID);
            List<string> viewfields = new List<string>();
            List<string> editfields = new List<string>();
            List<string> IsDropDown = new List<string>();
            List<List<VMDropDown>> DropDownValues = new List<List<VMDropDown>>();
            BOs BoDetails = dbcontext.BOs.Find(query.BOID);
            string tablename = BoDetails.Name;
            var MappedFields = Spdb.MappedFields.Where(m => m.OrganizationID == orgid && m.ClassID == tab1click.ClassID).ToList();
            var BFields = dbcontext.BOFields.Where(m => m.BOID == query.BOID).ToList();
            var Popups = dbcontext.Popups.Where(m => m.StatusTypeID == 10).ToList();
            vmquery.ActionPopUp = query.ActionFieldValue;
            if (leadid != null)
            {
                vmquery.LeadID = Convert.ToInt32(leadid);
                var sectionids = tab1click.SectionID;
                var vfields = tab1click.ViewFields;
                var efields = tab1click.EditFields;
                List<string> VFields = new List<string>();
                List<string> EFields = new List<string>();
                List<SectionsData> Data = new List<SectionsData>();
                List<string> EditFields = new List<string>();
                List<string> secids = new List<string>();
                List<string> Editdatatypes = new List<string>();
                List<string> EditLengths = new List<string>();
                List<string> EditDescs = new List<string>();
                List<string> ViewDropDowns = new List<string>();
                List<string> ViewDDLTable = new List<string>();
                List<string> IsViewFK = new List<string>();
                List<int> ViewFKPopupID = new List<int>();
                string viewfieldforquery = "";
                if (sectionids != "0")
                {
                    if (sectionids.Contains(','))
                    {
                        secids = sectionids.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    }
                    else
                    {
                        secids = sectionids.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    }
                }
                else if (sectionids == "0")
                {
                    secids.Add("0");
                }
                for (int i = 0; i < secids.Count(); i++)
                {
                    SectionsData data = new SectionsData();
                    if (vfields != null)
                    {
                        string aliasname = "";
                        List<string> AliasViewFiels = new List<string>();
                        VFields = vfields.Split('/').ToList();
                        data.ViewFields = VFields[i].Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var items in data.ViewFields)
                        {
                            if (items.Contains("NE-"))
                            {
                                var editname = items.Split('-')[1];
                                aliasname = MappedFields.Where(m => m.AddField == items).Select(m => m.FieldName).FirstOrDefault();
                                if (aliasname == null)
                                {
                                    var BFld = BFields.Where(m => m.Name == editname && m.BOID == query.BOID).FirstOrDefault();
                                    aliasname = BFld.LabelName;
                                    var PopID = dbcontext.Popups.Where(m => m.FKColumnID == BFld.ID).FirstOrDefault();
                                    if (PopID != null)
                                    {
                                        IsViewFK.Add("True");
                                        ViewFKPopupID.Add(PopID.ID);
                                    }
                                    else
                                    {
                                        IsViewFK.Add("False");
                                        ViewFKPopupID.Add(0);
                                    }
                                }
                                else
                                {
                                    IsViewFK.Add("False");
                                    ViewFKPopupID.Add(0);
                                }
                                AliasViewFiels.Add("NE-" + aliasname);
                                viewfieldforquery = viewfieldforquery + editname + ", ";
                                ViewDropDowns.Add("False");
                                ViewDDLTable.Add(null);
                            }
                            else
                            {
                                var MapFld = MappedFields.Where(m => m.AddField == items).FirstOrDefault();
                                if (MapFld == null)
                                {
                                    var BFld = BFields.Where(m => m.Name == items && m.BOID == query.BOID).FirstOrDefault();
                                    aliasname = BFld.LabelName;
                                    if (BFld.FKTableName != null && BFld.FKTableName.Length > 0)
                                    {
                                        ViewDropDowns.Add("True");
                                        ViewDDLTable.Add(BFld.FKTableName);
                                    }
                                    else
                                    {
                                        ViewDropDowns.Add("False");
                                        ViewDDLTable.Add(null);
                                    }
                                    var PopID = dbcontext.Popups.Where(m => m.FKColumnID == BFld.ID).FirstOrDefault();
                                    if (PopID != null)
                                    {
                                        IsViewFK.Add("True");
                                        ViewFKPopupID.Add(PopID.ID);
                                    }
                                    else
                                    {
                                        IsViewFK.Add("False");
                                        ViewFKPopupID.Add(0);
                                    }
                                }
                                else
                                {
                                    aliasname = MapFld.FieldName;
                                    if (MapFld.IsDropDown)
                                    {
                                        ViewDropDowns.Add("True");
                                        ViewDDLTable.Add("Types");
                                    }
                                    else
                                    {
                                        ViewDropDowns.Add("False");
                                        ViewDDLTable.Add(null);
                                    }
                                    IsViewFK.Add("False");
                                    ViewFKPopupID.Add(0);
                                }
                                AliasViewFiels.Add(aliasname);
                                viewfieldforquery = viewfieldforquery + items + ", ";
                            }
                        }
                        data.ViewFields = AliasViewFiels;
                        data.IsViewFK = IsViewFK;
                        data.ViewFKPopuID = ViewFKPopupID;
                    }
                    int SectionID = Convert.ToInt32(secids[i]);
                    data.SectionID = Convert.ToInt32(secids[i]);
                    data.Tab1ClickID = Tab1clickID;
                    data.LeadID = leadid;
                    data.TabID = tab1click.TabID;
                    data.IsView = tab1click.IsView;
                    data.IsEdit = tab1click.IsEdit;
                    data.IsCreate = tab1click.IsCreate;
                    data.Rank = dbcontext.Sections.Where(m => m.ID == SectionID).Select(m => m.Rank).FirstOrDefault();
                    if (SectionID != 0)
                    {
                        var sections = dbcontext.Sections.ToList();
                        data.SectionName = sections.Where(m => m.ID == SectionID).Select(m => m.Name).FirstOrDefault();
                        data.IsBespoke = sections.Where(m => m.ID == SectionID).Select(m => m.IsBespoke).FirstOrDefault();
                        if (data.IsBespoke == true)
                        {
                            data.URL = sections.Where(m => m.ID == SectionID).Select(m => m.URL).FirstOrDefault();
                            data.RefreshType = sections.Where(m => m.ID == SectionID).Select(m => m.RefreshType).FirstOrDefault();
                        }
                    }
                    string editfieldforquery = "";
                    if (efields != null && efields.Length > 0)
                    {
                        var EDFields = efields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var field in EDFields)
                        {
                            if (field != null)
                            {
                                if (field.Contains("NE-"))
                                {
                                    string FieldName = "";
                                    var NonEdit = field.Replace("NE-", "");
                                    var fields = MappedFields.Where(m => m.AddField == field).FirstOrDefault();
                                    if (fields == null)
                                    {
                                        var AName = BFields.Where(m => m.Name == NonEdit && m.BOID == query.BOID).Select(m => m.LabelName).FirstOrDefault();
                                        FieldName = NonEdit;
                                        EditFields.Add("NE-" + AName);
                                        EditDescs.Add(null);
                                    }
                                    else
                                    {
                                        FieldName = fields.FieldName;
                                        EditDescs.Add(fields.Description);
                                        EditFields.Add(FieldName);
                                    }
                                    var bofield = BFields.Where(m => m.BOID == query.BOID).Where(m => m.Name == FieldName).FirstOrDefault();
                                    string type = ((BODatatypes)bofield.TypeID).ToString();
                                    Editdatatypes.Add(type);
                                    EditLengths.Add(bofield.MaxLength);
                                    DropDownValues.Add(new List<VMDropDown>());
                                    editfieldforquery = editfieldforquery + NonEdit + ", ";
                                    IsDropDown.Add("False");
                                }
                                else
                                {
                                    BOFields bofield = new BOFields();
                                    string FieldName = "";
                                    var fields = MappedFields.Where(m => m.AddField == field).FirstOrDefault();
                                    if (fields == null)
                                    {
                                        var AName = BFields.Where(m => m.Name == field && m.BOID == query.BOID).Select(m => m.LabelName).FirstOrDefault();
                                        FieldName = field;
                                        bofield = BFields.Where(m => m.BOID == query.BOID).Where(m => m.Name == FieldName).FirstOrDefault();
                                        EditFields.Add(AName);
                                        EditDescs.Add(bofield.Description);
                                    }
                                    else
                                    {
                                        FieldName = fields.FieldName;
                                        EditFields.Add(FieldName);
                                        EditDescs.Add(fields.Description);
                                        bofield = BFields.Where(m => m.BOID == query.BOID).Where(m => m.Name == fields.AddField).FirstOrDefault();
                                    }
                                    editfieldforquery = editfieldforquery + field + ", ";
                                    string type = ((BODatatypes)bofield.TypeID).ToString();
                                    Editdatatypes.Add(type);
                                    EditLengths.Add(bofield.MaxLength);
                                    if (bofield.FKTableName != null && bofield.FKTableName.Length > 0)
                                    {
                                        IsDropDown.Add("True");
                                        using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                                        {
                                            Con.Open();
                                            SqlCommand cmd = new SqlCommand("", Con);
                                            if (bofield.FKTableName == "AspNetUsers")
                                            {
                                                int RoleID = 0;
                                                if (bofield.Name == "iSalesUserID")
                                                {
                                                    RoleID = dbcontext.XIAppRoles.Where(m => m.sRoleName == "Sales").Select(m => m.RoleID).FirstOrDefault();
                                                }
                                                else
                                                {
                                                    RoleID = dbcontext.XIAppRoles.Where(m => m.sRoleName == "Outbound").Select(m => m.RoleID).FirstOrDefault();
                                                }
                                                var Users = dbcontext.XIAppUserRoles.Where(m => m.RoleID == RoleID).Select(m => m.UserID).ToList();
                                                var AllUsers = dbcontext.XIAppUsers.ToList();
                                                if (Users.Count() > 0)
                                                {
                                                    var DDLVlaues = new List<VMDropDown>();
                                                    foreach (var user in Users)
                                                    {
                                                        DDLVlaues.Add((from c in AllUsers.Where(m => m.UserID == user).ToList()
                                                                       select new VMDropDown { text = c.sFirstName, Value = c.UserID }).FirstOrDefault());
                                                    }
                                                    DropDownValues.Add(DDLVlaues);
                                                }
                                                else
                                                {
                                                    DropDownValues.Add(new List<VMDropDown>());
                                                }
                                            }
                                            else
                                            {
                                                if (bofield.FKTableName == EnumLeadTables.OrganizationClasses.ToString())
                                                {
                                                    cmd.CommandText = "Select ClassID, Class FROM " + bofield.FKTableName + " WHERE OrganizationID=" + orgid;
                                                }
                                                else
                                                {
                                                    cmd.CommandText = "Select ID, Name FROM " + bofield.FKTableName + " WHERE OrganizationID=" + orgid;
                                                }
                                                Con.ChangeDatabase(database);
                                                SqlDataReader reader = cmd.ExecuteReader();
                                                DataTable Ddata = new DataTable();
                                                Ddata.Load(reader);
                                                List<object[]> TotalResult = Ddata.AsEnumerable().Select(m => m.ItemArray).ToList();
                                                var DDLVlaues = (from c in TotalResult
                                                                 select new VMDropDown { Value = Convert.ToInt32(c[0]), text = c[1].ToString() }).ToList();
                                                DropDownValues.Add(DDLVlaues);
                                            }
                                        }
                                    }
                                    else if (fields != null)
                                    {
                                        if (fields.IsDropDown)
                                        {
                                            IsDropDown.Add("True");
                                            var DDLVlaues = new List<VMDropDown>();
                                            var Type = dbcontext.Types.Where(m => m.ID == fields.MasterID).Select(m => m.Expression).FirstOrDefault();
                                            DDLVlaues = (from c in dbcontext.Types.Where(m => m.Name == Type).ToList() select new VMDropDown { Value = c.ID, text = c.Expression }).ToList();
                                            DropDownValues.Add(DDLVlaues);
                                        }
                                        else
                                        {
                                            IsDropDown.Add("False");
                                            DropDownValues.Add(new List<VMDropDown>());
                                        }
                                    }
                                    else
                                    {
                                        IsDropDown.Add("False");
                                        DropDownValues.Add(new List<VMDropDown>());
                                    }
                                }
                            }
                        }
                    }
                    if (vfields != null && vfields.Length > 0)
                    {
                        viewfieldforquery = viewfieldforquery.Substring(0, viewfieldforquery.Length - 2);
                        var ViewQuery = "Select " + viewfieldforquery + " FROM " + tablename + " WHERE " + " ID= " + leadid;
                        var valueoflead = new List<string>();
                        using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                        {
                            Con.Open();
                            Con.ChangeDatabase(database);
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = Con;
                            cmd.CommandText = ViewQuery;
                            SqlDataReader reader = cmd.ExecuteReader();
                            DataTable Viewdata = new DataTable();
                            Viewdata.Load(reader);
                            List<object[]> TotalResult = Viewdata.AsEnumerable().Select(m => m.ItemArray).ToList();
                            valueoflead = (from c in TotalResult[0]
                                           select c.ToString()).ToList();
                            data.ViewFieldsData = valueoflead;
                            Con.Close();
                        }
                        for (int h = 0; h < ViewDropDowns.Count(); h++)
                        {
                            if (ViewDropDowns[h] == "True")
                            {
                                if (valueoflead[h] != null && valueoflead[h].Length > 0)
                                {
                                    string Masters = dbcontext.Database.SqlQuery<string>("Select Expression from " + ViewDDLTable[h] + " where id=" + valueoflead[h]).FirstOrDefault();
                                    valueoflead[h] = Masters;
                                }
                            }
                        }
                    }
                    else
                    {
                        data.ViewFieldsData = new List<string>();
                    }
                    if (efields != null)
                    {
                        editfieldforquery = editfieldforquery.Substring(0, editfieldforquery.Length - 2);
                        var EditQuery = "Select " + editfieldforquery + " FROM " + tablename + " WHERE " + " ID= " + leadid;
                        using (SqlConnection Conn = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                        {
                            Conn.Open();
                            Conn.ChangeDatabase(database);
                            SqlCommand cmmd = new SqlCommand();
                            cmmd.Connection = Conn;
                            cmmd.CommandText = EditQuery;
                            SqlDataReader Editreader = cmmd.ExecuteReader();
                            DataTable Editdata = new DataTable();
                            Editdata.Load(Editreader);
                            List<object[]> TotalResult = Editdata.AsEnumerable().Select(m => m.ItemArray).ToList();
                            var editvalueoflead = (from c in TotalResult[0]
                                                   select c.ToString()).ToList();
                            data.EditFields = EditFields;
                            data.EditFieldsData = editvalueoflead;
                            data.EditDataTypes = Editdatatypes;
                            data.EditLengths = EditLengths;
                            data.EditDescs = EditDescs;
                            data.DropDownValues = DropDownValues;
                            data.IsDropDown = IsDropDown;
                            Conn.Close();
                        }
                    }
                    else
                    {
                        data.EditFieldsData = new List<string>();
                        data.EditDataTypes = new List<string>();
                        data.EditLengths = new List<string>();
                    }

                    List<VMDropDown> StatusDD = new List<VMDropDown>();
                    StatusDD = (from c in dbcontext.Stages.Where(m => m.OrganizationID == 0 || m.OrganizationID == orgid).ToList()
                                select new VMDropDown { Value = c.ID, text = c.Name }).ToList();
                    data.StatusDDL = StatusDD;
                    Data.Add(data);
                    vmquery.SectionsData = Data;
                }
            }
            List<SingleBOField> bofields = new List<SingleBOField>();
            vmquery.SingleBOField = bofields;
            vmquery.Query = query.Query;
            vmquery.QueryID = tab1click.ReportID;
            return vmquery;
        }

        public VMQueryPreview EditData(List<FormData> FormValues, VMViewPopup Popup, string database, string sOrgName)
        {
            VMQueryPreview result = new VMQueryPreview();
            var ErrorMessage = "";
            var ReportID = dbcontext.Tab1Clicks.Where(m => m.ID == Popup.Tab1ClickID).Select(m => m.FKi1ClickIDXIGUID).FirstOrDefault();
            DataContext Spdb = new DataContext(database);
            var MappedFields = Spdb.MappedFields.Where(m => m.OrganizationID == Popup.OrganizationID).ToList();
            int Boid = Popup.BOID;
            string tablename = "";
            if (Popup.BOID != 0)
            {
                string BOName = dbcontext.BOs.Where(m => m.BOID == Popup.BOID).Select(m => m.Name).FirstOrDefault();
                tablename = BOName;
            }
            else
            {
                Boid = dbcontext.Reports.Where(m => m.XIGUID == ReportID).Select(m => m.BOID).FirstOrDefault();
                var boname = dbcontext.BOs.Where(m => m.BOID == Boid).Select(m => m.Name).FirstOrDefault();
                tablename = boname;
            }
            if (tablename == EnumLeadTables.Leads.ToString())
            {
                FormValues.Add(new FormData { Label = "XIUpdatedWhen", Value = DateTime.Now.ToString() });
            }
            var BoFields = dbcontext.BOFields.Where(m => m.BOID == Boid).ToList();
            if (Popup.FormType == "CallBack")
            {
                try
                {
                    var Date = Convert.ToDateTime(FormValues[0].Value);
                    var time = FormValues[1].Value.ToString();
                    DateTime SchTime = Date.Add(TimeSpan.Parse(time));
                    Spdb.Database.ExecuteSqlCommand("update " + tablename + " set iCallCount = iCallCount +1, iCallbackStatus = 10, dtCallSchedule='" + SchTime + "' where ID=" + Popup.LeadID + "");
                }
                catch (Exception ex)
                {
                    ErrorMessage = "Error";
                }
            }
            else if (Popup.FormType == "ManagerOverride")
            {
                var Value = Convert.ToInt32(FormValues[0].Value);
                Spdb.Database.ExecuteSqlCommand("update " + tablename + " set iStatus='" + Value + "' where ID=" + Popup.LeadID + "");
                var lead = Spdb.LeadTransitions.Where(m => m.LeadID == Popup.LeadID).ToList();
                var AllStages = Spdb.Stages.Where(m => m.OrganizationID == Popup.OrganizationID).ToList();
                LeadTransitions Trans = new LeadTransitions();
                Trans.StageID = Value;
                Trans.LeadID = Popup.LeadID;
                Trans.OrganizationID = Popup.OrganizationID;
                if (lead.Count() == 0)
                {
                    Trans.FromStatus = "Inbound";
                }
                else
                {
                    Trans.FromStatus = lead.Last().ToStatus;
                }
                Trans.ToStatus = AllStages.Where(m => m.ID == Value).Select(m => m.Name).FirstOrDefault();
                Spdb.LeadTransitions.Add(Trans);
                Spdb.SaveChanges();
                LeadStatus Sta = new LeadStatus();
                Sta.Status = AllStages.Where(m => m.ID == Value).Select(m => m.Name).FirstOrDefault();
                Sta.LeadID = Popup.LeadID;
                Sta.UserName = dbcontext.XIAppUsers.Where(m => m.UserID == Popup.UserID).Select(m => m.sFirstName).FirstOrDefault();
                Spdb.LeadStatus.Add(Sta);
                Spdb.SaveChanges();
            }
            else
            {
                foreach (var item in FormValues)
                {
                    if (item.Value != null)
                    {
                        var label = item.Label.Split('-')[0];
                        if (label != "ID")
                        {
                            string FieldName = "";
                            FieldName = MappedFields.Where(m => m.FieldName == label).Select(m => m.AddField).FirstOrDefault();
                            if (FieldName == null)
                            {
                                FieldName = BoFields.Where(m => m.LabelName == label).Select(m => m.Name).FirstOrDefault();
                                if (FieldName == null)
                                {
                                    FieldName = label;
                                }
                            }
                            Spdb.Database.ExecuteSqlCommand("UPDATE " + tablename + " SET" + " " + FieldName + "=" + "'" + item.Value + "'" + " " + "WHERE" + " ID=" + Popup.LeadID + "");
                        }
                    }
                }
            }
            if (Popup.PopType == "Navigate")
            {
                Stages Stage = new Stages();
                Stage.ID = Popup.StageID;
                Stage.LeadID = Popup.LeadID;
                SaveLeadTransaction(Stage, Popup.OrganizationID, Popup.Database, Popup.UserID);
            }
            Common Com = new Common();
            result = QueryDynamicForm(Popup.Tab1ClickID, Popup.LeadID, database, Popup.OrganizationID);
            result.Message = ErrorMessage;
            var Headings = Com.GetHeadings(ReportID, database, Popup.OrganizationID, Popup.UserID, sOrgName);
            var report = dbcontext.Tab1Clicks.Where(m => m.ID == Popup.Tab1ClickID).FirstOrDefault();
            //var Grid = RunUserQuery(report.ReportID, UserID, database, OrgID, Popup.LeadID);
            result.Headings = Headings.Headings;
            //result.Rows = Grid.Rows;
            int sectionid = Convert.ToInt32(report.SectionID);
            result.SectionName = dbcontext.Sections.Where(m => m.ID == sectionid).Select(m => m.Name).FirstOrDefault();
            result.TabID = report.TabID;
            result.TabName = dbcontext.Tabs.Where(m => m.ID == report.TabID).Select(m => m.Name).FirstOrDefault();
            result.Tab1ClickID = Popup.Tab1ClickID;
            result.XIGUID = ReportID;
            result.BOID = Popup.BOID;
            result.IsView = report.IsView;
            result.IsEdit = report.IsEdit;
            result.IsCreate = report.IsCreate;
            result.IsMouseOverColumn = Headings.IsMouseOverColumn;
            result.MouserOverColum = Headings.MouserOverColum;
            //result.MouseOverHeadings = Grid.MouseOverHeadings;
            //result.MouseOverValues = Grid.MouseOverValues;
            return result;
        }

        public VMQueryPreview RunUserQuery(int QueryID, int UserID, string database, int OrganizationID, int LeadID, string sOrgName)
        {
            VMQueryPreview vmquery = new VMQueryPreview();
            DataContext Spdb = new DataContext(database);
            Reports query = dbcontext.Reports.Find(QueryID);
            if (query != null)
            {
                int BOID = query.BOID;
                List<VMDropDown> KeyPositions = new List<VMDropDown>();
                var FromIndex = query.Query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
                var SelectQuery = query.Query.Substring(0, FromIndex);
                Common Com = new Common();
                var Keys = ServiceUtil.GetForeginkeyValues(SelectQuery);
                List<string> AllHeadings = new List<string>();
                List<string> Headings = query.SelectFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var str1 = "";
                if (Headings.Contains("ID") == false)
                {
                    str1 = "No";
                    Headings.Insert(0, "ID");
                    vmquery.IDExists = false;
                }
                else
                {
                    vmquery.IDExists = true;
                }
                string allfields = "";
                if (str1 == "No")
                {
                    var allfields1 = (query.Query).Insert(7, "ID, ");
                    allfields = (query.Query).Insert(7, "ID, ");
                }
                else
                    allfields = query.Query;
                int FKPosition = 0;
                List<string> IsMouseOver = new List<string>();
                List<string> MouseOverColumns = new List<string>();
                foreach (var items in Headings)
                {
                    if (items.Contains('{'))
                    {
                        string id = items.Substring(1, items.Length - 2);
                        int gid = Convert.ToInt32(id);
                        string groupid = Convert.ToString(gid);
                        BOGroupFields fields = dbcontext.BOGroupFields.Find(gid);
                        allfields = allfields.Replace("{" + groupid + "}", fields.BOSqlFieldNames);
                        if (fields.IsMultiColumnGroup == true)
                        {
                            List<string> fieldnames = fields.BOFieldNames.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            foreach (var names in fieldnames)
                            {
                                string aliasname = dbcontext.BOFields.Where(m => m.Name == names && m.BOID == BOID).Select(m => m.LabelName).FirstOrDefault();
                                //var mouseover = Spdb.MouseOverColumns.Where(m => m.FieldName == BOID).FirstOrDefault();

                                if (aliasname != null)
                                {
                                    AllHeadings.Add(aliasname);
                                    //if (mouseover != null)
                                    //{
                                    //    IsMouseOver.Add("True");
                                    //}
                                    //else
                                    //{
                                    //    IsMouseOver.Add("False");
                                    //}
                                }

                            }
                        }
                        else
                        {
                            AllHeadings.Add(fields.GroupName);
                        }
                    }
                    else
                    {
                        BOFields BOField = dbcontext.BOFields.Where(m => m.Name == items && m.BOID == BOID).FirstOrDefault();
                        //var mouseover = Spdb.MouseOverColumns.Where(m => m.FieldName == BOField.ID).FirstOrDefault();
                        foreach (var fks in Keys)
                        {
                            if (fks.text == items)
                            {
                                KeyPositions.Add(new VMDropDown
                                {
                                    text = items,
                                    Value = FKPosition
                                });
                            }
                        }
                        if (BOField != null)
                        {
                            AllHeadings.Add(BOField.LabelName);
                            //if (mouseover != null)
                            //{
                            //    IsMouseOver.Add("True");
                            //    var moverfileds = dbcontext.BOGroupFields.Where(m => m.ID == mouseover.FieldGroup).Select(m => m.BOFieldNames).FirstOrDefault();
                            //    MouseOverColumns.Add(moverfileds);
                            //}
                            //else
                            //{
                            //    IsMouseOver.Add("False");
                            //    MouseOverColumns.Add(null);
                            //}
                        }
                    }
                    FKPosition++;
                }
                int RoleID = dbcontext.XIAppUserRoles.Where(m => m.UserID == UserID).Select(m => m.RoleID).FirstOrDefault();
                string UserIDs = Com.GetSubUsers(UserID, OrganizationID, database, sOrgName);
                string Query = ServiceUtil.ReplaceQueryContent(allfields, UserIDs, UserID, OrganizationID, LeadID, 0);
                List<string> MouseOverHeadings = new List<string>();
                List<List<string>> AllMouseOverValues = new List<List<string>>();
                List<string[]> results = new List<string[]>();
                List<string> MouserOverAliasNames = new List<string>();
                using (SqlConnection Conn = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    SqlCommand cmmd = new SqlCommand();
                    Conn.Open();
                    Conn.ChangeDatabase(database);
                    cmmd.Connection = Conn;
                    using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                    {
                        SqlCommand cmd = new SqlCommand();
                        Con.Open();
                        Con.ChangeDatabase(database);
                        cmd.Connection = Con;
                        cmd.CommandText = Query;
                        SqlDataReader reader = cmd.ExecuteReader();
                        int count = reader.FieldCount;
                        string[] rows = new string[count];
                        while (reader.Read())
                        {
                            List<string> MouseOverValues = new List<string>();
                            List<string> values = new List<string>();
                            for (int i = 0; i < count; i++)
                            {
                                var DbValue = reader.IsDBNull(i) ? null : reader.GetValue(i).ToString();
                                values.Add(reader.IsDBNull(i) ? null : reader.GetValue(i).ToString());
                                var MOFields = MouseOverColumns[i];
                                if (DbValue != null && MouseOverColumns[i] != null)
                                {
                                    string NewSelectPart = "Select " + MouseOverColumns[i] + " ";
                                    var NewBOID = dbcontext.BOGroupFields.Where(m => m.BOFieldNames == MOFields).Select(m => m.BOID).FirstOrDefault();
                                    var BoName = dbcontext.BOs.Where(m => m.BOID == NewBOID).Select(m => m.Name).FirstOrDefault();
                                    string From = " From " + BoName;
                                    string Where = "";
                                    if (BoName == EnumLeadTables.OrganizationClasses.ToString())
                                    {
                                        Where = " Where ClassID=" + DbValue;
                                    }
                                    else
                                    {
                                        Where = " Where ID=" + DbValue;
                                    }

                                    string NewQuery = NewSelectPart + From + Where;
                                    cmmd.CommandText = NewQuery;
                                    SqlDataReader reader1 = cmmd.ExecuteReader();
                                    int Fcount = reader1.FieldCount;

                                    while (reader1.Read())
                                    {
                                        string Values = "";
                                        for (int j = 0; j < Fcount; j++)
                                        {
                                            Values = Values + reader1.GetValue(j).ToString() + ",";
                                        }
                                        if (Values.Length > 0)
                                        {
                                            Values = Values.Substring(0, Values.Length - 1);
                                        }
                                        MouseOverValues.Add(Values);
                                    }
                                    reader1.Close();
                                }
                                else
                                {
                                    MouseOverValues.Add(null);
                                }
                            }
                            AllMouseOverValues.Add(MouseOverValues);
                            string[] result = values.ToArray();
                            results.Add(result);
                        }
                        reader.Close();
                        Conn.Close();
                        Con.Close();

                        if (query.ActionFields != null)
                        {
                            vmquery.IsPopup = true;
                        }
                        else
                        {
                            vmquery.IsPopup = false;
                        }
                        for (int i = 0; i < MouseOverColumns.Count(); i++)
                        {
                            if (MouseOverColumns[i] != null)
                            {
                                MouseOverHeadings.Add(MouseOverColumns[i]);
                                if (MouseOverColumns[i].Length > 0)
                                {
                                    string Name = "", AliasName = "";
                                    var cols = MouseOverColumns[i].Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                    foreach (var item in cols)
                                    {
                                        Name = Spdb.MappedFields.Where(m => m.ClassID == query.Class && m.AddField == item).Select(m => m.FieldName).FirstOrDefault();
                                        if (Name == null)
                                        {
                                            Name = dbcontext.BOFields.Where(m => m.BOID == BOID && m.Name == item).Select(m => m.LabelName).FirstOrDefault();
                                        }
                                        if (Name == null)
                                        {
                                            AliasName = AliasName + item + ",";
                                        }
                                        else
                                        {
                                            AliasName = AliasName + Name + ",";
                                        }

                                    }
                                    AliasName = AliasName.Substring(0, AliasName.Length - 1);
                                    MouserOverAliasNames.Add(AliasName);
                                }
                            }
                            else
                            {
                                MouseOverHeadings.Add("");
                                MouserOverAliasNames.Add("");
                            }
                        }
                        Con.Close();
                    }
                }
                string ColumnHeadings = "ID, " + query.SelectFields;
                vmquery.Headings = AllHeadings;
                vmquery.Rows = results;//.Skip(0).Take(10).ToList();
                vmquery.QueryName = query.Name;
                vmquery.QueryID = QueryID;
                vmquery.ColumnNames = ColumnHeadings;//.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                vmquery.QueryIcon = dbcontext.UserReports.Where(m => m.ReportID == QueryID).Where(m => m.RoleID == RoleID).Select(m => m.Icon).FirstOrDefault();
                vmquery.ClassID = dbcontext.Reports.Where(m => m.ID == QueryID).Select(m => m.Class).FirstOrDefault();
                vmquery.QueryName = query.Name;
                vmquery.IsMouseOverColumn = IsMouseOver;
                vmquery.MouseOverValues = AllMouseOverValues;
                vmquery.MouseOverHeadings = MouserOverAliasNames;
            }
            else
            {
                vmquery.Headings = new List<string>();
                vmquery.Rows = new List<string[]>();
                //vmquery.ColumnNames = new List<string>();
            }
            return vmquery;
        }
        public VMQueryPreview GetEditRowDetails(int LeadID, int ClickID, int OrgID, string database)
        {
            var tab1click = dbcontext.Tab1Clicks.Find(ClickID);
            var result = QueryDynamicForm(ClickID, LeadID, database, OrgID);
            return result;
        }
        public List<SectionsData> GetCreateRowDetails(int Tab1ClickID, int BOID, int OrgID, string database)
        {
            List<SectionsData> Create = new List<SectionsData>();
            SectionsData CreateData = new SectionsData();
            List<string> CreateFields = new List<string>();
            List<string> CreateDataTypes = new List<string>();
            List<string> CreateLengths = new List<string>();
            List<string> CreateDrpts = new List<string>();
            List<List<VMDropDown>> DropDownValues = new List<List<VMDropDown>>();
            var tab1click = dbcontext.Tab1Clicks.Find(Tab1ClickID);
            var Bo = dbcontext.BOs.Find(BOID);
            var BoFields = Bo.BOFields;
            if (tab1click.CreateFields != null && tab1click.CreateFields.Length > 0)
            {
                var CFields = tab1click.CreateFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var items in CFields)
                {
                    if (items.Contains("NE-"))
                    {
                        CreateFields.Add(items);
                        CreateDataTypes.Add("");
                        CreateLengths.Add("");
                        DropDownValues.Add(new List<VMDropDown>());
                        CreateDrpts.Add("");
                    }
                    else
                    {
                        var Field = BoFields.Where(m => m.Name == items).FirstOrDefault();
                        CreateFields.Add(Field.LabelName);
                        string type = ((BODatatypes)Field.TypeID).ToString();
                        CreateDataTypes.Add(type);
                        CreateLengths.Add(Field.MaxLength);
                        CreateDrpts.Add(Field.Description);
                        if (Field.FKTableName != null && Field.FKTableName.Length > 0)
                        {
                            using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                            {
                                Con.Open();
                                SqlCommand cmd = new SqlCommand("", Con);
                                if (Field.FKTableName == EnumLeadTables.OrganizationClasses.ToString())
                                {
                                    cmd.CommandText = "Select ClassID, Class FROM " + Field.FKTableName + " WHERE OrganizationID=" + OrgID;
                                }
                                else
                                {
                                    cmd.CommandText = "Select ID, Name FROM " + Field.FKTableName + " WHERE OrganizationID=" + OrgID;
                                }
                                Con.ChangeDatabase(database);
                                SqlDataReader reader = cmd.ExecuteReader();
                                DataTable data = new DataTable();
                                data.Load(reader);
                                List<object[]> TotalResult = data.AsEnumerable().Select(m => m.ItemArray).ToList();
                                var DDLVlaues = (from c in TotalResult
                                                 select new VMDropDown { Value = Convert.ToInt32(c[0]), text = c[1].ToString() }).ToList();
                                DropDownValues.Add(DDLVlaues);
                            }
                        }
                        else
                        {
                            DropDownValues.Add(new List<VMDropDown>());
                        }
                    }
                }
            }
            CreateData.CreateFields = CreateFields;
            CreateData.CreateLengths = CreateLengths;
            CreateData.CreateDataTypes = CreateDataTypes;
            CreateData.DropDownValues = DropDownValues;
            CreateData.CreateDrpts = CreateDrpts;
            int SecID = Convert.ToInt32(tab1click.SectionID);
            CreateData.SectionName = dbcontext.Sections.Where(m => m.ID == SecID).Select(m => m.Name).FirstOrDefault();
            CreateData.TabID = tab1click.TabID;
            Create.Add(CreateData);
            //var result = QueryDynamicForm(Tab1ClickID, 0, database, OrgID);
            return Create;
        }
        public VMQueryPreview CreateRowFromGrid(List<FormData> FormValues, VMViewPopup popup, string database, string sOrgName)
        {
            var ReportID = dbcontext.Tab1Clicks.Where(m => m.ID == popup.Tab1ClickID).Select(m => m.FKi1ClickIDXIGUID).FirstOrDefault();
            //string database = dbcontext.AspNetUsers.Where(m => m.OrganizationID == OrgID).Select(m => m.DatabaseName).FirstOrDefault();
            int Boid = popup.BOID;
            DataContext Spdb = new DataContext(database);
            var MappedFields = Spdb.MappedFields.Where(m => m.OrganizationID == popup.OrganizationID).ToList();
            var Bofields = dbcontext.BOFields.Where(m => m.BOID == Boid).ToList();
            string BOName = dbcontext.BOs.Where(m => m.BOID == popup.BOID).Select(m => m.Name).FirstOrDefault();
            string tablename = BOName;
            string Columns = "", Values = "";
            if (BOName != EnumLeadTables.LeadInstances.ToString() && BOName != EnumLeadTables.Leads.ToString())
            {
                FormValues.Add(new FormData { Value = popup.OrganizationID.ToString(), Label = "OrganizationID" });
            }
            else if (BOName == EnumLeadTables.LeadInstances.ToString())
            {
                FormValues.Add(new FormData { Value = popup.LeadID.ToString(), Label = "LeadID" });
            }

            if (BOName == EnumLeadTables.WalletQuotes.ToString())
            {
                FormValues.Add(new FormData { Value = popup.LeadID.ToString(), Label = "LeadID" });
                FormValues.Add(new FormData { Value = "False", Label = "IsPosted" });
            }
            foreach (var item in FormValues)
            {
                var label = item.Label;
                if (label.Contains("NE-"))
                {
                    label = label.Replace("NE-", "");
                }
                var name = label.Split('-')[0];
                if (name != "ID")
                {
                    var ColName = MappedFields.Where(m => m.FieldName == name).Select(m => m.AddField).FirstOrDefault();
                    if (ColName == null)
                    {
                        ColName = Bofields.Where(m => m.LabelName == name).Select(m => m.Name).FirstOrDefault();
                    }
                    Columns = Columns + ColName + ", ";
                    Values = Values + "'" + item.Value + "', ";
                }

            }
            Columns = Columns.Substring(0, Columns.Length - 2);
            Values = Values.Substring(0, Values.Length - 2);
            Spdb.Database.ExecuteSqlCommand("INSERT INTO " + tablename + " (" + Columns + ") VALUES (" + Values + ")");
            Common Com = new Common();
            var result = Com.GetHeadings(ReportID, database, popup.OrganizationID, popup.UserID, sOrgName);
            var report = dbcontext.Tab1Clicks.Where(m => m.ID == popup.Tab1ClickID).FirstOrDefault();
            int sectionid = Convert.ToInt32(report.SectionID);
            result.SectionName = dbcontext.Sections.Where(m => m.ID == sectionid).Select(m => m.Name).FirstOrDefault();
            result.TabID = report.TabID;
            result.Tab1ClickID = popup.Tab1ClickID;
            result.BOID = popup.BOID;
            result.IsView = report.IsView;
            result.IsCreate = report.IsCreate;
            result.IsEdit = report.IsEdit;
            result.XIGUID = ReportID;
            return result;
        }
        //public ViewDetails TableRowValues(int ID, int BOID, int LeadID, string database)
        //{
        //    var tab = dbcontext.Tab1Clicks.Where(m => m.ID == ID).Select(m => m.ViewFields).SingleOrDefault();
        //    tab = tab.Replace("NE-", "");
        //    var bos = dbcontext.BOs.Where(m => m.BOID == BOID).Select(m => m.Name).SingleOrDefault();
        //    string str = "select " + tab + " from " + bos + " where ID=" + LeadID;
        //    SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString());
        //    Con.Open();
        //    Con.ChangeDatabase(database);
        //    SqlCommand cmd = new SqlCommand();
        //    cmd.Connection = Con;
        //    cmd.CommandText = str;
        //    SqlDataReader reader = cmd.ExecuteReader();
        //    List<string> results = new List<string>();
        //    int count = reader.FieldCount;
        //    string[] rows = new string[count];

        //    while (reader.Read())
        //    {
        //        List<string> values = new List<string>();
        //        for (int i = 0; i < count; i++)
        //        {
        //            results.Add(reader.GetValue(i).ToString());
        //        }
        //        //string[] result = values.ToArray();
        //        //results.Add(values);
        //    }
        //    Con.Close();
        //    ViewDetails viewdetails = new ViewDetails();
        //    viewdetails.results = results;
        //    var value = new List<string>();
        //    var details = tab.Split(',').ToList();
        //    foreach (var item in details)
        //    {
        //        value.Add(item);
        //    }
        //    viewdetails.headings = value;
        //    return viewdetails;

        //}

        public ViewDetails TableRowValues(string ID, int BOID, string BOName, string ColumnName, string database, int OrgID)
        {
            DataContext Spdb = new DataContext(database);
            BOs BO = new BOs();
            if (BOName == null || BOName == "")
            {
                BO = dbcontext.BOs.Find(BOID);
                BOName = BO.Name;
            }
            else
            {
                BO = dbcontext.BOs.Where(m => m.Name == BOName).FirstOrDefault();
            }
            var BOGrpFields = BO.BOGroups.Where(m => m.BOID == BO.BOID).ToList();
            string GroupFields = "";
            if (ColumnName == "ID")
            {
                GroupFields = BOGrpFields.Where(m => m.GroupName == "Summary Fields").Select(m => m.BOFieldNames).FirstOrDefault();
            }
            else
            {
                GroupFields = BOGrpFields.Where(m => m.GroupName == "MouseOver Group").Select(m => m.BOFieldNames).FirstOrDefault();
            }
            if (GroupFields != null)
            {
                string str = "";
                if (BOName == EnumLeadTables.OrganizationClasses.ToString())
                {
                    str = "select " + GroupFields + " from " + BOName + " where Class='" + ID + "' and OrganizationID=" + OrgID;
                }
                else if (BOName == EnumLeadTables.Leads.ToString())
                {
                    str = "select " + GroupFields + " from " + BOName + " where ID='" + ID + "' and FKiOrgID=" + OrgID;
                }
                else
                {
                    str = "select " + GroupFields + " from " + BOName + " where Name='" + ID + "' and OrganizationID=" + OrgID;
                }
                List<object[]> TotalResult = new List<object[]>();
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    Con.Open();
                    Con.ChangeDatabase(database);
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    cmd.CommandText = str;
                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable data = new DataTable();
                    data.Load(reader);
                    TotalResult = data.AsEnumerable().Select(m => m.ItemArray).ToList();
                    Con.Close();
                }
                ViewDetails viewdetails = new ViewDetails();
                if (TotalResult.Count() > 0)
                {
                    viewdetails.results = TotalResult[0];
                }
                var value = new List<string>();
                var details = GroupFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var BoFields = dbcontext.BOFields.Where(m => m.BOID == BO.BOID).ToList();
                foreach (var item in details)
                {
                    var Head = Spdb.MappedFields.Where(m => m.AddField == item).Select(m => m.FieldName).FirstOrDefault();
                    if (Head == null)
                    {
                        Head = BoFields.Where(m => m.Name == item).Select(m => m.LabelName).FirstOrDefault();
                    }
                    if (Head == null)
                    {
                        value.Add(item);
                    }
                    else
                    {
                        value.Add(Head);
                    }
                }
                viewdetails.headings = value;
                return viewdetails;
            }
            else
            {
                object[] ob = new object[0];
                ViewDetails viewdetails = new ViewDetails();
                viewdetails.headings = new List<string>();
                viewdetails.results = ob;
                return viewdetails;
            }

        }
        public VMQueryPreview DeleteRowDetails(int LeadID, int Tab1ClickID, int BOID, int OrgID, int UserID, string database, string sOrgName)
        {
            var ReportID = dbcontext.Tab1Clicks.Where(m => m.ID == Tab1ClickID).Select(m => m.FKi1ClickIDXIGUID).FirstOrDefault();
            string BOName = dbcontext.BOs.Where(m => m.BOID == BOID).Select(m => m.Name).FirstOrDefault();
            string tablename = BOName;
            using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                cmd.CommandText = "DELETE FROM " + tablename + " WHERE ID=" + LeadID;
                Con.Open();
                Con.ChangeDatabase(database);
                cmd.ExecuteNonQuery();
                Con.Dispose();
            }
            Common Com = new Common();
            var result = Com.GetHeadings(ReportID, database, OrgID, UserID, sOrgName);
            var report = dbcontext.Tab1Clicks.Where(m => m.ID == Tab1ClickID).FirstOrDefault();
            //var Grid = RunUserQuery(report.ReportID, UserID, database, OrgID, LeadID);
            //result.Headings = Grid.Headings;
            //result.Rows = Grid.Rows;
            int sectionid = Convert.ToInt32(report.SectionID);
            result.SectionName = dbcontext.Sections.Where(m => m.ID == sectionid).Select(m => m.Name).FirstOrDefault();
            result.TabID = report.TabID;
            result.Tab1ClickID = Tab1ClickID;
            result.BOID = BOID;
            result.IsView = report.IsView;
            result.IsCreate = report.IsCreate;
            result.IsEdit = report.IsEdit;
            //result.IsMouseOverColumn = Grid.IsMouseOverColumn;
            //result.MouseOverValues = Grid.MouseOverValues;
            //result.MouseOverHeadings = Grid.MouseOverHeadings;
            result.XIGUID = ReportID;
            return result;
        }

        #endregion TabResultList

        #region Documents
        public int SaveOrganizationDocument(WalletDocuments Document, int LeadID)
        {
            List<string> Email = GetLeadEmailID(LeadID, Document.OrganizationID);
            string EmailID = Email[0];
            string ClientID = dbcontext.WalletRequests.Where(m => m.EmailID == EmailID).Select(m => m.ClientID).FirstOrDefault();
            var Org = dbcontext.Organization.Find(Document.OrganizationID);
            DataContext Spdb = new DataContext(Org.DatabaseName);
            if (Document.ID == 0)
            {
                WalletDocuments Doc = new WalletDocuments();
                Doc.ClientID = ClientID;
                Doc.OriginalName = Document.OriginalName;
                Doc.UploadedOn = DateTime.Now;
                Doc.OrganizationID = Document.OrganizationID;
                Doc.Message = Document.Message;
                Doc.StatusTypeID = 10;
                Doc.Type = 20;
                Spdb.WalletDocuments.Add(Doc);
                Spdb.SaveChanges();
                return Doc.ID;
            }
            else
            {
                WalletDocuments Doc = Spdb.WalletDocuments.Find(Document.ID);
                Doc.DocumentName = Document.DocumentName;
                Spdb.SaveChanges();
                return Doc.ID;
            }
        }
        public DTResponse GetClientDocuments(jQueryDataTableParamModel param, int LeadID, int OrgID, string Type)
        {
            int DocType = 0;
            if (Type == "Sent")
            {
                DocType = 10;
            }
            else
            {
                DocType = 20;
            }
            var Email = GetLeadEmailID(LeadID, OrgID);
            string EmailID = Email[0];
            var ClientID = dbcontext.WalletRequests.Where(m => m.EmailID == EmailID).Select(m => m.ClientID).FirstOrDefault();
            var Org = dbcontext.Organization.Find(OrgID);
            DataContext Spdb = new DataContext(Org.DatabaseName);
            List<WalletDocuments> AllDocs = new List<WalletDocuments>();
            List<WalletDocuments> FilteredDocs = new List<WalletDocuments>();
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredDocs = Spdb.WalletDocuments.Where(m => m.ClientID == param.ClientID).Where(m => m.DocumentName.Contains(param.sSearch.ToUpper())).ToList();
                AllDocs.AddRange(FilteredDocs.OrderByDescending(m => m.ID).ToList());
                displyCount = displyCount + FilteredDocs.Count();
                AllDocs = AllDocs.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            }
            else
            {
                displyCount = displyCount + Spdb.WalletDocuments.Where(m => m.ClientID == ClientID && m.Type == DocType).Count();
                AllDocs.AddRange(Spdb.WalletDocuments.Where(m => m.ClientID == ClientID && m.Type == DocType).OrderByDescending(m => m.ID).ToList());
                AllDocs = AllDocs.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            }
            var result = (from cd in AllDocs
                          select new[] {
                             (i++).ToString(), Convert.ToString(cd.ID), cd.OrganizationID.ToString(), cd.DocumentName, cd.OriginalName, cd.Message, cd.UploadedOn.ToString("dd MMM yyyy"), cd.StatusTypeID.ToString(),""}).ToList();

            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        #endregion Documents

        #region HTML Attributes
        public DTResponse GetHTMLColorCodingsList(jQueryDataTableParamModel param, int OrgID, string database)
        {
            DataContext Spdb = new DataContext(database);
            IQueryable<HTMLColorCodings> AllCodings;
            AllCodings = Spdb.HTMLColorCodings;
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                //AllCodings = AllCodings.Where(m => m.Value.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllCodings.Count();
            AllCodings = QuerableUtil.GetResultsForDataTables(AllCodings, "", sortExpression, param);
            var clients = AllCodings.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.OrganizationID.ToString(), dbcontext.BOFields.Where(m=>m.ID==c.ColumnID).Select(m=>m.Name).FirstOrDefault(), dbcontext.Stages.Where(m=>m.ID==c.Value).Select(m=>m.Name).FirstOrDefault(), c.Result.ToString(), c.StatusTypeID.ToString(),""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public List<VMDropDown> GetAllBoFields()
        {
            var BO = dbcontext.BOs.Where(m => m.Name == EnumLeadTables.Leads.ToString()).FirstOrDefault();
            List<VMDropDown> AllFields = new List<VMDropDown>();
            AllFields = (from c in BO.BOFields.ToList() select new VMDropDown { Value = c.ID, text = c.Name }).ToList();
            return AllFields;
        }

        public List<VMDropDown> GetColumnValues(int ID, int OrgID)
        {
            List<VMDropDown> AllValues = new List<VMDropDown>();
            var FKTable = dbcontext.BOFields.Where(m => m.ID == ID).Select(m => m.FKTableName).FirstOrDefault();
            if (FKTable != null)
            {
                AllValues = (from c in dbcontext.Stages.Where(m => m.OrganizationID == 0 || m.OrganizationID == OrgID).ToList() select new VMDropDown { Value = c.ID, text = c.Name }).ToList();
            }
            return AllValues;
        }

        public int SaveHTMLColorCoding(HTMLColorCodings model, int OrgID, string Database)
        {
            DataContext Spdb = new DataContext(Database);
            HTMLColorCodings Code = new HTMLColorCodings();
            if (model.ID > 0)
            {
                Code = Spdb.HTMLColorCodings.Find(model.ID);
            }
            Code.ColumnID = model.ColumnID;
            Code.Value = model.Value;
            if (model.Type == 1)
            {
                Code.Result = model.Icon;
            }
            else
            {
                Code.Result = model.Result;
            }
            Code.Type = model.Type;
            Code.OrganizationID = OrgID;
            Code.StatusTypeID = model.StatusTypeID;
            if (model.ID == 0)
            {
                Spdb.HTMLColorCodings.Add(Code);
            }
            Spdb.SaveChanges();
            return Code.ID;
        }

        public HTMLColorCodings GetHTMLCodingByID(int ID, int OrgID, string Database)
        {
            DataContext Spdb = new DataContext(Database);
            HTMLColorCodings Code = new HTMLColorCodings();
            Code = Spdb.HTMLColorCodings.Find(ID);
            Code.Columns = GetAllBoFields();
            Code.Values = GetColumnValues(Code.ColumnID, OrgID);
            return Code;
        }

        #endregion HTML Attributes
        #region miscellaneous
        public List<IOServerDetails> ServerDetails(int Type, int OrgID)
        {
            var SDetails = dbcontext.IOServerDetails.Where(m => m.Type == Type).Where(m => m.OrganizationID == OrgID && m.Category == 1).ToList();
            return SDetails;
        }
        public DTResponse DisplayCompleteLeadDetails(jQueryDataTableParamModel param, int OrgID, string database)
        {
            var lSuccessDatas = new List<List<string>>();
            DataTable data = new DataTable();
            using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
            {
                SqlCommand cmd = new SqlCommand();
                Con.Open();
                Con.ChangeDatabase(database);
                cmd.Connection = Con;
                string FilteredDetails = "SELECT ID,[sName],[sForeName],[sLastName],[sMob],[sEmail],[dDOB],[sPostCode],[FKiLeadClassID] FROM [dbo].[Leads] WHERE FKiOrgID=" + OrgID + " ORDER BY ID asc";
                cmd.CommandText = FilteredDetails;
                SqlDataReader reader1 = cmd.ExecuteReader();
                data.Load(reader1);
            }
            var total = data.Rows.Count;
            List<object[]> TotalResult = data.AsEnumerable().Select(m => m.ItemArray).ToList();
            List<object[]> Res = new List<object[]>();
            Res = TotalResult.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            var displyCount = Res.Count();
            int i = 0;
            List<string[]> Results = new List<string[]>();
            foreach (var items in Res)
            {
                List<string> Record = new List<string>();
                Record.Add(items[0].ToString());
                Record.Add(items[1].ToString());
                Record.Add(items[2].ToString());
                Record.Add(items[3].ToString());
                Record.Add(items[4].ToString());
                Record.Add(items[5].ToString());
                Record.Add(items[6].ToString());
                Record.Add(items[7].ToString());
                Record.Add(items[8].ToString());
                Results.Add(Record.ToArray());
            }
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = Results
            };
        }

        public List<VMDropDown> GetOrgClassTypes(int OrgID, string database)
        {
            var OrgClassType = new List<VMDropDown>();
            DataContext Spdb = new DataContext(database);
            OrgClassType = ServiceUtil.GetOrgClasses(OrgID, "");
            return OrgClassType;
        }

        //Getting Templates to the dropdown
        public VMActionTypes GetAllTemplates(int OrgID, int Type, string database)
        {
            VMActionTypes model = new VMActionTypes();
            if (database == null)
            {
                database = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            }
            DataContext db = new DataContext(database);
            var EmailTemps = db.ContentEditors.Where(m => m.OrganizationID == OrgID && m.Category == 1).ToList();
            List<VMDropDown> Emailtemplates = new List<VMDropDown>();
            Emailtemplates = (from c in db.ContentEditors.Where(m => m.OrganizationID == OrgID && m.Category == 1).ToList()
                              select new VMDropDown { text = c.Name, Value = c.ID }).ToList();
            model.EmailTemplates = Emailtemplates;
            List<VMDropDown> SMStemplates = new List<VMDropDown>();
            SMStemplates = (from c in db.ContentEditors.Where(m => m.OrganizationID == OrgID && m.Category == 2).ToList()
                            select new VMDropDown { text = c.Name, Value = c.ID }).ToList();
            model.SMSTemplates = SMStemplates;
            List<VMDropDown> AllStages = new List<VMDropDown>();
            AllStages = (from c in dbcontext.Stages.Where(m => m.StatusTypeID == 10).ToList()
                         select new VMDropDown { text = c.Name, Value = c.ID }).ToList();
            model.Stages = AllStages;
            var Popups = dbcontext.Popups.Where(m => m.StatusTypeID == 10).ToList();
            List<VMDropDown> AllPopups = new List<VMDropDown>();
            AllPopups = (from c in dbcontext.Popups.Where(m => m.StatusTypeID == 10).ToList()
                         select new VMDropDown { text = c.Name, Value = c.ID }).ToList();
            model.Popups = AllPopups;
            var OneCLicks = dbcontext.Reports.Where(m => m.StatusTypeID == 10).ToList();
            List<VMDropDown> AllOneClicks = new List<VMDropDown>();
            AllOneClicks = (from c in dbcontext.Reports.Where(m => m.StatusTypeID == 10).ToList()
                            select new VMDropDown { text = c.Name, Value = c.ID }).ToList();
            model.OneClicks = AllOneClicks;
            return model;
        }
        public string SendMail(int iUserID, string sOrgName)
        {
            string ToEmailID = "", usern = "", pass = "", sender = "", security = "", serverName = "", messageBody = "", emailSubject = "";
            int port = 0;
            var Dbs = dbcontext.XIAppUsers.Select(m => m.sDatabaseName).Distinct().ToList();
            foreach (var db in Dbs)
            {
                DataContext Spdb = new DataContext(db);
                var Outbounds = Spdb.Outbounds.Where(m => m.StatusTypeID == 0).ToList();
                foreach (Outbounds items in Outbounds)
                {
                    if (items.Type == 1)
                    {
                        ToEmailID = items.Email;
                        //Get Template Category
                        var Category = Spdb.ContentEditors.Where(m => m.ID == items.TemplateID).Select(m => m.Category).FirstOrDefault();
                        //get email server details
                        var SDetails = dbcontext.IOServerDetails.Where(m => m.Type == items.Type).Where(m => m.OrganizationID == items.OrganizationID).ToList();
                        foreach (var item in SDetails)
                        {
                            usern = item.UserName;
                            pass = item.Password;
                            sender = item.FromAddress;
                            serverName = item.ServerName;
                            port = item.Port;
                            security = item.Security;
                        }
                        //getting msg content
                        messageBody = Spdb.ContentEditors.Where(m => m.ID == items.TemplateID).Select(m => m.Content).FirstOrDefault();
                        //search for replacable content
                        var GetFieldsRgx = @"(?<=\{{)[^}]*(?=\}})";
                        List<string> Columns = new List<string>();
                        foreach (Match m in Regex.Matches(messageBody, GetFieldsRgx))
                        {
                            Columns.Add(m.Value);
                        }
                        if (Category == 2)
                        {
                            emailSubject = "Source Communication";
                            //Appending Errors
                            var lErrorList = new List<string>();
                            int n = 0;
                            if (items.FileID != 0)
                            {
                                var sErrors = Spdb.ImportingErrorDetails.Where(m => m.FileID == items.FileID).Select(m => m.Message).ToList();
                                foreach (var error in sErrors)
                                {
                                    lErrorList.Add(error);
                                }
                                string ErrorMsgs = "";
                                foreach (var mesg in lErrorList)
                                {
                                    ErrorMsgs = ErrorMsgs + mesg + "<br/>";
                                }
                                foreach (var data in Columns)
                                {
                                    messageBody = messageBody.Replace("{{" + Columns[n] + "}}", ErrorMsgs);
                                    n++;
                                }
                            }
                            else
                            {
                                messageBody = messageBody + "<br/>Please check the file and send again.<br/><br/> Regards,<br/> Team XIDNA. <br/>";
                            }

                        }
                        else
                        {
                            emailSubject = "Lead Communication";
                            ContentRepository Content = new ContentRepository();
                            List<VMLeadEmail> res = Content.GetLeadsData(items.LeadID.ToString(), Columns, db, iUserID, sOrgName);
                            foreach (var user in res)
                            {
                                int n = 0;
                                foreach (var lead in user.Data)
                                {
                                    string Con = "";
                                    foreach (var item in lead)
                                    {
                                        if (item != null)
                                        {
                                            Con = Con + item + ",  ";
                                        }
                                    }
                                    Con = Con.Substring(0, Con.Length - 2);
                                    Con = Con.TrimEnd(',');
                                    messageBody = messageBody.Replace("{{" + Columns[n] + "}}", Con);
                                    n++;
                                }
                            }
                        }
                        //Search for images in content
                        string pattern = "(?<=src=\")[^,]+?(?=\")";
                        string input = messageBody;
                        string physicalPath = "";
                        List<string> ContentIds = new List<string>();
                        List<string> Paths = new List<string>();
                        if (messageBody.IndexOf("src=") >= 0)
                        {
                            int i = 1;
                            foreach (Match m in Regex.Matches(input, pattern))
                            {
                                physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                                Paths.Add(physicalPath + m.Value);
                                ContentIds.Add("ContentID" + i);
                                messageBody = messageBody.Replace(m.Value, "cid:ContentID" + i);
                                i++;
                            }
                        }
                        //Replacing Image Code
                        string html = @"<html><body>" + messageBody + "</body></html>";
                        AlternateView altView = AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html);
                        for (int j = 0; j < Paths.Count(); j++)
                        {
                            string Path = Paths[j];
                            string CntID = ContentIds[j];
                            LinkedResource yourPictureRes = new LinkedResource(Path, MediaTypeNames.Image.Jpeg);
                            yourPictureRes.ContentId = CntID;
                            altView.LinkedResources.Add(yourPictureRes);
                        }
                        //Adding Mail Details
                        MailMessage msg = new MailMessage();
                        msg.AlternateViews.Add(altView);
                        string username = HttpUtility.UrlEncode(usern);
                        string password = HttpUtility.UrlEncode(pass);

                        var FilePath = Spdb.ImportHistories.Where(d => d.ID == items.FileID).FirstOrDefault();
                        msg.To.Add(ToEmailID);
                        msg.From = new MailAddress(sender);
                        msg.Subject = emailSubject;
                        if (items.Cc != "" && items.Cc != String.Empty && items.Cc != null)
                        {
                            msg.CC.Add(items.Cc);
                        }
                        string Attach = "";
                        msg.Body = html;
                        if (FilePath != null)
                        {
                            string AttachPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                            string str = AttachPath.Substring(0, AttachPath.Length) + "\\Content\\images\\";
                            Attach = str + FilePath.FileName;
                            var attachement = new Attachment(Attach);
                            attachement.Name = FilePath.OriginalName;
                            msg.Attachments.Add(attachement);
                        }
                        msg.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = serverName;
                        smtp.Port = port;
                        //for gmail
                        smtp.EnableSsl = false;
                        smtp.Credentials = new System.Net.NetworkCredential(usern, pass);
                        smtp.Send(msg);
                    }
                    else
                    {
                        string PhoneNumbers = items.Mobile;
                        var ServerDetails = dbcontext.IOServerDetails.Where(m => m.Type == items.Type).Where(m => m.OrganizationID == items.OrganizationID).FirstOrDefault();
                        string MessageBody = Spdb.ContentEditors.Where(m => m.ID == items.TemplateID).Select(m => m.Content).FirstOrDefault();
                        //search for replacable content
                        var GetFieldsRgx = @"(?<=\{{)[^}]*(?=\}})";
                        List<string> Columns = new List<string>();
                        foreach (Match m in Regex.Matches(MessageBody, GetFieldsRgx))
                        {
                            Columns.Add(m.Value);
                        }
                        ContentRepository Content = new ContentRepository();
                        List<VMLeadEmail> res = Content.GetLeadsData(items.LeadID.ToString(), Columns, db, iUserID, sOrgName);
                        foreach (var user in res)
                        {
                            int n = 0;
                            foreach (var lead in user.Data)
                            {
                                string Con = "";
                                foreach (var item in lead)
                                {
                                    if (item != null)
                                    {
                                        Con = Con + item + ",  ";
                                    }
                                }
                                Con = Con.Substring(0, Con.Length - 2);
                                Con = Con.TrimEnd(',');
                                MessageBody = MessageBody.Replace("{{" + Columns[n] + "}}", Con);
                                n++;
                            }
                        }
                        string BulkSMSPath = ServerDetails.SMSPath;
                        string SMSFromID = ServerDetails.SenderID;
                        string SMSUserID = ServerDetails.UserName;
                        string SMSPassword = ServerDetails.Password;
                        //string SMSAPIKey = "9qbw2qwzbebon74";
                        string strResult = BulkSMSPath + "username=" + SMSUserID + "&pass=" + SMSPassword + "&senderid=" + SMSFromID + "&dest_mobileno=" + PhoneNumbers + "&message=" + MessageBody + "&response=Y" + "&mt=4";
                        //string strResult = BulkSMSPath + "apiKey=" + SMSAPIKey + "&from=" + SMSFromID + "&to=" + PhoneNumbers + "&message=" + MessageBody + "&mt=4";
                        WebClient WebClient = new WebClient();
                        System.IO.StreamReader reader = new System.IO.StreamReader(WebClient.OpenRead(strResult));
                        dynamic ResultHTML = reader.ReadToEnd();
                        return ResultHTML;
                    }
                    //var changestatus = Spdb.Outbounds.Where(m => m.ID == items.ID).FirstOrDefault();
                    //changestatus.StatusTypeID = 1;
                    //Spdb.SaveChanges();
                }
            }
            return null;
        }
        public List<string> GetLeadEmailID(int LeadID, int OrgID)
        {
            List<string> Ids = new List<string>();
            string EmailID = "";
            string database = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
            {
                SqlCommand cmd = new SqlCommand();
                Con.Open();
                Con.ChangeDatabase(database);
                cmd.Connection = Con;
                string FilteredDetails = "SELECT [sEmail] FROM [dbo].[Leads] WHERE FKiOrgID=" + OrgID + " AND ID=" + LeadID;
                cmd.CommandText = FilteredDetails;
                SqlDataReader reader = cmd.ExecuteReader();
                int count = reader.FieldCount;
                while (reader.Read())
                {
                    EmailID = reader.IsDBNull(0) ? null : reader.GetValue(0).ToString();
                }
                Ids.Add(EmailID);
                var ClientID = dbcontext.WalletRequests.Where(m => m.EmailID == EmailID).Select(m => m.ClientID).FirstOrDefault();
                Ids.Add(ClientID);
            }
            return Ids;
        }

        public DTResponse GetUploadedDocsGrid(jQueryDataTableParamModel param, string ClientID, int OrgID)
        {
            var database = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(database);
            IEnumerable<OrganizationDocuments> AllDocs, FilteredDocs;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredDocs = Spdb.OrganizationDocuments.Where(m => m.ClientID == ClientID && m.OrganizationID == OrgID).Where(m => m.OriginalName.Contains(param.sSearch)).OrderByDescending(m => m.ID).ToList();
                AllDocs = FilteredDocs.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = FilteredDocs.Count();
            }
            else
            {
                displyCount = Spdb.OrganizationDocuments.Where(m => m.ClientID == ClientID && m.OrganizationID == OrgID).Count();
                AllDocs = Spdb.OrganizationDocuments.Where(m => m.ClientID == ClientID && m.OrganizationID == OrgID).OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            }
            var result = from c in AllDocs
                         select new[] {
                             (i++).ToString(),Convert.ToString(c.ID), c.DocumentName,c.OriginalName,c.Message,c.StatusTypeID.ToString(),"" };
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        public VMQueryPreview PostQuoteToLead(VMViewPopup Popup, string sOrgName)
        {
            string database = dbcontext.Organization.Where(m => m.ID == Popup.OrganizationID).Select(m => m.DatabaseName).FirstOrDefault();
            BOs Bo = dbcontext.BOs.Find(Popup.BOID);
            DataContext Spdb = new DataContext(database);
            if (Bo.Name == EnumLeadTables.WalletQuotes.ToString() || Bo.Name == EnumLeadTables.WalletPolicies.ToString())
            {
                if (Bo.Name == EnumLeadTables.WalletQuotes.ToString())
                {
                    WalletQuotes Qute = new WalletQuotes();
                    Qute = Spdb.WalletQuotes.Find(Popup.QuoteID);
                    Qute.IsPosted = true;
                    Spdb.SaveChanges();
                }
                else if (Bo.Name == EnumLeadTables.WalletPolicies.ToString())
                {
                    WalletPolicies Polcy = new WalletPolicies();
                    Polcy = Spdb.WalletPolicies.Find(Popup.QuoteID);
                    Polcy.IsPosted = true;
                    Spdb.SaveChanges();
                }

                //LeadQuotes Quote = new LeadQuotes();
                //Quote.QuoteID = Popup.QuoteID;
                //Quote.LeadID = Popup.LeadID;
                //Quote.PostedOn = DateTime.Now;
                //Quote.OrganizationID = Popup.OrganizationID;
                //Spdb.LeadQuotes.Add(Quote);
                //Spdb.SaveChanges();
                Common Com = new Common();
                var result = Com.GetHeadings(Popup.FKi1ClickIDXIGUID, database, Popup.OrganizationID, Popup.UserID, sOrgName);
                var report = dbcontext.Tab1Clicks.Where(m => m.ID == Popup.Tab1ClickID).FirstOrDefault();
                int sectionid = Convert.ToInt32(report.SectionID);
                result.SectionName = dbcontext.Sections.Where(m => m.ID == sectionid).Select(m => m.Name).FirstOrDefault();
                result.TabID = report.TabID;
                result.Tab1ClickID = Popup.Tab1ClickID;
                result.BOID = Popup.BOID;
                result.IsView = report.IsView;
                result.IsCreate = report.IsCreate;
                result.IsEdit = report.IsEdit;
                result.ReportID = Popup.ReportID;
                result.XIGUID = Popup.FKi1ClickIDXIGUID;
                result.TabName = dbcontext.Tabs.Where(m => m.ID == report.TabID).Select(m => m.Name).FirstOrDefault();
                return result;
            }
            return null;
        }

        public int PostMessage(WalletMessages Message)
        {
            var EmailID = dbcontext.WalletRequests.Where(m => m.ClientID == Message.ClientID).Select(m => m.EmailID).FirstOrDefault();
            var Org = dbcontext.Organization.Find(Message.OrganizationID);
            DataContext Spdb = new DataContext(Org.DatabaseName);
            WalletMessages Msg = new WalletMessages();
            Msg.Message = Message.Message;
            Msg.MailType = Message.MailType;
            Msg.ReceivedOn = DateTime.Now;
            Msg.ClientID = Message.ClientID;
            Msg.OrganizationID = Message.OrganizationID;
            Msg.IsRead = false;
            Msg.ProductID = 0;
            Msg.Sender = 0;
            Msg.EmailID = EmailID;
            Msg.Type = "Email";
            Spdb.WalletMessages.Add(Msg);
            Spdb.SaveChanges();
            return Msg.ID;
        }

        public LeadInbounds ShowViewImport(int ID, int OrgID)
        {
            var Org = dbcontext.Organization.Find(OrgID);
            DataContext Spdb = new DataContext(Org.DatabaseName);
            LeadInbounds Inbound = new LeadInbounds();
            int InboundID = Spdb.Database.SqlQuery<int>("Select InboundID from " + EnumLeadTables.Leads.ToString() + " Where ID=" + ID).FirstOrDefault();
            if (InboundID > 0)
            {
                Inbound = Spdb.LeadInbounds.Find(InboundID);
            }
            return Inbound;
        }
        public int GetInnerReportID(int ReportID)
        {
            var InnerReportID = dbcontext.Reports.Where(m => m.ID == ReportID).Select(m => m.InnerReportID).FirstOrDefault();
            return InnerReportID;
        }

        public List<string> GetDatabases()
        {
            List<string> databases = dbcontext.XIAppUsers.Select(m => m.sDatabaseName).Distinct().ToList();
            return databases;
        }
        public int SendRegisterMail(int iUserID, string Email, string Type, int OrgID, string sDatabase, int ClassID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext(sDatabase);
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            WalletRequests WRequest = new WalletRequests();
            WRequest.OrganizationID = OrgID;
            WRequest.EmailID = Email;
            WRequest.FKiLeadClassID = ClassID;
            WRequest.IsActivated = false;
            Spdb.WalletRequests.Add(WRequest);
            Spdb.SaveChanges();
            Common Com = new Common();
            int Result = Com.SendUserRegisterMail(iUserID, Email, Type, sDatabase, OrgID, sOrgName);
            return Result;
        }
        //Getting Organization Names
        private string GetOrgName(int p)
        {
            var orgname = dbcontext.Organization.Where(m => m.ID == p).Select(m => m.Name).FirstOrDefault();
            return orgname;
        }
        //Getting the Dropdown List For Classes
        public List<VMDropDown> ClassesList(int OrgID, int ID, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext(sDatabase);
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            if (sDatabase == null)
            {
                sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            }
            DataContext Spdb = new DataContext(sOrgDB);
            if (OrgID == 0)
            {
                List<VMDropDown> OrgClassFields = new List<VMDropDown>();
                var Fields = dbcontext.Types.Where(m => m.Name == "Class Type").ToList();
                foreach (var items in Fields)
                {
                    if (ID > 0)
                    {
                        OrgClassFields.Add(new VMDropDown
                        {
                            text = items.Expression,
                            Value = items.ID
                        });

                    }
                    else
                    {
                        int classID = items.ID;
                        int ClassExists = Spdb.LeadConfigurations.Where(m => m.Class == classID).Select(m => m.ID).FirstOrDefault();
                        if (ClassExists == 0)
                        {
                            OrgClassFields.Add(new VMDropDown
                            {
                                text = items.Expression,
                                Value = items.ID
                            });

                        }

                    }

                }
                return OrgClassFields;
            }
            else
            {
                List<VMDropDown> ClassFields = new List<VMDropDown>();
                var Fields = Spdb.OrganizationClasses.Where(m => m.OrganizationID == OrgID).ToList();
                foreach (var items in Fields)
                {
                    if (ID > 0)
                    {
                        ClassFields.Add(new VMDropDown
                        {
                            text = items.Class,
                            Value = items.ClassID
                        });

                    }
                    else
                    {
                        int classID = items.ClassID;
                        int ClassExists = Spdb.LeadConfigurations.Where(m => m.Class == classID && m.OrganizationID == OrgID).Select(m => m.ID).FirstOrDefault();
                        if (ClassExists == 0)
                        {
                            ClassFields.Add(new VMDropDown
                            {
                                text = items.Class,
                                Value = items.ClassID
                            });
                        }
                    }

                }

                return ClassFields;
            }

        }
        //Getting the Class Names
        private string GetClasses(int p)
        {
            DataContext Spdb = new DataContext();
            var typename = Spdb.OrganizationClasses.Where(m => m.ClassID == p).Select(m => m.Class).FirstOrDefault();
            return typename;
        }
        private string getconversionstatus(int? LeadID, string database)
        {
            int StageID = 0;
            string Status = "";
            DataContext Spdb = new DataContext(database);
            StageID = Spdb.Database.SqlQuery<int>("SELECT StageID FROM LeadTransitions Where LeadID = " + LeadID).FirstOrDefault();
            if (StageID > 0)
            {
                Status = getstatusname(StageID, database);
            }
            else
            {
                Status = "Inbound";
            }
            return Status;
        }
        private string getstatusname(int p, string database)
        {
            var status = dbcontext.Stages.Where(m => m.ID == p).Select(m => m.Name).FirstOrDefault();
            return status;
        }

        public int GetLeadStage(int LeadID, string database)
        {
            DataContext Spdb = new DataContext(database);
            var StageID = Spdb.LeadHistories.Where(m => m.LeadID == LeadID).Select(m => m.StageID).FirstOrDefault();
            return StageID;
        }

        private string getclass(int p, int OrgID, string database)
        {
            DataContext Spdb = new DataContext(database);
            var classname = Spdb.OrganizationClasses.Where(m => m.ClassID == p && m.OrganizationID == OrgID).Select(m => m.Class).FirstOrDefault();
            return classname;
        }
        #endregion miscellaneous
    }
}