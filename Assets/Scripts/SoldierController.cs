using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoldierController : MonoBehaviour
{
    public int m_MaxHealthPoints;
    public int m_CurrentHealthPoints;

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

    public float m_FireRate = 1.0f;
    public float m_RemainingFireRate = 0.0f;

    public Transform m_Turret;
    public Transform m_BulletSpawnPoint;
    public Transform m_WhereToAim;
    private ShellPoolManager m_ShellPoolManager;


    // Start is called before the first frame update
    void Start()
    {
        m_NavMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        m_RemainingStandbyTime = m_StandbyTime;
        m_CurrentHealthPoints = m_MaxHealthPoints;
        m_ShellPoolManager = GameObject.Find("ShellPoolManager").GetComponent<ShellPoolManager>();
        m_Animator = GetComponent<Animator>();
        m_EnemyBase = GameObject.Find("EnemyBase");
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

                break;
            case SoldierStates.FIRING_ENEMY:
                TurretLooksAtPlayer(dt);
                CheckEnemy();
                FireEnemy(dt);
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
                m_NavMeshAgent.isStopped = false;
                m_NavMeshAgent.SetDestination(m_EnemyBase.transform.position);
                break;
            case SoldierStates.FIRING_BASE:
                
                break;
            case SoldierStates.FIRING_ENEMY:
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
        if (other.CompareTag("EnemyBase"))
        {
            OnStateEnter(SoldierStates.SELECT_AND_GO_TO_POINT);
        }
        if (other.CompareTag("Enemy"))
        {
            m_EnemyList.Add(other.gameObject);
            if(m_CurrentState != SoldierStates.FIRING_ENEMY)
            {
                OnStateEnter(SoldierStates.FIRING_ENEMY);
            }
            
        }
    }

    private void FireEnemy(float dt)
    {
        m_Animator.SetBool("IsFiring", true);   
        if (m_RemainingFireRate <= 0)
        {
            m_Animator.SetTrigger("Shoot");
            m_RemainingFireRate = m_FireRate;
            SpawnShell();
        }
        else
        {
            m_RemainingFireRate -= dt;
        }
    }

    private void Shoot()
    {
        Debug.Log("bala");
        SpawnShell();
    }
    private void TurretLooksAtPlayer(float dt)
    {
        //Look at player
        m_WhereToAim = m_EnemyList[0].GetComponent<Transform>();
        Vector3 lookPos = m_WhereToAim.transform.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        gameObject.transform.rotation = Quaternion.LookRotation(lookPos);
    }

    private void CheckEnemy()
    {
        for (int i = 0; i < m_EnemyList.Count; i++)
        {
            if (m_EnemyList[i].IsDestroyed())
            {
                m_EnemyList.RemoveAt(i);
            }
        }

        if (m_EnemyList.Count <= 0)
        { 
            OnStateEnter(SoldierStates.SELECT_AND_GO_TO_POINT);
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
        GameObject shell = m_ShellPoolManager.TakeShell();

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
