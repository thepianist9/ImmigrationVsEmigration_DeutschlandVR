using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UniRx;
using UnityEngine;

public class StateFilterButton : MonoBehaviour
{

    public StateRepresentative stateRepresentative;
    private ViewModel _vm;
    private ProximitySelectable _selectable;

    private string State => stateRepresentative.stateString;
    public bool IsSelected => _vm.selectedStates.Value.Contains(State);

    // Start is called before the first frame update
    void Start()
    {
        _selectable = GetComponent<ProximitySelectable>();
        _vm = FindObjectOfType<ViewModel>();
        _vm.InitIfNecessary();

        _vm.OnChanged.TakeUntilDestroy(this).Subscribe(_ =>
        {
            _selectable.IsSelected = IsSelected;
        });

        _selectable.OnSelect.TakeUntilDestroy(this).Subscribe(_ =>
        {
            var selectedStates = _vm.selectedStates.Value;
            if (IsSelected) selectedStates.Remove(State);
            else selectedStates.Add(State);

            _vm.selectedStates.OnNext(selectedStates);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
