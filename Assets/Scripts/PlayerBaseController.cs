using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
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
    




    // Start is called before the first frame update
    void Start()
    {
        m_GoldGathererButton.onClick.AddListener(GoldGathererSpawner);
        m_HeavySoldierButton.onClick.AddListener (HeavySoldierSpawner);
        m_MetalGathererButton.onClick.AddListener(MetalGathererSpawner);
        m_SoldierButton.onClick.AddListener(SoldierSpawner);
        m_CurrentHealthPoints = m_MaxHealthPoints;
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        m_GoldText.GetComponent<TextMeshProUGUI>().text = m_GoldAmount.ToString();
        m_MetalText.GetComponent<TextMeshProUGUI>().text = m_MetalAmount.ToString();

        //m_MetalText.text = m_MetalAmount .ToString();
    }
    
    private void GoldGathererSpawner()
    {
        if (m_GoldAmount >= 1 && m_MetalAmount >= 5)
        {
            m_GoldAmount -= 1;
            m_MetalAmount -= 5;
            Instantiate(m_GoldGatherer, m_GoldGathererSpawn.transform.position, m_GoldGathererSpawn.rotation);
        }
    }
    
    private void MetalGathererSpawner()
    {
        if (m_GoldAmount >= 5 && m_MetalAmount >= 1)
        {
            m_GoldAmount -= 5;
            m_MetalAmount -= 1;
            Instantiate(m_MetalGatherer, m_MetalGathererSpawn.transform.position, m_MetalGathererSpawn.rotation);
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

        }
    }
    private void HeavySoldierSpawner()
    {
        if (m_GoldAmount >= 30 && m_MetalAmount >= 30)
        {
            m_GoldAmount -= 30;
            m_MetalAmount -= 30;
            Instantiate(m_HeavySoldier, m_HeavySoldierSpawn.transform.position, m_HeavySoldierSpawn.rotation);

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
