using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using XIDNA.ViewModels;
using System.Configuration;
using System.Reflection;
using System.Data;
using System.IO;

namespace XIDNA.Repository
{
    public class MasterRepository : IMasterRepository
    {
        CommonRepository Common = new CommonRepository();
        public DTResponse GetMasterDataList(jQueryDataTableParamModel param, int Type, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            int iOrgID = UserDetails.FKiOrgID;
            var FKiAppID = UserDetails.FKiApplicationID;
            IQueryable<Types> AllTypes;
            if (Type == 0)
            {
                AllTypes = dbContext.Types.Where(m => m.OrganisationID == iOrgID).Where(m => m.FKiApplicationID == FKiAppID);
            }
            else
            {
                AllTypes = dbContext.Types.Where(m => m.OrganisationID == iOrgID).Where(m => m.FKiApplicationID == FKiAppID).Where(m => m.Code == Type);
            }
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllTypes = AllTypes.Where(m => m.Name.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllTypes.Count();
            AllTypes = QuerableUtil.GetResultsForDataTables(AllTypes, "", sortExpression, param);
            var clients = AllTypes.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            if (Type == 0)
            {
                result = from c in clients
                         select new[] {
                             (i++).ToString(), Convert.ToString(c.ID), c.Name, c.Expression, c.Icon, Convert.ToString(c.Status),""  };
            }
            else
            {
                result = from c in clients
                         select new[] {
                             (i++).ToString(), Convert.ToString(c.ID), c.Name, c.Expression, c.Icon, Convert.ToString(c.Status) };
            }
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        public List<VMDropDown> GetAllNames(int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            int iOrgID = UserDetails.FKiOrgID;
            var FKiAppID = UserDetails.FKiApplicationID;
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> AllNames = new List<VMDropDown>();
            AllNames = (from c in dbContext.Types.Where(m => m.FKiApplicationID == FKiAppID).Where(m => m.OrganisationID == iOrgID).Where(m => m.Name == "Sys Type").ToList()
                        select new VMDropDown { text = c.Expression, Value = c.ID }).ToList();
            AllNames.Insert(0, new VMDropDown
            {
                text = "--Select--",
                Value = 0
            });
            return AllNames;
        }

        public int SaveMasterData(Types model, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            int iOrgID = UserDetails.FKiOrgID;
            var FKiAppID = UserDetails.FKiApplicationID;
            Types type = new Types();
            if (model.ID == 0)
            {

            }
            else
            {
                type = dbContext.Types.Find(model.ID);
            }

            if (model.Code == 0)
            {
                type.Name = "Sys Type";
                type.Code = 1;
            }
            else
            {
                var name = dbContext.Types.Where(m => m.ID == model.Code).Select(m => m.Expression).FirstOrDefault();
                type.Name = name.Replace("_", " ");
                type.Code = model.Code;
            }
            type.Expression = model.Expression;
            if (model.ID == 0)
            {
                var value = dbContext.Types.Where(m => m.Code == type.Code).ToList();
                if (value.Count() > 0)
                {
                    var max = value.Max(m => m.Value);
                    type.Value = max + 1;
                }
                else
                {
                    type.Value = 1;
                }
            }
            type.Icon = model.Icon;
            type.TypeID = model.TypeID;
            type.Status = model.Status;
            type.FKiApplicationID = FKiAppID;
            type.OrganisationID = iOrgID;
            if (model.ID == 0)
            {
                dbContext.Types.Add(type);
                dbContext.SaveChanges();
            }
            else
            {
                dbContext.SaveChanges();
            }
            return type.ID;
        }

        public int SaveMasterDataFile(int id, string FileName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            Types Master = new Types();
            Master = dbContext.Types.Find(id);
            Master.FileName = FileName;
            dbContext.SaveChanges();
            return id;
        }

        public Types EditMasterData(int DataID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            Types model = dbContext.Types.Find(DataID);
            return model;
        }

        public bool IsExistsDataName(string Exprssion, int ID, int Code, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var MasterData = dbContext.Types.ToList();
            if (Code == 0)
            {
                Code = dbContext.Types.Where(m => m.Expression == "Sys Type").ToList().Select(m => m.ID).FirstOrDefault();
            }
            Types Data = MasterData.Where(m => m.Code == Code).Where(m => m.Expression.Equals(Exprssion, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (ID == 0)
            {
                if (Data != null)
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
                if (Data != null)
                {
                    if (ID == Data.ID)
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

        public List<Types> GetTypeExpressions(int TypeID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var result = dbContext.Types.Where(m => m.Code == TypeID).ToList();
            return result;
        }
        public List<Types> GetMasterData(int OrgID, string database, int PageIndex)
        {
            int PageSize = 23;
            int skip = 0;
            if (PageIndex == 2)
            {
                skip = 23;
                PageSize = 10;
            }
            else if (PageIndex >= 2)
            {
                skip = PageSize + 10 * (PageIndex - 2);
                PageSize = 10;
            }
            if (skip == 0)
            {
                PageSize = 23;
            }
            ModelDbContext MdbContext = new ModelDbContext();
            var lTypes = new List<Types>();
            var sTabDetails = MdbContext.Types.OrderBy(x => x.ID).Skip(skip).Take(PageSize).ToList();
            foreach (var items in sTabDetails)
            {
                lTypes.Add(new Types
                {
                    ID = items.ID,
                    Code = items.Code,
                    TypeID = items.TypeID,
                    Name = items.Name,
                    Value = items.Value,
                    Expression = items.Expression,
                    Icon = items.Icon,
                    Status = items.Status
                });

            }
            return lTypes;
        }

    }
}
