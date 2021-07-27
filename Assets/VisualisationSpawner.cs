using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IATK;
using UnityEngine;

public class VisualisationSpawner : MonoBehaviour
{
    private Material _material;

    // Start is called before the first frame update
    void Start()
    {
        var vb = new ViewBuilder(MeshTopology.Lines, "Whatever")
            .initialiseDataView(4)
            .setDataDimension(new[] {1f, 2, 3, 4}, ViewBuilder.VIEW_DIMENSION.X)
            .setDataDimension(new[] {1f, 2, 3, 4}, ViewBuilder.VIEW_DIMENSION.Y)
            .setDataDimension(new[] {1f, 2, 3, 4}, ViewBuilder.VIEW_DIMENSION.Z)
            .createIndicesConnectedLineTopology(new[] {1f, 1, 2, 2});

        Material mt =
            IATKUtil.GetMaterialFromTopology(AbstractVisualisation.GeometryType.LinesAndDots);
        mt.SetFloat("_MinSize", 0.01f);
        mt.SetFloat("_MaxSize", 0.05f);


        var v = vb.updateView()
            .apply(gameObject, mt);
    }

    // Update is called once per frame
    void Update()
    {
    }
}