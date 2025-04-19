namespace DotQuery.Tests;

[TestClass]
public partial class SelectBuilderTests
{
    [TestMethod]
    public async Task Should_return_parameterized_select_statement()
    {
        var sql = DotQuery.From<Order>().Where(o => o.Name == "TV").Select(o => o.Price).Build();
        await Verify(sql.Format);
    }

    [TestMethod]
    public async Task Should_return_select_statement()
    {
        var sql = DotQuery.From<Order>().Where(o => o.Name == "TV").Select(o => o.Price).Build().ToRawString();
        await Verify(sql);
    }

    private record Order(int Id, string Name, int Price);
}