using System.Collections;
using System.Collections.Generic;
using InfoVisView;
using UniRx;
using UnityEngine;


public class ProximitySelectable : MonoBehaviour
{
    private Renderer _renderer;

    public Subject<Unit> OnSelect = new Subject<Unit>();

    private bool _isSelected;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            UpdateMaterialColor();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        var interactible = GetComponent<ProximityInteractible>();
        interactible.onClicked.TakeUntilDestroy(this).Subscribe(OnSelect);

        UpdateMaterialColor();
    }

    void UpdateMaterialColor()
    {
        if (!_renderer) return;

        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        _renderer.GetPropertyBlock(propBlock);
        var color = _isSelected ? Style.selectionColor : Color.black;
        propBlock.SetColor("_Color", color);
        propBlock.SetColor("_EmissionColor", color);
        _renderer.SetPropertyBlock(propBlock);
    }
}