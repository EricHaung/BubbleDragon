using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class OutlineEffect : MonoBehaviour
{
    public Camera sourceCamera;

    Shader outlineShader;
    public Material outlineShaderMaterial;
    public RenderTexture renderTexture;

    void Start()
    {
        CreateMaterialsIfNeeded();

        if (sourceCamera == null)
        {
            sourceCamera = GetComponent<Camera>();

            if (sourceCamera == null)
                sourceCamera = Camera.main;
        }

        renderTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16, RenderTextureFormat.Default);
    }

    void OnDestroy()
    {
        if (renderTexture != null)
            renderTexture.Release();
        DestroyMaterials();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        outlineShaderMaterial.SetTexture("_OutlineSource", renderTexture);
        Graphics.Blit(source, destination, outlineShaderMaterial, 1);
    }

    private void CreateMaterialsIfNeeded()
    {
        if (outlineShader == null)
            outlineShader = Resources.Load<Shader>("OutlineShader");

        if (outlineShaderMaterial == null)
        {
            outlineShaderMaterial = new Material(outlineShader);
        }
    }

    private void DestroyMaterials()
    {
        DestroyImmediate(outlineShaderMaterial);
    }
}
