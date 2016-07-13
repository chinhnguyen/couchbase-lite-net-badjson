using Couchbase.Lite;
using Couchbase.Lite.Util;
//using Kiolyn.Security;
using System;
using System.Diagnostics;
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

      Couchbase.Lite.Util.Log.SetLogger(new ColorConsoleLogger());
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
  class ColorConsoleLogger : ILogger
  {
    static ColorConsoleLogger()
    {
      Console.BackgroundColor = ConsoleColor.Black;
      Console.ForegroundColor = ConsoleColor.White;

    }

    void WriteOptionalTraceInfo()
    {
      var traceInfo = new TraceEventCache();
      PrintDateTime(traceInfo);
      PrintThreadId(traceInfo);
    }

    void PrintThreadId(TraceEventCache info)
    {
      Console.Out.Write("[");
      Console.Out.Write(info.ThreadId);
      Console.Out.Write("]");
      Console.Out.Write(" ");
    }

    void PrintDateTime(TraceEventCache info)
    {
      Console.Out.Write(info.DateTime.ToLocalTime().ToString("yyyy-M-d hh:mm:ss.fffK"));
      Console.Out.Write(" ");
    }

    static string LevelToString(SourceLevels level)
    {
      switch (level)
      {
        case SourceLevels.ActivityTracing:
          return "DEBUG)";
        case SourceLevels.Verbose:
          return "VERBOSE)";
        case SourceLevels.Information:
          return "INFO)";
        case SourceLevels.Warning:
          return "WARN)";
        case SourceLevels.Error:
          return "ERROR)";
        default:
          return "UNKNOWN LEVEL)";
      }
    }

    public void WriteLine(SourceLevels level, string message, string category)
    {
      Console.Out.Write(String.Format("{0} {1}", LevelToString(level), category));
      Console.Out.Write(": ");
      Console.Out.Write(message);
      Console.Out.Write(Environment.NewLine);
      Console.Out.Flush();
    }

    private static Exception Flatten(Exception inE)
    {
      var ae = inE as AggregateException;
      if (ae == null)
      {
        return inE;
      }

      return ae.Flatten().InnerException;
    }

    #region ILogger

    public void V(string tag, string msg)
    {
      var tmp = Console.ForegroundColor;
      Console.ForegroundColor = ConsoleColor.DarkGray;
      WriteLine(SourceLevels.Verbose, msg, tag);
      Console.ForegroundColor = tmp;
    }

    public void V(string tag, string msg, Exception tr)
    {
      var tmp = Console.ForegroundColor;
      Console.ForegroundColor = ConsoleColor.DarkGray;
      WriteLine(SourceLevels.Verbose, String.Format("{0}:\r\n{1}", msg, Flatten(tr)), tag);
      Console.ForegroundColor = tmp;
    }

    public void V(string tag, string format, params object[] args)
    {
      V(tag, string.Format(format, args));
    }

    public void D(string tag, string msg)
    {
      var tmp = Console.ForegroundColor;
      Console.ForegroundColor = ConsoleColor.DarkGray;
      WriteLine(SourceLevels.ActivityTracing, msg, tag);
      Console.ForegroundColor = tmp;
    }

    public void D(string tag, string msg, Exception tr)
    {
      var tmp = Console.ForegroundColor;
      Console.ForegroundColor = ConsoleColor.DarkGray;
      WriteLine(SourceLevels.ActivityTracing, String.Format("{0}:\r\n{1}", msg, Flatten(tr)), tag);
      Console.ForegroundColor = tmp;
    }

    public void D(string tag, string format, params object[] args)
    {
      D(tag, string.Format(format, args));
    }

    public void I(string tag, string msg)
    {
      WriteLine(SourceLevels.Information, msg, tag);
    }

    public void I(string tag, string msg, Exception tr)
    {
      WriteLine(SourceLevels.Information, String.Format("{0}:\r\n{1}", msg, Flatten(tr)), tag);
    }

    public void I(string tag, string format, params object[] args)
    {
      I(tag, string.Format(format, args));
    }

    public void W(string tag, string msg)
    {
      var tmp = Console.ForegroundColor;
      Console.ForegroundColor = ConsoleColor.DarkYellow;
      WriteLine(SourceLevels.Warning, msg, tag);
      Console.ForegroundColor = tmp;
    }

    public void W(string tag, Exception tr)
    {

    }

    public void W(string tag, string msg, Exception tr)
    {
      var tmp = Console.ForegroundColor;
      Console.ForegroundColor = ConsoleColor.DarkYellow;
      WriteLine(SourceLevels.Warning, String.Format("{0}:\r\n{1}", msg, Flatten(tr)), tag);
      Console.ForegroundColor = tmp;
    }

    public void W(string tag, string format, params object[] args)
    {
      W(tag, string.Format(format, args));
    }

    public void E(string tag, string msg)
    {
      var tmp = Console.ForegroundColor;
      Console.ForegroundColor = ConsoleColor.Red;
      WriteLine(SourceLevels.Error, msg, tag);
      Console.ForegroundColor = tmp;
    }

    public void E(string tag, string msg, Exception tr)
    {
      var tmp = Console.ForegroundColor;
      Console.ForegroundColor = ConsoleColor.Red;
      WriteLine(SourceLevels.Error, String.Format("{0}:\r\n{1}", msg, Flatten(tr)), tag);
      Console.ForegroundColor = tmp;
    }

    public void E(string tag, string format, params object[] args)
    {
      E(tag, string.Format(format, args));
    }

    #endregion
  }
}
