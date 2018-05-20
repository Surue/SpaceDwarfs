using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI Behaviour/Actions/Attack")]
[System.Serializable]
public class SO_AttackAction : SO_Action {

    public override void Act(MonsterController controller) {
        Attack(controller);
    }

    void Attack(MonsterController controller) {
        Collider2D[] colliders;

        colliders = Physics2D.OverlapCircleAll(controller.transform.position, controller.stats.attackRange);

        foreach(Collider2D collider in colliders) {
            if(collider.tag == "Player") {
                RaycastHit2D hitPlayer = Physics2D.Raycast(controller.transform.position, collider.transform.position - controller.transform.position, Mathf.Infinity, ~(1 << LayerMask.NameToLayer("Monster")));
                if(hitPlayer.collider.tag == "Player") {
                    Debug.DrawRay(controller.transform.position, collider.transform.position - controller.transform.position, Color.green);

                    if(controller.CheckIfCountDownElapsed(controller.stats.attackRate)) {
                        controller.Attack(controller.stats.attackPoint);
                    }
                }
            }
        }
    }
}
