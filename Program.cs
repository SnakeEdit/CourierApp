using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CourierApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //В рамках проекта все файлы продублированы и изменяются внутри .../bin/Debug
            string inputFilePath = "orders.csv"; // Путь к файлу с заказами
            string outputFilePath = "filtered_orders.csv"; // Путь к файлу с результатами
            string logFilePath = "application.log"; // Путь к файлу с логами

            // Запрос ввода от пользователя
            Console.Write("Введите название района доставки: ");
            string cityDistrict = Console.ReadLine();

            Console.Write("Введите время первой доставки (yyyy-MM-dd HH:mm:ss): ");
            DateTime firstDeliveryDateTime;
            while (!DateTime.TryParse(Console.ReadLine(), out firstDeliveryDateTime))
            {
                Console.Write("Неверный формат времени. Пожалуйста, введите время в формате yyyy-MM-dd HH:mm:ss: ");
            }
            LogMessage("Application started", logFilePath);

            // Чтение данных из файла
            var orders = ReadOrders(inputFilePath);
            if (orders.Count == 0)
            {
                LogError("No orders found.");
                return;
            }

            // Фильтрация заказов
            var filteredOrders = FilterOrders(orders, cityDistrict, firstDeliveryDateTime);
            LogMessage($"Filtered {filteredOrders.Count} orders", logFilePath);

            // Запись результатов
            WriteFilteredOrdersToFile(filteredOrders, outputFilePath);
            LogMessage("Application finished successfully", logFilePath);
        }

        //Метод чтения заказов из файла orders.csv
        public static List<Order> ReadOrders(string filePath)
        {
            var orders = new List<Order>();
            try
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length != 4) continue;

                    var order = new Order
                    {
                        OrderId = parts[0].Trim(),
                        Weight = parts[1].Trim(),
                        Area = parts[2].Trim(),
                        DeliveryTime = DateTime.Parse(parts[3].Trim())
                    };
                    orders.Add(order);
                }
            }
            catch (Exception ex)
            {
                LogError("Error reading orders from file: " + ex.Message);
            }
            return orders;
        }
        //Метод фильтрации поиска заказов
        public static List<Order> FilterOrders(List<Order> orders, string cityDistrict, DateTime firstDeliveryDateTime)
        {
            return orders
                .Where(o => o.Area.Equals(cityDistrict, StringComparison.OrdinalIgnoreCase))
                .Where(o => o.DeliveryTime >= firstDeliveryDateTime && o.DeliveryTime <= firstDeliveryDateTime.AddMinutes(30))
                .ToList();
        }

        //Метод записи в файл отфильтрованных заказов
        public static void WriteFilteredOrdersToFile(List<Order> orders, string outputFilePath)
        {
            try
            {
                var writer = new StreamWriter(outputFilePath);
                foreach (var order in orders)
                {
                    writer.WriteLine($"{order.OrderId},{order.Weight},{order.Area},{order.DeliveryTime:yyyy-MM-dd HH:mm:ss}");
                }
            }
            catch (Exception ex)
            {
                LogError("Error writing filtered orders to file: " + ex.Message);
            }
        }

        //Метод для логгирования
        public static void LogMessage(string message, string logFilePath)
        {
            try
            {
                File.AppendAllText(logFilePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}");
            }
            catch (Exception)
            {
                Console.WriteLine(message);
            }
        }

        public static void LogError(string error)
        {
            Console.WriteLine($"Error: {error}");
        }
    }
}
