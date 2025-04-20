using static Server.Constants;

namespace Server;

/**
 * Main program for the server.
 */
class Program
{
    /**
     * The Entry point for the program
     * path should be "/tmp/foo.sock"
     */
    static async Task Main(string[] args)
    {
        try
        {
            if (args.Length == NoArgs)
            {
                throw new Exception("Please provide a path to the UNIX domain socket.");
            }

            var path = args[SocketPath];
            var server = new Server(path);
            
            await server.StartServer();
            
            server.TearDown();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}