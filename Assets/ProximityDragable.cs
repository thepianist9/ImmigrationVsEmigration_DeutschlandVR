using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(ProximityInteractible))]
public class ProximityDragable : MonoBehaviour
{
    private ProximityInteractible _interactible;
    [SerializeField] private Transform controller;




    // Start is called before the first frame update
    void Start()
    {
        _interactible = GetComponent<ProximityInteractible>();
        _interactible.onGrabbed.TakeUntilDestroy(this).Subscribe(_ => StartDrag(controller));
    }

    void RotateAround(Vector3 pivotPoint, Quaternion rot)
    {
        transform.position = rot * (transform.position - pivotPoint) + pivotPoint;
        transform.rotation = rot * transform.rotation;
    }

    public void StartDrag(Transform controller) => StartCoroutine(DragCoroutine(controller));

    private IEnumerator DragCoroutine(Transform controller)
    {
        var offset = transform.position - controller.position;
        var rotationOffset = transform.rotation * controller.rotation;

        while (OVRInput.Get(OVRInput.Button.Two))
        {
            transform.position = controller.position + offset;
            transform.rotation = controller.rotation * Quaternion.Inverse(rotationOffset);
            // var targetRotation = controller.rotation * rotationOffset;
            // RotateAround(controller.position,
            //     targetRotation * Quaternion.Inverse(transform.rotation));
            yield return null;
        }
    }
}