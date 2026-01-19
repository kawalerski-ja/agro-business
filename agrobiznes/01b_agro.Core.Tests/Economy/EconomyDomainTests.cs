using _01_agro.Core.Economy;

namespace _01b_agro.Core.Tests;

[TestClass]
public class MoneyTests
{
    [TestMethod]
    public void Money_Should_Set_Currency_From_Constructor()
    {
        var m = new Money(10m, "EUR");
        Assert.AreEqual("EUR", m.Currency);
    }

    [TestMethod]
    public void Money_Should_Default_To_PLN()
    {
        var m = new Money(10m);
        Assert.AreEqual("PLN", m.Currency);
    }

    [TestMethod]
    public void Money_Should_Throw_When_Amount_Is_Negative()
    {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Money(-1m, "PLN"));
    }
}
