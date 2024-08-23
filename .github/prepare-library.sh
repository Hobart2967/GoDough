VERSION=$1

xmlstarlet edit \
  --inplace \
  --update "/Project/PropertyGroup/AssemblyVersion" \
  --value  "$VERSION" \
  GoDough.csproj