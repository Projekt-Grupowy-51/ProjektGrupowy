to run the project:
docker-compose up --build

to run db:
docker-compose up db

to add migrations:
dotnet ef migrations add nazwa_migracji

to update db:
dotnet ef database update

project is running on localhost:5000