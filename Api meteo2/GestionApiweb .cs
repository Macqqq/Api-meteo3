using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO; // Ajoutez cette directive
using System.Net.Http;
using System.Threading.Tasks;
using static METEO2_taha_khelifi.MainWindow;

namespace METEO2_taha_khelifi
{
    public class Class2
    {
        private HttpClient _httpClient;

        public Class2()
        {
            // Initialisez votre client HTTP ici
            _httpClient = new HttpClient();
        }

        public async Task<string> GetWeatherData(string city)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"https://www.prevision-meteo.ch/services/json/{city}");
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return result;
                }
                else
                {
                    return "Error";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async Task<string> GetMeteo(string city)
        {
            HttpClient client = new HttpClient();
            try
            {
                HttpResponseMessage response = await client.GetAsync($"https://www.prevision-meteo.ch/services/json/{city}");
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return result;
                }
                else
                {
                    return "Error";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }



        // Vous pouvez ajouter d'autres méthodes pour interagir avec d'autres services ou API ici
        public void SaveCityDatabase(CityDatabase database)
        {
            string json = JsonConvert.SerializeObject(database);
            File.WriteAllText("cityDatabase.json", json);
        }

        public CityDatabase LoadCityDatabase()
        {
            if (File.Exists("cityDatabase.json"))
            {
                string json = File.ReadAllText("cityDatabase.json");
                return JsonConvert.DeserializeObject<CityDatabase>(json);
            }
            else
            {
                return new CityDatabase();
            }
        }

        public class CityDatabase
        {
            public List<string> Cities { get; set; } = new List<string>();
        }
    }
}