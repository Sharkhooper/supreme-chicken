using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private GameObject _player;

    [SerializeField] private float attackRadius = 1.0f;
    [SerializeField] private Vector3 attackPosition = Vector3.right; // wird auf Player transform addiert
    [SerializeField] private int enemyLayer = 1;
    [SerializeField] private float attackCD = 3.0f;
    private float currentCD = 0f;
    
    private void Awake()
    {
        _player = gameObject;
    }

    private void Update()
    {
        currentCD -= Time.deltaTime;
        if (currentCD < 0f)
        {
            currentCD = 0f;
        }
    }


    public void HandleAttackInput(InputAction.CallbackContext ctx)
    {
        if (currentCD == 0f)
        {
            currentCD = attackCD;
            Collider[] hits = Physics.OverlapSphere(_player.transform.position + attackPosition, attackRadius);
            if (hits.Length > 0)
            {
                foreach (var h in hits)
                {
                    GameObject tempGO = h.gameObject; // temporary GameObject
                    if (tempGO.layer.Equals(enemyLayer))
                    {
                        tempGO.GetComponent<Killable>().GetKilled();
                        Debug.Log("killed something");
                    }
                }
            }
        }
    }
}
