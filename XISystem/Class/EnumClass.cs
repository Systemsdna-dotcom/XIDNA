namespace xiEnumSystem
{
    public enum xiDirty
    {
        xiNoChange = 0,
        xiChanged = 10
    }


    public enum xiNodeCaching
    {
        xiCacheStasis = 0,
        xiCacheNone = 100
    }

    public enum xiSaveAction
    {
        xiAny = 210,
        xiInsert = 220,
        xiUpdate = 230
    }

    public enum xiNodeAction
    {
        xiInsert = 0,
        xiUpdate = 10,
        xiView = 20,
        xiEdit = 30,
        xiLoad = 40,
        xiDelete = 100
    }

    public enum xiNodeRelation
    {
        xiOneToMany = 0,
        xiManyToOne = 10
    }

    public enum xiNodeActionInstance
    {
        xiPreInsert = 0,
        xiPostInsert = 5,
        xiPreUpdate = 10,
        xiPostUpdate = 15,
        xiPreView = 20,
        xiPostView = 25,
        xiPreEdit = 30,
        xiPostEdit = 35,
        xiPreDelete = 100,
        xiPostDelete = 105
    }

    public enum xiGroupType
    {
        xiValues = 0,
        xiDynamic = 10
    }

    public enum xiQryOperator
    {
        xiEQ = 0,
        xiGT = 10,
        xiGTEQ = 20,
        xiLT = 30,
        xiLTEQ = 40,
        xiNE = 50,
        xiStarts = 100,
        xiLike = 110,
        xiBT = 200,
        xiNULL = 300,
        xiNULLNO = 310
    }

    public enum xiDBAction
    {
        xiDSInsert = 0,
        xiDSUpdate = 10,
        xiDSSelect = 20,
        xiDSExecute = 30,
        xiDSDelete = 100
    }

    public enum xiQueryAction
    {
        xiQADefault = 0,
        xiQAList = 10,
        xiQAEdit = 20,
        xiQAView = 30,
        xiQASearch = 40,
        xiQAAll = 100,
        xiQACustom = 110
    }

    public enum xiFuncResult
    {
        xiSuccess = 0,
        xiWarning = 10,
        xiError = 30,
        xiLogicalError = 40,
        xiInProcess = 100
    }


    public enum xiTraceLevel
    {
        xiNone = 0,
        xiLog = 10,
        xiPerformance = 20
    }

    public enum xiCacheContents
    {
        xiAll = 0,
        xiSettings = 10,
        xiDataSources = 20,
        xiValueTypes = 30,
        xiNodeDefinitions = 40
    }

    public enum xiLock
    {
        xiUnlock = 0,
        xiLock = 10
    }

    public enum xiLoadType
    {
        xiLoadById = 0,
        xiLoadByParentId = 10
    }

    public enum xistatus
    {
        xiactive = 10,
        xiinactive = 20
    }

    public enum xiEnumPolicyLookupResponses
    {
        Normal, Refer, Decline
    }

    public enum xiSectionContent
    {
        EditForm = 10, Sections = 20, Fields = 30, XIComponent = 40, Html = 50, Bespoke = 60, OneClick = 70
    }

    public enum xi1ClcikDisplayAS
    {
        KPICircle = 10, PieChart = 20, BarChart = 30, LineChart = 40, ResultList = 50, Summary = 60, Bespoke = 70, ViewRecord = 80, EmailReport = 90, CustomQuery = 100, Grid = 110, Repeater = 120, 
        List = 130, Groupby = 150,Chat=160, StoredProcedure=170
    }
    public enum BODatatypes
    {
        NONE = 0,
        BIGINT = 10,
        BIT = 20,
        SMALLINT = 30,
        DECIMAL = 40,
        SMALLMONEY = 50,
        INT = 60,
        TINYINT = 70,
        MONEY = 80,
        FLOAT = 90,
        REAL = 100,
        DATE = 110,
        DATETIMEOFFSET = 120,
        DATETIME2 = 130,
        SMALLDATETIME = 140,
        DATETIME = 150,
        TIME = 160,
        CHAR = 170,
        VARCHAR = 180,
        TEXT = 190,
        NCHAR = 200,
        NVARCHAR = 210,
        NTEXT = 220,
        Password = 230,
        XIScript = 240,
        UniqueIdentifier = 250
    }

    public enum xiBOTypes
    {
        MasterEntity, Technical, Reference, XISystem, Enum, CacheReference
    }
    public enum Enumxicache
    {
        NoCache = 0,
        QueryCache = 10
    }
    public enum EnumFileExtensions
    {
        txt, pdf, jpg, jpeg, png, docx, csv
    }
    public enum EnumTransactionEnabled
    {
        Yes = 10, No = 20
    }
    public enum EnumTransactions
    {
        Live = 0, Corrected = 10, CorrectionTransaction = 20, ReplacementTransaction = 30, NotPosted = 40
    }
    public enum EnumRoles
    {
        XISuperAdmin, SuperAdmin, Admin, User, WebUsers, WMBeneficier
    }

    public enum EnumURLType
    {
        QuestionSet, Application, Client, Page, Organisation
    }

    public enum EnumReportNodeTypes
    {
        oneClick = 10, Aggregator = 20, Resolved = 30
    }
    public enum EnumPivot
    {
        Yes = 10, No = 20, None = 0
    }
    public enum EnumReportAppendTypes
    {
        Right = 10, Left = 20, Bottom = 30, None = 0
    }
    public enum EnumBOActions
    {
        XI = 10, ActionChain = 20, XIAlgorithm = 30, Script = 40, QuestionSet = 50, DefaultPopup = 60, ConfirmationDialog=70, XILink=80
    }

    public enum EnumMatrixAction
    {
        XILink = 10, OneClick = 20, ObjectAction = 30, ActionChain = 40, Algorithm = 50, Component = 60, ClientForgotPassword = 70, ClientLogin = 80, ClientCodeVerify = 90, ClientResetPassword = 100
    }

    public enum EnumXIMonitorAction
    {
        SignalR = 10, Email = 20, SMS = 30
    }

    public enum EnumXIErrorPriority
    {
        Low = 10, Medium = 20, Critical = 30
    }

    public enum EnumXIErrorCriticality
    {
        Info = 10, Exception = 20, RegulatoryFunction = 30
    }
    public enum EnumCommnicationType
    {
        email = 10, sms = 20, whatsapp = 30
    }
    public enum EnumEmailStatus
    {
        sent = 200, received = 210, closed = 230, processed = 10, dropped = 20, deferred = 30, bounce = 40, delivered = 50, open = 60, click = 70, spamreport = 80, unsubscribe = 90, replied = 100, deleted = 110
    }
    public enum EnumDistributionType
    {
        Switch = 10, Each = 20
    }
    public enum EnumPCStatus
    {
        Process = 10, PCError = 20, PCSuccess = 30
    }
}
