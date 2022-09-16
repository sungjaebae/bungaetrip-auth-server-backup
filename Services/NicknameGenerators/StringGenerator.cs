namespace AuthenticationServer.API.Services.NicknameGenerators
{
    public class StringGenerator
    {
        private string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private char[] Charsarr = new char[256];
        private Random random = new Random();
        public StringGenerator()
        {

        }

        public string getRandomString(int length)
        {
            for (int i = 0; i < length; i++)
            {
                Charsarr[i] = characters[random.Next(characters.Length)];
            }
            ArraySegment<char> segment = new ArraySegment<char>(Charsarr,0, length);
            var resultString = new String(segment);
            return resultString;
        }
            
    }
}
