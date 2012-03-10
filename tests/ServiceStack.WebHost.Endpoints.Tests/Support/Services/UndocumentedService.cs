using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.ServiceHost;

namespace ServiceStack.WebHost.Endpoints.Tests.Support.Services
{
	[RestService("/DocumentationTests/UndocumentedService")]
	public class UndocumentedService
	{
		public string StringProperty { get; set; }

		public int[] ArrayProperty { get; set; }
	}
}
