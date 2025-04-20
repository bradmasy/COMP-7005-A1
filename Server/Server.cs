using System.Net.Sockets;
using System.Text;
using static Server.Constants;

namespace Server;

public class Server(string path)
{
    private static readonly byte[] Buffer = new byte[ByteArraySize];
    private Socket Socket { get; init; } = new(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);

    public async Task StartServer()
    {
        try
        {
            EnsurePath(path);

            var endPoint = new UnixDomainSocketEndPoint(path);

            Socket.Bind(endPoint);
            Socket.Listen(Connections);

            await Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void TearDown()
    {
        Socket.Close();
        File.Delete(path);
    }

    private async Task Run()
    {
        Console.WriteLine("Starting server...");
        while (true)
        {
            var clientConnection = await AcceptClientConnection();

            try
            {
                var message = Read(clientConnection);
                Console.WriteLine(message);
                SendEncryptedMessage(clientConnection, message);
                Flush();
            }
            catch (Exception ex)
            {
                var message = CreateErrorMessage(ex.Message);

                SendErrorMessage(clientConnection, message);
            }
            finally
            {
                EndClientSession(clientConnection);
            }
        }
    }

    private async Task<Socket> AcceptClientConnection()
    {
        try
        {
            return await Socket.AcceptAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    private static void Flush()
    {
        Buffer.AsSpan().Clear();
    }

    private static void EndClientSession(Socket clientSocket)
    {
        clientSocket.Close();
    }

    private static string Read(Socket clientSocket)
    {
        var numberOfBytesReceived = clientSocket.Receive(Buffer, 0, Buffer.Length, SocketFlags.None);

        return Parse(numberOfBytesReceived);
    }

    private static string Parse(int bytes)
    {
        return Encoding.UTF8.GetString(Buffer, 0, bytes);
    }

    private static void SendEncryptedMessage(Socket socket, string message)
    {
        var split = message.Split(Delimiter);
        var cipherText = ShiftCipher(split[Word], int.Parse(split[Shift]));

        socket.Send(Encoding.UTF8.GetBytes(cipherText));
    }

    private static string ShiftCipher(string input, int shift)
    {
        var builder = new StringBuilder();

        foreach (var letter in input)
        {
            if (char.IsLetter(letter))
            {
                var offset = char.IsUpper(letter) ? UpperAscii : LowerAscii;
                var encrypted = (char)(((letter - offset + shift) % AsciiShift + AsciiShift) % AsciiShift + offset);
                builder.Append(encrypted);
            }
            else if (char.IsDigit(letter))
            {
                var digit = letter - Zero;
                var encryptedDigit = (digit + shift % NumericShift + NumericShift) % NumericShift;
                var encrypted = (char)(encryptedDigit + Zero);
                builder.Append(encrypted);
            }
            else
            {
                builder.Append(letter);
            }
        }

        return builder.ToString();
    }


    private static byte[] CreateErrorMessage(string message)
    {
        return Encoding.UTF8.GetBytes(message);
    }

    private static void SendErrorMessage(Socket socket, byte[] errorMessage)
    {
        socket.Send(errorMessage);
    }

    private static void EnsurePath(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}