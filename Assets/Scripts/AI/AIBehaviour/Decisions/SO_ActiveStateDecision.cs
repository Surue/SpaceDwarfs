using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI Behaviour/Decisions/ActiveState")]
[System.Serializable]
public class SO_ActiveStateDecision : SO_Decision {

    public override bool Decide(StateController controller) {
        bool chaseTargetIsActive = controller.chaseTarget.gameObject.activeSelf;
        return chaseTargetIsActive;
    }
    
}
