using System;
[Serializable]
public class ResourceEntity
{
    public ResourceEntity(float in_xPos, float in_yPos, string in_resource, string in_UID)
    {
        xPos = in_xPos;
        yPos = in_yPos;
        UID = in_UID;
        resource = in_resource;
    }

    public float xPos;
    public float yPos;
    public string resource;
    public string UID;

}