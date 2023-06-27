using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] string SAVE_FOLDER;
    [SerializeField] BlockIds blockIDs;
    [SerializeField] float destroyRadius = 50;
    Transform destroyFollower;
    public int[,] map;
    public int[,] lastMap;
    [SerializeField] bool toSave = false;
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
        destroyRadius = worldManager.chunkDespawnDistance;

        SAVE_FOLDER = Application.dataPath + "/saves/" + worldManager.worldName + "/chunks";
        if (!Directory.Exists(SAVE_FOLDER)) {
            Directory.CreateDirectory(SAVE_FOLDER);
        }

        map = new int[width, height];
        lastMap = new int[width, height];
        //transform.position = new Vector3(Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).x / width) * width, Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).y / height) * height);
        transform.position = new Vector3(Mathf.Floor(transform.position.x / width) * width, Mathf.Floor(transform.position.y / height) * height);
        chunkCoords = new Vector2(Mathf.Floor(transform.position.x / width), Mathf.Floor(transform.position.y / height));
        gameObject.name = "chunk" + chunkCoords.x + "," + chunkCoords.y;

       

        ClearMap();
        if(File.Exists(SAVE_FOLDER + "/" + gameObject.name + ".chunk"))
        {
            Load();
        }
        else
        {
            GenerateChunk(worldManager.seed, worldManager.smoothness, worldManager.terrainHeightMultiplier, worldManager.heightDisplacement);
        }
        DrawChunk();



    }
    private void OnDestroy()
    {
        worldManager.chunks.Remove(gameObject.name);
        if (toSave) Save();
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
            toSave=true;
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
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] != lastMap[x, y])
                {
                    //Debug.Log("Redrawing Chunk [ " + chunkCoords.x + ", " + chunkCoords.y + " ]" + " Tile [ " + x + ", " + y + " ] With Tile : [" + map[x, y] + "]");
                    //tilemap.SetTile(new Vector3Int((int)(chunkCoords.x*10), (int)(chunkCoords.y * 10),0))), worldTiles[selectedChunk.map[x,y]]);
                    
                    worldManager.tilemap.SetTile(new Vector3Int(Mathf.RoundToInt(chunkCoords.x*width) + x, Mathf.RoundToInt(chunkCoords.y * height) + y, 0), blockIDs.Blocks[map[x, y]].tile[0]);
                    lastMap[x, y] = map[x, y];
                }
            }
        }
    }
    public void ClearChunk()
    {
        //Debug.Log("Clearing Chunk [ " + chunkCoords.x + ", " + chunkCoords.y + " ]");
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                worldManager.tilemap.SetTile(new Vector3Int(Mathf.RoundToInt(chunkCoords.x * width) + x, Mathf.RoundToInt(chunkCoords.y * height) + y, 0), null);

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
                if (y == perlinHeight)
                {
                    if(Random.Range(0f,1.0f) < 0.2f) map[x, y] = Random.Range(5,7);
                }
                else if(y == perlinHeight - 1)
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

    public void UpdateChunk()
    {
        List<Vector2Int> tilesToUpdate = new List<Vector2Int>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(CheckThatTileHasUpdate(blockIDs.Blocks[map[x, y]].chunkUpdate))
                {
                    tilesToUpdate.Add(new Vector2Int(x, y));
                }
            }
        }

        for (int i = 0; i < tilesToUpdate.Count; i++)
        {
            ChunkUpdateTile(tilesToUpdate[i]);
        }
    }

    public void ChunkUpdateTile(Vector2Int pos)
    {
        if (updatedThisFrame.Contains(pos)) return;
        if (CheckThatTileHasUpdate(blockIDs.Blocks[map[pos.x, pos.y]].chunkUpdate))
        {
            //Debug.Log("Updated!");
            Block tile = blockIDs.Blocks[map[pos.x, pos.y]];
            tile.position = new Vector3(transform.position.x + pos.x, transform.position.y + pos.y, 0);
            updatedThisFrame.Add(pos);
            tile.chunkUpdate.Invoke();

            if (tile.directionsToUpdateURDL[0]) worldManager.UpdateTile(tile.position + new Vector3(0, 1, 0));
            if (tile.directionsToUpdateURDL[1]) worldManager.UpdateTile(tile.position + new Vector3(1, 0, 0));
            if (tile.directionsToUpdateURDL[2]) worldManager.UpdateTile(tile.position + new Vector3(0, -1, 0));
            if (tile.directionsToUpdateURDL[3]) worldManager.UpdateTile(tile.position + new Vector3(-1, 0, 0));

        }
    }

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
        //Debug.Log(blockIDs.Blocks[map[pos.x, pos.y]]);
        //Debug.Log("X: " + pos.x + " Y: " + pos.y);
        //Debug.Log(map[pos.x, pos.y]);
        //Debug.Log("Hallo <DEBUG> " + "X: " + pos.x + " Y: " + pos.y + " Tile: ");
        //Debug.Log(blockIDs.Blocks[map[pos.x, pos.y]].randomUpdate);
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
        //Debug.Log("DEBUG");
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

    private void Save()
    {
        //string saveSeparator = " #SAVE# ";

        int[] saveInt;
        int idx = 0;
        saveInt = new int[width * height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                saveInt[idx] = map[x, y];
                idx++;
            }
        }
        SaveChunk saveChunk = new SaveChunk
        {
            //saveMap = map,
            saveMap = saveInt,            
        };
        string json = JsonUtility.ToJson(saveChunk);
        //Debug.Log(json);
        File.WriteAllText(SAVE_FOLDER + "/" + gameObject.name + ".chunk", json);
        //File.WriteAllText(Application.dataPath + "/saves/" + worldManager.worldName + "/chunks/" + gameObject.name + ".txt", json);

    }
    private void Load()
    {
        //string saveSeparator = " #SAVE# ";
        string saveString = File.ReadAllText(SAVE_FOLDER + "/" + gameObject.name + ".chunk");
        SaveChunk saveChunk = JsonUtility.FromJson<SaveChunk>(saveString);

        int idx = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(saveChunk.saveMap[idx] != 0)
                map[x, y] = saveChunk.saveMap[idx];
                idx++;
            }
        }
    }

    class SaveChunk
    {
        public int[] saveMap;
    }
}
