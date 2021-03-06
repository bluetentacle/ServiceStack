﻿using System;
using System.Runtime.Serialization;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.ServiceModel;

namespace ServiceStack.WebHost.IntegrationTests.Services
{
	//Request DTO
	[RestService("/geoinfo")]
	[DataContract]
	public class GeoInfo
	{
		[DataMember]
		public string AppToken { get; set; }

		[DataMember]
		public int OrderId { get; set; }

		[DataMember]
		public GeoPoint GeoCode { get; set; }
	}

	[Serializable]
	public class GeoPoint
	{
		public long t { get; set; }
		public decimal latitude { get; set; }
		public decimal longitude { get; set; }
	}

	//Response DTO
	public class GeoInfoResponse : IHasResponseStatus
	{
		public string Result { get; set; }
		public ResponseStatus ResponseStatus { get; set; } //Where Exceptions get auto-serialized
	}

	public class GeoInfoService : RestServiceBase<GeoInfo>
	{
		public override object OnPost(GeoInfo request)
		{
			return new GeoInfoResponse
			{
				Result = "Incoming Geopoint: Latitude="
					+ request.GeoCode.latitude.ToString()
					+ " Longitude="
					+ request.GeoCode.longitude.ToString()
			};
		}
	}
}