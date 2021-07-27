using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Assets.Mapbox.Unity.MeshGeneration.Modifiers.MeshModifiers;
using CsvHelper;
using IATK;
using Model;
using MoreLinq;
using UniRx;
using UnityEngine;
using Utils;
using Tuple = System.Tuple;

namespace DefaultNamespace
{
    public class ViewModel : MonoBehaviour
    {
        public DataSource source;

        public DataFrame Data;
        public DataFrame PairData;
        public DataFrame StateData;


        public BehaviorSubject<List<string>> selectedStates =
            new BehaviorSubject<List<string>>(new List<string>() { });

        public BehaviorSubject<List<float>> selectedYears =
            new BehaviorSubject<List<float>>(new List<float>()
            {
                2004, 2005, 2006, 2007, 2008, 2009, 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017,
                2018, 2019
            });

        public BehaviorSubject<Unit> OnChanged = new BehaviorSubject<Unit>(new Unit());

        // public selectedYears = new BehaviorSubject([])

        public List<float> AvailableYears;

        public List<string> AvailableStates;

        public IList<float> AvailableYearsNormalized;

        private DataScale<float> _yearScale;

        public void InitIfNecessary()
        {
            if (Data != null) return;

            source.load();
            Data = new DataFrame(source);

            AvailableStates = Data.Columns["origin"]
                .Distinct()
                .Where(it => it != "Insgesamt")
                .Cast<string>()
                .ToList();

            AvailableYears = Data.Columns["year"].Cast<float>().Distinct().ToList();
            AvailableYearsNormalized = AvailableYears.CreateDataScale().TransformedValues.Value;

            _yearScale = AvailableYears.CreateDataScale();

            Observable.CombineLatest(
                    selectedStates,
                    selectedYears,
                    (states, years) => (states, years))
                .Subscribe((it) =>
                {
                    InitPairData();
                    InitStateData();
                    OnChanged.OnNext(new Unit());
                });
        }

        public void Start()
        {
            InitIfNecessary();
        }

        private void InitStateData()
        {
            var stateYearCombinations = GetDisplayedStates().SelectMany(state =>
                GetDisplayedYears().Select(year => (state, year))
            );

            var colYear = stateYearCombinations.Select(it => it.year).ToArray();
            var colState = stateYearCombinations.Select(it => it.state).ToArray();

            StateData = new DataFrame(new Dictionary<string, dynamic[]>()
            {
                {"year", colYear.Cast<dynamic>().ToArray()},
                {"state", colState.Cast<dynamic>().ToArray()}
            });

            StateData.AddColumn("saldo", i => GetSaldo(colState[i], colYear[i]));

            StateData.AddColumn<float, float>("saldo", "saldoNormalized",
                col => col.CreateZeroCenteredDataScale().TransformedValues.Value);
            StateData.AddColumn("yearNormalized", i => _yearScale.Transform(colYear[i]));
        }

        public List<string> GetDisplayedStates() => selectedStates.Value.Count switch
        {
            0 => AvailableStates,
            _ => selectedStates.Value
        };


        public List<float> GetDisplayedYears() => selectedYears.Value.Count switch
        {
            0 => AvailableYears,
            _ => selectedYears.Value
        };

        public IEnumerable<(string a, string b)> GetDisplayedPairs() =>
            selectedStates.Value.Count switch
            {
                0 => AvailableStates.AllCombinations(),
                1 => AvailableStates.AllCombinations()
                    .Where(pair =>
                        selectedStates.Value.Contains(pair.a) ||
                        selectedStates.Value.Contains(pair.b)),
                _ => selectedStates.Value.AllCombinations()
            };

        private void InitPairData()
        {
            var pairs = AvailableStates.AllCombinations().ToList();

            var yearPairCombinations = GetDisplayedPairs()
                .SelectMany(pair =>
                    GetDisplayedYears().Select(year => (pair.a, pair.b, year))
                )
                .ToList();

            PairData = new DataFrame(new Dictionary<string, dynamic[]>
            {
                {"year", yearPairCombinations.Select(it => it.year).Cast<dynamic>().ToArray()},
                {"a", yearPairCombinations.Select(it => it.a).Cast<dynamic>().ToArray()},
                {"b", yearPairCombinations.Select(it => it.b).Cast<dynamic>().ToArray()},
            });

            var colA = PairData.Columns["a"];
            var colB = PairData.Columns["b"];
            var colYear = PairData.Columns["year"];

            PairData.AddColumn("aToB", i => GetMigrations(colA[i], colB[i], colYear[i]));
            PairData.AddColumn("bToA", i => GetMigrations(colB[i], colA[i], colYear[i]));

            var colAToB = PairData.Columns["aToB"];
            var colBToA = PairData.Columns["bToA"];

            var colGainedByA = Enumerable.Zip(colAToB, colBToA, (a, b) => b - a).ToArray();
            var colGainedByB = colGainedByA.Select(x => x * -1).ToArray();

            PairData.AddColumn(colGainedByA, "gainedByA");
            PairData.AddColumn(colGainedByB, "gainedByB");

            PairData.AddFloatNormalizedColumn("aToB", "aToBNormalized");
            PairData.AddFloatNormalizedColumn("bToA", "bToANormalized");
            PairData.AddColumn(
                "gainedByANormalized",
                colGainedByA.Cast<float>().CreateZeroCenteredDataScale().TransformedValues.Value
            );
            PairData.AddColumn(
                "gainedByBNormalized",
                colGainedByB.Cast<float>().CreateZeroCenteredDataScale().TransformedValues.Value
            );
        }

        private float GetMigrations(string from, string to, float year)
        {
            var originCol = Data.Columns["origin"];
            var destinationCol = Data.Columns["destination"];
            var yearCol = Data.Columns["year"];

            var i = Enumerable.Range(0, Data.RowCount).First(i =>
                originCol[i] == from
                && destinationCol[i] == to
                && yearCol[i] == year
            );

            return Data["all", i];
        }

        private float GetBidirectionalMigration(string a, string b, float year)
        {
            return GetMigrations(a, b, year) + GetMigrations(b, a, year);
        }

        private float GetSaldo(string state, float year)
        {
            var colAll = Data.Columns["all"];
            var colYear = Data.Columns["year"];
            var colDestination = Data.Columns["destination"];
            var colOrigin = Data.Columns["origin"];

            var iFromOthersToState = Enumerable.Range(0, Data.RowCount)
                .First(i =>
                    colOrigin[i] == "Insgesamt" && colDestination[i] == state &&
                    colYear[i] == year);

            var iFromStateToOthers = Enumerable.Range(0, Data.RowCount)
                .First(i =>
                    colOrigin[i] == state && colDestination[i] == "Insgesamt" &&
                    colYear[i] == year);

            return colAll[iFromOthersToState] - colAll[iFromStateToOthers];
        }
    }
}