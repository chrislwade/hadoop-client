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
using System.Collections;
using System.Collections.Generic;

using HBase.Stargate.Client.Models;

namespace HBase.Stargate.Client.Api
{
	/// <summary>
	///    Provides a standard implementation of the <see cref="IScanner" /> interface.
	/// </summary>
	public class Scanner : IScanner
	{
		private readonly IList<CellSet> _cachedResults;
		private readonly IStargate _stargate;
		private int _resultIndex;
		private bool _isDisposed;

		/// <summary>
		///    Initializes a new instance of the <see cref="Scanner" /> class.
		/// </summary>
		/// <param name="tableName">The HBase table name.</param>
		/// <param name="resource">The resource.</param>
		/// <param name="stargate">The current Stargate.</param>
		public Scanner(string tableName, string resource, IStargate stargate)
		{
			_cachedResults = new List<CellSet>();
			_stargate = stargate;
			Table = tableName;
			Resource = resource;
		}

		/// <summary>
		///    Gets the table name.
		/// </summary>
		/// <value>
		///    The table name.
		/// </value>
		public string Table { get; private set; }

		/// <summary>
		///    Gets the resource.
		/// </summary>
		/// <value>
		///    The resource.
		/// </value>
		public string Resource { get; private set; }

		/// <summary>
		///    Deletes this scanner and disables it to prevent future use.
		/// </summary>
		public void Dispose()
		{
			if (_isDisposed) return;

			_stargate.DeleteScanner(this);
			Resource = null;
			_isDisposed = true;
		}

		/// <summary>
		///    Returns an enumerator that iterates through the collection.
		/// </summary>
		public IEnumerator<CellSet> GetEnumerator()
		{
			return this;
		}

		/// <summary>
		///    Returns an enumerator that iterates through a collection.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		///    Advances the enumerator to the next element of the collection.
		/// </summary>
		public bool MoveNext()
		{
			AssertNotDisposed();

			if (_resultIndex >= _cachedResults.Count)
			{
				if (Current != null) _cachedResults.Add(Current);
				Current = _stargate.GetScannerResult(this);
			}
			else if (_resultIndex >= 0)
			{
				Current = _cachedResults[_resultIndex];
			}

			_resultIndex++;
			return Current != null;
		}

		/// <summary>
		///    Sets the enumerator to its initial position, which is before the first element in the collection.
		/// </summary>
		public void Reset()
		{
			AssertNotDisposed();

			_resultIndex = 0;
		}

		/// <summary>
		///    Gets the element in the collection at the current position of the enumerator.
		/// </summary>
		public CellSet Current { get; private set; }

		/// <summary>
		///    Gets the element in the collection at the current position of the enumerator.
		/// </summary>
		object IEnumerator.Current
		{
			get { return Current; }
		}

		/// <summary>
		///    Asserts that this instance is not disposed.
		/// </summary>
		private void AssertNotDisposed()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}
	}
}