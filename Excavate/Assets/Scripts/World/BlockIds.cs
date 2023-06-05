using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Excavate/Blocks/Block Id Manager", order = 1, fileName = "New Block Id Manager")]

public class BlockIds : ScriptableObject
{
    public Block[] Blocks;
}
