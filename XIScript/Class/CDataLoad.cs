using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using XISystem;

namespace XIScript
{
    public class CDataLoad
    {

        // Public Function API_Load(ByVal Optional MethodName As String = "") As Object
        //     Dim physicalPath As String = System.Web.Hosting.HostingEnvironment.MapPath("~")
        //     Dim sPath As String = physicalPath.Substring(0, physicalPath.Length) & "\bin\XICore.dll"
        //     Dim StrPath As String = sPath
        //     Dim setup As AppDomainSetup = AppDomain.CurrentDomain.SetupInformation
        //     Dim newDomain As AppDomain = AppDomain.CreateDomain("newDomain", AppDomain.CurrentDomain.Evidence, setup)
        //     Dim _assembly As Assembly = Assembly.Load(AssemblyName.GetAssemblyName(StrPath))
        //     Dim Ltype As Type = Nothing
        //     Dim objclass As Object = Nothing
        //     For Each type As System.Type In _assembly.GetTypes()
        //         If String.Compare(type.Name, "XIAPI", True) = 0 Then
        //             Ltype = type
        //             objclass = _assembly.CreateInstance(type.FullName)
        //             Exit For
        //         End If
        //     Next
        //     Dim MyMethod As MethodInfo = Ltype.GetMethod(MethodName)
        //     Dim parametersArray As Object() = New Object() {}
        //     Dim Response As Object = CObj(MyMethod.Invoke(objclass, parametersArray))
        //     AppDomain.Unload(newDomain)
        //     Return Response
        // End Function
        public object API_Load(string MethodName = "", List<CNV> Params = null)
        {
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            string sPath = (physicalPath.Substring(0, physicalPath.Length) + "\\bin\\XICore.dll");
            string StrPath = sPath;
            object Response;
            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            AppDomain newDomain = AppDomain.CreateDomain("newDomain", AppDomain.CurrentDomain.Evidence, setup);
            Assembly _assembly = Assembly.Load(AssemblyName.GetAssemblyName(StrPath));
            Type Ltype = null;
            object objclass = null;
            foreach (System.Type type in _assembly.GetTypes())
            {
                if (string.Compare(type.Name, "XIAPI", true) == 0)
                {
                    Ltype = type;
                    objclass = _assembly.CreateInstance(type.FullName);
                    break;
                }

            }

            MethodInfo MyMethod = Ltype.GetMethod(MethodName);
            if (Params != null && Params.Count() > 0){
                object[] parametersArray = new object[] { Params };
                Response = MyMethod.Invoke(objclass, parametersArray);
            }
            else
            {
                object[] parametersArray = new object[0];
                Response = MyMethod.Invoke(objclass, parametersArray);
            }

            AppDomain.Unload(newDomain);
            return Response;
        }
    }
}