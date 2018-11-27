using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lucene.Net.Analysis
{
    public class NoAnalyzer : Analyzer
    {
        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            return new NoTokenizer(reader);
        }

        public override TokenStream ReusableTokenStream(string fieldName, TextReader reader)
        {
            var tokenizer = (Tokenizer)PreviousTokenStream;
            if (tokenizer == null)
            {
                tokenizer = new NoTokenizer(reader);
                PreviousTokenStream = tokenizer;
            }
            else
                tokenizer.Reset(reader);
            return tokenizer;
        }
    }
}
