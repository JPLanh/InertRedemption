using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    public GameObject iconPrefab;

    List<TargetMarker> markers = new List<TargetMarker>();
    public RawImage compassImage;
    public Transform player;

    float compassUnit;

    // Start is called before the first frame update
    void Start()
    {
        compassUnit = compassImage.rectTransform.rect.width / 360f;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            compassImage.uvRect = new Rect(player.localEulerAngles.y / 360f, 0f, 1f, 1f);

            foreach (TargetMarker marker in markers)
            {
                marker.image.rectTransform.anchoredPosition = getPosOnCompass(marker);
            }
        }
    }

    public void addTarget (TargetMarker marker)
    {
        GameObject newMarker = Instantiate(iconPrefab, compassImage.transform);

        marker.image = newMarker.GetComponent<Image>();
        marker.image.sprite = marker.icon;
        marker.marker = newMarker;

        markers.Add(marker);
    }

    public void removeTarget(TargetMarker marker)
    {
        Destroy(marker.marker);
            markers.Remove(marker);
    }

    Vector2 getPosOnCompass(TargetMarker marker)
    {
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 playerFwd = new Vector2(player.transform.forward.x, player.transform.forward.z);

        float angle = Vector2.SignedAngle(marker.position - playerPos, playerFwd);

        return new Vector2(compassUnit * angle, 0f);
    }
}
