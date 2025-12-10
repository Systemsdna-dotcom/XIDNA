using XIDNA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.ViewModels;

namespace XIDNA.Repository
{
    public interface IFileRepository
    {
        DTResponse GetXIFileDetails(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase);
        XIFileTypes AddXIFileType(int ID, int iUserID, string sOrgName, string sDatabase);
        VMCustomResponse CreateFileSettings(XIFileTypes model, int iUserID, string sOrgName, string sDatabase);
        int DeleteFileDetails(int FileID, int iUserID, string sOrgName, string sDatabase);
        //Doc details
        DTResponse GetXIDocDetails(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase);
        XIDocTypes AddXIDocDetails(int ID, int iUserID, string sOrgName, string sDatabase);
        int DeleteDocDetails(int ID, int iUserID, string sOrgName, string sDatabase);
        VMCustomResponse CreateDocSettings(XIDocTypes model, int iUserID, string sOrgName, string sDatabase);
    }
}
