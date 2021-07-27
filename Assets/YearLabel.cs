using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DefaultNamespace;
using MoreLinq.Extensions;
using TMPro;
using UniRx;
using UnityEngine;

public class YearLabel : MonoBehaviour
{

    public float Year;

    [SerializeField] private TMP_Text label;
    [SerializeField] private ProximitySelectable selectable;

    private ViewModel _vm;

    public bool IsSelected =>  _vm.selectedYears.Value.Contains(Year);

    // Start is called before the first frame update
    void Start()
    {
        label.text = Year.ToString(CultureInfo.CurrentCulture);

        _vm = FindObjectOfType<ViewModel>();

        _vm.OnChanged.TakeUntilDestroy(this).Subscribe(_ =>
        {
            selectable.IsSelected = IsSelected;
        });

        selectable.OnSelect.TakeUntilDestroy(this).Subscribe(_ =>
        {
            var selectedYears = _vm.selectedYears.Value;

            if (IsSelected) selectedYears.Remove(Year);
            else selectedYears.Add(Year);

            _vm.selectedYears.OnNext(selectedYears);
        });
    }


}
