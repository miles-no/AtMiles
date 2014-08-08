﻿using System.Web.Http;
using Contact.Backend.Models.Api;

namespace Contact.Backend.Controllers
{
    
    public class StatusController : ApiController
    {
        /// <summary>
        /// Gets the status of a request
        /// </summary>
        /// <param name="id"></param>
        public Status Get(string id)
        {
            return new Status
            {
                Id = "stillPending",
                Url = Request.RequestUri.AbsoluteUri
            };
        }
         
    }
   
}