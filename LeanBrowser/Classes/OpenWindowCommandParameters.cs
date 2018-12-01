namespace LeanBrowser
{
    public class OpenWindowCommandParameters
    {
        public string [] Args { get; private set; }

        public OpenWindowCommandParameters(string[] args)
        {
            Args = args;
        }
    }
}
