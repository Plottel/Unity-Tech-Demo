using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Deft.UI
{
    public class UIPanel : MonoBehaviour
    {
        public delegate void VisibilityChangedHandler(bool visible);
        public VisibilityChangedHandler eventVisibilityChanged;

        [HideInInspector]
        public Canvas canvas;

        private bool isVisible = true;
        public bool IsVisible
        {
            get => isVisible;
            set
            {
                if (isVisible != value)
                {
                    isVisible = value;
                    canvas.gameObject.SetActive(value);
                    eventVisibilityChanged?.Invoke(value);
                }
            }
        }        
    }
}
