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

using System.Linq;

using HBase.Stargate.Client;
using HBase.Stargate.Client.Models;

using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

using _specs.Models;

namespace _specs.Steps
{
	[Binding]
	public class SimpleConstruction
	{
		private readonly HBaseContext _hBase;
		private readonly ResourceContext _resources;

		public SimpleConstruction(HBaseContext hBase, ResourceContext resources)
		{
			_hBase = hBase;
			_resources = resources;
		}

		[Given(@"I have created a set of cells")]
		public void CreateCellSet()
		{
			_hBase.CellSet = new CellSet();
		}

		[Given(@"I have created a set of cells for the ""(.+)"" table")]
		public void CreateCellSet(string table)
		{
			_hBase.CellSet = new CellSet {Table = table};
		}

		[Given(@"I have added (?:a cell|cells) to my set with the following properties:")]
		public void AddToCellSet(Table values)
		{
			_hBase.CellSet.AddRange(values.CreateSet<TestCell>().Select(ConvertToCell));
		}

		[Given(@"I have raw content equal to the resource called ""(.*)""")]
		public void SetRawContentToResource(string resourceName)
		{
			_hBase.RawContent = _resources.GetString(resourceName);
		}

		[Given(@"I have a cell with a (.+), (.+), (.*), (.*), and (.*)")]
		public void CreateCell(string row, string column, string qualifier, string timestamp, string value)
		{
			_hBase.Cell = new Cell(new Identifier
			{
				Row = row,
				CellDescriptor = new HBaseCellDescriptor
				{
					Column = column,
					Qualifier = qualifier
				},
				Timestamp = timestamp.ToNullableInt64()
			}, value);
		}

		public static Cell ConvertToCell(TestCell testCell)
		{
			return new Cell(new Identifier
			{
				Table = testCell.Table,
				Row = testCell.Row,
				CellDescriptor = new HBaseCellDescriptor
				{
					Column = testCell.Column,
					Qualifier = testCell.Qualifier
				},
				Timestamp = testCell.Timestamp
			}, testCell.Value);
		}
	}
}