﻿/* The following code was generated by JFlex 1.4.1 on 9/4/08 6:49 PM */
/* 
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


/*
    NOTE: if you change StandardTokenizerImpl.jflex and need to regenerate the tokenizer,
    the tokenizer, only use Java 1.4 !!!
    This grammar currently uses constructs (eg :digit:, :letter:) whose
    meaning can vary according to the JRE used to run jflex.  See
    https://issues.apache.org/jira/browse/LUCENE-1126 for details.
    For current backwards compatibility it is needed to support
    only Java 1.4 - this will change in Lucene 3.1.
*/

using System;
using Lucene.Net.Analysis.Tokenattributes;
using Token = Lucene.Net.Analysis.Token;

namespace Lucene.Net.Analysis.Standard
{
	
	
	/// <summary> This class is a scanner generated by 
	/// <a href="http://www.jflex.de/">JFlex</a> 1.4.1
	/// on 9/4/08 6:49 PM from the specification file
	/// <tt>/tango/mike/src/lucene.standarddigit/src/java/org/apache/lucene/analysis/standard/StandardTokenizerImpl.jflex</tt>
	/// </summary>
	class StandardTokenizerImpl
	{
		
		/// <summary>This character denotes the end of file </summary>
		public const int YYEOF = - 1;
		
		/// <summary>initial size of the lookahead buffer </summary>
		private const int ZZ_BUFFERSIZE = 16384;
		
		/// <summary>lexical states </summary>
		public const int YYINITIAL = 0;
		
		/// <summary> Translates characters to character classes</summary>
		private const System.String ZZ_CMAP_PACKED = "\x0009\x0000\x0001\x0000\x0001\x000D\x0001\x0000\x0001\x0000\x0001\x000C\x0012\x0000\x0001\x0000\x0005\x0000\x0001\x0005" + "\x0001\x0003\x0004\x0000\x0001\x0009\x0001\x0007\x0001\x0004\x0001\x0009\x000A\x0002\x0006\x0000\x0001\x0006\x001A\x000A" + "\x0004\x0000\x0001\x0008\x0001\x0000\x001A\x000A\x002F\x0000\x0001\x000A\x000A\x0000\x0001\x000A\x0004\x0000\x0001\x000A" + "\x0005\x0000\x0017\x000A\x0001\x0000\x001F\x000A\x0001\x0000\u0128\x000A\x0002\x0000\x0012\x000A\x001C\x0000\x005E\x000A" + "\x0002\x0000\x0009\x000A\x0002\x0000\x0007\x000A\x000E\x0000\x0002\x000A\x000E\x0000\x0005\x000A\x0009\x0000\x0001\x000A" + "\x008B\x0000\x0001\x000A\x000B\x0000\x0001\x000A\x0001\x0000\x0003\x000A\x0001\x0000\x0001\x000A\x0001\x0000\x0014\x000A" + "\x0001\x0000\x002C\x000A\x0001\x0000\x0008\x000A\x0002\x0000\x001A\x000A\x000C\x0000\x0082\x000A\x000A\x0000\x0039\x000A" + "\x0002\x0000\x0002\x000A\x0002\x0000\x0002\x000A\x0003\x0000\x0026\x000A\x0002\x0000\x0002\x000A\x0037\x0000\x0026\x000A" + "\x0002\x0000\x0001\x000A\x0007\x0000\x0027\x000A\x0048\x0000\x001B\x000A\x0005\x0000\x0003\x000A\x002E\x0000\x001A\x000A" + "\x0005\x0000\x000B\x000A\x0015\x0000\x000A\x0002\x0007\x0000\x0063\x000A\x0001\x0000\x0001\x000A\x000F\x0000\x0002\x000A" + "\x0009\x0000\x000A\x0002\x0003\x000A\x0013\x0000\x0001\x000A\x0001\x0000\x001B\x000A\x0053\x0000\x0026\x000A\u015f\x0000" + "\x0035\x000A\x0003\x0000\x0001\x000A\x0012\x0000\x0001\x000A\x0007\x0000\x000A\x000A\x0004\x0000\x000A\x0002\x0015\x0000" + "\x0008\x000A\x0002\x0000\x0002\x000A\x0002\x0000\x0016\x000A\x0001\x0000\x0007\x000A\x0001\x0000\x0001\x000A\x0003\x0000" + "\x0004\x000A\x0022\x0000\x0002\x000A\x0001\x0000\x0003\x000A\x0004\x0000\x000A\x0002\x0002\x000A\x0013\x0000\x0006\x000A" + "\x0004\x0000\x0002\x000A\x0002\x0000\x0016\x000A\x0001\x0000\x0007\x000A\x0001\x0000\x0002\x000A\x0001\x0000\x0002\x000A" + 
			"\x0001\x0000\x0002\x000A\x001F\x0000\x0004\x000A\x0001\x0000\x0001\x000A\x0007\x0000\x000A\x0002\x0002\x0000\x0003\x000A" + "\x0010\x0000\x0007\x000A\x0001\x0000\x0001\x000A\x0001\x0000\x0003\x000A\x0001\x0000\x0016\x000A\x0001\x0000\x0007\x000A" + "\x0001\x0000\x0002\x000A\x0001\x0000\x0005\x000A\x0003\x0000\x0001\x000A\x0012\x0000\x0001\x000A\x000F\x0000\x0001\x000A" + "\x0005\x0000\x000A\x0002\x0015\x0000\x0008\x000A\x0002\x0000\x0002\x000A\x0002\x0000\x0016\x000A\x0001\x0000\x0007\x000A" + "\x0001\x0000\x0002\x000A\x0002\x0000\x0004\x000A\x0003\x0000\x0001\x000A\x001E\x0000\x0002\x000A\x0001\x0000\x0003\x000A" + "\x0004\x0000\x000A\x0002\x0015\x0000\x0006\x000A\x0003\x0000\x0003\x000A\x0001\x0000\x0004\x000A\x0003\x0000\x0002\x000A" + "\x0001\x0000\x0001\x000A\x0001\x0000\x0002\x000A\x0003\x0000\x0002\x000A\x0003\x0000\x0003\x000A\x0003\x0000\x0008\x000A" + "\x0001\x0000\x0003\x000A\x002D\x0000\x0009\x0002\x0015\x0000\x0008\x000A\x0001\x0000\x0003\x000A\x0001\x0000\x0017\x000A" + "\x0001\x0000\x000A\x000A\x0001\x0000\x0005\x000A\x0026\x0000\x0002\x000A\x0004\x0000\x000A\x0002\x0015\x0000\x0008\x000A" + "\x0001\x0000\x0003\x000A\x0001\x0000\x0017\x000A\x0001\x0000\x000A\x000A\x0001\x0000\x0005\x000A\x0024\x0000\x0001\x000A" + "\x0001\x0000\x0002\x000A\x0004\x0000\x000A\x0002\x0015\x0000\x0008\x000A\x0001\x0000\x0003\x000A\x0001\x0000\x0017\x000A" + "\x0001\x0000\x0010\x000A\x0026\x0000\x0002\x000A\x0004\x0000\x000A\x0002\x0015\x0000\x0012\x000A\x0003\x0000\x0018\x000A" + "\x0001\x0000\x0009\x000A\x0001\x0000\x0001\x000A\x0002\x0000\x0007\x000A\x0039\x0000\x0001\x0001\x0030\x000A\x0001\x0001" + "\x0002\x000A\x000C\x0001\x0007\x000A\x0009\x0001\x000A\x0002\x0027\x0000\x0002\x000A\x0001\x0000\x0001\x000A\x0002\x0000" + "\x0002\x000A\x0001\x0000\x0001\x000A\x0002\x0000\x0001\x000A\x0006\x0000\x0004\x000A\x0001\x0000\x0007\x000A\x0001\x0000" + "\x0003\x000A\x0001\x0000\x0001\x000A\x0001\x0000\x0001\x000A\x0002\x0000\x0002\x000A\x0001\x0000\x0004\x000A\x0001\x0000" + 
			"\x0002\x000A\x0009\x0000\x0001\x000A\x0002\x0000\x0005\x000A\x0001\x0000\x0001\x000A\x0009\x0000\x000A\x0002\x0002\x0000" + "\x0002\x000A\x0022\x0000\x0001\x000A\x001F\x0000\x000A\x0002\x0016\x0000\x0008\x000A\x0001\x0000\x0022\x000A\x001D\x0000" + "\x0004\x000A\x0074\x0000\x0022\x000A\x0001\x0000\x0005\x000A\x0001\x0000\x0002\x000A\x0015\x0000\x000A\x0002\x0006\x0000" + "\x0006\x000A\x004A\x0000\x0026\x000A\x000A\x0000\x0027\x000A\x0009\x0000\x005A\x000A\x0005\x0000\x0044\x000A\x0005\x0000" + "\x0052\x000A\x0006\x0000\x0007\x000A\x0001\x0000\x003F\x000A\x0001\x0000\x0001\x000A\x0001\x0000\x0004\x000A\x0002\x0000" + "\x0007\x000A\x0001\x0000\x0001\x000A\x0001\x0000\x0004\x000A\x0002\x0000\x0027\x000A\x0001\x0000\x0001\x000A\x0001\x0000" + "\x0004\x000A\x0002\x0000\x001F\x000A\x0001\x0000\x0001\x000A\x0001\x0000\x0004\x000A\x0002\x0000\x0007\x000A\x0001\x0000" + "\x0001\x000A\x0001\x0000\x0004\x000A\x0002\x0000\x0007\x000A\x0001\x0000\x0007\x000A\x0001\x0000\x0017\x000A\x0001\x0000" + "\x001F\x000A\x0001\x0000\x0001\x000A\x0001\x0000\x0004\x000A\x0002\x0000\x0007\x000A\x0001\x0000\x0027\x000A\x0001\x0000" + "\x0013\x000A\x000E\x0000\x0009\x0002\x002E\x0000\x0055\x000A\x000C\x0000\u026c\x000A\x0002\x0000\x0008\x000A\x000A\x0000" + "\x001A\x000A\x0005\x0000\x004B\x000A\x0095\x0000\x0034\x000A\x002C\x0000\x000A\x0002\x0026\x0000\x000A\x0002\x0006\x0000" + "\x0058\x000A\x0008\x0000\x0029\x000A\u0557\x0000\x009C\x000A\x0004\x0000\x005A\x000A\x0006\x0000\x0016\x000A\x0002\x0000" + "\x0006\x000A\x0002\x0000\x0026\x000A\x0002\x0000\x0006\x000A\x0002\x0000\x0008\x000A\x0001\x0000\x0001\x000A\x0001\x0000" + "\x0001\x000A\x0001\x0000\x0001\x000A\x0001\x0000\x001F\x000A\x0002\x0000\x0035\x000A\x0001\x0000\x0007\x000A\x0001\x0000" + "\x0001\x000A\x0003\x0000\x0003\x000A\x0001\x0000\x0007\x000A\x0003\x0000\x0004\x000A\x0002\x0000\x0006\x000A\x0004\x0000" + "\x000D\x000A\x0005\x0000\x0003\x000A\x0001\x0000\x0007\x000A\x0082\x0000\x0001\x000A\x0082\x0000\x0001\x000A\x0004\x0000" + 
			"\x0001\x000A\x0002\x0000\x000A\x000A\x0001\x0000\x0001\x000A\x0003\x0000\x0005\x000A\x0006\x0000\x0001\x000A\x0001\x0000" + "\x0001\x000A\x0001\x0000\x0001\x000A\x0001\x0000\x0004\x000A\x0001\x0000\x0003\x000A\x0001\x0000\x0007\x000A\u0ecb\x0000" + "\x0002\x000A\x002A\x0000\x0005\x000A\x000A\x0000\x0001\x000B\x0054\x000B\x0008\x000B\x0002\x000B\x0002\x000B\x005A\x000B" + "\x0001\x000B\x0003\x000B\x0006\x000B\x0028\x000B\x0003\x000B\x0001\x0000\x005E\x000A\x0011\x0000\x0018\x000A\x0038\x0000" + "\x0010\x000B\u0100\x0000\x0080\x000B\x0080\x0000\u19b6\x000B\x000A\x000B\x0040\x0000\u51a6\x000B\x005A\x000B\u048d\x000A" + "\u0773\x0000\u2ba4\x000A\u215c\x0000\u012e\x000B\x00D2\x000B\x0007\x000A\x000C\x0000\x0005\x000A\x0005\x0000\x0001\x000A" + "\x0001\x0000\x000A\x000A\x0001\x0000\x000D\x000A\x0001\x0000\x0005\x000A\x0001\x0000\x0001\x000A\x0001\x0000\x0002\x000A" + "\x0001\x0000\x0002\x000A\x0001\x0000\x006C\x000A\x0021\x0000\u016b\x000A\x0012\x0000\x0040\x000A\x0002\x0000\x0036\x000A" + "\x0028\x0000\x000C\x000A\x0074\x0000\x0003\x000A\x0001\x0000\x0001\x000A\x0001\x0000\x0087\x000A\x0013\x0000\x000A\x0002" + "\x0007\x0000\x001A\x000A\x0006\x0000\x001A\x000A\x000A\x0000\x0001\x000B\x003A\x000B\x001F\x000A\x0003\x0000\x0006\x000A" + "\x0002\x0000\x0006\x000A\x0002\x0000\x0006\x000A\x0002\x0000\x0003\x000A\x0023\x0000";
		
		/// <summary> Translates characters to character classes</summary>
		private static readonly char[] ZZ_CMAP = ZzUnpackCMap(ZZ_CMAP_PACKED);
		
		/// <summary> Translates DFA states to action switch labels.</summary>
		private static readonly int[] ZZ_ACTION = ZzUnpackAction();
		
		private const System.String ZZ_ACTION_PACKED_0 = "\x0001\x0000\x0001\x0001\x0003\x0002\x0001\x0003\x0001\x0001\x000B\x0000\x0001\x0002\x0003\x0004" + "\x0002\x0000\x0001\x0005\x0001\x0000\x0001\x0005\x0003\x0004\x0006\x0005\x0001\x0006\x0001\x0004" + "\x0002\x0007\x0001\x0008\x0001\x0000\x0001\x0008\x0003\x0000\x0002\x0008\x0001\x0009\x0001\x000A" + "\x0001\x0004";
		
		private static int[] ZzUnpackAction()
		{
			int[] result = new int[51];
			int offset = 0;
			offset = ZzUnpackAction(ZZ_ACTION_PACKED_0, offset, result);
			return result;
		}
		
		private static int ZzUnpackAction(System.String packed, int offset, int[] result)
		{
			int i = 0; /* index in packed string  */
			int j = offset; /* index in unpacked array */
			int l = packed.Length;
			while (i < l)
			{
				int count = packed[i++];
				int value_Renamed = packed[i++];
				do 
					result[j++] = value_Renamed;
				while (--count > 0);
			}
			return j;
		}
		
		
		/// <summary> Translates a state to a row index in the transition table</summary>
		private static readonly int[] ZZ_ROWMAP = ZzUnpackRowMap();
		
		private const System.String ZZ_ROWMAP_PACKED_0 = "\x0000\x0000\x0000\x000E\x0000\x001C\x0000\x002A\x0000\x0038\x0000\x000E\x0000\x0046\x0000\x0054" + "\x0000\x0062\x0000\x0070\x0000\x007E\x0000\x008C\x0000\x009A\x0000\x00A8\x0000\x00B6\x0000\x00C4" + "\x0000\x00D2\x0000\x00E0\x0000\x00EE\x0000\x00FC\x0000\u010a\x0000\u0118\x0000\u0126\x0000\u0134" + "\x0000\u0142\x0000\u0150\x0000\u015e\x0000\u016c\x0000\u017a\x0000\u0188\x0000\u0196\x0000\u01a4" + "\x0000\u01b2\x0000\u01c0\x0000\u01ce\x0000\u01dc\x0000\u01ea\x0000\u01f8\x0000\x00D2\x0000\u0206" + "\x0000\u0214\x0000\u0222\x0000\u0230\x0000\u023e\x0000\u024c\x0000\u025a\x0000\x0054\x0000\x008C" + "\x0000\u0268\x0000\u0276\x0000\u0284";
		
		private static int[] ZzUnpackRowMap()
		{
			int[] result = new int[51];
			int offset = 0;
			offset = ZzUnpackRowMap(ZZ_ROWMAP_PACKED_0, offset, result);
			return result;
		}
		
		private static int ZzUnpackRowMap(System.String packed, int offset, int[] result)
		{
			int i = 0; /* index in packed string  */
			int j = offset; /* index in unpacked array */
			int l = packed.Length;
			while (i < l)
			{
				int high = packed[i++] << 16;
				result[j++] = high | packed[i++];
			}
			return j;
		}
		
		/// <summary> The transition table of the DFA</summary>
		private static readonly int[] ZZ_TRANS = ZzUnpackTrans();
		
		private const System.String ZZ_TRANS_PACKED_0 = "\x0001\x0002\x0001\x0003\x0001\x0004\x0007\x0002\x0001\x0005\x0001\x0006\x0001\x0007\x0001\x0002" + "\x000F\x0000\x0002\x0003\x0001\x0000\x0001\x0008\x0001\x0000\x0001\x0009\x0002\x000A\x0001\x000B" + "\x0001\x0003\x0004\x0000\x0001\x0003\x0001\x0004\x0001\x0000\x0001\x000C\x0001\x0000\x0001\x0009" + "\x0002\x000D\x0001\x000E\x0001\x0004\x0004\x0000\x0001\x0003\x0001\x0004\x0001\x000F\x0001\x0010" + "\x0001\x0011\x0001\x0012\x0002\x000A\x0001\x000B\x0001\x0013\x0010\x0000\x0001\x0002\x0001\x0000" + "\x0001\x0014\x0001\x0015\x0007\x0000\x0001\x0016\x0004\x0000\x0002\x0017\x0007\x0000\x0001\x0017" + "\x0004\x0000\x0001\x0018\x0001\x0019\x0007\x0000\x0001\x001A\x0005\x0000\x0001\x001B\x0007\x0000" + "\x0001\x000B\x0004\x0000\x0001\x001C\x0001\x001D\x0007\x0000\x0001\x001E\x0004\x0000\x0001\x001F" + "\x0001\x0020\x0007\x0000\x0001\x0021\x0004\x0000\x0001\x0022\x0001\x0023\x0007\x0000\x0001\x0024" + "\x000D\x0000\x0001\x0025\x0004\x0000\x0001\x0014\x0001\x0015\x0007\x0000\x0001\x0026\x000D\x0000" + "\x0001\x0027\x0004\x0000\x0002\x0017\x0007\x0000\x0001\x0028\x0004\x0000\x0001\x0003\x0001\x0004" + "\x0001\x000F\x0001\x0008\x0001\x0011\x0001\x0012\x0002\x000A\x0001\x000B\x0001\x0013\x0004\x0000" + "\x0002\x0014\x0001\x0000\x0001\x0029\x0001\x0000\x0001\x0009\x0002\x002A\x0001\x0000\x0001\x0014" + "\x0004\x0000\x0001\x0014\x0001\x0015\x0001\x0000\x0001\x002B\x0001\x0000\x0001\x0009\x0002\x002C" + "\x0001\x002D\x0001\x0015\x0004\x0000\x0001\x0014\x0001\x0015\x0001\x0000\x0001\x0029\x0001\x0000" + "\x0001\x0009\x0002\x002A\x0001\x0000\x0001\x0016\x0004\x0000\x0002\x0017\x0001\x0000\x0001\x002E" + "\x0002\x0000\x0001\x002E\x0002\x0000\x0001\x0017\x0004\x0000\x0002\x0018\x0001\x0000\x0001\x002A" + "\x0001\x0000\x0001\x0009\x0002\x002A\x0001\x0000\x0001\x0018\x0004\x0000\x0001\x0018\x0001\x0019" + "\x0001\x0000\x0001\x002C\x0001\x0000\x0001\x0009\x0002\x002C\x0001\x002D\x0001\x0019\x0004\x0000" + 
			"\x0001\x0018\x0001\x0019\x0001\x0000\x0001\x002A\x0001\x0000\x0001\x0009\x0002\x002A\x0001\x0000" + "\x0001\x001A\x0005\x0000\x0001\x001B\x0001\x0000\x0001\x002D\x0002\x0000\x0003\x002D\x0001\x001B" + "\x0004\x0000\x0002\x001C\x0001\x0000\x0001\x002F\x0001\x0000\x0001\x0009\x0002\x000A\x0001\x000B" + "\x0001\x001C\x0004\x0000\x0001\x001C\x0001\x001D\x0001\x0000\x0001\x0030\x0001\x0000\x0001\x0009" + "\x0002\x000D\x0001\x000E\x0001\x001D\x0004\x0000\x0001\x001C\x0001\x001D\x0001\x0000\x0001\x002F" + "\x0001\x0000\x0001\x0009\x0002\x000A\x0001\x000B\x0001\x001E\x0004\x0000\x0002\x001F\x0001\x0000" + "\x0001\x000A\x0001\x0000\x0001\x0009\x0002\x000A\x0001\x000B\x0001\x001F\x0004\x0000\x0001\x001F" + "\x0001\x0020\x0001\x0000\x0001\x000D\x0001\x0000\x0001\x0009\x0002\x000D\x0001\x000E\x0001\x0020" + "\x0004\x0000\x0001\x001F\x0001\x0020\x0001\x0000\x0001\x000A\x0001\x0000\x0001\x0009\x0002\x000A" + "\x0001\x000B\x0001\x0021\x0004\x0000\x0002\x0022\x0001\x0000\x0001\x000B\x0002\x0000\x0003\x000B" + "\x0001\x0022\x0004\x0000\x0001\x0022\x0001\x0023\x0001\x0000\x0001\x000E\x0002\x0000\x0003\x000E" + "\x0001\x0023\x0004\x0000\x0001\x0022\x0001\x0023\x0001\x0000\x0001\x000B\x0002\x0000\x0003\x000B" + "\x0001\x0024\x0006\x0000\x0001\x000F\x0006\x0000\x0001\x0025\x0004\x0000\x0001\x0014\x0001\x0015" + "\x0001\x0000\x0001\x0031\x0001\x0000\x0001\x0009\x0002\x002A\x0001\x0000\x0001\x0016\x0004\x0000" + "\x0002\x0017\x0001\x0000\x0001\x002E\x0002\x0000\x0001\x002E\x0002\x0000\x0001\x0028\x0004\x0000" + "\x0002\x0014\x0007\x0000\x0001\x0014\x0004\x0000\x0002\x0018\x0007\x0000\x0001\x0018\x0004\x0000" + "\x0002\x001C\x0007\x0000\x0001\x001C\x0004\x0000\x0002\x001F\x0007\x0000\x0001\x001F\x0004\x0000" + "\x0002\x0022\x0007\x0000\x0001\x0022\x0004\x0000\x0002\x0032\x0007\x0000\x0001\x0032\x0004\x0000" + "\x0002\x0014\x0007\x0000\x0001\x0033\x0004\x0000\x0002\x0032\x0001\x0000\x0001\x002E\x0002\x0000" + "\x0001\x002E\x0002\x0000\x0001\x0032\x0004\x0000\x0002\x0014\x0001\x0000\x0001\x0031\x0001\x0000" + 
			"\x0001\x0009\x0002\x002A\x0001\x0000\x0001\x0014\x0003\x0000";
		
		private static int[] ZzUnpackTrans()
		{
			int[] result = new int[658];
			int offset = 0;
			offset = ZzUnpackTrans(ZZ_TRANS_PACKED_0, offset, result);
			return result;
		}
		
		private static int ZzUnpackTrans(System.String packed, int offset, int[] result)
		{
			int i = 0; /* index in packed string  */
			int j = offset; /* index in unpacked array */
			int l = packed.Length;
			while (i < l)
			{
				int count = packed[i++];
				int value_Renamed = packed[i++];
				value_Renamed--;
				do 
					result[j++] = value_Renamed;
				while (--count > 0);
			}
			return j;
		}
		
		
		/* error codes */
		private const int ZZ_UNKNOWN_ERROR = 0;
		private const int ZZ_NO_MATCH = 1;
		private const int ZZ_PUSHBACK_2BIG = 2;
		
		/* error messages for the codes above */
		private static readonly System.String[] ZZ_ERROR_MSG = new System.String[]{"Unkown internal scanner error", "Error: could not match input", "Error: pushback value was too large"};
		
		/// <summary> ZZ_ATTRIBUTE[aState] contains the attributes of state <c>aState</c></summary>
		private static readonly int[] ZZ_ATTRIBUTE = ZzUnpackAttribute();
		
		private const System.String ZZ_ATTRIBUTE_PACKED_0 = "\x0001\x0000\x0001\x0009\x0003\x0001\x0001\x0009\x0001\x0001\x000B\x0000\x0004\x0001\x0002\x0000" + "\x0001\x0001\x0001\x0000\x000F\x0001\x0001\x0000\x0001\x0001\x0003\x0000\x0005\x0001";
		
		private static int[] ZzUnpackAttribute()
		{
			int[] result = new int[51];
			int offset = 0;
			offset = ZzUnpackAttribute(ZZ_ATTRIBUTE_PACKED_0, offset, result);
			return result;
		}
		
		private static int ZzUnpackAttribute(System.String packed, int offset, int[] result)
		{
			int i = 0; /* index in packed string  */
			int j = offset; /* index in unpacked array */
			int l = packed.Length;
			while (i < l)
			{
				int count = packed[i++];
				int value_Renamed = packed[i++];
				do 
					result[j++] = value_Renamed;
				while (--count > 0);
			}
			return j;
		}
		
		/// <summary>the input device </summary>
		private System.IO.TextReader zzReader;
		
		/// <summary>the current state of the DFA </summary>
		private int zzState;
		
		/// <summary>the current lexical state </summary>
		private int zzLexicalState = YYINITIAL;
		
		/// <summary>this buffer contains the current text to be matched and is
		/// the source of the yytext() string 
		/// </summary>
		private char[] zzBuffer = new char[ZZ_BUFFERSIZE];
		
		/// <summary>the textposition at the last accepting state </summary>
		private int zzMarkedPos;
		
		/// <summary>the textposition at the last state to be included in yytext </summary>
		private int zzPushbackPos;
		
		/// <summary>the current text position in the buffer </summary>
		private int zzCurrentPos;
		
		/// <summary>startRead marks the beginning of the yytext() string in the buffer </summary>
		private int zzStartRead;
		
		/// <summary>endRead marks the last character in the buffer, that has been read
		/// from input 
		/// </summary>
		private int zzEndRead;
		
		/// <summary>number of newlines encountered up to the start of the matched text </summary>
		private int yyline;
		
		/// <summary>the number of characters up to the start of the matched text </summary>
		private int yychar;
		
		/// <summary> the number of characters from the last newline up to the start of the 
		/// matched text
		/// </summary>
		private int yycolumn;

        /// <summary> zzAtBOL == true &lt;=&gt; the scanner is currently at the beginning of a line</summary>
		private bool zzAtBOL = true;

        /// <summary>zzAtEOF == true &lt;=&gt; the scanner is at the EOF </summary>
		private bool zzAtEOF;
		
		/* user code: */
		
		public static readonly int ALPHANUM;
		public static readonly int APOSTROPHE;
		public static readonly int ACRONYM;
		public static readonly int COMPANY;
		public static readonly int EMAIL;
		public static readonly int HOST;
		public static readonly int NUM;
		public static readonly int CJ;
		/// <deprecated> this solves a bug where HOSTs that end with '.' are identified
		/// as ACRONYMs.
		/// </deprecated>
        [Obsolete("this solves a bug where HOSTs that end with '.' are identified as ACRONYMs")]
		public static readonly int ACRONYM_DEP;
		
		public static readonly System.String[] TOKEN_TYPES;
		
		public int Yychar()
		{
			return yychar;
		}

        /*
        * Resets the Tokenizer to a new Reader.
        */
        internal void Reset(System.IO.TextReader r)
        {
            // reset to default buffer size, if buffer has grown
            if (zzBuffer.Length > ZZ_BUFFERSIZE)
            {
                zzBuffer = new char[ZZ_BUFFERSIZE];
            }
            Yyreset(r);
        }
		
		/// <summary> Fills Lucene token with the current token text.</summary>
		internal void  GetText(Token t)
		{
			t.SetTermBuffer(zzBuffer, zzStartRead, zzMarkedPos - zzStartRead);
		}
		
		/// <summary> Fills TermAttribute with the current token text.</summary>
		internal void  GetText(ITermAttribute t)
		{
			t.SetTermBuffer(zzBuffer, zzStartRead, zzMarkedPos - zzStartRead);
		}
		
		
		/// <summary> Creates a new scanner
		/// There is also a java.io.InputStream version of this constructor.
		/// 
		/// </summary>
        /// <param name="in_Renamed"> the java.io.Reader to read input from.
		/// </param>
		internal StandardTokenizerImpl(System.IO.TextReader in_Renamed)
		{
			this.zzReader = in_Renamed;
		}
		
		/// <summary> Creates a new scanner.
		/// There is also java.io.Reader version of this constructor.
		/// 
		/// </summary>
        /// <param name="in_Renamed"> the java.io.Inputstream to read input from.
		/// </param>
		internal StandardTokenizerImpl(System.IO.Stream in_Renamed):this(new System.IO.StreamReader(in_Renamed, System.Text.Encoding.Default))
		{
		}
		
		/// <summary> Unpacks the compressed character translation table.
		/// 
		/// </summary>
		/// <param name="packed">  the packed character translation table
		/// </param>
		/// <returns>         the unpacked character translation table
		/// </returns>
		private static char[] ZzUnpackCMap(System.String packed)
		{
			char[] map = new char[0x10000];
			int i = 0; /* index in packed string  */
			int j = 0; /* index in unpacked array */
			while (i < 1154)
			{
				int count = packed[i++];
				char value_Renamed = packed[i++];
				do 
					map[j++] = value_Renamed;
				while (--count > 0);
			}
			return map;
		}
		
		
		/// <summary> Refills the input buffer.
		/// </summary>
		/// <returns><c>false</c>, iff there was new input.
		/// 
		/// </returns>
		/// <exception cref="System.IO.IOException"> if any I/O-Error occurs
		/// </exception>
		private bool ZzRefill()
		{
			
			/* first: make room (if you can) */
			if (zzStartRead > 0)
			{
				Array.Copy(zzBuffer, zzStartRead, zzBuffer, 0, zzEndRead - zzStartRead);
				
				/* translate stored positions */
				zzEndRead -= zzStartRead;
				zzCurrentPos -= zzStartRead;
				zzMarkedPos -= zzStartRead;
				zzPushbackPos -= zzStartRead;
				zzStartRead = 0;
			}
			
			/* is the buffer big enough? */
			if (zzCurrentPos >= zzBuffer.Length)
			{
				/* if not: blow it up */
				char[] newBuffer = new char[zzCurrentPos * 2];
				Array.Copy(zzBuffer, 0, newBuffer, 0, zzBuffer.Length);
				zzBuffer = newBuffer;
			}
			
			/* finally: fill the buffer with new input */
			int numRead = zzReader.Read(zzBuffer, zzEndRead, zzBuffer.Length - zzEndRead);
			
			if (numRead <= 0)
			{
				return true;
			}
			else
			{
				zzEndRead += numRead;
				return false;
			}
		}
		
		
		/// <summary> Closes the input stream.</summary>
		public void  Yyclose()
		{
			zzAtEOF = true; /* indicate end of file */
			zzEndRead = zzStartRead; /* invalidate buffer    */
			
			if (zzReader != null)
				zzReader.Close();
		}
		
		
		/// <summary> Resets the scanner to read from a new input stream.
		/// Does not close the old reader.
		/// 
		/// All internal variables are reset, the old input stream 
		/// <b>cannot</b> be reused (internal buffer is discarded and lost).
		/// Lexical state is set to <tt>ZZ_INITIAL</tt>.
		/// 
		/// </summary>
		/// <param name="reader">  the new input stream 
		/// </param>
		public void  Yyreset(System.IO.TextReader reader)
		{
			zzReader = reader;
			zzAtBOL = true;
			zzAtEOF = false;
			zzEndRead = zzStartRead = 0;
			zzCurrentPos = zzMarkedPos = zzPushbackPos = 0;
			yyline = yychar = yycolumn = 0;
			zzLexicalState = YYINITIAL;
		}
		
		
		/// <summary> Returns the current lexical state.</summary>
		public int Yystate()
		{
			return zzLexicalState;
		}
		
		
		/// <summary> Enters a new lexical state
		/// 
		/// </summary>
		/// <param name="newState">the new lexical state
		/// </param>
		public void  Yybegin(int newState)
		{
			zzLexicalState = newState;
		}
		
		
		/// <summary> Returns the text matched by the current regular expression.</summary>
		public System.String Yytext()
		{
			return new System.String(zzBuffer, zzStartRead, zzMarkedPos - zzStartRead);
		}
		
		
		/// <summary> Returns the character at position <tt>pos</tt> from the 
		/// matched text. 
		/// 
		/// It is equivalent to yytext().charAt(pos), but faster
		/// 
		/// </summary>
		/// <param name="pos">the position of the character to fetch. 
		/// A value from 0 to yylength()-1.
		/// 
		/// </param>
		/// <returns> the character at position pos
		/// </returns>
		public char Yycharat(int pos)
		{
			return zzBuffer[zzStartRead + pos];
		}
		
		
		/// <summary> Returns the length of the matched text region.</summary>
		public int Yylength()
		{
			return zzMarkedPos - zzStartRead;
		}
		
		
		/// <summary> Reports an error that occured while scanning.
		/// 
		/// In a wellformed scanner (no or only correct usage of 
		/// yypushback(int) and a match-all fallback rule) this method 
		/// will only be called with things that "Can't Possibly Happen".
		/// If this method is called, something is seriously wrong
		/// (e.g. a JFlex bug producing a faulty scanner etc.).
		/// 
		/// Usual syntax/scanner level error handling should be done
		/// in error fallback rules.
		/// 
		/// </summary>
		/// <param name="errorCode"> the code of the errormessage to display
		/// </param>
		private void  ZzScanError(int errorCode)
		{
			System.String message;
			try
			{
				message = ZZ_ERROR_MSG[errorCode];
			}
			catch (System.IndexOutOfRangeException)
			{
				message = ZZ_ERROR_MSG[ZZ_UNKNOWN_ERROR];
			}
			
			throw new System.ApplicationException(message);
		}
		
		
		/// <summary> Pushes the specified amount of characters back into the input stream.
		/// 
		/// They will be read again by then next call of the scanning method
		/// 
		/// </summary>
		/// <param name="number"> the number of characters to be read again.
		/// This number must not be greater than yylength()!
		/// </param>
		public virtual void  Yypushback(int number)
		{
			if (number > Yylength())
				ZzScanError(ZZ_PUSHBACK_2BIG);
			
			zzMarkedPos -= number;
		}
		
		
		/// <summary> Resumes scanning until the next regular expression is matched,
		/// the end of input is encountered or an I/O-Error occurs.
		/// 
		/// </summary>
		/// <returns>      the next token
		/// </returns>
		/// <exception cref="System.IO.IOException"> if any I/O-Error occurs
		/// </exception>
		public virtual int GetNextToken()
		{
			int zzInput;
			int zzAction;
			
			// cached fields:
			int zzCurrentPosL;
			int zzMarkedPosL;
			int zzEndReadL = zzEndRead;
			char[] zzBufferL = zzBuffer;
			char[] zzCMapL = ZZ_CMAP;
			
			int[] zzTransL = ZZ_TRANS;
			int[] zzRowMapL = ZZ_ROWMAP;
			int[] zzAttrL = ZZ_ATTRIBUTE;
			
			while (true)
			{
				zzMarkedPosL = zzMarkedPos;
				
				yychar += zzMarkedPosL - zzStartRead;
				
				zzAction = - 1;
				
				zzCurrentPosL = zzCurrentPos = zzStartRead = zzMarkedPosL;
				
				zzState = zzLexicalState;
				
				
				{
					while (true)
					{
						
						if (zzCurrentPosL < zzEndReadL)
							zzInput = zzBufferL[zzCurrentPosL++];
						else if (zzAtEOF)
						{
							zzInput = YYEOF;
							goto zzForAction_brk;   // {{Aroush-2.9}} this 'goto' maybe in the wrong place
						}
						else
						{
							// store back cached positions
							zzCurrentPos = zzCurrentPosL;
							zzMarkedPos = zzMarkedPosL;
							bool eof = ZzRefill();
							// get translated positions and possibly new buffer
							zzCurrentPosL = zzCurrentPos;
							zzMarkedPosL = zzMarkedPos;
							zzBufferL = zzBuffer;
							zzEndReadL = zzEndRead;
							if (eof)
							{
								zzInput = YYEOF;
								goto zzForAction_brk;   // {{Aroush-2.9}} this 'goto' maybe in the wrong place
							}
							else
							{
								zzInput = zzBufferL[zzCurrentPosL++];
							}
						}
						int zzNext = zzTransL[zzRowMapL[zzState] + zzCMapL[zzInput]];
						if (zzNext == - 1)
						{
							goto zzForAction_brk;   // {{Aroush-2.9}} this 'goto' maybe in the wrong place
						}
						zzState = zzNext;
						
						int zzAttributes = zzAttrL[zzState];
						if ((zzAttributes & 1) == 1)
						{
							zzAction = zzState;
							zzMarkedPosL = zzCurrentPosL;
							if ((zzAttributes & 8) == 8)
							{
								goto zzForAction_brk;   // {{Aroush-2.9}} this 'goto' maybe in the wrong place
							}
						}
					}
				}

zzForAction_brk: ;  // {{Aroush-2.9}} this 'lable' maybe in the wrong place
				
				
				// store back cached position
				zzMarkedPos = zzMarkedPosL;
				
				switch (zzAction < 0?zzAction:ZZ_ACTION[zzAction])
				{
					
					case 4: 
					{
						return HOST;
					}
					
					case 11:  break;
					
					case 9: 
					{
						return ACRONYM;
					}
					
					case 12:  break;
					
					case 8: 
					{
						return ACRONYM_DEP;
					}
					
					case 13:  break;
					
					case 1: 
						{
							/* ignore */
						}
						goto case 14;
					
					case 14:  break;
					
					case 5: 
					{
						return NUM;
					}
					
					case 15:  break;
					
					case 3: 
					{
						return CJ;
					}
					
					case 16:  break;
					
					case 2: 
					{
						return ALPHANUM;
					}
					
					case 17:  break;
					
					case 7: 
					{
						return COMPANY;
					}
					
					case 18:  break;
					
					case 6: 
					{
						return APOSTROPHE;
					}
					
					case 19:  break;
					
					case 10: 
					{
						return EMAIL;
					}
					
					case 20:  break;
					
					default: 
						if (zzInput == YYEOF && zzStartRead == zzCurrentPos)
						{
							zzAtEOF = true;
							return YYEOF;
						}
						else
						{
							ZzScanError(ZZ_NO_MATCH);
						}
						break;
					
				}
			}
		}
		static StandardTokenizerImpl()
		{
			ALPHANUM = StandardTokenizer.ALPHANUM;
			APOSTROPHE = StandardTokenizer.APOSTROPHE;
			ACRONYM = StandardTokenizer.ACRONYM;
			COMPANY = StandardTokenizer.COMPANY;
			EMAIL = StandardTokenizer.EMAIL;
			HOST = StandardTokenizer.HOST;
			NUM = StandardTokenizer.NUM;
			CJ = StandardTokenizer.CJ;
			ACRONYM_DEP = StandardTokenizer.ACRONYM_DEP;
			TOKEN_TYPES = StandardTokenizer.TOKEN_TYPES;
		}
	}
}