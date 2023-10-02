﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

public class Item
{
    public string Title { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class Order
{
    public string ShipToName { get; set; }
    public string ShipToStreet { get; set; }
    public string ShipToAddress { get; set; }
    public string ShipToCountry { get; set; }
    public List<Item> Items { get; set; }
}

public static class XmlToOrderConverter
{
    public static Order Convert(string xml)
    {
        try
        {
            XElement root = XElement.Parse(xml);
            Order order = new Order
            {
                ShipToName = root.Element("shipTo")?.Element("name")?.Value,
                ShipToStreet = root.Element("shipTo")?.Element("street")?.Value,
                ShipToAddress = root.Element("shipTo")?.Element("address")?.Value,
                ShipToCountry = root.Element("shipTo")?.Element("country")?.Value,
                Items = new List<Item>()
            };

            foreach (var itemElement in root.Element("items")?.Elements("item") ?? Enumerable.Empty<XElement>())
            {
                Item item = new Item
                {
                    Title = itemElement.Element("title")?.Value,
                    Quantity = int.Parse(itemElement.Element("quantity")?.Value ?? "0"),
                    Price = decimal.Parse(itemElement.Element("price")?.Value ?? "0.0", CultureInfo.InvariantCulture)
                };

                order.Items.Add(item);
            }

            return order;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка під час конвертації XML в об'єкт Order: {ex.Message}");
            return null;
        }
    }
}

class Program
{
    static void Main()
    {
        string xmlFilePath = "D:\\XMLFile1.xml";

        try
        {
            string xml = File.ReadAllText(xmlFilePath);

            Order order = XmlToOrderConverter.Convert(xml);

            if (order != null)
            {
                Console.WriteLine($"Ім'я отримувача: {order.ShipToName}");
                Console.WriteLine($"Адреса отримувача: {order.ShipToAddress}, {order.ShipToStreet}, {order.ShipToCountry}");
                Console.WriteLine("Список товарів:");
                foreach (var item in order.Items)
                {
                    Console.WriteLine($"Назва товару: {item.Title}");
                    Console.WriteLine($"Кількість: {item.Quantity}");
                    Console.WriteLine($"Ціна: {item.Price:C}");
                    Console.WriteLine();
                }
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Файл не знайдено.");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Помилка читання файлу: {ex.Message}");
        }
    }
}
