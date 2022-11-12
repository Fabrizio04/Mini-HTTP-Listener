namespace Mini_HTTP_Listener.configuration
{
    internal class CustomPath : Attribute
    {
        public string URL { get; private set; }
        public string Method { get; private set; }
        public CustomPath(string URL)
        {
            this.URL = URL;
            Method = "GET";
        }
        public CustomPath(string URL, string Method)
        {
            this.URL = URL;
            this.Method = Method;
        }
    }
}
