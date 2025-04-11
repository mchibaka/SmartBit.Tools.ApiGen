namespace SmartBit.Tools.ApiGen.TextTemplating
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http.Headers;
    using Microsoft.VisualStudio.TextTemplating;

    public class CustomTextTemplatingHost : ITextTemplatingEngineHost
    {
        public object Model { get; set; }

        public string FileExtension { get; set; } = ".txt";
        public string OutputEncodingValue { get; set; } = "utf-8";

        public IList<string> StandardAssemblyReferences => new[]
        {
        typeof(System.Uri).Assembly.Location,
        typeof(object).Assembly.Location,
        typeof(System.Linq.Enumerable).Assembly.Location,
        typeof(Microsoft.EntityFrameworkCore.Metadata.IModel).Assembly.Location,
    };

        public IList<string> StandardImports => new[]
        {
        "System",
        "Microsoft.EntityFrameworkCore.Metadata"
    };

        public string ResolveAssemblyReference(string assemblyReference) => assemblyReference;

        public Type ResolveDirectiveProcessor(string processorName) => throw new Exception("No custom directive processors are supported");

        public string ResolveParameterValue(string directiveId, string processorName, string parameterName) => null;

        public string ResolvePath(string fileName)
        {
            var path = Path.Combine(Path.GetDirectoryName(TemplateFile), fileName);
            return File.Exists(path) ? path : fileName;
        }

        public void LogErrors(System.CodeDom.Compiler.CompilerErrorCollection errors)
        {
            foreach (System.CodeDom.Compiler.CompilerError error in errors)
            {
                Console.WriteLine(error.ToString());
            }
        }

        public AppDomain ProvideTemplatingAppDomain(string content) => AppDomain.CurrentDomain;

        public object GetHostOption(string optionName)
        {
            if (optionName == "CacheAssemblies") return true;
            return null;
        }

       
        public void SetFileExtension(string extension) => FileExtension = extension;

        public void SetOutputEncoding(System.Text.Encoding encoding, bool fromOutputDirective) =>
            OutputEncodingValue = encoding.WebName;

        public object GetService(Type serviceType) => null;

        public bool LoadIncludeText(string requestFileName, out string content, out string location)
        {
            location = requestFileName;
            content = File.ReadAllText(requestFileName);
            return true;
        }

        public IDictionary<string, object> Session { get; set; } = new Dictionary<string, object>();

        public string TemplateFile { get; set; }
    }

}
