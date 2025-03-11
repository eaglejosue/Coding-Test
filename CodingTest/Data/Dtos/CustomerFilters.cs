﻿namespace CodingTest.Data.Dtos;

public sealed class CustomerFilters
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public int? Age { get; set; }

    public string CacheKey() => string.Concat("CustomerFiltersKey: Id:", Id, " Name:", Name, " Age:", Age);
}
