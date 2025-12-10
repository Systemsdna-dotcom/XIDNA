using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using XIDNA.ViewModels;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace XIDNA.Repository
{
    public class WalletRepository : IWalletRepository
    {
        ModelDbContext dbcontext = new ModelDbContext();

        #region Request

        public int SaveWalletRequest(WalletRequests Request, string database)
        {
            WalletRequests Req = new WalletRequests();
            using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("", Con);
                cmd.CommandText = "Select sEmail from " + EnumLeadTables.Leads.ToString() + " Where ID=" + Request.LeadID;
                Con.Open();
                Con.ChangeDatabase(database);
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable data = new DataTable();
                data.Load(reader);
                List<object[]> TotalResult = data.AsEnumerable().Select(m => m.ItemArray).ToList();
                Req.EmailID = TotalResult[0][0].ToString();
                Con.Dispose();
            }
            //Req.EmailID = Request.EmailID;
            Req.FKiLeadClassID = Request.FKiLeadClassID;
            Req.OrganizationID = Request.OrganizationID;
            dbcontext.WalletRequests.Add(Req);
            dbcontext.SaveChanges();
            return Req.ID;
        }

        public List<VMRequests> GetAllRequests(string EmailID, int Count)
        {
            IQueryable<VMRequests> AllReqs;

            if (Count == 0)//0 means Requests Grid ; 1 means Count (of unaccepted Requests)
            {
                AllReqs = (from c in dbcontext.WalletRequests.Where(m => m.EmailID == EmailID)
                           join o in dbcontext.Organization on c.OrganizationID equals o.ID
                           select new VMRequests
                           {
                               OrganizationID = c.OrganizationID,
                               EmailID = c.EmailID,
                               Status = c.Status,
                               IsActivated = c.IsActivated,
                               OrgnizationName = o.Name
                           }).ToList().AsQueryable();
            }
            else
            {
                AllReqs = (from c in dbcontext.WalletRequests.Where(m => m.EmailID == EmailID && m.IsActivated == false && m.Status == null)
                           join o in dbcontext.Organization on c.OrganizationID equals o.ID
                           select new VMRequests
                           {
                               OrganizationID = c.OrganizationID,
                               EmailID = c.EmailID,
                               Status = c.Status,
                               IsActivated = c.IsActivated,
                               OrgnizationName = o.Name
                           }).ToList().AsQueryable();
            }

            List<VMRequests> l = AllReqs.ToList();
            return l;
        }

        public WalletRequests AcceptOrgRequest(WalletRequests model)
        {
            WalletRequests Req = new WalletRequests();
            Req = dbcontext.WalletRequests.Where(m => m.EmailID == model.EmailID && m.OrganizationID == model.OrganizationID).FirstOrDefault();
            Req.Status = model.Status;
            if (model.Status == "Accept")
            {
                Req.IsActivated = true;
            }
            else
            {
                Req.IsActivated = false;
            }
            Req.ClientID = model.ClientID;
            dbcontext.SaveChanges();
            return Req;
        }

        public string IsClientActivated(string ClientID)
        {
            var Reqs = dbcontext.WalletRequests.Where(m => m.ClientID == ClientID && m.Status == "Accept").ToList();
            string Status = "";
            if (Reqs.Count() > 0)
            {
                Status = "Accept";
            }
            else
            {
                Status = "Decline";
            }
            return Status;
        }

        public List<VMDropDown> GetAllLinkedOrgs(string ClientID)
        {
            List<VMDropDown> AllOrgs = new List<VMDropDown>();
            AllOrgs = (from c in dbcontext.WalletRequests.Where(m => m.ClientID == ClientID && m.Status == "Accept")
                       join o in dbcontext.Organization on c.OrganizationID equals o.ID
                       select new VMDropDown { Value = c.OrganizationID, text = o.Name }
                           ).ToList();
            return AllOrgs;
        }

        #endregion Request

        #region Inbox

        public List<VMMessages> GetAllMessages(string ClientID)
        {
            int i = 0;
            List<VMMessages> AllMails = new List<VMMessages>();
            List<WalletMessages> Mails = new List<WalletMessages>();
            var AllReqs = dbcontext.WalletRequests.Where(m => m.ClientID == ClientID).Select(m => m.OrganizationID).Distinct().ToList();
            foreach (var Orgs in AllReqs)
            {
                var Org = dbcontext.Organization.Find(Orgs);
                DataContext Spdb = new DataContext(Org.DatabaseName);
                Mails = Spdb.WalletMessages.Where(m => m.ClientID == ClientID && m.OrganizationID == Orgs).ToList();
                foreach (var items in Mails)
                {

                    var mainID = items.ID;
                    var val = Mails.Where(m => m.ParentID == mainID).ToList();
                    if (val.Count() > 0)
                    {

                    }
                    else
                    {
                        VMMessages Msg = new VMMessages();
                        Msg.ID = items.ID;
                        Msg.Message = items.Message;
                        Msg.Subject = items.Subject;
                        Msg.ReceivedOn = items.ReceivedOn;
                        Msg.OrganizationName = Org.Name;
                        Msg.Sender = items.Sender;
                        Msg.OrganizationID = items.OrganizationID;
                        Msg.IsRead = items.IsRead;
                        Msg.Attachments = items.Attachments;
                        Msg.PageSize = 5;
                        Msg.PageNumber = i++;
                        Msg.MailNumber = i;
                        AllMails.Add(Msg);
                    }
                }
            }
            // AllMails.FirstOrDefault().Total = AllMails.Count;
            return AllMails;
        }
        //MsgIconNotification
        public List<WalletOrders> ExpiryDateNotification(string ClientID)
        {
            List<WalletOrders> AllOrders = new List<WalletOrders>();
            var AllReqs = dbcontext.WalletRequests.Where(m => m.ClientID == ClientID && m.Status == "Accept").ToList();
            var Orgs = AllReqs.Select(m => m.OrganizationID).Distinct().ToList();
            string EmailID = AllReqs.Select(m => m.EmailID).FirstOrDefault();
            foreach (var items in Orgs)
            {
                var Org = dbcontext.Organization.Find(items);
                //var LeadIDs = GetLeadIDs(EmailID, items);
                DataContext Spdb = new DataContext(Org.DatabaseName);
                var orgname = Org.Name;
                var records = Spdb.WalletOrders.Where(m => m.OrganizationID == items).ToList();
                records.ToList().ForEach(m => m.OrganizationName = Org.Name);
                foreach (var item in records)
                {
                    var seeDate = item.ToDate;
                    var todayDate = DateTime.Now;
                    double num = (seeDate - todayDate).TotalDays;
                    if (num >= 0 && num <= 7)
                    {
                        item.DaysRemaining = Convert.ToInt32(num);
                        var row = Spdb.WalletProducts.Find(item.ProductID);
                        string prodname = row.Name;
                        item.ProductName = prodname;
                        AllOrders.Add(item);

                    }
                }
            }
            return AllOrders;
        }

        public List<VMMessages> GetMessageByID(string ID)
        {
            if (ID == "")
            {

                return null;
            }
            else
            {
                List<VMMessages> AllMsgs = new List<VMMessages>();
                var IDs = ID.Split('-').ToList();
                var OrgID = Convert.ToInt32(IDs[1]);
                var MsgID = Convert.ToInt32(IDs[0]);
                var Org = dbcontext.Organization.Find(OrgID);
                DataContext Spdb = new DataContext(Org.DatabaseName);
                WalletMessages Msg = new WalletMessages();
                var AllMessgs = Spdb.WalletMessages.ToList();
                Msg = AllMessgs.Where(m => m.ID == MsgID).FirstOrDefault();
                VMMessages Msgs = new VMMessages();
                Msgs.ID = Msg.ID;
                Msgs.Message = Msg.Message;
                Msgs.Subject = Msg.Subject;
                Msgs.ReceivedOn = Msg.ReceivedOn;
                Msgs.OrganizationName = Org.Name;
                Msgs.Sender = Msg.Sender;
                Msgs.OrganizationID = Msg.OrganizationID;
                AllMsgs.Add(Msgs);
                if (Msg.MasterMail != null)
                {
                    do
                    {
                        Msg = Msg.MasterMail;
                        VMMessages Parent = new VMMessages();
                        Parent.ID = Msg.ID;
                        Parent.Message = Msg.Message;
                        Parent.Subject = Msg.Subject;
                        Parent.ReceivedOn = Msg.ReceivedOn;
                        Parent.OrganizationName = Org.Name;
                        Parent.Sender = Msg.Sender;
                        Parent.OrganizationID = Msg.OrganizationID;
                        AllMsgs.Add(Parent);
                    } while (Msg.MasterMail != null);
                }
                return AllMsgs;
            }

        }

        public VMMessages ChangeMessageStatus(string ID)
        {
            var IDs = ID.Split('-').ToList();
            var OrgID = Convert.ToInt32(IDs[1]);
            var MsgID = Convert.ToInt32(IDs[0]);
            var Org = dbcontext.Organization.Find(OrgID);
            DataContext Spdb = new DataContext(Org.DatabaseName);
            var Messge = Spdb.WalletMessages.Find(MsgID);
            Messge.IsRead = true;
            Spdb.SaveChanges();
            VMMessages Mesg = new VMMessages();
            Mesg.ID = MsgID;
            return Mesg;
        }


        #endregion Inbox

        #region Products

        public List<WalletProducts> GetWalletProducts(string ClientID)
        {
            List<WalletProducts> AllProducts = new List<WalletProducts>();
            var AllReqs = dbcontext.WalletRequests.Where(m => m.ClientID == ClientID).ToList();
            var Orgs = AllReqs.Select(m => m.OrganizationID).Distinct().ToList();
            string EmailID = AllReqs.Select(m => m.EmailID).FirstOrDefault();
            foreach (var items in Orgs)
            {
                var AllClasses = GetLeadClasses(EmailID, items);
                var Org = dbcontext.Organization.Find(items);
                DataContext Spdb = new DataContext(Org.DatabaseName);
                foreach (var Cls in AllClasses)
                {
                    var Products = (from c in Spdb.WalletProducts.Where(m => m.ClassID == Cls && m.StatusTypeID == 10).Where(m => DbFunctions.TruncateTime(m.ExpiryDate) >= DbFunctions.TruncateTime(DateTime.Now))
                                    select c).ToList();
                    Products.ToList().ForEach(m => m.OrgName = Org.Name);
                    AllProducts.AddRange(Products);
                }
            }
            return AllProducts;
        }
        public WalletProducts GetWalletProductByID(int ID, int OrgID)
        {
            var Org = dbcontext.Organization.Find(OrgID);
            DataContext Spdb = new DataContext(Org.DatabaseName);
            WalletProducts Product = Spdb.WalletProducts.Find(ID);
            return Product;
        }

        public DTResponse GetWalletProductsList(jQueryDataTableParamModel param, int OrgID, string database)
        {
            DataContext Spdb = new DataContext(database);
            IQueryable<WalletProducts> AllProducts;
            AllProducts = Spdb.WalletProducts.Where(m => m.OrganizationID == OrgID);
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllProducts = AllProducts.Where(m => m.Name.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllProducts.Count();
            AllProducts = QuerableUtil.GetResultsForDataTables(AllProducts, "", sortExpression, param);
            var clients = AllProducts.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     join t in dbcontext.Types on c.ClassID equals t.ID
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.OrganizationID.ToString(), c.Name, c.Type.ToString(), c.Price.ToString(), c.ExpiryDate.ToString("dd MMM yyyy"), t.Expression, Convert.ToString(c.StatusTypeID), ""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public VMCustomResponse AddWalletProdut(WalletProducts Product, int OrgID, string database)
        {
            if (Product.ID == 0)
            {
                DataContext Spdb = new DataContext(database);
                WalletProducts Pro = new WalletProducts();
                Pro.Name = Product.Name;
                Pro.Content = Product.Content;
                Pro.TemplateID = Product.TemplateID;
                Pro.ClassID = Product.ClassID;
                Pro.Price = Product.Price;
                Pro.StatusTypeID = Product.StatusTypeID;
                Pro.OrganizationID = OrgID;
                Pro.ExpiryDate = Convert.ToDateTime(Product.ExpiryDate);
                Pro.Type = Product.Type;
                Spdb.WalletProducts.Add(Pro);
                Spdb.SaveChanges();
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = Pro.ID, Status = true };
            }
            else
            {
                DataContext Spdb = new DataContext(database);
                WalletProducts Pro = Spdb.WalletProducts.Find(Product.ID);
                Pro.Name = Product.Name;
                Pro.Content = Product.Content;
                Pro.ClassID = Product.ClassID;
                Pro.Price = Product.Price;
                Pro.TemplateID = Product.TemplateID;
                Pro.StatusTypeID = Product.StatusTypeID;
                Pro.ExpiryDate = Convert.ToDateTime(Product.ExpiryDate);
                Spdb.SaveChanges();
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = Pro.ID, Status = true };
            }

        }
        public int SaveImageOrDoc(int ID, string Name, string Type, int OrgID, string database)
        {
            DataContext Spdb = new DataContext(database);
            WalletProducts Product = Spdb.WalletProducts.Find(ID);
            if (Type == "Image")
            {
                Product.Image = Name;
            }
            else
            {
                Product.Document = Name;
            }
            Spdb.SaveChanges();
            return ID;
        }
        public bool IsExistsWalletProductName(string Name, int ID, int OrgID, string database)
        {
            DataContext Spdb = new DataContext(database);
            var Products = Spdb.WalletProducts.Where(m => m.OrganizationID == OrgID).ToList();
            WalletProducts Pro = Products.Where(m => m.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (ID == 0)
            {
                if (Pro != null)
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
                if (Pro != null)
                {
                    if (ID == Pro.ID)
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
        public List<WalletProducts> GetProductDetails(int ProdID)
        {
            DataContext db = new DataContext("XIDNAOrg1");
            if (ProdID == 0)
            {
                var list = db.WalletProducts.ToList();
                return list;
            }
            else
            {
                var list = db.WalletProducts.Where(m => m.ID == ProdID).ToList();
                return list;
            }

        }
        public WalletProducts GetWalletProductByID(int ID, int OrgID, string database)
        {
            DataContext Spdb = new DataContext(database);
            var Product = Spdb.WalletProducts.Find(ID);
            return Product;
        }

        #endregion Products

        #region Quotes

        public List<WalletQuotes> GetWalletQuotes(string ClientID)
        {
            List<WalletQuotes> AllQuotes = new List<WalletQuotes>();
            var AllReqs = dbcontext.WalletRequests.Where(m => m.ClientID == ClientID).ToList();
            var Orgs = AllReqs.Select(m => m.OrganizationID).Distinct().ToList();
            string EmailID = AllReqs.Select(m => m.EmailID).FirstOrDefault();
            foreach (var items in Orgs)
            {
                var AllClasses = GetLeadClasses(EmailID, items);
                var Org = dbcontext.Organization.Find(items);
                DataContext Spdb = new DataContext(Org.DatabaseName);
                var OrgClasses = Spdb.OrganizationClasses.Where(m => m.OrganizationID == items).ToList();
                var WQuotes = Spdb.WalletQuotes.Where(m => AllClasses.Contains(m.ClassID) && m.IsPosted == true).ToList();
                var LeadIDs = GetLeadIDs(EmailID, items);
                var Quotes = (from c in WQuotes.Where(m => LeadIDs.Contains(m.LeadID))
                              select c).ToList();
                Quotes.ToList().ForEach(s => s.ClassName = OrgClasses.Where(m => m.ClassID == s.ClassID).Select(m => m.Class).FirstOrDefault());
                Quotes.ToList().ForEach(s => s.OrgName = Org.Name);
                AllQuotes.AddRange(Quotes);
            }
            return AllQuotes;
        }

        public string GetQuoteByID(string QuoteID, string sDatabase)
        {
            var Details = QuoteID.Split('-').ToList();
            var Org = dbcontext.Organization.Find(Convert.ToInt32(Details[1]));
            DataContext Spdb = new DataContext(Org.DatabaseName);
            var Quote = Spdb.WalletQuotes.Find(Convert.ToInt32(Details[0]));
            var Template = Spdb.ContentEditors.Find(Quote.TemplateID);
            Common Com = new Common();
            var Content = Com.ReplaceTemplateContent(Template, Convert.ToInt32(Details[0]), EnumLeadTables.WalletQuotes.ToString(), sDatabase);
            return Content;
        }

        #endregion Quotes

        #region Policies

        public List<WalletPolicies> GetWalletPolicies(string ClientID)
        {
            List<WalletPolicies> AllPolicies = new List<WalletPolicies>();
            var AllReqs = dbcontext.WalletRequests.Where(m => m.ClientID == ClientID && m.Status == "Accept").ToList();
            var Orgs = AllReqs.Select(m => m.OrganizationID).Distinct().ToList();
            string EmailID = AllReqs.Select(m => m.EmailID).FirstOrDefault();
            foreach (var items in Orgs)
            {
                var Org = dbcontext.Organization.Find(items);
                var LeadIDs = GetLeadIDs(EmailID, items);
                DataContext Spdb = new DataContext(Org.DatabaseName);
                foreach (var ID in LeadIDs)
                {
                    var Policies = (from c in Spdb.WalletPolicies.Where(m => m.Status == "Live" && m.LeadID == ID && m.IsPosted == true)
                                    select c).ToList();
                    Policies.ToList().ForEach(m => m.OrgName = Org.Name);
                    AllPolicies.AddRange(Policies);
                }
            }
            return AllPolicies;
        }
        public List<WalletPolicies> GetPoliciesDocs(string ClientID)
        {
            List<WalletPolicies> AllPolicies = new List<WalletPolicies>();
            var AllReqs = dbcontext.WalletRequests.Where(m => m.ClientID == ClientID && m.Status == "Accept").ToList();
            var Orgs = AllReqs.Select(m => m.OrganizationID).Distinct().ToList();
            string EmailID = AllReqs.Select(m => m.EmailID).FirstOrDefault();
            foreach (var items in Orgs)
            {
                var Org = dbcontext.Organization.Find(items);
                var LeadIDs = GetLeadIDs(EmailID, items);
                DataContext Spdb = new DataContext(Org.DatabaseName);
                foreach (var ID in LeadIDs)
                {
                    var Policies = (from c in Spdb.WalletPolicies.Where(m => m.LeadID == ID && m.Document != null && m.IsPosted == true)
                                    select c).ToList();
                    Policies.ToList().ForEach(m => m.OrgName = Org.Name);
                    AllPolicies.AddRange(Policies);
                }
            }
            return AllPolicies;
            //DataContext Spdb=new DataContext();
            //var list = Spdb.WalletPolicies.Where(m => m.Document != null).ToList();
            //return list;
        }
        public string GetPolicyByID(string Policy, string sDatabase)
        {
            var Details = Policy.Split('-').ToList();
            var Org = dbcontext.Organization.Find(Convert.ToInt32(Details[1]));
            DataContext Spdb = new DataContext(Org.DatabaseName);
            var PolicyDetails = Spdb.WalletPolicies.Find(Convert.ToInt32(Details[0]));
            var Template = Spdb.ContentEditors.Find(PolicyDetails.TemplateID);
            Common Com = new Common();
            var Content = Com.ReplaceTemplateContent(Template, Convert.ToInt32(Details[0]), EnumLeadTables.WalletPolicies.ToString(), sDatabase);
            return Content;
        }
        public DTResponse GetWalletPoliciesList(jQueryDataTableParamModel param, int OrgID, string database)
        {
            IEnumerable<WalletPolicies> AllTabs;
            DataContext Spdb = new DataContext(database);
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            //if (!string.IsNullOrEmpty(param.sSearch))
            //{
            //    FilteredTabs = Spdb.WalletPolicies.Where(m => m.OrganizationID == OrgID).Where(m => m.Name.Contains(param.sSearch.ToUpper())).ToList();
            //    AllTabs = FilteredTabs.OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            //    displyCount = FilteredTabs.Count();
            //}
            //else
            //{
            displyCount = Spdb.WalletPolicies.OrderBy(m => m.ID).Count();
            AllTabs = Spdb.WalletPolicies.OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

            //}
            var result = from c in AllTabs

                         select new[] {
                             (i++).ToString(),Convert.ToString(c.ID), c.PolicyName, c.ProductName, c.Type.ToString(), Convert.ToString(c.ToDate), c.Status, ""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        public VMCustomResponse SaveWalletPolicy(WalletPolicies Policy, int OrgID, string database)//ILOCKER
        {
            if (Policy.ID == 0)
            {
                DataContext Spdb = new DataContext(database);
                WalletPolicies Pol = new WalletPolicies();
                Pol.PolicyName = Policy.PolicyName;
                Pol.Type = Policy.Type;
                Pol.ProductType = Policy.ProductType;
                Pol.ProductID = Policy.ProductID;
                Pol.LeadID = Policy.LeadID;
                Pol.OrganizationID = Policy.OrganizationID;
                var prodname = Spdb.WalletProducts.Find(Policy.ProductID);
                Pol.ProductName = prodname.Name;
                Pol.Status = Policy.Status;
                Pol.Notes = Policy.Notes;
                Pol.GrossPremium = Policy.GrossPremium;
                Pol.Commission = Policy.Commission;
                Pol.Charges = Policy.Charges;
                Pol.AddOnCommission = Policy.AddOnCommission;
                Pol.AddOnCharges = Policy.AddOnCharges;
                Pol.PolicySetupCharges = Policy.PolicySetupCharges;
                Pol.FromDate = Convert.ToDateTime(Policy.FromDate);
                Pol.ToDate = Convert.ToDateTime(Policy.ToDate);
                Pol.PurchaseDate = DateTime.Now;
                Pol.Date = DateTime.Now;
                var brokername = dbcontext.Organization.Find(OrgID);
                Pol.BrokerName = brokername.Name;
                Pol.TemplateID = Policy.TemplateID;
                Spdb.WalletPolicies.Add(Pol);
                Spdb.SaveChanges();
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = Pol.ID, Status = true };
            }
            else
            {
                DataContext Spdb = new DataContext(database);
                WalletPolicies Pol = Spdb.WalletPolicies.Find(Policy.ID);
                Pol.PolicyName = Policy.PolicyName;
                Pol.Type = Policy.Type;
                Pol.ProductType = Policy.ProductType;
                Pol.ProductID = Policy.ProductID;
                var prodname = Spdb.WalletProducts.Find(Policy.ProductID);
                Pol.ProductName = prodname.Name;
                Pol.Status = Policy.Status;
                Pol.Notes = Policy.Notes;
                Pol.GrossPremium = Policy.GrossPremium;
                Pol.Commission = Policy.Commission;
                Pol.Charges = Policy.Charges;
                Pol.AddOnCommission = Policy.AddOnCommission;
                Pol.AddOnCharges = Policy.AddOnCharges;
                Pol.PolicySetupCharges = Policy.PolicySetupCharges;
                Pol.ToDate = Policy.ToDate; Pol.FromDate = Policy.FromDate;
                var brokername = dbcontext.Organization.Find(OrgID);
                Pol.BrokerName = brokername.Name;
                Pol.TemplateID = Policy.TemplateID;
                Spdb.SaveChanges();
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = Pol.ID, Status = true };
            }
        }
        public int SavePolicyImageOrDoc(int ID, string Name, string Type, int OrgID, string database)
        {
            DataContext Spdb = new DataContext(database);
            WalletPolicies Product = Spdb.WalletPolicies.Find(ID);
            if (Type == "Image")
            {
                Product.Image = Name;
            }
            else
            {
                Product.Document = Name;
            }
            Spdb.SaveChanges();
            return ID;
        }
        public List<WalletPolicies> GetAllPolicies(int OrgID, string database)
        {
            DataContext Spdb = new DataContext(database);
            List<WalletPolicies> Policies = Spdb.WalletPolicies.ToList();
            return Policies;
        }
        public WalletPolicies EditWalletPolicy(int ID, int OrgID, string database)
        {
            DataContext Spdb = new DataContext(database);
            var Policy = Spdb.WalletPolicies.Find(ID);
            return Policy;
        }
        private List<int> GetLeadIDs(string EmailID, int OrgID)
        {
            List<int> IDs = new List<int>();
            var Org = dbcontext.Organization.Find(OrgID);
            using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("", Con);
                cmd.CommandText = "Select ID from " + EnumLeadTables.Leads.ToString() + " where sEmail='" + EmailID + "' and FKiOrgID=" + OrgID;
                Con.Open();
                Con.ChangeDatabase(Org.DatabaseName);
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable data = new DataTable();
                data.Load(reader);
                List<object[]> TotalResult = data.AsEnumerable().Select(m => m.ItemArray).ToList();
                IDs = (from c in TotalResult
                       select Convert.ToInt32(c[0])).ToList();
                Con.Dispose();
            }
            return IDs;
        }
        public bool IsExistsWalletPolicyName(string Name, int ID, int OrgID, string database)
        {
            DataContext Spdb = new DataContext(database);
            var policies = Spdb.WalletPolicies.Where(m => m.Status == "10").ToList();
            WalletPolicies pol = policies.Where(m => m.PolicyName.Equals(Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (ID == 0)
            {
                if (pol != null)
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
                if (pol != null)
                {
                    if (ID == pol.ID)
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
        public List<VMDropDown> GetProdList(int OrgID)
        {
            var database = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(database);
            List<VMDropDown> AllProds = new List<VMDropDown>();
            AllProds = (from c in Spdb.WalletProducts.Where(m => m.OrganizationID == OrgID)
                        select new VMDropDown { text = c.Name, Value = c.ID }).ToList();
            return AllProds;
        }
        public List<VMDropDown> ProductList(int iUserID, int OrgID, int ddlVal, string sDatabase, string sOrgName)
        {
            Common Com = new Common();
            List<VMDropDown> AllProducts = new List<VMDropDown>();
            AllProducts = Com.ProductList(iUserID, OrgID, ddlVal, sDatabase, sOrgName);
            return AllProducts;
        }

        #endregion Policies

        #region Documents

        public DTResponse GetRecievedDocuments(string Type, jQueryDataTableParamModel param)
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
            List<WalletDocuments> AllDocs = new List<WalletDocuments>();
            List<WalletDocuments> FilteredDocs = new List<WalletDocuments>();
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            var AllReqs = dbcontext.WalletRequests.Where(m => m.ClientID == param.ClientID).ToList();
            var Orgs = AllReqs.Select(m => m.OrganizationID).Distinct().ToList();
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                foreach (var items in Orgs)
                {
                    var database = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == items).Select(m => m.sDatabaseName).FirstOrDefault();
                    DataContext Spdb = new DataContext(database);
                    FilteredDocs = Spdb.WalletDocuments.Where(m => m.ClientID == param.ClientID && m.OrganizationID == items).Where(m => m.DocumentName.Contains(param.sSearch.ToUpper())).ToList();
                    AllDocs.AddRange(FilteredDocs.OrderByDescending(m => m.ID).ToList());
                    displyCount = displyCount + FilteredDocs.Count();
                }
                AllDocs = AllDocs.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            }
            else
            {
                foreach (var items in Orgs)
                {
                    var database = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == items).Select(m => m.sDatabaseName).FirstOrDefault();
                    DataContext Spdb = new DataContext(database);
                    displyCount = displyCount + Spdb.WalletDocuments.Where(m => m.ClientID == param.ClientID && m.OrganizationID == items && m.Type == DocType).Count();
                    AllDocs.AddRange(Spdb.WalletDocuments.Where(m => m.ClientID == param.ClientID && m.OrganizationID == items && m.Type == DocType).OrderByDescending(m => m.ID).ToList());
                }
                AllDocs = AllDocs.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            }
            var result = (from cd in AllDocs
                          select new[] {
                             (i++).ToString(), Convert.ToString(cd.ID), cd.OrganizationID.ToString(), cd.OriginalName, GetOrgName(cd.OrganizationID), cd.DocumentName, cd.Message, cd.UploadedOn.ToString("dd MMM yyyy"), cd.StatusTypeID.ToString(),""}).ToList();

            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        public WalletDocuments SaveClientDocument(string ClientID, WalletDocuments Document)
        {
            int latestID = 0;
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
                Doc.Type = 10;
                Spdb.WalletDocuments.Add(Doc);
                Spdb.SaveChanges();
                //Adding Item to the Inbox
                WalletMessages msg = new WalletMessages();
                msg.ParentID = null;
                msg.Sender = 1; //ask
                msg.ClientID = Doc.ClientID;
                msg.OrganizationID = Doc.OrganizationID;
                msg.Type = "Email";//ask (10 = Policy ;  20 = Quote)
                msg.Subject = null;
                msg.Message = Doc.Message;
                msg.Icon = null;
                msg.Importance = 0;
                msg.ReceivedOn = DateTime.Now;
                var record = dbcontext.Organization.Find(Doc.OrganizationID);
                msg.OrganizationName = record.Name;
                msg.IsRead = false;
                msg.MailType = null;
                msg.ProductID = 0;
                Spdb.WalletMessages.Add(msg);
                Spdb.SaveChanges();
                latestID = Spdb.WalletMessages.Max(u => u.ID);
                return Doc;
            }
            else
            {
                WalletDocuments Doc = Spdb.WalletDocuments.Find(Document.ID);
                Doc.DocumentName = Document.DocumentName;
                if (latestID != 0)
                {
                    WalletMessages Msg = Spdb.WalletMessages.Find(latestID);
                    Msg.Attachments = Document.DocumentName;
                }//Handle NULL if DocumentName=NULL.......
                Spdb.SaveChanges();
                return Doc;
            }
        }
        private string GetOrgName(int p)
        {
            var OrgName = dbcontext.Organization.Where(m => m.ID == p).Select(m => m.Name).FirstOrDefault();
            return OrgName;
        }

        #endregion Documents

        #region Orders
        //https://stackoverflow.com/questions/7332920/the-specified-linq-expression-contains-references-to-queries-that-are-associated
        public DTResponse GetPurhasesList(jQueryDataTableParamModel param)
        {
            IEnumerable<VMWalletOrders> AllDocs;
            List<VMWalletOrders> AllFilteredDocs = new List<VMWalletOrders>();
            List<VMWalletOrders> FilteredDocs = new List<VMWalletOrders>();

            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            var AllReqs = dbcontext.WalletRequests.Where(m => m.ClientID == param.ClientID).ToList();
            var Orgs = AllReqs.Select(m => m.OrganizationID).Distinct().ToList();
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                foreach (var items in Orgs)
                {
                    var orgname = dbcontext.Organization.Where(m => m.ID == items).Select(m => m.Name).FirstOrDefault();

                    var database = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == items).Select(m => m.sDatabaseName).FirstOrDefault();
                    DataContext Spdb = new DataContext(database);
                    displyCount = displyCount + Spdb.WalletOrders.Where(m => m.ClientID == param.ClientID && m.OrganizationID == items).Count();
                    AllDocs = (from n in Spdb.WalletOrders.Where(m => m.ClientID == param.ClientID && m.OrganizationID == items)
                               join p in Spdb.WalletProducts.Where(m => m.OrganizationID == items) on n.ProductID equals p.ID
                               orderby n.ID descending
                               select new VMWalletOrders
                               {
                                   ID = p.ID,
                                   ProductID = n.ProductID,
                                   ClientID = n.ClientID,
                                   OrderedOn = n.OrderedOn,
                                   StatusTypeID = n.StatusTypeID,
                                   OrganizationID = n.OrganizationID,
                                   EmailID = n.EmailID,
                                   FromDate = n.FromDate,
                                   ToDate = n.ToDate,
                                   Type = n.Type,
                                   ProductName = p.Name,
                               }).ToList();
                    AllDocs.ToList().ForEach(m => m.OrganizationName = orgname);
                    AllDocs = from n in AllDocs.Where(m => m.ProductName.ToUpper().Contains(param.sSearch.ToUpper())) select n;
                    AllFilteredDocs.AddRange(AllDocs);
                }
                AllFilteredDocs = AllFilteredDocs.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            }
            else
            {
                foreach (var items in Orgs)
                {
                    var orgname = dbcontext.Organization.Where(m => m.ID == items).Select(m => m.Name).FirstOrDefault();

                    var database = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == items).Select(m => m.sDatabaseName).FirstOrDefault();
                    DataContext Spdb = new DataContext(database);
                    displyCount = displyCount + Spdb.WalletOrders.Where(m => m.ClientID == param.ClientID && m.OrganizationID == items).Count();
                    AllDocs = (from n in Spdb.WalletOrders.Where(m => m.ClientID == param.ClientID && m.OrganizationID == items)
                               join p in Spdb.WalletProducts.Where(m => m.OrganizationID == items) on n.ProductID equals p.ID
                               orderby n.ID descending
                               select new VMWalletOrders
                               {
                                   ID = p.ID,
                                   ProductID = n.ProductID,
                                   ClientID = n.ClientID,
                                   OrderedOn = n.OrderedOn,
                                   StatusTypeID = n.StatusTypeID,
                                   OrganizationID = n.OrganizationID,
                                   EmailID = n.EmailID,
                                   FromDate = n.FromDate,
                                   ToDate = n.ToDate,
                                   Type = n.Type,
                                   ProductName = p.Name,
                               }).ToList();
                    AllDocs.ToList().ForEach(m => m.OrganizationName = orgname);
                    AllFilteredDocs.AddRange(AllDocs);
                }
                AllFilteredDocs = AllFilteredDocs.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            }
            var result = (from cd in AllFilteredDocs
                          select new[] {
                             (i++).ToString(),
                             Convert.ToString(cd.ID),
                             cd.OrganizationID.ToString(),
                             cd.OrganizationName,
                             cd.Type.ToString(),
                             cd.ProductName,
                             cd.ClientID,
                             cd.EmailID,
                             cd.OrderedOn.ToString("dd MMM yyyy"),
                             cd.StatusTypeID.ToString(),
                             cd.FromDate.ToString("dd MMM yyyy"),
                             cd.ToDate.ToString("dd MMM yyyy"),
                             ""}).ToList();

            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        //mm
        public DTResponse dtGetAllMails(jQueryDataTableParamModel param)
        {
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;


            List<VMMessages> AllMails = new List<VMMessages>();
            List<WalletMessages> Mails = new List<WalletMessages>();
            var AllReqs = dbcontext.WalletRequests.Where(m => m.ClientID == param.ClientID).Select(m => m.OrganizationID).Distinct().ToList();
            foreach (var Orgs in AllReqs)
            {
                var Org = dbcontext.Organization.Find(Orgs);
                DataContext Spdb = new DataContext(Org.DatabaseName);
                Mails = Spdb.WalletMessages.Where(m => m.ClientID == param.ClientID && m.OrganizationID == Orgs).ToList();
                foreach (var items in Mails)
                {
                    var mainID = items.ID;
                    var val = Mails.Where(m => m.ParentID == mainID).ToList();
                    if (val.Count() > 0)
                    {

                    }
                    else
                    {
                        VMMessages Msg = new VMMessages();
                        Msg.ID = items.ID;
                        Msg.Message = items.Message;
                        Msg.Subject = items.Subject;
                        Msg.ReceivedOn = items.ReceivedOn;
                        Msg.OrganizationName = Org.Name;
                        Msg.Sender = items.Sender;
                        Msg.OrganizationID = items.OrganizationID;
                        Msg.IsRead = items.IsRead;
                        Msg.Attachments = items.Attachments;
                        Msg.PageSize = 5;
                        Msg.Total = Mails.Count;
                        AllMails.Add(Msg);
                    }
                }

            }
            var result = (from cd in AllMails
                          select new[] {
                             "",
                             "",
                             "",
                             "",
                             Convert.ToString(cd.ID),
                             cd.Message,
                             cd.Subject,
                             cd.ReceivedOn.ToString("dd MMM yyyy"),
                             cd.OrganizationName,
                             cd.Sender.ToString(),
                             cd.OrganizationID.ToString(),
                             cd.IsRead.ToString(),
                             cd.Attachments
                             }).ToList();

            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        //mm
        public DTResponse GetOrdersList(jQueryDataTableParamModel param, int OrgID, string database)
        {
            IEnumerable<WalletOrders> AllOrders, FilteredOrders;
            DataContext Spdb = new DataContext(database);
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredOrders = Spdb.WalletOrders.Where(m => m.OrganizationID == OrgID).ToList();
                AllOrders = FilteredOrders.OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = FilteredOrders.Count();
            }
            else
            {
                displyCount = Spdb.WalletOrders.Where(m => m.OrganizationID == OrgID).OrderByDescending(m => m.ID).Count();
                AllOrders = Spdb.WalletOrders.Where(m => m.OrganizationID == OrgID).OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

            }
            var result = from c in AllOrders
                         join s in Spdb.WalletProducts on c.ProductID equals s.ID
                         select new[] {
                             (i++).ToString(),Convert.ToString(c.ID), c.OrganizationID.ToString(), c.EmailID, s.Name, c.OrderedOn.ToString(), Convert.ToString(c.StatusTypeID)};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        public WalletOrders AddWalletOrder(WalletOrders model)
        {
            var database = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == model.OrganizationID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(database);
            model.FromDate = DateTime.Now;
            model.ToDate = DateTime.Now.AddMonths(1);
            Spdb.WalletOrders.Add(model);
            Spdb.SaveChanges();
            var EmailID = dbcontext.WalletRequests.Where(m => m.ClientID == model.ClientID).Select(m => m.EmailID).FirstOrDefault();
            WalletMessages Msg = new WalletMessages();
            if (model.Type == 10)
            {
                Msg.Subject = "Product has been accepted";
                Msg.Message = "Product has been accepted";
            }
            else if (model.Type == 20)
            {
                Msg.Subject = "Quote has been accepted";
                Msg.Message = "Quote has been accepted";
            }
            else
            {
                Msg.Subject = "Order has been placed";
                Msg.Message = "Order has been placed";
            }
            Msg.EmailID = EmailID;
            Msg.Type = "Email";
            Msg.Sender = 1;
            Msg.ProductID = model.ProductID;
            Msg.ClientID = model.ClientID;
            Msg.ReceivedOn = DateTime.Now;
            Msg.IsRead = false;
            Msg.OrganizationID = model.OrganizationID;
            Msg.MailType = "Message";
            SendMessageToBroker(Msg);
            return model;
        }
        public WalletMessages SendMessageToBroker(WalletMessages model)
        {
            var Org = dbcontext.Organization.Find(model.OrganizationID);
            DataContext Spdb = new DataContext(Org.DatabaseName);
            WalletMessages Mail = new WalletMessages();
            if (model.ID == 0)
            {
                Mail.ClientID = model.ClientID;
                Mail.Sender = model.Sender;
                if (model.ParentID == 0)
                    Mail.ParentID = null;
                else
                    Mail.ParentID = model.ParentID;
                Mail.Subject = model.Subject;
                Mail.MailType = model.MailType;
                Mail.Message = model.Message;
                Mail.Importance = model.Importance;
                Mail.Icon = model.Icon;
                Mail.ReceivedOn = DateTime.Now;
                Mail.OrganizationID = model.OrganizationID;
                Spdb.WalletMessages.Add(Mail);
                Spdb.SaveChanges();
                return Mail;
            }
            else
            {
                Mail = Spdb.WalletMessages.Find(model.ID);
                Mail.Attachments = model.Attachments;
                Spdb.SaveChanges();
                return Mail;
            }

        }

        #endregion Orders

        #region Quotes

        public DTResponse GetWalletQuotesList(jQueryDataTableParamModel param, int OrgID, string database)
        {
            IEnumerable<WalletQuotes> AllQuotes, FilteredQuotes;
            DataContext Spdb = new DataContext(database);
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredQuotes = Spdb.WalletQuotes.Where(m => m.OrganizationID == OrgID).Where(m => m.Name.Contains(param.sSearch.ToUpper())).ToList();
                AllQuotes = FilteredQuotes.OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = FilteredQuotes.Count();
            }
            else
            {
                displyCount = Spdb.WalletQuotes.Where(m => m.OrganizationID == OrgID).OrderBy(m => m.Name).Count();
                AllQuotes = Spdb.WalletQuotes.Where(m => m.OrganizationID == OrgID).OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

            }
            var result = from c in AllQuotes
                         join s in Spdb.OrganizationClasses on c.ClassID equals s.ClassID
                         select new[] {
                             (i++).ToString(),Convert.ToString(c.ID), c.OrganizationID.ToString(), c.Name, Convert.ToString(c.QuoteValidTo), s.Class, c.Status, ""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        public bool IsExistsWalletQuoteName(string Name, int ID, int OrgID, string database)
        {
            DataContext Spdb = new DataContext(database);
            var Quotes = Spdb.WalletQuotes.Where(m => m.OrganizationID == OrgID).ToList();
            WalletQuotes Quote = Quotes.Where(m => m.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (ID == 0)
            {
                if (Quote != null)
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
                if (Quote != null)
                {
                    if (ID == Quote.ID)
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
        public VMCustomResponse AddWalletQuote(WalletQuotes model, int OrgID, string database)
        {
            if (model.ID == 0)
            {
                DataContext Spdb = new DataContext(database);
                WalletQuotes Quote = new WalletQuotes();
                Quote.Name = model.Name;
                Quote.Type = model.Type;
                Quote.Product = model.Product;
                Quote.OrganizationID = model.OrganizationID;
                Quote.Content = model.Content;
                Quote.ClassID = model.ClassID;
                Quote.QuoteValidTo = model.QuoteValidTo;
                Quote.GrossPremium = model.GrossPremium;
                Quote.Commission = model.Commission;
                Quote.AdminCharges = model.AdminCharges;
                Quote.AddOnCharges = model.AddOnCharges;
                Quote.AddOnCommission = model.AddOnCommission;
                Quote.PolicySetupCharges = model.PolicySetupCharges;
                Quote.Income = model.Income;
                Quote.FinancialQuote = model.FinancialQuote;
                Quote.Notes = model.Notes;
                Quote.SentBy = "Admin";
                Quote.SentOn = DateTime.Now;
                Quote.Status = model.Status;
                Spdb.WalletQuotes.Add(Quote);
                Spdb.SaveChanges();
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = Quote.ID, Status = true };
            }
            else
            {
                DataContext Spdb = new DataContext(database);
                WalletQuotes Quote = Spdb.WalletQuotes.Find(model.ID);
                Quote.Name = model.Name;
                Quote.Product = model.Product;
                Quote.OrganizationID = model.OrganizationID;
                Quote.Content = model.Content;
                Quote.ClassID = model.ClassID;
                Quote.QuoteValidTo = model.QuoteValidTo;
                Quote.GrossPremium = model.GrossPremium;
                Quote.Commission = model.Commission;
                Quote.AdminCharges = model.AdminCharges;
                Quote.AddOnCharges = model.AddOnCharges;
                Quote.AddOnCommission = model.AddOnCommission;
                Quote.PolicySetupCharges = model.PolicySetupCharges;
                Quote.Income = model.Income;
                Quote.FinancialQuote = model.FinancialQuote;
                Quote.Notes = model.Notes;
                Quote.SentBy = "Admin";
                Quote.SentOn = DateTime.Now;
                Quote.Type = model.Type;
                Quote.Status = model.Status;
                Spdb.SaveChanges();
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = Quote.ID, Status = true };
            }
        }
        public WalletQuotes GetWalletQuoteByID(int ID, int OrgID, string database)
        {
            DataContext Spdb = new DataContext(database);
            var Quote = Spdb.WalletQuotes.Find(ID);
            return Quote;
        }
        public int SaveQuoteImageOrDoc(int ID, string Name, string Type, int OrgID, string database)
        {
            DataContext Spdb = new DataContext(database);
            WalletQuotes Quote = Spdb.WalletQuotes.Find(ID);
            if (Type == "Image")
            {
                Quote.Image = Name;
            }
            else
            {
                Quote.Document = Name;
            }
            Spdb.SaveChanges();
            return ID;
        }

        #endregion Quotes

        #region Renewals

        public DTResponse GetRenewalsList(jQueryDataTableParamModel param)
        {
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            var res = ExpiryDateNotification(param.ClientID);
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var FilteredResult = res.Where(m => m.ProductName.ToUpper().Contains(param.sSearch.ToUpper()));
                var result = (from cd in FilteredResult
                              select new[] {
                             (i++).ToString(),
                             Convert.ToString(cd.ID),
                             cd.OrganizationID.ToString(),
                             cd.OrganizationName,
                             cd.Type.ToString(),
                             cd.ProductName,
                             cd.DaysRemaining.ToString(),
                             cd.ClientID,
                             cd.EmailID,
                             cd.OrderedOn.ToString("dd MMM yyyy"),
                             cd.FromDate.ToString("dd MMM yyyy"),
                             cd.ToDate.ToString("dd MMM yyyy"),
                             cd.StatusTypeID.ToString(),
                             ""}).ToList();
                return new DTResponse()
                {
                    sEcho = param.sEcho,
                    iTotalRecords = displyCount,
                    iTotalDisplayRecords = displyCount,
                    aaData = result
                };
            }
            else
            {
                var result = (from cd in res
                              select new[] {
                             (i++).ToString(),
                             Convert.ToString(cd.ID),
                             cd.OrganizationID.ToString(),
                             cd.OrganizationName,
                             cd.Type.ToString(),
                             cd.ProductName,
                             cd.DaysRemaining.ToString(),
                             cd.ClientID,
                             cd.EmailID,
                             cd.OrderedOn.ToString("dd MMM yyyy"),
                             cd.FromDate.ToString("dd MMM yyyy"),
                             cd.ToDate.ToString("dd MMM yyyy"),
                             cd.StatusTypeID.ToString(),
                             ""}).ToList();
                return new DTResponse()
                {
                    sEcho = param.sEcho,
                    iTotalRecords = displyCount,
                    iTotalDisplayRecords = displyCount,
                    aaData = result
                };

            }



        }

        #endregion Renewals

        #region Miscellaneous

        public List<VMDropDown> GetOrgClasses(int OrgID)
        {
            List<VMDropDown> AllClasses = new List<VMDropDown>();
            AllClasses = ServiceUtil.GetOrgClasses(OrgID, "");
            return AllClasses;
        }
        private List<int> GetLeadClasses(string EmailID, int OrgID)
        {
            List<int> AllClasses = new List<int>();
            var Org = dbcontext.Organization.Find(OrgID);
            DataContext Spdb = new DataContext(Org.DatabaseName);
            AllClasses = (from c in Spdb.LeadClients.Where(m => m.OrganizationID == OrgID && m.Email == EmailID)
                          select c.ClassID).ToList();
            return AllClasses;
        }
        public List<VMDropDown> TemplatesList(int OrgID)
        {
            var Org = dbcontext.Organization.Find(OrgID);
            DataContext Spdb = new DataContext(Org.DatabaseName);
            List<VMDropDown> AllTemplates = new List<VMDropDown>();
            AllTemplates = (from c in Spdb.ContentEditors.Where(m => m.Category == 1 && m.OrganizationID == OrgID)
                            select new VMDropDown { text = c.Name, Value = c.ID }).ToList();
            return AllTemplates;
        }

        #endregion Miscellaneous

    }
}

