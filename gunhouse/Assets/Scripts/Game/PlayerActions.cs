using UnityEngine;
using InControl;

namespace Gunhouse
{
    public class PlayerActions : PlayerActionSet
    {
        public PlayerAction Left;
        public PlayerAction Right;
        public PlayerAction Up;
        public PlayerAction Down;
        public PlayerTwoAxisAction Direction;
        public Vector2 Move;

        public PlayerAction X;
        public PlayerAction Y;

        public PlayerAction Submit;
        public PlayerAction Cancel;
        public PlayerAction Start;
        public PlayerAction Escape;

        public ControllerButton Submit_Button = ControllerButton.PS_X;
        public ControllerButton Cancel_Button = ControllerButton.PS_CIRCLE;
        public ControllerButton Alt_Button = ControllerButton.PS_TRIANGLE;

        public bool AnyWasPressd()
        {
            return Submit.WasPressed || Cancel.WasPressed || X.WasPressed || Y.WasPressed ||
                   Up.WasPressed || Down.WasPressed || Left.WasPressed || Right.WasPressed ||
                   Start.WasPressed || Submit.WasPressed || Cancel.WasPressed;
        }

        public PlayerActions()
        {
            Left = CreatePlayerAction("Move Left");
            Right = CreatePlayerAction("Move Right");
            Up = CreatePlayerAction("Move Up");
            Down = CreatePlayerAction("Move Down");
            Direction = CreateTwoAxisPlayerAction(Left, Right, Down, Up);

            X = CreatePlayerAction("X");
            Y = CreatePlayerAction("Y");

            Submit = CreatePlayerAction("Submit");
            Cancel = CreatePlayerAction("Cancel");
            Start = CreatePlayerAction("Start");
            Escape = CreatePlayerAction("Escape");
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

            actions.X.AddDefaultBinding(InputControlType.Action3);
            actions.Y.AddDefaultBinding(InputControlType.Action4);

            actions.Submit.AddDefaultBinding(Key.Return);
            actions.Submit.AddDefaultBinding(Key.Space);
            actions.Submit.AddDefaultBinding(InputControlType.Action1);

            actions.Cancel.AddDefaultBinding(Key.Backspace);
            actions.Cancel.AddDefaultBinding(InputControlType.Action2);

            actions.Start.AddDefaultBinding(InputControlType.Start);
            actions.Start.AddDefaultBinding(InputControlType.Command);

            #if UNITY_WEBGL
            actions.Escape.AddDefaultBinding(Key.Q);
            #else
            actions.Escape.AddDefaultBinding(Key.Escape);
            #endif

            return actions;
        }
    }
}
