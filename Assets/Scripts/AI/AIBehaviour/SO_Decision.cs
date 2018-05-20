using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class SO_Decision : ScriptableObject {

    public abstract bool Decide(StateController controller);
}
