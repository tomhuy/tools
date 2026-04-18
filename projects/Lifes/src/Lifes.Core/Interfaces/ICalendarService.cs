using Lifes.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lifes.Core.Interfaces;

public interface ICalendarService
{
    Task<IEnumerable<MementoModel>> GetAnnualEventsAsync(int year);
    Task<IEnumerable<MementoModel>> GetMonthlyEventsAsync(int year, int month);
}
