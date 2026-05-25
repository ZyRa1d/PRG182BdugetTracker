using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG182BudgeTracker
{
    internal class Program
    {
        static List<Transaction> listA = new List<Transaction>();
        static Dictionary<string, decimal> dictB = new Dictionary<string, decimal>();
        static decimal totalInc = 0;
        static decimal totalExp = 0;
        static decimal limitBud = 0;
        static int nextId = 1;

        
        static void Main() // Entry point — displays main menu and routes user input to the appropriate method
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== BUDGET TRACKER =====");
                Console.WriteLine("1. Add Transaction");
                Console.WriteLine("2. View All Transactions");
                Console.WriteLine("3. Generate Summary");
                Console.WriteLine("4. Category Breakdown");
                Console.WriteLine("5. Set Budget Limit");
                Console.WriteLine("6. Split Expense");
                Console.WriteLine("7. Filter Transactions");
                Console.WriteLine("8. Delete Transaction");
                Console.WriteLine("9. Exit");
                Console.Write("Choose option: ");
                string opt = Console.ReadLine();

                switch (opt)
                {
                    case "1": AddTrans(); break;
                    case "2": ViewAll(); break;
                    case "3": ShowSum(); break;
                    case "4": ShowCat(); break;
                    case "5": SetLimit(); break;
                    case "6": SplitExp(); break;
                    case "7": Filter(); break;
                    case "8": Delete(); break;
                    case "9":
                        Console.Write("Are you sure? (Y/N): ");
                        if (Console.ReadLine().ToUpper() == "Y") return;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Press Enter.");
                        Console.ReadLine();
                        break;
                }
                UpdateTotals();
            }
        }

        
        static decimal ExtractAmount(string input) // Strips non-numeric characters from input and returns a positive decimal amount, or -1 if invalid
        {
            string cleaned = "";
            foreach (char c in input)
            {
                if (char.IsDigit(c) || c == '.' || c == '-')
                    cleaned += c;
            }
            if (decimal.TryParse(cleaned, out decimal result) && result > 0)
                return result;
            return -1;
        }

        
        static void AddTrans() // Prompts the user for transaction details and adds a new Transaction to the list
        {
            decimal amt = 0;
            while (true)
            {
                Console.Write("Enter amount (e.g., 100, R50, $75.50): ");
                string raw = Console.ReadLine();
                amt = ExtractAmount(raw);
                if (amt > 0) break;
                Console.WriteLine("Invalid input. Enter a positive number (like 100, R50, $75.50).");
            }

            int typ = 0;
            while (true)
            {
                Console.Write("Type (1=Income, 2=Expense): ");
                if (int.TryParse(Console.ReadLine(), out typ) && (typ == 1 || typ == 2)) break;
                Console.WriteLine("Enter 1 or 2.");
            }

            Console.WriteLine("Categories: 1=Rent 2=Groceries 3=Utilities 4=Entertainment 5=Transport 6=Other");
            int catNum = 0;
            while (true)
            {
                Console.Write("Category (1-6): ");
                if (int.TryParse(Console.ReadLine(), out catNum) && catNum >= 1 && catNum <= 6) break;
                Console.WriteLine("Enter 1 to 6.");
            }
            string cat = "";
            switch (catNum)
            {
                case 1: cat = "Rent"; break;
                case 2: cat = "Groceries"; break;
                case 3: cat = "Utilities"; break;
                case 4: cat = "Entertainment"; break;
                case 5: cat = "Transport"; break;
                case 6: cat = "Other"; break;
            }

            Console.Write("Description: ");
            string desc = Console.ReadLine();

            DateTime d = DateTime.Now;
            Console.Write("Date (yyyy-mm-dd, press Enter for today): ");
            string dInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(dInput))
                DateTime.TryParse(dInput, out d);

            Transaction t = new Transaction();
            t.Id = nextId++;
            t.Amount = amt;
            t.Type = (typ == 1) ? "Income" : "Expense";
            t.Category = cat;
            t.Description = desc;
            t.Date = d;
            listA.Add(t);
            Console.WriteLine("Transaction added successfully! Press Enter.");
            Console.ReadLine();
        }

        
        static void ViewAll() // Displays all transactions stored in the list
        {
            Console.Clear();
            if (listA.Count == 0)
            {
                Console.WriteLine("No transactions yet.");
                Console.ReadLine();
                return;
            }
            foreach (var t in listA)
            {
                Console.WriteLine($"ID:{t.Id} | {t.Date:yyyy-MM-dd} | {t.Type} | {t.Category} | {t.Description} | {t.Amount:C}");
            }
            Console.ReadLine();
        }

       
        static void ShowSum()  // Shows total income, total expenses, net balance, and budget limit status
        {
            Console.Clear();
            Console.WriteLine($"Total Income:  {totalInc:C}");
            Console.WriteLine($"Total Expenses:{totalExp:C}");
            Console.WriteLine($"Net Balance:   {(totalInc - totalExp):C}");
            if (limitBud > 0 && totalExp > limitBud)
                Console.WriteLine($"WARNING: Budget exceeded! Spent {totalExp:C} of {limitBud:C} limit.");
            else if (limitBud > 0)
                Console.WriteLine($"Budget status: Within limit ({totalExp:C} / {limitBud:C})");
            else
                Console.WriteLine("No budget limit set.");
            Console.ReadLine();
        }

        
        static void ShowCat() // Displays total expenses grouped by category
        {
            Console.Clear();
            if (dictB.Count == 0)
            {
                Console.WriteLine("No expense categories yet.");
                Console.ReadLine();
                return;
            }
            foreach (var kv in dictB)
                Console.WriteLine($"{kv.Key}: {kv.Value:C}");
            Console.ReadLine();
        }

        
        static void SetLimit() // Allows the user to set a monthly budget spending limit
        {
            Console.Write("Enter monthly budget limit (e.g., 5000, R5000): ");
            string raw = Console.ReadLine();
            decimal lim = ExtractAmount(raw);
            if (lim > 0)
                limitBud = lim;
            else
                Console.WriteLine("Invalid amount.");
            Console.ReadLine();
        }

        
        static void SplitExp() // Splits a shared expense evenly across a given number of people
        {
            decimal total = 0;
            while (true)
            {
                Console.Write("Enter total expense amount (e.g., 1200, R1200): ");
                string raw = Console.ReadLine();
                total = ExtractAmount(raw);
                if (total > 0) break;
                Console.WriteLine("Invalid amount.");
            }
            int people = 0;
            while (true)
            {
                Console.Write("Number of housemates: ");
                if (int.TryParse(Console.ReadLine(), out people) && people > 0) break;
                Console.WriteLine("Enter positive number.");
            }
            decimal each = total / people;
            Console.WriteLine($"Each person pays: {each:C}");
            Console.ReadLine();
        }

        
        static void Filter() // Filters and displays transactions by category or date range
        {
            Console.WriteLine("Filter by: 1=Category 2=Date range");
            string ch = Console.ReadLine();
            if (ch == "1")
            {
                Console.Write("Enter category (Rent, Groceries, Utilities, Entertainment, Transport, Other): ");
                string catF = Console.ReadLine();
                var filtered = listA.Where(t => t.Category.Equals(catF, StringComparison.OrdinalIgnoreCase)).ToList();
                if (filtered.Count == 0) Console.WriteLine("No transactions.");
                else foreach (var t in filtered) Console.WriteLine($"{t.Date:yyyy-MM-dd} | {t.Type} | {t.Category} | {t.Description} | {t.Amount:C}");
            }
            else if (ch == "2")
            {
                Console.Write("Start date (yyyy-mm-dd): ");
                DateTime start = DateTime.Parse(Console.ReadLine());
                Console.Write("End date (yyyy-mm-dd): ");
                DateTime end = DateTime.Parse(Console.ReadLine());
                var filtered = listA.Where(t => t.Date >= start && t.Date <= end).ToList();
                if (filtered.Count == 0) Console.WriteLine("No transactions.");
                else foreach (var t in filtered) Console.WriteLine($"{t.Date:yyyy-MM-dd} | {t.Type} | {t.Category} | {t.Description} | {t.Amount:C}");
            }
            else Console.WriteLine("Invalid choice.");
            Console.ReadLine();
        }

       
        static void Delete()  // Removes a transaction from the list by its ID
        {
            if (listA.Count == 0)
            {
                Console.WriteLine("Nothing to delete.");
                Console.ReadLine();
                return;
            }
            Console.WriteLine("Transactions:");
            foreach (var t in listA) Console.WriteLine($"ID:{t.Id} - {t.Description} - {t.Amount:C}");
            Console.Write("Enter ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int idDel))
            {
                var rem = listA.FirstOrDefault(t => t.Id == idDel);
                if (rem != null)
                {
                    listA.Remove(rem);
                    Console.WriteLine("Deleted.");
                }
                else Console.WriteLine("ID not found.");
            }
            else Console.WriteLine("Invalid ID.");
            UpdateTotals();
            Console.ReadLine();
        }

        
        static void UpdateTotals() // Recalculates total income, total expenses, and the category breakdown dictionary
        {
            totalInc = 0;
            totalExp = 0;
            dictB.Clear();
            foreach (var t in listA)
            {
                if (t.Type == "Income") totalInc += t.Amount;
                else totalExp += t.Amount;

                if (t.Type == "Expense")
                {
                    if (dictB.ContainsKey(t.Category))
                        dictB[t.Category] += t.Amount;
                    else
                        dictB[t.Category] = t.Amount;
                }
            }
        }
    }

    class Transaction
    {
        public int Id;
        public decimal Amount;
        public string Type;
        public string Category;
        public string Description;
        public DateTime Date;
    }
}
