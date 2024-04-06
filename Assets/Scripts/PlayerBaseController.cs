using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseController : MonoBehaviour
{
    public int m_CurrentHealthPoints;
    public int m_MaxHealthPoints = 30;
    public int m_GoldAmount;
    public int m_MetalAmount;

    public object m_GoldGatherer;
    public object m_HeavySoldier;
    public object m_MetalGatherer;
    public object m_Soldier;

    
    
    
    // Start is called before the first frame update
    void Start()
    {
        m_CurrentHealthPoints = m_MaxHealthPoints;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
