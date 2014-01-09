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

using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using HBase.Stargate.Client;
using HBase.Stargate.Client.Models;

using TechTalk.SpecFlow;

using _specs.Models;

namespace _specs.Steps.Serialization
{
	[Binding]
	public class ContentConversion
	{
		private readonly HBaseContext _hBase;
		private readonly ContentConverter _converter;

		public ContentConversion(HBaseContext hBase, ContentConverter converter)
		{
			_hBase = hBase;
			_converter = converter;
		}

		[When(@"I convert my raw content to a set of cells")]
		public void ConvertRawContentToCellSet()
		{
			_hBase.CellSet = new CellSet(_converter.ConvertCells(_hBase.RawContent, string.Empty));
		}

		[When(@"I convert my raw content to a cell")]
		public void ConvertRawContentToCell()
		{
			IList<Cell> set = _converter.ConvertCells(_hBase.RawContent, string.Empty).ToList();
			set.Should().HaveCount(1);
			_hBase.Cell = set[0];
		}

		[When(@"I convert my set of cells to raw content")]
		public void ConvertCellSetToRawContent()
		{
			_hBase.RawContent = _converter.ConvertCells(_hBase.CellSet);
		}

		[When(@"I convert my cell to raw content")]
		public void ConvertCellToRawContent()
		{
			_hBase.RawContent = _converter.ConvertCell(_hBase.Cell);
		}
	}
}