using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoldierController;

public class GoldGathererController : MonoBehaviour
{
    public int m_MaxHealthPoints;
    public int m_CurrentHealthPoints;

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
        SELECT_AND_GO_TO_MINE,
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


    // Start is called before the first frame update
    void Start()
    {
        m_NavMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        m_RemainingStandbyTime = m_StandbyTime;
        m_RemainingMiningTime = m_MiningTime;
        m_CurrentHealthPoints = m_MaxHealthPoints;
        m_Animator = GetComponent<Animator>();
        m_Base = GameObject.Find("PlayerBaseBuild");
        m_GoldMine = GameObject.Find("GoldGatheringPoint").transform;
        m_GoldRunningPoint = GameObject.Find("GoldRunningPoint").transform;
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
            case GoldGathererStates.SELECT_AND_GO_TO_MINE:
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
                
                break;
        }
    }

    private void OnStateEnter(GoldGathererStates thisState)
    {
        switch (thisState)
        {
            case GoldGathererStates.STANDBY:

                break;
            case GoldGathererStates.SELECT_AND_GO_TO_MINE:
                m_Animator.SetBool("IsWalking", true);
                m_CurrentState = GoldGathererStates.SELECT_AND_GO_TO_MINE;
                m_NavMeshAgent.isStopped = false;
                m_NavMeshAgent.SetDestination(m_GoldMine.position);
                break;
            case GoldGathererStates.MINING:
                m_Animator.SetBool("IsWalking", false);
                m_Animator.SetBool("IsWalking", false);
                m_Animator.SetBool("IsWalking", false);
                break;
            case GoldGathererStates.ESCAPE:
                m_CurrentState = GoldGathererStates.ESCAPE;
                m_Animator.SetBool("IsWalking", false);
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

    private void StandbyBehaviour(float dt)
    {
        if (m_RemainingStandbyTime > 0)
        {
            m_RemainingStandbyTime -= dt;
        }
        else
        {
            m_RemainingStandbyTime = m_StandbyTime;
            OnStateEnter(GoldGathererStates.SELECT_AND_GO_TO_MINE);
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Enemy")
        {
            OnStateEnter(GoldGathererStates.ESCAPE);
        }
    }

    private void GoldMinned()
    {
        m_BaseScript.m_GoldAmount += 1;
    }

    private void DestroyObject()
    {
        this.gameObject.SetActive(false);
        Destroy (gameObject);
    }

}

