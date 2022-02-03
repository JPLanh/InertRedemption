using System;
[Serializable]
public class ResourceEntity
{
    public ResourceEntity(float in_xPos, float in_yPos, string in_type, string in_UID)
    {
        xPos = in_xPos;
        yPos = in_yPos;
        UID = in_UID;
        type = in_type;
    }

    public float xPos;
    public float yPos;
    public string type;
    public string UID;

}