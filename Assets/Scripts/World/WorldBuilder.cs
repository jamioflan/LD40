using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
    public WorldTile floor, hill;
    public WorldTile[,] worldTiles;
    public Material[] biomeMaterials = new Material[Biomes.iNUM_BIOMES];

	// Use this for initialization
	void Start ()
    {
        GenerateWorld(21, 21);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyUp(KeyCode.A))
            GenerateWorld(21, 21);
	}

    private void GenerateWorld(int width, int height)
    {
        //Delete old world
        if (worldTiles != null)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (worldTiles[i, j] != null)
                        Destroy(worldTiles[i, j].gameObject);
                }
            }
        }

        worldTiles = new WorldTile[width,height];

        // LET'S MAKE SOME NOISE!!!
        float[,] noise = new float[width, height];

        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                noise[i, j] = Random.Range(0.0f, 1.0f);
            }
        }

        bool[,] tiles = new bool[width, height];

        // Borders
        for(int i = 0; i < width; i++)
        {
            tiles[i, 0] = hill;
            tiles[i, height - 1] = hill;
        }

        for (int j = 0; j < height; j++)
        {
            tiles[0, j] = hill;
            tiles[width - 1, j] = hill;
        }

        // Fill randomly
        for (int i = 1; i < width - 1; i++)
        {
            for(int j = 1; j < height - 1; j++)
            {
                float fCumulative = 0.0f;
                for(int x = -1; x <= 1; x++)
                {
                    for(int y = -1; y <= 1; y++)
                    {                   
                        if (x != 0 || y != 0)
                            fCumulative += noise[i + x, j + y];
                    }
                }

                fCumulative *= 0.125f;

                tiles[i, j] = fCumulative > 0.5f;
            }
        }

        // TODO : Biome-y bsns
        Biome[,] biomes = new Biome[width, height];



        // Spawn tiles

        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                worldTiles[i, j] = Instantiate<WorldTile>(tiles[i,j] ? hill : floor, transform);
                worldTiles[i, j].transform.localPosition = new Vector3(i * 10.0f, 0.0f, j * 10.0f);

                // Setup walls
                if(tiles[i,j])
                {
                    worldTiles[i, j].walls[(int)Direction.UP].SetActive(j != height - 1 && !tiles[i, j + 1]);
                    worldTiles[i, j].walls[(int)Direction.RIGHT].SetActive(i != width - 1 && !tiles[i + 1, j]);
                    worldTiles[i, j].walls[(int)Direction.DOWN].SetActive(j != 0 && !tiles[i, j - 1]);
                    worldTiles[i, j].walls[(int)Direction.LEFT].SetActive(i != 0 && !tiles[i - 1, j]);
                }

            }
        }
    }
}
