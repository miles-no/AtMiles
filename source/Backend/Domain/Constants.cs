﻿namespace no.miles.at.Backend.Domain
{
    public static class Constants
    {
        public const int IgnoreVersion = -2;
        public const int NewVersion = -1;
        public const string EventStoreEventClrTypeHeader = "EventClrTypeName";
        public const string EventStoreAggregateClrTypeHeader = "AggregateClrTypeName";
        public const string GoogleIdProvider = "google-apps";
        public const string SystemUserId = "SYSTEM";
        public const string JwtSubject = "sub";
        public const string ConfigFilenameSetting = "ConfigFile";
    }
}
