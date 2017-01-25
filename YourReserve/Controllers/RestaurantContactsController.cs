/* Description: This controller handles all CRUD Operations for RestaurantContacts table.
 * Methods: GetRestaurantContacts, PostRestaurantContacts
 * Author: Gabriel Coach 
 * Email: gsctca@gmail.com
 */
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
    public class RestaurantContactsController : ApiController
    {
        private DB_9D2D33_YourReserveDBEntities db = new DB_9D2D33_YourReserveDBEntities();

        // GET: api/RestaurantContacts
        public IQueryable<RestaurantContact> GetRestaurantContacts()
        {
            return db.RestaurantContacts;
        }

        // GET: api/RestaurantContacts/5
        [ResponseType(typeof(RestaurantContact))]
        public IHttpActionResult GetRestaurantContact(int id)
        {
            RestaurantContact restaurantContact = db.RestaurantContacts.Find(id);
            if (restaurantContact == null)
            {
                return NotFound();
            }

            return Ok(restaurantContact);
        }

        // PUT: api/RestaurantContacts/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRestaurantContact(int id, RestaurantContact restaurantContact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != restaurantContact.RestaurantContactID)
            {
                return BadRequest();
            }

            db.Entry(restaurantContact).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantContactExists(id))
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

        // POST: api/RestaurantContacts
        [ResponseType(typeof(RestaurantContact))]
        public IHttpActionResult PostRestaurantContact(RestaurantContact restaurantContact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.RestaurantContacts.Add(restaurantContact);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (RestaurantContactExists(restaurantContact.RestaurantContactID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = restaurantContact.RestaurantContactID }, restaurantContact);
        }

        // DELETE: api/RestaurantContacts/5
        [ResponseType(typeof(RestaurantContact))]
        public IHttpActionResult DeleteRestaurantContact(int id)
        {
            RestaurantContact restaurantContact = db.RestaurantContacts.Find(id);
            if (restaurantContact == null)
            {
                return NotFound();
            }

            db.RestaurantContacts.Remove(restaurantContact);
            db.SaveChanges();

            return Ok(restaurantContact);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RestaurantContactExists(int id)
        {
            return db.RestaurantContacts.Count(e => e.RestaurantContactID == id) > 0;
        }
    }
}