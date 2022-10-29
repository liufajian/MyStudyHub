// See https://aka.ms/new-console-template for more information
using RuntimeBuilder;
using System.Reflection;

Console.WriteLine("Hello, World!");

MyClassBuilder MCB = new MyClassBuilder("Student");
var myclass = MCB.CreateObject(new string[3] { "ID", "Name", "Address" }, new Type[3] { typeof(int), typeof(string), typeof(string) });
Type TP = myclass.GetType();

foreach (PropertyInfo PI in TP.GetProperties())
{
    Console.WriteLine(PI.Name);
}

Console.ReadLine();
