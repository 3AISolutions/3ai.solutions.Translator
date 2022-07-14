namespace _3ai.solutions.Translator
{
    public interface ITranslationRepository
    {
        public Dictionary<int, Dictionary<int, Dictionary<int, string>>> GetTranslations();
        public Task SaveTranslation(Translation translation);
        public Task SaveTranslation(List<Translation> translation);
    }
}