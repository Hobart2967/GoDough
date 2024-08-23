VERSION=$1

dotnet pack --configuration Release
echo dotnet nuget push GoDough.${VERSION}.nupkg --api-key $NUGET_API_KEY