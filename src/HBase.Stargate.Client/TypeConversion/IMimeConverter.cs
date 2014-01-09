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

using System.Collections.Generic;

using HBase.Stargate.Client.Models;

namespace HBase.Stargate.Client.TypeConversion
{
	/// <summary>
	///    Provides HBase data conversion to a specific MIME type.
	/// </summary>
	public interface IMimeConverter
	{
		/// <summary>
		///    Gets the current MIME type.
		/// </summary>
		/// <value>
		///    The MIME type.
		/// </value>
		string MimeType { get; }

		/// <summary>
		///    Converts the specified cells to text according to the current MIME type.
		/// </summary>
		/// <param name="cells">The cells.</param>
		string ConvertCells(IEnumerable<Cell> cells);

		/// <summary>
		///    Converts the specified cell to text according to the current MIME type.
		/// </summary>
		/// <param name="cell"></param>
		string ConvertCell(Cell cell);

		/// <summary>
		///    Converts the specified data to a set of cells according to the current MIME type.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="tableName">The HBase table name.</param>
		IEnumerable<Cell> ConvertCells(string data, string tableName);

		/// <summary>
		///    Converts the specified data to a table schema according to the current MIME type.
		/// </summary>
		/// <param name="data">The data.</param>
		TableSchema ConvertSchema(string data);

		/// <summary>
		/// Converts the specified table schema to text according to the current MIME type.
		/// </summary>
		/// <param name="schema">The schema.</param>
		/// <returns></returns>
		string ConvertSchema(TableSchema schema);
	}
}