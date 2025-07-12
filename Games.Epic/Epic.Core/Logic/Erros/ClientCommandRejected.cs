namespace Epic.Core.Logic.Erros
{
    public class ClientCommandRejected : BattleLogicException
    {
        public ClientCommandRejected(string message) : base(message)
        {
        }
    }
}