using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DEWD_DLL_Loading_Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DEWD_DLL_Loader_Controller : ControllerBase
    {
        [HttpGet]
        [HttpPost]
        [Route("/LOAD")]
        public async Task<dynamic>  Load()
        {


            

            var sr = new System.IO.StreamReader(Request.Body);
            var json_str = await sr.ReadToEndAsync();
            Console.WriteLine(json_str);
            Params p = (Params)JsonHelper.FromJson(json_str, typeof(Params));


            string callingDomainName = AppDomain.CurrentDomain.FriendlyName;//Thread.GetDomain().FriendlyName; 
            Console.WriteLine(callingDomainName);

            
            String DLL_Name=System.IO.Path.GetFileName(p.dll_path);
            String DLL_Location = System.IO.Path.GetFileName(p.dll_path);
            DEWDAssemblyLoadContext alc = new DEWDAssemblyLoadContext(p.dll_path);
            System.Reflection.Assembly asm;
            try
            {
            asm= alc.LoadFromAssemblyPath(p.dll_path);
            }catch(Exception e)
            {
                 return "KEEP JUNK AWAY PLEASE"; 
            }


            if(Program.g_DLL_Map[p.dll_path]==null)
            {
                Program.g_DLL_Map[p.dll_path] = alc;
                Program.g_DLL_Name_Map[p.short_name] = p.dll_path;
            }
            else
            {
                DEWDAssemblyLoadContext oldalc = (DEWDAssemblyLoadContext) Program.g_DLL_Map[p.dll_path];
                oldalc.Unload();
                Program.g_DLL_Map[p.dll_path] = alc;
                Program.g_DLL_Name_Map[p.short_name] = p.dll_path;
            }
            
            


            return null;
        }

        [HttpGet]
        [HttpPost]
        [Route("/UNLOAD")]
        public async Task<dynamic> Unload()
        {
            var sr = new System.IO.StreamReader(Request.Body);
            var json_str = await sr.ReadToEndAsync();
            Console.WriteLine(json_str);
            Params p = (Params)JsonHelper.FromJson(json_str, typeof(Params));


           
            String dllpath=(String)Program.g_DLL_Name_Map[p.short_name];
            if (dllpath == null) { return "YOU ARE JUST TRYING TO RELEASE THE VACANT SLOT"; };
            DEWDAssemblyLoadContext oldalc = (DEWDAssemblyLoadContext)Program.g_DLL_Map[dllpath];
            oldalc.Unload();
            Program.g_DLL_Map[dllpath]=null;
            Program.g_DLL_Name_Map[p.short_name] = null;
            return null;
        }

        [HttpGet]
        [HttpPost]
        [Route("/RUN")]
        public async Task<dynamic> Run()
        {
            var sr = new System.IO.StreamReader(Request.Body);
            var json_str = await sr.ReadToEndAsync();
            Console.WriteLine(json_str);
            Params p = (Params)JsonHelper.FromJson(json_str, typeof(Params));
            DEWDAssemblyLoadContext alc = (DEWDAssemblyLoadContext)Program.g_DLL_Map[p.dll_path];
            var asmlist=alc.Assemblies.ToList();
            Type t_type=asmlist[0].GetType(p.class_name);
            Object t_object= asmlist[0].CreateInstance(p.class_name);
            var t_method=t_type.GetMethod(p.method_name);
            dynamic res= t_method.Invoke(t_object, new Object[] {this.Request});
            return null;
        }
        [HttpGet]
        [HttpPost]
        [Route("/RUNDLL/{dll_name}/{dll_class}/{dll_method}")]
        public async Task<dynamic> RunDLLShortName(string dll_name,string dll_class ,string dll_method)
        {

            String dllpath = (String)Program.g_DLL_Name_Map[dll_name];
            if (dllpath == null) { return "YOU ARE JUST TRYING TO RUN DLL OF A VACANT SLOT"; };
            DEWDAssemblyLoadContext alc = (DEWDAssemblyLoadContext)Program.g_DLL_Map[dllpath];
            var asmlist = alc.Assemblies.ToList();
            Type t_type = asmlist[0].GetType(dll_class);
            Object t_object = asmlist[0].CreateInstance(dll_class);
            var t_method = t_type.GetMethod(dll_method);
            dynamic res = t_method.Invoke(t_object, new Object[] { this.Request });
            return res;
        }


    }

    class DEWDAssemblyLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;

        public DEWDAssemblyLoadContext(string mainAssemblyToLoadPath) : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(mainAssemblyToLoadPath);
        }

        protected override Assembly Load(AssemblyName name)
        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(name);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }
    }
}