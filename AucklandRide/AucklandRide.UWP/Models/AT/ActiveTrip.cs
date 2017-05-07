using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.UWP.Models.AT
{
    public class TripHeader
    {
        [JsonProperty(PropertyName = "gtfs_realtime_version")]
        public string GtfsRealTimeVersion { get; set; }
        [JsonProperty(PropertyName = "incrementality")]
        public int Incrementality { get; set; }
        [JsonProperty(PropertyName = "timestamp")]
        public double Timestamp { get; set; }
    }

    public class TripInfo
    {
        [JsonProperty(PropertyName = "trip_id")]
        public string TripId { get; set; }
        [JsonProperty(PropertyName = "route_id")]
        public string RouteId { get; set; }
        [JsonProperty(PropertyName = "schedule_relationship")]
        public int ScheduleRelationship { get; set; }
    }

    public class TripVehicle
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }

    public class Arrival
    {
        [JsonProperty(PropertyName = "delay")]
        public int Delay { get; set; }
        [JsonProperty(PropertyName = "time")]
        public double Time { get; set; }
    }

    public class Departure
    {
        [JsonProperty(PropertyName = "delay")]
        public int Delay { get; set; }
        [JsonProperty(PropertyName = "time")]
        public double Time { get; set; }
    }

    public class StopTimeUpdate : INotifyPropertyChanged
    {
        private string _stopName;
        private string _stopRegionName;

        [JsonProperty(PropertyName = "stop_sequence")]
        public int StopSequence { get; set; }
        [JsonProperty(PropertyName = "stop_id")]
        public string StopId { get; set; }
        [JsonProperty(PropertyName = "schedule_relationship")]
        public int ScheduleRelationship { get; set; }
        [JsonProperty(PropertyName = "arrival")]
        public Arrival Arrival { get; set; }
        [JsonProperty(PropertyName = "departure")]
        public Departure Departure { get; set; }
        public string StopName
        {
            get { return _stopName; }
            set { _stopName = value;
                OnPropertyChanged();
            }
        }
        public string StopRegionName
        {
            get { return _stopRegionName; }
            set { _stopRegionName = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TripUpdate
    {
        [JsonProperty(PropertyName = "trip")]
        public TripInfo Trip { get; set; }
        [JsonProperty(PropertyName = "vehicle")]
        public TripVehicle Vehicle { get; set; }
        [JsonProperty(PropertyName = "stop_time_update")]
        public StopTimeUpdate StopTimeUpdate { get; set; }
        [JsonProperty(PropertyName = "timestamp")]
        public double Timestamp { get; set; }
    }

    public class TripEntity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "is_deleted")]
        public bool IsDeleted { get; set; }
        [JsonProperty(PropertyName = "trip_update")]
        public TripUpdate TripUpdate { get; set; }
    }

    public class TripResponse
    {
        [JsonProperty(PropertyName = "header")]
        public TripHeader Header { get; set; }
        [JsonProperty(PropertyName = "entity")]
        public List<TripEntity> Entity { get; set; }
    }

    public class ActiveTrip
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        [JsonProperty(PropertyName = "response")]
        public TripResponse Response { get; set; }
        [JsonProperty(PropertyName = "error")]
        public object Error { get; set; }
    }
}
