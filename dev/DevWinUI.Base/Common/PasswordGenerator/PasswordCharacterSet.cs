namespace DevWinUI;

[Flags]
public enum PasswordCharacterSet
{
    None = 0,
    UpperCase = 1 << 0, // ABC
    LowerCase = 1 << 1, // abc
    Numbers = 1 << 2,   // 0123
    Math = 1 << 3,      // + - * / < = >
    Punctuation = 1 << 4, // ! , . : ; ?
    Brackets = 1 << 5,  // ( ) [ ] { }
    Quotes = 1 << 6,    // " ' `
    Special = 1 << 7,    // # & $ % @ \ ~ ^ _ |
    Space = 1 << 8 // Space
}
