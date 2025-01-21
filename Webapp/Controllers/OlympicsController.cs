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
                    Person = ce.Competitor.Person,
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
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Persons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        public IActionResult Create(int id)
        {
            var competitorEvent = new CompetitorEventCreateModel();
            competitorEvent.Person = _context.Persons.Find(id);
            competitorEvent.Sports = _context.Sports
                .Select(s => new SelectListItem
                {
                    Value = s.SportName,
                    Text = s.SportName
                }).ToList();
            competitorEvent.Events = _context.Events
                .Select(e => new SelectListItem
                {
                    Value = e.EventName,
                    Text = e.EventName
                }).ToList();
            competitorEvent.Olympics = _context.Games
                .Select(o => new SelectListItem
                {
                    Value = o.GamesName,
                    Text = o.GamesName
                }).ToList();
            
            return View(competitorEvent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompetitorEventCreateModel competitorEvent, int page = 1, int size = 20)
        {
            if (ModelState.IsValid)
            {
                var newCompetitorEvent = new CompetitorEvent
                {
                    
                    CompetitorId = competitorEvent.Person.Id,
                    EventId = _context.Events.First(e => e.EventName == competitorEvent.EventName).Id,
                };

                _context.Add(newCompetitorEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { page, size });
            }

            // Repopulate dropdown lists if model state is invalid
            competitorEvent.Sports = _context.Sports
                .Select(s => new SelectListItem
                {
                    
                    Value = s.SportName,
                    Text = s.SportName
                }).ToList();
            competitorEvent.Events = _context.Events
                .Select(e => new SelectListItem
                {
                    Value = e.EventName,
                    Text = e.EventName
                }).ToList();
            competitorEvent.Olympics = _context.Games
                .Select(o => new SelectListItem
                {
                    Value = o.GamesName,
                    Text = o.GamesName
                }).ToList();

            return RedirectToAction(nameof(Index), new { page, size });
        }

        // GET: Olympics/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Persons.FindAsync(id);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Gender,Height,Weight")] Person person)
        {
            if (id != person.Id)
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Persons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: Olympics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var person = await _context.Persons.FindAsync(id);
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
