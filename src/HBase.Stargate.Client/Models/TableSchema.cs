﻿#region FreeBSD

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

namespace HBase.Stargate.Client.Models
{
	/// <summary>
	///    Encapsulates the options available for new table creation.
	/// </summary>
	public class TableSchema
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TableSchema"/> class.
		/// </summary>
		/// <param name="columns">The columns.</param>
		public TableSchema(IEnumerable<ColumnSchema> columns)
		{
			Columns = new List<ColumnSchema>(columns);
		}

		/// <summary>
		///    Initializes a new instance of the <see cref="TableSchema" /> class.
		/// </summary>
		public TableSchema()
		{
			Columns = new List<ColumnSchema>();
		}

		/// <summary>
		///    Gets or sets the name.
		/// </summary>
		/// <value>
		///    The name.
		/// </value>
		public string Name { get; set; }

		/// <summary>
		///    Gets or sets a value indicating whether this instance describes a meta-table.
		/// </summary>
		/// <value>
		///    <c>true</c> if this instance describes a meta-table; otherwise, <c>false</c>.
		/// </value>
		public bool? IsMeta { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is a root table.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is a root table; otherwise, <c>false</c>.
		/// </value>
		public bool? IsRoot { get; set; }

		/// <summary>
		///    Gets or sets the columns.
		/// </summary>
		/// <value>
		///    The columns.
		/// </value>
		public List<ColumnSchema> Columns { get; private set; }
	}
}