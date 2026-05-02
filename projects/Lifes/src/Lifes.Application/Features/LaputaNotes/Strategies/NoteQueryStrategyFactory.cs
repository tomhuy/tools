using System;
using System.Collections.Generic;
using Lifes.Core.Interfaces;

namespace Lifes.Application.Features.LaputaNotes.Strategies;

public class NoteQueryStrategyFactory
{
    private readonly INoteRepository _repository;
    private readonly Dictionary<string, INoteQueryStrategy> _strategies;

    public NoteQueryStrategyFactory(INoteRepository repository)
    {
        _repository = repository;
        _strategies = new Dictionary<string, INoteQueryStrategy>(StringComparer.OrdinalIgnoreCase)
        {
            { "inbox", new InboxQueryStrategy(_repository) },
            { "all", new AllNotesQueryStrategy(_repository) },
            { "category", new CategoryQueryStrategy(_repository) }
        };
    }

    public INoteQueryStrategy GetStrategy(string? queryType)
    {
        if (string.IsNullOrEmpty(queryType) || !_strategies.TryGetValue(queryType, out var strategy))
        {
            return new NullQueryStrategy(_repository);
        }
        return strategy;
    }
}
