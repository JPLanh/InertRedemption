using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Text loadingText;
    public RawImage blackdrop;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loading(string in_loading)
    {
        loadingText.text = in_loading;
        blackdrop.color = new Color(0f, 0f, 0f, 1f);
        NetworkMain.isBroadcastable = false;
    }

    public void unload()
    {
        blackdrop.color = new Color(0f, 0f, 0f, 0f);
        NetworkMain.isBroadcastable = true;
    }
}
