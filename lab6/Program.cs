using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

public class Store
{
    public string Name { get; set; }
    public string Address { get; set; }
    public List<string> Phones { get; set; } = new List<string>();
    public string Specialization { get; set; }
    public string WorkingHours { get; set; }

    public void AddPhone(string phone)
    {
        Phones.Add(phone);
    }

    public bool WorksEverydayWithoutBreak()
    {
        return WorkingHours.Equals("24/7", StringComparison.OrdinalIgnoreCase);
    }

    public bool HasShortPhoneNumber()
    {
        return Phones.Any(phone => phone.Length < 5);
    }

    public bool HasUkrainianMobileNumber()
    {
        return Phones.Any(phone => phone.StartsWith("380"));
    }

    public override string ToString()
    {
        return $"Store(Name='{Name}', Address='{Address}', Phones={string.Join(", ", Phones)}, Specialization='{Specialization}', WorkingHours='{WorkingHours}')";
    }
}

public class MyLinkedList<T> : List<T>
{
    public void RemoveIf(Predicate<T> predicate)
    {
        this.RemoveAll(predicate);
    }
}

class Program
{
    private static MyLinkedList<Store> stores = new MyLinkedList<Store>();
    private const string FileName = "stores.json";

    static void Main(string[] args)
    {
        LoadStores();

        if (args.Length > 0 && args[0].Equals("-auto", StringComparison.OrdinalIgnoreCase))
        {
            stores.Add(new Store { Name = "AutoStore", Address = "AutoAddress", Specialization = "AutoSpecialization", WorkingHours = "24/7" });
            DisplayStores();
            SaveStores();
            Console.WriteLine("Program terminated.");
            return;
        }

        while (true)
        {
            Console.WriteLine("Menu:");
            Console.WriteLine("1. Add a new store");
            Console.WriteLine("2. View list of stores");
            Console.WriteLine("3. Delete a store by name");
            Console.WriteLine("4. Search stores by keyword");
            Console.WriteLine("5. Sort by Name");
            Console.WriteLine("6. Sort by City in Address");
            Console.WriteLine("7. Sort by Specialization");
            Console.WriteLine("8. Find specific stores");
            Console.WriteLine("9. Exit program");
            Console.Write("Select an option: ");

            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    AddStore();
                    break;

                case 2:
                    DisplayStores();
                    break;

                case 3:
                    Console.Write("Enter the store name to delete: ");
                    var storeNameToDelete = Console.ReadLine();
                    stores.RemoveIf(s => s.Name.Equals(storeNameToDelete, StringComparison.OrdinalIgnoreCase));
                    Console.WriteLine($"Store with the name {storeNameToDelete} deleted (if it was found).");
                    break;

                case 4:
                    SearchStoresByKeyword();
                    break;

                case 5:
                    SortStoresByName();
                    Console.WriteLine("Stores sorted by name:");
                    DisplayStores();
                    break;

                case 6:
                    SortStoresByCityInAddress();
                    Console.WriteLine("Stores sorted by city in address:");
                    DisplayStores();
                    break;

                case 7:
                    SortStoresBySpecialization();
                    Console.WriteLine("Stores sorted by specialization:");
                    DisplayStores();
                    break;

                case 8:
                    DisplaySpecificStores();
                    break;

                case 9:
                    SaveStores();
                    Console.WriteLine("Program terminated.");
                    return;

                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        }
    }

    private static void LoadStores()
    {
        if (File.Exists(FileName))
        {
            try
            {
                var json = File.ReadAllText(FileName);
                stores = JsonSerializer.Deserialize<MyLinkedList<Store>>(json);
                Console.WriteLine("Data loaded successfully from stores.json");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading data from stores.json: {e.Message}. Starting with an empty list.");
            }
        }
        else
        {
            Console.WriteLine("stores.json not found. Starting with an empty list.");
        }
    }

    private static void AddStore()
    {
        Console.WriteLine("Enter store details:");

        Console.Write("Name: ");
        var name = Console.ReadLine();

        Console.Write("Address: ");
        var address = Console.ReadLine();

        Console.Write("Specialization: ");
        var specialization = Console.ReadLine();

        Console.Write("Working Hours: ");
        var workingHours = Console.ReadLine();

        var store = new Store
        {
            Name = name,
            Address = address,
            Specialization = specialization,
            WorkingHours = workingHours
        };

        while (true)
        {
            Console.Write("Add phone (Y/N): ");
            if (Console.ReadLine().Equals("Y", StringComparison.OrdinalIgnoreCase))
            {
                Console.Write("Phone number: ");
                store.AddPhone(Console.ReadLine());
            }
            else
            {
                break;
            }
        }

        stores.Add(store);
        Console.WriteLine("Store added.");
    }

    private static void DisplayStores()
    {
        foreach (var store in stores)
        {
            Console.WriteLine(store);
        }
    }

    private static void SaveStores()
    {
        var json = JsonSerializer.Serialize(stores, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FileName, json);
    }

    private static void SearchStoresByKeyword()
    {
        Console.Write("Enter keyword to search: ");
        var keyword = Console.ReadLine().ToLower();

        var filteredStores = stores.Where(store => store.Name.ToLower().Contains(keyword)
                                                    || store.Address.ToLower().Contains(keyword)
                                                    || store.Specialization.ToLower().Contains(keyword));

        foreach (var store in filteredStores)
        {
            Console.WriteLine(store);
        }
    }

    private static void DisplaySpecificStores()
    {
        foreach (Store store in stores)
        {
            if (store.WorksEverydayWithoutBreak() && store.HasShortPhoneNumber() && store.HasUkrainianMobileNumber())
            {
                Console.WriteLine(store);
            }
        }
    }

    private static void SortStoresByName()
    {
        stores.Sort((s1, s2) => s1.Name.CompareTo(s2.Name));
    }

    private static void SortStoresByCityInAddress()
    {
        stores.Sort((s1, s2) => GetCityFromAddress(s1.Address).CompareTo(GetCityFromAddress(s2.Address)));
    }

    private static void SortStoresBySpecialization()
    {
        stores.Sort((s1, s2) => s1.Specialization.CompareTo(s2.Specialization));
    }

    private static string GetCityFromAddress(string address)
    {
        
        return address.Split(' ')[0];
    }
}
