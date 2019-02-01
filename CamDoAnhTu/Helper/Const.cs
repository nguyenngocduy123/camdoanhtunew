using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CamDoAnhTu.Helper
{
    public class Const
    {
        public static string[] masotragopArr = { "BA", "CA", "MA", "ZA", "YA", "TA", "QA" };
        public static string[] masotradungArr = { "BD", "CD", "MD", "ZD", "YD", "TD" };
        public static int[] tragopArr = { 1, 2, 3, 4, 5, 6, 7 };
        public static int[] tradungArr = { 12, 13, 14, 15, 16, 17 };
        public static int todayYear = DateTime.Now.Year;
        public static int todayMonth = DateTime.Now.Month;
        public static int todayDay = DateTime.Now.Day;
        public static DateTime? chonngaylam = null;
        public static string timetemp;

    }
}