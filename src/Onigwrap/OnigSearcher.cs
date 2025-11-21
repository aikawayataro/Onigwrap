using System;
using System.Collections.Generic;

namespace Onigwrap
{
    public class OnigSearcher
    {
        private List<OnigRegExp> _regExps;

        public OnigSearcher(string[] regexps)
        {
            _regExps = new List<OnigRegExp>(regexps.Length);
            foreach (string regexp in regexps)
            {
                _regExps.Add(new OnigRegExp(regexp));
            }
        }

        public OnigResult Search(ReadOnlyMemory<char> source, in int charOffset)
        {
            int bestLocation = 0;
            OnigResult bestResult = null;
            int index = 0;

            foreach (OnigRegExp regExp in _regExps)
            {
                OnigResult result = regExp.Search(source, charOffset);
                if (result != null && result.Count() > 0)
                {
                    int location = result.LocationAt(0);

                    if (bestResult == null || location < bestLocation)
                    {
                        bestLocation = location;
                        bestResult = result;
                        bestResult.SetIndex(index);
                    }

                    if (location == charOffset)
                    {
                        break;
                    }
                }
                index++;
            }
            return bestResult;
        }
    }
}
