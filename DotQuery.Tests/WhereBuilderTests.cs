// ReSharper disable NotAccessedPositionalProperty.Local
// ReSharper disable ClassNeverInstantiated.Local
namespace DotQuery.Tests;

[TestClass]
public partial class WhereBuilderTests
{
    [TestMethod]
    public async Task Should_return_parameterized_where_statement()
    {
        var sql = DotQuery.From<Order>().Where(o => o.Name == "TV").Build();
        await Verify(sql.Format);
    }

    [TestMethod]
    public async Task Should_return_where_statement()
    {
        var sql = DotQuery.From<Order>().Where(o => o.Name == "TV").Build().ToRawString();
        await Verify(sql);
    }

    [TestMethod]
    public async Task Should_return_where_statement_with_not_equal_operator()
    {
        var sql = DotQuery.From<Order>().Where(o => o.Name != "TV").Build();
        await Verify(sql.Format);
    }

    [TestMethod]
    public async Task Should_return_where_statement_with_greater_than_operator()
    {
        var sql = DotQuery.From<Order>().Where(o => o.Price > 1000).Build();
        await Verify(sql.Format);
    }

    [TestMethod]
    public async Task Should_return_where_statement_with_greater_than_or_equal_operator()
    {
        var sql = DotQuery.From<Order>().Where(o => o.Price >= 1000).Build();
        await Verify(sql.Format);
    }

    [TestMethod]
    public async Task Should_return_where_statement_with_less_than_operator()
    {
        var sql = DotQuery.From<Order>().Where(o => o.Price < 1000).Build();
        await Verify(sql.Format);
    }

    [TestMethod]
    public async Task Should_return_where_statement_with_less_than_or_equal_operator()
    {
        var sql = DotQuery.From<Order>().Where(o => o.Price <= 1000).Build();
        await Verify(sql.Format);
    }

    [TestMethod]
    public async Task Should_return_where_statement_with_and_operator()
    {
        var sql = DotQuery.From<Order>().Where(o => o.Name == "TV" && o.Price > 1000).Build();
        await Verify(sql.Format);
    }

    [TestMethod]
    public async Task Should_return_where_statement_with_or_operator()
    {
        var sql = DotQuery.From<Order>().Where(o => o.Name == "TV" || o.Price > 1000).Build();
        await Verify(sql.Format);
    }

    [TestMethod]
    public async Task Should_return_where_statement_with_multiple_separate_logical_operators()
    {
        var sql = DotQuery.From<Order>().Where(o => o.Name == "TV" && o.Price > 1000 || o.Price < 9000).Build();
        await Verify(sql.Format);
    }

    [TestMethod]
    public async Task Should_return_where_statement_with_multiple_separate_logical_operators_with_not()
    {
        var sql = DotQuery.From<Order>().Where(o => o.Name == "TV" && !(o.Price > 1000) || o.Price < 9000).Build();
        await Verify(sql.Format);
    }

    [TestMethod]
    public async Task Should_return_where_statement_with_multiple_grouped_logical_operators()
    {
        var sql = DotQuery.From<Order>().Where(o => o.Name == "TV" && (o.Price > 1000 || o.Price < 9000)).Build();
        await Verify(sql.Format);
    }

    [TestMethod]
    public async Task Should_return_where_statement_with_multiple_grouped_logical_operators_with_not()
    {
        var sql = DotQuery.From<Order>().Where(o => o.Name == "TV" && !(o.Price > 1000 || o.Price < 9000)).Build();
        await Verify(sql.Format);
    }

    [TestMethod]
    public async Task Should_return_where_statement_with_multiple_where_clauses()
    {
        var sql = DotQuery.From<Order>()
            .Where(o => o.Name == "TV")
            .Where(o => o.Price > 1000 || o.Price < 9000)
            .Build();
        await Verify(sql.Format);
    }

    [TestMethod]
    public async Task Should_return_where_statement_with_in_statement()
    {
        var sql = DotQuery.From<Order>().Where(o => o.Name.In("TV", "Radio")).Build();
        await Verify(sql.Format);
    }

    [TestMethod]
    public async Task Should_return_where_statement_with_not_in_statement()
    {
        var sql = DotQuery.From<Order>().Where(o => !o.Name.In("TV", "Radio")).Build();
        await Verify(sql.Format);
    }

    [TestMethod]
    public async Task Should_return_parameterized_where_statement_with_in_statement_with_enumerable_value()
    {
        var inValues = new[] { "TV", "Radio" };
        var sql = DotQuery.From<Order>().Where(o => o.Name.In(inValues)).Build();
        await Verify(sql.Format);
    }

    [TestMethod]
    public async Task Should_return_where_statement_with_in_statement_with_enumerable_value()
    {
        var inValues = new[] { "TV", "Radio" };
        var sql = DotQuery.From<Order>().Where(o => o.Name.In(inValues)).Build();
        await Verify(sql.ToRawString());
    }


    private record Order(int Id, string Name, int Price);
}