using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI Behaviour/Actions/Go To Nest")]
[System.Serializable]
public class SO_GoToNestACtion : SO_Action {

    public override void Act(MonsterController controller) {
        GoBackToNest(controller);
    }

    public void GoBackToNest(MonsterController controller) {
        if(controller.path == null || controller.path.Count == 0) {
            controller.wayPointList = new List<Vector2>();
            controller.wayPointList.Add(FindObjectOfType<MapController>().GetNestPosition());

            controller.path = controller.aStart.GetPathFromTo(controller.transform.position, controller.wayPointList[0]);
        } else {
            if(controller.path == null || controller.path.Count == 0) {
                controller.path = controller.aStart.GetPathFromTo(controller.transform.position, controller.wayPointList[0]);
            }

            Vector2 direction = controller.path[0] - (Vector2)controller.transform.position;

            direction.Normalize();

            controller.body.velocity = direction * controller.stats.speed;

            if(Vector2.Distance(controller.transform.position, controller.path[0]) < controller.stats.stopingDistance) {
                controller.path.RemoveAt(0);

                if(Vector2.Distance(controller.transform.position, controller.wayPointList[0]) < controller.stats.stopingDistance) {
                    FindObjectOfType<MonsterManager>().PlayerFounded();
                    Destroy(controller.gameObject);
                }
            }
        }
    }
}
