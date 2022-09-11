namespace MortCalc.Core
{
    public static class Calculator
    {
        /// <summary>
        /// The percentage at which the ASP subsidy kicks in. Currently 3.8%.
        /// </summary>
        private const decimal AspInterestSoftCap = .038m;

        /// <summary>
        /// For any interest that exceeds the soft cap, this is the percentage of it that will be paid by the subsidy. Currently 70%.
        /// </summary>
        private const decimal AspSubsidyPercentage = 0.7m;

        public record MonthlyPayment(
            decimal AmountPerMonth,
            decimal PricipalPortion,
            decimal InterestPortion,
            decimal AspSavings
        );

        public static MonthlyPayment CalculateMonthlyPayment(
            decimal annualInterestRate,
            uint numPayments,
            decimal principal,
            bool useAspSubsidy
        )
        {
            decimal monthly = annualInterestRate / 12;
            decimal monthlyPaymentNoInterest = principal / numPayments;

            if (monthly == 0)
            {
                return new MonthlyPayment(monthlyPaymentNoInterest, monthlyPaymentNoInterest, 0, 0);
            }

            decimal onePlusRToTheN = (decimal)Math.Pow(1 + (double)monthly, numPayments);

            if (useAspSubsidy && annualInterestRate > AspInterestSoftCap)
            {
                decimal monthlySoftCapInterest = AspInterestSoftCap / 12;
                decimal onePlusRToTheNSoftCap = (decimal)
                    Math.Pow(1 + (double)monthlySoftCapInterest, numPayments);

                decimal monthlyPaymentAtSoftCap =
                    principal
                    * (
                        (monthlySoftCapInterest * onePlusRToTheNSoftCap)
                        / (onePlusRToTheNSoftCap - 1)
                    );

                decimal monthlyPaymentWithNoSubidy =
                    principal * ((monthly * onePlusRToTheN) / (onePlusRToTheN - 1));

                decimal preSoftCapInterestAmount =
                    monthlyPaymentAtSoftCap - monthlyPaymentNoInterest;
                decimal interestAmountWithNoSubsidy =
                    monthlyPaymentWithNoSubidy - monthlyPaymentNoInterest;

                decimal amountCoveredBySubsidy =
                    interestAmountWithNoSubsidy - preSoftCapInterestAmount;
                decimal monthlyPaymentWithSubsidy =
                    monthlyPaymentAtSoftCap + (amountCoveredBySubsidy * (1 - AspSubsidyPercentage));

                return new MonthlyPayment(
                    monthlyPaymentWithSubsidy,
                    monthlyPaymentNoInterest,
                    (monthlyPaymentWithSubsidy - monthlyPaymentNoInterest),
                    (amountCoveredBySubsidy * AspSubsidyPercentage)
                );
            }

            decimal amountPerMonth =
                principal * ((monthly * onePlusRToTheN) / (onePlusRToTheN - 1));

            return new MonthlyPayment(
                amountPerMonth,
                monthlyPaymentNoInterest,
                amountPerMonth - monthlyPaymentNoInterest,
                0
            );
        }
    }
}
