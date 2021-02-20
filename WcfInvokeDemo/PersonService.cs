using fastJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfInvokeDemo
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“Service1”。
    public class PersonService : IPersonService
    {
        public string HealthCheck()
        {
            DynamicJson dj = new DynamicJson("{}");
            dj.Set("status", "ok");
            return dj.ToString();
        }

        public string GetPersonName(string name)
        {
            return "Name : " + name;
        }

        /*public SharedAssembly.Person GetPerson(SharedAssembly.Person person)
        {
            return person;
        }*/

        public Person GetPerson(Person person)
        {
            return person;
        }
    }
}
