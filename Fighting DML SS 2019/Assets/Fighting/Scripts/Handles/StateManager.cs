﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StateManager : MonoBehaviour
{

    public int health = 100;

    public float horizontal;
    public float vertical;
    public bool attack1;
    public bool attack2;
    public bool attack3;
    public bool crouch;

    public bool canAttack;
    public bool gettingHit;
    public bool currentlyAttacking;

    public bool dontMove;
    public bool onGround;
    public bool lookRight;

    public bool blocking;

    public bool dead;

    public bool start;
    public bool win;

    public bool _fboss1, _fboss2, _fboss3;

    public Slider healthSlider;

    public AudioClip clip;

    //public Slider healthSlider;

    [HideInInspector]
    public HandleDamageColliders handleDC;
    [HideInInspector]
    public HandleAnimations handleAnim;
    [HideInInspector]
    public HandleMovement handleMovement;
    
    public bool spare;
    [HideInInspector]
    public bool mercy;

    public GameObject[] movementColliders;

    ParticleSystem[] blood;

    public GameObject camera;
    Camera_Shake shake;

    public GameObject enemy;

    public GameObject mat;
    private float currentY, startTime;
    private Material _mat;
    AudioSource source;
    void Start()
    {
        //_mat = mat.GetComponent<Renderer>().material;
        source = GetComponent<AudioSource>();
        shake = camera.GetComponent<Camera_Shake>();
        blood = GetComponentsInChildren<ParticleSystem>();
        handleDC = GetComponent<HandleDamageColliders>();
        handleAnim = GetComponent<HandleAnimations>();
        handleMovement = GetComponent<HandleMovement>();
        //blood = GetComponentInChildren<ParticleSystem>();

        

        spare = false;
        mercy = false;
    }

    private float speed = 2f;

    void FixedUpdate()
    {
        onGround = isOnGround();

        if (win)
        {
            dontMove = true;
        } else
        {
            dontMove = false;
        }

        if(healthSlider != null)
        {
            healthSlider.value = health;
        }
    }

    bool isOnGround()
    {
        bool retVal = false;

        LayerMask layer = ~(1 << gameObject.layer | 1 << 3);
        retVal = Physics.Raycast(transform.position, -Vector3.up, 0.15f, layer);

        Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.up), Color.blue);

        return retVal;
    }

    public void ResetStateInputs()
    {
        horizontal = 0;
        vertical = 0;
        attack1 = false;
        attack2 = false;
        attack3 = false;
        crouch = false;
        gettingHit = false;
        currentlyAttacking = false;
        dontMove = false;
        win = false;
    }

    public void CloseMovementCollider(int index)
    {
        movementColliders[index].SetActive(false);
    }

    public void OpenMovementCollider(int index)
    {
        movementColliders[index].SetActive(true);
    }

    public void TakeDamage(int damage, HandleDamageColliders.DamageType damageType)
    {
        if (!gettingHit && !blocking)
        {
            switch (damageType)
            {
                case HandleDamageColliders.DamageType.light:
                    StartCoroutine(CloseImmortality(0.3f));
                    break;
                case HandleDamageColliders.DamageType.heavy:
                    handleMovement.AddVelocityOnCharacter(
                        ((!lookRight) ? Vector3.right * -1 : Vector3.right), 0.5f
                        );
                    StartCoroutine(CloseImmortality(1));
                    break;
            }


            health -= damage;
            gettingHit = true;
            //if(blood != null)
            //{
            foreach (ParticleSystem ps in blood)
                if(ps.gameObject.tag == "Hit")
                    ps.Emit(30);

            StartCoroutine(shake.Shake(.2f, .2f));
            //}

            
        }

        if (blocking)
        {
            foreach (ParticleSystem ps in blood)
                if (ps.gameObject.tag == "Block")
                    ps.Emit(30);
        }
    }

    IEnumerator CloseImmortality(float timer)
    {
        yield return new WaitForSeconds(timer);
        gettingHit = false;
        if (health <= 0)
        {
            source.clip = clip;
            source.Play();
            dead = true;
            enemy.GetComponent<StateManager>().win = true;
        }
    }
}
