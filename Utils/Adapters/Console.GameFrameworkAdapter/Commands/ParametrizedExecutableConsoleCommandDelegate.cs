using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Console.FrameworkAdapter.Commands
{
    public sealed class ParametrizedExecutableConsoleCommandDelegate : ParametrizedExecutableConsoleCommand
    {
        public override string Text { get; }
        public override string Description { get; }
        public override string Title { get; }
        
        private Func<string[], Task> ExecuteAsync { get; }

        public ParametrizedExecutableConsoleCommandDelegate(string text, string title, string description,
            [NotNull] Action execute) : this(text, title, description, null, WrapIntoTask(execute))
        {
        }
        
        public ParametrizedExecutableConsoleCommandDelegate(string text, string title, string description,
            [NotNull] Action<string[]> execute) : this(text, title, description, null, WrapIntoTask(execute))
        {
            
        }
        
        public ParametrizedExecutableConsoleCommandDelegate(string text, string title, string description,
            [NotNull] Func<string[], Task> executeAsync) : this(text, title, description, null, executeAsync)
        {
            
        }
        
        public ParametrizedExecutableConsoleCommandDelegate(string text, string title, string description, object data,
            [NotNull] Func<string[], Task> executeAsync)
        {
            Text = text;
            Title = title;
            Description = description;
            ExecuteAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            Data = data;
        }

        protected override Task ExecuteAsyncWithParameters(string[] parameters)
        {
            return ExecuteAsync(parameters);
        }
        
        private static Func<string[], Task> WrapIntoTask([NotNull] Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return parameters =>
            {
                action();
                return Task.CompletedTask;
            };
        }

        private static Func<string[], Task> WrapIntoTask([NotNull] Action<string[]> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return parameters =>
            {
                action(parameters);
                return Task.CompletedTask;
            };
        }
    }
}