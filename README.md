# Practical EdgeDB.NET (2)

Samples on how to use EdgeDB with ASP.NET Core. It requires the .NET 8 preview 6.

- [Hello World](projects/hello-world)

  This sample connects to `hello-world` instance and `edgedb` database to query a `Message` object and display it. 

- [Simple Blog](projects/simple-blog)

  This sample shows how to insert data into EdgeDB. We are using `uuid`, `str`, `anyenum` and `datetime` EdgeDB types and they are mapped to C# as follows  `public record BlogPost(Guid Id, string Title, string Body, BlogPostStatus Status, EdgeDB.DataTypes.DateTime DateCreated);`
