using System;
using System.Data;

namespace VirtualRoulette.Common
{
    public static class DataRecordExtensions
    {
        public static string GetString(this IDataRecord self, string key) => self[key].ToString();
        public static int GetInt32(this IDataRecord self, string key) => Convert.ToInt32(self[key]);
        public static decimal GetDecimal(this IDataRecord self, string key) => Convert.ToDecimal(self[key]);
    }
}
