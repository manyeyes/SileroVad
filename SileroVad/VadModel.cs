using Microsoft.ML.OnnxRuntime;
using SileroVad.Model;
using SileroVad.Utils;

namespace SileroVad
{
    public class VadModel
    {
        private InferenceSession _modelSession;
        private ModelCustomMetadata? _customMetadata;
        public VadModel(string modelFilePath,string configFilePath="", float threshold = 0F, int threadsNum = 2)
        {
            _modelSession = initModel(modelFilePath, threadsNum);
            _customMetadata = LoadConf(configFilePath);
            if (_customMetadata == null)
            {
                _customMetadata = new ModelCustomMetadata();
            }
            if (threshold > 0F)
            {
                _customMetadata.Threshold = threshold;
            }
            //var encoder_meta = _modelSession.ModelMetadata.CustomMetadataMap;
            //_customMetadata.Version = encoder_meta.ContainsKey("version") ? encoder_meta["version"] : "";
        }

        public InferenceSession ModelSession { get => _modelSession; set => _modelSession = value; }
        public ModelCustomMetadata CustomMetadata { get => _customMetadata; set => _customMetadata = value; }

        public InferenceSession initModel(string modelFilePath, int threadsNum = 2)
        {
            Microsoft.ML.OnnxRuntime.SessionOptions options = new Microsoft.ML.OnnxRuntime.SessionOptions();
            options.LogSeverityLevel = OrtLoggingLevel.ORT_LOGGING_LEVEL_FATAL;
            //options.AppendExecutionProvider_DML(0);
            options.AppendExecutionProvider_CPU(0);
            //options.AppendExecutionProvider_CUDA(0);
            options.InterOpNumThreads = threadsNum;
            InferenceSession onnxSession = new InferenceSession(modelFilePath, options);
            return onnxSession;
        }

        private ModelCustomMetadata? LoadConf(string configFilePath)
        {
            if(string.IsNullOrWhiteSpace(configFilePath))
            {
                return null;
            }
            ModelCustomMetadata vadYamlEntity = YamlHelper.ReadYaml<ModelCustomMetadata>(configFilePath);
            return vadYamlEntity;
        }
    }
}
