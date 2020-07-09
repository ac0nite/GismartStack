using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI
{
    public class ClickDetect : MonoBehaviour, IPointerClickHandler
    {
        public event Action EventStartTapClick;
        public bool LockTapClick { get; private set; }

        public void Awake()
        {
            LockTap();
        }

        public void LockTap()
        {
            LockTapClick = true;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!LockTapClick)
            {
                Debug.Log($"OnPointerClick");
                EventStartTapClick?.Invoke();
            }
        }

        private void EndAnimation()
        {
            Debug.Log($"EndAnimation");
            LockTapClick = false;
        }
    }
}
