using Don_PlcDashboard_and_Reports.Models;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Don_PlcDashboard_and_Reports.Services
{
    public interface IPlcService
    {
        public Plc GetNewPlc(PlcModel plcModel);
    }
}
