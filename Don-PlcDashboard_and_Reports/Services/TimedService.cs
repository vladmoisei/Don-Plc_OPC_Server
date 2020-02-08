using Don_PlcDashboard_and_Reports.Data;
using Don_PlcDashboard_and_Reports.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Don_PlcDashboard_and_Reports.Services
{
    public class TimedService : IHostedService, IDisposable
    {
        private readonly ILogger<TimedService> _logger;
        RaportareDbContext _context;
        PlcService _plcService;
        // Timer
        System.Timers.Timer _timer;
        public TimedService(IServiceProvider services, ILogger<TimedService> logger)
        {
            _logger = logger;
            Services = services; // Get Service Provider
            Scope = Services.CreateScope(); // Create Scope
            _context = Scope.ServiceProvider.GetRequiredService<RaportareDbContext>(); // Get DbContext
            _plcService = Scope.ServiceProvider.GetRequiredService<PlcService>(); // Get PlcService
            // log
            _logger.LogInformation("{data}<=>{Messege}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), "A pornit TimedkService din TimedService Constructor");
        }
        public IServiceProvider Services { get; }
        public IServiceScope Scope { get; }
        public bool IsRunnungBackgroundService;
        public DateTime LastTimeRunBackgroundWork;
        public int ReadingTimeInterval { get; set; } = 1000; // Timp refresh in ms

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // log
            _logger.LogInformation("{data}<=>{Messege}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), "Din Start async din TimedService");

            _timer = new System.Timers.Timer();
            _timer.Interval = ReadingTimeInterval;
            _timer.Elapsed += DoWork;
            _timer.Start();
            IsRunnungBackgroundService = true;
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // log
            _logger.LogInformation("{data}<=>{Messege}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), "Din Stop async din TimedService");
            _timer.Stop();
            IsRunnungBackgroundService = false;
            _timer.Dispose();
            return Task.CompletedTask;
        }

        private void DoWork(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            _timer.Interval = ReadingTimeInterval;
            // logic
            IsRunnungBackgroundService = true;
            LastTimeRunBackgroundWork = DateTime.Now;
            try
            {
                // Foreach Plc Refresh tag values and write them to DbContext
                foreach (PlcModel plc in _plcService.ListPlcs)
                {
                    // Remake enable plc at every minute
                    _plcService.RemakeEnablePlc(plc);
                    // Return if plc is not enable
                    if (plc.IsEnable)
                    {
                        var _cancelTasks = new CancellationTokenSource();
                        var task = Task.Run(() =>
                        {
                            if (_plcService.IsConnectedPlc(plc)) // IsConnected also try to reconnect plc if it is available, and disable plc if it has pingrequestsfail grater than a nr
                            {
                                _plcService.RefreshTagValues(plc); // Refresh value from Plc
                                _plcService.UpdateDbContextTagsValue(_context, plc.TagsList); // Write to DbContext Tag Values
                            }
                        }, _cancelTasks.Token);
                        if (!task.Wait(TimeSpan.FromSeconds(0.25))) _cancelTasks.Cancel(); // Daca nu mai raspunde in timp util se opreste Task
                    }
                }
                // Write Async to DbContext
                var _cancelTasks2 = new CancellationTokenSource();
                var task2 = Task.Run(async () =>
                {
                    await _context.SaveChangesAsync();
                }, _cancelTasks2.Token);
                if (!task2.Wait(TimeSpan.FromSeconds(0.2))) _cancelTasks2.Cancel(); // Daca nu mai raspunde in timp util se opreste Task
            }
            catch (InvalidOperationException exCollection)
            {
                // log
                _logger.LogInformation("{data}<=>{Messege}<=>{error}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), "Log error InvalidOperation din DoWork din TimedService: ", exCollection.Message);
            }
            catch (AggregateException exAgreg)
            {
                // log
                _logger.LogInformation("{data}<=>{Messege}<=>{error}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), "Log error Agregate din DoWork din TimedService: ", exAgreg.Message);
            }
            catch (Exception ex)
            {
                // log
                _logger.LogInformation("{data}<=>{Messege}<=>{error}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), "Log error din DoWork din TimedService: ", ex.Message);
            }

            // log
            _logger.LogInformation("{data}<=>{Messege}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), "Doing din while, din DoWork Async TimedService");
            try
            {
                _timer.Start();
            }
            catch (ObjectDisposedException ex)
            {
                _logger.LogInformation("{data}<=>{Messege}<=>{error}", DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"), "Log error ObjectDisposedException din DoWork din TimedService Timer disposed: ", ex.Message);
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
