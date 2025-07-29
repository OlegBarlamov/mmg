namespace Epic.Logic
{
    public class ClientUserAction
    {
        public string CommandName { get; }

        public ClientUserAction(string commandName)
        {
            CommandName = commandName;
        }
    }
}