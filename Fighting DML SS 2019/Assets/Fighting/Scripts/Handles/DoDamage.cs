﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoDamage : MonoBehaviour
{
    StateManager states;

    public AudioClip clip;

    AudioSource aSource;

    public HandleDamageColliders.DamageType damageType;

    private void Start()
    {
        states = GetComponentInParent<StateManager>();
        aSource = GetComponent<AudioSource>();
        aSource.clip = clip;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<StateManager>())
        {
            StateManager oState = other.GetComponentInParent<StateManager>();

            if(oState != states)
            {
                if (!oState.currentlyAttacking)
                {
                    oState.TakeDamage(5, damageType);
                    aSource.Play();
                }
            }
        }
    }
}
