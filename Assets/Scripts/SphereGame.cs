using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SphereGame : MonoBehaviour
{
    public List<Sphere> spherePrefabList;
    public List<SphereGroup> sphereGroupPrefabList;
    public Transform spawnPoint;
    public Button playButton;
    public Text scoreLabel;
    public int score;
    private Sphere sphere;
    private SphereGroup sphereGroup;
    private int sphereId;
    private bool isGameOver = false;
    
    // Start is called before the first frame update
    void Start()
    {
        SetScore(0);
        sphereGroup = SpawnSphereGroup();        
    }

    // Update is called once per frame
    void Update()
    {
        if(isGameOver)
        {
            return;
        }

        UpdateScore();

        if(Input.GetMouseButton(0))
        {
            var mousePos = Input.mousePosition;
            var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            var spherePos = new Vector3(worldPos.x, spawnPoint.position.y, 0);
            // sphereGroup.gameObject.transform.position = spherePos;
            sphereGroup.SetPosition(spherePos);
        }

        if(Input.GetMouseButtonUp(0))
        {
            var mousePos = Input.mousePosition;
            var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            var spherePos = new Vector3(worldPos.x, spawnPoint.position.y, 0);
            // sphereGroup.gameObject.transform.position = spherePos;
            sphereGroup.SetPosition(spherePos);
            sphereGroup.SetSimulated(true);

            sphereGroup = SpawnSphereGroup();
        }
    }

    private SphereGroup SpawnSphereGroup()
    {
        var rand = Random.Range(0, sphereGroupPrefabList.Count);
        var prefab = sphereGroupPrefabList[rand].gameObject;
        var sphereGroupObj = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        var sphereGroup = sphereGroupObj.GetComponent<SphereGroup>();

        sphereGroup.SpawnSphere = SpawnSphere;
        sphereGroup.Spawn();

        return sphereGroup;
    }

    private Sphere SpawnNextSphere()
    {
        var rand = Random.Range(0, spherePrefabList.Count);
        var prefab = spherePrefabList[rand].gameObject;
        var pos = spawnPoint.position;

        return SpawnSphere(prefab, pos);
    }

    private Sphere SpawnSphere(GameObject prefab, Vector3 pos)
    {
        var obj = Instantiate(prefab, pos, Quaternion.identity);
        var sphere = obj.GetComponent<Sphere>();
        sphere.SetSimulated(false);

        sphere.OnLevelUp = (connectingSphere, contacts) =>
        {
            if(IsSphereExist(connectingSphere) && IsSphereExist(contacts[0]) && IsSphereExist(contacts[1]))
            {
                // Get connecting sphere's position
                var pos = connectingSphere.gameObject.transform.position;

                // Remove spheres in contact
                var sphere1 = contacts[0];
                var sphere2 = contacts[1];
                Sphere.RemoveSphere(sphere1);
                Sphere.RemoveSphere(sphere2);

                // Remove connecting sphere
                Sphere.RemoveSphere(connectingSphere);

                // Spawn next level sphere
                var nextLevelSphere = SpawnSphere(connectingSphere.nextLevelPrefab, pos);
                nextLevelSphere.SetSimulated(true);

                UpdateScore();
            }
        };

        sphere.OnGameOver = () =>
        {
            if (isGameOver == true)
            {
                return;
            }
            OnGameOver();
        };

        return sphere;
    }

    private void OnGameOver()
    {
        isGameOver = true;
        playButton.gameObject.SetActive(true);

        for (int i = 0; i < Sphere.instances.Count; i++)
        {
            Sphere.instances[i].SetSimulated(false);
            Destroy(Sphere.instances[i].gameObject);
        }

        Sphere.instances.Clear();
    }

    public void Restart()
    {
        playButton.gameObject.SetActive(false);

        isGameOver = false;

        StartCoroutine(SpawnSphereGroupCoroutine());
    }

    private IEnumerator SpawnSphereGroupCoroutine()
    {
        yield return new WaitForSeconds(0.05f);
        sphereGroup = SpawnSphereGroup();
    }

    private bool IsSphereExist(Sphere s)
    {
        for (int i = 0; i < Sphere.instances.Count; i++)
        {
            if (Sphere.instances[i].id == s.id)
            {
                return true;
            }
        }
        return false;
    }

    // Method to calculate and update score
    private void UpdateScore()
    {
        var redScore = 0;
        var blueScore = 0;
        var greenScore = 0;
        var yellowScore = 0;

        // loop through list of sphere
        foreach(var sphere in Sphere.instances)
        {
            var colour = sphere.colour;
            var level = sphere.level;
            // if sphere is blue
            if(colour == "Blue")
            {
                blueScore += level;
            }
            // if sphere is red
            else if(colour == "Red")
            {
                redScore += level;
            }
            // if sphere is green
            else if(colour == "Green")
            {
                greenScore += level;
            }
            // if sphere is yellow
            else if(colour == "Yellow")
            {
                yellowScore += level;
            }
        }

        var totalScore = blueScore + redScore + greenScore + yellowScore - 3; // -3 because theres 3 not dropped

        SetScore(totalScore);
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreLabel.text = $"{this.score}";
    }
}
