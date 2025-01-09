using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Webapp.Models.Olympics;

namespace Webapp.Controllers
{
    [Authorize(Roles = "amdin,user")]
    public class OlympicsController : Controller
    {
        private readonly OlympicsContext _context;

        public OlympicsController(OlympicsContext context)
        {
            _context = context;
        }

        // GET: Olympics
        public async Task<IActionResult> Index(int page = 1, int size = 20)
        {
            var personQuery = _context.Persons
                .Select(b => new PersonView()
                {
                    Id = b.Id,
                    FullName = b.FullName,
                    Gender = b.Gender,
                    Height = b.Height,
                    Weight = b.Weight,
                    GoldMedals = _context.CompetitorEvents.Count(ce => ce.MedalId == 1 && ce.CompetitorId == b.Id),
                    SilverMedals = _context.CompetitorEvents.Count(ce => ce.MedalId == 2 && ce.CompetitorId == b.Id),
                    BronzeMedals = _context.CompetitorEvents.Count(ce => ce.MedalId == 3 & ce.CompetitorId == b.Id),
                    NumberOfCompetitions = _context.CompetitorEvents.Count(ce => ce.CompetitorId == b.Id)
                })
                .Skip((page - 1) * size) 
                .Take(size)
                .ToList();
            
            
            var totalCount  =  await _context.Persons.CountAsync();
            var  paginatedList  = new  PaginatedList<PersonView>(personQuery,  totalCount, page,   size);  
            return View(paginatedList); 
        }

        public async Task<IActionResult> CompetitionList(int id)
        {
            var competitionList = await _context.CompetitorEvents
                .Where(ce => ce.CompetitorId == id)
                .Select(ce => new CompetitorEventsView()
                {
                    Name = ce.Competitor.Person.FullName,
                    SportName =  ce.Event.Sport.SportName,
                    EventName = ce.Event.EventName,
                    CitiName = _context.GamesCities.FirstOrDefault(gc => gc.GamesId == ce.Competitor.GamesId).City.CityName,
                    Season = ce.Competitor.Games.Season,
                    Age = ce.Competitor.Age,
                    Medal = ce.Medal.MedalName
                }).ToListAsync();
            
            return View(competitionList.AsQueryable());
        }
        
        // GET: Olympics/Details/5
        public async Task<IActionResult> Details(int? PersonId)
        {
            if (PersonId == null)
            {
                return NotFound();
            }

            var person = await _context.Persons
                .FirstOrDefaultAsync(m => m.Id == PersonId);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // GET: Olympics/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Olympics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Gender,Height,Weight")] Person person)
        {
            if (ModelState.IsValid)
            {
                _context.Add(person);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }

        // GET: Olympics/Edit/5
        public async Task<IActionResult> Edit(int? PersonId)
        {
            if (PersonId == null)
            {
                return NotFound();
            }

            var person = await _context.Persons.FindAsync(PersonId);
            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }

        // POST: Olympics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int PersonId, [Bind("Id,FullName,Gender,Height,Weight")] Person person)
        {
            if (PersonId != person.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.Id))
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
            return View(person);
        }

        // GET: Olympics/Delete/5
        public async Task<IActionResult> Delete(int? PersonId)
        {
            if (PersonId == null)
            {
                return NotFound();
            }

            var person = await _context.Persons
                .FirstOrDefaultAsync(m => m.Id == PersonId);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: Olympics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int PersonId)
        {
            var person = await _context.Persons.FindAsync(PersonId);
            if (person != null)
            {
                _context.Persons.Remove(person);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonExists(int id)
        {
            return _context.Persons.Any(e => e.Id == id);
        }
    }
}
