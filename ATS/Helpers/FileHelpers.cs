namespace ATS.Helpers
{
    public static class FileHelpers
    {
        public static string ConvertToBase64(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                var fileBytes = memoryStream.ToArray();
                return Convert.ToBase64String(fileBytes);
            }
        }

        public static byte[] ConvertFromBase64(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                return null;

            return Convert.FromBase64String(base64String);
        }
    }

}
