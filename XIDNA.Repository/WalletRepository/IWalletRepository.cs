using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.ViewModels;
using XIDNA.Models;

namespace XIDNA.Repository
{
    public interface IWalletRepository
    {

        #region Requests
        int SaveWalletRequest(WalletRequests Request, string database);
        WalletRequests AcceptOrgRequest(WalletRequests model);
        List<VMRequests> GetAllRequests(string EmailID, int Count);
        string IsClientActivated(string ClientID);
        List<VMDropDown> GetAllLinkedOrgs(string ClientID);
        #endregion Requests

        #region Inbox
        List<VMMessages> GetAllMessages(string EmailID);
        List<VMMessages> GetMessageByID(string ID);
        WalletMessages SendMessageToBroker(WalletMessages Msg);
        VMMessages ChangeMessageStatus(string ID);
        List<WalletOrders> ExpiryDateNotification(string UserName);

        #endregion Inbox

        #region Products
        List<WalletProducts> GetWalletProducts(string ClientID);
        WalletProducts GetWalletProductByID(int ID, int OrgID);
        #endregion Products

        #region Quotes
        //  List<WalletQuotes> GetWalletQuotes();
        List<WalletQuotes> GetWalletQuotes(string ClientID);
        string GetQuoteByID(string QuoteID,string sDatabase);
        #endregion Quotes

        #region Policies
        List<WalletPolicies> GetWalletPolicies(string ClientID);
        string GetPolicyByID(string QuoteID,string sDatabase);
        List<WalletPolicies> GetPoliciesDocs(string ClientID);
        #endregion Policies
        #region Orders
        DTResponse GetPurhasesList(jQueryDataTableParamModel param);
        #endregion Orders
        #region Documents
        DTResponse GetRecievedDocuments(string Type, jQueryDataTableParamModel param);
        WalletDocuments SaveClientDocument(string ClientID, WalletDocuments Doc);
        #endregion Document

        #region Renewals
        DTResponse GetRenewalsList(jQueryDataTableParamModel param);
        #endregion Renewals
        List<VMDropDown> GetOrgClasses(int OrgID);
        VMCustomResponse AddWalletProdut(WalletProducts Product, int OrgID, string database);
        int SaveImageOrDoc(int ID, string Name, string Type, int OrgID, string database);
        bool IsExistsWalletProductName(string Name, int ID, int OrgID, string database);
        DTResponse GetWalletProductsList(jQueryDataTableParamModel param, int OrgID, string database);
        WalletProducts GetWalletProductByID(int ID, int OrgID, string database);

        WalletOrders AddWalletOrder(WalletOrders model);
        DTResponse GetOrdersList(jQueryDataTableParamModel param, int OrgID, string database);

        List<WalletProducts> GetProductDetails(int ProdID);//ILocker

        DTResponse GetWalletPoliciesList(jQueryDataTableParamModel param, int OrgID, string database);//1        
        VMCustomResponse SaveWalletPolicy(WalletPolicies Policy, int OrgID, string database);//2
        int SavePolicyImageOrDoc(int ID, string Name, string Type, int OrgID, string database);//3
        List<WalletPolicies> GetAllPolicies(int OrgID, string database);//4
        WalletPolicies EditWalletPolicy(int ID, int OrgID, string database);//5
        bool IsExistsWalletPolicyName(string Name, int ID, int OrgID, string database);//6
        List<VMDropDown> GetProdList(int OrgID);
        List<VMDropDown> ProductList(int iUserID, int OrgID, int ddlVal,string sDatabase, string sOrgName);
        DTResponse GetWalletQuotesList(jQueryDataTableParamModel param, int OrgID, string database);
        bool IsExistsWalletQuoteName(string Name, int ID, int OrgID, string database);
        int SaveQuoteImageOrDoc(int ID, string Name, string Type, int OrgID, string database);
        VMCustomResponse AddWalletQuote(WalletQuotes Product, int OrgID, string database);
        WalletQuotes GetWalletQuoteByID(int ID, int OrgID, string database);
        List<VMDropDown> TemplatesList(int OrgID);
        DTResponse dtGetAllMails(jQueryDataTableParamModel param);
    }
}
