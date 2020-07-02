OHunt
==========================

A stateful crawler with a database.

The project uses OData to let user create arbitrary query. For example,
`GET http://localhost:5000/api/submissions/oj/zoj?$filter=UserName eq `vjudge` and Status eq `Accepted`&$count` can get the total ac count of the user.

## Environment Variables

### `DisableCrawlerWorker`
If it is set to `all`, no crawler worker is started.
