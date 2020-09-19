using UnityEngine;

namespace MainMenu
{
    public class QuitButton : Button
    {
        public override void Click()
        {
            if (!m_animating)
            {
                base.Click();

                m_animating = true;
                Animate();
                EventSystems.MainEventSystem.MainEvents.GameQuit();
                Application.Quit();
            }
        }
    }
}