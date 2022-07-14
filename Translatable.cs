namespace _3ai.solutions.Translator
{
    [AttributeUsage(AttributeTargets.Property)]
    public class Translatable : Attribute
    {
        private readonly string? _friendlyName;
        public Translatable() { }
        public Translatable(string friendlyName) { _friendlyName = friendlyName; }
        public string? Name { get { return _friendlyName; } }
    }
}