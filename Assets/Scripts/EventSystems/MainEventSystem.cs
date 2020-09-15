using System;
using UnityEngine;

namespace EventSystems
{
    public class MainEventSystem : MonoBehaviour
    {
        public event Action onGameStarts;

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
    }
}