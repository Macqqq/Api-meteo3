using System;
using System.IO;
using Newtonsoft.Json;

namespace METEO2
{
    public partial class Class1
    {
        private const string PreferencesFileName = "userPreferences.json";

        // Classe pour stocker les préférences de l'utilisateur
        public class UserPreferences
        {
            public string TemperatureUnit { get; set; } // Vous pouvez utiliser "Celsius" ou "Fahrenheit"
            // Ajoutez d'autres préférences ici si nécessaire
        }

        // Méthode pour récupérer les préférences de l'utilisateur
        public UserPreferences GetPreferences()
        {
            UserPreferences preferences = new UserPreferences();
            try
            {
                if (File.Exists(PreferencesFileName))
                {
                    string json = File.ReadAllText(PreferencesFileName);
                    preferences = JsonConvert.DeserializeObject<UserPreferences>(json);
                }
            }
            catch (Exception ex)
            {
                // Gérez les erreurs de lecture du fichier JSON
                Console.WriteLine("Erreur lors de la récupération des préférences : " + ex.Message);
            }

            return preferences;
        }

        // Méthode pour mettre à jour les préférences de l'utilisateur
        public void UpdatePreferences(UserPreferences preferences)
        {
            try
            {
                string json = JsonConvert.SerializeObject(preferences);
                File.WriteAllText(PreferencesFileName, json);
            }
            catch (Exception ex)
            {
                // Gérez les erreurs d'écriture du fichier JSON
                Console.WriteLine("Erreur lors de la mise à jour des préférences : " + ex.Message);
            }
        }
    }
}