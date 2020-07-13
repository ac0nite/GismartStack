using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI
{
    public class ClickDetect : MonoBehaviour, IPointerClickHandler
    {
        public event Action EventEndScreen;
        public event Action EventEndLoadScreen;
        private Animator _animator = null;
        private bool _lockTapClick = true;

        public void Awake()
        {
            _animator = GetComponent<Animator>();
            LockTap();
            GameController.Instance.EventChangeRecord += OnRestart;
        }

        private void OnDestroy()
        {
            GameController.Instance.EventChangeRecord -= OnRestart;
        }

        public void LockTap()
        {
            _lockTapClick = true;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_lockTapClick)
            {
                _animator.SetTrigger("End");
                Debug.Log($"OnPointerClick");
                LockTap();
            }
        }

        private void EndScreenAnimation()
        {
            Debug.Log($"EndAnimation");
            _lockTapClick = false;
            EventEndLoadScreen?.Invoke();
        }

        private void EndAnimation()
        {
            EventEndScreen?.Invoke();
        }

        private void OnRestart()
        {
            _animator.SetTrigger("Start");
        }
    }
}
