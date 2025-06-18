// See https://github.com/manyeyes for more information
// Copyright (c)  2023 by manyeyes
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SileroVad.Model
{
    public class ModelCustomMetadata
    {
        private string? _version="v4";
        private int _windows_frame_size = 96;
        private float _threshold = 0.5F;
        private int _min_silence_duration_ms = 0;
        private int _speech_pad_ms = 96;
        private int _min_speech_duration_ms = 8;
        private float _max_speech_duration_s = 6.0f; 
        private int _segment_start_extend_len = 1024;
        private int _segment_end_extend_len = 896;

        public string? Version { get => _version; set => _version = value; }
        public int Windows_frame_size { get => _windows_frame_size; set => _windows_frame_size = value; }
        public float Threshold { get => _threshold; set => _threshold = value; }
        public int Min_silence_duration_ms { get => _min_silence_duration_ms; set => _min_silence_duration_ms = value; }
        public int Speech_pad_ms { get => _speech_pad_ms; set => _speech_pad_ms = value; }
        public int Min_speech_duration_ms { get => _min_speech_duration_ms; set => _min_speech_duration_ms = value; }
        public float Max_speech_duration_s { get => _max_speech_duration_s; set => _max_speech_duration_s = value; }
        public int Segment_start_extend_len { get => _segment_start_extend_len; set => _segment_start_extend_len = value; }
        public int Segment_end_extend_len { get => _segment_end_extend_len; set => _segment_end_extend_len = value; }
    }
}
