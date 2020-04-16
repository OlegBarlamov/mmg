using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace ConsoleWindow.Common
{
    /// <summary> Represents object with change notifications. </summary>
    internal abstract class NotifyObject : INotifyPropertyChanged
    {
        /// <summary> The PropertyChanged invocator. </summary>
        /// <param name="property"> The property itself. </param>
        /// <param name="newValue"> New value to set. </param>
        /// <param name="propertyName"> The changed property name. </param>
        [NotifyPropertyChangedInvocator]
        protected bool SetProperty<T>(ref T property, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (Equals(property, newValue))
                return false;
            property = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary> Raised when a property is changed. </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary> The PropertyChanged invocator. </summary>
        /// <param name="propertyName"> The changed property name. </param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
