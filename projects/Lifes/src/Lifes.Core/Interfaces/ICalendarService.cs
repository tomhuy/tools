using Lifes.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lifes.Core.Interfaces;

public interface ICalendarService
{
    Task<IEnumerable<CalendarEventModel>> GetAnnualEventsAsync(int year);
    Task<IEnumerable<CalendarEventModel>> GetMonthlyEventsAsync(int year, int month);
}
