using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyLair : MonoBehaviour {

    public BasicEnemy enemyTemplate;
    public int spawnSize;
    public Transform holder;
	// Use this for initialization
	void Start () {

	}

    public List<BasicEnemy> SpawnEnemies(float range = 15.0f, int n = -1)
    {
        if (n < 0)
            n = spawnSize;
        List<BasicEnemy> enemies = new List<BasicEnemy>();
        for (int i = 0; i < n; ++i)
        {
            enemies.Add(SpawnEnemy(range));
        }
        return enemies;
    }
    public BasicEnemy SpawnEnemy(float range = 15.0f)
    {
        /*
        NavMeshHit hit;
        while (!NavMesh.SamplePosition(new Vector3(transform.position.x + Random.Range(-range, range), 1, transform.position.z + Random.Range(-range, range)), out hit, 4.0f, NavMesh.AllAreas) )
        {
            continue;
        }
        BasicEnemy newEnemy = Instantiate<BasicEnemy>(enemyTemplate, hit.position + new Vector3(0, 3, 0), transform.rotation);
        */
        BasicEnemy newEnemy = Instantiate<BasicEnemy>(enemyTemplate, new Vector3(0, 0, 0), Quaternion.identity );
        newEnemy.transform.position = transform.position;
        newEnemy.lair = this;
        return newEnemy;
    }

    public void AddResource(CollectedResource resource)
    {
        resource.leader = holder;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
