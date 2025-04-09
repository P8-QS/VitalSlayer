using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TrapManager : MonoBehaviour
{
    [Header("Trap Tiles")]
    public TileBase hiddenTrapTile;
    public TileBase[] trapAnimationTiles; // Your 3 animation tiles

    [Header("Settings")]
    public float damageAmount = 10f;
    public float animationSpeed = 0.1f;
    public bool resetAfterTriggering = true;
    public int animationLoops = 1;

    private Tilemap trapTilemap;
    private HashSet<Vector3Int> triggeredTraps = new HashSet<Vector3Int>();

    void Start()
    {
        trapTilemap = GetComponent<Tilemap>();

        if (GetComponent<TilemapCollider2D>() == null)
            gameObject.AddComponent<TilemapCollider2D>();

        GetComponent<TilemapCollider2D>().isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3Int cellPosition = trapTilemap.WorldToCell(other.transform.position);
            TileBase currentTile = trapTilemap.GetTile(cellPosition);

            if (currentTile == hiddenTrapTile && !triggeredTraps.Contains(cellPosition))
            {
                StartCoroutine(TriggerTrap(cellPosition, other.gameObject));
            }
        }
    }

    IEnumerator TriggerTrap(Vector3Int cellPosition, GameObject player)
    {
        triggeredTraps.Add(cellPosition);

        // Apply damage to player
        Fighter fighter = player.GetComponent<Fighter>();
        if (fighter != null)
        {
            Damage dmg = new Damage
            {
                damageAmount = Mathf.RoundToInt(damageAmount),
                origin = transform.position,
                pushForce = 2.0f,
                isCritical = false,
                minPossibleDamage = Mathf.RoundToInt(damageAmount),
                maxPossibleDamage = Mathf.RoundToInt(damageAmount)
            };

            fighter.SendMessage("ReceiveDamage", dmg);
        }

        // Play animation
        for (int loop = 0; loop < animationLoops; loop++)
        {
            foreach (TileBase animTile in trapAnimationTiles)
            {
                trapTilemap.SetTile(cellPosition, animTile);
                yield return new WaitForSeconds(animationSpeed);
            }
        }

        // Reset trap if configured to do so
        if (resetAfterTriggering)
        {
            trapTilemap.SetTile(cellPosition, hiddenTrapTile);
            triggeredTraps.Remove(cellPosition);
        }
        else
        {
            // Leave the last animation frame
            trapTilemap.SetTile(cellPosition, trapAnimationTiles[trapAnimationTiles.Length - 1]);
        }
    }

    // Call this to reset all traps
    public void ResetAllTraps()
    {
        BoundsInt bounds = trapTilemap.cellBounds;

        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                if (trapTilemap.HasTile(cellPosition))
                {
                    trapTilemap.SetTile(cellPosition, hiddenTrapTile);
                }
            }
        }

        triggeredTraps.Clear();
    }
}