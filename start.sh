#!/bin/bash
# Build the solution
dotnet build PawNest.sln
# Run the API project, binding to Railway's dynamic port
dotnet run --project PawNest.API --urls=http://0.0.0.0:${PORT}