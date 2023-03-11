using System.CommandLine;
using static MortCalc.Core.Calculator;

namespace MortCalc.Tool;

internal class Program
{
    private static void Main(string[] args)
    {
        var interestRateOption = new Option<decimal>(
            aliases: new[] { "--interest", "-i" },
            getDefaultValue: () => 0.5m,
            description: "Your mortgage's interest annual rate, as a percentage from 0.0 to 100.0."
        );

        var numPaymentsOption = new Option<uint>(
            aliases: new[] { "--payments", "-p" },
            getDefaultValue: () => 240,
            description: "The number of total payments in your mortgage. Usually the number of years, divided by 12 (i.e. total number of months)."
        );

        var loanPrincipalOption = new Option<decimal>(
            aliases: new[] { "--amount", "-a" },
            getDefaultValue: () => 161_000m,
            description: "Your total ammount borrowed in your loan, also known as its principal."
        );

        var aspSubsidyOption = new Option<bool>(
            aliases: new[] { "--subsidy", "-s" },
            getDefaultValue: () => true,
            description: "Whether or not the ASP subsidy should be applied to interest payments."
        );

        var rootCommand = new RootCommand(
            "Calculate your monthly mortgage payment. With optional Finnish ASP subsidy calculations!"
        );

        rootCommand.AddOption(interestRateOption);
        rootCommand.AddOption(numPaymentsOption);
        rootCommand.AddOption(loanPrincipalOption);
        rootCommand.AddOption(aspSubsidyOption);

        rootCommand.SetHandler(
            PrintMortgage,
            interestRateOption,
            numPaymentsOption,
            loanPrincipalOption,
            aspSubsidyOption
        );

        rootCommand.Invoke(args);
    }

    private static void PrintMortgage(
        decimal interestRate,
        uint numPayments,
        decimal loanPrincipal,
        bool useSubsidy
    )
    {
        var monthlyPayment = CalculateMonthlyPayment(
            interestRate / 100m,
            numPayments,
            loanPrincipal,
            useSubsidy
        );

        string amtPerMonth = $"€{monthlyPayment.AmountPerMonth:0.00}";
        string principal = $"€{monthlyPayment.PricipalPortion:0.00}";
        string interest = $"€{monthlyPayment.InterestPortion:0.00}";
        string aspSavings = $"€{monthlyPayment.AspSavings:0.00}";

        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine();
        Console.WriteLine(
            $"MortCalc: Calculating monthly payments...\nMortage: €{loanPrincipal:N2}, Interest: {interestRate:N2}%, Number of Payments: {numPayments}, ASP Subsidy: {useSubsidy}"
        );
        Console.WriteLine("***");
        Console.WriteLine(
            $"{"Monthly Payment", 15}{"Principal", 15}{"Interest", 15}{"ASP Savings", 15}"
        );
        Console.WriteLine("------------------------------------------------------------");
        Console.WriteLine(
            $"{amtPerMonth, 15}" + $"{principal, 15}" + $"{interest, 15}" + $"{aspSavings, 15}"
        );
        Console.WriteLine();
    }
}
