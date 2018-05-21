using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI Behaviour/Actions/Explore")]
[System.Serializable]
public class SO_ExploreAction:SO_Action {

    public override void Act(MonsterController controller) {
        Explore(controller);
    }

    void Explore(MonsterController controller) {
        if(controller.wayPointList.Count == 0) {
            controller.wayPointList.Add(FindObjectOfType<MapController>().GetPatrolPoint());

            if(controller.wayPointList.Count != 0) {
                controller.path = controller.aStart.GetPathFromTo(controller.transform.position, controller.wayPointList[0]);
            }
        } else {
            Vector2 direction = controller.path[0] - (Vector2)controller.transform.position;

            direction.Normalize();

            controller.body.velocity = direction * controller.stats.speed;

            if(Vector2.Distance(controller.transform.position, controller.path[0]) < controller.stats.stopingDistance) {
                controller.path.RemoveAt(0);

                if(controller.path.Count == 0) {
                    controller.wayPointList.RemoveAt(0);
                }
            }
        }
    }
}
