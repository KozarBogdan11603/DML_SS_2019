﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandleAnimations : MonoBehaviour
{
    public Animator anim;
    StateManager states;

    public float attackRate = .3f;
    public AttackBase[] attacks = new AttackBase[2];

    public bool isDead;

    public AudioClip clip;
    AudioSource aSource;

    // Start is called before the first frame update
    void Start()
    {
        states = GetComponent<StateManager>();
        anim = GetComponent<Animator>();
        aSource = GetComponent<AudioSource>();
        aSource.clip = clip;
        isDead = false;

        if (states.lookRight)
        {
            anim.SetBool("Player", true);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDead)
        {
            states.dontMove = anim.GetBool("DontMove");

            anim.SetBool("TakesHit", states.gettingHit);
            anim.SetBool("OnAir", !states.onGround);
            //anim.SetBool("Crouch", states.crouch);

            float movement = (states.lookRight) ? states.horizontal : -states.horizontal;
            anim.SetFloat("Movement", movement);

            if (states.vertical < 0)
            {
                states.crouch = true;
                anim.SetBool("Crouch", true);
            }
            else
            {
                states.crouch = false;
                anim.SetBool("Crouch", false);
            }

            HandleAttacks();
        } else
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                Spare();
                StartCoroutine(Restart());
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                Mercy();
                StartCoroutine(Restart());
            }
        }

        if (states.dead && !isDead)
        {
            anim.Play("Down_B");
            isDead = true;
        }
    }

    IEnumerator Restart()
    {
        yield return new WaitForSeconds(6);

        SceneManager.LoadScene(1);
    }

    void Spare()
    {
        anim.SetBool("Spare", true);
    }

    void Mercy()
    {
        anim.SetBool("Mercy", true);
    }

    void HandleAttacks()
    {
        if (states.canAttack)
        {
            if (states.attack1)
            {
                aSource.Play();
                attacks[0].attack = true;
                attacks[0].attackTimer = 0;
                attacks[0].timesPressed++;
            }

            if (attacks[0].attack)
            {
                attacks[0].attackTimer += Time.deltaTime;

                if(attacks[0].attackTimer > attackRate || attacks[0].timesPressed >= 3)
                {
                    attacks[0].attackTimer = 0;
                    attacks[0].attack = false;
                    attacks[0].timesPressed = 0;
                }
            }

            if (states.attack2)
            {
                aSource.Play();
                attacks[1].attack = true;
                attacks[1].attackTimer = 0;
                attacks[1].timesPressed++;
            }

            if (attacks[1].attack)
            {
                attacks[1].attackTimer += Time.deltaTime;

                if(attacks[1].attackTimer > attackRate || attacks[0].timesPressed >= 3)
                {
                    attacks[1].attackTimer = 0;
                    attacks[1].attack = false;
                    attacks[1].timesPressed = 0;
                }
            }

            if (states.attack3)
            {
                aSource.Play();
                attacks[2].attack = true;
                attacks[2].attackTimer = 0;
                attacks[2].timesPressed++;
            }

            if (attacks[2].attack)
            {
                attacks[2].attackTimer += Time.deltaTime;

                if (attacks[2].attackTimer > attackRate || attacks[0].timesPressed >= 3)
                {
                    attacks[2].attackTimer = 0;
                    attacks[2].attack = false;
                    attacks[2].timesPressed = 0;
                }
            }
        }

        anim.SetBool("Attack1", attacks[0].attack);
        anim.SetBool("Attack2", attacks[1].attack);
        anim.SetBool("Attack3", attacks[2].attack);
    }

    public void JumpAnim()
    {
        anim.SetBool("Attack1", false);
        anim.SetBool("Attack2", false);
        anim.SetBool("Attack3", false);
        anim.SetBool("Jump", true);
        StartCoroutine(CloseBoolInAnim("Jump"));
    }

    IEnumerator CloseBoolInAnim(string name)
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetBool(name, false);
    }

    [System.Serializable]
    public class AttackBase
    {
        public bool attack;
        public float attackTimer;
        public int timesPressed;
    }
}
