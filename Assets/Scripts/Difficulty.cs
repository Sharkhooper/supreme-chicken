using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Difficulty", menuName = "Difficulty", order = 51)]
public class Difficulty : ScriptableObject {
    private static Difficulty _current;

    public delegate void DifficultyChange(Difficulty d);

    public static event DifficultyChange OnDifficultyChange;

    public static Difficulty current { get => _current; set {
            _current = value;
            OnDifficultyChange.Invoke(_current);
        }
    }

    public static Difficulty defaultDif { get; private set; }
    public bool defaultDifficulty;
 
    private void OnEnable() {
        if(defaultDifficulty && defaultDif == null) {
            _current = this;
            defaultDif = this;
        }
    }

    [System.Serializable]
    public struct Chef
    {
        public float range;
        public float angle;
        public float attackSpeed;
        public float moveSpeed;
    }

    [System.Serializable]
    public struct Waiter {
        public float range;
        public float angle;
        public float attackSpeed;
        public float moveSpeed;
        public float plateSpeed;
        [Range(0,1)]
        public float accuracy;
    }

    [System.Serializable]
    public struct Cockroach
    {
        public float range;
        public float angle;
        public float moveSpeed;
        public float aimDuration;
        public float shootDuration;
        public float growSpeed;
        public float shootCooldown;
    }

    public Chef chef;
    public Waiter waiter;
    public Cockroach cockroach;
}
