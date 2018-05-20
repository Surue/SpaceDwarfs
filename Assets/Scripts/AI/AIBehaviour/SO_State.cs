using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AI Behaviour/State")]
[System.Serializable]
public class SO_State : ScriptableObject {
    [SerializeField]
    public SO_Action[] actions;
    [SerializeField]
    public Transition[] transitions;

    public void UpdateState(MonsterController controller) {
        DoAction(controller);
        CheckTransition(controller);
    }

    void DoAction(MonsterController controller) {
        for(int i = 0; i < actions.Length;i++) {
            actions[i].Act(controller);
        }
    }

    void CheckTransition(MonsterController controller) {
        for(int i = 0;i < transitions.Length;i++) {
            bool decisionIsTrue = transitions[i].decision.Decide(controller);

            if(decisionIsTrue) {
                controller.TransitionToState(transitions[i].trueState);
            } else {
                controller.TransitionToState(transitions[i].falseState);
            }
        }
    }
}
