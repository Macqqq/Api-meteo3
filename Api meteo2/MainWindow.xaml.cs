using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace METEO2
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();


            // Sélectionnez "Annecy" dans la ComboBox au démarrage
            foreach (ComboBoxItem item in CB_Ville.Items)
            {
                if (item.Content.ToString() == "Annecy")
                {
                    CB_Ville.SelectedItem = item;
                    break;
                }
            }

            // Chargez les villes depuis la base de données
            CityDatabase database = LoadCityDatabase();
            foreach (string city in database.Cities)
            {
                ComboBoxItem newItem = new ComboBoxItem();
                newItem.Content = city;
                CB_Ville.Items.Add(newItem);
            }

            // Appelez la méthode GetMeteo pour charger les données météorologiques d'Annecyy
            string selectedCity = "Annecy";
            GetMeteo(selectedCity);
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
                    Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(result);

                    // Info pour aujourd'hui
                    FcstDay0 fcstDay0 = myDeserializedClass.fcst_day_0;
                    CurrentCondition currentCondition = myDeserializedClass.current_condition;

                    TB_temperature.Text = "Actuellement : " + currentCondition.tmp.ToString() + "°C";
                    TB_condition.Text = currentCondition.condition;
                    TB_Humidité.Text = currentCondition.humidity.ToString() + "% d'humidité";
                    TB_bas.Text = "Min : " + fcstDay0.tmin.ToString() + "°C";
                    TB_haut.Text = "Max : " + fcstDay0.tmax.ToString() + "°C";
                    TB_jourCourt1.Text = "date: " + fcstDay0.date.ToString();
                    Meteoicon1.Source = new BitmapImage(new Uri(fcstDay0.icon_big));
                    // Gérez l'icône en fonction de la condition météorologique
                    

                    // Info pour demain
                    FcstDay1 fcstDay1 = myDeserializedClass.fcst_day_1;

                    TB_conditionD.Text = fcstDay1.condition;
                    TB_basD.Text = "Min : " + fcstDay1.tmin.ToString() + "°C";
                    TB_hautD.Text = "Max : " + fcstDay1.tmax.ToString() + "°C";
                    TB_jourCourt2.Text = "date: : " + fcstDay1.date.ToString();
                    Meteoicon2.Source = new BitmapImage(new Uri(fcstDay1.icon_big));

                    // Info pour après-demain
                    FcstDay2 fcstDay2 = myDeserializedClass.fcst_day_2;

                    TB_conditionAD.Text = fcstDay2.condition;
                    TB_basAD.Text = "Min : " + fcstDay2.tmin.ToString() + "°C";
                    TB_hautAD.Text = "Max : " + fcstDay2.tmax.ToString() + "°C";
                    TB_jourCourt3.Text = "date : " + fcstDay2.date.ToString();
                    Meteoicon3.Source = new BitmapImage(new Uri(fcstDay2.icon_big));
                    // Info pour après-après-demain
                    FcstDay3 fcstDay3 = myDeserializedClass.fcst_day_3;

                    TB_condition3J.Text = fcstDay3.condition;
                    TB_bas3J.Text = "Min : " + fcstDay3.tmin.ToString() + "°C";
                    TB_haut3J.Text = "Max : " + fcstDay3.tmax.ToString() + "°C";
                    TB_jourLong4.Text = $"date : {fcstDay3.date}";
                    Meteoicon4.Source = new BitmapImage(new Uri(fcstDay3.icon_big));
                    // Info pour la ville
                    CityInfo cityInfo = myDeserializedClass.city_info;
                    TB_VilleInput.Text = cityInfo.name + ", " + cityInfo.country;

                    // Vérifiez si la ville existe déjà dans la ComboBox
                    bool cityExists = false;
                    foreach (ComboBoxItem item in CB_Ville.Items)
                    {
                        if (item.Content.ToString() == city)
                        {
                            cityExists = true;
                            break;
                        }
                    }

                    // Si la ville n'existe pas, ajoutez-la à la ComboBox
                    if (!cityExists)
                    {
                        ComboBoxItem newItem = new ComboBoxItem();
                        newItem.Content = city;
                        CB_Ville.Items.Add(newItem);
                    }

                    // Sélectionnez la ville ajoutée dans la ComboBox
                    foreach (ComboBoxItem item in CB_Ville.Items)
                    {
                        if (item.Content.ToString() == city)
                        {
                            CB_Ville.SelectedItem = item;
                            break;
                        }
                    }

                    return "";
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

        private async void OnRechercherClicked(object sender, RoutedEventArgs e)
        {
            string city = TB_VilleInput.Text;

            // Vérifie si la ville saisie est valide (ici, on vérifie simplement si c'est un chiffre ou non)
            if (!string.IsNullOrWhiteSpace(city) && !int.TryParse(city, out _))
            {
                await GetMeteo(city);

                // Ajoutez la ville à la base de données
                CityDatabase database = LoadCityDatabase();
                if (!database.Cities.Contains(city))
                {
                    database.Cities.Add(city);
                    SaveCityDatabase(database);
                }
            }
            else
            {
                MessageBox.Show("Veuillez entrer un nom de ville valide (les chiffres ne sont pas autorisés).");
            }
        }

        private void OnSupprimerClicked(object sender, RoutedEventArgs e)
        {
            if (CB_Ville.SelectedItem != null)
            {
                string selectedCity = (CB_Ville.SelectedItem as ComboBoxItem).Content.ToString();

                // Supprimez la ville de la liste déroulante
                CB_Ville.Items.Remove(CB_Ville.SelectedItem);

                // Chargez la base de données actuelle
                CityDatabase database = LoadCityDatabase();

                // Supprimez la ville de la base de données
                if (database.Cities.Contains(selectedCity))
                {
                    database.Cities.Remove(selectedCity);
                }

                // Sauvegardez la base de données mise à jour
                SaveCityDatabase(database);

                // Effacez les informations météorologiques affichées (vous pouvez réinitialiser les TextBlocks ou les valeurs selon votre conception)
                ClearWeatherInformation();
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une ville à supprimer.");
            }
        }

        private void ClearWeatherInformation()
        {
            // Réinitialisez les TextBlocks ou les valeurs d'informations météorologiques ici.
            TB_temperature.Text = "";
            TB_condition.Text = "";
            TB_Humidité.Text = "";
            TB_bas.Text = "";
            TB_haut.Text = "";
            TB_jourCourt1.Text = "";
            // Réinitialisez également les autres TextBlocks d'informations météorologiques si nécessaire.
        }
        private void CB_Ville_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CB_Ville.SelectedItem != null)
            {
                string selectedCity = (CB_Ville.SelectedItem as ComboBoxItem).Content.ToString();
                GetMeteo(selectedCity);
            }
        }

        private void SaveCityDatabase(CityDatabase database)
        {
            string json = JsonConvert.SerializeObject(database);
            File.WriteAllText("cityDatabase.json", json);
        }

        private CityDatabase LoadCityDatabase()
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

        public class CityInfo
        {
            public string name { get; set; }
            public string country { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string elevation { get; set; }
            public string sunrise { get; set; }
            public string sunset { get; set; }
        }

        public class CurrentCondition
        {
            public string date { get; set; }
            public string hour { get; set; }
            public int tmp { get; set; }
            public int wnd_spd { get; set; }
            public int wnd_gust { get; set; }
            public string wnd_dir { get; set; }
            public double pressure { get; set; }
            public int humidity { get; set; }
            public string condition { get; set; }
            public string condition_key { get; set; }
            public string icon { get; set; }
            public string icon_big { get; set; }
        }

        public class FcstDay0
        {
            public string date { get; set; }
            public string day_short { get; set; }
            public string day_long { get; set; }
            public int tmin { get; set; }
            public int tmax { get; set; }
            public string condition { get; set; }
            public string condition_key { get; set; }
            public string icon { get; set; }
            public string icon_big { get; set; }
        }

        public class FcstDay1
        {
            public string date { get; set; }
            public string day_short { get; set; }
            public string day_long { get; set; }
            public int tmin { get; set; }
            public int tmax { get; set; }
            public string condition { get; set; }
            public string condition_key { get; set; }
            public string icon { get; set; }
            public string icon_big { get; set; }
        }

        public class FcstDay2
        {
            public string date { get; set; }
            public string day_short { get; set; }
            public string day_long { get; set; }
            public int tmin { get; set; }
            public int tmax { get; set; }
            public string condition { get; set; }
            public string condition_key { get; set; }
            public string icon { get; set; }
            public string icon_big { get; set; }
        }

        public class FcstDay3
        {
            public string date { get; set; }
            public string day_short { get; set; }
            public string day_long { get; set; }
            public int tmin { get; set; }
            public int tmax { get; set; }
            public string condition { get; set; }
            public string condition_key { get; set; }
            public string icon { get; set; }
            public string icon_big { get; set; }
        }

        public class ForecastInfo
        {
            public object latitude { get; set; }
            public object longitude { get; set; }
            public string elevation { get; set; }
        }

        public class Root
        {
            public CityInfo city_info { get; set; }
            public ForecastInfo forecast_info { get; set; }
            public CurrentCondition current_condition { get; set; }
            public FcstDay0 fcst_day_0 { get; set; }
            public FcstDay1 fcst_day_1 { get; set; }
            public FcstDay2 fcst_day_2 { get; set; }
            public FcstDay3 fcst_day_3 { get; set; }
        }

        private void TB_jourCourt1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
