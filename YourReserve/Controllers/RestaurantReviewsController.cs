using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using YourReserve.Models;

namespace YourReserve.Controllers
{
    public class RestaurantReviewsController : ApiController
    {
        private DB_9D2D33_YourReserveDBEntities db = new DB_9D2D33_YourReserveDBEntities();

        // GET: api/RestaurantReviews
        public IQueryable<RestaurantReview> GetRestaurantReviews()
        {
            return db.RestaurantReviews;
        }

        // GET: api/RestaurantReviews/5
        [ResponseType(typeof(RestaurantReview))]
        public IHttpActionResult GetRestaurantReview(int id)
        {
            RestaurantReview restaurantReview = db.RestaurantReviews.Find(id);
            if (restaurantReview == null)
            {
                return NotFound();
            }

            return Ok(restaurantReview);
        }

        // PUT: api/RestaurantReviews/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRestaurantReview(int id, RestaurantReview restaurantReview)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != restaurantReview.RestaurantReviewID)
            {
                return BadRequest();
            }

            db.Entry(restaurantReview).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantReviewExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/RestaurantReviews
        [ResponseType(typeof(RestaurantReview))]
        public IHttpActionResult PostRestaurantReview(RestaurantReview restaurantReview)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.RestaurantReviews.Add(restaurantReview);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (RestaurantReviewExists(restaurantReview.RestaurantReviewID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = restaurantReview.RestaurantReviewID }, restaurantReview);
        }

        // DELETE: api/RestaurantReviews/5
        [ResponseType(typeof(RestaurantReview))]
        public IHttpActionResult DeleteRestaurantReview(int id)
        {
            RestaurantReview restaurantReview = db.RestaurantReviews.Find(id);
            if (restaurantReview == null)
            {
                return NotFound();
            }

            db.RestaurantReviews.Remove(restaurantReview);
            db.SaveChanges();

            return Ok(restaurantReview);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RestaurantReviewExists(int id)
        {
            return db.RestaurantReviews.Count(e => e.RestaurantReviewID == id) > 0;
        }

        [HttpGet]
        [Route("api/RestaurantReviews/getReviewByRestaurant/{ID}")]
        public IOrderedQueryable<RestaurantReview> getReviewByRestaurant(int ID)
        {
            var query = from r in db.RestaurantReviews
                        where r.RestaurantID == ID
                        orderby r.ReviewDateTime descending
                        select r;

            return (IOrderedQueryable<RestaurantReview>)query;
        }

        [HttpGet]
        [Route("api/RestaurantReviews/getTopRestaurants")]
        public IOrderedQueryable getTopRestaurants()
        {
            var query = (from Rest in db.Restaurants
                         join Loc in db.RestaurantLocations on Rest.RestaurantID equals Loc.RestaurantID
                         join Rev in db.RestaurantReviews on Rest.RestaurantID equals Rev.RestaurantID
                         group Rev.Rating by Rest.RestaurantName into RatingByRestaurant
                         orderby RatingByRestaurant.Average() descending
                         select new { Restaurant = RatingByRestaurant.Key, AverageRating = RatingByRestaurant.Average(), }).Take(6);
            return (IOrderedQueryable)query;
        }
    }
}