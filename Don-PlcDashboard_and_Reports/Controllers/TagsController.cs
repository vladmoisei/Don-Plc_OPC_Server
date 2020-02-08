using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Don_PlcDashboard_and_Reports.Data;
using Don_PlcDashboard_and_Reports.Models;

namespace Don_PlcDashboard_and_Reports.Controllers
{
    public class TagsController : Controller
    {
        private readonly RaportareDbContext _context;

        public TagsController(RaportareDbContext context)
        {
            _context = context;
        }

        // GET: Tags
        public async Task<IActionResult> Index()
        {
            var raportareDbContext = _context.Tags.Include(t => t.PlcModel);
            return View(await raportareDbContext.ToListAsync());
        }

        // GET: Tags from Plc Controller
        public async Task<IActionResult> IndexFromPlc(string plcName)
        {
            var raportareDbContext = _context.Tags.Include(t => t.PlcModel).Where(item => item.PlcModel.Name == plcName);

            //return View(await raportareDbContext.ToListAsync());
            return View("Index", await raportareDbContext.ToListAsync() );
        }

        // GET: Tags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tagModel = await _context.Tags
                .Include(t => t.PlcModel)
                .FirstOrDefaultAsync(m => m.TagID == id);
            if (tagModel == null)
            {
                return NotFound();
            }

            return View(tagModel);
        }

        // GET: Tags/Create
        public IActionResult Create()
        {
            ViewData["PlcModelID"] = new SelectList(_context.Plcs, "PlcModelID", "Name");
            return View();
        }

        // POST: Tags/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TagID,Name,Value,Adress,DataType,PlcModelID")] TagModel tagModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tagModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PlcModelID"] = new SelectList(_context.Plcs, "PlcModelID", "Name", tagModel.PlcModelID);
            return View(tagModel);
        }

        // GET: Tags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tagModel = await _context.Tags.FindAsync(id);
            if (tagModel == null)
            {
                return NotFound();
            }
            ViewData["PlcModelID"] = new SelectList(_context.Plcs, "PlcModelID", "Name", tagModel.PlcModelID);
            return View(tagModel);
        }

        // POST: Tags/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TagID,Name,Value,Adress,DataType,PlcModelID")] TagModel tagModel)
        {
            if (id != tagModel.TagID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tagModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TagModelExists(tagModel.TagID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PlcModelID"] = new SelectList(_context.Plcs, "PlcModelID", "Name", tagModel.PlcModelID);
            return View(tagModel);
        }

        // GET: Tags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tagModel = await _context.Tags
                .Include(t => t.PlcModel)
                .FirstOrDefaultAsync(m => m.TagID == id);
            if (tagModel == null)
            {
                return NotFound();
            }

            return View(tagModel);
        }

        // POST: Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tagModel = await _context.Tags.FindAsync(id);
            _context.Tags.Remove(tagModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TagModelExists(int id)
        {
            return _context.Tags.Any(e => e.TagID == id);
        }
    }
}
