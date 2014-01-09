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

using HBase.Stargate.Client;
using HBase.Stargate.Client.Models;
using HBase.Stargate.Client.TypeConversion;

namespace _specs.Models
{
	public class ContentConverter : IMimeConverter
	{
		private IMimeConverter _converter;

		public string MimeType { get; private set; }

		public string ConvertCells(IEnumerable<Cell> cells)
		{
			if (_converter != null)
			{
				return _converter.ConvertCells(cells);
			}

			throw new NotImplementedException();
		}

		public string ConvertCell(Cell cell)
		{
			if (_converter != null)
			{
				return _converter.ConvertCell(cell);
			}

			throw new NotImplementedException();
		}

		public IEnumerable<Cell> ConvertCells(string data, string tableName)
		{
			if (_converter != null)
			{
				return _converter.ConvertCells(data, tableName);
			}

			throw new NotImplementedException();
		}

		public TableSchema ConvertSchema(string data)
		{
			if (_converter != null)
			{
				return _converter.ConvertSchema(data);
			}

			throw new NotImplementedException();
		}

		public string ConvertSchema(TableSchema schema)
		{
			if (_converter != null)
			{
				return _converter.ConvertSchema(schema);
			}

			throw new NotImplementedException();
		}

		public void SetConversionToXml()
		{
			MimeType = HBaseMimeTypes.Xml;
			_converter = new XmlMimeConverter(new SimpleValueConverter(), new Base64Codec());
		}

		public void SetConversionToJson()
		{
			MimeType = HBaseMimeTypes.Json;
			//TODO: _converter = new JsonMimeConverter();
		}

		public void SetConversionToProtobuf()
		{
			MimeType = HBaseMimeTypes.Protobuf;
			//TODO: _converter = new ProtobufMimeConverter();
		}

		public void SetConversionToBinary()
		{
			MimeType = HBaseMimeTypes.Stream;
			//TODO: _converter = new BinaryMimeConverter();
		}
	}
}