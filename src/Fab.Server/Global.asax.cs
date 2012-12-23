using System;
using System.Web;
using Common.Logging;

namespace Fab.Server
{
	public class Global : HttpApplication
	{
		protected void Application_Start(object sender, EventArgs e)
		{
			// Note: HttpContext.Current.Request.PhysicalApplicationPath - is also working.
			// Note: HttpContext.Current.Server.MapPath(".") in WCF service - is also working.
			// Note: System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath - is also working.
			// Server.MapPath("~/");

			var log = LogManager.GetCurrentClassLogger();
			log.Info("Application is Started.");
		}

		protected void Session_Start(object sender, EventArgs e)
		{
		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			// Only for remote connections
			// NOTE: prefer such restriction via configuration by IIS rewrite module rule instead of code bellow
			/*
			if (!HttpContext.Current.Request.IsSecureConnection && !HttpContext.Current.Request.IsLocal)
			{
				Response.Redirect("https://" + Request.ServerVariables["HTTP_HOST"]
				                  + HttpContext.Current.Request.RawUrl);
			}
			*/
		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{
		}

		protected void Application_Error(object sender, EventArgs e)
		{
			// get reference to the source of the exception chain
			var ex = Server.GetLastError().GetBaseException();

			// log the details of the exception and page state
			var log = LogManager.GetCurrentClassLogger();
			log.Fatal(
				"Application error:\n" +
				"\nFORM: " + Request.Form +
				"\nQUERYSTRING:" + Request.QueryString + "\n",
				ex);
		}

		protected void Session_End(object sender, EventArgs e)
		{
		}

		protected void Application_End(object sender, EventArgs e)
		{
			var log = LogManager.GetCurrentClassLogger();
			log.Info("Application is Down.");
		}
	}
}