using UnityEngine;

namespace MainMenu
{
    public class StartButton : Button
    {
        public Transform m_door;
        public Transform m_pivot;
        public GameObject finishLine;

        private bool started = false;
        private void Start() {
            Debug.LogError("STARTOO");
        }

        private void LateUpdate() {
            if (started) return;
            if (GameManager.Current.HotStart) {
                Click();

            }
        }

        public override void Click()
        {
            started = true;
            base.Click();
            SlowMotionOverTime slowMo = GetComponent<SlowMotionOverTime>();
            if(slowMo != null)
            {
                slowMo.StartScaling();
            }
            if(finishLine != null)
            {
                finishLine.GetComponent<finishLine>().ResetTime();
            }
            EventSystems.MainEventSystem.MainEvents.GameStarts();
                
            m_door.RotateAround(m_pivot.position, new Vector3(1, 0,0), -90);
            m_door.position = m_door.transform.forward * -1;
        }
    }
}