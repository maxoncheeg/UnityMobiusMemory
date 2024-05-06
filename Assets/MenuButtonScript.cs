using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Views.Menu.Scripts
{
    public class MenuButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float _opacitySpeed = 0.01f;
        private bool _isHover = false;
        private Image _renderer;

        // Start is called before the first frame update
        void Start()
        {
            _renderer = GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_isHover && _renderer.color.a < 1)
            {
                    Color color = _renderer.color;
                    color.a += _opacitySpeed;
                    
                    if (color.a > 1)color.a = 1;
                    
                    _renderer.color = color;
            }
            else if (_renderer.color.a > 0)
            {
                Color color = _renderer.color;
                color.a -= _opacitySpeed;
                    
                if (color.a < 0)color.a = 0;
                    
                _renderer.color = color;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHover = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHover = false;
        }
    }
}