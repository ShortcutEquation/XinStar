using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lucene.Net.Analysis
{
    public sealed class NoTokenizer : CharTokenizer
    {
        /// <summary>Construct a new NoTokenizer. </summary>
		public NoTokenizer(System.IO.TextReader @in):base(@in)
		{
        }

        /// <summary>Construct a new NoTokenizer using a given <see cref="AttributeSource" />. </summary>
        public NoTokenizer(AttributeSource source, System.IO.TextReader @in)
			: base(source, @in)
		{
        }

        /// <summary>Construct a new NoTokenizer using a given <see cref="Lucene.Net.Util.AttributeSource.AttributeFactory" />. </summary>
        public NoTokenizer(AttributeFactory factory, System.IO.TextReader @in)
			: base(factory, @in)
		{
        }

        protected internal override bool IsTokenChar(char c)
        {
            return true;
        }

        protected internal override char Normalize(char c)
        {
            return System.Char.ToLower(c);
        }
    }
}
