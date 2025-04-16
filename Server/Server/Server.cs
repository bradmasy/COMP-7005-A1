using System.Net.Sockets;
using System.Text;
using static Server.Constants.Constants;

namespace Server.Server;

public class Server(string path)
{
    private static readonly byte[] Buffer = new byte[ByteArraySize];

    public Socket Socket { get; set; } = new(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);

    public async Task StartServer()
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        var endPoint = new UnixDomainSocketEndPoint(path);

        Socket.Bind(endPoint);
        Socket.Listen(Connections);

        await Run();
    }

    private async Task Run()
    {
        Console.WriteLine("Starting server...");
        while (true)
        {
            var clientSocket = await Socket.AcceptAsync();

            var numberOfBytesReceived = clientSocket.Receive(Buffer, 0, Buffer.Length, SocketFlags.None);

            var message = Encoding.UTF8.GetString(Buffer, 0, numberOfBytesReceived);

            SendEncryptedMessage(clientSocket, message);
            Flush();
        }
    }

    private static void Flush()
    {
        Buffer.AsSpan().Clear();
    }

    private static void SendEncryptedMessage(Socket socket, string message)
    {
        Console.WriteLine("Sending message to server: " + message);

        var split = message.Split(Delimiter);
        var cipherText = ShiftCipher(split[Word], int.Parse(split[Shift]));

        socket.Send(Encoding.UTF8.GetBytes(cipherText));
    }

    private static string ShiftCipher(string input, int shift)
    {
        var inputArray = input.ToCharArray();
        var builder = new StringBuilder();

        foreach (var letter in inputArray)
        {
            var shiftedLetter = (char)(Convert.ToInt32(letter) + shift);
            builder.Append(shiftedLetter);
        }

        return builder.ToString();
    }
}