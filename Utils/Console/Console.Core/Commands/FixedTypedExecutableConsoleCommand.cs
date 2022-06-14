using System.Collections.Generic;
using System.Threading.Tasks;
using Console.Core.Commands.Types;

namespace Console.Core.Commands
{
    public abstract class FixedTypedExecutableConsoleCommand : TypedExecutableConsoleCommand
    {
        protected sealed override IConsoleCommandTypeDescription[] ExpectedTypes { get; } = new IConsoleCommandTypeDescription[0];

        protected override Task ExecuteAsyncWithVerifiedParametersTypes(IReadOnlyList<object> parameters)
        {
            return ExecuteAsync();
        }

        protected abstract Task ExecuteAsync();
    }
    
    public abstract class FixedTypedExecutableConsoleCommand<T> : TypedExecutableConsoleCommand
    {
        protected sealed override IConsoleCommandTypeDescription[] ExpectedTypes { get; }

        protected FixedTypedExecutableConsoleCommand()
        {
            ExpectedTypes = new []
            {
                ConsoleCommandTypeDescriptionAssociator.GetConsoleCommandTypeDescription(typeof(T))
            };
        }
        
        protected override Task ExecuteAsyncWithVerifiedParametersTypes(IReadOnlyList<object> parameters)
        {
            return ExecuteAsync((T) parameters[0]);
        }

        protected abstract Task ExecuteAsync(T parameter);
    }
    
    public abstract class FixedTypedExecutableConsoleCommand<T1, T2> : TypedExecutableConsoleCommand
    {
        protected sealed override IConsoleCommandTypeDescription[] ExpectedTypes { get; }

        protected FixedTypedExecutableConsoleCommand()
        {
            ExpectedTypes = new []
            {
                ConsoleCommandTypeDescriptionAssociator.GetConsoleCommandTypeDescription(typeof(T1)),
                ConsoleCommandTypeDescriptionAssociator.GetConsoleCommandTypeDescription(typeof(T2)),
            };
        }
        
        protected override Task ExecuteAsyncWithVerifiedParametersTypes(IReadOnlyList<object> parameters)
        {
            return ExecuteAsync((T1) parameters[0], (T2) parameters[1]);
        }

        protected abstract Task ExecuteAsync(T1 parameter1, T2 parameter2);
    }
    
    public abstract class FixedTypedExecutableConsoleCommand<T1, T2, T3> : TypedExecutableConsoleCommand
    {
        protected sealed override IConsoleCommandTypeDescription[] ExpectedTypes { get; }

        protected FixedTypedExecutableConsoleCommand()
        {
            ExpectedTypes = new []
            {
                ConsoleCommandTypeDescriptionAssociator.GetConsoleCommandTypeDescription(typeof(T1)),
                ConsoleCommandTypeDescriptionAssociator.GetConsoleCommandTypeDescription(typeof(T2)),
                ConsoleCommandTypeDescriptionAssociator.GetConsoleCommandTypeDescription(typeof(T3)),
            };
        }
        
        protected override Task ExecuteAsyncWithVerifiedParametersTypes(IReadOnlyList<object> parameters)
        {
            return ExecuteAsync((T1) parameters[0], (T2) parameters[1], (T3) parameters[2]);
        }

        protected abstract Task ExecuteAsync(T1 parameter1, T2 parameter2, T3 parameter3);
    }
    
    public abstract class FixedTypedExecutableConsoleCommand<T1, T2, T3, T4> : TypedExecutableConsoleCommand
    {
        protected sealed override IConsoleCommandTypeDescription[] ExpectedTypes { get; }

        protected FixedTypedExecutableConsoleCommand()
        {
            ExpectedTypes = new []
            {
                ConsoleCommandTypeDescriptionAssociator.GetConsoleCommandTypeDescription(typeof(T1)),
                ConsoleCommandTypeDescriptionAssociator.GetConsoleCommandTypeDescription(typeof(T2)),
                ConsoleCommandTypeDescriptionAssociator.GetConsoleCommandTypeDescription(typeof(T3)),
                ConsoleCommandTypeDescriptionAssociator.GetConsoleCommandTypeDescription(typeof(T4)),
            };
        }
        
        protected override Task ExecuteAsyncWithVerifiedParametersTypes(IReadOnlyList<object> parameters)
        {
            return ExecuteAsync((T1) parameters[0], (T2) parameters[1], (T3) parameters[2], (T4) parameters[3]);
        }

        protected abstract Task ExecuteAsync(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4);
    }
}