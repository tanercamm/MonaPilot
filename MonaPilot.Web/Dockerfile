FROM node:18 AS build
WORKDIR /app
COPY MonaPilot.Web/ .
RUN npm install
RUN npm run build

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/dist ./wwwroot
EXPOSE 5281
ENTRYPOINT ["dotnet", "MonaPilot.Web.dll"]
