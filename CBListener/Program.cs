using Couchbase.Lite;
using Couchbase.Lite.Listener;
using Couchbase.Lite.Listener.Tcp;
using Mono.Zeroconf.Providers.Bonjour;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBListener
{
   class Program
  {
    static void Main(string[] args)
    {
      var databaseLocation = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "databases"));
      Manager manager = new Manager(databaseLocation, Manager.DefaultOptions);
      var database = manager.GetDatabase("badjson");
      Console.WriteLine("Document count " + database.GetDocumentCount());

      CouchbaseLiteServiceListener listener = new CouchbaseLiteTcpListener(manager, 25610, "badjson");
      listener.Start();

      Console.ReadLine();

      listener.Stop();
    }
  }
}
