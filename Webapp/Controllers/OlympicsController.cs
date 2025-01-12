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
        
        public async Task<List<SelectListItem>> GetAllSportNamesAsync()
        {
            var sports = await _context.Sports
                .Select(s => new SelectListItem
                {
                    Value = s.SportName,
                    Text = s.SportName
                }).ToListAsync();

            return sports;
        }
        
        public async Task<List<SelectListItem>> GetAllEventNamesAsync()
        {
            var events = await _context.Events
                .Select(s => new SelectListItem
                {
                    Value = s.EventName,
                    Text = s.EventName
                }).ToListAsync();

            return events;
        }
        
        public async Task<List<SelectListItem>> GetAllGameNamesAsync()
        {
            var events = await _context.Games
                .Select(s => new SelectListItem
                {
                    Value = s.GamesName,
                    Text = s.GamesName
                }).ToListAsync();

            return events;
        }
        


        // GET: Olympics/Create
        public async Task<ViewResult> Create(int id)
        {
            
            var person =  _context.Persons.FindAsync(id);
            var sports =  GetAllSportNamesAsync();
            var events =  GetAllEventNamesAsync();
            var gameNames = GetAllGameNamesAsync();
            var competition = new CreateCompetitorEvent()
            {
                Person = person.Result,
                Sports =  sports.Result,
                Events = events.Result,
                GameNames = gameNames.Result,
                Age = 0,
            };
            return View(competition);
        }

        // POST: Olympics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCompetitorEvent model)
        {

            var maxId = await _context.GamesCompetitors.MaxAsync(s => (int?)s.Id) ?? 0;
            var personId = model.Person.Id;
            var gameId =  _context.Games.FirstOrDefault(g => g.GamesName == model.GameNames.First().Value).Id;
            var age = model.Age;
            var game = _context.Games.FirstOrDefault(g => g.Id == gameId);
            var thisPerson = model.Person;
            
            var newGame = new GamesCompetitor()
            {
                Id = maxId + 1,
                PersonId = personId,
                GamesId = gameId,
                Age = age,
                Games = game,
                Person = thisPerson
            };
            
            
            
            _context.GamesCompetitors.Add(newGame);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
