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

using System.Web.Script.Serialization;

using FluentAssertions;

using TechTalk.SpecFlow;

using _specs.Models;

namespace _specs.Steps.Serialization
{
	[Binding]
	public class Json
	{
		private readonly HBaseContext _hBase;
		private readonly ResourceContext _resources;
		private readonly ContentConverter _converter;
		private readonly JavaScriptSerializer _serializer;

		public Json(HBaseContext hbase, ResourceContext resources, ContentConverter converter)
		{
			_hBase = hbase;
			_resources = resources;
			_converter = converter;
			// TODO: ask John about how to inject this?
			_serializer = new JavaScriptSerializer();
		}

		[Given(@"I have everything I need to test a content converter for JSON")]
		public void SetConversionToJson()
		{
			_converter.SetConversionToJson();
		}

		[Then(@"my raw JSON content should be equivalent to the resource called ""(.*)""")]
		public void CompareJsonToResource(string resourceName)
		{
			var left = _serializer.DeserializeObject(_hBase.RawContent);
			var right = _serializer.DeserializeObject(_resources.GetString(resourceName));

			left.Should().Be(right);
		}
	}
}