using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellPoolManager : MonoBehaviour
{
    public List<GameObject> shellPool = new List<GameObject>();

    public GameObject m_ShellPrefab;
    public int m_PoolSize;

    private void Awake()
    {
        for (int i = 0; i < m_PoolSize; i++)
        {
            GameObject shell = Instantiate(m_ShellPrefab);
            shell.SetActive(false);
            shellPool.Add(shell);
        }
    }

    public GameObject TakeShell()
    {
        foreach (GameObject shell in shellPool)
        {
            if (!shell.activeSelf)
            {
                return shell;
            }
        }

        GameObject newShell = Instantiate(m_ShellPrefab);
        newShell.SetActive(false);
        shellPool.Add(newShell);
        return newShell;
    }

    public void ReturnShell(GameObject shell)
    {
        shell.SetActive(false);
    }
    
    
}
