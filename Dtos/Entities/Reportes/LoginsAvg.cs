using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos.Entities.Reportes
{
    public class LoginsAvg
    {
        public List<decimal> LoginsAvgs { get; set; }
        public List<double> SessionTimeAvgs { get; set; }
    }
}
