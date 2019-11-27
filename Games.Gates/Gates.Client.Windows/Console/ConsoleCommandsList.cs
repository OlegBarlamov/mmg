using ConsoleWindow;

namespace Gates.Client.Windows.Console
{
    internal static class ConsoleCommandsList
    {
        public static readonly CommandDescription[] Commands = {
            new  CommandDescription("srv_connect")
            {
                CommandSignature = "srv_connect [url]",
            },
            new  CommandDescription("srv_auth")
            {
                CommandSignature = "srv_auth [name]",
            },
            new  CommandDescription("rm_create")
            {
                CommandSignature = "rm_create [name] [password]",
            },
            new  CommandDescription("rm_enter")
            {
                CommandSignature = "rm_enter [name] [password]",
            },
            new  CommandDescription("start")
            {
                CommandSignature = "start",
            },
        };
    }
}
