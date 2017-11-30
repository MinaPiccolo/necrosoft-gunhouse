using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Gunhouse.Menu;

[RequireComponent(typeof(Selectable))]
public class SelectTouchDisable : MonoBehaviour, IPointerClickHandler,
                                  IPointerDownHandler, IPointerUpHandler,
                                  IPointerEnterHandler, IDeselectHandler
{
    static Button[] selectables;
    static Color highlightColor = new Color(1, 0.9f, 0.24f);
    static Color pressedColor = new Color(1, 0.9f, 0.62f);

    void OnEnable() { selectables = transform.parent.GetComponentsInChildren<Button>(); SetActiveButton(true); }
    public void OnPointerClick(PointerEventData eventData) { SetActiveButton(false); }
    public void OnPointerDown(PointerEventData eventData) { SetActiveButton(false); }
    public void OnPointerUp(PointerEventData eventData) { SetActiveButton(true); }
    public void OnDeselect(BaseEventData eventData) { GetComponent<Selectable>().OnPointerExit(null); }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MainMenu.ignoreFocus = true;
        SetButtonHighlight(true);

        if (!EventSystem.current.alreadySelecting) EventSystem.current.SetSelectedGameObject(gameObject);
    }

    void SetActiveButton(bool active)
    {
        for (int i = 0; i < selectables.Length; ++i) {
            if (selectables[i].gameObject == gameObject) continue;
            selectables[i].interactable = active;
        }
    }

    public static void SetButtonHighlight(bool white)
    {
        if (selectables == null || selectables.Length == 0) { return; }

        ColorBlock colors = selectables[0].colors;
        colors.highlightedColor = white ? Color.white : highlightColor;
        colors.pressedColor = white ? highlightColor : pressedColor;

        for (int i = 0; i < selectables.Length; ++i) { selectables[i].colors = colors; }
    }
}