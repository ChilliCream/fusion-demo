echo "------------------------------------------------------------------------------------"
echo “Restoring Tools”
echo "------------------------------------------------------------------------------------"

dotnet tool restore

echo "------------------------------------------------------------------------------------"
echo "Build solution"
echo "------------------------------------------------------------------------------------"

dotnet build

echo "------------------------------------------------------------------------------------"
echo "Export Schemas"
echo "------------------------------------------------------------------------------------"

dotnet run -p Subgraphs/Accounts -- schema export --output schema.graphql
dotnet run -p Subgraphs/Products -- schema export --output schema.graphql
dotnet run -p Subgraphs/Shipping -- schema export --output schema.graphql
dotnet run -p Subgraphs/Reviews -- schema export --output schema.graphql

echo "------------------------------------------------------------------------------------"
echo "Pack Subgraphs"
echo "------------------------------------------------------------------------------------"

dotnet fusion subgraph pack --working-directory ./Subgraphs/Accounts
dotnet fusion subgraph pack --working-directory ./Subgraphs/Products
dotnet fusion subgraph pack --working-directory ./Subgraphs/Shipping
dotnet fusion subgraph pack --working-directory ./Subgraphs/Reviews

echo "------------------------------------------------------------------------------------"
echo "Compose Subrgraphs"
echo "------------------------------------------------------------------------------------"

dotnet fusion compose -p ./Gateway/gateway.fgp -s ./Subgraphs/Accounts/accounts.fsp
dotnet fusion compose -p ./Gateway/gateway.fgp -s ./Subgraphs/Products/products.fsp
dotnet fusion compose -p ./Gateway/gateway.fgp -s ./Subgraphs/Shipping/shipping.fsp
dotnet fusion compose -p ./Gateway/gateway.fgp -s ./Subgraphs/Reviews/reviews.fsp