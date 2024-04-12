using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoldierController;

public class GathererController : MonoBehaviour
{
    public int m_MaxHealthPoints;
    public int m_CurrentHealthPoints;
    public bool m_GoldGatherer;
    public bool m_Allied;

    public Transform m_GoldMine;
    public Transform m_GoldRunningPoint;

    public GameObject m_Base;
    private PlayerBaseController m_BaseScript;
    private EnemyBaseController m_EnemyBaseScript;
    public Animator m_Animator;
    private ShellScript m_ShellScript;

    public enum GoldGathererStates
    {
        NONE = -1,
        STANDBY,
        GO_TO_MINE,
        MINING,
        ESCAPE,
        WAITING_ENEMY
    }

    public GoldGathererStates m_CurrentState = GoldGathererStates.STANDBY;

    public UnityEngine.AI.NavMeshAgent m_NavMeshAgent;

    public float m_StandbyTime = 0.5f;
    public float m_RemainingStandbyTime = 0.0f;
    public float m_MiningTime = 1.0f;
    public float m_RemainingMiningTime = 0.0f;
    public float m_WaitingTime = 4f;
    public float m_RemainingWaitingTime = 0.0f;


    void Start()
    {
        m_NavMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        m_RemainingStandbyTime = m_StandbyTime;
        m_RemainingMiningTime = m_MiningTime;
        m_CurrentHealthPoints = m_MaxHealthPoints;
        m_RemainingWaitingTime = m_WaitingTime;
        m_Animator = GetComponent<Animator>();
        //Finds the base and the necessary points of each gatherer deppending on if they are spawned from the player or the enemy
        if (m_Allied)
        {
            m_Base = GameObject.Find("PlayerBaseBuild");
        }
        else
        {
            m_Base = GameObject.Find("EnemyBase");
        }
        if (m_GoldGatherer)
        {
            if(m_Allied)
            {
                m_GoldMine = GameObject.Find("GoldGatheringPoint").transform;
                m_GoldRunningPoint = GameObject.Find("GoldRunningPoint").transform;
            }
            else
            {
                m_GoldMine = GameObject.Find("GoldGatheringPointEnemy").transform;
                m_GoldRunningPoint = GameObject.Find("GoldRunningPointEnemy").transform;
            }
        }
        else
        {
            if(m_Allied)
            {
                m_GoldMine = GameObject.Find("MetalGatheringPoint").transform;
                m_GoldRunningPoint = GameObject.Find("MetalRunningPoint").transform;
            }else
            {
                m_GoldMine = GameObject.Find("MetalGatheringPointEnemy").transform;
                m_GoldRunningPoint = GameObject.Find("MetalRunningPointEnemy").transform;
            }
        }
        if(m_Allied)
        {
            m_BaseScript = m_Base.GetComponent<PlayerBaseController>();
        }
        else
        {
            m_EnemyBaseScript = m_Base.GetComponent<EnemyBaseController>();
        }
    }

    void Update()
    {
        float dt = Time.deltaTime;

        switch (m_CurrentState)
        {
            case GoldGathererStates.STANDBY:
                StandbyBehaviour(dt);
                break;
            case GoldGathererStates.GO_TO_MINE:
                if(CheckArrival())
                {
                    OnStateEnter(GoldGathererStates.MINING);
                }
                break;
            case GoldGathererStates.MINING:
                Mining(dt);
                break;
            case GoldGathererStates.ESCAPE:
                if (CheckArrival())
                {
                    OnStateEnter(GoldGathererStates.WAITING_ENEMY);
                }
                break;
            case GoldGathererStates.WAITING_ENEMY:
                if (m_RemainingWaitingTime > 0)
                {
                    m_RemainingWaitingTime -= dt;
                }
                else
                {
                    m_RemainingWaitingTime = m_WaitingTime;
                    m_Animator.SetBool("Enemy", false);
                    OnStateEnter(GoldGathererStates.GO_TO_MINE);
                }
                break;
        }
    }

    private void OnStateEnter(GoldGathererStates thisState)
    {
        switch (thisState)
        {
            case GoldGathererStates.GO_TO_MINE:
                m_Animator.SetBool("IsRuning", true);
                m_Animator.SetBool("IsMining", false);
                m_Animator.SetBool("Enemy",false);
                m_CurrentState = GoldGathererStates.GO_TO_MINE;
                m_NavMeshAgent.isStopped = false;
                m_NavMeshAgent.SetDestination(m_GoldMine.position);
                break;
            case GoldGathererStates.MINING:
                m_Animator.SetBool("IsRuning", false);
                m_Animator.SetBool("IsMining", true);
                m_NavMeshAgent.isStopped=true;
                m_CurrentState = GoldGathererStates.MINING;
                break;
            case GoldGathererStates.ESCAPE:
                m_CurrentState = GoldGathererStates.ESCAPE;
                m_Animator.SetBool("IsRuning", true);
                m_Animator.SetBool("Enemy", true);
                m_Animator.SetBool("IsMining", false);
                m_NavMeshAgent.isStopped = false;
                m_NavMeshAgent.SetDestination(m_GoldRunningPoint.position);
                break;
            case GoldGathererStates.WAITING_ENEMY:
                m_Animator.SetBool("IsRuning", false);
                m_Animator.SetBool("IsMining", false);
                m_NavMeshAgent.isStopped = true;
                m_CurrentState = GoldGathererStates.WAITING_ENEMY;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //It checks when an enemy is on their vision field to escape from them
        if (m_Allied)
        {
            if (other.CompareTag("Enemy") && !other.isTrigger)
            {
                OnStateEnter(GoldGathererStates.ESCAPE);
            }
        }
        else
        {
            if (other.CompareTag("Allied") && !other.isTrigger)
            {
                OnStateEnter(GoldGathererStates.ESCAPE);
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        m_ShellScript = collision.gameObject.GetComponent<ShellScript>();
        if(collision.gameObject.CompareTag("Shell"))
        {
            TakeDamage(m_ShellScript.m_Damage);
        }
    }

    private void StandbyBehaviour(float dt)
    {
        //When the gatherer is spawned, it stays in standby 0.5 seconds
        if (m_RemainingStandbyTime > 0)
        {
            m_RemainingStandbyTime -= dt;
        }
        else
        {
            m_RemainingStandbyTime = m_StandbyTime;
            OnStateEnter(GoldGathererStates.GO_TO_MINE);
        }
    }

    private void Mining(float dt)
    {
        if (m_RemainingMiningTime > 0)
        {
            m_RemainingMiningTime -= dt;
        }
        else
        {
            m_RemainingMiningTime = m_MiningTime;
            if (m_Allied)
            {
                m_BaseScript.GetResource(m_GoldGatherer);
            }else
            {
                m_EnemyBaseScript.GetResource(m_GoldGatherer);
            }
            
        }

    }
    private bool CheckArrival()
    {
        if (m_NavMeshAgent.path != null)
        {
            if (m_NavMeshAgent.remainingDistance < 0.1f)
            {
                return true;
            }
        }
        return false;
    }
    public void TakeDamage(int damage)
    {
        m_CurrentHealthPoints -= damage;
        if (m_CurrentHealthPoints <= 0)
        {
            m_Animator.SetBool("IsDead", true);
        }
    }

    private void DestroyObject()
    {
        Destroy (gameObject);
    }

    public bool IsDead()
    {
        //Returns true if the gatherer is dead
        if(m_CurrentHealthPoints < 0)
        {
            return true;
        }
        return false;
    }
    

}

