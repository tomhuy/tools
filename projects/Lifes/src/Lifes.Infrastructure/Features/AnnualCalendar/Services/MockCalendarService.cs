using Lifes.Core.Interfaces;
using Lifes.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lifes.Infrastructure.Features.AnnualCalendar.Services;

public class MockCalendarService : ICalendarService
{
    public Task<IEnumerable<CalendarEventModel>> GetAnnualEventsAsync(int year)
    {
        var events = new List<CalendarEventModel>
        {
            // Note: Colors/Tags based on the user's explicit mockup labels
            new CalendarEventModel { Title = "Annual Plan", StartDate = new DateTime(year, 2, 16), EndDate = new DateTime(year, 2, 17), Category = "Planning" },
            
            new CalendarEventModel { Title = "ST1-T...", StartDate = new DateTime(year, 3, 2), EndDate = new DateTime(year, 3, 3), Category = "Learning" },
            new CalendarEventModel { Title = "CVS-...", StartDate = new DateTime(year, 3, 4), EndDate = new DateTime(year, 3, 5), Category = "Learning" },
            new CalendarEventModel { Title = "ST1-T...", StartDate = new DateTime(year, 3, 6), EndDate = new DateTime(year, 3, 7), Category = "Learning" },
            new CalendarEventModel { Title = "RC-12345", StartDate = new DateTime(year, 3, 9), EndDate = new DateTime(year, 3, 11), Category = "Release" },
            new CalendarEventModel { Title = "di au...", StartDate = new DateTime(year, 3, 9), EndDate = new DateTime(year, 3, 10), Category = "Competition" },
            new CalendarEventModel { Title = "ST1-T...", StartDate = new DateTime(year, 3, 11), EndDate = new DateTime(year, 3, 12), Category = "Learning" },

            new CalendarEventModel { Title = "RC-6", StartDate = new DateTime(year, 3, 9), EndDate = new DateTime(year, 3, 9), Category = "Release" },
            new CalendarEventModel { Title = "RC-7", StartDate = new DateTime(year, 3, 10), EndDate = new DateTime(year, 3, 11), Category = "Release" },
            new CalendarEventModel { Title = "Suy ngẫm sau Q1", StartDate = new DateTime(year, 3, 20), EndDate = new DateTime(year, 3, 28), Category = "Review" },
            new CalendarEventModel { Title = "Hàn cướp", StartDate = new DateTime(year, 3, 24), EndDate = new DateTime(year, 3, 29), Category = "Travel" },
            
            new CalendarEventModel { Title = "Health Check", StartDate = new DateTime(year, 4, 3), EndDate = new DateTime(year, 4, 4), Category = "Health" },
            new CalendarEventModel { Title = "Project Alpha Review", StartDate = new DateTime(year, 4, 6), EndDate = new DateTime(year, 4, 8), Category = "Work" },
            new CalendarEventModel 
            { 
                Title = "Product Launch Cycle", 
                StartDate = new DateTime(year, 4, 10), 
                EndDate = new DateTime(year, 4, 28), 
                Category = "Work",
                Phases = new List<CalendarEventPhaseModel>
                {
                    new CalendarEventPhaseModel { Title = "Preparation", StartDate = new DateTime(year, 4, 10), EndDate = new DateTime(year, 4, 14), Category = "Work" },
                    new CalendarEventPhaseModel { Title = "Main Event", StartDate = new DateTime(year, 4, 18), EndDate = new DateTime(year, 4, 22), Category = "Work" },
                    new CalendarEventPhaseModel { Title = "Wrap-up", StartDate = new DateTime(year, 4, 25), EndDate = new DateTime(year, 4, 28), Category = "Work" }
                }
            },
            new CalendarEventModel { Title = "Spring Conference", StartDate = new DateTime(year, 4, 13), EndDate = new DateTime(year, 4, 17), Category = "Conference" },
            new CalendarEventModel { Title = "Family Weekend", StartDate = new DateTime(year, 4, 18), EndDate = new DateTime(year, 4, 19), Category = "Personal" },
            new CalendarEventModel { Title = "Security Audit", StartDate = new DateTime(year, 4, 21), EndDate = new DateTime(year, 4, 23), Category = "Work" },
            new CalendarEventModel { Title = "New Tech Research", StartDate = new DateTime(year, 4, 25), EndDate = new DateTime(year, 4, 30), Category = "Learning" },
            
            new CalendarEventModel { Title = "Hackathon", StartDate = new DateTime(year, 5, 8), EndDate = new DateTime(year, 5, 14), Category = "Work" },
            
            new CalendarEventModel { Title = "Data Release", StartDate = new DateTime(year, 6, 1), EndDate = new DateTime(year, 6, 2), Category = "Release" },
            
            new CalendarEventModel { Title = "Mid-Year Review", StartDate = new DateTime(year, 7, 1), EndDate = new DateTime(year, 7, 4), Category = "Review" },
            
            new CalendarEventModel { Title = "Summer Vacation", StartDate = new DateTime(year, 8, 8), EndDate = new DateTime(year, 8, 28), Category = "Travel" },
            
            new CalendarEventModel { Title = "Product Launch Prep", StartDate = new DateTime(year, 9, 1), EndDate = new DateTime(year, 9, 5), Category = "Work" },
            new CalendarEventModel { Title = "Award Ceremony", StartDate = new DateTime(year, 9, 19), EndDate = new DateTime(year, 9, 21), Category = "Event" },
            
            new CalendarEventModel { Title = "Skill Development Program", StartDate = new DateTime(year, 10, 1), EndDate = new DateTime(year, 11, 14), Category = "Learning" },
            
            new CalendarEventModel { Title = "Product Launch", StartDate = new DateTime(year, 11, 18), EndDate = new DateTime(year, 11, 20), Category = "Release" },
            new CalendarEventModel { Title = "Holiday Travel", StartDate = new DateTime(year, 11, 19), EndDate = new DateTime(year, 11, 28), Category = "Travel" },
            
            new CalendarEventModel { Title = "Year End Review", StartDate = new DateTime(year, 12, 5), EndDate = new DateTime(year, 12, 17), Category = "Review" },
            new CalendarEventModel { Title = "Personal Reflection", StartDate = new DateTime(year, 12, 22), EndDate = new DateTime(year, 12, 29), Category = "Personal" },
        };

        return Task.FromResult(events.AsEnumerable());
    }

    public async Task<IEnumerable<CalendarEventModel>> GetMonthlyEventsAsync(int year, int month)
    {
        var allEvents = await GetAnnualEventsAsync(year);
        var monthStart = new DateTime(year, month, 1);
        int daysInMonth = DateTime.DaysInMonth(year, month);
        var monthEnd = new DateTime(year, month, daysInMonth);

        return allEvents.Where(e => e.StartDate.Date <= monthEnd && e.EndDate.Date >= monthStart);
    }
}
