using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplaySelectedBlock : MonoBehaviour
{
    WorldManager wm;
    public Image display;
    public TMP_Text tileName;
    public TMP_InputField selected;
    
    void Start()
    {
        wm = FindObjectOfType<WorldManager>();
    }

    // Update is called once per frame
    public void UpdateDisplay()
    {
        int temp = int.Parse(selected.text);
        if(temp>= wm.blockIDs.Blocks.Length)
        {
            temp = wm.blockIDs.Blocks.Length - 1;
            selected.text = temp.ToString();
        }
        wm.selectedTileToPlace = temp;

        display.sprite = wm.blockIDs.Blocks[wm.selectedTileToPlace].tileImage;
        tileName.text = wm.blockIDs.Blocks[wm.selectedTileToPlace].name;
    }
}
