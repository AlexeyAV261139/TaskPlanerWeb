namespace Logic.Stores
{
    public record MyTaskFilter(
        IEnumerable<Guid> Ids,
        IEnumerable<string> Names,
        IEnumerable<DateTime> Dates);   
}
