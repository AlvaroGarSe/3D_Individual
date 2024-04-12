using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyBaseController : MonoBehaviour
{
    public int m_CurrentHealthPoints;
    public int m_MaxHealthPoints = 50;
    public float m_GoldAmount;
    public float m_MetalAmount;

    public Transform m_GoldGathererSpawn;
    public Transform m_HeavySoldierSpawn;
    public Transform m_MetalGathererSpawn;
    public Transform m_SoldierSpawn;

    public GameObject m_GoldGatherer;
    public GameObject m_HeavySoldier;
    public GameObject m_MetalGatherer;
    public GameObject m_Soldier;

    private GathererController m_GoldGathererScript;
    private GathererController m_MetalGathererScript;
    public bool m_HasGoldGatherer;
    public bool m_HasMetalGatherer;
    private SoldierController m_SoldierScript;
    private SoldierController m_HeavySoldierScript;
    public int m_SoldierCount;

    private ShellScript m_ShellScript;
    // Start is called before the first frame update
    void Start()
    {
        m_CurrentHealthPoints = m_MaxHealthPoints;
        m_HasGoldGatherer = false;
        m_HasMetalGatherer = false;
        m_GoldAmount = 10;
        m_MetalAmount = 10;
        m_SoldierCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //When there is no gold gatherer one is spawned
        if (!m_HasGoldGatherer && m_GoldAmount>=1 && m_MetalAmount >=5)
        {
            m_GoldAmount -= 1;
            m_MetalAmount -= 5;
            GameObject newGold = Instantiate(m_GoldGatherer, m_GoldGathererSpawn.transform.position, m_GoldGathererSpawn.rotation);
            newGold.tag = "Enemy";
            m_GoldGathererScript = newGold.GetComponent<GathererController>();
            m_GoldGathererScript.m_Allied = false;
            m_GoldGathererScript.m_GoldGatherer = true;
            m_HasGoldGatherer = true;
        }
        //When there is no metal gatherer one is spawned
        if (!m_HasMetalGatherer && m_GoldAmount >= 5 && m_MetalAmount >= 1)
        {
            m_GoldAmount -= 5;
            m_MetalAmount -= 1;
            GameObject newMetal = Instantiate(m_MetalGatherer, m_MetalGathererSpawn.transform.position, m_MetalGathererSpawn.rotation);
            newMetal.tag = "Enemy";
            m_MetalGathererScript = newMetal.GetComponent<GathererController>();
            m_MetalGathererScript.m_Allied = false;
            m_MetalGathererScript.m_GoldGatherer = false;
            m_HasMetalGatherer = true;
        }

        //It spawns 3 soliders firstly and then one heavy soldier through all the time when there are resources
        if (m_SoldierCount < 3)
        {
            if(m_GoldAmount >= 15 && m_MetalAmount >= 15)
            {
                m_GoldAmount -= 15;
                m_MetalAmount -= 15;
                GameObject newSoldier = Instantiate(m_Soldier, m_SoldierSpawn.transform.position, m_SoldierSpawn.rotation);
                newSoldier.tag = "Enemy";
                m_SoldierScript = newSoldier.GetComponent<SoldierController>();
                m_SoldierScript.m_Allied = false;
                m_SoldierCount++;
            }
        }else
        {
            if (m_GoldAmount >= 30 && m_MetalAmount >= 30)
            {
                m_GoldAmount -= 30;
                m_MetalAmount -= 30;
                GameObject newHeavySoldier = Instantiate(m_HeavySoldier, m_HeavySoldierSpawn.transform.position, m_HeavySoldierSpawn.rotation);
                newHeavySoldier.tag = "Enemy";
                m_HeavySoldierScript = newHeavySoldier.GetComponent<SoldierController>();
                m_HeavySoldierScript.m_Allied = false;
                m_SoldierCount = 0;
            }
        }

        //It checks when the gold and metal gatherer is dead
        if (m_HasGoldGatherer && m_GoldGathererScript.m_CurrentHealthPoints <= 0) { m_HasGoldGatherer = false; }
        if (m_HasMetalGatherer && m_MetalGathererScript.m_CurrentHealthPoints <= 0) { m_HasMetalGatherer = false; }

        //When the enemy base has no health points the final scene is loaded showing a winning message
        if (m_CurrentHealthPoints <=0)
        {
            SceneManager.LoadScene("EndSceneWin");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //When a bullet collides and is allied, make damage deppends on who shooted it
        if (collision.gameObject.CompareTag("Shell"))
        {
            m_ShellScript = collision.gameObject.GetComponent<ShellScript>();
            if (m_ShellScript.m_AlliedBullet)
            {
                TakeDamage(m_ShellScript.m_Damage);
            }
        }
    }
    private void TakeDamage(int damage)
    {
        m_CurrentHealthPoints -= damage;
    }

    public void GetResource(bool IsGold)
    {
        //The enemy base get resources a 25% less than the player's base
        if (IsGold)
        {
            m_GoldAmount+= 0.75f;
        }
        else { m_MetalAmount+= 0.75f; }
    }
}
