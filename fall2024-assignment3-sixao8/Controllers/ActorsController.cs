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
using System.Text;

namespace fall2024_assignment3_sixao8.Controllers
{
    public class ActorsController : Controller
    {
        private readonly fall2024_assignment3_sixao8Context _context;
        private readonly IConfiguration _config;

        public ActorsController(fall2024_assignment3_sixao8Context context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        //get photo
        public async Task<IActionResult> GetActorPfp(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor.FindAsync(id);
            if (actor == null || actor.ProfilePic == null)
            {
                return NotFound();
            }

            var data = actor.ProfilePic;
            return File(data, "image/png");
        }
        // GET: Actors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Actor.ToListAsync());
        }

        // GET: Actors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            //tweet stuff
            var model = actor.Tweets;
            var HtmlBuilder = new StringBuilder();
            foreach (string tweet in model)
            {
                HtmlBuilder.AppendLine($"<h2 id='tweetTitle'>{actor.Name}</h2>");
                HtmlBuilder.AppendLine($"<p id='tweetText'>{tweet}</p>");
            }
            ViewBag.GeneratedHtml = HtmlBuilder.ToString();

            return View(actor);
        }

        public async Task GenerateTweets(Actor actor)
        {

            var ApiKey = _config["Actors:ApiKey"] ?? throw new Exception("Actors:ApiKey does not exist in the current Configuration");
            var ApiEndpoint = _config["Actors:ApiEndpoint"] ?? throw new Exception("Actors:ApiEndpoint does not exist in the current Configuration");
            var ApiDeployment = _config["Actors:ApiDeployment"] ?? throw new Exception("Actors:ApiDeployment does not exist in the current Configuration");
            ApiKeyCredential ApiCredential = new(ApiKey);

            ChatClient client = new AzureOpenAIClient(new Uri(ApiEndpoint), ApiCredential).GetChatClient(ApiDeployment);

            var messages = new ChatMessage[]
            {
                    new SystemChatMessage($"You represent the Twitter social media platform. generate an answer as a string separated by a '|' and no other text."),
                    new UserChatMessage($"Generate 5 tweets from {actor} the minion about life as a minion entirely in minionese, no english.")
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

            actor.Tweets = reviews;
            actor.TweetSentiment = sentimentAverage;

            return;
        }

        // GET: Actors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Actors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Gender,Age,IMDBlink,ProfilePic")] Actor actor, IFormFile? ProfilePic)
        {
            if (ModelState.IsValid)
            {
                await GenerateTweets(actor);

                if (ProfilePic != null && ProfilePic.Length > 0)
                {
                    using var memoryStream = new MemoryStream(); // Dispose() for garbage collection 
                    ProfilePic.CopyToAsync(memoryStream);
                    actor.ProfilePic = memoryStream.ToArray();
                }

                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // POST: Actors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Gender,Age,IMDBlink,ProfilePic")] Actor actor, IFormFile? ProfilePic)
        {
            if (id != actor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await GenerateTweets(actor);

                    if (ProfilePic != null && ProfilePic.Length > 0)
                    {
                        using var memoryStream = new MemoryStream(); // Dispose() for garbage collection 
                        ProfilePic.CopyToAsync(memoryStream);
                        actor.ProfilePic = memoryStream.ToArray();
                    }

                    _context.Update(actor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorExists(actor.Id))
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
            return View(actor);
        }

        // GET: Actors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // POST: Actors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actor.FindAsync(id);
            if (actor != null)
            {
                _context.Actor.Remove(actor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActorExists(int id)
        {
            return _context.Actor.Any(e => e.Id == id);
        }
    }
}
