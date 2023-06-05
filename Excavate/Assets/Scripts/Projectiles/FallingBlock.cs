using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    SpriteRenderer sr;
    Rigidbody2D rb;
    WorldManager wm;
    public int tile;
    public bool sticky;
    public Vector2 landingDirection;
    [SerializeField] float rotMin = -10;
    [SerializeField] float rotMax = 10;
    [SerializeField] float projectileLifetime = 10;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        wm = FindObjectOfType<WorldManager>();

        sr.sprite = wm.blockIDs.Blocks[wm.selectedTileToPlace].tileImage;

        rb.angularVelocity = Random.Range(rotMin, rotMax);
    }
    private void Update()
    {
        projectileLifetime -= Time.deltaTime;
        if (projectileLifetime <= 0) Destroy(gameObject);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        projectileLifetime -= 1;
        if (wm.GetTile(transform.position - (Vector3)landingDirection/2) != tile)
        {
            if (sticky)
            {
                wm.SetTile(transform.position - (Vector3)landingDirection/2, tile);
                Destroy(gameObject);
            }
            else
            {
                if (Physics2D.OverlapBox((Vector2)transform.position + (landingDirection - (Vector2)landingDirection/2), new Vector2(0.3f, 0.3f), 0))
                {
                    wm.SetTile(transform.position, tile);
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            transform.position = transform.position - (Vector3)landingDirection / 2;
        }
    }

}
