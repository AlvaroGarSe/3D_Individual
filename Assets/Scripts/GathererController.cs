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

    public bool IsDead = false;

    public Animator m_Animator;

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


    // Start is called before the first frame update
    void Start()
    {
        m_NavMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        m_RemainingStandbyTime = m_StandbyTime;
        m_RemainingMiningTime = m_MiningTime;
        m_CurrentHealthPoints = m_MaxHealthPoints;
        m_RemainingWaitingTime = m_WaitingTime;
        m_Animator = GetComponent<Animator>();
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
            m_GoldMine = GameObject.Find("GoldGatheringPoint").transform;
            m_GoldRunningPoint = GameObject.Find("GoldRunningPoint").transform;
        }
        else
        {
            m_GoldMine = GameObject.Find("MetalGatheringPoint").transform;
            m_GoldRunningPoint = GameObject.Find("MetalRunningPoint").transform;
        }
        m_BaseScript = m_Base.GetComponent<PlayerBaseController>();
    }

    // Update is called once per frame
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
                m_CurrentState = GoldGathererStates.GO_TO_MINE;
                m_NavMeshAgent.isStopped = false;
                m_NavMeshAgent.SetDestination(m_GoldMine.position);
                break;
            case GoldGathererStates.MINING:
                m_Animator.SetBool("IsRuning", false);
                m_NavMeshAgent.isStopped=true;
                m_CurrentState = GoldGathererStates.MINING;
                break;
            case GoldGathererStates.ESCAPE:
                m_CurrentState = GoldGathererStates.ESCAPE;
                m_Animator.SetBool("IsRunning", true);
                m_Animator.SetBool("Enemy", true);
                m_NavMeshAgent.isStopped = false;
                m_NavMeshAgent.SetDestination(m_GoldRunningPoint.position);
                break;
            case GoldGathererStates.WAITING_ENEMY:
                m_NavMeshAgent.isStopped = true;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
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
        if(collision.gameObject.CompareTag("Shell"))
        {
            TakeDamage(1);
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
            m_BaseScript.GetResource(m_GoldGatherer);
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
        this.gameObject.SetActive(false);
        Destroy (gameObject);
    }

}

