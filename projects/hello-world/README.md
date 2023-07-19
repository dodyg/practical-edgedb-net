# Hello world

This demonstrates the most basic way to connect to an EdgeDB instance `hello-world` to the default `edgedb` database. Here we will use the `INamingStrategy.SnakeCaseNamingStrategy` naming convention e.g. `first_name` property in the database will be mapped to `FirstName` in a C# class or record.

## Follow the following steps to make this sample works

- Go to `db` folder.
- Execute the following command `edgedb project init --server-instance hello-world`.
  - `Specify the name of EdgeDB instance to use with this project [default: hello-world]:`.
  - Just press enter.
- Move one directory up.
- Execute the following command `dotnet watch`.
- Open your browser to `http://localhost:5000`.

## How to clean up 

Run this command line `edgedb instance destroy -I hello-world  --force `.