using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XIDataBase
{
    public class XIDBMSSQL : XIDBBase
    {
        public XIDBMSSQL(XIDBBase oBase)
        {
            sSelectFields = oBase.sSelectFields;
            sWhereFields = oBase.sWhereFields;
            sTableName = oBase.sTableName;
        }
        string sSQL = null;
        public string Build()
        {
            if (!string.IsNullOrEmpty(sSelectFields))
            {
                sSQL = string.Format("SELECT {0}", sSelectFields);
            }
            if (!string.IsNullOrEmpty(sTableName))
            {
                sSQL = sSQL + string.Format(" FROM [{0}]", sTableName);
            }
            if (!string.IsNullOrEmpty(sWhereFields))
            {
                sSQL = sSQL + string.Format(" WHERE {0}", sWhereFields);
            }
            if (!string.IsNullOrEmpty(sGroupFields))
            {
                sSQL = sSQL + string.Format(" GROUP BY {0}", sGroupFields);
            }
            if (!string.IsNullOrEmpty(sOrderFields))
            {
                sSQL = sSQL + string.Format(" ORDER BY {0}", sOrderFields);
            }
            return sSQL;
        }
        string sOracleSQL = null;
        public string OracleBuild()
        {
            if (!string.IsNullOrEmpty(sSelectFields))
            {
                sOracleSQL = string.Format("SELECT {0}", sSelectFields);
            }
            if (!string.IsNullOrEmpty(sTableName))
            {
                sOracleSQL = sOracleSQL + string.Format(" FROM [{0}]", sTableName);
            }
            if (!string.IsNullOrEmpty(sWhereFields))
            {
                sOracleSQL = sOracleSQL + string.Format(" WHERE {0}", sWhereFields);
            }
            if (!string.IsNullOrEmpty(sGroupFields))
            {
                sOracleSQL = sOracleSQL + string.Format(" GROUP BY {0}", sGroupFields);
            }
            if (!string.IsNullOrEmpty(sOrderFields))
            {
                sOracleSQL = sOracleSQL + string.Format(" ORDER BY {0}", sOrderFields);
            }
            return sOracleSQL;
        }
    }
}