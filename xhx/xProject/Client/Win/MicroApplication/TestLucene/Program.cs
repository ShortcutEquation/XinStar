using com.star.Logic.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLucene
{
    class Program
    {
        static void Main(string[] args)
        {

            var model = new BuiltIndex_SingleThread();
            model.StartBuild();

            //var response = CallLogic<object, object>("com.star.Logic.Search.dll", "com.star.Logic.Search.BuiltIndex_SingleThread", "StartBuild", null);
        }
    }
}
