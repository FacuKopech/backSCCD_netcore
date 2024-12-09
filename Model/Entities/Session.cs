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
        public string Token { get; set; }
    }
}
