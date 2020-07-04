OHunt
==========================

A stateful crawler with a database.

The project uses OData to let user create arbitrary query. For example,
`GET http://localhost:5000/api/ohunt/submissions?oj=zoj&$filter=UserName eq 'vjudge' and Status eq 'Accepted'&$count=true&$top=0` can get the total ac count of the user in ZOJ.

## Environment Variables

### `DisableCrawlerWorker`
If it is set to `all`, no crawler worker is started.
