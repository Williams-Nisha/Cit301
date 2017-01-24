using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace MegaEscritorio
{
    class Order
    {
        static string readString(string prompt) {
            string result;
            do { Console.Write(prompt);
                result = Console.ReadLine();
            }
            while (result == "");
            return result;
        } static int readInt(string prompt, int low, int high) {
            int result;
            do { string intString = readString(prompt);
                result = int.Parse(intString);
            } while ((result < low) || (result > high));
            return result;
        }
        static int getDeskDimensions()
        {
            int length = Order.readInt("Enter the length in inches you want your desk to have? Choose a number between 24 and 72: ", 24, 72);
            int width = Order.readInt("Enter the width in inches you want your desk to have? Choose a number between 24 and 48: ", 24, 48);
            Console.WriteLine("Desk size will be: " + length + " inches x " + width + " inches");
            int surfaceArea = length * width;
            Console.WriteLine("That is a total surfaceArea of " + surfaceArea + " inches");
            return surfaceArea;
        }

        static int calcSurfacePrice(int surface)
        {

            int price;
            if (surface > 1000)
            {
                price = (surface - 1000) * 5;
            } else
            {
                price = 0;
            }
            return price;
        }
        static int getDrawerCost(int surfacePrice)
        {
            Console.WriteLine("The desk price is: $200 \n The upgrade fee for surface area is: $" + surfacePrice);
            int drawers = Order.readInt("The upgrade cost for drawers are $50 each. \n How many drawers will your desk have? Choose a number between 0-7: ", 0, 7);
            int drawerCost = drawers * 50;
            Console.WriteLine("The upgrade fee for " + drawers + " drawers is: " + drawerCost);
            return drawerCost;
        }
        static int getMaterialCost()
        {
            int costs = 0;
            string[] materials = new string[3] { "oak", "laminate", "pine" };
            int[] materialCosts = new int[3] { 200, 100, 50 };

            string materialChoice = Order.readString("Wood choices are: Oak: $200, Laminate: $100, or Pine: $50.\n Choose oak, laminate or pine: ");
            Console.WriteLine("You chose: " + materialChoice);
            for (var i = 0; i < materials.Length; i++)
            {
                if (materials[i] == materialChoice)
                {
                    costs = materialCosts[i];
                    Console.WriteLine("The costs for " + materialChoice + "is " + costs);
                    return costs;
                } else
                {
                    Console.WriteLine("Choice is not available. Please choose oak, laminate or pine: ");
                }
            }
            return costs;
        }

        static int totalDeskPriceBeforeShipping(int baseDeskPrice, int surfacePrice, int drawerPrice, int materialPrice)
        {
            return baseDeskPrice + surfacePrice + drawerPrice + materialPrice;
        }

        static int calcShippingCosts(int surfaceArea)
        {
            int shippingAmount = 0;
            int[,] rushOrderArray = new int[3, 3];
            try {
                //read allthelines from the given text file into a single string array
                //string[] rushPrices = File.ReadAllLines(@"/rushOrder.txt");
                int shippingChoice =Order.readInt("Do you want 3 day, 5 day or 7 day shipping?\n Enter 0- 3 , 1- 5, 2-7:", 0,2);
                string[] rushPrices = File.ReadAllLines("rushOrder.txt");

                for (int i = 0; i < rushPrices.Length; i++)
                {
                   rushOrderArray[i/3, i%3] = int.Parse(rushPrices[i]);
                }
               
                if (surfaceArea < 1000)
                {
                    shippingAmount = rushOrderArray[shippingChoice, 0];
                }
                else if (surfaceArea >= 1000 && surfaceArea < 1999)
                {
                    shippingAmount = rushOrderArray[shippingChoice, 1];
                }
                else if (surfaceArea >= 2000)
                {
                    shippingAmount = rushOrderArray[shippingChoice, 2];
                }
               

                Console.WriteLine("Your shipping costs are $" + shippingAmount);

            }
            catch (Exception e) {
                Console.Write(e.Message);
            }
            return shippingAmount;
        }
        static int totalPrice(int price, int shipping){
            Console.WriteLine("The total costs for your custom desk is: $" + (price + shipping));
            return price + shipping;
        }
      
        static void saveOrderToTextFile(int baseDeskPrice, int drawerPrice, int surfacePrice, int rushOrderPrice, int totalPrice)
        {
            string json = JsonConvert.SerializeObject(new
            {

                baseDesk = baseDeskPrice,
                drawers = drawerPrice,
                surfaceArea = surfacePrice,
                rushOrder = rushOrderPrice,
                totalPrice = totalPrice
            });
            //write string to file
            File.AppendAllText("orders.txt", json);
         
        } 
            
        public static void Main()
        {
            Console.WriteLine("Welcome to our custom desk ordering system!\n Let's start with your desk dimensions.");
            int surfaceArea = getDeskDimensions();
            int surfacePrice = calcSurfacePrice(surfaceArea);
            int drawerPrice = getDrawerCost(surfacePrice);
            int materialPrice = getMaterialCost();
            const int baseDeskPrice = 200;

            int pricebeforeShipping = totalDeskPriceBeforeShipping(baseDeskPrice, surfacePrice, drawerPrice, materialPrice);
            Console.WriteLine("Price before shipping is: " + pricebeforeShipping);
            int totalShippingCosts = calcShippingCosts(surfaceArea);
            int finalPriceQuote = totalPrice(pricebeforeShipping, totalShippingCosts);
            saveOrderToTextFile(baseDeskPrice, drawerPrice, surfacePrice, totalShippingCosts, finalPriceQuote);
        
        }

    }

}
