using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSensors : MonoBehaviour
{

    //Sensor

    public float sensorDistance = 50f;
    public float angle = 30f;
    public float height = 1.0f;
    public Color meshColor = Color.red;

    public int scanFrequency = 30;
    public LayerMask layers;

    List<GameObject> detectedObject = new List<GameObject>();
    Collider[] detected = new Collider[50];
    Mesh mesh;
    int scanCount;
    float scanInterval;
    float scanTimer;

    public GameObject focusTarget = null;
    float refreshFocusTimer;
    public float focusLength;

    // Start is called before the first frame update
    void Start()
    {
        mesh = CreateWedgeMesh();
        scanInterval = 1.0f / scanFrequency;
    }

    // Update is called once per frame
    void Update()
    {
        scanTimer -= Time.deltaTime;
        if (scanTimer < 0)
        {
            scanTimer += scanInterval;
            Scan();
        }
    }


    Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh();

        int segments = 10;
        int numOfTriangle = (segments * 4) + 2 + 2;
        int numOfVertices = numOfTriangle * 3;

        Vector3[] vertices = new Vector3[numOfVertices];
        int[] triangles = new int[numOfVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * sensorDistance;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * sensorDistance;

        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;

        int vert = 0;

        // left side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        // Right side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -angle;
        float deltaAngle = (angle * 2) / segments;
        for (int i = 0; i < segments; ++i)
        {

            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * sensorDistance;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * sensorDistance;

            topLeft = bottomLeft + Vector3.up * height;
            topRight = bottomRight + Vector3.up * height;

            // Far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            //Top
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            //Botttom
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;

            currentAngle += deltaAngle;

        }


        for (int i = 0; i < numOfVertices; i++)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    private void OnValidate()
    {
        mesh = CreateWedgeMesh();
        scanInterval = 1.0f / scanFrequency;
    }

    private void OnDrawGizmos()
    {
        if (mesh)
        {
            Gizmos.color = meshColor;
            Gizmos.DrawMesh(mesh, transform.position + new Vector3(0f, 0f, -4f), transform.rotation);
        }

        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, sensorDistance);
        //for (int i = 0; i < scanCount; ++i)
        //{
        //    Gizmos.DrawSphere(detected[i].transform.position, 1f);
        //}

        //Gizmos.color = Color.green;
        //foreach (var obj in detectedObject)
        //{
        //    Gizmos.DrawSphere(obj.transform.position, 1f);
        //}
    }

    //private void Scan()
    //{
    //    scanCount = Physics.OverlapSphereNonAlloc(transform.position, sensorDistance, detected, layers, QueryTriggerInteraction.Collide);
    //    detectedObject.Clear();
    //    for (int i = 0; i < scanCount; ++i)
    //    {
    //        GameObject obj = detected[i].gameObject;
    //        if (IsInSight(obj))
    //        {
    //            detectedObject.Add(obj);
    //        }
    //    }
    //}

    private void Scan()
    {
        scanCount = Physics.OverlapSphereNonAlloc(transform.position, sensorDistance, detected, layers, QueryTriggerInteraction.Collide);
        if (scanCount == 0)
        {
            focusTarget = null;
        }
        else
        {
                focusTarget = null;
            for (int i = 0; i < scanCount; ++i)
            {
                GameObject obj = detected[i].gameObject;
                if (IsInSight(obj))
                {
                    focusTarget = obj;
                }
            }
        }
    }

    public List<GameObject> getDetected()
    {
        return detectedObject;
    }
    private bool IsInSight(GameObject obj)
    {
        Vector3 origin = transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 dir = dest - origin;
        if (dir.y < 0 || dir.y > height)
        {
            return false;
        }
        dir.y = 0;
        float deltaAngle = Vector3.Angle(dir, transform.forward);
        if (deltaAngle > angle)
        {
            return false;
        }

        origin.y += height / 2;
        dest.y = origin.y;
        if (Physics.Linecast(origin, dest, out RaycastHit hit, layers))
            return hit.transform.tag == "Survivors";
        return false;
    }

}
