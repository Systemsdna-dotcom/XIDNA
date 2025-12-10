using XIDNA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using XIDNA.ViewModels;
using System.Data;
using System.Net;

namespace XIDNA.Repository
{
    public class FileRepository : IFileRepository
    {
        CommonRepository Common = new CommonRepository();
        public DTResponse GetXIFileDetails(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            IQueryable<XIFileTypes> AllFileTypes;
            AllFileTypes = dbContext.XIFileTypes;
            string sortExpression = "ID";
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllFileTypes = AllFileTypes.Where(m => m.Name.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllFileTypes.Count();
            AllFileTypes = QuerableUtil.GetResultsForDataTables(AllFileTypes, "", sortExpression, param);
            var Files = AllFileTypes.ToList();
            for (int j = 0; j < Files.Count; j++)
            {
                int iType = Convert.ToInt32(Files[j].FileType);
                var sType = dbContext.XIDocTypes.Where(m => m.ID == iType).Select(m => m.Type).FirstOrDefault();
                Files[j].FileType = sType;
            }
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in Files
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.Name,c.sCount, Convert.ToString(c.Type),c.FileType, Convert.ToString(c.MaxWidth),Convert.ToString(c.MaxHeight),c.Thumbnails,c.Preview,c.Drilldown, c.StatusTypeID.ToString(),""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public XIFileTypes AddXIFileType(int ID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            var FileTypes = new List<VMDropDown>();
            XIFileTypes Files = new XIFileTypes();
            var FileDetails = dbContext.XIDocTypes.ToList();
            foreach (var item in FileDetails)
            {
                FileTypes.Add(new VMDropDown
                {
                    text = item.Type,
                    Value = item.ID
                }

                    );
            }
            if (ID > 0)
            {
                Files = dbContext.XIFileTypes.Find(ID);
                Files.FileTypes = FileTypes;
            }
            else
            {
                Files.FileTypes = FileTypes;
            }

            return Files;
        }

        public VMCustomResponse CreateFileSettings(XIFileTypes model, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            int iSuccess = 0;
            XIFileTypes File = new XIFileTypes();
            if (model.ID == 0)
            {
                File.Name = model.Name;
                File.sCount = model.sCount;
                if (model.sCount == "10")
                {
                    File.MaxCount = 1;
                }
                else
                {
                    File.MaxCount = model.MaxCount;
                }

                File.Type = model.Type;
                File.FileType = model.FileType;
                File.MaxWidth = model.MaxWidth;
                File.MaxHeight = model.MaxHeight;
                File.Thumbnails = model.Thumbnails;
                File.ThumbWidth = model.ThumbWidth;
                File.ThumbHeight = model.ThumbHeight;
                File.Preview = model.Preview;
                File.PreviewWidth = model.PreviewWidth;
                File.PreviewHeight = model.PreviewHeight;
                File.Drilldown = model.Drilldown;
                File.DrillWidth = model.DrillWidth;
                File.DrillHeight = model.DrillHeight;
                File.StatusTypeID = model.StatusTypeID;
                File.CreatedBy = File.UpdatedBy = 1;
                File.CreatedTime = File.UpdatedTime = DateTime.Now;
                File.CreatedBySYSID = File.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                dbContext.XIFileTypes.Add(File);
                dbContext.SaveChanges();
                iSuccess = 1;
            }
            else
            {
                File = dbContext.XIFileTypes.Find(model.ID);
                File.sCount = model.sCount;
                if (model.sCount == "Single")
                {
                    File.MaxCount = 1;
                }
                else
                {
                    File.MaxCount = model.MaxCount;
                }
                File.Type = model.Type;
                File.FileType = model.FileType;
                File.MaxWidth = model.MaxWidth;
                File.MaxHeight = model.MaxHeight;
                File.Thumbnails = model.Thumbnails;
                File.ThumbWidth = model.ThumbWidth;
                File.ThumbHeight = model.ThumbHeight;
                File.Preview = model.Preview;
                File.PreviewWidth = model.PreviewWidth;
                File.PreviewHeight = model.PreviewHeight;
                File.Drilldown = model.Drilldown;
                File.DrillWidth = model.DrillWidth;
                File.DrillHeight = model.DrillHeight;
                File.UpdatedBy = 1;
                File.UpdatedTime = DateTime.Now;
                File.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                File.StatusTypeID = model.StatusTypeID;
                dbContext.SaveChanges();
                iSuccess = 1;
            }
            if (iSuccess == 1)
            {
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = File.ID, Status = true };
            }
            else
            {
                return new VMCustomResponse { ResponseMessage = ServiceConstants.TableDosntExist, Status = false };
            }
        }

        public int DeleteFileDetails(int ID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            int iStatus = 0;
            try
            {
                XIFileTypes FileDetails = dbContext.XIFileTypes.Find(ID);
                dbContext.XIFileTypes.Remove(FileDetails);
                dbContext.SaveChanges();
                iStatus = 1;
            }
            catch (Exception ex)
            {
                iStatus = 0;
            }
            return iStatus;
        }

        //XI Doc Settings
        public DTResponse GetXIDocDetails(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            IQueryable<XIDocTypes> AllDocTypes;
            AllDocTypes = dbContext.XIDocTypes;
            string sortExpression = "ID";
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllDocTypes = AllDocTypes.Where(m => m.Type.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllDocTypes.Count();
            AllDocTypes = QuerableUtil.GetResultsForDataTables(AllDocTypes, "", sortExpression, param);
            var Docs = AllDocTypes.ToList();

            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in Docs
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.Type,c.Path,c.StatusTypeID.ToString(),""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public XIDocTypes AddXIDocDetails(int ID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            XIDocTypes Docs = new XIDocTypes();
            var DocDetails = dbContext.XIDocTypes.ToList();
            if (ID > 0)
            {
                Docs = dbContext.XIDocTypes.Find(ID);
            }
            else
            {

            }

            return Docs;
        }

        public int DeleteDocDetails(int ID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            int iStatus = 0;
            try
            {
                XIDocTypes DocDetails = dbContext.XIDocTypes.Find(ID);
                dbContext.XIDocTypes.Remove(DocDetails);
                dbContext.SaveChanges();
                iStatus = 1;
            }
            catch (Exception ex)
            {
                iStatus = 0;
            }
            return iStatus;
        }
        public VMCustomResponse CreateDocSettings(XIDocTypes model, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            int iSuccess = 0;
            XIDocTypes Docs = new XIDocTypes();
            string sNewPath = "";
            if ((model.Path).Contains("\\"))
            {
                sNewPath = model.Path.Replace("\\", "/");
            }
            else if ((model.Path).Contains(@"\"))
            {
                sNewPath = model.Path.Replace(@"\", "/");
            }
            {
                sNewPath = model.Path;
            }
            if (model.ID == 0)
            {
                Docs.Type = model.Type;
                Docs.Path = sNewPath;
                Docs.SubDirectory = model.SubDirectory;
                //  Docs.SubDirectoryPath = "";
                Docs.CreatedBy = Docs.UpdatedBy = 1;
                Docs.CreatedTime = Docs.UpdatedTime = DateTime.Now;
                Docs.CreatedBySYSID = Docs.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                Docs.StatusTypeID = model.StatusTypeID;
                dbContext.XIDocTypes.Add(Docs);
                dbContext.SaveChanges();
                iSuccess = 1;
            }
            else
            {
                Docs = dbContext.XIDocTypes.Find(model.ID);
                Docs.Type = model.Type;
                Docs.Path = sNewPath;
                Docs.UpdatedBy = 1;
                Docs.UpdatedTime = DateTime.Now;
                Docs.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                Docs.StatusTypeID = model.StatusTypeID;
                dbContext.SaveChanges();
                iSuccess = 1;
            }
            if (iSuccess == 1)
            {
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = Docs.ID, Status = true };
            }
            else
            {
                return new VMCustomResponse { ResponseMessage = ServiceConstants.TableDosntExist, Status = false };
            }
        }
    }
}
