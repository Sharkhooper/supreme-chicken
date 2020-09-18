using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Difficulty : ScriptableObject {
    public static Difficulty current;

    [System.Serializable]
    public struct Waiter {
        public float range;
        public float attackSpeed;
    }

    public Waiter waiter;
}
