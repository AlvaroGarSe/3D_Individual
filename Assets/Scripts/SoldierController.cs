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

    public GameObject m_SoldierBody;
    public Transform m_BulletSpawnPoint;
    public Transform m_WhereToAim;
    private ShellPoolManager m_ShellPoolManager;

    // Start is called before the first frame update
    void Start()
    {
        m_NavMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        m_RemainingStandbyTime = m_StandbyTime;
        m_CurrentHealthPoints = m_MaxHealthPoints;
        m_RemainingFireRate = m_FireRate;
        m_ShellPoolManager = GameObject.Find("ShellPoolManager").GetComponent<ShellPoolManager>();
        m_Animator = GetComponent<Animator>();
        if(m_Allied)
        {
            m_EnemyBase = GameObject.Find("EnemyBase");
        }
        else { m_EnemyBase = GameObject.Find("PlayerBaseBuild"); }
        
        if(gameObject.CompareTag("Allied"))
        {
            m_Allied = true;
        }else { m_Allied=false; }
    }

    // Update is called once per frame
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
                    m_NavMeshAgent.velocity = Vector3.zero;
                    TurretLooksAtPlayer(dt);
                    FireEnemy(dt);
                }
                
                break;
            case SoldierStates.FIRING_ENEMY:
                if(m_CurrentHealthPoints > 0) 
                {
                    CheckEnemy();
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
                m_Animator.SetTrigger("Shoot");
                m_Animator.SetBool("IsFiring", true);
                Debug.Log("Baseee");
                m_EnemyList.Insert(0, m_EnemyBase);
                m_NavMeshAgent.isStopped = true;
                m_CurrentState = SoldierStates.FIRING_BASE;
                break;
            case SoldierStates.FIRING_ENEMY:
                m_Animator.SetTrigger("Shoot");
                m_Animator.SetBool("IsFiring", true);
                m_NavMeshAgent.isStopped= true;
                m_CurrentState = SoldierStates.FIRING_ENEMY;
                break;
        }
    }

    private void StandbyBehaviour(float dt)
    {
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
                m_EnemyList.Clear();
                m_EnemyList[0] = other.gameObject;
                OnStateEnter(SoldierStates.SELECT_AND_GO_TO_POINT);
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
            TakeDamage(1);
        }
    }

    private void FireEnemy(float dt)
    { 
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
        m_WhereToAim = m_EnemyList[0].GetComponent<Transform>();
        Vector3 lookPos = m_WhereToAim.transform.position;
        Vector3 targetDirection = m_WhereToAim.transform.position - transform.position;
        lookPos.y = 0;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, dt, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    private void CheckEnemy()
    {
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
        GameObject shell = m_ShellPoolManager.TakeShell(m_Allied);

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
        Destroy(gameObject);
    }

}
