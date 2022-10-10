using AuthenticationServer.API.Models;
using Microsoft.Extensions.Options;

namespace AuthenticationServer.API.Services.NicknameGenerators
{
    public class NicknameGenerator
    {
        private readonly string[] adjective;
        private readonly string[] noun;
        private readonly int randomLength;
        private readonly Random rand = new Random();
        private readonly StringGenerator stringGenerator;
        private readonly RandomNicknameConfiguration randomNicknameConfiguration;

        public NicknameGenerator(StringGenerator stringGenerator, IOptions<RandomNicknameConfiguration> randomNicknameConfiguration)
        {
            this.stringGenerator = stringGenerator;
            this.randomNicknameConfiguration = randomNicknameConfiguration.Value;
            this.adjective = this.randomNicknameConfiguration.Adjective;
            this.noun = this.randomNicknameConfiguration.Noun;
            this.randomLength = this.randomNicknameConfiguration.RandomLength;
        }

        public string generateNickname()
        {
            return $"{adjective[rand.Next(0, adjective.Length-1)]} {noun[rand.Next(0,noun.Length-1)]}_{stringGenerator.getRandomString(randomLength)}";
        }
    }

    
}
