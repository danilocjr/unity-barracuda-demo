using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    [SerializeField] private MainBehaviour mainView;

    [SerializeField] private WebcamComponent.WebcamController webcam;
    [SerializeField] private ImageClassifierComponent.ImageClassifierController classifier;

    private void Start()
    {
        WebcamComponent.WebcamController.OnStatusUpdated += WebcamController_OnWebcamInitiated;
        WebcamComponent.WebcamController.OnFrameUpdated += WebcamController_OnFrameUpdated;

        webcam.Init();

        ImageClassifierComponent.ImageClassifierController.OnStatusUpdated += ImageClassifierController_OnStatusUpdated;
        ImageClassifierComponent.ImageClassifierController.OnPredictionUpdated += ImageClassifierController_OnPredictionUpdated;

        classifier.Init();
    }

    #region Classifier Events

    private void ImageClassifierController_OnPredictionUpdated(string label, float confidence)
    {
        mainView.SetPredictionText(label, confidence);
    }

    private void ImageClassifierController_OnStatusUpdated(bool result)
    {
        if (result)
        {
            Debug.Log("Image Classifier Initialized Successfuly");
        }
        else
        {
            Debug.Log("Image Classifier Failed to Initialize");
        }
    }

    #endregion


    #region Webcam Events

    private void WebcamController_OnWebcamInitiated(bool result)
    {
        if (result)
        {
            Debug.Log("Webcam Initialized Successfuly");
            if (webcam.StartStream())
                Debug.Log("Webcam Started Streaming...");
            else
                Debug.Log("Webcam Failed Stream");
        }
        else
        {
            Debug.Log("Webcam Failed to Initialize");
        }
    }

    private void WebcamController_OnFrameUpdated(Texture frame)
    {
        if (classifier.isReady && webcam.isStreaming)
            classifier.Run(frame);
    }

    #endregion

}
