﻿using Contact.Backend.Models.Api.StatusModels;

namespace Contact.Backend.Models.Api
{
    public class Response
    {
        public string RequestId { get; set; }
        public Status Status { get; set; }
    }
}