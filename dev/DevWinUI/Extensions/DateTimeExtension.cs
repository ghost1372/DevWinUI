using System.Globalization;
namespace DevWinUI;

public static partial class DateTimeExtension
{
    extension(DateTime dateTime)
    {
        /// <summary>
        /// Get Shamsi Date From Miladi Year
        /// </summary>
        /// <param name="dateTime">Enter The Jalali DateTime</param>
        /// <returns></returns>
        public string ToShamsiDate()
        {
            PersianDateTime persianDateShamsi = new PersianDateTime();
            return persianDateShamsi.GetShamsiYearToString(dateTime) + "/" +
                   persianDateShamsi.GetShamsiMonthString(dateTime) + "/" +
                   persianDateShamsi.GetShamsiDayString(dateTime);
        }

        /// <summary>
        /// Get Short Shamsi Date From Miladi Year
        /// </summary>
        /// <param name="dateTime">Enter The Jalali DateTime</param>
        /// <returns></returns>
        public string ToShortShamsiDate()
        {
            PersianDateTime persianDateShamsi = new PersianDateTime();
            return persianDateShamsi.GetShortShamsiYear(dateTime) + "/" +
                   persianDateShamsi.GetShamsiMonthString(dateTime) + "/" +
                   persianDateShamsi.GetShamsiDayString(dateTime);
        }

        /// <summary>
        /// Get Long Shamsi Date From Miladi Year
        /// </summary>
        /// <param name="dateTime">Enter The Jalali DateTime</param>
        /// <returns></returns>
        public string ToLongShamsiDate()
        {
            PersianDateTime persianDateShamsi = new PersianDateTime();
            return persianDateShamsi.GetShamsiDayName(dateTime) + " " + persianDateShamsi.GetShamsiDay(dateTime) + " " +
                   persianDateShamsi.GetShamsiMonthName(dateTime) + " " + persianDateShamsi.GetShamsiYear(dateTime);
        }

        public string GetDiffrenceToNow()
        {
            var Current = DateTime.Now;
            var ts = (Current - dateTime);
            string opr = "پیش";
            if (dateTime > DateTime.Now)
            {
                opr = "بعد";
                ts = dateTime - Current;
            }

            if (ts.TotalMinutes < 1)
                return "اکنون";
            if (ts.TotalMinutes < 60)
                return $"{ts.Minutes} دقیقه {opr}";
            if (ts.TotalDays < 1)
            {
                return $"{ts.Hours} ساعت و {ts.Minutes} دقیقه {opr}";
            }

            if (ts.TotalDays < 30)
            {
                return $"{ts.Days} روز و {ts.Hours} ساعت و {ts.Minutes} دقیقه {opr}";
            }

            if (ts.TotalDays > 30 && ts.TotalDays < 365)
            {
                var months = Math.Floor(ts.TotalDays / 30);
                var days = Math.Floor(ts.TotalDays % 30);
                return (months > 0 ? $"{months} ماه و " : String.Empty) +
                       (days > 0 ? $"{days} روز و " : String.Empty) +
                       (ts.Hours > 0 ? $"{ts.Hours} ساعت و " : String.Empty) +
                       (ts.Minutes > 0 ? $"{ts.Minutes} دقیقه " : String.Empty) + opr;
            }

            if (ts.TotalDays >= 365)
            {
                var year = Math.Floor(ts.TotalDays / 365);
                var months = Math.Floor(ts.TotalDays % 365 / 30);
                var days = Math.Floor(ts.TotalDays % 365 / 30);
                return (year > 0 ? $"{year} سال و " : String.Empty) +
                       (months > 0 ? $"{months} ماه و " : String.Empty) +
                       (days > 0 ? $"{days} روز و " : String.Empty) +
                       (ts.Hours > 0 ? $"{ts.Hours} ساعت و " : String.Empty) +
                       (ts.Minutes > 0 ? $"{ts.Minutes} دقیقه " : String.Empty) + opr;
            }

            return "نامشخص";
        }

        public DateTime ToGregorianDate()
        {
            GregorianCalendar gc = new GregorianCalendar();
            return new DateTime(gc.GetYear(dateTime), gc.GetMonth(dateTime), gc.GetDayOfMonth(dateTime),
                gc.GetHour(dateTime), gc.GetMinute(dateTime), gc.GetSecond(dateTime),
                new System.Globalization.PersianCalendar());
        }
    }

    extension(PersianDateTime persianDateTime)
    {
        public string GetDiffrenceToNow() => GetDiffrenceToNow(persianDateTime.DateTime);

        public double DateDifference(PersianDateTime value) => (persianDateTime.DateTime - value.DateTime).TotalDays;

        public DateTime ToGregorianDate()
        {
            GregorianCalendar gc = new GregorianCalendar();
            return new DateTime(gc.GetYear(persianDateTime.DateTime), gc.GetMonth(persianDateTime.DateTime),
                gc.GetDayOfMonth(persianDateTime.DateTime), gc.GetHour(persianDateTime.DateTime),
                gc.GetMinute(persianDateTime.DateTime), gc.GetSecond(persianDateTime.DateTime),
                new System.Globalization.PersianCalendar());
        }
    }
}
