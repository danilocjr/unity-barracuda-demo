using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WebcamComponent
{
    public class WebcamController : MonoBehaviour
    {
        [SerializeField] private WebcamBehaviour webcamBvr;

        WebCamTexture _webcamTexture;
        Resolution _requestedResolution;

        public delegate void WebcamSetupEvent(bool result);
        public static event WebcamSetupEvent OnStatusUpdated;

        public delegate void WebcamStreamEvent(Texture frame);
        public static event WebcamStreamEvent OnFrameUpdated;

        [Header("Webcam Default Settings")]
        [SerializeField] private int _activeWebCamIndex = 0;
        [SerializeField] private int _requestedWidth = Screen.width;
        [SerializeField] private int _requestedHeight = Screen.height;

        public bool isStreaming = false;

        IEnumerator WebcamResolutionFitter()
        {
            float timeout = 3 * 60f;

            while (!webcamBvr.hasRatioSet)
            {
                if (_webcamTexture.width >= _requestedResolution.width)
                    webcamBvr.SetAspectRatio(_webcamTexture.width, _webcamTexture.height);

                yield return new WaitForEndOfFrame();

                timeout -= Time.deltaTime;
                if (timeout <= 0f)
                    break;
            }

            OnStatusUpdated?.Invoke(webcamBvr.hasRatioSet);
        }

        IEnumerator WebcamStreamer()
        {
            while (isStreaming)
            {
                if (_webcamTexture.didUpdateThisFrame)
                    OnFrameUpdated?.Invoke(_webcamTexture);

                yield return new WaitForEndOfFrame();
            }
        }

        #region PUBLIC METHODS

        public void Init()
        {
            Init(_requestedWidth, _requestedHeight);
        }

        public void Init(int width, int height)
        {
            _requestedResolution.width = width;
            _requestedResolution.height = height;

            WebCamDevice[] devices = WebCamTexture.devices;
            _webcamTexture = new WebCamTexture(devices[_activeWebCamIndex].name, _requestedResolution.width, _requestedResolution.height);

            webcamBvr.SetImageTexture(_webcamTexture);
            _webcamTexture.Play();

            StartCoroutine(WebcamResolutionFitter());
        }

        public bool StartStream()
        {
            if (webcamBvr.hasRatioSet && !isStreaming)
            {
                isStreaming = true;
                StartCoroutine(WebcamStreamer());
            }

            return isStreaming;
        }

        public bool StopStream()
        {
            isStreaming = false;
            StopCoroutine("StartStream");

            return isStreaming;
        }

        public Texture GetTexture()
        {
            if(_webcamTexture.didUpdateThisFrame)
                return _webcamTexture;

            return null;
        }

        #endregion

    }
}


