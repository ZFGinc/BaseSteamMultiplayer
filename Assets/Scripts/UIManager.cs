using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public bool IsMenu = true;

    public GameObject MenuCanvas;
    public GameObject PauseCanvas;
    public List<GameObject> DisablePanels = new();

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenPauseMenu();
        }
    }

    public void OpenMenuCanvas()
    {
        IsMenu = true;

        MenuCanvas.SetActive(true);
        PauseCanvas.SetActive(false);
    }
    
    public void CloseMenuCanvas()
    {
        IsMenu = false;

        MenuCanvas.SetActive(false);
        PauseCanvas.SetActive(false);
    }

    public void DefaultMenu()
    {
        foreach (GameObject panel in DisablePanels)
        {
            panel.SetActive(false);
        }
    }

    public void OpenPauseMenu()
    {
        if (IsMenu) return;

        PauseCanvas.SetActive(!PauseCanvas.activeSelf);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
