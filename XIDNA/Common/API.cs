using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using XIDNA.Models;

namespace XIDNA.Common
{
    public class API
    {
        public static List<T> GetList<T>(string path, string token = null)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["APIBaseUrl"].ToString());
            var req = new RestRequest(path, Method.GET);
            req.AddHeader(System.Net.HttpRequestHeader.Authorization.ToString(), string.Format("Bearer {0}", token));
            req.RequestFormat = DataFormat.Json;
            var res = client.Execute(req).Content;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<T> collection = serializer.Deserialize<List<T>>(res);
            return collection;
        }

        public static T PostGetList<T>(jQueryDataTableParamModel m, string path, string token = null)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["APIBaseUrl"].ToString());
            var req = new RestRequest(path, Method.POST);
            req.AddHeader(System.Net.HttpRequestHeader.Authorization.ToString(), string.Format("Bearer {0}", token));
            req.RequestFormat = DataFormat.Json;
            req.AddBody(m);
            var res = client.Execute(req).Content;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            T collection = serializer.Deserialize<T>(res);
            return collection;
        }
        public static string GetString(string path, string token = null)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["APIBaseUrl"].ToString());
            var req = new RestRequest(path, Method.POST);
            req.AddHeader(System.Net.HttpRequestHeader.Authorization.ToString(), string.Format("Bearer {0}", token));
            req.RequestFormat = DataFormat.Json;
            var res = client.Execute(req).Content;
            return res;
        }
        public static int Post(string path, string token = null)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["APIBaseUrl"].ToString());
            var req = new RestRequest(path, Method.POST);
            req.AddHeader(System.Net.HttpRequestHeader.Authorization.ToString(), string.Format("Bearer {0}", token));
            req.RequestFormat = DataFormat.Json;
            var res = client.Execute(req).Content;
            return Convert.ToInt32(res);
        }
        public static int Post<T>(T model, string path, string token = null)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["APIBaseUrl"].ToString());
            var req = new RestRequest(path, Method.POST);
            req.AddHeader(System.Net.HttpRequestHeader.Authorization.ToString(), string.Format("Bearer {0}", token));
            req.RequestFormat = DataFormat.Json;
            req.AddBody(model);
            var res = client.Execute(req).Content;
            return Convert.ToInt32(res);
        }
        public static int Postmodel<T>(T model, string path, string token = null)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["APIBaseUrl"].ToString());
            var req = new RestRequest(path, Method.POST);
            req.AddHeader(System.Net.HttpRequestHeader.Authorization.ToString(), string.Format("Bearer {0}", token));
            req.RequestFormat = DataFormat.Json;
            req.AddBody(model);
            var res = client.Execute(req).Content;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            int collection = serializer.Deserialize<int>(res);
            return collection;
        }
        public static List<T> PostmodelGetList<T>(List<int> model, string path, string token = null)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["APIBaseUrl"].ToString());
            var req = new RestRequest(path, Method.POST);
            req.AddHeader(System.Net.HttpRequestHeader.Authorization.ToString(), string.Format("Bearer {0}", token));
            req.RequestFormat = DataFormat.Json;
            req.AddBody(model);
            var res = client.Execute(req).Content;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<T> collection = serializer.Deserialize<List<T>>(res);
            return collection;
        }
        public static string PostListGetString<T>(List<T> model, string path, string token = null)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["APIBaseUrl"].ToString());
            var req = new RestRequest(path, Method.POST);
            req.AddHeader(System.Net.HttpRequestHeader.Authorization.ToString(), string.Format("Bearer {0}", token));
            req.RequestFormat = DataFormat.Json;
            req.AddBody(model);
            var res = client.Execute(req).Content;
            return res;
        }
        public static List<T> PostStringGetList<T>(string path, string token = null)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["APIBaseUrl"].ToString());
            var req = new RestRequest(path, Method.POST);
            req.AddHeader(System.Net.HttpRequestHeader.Authorization.ToString(), string.Format("Bearer {0}", token));
            req.RequestFormat = DataFormat.Json;
            var res = client.Execute(req).Content;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<T> collection = serializer.Deserialize<List<T>>(res);
            return collection;
        }
        public static T PostList<T>(T model, string path, string token = null)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["APIBaseUrl"].ToString());
            var req = new RestRequest(path, Method.POST);
            req.AddHeader(System.Net.HttpRequestHeader.Authorization.ToString(), string.Format("Bearer {0}", token));
            req.RequestFormat = DataFormat.Json;
            req.AddBody(model);
            var res = client.Execute(req).Content;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            T collection = serializer.Deserialize<T>(res);
            return collection;
        }
        public static int GetListForModel<T>(List<T> model, string path, string token = null)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["APIBaseUrl"].ToString());
            var req = new RestRequest(path, Method.POST);
            req.AddHeader(System.Net.HttpRequestHeader.Authorization.ToString(), string.Format("Bearer {0}", token));
            req.RequestFormat = DataFormat.Json;
            req.AddBody(model);
            return Convert.ToInt32(client.Execute(req).Content);
        }
        public static T GetModel<T>(string path, string token = null)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["APIBaseUrl"].ToString());
            var req = new RestRequest(path, Method.GET);
            req.AddHeader(System.Net.HttpRequestHeader.Authorization.ToString(), string.Format("Bearer {0}", token));
            var res = client.Execute(req).Content;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            T collection = serializer.Deserialize<T>(res);
            return collection;
        }
        public static bool IsExist<T>(T model, string path, string token = null)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["APIBaseUrl"].ToString());
            var req = new RestRequest(path, Method.POST);
            req.AddHeader(System.Net.HttpRequestHeader.Authorization.ToString(), string.Format("Bearer {0}", token));
            req.RequestFormat = DataFormat.Json;
            req.AddBody(model);
            var res = client.Execute(req).Content;
            return Convert.ToBoolean(res);
        }
        //public static Dictionary<int, string> Dictionary(string path, string token = null)
        //{
        //    var client = new RestClient();
        //    client.BaseUrl = new Uri(ConfigurationManager.AppSettings["APIBaseUrl"].ToString());
        //    var req = new RestRequest(path, Method.GET);
        //    req.RequestFormat = DataFormat.Json;
        //    req.AddHeader(System.Net.HttpRequestHeader.Authorization.ToString(), string.Format("Bearer {0}", token));
        //    var res = client.Execute(req).Content;
        //    JavaScriptSerializer serializer = new JavaScriptSerializer();
        //    Dictionary<int, string> list = new Dictionary<int, string>();
        //    foreach (var item in serializer.Deserialize<List<DropDownModel>>(res))
        //    {
        //        list.Add(item.Key, item.Value);
        //    }
        //    return list;
        //}
        public static Dictionary<int, string> DictionarySlot(string path, string token = null)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["APIBaseUrl"].ToString());
            var req = new RestRequest(path, Method.GET);
            req.RequestFormat = DataFormat.Json;
            req.AddHeader(System.Net.HttpRequestHeader.Authorization.ToString(), string.Format("Bearer {0}", token));
            var res = client.Execute(req).Content;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<int, string> list = JsonConvert.DeserializeObject<Dictionary<int, string>>(res); ;
            return list;
        }
    }
}