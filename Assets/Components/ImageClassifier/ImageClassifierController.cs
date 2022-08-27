using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Barracuda;
using UnityEngine;

namespace ImageClassifierComponent
{
    public class ImageClassifierController : MonoBehaviour
    {
        [Header("ONNX Model")]
        [SerializeField] private NNModel preTrainedModel;
        [SerializeField] private TextAsset labelsJson;
        [SerializeField] private float _minConfidenceLevel = 0.5f;
        [Space(10)]

        // TODO: Make imagePreprocessing interchangeable (SO)...
        [Header("Image PreProcessing Setup")]
        private ImageProcessing imageProcessing = new ImageProcessing();
        [SerializeField] private int _targetSize = 224;
        

        private Model _runtimeModel;
        private IWorker _worker;

        class LabelMap
        {
            public List<string> labels;
        }
        LabelMap _labelMap;

        public bool isReady = false;
        
        #region EVENT LISTENNERS

        public delegate void ImageClassifierSetupEvent(bool result);
        public static event ImageClassifierSetupEvent OnStatusUpdated;

        public delegate void ImageClassifierPredictionEvent(string label, float confidence);
        public static event ImageClassifierPredictionEvent OnPredictionUpdated;

        #endregion

        #region PUBLIC METHODS

        public void Init()
        {
            _runtimeModel = ModelLoader.Load(preTrainedModel);
            _worker = WorkerFactory.CreateWorker(_runtimeModel, WorkerFactory.Device.GPU);

            _labelMap = JsonUtility.FromJson<LabelMap>(labelsJson.ToString());

            isReady = true;
            OnStatusUpdated?.Invoke(isReady);
        }

        public void Run(Texture frame)
        {
            if (!isReady)
                return;
          
            imageProcessing.Proccess(frame, _targetSize, RunPrediction);
        }

        #endregion

        private void RunPrediction(float[] normalizedData)
        {
            Tensor inputTensor = new Tensor(1, _targetSize, _targetSize, 3, normalizedData);
            var inputs = new Dictionary<string, Tensor>
            {
                { "images", inputTensor}
            };

            _worker.Execute(inputs);
            inputTensor.Dispose();

            Tensor outputTensor = _worker.PeekOutput();

            float[] predictions = outputTensor.AsFloats();
            outputTensor.Dispose();

            float confidenceLevel = predictions.Max();
            if (confidenceLevel < _minConfidenceLevel)
                return;
            
            int bestFit = Array.IndexOf(predictions, confidenceLevel);
            LabelPrediction(bestFit, confidenceLevel);
        }

        private void LabelPrediction(int labelIndex, float confidenceLevel)
        {
            if (labelIndex >= _labelMap.labels.Count)
                return;

            OnPredictionUpdated?.Invoke(_labelMap.labels[labelIndex], confidenceLevel);
        }

        private void OnDestroy()
        {
            _worker?.Dispose();
        }

    }

}
    
