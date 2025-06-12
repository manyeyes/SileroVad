namespace SileroVad.Model
{
    public class SegmentEntity
    {
        private int _start;
        private int _end;

        public int Start { get => _start; set => _start = value; }
        public int End { get => _end; set => _end = value; }
    }
}
