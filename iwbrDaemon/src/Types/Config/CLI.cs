namespace IwbrDaemon.Types.Config 
{
    public class CLI
    {
        public bool IsEnabled { get; private set; }

        public CLI(bool isEnabled) 
        { 
            IsEnabled = isEnabled; 
        }
    }
}