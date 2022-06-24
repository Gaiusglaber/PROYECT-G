using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenObject : MonoBehaviour
{
    [SerializeField] private GameObject screenFeedback;
    [SerializeField] private UIScreenObject uiScreen;
    private bool playerIsNear;

    void Update()
    {
        if(playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            uiScreen.TogglePanel();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!screenFeedback.gameObject.activeSelf)
            {
                screenFeedback.gameObject.SetActive(true);
                playerIsNear = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (screenFeedback.gameObject.activeSelf)
            {
                screenFeedback.gameObject.SetActive(false);

                playerIsNear = false;
            }
        }
    }
}
