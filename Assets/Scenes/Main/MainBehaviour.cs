using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainBehaviour : MonoBehaviour
{
    public TMP_Text predictionText;

    void Start()
    {
        predictionText.text = "";
    }

    public void SetPredictionText(string label, float confidence)
    {
        string oneWordLabel = label.Split(',')[0];
        string confidenceFormatted = confidence.ToString("F1");

        predictionText.text = $"{oneWordLabel} [{confidenceFormatted}]";
    }

   
}
