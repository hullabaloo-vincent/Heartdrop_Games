﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Script_Enemy_Thug : MonoBehaviour
{
    void Start()
    {
        _Anim = GetComponent<Animator>();
        _AIBase = GetComponent<Script_Enemy_Base>();
        _Rd = GetComponent<Rigidbody>();
        _RightHand = gameObject.transform.Find(
            "Thug_rig/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand"
            ).gameObject; //get reference to right hand
        _LeftHand = gameObject.transform.Find(
            "Thug_rig/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand"
            ).gameObject; //get reference to left hand
        _RightFoot = gameObject.transform.Find(
            "Thug_rig/mixamorig:Hips/mixamorig:RightUpLeg/mixamorig:RightLeg/mixamorig:RightFoot"
            ).gameObject; //get reference to right hand
    }

    void Update()
    {
        if (!_Anim.GetBool("isDying") && !_IsDead)
        {
            /*
             * Checks if player is in line of sight OR is being loud around the enemy
             * @returns seenPlayer
             */
            if ((Vector3.Distance(_AIBase.GetPlayerLocation(), gameObject.transform.position) <= _VisibilityDistance && _AIBase.CanSeePlayer()) ||
                (Vector3.Distance(_AIBase.GetPlayerLocation(), gameObject.transform.position) <= _VisibilityDistance && (_AIBase.IsPlayerWalking() || _AIBase.IsPlayerRunning())))
            {
                if (!_AIBase.GetAlertStatus())
                {
                    _AIBase.AlertTeam();
                }

                Alerted();
            }
            if (!IsDodging)
            {
                /*
                 * If player is farther than the thug can punch, and is not currently attacking
                 * or blocking, move towards player and start walking animation
                 */
                if (seenPlayer && Vector3.Distance(_AIBase.GetPlayerLocation(), gameObject.transform.position) > 1.6f &&
                    !_Anim.GetBool("isAttacking"))
                {
                    ResetAnimation();
                    _Anim.SetBool("isWalking", true);
                }

                if (_Anim.GetCurrentAnimatorStateInfo(0).IsName("Walk") &&
                !_Anim.GetBool("isPunching") && !_Anim.GetBool("isKicking") &&
                !_Anim.GetBool("tookDamage_heavy") && !_Anim.GetBool("tookDamage_light"))
                {
                    _AIBase.UnlockMovement();
                    _AIBase.move();
                }

                /*
                 * If thug is not currently moving and is not currently attacking
                 * or blocking, make sure its default animation is idle
                 */
                if (_AIBase.GetAgentVelocity() == new Vector3(0f, 0f, 0f) && seenPlayer && Vector3.Distance(_AIBase.GetPlayerLocation(), gameObject.transform.position) <= 1.6f &&
                    !_Anim.GetBool("isAttacking"))
                {
                    ResetAnimation();
                    _Anim.SetBool("isIdle", true);
                }

                /*
                * The enemy is walking yet they are moving nowhere; the enemy might be stuck behind
                * other entities. This will change their course to flanking instead
                */
                if (_AIBase.GetAgentVelocity() == new Vector3(0f, 0f, 0f) && seenPlayer)
                {
                    _AIBase.SetFlankPosition();
                    _AIBase.SetFlank(true);
                }

                //Remove flank behavior if all teammates are dead
                if (_AIBase.GetTeam().Count == 0)
                {
                    _AIBase.SetFlank(false);
                }

                /*
                 * If player is within punching range and is in front of the thug, start attacking
                 */
                if (Vector3.Distance(_AIBase.GetPlayerLocation(), gameObject.transform.position) <= 1.5f &&
                    _AIBase.CanSeePlayerNarrow() && !_Anim.GetBool("isAttacking") && !_AIBase.IsFlanking() && !_FixRotation)
                {
                    _AIBase.stopMoving();
                    int attackChooser = Random.Range(1, 10);
                    if (attackChooser <= 4)
                    {
                        _Anim.SetBool("isPunching", true);
                    }
                    if (attackChooser >= 5 && attackChooser <= 8)
                    {
                        _Anim.SetBool("isChargedPunch", true);
                    }
                    if (attackChooser >= 9 && attackChooser <= 10)
                    {
                        _Anim.SetBool("isKicking", true);
                    }
                    _Anim.SetBool("isAttacking", true);
                    isBlocking = false;
                }

                /*
                 * If player goes outside of sight range, rotate towards player
                 */
                if (!_AIBase.CanSeePlayerNarrow() && !_Anim.GetBool("isAttacking") && !_AIBase.IsFlanking())
                {
                    _FixRotation = true;
                    _AIBase.RotateEnemy(_AIBase.GetPlayerLocation());
                }
                else if (_AIBase.CanSeePlayerNarrow() && _FixRotation)
                {
                    _FixRotation = false;
                }
                //If the enemy is attacking but cannot see the player anymore try to move to a new spot
                if (!_AIBase.CanSeePlayer() && !_AIBase.IsFlanking() && _Anim.GetBool("isAttacking"))
                {
                    _AIBase.SetFlankPosition();
                    _AIBase.SetFlank(true);
                }

                //Turn off flanking if the player is really close (EX: Player walks past enemy moving to flanking position)
                if (Vector3.Distance(_AIBase.GetPlayerLocation(), gameObject.transform.position) <= 1.5f && _AIBase.IsFlanking())
                {
                    _AIBase.SetFlank(false);
                }

                if (GameObject.Find("PlayerPunch(Clone)") &&
                    !_Anim.GetBool("tookDamage_light") &&
                    !_Anim.GetBool("tookDamage_heavy") &&
                    !blockDecision &&
                    seenPlayer && Vector3.Distance(_AIBase.GetPlayerLocation(), gameObject.transform.position) < 3f)
                {
                    //if the enemy is in mid punch, don't block
                    if (midPunch)
                    {
                        //don't block
                    }
                    else
                    {
                        blockDecision = true;
                        int blockChooser = Random.Range(1, 4);
                        // Debug.Log("Blocking with: " + blockChooser);
                        if (_AIBase.CanSeePlayer())
                        {
                            ResetAnimation();
                            //turn off physics forces
                            //SetRigidBodyKinematic(true);
                            if (blockChooser <= 2)
                            {
                                _Anim.SetBool("isBlocking", true);
                                _Anim.SetBool("isDodging", false);
                            }
                            else
                            {
                                _Anim.SetBool("isBlocking", false);
                                _Anim.SetBool("isDodging", true);
                            }
                        }
                    }
                }
            }
            if (IsDodging)
            {
                float force = 2;
                _Rd.MovePosition(_Rd.position + -transform.forward * force * Time.deltaTime);
            }

            if (_AIBase.GetPlayerFocus() == null && !_IsDead &&
             Vector3.Distance(_AIBase.GetPlayerLocation(), gameObject.transform.position) <= 5f)
            {
                _AIBase.PlayerReferece().GetComponent<Script_Player_Movement>().SetFocusEnemy(gameObject);
                //Tells the team that it is being focused by the player
                _AIBase.PlayerFocused();
            }
        }
    }

    //Called via animation event from Anim_Player_Punch
    public void PunchRight()
    {
        GameObject playerHit = Instantiate(punchObj, _RightHand.transform.position, _RightHand.transform.rotation);
    }
    public void PunchLeft()
    {
        GameObject playerHit = Instantiate(punchObj, _LeftHand.transform.position, _LeftHand.transform.rotation);
    }

    public void Kick()
    {
        GameObject playerHit = Instantiate(kickObj, _RightFoot.transform.position, _RightFoot.transform.rotation);
    }

    public void Alerted()
    {
        seenPlayer = true;
        _Anim.SetBool("isAttackingPlayer", true);
    }

    public void RecieveDamage(float damage)
    {
        if (!isBlocking && !_Anim.GetBool("isDying") && CanRecieveDamage && !_IsDead)
        {
            Health -= damage;
            if (Health > 0)
            {
                ResetAnimation();
                CanRecieveDamage = false;
                _AIBase.stopMoving();
                if (damage > 2f)
                {
                    _Anim.SetBool("tookDamage_heavy", true);
                    Camera.main.GetComponentInParent<Script_Camera_Shake>().TriggerShake(0.3f);
                }
                else
                {
                    _Anim.SetBool("tookDamage_light", true);
                    Camera.main.GetComponentInParent<Script_Camera_Shake>().TriggerShake(0.1f);
                }
            }
            else
            {
                if (damage > 2f)
                {
                    _Anim.SetBool("isDying_heavy", true);
                }
                else
                {
                    _Anim.SetBool("isDying_light", true);
                }
                Camera.main.GetComponentInParent<Script_Camera_Shake>().TriggerShake(0.5f);
                _Anim.SetBool("isDying", true);
                minimapObject.SetActive(false);
                _AIBase.PlayerReferece().GetComponent<Script_Player_Movement>().StopFocus();
                RemoveFromTeam();
            }
            StartCoroutine("DamageBuffer");
        }
    }
    IEnumerator DamageBuffer()
    {
        yield return new WaitForSeconds(0.2f);
        CanRecieveDamage = true;
        yield return 0;
    }

    private void RemoveFromTeam()
    {
        //Remove room enemy count
        _AIBase.GetSpawnActor().GetComponent<Script_Enemy_Spawning>().RemoveFromList(gameObject);
        for (int i = 0; i < _AIBase.GetTeam().Count; i++)
        {
            //Remove self from every team list
            _AIBase.GetTeam()[i].GetComponent<Script_Enemy_Base>().RemoveFromTeam(gameObject);
        }
    }

    private void ResetAnimation()
    {
        foreach (AnimatorControllerParameter parameter in _Anim.parameters)
        {
            _Anim.SetBool(parameter.name, false);
        }
        _Anim.SetBool("isAttackingPlayer", true);
    }

    #region Punching Animation Controls
    public void InPunch()
    {
        midPunch = true;
    }
    public void OutPunch()
    {
        midPunch = false;
    }
    public void StopPunch()
    {
        _Anim.SetBool("isPunching", false);
    }
    #endregion
    #region Kicking Animation Controls
    public void stopKick()
    {
        _Anim.SetBool("isKicking", false);
    }
    #endregion
    #region Blocking Animation Controls
    public void BlockingStart()
    {
        _AIBase.stopMoving();
        isBlocking = true;
        blockDecision = false;
    }
    public void BlockingEnd()
    {
        isBlocking = false;
    }
    public void TurnOffBlocking()
    {
        _Anim.SetBool("isBlocking", false);
    }
    #endregion
    #region Charged Punch Animation Controls
    public void turnOff_ChargedAttack()
    {
        ResetAnimation();
        _Anim.SetBool("isIdle", true);
    }
    #endregion
    #region Damage Animation Controls
    public void TurnOff_TakeDamageLight()
    {
        _AIBase.UnlockMovement();
        ResetAnimation();
        _Anim.SetBool("isIdle", true);
    }
    public void TurnOff_TakeDamageHeavy()
    {
        _AIBase.UnlockMovement();
        ResetAnimation();
        _Anim.SetBool("isIdle", true);
    }
    #endregion
    #region Death Animation Controls
    public void TurnOffDeath()
    {
        _IsDead = true;
        ResetAnimation();
        _AIBase.PlayerReferece().GetComponent<Script_Player_Movement>().StopFocus();
        Destroy(GetComponent<CapsuleCollider>());
        Destroy(GetComponent<NavMeshAgent>());
        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<Script_Enemy_Base>());
    }
    #endregion
    #region Dodge Back Animation Controls
    public void TurnOffDodgeBack()
    {
        ResetAnimation();
        _Anim.SetBool("isIdle", true); ;
    }
    #endregion
    #region Dodge Back
    public void DodgeBackStart()
    {
        _AIBase.stopMoving();
        IsDodging = true;
        ResetAnimation();
        _Anim.SetBool("isIdle", true);
    }
    public void DodgeBackEnd()
    {
        ResetAnimation();
        IsDodging = false;
        _Anim.SetBool("isIdle", true);
    }
    #endregion
    #region Turn off Attack
    public void TurnOffAttack()
    {
        ResetAnimation();
        _Anim.SetBool("isIdle", true);
    }
    #endregion
    #region Turn on Attack
    public void TurnOnAttack()
    {
        _AIBase.stopMoving();
        _Anim.SetBool("isAttacking", true);
    }
    #endregion

    private Animator _Anim;
    private Script_Enemy_Base _AIBase;
    private GameObject _RightHand;
    private GameObject _LeftHand;
    private GameObject _RightFoot;
    public GameObject punchObj;
    public GameObject kickObj;
    public GameObject rig;
    public GameObject minimapObject;

    private Rigidbody _Rd;

    public float Health = 5f;
    private float _VisibilityDistance = 7f;
    private float _VisibilityDistanceMultiplyer = 2f;
    private bool _IsDead = false;
    bool seenPlayer = false;
    bool isBlocking = false;
    bool blockDecision = false;
    bool midPunch = false;
    private bool IsDodging = false;

    bool CanRecieveDamage = true;
    private bool _FixRotation = false;
}
