namespace SimplePayrollApp.Models
{
    public class PayrollData
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeID { get; set; } = string.Empty;
        public double BasicSalary { get; set; }
        public double Allowances { get; set; }
        public double Bonus { get; set; }
        public double Overtime { get; set; }
        public double GrossSalary { get; set; }
        public double SSF { get; set; }
        public double PAYE { get; set; }
        public double NetSalary { get; set; }
        public DateTime PayPeriod { get; set; } = DateTime.Now;
    }
}
