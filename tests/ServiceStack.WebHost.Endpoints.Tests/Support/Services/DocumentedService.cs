using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.ServiceHost;

namespace ServiceStack.WebHost.Endpoints.Tests.Support.Services
{
	/// <summary>
	///		A mock service that is documented.
	/// </summary>
	/// <remarks>
	///		Remarks for <see cref="DocumentedService"/>.
	/// </remarks>
	[RestService("/DocumentationTests/DocumentedService")]
	public class DocumentedService
	{
		/// <summary>
		///		Gets or sets a string.
		/// </summary>
		/// <value>
		///		A <see cref="String"/> value; never <see langword="null"/>.
		/// </value>
		public string StringProperty { get; set; }

		/// <summary>
		///		Gets or sets an array property.
		/// </summary>
		/// <value>
		///		An array of integers; never <see langword="null"/>.
		/// </value>
		public int[] ArrayProperty { get; set; }

		/// <summary>
		///		Gets or sets a child object.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown whenever.</exception>
		public DocumentedChild Child { get; set; }

		private int PrivateProperty { get; set; }

		internal Guid InternalProperty { get; set; }

		protected DateTime ProtectedProperty { get; set; }

		public static string StaticProperty { get; set; }
	}

	/// <summary>
	///		A child object to <see cref="DocumentedService"/>.
	/// </summary>
	public class DocumentedChild
	{
		public string StringProperty { get; set; }
	}

	/// <summary>
	///		Response to <see cref="DocumentedService"/>.
	/// </summary>
	public class DocumentedServiceResponse
	{
		/// <summary>
		///		A string property.
		/// </summary>
		public string StringProperty { get; set; }
	}
}
