using System.Collections;
using System.Collections.Generic;
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
        loadingUI.loading(in_loading);
        lead.GetComponent<CharacterController>().enabled = false;
    }

    public void deinitLoadingScreen()
    {
        loadingUI.unload();
        loadingUI.gameObject.SetActive(false);
        lead.GetComponent<CharacterController>().enabled = true;
    }
}
