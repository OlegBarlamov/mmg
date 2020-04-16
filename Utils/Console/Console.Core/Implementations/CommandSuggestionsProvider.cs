using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Console.Core.Models;
using JetBrains.Annotations;

namespace Console.Core.Implementations
{
    public class CommandSuggestion
    {
        public IConsoleCommand Command { get; }
        public int StartHighlightIndex { get; }
        public int EndHighlightIndex { get; }

        public CommandSuggestion(IConsoleCommand command, int startHighlightIndex, int endHighlightIndex)
        {
            Command = command;
            StartHighlightIndex = startHighlightIndex;
            EndHighlightIndex = endHighlightIndex;
        }
    }
    
    public class CommandSuggestionsProvider
    {
        private class DuplicateKeyComparer<TKey> : IComparer<TKey> where TKey : IComparable
        {
            public int Compare(TKey x, TKey y)
            {
                int result = x.CompareTo(y);

                if (result == 0)
                    return 1;   // Handle equality as beeing greater
                else
                    return result;
            }
        }
        
        private IConsoleCommandExecutor CommandsProvider { get; }

        public CommandSuggestionsProvider([NotNull] IConsoleCommandExecutor commandsProvider)
        {
            CommandsProvider = commandsProvider ?? throw new ArgumentNullException(nameof(commandsProvider));
        }

        public IList<CommandSuggestion> GetSuggestions(string partOfCommand)
        {
            var result = new SortedList<int, CommandSuggestion>(new DuplicateKeyComparer<int>());
            var words = partOfCommand.Split(' ');
            bool exactCommand = words.Length > 1;
            foreach (var availableCommand in CommandsProvider.GetAvailableCommands())
            {
                if (exactCommand)
                {
                    if (IsExact(availableCommand, words, out var suggestion, out var reversedPriority))
                        result.Add(reversedPriority, suggestion);
                }
                else
                {
                    if (IsSuggest(availableCommand, partOfCommand, out var suggestion, out var reversedPriority))
                        result.Add(reversedPriority, suggestion);
                }
                
            }

            return result.Values;
        }

        private bool IsExact(IConsoleCommand command, string[] wordsOfCommand,
            [CanBeNull] out CommandSuggestion suggestion, out int reversedPriority)
        {
            suggestion = null;
            reversedPriority = int.MaxValue;
            var commandName = wordsOfCommand[0];
            if (!string.Equals(commandName, command.Text, StringComparison.OrdinalIgnoreCase))
                return false;

            suggestion = new CommandSuggestion(command, 0, commandName.Length);
            if (string.IsNullOrWhiteSpace(command.Metadata?.Title))
            {
                reversedPriority = wordsOfCommand.Length > 1 ? Int32.MaxValue : 0;
                return true;
            }

            var wordsInTitle = command.Metadata.Title.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            var notEmptyWordsInCommand = wordsOfCommand.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray(); 
            if (notEmptyWordsInCommand.Length > wordsInTitle.Length)
                return false;

            reversedPriority = wordsInTitle.Length - notEmptyWordsInCommand.Length;
            return true;
        }

        private bool IsSuggest(IConsoleCommand command, string partOfCommand, [CanBeNull] out CommandSuggestion suggestion, out int reversedPriority)
        {
            var commandText = command.Text;
            var index = commandText.IndexOf(partOfCommand, StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                suggestion = new CommandSuggestion(command, index, partOfCommand.Length);
                reversedPriority = index + commandText.Length;
                return true;
            }

            suggestion = null;
            reversedPriority = int.MaxValue;
            return false;
        }
    }
}