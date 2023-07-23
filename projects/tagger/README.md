# Tagger


## Follow the following steps to make this sample works

- Go to `db` folder.
- Execute the following command `edgedb project init --server-instance tagger`.
  - `Specify the name of EdgeDB instance to use with this project [default: tagger]:`.
  - Just press enter.
- Move one directory up.
- Execute the following command `dotnet watch`.
- Open your browser to `http://localhost:5000`.

## How to clean up 

Run this command line `edgedb instance destroy -I tagger  --force `.