﻿using System;

namespace Contact.Domain.Test
{
    public class FakeStreamEvent
    {
        public readonly string StreamId;
        public readonly Event Event;

        public FakeStreamEvent(string streamId, Event @event)
        {
            StreamId = streamId;
            Event = @event;
        }
    }
}
