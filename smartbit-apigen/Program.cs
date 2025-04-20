using CommandLine;
using CommandLine.Text;
using Figgle;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.IdentityModel.Tokens;
using Mono.TextTemplating;
using SmartBit.Tools.ApiGen.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using YamlDotNet.Serialization;
namespace SmartBit.Tools.ApiGen;

partial class Program
{
    class Options
    {
        [Value(0, Default = "apigen.yml", Required = false, HelpText = "Path to configuration file", MetaName = "Configuration File")]
        public string ConfigFile { get; set; }

        [Option('a', "assembly", HelpText = "assembly path")]
        public string Assembly { get; set; }

        [Option('o', "output-dir", HelpText = "Output directory")]
        public string OutputDirectory { get; set; }

        [Option('n', "namespace", HelpText = "Namespace for the generated APIs")]
        public string Namespace { get; set; } = null;

        [Option('f', "force", HelpText = "Overwrite if file exists", Default = false)]
        public bool Force { get; set; }

    }


    static void Main(string[] args)
    {
        Console.WriteLine(FiggleFonts.Standard.Render("SmartBit ApiGen"));
        Console.WriteLine("SmartBit API Generator is code generator that generates .net core APIs based on Entity Framework");
        var parser = new Parser((o) =>
       {
           o.AutoHelp = false;
       });
        var parserResults = parser.ParseArguments<Options>(args);
        parserResults.WithNotParsed((err) =>
        {
            var helpText = HelpText.AutoBuild(parserResults);
            Console.WriteLine(helpText);
            Environment.Exit(1); // Exit with non-zero code to indicate failure
        });

        var options = parserResults.Value;

        if (options.OutputDirectory.IsNullOrEmpty())
        {
            options.OutputDirectory = "Controllers";
        }

       

        if (!string.IsNullOrEmpty(options.ConfigFile) && File.Exists(options.ConfigFile))
        {
            var deserializer = new DeserializerBuilder().Build();
            var yamlOptions = deserializer.Deserialize<Options>(File.ReadAllText(options.ConfigFile));

            // Merge YAML settings with command-line options
            options.Assembly = options.Assembly ?? yamlOptions.Assembly;
            options.OutputDirectory = options.OutputDirectory ?? yamlOptions.OutputDirectory;
            options.Namespace = options.Namespace ?? yamlOptions.Namespace;
            options.Force = options.Force || yamlOptions.Force;
        }
        if (options.Assembly.IsNullOrEmpty())
        {
            if(!string.IsNullOrEmpty(options.ConfigFile) && !File.Exists(options.ConfigFile))
            {
                Console.WriteLine($"ERROR: File not found {options.ConfigFile}");
            }
            Console.WriteLine("ERROR: Assembly must be specified or path to config apigen.yml");
            return;
        }


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
        List<string> outputFiles = new List<string>();
        if (!options.Force)
        {
            foreach (var entityType in model.GetEntityTypes().Take(1))
            {

                if (string.IsNullOrEmpty(entityType.GetTableName()))
                    continue;
                string outputFilePath = GetOutputFilePath(options, entityType);
                if (File.Exists(outputFilePath))
                {
                    Console.WriteLine($"Error: Can't overwrite existing file (use -f), {outputFilePath}");
                    return;
                }
            }
        }
        string assemblyNameSpace = assembly.GetName().Name;
        string controllerNameSpace = GetCombinedNamespace(assemblyNameSpace, options.OutputDirectory);
        model.AddRuntimeAnnotation("ContextName", dbContext.GetType().Name);
        model.AddRuntimeAnnotation("AssemblyNamespace", assemblyNameSpace);
        model.AddRuntimeAnnotation("ControllerNamespace", options.Namespace ?? controllerNameSpace);
        var ttContent = ResourceHelper.ReadEmbeddedResource("smartbit-apigen.entity.tt");
        var host = new TemplateGenerator();
        host.ReferencePaths.Add(typeof(IEntityType).Assembly.Location);
        host.Refs.Add(typeof(IEntityType).Assembly.Location);
        host.Refs.Add(typeof(IList<>).Assembly.Location);
        host.Refs.Add(typeof(RelationalEntityTypeExtensions).Assembly.Location);
        host.Imports.Add("Microsoft.EntityFrameworkCore.Metadata");
        host.Imports.Add("Microsoft.EntityFrameworkCore");
        host.Imports.Add("System.Collections.Generic");
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
        foreach (var entityType in model.GetEntityTypes())
        {

            var dbSetname = GetDbSetName(dbContext.GetType(), entityType);
            entityType.SetRuntimeAnnotation("DbSetName", dbSetname);
            var generatedControllerCode = RunTemplate(host, compiledTemplate, model, entityType);
            var destFileName = GetOutputFilePath(options, entityType);
            if (options.Force || !File.Exists(destFileName))
            {
                if (!Directory.Exists(Path.GetDirectoryName(destFileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destFileName));
                }
                File.WriteAllText(destFileName, generatedControllerCode, Encoding.ASCII);
                Console.WriteLine($"Writing: {destFileName}");
            }
        }
        Console.WriteLine("Code generation complete!");
    }

    public static string GetDbSetName(Type ctx, IEntityType entityType)
    {
        var dbSetProperty = ctx.GetProperties()
            .FirstOrDefault(p => p.PropertyType.IsGenericType
                && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)
                && p.PropertyType.GetGenericArguments()[0].Name == entityType.ClrType.Name);

        return dbSetProperty?.Name;
    }

    private static string GetOutputFilePath(Options options, IEntityType entityType)
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        return Path.Combine(currentDirectory,
                  options.OutputDirectory,
                  $"_auto_{entityType.ClrType.Name}Controller.cs");
    }

    public static string RunTemplate(TemplateGenerator host, CompiledTemplate compiledTemplate, IModel efModel, IEntityType efEntityType)
    {
        var session = host.GetOrCreateSession();
        session.Clear();
        session.Add("EntityType", efEntityType);
        session.Add("Model", efModel);
        return compiledTemplate.Process();
    }

    public static string GetCombinedNamespace(string @namespace, string outputDir)
    {
        // Split the output directory into parts
        var dirParts = outputDir.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

        // Dehumanize each part (convert to valid namespace identifier)
        var validParts = dirParts.Select(part => new string(part.Where(c => char.IsLetterOrDigit(c) || c == '_').ToArray()));

        // Combine the namespace parts
        var combinedNamespace = string.Join(".", new[] { @namespace }.Concat(validParts));

        return combinedNamespace;
    }




}

