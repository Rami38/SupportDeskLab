using System;
using System.Collections.Generic;
using System.Linq;
using static SupportDeskLab.Utility;

namespace SupportDeskLab
{
    class Program
    {
        static int NextTicketId = 1;

        //Create Customer Dictionary
        static Dictionary<string, Customer> Customers = new Dictionary<string, Customer>();
        //create Ticket Queue
        static Queue<Ticket> Tickets = new Queue<Ticket>();
        //Create UndoEvent stack
        static Stack<UndoEvent> UndoEvents = new Stack<UndoEvent>();

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
            //uncomments these 3 lines after you create the Customer Dictionary
            Customers["C001"] = new Customer("C001", "Ava Martin", "ava@example.com");
            Customers["C002"] = new Customer("C002", "Ben Parker", "ben@example.com");
            Customers["C003"] = new Customer("C003", "Chloe Diaz", "chloe@example.com");
        }

        static void AddCustomer()
        {
            //look at the Demo captuerd image and add your code here
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

            Customer c = new Customer(id, name, email);
            Customers[id] = c;
            UndoEvents.Push(new UndoAddCustomer(c));

            Console.WriteLine("Customer added successfully!");
        }

        static void FindCustomer()
        {
            //look at the Demo captuerd image and add your code here
            Console.Write("Enter Customer ID: ");
            string id = Console.ReadLine();

            if (Customers.TryGetValue(id, out Customer c))
                Console.WriteLine($"Found: {c}");
            else
                Console.WriteLine("Customer not found.");
        }

        static void CreateTicket()
        {
            //look at the Demo captuerd image and add your code here
            Console.Write("Enter Customer ID: ");
            string id = Console.ReadLine();

            if (!Customers.ContainsKey(id))
            {
                Console.WriteLine("Customer not found.");
                return;
            }

            Console.Write("Enter Issue Description: ");
            string subject = Console.ReadLine();

            Ticket t = new Ticket(NextTicketId++, id, subject);
            Tickets.Enqueue(t);
            UndoEvents.Push(new UndoCreateTicket(t));

            Console.WriteLine("Ticket created successfully!");
        }

        static void ServeNext()
        {
            //look at the Demo captuerd image and add your code here
            if (Tickets.Count == 0)
            {
                Console.WriteLine("No tickets in queue.");
                return;
            }

            Ticket served = Tickets.Dequeue();
            UndoEvents.Push(new UndoServeTicket(served));
            Console.WriteLine($"Serving Ticket #{served.TicketId} for Customer {served.CustomerId}");
        }

        static void ListCustomers()
        {
            Console.WriteLine("-- Customers --");
            //look at the Demo captuerd image and add your code here
            foreach (var c in Customers.Values)
                Console.WriteLine(c);
        }

        static void ListTickets()
        {
            Console.WriteLine("-- Tickets (front to back) --");
            //look at the Demo captuerd image and add your code here
            foreach (var t in Tickets)
                Console.WriteLine(t);
        }

        static void Undo()
        {
            //look at the Demo captuerd image and add your code here
            if (UndoEvents.Count == 0)
            {
                Console.WriteLine("Nothing to undo.");
                return;
            }

            UndoEvent last = UndoEvents.Pop();

            switch (last)
            {
                case UndoAddCustomer uac:
                    Customers.Remove(uac.Customer.CustomerId);
                    Console.WriteLine($"Undo: removed customer {uac.Customer.CustomerId}");
                    break;

                case UndoCreateTicket uct:
                    var newQueue = new Queue<Ticket>();
                    foreach (var t in Tickets)
                        if (t.TicketId != uct.Ticket.TicketId)
                            newQueue.Enqueue(t);
                    Tickets = newQueue;
                    Console.WriteLine($"Undo: removed ticket #{uct.Ticket.TicketId}");
                    break;

                case UndoServeTicket ust:
                    Tickets = new Queue<Ticket>(new[] { ust.Ticket }.Concat(Tickets));
                    Console.WriteLine($"Undo: re-added served ticket #{ust.Ticket.TicketId}");
                    break;
            }
        }
    }
}
