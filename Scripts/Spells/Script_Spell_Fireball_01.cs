﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Spell_Fireball_01 : MonoBehaviour
{
    void Start()
    {
        _Spell = gameObject.GetComponent<SpellManager>();
        _Rd = gameObject.GetComponent<Rigidbody>();
        Destroy(gameObject, _Spell.GetSpellDestroyTime());
    }

    void Update()
    {
        _Rd.AddForce(transform.forward * 10f);
    }


    public (SpellManager.SpellLocation, SpellManager.CastingAnimation, float) SpellInit() {
        return (_Spell.Location, _Spell.Animation, _Spell.GetSpellDelay());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            // force is how forcefully we will push the player away from the enemy.
            float force = 20;
            other.gameObject.GetComponentInParent<Script_Enemy_Base>().RecieveDamage(_Spell.GetSpellDamage());
            Rigidbody enemyRigidBody = other.gameObject.GetComponent<Rigidbody>();
            enemyRigidBody.MovePosition(enemyRigidBody.position + -other.transform.forward * force * Time.deltaTime);
            Destroy(gameObject);
        }

        if (other.gameObject.tag == "CollidableObstacles")
        {
            Destroy(gameObject);
        }

        if (other.gameObject.tag == "Default")
        {
            Destroy(gameObject);
        }
    }

    private SpellManager _Spell;
    private Rigidbody _Rd;
}
