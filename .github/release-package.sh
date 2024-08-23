VERSION=$1

dotnet pack --configuration Release
dotnet nuget push \
  .godot/mono/temp/bin/Release/GoDough.${VERSION}.nupkg \
  --api-key $NUGET_API_KEY \
  --source https://api.nuget.org/v3/index.json