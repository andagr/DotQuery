namespace DotQuery;

public class Class1
{
    public void Test()
    {
        DotQuery.From<Order>()
            .Where(x => x.Name == "John")
            .Select(x => new { x.Id, x.Name })
            .Build();
    }
}

public record Order(int Id, string Name);