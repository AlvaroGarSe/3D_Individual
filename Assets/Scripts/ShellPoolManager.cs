using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellPoolManager : MonoBehaviour
{
    public List<GameObject> shellPool = new List<GameObject>();
    public ShellScript ScriptBullet;

    public GameObject m_ShellPrefab;
    public int m_PoolSize;

    private void Awake()
    {
        for (int i = 0; i < m_PoolSize; i++)
        {
            GameObject shell = Instantiate(m_ShellPrefab);
            shell.SetActive(false);
            ScriptBullet = shell.GetComponent<ShellScript>();
            if (i < m_PoolSize / 2)
            {
                ScriptBullet.AlliedBullet(true);

            } else
            {
                ScriptBullet.AlliedBullet(false);
            }
            shellPool.Add(shell);
        }
    }

    public GameObject TakeShell(bool Allied, int damage)
    {
        foreach (GameObject shell in shellPool)
        {
            ScriptBullet = shell.GetComponent<ShellScript>();
            if (!shell.activeSelf && ScriptBullet.m_AlliedBullet == Allied)
            {
                ScriptBullet.m_Damage = damage;
                return shell;
            }
        }

        GameObject newShell = Instantiate(m_ShellPrefab);
        newShell.SetActive(false);
        ScriptBullet = newShell.GetComponent<ShellScript>();
        ScriptBullet.m_AlliedBullet = Allied;
        ScriptBullet.m_Damage = damage;
        shellPool.Add(newShell);
        return newShell;
    }

    public void ReturnShell(GameObject shell)
    {
        shell.SetActive(false);
    }
}
