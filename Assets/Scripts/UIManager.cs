using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    [SerializeField]
    GameManager gameManager;
    
    public Text txt_distance;

    [SerializeField]
    GameObject UI_Game;

    [SerializeField]
    GameObject UI_GameOver;
    
    [SerializeField]
    GameObject UI_PauseMenu;

    private void Start()
    {
        UI_PauseMenu.SetActive(false);
        UI_GameOver.SetActive(false);
    }

    public void GameOver()
    {
        txt_distance.gameObject.SetActive(false);
        UI_GameOver.SetActive(true);

        UI_GameOver.transform.GetChild(4).GetChild(3).GetComponent<Text>().text = txt_distance.text;

        UI_GameOver.transform.GetChild(4).GetChild(5).GetComponent<Text>().text = gameManager.highScore.ToString();

    }
    public void OnClickPauseUnPause()
    {
        if (gameManager.gameState == GameManager.State.Running)
        {
            gameManager.gameState = GameManager.State.Pause;

            UI_Game.SetActive(false);
            UI_PauseMenu.SetActive(true);
        }
        else
        if (gameManager.gameState == GameManager.State.Pause)
        {
            gameManager.gameState = GameManager.State.Running;
            
            UI_Game.SetActive(true);
            UI_PauseMenu.SetActive(false);
            
            gameManager.UnPause();
        }
    }
    public void OnClickRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OnClickHome()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
