using System;
using System.Collections.Generic;
using System.IO;

namespace PayrollSystem
{
    // Base class for employees
    public class BaseEmployee
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public string Role { get; set; }
        public decimal BasicPay { get; set; }
        public decimal Allowances { get; set; }

        public BaseEmployee(string name, int id, string role, decimal basicPay, decimal allowances)
        {
            Name = name;
            ID = id;
            Role = role;
            BasicPay = basicPay;
            Allowances = allowances;
        }

        public virtual decimal CalculateSalary()
        {
            decimal deductions = CalculateDeductions();
            return BasicPay + Allowances - deductions;
        }

        protected virtual decimal CalculateDeductions()
        {
            return BasicPay * 0.1m; // Default deduction is 10% of basic pay
        }

        public override string ToString()
        {
            return $"ID: {ID}, Name: {Name}, Role: {Role}, Salary: {CalculateSalary():C}";
        }
    }

    // Manager class
    public class Manager : BaseEmployee
    {
        public decimal Bonus { get; set; }

        public Manager(string name, int id, decimal basicPay, decimal allowances, decimal bonus)
            : base(name, id, "Manager", basicPay, allowances)
        {
            Bonus = bonus;
        }

        public override decimal CalculateSalary()
        {
            return base.CalculateSalary() + Bonus;
        }
    }

    // Developer class
    public class Developer : BaseEmployee
    {
        public Developer(string name, int id, decimal basicPay, decimal allowances)
            : base(name, id, "Developer", basicPay, allowances)
        {
        }
    }

    // Intern class
    public class Intern : BaseEmployee
    {
        public Intern(string name, int id, decimal basicPay, decimal allowances)
            : base(name, id, "Intern", basicPay, allowances)
        {
        }

        protected override decimal CalculateDeductions()
        {
            return 0; // Interns have no deductions
        }
    }

    // Payroll system application
    class Program
    {
        private static List<BaseEmployee> employees = new List<BaseEmployee>();
        private const string FilePath = "employees.txt";

        static void Main(string[] args)
        {
            LoadData();
            while (true)
            {
                Console.WriteLine("\nPayroll System Menu:");
                Console.WriteLine("1. Add New Employee");
                Console.WriteLine("2. Display All Employees");
                Console.WriteLine("3. Calculate Individual Salary");
                Console.WriteLine("4. Display Total Payroll");
                Console.WriteLine("5. Save and Exit");

                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddEmployee();
                        break;
                    case "2":
                        DisplayEmployees();
                        break;
                    case "3":
                        CalculateSalary();
                        break;
                    case "4":
                        DisplayTotalPayroll();
                        break;
                    case "5":
                        SaveData();
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        static void AddEmployee()
        {
            Console.Write("Enter Name: ");
            string name = Console.ReadLine();

            Console.Write("Enter ID: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Enter Role (Manager/Developer/Intern): ");
            string role = Console.ReadLine();

            Console.Write("Enter Basic Pay: ");
            decimal basicPay = decimal.Parse(Console.ReadLine());

            Console.Write("Enter Allowances: ");
            decimal allowances = decimal.Parse(Console.ReadLine());

            BaseEmployee employee;

            if (role.Equals("Manager", StringComparison.OrdinalIgnoreCase))
            {
                Console.Write("Enter Bonus: ");
                decimal bonus = decimal.Parse(Console.ReadLine());
                employee = new Manager(name, id, basicPay, allowances, bonus);
            }
            else if (role.Equals("Developer", StringComparison.OrdinalIgnoreCase))
            {
                employee = new Developer(name, id, basicPay, allowances);
            }
            else if (role.Equals("Intern", StringComparison.OrdinalIgnoreCase))
            {
                employee = new Intern(name, id, basicPay, allowances);
            }
            else
            {
                Console.WriteLine("Invalid role. Employee not added.");
                return;
            }

            employees.Add(employee);
            Console.WriteLine("Employee added successfully.");
        }

        static void DisplayEmployees()
        {
            if (employees.Count == 0)
            {
                Console.WriteLine("No employees found.");
                return;
            }

            foreach (var employee in employees)
            {
                Console.WriteLine(employee);
            }
        }

        static void CalculateSalary()
        {
            Console.Write("Enter Employee ID: ");
            int id = int.Parse(Console.ReadLine());

            var employee = employees.Find(e => e.ID == id);
            if (employee != null)
            {
                Console.WriteLine($"Salary for {employee.Name}: {employee.CalculateSalary():C}");
            }
            else
            {
                Console.WriteLine("Employee not found.");
            }
        }

        static void DisplayTotalPayroll()
        {
            decimal totalPayroll = 0;
            foreach (var employee in employees)
            {
                totalPayroll += employee.CalculateSalary();
            }
            Console.WriteLine($"Total Payroll: {totalPayroll:C}");
        }

        static void SaveData()
        {
            using (StreamWriter writer = new StreamWriter(FilePath))
            {
                foreach (var employee in employees)
                {
                    writer.WriteLine($"{employee.ID}|{employee.Name}|{employee.Role}|{employee.BasicPay}|{employee.Allowances}|{(employee is Manager ? ((Manager)employee).Bonus : 0)}");
                }
            }
            Console.WriteLine("Data saved successfully.");
        }

        static void LoadData()
        {
            if (!File.Exists(FilePath)) return;

            using (StreamReader reader = new StreamReader(FilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split('|');
                    int id = int.Parse(parts[0]);
                    string name = parts[1];
                    string role = parts[2];
                    decimal basicPay = decimal.Parse(parts[3]);
                    decimal allowances = decimal.Parse(parts[4]);
                    decimal bonus = decimal.Parse(parts[5]);

                    BaseEmployee employee;
                    if (role == "Manager")
                    {
                        employee = new Manager(name, id, basicPay, allowances, bonus);
                    }
                    else if (role == "Developer")
                    {
                        employee = new Developer(name, id, basicPay, allowances);
                    }
                    else // Intern
                    {
                        employee = new Intern(name, id, basicPay, allowances);
                    }

                    employees.Add(employee);
                }
            }
        }
    }
}
