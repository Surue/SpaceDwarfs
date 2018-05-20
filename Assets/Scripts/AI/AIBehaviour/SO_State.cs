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

    public void UpdateState(StateController controller) {
        DoAction(controller);
        CheckTransition(controller);
    }

    void DoAction(StateController controller) {
        for(int i = 0; i < actions.Length;i++) {
            actions[i].Act(controller);
        }
    }

    void CheckTransition(StateController controller) {
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
