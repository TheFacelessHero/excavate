using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] BlockIds blockIDs;
    [SerializeField] float destroyRadius = 50;
    Transform destroyFollower;
    public int[,] map;
    public int[,] lastMap;
    public List<Vector2> updatedThisFrame;
    public Vector2 chunkCoords;
    [SerializeField] private WorldManager worldManager;
    private Tilemap tilemap;
    [SerializeField] private int width = 10, height = 10;
    public bool needsChunkRedraw = false;
    private void Awake()
    {
        worldManager = FindObjectOfType<WorldManager>();
        width = worldManager.allChunkSize.x;
        height = worldManager.allChunkSize.y;
        destroyFollower = worldManager.generateChunksAround;
        blockIDs = worldManager.blockIDs;

        map = new int[width, height];
        lastMap = new int[width, height];
        //transform.position = new Vector3(Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).x / width) * width, Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).y / height) * height);
        transform.position = new Vector3(Mathf.Floor(transform.position.x / width) * width, Mathf.Floor(transform.position.y / height) * height);
        chunkCoords = new Vector2(Mathf.Floor(transform.position.x / width), Mathf.Floor(transform.position.y / height));
        gameObject.name = "chunk" + chunkCoords.x + "," + chunkCoords.y;

        ClearMap();
        GenerateChunk(worldManager.seed, worldManager.smoothness, worldManager.terrainHeightMultiplier, worldManager.heightDisplacement);
        DrawChunk();
    }
    private void OnDestroy()
    {
        worldManager.chunks.Remove(gameObject.name);
        ClearChunk();
    }
    private void Update()
    {
        if(Vector2.Distance(transform.position,destroyFollower.position) > destroyRadius)
        {
            Destroy(gameObject);
        }
        if(needsChunkRedraw == true)
        {
            DrawChunk();
        }
        updatedThisFrame.Clear();
    }


    public void ClearMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = 0;
                lastMap[x, y] = 0;
            }
        }
    }
    public void DrawChunk()
    {
        needsChunkRedraw = false;
        //Debug.Log("Redrawing Chunk [ " + chunkCoords.x + ", " + chunkCoords.y + " ]");
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (map[x, y] != lastMap[x, y])
                {
                    //Debug.Log("Redrawing Tile [ " + x + ", " + y + " ]");
                    //tilemap.SetTile(new Vector3Int((int)(chunkCoords.x*10), (int)(chunkCoords.y * 10),0))), worldTiles[selectedChunk.map[x,y]]);
                    
                    worldManager.tilemap.SetTile(new Vector3Int(Mathf.RoundToInt(chunkCoords.x*width) + x, Mathf.RoundToInt(chunkCoords.y * height) + y, 0), blockIDs.Blocks[map[x, y]].tile);
                    lastMap[x, y] = map[x, y];
                }
            }
        }
    }

    public void ClearChunk()
    {
        Debug.Log("Clearing Chunk [ " + chunkCoords.x + ", " + chunkCoords.y + " ]");
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                worldManager.tilemap.SetTile(new Vector3Int(Mathf.RoundToInt(chunkCoords.x * width) + x, Mathf.RoundToInt(chunkCoords.y * height) + y, 0), blockIDs.Blocks[0].tile);

            }
        }
    }

    public void GenerateChunk(float seed,float smoothness,float heightMultiplier,int heightDisplacement)
    {
        int perlinHeight;
        for (int x = 0; x < width; x++)
        {
            perlinHeight = Mathf.RoundToInt(Mathf.PerlinNoise(((x + transform.position.x) / smoothness) , seed)*heightMultiplier);
            perlinHeight -= Mathf.RoundToInt(transform.position.y);
            perlinHeight += heightDisplacement;
            for (int y = 0; y < height; y++)
            {
                if (y >= perlinHeight - 1 && y <= perlinHeight)
                {
                    map[x, y] = 2;
                }else if (transform.position.y + y <=Random.Range(-2f,2f))
                {
                    map[x, y] = 3;
                }else if (y < perlinHeight)
                {
                    map[x, y] = 1;
                }
            }
        }
    }

    /*public void UpdateChunk()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                UpdateTile(new Vector2Int(x, y));
            }
        }
    }*/

    public void UpdateTile(Vector2Int pos)
    {
        if (updatedThisFrame.Contains(pos)) return;
        if (CheckThatTileHasUpdate(blockIDs.Blocks[map[pos.x, pos.y]].tileUpdate))
        {
            //Debug.Log("Updated!");
            Block tile = blockIDs.Blocks[map[pos.x, pos.y]];
            tile.position = new Vector3(transform.position.x + pos.x, transform.position.y + pos.y, 0);
            updatedThisFrame.Add(pos);
            tile.tileUpdate.Invoke();

            if (tile.directionsToUpdateURDL[0]) worldManager.UpdateTile(tile.position + new Vector3(0, 1, 0));
            if (tile.directionsToUpdateURDL[1]) worldManager.UpdateTile(tile.position + new Vector3(1, 0, 0));
            if (tile.directionsToUpdateURDL[2]) worldManager.UpdateTile(tile.position + new Vector3(0, -1, 0));
            if (tile.directionsToUpdateURDL[3]) worldManager.UpdateTile(tile.position + new Vector3(-1, 0, 0));

        }
    }

    public void RandomUpdateTile(Vector2Int pos)
    {
        if (CheckThatTileHasUpdate(blockIDs.Blocks[map[pos.x, pos.y]].randomUpdate))
        {
            Block tile = blockIDs.Blocks[map[pos.x, pos.y]];
            tile.position = new Vector3(transform.position.x + pos.x, transform.position.y + pos.y, 0);
            tile.randomUpdate.Invoke();
        }
    }

    public void RandomUpdateChunk(int tileToUpdate)
    {
        for (int i = 0; i < tileToUpdate; i++)
        {
            RandomUpdateTile( new Vector2Int(Mathf.RoundToInt(Random.Range(0, width)), Mathf.RoundToInt(Random.Range(0, height))));
        }
    }

    public bool CheckThatTileHasUpdate(UnityEvent clickEvent)
    {
        bool hasPersistentTarget = false;
        for (int i = 0; i < clickEvent.GetPersistentEventCount(); i++)
        {
            if (clickEvent.GetPersistentTarget(i) != null)
            {
                hasPersistentTarget = true;
                break;
            }
        }
        return hasPersistentTarget;
    }
}
