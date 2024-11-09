namespace Model.Entities
{
    public class Session
    {
        private Session() { }
        private static Session _instance;

        public static Session GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Session();
            }
            return _instance;
        }
        public string IdUserLogueado { get; set; }
        public string EmailUserLogueado { get; set; }
        public string UserNameUserLogueado { get; set; }
    }
}
