using SMCISD.Student360.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Grid
{
    public static class GridExtensionMethods
    {

        public static async Task<GridResponse> ExecuteGridQuery(this IQueryable query, GridMetadata metadata, bool allData = false) {


            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var response = new GridResponse();

            //// Total Count After Security
            //response.TotalCount = query.Count();

            // Adding Requested Filters
            query = query.Where(metadata.WhereString, metadata.WhereValues);

            // Count after filters
            response.FilteredCount = query.Count();
            response.TotalCount = response.FilteredCount;

            if (metadata.OrderByString != null)
                query = query.OrderBy(metadata.OrderByString);

            if (metadata.SelectString != null)
                query = query.Select(metadata.SelectString);

            if (allData)
                response.Data = await query.ToDynamicListAsync();
            else
                response.Data = await query.Skip(metadata.SkipCount).Take(metadata.TakeCount).ToDynamicListAsync();

            stopWatch.Stop();
            response.QueryExecutionMs = stopWatch.ElapsedMilliseconds;

            return response;
        }

        public static GridMetadata ProcessMetadata(this GridRequest request, Object entity)
        {
            var metadata = new GridMetadata();

            var filterValues = request.Filters.Select(x => x.Value.ToString()).ToList();
            filterValues = filterValues.ToList();


            if(request.SearchTerm != null && request.SearchTerm != String.Empty)
            {
                filterValues.Add(request.SearchTerm);
                metadata.WhereString = $"({request.Filters.FilterString()}) and ({request.SearchString(entity)})";
            }
            else
                metadata.WhereString = $"({request.Filters.FilterString()})";

            metadata.WhereValues = filterValues.ToArray();
            metadata.OrderByString = request.OrderBy.OrderByString();
            metadata.SelectString = request.Select.SelectString();
            metadata.TakeCount = request.PageSize;
            metadata.SkipCount = (request.PageNumber - 1) * request.PageSize;

            return metadata;
        }

        private static string SearchString(this GridRequest request, Object entity)
        {
            if (request.SearchTerm == null || request.SearchTerm.Length == 0)
                return "true";

            var result = "";

            if (request.Select.Count > 0)
            {
                for (int i = 0; i < request.Select.Count; i++)
                {
                    result += $"{request.Select[i]} != null and string(object({request.Select[i]})).Contains(@{request.Filters.Count})";

                    if (request.Select.Count > 1 && i < request.Select.Count - 1)
                        result += " or ";
                }
            }
            else
            {
                var objectProperties = entity.GetType().GetProperties().Select(x => x.Name).ToArray();

                for(int i =0; i<objectProperties.Length; i++)
                {
                    result += $"{objectProperties[i]} != null and string(object({objectProperties[i]})).Contains(@{request.Filters.Count})";

                    if (objectProperties.Length > 1 && i < objectProperties.Length - 1)
                        result += " or ";
                }
            }
            return result;
        }

        private static string FilterString(this List<Filter> Filters) {
            if (Filters.Count == 0)
                return "true";

            string result = "";
            for (int i = 0; i < Filters.Count; i++)
            {
                result += $"{Filters[i].Column} {Filters[i].Operator} @{i}";

                if (Filters.Count > 1 && i < Filters.Count - 1)
                    result += " and ";
            }

            return result;
        }
        private static string OrderByString(this List<OrderByProperties> OrderBy)
        {
            if (OrderBy.Count == 0)
                return null;

            string result = "";
            for (int i = 0; i < OrderBy.Count; i++)
            {
                result += $"{OrderBy[i].Column}";

                if (OrderBy[i].Direction.Trim().ToUpper() != "ASCENDING")
                    result += " desc";

                if (OrderBy.Count > 1 && i < OrderBy.Count - 1)
                    result += ",";
            }

            return result;
        }

        private static string SelectString(this List<string> Select)
        {
            if (Select.Count == 0)
                return null;

            string result = "new(";
            for (int i = 0; i < Select.Count; i++)
            {
                result += $"{Select[i]}";

                if (Select.Count > 1 && i < Select.Count - 1)
                    result += ",";

                if (i == Select.Count - 1)
                    result += ")";
            }

            return result;
        }
    }

    public class GridMetadata
    {
        public string SelectString { get; set; }
        public string OrderByString { get; set; }
        public string WhereString { get; set; }
        public string SearchValue { get; set; }
        public int TakeCount { get; set; }
        public int SkipCount { get; set; }
        public string[] WhereValues { get; set; }
        public int TotalCount { get; set; }
    }
}

