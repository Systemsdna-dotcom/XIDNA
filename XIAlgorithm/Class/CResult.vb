Public Class CResult

    Private sMyMessage As String = String.Empty
    Private sMyQuery As String = String.Empty
    Private oColNV As New Collection
    Private xiMyStatus As xiEnum.xiFuncResult   'Long
    Private oColResult As New Collection
    Private oMyResult As Object
    Private oMyEnvironment As Object

    Public Function LogToFile() As String

        Dim sResult As String = ""

        'OK, this is the simplest logging method, which is used to write errors down to a file, without knowing whether any other infrastructure is loaded
        '  so a simplle last ditch flat file system

        Try

            WriteLog(sMyMessage)
            sResult = ""

        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            sResult = "ERROR: [" & Get_Class() & "." & System.Reflection.MethodBase.GetCurrentMethod.Name & "] - " & ex.Message & " - Trace: " & ex.StackTrace & vbCrLf
        End Try


        Return sResult

    End Function

    Public Function LogToFile_Query() As String

        Dim sResult As String = ""

        'OK, this is the simplest logging method, which is used to write errors down to a file, without knowing whether any other infrastructure is loaded
        '  so a simplle last ditch flat file system

        Try

            WriteLog(sMyQuery, , "QUERY")
            sResult = ""

        Catch ex As Exception
            oMyResult.xiStatus = xiEnum.xiFuncResult.xiError
            sResult = "ERROR: [" & Get_Class() & "." & System.Reflection.MethodBase.GetCurrentMethod.Name & "] - " & ex.Message & " - Trace: " & ex.StackTrace & vbCrLf
        End Try


        Return sResult

    End Function

    Private Sub WriteLog(ByVal sMessage As String, Optional ByVal iType As Long = 0, Optional ByVal sSubLog As String = "")



        'Dim oLogger As System.IO.StreamWriter

        Dim sMyLogFile As String = "C:\XI.txt"
        Dim oFile As System.IO.FileStream
        Dim sSubLogDir As String = ""

        Try



            'sMyLogFile = "C:\XI.txt"      ' My.Settings.LogFile

            If Not oMyEnvironment Is Nothing Then
                If sSubLog <> "" Then
                    sSubLogDir = sSubLog & "\"
                    sSubLog = "." & sSubLog

                End If

                sMyLogFile = oMyEnvironment.sLogPath & sSubLogDir & oMyEnvironment.sAppName & sSubLog & "." & Format(Date.Today, "yyyy.MM.dd") & ".txt"

                If System.IO.Directory.Exists(oMyEnvironment.sLogPath & sSubLogDir) = False Then
                    Try
                        System.IO.Directory.CreateDirectory(oMyEnvironment.sLogPath & sSubLogDir)
                    Catch exDir As Exception
                        sMyLogFile = "C:\XI_no_Dir.txt"
                    End Try

                End If

                If System.IO.File.Exists(sMyLogFile) = False Then
                    Try
                        oFile = System.IO.File.Create(sMyLogFile)
                        oFile.Close()
                    Catch exCreate As Exception
                        'default to something
                        sMyLogFile = "C:\XI.txt"
                    End Try
                End If
            End If

            'If My.Settings.LogLevel <= iType Then    'so if loglevel is 5 and the type is 1 (not very important) it wont get written.

            'if loglevel is 0 then EVERYTHING is written

            Using w As System.IO.StreamWriter = System.IO.File.AppendText(sMyLogFile)

                Log(sMessage, w)

                ' Close the writer and underlying file.

                w.Close()

            End Using

            'End If



        Catch ex As Exception



        End Try



    End Sub



    Public Shared Sub Log(ByVal logMessage As String, ByVal w As System.IO.TextWriter)

        w.Write(ControlChars.CrLf & "Log Entry : ")

        w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString())

        w.WriteLine("  :")

        w.WriteLine("  :{0}", logMessage)

        w.WriteLine("-------------------------------")

        ' Update the underlying file.

        w.Flush()

    End Sub

    Public Property sMessage() As String
        Get
            Return sMyMessage
        End Get
        Set(ByVal value As String)
            sMyMessage = value
        End Set
    End Property

    Public Property sQuery() As String
        Get
            Return sMyQuery
        End Get
        Set(ByVal value As String)
            sMyQuery = value
        End Set
    End Property


    Public Property xiStatus() As xiEnum.xiFuncResult   ' Long
        Get
            Return xiMyStatus
        End Get
        Set(ByVal value As xiEnum.xiFuncResult)
            If xiMyStatus <> xiEnum.xiFuncResult.xiError Then
                'DO NOT ALLOW setting if already an error. Which could cause probs if re-using, but prevents overwrite
                xiMyStatus = value
            End If

        End Set
    End Property
    Public Property sAppend() As String

        Get

            Return sMyMessage

        End Get

        Set(ByVal value As String)

            sMessage = sMessage & value & vbCrLf

        End Set

    End Property
    Public Property oTraceStack() As Collection
        Get
            Return oColNV
        End Get
        Set(ByVal value As Collection)
            oColNV = value
        End Set
    End Property

    Public Property oResult() As Object
        Get
            Return oMyResult
        End Get
        Set(ByVal value As Object)
            oMyResult = value
        End Set
    End Property

    Public Property oCollectionResult() As Collection
        Get
            Return oColResult
        End Get
        Set(ByVal value As Collection)
            oColResult = value
        End Set
    End Property

    Public Property oEnvironment() As Object
        Get
            Return oMyEnvironment
        End Get
        Set(ByVal value As Object)
            oMyEnvironment = value
        End Set
    End Property

    Public Function Get_Class() As String


        Dim mb As System.Reflection.MethodBase
        Dim Type As Type
        Dim sFullReference As String = ""

        mb = System.Reflection.MethodBase.GetCurrentMethod
        Type = mb.DeclaringType

        sFullReference = Type.FullName

        Return sFullReference

    End Function
End Class
