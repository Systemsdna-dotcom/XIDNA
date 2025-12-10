namespace XIScriptEnum
{
    public enum xiDirty
    {
        xiNew = 0,
        xiClean = 10,
        xiDirty = 20,
    }
    public enum xiNodeCaching
    {

        xiCacheStasis = 0,

        xiCacheNone = 100,
    }
    public enum xiNodeAction
    {

        xiInsert = 0,

        xiUpdate = 10,

        xiView = 20,

        xiEdit = 30,

        xiLoad = 40,

        xiDelete = 100,
    }
    public enum xiNodeRelation
    {

        xiOneToMany = 0,

        xiManyToOne = 10,
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

        xiPostDelete = 105,
    }
    public enum xiGroupType
    {

        xiValues = 0,

        xiDynamic = 10,
    }
    public enum xiQryOperator
    {

        xiEQ = 0,

        xiGT = 10,

        xiGTEQ = 20,

        xiLT = 30,

        xiLTEQ = 40,

        xiLike = 100,

        xiStarts = 100,
    }
    public enum xiDBAction
    {

        xiDSInsert = 0,

        xiDSUpdate = 10,

        xiDSSelect = 20,

        xiDSDelete = 100,
    }
    public enum xiFuncResult
    {

        xiSuccess = 0,

        xiWarning = 10,

        xiError = 30,

        xiLogicalError = 40,

        xiInProcess = 100,
    }
    public enum xiCacheContents
    {

        xiAll = 0,

        xiSettings = 10,

        xiDataSources = 20,

        xiValueTypes = 30,

        xiNodeDefinitions = 40,
    }
    public enum xiLock
    {

        xiUnlock = 0,

        xiLock = 10,
    }
}
