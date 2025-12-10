Namespace xiEnum

    Public Enum xiDirty
        xiNew = 0
        xiClean = 10
        xiDirty = 20

    End Enum

    Public Enum xiNodeCaching
        xiCacheStasis = 0
        xiCacheNone = 100
    End Enum

    Public Enum xiNodeAction
        xiInsert = 0
        xiUpdate = 10
        xiView = 20
        xiEdit = 30
        xiLoad = 40

        xiDelete = 100
    End Enum

    Public Enum xiNodeRelation
        xiOneToMany = 0
        xiManyToOne = 10
    End Enum

    Public Enum xiNodeActionInstance
        xiPreInsert = 0
        xiPostInsert = 5
        xiPreUpdate = 10
        xiPostUpdate = 15
        xiPreView = 20
        xiPostView = 25
        xiPreEdit = 30
        xiPostEdit = 35

        xiPreDelete = 100
        xiPostDelete = 105
    End Enum

    Public Enum xiGroupType
        xiValues = 0
        xiDynamic = 10
    End Enum

    Public Enum xiQryOperator
        xiEQ = 0
        xiGT = 10
        xiGTEQ = 20
        xiLT = 30
        xiLTEQ = 40

        xiLike = 100
        xiStarts = 100
    End Enum

    Public Enum xiDBAction
        xiDSInsert = 0
        xiDSUpdate = 10
        xiDSSelect = 20

        xiDSDelete = 100
    End Enum

    Public Enum xiFuncResult
        xiSuccess = 0
        xiWarning = 10
        xiError = 30
        xiLogicalError = 40
        xiInProcess = 100
    End Enum

    Public Enum xiCacheContents
        xiAll = 0
        xiSettings = 10
        xiDataSources = 20
        xiValueTypes = 30
        xiNodeDefinitions = 40
    End Enum

    Public Enum xiLock
        xiUnlock = 0
        xiLock = 10
    End Enum
End Namespace