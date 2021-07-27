using System;
using System.Linq;
using DefaultNamespace;
using IATK;
using UniRx;
using UnityEngine;
using Utils;


namespace InfoVisView
{
    public class LinkManager : MonoBehaviour
    {
        public LineRenderer linkPrefab;

        public ViewModel vm;

        public Transform linkContainer;

        public AnimationCurve opacityCurve;

        private void Start()
        {
            vm = FindObjectOfType<ViewModel>();
            vm.InitIfNecessary();

            vm.OnChanged.TakeUntilDestroy(this).Subscribe(next => UpdateLinks());
        }

        private void UpdateLinks()
        {
            foreach (Transform child in linkContainer)
            {
                Destroy(child.gameObject);
            }

            var migrationVisualisations =
                gameObject.GetComponentsInChildren<BubbleTimeline>();

            migrationVisualisations
                .AllCombinations()
                .ForEach(pair => CreateLinks(pair.a, pair.b));
        }

        private void CreateLinks(BubbleTimeline timeline1, BubbleTimeline timeline2) => vm
            .GetDisplayedYears().ForEach(year => CreateLinkForYear(timeline1, timeline2, year));

        private void CreateLinkForYear(BubbleTimeline timeline1, BubbleTimeline timeline2,
            float year)
        {
            var state1 = timeline1.GetComponent<StateRepresentative>().stateString;
            var state2 = timeline2.GetComponent<StateRepresentative>().stateString;

            var colA = vm.PairData.Columns["a"];
            var colB = vm.PairData.Columns["b"];
            var colYear = vm.PairData.Columns["year"];

            var rowIndex = Enumerable.Range(0, vm.PairData.RowCount).FirstOrDefault(i =>
                ((colA[i] == state1 && colB[i] == state2) ||
                 (colB[i] == state1 && colA[i] == state2))
                && colYear[i] == year
            );
            if (rowIndex == null) return;

            var row = vm.PairData.Rows[rowIndex];

            var timelineA = state1 == row["a"] ? timeline1 : timeline2;
            var timelineB = state1 == row["b"] ? timeline1 : timeline2;

            var colorA = Style.ColorMap.Evaluate((row["gainedByANormalized"] / 2) + 0.5f);
            var colorB = Style.ColorMap.Evaluate((row["gainedByBNormalized"] / 2) + 0.5f);
            var opacity = opacityCurve.Evaluate(Math.Abs(row["gainedByANormalized"]));
            var lineRenderer = Instantiate(linkPrefab, linkContainer, true);

            var gradient = new Gradient();
            gradient.SetKeys(new[]
            {
                new GradientColorKey(colorA, 0),
                new GradientColorKey(colorB, 1),
            }, new[]
            {
                new GradientAlphaKey(opacity, 0)
            });

            lineRenderer.colorGradient = gradient;

            lineRenderer.startWidth = Math.Abs(row["gainedByANormalized"] * 0.01f);
            lineRenderer.endWidth = Math.Abs(row["gainedByBNormalized"] * 0.01f);

            lineRenderer.SetPositions(
                new[]
                {
                    timelineA.GetPositionForYear(year),
                    timelineB.GetPositionForYear(year)
                });
        }
    }
}