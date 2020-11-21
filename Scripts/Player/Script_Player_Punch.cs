﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Player_Punch : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, DestroyTime);
    }

    public void SetPlayerReference(GameObject p)
    {
        _PlayerRef = p.GetComponent<Script_Player_Movement>();
        _Player = p;
    }

    //Called on when a chain attack is more than 1
    public void UpdateDamage(float newDamage)
    {
        Damage = newDamage;
    }

    private void OnCollisionEnter(Collision other)
    {
        // force is how forcefully we will push the player away from the enemy.
        float force = 10;
        // If the object we hit is the enemy
        if (other.gameObject.tag == "Enemy" && !_DoOnce)
        {
            _DoOnce = true;
            _PlayerRef.HitLanded();
            other.gameObject.GetComponentInParent<Script_Enemy_Base>().RecieveDamage(Damage);
            // Calculate Angle Between the collision point and the player
            Vector3 dir = other.contacts[0].point - transform.position;
            // We then get the opposite (-Vector3) and normalize it
            dir = -dir.normalized;
            // And finally we add force in the direction of dir and multiply it by force. 
            // This will push back the player
            //GetComponent<Rigidbody>().AddForce(dir * force);
            Rigidbody enemyRigidBody = other.gameObject.GetComponent<Rigidbody>();
            // enemyRigidBody.AddForce(dir * force, ForceMode.Impulse);


            Vector3 knock = _Player.transform.forward;
            knock *= force;
            enemyRigidBody.AddForce(knock);
        }
    }

    public float DestroyTime = 0.35f;
    public float Damage = 1.5f;

    private bool _DoOnce = false;

    private Script_Player_Movement _PlayerRef;

    private GameObject _Player;
}
