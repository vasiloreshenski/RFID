FROM mcr.microsoft.com/mssql/server
COPY ./db_create_script.sql ./
ENV ACCEPT_EULA=Y
ENV SA_PASSWORD=P@ssw0rd
RUN (/opt/mssql/bin/sqlservr --accept-eula & ) | grep -q "Service Broker manager has started" && /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P P@ssw0rd -d master -i db_create_script.sql