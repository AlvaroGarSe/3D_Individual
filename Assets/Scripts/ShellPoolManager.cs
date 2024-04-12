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
        //There are created half of bullets for the allies and another half for the enemyes
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
        //A bullet is taken from the pool deppending on if its allied or not and given to the soldier that is going to shoot 
        foreach (GameObject shell in shellPool)
        {
            ScriptBullet = shell.GetComponent<ShellScript>();
            if (!shell.activeSelf && ScriptBullet.m_AlliedBullet == Allied)
            {
                ScriptBullet.m_Damage = damage;
                return shell;
            }
        }
        //If all the bullets are used in the time that a soldier is going to shoot, another one is created and returned to the soldier
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
        //Returns back the used bullets to the pool
        shell.SetActive(false);
    }
}
