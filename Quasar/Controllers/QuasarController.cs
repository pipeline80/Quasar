using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Logic;
using Entities;
using Quasar.Extensions;
using System.Text.Json;

namespace Quasar.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class QuasarController : ControllerBase
    {
        [HttpPost("topSecret")]
        public IActionResult TopSecret(List<Satellite> satellites)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!Communications.ValidateSateliteInfo(satellites))
                return BadRequest("The information is incomplete!!");

            Point finalPosition = Communications.GetLocation(satellites);

            if (finalPosition == null)
                return BadRequest("Ops Somethig go wrong!!");

            var reponse = new {
                position = new {
                    x = finalPosition.PositionX,
                    y = finalPosition.PositionY
                },
                message = Communications.GetMessage(satellites)
            };
            return Ok(reponse);
        }
        [HttpGet("TopSecret_Split")]
        public IActionResult TopSecretSplit(string name, double distance, string message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string cookieData = GetCookie("message");
            List<Satellite> data = new List<Satellite>();
            if (!string.IsNullOrEmpty(cookieData))
                data = JsonSerializer.Deserialize<List<Satellite>>(cookieData);

            List<Satellite> tempData = new List<Satellite>();
            Satellite st = new Satellite()
            {
                Name = name,
                Distance = distance,
                Message = message.Split(",")
            };
            tempData.Add(st);

            if (!Communications.ValidateSateliteInfo(tempData))
                return BadRequest("The information is incomplete!!");

            var FinalList = data.Union(tempData).ToList();

            if (FinalList.Count < 3)
            {
                SetCookie("message", JsonSerializer.Serialize(FinalList), 0.5);
                return BadRequest("Need more information");
            }
            else
            {
                Point finalPosition = Communications.GetLocation(FinalList);

                if (finalPosition == null)
                    return BadRequest("Ops Somethig go wrong!!");

                var reponse = new
                {
                    position = new
                    {
                        x = finalPosition.PositionX,
                        y = finalPosition.PositionY
                    },
                    message = Communications.GetMessage(FinalList)
                };
                return Ok(reponse);
            }
            
        }
        [HttpPost("TopSecret_Split")]
        public IActionResult TopSecretSplit(List<Satellite> satellites)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!Communications.ValidateSateliteInfo(satellites))
                return BadRequest("The information is incomplete!!");

            string cookieData = GetCookie("message");
            List<Satellite> data = new List<Satellite>();
            if (!string.IsNullOrEmpty(cookieData))
                data = JsonSerializer.Deserialize<List<Satellite>>(cookieData);

            var FinalList = data.Union(satellites).ToList();

            if (FinalList.Count < 3)
            {
                SetCookie("message", JsonSerializer.Serialize(FinalList), 0.5);
                return BadRequest("Need more information");
            }
            else
            {
                Point finalPosition = Communications.GetLocation(FinalList);

                if (finalPosition == null)
                    return BadRequest("Ops Somethig go wrong!!");

                var reponse = new
                {
                    position = new
                    {
                        x = finalPosition.PositionX,
                        y = finalPosition.PositionY
                    },
                    message = Communications.GetMessage(FinalList)
                };
                return Ok(reponse);
            }
        }
        [HttpPost("TopSecret_Split/{name}")]
        public IActionResult TopSecretSplit(string name, SatelliteForm satellite)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string cookieData = GetCookie("message");
            List<Satellite> data = new List<Satellite>();
            if (!string.IsNullOrEmpty(cookieData))
                data = JsonSerializer.Deserialize<List<Satellite>>(cookieData);

            List<Satellite> tempData = new List<Satellite>();
            Satellite currentSatelite = new Satellite()
            {
                Name = name,
                Distance = satellite.Distance,
                Message = satellite.Message
            };
            
            tempData.Add(currentSatelite);

            if (!Communications.ValidateSateliteInfo(tempData))
                return BadRequest("The information is incomplete!!");

            var FinalList = data.Union(tempData).ToList();

            if (FinalList.Count <= Communications.GetSatellites().Count())
            {
                SetCookie("message", JsonSerializer.Serialize(FinalList), 0.5);
                return BadRequest("Need more information");
            }
            else
            {
                Point finalPosition = Communications.GetLocation(FinalList);

                if (finalPosition == null)
                    return BadRequest("Ops Somethig go wrong!!");

                var reponse = new
                {
                    position = new
                    {
                        x = finalPosition.PositionX,
                        y = finalPosition.PositionY
                    },
                    message = Communications.GetMessage(FinalList)
                };
                return Ok(reponse);
            }
        }

        private void SetCookie(string key, string value, double? expireTime)
        {
            CookieOptions option = new CookieOptions();
            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddMilliseconds(10);
            Response.Cookies.Append(key, value, option);
        }
        private string GetCookie(string key)
        {
            return Request.Cookies[key];
        }
    }
}
