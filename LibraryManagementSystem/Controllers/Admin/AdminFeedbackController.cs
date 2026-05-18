using System.Linq;
using System.Web.Mvc;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("Admin/Feedback")]
    public class AdminFeedbackController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Route("")]
        public ActionResult Index(int? ratingFilter)
        {
            var feedbacks = db.Feedbacks
                .Include("User")
                .Include("Book")
                .AsQueryable();

            if (ratingFilter.HasValue && ratingFilter.Value >= 1 && ratingFilter.Value <= 5)
            {
                feedbacks = feedbacks.Where(f => f.Rating == ratingFilter.Value);
            }

            ViewBag.RatingFilter = ratingFilter;
            ViewBag.TotalCount = feedbacks.Count();
            ViewBag.AverageRating = feedbacks.Any()
                ? System.Math.Round(feedbacks.Average(f => (double)f.Rating), 1)
                : (double?)null;

            var result = feedbacks.OrderByDescending(f => f.CreatedAt).ToList();
            return View("~/Views/Admin/Feedback/Index.cshtml", result);
        }

        [Route("Delete/{id}")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var feedback = db.Feedbacks.Find(id);
            if (feedback == null) return HttpNotFound();

            db.Feedbacks.Remove(feedback);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Feedback deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
