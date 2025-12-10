Imports System.Reflection
Imports XISystem

Public Class CDataLoad
    'Public Function API_Load(ByVal Optional MethodName As String = "") As Object
    '    Dim physicalPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~")
    '    Dim sPath As String = physicalPath.Substring(0, physicalPath.Length) & "\bin\XICore.dll"
    '    Dim StrPath As String = sPath
    '    Dim setup As AppDomainSetup = AppDomain.CurrentDomain.SetupInformation
    '    Dim newDomain As AppDomain = AppDomain.CreateDomain("newDomain", AppDomain.CurrentDomain.Evidence, setup)

    '    Dim _assembly As Assembly = Assembly.Load(AssemblyName.GetAssemblyName(StrPath))
    '    Dim Ltype As Type = Nothing
    '    Dim objclass As Object = Nothing

    '    For Each type As System.Type In _assembly.GetTypes()

    '        If String.Compare(type.Name, "XIAPI", True) = 0 Then
    '            Ltype = type
    '            objclass = _assembly.CreateInstance(type.FullName)
    '            Exit For
    '        End If
    '    Next

    '    Dim MyMethod As MethodInfo = Ltype.GetMethod(MethodName)
    '    Dim parametersArray As Object() = New Object() {}
    '    Dim Response As Object = CObj(MyMethod.Invoke(objclass, parametersArray))
    '    AppDomain.Unload(newDomain)
    '    Return Response
    'End Function

    Public Function API_Load(ByVal Optional MethodName As String = "", ByVal Optional Params As List(Of CNV) = Nothing) As Object
        Dim physicalPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~")
        Dim sPath As String = physicalPath.Substring(0, physicalPath.Length) & "\bin\XICore.dll"
        Dim StrPath As String = sPath
        Dim Response As Object
        Dim setup As AppDomainSetup = AppDomain.CurrentDomain.SetupInformation
        Dim newDomain As AppDomain = AppDomain.CreateDomain("newDomain", AppDomain.CurrentDomain.Evidence, setup)
        Dim _assembly As Assembly = Assembly.Load(AssemblyName.GetAssemblyName(StrPath))
        Dim Ltype As Type = Nothing
        Dim objclass As Object = Nothing

        For Each type As System.Type In _assembly.GetTypes()

            If String.Compare(type.Name, "XIAPI", True) = 0 Then
                Ltype = type
                objclass = _assembly.CreateInstance(type.FullName)
                Exit For
            End If
        Next

        Dim MyMethod As MethodInfo = Ltype.GetMethod(MethodName)

        If Params IsNot Nothing AndAlso Params.Count() > 0 Then
            Dim parametersArray As Object() = New Object() {Params}
            Response = CObj(MyMethod.Invoke(objclass, parametersArray))
        Else
            Dim parametersArray As Object() = New Object() {}
            Response = CObj(MyMethod.Invoke(objclass, parametersArray))
        End If

        AppDomain.Unload(newDomain)
        Return Response
    End Function
End Class
