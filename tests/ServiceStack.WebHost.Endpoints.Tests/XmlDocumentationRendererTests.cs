using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using NUnit.Framework;
using ServiceStack.WebHost.Endpoints.Support.Metadata.Controls;
using ServiceStack.WebHost.Endpoints.Metadata;

namespace ServiceStack.WebHost.Endpoints.Tests
{
	[TestFixture]
	public sealed class XmlDocumentationRendererTests
	{
		[Test]
		public void Can_render_empty()
		{
			var doc = XmlDocumentationRenderer.GetRenderedDocumentation(GetCodeElement(null));

			Assert.That(doc, Is.EqualTo(String.Empty));
		}

		[Test]
		public void Can_render_sections()
		{
			var doc = XmlDocumentationRenderer.GetRenderedDocumentation(
				GetCodeElement(@"<summary>This is a new world.</summary><remarks>And we are stuck in it.</remarks>"), noIndentation: true);

			Assert.That(doc, Is.EqualTo("<h1>Summary</h1>This is a new world.<h1>Remarks</h1>And we are stuck in it."));
		}

		[Test]
		public void Can_render_sections_with_summary_header_suppressed()
		{
			var doc = XmlDocumentationRenderer.GetRenderedDocumentation(
				GetCodeElement(@"<summary>This is a new world.</summary><remarks>And we are stuck in it.</remarks>"), noIndentation: true, suppressSummaryHeader: true);

			Assert.That(doc, Is.EqualTo("This is a new world.<h1>Remarks</h1>And we are stuck in it."));
		}

		[Test]
		public void Can_render_sections_with_different_tag()
		{
			var doc = XmlDocumentationRenderer.GetRenderedDocumentation(
				GetCodeElement(@"<summary>This is a new world.</summary><remarks>And we are stuck in it.</remarks>"), sectionTitleTag: "h2", noIndentation: true);

			Assert.That(doc, Is.EqualTo("<h2>Summary</h2>This is a new world.<h2>Remarks</h2>And we are stuck in it."));
		}

		[Test]
		public void Can_render_sections_with_css_class()
		{
			var doc = XmlDocumentationRenderer.GetRenderedDocumentation(
				GetCodeElement(@"<summary>This is a new world.</summary><remarks>And we are stuck in it.</remarks>"), sectionTitleCssClass: "Style", noIndentation: true);

			Assert.That(doc, Is.EqualTo("<h1 class=\"Style\">Summary</h1>This is a new world.<h1 class=\"Style\">Remarks</h1>And we are stuck in it."));
		}

		[Test]
		public void Can_render_para()
		{
			var doc = XmlDocumentationRenderer.GetRenderedDocumentation(
				GetCodeElement("<summary><para>Paragraph 1</para><para>Paragraph 2</para></summary>"), elementName: "summary", noIndentation: true);

			Assert.That(doc, Is.EqualTo("<p>Paragraph 1</p><p>Paragraph 2</p>"));
		}
		[Test]
		public void Can_render_with_indentations()
		{
			var doc = XmlDocumentationRenderer.GetRenderedDocumentation(
				GetCodeElement("<summary><para>Hello</para></summary><remarks><para>World</para></remarks>"));

			Assert.That(doc, Is.EqualTo("<h1>\r\n\tSummary\r\n</h1><p>Hello</p><h1>\r\n\tRemarks\r\n</h1><p>World</p>"));
		}

		[Test]
		public void Can_render_escaped_characters()
		{
			var doc = XmlDocumentationRenderer.GetRenderedDocumentation(
				GetCodeElement("<summary>&amp;&quot;</summary>"), elementName: "summary", noIndentation: true);

			Assert.That(doc, Is.EqualTo("&amp;&quot;"));
		}

		[Test]
		public void Can_render_bulleted_list()
		{
			var doc = XmlDocumentationRenderer.GetRenderedDocumentation(
				GetCodeElement(
				@"<summary>
					<list type=""bullet"">
						<item>Hello</item>
						<item>World</item>
					</list>
				</summary>"), elementName: "summary", noIndentation: true);

			Assert.That(doc, Is.EqualTo("<ul><li>Hello</li><li>World</li></ul>"));
		}

		[Test]
		public void Can_render_numbered_list()
		{
			var doc = XmlDocumentationRenderer.GetRenderedDocumentation(
				GetCodeElement(
				@"<summary>
					<list type=""number"">
						<item>Hello</item>
						<item>World</item>
					</list>
				</summary>"), elementName: "summary", noIndentation: true);

			Assert.That(doc, Is.EqualTo("<ol><li>Hello</li><li>World</li></ol>"));
		}

		[Test]
		public void Can_render_definition_list()
		{
			var doc = XmlDocumentationRenderer.GetRenderedDocumentation(
				GetCodeElement(
				@"<summary>
					<list>
						<item>
							<term>1</term>
							<description>Hello</description>
						</item>
						<item>
							<term>2</term>
							<description>World</description>
						</item>
					</list>
				</summary>"), elementName: "summary", noIndentation: true);

			Assert.That(doc, Is.EqualTo("<ul><li><strong>1: </strong>Hello</li><li><strong>2: </strong>World</li></ul>"));
		}

		[Test]
		public void Can_render_single_column_table()
		{
			var doc = XmlDocumentationRenderer.GetRenderedDocumentation(
				GetCodeElement(
				@"<summary>
					<list type=""table"">
						<item>Hello</item>
						<item>World</item>
					</list>
				</summary>"), elementName: "summary", noIndentation: true);

			Assert.That(doc, Is.EqualTo("<table><tr><td>Hello</td></tr><tr><td>World</td></tr></table>"));
		}

		[Test]
		public void Can_render_multi_column_table()
		{
			var doc = XmlDocumentationRenderer.GetRenderedDocumentation(
				GetCodeElement(
				@"<summary>
					<list type=""table"">
						<item>
							<term>1</term>
							<description>Hello</description>
						</item>
						<item>
							<term>2</term>
							<description>World</description>
						</item>
					</list>
				</summary>"), elementName: "summary", noIndentation: true);

			Assert.That(doc, Is.EqualTo("<table><tr><td>1</td><td>Hello</td></tr><tr><td>2</td><td>World</td></tr></table>"));
		}

		[Test]
		public void Can_render_table_with_header()
		{
			var doc = XmlDocumentationRenderer.GetRenderedDocumentation(
				GetCodeElement(
				@"<summary>
					<list type=""table"">
						<listheader>
							<term>Name</term>
							<description>Value</description>
						</listheader>
						<item>
							<term>1</term>
							<description>Hello</description>
						</item>
						<item>
							<term>2</term>
							<description>World</description>
						</item>
					</list>
				</summary>"), elementName: "summary", noIndentation: true);

			Assert.That(doc, Is.EqualTo("<table><tr><th>Name</th><th>Value</th></tr><tr><td>1</td><td>Hello</td></tr><tr><td>2</td><td>World</td></tr></table>"));
		}

		[Test]
		public void Can_render_see_without_contents()
		{
			var doc = XmlDocumentationRenderer.GetRenderedDocumentation(
				GetCodeElement("<summary><see cref=\"T:System.String\"/></summary>"), elementName: "summary", noIndentation: true);

			Assert.That(doc, Is.EqualTo("<strong>String</strong>"));
		}

		[Test]
		public void Can_render_see_with_contents()
		{
			var doc = XmlDocumentationRenderer.GetRenderedDocumentation(
				GetCodeElement("<summary><see cref=\"System.String\">Hello World</see></summary>"), elementName: "summary", noIndentation: true);

			Assert.That(doc, Is.EqualTo("<strong>Hello World</strong>"));
		}

		[Test]
		public void Can_render_langword()
		{
			var doc = XmlDocumentationRenderer.GetRenderedDocumentation(
				GetCodeElement("<summary><see langword=\"null\"/></summary>"), elementName: "summary", noIndentation: true);

			Assert.That(doc, Is.EqualTo("<strong>null</strong>"));
		}

		[Test]
		public void Can_render_paramref()
		{
			var doc = XmlDocumentationRenderer.GetRenderedDocumentation(
				GetCodeElement("<summary><paramref name=\"name\"/></summary>"), elementName: "summary", noIndentation: true);

			Assert.That(doc, Is.EqualTo("<strong>name</strong>"));
		}

		[Test]
		public void Can_render_typeparamref()
		{
			var doc = XmlDocumentationRenderer.GetRenderedDocumentation(
				GetCodeElement("<summary><typeparamref name=\"T\"/></summary>"), elementName: "summary", noIndentation: true);

			Assert.That(doc, Is.EqualTo("<strong>T</strong>"));
		}

		[Test]
		public void Can_render_c()
		{
			var doc = XmlDocumentationRenderer.GetRenderedDocumentation(
				GetCodeElement("<summary><c>2112298109</c></summary>"), elementName: "summary", noIndentation: true);

			Assert.That(doc, Is.EqualTo("<code>2112298109</code>"));
		}

		[Test]
		public void Can_render_code()
		{
			var doc = XmlDocumentationRenderer.GetRenderedDocumentation(
				GetCodeElement("<summary><code>2112298109</code></summary>"), elementName: "summary", noIndentation: true);

			Assert.That(doc, Is.EqualTo("<pre>2112298109</pre>"));
		}

		private DocumentedCodeElementBase GetCodeElement(string xml)
		{
			var xmlDoc = xml != null ? (XElement)XDocument.Parse("<member>" + xml + "</member>").FirstNode : null;

			return new DtoDoc()
			       	{
			       		Name = "e2ba7bbf412b4975aa2586abe44aa09f",
			       		XmlDocumentation = xmlDoc
			       	};
		}
	}
}
