using System;
using System.ComponentModel;
using UnityEngine;
public static class SpriteByteConverter
{
    public static byte[] ConvertToByte(Texture2D spr)
    {
        byte[] bitmap = new byte[8];

        Color[] colors = spr.GetPixels();

        for (int i = 0; i < 8; i++)
        {
            uint line = 0;

            for (int j = 0; j < 8; j++)
            {
                if (colors[j + i] == Color.white)
                {
                    uint tmp = 0b_1;

                    line += tmp << 7 - j;
                }
            }

            bitmap[i] = (byte)line;
        }

        return bitmap;
    }
}