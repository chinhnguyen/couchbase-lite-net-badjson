using Couchbase.Lite;
using Couchbase.Lite.Auth;
using Couchbase.Lite.Listener;
using Mono.Zeroconf.Providers.Bonjour;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBClient
{
  

  class Program
  {
    
    static void Main(string[] args)
    {
      var databaseLocation = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "databases"));
      Manager manager = new Manager(databaseLocation, Manager.DefaultOptions);

      Database database = manager.GetExistingDatabase("pulltarget");
      if (database != null)
      {
        try
        {
          database.Delete();
        }
        catch (Exception e)
        {
          Console.WriteLine("Cannot delete database " + e.Message);
        }
      }
      database = manager.GetDatabase("pulltarget");
      Console.WriteLine("Document count " + database.GetDocumentCount());


      var url = new Uri("http://localhost:25610/badjson");
      //var url = new Uri("http://192.168.1.40:25610/kiolyn");
      var pull = database.CreatePullReplication(url);
      pull.Continuous = true;
      EventHandler<ReplicationChangeEventArgs> changedCallback = null;
      changedCallback = (sender, e) =>
      {
        var active = pull.Status == ReplicationStatus.Active;
        if (active)
          ;
        else
          Console.WriteLine("Document count " + database.GetDocumentCount());
      };
      pull.Changed += changedCallback;
      pull.Start();

      Console.ReadLine();

      pull.Changed -= changedCallback;
      pull.Stop();
    }
  }
}
