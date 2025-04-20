// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable NotAccessedPositionalProperty.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local
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

    [TestMethod]
    public async Task Should_return_select_statement_with_anonymous_type_with_multiple_properties_without_aliases()
    {
        var sql = DotQuery.From<Order>().Select(o => new { o.Name, o.Price }).Build();
        await Verify(sql.Format);
    }

    [TestMethod]
    public async Task Should_return_select_statement_with_anonymous_type_with_multiple_properties_with_aliases()
    {
        var sql = DotQuery.From<Order>().Select(o => new { ProductName = o.Name, ProductPrice = o.Price }).Build();
        await Verify(sql.Format);
    }

    [TestMethod]
    public async Task Should_return_select_statement_with_record_type_with_multiple_properties_with_aliases()
    {
        var sql = DotQuery.From<Order>().Select(o => new OrderProductRecord(o.Name, o.Price)).Build();
        await Verify(sql.Format);
    }

    private record Order(int Id, string Name, int Price);
    private record OrderProductRecord(string ProductName, int ProductPrice);
}