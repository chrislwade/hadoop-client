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

using FluentAssertions;

using HBase.Stargate.Client.Api;

using TechTalk.SpecFlow;

using _specs.Models;

namespace _specs.Steps
{
	[Binding]
	public class ScannerSteps
	{
		private readonly HBaseContext _hBase;

		public ScannerSteps(HBaseContext hBase)
		{
			_hBase = hBase;
		}

		[Then(@"my scanner should have a resource set to ""(.*)""")]
		public void CheckScannerResource(string resource)
		{
			_hBase.Scanner.Resource.Should().Be(resource);
		}

		[Given(@"I have a scanner for the ""(.*)"" table named ""(.*)""")]
		public void ObtainManualScanner(string tableName, string scannerId)
		{
			_hBase.Scanner = new Scanner(tableName, string.Format("{0}/scanner/{1}", tableName, scannerId), _hBase.Stargate);
		}

		[When(@"I delete the scanner")]
		public void DeleteScanner()
		{
			_hBase.Stargate.DeleteScanner(_hBase.Scanner);
		}

		[When(@"I create a scanner for the ""(.*)"" table")]
		public void CreateScanner(string tableName)
		{
			_hBase.Scanner = _hBase.Stargate.CreateScanner(new ScannerOptions { TableName = tableName });
		}
	}
}