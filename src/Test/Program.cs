using System;

using log4net;


// Configure log4net using the .config file
[assembly: log4net.Config.XmlConfigurator(Watch = true)]


namespace Test
{
    using Company.Product.BusinessLogic;
    using Company.Product.ServiceTester;


    class Program
    {
		// Create a logger for use in this class.
		static readonly ILog _log = 
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        static void Main(string[] args)
		{
            ConsoleKeyInfo key = Console.ReadKey();
            while (key.Key != ConsoleKey.X)
			{
				DoLog();
			    DoWinDebug();
				key = Console.ReadKey();
			}
        }

        static void DoWinDebug()
        {
			Console.WriteLine("Doing WinDebug!");

            System.Diagnostics.Debug.WriteLine("This is a call to System.Diagnostics.Debug");
            System.Diagnostics.Trace.WriteLine("This is a call to System.Diagnostics.Trace");
        }

        static void DoLog()
		{
			Console.WriteLine("Doing Log!");

			if (_log.IsErrorEnabled)
				_log.Error("This is an Error...");

			if (_log.IsDebugEnabled)
				for (int i = 0; i < 10; i++)
					_log.Debug("This is a simple log!");

			if (_log.IsErrorEnabled)
				_log.Error("This is an Error...");

			if (_log.IsInfoEnabled)
				_log.Info("This is an Info...");


			_log.Warn("This is a Warning...");
			_log.Fatal("This is a Fatal...");


			DummyManager dm = new DummyManager();
			dm.DoIt();

			DummyTester dt = new DummyTester();
			dt.DoIt();
		}
    }
}


namespace Company.Product.BusinessLogic
{

	public class DummyManager
	{
		public static readonly ILog _log =
			LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


		public DummyManager()
		{
			if (_log.IsInfoEnabled)
				_log.Info("Dummy Manager ctor");
		}

		public void DoIt()
		{
			if (_log.IsDebugEnabled)
				_log.Debug("DM: Do It, Do It Now!!");

			_log.Warn("This is a Warning from DM...");
			_log.Error("This is an Error from DM...");
			_log.Fatal("This is a Fatal from DM...");
		}
	}
}

namespace Company.Product.ServiceTester
{

	public class DummyTester
	{
		public static readonly ILog _log =
			LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


		public DummyTester()
		{
			if (_log.IsInfoEnabled)
				_log.Info("Dummy Tester ctor");
		}

		public void DoIt()
		{
			if (_log.IsDebugEnabled)
				_log.Debug("DT: Do It, Do It Now!!");

			_log.Warn("This is a Warning from DT...");
			_log.Error("This is an Error from DT...");
			_log.Fatal("This is a Fatal from DT...");
		}
	}
}


