using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreadTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var isa = new List<int>() { 1,2,3,4};
            var s = isa.Select(x=>x).ToList();



            //var task = new ThreadManage();
            for(int i = 0;i<5; i++)
            ThreadManage.Do();

            Console.ReadLine();
        }
    }
}
