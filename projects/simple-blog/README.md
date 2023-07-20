# Simple Blog

This sample demonstrates a way on how to save data into EdgeDB.

We are using `uuid`, `str`, `anyenum` and `datetime` EdgeDB types and they are mapped to C# as follows  `public record BlogPost(Guid Id, string Title, string Body, BlogPostStatus Status, EdgeDB.DataTypes.DateTime DateCreated);`

## Follow the following steps to make this sample works

- Go to `db` folder.
- Execute the following command `edgedb project init --server-instance simple-blog`.
  - `Specify the name of EdgeDB instance to use with this project [default: simple-blog]:`.
  - Just press enter.
- Move one directory up.
- Execute the following command `dotnet watch`.
- Open your browser to `http://localhost:5000`.

## How to clean up 

Run this command line `edgedb instance destroy -I simple-blog  --force `.