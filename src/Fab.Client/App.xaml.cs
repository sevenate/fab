using System.Net;
using System.Net.Browser;

namespace Fab.Client
{
	public partial class App
	{
		public App()
		{
			InitializeComponent();

			// Required for support HTTP response code 500 (Internal Server Error)
			// with SOAP Faults xml information returned from WCF service in case of server side error.
			// Details: http://blogs.msdn.com/carlosfigueira/archive/2009/08/15/fault-support-in-silverlight-3.aspx
			// Note: should always return "true", unless the prefix has been previously registered.
			WebRequest.RegisterPrefix("https://", WebRequestCreator.ClientHttp);
		}
	}
}