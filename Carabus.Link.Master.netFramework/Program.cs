using System;
using System.IO;
using System.Linq;
using System.Net;

namespace Carabus.Link.Master.netFramework
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var ip = args.ElementAtOrDefault(0) ?? "192.168.1.200:80";
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add($"http://{ip}/");
            listener.Start();
            Console.WriteLine($"Started listening on: {listener.Prefixes.ElementAtOrDefault(0)}");
            while (true)
            {
                try
                {
                    ResolveFile(listener.GetContext());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            Console.WriteLine("Exiting...");
        }

        private static void ResolveFile(HttpListenerContext hlc)
        {
            var og = AppDomain.CurrentDomain.BaseDirectory + '\\';
            var str = hlc.Request.Url.PathAndQuery.Remove(0, 1);
            var dir = og + str.Remove(str.LastIndexOf('.'));
            Console.WriteLine($"Writing file to dir: {dir}");
            Directory.CreateDirectory(dir);
            var stream = hlc.Request.InputStream;
            var fs = new FileStream(dir + '\\' + str, FileMode.Create);
            stream.CopyTo(fs);
            hlc.Response.Close();
            fs.Dispose();
            Console.WriteLine("Ok!");
        }
    }
}