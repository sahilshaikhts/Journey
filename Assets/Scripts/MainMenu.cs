using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject ui_credit;
    [SerializeField] GameObject ui_menu;

    private void Start()
    {
            
    }
    public void OnClickPlay()
    {
        SceneManager.LoadScene("Main");
    }

    public void OnClickCredit()
    {
        ui_credit.SetActive(true);
        ui_menu.SetActive(false);
    }

    public void OnClickExitCredit()
    {
        ui_credit.SetActive(false);
        ui_menu.SetActive(true);
    }

    public void OnClickExit()
    {
        Application.Quit();
    }

}
