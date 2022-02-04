namespace _3ai.solutions.Translator
{
    public interface ITranslationCache
    {
        Dictionary<int, Dictionary<int, Dictionary<int, string>>> GetTranslations();
    }
}