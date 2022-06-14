using System;
using JetBrains.Annotations;

namespace Console.Core.Commands.Types
{
    public class EnumTypeDescription : IConsoleCommandTypeDescription
    {
        [NotNull] private Type EnumType { get; }
        public string Title { get; }

        public EnumTypeDescription([NotNull] Type enumType)
        {
            if (enumType == null) throw new ArgumentNullException(nameof(enumType));
            if (!enumType.IsEnum) throw new ArgumentException(nameof(enumType));
            
            EnumType = enumType;
            Title = enumType.Name;
        }
        
        public object Parse(string parameter)
        {
            return Enum.Parse(EnumType, parameter, true);
        }

        public bool IsParsable(string parameter)
        {
            try
            {
                return !string.IsNullOrWhiteSpace(Enum.Parse(EnumType, parameter, true).ToString());
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}