using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoldierController : MonoBehaviour
{
    public int m_MaxHealthPoints;
    public int m_CurrentHealthPoints;

    public GameObject m_EnemyBase;
    private Object m_Enemy;

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
                GoToEnemyBase();
                break;
            case SoldierStates.FIRING_BASE:

                break;
            case SoldierStates.FIRING_ENEMY:
                m_NavMeshAgent.isStopped = true;
                TurretLooksAtPlayer(dt);
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
                m_NavMeshAgent.isStopped = false;
                m_NavMeshAgent.SetDestination(m_EnemyBase.transform.position);
                break;
            case SoldierStates.FIRING_BASE:
                
                break;
            default:
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
            m_CurrentState = SoldierStates.SELECT_AND_GO_TO_POINT;
        }
    }
    private void GoToEnemyBase()
    {
        

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyBase")
        {
            m_CurrentState = SoldierStates.FIRING_ENEMY;
            m_Enemy = other;
            m_WhereToAim = other.transform;
        }
        if (other.tag == "Enemy")
        {
            m_CurrentState = SoldierStates.FIRING_ENEMY;
            m_Enemy = other;
            m_WhereToAim = other.transform;
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
            if(m_Enemy.IsDestroyed())
            {
                m_Animator.SetBool("IsFiring", false);
            }
        }
        else
        {
            m_RemainingFireRate -= dt;
        }
    }
    private void TurretLooksAtPlayer(float dt)
    {
        //Look at player
        Vector3 lookPos = m_WhereToAim.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        m_Turret.rotation = Quaternion.LookRotation(lookPos);
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
