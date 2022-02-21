using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerCanvas : MonoBehaviour
{
//    public GameObject userInterface;
    public GameObject crosshair;
    public GridSystem gridSystem;
    public ToastNotifications toast;
    public Compass playerCompass;
    public TimeSystem timeSystem;
    public GameObject lead;

    public LoadingScreen loadingUI;
    public LightIndicator lightIndicator;
    public LightIndicator ammoIndicator;
    public LightIndicator energyIndicator;

    public int countDownTimer = 10;
    private string displayText;

    public bool shop = false;

    // Start is called before the first frame update
    void Start()
    {
        //downloadButton = GameObject.Find("Download");
        //downloadButton = GameObject.Find("Upload");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void enableShop()
    {
        //downloadButton.SetActive(shop);
        //uploadButton.SetActive(shop);
    }

    public void initLoadingScreen(string in_loading)
    {
        loadingUI.gameObject.SetActive(true);
        displayText = in_loading;
        loadingUI.loading(in_loading);
        lead.GetComponent<CharacterController>().enabled = false;
    }

    public void deinitLoadingScreen()
    {
        loadingUI.unload();
        loadingUI.gameObject.SetActive(false);
        lead.GetComponent<CharacterController>().enabled = true;
    }
    public void gameOver()
    {
        StartCoroutine(countDown());
    }

    IEnumerator countDown()
    {
        while (true)
        {
            loadingUI.loading(displayText + " ... " + countDownTimer);

            if (countDownTimer == 0)
            {
                Dictionary<string, string> payload = new Dictionary<string, string>();
                payload["Action"] = "Leave Lobby";
                payload["Name"] = NetworkMain.LobbyID;
                payload["Type"] = "Action";
                NetworkMain.serverAction(payload);
                Cursor.lockState = CursorLockMode.None;
                NetworkMain.payloadStack.Clear();
                EntityManager.survivors.Clear();
                EntityManager.virus.Clear();
                EntityManager.players.Clear();
                SceneManager.LoadScene("Lobby");
            }
            countDownTimer -= 1;
            yield return new WaitForSeconds(1);
        }

    }
}
