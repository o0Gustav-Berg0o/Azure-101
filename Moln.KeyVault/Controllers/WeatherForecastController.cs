using Microsoft.AspNetCore.Mvc;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Core;
using System;
using Azure;

namespace Moln.KeyVault.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        public WeatherForecastController()
        {


        }


        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<string> GetAsync()
        {

            return "not";
        }
    }
}