﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class CustomButton : Button
    {
        // Event delegates triggered on click.
        [FormerlySerializedAs("onReleased")]
        [SerializeField]
        private UnityEvent m_OnReleased = new ButtonClickedEvent();
        public UnityEvent onReleased
        {
            get => m_OnReleased;
            set => m_OnReleased = value;
        }
        
        private void Released()
        {
            if (!IsActive() || !IsInteractable()) return;

            UISystemProfilerApi.AddMarker("Button.onReleased", this);
            m_OnReleased.Invoke();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            Released();
        }
    }
}