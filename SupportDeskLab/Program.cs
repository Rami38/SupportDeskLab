using System;
using System.Collections.Generic;
using static SupportDeskLab.Utility;


namespace SupportDeskLab
{
   
     
    class Program
    {
        static int NextTicketId = 1;

        //Create Customer Dictionary
        //create Ticket Queue
        //Create UndoEvent stack
        static Dictionary<string, Customer> Customers = new Dictionary<string, Customer>();
        static Queue<Ticket> Tickets = new Queue<Ticket>();
        static Stack<string> UndoEvents = new Stack<string>();

        static void Main()
        {
            initCustomer();

            while (true)
            {
                Console.WriteLine("\n=== Support Desk ===");
                Console.WriteLine("[1] Add customer");
                Console.WriteLine("[2] Find customer");
                Console.WriteLine("[3] Create ticket");
                Console.WriteLine("[4] Serve next ticket");
                Console.WriteLine("[5] List customers");
                Console.WriteLine("[6] List tickets");
                Console.WriteLine("[7] Undo last action");
                Console.WriteLine("[0] Exit");
                Console.Write("Choose: ");
                string choice = Console.ReadLine();

                //create switch cases and then call a reletive method 
                //for example for case 1 you need to have a method named addCustomer(); or case 2 add a method name findCustomer


                switch (choice)
                {
                    case "1": AddCustomer(); break;
                    case "2": FindCustomer(); break;
                    case "3": CreateTicket(); break;
                    case "4": ServeNext(); break;
                    case "5": ListCustomers(); break;
                    case "6": ListTickets(); break;
                    case "7": Undo(); break;
                    case "0": return;
                    default: Console.WriteLine("Invalid option."); break;
                }
            }
        }
        /*
         * Do not touch initCustomer method. this is like a seed to have default customers.
         */
        static void initCustomer()
        {
           
            Customers["C001"] = new Customer("C001", "Ava Martin", "ava@example.com");
            Customers["C002"] = new Customer("C002", "Ben Parker", "ben@example.com");
            Customers["C003"] = new Customer("C003", "Chloe Diaz", "chloe@example.com");
        }

        static void AddCustomer()
        {
            Console.Write("Enter Customer ID: ");
            string id = Console.ReadLine();
            Console.Write("Enter Name: ");
            string name = Console.ReadLine();
            Console.Write("Enter Email: ");
            string email = Console.ReadLine();

            if (Customers.ContainsKey(id))
            {
                Console.WriteLine("Customer already exists.");
                return;
            }

            Customers[id] = new Customer(id, name, email);
            UndoEvents.Push($"REMOVE_CUSTOMER:{id}");
            Console.WriteLine("Customer added successfully!");

        }

        static void FindCustomer()
        {
            Console.Write("Enter Customer ID: ");
            string id = Console.ReadLine();

            if (Customers.TryGetValue(id, out Customer c))
            {
                Console.WriteLine($"Found: {c.CustomerId} | {c.Name} | {c.Email}");
            }
            else
            {
                Console.WriteLine("Customer not found.");
            }

        }

        static void CreateTicket()
        {
            Console.Write("Enter Customer ID: ");
            string id = Console.ReadLine();

            if (!Customers.ContainsKey(id))
            {
                Console.WriteLine("Customer not found.");
                return;
            }

            Console.Write("Enter Issue Description: ");
            string issue = Console.ReadLine();

            Ticket t = new Ticket(NextTicketId++, id, issue);
            Tickets.Enqueue(t);
            UndoEvents.Push("REMOVE_TICKET");
            Console.WriteLine("Ticket created successfully!");

        }

        static void ServeNext()
        {
            if (Tickets.Count == 0)
            {
                Console.WriteLine("No tickets in queue.");
                return;
            }

            Ticket served = Tickets.Dequeue();
            Console.WriteLine($"Serving Ticket #{served.TicketId} for Customer {served.CustomerId}");
            UndoEvents.Push($"ADD_TICKET:{served.TicketId}:{served.CustomerId}:{served.Issue}");

        }

        static void ListCustomers()
        {
            Console.WriteLine("-- Customers --");

            foreach (var c in Customers.Values)
            {
                Console.WriteLine($"{c.CustomerId} | {c.Name} | {c.Email}");
            }

        }

        static void ListTickets()
        {
           
            Console.WriteLine("-- Tickets (front to back) --");
            foreach (var t in Tickets)
            {
                Console.WriteLine($"Ticket #{t.TicketId} | Customer: {t.CustomerId} | Issue: {t.Issue}");
            }

        }

        static void Undo()
        {
            if (UndoEvents.Count == 0)
            {
                Console.WriteLine("Nothing to undo.");
                return;
            }

            string lastAction = UndoEvents.Pop();

            if (lastAction.StartsWith("REMOVE_CUSTOMER:"))
            {
                string id = lastAction.Split(':')[1];
                Customers.Remove(id);
                Console.WriteLine($"Undo: removed customer {id}");
            }
            else if (lastAction == "REMOVE_TICKET")
            {
                if (Tickets.Count > 0)
                {
                    Ticket removed = Tickets.Last(); // requires System.Linq
                    var temp = new Queue<Ticket>(Tickets.Where(t => t != removed));
                    Tickets = temp;
                    Console.WriteLine("Undo: removed last created ticket");
                }
            }
            else if (lastAction.StartsWith("ADD_TICKET:"))
            {
                var parts = lastAction.Split(':');
                int tid = int.Parse(parts[1]);
                string cid = parts[2];
                string issue = parts[3];
                Tickets.Enqueue(new Ticket(tid, cid, issue));
                Console.WriteLine("Undo: re-added last served ticket");
            }
        }

    }
}

