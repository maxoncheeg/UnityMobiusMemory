using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Resources.Prefabs
{
    public class card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Animator _animator;

        private static readonly int IsHover = Animator.StringToHash("IsHover");

        // Start is called before the first frame update
        void Start()
        {
            _animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _animator.SetBool(IsHover, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _animator.SetBool(IsHover, false);
        }
    }
}