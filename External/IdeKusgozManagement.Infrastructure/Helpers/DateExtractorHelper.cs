using System.Globalization;
using System.Text.RegularExpressions;

namespace IdeKusgozManagement.Infrastructure.Helpers
{
    /// <summary>
    /// Metin içeriklerinden tarih bilgisini çıkarmak için yardımcı static sınıf
    /// </summary>
    public static class DateExtractorHelper
    {
        private static readonly CultureInfo TrCulture = new("tr-TR");

        /// <summary>
        /// Desteklenen tarih formatları ve regex patternleri
        /// </summary>
        private static readonly Dictionary<string, string[]> DatePatterns = new()
        {
            [@"(\d{1,2})\.(\d{1,2})\.(\d{4})"] = new[] { "dd.MM.yyyy", "d.M.yyyy" },
            [@"(\d{1,2})/(\d{1,2})/(\d{4})"] = new[] { "dd/MM/yyyy", "d/M/yyyy", "MM/dd/yyyy", "M/d/yyyy" },
            [@"(\d{4})-(\d{1,2})-(\d{1,2})"] = new[] { "yyyy-MM-dd", "yyyy-M-d" },
            [@"(\d{1,2})-(\d{1,2})-(\d{4})"] = new[] { "dd-MM-yyyy", "d-M-yyyy", "MM-dd-yyyy", "M-d-yyyy" }
        };

        /// <summary>
        /// Verilen metinden ilk tespit edilen tarihi döndürür
        /// </summary>
        /// <param name="content">Tarih aranacak metin içeriği</param>
        /// <param name="minYear">Minimum kabul edilecek yıl (varsayılan: 1900)</param>
        /// <param name="maxYear">Maximum kabul edilecek yıl (varsayılan: 2100)</param>
        /// <returns>Tespit edilen tarih, bulunamadıysa null</returns>
        public static DateTime? ExtractDate(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return null;

            // Boşlukları normalize et
            var normalizedContent = NormalizeWhitespace(content);
            int minYear = 1900;
            int maxYear = 2100;
            foreach (var (pattern, formats) in DatePatterns)
            {
                var matches = Regex.Matches(normalizedContent, pattern, RegexOptions.Compiled);

                foreach (Match match in matches)
                {
                    if (TryParseWithFormats(match.Value.Trim(), formats, out DateTime date))
                    {
                        if (IsValidYear(date, minYear, maxYear))
                        {
                            return date;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Verilen tarih string'ini belirtilen formatlarla parse etmeye çalışır
        /// </summary>
        private static bool TryParseWithFormats(string dateString, string[] formats, out DateTime date)
        {
            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(
                    dateString,
                    format,
                    TrCulture,
                    DateTimeStyles.None,
                    out date))
                {
                    return true;
                }
            }

            date = default;
            return false;
        }

        /// <summary>
        /// Tarihin geçerli yıl aralığında olup olmadığını kontrol eder
        /// </summary>
        private static bool IsValidYear(DateTime date, int minYear, int maxYear)
        {
            return date.Year >= minYear && date.Year <= maxYear;
        }

        /// <summary>
        /// Metindeki ardışık boşluk karakterlerini tek boşluğa indirir
        /// </summary>
        private static string NormalizeWhitespace(string content)
        {
            return Regex.Replace(content, @"\s+", " ", RegexOptions.Compiled);
        }

        /// <summary>
        /// Tarihi Türkçe formatta string'e çevirir (15 Ekim 2025)
        /// </summary>
        public static string FormatDateTurkish(DateTime date)
        {
            return date.ToString("dd MMMM yyyy", TrCulture);
        }

        /// <summary>
        /// Tarihi kısa Türkçe formatta string'e çevirir (15.10.2025)
        /// </summary>
        public static string FormatDateShort(DateTime date)
        {
            return date.ToString("dd.MM.yyyy", TrCulture);
        }
    }
}