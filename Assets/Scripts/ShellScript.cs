using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        //Turns enemy bullets into Red and the allied into Blue
        if (!m_AlliedBullet)
        {
            this.GetComponent<Renderer>().material.color = Color.red;
        }
        else { this.GetComponent<Renderer>().material.color = Color.blue; }
    }

    private void OnCollisionEnter(Collision other)
    {

        //If the bullet collides with the target it dissappear and return to the pool deppending on who shooted it
        if (m_AlliedBullet)
        {
            if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("EnemyBase"))
            {
                m_ShellPoolManager.ReturnShell(gameObject);
            }
        }
        else
        {
            if (other.gameObject.CompareTag("Allied") || other.gameObject.CompareTag("AlliedBase"))
            {
                m_ShellPoolManager.ReturnShell(gameObject);
            }
        }        
    }
    public void AlliedBullet(bool Allied)
    {
        //Turns the bullet into allied or not 
        m_AlliedBullet = Allied;
    }
}
