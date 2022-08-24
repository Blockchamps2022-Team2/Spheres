using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SphereGroup : MonoBehaviour
{
    
    public Sphere firstSphere;
    public Sphere secondSphere;
    public Sphere thirdSphere;
    
    public Vector3 firstOffset;
    public Vector3 secondOffset;
    public Vector3 thirdOffset;

    public Vector3 groupOffset;
    public float minX;
    public float maxX;

    public List<Sphere> spherePrefabList;
    public Func<GameObject, Vector3, Sphere> SpawnSphere;
    private List<Sphere> contacts = new List<Sphere>();

    // Start is called before the first frame update
    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
    }

    public void Spawn()
    {
        firstOffset = firstSphere.transform.position - transform.position;
        secondOffset = secondSphere.transform.position - transform.position;
        thirdOffset = thirdSphere.transform.position - transform.position;

        firstSphere = ReplaceSphere(firstSphere);
        secondSphere = ReplaceSphere(secondSphere);
        thirdSphere = ReplaceSphere(thirdSphere);

        SetPosition(transform.position);
    }

    private Sphere ReplaceSphere(Sphere sphere)
    {
        var spherePos = sphere.gameObject.transform.position;

        var rand = UnityEngine.Random.Range(0, spherePrefabList.Count);
        var prefab = spherePrefabList[rand].gameObject;

        Destroy(sphere.gameObject);

        return SpawnSphere(prefab, spherePos);
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position + groupOffset;
        if (transform.position.x < minX)
        {
            transform.position = new Vector3(minX, transform.position.y, transform.position.z);
        } else if (transform.position.x > maxX)
        {
            transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
        }

        firstSphere.transform.position = transform.position + firstOffset;
        secondSphere.transform.position = transform.position + secondOffset;
        thirdSphere.transform.position = transform.position + thirdOffset;
    }

    // Helper method to set simulated
    public void SetSimulated(bool b)
    {
        firstSphere.SetSimulated(b);
        secondSphere.SetSimulated(b);
        thirdSphere.SetSimulated(b);
    }
}
