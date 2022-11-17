using System.Collections.Generic;

namespace Common.Helpers
{
    public class FibonacciHelper: IFibonacciHelper
    {
        private static List<int> _cache = new List<int> { 1, 2 };
        private static readonly object _cacheLock = new object();

        public int GetNextNumber(int number)
        {
            lock (_cacheLock)
            {
                int index =_cache.FindLastIndex(x => x == number);
                if (index + 1 >= _cache.Count)
                {
                    _cache.Add(_cache[index] + _cache[index - 1]);
                }

                return _cache[index + 1];
            }
        }
    }
}