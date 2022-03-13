using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Affliction_Fear : MonoBehaviour
{
    public int fearLevel;
    public float timer;
    public int counter;

    public GameObject fearFX;
    public PlayerController lv_player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void init(int in_level, int in_counter)
    {
        fearLevel = in_level;
        counter = in_counter;
        resetTimer();

    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (!fearFX.activeInHierarchy)
        {
            if (timer < 0)
            {
                if (counter > 0)
                {
                    if (NetworkMain.Username.Equals(lv_player.name))
                        fearFX.SetActive(true);
                    lv_player.insanityLevel -= fearLevel;
                    counter -= 1;
                } else {
                    Destroy(gameObject); 
                }
            }
        } else
        {
            if (timer < -5)
            {

                if (NetworkMain.Username.Equals(lv_player.name))
                    fearFX.SetActive(false);
                resetTimer();
            }
        }
    }

    public void resetTimer()
    {
        timer = Random.Range(1f, fearLevel * 5f);

    }
}
