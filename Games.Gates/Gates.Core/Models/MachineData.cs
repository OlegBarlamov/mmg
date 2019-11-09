using System.Collections.Generic;
using System.Drawing;

namespace Gates.Core.Models
{
    public class MachineData
    {
        /// <summary>
        /// Bottom, Left point
        /// </summary>
        public Point Position { get; set; }

        public List<Particle> Particles { get; set; }

        public bool Direction { get; set; }

        public int AdditionalSpeed { get; set; }
    }
}
