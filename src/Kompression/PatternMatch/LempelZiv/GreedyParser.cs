﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kompression.Extensions;
using Kompression.IO;

namespace Kompression.PatternMatch.LempelZiv
{
    /// <summary>
    /// Searches for longest pattern matches at position i and jumps over them.
    /// </summary>
    public class GreedyParser : IMatchParser
    {
        private IMatchFinder _finder;

        public bool IsBackwards { get; }

        public int PreBufferSize { get; }

        public int SkipAfterMatch { get; }

        /// <summary>
        /// Creates a new instance of <see cref="GreedyParser"/>.
        /// </summary>
        /// <param name="finder">The <see cref="IMatchFinder"/> to find the longest pattern match at a given position.</param>
        /// <param name="skipAfterMatch">The bytes to skip after a found pattern match.</param>
        public GreedyParser(IMatchFinder finder, bool isBackwards, int preBufferSize, int skipAfterMatch)
        {
            _finder = finder;
            SkipAfterMatch = skipAfterMatch;
            IsBackwards = isBackwards;
            PreBufferSize = preBufferSize;
        }

        public Match[] ParseMatches(Stream input)
        {
            var toParse = input;

            if (PreBufferSize > 0)
                toParse = IsBackwards ? 
                    new ConcatStream(input, new MemoryStream(new byte[PreBufferSize])) : 
                    new ConcatStream(new MemoryStream(new byte[PreBufferSize]), input);

            if (IsBackwards)
                toParse = new ReverseStream(toParse, toParse.Length);

            return ParseMatches(toParse.ToArray(), PreBufferSize);
        }

        /// <inheritdoc cref="ParseMatches"/>
        public Match[] ParseMatches(byte[] input, int startPosition)
        {
            var results = new List<Match>();

            var positionOffset = 0;
            while (startPosition + positionOffset < input.Length)
            {
                // Get longest match at position positionOffset
                var match = _finder.FindMatches(input, startPosition + positionOffset).FirstOrDefault();
                if (match == null)
                    positionOffset += (int)_finder.DataType;
                else
                {
                    // Skip the whole pattern
                    results.Add(match);
                    positionOffset += (int)match.Length + SkipAfterMatch * (int)_finder.DataType;
                }
            }

            return results.ToArray();
        }

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool dispose)
        {
            if (dispose)
            {
                _finder.Dispose();
                _finder = null;
            }
        }

        #endregion
    }
}