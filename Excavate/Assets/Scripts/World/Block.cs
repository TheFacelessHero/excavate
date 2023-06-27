using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Excavate/Blocks/Block", order = 2, fileName = "New Block")]
public class Block : ScriptableObject
{
    public TileBase[] tile;
    public TileGroups[] tileLists;
    public int state = 0;
    public Sprite tileImage;
    public GameObject[] referenceGameObjects;
    public float health;
    public float friction = 1;
    public Vector3 position;

    public bool[] directionsToUpdateURDL = new bool[4];

    public UnityEvent chunkUpdate;
    public UnityEvent tileUpdate;
    public UnityEvent randomUpdate;

    //BlockEvents

    public string[] SeperateData(string data)
    {
        string[] strArr; 
        strArr = data.Split(" ");
        return strArr;
    }

    // Chance ReplaceWith
    public void ChangeRandomly(string data)
    {
        //  "<Chance>"

        string[] str = SeperateData(data);
        float randomChance = float.Parse(str[0]);
        int replaceWith = int.Parse(str[1]);

        if (randomChance >= Random.Range(0f, 100f))
        {
            WorldManager wm = FindObjectOfType<WorldManager>();
            wm.SetTile(position, replaceWith);
        }
    }

    // Chance SenseX SenseY ReplaceWith suffocateIfThis dontSuffocateIfThis
    public void Suffocate(string data)
    {
        //  "<Chance>,<OffsetX>,<OffsetY>"

        string[] str = SeperateData(data);
        float randomChance = float.Parse(str[0]);
        int offsetX = int.Parse(str[1]);
        int offsetY = int.Parse(str[2]);
        int replaceWith = int.Parse(str[3]);
        List<int> suffocateIfThis = new List<int>(System.Array.ConvertAll(str[4].Split(','), int.Parse));
        List<int> dontSuffocateIfThis = new List<int>(System.Array.ConvertAll(str[5].Split(','), int.Parse));

        if (randomChance >= Random.Range(0f, 100f))
        {
            WorldManager wm = FindObjectOfType<WorldManager>();
            int tileabove = wm.GetTile(new Vector2(position.x + offsetX, position.y + offsetY));
            if (tileabove != 0 && suffocateIfThis.Contains(-1) && !dontSuffocateIfThis.Contains(tileabove) || suffocateIfThis.Contains(tileabove) && !dontSuffocateIfThis.Contains(tileabove))
            {
                wm.SetTile(position, replaceWith);
            }
        }
    }
    public void BreakIfUnSupported(string data)
    {
        //  "<Chance>,<OffsetX>,<OffsetY>"

        string[] str = SeperateData(data);
        float randomChance = float.Parse(str[0]);
        int offsetX = int.Parse(str[1]);
        int offsetY = int.Parse(str[2]);
        int replaceWith = int.Parse(str[3]);
        List<int> supports = new List<int>(System.Array.ConvertAll(str[4].Split(','), int.Parse));

        if (randomChance >= Random.Range(0f, 100f))
        {
            WorldManager wm = FindObjectOfType<WorldManager>();
            int sensingTile = wm.GetTile(new Vector2(position.x + offsetX, position.y + offsetY));
            if (!supports.Contains(sensingTile))
            {//sensingTile != 0 && supports.Contains(-1) || supports.Contains(sensingTile)

                wm.SetTile(position, replaceWith);
            }
        }
    }
    // Chance OffsetX OffsetY SpreadX Spread Y ReplaceWith Replaceable NeededOffsetX NeededOffsetY NeededTile SpreadChance
    public void Spread(string data)
    {
        //  <Chance> <ReplaceWith> <Replaceable>

        string[] str = SeperateData(data);
        float randomChance = float.Parse(str[0]);
        int offsetX = int.Parse(str[1]);
        int offsetY = int.Parse(str[2]);
        int spreadX = int.Parse(str[3]);
        int spreadY = int.Parse(str[4]);
        int replaceWith = int.Parse(str[5]);
        List<int> replaceable = new List<int>(System.Array.ConvertAll(str[6].Split(','), int.Parse));
        int neededOffsetX = int.Parse(str[7]);
        int neededOffsetY = int.Parse(str[8]);
        int neededTile = int.Parse(str[9]);
        float randomSpreadChance = float.Parse(str[10]);

        if (randomChance >= Random.Range(0f, 100f))
        {
            WorldManager wm = FindObjectOfType<WorldManager>();
            int thisTile = wm.GetTile(position + new Vector3(neededOffsetX,neededOffsetY,0));
            if (thisTile != 0 && neededTile == -1 || thisTile == neededTile)
            {
                for (int x = -spreadX; x < spreadX+1; x++)
                {
                    for (int y = -spreadY; y < spreadY+1; y++)
                    {
                        if (randomSpreadChance >= Random.Range(0f, 100f))
                        {
                            int tileSensing = wm.GetTile(new Vector2(position.x + x + offsetX, position.y + y + offsetY));
                            if(tileSensing != replaceWith)
                            if (tileSensing != 0 && replaceable.Contains(-1) || replaceable.Contains(tileSensing))
                            {
                                wm.SetTile(new Vector2(position.x + x + offsetX, position.y + y + offsetY), replaceWith);
                            }
                        }
                    }
                }
            }

        }
    }

    // Chance SenseX SenseY ReplaceWith SenseFor Summon IsSticky LandingDirectionX LandingDirectionY
    public void FallingBlock(string data)
    {


        //  "<Chance>,<OffsetX>,<OffsetY>"

        string[] str = SeperateData(data);
        float randomChance = float.Parse(str[0]);
        int offsetX = int.Parse(str[1]);
        int offsetY = int.Parse(str[2]);
        int replaceWith = int.Parse(str[3]);
        List<int> replaceable = new List<int>(System.Array.ConvertAll(str[4].Split(','), int.Parse));
        int summon = int.Parse(str[5]);
        bool isSticky = bool.Parse(str[6]);
        float landDirX = float.Parse(str[7]);
        float landDirY = float.Parse(str[8]);

        if (randomChance >= Random.Range(0f, 100f))
        {
            WorldManager wm = FindObjectOfType<WorldManager>();
            int neededTile = wm.GetTile(new Vector2(position.x + offsetX, position.y + offsetY));
            if (neededTile != 0 && replaceable.Contains(-1) || replaceable.Contains(neededTile))
            {
                FallingBlock fb = Instantiate(referenceGameObjects[summon], position + new Vector3(0.5f,0.5f,0), Quaternion.identity, null).GetComponent<FallingBlock>();

                fb.tile = wm.GetTile(position);
                fb.sticky = isSticky;
                fb.landingDirection.x = landDirX;
                fb.landingDirection.y = landDirY;


                wm.SetTile(position, replaceWith);
            }
        }
    }

    public void Liquid(string data)
    {

        string[] str = SeperateData(data);
        float randomChance = float.Parse(str[0]);
        int replaceWith = int.Parse(str[1]);

        if (randomChance >= Random.Range(0f, 100f))
        {
            WorldManager wm = FindObjectOfType<WorldManager>();
            
            if (wm.GetTile(new Vector2(position.x, position.y - 1)) == 0)
            {//sensingTile != 0 && supports.Contains(-1) || supports.Contains(sensingTile)

                wm.SetTile(position + new Vector3(0,-1,0), replaceWith);
                wm.SetTile(position, 0);
            }
            else if (wm.GetTile(new Vector2(position.x -1, position.y - 1)) == 0)
            {//sensingTile != 0 && supports.Contains(-1) || supports.Contains(sensingTile)

                wm.SetTile(position + new Vector3(-1, -1, 0), replaceWith);
                wm.SetTile(position, 0);
            }
            else if (wm.GetTile(new Vector2(position.x + 1, position.y - 1)) == 0)
            {//sensingTile != 0 && supports.Contains(-1) || supports.Contains(sensingTile)

                wm.SetTile(position + new Vector3(1, -1, 0), replaceWith);
                wm.SetTile(position, 0);
            }

            else if (wm.GetTile(new Vector2(position.x - 1, position.y)) == 0)
            {//sensingTile != 0 && supports.Contains(-1) || supports.Contains(sensingTile)

                wm.SetTile(position + new Vector3(-1, 0, 0), replaceWith);
                wm.SetTile(position, 0);
            }
            else if (wm.GetTile(new Vector2(position.x + 1, position.y)) == 0)
            {//sensingTile != 0 && supports.Contains(-1) || supports.Contains(sensingTile)

                wm.SetTile(position + new Vector3(1, 0, 0), replaceWith);
                wm.SetTile(position, 0);
            }
        }
    }
}
