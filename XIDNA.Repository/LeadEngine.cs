using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.ViewModels;
using System.Linq.Expressions;
using XIDNA.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Globalization;

namespace XIDNA.Repository
{
    public class LeadEngine
    {
        public VMLeadEngine GetLeadDetails(List<FormData> Values, int OrgID)
        {
            DataContext dbcontext = new DataContext("XIDynawareClient");
            ModelDbContext modeldb = new ModelDbContext();
            int LeadID = -1, FKiSourceID = 0, OldFKiSourceID = 0, ClientID = 0;
            string sMob = "", sTel = "", sEMail = "", sForeName = "", sLastName = "", sPostCode = "";
            DateTime? dDOB = new DateTime(1998, 04, 30);
            DateTime dImportedOn = new DateTime();
            int FKiLeadClassID = 0;
            for (int i = 0; i < Values.Count(); i++)
            {
                if (Values[i].Label == "sMob")
                {
                    sMob = Values[i].Value;
                }
                if (Values[i].Label == "sTel")
                {
                    sTel = Values[i].Value;
                }
                if (Values[i].Label == "sEMail")
                {
                    sEMail = Values[i].Value;
                }
                if (Values[i].Label == "sForeName")
                {
                    sForeName = Values[i].Value;
                }
                if (Values[i].Label == "sLastName")
                {
                    sLastName = Values[i].Value;
                }
                if (Values[i].Label == "sPostCode")
                {
                    sPostCode = Values[i].Value;
                }
                if (Values[i].Label == "dDOB")
                {
                    dDOB = Convert.ToDateTime(Values[i].Value);
                }
                if (Values[i].Label == "FKiLeadClassID")
                {
                    FKiLeadClassID = Convert.ToInt32(Values[i].Value);
                }
                if (Values[i].Label == "FKiSourceID")
                {
                    FKiSourceID = Convert.ToInt32(Values[i].Value);
                }
            }

            bool bInsertLead = false, bLoaded = false;
            Expression<Func<Datas, bool>> sLeadMatchWhere = null;
            ParameterExpression parameterExpression = Expression.Parameter(typeof(Datas), "L");
            MemberExpression sMobExpr = Expression.PropertyOrField(parameterExpression, "sMob");
            MemberExpression sMob2Expr = Expression.PropertyOrField(parameterExpression, "sMob");
            MemberExpression sMob3Expr = Expression.PropertyOrField(parameterExpression, "sMob");
            MemberExpression sTelExpr = Expression.PropertyOrField(parameterExpression, "sTel");
            MemberExpression sEMailExpr = Expression.PropertyOrField(parameterExpression, "sEMail");
            MemberExpression sEMail2Expr = Expression.PropertyOrField(parameterExpression, "sEMail");
            MemberExpression sEMail3Expr = Expression.PropertyOrField(parameterExpression, "sEMail");
            MemberExpression sEMail4Expr = Expression.PropertyOrField(parameterExpression, "sEMail");
            MemberExpression sEMail5Expr = Expression.PropertyOrField(parameterExpression, "sEMail");
            MemberExpression sNameExpr = Expression.PropertyOrField(parameterExpression, "sName");
            MemberExpression sPostCodeExpr = Expression.PropertyOrField(parameterExpression, "sPostCode");
            MemberExpression dDOBExpr = Expression.PropertyOrField(parameterExpression, "dDOB");
            MemberExpression FKiLeadClassIDExpr = Expression.PropertyOrField(parameterExpression, "FKiLeadClassID");

            ConstantExpression sMobvalueExpr = Expression.Constant(sMob);
            ConstantExpression sTelvalueExpr = Expression.Constant(sTel);
            ConstantExpression sEMailvalueExpr = Expression.Constant(sEMail);
            ConstantExpression sNamevalueExpr = Expression.Constant(sForeName + " " + sLastName);
            ConstantExpression sPostcodevalueExpr = Expression.Constant(sPostCode);
            ConstantExpression dDOBvalueExpr = Expression.Constant(dDOB, typeof(DateTime?));
            ConstantExpression FKiLeadClassIDvalueExpr = Expression.Constant(FKiLeadClassID);

            BinaryExpression sMobnsMobBinExpr = Expression.Equal(sMobExpr, sMobvalueExpr);
            BinaryExpression sMob2nsMobBinExpr = Expression.Equal(sMob2Expr, sMobvalueExpr);
            BinaryExpression sMob3nsMobBinExpr = Expression.Equal(sMob3Expr, sMobvalueExpr);
            BinaryExpression sMobnsTelBinExpr = Expression.Equal(sMobExpr, sTelvalueExpr);
            BinaryExpression sTelnsMobBinExpr = Expression.Equal(sTelExpr, sMobvalueExpr);
            BinaryExpression sTelnsTelBinExpr = Expression.Equal(sTelExpr, sTelvalueExpr);
            BinaryExpression sEmailnsEmailBinExpr = Expression.Equal(sEMailExpr, sEMailvalueExpr);
            BinaryExpression sEmail2nsEmailBinExpr = Expression.Equal(sEMail2Expr, sEMailvalueExpr);
            BinaryExpression sEmail3nsEmailBinExpr = Expression.Equal(sEMail3Expr, sEMailvalueExpr);
            BinaryExpression sEmail4nsEmailBinExpr = Expression.Equal(sEMail4Expr, sEMailvalueExpr);
            BinaryExpression sEmail5nsEmailBinExpr = Expression.Equal(sEMail5Expr, sEMailvalueExpr);
            BinaryExpression sNamensNameBinExpr = Expression.Equal(sNameExpr, sNamevalueExpr);
            BinaryExpression sPostcodensPostcodeBinExpr = Expression.Equal(sPostCodeExpr, sPostcodevalueExpr);
            BinaryExpression dDOBndDOBBinExpr = Expression.Equal(dDOBExpr, dDOBvalueExpr);
            BinaryExpression FKiLeadClassIDBinExpr = Expression.Equal(FKiLeadClassIDExpr, FKiLeadClassIDvalueExpr);

            if ((sMob != null) && (sTel != null) && (sEMail != null))
            {
                Expression E1rE2 = Expression.OrElse(sMobnsMobBinExpr, sMob2nsMobBinExpr);
                Expression E1rE2rE3 = Expression.OrElse(sMob3nsMobBinExpr, E1rE2);
                Expression E1rE2rE3rE4 = Expression.OrElse(sMobnsTelBinExpr, E1rE2rE3);
                Expression E1rE2rE3rE4rE5 = Expression.OrElse(sTelnsMobBinExpr, E1rE2rE3rE4);
                Expression E1rE2rE3rE4rE5rE6 = Expression.OrElse(sTelnsTelBinExpr, E1rE2rE3rE4rE5);
                Expression E1rE2rE3rE4rE5rE6rE7 = Expression.OrElse(sEmailnsEmailBinExpr, E1rE2rE3rE4rE5rE6);
                Expression E1rE2rE3rE4rE5rE6rE7rE8 = Expression.OrElse(sEmail2nsEmailBinExpr, E1rE2rE3rE4rE5rE6rE7);
                Expression E1rE2rE3rE4rE5rE6rE7rE8rE9 = Expression.OrElse(sEmail3nsEmailBinExpr, E1rE2rE3rE4rE5rE6rE7rE8);
                Expression E1rE2rE3rE4rE5rE6rE7rE8rE9rE10 = Expression.OrElse(sEmail4nsEmailBinExpr, E1rE2rE3rE4rE5rE6rE7rE8rE9);
                Expression E1rE2rE3rE4rE5rE6rE7rE8rE9rE10rE11 = Expression.OrElse(sEmail5nsEmailBinExpr, E1rE2rE3rE4rE5rE6rE7rE8rE9rE10);
                sLeadMatchWhere = Expression.Lambda<Func<Datas, bool>>(E1rE2rE3rE4rE5rE6rE7rE8rE9rE10rE11, parameterExpression);
                // LeadID = dbcontext.Leads.Where(sLeadMatchWhere).Select(L => L.id).FirstOrDefault();
            }
            else if ((sMob != null) && (sTel != null))
            {
                Expression E1rE2 = Expression.OrElse(sMobnsMobBinExpr, sMob2nsMobBinExpr);
                Expression E1rE2rE3 = Expression.OrElse(sMob3nsMobBinExpr, E1rE2);
                Expression E1rE2rE3rE4 = Expression.OrElse(sMobnsTelBinExpr, E1rE2rE3);
                Expression E1rE2rE3rE4rE5 = Expression.OrElse(sTelnsMobBinExpr, E1rE2rE3rE4);
                Expression E1rE2rE3rE4rE5rE6 = Expression.OrElse(sTelnsTelBinExpr, E1rE2rE3rE4rE5);
                sLeadMatchWhere = Expression.Lambda<Func<Datas, bool>>(E1rE2rE3rE4rE5rE6, parameterExpression);
                //LeadID = dbcontext.Leads.Where(sLeadMatchWhere).Select(L => L.id).FirstOrDefault();
            }
            else if ((sMob != null) && (sEMail != null))
            {
                Expression E1rE2 = Expression.OrElse(sMobnsMobBinExpr, sMob2nsMobBinExpr);
                Expression E1rE2rE3 = Expression.OrElse(sMob3nsMobBinExpr, E1rE2);
                Expression E1rE2rE3rE4 = Expression.OrElse(sEmailnsEmailBinExpr, E1rE2rE3);
                Expression E1rE2rE3rE4rE5 = Expression.OrElse(sEmail2nsEmailBinExpr, E1rE2rE3rE4);
                Expression E1rE2rE3rE4rE5rE6 = Expression.OrElse(sEmail3nsEmailBinExpr, E1rE2rE3rE4rE5);
                Expression E1rE2rE3rE4rE5rE6rE7 = Expression.OrElse(sEmail4nsEmailBinExpr, E1rE2rE3rE4rE5rE6);
                Expression E1rE2rE3rE4rE5rE6rE7rE8 = Expression.OrElse(sEmail5nsEmailBinExpr, E1rE2rE3rE4rE5rE6rE7);
                sLeadMatchWhere = Expression.Lambda<Func<Datas, bool>>(E1rE2rE3rE4rE5rE6rE7rE8, parameterExpression);
                //LeadID = dbcontext.Leads.Where(sLeadMatchWhere).Select(L => L.id).FirstOrDefault();
            }
            else if ((sMob != null))
            {
                Expression E1rE2 = Expression.OrElse(sMobnsMobBinExpr, sMob2nsMobBinExpr);
                Expression E1rE2rE3 = Expression.OrElse(sMob3nsMobBinExpr, E1rE2);
                Expression E1rE2rE3rE5 = Expression.OrElse(sTelnsMobBinExpr, E1rE2rE3);
                sLeadMatchWhere = Expression.Lambda<Func<Datas, bool>>(E1rE2rE3rE5, parameterExpression);
                // LeadID = dbcontext.Leads.Where(sLeadMatchWhere).Select(L => L.id).FirstOrDefault();
            }
            else if ((sTel != null))
            {
                BinaryExpression sMob2nsTelBinExpr = Expression.Equal(sMob2Expr, sTelvalueExpr);
                BinaryExpression sMob3nsTelBinExpr = Expression.Equal(sMob3Expr, sTelvalueExpr);
                Expression E1rE2 = Expression.OrElse(sMobnsTelBinExpr, sMob2nsTelBinExpr);
                Expression E1rE2rE3 = Expression.OrElse(sMob3nsTelBinExpr, E1rE2);
                Expression E1rE2rE3rE4 = Expression.OrElse(sTelnsTelBinExpr, E1rE2rE3);
                sLeadMatchWhere = Expression.Lambda<Func<Datas, bool>>(E1rE2rE3rE4, parameterExpression);
                //LeadID = dbcontext.Leads.Where(sLeadMatchWhere).Select(L => L.id).FirstOrDefault();
            }
            else if (sEMail != null)
            {
                Expression E1rE2 = Expression.OrElse(sEmailnsEmailBinExpr, sEmail2nsEmailBinExpr);
                Expression E1rE2rE3 = Expression.OrElse(sEmail3nsEmailBinExpr, E1rE2);
                Expression E1rE2rE3rE4 = Expression.OrElse(sEmail4nsEmailBinExpr, E1rE2rE3);
                Expression E1rE2rE3rE4rE5 = Expression.OrElse(sEmail5nsEmailBinExpr, E1rE2rE3rE4);
                sLeadMatchWhere = Expression.Lambda<Func<Datas, bool>>(E1rE2rE3rE4rE5, parameterExpression);
                //LeadID = dbcontext.Leads.Where(sLeadMatchWhere).Select(L => L.id).FirstOrDefault();
            }
            else
            {
                LeadID = 0;
                bInsertLead = true;
                if (sEMail != null && sEMail.Length > 2)
                {
                    if (sPostCode != null)
                    {
                        Expression E1rE2 = Expression.AndAlso(sNamensNameBinExpr, sPostcodensPostcodeBinExpr);
                        sLeadMatchWhere = Expression.Lambda<Func<Datas, bool>>(E1rE2, parameterExpression);
                    }
                }
                else
                {
                    if (dDOB != null)
                    {
                        if (Convert.ToInt32(dDOB.Value.Year.ToString()) > 1900)
                        {
                            Expression E1rE2 = Expression.AndAlso(sNamensNameBinExpr, dDOBndDOBBinExpr);
                            sLeadMatchWhere = Expression.Lambda<Func<Datas, bool>>(E1rE2, parameterExpression);
                        }
                    }
                }
            }

            if (FKiLeadClassID != 0)
            {
                Expression<Func<Datas, bool>> LeadClassID =
                Expression.Lambda<Func<Datas, bool>>(FKiLeadClassIDBinExpr, parameterExpression);
                Expression<Func<Datas, bool>> body = null;
                if (sLeadMatchWhere != null)
                {
                    body = Expression.Lambda<Func<Datas, bool>>(
                           Expression.AndAlso(
                           ((LambdaExpression)LeadClassID).Body,
                           ((LambdaExpression)sLeadMatchWhere).Body),
                           parameterExpression);
                }
                else
                {
                    body = Expression.Lambda<Func<Datas, bool>>(FKiLeadClassIDBinExpr, parameterExpression);
                }
                var orgname = modeldb.Organization.Where(m => m.ID == OrgID).Select(m => m.Name).FirstOrDefault();
                var LeadsTable = EnumLeadTables.Leads.ToString();
                //string InstanceTable = "LeadInstance" + OrgID;
                string boundid = "Select ID from " + LeadsTable + " where " + body;


                boundid = boundid.Replace("L =>", "").Replace("(L.", "(").Replace("==", "=").Replace(@"\", "").Replace(@"""", @"'").Replace("AndAlso", "And").Replace("OrElse", "OR");
                //LeadID = dbContext.Datas.Where(body).Select(L => L.id).FirstOrDefault();
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    cmd.CommandText = boundid;
                    Con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LeadID = reader.GetInt32(0);
                    }
                    reader.Close();
                    if (LeadID > 0)
                    {
                        string ImportedOn = "Select dImportedOn, FKiSourceID from LeadInstances where ID = " + LeadID;
                        cmd.CommandText = ImportedOn;
                        SqlDataReader datareader = cmd.ExecuteReader();
                        while (datareader.Read())
                        {
                            dImportedOn = Convert.ToDateTime(datareader.GetDateTime(0));
                            OldFKiSourceID = datareader.GetInt32(1);
                        }
                    }
                    Con.Close();
                }
                if (LeadID > 0)
                {
                    bLoaded = true;
                }
                if (bLoaded)
                {
                    bInsertLead = false;
                }
                else
                {
                    bInsertLead = true;
                }
                string ClientsTable = EnumLeadTables.LeadClients.ToString();
                using (SqlConnection Conn = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Conn;
                    cmd.CommandText = boundid;
                    Conn.Open();
                    if (LeadID > 0)
                    {
                        string ImportedOn = "Select ID from " + ClientsTable + " where sName='" + sForeName + " " + sLastName + "' And iClassID = " + FKiLeadClassID;
                        cmd.CommandText = ImportedOn;
                        SqlDataReader datareader = cmd.ExecuteReader();
                        while (datareader.Read())
                        {
                            ClientID = datareader.GetInt32(0);
                        }
                    }
                    Conn.Close();
                }
            }
            VMLeadEngine engine = new VMLeadEngine();
            var interval = dbcontext.LeadConfigurations.Where(m => m.OrganizationID == OrgID).Where(m => m.Class == FKiLeadClassID).Select(m => m.Interval).FirstOrDefault();
            if (LeadID > 0)
            {
                int difference = Convert.ToInt32((DateTime.Now.Date - dImportedOn.Date).TotalDays);
                if (difference > interval)
                {
                    engine.bUpdateSource = true;
                }
                else
                {
                    engine.bUpdateSource = false;
                }
            }

            engine.bInsertLead = bInsertLead;
            engine.LeadID = LeadID;
            engine.Interval = interval;
            engine.SourceID = FKiSourceID;
            engine.OldSourceID = OldFKiSourceID;
            engine.ClassID = FKiLeadClassID;
            engine.ClientID = ClientID;
            return engine;
            //        int LeadID = -1;
            //        string sMobValue = "", sTelValue = "", sEMailValue = "", sForeNameValue = "", sLastNameValue = "", sPostCodeValue = "";
            //        DateTime? dDOBValue = null;
            //        int FKiLeadClassIDValue = 0;
            //        for (int i = 0; i < Values.Count(); i++)
            //        {
            //            if (Values[i].Label == "sMob")
            //            {
            //                sMobValue = Values[i].Value;
            //            }
            //            if (Values[i].Label == "sTel")
            //            {
            //                sTelValue = Values[i].Value;
            //            }
            //            if (Values[i].Label == "sEMail")
            //            {
            //                sEMailValue = Values[i].Value;
            //            }
            //            if (Values[i].Label == "sForeName")
            //            {
            //                sForeNameValue = Values[i].Value;
            //            }
            //            if (Values[i].Label == "sLastName")
            //            {
            //                sLastNameValue = Values[i].Value;
            //            }
            //            if (Values[i].Label == "sPostCode")
            //            {
            //                sPostCodeValue = Values[i].Value;
            //            }
            //            if (Values[i].Label == "dDOB")
            //            {
            //                dDOBValue = Convert.ToDateTime(Values[i].Value);
            //            }
            //            if (Values[i].Label == "FKiLeadClassID")
            //            {
            //                FKiLeadClassIDValue = Convert.ToInt32(Values[i].Value);
            //            }
            //        }
            //        string sNameValue = sForeNameValue + sLastNameValue;

            //        bool bInsertLead = false, bLoaded = false;
            //        List<VMFieldExpressions> Exprs = new List<VMFieldExpressions>();
            //        var Fields = "Select Field, Expression From LeadExpressions";
            //        SqlConnection Conn = new SqlConnection(ServiceUtil.GetClientConnectionString());
            //        using (SqlCommand cmd = new SqlCommand("", Conn))
            //        {
            //            cmd.CommandText = Fields;
            //            Conn.Open();
            //            SqlDataReader reader = cmd.ExecuteReader();
            //            while (reader.Read())
            //            {
            //                Exprs.Add(new VMFieldExpressions
            //                {
            //                    FieldName = reader.GetString(0),
            //                    Expression = reader.GetString(1)
            //                });
            //            }
            //            reader.Close();
            //            Conn.Close();
            //        }
            //        string sLeadMatchWhere = null;
            //        string MobExprs = "", EmailExprs = "", TelExprs = "", NameExprs = "", PostCodeExprs = "", DOBExprs = "", ClassExprs = "";
            //        foreach (var items in Exprs)
            //        {
            //            if (items.FieldName == "sMob")
            //            {
            //                MobExprs = items.Expression;
            //                MobExprs = MobExprs.Replace("sMobValue", "'" + sMobValue + "'");
            //                MobExprs = MobExprs.Replace("sTelValue", "'"+sTelValue+"'");
            //            }
            //            else if (items.FieldName == "sEMail")
            //            {
            //                EmailExprs = items.Expression;
            //                EmailExprs = EmailExprs.Replace("sEMailValue", "'" + sEMailValue + "'");
            //            }
            //            else if (items.FieldName == "sTel")
            //            {
            //                TelExprs = items.Expression;
            //                TelExprs = TelExprs.Replace("sTelValue", "'" + sTelValue + "'");
            //                TelExprs = TelExprs.Replace("sMobValue", "'" + sMobValue + "'");
            //            }
            //            else if (items.FieldName == "sName")
            //            {
            //                NameExprs = items.Expression;
            //                NameExprs = NameExprs.Replace("sNameValue", "'" + sNameValue + "'");
            //            }
            //            else if (items.FieldName == "sPostCode")
            //            {
            //                PostCodeExprs = items.Expression;
            //                PostCodeExprs = PostCodeExprs.Replace("sPostCodeValue", "'" + sPostCodeValue + "'");
            //            }
            //            else if (items.FieldName == "dDOB")
            //            {
            //                DOBExprs = items.Expression;
            //                DOBExprs = DOBExprs.Replace("dDOBValue", dDOBValue.ToString());
            //            }
            //            else if (items.FieldName == "FKiLeadClassID")
            //            {
            //                ClassExprs = items.Expression;
            //                ClassExprs = ClassExprs.Replace("FKiLeadClassIDValue", FKiLeadClassIDValue.ToString());
            //            }
            //        }

            //        //ParameterExpression parameterExpression = Expression.Parameter(typeof(Datas), "L");
            //        //MemberExpression sMobExpr = Expression.PropertyOrField(parameterExpression, "sMob");
            //        //MemberExpression sMob2Expr = Expression.PropertyOrField(parameterExpression, "sMob2");
            //        //MemberExpression sMob3Expr = Expression.PropertyOrField(parameterExpression, "sMob3");
            //        //MemberExpression sTelExpr = Expression.PropertyOrField(parameterExpression, "sTel");
            //        //MemberExpression sEMailExpr = Expression.PropertyOrField(parameterExpression, "sEMail");
            //        //MemberExpression sEMail2Expr = Expression.PropertyOrField(parameterExpression, "sEMail2");
            //        //MemberExpression sEMail3Expr = Expression.PropertyOrField(parameterExpression, "sEMail3");
            //        //MemberExpression sEMail4Expr = Expression.PropertyOrField(parameterExpression, "sEMail4");
            //        //MemberExpression sEMail5Expr = Expression.PropertyOrField(parameterExpression, "sEMail5");
            //        //MemberExpression sNameExpr = Expression.PropertyOrField(parameterExpression, "sName");
            //        //MemberExpression sPostCodeExpr = Expression.PropertyOrField(parameterExpression, "sPostCode");
            //        //MemberExpression dDOBExpr = Expression.PropertyOrField(parameterExpression, "dDOB");
            //        //MemberExpression FKiLeadClassIDExpr = Expression.PropertyOrField(parameterExpression, "FKiLeadClassID");

            //        //ConstantExpression sMobvalueExpr = Expression.Constant(sMob);
            //        //ConstantExpression sTelvalueExpr = Expression.Constant(sTel);
            //        //ConstantExpression sEMailvalueExpr = Expression.Constant(sEMail);
            //        //ConstantExpression sNamevalueExpr = Expression.Constant(sForeName + " " + sLastName);
            //        //ConstantExpression sPostcodevalueExpr = Expression.Constant(sPostCode);
            //        //ConstantExpression dDOBvalueExpr = Expression.Constant(dDOB, typeof(DateTime?));
            //        //ConstantExpression FKiLeadClassIDvalueExpr = Expression.Constant(FKiLeadClassID);

            //        //BinaryExpression sMobnsMobBinExpr = Expression.Equal(sMobExpr, sMobvalueExpr);
            //        //BinaryExpression sMob2nsMobBinExpr = Expression.Equal(sMob2Expr, sMobvalueExpr);
            //        //BinaryExpression sMob3nsMobBinExpr = Expression.Equal(sMob3Expr, sMobvalueExpr);
            //        //BinaryExpression sMobnsTelBinExpr = Expression.Equal(sMobExpr, sTelvalueExpr);
            //        //BinaryExpression sTelnsMobBinExpr = Expression.Equal(sTelExpr, sMobvalueExpr);
            //        //BinaryExpression sTelnsTelBinExpr = Expression.Equal(sTelExpr, sTelvalueExpr);
            //        //BinaryExpression sEmailnsEmailBinExpr = Expression.Equal(sEMailExpr, sEMailvalueExpr);
            //        //BinaryExpression sEmail2nsEmailBinExpr = Expression.Equal(sEMail2Expr, sEMailvalueExpr);
            //        //BinaryExpression sEmail3nsEmailBinExpr = Expression.Equal(sEMail3Expr, sEMailvalueExpr);
            //        //BinaryExpression sEmail4nsEmailBinExpr = Expression.Equal(sEMail4Expr, sEMailvalueExpr);
            //        //BinaryExpression sEmail5nsEmailBinExpr = Expression.Equal(sEMail5Expr, sEMailvalueExpr);
            //        //BinaryExpression sNamensNameBinExpr = Expression.Equal(sNameExpr, sNamevalueExpr);
            //        //BinaryExpression sPostcodensPostcodeBinExpr = Expression.Equal(sPostCodeExpr, sPostcodevalueExpr);
            //        //BinaryExpression dDOBndDOBBinExpr = Expression.Equal(dDOBExpr, dDOBvalueExpr);
            //        //BinaryExpression FKiLeadClassIDBinExpr = Expression.Equal(FKiLeadClassIDExpr, FKiLeadClassIDvalueExpr);

            //        if ((sMobValue != null) && (sTelValue != null) && (sEMailValue != null))
            //        {

            //            //Expression E1rE2 = Expression.OrElse(sMobnsMobBinExpr, sMob2nsMobBinExpr);
            //            //Expression E1rE2rE3 = Expression.OrElse(sMob3nsMobBinExpr, E1rE2);
            //            //Expression E1rE2rE3rE4 = Expression.OrElse(sMobnsTelBinExpr, E1rE2rE3);
            //            //Expression E1rE2rE3rE4rE5 = Expression.OrElse(sTelnsMobBinExpr, E1rE2rE3rE4);
            //            //Expression E1rE2rE3rE4rE5rE6 = Expression.OrElse(sTelnsTelBinExpr, E1rE2rE3rE4rE5);
            //            //Expression E1rE2rE3rE4rE5rE6rE7 = Expression.OrElse(sEmailnsEmailBinExpr, E1rE2rE3rE4rE5rE6);
            //            //Expression E1rE2rE3rE4rE5rE6rE7rE8 = Expression.OrElse(sEmail2nsEmailBinExpr, E1rE2rE3rE4rE5rE6rE7);
            //            //Expression E1rE2rE3rE4rE5rE6rE7rE8rE9 = Expression.OrElse(sEmail3nsEmailBinExpr, E1rE2rE3rE4rE5rE6rE7rE8);
            //            //Expression E1rE2rE3rE4rE5rE6rE7rE8rE9rE10 = Expression.OrElse(sEmail4nsEmailBinExpr, E1rE2rE3rE4rE5rE6rE7rE8rE9);
            //            //Expression E1rE2rE3rE4rE5rE6rE7rE8rE9rE10rE11 = Expression.OrElse(sEmail5nsEmailBinExpr, E1rE2rE3rE4rE5rE6rE7rE8rE9rE10);
            //            sLeadMatchWhere = MobExprs + " or " + TelExprs + " or " + EmailExprs; 
            //            // LeadID = dbcontext.Leads.Where(sLeadMatchWhere).Select(L => L.id).FirstOrDefault();
            //        }
            //        else if ((sMobValue != null) && (sTelValue != null))
            //        {
            //            //Expression E1rE2 = Expression.OrElse(sMobnsMobBinExpr, sMob2nsMobBinExpr);
            //            //Expression E1rE2rE3 = Expression.OrElse(sMob3nsMobBinExpr, E1rE2);
            //            //Expression E1rE2rE3rE4 = Expression.OrElse(sMobnsTelBinExpr, E1rE2rE3);
            //            //Expression E1rE2rE3rE4rE5 = Expression.OrElse(sTelnsMobBinExpr, E1rE2rE3rE4);
            //            //Expression E1rE2rE3rE4rE5rE6 = Expression.OrElse(sTelnsTelBinExpr, E1rE2rE3rE4rE5);
            //            sLeadMatchWhere = MobExprs + " or " + TelExprs;
            //            //LeadID = dbcontext.Leads.Where(sLeadMatchWhere).Select(L => L.id).FirstOrDefault();
            //        }
            //        else if ((sMobValue != null) && (sEMailValue != null))
            //        {
            //            //Expression E1rE2 = Expression.OrElse(sMobnsMobBinExpr, sMob2nsMobBinExpr);
            //            //Expression E1rE2rE3 = Expression.OrElse(sMob3nsMobBinExpr, E1rE2);
            //            //Expression E1rE2rE3rE4 = Expression.OrElse(sEmailnsEmailBinExpr, E1rE2rE3);
            //            //Expression E1rE2rE3rE4rE5 = Expression.OrElse(sEmail2nsEmailBinExpr, E1rE2rE3rE4);
            //            //Expression E1rE2rE3rE4rE5rE6 = Expression.OrElse(sEmail3nsEmailBinExpr, E1rE2rE3rE4rE5);
            //            //Expression E1rE2rE3rE4rE5rE6rE7 = Expression.OrElse(sEmail4nsEmailBinExpr, E1rE2rE3rE4rE5rE6);
            //            //Expression E1rE2rE3rE4rE5rE6rE7rE8 = Expression.OrElse(sEmail5nsEmailBinExpr, E1rE2rE3rE4rE5rE6rE7);
            //            sLeadMatchWhere = MobExprs + " or " + EmailExprs;
            //            //LeadID = dbcontext.Leads.Where(sLeadMatchWhere).Select(L => L.id).FirstOrDefault();
            //        }
            //        else if ((sMobValue != null))
            //        {
            //            //Expression E1rE2 = Expression.OrElse(sMobnsMobBinExpr, sMob2nsMobBinExpr);
            //            //Expression E1rE2rE3 = Expression.OrElse(sMob3nsMobBinExpr, E1rE2);
            //            //Expression E1rE2rE3rE5 = Expression.OrElse(sTelnsMobBinExpr, E1rE2rE3);
            //            sLeadMatchWhere = MobExprs;
            //            // LeadID = dbcontext.Leads.Where(sLeadMatchWhere).Select(L => L.id).FirstOrDefault();
            //        }
            //        else if ((sTelValue != null))
            //        {
            //            //BinaryExpression sMob2nsTelBinExpr = Expression.Equal(sMob2Expr, sTelvalueExpr);
            //            //BinaryExpression sMob3nsTelBinExpr = Expression.Equal(sMob3Expr, sTelvalueExpr);
            //            //Expression E1rE2 = Expression.OrElse(sMobnsTelBinExpr, sMob2nsTelBinExpr);
            //            //Expression E1rE2rE3 = Expression.OrElse(sMob3nsTelBinExpr, E1rE2);
            //            //Expression E1rE2rE3rE4 = Expression.OrElse(sTelnsTelBinExpr, E1rE2rE3);
            //            sLeadMatchWhere = TelExprs;
            //            //LeadID = dbcontext.Leads.Where(sLeadMatchWhere).Select(L => L.id).FirstOrDefault();
            //        }
            //        else if (sEMailValue != null)
            //        {
            //            //Expression E1rE2 = Expression.OrElse(sEmailnsEmailBinExpr, sEmail2nsEmailBinExpr);
            //            //Expression E1rE2rE3 = Expression.OrElse(sEmail3nsEmailBinExpr, E1rE2);
            //            //Expression E1rE2rE3rE4 = Expression.OrElse(sEmail4nsEmailBinExpr, E1rE2rE3);
            //            //Expression E1rE2rE3rE4rE5 = Expression.OrElse(sEmail5nsEmailBinExpr, E1rE2rE3rE4);
            //            sLeadMatchWhere = EmailExprs;
            //            //LeadID = dbcontext.Leads.Where(sLeadMatchWhere).Select(L => L.id).FirstOrDefault();
            //        }
            //        else
            //        {
            //            LeadID = 0;
            //            bInsertLead = true;
            //            if (sEMailValue != null && sEMailValue.Length > 2)
            //            {
            //                if (sPostCodeValue != null)
            //                {
            //                    sLeadMatchWhere = NameExprs + " And " + PostCodeExprs;
            //                }
            //            }
            //            else
            //            {
            //                if (dDOBValue != null)
            //                {
            //                    if (Convert.ToInt32(dDOBValue.Value.Year.ToString()) > 1900)
            //                    {
            //                        sLeadMatchWhere = NameExprs + " And " + DOBExprs;
            //                    }
            //                }
            //            }
            //        }

            //        if (FKiLeadClassIDValue != null)
            //        {
            //            //Expression<Func<Datas, bool>> LeadClassID =
            //            //Expression.Lambda<Func<Datas, bool>>(FKiLeadClassIDBinExpr, parameterExpression);
            //            string body = null;
            //            if (sLeadMatchWhere != null)
            //            {
            //                //body = Expression.Lambda<Func<Datas, bool>>(
            //                //       Expression.AndAlso(
            //                //       ((LambdaExpression)LeadClassID).Body,
            //                //       ((LambdaExpression)sLeadMatchWhere).Body),
            //                //       parameterExpression);
            //                body = ClassExprs + " And " + sLeadMatchWhere;
            //            }
            //            else
            //            {
            //                body = ClassExprs;
            //            }
            //            string InstanceTable = "LeadInstance" + OrgID;
            //            string boundid = "Select ID from " + InstanceTable + " where " + body;
            //            //boundid = boundid.Replace("L =>", "").Replace("(L.", "(").Replace("==", "=").Replace(@"\", "").Replace(@"""", @"'").Replace("AndAlso", "And").Replace("OrElse", "OR");
            //            //LeadID = dbContext.Datas.Where(body).Select(L => L.id).FirstOrDefault();
            //            SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString());
            //            using (SqlCommand cmd = new SqlCommand("", Con))
            //            {
            //                cmd.CommandText = boundid;
            //                Con.Open();
            //                SqlDataReader reader = cmd.ExecuteReader();
            //                while (reader.Read())
            //                {
            //                    LeadID = reader.GetInt32(0);

            //                }
            //                reader.Close();
            //                Con.Close();
            //            }
            //            if (LeadID > 0)
            //            {
            //                bLoaded = true;
            //            }
            //            if (bLoaded)
            //            {
            //                bInsertLead = false;
            //            }
            //            else
            //            {
            //                bInsertLead = true;
            //            }
            //        }
            //        VMLeadEngine engine = new VMLeadEngine();
            //        engine.bInsertLead = bInsertLead;
            //        engine.LeadID = LeadID;
            //        return engine;
        }
        public VMLeadEngine GetEmailLeadDetails(List<string> Data, int OrgID, string database, int SubClassID)
        {
            DataContext dbcontext = new DataContext(database);
            var Configs = dbcontext.LeadConfigurations.Where(m => m.OrganizationID == OrgID && m.Class == SubClassID).FirstOrDefault();
            if (Configs.Settings == "Always Insert")
            {
                VMLeadEngine Eng = new VMLeadEngine();
                Eng.ClientID = 0;
                Eng.AlwaysInsert = true;
                Eng.InsertToLead = true;
                Eng.InsertToInstance = true;
                return Eng;
            }
            //DataContext db = new DataContext();
            ModelDbContext modeldb = new ModelDbContext();
            int LeadID = -1, FKiSourceID = 0, OldFKiSourceID = 0, ClientID = 0;
            //Poovanna 21/07/2017 take seperate string for date datatype as the query gives error when no quotes are included for date.
            string sMob = "", sTel = "", sEMail = "", sForeName = "", sLastName = "", sPostCode = "";
            string dDOB = "";
            //DateTime? dDateRenewal = new DateTime(1900, 04, 30);
            DateTime dImportedOn = new DateTime();
            int FKiLeadClassID = SubClassID;
            List<FormData> Values = new List<FormData>();
            string dDateRenewal = "";
            foreach (var data in Data)
            {
                if (data.Contains("<>"))
                {
                    var keyvalues = Regex.Split(data, "<>");
                    Values.Add(new FormData
                    {
                        Label = keyvalues[0].TrimStart().TrimEnd(),
                        Value = keyvalues[1].TrimStart().TrimEnd()
                    });
                }
            }
            for (int i = 0; i < Values.Count(); i++)
            {
                if (Values[i].Label.IndexOf("Mobile Number") >= 0)
                {
                    sMob = Values[i].Value;
                }
                if (Values[i].Label.IndexOf("Telephone Number") >= 0)
                {
                    sTel = Values[i].Value;
                }
                if (Values[i].Label == "Email")
                {
                    sEMail = Values[i].Value;
                }
                if (Values[i].Label == "First Name")
                {
                    sForeName = Values[i].Value;
                }
                if (Values[i].Label == "Last Name")
                {
                    sLastName = Values[i].Value;
                }
                if (Values[i].Label == "PostCode")
                {
                    sPostCode = Values[i].Value;
                }
                //if (Values[i].Label == "Date Of Birth")
                //Poovanna 18/07/2017 the name is DOB but used Date of birth
                if (Values[i].Label == "DOB")
                {
                    //dDOB = Convert.ToDateTime(Values[i].Value);
                    dDOB = DateTime.ParseExact(Values[i].Value, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture).ToString("yyyy'-'MM'-'dd");
                    // sDOB= Values[i].Value;
                }
                if (Values[i].Label == "FKiLeadClassID")
                {
                    FKiLeadClassID = SubClassID;
                }
                if (Values[i].Label == "FKiSourceID")
                {
                    FKiSourceID = Convert.ToInt32(Values[i].Value);
                }
                //Poovanna-17/07/2017 error date format
                if (Values[i].Label == "RenewalDate")
                {
                    dDateRenewal = DateTime.ParseExact(Values[i].Value, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture).ToString("yyyy'-'MM'-'dd");
                }
            }
            Expression<Func<Datas, bool>> sLeadMatchWhere = null;
            ParameterExpression parameterExpression = Expression.Parameter(typeof(Datas), "L");
            MemberExpression sMobExpr = Expression.PropertyOrField(parameterExpression, "sMob");
            MemberExpression sMob2Expr = Expression.PropertyOrField(parameterExpression, "sMob");
            MemberExpression sMob3Expr = Expression.PropertyOrField(parameterExpression, "sMob");
            MemberExpression sTelExpr = Expression.PropertyOrField(parameterExpression, "sTel");
            MemberExpression sEMailExpr = Expression.PropertyOrField(parameterExpression, "sEmail");
            MemberExpression sEMail2Expr = Expression.PropertyOrField(parameterExpression, "sEmail");
            MemberExpression sEMail3Expr = Expression.PropertyOrField(parameterExpression, "sEmail");
            MemberExpression sEMail4Expr = Expression.PropertyOrField(parameterExpression, "sEmail");
            MemberExpression sEMail5Expr = Expression.PropertyOrField(parameterExpression, "sEmail");
            MemberExpression sNameExpr = Expression.PropertyOrField(parameterExpression, "sName");
            MemberExpression sPostCodeExpr = Expression.PropertyOrField(parameterExpression, "sPostcode");
            MemberExpression dDOBExpr = Expression.PropertyOrField(parameterExpression, "dDOB");
            MemberExpression dDateRenewalExpr = Expression.PropertyOrField(parameterExpression, "dDateRenewal");
            MemberExpression FKiLeadClassIDExpr = Expression.PropertyOrField(parameterExpression, "FKiLeadClassID");

            ConstantExpression sMobvalueExpr = Expression.Constant(sMob);
            ConstantExpression sTelvalueExpr = Expression.Constant(sTel);
            ConstantExpression sEMailvalueExpr = Expression.Constant(sEMail);
            ConstantExpression sNamevalueExpr = Expression.Constant(sForeName + " " + sLastName);
            ConstantExpression sPostcodevalueExpr = Expression.Constant(sPostCode);
            ConstantExpression dDOBvalueExpr = Expression.Constant(dDOB);
            ConstantExpression dDateRenewalvalueExpr = Expression.Constant(dDateRenewal);
            ConstantExpression FKiLeadClassIDvalueExpr = Expression.Constant(FKiLeadClassID);

            BinaryExpression sMobnsMobBinExpr = Expression.Equal(sMobExpr, sMobvalueExpr);
            BinaryExpression sMob2nsMobBinExpr = Expression.Equal(sMob2Expr, sMobvalueExpr);
            BinaryExpression sMob3nsMobBinExpr = Expression.Equal(sMob3Expr, sMobvalueExpr);
            BinaryExpression sMobnsTelBinExpr = Expression.Equal(sMobExpr, sTelvalueExpr);
            BinaryExpression sTelnsMobBinExpr = Expression.Equal(sTelExpr, sMobvalueExpr);
            BinaryExpression sTelnsTelBinExpr = Expression.Equal(sTelExpr, sTelvalueExpr);
            BinaryExpression sEmailnsEmailBinExpr = Expression.Equal(sEMailExpr, sEMailvalueExpr);
            BinaryExpression sEmail2nsEmailBinExpr = Expression.Equal(sEMail2Expr, sEMailvalueExpr);
            BinaryExpression sEmail3nsEmailBinExpr = Expression.Equal(sEMail3Expr, sEMailvalueExpr);
            BinaryExpression sEmail4nsEmailBinExpr = Expression.Equal(sEMail4Expr, sEMailvalueExpr);
            BinaryExpression sEmail5nsEmailBinExpr = Expression.Equal(sEMail5Expr, sEMailvalueExpr);
            BinaryExpression sNamensNameBinExpr = Expression.Equal(sNameExpr, sNamevalueExpr);
            BinaryExpression sPostcodensPostcodeBinExpr = Expression.Equal(sPostCodeExpr, sPostcodevalueExpr);
            BinaryExpression dDOBndDOBBinExpr = Expression.Equal(dDOBExpr, dDOBvalueExpr);
            BinaryExpression dDateRenewalBinExpr = Expression.Equal(dDateRenewalExpr, dDateRenewalvalueExpr);
            BinaryExpression FKiLeadClassIDBinExpr = Expression.Equal(FKiLeadClassIDExpr, FKiLeadClassIDvalueExpr); Expression.Constant(FKiLeadClassID);

            if ((sMob != null && sMob.Length > 0) && (sTel != null && sTel.Length > 0) && (sEMail != null && sEMail.Length > 0))
            {
                Expression E1rE2 = Expression.OrElse(sMobnsMobBinExpr, sMob2nsMobBinExpr);
                Expression E1rE2rE3 = Expression.OrElse(sMob3nsMobBinExpr, E1rE2);
                Expression E1rE2rE3rE4 = Expression.OrElse(sMobnsTelBinExpr, E1rE2rE3);
                Expression E1rE2rE3rE4rE5 = Expression.OrElse(sTelnsMobBinExpr, E1rE2rE3rE4);
                Expression E1rE2rE3rE4rE5rE6 = Expression.OrElse(sTelnsTelBinExpr, E1rE2rE3rE4rE5);
                Expression E1rE2rE3rE4rE5rE6rE7 = Expression.OrElse(sEmailnsEmailBinExpr, E1rE2rE3rE4rE5rE6);
                Expression E1rE2rE3rE4rE5rE6rE7rE8 = Expression.OrElse(sEmail2nsEmailBinExpr, E1rE2rE3rE4rE5rE6rE7);
                Expression E1rE2rE3rE4rE5rE6rE7rE8rE9 = Expression.OrElse(sEmail3nsEmailBinExpr, E1rE2rE3rE4rE5rE6rE7rE8);
                Expression E1rE2rE3rE4rE5rE6rE7rE8rE9rE10 = Expression.OrElse(sEmail4nsEmailBinExpr, E1rE2rE3rE4rE5rE6rE7rE8rE9);
                Expression E1rE2rE3rE4rE5rE6rE7rE8rE9rE10rE11 = Expression.OrElse(sEmail5nsEmailBinExpr, E1rE2rE3rE4rE5rE6rE7rE8rE9rE10);
                sLeadMatchWhere = Expression.Lambda<Func<Datas, bool>>(E1rE2rE3rE4rE5rE6rE7rE8rE9rE10rE11, parameterExpression);
                // LeadID = dbcontext.Leads.Where(sLeadMatchWhere).Select(L => L.id).FirstOrDefault();
            }
            else if ((sMob != null && sMob.Length > 0) && (sTel != null && sTel.Length > 0))
            {
                Expression E1rE2 = Expression.OrElse(sMobnsMobBinExpr, sMob2nsMobBinExpr);
                Expression E1rE2rE3 = Expression.OrElse(sMob3nsMobBinExpr, E1rE2);
                Expression E1rE2rE3rE4 = Expression.OrElse(sMobnsTelBinExpr, E1rE2rE3);
                Expression E1rE2rE3rE4rE5 = Expression.OrElse(sTelnsMobBinExpr, E1rE2rE3rE4);
                Expression E1rE2rE3rE4rE5rE6 = Expression.OrElse(sTelnsTelBinExpr, E1rE2rE3rE4rE5);
                sLeadMatchWhere = Expression.Lambda<Func<Datas, bool>>(E1rE2rE3rE4rE5rE6, parameterExpression);
                //LeadID = dbcontext.Leads.Where(sLeadMatchWhere).Select(L => L.id).FirstOrDefault();
            }
            else if ((sMob != null && sMob.Length > 0) && (sEMail != null && sEMail.Length > 0))
            {
                Expression E1rE2 = Expression.OrElse(sMobnsMobBinExpr, sMob2nsMobBinExpr);
                Expression E1rE2rE3 = Expression.OrElse(sMob3nsMobBinExpr, E1rE2);
                Expression E1rE2rE3rE4 = Expression.OrElse(sEmailnsEmailBinExpr, E1rE2rE3);
                Expression E1rE2rE3rE4rE5 = Expression.OrElse(sEmail2nsEmailBinExpr, E1rE2rE3rE4);
                Expression E1rE2rE3rE4rE5rE6 = Expression.OrElse(sEmail3nsEmailBinExpr, E1rE2rE3rE4rE5);
                Expression E1rE2rE3rE4rE5rE6rE7 = Expression.OrElse(sEmail4nsEmailBinExpr, E1rE2rE3rE4rE5rE6);
                Expression E1rE2rE3rE4rE5rE6rE7rE8 = Expression.OrElse(sEmail5nsEmailBinExpr, E1rE2rE3rE4rE5rE6rE7);
                sLeadMatchWhere = Expression.Lambda<Func<Datas, bool>>(E1rE2rE3rE4rE5rE6rE7rE8, parameterExpression);
                //LeadID = dbcontext.Leads.Where(sLeadMatchWhere).Select(L => L.id).FirstOrDefault();
            }
            else if ((sMob != null && sMob.Length > 0))
            {
                Expression E1rE2 = Expression.OrElse(sMobnsMobBinExpr, sMob2nsMobBinExpr);
                Expression E1rE2rE3 = Expression.OrElse(sMob3nsMobBinExpr, E1rE2);
                Expression E1rE2rE3rE5 = Expression.OrElse(sTelnsMobBinExpr, E1rE2rE3);
                sLeadMatchWhere = Expression.Lambda<Func<Datas, bool>>(E1rE2rE3rE5, parameterExpression);
                // LeadID = dbcontext.Leads.Where(sLeadMatchWhere).Select(L => L.id).FirstOrDefault();
            }
            else if ((sTel != null && sTel.Length > 0))
            {
                BinaryExpression sMob2nsTelBinExpr = Expression.Equal(sMob2Expr, sTelvalueExpr);
                BinaryExpression sMob3nsTelBinExpr = Expression.Equal(sMob3Expr, sTelvalueExpr);
                Expression E1rE2 = Expression.OrElse(sMobnsTelBinExpr, sMob2nsTelBinExpr);
                Expression E1rE2rE3 = Expression.OrElse(sMob3nsTelBinExpr, E1rE2);
                Expression E1rE2rE3rE4 = Expression.OrElse(sTelnsTelBinExpr, E1rE2rE3);
                sLeadMatchWhere = Expression.Lambda<Func<Datas, bool>>(E1rE2rE3rE4, parameterExpression);
                //LeadID = dbcontext.Leads.Where(sLeadMatchWhere).Select(L => L.id).FirstOrDefault();
            }
            else if (sEMail != null && sEMail.Length > 0)
            {
                Expression E1rE2 = Expression.OrElse(sEmailnsEmailBinExpr, sEmail2nsEmailBinExpr);
                Expression E1rE2rE3 = Expression.OrElse(sEmail3nsEmailBinExpr, E1rE2);
                Expression E1rE2rE3rE4 = Expression.OrElse(sEmail4nsEmailBinExpr, E1rE2rE3);
                Expression E1rE2rE3rE4rE5 = Expression.OrElse(sEmail5nsEmailBinExpr, E1rE2rE3rE4);
                sLeadMatchWhere = Expression.Lambda<Func<Datas, bool>>(E1rE2rE3rE4rE5, parameterExpression);
                //LeadID = dbcontext.Leads.Where(sLeadMatchWhere).Select(L => L.id).FirstOrDefault();
            }
            else
            {
                LeadID = 0;
                if (sEMail != null && sEMail.Length > 2)
                {
                    if (sPostCode != null)
                    {
                        Expression E1rE2 = Expression.AndAlso(sNamensNameBinExpr, sPostcodensPostcodeBinExpr);
                        sLeadMatchWhere = Expression.Lambda<Func<Datas, bool>>(E1rE2, parameterExpression);
                    }
                }
                else
                {
                    if (dDOB != null && dDOB.Length > 0)
                    {
                        Expression E1rE2 = Expression.AndAlso(sNamensNameBinExpr, dDOBndDOBBinExpr);
                        sLeadMatchWhere = Expression.Lambda<Func<Datas, bool>>(E1rE2, parameterExpression);
                    }
                }
            }
            var Table = EnumLeadTables.Leads.ToString();
            string ClientsTable = EnumLeadTables.LeadClients.ToString();
            Expression<Func<Datas, bool>> body = null;
            string Query = "";

            if (Configs.Settings == "Insert by class and renewal date")
            {
                Expression<Func<Datas, bool>> LeadClassID =
                        Expression.Lambda<Func<Datas, bool>>(FKiLeadClassIDBinExpr, parameterExpression);
                Expression<Func<Datas, bool>> RenewalDate =
                Expression.Lambda<Func<Datas, bool>>(dDateRenewalBinExpr, parameterExpression);

                if (sLeadMatchWhere != null)
                {
                    body = Expression.Lambda<Func<Datas, bool>>(
                           Expression.AndAlso(
                           ((LambdaExpression)LeadClassID).Body,
                           Expression.AndAlso(
                           ((LambdaExpression)RenewalDate).Body,
                           ((LambdaExpression)sLeadMatchWhere).Body)), parameterExpression);
                }
                else
                {
                    body = Expression.Lambda<Func<Datas, bool>>(FKiLeadClassIDBinExpr, parameterExpression);
                }
                Query = "Select ID from " + Table + " where " + body;
                Query = Query.Replace("L =>", "").Replace("(L.", "(").Replace("==", "=").Replace(@"\", "").Replace(@"""", @"'").Replace("AndAlso", "And").Replace("OrElse", "OR").Replace("dDOB = ", "dDOB = '").Replace(" AM)", " AM')").Replace(" PM)", " PM')");
            }
            else if (Configs.Settings == "Insert by class")
            {
                Expression<Func<Datas, bool>> LeadClassID =
                Expression.Lambda<Func<Datas, bool>>(FKiLeadClassIDBinExpr, parameterExpression);
                if (sLeadMatchWhere != null)
                {
                    body = Expression.Lambda<Func<Datas, bool>>(
                           Expression.AndAlso(
                           ((LambdaExpression)LeadClassID).Body,
                           ((LambdaExpression)sLeadMatchWhere).Body),
                           parameterExpression);
                }
                else
                {
                    body = Expression.Lambda<Func<Datas, bool>>(FKiLeadClassIDBinExpr, parameterExpression);
                }
                Query = "Select ID from " + Table + " where " + body;
                Query = Query.Replace("L =>", "").Replace("(L.", "(").Replace("==", "=").Replace(@"\", "").Replace(@"""", @"'").Replace("AndAlso", "And").Replace("OrElse", "OR").Replace("dDOB=", "dDOB='").Replace(" AM)", " AM')").Replace(" PM)", " PM')");
            }
            else
            {
                if (sLeadMatchWhere != null)
                {
                    body = sLeadMatchWhere;
                }
                Query = "Select ID from " + Table + " where " + body;
                Query = Query.Replace("L =>", "").Replace("(L.", "(").Replace("==", "=").Replace(@"\", "").Replace(@"""", @"'").Replace("AndAlso", "And").Replace("OrElse", "OR");
            }

            if (Query.Length > 0)
            {
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    cmd.CommandText = Query;
                    Con.Open();
                    Con.ChangeDatabase(database);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        LeadID = reader.GetInt32(0);
                    }
                    reader.Close();
                    if (LeadID > 0)
                    {
                        string ImportedOn = "Select dImportedOn, FKiSourceID from " + Table + " where ID = " + LeadID;
                        cmd.CommandText = ImportedOn;
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            dImportedOn = Convert.ToDateTime(reader.GetDateTime(0));
                            OldFKiSourceID = reader.GetInt32(1);
                        }
                    }
                    else
                    {
                        OldFKiSourceID = FKiSourceID;
                    }
                    reader.Close();
                    string ClientsQuery = "Select ID from " + ClientsTable + " where Name='" + sForeName + " " + sLastName + "' And ClassID = " + FKiLeadClassID;
                    cmd.CommandText = ClientsQuery;
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ClientID = reader.GetInt32(0);
                    }
                    Con.Close();
                }
            }
            VMLeadEngine engine = new VMLeadEngine();
            engine.LeadID = LeadID;
            engine.ClientID = ClientID;
            if (Configs.Settings == "Always Insert")
            {
                engine.AlwaysInsert = true;
                engine.InsertToLead = true;
                engine.InsertToInstance = true;
            }
            else if (Configs.Settings == "Insert if with x days")
            {
                if (LeadID > 0)
                {
                    int difference = Convert.ToInt32((DateTime.Now.Date - dImportedOn.Date).TotalDays);
                    if (difference > Configs.Interval)
                    {
                        engine.bUpdateSource = true;
                        engine.InsertToLead = true;
                        engine.InsertToInstance = true;
                    }
                    else
                    {
                        engine.bUpdateSource = false;
                        engine.InsertToLead = false;
                        engine.InsertToInstance = true;
                    }
                }
                else
                {
                    engine.bUpdateSource = false;
                    engine.InsertToLead = true;
                    engine.InsertToInstance = true;
                }
            }
            else if (Configs.Settings == "Insert by class and renewal date")
            {
                engine.InsertWithClassAndRenewalDate = true;
                if (LeadID > 0)
                {
                    engine.InsertToLead = false;
                    engine.InsertToInstance = true;
                }
                else
                {
                    engine.InsertToLead = true;
                    engine.InsertToInstance = true;
                }
            }
            else if (Configs.Settings == "Insert by class")
            {
                engine.InsertByClass = true;
                if (LeadID > 0)
                {
                    engine.InsertToLead = false;
                    engine.InsertToInstance = true;
                }
                else
                {
                    engine.InsertToLead = true;
                    engine.InsertToInstance = true;
                }
            }
            else
            {
                engine.InsertByClass = false;
                engine.InsertToLead = false;
                engine.InsertToInstance = false;
            }
            engine.SourceID = FKiSourceID;
            engine.OldSourceID = OldFKiSourceID;
            engine.ClassID = FKiLeadClassID;
            return engine;
        }
    }
}
