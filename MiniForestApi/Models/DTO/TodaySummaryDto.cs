using System;

public class TodaySummaryDto(DateTime date, int totalMinutes)
{
    public DateTime Date { get; set; }=date;
    public int TotalMinutes { get; set; }= totalMinutes;


}

