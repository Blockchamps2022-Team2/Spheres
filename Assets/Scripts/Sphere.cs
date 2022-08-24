using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    public static int count = 0;
    public static List<Sphere> instances = new List<Sphere>();
    public int id;
    public int level;
    public String spriteName;
    public String colour;
    public GameObject nextLevelPrefab;
    public Action<Sphere, List<Sphere>> OnLevelUp;
    public Action OnGameOver;
    private List<Sphere> contacts = new List<Sphere>();
    private Rigidbody2D rigid;
    private bool isTouchRedline;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        // Set Id
        id = count++;

        // Set sprite name
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteName = spriteRenderer.sprite.name;

        // Set colour name
        colour = spriteName.Split("-")[0];
        Debug.Log(colour);

        // Add to instances
        instances.Add(this);
    }


    // Update is called once per frame
    void Update()
    {
        if(isTouchRedline == false)
        {
            return;
        }
        timer += Time.deltaTime;
        if(timer > 3)
        {
            Debug.Log("Game Over");
            OnGameOver?.Invoke();
        }
    }

    // Helper method to set simulated
    public void SetSimulated(bool b)
    {
        if(rigid == null)
        {
            rigid = GetComponent<Rigidbody2D>();
        }
        rigid.simulated = b;
    }

    // Helper method to search for an id in contacts
    public bool IsContact(int id)
    {
        foreach(var c in contacts)
        {
            if(c.id == id)
            {
                return true;
            }
        }
        return false;
    }

    // When collides with another sphere
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var obj = collision.gameObject;
        var sphere = obj.GetComponent<Sphere>();
        if (obj.CompareTag("Sphere"))
        {
            if(obj.name == gameObject.name)
            {
                // Add to colliding sphere to contacts
                if (sphere.spriteName == spriteName && !IsContact(sphere.id))
                {
                    Debug.Log("Add to contact");
                    contacts.Add(sphere);
                }
                
                // If two spheres in contact, level up
                if (contacts.Count == 2 && nextLevelPrefab != null)
                {
                    OnLevelUp?.Invoke(this, contacts);
                }
            }
        }
    }

    // When leave collision with another sphere
    private void OnCollisionExit2D(Collision2D collision)
    {
        var obj = collision.gameObject;
        var sphere = obj.GetComponent<Sphere>();
        if (obj.CompareTag("Sphere"))
        {
            if (obj.name == gameObject.name)
            {
                // Remove from colliding sphere to contacts
                if (sphere.spriteName == spriteName && IsContact(sphere.id))
                {
                    Debug.Log("Remove from contact");
                    contacts.Remove(sphere);
                }
            }
        }
    }

    // When enter redline
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var obj = collision.gameObject;
        if (obj.CompareTag("Redline"))
        {
            Debug.Log("OnTriggerEnter2D Redline");
            isTouchRedline = true;
        }
    }

    // When leave redline
    private void OnTriggerExit2D(Collider2D collision)
    {
        var obj = collision.gameObject;
        if (obj.CompareTag("Redline"))
        {
            Debug.Log("OnTriggerExit2D Redline");
            isTouchRedline = false;
            timer = 0;
        }
    }

    public static void RemoveSphere(Sphere sphere)
    {
        for (int i = 0; i < Sphere.instances.Count; i++)
        {
            if (Sphere.instances[i].id == sphere.id)
            {
                Sphere.instances.Remove(sphere);
                Destroy(sphere.gameObject);
                return;
            }
        }
    }
}
