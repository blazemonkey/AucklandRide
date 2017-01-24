using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AucklandRide.Updater.Services.RestService;
using Moq;
using RestSharp;
using System.Collections.Generic;
using AucklandRide.Updater.Models;
using System.Linq;
using AucklandRide.Services.WebClientService;
using AucklandRide.Updater.Services.SqlService;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;

namespace AucklandRide.UnitTests
{
    [TestClass]
    public class Updater
    {
        protected Mock<RestService> mockRestService;
        protected Mock<SqlService> mockSqlService;
        protected Mock<WebClientService> mockWebClientService;

        [TestInitialize]
        public void InitTests()
        {
            mockRestService = new Mock<RestService>();
            mockSqlService = new Mock<SqlService>();
            mockWebClientService = new Mock<WebClientService>();
        }

        [TestMethod]
        public void Rest_GetVersion()
        {
            var content = @"{""status"": ""OK"",
                ""response"": [{""version"": ""20170116102900_v50.11"",""startdate"": ""2017-01-19T00:00:00Z"",""enddate"": ""2017-03-19T00:00:00Z""}],
                ""error"": null}";

            var response = new RestResponse<RestWrapper<AucklandRide.Updater.Models.Version>>();
            response.Content = content;
            response.StatusCode = System.Net.HttpStatusCode.OK;

            mockRestService.Setup(x => x.ExecuteRequest<AucklandRide.Updater.Models.Version>(It.IsAny<RestClient>(),
                It.IsAny<RestRequest>())).ReturnsAsync(response);

            var version = mockRestService.Object.GetVersions().Result;
            Assert.IsNotNull(version);
            Assert.IsTrue(version.Count() == 1);
            Assert.IsNotNull(version.First().Ver);
            Assert.IsNotNull(version.First().StartDate);
            Assert.IsNotNull(version.First().EndDate);
        }

        [TestMethod]
        public void Sql_AddVersions()
        {
            var mockVersions = new Mock<DbSet<AucklandRide.Updater.Models.Version>>();
            var mockContext = new Mock<AucklandRideContext>();
            mockContext.Setup(x => x.Versions).Returns(mockVersions.Object);

            var versionsList = new List<AucklandRide.Updater.Models.Version>();
            var version1 = new AucklandRide.Updater.Models.Version { Ver = "20170116102900_v50.11", StartDate = DateTime.Parse("2017-01-19 00:00:00.000"), EndDate = DateTime.Parse("2017-03-19 00:00:00.000") };
            versionsList.Add(version1);

            mockSqlService.Setup(x => x.GetDbContext()).Returns(mockContext.Object);
            mockSqlService.Setup(x => x.OpenAsync(It.IsAny<DbContext>())).Returns(Task.FromResult(0));         
            mockSqlService.Object.AddVersions(versionsList).Wait();
            mockVersions.Verify(x => x.AddRange(It.IsAny<IEnumerable<AucklandRide.Updater.Models.Version>>()), Times.Once());
            mockContext.Verify(x => x.SaveChangesAsync(), Times.Once());
        }

        [TestMethod]
        public void WebClient_GetAgencies()
        {
            var content = @"agency_phone,agency_url,agency_id,agency_name,agency_timezone,agency_lang
(09)355-3553,http://www.aucklandtransport.govt.nz,NZBGW,Go West,Pacific/Auckland,en
(09)355-3553,http://www.aucklandtransport.govt.nz,SLPH,SeaLink Pine Harbour,Pacific/Auckland,en
(09)355-3553,http://www.aucklandtransport.govt.nz,ABEXP,SkyBus,Pacific/Auckland,en";

            mockWebClientService.Setup(x => x.DownloadFile(It.IsAny<string>())).ReturnsAsync(content);

            var agencies = mockWebClientService.Object.GetAgencies().Result;
            Assert.IsNotNull(agencies);
            Assert.IsTrue(agencies.Count() == 3);
            Assert.IsTrue(agencies.First().Id == "NZBGW");
        }

        [TestMethod]
        public void WebClient_GetCalendars()
        {
            var content = @"service_id,start_date,end_date,monday,tuesday,wednesday,thursday,friday,saturday,sunday
14020065011-20170116102900_v50.11,20170119,20170319,0,0,0,0,0,1,0
14756054300-20170116102900_v50.11,20170119,20170319,0,0,0,0,0,0,1
14313060430-20170116102900_v50.11,20170119,20170319,1,1,1,1,1,0,0";

            mockWebClientService.Setup(x => x.DownloadFile(It.IsAny<string>())).ReturnsAsync(content);

            var calendars = mockWebClientService.Object.GetCalendars().Result;
            Assert.IsNotNull(calendars);
            Assert.IsTrue(calendars.Count() == 3);
            Assert.IsTrue(calendars.First().ServiceId == "14020065011-20170116102900_v50.11");
        }

        [TestMethod]
        public void WebClient_GetCalendarDates()
        {
            var content = @"service_id,date,exception_type
14756054300-20170116102900_v50.11,20170206,1
14756054300-20170116102900_v50.11,20170130,1
14313060430-20170116102900_v50.11,20170206,2";

            mockWebClientService.Setup(x => x.DownloadFile(It.IsAny<string>())).ReturnsAsync(content);

            var calendars = mockWebClientService.Object.GetCalendarDates().Result;
            Assert.IsNotNull(calendars);
            Assert.IsTrue(calendars.Count() == 3);
            Assert.IsTrue(calendars.First().ServiceId == "14756054300-20170116102900_v50.11");
        }

        [TestMethod]
        public void WebClient_GetRoutes()
        {
            var content = @"route_long_name,route_type,route_text_color,agency_id,route_id,route_color,route_short_name
Belmont Intermediate To Stanley Bay,3,,RTH,08152-20161207093526_v48.9,,081
Newmarket To Beach Haven Via Ponsonby,3,,BTL,96602-20170116102900_v50.11,,966
Mayoral Dr To Mairangi Bay Express,3,,NZBNS,86304-20170116102900_v50.11,,863X";

            mockWebClientService.Setup(x => x.DownloadFile(It.IsAny<string>())).ReturnsAsync(content);

            var routes = mockWebClientService.Object.GetRoutes().Result;
            Assert.IsNotNull(routes);
            Assert.IsTrue(routes.Count() == 3);
            Assert.IsTrue(routes.First().Id == "08152-20161207093526_v48.9");
        }

        [TestMethod]
        public void WebClient_GetShapes()
        {
            var content = @"shape_id,shape_pt_lat,shape_pt_lon,shape_pt_sequence,shape_dist_traveled
1097-20170116102900_v50.11,-36.88387,174.73319,1,
1097-20170116102900_v50.11,-36.88391,174.73316,2,
1097-20170116102900_v50.11,-36.88394,174.73322,3,";

            mockWebClientService.Setup(x => x.DownloadFile(It.IsAny<string>())).ReturnsAsync(content);

            var shapes = mockWebClientService.Object.GetShapes().Result;
            Assert.IsNotNull(shapes);
            Assert.IsTrue(shapes.Count() == 3);
            Assert.IsTrue(shapes.First().Id == "1097-20170116102900_v50.11");
        }

        [TestMethod]
        public void WebClient_GetStops()
        {
            var content = @"stop_lat,zone_id,stop_lon,stop_id,parent_station,stop_desc,stop_name,location_type,stop_code
-36.82308,,174.80429,3480,41023,,40 Vauxhall Rd,0,3480
-36.87175,,174.60227,21094,,,Metcalfe Rd and Ranui,1,21094
-36.89001,,174.77511,11542,,,Manukau Rd and King George,1,11542";

            mockWebClientService.Setup(x => x.DownloadFile(It.IsAny<string>())).ReturnsAsync(content);

            var stops = mockWebClientService.Object.GetStops().Result;
            Assert.IsNotNull(stops);
            Assert.IsTrue(stops.Count() == 3);
            Assert.IsTrue(stops.First().Id == "3480");
        }

        [TestMethod]
        public void WebClient_GetStopTimes()
        {
            var content = @"trip_id,arrival_time,departure_time,stop_id,stop_sequence,stop_headsign,pickup_type,drop_off_type,shape_dist_traveled
14020065011-20170116102900_v50.11,19:30:00,19:30:00,8492,1,,,,
14020065011-20170116102900_v50.11,19:30:40,19:30:40,8490,2,,,,
14020065011-20170116102900_v50.11,19:31:04,19:31:04,8488,3,,,,";

            mockWebClientService.Setup(x => x.DownloadFile(It.IsAny<string>())).ReturnsAsync(content);

            var stops = mockWebClientService.Object.GetStopTimes().Result;
            Assert.IsNotNull(stops);
            Assert.IsTrue(stops.Count() == 3);
            Assert.IsTrue(stops.First().TripId == "14020065011-20170116102900_v50.11");
        }

        [TestMethod]
        public void WebClient_GetTrips()
        {
            var content = @"block_id,route_id,direction_id,trip_headsign,shape_id,service_id,trip_id
,02001-20170116102900_v50.11,0,Britomart,3-20170116102900_v50.11,14020065011-20170116102900_v50.11,14020065011-20170116102900_v50.11
,75602-20170116102900_v50.11,1,Panmure,866-20170116102900_v50.11,14756054300-20170116102900_v50.11,14756054300-20170116102900_v50.11
,31301-20170116102900_v50.11,0,Manukau,1165-20170116102900_v50.11,14313060430-20170116102900_v50.11,14313060430-20170116102900_v50.11";

            mockWebClientService.Setup(x => x.DownloadFile(It.IsAny<string>())).ReturnsAsync(content);

            var trips = mockWebClientService.Object.GetTrips().Result;
            Assert.IsNotNull(trips);
            Assert.IsTrue(trips.Count() == 3);
            Assert.IsTrue(trips.First().Id == "14020065011-20170116102900_v50.11");
        }
    }
}
