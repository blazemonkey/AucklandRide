using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.UWP.Models
{
    public class CombinedData : INotifyPropertyChanged
    {
        private Route _route;
        private Trip _selectedTrip;
        private List<StopTime> _stopTimes;
        private Calendar _calendar;

        public Route Route
        {
            get { return _route; }
            set { _route = value;
                OnPropertyChanged();
            }
        }

        public Trip SelectedTrip
        {
            get { return _selectedTrip; }
            set { _selectedTrip = value;
                OnPropertyChanged();
            }
        }

        public List<StopTime> StopTimes
        {
            get { return _stopTimes; }
            set { _stopTimes = value;
                OnPropertyChanged();
            }
        }

        public Calendar Calendar
        {
            get { return _calendar; }
            set { _calendar = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
