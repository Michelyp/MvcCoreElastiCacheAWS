using StackExchange.Redis;

namespace MvcCoreElastiCacheAWS.Helpers
{
    public class HelperCacheRedis
    {
        public static Lazy<ConnectionMultiplexer>
            CreateConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                //Aquí iria nuestra cadena de conexión 
                string connectionString = "cache-coches.hfvwlc.ng.0001.use1.cache.amazonaws.com:6379";
                return ConnectionMultiplexer.Connect(connectionString);
            });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return CreateConnection.Value;
            }
        }
    }
}
