using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.UWP.Models.AT
{
    public class VehicleTripInfo
    {
        [JsonProperty(PropertyName = "trip_id")]
        public string TripId { get; set; }
        [JsonProperty(PropertyName = "route_id")]
        public string RouteId { get; set; }
        [JsonProperty(PropertyName = "start_time")]
        public string StartTime { get; set; }
        [JsonProperty(PropertyName = "schedule_relationship")]
        public int ScheduleRelationship { get; set; }        
    }

    public class VehicleId
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }

    public class Position
    {
        [JsonProperty(PropertyName = "latitude")]
        public double Latitude { get; set; }
        [JsonProperty(PropertyName = "longitude")]
        public double Longitude { get; set; }
        [JsonProperty(PropertyName = "bearing")]
        public string Bearing { get; set; }
    }

    public class Vehicle
    {
        [JsonProperty(PropertyName = "trip")]
        public VehicleTripInfo Trip { get; set; }
        [JsonProperty(PropertyName = "vehicle")]
        public VehicleId VehicleId { get; set; }
        [JsonProperty(PropertyName = "position")]
        public Position Position { get; set; }
        [JsonProperty(PropertyName = "timestamp")]
        public double Timestamp { get; set; }
    }

    public class VehicleEntity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "is_deleted")]
        public bool IsDeleted { get; set; }
        [JsonProperty(PropertyName = "vehicle")]
        public Vehicle Vehicle { get; set; }
    }

    public class VehicleHeader
    {
        [JsonProperty(PropertyName = "gtfs_realtime_version")]
        public string GtfsRealTimeVersion { get; set; }
        [JsonProperty(PropertyName = "incrementality")]
        public int Incrementality { get; set; }
        [JsonProperty(PropertyName = "timestamp")]
        public double Timestamp { get; set; }
    }

    public class VehicleResponse
    {
        [JsonProperty(PropertyName = "entity")]
        public List<VehicleEntity> Entity { get; set; }
        [JsonProperty(PropertyName = "header")]
        public VehicleHeader Header { get; set; }
    }

    public class ActiveVehicle
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        [JsonProperty(PropertyName = "response")]
        public VehicleResponse Response { get; set; }
        [JsonProperty(PropertyName = "error")]
        public object Error { get; set; }
    }
}
