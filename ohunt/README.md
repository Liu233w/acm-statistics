OHunt
==========================

A stateful crawler with a database.

The project uses OData to let user create arbitrary query. For example,
`GET http://localhost:5000/api/ohunt/submissions?oj=zoj&$filter=userName eq 'vjudge' and status eq 'Accepted'&$count=true&$top=0` can get the total ac count of the user in ZOJ.

## Development

### Requirement
- .Net Core 3.1
- MySQL 8.0

Update connection string in `appsettings.json` to match your settings. Do not forget to create a database.

After started, it is deployed in `http://localhost:5000`.

## Environment Variables

### `DisableCrawlerWorker`
If it is set to `all`, no crawler worker is started.
