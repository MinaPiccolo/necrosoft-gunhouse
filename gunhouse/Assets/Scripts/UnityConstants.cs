namespace UnityConstants
{
    public static class Tags
    {
        public const string Untagged = "Untagged";
        public const string Respawn = "Respawn";
        public const string Finish = "Finish";
        public const string EditorOnly = "EditorOnly";
        public const string MainCamera = "MainCamera";
        public const string Player = "Player";
        public const string GameController = "GameController";
    }

    public static class SortingLayers
    {
        public const int Default = 0;
        public const int Menu = -609691605;
    }

    public static class Layers
    {
        public const int Default = 0;
        public const int TransparentFX = 1;
        public const int Ignore_Raycast = 2;
        public const int Water = 4;
        public const int UI = 5;
        public const int Menu = 10;
    }

    public static class Masks
    {
        public const int Default = 1 << 0;
        public const int TransparentFX = 1 << 1;
        public const int Ignore_Raycast = 1 << 2;
        public const int Water = 1 << 4;
        public const int UI = 1 << 5;
        public const int Menu = 1 << 10;
    }

    public static class Scenes
    {
        public const int Game = 0;
    }
}
