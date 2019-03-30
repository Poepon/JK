FROM microsoft/dotnet:2.2.2-aspnetcore-runtime

WORKDIR /app
COPY . .

ENV ASPNETCORE_URLS http://+:5174;http://+:5175
EXPOSE 5174
EXPOSE 5175

ENTRYPOINT ["dotnet", "JK.Web.Mvc.dll"]
