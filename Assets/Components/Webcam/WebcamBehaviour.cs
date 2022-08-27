using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WebcamComponent
{
    public class WebcamBehaviour : MonoBehaviour
    {
        [SerializeField] private RawImage rawImage;
        [SerializeField] private AspectRatioFitter fitter;

        public bool hasRatioSet;

        public void SetAspectRatio(int width, int height)
        {
            fitter.aspectRatio = (float) width / (float) height;
            hasRatioSet = true;
        }

        public void SetImageTexture(Texture texture)
        {
            rawImage.texture = texture;
        }

    }
}

