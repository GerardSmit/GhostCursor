using CefSharp;
using CefSharp.WinForms;
using HttpStack;
using HttpStack.CefSharp;
using HttpStack.StaticFiles;
using Microsoft.Extensions.FileProviders;

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

		var app = new HttpStackBuilder();
		var provider = new EmbeddedFileProvider(typeof(DefaultForm).Assembly, "Sandbox.wwwroot");

		app.UseStaticFiles(provider);

		app.Run(async context =>
		{
			await context.Response.WriteAsync($"Invalid request {context.Request.Path}. Available routes:");

			foreach (var content in provider.GetDirectoryContents("/"))
			{
				await context.Response.WriteAsync("\n" + content.Name);
			}
		});

		settings.RegisterScheme(new CefCustomScheme
		{
			SchemeName = "browser",
			SchemeHandlerFactory = app.ToSchemeHandlerFactory(),
			IsSecure = true
		});

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
