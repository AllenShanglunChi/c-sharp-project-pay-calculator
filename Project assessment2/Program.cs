using System;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;


public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

public class Login
{
    public string UserName { get; set; }
    public string PasswordHash { get; set; }
}

public class Employee : Person
{
    public int EmployeeID { get; set; }
    public string UserName { get; set; }
    public string PasswordHash { get; set; }
    public int TaxNumber { get; set; }
    public bool TaxThreshold { get; set; }
}

public class Manager : Employee
{
    public bool Permissions { get; set; }
}

public class PaySlip : Employee
{
    private object calculator;

    public int WeekNumber { get; set; }
    public double WeekHours { get; set; }
    public string SubmittedBy { get; set; }
    public DateTime SubmittedDate { get; set; }
    public string ApprovedBy { get; set; }
    public DateTime ApprovedDate { get; set; }
    public double PayGrossCalculated { get; set; }
    public double TaxCalculated { get; set; }
    public double SuperCalculated { get; set; }
    public double PayNetCalculated { get; set; }

    public PaySlip(int employeeID, string firstName, string lastName, int taxNumber, bool taxThreshold, int weekNumber, double weekHours, string submittedBy, DateTime submittedDate, string approvedBy, DateTime approvedDate)
    {
        EmployeeID = employeeID;
        FirstName = firstName;
        LastName = lastName;
        TaxNumber = taxNumber;
        TaxThreshold = taxThreshold;
        WeekNumber = weekNumber;
        WeekHours = weekHours;
        SubmittedBy = submittedBy;
        SubmittedDate = submittedDate;
        ApprovedBy = approvedBy;
        ApprovedDate = approvedDate;
    }

    public void CalculatePay(PayCalculatorBase calculator)
    {
        PayGrossCalculated = calculator.CalculateGrossPay(WeekHours);
        TaxCalculated = calculator.CalculateTax(PayGrossCalculated);
        SuperCalculated = calculator.CalculateSuperannuation(PayGrossCalculated);
        PayNetCalculated = PayGrossCalculated - TaxCalculated - SuperCalculated;
    }

    public void DisplayPaySlip()
    {
        Console.WriteLine("");
        Console.WriteLine($"Employee Details: ID {EmployeeID}, {FirstName} {LastName}");
        Console.WriteLine($"Hours Worked: {WeekHours}");
        Console.WriteLine($"Hourly Rate: {PayCalculatorBase.HourlyRate:C2}");
        Console.WriteLine($"Tax Threshold: {(TaxThreshold ? "Claimed" : "Not Claimed")}");
        Console.WriteLine($"Gross Pay: {PayGrossCalculated:C2}");
        Console.WriteLine($"Tax: {TaxCalculated:C2}");
        Console.WriteLine($"Net Pay: {PayNetCalculated:C2}");
        Console.WriteLine($"Superannuation: {SuperCalculated:C2}");
    }

}

public abstract class PayCalculatorBase
{
    public static double HourlyRate { get; set; }
    public double CalculateGrossPay(double hours)
    {
        return HourlyRate * hours;
    }
    public double CalculateSuperannuation(double grossPay)
    {
        return grossPay * 0.11;
    }
    public abstract double CalculateTax(double grossPay);
}

public class PayCalculatorNoThreshold : PayCalculatorBase
{
    public override double CalculateTax(double grossPay)
    {
        double tax;
        double x = grossPay + 0.99;
        if (grossPay < 88)
        {
            tax = x * 0.1900 - 0.1900;
        }
        else if (grossPay < 371)
        {
            tax = x * 0.2348 - 3.9639;
        }
        else if (grossPay < 515)
        {
            tax = x * 0.2190 - (-1.9003);
        }
        else if (grossPay < 932)
        {
            tax = x * 0.3477 - 64.4297;
        }
        else if (grossPay < 1957)
        {
            tax = x * 0.3450 - 61.9132;
        }
        else if (grossPay < 3111)
        {
            tax = x * 0.3900 - 150.0093;
        }
        else
        {
            tax = x * 0.4700 - 398.9324;
        }
        return tax;
    }
}

public class PayCalculatorWithThreshold : PayCalculatorBase
{
    public override double CalculateTax(double grossPay)
    {
        double tax;
        double x = grossPay + 0.99;
        if (grossPay < 359)
        {
            tax = 0.0; 
        }
        else if (grossPay < 438)
        {
            tax = x * 0.1900 - 68.3462;
        }
        else if (grossPay < 548)
        {
            tax = x * 0.2900 - 112.1942;
        }
        else if (grossPay < 721)
        {
            tax = x * 0.2100 - 68.3465;
        }
        else if (grossPay < 865)
        {
            tax = x * 0.2190 - 74.8369;
        }
        else if (grossPay < 1282)
        {
            tax = x * 0.3477 - 186.2119;
        }
        else if (grossPay < 2307)
        {
            tax = x * 0.3450 - 182.7504;
        }
        else if (grossPay < 3461)
        {
            tax = x * 0.3900 - 286.5965;
        }
        else
        {
            tax = x * 0.4700 - 563.5196;
        }
        return tax;
    }
}

/// <summary>
/// Add a class to use the selected reusable component - CsvHelper to write payslip data to a csv file
/// </summary>

public class CsvWriterService
{
    public void WriteToCsv(PaySlip paySlip)
    {
        var fileName = $"Pay-EmployeeID-{paySlip.EmployeeID}-Fullname-{paySlip.FirstName}-{paySlip.LastName}-{DateTime.Now:yyyyMMddHHmmss}.csv";

        using (var writer = new StreamWriter(fileName))
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            csv.WriteRecords(new[] {paySlip});
        }

        Console.WriteLine($"Payslip data has been written to {fileName}");
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Welcome to the weekly payment calculaotr.Please enter employee details to calculate payment:");

/// <summary>
/// populate the PaySlip from payroll officer input
/// </summary>
        Console.Write("Employee ID: ");
        int employeeID = int.Parse(Console.ReadLine());

        Console.Write("First Name: ");
        string firstName = Console.ReadLine();

        Console.Write("Last Name: ");
        string lastName = Console.ReadLine();

        Console.Write("Tax Number: ");
        int taxNumber = int.Parse(Console.ReadLine());

        Console.Write("Tax Threshold (true/false): ");
        bool taxThreshold = bool.Parse(Console.ReadLine());

        Console.Write("Week Number: ");
        int weekNumber = int.Parse(Console.ReadLine());

        Console.Write("Week Hours: ");
        double weekHours = double.Parse(Console.ReadLine());

        Console.Write("Submitted By: ");
        string submittedBy = Console.ReadLine();

        Console.Write("Submitted Date: ");
        DateTime submittedDate = DateTime.Parse(Console.ReadLine());

        Console.Write("Approved By: ");
        string approvedBy = Console.ReadLine();

        Console.Write("Approved Date: ");
        DateTime approvedDate = DateTime.Parse(Console.ReadLine());

/// <summary>
/// Create a PaySlip with user input using constructor
/// </summary>
        var paySlip = new PaySlip(employeeID, firstName, lastName, taxNumber, taxThreshold, weekNumber, weekHours, submittedBy, submittedDate, approvedBy, approvedDate);

/// <summary>
/// Prompt for the hourly rate
/// </summary>
        Console.Write("Hourly Rate: ");
        PayCalculatorBase.HourlyRate = double.Parse(Console.ReadLine());

/// <summary>
/// Calculate pay
/// </summary>
        PayCalculatorBase calculator;
        if (paySlip.TaxThreshold)
        {
            calculator = new PayCalculatorWithThreshold();
        }
        else
        {
            calculator = new PayCalculatorNoThreshold();
        }

        paySlip.CalculatePay(calculator);

/// <summary>
/// Display the pay slip
/// </summary> 
        paySlip.DisplayPaySlip();

/// <summary>
/// Use CsvHelper to output data to a CSV file
/// </summary> 
        var csvWriterService = new CsvWriterService();
        csvWriterService.WriteToCsv(paySlip); 
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              