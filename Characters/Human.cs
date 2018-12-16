﻿/// <summary>
/// Base class for all humanoid GameObjects.
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Human : MonoBehaviour {

    protected float speed;
    public int health;

    [SerializeField]
    protected bool isMoving;
    protected NavMeshAgent agent;

    [SerializeField]
    public Vector3 target;

    protected Animator charAnimator;

    // Use this for initialization
    protected virtual void Awake () {
        health = 100;
        target = transform.position;
        isMoving = false;
        agent = gameObject.GetComponent<NavMeshAgent>();
        target = transform.position;
        charAnimator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    protected virtual void Update () {
        StartCoroutine("GetSpeed");
    }
    protected void ManageHealth()
    {
        if (health > 100)
        {
            health = 100;
        }
        if (health <= 0)
        {
            health = 0;
            Death();
        }
    }
    protected void Death()
    {
        gameObject.SetActive(false);
    }
    protected IEnumerator GetSpeed()
    {
        Vector3 pos1 = transform.position;
        yield return new WaitForSeconds(0.1f);
        Vector3 pos2 = transform.position;
        if (pos1 != pos2)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }
}
