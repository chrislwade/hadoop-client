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

using System;
using System.Collections.Generic;
using System.Linq;

namespace HBase.Stargate.Client.TypeConversion
{
	/// <summary>
	///    Defines an IoC-driven implementation of <see cref="IMimeConverterFactory" />.
	/// </summary>
	public class MimeConverterFactory : IMimeConverterFactory
	{
		private readonly IEnumerable<IMimeConverter> _converters;

		/// <summary>
		/// Initializes a new instance of the <see cref="MimeConverterFactory"/> class.
		/// </summary>
		/// <param name="converters">The converters.</param>
		public MimeConverterFactory(IEnumerable<IMimeConverter> converters)
		{
			_converters = converters;
		}

		/// <summary>
		/// Creates the converter appropriate for the specified MIME type.
		/// </summary>
		/// <param name="mimeType">The MIME type.</param>
		public IMimeConverter CreateConverter(string mimeType)
		{
			return _converters.FirstOrDefault(converter => StringComparer.OrdinalIgnoreCase.Equals(converter.MimeType, mimeType));
		}
	}
}