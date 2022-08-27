using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace ImageClassifierComponent
{
    public class ImageProcessing
    {
        private Vector2 scale = new Vector2(1, 1);
        private Vector2 offset = Vector2.zero;

        private RenderTexture _renderTexture;


        public void Proccess(Texture frame, int targetSize, UnityAction<float[]> result)
        {
            if(_renderTexture==null)
                _renderTexture = new RenderTexture(targetSize, targetSize, 0, RenderTextureFormat.ARGB32);

            // Crop by the center
            scale.x = (float)frame.height / (float)frame.width;
            offset.x = (1 - scale.x) / 2f;
            Graphics.Blit(frame, _renderTexture, scale, offset);

            // Copy 
            AsyncGPUReadback.Request(_renderTexture, 0, TextureFormat.RGB24, (AsyncGPUReadbackRequest request) => {
                if (request.hasError)
                    return;

                byte[] rawData = request.GetData<byte>().ToArray();
                float[] normalizedData = new float[rawData.Length];

                for (int i = 0; i < rawData.Length; i++)
                    normalizedData[i] = (rawData[i] - 127f) / 128f;

                result.Invoke(normalizedData);
            });
        }

    }
}