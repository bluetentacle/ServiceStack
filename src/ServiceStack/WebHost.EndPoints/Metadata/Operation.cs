using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceStack.WebHost.Endpoints.Metadata
{
	/// <summary>
	///		Encapsulates the metadata of an operation.
	/// </summary>
	public sealed class Operation
	{
		/// <summary>
		/// 	<para>Initializes an instance of the <see cref="Operation"/> class.</para>
		/// </summary>
		internal Operation()
		{
		}

		/// <summary>
		///		Gets the name of this operation.
		/// </summary>
		/// <value>
		///		A <see cref="String"/> value providing the name of this operation;
		///		never <see langword="null"/>.
		/// </value>
		public string Name { get; internal set; }

		/// <summary>
		///		Gets the type of the request DTO associated with this operation.
		/// </summary>
		/// <value>
		///		The <see cref="Type"/> of the request DTO; never <see langword="null"/>.
		/// </value>
		public Type RequestType { get; internal set; }

		/// <summary>
		///		Gets the documentation of this operation.
		/// </summary>
		/// <value>
		///		An <see cref="OperationDoc"/> instance; never <see langword="null"/>.
		/// </value>
		public OperationDoc Doc { get; internal set; }
	}
}
