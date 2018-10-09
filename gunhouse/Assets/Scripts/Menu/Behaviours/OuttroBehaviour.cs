using UnityEngine;

namespace Gunhouse.Menu
{
    public class OuttroBehaviour : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //Debug.Log("Outtro, OnStateEnter");
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //Debug.Log("Outtro, OnStateUpdate");
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //Debug.Log("Outtro, OnStateExit");
        }
    }
}