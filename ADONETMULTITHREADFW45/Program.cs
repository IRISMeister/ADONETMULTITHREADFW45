using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterSystems.Data.IRISClient;
using System.Threading;

namespace ADONETMULTITHREADFW45
{
    class Program
    {
        private static IRISConnection IRISConnect = null;

        static void Main(string[] args)
        {
            String host = "localhost";
            String port = "1972";
            String username = "_SYSTEM";
            String password = "SYS";
            String Namespace = "SAMPLES";

            IRISConnect = new IRISConnection();
            IRISConnect.ConnectionString = "Server = " + host
                + "; Port = " + port + "; Namespace = " + Namespace
                + "; Password = " + password + "; User ID = " + username + ";"
                + " POOLING = false;";

            IRISConnect.Open();

            Task<int> task1 = Task.Run(() => {
                return HeavyMethod1();
            });
            Thread.Sleep(1000);

            Task<int> task2 = Task.Run(() => {
                return LightMethod1();
            });

            Task<int> task3 = Task.Run(() => {
                return LightMethod1();
            });

            Task.WhenAll(task1, task2, task3);
            int r = task1.Result;
            r = task2.Result;
            r = task3.Result;

            IRISConnect.Close();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

        }
        static int HeavyMethod1()
        {
            Console.WriteLine("Very heavy query started");

            String queryString = "SELECT avg(p1.Age),avg(p2.Age),avg(p3.Age) FROM Sample.Person p1,Sample.Person p2,Sample.Person p3,Sample.Person p4 where p4.ID<10";
            IRISCommand cmd4 = new IRISCommand(queryString, IRISConnect);

            IRISDataReader Reader = cmd4.ExecuteReader();

            Console.WriteLine("Printing out contents of SELECT query: ");
            while (Reader.Read())
            {
                Console.WriteLine(Reader.GetValue(0).ToString() + ", " + Reader.GetValue(1).ToString() + ", " + Reader.GetValue(2).ToString());
            }

            Reader.Close();
            cmd4.Dispose();

            Console.WriteLine("Very heavy query finished");
            return 1;
        }

        static int LightMethod1()
        {
            Console.WriteLine("Light query started");

            String queryString = "SELECT top 10 * FROM Sample.Person";
            IRISCommand cmd4 = new IRISCommand(queryString, IRISConnect);

            IRISDataReader Reader = cmd4.ExecuteReader();

            Console.WriteLine("Printing out contents of SELECT query: ");
            while (Reader.Read())
            {
                Console.WriteLine(Reader.GetValue(0).ToString() + ", " + Reader.GetValue(1).ToString() + ", " + Reader.GetValue(2).ToString());
            }

            Reader.Close();
            cmd4.Dispose();

            Console.WriteLine("Light query finished");
            return 2;
        }

    }
}
