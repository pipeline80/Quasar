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
        /// <summary>
        /// Determine the postion of point with the disance and the position of 3 knows point
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///	    [
        ///		    {
        ///			    "name": "Kenobi",
        ///			    "distance": 100,
        ///		    	"message": ["este", "", "", "mensaje",""]
        ///	    	},
        ///	    	{
        ///	    		"name": "Skywalker",
        ///	    		"distance": 115.5,
        ///	    		"message": ["", "es", "", "", "secreto"]
        ///	    	},
        ///	    	{
        ///	    		"name": "Sato",
        ///	    		"distance": 142.7,
        ///	    		"message": ["este", "", "un", "mensaje", ""]
        ///	    	}
        ///	    ]
        /// </remarks>
        /// <param name="satellites">List of satelites and the distance to each</param>
        /// <returns>Json with the position and message</returns>
        /// <response code="200">
        /// {
        ///		"position": {
        ///		"x": -202.78354166666668,
        ///		"y": -17.557599999999866
        ///		},
        ///		"message": "este es un mensaje secreto"
        ///	}
        ///	</response>
        /// <response code="400">If the information is incomplete!!</response>
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
        /// <summary>
        /// Determine the postion of point with the disance and the position of 3 knows point. Need a request to the server for each satellite
        /// </summary>
        /// <param name="name">Satellite Name</param>
        /// <param name="distance">distance to the satellite</param>
        /// <param name="message">part of the message in string comma separated Sample: message=este,es,un,,secreto</param>
        /// <returns>Json with the position and message</returns>
        /// <response code="200">
        /// {
        ///		"position": {
        ///		"x": -202.78354166666668,
        ///		"y": -17.557599999999866
        ///		},
        ///		"message": "este es un mensaje secreto"
        ///	}
        ///	</response>
        /// <response code="400">If the information is incomplete!!</response>
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
                DeleteCookie("message");
                return Ok(reponse);
            }
            
        }
        /// <summary>
        /// Determine the postion of point with the disance and the position of 3 knows point
        /// Need a request to the server to complete the info of all the satelites
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///	    [
        ///		    {
        ///			    "name": "Kenobi",
        ///			    "distance": 100,
        ///		    	"message": ["este", "", "", "mensaje",""]
        ///	    	},
        ///	    	{
        ///	    		"name": "Skywalker",
        ///	    		"distance": 115.5,
        ///	    		"message": ["", "es", "", "", "secreto"]
        ///	    	}
        ///	    ]
        /// </remarks>
        /// <param name="satellites"></param>
        /// <returns>List of satelites and the distance to each one Can have 1 2 or 3 satellites in the request</returns>
        /// <response code="200">
        /// {
        ///		"position": {
        ///		"x": -202.78354166666668,
        ///		"y": -17.557599999999866
        ///		},
        ///		"message": "este es un mensaje secreto"
        ///	}
        ///	</response>
        /// <response code="400">If the information is incomplete!!</response>
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
                DeleteCookie("message");
                return Ok(reponse);
            }
        }
        /// <summary>
        /// Determine the postion of point with the disance and the position of 3 knows point
        /// Need a request to the server to complete the info of all the satelites
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///	    [
        ///		    {
        ///			    "distance": 100,
        ///		    	"message": ["este", "", "", "mensaje",""]
        ///	    	}
        ///	    ]
        /// </remarks>
        /// <param name="name"></param>
        /// <param name="satellite"></param>
        /// <returns>List of satelites and the distance to each one
        /// Can have 1 2 or 3 satellites in the request</returns>
        /// <response code="200">
        /// {
        ///		"position": {
        ///		"x": -202.78354166666668,
        ///		"y": -17.557599999999866
        ///		},
        ///		"message": "este es un mensaje secreto"
        ///	}
        ///	</response>
        /// <response code="400">If the information is incomplete!!</response>
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
                DeleteCookie("message");
                return Ok(reponse);
            }
        }
        /// <summary>
        /// Set a cookie to save the satelite info
        /// </summary>
        /// <param name="key">cookie key</param>
        /// <param name="value">json with the satellite info</param>
        /// <param name="expireTime">time to expire the cookie</param>
        private void SetCookie(string key, string value, double? expireTime)
        {
            CookieOptions option = new CookieOptions();
            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddMilliseconds(10);
            Response.Cookies.Append(key, value, option);
        }
        /// <summary>
        /// Get a cookie with the satelite info
        /// </summary>
        /// <param name="key">cookie key</param>
        /// <returns></returns>
        private string GetCookie(string key)
        {
            return Request.Cookies[key];
        }
        /// <summary>
        /// Remove the cookie
        /// </summary>
        /// <param name="key">cookie key</param>
        private void DeleteCookie(string key)
        {
            Response.Cookies.Delete(key);
        }
    }
}
