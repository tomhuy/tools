namespace Lifes.Core.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;
using Lifes.Core.Models;

public interface IDocumentService
{
    Task<IEnumerable<DocumentModel>> GetDocumentsAsync(int year, int month);
}
