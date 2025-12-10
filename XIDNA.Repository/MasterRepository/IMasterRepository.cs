using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using XIDNA.ViewModels;

namespace XIDNA.Repository
{
    public interface IMasterRepository
    {
        DTResponse GetMasterDataList(jQueryDataTableParamModel param, int Type, int iUserID, string sOrgName, string sDatabase);
        List<VMDropDown> GetAllNames(int iUserID, string sOrgName, string sDatabase);
        int SaveMasterData(Types model, int iUserID, string sOrgName, string sDatabase);
        int SaveMasterDataFile(int id, string FileName, string sDatabase);
        Types EditMasterData(int DataID, string sDatabase);
        bool IsExistsDataName(string Expression, int ID, int Code, string sDatabase);
        List<Types> GetTypeExpressions(int TypeID, string sDatabase);
        List<Types> GetMasterData(int OrgID, string sDatabase, int PageIndex);
    }
}
