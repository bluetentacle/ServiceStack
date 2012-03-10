using System.Collections.Generic;

namespace ServiceStack.WebHost.Endpoints.Metadata
{
	/// <summary>
	///		Represents a documented DTO, such as a request or a response.
	/// </summary>
	public sealed class DtoDoc: DocumentedCodeElementBase
	{
		/// <summary>
		/// 	<para>Initializes an instance of the <see cref="DtoDoc"/> class.</para>
		/// </summary>
		internal DtoDoc()
		{
			Properties = new List<PropertyDoc>();
		}

		/// <summary>
		///		Gets the collection of objects encapsulating documentation on DTO properties.
		/// </summary>
		/// <value>
		///		A collection of <see cref="Properties"/> objects; could be empty, but never <see langword="null"/>.
		/// </value>
		public List<PropertyDoc> Properties { get; private set; }
	}
}
