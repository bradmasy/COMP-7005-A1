using static Client.Constants;

namespace Client;

public static class Validator
{
    public static void ValidateArguments(string[] args)
    {
        if (args.Length == NoArguments)
        {
            throw new Exception("Error: Please provide an argument for the path, the word and cipher shift amount.");
        }

        if (!File.Exists(args[SocketPath]))
        {
            throw new Exception("Error: Please provide a valid path for connection.");
        }

        if (args.Length != ArgumentAmount)
        {
            throw new Exception(
                "Error: You must provide exactly 3 arguments. Arg 1 for the path. Arg 2 for the word, and Arg 3 for the cipher shift amount.");
        }
    }


    public static int ValidateShiftArgument(string shiftArg)
    {
        var isInt = int.TryParse(shiftArg, out var shift);

        if (!isInt) throw new Exception("Error: Please provide a valid integer value to shift word.");
        return shift;
    }
}