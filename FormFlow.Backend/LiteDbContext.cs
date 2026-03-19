using LiteDB;
using System;

public class LiteDbContext
{
    private readonly ILiteDatabase _database;

    public LiteDbContext(ILiteDatabase database)
    {
        _database = database;
    }

}