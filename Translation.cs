using System.ComponentModel.DataAnnotations.Schema;

namespace _3ai.solutions.Translator
{
    public record Translation
    {
        public int KeyId { get; set; }
        [ForeignKey("Language")]
        public int LanguageId { get; set; }
        public int ForeignId { get; set; }
        public string Value { get; set; } = string.Empty;
        [NotMapped]
        public string Name { get; set; } = string.Empty;
        [NotMapped]
        public bool IsLongText { get; set; }

    }
    [AttributeUsage(AttributeTargets.Property)]
    public class Translatable : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public class TranslationKey : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public class TranslationLongText : Attribute { }
}