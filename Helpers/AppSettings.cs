namespace ncache_dotnet.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string RedisHost { get; set; }
        public string RedisPassword { get; set; }

        public string RedisInstanceName { get; set; }
    }
}
