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

        [HttpGet]
        [Route("api/Restaurants/SearchRestaurant/{SearchCriteria}")]
        public IQueryable<Restaurant> SearchRestaurant(string SearchCriteria)
        {
            var query = from s in db.Restaurants
                        where s.RestaurantName.Contains(SearchCriteria)
                        select s;


            return query;
        }

        [HttpPost]
        [Route("api/Restaurants/registerRestaurant")]
        public IHttpActionResult registerRestaurant(dynamic item)
        {
            try
            {
                List<string> UserNames = new List<string>();
                var SMTP = new YourReserveSMTP();

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

                if(queryUserNames.Any())
                {
                    throw new Exception("This UserName already exists");
                }

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

                var oRestaurantContact = new RestaurantContact();
                oRestaurantContact.RestaurantID = iRestaurantID;
                oRestaurantContact.RestaurantOwnerID = iRestaurantOwnerID;
                oRestaurantContact.RestaurantLocationID = iRestaurantLocationID;
                oRestaurantContact.PhoneNumber = item[1].PhoneNumber;
                oRestaurantContact.Email = item[1].Email;

                db.RestaurantContacts.Add(oRestaurantContact);
                db.SaveChanges();

                SMTP.sendRestConfirmationEmail(oRestaurantOwner);

                return StatusCode(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }



        }

        [HttpGet]
        [Route("api/Restaurants/LatLng/{JSON}")]
        public JObject getLatLng(string JSON)
        {
            //string sReturn = "";
            JObject jReturn = null;

            string[] arrayJSON = JSON.Split('~');
            string partialaddress1 = arrayJSON[0];
            string partialaddress2 = arrayJSON[1];
            string partialaddress3 = arrayJSON[2];
            string City = arrayJSON[3].Replace(" ", "+");
            string State = arrayJSON[4];

            string sReturn = GrabPage("https://maps.googleapis.com/maps/api/geocode/xml?address=" + partialaddress1 + "+" + partialaddress2 + "+" + partialaddress3 + ",+" + City + ",+" + State + "&key=AIzaSyCwgOFfOV8dvATrRpQz3XlAwg8VH7JNNWo", "GET", "", "");

            System.Xml.XmlDocument oXMLDoc = new System.Xml.XmlDocument();
            oXMLDoc.LoadXml(sReturn);

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

        [HttpGet]
        [Route("api/getRestaurantIDByName")]
        public IOrderedQueryable<Restaurant> getRestaurantIDByName(string Name)
        {
            var query = from r in db.Restaurants
                        where r.RestaurantName == Name
                        select r.RestaurantID;

            return (IOrderedQueryable<Restaurant>)query;
        }

        private static string GrabPage(string url, string sMethod, string strRequest, string sToken)
        {
            try
            {
                System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                req.Method = sMethod;
                if (sMethod != "GET")
                {
                    req.ContentType = "application/json";
                }

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