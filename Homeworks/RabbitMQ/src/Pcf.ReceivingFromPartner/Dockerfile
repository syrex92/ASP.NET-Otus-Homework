# Prepare the build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the solution file and project files, then restore dependencies
COPY Pcf.Messages/Pcf.Messages.csproj Pcf.Messages/
COPY Pcf.ReceivingFromPartner/*.sln Pcf.ReceivingFromPartner/
COPY Pcf.ReceivingFromPartner/*/*.csproj ./Pcf.ReceivingFromPartner/
WORKDIR /app/Pcf.ReceivingFromPartner
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*} && mv $file ${file%.*}; done
RUN dotnet restore

# Copy the remaining files and publish the application
COPY . ./aspnetapp
WORKDIR /app/Pcf.ReceivingFromPartner/aspnetapp
RUN dotnet publish -c Release -o out

# Use the .NET 8 runtime for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/Pcf.ReceivingFromPartner/aspnetapp/out ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "Pcf.ReceivingFromPartner.WebHost.dll"]