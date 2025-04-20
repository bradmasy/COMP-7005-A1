using static Client.Constants;

namespace Client;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            Validator.ValidateArguments(args);

            var shift = Validator.ValidateShiftArgument(args[Shift]);
            var path = args[SocketPath];
            var word = args[Word];

            var client = new Client(path);

            await client.Connect();
            await client.SendCipher(word, shift);

            var response = await client.ReceiveData();
            var decrypted = client.Decrypt(response, shift);

            client.DisplayMessage(decrypted);

            client.Teardown();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}