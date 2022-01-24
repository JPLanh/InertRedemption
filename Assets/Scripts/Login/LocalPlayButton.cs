using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LocalPlayButton : MonoBehaviour
{
    public Text username;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClick()
    {
        NetworkMain.Username = username.text;
        NetworkMain.UserID = "Local";
        NetworkMain.local = true;
        SceneManager.LoadScene("mainScene");
        //        SceneManager.LoadScene("mainScene");
    }
}
