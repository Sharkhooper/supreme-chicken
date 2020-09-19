using System;
using DG.Tweening;
using UnityEngine;

namespace MainMenu
{
    public class Button : MenuItem
    {
        protected bool m_animating;

        private void Start()
        {
            m_transformProperty = transform;
        }

        public override void Click()
        {
            m_animating = true;
            Animate();
        }


        protected void Animate()
        {
            Vector3 backPos = transform.localPosition - new Vector3(0.1f,0,0);
            Vector3 fowPos = transform.localPosition;
        
            transform.DOLocalMove(backPos, 0.6f)
                .onComplete += () =>
                transform.DOLocalMove(fowPos, 0.6f)
                    .onComplete += () => m_animating = false;
        }
        
    }
}