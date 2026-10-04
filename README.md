# FetaMilter

Mostly a project to play around with modern Razor views and EF Core, and some testing technology.

## How to run locally

1. Copy `.env.example` to `.env`
2. Run `docker compose up --build`
3. Navigate your browser to [http://www.fetamilter.localhost](http://www.fetamilter.localhost)
4. There is no step 4

You can also run it locally, but you'll need to update `appsettings.json`

## How to test

Just `dotnet test` - uses Testcontainers & Playwright to create a self-contained.

## Bonus step - Get Caddy HTTPS CA from docker

If you want to use HTTPS directly, you can extract it from the docker container and add it to your cert store

For Windows, you can use the command below in an admin pwsh window,
otherwise you can check out the [Caddy Docs](https://caddyserver.com/docs/running#local-https-with-docker)

```
docker compose cp fetamilter-proxy:/data/caddy/pki/authorities/local/root.crt  $env:TEMP/root.crt && certutil -addstore -f "ROOT" $env:TEMP/root.crt
```

## Generating Database Migrations

install the dotnet ef tools

```
dotnet tool install --global dotnet-ef
```

navigate the console to the App project, then run

```
dotnet ef migrations add <migration_name> --project ..\PgsqlMigrations\
```
