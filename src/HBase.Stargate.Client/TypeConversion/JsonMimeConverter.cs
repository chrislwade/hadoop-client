#region FreeBSD

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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

using HBase.Stargate.Client.Models;

namespace HBase.Stargate.Client.TypeConversion
{
	/// <summary>
	///     Defines a JSON implementation of <see cref="IMimeConverter" />.
	/// </summary>
	public class JsonMimeConverter : IMimeConverter
	{
		private readonly ICodec _codec;
		private readonly ISimpleValueConverter _valueConverter;
		private readonly JavaScriptSerializer _serializer;

		/// <summary>
		///     Intializes a new instance of the <see cref="JsonMimeConverter" /> class.
		/// </summary>
		/// <param name="valueConverter">The value conveter.</param>
		/// <param name="codec">The codec.</param>
		public JsonMimeConverter(ISimpleValueConverter valueConverter, ICodec codec)
		{
			_valueConverter = valueConverter;
			_codec = codec;
			_serializer = new JavaScriptSerializer();
		}

		/// <summary>
		///    Gets the current MIME type.
		/// </summary>
		/// <value>
		///    The MIME type.
		/// </value>
		public string MimeType
		{
			get { return HBaseMimeTypes.Json; }
		}

		/// <summary>
		///    Converts the specified cells to text according to the current MIME type.
		/// </summary>
		/// <param name="cells"></param>
		/// <returns></returns>
		public string ConvertCells(IEnumerable<Cell> cells)
		{
			return _serializer.Serialize(ObjectForCells(cells));
		}

		/// <summary>
		///    Converts the specified cell to text according to the current MIME type.
		/// </summary>
		/// <param name="cell"></param>
		/// <returns></returns>
		public string ConvertCell(Cell cell)
		{
			return _serializer.Serialize(ObjectForCell(cell));
		}

		/// <summary>
		///    Converts the specified data to a set of cells according to the current MIME type.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="tableName">The HBase table name.</param>
		public IEnumerable<Cell> ConvertCells(string data, string tableName)
		{
			return CellsFromObject(tableName, _serializer.Deserialize<Dictionary<string, object>>(data));
		}

		/// <summary>
		///    Converts the specified data to a table schema according to the current MIME type.
		/// </summary>
		/// <param name="data">The data.</param>
		public TableSchema ConvertSchema(string data)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///    Converts the specified table schema to text according to the current MIME type.
		/// </summary>
		/// <param name="schema">The schema.</param>
		public string ConvertSchema(TableSchema schema)
		{
			throw new NotImplementedException();
		}

		#region Private Serialization Methods
		private object ObjectForCell(Cell cell)
		{
			return new Dictionary<string, object> {
			{ "column", string.Format("{0}:{1}", cell.Identifier.CellDescriptor.Column, cell.Identifier.CellDescriptor.Qualifier) },
			{ "timestamp", cell.Identifier.Timestamp },
			{ "$", cell.Value }
		};
		}
		private object ObjectForRow(IGrouping<string, Cell> row)
		{
			return new Dictionary<string, object> {
			{ "key", row.Key },
			{ "Cell", row.Select(ObjectForCell) }
		};
		}
		private object ObjectForCells(IEnumerable<Cell> cells)
		{
			return new Dictionary<string, object> {
			{
				"Row",
				cells.GroupBy(cell => cell.Identifier.Row)
					.Aggregate(new List<object>(), (rows, row) => {
						rows.Add(ObjectForRow(row));
						return rows;
					})
			}
		};
		}
		#endregion
		#region Private Deserialization Methods
		private Cell CellFromObject(string table, string row, Dictionary<string, object> obj)
		{
			return new Cell(
				new Identifier
				{
					Row = row,
					Table = table,
					Timestamp = (long?)obj["timestamp"],
					CellDescriptor = new HBaseCellDescriptor
					{
						Column = ((string)obj["column"]).Split(':')[0],
						Qualifier = ((string)obj["column"]).Split(':')[1]
					}
				},
				(string)obj["$"]
			);
		}
		private IEnumerable<Cell> CellsFromRow(string table, Dictionary<string, object> obj)
		{
			var key = (string)obj["key"];
			var row = new List<object>(((ArrayList)obj["Cell"]).ToArray());

			return row.Select(cell => CellFromObject(table, key, (Dictionary<string, object>)cell));
		}
		private IEnumerable<Cell> CellsFromObject(string table, Dictionary<string, object> obj)
		{
			var rows = new List<object>(((ArrayList)obj["Row"]).ToArray());

			return new CellSet(
				rows.SelectMany(row => CellsFromRow(table, (Dictionary<string, object>)row))
			) { Table = table };
		}
		#endregion
	}
}