using Microsoft.VisualStudio.TestTools.UnitTesting;


[TestClass]
public class PayCalculatorTests
{
    [TestMethod]
    public void TestCalculateTaxWithThreshold()
    {
        var calculator = new PayCalculatorWithThreshold();
        double grossPay = 1000; // Replace with your test values
        double expectedTax = calculator.CalculateTax(grossPay);
        double actualTax = 179.63; // Replace with the expected tax for your test case
        Assert.AreEqual(expectedTax, actualTax, 0.01); // Adjust the delta value as needed
    }

    [TestMethod]
    public void TestCalculateTaxWithoutThreshold()
    {
        var calculator = new PayCalculatorNoThreshold();
        double grossPay = 1000; // Replace with your test values
        double expectedTax = calculator.CalculateTax(grossPay);
        double actualTax = 228.55; // Replace with the expected tax for your test case
        Assert.AreEqual(expectedTax, actualTax, 0.01); // Adjust the delta value as needed
    }
}
