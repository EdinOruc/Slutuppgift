using LabShortestRouteFinder.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace LabShortestRouteFinder.ViewModel
{
    public class RouteViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Route> Routes { get; }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private Route _selectedRoute;
        public Route SelectedRoute
        {
            get => _selectedRoute;
            set
            {
                _selectedRoute = value;
                OnPropertyChanged(nameof(SelectedRoute));
            }
        }
        public RouteViewModel(MainViewModel mainViewModel)
        {
            // Reference the shared Routes collection
            Routes = mainViewModel.Routes;
        }
    }
}
