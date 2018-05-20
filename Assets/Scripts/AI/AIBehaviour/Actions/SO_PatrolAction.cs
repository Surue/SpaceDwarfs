using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AI Behaviour/Actions/Patrol")]
[System.Serializable]
public class SO_PatrolAction : SO_Action {

    public override void Act(StateController controller) {
        Patrol(controller);
    }

    private void Patrol(StateController controller) {
        if(controller.wayPointList.Count == 0) {
            controller.wayPointList = FindObjectOfType<MapController>().GetPatrolsPointForRegion(controller.transform);
            if(controller.wayPointList.Count != 0) {
                controller.path = controller.aStart.GetPathFromTo(controller.transform.position, controller.wayPointList[controller.nextWayPoint]);
                //controller.path.Add(controller.wayPointList[controller.nextWayPoint] + new Vector2(0.5f, 0.5f));
            }
        } else {

            Vector2 direction = controller.path[0] - (Vector2)controller.transform.position;

            direction.Normalize();

            controller.body.velocity = direction * controller.stats.speed;

            if(Vector2.Distance(controller.transform.position, controller.path[0]) < controller.stats.stopingDistance) {
                controller.path.RemoveAt(0);

                if(Vector2.Distance(controller.transform.position, controller.wayPointList[controller.nextWayPoint]) < controller.stats.stopingDistance) {
                    controller.nextWayPoint = (controller.nextWayPoint + 1) % controller.wayPointList.Count;
                    controller.path = controller.aStart.GetPathFromTo(controller.transform.position, controller.wayPointList[controller.nextWayPoint]);
                }
            }
        }
    }
}
