﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScopeRender : MonoBehaviour {

    private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        Material myMaterial = line.GetComponent<Material>();
        myMaterial.mainTextureScale = new Vector2(100,100);
    }

    void Update()
    {
        
    }

    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);
        for (int i = 0; i < result.height; ++i)
        {
            for (int j = 0; j < result.width; ++j)
            {
                Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                result.SetPixel(j, i, newColor);
            }
        }
        result.Apply();
        return result;
    }
}
