```shell
dotnet nuget add source "https://nuget.pkg.github.com/{name}/index.json" \
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
--api-key YOUR_GITHUB_PAT
```