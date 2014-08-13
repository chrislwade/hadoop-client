﻿#region FreeBSD

// Copyright (c) 2014, The Tribe
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

using System.Linq;

using FluentAssertions;

using HBase.Stargate.Client.Models;

using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

using _specs.Models;

namespace _specs.Steps
{
	[Binding]
	public class SimpleAssertions
	{
		private readonly HBaseContext _cells;

		public SimpleAssertions(HBaseContext cells)
		{
			_cells = cells;
		}

		[Then(@"my cell should have a (.+), (.+), (.*), (.*), and (.*)")]
		public void CheckCellContents(string row, string column, string qualifier, string timestamp, string value)
		{
			CellMatchesTestValue(_cells.Cell, new TestCell
			{
				Row = row,
				Column = column,
				Qualifier = qualifier,
				Timestamp = timestamp.ToNullableInt64(),
				Value = value
			}).Should().BeTrue();
		}

		[Then(@"my set should contain (\d+) cells?")]
		public void CheckCellSetCount(int count)
		{
			_cells.CellSet.Should().HaveCount(count);
		}

		[Then(@"(?:one of )?the cells in my set should have the following properties:")]
		public void CheckAnyCellInSet(Table values)
		{
			values.CompareToSet(_cells.CellSet.Select(cell => (TestCell) cell));
		}

        [Then(@"the result should be ""(.+)""")]
        public void CheckResulValue(TestString value)
        {
            _cells.CellValue.Should().Be(value);
        }

		private static bool CellMatchesTestValue(Cell cell, TestCell testCell)
		{
			return cell.Identifier.Row == testCell.Row
				&& cell.Identifier.CellDescriptor != null
				&& cell.Identifier.CellDescriptor.Column == testCell.Column
				&& cell.Identifier.CellDescriptor.Qualifier == testCell.Qualifier
				&& cell.Identifier.Timestamp == testCell.Timestamp
				&& cell.Value == testCell.Value;
		}
	}
}