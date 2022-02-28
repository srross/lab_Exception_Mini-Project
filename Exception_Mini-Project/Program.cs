//This program will run if the user puts all the right input in
//However, the user is the enemy, so we need to make sure it functions with no run time errors
//There may also be LOGICAL errors in this program

//Our database consists of the store items and a list of customers who have shopped here before.
using System.Globalization;

Dictionary<string, decimal> storeItems = new Dictionary<string, decimal>()
{
    { "Widget", 1.25m },
    { "Sprocket", 2.75m },
    { "Thingamabobs", 10.45m },
    { "Sliced Bread", 5.45m },
    { "Headphones", 50.95m },
    { "TV", 500.00m }
};

List<string> currentCustomerList = new List<string>()
{
    "Harry",
    "Betsy",
    "Mergatroid",
    "Larry",
    "Joe",
    "Shannon",
    "Jonathan",
    "Cassly"
};

var isShopping = false;
bool isCustomerLoggedIn = false;

var currentCustomer = string.Empty;
bool isReturnCustomer = false;

var shoppingCart = new List<string>();
var priceList = new List<decimal>();
decimal cartTotalCost = 0;

Console.WriteLine("Welcome to FaceAzonGoogleAppleFlix! Returning customers get 10% off!"); // Seperate Welcome message and enter name prompt.
// What if: Numeric,null(Enter key),misMatched case, inputName not in list is entered?
// Error thrown on .Where(x => x == currentCustomerInput) -->  InvalidOperationException, Sequence contains no elements

while (!isCustomerLoggedIn)
{
    Console.Write("Please type in your name! ");
    var userInputName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Console.ReadLine());
    //  Accepted Enter input w/o error.
    //  Added Console.ReadLine.
    //  Used CultureInfo: to put User Name in correct format - proactive error handling

    currentCustomer = currentCustomerList.Where(x => x.ToLower() == userInputName.ToLower()).FirstOrDefault();
    // InvalidOperationException, Sequence contains no elements
    // use Where() or Contains(), and FirstOrDefault()
    // Proactive error handling -> CultureInfo.CurrentCulture.TextInfo.ToTitleCase() transforms userInputName into proper format before adding to list

    if (string.IsNullOrEmpty(currentCustomer)) // Null check on  
    {
        Console.WriteLine($"Sorry, an account for {userInputName} was not found.");
        Console.Write($"Press any key to try again --OR-- 1 to CREATE NEW customer account: ");


        var loginFailedResponse = 0;
        var IsValidNumber = int.TryParse(Console.ReadLine(), out loginFailedResponse);

        if (loginFailedResponse == 1)
        {
            currentCustomer = CreateNewCustomer(userInputName);

            Console.WriteLine();
            Console.WriteLine($"Welcome {currentCustomer}!");

            isCustomerLoggedIn = true;
            isShopping = true;
        }
    }
    else
    {
        Console.WriteLine($"Welcome back {currentCustomer}!  You're eligible for 10% off your total order today");

        isReturnCustomer = true;
        isCustomerLoggedIn = true;
        isShopping = true;
    }

    while (isCustomerLoggedIn && isShopping)
    {
        var addItem = "y";

        Console.WriteLine("Here is our list of items");
        Console.WriteLine("-------------------------");
        PrintItemList();
        Console.WriteLine();

        Console.Write("Please enter an item by name: ");
        var userIntputItem = Console.ReadLine();
        var currentItem = storeItems.Keys.Where(k => k.ToLower().Contains(userIntputItem.ToLower())).FirstOrDefault();
        // keyNotFoundException for item not in Dictionary
        // used Contains(), ToLower(), FirstOrDefault()

        if (currentItem == null)
        {
            // what to do if currentItem is null (user entry is not found))?
            Console.WriteLine($"Sorry, {userIntputItem} was not found.");
            Console.Write("Would you like to try another item (y/n)? ");
            addItem = Console.ReadLine().ToLower();

            isShopping = (addItem == "y" || addItem == "yes");
        }
        else
        {
            var tryAgain = "n";

            Console.Write($"How many {currentItem} items would you like? "); // Is a good idea to echo back item name to confirm selection?
            int qtyCurrentItem = 0;
            var isValidNumber = int.TryParse(Console.ReadLine(), out qtyCurrentItem);
            // System.FormatException: 'Input string was not in a correct format.'
            // replaced parse with TryParse
            // defined the out variable above TryParse to make it available for the out parameter.
            if (qtyCurrentItem > 0)
            {
                AddItemToCart(currentItem, qtyCurrentItem);
            }
            else
            {
                // what to do if entry is not an integer?
            }
        }

        Console.Write("Would you like to add another item (y/n)? "); // prompt user with kind of input we expect, 
        addItem = Console.ReadLine().ToLower();

        if (addItem != "y" || addItem != "yes")
        {
            CheckOut();
            // offer option to add additional items
            Console.Write("Any last minute add-on items before checking out (y/n)? ");
            addItem = Console.ReadLine().ToLower();
            isShopping = (addItem == "y" || addItem == "yes");
        }

        Console.WriteLine($"Thanks for shopping with us today! See you next time {currentCustomer}!");
        isCustomerLoggedIn = false;
    }
    break;
}



// Methods
void CheckOut()
{
    Console.WriteLine("Time to check out!");
    Console.WriteLine();

    Console.WriteLine("Here's what you bought:");
    foreach (var item in shoppingCart)
    {
        Console.WriteLine($"{item} @ {storeItems[item]}/item"); // Can duplicates be grouped?
    }

    Console.WriteLine($"Your cart total is currently: ${decimal.Round(cartTotalCost, 2)}"); // logic doesn't apply discount for returning users
}

void AddItemToCart(string currentItem, int quantity)
{
    for (int i = 0; i < quantity; i++)
    {
        shoppingCart.Add(currentItem); // never trust unvalidated userInput.  Using currentItemFound instead
        priceList.Add(storeItems[currentItem]);

        cartTotalCost = !isReturnCustomer ? cartTotalCost + storeItems[currentItem]
            : cartTotalCost + (storeItems[currentItem] - (storeItems[currentItem] * .1m));
    }

    var message = string.Empty;

    if (quantity == 1)
    {
        message = $"A quantity of {quantity} {currentItem} at a cost of ${storeItems[currentItem]} has been added to your cart!"; // verb tense
    }
    else if (!currentItem.EndsWith("s"))
    {
        message = $"A quantity of {quantity} {currentItem}s at a cost of ${storeItems[currentItem]} each has been added to your cart!"; // verb tense
    }
    else
    {
        message = $"A quantity of {quantity} {currentItem} at a cost of ${storeItems[currentItem]} each has been added to your cart!";
    }

    Console.WriteLine(message);
}

string CreateNewCustomer(string newName)
{
    var formattedNewName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(newName.ToLower()); // Could use this at time of input, except bad legacy data.  Better here to ensure data integrity moving forward.
    currentCustomerList.Add(formattedNewName);

    currentCustomer = currentCustomerList.Where(x => x == formattedNewName).FirstOrDefault(); // Make sure saved in proper format

    return currentCustomer;
}

void PrintItemList()
{
    foreach (KeyValuePair<string, decimal> kvpItem in storeItems)
    {
        Console.WriteLine(kvpItem.Key + ": $" + kvpItem.Value);
    }
}