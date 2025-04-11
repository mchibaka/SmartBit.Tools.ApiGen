using CommandLine;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Mono.TextTemplating;
using SmartBit.Tools.ApiGen.Models;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
namespace SmartBit.Tools.ApiGen;

partial class Program
{
    class Options
    {
        [Option('a', "assembly", Required = true, HelpText = "assembly path")]
        public string Assembly { get; set; }
    }

    static void Main(string[] args)
    {
        var parserResults = Parser.Default.ParseArguments<Options>(args);
        var options = parserResults.Value;
        var assembly = Assembly.LoadFrom(options.Assembly);
        var dbContextType = assembly.GetTypes()
            .FirstOrDefault(t => typeof(DbContext).IsAssignableFrom(t) && !t.IsAbstract);
        if (dbContextType == null)
        {
            Console.WriteLine("No DbContext found in the assembly.");
            return;
        }
        var factoryType = assembly.GetTypes()
            .FirstOrDefault(t =>
                t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IDesignTimeDbContextFactory<>) &&
                    i.GenericTypeArguments[0] == dbContextType));
        DbContext dbContext;
        if (factoryType != null)
        {
            var factory = Activator.CreateInstance(factoryType);
            var createMethod = factoryType.GetMethod("CreateDbContext");
            dbContext = (DbContext)createMethod.Invoke(factory, new object[] { Array.Empty<string>() });
        }
        else
        {
            dbContext = (DbContext)Activator.CreateInstance(dbContextType);
        }

        if (dbContext == null)
        {
            Console.WriteLine("Failed to create an instance of DbContext.");
            return;
        }

        IModel model = dbContext.Model;
        model.AddRuntimeAnnotation("ContextName", dbContext.GetType().Name);
        var tabledto = new TableDto();
        var ttContent = ResourceHelper.ReadEmbeddedResource("smartbit-apigen.entity.tt");
        var host = new TemplateGenerator();
        host.ReferencePaths.Add(typeof(IEntityType).Assembly.Location);
        host.Refs.Add(typeof(IEntityType).Assembly.Location);
        host.Refs.Add(typeof(RelationalEntityTypeExtensions).Assembly.Location);
        host.Imports.Add("Microsoft.EntityFrameworkCore.Metadata");
        host.Imports.Add("Microsoft.EntityFrameworkCore");
        CompiledTemplate compiledTemplate = null;
        try
        {
            compiledTemplate = host.CompileTemplateAsync(ttContent).Result;
            if (host.Errors.HasErrors)
            {

                foreach (var error in host.Errors)
                {
                    Console.WriteLine("Host Error : " + error.ToString());
                }
                if (host.Errors.Count > 0) return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("TextTemplate Compilation Error: " + ex.ToString());
        }
        foreach (var entityType in model.GetEntityTypes().Take(1))
        {
           if (string.IsNullOrEmpty(entityType.GetTableName()))
                continue;
            Console.WriteLine(RunTemplate(host, compiledTemplate, model, entityType));
        }
}

    public  static string RunTemplate(TemplateGenerator host, CompiledTemplate compiledTemplate, IModel efModel, IEntityType efEntityType)
    {
        var session = host.GetOrCreateSession();
        session.Clear();
        session.Add("EntityType", efEntityType);
        session.Add("Model", efModel);
        return compiledTemplate.Process(); ;
    }

}

