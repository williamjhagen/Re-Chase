using UnityEngine;
using System.Collections;

public class FightCam : MonoBehaviour
{
    public Character c1;
    public Character c2;
    private GameObject cam;
    // Use this for initialization
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 temp = cam.transform.position;
        temp.x = FindMidpoint(c1.transform.position, c2.transform.position);
        cam.transform.position = temp;
    }

    private float DistanceApart()
    {
        return Vector2.Distance(c1.transform.position, c2.transform.position);
    }

    private float FindMidpoint(Vector2 v1, Vector2 v2)
    {
        return (v1.x + v2.x) / 2f;
    }
}
