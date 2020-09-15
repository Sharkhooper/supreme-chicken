using System;
using UnityEngine;

namespace EventSystems
{
    public class MainEventSystem : MonoBehaviour
    {
        public event Action onGameStarts;
        public event Action onGameQuit;

        public static MainEventSystem MainEvents;

        private void Awake()
        {
            MainEvents = this;
        }

        public void GameStarts()
        {
            if (onGameStarts != null)
            {
                onGameStarts();
            }
        }
        public void GameQuit()
        {
            if (onGameQuit != null)
            {
                onGameQuit();
            }
        }
    }
}