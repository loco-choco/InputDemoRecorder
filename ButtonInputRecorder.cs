namespace InputDemoRecorder
{
    public struct ButtonInputRecorder
    {
        public bool Button;
        public bool ButtonDown;
        public bool ButtonUp;

        public ButtonInputRecorder(InputChannel button)
        {
            Button = button.GetButton();
            ButtonDown = button.GetButtonDown();
            ButtonUp = button.GetButtonUp();
        }
        public void RecordButtonState(InputChannel button)
        {
            Button = button.GetButton();
            ButtonDown = button.GetButtonDown();
            ButtonUp = button.GetButtonUp();
        }
    }
}
