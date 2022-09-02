using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool PausedGame = false;
    public GameObject Menu;
    public GameObject Options;

    // Update is called once per frame
    private void Start()
    {
        Time.timeScale = 1f;

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PausedGame)
            {
                Resume();
            }
            else
            {
                Paused();
            }
        }
    }
    public void Resume()
    {
        Menu.SetActive(false);
        Options.SetActive(false);
        Time.timeScale = 1f;

        PausedGame = false;
    } 
    void Paused()
    {
        Menu.SetActive(true);
        Time.timeScale = 0f;
    PausedGame = true;
    }
}
