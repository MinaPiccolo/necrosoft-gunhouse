namespace Gunhouse
{
#if UNITY_PSP2 || UNITY_PS4
    public class Achievement
    {
        public const int ToothSome = 0;
        public const int HalfStep = 1;
        public const int Misdeeds = 2;
        public const int Rambunctious = 3;
        public const int Savior = 4;
        public const int BlueSkies = 5;
        public const int AnotherPeter = 6;
        public const int LikeMothsToFlame = 7;
        public const int Molytrols = 8;
        public const int TooManyGuns = 9;
    }
#elif PLAY_STORE
    public class Achievement
    {
        public const string ToothSome = GPGSIds.achievement_toothsome;
        public const string HalfStep = GPGSIds.achievement_halfstep;
        public const string Misdeeds = GPGSIds.achievement_misdeeds;
        public const string Rambunctious = GPGSIds.achievement_rambunctious;
        public const string Savior = GPGSIds.achievement_savior;
        public const string BlueSkies = GPGSIds.achievement_blue_skies;
        public const string AnotherPeter = GPGSIds.achievement_another_peter;
        public const string LikeMothsToFlame = GPGSIds.achievement_like_moths_to_flame;
        public const string Molytrols = GPGSIds.achievement_molytrois;
        public const string TooManyGuns = GPGSIds.achievement_too_many_guns;
    }
#elif APP_STORE
    public class Achievement
    {
        public const string ToothSome = "grp.achievement_toothsome";
        public const string HalfStep = "grp.achievement_half_step";
        public const string Misdeeds = "grp.achievement_misdeeds";
        public const string Rambunctious = "grp.achievement_rambunctious";
        public const string Savior = "grp.achievement_savior";
        public const string BlueSkies = "grp.achievement_blue_skies";
        public const string AnotherPeter = "grp.achievement_another_peter";
        public const string LikeMothsToFlame = "grp.achievement_like_moths_to_flame";
        public const string Molytrols = "grp.achievement_molytrois";
        public const string TooManyGuns = "grp.achievement_too_many_guns";
    }
#else
    public class Achievement
    {
        public const string ToothSome = "";
        public const string HalfStep = "";
        public const string Misdeeds = "";
        public const string Rambunctious = "";
        public const string Savior = "";
        public const string BlueSkies = "";
        public const string AnotherPeter = "";
        public const string LikeMothsToFlame = "";
        public const string Molytrols = "";
        public const string TooManyGuns = "";
    }
#endif
}