using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Console.Core.Commands
{
    public sealed class FixedTypedExecutableConsoleCommandDelegate : FixedTypedExecutableConsoleCommand
    {
        public override string Text { get; }
        public override string Description { get; }
     
        private Func<Task> ExecuteAsyncDelegate { get; }

        public FixedTypedExecutableConsoleCommandDelegate(string text, string description,
            [NotNull] Action execute) : this(text, description, null, WrapIntoTask(execute))
        {
            
        }
        
        public FixedTypedExecutableConsoleCommandDelegate(string text, string description,
            [NotNull] Func<Task> executeAsyncDelegate) : this(text, description, null, executeAsyncDelegate)
        {
            
        }
        
        public FixedTypedExecutableConsoleCommandDelegate(string text, string description, object data, [NotNull] Func<Task> executeAsyncDelegate)
        {
            Text = text;
            Description = description;
            Data = data;
            ExecuteAsyncDelegate = executeAsyncDelegate ?? throw new ArgumentNullException(nameof(executeAsyncDelegate));
        }
        
        protected override Task ExecuteAsync()
        {
            return ExecuteAsyncDelegate();
        }

        private static Func<Task> WrapIntoTask([NotNull] Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return () =>
            {
                action();
                return Task.CompletedTask;
            };
        }
    }
    
    public sealed class FixedTypedExecutableConsoleCommandDelegate<T> : FixedTypedExecutableConsoleCommand<T>
    {
        public override string Text { get; }
        public override string Description { get; }
     
        private Func<T, Task> ExecuteAsyncDelegate { get; }

        public FixedTypedExecutableConsoleCommandDelegate(string text, string description,
            [NotNull] Action<T> execute) : this(text, description, null, WrapIntoTask(execute))
        {
            
        }
        
        public FixedTypedExecutableConsoleCommandDelegate(string text, string description,
            [NotNull] Func<T, Task> executeAsyncDelegate) : this(text, description, null, executeAsyncDelegate)
        {
            
        }
        
        public FixedTypedExecutableConsoleCommandDelegate(string text, string description, object data, [NotNull] Func<T, Task> executeAsyncDelegate)
        {
            Text = text;
            Description = description;
            Data = data;
            ExecuteAsyncDelegate = executeAsyncDelegate ?? throw new ArgumentNullException(nameof(executeAsyncDelegate));
        }
        
        protected override Task ExecuteAsync(T parameter)
        {
            return ExecuteAsyncDelegate(parameter);
        }

        private static Func<T, Task> WrapIntoTask([NotNull] Action<T> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return parameters =>
            {
                action(parameters);
                return Task.CompletedTask;
            };
        }
    }
    
    public sealed class FixedTypedExecutableConsoleCommandDelegate<T1, T2> : FixedTypedExecutableConsoleCommand<T1, T2>
    {
        public override string Text { get; }
        public override string Description { get; }
     
        private Func<T1, T2, Task> ExecuteAsyncDelegate { get; }

        public FixedTypedExecutableConsoleCommandDelegate(string text, string description,
            [NotNull] Action<T1, T2> execute) : this(text, description, null, WrapIntoTask(execute))
        {
            
        }
        
        public FixedTypedExecutableConsoleCommandDelegate(string text, string description,
            [NotNull] Func<T1, T2, Task> executeAsyncDelegate) : this(text, description, null, executeAsyncDelegate)
        {
            
        }
        
        public FixedTypedExecutableConsoleCommandDelegate(string text, string description, object data, [NotNull] Func<T1, T2, Task> executeAsyncDelegate)
        {
            Text = text;
            Description = description;
            Data = data;
            ExecuteAsyncDelegate = executeAsyncDelegate ?? throw new ArgumentNullException(nameof(executeAsyncDelegate));
        }
        
        protected override Task ExecuteAsync(T1 parameter1, T2 parameter2)
        {
            return ExecuteAsyncDelegate(parameter1, parameter2);
        }

        private static Func<T1, T2, Task> WrapIntoTask([NotNull] Action<T1, T2> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return (parameter1, parameter2) =>
            {
                action(parameter1, parameter2);
                return Task.CompletedTask;
            };
        }
    }
    
    public sealed class FixedTypedExecutableConsoleCommandDelegate<T1, T2, T3> : FixedTypedExecutableConsoleCommand<T1, T2, T3>
    {
        public override string Text { get; }
        public override string Description { get; }
     
        private Func<T1, T2, T3, Task> ExecuteAsyncDelegate { get; }

        public FixedTypedExecutableConsoleCommandDelegate(string text, string description,
            [NotNull] Action<T1, T2, T3> execute) : this(text, description, null, WrapIntoTask(execute))
        {
            
        }
        
        public FixedTypedExecutableConsoleCommandDelegate(string text, string description,
            [NotNull] Func<T1, T2, T3, Task> executeAsyncDelegate) : this(text, description, null, executeAsyncDelegate)
        {
            
        }
        
        public FixedTypedExecutableConsoleCommandDelegate(string text, string description, object data, [NotNull] Func<T1, T2, T3, Task> executeAsyncDelegate)
        {
            Text = text;
            Description = description;
            Data = data;
            ExecuteAsyncDelegate = executeAsyncDelegate ?? throw new ArgumentNullException(nameof(executeAsyncDelegate));
        }
        
        protected override Task ExecuteAsync(T1 parameter1, T2 parameter2, T3 parameter3)
        {
            return ExecuteAsyncDelegate(parameter1, parameter2, parameter3);
        }

        private static Func<T1, T2, T3, Task> WrapIntoTask([NotNull] Action<T1, T2, T3> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return (parameter1, parameter2, parameter3) =>
            {
                action(parameter1, parameter2, parameter3);
                return Task.CompletedTask;
            };
        }
    }
    
    public sealed class FixedTypedExecutableConsoleCommandDelegate<T1, T2, T3, T4> : FixedTypedExecutableConsoleCommand<T1, T2, T3, T4>
    {
        public override string Text { get; }
        public override string Description { get; }
     
        private Func<T1, T2, T3, T4, Task> ExecuteAsyncDelegate { get; }

        public FixedTypedExecutableConsoleCommandDelegate(string text, string description,
            [NotNull] Action<T1, T2, T3, T4> execute) : this(text, description, null, WrapIntoTask(execute))
        {
            
        }
        
        public FixedTypedExecutableConsoleCommandDelegate(string text, string description,
            [NotNull] Func<T1, T2, T3, T4, Task> executeAsyncDelegate) : this(text, description, null, executeAsyncDelegate)
        {
            
        }
        
        public FixedTypedExecutableConsoleCommandDelegate(string text, string description, object data, [NotNull] Func<T1, T2, T3, T4, Task> executeAsyncDelegate)
        {
            Text = text;
            Description = description;
            Data = data;
            ExecuteAsyncDelegate = executeAsyncDelegate ?? throw new ArgumentNullException(nameof(executeAsyncDelegate));
        }
        
        protected override Task ExecuteAsync(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4)
        {
            return ExecuteAsyncDelegate(parameter1, parameter2, parameter3, parameter4);
        }

        private static Func<T1, T2, T3, T4, Task> WrapIntoTask([NotNull] Action<T1, T2, T3, T4> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return (parameter1, parameter2, parameter3, parameter4) =>
            {
                action(parameter1, parameter2, parameter3, parameter4);
                return Task.CompletedTask;
            };
        }
    }
}