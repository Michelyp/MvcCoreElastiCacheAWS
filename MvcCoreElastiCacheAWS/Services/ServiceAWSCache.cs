using Microsoft.Extensions.Caching.Distributed;
using MvcCoreElastiCacheAWS.Helpers;
using MvcCoreElastiCacheAWS.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MvcCoreElastiCacheAWS.Services
{
    public class ServiceAWSCache
    {
        private IDistributedCache cache;
        public ServiceAWSCache()
        {
            this.cache = cache;
        }
        public async Task<List<Coche>> GetCochesFavoritosAsync()
        {
            //Almacenaremos una coleccion de coches en formato JSON
            //Las keys deben ser unicas para cada user
            string jsonCoches = await this.cache.GetStringAsync("cochesfavoritos");
            if(jsonCoches == null)
            {
                return null;
            }
            else
            {
                List<Coche> cars = JsonConvert.DeserializeObject<List<Coche>>(jsonCoches);
                return cars;
            }
        }
        public async Task AddCocheFavoritoAsync(Coche car)
        {
            List<Coche> coches = await this.GetCochesFavoritosAsync();
            //Si no existen coches favoritos todavía, creamos 
            //la colección
            if(coches == null)
            {
                coches = new List<Coche>();
                //Añadimos el nuevo coche de la colección 
                coches.Add(car);
                //Serializamos a JSON la colección 
                string jsonCoches = JsonConvert.SerializeObject(coches);
                DistributedCacheEntryOptions options = new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                };
                //Almacenamos la coleccion dentro de cache redis 
                //indicaremos que los datos duran 30 minutos
                await this.cache.SetStringAsync("cochesfavoritos", jsonCoches,options);

            }
        }
        public async Task DeleteCocheFavoritoAsync(int idcoche)
        {
            List<Coche> cars = await this.GetCochesFavoritosAsync();
            if (cars != null)
            {
                Coche cocheEliminar = cars.FirstOrDefault(x => x.IdCoche == idcoche);
                cars.Remove(cocheEliminar);
                //Comprobamos si la coleccion tiene coches favoritos 
                //todavía o no tiene 
                //si no tenemos coches, eliminamos la key de cache redis
                if(cars.Count == 0)
                {
                    await this.cache.RemoveAsync("cochesfavoritos");
                }
                else
                {
                    //Almacenamos de nuevo los coches sin el car eliminado
                    string jsonCoches = JsonConvert.SerializeObject(cars);
                    DistributedCacheEntryOptions options = new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(30)
                    };
                    //actualizamos el cache redis
                    await this.cache.SetStringAsync("cochesfavoritos", jsonCoches,options);
                }
            }
        }
    }
}
