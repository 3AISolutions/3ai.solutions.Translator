namespace _3ai.solutions.Translator;

public interface ITranslationRepository
{
    public Dictionary<int, Dictionary<int, Dictionary<int, string>>> GetTranslations();
    public Task<Dictionary<int, Dictionary<int, string>>?> GetTranslationsAsync(int languageId, CancellationToken cancellationToken = default);
    public Task SaveTranslationAsync(Translation translation, CancellationToken cancellationToken = default);
    public Task SaveTranslationAsync(List<Translation> translations, CancellationToken cancellationToken = default);
    public Task SaveTranslationNameAsync(TranslationName translationName, CancellationToken cancellationToken = default);
    public Task SaveTranslationNameAsync(List<TranslationName> translationNames, CancellationToken cancellationToken = default);
}
