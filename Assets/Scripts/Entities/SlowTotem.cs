using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTotem : MonoBehaviour {

    public MeshRenderer slowPFX;
    public float fLifetime = 2.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        for(float fTest = 0.0f; fTest < 2.0f; fTest += 0.5f)
        {
            if (fLifetime > fTest && fLifetime - Time.deltaTime <= fTest)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, 10.0f);
                foreach (Collider collider in colliders)
                {
                    BasicEnemy enemy = collider.GetComponent<BasicEnemy>();
                    if (enemy != null)
                    {
                        enemy.Slow(2.0f);
                    }
                }
            }
        }

        fLifetime -= Time.deltaTime;
        slowPFX.transform.localEulerAngles = new Vector3(0.0f, 360.0f * fLifetime, 0.0f);
        slowPFX.material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - Mathf.Abs(fLifetime - 1.0f));

        if (fLifetime < 0.0f)
            Destroy(gameObject);
    }
}
