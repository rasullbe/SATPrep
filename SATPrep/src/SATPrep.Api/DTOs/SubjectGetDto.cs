using System.Collections.Generic;

namespace SATPrep.Api.DTOs;

public record SubjectGetDto(long SubjectId, string Name, IEnumerable<TopicGetDto> Topics);
