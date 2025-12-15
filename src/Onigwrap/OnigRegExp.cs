using System;
using System.Runtime.InteropServices;

namespace Onigwrap
{
    public class OnigRegExp
    {
        private ReadOnlyMemory<char> _lastSearchString;
        private int _lastSearchPosition;
        private OnigResult _lastSearchResult;
        private ORegex _regex;

        public OnigRegExp(string source)
        {
            _lastSearchString = ReadOnlyMemory<char>.Empty;
            _lastSearchPosition = -1;
            _lastSearchResult = null;

            _regex = new ORegex(source, false, false);
        }

        public OnigResult Search(string str, int position)
        {
            return Search(str.AsMemory(), position);
        }

        public OnigResult Search(ReadOnlyMemory<char> str, in int position)
        {
            // Only use cache for string-backed memory (immutable).
            // Array-backed memory can have its content changed while keeping the same
            // reference, making reference-based comparison unsafe (e.g., ArrayPool reuse).
            bool isStringBacked = MemoryMarshal.TryGetString(str, out _, out _, out _);

            if (isStringBacked &&
                _lastSearchString.Equals(str) &&
                _lastSearchPosition <= position &&
                (_lastSearchResult == null || _lastSearchResult.LocationAt(0) >= position))
            {
                return _lastSearchResult;
            }

            _lastSearchString = str;
            _lastSearchPosition = position;
            _lastSearchResult = GetOnigResult(str, position);
            return _lastSearchResult;
        }

        private OnigResult GetOnigResult(ReadOnlyMemory<char> data, in int position)
        {
            return _regex.SafeSearch(data.Span, position);
        }
    }
}
