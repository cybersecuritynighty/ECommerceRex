# Copy scripts into the container
docker cp Database/create_tables.sql sqlserver:/tmp/create_tables.sql
docker cp Database/seed_data.sql sqlserver:/tmp/seed_data.sql

# Execute the scripts
docker exec -i sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Your_password123 -d master -i /tmp/create_tables.sql
docker exec -i sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Your_password123 -d master -i /tmp/seed_data.sql
