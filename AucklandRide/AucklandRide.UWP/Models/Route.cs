﻿using AucklandRide.UWP.Models.AT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.UWP.Models
{
    public class Route
    {
        public string Id { get; set; }
        public string AgencyId { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public byte Type { get; set; }
        public string Color { get; set; }
        public string TextColor { get; set; }
        public string AgencyName { get; set; }
        public string ShortId { get; set; }
        public List<Trip> Trips { get; set; }
        public bool IsLive { get; set; }
        public List<TripEntity> ActiveTrips { get; set; }
    }
}
