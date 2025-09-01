using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoverEvent : MonoBehaviour, IPointerEnterHandler
{
    public Button theButton;
    private float timeCount;

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.instance.Play("MenuHover");
    }
}
