using UnityEngine;
using InControl;

namespace Gunhouse
{
    public class PlayerInput : MonoBehaviour
    {
        [HideInInspector] public PlayerActions pad;

        void Awake()
        {
            pad = PlayerActions.CreateBindings();
            pad.Device = null;
        }

        void FixedUpdate()
        {
            pad.Move = Vector2.zero;

            if (pad.Direction.X < -0.5f) pad.Move.x = -1;
            if (pad.Direction.X >  0.5f) pad.Move.x = 1;
            if (pad.Direction.Y < -0.5f) pad.Move.y = -1;
            if (pad.Direction.Y >  0.5f) pad.Move.y = 1;

            if (pad.Direction.Value.magnitude > 0.5f) {
                if (Mathf.Abs(pad.Direction.X) > Mathf.Abs(pad.Direction.Y))
                {
                    if (pad.Direction.X > 0) pad.Move = new Vector2(1, 0);
                    else pad.Move = new Vector2(-1, 0);
                }
                else
                {
                    if (pad.Direction.Y > 0) pad.Move = new Vector2(0, -1);
                    else pad.Move = new Vector2(0, 1);
                }
            }
        }
    }
}