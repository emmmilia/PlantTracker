using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantTracker.Core
{
    public class WateringLog
    {
        public int Id { get; set; }
        public int PlantId { get; set; }
        public DateTime LastWatered { get; set; }
        public string Notes { get; set; }
    }
}
