using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum UI_Element_Type {buttonElement, selectorElement, sliderElement, keybinderElement}

public class UI_Element : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
	public UI_Element_Type elementType;
	public GameObject outlineObject;
	public UI_Element elementAbove;
	public UI_Element elementBelow;
	[Space(7f)]
	public UnityEvent ClickEvent;
	public UnityEvent PositiveEvent;
	public UnityEvent NegativeEvent;

	//Callback function for the mouse entering the UI element.
	public void OnPointerEnter(PointerEventData eventData)
	{
		UI_Manager.Instance.SetSelectedElement(this);
	}

	//Callback function for the mouse clicking the UI element.
	public void OnPointerClick(PointerEventData eventData)
	{
		ClickEvent.Invoke();
	}
}
