using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ProximityInteractible : MonoBehaviour
{
    public readonly BehaviorSubject<bool> isHovered = new BehaviorSubject<bool>(false);
    public readonly Subject<Unit> onClicked = new Subject<Unit>();
    public readonly Subject<Unit> onDraggedOver = new Subject<Unit>();

    public void Start()
    {
        FindObjectOfType<ProximityInteractionManager>().Register(this);
    }
}
