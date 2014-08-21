namespace Contact.Domain.ValueTypes
{
    public class Picture
    {
        public string Title { get; private set; }
        public string Extension { get; private set; }
        public byte[] Content { get; private set; }
        public string ContentType { get; private set; }
        public byte[] Md5Hash { get; private set; }

        public Picture(string title, string extension, byte[] content, string contentType, byte[] md5Hash)
        {
            Title = title;
            Extension = extension;
            Content = content;
            ContentType = contentType;
            Md5Hash = md5Hash;
        }
    }
}
