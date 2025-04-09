// See https://aka.ms/new-console-template for more information

class Program
{
    static async Task Main(string[] args)
    {
        var server = new Server.Server.Server("/tmp/foo.sock");

        using var socket = server.Socket;

        await server.StartServer();
    }
}