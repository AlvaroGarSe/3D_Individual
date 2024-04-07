using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellScript : MonoBehaviour
{
    public float lifeTime = 10f;

    public ShellPoolManager m_ShellPoolManager;

    public GameObject m_ExplossionEffect;

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
        lifeTime = 10f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<SoldierController>())
        {
            collision.gameObject.GetComponent<SoldierController>().TakeDamage(2);
        }
        m_ShellPoolManager.ReturnShell(gameObject);
    }
}
