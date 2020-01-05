using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSweet : MonoBehaviour
{
    public enum ColorType
    {
        YELLOW,
        PURPLE,
        RED,
        BLUE,
        PINK,
        ANY,
        GREEN,
        COUNT
    }
    private Dictionary<ColorType, Sprite> colorSpriteDic;
    [System.Serializable]
    public struct ColorSprite
    {
        public ColorType color;
        public Sprite sprite;
    }
    public ColorSprite[] ColorSprites;
    public ColorType Type { get; set; }
    private SpriteRenderer sprite;
    public int NumColors
    {
        get
        {
            return ColorSprites.Length;
        }
    }
    public ColorType Color { get; private set; }
    private void Awake()
    {
        sprite = transform.Find("Sweet").GetComponent<SpriteRenderer>();
        colorSpriteDic = new Dictionary<ColorType, Sprite>();
        for (int i = 0; i < ColorSprites.Length; i++)
        {
            if(!colorSpriteDic.ContainsKey(ColorSprites[i].color))
            {
                colorSpriteDic.Add(ColorSprites[i].color, ColorSprites[i].sprite);
            }
        }
    }
    public void SetColor(ColorType newColor)
    {
        Color = newColor;
        if (colorSpriteDic.ContainsKey(newColor))
        {
            sprite.sprite = colorSpriteDic[newColor];
        }
    }
}

