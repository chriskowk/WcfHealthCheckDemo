using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ThirdInvoke
{
    [ServiceContract]
    public interface IPersonService
    {
        [OperationContract]
        string GetPersonName(string name);

        [OperationContract]
        SharedAssembly.Person GetPerson(SharedAssembly.Person person);
    }
}
