
namespace Client;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            if (args.Length == 0)
            {
                throw new Exception("Error: Please provide an argument for word and cipher amount.");
            }

            if (args.Length != 2)
            {
                throw new Exception("Error: You must provide exactly 2 arguments. Arg 1 is the word, and 2 is the cipher amount.");
            }
            
            var word = args[0];
            var cipher = args[1];

            const string path = "/tmp/foo.sock";

            var client = new global::Client.Client.Client(path);

            await client.Connect();
            Console.WriteLine("Connected");
            
            await client.SendCipher(word, cipher);
            var response = await client.ReceiveData();
            Console.WriteLine($"Cipher received:{response}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
     

    }
}