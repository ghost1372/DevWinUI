namespace DevWinUI;

public sealed partial class ActivityDay
{
    public ActivityDay() { }

    public ActivityDay(DateTime date, int count)
    {
        Date = date.Date;
        Count = count;
    }

    public DateTime Date { get; set; }
    public int Count { get; set; }
}
