using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldManager : MonoBehaviour
{
    public Transform generateChunksAround;
    public float seed;
    public float smoothness = 30;
    public float terrainHeightMultiplier = 10;
    public int heightDisplacement = 100;
    public int chunkGenRadius = 3;
    public Tilemap tilemap;
    public GameObject chunkPrefab;
    public List<string> chunks;
    private Camera mainCam;
    public int selectedTileToPlace = 1;
    public BlockIds blockIDs;

    public Vector2Int allChunkSize = new Vector2Int(10, 10);
    public float timeBetweenPlacement = 0.05f;
    float timeBetweenBlocks = 0;
    public float randomUpdateTimer = 0.5f;
    public int randomUpdates = 3;
    float currandTimer;

    [SerializeField] int chunkGenX;
    [SerializeField] int chunkGenY;
    [SerializeField] int chunksPerFrame = 5;
    private void Awake()
    {
        if (seed == 0) seed = Random.Range(-1000f, 1000f);
        mainCam = Camera.main;

        
    }

    //Add function that gets chunk
    private void Update()
    {


        if (mainCam == null) mainCam = Camera.main;

        if (Input.GetMouseButtonDown(0))
        {
            timeBetweenBlocks = 0;
            
        }
        if (Input.GetMouseButtonDown(1))
        {
            timeBetweenBlocks = 0;
            
        }

        if (Input.GetMouseButton(0))
        {
            timeBetweenBlocks -= Time.deltaTime;
            if (timeBetweenBlocks <= 0)
            {
                timeBetweenBlocks = timeBetweenPlacement;
                SetTile(mainCam.ScreenToWorldPoint(Input.mousePosition), 0);
            }
        }else if (Input.GetMouseButton(1))
        {
            timeBetweenBlocks -= Time.deltaTime;
            if (timeBetweenBlocks <= 0)
            {
                timeBetweenBlocks = timeBetweenPlacement;
                SetTile(mainCam.ScreenToWorldPoint(Input.mousePosition), selectedTileToPlace);
            }
        }
        else if (Input.GetMouseButtonDown(2))
        {
            Debug.Log(GetTile(mainCam.ScreenToWorldPoint(Input.mousePosition)));
            UpdateTile(mainCam.ScreenToWorldPoint(Input.mousePosition));
        }

        /*for (int x = -chunkGenRadius; x < chunkGenRadius; x++)
        {
            for (int y = -chunkGenRadius; y < chunkGenRadius; y++)
            {
                GenerateChunk(new Vector2(generateChunksAround.position.x + x * 10, generateChunksAround.position.y + y * 10), new Vector2(10, 10));
            }
        }*/
        for (int i = 0; i < chunksPerFrame; i++)
        {
            if (chunkGenX <= chunkGenRadius)
            {
                if (chunkGenY <= chunkGenRadius)
                {
                    GenerateChunk(new Vector2(generateChunksAround.position.x + chunkGenX * 10, generateChunksAround.position.y + chunkGenY * 10));
                    chunkGenY++;

                    if (chunkGenY >= chunkGenRadius)
                    {
                        chunkGenY = -chunkGenRadius;
                        chunkGenX++;
                    }
                }

                if (chunkGenX >= chunkGenRadius)
                {
                    chunkGenX = -chunkGenRadius;
                }
            }
        }



        currandTimer -= Time.deltaTime;
        if (currandTimer <= 0)
        {
            currandTimer = randomUpdateTimer;
            ChunkManager[] chunks = GetComponentsInChildren<ChunkManager>();
            for (int i = 0; i < chunks.Length; i++)
            {
                chunks[i].RandomUpdateChunk(randomUpdates);
            }
        }

    }

    public ChunkManager getChunk(Vector2 pos)
    {
        
        Vector2 chunkPos = new Vector2(Mathf.Floor(pos.x / allChunkSize.x), Mathf.Floor(pos.y / allChunkSize.y));
        string chunkName = "chunk" + chunkPos.x + "," + chunkPos.y;
        //Debug.Log(chunkName);
        ChunkManager selectedChunk = null;

        if (GameObject.Find(chunkName) != null)
            selectedChunk = GameObject.Find(chunkName).GetComponent<ChunkManager>();

        if (selectedChunk == null) return null;

        return selectedChunk;
    }

    public void SetTile(Vector2 pos,int tile)
    {
        Vector2 tileOffset = new Vector2(Mathf.Floor(pos.x), Mathf.Floor(pos.y)) - new Vector2(Mathf.Floor(pos.x / allChunkSize.x) * allChunkSize.x, Mathf.Floor(pos.y / allChunkSize.y) * allChunkSize.y);

        ChunkManager selectedChunk = getChunk(pos);
        if (selectedChunk == null) return;
        //Debug.Log("Set Tile Debug: Chunk [ " + chunkName + "] TileID [ " + tile + " ] Pos [ X " + pos.x + " Y " + pos.y + " ] Tile Offset [ X " + tileOffset.x + " Y " + tileOffset.y + " ] Current Chunk Tile [ " + selectedChunk.map[Mathf.RoundToInt(tileOffset.x), Mathf.RoundToInt(tileOffset.y)] + " ] Last Chunk Tile [ " + selectedChunk.lastMap[Mathf.RoundToInt(tileOffset.x), Mathf.RoundToInt(tileOffset.y)] + " ]");

        selectedChunk.map[Mathf.RoundToInt(tileOffset.x), Mathf.RoundToInt(tileOffset.y)] = tile;

        //Debug.Log("Set Tile Debug: Chunk [ " + chunkName + "] TileID [ " + tile + " ] Pos [ X " + pos.x + " Y " + pos.y + " ] Tile Offset [ X " + tileOffset.x + " Y " + tileOffset.y + " ] Current Chunk Tile [ " + selectedChunk.map[Mathf.RoundToInt(tileOffset.x), Mathf.RoundToInt(tileOffset.y)] + " ] Last Chunk Tile [ " + selectedChunk.lastMap[Mathf.RoundToInt(tileOffset.x), Mathf.RoundToInt(tileOffset.y)] + " ]");

        //selectedChunk.DrawChunk();
        UpdateTile(pos);

        UpdateTile(pos + new Vector2(0, 1));
        UpdateTile(pos + new Vector2(1, 0));
        UpdateTile(pos + new Vector2(0, -1));
        UpdateTile(pos + new Vector2(-1, 0));

        selectedChunk.needsChunkRedraw = true;

    }

    public void UpdateTile(Vector2 pos)
    {
        Vector2 tileOffset = new Vector2(Mathf.Floor(pos.x), Mathf.Floor(pos.y)) - new Vector2(Mathf.Floor(pos.x / allChunkSize.x) * allChunkSize.x, Mathf.Floor(pos.y / allChunkSize.y) * allChunkSize.y);

        ChunkManager selectedChunk = getChunk(pos);
        if (selectedChunk == null) return;
        //Debug.Log("Set Tile Debug: Chunk [ " + chunkName + "] TileID [ " + tile + " ] Pos [ X " + pos.x + " Y " + pos.y + " ] Tile Offset [ X " + tileOffset.x + " Y " + tileOffset.y + " ] Current Chunk Tile [ " + selectedChunk.map[Mathf.RoundToInt(tileOffset.x), Mathf.RoundToInt(tileOffset.y)] + " ] Last Chunk Tile [ " + selectedChunk.lastMap[Mathf.RoundToInt(tileOffset.x), Mathf.RoundToInt(tileOffset.y)] + " ]");

        selectedChunk.UpdateTile(new Vector2Int(Mathf.RoundToInt(tileOffset.x), Mathf.RoundToInt(tileOffset.y)));
        

    }

    public int GetTile(Vector2 pos)
    {
        Vector2 tileOffset = new Vector2(Mathf.Floor(pos.x), Mathf.Floor(pos.y)) - new Vector2(Mathf.Floor(pos.x / allChunkSize.x) * allChunkSize.x, Mathf.Floor(pos.y / allChunkSize.y) * allChunkSize.y);

        ChunkManager selectedChunk = getChunk(pos);
        if (selectedChunk == null) return -1;

        //Debug.Log("Set Tile Debug: Chunk [ " + chunkName + "] TileID [ " + tile + " ] Pos [ X " + pos.x + " Y " + pos.y + " ] Tile Offset [ X " + tileOffset.x + " Y " + tileOffset.y + " ] Current Chunk Tile [ " + selectedChunk.map[Mathf.RoundToInt(tileOffset.x), Mathf.RoundToInt(tileOffset.y)] + " ] Last Chunk Tile [ " + selectedChunk.lastMap[Mathf.RoundToInt(tileOffset.x), Mathf.RoundToInt(tileOffset.y)] + " ]");

        return selectedChunk.map[Mathf.RoundToInt(tileOffset.x), Mathf.RoundToInt(tileOffset.y)];


    }


    public void GenerateChunk(Vector2 pos)
    {
        Vector2 chunkPos = new Vector2(Mathf.Floor(pos.x / allChunkSize.x), Mathf.Floor(pos.y / allChunkSize.y));
        string chunkName = "chunk" + chunkPos.x + "," + chunkPos.y;
        if(chunks.Contains(chunkName) != true)
        {
            GameObject newChunk = Instantiate(chunkPrefab, new Vector3(chunkPos.x * allChunkSize.x, chunkPos.y * allChunkSize.y, 0), Quaternion.identity, transform);
            newChunk.name = chunkName;
            chunks.Add(chunkName);
        }
    }
    
}
