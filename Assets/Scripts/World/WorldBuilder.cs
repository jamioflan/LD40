using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorldBuilder : MonoBehaviour
{
    [System.Serializable]
    public class eurgh
    {
        public List<GameObject> whyDoIHaveToDoThisUnity = new List<GameObject>();
    }

    public WorldTile floor, hill, edge, home;
    public eurgh[] trash = new eurgh[Biomes.iNUM_BIOMES];
    public eurgh[] sweetLoot = new eurgh[Biomes.iNUM_BIOMES];
    public PlayerBehaviour playerPrefab;
    public WorldTile[,] worldTiles;
    public WorldTile theBase;
    public Material[] biomeMaterials = new Material[Biomes.iNUM_BIOMES];
    public int width = 40, height = 40;
    public float fHillNoiseScale = 9.57f;
    public float fBiomeNoiseScale = 3.81f;

    public BasicEnemy enemyPrefab;

	void Start ()
    {
    }
	
	void Update ()
    {
	}

    public void GenerateWorld()
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

        float[,] tiles = new float[width, height];

        // Borders
        for(int i = 0; i < width; i++)
        {
            tiles[i, 0] = 2.0f;
            tiles[i, height - 1] = 2.0f;
        }

        for (int j = 0; j < height; j++)
        {
            tiles[0, j] = 2.0f;
            tiles[width - 1, j] = 2.0f;
        }

        float fNoiseOffsetX = Random.Range(-10000.0f, 10000.0f);
        float fNoiseOffsetY = Random.Range(-10000.0f, 10000.0f);

        // Fill randomly
        for (int i = 1; i < width - 1; i++)
        {
            for(int j = 1; j < height - 1; j++)
            {
                tiles[i, j] = Mathf.PerlinNoise(fNoiseOffsetX + fHillNoiseScale * (float)i / (float)width, fNoiseOffsetY + fHillNoiseScale * (float)j / (float)height);
            }
        }

        float fLowestPerlinScore = 1.0f;
        float fHighestPerlinScore = 0.0f;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
               float fPerlin = Mathf.PerlinNoise(fNoiseOffsetX + fBiomeNoiseScale * (float)i / (float)width, fNoiseOffsetY + fBiomeNoiseScale * (float)j / (float)height);
                if (fPerlin < fLowestPerlinScore)
                    fLowestPerlinScore = fPerlin;
                if (fPerlin > fHighestPerlinScore)
                    fHighestPerlinScore = fPerlin;
            }
        }

        fLowestPerlinScore -= 0.1f;
        fHighestPerlinScore += 0.1f;

        // Biome-y bsns
        List<KeyValuePair<int, int>>[] tilesByBiome = new List<KeyValuePair<int, int>>[Biomes.iNUM_BIOMES];
        for (int i = 0; i < Biomes.iNUM_BIOMES; i++)
        {
            tilesByBiome[i] = new List<KeyValuePair<int, int>>();
        }

        Biome[,] biomes = new Biome[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float fBiomeSeed = Mathf.PerlinNoise(fNoiseOffsetX + fBiomeNoiseScale * (float)i / (float)width, fNoiseOffsetY + fBiomeNoiseScale * (float)j / (float)height);
                fBiomeSeed = fLowestPerlinScore + (fHighestPerlinScore - fLowestPerlinScore) * fBiomeSeed;
                if (fBiomeSeed > 0.7f)
                    biomes[i, j] = Biome.SAND;
                else if (fBiomeSeed > 0.4f)
                    biomes[i, j] = Biome.CHEESE;
                else if (fBiomeSeed > 0.1f)
                    biomes[i, j] = Biome.CRUMPETS;
                else biomes[i, j] = Biome.SHINYLAND;

                tilesByBiome[(int)biomes[i, j]].Add(new KeyValuePair<int, int>(i, j));
            }
        }




        // Spawn tiles
        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                WorldTile tile = null;
                if(tiles[i, j] > 1.0f)
                    tile = edge;
                else tile = tiles[i, j] > 0.5f ? hill : floor;

                worldTiles[i, j] = Instantiate<WorldTile>(tile, transform);
                worldTiles[i, j].transform.localPosition = new Vector3(-width * 5.0f + i * 10.0f, 0.0f, -height * 5.0f + j * 10.0f);

                float fFloorHeight = 0.0f;

                // Setup walls
                if (tiles[i, j] > 1.0f) // Edge
                {
                    fFloorHeight = 1000.0f;
                    worldTiles[i, j].walls[(int)Direction.UP].SetActive(j != height - 1 && tiles[i, j + 1] < 2.0f);
                    worldTiles[i, j].walls[(int)Direction.RIGHT].SetActive(i != width - 1 && tiles[i + 1, j] < 2.0f);
                    worldTiles[i, j].walls[(int)Direction.DOWN].SetActive(j != 0 && tiles[i, j - 1] < 2.0f);
                    worldTiles[i, j].walls[(int)Direction.LEFT].SetActive(i != 0 && tiles[i - 1, j] < 2.0f);
                }
                else if (tiles[i, j] > 0.5f) // Hill
                {
                    fFloorHeight = 2.0f;
                    worldTiles[i, j].walls[(int)Direction.UP].SetActive(j != height - 1 && tiles[i, j + 1] <0.5f);
                    worldTiles[i, j].walls[(int)Direction.RIGHT].SetActive(i != width - 1 && tiles[i + 1, j] < 0.5f);
                    worldTiles[i, j].walls[(int)Direction.DOWN].SetActive(j != 0 && tiles[i, j - 1] < 0.5f);
                    worldTiles[i, j].walls[(int)Direction.LEFT].SetActive(i != 0 && tiles[i - 1, j] < 0.5f);
                }
                else // Floor
                {
                    float fPerlinScore = Mathf.PerlinNoise(fNoiseOffsetX + fHillNoiseScale * (float)i / (float)width, fNoiseOffsetY + fHillNoiseScale * (float)j / (float)height);
                    float fDistFromCentre = new Vector3(i - width / 2, 0.0f, j - height / 2).magnitude;
                    float fScore = (fPerlinScore + 1.0f) * (fDistFromCentre + 1.0f);
                    if (fScore < fBestHomeScore)
                    {
                        fBestHomeScore = fScore;
                        iBestHomeX = i;
                        iBestHomeY = j;
                    }
                }

                // Setup biome
                worldTiles[i, j].floorMesh.material = biomeMaterials[(int)biomes[i, j]];

                // Spawn some trash, maybe
                if (fFloorHeight < 500.0f)
                {
                    if(Random.Range(0.0f, 1.0f) < 0.1f)
                    {
                        List<GameObject> trashList = trash[(int)biomes[i, j]].whyDoIHaveToDoThisUnity;
                        GameObject whatAnExcellentPieceOfTrash = Instantiate<GameObject>(trashList[Random.Range(0, trashList.Count)]);
                        whatAnExcellentPieceOfTrash.transform.SetParent(worldTiles[i, j].transform);
                        whatAnExcellentPieceOfTrash.transform.localPosition = new Vector3(Random.Range(2.0f, 8.0f), fFloorHeight, Random.Range(2.0f, 8.0f));
                        whatAnExcellentPieceOfTrash.transform.localEulerAngles = new Vector3(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
                    }
                }

                //worldTiles[i, j].floorMesh.GetComponent<NavMeshSurface>().BuildNavMesh();
            }
        }

        // Spawn some sweet loot
        for (int i = 0; i < Biomes.iNUM_BIOMES; i++)
        {
            for (int j = 0; j < sweetLoot[i].whyDoIHaveToDoThisUnity.Count; j++)
            {
                KeyValuePair<int, int> pair = tilesByBiome[i][Random.Range(0, tilesByBiome[i].Count - 1)];
                WorldTile tile = worldTiles[pair.Key, pair.Value];
                GameObject loot = Instantiate<GameObject>(sweetLoot[i].whyDoIHaveToDoThisUnity[j], tile.transform);
                loot.transform.localPosition = new Vector3(Random.Range(2.0f, 8.0f), tile.walls.Length > 0 ? 2.0f : 0.0f, Random.Range(2.0f, 8.0f)); // No hacks here, honest.
                loot.transform.localEulerAngles = new Vector3(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
            }
        }

        Destroy(worldTiles[iBestHomeX, iBestHomeY].gameObject);
        worldTiles[iBestHomeX, iBestHomeY] = Instantiate<WorldTile>(home, transform);
        worldTiles[iBestHomeX, iBestHomeY].transform.localPosition = new Vector3(-width * 5.0f + iBestHomeX * 10.0f, 0.0f, -height * 5.0f + iBestHomeY * 10.0f);
        theBase = worldTiles[iBestHomeX, iBestHomeY];
        theBase.floorMesh.GetComponent<NavMeshSurface>().BuildNavMesh();

        Core.GetCore().thePlayer = Instantiate<PlayerBehaviour>(playerPrefab);
        Core.GetCore().thePlayer.transform.position = new Vector3(-width * 5.0f + iBestHomeX * 10.0f + 5.0f, 2.0f, -height * 5.0f + iBestHomeY * 10.0f + 5.0f);

        BasicEnemy foe = Instantiate<BasicEnemy>(enemyPrefab);
        foe.transform.position = Core.GetCore().thePlayer.transform.position + new Vector3(1, 0, 0);
    }
}
