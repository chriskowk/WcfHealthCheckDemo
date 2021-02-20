using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstInvoke
{
    class Program
    {
        static void Main(string[] args)
        {
            PersonService.PersonServiceClient client = new PersonService.PersonServiceClient();
            Console.WriteLine(client.GetPersonName("Lily"));
            PersonService.Person person = client.GetPerson(new PersonService.Person() { Age = 20, Address = "Nanshan district", Name = "Lily" });
            Console.WriteLine("Name:" + person.Name + "\r\n" + "Age:" + person.Age + "\r\n" + "Address:" + person.Address);
            Console.Read();
        }
    }
}
