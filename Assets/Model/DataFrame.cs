using System;
using System.Collections.Generic;
using System.Linq;
using IATK;
using Utils;

namespace Model
{
    public class DataFrame
    {
        private readonly List<dynamic[]> _data;

        public readonly List<string>? ColumnKeys;

        public readonly List<string>? RowKeys;

        public int RowCount => _data[0].Length;

        public DataFrame(DataSource source)
        {
            ColumnKeys = source.Select(dimension => dimension.Identifier).ToList();
            RowKeys = null;
            _data = source
                .Select((dimension, i) =>
                    dimension.Data
                        .Select(it => source.getOriginalValue(it, i))
                        .Cast<dynamic>()
                        .ToArray())
                .ToList();
        }

        public DataFrame(List<dynamic[]> data, List<string>? columnKeys = null,
            List<string>? rowKeys = null)
        {
            _data = data;
            ColumnKeys = columnKeys;
            RowKeys = rowKeys;
        }

        public dynamic this[string col, int row] => Columns[col][row];

        public DataFrame(Dictionary<string, dynamic[]> columns)
        {
            RowKeys = null;
            ColumnKeys = new List<string>();
            _data = new List<dynamic[]>();

            foreach (var entry in columns)
            {
                ColumnKeys.Add(entry.Key);
                _data.Add(entry.Value);
            }
        }

        public ReadOnlyIndexedProperty<string, Series<dynamic>> Rows =>
            new ReadOnlyIndexedProperty<string, Series<dynamic>>(
                _data.FirstOrDefault()?.Length ?? 0,
                i => new Series<dynamic>(
                    RowKeys?[i],
                    _data.Select(it => it[i]).ToArray(),
                    ColumnKeys?.ToArray()
                ),
                key => RowKeys!.IndexOf(key)
            );


        public ReadOnlyIndexedProperty<string, Series<dynamic>> Columns =>
            new ReadOnlyIndexedProperty<string, Series<dynamic>>(
                _data.Count,
                i => new Series<dynamic>(ColumnKeys?[i], _data[i], RowKeys?.ToArray()),
                key => ColumnKeys!.IndexOf(key));

        public void AddColumn(dynamic[] data, string? key = null)
        {
            // verify length
            var expectedLength = _data.ElementAtOrDefault(0)?.Length;
            if (expectedLength != null && data.Length != expectedLength)
            {
                throw new Exception("dimension has wrong length");
            }

            _data.Add(data);
            if (key != null) ColumnKeys!.Add(key);
        }

        public void AddColumn<T>(string? key, IEnumerable<T> data)
        {
            var dataArray = data.Cast<dynamic>().ToArray();

            // verify length
            var expectedLength = _data.ElementAtOrDefault(0)?.Length;
            if (expectedLength != null && dataArray.Length != expectedLength)
            {
                throw new Exception("dimension has wrong length");
            }

            _data.Add(dataArray);
            if (key != null) ColumnKeys!.Add(key);
        }

        public void AddColumn(Func<Series<dynamic>, dynamic> mapRow, string? key = null)
        {
            var column = Rows.Select(mapRow).ToArray();

            _data.Add(column);
            if (key != null) ColumnKeys!.Add(key);
        }

        public void AddNormalizedColumn(string sourceColName, string targetColName)
        {
            var transformedValues = Columns[sourceColName]
                .CreateDataScale()
                .TransformedValues.Value
                .Cast<dynamic>()
                .ToArray();

            AddColumn(transformedValues, targetColName);
        }

        public void AddIntNormalizedColumn(string sourceColName, string targetColName)
        {
            var transformedValues = Columns[sourceColName]
                .Cast<int>()
                .CreateDataScale()
                .TransformedValues.Value
                .Cast<dynamic>()
                .ToArray();

            AddColumn(transformedValues, targetColName);
        }

        public void AddFloatNormalizedColumn(string sourceColName, string targetColName)
        {
            var transformedValues = Columns[sourceColName]
                .Cast<float>()
                .CreateDataScale()
                .TransformedValues.Value
                .Cast<dynamic>()
                .ToArray();

            AddColumn(transformedValues, targetColName);
        }


        public void AddColumn<TSource, TTarget>(string sourceColName, string targetColName,
            Func<IEnumerable<TSource>, IEnumerable<TTarget>> mapper)
        {
            var sourceCol = Columns[sourceColName].Cast<TSource>();
            var transformedCol = mapper(sourceCol).Cast<dynamic>().ToArray();

            AddColumn(transformedCol, targetColName);
        }

        public void AddColumn<T>(string targetColName, Func<int, T> mapper)
        {
            var newCol = Enumerable.Range(0, RowCount).Select(mapper).Cast<dynamic>().ToArray();
            AddColumn(newCol, targetColName);
        }
    }
}