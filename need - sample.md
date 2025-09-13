```shell
dotnet nuget add source "https://nuget.pkg.github.com/codewithmecoder/index.json" \
--name "github" \
--username \
--password 
```

```shell
dotnet pack --configuration Release
```

```shell
dotnet nuget push "bin/Release/CommandQuery.1.0.0.nupkg" \
--source "github" \
--api-key 
```
```shell
dotnet nuget push "bin/Release/MyLibrary.1.0.0.nupkg" \
--api-key YOUR_NUGET_API_KEY \
--source https://api.nuget.org/v3/index.json
```