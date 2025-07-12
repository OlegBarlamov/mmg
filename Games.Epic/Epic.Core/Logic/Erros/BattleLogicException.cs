using System;

namespace Epic.Core.Logic.Erros
{
    public class BattleLogicException : Exception
    {
        public BattleLogicException(string message) : base(message) 
        {
        }
    }
}