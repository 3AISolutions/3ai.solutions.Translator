using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace _3ai.solutions.Translator
{
    public class TranslationService
    {
        private readonly Dictionary<int, Dictionary<int, Dictionary<int, string>>> _translations;

        public TranslationService(ITranslationRepository repo)
        {
            _translations = repo.GetTranslations();
        }

        public List<T>? GetTranslated<T>(List<T> items, int languageId, bool newItem = false)
        {
            if (items == null || !items.Any() || languageId == 1) return items;

            List<T>? newItems = (List<T>?)Activator.CreateInstance(typeof(List<T>));
            if (newItems is null) throw new Exception();
            foreach (T item in items)
            {
                var it = GetTranslated(item, languageId, GetPrimaryKeyValue(item), newItem);
                if (it is not null)
                    newItems.Add(it);
            }
            return newItems;
        }

        public T? GetTranslated<T>(T item, int languageId, int? foreignId, bool newItem = false)
        {
            if (item != null && languageId > 1)
            {
                if (foreignId is null || foreignId == 0) foreignId = GetPrimaryKeyValue(item);
                if (foreignId is null || foreignId == 0) throw new Exception($"No TranslationKey found");
                if (_translations.TryGetValue(languageId, out Dictionary<int, Dictionary<int, string>>? LangDict) &&
                    LangDict.TryGetValue(foreignId.Value, out Dictionary<int, string>? ItemDict))
                {
                    if (newItem)
                    {
                        T? translatedItem = (T?)Activator.CreateInstance(typeof(T));
                        foreach (var kv in GetAllPropertyInfos<T>())
                        {
                            string key = $"{kv.Key.ReflectedType?.FullName}.{kv.Key.Name}";
                            object? value = kv.Key.GetValue(item);
                            if (value is string && kv.Value && ItemDict != null && ItemDict.TryGetValue(GetHash(key), out string? trans))
                            {
                                kv.Key.SetValue(translatedItem, trans);
                            }
                            else
                            {
                                kv.Key.SetValue(translatedItem, value);
                            }
                        }
                        return translatedItem;
                    }
                    else
                    {
                        foreach (PropertyInfo propertyInfo in GetPropertyInfos<T>())
                        {
                            string key = $"{propertyInfo.ReflectedType?.FullName}.{propertyInfo.Name}";
                            if (ItemDict != null && ItemDict.TryGetValue(GetHash(key), out string? trans))
                                propertyInfo.SetValue(item, trans);
                        }
                    }

                }
            }
            return item;
        }

        public static List<Translation> GetTranslatableItems<T>(T item, int? foreignId = 0)
        {
            if (foreignId is null || foreignId == 0) foreignId = GetPrimaryKeyValue(item);
            if (foreignId is null || foreignId == 0) throw new Exception($"No TranslationKey found");
            List<Translation> lst = new();
            foreach (PropertyInfo propertyInfo in GetPropertyInfos<T>())
            {
                string key = $"{propertyInfo.ReflectedType?.FullName}.{propertyInfo.Name}";
                lst.Add(new Translation
                {
                    KeyId = GetHash(key),
                    ForeignId = foreignId.Value,
                    Name = propertyInfo.GetCustomAttribute<Translatable>()?.Name ?? propertyInfo.Name,
                    Value = (string)(propertyInfo.GetValue(item) ?? ""),
                    IsLongText = propertyInfo.GetCustomAttribute<TranslationLongText>() != null,
                    IsRichText = propertyInfo.GetCustomAttribute<TranslationRichText>() != null
                });
            }
            return lst;
        }

        private static KeyValuePair<PropertyInfo, bool>[] GetAllPropertyInfos<T>()
        {
            List<KeyValuePair<PropertyInfo, bool>> lst = new();
            Type type = typeof(T);
            foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (propertyInfo.GetCustomAttribute<Translatable>() != null)
                    lst.Add(new KeyValuePair<PropertyInfo, bool>(propertyInfo, true));
                else
                    lst.Add(new KeyValuePair<PropertyInfo, bool>(propertyInfo, false));
            }
            return lst.ToArray();
        }

        private static int? GetPrimaryKeyValue<T>(T entity)
        {
            Type type = typeof(T);
            PropertyInfo? propertyInfo = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                        .FirstOrDefault(p => p.GetCustomAttributes<TranslationKey>().Any());
            if (propertyInfo is null) throw new Exception($"No TranslationKey for {type.Name} found");
            return (int?)propertyInfo.GetValue(entity);
        }

        private static PropertyInfo[] GetPropertyInfos<T>()
        {
            List<PropertyInfo> lst = new();
            Type type = typeof(T);
            foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (propertyInfo.GetCustomAttributes<Translatable>().Any())
                    lst.Add(propertyInfo);
            }
            return lst.ToArray();
        }

        private static int GetHash(string key)
        {
            using MD5 md5Hasher = MD5.Create();
            byte[] hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(key));
            return BitConverter.ToInt32(hashed, 0);
        }
    }
}
