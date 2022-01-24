using UnityEngine;
using UnityEngine.UI;

public class TargetMarker : MonoBehaviour
{
    public Sprite icon;
    public Image image;
    public GameObject marker;

    public Vector2 position
    {
        get { return new Vector2(transform.position.x, transform.position.z); }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
