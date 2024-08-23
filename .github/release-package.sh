VERSION=$1

dotnet pack --configuration Release
echo dotnet nuget push GoDough.${VERSION}.nupkg