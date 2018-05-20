using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Transition {
    [SerializeField]
    public SO_Decision decision;
    [SerializeField]
    public SO_State trueState;
    [SerializeField]
    public SO_State falseState;
}
