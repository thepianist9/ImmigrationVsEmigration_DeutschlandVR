using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace Model
{
    public static class DataScaleCreator
    {
        public static DataScale<float> CreateDataScale(this IEnumerable<float> input)
        {
            var enumerable = input as float[] ?? input.ToArray();

            var max = enumerable.Max();
            var min = enumerable.Min();

            return new DataScale<float>
            {
                OriginalValues = enumerable,
                Transform = (x) => (max == min) ? 0 : (x - min) / (max - min)
            };
        }

        public static DataScale<int> CreateDataScale(this IEnumerable<int> input)
        {
            var enumerable = input as int[] ?? input.ToArray();

            float max = enumerable.Max();
            float min = enumerable.Min();

            return new DataScale<int>
            {
                OriginalValues = enumerable,
                Transform = (x) => (x - min) / (max - min)
            };
        }

        public static DataScale<dynamic> CreateDataScale(this IEnumerable<dynamic> input)
        {
            var enumerable = input as dynamic[] ?? input.ToArray();

            var dictionary = enumerable
                .Distinct()
                .OrderBy(it => it)
                .Select((item, i) => (item, i))
                .ToDictionary();

            float max = 0;
            float min = dictionary.Count;

            return new DataScale<dynamic>
            {
                OriginalValues = enumerable,
                Transform = (x) => (dictionary[x] - min) / (max - min)
            };
        }

        public static DataScale<float> CreateZeroCenteredDataScale(this IEnumerable<float> input)
        {
            var enumerable = input as float[] ?? input.ToArray();

            var max = Math.Max(Math.Abs(enumerable.Max()), Math.Abs(enumerable.Min()));
            var min = max * -1;

            return new DataScale<float>
            {
                OriginalValues = enumerable,
                Transform = (x) => (((x - min) / (max - min)) -0.5f) * 2
            };
        }
    }

    public class DataScale<T>
    {
        public IList<T> OriginalValues;
        public Func<T, float> Transform;

        public Lazy<IList<float>> TransformedValues => new Lazy<IList<float>>(() => OriginalValues
            .Select(Transform)
            .ToList());
    }
}