using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.Diagnostics;

namespace JsonDataProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            const string url= "http://files.olo.com/pizzas.json";
            const int printOutNum = 20;
            var list= Utility<Order>.getDataFromUrl(url);
            Console.WriteLine($"\nThe Total Number of All Orders {list.Count}\n");

            var newList = list.GroupBy(o => o.ToString()).Select(x => new {x.Key, c=x.Count()})
                .OrderByDescending(y=>y.c).ToList();
            
            var len = Math.Min(printOutNum, newList.Count);
            int maxLen = 0;
            for(var i = 0; i < len; i++)
            {
                maxLen = Math.Max(maxLen, newList[i].Key.Length);
            }
            Console.WriteLine("The Top 20 most Frequently Ordered Pizza Topping Combination:\n");
            Console.WriteLine("The Rank --- Topping--- The Number of Times\n");
            for (int i=0; i < len; i++)
            {
                var rank = (i+1) < 10 ? (" " + (i+1).ToString()) : (i+1).ToString();
                var keyLength= maxLen - newList[i].Key.Length + 1;
                var keyspace = new string('.', keyLength);

                Console.WriteLine($"{rank}...{newList[i].Key}{keyspace}{newList[i].c}");
            }
            sw.Stop();

            Console.WriteLine($"Runtime:{sw.ElapsedMilliseconds} millionseconds");
            Console.WriteLine("\n\n\n Press any Key to Exit");
            Console.ReadKey();
            
        }
    }
     
    public static class Utility<T> where T: IComparable<T>, new()
    {
        public static List<T> getDataFromUrl(string url) 
        {
            using (var wc=new WebClient())
            {
                var json_data = string.Empty;
                try
                {
                    json_data=wc.DownloadString(url);
                }
                catch (Exception) { }
                return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<List<T>>(json_data) : new List<T> { new T() };
            }
            
        }


        
    }

    public  class Order : IComparable<Order>
    {
        public string[] Toppings { get; set; }

        public Order() { }

        public int CompareTo(Order other)
        {
            Array.Sort<string>(this.Toppings);
            Array.Sort<string>(other.Toppings);

            string s1 = string.Join("", this.Toppings);
            string s2 = string.Join("", other.Toppings);

            return string.Compare(s1, s2, true);
        }
        public override string ToString()
        {
            if (this.Toppings == null|| this.Toppings.Length == 0) return "";
            return string.Join(",", this.Toppings);
        }
    }
    

}


