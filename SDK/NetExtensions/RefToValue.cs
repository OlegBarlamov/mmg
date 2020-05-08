namespace NetExtensions
{
    public class RefToValue<T> where T : struct
    {
        public T Value { get; set; }

        public RefToValue()
            :this(default)
        {
            
        }
        
        public RefToValue(T value)
        {
            Value = value;
        }
    }
}