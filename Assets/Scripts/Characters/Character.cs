﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Character : MonoBehaviour
{

    public string faction;
    [SerializeField]
    protected float speed;

    protected Vector2 direction;
    [SerializeField]
    protected bool isMoving;
    protected NavMeshAgent agent;

    [SerializeField]
    protected Vector3 target;

    //Figthing
    [SerializeField]
    protected int health;
    [SerializeField]
    protected bool isAttacking;
    [SerializeField]
    protected GameObject attackTarget;
    [SerializeField]
    protected bool enemyInRange;
    [SerializeField]
    protected float distanceToEnemy;
    [SerializeField]
    protected int ammo;
    [SerializeField]
    protected GameObject shootFrom;
    [SerializeField]
    protected GameObject bullet;
    public LayerMask ignoreMask;

    protected virtual void Awake()
    {
        health = 100;
        target = transform.position;
        agent = gameObject.GetComponent<NavMeshAgent>();
        ammo = 100;
        isMoving = false;
    }

    protected virtual void Update()
    {
        StartCoroutine(GetSpeed());
        CaptureBuilding();
        ManageHealth();
        if (target != transform.position && isMoving == false)
        {
            Move();
        }
        if (attackTarget != null)
        {
            Attack();
        }
    }

    protected void Move()
    {
        if (isAttacking == true)
        {
            StopCoroutine(Melee());
            StopCoroutine(Shoot());
            distanceToEnemy = 0f;
            enemyInRange = false;
            isAttacking = false;
        }
        Debug.Log("void Move");
        agent.SetDestination(target);
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
    protected void Attack()
    {
        target = transform.position;
        distanceToEnemy = Vector3.Distance(gameObject.transform.position, attackTarget.transform.position);
        Vector3 directionToEnemy = (attackTarget.transform.position - transform.position).normalized;
        if (isAttacking == false)
        {
            Debug.Log("LookAt");
            gameObject.transform.LookAt(attackTarget.transform.position);
            if (!Physics.Raycast(shootFrom.transform.position, directionToEnemy, distanceToEnemy, ignoreMask))
            {
                enemyInRange = true;
            }
            else
            {
                enemyInRange = false;
            }
        }
        if (enemyInRange == true)
        {
            if (distanceToEnemy < 3f && isAttacking == false)
            {
                StartCoroutine(Melee());
            }
            else if (distanceToEnemy > 3f && isAttacking == false) //Also if total ammo > 0
            {
                StartCoroutine(Shoot());
            }
        }
        else
        {

        }
    }
        protected IEnumerator Melee() //Hit attackTarget
        {
            StopCoroutine(Shoot());
            isAttacking = true;
            yield return new WaitForSeconds(1);
            attackTarget.GetComponent<Character>().health -= 10;
            isAttacking = false;
        }
        protected IEnumerator Shoot() //Launch a bullet at attackTarget
        {
            if (ammo > 0)
            {
                StopCoroutine(Melee());
                isAttacking = true;
                //Launch a bullet
                yield return new WaitForSeconds(1);
                Vector3 shootTo = new Vector3(Random.Range(attackTarget.transform.position.x - 0.5f, attackTarget.transform.position.x + 0.5f), Random.Range(attackTarget.transform.position.y - 0.5f, attackTarget.transform.position.y + 0.5f), Random.Range(attackTarget.transform.position.z - 0.5f, attackTarget.transform.position.z + 0.5f));
                GameObject spawnedBullet = Instantiate(bullet, shootFrom.transform.position, shootFrom.transform.rotation);
                spawnedBullet.GetComponent<Bullet>().ShootTo(shootTo);
                ammo -= 1;
                //Rough aiming at the attackTarget
                isAttacking = false;
            }
            else
            {
                //reload
            }
        }
    protected IEnumerator GetSpeed()
    {
        Vector3 pos1 = transform.position;
        yield return new WaitForSeconds(0.1f);
        Vector3 pos2 = transform.position;
        if(pos1 != pos2)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }
        protected void CaptureBuilding() //Replacement for trigger colliders
        {
            GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
            foreach (GameObject building in buildings)
            {
                BuildingClass buildingClass = building.GetComponent<BuildingClass>();
                float distance = Vector3.Distance(gameObject.transform.position, building.transform.position);
                if (buildingClass.faction != faction && distance < 10f)
                {
                    buildingClass.StartCoroutine(buildingClass.Capture(faction));
                }
            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Bullet") //Take bullet damage
            {
                Debug.Log(gameObject.name + " " + health);
                health -= 10;
            }
        }

    }
