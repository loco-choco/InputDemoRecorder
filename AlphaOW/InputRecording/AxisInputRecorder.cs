namespace InputDemoRecorder
{
    public struct AxisInputRecorder
    {
        public float Axis;
        public float AxisRaw;

        public AxisInputRecorder(float axis, float axisRaw)
        {
            Axis = axis;
            AxisRaw = axisRaw;
        }
        public AxisInputRecorder(InputChannel axis)
        {
            Axis = axis.GetAxis();
            AxisRaw = axis.GetAxisRaw();
        }
        public void RecordButtonState(InputChannel axis)
        {
            Axis = axis.GetAxis();
            AxisRaw = axis.GetAxisRaw();
        }
    }
}
