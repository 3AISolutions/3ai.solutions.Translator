# 3ai.solutions.Translator

## Dependancy Injection

```csharp
builder.Services.AddTranslationService<ImplementedITranslationRepository>();
```

## Database

Create entities for Translation and TranslationName, note that you will need some sort of entity for your language selection

```csharp
    private static void CreateSystemModels(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Language>(e =>
        {
            e.ToTable("SYS_Languages");
            e.HasData(
                new Language() { Id = 1, Code = "en-gb", Name = "English", IsActive = true },
                new Language() { Id = 2, Code = "el-gr", Name = "Ελληνικά", IsActive = true }
            );
        });

        modelBuilder.Entity<Translation>(e =>
        {
            e.ToTable("SYS_Translation");
            e.HasKey(p => new { p.ForeignId, p.LanguageId, p.KeyId });
            e.HasOne<Language>().WithMany().HasForeignKey(p => p.LanguageId).IsRequired();
        });

        modelBuilder.Entity<TranslationName>(e =>
        {
            e.ToTable("SYS_TranslationName");
            e.HasNoKey().HasIndex(p => p.KeyId).IsUnique(); 
        });
    }
```

## Sample Service

```csharp
public class SampleService
{
    private readonly IMemoryCache memoryCache;
    private readonly TranslationService translationService;
    private readonly SampleContext context;

    public SampleService(TranslationService translationService, SampleContext context,
                         IMemoryCache memoryCache)
    {
        this.memoryCache = memoryCache;
        this.translationService = translationService;
        this.context = context;
    }

    //Simple cache example
    public async Task<List<Item>> GetCachedTranslatedItemsAsync()
    {
        var items = await memoryCache.GetOrCreateAsync("items", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await context.Items.ToListAsync();
        }) ?? new();
        return await translationService.GetTranslatedAsync(items, languageId: 1, newItem: true);
    }

    public async Task<List<Item>> GetCachedTranslatedItemsAsync(int languageId = 1)
    {
        return = await memoryCache.GetOrCreateAsync($"items:{languageId}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            if (languageId == 1)
                return await context.Items.ToListAsync();
            else
                return await translationService.GetTranslatedAsync(await GetCachedTranslatedItemsAsync(),
                                                                   languageId: languageId, newItem: true);
        }) ?? new();
    }

    //Returns items translated into specified language,
    //please note that languageId 1 is reserved for the default language
    public async Task<List<Item>> GetTranslatedItemsAsync(List<Item> items)
    {
        return await translationService.GetTranslatedAsync(items, languageId: 1, newItem: true);
    }

    //Creates entries with the name of the property and it's keyId
    public async Task CreateTranslationNamesAsync()
    {
        await translationService.SaveTransltionNamesAsync<Item>();
    }

    //Returns properties of Item which can be translated
    public List<Translation> GetTranslatableItemsOfItem(Item item)
    {
        return TranslationService.GetTranslatableItems(item);
    }
}
```
