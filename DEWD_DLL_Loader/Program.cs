using System;
using System.Threading.Tasks;

namespace DEWD_DLL_Loader
{
    class Program
    {
        //"C:\Users\clow_\source\repos\DEWD_DLL_Loader\bin\Debug\netcoreapp3.1\DEWD_DLL_Loader.dll"
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!"+args[0]);
        }
    }
    class T_DEWD
    {
        public  dynamic  RUN(Microsoft.AspNetCore.Http.HttpRequest request)
        {

            var sr = new System.IO.StreamReader(request.Body);
            var json_str_task =  sr.ReadToEndAsync();
            json_str_task.Wait();
            return json_str_task.Result;

        }


    }
}
