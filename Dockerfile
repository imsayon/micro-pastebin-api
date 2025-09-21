# Stage 1: The 'build' stage, used to compile our code
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy all the project files and build the application
COPY . .
RUN dotnet publish -c Release -o out

# Stage 2: The final stage, used to create the small runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy only the compiled output from the 'build' stage
COPY --from=build /app/out .

# Expose the port the app will run on
EXPOSE 8080

# The command to start the application
ENTRYPOINT ["dotnet", "MicroPastebinApi.dll"]