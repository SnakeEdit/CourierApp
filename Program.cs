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
            string filePath = "orders.csv"; //Указатель на файл содержащий список заказов (внутри bin/debug)
            List<Order> orders = ReadOrders(filePath); //Список заказов

            string command = "";

            Console.WriteLine("Для завершения работы - введите quit; Для поиска заказа - введите order");

            while (!command.ToLower().Contains("quit"))
            {
                Console.Write("> ");
                command = Console.ReadLine();

                if (command.ToLower() == "order")
                {
                    Console.WriteLine("Введите район для фильтрации:");
                    string area = Console.ReadLine();

                    Console.WriteLine("Введите начальную дату периода для заказов (yyyy-MM-dd HH:mm:ss):");
                    DateTime startTime = DateTime.Parse(Console.ReadLine());

                    Console.WriteLine("Введите конечную дату периода заказов (yyyy-MM-dd HH:mm:ss):");
                    DateTime endTime = DateTime.Parse(Console.ReadLine());

                    Console.WriteLine("Введите минимальное количество заказов:");
                    int minOrderCount = int.Parse(Console.ReadLine());

                    var filteredOrders = FilterOrders(orders, area, startTime, endTime, minOrderCount);

                    if (filteredOrders.Count > 0)
                    {
                        Console.WriteLine("Отфильтрованные заказы:");
                        foreach (var order in filteredOrders)
                        {
                            Console.WriteLine($"Номер заказа: {order.OrderId}, Вес: {order.Weight}, Район: {order.Area}, Время доставки: {order.DeliveryTime}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Нет заказов удовлетворяющих запрос");
                    }
                }
            }
        }

        //Метод чтения заказов из файла orders.csv
        static List<Order> ReadOrders(string filePath) 
        {
            var orders = new List<Order>();

            try
            {
                var lines = File.ReadAllLines(filePath);

                foreach (var line in lines)
                {
                    var parts = line.Split(',');

                    // Пропускаем строку заголовка или некорректные строки
                    if (parts[0] == "OrderId" || parts.Length < 4)
                        continue;

                    try
                    {
                        orders.Add(new Order
                        {
                            OrderId = parts[0],
                            Weight = parts[1],
                            Area = parts[2],
                            DeliveryTime = DateTime.ParseExact(parts[3], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                        }) ;
                    }
                    catch (FormatException fe)
                    {
                        Console.WriteLine($"Ошибка формата в строке: {line}. {fe.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
            }
            return orders;
        }
        //Метод фильтрации поиска заказов
        static List<Order> FilterOrders(List<Order> orders, string area, DateTime startTime, DateTime endTime, int minOrderCount)
        {
            var filteredOrders = orders
                .Where(o => o.Area == area && o.DeliveryTime >= startTime && o.DeliveryTime <= endTime)
                .ToList();

            if (filteredOrders.Count >= minOrderCount)
            {
                return filteredOrders;
            }
            // Возвращаем пустой список, если не выполняется условие по количеству заказов
            return new List<Order>();
        }
    }
}
