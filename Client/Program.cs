using Client.Validation;
using Client.Client;
using static Client.Constants.Constants;

namespace Client;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            Validator.ValidateArguments(args);

            var path = args[SocketPath];
            var word = args[Word];
            var success = int.TryParse(args[Shift], out var shift);

            if (!success) throw new Exception("Error: Please provide a valid integer value to shift word.");
            
            var client = new Client.Client(path);

            await client.Connect();

            await client.SendCipher(word, shift);
            var response = await client.ReceiveData();
            Console.WriteLine($"Cipher received:{response}");
            var decrypted = client.Decrypt(response, shift);
            Console.WriteLine($"Decrypted:{decrypted}");
            client.Teardown();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}