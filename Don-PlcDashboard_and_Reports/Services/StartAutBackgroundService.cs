using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Don_PlcDashboard_and_Reports.Services
{
    public class StartAutBackgroundService
    {
        private TimedService _backgroundService;
        
        public StartAutBackgroundService(TimedService backgroundService)
        {
            _backgroundService = backgroundService;
            _backgroundService.StartAsync(new CancellationToken());
            Console.WriteLine("Constructor Start automat backgroundService");
        }
    }
}
