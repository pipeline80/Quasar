using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities
{
    public class Satellite : IEquatable<Satellite>
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public double Distance { get; set; }
        [Required]
        public string[] Message { get; set; }
        public Point Point { get; set; }

        public override int GetHashCode()
        {
            return (Name == null ? 0 : Name.GetHashCode());
        }
        public bool Equals(Satellite satellite)
        {
            return Name.Equals(satellite.Name);
        }
    }
}
