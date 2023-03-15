using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Post_process: MonoBehaviour
{

    public ComputeShader textureShader;

    private RenderTexture _rTexture;
    public Light sun;    
    public int NumSamples;
    public int Density;
    public int Weight;
    public float Decay;
    public int Exposure;
    Camera cam;
    private void OnRenderImage(RenderTexture source, RenderTexture destintion){
        
      Vector3 sunScreenPos = cam.WorldToScreenPoint(sun.transform.position);
      float[] sunScreenArray = new float[3];
      for(int i = 0; i < 3; i ++){
          sunScreenArray[i] = sunScreenPos[i];
      }
      
      RenderTexture r = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
      r.enableRandomWrite = true;
      r.Create();
      Graphics.Blit(source, r);

            
      _rTexture = new RenderTexture(Screen.width, Screen.height, 0,  RenderTextureFormat.ARGB32);
      _rTexture.enableRandomWrite = true;
      _rTexture.Create();



      int kernel = textureShader.FindKernel("CSMain");

      textureShader.SetFloats("sunScreenPos", sunScreenArray);


      textureShader.SetTexture(kernel, "result_buffer", _rTexture);
      textureShader.SetTexture(kernel, "screen_buffer", r);

      int workgroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
      int workgroupsY  = Mathf.CeilToInt(Screen.height / 8.0f);

      textureShader.Dispatch(kernel, workgroupsX, workgroupsY, 1);
      Graphics.Blit(_rTexture, destintion);

      r.Release();
      _rTexture.Release();
    }

    void Start()
    {
        cam = GetComponent<Camera>();
        NumSamples = 50;
        Density = 1;
        Weight = 1;
        Decay = 0.25f;
        Exposure = 1;
    }

}
