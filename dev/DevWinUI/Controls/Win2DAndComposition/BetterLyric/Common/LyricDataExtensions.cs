// https://github.com/jayfunc/BetterLyrics

namespace DevWinUI;

public static partial class LyricDataExtensions
{
    extension(LyricData lyricsData)
    {
        public static LyricData GetEnglishSample()
        {
            static LyricLine Line(string text, int startSec, int endSec) => new LyricLine
            {
                StartMs = (int)TimeSpan.FromSeconds(startSec).TotalMilliseconds,
                EndMs = (int)TimeSpan.FromSeconds(endSec).TotalMilliseconds,
                PrimaryText = text,
                PrimarySyllables =
                [
                    new BaseLyric
                    {
                        Text = text,
                        StartMs = (int)TimeSpan.FromSeconds(startSec).TotalMilliseconds,
                        EndMs = (int)TimeSpan.FromSeconds(endSec).TotalMilliseconds,
                    }
                ],
                IsPrimaryHasRealSyllableInfo = true,
            };

            return new LyricData()
            {
                LyricsLines =
                [
                    Line("Is this the real life?",        0,  4),
                    Line("Is this just fantasy?",         4,  8),
                    Line("Caught in a landslide,",        8, 12),
                    Line("No escape from reality.",      12, 16),
                    Line("Open your eyes,",              16, 20),
                    Line("Look up to the skies and see,",20, 25),
                    Line("I'm just a poor boy,",         25, 29),
                    Line("I need no sympathy.",          29, 33),
                    Line("Because it's easy come, easy go,", 33, 38),
                    Line("Little high, little low.",     38, 43),
                    Line("Anyway the wind blows",        43, 47),
                    Line("Doesn't really matter to me,", 47, 52),
                    Line("To me.",                       52, 56),
                ],
            };
        }
        public static LyricData GetPersianSample()
        {
            static LyricLine Line(string text, int startSec, int endSec) => new LyricLine
            {
                StartMs = (int)TimeSpan.FromSeconds(startSec).TotalMilliseconds,
                EndMs = (int)TimeSpan.FromSeconds(endSec).TotalMilliseconds,
                PrimaryText = text,
                PrimarySyllables =
                [
                    new BaseLyric
            {
                Text = text,
                StartMs = (int)TimeSpan.FromSeconds(startSec).TotalMilliseconds,
                EndMs = (int)TimeSpan.FromSeconds(endSec).TotalMilliseconds,
            }
                ],
                IsPrimaryHasRealSyllableInfo = true,
            };

            return new LyricData()
            {
                LyricsLines =
                [
                    Line("آیا این زندگی واقعی است؟", 0, 4),
            Line("آیا این فقط خیال است؟", 4, 8),
            Line("در یک رانش زمین گرفتار شده‌ام،", 8, 12),
            Line("هیچ راه گریزی از واقعیت نیست.", 12, 16),
            Line("چشمانت را باز کن،", 16, 20),
            Line("به آسمان نگاه کن و ببین،", 20, 25),
            Line("من فقط یک پسر فقیرم،", 25, 29),
            Line("نیازی به دلسوزی ندارم.", 29, 33),
            Line("چون هر چه آسان می‌آید، آسان می‌رود،", 33, 38),
            Line("کمی بالا، کمی پایین.", 38, 43),
            Line("به هر حال باد می‌وزد", 43, 47),
            Line("برای من اهمیتی ندارد،", 47, 52),
            Line("برای من.", 52, 56),
        ],
            };
        }
        public static LyricData GetEnglishWithTranslationSample()
        {
            static LyricLine Line(string text, string translation, string pronunciation, int startSec, int endSec) => new LyricLine
            {
                StartMs = (int)TimeSpan.FromSeconds(startSec).TotalMilliseconds,
                EndMs = (int)TimeSpan.FromSeconds(endSec).TotalMilliseconds,
                PrimaryText = text,
                SecondaryText = translation,
                TertiaryText = pronunciation,
                PrimarySyllables =
                [
                    new BaseLyric
                    {
                        Text = text,
                        StartMs = (int)TimeSpan.FromSeconds(startSec).TotalMilliseconds,
                        EndMs = (int)TimeSpan.FromSeconds(endSec).TotalMilliseconds,
                    }
                ],
                IsPrimaryHasRealSyllableInfo = true,
            };

            return new LyricData()
            {
                LyricsLines =
                [
                    Line("Is this the real life?",              "これは現実なの？",                  "Kore wa genjitsu na no?",              0,  4),
                    Line("Is this just fantasy?",               "それとも幻想なの？",                 "Soretomo gensō na no?",                4,  8),
                    Line("Caught in a landslide,",              "地滑りに巻き込まれて、",              "Jisuberi ni makikomarete,",            8, 12),
                    Line("No escape from reality.",             "現実から逃げ場はない。",              "Genjitsu kara nigeba wa nai.",         12, 16),
                    Line("Open your eyes,",                     "目を開けて、",                       "Me wo akete,",                         16, 20),
                    Line("Look up to the skies and see,",       "空を見上げてごらん、",               "Sora wo miagete goran,",               20, 25),
                    Line("I'm just a poor boy,",                "僕はただの貧しい少年、",             "Boku wa tada no mazushii shōnen,",     25, 29),
                    Line("I need no sympathy.",                 "同情なんていらない。",               "Dōjō nante iranai.",                   29, 33),
                    Line("Because it's easy come, easy go,",   "なぜなら簡単に来て、簡単に去るから、", "Nazenara kantan ni kite, kantan ni saru kara,", 33, 38),
                    Line("Little high, little low.",            "少し高く、少し低く。",               "Sukoshi takaku, sukoshi hikuku.",       38, 43),
                    Line("Anyway the wind blows",               "どちらに風が吹いても",               "Dochira ni kaze ga fuite mo",           43, 47),
                    Line("Doesn't really matter to me,",       "僕にはどうでもいい、",               "Boku ni wa dō demo ii,",               47, 52),
                    Line("To me.",                              "僕には。",                           "Boku ni wa.",                          52, 56),
                ],
            };
        }

        public static LyricData GetLoadingPlaceholder()
        {
            return new LyricData()
            {
                LyricsLines = [
                    new LyricLine
                    {
                        StartMs = 0,
                        EndMs = (int)TimeSpan.FromSeconds(30).TotalMilliseconds,
                        PrimaryText = "Loading...",
                        PrimarySyllables = [new BaseLyric { Text = "Loading...", StartMs = 0, EndMs = (int)TimeSpan.FromSeconds(30).TotalMilliseconds }],
                        IsPrimaryHasRealSyllableInfo = true,
                    },
                ],
            };
        }

        public static LyricData GetNotfoundPlaceholder()
        {
            return new LyricData([new LyricLine
            {
                StartMs = 0,
                EndMs = (int)TimeSpan.FromMinutes(99).TotalMilliseconds,
                PrimaryText = "Not Found",
                PrimarySyllables = [new BaseLyric { Text = "Not Found", StartMs = 0, EndMs = (int)TimeSpan.FromMinutes(99).TotalMilliseconds }],
            }]);
        }

        public void SetTranslatedText(LyricData translationData, int toleranceMs = 50)
        {
            foreach (var line in lyricsData.LyricsLines)
            {
                // 在翻译歌词中查找与当前行开始时间最接近且在容忍范围内的行
                var transLine = translationData.LyricsLines
                    .FirstOrDefault(t => Math.Abs(t.StartMs - line.StartMs) <= toleranceMs);

                if (transLine != null)
                {
                    // 此处 transLine.PrimaryText 指翻译中的“原文”属性
                    line.SecondaryText = transLine.PrimaryText;
                }
                else
                {
                    // 没有匹配的翻译
                    line.SecondaryText = "";
                }
            }
        }

        public void SetPhoneticText(LyricData phoneticData, int toleranceMs = 50)
        {
            foreach (var line in lyricsData.LyricsLines)
            {
                // 在音译歌词中查找与当前行开始时间最接近且在容忍范围内的行
                var transLine = phoneticData.LyricsLines
                    .FirstOrDefault(t => Math.Abs(t.StartMs - line.StartMs) <= toleranceMs);

                if (transLine != null)
                {
                    // 此处 transLine.PrimaryText 指音译中的“原文”属性
                    line.TertiaryText = transLine.PrimaryText;
                }
                else
                {
                    // 没有匹配的音译
                    line.TertiaryText = "";
                }
            }
        }

        public void SetTranslation(string translation)
        {
            List<string> translationArr = translation.Split("\n").ToList();
            int i = 0;
            foreach (var line in lyricsData.LyricsLines)
            {
                if (i >= translationArr.Count)
                {
                    line.SecondaryText = ""; // No translation available, keep empty
                }
                else
                {
                    line.SecondaryText = translationArr[i];
                }
                i++;
            }
        }

        public void SetTransliteration(string transliteration)
        {
            List<string> transliterationArr = transliteration.Split("\n").ToList();
            int i = 0;
            foreach (var line in lyricsData.LyricsLines)
            {
                if (i >= transliterationArr.Count)
                {
                    line.TertiaryText = ""; // No transliteration available, keep empty
                }
                else
                {
                    line.TertiaryText = transliterationArr[i];
                }
                i++;
            }
        }

        public LyricLine? GetLyricsLine(double sec)
        {
            for (int i = 0; i < lyricsData.LyricsLines.Count; i++)
            {
                var line = lyricsData.LyricsLines[i];
                if (line.StartMs > sec * 1000)
                {
                    return lyricsData.LyricsLines.ElementAtOrDefault(i - 1);
                }
            }
            return null;
        }

    }
}
