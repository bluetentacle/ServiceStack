using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Text;
using NUnit.Framework;
using ServiceStack.ServiceHost;
using ServiceStack.WebHost.Endpoints.Metadata;
using ServiceStack.WebHost.Endpoints.Tests.Support.Services;

namespace ServiceStack.WebHost.Endpoints.Tests
{
	[TestFixture]
	public class OperationDocTests
	{
		[Test]
		public void Can_parse_documented_service()
		{
			var doc = OperationDoc.GetForRequestType(typeof (DocumentedService));

			Assert.AreEqual("DocumentedService", doc.Name);
			
			Assert.IsNotNull(doc.Request);
			Assert.AreEqual("DocumentedService", doc.Request.Name);
			Assert.IsNotNull(doc.Request.XmlDocumentation);
			Assert.IsNotNull(doc.Request.XmlDocumentation.XPathSelectElement("summary"));
			Assert.IsNotNull(doc.Request.XmlDocumentation.XPathSelectElement("remarks"));
			
			Assert.AreEqual(3, doc.Request.Properties.Count);

			Assert.AreEqual("ArrayProperty", doc.Request.Properties[0].Name);
			Assert.AreEqual("Int32[]", doc.Request.Properties[0].PropertyType);
			Assert.IsNotNull(doc.Request.Properties[0].XmlDocumentation);
			Assert.IsNotNull(doc.Request.Properties[0].XmlDocumentation.XPathSelectElement("summary"));
			Assert.IsNotNull(doc.Request.Properties[0].XmlDocumentation.XPathSelectElement("value"));

			Assert.AreEqual("Child", doc.Request.Properties[1].Name);
			Assert.AreEqual("DocumentedChild", doc.Request.Properties[1].PropertyType);
			Assert.IsNotNull(doc.Request.Properties[1].XmlDocumentation);
			Assert.IsNotNull(doc.Request.Properties[1].XmlDocumentation.XPathSelectElement("summary"));
			Assert.IsNull(doc.Request.Properties[1].XmlDocumentation.XPathSelectElement("value"));
			Assert.IsNull(doc.Request.Properties[1].XmlDocumentation.XPathSelectElement("exception"));

			Assert.AreEqual("StringProperty", doc.Request.Properties[2].Name);
			Assert.AreEqual("String", doc.Request.Properties[2].PropertyType);
			Assert.IsNotNull(doc.Request.Properties[2].XmlDocumentation);
			Assert.IsNotNull(doc.Request.Properties[2].XmlDocumentation.XPathSelectElement("summary"));
			Assert.IsNotNull(doc.Request.Properties[2].XmlDocumentation.XPathSelectElement("value"));

			Assert.IsNotNull(doc.Responses);
			Assert.AreEqual(1, doc.Responses.Count);

			var response = doc.Responses[0];
			Assert.AreEqual("DocumentedServiceResponse", response.Name);
			Assert.IsNotNull(response.XmlDocumentation);
			Assert.AreEqual(1, response.Properties.Count);
			Assert.AreEqual("StringProperty", response.Properties[0].Name);
			Assert.AreEqual("String", response.Properties[0].PropertyType);
			Assert.IsNotNull(response.Properties[0].XmlDocumentation);
		}

		[Test]
		public void Can_handle_undocumented_service()
		{
			var doc = OperationDoc.GetForRequestType(typeof(UndocumentedService));

			Assert.AreEqual("UndocumentedService", doc.Name);

			Assert.IsNotNull(doc.Request);
			Assert.AreEqual("UndocumentedService", doc.Request.Name);
			Assert.IsNull(doc.Request.XmlDocumentation);

			Assert.AreEqual(2, doc.Request.Properties.Count);

			Assert.AreEqual("ArrayProperty", doc.Request.Properties[0].Name);
			Assert.AreEqual("Int32[]", doc.Request.Properties[0].PropertyType);
			Assert.IsNull(doc.Request.Properties[0].XmlDocumentation);

			Assert.AreEqual("StringProperty", doc.Request.Properties[1].Name);
			Assert.AreEqual("String", doc.Request.Properties[1].PropertyType);
			Assert.IsNull(doc.Request.Properties[1].XmlDocumentation);
		}
	}
}
