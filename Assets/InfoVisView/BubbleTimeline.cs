using System.Linq;
using DefaultNamespace;
using EasyButtons;
using IATK;
using Model;
using UniRx;
using UnityEngine;

namespace InfoVisView
{
    public class BubbleTimeline : MonoBehaviour
    {
        public ViewModel vm;

        public float height;

        public Transform bubbleContainer;

        public AnimationCurve _sizeCurve;


        [Button]
        private void UpdateBubbles()
        {
            foreach (Transform child in bubbleContainer)
            {
                Destroy(child.gameObject);
            }

            var state = _stateRepresentative.stateString;
            var rows = vm.StateData.Rows.Where(it => it["state"] == state);

            rows.ForEach(DrawBubble);
        }


        public GameObject bubblePrefab;

        public GameObject timelinePrefab;

        private StateRepresentative _stateRepresentative;

        public Vector3 GetPositionForYear(float year)
        {
            var offset = height / 20;
            var remainingHeight = height - offset;

            var row = vm.AvailableYears.IndexOf(year);
            var y = vm.AvailableYearsNormalized[row] * remainingHeight + offset;

            return transform.TransformPoint(new Vector3(0, y, 0));
        }

        // Start is called before the first frame update
        void Start()
        {
            vm = FindObjectOfType<ViewModel>();
            vm.InitIfNecessary();

            _stateRepresentative = GetComponent<StateRepresentative>();

            vm.selectedStates.Subscribe(it => UpdateBubbles());
            vm.OnChanged.TakeUntilDisable(this).Subscribe(it => UpdateBubbles());
        }

        private void DrawBubble(Series<dynamic> row)
        {
            float saldo = row["saldoNormalized"];
            // float size = _sizeCurve.Evaluate(saldo);
            float size = 0.02f;


            GameObject bubble = Instantiate(bubblePrefab,
                GetPositionForYear(row["year"]),
                Quaternion.identity,
                bubbleContainer);

            bubble.transform.localScale = new Vector3(size, size, size);

            var renderer = bubble.GetComponent<Renderer>();

            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(propBlock);
            var color = Style.ColorMap.Evaluate((saldo + 1) / 2);
            propBlock.SetColor("_Color", color);
            propBlock.SetColor("_EmissionColor", color);
            renderer.SetPropertyBlock(propBlock);
        }
    }
}