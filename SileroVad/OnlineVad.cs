using Microsoft.Extensions.Logging;
using Microsoft.ML.OnnxRuntime;
using SileroVad.Model;

namespace SileroVad
{
    delegate void ForwardBatchOnline(List<OnlineStream> streams);
    public class OnlineVad
    {
        private readonly ILogger<OnlineVad> _logger;
        private IVadProj? _vadProj;
        private ForwardBatchOnline? _forwardBatch;

        /// <summary>
        /// OnlineVad
        /// </summary>
        /// <param name="modelFilePath">模型文件</param>
        /// <param name="configFilePath">配置文件</param>
        /// <param name="threshold">vad 阈值</param>
        /// <param name="batchSize">batch size 上限</param>
        /// <param name="sampleRate">采样率</param>
        /// <param name="threadsNum">onnx runtime 线程数</param>
        /// <param name="isDebug">是否输出调试日志</param>
        public OnlineVad(string modelFilePath, string configFilePath = "", float threshold = 0F, int batchSize = 1, int sampleRate = 16000, int threadsNum = 2, bool isDebug = false)
        {
            VadModel vadModel = new VadModel(modelFilePath, configFilePath: configFilePath, threshold: threshold, threadsNum: threadsNum);
            switch (vadModel.CustomMetadata.Version)
            {
                case "v4":
                    _vadProj = new VadProj(vadModel, sampleRate, isDebug);
                    break;
                case "v5":
                    _vadProj = new VadProjOfV5(vadModel, sampleRate, isDebug);
                    break;
            }
            _forwardBatch = new ForwardBatchOnline(this.ForwardBatch);

            ILoggerFactory loggerFactory = new LoggerFactory();
            _logger = new Logger<OnlineVad>(loggerFactory);
        }

        public OnlineStream CreateOnlineStream()
        {
            OnlineStream onlineStream = new OnlineStream(_vadProj);
            return onlineStream;
        }

        public List<VadResultEntity> GetResults(List<OnlineStream> streams)
        {
            List<VadResultEntity> segmentEntities = new List<VadResultEntity>();
#pragma warning disable CS8602 // 解引用可能出现空引用。
            _forwardBatch.Invoke(streams);
#pragma warning restore CS8602 // 解引用可能出现空引用。
            segmentEntities = this.DecodeMulti(streams);
            return segmentEntities;
        }
        public void ForwardBatch(List<OnlineStream> streams)
        {
            if (streams.Count == 0)
            {
                return;
            }
            List<ModelInputEntity> modelInputs = new List<ModelInputEntity>();
            List<List<float[]>> stateList = new List<List<float[]>>();
            List<OnlineStream> streamsTemp = new List<OnlineStream>();
            foreach (OnlineStream stream in streams)
            {
                ModelInputEntity modelInputEntity = new ModelInputEntity();
                modelInputEntity.Speech = stream.GetDecodeChunk(stream.ChunkLength);
                if (modelInputEntity.Speech == null)
                {
                    streamsTemp.Add(stream);
                    continue;
                }
                //iMax++;
                modelInputEntity.SpeechLength = modelInputEntity.Speech.Length;
                modelInputs.Add(modelInputEntity);
                stream.RemoveChunk(stream.ShiftLength);
                stateList.Add(stream.States);
            }
            if (modelInputs.Count == 0)
            {
                return;
            }
            foreach (OnlineStream stream in streamsTemp)
            {
                streams.Remove(stream);
            }
            try
            {
                List<float[]> states = new List<float[]>();
                List<float[]> stackStatesList = new List<float[]>();
                stackStatesList = _vadProj.stack_states(stateList);
                ModelOutputEntity modelOutput = _vadProj.ModelProj(modelInputs, stackStatesList);

                List<List<float[]>> next_statesList = new List<List<float[]>>();
                next_statesList = _vadProj.unstack_states(modelOutput.ModelOutStates);
                int streamIndex = 0;
                foreach (OnlineStream stream in streams)
                {
                    stream.PushIndex(modelOutput.ModelOut[streamIndex]);
                    //stream.Timestamps.AddRange(timestamps[streamIndex]);
                    stream.States = next_statesList[streamIndex];
                    streamIndex++;
                }
            }
            catch (Exception ex)
            {
                //
            }
        }

        public List<VadResultEntity> DecodeMulti(List<OnlineStream> streams)
        {
            List<VadResultEntity> vadResultEntities = new List<VadResultEntity>();
            foreach (OnlineStream stream in streams)
            {
                VadResultEntity vadResultEntity = new VadResultEntity();
                vadResultEntity.Segments = stream.Segments;
                vadResultEntity.Waveforms = new List<float[]>();
                if (stream.Segments.Count > stream.LastWaveFormsNum)
                {
                    stream.LastWaveFormsNum = stream.Segments.Count;
                    int i = 0;
                    foreach (SegmentEntity segment in stream.Segments)
                    {
                        i++;
                        //waveform segment
                        int multiple = 1;
                        if (i == stream.Segments.Count)
                        {
                            int startExtendLen = 160*10;//1024
                            int endExtendLen = 160*10;//896
                            int startExtend = segment.Start * multiple;
                            if (segment.Start * multiple - startExtendLen > 0)// && i==1
                            {
                                startExtend = segment.Start * multiple - startExtendLen;
                            }
                            int endExtend = segment.End * multiple;
                            if (stream.Waveform.Length - startExtend - (segment.End * multiple - startExtend) > endExtendLen)
                            {
                                endExtend = segment.End * multiple + endExtendLen;
                            }
                            float[] waveformItem = new float[endExtend - startExtend];

                            Array.Copy(stream.Waveform, startExtend, waveformItem, segment.Start * multiple - startExtend, endExtend - segment.Start * multiple);

                            vadResultEntity.Waveforms.Add(waveformItem);
                        }
                        else
                        {
                            vadResultEntity.Waveforms.Add(null);
                        }
                        //waveform segment
                    }
                }
                vadResultEntities.Add(vadResultEntity);
            }
            return vadResultEntities;
        }
        
    }
}
