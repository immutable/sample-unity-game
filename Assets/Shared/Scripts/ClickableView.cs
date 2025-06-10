using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HyperCasual.Runner
{
    public class ClickableView : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public delegate void OnClickAction();

        private bool m_IsDragging;

        private ScrollRect m_ParentScrollRect;

        private void Awake()
        {
            m_ParentScrollRect = GetComponentInParent<ScrollRect>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            m_IsDragging = true;
            m_ParentScrollRect?.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            m_ParentScrollRect?.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            m_IsDragging = false;
            m_ParentScrollRect?.OnEndDrag(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!m_IsDragging) OnClick?.Invoke();
        }

        public event OnClickAction OnClick;

        public void ClearAllSubscribers()
        {
            OnClick = null;
        }
    }
}