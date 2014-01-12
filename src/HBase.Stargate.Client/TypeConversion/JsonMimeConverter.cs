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
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

using HBase.Stargate.Client.Models;

namespace HBase.Stargate.Client.TypeConversion
{
	/// <summary>
	///     Defines a JSON implementation of <see cref="IMimeConverter" />.
	/// </summary>
	public class JsonMimeConverter : IMimeConverter
	{
		private const string _cellSetName = "CellSet";
		private const string _rowName = "Row";
		private const string _keyName = "key";
		private const string _valueName = "$";
		private const string _columnFormat = "{0}:{1}";
		private const string _columnName = "column";
		private const string _qualifierName = "qualifier";
		private const string _timestampName = "timestamp";
		private const string _cellName = "Cell";
		private const string _tableSchemaName = "TableSchema";
		private const string _nameName = "name";
		private const string _isMetaName = "IS_META";
		private const string _isRootName = "IS_ROOT";
		private const string _columnSchemaName = "ColumnSchema";
		private const string _blockSizeName = "BLOCKSIZE";
		private const string _bloomFilterName = "BLOOMFILTER";
		private const string _blockCacheName = "BLOCKCACHE";
		private const string _compressionName = "COMPRESSION";
		private const string _versionsName = "VERSIONS";
		private const string _ttlName = "TTL";
		private const string _inMemoryName = "IN_MEMORY";
		private const string _columnParserFormat = "(?<{0}>[^:]+):(?<{1}>.+)?";
		private const string _keepDeletedCellsName = "KEEP_DELETED_CELLS";
		private const string _minVersionsName = "MIN_VERSIONS";
		private const string _dataBlockEncodingName = "DATA_BLOCK_ENCODING";
		private const string _replicationScopeName = "REPLICATION_SCOPE";
		private const string _encodeOnDiskName = "ENCODE_ON_DISK";
		private static readonly Regex _columnParser = new Regex(string.Format(_columnParserFormat, _columnName, _qualifierName));
		private readonly ICodec _codec;
		private readonly JavaScriptSerializer _serializer;
		private readonly ISimpleValueConverter _valueConverter;

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
		///     Gets the current MIME type.
		/// </summary>
		/// <value>
		///     The MIME type.
		/// </value>
		public string MimeType
		{
			get { return HBaseMimeTypes.Json; }
		}

		/// <summary>
		///     Converts the specified cells to text according to the current MIME type.
		/// </summary>
		/// <param name="cells"></param>
		/// <returns></returns>
		public string ConvertCells(IEnumerable<Cell> cells)
		{
			return _serializer.Serialize(ObjectForCells(cells));
		}

		/// <summary>
		///     Converts the specified cell to text according to the current MIME type.
		/// </summary>
		/// <param name="cell"></param>
		/// <returns></returns>
		public string ConvertCell(Cell cell)
		{
			return ConvertCells(new List<Cell> {cell});
		}

		/// <summary>
		///     Converts the specified data to a set of cells according to the current MIME type.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="tableName">The HBase table name.</param>
		public IEnumerable<Cell> ConvertCells(string data, string tableName)
		{
			return CellsFromObject(tableName, _serializer.Deserialize<Dictionary<string, object>>(data));
		}

		/// <summary>
		///     Converts the specified data to a table schema according to the current MIME type.
		/// </summary>
		/// <param name="data">The data.</param>
		public TableSchema ConvertSchema(string data)
		{
			return TableSchemaFromObject(_serializer.Deserialize<Dictionary<string, object>>(data));
		}

		/// <summary>
		///     Converts the specified table schema to text according to the current MIME type.
		/// </summary>
		/// <param name="schema">The schema.</param>
		public string ConvertSchema(TableSchema schema)
		{
			return _serializer.Serialize(ObjectForTableSchema(schema));
		}

		#region Data Serialization Methods

		private object ObjectForCells(IEnumerable<Cell> cells)
		{
			return new Dictionary<string, object>
				{
					{
						_rowName,
						cells.GroupBy(cell => cell.Identifier.Row)
							.Aggregate(new List<object>(), (rows, row) =>
								{
									rows.Add(ObjectForRow(row));
									return rows;
								})
					}
				};
		}

		private object ObjectForRow(IGrouping<string, Cell> row)
		{
			return new Dictionary<string, object>
				{
					{_keyName, _codec.Encode(row.Key)},
					{_cellName, row.Select(ObjectForCell)}
				};
		}

		private object ObjectForCell(Cell cell)
		{
			Identifier identifier = cell.Identifier;
			HBaseCellDescriptor descriptor = identifier != null ? identifier.CellDescriptor : null;

			string column = descriptor != null ? descriptor.Column : null;
			string qualifier = descriptor != null ? descriptor.Qualifier : null;
			long? timestamp = identifier != null ? identifier.Timestamp : null;

			var obj = new Dictionary<string, object>
				{
					{_valueName, _codec.Encode(cell.Value)}
				};

			if (!string.IsNullOrEmpty(column))
			{
				obj.Add(_columnName, _codec.Encode(string.Format(_columnFormat, column, qualifier)));
			}

			if (timestamp.HasValue)
			{
				obj.Add(_timestampName, timestamp.Value);
			}

			return obj;
		}

		#endregion

		#region Data Deserialization Methods

		private IEnumerable<Cell> CellsFromObject(string table, IReadOnlyDictionary<string, object> obj)
		{
			var rows = new List<object>(((ArrayList) obj[_rowName]).ToArray());

			return new CellSet(
				rows.SelectMany(row => CellsFromRow(table, (Dictionary<string, object>) row))
				) {Table = table};
		}

		private IEnumerable<Cell> CellsFromRow(string table, IReadOnlyDictionary<string, object> obj)
		{
			var key = (string) obj[_keyName];
			var row = new List<object>(((ArrayList) obj[_cellName]).ToArray());

			return row.Select(cell => CellFromObject(table, key, (Dictionary<string, object>) cell));
		}

		private Cell CellFromObject(string table, string row, IReadOnlyDictionary<string, object> obj)
		{
			row = _codec.Decode(row);
			ParsedColumn column = ParseColumn(obj);

			// TODO: figure out a better way to do this
			long? timestamp = obj.ContainsKey(_timestampName) ? (obj[_timestampName]).ToString().ToNullableInt64() : null;
			string value = _codec.Decode((string) obj[_valueName]);

			return new Cell(
				new Identifier
					{
						Row = row,
						Table = table,
						Timestamp = timestamp,
						CellDescriptor = new HBaseCellDescriptor
							{
								Column = column.Column,
								Qualifier = column.Qualifier
							}
					},
				value
				);
		}

		private ParsedColumn ParseColumn(IReadOnlyDictionary<string, object> obj)
		{
			if (!obj.ContainsKey(_columnName))
			{
				return new ParsedColumn();
			}

			string columnValue = _codec.Decode((string) obj[_columnName]);
			Match match = _columnParser.Match(columnValue);
			if (!match.Success)
			{
				return new ParsedColumn();
			}

			string column = match.Groups[_columnName].Value;
			string qualifier = match.Groups[_qualifierName].Value;

			return new ParsedColumn(column, qualifier);
		}

		#endregion

		#region Schema Serialization Methods

		private object ObjectForTableSchema(TableSchema schema)
		{
			var obj = new Dictionary<string, object>
				{
					{_nameName, schema.Name},
					{_columnSchemaName, schema.Columns.Select(ObjectForColumnSchema)}
				};

			AddConditionalAttribute(obj, _isRootName, schema.IsRoot);
			AddConditionalAttribute(obj, _isMetaName, schema.IsMeta);

			return obj;
		}

		private object ObjectForColumnSchema(ColumnSchema schema)
		{
			var obj = new Dictionary<string, object>
				{
					{_nameName, schema.Name}
				};

			AddConditionalAttribute(obj, _blockSizeName, schema.BlockSize);
			AddConditionalAttribute(obj, _bloomFilterName, schema.BloomFilter, _valueConverter.ConvertBloomFilter);
			AddConditionalAttribute(obj, _minVersionsName, schema.MinVersions);
			AddConditionalAttribute(obj, _keepDeletedCellsName, schema.KeepDeletedCells);
			AddConditionalAttribute(obj, _encodeOnDiskName, schema.EncodeOnDisk);
			AddConditionalAttribute(obj, _blockCacheName, schema.BlockCache);
			AddConditionalAttribute(obj, _compressionName, schema.Compression, _valueConverter.ConvertCompressionType);
			AddConditionalAttribute(obj, _versionsName, schema.Versions);
			AddConditionalAttribute(obj, _replicationScopeName, schema.ReplicationScope);
			AddConditionalAttribute(obj, _ttlName, schema.TimeToLive);
			AddConditionalAttribute(obj, _dataBlockEncodingName, schema.DataBlockEncoding, _valueConverter.ConvertDataBlockEncoding);
			AddConditionalAttribute(obj, _inMemoryName, schema.InMemory);

			return obj;
		}

		#endregion

		#region Schema Deserialization Methods

		private TableSchema TableSchemaFromObject(IReadOnlyDictionary<string, object> obj)
		{
			return new TableSchema
				{
					Name = (string) obj[_nameName],
					IsRoot = ParseAttributeValue(obj, _isRootName, bool.Parse),
					IsMeta = ParseAttributeValue(obj, _isMetaName, bool.Parse),
					Columns =
						new List<object>(((ArrayList) obj[_columnSchemaName]).ToArray()).Select(col => ColumnSchemaFromObject((Dictionary<string, object>) col))
							.ToList()
				};
		}

		private ColumnSchema ColumnSchemaFromObject(IReadOnlyDictionary<string, object> obj)
		{
			return new ColumnSchema
				{
					Name = (string) obj[_nameName],
					BlockSize = ParseAttributeValue(obj, _blockSizeName, int.Parse),
					BloomFilter = ParseAttributeValue(obj, _bloomFilterName, _valueConverter.ConvertBloomFilter),
					MinVersions = ParseAttributeValue(obj, _minVersionsName, int.Parse),
					KeepDeletedCells = ParseAttributeValue(obj, _keepDeletedCellsName, bool.Parse),
					EncodeOnDisk = ParseAttributeValue(obj, _encodeOnDiskName, bool.Parse),
					BlockCache = ParseAttributeValue(obj, _blockCacheName, bool.Parse),
					Compression = ParseAttributeValue(obj, _compressionName, _valueConverter.ConvertCompressionType),
					Versions = ParseAttributeValue(obj, _versionsName, int.Parse),
					ReplicationScope = ParseAttributeValue(obj, _replicationScopeName, int.Parse),
					TimeToLive = ParseAttributeValue(obj, _ttlName, int.Parse),
					DataBlockEncoding = ParseAttributeValue(obj, _dataBlockEncodingName, _valueConverter.ConvertDataBlockEncoding),
					InMemory = ParseAttributeValue(obj, _inMemoryName, bool.Parse)
				};
		}

		#endregion

		#region Schema Helpers

		private static void AddConditionalAttribute<TValue>(IDictionary<string, object> obj, string name, TValue value,
		                                                    Func<TValue, string> valueExtractor = null)
		{
			if (ReferenceEquals(null, value))
			{
				return;
			}

			valueExtractor = valueExtractor ?? (current => ((object) current) != null ? current.ToString() : null);
			obj.Add(name, valueExtractor(value));
		}

		private static TValue? ParseAttributeValue<TValue>(IReadOnlyDictionary<string, object> obj, string key, Func<string, TValue> converter)
			where TValue : struct
		{
			return obj.ContainsKey(key) ? converter((string) obj[key]) : (TValue?) null;
		}

		#endregion

		#region Inner Types

		private struct ParsedColumn
		{
			public ParsedColumn(string column, string qualifier)
				: this()
			{
				Column = column;
				Qualifier = qualifier;
			}

			public string Column { get; private set; }
			public string Qualifier { get; private set; }
		}

		#endregion
	}
}