using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
    public WorldTile floor, hill, edge, home;
    public PlayerBehaviour playerPrefab;
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
        int iBestHomeX = 0, iBestHomeY = 0;
        float fBestHomeScore = 10000000.0f;

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
                worldTiles[i, j].transform.localPosition = new Vector3(i * 10.0f, 0.0f, j * 10.0f);

                // Setup walls
                switch (tiles[i, j])
                {
                    case 0:
                    {
                        if (i == 0 || i == width - 1)
                            break;
                        if (j == 0 || j == height - 1)
                            break;

                        bool bNaff = false;
                        for(int x = -1; x <= 1; x++)
                        {
                            for(int y = -1; y <= 1; y++)
                            {
                                if (tiles[x + i, y + j] > 0)
                                   bNaff = true;
                            }
                        }
                        if (bNaff)
                            break;

                        float fPerlinScore = Mathf.PerlinNoise(fNoiseOffsetX + fHillNoiseScale * (float)i / (float)width, fNoiseOffsetY + fHillNoiseScale * (float)j / (float)height);
                        float fDistFromCentre = new Vector3(i - width / 2, 0.0f, j - height / 2).magnitude;
                        float fScore = (fPerlinScore + 1.0f) * (fDistFromCentre + 1.0f);
                        if (fScore < fBestHomeScore)
                        {
                            fBestHomeScore = fScore;
                            iBestHomeX = i;
                            iBestHomeY = j;
                        }
                        break;
                    }
                    case 1:
                    {
                        worldTiles[i, j].walls[(int)Direction.UP].SetActive(j != height - 1 && tiles[i, j + 1] == 0);
                        worldTiles[i, j].walls[(int)Direction.RIGHT].SetActive(i != width - 1 && tiles[i + 1, j] == 0);
                        worldTiles[i, j].walls[(int)Direction.DOWN].SetActive(j != 0 && tiles[i, j - 1] == 0);
                        worldTiles[i, j].walls[(int)Direction.LEFT].SetActive(i != 0 && tiles[i - 1, j] == 0);
                        break;
                    }
                    case 2:
                    {
                        worldTiles[i, j].walls[(int)Direction.UP].SetActive(j != height - 1 && tiles[i, j + 1] < 2);
                        worldTiles[i, j].walls[(int)Direction.RIGHT].SetActive(i != width - 1 && tiles[i + 1, j] < 2);
                        worldTiles[i, j].walls[(int)Direction.DOWN].SetActive(j != 0 && tiles[i, j - 1] < 2);
                        worldTiles[i, j].walls[(int)Direction.LEFT].SetActive(i != 0 && tiles[i - 1, j] < 2);
                        break;
                    }
                }

                // Setup biome
                worldTiles[i, j].floorMesh.material = biomeMaterials[(int)biomes[i, j]];
            }
        }

        Destroy(worldTiles[iBestHomeX, iBestHomeY].gameObject);
        worldTiles[iBestHomeX, iBestHomeY] = Instantiate<WorldTile>(home, transform);
        worldTiles[iBestHomeX, iBestHomeY].transform.localPosition = new Vector3(-width * 5.0f + iBestHomeX * 10.0f, 0.0f, -height * 5.0f + iBestHomeY * 10.0f);

        PlayerBehaviour player = Instantiate<PlayerBehaviour>(playerPrefab);
        player.transform.position = new Vector3(-width * 5.0f + iBestHomeX * 10.0f + 5.0f, 2.0f, -height * 5.0f + iBestHomeY * 10.0f + 5.0f);
    }
}
