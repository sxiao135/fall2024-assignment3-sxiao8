using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fall2024_assignment3_sixao8.Data;
using fall2024_assignment3_sixao8.Models;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using VaderSharp2;
using System.Numerics;
using System.Text;

namespace fall2024_assignment3_sixao8.Controllers
{
    public class MoviesController : Controller
    {
        private readonly fall2024_assignment3_sixao8Context _context;
        private readonly IConfiguration _config;

        public MoviesController(fall2024_assignment3_sixao8Context context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        //get poster
        public async Task<IActionResult> GetMoviePoster(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null || movie.Media == null)
            {
                return NotFound();
            }

            var data = movie.Media;
            return File(data, "image/jpg");
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movie.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            //tweet stuff
            var model = movie.Reviews;
            var HtmlBuilder = new StringBuilder();
            foreach (string review in model)
            {
                HtmlBuilder.AppendLine($"<p>{review}</p>");
                HtmlBuilder.AppendLine($"<hr>");
            }
            ViewBag.GeneratedHtml = HtmlBuilder.ToString();

            return View(movie);
        }

        private async Task GenerateReviews(Movie movie)
        {
            var ApiKey = _config["Actors:ApiKey"] ?? throw new Exception("Actors:ApiKey does not exist in the current Configuration");
            var ApiEndpoint = _config["Actors:ApiEndpoint"] ?? throw new Exception("Actors:ApiEndpoint does not exist in the current Configuration");
            var ApiDeployment = _config["Actors:ApiDeployment"] ?? throw new Exception("Actors:ApiDeployment does not exist in the current Configuration");
            ApiKeyCredential ApiCredential = new(ApiKey);

            ChatClient client = new AzureOpenAIClient(new Uri(ApiEndpoint), ApiCredential).GetChatClient(ApiDeployment);

            string[] personas = { "is harsh", "loves romance", "loves comedy", "loves thrillers", "loves fantasy", "unattentive", "hates kevin" };
            var messages = new ChatMessage[]
            {
            new SystemChatMessage($"You represent a group of 6-12 minions who have the following personalities: {string.Join(",", personas)}. When you receive a question, respond as each member of the group with each response separated by a '|'. respond entirely in minionese, no english."),
            new UserChatMessage($"How would you rate your experience as a minion in the movie {movie.Title} out of 10 in 10- 150 words?")
            };
            ClientResult<ChatCompletion> result = await client.CompleteChatAsync(messages);
            string[] reviews = result.Value.Content[0].Text.Split('|').Select(s => s.Trim()).ToArray();

            var analyzer = new SentimentIntensityAnalyzer();
            double sentimentTotal = 0;



            for (int i = 0; i < reviews.Length; i++)
            {
                string review = reviews[i];
                SentimentAnalysisResults sentiment = analyzer.PolarityScores(review);
                sentimentTotal += sentiment.Compound;
            }

            double sentimentAverage = sentimentTotal / reviews.Length;
            movie.Reviews = reviews;
            movie.reviewSentiment = sentimentAverage;
            return;
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,IMDB,Genre,releaseYear,Media")] Movie movie, IFormFile? Media)
        {
            if (ModelState.IsValid)
            {
                await GenerateReviews(movie);

                if (Media != null && Media.Length > 0)
                {
                    using var memoryStream = new MemoryStream(); // Dispose() for garbage collection 
                    Media.CopyToAsync(memoryStream);
                    movie.Media = memoryStream.ToArray();
                }

                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,IMDB,Genre,releaseYear,Media")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await GenerateReviews(movie);
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
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
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }
    }
}
