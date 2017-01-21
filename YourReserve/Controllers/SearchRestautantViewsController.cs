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
using LinqKit;

namespace YourReserve.Controllers
{
    public class SearchRestautantViewsController : ApiController
    {
        private DB_9D2D33_YourReserveDBEntities db = new DB_9D2D33_YourReserveDBEntities();

        // GET: api/SearchRestautantViews
        public IQueryable<SearchRestautantView> GetSearchRestautantViews()
        {
            return db.SearchRestautantViews;
        }

        // GET: api/SearchRestautantViews/5
        [ResponseType(typeof(SearchRestautantView))]
        public IHttpActionResult GetSearchRestautantView(int id)
        {
            SearchRestautantView searchRestautantView = db.SearchRestautantViews.Find(id);
            if (searchRestautantView == null)
            {
                return NotFound();
            }

            return Ok(searchRestautantView);
        }

        // PUT: api/SearchRestautantViews/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutSearchRestautantView(int id, SearchRestautantView searchRestautantView)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != searchRestautantView.RestaurantID)
            {
                return BadRequest();
            }

            db.Entry(searchRestautantView).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SearchRestautantViewExists(id))
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

        // POST: api/SearchRestautantViews
        [ResponseType(typeof(SearchRestautantView))]
        public IHttpActionResult PostSearchRestautantView(SearchRestautantView searchRestautantView)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SearchRestautantViews.Add(searchRestautantView);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (SearchRestautantViewExists(searchRestautantView.RestaurantID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = searchRestautantView.RestaurantID }, searchRestautantView);
        }

        // DELETE: api/SearchRestautantViews/5
        [ResponseType(typeof(SearchRestautantView))]
        public IHttpActionResult DeleteSearchRestautantView(int id)
        {
            SearchRestautantView searchRestautantView = db.SearchRestautantViews.Find(id);
            if (searchRestautantView == null)
            {
                return NotFound();
            }

            db.SearchRestautantViews.Remove(searchRestautantView);
            db.SaveChanges();

            return Ok(searchRestautantView);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SearchRestautantViewExists(int id)
        {
            return db.SearchRestautantViews.Count(e => e.RestaurantID == id) > 0;
        }

        [HttpGet]
        [Route("api/SearchRestaurantView/searchRestaurant/{RestaurantName}")]
        public IOrderedQueryable searchRestaurant(string RestaurantName)
        {
            try
            {
               var query = from r in db.SearchRestautantViews
                        where r.RestaurantName.Contains(RestaurantName)
                        select r;

                return (IOrderedQueryable)query;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            
        }
    }
}