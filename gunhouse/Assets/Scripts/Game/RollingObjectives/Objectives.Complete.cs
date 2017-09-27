using UnityEngine;
using Necrosoft;
using Necrosoft.ThirdParty;

namespace Gunhouse
{
    public partial class Objectives : MonoBehaviour
    {
        void SetTaskComplete(int index)
        {
            ticks[index].enabled = true;
            ticks[index].transform.ScaleXY(Vector2.zero);
            LeanTween.scale(ticks[index].gameObject, Vector3.one, 0.2f).setEase(tickCurve);

            int cashToGive = (int)MetaState.logCurve(DataStorage.StartOnWave, 400, 500, 1000);
            DataStorage.Money += cashToGive;
            cash[index].enabled = true;
            cash[index].text = "+$" + cashToGive;

            LeanTween.moveLocalY(cash[index].gameObject, 50, 2).setEase(cashCurve);

            Color textColor = cash[index].color;
            LeanTween.value(gameObject, 1, 0, 2).setOnUpdate((float value) => {
                textColor.a = value;
                cash[index].color = textColor;
            });

            LeanTween.delayedCall(2, () =>{ TextBlock.Wipe(tasks[index],
                ()=> {
                    ticks[index].enabled = false;
                    cash[index].transform.TranslateY(-50);
                    cash[index].enabled = false;
                    LeanTween.delayedCall(0.5f, () => { RequestTask(index); });
                });
            });
        }
    }
}