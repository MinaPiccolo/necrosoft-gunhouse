using UnityEngine;
using System;
using TMPro;
using Necrosoft.ThirdParty;

namespace Gunhouse
{
    public class MatchBonus : MonoBehaviour
    {
        GameObject root;
        TextMeshProUGUI[] texts;
        float fontSize = 58;

        void Awake()
        {
            root = transform.GetChild(0).gameObject;
            texts = GetComponentsInChildren<TextMeshProUGUI>(true);
            root.SetActive(false);
        }

        public void ResumeAnimations()
        {
            for (int i = 0; i < texts.Length; ++i) {
                if (!texts[i].gameObject.activeInHierarchy) continue;
                LeanTween.resume(texts[i].gameObject);
            }
        }

        public void PauseAnimations()
        {
            for (int i = 0; i < texts.Length; ++i) {
                if (!texts[i].gameObject.activeInHierarchy) continue;
                LeanTween.pause(texts[i].gameObject);
            }
        }

        public void DismissAnimations()
        {
            for (int i = 0; i < texts.Length; ++i) {
                if (!texts[i].gameObject.activeInHierarchy) continue;
                LeanTween.cancel(texts[i].gameObject);
                texts[i].gameObject.SetActive(false);
            }
        }

        public void SpawnFloatyText(int amount)
        {
            root.SetActive(true);

            for (int i = 0; i < texts.Length; ++i) {
                if (texts[i].gameObject.activeInHierarchy) continue;

                texts[i].gameObject.SetActive(true);
                AppMain.MainMenu.builder.Length = 0;
                texts[i].fontSize = fontSize;
                texts[i].SetText(AppMain.MainMenu.builder.AppendFormat("Match! x{0}", amount));
                texts[i].color = Color.white;

                texts[i].alignment = TextAlignmentOptions.Center;
                texts[i].rectTransform.pivot = new Vector2(0.5f, 0.5f);
                texts[i].rectTransform.SetXY(340, 450);

                LeanTween.moveLocalY(texts[i].gameObject, 540, 3).setEase(LeanTweenType.easeInQuad);
                LeanTween.value(gameObject, (Action<float, object>)ChangeAlpha, 1, 0, 5)
                         .setOnUpdateParam(i as object)
                         .setOnComplete(() => { texts[i].gameObject.SetActive(false); });

                break;
            }
        }

        public void SpawnFloatyText(int amount, float multiplier,
                                    Vector2 position, float size,
                                    Gun.Ammo ammo)
        {
            root.SetActive(true);

            for (int i = 0; i < texts.Length; ++i) {
                if (texts[i].gameObject.activeInHierarchy) continue;

                texts[i].gameObject.SetActive(true);

                AppMain.MainMenu.builder.Length = 0;
                AppMain.MainMenu.builder.AppendFormat("+{0}", amount);
                if (multiplier > 1) { AppMain.MainMenu.builder.AppendFormat("x{0}", multiplier); }

                texts[i].fontSize = fontSize * size * 0.6f;
                texts[i].SetText(AppMain.MainMenu.builder);
                texts[i].color = AmmoColor(ammo);

                /* calculate position from old point system */
                {
                    if (position.x > 270) {
                        texts[i].alignment = TextAlignmentOptions.Left;
                        texts[i].rectTransform.pivot = new Vector2(0, 0.5f);
                        position.x = 410;
                    }
                    else if (position.x > 70) {
                        texts[i].alignment = TextAlignmentOptions.Right;
                        texts[i].rectTransform.pivot = new Vector2(1, 0.5f);
                        position.x = 930;
                    }

                    if (position.y > 400) { position.y = -250; }
                    else if (position.y > 270) { position.y = 10; }
                    else { position.y = 250; }
                }

                texts[i].rectTransform.SetXY(position.x, position.y);

                LeanTween.moveLocalY(texts[i].gameObject, position.y + 100, 5).setEase(LeanTweenType.easeOutSine);
                LeanTween.value(gameObject, (Action<float, object>)ChangeAlpha, 1, 0, 5)
                         .setOnUpdateParam(i as object)
                         .setOnComplete(() => { texts[i].gameObject.SetActive(false); });
                break;
            }
        }

        void ChangeAlpha(float val, object obj) { texts[(int)obj].alpha = val; }

        Color AmmoColor(Gun.Ammo ammo)
        {
            switch (ammo)
            {
                case Gun.Ammo.DRAGON:
                case Gun.Ammo.FLAME: return new Color(1.0f, 0.7f, 0.1f, 1);
                case Gun.Ammo.IGLOO:
                case Gun.Ammo.FORK: return new Color(0.1f, 0.5f, 1.0f, 1);
                case Gun.Ammo.SKULL:
                case Gun.Ammo.BOUNCE: return new Color(0.8f, 0.2f, 0.7f, 1);
                case Gun.Ammo.VEGETABLE:
                case Gun.Ammo.BOOMERANG: return new Color(0.4f, 0.8f, 0.3f, 1);
                case Gun.Ammo.LIGHTNING:
                case Gun.Ammo.SIN: return new Color(1.0f, 0.3f, 0.3f, 1);
            }

            return Color.white;
        }
    }
}