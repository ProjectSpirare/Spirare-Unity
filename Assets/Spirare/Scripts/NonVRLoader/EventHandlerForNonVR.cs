using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Spirare
{
    public class EventHandlerForNonVR : MonoBehaviour, IPointerClickHandler
    {
        public event Action OnSelect;
        public event Action OnUse;
        public event Action OnEquip;

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log(eventData.button);

            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    OnUse?.Invoke();
                    break;
            }
            eventData.Use();
        }
    }
}
