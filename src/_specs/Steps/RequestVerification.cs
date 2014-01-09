#region FreeBSD

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

using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using HBase.Stargate.Client.Api;
using HBase.Stargate.Client.Models;
using HBase.Stargate.Client.TypeConversion;

using Patterns.Testing.Moq;

using RestSharp;

using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

using _specs.Models;

namespace _specs.Steps
{
	[Binding]
	public class RequestVerification
	{
		private readonly IMimeConverterFactory _converterFactory;
		private readonly RestContext _rest;
		private readonly ErrorContext _errors;

		public RequestVerification(RestContext rest, ErrorContext errors, IMoqContainer container)
		{
			_rest = rest;
			_errors = errors;
			_converterFactory = container.Create<IMimeConverterFactory>();
		}

		[Then(@"a REST request should have been submitted with the following values:")]
		public void CheckRequest(Table values)
		{
			AssertRequestValuesMatch(_rest.Request, values.CreateInstance<ExpectedRequestProperties>());
		}

		[Then(@"a REST request for schema updates should have been submitted with the following values:")]
		public void CheckSchemaUpdateRequest(Table values)
		{
			CheckSchemaUpdateProperties(values.CreateInstance<ExpectedSchemaUpdateRequestProperties>());
		}

		[Then(@"if the operation succeeded, a REST request for schema updates should have been submitted with the correct (.*), (.*), (.*), and (.*)")]
		public void CheckSchemaUpdateRequest(Method method, string resource, string table, string column)
		{
			if (!_errors.OutcomeViewedAsSuccessful) return;

			CheckSchemaUpdateProperties(new ExpectedSchemaUpdateRequestProperties
			{
				Column = column,
				Method = method,
				Resource = resource,
				Table = table
			});
		}

		[Then(@"a REST request should have been submitted with the correct (.+) and (.+)")]
		public void CheckRequest(Method method, string resource)
		{
			AssertRequestValuesMatch(_rest.Request, new ExpectedRequestProperties {Method = method, Resource = resource});
		}

		[Then(@"the REST request should have contained (.*) cells?")]
		public void CheckRequestCellCount(int count)
		{
			IEnumerable<Cell> row = GetRowFromRequest();
			row.Should().HaveCount(count);
		}

		[Then(@"one of the cells in the REST request should have had the value ""(.*)""")]
		public void CheckAnyCellValue(string value)
		{
			IEnumerable<Cell> row = GetRowFromRequest();
			row.SingleOrDefault(cell => cell.Value == value).Should().NotBeNull();
		}

		[Then(@"(?:one of )?the cells in the REST request should have had the following values:")]
		public void CheckAnyCellValues(Table values)
		{
			values.CompareToSet(GetRowFromRequest().Select(SimpleAssertions.ConvertToTestCell));
		}

		private IEnumerable<Cell> GetRowFromRequest()
		{
			string content = _rest.Request.Parameters
				.Where(cell => cell.Type == ParameterType.RequestBody)
				.Select(cell => cell.Value.ToString())
				.FirstOrDefault();
			IEnumerable<Cell> row = CreateRequestConverter(_rest.Request, _converterFactory).ConvertCells(content, string.Empty).ToArray();
			row.Should().NotBeNull();
			return row;
		}

		private static IMimeConverter CreateRequestConverter(IRestRequest request, IMimeConverterFactory mimeConverterFactory)
		{
			string mimeType = request.Parameters
				.Where(parameter => parameter.Type == ParameterType.HttpHeader && IsAcceptOrContentType(parameter.Name))
				.Select(parameter => parameter.Value.ToString())
				.FirstOrDefault();

			mimeType.Should().NotBeEmpty();

			IMimeConverter converter = mimeConverterFactory.CreateConverter(mimeType);
			converter.Should().NotBeNull();

			return converter;
		}

		private static bool IsAcceptOrContentType(string name)
		{
			return name == RestConstants.ContentTypeHeader
				|| name == RestConstants.AcceptHeader;
		}

		private static void AssertRequestValuesMatch(IRestRequest request, ExpectedRequestProperties properties)
		{
			request.Method.Should().Be(properties.Method);
			request.Resource.Should().Be(properties.Resource);
		}

		private static void AssertSchemaValuesMatch(TableSchema requestedSchema, ExpectedSchemaUpdateRequestProperties properties)
		{
			requestedSchema.Name.Should().Be(properties.Table);
			requestedSchema.Columns.Should().HaveCount(1);
			requestedSchema.Columns[0].Name.Should().Be(properties.Column);
		}

		private TableSchema GetTableSchemaFromRequest()
		{
			string content = _rest.Request.Parameters
				.Where(cell => cell.Type == ParameterType.RequestBody)
				.Select(cell => cell.Value.ToString())
				.FirstOrDefault();
			TableSchema schema = CreateRequestConverter(_rest.Request, _converterFactory).ConvertSchema(content);
			schema.Should().NotBeNull();
			return schema;
		}

		private void CheckSchemaUpdateProperties(ExpectedSchemaUpdateRequestProperties properties)
		{
			AssertRequestValuesMatch(_rest.Request, properties);
			AssertSchemaValuesMatch(GetTableSchemaFromRequest(), properties);
		}
	}
}