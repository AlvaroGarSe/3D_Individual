using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerBaseController : MonoBehaviour
{
    public int m_CurrentHealthPoints;
    public int m_MaxHealthPoints = 50;
    public int m_GoldAmount;
    public int m_MetalAmount;
    public Object m_GoldText;
    public Object m_MetalText;

    public Transform m_GoldGathererSpawn;
    public Transform m_HeavySoldierSpawn;
    public Transform m_MetalGathererSpawn;
    public Transform m_SoldierSpawn;

    public GameObject m_GoldGatherer;
    public GameObject m_HeavySoldier;
    public GameObject m_MetalGatherer;
    public GameObject m_Soldier;

    public Button m_GoldGathererButton;
    public Button m_HeavySoldierButton;
    public Button m_MetalGathererButton;
    public Button m_SoldierButton;

    private GathererController m_GoldGathererScript;
    private GathererController m_MetalGathererScript;
    public bool m_HasGoldGatherer;
    public bool m_HasMetalGatherer;
    private SoldierController m_SoldierScript;
    private SoldierController m_HeavySoldierScript;

    private ShellScript m_ShellScript;

    // Start is called before the first frame update
    void Start()
    {
        m_GoldGathererButton.onClick.AddListener(GoldGathererSpawner);
        m_HeavySoldierButton.onClick.AddListener (HeavySoldierSpawner);
        m_MetalGathererButton.onClick.AddListener(MetalGathererSpawner);
        m_SoldierButton.onClick.AddListener(SoldierSpawner);
        m_CurrentHealthPoints = m_MaxHealthPoints;
        m_HasGoldGatherer = false;
        m_HasMetalGatherer = false;

        //Each base starts with 10 gold and 10 metal
        m_GoldAmount = 10;
        m_MetalAmount = 10;
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        m_GoldText.GetComponent<TextMeshProUGUI>().text = m_GoldAmount.ToString();
        m_MetalText.GetComponent<TextMeshProUGUI>().text = m_MetalAmount.ToString();
        //It checks when the gold and metal gatherer is dead
        if (m_HasGoldGatherer && m_GoldGathererScript.m_CurrentHealthPoints <= 0) { m_HasGoldGatherer = false; }
        if(m_HasMetalGatherer && m_MetalGathererScript.m_CurrentHealthPoints <= 0) { m_HasMetalGatherer = false;}

        //When the player base has no health points the final scene is loaded showing a losing message
        if (m_CurrentHealthPoints <= 0)
        {
            SceneManager.LoadScene("EndSceneLose");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //When a bullet collides and is allied, make damage deppends on who shooted it
        if (collision.gameObject.CompareTag("Shell"))
        {
            m_ShellScript = collision.gameObject.GetComponent<ShellScript>();
            if(!m_ShellScript.m_AlliedBullet)
            {
                TakeDamage(m_ShellScript.m_Damage);
            }
        }
    }

    private void TakeDamage(int damage)
    {
        m_CurrentHealthPoints -= damage;
    }
    //All the spawner functions spawn a NPC when the player clicks on the button and when the player has the resources needed
    //When the NPC is spawned their tag and other parameters from their script are changed
    private void GoldGathererSpawner()
    {
        if (m_GoldAmount >= 1 && m_MetalAmount >= 5)
        {
            //If there is a goald gatherer you can not spawn another one
            if (!m_HasGoldGatherer)
            {
                m_GoldAmount -= 1;
                m_MetalAmount -= 5;
                GameObject newGold = Instantiate(m_GoldGatherer, m_GoldGathererSpawn.transform.position, m_GoldGathererSpawn.rotation);
                newGold.tag = "Allied";
                m_GoldGathererScript = newGold.GetComponent<GathererController>();
                m_GoldGathererScript.m_Allied = true;
                m_GoldGathererScript.m_GoldGatherer = true;
                m_HasGoldGatherer = true;
            }
        }
    }
    
    private void MetalGathererSpawner()
    {
        if (m_GoldAmount >= 5 && m_MetalAmount >= 1)
        {
            //If there is a metal gatherer you can not spawn another one
            if (!m_HasMetalGatherer)
            {
                m_GoldAmount -= 5;
                m_MetalAmount -= 1;
                GameObject newMetal = Instantiate(m_MetalGatherer, m_MetalGathererSpawn.transform.position, m_MetalGathererSpawn.rotation);
                newMetal.tag = "Allied";
                m_MetalGathererScript = newMetal.GetComponent<GathererController>();
                m_MetalGathererScript.m_Allied = true;
                m_MetalGathererScript.m_GoldGatherer = false;
                m_HasMetalGatherer = true;
            }
        }
    }
    private void SoldierSpawner()
    {
        if(m_GoldAmount >= 15 &&  m_MetalAmount >= 15)
        {
            m_GoldAmount -= 15;
            m_MetalAmount -= 15;
            GameObject newSoldier = Instantiate(m_Soldier, m_SoldierSpawn.transform.position, m_SoldierSpawn.rotation);
            newSoldier.tag = "Allied";
            m_SoldierScript = newSoldier.GetComponent<SoldierController>();
            m_SoldierScript.m_Allied = true;
        }
    }
    private void HeavySoldierSpawner()
    {
        if (m_GoldAmount >= 30 && m_MetalAmount >= 30)
        {
            m_GoldAmount -= 30;
            m_MetalAmount -= 30;
            GameObject newHeavySoldier = Instantiate(m_HeavySoldier, m_HeavySoldierSpawn.transform.position, m_HeavySoldierSpawn.rotation);
            newHeavySoldier.tag = "Allied";
            m_HeavySoldierScript = newHeavySoldier.GetComponent<SoldierController>();
            m_HeavySoldierScript.m_Allied = true;
        }
    }

    public void GetResource (bool IsGold)
    {
        if (IsGold)
        {
            m_GoldAmount++;
        }
        else { m_MetalAmount++;}
    }
}
