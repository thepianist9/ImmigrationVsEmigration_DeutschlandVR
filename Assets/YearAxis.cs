using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;

public class YearAxis : MonoBehaviour
{
    [SerializeField] private Transform labelContainer;

    private ViewModel vm;

    [SerializeField] private float height;

    [SerializeField] private YearLabel labelPrefab;

    public Vector3 GetPositionForYear(float year)
    {
        var offset = height / 20;
        var remainingHeight = height - offset;

        var i = vm.AvailableYears.IndexOf(year);
        var y = vm.AvailableYearsNormalized[i] * remainingHeight + offset;

        return transform.TransformPoint(new Vector3(0, y, 0));
    }

    // Start is called before the first frame update
    void Start()
    {
        vm = FindObjectOfType<ViewModel>();
        vm.InitIfNecessary();

        vm.AvailableYears.ForEach(year =>
        {
            var yearLabel = Instantiate(labelPrefab, GetPositionForYear(year), transform.rotation,
                labelContainer);
            yearLabel.transform.position = GetPositionForYear(year);
            yearLabel.Year = year;
        });
    }

    // Update is called once per frame
}