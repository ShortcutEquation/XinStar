using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Index;

namespace com.star.Logic.Search
{
    public class XSimilarity : Similarity
    {
        public override float ComputeNorm(string field, FieldInvertState state)
        {
            return (float)(state.Boost);
        }

        public override float Coord(int overlap, int maxOverlap)
        {
            return 1.0f;
        }

        public override float Idf(int docFreq, int numDocs)
        {
            return 1.0f;
        }

        public override float LengthNorm(string fieldName, int numTokens)
        {
            return 1.0f;
        }

        public override float QueryNorm(float sumOfSquaredWeights)
        {
            return 1.0f;
        }

        public override float SloppyFreq(int distance)
        {
            return 1.0f / (distance + 1);
        }

        public override float Tf(float freq)
        {
            return 1.0f;
        }
    }
}
