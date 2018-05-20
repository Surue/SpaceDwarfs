using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class SO_Action : ScriptableObject {

    public abstract void Act(MonsterController controller);
}
