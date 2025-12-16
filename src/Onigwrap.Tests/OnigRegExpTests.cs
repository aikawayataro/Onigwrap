using System;
using System.Buffers;
using NUnit.Framework;

namespace Onigwrap.Tests
{
    class OnigRegExpTests
    {
        [Test]
        public void Basic_Regex_Should_Have_Valid_Location_And_Length()
        {
            OnigRegExp regExp = new OnigRegExp("[A-C]+");

            string str = "abcABC123";
            OnigResult result = regExp.Search(str.AsMemory(), 0);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(3, result.LocationAt(0));
            Assert.AreEqual(3, result.LengthAt(0));
        }

        [Test]
        public void UTF8_Regex_Should_Have_Valid_Location_And_Length()
        {
            OnigRegExp regExp = new OnigRegExp("[á]+");

            string str = "00áá00";
            OnigResult result = regExp.Search(str.AsMemory(), 0);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(2, result.LocationAt(0));
            Assert.AreEqual(2, result.LengthAt(0));
        }

        [Test]
        public void Unicode_Regex_Should_Have_Valid_Location_And_Length()
        {
            string text = "\"安\"";
            string pattern = "\\\"[^\"]*\\\"";

            OnigRegExp regExp = new OnigRegExp(pattern);

            OnigResult result = regExp.Search(text.AsMemory(), 0);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(0, result.LocationAt(0));
            Assert.AreEqual(3, result.LengthAt(0));
        }

        [Test]
        public void Basic_Regex_Should_Have_Valid_Location_And_Length2()
        {
            string text = "string s=\"安\"";
            string pattern = "\\\"[^\"]*\\\"";

            OnigRegExp regExp = new OnigRegExp(pattern);

            OnigResult result = regExp.Search(text.AsMemory(), 0);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(9, result.LocationAt(0));
            Assert.AreEqual(3, result.LengthAt(0));
        }

        [Test]
        public void Unicode_Regex_Without_Braces_Should_Be_Valid()
        {
            string pattern = "[\\xa0-\\xF7]";

            ORegex oRegex = new ORegex(pattern);

            Assert.IsTrue(oRegex.Valid);
        }

        [Test]
        public void Unicode_Regex_With_Constraint_Pattern_Should_Be_Valid()
        {
            string pattern = "(?i)^\\s*(interface)\\s+([a-z_\\x{7f}-\\x{7fffffff}][a-z0-9_\\x{7f}-\\x{7fffffff}]*)\\s*(extends)?\\s*";

            ORegex oRegex = new ORegex(pattern);

            Assert.IsTrue(oRegex.Valid);
        }

        [Test]
        public void Unicode_Regex_With_Unicode_Chars_Bigger_Than_2_Bytes_Should_Be_Valid()
        {
            string pattern = "\U0001D11E";

            ORegex oRegex = new ORegex(pattern);

            Assert.IsTrue(oRegex.Valid);
        }

        [Test]
        public void Search_Twice_In_Regex_Should_Regenerate_Location_And_Length()
        {
            OnigRegExp regExp = new OnigRegExp("[A-C]+");

            string str1 = "abcABC123";
            string str2 = "abc123ABC";

            OnigResult result1 = regExp.Search(str1.AsMemory(), 0);
            OnigResult result2 = regExp.Search(str2.AsMemory(), 0);

            Assert.AreEqual(1, result1.Count());
            Assert.AreEqual(3, result1.LocationAt(0));
            Assert.AreEqual(3, result1.LengthAt(0));

            Assert.AreEqual(1, result2.Count());
            Assert.AreEqual(6, result2.LocationAt(0));
            Assert.AreEqual(3, result2.LengthAt(0));
        }
        
        [Test]
        public void Search_Within_Previous_Match_Should_Return_Same_Result()
        {
            OnigRegExp regExp = new OnigRegExp("[A-C]+");

            string str = "abcABC123";

            OnigResult result1 = regExp.Search(str.AsMemory(), 0);
            OnigResult result2 = regExp.Search(str.AsMemory(), 2); // within the previous match

            Assert.AreSame(result1, result2);
        }

        [Test]
        public void ArrayPool_Buffer_Reuse_Should_Not_Return_Cached_Results_For_Different_Content()
        {
            // Pattern that matches digits
            OnigRegExp regExp = new OnigRegExp("\\d+");

            // Use a fixed-size buffer to ensure we get the same buffer back from ArrayPool
            const int bufferSize = 10;

            // First search: content has digits at position 0
            char[] buffer1 = ArrayPool<char>.Shared.Rent(bufferSize);
            try
            {
                "123abc____".CopyTo(0, buffer1, 0, bufferSize);
                ReadOnlyMemory<char> memory1 = buffer1.AsMemory(0, bufferSize);

                OnigResult result1 = regExp.Search(memory1, 0);
                Assert.IsNotNull(result1, "First search should find digits");
                Assert.AreEqual(0, result1.LocationAt(0), "First search: digits at position 0");
                Assert.AreEqual(3, result1.LengthAt(0), "First search: 3 digits");
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer1, clearArray: false);
            }

            // Second search: different content with digits at position 6
            // We rent again to get the same buffer
            char[] buffer2 = ArrayPool<char>.Shared.Rent(bufferSize);
            try
            {
                "abcdef789_".CopyTo(0, buffer2, 0, bufferSize);
                ReadOnlyMemory<char> memory2 = buffer2.AsMemory(0, bufferSize);

                // If buffer2 == buffer1 (same underlying array), the cache will incorrectly
                // return result1 because ReadOnlyMemory.Equals compares references not content
                OnigResult result2 = regExp.Search(memory2, 0);

                Assert.IsNotNull(result2, "Second search should find digits");
                Assert.AreEqual(6, result2.LocationAt(0), "Second search: digits should be at position 6");
                Assert.AreEqual(3, result2.LengthAt(0), "Second search: 3 digits");
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer2, clearArray: false);
            }
        }
    }
}
