using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Don_PlcDashboard_and_Reports.Data;
using Don_PlcDashboard_and_Reports.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Don_PlcDashboard_and_Reports.Controllers
{
    public class PlcServiceController : Controller
    {
        private readonly ILogger<PlcsController> _logger;
        private readonly RaportareDbContext _context;
        private readonly PlcService _plcService;
        // Constructor
        public PlcServiceController(RaportareDbContext context, ILogger<PlcsController> logger, PlcService plcService)
        {
            _logger = logger;
            _context = context;
            _plcService = plcService;
        }
        // View List of created PLC
        public IActionResult Index()
        {
            return View(_plcService.ListPlcs);
        }
    }
}