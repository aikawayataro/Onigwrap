using System;
using System.Text;

using Onigwrap.Internal;

namespace Onigwrap
{
    public class ORegex
    {
        private static object _createRegexSync = new object();

        private IntPtr _regex;
        private IntPtr _region;
        private object _syncObject = new object();
        private string _regexString;

        public bool Valid
        {
            get
            {
                return _regex != IntPtr.Zero;
            }
        }

        public unsafe ORegex(string pattern, bool ignoreCase = true, bool multiline = false)
        {
            int ignoreCaseArg = ignoreCase ? 1 : 0;
            int multilineArg = multiline ? 1 : 0;

            pattern = UnicodeCharEscape.AddBracesToUnicodePatterns(pattern);
            pattern = UnicodeCharEscape.ConstraintUnicodePatternLenght(pattern);

            lock (_createRegexSync)
            {
                fixed (char* patternPtr = pattern)
                {
                    _regex = OnigInterop.onigwrap_create(
                        patternPtr,
                        Encoding.Unicode.GetByteCount(patternPtr, pattern.Length),
                        ignoreCaseArg,
                        multilineArg);
                }
            }

            if (!Valid)
                _regexString = pattern; // Save the pattern off on invalid patterns for throwing exceptions
        }

        /// <summary>
        /// Performs a thread safe search and returns the results in a list
        /// </summary>
        /// <param name="text">The text to search</param>
        /// <param name="offset">An offset from which to start</param>
        /// <returns></returns>
        public unsafe OnigResult SafeSearch(string text, int offset = 0)
        {
            if (!Valid) throw new ArgumentException($"Invalid regular expression: {_regexString}");

            lock (_syncObject)
            {
                if (_region == IntPtr.Zero)
                    _region = OnigInterop.onigwrap_region_new();

                fixed (char* textPtr = text)
                {
                    OnigInterop.onigwrap_search(
                        _regex,
                        textPtr,
                        Encoding.Unicode.GetByteCount(textPtr, offset),
                        Encoding.Unicode.GetByteCount(textPtr, text.Length),
                        _region);
                }

                var captureCount = OnigInterop.onigwrap_num_regs(_region);

                Region region = null;

                for (var capture = 0; capture < captureCount; capture++)
                {
                    var pos = OnigInterop.onigwrap_pos(_region, capture);
                    if (capture == 0 && pos < 0)
                        return null;

                    int len = pos == -1 ? 0 : OnigInterop.onigwrap_len(_region, capture);

                    if (region == null)
                        region = new Region(captureCount);

                    region.Start[capture] = pos;
                    region.End[capture] = pos + len;
                }

                return new OnigResult(region, -1);
            }
        }

        ~ORegex()
        {
            if (_region != IntPtr.Zero)
                OnigInterop.onigwrap_region_free(_region);

            if (_regex != IntPtr.Zero)
                OnigInterop.onigwrap_free(_regex);
        }
    }
}
