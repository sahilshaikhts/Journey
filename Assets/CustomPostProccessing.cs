using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPostProccessing : MonoBehaviour
{
    public float intensity;
    public Material material;


    //void OnRenderImage(RenderTexture source, RenderTexture destination)
    //{
    //    if (intensity == 0)
    //    {
    //        Graphics.Blit(source, destination);
    //        return;
    //    }
    //  //  material.SetFloat("_bwBlend", intensity);
    //    //Graphics.Blit(source, destination, material);
    //}
}
