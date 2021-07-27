using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ProximityInteractionManager : MonoBehaviour
{
    private List<ProximityInteractible> _interactibles = new List<ProximityInteractible>() { };
    [SerializeField] private Transform controller;

    private ProximityInteractible _activeInteractible;

    public void Register(ProximityInteractible interactible)
    {
        _interactibles.Add(interactible);
    }

    // Update is called once per frame
    void Update()
    {
        ProximityInteractible closestInteractible = null;
        float closestDistance = float.PositiveInfinity;

        foreach (var interactible in _interactibles)
        {
            var distance = Vector3.Distance(controller.position, interactible.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractible = interactible;
            }
        }

        if (!closestInteractible) return;

        if (_activeInteractible != closestInteractible)
        {
            if (_activeInteractible) _activeInteractible.isHovered.OnNext(false);
            closestInteractible.isHovered.OnNext(true);
            _activeInteractible = closestInteractible;
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            _activeInteractible.onClicked.OnNext(new Unit());
        }
    }
}