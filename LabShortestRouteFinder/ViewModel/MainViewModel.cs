using LabShortestRouteFinder.Converters;
using LabShortestRouteFinder.Helpers;
using LabShortestRouteFinder.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Xml.Linq;

namespace LabShortestRouteFinder.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<CityNode> Cities { get; private set; }
        public ObservableCollection<Route> Routes { get; private set; }

                

        private MapTransformer? mapTransformer;

        public MainViewModel()
        {
            // Initialize data here or load from JSON
            Cities = new ObservableCollection<CityNode>();
            Routes = new ObservableCollection<Route>();

            LoadRectangleAndCitiesFromJson();
            LoadRouteInformationFileFromJson();

            NormalizeCoordinates();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void NormalizeCoordinates()
        {
            int maxX = Cities.Max(c => c.X);
            int maxY = Cities.Max(c => c.Y);

            foreach (var city in Cities)
            {
                city.X = (city.X * 433) / maxX; // Normalize to Canvas width
                city.Y = (city.Y * 842) / maxY; // Normalize to Canvas height
            }

            foreach (var route in Routes)
            {
                route.Start.X = (route.Start.X * 433) / maxX; // Normalize to Canvas width
                route.Start.Y = (route.Start.Y * 842) / maxY; // Normalize to Canvas height
                route.Destination.X = (route.Destination.X * 433) / maxX; // Normalize to Canvas width
                route.Destination.Y = (route.Destination.Y * 842) / maxY; // Normalize to Canvas height
            }
        }

        private void LoadRectangleAndCitiesFromJson()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "swCities.json");
            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                var jsonDocument = JsonDocument.Parse(jsonString);

                if (jsonDocument.RootElement.ValueKind == JsonValueKind.Array && jsonDocument.RootElement.GetArrayLength() > 0)
                {
                    var rootElement = jsonDocument.RootElement[0];

                    if (rootElement.TryGetProperty("Rectangle", out JsonElement rectangleElement) && rectangleElement.ValueKind == JsonValueKind.Object)
                    {
                        var rectangle = JsonSerializer.Deserialize<RectangleCoordinates>(rectangleElement.GetRawText());
                        var cities = JsonSerializer.Deserialize<List<CityNode>>(rootElement.GetProperty("Cities").GetRawText());

                        if (rectangle != null && cities != null)
                        {
                            foreach (var city in cities)
                            {
                                Cities.Add(city);
                            }

                            var minGpsCoord = new Tuple<double, double>(rectangle.SouthWest.Latitude, rectangle.SouthWest.Longitude);
                            var maxGpsCoord = new Tuple<double, double>(rectangle.NorthEast.Latitude, rectangle.NorthEast.Longitude);

                            int windowWidth = (int)WGS84DistanceCalculator.CalculateDistance(rectangle.NorthEast.Latitude, rectangle.NorthEast.Longitude, rectangle.NorthWest.Latitude, rectangle.NorthWest.Longitude);
                            int windowHeight = (int)WGS84DistanceCalculator.CalculateDistance(rectangle.NorthEast.Latitude, rectangle.NorthEast.Longitude, rectangle.SouthEast.Latitude, rectangle.SouthEast.Longitude);

                            var windowMaxXY = new Tuple<int, int>(windowWidth, windowHeight);

                            MapWin mapWin = new MapWin(minGpsCoord, maxGpsCoord, windowMaxXY);
                            mapTransformer = new MapTransformer(mapWin);

                            var transformedCities = mapTransformer.TransformCities(Cities);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("The 'Rectangle' property is not an object.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("The root element is not a non-empty array.");
                }
            }
            else
            {
                throw new FileNotFoundException($"The file {filePath} was not found.");
            }
        }

        private void LoadRouteInformationFileFromJson()
        {
            var  filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Routes.json");

            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                var jsonDocument = JsonDocument.Parse(jsonString);

                var routes = JsonSerializer.Deserialize<List<Route>>(jsonDocument.RootElement.GetRawText());

                if (routes != null && mapTransformer != null)
                {
                    foreach (var route in routes)
                    {
                        (route.Start.X, route.Start.Y) = mapTransformer.TransformToScreenPosition(route.Start.Latitude, route.Start.Longitude);
                        (route.Destination.X, route.Destination.Y) = mapTransformer.TransformToScreenPosition(route.Destination.Latitude, route.Destination.Longitude);
                        Routes.Add(route);
                    }
                }
                else
                {
                    throw new InvalidOperationException("The routes could not be deserialized or the mapTransformer is null.");
                }
            }
            else
            {
                throw new FileNotFoundException($"The file {filePath} was not found.");
            }
        }
    }
}
