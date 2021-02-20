using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using System.Reflection;

namespace FourthInvoke
{
    class Program
    {
        static void Main(string[] args)
        {
            // 这里给出的是WSDL的地址
            // 我现在严重怀疑这种方法就是SvcUtil工具的实现原理？
            string uri = @"http://localhost:7003/WcfInvokeDemo.PersonService.svc?wsdl";
            string contractName = "IPersonService";
            CompilerResults compilerResults = null;
            // 获取代理实例，本例中即为PersonServiceClient
            var proxyInstance = InvokeService.GetProxyInstance(ref compilerResults, uri, contractName);
            
            // 输出proxyInstance的类型名称，为“PersonServiceClient"
            //Console.WriteLine(proxyInstance.GetType().Name); 

            //string operationName = "GetPersonName";
            string operationName1 = "GetPerson";
            // 获取PersonServiceClient中名为“GetPersonName"的方法
            //MethodInfo methodInfo = proxyInstance.GetMethod(operationName);
            MethodInfo methodInfo1 = proxyInstance.GetType().GetMethod(operationName1);
            // 获取方法所需的参数
            //ParameterInfo[] parameterInfo = methodInfo.GetParameters();
            ParameterInfo[] parameterInfo1 = methodInfo1.GetParameters();
            /*foreach(var item in parameterInfo)
            {
                // 在本例中，参数为name，类型为String，因此输出为name:String
                Console.WriteLine(item.Name + ":" + item.ParameterType.Name);
            }
            foreach(var item in parameterInfo1)
                // 再试一下，如果为ComplexType，会输出person:Person
                Console.WriteLine(item.Name + ":" + item.ParameterType.Name);*/
            // 获取ComplexType的属性，这里是因为只有一个参数，即为person
            var properties = parameterInfo1[0].ParameterType.GetProperties(); 
            foreach (var item in properties)
            /**
            输出的结果为：
               ExtensionData: ExtensionDataObject
               Address:String
               Age:Int32
               Name:String
               其中，ExtensionDataObject类用于存储已经通过添加新成员扩展的版本化数据协定中的数据
               用于解决两个版本的数据契约之间某个参数被添加或者删除的问题
               详情可见：
               https://www.cnblogs.com/CharlesLiu/archive/2010/02/09/1666605.html
           **/
                Console.WriteLine(item.Name + ":" + item.PropertyType.Name);

            // 如果调用GetPerson方法，需要给参数的属性赋值
            // 创建参数的实例
            var parameter = compilerResults.CompiledAssembly.CreateInstance(parameterInfo1[0].ParameterType.FullName, false, BindingFlags.CreateInstance, null, null, null, null);

            //Console.WriteLine(parameter.GetType().Name);
            // 给参数的属性赋值时，一定要根据顺序来
            properties[1].SetValue(parameter, "Home");
            properties[2].SetValue(parameter, 20);
            properties[3].SetValue(parameter, "Lily");

            object[] operationParameters = new object[] { parameter };
            // 调用methodInfo1方法
            var ans = methodInfo1.Invoke(proxyInstance, operationParameters);

            // 输出返回值的每个属性的值
            foreach(var item in ans.GetType().GetProperties())
            {
                Console.WriteLine(item.Name + ":" + item.GetValue(ans));
            }

            Console.Read();
        }
    }
}
