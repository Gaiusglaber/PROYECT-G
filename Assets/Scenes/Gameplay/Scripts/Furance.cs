using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furance : MonoBehaviour
{
    [SerializeField] private GameObject furancePanel;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ActivateFurancePanel();
        }
    }

    public void ActivateFurancePanel()
    {
        furancePanel.SetActive(true);
    }

    public void DisableFurancePanel()
    {
        furancePanel.SetActive(false);
    }
}
