using System;
using LinearInterpolation;
using System.Collections.Generic;

namespace Discount
{
    class Program
    {
        static class GetData
        {
            private static DateTime StrToDate(String str)
            {
                DateTime Now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                if(str == "ON")
                {
                    return Now;
                }
                Char Period = str[str.Length - 1];
                str = str.Remove(str.Length - 1);
                if(Period == 'W')
                {
                    Now = Now.AddDays(7 * Convert.ToInt32(str));
                }
                else if(Period == 'M')
                {
                    Now = Now.AddMonths(Convert.ToInt32(str));
                }
                else if(Period == 'Y')
                {
                    Now = Now.AddYears(Convert.ToInt32(str));
                }
                return Now;
            }
            public static SortedList<DateTime, double> GetFromCSV(String path)
            {
                SortedList<DateTime, double> data = new SortedList<DateTime, double>();
                using(System.IO.StreamReader Read = new System.IO.StreamReader(path))
                {
                    DateTime date;
                    double value;
                    Read.ReadLine();
                    while(Read.EndOfStream == false)
                    {
                        string line = Read.ReadLine();
                        string[] swappoints = line.Split(',');
                        date = StrToDate(swappoints[0]);
                        value = Convert.ToInt32(swappoints[1]);
                        data.Add(date, value);
                    }
                }
                return data;
            }
       }
        public static SortedList<DateTime, double> DiscountFactors(SortedList<DateTime, double> spoints, Func<List<DateTime>, SortedList<DateTime, double>> DiscountFactorsUSD, double price, List<DateTime> date)
        {
            LinearInterpolation.Interpolation Interpolator = new Interpolation();
            SortedList<DateTime, double> usdfactors = DiscountFactorsUSD(date);
            SortedList<DateTime, double> data = new SortedList<DateTime, double>();
            Point[] tmp = new Point[spoints.Count];
            SortedList<DateTime, double> rubfactors = new SortedList<DateTime, double>();
            for(int i = 0; i < spoints.Count; i++)
            {
                tmp[i].x = spoints.Keys[i].Ticks;
                tmp[i].y = spoints.Values[i];
            }
            for(int i = 0; i < date.Count; i++)
            {
                var LinInt = Interpolation.GetInterpolation(tmp);
                data.Add(date[i], LinInt(date[i].Ticks));
            }
            foreach(DateTime key in data.Keys)
            {
                rubfactors.Add(key, 10000*price*usdfactors[key]/(data[key] + 10000*price));
            }
            return rubfactors;
        }
        public static SortedList<DateTime, double> SwapPoints(SortedList<DateTime, double> spoints, Func<List<DateTime>, SortedList<DateTime, double>> DiscountFactorsUSD, double price, List<DateTime> date)
        {
            SortedList<DateTime, double> usdfactors = DiscountFactorsUSD(date);
            SortedList<DateTime, double> rubfactors = DiscountFactors(spoints, DiscountFactorsUSD, price, date);
            SortedList<DateTime, double> scurve = new SortedList<DateTime, double>();
            foreach(DateTime key in rubfactors.Keys)
            {
                scurve.Add(key, 10000*price*((usdfactors[key]/rubfactors[key]) - 1));
            }
            return scurve;
        }
        public static SortedList<DateTime, double> Task1(List<DateTime> dates)
        {
            SortedList<DateTime, double> data = new SortedList<DateTime, double>();
            for(int i = 0; i < dates.Count; i++)
            {
                data.Add(dates[i], 1);
            }
            return data;
        }
        static void Main()
        {
            //Testing with a senseless Task1 function (made only for checking for errors)
            SortedList<DateTime, double> list = GetData.GetFromCSV("RUB swap points.csv");
            List<DateTime> dates = new List<DateTime>();
            for(int i=1; i<13; i++)
            {
                dates.Add(new System.DateTime(2022, i, 10));
            }
            for(int i=1; i<13; i++)
            {
                dates.Add(new System.DateTime(2023, i, 10));
            }
            double price = 75;
            SortedList<DateTime, double> df = DiscountFactors(list, Task1, price, dates);
            SortedList<DateTime, double> sp = SwapPoints(list, Task1, price, dates);
            Console.WriteLine("Discount factors:");
            foreach(var key in df.Keys)
            {
                Console.WriteLine(df[key]);
            }
            Console.WriteLine("Swap points:");
            foreach(var key in sp.Keys)
            {
                Console.WriteLine(sp[key]);
            }
        }
    }
}