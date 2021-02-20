using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondInvoke
{
    class Program
    {
        static void Main(string[] args)
        {
            PersonServiceClient client = new PersonServiceClient();
            Console.WriteLine(client.GetPersonName("Lily"));

            WcfInvokeDemo.Person person = client.GetPerson(new WcfInvokeDemo.Person() { Age = 20, Address = "Nanshan district", Name = "Lily" });
            Console.WriteLine("Name:" + person.Name + "\r\n" + "Age:" + person.Age + "\r\n" + "Address:" + person.Address);
            Console.Read();
        }
    }
}
