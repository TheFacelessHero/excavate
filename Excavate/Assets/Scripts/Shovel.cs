using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shovel : MonoBehaviour
{
    [SerializeField] Animator an;
    [SerializeField] WorldManager wm;
    [SerializeField] Transform head;
    [SerializeField] int selectedTile;
    [SerializeField] GameObject fallingBlock;
    [SerializeField] bool mouseDown;
    [SerializeField] float throwPower = 5;
    [SerializeField] SpriteRenderer holdingSprite;
    [SerializeField] LayerMask ground;

    private void Start()
    {
        wm = FindObjectOfType<WorldManager>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseDown = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            mouseDown = false;
        }

        an.SetBool("mouseDown", mouseDown);

        if(selectedTile > 0)
        {
            holdingSprite.enabled = true;
            holdingSprite.sprite = wm.blockIDs.Blocks[selectedTile].tileImage;
        }
        else
        {
            selectedTile = 0;
            holdingSprite.enabled = false;
        }

    }

    public void Impact()
    {
        if (selectedTile != 0)
        {
            FallingBlock fb = Instantiate(fallingBlock, head.position, Quaternion.identity, null).GetComponent<FallingBlock>();

            fb.tile = selectedTile;
            fb.GetComponent<Rigidbody2D>().velocity = transform.right * throwPower;
            selectedTile = 0;
        }
        else
        {
            //raycast
            RaycastHit2D ray = Physics2D.Raycast(transform.position, head.position - transform.position, 1,ground);
            Vector2 collisionPoint = ray.point + (Vector2)transform.right * 0.1f;
            selectedTile = wm.GetTile(collisionPoint);
            wm.SetTile(collisionPoint, 0);
            //head.position | collisionPoint
        }
    }
}
