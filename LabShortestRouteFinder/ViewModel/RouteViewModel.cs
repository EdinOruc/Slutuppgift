using LabShortestRouteFinder.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LabShortestRouteFinder.ViewModel
{
    public class RouteViewModel
    {
        public ObservableCollection<Route> Routes { get; }
        public event PropertyChangedEventHandler PropertyChanged;
        public RouteViewModel(MainViewModel mainViewModel)
        {
            // Reference the shared Routes collection
            Routes = mainViewModel.Routes;
        }
    }
}
