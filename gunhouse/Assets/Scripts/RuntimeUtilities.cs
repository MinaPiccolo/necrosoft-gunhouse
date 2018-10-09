using UnityEngine;

namespace DemonSchool
{
    public static class RuntimeUtilities
    {
        #region Textures

        static Texture2D whiteTexture;
        public static Texture2D WhiteTexture
        {
            get
            {
                if (whiteTexture != null) return whiteTexture;

                whiteTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false) { name = "White Texture" };
                whiteTexture.SetPixel(0, 0, Color.white);
                whiteTexture.Apply();

                return whiteTexture;
            }
        }

        static Texture3D whiteTexture3D;
        public static Texture3D WhiteTexture3D
        {
            get
            {
                if (whiteTexture3D != null) return whiteTexture3D;

                whiteTexture3D = new Texture3D(1, 1, 1, TextureFormat.ARGB32, false) { name = "White Texture 3D" };
                whiteTexture3D.SetPixels(new Color[] { Color.white });
                whiteTexture3D.Apply();

                return whiteTexture3D;
            }
        }

        static Texture2D blackTexture;
        public static Texture2D BlackTexture
        {
            get
            {
                if (blackTexture != null) return blackTexture;

                blackTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false) { name = "Black Texture" };
                blackTexture.SetPixel(0, 0, Color.black);
                blackTexture.Apply();

                return blackTexture;
            }
        }

        static Texture3D blackTexture3D;
        public static Texture3D BlackTexture3D
        {
            get
            {
                if (blackTexture3D != null) return blackTexture3D;

                blackTexture3D = new Texture3D(1, 1, 1, TextureFormat.ARGB32, false) { name = "Black Texture 3D" };
                blackTexture3D.SetPixels(new Color[] { Color.black });
                blackTexture3D.Apply();

                return blackTexture3D;
            }
        }

        static Texture2D transparentTexture;
        public static Texture2D TransparentTexture
        {
            get
            {
                if (transparentTexture != null) return transparentTexture;

                transparentTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false) { name = "Transparent Texture" };
                transparentTexture.SetPixel(0, 0, Color.clear);
                transparentTexture.Apply();

                return transparentTexture;
            }
        }

        static Texture3D transparentTexture3D;
        public static Texture3D TransparentTexture3D
        {
            get
            {
                if (transparentTexture3D != null) return transparentTexture3D;

                transparentTexture3D = new Texture3D(1, 1, 1, TextureFormat.ARGB32, false) { name = "Transparent Texture 3D" };
                transparentTexture3D.SetPixels(new Color[] { Color.clear });
                transparentTexture3D.Apply();

                return transparentTexture3D;
            }
        }

        #endregion

        #region Sprites

        static Sprite whiteSprite;
        public static Sprite WhiteSprite
        {
            get
            {
                if (whiteSprite != null) return whiteSprite;

                whiteSprite = Sprite.Create(WhiteTexture,
                                            new Rect(0.0f, 0.0f, WhiteTexture.width, WhiteTexture.height),
                                            new Vector2(0.5f, 0.5f), 1);

                return whiteSprite;
            }
        }

        static Sprite blackSprite;
        public static Sprite BlackSprite
        {
            get
            {
                if (blackSprite != null) return blackSprite;

                blackSprite = Sprite.Create(BlackTexture,
                                            new Rect(0.0f, 0.0f, BlackTexture.width, BlackTexture.height),
                                            new Vector2(0.5f, 0.5f), 1);

                return blackSprite;
            }
        }

        static Sprite transparentSprite;
        public static Sprite TransparentSprite
        {
            get
            {
                if (transparentSprite != null) return transparentSprite;

                transparentSprite = Sprite.Create(TransparentTexture,
                                                  new Rect(0.0f, 0.0f, TransparentTexture.width, TransparentTexture.height),
                                                  new Vector2(0.5f, 0.5f), 1);



                return transparentSprite;
            }
        }

        #endregion
    }
}