using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI Behaviour/Actions/Flee")]
[System.Serializable]
public class SO_FleeAction : SO_Action {

    public override void Act(MonsterController controller) {
        Flee(controller);
    }

    void Flee(MonsterController controller) {
        Vector2 direction = controller.transform.position - controller.chaseTarget.transform.position;

        direction.Normalize();

        controller.body.velocity = direction * controller.stats.speed;
    }
}
