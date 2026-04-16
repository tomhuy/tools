namespace Lifes.Infrastructure.Features.DocumentManagement.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lifes.Core.Models;
using Lifes.Core.Interfaces;

public class MockDocumentService : IDocumentService
{
    public Task<IEnumerable<DocumentModel>> GetDocumentsAsync(int year, int month)
    {
        var nextDate = (int day) => new DateTime(year, month, Math.Min(day, DateTime.DaysInMonth(year, month)));
        
        var list = new List<DocumentModel>
        {
            new DocumentModel { Id = "1", Title = "Custom Text field columns - Data loader does not return an error", StartDate = nextDate(17), EndDate = nextDate(17), Status = "Active" },
            new DocumentModel { Id = "2", Title = "Generic Behavior: Support processing the same behavior code across multiple labels", StartDate = nextDate(17), EndDate = nextDate(17), Status = "Active" },
            new DocumentModel { Id = "3", Title = "Mckeesport, PA (8060) - Subjects in Grades File Do not match ones in our DB", StartDate = nextDate(16), EndDate = nextDate(16), Status = "Active" },
            new DocumentModel { Id = "4", Title = "New issue with updated grades integration", StartDate = nextDate(15), EndDate = nextDate(15), Status = "Active" },
            new DocumentModel { Id = "5", Title = "IXL Level-Up Processed Files Not Storing in BackupData FTP Folder", StartDate = nextDate(20), EndDate = nextDate(20), Status = "Active" },
            new DocumentModel { Id = "6", Title = "Add validation to Data Loader", StartDate = nextDate(17), EndDate = nextDate(17), Status = "Active" },
            new DocumentModel { Id = "7", Title = "i-Ready Early Literacy and Dyslexia Risk Screener Automated Scheduled Import", StartDate = nextDate(17), EndDate = nextDate(17), Status = "Active" },
            new DocumentModel { Id = "8", Title = "Transfer", StartDate = nextDate(22), EndDate = nextDate(29), Status = "Active" },
            new DocumentModel { Id = "9", Title = "Summarize & Visualize Integration Services Overview", StartDate = nextDate(22), EndDate = nextDate(29), Status = "Active" },
            new DocumentModel { Id = "10", Title = "Studentloader CICD, Staffloader CICD, Automation CICD, OneRoster Integration CICD", StartDate = nextDate(20), EndDate = nextDate(20), Status = "Active" },
            new DocumentModel { Id = "11", Title = "Merge class", StartDate = nextDate(17), EndDate = nextDate(17), Status = "Active" },
            
            // Sub tasks
            new DocumentModel { Id = "101", ParentId = "11", Title = "cảm xúc A", StartDate = nextDate(16), EndDate = nextDate(16) },
            new DocumentModel { Id = "102", ParentId = "11", Title = "cảm xúc B", StartDate = nextDate(17), EndDate = nextDate(17) },
            
            // Habits / continuous
            new DocumentModel { Id = "12", Title = "đọc sách", StartDate = nextDate(1), EndDate = nextDate(DateTime.DaysInMonth(year, month)), Status = "Active" },
            new DocumentModel { Id = "13", Title = "ghi nhật ký", StartDate = nextDate(1), EndDate = nextDate(DateTime.DaysInMonth(year, month)), Status = "Active" },
            new DocumentModel { Id = "14", Title = "tập thể dục", StartDate = nextDate(1), EndDate = nextDate(DateTime.DaysInMonth(year, month)), Status = "Active" }
        };

        return Task.FromResult(list.AsEnumerable());
    }
}
