using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Repository
{
    public static class ServiceConstants
    {
        public const string DateFormat = "dd-MMM-yyyy";
        public const string DateTimeFormat = "dd-MMM-yyyy hh:mm";
        public const string DtTimeFormat = "yyyy-MM-dd HH:mm tt";
        public const string MonthName = "MMMM";
        public const string NoHiphenUserID = "";
        public const string SuperAdminRoleName = "SuperAdmin";
        public const string UserTable = "AspNetUsers";
        public const string DateSqlFormat = "yyyy-MM-dd";
        public const string SortExpression = "ID";
        public const string ClientDBName = "XIDNAOrg";
        public const string LeadImportingError = "Oops!! Looks like the data is Null or there might be errors in Uploaded file. Please check the file and Try again.";
        public const string LeadImportingException = "Intrupted!! Error while Importing data from the Uploaded file. Please check the file and Try again.";
        public const string ErrorMessageComptdColumn = "Error Computed Column";
        public const string SuccessMessageComptdColumn = "Success Computed Column";
        public const string DataTypeChangeSuccess = "Error Data Type";
        public const string DataTypeChangeError = "Success Data Type";
        public const int StatusTypeID = 10;


        //Status messages
        public const string SuccessMessage = "<strong>Success!</strong> Data Saved Successfully";
        public const string TableDosntExist = "<strong>Success!</strong> Data Saved Successfully";
        public const string ErrorMessage = "<strong>Failure!</strong> Error occured";
        public const string AlreadyExist = "Record Already Exist";
        public const string CommonPassword = "Admin.123";

        //Groups
        public const string SummaryGroup = "Summary";
        public const string LabelGroup = "Label";
        public const string ShowGroup = "Show Group";
        public const string SaveGroup = "Save Group";
        public const string DefaultGroup = "Default";
        public const string DescriptionGroup = "Description";
        public const string Details1Group = "Details1";
        public const string CreateGroup = "Create";
        public const string SearchGroup = "Search";
        public const string EditGroup = "Create";
        public const string ThemeColor = "#d0592e";
        public const string AutoLayoutHTML = "<div class=\"row\"><div class=\"col-md-12\" id=\"1\"></div></div>";
        public const string FormLayoutTemplate = @"<div class=""row""><div class=""col-md-12"" id=""1""></div></div>";
        //Auto Create Variables for popup
        public const string PopupLayoutName = "TabLayoutTemplate";
        public const string PopupTabArea = "PopupTabArea";
        public const string PopupOutputArea = "PopupTabContentArea";
        public const string PopupLeftPaneArea = "LeftPane";

        public const string FormLayoutName = "FormLayoutTemplate";
        public const string FormLayoutAtea = "FormContent";

        public const string Level1Layout = "Level1 Layout";
        public const string QSComponentHolder = "QSComponentHolder";

        //Component
        public const string OneClickComponent = "OneClickComponent";
        public const string FormComponent = "Form Component";
        public const string TreeStructureComponet = "XITreeStructure";
        public const string QSComponent = "QSComponent";
        public const string MappingComponent = "MappingComponent";
        public const string MultiRowComponent = "MultiRowComponent";

        //Left Tree Layout
        public const string LeftTreeLayout = "Left Tree Layout";
        public const string LeftTreeArea = "Tree Structure";
        public const string LeftTreeOutput = "LeftTreeOutput";
        public const string SummeryHolder = "SummeryHolder";

        //Sub Entity Layout
        public const string SubEntityLayout = "SubEntityLayout";
        public const string SubEntityListArea = "SubEntityListArea";
        public const string SubEntityDetailsArea = "SubEntityDetailsArea";
    }
}
