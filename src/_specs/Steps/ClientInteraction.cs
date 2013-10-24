﻿#region FreeBSD

// Copyright (c) 2013, The Tribe
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
//  * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
//  * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the
//    documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
// TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using System.Net;

using HBase.Stargate.Client.Api;
using HBase.Stargate.Client.MimeConversion;
using HBase.Stargate.Client.Models;

using Moq;

using Patterns.Testing.Moq;

using RestSharp;
using RestSharp.Injection;

using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

using _specs.Models;

namespace _specs.Steps
{
	[Binding]
	public class ClientInteraction
	{
		private readonly IMoqContainer _container;
		private readonly HBaseContext _hBase;
		private readonly RestContext _rest;

		public ClientInteraction(HBaseContext hBase, RestContext rest, IMoqContainer container)
		{
			_hBase = hBase;
			_rest = rest;
			_container = container;
		}

		[Given(@"I have everything I need to test a disconnected HBase client, with the following options:")]
		public void SetupClient(Table options)
		{
			Mock<IRestSharpFactory> factoryMock = _container.Mock<IRestSharpFactory>();

			factoryMock.Setup(factory => factory.CreateRequest(It.IsAny<string>(), It.IsAny<Method>()))
				.Returns<string, Method>((resource, method) => (_rest.Request = new RestRequest(resource, method)));

			Mock<IRestClient> clientMock = _container.Mock<IRestClient>();

			Mock<IRestResponse> responseMock = _container.Mock<IRestResponse>();
			responseMock.SetupGet(response => response.StatusCode).Returns(HttpStatusCode.OK);

			clientMock.Setup(client => client.Execute(It.IsAny<IRestRequest>()))
				.Returns(() => responseMock.Object);

			factoryMock.Setup(factory => factory.CreateClient(It.IsAny<string>()))
				.Returns(() => clientMock.Object);

			_container.Update<IStargateOptions>(options.CreateInstance<StargateOptions>());

			_container.Update<IMimeConverter, XmlMimeConverter>();
			_container.Update<IMimeConverterFactory, MimeConverterFactory>();
			_container.Update<IResourceBuilder, ResourceBuilder>();
			_container.Update<ISimpleValueConverter, SimpleValueConverter>();

			_container.Update<IStargate, Stargate>();
		}

		[Given(@"I have an HBase client")]
		public void CreateClient()
		{
			_hBase.Stargate = _container.Create<IStargate>();
		}

		[Given(@"I have an identifier consisting of a ([^,]*)")]
		public void SetIdentifier(string table)
		{
			_hBase.Identifier = new Identifier {Table = table};
		}

		[Given(@"I have an identifier consisting of a ([^,]*), a ([^,]*), a ([^,]*), a ([^,]*), and a ([^,]*)")]
		public void SetIdentifier(string table, string row, string column, string qualifier, string timestamp)
		{
			_hBase.Identifier = new Identifier
			{
				Table = table,
				Row = row,
				Cell = new HBaseCellDescriptor
				{
					Column = column,
					Qualifier = qualifier
				},
				Timestamp = timestamp.ToNullableInt64()
			};
		}

		[Given(@"I have an identifier consisting of a ([^,]*), a ([^,]*), a ([^,]*), and a ([^,]*)")]
		public void SetIdentifier(string table, string row, string column, string qualifier)
		{
			_hBase.Identifier = new Identifier
			{
				Table = table,
				Row = row,
				Cell = new HBaseCellDescriptor
				{
					Column = column,
					Qualifier = qualifier
				}
			};
		}

		[Given(@"I have a cell query consisting of a (.*), a (.*), a (.*), a (.*), a (.*) timestamp, a (.*) timestamp, and a (.*) number of results")]
		public void SetQuery(string table, string row, string column, string qualifier, string beginTimestamp, string endTimestamp, string maxResults)
		{
			_hBase.Query = new CellQuery
			{
				Table = table,
				Row = row,
				Cells = new[]
				{
					new HBaseCellDescriptor
					{
						Column = column,
						Qualifier = qualifier
					}
				},
				BeginTimestamp = beginTimestamp.ToNullableInt64(),
				EndTimestamp = endTimestamp.ToNullableInt64(),
				MaxResults = maxResults.ToNullableInt32()
			};
		}
	}
}