using Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace Logic
{
    public static class Communications
    {
        /// <summary>
        /// Get the location of the ship
        /// </summary>
        /// <param name="satellites">Distance to each ship</param>
        /// <returns>Point of the ship</returns>
        public static Point GetLocation(List<Satellite> satellites)
        {
            try
            {
                if (satellites.Count() < GetSatellites().Count())
                    return null;

                foreach (var item in satellites)
                {
                    item.Point = GetSatelliteposition(item.Name);
                }

                double S2S1Distance = Math.Pow(Math.Pow(satellites[1].Point.PositionX - satellites[0].Point.PositionX, 2) + Math.Pow(satellites[1].Point.PositionY - satellites[0].Point.PositionY, 2), 0.5);
                Point ex = new Point(); ex.PositionX = (satellites[1].Point.PositionX - satellites[0].Point.PositionX) / S2S1Distance; ex.PositionY = (satellites[1].Point.PositionY - satellites[0].Point.PositionY) / S2S1Distance;
                Point aux = new Point(); aux.PositionX = satellites[2].Point.PositionX - satellites[0].Point.PositionX; aux.PositionY = satellites[2].Point.PositionY - satellites[0].Point.PositionY;
                double i = ex.PositionX * aux.PositionX + ex.PositionY * aux.PositionY;
                Point aux2 = new Point(); aux2.PositionX = satellites[2].Point.PositionX - satellites[0].Point.PositionX - i * ex.PositionX; aux2.PositionY = satellites[2].Point.PositionY - satellites[0].Point.PositionY - i * ex.PositionY;
                Point ey = new Point(); ey.PositionX = aux2.PositionX / Norm(aux2); ey.PositionY = aux2.PositionY / Norm(aux2);

                double j = ey.PositionX * aux.PositionX + ey.PositionY * aux.PositionY;

                double x = (Math.Pow(satellites[0].Distance, 2) - Math.Pow(satellites[1].Distance, 2) + Math.Pow(S2S1Distance, 2)) / (2 * S2S1Distance);
                double y = (Math.Pow(satellites[0].Distance, 2) - Math.Pow(satellites[2].Distance, 2) + Math.Pow(i, 2) + Math.Pow(j, 2)) / (2 * j) - i * x / j;
                //result coordinates
                double finalX = satellites[0].Point.PositionX + x * ex.PositionX + y * ey.PositionX;
                double finalY = satellites[0].Point.PositionY + x * ex.PositionY + y * ey.PositionY;

                Point returnPoint = new Point();
                returnPoint.PositionX = finalX; returnPoint.PositionY = finalY;
                return returnPoint;
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }
        /// <summary>
        /// Get the message of the ship
        /// </summary>
        /// <param name="satellites">Message sent to each satellite</param>
        /// <returns>Message complete</returns>
        public static string GetMessage(List<Satellite> satellites)
        {
            string message = string.Empty;
            string[] finalArray = new string[0];
            foreach (Satellite item in satellites)
            {
                if (finalArray.Length <= 0)
                {
                    finalArray = new string[item.Message.Length];
                    finalArray = item.Message;
                }
                else
                {
                    for (int i = 0; i < item.Message.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(item.Message[i]))
                        {
                            finalArray[i] = item.Message[i];
                        }
                    }
                }
            }
            return string.Join(" ", finalArray);
        }
        /// <summary>
        /// Get configured Satellites with position
        /// </summary>
        /// <returns>SatellitePosition</returns>
        public static IEnumerable<SatellitePosition> GetSatellites()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var jsonString = File.ReadAllText("Satellites.json");
            var jsonModel = JsonSerializer.Deserialize<List<SatellitePosition>>(jsonString, options);

            return jsonModel.ToList();
        }
        /// <summary>
        /// validate if a list of satellites have all the info
        /// </summary>
        /// <param name="satellites">List o satellites to validate</param>
        /// <returns>False if some of the values is missing / True if all the values are complete</returns>
        public static bool ValidateSateliteInfo(List<Satellite> satellites)
        {
            foreach (var satellite in satellites)
            {
                if (string.IsNullOrEmpty(satellite.Name))
                    return false;
                if (satellite.Distance <= 0)
                    return false;
                if (satellite.Message.Length <= 0)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Get the norm of a vector
        /// </summary>
        /// <param name="p">Point for vectors</param>
        /// <returns>norm vector number</returns>
        private static double Norm(Point p)
        {
            return Math.Pow(Math.Pow(p.PositionX, 2) + Math.Pow(p.PositionY, 2), 0.5);
        }
        /// <summary>
        /// Get the position of a configured satelyte by name
        /// </summary>
        /// <param name="satelliteName">Satelite name</param>
        /// <returns>Point of the satelite</returns>
        private static Point GetSatelliteposition(string satelliteName)
        {
  
            List<SatellitePosition> jsonModel = GetSatellites().ToList();

            return(jsonModel.Find(x => x.Name.ToLower().Equals(satelliteName.ToLower())).Position);
        }
        
    }
}
