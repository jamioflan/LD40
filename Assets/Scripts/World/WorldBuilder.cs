using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
    public WorldTile floor, hill, edge;
    public WorldTile[,] worldTiles;
    public Material[] biomeMaterials = new Material[Biomes.iNUM_BIOMES];
    public int iWidth = 40, iHeight = 40;
    public float fHillNoiseScale = 9.57f;
    public float fBiomeNoiseScale = 3.81f;

	void Start ()
    {
        GenerateWorld(iWidth, iHeight);
    }
	
	void Update ()
    {
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

        int[,] tiles = new int[width, height];

        // Borders
        for(int i = 0; i < width; i++)
        {
            tiles[i, 0] = 2;
            tiles[i, height - 1] = 2;
        }

        for (int j = 0; j < height; j++)
        {
            tiles[0, j] = 2;
            tiles[width - 1, j] = 2;
        }

        float fNoiseOffsetX = Random.Range(-10000.0f, 10000.0f);
        float fNoiseOffsetY = Random.Range(-10000.0f, 10000.0f);

        // Fill randomly
        for (int i = 1; i < width - 1; i++)
        {
            for(int j = 1; j < height - 1; j++)
            {
                tiles[i, j] = Mathf.PerlinNoise(fNoiseOffsetX + fHillNoiseScale * (float)i / (float)width, fNoiseOffsetY + fHillNoiseScale * (float)j / (float)height) > 0.5f ? 1 : 0;
            }
        }

        // Biome-y bsns
        Biome[,] biomes = new Biome[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float fBiomeSeed = Mathf.PerlinNoise(fNoiseOffsetX + fBiomeNoiseScale * (float)i / (float)width, fNoiseOffsetY + fBiomeNoiseScale * (float)j / (float)height);
                if (fBiomeSeed > 0.7f)
                    biomes[i, j] = Biome.SAND;
                else if (fBiomeSeed > 0.4f)
                    biomes[i, j] = Biome.CHEESE;
                else if (fBiomeSeed > 0.1f)
                    biomes[i, j] = Biome.CRUMPETS;
                else biomes[i, j] = Biome.SHINYLAND;
            }
        }


        // Spawn tiles
        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                WorldTile tile = null;
                switch(tiles[i, j])
                {
                    case 0: tile = floor; break;
                    case 1: tile = hill; break;
                    case 2: tile = edge; break;
                }
                worldTiles[i, j] = Instantiate<WorldTile>(tile, transform);
                worldTiles[i, j].transform.localPosition = new Vector3(-width * 5.0f + i * 10.0f, 0.0f, -height * 5.0f + j * 10.0f);

                // Setup walls
                if(tiles[i,j] == 1)
                {
                    worldTiles[i, j].walls[(int)Direction.UP].SetActive(j != height - 1 && tiles[i, j + 1] == 0);
                    worldTiles[i, j].walls[(int)Direction.RIGHT].SetActive(i != width - 1 && tiles[i + 1, j] == 0);
                    worldTiles[i, j].walls[(int)Direction.DOWN].SetActive(j != 0 && tiles[i, j - 1] == 0);
                    worldTiles[i, j].walls[(int)Direction.LEFT].SetActive(i != 0 && tiles[i - 1, j] == 0);
                }
                else if (tiles[i, j] == 2)
                {
                    worldTiles[i, j].walls[(int)Direction.UP].SetActive(j != height - 1 && tiles[i, j + 1] < 2);
                    worldTiles[i, j].walls[(int)Direction.RIGHT].SetActive(i != width - 1 && tiles[i + 1, j] < 2);
                    worldTiles[i, j].walls[(int)Direction.DOWN].SetActive(j != 0 && tiles[i, j - 1] < 2);
                    worldTiles[i, j].walls[(int)Direction.LEFT].SetActive(i != 0 && tiles[i - 1, j] < 2);
                }

                // Setup biome
                worldTiles[i, j].floorMesh.material = biomeMaterials[(int)biomes[i, j]];
            }
        }
    }
}
