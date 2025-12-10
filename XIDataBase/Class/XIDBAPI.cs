using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using System.Data.Common;
using XISystem;

namespace XIDatabase
{
    public class XIDBAPI
    {
        [AttributeUsage(AttributeTargets.Property)]
        public class DapperKey : Attribute
        {
        }

        [AttributeUsage(AttributeTargets.Property)]
        public class DapperIgnore : Attribute
        {
        }

        public XIDBAPI()
        {

        }

        private readonly string _connectionString;

        public XIDBAPI(string connectionString)
        {
            _connectionString = connectionString;
        }

        static bool TestConnectionString<T>(string connectionString) where T : DbConnection, new()
        {
            using (DbConnection connection = new T())
            {
                connection.ConnectionString = connectionString;
                try
                {
                    connection.Open();
                    connection.Close();
                    return true;
                }
                catch (SqlException)
                {
                    return false;
                }
            }
        }

        public bool Test_ConnectionString<T>(string connectionString) where T : DbConnection, new()
        {
            using (DbConnection connection = new T())
            {
                connection.ConnectionString = connectionString;
                try
                {
                    connection.Open();
                    connection.Close();
                    return true;
                }
                catch (SqlException)
                {
                    return false;
                }
            }
        }

        protected SqlConnection GetOpenConnection()
        {
            var conn = TestConnectionString<SqlConnection>(_connectionString);
            if (conn)
            {
                var connection = new SqlConnection(_connectionString);
                connection.Open();
                return connection;
            }
            return null;
        }

        #region SQLAPI

        public IEnumerable<T>
            GetItems<T>(CommandType commandType, string sql, object parameters = null)
        {
            using (var connection = GetOpenConnection())
            {
                return connection.Query<T>(sql, parameters, commandType: commandType);
            }
        }

        public Dictionary<string, string> GetDDLItems(CommandType commandType, string sql, object parameters = null)
        {
            using (var connection = GetOpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = (SqlConnection)connection;
                cmd.CommandText = sql;
                SqlDataReader reader = cmd.ExecuteReader();
                Dictionary<string, string> FKDDL = new Dictionary<string, string>();
                while (reader.Read())
                {
                    if (reader.FieldCount > 1)
                    {
                        var text = reader.IsDBNull(0) ? null : reader.GetValue(0).ToString();
                        var Expression = reader.IsDBNull(1) ? null : reader.GetValue(1).ToString();
                        if (!string.IsNullOrEmpty(text))
                        {
                            FKDDL[text] = Expression;
                        }
                    }
                }
                return FKDDL;
            }
        }

        public string GetString(CommandType commandType, string sql, object parameters = null)
        {
            using (var connection = GetOpenConnection())
            {
                return connection.ExecuteScalar<string>(sql, parameters, commandType: commandType);
            }
        }
        protected int Execute(CommandType commandType, string sql, object parameters = null)
        {
            using (var connection = GetOpenConnection())
            {
                return connection.Execute(sql, parameters, commandType: commandType);
            }
        }

        //OLD ONE DONT USE FOR PARAMETERIZED QUERIES
        public object ExecuteReader(CommandType commandType, string sql, object parameters = null)
        {
            List<string[]> Rows = new List<string[]>();
            using (var connection = GetOpenConnection())
            {
                using (var reader = (SqlDataReader)((IWrappedDataReader)connection.ExecuteReader(sql)).Reader)
                {
                    DataTable data = new DataTable();
                    data.Load(reader);
                    return data;
                }
            }
        }

        public string GetTotalCount(CommandType commandType, string sql, List<SqlParameter> parameters)
        {
            using (var connection = GetOpenConnection())
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    parameters = parameters ?? new List<SqlParameter>();
                    if (parameters.Count() > 0)
                    {
                        List<SqlParameter> reInstantiate = parameters.Select(p => new SqlParameter { ParameterName = p.ParameterName, Value = p.Value }).ToList();
                        command.Parameters.AddRange(reInstantiate.ToArray());
                    }
                    var result = command.ExecuteScalar();
                    if (result == null)
                    {
                        result = 0;
                    }
                    return result.ToString();
                }
                //return connection.ExecuteScalar<string>(sql, parameters, commandType: commandType);
            }
        }

        public string GetRecordCount(CommandType commandType, string sql, List<SqlParameter> parameters)
        {
            
            using (var connection = GetOpenConnection())
            {
                int result = 0;
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    parameters = parameters ?? new List<SqlParameter>();
                    if (parameters.Count() > 0)
                    {
                        List<SqlParameter> reInstantiate = parameters.Select(p => new SqlParameter { ParameterName = p.ParameterName, Value = p.Value }).ToList();
                        command.Parameters.AddRange(reInstantiate.ToArray());
                    }
                    SqlDataAdapter res = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    res.Fill(dt);
                    result = dt.Rows.Count;
                    return result.ToString();
                }
                //return connection.ExecuteScalar<string>(sql, parameters, commandType: commandType);
            }
            //int result = 0;
            //SqlCommand Command = new SqlCommand(sql, GetOpenConnection());
            //SqlDataAdapter res = new SqlDataAdapter(Command);
            //DataTable dt = new DataTable();
            //res.Fill(dt);
            //result = dt.Rows.Count;
            //return result.ToString();
        }
        // Used to Execute Paramaterized Queries for avoiding SQL INJECTION
        protected object ExecuteReader(CommandType commandType, string sqlquery, List<SqlParameter> parameters, TransactionInitiation TransactionIntiation = null)
        {
            List<string[]> Rows = new List<string[]>();
            SqlCommand command = new SqlCommand();
            if (TransactionIntiation != null)
            {
                using (command = new SqlCommand(sqlquery, TransactionIntiation.TXSqlconn, TransactionIntiation.TXSqltrans))
                {
                    if (TransactionIntiation.TXSqlconn.State == ConnectionState.Closed)
                    {
                        TransactionIntiation.TXSqlconn.Open();
                    }
                    parameters = parameters ?? new List<SqlParameter>();
                    if (parameters.Count() > 0)
                    {
                        //recreate new instance for Sql parameters
                        List<SqlParameter> reInstantiate = parameters.Select(p => new SqlParameter { ParameterName = p.ParameterName, Value = p.Value }).ToList();
                        command.Parameters.AddRange(reInstantiate.ToArray());
                    }
                    using (var reader = command.ExecuteReader())
                    {
                        DataTable data = new DataTable();
                        data.Load(reader);
                        return data;
                    }
                }
            }
            else
            {
                using (var connection = GetOpenConnection())
                {
                    using (command = new SqlCommand(sqlquery, connection))
                    {
                        command.CommandType = commandType;
                        parameters = parameters ?? new List<SqlParameter>();
                        if (parameters.Count() > 0)
                        {
                            //recreate new instance for Sql parameters
                            if(commandType == CommandType.StoredProcedure)
                            {
                                List<SqlParameter> reInstantiate = parameters.Select(p => new SqlParameter { ParameterName = "@" + p.ParameterName, Value = p.Value }).ToList();
                                command.Parameters.AddRange(reInstantiate.ToArray());
                            }
                            else
                            {
                                List<SqlParameter> reInstantiate = parameters.Select(p => new SqlParameter { ParameterName = p.ParameterName, Value = p.Value }).ToList();
                                command.Parameters.AddRange(reInstantiate.ToArray());
                            }
                        }
                        using (var reader = command.ExecuteReader())
                        {
                            DataTable data = new DataTable();
                            data.Load(reader);
                            return data;
                        }
                    }
                }
            }
        }
        #endregion SQLAPI

        #region QueryBuilder

        // Query To Get BO Definition


        // Passing Table and Where Params to get records
        //Ex: SELECT * FROM [XIBO_T] WHERE Name=@Name
        public IEnumerable<T> Select<T>(string sTableName, Dictionary<string, object> Params, string sSelectFields = "")
        {
            var properties = ParseProperties(Params);
            List<string> Keys = Params.Keys.ToList();
            var sqlPairs = GetSqlPairs(Keys, " AND ");
            if (string.IsNullOrEmpty(sSelectFields))
            {
                sSelectFields = "*";
            }

            var sql = string.Empty;
            if (Params.Count == 0)
            {
                sql = string.Format("SELECT {1} FROM [{0}]", sTableName, sSelectFields);
            }
            else
            {
                sql = string.Format("SELECT {2} FROM [{0}] WHERE {1}", sTableName, sqlPairs, sSelectFields);
            }
            return GetItems<T>(CommandType.Text, sql, properties.AllPairs);
        }

        public IEnumerable<T> SelectIN<T>(string sTableName, string sWhere, string sSelectFields = "")
        {
            if (string.IsNullOrEmpty(sSelectFields))
            {
                sSelectFields = "*";
            }

            var sql = string.Format("SELECT {2} FROM [{0}] WHERE {1}", sTableName, sWhere, sSelectFields);
            return GetItems<T>(CommandType.Text, sql);
        }

        public Dictionary<string, string> SelectDDL(string sSelect, string sTableName, string sWhrClause = "")
        {
            string sWhereCond = XIConstant.Key_XIDeleted + " = 0";
            try
            {
                if (!string.IsNullOrEmpty(sWhrClause))
                {
                    sWhereCond = sWhereCond + " and " + sWhrClause;
                }
                var sql = string.Format("SELECT {1} FROM [{0}] Where {2}", sTableName, sSelect, sWhereCond);
                return GetDDLItems(CommandType.Text, sql, null);
            }
            catch(Exception e)
            {
                var sql = string.Empty;
                if (e.Message.StartsWith("Invalid column"))
                {
                    if (!string.IsNullOrEmpty(sWhrClause))
                    {
                        sql = string.Format("SELECT {1} FROM [{0}] Where {2}", sTableName, sSelect, sWhrClause);
                    }
                    else
                    {
                        sql = string.Format("SELECT {1} FROM [{0}]", sTableName, sSelect);
                    }
                }
                return GetDDLItems(CommandType.Text, sql, null);
            }
        }

        public string SelectString(string sSelect, string sTableName, Dictionary<string, object> Params)
        {
            var properties = ParseProperties(Params);
            List<string> Keys = Params.Keys.ToList();
            var sqlPairs = GetSqlPairs(Keys, " AND ");
            var sql = string.Format("SELECT {1} FROM [{0}] WHERE {2}", sTableName, sSelect, sqlPairs);
            return GetString(CommandType.Text, sql, properties.AllPairs);
        }

        public T Insert<T>(T obj, string sTableName, string sIdentityColumn)
        {
            var propertyContainer = ParseProperties_Insert(obj, sIdentityColumn);
            var sql = string.Format("INSERT INTO [{0}] ({1}) VALUES(@{2}) SELECT CAST(scope_identity() AS int)",
                sTableName,
                string.Join(", ", propertyContainer.ValueNames),
                string.Join(", @", propertyContainer.ValueNames));

            using (var connection = GetOpenConnection())
            {
                var id = connection.Query<int>
                (sql, propertyContainer.ValuePairs, commandType: CommandType.Text).First();
                SetId(obj, id, propertyContainer.IdPairs, sIdentityColumn);
                return obj;
            }
        }

        public T Update<T>(T obj, string sTableName, string sIdentityColumn)
        {
            var propertyContainer = ParseProperties_Insert(obj, sIdentityColumn);
            var sqlIdPairs = GetSqlPairs(propertyContainer.IdNames);
            var sqlValuePairs = GetSqlPairs(propertyContainer.ValueNames);
            var sql = string.Format("UPDATE [{0}] SET {1} WHERE {2}", sTableName, sqlValuePairs, sqlIdPairs);
            Execute(CommandType.Text, sql, propertyContainer.AllPairs);
            return obj;
        }

        public void Delete<T>(T obj, string sTableName, string sIdentityColumn)
        {
            var propertyContainer = ParseProperties_Insert(obj, sIdentityColumn);
            var sqlIdPairs = GetSqlPairs(propertyContainer.IdNames);
            var sql = string.Format("DELETE FROM [{0}] WHERE {1}", sTableName, sqlIdPairs);
            Execute(CommandType.Text, sql, propertyContainer.IdPairs);
        }

        private T SetId<T>(T obj, int id, IDictionary<string, object> propertyPairs, string sIdentityColumn)
        {
            if (propertyPairs.Count == 1)
            {
                var propertyName = propertyPairs.Keys.First();
                var propertyInfo = obj.GetType().GetProperty(sIdentityColumn);
                if (propertyInfo.PropertyType == typeof(int))
                {
                    propertyInfo.SetValue(obj, id, null);
                }
            }
            return obj;
        }

        public object ExecuteQuery(string sQuery, List<SqlParameter> lstparams = null, TransactionInitiation TXInitiation = null)
        {
            using (var connection = GetOpenConnection())
            {
                var Data = ExecuteReader(CommandType.Text, sQuery, lstparams, TXInitiation);
                return Data;
            }
        }
        public object ExecuteStoredProcedure(string sQuery, List<SqlParameter> lstparams = null, TransactionInitiation TXInitiation = null)
        {
            using (var connection = GetOpenConnection())
            {
                var Data = ExecuteReader(CommandType.StoredProcedure, sQuery, lstparams, TXInitiation);
                return Data;
            }
        }
        public object Execute_Query(string sQuery)
        {
            try
            {
                using (var connection = GetOpenConnection())
                {
                    SqlCommand command = new SqlCommand();
                    using (command = new SqlCommand(sQuery, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            DataTable data = new DataTable();
                            data.Load(reader);
                            return data;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public bool ExecuteSP(string sTableName, bool bIsUpdateFK)
        {
            var p = new DynamicParameters();
            p.Add("@sTableName", sTableName);
            p.Add("@bIsUpdateFK", bIsUpdateFK);
            p.Add("@b", dbType: DbType.Boolean, direction: ParameterDirection.Output);

            using (var connection = GetOpenConnection())
            {
                connection.Execute("usp_GenerateGUID", p, commandType: CommandType.StoredProcedure);
            }
            bool b = p.Get<bool>("@b");
            return b;
        }

        private static PropertyContainer ParseProperties(Dictionary<string, object> Params)
        {
            var propertyContainer = new PropertyContainer();
            foreach (var property in Params)
            {
                var name = property.Key;
                var value = property.Value;
                propertyContainer.AddId(name, value);
            }

            return propertyContainer;
        }

        private static PropertyContainer ParseProperties_Insert<T>(T obj, string sIdentityColumn)
        {
            var propertyContainer = new PropertyContainer();

            var typeName = typeof(T).Name;

            var validKeyNames = new string[3];
            if (!string.IsNullOrEmpty(sIdentityColumn))
            {
                validKeyNames = new[] { sIdentityColumn };
            }
            else
            {
                validKeyNames = new[] { "Id",
            string.Format("{0}Id", typeName), string.Format("{0}_Id", typeName) };
            }


            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {

                // Skip reference types (but still include string!)
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    continue;

                // Skip methods without a public setter
                if (property.GetSetMethod() == null)
                    continue;

                // Skip methods specifically ignored
                if (property.IsDefined(typeof(DapperIgnore), false))
                    continue;

                var name = property.Name;
                var value = typeof(T).GetProperty(property.Name).GetValue(obj, null);

                if (property.IsDefined(typeof(DapperKey), false) || validKeyNames.Contains(name))
                {
                    propertyContainer.AddId(name, value);
                }
                else
                {
                    propertyContainer.AddValue(name, value);
                }
            }

            return propertyContainer;
        }

        /// <summary>
        /// Create a commaseparated list of value pairs on 
        /// the form: "key1=@value1, key2=@value2, ..."
        /// </summary>
        private static string GetSqlPairs(IEnumerable<string> keys, string separator = ", ")
        {
            var pairs = keys.Select(key => string.Format("{0}=@{0}", key)).ToList();
            return string.Join(separator, pairs);
        }

        private class PropertyContainer
        {
            private Dictionary<string, object> _ids;
            private Dictionary<string, object> _values;

            #region Properties

            public IEnumerable<string> IdNames
            {
                get { return _ids.Keys; }
                set { }
            }

            public IEnumerable<string> ValueNames
            {
                get { return _values.Keys; }
                set { }
            }

            public IEnumerable<string> AllNames
            {
                get { return _ids.Keys.Union(_values.Keys); }
                set { }
            }

            public IDictionary<string, object> IdPairs
            {
                get { return _ids; }
                set { }
            }

            public IDictionary<string, object> ValuePairs
            {
                get { return _values; }
                set { }
            }

            public IEnumerable<KeyValuePair<string, object>> AllPairs
            {
                get { return _ids.Concat(_values); }
                set { }
            }

            #endregion

            #region Constructor

            internal PropertyContainer()
            {
                _ids = new Dictionary<string, object>();
                _values = new Dictionary<string, object>();
            }

            #endregion

            #region Methods

            internal void AddId(string name, object value)
            {
                _ids.Add(name, value);
            }

            internal void AddValue(string name, object value)
            {
                _values.Add(name, value);
            }

            #endregion
        }


        #endregion QueryBuilder

        public class TransactionInitiation
        {
            private string sMyTXBeginAt;
            public string sTXBeginAt
            {
                get
                {
                    return sMyTXBeginAt;
                }
                set
                {
                    sMyTXBeginAt = value;
                }
            }
            public TransactionInitiation()
            {
                mysqlconn = GetConnection();
                mysqltrans = GetTransactions();
            }
            //private const string sConnection = @"Data Source=192.168.7.8;initial catalog=iPMTest;User Id=crqauser; Password=crqauser; MultipleActiveResultSets=True";
            private SqlConnection GetConnection()
            {
                SqlConnection _conn = new SqlConnection(ConfigurationManager.AppSettings["ClientDataBase"].ToString());
                if (_conn.State == System.Data.ConnectionState.Closed)
                    _conn.Open();
                return _conn;
            }

            private SqlTransaction GetTransactions()
            {
                return mysqlconn.BeginTransaction();
            }
            private SqlConnection mysqlconn = null;
            private SqlTransaction mysqltrans = null;
            public SqlConnection TXSqlconn
            {
                get
                {
                    return mysqlconn;
                }
                set
                {
                    mysqlconn = value;
                }
            }
            public SqlTransaction TXSqltrans
            {
                get
                {
                    return mysqltrans;
                }
                set
                {
                    mysqltrans = value;
                }
            }
            public CResult TXCommitRollback(CResult input)
            {
                const string ROUTINE_NAME = "TXCommitRollback_CommitRollback";
                CResult oCResult = new CResult();
                //XIInstanceBase oIB = new XIInstanceBase();
                try
                {
                    if (mysqlconn.State == ConnectionState.Open)
                    {
                        if (input.xiStatus == xiEnumSystem.xiFuncResult.xiError)
                        {
                            mysqltrans.Rollback();
                        }
                        else
                        {
                            mysqltrans.Commit();
                        }
                        mysqlconn.Close();
                    }
                    else
                    {
                        oCResult.sMessage = ROUTINE_NAME + ",Unable to ROLLBACK/COMMIT TRANSACTION, Connection String CLOSED";
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.LogToFile();
                        oCResult.sMessage = ROUTINE_NAME + ",Unable to ROLLBACK/COMMIT TRANSACTION, Connection String CLOSED";
                        //oIB.SaveErrortoDB(oCResult);
                    }
                }
                catch (Exception ex)
                {
                    mysqltrans.Rollback();
                    mysqlconn.Close();
                    oCResult.sMessage = ROUTINE_NAME + ", Unable to Commit/ROLLBACK the Transaction";
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.LogToFile();
                    oCResult.sMessage = ROUTINE_NAME + ", Unable to Commit/ROLLBACK the Transaction";
                    //oIB.SaveErrortoDB(oCResult);
                }
                finally
                {
                    mysqlconn.Dispose();
                    mysqltrans.Dispose();
                }
                return oCResult;
            }

        }
    }
}