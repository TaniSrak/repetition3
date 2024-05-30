using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace repetition3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //повторение json, xml
            XDocument doc = new XDocument
                (
                    new XElement("RootElement"),
                    new XElement("ChildElement", "Value")
                );

            XDocument doc2 = XDocument.Load("path/to/file.xml"); //загрузка
            XDocument doc3 = XDocument.Parse("<dfvdsfv>"); //парсим

            //добавляем к корневому элементу дочерний
            doc.Root.Add(new XElement("NewChild"));
            //получили один дочерний элемент
            XElement childElement = doc.Element("RootElement").Element("ChildElement");
            //получаем список дочерних элементов
            IEnumerable<XElement> childElements = doc.Element("RootElement").Elements("ChildElement");
            //задаем новое значение newvalue
            childElement.Value = "NewValue";
            //доюавить атрибут в корневой элемент
            doc.Root.Add(new XAttribute("NewSttribute", "AttributeValue"));
            //добавляем атрибут в дочерний элемент
            doc.Root.Element("ChildElement").Add(new XAttribute("NewSttribute", "AttributeValue"));
            //используем linq искали элемент с помощью значения
            var elements = doc.Descendants("ChildElement").Where(e => (string)e == "value");
            //найти атрибут через linq
            var elementss = doc.Descendants("ChildElement").FirstOrDefault(e => e.Attribute("asd") != null);
            //изменить атрибут
            XElement childAnother = doc.Element("RootElement").Element("ChildElement");
            childAnother.SetAttributeValue("Attribute", "asd"); //вместо атрибутвейлю, будет asd

            //Task1
            Catalog catalog = new Catalog();
            catalog.AddProduct(new Product { Id = 1, Name = "Product 1", Price = 10.5, Category = "Category 1" });
            catalog.AddProduct(new Product { Id = 2, Name = "Product 2", Price = 20.75, Category = "Category 2" });

            //сохраняем и выгружаем
            catalog.SaveToJson("products.json"); //сохраним json
            catalog.LoadFromJson("products.json"); //выгрузим json
            foreach (var i in catalog.products)
            {
                Console.WriteLine($"Id: {i.Id}, Name: {i.Name}, Price: {i.Price}, Category: {i.Category}");
            }

            catalog.SaveToXml("products.xml"); //сохраним xml
            catalog.LoadFromXml("products.xml"); //выгрузим xml
            foreach (var i in catalog.products)
            {
                Console.WriteLine($"Id: {i.Id}, Name: {i.Name}, Price: {i.Price}, Category: {i.Category}");
            }

            //фильтрация по категории и по цене
            IEnumerable<Product> filtereProducts = catalog.FilterProductsByCategory("category 1");
            foreach (var i in filtereProducts)
            {
                Console.WriteLine($"Id: {i.Id}, Name: {i.Name}, Price: {i.Price}, Category: {i.Category}");
            }

            IEnumerable<Product> sortedProducts = catalog.SortProductsByPrice();
            foreach (var i in sortedProducts)
            {
                Console.WriteLine($"Id: {i.Id}, Name: {i.Name}, Price: {i.Price}, Category: {i.Category}");
            }

            Console.ReadLine();
        }
    }

    //Task1
    public interface IProduct
    {
        int Id { get; set; }
        string Name { get; set; }
        double Price { get; set; }
        string Category { get; set; }
    }

    public class Product : IProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Category { get; set; }
    }

    public class Catalog
    {
        public List<Product> products = new List<Product>();
        public void AddProduct(Product product)
        {
            products.Add(product);
        }
        public void RemoveProduct(int id)
        {
            products.RemoveAll(p => p.Id == id);
        }
        public void UpdateProduct(Product prod)
        {
            var existing = products.FirstOrDefault(p => p.Id == prod.Id);
            if (existing != null)
            {
                existing.Name = prod.Name;
                existing.Price = prod.Price;
                existing.Category = prod.Category;
            }
        }
        //сохранять и выгружать из json
        public void SaveToJson(string path)
        {
            string json = JsonConvert.SerializeObject(products);
            File.WriteAllText(path, json);
        }
        //создали список json  и возвращаем список продуктов
        public void LoadFromJson(string path)
        {
            string json = File.ReadAllText(path);
            products = JsonConvert.DeserializeObject<List<Product>>(json);
        }
        //сохраняем в xml с помощью линки
        public void SaveToXml(string path)
        {
            XElement xml = new XElement("Products",
                from product in products
                select new XElement("Product",
                    new XAttribute("Id", product.Id),
                    new XAttribute("Name", product.Name),
                    new XAttribute("Price", product.Price),
                    new XAttribute("Category", product.Category)
                ));
            xml.Save(path);
        }

        public void LoadFromXml(string path)
        {
            //указываем культуру, которая использует точку в качестве десятичного разделителя
            var culture = System.Globalization.CultureInfo.InvariantCulture;
            XDocument xml = XDocument.Load(path);
            products = xml.Descendants("Product").Select(p => new Product
            {
                Id = int.Parse(p.Attribute("Id").Value),
                Name = p.Attribute("Name").Value,
                Price = double.Parse(p.Attribute("Price").Value, culture),
                Category = p.Attribute("Category").Value,
            }).ToList(); //перевод в список
        }

        //фильтрация пок атегории и цене с помощью линки
        public IEnumerable<Product> FilterProductsByCategory(string category)
        {
            return products.Where(p => p.Category.ToLower() == category.ToLower());
        }
        public IEnumerable<Product> FilterProductsByPrice(double minPrice, double maxPrice)
        {
            return products.Where(p => p.Price >= minPrice && p.Price <= maxPrice);
        }

        //сортировка по цене и имени
        public IEnumerable<Product> SortProductsByPrice()
        {
            return products.OrderBy(p => p.Price);
        }
        public IEnumerable<Product> SortProductsByName()
        {
            return products.OrderBy(p => p.Name);
        }

    }

}
