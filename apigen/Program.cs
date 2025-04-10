

// Change this to the path of the assembly you want to inspect
using DotLiquid;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

partial class Program
{


    static void Main(string[] args)
    {
        //apigen.entity.liquid


        var assemblyPath = @"C:\Users\Michael\source\repos\ConsoleApp3\ConsoleApp3\bin\Debug\net9.0\consoleapp3.dll";


        var assembly = Assembly.LoadFrom(assemblyPath);

        // Try to find the DbContext type
        var dbContextType = assembly.GetTypes()
            .FirstOrDefault(t => typeof(DbContext).IsAssignableFrom(t) && !t.IsAbstract);

        if (dbContextType == null)
        {
            Console.WriteLine("No DbContext found in the assembly.");
            return;
        }

        // Try to find a design-time factory if available
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
            // Try to use a parameterless constructor
            dbContext = (DbContext)Activator.CreateInstance(dbContextType);
        }

        if (dbContext == null)
        {
            Console.WriteLine("Failed to create an instance of DbContext.");
            return;
        }

        // Get the model and print entity names
        IModel model = dbContext.Model;
        var tabledto = new TableDto();
        foreach (var entityType in model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            tabledto.Name = tableName;
            
            // Skip entities without a table name (like views or owned types)
            if (string.IsNullOrEmpty(tableName))
                continue;

            var primaryKey = entityType.FindPrimaryKey();
            if (primaryKey == null)
                continue;

            foreach (var keyProperty in primaryKey.Properties)
            {
                string columnName = keyProperty.GetColumnName();
                Console.WriteLine($"{tableName} -> {columnName}");
            }


            bool isKeyless = entityType.FindPrimaryKey() == null;
            if (isKeyless)
            {
                Console.WriteLine($"****** FOUND VIew *** -> {entityType.Name}");
            }
            tabledto.Columns = new List<ColumnDto>();
            foreach (var property in entityType.GetProperties())
            {
                string columnName = property.GetColumnName();
                string propertyName = property.Name;
                Type clrType = property.ClrType;

                // Make nullable type readable, e.g. "int?" instead of "Nullable<int>"
                string typeName = Nullable.GetUnderlyingType(clrType) != null
                    ? $"{Nullable.GetUnderlyingType(clrType).Name}?"
                    : clrType.Name;

                Console.WriteLine($"  Property: {propertyName}  |  Column: {columnName}  |  Type: {typeName}");
                tabledto.Columns.Add(new ColumnDto() { Name = propertyName, Type = typeName });

            }

            var temp = ReadEmbeddedResource("apigen.entity.liquid");
            var template = DotLiquid.Template.Parse(temp);
           
            var localVariables = Hash.FromAnonymousObject(new { tabledto });
            var result = template.Render(localVariables); 
            Console.WriteLine(result);


        }


    }
    public static string ReadEmbeddedResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();  // or use another assembly if needed
        using (var stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null)
            {
                throw new FileNotFoundException($"Resource {resourceName} not found.");
            }

            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}

