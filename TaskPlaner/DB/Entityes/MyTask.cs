public class MyTask
{
    public Guid Id { get; set; }
    public string Heading { get; set; } = "";
    public string Content { get; set; } = "";
    public DateOnly? Date { get; set; }  
    public int? Priority { get; set; }
}

