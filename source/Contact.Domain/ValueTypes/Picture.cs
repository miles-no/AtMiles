﻿namespace Contact.Domain.ValueTypes
{
    public class Picture
    {
        public string Title { get; private set; }
        public string Extension { get; private set; }
        public byte[] Content { get; private set; }

        public Picture(string title, string extension, byte[] content)
        {
            Title = title;
            Extension = extension;
            Content = content;
        }
    }
}