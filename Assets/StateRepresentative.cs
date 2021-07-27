using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateRepresentative : MonoBehaviour
{
    public FederalState state = FederalState.Insgesamt;

    public string stateString => state.toString();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}