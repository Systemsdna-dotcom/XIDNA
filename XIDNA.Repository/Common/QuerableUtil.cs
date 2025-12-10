using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using XIDNA.Models;

namespace XIDNA.Repository
{
    public static class QuerableUtil
    {

        public static int GetRecordsCount<T>(this IQueryable<T> source, string filterExpression)
        {
            if (!String.IsNullOrWhiteSpace(filterExpression))
                return source.Where(filterExpression).Count();
            else
                return source.Count();
        }

        public static IQueryable<T> GetResults<T>(this IQueryable<T> source, string filterExpression, string sortExpression, int pageIndex, int recordsPerPage, string sortDirection = "Desc")
        {
            if (!String.IsNullOrWhiteSpace(filterExpression))
                return source.Where(filterExpression).OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * recordsPerPage).Take(recordsPerPage);
            else

                return source.OrderBy(sortExpression + " " + sortDirection).Skip(pageIndex * recordsPerPage).Take(recordsPerPage);
        }
        public static IQueryable<T> GetResultsForDataTables<T>(this IQueryable<T> source, string filterExpression, string sortExpression, jQueryDataTableParamModel param)
        {
            string sortDirection = param.sSortDir;
            {
                // donot sort column with zero index because first column will be serial no(not sortable)
                if (param.iSortCol != 0)
                {
                    sortExpression = param.sColumns.Split(',')[param.iSortCol];                    
                }
            }
            
            // if no sort direction take as descending
            if (string.IsNullOrWhiteSpace(sortDirection))
            {
                sortDirection = "Desc";
            }
            if (string.IsNullOrWhiteSpace(filterExpression))
            {
                if (param.iDisplayLength != -1) { 
                var result = source.OrderBy(sortExpression + " " + sortDirection).Skip(param.iDisplayStart).Take(param.iDisplayLength);
                return result;
                }
                else
                {
                    var result = source.OrderBy(sortExpression + " " + sortDirection);
                    return result;
                }
            }
            else
            {
                if (param.iDisplayLength != -1)
                {
                    return source.Where(filterExpression).OrderBy(sortExpression + " " + sortDirection).Skip(param.iDisplayStart).Take(param.iDisplayLength);
                }
                else
                {
                    return source.Where(filterExpression).OrderBy(sortExpression + " " + sortDirection);
                }                
            }
        }
        public static IQueryable<T> GetRecordsCountExpr<T>(this IQueryable<T> source, string filterExpression)
        {
            if (!String.IsNullOrWhiteSpace(filterExpression))
                return source.Where(filterExpression);
            else
                return source;
        }

    }
}
