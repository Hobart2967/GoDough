VERSION=$1

dotnet pack --configuration Release
dotnet nuget push \
  GoDough.${VERSION}.nupkg \
  --api-key $NUGET_API_KEY \
  --source https://api.nuget.org/v3/index.json