﻿using System.ComponentModel.DataAnnotations.Schema;

namespace _3ai.solutions.Translator;

public record Translation
{
    public int KeyId { get; set; }
    public int LanguageId { get; set; }
    public int ForeignId { get; set; }
    public string Value { get; set; } = string.Empty;
    [NotMapped]
    public string Name { get; set; } = string.Empty;
    [NotMapped]
    public bool IsLongText { get; set; }
    [NotMapped]
    public bool IsRichText { get; set; }
}
