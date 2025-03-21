using UnityEngine;
using UnityEngine.U2D;

public class SpriteManager : MonoBehaviour
{
    public SpriteAtlas spriteAtlas; 

    public static SpriteManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public Sprite GetSprite(string spriteName)
    {
        return spriteAtlas?.GetSprite(spriteName);
    }
}
