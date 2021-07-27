using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Utils
{
    public class ReadOnlyIndexedProperty<TIndex, TValue> : IEnumerable<TValue>
    {
        private readonly Func<int, TValue> _getFunc;
        private readonly Func<TIndex, int> _getIndex;

        public int Length;

        public ReadOnlyIndexedProperty(int length, Func<int, TValue> getFunc,
            Func<TIndex, int> getIndex)
        {
            _getFunc = getFunc;
            _getIndex = getIndex;
            Length = length;
        }

        public TValue this[TIndex i] => _getFunc(_getIndex(i));

        public TValue this[int i] => _getFunc(i);

        public IEnumerator<TValue> GetEnumerator()
        {
            for (int i = 0; i < Length; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    static class Utils
    {
        public static IEnumerable<(T a, T b)> AllCombinations<T>(this IEnumerable<T> input)
        {
            var enumerable = input as T[] ?? input.ToArray();
            return enumerable.SelectMany((a, i) => enumerable.Take(i).Select(b => (a, b)));
        }
    }
}