# SmartBit ApiGen

A tool for generating Apis from EF Context. 

# Customnization

Modify the text template entity.tt to generate code to your specific needs.
Happy coding!

# How it works

- It inputs an assembly(myproject.dll) --> Use reflection to extract Ef Model (IModel) --> Loops through all entities in the model ->
- For each entity, create entityController and web methods to support CRUD opetation and corresponding Data Transfer Objects (DTOs)

