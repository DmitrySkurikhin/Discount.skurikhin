using System;
using Quantlib;
using System.Collections.Generic;

namespace Discount
{
    struct Quote{
        public DateTime date;
        public double value;
        public double DateToNumber(){
            return date.Ticks;
        }
        public Quote(DateTime date_init, double value_init){
            date = date_init;
            value = value_init;
        }
    }
    class Program
    {
        private static double DateToDouble(DateTime date)
            {
                double tmp = date.Year*8640 + date.Month*720 + date.Day*24 + date.Hour;
                return tmp;
            }
        static class Reader
        {
            private static DateTime StrToDate(String str)
            {
                DateTime Now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                if (str == "ON"){
                    return Now;
                }
                Char lastSym = str[str.Length - 1];
                str = str.Remove(str.Length - 1);
                if (lastSym == 'W'){
                    Now = Now.AddDays(7 * Convert.ToInt32(str));
                }
                else if (lastSym == 'M'){
                    Now = Now.AddMonths(Convert.ToInt32(str));
                }
                else if (lastSym == 'Y'){
                    Now = Now.AddYears(Convert.ToInt32(str));
                }
                else{
                    throw new Exception("Error! Invalid input argument!");
                }
                return Now;
            }
            public static SortedList<double, double> ReadFile(String path)
            {
                SortedList<double, double> data = new SortedList<double, double>();
                using(var reader = new System.IO.StreamReader(path)){
                    double date;
                    double value;
                    reader.ReadLine();
                    while (!reader.EndOfStream){
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        date = DateToDouble(StrToDate(values[0]));
                        value = (double)Convert.ToInt32(values[1]);
                        data.Add(date, value);
                    }
                }
                return data;
            }
        }
        public static SortedList<DateTime, double> SwapPoints(SortedList<DateTime, double> spoints, List<DateTime> date)
        {
            LinearInterpolator Interpolator = new LinearInterpolator();
            SortedList<DateTime, double> data = new SortedList<DateTime, double>();
            SortedList<double, double> tmp = new SortedList<double, double>();
            for(int i = 0; i < spoints.Count; i++)
            {
                tmp.Add(spoints.Keys[i].Ticks, spoints.Values[i]);
            }
            for(int i = 0; i < date.Count; i++)
            {
                data.Add(date[i], Interpolator.Calculate(tmp, date[i].Ticks));
            }
            return data;
        }
        public static SortedList<DateTime, double> DiscountFactors(SortedList<DateTime, double> spoints, Func<SortedList<DateTime, double>> DiscountFactorsUSD, List<DateTime> date)
        {
            LinearInterpolator Interpolator = new LinearInterpolator();
            SortedList<DateTime, double> data = new SortedList<DateTime, double>();
            SortedList<double, double> tmp = new SortedList<double, double>();
            for(int i = 0; i < spoints.Count; i++)
            {
                tmp.Add(spoints.Keys[i].Ticks, spoints.Values[i]);
            }
            for(int i = 0; i < date.Count; i++)
            {
                data.Add(date[i], Interpolator.Calculate(tmp, date[i].Ticks));
            }
            return data;
        }
        static void Main()
        {
            SortedList<DateTime, double> spoints = new SortedList<DateTime, double>();
            List<DateTime> date = new List<DateTime>();
            LinearInterpolator interpolator = new LinearInterpolator();
            int n = 12;
            for(int i = 0; i < n; i++)
            {
                spoints.Add(new DateTime(2020, i+1, 1, 0, 0, 0), (i+1)*(i+1)*100);
            }
            int m = 31;
            for(int i = 0; i < m; i++)
            {
                date.Add(new DateTime(2020, 10, i+1, 0, 0, 0));
            }
            SortedList<DateTime, double> swap = SwapPoints(spoints, date);
            foreach(DateTime key in swap.Keys)
            {
                Console.WriteLine(key.ToString());
                Console.WriteLine(swap[key]);
                Console.WriteLine();
            }
        }
    }
}