using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Globalization;
using System.Collections.ObjectModel;
using System.CodeDom.Compiler;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace FourthInvoke
{
    public static class InvokeService
    {
        // 创建代理实例
        public static object GetProxyInstance(ref CompilerResults compilerResults, string uri, string contractName)
        {
            object proxyInstance = null;
            Uri address = new Uri(uri);
            MetadataExchangeClientMode mexMode = MetadataExchangeClientMode.HttpGet;
            MetadataExchangeClient metadataExchangeClient = new MetadataExchangeClient(address, mexMode);
            metadataExchangeClient.ResolveMetadataReferences = true;
            MetadataSet metadataSet = metadataExchangeClient.GetMetadata();
            WsdlImporter wsdlImporter = new WsdlImporter(metadataSet);
            Collection<ContractDescription> contracts = wsdlImporter.ImportAllContracts();
            ServiceEndpointCollection allEndPoints = wsdlImporter.ImportAllEndpoints();
            ServiceContractGenerator serviceContractGenerator = new ServiceContractGenerator();
            var endpointsForContracts = new Dictionary<string, IEnumerable<ServiceEndpoint>>();
            foreach(ContractDescription contract in contracts)
            {
                serviceContractGenerator.GenerateServiceContractType(contract);
                endpointsForContracts[contract.Name] = allEndPoints.Where(x => x.Contract.Name == contract.Name).ToList();
            }

            CodeGeneratorOptions codeGeneratorOptions = new CodeGeneratorOptions();
            codeGeneratorOptions.BracingStyle = "C";
            CodeDomProvider codeDomProvider = CodeDomProvider.CreateProvider("C#");
            CompilerParameters compilerParameters = new CompilerParameters(new string[] { "System.dll", "System.ServiceModel.dll", "System.Runtime.Serialization.dll" });
            compilerParameters.GenerateInMemory = true;
            compilerResults = codeDomProvider.CompileAssemblyFromDom(compilerParameters, serviceContractGenerator.TargetCompileUnit);

            if (compilerResults.Errors.Count == 0)
            {
                Type proxyType = compilerResults.CompiledAssembly.GetTypes().First(t => t.IsClass && t.GetInterface(contractName) != null && t.GetInterface(typeof(ICommunicationObject).Name) != null);
                ServiceEndpoint serviceEndpoint = endpointsForContracts[contractName].First();
                proxyInstance = compilerResults.CompiledAssembly.CreateInstance(proxyType.Name, false, BindingFlags.CreateInstance,
                    null, new object[] { serviceEndpoint.Binding, serviceEndpoint.Address }, CultureInfo.CurrentCulture, null);
            }
            return proxyInstance;
        }
    }
}
