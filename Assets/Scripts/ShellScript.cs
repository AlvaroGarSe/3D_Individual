using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellScript : MonoBehaviour
{
    public float lifeTime =5f;

    public bool m_AlliedBullet;
    public int m_Damage;

    public ShellPoolManager m_ShellPoolManager;

    private void Awake()
    {
        m_ShellPoolManager = FindObjectOfType<ShellPoolManager>();
    }
    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            m_ShellPoolManager.ReturnShell(gameObject);
        }
    }

    private void OnEnable()
    {
        lifeTime = 5f;
        if (!m_AlliedBullet)
        {
            //Turns enmy bullets into Red and the allied into Blue
            this.GetComponent<Renderer>().material.color = Color.red;
        }
        else { this.GetComponent<Renderer>().material.color = Color.blue; }
    }

    private void OnCollisionEnter(Collision other)
    {
            if (m_AlliedBullet)
            {
                if (other.gameObject.CompareTag("Enemy"))
                {

                    m_ShellPoolManager.ReturnShell(gameObject);
                }
            }
            else
            {
                if (other.gameObject.CompareTag("Allied"))
                {
                    m_ShellPoolManager.ReturnShell(gameObject);
                }
            }
        
    }
    public void AlliedBullet(bool Allied)
    {
        m_AlliedBullet = Allied;
    }
}
