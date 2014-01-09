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

using HBase.Stargate.Client.Models;

using TechTalk.SpecFlow;

using _specs.Models;

namespace _specs.Steps
{
	[Binding]
	public class Reading
	{
		private readonly HBaseContext _context;

		public Reading(HBaseContext context)
		{
			_context = context;
		}

		[When(@"I read a single value using my identifier")]
		public void ReadSingleValue()
		{
			_context.CellValue = _context.Stargate.ReadValue(_context.Identifier);
		}

		[When(@"I read a row using my query")]
		public void ReadRow()
		{
			_context.CellSet = _context.Stargate.FindCells(_context.Query);
		}

		[When(@"I read all cells from the ""(.*?)"" table")]
		public void ReadTable(string tableName)
		{
			_context.CellSet = _context.Stargate.FindCells(new CellQuery { Table = tableName });
		}
		
		[When(@"I read a result from the scanner")]
		public void ReadScanner()
		{
			bool success = _context.Scanner.MoveNext();
			success.Should().Be(true);

			_context.CellSet = _context.Scanner.Current;
		}

		[When(@"I read the names of all tables")]
		public void ReadTableNames()
		{
			_context.TableNames = _context.Stargate.GetTableNames();
		}
	}
}