namespace TspResponder.Core.Http
{
    public class TspHttpResponse
    {
        public string MediaType { get; }

        public byte[] Content { get; }

        public TspHttpResponse(byte[] content, string mediaType)
        {
            Content = content;
            MediaType = mediaType;
        }
    }
}