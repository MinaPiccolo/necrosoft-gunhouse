﻿using UnityEngine;
using InControl;

namespace Gunhouse
{
    public class PlayerInput : MonoBehaviour
    {
        PlayerActions actions;
        InControlInputModule inControlInputModule;

        public PlayerAction Left { get { return actions.Left; } }
        public PlayerAction Right { get { return actions.Right; } }
        public PlayerAction Up { get { return actions.Up; } }
        public PlayerAction Down { get { return actions.Down; } }
        public PlayerTwoAxisAction Direction { get { return actions.Direction; } }

        public PlayerAction Submit { get { return actions.Submit; } }
        public PlayerAction Start { get { return actions.Start; } }
        public PlayerAction Cancel { get { return actions.Cancel; } }
        public PlayerAction Escape { get { return actions.Escape; } }

        public bool AnyWasPressed { get { return actions.AnyWasPressed() || InputManager.ActiveDevice.AnyButtonWasPressed; } }
        public bool AnyIsPressed { get { return actions.AnyIsPressed() || InputManager.ActiveDevice.AnyButtonIsPressed; } }
        public Vector2 Move = Vector2.zero;

        void OnEnable()
        {
            actions = PlayerActions.CreateBindings();
            actions.Device = null;

            inControlInputModule = FindObjectOfType<InControlInputModule>();
            inControlInputModule.SubmitAction = actions.Submit;
            inControlInputModule.CancelAction = actions.Cancel;
            inControlInputModule.MoveAction = actions.Direction;
        }

        // void Update()
        void FixedUpdate()
        {
            Move = Vector2.zero;

            if (actions.Direction.X < -0.7f) Move.x = -1;
            if (actions.Direction.X >  0.7f) Move.x = 1;
            if (actions.Direction.Y < -0.5f) Move.y = -1;
            if (actions.Direction.Y >  0.5f) Move.y = 1;

            if (actions.Direction.Value.magnitude > 0.5f) {
                if (Mathf.Abs(actions.Direction.X) > Mathf.Abs(actions.Direction.Y)) {
                    if (actions.Direction.X > 0) { Move = new Vector2(1, 0); }
                    else { Move = new Vector2(-1, 0); }
                }
                else {
                    if (actions.Direction.Y > 0) { Move = new Vector2(0, -1); }
                    else { Move = new Vector2(0, 1); }
                }
            }
        }

        public void ClearInput() { actions.ClearInputState(); }
    }

    class PlayerActions : PlayerActionSet
    {
        public PlayerAction Left;
        public PlayerAction Right;
        public PlayerAction Up;
        public PlayerAction Down;
        public PlayerTwoAxisAction Direction;

        public PlayerAction Submit;
        public PlayerAction Start;
        public PlayerAction Cancel;
        public PlayerAction Escape;

        public PlayerAction AnyOther;

        public bool AnyWasPressed()
        {
            return Up.WasPressed || Down.WasPressed || Left.WasPressed ||
                   Right.WasPressed || Submit.WasPressed || Cancel.WasPressed ||
                   Start.WasPressed || Escape.WasPressed || AnyOther.WasPressed;
        }

        public bool AnyIsPressed()
        {
            return Up.IsPressed || Down.IsPressed || Left.IsPressed ||
                   Right.IsPressed || Submit.IsPressed || Cancel.IsPressed ||
                   Start.IsPressed || Escape.IsPressed || AnyOther.IsPressed;
        }

        public PlayerActions()
        {
            Left = CreatePlayerAction("Move Left");
            Right = CreatePlayerAction("Move Right");
            Up = CreatePlayerAction("Move Up");
            Down = CreatePlayerAction("Move Down");
            Direction = CreateTwoAxisPlayerAction(Left, Right, Down, Up);

            Submit = CreatePlayerAction("Submit");
            Cancel = CreatePlayerAction("Cancel");
            Start = CreatePlayerAction("Start");
            Escape = CreatePlayerAction("Escape");

            AnyOther = CreatePlayerAction("AnyOther");
        }

        public static PlayerActions CreateBindings()
        {
            PlayerActions actions = new PlayerActions();

            actions.Up.AddDefaultBinding(Key.UpArrow);
            actions.Down.AddDefaultBinding(Key.DownArrow);
            actions.Left.AddDefaultBinding(Key.LeftArrow);
            actions.Right.AddDefaultBinding(Key.RightArrow);

            actions.Up.AddDefaultBinding(InputControlType.LeftStickUp);
            actions.Down.AddDefaultBinding(InputControlType.LeftStickDown);
            actions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
            actions.Right.AddDefaultBinding(InputControlType.LeftStickRight);

            actions.Up.AddDefaultBinding(InputControlType.DPadUp);
            actions.Down.AddDefaultBinding(InputControlType.DPadDown);
            actions.Left.AddDefaultBinding(InputControlType.DPadLeft);
            actions.Right.AddDefaultBinding(InputControlType.DPadRight);

            actions.Start.AddDefaultBinding(InputControlType.Start);
            actions.Start.AddDefaultBinding(InputControlType.Command);

            actions.AnyOther.AddDefaultBinding(InputControlType.LeftBumper);
            actions.AnyOther.AddDefaultBinding(InputControlType.RightBumper);
            actions.AnyOther.AddDefaultBinding(InputControlType.LeftTrigger);
            actions.AnyOther.AddDefaultBinding(InputControlType.RightTrigger);

            #if UNITY_SWITCH
            actions.Start.AddDefaultBinding(InputControlType.Plus);
            actions.Start.AddDefaultBinding(InputControlType.Minus);
            actions.Submit.AddDefaultBinding(InputControlType.Action2);
            actions.Cancel.AddDefaultBinding(InputControlType.Action1);
            #else
            actions.Submit.AddDefaultBinding(Key.Return);
            actions.Submit.AddDefaultBinding(Key.Space);
            actions.Submit.AddDefaultBinding(InputControlType.Action1);

            actions.Cancel.AddDefaultBinding(Key.Backspace);
            actions.Cancel.AddDefaultBinding(InputControlType.Action2);
            #endif

            #if UNITY_WEBGL
            actions.Escape.AddDefaultBinding(Key.Q);
            #else
            actions.Escape.AddDefaultBinding(Key.Escape);
            #endif

            #if UNITY_TVOS
            InputManager.OnDeviceAttached += delegate(InputDevice inputDevice) {
                if (inputDevice.DeviceClass == InputDeviceClass.Remote) {
                    Remote.reportAbsoluteDpadValues = true;
                    Remote.touchesEnabled = false;
                    inputDevice.LeftStick.LowerDeadZone = 0.5f;  // Default is usually 0.2f
                    inputDevice.LeftStick.StateThreshold = 0.5f; // Default is usually 0.0f
                }
            };
            #endif

            //actions.ListenOptions.OnBindingFound = (action, binding) =>
            //{
            //    if (binding == new KeyBindingSource(Key.Escape)) {
            //        action.StopListeningForBinding();
            //        return false;
            //    }
            //    return true;
            //};

            return actions;
        }
    }
}