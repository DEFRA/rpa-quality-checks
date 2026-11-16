FROM mcr.microsoft.com/dotnet/sdk:10.0.403-noble AS build
WORKDIR /src
COPY RPA.QualityPortal/RPA.QualityPortal/RPA.QualityPortal.csproj RPA.QualityPortal/RPA.QualityPortal/

# Optional non-secret package source (for example an internal feed mirror).
ARG NUGET_SOURCE
RUN if [ -n "$NUGET_SOURCE" ]; then \
        dotnet restore RPA.QualityPortal/RPA.QualityPortal/RPA.QualityPortal.csproj --source "$NUGET_SOURCE"; \
    else \
        dotnet restore RPA.QualityPortal/RPA.QualityPortal/RPA.QualityPortal.csproj; \
    fi

COPY RPA.QualityPortal/RPA.QualityPortal/ RPA.QualityPortal/RPA.QualityPortal/
RUN dotnet publish RPA.QualityPortal/RPA.QualityPortal/RPA.QualityPortal.csproj \
    -c Release --no-restore -o /out /p:UseAppHost=false

# Ubuntu includes ICU: en-GB dates/number formats must not use invariant globalization.
FROM mcr.microsoft.com/dotnet/aspnet:10.0.14-noble AS final
WORKDIR /app
COPY --from=build /out/ ./
ENV ASPNETCORE_HTTP_PORTS=8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "RPA.QualityPortal.dll"]