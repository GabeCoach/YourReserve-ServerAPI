/* Description: This controller handles all CRUD Operations for Restaurants table.
 * Methods: SearchRestaurant, registerRestaurant
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
using YourReserve.SMTP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace YourReserve.Controllers
{
    public class RestaurantsController : ApiController
    {
        private DB_9D2D33_YourReserveDBEntities db = new DB_9D2D33_YourReserveDBEntities();

        // GET: api/Restaurants
        public IQueryable<Restaurant> GetRestaurants()
        {
            return db.Restaurants;
        }

        // GET: api/Restaurants/5
        [ResponseType(typeof(Restaurant))]
        public IHttpActionResult GetRestaurant(int id)
        {
            Restaurant restaurant = db.Restaurants.Find(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            return Ok(restaurant);
        }

        // PUT: api/Restaurants/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRestaurant(int id, Restaurant restaurant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != restaurant.RestaurantID)
            {
                return BadRequest();
            }

            db.Entry(restaurant).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantExists(id))
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

        // POST: api/Restaurants
        [ResponseType(typeof(Restaurant))]
        public IHttpActionResult PostRestaurant(Restaurant restaurant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Restaurants.Add(restaurant);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (RestaurantExists(restaurant.RestaurantID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = restaurant.RestaurantID }, restaurant);
        }

        // DELETE: api/Restaurants/5
        [ResponseType(typeof(Restaurant))]
        public IHttpActionResult DeleteRestaurant(int id)
        {
            Restaurant restaurant = db.Restaurants.Find(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            db.Restaurants.Remove(restaurant);
            db.SaveChanges();

            return Ok(restaurant);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RestaurantExists(int id)
        {
            return db.Restaurants.Count(e => e.RestaurantID == id) > 0;
        }

        /* Description: This method Searches for restaurants based on the string paramater
         * Params: string SearchCriteria
         */
        [HttpGet]
        [Route("api/Restaurants/SearchRestaurant/{SearchCriteria}")]
        public IQueryable<Restaurant> SearchRestaurant(string SearchCriteria)
        {
            var query = from s in db.Restaurants
                        where s.RestaurantName.Contains(SearchCriteria)
                        select s;

            return query;
        }

        /* Description: registers restaurant in the database.
         * Params: dynamic Item
         * Returns: HttpStatusCode
         */
        [HttpPost]
        [Route("api/Restaurants/registerRestaurant")]
        public IHttpActionResult registerRestaurant(dynamic item)
        {
            try
            {
                List<string> UserNames = new List<string>();
                var SMTP = new YourReserveSMTP();

                //Add Restaurant Name info into database
                var oRestaurant = new Restaurant();
                oRestaurant.RestaurantName = item[3].RestaurantName;
                string inputUserName = item[2].UserName;

                db.Restaurants.Add(oRestaurant);
                db.SaveChanges();

                int iRestaurantID = oRestaurant.RestaurantID;

                var oRestaurantOwner = new RestaurantOwner();

                var queryUserNames = from u in db.RestaurantOwners
                                     where u.UserName.Contains(inputUserName)
                                     select u;

                //Check if entered username exists
                if(queryUserNames.Any())
                {
                    throw new Exception("This UserName already exists");
                }

                //Add Restaurant Owner information to the database
                oRestaurantOwner.RestaurantID = iRestaurantID;
                oRestaurantOwner.FirstName = item[2].FirstName;
                oRestaurantOwner.LastName = item[2].LastName;
                oRestaurantOwner.Email = item[2].Email;
                oRestaurantOwner.PhoneNumber = item[2].Email;
                oRestaurantOwner.Address = item[2].Address;
                oRestaurantOwner.City = item[2].City;
                oRestaurantOwner.State_Province = item[2].State_Province;
                oRestaurantOwner.Country = item[2].Country;
                oRestaurantOwner.ZipCode = item[2].ZipCode;
                oRestaurantOwner.ApartmentNumber = item[2].ApartmentNumber;
                oRestaurantOwner.UserName = item[2].UserName;
                oRestaurantOwner.Password = item[2].Password;

                db.RestaurantOwners.Add(oRestaurantOwner);
                db.SaveChanges();

                int iRestaurantOwnerID = oRestaurantOwner.RestaurantOwnerID;

                //Add Restaurant Location to database
                var oRestaurantLocation = new RestaurantLocation();
                oRestaurantLocation.RestaurantID = iRestaurantID;
                oRestaurantLocation.Address = item[0].Address;
                oRestaurantLocation.City = item[0].City;
                oRestaurantLocation.State_Province = item[0].State_Province;
                oRestaurantLocation.Country = item[0].Country;
                oRestaurantLocation.ZipCode = item[0].ZipCode;
                oRestaurantLocation.SuiteNumber = item[0].SuiteNumber;

                int iRestaurantLocationID = oRestaurantLocation.RestaurantLocationID;

                db.RestaurantLocations.Add(oRestaurantLocation);
                db.SaveChanges();

                //Add RestaurantContact to database
                var oRestaurantContact = new RestaurantContact();
                oRestaurantContact.RestaurantID = iRestaurantID;
                oRestaurantContact.RestaurantOwnerID = iRestaurantOwnerID;
                oRestaurantContact.RestaurantLocationID = iRestaurantLocationID;
                oRestaurantContact.PhoneNumber = item[1].PhoneNumber;
                oRestaurantContact.Email = item[1].Email;

                db.RestaurantContacts.Add(oRestaurantContact);
                db.SaveChanges();

                //Send a confirmation email to the registered restaurant.
                SMTP.sendRestConfirmationEmail(oRestaurantOwner);

                return StatusCode(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }



        }

        /* Description: This method gets Lat and Long from the Google Maps API.
         * Params: JSON string that contains an address
         * Returns: Returns a JSON string with the Lat and Long 
         */
        [HttpGet]
        [Route("api/Restaurants/LatLng/{JSON}")]
        public JObject getLatLng(string JSON)
        {

            JObject jReturn = null;

            //Split JSON array into different variables
            string[] arrayJSON = JSON.Split('~');
            string partialaddress1 = arrayJSON[0];
            string partialaddress2 = arrayJSON[1];
            string partialaddress3 = arrayJSON[2];
            string City = arrayJSON[3].Replace(" ", "+");
            string State = arrayJSON[4];

            //Grab the Lat and Long from the GoogleAPI
            string sReturn = GrabPage("https://maps.googleapis.com/maps/api/geocode/xml?address=" + partialaddress1 + "+" + partialaddress2 + "+" + partialaddress3 + ",+" + City + ",+" + State + "&key=AIzaSyCwgOFfOV8dvATrRpQz3XlAwg8VH7JNNWo", "GET", "", "");

            System.Xml.XmlDocument oXMLDoc = new System.Xml.XmlDocument();
            oXMLDoc.LoadXml(sReturn);

            //Get Lat and Long nodes from the xml 
            System.Xml.XmlNode oNodeLat = oXMLDoc.SelectSingleNode("//geometry/location/lat");
            System.Xml.XmlNode oNodeLng = oXMLDoc.SelectSingleNode("//geometry/location/lng");

            string sLat = oNodeLat.ChildNodes[0].Value.ToString();
            string sLng = oNodeLng.ChildNodes[0].Value.ToString();

            LatLng oLatLng = new LatLng();
            oLatLng.Lat = sLat;
            oLatLng.Lng = sLng;

            string sTest = JsonConvert.SerializeObject(oLatLng);

            jReturn = JObject.Parse(sTest);

            return jReturn;
        }

        /* Description: This method gets RestaurantID by Restaurant name.
         * Params: string Name
         * Returns: a query that gives a restaurant ID
         */
        [HttpGet]
        [Route("api/getRestaurantIDByName")]
        public IOrderedQueryable<Restaurant> getRestaurantIDByName(string Name)
        {
            var query = from r in db.Restaurants
                        where r.RestaurantName == Name
                        select r.RestaurantID;

            return (IOrderedQueryable<Restaurant>)query;
        }

        /* Description: This controller handles all CRUD Operations for RestaurantContacts table.
         * Params: string url, string sMethod, string, strRequest, string sToken
         * Returns: returns a string that contains Lat Long.
         */
        private static string GrabPage(string url, string sMethod, string strRequest, string sToken)
        {
            try
            {
                //Create HttpRequest
                System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                req.Method = sMethod;
                if (sMethod != "GET")
                {
                    req.ContentType = "application/json";
                }

                //Add Bearer Token
                if (!string.IsNullOrEmpty(sToken))
                {
                    req.Headers.Add("Authorization", "Bearer " + sToken);
                }
                if (sMethod != "GET")
                {
                    System.IO.StreamWriter streamOut = new System.IO.StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII);
                    streamOut.Write(strRequest);
                    streamOut.Close();
                }

                //Return Response
                System.IO.StreamReader streamIn = new System.IO.StreamReader(req.GetResponse().GetResponseStream());
                string strResponse = streamIn.ReadToEnd();
                streamIn.Close();
                return strResponse;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }

    public class LatLng
    {
        public string Lat { get; set; }
        public string Lng { get; set; }
    }
}