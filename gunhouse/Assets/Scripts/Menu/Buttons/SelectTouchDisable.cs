using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Gunhouse.Menu;

[RequireComponent(typeof(Selectable))]
public class SelectTouchDisable : MonoBehaviour, IPointerClickHandler,
                                  IPointerDownHandler, IPointerUpHandler,
                                  IPointerEnterHandler, IDeselectHandler
{
    [SerializeField] bool enableButton;
    static Button[] selectables;

    void OnEnable() { selectables = transform.parent.GetComponentsInChildren<Button>(); SetActiveButton(true); }
    public void OnPointerClick(PointerEventData eventData) { if (enableButton) { return; } SetActiveButton(false); }
    public void OnPointerDown(PointerEventData eventData) { SetActiveButton(false); }
    public void OnPointerUp(PointerEventData eventData) { SetActiveButton(true); }
    public void OnDeselect(BaseEventData eventData) { GetComponent<Selectable>().OnPointerExit(null); }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MainMenu.ignoreFocus = true;

        if (!EventSystem.current.alreadySelecting) EventSystem.current.SetSelectedGameObject(gameObject);
    }

    void SetActiveButton(bool active)
    {
        for (int i = 0; i < selectables.Length; ++i) {
            if (selectables[i].gameObject == gameObject) continue;
            selectables[i].interactable = active;
        }
    }
}