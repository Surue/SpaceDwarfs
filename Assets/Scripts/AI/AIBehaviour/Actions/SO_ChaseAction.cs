using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AI Behaviour/Actions/Chase")]
[System.Serializable]
public class SO_ChaseAction : SO_Action {

    public override void Act(MonsterController controller) {
        Chase(controller);
    }

    void Chase(MonsterController controller) {
        if(controller.path.Count == 0) {
            controller.path = controller.aStart.GetPathFromTo(controller.transform.position, controller.chaseTarget.position);
        }

        Vector2 direction = controller.path[0] - (Vector2)controller.transform.position;

        direction.Normalize();

        controller.body.velocity = direction * controller.stats.speed;

        if(Vector2.Distance(controller.transform.position, controller.path[0]) < controller.stats.stopingDistance) {
            controller.path.RemoveAt(0);
            controller.path = controller.aStart.GetPathFromTo(controller.transform.position, controller.chaseTarget.position);
        }
        
    }
}
