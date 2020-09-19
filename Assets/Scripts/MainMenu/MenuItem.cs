using System;
using UnityEngine;

namespace MainMenu
{
    public abstract class MenuItem : MonoBehaviour
    {
        public Transform m_transformProperty { get; set; }
        public abstract void Click();

        private void Awake()
        {
            m_transformProperty = transform;
        }
    }
}