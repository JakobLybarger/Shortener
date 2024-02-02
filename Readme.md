# Shortener

A URL shortener written in C#.

## Purpose

The purpose of this project is to expand my capabilities when it comes to technical concepts relating to building upon the foundation I have when it comes to C#, .NET, APIs, testing, and databases. As well as dive into other technical concepts such as caching, performance, and optimization techniques, and potentially so cloud environment stuff outside the realm of past exposures.

## Example usage

### Shorten a URL

#### Request

```
curl -X POST -H "Content-Type: application/json" -d "{\"url\": \"https://www.example.com\"}" http://localhost:5249/shorten
```

#### Response

```
{
    "url": "http://localhost:8080/PXDONQ"
}
```

### Shorten a customized URL

#### Request

```
curl -X POST -H "Content-Type: application/json" -d "{\"url\": \"https://www.example.com\", \"customshorturl\": \"ABCDEF\"}" http://localhost:5249/shorten
```

#### Response

```
{
    "url": "http://localhost:5249/ABCDEF"
}
```

### Redirect

```
curl -X GET http://localhost:5249/PXDONQ
```
