using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownscaleRenderTexture : MonoBehaviour {
    public Camera cam;
    RenderTexture texture;
    public Material outputMaterial;
    public FilterMode _filterMode = FilterMode.Point;
    public Vector3 _textureScale = new Vector3(320, 180, 24);

    private void Awake() {
        //cam = GetComponent<Camera>();
    }
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        //Render Texture
        ////RenderTexture.ReleaseTemporary(texture);
        //texture = RenderTexture.GetTemporary(320, 180, 24);
        ////texture = RenderTexture.GetTemporary((int)_textureScale.x, (int)_textureScale.y, (int)_textureScale.z);
        ////texture.filterMode = _filterMode;
        ////cam.targetTexture = texture;
        ////outputMaterial.SetTexture("_MainTex", texture);

    }
}