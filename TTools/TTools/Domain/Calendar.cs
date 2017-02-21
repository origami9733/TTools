using System;
using System.Data;

namespace TTools.Domain
{
    public static class Calendar
    {
        private static DataTable calendar = TpicsDbContext.LoadCalendar(DateTime.Now.ToString("yyyyMM"));

        static public string LeadTimeCalc(int leadTime)
        {
            if (leadTime == 0) return DateTime.Now.ToString("yyyy/MM/dd");

            DateTime a = DateTime.Now;
            int nowDay = int.Parse(a.ToString("dd"));

            int rowSelector = 1;
            int dayCount;

            string returnDate = null;

            do
            {
                dayCount = (int)calendar.Rows[rowSelector][3];
                if (rowSelector == 1)
                {
                    for (var i = nowDay + 4; i < dayCount + 4; i++)
                    {
                        if ((decimal)calendar.Rows[rowSelector][i] == 1)
                        {
                            leadTime = leadTime - 1;
                            if (leadTime == 0)
                            {
                                returnDate = calendar.Rows[rowSelector - 1][i].ToString();
                                break;
                            }
                        }
                    }
                }
                else
                {
                    for (var i = 4; i < dayCount + 4; i++)
                    {
                        if ((decimal)calendar.Rows[rowSelector][i] == 1)
                        {
                            leadTime = leadTime - 1;
                            if (leadTime == 0)
                            {
                                returnDate = calendar.Rows[rowSelector - 1][i].ToString();
                                break;
                            }
                        }
                    }
                }
                rowSelector = rowSelector + 3;
            } while (returnDate == null);

            returnDate = SetSplitChar(returnDate);
            return returnDate;
        }

        private static string SetSplitChar(string date)
        {
            var year = date.Substring(0, 4);
            var month = date.Substring(4, 2);
            var day = date.Substring(6, 2);

            string splitChar = "/";

            return year + splitChar + month + splitChar + day;
        }
    }
}