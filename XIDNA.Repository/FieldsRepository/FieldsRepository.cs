using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using System.Data.SqlClient;
using System.Configuration;
using XIDNA.ViewModels;

namespace XIDNA.Repository
{
    public class FieldsRepository : IFieldsRepository
    {
        CommonRepository Common = new CommonRepository();
        public int SaveField(AddFields model, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            if (model.ID == 0)
            {
                var dbs = dbCore.XIAppUsers.Select(m => m.sDatabaseName).Distinct().ToList();
                string LeadQuery = "";
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    string OrgName = GetOrganization(model.OrganizationID, sDatabase);
                    model.TableName = EnumLeadTables.Leads.ToString();
                    int BOID = dbContext.BOs.Where(m => m.Name == model.TableName).Select(m => m.BOID).FirstOrDefault();
                    if (model.FieldType == "VARCHAR")
                    {
                        if (model.OrganizationID >= 0)
                        {
                            AddOrganizationField(model, sDatabase, iUserID, sOrgName);
                        }
                        else
                        {
                            Con.Open();
                            List<string> Tables = new List<string>();
                            Tables.Add(model.TableName);
                            //Tables.Add("LeadInstances");
                            foreach (var table in Tables)
                            {
                                foreach (var dbname in dbs)
                                {
                                    Con.ChangeDatabase(dbname);
                                    LeadQuery = "ALTER TABLE " + table + " ADD " + model.FieldName + " " + model.FieldType + "(" + model.Length + ")";
                                    cmd.CommandText = LeadQuery;
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            Con.Close();
                        }
                    }
                    else
                    {
                        if (model.OrganizationID >= 0)
                        {
                            AddOrganizationField(model, sDatabase, iUserID, sOrgName);
                        }
                        else
                        {
                            List<string> Tables = new List<string>();
                            //Tables.Add("Leads");
                            Tables.Add(model.TableName);
                            //Tables.Add("LeadInstances");
                            Con.Open();
                            foreach (var table in Tables)
                            {
                                foreach (var dbname in dbs)
                                {
                                    Con.ChangeDatabase(dbname);
                                    if (model.FieldType == "DATETIME")
                                    {
                                        LeadQuery = "ALTER TABLE " + table + " ADD " + model.FieldName + " " + model.FieldType + " DEFAULT '1900-01-01 12:00:00.000'";
                                    }
                                    else
                                    {
                                        LeadQuery = "ALTER TABLE " + table + " ADD " + model.FieldName + " " + model.FieldType + " DEFAULT 0";
                                    }
                                    cmd.CommandText = LeadQuery;
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            Con.Close();
                        }
                    }
                }
                if (model.OrganizationID >= 0)
                {

                }
                else
                {
                    int BID = dbContext.BOs.Where(m => m.Name == EnumLeadTables.Leads.ToString()).Select(m => m.BOID).FirstOrDefault();
                    AddBOField(model, BID, sDatabase, model.OrganizationID);
                }
                return model.ClassID;
            }
            else
            {
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    Con.Open();
                    Con.ChangeDatabase(sOrgDB);
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    string LeadQuery = "";
                    if (model.OrganizationID == 0)
                    {
                        MasterTemplates Master = dbContext.MasterTemplates.Find(model.ID);
                        Master.FieldLength = model.Length;
                        Master.FieldType = model.FieldType;
                        Master.DataFieldName = model.FieldName;
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        string ColumnName = model.Name.Substring(0, 4);
                        SqlConnection MyCon = new SqlConnection(ServiceUtil.GetClientConnectionString());
                        MyCon.Open();
                        SqlCommand cmmd = new SqlCommand();
                        cmmd.Connection = MyCon;
                        cmmd.CommandText = "SELECT name AS ConstraintName FROM sys.default_constraints Where OBJECT_NAME(parent_object_id)= 'Leads' AND name like '%" + ColumnName + "%' ORDER BY ConstraintName";
                        string constraint = "";
                        try
                        {
                            SqlDataReader data = cmmd.ExecuteReader();
                            while (data.Read())
                            {
                                constraint = data.GetString(0);
                            }
                            MyCon.Close();
                            string ConstraintQuery = "ALTER TABLE " + EnumLeadTables.Leads.ToString() + " DROP CONSTRAINT " + constraint;
                            cmd.CommandText = ConstraintQuery;
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            if (model.FieldType == "VARCHAR")
                            {
                                LeadQuery = "ALTER TABLE " + EnumLeadTables.Leads.ToString() + " ALTER COLUMN " + model.Name + " " + model.FieldType + "(" + model.Length + ")";
                            }
                            else
                            {
                                LeadQuery = "ALTER TABLE " + EnumLeadTables.Leads.ToString() + " ALTER COLUMN " + model.Name + " " + model.FieldType;
                            }
                            cmd.CommandText = LeadQuery;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                List<BOFields> field = new List<BOFields>();
                if (model.OrganizationID != 0)
                {
                    field = dbContext.BOFields.Where(m => m.Name == model.Name).Where(m => m.OrganizationID == model.OrganizationID).ToList();
                }
                else
                {
                    field = dbContext.BOFields.Where(m => m.Name == model.Name).ToList();
                }
                //foreach (var items in field)
                //{
                //    var bofield = dbContext.BOFields.Find(items.ID);
                //    var values = Enum.GetValues(typeof(Datatypes));
                //    string s = model.FieldType;
                //    int key = 0;
                //    key = (int)Enum.Parse(typeof(Datatypes), s);
                //    bofield.TypeID = Convert.ToInt32(key);
                //    bofield.MaxLength = model.Length;
                //    dbContext.SaveChanges();
                //}
                return model.ClassID;
            }
        }

        public VMCustomResponse SaveOrgField(MappedFields model, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            MappedFields Fields = new MappedFields();
            Fields.BOID = model.BOID;
            Fields.ClassID = model.ClassID;
            Fields.OrganizationID = model.OrganizationID;
            Fields.FieldName = model.FieldName;
            Fields.FieldType = model.FieldType;
            Fields.Length = model.Length;
            Fields.Description = model.Description;
            if (model.FieldType == "VARCHAR")
            {
                Fields.Length = model.Length;
            }
            else
            {
                Fields.Length = "0";
            }
            Spdb.MappedFields.Add(Fields);
            Spdb.SaveChanges();
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = Fields.ID, Status = true };
        }

        private void AddOrganizationField(AddFields model, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            if (model.OrganizationID > 0)
            {
                DataContext Spdb = new DataContext(sOrgDB);
                MappedFields Fields = new MappedFields();
                Fields.ClassID = model.ClassID;
                Fields.OrganizationID = model.OrganizationID;
                Fields.FieldName = model.FieldName;
                Fields.FieldType = model.FieldType;
                if (model.FieldType == "VARCHAR")
                {
                    Fields.Length = model.Length;
                }
                else
                {
                    Fields.Length = "0";
                }
                Spdb.MappedFields.Add(Fields);
                Spdb.SaveChanges();
            }
            else
            {
                MasterTemplates Master = new MasterTemplates();
                Master.ClassID = model.ClassID;
                Master.DataFieldName = model.FieldName;
                Master.FieldType = model.FieldType;
                Master.FieldLength = model.Length;
                dbContext.MasterTemplates.Add(Master);
                dbContext.SaveChanges();
            }

        }

        public VMCustomResponse SaveOrgEditedField(MappedFields model, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            MappedFields Fields = Spdb.MappedFields.Find(model.ID);
            Fields.FieldName = model.FieldName;
            Fields.FieldType = model.FieldType;
            Fields.Description = model.Description;
            if (model.FieldType == "VARCHAR")
            {
                Fields.Length = model.Length;
            }
            else
            {
                Fields.Length = "0";
            }
            //dbContext.MappedFields.Add(Fields);
            Spdb.SaveChanges();
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = Fields.ID, Status = true };
        }

        public int AddBOField(AddFields model, int BOID, string sDatabase, int OrgID)
        {
            ModelDbContext dbContext = new ModelDbContext();
            BOFields field = new BOFields();
            field.BOID = BOID;
            field.FieldCreatedID = OrgID;
            field.OrganizationID = model.OrganizationID;
            field.FieldClassID = model.ClassID;
            field.Name = model.FieldName;
            field.LabelName = model.FieldName;
            field.FieldClass = model.ClassName;
            var values = Enum.GetValues(typeof(BODatatypes));
            string s = model.FieldType;
            int key = 0;
            key = (int)Enum.Parse(typeof(BODatatypes), s);
            field.TypeID = Convert.ToInt32(key);
            if (model.FieldType == "VARCHAR")
            {
                field.MaxLength = model.Length;
            }
            else
            {
                field.MaxLength = "0";
            }
            //field.IsVisible = field.IsWhere = false;
            //field.IsTotal = false;
            //field.IsRunTime = true;
            //field.IsGroupBy = field.IsOrderBy = false;
            field.IsDBValue = field.IsExpression = field.IsDate = false;
            //field.DBQuery = field.Expression = field.ExpreValue = null;
            field.DateExpression = field.DateValue = null;
            field.Description = model.FieldName + " Description";
            field.StatusTypeID = 10;
            field.CreatedByID = 1;
            field.CreatedBySYSID = "1";
            field.CreatedByName = "Admin";
            field.CreatedTime = DateTime.Now;
            dbContext.BOFields.Add(field);
            dbContext.SaveChanges();
            BOs BOsCount = dbContext.BOs.Find(field.BOID);
            int count = BOsCount.FieldCount;
            BOsCount.FieldCount = count + 1;
            dbContext.SaveChanges();
            return 0;
        }

        public DTResponse GetFieldsList(jQueryDataTableParamModel param, string Type, string sDatabase, int OrgID, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext SPdb = new DataContext(sOrgDB);
            IQueryable<MasterTemplates> AllFields;
            if (Type == "ClassSpecific")
            {
                AllFields = dbContext.MasterTemplates.Where(m => m.ClassID != 0 && m.DataFieldName != null);
            }
            else
            {
                AllFields = dbContext.MasterTemplates.Where(m => m.ClassID == 0);
            }
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllFields = AllFields.Where(m => m.Name.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllFields.Count();
            AllFields = QuerableUtil.GetResultsForDataTables(AllFields, "", sortExpression, param);
            var clients = AllFields.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            if (Type == "ClassSpecific")
            {
                result = (from cd in clients
                          join c in dbContext.Types on cd.ClassID equals c.ID
                          select new[] {
                             (i++).ToString(), Convert.ToString(cd.ID), c.Expression, cd.DataFieldName, cd.FieldType.ToString(), ""  }).ToList();
            }
            else
            {
                result = (from cd in clients
                          join c in dbContext.Types on cd.ClassID equals c.ID
                          select new[] {
                             (i++).ToString(), Convert.ToString(cd.ID), c.Expression, cd.DataFieldName, cd.FieldType.ToString(), ""  }).ToList();
            }
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public DTResponse GetOrgNonClassFieldsList(jQueryDataTableParamModel param, string Type, string sDatabase, int OrgID, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext SPdb = new DataContext(sOrgDB);
            IQueryable<MappedFields> AllFields;
            AllFields = SPdb.MappedFields.Where(m => m.ClassID == 0);
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllFields = AllFields.Where(m => m.FieldName.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllFields.Count();
            AllFields = QuerableUtil.GetResultsForDataTables(AllFields, "", sortExpression, param);
            var clients = AllFields.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(), Convert.ToString(c.ID), c.FieldName, c.FieldType, c.Length, ""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        private string Datatype(int p)
        {
            string type = ((BODatatypes)p).ToString();
            return type;
        }

        public DTResponse GetOrgClassFields(jQueryDataTableParamModel param, string Type, int ClassID, string sDatabase, int OrgID, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext SPdb = new DataContext(sOrgDB);
            IQueryable<MappedFields> AllFields;
            AllFields = SPdb.MappedFields.Where(m => m.ClassID != 0);
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllFields = AllFields.Where(m => m.FieldName.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllFields.Count();
            AllFields = QuerableUtil.GetResultsForDataTables(AllFields, "", sortExpression, param);
            var clients = AllFields.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(), Convert.ToString(c.ID), GetBO(c.BOID,sDatabase), GetClass(c.ClassID, sDatabase, OrgID), c.FieldName, c.FieldType, c.Length, ""  };
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        private string GetBO(int BOID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            return dbContext.BOs.Where(m => m.BOID == BOID).Select(m => m.Name).FirstOrDefault();
        }

        //public DTResponse GetOrgClassFields(jQueryDataTableParamModel param, string Type, int ClassID, string sDatabase, int orgid)
        //{
        //    DataContext SPdb = new DataContext(database);
        //    IQueryable<VMMappedFields> AllFields;
        //    //AllFields = SPdb.MappedFields.Where(m => m.OrganizationID == orgid && m.ClassID != 0);
        //    AllFields = (from c in SPdb.MappedFields.Where(m => m.OrganizationID == orgid && m.ClassID != 0)
        //                 join d in SPdb.OrganizationClasses on c.ClassID equals d.ClassID
        //                 select new VMMappedFields
        //                 {
        //                     ID = c.ID,
        //                     ClassID = c.ClassID,
        //                     Class = d.Class,
        //                     FieldName = c.FieldName,
        //                     FieldType = c.FieldType,
        //                     Length = c.Length
        //                 }).OrderBy(m => m.ClassID);
        //    //string sortExpression = ServiceConstants.SortExpression;
        //    string sortExpression = "ClassID";
        //    if (!string.IsNullOrWhiteSpace(param.sSearch))
        //    {
        //        List<int> total = SPdb.OrganizationClasses.Where(m => m.Class.Contains(param.sSearch)).Select(m => m.ClassID).ToList();
        //        if (total != null)
        //        {
        //            AllFields = AllFields.Where(m => total.Contains(m.ClassID));
        //        }
        //    }
        //    int displyCount = 0;
        //    displyCount = AllFields.Count();
        //    AllFields = QuerableUtil.GetResultsForDataTables(AllFields, "", sortExpression, param);
        //    int i = param.iDisplayStart + 1;
        //    var result = from c in AllFields.ToList()
        //                 select new[] {
        //                          (i++).ToString(), Convert.ToString(c.ID), c.Class,c.FieldName,c.FieldType,c.Length,""};

        //    return new DTResponse()
        //    {
        //        sEcho = param.sEcho,
        //        iTotalRecords = displyCount,
        //        iTotalDisplayRecords = displyCount,
        //        aaData = result
        //    };
        //}

        private string GetSoruceName(string SubID, string sDatabase, int iUserID, string sOrgName)
        {
            if (SubID != null)
            {
                ModelDbContext dbContext = new ModelDbContext();
                var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
                DataContext Spdb = new DataContext(sOrgDB);
                var Source = Spdb.OrganizationSubscriptions.Where(m => m.SubscriptionID == SubID).FirstOrDefault();
                string SourceName = Spdb.OrganizationSources.Where(m => m.ID == Source.SourceID).Select(m => m.Name).FirstOrDefault();
                string Class = Spdb.OrganizationClasses.Where(m => m.ClassID == Source.ClassID).Select(m => m.Class).FirstOrDefault();
                var Names = SourceName + "-" + Class;
                return Names;
            }
            else
            {
                return null;
            }

        }

        private string GetClass(int ClassID, string sDatabase, int OrgID)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var Classes = dbContext.BOClassAttributes.Where(m => m.ID == ClassID).Select(m => m.Class).FirstOrDefault();
            return Classes;
        }

        public AddFields EditField(int ColumnID, int OrgID, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            MasterTemplates bofield = dbContext.MasterTemplates.Find(ColumnID);
            AddFields model = new AddFields();
            model.ID = ColumnID;
            model.Class = bofield.ClassID;
            model.FieldName = bofield.DataFieldName;
            //int TypeID = bofield.TypeID;
            //Datatypes DataType = (Datatypes)TypeID;
            //string type = DataType.ToString();
            model.FieldType = bofield.FieldType;
            model.Length = bofield.FieldLength;
            //BOs boname = new BOs();
            //boname = dbContext.BOs.Find(bofield.BOID);
            //model.TableName = boname.Name;
            //model.BOID = boname.BOID;
            model.OldFieldName = bofield.Name;
            model.Classes = GetOrgClasses(OrgID, sDatabase, iUserID, sOrgName);
            //model.ClassName = bofield.FieldClass;
            model.ClassID = bofield.ClassID;
            if (bofield.ClassID != 0)
            {
                model.Type = "ClassSpecific";
            }
            else
            {
                model.Type = "ClassNonSpecific";
            }
            return model;
        }

        public MappedFields EditOrgField(int ColumnID, int OrgID, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            MappedFields Field = Spdb.MappedFields.Find(ColumnID);
            var Classes = Spdb.OrganizationClasses.Where(m => m.OrganizationID == OrgID).ToList();
            List<Classes> AllClasses = new List<Classes>();
            if (Classes.Count() > 0)
            {
                foreach (var items in Classes)
                {
                    AllClasses.Add(new Classes
                    {
                        text = items.Class,
                        value = items.ClassID
                    });
                }
            }
            else
            {
                AllClasses.Add(new Classes { });
            }

            Field.Classes = AllClasses;
            return Field;
        }

        public List<Classes> GetClasses(int orgid, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            List<Classes> Classes = new List<Classes>();
            List<OrganizationClasses> orgclasses = new List<OrganizationClasses>();
            if (orgid == 0)
            {
                var classnames = dbContext.Types.Where(m => m.Name == "Class Type").ToList();
                foreach (var items in classnames)
                {
                    Classes.Add(new Classes
                    {
                        text = items.Expression,
                        value = items.ID
                    });
                }
            }
            else
            {
                orgclasses = Spdb.OrganizationClasses.Where(m => m.OrganizationID == orgid).ToList();
                foreach (var items in orgclasses)
                {
                    Classes.Add(new Classes
                    {
                        text = items.Class,
                        value = items.ClassID
                    });
                }
            }
            return Classes;
        }

        public List<VMDropDown> GetSubscriptions(int OrgID, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            List<VMDropDown> Classes = new List<VMDropDown>();
            List<OrganizationSubscriptions> orgclasses = new List<OrganizationSubscriptions>();
            if (OrgID == 0)
            {
                var classnames = dbContext.Types.Where(m => m.Name == "Class Type").ToList();
                foreach (var items in classnames)
                {
                    Classes.Add(new VMDropDown
                    {
                        text = items.Expression,
                        Value = items.ID
                    });
                }
            }
            else
            {
                orgclasses = Spdb.OrganizationSubscriptions.Where(m => m.OrganizationID == OrgID && m.StatusTypeID == 10).ToList();
                foreach (var items in orgclasses)
                {
                    string SourceName = Spdb.OrganizationSources.Where(m => m.ID == items.SourceID).Select(m => m.Name).FirstOrDefault();
                    string ClassName = Spdb.OrganizationClasses.Where(m => m.ClassID == items.ClassID).Select(m => m.Class).FirstOrDefault();
                    Classes.Add(new VMDropDown
                    {
                        text = SourceName + "-" + ClassName,
                        Expression = items.SubscriptionID
                    });
                }
            }
            return Classes;
        }

        public List<Classes> GetOrgClasses(int OrgID, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            List<Classes> Classes = new List<Classes>();
            List<OrganizationClasses> orgclasses = new List<OrganizationClasses>();
            if (OrgID == 0)
            {
                var classnames = dbContext.Types.Where(m => m.Name == "Class Type").ToList();
                foreach (var items in classnames)
                {
                    Classes.Add(new Classes
                    {
                        text = items.Expression,
                        value = items.ID
                    });
                }
            }
            else
            {
                orgclasses = Spdb.OrganizationClasses.Where(m => m.OrganizationID == OrgID).ToList();
                foreach (var items in orgclasses)
                {
                    Classes.Add(new Classes
                    {
                        text = items.Class,
                        value = items.ClassID
                    });
                }
            }
            return Classes;
        }

        public List<VMDropDown> GetAllOrgClasses(int orgid, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            List<VMDropDown> Classes = new List<VMDropDown>();
            List<OrganizationClasses> orgclasses = new List<OrganizationClasses>();
            Classes = ServiceUtil.GetOrgClasses(orgid, sDatabase);
            return Classes;
        }

        public List<VMDropDown> GetAllBOClasses(int OrgID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> AllBOClasses = new List<VMDropDown>();
            AllBOClasses = (from c in dbContext.BOClassAttributes.ToList()
                            select new VMDropDown
                            {
                                Value = c.ID,
                                text = c.Class
                            }).ToList();
            return AllBOClasses;
        }

        public string GetOrganization(int OrgID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            string OrgName = dbCore.Organization.Where(m => m.ID == OrgID).Select(m => m.Name).FirstOrDefault();
            return OrgName;
        }
        public bool IsExistsFieldName(string FieldName, string sDatabase, int ID, int ClassID, int OrgID, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            BOFields model = new BOFields();
            model = dbContext.BOFields.Where(m => m.Name.Equals(FieldName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            MappedFields Field = Spdb.MappedFields.Where(m => m.FieldName.Equals(FieldName, StringComparison.CurrentCultureIgnoreCase) && m.OrganizationID == OrgID && m.ClassID == ClassID).FirstOrDefault();
            if (OrgID == 0)
            {
                if (ID == 0)
                {
                    if (model != null && model.BOID != 0)
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
                    if (model != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (ID == 0)
                {
                    if (Field != null)
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
                    if (Field != null)
                    {
                        if (ID == Field.ID)
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

        }
        public DTResponse GetSelectedFields(jQueryDataTableParamModel param, int ID, string sDatabase, int OrgID)
        {
            ModelDbContext dbContext = new ModelDbContext();
            IEnumerable<MasterTemplates> AllFields, FilteredFields;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredFields = dbContext.MasterTemplates.Where(m => m.DataFieldName.Contains(param.sSearch.ToUpper()) && m.DataFieldName != null).ToList();
                AllFields = FilteredFields.OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = FilteredFields.Count();
            }
            else
            {
                displyCount = dbContext.MasterTemplates.Where(m => m.ClassID == ID && m.DataFieldName != null).OrderByDescending(m => m.ID).Count();
                AllFields = dbContext.MasterTemplates.Where(m => m.ClassID == ID && m.DataFieldName != null).OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

            }
            var result = from c in AllFields
                         join s in dbContext.Types on c.ClassID equals s.ID
                         select new[] {
                             (i++).ToString(), Convert.ToString(c.ID), s.Expression, c.DataFieldName, c.FieldLength};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public DTResponse GetSelectedOrgFields(jQueryDataTableParamModel param, int ID, string sDatabase, int OrgID, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            IEnumerable<MappedFields> AllFields, FilteredFields;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredFields = Spdb.MappedFields.Where(m => m.ClassID == ID).Where(m => m.FieldName.Contains(param.sSearch.ToUpper())).ToList();
                AllFields = FilteredFields.OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = FilteredFields.Count();
            }
            else
            {
                displyCount = Spdb.MappedFields.Where(m => m.ClassID == ID).Where(m => m.OrganizationID == OrgID).OrderByDescending(m => m.ID).Count();
                AllFields = Spdb.MappedFields.Where(m => m.ClassID == ID).Where(m => m.OrganizationID == OrgID).OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

            }
            var result = from c in AllFields
                         select new[] {
                             (i++).ToString(), Convert.ToString(c.ID), c.FieldName, c.FieldType, Convert.ToString(c.Length)};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public DTResponse GetNonSelectedField(jQueryDataTableParamModel param, string sDatabase, int OrgID)
        {
            ModelDbContext dbContext = new ModelDbContext();
            IEnumerable<BOFields> AllFields, FilteredFields;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredFields = dbContext.BOFields.Where(m => m.Name.Contains(param.sSearch.ToUpper())).ToList();
                AllFields = FilteredFields.OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = FilteredFields.Count();
            }
            else
            {
                displyCount = dbContext.BOFields.Where(m => m.OrganizationID == OrgID && m.FieldClass == null).OrderByDescending(m => m.ID).Count();
                AllFields = dbContext.BOFields.Where(m => m.OrganizationID == OrgID && m.FieldClass == null).OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

            }
            var result = from c in AllFields
                         select new[] {
                             (i++).ToString(), Convert.ToString(c.ID), c.Name, Datatype(c.TypeID), Convert.ToString(c.MaxLength)};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public DTResponse GetOrgNonSelectedField(jQueryDataTableParamModel param, string sDatabase, int OrgID, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            IEnumerable<MappedFields> AllFields, FilteredFields;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredFields = Spdb.MappedFields.Where(m => m.FieldName.Contains(param.sSearch.ToUpper())).ToList();
                AllFields = FilteredFields.OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = FilteredFields.Count();
            }
            else
            {
                displyCount = Spdb.MappedFields.Where(m => m.OrganizationID == OrgID && m.ClassID == 0).OrderBy(m => m.ID).Count();
                AllFields = Spdb.MappedFields.Where(m => m.OrganizationID == OrgID && m.ClassID == 0).OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

            }
            var result = from c in AllFields
                         select new[] {
                             (i++).ToString(), Convert.ToString(c.ID), c.FieldName, c.FieldType, c.Length};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        public bool IsTypeChangable(string FieldType, int ID, string sDatabase, int OrgID, string CreationType, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            if (OrgID == 0)
            {
                if (CreationType == "Create")
                {
                    return true;
                }
                else
                {
                    if (ID > 0)
                    {
                        BOFields model = new BOFields();
                        model = dbContext.BOFields.Find(ID);
                        int TypeID = model.TypeID;
                        string type = ((BODatatypes)TypeID).ToString();
                        if (type == "VARCHAR" && FieldType == "VARCHAR")
                        {
                            return true;
                        }
                        else if (type == "VARCHAR" && FieldType == "INT")
                        {
                            return false;
                        }
                        else if (type == "VARCHAR" && FieldType == "DATETIME")
                        {
                            return false;
                        }
                        else if (type == "INT" && FieldType == "DATETIME")
                        {
                            return false;
                        }
                        else if (type == "INT" && FieldType == "VARCHAR")
                        {
                            return true;
                        }
                        else if (type == "INT" && FieldType == "INT")
                        {
                            return true;
                        }
                        else if (type == "DATETIME" && FieldType == "DATETIME")
                        {
                            return true;
                        }
                        else if (type == "DATETIME" && FieldType == "INT")
                        {
                            return false;
                        }
                        else if (type == "DATETIME" && FieldType == "VARCHAR")
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
            else
            {
                if (CreationType == "Create")
                {
                    return true;
                }
                else
                {
                    if (ID > 0)
                    {
                        DataContext Spdb = new DataContext(sOrgDB);
                        MappedFields fields = Spdb.MappedFields.Find(ID);
                        if (fields.FieldType == "VARCHAR" && FieldType == "VARCHAR")
                        {
                            return true;
                        }
                        else if (fields.FieldType == "VARCHAR" && FieldType == "INT")
                        {
                            return false;
                        }
                        else if (fields.FieldType == "VARCHAR" && FieldType == "DATETIME")
                        {
                            return false;
                        }
                        else if (fields.FieldType == "INT" && FieldType == "DATETIME")
                        {
                            return false;
                        }
                        else if (fields.FieldType == "INT" && FieldType == "VARCHAR")
                        {
                            return true;
                        }
                        else if (fields.FieldType == "INT" && FieldType == "INT")
                        {
                            return true;
                        }
                        else if (fields.FieldType == "DATETIME" && FieldType == "DATETIME")
                        {
                            return true;
                        }
                        else if (fields.FieldType == "DATETIME" && FieldType == "INT")
                        {
                            return false;
                        }
                        else if (fields.FieldType == "DATETIME" && FieldType == "VARCHAR")
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

        }
        public bool IsLengthChangable(string Length, int ID, string sDatabase, int OrgID, string CreationType, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            if (OrgID == 0)
            {
                if (CreationType == "Create")
                {
                    return true;
                }
                else
                {
                    if (ID > 0)
                    {
                        BOFields model = new BOFields();
                        model = dbContext.BOFields.Find(ID);
                        if (model.MaxLength == "MAX" && Length == "MAX")
                        {
                            return true;
                        }
                        else if (model.MaxLength == "MAX")
                        {
                            return false;
                        }
                        else if (Convert.ToInt32(model.MaxLength) == Convert.ToInt32(Length))
                        {
                            return true;
                        }
                        else if (Convert.ToInt32(model.MaxLength) > Convert.ToInt32(Length))
                        {
                            return false;
                        }
                        else if (Convert.ToInt32(model.MaxLength) < Convert.ToInt32(Length))
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
            else
            {
                if (CreationType == "Create")
                {
                    return true;
                }
                else
                {
                    if (ID > 0)
                    {
                        DataContext Spdb = new DataContext(sOrgDB);
                        MappedFields fields = Spdb.MappedFields.Find(ID);
                        if (fields.Length == "MAX" && Length == "MAX")
                        {
                            return true;
                        }
                        else if (fields.Length == "MAX")
                        {
                            return false;
                        }
                        else if (Convert.ToInt32(fields.Length) == Convert.ToInt32(Length))
                        {
                            return true;
                        }
                        else if (Convert.ToInt32(fields.Length) > Convert.ToInt32(Length))
                        {
                            return false;
                        }
                        else if (Convert.ToInt32(fields.Length) < Convert.ToInt32(Length))
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
        }

        //public DTResponse DisplayOrgMappedFields(jQueryDataTableParamModel param, string sDatabase, int OrgID)
        //{

        //    DataContext Spdb = new DataContext(database);
        //    //IQueryable<MappedFields> AllFields;
        //    IQueryable<VMMappedFields> AllFields;
        //    //AllFields = Spdb.MappedFields.Where(m => m.AddField != null).GroupBy(m => m.ClassID).Select(m => m.FirstOrDefault());
        //    AllFields = (from c in Spdb.MappedFields.Where(m => m.AddField != null)
        //                 join t in Spdb.OrganizationClasses on c.ClassID equals t.ClassID
        //                 select new VMMappedFields
        //                 {
        //                     ClassID = c.ClassID,
        //                     Class = t.Class
        //                 }).GroupBy(m => m.ClassID).Select(m => m.FirstOrDefault());
        //    //string sortExpression = ServiceConstants.SortExpression;
        //    string sortExpression = "ClassID";
        //    if (!string.IsNullOrWhiteSpace(param.sSearch))
        //    {
        //        List<int> total = Spdb.OrganizationClasses.Where(m => m.Class.Contains(param.sSearch)).Select(m => m.ClassID).ToList();
        //        if (total != null)
        //        {
        //            AllFields = AllFields.Where(m => total.Contains(m.ClassID));
        //        }
        //    }
        //    int displyCount = 0;
        //    displyCount = AllFields.Count();
        //    AllFields = QuerableUtil.GetResultsForDataTables(AllFields, "", sortExpression, param);
        //    int i = param.iDisplayStart + 1;
        //    var result = from c in AllFields.ToList()
        //                 select new[] {
        //                          (i++).ToString(), c.ClassID.ToString(), c.Class,""};
        //    return new DTResponse()
        //    {
        //        sEcho = param.sEcho,
        //        iTotalRecords = displyCount,
        //        iTotalDisplayRecords = displyCount,
        //        aaData = result
        //    };
        //}      

        public DTResponse DisplayOrgMappedFields(jQueryDataTableParamModel param, string sDatabase, int OrgID, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            var AllClasses = dbContext.BOClassAttributes.ToList();
            IQueryable<VMMappedFields> AllFields;
            List<VMMappedFields> Notify = new List<VMMappedFields>();
            AllFields = (from c in Spdb.MappedFields.Where(m => m.AddField != null)
                         select new VMMappedFields
                         {
                             ClassID = c.ClassID,
                             BOID = c.BOID
                         });

            foreach (var items in AllFields)
            {
                if (items.ClassID > 0)
                {
                    items.Class = AllClasses.Where(m => m.ID == items.ClassID).Select(m => m.Class).FirstOrDefault();
                }
                Notify.Add(items);
            }

            string sortExpression = "ClassID";
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                Notify = Notify.Where(m => m.Class.ToLower().Contains(param.sSearch)).ToList();
            }
            int displyCount = 0;
            displyCount = Notify.Count();
            AllFields = QuerableUtil.GetResultsForDataTables(Notify.AsQueryable(), "", sortExpression, param);
            int i = param.iDisplayStart + 1;
            var result = from c in AllFields.ToList()
                         select new[] {
                                  (i++).ToString(), c.ClassID.ToString() , c.BOID.ToString() , c.Class,""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        //private string GetClassName(int p, string sDatabase)
        //{
        //    DataContext Spdb = new DataContext(database);
        //    string ClassName = Spdb.OrganizationClasses.Where(m => m.ClassID == p).Select(m => m.Class).FirstOrDefault();
        //    return ClassName;
        //}

        private string GetSubscription(string p, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            var sub = Spdb.OrganizationSubscriptions.Where(m => m.SubscriptionID == p).FirstOrDefault();
            var source = Spdb.OrganizationSources.Where(m => m.ID == sub.SourceID).Select(m => m.Name).FirstOrDefault();
            var classname = Spdb.OrganizationClasses.Where(m => m.ClassID == sub.ClassID).Select(m => m.Class).FirstOrDefault();
            var subid = source + "-" + classname;
            return subid;
        }

        public List<VMDropDown> GetOrgSubscriptions(int OrgID, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            var OrgClassType = new List<VMDropDown>();
            DataContext Spdb = new DataContext(sOrgDB);
            OrgClassType = (from s in Spdb.OrganizationSubscriptions.Where(m => m.OrganizationID == OrgID).Where(m => m.StatusTypeID == 10)
                            join r in Spdb.OrganizationSources on s.SourceID equals r.ID
                            join c in Spdb.OrganizationClasses on s.ClassID equals c.ClassID
                            select new VMDropDown { text = r.Name + "-" + c.Class, Expression = s.SubscriptionID }).ToList();

            //var OrgClasses = Spdb.OrganizationSubscriptions.Where(m => m.OrganizationID == OrgID).Where(m => m.StatusTypeID == 10).ToList();
            //foreach (var item in OrgClasses)
            //{

            //    string SourceName = Spdb.OrganizationSources.Where(m => m.ID == item.SourceID).Select(m => m.Name).FirstOrDefault();
            //    string ClassName = Spdb.OrganizationClasses.Where(m => m.ClassID == item.ClassID).Select(m => m.Class).FirstOrDefault();
            //    OrgClassType.Add(new VMDropDown
            //    {
            //        text = SourceName + "-" + ClassName,
            //        Expression = item.SubscriptionID
            //    });
            //}
            return OrgClassType;
        }

        public LeadMappings ViewOrgMappedFields(int ClassID, string sDatabase, int OrgID, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            LeadMappings model = new LeadMappings();
            var MappedFields = new List<MappedField>();
            DataContext Spdb = new DataContext(sOrgDB);
            var Fields = Spdb.MappedFields.Where(m => m.OrganizationID == OrgID).Where(m => m.ClassID == ClassID && m.AddField != null).ToList();

            foreach (var items in Fields)
            {
                MappedFields.Add(new MappedField
                {
                    Name = items.FieldName,
                    Type = items.AddField
                });
            }
            model.MappedFields = MappedFields;
            return model;
        }

        public VMLeadMappings GetOrgLeadFields(int ClassID, string Type, string Category, int OrgID, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            if (Category == "String" || Category == "Alphanumeric")
            {
                Category = "varchar";
            }
            DataContext Spdb = new DataContext(sOrgDB);
            var SetupFields = dbContext.OrganizationSetups.Where(m => m.Name == EnumLeadTables.Leads.ToString()).Select(m => m.Columns).FirstOrDefault();
            var CmnFields = SetupFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var DefaultFields = new List<string>();
            foreach (var items in CmnFields)
            {
                if (items.IndexOf("AddField", StringComparison.OrdinalIgnoreCase) >= 0 && items.IndexOf(Category, StringComparison.OrdinalIgnoreCase) >= 0)
                {

                }
                else
                {
                    DefaultFields.Add(items.Split(' ')[0]);
                }
            }

            var SAFields = dbContext.BOFields.Where(m => m.BOID == 1).Where(m => m.OrganizationID == 0 && (m.FieldClassID == 0)).ToList();
            var SAAddedFields = new List<string>();
            foreach (var items in SAFields)
            {
                SAAddedFields.Add(items.Name);
            }
            var AddFields = SAAddedFields.Except(DefaultFields).ToList();
            var MappedFields = Spdb.MappedFields.Where(m => m.ClassID == ClassID && m.FieldType == Category.ToUpper()).Select(m => m.AddField).ToList();
            AddFields = AddFields.Except(MappedFields).ToList();
            var AFields = Spdb.MappedFields.Where(m => m.OrganizationID == OrgID).Where(m => m.ClassID == ClassID && m.AddField == null).Where(m => m.FieldType == Category.ToUpper()).ToList();
            var AAddedFields = new List<string>();
            foreach (var items in AFields)
            {
                AAddedFields.Add(items.FieldName);
            }
            var IncludedFields = new List<string>();
            if (Type == "Create")
            {
                IncludedFields = Spdb.MappedFields.Where(m => m.ClassID == ClassID && m.AddField != null).Where(m => m.FieldType == Category.ToUpper()).Select(m => m.FieldName).ToList();
                AAddedFields = AAddedFields.Except(IncludedFields).ToList();
            }
            else
            {
                //AAddedFields = IncludedFields;
            }
            VMLeadMappings Result = new VMLeadMappings();
            Result.NonClassFields = AddFields;
            Result.ExistingFields = AAddedFields;
            return Result;
        }

        public VMCustomResponse SaveOrgMappedLeadFields(int ClassID, string LeadField, string OrgField, int MasterID, int OrgID, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            var OrgFieldData = Spdb.MappedFields.Where(m => m.ClassID == ClassID && m.FieldName == OrgField).FirstOrDefault();
            OrgFieldData.AddField = LeadField;
            if (MasterID > 0)
            {
                OrgFieldData.MasterID = MasterID;
                OrgFieldData.IsDropDown = true;
            }
            Spdb.SaveChanges();
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = ClassID, Status = true };
        }

        public DTResponse OrgMappedFieldsGrid(jQueryDataTableParamModel param, int ClassID, string Type, string sDatabase, int OrgID, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            if (Type == "String")
            {
                Type = "varchar";
            }
            DataContext Spdb = new DataContext(sOrgDB);
            IEnumerable<MappedFields> AllDetails, FilteredDetails;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredDetails = Spdb.MappedFields.Where(m => m.OrganizationID == OrgID).Where(m => m.FieldName.Contains(param.sSearch.ToUpper()) && m.AddField != null && m.FieldType.ToLower() == Type.ToLower()).ToList();
                AllDetails = FilteredDetails.OrderBy(m => m.FieldName).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = FilteredDetails.Count();
            }
            else
            {
                displyCount = Spdb.MappedFields.Where(m => m.ClassID == ClassID && m.AddField != null && m.FieldType == Type.ToUpper()).Where(m => m.OrganizationID == OrgID).Count();
                AllDetails = Spdb.MappedFields.Where(m => m.OrganizationID == OrgID).Where(m => m.ClassID == ClassID && m.AddField != null && m.FieldType == Type.ToUpper()).OrderBy(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

            }
            var result = from c in AllDetails
                         select new[] {
                             (i++).ToString(),Convert.ToString(c.ID), c.FieldName, c.AddField,c.IsDropDown.ToString(),""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public int DeleteMappedField(int ID, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            MappedFields Field = Spdb.MappedFields.Find(ID);
            Field.AddField = null;
            Spdb.SaveChanges();
            return 0;
        }

        public DTResponse DisplayFields(jQueryDataTableParamModel param, string sDatabase, int OrgID, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            var OrgClasses = Spdb.OrganizationClasses.Where(m => m.OrganizationID == OrgID).Select(m => m.ClassID).ToList();
            IEnumerable<VMDropDown> AllDetails, FilteredDetails;
            int displayCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredDetails = (from c in Spdb.LeadMappings
                                   group c by new
                                   {
                                       c.SubscriptionID,
                                       c.ClassID
                                   } into gcs
                                   select new VMDropDown()
                                   {
                                       text = gcs.Key.SubscriptionID.ToString(),
                                       Value = gcs.Key.ClassID,
                                   }).ToList();
                AllDetails = (from c in FilteredDetails
                              join oc in Spdb.OrganizationClasses.Where(m => m.OrganizationID == OrgID) on c.Value equals oc.ClassID
                              where oc.Class.ToUpper().Contains(param.sSearch.ToUpper())
                              select new VMDropDown()
                              {
                                  text = c.text,
                                  Expression = c.Value.ToString(),
                                  Value = c.Value
                              }).ToList();
                displayCount = AllDetails.Count();
                if (OrgID != 0)
                {
                    AllDetails = AllDetails.Where(m => OrgClasses.Contains(m.Value)).OrderBy(m => m.Expression).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                }
                else
                {
                    AllDetails = AllDetails.OrderBy(m => m.Expression).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                }
            }
            else
            {
                AllDetails = (from c in dbContext.MasterTemplates
                              group c by new
                              {
                                  c.ClassID
                              } into gcs
                              select new VMDropDown()
                              {
                                  Expression = gcs.Key.ClassID.ToString(),
                                  Value = gcs.Key.ClassID
                              }).ToList();

                displayCount = AllDetails.Count();
                if (OrgID != 0)
                {
                    AllDetails = AllDetails.Where(m => OrgClasses.Contains(m.Value)).OrderBy(m => m.Expression).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                }
                else
                {
                    AllDetails = AllDetails.OrderBy(m => m.Expression).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                }
            }
            var result = from c in AllDetails
                         select new[] {
                         (i++).ToString(),Convert.ToString(c.Expression), GetClass(c.Expression, sDatabase),""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displayCount,
                iTotalDisplayRecords = displayCount,
                aaData = result
            };
        }

        private string GetClass(string classID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            int Classid = Convert.ToInt32(classID);
            var ClassID = dbContext.Types.Where(m => m.ID == Classid).Select(m => m.Expression).FirstOrDefault();
            return ClassID;
        }

        public VMLeadMappings GetLeadFields(int ClassID, string Type, int OrgID, string sDatabase)
        {
            //DataContext db = new DataContext();
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var database = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            var NonClassFields = dbContext.BOFields.Where(m => m.OrganizationID == 0 && m.BOID == 1).Where(m => m.FieldCreatedID == 0 && m.FieldClassID == 0).OrderBy(m => m.ID).Select(m => m.Name).ToList();
            var ClassFields = dbContext.MasterTemplates.Where(m => m.ClassID == ClassID && m.FieldName == null).Select(m => m.DataFieldName).ToList();
            var AllClass = dbContext.Types.Where(m => m.Name == "Class Type").ToList();
            List<VMDropDown> AllClasses = new List<VMDropDown>();
            AllClasses = (from c in dbContext.Types.Where(m => m.Name == "Class Type").ToList()
                          select new VMDropDown { Value = c.ID, text = c.Expression }).ToList();
            if (Type == "Create")
            {
                var SetupFields = dbContext.OrganizationSetups.Where(m => m.Name == EnumLeadTables.Leads.ToString()).Select(m => m.Columns).FirstOrDefault();
                var CmnFields = SetupFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var DefaultFields = new List<string>();
                var addedFileds = dbContext.MasterTemplates.Where(m => m.ClassID == ClassID && m.DataFieldName != null).Select(m => m.FieldName).ToList();
                foreach (var items in CmnFields)
                {
                    DefaultFields.Add(items.Split(' ')[0]);
                }
                var NotAddedNonClassFields = NonClassFields.Except(addedFileds).ToList();
                VMLeadMappings Results = new VMLeadMappings();
                Results.NonClassFields = NotAddedNonClassFields;
                Results.ClassFields = ClassFields;
                Results.DefaultFields = NotAddedNonClassFields;
                Results.Classes = AllClasses;
                return Results;
            }
            else
            {
                var AllMappedFields = dbContext.MasterTemplates.Where(m => m.ClassID == ClassID).Select(m => m.FieldName).ToList();
                var NewNonClassFields = NonClassFields.Except(AllMappedFields).ToList();
                var NewClassFields = ClassFields.Except(AllMappedFields).ToList();
                VMLeadMappings Results = new VMLeadMappings();
                Results.NonClassFields = NewNonClassFields;
                Results.ClassFields = NewClassFields;
                Results.DefaultFields = AllMappedFields;
                Results.Classes = AllClasses;
                Results.TempName = dbContext.MasterTemplates.Where(m => m.ClassID == ClassID).Select(m => m.Name).FirstOrDefault();
                return Results;
            }
        }

        public LeadMappings MappedFieldsGrid(LeadMappings model, int ClsID, string sDatabase, int OrgID)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var MappedFields = new List<MappedField>();
            List<MasterTemplates> sFields = new List<MasterTemplates>();
            //var ClassID = dbContext.OrganizationClasses.Where(m => m.ClassID == ClsID).Select(m => m.ClassID).FirstOrDefault();
            if (OrgID == 0)
            {
                MappedFields = (from c in dbContext.MasterTemplates.Where(m => m.ClassID == ClsID && m.DataFieldName != null && m.FieldName != null)
                                select new MappedField { Name = c.FieldName, ColumnName = c.DataFieldName }
                                    ).ToList();
            }
            else
            {
                sFields = dbContext.MasterTemplates.Where(m => m.ClassID == ClsID).ToList();
                foreach (var item in sFields)
                {
                    MappedFields.Add(new MappedField
                    {
                        Name = item.FieldName,
                        Type = item.FieldType
                    });
                }
            }
            model.MappedFields = MappedFields;
            return model;
        }

        public bool IsExistsTemplateTitle(string Type, string Name, int ID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var templates = dbContext.MasterTemplates.Where(m => m.Name == Name).ToList();
            var template = templates.Where(m => m.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (ID == 0)
            {
                if (template != null)
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
                if (template != null)
                {
                    if (ID == template.ID)
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

        public string SaveMappedLeadFields(string Name, int ClsID, string DataField, string ColumnField, int OrgID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            MasterTemplates Master = dbContext.MasterTemplates.Where(m => m.DataFieldName == DataField && m.ClassID == ClsID).FirstOrDefault();
            Master.FieldName = ColumnField;
            dbContext.SaveChanges();
            //var dbName = dbContext.AspNetUsers.Where(m => m.OrganizationID == OrgID).Select(m => m.DatabaseName).FirstOrDefault();
            //var DeleteFields = new List<string>();
            //var AddedFields = new List<string>();
            //AddedFields = dbContext.MasterTemplates.Where(m => m.ClassID == ClsID).Select(m => m.FieldName).ToList();
            //List<string> NewFields = new List<string>();
            //for (int i = 0; i < MappedFields.Length; i++)
            //{
            //    NewFields.Add(MappedFields[i]);
            //}
            //DeleteFields = AddedFields.Except(NewFields).ToList();
            //foreach (var items in DeleteFields)
            //{
            //    MasterTemplates Template = dbContext.MasterTemplates.Where(m => m.ClassID == ClsID).Where(m => m.FieldName == items).FirstOrDefault();
            //    dbContext.MasterTemplates.Remove(Template);
            //    dbContext.SaveChanges();
            //}
            //var RemainingFields = NewFields.Except(AddedFields).ToList();
            //if (AddedFields.Count() > 0)
            //{
            //    foreach (var items in RemainingFields)
            //    {
            //        MasterTemplates Template = new MasterTemplates();
            //        Template.Name = Name;
            //        Template.ClassID = ClsID;
            //        Template.FieldName = items;
            //        dbContext.MasterTemplates.Add(Template);
            //        dbContext.SaveChanges();
            //    }
            //}
            //else
            //{
            //    for (int i = 0; i < MappedFields.Length; i++)
            //    {
            //        var keyvalues = MappedFields[i];
            //        MasterTemplates Template = new MasterTemplates();
            //        Template.Name = Name;
            //        Template.ClassID = ClsID;
            //        Template.FieldName = keyvalues;
            //        dbContext.MasterTemplates.Add(Template);
            //        dbContext.SaveChanges();
            //    }
            //}
            return null;
        }

        public List<VMDropDown> GetClassTypes(int OrgID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var OrgClassType = new List<VMDropDown>();
            //DataContext dbContext = new DataContext();
            //var sClass = dbContext.OrganizationClasses.Where(m => m.OrganizationID == OrgID).ToList();
            var AddedClasses = dbContext.MasterTemplates.Select(m => m.ClassID).Distinct().ToList();
            var sClass = dbContext.Types.Where(m => m.Name == "Class Type").Select(m => m.ID).ToList();
            var RemainingClasses = sClass.Except(AddedClasses).ToList();
            if (OrgID == 0)
            {
                foreach (var item in RemainingClasses)
                {
                    var Classes = dbContext.Types.Where(m => m.ID == item).FirstOrDefault();
                    OrgClassType.Add(new VMDropDown
                    {
                        text = Classes.Expression,
                        Value = Classes.ID
                    });
                }
            }
            else
            {
                foreach (var item in sClass)
                {
                    var Classes = dbContext.Types.Where(m => m.ID == item).FirstOrDefault();
                    OrgClassType.Add(new VMDropDown
                    {
                        text = Classes.Expression,
                        Value = Classes.ID
                    });
                }
            }
            return OrgClassType;
        }
        public DTResponse GetMappedMasterFieldsList(jQueryDataTableParamModel param, int ClassID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            IEnumerable<MasterTemplates> AllDetails, FilteredDetails;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredDetails = dbContext.MasterTemplates.Where(m => m.ClassID == ClassID).Where(m => m.FieldName.Contains(param.sSearch.ToUpper())).ToList();
                AllDetails = FilteredDetails.OrderBy(m => m.FieldName).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = FilteredDetails.Count();
            }
            else
            {
                displyCount = dbContext.MasterTemplates.Where(m => m.ClassID == ClassID && m.DataFieldName != null && m.FieldName != null).Count();
                AllDetails = dbContext.MasterTemplates.Where(m => m.ClassID == ClassID && m.DataFieldName != null && m.FieldName != null).OrderBy(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

            }
            var result = from c in AllDetails
                         select new[] {
                             (i++).ToString(),Convert.ToString(c.ID), c.FieldName, c.DataFieldName,""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        public int DeleteMasterMappedField(int ID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            MasterTemplates Master = dbContext.MasterTemplates.Find(ID);
            Master.FieldName = null;
            dbContext.SaveChanges();
            return ID;
        }

        public List<VMDropDown> GetAllMasterTypes(string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> AllTypes = new List<VMDropDown>();
            AllTypes = (from c in dbContext.Types.Where(m => m.Name == "Sys Type").ToList()
                        select new VMDropDown { text = c.Expression, Value = c.ID }).ToList();
            return AllTypes;
        }
    }
}
