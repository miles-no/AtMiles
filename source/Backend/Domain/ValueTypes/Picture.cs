namespace no.miles.at.Backend.Domain.ValueTypes
{
    public class Picture
    {
        public readonly string Title;
        public readonly string Extension;
        public readonly byte[] Content;
        public readonly string ContentType;
        public readonly byte[] Md5Hash;

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
