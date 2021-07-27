using System;
using Utils;

namespace Model
{
    public class Series<T> : ReadOnlyIndexedProperty<string, T>
    {
        public readonly string? Key;

        public readonly string[]? ItemKeys;

        private T[] _data;

        public Series(string? key, T[] data, string[]? header) : base(
            data.Length,
            i => data[i],
            s => Array.IndexOf(header!, s))
        {
            this.Key = key;
            _data = data;
            ItemKeys = header;
        }
    }
}