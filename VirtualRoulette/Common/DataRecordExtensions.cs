using System;
using System.Data;

namespace VirtualRoulette.Common
{
    public static class DataRecordExtensions
    {
        public static string GetString(this IDataRecord self, string key) => self[key].ToString();
        public static int GetInt32(this IDataRecord self, string key) => Convert.ToInt32(self[key]);
        public static bool GetBoolean(this IDataRecord self, string key) => Convert.ToBoolean(self.GetInt32(key));
        public static decimal GetDecimal(this IDataRecord self, string key) => Convert.ToDecimal(self[key]);
        public static decimal? GetDecimalOrNull(this IDataRecord self, string key) => self[key] is DBNull ? default(decimal?) : Convert.ToDecimal(self[key]);
        public static DateTime GetDateTime(this IDataRecord self, string key) => Convert.ToDateTime(self[key]);
        public static DateTime? GetDateTimeOrNull(this IDataRecord self, string key) => self[key] is DBNull ? default(DateTime?) : self.GetDateTime(key);
    }
}
