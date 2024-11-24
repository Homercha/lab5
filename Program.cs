using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace GalleryManagement
{
    public abstract class Exhibition
    {
        public string Title { get; set; } // Назва виставки
        public string Artist { get; set; } // Ім'я автора
        public int Year { get; set; } // Рік створення
        public double Price { get; set; } // Ціна виставки

        public Exhibition() { }

        public Exhibition(string title, string artist, int year, double price)
        {
            Title = title;
            Artist = artist;
            Year = year;
            Price = price;
        }

        public abstract void DisplayInfo();
        public abstract string GetTypeName(); // Абстрактний метод для отримання типу виставки
    }

    public interface IVisitable
    {
        void Visit();
    }

    public interface ISellable
    {
        void Sell(double customPrice = 0);
    }

    public class PaintingExhibition : Exhibition, IVisitable, ISellable
    {
        public PaintingExhibition() { }

        public PaintingExhibition(string title, string artist, int year, double price)
            : base(title, artist, year, price) { }

        public override void DisplayInfo()
        {
            Console.WriteLine($"[Картина] Назва: {Title}, Автор: {Artist}, Рік: {Year}, Ціна: {Price:C}");
        }

        public override string GetTypeName()
        {
            return "Картина";
        }

        public void Visit()
        {
            Console.WriteLine($"Ви добре провели час на виставці картин: {Title}");
        }

        public void Sell(double customPrice = 0)
        {
            double finalPrice = customPrice > 0 ? customPrice : Price;
            Console.WriteLine($"Картина \"{Title}\" продана за {finalPrice:C}.");
        }
    }

    public class SculptureExhibition : Exhibition, IVisitable, ISellable
    {
        public SculptureExhibition() { }

        public SculptureExhibition(string title, string artist, int year, double price)
            : base(title, artist, year, price) { }

        public override void DisplayInfo()
        {
            Console.WriteLine($"[Скульптура] Назва: {Title}, Автор: {Artist}, Рік: {Year}, Ціна: {Price:C}");
        }

        public override string GetTypeName()
        {
            return "Скульптура";
        }

        public void Visit()
        {
            Console.WriteLine($"Ви добре провели час на виставці скульптур: {Title}");
        }

        public void Sell(double customPrice = 0)
        {
            double finalPrice = customPrice > 0 ? customPrice : Price;
            Console.WriteLine($"Скульптура \"{Title}\" продана за {finalPrice:C}.");
        }
    }

    public static class ExhibitionManager
    {
        public static void SaveExhibitionsToFile(List<Exhibition> exhibitions, string filePath)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<Exhibition>), new Type[] { typeof(PaintingExhibition), typeof(SculptureExhibition) });
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    serializer.Serialize(stream, exhibitions);
                }
                Console.WriteLine("Дані збережено у XML файл.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка збереження: {ex.Message}");
            }
        }

        public static List<Exhibition> LoadExhibitionsFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new List<Exhibition>();
            }

            try
            {
                var serializer = new XmlSerializer(typeof(List<Exhibition>), new Type[] { typeof(PaintingExhibition), typeof(SculptureExhibition) });
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    return (List<Exhibition>)serializer.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка завантаження: {ex.Message}");
                return new List<Exhibition>();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string filePath = "exhibitions.xml";
            List<Exhibition> exhibitions = ExhibitionManager.LoadExhibitionsFromFile(filePath);

            while (true)
            {
                Console.WriteLine("\nСистема управління галереєю");
                Console.WriteLine("1. Додати новий шедевр");
                Console.WriteLine("2. Список виставок");
                Console.WriteLine("3. Продати картину або скульптуру");
                Console.WriteLine("4. Видалити виставку");
                Console.WriteLine("5. Вийти");

                Console.Write("Виберіть опцію: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddExhibition(exhibitions, filePath);
                        break;
                    case "2":
                        ListExhibitions(exhibitions);
                        break;
                    case "3":
                        SellArtwork(exhibitions, filePath);
                        break;
                    case "4":
                        DeleteExhibition(exhibitions, filePath);
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Невірна опція. Спробуйте ще раз.");
                        break;
                }
            }
        }

        static void AddExhibition(List<Exhibition> exhibitions, string filePath)
        {
            try
            {
                Console.Write("Введіть назву шедевра: ");
                string title = Console.ReadLine();
                Console.Write("Введіть ім'я автора: ");
                string artist = Console.ReadLine();
                Console.Write("Введіть рік створення: ");
                int year = int.Parse(Console.ReadLine());
                Console.Write("Оберіть тип (1: Картина, 2: Скульптура): ");
                string type = Console.ReadLine();

                double basePrice = 1000;
                double price = basePrice + (DateTime.Now.Year - year) * (type == "1" ? 50 : 30);

                Exhibition newExhibition = type == "1"
                    ? new PaintingExhibition(title, artist, year, price)
                    : new SculptureExhibition(title, artist, year, price);

                newExhibition.DisplayInfo();
                Console.Write("Чи хочете зберегти шедевр? (1: Так, 0: Ні): ");
                if (Console.ReadLine() == "1")
                {
                    exhibitions.Add(newExhibition);
                    ExhibitionManager.SaveExhibitionsToFile(exhibitions, filePath);
                    Console.WriteLine("Шедевр успішно збережено.");
                }
                else
                {
                    Console.WriteLine("Шедевр не збережено.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }

        static void ListExhibitions(List<Exhibition> exhibitions)
        {
            if (!exhibitions.Any())
            {
                Console.WriteLine("Немає жодної виставки.");
                return;
            }

            for (int i = 0; i < exhibitions.Count; i++)
            {
                Console.WriteLine($"{i + 1}. [{exhibitions[i].GetTypeName()}] {exhibitions[i].Title} ({exhibitions[i].Artist}, {exhibitions[i].Year}) — {exhibitions[i].Price:C}");
            }

            Console.Write("Виберіть виставку для відвідування або натисніть Enter для повернення: ");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int index) && index > 0 && index <= exhibitions.Count)
            {
                if (exhibitions[index - 1] is IVisitable visitable)
                {
                    visitable.Visit();
                }
            }
        }

        static void SellArtwork(List<Exhibition> exhibitions, string filePath)
        {
            try
            {
                Console.Write("Виберіть номер виставки для продажу: ");
                int index = int.Parse(Console.ReadLine()) - 1;

                if (index >= 0 && index < exhibitions.Count && exhibitions[index] is ISellable sellable)
                {
                    Console.Write($"Продати за поточною ціною ({exhibitions[index].Price:C}) чи ввести свою (1: Поточна, 2: Свою)? ");
                    string option = Console.ReadLine();
                    if (option == "2")
                    {
                        Console.Write("Введіть свою ціну: ");
                        double customPrice = double.Parse(Console.ReadLine());
                        sellable.Sell(customPrice);
                    }
                    else
                    {
                        sellable.Sell();
                    }

                    exhibitions.RemoveAt(index);
                    ExhibitionManager.SaveExhibitionsToFile(exhibitions, filePath);
                }
                else
                {
                    Console.WriteLine("Некоректний номер виставки.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }

        static void DeleteExhibition(List<Exhibition> exhibitions, string filePath)
        {
            try
            {
                Console.Write("Введіть номер виставки для видалення: ");
                int index = int.Parse(Console.ReadLine()) - 1;

                if (index >= 0 && index < exhibitions.Count)
                {
                    exhibitions.RemoveAt(index);
                    ExhibitionManager.SaveExhibitionsToFile(exhibitions, filePath);
                    Console.WriteLine("Виставку успішно видалено.");
                }
                else
                {
                    Console.WriteLine("Некоректний номер виставки.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }
    }
}
