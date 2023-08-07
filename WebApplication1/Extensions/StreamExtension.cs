namespace WebApplication1.Extensions
{
    public static class StreamExtension
    {
        public static byte[] ToByteArray(this Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                input.CopyTo(ms, 16 * 1024);
                return ms.ToArray();
            }
        }
    }
}
