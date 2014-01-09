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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

using HBase.Stargate.Client.Models;

namespace HBase.Stargate.Client.TypeConversion
{
	/// <summary>
	///    Defines an XML implementation of <see cref="IMimeConverter" />.
	/// </summary>
	public class XmlMimeConverter : IMimeConverter
	{
		private const string _cellSetName = "CellSet";
		private const string _rowName = "Row";
		private const string _keyName = "key";
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
		private readonly ISimpleValueConverter _valueConverter;
		private readonly ICodec _codec;

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlMimeConverter" /> class.
		/// </summary>
		/// <param name="valueConverter">The value converter.</param>
		/// <param name="codec">The codec.</param>
		public XmlMimeConverter(ISimpleValueConverter valueConverter, ICodec codec)
		{
			_valueConverter = valueConverter;
			_codec = codec;
		}

		/// <summary>
		///    Gets the current MIME type.
		/// </summary>
		/// <value>
		///    The MIME type.
		/// </value>
		public virtual string MimeType
		{
			get { return HBaseMimeTypes.Xml; }
		}

		/// <summary>
		///    Converts the specified cells to text according to the current MIME type.
		/// </summary>
		/// <param name="cells">The cells.</param>
		public virtual string ConvertCells(IEnumerable<Cell> cells)
		{
			IDictionary<string, Cell[]> rows = cells
				.GroupBy(cell => cell.Identifier != null ? cell.Identifier.Row : string.Empty)
				.ToDictionary(group => group.Key, group => group.ToArray());

			XElement xml = XmlForCellSet(rows.Keys.Select(row => XmlForRow(row, rows[row].Select(XmlForCell))));

			return xml.ToString();
		}

		/// <summary>
		///    Converts the specified cell to text according to the current MIME type.
		/// </summary>
		/// <param name="cell"></param>
		public virtual string ConvertCell(Cell cell)
		{
			string row = cell.Identifier != null ? cell.Identifier.Row : null;
			XElement xml = XmlForCellSet(new[]
			{
				XmlForRow(row, new[]
				{
					XmlForCell(cell)
				})
			});

			return xml.ToString();
		}

		/// <summary>
		///    Converts the specified data to a set of cells according to the current MIME type.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="tableName">The HBase table name.</param>
		public virtual IEnumerable<Cell> ConvertCells(string data, string tableName)
		{
			return string.IsNullOrEmpty(data)
				? Enumerable.Empty<Cell>()
				: XElement.Parse(data).Elements(_rowName)
					.SelectMany(row => row.Elements(_cellName))
					.Select(cell => CellForXml(cell, tableName));
		}

		/// <summary>
		///    Converts the specified data to a table schema according to the current MIME type.
		/// </summary>
		/// <param name="data">The data.</param>
		public virtual TableSchema ConvertSchema(string data)
		{
			return string.IsNullOrEmpty(data) ? null : SchemaForXml(XElement.Parse(data));
		}

		/// <summary>
		///    Converts the specified table schema to text according to the current MIME type.
		/// </summary>
		/// <param name="schema">The schema.</param>
		public virtual string ConvertSchema(TableSchema schema)
		{
			var xml = new XElement(_tableSchemaName,
				new XAttribute(_nameName, schema.Name),
				schema.Columns.Select(XmlForColumnSchema));

			AddConditionalAttribute(xml, _isMetaName, schema.IsMeta);
			AddConditionalAttribute(xml, _isRootName, schema.IsRoot);

			return xml.ToString();
		}

		private TableSchema SchemaForXml(XElement xml)
		{
			var schema = new TableSchema
			{
				Name = xml.Attribute(_nameName).Value,
				IsMeta = ParseAttributeValue(xml.Attribute(_isMetaName), bool.Parse),
				IsRoot = ParseAttributeValue(xml.Attribute(_isRootName), bool.Parse),
				Columns = new List<ColumnSchema>(xml.Elements(_columnSchemaName).Select(ColumnSchemaForXml))
			};

			return schema;
		}

		private XElement XmlForColumnSchema(ColumnSchema schema)
		{
			var xml = new XElement(_columnSchemaName, new XAttribute(_nameName, schema.Name));
			AddConditionalAttribute(xml, _blockSizeName, schema.BlockSize);
			AddConditionalAttribute(xml, _bloomFilterName, schema.BloomFilter, _valueConverter.ConvertBloomFilter);
			AddConditionalAttribute(xml, _minVersionsName, schema.MinVersions);
			AddConditionalAttribute(xml, _keepDeletedCellsName, schema.KeepDeletedCells);
			AddConditionalAttribute(xml, _encodeOnDiskName, schema.EncodeOnDisk);
			AddConditionalAttribute(xml, _blockCacheName, schema.BlockCache);
			AddConditionalAttribute(xml, _compressionName, schema.Compression, _valueConverter.ConvertCompressionType);
			AddConditionalAttribute(xml, _versionsName, schema.Versions);
			AddConditionalAttribute(xml, _replicationScopeName, schema.ReplicationScope);
			AddConditionalAttribute(xml, _ttlName, schema.TimeToLive);
			AddConditionalAttribute(xml, _dataBlockEncodingName, schema.DataBlockEncoding, _valueConverter.ConvertDataBlockEncoding);
			AddConditionalAttribute(xml, _inMemoryName, schema.InMemory);
			return xml;
		}

		private ColumnSchema ColumnSchemaForXml(XElement xml)
		{
			return new ColumnSchema
			{
				Name = xml.Attribute(_nameName).Value,
				BlockSize = ParseAttributeValue(xml.Attribute(_blockSizeName), int.Parse),
				BloomFilter = ParseAttributeValue(xml.Attribute(_bloomFilterName), _valueConverter.ConvertBloomFilter),
				MinVersions = ParseAttributeValue(xml.Attribute(_minVersionsName), int.Parse),
				KeepDeletedCells = ParseAttributeValue(xml.Attribute(_keepDeletedCellsName), bool.Parse),
				EncodeOnDisk = ParseAttributeValue(xml.Attribute(_encodeOnDiskName), bool.Parse),
				BlockCache = ParseAttributeValue(xml.Attribute(_blockCacheName), bool.Parse),
				Compression = ParseAttributeValue(xml.Attribute(_compressionName), _valueConverter.ConvertCompressionType),
				Versions = ParseAttributeValue(xml.Attribute(_versionsName), int.Parse),
				ReplicationScope = ParseAttributeValue(xml.Attribute(_replicationScopeName), int.Parse),
				TimeToLive = ParseAttributeValue(xml.Attribute(_ttlName), int.Parse),
				DataBlockEncoding = ParseAttributeValue(xml.Attribute(_dataBlockEncodingName), _valueConverter.ConvertDataBlockEncoding),
				InMemory = ParseAttributeValue(xml.Attribute(_inMemoryName), bool.Parse)
			};
		}

		private static TValue? ParseAttributeValue<TValue>(XAttribute attribute, Func<string, TValue> converter) where TValue : struct
		{
			return attribute == null ? (TValue?) null : converter(attribute.Value);
		}

		private static void AddConditionalAttribute<TValue>(XContainer xml, string name, TValue value, Func<TValue, string> valueExtractor = null)
		{
			if(ReferenceEquals(null, value))
			{
				return;
			}

			valueExtractor = valueExtractor ?? (current => ((object)current) != null ? current.ToString() : null);
			xml.Add(new XAttribute(name, valueExtractor(value)));
		}

		private Cell CellForXml(XElement cell, string tableName)
		{
			XElement parent = cell.Parent;
			if (parent == null)
			{
				return null;
			}

			XAttribute keyAttribute = parent.Attribute(_keyName);
			if (keyAttribute == null)
			{
				return null;
			}

			string row = _codec.Decode(keyAttribute.Value);
			ParsedColumn parsedColumn = ParseColumn(cell);

			XAttribute timestampAttribute = cell.Attribute(_timestampName);
			long? timestamp = timestampAttribute != null ? timestampAttribute.Value.ToNullableInt64() : null;
			string value = _codec.Decode(cell.Value);

			return new Cell(new Identifier
			{
				Table = tableName,
				Row = row,
				CellDescriptor = new HBaseCellDescriptor
				{
					Column = parsedColumn.Column,
					Qualifier = parsedColumn.Qualifier
				},
				Timestamp = timestamp
			}, value);
		}

		private ParsedColumn ParseColumn(XElement cell)
		{
			XAttribute columnAttribute = cell.Attribute(_columnName);
			if (columnAttribute == null)
			{
				return new ParsedColumn();
			}

			string columnValue = _codec.Decode(columnAttribute.Value);
			Match match = _columnParser.Match(columnValue);
			if (!match.Success)
			{
				return new ParsedColumn();
			}

			string column = match.Groups[_columnName].Value;
			string qualifier = match.Groups[_qualifierName].Value;

			return new ParsedColumn(column, qualifier);
		}

		private static XElement XmlForCellSet(IEnumerable<XElement> rows)
		{
			return new XElement(_cellSetName, rows);
		}

		private  XElement XmlForRow(string row, IEnumerable<XElement> cells)
		{
			var xml = new XElement(_rowName, cells);
			if (!string.IsNullOrEmpty(row))
			{
				xml.Add(new XAttribute(_keyName, _codec.Encode(row)));
			}
			return xml;
		}

		private XElement XmlForCell(Cell cell)
		{
			Identifier identifier = cell.Identifier;
			HBaseCellDescriptor cellDescriptor = identifier != null ? identifier.CellDescriptor : null;
			string column = cellDescriptor != null ? cellDescriptor.Column : null;
			string qualifier = cellDescriptor != null ? cellDescriptor.Qualifier : null;
			long? timestamp = identifier != null ? identifier.Timestamp : null;

			var cellXml = new XElement(_cellName, new XText(_codec.Encode(cell.Value)));

			if (!string.IsNullOrEmpty(column))
			{
				cellXml.Add(new XAttribute(_columnName, _codec.Encode(string.Format(_columnFormat, column, qualifier))));
			}
			if (timestamp.HasValue)
			{
				cellXml.Add(new XAttribute(_timestampName, timestamp.Value));
			}

			return cellXml;
		}

		private struct ParsedColumn
		{
			public ParsedColumn(string column, string qualifier) : this()
			{
				Column = column;
				Qualifier = qualifier;
			}

			public string Column { get; private set; }
			public string Qualifier { get; private set; }
		}
	}
}