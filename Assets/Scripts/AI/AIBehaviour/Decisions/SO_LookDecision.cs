using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName ="AI Behaviour/Decisions/Look")]
[System.Serializable]
public class SO_LookDecision : SO_Decision {

    public override bool Decide(MonsterController controller) {
        bool targetVisible = Look(controller);
        return targetVisible;
    }

    bool Look(MonsterController controller) {
        Collider2D[] colliders;

        colliders = Physics2D.OverlapCircleAll(controller.transform.position, controller.stats.lookShpereCastRadius);

        foreach(Collider2D collider in colliders) {
            if(collider.tag == "Player") {
                RaycastHit2D hitPlayer = Physics2D.Raycast(controller.transform.position, collider.transform.position - controller.transform.position, Mathf.Infinity, ~(1 << LayerMask.NameToLayer("Monster")));
                if(hitPlayer.collider.tag == "Player") {
                    Debug.DrawRay(controller.transform.position, collider.transform.position - controller.transform.position, Color.green);
                    controller.chaseTarget = collider.transform;
                    return true;
                } else {
                    Debug.DrawRay(controller.transform.position, collider.transform.position - controller.transform.position, Color.red);
                }
            }
        }

        return false;
    }
}
