using CefSharp;
using CefSharp.WinForms;

namespace Sandbox;

static class Program
{
	/// <summary>
	///  The main entry point for the application.
	/// </summary>
	[STAThread]
	static void Main()
	{

		var settings = new CefSettings()
		{
			LogFile = "Debug.log", //You can customise this path
			LogSeverity = LogSeverity.Default // You can change the log level
		};

		Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

		// DPI aware
		Application.EnableVisualStyles();
		Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

		// To customize application configuration such as set high DPI settings or default font,
		// see https://aka.ms/applicationconfiguration.
		ApplicationConfiguration.Initialize();
		Application.Run(new DefaultForm());
	}
}
