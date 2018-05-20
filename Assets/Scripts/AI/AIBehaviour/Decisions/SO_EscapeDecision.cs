using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI Behaviour/Decisions/Escape")]
[System.Serializable]
public class SO_EscapeDecision :SO_Decision {

    public override bool Decide(MonsterController controller) {
        return Vector3.Distance(controller.transform.position, controller.chaseTarget.position) > controller.stats.lookShpereCastRadius + (0.5f * controller.stats.lookShpereCastRadius);
    }
}
