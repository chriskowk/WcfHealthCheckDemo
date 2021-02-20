using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace ThirdInvoke
{ 
    class Program
    {
        static void Main(string[] args)
        {
            string uri = @"http://localhost:7003/WcfInvokeDemo.PersonService.svc";
            EndpointAddress endpointAddress = new EndpointAddress(uri);
            BasicHttpBinding basicBind = new BasicHttpBinding();
            ChannelFactory<IPersonService> factory = new ChannelFactory<IPersonService>(basicBind, endpointAddress);
            IPersonService channel = factory.CreateChannel();

            // 可以使用IDisposable
            using(channel as IDisposable)
            {
                Console.WriteLine(channel.GetPersonName("Lily"));
                SharedAssembly.Person person = new SharedAssembly.Person() { Address = "Nanshan district", Age = 20, Name = "Lily" };
                SharedAssembly.Person _person = channel.GetPerson(person);
                Console.WriteLine("Name:" + _person.Name + "\r\n" + "Age:" + _person.Age + "\r\n" + "Address:" + _person.Address);
            }

            // 也可以使用Close()
            IPersonService channel1 = factory.CreateChannel();
            Console.WriteLine(channel1.GetPersonName("Spencer"));
            ICommunicationObject channel2 = channel1 as ICommunicationObject;
            channel2.Close();

            Console.Read();
        }
    }
}
