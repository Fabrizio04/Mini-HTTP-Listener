namespace Mini_HTTP_Listener.configuration
{
    internal class Mini_HTTP_Listener_Configuration
    {
        public string[] ip_address { get; set; }
        public string port_number { get; set; }
        public string secure { get; set; }
        public string title { get; set; }
        public string authentication_schemes { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string start_browser { get; set; }
        public string start_host { get; set; }
        public string enable_static { get; set; }
        public string static_folder { get; set; }
        public string enable_php { get; set; }
        public string php_cgi { get; set; }
        public string php { get; set; }
    }
}
