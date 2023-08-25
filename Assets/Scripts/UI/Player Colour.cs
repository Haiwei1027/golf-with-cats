using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColour
{
    public static readonly Color RED = new Color32(0xEC, 0x1C, 0x24, 0xFF);
    public static readonly Color YELLOW = new Color32(0xFF, 0xF2, 0x00, 0xFF);
    public static readonly Color GREEN = new Color32(0xC4, 0xFF, 0x0E, 0xFF);
    public static readonly Color BLUE = new Color32(0x00, 0xA8, 0xF8, 0xFF);
    public static readonly Color ORANGE = new Color32(0xFF, 0x7F, 0x27, 0xFF);
    public static readonly Color PINK = new Color32(0xFF, 0xAE, 0xC8, 0xFF);
    public static readonly Color BROWN = new Color32(0xB9,0x7A,0x56,0xFF);

    private static readonly Color[] colors = { RED, YELLOW, GREEN, BLUE, ORANGE, PINK, BROWN };
    public const int COUNT = 7;

    public static Color Get(int i)
    {
        return colors[i%colors.Length];
    }
}
