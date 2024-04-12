using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class SoldierController : MonoBehaviour
{
    public int m_MaxHealthPoints;
    public int m_CurrentHealthPoints;
    public bool m_Allied;

    public GameObject m_EnemyBase;
    public List<GameObject> m_EnemyList = new List<GameObject>();

    public Animator m_Animator;

    public enum SoldierStates
    {
        NONE = -1,
        STANDBY,
        SELECT_AND_GO_TO_POINT,
        FIRING_BASE,
        FIRING_ENEMY
    }

    public SoldierStates m_CurrentState = SoldierStates.STANDBY;

    public UnityEngine.AI.NavMeshAgent m_NavMeshAgent;

    public float m_StandbyTime = 1.0f;
    public float m_RemainingStandbyTime = 0.0f;

    public float m_FireRate = 2.0f;
    public float m_RemainingFireRate = 0.0f;

    public Transform m_BulletSpawnPoint;
    public Transform m_WhereToAim;
    private ShellPoolManager m_ShellPoolManager;
    public bool m_IsHeavy;
    private int m_Damage;
    private ShellScript m_ShellScript;


    void Start()
    {
        m_NavMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        m_RemainingStandbyTime = m_StandbyTime;
        m_CurrentHealthPoints = m_MaxHealthPoints;
        m_RemainingFireRate = m_FireRate;
        m_ShellPoolManager = GameObject.Find("ShellPoolManager").GetComponent<ShellPoolManager>();
        m_Animator = GetComponent<Animator>();

        //Finds the base and the necessary points of each gatherer deppending on if they are spawned from the player or the enemy
        if (m_Allied)
        {
            m_EnemyBase = GameObject.Find("EnemyBase");
        }
        else 
        { 
            m_EnemyBase = GameObject.Find("PlayerBaseBuild");
        }
        
        if(gameObject.CompareTag("Allied"))
        {
            m_Allied = true;
        }else { m_Allied=false; }

        //If the soldier is heavy the bullet shooted will do more damage
        if(m_IsHeavy) { m_Damage = 2; }
        else
        {
            m_Damage = 1;
        }
    }

    void Update()
    {
        float dt = Time.deltaTime;

        switch (m_CurrentState)
        {
            case SoldierStates.STANDBY:
                StandbyBehaviour(dt);
                break;
            case SoldierStates.SELECT_AND_GO_TO_POINT:
                break;
            case SoldierStates.FIRING_BASE:
                if(m_CurrentHealthPoints > 0)
                {
                    //Stops instantly the soldier
                    m_NavMeshAgent.velocity = Vector3.zero;
                    TurretLooksAtPlayer(dt);
                    FireEnemy(dt);
                }
                break;
            case SoldierStates.FIRING_ENEMY:
                if(m_CurrentHealthPoints > 0) 
                {
                    CheckEnemy();
                    //Stops instantly the soldier
                    m_NavMeshAgent.velocity = Vector3.zero;
                    TurretLooksAtPlayer(dt);
                    FireEnemy(dt);
                }
                break;
        }

    }
    private void OnStateEnter(SoldierStates thisState)
    {
        switch (thisState)
        {
            case SoldierStates.STANDBY:
                
                break;
            case SoldierStates.SELECT_AND_GO_TO_POINT:
                m_CurrentState = SoldierStates.SELECT_AND_GO_TO_POINT;
                m_Animator.SetBool("IsWalking", true);
                m_Animator.SetBool("IsFiring", false);
                m_NavMeshAgent.isStopped = false;
                m_NavMeshAgent.SetDestination(m_EnemyBase.transform.position);
                break;
            case SoldierStates.FIRING_BASE:
                m_Animator.SetBool("IsFiring", true);
                m_EnemyList.Insert(0, m_EnemyBase);
                m_NavMeshAgent.isStopped = true;
                m_CurrentState = SoldierStates.FIRING_BASE;
                break;
            case SoldierStates.FIRING_ENEMY:
                m_Animator.SetBool("IsFiring", true);
                m_NavMeshAgent.isStopped= true;
                m_CurrentState = SoldierStates.FIRING_ENEMY;
                break;
        }
    }

    private void StandbyBehaviour(float dt)
    {
        //When the soldier is spawned, it stays in standby 1 second
        if (m_RemainingStandbyTime > 0)
        {
            m_RemainingStandbyTime -= dt;
        }
        else
        {
            m_RemainingStandbyTime = m_StandbyTime;
            OnStateEnter(SoldierStates.SELECT_AND_GO_TO_POINT);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        //It checks when the soldier can shoot an enemy deppending on if its allied or not
        if (m_Allied)
        {
            if (other.CompareTag("EnemyBase"))
            {
                OnStateEnter(SoldierStates.FIRING_BASE);
            }
            if (other.CompareTag("Enemy") && !other.isTrigger)
            {
                m_EnemyList.Add(other.gameObject);
                if (m_CurrentState != SoldierStates.FIRING_ENEMY)
                {
                    OnStateEnter(SoldierStates.FIRING_ENEMY);
                }

            }
        }
        else
        {
            if (other.CompareTag("AlliedBase"))
            {
                OnStateEnter(SoldierStates.FIRING_BASE);
            }
            if (other.CompareTag("Allied") && !other.isTrigger)
            {
                m_EnemyList.Add(other.gameObject);
                if (m_CurrentState != SoldierStates.FIRING_ENEMY)
                {
                    OnStateEnter(SoldierStates.FIRING_ENEMY);
                }

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Removes from the list the enemyes that gets ot from the range of the soldier
        if(m_Allied)
        {
            if (other.gameObject.tag.Equals("Enemy"))
            {
                m_EnemyList.Remove(other.gameObject);
            }
        }else
        {
            if (other.gameObject.tag.Equals("Allied"))
            {
                m_EnemyList.Remove(other.gameObject);
            }
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Shell"))
        {
            m_ShellScript = collision.gameObject.GetComponent<ShellScript>();
            TakeDamage(m_ShellScript.m_Damage);
        }
    }

    private void FireEnemy(float dt)
    { 
        //Shoots to the enemy every 2 secondes
        if (m_RemainingFireRate <= 0)
        {
            m_RemainingFireRate = m_FireRate;
            SpawnShell();
        }
        else
        {
            m_RemainingFireRate -= dt;
        }
    }
    private void TurretLooksAtPlayer(float dt)
    {
        //The solider aims to the target
        m_WhereToAim = m_EnemyList[0].GetComponent<Transform>();
        Vector3 lookPos = m_WhereToAim.transform.position;
        Vector3 targetDirection = m_WhereToAim.transform.position - transform.position;
        lookPos.y = 0;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, dt, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    private void CheckEnemy()
    {
        //Checks if there is an enemy in the range of the soldier
        if (m_EnemyList.Count <= 0)
        {
            OnStateEnter(SoldierStates.SELECT_AND_GO_TO_POINT);
        }
        else
        {
            for (int i = 0; i < m_EnemyList.Count; i++)
            {
                if (m_EnemyList[i] == null || !m_EnemyList[i].activeInHierarchy)
                {
                    m_EnemyList.RemoveAt(i);
                    OnStateEnter(SoldierStates.SELECT_AND_GO_TO_POINT);
                }
            }
        }
    }
    public void TakeDamage(int damage)
    {
        m_CurrentHealthPoints -= damage;
        if (m_CurrentHealthPoints <= 0)
        {
            m_Animator.SetBool("IsDead",true);
        }
    }

    private void SpawnShell()
    {
        //Spawns a bullet in the tip of the gun of the soldier and shoot it to the enemy position
        GameObject shell = m_ShellPoolManager.TakeShell(m_Allied,m_Damage);

        shell.transform.position = m_BulletSpawnPoint.position;
        shell.transform.rotation = m_BulletSpawnPoint.rotation;

        Rigidbody shellRB = shell.GetComponent<Rigidbody>();
        shellRB.velocity = Vector3.zero;
        shellRB.angularVelocity = Vector3.zero;

        shell.SetActive(true);

        shell.GetComponent<Rigidbody>().AddForce(shell.transform.forward * 1000);
    }

    private void DestroyObject()
    {
        //When the death animation is completed the soldier is destroyed
        Destroy(gameObject);
    }

}
