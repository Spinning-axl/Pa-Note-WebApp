using Microsoft.AspNetCore.Mvc;
using Pa_Note_WebApp.Data;
using Pa_Note_WebApp.Models;
using System.Linq;

namespace Pa.Controllers
{
    public class NotesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("User    Id");
            var notes = _context.Notes.Where(n => n.UserId == userId).ToList();
            return View(notes);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Note note)
        {
            var userId = HttpContext.Session.GetInt32("User    Id");
            if (!userId.HasValue)
            {
                return BadRequest("User    Id is not set in the session. Please log in.");
            }

            note.UserId = userId.Value;
            note.CreatedAt = DateTime.Now;

            _context.Notes.Add(note);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Console.WriteLine($"Editing note with ID: {id}"); // Log the ID being edited
            var note = _context.Notes.FirstOrDefault(n => n.NoteId == id); // Use FirstOrDefault to find the note
            if (note == null)
            {
                Console.WriteLine("Note not found."); // Log if the note is not found
                return NotFound(); // Return a 404 if the note is not found
            }
            return View(note);
        }

        [HttpPost]
        public IActionResult Edit(Note note)
        {
            if (!ModelState.IsValid)
            {
                return View(note);
            }

            var existingNote = _context.Notes.Find(note.NoteId);
            if (existingNote == null)
            {
                ModelState.AddModelError("", "Note not found.");
                return View(note);
            }

            existingNote.Title = note.Title;
            existingNote.Content = note.Content;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var note = _context.Notes.Find(id);
            if (note != null)
            {
                _context.Notes.Remove(note);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
