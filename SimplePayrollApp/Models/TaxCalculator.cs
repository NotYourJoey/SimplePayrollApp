namespace SimplePayrollApp.Models
{
    public static class TaxCalculator
    {
        public const double SSF_RATE = 0.055; // 5.5%

        public static double CalculateSSF(double basicSalary)
        {
            return basicSalary * SSF_RATE;
        }

        public static double CalculatePAYE(double grossSalary)
        {
            // Implement progressive tax brackets
            return grossSalary > 3000 ? grossSalary * 0.175 : grossSalary * 0.1;
        }

        public static PayrollData CalculatePayroll(string name, string id, double basicSalary,
            double allowances, double bonus, double overtime)
        {
            double grossSalary = basicSalary + allowances + bonus + overtime;
            double ssf = CalculateSSF(basicSalary);
            double paye = CalculatePAYE(grossSalary);
            double netSalary = grossSalary - ssf - paye;

            return new PayrollData
            {
                EmployeeName = name,
                EmployeeID = id,
                BasicSalary = basicSalary,
                Allowances = allowances,
                Bonus = bonus,
                Overtime = overtime,
                GrossSalary = grossSalary,
                SSF = ssf,
                PAYE = paye,
                NetSalary = netSalary,
                PayPeriod = DateTime.Now
            };
        }
    }
}
