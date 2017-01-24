using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AucklandRide.Updater.Services.RestService;
using Moq;
using RestSharp;
using System.Collections.Generic;
using AucklandRide.Updater.Models;

namespace AucklandRide.UnitTests
{
    [TestClass]
    public class Updater
    {
        protected Mock<RestService> mockRestService;

        [TestInitialize]
        public void InitTests()
        {
            mockRestService = new Mock<RestService>();
        }

        [TestMethod]
        public void Rest_GetVersion()
        {
            var content = @"{
              \""status\"": \""OK\"",
              \""response\"": [
                {
                  \""version\"": \""20170116102900_v50.11\"",
                  \""startdate\"": \""2017-01-19T00:00:00Z\"",
                  \""enddate\"": \""2017-03-19T00:00:00Z\""
                }
              ],
              \""error\"": null
            }";

            var response = new RestResponse<RestWrapper<AucklandRide.Updater.Models.Version>>();
            response.Content = content;
            response.StatusCode = System.Net.HttpStatusCode.OK;

            mockRestService.Setup(x => x.ExecuteRequest<RestWrapper<AucklandRide.Updater.Models.Version>>(It.IsAny<RestClient>(), 
                It.IsAny<RestRequest>())).ReturnsAsync(response

                );
        }
    }
}
