using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;
using System.Linq;
using TMPro;
using System;

public class Timevis : MonoBehaviour
{
    public CSVDataSource data;
    public List<string> states = new List<string>(new string[] { "Baden-Wuerttemberg"});

    [SerializeField]
    private GameObject labelTemplatez;
    [SerializeField]
    private GameObject AxisLabels;
    [SerializeField]
    private GameObject Axis;


    private static Hashtable stateColourValues = new Hashtable{
         { "Baden-Wuerttemberg", (Color)new Color32(255, 82, 82, 255) },
         { "Bayern", (Color)new Color32(245, 0, 87, 255) },
         { "Berlin", (Color)new Color32(170, 0, 255, 255) },
         { "Brandenburg", (Color)new Color32(101, 31, 255, 255) },
         { "Bremen", (Color)new Color32(61, 90, 254, 255) },
         { "Hamburg",(Color)new Color32(41, 121, 255, 255) },
         { "Hessen", (Color)new Color32(0, 176, 255, 255) },
         { "Mecklenburg-Vorpommern", (Color)new Color32(24, 255, 255, 255) },
         { "Niedersachsen", (Color)new Color32(100, 255, 218, 255) },
         { "Nordrhein-Westfalen", (Color)new Color32(0, 230, 118, 255) },
         { "Rheinland-Pfalz", (Color)new Color32(118, 255, 3, 255) },
         { "Saarland", (Color)new Color32(198, 255, 0, 255) },
         { "Sachsen", (Color)new Color32(255, 234, 0, 255) },
         { "Sachsen-Anhalt", (Color)new Color32(255, 196, 0, 255) },
         { "Schleswig-Holstein", (Color)new Color32(255, 145, 0, 255) },
         { "Thueringen", (Color)new Color32(255, 61, 0, 255) }
     };
    private object vis;

    void Start()
    {
        FacetBy(states);
    }

    private void FacetBy(List<string> states)
    {
        Double offset = 0.78;
        float z_spacing = 0.1f;
        Vector3 p = Axis.transform.position;
        Vector3 r = Axis.transform.localEulerAngles;
        Vector3 zAxisLabels = AxisLabels.transform.position;
        for (int i=0; i < states.Count; i++)
        {
            View view = Facet(data, states[i], "Place", (Color)stateColourValues[states[i]]);
            view.transform.position = new Vector3((float)(p.x-offset), (float)(p.y - offset), (i*p.z*z_spacing));
            Vector3 vector3 = new Vector3(r.x, r.y, r.z);
            view.transform.localEulerAngles = vector3;

            Transform labelz = Instantiate(labelTemplatez.transform);
            labelz.SetParent(AxisLabels.transform);
            labelz.gameObject.SetActive(true);
            labelz.position = new Vector3(zAxisLabels.x, (float)(zAxisLabels.y+1.22*offset), (i*(z_spacing-0.02f)));
            labelz.GetChild(0).GetComponent<TMP_Text>().text = states[i];
        }
    }



    delegate float[] Filter(float[] ar, CSVDataSource csvds, string fiteredValue, string filteringAttribute);


    View Facet(CSVDataSource csvds, string filteringValue, string filteringAttribute, Color color)
    {
       


        Filter baseFilter = (array, datasource, value, dimension) =>
        {
            return array.Select((b, i) => new { index = i, _base = b })
            .Where(b => datasource.getOriginalValuePrecise(csvds[dimension].Data[b.index], dimension).ToString() == value)
            .Select(b => b._base).ToArray();
        };
        

        Filter identity = (ar, ds, fv, fa) => { return ar; };
        // baseFilter = identity;

        var xData = baseFilter(csvds["Year"].Data, csvds, filteringValue, filteringAttribute);
        var yData = baseFilter(csvds["resultant"].Data, csvds, filteringValue, filteringAttribute);
        var zData = baseFilter(csvds["Place"].Data, csvds, filteringValue, filteringAttribute);
        ViewBuilder vb = new ViewBuilder(MeshTopology.Points, "Time Visualisation of Selected States").
            initialiseDataView(xData.Length).
            setDataDimension(xData, ViewBuilder.VIEW_DIMENSION.X).
            setDataDimension(yData, ViewBuilder.VIEW_DIMENSION.Y).
            setSize(baseFilter(csvds["resultant"].Data, csvds, filteringValue, filteringAttribute)).
            setColors(zData.Select(x => color).ToArray());

        Material mt = IATKUtil.GetMaterialFromTopology(AbstractVisualisation.GeometryType.Points);
        mt.SetFloat("_MinSize", 0.001f);
        mt.SetFloat("_MaxSize", 0.1f);

        return vb.updateView().apply(gameObject, mt);
    }
}

