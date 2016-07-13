using Couchbase.Lite;
//using Kiolyn.Security;
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
//using Willbe.Log;


namespace Kiolyn.Desktop
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      FileStream ostrm;
      StreamWriter writer;
      TextWriter oldOut = Console.Out;
      try
      {
        ostrm = new FileStream("./badjson.txt", FileMode.OpenOrCreate, FileAccess.Write);
        writer = new StreamWriter(ostrm);
      }
      catch (Exception ev)
      {
        Console.WriteLine("Cannot open badjson.txt for writing");
        Console.WriteLine(ev.Message);
        return;
      }
      //Console.SetOut(writer);

      var databaseLocation = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "databases"));
      Manager manager = new Manager(databaseLocation, Manager.DefaultOptions);

      Database database = manager.GetExistingDatabase("pulltarget1");
      if (database != null)
      {
        try
        {
          database.Delete();
        }
        catch (Exception ev)
        {
          Console.WriteLine("Cannot delete database " + ev.Message);
        }
      }
      database = manager.GetDatabase("pulltarget1");
      Console.WriteLine("Document count " + database.GetDocumentCount());

      var url = new Uri("http://localhost:25610/badjson");
      //var url = new Uri("http://192.168.1.40:25610/kiolyn");
      var pull = database.CreatePullReplication(url);
      pull.Continuous = true;
      EventHandler<ReplicationChangeEventArgs> changedCallback = null;
      changedCallback = (sender, ev) =>
      {
        var active = pull.Status == ReplicationStatus.Active;
        if (active)
          ;
        else
          Console.WriteLine("Document count " + database.GetDocumentCount());
      };
      pull.Changed += changedCallback;
      pull.Start();
    }

    private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
    }
  }
}
